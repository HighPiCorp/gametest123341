global.exclusiveCar = null;

mp.events.add('CLIENT::EXCLUSIVE:OPEN', function (data) {
    if (global.exclusiveCar != null) {
        global.exclusiveCar.destroy();
        global.exclusiveCar = null;
    }
    global.exclusiveCar = mp.browsers.new('package://cef/resalecars/exclusive.html');

    mp.gui.cursor.visible = true;
    let speed = (mp.game.vehicle.getVehicleModelMaxSpeed(mp.players.local.vehicle.model) / 1.2).toFixed();
    let breaks = (mp.game.vehicle.getVehicleModelMaxBraking(mp.players.local.vehicle.model) * 50).toFixed(2);
    let boost = (mp.game.vehicle.getVehicleModelAcceleration(mp.players.local.vehicle.model) * 100).toFixed(2);
    let clutch = (mp.game.vehicle.getVehicleModelMaxTraction(mp.players.local.vehicle.model) * 10).toFixed(2);

    let str ="["+speed+","+breaks+","+boost+","+clutch+"]";
    global.exclusiveCar.execute(`ExclusiveCar.setinfo(${data}, ${str});`);
});
mp.events.add('CLIENT::EXCLUSIVE:CLOSE', function () {
    if (global.exclusiveCar != null) {
        global.exclusiveCar.destroy();
        global.exclusiveCar = null;
        mp.events.call('fromBlur', 200)
        mp.gui.cursor.visible = false;
    }
});
mp.events.add('CLIENT::EXCLUSIVE:BUY', function (type) {
    mp.events.call('CLIENT::EXCLUSIVE:CLOSE');
    global.anyEvents.SendServer(() => mp.events.callRemote('SERVER::EXCLUSIVE:BUY_CAR',false, type));
});
mp.events.add('CLIENT::EXCLUSIVE:BUY_FAMILY', function () {
    mp.events.call('CLIENT::EXCLUSIVE:CLOSE');
    global.anyEvents.SendServer(() => mp.events.callRemote('SERVER::EXCLUSIVE:BUY_CAR',true, 0));
});
