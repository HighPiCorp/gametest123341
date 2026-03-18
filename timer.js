const thisMenu = 'Timer';
const thisHTML = 'FeST1VaL';
const thisMenuCall = global.festMenu;

mp.events.add('CLIENT::Timer:startTimer', (time) => {
    thisMenuCall.call("CEF::Timer:start", time);
    global.anyMenuHTML.openAnyHUDElement(thisHTML, thisMenu);
  });
  
  mp.events.add('CLIENT::Timer:stopTimer', () => {
    thisMenuCall.call("CEF::Timer:end");
    global.anyMenuHTML.closeAnyHUDElement(thisHTML, thisMenu);
  });

  mp.events.add('CLIENT::race:getResultTimer', (elapsedTime) => {
    global.anyMenuHTML.closeAnyHUDElement(thisHTML, thisMenu);
  });