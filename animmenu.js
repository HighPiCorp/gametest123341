const thisMenu = 'animmenu';
const thisHTML = 'Oscar2';
const thisMenuCall = global.oscar2Menu;

global.animMenuOpen = false;
global.activeAnim = null;

const callbackOnClose = () => {
    if (global.animMenuOpen) {
        global.animMenuOpen = false;
        const data = {show: false};
        thisMenuCall.call('CEF::animation:update', JSON.stringify(data));
        thisMenuCall.call('CEF::animation:closeOnCallback');
        global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
    }
};

const AnimCategory = [
    {
    name: "Сесть/Лечь",
    list : [
        "Сидеть на корточках",
        "Сидеть на чем либо",
        "Сидеть в унынии",
        "Сидеть оглядываясь",
        "Сидеть облокотившись",
        "Сидеть в страхе",
        "Сидеть полулежа",
        "Сидеть задумавшись",
        "Лежать на спине",
        "Лежать в стиле Рок-н-Ролл",
        "Лежать схватившись за голову",
        "Смотреть на солнце",
        "Лежать пьяным",
        "Лежать на животе",
        "Развалиться на диване",
        "Притвориться мёртвым"
    ]},
    {name: "Социальные", list: [
        "Поднять руки",
        "Осмотреть и записать",
        "Лайк",
        "Воинское приветствие",
        "Крутить у виска двумя руками",
        "Королевское приветствие",
        "Понтоваться",
        "Двойной лайк",
        "Испугать",
        "Сдаться",
        "Медленно хлопать",
        "Мир",
        "Отказ",
        "Радость",
        "Показать рыбку",
        "Фэйспалм",
        "Показать курочк",
        "ОК",
        "Позвать за собой",
        "РОК",
        "Мир всем",
        "Руки за голову на коленях",
        "Руки вверх на коленях",
        "Сдаться 2",
        "Сидеть в унынии 2",
        "Сильно нервничать дёргая ногой",
        "Саркастично хлопать №2",
        "Женское неодобрение",
        "Ну на двоечку",
        "Женское неодобрение 2",
        "facepalm",
        "Нет",
        "Ты сумашедший!",
        "Отряхиваться",
        "Выразить респект",
        "Отпрявлять воздушные поцелуи",
        "Стучать в дверь (со звуком)",
        "Ставить пальцы вверх",
        "Ммм, брависимо",
        "Махать руками привлекая внимание",
        "Дождь из денег",
        "Кидаться козявкой",
        "Болельщик",
        "Как вы меня задолбали"
    ]},
    {name: "Физ. упражнения", list: [
        "Зарядка 1",
        "Зарядка 2",
        "Качать пресс",
        "Отжиматься",
        "Медитировать",
    ]},
    {name: "Неприличное", list: [
        "Показать средний палец",
        "Показать что-то ещё",
        "Ковыряться в носу",
        "Показать средний палец всем",
        "Показать средний палец яростно",
    ]},
    {name: "Стойка", list: [
        "Стоять, руки на поясе",
        "Размять руки",
        "Скрестить руки на груди",
        "Стоять, прогнать человека",
        "Стоять, отказать в проходе",
        "Показать бицепс 1",
        "Показать бицепс 2",
        "Показать бицепс 3",
        "Показать бицепс 4",
        "Показать бицепс 5",
        "Показать бицепс 6",
    ]},
    {name: "Танцы", list: [
        "Читать реп",
        "Танец волна",
        "Лезгинка",
        "Танец зумбы",
        "Активный танец на месте",
        "Танец качающийся (Танец кача)",
        "Танец на месте №1",
        "Танец на месте №2",
        "Танец на месте №3",
        "Танец на месте №4",
        "Танец на месте №5",
        "Танец на месте №6",
        "Скромный танец №1",
        "Скромный танец №2",
        "Клубный танец №1",
        "Танец Диско",
        "Танец Мачо",
        "Танец с палками (в сценарий можно)",
        "Танец с палками №2 (в сценарий можно)",
        "Танец с палками №3 (в сценарий можно)",
        "Танец на месте №7",
        "Низкий флекс",
        "Стриптиз 1",
        "Стриптиз 2",
        "Стриптиз 3",
        "Стриптиз 4",
        "Стриптиз 5",
        "Стриптиз 6",
        "Танец руками",
        "Танец робота",
        "Танец бедрами",
        "Танец паучка",
        "Танец Лейла",
        "Танец локтями",
        "Энергосберегающий танец",
        "Танец 'В отрыв'",
        "Шафл руками",
        "Танец пингвина",
        "Пританцовывать 2",
        "Пританцовывать 3",
        "Изображать dj",
    ]},
    {name: "Эмоции лица", list: [
        "Презрение",
        "Хмурость",
        "Подшофе",
        "Веселье",
        "Удивление",
        "Злость",
    ]},
    {name: "Cтили походки", list: [
        "Стремительный",
        "Уверенный",
        "Вразвалку",
        "Грустный",
        "Женственный",
        "Испуганный",
        "Быстрый",
    ]},
];

