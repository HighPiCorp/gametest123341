require("./initialization");

global.debounceEvent = (ms, triggerCouns, fn) => {
  let g_swapDate = Date.now();
  let g_triggersCount = 0;

  return (...args) => {
    if (++g_triggersCount > triggerCouns) {
      let currentDate = Date.now();

      if ((currentDate - g_swapDate) > ms) {
        g_swapDate = currentDate;
        g_triggersCount = 0;
      } else {
        return true; // cancel event trigger
      }
    }

    fn(...args);
  };
};

/* Events error handling */
const eventsMap = new Map();
const eventsAdd = Symbol('eventsAdd');
const rendersTicks = new Map();
let renderId = -1;
let isRenderDebugActive = false;
global.isGodModeActive = false;

mp.events[eventsAdd] = mp.events.add;
const __eventAdd__ = (eventName, eventFunction, name) => {
    if (
        eventName === 'render' &&
        (
            typeof name !== 'string' ||
            !name.length
        )
    ) {
        renderId++;
        name = renderId;
    }

    const proxyEventFunction = new Proxy(eventFunction, {
        apply: (target, thisArg, argumentsList) => {
            try {
                const start = Date.now();

                target.apply(thisArg, argumentsList);

                if (eventName === 'render') {
                    rendersTicks.set(name, Date.now() - start);
                }
            } catch(e) {
                mp.game.graphics.notify(`${eventName}:error:1`);
	              mp.game.graphics.notify(`error: ` + e.message);
	              mp.console.logError(`${eventName}:error:stack:-->>>` + e.stack, true, true);
            }
        }
    });

    eventsMap.set(eventFunction, proxyEventFunction);

    mp.events[eventsAdd](eventName, proxyEventFunction);
};

mp.events.add = (eventNameOrObject, ...args) => {
    if (typeof eventNameOrObject === 'object') {
        mp.events[eventsAdd](eventNameOrObject);

        return;
    }

    __eventAdd__(eventNameOrObject, ...args);
};

mp.events.add('render', () => {
    if (!isRenderDebugActive) {
        return;
    }

    const rendersTicksValues = [...rendersTicks.entries()];

    for (let i = 0; i < rendersTicksValues.length; i++) {
        mp.game.graphics.drawText(`${rendersTicksValues[i][0]} - ${rendersTicksValues[i][1]}ms`,
            [0.5, 0.1 + (i * 0.03)],
            {
                scale: [0.3, 0.3],
                outline: true,
                color: [255, 255, 255, 255],
                font: 4
            }
        );
    }

}, 'index-render');

mp.events.add('debug.render', () => {
    isRenderDebugActive = !isRenderDebugActive;
});

mp.events.add('admin.toggleGodMode', () => {
  global.isGodModeActive = !global.isGodModeActive;

  mp.players.local.setInvincible(global.isGodModeActive);

  mp.events.call('notify', 4, 9, `GM - ${global.isGodModeActive ? 'включен' : 'выключен'}`, 3000);
});

global.chatActive = false;
global.loggedin = false;
global.localplayer = mp.players.local;
const player = mp.players.local;
global.BLOCK_CONTROLS_DURING_ANIMATION = false;

// mp.gui.execute("window.location = 'package://cef/hud.html'");
mp.gui.execute("window.location = 'package://cef/hud/hud.html'");
if (mp.storage.data.chatcfg == undefined) {
  mp.storage.data.chatcfg = {
    timestamp: 0,
    chatsize: 0,
    fontstep: 0,
    alpha: 1,
  };
  mp.storage.flush();
}
setTimeout(() => {
  // mp.gui.execute(`newcfg(0,${mp.storage.data.chatcfg.timestamp}); newcfg(1,${mp.storage.data.chatcfg.chatsize}); newcfg(2,${mp.storage.data.chatcfg.fontstep}); newcfg(3,${mp.storage.data.chatcfg.alpha});`);
  global.oldchat.execute(`newcfg(0,${mp.storage.data.chatcfg.timestamp}); newcfg(1,${mp.storage.data.chatcfg.chatsize}); newcfg(2,${mp.storage.data.chatcfg.fontstep}); newcfg(3,${mp.storage.data.chatcfg.alpha});`);
  mp.events.call('showHUD', false);
}, 1000);

