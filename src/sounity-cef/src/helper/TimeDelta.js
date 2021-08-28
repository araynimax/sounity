export default class TimeDelta {
    constructor() {
        this._currentTime = Date.now();
    }

    getDelta() {
        const newTime = Date.now();
        const diff = (newTime - this._currentTime) / 1000;
        this._currentTime = newTime;

        return diff;
    }
}