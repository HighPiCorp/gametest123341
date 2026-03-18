global.keyBinds = {
    MICRO: {
        name: 'micro',
        keyName: 'N',
        admin: false,
        func: global.disableMicrophone,
    },
    ENGINE_CAR: {
        name: 'engine',
        keyName: '2',
        admin: false,
        func: global.engineCar,
    },
    TOGGLE_CRUISE_CONTROL: {
        name: 'cruise',
        keyName: '6',
        admin: false,
        func: global.cruiseControl,
    },
    EMERGENCY_SIGNAL: {
        name: 'emergency',
        keyName: 'DOWN',
        admin: false,
        func: global.emergencySignal,
    },
    PHONE: {
        name: 'phone',
        keyName: 'UP',
        admin: false,
        func: global.openPhone,
    },
    RELOAD: {
        name: 'reload',
        keyName: 'R',
        admin: false,
        func: global.reloadWeapon,
    },
    SAFE: {
        name: 'belt',
        keyName: 'J',
        admin: false,
        func: global.seatBelt,
    },
    LEFT_SIGNAL: {
        name: 'signalsL',
        keyName: 'LEFT',
        admin: false,
        func: global.leftSignal,
    },
    RIGHT_SIGNAL: {
        name: 'signalsR',
        keyName: 'RIGHT',
        admin: false,
        func: global.rightSignal,
    },
    LOCK_CAR_DOORS: {
        name: 'lockVehicle',
        keyName: 'L',
        admin: false,
        func: global.lockVehicle,
    },
};

var keyBindsDefault = {
    MICRO: {
        name: 'micro',
        keyName: 'N',
        admin: false,
        func: global.disableMicrophone,
    },
    ENGINE_CAR: {
        name: 'engine',
        keyName: '2',
        admin: false,
        func: global.engineCar,
    },
    TOGGLE_CRUISE_CONTROL: {
        name: 'cruise',
        keyName: '6',
        admin: false,
        func: global.cruiseControl,
    },
    EMERGENCY_SIGNAL: {
        name: 'emergency',
        keyName: 'DOWN',
        admin: false,
        func: global.emergencySignal,
    },
    PHONE: {
        name: 'phone',
        keyName: 'UP',
        admin: false,
        func: global.openPhone,
    },
    RELOAD: {
        name: 'reload',
        keyName: 'R',
        admin: false,
        func: global.reloadWeapon,
    },
    SAFE: {
        name: 'belt',
        keyName: 'J',
        admin: false,
        func: global.seatBelt,
    },
    LEFT_SIGNAL: {
        name: 'signalsL',
        keyName: 'LEFT',
        admin: false,
        func: global.leftSignal,
    },
    RIGHT_SIGNAL: {
        name: 'signalsR',
        keyName: 'RIGHT',
        admin: false,
        func: global.rightSignal,
    },
    LOCK_CAR_DOORS: {
        name: 'lockVehicle',
        keyName: 'L',
        admin: false,
        func: global.lockVehicle,
    },
};

global.codeSplit = {
    'micro' : 'MICRO',
    'engine' : 'ENGINE_CAR',
    'cruise' : 'TOGGLE_CRUISE_CONTROL',
    'emergency' : 'EMERGENCY_SIGNAL',
    'phone' : 'PHONE',
    'reload': 'RELOAD',
    'belt' : 'SAFE',
    'signalsR' : 'RIGHT_SIGNAL',
    'signalsL' : 'LEFT_SIGNAL',
    'lockVehicle' : 'LOCK_CAR_DOORS',
}

