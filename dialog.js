const thisMenu = 'dialog';
const thisHTML = 'Oscar2';
const thisMenuCall = global.oscar2Menu;

const callbackOnClose = () => {
    thisMenuCall.call('CEF::dialog:closeOnCallback');
    global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
    mp.events.call('DestroyCamToNPC');
    global.INTERACTIONCHECK = true;
};

mp.events.add('CLIENT::DIALOG:OPEN', (data) => {
    thisMenuCall.call('CEF::dialog:show', data);
    global.anyMenuHTML.openAnyMenu(thisHTML, thisMenu, callbackOnClose);
    global.INTERACTIONCHECK = false;
});

mp.events.add('CLIENT::dialog:click', (data) => {
    global.anyEvents.SendServer(() => mp.events.callRemote("SERVER::DIALOG:CALLBACK", data));
});

mp.events.add('CLIENT::dialog:hide', () => {
    mp.events.call('DestroyCamToNPC');
    global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
    global.INTERACTIONCHECK = false;
});

mp.events.add('CLIENT::DIALOG:CLOSE', () => {
    thisMenuCall.call('CEF::dialog:closeOnCallback');
    mp.events.call('DestroyCamToNPC');
    global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
    global.INTERACTIONCHECK = false;
});
