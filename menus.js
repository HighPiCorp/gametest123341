let menuWasOpened = false;
let hudWasOpened = true;

global.menuOpened = false;
global.phoneOpen = false;
global.menu = null;
global.houseMenuHTML = null;
// global.questMenuHTML = null;
global.raceMenuHTML = null;
global.gameMenuHTML = null;
global.workMenuHTML = null;
global.rentMenuHTML = null;
global.rouletteMenuHTML = null;
global.selectedCarManageNumber = null;
global.selectedFurnitureManageId = null;
global.selectedRoommateManageId = null;
// global.inventoryk = null;

global.globalTuningModel = null;

global.menuCheck = function () {
  if (global.menu !== null) {
    return global.menuOpened;
  }

  return true;
};

global.menuClose = function () {
  mp.gui.cursor.visible = false;
  global.menuOpened = false;
  mp.events.call('showHUD', true);
  global.INTERACTIONCHECK = false;
};
global.menuOpen = function () {
  mp.gui.cursor.visible = true;
  global.menuOpened = true;
  hudWasOpened = global.showhud;
  mp.events.call('showHUD', false);
  global.INTERACTIONCHECK = false;
};
global.menuOpenWithHud = function () {
  mp.gui.cursor.visible = true;
  global.menuOpened = true;
  global.INTERACTIONCHECK = false;
};

global.menuCloseIfNotOpened = function () {
  try {
    if (!menuWasOpened) global.menuClose();
  } catch (e) {
    logger.error(e);
  }
}

global.menuOpenIfNotOpened = function () {
  try {
    menuWasOpened = global.menuOpened;
    if (!menuWasOpened) global.menuOpen();
  } catch (e) {
    logger.error(e);
  }
}



mp.events.add('playerQuit', (player, exitType, reason) => {
  if (global.menu !== null) {
    if (player.name === localplayer.name) {
      global.menuClose();
      global.menu.destroy();
      global.menu = null;
    }
  }

  mp.players.local.setVisible(true, false);
});

let alcoUI = null;
const ClubNames = {
  10: 'Bahama Mamas West',
  11: 'Vanila Unicorn',
  12: 'Tequi-la-la',
  13: 'Diamond Penthouse',
};
const ClubAlcos = {
  10: ['«Martini Asti»', '«Sambuca»', '«Campari»'],
  11: ['«На корке лимона»', '«На бруснике»', '«Русский стандарт»'],
  12: ['«Asahi»', '«Midori»', '«Yamazaki»'],
  13: ['«Дживан»', '«Арарат»', '«Noyan Tapan»'],
};
const ClubDrinks = [75, 115, 150];
let selectedAlco = 0;
mp.events.add('openAlco', (club, modief, isOwner, stock) => {
  selectedAlco = 0;
  global.menuOpen();
  mp.gui.cursor.visible = false;
  const res = mp.game.graphics.getScreenActiveResolution(0, 0);
  const UIPositions = {
    RightMiddle: new Point(res.x - 180, res.y / 2),
    LeftMiddle: new Point(0, res.y / 2 - 200),
  };
  alcoUI = new Menu('Клуб', ClubNames[club], UIPositions.LeftMiddle);

  const drinks = [` ${ClubAlcos[club][0]} ${(ClubDrinks[0] * modief).toFixed()}$`,
    ` ${ClubAlcos[club][1]} ${(ClubDrinks[1] * modief).toFixed()}$`,
    ` ${ClubAlcos[club][2]} ${(ClubDrinks[2] * modief).toFixed()}$`];

  alcoUI.AddItem(new UIMenuListItem(
    'Напитки',
    'Вы можете выбрать любой напиток',
    new ItemsCollection(drinks),
  ));

  if (isOwner) {
    alcoUI.AddItem(new UIMenuItem('Инфо', `Материалы: ${stock[0]}\n${ClubAlcos[club][0]} - ${stock[1]}\n${ClubAlcos[club][1]} - ${stock[2]}\n${ClubAlcos[club][2]} - ${stock[3]}`));
    alcoUI.AddItem(new UIMenuItem('Взять', 'Взять выбранный напиток со склада'));
    alcoUI.AddItem(new UIMenuItem('Скрафтить', 'Скрафтить выбранный напиток'));
    alcoUI.AddItem(new UIMenuItem('Установить цену', 'Установить модификатор цены для всех продуктов (от 50% до 150%)'));
  }

  alcoUI.AddItem(new UIMenuItem('Купить', 'Купить выбранный напиток'));

  const uiItem = new UIMenuItem('Закрыть', 'Закрыть меню');
  uiItem.BackColor = new Color(255, 0, 0);
  alcoUI.AddItem(uiItem);

  alcoUI.ItemSelect.on((item) => {
    if (new Date().getTime() - global.lastCheck < 100) return;
    global.lastCheck = new Date().getTime();
    if (item.Text == 'Купить') {
      global.anyEvents.SendServer(() => mp.events.callRemote('menu_alco', 0, selectedAlco));
    } else if (item.Text == 'Взять') {
      global.anyEvents.SendServer(() => mp.events.callRemote('menu_alco', 1, selectedAlco));
    } else if (item.Text == 'Скрафтить') {
      global.anyEvents.SendServer(() => mp.events.callRemote('menu_alco', 2, selectedAlco));
    } else if (item.Text == 'Установить цену') {
      global.menuClose();
      alcoUI.Close();
      global.anyEvents.SendServer(() => mp.events.callRemote('menu_alco', 3, 0));
    } else if (item.Text == 'Закрыть') {
      global.menuClose();
      alcoUI.Close();
    }
  });

  alcoUI.ListChange.on((item, index) => {
    selectedAlco = index;
  });

  alcoUI.Open();
});

// // // // //
global.input = {
  head: '',
  desc: '',
  len: '',
  cBack: '',
  set(h, d, l, c) {
    this.head = h, this.desc = d;
    this.len = l, this.cBack = c;
    if (global.menuCheck()) return;
    global.menu.execute(`input.set("${this.head}","${this.desc}","${this.len}");`);
  },
  open() {
    if (global.menuCheck()) return;
    global.menu.execute('input.active=1');
    // Table
    global.menuOpen();
    mp.events.call('startScreenEffect', 'MenuMGHeistIn', 1, true);
  },
  close() {
    // Table
    global.menuClose();
    global.menu.execute('input.active=0');
    mp.events.call('stopScreenEffect', 'MenuMGHeistIn');
  },
};
mp.events.add('input', (text) => {
  if (input.cBack == '') return;
  if (input.cBack == 'setCruise') mp.events.call('setCruiseSpeed', text);
  else global.anyEvents.SendServer(() => mp.events.callRemote('inputCallback', input.cBack, text));
  input.cBack = '';
  input.close();
});
mp.events.add('openInput', (h, d, l, c) => {
  if (global.menuCheck()) return;
  input.set(h, d, l, c);
  input.open();
});
mp.events.add('closeInput', () => {
  input.close();
});

mp.events.add('playerQuit', (player) => {
  if (global.inventoryk !== null) {
    //mp.console.logError('playerQuit? inventory need destroy?', true, true);
    // closeInventory();
    // global.inventoryk.destroy();
    // global.inventoryk = null;
  }
});

global.inventoryk = mp.browsers.new('package://cef/inventory/index.html');
global.inventoryOpen = false;
global.cantOpenInventory = false;

mp.keys.bind(Keys.VK_I, false, () => {
  if (!global.loggedin || cantOpenInventory || global.chatActive || global.phoneOpen || global.editing || global.cuffed || global.localplayer.getVariable('InDeath') == true) return;

  if (global.inventoryOpen) mp.events.call('inventory', 1);
  else mp.events.call('inventory', 0);
});

mp.events.add('CLIENT::inventory:cantOpen', (toggle) => {
  global.cantOpenInventory = toggle;
});

function openInventory() {
  if (global.menuOpened) return;
  if (global.inventoryOpen == true) return;

  global.closeAnyMenuFunctions.closePhone();

  if (!global.menuOpened) {
    global.menuOpen();
  }

  // mp.events.call('toBlur', 200);
  mp.gui.cursor.show(true, true);
  global.inventoryOpen = true;
  global.inventoryk.call('inventory_InvShow');
  if (global.inventoryk === null) {
    // mp.console.logInfo('HTML инвентаря null OpenInventory');
  }
}

function closeInventory() {
  if (global.inventoryk == null) return;
  if (global.inventoryOpen == false) return;
  global.anyMenuHTML.removeCloseCallback();

  if (global.menuOpened) {
    global.menuClose();
  }

  global.inventoryOpen = false;
  // mp.events.call('fromBlur', 200);
  mp.gui.cursor.show(false, false);
  mp.events.callRemote('closeInventory');
  global.inventoryk.call('inventory_UpdateOutside', 'close');
  global.inventoryk.call('inventory_UpdateTrade', 'close');
  global.inventoryk.call('inventory_InvShow');

  if (global.inventoryk === null) {
    // mp.console.logInfo('HTML инвентаря null CloseInventory');
  }
}
mp.events.add('inventoryConcat_Client', (sourceSlotId, targetSlotId, targetGroupCode, sourceGroupCode, count) => {
  mp.events.callRemote('inventoryConcat_Server', sourceSlotId, targetSlotId, targetGroupCode, sourceGroupCode, count);
});
mp.events.add('inventoryFastSlot_Client', (targetGroupCode, sourceSlotId, targetSlotId) => {
  mp.events.callRemote('inventoryFastSlot_Server', targetGroupCode, sourceSlotId, targetSlotId);
});
mp.events.add('inventoryFastSlotOut_Client', (targetGroupCode, sourceSlotId, targetSlotId) => {
  mp.events.callRemote('inventoryFastSlotOut_Server', targetGroupCode, sourceSlotId, targetSlotId);
});
mp.events.add('inventoryTransfer_Client', (sourceGroupCode, targetGroupCode, sourceSlotId, targetSlotId) => {
  mp.events.callRemote('inventoryTransfer_Server', sourceGroupCode, targetGroupCode, sourceSlotId, targetSlotId);
});
mp.events.add('inventoryDrop_Client', (groupCode, slotId) => {
  mp.events.callRemote('inventoryDrop_Server', groupCode, slotId);
});
mp.events.add('inventoryUse_Client', (sourceGroupCode, sourceSlotId, targetSlotId) => {
  mp.events.callRemote('inventoryUse_Server', sourceGroupCode, sourceSlotId, targetSlotId);
});
mp.events.add('inventoryChangeSlot_Client', (groupCode, sourceSlotId, targetSlotId) => {
  mp.events.callRemote('inventoryChangeSlot_Server', groupCode, sourceSlotId, targetSlotId);
});
mp.events.add('inventorySeparate_Client', (id, countTarget, targetSlotIdValue, targetGroupCode, sourceGroupCode) => {
  mp.events.callRemote('inventorySeparate_Server', id, countTarget, targetSlotIdValue, targetGroupCode, sourceGroupCode);
});
mp.events.add('inventorytradeAccept_Client', () => {
  mp.events.callRemote('inventorytradeAccept_Server');
});
mp.events.add('inventorySendtrademoney_Client', (TradeMoney) => {
  mp.events.callRemote('inventorySendtrademoney_Server', TradeMoney);
});

