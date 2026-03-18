const thisMenu = 'Craft';
const thisHTML = 'Oscar2';
const thisMenuCall = global.oscar2Menu;

localplayer = mp.players.local;

const callbackOnClose = () => {
    global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
    thisMenuCall.call("CEF::craft:closeOnCallback");
};

mp.events.add('CLIENT::craft:update', (data) => {
    global.anyMenuHTML.openAnyMenu(thisHTML, thisMenu, callbackOnClose);
    thisMenuCall.call("CEF::craft:update",data);
});

mp.events.add('CLIENT::craft:close', () => {
    callbackOnClose();
});
mp.events.add('CLIENT::craft:updaterecept', () => {
    global.anyEvents.SendServer(() => mp.events.callRemote("SERVER::craft:updaterecept"));
});
mp.events.add('CLIENT::craft:changeActiveTab', (id) => {
    global.anyEvents.SendServer(() => mp.events.callRemote("SERVER::craft:changeTab", id));
});
mp.events.add('CLIENT::craft:openRecipeInfo', () => {
    global.anyEvents.SendServer(() => mp.events.callRemote("SERVER::craft:openRecipeInfo"));
});
mp.events.add('CLIENT::craft:deposit', (count) => {
    global.anyEvents.SendServer(() => mp.events.callRemote("SERVER::craft:deposit", count));
});

mp.events.add('CLIENT::craft:withdraw', (count) => {
    global.anyEvents.SendServer(() => mp.events.callRemote("SERVER::craft:withdraw", count));
});
mp.events.add('CLIENT::craft:openShop', (count) => {
    global.anyEvents.SendServer(() => mp.events.callRemote("SERVER::craft:openShop", count));
});
mp.events.add('CLIENT::craft:openShopModalChangeItem', (elem) => {
    global.anyEvents.SendServer(() => mp.events.callRemote("SERVER::craft:openShopModalChangeItem", elem));
});
mp.events.add('CLIENT::craft:addShopItem', (count) => {
    global.anyEvents.SendServer(() => mp.events.callRemote("SERVER::craft:addShopItem", count));
});
mp.events.add('CLIENT::craft:getShopItem', (count) => {
    global.anyEvents.SendServer(() => mp.events.callRemote("SERVER::craft:getShopItem", count));
});
mp.events.add('CLIENT::craft:getAllShopItem', (count) => {
    global.anyEvents.SendServer(() => mp.events.callRemote("SERVER::craft:getAllShopItem", count));
});
mp.events.add('CLIENT::craft:openDetails', (count) => {
    global.anyEvents.SendServer(() => mp.events.callRemote("SERVER::craft:openDetails", count));
});
mp.events.add('CLIENT::craft:startShop', () => {
    global.anyEvents.SendServer(() => mp.events.callRemote("SERVER::craft:startShop",));
});
mp.events.add('CLIENT::craft:createMachine', () => {
    global.anyEvents.SendServer(() => mp.events.callRemote("SERVER::craft:createMachine"));
});
mp.events.add('CLIENT::craft:getMachine', () => {
    global.anyEvents.SendServer(() => mp.events.callRemote("SERVER::craft:getMachine"));
});
mp.events.add('CLIENT::craft:buyShop', (id) => {
    global.anyEvents.SendServer(() => mp.events.callRemote("SERVER::craft:buyShop", id));
});
mp.events.add('CLIENT::craft:getAllShopItemResult', (recept) => {
    global.anyEvents.SendServer(() => mp.events.callRemote("SERVER::craft:getAllShopItemResult", recept));
});
mp.events.add('CLIENT::craft:getShopItemResult', (recept) => {
    global.anyEvents.SendServer(() => mp.events.callRemote("SERVER::craft:getShopItemResult", recept));
});
