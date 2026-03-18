let cam = mp.cameras.new('default', new mp.Vector3(0, 0, 0), new mp.Vector3(0, 0, 0), false);
const effect = '';
let blipRoute;
let markerRoute;

global.loggedin = false;
global.lastCheck = 0;
global.chatLastCheck = 0;
global.pocketEnabled = false;
global.lightsOn = false;
global.highbeamsOn = false;

// var emscol = mp.colshapes.newSphere(264.5199, -1352.684, 23.446, 50, 0);

const Peds = [
  //Quest
  { Hash: -1686040670, Pos: new mp.Vector3(122.80321, 6627.4014, 31.908588), Angle: -138.7241 }, // Тревор
  { Hash: 1099825042, Pos: new mp.Vector3(145.25899, -1061.2537, 29.17236), Angle: 160.85257 }, // Джейкоб
  { Hash: 0x64FDEA7D, Pos: new mp.Vector3(-2294.3765, 4252.08, 42.61832), Angle: -103.04031 }, // RaceNPC
  { Hash: -886023758, Pos: new mp.Vector3(2577.3594, 305.09604, 108.60837), Angle: 2.4870493 }, // Дейв
  { Hash: 1767447799, Pos: new mp.Vector3(-517.93207, -251.18907, 35.67812), Angle: -148.5389 }, // Барри

  { Hash: -39239064, Pos: new mp.Vector3(1395.184, 3613.144, 34.9892), Angle: 270.0 }, // Sergey Mavrodi
  { Hash: -283816889, Pos: new mp.Vector3(-1596.81, 5206.15, 4.290094), Angle: 24.0 }, // old fishman
  { Hash: -1176698112, Pos: new mp.Vector3(166.6278, 2229.249, 90.73845), Angle: 47.0 }, // Matthew Allen
  { Hash: 1161072059, Pos: new mp.Vector3(2887.687, 4387.17, 50.65578), Angle: 174.0 }, // Owen Nelson
  // { Hash: -1398552374, Pos: new mp.Vector3(2192.614, 5596.246, 53.75177), Angle: 318.0 }, // Daniel Roberts
  //{ Hash: -459818001, Pos: new mp.Vector3(-215.4299, 6445.921, 31.30351), Angle: 262.0 }, // Michael Turner
  // { Hash: 0x9D0087A8, Pos: new mp.Vector3(480.9385, -1302.576, 29.24353), Angle: 224.0 }, // jimmylishman
  // { Hash: 1706635382, Pos: new mp.Vector3(-222.5464, -1617.449, 34.86932), Angle: 309.2058 }, // Lamar_Davis
  //{ Hash: 588969535, Pos: new mp.Vector3(85.79006, -1957.156, 20.74745), Angle: 320.4474 }, // Carl_Ballard
  { Hash: -812470807, Pos: new mp.Vector3(892.2745, -2172.252, 32.28627), Angle: 180.0 }, // Chiraq_Bloody
  { Hash: 653210662, Pos: new mp.Vector3(485.6168, -1529.195, 29.28829), Angle: 0.0 }, // Riki_Veronas
  { Hash: 663522487, Pos: new mp.Vector3(1408.224, -1486.415, 60.65733), Angle: 172.3141 }, // Santano_Amorales
  //{ Hash: 645279998, Pos: new mp.Vector3(-113.9224, 985.793, 235.754), Angle: 110.9234 }, // Vladimir_Medvedev
  { Hash: -236444766, Pos: new mp.Vector3(-1811.368, 438.4105, 128.7074), Angle: 348.107 }, // Kaha_Panosyan
  //{ Hash: -1427838341, Pos: new mp.Vector3(-1549.287, -89.35114, 54.92917), Angle: 7.874235 }, // Jotaro_Josuke
  { Hash: -2034368986, Pos: new mp.Vector3(1392.098, 1155.892, 114.4433), Angle: 82.24557 }, // Solomon_Gambino
  //{ Hash: -1920001264, Pos: new mp.Vector3(485.45993, -1006.5963, 25.75454), Angle: 31.060461 }, // Alonzo_Harris
  //{ Hash: 368603149, Pos: new mp.Vector3(443.53638, -982.2638, 30.709311), Angle: 102.43954 }, // Nancy_Spungen
  //{ Hash: 1581098148, Pos: new mp.Vector3(484.6184, -1003.7101, 25.714645), Angle: 5.0195193 }, // Bones_Bulldog
  { Hash: 941695432, Pos: new mp.Vector3(149.1317, -758.3485, 242.152), Angle: 66.82055 }, //  Steve_Hain
  { Hash: 1558115333, Pos: new mp.Vector3(152.17671, -759.8796, 45.833297), Angle: 57.193066 }, // Michael Bisping
  { Hash: 1925237458, Pos: new mp.Vector3(-2347.958, 3268.936, 32.81076), Angle: 240.8822 }, // Ronny_Pain
  { Hash: 988062523, Pos: new mp.Vector3(-530.32465, -176.10587, 42.871305), Angle: 13.0 }, // Tom Logan
  //{ Hash: 2120901815, Pos: new mp.Vector3(-530.32465, -176.10587, 42.871305), Angle: 24.299559 }, // Lorens_Hope
  { Hash: 0xF0D4BE2E, Pos: new mp.Vector3(-536.9456, -192.18391, 38.184633), Angle: 24.311018 }, // Heady_Hunter
  { Hash: -1420211530, Pos: new mp.Vector3(359.33453, -1406.1421, 49.609615), Angle: -46.048203 }, // Bdesma_Katsuni
  { Hash: 1092080539, Pos: new mp.Vector3(357.1841, -1410.2632, 32.594926), Angle: 50.420685 }, // Steve_Hobs
  { Hash: -1306051250, Pos: new mp.Vector3(347.10608, -1453.3138, 32.594983), Angle: 39.305317 }, // Billy_Moore
  { Hash: -907676309, Pos: new mp.Vector3(724.8585, 134.1029, 80.95643), Angle: 245.0083 }, // Ronny_Bolls
  //{ Hash: 940330470, Pos: new mp.Vector3(458.7059, -995.118, 25.35196), Angle: 176.8092 }, // Rashkovsky
  //{ Hash: 1596003233, Pos: new mp.Vector3(459.7471, -1000.333, 24.91329), Angle: 177.2829 }, // Muscle Prisoner
  //{ Hash: -520477356, Pos: new mp.Vector3(-455.9738, 6014.119, 31.59654), Angle: 357.7483 }, // Bot
  //{ Hash: -1614285257, Pos: new mp.Vector3(-449.8658, 6012.458, 31.59655), Angle: 308.1411 }, // Kira
  //{ Hash: -1699520669, Pos: new mp.Vector3(-429.0482, 5997.3, 31.59655), Angle: 86.12 }, // Stepa
  { Hash: 0x86BDFE26, Pos: new mp.Vector3(241.87624, 226.89244, 106.16717), Angle: 158.92744 }, // Bankir
  { Hash: 0x6B38B8F8, Pos: new mp.Vector3(243.72769, 226.258, 106.26754), Angle: 154.09741 }, // Bankir
  { Hash: 0xD2E3A284, Pos: new mp.Vector3(247.0752, 225.08812, 106.26754), Angle: 158.62378 }, // Bankir
  { Hash: 0x9712C38F, Pos: new mp.Vector3(248.89041, 224.42743, 106.26713), Angle: 157.45494 }, // Bankir
  { Hash: 0x69E8ABC3, Pos: new mp.Vector3(252.23082, 223.09984, 106.26682), Angle: 151.47325 }, // Bankir
  { Hash: 0x50610C43, Pos: new mp.Vector3(254.0354, 222.5548, 106.26682), Angle: 158.7842 }, // Bankir
  { Hash: 0x5972CCF0, Pos: new mp.Vector3(-1213.3234, -332.73706, 37.860904), Angle: 39.677742 }, // Bankir
  { Hash: 0x2E420A24, Pos: new mp.Vector3(-1211.9491, -332.04126, 37.860923), Angle: 19.29191 }, // Bankir
  { Hash: 0xE5A11106, Pos: new mp.Vector3(-187.32973, -802.9658, 30.454018), Angle: 115.85247 }, // scooter
  { Hash: 0xB3B3F5E6, Pos: new mp.Vector3(1176.5304, 2708.293, 38.06788), Angle: 175.50885 }, // Bankir
  { Hash: 0x69E8ABC3, Pos: new mp.Vector3(1175.0182, 2708.2075, 38.06793), Angle: 171.8992 }, // Bankir
  { Hash: 0x5972CCF0, Pos: new mp.Vector3(-110.1932, 6468.9536, 31.606699), Angle: 130.13057 }, // Bankir
  { Hash: 0x6B38B8F8, Pos: new mp.Vector3(-111.180084, 6470.057, 31.6067), Angle: 131.10461 }, // Bankir
  { Hash: 0x6B38B8F8, Pos: new mp.Vector3(-112.245544, 6471.1, 31.6067), Angle: 128.56067 }, // Bankir
  { Hash: 0x8D67EE7D, Pos: new mp.Vector3(-576.0714, -198.01747, 48.024793), Angle: -53.422173 }, // FamilyNPC
  { Hash: 0xE5A11106, Pos: new mp.Vector3(-521.41064, -261.145, 35.48497), Angle: -102.36995 }, // ScooterNPC
  { Hash: 0xE5A11106, Pos: new mp.Vector3(746.827, 119.52474, 78.552965), Angle: -86.458916 }, // ScooterNPC
  { Hash: 0xE5A11106, Pos: new mp.Vector3(2572.3352, 307.19296, 108.58946), Angle: 2.1375484 }, // ScooterNPC
  { Hash: 0xE5A11106, Pos: new mp.Vector3(-1343.9868, 19.45224, 53.430166), Angle: -131.94603 }, // ScooterNPC
  { Hash: 0xE5A11106, Pos: new mp.Vector3(276.48694, -346.47394, 44.89989), Angle: -108.81572 }, // ScooterNPC
  { Hash: 0xE5A11106, Pos: new mp.Vector3(-1032.3329, -2678.6523, 14.046008), Angle: -26.412941 }, // ScooterNPC
  { Hash: 0xE5A11106, Pos: new mp.Vector3(-274.37897, 6073.1426, 31.427463), Angle: 93.7134 }, // ScooterNPC

  { Hash: 0x69E8ABC3, Pos: new mp.Vector3(148.01831, -1041.5494, 29.347932), Angle: -22.809032 }, // Bankir
  { Hash: 0x5972CCF0, Pos: new mp.Vector3(149.3966, -1042.041, 29.347988), Angle: -24.854496 }, // Bankir
  { Hash: 0x69E8ABC3, Pos: new mp.Vector3(-2961.1794, 481.51566, 15.676951), Angle: 85.829475 }, // Bankir
  { Hash: 0x5972CCF0, Pos: new mp.Vector3(-2961.1636, 483.06906, 15.677014), Angle: 84.25386 }, // Bankir
  { Hash: 0x69E8ABC3, Pos: new mp.Vector3(312.4231, -279.92368, 54.144632), Angle: -22.585577 }, // Bankir
  { Hash: 0x5972CCF0, Pos: new mp.Vector3(313.8048, -280.467, 54.14468), Angle: -23.210102 }, // Bankir
  { Hash: 0x69E8ABC3, Pos: new mp.Vector3(-352.7821, -50.87008, 49.0164), Angle: -24.058083 }, // Bankir
  { Hash: 0x5972CCF0, Pos: new mp.Vector3(-351.4088, -51.417828, 49.016477), Angle: -22.74028 }, // Bankir
  { Hash: 0x31430342, Pos: new mp.Vector3(-808.324, -1358.7196, 4.69086), Angle: -58.35533 }, // Realtor
  { Hash: 0x31430342, Pos: new mp.Vector3(-801.7512, -1351.7385, 4.6858766), Angle: 134.34183 }, // Realtor
  { Hash: 0x31430342, Pos: new mp.Vector3(-796.08777, -1356.7195, 4.6832053), Angle: 118.339096 }, // Realtor
  { Hash: 0x31430342, Pos: new mp.Vector3(-802.5464, -1363.765, 4.6838557), Angle: -61.758705 }, // Realtor
  { Hash: 0x31430342, Pos: new mp.Vector3(-796.04034, -1368.8282, 4.6936824), Angle: 6.022205 }, // Realtor
  //{ Hash: 0x31430342, Pos: new mp.Vector3(-1036.72, -1369.53, 5.0898247), Angle: -95.41997 }, // Realtor

  { Hash: 0xD7DA9E99, Pos: new mp.Vector3(-509.86484, -1003.16003, 23.530502), Angle: 89.51933 }, // builder 3
  { Hash: 0xD7DA9E99, Pos: new mp.Vector3(37.365734, 6544.204, 31.622476), Angle: -139.71533 }, // builder 1
  { Hash: 0x49EA5685, Pos: new mp.Vector3(144.8581, -373.5612, 43.25), Angle: 35.74032 }, // builder 2
  { Hash: 0x76284640, Pos: new mp.Vector3(-934.0029, -1555.7467, 5.221231), Angle: -71.126564 }, //rent 1
  { Hash: 0x76284640, Pos: new mp.Vector3(-532.26685, 35.492958, 52.59122), Angle: -33.34572 }, //rent 2
  { Hash: 0x76284640, Pos: new mp.Vector3(807.1209, -809.9397, 26.192802), Angle: 95.220276 }, //rent 3
  { Hash: 0x76284640, Pos: new mp.Vector3(51.246513, -2571.455, 5.994591), Angle: 0.24106468 }, //rent 4
  { Hash: 0x76284640, Pos: new mp.Vector3(1925.7455, 3730.981, 32.756719), Angle: 29.118134 }, // rent 5
  { Hash: 0x76284640, Pos: new mp.Vector3(-223.14903, 6242.9873, 31.502387), Angle: 46.679287 }, //rent 6
  { Hash: 0x76284640, Pos: new mp.Vector3(1521.5321, 3916.8125, 31.600618), Angle: -99.361916 }, //rent 7
  { Hash: 0x76284640, Pos: new mp.Vector3(102.55237, 6607.8423, 31.847175), Angle: -92.94479 }, //rent 8
  { Hash: 0x76284640, Pos: new mp.Vector3(290.4201, -782.907, 29.321908), Angle: -108.088066 }, //rent 9
  { Hash: 776079908, Pos: new mp.Vector3(-553.53766, -196.5979, 51.66287), Angle: -63.04212 }, //NPC for Investor
  { Hash: -886023758, Pos: new mp.Vector3(-548.2103, -198.72198, 51.66739), Angle: 32.80798 }, //NPC for Prorab
  { Hash: 0x76284640, Pos: new mp.Vector3(3866.5056, 4463.681, 2.7272735), Angle: 2.4870493 }, //rent Лодки
  { Hash: -907676309, Pos: new mp.Vector3(344.8153, 3404.7915, 36.455365), Angle: 20.844616 }, //NPC Robbery House
  { Hash: -907676309, Pos: new mp.Vector3(-276.52277, -2025.3849, 30.155599), Angle: -144.95 }, //NPC Robbery House
];

