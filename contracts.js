const thisMenu = 'FamilyContracts';
const thisHTML = 'Oscar2';
const thisMenuCall = global.oscar2Menu;

const callbackOnClose = () => {
    thisMenuCall.call("CEF::familycontract:closeOnCallback");
    // thisMenuCall.call("CEF::familycontract:update", JSON.stringify({show: false}));

    global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
};

mp.events.add('CLIENT::FAMILY:OPEN_CONTRACTS', (info) => {
    global.anyMenuHTML.openAnyMenu(thisHTML, thisMenu, callbackOnClose);
    thisMenuCall.call("CEF::familycontract:update", info);
});

mp.events.add('CLIENT::familycontract:close', () => {
    // thisMenuCall.call("CEF::familycontract:closeOnCallback");
    global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
});

mp.events.add('CLIENT::familycontract:changeActive', (index) => {
    global.anyEvents.SendServer(() => mp.events.callRemote("SERVER::CONTRACTS:CHANGE_ACTIVE", index));
});

mp.events.add('CLIENT::FAMILY:UPDATE_CONTRACTS', (info) => {
    thisMenuCall.call("CEF::familycontract:change", info);
});

mp.events.add('CLIENT::familycontract:getContract', (index) => {
    global.anyEvents.SendServer(() => mp.events.callRemote("SERVER::CONTRACTS:START_CONTRACT", index));
    callbackOnClose();
});


