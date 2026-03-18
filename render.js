global.entity = null;
global.nearestObject = null;

const lastEntCheck = 0;
const checkInterval = 200;

const backlightColor = [196, 17, 21];

let blockcontrols = false;
global.cuffed = false;
let hasmoney = false;

let lastCuffUpdate = new Date().getTime();

function getLookingAtEntity() {
  const startPosition = localplayer.getBoneCoords(12844, 0, 0, 0);
  const resolution = mp.game.graphics.getScreenActiveResolution(1, 1);
  const secondPoint = mp.game.graphics.screen2dToWorld3d([resolution.x / 2, resolution.y / 2, (2 | 4 | 8)]);
  if (secondPoint == undefined) return null;

  startPosition.z -= 0.3;
  const result = mp.raycasting.testPointToPoint(startPosition, secondPoint, localplayer, (2 | 4 | 8 | 16));

  if (typeof result !== 'undefined') {
    if (typeof result.entity.type === 'undefined') return null;
    if (result.entity.type == 'object' && result.entity.getVariable('TYPE') == undefined) return null;

    const entPos = result.entity.position;
    const lPos = localplayer.position;
    if (mp.game.gameplay.getDistanceBetweenCoords(entPos.x, entPos.y, entPos.z, lPos.x, lPos.y, lPos.z, true) > 8) return null;
    return result.entity;
  }
  return null;
}

function getNearestObjects() {
  let tempO = null;
  if (localplayer.isInAnyVehicle(false)) {
    var players = mp.players.toArray();
    players.forEach(
      (player) => {
        const posL = localplayer.position;
        const posO = player.position;
        const distance = mp.game.gameplay.getDistanceBetweenCoords(posL.x, posL.y, posL.z, posO.x, posO.y, posO.z, true);
        if (localplayer != player && localplayer.dimension === player.dimension && distance < 3) {
          if (tempO === null) tempO = player;
          else if (mp.game.gameplay.getDistanceBetweenCoords(posL.x, posL.y, posL.z, posO.x, posO.y, posO.z, true)
                        < mp.game.gameplay.getDistanceBetweenCoords(posL.x, posL.y, posL.z, tempO.position.x, tempO.position.y, tempO.position.z, true)) tempO = player;
        }
      },
    );
  } else {
    var objects = mp.objects.toArray();
    objects.forEach(
      (object) => {
        const posL = localplayer.position;
        const posO = object.position;
        const distance = mp.game.gameplay.getDistanceBetweenCoords(posL.x, posL.y, posL.z, posO.x, posO.y, posO.z, true);
        if (object.getVariable('TYPE') != undefined && localplayer.dimension === object.dimension && distance < 3) {
          if (tempO === null) tempO = object;
          else if (mp.game.gameplay.getDistanceBetweenCoords(posL.x, posL.y, posL.z, posO.x, posO.y, posO.z, true)
                        < mp.game.gameplay.getDistanceBetweenCoords(posL.x, posL.y, posL.z, tempO.position.x, tempO.position.y, tempO.position.z, true)) tempO = object;
        }
      },
    );
  }
  global.nearestObject = tempO;
}

mp.events.add('blockMove', (argument) => {
  blockcontrols = argument;
});

mp.events.add('CUFFED', (argument) => {
  global.cuffed = argument;
});

mp.events.add('hasMoney', (argument) => {
  hasmoney = argument;
  if (!argument) localplayer.setEnableHandcuffs(false);
});

mp.keys.bind(Keys.VK_G, false, function () { // G key
  try {
      if (!global.loggedin || global.editing || global.phoneOpen || global.animMenuOpen || global.statsOpen || global.menuCheck() || global.taxiPriceOpen || cuffed || global.cutscenePlaying) return;

      if (global.circleOpen) {
          mp.events.call('circle:close');
          return;
      }
      if (!global.loggedin || chatActive || new Date().getTime() - lastCheck < 300 || localplayer.getVariable('InDeath') == true && !localplayer.isInAnyVehicle(false)) return;

      if (global.entity !== null) {
          switch (global.entity.type) {
              case "object":
                  if (entity && mp.objects.exists(entity)) {
                      mp.events.callRemote('oSelected', entity);
                  }
                global.entity = null;
                  return;
              case "player":
                  mp.events.call('circle:showMenu', 'player');
                  return;
              case "vehicle":
                  if(entity.hasVariable("BLOCKED")) return;

                  if (global.handBox) {
                    mp.events.callRemote('SERVER:TRUCKER:put_box', entity);
                    break;
                  }
                  if(global.unloadVeh != null && global.unloadVeh == entity){
                    mp.events.callRemote('vehicleSelected', entity, 'getbox');
                    break;
                  }
                  mp.events.call('circle:showMenu', 'vehicle');

                  return;
          }
      } else {
          mp.events.call('circle:showMenu', 'self');
      }

      global.lastCheck = new Date().getTime();
  } catch (e) {
      logger.error(e);
  }
});

