import SounityBaseNode from './SounityBaseNode';
import SountiyController from '../SounityController';

export default class SounityOutputNode extends SounityBaseNode {
  private audioCtx: AudioContext;

  private masterVolumeGainNode: GainNode;

  public constructor(controller: SountiyController) {
    super();

    this.audioCtx = controller.getAudioCtx();

    this.masterVolumeGainNode = this.audioCtx.createGain();

    this.setOutputNode(this.masterVolumeGainNode);
  }

  public getInputNode(): AudioNode {
    return this.masterVolumeGainNode;
  }

  public tick(endTime: number) {}
}
