const thisMenu = 'FamilyManager';
const thisHTML = 'Oscar2';
const thisMenuCall = global.oscar2Menu;

const callbackOnClose = () => {
  thisMenuCall.call("CEF::familysetting:closeOnCallback");
  global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
};

mp.events.add('CLIENT::familysetting:close', () => {
  global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
});

mp.events.add('CLIENT::familysetting:leaveFamily', () => {
  global.anyEvents.SendServer(() => mp.events.callRemote("SERVER::FAMILY:LEAVE_FAMILY"));
  global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
});
mp.events.add('CLIENT::familycreate:expandFamily', () => {
  global.anyEvents.SendServer(() => mp.events.callRemote("SERVER::FAMILY:EXPAND_FAMILY"));
  global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
});

mp.events.add('CLIENT::FAMILY:OPEN_MANAGER', (info) => {
  global.anyMenuHTML.openAnyMenu(thisHTML, thisMenu, callbackOnClose);
  thisMenuCall.call("CEF::familysetting:update", info);
});

mp.events.add('CLIENT::FAMILY:UPDATE_MANAGER', (info) => {
  thisMenuCall.call("CEF::familysetting:update", info);
});

mp.events.add('CLIENT::familysetting:changeActiveItem', (index) => {
  global.anyEvents.SendServer(() => mp.events.callRemote("SERVER::FAMILY:CHANGE_RANK_ITEM", index));
});

mp.events.add('CLIENT::familysetting:addRank', () => {
  global.anyEvents.SendServer(() => mp.events.callRemote("SERVER::FAMILY:ADD_NEW_RANK"));
});

mp.events.add('CLIENT::familysetting:saveRank', (data) => {
  global.anyEvents.SendServer(() => mp.events.callRemote("SERVER::FAMILY:SAVE_RANK", data));
});

mp.events.add('CLIENT::familysetting:deleteRank', (index) => {
  global.anyEvents.SendServer(() => mp.events.callRemote("SERVER::FAMILY:DELETE_RANK", index));
});

mp.events.add('CLIENT::familysetting:invite', (id) => {
  global.anyEvents.SendServer(() => mp.events.callRemote("SERVER::FAMILY:INVITE", id));
});

mp.events.add('CLIENT::familysetting:saveAds', (text) => {
  global.anyEvents.SendServer(() => mp.events.callRemote("SERVER::FAMILY:SAVE_ABOUT", "ADS", text));
});

mp.events.add('CLIENT::familysetting:saveAbout', (about) => {
  global.anyEvents.SendServer(() => mp.events.callRemote("SERVER::FAMILY:SAVE_ABOUT", "ABOUT", about));
});

mp.events.add('CLIENT::familysetting:changeRank', (playerName, rank) => {
  global.anyEvents.SendServer(() => mp.events.callRemote("SERVER::FAMILY:CHANGE_RANK", playerName, rank));
});

mp.events.add('CLIENT::familysetting:expelPlayer', (playerName) => {
  global.anyEvents.SendServer(() => mp.events.callRemote("SERVER::FAMILY:UNINVITE", playerName));
});

mp.events.add('CLIENT::familysetting:deleteFamily', () => {
  global.anyEvents.SendServer(() => mp.events.callRemote("SERVER::FAMILY:DISBAND"));
  callbackOnClose();
});

mp.events.add('CLIENT::familysetting:changeimg', (chank, count, current, mimetype) => {
  global.anyEvents.SendServer(() => mp.events.callRemote("editFamilyImageChanks", chank, count, current, mimetype));
});

mp.events.add('CLIENT::familysetting:evacCar', (id) => {
  global.anyEvents.SendServer(() => mp.events.callRemote("SERVER::FAMILY:evacCar", id));
  callbackOnClose();
});

mp.events.add('CLIENT::familysetting:pointCar', (id) => {
  global.anyEvents.SendServer(() => mp.events.callRemote("SERVER::FAMILY:pointCar", id));
  callbackOnClose();
});

mp.events.add('CLIENT::familysetting:takeCar', (id) => {
  global.anyEvents.SendServer(() => mp.events.callRemote("SERVER::FAMILY:takeCar", id));
  callbackOnClose();
});

mp.events.add('CLIENT::familysetting:rewardPlayer', (playerName, amountReward, comment) => {
  mp.events.callRemote("SERVER::FAMILY:rewardPlayer", playerName, amountReward, comment);
  callbackOnClose();
});