/* mp.colshapes.forEach(
	(colshape) => {
		if(colshape == emscol) mp.gui.chat.push("You are near EMS");
	}
); */

setTimeout(() => {
  Peds.forEach((ped) => {
    mp.peds.new(ped.Hash, ped.Pos, ped.Angle, 0);
  });
}, 10000);

mp.game.gameplay.disableAutomaticRespawn(true);
mp.game.gameplay.ignoreNextRestart(true);
mp.game.gameplay.setFadeInAfterDeathArrest(false);
mp.game.gameplay.setFadeOutAfterDeath(false);
mp.game.gameplay.setFadeInAfterLoad(false);
//mp.game.invoke("0x7635B349", true);

mp.events.add('freeze', (toggle) => {
  localplayer.freezePosition(toggle);
  if(localplayer.vehicle != null){
    localplayer.vehicle.freezePosition(toggle);
  }
});

mp.events.add('freezeplayer', (toggle) => {
  localplayer.freezePosition(toggle);
});

mp.events.add('freezeVeh', (toggle) => {
  localplayer.vehicle.freezePosition(toggle);
});

mp.events.add('freezeVehV2', (vehicle, toggle) => {
  vehicle.freezePosition(toggle);
});

mp.events.add('destroyCamera', () => {
  if (cam && cam !== null) {
    cam.destroy();
    mp.game.cam.renderScriptCams(false, false, 3000, true, true);
    //mp.console.logInfo('destroyCamera main.js');
  } else {
    //mp.console.logError('destroyCamera ERROR main.js cam: '+ cam);
  }
});