mp.events.add('CLIENT::ANIM_MENU:SHOW', (data) => {
    thisMenuCall.call('CEF::animation:update', JSON.stringify(data));
    global.anyMenuHTML.openAnyMenu(thisHTML, thisMenu, false);
    //global.menuOpened = false;
});

mp.keys.bind(Keys.VK_U, false, () => { // Animations selector
  if (!global.loggedin || global.menuOpened || global.circleOpen || global.chatActive || global.inTrunk || global.phoneOpen || global.popupOpen || global.statsOpen || global.editing || global.taxiPriceOpen || localplayer.getVariable('InDeath') == true || new Date().getTime() - lastCheck < 1000) return;

  if (!global.animMenuOpen) {
    global.animMenuOpen = true;
    //global.menuOpened = true;
    updateAnimMenu();
  }
 /* else {
    animMenuOpen = false;
    var data = {show: false};
    thisMenuCall.call('CEF::animation:update', JSON.stringify(data));
    thisMenuCall.call('CEF::animation:closeOnCallback');
    global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
  }*/
});

mp.events.add('CLIENT::animation:toogleAnimation', (anim, toggle) => {
    if (mp.players.local.isFalling()) return;

    global.anyEvents.SendServer(() => mp.events.callRemote('SERVER::ANIMATION:TOGGLE', anim, toggle));

    if (toggle) {
        if (!AnimCategory[6].list.includes(anim) && !AnimCategory[7].list.includes(anim))
            global.activeAnim = anim;

        if (mp.storage.data.animHistory.length >= 15)
        {
            while(mp.storage.data.animHistory.length >= 15)
                mp.storage.data.animHistory.shift();
        }

        mp.storage.data.animHistory.push(anim);
        updateAnimMenu();
    }
    else
    {
        global.activeAnim = null;
        updateAnimMenu();
    }
});

mp.events.add('CLIENT::animation:toogleFavourite', (anim) => {
    if(!mp.storage.data.animList.includes(anim)){
        mp.storage.data.animList.push(anim);
    }
    else {
        if (mp.storage.data.animList.includes(anim)){
            for(let i = 0; i < mp.storage.data.animList.length; i++) {
                if (mp.storage.data.animList[i] === anim) {
                    mp.storage.data.animList.splice(i, 1);
                    i--;
                }
            }
        }
    }

    updateAnimMenu();
});

function updateAnimMenu(show = true){
    var history = mp.storage.data.animHistory.slice().reverse();
    var data = {
        show: show,
        activeAnimation: global.activeAnim,
        animationList: [
            {title: "Избранные", list: mp.storage.data.animList},
            {title: "История", list: history},
        ],
    };

    AnimCategory.forEach((t) => {
        data.animationList.push({
            title: t.name,
            list: t.list
        });
     });

    thisMenuCall.call('CEF::animation:update', JSON.stringify(data));
    global.anyMenuHTML.openAnyMenu(thisHTML, thisMenu, callbackOnClose);
}


if (mp.storage.data.animList == undefined) {
    mp.storage.data.animList = [];
    mp.storage.flush();
}

if (mp.storage.data.animHistory == undefined) {
    mp.storage.data.animHistory = [];
    mp.storage.flush();
}


mp.keys.bind(Keys.VK_C, false, () => {
    if (global.activeAnim != null) {
        global.anyEvents.SendServer(() => mp.events.callRemote('SERVER::ANIMATION:TOGGLE', global.activeAnim, false));
        global.activeAnim = null;
    }
});
