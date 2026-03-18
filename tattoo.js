const thisMenu = 'tattoo';
const thisHTML = 'Oscar2';
const thisMenuCall = global.oscar2Menu;

// Camera
const bodyCamValues = {
  torso: [
    { Angle: 0, Dist: 1, Height: 0.2 },
    { Angle: 0, Dist: 1, Height: 0.2 },
    { Angle: 0, Dist: 1, Height: 0.2 },
    { Angle: 180, Dist: 1, Height: 0.2 },
    { Angle: 180, Dist: 1, Height: 0.2 },
    { Angle: 180, Dist: 1, Height: 0.2 },
    { Angle: 180, Dist: 1, Height: 0.2 },
    { Angle: 305, Dist: 1, Height: 0.2 },
    { Angle: 55, Dist: 1, Height: 0.2 },
  ],
  head: [
    { Angle: 0, Dist: 1, Height: 0.5 },
    { Angle: 305, Dist: 1, Height: 0.5 },
    { Angle: 55, Dist: 1, Height: 0.5 },
    { Angle: 180, Dist: 1, Height: 0.5 },
    { Angle: 0, Dist: 0.5, Height: 0.5 },
    { Angle: 0, Dist: 0.5, Height: 0.5 },
  ],
  leftarm: [
    { Angle: 55, Dist: 1, Height: 0.0 },
    { Angle: 55, Dist: 1, Height: 0.1 },
    { Angle: 55, Dist: 1, Height: 0.1 },
  ],
  rightarm: [
    { Angle: 305, Dist: 1, Height: 0.0 },
    { Angle: 305, Dist: 1, Height: 0.1 },
    { Angle: 305, Dist: 1, Height: 0.1 },
  ],
  leftleg: [
    { Angle: 55, Dist: 1, Height: -0.6 },
    { Angle: 55, Dist: 1, Height: -0.6 },
  ],
  rightleg: [
    { Angle: 305, Dist: 1, Height: -0.6 },
    { Angle: 305, Dist: 1, Height: -0.6 },
  ],
};
let bodyCam = null;
let bodyCamStart = new mp.Vector3(0, 0, 0);

const tattooValues = [0, 0, 0, 0, 0, 0];
const tattooIds = ['torso', 'head', 'leftarm', 'rightarm', 'leftleg', 'rightleg'];

const cameraRotator = require('public/utils/cameraRotator');

function TattooCameraInit() {
  const camValues = bodyCamValues.torso[0];
  const pos = global.init.getCameraOffset(new mp.Vector3(bodyCamStart.x, bodyCamStart.y, bodyCamStart.z + camValues.Height), localplayer.getRotation(2).z + 90 + camValues.Angle, camValues.Dist);

  bodyCam = mp.cameras.new('default', pos, new mp.Vector3(0, 0, 0), 50);
  bodyCam.pointAtCoord(bodyCamStart.x, bodyCamStart.y, bodyCamStart.z + camValues.Height);
  bodyCam.setActive(true);
  mp.game.cam.renderScriptCams(true, false, 500, true, false);

  cameraRotator.start(bodyCam, bodyCamStart, bodyCamStart, new mp.Vector3(-1.0, 1.5, 0.5), -25);
  cameraRotator.setZBound(-0.8, 2.0);
  cameraRotator.setLBound(-4, 3);
  cameraRotator.setZUpMultipler(5);

  cameraRotator.pause(false);

  // mp.events.call('camMenu', true);
}

function TattooCameraDestroy() {
  cameraRotator.stop();
  bodyCam.destroy();
  mp.game.cam.renderScriptCams(false, false, 500, true, false);
}

const callbackOnClose = () => {
  localplayer.clearDecorations();
  mp.events.callRemote('SERVER::tattoo:closeMenu');

  thisMenuCall.call('CEF::tattoo:closeOnCallback');

  TattooCameraDestroy();
};

mp.events.add('CLIENT::tattoo:openTattooMenu', (productPrice, bizMarkup) => {
  try {
    const gender = localplayer.getVariable('GENDER');
    const tattooPrices = [];
    let tattooPricesList = JSON.parse(JSON.stringify(tattoos));

    for (let i = 0; i < 6; i++) {
      const id = tattooIds[i];

      for (let x = 0; x < tattooPricesList[id].length; x++) {
        // const hash = (gender) ? tattooPricesList[id][x].MaleHash : tattooPricesList[id][x].FemaleHash;
        // if (hash === '') tattooPricesList[id][x].Skip = true;
        // else tattooPricesList[id][x].Skip = false;

        // eslint-disable-next-line no-mixed-operators
        tattooPricesList[id][x].Price = tattooPricesList[id][x].Price / 100 * bizMarkup;
        //tattooPrices[x] = tempPrice.toFixed();
      }

      bodyCamStart = new mp.Vector3(324.9798, 180.6418, 103.6665);
    }

    TattooCameraInit();

    // logger.debug(JSON.stringify(tattoos));
    thisMenuCall.call('CEF::tattoo:update', { show: true, tattoo: tattooPricesList, gender: gender});
    global.anyMenuHTML.openAnyMenu(thisHTML, thisMenu, callbackOnClose);
  }
  catch (e) {
    logger.error(e);
  }
});

mp.events.add('CLIENT::tattoo:hide', () => {
  callbackOnClose();
  global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
});