mp.events.add('screenFadeOut', (duration) => {
  mp.game.cam.doScreenFadeOut(duration);
});

mp.events.add('screenFadeIn', (duration) => {
  mp.game.cam.doScreenFadeIn(duration);
});

let lastScreenEffect = '';
mp.events.add('startScreenEffect', (effectName, duration, looped) => {
  try {
    lastScreenEffect = effectName;
    mp.game.graphics.startScreenEffect(effectName, duration, looped);
  } catch (e) { }
});

mp.events.add('stopScreenEffect', (effectName) => {
  try {
    const effect = (effectName == undefined) ? lastScreenEffect : effectName;
    mp.game.graphics.stopScreenEffect(effect);
  } catch (e) { }
});

mp.events.add('stopAndStartScreenEffect', (stopEffect, startEffect, duration, looped) => {
  try {
    mp.game.graphics.stopScreenEffect(stopEffect);
    mp.game.graphics.startScreenEffect(startEffect, duration, looped);
  } catch (e) { }
});

mp.events.add('setHUDVisible', (arg) => {
  mp.game.ui.displayHud(arg);
  mp.gui.chat.show(arg);
  mp.game.ui.displayRadar(arg);
});

mp.events.add('setPocketEnabled', (state) => {
  global.pocketEnabled = state;
  if (state) {
    mp.gui.execute("fx.set('inpocket')");
    mp.game.invoke(getNative('SET_FOLLOW_PED_CAM_VIEW_MODE'), 4);
  } else {
    mp.gui.execute('fx.reset()');
  }
});

mp.keys.bind(Keys.VK_Y, false, () => {
  if (!global.loggedin || chatActive || editing || new Date().getTime() - lastCheck < 1000 || global.menuOpened) return;
  mp.events.callRemote('acceptPressed');
  global.lastCheck = new Date().getTime();
});

mp.keys.bind(Keys.VK_N, false, () => {
  if (!global.loggedin || chatActive || editing || new Date().getTime() - lastCheck < 1000 || global.menuOpened) return;
  mp.events.callRemote('cancelPressed');
  global.lastCheck = new Date().getTime();
});

// mp.events.add('connected', () => {
//   mp.game.ui.displayHud(false);
//   cam = mp.cameras.new('default', global.startCamPos, global.startCamRot, 90.0);
//   cam.setActive(true);
//   mp.game.graphics.startScreenEffect('SwitchSceneMichael', 5000, false);
//   const effect = 'SwitchSceneMichael';
// });

mp.events.add('ready', () => {
  mp.game.ui.displayHud(true);
  // cam.setActive(false);
  // mp.game.graphics.stopScreenEffect(effect);
});

mp.events.add('kick', (notify) => {
  mp.events.call('notify', 4, 9, notify, 10000);
  mp.events.callRemote('kickclient');
});

mp.events.add('loggedIn', () => {
  global.loggedin = true;
});

var followPlayer = null;

mp.events.add('setFollow', function (toggle, entity) {
    try {
        if (toggle) {
            if (entity && mp.players.exists(entity)) {
                followPlayer = entity;
            }
        } else
            followPlayer = null;
    } catch (e) {
        logger.error(e);
    }
});

setInterval(function () {
  try {
      // if (localplayer.getArmour() <= 0 && localplayer.getVariable('HASARMOR') === true) {
      //     global.anyEvents.SendServer(() => mp.events.callRemote('deletearmor'));
      // }

      if (followPlayer) {
          var pos = followPlayer.position;
          var localPos = mp.players.local.position;
          var dist = mp.game1.system.vdist(pos.x, pos.y, pos.z, localPos.x, localPos.y, localPos.z);
          if (dist > 30) {
              followPlayer = null;
              return;
          }
          var speed = 3;
          if (dist < 10) speed = 2;
          if (dist < 5) speed = 1;
          mp.players.local.taskFollowNavMeshToCoord(pos.x, pos.y, pos.z, speed, -1, 1, true, 0);
          mp.events.callRemote('synsfollow', pos.x, pos.z, pos.z, speed);
      }
  } catch (e) {
      logger.error(e);
  }
}, 600);

global.hasFollower = false;

mp.events.add('synsfollow', (player, x,y,z, speed) => {
  try {
      if (mp.players.local == player) return;
      player.taskFollowNavMeshToCoord(x, y, z, speed, -1, 1, true, 0);
  } catch (e) {
      logger.error(e);
  }
});

mp.events.add('followattach', (player, target) => {
  try {
      if (player === target) return;
      let bony = player.getBoneIndexByName("SKEL_ROOT");
      target.attachTo(player.handle, bony, 0, 0.5, 0, 0, 0, 0, true, false, false, false, 0, false);

      if(mp.players.local.player == player){
        global.hasFollower = true;
      }
  } catch (e) {
      logger.error(e);
  }
});

