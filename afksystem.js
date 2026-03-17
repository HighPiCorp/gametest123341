global.afkSecondsCount = 0;
let isAfk = false;

setInterval(function () {
    if(isAfk == true && afkSecondsCount == 0)
    {
        mp.events.callRemote('unAfkClient');
        isAfk = false;
    }
    if (!global.menuOpened) {
        if(isAfk) return;
        afkSecondsCount++;
        if (afkSecondsCount >= 900) {
            if(localplayer.getVariable('IS_ADMIN') == true) afkSecondsCount = 0;
            else {
              //mp.gui.chat.push('Вы были исключены из игры за AFK более 15 минут.');
              mp.events.callRemote('clientAFK');
              isAfk = true;
            }
        }
    }
}, 1000);
