const thisMenu = '[Admin]PromoCodes';
const thisHTML = 'FeST1VaL';
const thisMenuCall = global.festMenu;

const callbackOnClose = () => {
  thisMenuCall.call("CEF::promoCodes:close");
  global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
};

mp.events.add('CLIENT::promoCodes:open', (info) => {
    global.anyMenuHTML.openAnyMenu(thisHTML, thisMenu, callbackOnClose());
    thisMenuCall.call("CEF::promoCodes:open", info);
});

mp.events.add('CLIENT::promoCodes:close', () => {
  global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
});

mp.events.add('CLIENT::promoCodes:onSave', (info, edit = false) => {
  //mp.console.logInfo("info: "+info);
  global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
  mp.events.callRemote("SERVER::promoCodes:onSave", info, edit, false);
});

mp.events.add('CLIENT::promoCodes:onDelete', (id) => {
  global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
  mp.events.callRemote("SERVER::promoCodes:onDelete", id);
});

mp.events.add('CLIENT::promoCodes:takeItem', (name) => {
  mp.events.callRemote("SERVER::promoCodes:takeItem", name);
});

mp.events.add('CLIENT::promoCodes:setEditItem', (info) => {
  thisMenuCall.call("CEF::promoCodes:setEditItem", info);
});
