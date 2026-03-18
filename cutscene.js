const localPlayer = mp.players.local;
global.cutscenePlaying = false;

const thisMenu = 'cutscene';
const thisHTML = 'FeST1VaL';
const thisMenuCall = global.festMenu;

const callbackOnClose = () => {
  thisMenuCall.call('CEF::cutscene:closeOnCallback');
};

let PLAYER = {
  defaultPos: localPlayer.position,
  entity: localPlayer,
  pos: [
    new mp.Vector3(158.99527, 6555.081, 31.981355), // Scene1
    new mp.Vector3(293.45306, -784.8559, 29.315368), // Scene2 BusExit
  ],
  exit: {
    pos: new mp.Vector3(300.29175, -777.128, 29.30891),
    rot: new mp.Vector3(0, 0, 117.336426),
  }
};

let CLONE = {
  entity: null,
  pos: [
    new mp.Vector3(140.07083, 6567.935, 32.141887),
    new mp.Vector3(325.30978, -730.0081, 29.351762)
  ],
  rot: 270.0,
};

let PED = {
  entity: null,
  pos: [
    new mp.Vector3(141.55641, 6576.4653, 31.953048),
    new mp.Vector3(323.77628, -732.3841, 29.351731)
  ],
  rot: 270.0,
}

let CAM = {
  entity: null,
  pos: [
    new mp.Vector3(145.58028, 6563.2944, 32.572866),
    new mp.Vector3(304.12943, -755.5896, 39.121472),
  ],
  rot: [
    new mp.Vector3(0, 0, 6.64673),
    new mp.Vector3(0, 0, 170.26056),
  ],
  angle: 60,
  pointAtCoord: [
    new mp.Vector3(145.58028, 6563.2944, 32.572866),
    new mp.Vector3(302.7307, -777.73987, 29.20511)
  ],
  skip: false,
  rendered: false
}

let BUS = {
  entity: null,
  startMoving: false,
  driveToCoord: [
    {x:145.1225, y:6528.722, z:30.578833},
    {x:305.65662, y:-775.52026, z:29.303202}
  ]
}

let BUS2 = {
  entity: null,
  startMoving: false,
  driveToCoord: [
    {x:145.1225, y:6528.722, z:30.578833},
    {x:305.65662, y:-775.52026, z:29.303202}
  ]
}

class CUTSCENE {
  static clear() {
    if (CLONE.entity !== null) {
      CLONE.entity.destroy();
      CLONE.entity = null;
    }

    if (BUS.entity !== null || BUS2.entity !== null) {
      if (BUS.entity !== null && BUS.entity !== undefined) {
        BUS.entity.destroy();
        BUS.entity = null;
      }

      if (BUS2.entity !== null && BUS2.entity !== undefined) {
        BUS2.entity.destroy();
        BUS2.entity = null;
      }

      if (CAM.entity !== null) {
        if (CAM.rendered) {
          mp.game.cam.renderScriptCams(false, false, 500, true, false);

          CAM.rendered = false;
        }

        CAM.entity.destroy();
        CAM.entity = null;
        CAM.skip = false;
      }

      if (PED.entity !== null) {
        PED.entity.destroy();
      }
    }
  }

  static createPlayerClone(clonePos, cloneRot) {
    CLONE.entity = mp.peds.new(PLAYER.entity.model, clonePos, cloneRot, PLAYER.entity.dimension);

    setTimeout(() => {
      PLAYER.entity.cloneToTarget(CLONE.entity.handle);

      CLONE.entity.clearTasksImmediately();
      CLONE.entity.setInvincible(false);
      CLONE.entity.freezePosition(false);
    }, 250);
  }

  static createBUSPed(pedPos, pedRot) {
    PED.entity = mp.peds.new(PLAYER.entity.model, pedPos, pedRot, PLAYER.entity.dimension);

    PED.entity.clearTasksImmediately();
    PED.entity.setInvincible(false);
    PED.entity.freezePosition(false);
  }

  static createCam(cameraPos, cameraRot, angle, pointAtCoord) {
    CAM.entity = mp.cameras.new('default', cameraPos, cameraRot, angle);
    CAM.entity.pointAtCoord(pointAtCoord.x, pointAtCoord.y, pointAtCoord.z);
    CAM.entity.setActive(true);
    CAM.rendered = true;
    mp.game.cam.renderScriptCams(true, false, 0, true, false);
  }

