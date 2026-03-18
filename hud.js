global.showhud = true;
global.passports = {};

let cruiseSpeed = -1;
let cruiseLastPressed = 0;
let showHint = true;

let waterLevel = 0;
let eatLevel = 0;

const hudstatus = {
  safezone: null, // Last safezone size
  online: 0, // Last online int

  street: null,
  area: null,

  invehicle: false,
  updatespeedTimeout: 0, // Timeout for optimization speedometer
  engine: false,
  belt: false,
  doors: true,
  fuel: 0,
  health: 0,
};

// fishing
let fishingState = 0;
let fishingSuccess = 0;
let fishingBarPosition = 0;
let fishingBarMin = 0;
let fishingBarMax = 0;
let movementRight = true;
let fishingAchieveStart = 0;
let intervalFishing;
const isIntervalCreated = false;
const isInZone = false;
const isShowPrompt = false;
let isEnter = false;
const isjoinTable = false;

mp.events.add('fishingBaitTaken', () => {
  fishingBarMin = 0.277;
  fishingBarMax = 0.675;
  fishingAchieveStart = Math.random() * 0.39 + fishingBarMin;
  isEnter = true;
  fishingBarPosition = 0.476;
  fishingSuccess = 0;
  fishingState = 3;
});

function drawFishingMinigame() {
  if (mp.game.controls.isControlPressed(0, 24) && mp.game.controls.isControlJustPressed(0, 24)) {
    switch (fishingState) {
      case 2:
        fishingState = -1;
        mp.events.callRemote('stopFishDrop');
        isEnter = false;
        break;
      case 3:
        if (fishingBarPosition > fishingAchieveStart - 0.01 && fishingBarPosition < fishingAchieveStart + 0.01) {
          fishingSuccess++;
          if (fishingSuccess == 1) {
            fishingState = -1;
            const heading = localplayer.getHeading() + 90;
            const point = {
              x: localplayer.position.x + 15 * Math.cos(heading * Math.PI / 180.0),
              y: localplayer.position.y + 15 * Math.sin(heading * Math.PI / 180.0),
              z: localplayer.position.z,
            };
            mp.events.callRemote('giveRandomFish');
            isEnter = false;
          } else {
            movementRight = true;
            fishingBarPosition = 0.476;
            fishingAchieveStart = Math.random() * 0.39 + fishingBarMin;
          }
        } else {
          fishingState = -1;
          mp.events.callRemote('stopFishDrop');
          isEnter = false;
        }
        break;
    }
    return;
  }

  if (fishingState == 3) {
    mp.game.graphics.drawRect(0.47, 0.2, 0.39, 0.025, 60, 60, 60, 120);
    // x y w h r g b a
    mp.game.graphics.drawRect(fishingAchieveStart, 0.2, 0.030, 0.025, 0, 255, 0, 255);
    mp.game.graphics.drawRect(fishingBarPosition, 0.19, 0.002, 0.026, 255, 255, 255, 255);
    if (movementRight) {
      fishingBarPosition += 0.001;
      if (fishingBarPosition > fishingBarMax) {
        fishingBarPosition = fishingBarMax;
        movementRight = false;
      }
    } else {
      fishingBarPosition -= 0.001;
      if (fishingBarPosition < fishingBarMin) {
        fishingBarPosition = fishingBarMin;
        movementRight = true;
      }
    }
  }
}

// end fishing
global.playSound = false;
const NOTIFY_SOUND = {
  0 : "notify_alert",
  1 : "notify_error",
  2 : "notify_success",
  3 : "notify_alert",
  4 : "notify_alert",
};
// HUD events
mp.events.add('notify', (type, layout, msg, time = 3000) => {
  // if (global.loggedin) mp.gui.execute(`HUD.pushAlert(${type},${layout},'${msg}',${time})`);
  if (global.loggedin) mp.gui.execute(`notify(${type},${layout},'${msg}',${time})`);
  else mp.events.call('authNotify', type, layout, msg, time);

  if(!global.playSound && mp.storage.data.notifySound){
    global.policeGarage.execute(`client_playMusic('package://cef/sounds/${NOTIFY_SOUND[type]}.mp3', 0.3)`);

    global.playSound = true;
    setTimeout(() => {
      global.playSound = false;
    }, 1000);
  }
});

