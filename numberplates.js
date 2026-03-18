global.numberPlates = mp.browsers.new('package://interfaces/ui/NumberPlates/index.html');
global.numberPlatesOpen = false;

mp.events.add(
    {
        'numberPlates:open': (price, plateNumber) => {
            try {
                if (!global.loggedin || global.menuCheck() || cuffed || localplayer.getVariable('InDeath') === true) return;

                if (global.numberPlates === null)
                    global.numberPlates = mp.browsers.new('package://interfaces/ui/NumberPlates/index.html');


                global.numberPlates.execute(`numberPlates.init(${price}, '${plateNumber}')`);
                global.numberPlatesOpen = true;
                global.menuOpen();
                mp.gui.cursor.visible = true;
            } catch (e) {
                logger.error(e);
            }
        },

        'numberPlates:close': () => {
            try {
                global.numberPlates.execute(`numberPlates.hide()`);
                global.menuClose();
                global.numberPlatesOpen = false;
                mp.gui.cursor.visible = false;
                mp.events.callRemote('numberPlates:close');
            } catch (e) {
                logger.error(e);
            }
        },

        'numberPlates:setNumber': (number) => global.numberPlates.execute(`numberPlates.setPlateNumber('${number}')`)
    }
);
