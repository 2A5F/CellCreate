import * as path from "https://deno.land/std@0.224.0/path/mod.ts";

console.log(
  "================================================== start build dotnet =================================================="
);

const [build_output, proj, debug] = Deno.args;

const is_debug = debug.toLowerCase() == "d";

const output_path = path.resolve(build_output, "bin", "managed");
Deno.mkdirSync(output_path, { recursive: true });

const cmd = new Deno.Command("dotnet", {
  cwd: Deno.cwd(),
  args: [
    "build",
    proj,
    ...(is_debug ? [] : ["-c", "Release"]),
    "-o",
    output_path,
  ],
  env: {
    DOTNET_CLI_UI_LANGUAGE: "en-us",
  },
  stdout: "inherit",
  stderr: "inherit",
});
cmd.outputSync();

console.log(
  "================================================== end build dotnet =================================================="
);
