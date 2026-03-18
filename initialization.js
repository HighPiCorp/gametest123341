global.init = {
  getMinimapAnchor: function () {
    let sfX = 1.0 / 20.0;
    let sfY = 1.0 / 20.0;
    let safeZone = mp.game.graphics.getSafeZoneSize();
    let aspectRatio = mp.game.graphics.getScreenAspectRatio(false);
    let resolution = mp.game.graphics.getScreenActiveResolution(0, 0);
    let scaleX = 1.0 / resolution.x;
    let scaleY = 1.0 / resolution.y;

    let minimap = {
      width: scaleX * (resolution.x / (4 * aspectRatio)),
      height: scaleY * (resolution.y / 5.674),
      scaleX: scaleX,
      scaleY: scaleY,
      leftX: scaleX * (resolution.x * (sfX * (Math.abs(safeZone - 1.0) * 10))),
      bottomY: 1.0 - scaleY * (resolution.y * (sfY * (Math.abs(safeZone - 1.0) * 10))),
    };

    minimap.rightX = minimap.leftX + minimap.width;
    minimap.topY = minimap.bottomY - minimap.height;
    return minimap;
  },
  formatIntZero: function (num, length) {

    return ("0" + num).slice(length);
  },
  getCharacteristics: function (model = null) {
    let speed;
    let brakes;
    let boost;
    let clutch;

    if (model !== null) {
      model = mp.game.joaat(model);

      speed = (mp.game.vehicle.getVehicleModelMaxSpeed(model) / 1.2).toFixed();
      brakes = (mp.game.vehicle.getVehicleModelMaxBraking(model) * 100).toFixed(2);
      boost = (mp.game.vehicle.getVehicleModelAcceleration(model) * 100).toFixed(2);
      clutch = (mp.game.vehicle.getVehicleModelMaxTraction(model) * 10).toFixed(2);
    } else {
      speed = (mp.game.vehicle.getVehicleModelMaxSpeed(localplayer.vehicle.model) / 1.2).toFixed();
      brakes = (localplayer.vehicle.getMaxBraking() * 100).toFixed(2);
      boost = (localplayer.vehicle.getAcceleration() * 100).toFixed(2);
      clutch = (localplayer.vehicle.getMaxTraction() * 10).toFixed(2);
    }

    return [
      speed,
      brakes,
      boost,
      clutch,
    ];
  },
  getCameraOffset: function (pos, angle, dist) {
    // mp.gui.chat.push(`Sin: ${Math.sin(angle)} | Cos: ${Math.cos(angle)}`);

    angle *= 0.0174533;

    pos.y += dist * Math.sin(angle);
    pos.x += dist * Math.cos(angle);

    // mp.gui.chat.push(`X: ${pos.x} | Y: ${pos.y}`);

    return pos;
  },
  CLIENT__RENT_getCharacteristics: function (thisMenuCall, thisHTML, thisMenu, callbackOnClose, data) {
    try {
      if (typeof data !== 'object') data = JSON.parse(data);

      const newVehiclesList = [];

      for (const vehicle of Object.values(data.list)) {
        // mp.console.logInfo(`vehicle: ${JSON.stringify(vehicle)}`);
        const char = global.init.getCharacteristics(vehicle.model);
        vehicle.characteristics = {};
        vehicle.characteristics.speed = char[0];
        vehicle.characteristics.break = char[1];
        vehicle.characteristics.boost = char[2];
        vehicle.characteristics.clutch = char[3];

        newVehiclesList.push(vehicle);
      }

      data.list = newVehiclesList;

      // mp.console.logInfo(`newVehiclesList: ${JSON.stringify(data)}`);
      thisMenuCall.call('CEF::rent:update', data);
      global.anyMenuHTML.openAnyMenu(thisHTML, thisMenu, callbackOnClose);
    } catch (e) {
      logger.error(e);
    }
  },
  getLocalCharacteristics: function (model = null) {
    const speed = (mp.game.vehicle.getVehicleModelMaxSpeed(model) / 1.2).toFixed();
    const brakes = (localplayer.vehicle.getMaxBraking() * 100).toFixed(2);
    const boost = (localplayer.vehicle.getAcceleration() * 100).toFixed(2);
    const clutch = (localplayer.vehicle.getMaxTraction() * 10).toFixed(2);

    return [
      speed,
      brakes,
      boost,
      clutch,
    ];
  },
  getMapAddress: function (pos)
  {
    let street = mp.game.pathfind.getStreetNameAtCoord(pos.x, pos.y, pos.z, 0, 0);
    let area = mp.game.zone.getNameOfZone(pos.x, pos.y, pos.z);

    return mp.game.ui.getLabelText(area) + ", " + mp.game.ui.getStreetNameFromHashKey(street.streetName);
  },
  playFocusSound: function () {
    mp.events.call('playFrontEndSound', "NAV_UP_DOWN", "HUD_FRONTEND_DEFAULT_SOUNDSET");
  },

  playBackSound: function () {
    mp.events.call('playFrontEndSound', "CANCEL", "HUD_FRONTEND_DEFAULT_SOUNDSET");
  },

  playSelectSound: function () {
    mp.events.call('playFrontEndSound', "SELECT", "HUD_FRONTEND_DEFAULT_SOUNDSET");
  }
}