mp.events.add('showTargetInteraction', (text, playerId = null, playerName = '') => {
  mp.gui.execute(`HUD.sentInteraction('${text}','${playerId}','${playerName}')`);
})

mp.events.add('playerHandshakeTarget', (text, playerId, playerName) => {
  mp.gui.execute(`HUD.sentInteraction('${text}','${playerId}','${playerName}')`);
});

mp.events.add('cancelRequest', () => {
  mp.events.callRemote('cancelPressed');
})

mp.events.add('newFriend', (player) => {
  try {
    if (player && mp.players.exists(player)) {
      let indexFriend = -1;
      //mp.console.logInfo("newFriend before: "+JSON.stringify(mp.storage.data.friends));
      if (mp.storage.data.friends === null || mp.storage.data.friends === undefined) {
        mp.storage.data.friends = [];
        mp.storage.data.friends.push(player.name);
      } else {
        indexFriend = mp.storage.data.friends.indexOf(player.name);
        if (indexFriend === -1) mp.storage.data.friends.push(player.name);
      }

      //mp.console.logInfo("newFriend after: "+JSON.stringify(mp.storage.data.friends));
      mp.storage.flush();
    }
  } catch (e) {
    logger.error(e);
  }
});

mp.events.add('setFriends', (playerName) => {
  //mp.console.logInfo("setFriends before: "+mp.storage.data.friends + " typeof: "+ typeof mp.storage.data.friends);
  if (mp.storage.data.friends === null || mp.storage.data.friends === undefined || typeof mp.storage.data.friends === 'object' && !mp.storage.data.friends.length) {
    mp.storage.data.friends = [];
    mp.storage.data.friends.push(playerName);
  } else {
    const tempFriends = mp.storage.data.friends +"";
    const indexFriend = tempFriends.indexOf(playerName);
    if (indexFriend === -1) mp.storage.data.friends.push(playerName);
  }

  //mp.console.logInfo("setFriends after: "+JSON.stringify(mp.storage.data.friends));
  mp.storage.flush();
  // mp.console.logInfo("setFriends after flush: "+JSON.stringify(mp.storage.data.friends));
})

global.INTERACTIONCHECK = false;
global.showInteraction = true;
mp.events.add('playerInteractionCheck', (state) => {
  global.INTERACTIONCHECK = state;
});

let showhint = false;

mp.events.add('render', () => {
  if (global.showInteraction && global.INTERACTIONCHECK === true && !showhint && !global.openCasino) {
    /*mp.game.ui.resetHudComponentValues(10);
    mp.game.ui.setHudComponentPosition(10, 0.77, 0.92);
    mp.game.ui.setTextComponentFormat('STRING');
    mp.game.ui.addTextComponentSubstringPlayerName('~h~Нажмите ~g~ ~INPUT_CONTEXT~ ~s~ для взаимодействия.');
    mp.game.ui.displayHelpTextFromStringLabel(0, false, true, -1);*/
    showhint = true;

    var list = ["E", "взаимодействовать"];

    if (global.menu !== null) {
      global.menu.execute(`presskeyto.open('${JSON.stringify(list)}')`);
    }

  }
  else if(global.INTERACTIONCHECK === false && showhint){
    if (global.menu !== null) {
      global.menu.execute('presskeyto.close()');
    }

    showhint = false;
  }
});

if(mp.storage.data.hud == undefined)
{
  mp.storage.data.hud = {
    quest : true,
    help : true,
    hud : true,
    playersName : true,
  };
}

if(mp.storage.data.notifySound == undefined)
{
  mp.storage.data.notifySound = true;
}


if(mp.storage.data.aim == undefined)
{
  mp.storage.data.aim = {
    type : 0,
    color : '#FFFFFF',
  };
}


global.showQuest = mp.storage.data.hud.quest;
global.showGlobalHud = mp.storage.data.hud.hud;
global.showHint = mp.storage.data.hud.help;
global.showGamertags = mp.storage.data.hud.playersName;


