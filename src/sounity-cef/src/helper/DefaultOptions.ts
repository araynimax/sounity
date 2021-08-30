import ISourceNodeOptions from '../nodes/ISourceNodeOptions';

const DefaultOptions: ISourceNodeOptions = {};

//@ts-ignore
fetch(`https://${GetParentResourceName()}/sounity:get-defaults`)
  .then((response) => response.text())
  .then((text) => JSON.parse(JSON.parse(text)))
  .then((json_defaults) => Object.keys(json_defaults).forEach((key) => (DefaultOptions[key] = json_defaults[key])));

export function Get<T extends ISourceNodeOptions, K extends keyof T>(key: K, optionsValue: T[K], fallback: T[K]): T[K] {
  if (optionsValue !== undefined) return optionsValue;
  // @ts-ignore
  if (key in DefaultOptions) return DefaultOptions[key];

  return fallback;
}
