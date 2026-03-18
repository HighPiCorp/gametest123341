const thisMenu = 'masks';
const thisHTML = 'Oscar2';
const thisMenuCall = global.oscar2Menu;

const camera = require('public/business/Masks/camera');

const CN_masks = JSON.parse(require('public/business/Masks/data/masksNames/masks'));

let masks = {
  type: 0,
  style: 0,
  color: 0,
  colors: [0, 0, 0],
  price: 0,
};

const callbackOnClose = () => {
  mp.events.callRemote('SERVER::mask:closeMenu');
  thisMenuCall.call('CEF::mask:closeOnCallback');

  camera.destroy();
};

mp.events.add('CLIENT::mask:hide', () => {
  callbackOnClose();
  global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
});

mp.events.add('CLIENT::mask:open', (productPrice) => {
  try {
    const type = 0;

    const styles = [];
    const colors = clothesMasks[0].Colors;
    const list = [];

    const maskNames = CN_masks;

    camera.init();

    clothesMasks.forEach((mask, index) => {
      const listItem = {
        title: null,
        price: null,
      };

      const tempPrice = mask.Price / 100 * productPrice;
      listItem.price = parseInt(tempPrice.toFixed());

      styles.push(mask.Variation);

      // names
      let cloth_name = `Вариант №${index + 1}`;
      let real_name;
      let gxt_name;

      if (maskNames[mask.Variation] !== undefined) real_name = maskNames[mask.Variation][type];
      if (real_name !== undefined) {
        if (real_name.GXT !== undefined && real_name.GXT !== 'NO_LABEL') {
          gxt_name = mp.game.ui.getLabelText(real_name.GXT);
          cloth_name = gxt_name; // Выдергиваем название шмотки из самой GTA V
        }

        if (maskNames[mask.Variation][type].Localized !== undefined && maskNames[mask.Variation][type].Localized !== 'NULL') {
          real_name = maskNames[mask.Variation][type].Localized;
          // cloth_name = real_name; // Название шмотки из кастомного конфига
        }
      }
      listItem.title = cloth_name;
      list.push(listItem);
    });

    masks = {
      type: 0,
      style: styles[0],
      color: colors[0],
      colors,
      price: productPrice,
    };

    localplayer.setComponentVariation(1, styles[0], colors[0], 0);

    localplayer.clearProp(0);
    localplayer.clearProp(1);

    thisMenuCall.call('CEF::mask:update', {
      show: true,
      colors,
      list,
    });

    global.anyMenuHTML.openAnyMenu(thisHTML, thisMenu, callbackOnClose);
  } catch (e) {
    logger.error(e);
  }
});

mp.events.add('CLIENT::mask:buy', (buyType) => {
  global.anyEvents.SendServer(() => mp.events.callRemote('SERVER::mask:buy', buyType, masks.style, masks.color));
});

mp.events.add('CLIENT::mask:getColor', (index) => {
  try {
    const colors = clothesMasks[index].Colors;

    masks.style = clothesMasks[index].Variation;
    masks.color = colors[0];
    masks.colors = colors;

    // mp.console.logInfo("[getColor] mask colors: "+colors+ " index: "+index);

    localplayer.setComponentVariation(1, masks.style, masks.color, 0);

    thisMenuCall.call('CEF::mask:update', {
      colors,
    });
  } catch (e) {
    logger.error(e);
  }
});

mp.events.add('CLIENT::mask:preSetComponent', (index) => {
  try {
    const colors = clothesMasks[index].Colors;

    const style = clothesMasks[index].Variation;

    localplayer.setComponentVariation(1, style, colors[0], 0);
  } catch (e) {
    logger.error(e);
  }
});


mp.events.add('CLIENT::mask:preChangeColor', (index) => {
  try {
    const color = parseInt(masks.colors[index]);
    //mp.console.logInfo(`changeColor: ${masks.color}`);
    localplayer.setComponentVariation(1, masks.style, color, 0);
  } catch (e) {
    logger.error(e);
  }
});

mp.events.add('CLIENT::mask:changeColor', (index) => {
  try {
    masks.color = parseInt(masks.colors[index]);
    //mp.console.logInfo(`changeColor: ${masks.color}`);
    localplayer.setComponentVariation(1, masks.style, masks.color, 0);
  } catch (e) {
    logger.error(e);
  }
});