mp.events.add('followdetach', (target, player) => {
  try {
      if(mp.players.local.player == player){
        global.hasFollower = false;
      }

      target.detach(false, false);
  } catch (e) {
      logger.error(e);
  }
});


// телепорт на метку
let waypoint;
let lastWaypointCoords;

mp.keys.bind(Keys.VK_P, false, () => { // Телепорт (P)
  if (!global.loggedin || chatActive || editing || global.menuCheck() || cuffed || localplayer.getVariable('InDeath') == true) return;

  if (!global.localplayer.getVariable('IS_ADMIN')) return;
  if (!lastWaypointCoords) { mp.game.graphics.notify('Ошибка: ~n~~h~~r~Нет записи последнего waypoint\'a.'); return; }
  mp.events.call('tpToWaypoint');

  global.lastCheck = new Date().getTime();
});

function findZ(mp, maxAttempts, delay, wpos, oldpos, oldcarpos) {
  const player = mp.players.local;
  const { vehicle } = player;

  player.position = new mp.Vector3(wpos.x, wpos.y, 0);
  player.freezePosition(true);
  let attempts = 1;
  let timeout = setTimeout(function getZ() {
    wpos.z = mp.game.gameplay.getGroundZFor3dCoord(wpos.x, wpos.y, 1000, 0, false);
    if (!wpos.z && attempts < 10) {
      attempts++;
      player.position = new mp.Vector3(wpos.x, wpos.y, attempts * 50);

		        if (vehicle) {
			        vehicle.position = new mp.Vector3(wpos.x, wpos.y, attempts * 50);

			        mp.game.cam.doScreenFadeIn(250);
			        setTimeout(() => {
			        	player.setIntoVehicle(vehicle.handle, -1);
			        }, 100);
		        }

      timeout = setTimeout(getZ, delay);
    } else if (!wpos.z && attempts == maxAttempts) {
      player.position = oldpos;

		        if (vehicle && oldcarpos != null) {
			        vehicle.position = oldcarpos;
			        mp.game.cam.doScreenFadeIn(250);
			        setTimeout(() => {
			        	player.setIntoVehicle(vehicle.handle, -1);
			        }, 100);
		        }

      mp.game.graphics.notify('Ошибка: ~n~~h~~r~Не удалось получить координату Z.');
      player.freezePosition(false);
      clearTimeout(timeout);
    } else { // if z found - tp to wpos
      player.position = new mp.Vector3(wpos.x, wpos.y, wpos.z + 2);

		        if (vehicle) {
			        vehicle.position = new mp.Vector3(wpos.x, wpos.y, wpos.z + 2);
			        mp.game.cam.doScreenFadeIn(250);
			        setTimeout(() => {
			        	player.setIntoVehicle(vehicle.handle, -1);
			        }, 100);
		        }

      player.freezePosition(false);
      // global.anyEvents.SendServer(() => mp.events.callRemote('notifyCoords', 'Телепорт пo координатам:', wpos.x, wpos.y, wpos.z+1));
      clearTimeout(timeout);
    }
  }, delay);
}

function findWP(mp) {
  const wpos = { ...lastWaypointCoords };
  const player = mp.players.local;
  const { vehicle } = player;
  const oldpos = player.position;
  let oldcarpos = null;
  if (vehicle) oldcarpos = vehicle.position;

  if (wpos.z != 20) {

    player.position = new mp.Vector3(wpos.x, wpos.y, wpos.z + 2);

    if (vehicle) {
      vehicle.position = new mp.Vector3(wpos.x, wpos.y, wpos.z + 2);
      mp.game.cam.doScreenFadeIn(250);
      setTimeout(() => {
        player.setIntoVehicle(vehicle.handle, -1);
      }, 100);
    }

    // global.anyEvents.SendServer(() => mp.events.callRemote('notifyCoords', 'Телепорт по координатам:', wpos.x, wpos.y, wpos.z+1));
    return;
  }

  findZ(mp, 10, 150, wpos, oldpos, oldcarpos);
}

mp.events.add('tpToWaypoint', () => {
  findWP(mp);
});

mp.events.add('render', () => {
  if (waypoint !== mp.game.invoke('0x1DD1F58F493F1DA5')) {
    waypoint = mp.game.invoke('0x1DD1F58F493F1DA5');
    if (waypoint) {
      const blip = mp.game.invoke('0x1BEDE233E6CD2A1F', 8);
      const coords = mp.game.ui.getBlipInfoIdCoord(blip);
      lastWaypointCoords = coords;
      // mp.events.call('tpToWaypoint');
    }
  }
});
//

mp.keys.bind(Keys.VK_E, false, () => { // E key
  // const isFlashLightOn = mp.game.invoke("0x4B7620C47217126C", localplayer.handle);
  // const _SET_FLASH_LIGHT_ENABLED = "0x68EDDA28A5976D07";
  // if (isFlashLightOn && !mp.game.invoke(_SET_FLASH_LIGHT_ENABLED)) {
  //   mp.game.invoke("0x988DB6FE9B3AC000", localplayer.handle, true);
  // } else if (!isFlashLightOn) {
  //   mp.game.invoke("0x988DB6FE9B3AC000", localplayer.handle, false);
  // }

  if (!global.loggedin || chatActive || editing || new Date().getTime() - lastCheck < 1000 || global.menuOpened) return;
  if (global.casinoOpened) {
    global.anyEvents.SendServer(() => mp.events.callRemote('interactionPressed'));
  }
  if (global.menuOpened) return;
  global.anyEvents.SendServer(() => mp.events.callRemote('interactionPressed'));
  global.lastCheck = new Date().getTime();
  global.acheat.pos();
});

/*mp.keys.bind(getKeyBy('LOCK_CAR_DOORS'), false, () => { // L key
  if (!global.loggedin || chatActive || editing || new Date().getTime() - lastCheck < 1000 || global.menuOpened) return;
  global.anyEvents.SendServer(() => mp.events.callRemote('lockCarPressed'));
  lastCheck = new Date().getTime();
});*/

global.lockVehicle = function() { // L key
  if (!global.loggedin || chatActive || editing || new Date().getTime() - global.lastCheck < 1000 || global.menuOpened) return;
  mp.events.callRemote('lockCarPressed');
  global.lastCheck = new Date().getTime();
};

/*mp.keys.bind(Keys.VK_LEFT, true, () => {
  if (mp.gui.cursor.visible || !loggedin) return;
  if (localplayer.vehicle) {
    if (localplayer.vehicle.getPedInSeat(-1) != localplayer.handle) return;

    if (new Date().getTime() - lastCheck > 500) {
      lastCheck = new Date().getTime();
      if (localplayer.vehicle.getVariable('leftlight') == true) {
			    global.anyEvents.SendServer(() => mp.events.callRemote('VehStream_SetIndicatorLightsData', localplayer.vehicle, 0, 0));
			    mp.gui.execute('HUD.rightSignal=false');
			    mp.gui.execute('HUD.leftSignal=false');
      } else if (localplayer.vehicle.getIsEngineRunning()) {
			    global.anyEvents.SendServer(() => mp.events.callRemote('VehStream_SetIndicatorLightsData', localplayer.vehicle, 1, 0));
			    mp.gui.execute('HUD.leftSignal=true');
			    mp.gui.execute('HUD.rightSignal=false');
      }
    }
  }
});*/

