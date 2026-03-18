global.circleMenu = mp.browsers.new('package://interfaces/ui/Circle/index.html');
global.circleOpen = false;
global.circleEntity = null;

mp.events.add({
    'circle:close': () => {
        circleMenu.execute(`circleMenuAPI.hide()`);
        global.circleOpen = false;
        mp.gui.cursor.visible = false;
    },
    'circle:showMenu': (type = '') => {
        if (type === '' || global.circleOpen) return;
        const fraction = global.localplayer.getVariable('fraction');
        const family = global.localplayer.getVariable('family');
        const fractionAccess = global.localplayer.getVariable('fractionAccess');
        if (fraction) circleMenu.execute(`fraction=${fraction}`);
        if (family) circleMenu.execute(`family=${family}`);
        if (fractionAccess) circleMenu.execute(`setFractionAccess('${fractionAccess}')`);
        if (type === 'player')
            circleMenu.execute(`circleMenuAPI.showPlayerMenu()`);
        else if (type === 'vehicle')
            circleMenu.execute(`circleMenuAPI.showVehicleMenu()`);
        else if (type === 'self')
            circleMenu.execute(`circleMenuAPI.showSelfMenu()`);
        global.circleOpen = true;
        mp.gui.cursor.visible = true;
    },
    'circle:selectPlayer': (action) => {
        if (entity === null || action === '') return;
        global.circleEntity = entity;
        mp.events.callRemote('pSelected', entity, action);
    },
    'circle:selectSelf': (action) => {
        if (action === '') return;
        mp.events.callRemote('selfSelected', action);
    },
    'circle:selectVehicle': (action) => {
        if (entity === null || action === '') return;
        global.circleEntity = entity;
        mp.events.callRemote('vehicleSelected', entity, action);
    }
});
