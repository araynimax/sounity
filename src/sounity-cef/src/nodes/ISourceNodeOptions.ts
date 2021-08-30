export default interface ISourceNodeOptions {
  // general
  volume?: number;
  outputType?: 'sfx' | 'music';
  loop?: boolean;

  // position
  posX?: number;
  posY?: number;
  posZ?: number;

  // rotation
  rotX?: number;
  rotY?: number;
  rotZ?: number;

  // 3d sound options
  panningModel?: PanningModelType;
  distanceModel?: DistanceModelType;
  maxDistance?: number;
  refDistance?: number;
  rolloffFactor?: number;
  coneInnerAngle?: number;
  coneOuterAngle?: number;
  coneOuterGain?: number;
}
