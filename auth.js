global.restore = null;
global.restoreExit = null;
global.restorePageOpen = false;
global.restorePageExitOpen = false;

var cam = mp.cameras.new('default', new mp.Vector3(-95, 19, 1182), new mp.Vector3(0, 0, 0), 70);
cam.pointAtCoord(-95, 19, 0);
cam.setActive(true);
mp.game.cam.renderScriptCams(true, false, 0, true, false);

var respawn;
// var auth = mp.browsers.new('package://cef/auth.html');
var auth;
//auth.execute(`slots.server=${serverid};`);

const thisMenu = 'auth';
const thisHTML = 'Oscar2';
const thisMenuCall = mp.browsers.new('package://interfaces/ui_new/Auth/index.html');

//mp.gui.cursor.visible = true;

var lastButAuth = 0;
var lastButSlots = 0;

setTimeout(function () {

    mp.gui.cursor.visible = true;

}, 1500);

mp.events.add('browserDomReady', (browser) => {
    if(browser == thisMenuCall)
    {
        if (mp.storage.data.account != undefined)
        {
            thisMenuCall.call('CEF::auth:update', JSON.stringify({login: mp.storage.data.account.username, password: mp.storage.data.account.pass, remember: true}));
        }
        //thisMenuCall.call('CEF::auth:update', JSON.stringify({login: mp.storage.data.account.username, password: mp.storage.data.account.pass}));
    }
});

mp.events.add('selectCharacter', () => {

});

// events from cef
mp.events.add('CLIENT::auth:auth', function (authData) {
    if (new Date().getTime() - lastButAuth < 3000) {
        //mp.events.call('notify', 4, 9, "Слишком быстро", 3000);
        thisMenuCall.call('CEF::auth:update', JSON.stringify({error: 'Слишком быстро', errorInput: []}));
        return;
    }
    lastButAuth = new Date().getTime();
    //mp.console.logError(authData);
    authData = JSON.parse(authData);

    var username = authData['login'].value,
        pass = authData['password'].value,
        check = authData['remember'];

    //mp.console.logInfo(authData['remember']);
    if (check) {
        //mp.console.logInfo('save');
        mp.storage.data.account = {
            username: username,
            pass: pass
        };
    } else delete mp.storage.data.account;

    mp.events.callRemote('signin', username, pass);
});

mp.events.add('CLIENT::auth:restore', function (login) {
    if (new Date().getTime() - lastButAuth < 3000) {
        thisMenuCall.call('CEF::auth:update', JSON.stringify({error: 'Слишком быстро', errorInput: []}));
        return;
    }

    mp.events.callRemote('SERVER::auth:restorePassword', login);
});

mp.events.add("CLIENT::auth:changeActive", (idx) => {
  thisMenuCall.call("CEF::auth:changeActive", idx);
})