// mp.keys.bind(Keys.VK_H, false, () => {
//  	if(mp.gui.cursor.visible || !loggedin) return;
//  	if(localplayer.vehicle) {
//  		setTimeout(function () {
//  			var lightState = localplayer.vehicle.getLightsState(1,1);

//  			mp.gui.execute(`HUD.updateLights(${lightState.lightsOn}, ${lightState.highbeamsOn})`);

//  			//mp.gui.chat.push(`lightState: ${lightState} | ${JSON.stringify(lightState)}`);
//  		}, 150);
//  	}

//  	// mp.gui.execute(`HUD.lowBeam=${lightState.lightsOn}; HUD.highBeam=${lightState.lightsOn}`);
// });

 /*mp.events.add('render', () => {
 	if(localplayer.vehicle) {
		var lightState = localplayer.vehicle.getLightsState(1,1);

		if(global.lightsOn != lightState.lightsOn || global.highbeamsOn != lightState.highbeamsOn)
		{
			global.lightsOn = lightState.lightsOn;
			global.highbeamsOn = lightState.highbeamsOn;

			// mp.gui.chat.push(`lightState: ${lightState} | ${JSON.stringify(lightState)} | ${lightState.lightsOn}:${lightState.highbeamsOn}`);
			// mp.gui.chat.push(`Фары переключились.`);

			if(!global.lightsOn)
			{
				// день
				if(global.highbeamsOn) {
					// mp.gui.chat.push(`Дневной свет.`);

					mp.gui.execute(`HUD.updateLights(1, 0)`);
				}
			}
			else
			{
				// ночь
				if(!global.highbeamsOn) {
					// mp.gui.chat.push(`Дневной свет.`);
					mp.gui.execute(`HUD.updateLights(1, 0)`);
				} else {
					// mp.gui.chat.push(`Дальний свет.`);
					mp.gui.execute(`HUD.updateLights(1, 1)`);
				}
			}
			if(!global.lightsOn && !global.highbeamsOn) {
				mp.gui.execute(`HUD.updateLights(0, 0)`);
			}
		}
	}
});*/
/*
mp.keys.bind(Keys.VK_RIGHT, true, () => {
  if (mp.gui.cursor.visible || !loggedin) return;
  if (localplayer.vehicle) {
    if (localplayer.vehicle.getPedInSeat(-1) != localplayer.handle) return;
    if (new Date().getTime() - lastCheck > 500) {
      lastCheck = new Date().getTime();
      if (localplayer.vehicle.getVariable('rightlight') == true) {
			    global.anyEvents.SendServer(() => mp.events.callRemote('VehStream_SetIndicatorLightsData', localplayer.vehicle, 0, 0));
			    mp.gui.execute('HUD.rightSignal=false');
			    mp.gui.execute('HUD.leftSignal=false');
      } else if (localplayer.vehicle.getIsEngineRunning()) {
			    global.anyEvents.SendServer(() => mp.events.callRemote('VehStream_SetIndicatorLightsData', localplayer.vehicle, 0, 1));
			    mp.gui.execute('HUD.rightSignal=true');
			    mp.gui.execute('HUD.leftSignal=false');
      }
    }
  }
});*/

/*mp.keys.bind(getKeyBy('EMERGENCY_SIGNAL'), true, () => {
  if (mp.gui.cursor.visible || !loggedin) return;
  if (localplayer.vehicle) {
    if (localplayer.vehicle.getPedInSeat(-1) != localplayer.handle) return;
    if (new Date().getTime() - lastCheck > 500) {
      lastCheck = new Date().getTime();
      if (localplayer.vehicle.getVariable('leftlight') == true || localplayer.vehicle.getVariable('rightlight') == true) {
        global.anyEvents.SendServer(() => mp.events.callRemote('VehStream_SetIndicatorLightsData', localplayer.vehicle, 0, 0));

			    mp.gui.execute('HUD.leftSignal=false');
			    mp.gui.execute('HUD.rightSignal=false');
      } else if (localplayer.vehicle.getIsEngineRunning()) {
        global.anyEvents.SendServer(() => mp.events.callRemote('VehStream_SetIndicatorLightsData', localplayer.vehicle, 1, 1));

			    mp.gui.execute('HUD.leftSignal=true');
			    mp.gui.execute('HUD.rightSignal=true');
      }
    }
  }
});*/
let dropBlip = null;

mp.events.add('syncobj', (x, y, z) => {
    try {
        mp.game1.fire.addExplosion(x, y, z, 22, 0, true, false, 1);
    } catch (e) {
        logger.error(e);
    }
});

mp.events.add('showDropblip', (x, y, z) => {
    try {
        dropBlip = mp.blips.new(9, new mp.Vector3(x, y, z),
            {
                scale: 1,
                color: 1,
                alpha: 120,
                shortRange: false,
                radius: 250,
            });
    } catch (e) {
        logger.error(e);
    }
});


mp.events.add('hideDropblip', () => {
    try {
        if (dropBlip != null) {
            dropBlip.destroy();
            dropBlip = null;
        }
    } catch (e) {
        logger.error(e);
    }
});
global.leftSignal = function() {
  if (mp.gui.cursor.visible || !global.loggedin) return;
  if (localplayer.vehicle) {
    if (localplayer.vehicle.getPedInSeat(-1) != localplayer.handle) return;

    if (new Date().getTime() - global.lastCheck > 500) {
      global.lastCheck = new Date().getTime();
      if (localplayer.vehicle.getVariable('leftlight') == true) {
			    mp.events.callRemote('VehStream_SetIndicatorLightsData', localplayer.vehicle, 0, 0);
			    mp.gui.execute('HUD.rightSignal=false');
			    mp.gui.execute('HUD.leftSignal=false');
      } else if (localplayer.vehicle.getIsEngineRunning()) {
			    mp.events.callRemote('VehStream_SetIndicatorLightsData', localplayer.vehicle, 1, 0);
			    mp.gui.execute('HUD.leftSignal=true');
			    mp.gui.execute('HUD.rightSignal=false');
      }
    }
  }
};


global.rightSignal = function() {
  if (mp.gui.cursor.visible || !global.loggedin) return;
  if (localplayer.vehicle) {
    if (localplayer.vehicle.getPedInSeat(-1) != localplayer.handle) return;
    if (new Date().getTime() - global.lastCheck > 500) {
      global.lastCheck = new Date().getTime();
      if (localplayer.vehicle.getVariable('rightlight') == true) {
			    mp.events.callRemote('VehStream_SetIndicatorLightsData', localplayer.vehicle, 0, 0);
			    mp.gui.execute('HUD.rightSignal=false');
			    mp.gui.execute('HUD.leftSignal=false');
      } else if (localplayer.vehicle.getIsEngineRunning()) {
			    mp.events.callRemote('VehStream_SetIndicatorLightsData', localplayer.vehicle, 0, 1);
			    mp.gui.execute('HUD.rightSignal=true');
			    mp.gui.execute('HUD.leftSignal=false');
      }
    }
  }
};

