const thisMenu = 'agency';
const thisHTML = 'Oscar2';
const thisMenuCall = global.oscar2Menu;
let blip = null;

const callbackOnClose = () => {
  thisMenuCall.call('CEF::agency:closeOnCallback');
};

mp.events.add('CLIENT::agency:open', (data) => {
  if (!global.loggedin) return;

  thisMenuCall.call('CEF::agency:open', data);
  global.anyMenuHTML.openAnyMenu(thisHTML, thisMenu, callbackOnClose);
});

mp.events.add('CLIENT::agency:close', () => {
  callbackOnClose();
  global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
});

mp.events.add('CLIENT::agency:getList', (type) => {
  global.anyEvents.SendServer(() => mp.events.callRemote('SERVER::agency:getList', type));
});

mp.events.add('CLIENT::agency:setList', (type, list) => {
  thisMenuCall.call('CEF::agency:setList', type, list);
});

mp.events.add('CLIENT::agency:getAddress', (address) => {
  if (typeof address !== 'object') address = JSON.parse(address);

  const street = mp.game.ui.getStreetNameFromHashKey(mp.game.pathfind.getStreetNameAtCoord(parseFloat(address.x), parseFloat(address.y), parseFloat(address.z), 0, 0).streetName);
  const label = mp.game.zone.getNameOfZone(parseFloat(address.x), parseFloat(address.y), parseFloat(address.z));
  const area = mp.game.ui.getLabelText(label);

  thisMenuCall.call('CEF::agency:setAddress', `${street}, ${area}`);
});

mp.events.add('CLIENT::agency:buyInfo', (type) => {
  global.anyEvents.SendServer(() => mp.events.callRemote('SERVER::agency:buyInfo', type));
});

mp.events.add('CLIENT::agency:createBlip', (position, type, id) => {
  if (blip != null) blip.destroy();

  blip = mp.markers.new(
    0,
    position,
    1,
    {
      rotation: new mp.Vector3(0, 0, 0),
      visible: true,
      dimension: 0,
      range: 2,
    },
  );

  global.anyEvents.SendServer(() => mp.events.callRemote('SERVER::agency:createColshape', type, id));
});

mp.events.add('CLIENT::agency:removeBlip', () => {
  if (blip != null) blip.destroy();

  blip = null;
});
