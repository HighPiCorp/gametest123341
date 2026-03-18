const markers = [];

mp.events.add('createCheckpoint', (uid, type, position, scale, dimension, r, g, b, dir) => {
  //mp.console.logInfo("createCheckpoint: uid: "+uid);
  if (typeof markers[uid] !== 'undefined') {
    markers[uid].destroy();
    markers[uid] = undefined;
  }
  if (dir != undefined) {
    markers[uid] = mp.checkpoints.new(
      type,
      position,
      scale,
      {
        direction: dir,
        visible: true,
        dimension,
      },
    );
  } else {
    markers[uid] = mp.markers.new(
      type,
      position,
      scale,
      {
        visible: true,
        dimension,
        range: 2,
      },
    );
  }
});

mp.events.add('deleteCheckpoint', (uid) => {
  //mp.console.logInfo("deleteCheckpoint: uid: "+uid);
  if (typeof markers[uid] === 'undefined') return;
  markers[uid].destroy();
  markers[uid] = undefined;
});

mp.events.add('createWaypoint', (x, y) => {
  mp.game.ui.setNewWaypoint(x, y);
});

let workBlip = null;
mp.events.add('createWorkBlip', (position, setRoute = false) => {
  if (workBlip != null) workBlip.destroy();
  workBlip = mp.blips.new(
    0,
    position,
    {
      name: 'Чекпоинт',
      scale: 1,
      color: 49,
      alpha: 255,
      drawDistance: 100,
      shortRange: false,
      rotation: 0,
      dimension: 0,
    },
  );
  workBlip.setRoute(setRoute);
});
mp.events.add('deleteWorkBlip', () => {
  if (workBlip != null) workBlip.destroy();
  workBlip = null;
});

schoolBlip = null;
mp.events.add('createschoolBlip', (position,dim, setRoute = false) => {
  if (schoolBlip != null) schoolBlip.destroy();
  schoolBlip = mp.blips.new(
    0,
    position,
    {
      name: 'Чекпоинт',
      scale: 1,
      color: 49,
      alpha: 255,
      drawDistance: 100,
      shortRange: false,
      rotation: 0,
      dimension: dim,
    },
  );
  schoolBlip.setRoute(setRoute);
});

mp.events.add('deleteschoolBlip', () => {
  if (schoolBlip != null) schoolBlip.destroy();
  schoolBlip = null;
});


global.schoolCheckpoint = null;
mp.events.add('createschoolpoint', (position,dim) => {
  if (schoolCheckpoint !== null) {
    schoolCheckpoint.destroy();
    global.schoolCheckpoint = null;
  }

  position.z += 4;
  global.schoolCheckpoint = mp.markers.new(
    2,
    position,
    1,
    {
      rotation: new mp.Vector3(180, 0, 0),
      visible: true,
      dimension: dim,
      range: 2,
    },
  );
});

mp.events.add('deleteschoolCheckpoint', () => {
  if (schoolCheckpoint === null) return;
  schoolCheckpoint.destroy();
  global.schoolCheckpoint = null;
});

let garageBlip = null;
mp.events.add('createGarageBlip', (position) => {
  if (garageBlip != null) garageBlip.destroy();
  garageBlip = mp.blips.new(
    357,
    position,
    {
      name: 'Гараж',
      scale: 0.8,
      color: 45,
      alpha: 255,
      drawDistance: 100,
      shortRange: true,
      rotation: 0,
      dimension: 0,
    },
  );
});
mp.events.add('deleteGarageBlip', () => {
  if (garageBlip != null) garageBlip.destroy();
  garageBlip = null;
});

const hiddenBlip = [];
mp.events.add('createHiddenBlip', (id, position, sprite, name = 'blip', scale = 1, color = 45) => {
  if (typeof hiddenBlip[id] !== 'undefined') {
    if (hiddenBlip === null) return;

    hiddenBlip[id].destroy();
    hiddenBlip[id] = null;
  }

  //mp.console.logInfo(`id: ${id} position: ${position} sprite: ${sprite} name: ${name} scale: ${parseFloat(scale)} color: ${color}`);

  hiddenBlip[id] = mp.blips.new(
    parseInt(sprite),
    position,
    {
      name,
      scale: parseFloat(scale),
      color: parseInt(color),
      alpha: 255,
      drawDistance: 100,
      shortRange: true,
      rotation: 0,
      dimension: 0,
    },
  );
});

mp.events.add('deleteHiddenBlip', () => {
  if (typeof hiddenBlip[id] === 'undefined') return;

  hiddenBlip[id].destroy();
  hiddenBlip[id] = null;
});

mp.events.add('changeBlipColor', (blip, color) => {
  try {
    if (blip == null) return;
    blip.setColour(color);
  } catch (e) { }
});
mp.events.add('changeBlipAlpha', (blip, alpha) => {
  try {
    if (blip == null) return;
    blip.setAlpha(alpha);
  } catch (e) { }
});






// CityRP

mp.events.add('collectorRoute', function (x, y, z) {
  try {
    blipRoute = mp.blips.new(1, new mp.Vector3(x, y, z),
      {
        color: 70,
        shortRange: false,
        dimension: mp.players.local.dimension,
      });

    blipRoute.setRoute(true);
  } catch (e) {
    logger.error(e);
  }
});

mp.events.add('collectorUnRoute', function () {
  try {
    if (blipRoute !== undefined || blipRoute !== null) {
      blipRoute.setRoute(false);
      blipRoute.destroy();
    }
  } catch (e) {
    logger.error(e);
  }
});

const router = {};

mp.events.add('router', (blipName, blip) => {
  try {
    if (blipName === '') return;

    mp.events.call('unrouter', blipName);

    blip = JSON.parse(blip);

    const [sprite, [x, y, z], name, scale, color, alpha, shortRange, rotation, dimension, radius, route] = blip;

    router[blipName] = mp.blips.new(sprite, new mp.Vector3(x, y, z),
      {
        name,
        scale,
        color,
        alpha,
        shortRange,
        rotation,
        dimension,
        radius
      });

    router[blipName].setRoute(route);
  } catch (e) {
    logger.error(e);
  }
});

mp.events.add('unrouter', (name) => {
  try {
    if (router[name]) {
      router[name].setRoute(false);
      router[name].destroy();
      delete router[name];
    }
  } catch (e) {
    logger.error(e);
  }
});

// Маркеры

const routerMarkers = {};

mp.events.add('unmarker', name => {
  try {
    if (routerMarkers[name]) {
      routerMarkers[name].destroy();
      delete routerMarkers[name];
    }
  } catch (e) {
    logger.error(e);
  }
});

mp.events.add('marker', (name, marker) => {
  try {
    if (name === '') return;

    mp.events.call('unmarker', name);

    //mp.events.call('stc', marker);

    marker = JSON.parse(marker);

    const [type, [x,y,z], scale, [r, g, b, a], dimension] = marker;

    routerMarkers[name] = mp.markers.new(type, new mp.Vector3(x, y, z), scale,
      {
        visible: true,
        dimension: dimension,
        color: [r, g, b, a]
      });
  } catch (e) {
    logger.error(e);
  }
});
