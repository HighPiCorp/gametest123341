global.tablet = mp.browsers.new('package://interfaces/ui/Tablet/index.html');
global.tabletOpen = false;

mp.events.add('tablet:close', () => {
    try {
        global.menuClose();
        global.tablet.execute('tablet.hide();');
        global.tabletOpen = false;
    } catch (e) {
        logger.error(e);
    }
});

mp.events.add('tablet:apply', (index) => {
    try {
        mp.storage.data.wallpaper = index;
        global.tablet.execute(`tablet.load(${index});`);
    } catch(e) {
        logger.error(e);
    }
});

mp.events.addDataHandler("BIZIDS", (entity, data) => {
    try {
        if (entity && entity.type === 'player' && entity === localplayer) {
            data = JSON.parse(data);
            const hasBiz = (data.length !== 0);
            if (!hasBiz) {
                global.tablet.execute(`tablet.closeBizScreenIfIsActive()`);
            }
            global.tablet.execute(`store.dispatch('setHasBiz', ${hasBiz});`);
        }
    } catch (e) {
        logger.error(e);
    }
});

mp.events.addDataHandler("fraction", (entity, data) => {
    try {
        if (entity && entity.type === 'player' && entity === localplayer) {
            if (data === 0 || data === undefined) {
                global.tablet.execute(`store.dispatch('setHasFrac', false);`);
                global.tablet.execute(`store.dispatch('setFracId', 0);`);
                global.tablet.execute(`tablet.closeFracScreenIfIsActive()`);
            } else {
                global.tablet.execute(`store.dispatch('setHasFrac', true);`);
                global.tablet.execute(`store.dispatch('setFracId', ${data});`);
            }
        }
    } catch (e) {
        logger.error(e);
    }
});

mp.events.addDataHandler("WORKID", (entity, data) => {
    try {
        if (entity && entity.type === 'player' && entity === localplayer) {
            global.tablet.execute(`store.dispatch('setWorkId', ${data});`);
        }
    } catch (e) {
        logger.error(e);
    }
});

/*mp.keys.bind(Keys.VK_6, false, function() {
    try {
        if (!loggedin || chatActive || global.menuCheck() || editing || cuffed || localplayer.getVariable('InDeath') === true) return;

        global.tablet.execute('tablet.init();');
        if (mp.storage.data.wallpaper) {
            const index = mp.storage.data.wallpaper;
            global.tablet.execute(`tablet.load(${index});`);
            global.tablet.execute(`store.dispatch('setPlayerName', '${localplayer.name}');`);
        }
        global.tabletOpen = true;

        global.menuOpen();
    } catch (e) {
        logger.error(e);
    }
});*/

mp.events.add('tablet:auction', (act, data) => {
    try {
        switch (act) {
            case 'doBet':
                const [id, value] = JSON.parse(data);
                mp.events.callRemote(act, id, value);
                break;
            case 'setGps':
                mp.events.callRemote(act, data);
                break;
            default:
                mp.events.callRemote(act);
                break;
        }
    } catch (e) {
        logger.error(e);
    }
});

mp.events.add('addadvert', (id, author, quest) => {
    try {
        const data = JSON.stringify({
            id,
            author,
            text: quest,
            blocked: false,
            blockedBy: ''
        });
        global.tablet.execute(`tabletApi.fraction.callback('addAdvert', '${data}')`);
        //mp.events.call('notify', 0, 2, "Пришло новое объявление!", 3000);
    } catch (e) {
        logger.error(e);
    }
})
mp.events.add('setadvert', (id, name = '') => {
    try {
        const data = JSON.stringify({id, name});
        if (name.length < 1) {
            global.tablet.execute(`tabletApi.fraction.callback('unblockAdvert', ${id})`);
        } else {
            global.tablet.execute(`tabletApi.fraction.callback('setStatus', '${data}')`);
        }
    } catch (e) {
        logger.error(e);
    }
})
mp.events.add('deladvert', (id) => {
    global.tablet.execute(`tabletApi.fraction.callback('deleteAdvert', ${id})`);
})
mp.events.add('takeadvert', (id, r) => {
    global.lastCheck = new Date().getTime();
    global.anyEvents.SendServer(() => mp.events.callRemote('takeadvert', id, r));
})
mp.events.add('sendadvert', (id, a) => {
    global.lastCheck = new Date().getTime();
    global.anyEvents.SendServer(() => mp.events.callRemote('sendadvert', id, a));
})

