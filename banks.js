const thisHTML = 'Oscar';
const thisMenuCall = global.new_menu;
const thisMenu = 'bank';

const callbackOnClose = () => {
  thisMenuCall.call('bank:closeOnCallback');
};

mp.events.add('bank::open', (data) => {
  global.new_menu.call('bank:update', JSON.parse(data));
  global.anyMenuHTML.openAnyMenu(thisHTML, thisMenu, callbackOnClose);
});

mp.events.add('bank::reopen', (data) => {
  global.new_menu.call('bank:update', JSON.parse(data));
  global.anyMenuHTML.openAnyMenu(thisHTML, thisMenu, callbackOnClose);
});

mp.events.add('bank::close', () => {
//  global.new_menu.call('bank:close');
  global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
});

mp.events.add('bank::closeModal', () => {
  // mp.console.logInfo('bank:closeModal');
  global.new_menu.call('bank:closeModal');
});

mp.events.add('bank::updateMoney', (balance) => {
  // mp.console.logInfo(`bank:updateMoney: ${balance}`);
  global.new_menu.call('bank:updateMoney', JSON.parse(balance));
});

mp.events.add('bank::openModalTest', (data) => {
  global.new_menu.call('bank:openModal', data);
});

mp.events.add('bank::putMoney', (value) => {
  global.anyEvents.SendServer(() => mp.events.callRemote('bank::putMoney', value));
});

mp.events.add('bank::withdrawMoney', (value) => {
  global.anyEvents.SendServer(() => mp.events.callRemote('bank::withdrawMoney', value));
});

mp.events.add('bank::getPlayerCheck', (value) => {
  global.anyEvents.SendServer(() => mp.events.callRemote('bank::getPlayerCheck', value));
});

mp.events.add('bank::sendMoneyToPlayer', (value) => {
  global.anyEvents.SendServer(() => mp.events.callRemote('bank::sendMoneyToPlayer', value));
});

mp.events.add('bank::payHouse', (check, value) => {
  global.anyEvents.SendServer(() => mp.events.callRemote('bank::payHouse', check, value));
});

mp.events.add('bank::payBiz', (check, value) => {
  global.anyEvents.SendServer(() => mp.events.callRemote('bank::payBiz', check, value));
});

mp.events.add('bank::getTargetSim', (value) => {
  global.anyEvents.SendServer(() => mp.events.callRemote('bank::getTargetSim', value));
});

mp.events.add('bank::sendMoneyToTargetSim', (value) => {
  global.anyEvents.SendServer(() => mp.events.callRemote('bank::sendMoneyToTargetSim', value));
});

mp.events.add('bank::putMoneyFraction', (value) => {
  global.anyEvents.SendServer(() => mp.events.callRemote('bank::putMoneyFraction', value));
});

mp.events.add('bank::withdrawMoneyFraction', (value) => {
  global.anyEvents.SendServer(() => mp.events.callRemote('bank::withdrawMoneyFraction', value));
});

mp.events.add('bank::putMoneyFamily', (value) => {
  global.anyEvents.SendServer(() => mp.events.callRemote('bank::putMoneyFamily', value));
});

mp.events.add('bank::withdrawMoneyFamily', (value) => {
  global.anyEvents.SendServer(() => mp.events.callRemote('bank::withdrawMoneyFamily', value));
});

mp.events.add('console::logError', (text) => {
  mp.console.logError(text);
});