mp.events.add('showHUD', (show) => {
  if(!global.showGlobalHud) return;
  if (show === undefined || show === null) return;

  global.showhud = show;

  if (!show && global.showHint) mp.gui.execute(`hidehelp(true)`);
  else if (show && global.showHint) mp.gui.execute(`hidehelp(false)`);
  else if (!global.showHint) mp.gui.execute(`hidehelp(true)`);

  if (show) {
    mp.gui.execute(`HUD.server=${serverid};`);
    mp.gui.execute(`HUD.playerId=${mp.players.local.remoteId}`);
  }
  mp.gui.execute(`hidehud(${!global.showhud})`);
  mp.gui.execute(`HUD.setAimData(${mp.storage.data.aim.type}, '${mp.storage.data.aim.color}')`);

  const screen = mp.game.graphics.getScreenActiveResolution(0, 0);
  mp.gui.execute(`updateSafeZoneSize(${screen.x},${screen.y},${hudstatus.safezone})`);

  const minimap = global.init.getMinimapAnchor();
  mp.gui.execute(`HUD.minimapFix=${(minimap.rightX * 100) * 1.2}`);

  const playerId = mp.players.local.getVariable('REMOTE_ID');

  const personId = mp.players.local.getVariable('PERSON_ID');
  mp.gui.execute(`HUD.personId='${personId}'`);
  global.inventoryk.call('inventory_UpdateName', mp.players.local.name);
  if (global.inventoryk === null) {
    //mp.console.logInfo('HTML инвентаря null showHUD UpdateName');
  }

  mp.gui.execute(`HUD.playerId='${playerId}'`);

  mp.game.ui.displayAreaName(global.showhud);
  mp.game.ui.displayRadar(global.showhud);
  mp.game.ui.displayHud(global.showhud);
  mp.gui.chat.show(global.showhud);

  // mp.gui.execute(`HUD.updateHambre(${eatLevel})`);
  // mp.gui.execute(`HUD.updateWater(${waterLevel})`);
});

mp.events.add('toggleQuestPanel', (state) => {
  mp.gui.execute(`HUD.questPanel=${state}`);
  global.showQuest = state;
  mp.storage.data.hud.quest = state;
});

mp.events.add('updateQuestPanel', (data) => {
  mp.gui.execute(`HUD.updateQuestPanel(${data})`);
});

mp.events.add('UpdateMoney', (temp, amount) => {
  const money = temp.toString().replace(/(\d)(?=(\d\d\d)+(?!\d))/g, '$1 ');
  mp.gui.execute(`HUD.money="${money}"`);
  global.inventoryk.call('inventory_UpdateMoney', money);
  if (global.inventoryk === null) {
    //mp.console.logInfo('HTML инвентаря null UpdateMoney');
  }
});

mp.events.add('UpdateBank', (temp, amount) => {
  const money = temp.toString().replace(/(\d)(?=(\d\d\d)+(?!\d))/g, '$1 ');
  mp.gui.execute(`HUD.bank="${money}"`);
});

mp.events.add('setWanted', (lvl) => {
  mp.game.gameplay.setFakeWantedLevel(lvl);
});

const blips = {};
class BlipHelper {
  static createBlip(name, position, color) {
    if (blips.length != 0 && blips[name] !== undefined && blips[name] !== null) {
      blips[name].destroy();
      blips[name] = null;
    }

    const blip = mp.blips.new(1, position, {
      name,
      color,

      shortRange: false,
    });
    blips[name] = blip;
    return blip;
  }

  static createBlipExt(name, position, color, scale, sprite = 0, shortRange = false, bname = null, dimension = 0) {
    //mp.console.logInfo("createBlipExt: name: "+name);
    if (blips.length != 0 && blips[name] !== undefined && blips[name] !== null) {
      blips[name].destroy();
      blips[name] = null;
    }
    let blip;

    if (bname == null) {
      blip = mp.blips.new(1, position, {
        //name: name,
        //color,
        //scale,
        //shortRange,
        dimension
      });
    } else {
      blip = mp.blips.new(1, position, {
        //name: bname,
        //color,
        //scale,
        //shortRange,
        dimension
      });
    }

    blips[name] = blip;
    blips[name].setColour(color);
    blips[name].setAsShortRange(shortRange);
    blips[name].setScale(scale);
    blips[name].name = name;

    if (sprite != 0) blips[name].setSprite(sprite);
    return blip;
  }

  static removeBlip(name) {
    //mp.console.logInfo("removeBlip: name: "+name);
    if (blips.length != 0 && blips[name] !== undefined && blips[name] !== null) {
      blips[name].destroy();
      blips[name] = null;
    }
    // mp.game.ui.removeBlip(blips[name]);
    // blips[name] = null;
  }