global.emergencySignal = function() {
  if (mp.gui.cursor.visible || !global.loggedin) return;
  if (localplayer.vehicle) {
    if (localplayer.vehicle.getPedInSeat(-1) != localplayer.handle) return;
    if (new Date().getTime() - global.lastCheck > 500) {
      global.lastCheck = new Date().getTime();
      if (localplayer.vehicle.getVariable('leftlight') == true || localplayer.vehicle.getVariable('rightlight') == true) {
        mp.events.callRemote('VehStream_SetIndicatorLightsData', localplayer.vehicle, 0, 0);

			    mp.gui.execute('HUD.leftSignal=false');
			    mp.gui.execute('HUD.rightSignal=false');
      } else if (localplayer.vehicle.getIsEngineRunning()) {
        mp.events.callRemote('VehStream_SetIndicatorLightsData', localplayer.vehicle, 1, 1);

			    mp.gui.execute('HUD.leftSignal=true');
			    mp.gui.execute('HUD.rightSignal=true');
      }
    }
  }
};

/*mp.keys.bind(getKeyBy('ENGINE_CAR'), false, () => { // 2 key
  if (!loggedin || chatActive || editing || new Date().getTime() - lastCheck < 400 || global.menuOpened) return;
  if (localplayer.isInAnyVehicle(false) && localplayer.vehicle.getSpeed() <= 3) {
    lastCheck = new Date().getTime();
    mp.events.callRemote('engineCarPressed');
  }
});*/

global.engineCar = function () { // 2 key
  if (!global.loggedin || chatActive || editing || new Date().getTime() - lastCheck < 400 || global.menuOpened) return;
  if (localplayer.isInAnyVehicle(false) && localplayer.vehicle.getSpeed() <= 3) {
    global.lastCheck = new Date().getTime();
    mp.events.callRemote('engineCarPressed');
  }
};


/*mp.keys.bind(Keys.VK_UP, false, function () { // стрелка вверх
    if (!loggedin || chatActive || editing || cuffed || localplayer.getVariable('InDeath') == true || new Date().getTime() - lastCheck < 400) return;

    if(global.phoneOppened)
    {
      global.phoneOppened = false;
      global.anyEvents.SendServer(() => mp.events.callRemote('closePlayerMenu'));
    }
    else {
      global.anyEvents.SendServer(() => mp.events.callRemote('openPlayerMenu'));
      mp.game.mobile.createMobilePhone(3);
      mp.game.mobile.setMobilePhoneScale (0);
      mp.game.mobile.scriptIsMovingMobilePhoneOffscreen(false);
      mp.game.mobile.setPhoneLean(false);
      lastCheck = new Date().getTime();
      global.phoneOppened = true;
    }
});*/



/*mp.keys.bind(getKeyBy('PHONE'), false, () => {
  if (!loggedin || chatActive || editing || global.menuCheck() || cuffed || localplayer.getVariable('InDeath') == true || new Date().getTime() - lastCheck < 400) return;

  if (global.phoneOpen) {
    mp.game.invoke('0x3BC861DF703E5097', mp.players.local.handle, true);
    global.anyEvents.SendServer(() => mp.events.callRemote('closePlayerMenu'));

    global.phoneOpen = 0;
  } else {
    global.anyEvents.SendServer(() => mp.events.callRemote('openPlayerMenu'));
    mp.game.mobile.createMobilePhone(3);
    mp.game.mobile.setMobilePhoneScale(0);
    mp.game.mobile.scriptIsMovingMobilePhoneOffscreen(false);
    mp.game.mobile.setPhoneLean(false);
    lastCheck = new Date().getTime();

    global.phoneOpen = 1;
  }
});*/

global.openPhone = function() {
  if (!global.loggedin || mp.game.ui.isPauseMenuActive()|| global.chatActive || global.insideTrackSeat || global.activeAnim != null || global.inTrunk || global.circleOpen || global.taxiPriceOpen || global.editing || global.menuCheck() || global.cuffed || localplayer.getVariable('InDeath') == true || new Date().getTime() - lastCheck < 400 ) return;

  if (global.phoneOpen) {
    mp.game.invoke('0x3BC861DF703E5097', mp.players.local.handle, true);
    mp.events.callRemote('closePlayerMenu');
    mp.gui.chat.activate(true);

    global.phoneOpen = 0;
  } else {
    mp.events.callRemote('openPlayerMenu');
    mp.game.mobile.createMobilePhone(3);
    mp.game.mobile.setMobilePhoneScale(0);
    mp.game.mobile.scriptIsMovingMobilePhoneOffscreen(false);
    mp.game.mobile.setPhoneLean(false);
    global.lastCheck = new Date().getTime();
    mp.gui.chat.activate(false);

    global.phoneOpen = 1;
  }
};

/*setInterval(() => {
  const MobileObject = mp.game.invoke('0x2AFE52F782F25775', mp.players.local.handle);
  // mp.gui.chat.push(`render: MobileObject: ${MobileObject}, global.menuOpened: ${global.menuOpened}, Cursor: ${mp.gui.cursor.visible}, Phone: ${global.phoneOpen}`);
  if (MobileObject && !mp.gui.cursor.visible) {
    mp.game.invoke('0x3BC861DF703E5097', mp.players.local.handle, true);
    global.phoneOpen = 0;

    global.anyEvents.SendServer(() => mp.events.callRemote('closePlayerMenu'));
  }
}, 2000);*/


global.inTrunk = false;

mp.events.add('testtrunk', function (player, vehicle) {
  try {
      if(player == mp.players.local) global.inTrunk = true;
      let bony = vehicle.getBoneIndexByName("boot");
      //mp.events.call("stc", JSON.stringify(bony));
      player.attachTo(vehicle.handle, bony, 0, 0, 0.5, 0, 0, 0, true, false, false, false, 0, false);
  } catch (e) {
      logger.error(e);
  }
});

mp.events.add('outTrunk', function () {
  try {
    global.inTrunk = false;
  } catch (e) {
    logger.error(e);
  }
});


mp.keys.bind(Keys.VK_F12, true, () => { // F8-Key
  const date = new Date();
  const name = `saintsworld-${date.getDate()}.${date.getMonth()}.${date.getFullYear()}-${date.getHours()}.${date.getMinutes()}.${date.getSeconds()}.png`;
  mp.gui.takeScreenshot(name, 1, 100, 0);
});

mp.keys.bind(Keys.VK_X, false, () => { // X key
  if (!global.loggedin || chatActive || editing || new Date().getTime() - lastCheck < 1000 || global.menuOpened || localplayer.getVariable('fraction') <= 0) return;
  mp.events.callRemote('playerPressCuffBut');
  global.lastCheck = new Date().getTime();
});

