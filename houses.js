const thisMenu = 'FamilyHouse';
const thisHTML = 'Oscar2';
const thisMenuCall = global.oscar2Menu;

const callbackOnClose = () => {
    thisMenuCall.call("CEF::familyhouse:close");
    global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
};

mp.events.add('CLIENT::familyhouse:close', () => {
    global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
});

mp.events.add('CLIENT::FAMILY:OPEN_HOUSE', (info) => {
    global.anyMenuHTML.openAnyMenu(thisHTML, thisMenu, callbackOnClose);
    thisMenuCall.call("CEF::familyhouse:update", info);
});

mp.events.add('CLIENT::familyhouse:buy', () => {
    global.anyEvents.SendServer(() => mp.events.callRemote("SERVER::FAMILY:BUY_FAMILY_HOUSE"));
    global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
});
