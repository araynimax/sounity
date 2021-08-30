import { EventEmitter } from 'events';

export default abstract class SounityBaseNode {
  private outputNode: AudioNode;
  private connectedToOutput: AudioNode;
  private eventEmitter = new EventEmitter();

  public on(type: string, listener: (...args: any[]) => void) {
    this.eventEmitter.on(type, listener);
  }

  public emit(type: string, ...args: any[]) {
    return this.eventEmitter.emit(type, ...args);
  }

  public once(type: string, listener: (...args: any[]) => void) {
    this.eventEmitter.once(type, listener);
  }

  protected setOutputNode(node: AudioNode) {
    if (this.connectedToOutput) {
      this.outputNode.disconnect(this.connectedToOutput);
      node.connect(this.connectedToOutput);
    }

    this.outputNode = node;
  }

  connect(node: AudioNode) {
    if (this.connectedToOutput) {
      this.outputNode.disconnect(this.connectedToOutput);
    }

    if (this.outputNode) {
      this.outputNode.connect(node);
    }

    this.connectedToOutput = node;
  }

  disconnect() {
    if (this.outputNode) {
      this.outputNode.disconnect();
    }

    this.connectedToOutput = null;
  }

  abstract tick(endTime: number): void;
}
