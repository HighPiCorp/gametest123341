const thisMenu = 'popup';
const thisHTML = 'FeST1VaL';
const thisMenuCall = global.festMenu;

global.popupOpen = false;

const callbackOnClose = (callback, type) => {
  thisMenuCall.call('popup:closeOnCallback', callback, type);
  global.popupOpen = false;
};

mp.events.add('popup::open', (callback, description) => {
  //mp.console.logInfo(`popup::open callback: ${callback} description: ${description}`);
  thisMenuCall.call('popup:open', callback, description);
  if (callback === 'tuningbuy') mp.events.call('hideTuningMenu');
  global.anyMenuHTML.openAnyMenu(thisHTML, thisMenu, () => callbackOnClose(callback, 'alert'));
  global.popupOpen = true;
});

mp.events.add('popup::callback', (callback, result) => {
  //mp.console.logInfo(`popup::callback: ${callback} result: ${result}`);

  if (callback === 'tuningbuy') {
    mp.events.call('tuningBuyComponent', result);
  } else {
    global.anyEvents.SendServer(() => mp.events.callRemote('SERVER::popup:callback', callback, result));
  }

  global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
  global.popupOpen = false;
});

mp.events.add('popup::openInput', (header, description, length, callback) => {
  // mp.console.logInfo(`popup::openInput callback: ${callback} header: ${header} description: ${description}`);
  thisMenuCall.call('popup:openInput', header, description, length, callback);
  global.anyMenuHTML.openAnyMenu(thisHTML, thisMenu, () => callbackOnClose(callback, 'input'));
});

mp.events.add('popup::callbackInput', (callback, result) => {
  // mp.console.logInfo(`popup::callbackInput: ${callback} result: ${result}`);

  if (callback === '') return;
  if (callback === 'setCruise') {
    mp.events.call('setCruiseSpeed', result);
  } else {
    global.anyEvents.SendServer(() => mp.events.callRemote('SERVER::popup:callbackInput', callback, result));
  }

  global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
});

mp.events.add('popup::close', () => {
  global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
  global.popupOpen = false;
});



mp.events.add('CLIENT::EMS:removeTattoo', (idx) => {
  mp.console.logInfo(`CLIENT::EMS:removeTattoo: idx: ${idx}`);

  global.anyEvents.SendServer(() => mp.events.callRemote('SERVER::EMS:removeTattoo', idx));

  global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
  global.popupOpen = false;
});

mp.events.add('CLIENT::EMS:openRemoveTattoo', (data) => {
  mp.console.logInfo(`CLIENT::EMS:openRemoveTattoo: ${data}`);
  thisMenuCall.call('popup:openSelect', data);
  global.anyMenuHTML.openAnyMenu(thisHTML, thisMenu, () => callbackOnClose("removeTattoo", 'select'));
  global.popupOpen = true;
});

