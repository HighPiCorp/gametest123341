const thisMenu = 'barber';
const thisHTML = 'Oscar2';
const thisMenuCall = global.oscar2Menu;

const hairIDList = [
  // male
  [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 30, 31, 32, 33, 34, 73, 76, 77, 78, 79, 80, 81, 82, 84, 85],
  // female
  [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 31, 76, 77, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 90, 91],
];

const bodyCamValues = {
  hair: { Angle: 0, Dist: 0.5, Height: 0.7 },
  beard: { Angle: 0, Dist: 0.5, Height: 0.7 },
  eyebrows: { Angle: 0, Dist: 0.5, Height: 0.7 },
  chesthair: { Angle: 0, Dist: 1, Height: 0.2 },
  lenses: { Angle: 0, Dist: 0.5, Height: 0.7 },
  lipstick: { Angle: 0, Dist: 0.5, Height: 0.7 },
  blush: { Angle: 0, Dist: 0.5, Height: 0.7 },
  makeup: { Angle: 0, Dist: 0.5, Height: 0.7 },
};

let bodyCam = null;
let bodyCamStart = new mp.Vector3(0, 0, 0);
let cameraLoaded = false;

const barberValues = {
  'hair': { Style: 0, Color: 0, Index:0 },
  'beard': { Style: 255, Color: 0, Index:0 },
  'eyebrows': { Style: 255, Color: 0, Index:0 },
  'chesthair': { Style: 255, Color: 0, Index:0 },
  'lenses': { Style: 0, Color: 0, Index:0 },
  'lipstick': { Style: 255, Color: 0, Index:0 },
  'blush': { Style: 255, Color: 0, Index:0 },
  'makeup': { Style: 255, Color: 0, Index:0 },
};

const barberIds = ['hair', 'beard', 'eyebrows', 'chesthair', 'lenses', 'lipstick', 'blush', 'makeup'];
const cameraRotator = require('public/utils/cameraRotator');

async function BarberCameraInit() {
  /*const camValues = bodyCamValues.hair;
  const pos = getCameraOffset(new mp.Vector3(bodyCamStart.x, bodyCamStart.y, bodyCamStart.z + camValues.Height), localplayer.getRotation(2).z + 90 + camValues.Angle, camValues.Dist);

  bodyCam = mp.cameras.new('default', pos, new mp.Vector3(0, 0, 0), 50);
  bodyCam.pointAtCoord(bodyCamStart.x, bodyCamStart.y, bodyCamStart.z + camValues.Height);
  bodyCam.setActive(true);
  mp.game.cam.renderScriptCams(true, false, 500, true, false);


  cameraRotator.start(bodyCam, pos, new mp.Vector3(bodyCamStart.x, bodyCamStart.y, bodyCamStart.z + camValues.Height), new mp.Vector3(0.3, 0.3, 0.3), 0);
  cameraRotator.setZBound(-0.8, 2.0);
  cameraRotator.setLBound(-4, 3);
  cameraRotator.setZUpMultipler(5);

  cameraRotator.pause(false);*/

  // mp.events.call('camMenu', true);
  mp.game.cam.doScreenFadeOut(500);

  CreateCam();

  await mp.game.waitAsync(500);

  localplayer.position = new mp.Vector3(137.72, -1710.64, 28.60);

  localplayer.setHeading(237.22);

  const pos = localplayer.getOffsetFromInWorldCoords(0.0, 0 - 0.5, 0.5);
  localplayer.taskStartScenarioAtPosition("PROP_HUMAN_SEAT_CHAIR_MP_PLAYER", pos.x, pos.y, pos.z, localplayer.getHeading(), 0, true, false);

}


function CreateCam(){
  //var cam = mp.game.cam.createCamWithParams('DEFAULT_SCRIPTED_CAMERA', 138.45, -1711.05, 29.70, 0.0, 0.0, 45.00, 65.0, false, 2);

  bodyCam = mp.cameras.new('default', new mp.Vector3(138.45, -1711.05, 29.70), new mp.Vector3(0.0, 0.0, 55.00), 65.0);
  setTimeout(() => {
    mp.game.cam.doScreenFadeIn(500);

    bodyCam.setActive(true);
    mp.game.cam.renderScriptCams(true, false, 0, true, false);

    cameraLoaded = true;

   }, 1000);
}

function BarberCameraDestroy() {
  bodyCam.destroy();
  mp.game.cam.renderScriptCams(false, false, 500, true, false);
  cameraLoaded = false;
}

function BarberCameraChange(id) {
 /* const camValues = bodyCamValues[id];
  const camPos = getCameraOffset(new mp.Vector3(bodyCamStart.x, bodyCamStart.y, bodyCamStart.z + camValues.Height), localplayer.getRotation(2).z + 90 + camValues.Angle, camValues.Dist);

  bodyCam.setCoord(camPos.x, camPos.y, camPos.z);
  bodyCam.pointAtCoord(bodyCamStart.x, bodyCamStart.y, bodyCamStart.z + camValues.Height);*/
}

