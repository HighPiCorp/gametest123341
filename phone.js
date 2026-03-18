const thisMenu = 'phone';
const thisHTML = 'Oscar2';
const thisMenuCall = global.oscar2Menu;

const callbackOnClose = () => {
  mp.events.callRemote("closePlayerMenu");
  global.anyMenuHTML.closeAnyHUDElement(thisHTML, thisMenu);
  thisMenuCall.call("CEF::phone:closeOnCallback");
};

mp.events.add('CLIENT::PHONE:CLOSE', (hide = false) => {
  var res = {show: false};

  thisMenuCall.call('CEF::phone:update', JSON.stringify(res));
  thisMenuCall.call('CEF::phone:closeOnCallback');

  if (!hide) {
    global.anyMenuHTML.closeAnyHUDElement(thisHTML, thisMenu);
    mp.gui.cursor.visible = false;
  }
});

mp.events.add('CLIENT::PHONE:OPEN_PHONE', (data) => {
  var d = new Date();
  var minutes = d.getMinutes();
  var hours = d.getHours();

  var res = JSON.parse(data);

  res.time = hours.toString().padStart(2, '0') + ":" + minutes.toString().padStart(2, '0');

  thisMenuCall.call('CEF::phone:update', JSON.stringify(res));

  global.anyMenuHTML.openAnyHUDElement(thisHTML, thisMenu, callbackOnClose);

  mp.gui.cursor.visible = true;
});

mp.events.add('CLIENT::PHONE:UPDATE_PHONE', (data, hide = false) => {
  var res = JSON.parse(data);
  if(!hide){
    res.show = true;
    thisMenuCall.call('CEF::phone:update', JSON.stringify(res));
  }
  else global.policeGarage.execute(`client_playMusic('package://cef/sounds/message.mp3', 0.1)`);
});

mp.events.add('CLIENT::PHONE:CANCEL_CALL', () => {
  thisMenuCall.call('CEF::phone:cancelCall');
});

mp.events.add('CLIENT::phone:saveChangeContact', (oldnum, item) => {
    var data = JSON.parse(item);
    global.anyEvents.SendServer(() => mp.events.callRemote("SERVER::PHONE:EDIT_CONTACT_INFO", oldnum, data.name, data.phoneNumber));
});

mp.events.add('CLIENT::phone:changeFavouriteContact', (item) => {
    var data = JSON.parse(item);
    global.anyEvents.SendServer(() => mp.events.callRemote("SERVER::PHONE:EDIT_CONTACT", data.phoneNumber, data.name, data.phoneNumber, data.isFavourite));
});

mp.events.add('CLIENT::phone:getCar', (item) => {
  var res = JSON.parse(item);

  global.anyEvents.SendServer(() => mp.events.callRemote("SERVER::PHONE:GET_CAR", res.id));
});

mp.events.add('CLIENT::phone:searchCar', (item) => {
  var res = JSON.parse(item);

  global.anyEvents.SendServer(() => mp.events.callRemote("SERVER::PHONE:SEARCH_CAR", res.id));
});

mp.events.add('CLIENT::phone:changeCarPriority', (item, idx) => {
  var res = JSON.parse(item);
  global.anyEvents.SendServer(() => mp.events.callRemote("SERVER::PHONE:CHANGE_CAR_PRIORITY", res.id, idx));
});

mp.events.add('CLIENT::phone:cancelTaxi', () => {
  global.anyEvents.SendServer(() => mp.events.callRemote("SERVER::TAXI:CANCEL_TAXI"));
});

mp.events.add('CLIENT::phone:messagePlayer', (number) => {
  global.anyEvents.SendServer(() => mp.events.callRemote("SERVER::PHONE:SEND_MESSAGE", parseInt(number), 'none'));
});

mp.events.add('CLIENT::phone:sendMessage', (contact, text) => {
  global.anyEvents.SendServer(() => mp.events.callRemote("SERVER::PHONE:SEND_MESSAGE", contact, text));
});

mp.events.add('CLIENT::phone:addContact', (name, number) => {
    global.anyEvents.SendServer(() => mp.events.callRemote("SERVER::PHONE:ADD_CONTACT", name, number));
    global.anyMenuHTML.openAnyHUDElement(thisHTML, thisMenu, callbackOnClose);
});

mp.events.add('CLIENT::phone:callPlayer', (number) => {
  global.anyEvents.SendServer(() => mp.events.callRemote("SERVER::PHONE:CALL_PLAYER", number));
});

mp.events.add('CLIENT::phone:cancelCall', () => {
  global.anyEvents.SendServer(() => mp.events.callRemote("SERVER::PHONE:CANCEL_CALL"));
});

mp.events.add('CLIENT::phone:getSIM', () => {
  global.anyEvents.SendServer(() => mp.events.callRemote("SERVER::PHONE:GET_SIM"));
});

mp.events.add('CLIENT::phone:setGPS', (name) => {
  global.anyEvents.SendServer(() => mp.events.callRemote("SERVER::PHONE:SET_GPS", name));
});

mp.events.add('CLIENT::phone:cancelGPS', () => {
  global.anyEvents.SendServer(() => mp.events.callRemote("SERVER::PHONE:CANCEL_GPS"));
});

mp.events.add('CLIENT::phone:taxiSearch', () => {
  global.anyEvents.SendServer(() => mp.events.callRemote("SERVER::TAXI:CALL_TAXI"));
});

mp.events.add('CLIENT::phone:sendAds', (text, price) => {
  global.anyEvents.SendServer(() => mp.events.callRemote("SERVER::ADS:SEND_ADVERT", text, price));
});

mp.events.add('CLIENT::phone:deleteAds', (ad) => {
  var res = JSON.parse(ad);

  if(res.id)
    global.anyEvents.SendServer(() => mp.events.callRemote("SERVER::ADS:DELETE_ADVERT", res.id));
  else if(res.mId)
    global.anyEvents.SendServer(() => mp.events.callRemote("SERVER::ADS:DELETE_ADVERT_IN", res.mId));
});

mp.events.add('CLIENT::phone:deleteContact', (number) => {
  global.anyEvents.SendServer(() => mp.events.callRemote("SERVER::PHONE:DELETE_CONTACT", number));
});

mp.events.add('CLIENT::phone:calling', (data) => {
  thisMenuCall.call('CEF::phone:playerCall', data);
});

mp.events.add('CLIENT::phone:acceptCall', (num) => {
  global.anyEvents.SendServer(() => mp.events.callRemote("SERVER::PHONE:ACCEPT_CALL"));
});

mp.events.add('CLIENT::phone:cancelRentCar', (info) => {
  global.anyEvents.SendServer(() => mp.events.callRemote("SERVER::RENTCAR:CANCEL_RENT"));
});

mp.events.add('CLIENT::phone:call', (data) => {
  thisMenuCall.call('CEF::phone:call', data);
});