mp.events.add('inventory', (act, data, maxWeight, name = null, rowcell = null) => {
  // //mp.console.logInfo(`Inventory: act: ${act} data: ${data} maxWeight: ${maxWeight} name: ${name} rowcell: ${rowcell}`);
  switch (act) {
    case 0:
      openInventory();
      break;
    case 1:
      closeInventory();
      break;
    case 2:
      if (global.inventoryk != null) global.inventoryk.call('inventory_UpdateItems', data);
      // mp.console.logWarning(`data: ${data}`, true, true);
      break;
    case 3:
      //mp.console.logInfo(`maxWeight: ${maxWeight} name: ${name} rowcell: ${rowcell}`);
      if (global.inventoryk != null) global.inventoryk.call('inventory_UpdateBugs', data, maxWeight, rowcell, name);
      break;
    case 4:
      if (global.inventoryk != null) global.inventoryk.call('inventory_UpdateOutside', data, maxWeight, name, rowcell);
      // //mp.console.logInfo(`maxWeight: ${maxWeight} name: ${name} rowcell: ${rowcell}`);
      break;
    case 5:
      if (global.inventoryk != null) global.inventoryk.call('inventory_UpdateTrade', data);
      break;
    case 6:
      if (global.inventoryk != null) global.inventoryk.call('inventory_UpdateTradeOut', data);
      break;
    case 7:
      if (global.inventoryk != null) global.inventoryk.call('inventory_AcceptTradeOut');
      break;
    case 8:
      if (global.inventoryk != null) global.inventoryk.call('inventory_SetTradeOutMoney', data.toString());
      break;
    case 9:
      // if(global.inventoryk!=null)
      //     // mp.gui.chat.push("dds "+data);
      global.inventoryk.call('inventory_SetPlayermoney', data.toString());
      break;

    case 10:
      // if(global.inventoryk!=null)
      global.inventoryk.call('inventory_SetPlayerANDTargetNAme', data.toString());
      break;
  }
});

global.houseMenuHTML = mp.browsers.new('package://cef/resources/views/menus/house.html');
global.houseMenuHTML.active = false;

// HOUSE MENU
global.houseMenu = {
  title: '',
  menuState: false,
  lastTime: false,
  open(menu, data) {
    global.closeAnyMenuFunctions.closePhone();

    global.houseMenuHTML.active = true;
    global.houseMenuHTML.execute('houseMenu.returnToDefault()');
    switch (menu) {
      case 'buyMenu':
        global.houseMenuHTML.execute(`houseMenu.updateInfo(${data})`);
        global.houseMenuHTML.execute('houseMenu.dirty.title=\'Покупка дома\'');
        global.houseMenuHTML.execute('houseMenu.dirty.active=true;');
        global.houseMenuHTML.execute('houseMenu.dirty.buyActive=true;');

        break;
      case 'aboutMenu':
        global.houseMenuHTML.execute(`houseMenu.updateInfo(${data})`);
        global.houseMenuHTML.execute('houseMenu.dirty.title=\'Информация о доме\'');
        global.houseMenuHTML.execute('houseMenu.dirty.active=true;');
        global.houseMenuHTML.execute('houseMenu.dirty.aboutActive=true;');

        break;
      case 'manageMenu':
        global.houseMenuHTML.execute(`houseMenu.updateInfo(${data})`);
        global.houseMenuHTML.execute('houseMenu.dirty.title=\'Управление домом\'');
        global.houseMenuHTML.execute('houseMenu.dirty.active=true;');
        global.houseMenuHTML.execute('houseMenu.dirty.manageActive=true;');

        break;
      case 'carsListMenu':
        global.houseMenuHTML.execute(`houseMenu.updateCarsList(${data})`);
        global.houseMenuHTML.execute('houseMenu.dirty.title=\'Список машин в гараже\'');
        global.houseMenuHTML.execute('houseMenu.dirty.active=true;');
        global.houseMenuHTML.execute('houseMenu.dirty.carsListActive=true;');

        break;
      case 'saleCarsListMenu':
        global.houseMenuHTML.execute(`houseMenu.updateCarsList(${data})`);
        global.houseMenuHTML.execute('houseMenu.dirty.title=\'Список машин в гараже\'');
        global.houseMenuHTML.execute('houseMenu.dirty.active=true;');
        global.houseMenuHTML.execute('houseMenu.dirty.saleCarsListActive=true;');

        break;
      case 'carManageMenu':
        global.houseMenuHTML.execute(`houseMenu.updateSelectedCarInfo(${data})`);
        global.houseMenuHTML.execute('houseMenu.dirty.active=true;');
        global.houseMenuHTML.execute('houseMenu.dirty.carManageActive=true;');

        break;
      case 'furnitureListMenu':
        global.houseMenuHTML.execute(`houseMenu.updateFurnitureList(${data})`);
        global.houseMenuHTML.execute('houseMenu.dirty.title=\'Управление мебелью\'');
        global.houseMenuHTML.execute('houseMenu.dirty.active=true;');
        global.houseMenuHTML.execute('houseMenu.dirty.furnitureListActive=true;');

        break;
      case 'furnitureManageMenu':
        global.houseMenuHTML.execute(`houseMenu.updateFurnitureInfo(${data})`);
        global.houseMenuHTML.execute('houseMenu.dirty.title=\'Управление мебелью\'');
        global.houseMenuHTML.execute('houseMenu.dirty.active=true;');
        global.houseMenuHTML.execute('houseMenu.dirty.furnitureManageActive=true;');

        break;
      case 'roommatesListMenu':
        global.houseMenuHTML.execute(`houseMenu.updateRoommatesList(${data})`);
        global.houseMenuHTML.execute('houseMenu.dirty.title=\'Сожители\'');
        global.houseMenuHTML.execute('houseMenu.dirty.active=true;');
        global.houseMenuHTML.execute('houseMenu.dirty.roommatesListActive=true;');

        break;
      case 'guestsListMenu':
        global.houseMenuHTML.execute(`houseMenu.updateGuestsList(${data})`);
        global.houseMenuHTML.execute('houseMenu.dirty.title=\'Гости\'');
        global.houseMenuHTML.execute('houseMenu.dirty.active=true;');
        global.houseMenuHTML.execute('houseMenu.dirty.guestsListActive=true;');

        break;
    }

    global.houseMenu.lastTime = 0;

    global.houseMenu.menuState = true;

    if (global.houseMenu.menuState) {
      global.menuOpen();
    }

    mp.gui.cursor.visible = true;

    global.houseMenuOpen = true;
  },
  close() {
    if(global.houseMenuHTML !== null && global.houseMenuHTML.active) {
      global.anyMenuHTML.removeCloseCallback();
    }

    global.houseMenuHTML.active = false;
    global.houseMenu.menuState = false;



    if (global.houseMenuHTML != null) {
      global.houseMenuHTML.execute('houseMenu.active=false;');
    }

    if (!global.houseMenu.menuState) {
      global.menuClose();
    }

    mp.events.call('stopScreenEffect', 'MenuMGHeistIn');
    global.houseMenuOpen = false;
  },
  info(data) {
    global.houseMenuHTML.execute(`houseMenu.updateInfo(${data})`);
  },
  buy() {
    mp.events.call('buyHouse');
  },
  view() {
    mp.events.call('viewHouse');
  },

  // Manage
  toggleLock() {
    mp.events.call('toggleLock');
  },
  openFurnitureList() {
    mp.events.call('openFurnitureList');
  },
  openCarsList() {
    mp.events.call('openCarsList');
  },
  openCarManage(number) {
    mp.events.call('openCarManage', number);
  },
  openCarSale(number) {
    mp.events.call('openCarSale', number);
  },
  openGuestList() {
    mp.events.call('openGuestList');
  },
  openRoommatesList() {
    mp.events.call('openRoommatesList');
  },
  removeRoommate(name) {
    mp.events.call('removeRoommate', name);
  },
  leaveHouse() {
    mp.events.call('leaveHouse');
  },
  kickAll() {
    mp.events.call('kickAll');
  },
  kickGuest(id) {
    mp.events.call('kickGuest', id);
  },

  // danger
  sell() {
    mp.events.call('sellHouse');
  },
};

mp.events.add('buyHouse', () => {
  if (new Date().getTime() - global.lastCheck < 50) return;
  global.lastCheck = new Date().getTime();

  global.houseMenu.close();

  global.anyEvents.SendServer(() => mp.events.callRemote('houseBuyMenu', 'buy'));
});

mp.events.add('enterHouse', () => {
  if (new Date().getTime() - global.lastCheck < 50) return;
  global.lastCheck = new Date().getTime();

  global.houseMenu.close();

  mp.events.callRemote('enterHouse');
});

mp.events.add('viewHouse', () => {
  if (new Date().getTime() - global.lastCheck < 50) return;
  global.lastCheck = new Date().getTime();

  global.houseMenu.close();

  mp.events.callRemote('houseBuyMenu', 'view');
});

mp.events.add('manageHouse', () => {
  if (new Date().getTime() - global.lastCheck < 50) return;
  global.lastCheck = new Date().getTime();

  global.houseMenu.close();

  mp.events.callRemote('openHouseManageMenu');
});

mp.events.add('openHouseMenu', (menu, ...data) => {
  global.houseMenu.open(menu, data);
});

mp.events.add('closeHouseMenu', () => {
  global.houseMenu.close();
  mp.events.call('showHUD', true);
});

mp.events.add('toggleLock', () => {
  if (new Date().getTime() - global.lastCheck < 50) return;
  global.lastCheck = new Date().getTime();

  mp.events.callRemote('houseManageMenu', 'toggleLock');
});

mp.events.add('openFurnitureList', () => {
  if (new Date().getTime() - global.lastCheck < 50) return;
  global.lastCheck = new Date().getTime();

  mp.events.callRemote('houseManageMenu', 'furnitureList');
});

mp.events.add('openFurnitureManage', (id) => {
  if (new Date().getTime() - global.lastCheck < 50) return;
  global.lastCheck = new Date().getTime();

  global.selectedFurnitureManageId = id;

  mp.events.callRemote('openFurnitureManage', id);
});

mp.events.add('openGuestList', () => {
  mp.events.callRemote('houseManageMenu', 'guestList');
});

mp.events.add('openRoommatesList', () => {
  mp.events.callRemote('houseManageMenu', 'roommatesList');
});

mp.events.add('updateHouseCarsList', (data) => {
  // mp.console.logError("updateHouseCarsList: -> " + data);
  global.houseMenuHTML.execute(`houseMenu.updateCarsList(${data})`);
});

mp.events.add('houseGetCarCharacteristics', (model) => {
  const characteristics = global.init.getCharacteristics(model);

  global.houseMenuHTML.execute(`houseMenu.dirty.car.characteristic.speed=${characteristics[0]}`);
  global.houseMenuHTML.execute(`houseMenu.dirty.car.characteristic.brakes=${characteristics[1]}`);
  global.houseMenuHTML.execute(`houseMenu.dirty.car.characteristic.boost=${characteristics[2]}`);
  global.houseMenuHTML.execute(`houseMenu.dirty.car.characteristic.clutch=${characteristics[3]}`);
});

mp.events.add('updateHouseRoommatesList', (data) => {
  global.houseMenuHTML.execute(`houseMenu.updateRoommatesList(${data})`);
});

mp.events.add('updateHouseGuestsList', (data) => {
  global.houseMenuHTML.execute(`houseMenu.updateGuestsList(${data})`);
});

mp.events.add('openRoommateManage', (id) => {
  if (new Date().getTime() - global.lastCheck < 50) return;
  global.lastCheck = new Date().getTime();

  global.selectedRoommateManageId = id;

  mp.events.callRemote('openRoommateManage', id);
});

mp.events.add('inviteToHouseGuest', (id) => {
  if (new Date().getTime() - global.lastCheck < 50) return;
  global.lastCheck = new Date().getTime();

  global.anyEvents.SendServer(() => mp.events.callRemote('inviteToHouseGuest', id));
});

mp.events.add('inviteToHouseRoommate', (id) => {
  if (new Date().getTime() - global.lastCheck < 50) return;
  global.lastCheck = new Date().getTime();

  global.anyEvents.SendServer(() => mp.events.callRemote('inviteToHouseRoommate', id));
});