const callbackOnClose = () => {
  mp.game.cam.doScreenFadeOut(500);

  localplayer.clearDecorations();

  BarberCameraDestroy();
  mp.events.callRemote('SERVER::barber:closeMenu');

  thisMenuCall.call('CEF::barber:closeOnCallback');
  global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);

  setTimeout(() => {
    mp.game.cam.doScreenFadeIn(500);
  }, 1000);
};

mp.events.add('CLIENT::barber:openBarberMenu', async (productPrice, markup) => {
  const gender = localplayer.getVariable('GENDER');
  const genderBool = (gender) ? 'male' : 'female';
  let barberPricesList = JSON.parse(JSON.stringify(barberPrices));

  for (let i = 0; i < 8; i++) {
    const id = barberIds[i];
    const barberSkip = [];

    if (id === "hair") {
      for (let x = 0; x < barberPricesList[id][genderBool].length; x++) {
        // если нету цены, то берем предыдущую
        if (!barberPricesList[id][genderBool][x]) {
          // mp.console.logError(barberPricesList[id][x]);
          //1400 / 100 * 150 = 14 * 150
          barberPricesList[id][genderBool][x] = barberPricesList[id][genderBool][x - 1] / 100 * markup;
        } else {
          barberPricesList[id][genderBool][x] = barberPricesList[id][genderBool][x] / 100 * markup;
        }
      }
    } else {
      for (let x = 0; x < barberPricesList[id].length; x++) {
        // если нету цены, то берем предыдущую
        if (!barberPricesList[id][x]) {
          // mp.console.logError(barberPricesList[id][x]);
          barberPricesList[id][x] = barberPricesList[id][x - 1] / 100 * markup;
        } else {
          barberPricesList[id][x] = barberPricesList[id][x] / 100 * markup;
        }
      }
    }


  }

  bodyCamStart = localplayer.position;

  await BarberCameraInit();

  thisMenuCall.call('CEF::barber:update', {
    show: true,
    styles: barberStyles[genderBool],
    colors: barberColors,
    prices: barberPricesList,
    gender: genderBool,
  });

  global.anyMenuHTML.openAnyMenu(thisHTML, thisMenu, callbackOnClose);
});

mp.events.add('CLIENT::barber:hide', async () => {
  while(!cameraLoaded){
    await mp.game.waitAsync(50);
  }
  callbackOnClose();
});

mp.events.add('CLIENT::barber:buyCash', (id) => {
  global.anyEvents.SendServer(() => mp.events.callRemote('SERVER::barber:buyBarber', 0, id, barberValues[id].Style, barberValues[id].Color, barberValues[id].Index));
  // mp.events.call('CLIENT::barber:hide');
});

mp.events.add('CLIENT::barber:buyCard', (id) => {
  global.anyEvents.SendServer(() => mp.events.callRemote('SERVER::barber:buyBarber', 1, id, barberValues[id].Style, barberValues[id].Color, barberValues[id].Index));
  // mp.events.call('CLIENT::barber:hide');
});

mp.events.add('CLIENT::barber:changeStyle', (id, val) => {
  switch (id) {
    case 'hair':
      // const gender = (localplayer.getVariable('GENDER')) ? 0 : 1;
      const gender = localplayer.getVariable('GENDER');
      const genderBool = (gender) ? 'male' : 'female';
      barberValues.hair.Style = barberStyles[genderBool]["hair"].list[val];
      barberValues.hair.Index = val;
      localplayer.setComponentVariation(2, barberValues.hair.Style, 0, 0);
      localplayer.setHairColor(barberValues.hair.Color, 0);
      break;
    case 'beard':
      // 255 default without beard
      barberValues.beard.Style = (val == 0) ? 255 : val;
      barberValues.beard.Index = val;
      localplayer.setHeadOverlay(1, barberValues.beard.Style, 100, barberValues.beard.Color, barberValues.beard.Color);
      break;
    case 'eyebrows':
      barberValues.eyebrows.Style = (val == 0) ? 255 : val - 1;
      barberValues.eyebrows.Index = val;
      localplayer.setHeadOverlay(2, barberValues.eyebrows.Style, 100, barberValues.eyebrows.Color, barberValues.eyebrows.Color);
      break;
    case 'chesthair':
      barberValues.chesthair.Style = (val == 0) ? 255 : val - 1;
      barberValues.chesthair.Index = val;
      localplayer.setHeadOverlay(10, barberValues.chesthair.Style, 100, barberValues.chesthair.Color, barberValues.chesthair.Color);
      break;
    case 'lenses':
      barberValues.lenses.Style = val;
      barberValues.lenses.Index = val;
      localplayer.setEyeColor(val);
      break;
    case 'lipstick':
      barberValues.lipstick.Style = (val == 0) ? 255 : val - 1;
      barberValues.lipstick.Index = val;
      localplayer.setHeadOverlay(8, barberValues.lipstick.Style, 100, barberValues.lipstick.Color, barberValues.lipstick.Color);
      break;
    case 'blush':
      barberValues.blush.Style = (val == 0) ? 255 : val - 1;
      barberValues.blush.Index = val;
      localplayer.setHeadOverlay(5, barberValues.blush.Style, 100, barberValues.blush.Color, barberValues.blush.Color);
      break;
    case 'makeup':
      barberValues.makeup.Style = (val == 0) ? 255 : val - 1;
      barberValues.makeup.Index = val;
      localplayer.setHeadOverlay(4, barberValues.makeup.Style, 100, barberValues.makeup.Color, barberValues.makeup.Color);
      break;
  }

  BarberCameraChange(id);
});