global.keyCodes = {
    'ESC': Keys.VK_ESCAPE,// +
    'F1': Keys.VK_F1,// +
    'F2': Keys.VK_F2,// +
    'F3': Keys.VK_F3,// +
    'F4': Keys.VK_F4,// +
    'F5': Keys.VK_F5,// +
    'F6': Keys.VK_F6,// +
    'F7': Keys.VK_F7,// +
    'F8': Keys.VK_F8,// +
    'F9': Keys.VK_F9,// +
    'F10': Keys.VK_F10,// +
    'F11': Keys.VK_F11,// +
    'F12': Keys.VK_F12,// +
    '~': Keys.VK_OEM_3,// +
    '1': Keys.VK_1,// +
    '2': Keys.VK_2,// +
    '3': Keys.VK_3,// +
    '4': Keys.VK_4,// +
    '5': Keys.VK_5,// +
    '6': Keys.VK_6,// +
    '7': Keys.VK_7,// +
    '8': Keys.VK_8,// +
    '9': Keys.VK_9,// +
    '-': Keys.VK_OEM_MINUS,// +
    '+': Keys.VK_OEM_PLUS,// +
    'DELETE': Keys.VK_DELETE,// +
    'TAB': Keys.VK_TAB, // +
    'Q': Keys.VK_Q, // +
    'W': Keys.VK_W, // +
    'E': Keys.VK_E, // +
    'R': Keys.VK_R,// +
    'T': Keys.VK_T,// +
    'Y': Keys.VK_Y,// +
    'U': Keys.VK_U,// +
    'I': Keys.VK_I,// +
    'O': Keys.VK_O,// +
    'P': Keys.VK_P,// +
    'OEM_4': Keys.VK_OEM_4, // {[ +
    'OEM_6': Keys.VK_OEM_6, // }] +
    'OEM_5': Keys.VK_OEM_5, // |\ +
    'CAPITAL': Keys.VK_CAPITAL, // CAPS LOCK +
    'A': Keys.VK_A,// +
    'S': Keys.VK_S,// +
    'D': Keys.VK_D,// +
    'F': Keys.VK_F,// +
    'G': Keys.VK_G,// +
    'H': Keys.VK_H,// +
    'J': Keys.VK_J,// +
    'K': Keys.VK_K,// +
    'L': Keys.VK_L,// +
    'OEM_1': Keys.VK_OEM_1, // :; +
    'OEM_7': Keys.VK_OEM_7, // "' +
    'ENTER': Keys.VK_RETURN, // +
    'LSHIFT': Keys.VK_LSHIFT,// +
    'Z': Keys.VK_Z,// +
    'X': Keys.VK_X,// +
    'C': Keys.VK_C,// +
    'V': Keys.VK_V,// +
    'B': Keys.VK_B,// +
    'N': Keys.VK_N,// +
    'M': Keys.VK_M,// +
    'OEM_COMMA': Keys.VK_OEM_COMMA, // <, +
    'OEM_PERIOD': Keys.VK_OEM_PERIOD, // >. +
    'OEM_2': Keys.VK_OEM_2, // ?/ +
    'RSHIFT': Keys.VK_RSHIFT,// +
    'CONTROL': Keys.VK_CONTROL,// +
    'MENU': Keys.VK_MENU,// +
    'LWIN': Keys.VK_LWIN,// +
    'RWIN': Keys.VK_RWIN,// +
    'SPACE': Keys.VK_SPACE,// +
    'LEFT': Keys.VK_LEFT,// +
    'UP': Keys.VK_UP,// +
    'RIGHT': Keys.VK_RIGHT,// +
    'DOWN': Keys.VK_DOWN,// +
    'N_/': Keys.VK_DIVIDE,// +
    'N_*': Keys.VK_MULTIPLY,// +
    'N_-': Keys.VK_SUBTRACT,// +
    'N9': Keys.VK_NUMPAD9,// +
    'N8': Keys.VK_NUMPAD8,// +
    'N7': Keys.VK_NUMPAD7,// +
    'N6': Keys.VK_NUMPAD6,// +
    'N5': Keys.VK_NUMPAD5,// +
    'N4': Keys.VK_NUMPAD4,// +
    'N3': Keys.VK_NUMPAD3,// +
    'N2': Keys.VK_NUMPAD2,// +
    'N1': Keys.VK_NUMPAD1,// +
    'N0': Keys.VK_NUMPAD0,// +
    'N_.': Keys.VK_DECIMAL,// +
    'N_+': Keys.VK_ADD// +
};

