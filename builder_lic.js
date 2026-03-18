const thisMenu = 'qualification';
const thisHTML = 'Oscar2';
const thisMenuCall = global.oscar2Menu;

const callbackOnClose = () => {
    thisMenuCall.call('CEF::qualification:closeOnCallback');
    global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
    global.menuOpened = false;
  };

mp.events.add('CLIENT::DOCS:OPEN_BUILDER_LIC', (data) => {
    thisMenuCall.call('CEF::qualification::update', data);
    global.anyMenuHTML.openAnyMenu(thisHTML, thisMenu, callbackOnClose);
    global.menuOpened = true;
});