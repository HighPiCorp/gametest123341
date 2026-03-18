global.tuningMenuHTML = mp.browsers.new('package://cef/resources/views/menus/tuning.html');
global.tuningMenuHTML.active = false;

global.tuningCam = null;
global.tuningCamFrom = null;
global.tuningItem = null;
global.tuningColorItem = null;
global.tuningVehicleComponents = null;
global.tuningSelectedColor = { r: 0, g: 0, b: 0 };
global.tuningPearlColor = 0;
global.tuningWheelColor = 0;
global.tuningColorType1 = 0;
global.tuningColorType2 = 0;
global.tuningSelected = null;
global.tuningWheelsTypes = {
  wheels_exclusive: 7,
  wheels_lowrider: 2,
  wheels_musclecar: 1,
  wheels_4x4: 3,
  wheels_sport: 0,
  wheels_4x4_2: 4,
  wheels_tuner: 5,
};

global.tuningHornNames = {
  '-1': 'HORN_STOCK',
  0: 'HORN_TRUCK',
  1: 'HORN_POLICE',
  2: 'HORN_CLOWN',
  3: 'HORN_MUSICAL1',
  4: 'HORN_MUSICAL2',
  5: 'HORN_MUSICAL3',
  6: 'HORN_MUSICAL4',
  7: 'HORN_MUSICAL5',
  8: 'HORN_SADTROMBONE',
  9: 'HORN_CALSSICAL1',
  10: 'HORN_CALSSICAL2',
  11: 'HORN_CALSSICAL3',
  12: 'HORN_CALSSICAL4',
  13: 'HORN_CALSSICAL5',
  14: 'HORN_CALSSICAL6',
  15: 'HORN_CALSSICAL7',
  16: 'HORN_CLASSICAL8',
  17: 'HORN_CLASSICALLOOP1',
  18: 'HORN_CLASSICALLOOP2',
  19: 'HORN_SCALEDO',
  20: 'HORN_SCALERE',
  21: 'HORN_SCALEMI',
  22: 'HORN_SCALEFA',
  23: 'HORN_SCALESOL',
  24: 'HORN_SCALELA',
  25: 'HORN_SCALETI',
  26: 'HORN_SCALEDO_HIGH',
  27: 'HORN_JAZZ1',
  28: 'HORN_JAZZ2',
  29: 'HORN_JAZZ3',
  30: 'HORN_JAZZLOOP',
  31: 'HORN_STARSPANGBAN1',
  32: 'HORN_STARSPANGBAN2',
  33: 'HORN_STARSPANGBAN3',
  34: 'HORN_STARSPANGBAN4',
};