  static exitBus() {
    CLONE.entity.taskLeaveVehicle(BUS2.entity.handle, 0);

    mp.game.cam.doScreenFadeOut(4000);

    setTimeout(() => {
      CUTSCENE.clear();

      mp.events.callRemote("SERVER::cutscene:deleteBus");

      PLAYER.entity.setAlpha(255);

      mp.events.callRemote("SERVER::cutscene:returnPlayer");

      setTimeout(() => {
        // Меняем куда смотрит камерой игрок.
        mp.game.cam.setGameplayCamRelativeHeading(0);
        mp.game.cam.doScreenFadeIn(500);
        global.cutscenePlaying = false;

        CUTSCENE.showHUD();
      }, 1000);
    }, 5000);
  }

  static hideHUD() {
    mp.gui.cursor.visible = true;
    mp.events.call('showHUD', false);
    global.INTERACTIONCHECK = false;
  }

  static showHUD() {
    mp.gui.cursor.visible = false;
    mp.events.call('showHUD', true);
    global.INTERACTIONCHECK = false;
  }
}

mp.events.add("CLIENT::cutscene:busStart", async (busEntity) => {
  try {
    global.anyMenuHTML.closeAllMenu();

    global.cutscenePlaying = true;

    CUTSCENE.clear();

    // Делаем невидимым персонажа
    PLAYER.entity.setAlpha(0);

    // Затемняем экран
    mp.game.cam.doScreenFadeOut(0);

    // Скрываем худ
    CUTSCENE.hideHUD();

    // Создаем ped-клона
    CUTSCENE.createPlayerClone(CLONE.pos[0], CLONE.rot);

    // Создаем камеру
    CUTSCENE.createCam(CAM.pos[0], CAM.rot[0], CAM.angle, CAM.pointAtCoord[0]);

    BUS.entity = busEntity;

    CUTSCENE.createBUSPed(PED.pos[0], PED.rot);

    setTimeout(() => {
      //mp.game.audio.triggerMusicEvent("FM_INTRO_START");
      //Сажаем ped-водителя в автобус
      PED.entity.taskEnterVehicle(BUS.entity.handle, -1, -1, 1.0, 16, 0);

      setTimeout(() => {
        CLONE.entity.taskEnterVehicle(BUS.entity.handle, 5000, -2, 1.0, 1, 0);
      }, 1000);


      setTimeout(() => {
        //Приднудительно закрываем дверь тк персонаж пидр.
        BUS.entity.setDoorShut(0, true);

        setTimeout(() => {
          //https://vespura.com/fivem/drivingstyle/
          PED.entity.taskVehicleDriveToCoord(BUS.entity.handle,  BUS.driveToCoord[0].x, BUS.driveToCoord[0].y, BUS.driveToCoord[0].z, 35, 1, -2072933068, 2883621, 30, true);
        }, 1000);
      }, 9000);

    }, 1000);

    setTimeout(() => {
      mp.game.cam.doScreenFadeIn(500);
    }, 2000);

    setTimeout(() => {
      mp.game.cam.doScreenFadeOut(1000);
    }, 13000);

    setTimeout(() => {
      CUTSCENE.clear();
      mp.events.callRemote("SERVER::cutscene:deleteBus");

      mp.events.call("CLIENT::cutscene:startFly", '0');
    }, 15000);
  } catch (e) {logger.error(e);}
});

mp.events.add("CLIENT::cutscene:busEnd", async (busEntity) => {
  try {
    global.cutscenePlaying = true;
    BUS2.startMoving = false;
    waitBusSTOP = false;

    CUTSCENE.clear();

    // Делаем невидимым персонажа
    PLAYER.entity.setAlpha(0);

    // Затемняем экран
    mp.game.cam.doScreenFadeOut(0);

    // Скрываем худ
    CUTSCENE.hideHUD();

    // Создаем ped-клона
    CUTSCENE.createPlayerClone(CLONE.pos[1], CLONE.rot);

    // Создаем камеру
    CUTSCENE.createCam(CAM.pos[1], CAM.rot[1], CAM.angle, CAM.pointAtCoord[1]);

    BUS2.entity = busEntity;

    CUTSCENE.createBUSPed(PED.pos[1], PED.rot);

    setTimeout(() => {
      PED.entity.taskEnterVehicle(BUS2.entity.handle, -1, -1, 1.0, 16, 0);

      setTimeout(() => {
        CLONE.entity.taskEnterVehicle(BUS2.entity.handle, -1, 0, 1.0, 16, 0);
      }, 1000);

      //https://vespura.com/drivingstyle/ VPN
      setTimeout(() => {
        PED.entity.taskVehicleDriveToCoord(BUS2.entity.handle,  BUS2.driveToCoord[1].x, BUS2.driveToCoord[1].y, BUS2.driveToCoord[1].z, 10, 1, -2072933068, 16777216, 1.0, true);

        setTimeout(() => {
          BUS2.startMoving = true;
        }, 1500);

        setTimeout(() => {
          mp.game.cam.doScreenFadeIn(500);
        }, 500);

      }, 1000);
    }, 1500);

  } catch (e) {logger.error(e);}
});


