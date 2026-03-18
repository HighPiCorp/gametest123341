// global.familyManager = mp.browsers.new('package://cef/FamilyManager/index.html'); //Browser 2AC
// global.familyManager.active = false;
//
// // mp.keys.bind(Keys.VK_O, false, function () {
// //     if (!loggedin || chatActive || editing || new Date().getTime() - lastCheck < 1000 || global.menuOpened || !localplayer.getVariable('IS_FAMILY')) return;
// //     global.anyEvents.SendServer(() => mp.events.callRemote('openfamilymanager'));
// //     lastCheck = new Date().getTime();
// // });
//
// mp.events.add("openFamilyMenu", (json) => {
//   if (!loggedin || chatActive || editing || cuffed) return;
//   global.menuOpen();
//   global.familyManager.active = true;
//   setTimeout(function() {
//     global.familyManager.execute(`familyManager.active=true`);
//     global.familyManager.execute(`familyManager.setinfo(${json})`);
//   }, 250);
// });
//
// mp.events.add("closeFamilyManagerMenu", () => {
//   setTimeout(function() {
// 		global.menuClose();
// 		global.familyManager.active = false;
// 	}, 100);
// });
//
// mp.events.add("loadfamilymemberstomenu", (json) => {
// 	if(json == "client")
// 	{
// 		mp.events.callRemote('loadfamilymembers');
// 		return;
// 	}
// 	global.familyManager.execute(`familyManager.members=${json}`);
// });
//
// mp.events.add("invitePlayerToFamily", (value) => {
// 	global.anyEvents.SendServer(() => mp.events.callRemote('invitePlayerToFamily', value));
// });
//
// mp.events.add("changefamilyrank", (id, rank) => {
// 	//mp.game.graphics.notify(id + " - " + rank);
// 	global.anyEvents.SendServer(() => mp.events.callRemote('changefamilyrank', id, rank));
// });
//
// mp.events.add("kickfamilymember", (id, reason) => {
// 	//mp.game.graphics.notify(id + " - " + rank);
// 	global.anyEvents.SendServer(() => mp.events.callRemote('kickfamilymember', id, reason));
// });
//
// mp.events.add("saveFamilySettings", (desc_1, desc_2) => {
// 	//mp.game.graphics.notify(id + " - " + rank);
// 	global.anyEvents.SendServer(() => mp.events.callRemote('saveFamilySettings', desc_1, desc_2));
// });
//
// mp.events.add("saveChangesRanks", (allranks) => {
// 	global.anyEvents.SendServer(() => mp.events.callRemote('saveChangesRanks', allranks));
// });
//
// mp.events.add("disbandFamily", (reason) => {
// 	global.anyEvents.SendServer(() => mp.events.callRemote('disbandFamily', reason));
// });
