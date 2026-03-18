class MaskCamera {
  constructor() {
    this.cam = null;
    this.camStart = null;
  }

  init() {
    this.camStart = localplayer.position;
    const camValues = { Angle: localplayer.getRotation(2).z + 90, Dist: 0.7, Height: 0.6 };
    const pos = global.init.getCameraOffset(new mp.Vector3(this.camStart.x, this.camStart.y, this.camStart.z + camValues.Height), camValues.Angle, camValues.Dist);

    this.cam = mp.cameras.new('default', pos, new mp.Vector3(0, 0, 0), 50);
    this.cam.pointAtCoord(this.camStart.x, this.camStart.y, this.camStart.z + camValues.Height);
    this.cam.setActive(true);

    mp.game.cam.renderScriptCams(true, false, 500, true, false);
  }

  destroy() {
    this.cam.destroy();
    mp.game.cam.renderScriptCams(false, false, 500, true, false);

    mp.events.callRemote('cancelMasks');
  }
}

const maskCamera = new MaskCamera();

exports = maskCamera;
