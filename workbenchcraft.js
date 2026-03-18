const thisMenu = 'CraftMenu';
const thisHTML = 'Oscar2';
const thisMenuCall = global.oscar2Menu;

localplayer = mp.players.local;

const callbackOnClose = () => {
    global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
    thisMenuCall.call("CEF::craftmenu:closeOnCallback");
};

mp.keys.bind(Keys.VK_NUMPAD8, false, () => {
    //if (!loggedin || localplayer.getVariable('IS_ADMIN') !== true) return;
    //global.anyEvents.SendServer(() => mp.events.callRemote('SERVER::craftmenu:open'));
});

mp.events.add('CLIENT::craftmenu:close', (data) => {
    callbackOnClose();
})
mp.events.add('CLIENT::craftmenu:update', (data) => {
    global.anyMenuHTML.openAnyMenu(thisHTML, thisMenu, callbackOnClose);
    thisMenuCall.call("CEF::craftmenu:update",data);
});

mp.events.add('CLIENT::craftMenu:openShopModalChangeItem', (data) => {
    global.anyEvents.SendServer(() => mp.events.callRemote("SERVER::craftMenu:openShopModalChangeItem",data));
});

mp.events.add('CLIENT::craftmenu:addShopItem', (count) => {
    global.anyEvents.SendServer(() => mp.events.callRemote("SERVER::craftmenu:addShopItem",count));
});

mp.events.add('CLIENT::craftmenu:getShopItem', (count) => {
    global.anyEvents.SendServer(() => mp.events.callRemote("SERVER::craftmenu:getShopItem",count));
});
mp.events.add('CLIENT::craftMenu:openRecept', (recept) => {
    global.anyEvents.SendServer(() => mp.events.callRemote("SERVER::craftMenu:openRecept", recept));
});
mp.events.add('CLIENT::craftMenu:startShop', (recept) => {
    global.anyEvents.SendServer(() => mp.events.callRemote("SERVER::craftMenu:startShop", recept));
});
mp.events.add('CLIENT::craftMenu:getShopItemResult', (recept) => {
    global.anyEvents.SendServer(() => mp.events.callRemote("SERVER::craftMenu:getShopItemResult"));
});