mp.events.add('removeRoommate', (name) => {
  if (new Date().getTime() - global.lastCheck < 50) return;
  global.lastCheck = new Date().getTime();

  global.houseMenu.close();

  global.anyEvents.SendServer(() => mp.events.callRemote('roommatesActions', 'remove', name));
});

mp.events.add('leaveHouse', () => {
  if (new Date().getTime() - global.lastCheck < 50) return;
  global.lastCheck = new Date().getTime();

  global.houseMenu.close();

  global.anyEvents.SendServer(() => mp.events.callRemote('roommatesActions', 'leave'));
});

mp.events.add('removeAllRoommates', () => {
  if (new Date().getTime() - global.lastCheck < 50) return;
  global.lastCheck = new Date().getTime();

  global.houseMenu.close();

  global.anyEvents.SendServer(() => mp.events.callRemote('roommatesActions', 'removeAll'));
});

mp.events.add('openRoommateManage', (id) => {
  if (new Date().getTime() - global.lastCheck < 50) return;
  global.lastCheck = new Date().getTime();

  global.selectedRoommateManageId = id;

  global.anyEvents.SendServer(() => mp.events.callRemote('openRoommateManage', id));
});

mp.events.add('openCarsList', () => {
  if (new Date().getTime() - global.lastCheck < 50) return;
  global.lastCheck = new Date().getTime();

  mp.events.callRemote('houseManageMenu', 'carsList');
});

mp.events.add('openCarSale', (number) => {
  if (new Date().getTime() - global.lastCheck < 50) return;
  global.lastCheck = new Date().getTime();
  global.selectedCarManageNumber = number;

  global.anyEvents.SendServer(() => mp.events.callRemote('openCarSale', number));
});

mp.events.add('kickAll', () => {
  if (new Date().getTime() - global.lastCheck < 50) return;
  global.lastCheck = new Date().getTime();

  global.anyEvents.SendServer(() => mp.events.callRemote('houseManageMenu', 'kickAll'));
});

mp.events.add('kickGuest', (name) => {
  if (new Date().getTime() - global.lastCheck < 50) return;
  global.lastCheck = new Date().getTime();

  global.anyEvents.SendServer(() => mp.events.callRemote('houseManageMenu', 'kickGuest', name));
});

mp.events.add('sellHouse', () => {
  if (new Date().getTime() - global.lastCheck < 50) return;
  global.lastCheck = new Date().getTime();

  global.anyEvents.SendServer(() => mp.events.callRemote('houseManageMenu', 'sell'));
});

mp.events.add('setSelectedCarManageNumber', (number) => {
  global.selectedCarManageNumber = number;
});

// Car Manage
mp.events.add('carRepair', () => {
  if (new Date().getTime() - global.lastCheck < 50) return;
  global.lastCheck = new Date().getTime();

  global.anyEvents.SendServer(() => mp.events.callRemote('houseCarManageMenu', 'carRepair', global.selectedCarManageNumber));
});

mp.events.add('carGetKey', () => {
  if (new Date().getTime() - global.lastCheck < 50) return;
  global.lastCheck = new Date().getTime();

  global.anyEvents.SendServer(() => mp.events.callRemote('houseCarManageMenu', 'carGetKey', global.selectedCarManageNumber));
});

mp.events.add('carChangeKey', () => {
  if (new Date().getTime() - global.lastCheck < 50) return;
  global.lastCheck = new Date().getTime();

  global.anyEvents.SendServer(() => mp.events.callRemote('houseCarManageMenu', 'carChangeKey', global.selectedCarManageNumber));
});

mp.events.add('carEvacuation', () => {
  if (new Date().getTime() - global.lastCheck < 50) return;
  global.lastCheck = new Date().getTime();

  global.anyEvents.SendServer(() => mp.events.callRemote('houseCarManageMenu', 'carEvacuation', global.selectedCarManageNumber));
});

mp.events.add('carGps', () => {
  if (new Date().getTime() - global.lastCheck < 50) return;
  global.lastCheck = new Date().getTime();

  global.anyEvents.SendServer(() => mp.events.callRemote('houseCarManageMenu', 'carGps', global.selectedCarManageNumber));
});

mp.events.add('carEvacuationPosition', () => {
  if (new Date().getTime() - global.lastCheck < 50) return;
  global.lastCheck = new Date().getTime();

  global.anyEvents.SendServer(() => mp.events.callRemote('houseCarManageMenu', 'carEvacuationPosition', global.selectedCarManageNumber));
});

mp.events.add('carSell', () => {
  if (new Date().getTime() - global.lastCheck < 50) return;
  global.lastCheck = new Date().getTime();
  global.houseMenu.close();

  global.anyEvents.SendServer(() => mp.events.callRemote('houseCarManageMenu', 'carSell', global.selectedCarManageNumber));
});

mp.events.add('furnitureSet', () => {
  if (new Date().getTime() - global.lastCheck < 50) return;
  global.lastCheck = new Date().getTime();

  global.anyEvents.SendServer(() => mp.events.callRemote('furnitureActions', 'furnitureSet', global.selectedFurnitureManageId));
});

mp.events.add('furnitureSell', () => {
  if (new Date().getTime() - global.lastCheck < 50) return;
  global.lastCheck = new Date().getTime();

  global.anyEvents.SendServer(() => mp.events.callRemote('furnitureActions', 'furnitureSell', global.selectedFurnitureManageId));
});

global.casinoKeys = mp.browsers.new('package://cef/UI/CasinoKeys/index.html');

mp.events.add('casinoKeys', (act, ...data) => {
  switch (act) {
    case 'setChips':
      global.casinoKeys.execute(`casinoKeys.setChips('${data[0]}', '${data[1]}')`);
        break;
    case 'setBet':
      global.casinoKeys.execute(`casinoKeys.setBet('${data[0]}', '${data[1]}')`);
      break;
    case 'setTime':
      global.casinoKeys.execute(`casinoKeys.setTime('${data[0]}', '${data[1]}')`);
      break;
    case 'toggleStart':
      global.casinoKeys.execute(`casinoKeys.toggleStart(${data[0]})`);
      break;
    case 'show':
      global.casinoKeys.execute('casinoKeys.show()');
      break;
    case 'hide':
      global.casinoKeys.execute('casinoKeys.hide()');
        break;
    }
});

global.casinoOpened = false;

global.casinoClose = function () {
  global.casinoOpened = false;
  global.menuOpened = false;
    mp.gui.cursor.visible = false;
};
global.casinoOpen = function () {
  mp.gui.cursor.visible = true;
  global.casinoOpened = true;
  global.menuOpened = true;
};

mp.events.add('client_press_key_to', (act, data) => {
  switch (act) {
    case 'open':
      if (global.menuCheck()) return;
      global.menu.execute(`presskeyto.open('${data}')`);
      break;
    case 'close':
      if (global.menuCheck()) return;
      global.menu.execute('presskeyto.close()');
      break;
  }
});

mp.events.add('client_casino_bet', (act, data) => {
  switch (act) {
    case 'open':
      if (global.menuCheck()) return;
      global.casinoOpen();
      global.menu.execute(`casino.show('${data}')`);
      break;
    case 'close':
      global.casinoClose();
      global.menu.execute('casino.hide()');
      break;
  }
});

mp.events.add('updateCasinoTime', (data) => {
  global.menu.execute(`casino.setTimeToStart('${data}')`);
});

mp.events.add('updateCasinoChips', (data) => {
  global.menu.execute(`casino.setChips('${data}')`);
});

mp.keys.bind(global.Keys.VK_ESCAPE, false, () => {
  if (global.slotStarted) return;
  if (global.cutscenePlaying) return;

  if (global.menuOpened) {
    const createCharacterIndex = global.openedMenus["Oscar2"].indexOf("createCharacter");
    if (createCharacterIndex >= 0) return;

    const cutSceneIndex = global.openedMenus["FeST1VaL"].indexOf("cutscene");
    if (cutSceneIndex >= 0) return;


    global.HTMLMenuClose();
    global.anyMenuHTML.closeAllMenu();

    global.menuClose();
  }
});

mp.events.add('CLIENT::HTML:close', () => {
  global.HTMLMenuClose();
});

global.HTMLMenuClose = function () {
  // mp.console.logInfo("HTMLMenuClosed");

  // if(global.questMenuHTML !== null && global.questMenuHTML.active) {
  //   global.anyMenuHTML.removeCloseCallback();
  //   global.questMenuHTML.active = false;
  // }

  if(global.tuningMenuHTML !== null && global.tuningMenuHTML.active) {
    global.anyMenuHTML.removeCloseCallback();
    global.tuningMenuHTML.execute("tuningMenu.close()");
    // mp.events.call("tuningCloseMenu");
  }

  if(global.workMenuHTML !== null && global.workMenuHTML.active) {
    global.anyMenuHTML.removeCloseCallback();
    global.workMenuHTML.active = false;
  }

  if(global.rouletteMenuHTML !== null && global.rouletteMenuHTML.active) {
    mp.events.call("CLIENT::caseRoulette:close");
  }

  if(global.BattlePassRouletteMenuHTML !== null && global.BattlePassRouletteMenuHTML.active) {
    mp.events.call("CLIENT::battlePassRoulette:close");
  }

  if(global.robberyHTML !== null && global.robberyHTML.active) {
    global.anyMenuHTML.removeCloseCallback();
    mp.events.call('CLIENT::robbery:close');
    global.robberyHTML.active = false;
  }

  if (global.menu !== null && global.menu.active) {
    mp.events.call('closePetrol');
  }

  // Инвентарь
  closeInventory();

  // Работы
  mp.events.call('closeJobMenu');

  // Дом
  global.houseMenu.close();

  // Всякие эффекты
  mp.events.call('stopScreenEffect', 'MenuMGHeistIn');
  // mp.events.call('fromBlur', 200);
};

// require("/cef/resources/js/modules/unitls");
// function getCharacteristics(model = null) {
//   let speed;
//   let brakes;
//   let boost;
//   let clutch;
//
//   if (model !== null) {
//     model = mp.game.joaat(model);
//
//     speed = (mp.game.vehicle.getVehicleModelMaxSpeed(model) / 1.2).toFixed();
//     brakes = (mp.game.vehicle.getVehicleModelMaxBraking(model) * 100).toFixed(2);
//     boost = (mp.game.vehicle.getVehicleModelAcceleration(model) * 100).toFixed(2);
//     clutch = (mp.game.vehicle.getVehicleModelMaxTraction(model) * 10).toFixed(2);
//   } else {
//     speed = (mp.game.vehicle.getVehicleModelMaxSpeed(localplayer.vehicle.model) / 1.2).toFixed();
//     brakes = (localplayer.vehicle.getMaxBraking() * 100).toFixed(2);
//     boost = (localplayer.vehicle.getAcceleration() * 100).toFixed(2);
//     clutch = (localplayer.vehicle.getMaxTraction() * 10).toFixed(2);
//   }
//
//   return [
//     speed,
//     brakes,
//     boost,
//     clutch,
//   ];
// }

// function getLocalCharacteristics(model = null) {
//   const speed = (mp.game.vehicle.getVehicleModelMaxSpeed(model) / 1.2).toFixed();
//   const brakes = (localplayer.vehicle.getMaxBraking() * 100).toFixed(2);
//   const boost = (localplayer.vehicle.getAcceleration() * 100).toFixed(2);
//   const clutch = (localplayer.vehicle.getMaxTraction() * 10).toFixed(2);
//
//   return [
//     speed,
//     brakes,
//     boost,
//     clutch,
//   ];
// }

