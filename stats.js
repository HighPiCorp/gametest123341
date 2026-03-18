const thisMenu = 'stats';
const thisHTML = 'Oscar2';
const thisMenuCall = global.oscar2Menu;

const KeyDict = {
  0: "help",
  1: "hud",
  2: "quest",
  3: "interaction",
  4: "playersName",
  5: "transparent",
  6: "timeMessage",
  7: "notifySound",
};

global.donate = null;
global.donateExit = null;

global.statsOpen = false;
global.donatePageOpen = false;
global.donatePageExitOpen = false;

  const callbackOnClose = () => {
    const res = {show: false};

    thisMenuCall.call('CEF::stat:update', JSON.stringify(res));
    global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
    global.statsOpen = false;
  };

  mp.keys.bind(Keys.VK_F9, false, () => {
    if (!global.loggedin || global.chatActive || global.menuOpened || global.taxiPriceOpen || global.popupOpen || global.animMenuOpen || global.phoneOpen || global.editing || global.cuffed || global.cutscenePlaying || global.localplayer.getVariable('InDeath') == true) return;

    if (global.statsOpen) {
      callbackOnClose();
    } else {
      global.anyEvents.SendServer(() => mp.events.callRemote("SERVER::OPEN_STATS", 0));
    }
  });

  mp.events.add('CLIENT::STATS:OPEN', (data) => {
    if (global.cutscenePlaying) return;
    let res = JSON.parse(data);

    // res['setting']['control']['micro'] = global.keyCodesString[global.keyBinds.MICRO.keyName].toString();
    // res['setting']['control']['reload'] = global.keyCodesString[global.keyBinds.RELOAD.keyName].toString();
    // res['setting']['control']['engine'] = global.keyCodesString[global.keyBinds.ENGINE_CAR.keyName].toString();
    // res['setting']['control']['belt'] = global.keyCodesString[global.keyBinds.SAFE.keyName].toString();
    // res['setting']['control']['cruise'] = global.keyCodesString[global.keyBinds.TOGGLE_CRUISE_CONTROL.keyName].toString();
    // res['setting']['control']['signalsL'] = global.keyCodesString[global.keyBinds.LEFT_SIGNAL.keyName].toString();
    // res['setting']['control']['emergency'] = global.keyCodesString[global.keyBinds.EMERGENCY_SIGNAL.keyName].toString();
    // res['setting']['control']['signalsR'] = global.keyCodesString[global.keyBinds.RIGHT_SIGNAL.keyName].toString();
    // res['setting']['control']['lockVehicle'] = global.keyCodesString[global.keyBinds.LOCK_CAR_DOORS.keyName].toString();
    // res['setting']['control']['phone'] = global.keyCodesString[global.keyBinds.PHONE.keyName].toString();
    //
    // res['setting']['interface']['toogle']['help'] = mp.storage.data.hud.help;
    // res['setting']['interface']['toogle']['hud'] = mp.storage.data.hud.hud;
    // res['setting']['interface']['toogle']['quest'] = mp.storage.data.hud.quest;
    //
    // res['setting']['interface']['toogle']['interaction'] = global.showInteraction;
    // res['setting']['interface']['toogle']['playersName'] = mp.storage.data.hud.playersName;
    //
    // res['setting']['interface']['toogle']['transparent'] = mp.storage.data.chatcfg.alpha;
    // res['setting']['interface']['toogle']['timeMessage'] = mp.storage.data.chatcfg.timestamp;
    // res['setting']['interface']['toogle']['notifySound'] = mp.storage.data.notifySound;
    //
    //
    // res['setting']['interface']['select']['heightChat']['val'] = mp.storage.data.chatcfg.chatsize;
    // res['setting']['interface']['select']['heightFontChat']['val'] = mp.storage.data.chatcfg.fontstep;
    // res['setting']['interface']['select']['mutLvl']['val'] = mp.storage.data.mutelvl;
    //
    // res['setting']['aim']['type'] = mp.storage.data.aim.type;
    // res['setting']['aim']['color'] = mp.storage.data.aim.color;

    thisMenuCall.call('CEF::stat:update', JSON.stringify(res));

    global.statsOpen = true;
    global.anyMenuHTML.openAnyMenu(thisHTML, thisMenu, callbackOnClose);
  });

  mp.events.add('CLIENT::STATS:UPDATE', (data) => {
    thisMenuCall.call('CEF::stat:update', data);
  });

  mp.events.add('CLIENT::STATS:UPDATE_SETTINGS', (data) => {
    let res = JSON.parse(data);

    res['setting']['control']['micro'] = global.keyCodesString[global.keyBinds.MICRO.keyName].toString();
    res['setting']['control']['reload'] = global.keyCodesString[global.keyBinds.RELOAD.keyName].toString();
    res['setting']['control']['engine'] = global.keyCodesString[global.keyBinds.ENGINE_CAR.keyName].toString();
    res['setting']['control']['belt'] = global.keyCodesString[global.keyBinds.SAFE.keyName].toString();
    res['setting']['control']['cruise'] = global.keyCodesString[global.keyBinds.TOGGLE_CRUISE_CONTROL.keyName].toString();
    res['setting']['control']['signalsL'] = global.keyCodesString[global.keyBinds.LEFT_SIGNAL.keyName].toString();
    res['setting']['control']['emergency'] = global.keyCodesString[global.keyBinds.EMERGENCY_SIGNAL.keyName].toString();
    res['setting']['control']['signalsR'] = global.keyCodesString[global.keyBinds.RIGHT_SIGNAL.keyName].toString();
    res['setting']['control']['lockVehicle'] = global.keyCodesString[global.keyBinds.LOCK_CAR_DOORS.keyName].toString();
    res['setting']['control']['phone'] = global.keyCodesString[global.keyBinds.PHONE.keyName].toString();

    res['setting']['interface']['toogle']['help'] = mp.storage.data.hud.help;
    res['setting']['interface']['toogle']['hud'] = mp.storage.data.hud.hud;
    res['setting']['interface']['toogle']['quest'] = mp.storage.data.hud.quest;

    res['setting']['interface']['toogle']['interaction'] = global.showInteraction;
    res['setting']['interface']['toogle']['playersName'] = mp.storage.data.hud.playersName;

    res['setting']['interface']['toogle']['transparent'] = mp.storage.data.chatcfg.alpha;
    res['setting']['interface']['toogle']['timeMessage'] = mp.storage.data.chatcfg.timestamp;
    res['setting']['interface']['toogle']['notifySound'] = mp.storage.data.notifySound;


    res['setting']['interface']['select']['heightChat']['val'] = mp.storage.data.chatcfg.chatsize;
    res['setting']['interface']['select']['heightFontChat']['val'] = mp.storage.data.chatcfg.fontstep;
    res['setting']['interface']['select']['mutLvl']['val'] = mp.storage.data.mutelvl;

    res['setting']['aim']['type'] = mp.storage.data.aim.type;
    res['setting']['aim']['color'] = mp.storage.data.aim.color;

    thisMenuCall.call('CEF::stat:update', JSON.stringify(res));
  });

  mp.events.add('CLIENT::stat:close', () => {
    global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
    global.statsOpen = false;
  });

  mp.events.add('CLIENT::stat:toogleChat', (data) => {
    if(data == true) global.oldchat.execute('show()');
    else global.oldchat.execute('hide()');
  });

  mp.events.add('CLIENT::stat:changeName', (data) => {
    global.anyEvents.SendServer(() => mp.events.callRemote("SERVER::stat:changeName", data));
    global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
  });

  mp.events.add('CLIENT::stat:addCoin', (login) => {
    try {
      const url = 'https://saintsworld.net/payment/?login=' + login; //daniellik
      logger.debug('https://saintsworld.net/payment/?login=' + login);

      global.donate = mp.browsers.new(url);
      global.donate.active = true;

      global.donatePageOpen = true;

      global.donateExit = mp.browsers.new('package://RouleteDonate/exit.html');
      global.donateExit.active = false;

      global.donatePageExitOpen = true;

      setTimeout(() => {
        if (donatePageExitOpen) {
          global.donateExit.active = true;
        }
      }, 3000);
    }
    catch(e) { logger.error(e) }
  });

  mp.events.add('CLIENT::payment:exit', () => {
    if (global.statsOpen) {
      callbackOnClose();
    }

    if (global.donatePageOpen) {
      global.donate.active = false;
      global.donate.destroy();

      global.donatePageOpen = false;
    }

    if (global.donatePageExitOpen) {
      global.donateExit.active = false;
      global.donateExit.destroy();

      global.donatePageExitOpen = false;
    }

    if (global.restorePageOpen) {
      global.restore.active = false;
      global.restore.destroy();

      global.restorePageOpen = false;

      mp.events.call("CLIENT::auth:changeActive", 0);
    }
  });

  mp.keys.bind(Keys.VK_ESCAPE, false, function() {
    mp.events.call("CLIENT::payment:exit");
  });

  mp.events.add('CLIENT::stat:buyShop', (data) => {
    callbackOnClose();
    var res = JSON.parse(data);
    global.anyEvents.SendServer(() => mp.events.callRemote("SERVER::DONATE:BUY_SHOP", res.name));
  });

  mp.events.add('CLIENT::stat:refreshMicro', () => {
    mp.events.call('v_reload');
  });

  mp.keys.bind(Keys.VK_F3, false, function () {
    mp.events.call('v_reload');
  });

  mp.events.add('CLIENT::stat:changeInterfaceToogle', (key, toggle) => {
    let action = KeyDict[key];
    //mp.console.logInfo(`${action} ${toggle}`);
    switch(action){
      case "help":
        global.showHint = toggle;
        mp.storage.data.hud.help = toggle;
        mp.gui.execute(`hidehelp(${!global.showHint})`);
        break;
      case "hud":
        global.showGlobalHud = toggle;
        mp.storage.data.hud.hud = toggle;
        mp.events.call('showHUD', toggle);
        break;
      case "quest":
        mp.storage.data.hud.quest = toggle;
        mp.events.call('toggleQuestPanel', toggle);
        break;
      case "name":
       // global.showGamertags = toggle;
        break;
      case "timer":
        //global.showGamertags = toggle;
        break;
      case "interaction":
        global.showInteraction = toggle;
        break;
      case "playersName":
        mp.storage.data.hud.playersName = toggle;
        global.showGamertags = toggle;
        break;
      case "compass":
        break;
      case "transparent":
        if(toggle){
          mp.storage.data.chatcfg.alpha = true;
          global.oldchat.execute(`newcfg(3,1);`);
        }
        else {
          mp.storage.data.chatcfg.alpha = false;
          global.oldchat.execute(`newcfg(3,0);`);
        }
        break;
      case "timeMessage":
        if(toggle){
          global.oldchat.execute(`newcfg(0,1);`);
          mp.storage.data.chatcfg.timestamp = true;
        }
        else {
          global.oldchat.execute(`newcfg(0,0);`);
          mp.storage.data.chatcfg.timestamp = false;
        }
        break;
      case "notifySound":
        mp.storage.data.notifySound = toggle;
        break;
    }
  });

  mp.events.add('CLIENT::stat:getReward', (data) => {
    var res = JSON.parse(data);
    global.anyEvents.SendServer(() => mp.events.callRemote("SERVER::REWARD:GET", res.title, res.list[0].text, res.type));
  });

  mp.events.add('CLIENT::stat:changeMoney', (num) => {
    global.anyEvents.SendServer(() => mp.events.callRemote("SERVER::DONATE:CHANGE", num));
  });

  mp.events.add('CLIENT::stat:sendPromo', (promo) => {
    global.anyEvents.SendServer(() => mp.events.callRemote("SERVER::PROMO:SET_PROMO", promo));
  });

  mp.events.add('CLIENT::stat:openRoulette', () => {
    callbackOnClose();

    mp.events.call('CLIENT::caseRoulette:init');
    //mp.events.call('notify', 4, 9, "Кейсы временно не доступны", 3000);
  });

  mp.events.add('CLIENT::stat:createNewReport', (text = "Мне нужна помощь") => {
    global.anyEvents.SendServer(() => mp.events.callRemote("SERVER::REPORT:CREATE_REPORT", text));
  });

  mp.events.add('CLIENT::stat:changeReportItem', (id) => {
    mp.events.callRemote("SERVER::REPORT:CHANGE_REPORT", id);
  });

  mp.events.add('CLIENT::stat:update_report', (data) => {
    thisMenuCall.call('CEF::stat:updateActiveReport', data);
  });

  mp.events.add('CLIENT::stat:sendReportMessage', (text) => {
    mp.events.callRemote("SERVER::REPORT:SEND_MESSAGE", text);
  });

  mp.events.add('CLIENT::stat:reportClose', (id) => {
    mp.events.callRemote("SERVER::REPORT:CLOSE", id);
  });

  mp.events.add('CLIENT::stat:reportTp', () => {
    mp.events.callRemote("SERVER::REPORT:TP");
    callbackOnClose();
  });

  mp.events.add('CLIENT::stat:reportSpectate', () => {
    mp.events.callRemote("SERVER::REPORT:SPECTATE");
    callbackOnClose();
  });

  mp.events.add('CLIENT::stat:getWorkReport', (id) => {
    mp.events.callRemote("SERVER::REPORT:GET_WORK", id);
  });

  mp.events.add('CLIENT::stat:getVip', (id) => {
    global.anyEvents.SendServer(() => mp.events.callRemote("SERVER::DONATE:GET_VIP", id));
  });

  mp.events.add('CLIENT::stat:reportRollback', (id) => {
    global.anyEvents.SendServer(() => mp.events.callRemote("SERVER::REPORT:ROLLBACK", id));
  });

  mp.events.add('CLIENT:stat:changeInterfaceVal', (key, value ) => {
    //mp.console.logInfo(`key: ${key} value: ${value} mp.storage.data.mutelvl: ${mp.storage.data.mutelvl}`);
    switch(key){
      case "mutLvl":
        mp.storage.data.mutelvl = value;
        break;
      case "heightChat":
        global.oldchat.execute(`newcfg(1,${value});`);
        mp.storage.data.chatcfg.chatsize = value;
        break;
      case "heightFontChat":
        global.oldchat.execute(`newcfg(2,${value});`);
        mp.storage.data.chatcfg.fontstep = value;
        break;
      case "heightStringChat":
        //
        break;
    }
  });

  mp.events.add('CLIENT::stat:reportDelete', (id) => {
    global.anyEvents.SendServer(() => mp.events.callRemote("SERVER::REPORT:DELETE", id));
  });

  mp.events.add('CLIENT::stat:changeTabHelp', (id) => {
    mp.events.callRemote("SERVER::HELP:CHANGE_TAB", id);
  });

  mp.events.add('CLIENT::stat:changeTab', (id) => {
    mp.events.callRemote("SERVER::STATS:CHANGE_TAB", id);
  });

  var playReportSound = false;

  mp.events.add('CLIENT::REPORT:PLAY_SOUND', () => {
    if(!playReportSound){
      global.policeGarage.execute(`client_playMusic('package://cef/sounds/report.mp3', 0.6)`);
      playReportSound = true;

      setTimeout(() => {
        playReportSound = false;
      }, 1000);
    }
  });


  mp.events.add('CLIENT::stat:acceptAim', (data) => {
    var info = JSON.parse(data);

    mp.storage.data.aim.type = info.type;
    mp.storage.data.aim.color = `#${info.newColorHex}`;

    mp.gui.execute(`HUD.setAimData(${mp.storage.data.aim.type}, '${mp.storage.data.aim.color}')`);

    mp.events.call('notify', 2, 9, `Вы изменили прицел`, 3000);
  });

  mp.events.add('CLIENT::stat:resetAim', () => {
    mp.storage.data.aim.type = 0;
    mp.storage.data.aim.color = `#FFFFFF`;

    mp.gui.execute(`HUD.setAimData(${mp.storage.data.aim.type}, '${mp.storage.data.aim.color}')`);
    global.anyEvents.SendServer(() => mp.events.callRemote("SERVER::STAT:RESET_AIM"));
  });



