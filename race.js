const thisMenu = 'race';
const thisHTML = 'FeST1VaL';
const thisMenuCall = global.festMenu;
const thisOscarMenuCall = global.oscar2Menu;

const callbackOnClose = () => {
  thisMenuCall.call('CEF::race:closeOnCallback');
};

global.raceMenu = {
  open(data) {
    thisMenuCall.call('race::openMenu', data);
    global.anyMenuHTML.openAnyMenu(thisHTML, thisMenu, callbackOnClose);
  },
  openPrize(data, hasSelectedCar, selectedCarIndex, position) {
    thisMenuCall.call('race::openPrize', data, hasSelectedCar, selectedCarIndex, position);
    global.anyMenuHTML.openAnyMenu(thisHTML, thisMenu, callbackOnClose);
  },
  close() {
    thisMenuCall.call('race::closeMenu');
    callbackOnClose();
    global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
  },
  info(data) {
    thisMenuCall.call('race::updateInfo', data);
  },
};

mp.events.add('CLIENT::race:startTimer', () => {
  thisMenuCall.call("CEF::raceTimer:start");
  global.anyMenuHTML.openAnyHUDElement(thisHTML, thisMenu);
});

mp.events.add('CLIENT::race:stopTimer', () => {
  thisMenuCall.call("CEF::raceTimer:end");
});

mp.events.add('CLIENT::race:getResultTimer', (elapsedTime) => {
  mp.events.callRemote("SERVER::race:end", elapsedTime);
  global.anyMenuHTML.closeAnyHUDElement(thisHTML, thisMenu);
});

mp.events.add('raceSpawnCar', (name) => {
  global.anyEvents.SendServer(() => mp.events.callRemote('raceMenuSpawnCar', name));
});

mp.events.add('raceGivePrizeCar', (model) => {
  global.anyEvents.SendServer(() => mp.events.callRemote('race:givePrizeCar', model));
});

mp.events.add('raceOpenMenu', (data) => {
  global.raceMenu.open(data);
});

mp.events.add('raceOpenPrizeMenu', (data, hasSelectedCar, selectedCarIndex, position) => {
  global.raceMenu.openPrize(data, hasSelectedCar, selectedCarIndex, position);
});

mp.events.add('raceCloseMenu', () => {
  global.raceMenu.close();
});

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
//     brakes = (mp.game.vehicle.getVehicleModelMaxBraking(model) * 100).toFixed(2);
//     boost = (mp.game.vehicle.getVehicleModelAcceleration(model) * 100).toFixed(2);
//     clutch = (mp.game.vehicle.getVehicleModelMaxTraction(model) * 10).toFixed(2);
//   } else {
//     speed = (mp.game.vehicle.getVehicleModelMaxSpeed(localplayer.vehicle.model) / 1.2).toFixed();
//     brakes = (localplayer.vehicle.getMaxBraking() * 100).toFixed(2);
//     boost = (localplayer.vehicle.getAcceleration() * 100).toFixed(2);
//     clutch = (localplayer.vehicle.getMaxTraction() * 10).toFixed(2);
//   }
//
//   return [
//     speed,
//     brakes,
//     boost,
//     clutch,
//   ];
// }

function getLocalCharacteristics() {
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
}

mp.events.add('raceGetCarCharacteristics', (model) => {
  const characteristics = global.init.getCharacteristics(model);
  thisMenuCall.call('race::updateCarCharacteristics', characteristics[0], characteristics[1], characteristics[2], characteristics[3]);
});

mp.events.add('raceGetAllCarsCharacteristics', (cars, type = 1) => {
  try {
    if (typeof cars !== 'object') cars = JSON.parse(cars);

    const newCarList = [];

    for (const car of Object.values(cars)) {
      const char = global.init.getCharacteristics(car.model);
      car.characteristics = {};
      car.characteristics.speed = char[0];
      car.characteristics.brakes = char[1];
      car.characteristics.boost = char[2];
      car.characteristics.clutch = char[3];

      newCarList.push(car);
    }

    switch (type) {
      case 1:
        thisMenuCall.call('race::updateCars', newCarList);
        break;
      case 2:
        thisMenuCall.call('race::updatePrizeCars', newCarList);
        break;
    }
  } catch (e) {
    logger.error(e);
  }
});