mp.events.add('notificationTOP', (text, timeout = 3000, type = false, color = 0) => {
  mp.gui.execute(`HUD.notificationPanel="${true}"`);

  if (type === 'countdown') {
    for (let i = text, time = 1; i >= 1; i--, time++) {
      (function (count, timeout) {
        setTimeout(() => {
          mp.gui.execute(`HUD.notification.text="${count}"`);
        }, timeout * 1000);
      }(i, time));
    }

    setTimeout(() => {
      mp.gui.execute('HUD.notification.text="START!"');
    }, 3500);

    setTimeout(() => {
      mp.gui.execute(`HUD.notificationPanel="${false}"`);
      mp.gui.execute('HUD.notification.text=""');
    }, 4500);

    return;
  };

  mp.gui.execute(`HUD.notification.text="${text}"`);
  mp.gui.execute(`HUD.notification.color=${color}`);

  if (timeout !== 0) {
    (function (timeout, color) {
      setTimeout(() => {
        mp.gui.execute(`HUD.notificationPanel="${false}"`);
        mp.gui.execute('HUD.notification.color=0');
        mp.gui.execute('HUD.notification.text=""');
      }, timeout);
    }(timeout, color));
  }
});






















// // // // //
global.dialog = {
  question: '',
  cBack: '',
  menuWasopened: false,
  lastTime: false,
  open() {
    global.dialog.lastTime = 0;
    if (global.menu != null) {
      global.menu.execute(`dialog.title='${global.dialog.question}'`);
      global.menu.execute('dialog.active=1');
    }
    global.menuWasopened = global.menuOpened;
    mp.gui.cursor.visible = true;

    if (!global.menuOpened) global.menuOpen();

    mp.events.call('startScreenEffect', 'MenuMGHeistIn', 1, true);
  },
  close() {
    if (global.menu != null) global.menu.execute('dialog.active=0');
    // if (global.menuOpened) global.menuClose();
    // if (!menuWasopened) global.menuClose();

    global.menuClose();

    mp.events.call('stopScreenEffect', 'MenuMGHeistIn');
  },
};
mp.events.add('openDialog', (c, q) => {
  global.dialog.cBack = c;
  global.dialog.question = q;
  global.dialog.open();
  mp.gui.cursor.visible = true;
});
mp.events.add('closeDialog', () => {
  global.dialog.close();
});
mp.events.add('dialogCallback', (state) => {
  if (global.dialog.cBack === 'tuningbuy') mp.events.call('tuningBuyComponent', state);
  else global.anyEvents.SendServer(() => mp.events.callRemote('dialogCallback', global.dialog.cBack, state));

  global.dialog.close();
});
// DIAL //
mp.events.add('dial', (act, val, reset) => {
  switch (act) {
    case 'open':
      if (reset == true) {
        global.menu.execute('dial.hide()');
        global.menuClose();
      }

      var off = Math.random(2, 5);

      global.menu.execute(`dial.val=${val};dial.off=${off};dial.show();`);
      global.menuOpen();

      break;
    case 'close':
      global.menu.execute('dial.hide()');
      global.menuClose();
      break;
    case 'call':
      global.anyEvents.SendServer(() => mp.events.callRemote('dialPress', val));
      global.menuClose();
      break;
  }
});
// STOCK //
mp.events.add('openStock', (data) => {
  if (global.menuCheck()) return;
  global.menu.execute(`stock.count=JSON.parse('${data}');stock.show();`);
  global.menuOpen();
});
mp.events.add('closeStock', () => {
  global.menuClose();
});
mp.events.add('stockTake', (index) => {
  global.menuClose();
  switch (index) {
    case 3: // mats
      global.anyEvents.SendServer(() => mp.events.callRemote('setStock', 'mats'));
      global.input.set('Взять маты', 'Введите кол-во матов', 10, 'take_stock');
      global.input.open();
      break;
    case 0: // cash
      global.anyEvents.SendServer(() => mp.events.callRemote('setStock', 'money'));
      global.input.set('Взять деньги', 'Введите кол-во денег', 10, 'take_stock');
      global.input.open();
      break;
    case 1: // healkit
      global.anyEvents.SendServer(() => mp.events.callRemote('setStock', 'medkits'));
      global.input.set('Взять аптечки', 'Введите кол-во аптечек', 10, 'take_stock');
      global.input.open();
      break;
    case 2: // weed
      global.anyEvents.SendServer(() => mp.events.callRemote('setStock', 'drugs'));
      global.input.set('Взять наркотики', 'Введите кол-во наркоты', 10, 'take_stock');
      global.input.open();
      break;
    case 4: // weapons stock
      global.anyEvents.SendServer(() => mp.events.callRemote('openWeaponStock'));
      break;
  }
});
mp.events.add('stockPut', (index) => {
  global.menuClose();
  switch (index) {
    case 3: // mats
      global.anyEvents.SendServer(() => mp.events.callRemote('setStock', 'mats'));
      global.input.set('Положить маты', 'Введите кол-во матов', 10, 'put_stock');
      global.input.open();
      break;
    case 0: // cash
      global.anyEvents.SendServer(() => mp.events.callRemote('setStock', 'money'));
      global.input.set('Положить деньги', 'Введите кол-во денег', 10, 'put_stock');
      global.input.open();
      break;
    case 1: // healkit
      global.anyEvents.SendServer(() => mp.events.callRemote('setStock', 'medkits'));
      global.input.set('Положить аптечки', 'Введите кол-во аптечек', 10, 'put_stock');
      global.input.open();
      break;
    case 2: // weed
      global.anyEvents.SendServer(() => mp.events.callRemote('setStock', 'drugs'));
      global.input.set('Положить наркотики', 'Введите кол-во наркоты', 10, 'put_stock');
      global.input.open();
      break;
    case 4: // weapons stock
      global.anyEvents.SendServer(() => mp.events.callRemote('openWeaponStock'));
      break;
  }
});
mp.events.add('stockExit', () => {
  global.menuClose();
});
// POLICE PC //
let pcSubmenu;
mp.events.add('pcMenu', (index) => {
  switch (index) {
    case 1:
      global.menu.execute('pc.clearWanted()');
      pcSubmenu = 'clearWantedLvl';
      return;
    case 2:
      global.menu.execute('pc.openCar()');
      pcSubmenu = 'checkNumber';
      return;
    case 3:
      global.menu.execute('pc.openPerson()');
      pcSubmenu = 'checkPerson';
      return;
    case 4:
      global.anyEvents.SendServer(() => mp.events.callRemote('checkWantedList'));
      pcSubmenu = 'wantedList';
      return;
    case 5:
      global.menu.execute('pc.hide()');
      global.menuClose();
  }
});
mp.events.add('pcMenuInput', (data) => {
  mp.events.callRemote(pcSubmenu, data);
});
mp.events.add('executeWantedList', (data) => {
  global.menu.execute(`pc.openWanted('${data}')`);
});
mp.events.add('executeCarInfo', (model, holder) => {
  global.menu.execute(`pc.openCar("${model}","${holder}")`);
});
mp.events.add('executePersonInfo', (name, lastname, uuid, gender, wantedlvl, lic) => {
  global.menu.execute(`pc.openPerson("${name}","${lastname}","${uuid}","${gender}","${wantedlvl}","${lic}")`);
});

mp.events.add('openPc', () => {
  if (global.menuCheck()) return;
  global.menu.execute('pc.show()');
  global.menuOpen();
});
mp.events.add('closePc', () => {
  if (global.menu !== null) {
    global.menu.execute('pc.hide()');
    global.menuClose();
  }
});
// DOCS //
mp.events.add('passport', (data) => {
  if (global.menu !== null) {
    global.menu.execute(`passport.set('${data}');`);
    if (global.menuCheck()) return;
    global.menu.execute('passport.show()');
    global.menuOpen();
  }
});
mp.events.add('licenses', (data) => {
  global.menu.execute(`license.set('${data}');`);
  if (global.menuCheck()) return;
  global.menu.execute('license.show()');
  global.menuOpen();
});
mp.events.add('dochide', () => {
  global.menuClose();
});

global.workMenuHTML = mp.browsers.new('package://cef/resources/views/menus/work.html');
global.workMenuHTML.active = false;

mp.events.add('showJobMenu', (level, currentjob) => {
  global.closeAnyMenuFunctions.closePhone();

  mp.gui.cursor.visible = true;
  global.workMenuHTML.active = true;
  global.workMenuHTML.menuState = true;

  if (global.workMenuHTML.menuState) {
    global.menuOpen();
  }

  global.workMenuHTML.execute(`openWorks(${level},${currentjob});`);
});

mp.events.add('closeJobMenu', () => {
  if (global.workMenuHTML !== null && global.workMenuHTML.active) {
    global.anyMenuHTML.removeCloseCallback();
  }

  mp.gui.cursor.visible = false;
  global.workMenuHTML.active = false;
  global.workMenuHTML.menuState = false;

  if (!global.workMenuHTML.menuState) {
    global.menuClose();
  }

  global.workMenuHTML.execute('jobselector.active=false;');
});

mp.events.add('selectJob', (jobid) => {
  if (new Date().getTime() - global.lastCheck < 1000) return;
  global.lastCheck = new Date().getTime();

  global.anyEvents.SendServer(() => mp.events.callRemote('jobjoin', jobid));
  mp.events.call('closeJobMenu');
});

// SIMPLE MENU //
function openSM(type, data) {
  if (global.menuCheck()) return;
  global.menu.execute('menu.show()');
  switch (type) {
    // case 0: menu.execute(`openWorks(${data});`); break;
    case 1: menu.execute(`openShop('${data}');`); break;
    case 2: menu.execute(`openBlack('${data}');`); break;
    case 3: menu.execute(`openFib('${data}');`); break;
    case 4: menu.execute(`openLspd('${data}');`); break;
    case 5: menu.execute(`openArmy('${data}');`); break;
    case 6: menu.execute(`openGov('${data}');`); break;
    case 7: menu.execute(`openArmygun('${data}');`); break;
    case 8: menu.execute(`openGang('${data}');`); break;
    case 9: menu.execute(`openMafia('${data}');`); break;
    case 10: menu.execute(`openFishShop('${data}');`); break;
    case 11: menu.execute(`openSheriff('${data}');`); break;
    case 12: menu.execute(`openMWgun('${data}');`); break;
  }
  global.menuOpen();
}
function closeSM() {
  global.menu.execute('menu.hide()');
  global.menuClose();
}
mp.events.add('smExit', () => {
  // mp.gui.chat.push('exit');
  closeSM();
});
mp.events.add('smOpen', (type, data) => {
  openSM(type, data);
});
mp.events.add('menu', (action, data) => {
  switch (action) {
    case 'resign':
      global.anyEvents.SendServer(() => mp.events.callRemote('jobjoin', -1));
      break;
    case 'work':
      global.anyEvents.SendServer(() => mp.events.callRemote('jobjoin', data));
      break;
    case 'shop':
      global.anyEvents.SendServer(() => mp.events.callRemote('shop', data));
      break;
    case 'black':
      global.anyEvents.SendServer(() => mp.events.callRemote('mavrbuy', data));
      break;
    case 'fib':
      global.anyEvents.SendServer(() => mp.events.callRemote('fbigun', data));
      break;
    case 'lspd':
      global.anyEvents.SendServer(() => mp.events.callRemote('lspdgun', data));
      break;
    case 'army':
      global.anyEvents.SendServer(() => mp.events.callRemote('armygun', data));
      break;
    case 'gov':
      global.anyEvents.SendServer(() => mp.events.callRemote('govgun', data));
      break;
    case 'gang':
      global.anyEvents.SendServer(() => mp.events.callRemote('gangmis', data));
      break;
    case 'mafia':
      global.anyEvents.SendServer(() => mp.events.callRemote('mafiamis', data));
      break;
    case 'fishshop':
      global.anyEvents.SendServer(() => mp.events.callRemote('fishshop', data));
      break;
    case 'sheriff':
      global.anyEvents.SendServer(() => mp.events.callRemote('sheriffgun', data));
      break;
    case 'mw':
      global.anyEvents.SendServer(() => mp.events.callRemote('merygun', data));
      break;
  }
});
// SM DATA //
mp.events.add('policeg', () => {
  const data = [
    'Дубинка',
    'Пистолет',
    'SMG',
    'Дробовик',
    'Tazer',
    'Бронежилет',
    'Аптечка',
    'Пистолетный калибр x12',
    'Малый калибр x30',
    'Дробь x6',
    'LSPD квадрокоптер',
  ];
  openSM(4, JSON.stringify(data));
});

