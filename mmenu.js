const thisMenu = 'Mmenu';
const thisHTML = 'FeST1VaL';
const thisMenuCall = global.festMenu;

let MmenuOpen = false;

const callbackOnClose = () => {
  thisMenuCall.call("CEF::Mmenu:close");
  global.anyMenuHTML.closeAnyHUDElement(thisHTML, thisMenu);
  MmenuOpen = false;
  global.menuOpened = false;
};

mp.keys.bind(Keys.VK_M, false, () => {
  if (!global.loggedin || global.chatActive || global.editing || global.phoneOpen || global.cuffed || global.localplayer.getVariable('InDeath') == true) return;

  if (MmenuOpen) {
    callbackOnClose();

    mp.gui.cursor.visible = false;
    global.menuOpened = false;
  } else {
    global.anyEvents.SendServer(() => mp.events.callRemote("SERVER::Mmenu:open"));
  }
});

mp.events.add('CLIENT::Mmenu:open', (info) => {
  if (!global.menuOpened) {
    mp.gui.cursor.visible = true;
    global.menuOpened = true;

    global.anyMenuHTML.openAnyHUDElement(thisHTML, thisMenu, callbackOnClose());
    thisMenuCall.call("CEF::Mmenu:open", info);
    MmenuOpen = true;
  }
});

mp.events.add('CLIENT::Mmenu:press', (name) => {
  callbackOnClose();

  switch (name) {
    case "Меню статистики": // меню статистики
      mp.events.callRemote("SERVER::OPEN_STATS", 0);
      break;
    case "База и вызовы": // База и вызовы ПЛАНШЕТ
      global.tablet.execute('tablet.openPolice();');
      global.menuOpen();
      break;
    case "Меню фракции": // меню фракции ПЛАНШЕТ
      global.tablet.execute('tablet.openFrac();');
      global.menuOpen();
      break;
    case "Меню дома": // меню дома
      mp.events.call("manageHouse");
      break;
    case "Меню семьи": // меню семьи
      if (!localplayer.getVariable('IS_FAMILY')) return;
      mp.events.callRemote('openfamilymanager');
      break;
    case "Рабочее меню": // рабочее меню
      mp.gui.cursor.visible = false;
      mp.events.callRemote('SERVER::JOB:openMenu');
      break;
    case "Меню бизнеса": // меню бизнеса
      mp.events.callRemote('SERVER::BUSINESS:OPEN_MANAGE_MENU', true, -1);
      break;
    case "BattlePass": // меню бизнеса
      mp.events.callRemote('SERVER::bp:open');
      break;
    // case 6:
    //   break;
  }
});