mp.events.add('CLIENT::barber:preChangeStyle', (id, val) => {
  let style;
  switch (id) {
    case 'hair':
      const gender = localplayer.getVariable('GENDER');
      const genderBool = (gender) ? 'male' : 'female';
      style = barberStyles[genderBool]["hair"].list[val];
      localplayer.setComponentVariation(2, style, 0, 0);
      localplayer.setHairColor(barberValues.hair.Color, 0);
      break;
    case 'beard':
      // 255 default without beard
      style = (val == 0) ? 255 : val;
      localplayer.setHeadOverlay(1, style, 100, barberValues.beard.Color, barberValues.beard.Color);
      break;
    case 'eyebrows':
      style = (val == 0) ? 255 : val - 1;
      localplayer.setHeadOverlay(2, style, 100, barberValues.eyebrows.Color, barberValues.eyebrows.Color);
      break;
    case 'chesthair':
      style = (val == 0) ? 255 : val - 1;
      localplayer.setHeadOverlay(10, style, 100, barberValues.chesthair.Color, barberValues.chesthair.Color);
      break;
    case 'lenses':
      localplayer.setEyeColor(val);
      break;
    case 'lipstick':
      style = (val == 0) ? 255 : val - 1;
      localplayer.setHeadOverlay(8, style, 100, barberValues.lipstick.Color, barberValues.lipstick.Color);
      break;
    case 'blush':
      style = (val == 0) ? 255 : val - 1;
      localplayer.setHeadOverlay(5, style, 100, barberValues.blush.Color, barberValues.blush.Color);
      break;
    case 'makeup':
      style = (val == 0) ? 255 : val - 1;
      localplayer.setHeadOverlay(4, style, 100, barberValues.makeup.Color, barberValues.makeup.Color);
      break;
  }

  // BarberCameraChange(id);
});

mp.events.add('CLIENT::barber:changeColor', (id, val) => {
  switch (id) {
    case 'hair':
      barberValues.hair.Color = val;
      localplayer.setComponentVariation(2, barberValues.hair.Style, 0, 0);
      localplayer.setHairColor(barberValues.hair.Color, 0);
      break;
    case 'beard':
      barberValues.beard.Color = val;
      localplayer.setHeadOverlay(1, barberValues.beard.Style, 100, barberValues.beard.Color, barberValues.beard.Color);
      break;
    case 'eyebrows':
      barberValues.eyebrows.Color = val;
      localplayer.setHeadOverlay(2, barberValues.eyebrows.Style, 100, barberValues.eyebrows.Color, barberValues.eyebrows.Color);
      break;
    case 'chesthair':
      barberValues.chesthair.Color = val;
      localplayer.setHeadOverlay(10, barberValues.chesthair.Style, 100, barberValues.chesthair.Color, barberValues.chesthair.Color);
      break;
    case 'lipstick':
      barberValues.lipstick.Color = val;
      localplayer.setHeadOverlay(8, barberValues.lipstick.Style, 100, barberValues.lipstick.Color, barberValues.lipstick.Color);
      break;
    case 'blush':
      barberValues.blush.Color = val;
      localplayer.setHeadOverlay(5, barberValues.blush.Style, 100, barberValues.blush.Color, barberValues.blush.Color);
      break;
  }
});

mp.events.add('CLIENT::barber:preChangeColor', (id, val) => {
  switch (id) {
    case 'hair':
      localplayer.setComponentVariation(2, barberValues.hair.Style, 0, 0);
      localplayer.setHairColor(val, 0);
      break;
    case 'beard':
      localplayer.setHeadOverlay(1, barberValues.beard.Style, 100, val, val);
      break;
    case 'eyebrows':
      localplayer.setHeadOverlay(2, barberValues.eyebrows.Style, 100, val, val);
      break;
    case 'chesthair':
      localplayer.setHeadOverlay(10, barberValues.chesthair.Style, 100, val, val);
      break;
    case 'lipstick':
      localplayer.setHeadOverlay(8, barberValues.lipstick.Style, 100, val, val);
      break;
    case 'blush':
      localplayer.setHeadOverlay(5, barberValues.blush.Style, 100, val, val);
      break;
  }
});
