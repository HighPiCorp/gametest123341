const thisMenu = 'Arena';
const thisHTML = 'Oscar2';
const thisMenuCall = global.oscar2Menu;

localplayer = mp.players.local;

const callbackOnClose = () => {
    mp.events.callRemote("SERVER::arena:closeOnCallback");
    global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
    thisMenuCall.call("CEF::arena:closeOnCallback");
};

mp.events.add('CLIENT::arena:update', (data) => {
    global.anyMenuHTML.openAnyMenu(thisHTML, thisMenu, callbackOnClose);
    thisMenuCall.call("CEF::arena:update",data);
});

mp.events.add('CLIENT::arena:createLobby', (data) => {
    mp.events.callRemote("SERVER::arena:createLobby", data);
});
mp.events.add('CLIENT::arena:connectLobby', (id) => {
    mp.events.callRemote("SERVER::arena:connectLobby", id);
});
mp.events.add('CLIENT::arena:outLobby', () => {
    mp.events.callRemote("SERVER::arena:outLobby");
});
mp.events.add('CLIENT::arena:deleteLobby', () => {
    mp.events.callRemote("SERVER::arena:deleteLobby");
});
mp.events.add('CLIENT::arena:kickPlayer', (id) => {
    mp.events.callRemote("SERVER::arena:kickPlayer", id);
});
mp.events.add('CLIENT::arena:startLobby', () => {
    mp.events.callRemote("SERVER::arena:startLobby");
});
mp.events.add('CLIENT::arena:close', () => {
    callbackOnClose();
});
 
mp.events.add('setregdoll', () => {
    localplayer.setToRagdoll(1500, 1500, 0, true, true, true);
});
mp.events.add('arenatp', (wpos) => {
    mp.game.cam.doScreenFadeOut(500);
    localplayer.setAlpha(0);
    setTimeout(() => {
        localplayer.position = new mp.Vector3(wpos.x, wpos.y, wpos.z);
        localplayer.freezePosition(true);
    }, (500));
    setTimeout(() => {
        localplayer.setAlpha(255);
        mp.game.cam.doScreenFadeIn(500);
        localplayer.freezePosition(false);
    }, (1000));
});