const cutScenesCams = {
  0: {
    music: true,
    voice: true,
    points: [
      { // meria
        title: "Мэрия",
        text: "Мэрия - сердце нашего штата. Ведь именно здесь происходят все важные процессы: от устройства на работу и получения лицензий до создания собственной семьи.",
        position: 7,
        startPosition: new mp.Vector3(-529.0831, -295.24442, 72.27632),
        startRotation: new mp.Vector3(0, 0, 8.674995),
        endPosition: new mp.Vector3(-480.6087, -272.0082, 72.15339),
        endRotation: new mp.Vector3(0, 0, 40.01006),
        speed: 6.9,
        delay: 3000,
        timing: {
          timeStart: 6.34,
          timeEnd: 16.03
        }
      },
      { // family
        title: "Семьи",
        text: "К слову, семьи - это отличный вариант для совместного развития вместе с вашими единомышлениками или друзьями.",
        position: 7,
        startPosition: new mp.Vector3(-38.54848, -387.84335, 146.30661),
        startRotation: new mp.Vector3(0, 0, 153.7479),
        endPosition: new mp.Vector3(-155.50278, -361.71295, 146.30661),
        endRotation: new mp.Vector3(0, 0, 178.17308),
        speed: 23,
        delay: 3000,
        timing: {
          timeStart: 18.00,
          timeEnd: 24.80
        }
      },
      { // frac
        title: "Фракции",
        text: "У нас вы можете стать кем захотите, начиная от обычного работяги и заканчивая Губернатором целого штата. Но будьте осторожны, некоторые зарабатывают в меру своих возможностей, используя скользкие тропы. Хотя, выбор все равно останется за вами!",
        position: 7,
        startPosition: new mp.Vector3(-1833.5211, 339.71564, 144.16109),
        startRotation: new mp.Vector3(0, 0, -11.905647),
        endPosition: new mp.Vector3(-1755.177, 342.84927, 144.16109),
        endRotation: new mp.Vector3(0, 0, 30.584902),
        speed: 6.3,
        delay: 3000,
        timing: {
          timeStart: 27.00,
          timeEnd: 45.11
        }
      },
      { // avto
        title: "Автосалоны",
        text: "Так же в нашем штате представлен огромный ассортимент транспортных средств, благодаря чему вы сможете купить себе то, о чем давно мечтали, будь то скутер, гиперкар или эксклюзивный автомобиль. А может и все сразу!",
        position: 7,
        startPosition: new mp.Vector3(-805.21967, -308.72946, 47.23857),
        startRotation: new mp.Vector3(0, 0, -66.32518),
        endPosition: new mp.Vector3(-805.76666, -263.69336, 47.23857),
        endRotation: new mp.Vector3(0, 0, -108.98761),
        speed: 4.55,
        delay: 3000,
        timing: {
          timeStart: 47.53,
          timeEnd: 60.46
        }
      },
      { // lsc
        title: "Los Santos Customs",
        text: "Если вас не устраивает ваш транспорт на все 100%, вы можете его модифицировать. В этом вам поможет Los Santos Customs, где вы сможете найти улучшения на любой вкус.",
        position: 7,
        startPosition: new mp.Vector3(-376.6633, -103.64183, 45.650166),
        startRotation: new mp.Vector3(0, 0, -128.68018),
        endPosition: new mp.Vector3(-386.4858, -131.76881, 45.650166),
        endRotation: new mp.Vector3(0, 0, -86.527664),
        speed: 3.6,
        delay: 3000,
        timing: {
          timeStart: 62.76,
          timeEnd: 73.50
        }
      },
      { // rieltor
        title: "Риелторское агенство",
        text: "Правда, в отсутсвии дома вам придется пользоваться парковками, что в долгосрочной перспективе не так уж и выгодно. Но если вы уже задумываетесь о его покупке, то Агенство по недвижимости поможет вам подобрать лучший вариант!",
        position: 7,
        startPosition: new mp.Vector3(-849.63257, -1337.2776, 10.989991),
        startRotation: new mp.Vector3(0, 0, -115.60526),
        endPosition: new mp.Vector3(-828.457, -1314.4276, 10.989991),
        endRotation: new mp.Vector3(0, 0, -150.70467),
        speed: 3.2,
        delay: 3000,
        timing: {
          timeStart: 75.80,
          timeEnd: 89.19
        }
      },
      { // bank
        title: "Банки",
        text: "Главное - не забывайте платить налоги за дом в срок, это можно сделать в любом банке или банкомате. Однако это не единственная их польза! В них вы можете взаимодействовать со всеми своими финансами!",
        position: 7,
        startPosition: new mp.Vector3(148.06288, -1018.42004, 30.546556),
        startRotation: new mp.Vector3(0, 0, 171.5726),
        endPosition: new mp.Vector3(165.47891, -1026.2374, 30.546556),
        endRotation: new mp.Vector3(0, 0, 122.8712),
        speed: 2.4,
        delay: 3000,
        timing: {
          timeStart: 91.26,
          timeEnd: 102.92
        }
      },
      { // odejda
        title: "Магазин одежды",
        text: "Очевидно, каждый из нас хочет идти в ногу со временем, выглядеть стильно и опрятно. В этом нам помогут магазины одежды, которые можно найти почти в каждом уголке нашего штата.",
        position: 7,
        startPosition: new mp.Vector3(-741.8081, -152.90665, 40.784187),
        startRotation: new mp.Vector3(0, 0, -89.52423),
        endPosition: new mp.Vector3(-728.0915, -179.10173, 40.784187),
        endRotation: new mp.Vector3(0, 0, -32.89308),
        speed: 3.8,
        delay: 3000,
        timing: {
          timeStart: 105.00,
          timeEnd: 115.61
        }
      },
      { // riba
        title: "Рыболовный магазин",
        text: "А если вдруг вы ярый поклонник рыбалки, то вы сможете неплохо заработать на своем хобби и отдохнуть в атмосферной компании друзей и знакомых. Ведь у нас огромная популяция рыбы. Хотите испытать свои силы в этом направлении? Тогда запасайтесь наживками, берите удочку и отправляйтесь на близжайший пирс!",
        position: 7,
        startPosition: new mp.Vector3(-1659.3248, -1021.307, 13.567197),
        startRotation: new mp.Vector3(0, 0, -164.22075),
        endPosition: new mp.Vector3(-1673.8625, -1034.1167, 13.567197),
        endRotation: new mp.Vector3(0, 0, -104.73627),
        speed: 1.35,
        delay: 3000,
        timing: {
          timeStart: 117.80,
          timeEnd: 136.96
        }
      },
      { // casino
        title: "Казино",
        text: "И напоследок - казино. Это место прекрасно подойдет для тех, кто после тяжелого рабочего дня хочет хорошенько отдохнуть и испытать свою удачу. Вас будет сопровождать бесконечный звон бокалов, баснословные ставки и веселые (а может и грустные) лица окружающих Вас людей.",
        position: 7,
        startPosition: new mp.Vector3(821.27563, 108.415115, 100.80487),
        startRotation: new mp.Vector3(0, 0, -122.42919),
        endPosition: new mp.Vector3(804.82166, 46.346363, 100.80487),
        endRotation: new mp.Vector3(0, 0, -93.94174),
        speed: 4.5,
        delay: 3000,
        timing: {
          timeStart: 139.19,
          timeEnd: 157.88
        }
      }
    ]
  }
};

