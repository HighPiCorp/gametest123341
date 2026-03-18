const thisMenu = 'BuilderGame';
const thisHTML = 'Oscar2';
const thisMenuCall = global.oscar2Menu;

localplayer = mp.players.local;
const callbackOnClose = () => {
    thisMenuCall.call("CEF::buildergame:closeOnCallback");
    global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
    mp.events.callRemote('builder:fail_minigame', true);
};

mp.events.add('CLIENT::buildergame:start', (data) => {
    global.anyMenuHTML.openAnyMenu(thisHTML, thisMenu, callbackOnClose);
    thisMenuCall.call("CEF::buildergame:update",data);
});

mp.events.add('CLIENT::buildergame:win', () => {
    mp.events.callRemote('builder:finish_minigame');
    thisMenuCall.call("CEF::buildergame:closeOnCallback");
    global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
});