global.tuningMenuList = {
  0: {
    name: 'Глушители',
    uniq_name: 'muffler',
    numMods: 0,
    icon: 'muffler',
    disabled: false,
    modId: 4,
    categoryModId: 0,
    submenus: null,
    cam: {
      heading: 85.0,
      position: [-333.7966, -137.409, 38.88963],
    },
  },
  1: {
    name: 'Пороги',
    uniq_name: 'sideskirt',
    numMods: 0,
    icon: 'step',
    disabled: false,
    modId: 3,
    categoryModId: 1,
    submenus: null,
    cam: {
      heading: 148.9986,
      position: [-333.7966, -137.409, 39.28963],
    },
  },
  2: {
    name: 'Капоты',
    uniq_name: 'hood',
    numMods: 0,
    icon: 'car',
    disabled: false,
    modId: 7,
    categoryModId: 2,
    submenus: null,
    cam: {
      heading: 265.0,
      position: [-333.7966, -137.409, 40.58963],
    },
  },
  3: {
    name: 'Спойлеры',
    uniq_name: 'spoiler',
    numMods: 0,
    icon: 'spoiler',
    disabled: false,
    modId: 0,
    categoryModId: 3,
    submenus: null,
    cam: {
      heading: 85.0,
      position: [-333.7966, -137.409, 39.28963],
    },
  },
  4: {
    name: 'Решетка радиатора',
    uniq_name: 'lattice',
    numMods: 0,
    icon: 'grid',
    disabled: false,
    modId: 6,
    categoryModId: 4,
    submenus: null,
    cam: {
      heading: 265.0,
      position: [-333.7966, -137.409, 38.88963],
    },
  },
  5: {
    name: 'Крылья',
    uniq_name: 'wings',
    numMods: 0,
    icon: 'wings',
    disabled: false,
    modId: 8,
    categoryModId: 5,
    submenus: {
      6: {
        name: 'Передние крылья',
        uniq_name: 'wings',
        numMods: 0,
        icon: 'wings',
        disabled: false,
        modId: 8,
        categoryModId: 5,
        submenus: null,
        cam: {
          heading: 148.9986,
          position: [-333.7966, -137.409, 39.28963],
        },
      },
      7: {
        name: 'Задние крылья',
        uniq_name: 'rearwings',
        numMods: 0,
        icon: 'wings',
        disabled: false,
        modId: 9,
        categoryModId: 101,
        submenus: null,
        cam: {
          heading: 148.9986,
          position: [-333.7966, -137.409, 39.28963],
        },
      },
    },
    cam: {
      heading: 148.9986,
      position: [-333.7966, -137.409, 39.28963],
    },
  },
  6: {
    name: 'Крыши',
    uniq_name: 'roof',
    numMods: 0,
    icon: 'roof',
    disabled: false,
    modId: 10,
    categoryModId: 6,
    submenus: null,
    cam: {
      heading: 148.9986,
      position: [-333.7966, -137.409, 40.08963],
    },
  },
  7: {
    name: 'Винилы',
    uniq_name: 'vinyls',
    numMods: 0,
    icon: 'flame',
    disabled: false,
    modId: 48,
    categoryModId: 7,
    submenus: null,
    cam: {
      heading: 160.9986,
      position: [-333.7966, -137.409, 42.08963],
    },
  },
  8: {
    name: 'Бампера',
    uniq_name: 'bamper',
    numMods: 0,
    icon: 'bump',
    disabled: false,
    modId: 1,
    categoryModId: 8,
    submenus: {
      11: {
        name: 'Передние бампера',
        uniq_name: 'frontbumper',
        numMods: 0,
        icon: 'bump',
        disabled: false,
        modId: 1,
        categoryModId: 8,
        submenus: null,
        cam: {
          heading: 265.0,
          position: [-333.7966, -137.409, 38.88963],
        },
      },
      12: {
        name: 'Задние бампера',
        uniq_name: 'rearbumper',
        numMods: 0,
        icon: 'bump',
        disabled: false,
        modId: 2,
        categoryModId: 9,
        submenus: null,
        cam: {
          heading: 85.0,
          position: [-333.7966, -137.409, 38.88963],
        },
      },
    },
    cam: null,
  },
  10: {
    name: 'Двигатели',
    uniq_name: 'engine',
    numMods: 0,
    icon: 'engine',
    disabled: false,
    modId: 11,
    categoryModId: 10,
    submenus: null,
    cam: {
      heading: 265.0,
      position: [-333.7966, -137.409, 40.58963],
    },
  },
  11: {
    name: 'Турбины',
    uniq_name: 'turbo',
    numMods: 1,
    icon: 'turbine',
    disabled: false,
    modId: 18,
    categoryModId: 11,
    submenus: null,
    cam: {
      heading: 265.0,
      position: [-333.7966, -137.409, 40.58963],
    },
  },
  12: {
    name: 'Клаксоны',
    uniq_name: 'horn',
    numMods: 0,
    icon: 'horn',
    disabled: false,
    modId: 14,
    categoryModId: 12,
    submenus: null,
    cam: {
      heading: 148.9986,
      position: [-333.7966, -137.409, 40.58963],
    },
  },
  13: {
    name: 'Трансмиссии',
    uniq_name: 'transmission',
    numMods: 0,
    icon: 'kpp',
    disabled: false,
    modId: 13,
    categoryModId: 13,
    submenus: null,
    cam: {
      heading: 265.0,
      position: [-333.7966, -137.409, 40.58963],
    },
  },
  14: {
    name: 'Стекла',
    uniq_name: 'windowtint',
    numMods: 0,
    icon: 'windows',
    disabled: false,
    modId: 55,
    categoryModId: 14,
    submenus: null,
    cam: {
      heading: 148.9986,
      position: [-333.7966, -137.409, 39.28963],
    },
  },
  15: {
    name: 'Подвески',
    uniq_name: 'suspension',
    numMods: 0,
    icon: 'suspension',
    disabled: false,
    modId: 15,
    categoryModId: 15,
    submenus: null,
    cam: {
      heading: 148.9986,
      position: [-333.7966, -137.409, 39.28963],
    },
  },
  16: {
    name: 'Тормоза',
    uniq_name: 'brakes',
    numMods: 0,
    icon: 'brake',
    disabled: false,
    modId: 12,
    categoryModId: 16,
    submenus: null,
    cam: {
      heading: 265.0,
      position: [-333.7966, -137.409, 40.58963],
    },
  },
  17: {
    name: 'Фары',
    uniq_name: 'headlights',
    numMods: 12,
    icon: 'lights',
    disabled: false,
    modId: 22,
    categoryModId: 17,
    submenus: null,
    cam: {
      heading: 265.0,
      position: [-333.7966, -137.409, 38.88963],
    },
  },
  18: {
    name: 'Номера',
    uniq_name: 'numberplate',
    numMods: 0,
    icon: 'license',
    disabled: false,
    modId: 53,
    categoryModId: 18,
    submenus: null,
    cam: {
      heading: 85.0,
      position: [-333.7966, -137.409, 38.88963],
    },
  },
  19: {
    name: 'Колеса',
    uniq_name: 'wheels',
    numMods: 0,
    icon: 'wheel',
    disabled: false,
    modId: 23,
    categoryModId: 19,
    submenus: {
      0: {
        name: 'Спортивные колеса',
        uniq_name: 'wheels_sport',
        numMods: 0,
        icon: '',
        disabled: false,
        modId: 0,
        categoryModId: 23,
        submenus: null,
        cam: null,
      },
      1: {
        name: 'Маслкар колеса',
        uniq_name: 'wheels_musclecar',
        numMods: 0,
        icon: '',
        disabled: false,
        modId: 1,
        categoryModId: 23,
        submenus: null,
        cam: null,
      },
      2: {
        name: 'Лоурайдер колеса',
        uniq_name: 'wheels_lowrider',
        numMods: 0,
        icon: '',
        disabled: false,
        modId: 2,
        categoryModId: 23,
        submenus: null,
        cam: null,
      },
      3: {
        name: 'Внедорожные колеса',
        uniq_name: 'wheels_4x4',
        numMods: 0,
        icon: '',
        disabled: false,
        modId: 3,
        categoryModId: 23,
        submenus: null,
        cam: null,
      },
      4: {
        name: 'Внедорожные #2 колеса',
        uniq_name: 'wheels_4x4_2',
        numMods: 0,
        icon: '',
        disabled: false,
        modId: 4,
        categoryModId: 23,
        submenus: null,
        cam: null,
      },
      5: {
        name: 'Тюнер колеса',
        uniq_name: 'wheels_tuner',
        numMods: 0,
        icon: '',
        disabled: false,
        modId: 5,
        categoryModId: 23,
        submenus: null,
        cam: null,
      },
      7: {
        name: 'Эксклюзивные колеса',
        uniq_name: 'wheels_exclusive',
        numMods: 0,
        icon: '',
        disabled: false,
        modId: 7,
        categoryModId: 23,
        submenus: null,
        cam: null,
      },
    },
    cam: {
      heading: 148.9986,
      position: [-333.7966, -137.409, 39.28963],
    },
  },
  20: {
    name: 'Покраска',
    uniq_name: 'paint',
    numMods: 0,
    icon: 'paintbrush',
    disabled: false,
    modId: 66,
    categoryModId: 20,
    submenus: {
      0: {
        name: 'Основной цвет',
        uniq_name: 'paint_one',
        numMods: 0,
        icon: 'paintbrush',
        disabled: false,
        modId: 66,
        categoryModId: 66,
        submenus: null,
        cam: null,
      },
      1: {
        name: 'Дополнительный цвет',
        uniq_name: 'paint_two',
        numMods: 0,
        icon: 'paintbrush',
        disabled: false,
        modId: 67,
        categoryModId: 67,
        submenus: null,
        cam: null,
      },
      2: {
        name: 'Неоновая подсветка',
        uniq_name: 'paint_three',
        numMods: 0,
        icon: 'paintbrush',
        disabled: false,
        modId: 66, // pseudo
        categoryModId: 'neon',
        submenus: null,
        cam: null,
      },
      3: {
        name: 'Перламутровый цвет',
        uniq_name: 'paint_four',
        numMods: 0,
        icon: 'paintbrush',
        disabled: false,
        modId: 68,
        categoryModId: 68,
        submenus: null,
        cam: null,
      },
      4: {
        name: 'Цвет колес',
        uniq_name: 'paint_fifth',
        numMods: 0,
        icon: 'paintbrush',
        disabled: false,
        modId: 69,
        categoryModId: 69,
        submenus: null,
        cam: null,
      },
    },
    cam: {
      heading: 160.9986,
      position: [-333.7966, -137.409, 40.08963],
    },
  },
};