  static moveBlip(name, position) {
    if (blips[name] == null) {
      return;
    }
    blips[name].setCoords(position);
  }

  static colorBlip(name, color) {
    if (blips[name] == null) {
      return;
    }
    blips[name].setColour(color);
  }

  static SetRoute(name, enabled) {
    if (blips[name] == null) {
      return;
    }
    blips[name].setRoute(enabled);
  }
}

mp.events.add('blip_create', (name, position, color) => {
  // mp.gui.chat.push('blip created: ->>> ' + name);
  // mp.console.logInfo("blip_create: name: "+name);
  BlipHelper.createBlip(name, position, color);
  BlipHelper.colorBlip(name, color);
});
mp.events.add('blip_create_ext', (name, position, color, size, sprite = 0, range = false, bname = null, dimension = 0) => {
  //mp.console.logInfo("blip_create_ext: name: "+name);
  BlipHelper.createBlipExt(name, position, color, size, sprite, range, bname, dimension);
  BlipHelper.colorBlip(name, color);
});
mp.events.add('blip_remove', (name) => {
  // mp.gui.chat.push('blip removed: ->>> ' + name);
  // mp.gui.chat.push('all blips: ' + blips);
  //mp.console.logInfo("blip_remove: name: ->>>  "+name);
  BlipHelper.removeBlip(name);
});
mp.events.add('blip_move', (name, position) => {
  // mp.gui.chat.push('blip MOVED: ->>> ' + name);
  BlipHelper.moveBlip(name, position);
});
mp.events.add('blip_color', (name, color) => {
  BlipHelper.colorBlip(name, color);
});
mp.events.add('blip_setRoute', (name, enabled) => {
  //mp.console.logInfo("blip_setRoute: name: ->>>  "+name+" status: " + enabled);
  // mp.gui.chat.push('blip setRoute: ->>> ' + name + " status: " + enabled);
  BlipHelper.SetRoute(name, enabled);
});

mp.keys.bind(Keys.VK_F5, false, () => { // F5 key
  if (global.menuOpened) {
    global.HTMLMenuClose();
    global.anyMenuHTML.closeAllMenu();
    global.menuClose();
  }

    global.showhud = !global.showhud;
    mp.events.call('showHUD', global.showhud);
});

var freezeBoat = false;

global.seatBelt = function(){ // belt system (J key)
  if (!global.loggedin || chatActive || editing || new Date().getTime() - lastCheck < 400 || global.menuOpened) return;
  if(localplayer.vehicle.getClass() == 14){
    if (localplayer.vehicle.getSpeed() > (3 * 3.6)) return;
    freezeBoat = !freezeBoat;
    mp.game1.invoke('0x75DBEC174AEEAD10', localplayer.vehicle.handle, freezeBoat);
    mp.game1.invoke('0xE3EBAAE484798530', localplayer.vehicle.handle, freezeBoat);
    mp.events.callRemote('setAnchor', freezeBoat);
  }
  else {
    if (localplayer.isInAnyVehicle(false)) {
      global.lastCheck = new Date().getTime();

      if (hudstatus.belt) {
        localplayer.setConfigFlag(32, true);
        mp.events.call('notify', 2, 9, "Вы отстегнули ремень безопасности", 2000);
      } else {
        localplayer.setConfigFlag(32, false);
        mp.events.call('notify', 2, 9, "Вы пристегнули ремень безопасности", 2000);
      }

      hudstatus.belt = !hudstatus.belt;
      mp.gui.execute(`HUD.belt=${hudstatus.belt}`);

      const testBelt = localplayer.getConfigFlag(32, true);
      // mp.gui.chat.push(`flag32: ` + testBelt + ` hud.belt ` + hudstatus.belt);

      mp.events.callRemote('beltCarPressed', testBelt);
    }
  }
};

