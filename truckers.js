const thisMenu = 'truckers';
const thisHTML = 'FeST1VaL';
const thisMenuCall = global.festMenu;

const LOADING_MARKERS = [
  new mp.Vector3(2786.1199, 1715.2411, 24.56988), // benz
  new mp.Vector3(), // auto
  new mp.Vector3(2695.351, 3460.388, 56.22948), // guns
  new mp.Vector3(2681.651, 3507.565, 53.303883), // clothes
  new mp.Vector3(93.09571, 6334.708, 31.375862), // products
];

let loadingMarker = null;
let loadingLabel = null;
let maxLoad = 0;
let loaded = 0;

global.handBox = false;

const callbackOnClose = () => {
  thisMenuCall.call('trucker:closeOnCallback');
  mp.events.callRemote('SERVER:TRUCKER:close_orders_menu');
};

mp.events.add('trucker::userData', (data) => {
  thisMenuCall.call('trucker::userData', data);
});

mp.events.add('trucker::close', () => {
  global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
  callbackOnClose();
});

mp.events.add('trucker::endWorkDay', () => {
  global.anyEvents.SendServer(() => mp.events.callRemote('SERVER:TRUCKER:endWorkDay'));
});

mp.events.add('trucker::open', (data) => {
  thisMenuCall.call('trucker::open', data);
  global.anyMenuHTML.openAnyMenu(thisHTML, thisMenu, callbackOnClose);
});

mp.events.add('trucker::ADD_ORDER', (index, data) => {
  thisMenuCall.call('trucker::ADD_ORDER', index, data);
});

mp.events.add('trucker::accept_gosorder', (orderId) => {
  global.anyEvents.SendServer(() => mp.events.callRemote('SERVER:TRUCKER:take_gov_order', orderId));
  global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
});

mp.events.add('trucker::accept_order', (orderId) => {
  global.anyEvents.SendServer(() => mp.events.callRemote('SERVER:TRUCKER:take_order', orderId));
  global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
});

mp.events.add('trucker::handBox', (toggle) => {
  global.handBox = toggle;
});

mp.events.add('trucker::update_text_label', (box, unload = false) => {
  loaded = box;

  if (!unload) loadingLabel.text = `Загружено ${loaded} / ${maxLoad}`;
  else loadingLabel.text = `Выгружено ${loaded} / ${maxLoad}`;
});

mp.events.add('trucker::create_loading_marker', (type, maxBox, loadBox) => {
  if (loadingMarker != null) loadingMarker.destroy();

  if (loadingLabel != null) loadingLabel.destroy();

  maxLoad = maxBox;
  loaded = loadBox;

  loadingMarker = mp.markers.new(
    0,
    LOADING_MARKERS[type],
    1,
    {
      rotation: new mp.Vector3(0, 0, 0),
      visible: true,
      dimension: 0,
      range: 2,
    },
  );

  loadingLabel = mp.labels.new(
    `Загружено ${loaded} / ${maxLoad}`,
    LOADING_MARKERS[type],
    {
      los: true,
      font: 0,
      drawDistance: 100,
    },
  );
});

mp.events.add('trucker::create_unloading_marker', (position, maxBox, loadBox) => {
  if (loadingLabel != null) loadingLabel.destroy();

  if (loadingMarker != null) loadingMarker.destroy();

  maxLoad = maxBox;
  loaded = loadBox;

  loadingMarker = mp.markers.new(
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

  loadingLabel = mp.labels.new(
    `Выгружено ${loaded} / ${maxLoad}`,
    position,
    {
      los: true,
      font: 0,
      drawDistance: 100,
    },
  );
});

mp.events.add('trucker::remove_loading_marker', () => {
  if (loadingMarker != null) loadingMarker.destroy();

  if (loadingLabel != null) loadingLabel.destroy();

  loadingMarker = null;
  loadingLabel = null;
});

global.unloadVeh = null;

mp.events.add('CLIENT::TRUCKERS:SET_UNLOAD_VEH', (veh) => {
  global.unloadVeh = veh;
});

mp.events.add('CLIENT::TRUCKERS:OFF_UNLOAD_VEH', () => {
  global.unloadVeh = null;
});