global.tuningMenu = {
  title: '',
  menuState: false,
  lastTime: false,
  active: false,
  coef: null,
  data: null,
  open(coef = null, data = null, menu = null) {
    global.tuningMenuHTML.active = true;
    global.tuningMenu.menuState = true;

    if (global.tuningMenu.menuState) {
      global.menuOpen();
    }

    global.tuningMenu.coef = coef;
    global.tuningMenu.data = data;

    // //mp.console.logInfo("typeof1 "+ typeof global.tuningMenu.data);

    global.tuningMenuHTML.execute(`tuningMenu.updateInfo(${coef}, ${data}, ${JSON.stringify(menu)})`);
    global.tuningMenuHTML.execute('tuningMenu.dirty.active=true;');
    global.tuningMenuHTML.execute('tuningMenu.dirty.menuActive=true;');

    mp.gui.cursor.visible = true;
  },
  close() {
    global.tuningMenuHTML.active = false;
    global.tuningMenu.menuState = false;

    global.tuningMenuHTML.execute('tuningMenu.active=false;');

    if (!global.tuningMenu.menuState) {
      global.menuClose();
    }

    global.tuningMenuOpen = false;
    mp.gui.cursor.visible = false;
  },
  info(data) {
    global.tuningMenuHTML.execute(`tuningMenu.updateInfo(${data})`);
  },
};