mp.keys.bind(0x71, false, () => { // F2 key
  if (global.menuCheck() || cuffed || localplayer.getVariable('InDeath') == true) return;
  // player
  if (circleOpen) {
    CloseCircle();
    return;
  }
  if (!global.loggedin || chatActive || nearestObject == null || new Date().getTime() - lastCheck < 1000) return;

  if (nearestObject && nearestObject.type == 'object' && mp.objects.exists(nearestObject)) {
    mp.events.callRemote('oSelected', nearestObject);
  } else if (nearestObject && mp.players.exists(nearestObject)) {
    global.entity = nearestObject;
    mp.gui.cursor.visible = true;
    OpenCircle('Игрок', 0);
  }

  global.lastCheck = new Date().getTime();
});

let truckorderveh = null;

mp.events.add('SetOrderTruck', (vehicle) => {
  try {
    if (truckorderveh == null) truckorderveh = vehicle;
    else truckorderveh = null;
  } catch (e) {
  }
});

global.aimShow = false;

function AimToggle(toggle){
  global.aimShow = toggle;
  mp.gui.execute(`HUD.toggleAim(${toggle})`);
}

mp.events.add('render', () => {

  /*if(mp.game.invoke("0x68EDDA28A5976D07")){

   // mp.game.graphics.drawText('-', [0.5, 0.5 ], { font: 0, color: [255, 255, 255, 255], scale: [0.35, 0.35], outline: true });
    mp.game.graphics.drawText('+', [0.5, 0.49 ], { font: 0, color: [255, 255, 255, 255], scale: [0.5, 0.5], outline: true });
   // mp.game.graphics.drawText('123123123123123123123123123123', [0.4, 0.3 ], { font: 0, color: [255, 255, 255, 255], scale: [1.5, 1.5], outline: true });
  }*/
  try {
    if (!global.loggedin) return;

    if(global.hasFollower){
      mp.game.controls.disableControlAction(0, 21, true); // sprint
      mp.game.controls.disableControlAction(0, 22, true); // jump
      mp.game.controls.disableControlAction(2, 21, true); // sprint for test (misha)
      mp.game.controls.disableControlAction(32, 21, true); // sprint for test (misha)

      mp.game.controls.disableControlAction(2, 24, true);
      mp.game.controls.disableControlAction(2, 69, true);
      mp.game.controls.disableControlAction(2, 70, true);
      mp.game.controls.disableControlAction(2, 92, true);
      mp.game.controls.disableControlAction(2, 114, true);
      mp.game.controls.disableControlAction(2, 121, true);
      mp.game.controls.disableControlAction(2, 140, true);
      mp.game.controls.disableControlAction(2, 141, true);
      mp.game.controls.disableControlAction(2, 142, true);
      mp.game.controls.disableControlAction(2, 257, true);
      mp.game.controls.disableControlAction(2, 263, true);
      mp.game.controls.disableControlAction(2, 264, true);
      mp.game.controls.disableControlAction(2, 331, true);
    }

    if(mp.game.invoke("0x68EDDA28A5976D07") && mp.storage.data.aim.type != 0 && !global.aimShow){
      AimToggle(true);
    }
    else if(!mp.game.invoke("0x68EDDA28A5976D07") && global.aimShow){
      AimToggle(false);
    }

    if(global.aimShow){
      mp.game.ui.hideHudComponentThisFrame(14);
    }

    if (global.pressedraw) {
      mp.game.graphics.drawText('', [0.10, 0.75], {
        font: 0,
        color: [255, 255, 255, 185],
        scale: [0.35, 0.35],
        outline: true,
      });
    }

    if (global.pedsaying != null) {
      const pos = global.pedsaying.getBoneCoords(12844, 0.5, 0, 0);
      mp.game.graphics.drawText(global.pedtext, [pos.x, pos.y, pos.z + 0.1], {
        font: 0,
        color: [255, 255, 255, 185],
        scale: [0.35, 0.35],
        outline: true,
      });
      if (global.pedtext2 != null) {
        const pos = global.pedsaying.getBoneCoords(12844, 0.5, 0, 0);
        mp.game.graphics.drawText(global.pedtext2, [pos.x, pos.y, pos.z + 0.017], {
          font: 0,
          color: [255, 255, 255, 185],
          scale: [0.35, 0.35],
          outline: true,
        });
      }
    }
    if (!global.admingm) localplayer.setInvincible(false);
    if (localplayer.isSprinting() || localplayer.isOnAnyBike()) mp.game.player.restoreStamina(100);
    mp.game.player.setLockonRangeOverride(1.5);
    mp.game.controls.disableControlAction(1, 7, true);
    // thanks to kemperrr
    if (mp.game.invoke(getNative('IS_CUTSCENE_ACTIVE'))) {
      mp.game.invoke(getNative('STOP_CUTSCENE_IMMEDIATELY'));
    }

    if (mp.game.invoke(getNative('GET_RANDOM_EVENT_FLAG'))) {
      mp.game.invoke(getNative('SET_RANDOM_EVENT_FLAG'), false);
    }

    if (mp.game.invoke(getNative('GET_MISSION_FLAG'))) {
      mp.game.invoke(getNative('SET_MISSION_FLAG'), false);
    }

    if (global.pocketEnabled) {
      mp.game.controls.disableControlAction(2, 0, true);
    }

    if (blockcontrols) {
      mp.game.controls.disableAllControlActions(2);
      mp.game.controls.enableControlAction(2, 30, true);
      mp.game.controls.enableControlAction(2, 31, true);
      mp.game.controls.enableControlAction(2, 32, true);
      mp.game.controls.enableControlAction(2, 1, true);
      mp.game.controls.enableControlAction(2, 2, true);
    }
    if (hasmoney) {
      localplayer.setEnableHandcuffs(true);
    }

    if (mp.keys.isDown(32) && cuffed && new Date().getTime() - lastCuffUpdate >= 3000) {
      mp.events.callRemote('cuffUpdate');
      lastCuffUpdate = new Date().getTime();
    }

    if (!localplayer.isInAnyVehicle(false) && !localplayer.isDead()) {
      if (!circleOpen) global.entity = getLookingAtEntity();
      getNearestObjects();
      if (entity != null && entity.getVariable('INVISIBLE') == true) entity = null;
    } else {
      getNearestObjects();
      if (entity != nearestObject) global.entity = null;
    }

    if (nearestObject != null && (entity == null || entity.type != 'object') && nearestObject.type != 'player' && nearestObject.type != 'vehicle') {
      mp.game.graphics.drawText('F2', [nearestObject.position.x, nearestObject.position.y, nearestObject.position.z], {
        font: 0,
        color: [255, 255, 255, 185],
        scale: [0.4, 0.4],
        outline: true,
      });
      nearestObject.setCollision(false, true);

    } else if (entity != null && !localplayer.isInAnyVehicle(false)) {
      if (truckorderveh == null || entity != truckorderveh) {
        if(entity.type == "player"){
          /*mp.game1.graphics.drawMarker(20, // Стрелка сверху
            entity.position.x, entity.position.y, entity.position.z + 1,
            0, 0, 0,
            180, 0, 0,
            0.3, 0.3, 0.3,
            255, 0, 0, 125,
            false, false, 2,
            true, "", "", false);
          mp.game1.graphics.drawMarker(27, // Круг под ногами
              entity.position.x, entity.position.y, entity.position.z - 1,
              0, 0, 0,
              0, 0, 0,
              1.0, 1.0, 1.0,
              255, 0, 0, 125,
              false, false, 2,
              true, "", "", false);*/
          mp.game.graphics.drawText('G (взаимодействие)', [entity.position.x, entity.position.y, entity.position.z], {
                font: 4,
                color: [255, 255, 255, 185],
                scale: [0.3, 0.3],
                outline: true,
              });
        }
        else if(entity.type == "vehicle") {
          if(entity.getEngineHealth() > 0 && !entity.hasVariable("BLOCKED")){
          let rotations = entity.getRotation(5);
          let vector = new mp.Vector3(cos(rotations.x) * sin(rotations.y), sin(rotations.x), cos(rotations.x) * cos(rotations.y));//

          let dis = entity.getHeight(entity.position.x, entity.position.y, entity.position.z, true, false) + 0.5;

          mp.game1.graphics.drawMarker(20,
            entity.position.x + vector.x * dis, entity.position.y + (vector.y * -1) * dis, entity.position.z + vector.z * dis,
            0, 0, 0,
            rotations.x - 180, rotations.y, rotations.z,
            1.0, 1.0, 1.0,
            255, 255, 255, 125,
            false, false, 2,
            false, "", "", false);
          if(entity.getVariable('exclusivePrice'))
          {
            mp.game.graphics.drawText(`${entity.getVariable('exclusivePrice').toLocaleString('ru')} $`, [entity.position.x, entity.position.y, entity.position.z], {
              font: 4,
              color: [255, 255, 255, 225],
              scale: [0.5, 0.5],
              outline: true,
            });
          }
          else {
          mp.game.graphics.drawText('G (взаимодействие)', [entity.position.x, entity.position.y, entity.position.z], {
            font: 4,
            color: [255, 255, 255, 185],
            scale: [0.3, 0.3],
            outline: true,
          });
          }
        }
        }
      } else if (entity == truckorderveh) {
        mp.game.graphics.drawText('Ваш заказ', [entity.position.x, entity.position.y, entity.position.z], {
          font: 1,
          color: [255, 255, 255, 255],
          scale: [1.2, 1.2],
          outline: true,
        });
      }
    }
  } catch (e) {
    mp.game.graphics.notify(`RE:${e.toString()}`);
  }
});

function cos(n){
  return mp.game1.system.cos(n);
}

function sin(n){
  return mp.game1.system.sin(n);
}

function abs(n){
  return mp.game1.gameplay.absf(n);
}
