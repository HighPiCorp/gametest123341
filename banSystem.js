const localplayer = mp.players.local;

mp.events.add("CLIENT::ban:addInStorage", () => {
  try {
    if (mp.storage.data.banned === undefined || mp.storage.data.banned.isBanned === false) {
      mp.storage.data.banned = { isBanned: true };
      mp.storage.flush();
    }

    setTimeout(() => {
      mp.events.callRemote("SERVER::ban:kickPlayer");
    }, 5000);
  } catch (e) { logger.debug(e) }
})

mp.events.add("CLIENT::ban:removeFromStorage", () => {
  try {
    mp.storage.data.banned.isBanned = false;
    mp.storage.flush();
  } catch (e) { logger.debug(e) }
})

mp.events.add("CLIENT::ban:checkStorage", (id) => {
  try {
    if (mp.storage.data.banned.isBanned !== undefined) {
      if (mp.storage.data.banned.isBanned === true) mp.events.callRemote("SERVER::ban:kickPlayer");
    }

  } catch (e) { logger.debug(e) }
});
