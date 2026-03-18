const thisMenu = 'CarRoom';
const thisHTML = 'Oscar';
const thisMenuCall = global.new_menu;

const AUTOCOLORS = [
  'Красный',
  'Малиновый',
  'Оранжевый',
  'Желтый',
  'Зеленый',
  'Мятный',
  'Синий',
  'Голубой',
  'Фиолетовый',
  'Черный',
  'Серый',
  'Белый',
].reverse();

const COLORS = {
  'Черный': [0, 0, 0],
  "Белый": [225, 225, 225],
  "Серый": [144, 144, 144],
  "Красный": [230, 0, 0],
  "Малиновый": [220, 20, 60],
  "Оранжевый": [255, 115, 0],
  "Желтый": [240, 240, 0],
  "Зеленый": [0, 230, 0],
  "Мятный": [144, 255, 119],
  "Голубой": [0, 205, 255],
  "Синий": [0, 0, 230],
  "Фиолетовый":  [190, 60, 165],
};

const VEH_POSITION = new mp.Vector3(832.75964, -1384.0072, -48.802334);
const VEH_ROTATION = new mp.Vector3(0, 0, 140.10686);
const CAM_POSITION = new mp.Vector3(832.21490000, -1388.32300000, -46.99698000);
const CAM_ROTATION = new mp.Vector3(0, 0, -6.542761);

let autoModels = [];
let autoPrices = [];
let autoTrunks = [];
let autoTrunksWeight = [];
let autoFuel = [];
let loadCamera = false;

let camAuto = null;
const auto = {
  model: null,
  color: null,
  colorSecondary: null,
  entity: null,
};

let testDrive = {
  carIndex: -1,
  carColor1: -1,
  carColor2: -1,
};

const cameraRotator = require('public/utils/cameraRotator');

async function createCam(pos, rot, viewangle) {
  camAuto = mp.cameras.new('default');

  if (!camAuto.doesExist() || camAuto == null || camAuto == undefined) await mp.game.waitAsync(20);

  camAuto.setCoord(pos.x, pos.y, pos.z);
  camAuto.setRot(rot.x, rot.y, rot.z, 2);
  camAuto.setFov(viewangle);
  camAuto.setActive(true);

  cameraRotator.start(camAuto, VEH_POSITION, VEH_POSITION, new mp.Vector3(-3.0, 6.5, 0.5), -295);
  cameraRotator.setZBound(-0.8, 2.0);
  cameraRotator.setLBound(-4, 3);
  cameraRotator.setZUpMultipler(5);
  cameraRotator.pause(true);

  mp.game.cam.renderScriptCams(true, false, 3000, true, false);

  loadCamera = true;
}

// function getCharacteristics(model = null) {
//   let speed;
//   let brakes;
//   let boost;
//   let clutch;
//
//   if (model !== null) {
//     model = mp.game.joaat(model);
//
//     speed = (mp.game.vehicle.getVehicleModelMaxSpeed(model) / 1.2).toFixed();
//     brakes = (mp.game.vehicle.getVehicleModelMaxBraking(model) * 100).toFixed();
//     boost = (mp.game.vehicle.getVehicleModelAcceleration(model) * 100).toFixed();
//     clutch = (mp.game.vehicle.getVehicleModelMaxTraction(model) * 10).toFixed();
//   } else {
//     speed = (mp.game.vehicle.getVehicleModelMaxSpeed(localplayer.vehicle.model) / 1.2).toFixed();
//     brakes = (localplayer.vehicle.getMaxBraking() * 100).toFixed();
//     boost = (localplayer.vehicle.getAcceleration() * 100).toFixed();
//     clutch = (localplayer.vehicle.getMaxTraction() * 10).toFixed();
//   }
//
//   return [
//     speed,
//     brakes,
//     boost,
//     clutch,
//   ];
// }

function setCharacteristics(index) {
  const carInfo = global.init.getCharacteristics(autoModels[index].model);

  thisMenuCall.call('autoshow::updateCarInfo', JSON.stringify({
    speed: carInfo[0],
    clutch: carInfo[3],
    boost: carInfo[2],
    break: carInfo[1],
    fuel: autoFuel[index],
    trunk: autoTrunksWeight[index],
    price: autoPrices[index],
  }));
}

mp.events.add('autoshow::changeColors', (colorIndex) => {
  auto.color = AUTOCOLORS[colorIndex];
  auto.entity.setCustomPrimaryColour(COLORS[AUTOCOLORS[colorIndex]][0], COLORS[AUTOCOLORS[colorIndex]][1], COLORS[AUTOCOLORS[colorIndex]][2]);
});

mp.events.add('autoshow::changeSecondColors', (colorIndex) => {
  auto.colorSecondary = AUTOCOLORS[colorIndex];
  auto.entity.setCustomSecondaryColour(COLORS[AUTOCOLORS[colorIndex]][0], COLORS[AUTOCOLORS[colorIndex]][1], COLORS[AUTOCOLORS[colorIndex]][2]);
});

