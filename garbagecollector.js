const thisMenu = 'garbage';
const thisHTML = 'Oscar';
const thisMenuCall = global.new_menu;

const callbackOnClose = () => {
  global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
};

const callbackOnCloseNPC = () => {
  thisMenuCall.call('CEF::dialog:closeOnCallback');
};



mp.events.add('CLIENT:GARBAGE:pick_up_bag', async (player) => {
  if (!mp.game.streaming.hasAnimDictLoaded('missfbi4prepp1')) {
    mp.game.streaming.requestAnimDict('missfbi4prepp1');

    do await mp.game.waitAsync(10);
    while (
      !mp.game.streaming.hasAnimDictLoaded(
        'missfbi4prepp1',
      )
    );
  }

  if (player && mp.players.exists(player)) {
    player.taskPlayAnim('missfbi4prepp1', '_bag_pickup_garbage_man', 4, -1000, -1, 39, 0, false, true, false);
    await mp.game.waitAsync(2000);
    player.taskPlayAnim('missfbi4prepp1', '_bag_walk_garbage_man', 4, -1000, -1, 49, 0, false, true, false);
  }
});

mp.events.add('CLIENT:GARBAGE:throw_bag', async (player) => {
  if (!mp.game.streaming.hasAnimDictLoaded('missfbi4prepp1')) {
    mp.game.streaming.requestAnimDict('missfbi4prepp1');

    do await mp.game.waitAsync(10);
    while (
      !mp.game.streaming.hasAnimDictLoaded(
        'missfbi4prepp1',
      )
    );
  }

  if (player && mp.players.exists(player)) {
    player.taskPlayAnim('missfbi4prepp1', '_bag_throw_garbage_man', 4, -1000, -1, 39, 0, false, true, false);
    await mp.game.waitAsync(2000);
    player.stopAnimTask('missfbi4prepp1', '_bag_throw_garbage_man', 3.0);
  }
});

let vehMarker = null;
let pointMarker = null;
let started = false;
let hit = false;
const hitPosition = new mp.Vector3(-589.99054, -1638.7491, 19.050714);

mp.events.add('CLIENT:GARBAGE:create_unlodiang_marker', () => {
  try {
    started = true;
    pointMarker = mp.markers.new(
      27,
      hitPosition,
      5,
      {
        direction: new mp.Vector3(-589.99054, -1638.7491, 18.950714),
        color: [255, 0, 0, 255],
        visible: true,
        dimension: 0,
        range: 2,
      },
    );

    vehMarker = mp.markers.new(
      20,
      new mp.Vector3(-589.99054, -1638.7491, 20.550714),
      2,
      {
        rotation: new mp.Vector3(180, 0, 0),
        visible: true,
        dimension: 0,
        range: 2,
      },
    );
  } catch (ex) {
    logger.error(ex);
    //mp.console.logError(`create_unlodiang_marker: ${ex}`);
  }
});

mp.events.add('CLIENT:GARBAGE:delete_unlodiang_marker', () => {
  if (started) {
    try {
      started = false;

      if (pointMarker != null) pointMarker.destroy();

      if (vehMarker != null) vehMarker.destroy();
    } catch (ex) {
      logger.error(ex);
      //mp.console.logError(`delete_unlodiang_marker: ${ex}`);
    }
  }
});

function getTrunkPosition(pos, rot) {
  const vehLong = 6;

  let q = pos.x;
  let w = pos.y;
  const angle = Math.PI * rot / 180;

  q += (vehLong * -Math.sin(-angle));
  w += (vehLong * -Math.cos(-angle));

  return new mp.Vector3(q, w, pos.z + 1);
}

