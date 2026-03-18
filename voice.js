const Use3d = true;
const UseAutoVolume = false;
const MaxRange = 10.0;

global.enableMicrophone = () => {
    if (global.chatActive || !global.loggedin) return;

    if (localplayer.getVariable('voice.muted') == true) return;

    if (mp.voiceChat.muted) {
        mp.voiceChat.muted = false;
        mp.gui.execute(`HUD.mic=${true}`);
        localplayer.playFacialAnim("mic_chatter", "mp_facial");
    }
}

global.disableMicrophone = () => {
    if (!global.loggedin) return;
    if (!mp.voiceChat.muted) {
        mp.voiceChat.muted = true;
        mp.gui.execute(`HUD.mic=${false}`);
        localplayer.playFacialAnim("mood_normal_1", "facials@gen_male@variations@normal");

        //mp.voiceChat.cleanupAndReload(true, false, false); // voice fix (если все равно проблемы с войсом расскоментируйте это)
    }
}

/*mp.keys.bind(getKeyBy('MICRO'), true, enableMicrophone);
mp.keys.bind(getKeyBy('MICRO'), false, disableMicrophone);
*/
let g_voiceMgr =
  {
      listeners: [],

      add: function (player, notify) {
          if (this.listeners.indexOf(player) === -1) {
              if (notify) mp.events.callRemote("add_voice_listener", player);

              this.listeners.push(player);
              player.voice3d = true;
              player.voiceVolume = 0.0;
              player.isListening = true;
          } else {
            //mp.console.logInfo(`cant ADD listener player: ${player.name}`);
          }
      },

      remove: function (player, notify) {
          let idx = this.listeners.indexOf(player);
          if (idx !== -1) {
              if (notify) mp.events.callRemote("remove_voice_listener", player);
              this.listeners.splice(idx, 1);
              player.isListening = false;
          } else {
           // mp.console.logInfo(`cant REMOVE listener player: ${player.name}`);
          }
      }
  };

mp.events.add("playerQuit", (player) => {
    if (player.isListening) g_voiceMgr.remove(player, false);
});

var PHONE = {
    target: null,
    status: false
};

mp.events.add('voice.mute', () => {
    disableMicrophone();
})
mp.events.add('voice.phoneCall', (target) => {
    if (!PHONE.target) {
        PHONE.target = target;
        PHONE.status = true;
        mp.events.callRemote("add_voice_listener", target);
        target.voiceVolume = 1.0;
        target.voice3d = false;
        g_voiceMgr.remove(target, false);
    }
});
mp.events.add("voice.phoneStop", () => {
    if (PHONE.target) {
        if (mp.players.exists(PHONE.target)) {
            let localPos = localplayer.position;
            const playerPos = PHONE.target.position;
            let dist = mp.game.system.vdist(playerPos.x, playerPos.y, playerPos.z, localPos.x, localPos.y, localPos.z);
            if (dist > MaxRange) mp.events.callRemote("remove_voice_listener", PHONE.target);
            else g_voiceMgr.add(PHONE.target, false);
        } else mp.events.callRemote("remove_voice_listener", PHONE.target);
        PHONE.target = null;
        PHONE.status = false;
    }
});

var lastVoiceFixTime = new Date().getTime();
var voiceFixTimer = setInterval(function () {
	if ((new Date().getTime() - lastVoiceFixTime) > (5 * 60 * 1000)) {
		if (mp.voiceChat.muted) {
			mp.voiceChat.cleanupAndReload(false, false, true);
			lastVoiceFixTime = new Date().getTime();
		}
	}
}, 30 * 1000);

mp.events.add('v_reload', () => {
	try {
    mp.voiceChat.cleanupAndReload(true, true, true);
    mp.events.call('notify', 2, 9, `Вы перезагрузили микрофон!`, 3000);
	} catch { }
});

mp.events.add('v_reload2', () => {
	try {
		mp.voiceChat.cleanupAndReload(false, false, true);
	} catch { }
});

mp.events.add('v_reload3', () => {
	try {
		mp.voiceChat.cleanupAndReload(true, false, false);
	} catch { }
});

mp.events.add('playerStartTalking', (player) => {
    if (PHONE.target != player) player.voice3d = true;
	player.playFacialAnim("mic_chatter", "mp_facial");

});

mp.events.add('playerStopTalking', (player) => {
	player.playFacialAnim("mood_normal_1", "facials@gen_male@variations@normal");
});

var localPos = global.localplayer.position;
var playerPos = global.localplayer.position;

setInterval(() => {
    localPos = global.localplayer.position;

    mp.players.forEachInStreamRange(player => {
        if (player != global.localplayer) {
            if (!player.isListening && (!PHONE.target || PHONE.target != player)) {
                const playerLVL = player.getVariable("GAME_LEVEL");
                if (playerLVL <= mp.storage.data.mutelvl)
                {
                    // mp.console.logInfo(`player.voiceVolume: ${player.voiceVolume} playerMuted: ${player.name} lvl: ${playerLVL} mp.storage.data.mutelvl: ${mp.storage.data.mutelvl}`);
                    g_voiceMgr.remove(player, true);
                    player.voiceVolume = 0;
                }
                else
                {
                    playerPos = player.position;
                    let dist = mp.game.system.vdist(playerPos.x, playerPos.y, playerPos.z, localPos.x, localPos.y, localPos.z);

                    if (dist <= MaxRange) {
                      g_voiceMgr.add(player, true);
                      player.voiceVolume = 1 - (dist / MaxRange);
                    }
                }
            }
        }
    });

    g_voiceMgr.listeners.forEach((player) => {
        if (player.handle !== 0) {
            playerPos = player.position;
            let dist = mp.game.system.vdist(playerPos.x, playerPos.y, playerPos.z, localPos.x, localPos.y, localPos.z);

            if (dist > MaxRange) g_voiceMgr.remove(player, true);
            else if (!UseAutoVolume) player.voiceVolume = 1 - (dist / MaxRange);

            const playerLVL = player.getVariable("GAME_LEVEL");
            if (playerLVL <= mp.storage.data.mutelvl)
            {
              // mp.console.logInfo(`1 playerMuted: ${player.name} lvl: ${playerLVL} mp.storage.data.mutelvl: ${mp.storage.data.mutelvl}`)
              // g_voiceMgr.remove(player, true);
              player.voiceVolume = 0;
            }
        }
        else g_voiceMgr.remove(player, true);
    });
}, 350);