// setInterval(function () {
//     var name = (localplayer.getVariable('REMOTE_ID') == undefined) ? `Не авторизован` : `Игрок №${localplayer.getVariable("REMOTE_ID")}`;
// 	mp.discord.update('SaintsWorld', name);
// }, 10000);

global.pedsaying = null;
global.pedtext = '';
global.pedtext2 = null;
global.pedtimer = false;

global.pressedraw = false;
var accessRoding = false;

global.walkstyles = [
	null,
	"move_m@brave",
	"move_m@confident",
	"move_m@shadyped@a",
	"move_m@sad@a",
	"move_f@sexy@a",
	"move_ped_crouched",
	"move_m@quick"
];
global.moods = [null,"mood_aiming_1", "mood_angry_1", "mood_drunk_1", "mood_happy_1", "mood_injured_1", "mood_stressed_1"];
mp.game.streaming.requestClipSet("move_m@brave");
mp.game.streaming.requestClipSet("move_m@confident");
mp.game.streaming.requestClipSet("move_m@drunk@verydrunk");
mp.game.streaming.requestClipSet("move_m@shadyped@a");
mp.game.streaming.requestClipSet("move_m@sad@a");
mp.game.streaming.requestClipSet("move_m@quick");
mp.game.streaming.requestClipSet("move_f@sexy@a");
mp.game.streaming.requestClipSet("move_ped_crouched");
global.admingm = false;

mp.game.audio.setAudioFlag("DisableFlightMusic", true);
global.NativeUI = require("./nativeui.js");
global.Menu = NativeUI.Menu;
global.UIMenuItem = NativeUI.UIMenuItem;
global.UIMenuListItem = NativeUI.UIMenuListItem;
global.UIMenuCheckboxItem = NativeUI.UIMenuCheckboxItem;
global.UIMenuSliderItem = NativeUI.UIMenuSliderItem;
global.BadgeStyle = NativeUI.BadgeStyle;
global.Point = NativeUI.Point;
global.ItemsCollection = NativeUI.ItemsCollection;
global.Color = NativeUI.Color;
global.ListItem = NativeUI.ListItem;

function SetWalkStyle(entity, walkstyle) {
	try {
		if (walkstyle == null) entity.resetMovementClipset(0.0);
		else entity.setMovementClipset(walkstyle, 0.0);
	} catch (e) { }
}

function SetMood(entity, mood) {
	try {
		if (mood == null) entity.clearFacialIdleAnimOverride();
		else mp.game.invoke('0xFFC24B988B938B38', entity.handle, mood, 0);
	} catch (e) { }
}

mp.events.add('chatconfig', function (a, b) {
	if(a == 0) mp.storage.data.chatcfg.timestamp = b;
    else if(a == 1) mp.storage.data.chatcfg.chatsize = b;
	else if(a == 2) mp.storage.data.chatcfg.fontstep = b;
	else mp.storage.data.chatcfg.alpha = b;
	mp.storage.flush();
});

mp.events.add('setClientRotation', function (player, rots) {
	if (player !== undefined && player != null && localplayer != player) player.setRotation(0, 0, rots, 2, true);
});

mp.events.add('setWorldLights', function (toggle) {
	try {
		mp.game.graphics.resetLightsState();
		for (let i = 0; i <= 16; i++) {
			if(i != 6 && i != 7) mp.game.graphics.setLightsState(i, toggle);
		}
	} catch { }
});

mp.events.add('changeChatState', function (state) {
    global.chatActive = state;
});

mp.events.add('PressE', function (toggle) {
  global.pressedraw = toggle;
});

mp.events.add('allowRoding', function (toggle) {
    accessRoding = toggle;
});

mp.events.add('UpdateMoney', function (temp, amount) {
  mp.events.call('UpdateMoneyHud', temp, amount);
  mp.events.call('UpdateMoneyPhone', temp, amount);
});

mp.events.add('UpdateBank', function (temp, amount) {
  mp.events.call('UpdateBankHud', temp, amount);
  mp.events.call('UpdateBankPhone', temp, amount);
});

// Casino M4ybe
var alerMessageBrowser = mp.browsers.new("package://cef/show-alert.html");

