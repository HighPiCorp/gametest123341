const thisMenu = 'FishShop';
const thisHTML = 'Oscar2';
const thisMenuCall = global.oscar2Menu;
var sellType = 0;

const callbackOnClose = () => {
  thisMenuCall.call('CEF::fishbuyer:closeOnCallback');
  global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
};

mp.events.add('CLIENT::fishbuyer:open', (list,type,_name) => {
  try {
    sellType = type;
    thisMenuCall.call('CEF::fishbuyer:update', {
      show: true,
      list: JSON.parse(list),
      name: _name,
    });

    global.anyMenuHTML.openAnyMenu(thisHTML, thisMenu, callbackOnClose);
  } catch (e) {
    logger.error(e);
  }
});

mp.events.add('CLIENT::fishbuyer:close', () => {
  try {
    thisMenuCall.call('CEF::fishbuyer:update', {
      show: false
    });
    global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
  } catch (e) {
    logger.error(e);
  }
});

mp.events.add('CLIENT::fishbuyer:sellAll', () => {
  try {
    switch (sellType)
    {
      case 1:
        global.anyEvents.SendServer(() => mp.events.callRemote('SERVER::trashbuyer:sellAll'));
        callbackOnClose();
        break;
      default:
        global.anyEvents.SendServer(() => mp.events.callRemote('SERVER::fishbuyer:sellAll'));
        callbackOnClose();
        break;
    }
  } catch (e) {
    logger.error(e);
  }
});

mp.events.add('CLIENT::fishbuyer:sell', (key, count) => {
  try {
    switch (sellType)
    {
      case 1:
        global.anyEvents.SendServer(() => mp.events.callRemote('SERVER::trashbuyer:sell', key, count));
        callbackOnClose();
        break;
      default:
        global.anyEvents.SendServer(() => mp.events.callRemote('SERVER::fishbuyer:sell', key, count));
        callbackOnClose();
        break;
    }
  } catch (e) {
    logger.error(e);
  }
});


