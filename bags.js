const thisMenu = 'bags';
const thisHTML = 'Oscar2';
const thisMenuCall = global.oscar2Menu;

const cameraRotator = require('public/utils/cameraRotator');

let bodyCam = null;
let bodyCamStart = null;

let bags = {
  type: 0,
  style: 0,
  color: 0,
  colors: [0, 0, 0],
  price: 0,
};

const callbackOnClose = () => {
  mp.events.callRemote('SERVER::bags:closeMenu');
  thisMenuCall.call('CEF::bags:closeOnCallback');

  BagsCameraDestroy();
};

mp.events.add('CLIENT::bags:hide', () => {
  callbackOnClose();
  global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
});

mp.events.add('CLIENT::bags:open', (productPrice) => {
  try {
    const type = 0;

    const styles = [];
    const colors = clothesBags[0].Colors;
    const list = [];

    BagsInitCam();

    clothesBags.forEach((bags, index) => {
      const listItem = {
        title: null,
        price: null,
      };

      const tempPrice = bags.Price / 100 * productPrice;
      listItem.price = parseInt(tempPrice.toFixed());

      styles.push(bags.Variation);

      // names
      let cloth_name = `Вариант №${index + 1} Вес:${bags.Weight}`;

      listItem.title = cloth_name;
      list.push(listItem);
    });

    bags = {
      type: 0,
      style: styles[0],
      color: colors[0],
      colors,
      price: productPrice,
    };

    localplayer.setComponentVariation(5, styles[0], colors[0], 0);

    // localplayer.clearProp(8);
    // localplayer.clearProp(9);

    thisMenuCall.call('CEF::bags:update', {
      show: true,
      colors,
      list,
    });

    global.anyMenuHTML.openAnyMenu(thisHTML, thisMenu, callbackOnClose);
  } catch (e) {
    logger.error(e);
  }
});

mp.events.add('CLIENT::bags:buy', (buyType) => {
  global.anyEvents.SendServer(() => mp.events.callRemote('SERVER::bags:buy', buyType, bags.style, bags.color));
});

mp.events.add('CLIENT::bags:getColor', (index) => {
  try {
    const colors = clothesBags[index].Colors;

    bags.style = clothesBags[index].Variation;
    bags.color = colors[0];
    bags.colors = colors;

    //mp.console.logInfo("[getColor] bags.style: "+bags.style+ " bags.color: "+bags.color);

    localplayer.setComponentVariation(5, bags.style, bags.color, 0);

    thisMenuCall.call('CEF::bags:update', {
      colors,
    });
  } catch (e) {
    logger.error(e);
  }
});

mp.events.add('CLIENT::bags:changeColor', (index) => {
  try {
    bags.color = parseInt(bags.colors[index]);
    //mp.console.logInfo(`changeColor: ${bags.color}`);
    localplayer.setComponentVariation(5, bags.style, bags.color, 0);
  } catch (e) {
    logger.error(e);
  }
});

function BagsInitCam() {
  bodyCamStart = localplayer.position;

  const camValues = { Angle: localplayer.getRotation(2).z + 90, Dist: 1.3, Height: 0.3 };
  const pos = global.init.getCameraOffset(new mp.Vector3(bodyCamStart.x, bodyCamStart.y, bodyCamStart.z + camValues.Height), camValues.Angle, camValues.Dist);

  bodyCam = mp.cameras.new('default', pos, new mp.Vector3(0, 0, 0), 50);
  bodyCam.pointAtCoord(bodyCamStart.x, bodyCamStart.y, bodyCamStart.z + camValues.Height);
  bodyCam.setActive(true);
  mp.game.cam.renderScriptCams(true, false, 500, true, false);

  cameraRotator.start(bodyCam, bodyCamStart, bodyCamStart, new mp.Vector3(-1.0, 1.5, 0.5), -25);
  cameraRotator.setZBound(-0.8, 2.0);
  cameraRotator.setLBound(-4, 3);
  cameraRotator.setZUpMultipler(5);

  cameraRotator.pause(false);

}

function BagsCameraDestroy() {
  cameraRotator.stop();
  bodyCam.destroy();
  mp.game.cam.renderScriptCams(false, false, 500, true, false);
}
