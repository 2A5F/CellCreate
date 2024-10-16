export type ShaderMeta = {
  id: string;
  type: "shader";
  pass: Record<string, ShaderPass>;
};

export type ShaderPass =
  & {
    [K in (typeof ShaderStages)[number]]?: string;
  }
  & { [key: string]: unknown };

export const ShaderStages = [
  "cs",
  "vs",
  "ps",
  "ms",
  "ts",
] as const;
