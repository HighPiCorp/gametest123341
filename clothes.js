const thisMenu = 'clothes';
const thisHTML = 'Oscar2';
const thisMenuCall = global.oscar2Menu;

let preComponent = null;
let preColor = null;

const clothesCamValues = [
  { Angle: 0, Dist: 1.4, Height: 0.2 }, // -11 Верхняя одежда
  { Angle: 0, Dist: 1.4, Height: 0.2 }, // -8 Майки
  { Angle: 0, Dist: 1.4, Height: -0.4 }, // -4 Брюки
  { Angle: 0, Dist: 1.2, Height: -0.7 }, // -6 Обувь
  { Angle: 0, Dist: 0.7, Height: 0.6 }, // -12 Шляпы
  { Angle: 0, Dist: 1, Height: 0.3 }, // -7 Аксессуары
  { Angle: 74, Dist: 1, Height: 0 }, // -14 Часы
  { Angle: 0, Dist: 0.7, Height: 0.6 }, // -13 Очки
  { Angle: 0, Dist: 1, Height: -0.2 }, // -3 Перчатки
  { Angle: 0, Dist: 1, Height: 0.3 }, // -14 Браслеты
];

function getClothesArray(paymentType, gender) {
  switch (paymentType) {
    case 0:
      return { // За $
        0: clothesTops[gender], // -11 Верхняя одежда
        1: clothesUnderwears[gender], // -8 Майки
        2: clothesLegs[gender], // -4 Брюки
        3: clothesFeets[gender], // -6 Обувь
        4: clothesHats[gender], // -12 Шляпы
        5: clothesJewerly[gender], // -7 Аксессуары
        6: clothesWatches[gender], // -14 Часы
        7: clothesGlasses[gender], // -13 Очки
        8: clothesGloves[gender], // -3 Перчатки
        // 9: null, // -14 Браслеты
      }
    case 1:
      return { // За $
        // 0: donateClothesTops[gender], // -11 Верхняя одежда
        // 1: donateClothesUnderwears[gender], // -8 Майки
        // 2: donateClothesLegs[gender], // -4 Брюки
        // 3: donateClothesFeets[gender], // -6 Обувь
        // 4: donateClothesHats[gender], // -12 Шляпы
        5: donateClothesJewerly[gender], // -7 Аксессуары
        // 6: donateClothesWatches[gender], // -14 Часы
        // 7: donateClothesGlasses[gender], // -13 Очки
        // 8: donateClothesGloves[gender], // -3 Перчатки
        // 9: null, // -14 Браслеты
      }
    default:
      return null;
  }

  return null;
};

function getClothesCategoriesArray(paymentType) {
  switch (paymentType) {
    case 0:
      return [
        {
          key: -11,
          id: 0,
          title: "Верхняя одежда",
        },
        {
          key: -8,
          id: 1,
          title: "Майки",
        },
        {
          key: -4,
          id: 2,
          title: "Брюки",
        },
        {
          key: -6,
          id: 3,
          title: "Обувь",
        },
        {
          key: -12,
          id: 4,
          title: "Шляпы",
        },
        {
          key: -7,
          id: 5,
          title: "Аксессуары",
        },
        {
          key: -14,
          id: 6,
          title: "Часы",
        },
        {
          key: -13,
          id: 7,
          title: "Очки",
        },
        {
          key: -3,
          id: 8,
          title: "Перчатки",
        }
      ]
    case 1:
      return [
        // {
        //   key: -11,
        //   id: 0,
        //   title: "Верхняя одежда",
        // },
        // {
        //   key: -8,
        //   id: 1,
        //   title: "Майки",
        // },
        // {
        //   key: -4,
        //   id: 2,
        //   title: "Брюки",
        // },
        // {
        //   key: -6,
        //   id: 3,
        //   title: "Обувь",
        // },
        // {
        //   key: -12,
        //   id: 4,
        //   title: "Шляпы",
        // },
        {
          key: -7,
          id: 5,
          title: "Аксессуары",
        },
        // {
        //   key: -14,
        //   id: 6,
        //   title: "Часы",
        // },
        // {
        //   key: -13,
        //   id: 7,
        //   title: "Очки",
        // },
        // {
        //   key: -3,
        //   id: 8,
        //   title: "Перчатки",
        // }
    ]
    default:
      return null;
  }

  return null;
};

