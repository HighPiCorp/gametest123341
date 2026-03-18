let ammosweap = 0;
let givenWeapon = -1569615261;
let currentAmmo = 0;

mp.game1.weapon.unequipEmptyWeapons = false;
// mp.game.weapon.unequipEmptyWeapons = false;

const SET_CURRENT_PED_WEAPON = '0xADF692B254977C0C';
mp.game.ped.setAiWeaponDamageModifier(0.5);
mp.game.ped.setAiMeleeWeaponDamageModifier(0.4);

mp.game.player.setMeleeWeaponDefenseModifier(0.25);
mp.game.player.setWeaponDefenseModifier(1.3);

let resistStages = {
  0: 0.0,
  1: 0.05,
  2: 0.07,
  3: 0.1,
};

const canUseInCar = [
  453432689,
  1593441988,
  -1716589765,
  -1076751822,
  -771403250,
  137902532,
  -598887786,
  -1045183535,
  584646201,
  911657153,
  1198879012,
  324215364,
  -619010992,
  -1121678507,
];

const Natives = {
  GIVE_WEAPON_COMPONENT_TO_PED: "0xD966D51AA5B28BB9",
  REMOVE_WEAPON_COMPONENT_FROM_PED: "0x1E8BE90C74FB4C09",
  SET_CURRENT_PED_WEAPON: "0xADF692B254977C0C"
};

let blackList = [
  -1569615261,
  -1716189206,
  1737195953,
  1317494643,
  -1786099057,
  -2067956739,
  1141786504,
  -102323637,
  -1834847097,
  -102973651,
  -656458692,
  -581044007,
  -1951375401,
  -538741184,
  -1810795771,
  419712736,
  -853065399
];

const currentWeapon = () => mp.game.invoke(getNative("GET_SELECTED_PED_WEAPON"), localplayer.handle);

mp.keys.bind(Keys.VK_R, false, function () {
  // R key
  try {
    if (!global.loggedin || chatActive || new Date().getTime() - global.lastCheck < 1000 || mp.gui.cursor.visible) return;
    let current = currentWeapon();
    // mp.console.logInfo("current weapon: -> "+current);

    if (current == -1569615261 || current == 911657153) return;

    const ammo = mp.game.invoke(getNative("GET_AMMO_IN_PED_WEAPON"), localplayer.handle, current);
    //mp.console.logInfo(`maxClips: ${mp.game.weapon.getWeaponClipSize(current)} ammo: ${ammo}`);

    if (mp.game.weapon.getWeaponClipSize(current) == ammo) return;

    global.anyEvents.SendServer(() => mp.events.callRemote("SERVER::weapon:playerReload", current));
    global.lastCheck = new Date().getTime();
  } catch(e) { logger.error(e); }
});