mp.events.add('CLIENT::auth:restorePassword', function (login, hash) {
  try {
    const url = `https://saintsworld.net/restore/?login=${login}&hash=${hash}`;
    logger.debug(`https://saintsworld.net/restore/?login=${login}&hash=${hash}`);

    global.restore = mp.browsers.new(url);
    global.restore.active = true;

    global.restorePageOpen = true;

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

mp.events.add('CLIENT::auth:reg', function (regData) {
    if (new Date().getTime() - lastButAuth < 3000) {
        //mp.events.call('notify', 4, 9, "Слишком быстро", 3000);
        thisMenuCall.call('CEF::auth:update', JSON.stringify({error: 'Слишком быстро', errorInput: []}));
        return;
    }
    lastButAuth = new Date().getTime();

    regData = JSON.parse(regData);
    var username = regData['login'].value,
        email = regData['mail'].value,
        pass1 = regData['password'].value,
        pass2 = regData['passwordrepeat'].value;

    if (checkLgin(username) || username.length > 50) {
        thisMenuCall.call('CEF::auth:update', JSON.stringify({error: 'Логин не соответствует формату или слишком длинный!', errorInput: [1]}));
        return;
    }

    if (pass1 != pass2 || pass1.length < 3) {
        thisMenuCall.call('CEF::auth:update', JSON.stringify({error: 'Ошибка при вводе пароля!', errorInput: [2,3]}));
        return;
    }

    mp.events.callRemote('signup', username, pass1, email);
});

mp.events.add('CLIENT::selectcharacter:enter', function (slot) {
    if (new Date().getTime() - lastButSlots < 1000) {
        mp.events.call('notify', 4, 9, "Слишком быстро", 3000);
        return;
    }
    lastButSlots = new Date().getTime();

    //mp.console.logInfo("CLIENT::selectcharacter:enter slot: "+slot);

    global.oscar2Menu.call('CLIENT::selectcharacter:update', JSON.stringify({show: false}));
    global.oscar2Menu.call('CLIENT::selectcharacter:callbackOnclose');
    //global.anyMenuHTML.closeAnyMenu("Oscar2", "auth");
    global.oscar2Menu.call('CEF::selectspawn:update', JSON.stringify({show: true}));
    mp.events.callRemote('selectchar', slot);

	/*if (auth != null) {
        auth.destroy();
        auth = null;

		mp.events.callRemote('selectchar', slot);
    }*/
});

mp.events.add('CLIENT::selectcharacter:create', function (slot, name, lastname, promo) {
    if (checkName(name) || !checkName2(name) || name.length > 25 || name.length <= 2) {
        mp.events.call('notify', 1, 9, 'Правильный формат имени: 3-25 символов и первая буква имени заглавная', 3000);
        return;
    }

    if (checkName(lastname) || !checkName2(lastname) || lastname.length > 25 || lastname.length <= 2) {
        mp.events.call('notify', 1, 9, 'Правильный формат фамилии: 3-25 символов и первая буква фамилии заглавная', 3000);
        return;
    }

    if (new Date().getTime() - lastButSlots < 3000) {
        mp.events.call('notify', 4, 9, "Слишком быстро", 3000);
        return;
    }
    lastButSlots = new Date().getTime();

    mp.events.callRemote('newchar', slot + 1, name, lastname, promo);
});

mp.events.add('CLIENT::selectcharacter:delete', function (slot, name, lastname, pass) {
    if (checkName(name) || name.length > 25 || name.length <= 2) {
        mp.events.call('notify', 1, 9, 'Правильный формат имени: 3-25 символов и первая буква имени заглавная', 3000);
        return;
    }

    if (checkName(lastname) || lastname.length > 25 || lastname.length <= 2) {
        mp.events.call('notify', 1, 9, 'Правильный формат фамилии: 3-25 символов и первая буква фамилии заглавная', 3000);
        return;
    }

    if (new Date().getTime() - lastButSlots < 3000) {
        mp.events.call('notify', 4, 9, 'Слишком быстро', 3000);
        return;
    }
    lastButSlots = new Date().getTime();

    mp.events.callRemote('delchar', slot, name, lastname, pass);
});

mp.events.add('transferChar', function (slot, name, lastname, pass) {
    if (checkName(name)) {
        mp.events.call('notify', 1, 9, 'Имя не соответствует формату или слишком длинное!', 3000);
        return;
    }

    if (checkName(lastname)) {
        mp.events.call('notify', 1, 9, 'Фамилия не соответствует формату или слишком длинное!', 3000);
        return;
    }

    if (new Date().getTime() - lastButSlots < 3000) {
        mp.events.call('notify', 4, 9, "Слишком быстро", 3000);
        return;
    }
    lastButSlots = new Date().getTime();

    mp.events.callRemote('transferchar', slot, name, lastname, pass);
});

mp.events.add('CLIENT::selectspawn:select', function (data) {
    if (new Date().getTime() - lastButSlots < 1000) {
        mp.events.call('notify', 4, 9, "Слишком быстро", 3000);
        return;
    }
        global.oscar2Menu.call('CEF::selectspawn:update', JSON.stringify({show: false}));
        global.anyMenuHTML.closeAnyMenu("Oscar2", "auth");
        mp.events.call('showHUD', false);
        lastButSlots = new Date().getTime();
        mp.events.callRemote('spawn', data);
});

mp.events.add('CLIENT::selectcharacter:buy', function (active, data) {
    if (new Date().getTime() - lastButSlots < 3000) {
        mp.events.call('notify', 4, 9, "Слишком быстро", 3000);
        return;
    }
    lastButSlots = new Date().getTime();
    mp.events.callRemote('SERVER::DONATE:BUYSLOT');
});

// events from server
mp.events.add('delCharSuccess', function (data) {
    //auth.execute(`delCharSuccess(${data})`);
    //mp.console.logInfo("delCharSuccess "+data);
    global.oscar2Menu.call('CEF::selectcharacter:updateSlot'); // Empty
    mp.events.call('characters.ped.remove', data);
});

mp.events.add('unlockSlot', function (data) {
    //auth.execute(`unlockSlot(${data})`);

  global.oscar2Menu.call('CEF::selectcharacter:updateSlot');

});

mp.events.add('toslots', function (data) {
    thisMenuCall.call('CEF::auth:update', JSON.stringify({show: false}));
    thisMenuCall.destroy();
    //global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
    mp.events.call('showHUD', false);
    mp.gui.cursor.visible = true;

    //auth = mp.browsers.new('package://cef/resources/views/menus/auth.html');
    //auth.execute(`toslots('${data}')`);

    let char = JSON.parse(data);

    let dirty = {
        active: false,
        title: "Управление персонажами",
        slot: 0,
        deleteIsShow: false,
        createIsShow: false,
        controlsIsShow: false,
        countCharacters: 0,
        characters: {
            0: {
                id: 1,
                name: "",
                firstName: "",
                secondName: "",
                fractionName: "",
                exp: 0,
                totalExp: 0,
                level: 0,
                money: 0,
                bank: 0,
                banned: false,
                isEmpty: true,
                uuid: 0,
                customization: {
                    custom: null,
                    gender: null,
                    torsoV: null,
                    torsoT: null,
                    hasMask: null,
                },
                isLocked: false,
                status: 'free'
            },
            1: {
                id: 2,
                name: "",
                firstName: "",
                secondName: "",
                fractionName: "",
                exp: 0,
                totalExp: 0,
                level: 0,
                money: 0,
                bank: 0,
                banned: false,
                isEmpty: true,
                uuid: 0,
                customization: {
                    custom: null,
                    gender: null,
                    torsoV: null,
                    torsoT: null,
                    hasMask: null,
                },
                isLocked: false,
                status: 'free'
            },
            2: {
                id: 3,
                name: "",
                firstName: "",
                secondName: "",
                fractionName: "",
                exp: 0,
                totalExp: 0,
                level: 0,
                money: 0,
                bank: 0,
                banned: false,
                isEmpty: true,
                uuid: 0,
                customization: {
                    custom: null,
                    gender: null,
                    torsoV: null,
                    torsoT: null,
                    hasMask: null,
                },
                isLocked: true,
                status: 'donate'
            },
        }
    };

    for (let i = 0; i <= 3; i++) {
        if(typeof char[i] === "object") {
            let character = char[i];

            if (character.type == 'ban') {
                dirty.characters[i] = {
                    banned: {
                        reason: character.reason,
                        admin: character.byAdmin,
                        banDate: character.banDate,
                        unBanDate: character.unBanDate,
                    }
                };
            } else {
                dirty.characters[i] = {
                    id: i + 1,
                    name: character.full_name,
                    firstName: character.first_name,
                    secondName: character.second_name,
                    fractionName: character.fraction_name,
                    exp: character.exp,
                    totalExp: character.lvl <= 15 ? character.lvl * 3 : 15 * 3 + 2 * (character.lvl - 15),
                    level: character.lvl,
                    money: character.money,
                    bank: character.bank,
                    isEmpty: false,
                    // uuid: 0,
                    customization: {
                        custom: character.customization.custom,
                        gender: character.customization.gender,
                        torsoV: character.customization.torsoV,
                        torsoT: character.customization.torsoT,
                        hasMask: character.customization.hasMask,
                    },
                    isLocked: false,
                    status: 'free'
                };
            }

            dirty.countCharacters++;
        }
    }

    var res = JSON.parse(data);

    var character = [];

    let count = 0;

    for(let i = 0; i < 3; i++) {
        if(typeof res[i] === "object"){
            var ch = res[i];
            // mp.console.logInfo("ch: "+JSON.stringify(ch));
          if (ch.type == 'ban') {
            character.push({
              status: 3,
              name: ch.full_name,
              exp: ch.exp,
              maxExp: ch.lvl <= 15 ? ch.lvl * 3 : 15 * 3 + 2 * (ch.lvl - 15),
              lvl: ch.lvl,
              banned: {
                reason: ch.reason,
                admin: ch.byAdmin,
                banDate: ch.banTime,
                unBanDate: ch.banUntil,
              }
            });
          } else {
            character.push({
              status: 0,
              name: ch.full_name,
              fraction: ch.fraction_name,
              exp: ch.exp,
              maxExp: ch.lvl <= 15 ? ch.lvl * 3 : 15 * 3 + 2 * (ch.lvl - 15),
              lvl: ch.lvl,
              cash: ch.money,
              bank: ch.bank,
            });
            count++;
          }
        }
        else {
            if(i == 2 && !res[i].length && res[i] == -1) {
                character.push({
                    status: 1,
                });
            }
            else {
                if(i == 0 || i == 1){
                    character.push({
                        status: 1,
                    });
                }
                else {character.push({
                        status: 2,
                        coinbuy: 100,
                    });
                }
            }
        }
    }

    var dict = {
        show: true,
        coin: res[res.length - 2],
        characterAll: character,
    };

    global.oscar2Menu.call('CLIENT::selectcharacter:update', JSON.stringify(dict));
    global.anyMenuHTML.openAnyMenu("Oscar2", "auth", false);

    mp.events.call('characters.init', dirty.countCharacters, JSON.stringify(dirty.characters));
});

mp.events.add('loginNotify', function(errortext, inputList){
    var list = JSON.parse(inputList);
    thisMenuCall.call('CEF::auth:update', JSON.stringify({error: errortext, errorInput: list}));
});

mp.events.add('spawnShow', function (data, addresses) {
    if (data === false) {
        if (respawn != null) {
            global.oscar2Menu.call('CLIENT::selectcharacter:update', JSON.stringify({show: false}));
            global.oscar2Menu.call('CEF::selectspawn:closeOnCallback');

           // respawn.destroy();
            //respawn = null;
        }
    }
    else {
       // respawn = mp.browsers.new('package://cef/resources/views/menus/respawn.html');
        //respawn.execute(`set('${data}')`);
        global.oscar2Menu.call('CLIENT::selectcharacter:update', JSON.stringify({show: false}));

        var res = JSON.parse(data);


        var vectors = JSON.parse(addresses);
        var streets = [], areas = [];

        for(let i = 0; i < vectors.length; i++){
            if(vectors[i].x == 0){
                streets.push('');
                areas.push('');
                continue;
            }
            var street = mp.game.pathfind.getStreetNameAtCoord(vectors[i].x, vectors[i].y, vectors[i].z, 0, 0);
            var area = mp.game.zone.getNameOfZone(vectors[i].x, vectors[i].y, vectors[i].z);

            streets.push(`${mp.game.ui.getStreetNameFromHashKey(street.streetName)}`);
            areas.push(`${mp.game.ui.getLabelText(area)}`);
        }

        var spawnDict = {
            show: true,
            list: [
                {status: res[0], address: areas[0], secondAddress: streets[0], img: "first", icon: "first", name: "Место выхода", notification: ""},
                {status: res[1], address: areas[1], secondAddress: streets[1], img: "fraction", icon: "fraction", name: "База фракции", notification: ""},
                {status: res[2], address: areas[2], secondAddress: streets[2], img: "house", icon: "house", name: "Дом", notification: ""},
                {status: res[3], address: areas[3], secondAddress: streets[3], img: "family", icon: "family", name: "Особняк семьи", notification: ""},
            ]
        }

        cam = mp.cameras.new('default', new mp.Vector3(-95, 19, 1682), new mp.Vector3(0, 0, 0), 70);
        cam.pointAtCoord(-95, 19, 0);
        cam.setActive(true);

        global.oscar2Menu.call('CEF::selectspawn:update', JSON.stringify(spawnDict));
    }

    //global.anyMenuHTML.closeAnyMenu("Oscar2", "auth");
});

mp.events.add('ready', () => {
  mp.game.cam.doScreenFadeOut(500);

  setTimeout(() => {
    global.loggedin = true;
    global.menuClose();
    mp.game.cam.renderScriptCams(false, false, 0, true, false);

    mp.events.call('loadNpc');
    mp.events.call('showHUD', true);
    mp.events.call('hideTun');
    mp.game.player.setHealthRechargeMultiplier(0);

    global.menu = mp.browsers.new('package://cef/menu.html');
    global.helpmenu = mp.browsers.new('package://cef/help.html');

    if (respawn != null) {
      respawn.destroy();
      respawn = null;
    }

    // if (auth != null) {
    //   auth.destroy();
    //   auth = null;
    // }

    mp.game.cam.doScreenFadeIn(1000);
  }, 1000);
});

function checkLgin(str) {
    return !(/^[a-zA-Z1-9]*$/g.test(str));
}

function checkName(str) {
    return !(/^[a-zA-Z]*$/g.test(str));
}

function checkName2(str) {
    let ascii = str.charCodeAt(0);
    if (ascii < 65 || ascii > 90) return false; // Если первый символ не заглавный, сразу отказ
    let bsymbs = 0; // Кол-во заглавных символов
    for (let i = 0; i != str.length; i++) {
        ascii = str.charCodeAt(i);
        if (ascii >= 65 && ascii <= 90) bsymbs++;
    }
    if (bsymbs > 2) return false; // Если больше 2х заглавных символов, то отказ. (На сервере по правилам разрешено иметь Фамилию, например McCry, то есть с приставками).
    return true; // string (имя или фамилия) соответствует
}

mp.events.add('authNotify', (type, layout, msg, time) => {
    // if(auth != null) auth.execute(`notify(${type},${layout},'${msg}',${time})`);
    // else mp.events.call('notify', type, layout, msg, time);
    //mp.events.call('notify', type, layout, msg, time);
    global.oscar2Menu.call('CLIENT::selectcharacter:update', JSON.stringify({error: msg, errorInput: []}));
});