mp.events.add('sheriffg', () => {
  const data = [
    'Дубинка',
    'Пистолет',
    'SMG',
    'Дробовик',
    'Tazer',
    'Бронежилет',
    'Аптечка',
    'Пистолетный калибр x12',
    'Малый калибр x30',
    'Дробь x6',
    'LSPD квадрокоптер',
  ];
  openSM(4, JSON.stringify(data));
});

mp.events.add('fbiguns', () => {
  const data = [
    'Tazer',
    'Пистолет',
    'ПОС',
    'Карабин',
    'Снайперская винтовка',
    'Бронежилет',
    'Аптечка',
    'Пистолетный калибр x12',
    'Малый калибр x30',
    'Автоматный калибр x30',
    'Снайперский калибр x5',
    'Бейдж',
    'LSPD квадрокоптер',
  ];
  openSM(3, JSON.stringify(data));
});
mp.events.add('govguns', () => {
  const data = [
    'Tazer',
    'Пистолет',
    'Advanced Rifle',
    'Gusenberg Sweeper',
    'Бронежилет',
    'Аптечка',
    'Пистолетный калибр x12',
    'Малый калибр x30',
    'Автоматный калибр x30',
  ];
  openSM(6, JSON.stringify(data));
});
mp.events.add('armyguns', () => {
  const data = [
    'Пистолет',
    'Карабин',
    'Боевой пулемет',
    'Бронежилет',
    'Аптечка',
    'Пистолетный калибр x12',
    'Автоматный калибр x30',
    'Малый калибр x100',
  ];
  openSM(7, JSON.stringify(data));
});
mp.events.add('meryg', () => {
  const data = [
    'Дубинка',
    'Пистолет',
    'Compact Rifle',
    'Пистолетный калибр x12',
    'RiflesAmmo',
    'Аптечка',
    'Бронежилет',
  ];
  openSM(12, JSON.stringify(data));
});
mp.events.add('mavrshop', () => {
  const data = [
    ['Услуга по отмыву денег', ''],
    ['Дрель для взлома', '100000$'],
    ['Отмычка для замков', '600$'],
    ['Военная отмычка', '2500$'],
    ['Стяжки для рук', '2500$'],
    ['Мешок на голову', '2500$'],
    ['Понизить розыск', '3000$'],
    ['Бронежилет', '200000$'],
  ];
  openSM(2, JSON.stringify(data));
});
mp.events.add('gangmis', () => {
  const data = [
    'Угон автотранспорта',
    'Перевозка автотранспорта',
  ];
  openSM(8, JSON.stringify(data));
});
mp.events.add('mafiamis', () => {
  const data = [
    'Перевозка оружия',
    'Перевозка денег',
    'Перевозка трупов',
  ];
  openSM(9, JSON.stringify(data));
});
mp.events.add('shop', (json) => {
  const data = JSON.parse(json);
  openSM(1, JSON.stringify(data));
});
mp.events.add('fishshop', (json) => {
  const data = JSON.parse(json);
  global.openSM(10, JSON.stringify(data));
});
// ELEVATOR //
let liftcBack = '';
function openLift(type, cBack) {
  if (global.menuCheck()) return;
  const floors = [
    // ['Гараж', '1 этаж', '49 этаж', 'Крыша'],
    ['Гараж', '1 этаж', 'Крыша'],
  ];
  const json = JSON.stringify(floors[type]);
  global.menu.execute(`lift.set('${json}');lift.active=1;`);
  global.menuOpen();
  liftcBack = cBack;
}
function closeLift() {
  global.menuClose();
  global.menu.execute('lift.active=0;lift.reset();');
  liftcBack = '';
}
mp.events.add('openlift', (type, cBack) => {
  openLift(type, cBack);
});
mp.events.add('lift', (act, data) => {
  switch (act) {
    case 'stop':
      closeLift();
      break;
    case 'start':
      mp.events.callRemote(liftcBack, data);
      closeLift();
      break;
  }
});
// PETROL //
mp.events.add('petrol', (data, data2) => {
  global.anyEvents.SendServer(() => mp.events.callRemote('petrol', data, data2));
  //
  global.menuClose();
  global.menu.execute('petrol.reset()');
});
mp.events.add('petrol.full', () => {
  global.anyEvents.SendServer(() => mp.events.callRemote('petrol', 9999));
  //
  global.menuClose();
  global.menu.execute('petrol.reset()');
});
mp.events.add('petrol.gov', () => {
  global.anyEvents.SendServer(() => mp.events.callRemote('petrol', 99999, 'gov'));
  //
  global.menuClose();
  global.menu.execute('petrol.reset()');
});

mp.events.add('openPetrol', (fuelType, fuel, full, price) => {
  if (global.menuCheck()) return;
  global.menuOpen();
  global.menu.execute(`petrol.changeLeftToFull('${fuelType}',${fuel},${full},${price})`);
  global.menu.execute('petrol.active=1');
});
mp.events.add('closePetrol', () => {
  global.menuClose();
  global.menu.execute('petrol.reset()');
});

// FRACTION MENU //

mp.events.add('openfm', () => {
  if (global.menuCheck()) return;
  global.menuOpen();
  global.menu.execute('fmenu.active=1');
});

mp.events.add('setmem', (json, count, on, off) => {
  global.menu.execute(`fmenu.set('${json}',${count},${on},${off});`);
});

mp.events.add('closefm', () => {
  global.menuClose();
});

mp.events.add('fmenu', (act, data1, data2) => {
  global.anyEvents.SendServer(() => mp.events.callRemote('fmenu', act, data1, data2));
  global.menuClose();
});

// MATS //
/* mp.keys.bind(0x78, false, function () { // F9
 mp.events.call('matsOpen', true);
 }); */
mp.events.add('matsOpen', (isArmy, isMed) => {
  if (global.menuCheck()) return;
  global.menuOpen();
  global.menu.execute(`mats.show(${isArmy},${isMed})`);
});
mp.events.add('matsL', (act) => { // load
  global.menuClose();
  switch (act) {
    case 1:
      global.input.set('Загрузить маты', 'Введите кол-во матов', 4, 'loadmats');
      global.input.open();
      break;
    case 2:
      global.input.set('Загрузить маты', 'Введите кол-во матов', 4, 'loadmats');
      global.input.open();
      break;
    case 3:
      global.input.set('Загрузить наркоту', 'Введите кол-во наркоты', 4, 'loaddrugs');
      global.input.open();
      break;
    case 4:
      global.input.set('Загрузить аптечки', 'Введите кол-во аптечек', 4, 'loadmedkits');
      global.input.open();
      break;
  }
});
mp.events.add('matsU', (act) => { // unload
  global.menuClose();
  switch (act) {
    case 1:
      global.input.set('Выгрузить маты', 'Введите кол-во матов', 4, 'unloadmats');
      global.input.open();
      break;
    case 2:
      global.input.set('Выгрузить маты', 'Введите кол-во матов', 4, 'unloadmats');
      global.input.open();
      break;
    case 3:
      global.input.set('Выгрузить наркоту', 'Введите кол-во наркоты', 4, 'unloaddrugs');
      global.input.open();
      break;
    case 4:
      global.input.set('Выгрузить аптечки', 'Введите кол-во аптечек', 4, 'unloadmedkits');
      global.input.open();
      break;
  }
});
// BODY SEARCH //
/* mp.keys.bind(0x78, false, function () { // F9
 mp.events.call('bsearchOpen', '["FirstName LastName",["Deser Eaagle"],["Water","Keys for Car"]]');
 }); */
mp.events.add('bsearch', (act) => {
  global.menuClose();
  switch (act) {
    case 1:
      global.anyEvents.SendServer(() => mp.events.callRemote('pSelected', global.circleEntity, 'Посмотреть лицензии'));
      break;
    case 2:
      global.anyEvents.SendServer(() => mp.events.callRemote('pSelected', global.circleEntity, 'Посмотреть документы при обыске'));
      break;
    case 3:
      global.anyEvents.SendServer(() => mp.events.callRemote('pSelected', global.circleEntity, 'Изъять нелегал'));
      break;
  }
  global.menu.execute('bsearch.active=false');
  global.menuClose();
});
mp.events.add('bsearchOpen', (data) => {
  if (global.menuCheck()) return;
  global.menuOpen();
  global.menu.execute('bsearch.active=true');
  global.menu.execute(`bsearch.set('${data}')`);
});
// BODY CUSTOM //
// function getCameraOffset(pos, angle, dist) {
//   // mp.gui.chat.push(`Sin: ${Math.sin(angle)} | Cos: ${Math.cos(angle)}`);
//
//   angle *= 0.0174533;
//
//   pos.y += dist * Math.sin(angle);
//   pos.x += dist * Math.cos(angle);
//
//   // mp.gui.chat.push(`X: ${pos.x} | Y: ${pos.y}`);
//
//   return pos;
// }

const cameraRotator = require('cef/resources/js/helpers/cameraRotatorNew.js');
// const cameraRotator = camera360.Rotator();

const camPos = new mp.Vector3(-1500.3770751953125, -2998.169677734375, -81.15196990966797);
let camera = null;

function createCam(vPos = false, name = "default", fov = true) {
  camera = mp.cameras.new(name);

  let pos = camPos;

  if (vPos) {
    pos = vPos;
  }

  if (fov) {
    cameraRotator.start(camera, pos, pos, new mp.Vector3(0, 10, 0), 360);
    cameraRotator.setXBound(-360, 360);
  } else {
    cameraRotator.start(camera, pos, pos, new mp.Vector3(0, 5, 0));
  }

  // // // mp.console.logInfo("pos: -> " + pos, true, true);

  mp.game.cam.renderScriptCams(true, false, 0, false, false);
}

function destroyCamera() {
  try {
    cameraRotator.stop();
    cameraRotator.reset();
    if (camera) {
      camera.destroy();
      camera = null;
    }
  } catch (e) {
    console.log('destroyCamera: ', e);
  }

  // // // mp.console.logInfo("cam destroyed", true, true);

  mp.game.cam.renderScriptCams(false, true, 0, true, true);
}

mp.events.add('createPhotoCar', (x, y, z) => {
  createCam(new mp.Vector3(x, y, z), 'photo', false); // координаты камеры и ротация
});

mp.events.add('removePhotoCar', () => {
  destroyCamera();
});


// PETSHOP

let petModels = null;
let petHashes = null;

const pet = {
  model: null,
  entity: null,
  dimension: 0,
};

