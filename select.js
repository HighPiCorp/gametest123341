mp.game.streaming.requestAnimDict("amb@world_human_muscle_flex@arms_at_side@base");
mp.game.streaming.requestAnimDict("amb@world_human_muscle_flex@arms_at_side@idle_a");
mp.game.streaming.requestAnimDict("amb@world_human_muscle_flex@arms_in_front@idle_a");

mp.game.streaming.requestAnimDict("anim@mp_player_intuppersalute");
mp.game.streaming.requestAnimDict("anim@mp_player_intselfiethe_bird");
mp.game.streaming.requestAnimDict("anim@amb@nightclub@peds@");

mp.game.streaming.requestAnimDict("anim@mp_player_intupperwave");

mp.game.streaming.requestAnimDict("anim@mp_player_intupperthumbs_up");
mp.game.streaming.requestAnimDict("amb@world_human_cop_idles@male@base");

mp.game.streaming.requestAnimDict("anim@amb@nightclub@dancers@crowddance_groups@");
mp.game.streaming.requestAnimDict("anim@mp_player_intupperfinger");


const ANIMS = [
    ["amb@world_human_muscle_flex@arms_at_side@base", "base"],

    ["amb@world_human_muscle_flex@arms_at_side@idle_a", "idle_a"],
    ["amb@world_human_muscle_flex@arms_at_side@idle_a", "idle_c"],

    ["amb@world_human_muscle_flex@arms_in_front@idle_a", "idle_a"],
    ["amb@world_human_muscle_flex@arms_in_front@idle_a", "idle_b"],

    ["anim@amb@nightclub@peds@", "rcmme_amanda1_stand_loop_cop"],

    ["anim@mp_player_intupperthumbs_up", "idle_a"],

    ["amb@world_human_cop_idles@male@base", "base"],

    ["anim@mp_player_intupperfinger", "idle_a"],
    ["anim@mp_player_intuppersalute", "idle_a"],
    ["anim@mp_player_intupperwave", "idle_a"],

    ["anim@amb@nightclub@dancers@crowddance_groups@", "hi_dance_crowd_09_v1_female^1"],
    ["anim@amb@nightclub@dancers@crowddance_groups@", "hi_dance_crowd_09_v2_female^1"],
    ["anim@amb@nightclub@dancers@crowddance_groups@", "hi_dance_crowd_09_v2_female^3"],
    ["anim@amb@nightclub@dancers@crowddance_groups@", "hi_dance_crowd_11_v1_female^1"],
];


let authCharNum;
let authCharacters;
let authInformation;
const authPeds = [];
const authSelectMarkers = [];
let authCurrentCharacter = 0;

/// ИЗМЕНЯТЬ ДАННЫЕ НАСТРОЙки ДЛЯ УСТАНОВКИ ПЕДОВ
/// Начальная координата камеры
// -204.4649, 6612.0166, 0.17672324
const authCamPos = [-217.65994, 6557.716, 11.811612];// [1220.15, 195.36, 80.5];//[-1828.8, -870.1, 3.1];
/// На сколько ниже камера смотрит, чем находится
const authCamPosZDelta = -0.4;
/// Расстояние от камеры до текущего педа
const authCamDist = 2.5;
/// Расстояние между педами
const authPedDist = 2.5;
/// Поворот линии педов
const authPedsRotation = 225;
/// Поворот педа
const authPedRotation = 120;
/// Поворот камеры
const authCamRotation = 70;

const authCosCamRot = Math.cos(authCamRotation * Math.PI / 180);
const authSinCamRot = Math.sin(authCamRotation * Math.PI / 180);
const authCosPedRot = Math.cos((authPedRotation - 90) * Math.PI / 180);
const authSinPedRot = Math.sin((authPedRotation - 90) * Math.PI / 180);
let authCreatorTimer = null;

const PED_DEFAULT_POS = [
  new mp.Vector3(914.9882, 41.98926, 111.78104),
  //new mp.Vector3(427.87964, -754.4912, -144.86906),
  // new mp.Vector3(427.34305, -757.2979, -145.3143),

  new mp.Vector3(913.8323, 40.765842, 111.7813),
  //new mp.Vector3(427.4235, -756.85284, -144.86906),
  // new mp.Vector3(425.62546, -755.0478, -145.36642),

  new mp.Vector3(916.1949, 40.12284, 111.7813),
  //new mp.Vector3(426.02527, -755.0878, -144.86906),
];

const PED_SECOND_POS = [
  // new mp.Vector3(428.83246, -756.62897, -144.86641),
  new mp.Vector3(914.9882, 41.98926, 111.78104),

  // new mp.Vector3(428.7704, -755.0786, -144.86906),
  new mp.Vector3(914.0489, 41.411003, 111.7813),

  // new mp.Vector3(427.1762, -753.7896, -144.86906),
  new mp.Vector3(916.1707, 40.974056, 111.7813),
];

const PED_DEFAULT_ROT = [
  -14.306335,
  -7.121455,
  8.990127,
];