mp.events.add('tuningGetCarCharacteristics', () => {
  const characteristics = global.init.getCharacteristics();

  global.tuningMenuHTML.execute(`tuningMenu.dirty.car.characteristic.speed=${characteristics[0]}`);
  global.tuningMenuHTML.execute(`tuningMenu.dirty.car.characteristic.brakes=${characteristics[1]}`);
  global.tuningMenuHTML.execute(`tuningMenu.dirty.car.characteristic.boost=${characteristics[2]}`);
  global.tuningMenuHTML.execute(`tuningMenu.dirty.car.characteristic.clutch=${characteristics[3]}`);
});

mp.events.add("CLIENT::Vehicle:applyCustomization", (entity, data) => {
  try {
    if (!data) return;
    // if (!entity) {
    //   while(!entity) await mp.game.waitAsync(0); // Один раз у меня почему то вылазила ошибка что нету entity при заходе в гараж.
    // }

    if (entity && entity.type !== "vehicle") return;

    if (entity && mp.vehicles.exists(entity)) {
      if (typeof data !== 'object') {
        data = JSON.parse(data);
      }

      setTimeout(() => {
        entity.setCustomPrimaryColour(data.PrimColor.Red, data.PrimColor.Green, data.PrimColor.Blue);
        entity.setCustomSecondaryColour(data.SecColor.Red, data.SecColor.Green, data.SecColor.Blue);

        entity.setModColor1(data.PrimModColor, 0, data.PearlColor);
        entity.setModColor2(data.SecModColor, 0);
        entity.setExtraColours(data.PearlColor, data.WheelsColor);
      }, 1000);
    }
  } catch (e) {
    logger.error(e);
  }
})

global.currentTuningMenuList = null;

mp.events.add('openTuningMenu', (reopen = false, coef = null, data = null, vehicleComponents = null) => {
  global.closeAnyMenuFunctions.closePhone();

  global.tuningComponents = JSON.parse(vehicleComponents);

  global.tuningPearlColor = global.tuningComponents.PearlColor;
  global.tuningWheelColor = global.tuningComponents.WheelsColor;
  global.tuningColorType1 = global.tuningComponents.PrimModColor;
  global.tuningColorType2 = global.tuningComponents.SecModColor;

  if (!reopen) {
    global.currentTuningMenuList = JSON.parse(JSON.stringify(global.tuningMenuList));

    for (const element in global.currentTuningMenuList) {
      const category = global.currentTuningMenuList[element];

      if (category.modId !== null && category.submenus !== null) {
        for (const submenuElement in category.submenus) {
          const submenuCategory = category.submenus[submenuElement];

          // Skip lights and colors menu just all time give 0
          if (submenuCategory.modId && (submenuCategory.modId === 66 || submenuCategory.modId === 67 || submenuCategory.modId === 68 || submenuCategory.modId === 69 || submenuCategory.modId === 22 || submenuCategory.modId === 18)) continue;
          let vehicleNumMods = mp.players.local.vehicle.getNumMods(submenuCategory.modId);
          // mp.console.logError(`ModKits: category.modId: ${category.modId} vehicleNumMods: ${vehicleNumMods} categoryName: ${category.name}`);

          submenuCategory.numMods = vehicleNumMods;

          // Если в текущей подкатегории нету numMods, то подкатегория disabled true
          if (submenuCategory.modId && vehicleNumMods <= 0) {
            submenuCategory.disabled = true;
            category.disabled = true;

            // // mp.console.logInfo("ModKits: Если в текущей подкатегории нету numMods, то подкатегория disabled true");
          }
        }

        // Перепроверяем disabled у подкатегорий, если есть disabled false, то категория активна остается
        let isDisabled;

        for (const recheckSubmenuElement in category.submenus) {
          const recheckSubmenuCategory = category.submenus[recheckSubmenuElement];
          if (recheckSubmenuCategory.modId && (recheckSubmenuCategory.modId === 66 || recheckSubmenuCategory.modId === 67 || recheckSubmenuCategory.modId === 68 || recheckSubmenuCategory.modId === 69 || recheckSubmenuCategory.modId === 22 || recheckSubmenuCategory.modId === 18)) continue;
          isDisabled = recheckSubmenuCategory.disabled;

          // Если в подкатегории есть не disabled элемент, то категорию не disabled true
          if (!isDisabled) {
            // // mp.console.logInfo("Recheck: Если в подкатегории есть не disabled элемент, то категорию disabled false");
            category.disabled = false;
          }
        }
      } else {
        // Skip lights and colors menu just all time give 0 AND TURBO
        if (category.modId && (category.modId === 66 || category.modId === 67 || category.modId === 68 || category.modId === 69 || category.modId === 22 || category.modId === 18)) continue;

        let vehicleNumMods = mp.players.local.vehicle.getNumMods(category.modId);
        // mp.console.logError(`ModID null || submenus?: ${category.modId} vehicleNumMods: ${vehicleNumMods} categoryName: ${category.name}`);

        category.numMods = vehicleNumMods;

        if (vehicleNumMods <= 0) {
          category.disabled = true;

          // // mp.console.logInfo("Other: Если в текущей подкатегории нету numMods, то подкатегория disabled true");
        }
      }
    }

    global.tuningMenu.open(coef, data, global.currentTuningMenuList);

    createCam(CAM_POSITION, CAM_ROTATION, CAM_VIEWANGLE);
    cameraRotator.pause(false);
  } else {
    global.tuningMenu.open(coef, data, global.currentTuningMenuList);
  }
});