global.keyCodesString = {
    'ESC': 'Keys.VK_ESCAPE',// +
    'F1': 'Keys.VK_F1',// +
    'F2': 'Keys.VK_F2',// +
    'F3': 'Keys.VK_F3',// +
    'F4': 'Keys.VK_F4',// +
    'F5': 'Keys.VK_F5',// +
    'F6': 'Keys.VK_F6',// +
    'F7': 'Keys.VK_F7',// +
    'F8': 'Keys.VK_F8',// +
    'F9': 'Keys.VK_F9',// +
    'F10': 'Keys.VK_F10',// +
    'F11': 'Keys.VK_F11',// +
    'F12': 'Keys.VK_F12',// +
    '~': 'Keys.VK_OEM_3',// +
    '1': 'Keys.VK_1',// +
    '2': 'Keys.VK_2',// +
    '3': 'Keys.VK_3',// +
    '4': 'Keys.VK_4',// +
    '5': 'Keys.VK_5',// +
    '6': 'Keys.VK_6',// +
    '7': 'Keys.VK_7',// +
    '8': 'Keys.VK_8',// +
    '9': 'Keys.VK_9',// +
    '-': 'Keys.VK_OEM_MINUS',// +
    '+': 'Keys.VK_OEM_PLUS',// +
    'DELETE': 'Keys.VK_DELETE',// +
    'TAB': 'Keys.VK_TAB', // +
    'Q': 'Keys.VK_Q', // +
    'W': 'Keys.VK_W', // +
    'E': 'Keys.VK_E', // +
    'R': 'Keys.VK_R',// +
    'T': 'Keys.VK_T',// +
    'Y': 'Keys.VK_Y',// +
    'U': 'Keys.VK_U',// +
    'I': 'Keys.VK_I',// +
    'O': 'Keys.VK_O',// +
    'P': 'Keys.VK_P',// +
    'OEM_4': 'Keys.VK_OEM_4', // {[ +
    'OEM_6': 'Keys.VK_OEM_6', // }] +
    'OEM_5': 'Keys.VK_OEM_5', // |\ +
    'CAPITAL': 'Keys.VK_CAPITAL', // CAPS LOCK +
    'A': 'Keys.VK_A',// +
    'S': 'Keys.VK_S',// +
    'D': 'Keys.VK_D',// +
    'F': 'Keys.VK_F',// +
    'G': 'Keys.VK_G',// +
    'H': 'Keys.VK_H',// +
    'J': 'Keys.VK_J',// +
    'K': 'Keys.VK_K',// +
    'L': 'Keys.VK_L',// +
    'OEM_1': Keys.VK_OEM_1, // :; +
    'OEM_7': Keys.VK_OEM_7, // "' +
    'ENTER': Keys.VK_RETURN, // +
    'LSHIFT': 'Keys.VK_LSHIFT',// +
    'Z': 'Keys.VK_Z',// +
    'X': 'Keys.VK_X',// +
    'C': 'Keys.VK_C',// +
    'V': 'Keys.VK_V',// +
    'B': 'Keys.VK_B',// +
    'N': 'Keys.VK_N',// +
    'M': 'Keys.VK_M',// +
    'OEM_COMMA': Keys.VK_OEM_COMMA, // <, +
    'OEM_PERIOD': Keys.VK_OEM_PERIOD, // >. +
    'OEM_2': Keys.VK_OEM_2, // ?/ +
    'RSHIFT': 'Keys.VK_RSHIFT',// +
    'CONTROL': 'Keys.VK_LCONTROL',// +
    'MENU': 'Keys.VK_MENU',// +
    'LMENU': 'Keys.VK_LMENU',
    'RMENU': 'Keys.VK_RMENU',
    'LWIN': Keys.VK_LWIN,// +
    'RWIN': Keys.VK_RWIN,// +
    'SPACE': 'Keys.VK_SPACE',// +
    'LEFT': 'Keys.VK_LEFT',// +
    'UP': 'Keys.VK_UP',// +
    'RIGHT': 'Keys.VK_RIGHT',// +
    'DOWN': 'Keys.VK_DOWN',// +
    'N_/': 'Keys.VK_DIVIDE',// +
    'N_*': 'Keys.VK_MULTIPLY',// +
    'N_-': 'Keys.VK_SUBTRACT',// +
    'N9': 'Keys.VK_NUMPAD9',// +
    'N8': 'Keys.VK_NUMPAD8',// +
    'N7': 'Keys.VK_NUMPAD7',// +
    'N6': 'Keys.VK_NUMPAD6',// +
    'N5': 'Keys.VK_NUMPAD5',// +
    'N4': 'Keys.VK_NUMPAD4',// +
    'N3': 'Keys.VK_NUMPAD3',// +
    'N2': 'Keys.VK_NUMPAD2',// +
    'N1': 'Keys.VK_NUMPAD1',// +
    'N0': 'Keys.VK_NUMPAD0',// +
    'N_.': 'Keys.VK_DECIMAL',// +
    'N_+': 'Keys.VK_ADD'// +
};

global.getKeyBy = function(by) {
    try {
        if (by in keyBinds) {
            const keyName = keyBinds[by].keyName;
            return keyCodes[keyName];
        }
    } catch (e) {
        logger.error(e);
    }
};

global.getKeyNameBy = function(by) {
    try {
        if (by in keyBinds) {
            return keyBinds[by].keyName;
        }
    } catch (e) {
        logger.error(e);
    }
};

