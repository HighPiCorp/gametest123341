const thisMenu = 'casino';
const thisHTML = 'Oscar2';
const thisMenuCall = global.oscar2Menu;

let blackJack = false;
let roulette = false;
let slots = false;

global.openCasino = false;

const callbackOnCloseCasino = () => {
    thisMenuCall.call('CEF::casino:closeOnCallback');
    global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
    mp.events.callRemote("SERVER::CASINO:EXIT_MENU");
   //hisMenuCall.call('CEF::interaction:show',  false);
    blackJack = false;
    roulette = false;
    slots = false;
    global.openCasino = false;
    global.INTERACTIONCHECK = false;
};

mp.events.add('CLIENT::casino:hide', () => {
    global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
    mp.events.callRemote("SERVER::CASINO:EXIT_MENU");

   // thisMenuCall.call('CEF::interaction:show',  false);
    blackJack = false;
    roulette = false;
    slots = false;
    mp.gui.cursor.visible = false;
    global.openCasino = false;
    global.INTERACTIONCHECK = false;
});

mp.events.add('CLIENT::casino:hidef', () => {
    global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
    thisMenuCall.call('CEF::casino:update', JSON.stringify({show: false}));
    roulette = false;
    blackJack = false;
    slots = false;
    mp.gui.cursor.visible = false;
    global.openCasino = false;
    global.INTERACTIONCHECK = false;
});

mp.events.add('CLIENT::casino:close', () => {
    if(roulette) mp.events.callRemote("SERVER::CASINO:EXIT_ROULETTE");
});

mp.events.add('CLIENT::casino:hide_roulette', () => {
    global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
    //thisMenuCall.call('CEF::interaction:show',  false);
    blackJack = false;
    roulette = false;
    slots = false;
    mp.gui.cursor.visible = false;
    global.openCasino = false;
    global.INTERACTIONCHECK = false;
});


mp.events.add('CLIENT::CASINO:OPEN', (data, type) => {
    switch(type) {
        case "SLOTS":
            slots = true;
            break;
        case "BLACKJACK":
            blackJack = true;
            break;
        case "ROULETTE":
            roulette = true;
            break;
        default:
            break;
    }

    global.openCasino = true;
    thisMenuCall.call('CEF::casino:update', data);
    global.INTERACTIONCHECK = false;
    global.anyMenuHTML.openAnyMenu(thisHTML, thisMenu, callbackOnCloseCasino);
    mp.gui.cursor.visible = true;
});

mp.events.add('CLIENT::CASINO:UPDATE', (data) => {
    var res = JSON.parse(data);

    thisMenuCall.call('CEF::casino:update', data);

    if(blackJack) {
        if((res.status && res.status == 0) || (res.time)) mp.gui.cursor.visible = true;
        else mp.gui.cursor.visible = false;
    }
});

mp.events.add('CLIENT::casino:buyChips', (count) => {
    global.anyEvents.SendServer(() => mp.events.callRemote("SERVER::CASINO:BUY_CHIPS", count));
});

mp.events.add('CLIENT::casino:sellChips', (count) => {
    global.anyEvents.SendServer(() => mp.events.callRemote("SERVER::CASINO:SELL_CHIPS", count));
});

mp.events.add('CLIENT::casino:placeBet', (count) => {
    if(blackJack) global.anyEvents.SendServer(() => mp.events.callRemote("SERVER::CASINO:PLACE_BET", count));
    else if(roulette) mp.events.call("casinoBet", count);
    else if(slots) global.anyEvents.SendServer(() => mp.events.callRemote("SERVER::SLOTS:RUN", count));
    mp.gui.cursor.visible = false;
});

mp.events.add('CLIENT::casino:addCard', () => {
    global.anyEvents.SendServer(() => mp.events.callRemote("SERVER::CASINO:GET_CARD"));
    mp.gui.cursor.visible = false;
});
mp.events.add('CLIENT::casino::cancelCard', () => {
    global.anyEvents.SendServer(() => mp.events.callRemote("SERVER::CASINO:CANCEL_CARD"));
    mp.gui.cursor.visible = false;
});
