const thisMenu = 'roding';
const thisHTML = 'Oscar2';
const thisMenuCall = global.oscar2Menu;

global.roding = false;

mp.events.add('CLIENT::FISHING', (toggle) => {
    global.roding = toggle;
});

const callbackOnClose = () => {
    global.roding = false;
    thisMenuCall.call('CEF::fishinggame:closeOnCallback');
    mp.events.callRemote("SERVER::ROD:FAILED");
    global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
    global.INTERACTIONCHECK = true;
};

mp.events.add('CLIENT::fising:startGame', (data) => {
    thisMenuCall.call('CEF::fishinggame:start', data);
    global.anyMenuHTML.openAnyMenu(thisHTML, thisMenu, callbackOnClose);
    global.INTERACTIONCHECK = false;
});

mp.events.add('CLIENT::fishinggame:failed', () => {
    mp.events.callRemote("SERVER::ROD:FAILED");
    global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
    global.INTERACTIONCHECK = false;
    global.roding = false;
});

mp.events.add('CLIENT::fishinggame:win', () => {
    mp.events.callRemote("SERVER::ROD:GIVE_FISH");
    global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
    global.INTERACTIONCHECK = false;
    global.roding = false;
});

