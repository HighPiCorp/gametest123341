const thisMenu = 'shop';
const thisHTML = 'Oscar2';
const thisMenuCall = global.oscar2Menu;

const callbackOnClose = () => {
  thisMenuCall.call('CEF::shop:closeOnCallback');
};

mp.events.add('CLIENT::shop:open', (title, list) => {
  try {
    if (typeof list !== 'object') {
      list = JSON.parse(list);
    }

    thisMenuCall.call('CEF::shop:update', {
      show: true,
      title,
      list,
    });

    global.anyMenuHTML.openAnyMenu(thisHTML, thisMenu, callbackOnClose);
  } catch (e) {
    logger.error(e);
  }
});

mp.events.add('CLIENT::shop:close', () => {
  try {
    global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
  } catch (e) {
    logger.error(e);
  }
});

mp.events.add('CLIENT::shop:buy', (buyType, key, index, count) => {
  try {
    global.anyEvents.SendServer(() => mp.events.callRemote('SERVER::shop:buy', buyType, parseInt(key), parseInt(index), parseInt(count)));
  } catch (e) {
    logger.error(e);
  }
});