mp.events.add('reopenTuningMenu', () => {
  global.anyEvents.SendServer(() => mp.events.callRemote('reopenTuningMenu'));
});

mp.events.add('hideTuningMenu', () => {
  global.tuningMenu.close();

  localplayer.vehicle.setLights(0);
});

mp.events.add('tuningCloseMenu', () => {
  global.tuningMenu.close();

  destroyCam();

  localplayer.vehicle.setLights(0);

  mp.events.callRemote('exitTuning');
});

mp.events.add('tuningSeatsCheck', () => {
  for (let i = 0; i < 7; i++) {
    if (localplayer.vehicle.getPedInSeat(i) !== 0) {
      mp.events.call('notify', 4, 9, 'Попросите выйти всех пассажиров', 3000);
      return;
    }
  }

   mp.events.callRemote('tuningSeatsCheck');
});

mp.events.add('getTuningList', (model, index) => {
  mp.events.callRemote('getTuningList', model, index);
});

mp.events.add('getTuningWheelsList', (index) => {
  mp.events.callRemote('getTuningWheelsList', index);
});

mp.events.add('resetTuningCustomization', () => {
  mp.events.callRemote('resetTuningCustomization');
});

mp.events.add('updateTuningList', (data, isWheels) => {
  global.tuningMenuHTML.execute(`tuningMenu.updateTuningList(${data}, ${isWheels})`);
});

mp.events.add('setTuningMod', (item, id) => {
  item = JSON.parse(item);
  const { modId } = item;
  const { categoryModId } = item;
  //mp.console.logError(`name: ${item.uniq_name} id: ${id} modId: ${modId} categoryModId: ${categoryModId}`, true, true);

  switch (item.uniq_name) {
    case 'wheels_exclusive':
    case 'wheels_lowrider':
    case 'wheels_musclecar':
    case 'wheels_4x4':
    case 'wheels_sport':
    case 'wheels_4x4_2':
    case 'wheels_tuner':

      localplayer.vehicle.setWheelType(parseInt(modId));
      localplayer.vehicle.setMod(parseInt(categoryModId), parseInt(id));
      break;
    case 'windowtint':
      localplayer.vehicle.setWindowTint(parseInt(id));
      break;
    case 'horn':
      // // mp.console.logInfo(`horn id: ${id} id is: ${typeof id}`);
      localplayer.vehicle.setMod(14, parseInt(id));
      localplayer.vehicle.startHorn(5000, mp.game.joaat(global.tuningHornNames[id]), false);
      break;
    case 'headlights':
      localplayer.vehicle.setLights(2);

      if (id >= 0) {
        localplayer.vehicle.setMod(parseInt(modId), 0);
        localplayer.vehicle.toggleMod(parseInt(modId), true);
        mp.game.invoke('0xE41033B25D003A07', localplayer.vehicle.handle, parseInt(id));
      } else {
        localplayer.vehicle.setMod(parseInt(modId), -1);
      }
      break;
    case 'numberplate':
      localplayer.vehicle.setNumberPlateTextIndex(parseInt(id));
      break;
    case 'spoiler':
      localplayer.vehicle.setMod(parseInt(modId), parseInt(id));
      break;
    case 'turbo':
      localplayer.vehicle.toggleMod(parseInt(modId), true);
      localplayer.vehicle.setMod(parseInt(modId), parseInt(id));
      mp.events.call('tuningGetCarCharacteristics');
      break;
    case 'paint_three':
      localplayer.vehicle.setNeonLightEnabled(0, true);
      localplayer.vehicle.setNeonLightEnabled(1, true);
      localplayer.vehicle.setNeonLightEnabled(2, true);
      localplayer.vehicle.setNeonLightEnabled(3, true);
      break;
    default:
      localplayer.vehicle.setMod(parseInt(modId), parseInt(id));
      mp.events.call('tuningGetCarCharacteristics');
      break;
  }
});