mp.events.add('tablet:auctionCallback', (act, data) => {
    global.tablet.execute(`tabletApi.trade.callback('${act}', '${data}')`);
});

mp.events.add('fraction', (act, ...data) => {
    mp.events.callRemote(act, ...data);
});

mp.events.add('police', (act, ...data) => {
    mp.events.callRemote(act, ...data);
});

mp.events.add('setfractionnews', (data) => {
    global.tablet.execute(`tabletApi.fraction.callback('setNews', '${data}')`);
});

// mp.events.add('openfractionmenu', (status) => {
//     if (!status) {
//         global.tablet.execute('tablet.back();');
//     }
// });

mp.events.add('setfractiondata', (data) => {
    global.tablet.execute(`tabletApi.fraction.callback('setFractionData', '${data}')`);
});

mp.events.add('setmemberdata', (data) => {
    global.tablet.execute(`tabletApi.fraction.callback('setPersonData', '${data}')`);
});

mp.events.add('setfractionaccess', (data) => {
    global.tablet.execute(`tabletApi.fraction.callback('setPerm', '${data}')`);
});

mp.events.add('openfractionmember', (data) => {
    global.tablet.execute(`tabletApi.fraction.callback('setMembers', '${data}')`);
});

mp.events.add('openlogs', (data) => {
    global.tablet.execute(`tabletApi.fraction.callback('setEventsLog', '${data}')`);
});

mp.events.add('openeditranks', (data) => {
    global.tablet.execute(`tabletApi.fraction.callback('setRankSettings', '${data}')`);
});

mp.events.add('openrankedit', (data) => {
    global.tablet.execute(`tabletApi.fraction.callback('openRankEditInfo', '${data}')`);
});

mp.events.add('openeditmemberrank', (data) => {
    global.tablet.execute(`tabletApi.fraction.callback('openEditMemberRank', '${data}')`);
});

mp.events.add('openeditrankaccess', (data) => {
    global.tablet.execute(`tabletApi.fraction.callback('openEditRankAccess', '${data}')`);
});

mp.events.add('stockstate', (data) => {
    global.tablet.execute(`tabletApi.fraction.callback('setStockState', ${data})`);
});

mp.events.add('openfractioncars', (data) => {
    global.tablet.execute(`tabletApi.fraction.callback('setFracCars', '${data}')`);
});

mp.events.add('openeditfractionpayday', (data) => {
    global.tablet.execute(`tabletApi.fraction.callback('setEditFractionPayday', '${data}')`);
});

mp.events.add('openorgsalary', (data) => {
    global.tablet.execute(`tabletApi.fraction.callback('setOrgs', '${data}')`);
});

mp.events.add('openfractionpayday', (data) => {
    global.tablet.execute(`tabletApi.fraction.callback('setOrg', '${data}')`);
});

mp.events.add('settaxpercent', (data) => {
    global.tablet.execute(`tabletApi.fraction.callback('setTaxPercent', '${data}')`);
});

mp.events.add('setactpercent', (data) => {
    global.tablet.execute(`tabletApi.fraction.callback('setActPercent', '${data}')`);
});

// police

mp.events.add("executeOutputWantedCar", (data) => {
    global.tablet.execute(`tabletApi.police.callback('setWantedVehicleList', '${data}')`);
});

mp.events.add("executeOutputWantedPerson", (data) => {
    global.tablet.execute(`tabletApi.police.callback('setWantedPersonList', '${data}')`);
});

mp.events.add("addCall", (data) => {
    global.tablet.execute(`tabletApi.police.callback('addCall', '${data}')`);
});

mp.events.add("deleteCall", (id) => {
    global.tablet.execute(`tabletApi.police.callback('deleteCall', ${id})`);
});

mp.events.add("opencalls", (data) => {
    global.tablet.execute(`tabletApi.police.callback('setCalls', '${data}')`);
});

mp.events.add("findPersonDatabase", (data) => {
    global.tablet.execute(`tabletApi.police.callback('setFoundPersons', '${data}')`);
});

mp.events.add("findVehicleDatabase", (data) => {
    global.tablet.execute(`tabletApi.police.callback('setFoundVehicles', '${data}')`);
});

mp.events.add("opendatabase", (data) => {
    global.tablet.execute(`tabletApi.police.callback('setPoliceInitData', ${data})`);
});