global.showAlert = function(style, message) {
	if(alerMessageBrowser != null) alerMessageBrowser.execute('alertMsg("'+style+'", "'+message+'")');
}

global.loadAnim = function(dict)
{
	new Promise((resolve, reject) => {
		const timer = setInterval(() => {
			if (mp.game.streaming.hasAnimDictLoaded(dict))
			{
				//mp.gui.chat.push(`Anim ${dict} has been loaded!`);
				clearInterval(timer);
				resolve();
			}
			else
			{
				//mp.gui.chat.push(`Anim ${dict} is not loaded`);
				mp.game.streaming.requestAnimDict(dict);
			}

		}, 300);
	});
};

require('./logger');
// require("./cef/resources/js/modules/unitls.js");
// require('./cef/resources/js/modules/menus/global');
// // // // // // //
//require('./ClothesMenu'); // Если надо протестить одежду и нажимать Numpad9
require('./client/utils/keys');
require('./menus');
require('./client/player/afksystem');
require('./render');
require('./main');
require('./voice');
require('./hud');
require('./board');
require('./keybind');


global.loadBinds();

// require('./phone');
require('./checkpoints');

require('./casino_roulette_c');
// require('./inventory');
require('./familyhouses');
require('./familycreator');
require('./familymanager');
require('./resalecars');
require('./exclusive');
// require("./invent.js");
require('./weapondisplay/index');
require('./static-attachments/index');
require('./scripts/weapons_damage');
require('./gamertag');
require('./furniture');
require('./admesp');
require('./circle');
require('./vehiclesync');
require('./spmenu');
require('./basicsync');
require('./gangzones');
require('./fly');

require('./environment');
require('./elections');
require('./animals');
require('./client/utils/utils');
require('./scripts/autopilot');
// require('./scripts/crouch');
// require('./scripts/location');
require('./scripts/markers');
require('./scripts/fingerPointer');
// require('./scripts/Hunting'); НЕ РАБОТАЕТ

require('./admin/adminpanel');

require('./tablet/tablet');

require('./configs/natives');
require('./configs/tuning');

require('./radiosync');

require('./cayo_perico/heistisland');

require('./containers/containers');

require('./bigmap');

// Работа - Мусорщик (by BlackGold)
// require('./public/work');
require('./public/colshapes');
require('./public/peds');
//

// require('./farmerWork/farmer');
// require('./farmerWork/market');

require('./betternotifs/index');

// require('./casino/luckywheel');

// require('./casino');
require('./cef/resources/js/helpers/keybindsdefault');
require('./cef/resources/js/helpers/keybindsEvents');
require('./blackjack');

require('./public/slots.js');
require('./public/insidetrack.js');
require('./public/roulette.js');
//require('ClothesMenu');


require('./world/doors');
require('./drone');

require('./cef/resources/js/utils/index');
require('./cef/resources/js/timer/index');

// CITYRP - fataldose
require('./npc');
require('./docs');
require('./main_menu');
require('./numberplates');

//
require('./public/banks');
require('./public/garbagecollector');
require('./public/parkings');
require('./public/truckers');
require('./public/carroom');
require('./public/taxi');
require('./public/rent');
require('./public/mechanic');
require('./public/phone');
require('./public/casino');

require('./public/realtor');

require('./public/builder/menu');
require('./public/builder/builder_lic');
require('./public/builder/builderMiniGame.js');
require('./public/roding');

require('./public/dialog');

require('./public/admin/banSystem');

require('./public/craft/craft');
require('./public/craft/detector');
require('./public/craft/shovelminigame');
require('./public/craft/workbenchcraft');
require('./public/doors');
require('./public/timer');
require('./public/arena/arena');
require('./public/arena/arenahud');

// Business
//   Tattoo
require('./public/business/Tattoo/data/tattoo'); // data
require('./public/business/Tattoo/tattoo');

//   Barber
require('./public/business/Barber/data/barber'); // data
require('./public/business/Barber/barber');

//   Clothes Shop
require('./public/business/Clothes/data/clothes'); // data
require('./public/business/Clothes/data/donateClothes'); // data
require('./public/business/Clothes/clothes');

