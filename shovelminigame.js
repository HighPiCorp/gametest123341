const thisMenu = 'ShovelGame';
const thisHTML = 'Oscar2';
const thisMenuCall = global.oscar2Menu;

localplayer = mp.players.local;
const callbackOnClose = () => {
    thisMenuCall.call("CEF::Craft:ShovelMiniGame:CloseOnCallback");
    global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
};

mp.events.add('CLIENT::Craft:ShovelMiniGame:Start', (data) => {
    global.anyMenuHTML.openAnyMenu(thisHTML, thisMenu, callbackOnClose);
    thisMenuCall.call("CEF::Craft:ShovelMiniGame:Update",data);
});

mp.events.add('CLIENT::Craft:ShovelMiniGame:Finish', () => {
    mp.events.callRemote('SERVER::Craft:ShovelMiniGame:Finish', true);
    callbackOnClose();
});
mp.events.add('CLIENT::Craft:ShovelMiniGame:Failed', () => {
    mp.events.callRemote('SERVER::Craft:ShovelMiniGame:Finish', false);
    callbackOnClose();
});
