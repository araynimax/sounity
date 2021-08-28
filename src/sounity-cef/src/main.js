import SounitySoundManager from "./SounitySoundManager";

const sounitySoundManager = new SounitySoundManager();
window.sounitySoundManager = sounitySoundManager;

setInterval(() => sounitySoundManager._tick(), 0);

const handlers = {
    update(data) {
        sounitySoundManager.moveListener(data.posX, data.posY, data.posZ);
        sounitySoundManager.rotateListener(data.rotX, data.rotY, data.rotZ);
        sounitySoundManager.setVolume(data.sfxVolume, data.musicVolume);
        sounitySoundManager.setUnderwater(data.underwater);
    },

    moveSound(data) {
        sounitySoundManager.moveSound(data.identifier, data.posX, data.posY, data.posZ, data.underwater);
    },

    rotateSound(data) {
        sounitySoundManager.rotateSound(data.identifier, data.rotX, data.rotY, data.rotZ);
    },

    createSound(data) {
        sounitySoundManager.createSound(data.identifier, data.source, data.options, data.underwater);
    },

    startSound(data) {
        sounitySoundManager.startSound(data.identifier, data.startTime);
    },

    stopSound(data) {
        sounitySoundManager.pauseSound(data.identifier);
    },

    disposeSound(data) {
        sounitySoundManager.disposeSound(data.identifier);
    }
}

window.addEventListener('message', (event) => {
    if (event.data.type in handlers) handlers[event.data.type](event.data);
});

fetch(`https://${GetParentResourceName()}/sounity:ready`, {
    method: 'POST',
});