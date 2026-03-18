global.familyCreator = mp.browsers.new('package://cef/FamilyCreator/index.html');
global.familyCreator.active = false;

mp.events.add("openCreatorFamilyMenu", (json) => {
  if (!global.loggedin || chatActive || editing || cuffed) return;
  global.menuOpen();
  // mp.events.call('toBlur', 200)
  global.familyCreator.active = true;
  setTimeout(function() {
    global.familyCreator.execute(`familyCreator.active=true`);
    global.familyCreator.execute(`familyCreator.setinfo(${json})`);
  }, 250);
});

mp.events.add("closeFamilyCreatorMenu", () => {
  setTimeout(function() {
		global.menuClose();
		mp.events.call('fromBlur', 200)
		global.familyCreator.active = false;
	}, 100);
});



mp.events.add("createFamilyImageChanks", (chank, count, current, mimetype, name, maxplayers) => {
	mp.events.callRemote("createFamilyImageChanks", chank, count, current, mimetype, name, maxplayers);
});

mp.events.add("createFamily", (name, maxpl) => {
	global.anyEvents.SendServer(() => mp.events.callRemote("createnewfamily", name, maxpl));
});

mp.events.add("loadlistfamilies", (sender, json) => {
	if(sender == "client") mp.events.callRemote("loadlistfamilies");
	if(sender == "server") global.familyCreator.execute(`familyCreator.dirty.families=${json}`);
});