let bodyCam = null;
let bodyCamStart = null;

let clothes = {
  type: 0,
  style: 0,
  color: 0,
  colors: [0, 0, 0],
  price: 0,
};

let paymentType = 0;

function clearClothes() {
  const gender = (localplayer.getVariable('GENDER')) ? 1 : 0;

  localplayer.clearProp(0);
  localplayer.clearProp(1);
  localplayer.clearProp(2);
  localplayer.clearProp(6);
  localplayer.clearProp(7);

  localplayer.setComponentVariation(1, clothesEmpty[gender][1], 0, 0);
  localplayer.setComponentVariation(3, clothesEmpty[gender][3], 0, 0);
  localplayer.setComponentVariation(4, clothesEmpty[gender][4], 0, 0);
  localplayer.setComponentVariation(5, clothesEmpty[gender][5], 0, 0);
  localplayer.setComponentVariation(6, clothesEmpty[gender][6], 0, 0);
  localplayer.setComponentVariation(7, clothesEmpty[gender][7], 0, 0);
  localplayer.setComponentVariation(8, clothesEmpty[gender][8], 0, 0);
  localplayer.setComponentVariation(9, clothesEmpty[gender][9], 0, 0);
  localplayer.setComponentVariation(10, clothesEmpty[gender][10], 0, 0);
  localplayer.setComponentVariation(11, clothesEmpty[gender][11], 0, 0);
}

const cameraRotator = require('public/utils/cameraRotator');

