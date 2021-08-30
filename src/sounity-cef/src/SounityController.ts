import { principalAxesToOrientation } from './helper/CalcOrientation';
import Clock from './helper/Clock';
import ISourceNodeOptions from './nodes/ISourceNodeOptions';
import SounityOutputNode from './nodes/SounityOutputNode';
import SounitySoundNode, { ESounitySourceNodeState } from './nodes/SounitySoundNode';

//window.addEventListener('message', (event) => event.data.type in events && events[event.data.type](event.data));

export default class SountiyController {
  private soundNodes: Record<string, SounitySoundNode> = {};
  private audioCtx: AudioContext;
  private clock: Clock = new Clock();

  // listener position
  private posX: number = 0;
  private posY: number = 0;
  private posZ: number = 0;

  // listener orientation
  private forwardX: number = 1;
  private forwardY: number = 0;
  private forwardZ: number = 0;
  private upX: number = 0;
  private upY: number = 1;
  private upZ: number = 0;

  // volumes
  private masterVolume: number = 1;
  private sfxVolume: number = 1;
  private musicVolume: number = 1;

  private tickId: number;

  private outputNode: SounityOutputNode;

  constructor() {
    this.audioCtx = new AudioContext();

    this.outputNode = new SounityOutputNode(this);
    this.outputNode.connect(this.audioCtx.destination);

    this.tickId = setInterval(() => {
      this.tick();
    });
  }

  public moveListener(x: number, y: number, z: number) {
    this.posX = x;
    this.posY = z;
    this.posZ = y;
  }

  public rotateListener(x, y, z) {
    const { forward, up } = principalAxesToOrientation(z, x, y);

    this.forwardX = forward.x;
    this.forwardY = forward.y * -1;
    this.forwardZ = forward.z;

    this.upX = up.x;
    this.upY = up.y;
    this.upZ = up.z;
  }

  public setVolume(sfxVolume: number, musicVolume: number) {
    this.sfxVolume = sfxVolume;
    this.musicVolume = musicVolume;
  }

  public getAudioCtx() {
    return this.audioCtx;
  }

  private getSoundNode(identifier: string): SounitySoundNode {
    if (identifier in this.soundNodes) return this.soundNodes[identifier];

    throw new Error(`There does not exists a sound with given identifier '${identifier}'`);
  }

  public createSound(identifier: string, url: string, options: ISourceNodeOptions): void {
    if (identifier in this.soundNodes)
      throw new Error(`There already exists a sound with given identifier '${identifier}'`);

    const soundNode = new SounitySoundNode(identifier, url, options, this);

    soundNode.connect(this.outputNode.getInputNode());

    this.soundNodes[identifier] = soundNode;
  }

  public startSound(identifier: string, startTime?: number): void {
    this.getSoundNode(identifier).start(startTime);
  }

  public stopSound(identifier: string): void {
    this.getSoundNode(identifier).stop();
  }

  public setCurrentTime(identifier: string, newTime: number): void {
    this.getSoundNode(identifier).setCurrentTime(newTime);
  }

  public moveSound(identifier: string, x: number, y: number, z: number): void {
    this.getSoundNode(identifier).move(x, y, z);
  }

  public rotateSound(identifier: string, x: number, y: number, z: number): void {
    this.getSoundNode(identifier).rotate(x, y, z);
  }

  public disposeSound(identifier: string): void {
    this.getSoundNode(identifier).dispose();
  }

  private tick() {
    const endTime = this.audioCtx.currentTime + this.clock.getDelta();

    // controller tick logic

    this.audioCtx.listener.positionX.linearRampToValueAtTime(this.posX, endTime);
    this.audioCtx.listener.positionY.linearRampToValueAtTime(this.posY, endTime);
    this.audioCtx.listener.positionZ.linearRampToValueAtTime(this.posZ, endTime);

    this.audioCtx.listener.forwardX.linearRampToValueAtTime(this.forwardX, endTime);
    this.audioCtx.listener.forwardY.linearRampToValueAtTime(this.forwardY, endTime);
    this.audioCtx.listener.forwardZ.linearRampToValueAtTime(this.forwardZ, endTime);

    this.audioCtx.listener.upX.linearRampToValueAtTime(this.upX, endTime);
    this.audioCtx.listener.upY.linearRampToValueAtTime(this.upY, endTime);
    this.audioCtx.listener.upZ.linearRampToValueAtTime(this.upZ, endTime);

    // tick nodes that are ready
    for (const soundNode of Object.values(this.soundNodes)) {
      if (soundNode.getState() !== ESounitySourceNodeState.SETUP) soundNode.tick(endTime);
    }

    // tick output node
    this.outputNode.tick(endTime);
  }
}