mp.events.add('autoshow::changeCar', (autoIndex) => {
  auto.model = autoModels[autoIndex].model;

  setCharacteristics(autoIndex);

  auto.entity.model = mp.game.joaat(autoModels[autoIndex].model);

  auto.entity.setCustomPrimaryColour(COLORS[auto.color][0], COLORS[auto.color][1], COLORS[auto.color][2]);
  auto.entity.setCustomSecondaryColour(COLORS[auto.colorSecondary][0], COLORS[auto.colorSecondary][1], COLORS[auto.colorSecondary][2]);
  auto.entity.setDirtLevel(0.0);
});

const callbackOnClose = () => {
  thisMenuCall.call('autoshow:closeOnCallback');
  mp.events.callRemote('carroomCancel');

  if (auto.entity != null) {
    auto.entity.destroy();
    auto.entity = null;
  }

  cameraRotator.stop();

  if (camAuto != null) {
    camAuto.destroy();
    camAuto = null;
  }

  testDrive = {
    carIndex: -1,
    carColor1: -1,
    carColor2: -1,
  };

  loadCamera = false;

  mp.game.cam.renderScriptCams(false, false, 500, true, false);
};

mp.events.add('autoshow::close', () => {
  if (new Date().getTime() - global.lastCheck < 50) return;
  global.lastCheck = new Date().getTime();
  global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);

  callbackOnClose();
});

mp.events.add('autoshow::buyCar', (payType) => {
  if (new Date().getTime() - global.lastCheck < 500) return;
  global.lastCheck = new Date().getTime();

  global.anyEvents.SendServer(() => mp.events.callRemote('carroomBuy', auto.model, auto.color, auto.colorSecondary, payType));
});

mp.events.add('autoshow::onClickTestDrive', (autoIndex) => {
  if (new Date().getTime() - global.lastCheck < 500) return;
  global.lastCheck = new Date().getTime();

  testDrive = {
    carIndex: autoIndex,
    carColor1: AUTOCOLORS.indexOf(auto.color),
    carColor2: AUTOCOLORS.indexOf(auto.colorSecondary),
  };

  mp.events.callRemote('carroomTestDrive', auto.model.toString(), JSON.stringify(COLORS[auto.color]), JSON.stringify(COLORS[auto.colorSecondary]));
});

mp.events.add('autoshow::destroyCamera', () => {
  if (auto.entity != null) {
    auto.entity.destroy();
    auto.entity = null;
  }

  global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);

  cameraRotator.stop();

  if (camAuto != null) {
    camAuto.destroy();
    camAuto = null;
  }

  mp.players.local.freezePosition(false);

  loadCamera = false;

  mp.game.cam.renderScriptCams(false, false, 500, true, false);
});

mp.events.add('openAuto', async (data) => {
  if (global.menuCheck()) return;

  const info = JSON.parse(data);

  autoModels = info.models;
  autoPrices = info.prices;
  autoTrunks = info.trunks;
  autoFuel = info.fueltank;
  autoTrunksWeight = info.trunksWeight;

  createCam(CAM_POSITION, CAM_ROTATION, 50);

  cameraRotator.pause(false);

  auto.color = testDrive.carColor1 == -1 ? AUTOCOLORS[0] : AUTOCOLORS[testDrive.carColor1];
  auto.colorSecondary = testDrive.carColor2 == -1 ? AUTOCOLORS[0] : AUTOCOLORS[testDrive.carColor2];

  while (!loadCamera) await mp.game.waitAsync(50);

  auto.entity = mp.vehicles.new(
    testDrive.carIndex == -1 ? mp.game.joaat(autoModels[0].model) : mp.game.joaat(autoModels[testDrive.carIndex].model),
    VEH_POSITION.subtract(new mp.Vector3(0, 0, 1)),
    {
      heading: VEH_ROTATION.z,
      numberPlate: 'CARROOM',
      alpha: 255,
      color: [COLORS[auto.color], COLORS[auto.colorSecondary]],
      locked: false,
      engine: false,
      dimension: localplayer.dimension,
    },
  );

  auto.entity.setRotation(VEH_ROTATION.x, VEH_ROTATION.y, VEH_ROTATION.z, 2, true);
  auto.entity.freezePosition(true);
  auto.entity.setCustomPrimaryColour(COLORS[auto.color][0], COLORS[auto.color][1], COLORS[auto.color][2]);
  auto.entity.setCustomSecondaryColour(COLORS[auto.colorSecondary][0], COLORS[auto.colorSecondary][1], COLORS[auto.colorSecondary][2]);
  auto.entity.setDirtLevel(0.0);

  if (testDrive.carIndex == -1) auto.model = autoModels[0].model;
  else auto.model = autoModels[testDrive.carIndex].model;

  thisMenuCall.call('autoshow::updateCar', JSON.stringify({
    show: true,
    auto: autoModels,
    moneyType: info.moneyType,
  }));

  setCharacteristics(testDrive.carIndex == -1 ? 0 : testDrive.carIndex);

  global.anyMenuHTML.openAnyMenu(thisHTML, thisMenu, callbackOnClose);
});
// mp.events.add('screenFadeIn', (duration) => {
//   try {
//     mp.game.cam.doScreenFadeIn(duration);
//   } catch (e) {
//
//   }
// });
//
// mp.events.add('screenFadeOut', (duration) => {
//   try {
//     mp.game.cam.doScreenFadeOut(duration);
//   } catch (e) {
//
//   }
// });
