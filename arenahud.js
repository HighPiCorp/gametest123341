const thisMenu = 'ArenaHUD';
const thisHTML = 'Oscar2';
const thisMenuCall = global.oscar2Menu;

localplayer = mp.players.local;

const callbackOnClose = () => {
    global.anyMenuHTML.closeAnyHUDElement(thisHTML, thisMenu);
    thisMenuCall.call("CEF::arenahud:closeOnCallback");
};


mp.events.add('CLIENT::arenahud:update', (data) => {
    global.anyMenuHTML.openAnyHUDElement(thisHTML, thisMenu);
    thisMenuCall.call("CEF::arenahud:update",data);
});

mp.events.add('CLIENT::arenahud:close', (data) => {
    callbackOnClose()
});