function setPet(type, jsonstr) {
  global.menu.execute(`petshop.${type}=${jsonstr}`);
}
mp.events.add('petshop', (act, value) => {
  switch (act) {
    case 'model':
      pet.model = petModels[value];
      if (pet.entity != null) {
        pet.entity.destroy();
        pet.entity = mp.peds.new(petHashes[value], new mp.Vector3(-758.2859, 320.9569, 175.2784), 218.8, pet.dimension);
      }
      break;
  }
});
mp.events.add('buyPet', () => {
  if (new Date().getTime() - global.lastCheck < 50) return;
  global.lastCheck = new Date().getTime();

  global.menuClose();
  global.menu.execute('petshop.active=0');

  global.anyEvents.SendServer(() => mp.events.callRemote('petshopBuy', pet.model));

  if (pet.entity == null) return;
  pet.entity.destroy();
  pet.entity = null;
});
mp.events.add('closePetshop', () => {
  if (new Date().getTime() - global.lastCheck < 50) return;
  global.lastCheck = new Date().getTime();
  global.menuClose();
  global.menu.execute('petshop.active=0');

  mp.events.callRemote('petshopCancel');

  if (pet.entity == null) return;
  pet.entity.destroy();
  pet.entity = null;
});
mp.events.add('openPetshop', (models, hashes, prices, dim) => {
  if (global.menuCheck()) return;

  petModels = JSON.parse(models);
  petHashes = JSON.parse(hashes);

  setPet('models', models);
  setPet('hashes', hashes);
  setPet('prices', prices);

  pet.entity = mp.peds.new(petHashes[0], new mp.Vector3(-758.2859, 320.9569, 175.2784), 218.8, dim);
  pet.dimension = dim;
  localplayer.setRotation(0, 0, 0, 2, true);
  pet.model = petModels[0];

  global.menuOpen();
  global.menu.execute('petshop.active=true');

  cam = mp.cameras.new('default', new mp.Vector3(-755.5227, 320.0132, 177.302), new mp.Vector3(0, 0, 0), 50);
  cam.pointAtCoord(-758.2859, 320.9569, 175.7484);
  cam.setActive(true);
  mp.game.cam.renderScriptCams(true, false, 0, true, false);
});
//

// WEAPON SHOP //
/* mp.keys.bind(0x78, false, function () { // F9
 mp.events.call('openWShop', 0, '[[0,1,0,1,0,1,0]]');
 }); */
const wshop = {
  lid: -1,
  tab: 0,
  data: [],
};
mp.events.add('wshop', (act, value, sub) => {
  if (new Date().getTime() - global.lastCheck < 50) return;
  global.lastCheck = new Date().getTime();
  switch (act) {
    case 'cat':
      if (value == 5) return;
      wshop.tab = value;
      global.menu.execute(`wshop.set(${value},'${JSON.stringify(wshop.data[value])}')`);
      break;
    case 'buy':
      global.anyEvents.SendServer(() => mp.events.callRemote('wshop', wshop.tab, value));
      break;
    case 'rangebuy':
      global.anyEvents.SendServer(() => mp.events.callRemote('wshopammo', value, sub));
      break;
  }
});
mp.events.add('closeWShop', () => {
  global.menuClose();
  wshop.tab = 0;
});
mp.events.add('openWShop', (id, json) => {
  if (global.menuCheck()) return;
  global.menuOpen();
  if (id !== wshop.lid) wshop.data = JSON.parse(json);
  global.menu.execute(`wshop.set(0,'${JSON.stringify(wshop.data[0])}')`);
  global.menu.execute('wshop.active=1');
  wshop.lid = id;
});


var playerLastPos = null;
var playerLastHeading = null;

function startCameraRotator(camVal) {
    try {
        bodyCamStart = localplayer.position;
        var camValues = camVal ? camVal : {Angle: localplayer.getRotation(2).z + 90, Dist: 1, Height: 0.6};
        var pos = global.init.getCameraOffset(new mp.Vector3(bodyCamStart.x, bodyCamStart.y, bodyCamStart.z + camValues.Height), camValues.Angle, camValues.Dist);
        bodyCam = mp.cameras.new('default', pos, new mp.Vector3(0, 0, 0), 50);
        bodyCam.pointAtCoord(bodyCamStart.x, bodyCamStart.y, bodyCamStart.z + camValues.Height);
        bodyCam.setActive(true);

        cameraRotator.start(bodyCam, bodyCamStart, bodyCamStart, new mp.Vector3(0, 1.5, 1), localplayer.getHeading());
        cameraRotator.setZBound(-0.8, 1.8);
        cameraRotator.setZUpMultipler(5);
        cameraRotator.setType(1);
        cameraRotator.pause(false);

        mp.game1.cam.renderScriptCams(true, false, 500, true, false);
    } catch (e) {
        logger.error(e);
    }
}

var styleIndexes =  {
    // style, color
        hats: 0,
        tops: 1,
        legs: 2,
        underwears: 3,
        feets: 4,
        gloves: 5,
        watches: 6,
        glasses: 7
}

var fracClothes = {
    type: 0,
    style: 0,
    color: 0,
    colors: [0, 0, 0],
    styles: [
        [-1, -1],
        [-1, -1],
        [-1, -1],
        [-1, -1],
        [-1, -1],
        [-1, -1],
        [-1, -1],
        [-1, -1]
    ]
    // localplayer.setComponentVariation(1, clothesEmpty[gender][1], 0, 0);
    // localplayer.setComponentVariation(3, clothesEmpty[gender][3], 0, 0);
    // localplayer.setComponentVariation(4, clothesEmpty[gender][4], 0, 0);
    // localplayer.setComponentVariation(5, clothesEmpty[gender][5], 0, 0);
    // localplayer.setComponentVariation(6, clothesEmpty[gender][6], 0, 0);
    // localplayer.setComponentVariation(7, clothesEmpty[gender][7], 0, 0);
    // localplayer.setComponentVariation(8, clothesEmpty[gender][8], 0, 0);
    // localplayer.setComponentVariation(9, clothesEmpty[gender][9], 0, 0);
    // localplayer.setComponentVariation(10, clothesEmpty[gender][10], 0, 0);
    // localplayer.setComponentVariation(11, clothesEmpty[gender][11], 0, 0);
};


var activeCats = [];
var activeUndershirts = [];

mp.events.add('fractionCatsSet', (cats) => {
    try {
        activeCats = JSON.parse(cats);
    } catch (e) {
        logger.error(e);
    }
});

mp.events.add('saveCurFractionStyle', () => {
    mp.events.callRemote('saveCurFractionStyle', JSON.stringify(activeCustomFracClothes));
});

mp.events.add('fractionClothes', (act, value, subdata) => {
    try {
        const gender = (localplayer.getVariable("GENDER")) ? 1 : 0;

        switch (act) {
            case "clear":
                switch (value) {
                    case -1:
                        localplayer.clearProp(0);
                        localplayer.clearProp(1);
                        localplayer.clearProp(2);
                        localplayer.clearProp(6);
                        localplayer.clearProp(7);

                        localplayer.setComponentVariation(1, clothesEmpty[gender][1], 0, 0);
                        localplayer.setComponentVariation(3, clothesEmpty[gender][3], 0, 0);
                        localplayer.setComponentVariation(4, clothesEmpty[gender][4], 0, 0);
                        localplayer.setComponentVariation(5, clothesEmpty[gender][5], 0, 0);
                        localplayer.setComponentVariation(6, clothesEmpty[gender][6], 0, 0);
                        localplayer.setComponentVariation(7, clothesEmpty[gender][7], 0, 0);
                        localplayer.setComponentVariation(8, clothesEmpty[gender][8], 0, 0);
                        localplayer.setComponentVariation(9, clothesEmpty[gender][9], 0, 0);
                        localplayer.setComponentVariation(10, clothesEmpty[gender][10], 0, 0);
                        localplayer.setComponentVariation(11, clothesEmpty[gender][11], 0, 0);
                        return;
                    case 0:
                        localplayer.clearProp(0);
                        activeCustomFracClothes.Hat = null;
                        return;
                    case 1:
                        localplayer.setComponentVariation(11, clothesEmpty[gender][11], 0, 0);
                        localplayer.setComponentVariation(3, clothesEmpty[gender][3], 0, 0);
                        localplayer.setComponentVariation(8, clothesEmpty[gender][8], 0, 0);

                        if (activeCustomFracClothes.Undershirts) {
                            localplayer.setComponentVariation(11, activeCustomFracClothes.Undershirts, 0, 0);
                            localplayer.setComponentVariation(3, validTorsos[gender][activeCustomFracClothes.Undershirts], 0, 0);
                        }

                        activeCustomFracClothes.Top = clothesEmpty[gender][11];
                        return;
                    case 2:
                        localplayer.setComponentVariation(11, clothesEmpty[gender][11], 0, 0);
                        localplayer.setComponentVariation(3, clothesEmpty[gender][3], 0, 0);
                        activeCustomFracClothes.Undershirts = clothesEmpty[gender][11];
                        return;
                    case 3:
                        localplayer.setComponentVariation(4, clothesEmpty[gender][4], 0, 0);
                        activeCustomFracClothes.Leg = clothesEmpty[gender][4];
                        return;
                    case 4:
                        localplayer.setComponentVariation(6, clothesEmpty[gender][6], 0, 0);
                        activeCustomFracClothes.Feet = clothesEmpty[gender][6];
                        return;
                    case 5:
                        let top = activeCustomFracClothes.Top || 15;
                        localplayer.setComponentVariation(3, validTorsos[gender][top], 0, 0);
                        activeCustomFracClothes.Gloves = validTorsos[gender][top];
                        return;
                    case 6:
                        localplayer.clearProp(6);
                        activeCustomFracClothes.Watches = null;
                        return;
                    case 7:
                        localplayer.clearProp(1);
                        activeCustomFracClothes.Glasses = null;
                        return;
                }
                break;
            case "style":
                switch (subdata) {
                    case 0:
                            fracClothes.style = CustomFractionClothes[gender][fracType].Hat[value];
                            activeCustomFracClothes.Hat = fracClothes.style;
                            localplayer.setPropIndex(0, fracClothes.style, 0, true);
                        return;
                    case 1:
                            fracClothes.style = CustomFractionClothes[gender][fracType].Top[value];
                            activeCustomFracClothes.Top = fracClothes.style;

                            // if (!activeCats.includes(1)) {
                            //     activeUndershirts = CustomFractionClothes[gender][fracType].Undershirts;
                            // }
                            // else {
                            //     let undershirts = [];
                            //     CustomFractionClothes[gender][fracType].Undershirts.forEach(u => {
                            //         let topType = global.clothesTops[gender].find(t => t.Variation == fracClothes.style).Type;
                            //         if (!global.clothesUnderwears[gender][u].UndershirtIDs[topType]){
                            //             undershirts.push(global.clothesUnderwears[gender][u].UndershirtIDs[topType]);
                            //         }
                            //     });
                            //     activeUndershirts = undershirts;
                            // }

                            mp.events.call('stc', JSON.stringify(activeUndershirts));

                            localplayer.setComponentVariation(11, fracClothes.style, 0, 0);
                            localplayer.setComponentVariation(3, validTorsos[gender][fracClothes.style], 0, 0);
                        return;
                    case 2:
                            if (activeCustomFracClothes.Top == global.clothesEmpty[gender][11]) {
                                fracClothes.style = CustomFractionClothes[gender][fracType].Undershirts[value];
                                activeCustomFracClothes.Undershirts = fracClothes.style;

                                localplayer.setComponentVariation(11, fracClothes.style, fracClothes.color, 0);
                                localplayer.setComponentVariation(3, validTorsos[gender][fracClothes.style], 0, 0);
                            } else {
                                let topType = global.clothesTops[gender].find(t => t.Variation == activeCustomFracClothes.Top).Type;
                                fracClothes.style = global.fractionClothes['underwears'][gender][value].UndershirtIDs[topType];
                                activeCustomFracClothes.Undershirts = fracClothes.style;

                                mp.events.call('stc', `topType: ${topType} | style: ${fracClothes.styles[styleIndexes['tops']][0]} | curStyle: ${fracClothes.style}`);

                                localplayer.setComponentVariation(8, fracClothes.style, 0, 0);
                                // localplayer.setComponentVariation(3, validTorsos[gender][fracClothes.style], 0, 0);
                            }
                        return;
                    case 3:
                            fracClothes.style = CustomFractionClothes[gender][fracType].Leg[value];
                            activeCustomFracClothes.Leg = fracClothes.style;
                            localplayer.setComponentVariation(4, fracClothes.style, 0, 0);
                        return;
                    case 4:
                            fracClothes.style = CustomFractionClothes[gender][fracType].Feet[value];
                            activeCustomFracClothes.Feet = fracClothes.style;
                            localplayer.setComponentVariation(6, fracClothes.style, 0, 0);
                        return;
                    case 5:
                            let top = activeCustomFracClothes.Top || 45;
                            fracClothes.style = CustomFractionClothes[gender][fracType].Gloves[value];
                            activeCustomFracClothes.Gloves = fracClothes.style;
                            localplayer.setComponentVariation(3, correctGloves[gender][fracClothes.style][validTorsos[gender][top]], 0, 0);
                        return;
                    case 6:
                            fracClothes.style = CustomFractionClothes[gender][fracType].Watches[value];
                            activeCustomFracClothes.Watches = fracClothes.style;
                            localplayer.setPropIndex(6, fracClothes.style, 0, true);
                        return;
                    case 7:
                            fracClothes.style = CustomFractionClothes[gender][fracType].Glasses[value];
                            activeCustomFracClothes.Glasses = fracClothes.style;
                            localplayer.setPropIndex(1, fracClothes.style, 0, true);
                        return;
                }
                break;
        }
    } catch (e) {
        logger.error(e);
    }
});

