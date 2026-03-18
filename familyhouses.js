// HOUSE //
global.familyhouse = mp.browsers.new('package://cef/FamilyHouses/index.html');

mp.events.add('FamilyHouseMenu', (id, Owner, Type, Locked, Price ,VehiclesPositions) => {
	if (global.menuCheck()) return;
	menuOpen();
	global.familyhouse.execute(`hm.set('${id}','${Owner}','${Type}','${Locked}','${Price}','${VehiclesPositions}')`);
	global.familyhouse.execute('hm.active=true');
});

mp.events.add('FamilyHouseMenuBuy', (id, Owner, Type, Locked, Price, VehiclesPositions) => {
	if (global.menuCheck()) return;
  menuOpen();
	global.familyhouse.execute(`hmBuy.set('${id}','${Owner}','${Type}','${Locked}','${Price}','${VehiclesPositions}')`);
	global.familyhouse.execute('hmBuy.active=true');
});
mp.events.add('CloseFamilyHouseMenu', () => {
	global.familyhouse.execute('hm.active=false');
  mp.gui.cursor.visible = false;
	global.menuClose();
});

mp.events.add('CloseFamilyHouseMenuBuy', () => {
	global.familyhouse.execute('hmBuy.active=false');
  mp.gui.cursor.visible = false;
	global.menuClose();
});

mp.events.add("buyFamilyHouseMenu", (id) => {
  global.anyEvents.SendServer(() => mp.events.callRemote("buyFamilyHouse", id));
	global.familyhouse.execute('hmBuy.active=false');
  mp.gui.cursor.visible = false;
	global.menuClose();
});

mp.events.add("SellFamilyHome", (id) => {
  global.anyEvents.SendServer(() => mp.events.callRemote("SellFamilyHome", id));
	global.familyhouse.execute('hm.active=false');
  mp.gui.cursor.visible = false;
	global.menuClose();
});