function CheckMyWaypoint() {
  try {
    if (mp.game.invoke('0x1DD1F58F493F1DA5')) {
      let foundblip = false;
      const blipIterator = mp.game.invoke('0x186E5D252FA50E7D');
      const totalBlipsFound = mp.game.invoke('0x9A3FF3DE163034E8');
      const FirstInfoId = mp.game.invoke('0x1BEDE233E6CD2A1F', blipIterator);
      const NextInfoId = mp.game.invoke('0x14F96AA50D6FBEA7', blipIterator);
      for (let i = FirstInfoId, blipCount = 0; blipCount != totalBlipsFound; blipCount++, i = NextInfoId) {
        if (mp.game.invoke('0x1FC877464A04FC4F', i) == 8) {
          var coord = mp.game.ui.getBlipInfoIdCoord(i);
          foundblip = true;
          break;
        }
      }
      if (foundblip)
      {
        let playerPos = localplayer.position;
        let targetZ = mp.game.gameplay.getGroundZFor3dCoord(coord.x, coord.y, coord.z, 0.0, false);
        let targetPos = new mp.Vector3(coord.x, coord.y, targetZ);

        sendTaxiTargetPlace(targetPos);
      }
    }
  } catch (e) { }
}

function sendTaxiTargetPlace(targetPos)
{
  let playerPos = localplayer.position;
  let dist = mp.game.pathfind.calculateTravelDistanceBetweenPoints(playerPos.x, playerPos.y, playerPos.z, targetPos.x, targetPos.y, targetPos.z);

  let address = global.init.getMapAddress(targetPos);

  mp.events.callRemote('syncWaypoint', targetPos.x, targetPos.y, targetPos.z, address, dist.toFixed(2));
}

mp.events.add('CLIENT::TAXI:GET_PLAYER_ADDRESS', function(){

  let address = global.init.getMapAddress(mp.players.local.position);
  mp.events.callRemote("SERVER::TAXI:SET_PLAYER_ADDRESS", address);
});

// function getMapAddress(pos)
// {
//   let street = mp.game.pathfind.getStreetNameAtCoord(pos.x, pos.y, pos.z, 0, 0);
//   let area = mp.game.zone.getNameOfZone(pos.x, pos.y, pos.z);
//
//   return mp.game.ui.getLabelText(area) + ", " + mp.game.ui.getStreetNameFromHashKey(street.streetName);
// }

mp.keys.bind(Keys.VK_Z, false, () => { // Z key
  if (!global.loggedin || chatActive || editing || new Date().getTime() - lastCheck < 1000 || global.menuOpened || localplayer.getVariable('fraction') <= 0) return;
  if (localplayer.isInAnyVehicle(false)) {
    CheckMyWaypoint();
  } else mp.events.callRemote('playerPressFollowBut');
  global.lastCheck = new Date().getTime();
});

mp.events.add('syncWP', (bX, bY, type) => {
  if (!mp.game.invoke('0x1DD1F58F493F1DA5')) {
    mp.game.ui.setNewWaypoint(bX, bY);
    if (type == 0) mp.events.call('notify', 2, 9, 'Пассажир передал Вам информацию о своём маршруте!', 3000);
    else if (type == 1) mp.events.call('notify', 2, 9, 'Человек из списка контактов Вашего телефона передал Вам метку его местоположения!', 3000);
  } else if (type == 0) mp.events.call('notify', 4, 9, 'Пассажир попытался передать Вам информацию о маршруте, но у Вас уже установлен другой маршрут.', 5000);
  else if (type == 1) mp.events.call('notify', 4, 9, 'Человек из списка контактов Вашего телефона попытался передать Вам метку его местоположения, но у Вас уже установлена другая метка.', 5000);
});

/*mp.keys.bind(Keys.VK_U, false, () => { // U key
  if (!loggedin || chatActive || editing || global.menuOpened || new Date().getTime() - lastCheck < 1000) return;
  global.anyEvents.SendServer(() => mp.events.callRemote('openCopCarMenu'));
  lastCheck = new Date().getTime();
});*/

mp.keys.bind(Keys.VK_OEM_3, false, () => { // ` key
  if (chatActive || (global.menuOpened && mp.gui.cursor.visible)) return;
  mp.gui.cursor.visible = !mp.gui.cursor.visible;
});

const lastPos = new mp.Vector3(0, 0, 0);

mp.game.gameplay.setFadeInAfterDeathArrest(false);
mp.game.gameplay.setFadeInAfterLoad(false);

let deathTimerOn = false;
let deathTimer = 0;

mp.events.add('DeathTimer', (time) => {
  //mp.console.logInfo("DeathTimer: "+time);
  if (time === false) {
    deathTimerOn = false;
    mp.events.call('DeathTimerClose');
    //mp.players.local.freezePosition(false);
  } else {
    deathTimerOn = true;
    deathTimer = new Date().getTime() + time;
   // mp.console.logInfo("DeathTimer2: "+global.dialog.cBack);
    if (global.dialog.cBack !== 'DEATH_CONFIRM') {
      mp.events.call('DeathTimerOpen');
    }

    //mp.players.local.freezePosition(true);
  }
});

mp.events.add('DeathTimerSet', (time) => {
  global.policeGarage.execute(`deathTimer.set('${time}');`);
});
mp.events.add('DeathTimerKillerSet', (killer) => {
  global.policeGarage.execute(`deathTimer.setKiller('${killer}');`);
});
mp.events.add('DeathTimerOpen', () => {
  //mp.console.logInfo("deathTimerOpen: "+global.policeGarage);
  global.policeGarage.execute(`deathTimer.open();`);
});

mp.events.add('DeathTimerClose', () => {
  global.policeGarage.execute(`deathTimer.close();`);
});


mp.events.add('render', () => {
  if (localplayer.getVariable('InDeath') == true) {
    mp.game.controls.disableAllControlActions(0);
    mp.game.controls.disableAllControlActions(1);
    mp.game.controls.disableAllControlActions(2);
    mp.game.controls.enableControlAction(2, 1, true);
    mp.game.controls.enableControlAction(2, 2, true);
    mp.game.controls.enableControlAction(2, 3, true);
    mp.game.controls.enableControlAction(2, 4, true);
    mp.game.controls.enableControlAction(2, 5, true);
    mp.game.controls.enableControlAction(2, 6, true);
  }

  mp.players.forEachInStreamRange((player) => {
    if (player !== mp.players.local) {
      /*if (player.getVariable('InDeath') == true) {
        player.freezePosition(true);
      }*/
    }
  });

  if (deathTimerOn) {
    var secondsLeft = Math.trunc((deathTimer - new Date().getTime()) / 1000);
    var minutes = Math.trunc(secondsLeft / 60);
    var seconds = secondsLeft % 60;
    if (seconds <= -1) seconds = 0;
    if (Math.trunc(seconds / 10) === 0) seconds = `0${seconds}`;
    mp.events.call('DeathTimerSet', `${minutes}:${seconds}`);
 }

  if (mp.game.controls.isControlPressed(0, 32)
        || mp.game.controls.isControlPressed(0, 33)
        || mp.game.controls.isControlPressed(0, 321)
        || mp.game.controls.isControlPressed(0, 34)
        || mp.game.controls.isControlPressed(0, 35)
        || mp.game.controls.isControlPressed(0, 24)
        || localplayer.getVariable('InDeath') == true) {
    global.afkSecondsCount = 0;
  } else if (localplayer.isInAnyVehicle(false) && localplayer.vehicle.getSpeed() != 0) {
    global.afkSecondsCount = 0;
  } else if (global.spectating) { // Чтобы не кикало администратора в режиме слежки
    global.afkSecondsCount = 0;
  }
});