// CRUISE CONTROL //
/*mp.keys.bind(getKeyBy('TOGGLE_CRUISE_CONTROL'), false, () => { // 5 key - cruise mode on/off
  if (!global.loggedin || global.chatActive || editing || global.menuOpened) return;
  if (!localplayer.isInAnyVehicle(true) || localplayer.vehicle.getPedInSeat(-1) != localplayer.handle) return;
  const vclass = localplayer.vehicle.getClass();
  if (vclass == 14 || vclass == 15 || vclass == 16) return;
  if (localplayer.vehicle.isOnAllWheels() == false) return;
  if (new Date().getTime() - cruiseLastPressed < 300) {
    mp.events.call('popup::openInput', 'Круиз-контроль', 'Укажите скорость в км/ч', 3, 'setCruise');
  } else {
    const veh = localplayer.vehicle;
    if (cruiseSpeed == -1) {
      const vspeed = veh.getSpeed();
      if (vspeed > 1) {
        veh.setMaxSpeed(vspeed);
        mp.gui.execute('HUD.cruiseColor=\'#eebe00\'');
        cruiseSpeed = vspeed;
      }
    } else {
      cruiseSpeed = -1;
      veh.setMaxSpeed(mp.game.vehicle.getVehicleModelMaxSpeed(veh.model));
      mp.gui.execute('HUD.cruiseColor=\'#ffffff\'');
    }
  }
  cruiseLastPressed = new Date().getTime();
});*/


global.cruiseControl = function() { // 5 key - cruise mode on/off
  if (!global.loggedin || global.chatActive || editing || global.menuOpened) return;
  if (!localplayer.isInAnyVehicle(true) || localplayer.vehicle.getPedInSeat(-1) != localplayer.handle) return;
  const vclass = localplayer.vehicle.getClass();
  if (vclass == 14 || vclass == 15 || vclass == 16) return;
  if (localplayer.vehicle.isOnAllWheels() == false) return;
  if (new Date().getTime() - cruiseLastPressed < 300) {
    mp.events.call('popup::openInput', 'Круиз-контроль', 'Укажите скорость в км/ч', 3, 'setCruise');
  } else {
    const veh = localplayer.vehicle;
    if (cruiseSpeed == -1) {
      const vspeed = veh.getSpeed();
      if (vspeed > 1) {
        veh.setMaxSpeed(vspeed);
        mp.gui.execute('HUD.cruiseColor=\'#eebe00\'');
        cruiseSpeed = vspeed;
      }
    } else {
      cruiseSpeed = -1;
      veh.setMaxSpeed(mp.game.vehicle.getVehicleModelMaxSpeed(veh.model));
      mp.gui.execute('HUD.cruiseColor=\'#ffffff\'');
    }
  }
  cruiseLastPressed = new Date().getTime();
};

mp.events.add('setCruiseSpeed', (speed) => {
  speed = parseInt(speed, 10);
  if (speed === NaN || speed < 1) return;
  if (!localplayer.isInAnyVehicle(true) || localplayer.vehicle.getPedInSeat(-1) != localplayer.handle) return;
  const vclass = localplayer.vehicle.getClass();
  if (vclass == 14 || vclass == 15 || vclass == 16) return;
  if (localplayer.vehicle.isOnAllWheels() == false) return;
  const veh = localplayer.vehicle;
  const curSpeed = veh.getSpeed();
  if (speed < curSpeed) {
    mp.events.call('notify', 4, 9, 'Нельзя установить скорость меньше, чем она есть на данный момент, снизьте скорость и попробуйте еще раз.', 6000);
    return;
  }
  speed /= 3.6; // convert from kph to mps
  const maxSpeed = mp.game.vehicle.getVehicleModelMaxSpeed(veh.model);
  if (speed > maxSpeed) speed = maxSpeed;
  veh.setMaxSpeed(speed);
  mp.gui.execute('HUD.cruiseColor=\'#eebe00\'');
  cruiseSpeed = speed;
});

mp.events.add('newPassport', (player, pass) => {
  if (player && mp.players.exists(player)) global.passports[player.name] = pass;
});

let showAltTabHint = false;
mp.events.add('showAltTabHint', () => {
  showAltTabHint = true;
  setTimeout(() => { showAltTabHint = false; }, 10000);
});