mp.events.add('render', () => {
  try {

    // others //
    mp.game.controls.disableControlAction(1, 243, true); // Default cheat panel Disable
    mp.game.controls.disableControlAction(2, 243, true); // INPUT_ENTER_CHEAT_CODE

    mp.game.controls.disableControlAction(2, 45, true); // reload control

    // vehicle restrict actions //
    mp.game.controls.disableControlAction(2, 99, true); // INPUT_VEH_SELECT_NEXT_WEAPON
    mp.game.controls.disableControlAction(2, 100, true); // INPUT_VEH_SELECT_PREV_WEAPON

    mp.game.controls.disableControlAction(32, 68, true); // vehicle weapon
    mp.game.controls.disableControlAction(32, 70, true); // vehicle weapon
    mp.game.controls.disableControlAction(32, 73, true); // vehicle weapon
    mp.game.controls.disableControlAction(32, 69, true); // vehicle weapon
    mp.game.controls.disableControlAction(32, 354, true); // vehicle weapon
    mp.game.controls.disableControlAction(32, 357, true); // vehicle weapon
    mp.game.controls.disableControlAction(32, 105, true); // vehicle weapon
    mp.game.controls.disableControlAction(32, 350, true); // vehicle weapon
    mp.game.controls.disableControlAction(32, 351, true); // vehicle weapon

    //     weapon switch controls       //
    mp.game.controls.disableControlAction(2, 12, true); // INPUT_WEAPON_WHEEL_UD
    mp.game.controls.disableControlAction(2, 13, true); // INPUT_WEAPON_WHEEL_LR
    mp.game.controls.disableControlAction(2, 14, true); // INPUT_WEAPON_WHEEL_NEXT
    mp.game.controls.disableControlAction(2, 15, true); // INPUT_WEAPON_WHEEL_PREV
    mp.game.controls.disableControlAction(2, 16, true); // INPUT_SELECT_NEXT_WEAPON
    mp.game.controls.disableControlAction(2, 17, true); // INPUT_SELECT_PREV_WEAPON

    mp.game.controls.disableControlAction(2, 37, true); // INPUT_SELECT_WEAPON

    mp.game.controls.disableControlAction(2, 157, true); // INPUT_SELECT_WEAPON_UNARMED
    mp.game.controls.disableControlAction(2, 158, true); // INPUT_SELECT_WEAPON_MELEE
    mp.game.controls.disableControlAction(2, 159, true); // INPUT_SELECT_WEAPON_HANDGUN
    mp.game.controls.disableControlAction(2, 160, true); // INPUT_SELECT_WEAPON_SHOTGUN
    mp.game.controls.disableControlAction(2, 161, true); // INPUT_SELECT_WEAPON_SMG
    mp.game.controls.disableControlAction(2, 162, true); // INPUT_SELECT_WEAPON_AUTO_RIFLE
    mp.game.controls.disableControlAction(2, 163, true); // INPUT_SELECT_WEAPON_SNIPER
    mp.game.controls.disableControlAction(2, 164, true); // INPUT_SELECT_WEAPON_HEAVY
    mp.game.controls.disableControlAction(2, 165, true); // INPUT_SELECT_WEAPON_SPECIAL

    mp.game.controls.disableControlAction(2, 261, true); // INPUT_PREV_WEAPON
    mp.game.controls.disableControlAction(2, 262, true); // INPUT_NEXT_WEAPON

    //      weapon switch controls       //
    if (blackList.indexOf(currentWeapon()) === -1) {
      mp.game.controls.disableControlAction(2, 140, true); // INPUT_MELEE_ATTACK_LIGHT
      mp.game.controls.disableControlAction(2, 141, true); // INPUT_MELEE_ATTACK_HEAVY
      mp.game.controls.disableControlAction(2, 142, true); // INPUT_MELEE_ATTACK_ALTERNATE
      mp.game.controls.disableControlAction(2, 143, true); // INPUT_MELEE_BLOCK
      mp.game.controls.disableControlAction(2, 263, true); // INPUT_MELEE_ATTACK1
      mp.game.controls.disableControlAction(2, 264, true); // INPUT_MELEE_ATTACK2
    }
  } catch (e) { logger.error(e); }
});

var checkTimer = setInterval(function () {
  var current = currentWeapon();
  if (localplayer.isInAnyVehicle(true)) {
    var vehicle = localplayer.vehicle;
    if (vehicle == null) return;

    if (vehicle.getClass() == 15) {
      if (vehicle.getPedInSeat(-1) == localplayer.handle || vehicle.getPedInSeat(0) == localplayer.handle) return;
    }
    else {
      if (canUseInCar.indexOf(current) == -1) return;
    }
  }

  if (mp.game.weapon.getWeapontypeModel(currentWeapon())== 0 && currentWeapon()!= -1569615261) {
    mp.game.invoke(getNative("GIVE_WEAPON_TO_PED"), localplayer.handle, givenWeapon, 1, false, true);
    mp.game.invoke(getNative("SET_PED_AMMO"), localplayer.handle, givenWeapon, 0);
    localplayer.taskReloadWeapon(false);
    localplayer.taskSwapWeapon(false);
  }
}, 100);

mp.events.addDataHandler('currentAmmo', function (entity, value, oldValue) {
  // mp.console.logInfo(`[currentAmmo] entity:${entity.id} local: ${mp.players.local.id} value: ${value} ==: ${entity === mp.players.local}`);

  if (entity !== mp.players.local) return;
  mp.gui.execute(`HUD.weapon.current=${value};`);
  currentAmmo = value;
});

mp.events.add('CLIENT::HUD:weaponShow', function (toggle, id) {
  mp.gui.execute(`HUD.weaponPanel=${toggle};`);

  if (id !== null) {
    mp.gui.execute(`HUD.weapon.id=${id};`);
  }
})

mp.events.add('CLIENT::HUD:weaponUpdate', function (current, max) {
  mp.gui.execute(`HUD.weapon.current=${current};`);
  mp.gui.execute(`HUD.weapon.max=${max};`);

  currentAmmo = current;
})

mp.events.add("playerWeaponShot", async (targetPosition, targetEntity) => {
  let ammo = mp.players.local.getAmmoInClip(mp.players.local.weapon);

  if (ammo === 0) {
    let weapon = mp.players.local.weapon;
    mp.game.invoke(SET_CURRENT_PED_WEAPON, mp.players.local.handle, mp.game.joaat('weapon_unarmed') >> 0, true);
    await mp.game.waitAsync(0);
    mp.game.invoke(SET_CURRENT_PED_WEAPON, mp.players.local.handle, weapon >> 0, true);
  }

  mp.gui.execute(`HUD.weapon.current=${ammo};`);
});