//   Family
require('./public/family/create');
require('./public/family/officies');
require('./public/family/contracts');
require('./public/family/manager');
require('./public/family/houses');
//   Tuning Shop
require('./public/tuning');

//   Ammunation Shop
// require('./public/business/Clothes/data/clothes'); // data
require('./public/business/Ammunation/ammunation');

//   Shop
require('./public/business/Shop/shop');

//   Mask
require('./public/business/Masks/masks');

//   Bags
require('./public/business/Bags/bags');

//   Fish shop
require('./public/business/Fish/fish');

//   Business Mange
require('./public/business/menu');

// Quests
//   Race
require('./public/quests/race');
require('./public/quests/timer');

// Other
//   Mmenu
require('./public/modules/Mmenu/mmenu');

//   Color Picker
require('./public/modules/ColorPicker/colorpicker');

// Admin
//   Create Promocodes
require('./public/admin/Promocodes/promocodes');

require('./public/stats');

require('./public/character/create/create');
require('./public/character/select/select');
require('./public/character/weapon/weapon');

require('./public/utils/popup');

require('./public/animmenu.js');

require('./fractions/fraction');

// Houses
//   Robbery
require('./public/house/robbery/robbery');

// Vehicles
//   Create
require('./public/vehicle/create');

// Zones
require('./public/zones/zoneManager');
require('./public/zones/zones');


// Donate
//    Roulette
require('./public/donate/roulette/roulette');

require('./public/quests/cutscene');

// BattlePass
require('./public/battlepass/battlepass');
require('./public/battlepass/roulette.js');



// require('./cef/BankFleeca/index');

// // // // // // //

if (mp.storage.data.friends == undefined) {
    mp.storage.data.friends = [];
    mp.storage.flush();
}

// // // // // // //
const mSP = 30;
var prevP = mp.players.local.position;
var localWeapons = {};

function distAnalyze() {
	if(new Date().getTime() - global.lastCheck < 100) return;
	global.lastCheck = new Date().getTime();
    let temp = mp.players.local.position;
    let dist = mp.game.gameplay.getDistanceBetweenCoords(prevP.x, prevP.y, prevP.z, temp.x, temp.y, temp.z, true);
    prevP = mp.players.local.position;
    if (mp.players.local.isInAnyVehicle(true)) return;
    if (dist > mSP) {
        mp.events.callRemote("acd", "fly");
    }
}

global.serverid = 1;

mp.events.add('ServerNum', (server) => {
   global.serverid = server;
});

global.acheat = {
    pos: () => prevP = mp.players.local.position,
    guns: () => localWeapons = mp.players.local.getAllWeapons(),
    start: () => {
        setInterval(distAnalyze, 2000);
    }
}

mp.events.add('authready', () => {
  // eslint-disable-next-line global-require
  require('./auth');
  mp.events.call('showAuth');
});

mp.events.add('acpos', () => {
    global.acheat.pos();
})
// // // // // // //
var spectating = false;
var sptarget = null;

//mp.game.invoke(getNative("REMOVE_ALL_PED_WEAPONS"), localplayer.handle, false);
/*mp.keys.bind(getKeyBy('RELOAD'), false, function () {
	// R key
	try {
		if (!loggedin || chatActive || new Date().getTime() - global.lastCheck < 1000 || mp.gui.cursor.visible) return;
		let current = currentWeapon();
		// mp.console.logInfo("current weapon: -> "+current);

		if (current == -1569615261 || current == 911657153) return;
		var ammo = mp.game.invoke(getNative("GET_AMMO_IN_PED_WEAPON"), localplayer.handle, current);
		if (mp.game.weapon.getWeaponClipSize(current) == ammo) return;
		global.anyEvents.SendServer(() => mp.events.callRemote("playerReload", current));
		global.lastCheck = new Date().getTime();
	} catch {}
});*/

mp.keys.bind(Keys.VK_1, false, function () {
	// 1 key
	if (
		!global.loggedin ||
		chatActive ||
		new Date().getTime() - global.lastCheck < 1000 ||
		global.menuOpened ||
		mp.gui.cursor.visible
	)
		return;
	//global.anyEvents.SendServer(() => mp.events.callRemote("changeweap", 1));
	mp.events.callRemote("fastSlot", 1);
	global.lastCheck = new Date().getTime();
});

