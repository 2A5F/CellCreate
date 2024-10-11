import * as path from "https://deno.land/std@0.224.0/path/mod.ts";
import yaml from "npm:yaml";
import { exists } from "https://deno.land/std@0.224.0/fs/exists.ts";
import type { ShaderMeta } from "./ShaderMeta.ts";

console.log(
  "================================================== start build shader ==================================================",
);

const [binary_dir, shader_dir, d_r, dxc, re_tool] = Deno.args;

const is_debug = d_r == "D";

// console.log({ binary_dir, shader_dir, is_debug, dxc, re_tool });

async function* get_all_src(
  base: string,
): AsyncGenerator<{ path: string; name: string }, void, unknown> {
  for await (const item of Deno.readDir(base)) {
    if (item.isFile && path.extname(item.name) == ".meta") {
      yield { path: `${base}/${item.name}`, name: item.name };
    } else if (item.isDirectory) {
      yield* get_all_src(`${base}/${item.name}`);
    }
  }
}

const tmp_root_path = path.resolve(binary_dir, "./shader/tmp");
await Deno.mkdir(tmp_root_path, { recursive: true });

const meta_paths: {
  path: string;
  name: string;
}[] = [];
for await (const meta_path of get_all_src(shader_dir)) {
  meta_paths.push(meta_path);
}

const pack_items: string[] = [];
const manifest_item: Record<string, string> = {};

await Promise.all(meta_paths.map(build));
await create_manifest();
await copy_files();

console.log(
  "================================================== end build shader ==================================================",
);

// ================================================== functions ==================================================

async function build({
  path: meta_path,
  name: _meta_name,
}: {
  path: string;
  name: string;
}) {
  const meta: ShaderMeta = yaml.parse(await Deno.readTextFile(meta_path));
  if (!meta.id || meta.type != "shader") return;
  const tmp_path = path.resolve(tmp_root_path, meta.id);
  await Deno.mkdir(tmp_path, { recursive: true });

  manifest_item[
    path.relative(shader_dir, meta_path).replace("\\", "/").slice(0, -5)
  ] = meta.id;

  const shader_file = `${
    meta_path.substring(
      0,
      meta_path.length - path.extname(meta_path).length,
    )
  }.hlsl`;
  if (!(await exists(shader_file))) {
    console.warn(`${shader_file} not exists`);
    return;
  }

  const tasks: Promise<unknown>[] = [];

  for (const [pass_name, pass] of Object.entries(meta.pass)) {
    for (const [stage, entry] of Object.entries(pass)) {
      const output_dxil = path.resolve(tmp_path, `${pass_name}.${stage}.dxil`);
      const output_asm = path.resolve(tmp_path, `${pass_name}.${stage}.asm`);
      const output_re = path.resolve(tmp_path, `${pass_name}.${stage}.re.bin`);
      const output_re_json = path.resolve(tmp_path, `${pass_name}.${stage}.re`);

      const cmd_dxil = new Deno.Command(dxc, {
        cwd: Deno.cwd(),
        args: [
          "-T",
          `${stage}_6_6`,
          "-E",
          entry,
          ...(is_debug ? ["-Od", "-Zi", "-Qembed_debug"] : ["-O3", "-Zs"]),
          "-Fo",
          output_dxil,
          "-Fre",
          output_re,
          ...(is_debug ? ["-Fc", output_asm] : []),
          shader_file,
        ],
        stdout: "inherit",
        stderr: "inherit",
      });
      tasks.push(
        cmd_dxil.output().then(async () => {
          const cmd_re = new Deno.Command(re_tool, {
            cwd: Deno.cwd(),
            args: [output_re, output_re_json],
          });
          await cmd_re.output();
        }),
      );
      pack_items.push(output_dxil);
      pack_items.push(output_re_json);
      if (is_debug) {
        pack_items.push(output_asm);
      }
    }
  }

  {
    const meta_data = {
      passes: Object.fromEntries(
        Object.entries(meta.pass).map(([k, v]) => [k, Object.keys(v)]),
      ),
    };
    const meta_output_path = path.resolve(tmp_path, ".meta");
    tasks.push(Deno.writeTextFile(meta_output_path, JSON.stringify(meta_data)));
    pack_items.push(meta_output_path);
  }

  await Promise.all(tasks);
}

async function create_manifest() {
  const output_path = path.resolve(tmp_root_path, ".manifest");
  await Deno.writeTextFile(output_path, JSON.stringify(manifest_item));
  pack_items.unshift(output_path);
}

async function copy_files() {
  const target_path = path.resolve(binary_dir, "./bin/content/shaders");
  // if (await exists(target_path)) {
  //   await Deno.remove(target_path, { recursive: true });
  // }
  await Deno.mkdir(target_path, { recursive: true });
  await Promise.all(pack_items.map(async (src_path) => {
    const dst_path = path.resolve(
      target_path,
      path.relative(tmp_root_path, src_path),
    );
    const dir_path = path.dirname(dst_path);
    await Deno.mkdir(dir_path, { recursive: true });
    await Deno.copyFile(src_path, dst_path);
  }));
}