mp.events.add('CLIENT::stat:resetButton', () => {

    var binds = mp.storage.data.binds;
    var keys = Object.keys(keyBinds);
    keys.forEach(function (key) {
        if (key in binds) {
            if(key == 'MICRO') mp.keys.unbind(keyCodes[keyBinds[key].keyName], true, global.enableMicrophone);

            mp.keys.unbind(keyCodes[keyBinds[key].keyName], false, keyBinds[key].func);
        }
    });

    global.keyBinds = keyBindsDefault;
    saveBinds();

    binds = mp.storage.data.binds;
    keys = Object.keys(keyBinds);
    keys.forEach(function (key) {
        if (key in binds) {
            mp.keys.bind(keyCodes[keyBinds[key].keyName], false, keyBinds[key].func);
            if(key == 'MICRO') mp.keys.bind(keyCodes[keyBinds[key].keyName], true, global.enableMicrophone);
        }
    });

    mp.events.callRemote("SERVER::BINDER:RESET_BUTTONS");
});

mp.events.add('CLIENT::stat:changeButton', (name, kkey) => {

    //mp.console.logInfo(`${name} ${kkey}`);
    let key = 'A';

    for (const [keys, value] of Object.entries(global.keyCodesString)) {
        if(value == kkey){
            key = keys;
        }
    }

    let k = key.toUpperCase();
    name = codeSplit[name];
    if(name == 'MICRO') mp.keys.unbind(keyCodes[keyBinds[name].keyName], true, global.enableMicrophone);

    mp.keys.unbind(keyCodes[keyBinds[name].keyName], false, keyBinds[name].func);

    try {
        if (name in keyBinds && k in keyCodes) {
            keyBinds[name].keyName = k;
            saveBinds();
            //updateKeysData();
        }

        mp.keys.bind(keyCodes[keyBinds[name].keyName], false, keyBinds[name].func);
        if(name == 'MICRO') mp.keys.bind(keyCodes[keyBinds[name].keyName], true, global.enableMicrophone);
        mp.events.call('notify', 2, 9, `Вы поменяли кнопку ${keyBinds[name].name} на ${keyBinds[name].keyName}`, 3000);

    } catch (e) {
        logger.error(e);
    }

    updateHudKeys();
});

mp.events.add('CLIENT::stat:startChangeButton', (key) => {
    mp.events.call('notify', 2, 9, `Нажмите на желаемую кнопку для назначения`, 3000);
});



global.saveBinds = function() {
    try {
        mp.storage.data.binds = Object.assign({}, keyBinds);
    } catch (e) {
        logger.error(e);
    }
};
// mp.storage.data.binds = [];

global.loadBinds = function() {
    try {
        if (mp.storage.data.binds) {
            const binds = mp.storage.data.binds;
            const keys = Object.keys(keyBinds);
            keys.forEach(function (key) {
                if (key in binds) {
                    keyBinds[key].keyName = binds[key].keyName;
                    mp.keys.bind(keyCodes[keyBinds[key].keyName], false, keyBinds[key].func);
                    if(key == 'MICRO') mp.keys.bind(keyCodes[keyBinds[key].keyName], true, global.enableMicrophone);
                }
            });
        }
        else
        {
            global.keyBinds = keyBindsDefault;
            let binds = keyBindsDefault;
            let keys = Object.keys(global.keyBinds);
            keys.forEach(function (key) {
                if (key in binds) {
                    mp.keys.bind(global.keyCodes[global.keyBinds[key].keyName], false, global.keyBinds[key].func);
                    if(key == 'MICRO') mp.keys.bind(global.keyCodes[global.keyBinds[key].keyName], true, global.enableMicrophone);
                }
            });

            saveBinds();
        }

        updateHudKeys();

    } catch (e) {
        logger.error(e);
    }
};

function updateHudKeys(){
    var keys = [
        {key: keyBinds.PHONE.keyName.toLowerCase(), name: "открыть телефон"},
        {key: keyBinds.ENGINE_CAR.keyName.toLowerCase(), name: "завести двигатель"},
        {key: "I", name: "открыть инвентарь"},
        {key: "M", name: "открыть меню персонажа"},
        {key: "U", name: "открыть меню анимаций"},
        {key: "5", name: "увидеть никнеймы"},
        {key: "F5", name: "скрыть игровой HUD"},
    ];
    mp.gui.execute(`HUD.setButtonKeys('${JSON.stringify(keys)}')`);
}

mp.events.add('CLIENT::keyBind:engine_notify', () => {
    mp.events.call('notify', 3, 9, `Что бы завести двигатель нажмите ${keyBinds.ENGINE_CAR.keyName}`, 3000);
  });
