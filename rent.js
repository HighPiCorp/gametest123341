const thisMenu = 'rent';
const thisHTML = 'Oscar2';
const thisMenuCall = global.oscar2Menu;

const callbackOnClose = () => {
  thisMenuCall.call('CEF::rent:closeOnCallback');
};

// eslint-disable-next-line camelcase
// function CLIENT__RENT_getCharacteristics(data) {
//   try {
//     if (typeof data !== 'object') data = JSON.parse(data);
//
//     const newVehiclesList = [];
//
//     for (const vehicle of Object.values(data.list)) {
//       // mp.console.logInfo(`vehicle: ${JSON.stringify(vehicle)}`);
//       const char = getCharacteristics(vehicle.model);
//       vehicle.characteristics = {};
//       vehicle.characteristics.speed = char[0];
//       vehicle.characteristics.break = char[1];
//       vehicle.characteristics.boost = char[2];
//       vehicle.characteristics.clutch = char[3];
//
//       newVehiclesList.push(vehicle);
//     }
//
//     data.list = newVehiclesList;
//
//     // mp.console.logInfo(`newVehiclesList: ${JSON.stringify(data)}`);
//     thisMenuCall.call('CEF::rent:update', data);
//     global.anyMenuHTML.openAnyMenu(thisHTML, thisMenu, callbackOnClose);
//   } catch (e) {
//     logger.error(e);
//   }
// }

mp.events.add('CLIENT::rent:update', (data) => {
  if (!global.loggedin) return;

  global.init.CLIENT__RENT_getCharacteristics(thisMenuCall, thisHTML, thisMenu, callbackOnClose, data);
});

mp.events.add('CLIENT::rent:hide', () => {
  global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
});

mp.events.add('CLIENT::rent:spawn', (model) => {
  global.anyEvents.SendServer(() => mp.events.callRemote('SERVER::rent:spawn', model));
});
