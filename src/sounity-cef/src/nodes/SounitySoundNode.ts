import { principalAxesToOrientation } from '../helper/CalcOrientation';
import * as DefaultOptions from '../helper/DefaultOptions';
import ISourceNodeOptions from './ISourceNodeOptions';
import SounityBaseNode from './SounityBaseNode';
import SountiyController from '../SounityController';
import SoundLoader from '../helper/SoundLoader';

export enum ESounitySourceNodeState {
  SETUP,
  READY,
  PLAYING,
  FINISHED,
  ERROR,
}

export default class SounitySourceNode extends SounityBaseNode {
  private state: ESounitySourceNodeState = ESounitySourceNodeState.SETUP;
  private options: ISourceNodeOptions;
  private identifier: string;
  private url: string;
  private outputType: 'sfx' | 'music';
  private duration?: number;
  private volume: number;
  private loop: boolean;

  private posX: number;
  private posY: number;
  private posZ: number;

  private rotX: number;
  private rotY: number;
  private rotZ: number;

  private audioBuffer: AudioBuffer;
  private audioBufferSourceNode: AudioBufferSourceNode;
  private volumeGainNode: GainNode;
  private pannerNode: PannerNode;

  public constructor(
    identifier: string,
    url: string,
    options: ISourceNodeOptions,
    sounityController: SountiyController
  ) {
    super(sounityController);

    this.identifier = identifier;
    this.url = url;
    this.sounityController = sounityController;
    this.audioCtx = sounityController.getAudioCtx();
    this.options = options;
    this.loop = DefaultOptions.Get('loop', options.loop, false);

    this.outputType = DefaultOptions.Get('outputType', options.outputType, 'sfx');

    this.volume = DefaultOptions.Get('volume', options.volume, 1);

    this.move(
      DefaultOptions.Get('posX', options.posX, 0),
      DefaultOptions.Get('posY', options.posY, 0),
      DefaultOptions.Get('posZ', options.posZ, 0)
    );

    this.rotate(
      DefaultOptions.Get('rotX', options.rotX, 0),
      DefaultOptions.Get('rotY', options.rotY, 0),
      DefaultOptions.Get('rotZ', options.rotZ, 0)
    );

    this.setup();
  }

  private async setup() {
    const promiseStack: Promise<void>[] = [];

    this.audioBuffer = await this.audioCtx.decodeAudioData(await SoundLoader.getInstance().loadUrl(this.url));

    this.audioBufferSourceNode = this.audioCtx.createBufferSource();
    this.audioBufferSourceNode.loop = DefaultOptions.Get('loop', this.options.loop, false);
    this.audioBufferSourceNode.buffer = this.audioBuffer;
    this.duration = this.audioBuffer.duration;

    this.volumeGainNode = this.audioCtx.createGain();
    this.volumeGainNode.gain.value = this.getVolume();

    this.pannerNode = this.audioCtx.createPanner();
    this.pannerNode.panningModel = DefaultOptions.Get('panningModel', this.options.panningModel, 'HRTF');
    this.pannerNode.distanceModel = DefaultOptions.Get('distanceModel', this.options.distanceModel, 'inverse');
    this.pannerNode.maxDistance = DefaultOptions.Get('maxDistance', this.options.maxDistance, 500);
    this.pannerNode.refDistance = DefaultOptions.Get('refDistance', this.options.refDistance, 3);
    this.pannerNode.rolloffFactor = DefaultOptions.Get('rolloffFactor', this.options.refDistance, 1);
    this.pannerNode.coneInnerAngle = DefaultOptions.Get('coneInnerAngle', this.options.coneInnerAngle, 360);
    this.pannerNode.coneOuterAngle = DefaultOptions.Get('coneOuterAngle', this.options.coneOuterAngle, 0);
    this.pannerNode.coneOuterGain = DefaultOptions.Get('coneOuterGain', this.options.coneOuterGain, 0);

    this.pannerNode.positionX.value = this.posX;
    this.pannerNode.positionY.value = this.posY;
    this.pannerNode.positionZ.value = this.posZ;

    this.pannerNode.orientationX.value = this.rotX;
    this.pannerNode.orientationY.value = this.rotY;
    this.pannerNode.orientationZ.value = this.rotZ;

    this.audioBufferSourceNode.connect(this.volumeGainNode);
    this.volumeGainNode.connect(this.pannerNode);
    this.setOutputNode(this.pannerNode);

    try {
      await Promise.all(promiseStack);
      this.setState(ESounitySourceNodeState.READY);
    } catch (err) {
      console.error(err);
      this.setState(ESounitySourceNodeState.ERROR);
    }
  }

  private setState(state: ESounitySourceNodeState) {
    this.state = state;
    if (state === ESounitySourceNodeState.READY) this.emit('ready');
    this.emit('statechange');
  }

  public getState(): ESounitySourceNodeState {
    return this.state;
  }

  start(startTime?: number) {
    if (this.state === ESounitySourceNodeState.ERROR) return;

    if (this.state === ESounitySourceNodeState.SETUP) {
      return this.once('ready', () => this.start(startTime));
    }

    let offset = 0;

    if (startTime !== undefined && this.duration !== Infinity) {
      const startTimeInSec = startTime / 1000;
      if (startTimeInSec >= this.duration && this.loop === true) {
        offset = startTimeInSec % this.duration;
      } else if (startTimeInSec < this.duration) {
        offset = startTimeInSec;
      } else {
        return; // dont start! the audio is already finished
      }
    }

    this.audioBufferSourceNode.start(0, offset);
  }

  public move(x: number, y: number, z: number): void {
    this.posX = x;
    this.posY = z;
    this.posZ = y;
  }

  public rotate(x: number, y: number, z: number): void {
    const { forward } = principalAxesToOrientation(z * -1, x, y);

    this.rotX = forward.x;
    this.rotY = forward.y;
    this.rotZ = forward.z;
  }

  stop() {
    if (this.state === ESounitySourceNodeState.ERROR) return;

    if (this.state === ESounitySourceNodeState.SETUP) {
      return this.once('ready', () => this.stop());
    }

    this.audioBufferSourceNode.stop(0);
  }

  dispose() {
    this.disconnect();
  }

  private getVolume() {
    if (this.outputType === 'music') {
      return this.volume * this.sounityController.getMusicVolume();
    } else if (this.outputType === 'sfx') {
      return this.volume * this.sounityController.getSfxVolume();
    }

    return this.volume;
  }

  public tick(endTime: number) {
    this.pannerNode.positionX.linearRampToValueAtTime(this.posX, endTime);
    this.pannerNode.positionY.linearRampToValueAtTime(this.posY, endTime);
    this.pannerNode.positionZ.linearRampToValueAtTime(this.posZ, endTime);

    this.pannerNode.orientationX.linearRampToValueAtTime(this.rotX, endTime);
    this.pannerNode.orientationY.linearRampToValueAtTime(this.rotY, endTime);
    this.pannerNode.orientationZ.linearRampToValueAtTime(this.rotZ, endTime);

    this.volumeGainNode.gain.linearRampToValueAtTime(this.getVolume(), endTime);
  }
}