const PED_SECOND_ROT = [
  -14.306335,
  -7.121455,
  8.990127,
];


const PODIUM_POS = new mp.Vector3(427.87964, -754.4912, -144.86906);
const PODIUM_ROT = -36.72477;

var peds = [];

let anims = [
  0,0,0
];


let secondAtPod = false;
let thirdAtPod = false;

let pedNow = 0;

mp.events.add('characters.init', (count, characters) => {
  authCharacters = JSON.parse(characters);

  authCharNum = count;

  mp.events.call('characters.ped.create');

  // Устанавливаем позицию игрока
  mp.players.local.position = new mp.Vector3(916.58545, 52.07417, 111.78098);

  // Создание камеры на пристани
  //mp.utils.cam.create(authCamPos[0], authCamPos[1], authCamPos[2], authCamPos[0] + authCamDist * authSinCamRot, authCamPos[1] + authCamDist * authCosCamRot, authCamPos[2] + authCamPosZDelta, 60);

  mp.utils.cam.create(915.5094, 43.45662, 112.380956, 914.8697, 40.671707, 112.099916, 50, 0, 0, 0.0);
});


mp.events.add('characters.ped.create', () => {
  if (peds.length !== 0) return;
  authCreatorTimer = mp.timer.add(async () => {
    for (let indexPed = 0; indexPed < 3; indexPed++) {
      if (authCharacters[indexPed].isEmpty === true) {
        peds.push(null);
        authSelectMarkers.push("empty");
        continue;
      }

      if (authCharacters[indexPed].hasOwnProperty("banned")) {
        peds.push(null);
        authSelectMarkers.push("banned");
        continue;
      }

      setCharacterCustom(indexPed);
      setCharacterClothes(indexPed);
      setCharacterTattoos(indexPed);
      const ped = mp.peds.new(mp.players.local.model, PED_DEFAULT_POS[indexPed], PED_DEFAULT_ROT[indexPed], mp.players.local.dimension)
      peds.push(ped);

      mp.players.local.cloneToTarget(ped.handle);
    }
    authCreatorTimer = null;
  }, 500);
});

mp.events.add('characters.ped.remove', (index) => {
  if (peds.length === 0) return;

  peds[index].destroy();
  // authSelectMarkers[index - 1].destroy();
});

mp.events.add('characters.ped.updateMarkers', () => {
  for (let i = 0; i < authSelectMarkers.length; i++) {
    if (authSelectMarkers[i] === "empty") continue;
    authSelectMarkers[i].destroy();

    const x = (authCamPos[0] + i * authPedDist * authSinPedRot) + authCamDist * authSinCamRot;
    const y = (authCamPos[1] + i * authPedDist * authCosPedRot) + authCamDist * authCosCamRot;
    const z = mp.game.gameplay.getGroundZFor3dCoord(x, y, authCamPos[2] + 1, 0.0, false) + 1;

    authSelectMarkers[i] = mp.markers.new(
      2,
      new mp.Vector3(x, y, z + 1),
      0.2,
      {
        direction: 0,
        rotation: new mp.Vector3(0, 180, 0),
        color: (i === authCurrentCharacter) ? [255, 221, 85, 255] : [255, 255, 255, 120],
        visible: true,
        dimension: mp.players.local.dimension,
      },
    );
  }
});

mp.events.add('characters.cam.slide', (vector = 'right', currentCharacter) => {
  authCurrentCharacter = currentCharacter;
  mp.events.call('characters.ped.updateMarkers');
  mp.utils.cam.moveTo(
    authCamPos[0] + currentCharacter * authPedDist * authSinPedRot,
    authCamPos[1] + currentCharacter * authPedDist * authCosPedRot,
    authCamPos[2],
    (authCamPos[0] + currentCharacter * authPedDist * authSinPedRot) + authCamDist * authSinCamRot,
    (authCamPos[1] + currentCharacter * authPedDist * authCosPedRot) + authCamDist * authCosCamRot,
    authCamPos[2] + authCamPosZDelta,
    500,
  );
});

const freemodeCharacters = [mp.game.joaat('mp_m_freemode_01'), mp.game.joaat('mp_f_freemode_01')];

