global.new_menu = mp.browsers.new('package://interfaces/ui_new/Oscar/index.html');
global.oscar2Menu = mp.browsers.new('package://interfaces/ui_new/Oscar2/index.html');
global.festMenu = mp.browsers.new('package://interfaces/ui_new/FeST1VaL/index.html');
global.interfaceCloseCallback = null;

global.oscar2Menu.active = false;

global.openedMenus = {
  'Oscar': [],
  'Oscar2': [],
  'FeST1VaL': [],
};

global.openedHudModal = {
  'Oscar': [],
  'Oscar2': [],
  'FeST1VaL': [],
};

function openAnyMenu(html, menu, callbackFnOnClose) {
  if (global.cutscenePlaying && menu !== 'cutscene') return;
  //mp.console.logInfo("openAnyMenu callbackFnOnClose: "+ callbackFnOnClose);
  if (callbackFnOnClose !== null) {
    global.interfaceCloseCallback = callbackFnOnClose;
  }

  global.closeAnyMenuFunctions.closePhone();

  switch (html) {
    case 'Oscar':
      // if (global.new_menu.active) return;

      global.new_menu.active = true;
      if (global.OpenedNewMenu !== menu) {
        global.OpenedNewMenu = menu;
        global.openedMenus[html].push(menu);
      }
      global.menuOpen();
      break;
    case 'Oscar2':
      //if (global.new_menu.active) return;

      global.oscar2Menu.active = true;
      if (global.OpenedNewMenu !== menu) {
        global.OpenedNewMenu = menu;
        global.openedMenus[html].push(menu);
      }

      global.menuOpen();
      break;
    case 'FeST1VaL':
      // if (global.festMenu.active) return;

      global.festMenu.active = true;
      if (global.OpenedAnyMenu !== menu) {
        global.OpenedAnyMenu = menu;
        global.openedMenus[html].push(menu);
      }

      global.menuOpen();
      break;
  }

  global.INTERACTIONCHECK = false;
}

function openAnyHUDElement(html, menu, callbackFnOnClose) {
  if (global.cutscenePlaying) return;
  if (callbackFnOnClose !== null) {
    global.interfaceCloseCallback = callbackFnOnClose;
  }

  //global.closeAnyMenuFunctions.closePhone();

  switch (html) {
    case 'Oscar':
      // if (global.new_menu.active) return;

      global.new_menu.active = true;
      if (global.OpenedNewMenu !== menu) {
        global.OpenedNewMenu = menu;
        global.openedHudModal[html].push(menu);
      }

      break;
    case 'Oscar2':
      // if (global.new_menu.active) return;

      global.oscar2Menu.active = true;
      if (global.OpenedNewMenu !== menu) {
        global.OpenedNewMenu = menu;
        global.openedHudModal[html].push(menu);
      }

      break;
    case 'FeST1VaL':
      // if (global.festMenu.active) return;

      global.festMenu.active = true;
      if (global.OpenedAnyMenu !== menu) {
        global.OpenedAnyMenu = menu;
        global.openedHudModal[html].push(menu);
      }

      break;
  }

  global.closeAnyMenuFunctions.removeCloseCallback();
  // logger.debug(`openedMenus: ${JSON.stringify(global.openedMenus)}`);
  // logger.debug(`openedHudModal: ${JSON.stringify(global.openedHudModal)}`);
}

function closeAnyHUDElement(html, menu) {
  switch (html) {
    case 'Oscar':
      global.new_menu.active = false;

      break;
    case 'Oscar2':
      global.oscar2Menu.active = false;

      break;
    case 'FeST1VaL':
      global.festMenu.active = false;

      break;
  }

  const menuIndex = global.openedHudModal[html].indexOf(menu);
  if (menuIndex >= 0) {
    global.openedHudModal[html].splice(menuIndex, 1);
    global.OpenedNewMenu = null;
  }

}

function closeAnyMenu(html, menu, hide = false) {
  switch (html) {
    case 'Oscar':
      // if (!global.new_menu.active) return;

      if (!global.openedHudModal[html].length) {
        global.new_menu.active = false;
      }

      if (!hide) global.menuClose();
      break;
    case 'Oscar2':
      // if (!global.new_menu.active) return;

      if (!global.openedHudModal[html].length) {
        global.oscar2Menu.active = false;
      }

      if (!hide) global.menuClose();
      break;
    case 'FeST1VaL':
      // if (!global.festMenu.active) return;

      if (!global.openedHudModal[html].length) {
        global.festMenu.active = false;
      }

      if (!hide) global.menuClose();
      break;
  }

  const menuIndex = global.openedMenus[html].indexOf(menu);
  if (menuIndex >= 0) {
    global.openedMenus[html].splice(menuIndex, 1);
    global.OpenedNewMenu = null;
  }
  // mp.events.call('showHUD', true);

  global.closeAnyMenuFunctions.removeCloseCallback();
}

