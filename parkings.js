// global.menugg = mp.browsers["new"]('package://cef/UI/garbage/index.html');
const thisHTML = 'Oscar';
const thisMenuCall = global.new_menu;
let parkCheckpoint;
const thisMenu = 'parkings';

const callbackOnClose = () => {
  thisMenuCall.call('park:closeOnCallback');
  global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
};

mp.events.add('openParkingMenu', (data) => {
  global.new_menu.call('park::update', data);
  global.anyMenuHTML.openAnyMenu(thisHTML, thisMenu, callbackOnClose);
});

mp.events.add('park::hide', () => {
  global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
});

mp.events.add('park::cash', (keynum, place, hour) => {
  thisMenuCall.call('park:closeOnCallback');
  global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
  global.anyEvents.SendServer(() => mp.events.callRemote('rentParkingPlace', 0, keynum, place, hour));
});

mp.events.add('park::card', (keynum, place, hour) => {
  thisMenuCall.call('park:closeOnCallback');
  global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
  global.anyEvents.SendServer(() => mp.events.callRemote('rentParkingPlace', 1, keynum, place, hour));
});

mp.events.add('CLIENT::park:cancelRent', (num, num2) => {
  thisMenuCall.call('park:closeOnCallback');
  global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
  global.anyEvents.SendServer(() => mp.events.callRemote('SERVER::PARKING:CANCEL_PARK', parseInt(num)));
});

mp.events.add('createParkCheckpoint', (position) => {
  if (typeof parkCheckpoint !== 'undefined') {
    parkCheckpoint.destroy();
    parkCheckpoint = undefined;
  }
  parkCheckpoint = mp.markers.new(
    2,
    position,
    1,
    {
      rotation: new mp.Vector3(180, 0, 0),
      visible: true,
      dimension: 0,
      range: 2,
    },
  );
});

mp.events.add('deleteParkCheckpoint', () => {
  if (typeof parkCheckpoint === 'undefined') return;
  parkCheckpoint.destroy();
  parkCheckpoint = undefined;
});