mp.events.add('tunColor', (colors, type = 0) => {
  const item = JSON.parse(global.tuningItem);
  if (!item) return;

  switch (item.uniq_name) {
    case 'paint_one':

      localplayer.vehicle.setModColor1(type, 0, 0);
      localplayer.vehicle.setCustomPrimaryColour(colors.r, colors.g, colors.b);
      localplayer.vehicle.setExtraColours(global.tuningPearlColor, global.tuningWheelColor);

      global.tuningColorType1 = type;
      global.tuningSelectedColor = { r: colors.r, g: colors.g, b: colors.b, type: type };

      break;
    case 'paint_two':
      localplayer.vehicle.setModColor2(type, 0);
      localplayer.vehicle.setCustomSecondaryColour(colors.r, colors.g, colors.b);
      localplayer.vehicle.setExtraColours(global.tuningPearlColor, global.tuningWheelColor);
      global.tuningSelectedColor = { r: colors.r, g: colors.g, b: colors.b, type: type };
      global.tuningColorType2 = type;

      break;
    case 'paint_three':
      localplayer.vehicle.setNeonLightsColour(colors.r, colors.g, colors.b);
      localplayer.vehicle.setNeonLightEnabled(0, true);
      localplayer.vehicle.setNeonLightEnabled(1, true);
      localplayer.vehicle.setNeonLightEnabled(2, true);
      localplayer.vehicle.setNeonLightEnabled(3, true);
      global.tuningSelectedColor = { r: colors.r, g: colors.g, b: colors.b, type: 0 };
      break;
    case 'paint_four': // Перламутр
      // mp.console.logInfo(`paint_four Перламутр: ${colors}`);
      localplayer.vehicle.setModColor1(global.tuningColorType1, 0, colors);
      localplayer.vehicle.setModColor2(global.tuningColorType2, 0);
      localplayer.vehicle.setExtraColours(colors, global.tuningWheelColor);
      global.tuningPearlColor = colors;

      break;
    case 'paint_fifth': // Цвет колес
      // mp.console.logInfo(`tuningPearlColor: ${tuningPearlColor} tuningWheelColor: ${tuningWheelColor}`);
      // mp.console.logInfo(`paint_fifth Цвет колес: ${colors}`);
      // const test = localplayer.vehicle.getModColor1(global.tuningColorType1, 0, global.tuningPearlColor);
      // mp.console.logInfo(`test : ${test.color}`);
      localplayer.vehicle.setModColor1(global.tuningColorType1, 0, global.tuningPearlColor);
      localplayer.vehicle.setModColor2(global.tuningColorType2, 0);
      localplayer.vehicle.setExtraColours(global.tuningPearlColor, colors);
      localplayer.vehicle.wheelColor = colors;
      global.tuningWheelColor = colors;
      break;
  }
});

mp.events.add('tuningTakeComponent', (item, vehicleComponents, id = null) => {
  if (id === undefined) return;

  if (id !== null) {
    id = parseInt(id);
  }

  global.tuningItem = JSON.parse(item);

  if (vehicleComponents !== null) {
    global.tuningVehicleComponents = JSON.parse(vehicleComponents);
  }

  let setted = false;
  switch (global.tuningItem.uniq_name) {
    case 'muffler':
      if (global.tuningVehicleComponents.Muffler === id) setted = true;
      break;
    case 'sideskirt':
      if (global.tuningVehicleComponents.SideSkirt === id) setted = true;
      break;
    case 'hood':
      if (global.tuningVehicleComponents.Hood === id) setted = true;
      break;
    case 'spoiler':
      if (global.tuningVehicleComponents.Spoiler === id) setted = true;
      break;
    case 'lattice':
      if (global.tuningVehicleComponents.Lattice === id) setted = true;
      break;
    case 'wings':
      if (global.tuningVehicleComponents.Wings === id) setted = true;
      break;
    case 'rearwings':
      if (global.tuningVehicleComponents.RearWings === id) setted = true;
      break;
    case 'roof':
      if (global.tuningVehicleComponents.Roof === id) setted = true;
      break;
    case 'vinyls':
      if (global.tuningVehicleComponents.Vinyls === id) setted = true;
      break;
    case 'frontbumper':
      if (global.tuningVehicleComponents.FrontBumper === id) setted = true;
      break;
    case 'rearbumper':
      if (global.tuningVehicleComponents.RearBumper === id) setted = true;
      break;
    case 'engine':
      if (global.tuningVehicleComponents.Engine === id) setted = true;
      break;
    case 'turbo':
      if (global.tuningVehicleComponents.Turbo === id) setted = true;
      break;
    case 'horn':
      if (global.tuningVehicleComponents.Horn === id) setted = true;
      break;
    case 'transmission':
      if (global.tuningVehicleComponents.Transmission === id) setted = true;
      break;
    case 'windowtint':
      if (global.tuningVehicleComponents.WindowTint === id) setted = true;
      break;
    case 'suspension':
      if (global.tuningVehicleComponents.Suspension === id) setted = true;
      break;
    case 'brakes':
      if (global.tuningVehicleComponents.Brakes === id) setted = true;
      break;
    case 'headlights':
      if (global.tuningVehicleComponents.Headlights === id) setted = true;
      break;
    case 'numberplate':
      if (global.tuningVehicleComponents.NumberPlate === id) setted = true;
      break;
    case 'wheels_exclusive':
    case 'wheels_lowrider':
    case 'wheels_musclecar':
    case 'wheels_4x4':
    case 'wheels_sport':
    case 'wheels_4x4_2':
    case 'wheels_tuner':
      if (global.tuningVehicleComponents.WheelsType === item.modId && global.tuningVehicleComponents.Wheels === id) setted = true;
      break;
  }

  if (setted) mp.events.call('notify', 1, 9, 'У Вас уже установлена данная модификация', 3000);
  else {
    // Hide tuning
    // mp.events.call('openTuningMenu');
    mp.events.call('hideTuningMenu');

    global.tuningSelected = id;

    let title = 'Вы действительно хотите установить данную модификацию?';

    switch (global.tuningItem.uniq_name) {
      case 'paint_one':
      case 'paint_two':
      case 'paint_three':
      case 'paint_four':
      case 'paint_fifth':
        title = 'Вы действительно хотите покрасить машину в данный цвет?';
        mp.events.call('CLIENT::colorPicker:exit');
        break;
    }

    mp.events.call('popup::open', 'tuningbuy', title);
  }
});