mp.events.add("CLIENT::cutscene:startFly", (cutSceneName) => {
  try {
    global.cutscenePlaying = true;
    startCutscene(cutSceneName);
    mp.game.audio.startAudioScene("CHARACTER_CHANGE_IN_SKY_SCENE");
  } catch (e) { logger.error(e) }
});

mp.events.add("CLIENT::cutscene:stopFly", () => {
  try {
    destroyMoveCam();
    mp.game.audio.stopAudioScene("CHARACTER_CHANGE_IN_SKY_SCENE");
  } catch (e) { logger.error(e) }
});

mp.events.add("CLIENT::cutscene:skipFly", () => {
  try {
    CAM.skip = true;
    finishMoveCam();
    mp.game.audio.stopAudioScene("CHARACTER_CHANGE_IN_SKY_SCENE");
  } catch (e) { logger.error(e) }
});

mp.events.add("CLIENT::CAM:forceDestroy", () => {
  try {
    CAM.rendered = false;
    mp.game.cam.renderScriptCams(false, false, 500, true, false);
  } catch (e) { logger.error(e) }
});

function startCutscene(cutSceneName) {
  try {
    if (!cutSceneName) return;
    if (!cutScenesCams[cutSceneName]) return;
    if (!cutScenesCams[cutSceneName].points.length) return;

    // Скрываем худ
    CUTSCENE.hideHUD();

    mp.cutSceneName = cutSceneName;
    mp.amountPointsForMoveCam = cutScenesCams[cutSceneName].points.length;
    mp.indexPathForMoveCam = 0;

    mp.game.cam.doScreenFadeOut(0);

    if (cutSceneName == 0) {
      thisMenuCall.call('CEF::cutscene:open', cutScenesCams[cutSceneName].points);

      global.anyMenuHTML.openAnyMenu(thisHTML, thisMenu, false);
    }

    PLAYER.entity.setAlpha(0);
    PLAYER.entity.freezePosition(true);

    setTimeout(() => {
      camPathHandler(0);
    }, 3690); // 3690 - время начального приветствия.

  } catch (e) { logger.error(e) }
}