mp.events.add('sendRPMessage', (type, msg, players) => {
  let chatcolor = '';

  players.forEach((id) => {
    const player = mp.players.atRemoteId(id);
    if (mp.players.exists(player)) {
      if (type === 'chat' || type === 's') {
        const localPos = localplayer.position;
        const playerPos = player.position;
        const dist = mp.game.system.vdist(playerPos.x, playerPos.y, playerPos.z, localPos.x, localPos.y, localPos.z);
        const color = (dist < 2) ? 'FFFFFF'
          : (dist < 4) ? 'F7F9F9'
            : (dist < 6) ? 'DEE0E0'
              : (dist < 8) ? 'C5C7C7' : 'ACAEAE';

        chatcolor = color;
      }

      let name = '';
      if (player.getVariable('IS_MASK') == true) {
        name = (player === localplayer || localplayer.getVariable('IS_ADMIN') == true) ? `${player.name.replace('_', ' ')} (${player.getVariable('REMOTE_ID')})` : `Незнакомец (${id})`;
      } else {
        name = (player === localplayer || localplayer.getVariable('IS_ADMIN') == true || global.passports[player.name] != undefined || mp.storage.data.friends[player.name] != undefined) ? `${player.name.replace('_', ' ')} (${player.getVariable('REMOTE_ID')})` : `Незнакомец (${id})`;
      }
      msg = msg.replace('{name}', name);
    }
  });

  if (type === 'chat' || type === 's') msg = `!{#${chatcolor}}${msg}`;

  mp.gui.chat.push(msg);
});

mp.events.add('render', (nametags) => {
  if (!global.loggedin) return;
  if (fishingState > 0) {
    drawFishingMinigame();
  }
  // Disable HUD components.
  mp.game.ui.hideHudComponentThisFrame(2); // HUD_WEAPON_ICON
  mp.game.ui.hideHudComponentThisFrame(3); // HUD_CASH
  mp.game.ui.hideHudComponentThisFrame(6); // HUD_VEHICLE_NAME
  mp.game.ui.hideHudComponentThisFrame(7); // HUD_AREA_NAME
  mp.game.ui.hideHudComponentThisFrame(8); // HUD_VEHICLE_CLASS
  mp.game.ui.hideHudComponentThisFrame(9); // HUD_STREET_NAME

  mp.game.ui.hideHudComponentThisFrame(19); // HUD_WEAPON_WHEEL
  mp.game.ui.hideHudComponentThisFrame(20); // HUD_WEAPON_WHEEL_STATS
  mp.game.ui.hideHudComponentThisFrame(22); // MAX_HUD_WEAPONS

  // Update online counter in logotype.
  if (hudstatus.online != mp.players.length) {
    hudstatus.online = mp.players.length;
    mp.gui.execute(`HUD.online=${hudstatus.online}`);
  }

  // Update street & district
  const street = mp.game.pathfind.getStreetNameAtCoord(localplayer.position.x, localplayer.position.y, localplayer.position.z, 0, 0);
  const area = mp.game.zone.getNameOfZone(localplayer.position.x, localplayer.position.y, localplayer.position.z);
  if (hudstatus.street != street || hudstatus.area != area) {
    hudstatus.street = street;
    hudstatus.area = area;

    mp.gui.execute(`HUD.street='${mp.game.ui.getStreetNameFromHashKey(street.streetName)}'`);
    mp.gui.execute(`HUD.crossingRoad='${mp.game.ui.getLabelText(hudstatus.area)}'`);
  }

  // Update CEF safezone.
  const lastsafezone = mp.game.graphics.getSafeZoneSize();
  if (lastsafezone != hudstatus.safezone) {
    hudstatus.safezone = lastsafezone;
    const resolution = mp.game.graphics.getScreenActiveResolution(0, 0);
    mp.gui.execute(`updateSafeZoneSize(${resolution.x},${resolution.y},${hudstatus.safezone})`);
  }

  if (localplayer.isInAnyVehicle(false)) {
    if (localplayer.vehicle.getPedInSeat(-1) == localplayer.handle) {
      if (!hudstatus.invehicle) mp.gui.execute('HUD.inVeh=1');
      hudstatus.invehicle = true;

      const veh = localplayer.vehicle;

      // if (veh.getVariable('FUELTANK') !== undefined) {
      //   const fueltank = veh.getVariable('FUELTANK');
      //   mp.game.graphics.drawText(`Загружено: ${fueltank}/1000л`, [0.93, 0.80], {
      //     font: 0,
      //     color: [255, 255, 255, 185],
      //     scale: [0.4, 0.4],
      //     outline: true,
      //   });
      // }
      if (veh.getVariable('PETROL') !== undefined && veh.getVariable('MAXPETROL') !== undefined) {
        const petrol = veh.getVariable('PETROL');
        const maxpetrol = veh.getVariable('MAXPETROL');

        if (petrol >= 0) {
          let petrolPercent = 100;
          if((petrol / maxpetrol * 100) > 100)
            petrolPercent = 100;
          else
            petrolPercent = petrol / maxpetrol * 100;
          mp.gui.execute(`HUD.changeFuel(${petrolPercent})`);
          hudstatus.fuel = petrol;

          let ifuel = 2;

          if (petrol <= (maxpetrol * 0.2)) ifuel = 0;
          else if (petrol <= (maxpetrol * 0.6)) ifuel = 1;
          else ifuel = 2;
          mp.gui.execute(`HUD.ifuel=${ifuel}`);
        }
      }

      const engine = veh.getIsEngineRunning();
      if (engine != null && engine !== hudstatus.engine) {
        if (engine == true) mp.gui.execute('HUD.engine=1');
        else mp.gui.execute('HUD.engine=0');

        hudstatus.engine = engine;
      }

      if (veh.getVariable('LOCKED') !== undefined) {
        const locked = veh.getVariable('LOCKED');

        if (hudstatus.doors !== locked) {
          if (locked == true) mp.gui.execute('HUD.doors=0');
          else mp.gui.execute('HUD.doors=1');

          hudstatus.doors = locked;
        }
      }

      let hp = veh.getHealth() / 10;
      hp = hp.toFixed();
      if (hp !== hudstatus.health) {
        mp.gui.execute(`HUD.hp=${hp}`);
        hudstatus.health = hp;
      }

      if (new Date().getTime() - hudstatus.updatespeedTimeout > 50) {
        const speed = (veh.getSpeed() * 3.6).toFixed();
        mp.gui.execute(`HUD.changeSpeed(${speed})`);
        hudstatus.updatespeedTimeout = new Date().getTime();

        if (cruiseSpeed != -1) // kostyl'
        { veh.setMaxSpeed(cruiseSpeed); }
      }
    }
  } else {
    if (hudstatus.invehicle) mp.gui.execute('HUD.inVeh=0');
    hudstatus.invehicle = false;
    hudstatus.belt = false;
    mp.gui.execute(`HUD.belt=${hudstatus.belt}`);
  }
});