mp.events.add('tuningBuyComponent', (state) => {
  let item = global.tuningItem;
  let itemColor = null;
  // mp.console.logInfo("tuningBuyComponent item: "+item+" typeof: "+typeof item);
  if (typeof item !== 'object') {
    item = JSON.parse(item);
  }
  // mp.console.logInfo("tuningBuyComponent typeof: "+typeof item);

  // mp.console.logInfo("global.tuningItem: "+JSON.stringify(item));
  // if (global.tuningColorItem !== null && item.uniq_name === 'paint') {
  //   itemColor = JSON.parse(global.tuningColorItem);
  // //   mp.console.logInfo("itemColor.uniq_name: "+itemColor.uniq_name);
  // }

  if (state) {
    if (item && global.tuningWheelsTypes[item.uniq_name] !== undefined) {
      global.anyEvents.SendServer(() => mp.events.callRemote('buyTuning', 19, global.tuningSelected, item.modId));
    }
    else if (item.uniq_name === 'paint_one' || item.uniq_name === 'paint_two' || item.uniq_name === 'paint_three' || item.uniq_name === 'paint_four' || item.uniq_name === 'paint_fifth')
    {
      let paintType;
      let paintCategory;
      let paintValue;

      switch (item.uniq_name) {
        case 'paint_one':
          paintType = 0;
          break;
        case 'paint_two':
          paintType = 1;
          break;
        case 'paint_three':
          paintType = 2;
          break;
        case 'paint_four':
          paintType = 3;
          paintCategory = 120;
          paintValue = global.tuningPearlColor;
          break;
        case 'paint_fifth':
          paintType = 4;
          paintCategory = 121;
          paintValue = global.tuningWheelColor;
          break;
      }

      if (paintType === 3 || paintType === 4) global.anyEvents.SendServer(() => mp.events.callRemote('buyTuning', paintCategory, paintValue));
      else global.anyEvents.SendServer(() => mp.events.callRemote('buyTuning', 20, paintType, global.tuningSelectedColor.r, global.tuningSelectedColor.g, global.tuningSelectedColor.b, global.tuningSelectedColor.type));
      global.tuningColorItem = null;
    } else {
      global.anyEvents.SendServer(() => mp.events.callRemote('buyTuning', item.categoryModId, global.tuningSelected, -1));
    }
  }
  // Reopen tuning
  mp.events.call('reopenTuningMenu');
  mp.events.call('resetTuningCustomization');
});

const VEH_POSITION = new mp.Vector3(-337.7784, -136.5316, 39.4032);
const VEH_ROTATION = new mp.Vector3(0.04308624, 0.07037075, 148.9986);
const CAM_VIEWANGLE = 60;
const CAM_POSITION = new mp.Vector3(-333.7966, -137.409, 40.58963);
const CAM_ROTATION = new mp.Vector3(0, 0, 0);
const CAM_LOOKATPOSITION = new mp.Vector3(-337.7784, -136.5316, 37.88963);

let loadCamera = false;
let camAuto = null;

const cameraRotator = require('public/utils/cameraRotator');

