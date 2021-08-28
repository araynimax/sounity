import TimeDelta from "./helper/TimeDelta";
import SounitySoundManager from "./SounitySoundManager";
import { principalAxesToOrientation } from './helper/CalcOrientation';

// not in use @todo 
export const ESourceType = Object.freeze({
    DIRECT_SOURCE_URL: 'DIRECT_SOURCE_URL',
    YOUTUBE: 'YOUTUBE',
    YOUTUBE_LIVE: 'YOUTUBE_LIVE',
});

export const EOutputType = Object.freeze({
    SFX: 'sfx',
    MUSIC: 'music',
});


export default class SounitySound {
    /**
     * 
     * @param {string} identifier 
     * @param {string} source 
     * @param {*} options 
     * @param {SounitySoundManager} soundManager 
     */
    constructor(identifier, source, {
        volume = 1,        // volume of sound in percent
        outputType = EOutputType.SFX, // will apply volume level from settings,
        loop = false,      // music restarts on end

        // sound pos
        posX = 0,
        posY = 0,
        posZ = 0,

        // sound rotation (orientation)
        rotX = 0,
        rotY = 0,
        rotZ = 0,

        // other 3d options
        panningModel = "HRTF",
        distanceModel = "inverse",
        maxDistance = 500,
        refDistance = 3,
        rolloffFactor = 1,
        coneInnerAngle = 360,
        coneOuterAngle = 360,
        coneOuterGain = 0,

    } = {}, soundManager) {
        this.identifier = identifier;
        this.source = source;

        this.outputType = outputType;

        this.move(posX, posY, posZ);

        this.rotate(rotX, rotY, rotZ);

        this.soundManager = soundManager;
        this.audioCtx = soundManager.audioCtx;

        this.timeDelta = new TimeDelta();

        this.audio = new Audio(this.source);
        this.audio.crossOrigin = "anonymous";
        this.audio.preload = "metadata";
        this.audio.loop = loop;
        this.loop = loop;
        this.duration = null;
        this.audio.addEventListener('durationchange', () => {
            this.duration = this.audio.duration;
        });

        this.audioSource = this.audioCtx.createMediaElementSource(this.audio);

        this.volume = volume;
        this.gainNode = this.audioCtx.createGain();
        this.gainNode.gain.value = this._getVolume();

        this.pannerNode = this.audioCtx.createPanner();
        this.pannerNode.panningModel = panningModel;
        this.pannerNode.distanceModel = distanceModel;
        this.pannerNode.maxDistance = maxDistance;
        this.pannerNode.refDistance = refDistance;
        this.pannerNode.rolloffFactor = rolloffFactor;
        this.pannerNode.coneInnerAngle = coneInnerAngle;
        this.pannerNode.coneOuterAngle = coneOuterAngle;
        this.pannerNode.coneOuterGain = coneOuterGain;

        this.pannerNode.positionX.value = this.posX;
        this.pannerNode.positionY.value = this.posY;
        this.pannerNode.positionZ.value = this.posZ;

        this.pannerNode.orientationX.value = this.rotX;
        this.pannerNode.orientationY.value = this.rotY;
        this.pannerNode.orientationZ.value = this.rotZ;

        // effects
        this.isUnderwater = false;
        this.underwaterEffectNode = this.audioCtx.createBiquadFilter();
        this.underwaterEffectNode.frequency.value = this.audioCtx.sampleRate / 2;

        this.audioSource.connect(this.gainNode);
        this.gainNode.connect(this.pannerNode);
        this.pannerNode.connect(this.underwaterEffectNode);
        this.underwaterEffectNode.connect(this.soundManager.globalGainNode);
    }

    _getVolume() {
        if (this.outputType === EOutputType.SFX)
            return this.soundManager.sfxVolume / 10 * this.volume;

        if (this.outputType === EOutputType.MUSIC)
            return this.soundManager.musicVolume / 10 * this.volume;

        return this.volume;
    }

    start(startTime) {
        if (startTime) {
            if (this.duration === null) return this.audio.addEventListener('durationchange', () => this.start(startTime));
            if (this.duration !== Infinity) {
                const startTimeInSec = (startTime / 1000);
                if (startTimeInSec >= this.duration && this.loop === true) {
                    this.setCurrentTime(startTimeInSec % this.duration)
                } else if (startTimeInSec < this.duration) {
                    this.setCurrentTime(startTimeInSec);
                } else {
                    return; // dont start! the audio is already finished
                }
            }
        }

        this.audio.play();
    }

    stop() {
        this.audio.pause();
        this.audio.currentTime = 0;
    }

    setCurrentTime(newTime) {
        this.audio.currentTime = newTime;
    }

    setUnderwater(toggle) {
        if (this.isUnderwater === toggle) return;
        this.isUnderwater = toggle;
        this.underwaterEffectNode.frequency.cancelAndHoldAtTime(0);
        this.underwaterEffectNode.frequency.linearRampToValueAtTime(this.isUnderwater ? 100 : this.audioCtx.sampleRate / 2, .5);
    }

    move(posX, posY, posZ) {
        this.posX = posX;
        this.posY = posZ;
        this.posZ = posY;
    }

    rotate(rotX, rotY, rotZ) {
        const { forward } = principalAxesToOrientation(rotZ * -1, rotX, rotY);

        this.rotX = forward.x;
        this.rotY = forward.y;
        this.rotZ = forward.z;
    }

    _tick() {
        const endTime = this.audioCtx.currentTime + this.timeDelta.getDelta();

        this.pannerNode.positionX.linearRampToValueAtTime(this.posX, endTime);
        this.pannerNode.positionY.linearRampToValueAtTime(this.posY, endTime);
        this.pannerNode.positionZ.linearRampToValueAtTime(this.posZ, endTime);

        this.pannerNode.orientationX.linearRampToValueAtTime(this.rotX, endTime);
        this.pannerNode.orientationY.linearRampToValueAtTime(this.rotY, endTime);
        this.pannerNode.orientationZ.linearRampToValueAtTime(this.rotZ, endTime);

        this.gainNode.gain.linearRampToValueAtTime(this._getVolume(), endTime);
    }

    dispose() {
        this.audioSource.disconnect();
    }
}