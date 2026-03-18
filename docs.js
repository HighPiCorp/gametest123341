global.docs = mp.browsers.new('package://interfaces/ui/Licenses/index.html');
global.docsOpen = false;

mp.events.add('browserDomReady', (browser) => {
    if (browser === docs) {
        mp.events.add('closeDocs', () => {
            global.docs.execute(`licenses.hide();`);
            global.docsOpen = false;
            global.menuCloseIfNotOpened();
        });
        mp.events.add('showDocs', (data) => {
            global.docs.execute(`licenses.init('${data}', ${localplayer.getVariable('GENDER')});`);
            global.docsOpen = true;
            global.menuOpenIfNotOpened();
        });
    }
});

