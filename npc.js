var npc;
var cam = null;

mp.events.add('loadNpc', () => {
  try {
      npc = mp.browsers.new('package://interfaces/ui/Npc/index.html');
  } catch (e) {
    logger.error(e);
  }
});

// dialog npc v2
mp.events.add('openDialogNpc_v2', (dialogId, title, text, answers) => {
  try {
    if (!global.loggedin || cuffed || localplayer.getVariable('InDeath') === true) return;

    global.menuOpen();

    var data = JSON.stringify({dialogId, title, text, answers});

    npc.execute(`npcv2.init('${data}')`);

    mp.gui.cursor.visible = true;
  } catch (e) {
      logger.error(e);
  }
});

mp.events.add('choiceNpc_v2', (id, answer) => {
  try {
    global.init.playSelectSound();
    global.menuClose();
    mp.events.callRemote('dialog_callback', id, answer);
    npc.execute('npcv2.close()');
    mp.gui.cursor.visible = false;
  } catch (e) {
    logger.error(e);
  }
});

mp.events.add('openDialogNpc', function (title, text, answers) {
  try {
    if (!global.loggedin || cuffed || localplayer.getVariable('InDeath') === true) return;

    global.menuOpen();

    var data = JSON.stringify({
      title: title,
      text: text,
      answers: answers
    });

    npc.execute('npc.init(\'' + data + '\')');

    mp.gui.cursor.visible = true;
  } catch (e) {
    logger.error(e);
  }
});

mp.events.add('choiceNpc', function (id) {
  try {
    global.init.playSelectSound()
    global.menuClose();
    mp.events.callRemote('choiceNpc', id);
    npc.execute('npc.close()');
    mp.gui.cursor.visible = false;
  } catch (e) {
    logger.error(e);
  }
});

// AUTOSCOOL

mp.events.add('openNpcAutoschoolDialog', (title, message, answers) => {
  try {
    global.menuOpenIfNotOpened();

    let data = JSON.stringify({
      title,
      message,
      answers
    });

    npc.execute('npcAutoschool.init(\'' + data + '\')');
    mp.gui.cursor.visible = true;
  } catch (e) {
    logger.error(e);
  }
});

mp.events.add('choiceNpcAutoschool', (id) => {
  try {
    global.menuClose();
    mp.events.callRemote('choiceNpcAutoschool', id, -1);
    npc.execute('npcAutoschool.close()');
    mp.gui.cursor.visible = false;
  } catch (e) {
    logger.error(e);
  }
});

// TABLET AUTOSCOOL

mp.events.add('choiceTabletAutoschool', (id, payType) => {
  try {
    if (payType == 1 || payType == 0)
      mp.events.callRemote('choiceNpcAutoschool', id, payType);
    else
      mp.events.callRemote('choiceNpcAutoschool', id, -1);
  } catch (e) {
    logger.error(e);
  }
});

mp.events.add('setAutoscoolTable', (page, data) => {
  try {
    npc.execute(`tabletAuto.set('${page}', ${data})`);
    mp.gui.cursor.visible = true;
  } catch (e) {
    logger.error(e);
  }
});

mp.events.add('autoscoolTableClose', () => {
  try {
    mp.gui.cursor.visible = false;
    mp.events.callRemote('autoscoolTableClose');
    global.menuCloseIfNotOpened();
  } catch (e) {
    logger.error(e);
  }
});

mp.events.add('client_autoscoolTableClose', () => {
  npc.execute(`tabletAuto.close();`);
});