mp.events.add('playerRuleTriggered', (rule, counter) => {
  if (rule === 'ping' && counter > 5) {
    mp.events.call('notify', 4, 2, 'Ваш ping слишком большой. Зайдите позже', 5000);
    mp.events.callRemote('kickclient');
  }
  /* if (rule === 'packetLoss' && counter => 10) {
        mp.events.call('notify', 4, 2, "У Вас большая потеря пакетов. Зайдите позже", 5000);
        global.anyEvents.SendServer(() => mp.events.callRemote("kickclient"));
    } */
});


// CityRP
// Logger

let NPC_Cam;

mp.events.add('client:ui:debug', (fileName, log, messageToPlayer = '', notify = true) => {
  try {
    if (notify) {
      mp.game1.graphics.notify(
        messageToPlayer === ''
          ? '~r~Ошибка :c ~g~Свяжитесь с администрацией'
          : messageToPlayer,
      );
    }

    const currentdate = new Date();
    const normalizeDate = `${currentdate.getDate()}/${currentdate.getMonth() + 1}/${currentdate.getFullYear()}`;
    const normalizeTime = `${currentdate.getHours()}:${currentdate.getMinutes()}:${currentdate.getSeconds()}`;

    mp.events.callRemote('WriteClientLog', fileName, `Дата и время: ${normalizeDate} | ${normalizeTime}\nИгрок: ${localplayer.name}\nСообщение: ${log}\r\n--------------------------------------------`);
  } catch (e) {
    throw e;
  }
});

mp.events.add('gpsRoute', function (icon, x, y, z) {

  try {
    blipRoute = mp.blips.new(icon, new mp.Vector3(x, y, z),
      {
        color: 59,
        shortRange: false,
        dimension: mp.players.local.dimension
      });

    markerRoute = mp.markers.new(0, new mp.Vector3(x, y, z + 2.0), 1.0,
      {
        direction: new mp.Vector3(x, y, z),
        rotation: new mp.Vector3(0, 0, 0),
        color: [247, 202, 24, 190],
        visible: true,
        dimension: mp.players.local.dimension
      });


    blipRoute.setRoute(true);
  } catch (e) {
    logger.error(e);
  }
});

mp.events.add('gpsUnRoute', function () {
  try {
    if (blipRoute !== undefined && markerRoute !== undefined && markerRoute !== null) {
      blipRoute.setRoute(false);
      blipRoute.destroy();
      markerRoute.destroy();
    }
  } catch (e) {
    logger.error(e);
  }
});

mp.events.add('busRoute', function ( x, y, z, nx, ny, nz) {

  try {
    if (nx == undefined) {
      nx = 0.0;
      ny = 0.0;
      nz = 0.0;
    }

    blipRoute = mp.blips.new(1, new mp.Vector3(x, y, z),
      {
        color: 70,
        shortRange: false,
        dimension: mp.players.local.dimension
      });


    var fixZ = z + 0.1;
    var fixnZ = nz - 0.1;
    markerRoute = mp.checkpoints.new(1, new mp.Vector3(x, y, fixZ), 5.0,
      {
        direction: new mp.Vector3(nx, ny, fixnZ),
        color: [247, 202, 24, 190],
        visible: true,
        dimension: mp.players.local.dimension
      });


    blipRoute.setRoute(true);
  } catch (e) {
    logger.error(e);
  }
});

mp.events.add('busUnRoute', function () {
  try {
    if (blipRoute !== undefined && markerRoute !== undefined && markerRoute !== null && blipRoute !== null) {
      blipRoute.setRoute(false);
      blipRoute.destroy();
      markerRoute.destroy();
    }
  } catch (e) {
    logger.error(e);
  }
});


function offsetPosition(pos, rot, distance) {
  return new mp.Vector3(pos.x + Math.sin(-rot * Math.PI / 180) * distance, pos.y + Math.cos(-rot * Math.PI / 180) * distance, pos.z);
}

mp.events.add('2QUEST::CREATEMARKER', function () {
  try {
    QUEST2Marker.visible = true;
  } catch (e) {
    logger.error(e);
  }
});

let QUEST2Marker = mp.markers.new(42, {x: 145.58028, y :6563.2944,z: 31.994741}, 2,
{
  color: [106,48,145,100],
  visible: false,
});

mp.events.add('2QUEST::DELETEMARKER', function () {
  try {
    QUEST2Marker.visible = false;
  } catch (e) {
    logger.error(e);
  }
});

function offsetPositionXYZ(pos, rot, distance) {
  return { x: pos.x + Math.sin(-rot * Math.PI / 180) * distance, y: pos.y + Math.cos(-rot * Math.PI / 180) * distance, z: pos.z };
}

/*function CamToNPC(position, rotation) {
  try {
    position.z += 0.7; // рост НПС
    const pos = offsetPosition(position, rotation, 0.8); // 0.8 дальность от НПС
    NPC_Cam = mp.cameras.new('default', pos, new mp.Vector3(0, 0, 0), 50);
    NPC_Cam.pointAtCoord(position.x, position.y, position.z - 0.2); // куда смотрит камера (сверзу вниз)
    NPC_Cam.setActive(true);
    mp.game1.cam.renderScriptCams(true, true, 1000, true, false);
  } catch (e) {
    logger.error(e);
  }
}

mp.events.add('CamToNPC', (position, rotation) => {
  try {
    CamToNPC(position, rotation);
    setTimeout(() => {
      mp.players.local.setVisible(false, false);
    }, 500);
  } catch (e) {
    logger.error(e);
  }
});

mp.events.add('DestroyCamToNPC', () => {
  try {
    if (NPC_Cam == null) return;
    NPC_Cam.setActive(false);
    NPC_Cam.destroy();
    mp.game1.cam.renderScriptCams(false, true, 1000, true, true);
    mp.players.local.setVisible(true, false);
    NPC_Cam = null;
  } catch (e) {
    logger.error(e);
  }
});*/


let cartheftblip = null;
mp.events.add('render', () => {
    try {
        let veh = null;

        let frac = localplayer.getVariable('fraction');
        if (frac == 7 || frac == 18) {
            mp.vehicles.forEachInStreamRange(vehicle => {
                if (vehicle.handle === 0)
                    return;
                if (vehicle.getVariable("CARTHEFT") == true) {
                    if (localplayer.vehicle == null || (localplayer.vehicle.getVariable("FRACTION") != 7 && localplayer.vehicle.getVariable("FRACTION") != 18))
                        return;
                    if (mp.game1.gameplay.getDistanceBetweenCoords(vehicle.position.x, vehicle.position.y, vehicle.position.z,
                        localplayer.position.x, localplayer.position.y, localplayer.position.z, false) > 150)
                        return;
                    veh = vehicle;
                }
            });
        }

        if (veh == null) {
            if (cartheftblip != null) {
                cartheftblip.destroy();
                cartheftblip = null;
            }
        } else {
            if (cartheftblip == null) {
                cartheftblip = mp.blips.new(645, new mp.Vector3(veh.position.x, veh.position.y, veh.position.z),
                    {
                        scale: 1,
                        color: 1,
                        shortRange: false,
                        name: "Авто в угоне"
                    });
            } else {
                cartheftblip.setCoords(new mp.Vector3(veh.position.x, veh.position.y, veh.position.z));
            }
        }
    } catch (e) {
        logger.error(e);
    }

});
