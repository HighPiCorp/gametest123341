// credits to ragempdev

const controlsIds = {
    F7: 0x76,
    W: 32,
    S: 33,
    A: 34,
    D: 35,
    Space: 321,
    LCtrl: 326,
    LMB: 24,
    RMB: 25
};

global.fly = {
    flying: false, f: 2.0, w: 2.0, h: 2.0, point_distance: 1000,
};
global.gameplayCam = mp.cameras.new('gameplay');

let direction = null;
let coords = null;

function pointingAt(distance) {
    const farAway = new mp.Vector3((direction.x * distance) + (coords.x), (direction.y * distance) + (coords.y), (direction.z * distance) + (coords.z));

    const result = mp.raycasting.testPointToPoint(coords, farAway, [1, 16]);
    if (result === undefined) {
        return 'undefined';
    }
    return result;
}

mp.events.add("AGM", (toggle) => {
  global.admingm = toggle;
  global.localplayer.setInvincible(toggle);
	mp.game.graphics.notify(toggle ? '~g~Бессмертие включено' : '~r~Бессмертие выключено');
});

mp.keys.bind(Keys.VK_F7, false, function () {
    if (!global.loggedin || global.localplayer.getVariable('IS_ADMIN') !== true) return;

    const controls = mp.game.controls;
    direction = global.gameplayCam.getDirection();
    coords = global.gameplayCam.getCoord();

    global.fly.flying = !global.fly.flying;

    const player = mp.players.local;

    if(!global.admingm) player.setInvincible(global.fly.flying);
    player.freezePosition(global.fly.flying);
    player.setAlpha(global.fly.flying ? 0 : 255);

    if (!global.fly.flying && !controls.isControlPressed(0, controlsIds.Space)) {
        const position = mp.players.local.position;
        position.z = mp.game.gameplay.getGroundZFor3dCoord(position.x, position.y, position.z, 0.0, false);
        mp.players.local.setCoordsNoOffset(position.x, position.y, position.z, false, false, false);
    }

    mp.events.callRemote('invisible', global.fly.flying);
    mp.game.graphics.notify(global.fly.flying ? '~g~Полёт включен' : '~r~Полёт выключен');
});

mp.events.add('render', () => {
    if (global.fly.flying) {
        const controls = mp.game.controls;
        direction = global.gameplayCam.getDirection();
        coords = global.gameplayCam.getCoord();

        let updated = false;
        const position = mp.players.local.position;
		var speed;
        if(controls.isControlPressed(0, controlsIds.LMB)) speed = 1.0
		else if(controls.isControlPressed(0, controlsIds.RMB)) speed = 0.02
		else speed = 0.2
		if (controls.isControlPressed(0, controlsIds.W)) {
            if (global.fly.f < 8.0) global.fly.f *= 1.025;
            position.x += direction.x * global.fly.f * speed;
            position.y += direction.y * global.fly.f * speed;
            position.z += direction.z * global.fly.f * speed;
            updated = true;
        } else if (controls.isControlPressed(0, controlsIds.S)) {
            if (global.fly.f < 8.0) global.fly.f *= 1.025;
            position.x -= direction.x * global.fly.f * speed;
            position.y -= direction.y * global.fly.f * speed;
            position.z -= direction.z * global.fly.f * speed;
            updated = true;
        } else global.fly.f = 2.0;
        if (controls.isControlPressed(0, controlsIds.A)) {
            if (global.fly.l < 8.0) global.fly.l *= 1.025;
            position.x += (-direction.y) * global.fly.l * speed;
            position.y += direction.x * global.fly.l * speed;
            updated = true;
        } else if (controls.isControlPressed(0, controlsIds.D)) {
            if (global.fly.l < 8.0) global.fly.l *= 1.05;
            position.x -= (-direction.y) * global.fly.l * speed;
            position.y -= direction.x * global.fly.l * speed;
            updated = true;
        } else global.fly.l = 2.0;
        if (controls.isControlPressed(0, controlsIds.Space)) {
            if (global.fly.h < 8.0) global.fly.h *= 1.025;
            position.z += global.fly.h * speed;
            updated = true;
        } else if (controls.isControlPressed(0, controlsIds.LCtrl)) {
            if (global.fly.h < 8.0) global.fly.h *= 1.05;
            position.z -= global.fly.h * speed;
            updated = true;
        } else global.fly.h = 2.0;
        if (updated) mp.players.local.setCoordsNoOffset(position.x, position.y, position.z, false, false, false);
    }
});

mp.events.add('getCamCoords', (name) => {
    mp.events.callRemote('saveCamCoords', JSON.stringify(coords), JSON.stringify(pointingAt(global.fly.point_distance)), name);
});