mp.events.add('UpdateEat', (value, init = false) => {
  mp.gui.execute(`HUD.eat=${value}`);
  // mp.gui.execute(`HUD.updateHambre(${value}, ${init})`);
  eatLevel = value;
  // console.log('eatLevel: ', value);
  global.inventoryk.call('inventory_UpdateEat', value);
  if (global.inventoryk === null) {
    //mp.console.logInfo('HTML инвентаря null UpdateEat');
  }
  // mp.gui.chat.push("inventory_UpdateEat value:" + value);
  if (value == 0) {
    // if (!localplayer.vehicle)
    //     localplayer.setToRagdoll(1000, 1000, 4, false, false, false);
  }
});

mp.events.add('UpdateWater', (value, init = false) => {
  mp.gui.execute(`HUD.water=${value}`);
  waterLevel = value;
  // console.log('waterLevel: ', value);
  global.inventoryk.call('inventory_UpdateWater', value);
  if (global.inventoryk === null) {
    //mp.console.logInfo('HTML инвентаря null UpdateWater');
  }
  // mp.gui.chat.push("inventory_UpdateWater value:" + value);
  if (value == 0) {
    // if (!localplayer.vehicle)
    // localplayer.setToRagdoll(1000, 1000, 4, false, false, false);
  }
});

mp.events.add('updlastbonus', (bonus) => {
  mp.gui.execute(`HUD.lastbonus="${bonus}"`);
});

mp.events.add('ShowPrompt', (Prompt) => {
  mp.gui.execute(`prompt.showByName('${Prompt}')`);
});
mp.events.add('HidePrompt', () => {
  mp.gui.execute('prompt.hide()');
});
mp.keys.bind(Keys.VK_O, false, () => { // O key
  if (!global.loggedin || chatActive || editing || new Date().getTime() - lastCheck < 1000 || global.menuOpened) return;
  global.anyEvents.SendServer(() => mp.events.callRemote('RobberyHouse'));
});

mp.events.add('showFortune', (hour, minute) => {
  mp.gui.execute(`HUD.showFortune(${hour},${minute})`);
});
