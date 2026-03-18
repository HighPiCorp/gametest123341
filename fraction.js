
 global.policeGarage = mp.browsers.new('package://interfaces/ui/Menus/index.html');
var isFam = false;
mp.events.add('openPoliceGarage', (data, title, isFamily = false) => {
    if (global.menuCheck() || !global.loggedin || chatActive) return;
    isFam = isFamily;
    global.menuOpen();
    global.policeGarage.execute(`policeGarage.init(${data}, '${title}',${isFam})`);
});

mp.events.add('closePoliceGarage', () => {
    isFam = false;
    global.menuClose();
    global.policeGarage.execute(`policeGarage.hide()`);
});

mp.events.add('takePoliceGarage', (name) => {
  //mp.console.logInfo(name);
    isFam ? global.anyEvents.SendServer(() => mp.events.callRemote('takeFamilyGarage', parseInt(name))) : global.anyEvents.SendServer(() => mp.events.callRemote('takePoliceGarage', name));

    //isFam ? mp.events.callRemote('takeFamilyGarage', parseInt(name)) : mp.events.callRemote('takePoliceGarage', name);
});

const wcraft = {
    tab: 0,
    frac: 0,
    data: [],
  };
  mp.events.add('wcraft', (act, value, sub) => {
    switch (act) {
      case 'cat':
        wcraft.tab = value;
        global.policeGarage.execute(`wcraft.set(${wcraft.frac},${value},'${JSON.stringify(wcraft.data[value])}')`);
        break;
      case 'buy':
        global.anyEvents.SendServer(() => mp.events.callRemote('wcraft', wcraft.frac, value));
        break;
      case 'ammo':
        global.anyEvents.SendServer(() => mp.events.callRemote('wcraftammo', wcraft.frac, value));
        break;
    }
  });
  mp.events.add('closeWCraft', () => {
    global.menuClose();
    wcraft.top = 0;
  });
  mp.events.add('openWCraft', (frac, json) => {
    // mp.gui.chat.push(`${frac}:${json}`);
    wcraft.data = JSON.parse(json);
    wcraft.frac = frac;
    global.policeGarage.execute(`wcraft.set(${frac}, 0,'${JSON.stringify(wcraft.data)}')`);
    global.policeGarage.execute('wcraft.active=1');
    global.menuOpen();
  });

  mp.events.add('orderMatsOpen', () => {
    if (!global.loggedin || global.menuCheck() || cuffed || localplayer.getVariable('InDeath') === true) return;
    global.menuOpen();
    global.policeGarage.execute('orderMats.show()');
  });

  mp.events.add('orderMatsClose', () => {
    global.menuClose();
    global.policeGarage.execute('orderMats.hide()');
  });

  mp.events.add('orderMatsSet', (json) => {
    global.policeGarage.execute(`orderMats.set(${json})`);
  });

  mp.events.add('orderMats', (val, data) => {
    switch (val) {
        case 'info':
            global.anyEvents.SendServer(() => mp.events.callRemote('orderMats', val, data));
            break;
        case 'order':
            global.anyEvents.SendServer(() => mp.events.callRemote('orderMats', val, data));
            break;
        case 'med':
            global.anyEvents.SendServer(() => mp.events.callRemote('orderMats', val, data));
            break;
    }
  });
