const thisMenu = 'colorPicker';
const thisHTML = 'Oscar2';
const thisMenuCall = global.oscar2Menu;

let thisColorType = 0;
let thisKey = 'tuning';

const callbackOnClose = () => {
  thisMenuCall.call('CEF::colorPicker:closeOnCallback');
  global.anyMenuHTML.hideAnyMenu(thisHTML, thisMenu);
  mp.events.call('resetTuningCustomization');
};

mp.events.add('CLIENT::colorPicker:show', (key, colors = []) => {
  global.anyMenuHTML.openAnyMenu(thisHTML, thisMenu,  callbackOnClose);
  //mp.console.logInfo(`key: ${key} colors: ${colors}`);
  thisKey = key;
  thisMenuCall.call('CEF::colorPicker:update', JSON.stringify({
    show: true,
    showSelectorBox: true,
    showColorBox: false,
    key: key,
    currentColor: "#ff0000",
    colorList: colors
  }));
});

mp.events.add('CLIENT::colorPicker:openBox', () => {
  global.anyMenuHTML.openAnyMenu(thisHTML, thisMenu,  callbackOnClose);
  thisMenuCall.call('CEF::colorPicker:update', JSON.stringify({
    show: true,
    showSelectorBox: false,
    showColorBox: true,
    key: thisKey,
  }));
});

mp.events.add('CLIENT::colorPicker:open', (key) => {
  mp.events.callRemote('SERVER::colorPicker:open', key);
});

mp.events.add('CLIENT::colorPicker:changeColorType', (index) => {
  //mp.console.logInfo("CLIENT::colorPicker:changeColorType: "+index);
  thisColorType = index;
});

mp.events.add('CLIENT::colorPicker:acceptColor', (key) => { // apply
  global.anyMenuHTML.hideAnyMenu(thisHTML, thisMenu);
  if (key === 'tuning') mp.events.call('popup::open', 'tuningbuy', 'Вы действительно хотите покрасить машину в данный цвет?');
});

// mp.events.add('CLIENT::colorPicker:acceptColorBox', (key) => { // apply
//   global.anyMenuHTML.hideAnyMenu(thisHTML, thisMenu);
//   mp.events.call('popup::open', 'tuningbuy', 'Вы действительно хотите покрасить машину в данный цвет?');
// });

mp.events.add('CLIENT::colorPicker:exit', () => { // cancel
  callbackOnClose();
  // global.anyMenuHTML.hideAnyMenu(thisHTML, thisMenu);
});

mp.events.add('CLIENT::colorPicker:close', () => { // close
  callbackOnClose();
  // global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
});

mp.events.add('CLIENT::colorPicker:setColor', (key, color, type) => {
  if (key === 'tuning') {
    //mp.console.logInfo("ColorType: "+thisColorType);
    mp.events.call('tunColor', JSON.parse(color), thisColorType);
  }
});

mp.events.add('CLIENT::colorPicker:changeColorBoxColor', (colorIndex) => {
  //mp.console.logInfo("Perl colorIndex: "+colorIndex);
  mp.events.call('tunColor', colorIndex);
});

// mp.events.add('CLIENT::colorPicker:changeColor', (key, color) => {
//   if (key === 'tuning') mp.events.call('tunColor', JSON.parse(color));
// });

mp.events.add('CLIENT::colorPicker:addColor', (key, index, rgba) => {
  global.anyEvents.SendServer(() => mp.events.callRemote('SERVER::colorPicker:addPresetColor', key, index, rgba));
});

mp.events.add('CLIENT::colorPicker:deleteColor', (key, index) => {
  global.anyEvents.SendServer(() => mp.events.callRemote('SERVER::colorPicker:deletePresetColor', key, index));
});

mp.events.add('CLIENT::colorPicker:updatePresets', (colors) => {
  thisMenuCall.call('CEF::colorPicker:update', JSON.stringify({
    colorList: colors
  }));
});