mp.keys.bind(Keys.VK_2, false, function () {
	// 2 key
	if (
		!global.loggedin ||
		chatActive ||
		new Date().getTime() - global.lastCheck < 1000 ||
		global.menuOpened ||
		mp.gui.cursor.visible
	)
		return;
	mp.events.callRemote("fastSlot", 2);
	global.lastCheck = new Date().getTime();
});

mp.keys.bind(Keys.VK_3, false, function () {
	// 3 key
	if (
		!global.loggedin ||
		chatActive ||
		new Date().getTime() - global.lastCheck < 1000 ||
		global.menuOpened ||
		mp.gui.cursor.visible
	)
		return;
	mp.events.callRemote("fastSlot", 3);
	global.lastCheck = new Date().getTime();
});

mp.keys.bind(Keys.VK_4, false, function () {
	// 4 key
	if (
		!global.loggedin ||
		chatActive ||
		new Date().getTime() - global.lastCheck < 1000 ||
		global.menuOpened ||
		mp.gui.cursor.visible
	)
		return;
	mp.events.callRemote("fastSlot", 4);
	global.lastCheck = new Date().getTime();
});

mp.events.add("toBlur", (time = 1000) => {
	mp.game.graphics.transitionToBlurred(time);
});

mp.events.add("fromBlur", (time = 1000) => {
	mp.game.graphics.transitionFromBlurred(time);
});

var petathouse = null;
mp.events.add('petinhouse', (petName, petX, petY, petZ, petC, Dimension) => {
	if(petathouse != null) {
		petathouse.destroy();
		petathouse = null;
	}
	switch(petName) {
		case "Husky":
			petName = 1318032802;
			break;
		case "Poodle":
			petName = 1125994524;
			break;
		case "Pug":
			petName = 1832265812;
			break;
		case "Retriever":
			petName = 882848737;
			break;
		case "Rottweiler":
			petName = 2506301981;
			break;
		case "Shepherd":
			petName = 1126154828;
			break;
		case "Westy":
			petName = 2910340283;
			break;
		case "Cat":
			petName = 1462895032;
			break;
		case "Rabbit":
			petName = 3753204865;
			break;
	}
	petathouse = mp.peds.new(petName, new mp.Vector3(petX, petY, petZ), petC, Dimension);
});



mp.game.vehicle.defaultEngineBehaviour = false;
mp.players.local.setConfigFlag(429, true);
mp.players.local.setConfigFlag(241, true);

mp.events.add('render', () => {
    try {
	      if(BLOCK_CONTROLS_DURING_ANIMATION == true)
        {
            //mp.game.invoke('0x5E6CC07646BBEAB8', mp.players.local.handle, true); // DISABLE_PLAYER_FIRING
            mp.game.controls.disableControlAction(0, 257, true); // стрельба
            mp.game.controls.disableControlAction(0, 22, true);
            mp.game.controls.disableControlAction(2, 25, true);
            mp.game.controls.disableControlAction(2, 24, true);
            mp.game.controls.disableControlAction(0, 23, true); // INPUT_ENTER
        }
    } catch (e) { }
});

mp.events.add("Player_SetMood", (player, index) => {
    try {
        if (player !== undefined) {
            if (index == 0) player.clearFacialIdleAnimOverride();
			else mp.game.invoke('0xFFC24B988B938B38', player.handle, moods[index], 0);
        }
    } catch (e) {
      logger.error(e);
      // mp.console.logInfo("SetMood Debug: " + e.toString());
		}
});

mp.events.add("Player_SetWalkStyle", (player, index) => {
    try {
        if (player !== undefined) {
            if (index == 0) player.resetMovementClipset(0.0);
			else player.setMovementClipset(walkstyles[index], 0.0);
        }
    } catch (e) {
      logger.error(e);
      // mp.console.logInfo("SetWalkStyle Debug: " + e.toString());
	}
});

