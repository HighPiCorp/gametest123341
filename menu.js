const thisMenu = 'business';
const thisHTML = 'Oscar2';
const thisMenuCall = global.oscar2Menu;

const callbackBriefOnClose = () => {
    var data = {shows: false};
    thisMenuCall.call('CEF::businessbrief:update', JSON.stringify(data));
    thisMenuCall.call('CEF::businessbrief:closeOnCallback');
    global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
};

const callbackMenuOnClose = () => {
    var data = {show: false};
    thisMenuCall.call('CEF::businessmenu:update', JSON.stringify(data));
    thisMenuCall.call('CEF::businessmenu:closeOnCallback');
    global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
    global.INTERACTIONCHECK = true;
};

mp.events.add('CLIENT::BUSINESS:OPEN_MENU', function(data){
    thisMenuCall.call('CEF::businessbrief:update', data);
    global.anyMenuHTML.openAnyMenu(thisHTML, thisMenu, callbackBriefOnClose);
    global.INTERACTIONCHECK = false;
});

mp.events.add('CLIENT::bussinesbrief:buy', function(){
    global.anyEvents.SendServer(() => mp.events.callRemote("SERVER::BUSSINESS:buyBussiness"));
    global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
    global.INTERACTIONCHECK = true;
});

mp.events.add('CLIENT::bussinesbrief:close', function(){
    global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
    global.INTERACTIONCHECK = true;
});

mp.events.add('CLIENT::BUSINESS:OPEN_MANAGE_MENU', function(data){
    thisMenuCall.call('CEF::businessmenu:update', data);
    global.anyMenuHTML.openAnyMenu(thisHTML, thisMenu, callbackMenuOnClose);
    global.INTERACTIONCHECK = false;
});

mp.events.add('CLIENT::businessmenu:buy', function(data){
    var res = JSON.parse(data);
    res.forEach(arr => {
        if(arr != null){
            mp.events.callRemote("SERVER::BUSINESS:ORDER_PRODUCT", arr.key, arr.orderPieces);
        }
    });

    global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
    global.INTERACTIONCHECK = true;
});

mp.events.add('CLIENT::businessmenu:sell', function(){
    global.anyEvents.SendServer(() => mp.events.callRemote("SERVER::BUSINESS:SELL_BUSINESS"));
    global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
    global.INTERACTIONCHECK = true;
});

mp.events.add('CLIENT::businessmenu:topupMoney', function(){
    global.anyEvents.SendServer(() => mp.events.callRemote("SERVER::BUSINESS:TOPUP"));
    //global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
});

mp.events.add('CLIENT::businessmenu:widthdrawMoney', function(){
    global.anyEvents.SendServer(() => mp.events.callRemote("SERVER::BUSINESS:WIDTHDRAW"));
    //global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
});

mp.events.add('CLIENT::businessmenu:changeMarkup', function(perc){
    global.anyEvents.SendServer(() => mp.events.callRemote("SERVER::BUSINESS:SET_MARKUP", perc));
   // global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
});
mp.events.add('CLIENT::businesscontrol:hide', function(){
    thisMenuCall.call('CEF::businessmenu:closeOnCallback');
    global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
    global.INTERACTIONCHECK = true;
});

mp.events.add('CLIENT::businessmenu:close', function(){
    thisMenuCall.call('CEF::businessmenu:closeOnCallback');
    global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
    global.INTERACTIONCHECK = true;
});


mp.events.add('CLIENT::businessmenu:changeChat', function(data){
    global.oldchat.active = data;
    if(data == true) global.oldchat.execute('show()');
    else global.oldchat.execute('hide()');
    mp.gui.chat.show(data);
});

mp.events.add('CLIENT::bussinesbrief:startWar', () => {
    global.anyEvents.SendServer(() => mp.events.callRemote("SERVER::MAFIA:START_BIZWAR"));
    thisMenuCall.call('CEF::businessbrief:closeOnCallback');
    global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
    global.INTERACTIONCHECK = true;
});
