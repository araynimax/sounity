import TimeDelta from "./helper/TimeDelta";
import SounitySound from "./SounitySound";
import { principalAxesToOrientation } from './helper/CalcOrientation';

export default class SounitySoundManager {
    constructor() {
        /**@type{Record<string, SounitySound>} */
        this.sounds = {};

        this.audioCtx = new AudioContext();

        this.timeDelta = new TimeDelta();

        // position
        this.listenerPosX = 0;
        this.listenerPosY = 0;
        this.listenerPosZ = 0;

        // orientation
        this.listenerForwardX = 1;
        this.listenerForwardY = 0;
        this.listenerForwardZ = 0;

        this.listenerUpX = 0;
        this.listenerUpY = 1;
        this.listenerUpZ = 0;

        this.masterVolume = 1;
        this.sfxVolume = 1;
        this.musicVolume = 1;

        this.globalUnderwaterEffectNode = this.audioCtx.createBiquadFilter();
        this.globalUnderwaterEffectNode.frequency.value = this.audioCtx.sampleRate / 2;

        this.globalGainNode = this.audioCtx.createGain();
        this.globalGainNode.gain.value = 1;
        this.globalGainNode.connect(this.globalUnderwaterEffectNode);
        this.globalUnderwaterEffectNode.connect(this.audioCtx.destination);
    }

    moveListener(posX, posY, posZ) {
        this.listenerPosX = posX;
        this.listenerPosY = posZ;
        this.listenerPosZ = posY;
    }

    rotateListener(rotX, rotY, rotZ) {
        const { forward, up } = principalAxesToOrientation(rotZ, rotX, rotY);

        this.listenerForwardX = forward.x;
        this.listenerForwardY = forward.y * -1;
        this.listenerForwardZ = forward.z;

        this.listenerUpX = up.x;
        this.listenerUpY = up.y;
        this.listenerUpZ = up.z;
    }

    setVolume(sfxVolume, musicVolume) {
        this.sfxVolume = sfxVolume;
        this.musicVolume = musicVolume;
    }
    setUnderwater(toggle) {
        if (this.isUnderwater === toggle) return;
        this.isUnderwater = toggle;
        this.globalUnderwaterEffectNode.frequency.cancelAndHoldAtTime(0);
        this.globalUnderwaterEffectNode.frequency.linearRampToValueAtTime(this.isUnderwater ? 40 : this.audioCtx.sampleRate / 2, .5);
    }

    _tick() {
        for (const soundId in this.sounds) {
            this.sounds[soundId]._tick();
        }

        const endTime = this.audioCtx.currentTime + this.timeDelta.getDelta();

        this.audioCtx.listener.positionX.linearRampToValueAtTime(this.listenerPosX, endTime);
        this.audioCtx.listener.positionY.linearRampToValueAtTime(this.listenerPosY, endTime);
        this.audioCtx.listener.positionZ.linearRampToValueAtTime(this.listenerPosZ, endTime);

        this.audioCtx.listener.forwardX.linearRampToValueAtTime(this.listenerForwardX, endTime);
        this.audioCtx.listener.forwardY.linearRampToValueAtTime(this.listenerForwardY, endTime);
        this.audioCtx.listener.forwardZ.linearRampToValueAtTime(this.listenerForwardZ, endTime);

        this.audioCtx.listener.upX.linearRampToValueAtTime(this.listenerUpX, endTime);
        this.audioCtx.listener.upY.linearRampToValueAtTime(this.listenerUpY, endTime);
        this.audioCtx.listener.upZ.linearRampToValueAtTime(this.listenerUpZ, endTime);
    }


    createSound(identifier, source, options, underwater) {
        if (identifier in this.sounds)
            throw new Error(`There already exists a sound with given identifier '${identifier}'`);

        this.sounds[identifier] = new SounitySound(identifier, source, options, this);

        if (underwater !== undefined) {
            this.sounds[identifier].setUnderwater(underwater);
        }

        return this;
    }

    _getSound(identifier) {
        if (identifier in this.sounds)
            return this.sounds[identifier];

        throw new Error(`There does not exists a sound with given identifier '${identifier}'`);
    }

    startSound(identifier, startTime) {
        const sound = this._getSound(identifier);

        sound.start(startTime);

        return this;
    }

    stopSound(identifier) {
        const sound = this._getSound(identifier);

        sound.stop();

        return this;
    }

    setCurrentTime(identifier, newTime) {
        const sound = this._getSound(identifier);

        sound.setCurrentTime(newTime);

        return this;
    }

    moveSound(identifier, posX, posY, posZ, underwater) {
        const sound = this._getSound(identifier);

        sound.move(posX, posY, posZ);

        if (underwater !== undefined) {
            sound.setUnderwater(underwater);
        }

        return this;
    }

    rotateSound(identifier, rotX, rotY, rotZ) {
        const sound = this._getSound(identifier);

        sound.rotate(rotX, rotY, rotZ);

        return this;
    }

    enableFilter(identifier) {
        const sound = this._getSound(identifier);

        sound.enableFilter();

        return this;
    }

    changeFilterOptions(identifier, filterOptions) {
        const sound = this._getSound(identifier);

        sound.changeFilterOptions(filterOptions);

        return this;
    }

    disableFilter(identifier) {
        const sound = this._getSound(identifier);

        sound.disableFilter();

        return this;
    }

    disposeSound(identifier) {
        const sound = this._getSound(identifier);

        sound.dispose();

        delete this.sounds[identifier];
    }

}