mp.events.add('render', () => {
  if (started) {
    const { vehicle } = mp.players.local;

    if (vehicle == null) return;

    const cords = vehicle.getWorldPositionOfBone(vehicle.getBoneIndexByName('boot'));

    const rot = vehicle.getRotation(5);
    const trunkPos = getTrunkPosition(vehicle.position, rot.z);

    if (vehMarker != null) {
      // vehMarker.setCoords(cords.x, cords.y, cords.z + 1, true, false, false, false);
      vehMarker.position = trunkPos;
    }

    if (mp.game.gameplay.getDistanceBetweenCoords(trunkPos.x, trunkPos.y, trunkPos.z, hitPosition.x, hitPosition.y, hitPosition.z, false) < 0.5) {
      pointMarker.colour = [0, 255, 0, 255];
      vehMarker.colour = [0, 255, 0, 255];
      hit = true;
    } else {
      pointMarker.colour = [255, 0, 0, 255];
      vehMarker.colour = [255, 0, 0, 255];
      hit = false;
    }
  }
  else {
    hit = false;
  }
});

mp.keys.bind(Keys.VK_E, false, () => { // E key
  if (mp.players.local.vehicle == null) return;
  if (!hit) return;
  global.anyEvents.SendServer(() => mp.events.callRemote('SERVER:GARBAGE:start_unloading'));
});

mp.events.add('gabage::hideGabage', () => {
  global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
});

mp.events.add('garbage:showNPCMenu', (data) => {
  thisMenuCall.call('garbage::showNPCMenu', JSON.stringify(data));
  global.anyMenuHTML.openAnyMenu(thisHTML, thisMenu, callbackOnClose);
});

mp.events.add('garbage:showMenu', (data) => {
  thisMenuCall.call('garbage::showMenu', JSON.stringify(data));
  global.anyMenuHTML.openAnyMenu(thisHTML, thisMenu, callbackOnClose);
});

mp.events.add('garbage:startWork', (data) => {
  global.anyMenuHTML.openAnyMenu(thisHTML, thisMenu, callbackOnClose);
  thisMenuCall.call('gabage::startWork', JSON.stringify(data));
});

mp.events.add('gabage::onClickMap', (val) => {
  global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
  global.anyEvents.SendServer(() => mp.events.callRemote('gabage::onClickMap', val));
});

mp.events.add('gabage::addPlayer', (id) => {
  global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
  global.anyEvents.SendServer(() => mp.events.callRemote('gabage::addPlayer', id));
});

mp.events.add('gabage::dropPlayer', (id) => {
  global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
  global.anyEvents.SendServer(() => mp.events.callRemote('gabage::dropPlayer', id));
});

mp.events.add('gabage::nextWork', () => {
  global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
  global.anyEvents.SendServer(() => mp.events.callRemote('gabage::nextWork'));
});

mp.events.add('gabage::parkCar', () => {
  global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
  global.anyEvents.SendServer(() => mp.events.callRemote('gabage::parkCar'));
});

mp.events.add('gabage::finishWork', () => {
  global.anyEvents.SendServer(() => mp.events.callRemote('gabage::finishWork'));
});

mp.events.add('gabage::addSkill', (level, skill) => {
  global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
  global.anyEvents.SendServer(() => mp.events.callRemote('gabage::addSkill', level, skill));
});

mp.events.add('gabage::exitModal', () => {
  global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
});

mp.events.add('gabage::resetScore', () => {
  global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
  global.anyEvents.SendServer(() => mp.events.callRemote('gabage::resetScore'));
});

mp.events.add('garbage::resetSkill', (data) => {
  thisMenuCall.call('garbage::resetSkill', JSON.stringify(data));
});

mp.events.add('garbage::openInteractionMenu', () => {
  thisMenuCall.call('gabage::interaction');
  global.anyMenuHTML.openAnyMenu(thisHTML, thisMenu, false);
});

mp.events.add('gabage::rent', (time) => {
  global.anyEvents.SendServer(() => mp.events.callRemote('gabage::rentWorkCar', time));
  global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
});

mp.events.add('interaction::exit', () => {
  global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
});

mp.keys.bind(Keys.VK_8, false, () => { // 8 key
  if (!global.loggedin || global.chatActive || global.editing || global.taxiPriceOpen || global.cuffed || global.phoneOpen || global.statsOpen || global.animMenuOpen || mp.players.local.getVariable('InDeath') == true) return;

  global.anyEvents.SendServer(() => mp.events.callRemote('SERVER::JOB:openMenu'));
});
