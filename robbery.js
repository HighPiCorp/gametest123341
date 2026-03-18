global.robberyHTML = mp.browsers.new('package://interfaces/ui_new/Robbery/index.html');
global.robberyHTML.active = false;
global.robberyOpened = false;

const thisHTML = 'Robbery';
const thisMenuCall = global.robberyHTML;
const thisMenu = 'robbery';

global.robberyType = null;

const callbackOnClose = () => {

};

mp.events.add('CLIENT::robbery:start', (data) => {
  global.robberyHTML.active = true;
  global.robberyOpened = true;
  global.menuOpen();

  if (typeof data != 'object') {
    const tempData = JSON.parse(data);
    if (tempData.hasOwnProperty('type')) global.robberyType = tempData.type;
  }

  data = JSON.stringify(data);

  thisMenuCall.execute(`start(${data})`);
});

mp.events.add('CLIENT::robbery:breakPin', () => {
  mp.events.callRemote('SERVER::robbery:breakPin');
});

mp.events.add('CLIENT::robbery:close', () => {
  thisMenuCall.execute(`close()`);

  global.robberyHTML.active = false;
  global.robberyOpened = false
  global.menuClose();

  mp.events.callRemote('SERVER::robbery:end', false, global.robberyType);
});

mp.events.add('CLIENT::robbery:success', () => {
  global.robberyHTML.active = false;
  global.robberyOpened = false;
  global.menuClose();

  let address = global.init.getMapAddress(mp.players.local.position);
  mp.events.callRemote('SERVER::robbery:end', true, global.robberyType, address);
});

mp.events.add('CLIENT::robbery:failed', () => {
  global.robberyHTML.active = false;
  global.robberyOpened = false;
  global.menuClose();

  mp.events.callRemote('SERVER::robbery:end', false, global.robberyType);
});