mp.events.add('svem', (pm, tm) => {
	var vehc = localplayer.vehicle;
	vehc.setEnginePowerMultiplier(pm);
	vehc.setEngineTorqueMultiplier(tm);
});
let driftmode = false;
mp.events.add('driftmode', () => {
	var vehc = localplayer.vehicle;
	if(driftmode)
	{
		vehc.setHandling("fInitialDriveForce", 0.1);
		driftmode = false;
	}
	else
	{
		vehc.setHandling("fInitialDriveForce", 1);
		driftmode = true;
	}
});
mp.game.gxt.set("PM_PAUSE_HDR", "SAINTSWORLD.RU");

/* Недостающие части IPL карты */

// mp.game.streaming.requestIpl("ch1_02_open"); // Глитч открытого интерьера на пляже
mp.game.streaming.requestIpl("sp1_10_real_interior"); // открытый интерьер стадика
mp.game.streaming.requestIpl("sp1_10_real_interior_lod"); // открытый интерьер стадика
mp.game.streaming.requestIpl("ferris_finale_Anim"); // колесо обозрения на пляже
mp.game.streaming.requestIpl("gr_case6_bunkerclosed"); // закрытый бункер merryweather
// mp.game.streaming.requestIpl("Coroner_Int_On"); // части интерьера больницы
mp.game.streaming.requestIpl("TrevorsTrailerTidy"); // Трейлеп Тревора

/* */

/* casino */

// Подгружаем недостающие части здания самого казино...
mp.game.streaming.requestIpl("hei_dlc_windows_casino");
mp.game.streaming.requestIpl("hei_dlc_casino_aircon");
mp.game.streaming.requestIpl("vw_dlc_casino_door");
mp.game.streaming.requestIpl("hei_dlc_casino_door");
mp.game.streaming.requestIpl("vw_casino_penthouse");
mp.game.streaming.requestIpl("vw_casino_garage");
mp.game.streaming.requestIpl("vw_casino_carpark");

mp.game.streaming.requestIpl("apa_v_mp_h_01_a");
mp.game.streaming.requestIpl("apa_v_mp_h_01_c");
mp.game.streaming.requestIpl("apa_v_mp_h_01_b");
mp.game.streaming.requestIpl("apa_v_mp_h_02_a");
mp.game.streaming.requestIpl("apa_v_mp_h_02_c");
mp.game.streaming.requestIpl("apa_v_mp_h_02_b");
mp.game.streaming.requestIpl("apa_v_mp_h_03_a");
mp.game.streaming.requestIpl("apa_v_mp_h_03_c");
mp.game.streaming.requestIpl("apa_v_mp_h_03_b");
mp.game.streaming.requestIpl("apa_v_mp_h_04_a");
mp.game.streaming.requestIpl("apa_v_mp_h_04_c");
mp.game.streaming.requestIpl("apa_v_mp_h_04_b");
mp.game.streaming.requestIpl("apa_v_mp_h_05_a");
mp.game.streaming.requestIpl("apa_v_mp_h_05_c");
mp.game.streaming.requestIpl("apa_v_mp_h_05_b");
mp.game.streaming.requestIpl("apa_v_mp_h_06_a");
mp.game.streaming.requestIpl("apa_v_mp_h_06_c");
mp.game.streaming.requestIpl("apa_v_mp_h_06_b");
mp.game.streaming.requestIpl("apa_v_mp_h_07_a");
mp.game.streaming.requestIpl("apa_v_mp_h_07_c");
mp.game.streaming.requestIpl("apa_v_mp_h_07_b");
mp.game.streaming.requestIpl("apa_v_mp_h_08_a");
mp.game.streaming.requestIpl("apa_v_mp_h_08_c");
mp.game.streaming.requestIpl("apa_v_mp_h_08_b");

/*
mp.game.streaming.requestIpl("imp_dt1_02_cargarage_a");
mp.game.streaming.requestIpl("imp_dt1_02_cargarage_b");
mp.game.streaming.requestIpl("imp_dt1_02_cargarage_c");
mp.game.streaming.requestIpl("imp_dt1_02_modgarage");

mp.game.streaming.requestIpl("imp_dt1_11_cargarage_a");
mp.game.streaming.requestIpl("imp_dt1_11_cargarage_b");
mp.game.streaming.requestIpl("imp_dt1_11_cargarage_c");
mp.game.streaming.requestIpl("imp_dt1_11_modgarage");

mp.game.streaming.requestIpl("imp_sm_13_cargarage_a");
mp.game.streaming.requestIpl("imp_sm_13_cargarage_b");
mp.game.streaming.requestIpl("imp_sm_13_cargarage_c");
mp.game.streaming.requestIpl("imp_sm_13_modgarage");

mp.game.streaming.requestIpl("imp_sm_15_cargarage_a");
mp.game.streaming.requestIpl("imp_sm_15_cargarage_b");
mp.game.streaming.requestIpl("imp_sm_15_cargarage_c");
mp.game.streaming.requestIpl("imp_sm_15_modgarage");
*/