function camPathHandler(index, direction = 'circle', currentDirectionCam = 'toEnd', init = true) {
  try {
    let startPos, startRot, endPos, endRot, speed, delay, angle;
    const currentPoint = cutScenesCams[mp.cutSceneName].points[index];

    if (direction === 'any') {
      startPos = currentPoint.startPosition;
      startRot = currentPoint.startRotation;
      endPos = currentPoint.endPosition;
      endRot = currentPoint.endRotation;
      speed = currentPoint.speed;
      delay = currentPoint.delay;
    }

    if (direction === 'circle') {
      switch (currentDirectionCam) {
        case "toEnd":
          startPos = currentPoint.startPosition;
          startRot = currentPoint.startRotation;
          endPos = currentPoint.endPosition;
          endRot = currentPoint.endRotation;
          speed = currentPoint.speed;
          delay = currentPoint.delay;
          break;
        case "toStart":
          startPos = currentPoint.endPosition;
          startRot = currentPoint.endRotation;
          endPos = currentPoint.startPosition;
          endRot = currentPoint.startRotation;
          speed = currentPoint.speed;
          delay = currentPoint.delay;
      }
    }


    if (init) {
      mp.game.cam.doScreenFadeOut(0);

      setTimeout(() => {
        if (CAM.skip) return;
        mp.game.cam.doScreenFadeIn(500);
      }, delay);

      PLAYER.entity.position = startPos;

      thisMenuCall.call('CEF::cutscene:showFrame', index);
    }


    // setTimeout(() => {
    //   PLAYER.entity.position = startPos;
    //
    //   thisMenuCall.call('CEF::cutscene:showFrame', index);

      if (!angle) angle = 60;

      if (!mp.movingcam) mp.movingcam = mp.cameras.new('default', startPos, startRot, angle);
      else {
        mp.movingcam.setRot(startRot.x, startRot.y, startRot.z, 2);
        mp.movingcam.setCoord(startPos.x, startPos.y, startPos.z);
      }

      if (init) {
        const timeDuration = parseFloat(currentPoint.timing.timeEnd) - parseFloat(currentPoint.timing.timeStart);
        // mp.console.logInfo(`currentPoint.timing.timeEnd: ${currentPoint.timing.timeEnd} currentPoint.timing.timeStart: ${currentPoint.timing.timeStart} timeDuration: ${timeDuration.toFixed(2)}`);

        mp.movingcam.timeDuration = timeDuration + delay / 1000;
        mp.movingcam.currentTimeDuration = 0;
        mp.movingcam.timeStart = Date.now();
      }

      mp.movingcam.currentDirection = currentDirectionCam;

      mp.movingcam.setActive(true);
      CAM.rendered = true;
      mp.game.cam.renderScriptCams(true, false, 0, true, false);

      let dx = endPos.x - startPos.x;
      let dy = endPos.y - startPos.y;
      let dz = endPos.z - startPos.z;

      let length = Math.sqrt(dx * dx + dy * dy + dz * dz);

      mp.movingcam.dx = (dx / length) * (speed / 100);
      mp.movingcam.dy = (dy / length) * (speed / 100);
      mp.movingcam.dz = (dz / length) * (speed / 100);

      let amountFrame = Math.round(dx / mp.movingcam.dx);
      if (amountFrame == null || isNaN(amountFrame)) amountFrame = 1;

      mp.movingcam.rx = (endRot.x - startRot.x) / amountFrame;
      mp.movingcam.ry = (endRot.y - startRot.y) / amountFrame;
      mp.movingcam.rz = (endRot.z - startRot.z) / amountFrame;

      mp.movingcam.startmove = true;
      mp.movingcam.isCutScene = true;
      mp.drawDescription = true;

      mp.movingcam.endX = endPos.x;
      mp.movingcam.endY = endPos.y;
      mp.movingcam.endZ = endPos.z;

    // }, 1250);
  } catch (e) { logger.error(e) }
}