function ClothesInitCam() {
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

function ClothesCameraDestroy() {
  cameraRotator.stop();
  bodyCam.destroy();
  mp.game.cam.renderScriptCams(false, false, 500, true, false);
}

function ClothesCameraChange(id) {
  const camValues = clothesCamValues[id];
  const camPos = global.init.getCameraOffset(new mp.Vector3(bodyCamStart.x, bodyCamStart.y, bodyCamStart.z + camValues.Height), localplayer.getRotation(2).z + 90 + camValues.Angle, camValues.Dist);

  bodyCam.setCoord(camPos.x, camPos.y, camPos.z);
  bodyCam.pointAtCoord(bodyCamStart.x, bodyCamStart.y, bodyCamStart.z + camValues.Height);
}

const callbackOnClose = () => {
  mp.events.callRemote('SERVER::clothes:closeMenu');
  thisMenuCall.call('CEF::clothes:closeOnCallback');

  ClothesCameraDestroy();
};

function setComponentVariation(gender, clothesArr, index, type) {
  let colors = null;

  switch (type) {
    case 0: // -11 Верхняя одежда
      colors = clothesArr[index].Colors;

      clothes.style = clothesArr[index].Variation;
      clothes.color = colors[0];
      clothes.colors = colors;

      localplayer.setComponentVariation(11, clothes.style, clothes.color, 0);
      localplayer.setComponentVariation(3, parseInt(validTorsos[gender][clothes.style]), 0, 0);
      break;
    case 1: // -8 Майки
      colors = clothesArr[index].Colors;

      clothes.style = clothesArr[index].Top;
      clothes.color = colors[0];
      clothes.colors = colors;

      localplayer.setComponentVariation(11, clothes.style, clothes.color, 0);
      localplayer.setComponentVariation(3, parseInt(validTorsos[gender][clothes.style]), 0, 0);
      break;
    case 2: // -4 Брюки
      colors = clothesArr[index].Colors;

      clothes.style = clothesArr[index].Variation;
      clothes.color = colors[0];
      clothes.colors = colors;

      localplayer.setComponentVariation(4, clothes.style, clothes.color, 0);
      break;
    case 3: // -6 Обувь
      colors = clothesArr[index].Colors;

      clothes.style = clothesArr[index].Variation;
      clothes.color = colors[0];
      clothes.colors = colors;

      localplayer.setComponentVariation(6, clothes.style, clothes.color, 0);
      break;
    case 4: // -12 Шляпы
      colors = clothesArr[index].Colors;

      clothes.style = clothesArr[index].Variation;
      clothes.color = colors[0];
      clothes.colors = colors;

      localplayer.setPropIndex(0, clothes.style, clothes.color, true);
      break;
    case 5: // -7 Аксессуары
      // mp.console.logInfo(`getColor: 5 index`);
      colors = clothesArr[index].Colors;

      clothes.style = clothesArr[index].Variation;
      clothes.color = colors[0];
      clothes.colors = colors;

      localplayer.setComponentVariation(7, clothes.style, clothes.color, 0);

      // mp.console.logInfo(`CLIENT::clothes:getColor ->>> variation: ${clothes.style} texture: ${clothes.color}`);
      break;
    case 6: // -14 Часы
      colors = clothesArr[index].Colors;

      clothes.style = clothesArr[index].Variation;
      clothes.color = colors[0];
      clothes.colors = colors;

      localplayer.setPropIndex(6, clothes.style, clothes.color, true);
      break;
    case 7: // -13 Очки
      colors = clothesArr[index].Colors;

      clothes.style = clothesArr[index].Variation;
      clothes.color = colors[0];
      clothes.colors = colors;

      localplayer.setPropIndex(1, clothes.style, clothes.color, true);
      break;
    case 8: // -3 Перчатки
      colors = clothesArr[index].Colors;

      clothes.style = clothesArr[index].Variation;
      clothes.color = colors[0];
      clothes.colors = colors;

      localplayer.setComponentVariation(3, correctGloves[gender][clothes.style][15], clothes.color, 0);
      break;
    case 9: // -14 Браслеты

      break;
  }

  return colors;
}

mp.events.add('CLIENT::clothes:open', (productPrice, type) => {
  try {
    const gender = (localplayer.getVariable('GENDER')) ? 1 : 0;
    paymentType = type;

    let clothesArr = getClothesArray(paymentType, gender);
    const firstIndex = parseInt(Object.keys(clothesArr)[0]);
    if (firstIndex == 'undefined') return;

    clothesArr = clothesArr[firstIndex];
    if (clothesArr == null) return;

    const clothesCategoriesArr = getClothesCategoriesArray(paymentType);
    if (clothesCategoriesArr == null) return;

    const styles = [];
    const colors = clothesArr[firstIndex].Colors;
    const list = [];

    clothesArr.forEach((item, indexs) => {
      const listItem = {
        title: null,
        price: null,
      };

      const tempPrice = item.Price / 100 * productPrice;
      // mp.console.logInfo(`tempPrice: ${tempPrice} ${item.Price} / 100 * ${productPrice}`);
      listItem.price = parseInt(tempPrice.toFixed());

      styles.push(item.Variation);

      // names
      let cloth_name = `Вариант №${item.Variation}`;

      listItem.title = cloth_name;
      list.push(listItem);
    });

    clothes = {
      type: firstIndex,
      style: styles[0],
      color: colors[0],
      colors,
      price: productPrice,
    };

    clearClothes();

    ClothesInitCam();

    thisMenuCall.call('CEF::clothes:update', {
      show: true,
      // styles,
      colors,
      // names,
      list,
      // prices
      paymentType, // 0 - $, 1 - SWC
      categories: clothesCategoriesArr
    });

    setComponentVariation(gender, clothesArr, 0, clothes.type);

    global.anyMenuHTML.openAnyMenu(thisHTML, thisMenu, callbackOnClose);
  } catch (e) {
    logger.error(e);
  }
});

mp.events.add('CLIENT::clothes:hide', () => {
  callbackOnClose();
  global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
});

mp.events.add('CLIENT::clothes:buyCash', () => {
  global.anyEvents.SendServer(() => mp.events.callRemote('SERVER::clothes:buy', 0, clothes.type, clothes.style, clothes.color));
  // mp.events.call('CLIENT::clothes:hide');
});

mp.events.add('CLIENT::clothes:buyCard', () => {
  global.anyEvents.SendServer(() => mp.events.callRemote('SERVER::clothes:buy', 1, clothes.type, clothes.style, clothes.color));
  // mp.events.call('CLIENT::barber:hide');
});

mp.events.add('CLIENT::clothes:buySWC', () => {
  global.anyEvents.SendServer(() => mp.events.callRemote('SERVER::clothes:buy', 2, clothes.type, clothes.style, clothes.color));
  // mp.events.call('CLIENT::barber:hide');
});


let shoes = false;
mp.events.add('CLIENT::clothes:getList', (index) => {
  try {
    const gender = (localplayer.getVariable('GENDER')) ? 1 : 0;

    let clothesArr = getClothesArray(paymentType, gender)[index];
    if (clothesArr == null) return;

    switch (index) {
      case 3: // -6 Обувь
        if(!shoes) {
          shoes = true;
          localplayer.position = new mp.Vector3(localplayer.position.x, localplayer.position.y, localplayer.position.z + 0.1);
        }
        break;
    }

    if(index != 3 && shoes){
      localplayer.position = new mp.Vector3(localplayer.position.x, localplayer.position.y, localplayer.position.z - 0.1);
      shoes = false;
    }

    const styles = [];
    const list = [];
    const colors = clothesArr[0].Colors;
    let temp_variant;

    clothesArr.forEach((item, indexs) => {
      const listItem = {
        title: null,
        price: null,
      };

      const tempPrice = item.Price / 100 * clothes.price;
      // prices.push(tempPrice.toFixed());
      listItem.price = parseInt(tempPrice.toFixed());

      if (index === 1) {
        styles.push(item.Top);
        temp_variant = item.Top;
      } else {
        styles.push(item.Variation);
        temp_variant = item.Variation;
      }

      // names
      let cloth_name = `Вариация №${temp_variant}`;

      listItem.title = cloth_name;
      list.push(listItem);
    });

    clothes.type = index;
    clothes.style = styles[0];
    clothes.color = colors[0];
    clothes.colors = colors;

    thisMenuCall.call('CEF::clothes:update', {
      // styles,
      colors,
      // names,
      list,
      // prices
    });

    setComponentVariation(gender, clothesArr, 0, clothes.type);

    ClothesCameraChange(index);
  } catch (e) {
    logger.error(e);
  }
});

mp.events.add('CLIENT::clothes:getColor', (index) => {
  try {
    const gender = (localplayer.getVariable('GENDER')) ? 1 : 0;
    let colors;
    let clothesArr = getClothesArray(paymentType, gender)[clothes.type];
    if (clothesArr == null) return;

    colors = setComponentVariation(gender, clothesArr, index, clothes.type);

    thisMenuCall.call('CEF::clothes:update', {
      colors,
    });
  } catch (e) {
    logger.error(e);
  }
});

mp.events.add('CLIENT::clothes:changeColor', (index) => {
  try {
    const gender = (localplayer.getVariable('GENDER')) ? 1 : 0;

    // mp.console.logInfo(`CLIENT::clothes:changeColor ->>> clothes.type: ${clothes.type} index: ${index} clothes.colors[index]: ${clothes.colors[index]}`);
    clothes.color = parseInt(clothes.colors[index].toString());

    switch (clothes.type) {
      case 0: // -11 Верхняя одежда
        localplayer.setComponentVariation(11, clothes.style, clothes.color, 0);
        return;
      case 1: // -8 Майки
        localplayer.setComponentVariation(11, clothes.style, clothes.color, 0);
        return;
      case 2: // -4 Брюки
        localplayer.setComponentVariation(4, clothes.style, clothes.color, 0);
        return;
      case 3: // -6 Обувь
        localplayer.setComponentVariation(6, clothes.style, clothes.color, 0);
        return;
      case 4: // -12 Шляпы
        localplayer.setPropIndex(0, clothes.style, clothes.color, true);
        return;
      case 5: // -7 Аксессуары
        localplayer.setComponentVariation(7, clothes.style, clothes.color, 0);
        return;
      case 6: // -14 Часы
        localplayer.setPropIndex(6, clothes.style, clothes.color, true);
        return;
      case 7: // -13 Очки
        localplayer.setPropIndex(1, clothes.style, clothes.color, true);
        return;
      case 8: // -3 Перчатки
        localplayer.setComponentVariation(3, correctGloves[gender][clothes.style][15], clothes.color, 0); // TODO: CHECK 15
        return;
      case 9: // -14 Браслеты
      // clothesArr = null;
      // return;
    }
  } catch (e) {
    logger.error(e);
  }
});