mp.events.add('CLIENT::tattoo:buyCash', (id) => {
//  mp.events.call('CLIENT::tattoo:hide');
  global.anyEvents.SendServer(() => mp.events.callRemote('SERVER::tattoo:buyTattoo', 0, tattooIds.indexOf(id), tattooValues[tattooIds.indexOf(id)]));
});

mp.events.add('CLIENT::tattoo:buyCard', (id) => {
//  mp.events.call('CLIENT::tattoo:hide');
  global.anyEvents.SendServer(() => mp.events.callRemote('SERVER::tattoo:buyTattoo', 1, tattooIds.indexOf(id), tattooValues[tattooIds.indexOf(id)]));
});

mp.events.add('CLIENT::tattoo:preChangeTattoo', (id, val) => {
  const tId = tattooIds.indexOf(id);
  tattooValues[tId] = val;

  const tattoo = tattoos[id][val];
  const hash = (localplayer.getVariable('GENDER')) ? tattoo.MaleHash : tattoo.FemaleHash;
  localplayer.clearDecorations();

  const playerTattoos = JSON.parse(localplayer.getVariable('TATTOOS'));

  // Очищаем ненужные татушки
  for (let x = 0; x < playerTattoos[tId].length; x++) {
    for (let i = 0; i < tattoo.Slots.length; i++) {
      if (playerTattoos[tId][x].Slots.indexOf(tattoo.Slots[i]) !== -1) {
        playerTattoos[tId][x] = null;
        break;
      }
    }
  }

  // Восстанавливаем старые татуировки игрока, кроме тех, которые занимают очищенные слоты
  for (let x = 0; x < 6; x++) {
    if (playerTattoos[x] != null) {
      for (let i = 0; i < playerTattoos[x].length; i++) {
        if (playerTattoos[x][i] != null) {
          localplayer.setDecoration(mp.game.joaat(playerTattoos[x][i].Dictionary), mp.game.joaat(playerTattoos[x][i].Hash));
        }
      }
    }
  }

  // Ну и применяем выбранную татуировку
  localplayer.setDecoration(mp.game.joaat(tattoo.Dictionary), mp.game.joaat(hash));

  const camValues = bodyCamValues[id][tattoo.Slots[0]];
  const camPos = global.init.getCameraOffset(new mp.Vector3(bodyCamStart.x, bodyCamStart.y, bodyCamStart.z + camValues.Height), localplayer.getRotation(2).z + 90 + camValues.Angle, camValues.Dist);

  bodyCam.setCoord(camPos.x, camPos.y, camPos.z);
  bodyCam.pointAtCoord(bodyCamStart.x, bodyCamStart.y, bodyCamStart.z + camValues.Height);
});

mp.events.add('CLIENT::tattoo:changeTattoo', (id, val) => {
  const tId = tattooIds.indexOf(id);
  tattooValues[tId] = val;

  const tattoo = tattoos[id][val];
  const hash = (localplayer.getVariable('GENDER')) ? tattoo.MaleHash : tattoo.FemaleHash;
  localplayer.clearDecorations();

  const playerTattoos = JSON.parse(localplayer.getVariable('TATTOOS'));

  // Очищаем ненужные татушки
  for (let x = 0; x < playerTattoos[tId].length; x++) {
    for (let i = 0; i < tattoo.Slots.length; i++) {
      if (playerTattoos[tId][x].Slots.indexOf(tattoo.Slots[i]) !== -1) {
        playerTattoos[tId][x] = null;
        break;
      }
    }
  }

  // Восстанавливаем старые татуировки игрока, кроме тех, которые занимают очищенные слоты
  for (let x = 0; x < 6; x++) {
    if (playerTattoos[x] != null) {
      for (let i = 0; i < playerTattoos[x].length; i++) {
        if (playerTattoos[x][i] != null) {
          localplayer.setDecoration(mp.game.joaat(playerTattoos[x][i].Dictionary), mp.game.joaat(playerTattoos[x][i].Hash));
        }
      }
    }
  }

  // Ну и применяем выбранную татуировку
  localplayer.setDecoration(mp.game.joaat(tattoo.Dictionary), mp.game.joaat(hash));

  const camValues = bodyCamValues[id][tattoo.Slots[0]];
  const camPos = global.init.getCameraOffset(new mp.Vector3(bodyCamStart.x, bodyCamStart.y, bodyCamStart.z + camValues.Height), localplayer.getRotation(2).z + 90 + camValues.Angle, camValues.Dist);

  bodyCam.setCoord(camPos.x, camPos.y, camPos.z);
  bodyCam.pointAtCoord(bodyCamStart.x, bodyCamStart.y, bodyCamStart.z + camValues.Height);
});

mp.events.addDataHandler('TATTOOS', function (entity, value, oldValue) {

  entity.clearDecorations();

  const playerTattoos = JSON.parse(value);

  // Восстанавливаем старые татуировки игрока, кроме тех, которые занимают очищенные слоты
  for (let x = 0; x < 6; x++) {
    if (playerTattoos[x] != null) {
      for (let i = 0; i < playerTattoos[x].length; i++) {
        if (playerTattoos[x][i] != null) {
          entity.setDecoration(mp.game.joaat(playerTattoos[x][i].Dictionary), mp.game.joaat(playerTattoos[x][i].Hash));
        }
      }
    }
  }
});
