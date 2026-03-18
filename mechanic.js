const thisMenu = 'mechanic';
const thisHTML = 'Oscar2';
const thisMenuCall = global.oscar2Menu;

const callbackOnClose = () => {
  var data = {show: false};
  thisMenuCall.call('CEF::mechanic:update',JSON.stringify(data));
  thisMenuCall.call('CEF::mechanic:closeOnCallback');
  mp.events.callRemote('SERVER::MECHANIC:CLOSE_MENU');
};

mp.events.add('CLIENT::MECHANIC:OPEN_MECHANIC_MENU', (data) => {
  thisMenuCall.call('CEF::mechanic:update', data);
  global.anyMenuHTML.openAnyMenu(thisHTML, thisMenu, callbackOnClose);
});

mp.events.add('CLIENT::MECHANIC:UPDATE_MECHANIC_ORDERS', (data) => {
  thisMenuCall.call('CEF::mechanic:update', data);
});

mp.events.add('CLIENT::mechanic:route', (orderId) => {
  global.anyEvents.SendServer(() => mp.events.callRemote('SERVER::MECHANIC:ACCEPT_CALL', orderId));
  global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
});

mp.events.add('CLIENT::mechanic:repair', (id, price) => {
  global.anyEvents.SendServer(() => mp.events.callRemote('SERVER::MECHANIC:REPAIR', id, price));
  global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
});

mp.events.add('CLIENT::mechanic:fuel', (id, fuel, price) => {
  global.anyEvents.SendServer(() => mp.events.callRemote('SERVER::MECHANIC:REFILL', id, fuel, price));
  global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
});

mp.events.add('CLIENT::mechanic:hide', () => {
  var data = {show: false};
  thisMenuCall.call('CEF::mechanic:update',JSON.stringify(data));
  global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
  mp.events.callRemote('SERVER::MECHANIC:CLOSE_MENU');
});

mp.events.add('CLIENT::MECHANIC:UPDATE_MODAL_MENU', (data) => {
  thisMenuCall.call('mechanicmodal::update', data);

  if (typeof data !== 'object') {
    data = JSON.parse(data);
  }

  if (data.show) {
    global.anyMenuHTML.openAnyHUDElement(thisHTML, thisMenu);
  } else {
    global.anyMenuHTML.closeAnyHUDElement(thisHTML, thisMenu);
  }
});