function destroyMoveCam() {
  if (!mp.movingcam) return;

  mp.game.cam.doScreenFadeIn(500);

  mp.movingcam.setActive(false);
  mp.movingcam.startmove = false;
  CAM.rendered = false;
  mp.game.cam.renderScriptCams(false, false, 0, false, false);

  callbackOnClose();
  global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
}

function finishMoveCam() {
  try {
    if (!mp.movingcam) return;

    PLAYER.entity.setAlpha(255);
    PLAYER.entity.position = PLAYER.pos[1];
    PLAYER.entity.freezePosition(false);

    mp.movingcam.setActive(false);
    mp.movingcam.startmove = false;
    CAM.rendered = false;
    mp.game.cam.renderScriptCams(false, false, 0, false, false);

    callbackOnClose();
    global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);

    mp.events.callRemote('SERVER::cutscene:busEnd');
  } catch (e) { logger.error(e) }
}

function moveCamToPoint() {
  try {
    let pos = mp.movingcam.getCoord();
    let rot = mp.game.cam.getGameplayCamRot(0);

    mp.movingcam.currentTimeDuration = parseInt((Date.now() - mp.movingcam.timeStart) / 1000);

    mp.movingcam.setCoord(pos.x += mp.movingcam.dx, pos.y += mp.movingcam.dy, pos.z += mp.movingcam.dz);
    mp.movingcam.setRot(rot.x + mp.movingcam.rx, rot.y + mp.movingcam.ry, rot.z + mp.movingcam.rz, 2);

    if (Math.abs(mp.movingcam.endX - pos.x) < Math.abs(mp.movingcam.dx) || Math.abs(mp.movingcam.endY - pos.y) < Math.abs(mp.movingcam.dy) || Math.abs(mp.movingcam.endZ - pos.z) < Math.abs(mp.movingcam.dz)) {
      if (!mp.movingcam.isCutScene) {
        mp.movingcam.startmove = false;
        return;
      }

      if (mp.movingcam.currentTimeDuration < mp.movingcam.timeDuration) {
        switch (mp.movingcam.currentDirection) {
          case "toEnd":
            camPathHandler(mp.indexPathForMoveCam, 'circle', 'toStart', false);
            break;
          case "toStart":
            camPathHandler(mp.indexPathForMoveCam, 'circle', 'toEnd', false);
            break;
        }
      }
    }

    if (mp.movingcam.currentTimeDuration >= mp.movingcam.timeDuration) {
      if (mp.indexPathForMoveCam == mp.amountPointsForMoveCam - 1) {
        finishMoveCam();
      } else {
        mp.indexPathForMoveCam++;
        camPathHandler(mp.indexPathForMoveCam);
      }
    }


  } catch (e) { logger.error(e) }
}

let waitBusSTOP = false;

mp.events.add("render", () => {
  if (mp.movingcam) {
    if (mp.movingcam.startmove) {
      //drawDebugText();
      moveCamToPoint();
    }
  }

  if (BUS2.startMoving && CLONE.entity.isInVehicle(BUS2.entity.handle, false)) {
    let currentSpeed = BUS2.entity.getSpeed();

    if (currentSpeed > 0 && !waitBusSTOP) waitBusSTOP = true;

    if (currentSpeed === 0 && waitBusSTOP) {
      BUS2.startMoving = false;
      CUTSCENE.exitBus();
    }
  }
});


function drawDebugText() {
  const currentTimeDuration = parseInt((Date.now() - mp.movingcam.timeStart) / 1000);
  // let message = `timeStart: ${mp.movingcam.timeStart.toFixed(2)}`;
  let message = `\ncurrent: ${currentTimeDuration.toFixed(2)}`;
  message += `\nduration: ${mp.movingcam.timeDuration.toFixed(2)}`;

  mp.game.graphics.drawText(message, [0.5, 0.005], {
    font: 4,
    color: [255, 255, 255, 185],
    scale: [0.8, 0.8],
    outline: true,
    centre: true
  });
}