// mp.keys.bind(Keys.VK_NUMPAD8, false, () => {
//     if (!loggedin || localplayer.getVariable('IS_ADMIN') !== true) return;
//     mp.events.call('openFractionClothes', 6, 10);
// });

var fracType = -1;
var fracLvl = -1;
var curFracClothes = -1;

class ComponentItem
    {
        constructor(variation, texture)
        {
            this.Variation = variation;
            this.Texture = texture;
        }
    }

class ClothesData
    {
        constructor(obj = {})
        {
            this.Mask = obj.mask || new ComponentItem(0, 0);
            this.Gloves = obj.gloves || new ComponentItem(0, 0);
            this.Torso = obj.torso || new ComponentItem(15, 0);
            this.Leg = obj.leg || new ComponentItem(21, 0);
            this.Bag = obj.bag || new ComponentItem(0, 0);
            const gender = (localplayer.getVariable("GENDER")) ? 1 : 0;
            this.Feet = obj.feet || new ComponentItem(gender? 34 : 35, 0);
            this.Accessory = obj.accessory || new ComponentItem(0, 0);
            this.Undershit = obj.undershit || new ComponentItem(-1, 0);
            this.Bodyarmor = obj.bodyarmor || new ComponentItem(0, 0);
            this.Decals = obj.decals || new ComponentItem(0, 0);
            this.Top = obj.top || new ComponentItem(15, 0);
        }
    }

    class AccessoryData
    {
        constructor(obj = {})
        {
            this.Hat = obj.hat || new ComponentItem(-1, 0);
            this.Glasses = obj.glasses || new ComponentItem(-1, 0);
            this.Ear = obj.ear || new ComponentItem(-1, 0);
            this.Watches = obj.watches || new ComponentItem(-1, 0);
            this.Bracelets = obj.bracelets || new ComponentItem(-1, 0);
        }
    }

var defaultFractionClothes = {
    1: {
        // fraction
        1: [
            {
                id: 0,
                name: 'Комплект 1',
                minRank: 1,
                clothes: (new ClothesData()),
                accessory: (new AccessoryData())
            },
        ],
        2: [
            {
                id: 0,
                name: 'Комплект 1',
                minRank: 1,
                clothes: (new ClothesData()),
                accessory: (new AccessoryData())
            },
        ],
        3: [
            {
                id: 0,
                name: 'Комплект 1',
                minRank: 1,
                clothes: (new ClothesData()),
                accessory: (new AccessoryData())
            },
        ],
        4: [
            {
                id: 0,
                name: 'Комплект 1',
                minRank: 1,
                clothes: (new ClothesData()),
                accessory: (new AccessoryData())
            },
        ],
        5: [
            {
                id: 0,
                name: 'Комплект 1',
                minRank: 1,
                clothes: (new ClothesData()),
                accessory: (new AccessoryData())
            },
        ],
        6: [
            {
                id: 0,
                name: 'Комплект 1',
                minRank: 1,
                clothes: (new ClothesData({top: new ComponentItem(242, 2), leg: new ComponentItem(25,0), undershit: new ComponentItem(129, 0),
                    feet: new ComponentItem(54,0)})),
                accessory: (new AccessoryData())
            },
            {
                id: 1,
                name: 'Комплект 2',
                minRank: 2,
                clothes: (new ClothesData({top: new ComponentItem(120, 11), leg: new ComponentItem(25,2), undershit: new ComponentItem(6, 0),
                    feet: new ComponentItem(10,0)})),
                accessory: (new AccessoryData())
            },
            {
                id: 2,
                name: 'Комплект 3',
                minRank: 3,
                clothes: (new ClothesData({top: new ComponentItem(120, 11), leg: new ComponentItem(25,2), undershit: new ComponentItem(6, 0),
                    feet: new ComponentItem(10,0)})),
                accessory: (new AccessoryData())
            }
        ],
        7: [
            {
                id: 0,
                name: 'Комплект 1',
                minRank: 1,
                clothes: (new ClothesData()),
                accessory: (new AccessoryData())
            },
        ],
        8: [
            {
                id: 0,
                name: 'Комплект 1',
                minRank: 1,
                clothes: (new ClothesData()),
                accessory: (new AccessoryData())
            },
        ],
        9: [
            {
                id: 0,
                name: 'Комплект 1',
                minRank: 1,
                clothes: (new ClothesData()),
                accessory: (new AccessoryData())
            },
        ],
        10: [
            {
                id: 0,
                name: 'Комплект 1',
                minRank: 1,
                clothes: (new ClothesData()),
                accessory: (new AccessoryData())
            },
        ],
        11: [
            {
                id: 0,
                name: 'Комплект 1',
                minRank: 1,
                clothes: (new ClothesData()),
                accessory: (new AccessoryData())
            },
        ],
        12: [
            {
                id: 0,
                name: 'Комплект 1',
                minRank: 1,
                clothes: (new ClothesData()),
                accessory: (new AccessoryData())
            },
        ],
        13: [
            {
                id: 0,
                name: 'Комплект 1',
                minRank: 1,
                clothes: (new ClothesData()),
                accessory: (new AccessoryData())
            },
        ],
        14: [
            {
                id: 0,
                name: 'Комплект 1',
                minRank: 1,
                clothes: (new ClothesData()),
                accessory: (new AccessoryData())
            },
        ],
        15: [
            {
                id: 0,
                name: 'Комплект 1',
                minRank: 1,
                clothes: (new ClothesData()),
                accessory: (new AccessoryData())
            },
        ],
        16: [
            {
                id: 0,
                name: 'Комплект 1',
                minRank: 1,
                clothes: (new ClothesData()),
                accessory: (new AccessoryData())
            },
        ],
        17: [
            {
                id: 0,
                name: 'Комплект 1',
                minRank: 1,
                clothes: (new ClothesData()),
                accessory: (new AccessoryData())
            },
        ],
        18: [
            {
                id: 0,
                name: 'Комплект 1',
                minRank: 1,
                clothes: (new ClothesData()),
                accessory: (new AccessoryData())
            },
        ],
    },
    0: {
        1: [
            {
                id: 0,
                name: 'Комплект 1',
                minRank: 1,
                clothes: (new ClothesData()),
                accessory: (new AccessoryData())
            },
        ],
        2: [
            {
                id: 0,
                name: 'Комплект 1',
                minRank: 1,
                clothes: (new ClothesData()),
                accessory: (new AccessoryData())
            },
        ],
        3: [
            {
                id: 0,
                name: 'Комплект 1',
                minRank: 1,
                clothes: (new ClothesData()),
                accessory: (new AccessoryData())
            },
        ],
        4: [
            {
                id: 0,
                name: 'Комплект 1',
                minRank: 1,
                clothes: (new ClothesData()),
                accessory: (new AccessoryData())
            },
        ],
        5: [
            {
                id: 0,
                name: 'Комплект 1',
                minRank: 1,
                clothes: (new ClothesData()),
                accessory: (new AccessoryData())
            },
        ],
        6: [
            {
                id: 0,
                name: 'Комплект 1',
                minRank: 1,
                clothes: (new ClothesData()),
                accessory: (new AccessoryData())
            },
            {
                id: 1,
                name: 'Комплект 2',
                minRank: 2,
                clothes: (new ClothesData()),
                accessory: (new AccessoryData())
            },
            {
                id: 2,
                name: 'Комплект 3',
                minRank: 3,
                clothes: (new ClothesData()),
                accessory: (new AccessoryData())
            }
        ],
        7: [
            {
                id: 0,
                name: 'Комплект 1',
                minRank: 1,
                clothes: (new ClothesData()),
                accessory: (new AccessoryData())
            },
        ],
        8: [
            {
                id: 0,
                name: 'Комплект 1',
                minRank: 1,
                clothes: (new ClothesData()),
                accessory: (new AccessoryData())
            },
        ],
        9: [
            {
                id: 0,
                name: 'Комплект 1',
                minRank: 1,
                clothes: (new ClothesData()),
                accessory: (new AccessoryData())
            },
        ],
        10: [
            {
                id: 0,
                name: 'Комплект 1',
                minRank: 1,
                clothes: (new ClothesData()),
                accessory: (new AccessoryData())
            },
        ],
        11: [
            {
                id: 0,
                name: 'Комплект 1',
                minRank: 1,
                clothes: (new ClothesData()),
                accessory: (new AccessoryData())
            },
        ],
        12: [
            {
                id: 0,
                name: 'Комплект 1',
                minRank: 1,
                clothes: (new ClothesData()),
                accessory: (new AccessoryData())
            },
        ],
        13: [
            {
                id: 0,
                name: 'Комплект 1',
                minRank: 1,
                clothes: (new ClothesData()),
                accessory: (new AccessoryData())
            },
        ],
        14: [
            {
                id: 0,
                name: 'Комплект 1',
                minRank: 1,
                clothes: (new ClothesData()),
                accessory: (new AccessoryData())
            },
        ],
        15: [
            {
                id: 0,
                name: 'Комплект 1',
                minRank: 1,
                clothes: (new ClothesData()),
                accessory: (new AccessoryData())
            },
        ],
        16: [
            {
                id: 0,
                name: 'Комплект 1',
                minRank: 1,
                clothes: (new ClothesData()),
                accessory: (new AccessoryData())
            },
        ],
        17: [
            {
                id: 0,
                name: 'Комплект 1',
                minRank: 1,
                clothes: (new ClothesData()),
                accessory: (new AccessoryData())
            },
        ],
        18: [
            {
                id: 0,
                name: 'Комплект 1',
                minRank: 1,
                clothes: (new ClothesData()),
                accessory: (new AccessoryData())
            },
        ],
    }
};

var activeCustomFracClothes = {
    Mask: null,
    Gloves: null,
    Torso: null,
    Leg: null,
    Bag: null,
    Feet: null,
    Accessory: null,
    Undershirts: null,
    Bodyarmor: null,
    Decals: null,
    Top: null,
    Hat: null,
    Glasses: null,
    Ear: null,
    Watches: null,
    Bracelets: null
};

