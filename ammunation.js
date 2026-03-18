const thisMenu = 'gunshop';
const thisHTML = 'Oscar2';
const thisMenuCall = global.oscar2Menu;

const callbackOnClose = () => {
  thisMenuCall.call('CEF::gunshop:closeOnCallback');
};

mp.events.add('CLIENT::gunshop:open', (id, info) => {
  try {
    if (typeof info !== 'object') {
      info = JSON.parse(info);
    }

    thisMenuCall.call('CEF::gunshop:update', {
      show: true,
      gun: info.gun,
      items: info.items,
    });

    global.anyMenuHTML.openAnyMenu(thisHTML, thisMenu, callbackOnClose);
  } catch (e) {
    logger.error(e);
  }
});

mp.events.add('CLIENT::gunshop:close', () => {
  try {
    global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
  } catch (e) {
    logger.error(e);
  }
});

mp.events.add('CLIENT::gunshop:getGunsItems', (itemType) => {
  try {
    mp.events.callRemote('SERVER::gunshop:getGunsItems', itemType);
  } catch (e) {
    logger.error(e);
  }
});

mp.events.add('CLIENT::gunshop:setGunsItems', (id, info) => {
  try {
    if (typeof info !== 'object') {
      info = JSON.parse(info);
    }

    thisMenuCall.call('CEF::gunshop:update', {
      items: info.items,
    });
  } catch (e) {
    logger.error(e);
  }
});

mp.events.add('CLIENT::gunshop:getModificationsItems', (itemType, itemIndex, modificationIndex) => {
  try {
    mp.events.callRemote('SERVER::gunshop:getModificationsItems', itemType, itemIndex, modificationIndex);
  } catch (e) {
    logger.error(e);
  }
});

mp.events.add('CLIENT::gunshop:setModificationsItems', (id, info) => {
  try {
    if (typeof info !== 'object') {
      info = JSON.parse(info);
    }

    thisMenuCall.call('CEF::gunshop:update', {
      items: info.items,
    });
  } catch (e) {
    logger.error(e);
  }
});

mp.events.add('CLIENT::gunshop:buy', (type, categoryIndex, weaponIndex, count, price) => {
  try {
    global.anyEvents.SendServer(() => mp.events.callRemote('SERVER::gunshop:buy', type, parseInt(categoryIndex), parseInt(weaponIndex), parseInt(count)));
  } catch (e) {
    logger.error(e);
  }
});

mp.events.add('CLIENT::gunshop:buyAmmo', (type, ammoIndex, count) => {
  try {
    global.anyEvents.SendServer(() => mp.events.callRemote('SERVER::gunshop:buyAmmo', type, parseInt(ammoIndex), parseInt(count)));
  } catch (e) {
    logger.error(e);
  }
});

mp.events.add('CLIENT::gunshop:buyModification', (buyType, categoryIndex, weaponIndex, modIndex, itemIndex, count) => {
  try {
    global.anyEvents.SendServer(() => mp.events.callRemote('SERVER::gunshop:buyModification', buyType, parseInt(categoryIndex), parseInt(weaponIndex), parseInt(modIndex), parseInt(itemIndex), parseInt(count)));
  } catch (e) {
    logger.error(e);
  }
});
