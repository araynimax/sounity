export default class Clock {
  private currentTime: number;

  constructor() {
    this.currentTime = Date.now();
  }

  getDelta() {
    const newTime = Date.now();
    const diff = (newTime - this.currentTime) / 1000;
    this.currentTime = newTime;

    return diff;
  }
}
