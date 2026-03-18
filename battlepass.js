const thisMenu = 'BattlePass';
const thisHTML = 'Oscar2';
const thisMenuCall = global.oscar2Menu;
lastCheck = new Date().getTime();

const callbackOnClose = () => {
  thisMenuCall.call('CEF::bp:closeOnCallback');
};

mp.events.add('CLIENT::bp:update', (data) => {
  thisMenuCall.call('CEF::bp:update', data);
  global.anyMenuHTML.openAnyMenu(thisHTML, thisMenu, callbackOnClose);
});

mp.events.add('CLIENT::bp:close', () => {
  callbackOnClose();
  global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
});

mp.events.add('CLIENT::bp:changeTab', (menu) => {
  global.anyEvents.SendServer(() => mp.events.callRemote('SERVER::bp:changeTab', menu));
});

// mp.events.add('CLIENT::bp:openInfo', () => {
//   mp.console.logInfo("CLIENT::bp:openInfo");
// });
//
// mp.events.add('CLIENT::bp:changeInfoTab', (tab) => {
//   mp.console.logInfo("CLIENT::bp:changeInfoTab: "+tab);
// });

mp.events.add('CLIENT::bp:selectQuest', (id) => {
  global.anyEvents.SendServer(() => mp.events.callRemote('SERVER::bp:selectQuest', id));
});

mp.events.add('CLIENT::bp:getItem', (bpType, rewardlvl, scrollCurrentLVL) => {
  global.anyEvents.SendServer(() => mp.events.callRemote('SERVER::bp:getItem', bpType, rewardlvl, scrollCurrentLVL));
});

mp.events.add('CLIENT::bp:listScroll', (lvl) => {
  // global.anyEvents.SendServer(() => mp.events.callRemote('SERVER::bp:listScroll', lvl));

  mp.events.callRemote('SERVER::bp:listScroll', lvl);
});

mp.events.add('CLIENT::bp:scrollRight', (lvl) => {
  global.anyEvents.SendServer(() => mp.events.callRemote('SERVER::bp:scrollRight', lvl));
});

mp.events.add('CLIENT::bp:takeItem', (bpType, rewardlvl, rewardSeason) => {
  global.anyEvents.SendServer(() => mp.events.callRemote('SERVER::bp:takeItem', bpType, rewardlvl, rewardSeason));
});

mp.events.add('CLIENT::bp:sprayItem', (bpType, rewardlvl, rewardSeason) => {
  global.anyEvents.SendServer(() => mp.events.callRemote('SERVER::bp:sprayItem', bpType, rewardlvl, rewardSeason));
});

mp.events.add('CLIENT::bp:buyBPPayment', (name) => {
  callbackOnClose();
  global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);

  global.anyEvents.SendServer(() => mp.events.callRemote('SERVER::bp:buyBPPayment', name));
});

mp.events.add('CLIENT::bp:buyExpPayment', (index) => {
  global.anyEvents.SendServer(() => mp.events.callRemote('SERVER::bp:buyExpPayment', index));
});

mp.events.add('CLIENT::bp:getRewardQuest', (id, type) => {
  global.anyEvents.SendServer(() => mp.events.callRemote('SERVER::bp:getRewardQuest', id, type));
});

mp.events.add('CLIENT::bp:skipQuest', (id, type) => {
  global.anyEvents.SendServer(() => mp.events.callRemote('SERVER::bp:skipQuest', id, type));
});

mp.events.add('CLIENT::bp:changeInfoTab', (id) => {
  global.anyEvents.SendServer(() => mp.events.callRemote('SERVER::bp:changeInfoTab', id));
});

mp.events.add('CLIENT::bp:sprayAllReward', () => {
  global.anyEvents.SendServer(() => mp.events.callRemote('SERVER::bp:sprayAllReward'));
});

mp.events.add('CLIENT::bp:buyBPcoins', (login) => {
  try {
    mp.events.call("CLIENT::bp:close");

    const url = 'https://saintsworld.net/payment/?login=' + login + '&type=bpc'; //daniellik
    logger.debug('https://saintsworld.net/payment/?login=' + login + '&type=bpc');

    global.donate = mp.browsers.new(url);
    global.donate.active = true;

    global.donatePageOpen = true;

    global.donateExit = mp.browsers.new('package://RouleteDonate/exit.html');
    global.donateExit.active = false;

    global.donatePageExitOpen = true;

    setTimeout(() => {
      if (donatePageExitOpen) {
        global.donateExit.active = true;
      }
    }, 3000);
  }
  catch(e) { logger.error(e) }
});