async function createCam(pos, rot, viewangle) {
  global.tuningCam = mp.cameras.new('default');

  if (!global.tuningCam.doesExist() || global.tuningCam == null || global.tuningCam == undefined) await mp.game.waitAsync(20);

  global.tuningCam.setCoord(pos.x, pos.y, pos.z);
  global.tuningCam.setRot(rot.x, rot.y, rot.z, 2);
  global.tuningCam.setFov(viewangle);
  global.tuningCam.setActive(true);
  // global.tuningCam = mp.cameras.new('default', CAM_POSITION, CAM_ROTATION, CAM_VIEWANGLE);
  // global.tuningCam.pointAtCoord(CAM_LOOKATPOSITION.x, CAM_LOOKATPOSITION.y, CAM_LOOKATPOSITION.z);
  // global.tuningCam.setActive(true);

  cameraRotator.start(global.tuningCam, VEH_POSITION, VEH_POSITION, new mp.Vector3(-1.0, 1.5, 0.5), 260);
  // cameraRotator.setZBound(-1.4, 0.7); // Высота камеры, настраивается по полу и потолку
  // //cameraRotator.setXBound(0, 360);
  // cameraRotator.setLBound(-3, 4.5); // Максимальное приближение и отдаление
  cameraRotator.setZUpMultipler(5); // Скорость изменения высоты

  cameraRotator.setOffsetLength(3);
  cameraRotator.setZBound(-1.4, 0.7);
  cameraRotator.setLBound(-4, 3);

  mp.game.cam.renderScriptCams(true, false, 3000, true, false);

  cameraRotator.pause(false);

  loadCamera = true;
}

function destroyCam() {
  cameraRotator.stop();
  if (global.tuningCam != null) {
    global.tuningCam.destroy();
    global.tuningCam = null;
  }
  loadCamera = false;
  mp.game.cam.renderScriptCams(false, false, 500, true, false);
}

async function changeCam(item) {
  item = JSON.parse(item);

  //mp.console.logInfo(`tuningChangeCam item: ${JSON.stringify(item)}`);

  let { cam } = item;
  let isSubmenu = false;

  if (cam === null) {
    isSubmenu = true;
    cam = {
      heading: 148.9986,
      position: [-333.7966, -137.409, 40.58963],
    };
  }

  const camPosition = cam.position;
  const camHeanding = cam.heading;

  global.tuningCamFrom = null;

  if (global.tuningCam) {
    global.tuningCamFrom = global.tuningCam;
  }

  // const camFromPosition = global.tuningCam.getCoords();

  // mp.console.logError(`cam POS: ${cam.position}`, true, true);

  // global.tuningCam = mp.cameras.new('default');
  // global.tuningCam.setCoord(camPosition[0], camPosition[1], camPosition[2]);
  // global.tuningCam.setRot(0, 0, 0, 2);
  // global.tuningCam.setFov(60);
  // global.tuningCam.setActive(true);
  //global.tuningCam = mp.cameras.new('default', new mp.Vector3(camPosition[0], camPosition[1], camPosition[2]), new mp.Vector3(0, 0, 0), 60);
  if (!global.tuningCam.doesExist() || global.tuningCam == null || global.tuningCam == undefined) await mp.game.waitAsync(20);

  global.tuningCam.setCoord(camPosition[0], camPosition[1], camPosition[2]);
  global.tuningCam.pointAtCoord(VEH_POSITION.x, VEH_POSITION.y, VEH_POSITION.z);
  if (global.tuningCamFrom) global.tuningCam.setActiveWithInterp(global.tuningCamFrom.handle, 500, 1, 1);

  localplayer.vehicle.setHeading(camHeanding);
}

mp.events.add('tuningChangeCam', (item) => {
  changeCam(item);
  cameraRotator.pause(false);
});

mp.events.add('openColorPicker', (item, index) => {
  // global.tuningColorItem = item;
  global.tuningItem = item;

  if (index === 68) {
    mp.events.call('CLIENT::colorPicker:openBox');
    localplayer.vehicle.setModColor1(global.tuningColorType1, 0, global.tuningPearlColor);
    localplayer.vehicle.setModColor2(global.tuningColorType1, 0);
    localplayer.vehicle.setExtraColours(global.tuningPearlColor, global.tuningWheelColor);
    return;
  }

  if (index === 69) {
    mp.events.call('CLIENT::colorPicker:openBox');
    localplayer.vehicle.wheelColor = global.tuningWheelColor;
    localplayer.vehicle.setExtraColours(global.tuningPearlColor, global.tuningWheelColor);
    return;
  }

  // //mp.console.logInfo(`item: ${item}`);
  mp.events.call('CLIENT::colorPicker:open', 'tuning');

  if (index === 66) {
    localplayer.vehicle.getCustomPrimaryColour(1, 1, 1);
  }

  if (index === 67) {
    localplayer.vehicle.getCustomSecondaryColour(1, 1, 1);
  }

  if (index === 'neon') {
    localplayer.vehicle.getNeonLightsColour(1, 1, 1);
  }
});