function closeAllMenu() {

  if (global.interfaceCloseCallback !== false) {
    if (global.new_menu.active || global.oscar2Menu.active || global.festMenu.active) {
      //mp.console.logInfo("global.openedHudModal: "+JSON.stringify(global.openedHudModal));
      if (global.openedHudModal['Oscar'].length === 0) global.new_menu.active = false;
      if (global.openedHudModal['Oscar2'].length === 0) global.oscar2Menu.active = false;
      if (global.openedHudModal['FeST1VaL'].length === 0) global.festMenu.active = false;

      global.OpenedNewMenu = null;

      if (global.interfaceCloseCallback !== null) {
        //mp.console.logInfo("interfaceCloseCallback: "+ global.interfaceCloseCallback);
        global.interfaceCloseCallback();
      }
    }
  }

  // logger.debug(`openedMenus: ${JSON.stringify(global.openedMenus)}`);
  // logger.debug(`openedHudModal: ${JSON.stringify(global.openedHudModal)}`);
}

global.anyMenuHTML = {
  openAnyMenu(html = 'Oscar', menu = 'any', callback = null) {
    openAnyMenu(html, menu, callback);
  },
  openAnyHUDElement(html = 'Oscar', menu = 'any', callback = null) {
    openAnyHUDElement(html, menu, callback);
  },
  closeAnyHUDElement(html = 'Oscar', menu = 'any') {
    closeAnyHUDElement(html, menu);
  },
  closeAnyMenu(html = 'Oscar', menu = 'any') {
    closeAnyMenu(html, menu);
  },
  closeAllMenu() {
    closeAllMenu();
  },
  hideAnyMenu(html = 'Oscar', menu = 'any') {
    closeAnyMenu(html, menu, true);
  },
  useCloseCallBack(){
    global.closeAnyMenuFunctions.useCloseCallBack();
  },
  removeCloseCallback() {
    global.closeAnyMenuFunctions.removeCloseCallback();
  }
};

global.anyEventslastRemoteEventName = null;
global.lastSendServerCheck = new Date().getTime();

global.anyEvents = {
  SendServer(callback) {
    const time = new Date().getTime() - global.lastSendServerCheck;

    const stringCallback = callback+"";
    const regex = "((\'|\")(.*?)(\'|\"))";
    let remoteEventName = stringCallback.match(regex);

    if (remoteEventName.length >= 0) {
      remoteEventName = remoteEventName[3];

      if (remoteEventName === global.anyEventslastRemoteEventName && time < 500) {
        mp.events.call('notify', 4, 9, "Слишком быстро", 1000);
        mp.console.logInfo("Слишком быстро, остановись! - "+time);
        return false;
      }
      global.lastSendServerCheck = new Date().getTime();

      //mp.console.logInfo("Callback: "+remoteEventName);
      global.anyEventslastRemoteEventName = remoteEventName;
    }

    callback();
    return true;
  }
}




global.closeAnyMenuFunctions = {
  closePhone(html, menu, callback) {
    if (menu !== 'phone' && (global.openedMenus['Oscar2'].indexOf("phone") >= 0 || global.openedHudModal['Oscar2'].indexOf("phone") >= 0))
    {
      mp.game.invoke('0x3BC861DF703E5097', mp.players.local.handle, true);
      mp.events.callRemote('closePlayerMenu', true);
      //mp.gui.chat.activate(true);

      global.phoneOpen = 0;
    }
  },
  removeCloseCallback() {
    if (global.interfaceCloseCallback !== false || global.interfaceCloseCallback !== null) {
      global.interfaceCloseCallback = null;
    }
  },
  useCloseCallBack() {
    //mp.console.logInfo("[USE] interfaceCloseCallback: "+global.interfaceCloseCallback);
    if (global.interfaceCloseCallback && global.interfaceCloseCallback !== false || global.interfaceCloseCallback !== null) {
      global.interfaceCloseCallback();
    }
  }
}