mp.events.add("removeAllWeapons", function () {
  givenWeapon = -1569615261;
});

mp.events.add('dmgmodif', (multi) => {
  mp.game.ped.setAiWeaponDamageModifier(multi);
});

mp.events.add("setResistStage", function (stage) {
  mp.game.player.setMeleeWeaponDefenseModifier(0.25 + resistStages[stage]);
  mp.game.player.setWeaponDefenseModifier(1.3 + resistStages[stage]);
});


mp.events.add('wgive', (weaponHash, ammo, isReload, equipNow) => {
  weaponHash = parseInt(weaponHash);
  ammo = parseInt(ammo);
  ammo = ammo >= 9999 ? 9999 : ammo;
  givenWeapon = weaponHash;
  ammo += mp.game.invoke(getNative("GET_AMMO_IN_PED_WEAPON"), localplayer.handle, weaponHash);
  mp.game.invoke(getNative("SET_PED_AMMO"), localplayer.handle, weaponHash, 0);
  ammosweap = ammo;
  mp.gui.execute(`HUD.weapon.current=${ammo};`);
  // GIVE_WEAPON_TO_PED //
  mp.game.invoke(getNative("GIVE_WEAPON_TO_PED"), localplayer.handle, weaponHash, ammo, false, equipNow);

  if (isReload) {
    mp.game.invoke(getNative("MAKE_PED_RELOAD"), localplayer.handle);
  }

  // mp.console.logWarning("wgive", true, true);
});

mp.events.add('takeOffWeapon', (weaponHash) => {
  try {
    weaponHash = parseInt(weaponHash);
    var ammo = mp.game.invoke(getNative("GET_AMMO_IN_PED_WEAPON"), localplayer.handle, weaponHash);
    if(ammo == ammosweap) mp.events.callRemote('playerTakeoffWeapon', weaponHash, ammo, 0);
    else mp.events.callRemote('playerTakeoffWeapon', weaponHash, ammosweap, 1);
    ammosweap = 0;
    mp.game.invoke(getNative("SET_PED_AMMO"), localplayer.handle, weaponHash, 0);
    mp.game.invoke(getNative("REMOVE_WEAPON_FROM_PED"), localplayer.handle, weaponHash);
    givenWeapon = -1569615261;
    mp.gui.execute(`HUD.weapon.current=0;`);

    // mp.console.logWarning("takeOffWeapon", true, true);
  } catch (e) { logger.error(e); }
});

mp.events.add('serverTakeOffWeapon', (weaponHash) => {
  try {
    weaponHash = parseInt(weaponHash);
    var ammo = mp.game.invoke(getNative("GET_AMMO_IN_PED_WEAPON"), localplayer.handle, weaponHash);
    if(ammo == ammosweap) mp.events.callRemote('takeoffWeapon', weaponHash, ammo, 0);
    else mp.events.callRemote('takeoffWeapon', weaponHash, ammosweap, 1);
    ammosweap = 0;
    mp.game.invoke(getNative("SET_PED_AMMO"), localplayer.handle, weaponHash, 0);
    mp.game.invoke(getNative("REMOVE_WEAPON_FROM_PED"), localplayer.handle, weaponHash);
    givenWeapon = -1569615261;
    mp.gui.execute(`HUD.weapon.current=0;`);

    // mp.console.logWarning("serverTakeOffWeapon", true, true);

  } catch (e) { logger.error(e); }
});




















function addComponentToPlayer(player, weaponHash, componentHash) {
  if (!player.hasOwnProperty("__weaponComponentData")) player.__weaponComponentData = {};
  if (!player.__weaponComponentData.hasOwnProperty(weaponHash)) player.__weaponComponentData[weaponHash] = new Set();

  player.__weaponComponentData[weaponHash].add(componentHash);
  mp.game.invoke(Natives.GIVE_WEAPON_COMPONENT_TO_PED, player.handle, weaponHash >> 0, componentHash >> 0);
}

function removeComponentFromPlayer(player, weaponHash, componentHash) {
  if (!player.hasOwnProperty("__weaponComponentData")) player.__weaponComponentData = {};
  if (!player.__weaponComponentData.hasOwnProperty(weaponHash)) player.__weaponComponentData[weaponHash] = new Set();

  player.__weaponComponentData[weaponHash].delete(componentHash);
  mp.game.invoke(Natives.REMOVE_WEAPON_COMPONENT_FROM_PED, player.handle, weaponHash >> 0, componentHash >> 0);
}