var CustomFractionClothes = { // gender, fractionID, clothes/accessory
    1: {
        6: {
            Mask: [-1],
            Gloves: [4, 5, 6],
            Torso: [-1],
            Leg: [1, 12, 13, 23],
            Bag: [-1],
            Feet: [1, 12, 14, 15],
            Accessory: [-1],
            Undershirts: [3, 11, 12],
            Bodyarmor: [-1],
            Decals: [-1],
            Top: [3, 23, 24, 27],
            Hat: [2, 4, 12, 13],
            Glasses: [-1],
            Ear: [-1],
            Watches: [-1],
            Bracelets: [-1],
        }
    },
    0: {
        6: {
            Mask: [-1],
            Gloves: [4, 5, 6],
            Torso: [-1],
            Leg: [1, 12, 13, 23],
            Bag: [-1],
            Feet: [1, 12, 14, 15],
            Accessory: [-1],
            Undershirts: [3, 11, 12],
            Bodyarmor: [-1],
            Decals: [-1],
            Top: [3, 23, 24, 27],
            Hat: [2, 4, 12, 13],
            Glasses: [-1],
            Ear: [-1],
            Watches: [-1],
            Bracelets: [-1],
        }
    }
};

// var useTop = false;

// mp.events.add('', () => {
//     const gender = (localplayer.getVariable("GENDER")) ? 1 : 0;
//     if (fractionClothes.activeCats.in )//!useTop)
//         Undershirts = CustomFractionClothes[gender][fracType].Undershirts;
//     else {
//         let undershirts = [];
//         CustomFractionClothes[gender][fracType].Undershirts.forEach(u => {
//             let topType = global.clothesTops.find(t => t.Variation == /* текущая верхняя одежда */).Type;
//             if (global.clothesUnderwears[gender][u].UndershirtIDs[topType] != undefined){
//                 undershirts.push(global.clothesUnderwears[gender][u].UndershirtIDs[topType]);
//             }
//         });
//     }
// })

// function UpdateClothesPool(){

// }

function GetCorrectTorsos(topId) {
    try {
        return global.validTorsos[gender ? "1" : "0"][topId]
    } catch (e) {
        logger.error(e);
    }
}

mp.events.add('setDefaultFractionClothes', (id) => {
    try {
        const gender = (localplayer.getVariable("GENDER")) ? 1 : 0;

        clearClothes();

        const item = defaultFractionClothes[gender][fracType].find(fracItem => fracItem.id === id)

        // item.clothes.forEach(value => {
        //     localplayer.setComponentVariation(value[0], value[1], value[2], 0);
        // });


        localplayer.setComponentVariation(1, item.clothes.Mask.Variation, item.clothes.Mask.Texture, 2);
        localplayer.setComponentVariation(3, GetCorrectTorsos(item.clothes.Top.Variation), 0, 2);
        localplayer.setComponentVariation(4, item.clothes.Leg.Variation, item.clothes.Leg.Texture, 2);
        localplayer.setComponentVariation(6, item.clothes.Feet.Variation, item.clothes.Feet.Texture, 2);
        localplayer.setComponentVariation(7, item.clothes.Accessory.Variation, item.clothes.Accessory.Texture, 2);
        localplayer.setComponentVariation(8, item.clothes.Undershit.Variation, item.clothes.Undershit.Texture, 2);
        localplayer.setComponentVariation(10, item.clothes.Decals.Variation, item.clothes.Decals.Texture, 2);
        localplayer.setComponentVariation(11, item.clothes.Top.Variation, item.clothes.Top.Texture, 2);


        // this.Mask = obj.mask || new ComponentItem(0, 0);
        // this.Gloves = obj.gloves || new ComponentItem(0, 0);
        // this.Torso = obj.torso || new ComponentItem(15, 0);
        // this.Leg = obj.leg || new ComponentItem(21, 0);
        // this.Bag = obj.bag || new ComponentItem(0, 0);
        // const gender = (localplayer.getVariable("GENDER")) ? 1 : 0;
        // this.Feet = obj.feet || new ComponentItem(gender? 34 : 35, 0);
        // this.Accessory = obj.accessory || new ComponentItem(0, 0);
        // this.Undershit = obj.undershit || new ComponentItem(-1, 0);
        // this.Bodyarmor = obj.bodyarmor || new ComponentItem(0, 0);
        // this.Decals = obj.decals || new ComponentItem(0, 0);
        // this.Top = obj.top || new ComponentItem(15, 0);


        //
        /*if (item.clothes.Top.Variation == global.clothesEmpty[11]){
            localplayer.setComponentVariation(8, global.clothesUnderwears[gender][item.clothes.Undershit.Variation].Top, item.clothes.Undershit.Texture, 2);
        }
        else {
            localplayer.setComponentVariation(11, item.clothes.Top.Variation, item.clothes.Top.Texture, 2);
            if (
                global.clothesTops[gender].find(t => t.Variation == item.clothes.Top.Variation) != undefined &&
                global.clothesTops[gender].find(t => t.Variation == item.clothes.Top.Variation).Type != undefined
            ){ // надо сделать if (!global.clothesUnderwears[gender].has[item.clothes.Undershit.Variation])
                let topType = global.clothesTops[gender].find(t => t.Variation == item.clothes.Top.Variation).Type;
                localplayer.setComponentVariation(8, global.clothesUnderwears[gender][item.clothes.Undershit.Variation].UndershirtIDs[topType], item.clothes.Undershit.Texture, 2);
            }
            else
                localplayer.setComponentVariation(8, item.clothes.Undershit.Variation, item.clothes.Undershit.Texture, 2);
        }
        // localplayer.setComponentVariation(11, item.clothes.Top.Variation, item.clothes.Top.Texture, 2);
        // localplayer.setComponentVariation(8, item.clothes.Undershit.Variation, item.clothes.Undershit.Texture, 2);
        //
        localplayer.setComponentVariation(3, global.validTorsos[gender][item.clothes.Top.Variation], 0, 2);
        //

        localplayer.setPropIndex(0, item.accessory.Hat.Variation, item.accessory.Hat.Texture, true);
        localplayer.setPropIndex(1, item.accessory.Glasses.Variation, item.accessory.Glasses.Texture, true);
        localplayer.setPropIndex(6, item.accessory.Watches.Variation, item.accessory.Watches.Texture, true);*/

        // item.accessory.forEach(value => {
        //     localplayer.setPropIndex(value[0], value[1], value[2], true);
        // });

        curFracClothes = item;
    } catch (e) {
        logger.error(e);
    }
});

mp.events.add('openFractionClothes', (type, lvl) => {
    try {
        if (!global.loggedin || global.menuCheck() || cuffed || localplayer.getVariable('InDeath') === true) return;

        startCameraRotator();

        fracType = type;
        fracLvl = lvl;
        curFracClothes = -1;

        const gender = (localplayer.getVariable("GENDER")) ? 1 : 0;

        var defStyles = defaultFractionClothes[gender][fracType].filter(item => item.minRank <= fracLvl);

        setFractionClothes('defStyles', JSON.stringify(defStyles));

        const styles = [
            CustomFractionClothes[gender][fracType].Hat,
            CustomFractionClothes[gender][fracType].Top,
            CustomFractionClothes[gender][fracType].Undershirts,
            CustomFractionClothes[gender][fracType].Leg,
            CustomFractionClothes[gender][fracType].Feet,
            CustomFractionClothes[gender][fracType].Gloves,
            CustomFractionClothes[gender][fracType].Watches
        ];

        global.policeGarage.execute(`fractionClothes.styles=${JSON.stringify(styles)}`);

        global.menuOpen();
        mp.events.call('showHUD', false);
        global.policeGarage.execute(`fractionClothes.active=true`);
    } catch (e) {
        logger.error(e);
    }
});

function setFractionClothes(type, jsonstr) {
    try {
        global.policeGarage.execute(`fractionClothes.${type}=${jsonstr}`);
        if (type == 'colors') global.policeGarage.execute(`fractionClothes.indexC=0`);
        else if (type == 'styles') global.policeGarage.execute(`fractionClothes.indexS=0`);
    } catch (e) {
        logger.error(e);
    }
}

var clothesPremium = false;

// WEAPON CRAFT //
/* mp.keys.bind(0x78, false, function () { // F9
 mp.events.call('openWCraft', 0, '[[0,1,0,1,0,1,0]]');
 }); */

mp.events.add('playFocusSound', global.init.playFocusSound);
mp.events.add('playBackSound', global.init.playBackSound);
mp.events.add('playSelectSound', global.init.playSelectSound);

mp.events.add('playFrontEndSound', (soundName, soundSetName) => {
  try {
    mp.game1.audio.playSoundFrontend(-1, soundName, soundSetName, true);
  } catch (e) {
    logger.error(e);
  }
});

// function playFocusSound() {
//   mp.events.call('playFrontEndSound', "NAV_UP_DOWN", "HUD_FRONTEND_DEFAULT_SOUNDSET");
// }
//
// function playBackSound() {
//   mp.events.call('playFrontEndSound', "CANCEL", "HUD_FRONTEND_DEFAULT_SOUNDSET");
// }
//
// function playSelectSound() {
//   mp.events.call('playFrontEndSound', "SELECT", "HUD_FRONTEND_DEFAULT_SOUNDSET");
// }
function offsetPosition(pos, rot, distance) {
  return new mp.Vector3(pos.x + Math.sin(-rot * Math.PI / 180) * distance, pos.y + Math.cos(-rot * Math.PI / 180) * distance, pos.z);
}

function offsetPositionXYZ(pos, rot, distance) {
  return { x: pos.x + Math.sin(-rot * Math.PI / 180) * distance, y: pos.y + Math.cos(-rot * Math.PI / 180) * distance, z: pos.z };
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

let NPC_Cam;

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
});

function CamToNPC(position, rotation) {
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

/* FROM CITY */
global.infoCEF = mp.browsers.new('package://interfaces/ui/Info/index.html');
global.infoOpen = false;

mp.events.add('browserDomReady', (browser) => {
   try {
       if (browser === infoCEF) {
           mp.events.add({
               'info:open': (title, info, btns) => {
                   global.infoCEF.execute(`info.init('${title}', '${info}', '${btns}')`);
                   global.infoOpen = true;
                   global.menuOpenIfNotOpened();
               },
               'info:close': () => {
                   global.infoCEF.execute(`info.hide()`);
                   global.infoOpen = false;
                   global.menuCloseIfNotOpened();
               }
           });
       }
   } catch (e) {
       logger.error(e);
   }
});

mp.events.add('events.callRemote', (trigger, ...data) => {
  mp.events.callRemote(trigger, ...data);
});


global.listCEF = null;
global.listOpen = false;

mp.events.add(
    {
        'list:open': (items, title) => {
            if (!global.loggedin || global.menuCheck() || cuffed || localplayer.getVariable('InDeath') === true) return;

            if (global.listCEF === null)
                global.listCEF = mp.browsers.new('package://interfaces/ui/List/index.html');

            global.menuOpen();
            global.listCEF.execute(`list.init('${items}', '${title}')`);
            global.listOpen = true;
        },

        'list:close': () => {
            global.listCEF.execute(`list.hide()`);
            global.menuClose();
            global.listOpen = false;
        }
    }
);

global.inputCEF = mp.browsers.new('package://interfaces/ui/Input/index.html');
let inputAction = '';

mp.events.add({
    'openInputv2': (title, info, plholder, len, value, action = '') => {
        inputAction = action;
        global.inputCEF.execute(`inputv2.set('${title}', ${info}, '${plholder}', ${len}, '${value}');`);
        global.inputCEF.execute('inputv2.active=true');
        global.menuOpenIfNotOpened();
        mp.events.call('startScreenEffect', "MenuMGHeistIn", 1, true);
    },
    'inputv2': (value) => {
        switch (inputAction) {
            case "CONFIRM_CHAR_DELETE":
                global.auth.execute(`slots.confirmCharDelete('${value}')`);
                break;
            default:
                if(value != ''){
                  mp.events.callRemote('inputCallback', '', value);
                }
                break;
        }
        global.inputCEF.execute('inputv2.active=false');
        mp.events.call('stopScreenEffect', "MenuMGHeistIn");
        global.menuCloseIfNotOpened();
    }
});