let phIntID = mp.game.interior.getInteriorAtCoords(976.636, 70.295, 115.164);
let phPropList = [
	"Set_Pent_Tint_Shell",
	"Set_Pent_Pattern_03",
	"Set_Pent_Spa_Bar_Open",
	"Set_Pent_Media_Bar_Open",
	// "Set_Pent_Dealer",
	// "Set_Pent_Arcade_Retro",
	"Set_Pent_Bar_Clutter",
	"Set_Pent_Clutter_01",
	"set_pent_bar_light_01",
	"set_pent_bar_party_0"
];

for (const propName of phPropList) {
	mp.game.interior.enableInteriorProp(phIntID, propName);
	mp.game.invoke("0xC1F1920BAF281317", phIntID, propName, 3); // с деднета
}
mp.game.interior.refreshInterior(phIntID);

//Arcadius?
// mp.game.streaming.requestIpl("imp_dt1_02_modgarage");
/* casino end */

mp.events.add('loadProp', (x, y, z, prop) => {
    var interior = mp.game.interior.getInteriorAtCoords(x, y, z);
    mp.game.interior.enableInteriorProp(interior, prop);
    mp.game.interior.refreshInterior(interior);
});

mp.events.add('UnloadProp', (x, y, z, prop) => {
    var interior = mp.game.interior.getInteriorAtCoords(x, y, z);
    mp.game.interior.disableInteriorProp(interior, prop);
    mp.game.interior.refreshInterior(interior);
});

//mp.game.invoke("0x5E1460624D194A38", true); // for island map in pause menu and minimap
//mp.game.invoke("0x9A9D1BA639675CF1", "HeistIsland", true); // enable HeistIsland

mp.events.add('BetterNotify', (type, header, header1, text, a, b, c, pic, icon) => {
    if(type) mp.game.ui.notifications.showWithPicture(header, header1, text, pic, icon, a, b, c);
    else mp.game.ui.notifications.show(text, a, b, c);
});


mp.events.add("cef:error", (errorMessage, url, line) => {
	mp.console.logError(`${errorMessage} at ${url} line ${line.toString()}`);
});


let isGpuRenderingEnabled = mp.gui.isGpuRenderingEnabled;
mp.console.logInfo("GPU Rendering Enabled: " + isGpuRenderingEnabled);

// let customChat = null;
// mp.events.add('guiReady', () => {
// 	if (!customChat) {
// 		customChat = mp.browsers.new('package://cef/chat/chat.html');
// 		customChat.markAsChat();
// 	}
// })

//
// mp.events.add('render', (nametags) => {
//   var player = mp.players.local;
//
//   let pos = player.position;
//   mp.game.graphics.drawText(`My Pos: X: ${pos.x.toFixed(2)} Y: ${pos.y.toFixed(2)} Z: ${pos.z.toFixed(2)} | R: ${player.getHeading().toFixed(2)}`, [0.5, 0.005], {
//     font: 4,
//     color: [255, 255, 255, 255],
//     scale: [1,1],
//     outline: true
//   });
// });


global.oldchat = mp.browsers.new('package://interfaces/ui_new/OldChat/chat.html');
global.oldchat.markAsChat();

mp.keys.bind(Keys.VK_T, false, () => {
  if(global.phoneOpen || global.statsOpen || global.animMenuOpen || global.taxiPriceOpen || global.menuCheck()) return;
  global.oldchat.execute('chatToggle()');
});

// mp.events.add("explosion", (sourcePlayer, type, pos) => { return true; });
// mp.events.add("projectile", (sourcePlayer, weaponHash, ammoType, position, direction) => { return true; });