mp.events.add("updatePlayerWeaponComponent", (player_Id, weaponHash, componentHash,remove) =>
{
  //mp.console.logInfo("weaponHash "+weaponHash, true, true);
  setTimeout(function () {
    let player = mp.players.toArray().find(p => p.remoteId === player_Id);
    if(global.localplayer.remoteId===player.remoteId)
    {
      //mp.console.logInfo("updatePlayerWeapon 1 ", true, true);
      return;
    }

    if (remove) {
      removeComponentFromPlayer(player, weaponHash, componentHash);
    } else {
      //mp.console.logInfo("updatePlayerWeapon 3 ", true, true);
      addComponentToPlayer(player, weaponHash, componentHash);
    }
    setTimeout(function () {
      // mp.console.logError("updatePlayerWeaponComponent: " + weaponHash, true,true);
      mp.game.invoke(Natives.SET_CURRENT_PED_WEAPON, player.handle, weaponHash >> 0, true);
    }, 400);

    /* for (let component of componentsHash) {
     if(component == null) continue;
     mp.game.invoke("0xD966D51AA5B28BB9", player.handle, weaponHash >> 0, component >> 0);
     }
     mp.game.invoke("0xADF692B254977C0C", player.handle, weaponHash >> 0, true);*/
  }, 200);

});

mp.events.addDataHandler("currentWeaponComponents", (entity, value) => {

  if (entity.type === "player" && entity.handle !== 0) {

    if (!entity.hasOwnProperty("__weaponComponentData"))
    {

      entity.__weaponComponentData = {};
    }

    let [weaponHash, components] = value.split(".");
    // weaponHash = parseInt(weaponHash, 36);
    if (!entity.__weaponComponentData.hasOwnProperty(weaponHash))
    {

      entity.__weaponComponentData[weaponHash] = new Set();
    }
    let currentComponents = entity.__weaponComponentData[weaponHash];
    let newComponents = (components && components.length > 0) ? components.split('|')/*.map(hash => parseInt(hash, 36))*/ : [];
    setTimeout(function () {
      for (let component of currentComponents) {
        if(component==null)
        {
          continue;
        }
        if (!newComponents.includes(component))
        {
          removeComponentFromPlayer(entity, weaponHash, component);
        }
      }

      for (let component of newComponents)
      {
        if(component==null)
        {
          continue;
        }
        addComponentToPlayer(entity, weaponHash, component);
      }
      mp.game.invoke(Natives.SET_CURRENT_PED_WEAPON, entity.handle, weaponHash >> 0, true);
      // mp.console.logError("currentWeaponComponents: " + weaponHash, true,true);

      entity.__weaponComponentData[weaponHash] = new Set(newComponents);
    }, 200);
  }
  /*   setInterval(function () {
   for (let component of value)
   {
   if(component == null) continue;
   mp.game.invoke("0xD966D51AA5B28BB9", entity.handle, currentWeapon() >> 0, component >> 0);
   }
   mp.game.invoke("0xADF692B254977C0C", entity.handle, currentWeapon() >> 0, true);
   }, 200);*/
});

mp.events.add("entityStreamOut", (entity) => {
  if (entity.type === "player" && entity.hasOwnProperty("__weaponComponentData")) entity.__weaponComponentData = {};
});

mp.events.add("entityStreamIn", (entity) => {
  if (entity.type === "player") {
    let data = entity.getVariable("currentWeaponComponents");
    if (data) {
      setTimeout(function () {
        let [weaponHash, components] = data.split(".");
        // weaponHash = parseInt(weaponHash, 36);
        let componentsArray = (components && components.length > 0) ? components.split('|')/*.map(hash => parseInt(hash, 36)) */: [];

        // entity.giveWeapon(weaponHash, -1, true);
        for (let component of componentsArray)
        {
          if(component==null) continue;
          //mp.console.logInfo("stream in add weap comp "+entity.remoteId, true, true);
          addComponentToPlayer(entity, weaponHash, component);
          mp.game.invoke(Natives.SET_CURRENT_PED_WEAPON, entity.handle, weaponHash >> 0, true);
          // mp.console.logError("entityStreamIn weaponHash: " + weaponHash, true,true);
        }
      }, 2500);
      mp.events.callRemote('SetComponentFix');
    }
  }
});

mp.events.add('components', (whash,chash) => {
  mp.game.invoke("0xD966D51AA5B28BB9", global.localplayer.handle, whash >> 0, chash >> 0);
  mp.game.invoke("0xADF692B254977C0C", global.localplayer.handle, whash >> 0, true);

});
