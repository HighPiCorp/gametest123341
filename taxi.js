const thisMenu = 'taxi';
const thisHTML = 'FeST1VaL';
const thisMenuCall = global.festMenu;

let taxiClient = null;
let taxiGpsBlip = null;

global.taxiPriceOpen = false;

const callbackOnClose = () => {
  thisMenuCall.call('CEF::taxi:closeOnCallback');
  mp.events.callRemote('SERVER::TAXI:closeMenu');
};

mp.events.add('CLIENT::TAXI:openTaxiMenu', (data) => {
  thisMenuCall.call('CEF::taxi:open', data);
  global.anyMenuHTML.openAnyMenu(thisHTML, thisMenu, callbackOnClose);
});

mp.events.add('CLIENT::TAXI:updateMenuOrders', (data) => {
  thisMenuCall.call('CEF::taxi:open', data);
});

mp.events.add('CLIENT::taxi:acceptOrder', (order) => {
  const ord = JSON.parse(order);
  global.anyEvents.SendServer(() => mp.events.callRemote('SERVER::TAXI:ACCEPT_ORDER', ord.id));
  thisMenuCall.call('CEF::taxi:close');
  global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
});

mp.events.add('CLIENT::taxi:setRoutePrice', (order) => {
  global.taxiPriceOpen = true;
  thisMenuCall.call('CEF::taxi:openOrder', order);
  global.anyMenuHTML.openAnyMenu(thisHTML, thisMenu, callbackOnClose);
});

mp.events.add('CLIENT::taxi:makeOrder', (orderId, price) => {
  global.anyEvents.SendServer(() => mp.events.callRemote('SERVER::TAXI:OFFER_PAYMENT', orderId, price));
  thisMenuCall.call('CEF::taxi:close');
  global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
  global.taxiPriceOpen = false;
});

mp.events.add('CLIENT::taxi:close_maker_order', () => {
  thisMenuCall.call('CEF::taxi:close');
  global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
  global.taxiPriceOpen = false;
});

mp.events.add('CLIENT::taxi:finishOrder', () => {
  thisMenuCall.call('CEF::taxi:finishOrder');
});

mp.events.add('CLIENT::taxi:closeMenu', () => {
  mp.events.callRemote('SERVER::TAXI:closeMenu');
  global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
});

mp.events.add('CLIENT::TAXI:UPDATE_GPS_BLIP', (pos) => {
  if(taxiGpsBlip == null){
    taxiGpsBlip = mp.blips.new(
      198,
      pos,
      {
        name: 'Водитель',
        scale: 1,
        color: 49,
        alpha: 255,
        drawDistance: 100,
        shortRange: false,
        rotation: 0,
        dimension: 0,
      },
    );
  }
  else {
    taxiGpsBlip.setCoords(pos);
  }
});

mp.events.add('CLIENT::TAXI:DELETE_GPS_BLIP', () => {
  if(taxiGpsBlip != null)
    taxiGpsBlip.destroy();

    taxiGpsBlip = null;
});


mp.events.add('CLIENT::TAXI:SET_CLIENT_ARROW', (entity) => {
  taxiClient = entity;
});

mp.events.add('CLIENT::TAXI:DELETE_CLIENT_ARROW', () => {
  taxiClient = null;
});

mp.events.add('render', function(){
  if(taxiClient != null){
    mp.players.forEachInStreamRange(
      (player, id) => {
       if(taxiClient != null && player == taxiClient){
        mp.game.graphics.drawMarker(
          2,
          player.position.x, player.position.y, player.position.z + 2.5,
          0, 0, 0,
          180, 0, 0,
          1, 1, 1,
          107, 107, 250, 120,
          false, true, 1,
          false, null, null, false,
        );
       }
      }
    );
  }
});
