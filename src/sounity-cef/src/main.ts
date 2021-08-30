import SountiyController, { FilterType } from './SounityController';

const sounityController = new SountiyController();

// for debugging
//@ts-ignore
window.sounityController = sounityController;

const handlers = {
  update(data) {
    sounityController.moveListener(data.posX, data.posY, data.posZ);
    sounityController.rotateListener(data.rotX, data.rotY, data.rotZ);
    sounityController.setVolume(data.sfxVolume, data.musicVolume);
  },

  moveSound(data) {
    sounityController.moveSound(data.identifier, data.posX, data.posY, data.posZ);
  },

  rotateSound(data) {
    sounityController.rotateSound(data.identifier, data.rotX, data.rotY, data.rotZ);
  },

  createSound(data) {
    sounityController.createSound(data.identifier, data.source, data.options);
  },

  startSound(data) {
    sounityController.startSound(data.identifier, data.startTime);
  },

  stopSound(data) {
    sounityController.stopSound(data.identifier);
  },

  disposeSound(data) {
    sounityController.disposeSound(data.identifier);
  },

  addSoundFilter(data) {
    sounityController.addSoundFilter(data.identifier, data.filterName);
  },

  removeSoundFilter(data) {
    sounityController.removeSoundFilter(data.identifier, data.filterName);
  },

  addListenerFilter(data) {
    sounityController.addListenerFilter(data.filterName);
  },

  removeListenerFilter(data) {
    sounityController.removeListenerFilter(data.filterName);
  },

  createFilter(data) {
    sounityController.createFilter(data.filterName, data.filterType, data.options);
  },
};

window.addEventListener('message', (event) => {
  if (event.data.type in handlers) handlers[event.data.type](event.data);
});

// @ts-ignore
fetch(`https://${GetParentResourceName()}/sounity:ready`);
