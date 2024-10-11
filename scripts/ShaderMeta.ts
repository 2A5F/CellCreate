export type ShaderMeta = {
  id: string;
  type: "shader";
  pass: Record<string, ShaderPass>;
};

export type ShaderPass = {
  cs?: string;
  vs?: string;
  ps?: string;
  ms?: string;
  ts?: string;
};