let setCharacterCustom = function (indexPed) {
  if (authCharacters.length < indexPed) return;

  const character = authCharacters[indexPed];
  const customization = character.customization.custom;

  mp.players.local.model = freemodeCharacters[parseInt(customization.Gender)];
  mp.players.local.setHeadBlendData(
    // shape
    parseInt(customization.Parents.Mother),
    parseInt(customization.Parents.Father),
    0,

    // skin
    parseInt(customization.Parents.Mother),
    parseInt(customization.Parents.Father),
    0,

    // mixes
    Math.floor(customization.Parents.Similarity),
    Math.floor(customization.Parents.SkinSimilarity),
    0.0,

    false,
  );

  mp.players.local.setComponentVariation(2, parseInt(customization.Hair.Hair), 0, 0);
  mp.players.local.setHairColor(parseInt(customization.Hair.Color), parseInt(customization.Hair.Highlightcolor));
  mp.players.local.setEyeColor(parseInt(customization.EyeColor));

  for (let i = 0; i < 11; i++) {
    //mp.console.logInfo(`Appearances: ${JSON.stringify(customization.Appearances)}`);
    if (customization.Appearances[i].Value === undefined) {
      mp.console.logInfo(`Appearances: index: ${i} value undefined`);
      continue;
    }

    mp.players.local.setHeadOverlay(
      i,
      parseInt(customization.Appearances[i].Value),
      parseInt(customization.Appearances[i].Opacity),
      colorForOverlayIdx(i, indexPed, customization),
      0,
    );
  }

  for (let i = 0; i < 20; i++) {
    if (customization.Features[i].Value === undefined) {
      mp.console.logInfo(`Features: index: ${i} value undefined`);
      continue;
    }
    mp.players.local.setFaceFeature(i, parseInt(customization.Features[i].Value));
  }
};

let colorForOverlayIdx = function (index, indexPed, customization) {
  let color;

  switch (index) {
    case 1:
      color = customization.BeardColor;
      break;

    case 2:
      color = customization.EyebrowColor;
      break;

    case 5:
      color = customization.BlushColor;
      break;

    case 8:
      color = customization.LipstickColor;
      break;

    case 10:
      color = customization.ChestHairColor;
      break;

    default:
      color = 0;
  }
  return parseInt(color);
};

let setCharacterClothes = function (indexPed) {
  if (authCharacters.length < indexPed) return;

  const character = authCharacters[indexPed];
  const customization = character.customization.custom;

  // раздеваем игрока полностью
  mp.utils.clearAllView(mp.players.local, parseInt(customization.Hair.Hair));

  const clothes = customization.Clothes;
  for (let i = 0; i <= 9; i++) {
    mp.players.local.setComponentVariation(parseInt(clothes[i].ComponentID), parseInt(clothes[i].Variation), parseInt(clothes[i].Texture), 0);
  }

  const accessory = customization.Accessory;
  for (let i = 0; i < accessory.length; i++) {
    mp.players.local.setPropIndex(parseInt(accessory[i].ComponentID), parseInt(accessory[i].Variation), parseInt(accessory[i].Texture), false);
  }
};

let setCharacterTattoos = function (indexPed) {
  if (authCharacters.length < indexPed) return;

  const character = authCharacters[indexPed];
  const customization = character.customization.custom;

  const tattoos = customization.Tattoos;
  mp.players.local.clearDecorations();

  for (let i = 0; i <= 4; i++) {
    if (tattoos[i].Collection === undefined) continue;

    mp.players.local.setDecoration(parseInt(tattoos[i].Collection), parseInt(tattoos[i].Hash));
  }
};


mp.events.add('CLIENT::selectcharacter:changeSlot', async (id) => {
  //mp.console.logInfo("changeSlot: id: "+id);
  if (id === pedNow) return;

  // if (authCharacters[id].isEmpty === true) {
  //   return;
  // }
  //
  // if (authCharacters[id].hasOwnProperty("banned")) {
  //   return;
  // }

  if (id === 0) {
      mp.utils.cam.moveTo(
          915.5094, 43.45662, 112.380956,
          914.8697, 40.671707, 112.099916,
          500,
      );
      pedNow = id;
  }
  else {
      if (id === 1) {
          mp.utils.cam.moveTo(
              914.292, 43.04261, 112.380956,
              913.8323, 40.765842, 112.099916,
              500,
          );

          if(!secondAtPod && authCharacters[id].isEmpty !== true && !authCharacters[id].hasOwnProperty("banned")) {
              peds[id].clearTasksImmediately();
              peds[id].taskGoStraightToCoord(PED_SECOND_POS[id].x, PED_SECOND_POS[id].y, PED_SECOND_POS[id].z, 1, 5000, PED_SECOND_ROT[id], 0.1);
              secondAtPod = true;
              await mp.game.waitAsync(2500);
              peds[id].taskPlayAnim(ANIMS[anims[id]][0], ANIMS[anims[id]][1], 1.0, 1.0, -1, 39, 1, true, true, true);
          }

          pedNow = id;
      }
      else if (id === 2) {
          mp.utils.cam.moveTo(
              916.25977, 42.57063, 112.380956,
              916.1949, 40.12284, 112.099916,
              500,
          );

          if(!thirdAtPod && authCharacters[id].isEmpty !== true && !authCharacters[id].hasOwnProperty("banned")){
              //peds[id].clearTasksImmediately();
              peds[id].taskGoStraightToCoord(PED_SECOND_POS[id].x, PED_SECOND_POS[id].y, PED_SECOND_POS[id].z, 1, 5000, PED_SECOND_ROT[id], 0.1);
              thirdAtPod = true;
              await mp.game.waitAsync(2500);
              peds[id].taskPlayAnim(ANIMS[anims[id]][0], ANIMS[anims[id]][1], 1.0, 1.0, -1, 39, 1, true, true, true);
          }

          pedNow = id;
      }
  }
});
