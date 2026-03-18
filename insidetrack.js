var screenTarget = null;
var bigScreenScaleform = null;
var bigScreenRender = null;
let isBigScreenLoaded = false;

var scaleform = null;
var startLoading = false;
var registerTarg = false;
var waitTarget = false;

global.insideTrackActive = false;
global.insideTrackSeat = false;

let tick = 0;
let cooldown = 60;
let clicked = -1;
let selectedGame = -1;

var startRace = false;

let screenPos = new mp.Vector3(1092.75, 264.56, -51.24);
let currentHorse = 1;
let currentBet = 100;
let currentGain = 200;
let finishReady = false;

let isBet = false;
let isSeating = false;
let winnerBet = -1;
let winner = -1;
let bet = 0;

let balance = 0;

let singleHorses = [1, 2, 3, 4, 5, 6];

var horses = null;
var videoWallTarget = null;

const NATIVES = {
   REGISTER_NAMED_RENDERTARGET : "0x57D9C12635E25CE3",
   LINK_NAMED_RENDERTARGET : "0xF6C09E276AEB3F2D",
   GET_NAMED_RENDERTARGET_RENDER_ID : "0x1A6478B61C6BDC3B",
   IS_NAMED_RENDERTARGET_REGISTERED : "0x78DCDC15C9F116B4",
   REQUEST_SCALEFORM_MOVIE : "0x11FE353CF9733E6F",
   BEGIN_SCALEFORM_MOVIE_METHOD : "0xF6E48914C7A8694E",
   SCALEFORM_MOVIE_METHOD_ADD_PARAM_INT : "0xC3D0841A0CC546A6",
   END_SCALEFORM_MOVIE_METHOD : "0xC6796A8FFA375E53",
   _SET_SCALEFORM_FIT_RENDERTARGET : "0xE6A9F00D4240B519",
   SET_TEXT_RENDER_ID : "0x5F15302936E07111",
   SET_SCRIPT_GFX_DRAW_ORDER : "0x61BB1D9B3A95D802",
   SET_SCRIPT_GFX_DRAW_BEHIND_PAUSEMENU : "0xC6372ECD45D73BCD",
   DRAW_SCALEFORM_MOVIE_FULLSCREEN : "0x0DF606929C105BE1",
   GET_DEFAULT_SCRIPT_RENDERTARGET_RENDER_ID : "0x52F0982D7FD156B6",
   RELEASE_NAMED_RENDERTARGET : "0xE9F6FFE837354DD4",
   SET_SCALEFORM_MOVIE_AS_NO_LONGER_NEEDED : "0x1D132D614DD86811",
   BEGIN_TEXT_COMMAND_SCALEFORM_STRING : "0x80338406F3475E55",
   END_TEXT_COMMAND_SCALEFORM_STRING : "0x362E2D3FE93A9959",
   SCALEFORM_MOVIE_METHOD_ADD_PARAM_PLAYER_NAME_STRING : "0xE83A3E3557A56640",
   SCALEFORM_MOVIE_METHOD_ADD_PARAM_FLOAT : "0xD69736AAE04DB51A",
   SCALEFORM_MOVIE_METHOD_ADD_PARAM_BOOL : "0xC58424BA936EB458",
   CALL_SCALEFORM_MOVIE_METHOD_WITH_NUMBER : "0xD0837058AE2E4BEE",
   END_SCALEFORM_MOVIE_METHOD_RETURN_VALUE : "0xC50AA39A577AF886",
   GET_SCALEFORM_MOVIE_METHOD_RETURN_VALUE_INT : "0x2DE7EFA66B906036",
   IS_SCALEFORM_MOVIE_METHOD_RETURN_VALUE_READY : "0x768FF8961BA904D6",
   GET_SCALEFORM_MOVIE_METHOD_RETURN_VALUE_BOOL : "0xD80A80346A45D761",
}

const HORSE_STYLES = [
   [15553363,5474797,9858144,4671302],
    [16724530,3684408,14807026,16777215],
    [13560920,15582764,16770746,7500402],
    [16558591,5090807,10446437,7493977],
    [5090807,16558591,3815994,9393493],
    [16269415,16767010,10329501,16777215],
    [2263807,16777215,9086907,3815994],
    [4879871,16715535,3815994,16777215],
    [16777215,2263807,16769737,15197642],
]

function RegisterTarget(name, model) {
   try {
      mp.game1.audio.stopSound(0);

      registerTarg = false;

      if(!mp.game1.ui.isNamedRendertargetRegistered(name)){
         mp.game1.ui.registerNamedRendertarget(name, false);
         mp.game1.ui.linkNamedRendertarget(mp.game1.joaat(model));

      }

      const timer = setInterval(() => {
         try {
            if (mp.game1.ui.isNamedRendertargetRegistered(name) && mp.game1.ui.isNamedRendertargetLinked(mp.game1.joaat(model))) {
               screenTarget = mp.game1.ui.getNamedRendertargetRenderId(name);
               registerTarg = true;
               clearInterval(timer);
            }
         } catch (e) {
            logger.error(e);
         }
      }, 500);
   }
   catch(e){
      logger.error(e);
   }
}

function RegisterTargetNull(name){
   try {
      if (!mp.game1.ui.isNamedRendertargetRegistered(name)) {
         mp.game1.ui.registerNamedRendertarget(name, false);
         mp.game1.ui.linkNamedRendertarget(903186242);

      }

      return mp.game1.ui.getNamedRendertargetRenderId(name);
   } catch (e) {
      logger.error(e);
   }
}


async function LoadBigScreen(){

   startLoading = true;

   mp.game.ui.releaseNamedRendertarget("casinoscreen_02");

   mp.game1.audio.requestScriptAudioBank('DLC_VINEWOOD/CASINO_GENERAL', true);
   mp.game1.streaming.requestAnimDict("anim_casino_a@amb@casino@games@insidetrack@ped_male@engaged@01a@base_big_screen");

   bigScreenScaleform = mp.game1.graphics.requestScaleformMovie('HORSE_RACING_WALL');

   RegisterTarget("casinoscreen_02", `vw_vwint01_betting_screen`);


   while(!registerTarg){
      await mp.game.waitAsync(0);
   }

   registerTarg = false;
   waitTarget = true;

      /*const timer = setInterval(async () => {
         try {
            if (mp.game1.graphics.hasScaleformMovieLoaded(bigScreenScaleform) && waitTarget) {
               mp.game1.graphics.pushScaleformMovieFunction(bigScreenScaleform, 'SHOW_SCREEN');

               mp.game1.graphics.pushScaleformMovieFunctionParameterInt(0);

               mp.game1.graphics.popScaleformMovieFunctionVoid();

               mp.game1.invoke(NATIVES._SET_SCALEFORM_FIT_RENDERTARGET, bigScreenScaleform, true);

               isBigScreenLoaded = true;

               startLoading = false;
               waitTarget = false;

               clearInterval(timer);
            }
         } catch (e) {
            logger.error(e);
         }
      }, 1000);*/

      while(!mp.game1.graphics.hasScaleformMovieLoaded(bigScreenScaleform))
        await mp.game.waitAsync(0);

      if (mp.game1.graphics.hasScaleformMovieLoaded(bigScreenScaleform) && waitTarget)
      {
        mp.game1.graphics.pushScaleformMovieFunction(bigScreenScaleform, 'SHOW_SCREEN');

        mp.game1.graphics.pushScaleformMovieFunctionParameterInt(0);

        mp.game1.graphics.popScaleformMovieFunctionVoid();

        mp.game1.invoke(NATIVES._SET_SCALEFORM_FIT_RENDERTARGET, bigScreenScaleform, true);

        isBigScreenLoaded = true;

        startLoading = false;
        waitTarget = false;
     }

}

mp.events.add('seatAtComp', (player, x, y, z) => {
   try {
     // global.insideTrackActive = true;
      global.insideTrackSeat = true;
      player.position = new mp.Vector3(x, y, z);
      player.taskPlayAnim("anim_casino_a@amb@casino@games@insidetrack@ped_male@engaged@01a@base_big_screen", "base_big_screen", 8.0, 1.0, -1, 69, 1.0, false, false, false);

      player.setHeading(45);

      if (player == mp.players.local) {
         isSeating = true;
      }
   } catch (e) {
      logger.error(e);
   }
});

mp.events.add('exitComp', function(player){
   try {
      if (player == mp.players.local) {
         isSeating = false;
      }
      //global.insideTrackActive = false;
      global.insideTrackSeat = false;
      canDoBets = false;
      player.taskPlayAnim("anim_casino_b@amb@casino@games@shared@player@", "sit_exit_left", 3.0, 1.0, 2500, 2, 0, false, false, false);
   } catch (e) {
      logger.error(e);
   }
});

mp.events.add('setMainEvent', (toggle) => {
   try {
      mp.game1.graphics.pushScaleformMovieFunction(scaleform, 'SET_MAIN_EVENT_IN_PROGRESS');
      mp.game1.graphics.pushScaleformMovieFunctionParameterBool(!toggle);
      mp.game1.graphics.popScaleformMovieFunctionVoid();
   } catch (e) {
      logger.error(e);
   }
});

mp.events.add('updateBalance', (count) => {
   balance = count;
});

mp.events.add('setHorses', (data) => {
   try {
      if (!isBigScreenLoaded) {
         const timer = setInterval(() => {
            try {
               if (isBigScreenLoaded) {
                  horses = JSON.parse(data);
                  AddHorsesMp(horses, bigScreenScaleform);
                  clearInterval(timer);
                  return;
               }
            } catch (e) {
               logger.error(e);
            }
         }, 100);
      }

      AddHorsesMp(JSON.parse(data), bigScreenScaleform);
   } catch (e) {
      logger.error(e);
   }
});


mp.events.add('updateCountdown', (count) => {
   try {
      if (startLoading || (!isBigScreenLoaded && !insideTrackActive))
         return;

      if (insideTrackActive) {
         mp.game1.graphics.pushScaleformMovieFunction(scaleform, 'SET_MAIN_EVENT_IN_PROGRESS');
         mp.game1.graphics.pushScaleformMovieFunctionParameterBool(false);
         mp.game1.graphics.popScaleformMovieFunctionVoid();

         mp.game1.graphics.pushScaleformMovieFunction(scaleform, 'SET_COUNTDOWN');

      } else {
         mp.game1.graphics.pushScaleformMovieFunction(bigScreenScaleform, 'SET_COUNTDOWN');
      }

      mp.game1.graphics.pushScaleformMovieFunctionParameterInt(count);
      mp.game1.graphics.popScaleformMovieFunctionVoid();
   } catch (e) {
      logger.error(e);
   }
});

mp.events.add('showHorse', (num) => {
   try {
      if (startLoading || (!isBigScreenLoaded && !insideTrackActive))
         return;

      mp.game1.graphics.pushScaleformMovieFunction(bigScreenScaleform, 'SHOW_SCREEN');

      mp.game1.graphics.pushScaleformMovieFunctionParameterInt(1);

      mp.game1.graphics.popScaleformMovieFunctionVoid();

      mp.game1.graphics.pushScaleformMovieFunction(bigScreenScaleform, 'SET_DETAIL_HORSE');
      mp.game1.graphics.pushScaleformMovieFunctionParameterInt(num);

      mp.game1.graphics.popScaleformMovieFunctionVoid();
   } catch (e) {
      logger.error(e);
   }
});

mp.events.add('showMain', () => {
   try {
      if (startLoading || (!isBigScreenLoaded && !insideTrackActive))
         return;

      mp.game1.graphics.pushScaleformMovieFunction(bigScreenScaleform, 'SHOW_SCREEN');

      mp.game1.graphics.pushScaleformMovieFunctionParameterInt(0);

      mp.game1.graphics.popScaleformMovieFunctionVoid();
   } catch (e) {
      logger.error(e);
   }

});

mp.events.add('setbet', (toggle) => {
   try {
      isBet = toggle;

      if (isBet) {
         HideInsideTrack();
      }
   } catch (e) {
      logger.error(e);
   }
 });

mp.events.add('addBet', (name, horse, bet) => {
  AddBet(name, horse, bet);
});

function AddBet(name, horse, bet){
   try {
      mp.game1.graphics.pushScaleformMovieFunction(bigScreenScaleform, 'ADD_PLAYER');

      mp.game1.graphics.pushScaleformMovieFunctionParameterString(`${name}`);
      mp.game1.graphics.pushScaleformMovieFunctionParameterInt(horse);
      mp.game1.graphics.pushScaleformMovieFunctionParameterInt(bet);

      mp.game1.graphics.popScaleformMovieFunctionVoid();
   } catch (e) {
      logger.error(e);
   }
}

mp.events.add('startRace', (horses) => {
   try {
      if (startLoading || (!isBigScreenLoaded && !insideTrackActive))
         return;
      StartRaceMain(horses, 0.0);

      mp.game1.graphics.pushScaleformMovieFunction(scaleform, 'SET_MAIN_EVENT_IN_PROGRESS');
      mp.game1.graphics.pushScaleformMovieFunctionParameterBool(true);
      mp.game1.graphics.popScaleformMovieFunctionVoid();
   } catch (e) {
      logger.error(e);
   }
});

mp.events.add('showRace', (horses, offset) => {
   try {
      if (startLoading || (!isBigScreenLoaded && !insideTrackActive))
         return;
      StartRaceMain(horses, parseFloat(offset));

      mp.game1.graphics.pushScaleformMovieFunction(scaleform, 'SET_MAIN_EVENT_IN_PROGRESS');
      mp.game1.graphics.pushScaleformMovieFunctionParameterBool(true);
      mp.game1.graphics.popScaleformMovieFunctionVoid();
   } catch (e) {
      logger.error(e);
   }
});

mp.events.add('startSingleRace', () => {
   StartRace(scaleform);
});

mp.events.add('addBetsInside', (bets) => {
   try {
      if (startLoading || (!isBigScreenLoaded && !insideTrackActive))
         return;

      mp.game1.graphics.pushScaleformMovieFunction(bigScreenScaleform, 'CLEAR_ALL_PLAYERS');
      mp.game1.graphics.popScaleformMovieFunctionVoid();

      var arr = JSON.parse(bets);
      arr.forEach(t => {
         AddBet(t.Name, parseInt(t.Horse), parseInt(t.BetSize));
      });
   } catch (e) {
      logger.error(e);
   }
});


mp.events.add('clearPlayers', () => {
   try {
      if (startLoading || (!isBigScreenLoaded && !insideTrackActive))
         return;
      mp.game1.graphics.pushScaleformMovieFunction(bigScreenScaleform, 'CLEAR_ALL_PLAYERS');
      mp.game1.graphics.popScaleformMovieFunctionVoid();

      mp.game1.audio.stopSound(0);
   } catch (e) {
      logger.error(e);
   }
});


mp.events.add('render', () => {


   try {
      if (videoWallTarget != null) {
         mp.game1.ui.setTextRenderId(videoWallTarget);

         mp.game1.invoke(NATIVES.SET_SCRIPT_GFX_DRAW_ORDER, 4);

         mp.game1.invoke(NATIVES.SET_SCRIPT_GFX_DRAW_BEHIND_PAUSEMENU, true);

         mp.game1.graphics.drawSprite('Prop_Screen_Vinewood', 'BG_Wall_Colour_4x4', 0.25, 0.5, 0.5, 1.0, 0.0, 255, 255, 255, 255);

         mp.game1.graphics.drawTvChannel(0.5, 0.5, 1.0, 1.0, 0.0, 255, 255, 255, 255);

         mp.game1.ui.setTextRenderId(1);
      }

      let playerPos = mp.players.local.position;

      let distance = mp.game1.gameplay.getDistanceBetweenCoords(playerPos.x, playerPos.y, playerPos.z, screenPos.x, screenPos.y, screenPos.z, false);

      if (distance < 30.0 && !insideTrackActive) {

         if (!isBigScreenLoaded && !startLoading) {
            LoadBigScreen();
         }

         if (!bigScreenRender) {
            bigScreenRender = true;
         }

         if (!waitTarget && isBigScreenLoaded) {

            mp.game1.ui.setTextRenderId(screenTarget);

            mp.game1.invoke(NATIVES.SET_SCRIPT_GFX_DRAW_ORDER, 4);

            mp.game1.invoke(NATIVES.SET_SCRIPT_GFX_DRAW_BEHIND_PAUSEMENU, true);


            // mp.game1.graphics.drawScaleformMovieFullscreen(bigScreenScaleform, 255, 255, 255, 255, true);

            mp.game1.graphics.drawScaleformMovie(bigScreenScaleform, 0.5, 0.5, 0.999, 0.999, 255, 255, 255, 255, 0);

            let nm = mp.game1.invoke(NATIVES.GET_DEFAULT_SCRIPT_RENDERTARGET_RENDER_ID);

            mp.game1.ui.setTextRenderId(1);
         }

      } else if (bigScreenRender && isBigScreenLoaded) {
         bigScreenRender = false;
         isBigScreenLoaded = false;

         mp.game.ui.releaseNamedRendertarget("casinoscreen_02");


         mp.game1.audio.stopSound(0);
      }

      UIRender();
   } catch (e) {
      logger.error(e);
   }

});

function HideInsideTrack(){
   try {
      mp.events.callRemote("hideInsideTrack");

      insideTrackActive = false;

      mp.game1.ui.displayHud(true);

      mp.events.call('showHUD', true);

   } catch (e) {
      logger.error(e);
   }

}

mp.keys.bind(0x20, false, function () { // backspace key
   try {
      if (insideTrackActive && isSeating) {

         HideInsideTrack();
      } else {
         if (isBigScreenLoaded && !insideTrackActive && isSeating) {
            OpenInsideTrack();
         }
      }
   } catch (e) {
      logger.error(e);
   }
});

 function UIRender() {
    try {
       if (insideTrackActive) {
          let x = mp.game1.controls.getDisabledControlNormal(2, 239);
          let y = mp.game1.controls.getDisabledControlNormal(2, 240);

          mp.game1.graphics.pushScaleformMovieFunction(scaleform, 'SET_MOUSE_INPUT');

          mp.game1.graphics.pushScaleformMovieFunctionParameterFloat(x);
          mp.game1.graphics.pushScaleformMovieFunctionParameterFloat(y);

          mp.game1.graphics.popScaleformMovieFunctionVoid();

          mp.game1.graphics.drawScaleformMovieFullscreen(scaleform, 255, 255, 255, 255, true);


          if (mp.game1.controls.isDisabledControlPressed(2, 237)) {
               //mp.console.logInfo(`IN METOD`);
             GetMouseClickedButton();
          }

          if (clicked != -1) {
             if (clicked == 15) {
                ShowRules();
             } else if (clicked == 12) {
                ShowMainScreen();
             } else if (clicked == 10) {
                if (selectedGame == 2) {
                   winnerBet = currentHorse;
                   bet = currentBet;

                   global.anyEvents.SendServer(() => mp.events.callRemote("trySingleBet", currentBet));

                } else {
                   global.anyEvents.SendServer(() => mp.events.callRemote("addbet", currentHorse, currentBet));

                   //AddBet("mahorazb", currentHorse, 100);
                }
             } else if (clicked == 13) {
                ShowMainScreen();
             } else if (clicked == 14) {
                ShowHorseSelectionMp();
                selectedGame = 1;
             } else if (clicked == 8) {
                if (balance < currentBet + 100) {
                   return;
                }

                if (currentBet + 100 > 10000) {
                   return;
                }
                currentBet = currentBet + 100;
                currentGain = currentBet * 2;
                UpdateBetValues(currentHorse, currentBet, balance, currentGain);
             } else if (clicked == 9) {
                if (99 > currentBet - 100) {
                   return;
                }
                currentBet = currentBet - 100;
                currentGain = currentBet * 2;
                UpdateBetValues(currentHorse, currentBet, balance, currentGain);
             } else if (clicked == 1) {
                selectedGame = 2;
                ShowHorseSelection();
             } else if (clicked != 12 && clicked != -1) {
                currentHorse = (clicked - 1);
                if (selectedGame == 2) {
                   ShowBetScreen();
                } else {
                   ShowBetScreenMp();
                }
             }
             clicked = -1;
          }

          if (startRace) {
             IsRaceFinished();

             if (finishReady) {
                finishReady = false;
                startRace = false;

                if (winner == winnerBet) {

                   global.anyEvents.SendServer(() => mp.events.callRemote("winnerBet", currentBet));
                }

                ShowResults(scaleform);
                mp.game.audio.stopSound(-1);
             }
          }
       }
    } catch (e) {
       logger.error(e);
    }
 }

function ShowRules(){
   try {
      mp.game1.graphics.pushScaleformMovieFunction(scaleform, 'SHOW_SCREEN');
      mp.game1.graphics.pushScaleformMovieFunctionParameterInt(9);
      mp.game1.graphics.popScaleformMovieFunctionVoid();
   } catch (e) {
      logger.error(e);
   }
}

function ShowMainScreen(){
   try {
      mp.game1.graphics.pushScaleformMovieFunction(scaleform, 'SHOW_SCREEN');
      mp.game1.graphics.pushScaleformMovieFunctionParameterInt(0);
      mp.game1.graphics.popScaleformMovieFunctionVoid();
   } catch (e) {
      logger.error(e);
   }
}

function ShowHorseSelection(){
   try {
      AddHorses(scaleform);

      mp.game1.graphics.pushScaleformMovieFunction(scaleform, 'SHOW_SCREEN');
      mp.game1.graphics.pushScaleformMovieFunctionParameterInt(1);
      mp.game1.graphics.popScaleformMovieFunctionVoid();
   } catch (e) {
      logger.error(e);
   }
}

function ShowHorseSelectionMp(){

   try {
      if (horses != null)
         AddHorsesMp(horses, scaleform);

      if (isBet) {
         UpdateBetValues(currentHorse, currentBet, balance, currentGain);

         mp.game1.graphics.pushScaleformMovieFunction(scaleform, 'SET_BETTING_ENABLED');
         mp.game1.graphics.pushScaleformMovieFunctionParameterBool(false);
         mp.game1.graphics.popScaleformMovieFunctionVoid();

         mp.game1.graphics.pushScaleformMovieFunction(scaleform, 'SHOW_SCREEN');
         mp.game1.graphics.pushScaleformMovieFunctionParameterInt(4);
         mp.game1.graphics.popScaleformMovieFunctionVoid();


      } else {
         UpdateBetValues(currentHorse, currentBet, balance, currentGain);

         mp.game1.graphics.pushScaleformMovieFunction(scaleform, 'SHOW_SCREEN');
         mp.game1.graphics.pushScaleformMovieFunctionParameterInt(2);
         mp.game1.graphics.popScaleformMovieFunctionVoid();

         mp.game1.graphics.pushScaleformMovieFunction(scaleform, 'SET_BETTING_ENABLED');
         mp.game1.graphics.pushScaleformMovieFunctionParameterBool(true);
         mp.game1.graphics.popScaleformMovieFunctionVoid();


      }
   } catch (e) {
      logger.error(e);
   }
}

function ShowResults(sc){
   try {
      mp.game1.audio.stopSound(0);

      mp.game1.graphics.pushScaleformMovieFunction(sc, 'SHOW_SCREEN');
      mp.game1.graphics.pushScaleformMovieFunctionParameterInt(7);
      mp.game1.graphics.popScaleformMovieFunctionVoid();
   } catch (e) {
      logger.error(e);
   }
}

function IsRaceFinished(){
   try {
      mp.game1.graphics.pushScaleformMovieFunction(scaleform, 'GET_RACE_IS_COMPLETE');

      var returnValue = mp.game1.invoke(NATIVES.END_SCALEFORM_MOVIE_METHOD_RETURN_VALUE);

      const timer = setInterval(() => {
         try {
            if (mp.game1.invoke(NATIVES.IS_SCALEFORM_MOVIE_METHOD_RETURN_VALUE_READY, returnValue)) {
               if (startRace) {
                  finishReady = mp.game1.invoke(NATIVES.GET_SCALEFORM_MOVIE_METHOD_RETURN_VALUE_BOOL, returnValue);
               } else {
                  finishReady = false;
                  mp.game.audio.stopSound(-1);
               }
               clearInterval(timer);
            }
         } catch (e) {
            logger.error(e);
         }
      }, 200);
   } catch (e) {
      logger.error(e);
   }
}



function UpdateBetValues(horse, bet, balance, gain){
   try {
      mp.game1.graphics.pushScaleformMovieFunction(scaleform, 'SET_BETTING_VALUES');
      mp.game1.graphics.pushScaleformMovieFunctionParameterInt(horse);
      mp.game1.graphics.pushScaleformMovieFunctionParameterInt(bet);
      mp.game1.graphics.pushScaleformMovieFunctionParameterInt(balance);
      mp.game1.graphics.pushScaleformMovieFunctionParameterInt(gain);
      mp.game1.graphics.popScaleformMovieFunctionVoid();
   } catch (e) {
      logger.error(e);
   }
}

function ShowBetScreen(){
   try {
      UpdateBetValues(currentHorse, currentBet, balance, currentGain);


      mp.game1.graphics.pushScaleformMovieFunction(scaleform, 'SHOW_SCREEN');
      mp.game1.graphics.pushScaleformMovieFunctionParameterInt(3);
      mp.game1.graphics.popScaleformMovieFunctionVoid();


      mp.game1.graphics.pushScaleformMovieFunction(scaleform, 'SET_BETTING_ENABLED');
      mp.game1.graphics.pushScaleformMovieFunctionParameterBool(true);
      mp.game1.graphics.popScaleformMovieFunctionVoid();
   } catch (e) {
      logger.error(e);
   }
}


function ShowBetScreenMp(){
   try {
      UpdateBetValues(currentHorse, currentBet, balance, currentGain);
      mp.game1.graphics.pushScaleformMovieFunction(scaleform, 'SHOW_SCREEN');
      mp.game1.graphics.pushScaleformMovieFunctionParameterInt(4);
      mp.game1.graphics.popScaleformMovieFunctionVoid();
   } catch (e) {
      logger.error(e);
   }

}
var clickeeed = false;

function GetMouseClickedButton(){

      if(clickeeed) return;
      //mp.console.logInfo(`IN FUNC METOD`);

      clickeeed = true;

      mp.game1.graphics.callScaleformMovieFunctionFloatParams(scaleform, 'SET_INPUT_EVENT', 237.0, -1082130432, -1082130432, -1082130432, -1082130432);

      mp.game1.graphics.pushScaleformMovieFunction(scaleform, 'GET_CURRENT_SELECTION');

      let returnValue = -1;

      returnValue = mp.game.invoke(NATIVES.END_SCALEFORM_MOVIE_METHOD_RETURN_VALUE);

      const timer = setInterval(() => {

            //mp.console.logInfo(`IN INTERVAL ${returnValue}`);
            if (mp.game.invoke(NATIVES.IS_SCALEFORM_MOVIE_METHOD_RETURN_VALUE_READY, returnValue)) {

               clicked = mp.game1.invoke(NATIVES.GET_SCALEFORM_MOVIE_METHOD_RETURN_VALUE_INT, returnValue);

               //mp.console.logInfo(`CLICK ${clicked}`);

               clearInterval(timer);
            }

      }, 10);

      setTimeout(() => {
         clickeeed = false;
      }, 500);
}

function SetScreenCooldown(sc, count){
   try {
      mp.game1.graphics.pushScaleformMovieFunction(sc, 'SET_COUNTDOWN');
      mp.game1.graphics.pushScaleformMovieFunctionParameterInt(count);
      mp.game1.graphics.popScaleformMovieFunctionVoid();
   } catch (e) {
      logger.error(e);
   }
}


async function OpenInsideTrack(){
  try {
   mp.events.callRemote("openInsideTrack");

   scaleform = mp.game1.graphics.requestScaleformMovie('HORSE_RACING_CONSOLE');

   /*const timer = setInterval(async () =>  {
      try {
         if (mp.game1.graphics.hasScaleformMovieLoaded(scaleform)) {
            insideTrackActive = true;

            mp.game1.ui.displayHud(false);

            mp.events.call('showHUD', false);

            mp.game.ui.releaseNamedRendertarget("casinoscreen_02");


            ShowMainScreen();

            AddHorsesMp(horses, scaleform);


            clearInterval(timer);
         }
      } catch (e) {
         logger.error(e);
      }
   }, 1000);*/

   while(!mp.game1.graphics.hasScaleformMovieLoaded(scaleform))
    await mp.game.waitAsync(0);

    insideTrackActive = true;

    mp.game1.ui.displayHud(false);

    mp.events.call('showHUD', false);

    mp.game.ui.releaseNamedRendertarget("casinoscreen_02");

    ShowMainScreen();

    AddHorsesMp(horses, scaleform);

  }
  catch(e){
     logger.error(e);
  }
}



function StartRace(sc){
   try {
      startRace = true;

      mp.game1.audio.playSoundFrontend(-1, 'race_loop', 'dlc_vw_casino_inside_track_betting_single_event_sounds', false);

      mp.game1.graphics.pushScaleformMovieFunction(sc, 'START_RACE');

      mp.game1.graphics.pushScaleformMovieFunctionParameterFloat(15000.0);

      mp.game1.graphics.pushScaleformMovieFunctionParameterInt(getRandomInt(255));

      singleHorses = shuffle(singleHorses);

      singleHorses.forEach((t) => {
            mp.game1.graphics.pushScaleformMovieFunctionParameterInt(t);
      });

      winner = singleHorses[0];

      mp.game1.graphics.pushScaleformMovieFunctionParameterFloat(0.0);
      mp.game1.graphics.pushScaleformMovieFunctionParameterBool(false);

      mp.game1.graphics.popScaleformMovieFunctionVoid();
   } catch (e) {
      logger.error(e);
   }
}

function StartRaceMain(horses, offset){
   try {
      var arr = JSON.parse(horses);

      mp.game1.audio.playSoundFrontend(-1, 'race_loop', 'dlc_vw_casino_inside_track_betting_single_event_sounds', false);

      mp.game1.graphics.pushScaleformMovieFunction(bigScreenScaleform, 'START_RACE');

      mp.game1.graphics.pushScaleformMovieFunctionParameterFloat(45000.0);

      mp.game1.graphics.pushScaleformMovieFunctionParameterInt(getRandomInt(255));

      mp.game1.graphics.pushScaleformMovieFunctionParameterInt(arr[0]);
      mp.game1.graphics.pushScaleformMovieFunctionParameterInt(arr[1]);
      mp.game1.graphics.pushScaleformMovieFunctionParameterInt(arr[2]);
      mp.game1.graphics.pushScaleformMovieFunctionParameterInt(arr[3]);
      mp.game1.graphics.pushScaleformMovieFunctionParameterInt(arr[4]);
      mp.game1.graphics.pushScaleformMovieFunctionParameterInt(arr[5]);

      mp.game1.graphics.pushScaleformMovieFunctionParameterFloat(offset);
      mp.game1.graphics.pushScaleformMovieFunctionParameterBool(true);
      mp.game1.graphics.popScaleformMovieFunctionVoid();
   } catch (e) {
      logger.error(e);
   }
}

function AddHorses(sf){
   try {
      for (let i = 1; i <= 6; i++) {
         let name = GetRandomHorseName();

         mp.game1.graphics.pushScaleformMovieFunction(sf, 'SET_HORSE');

         mp.game1.graphics.pushScaleformMovieFunctionParameterInt(i);

         mp.game1.invoke(NATIVES.BEGIN_TEXT_COMMAND_SCALEFORM_STRING, name);
         mp.game1.invoke(NATIVES.END_TEXT_COMMAND_SCALEFORM_STRING);

         mp.game1.invoke(NATIVES.SCALEFORM_MOVIE_METHOD_ADD_PARAM_PLAYER_NAME_STRING, `1/2`);

         mp.game1.graphics.pushScaleformMovieFunctionParameterInt(HORSE_STYLES[i][0]);
         mp.game1.graphics.pushScaleformMovieFunctionParameterInt(HORSE_STYLES[i][1]);
         mp.game1.graphics.pushScaleformMovieFunctionParameterInt(HORSE_STYLES[i][2]);
         mp.game1.graphics.pushScaleformMovieFunctionParameterInt(HORSE_STYLES[i][3]);

         mp.game1.graphics.popScaleformMovieFunctionVoid();
      }
   } catch (e) {
      logger.error(e);
   }
}

function AddHorsesMp(arr, sc){
   try {
      horses = arr;
      for (let i = 1; i <= 6; i++) {
         let name = arr[i - 1] < 10 ? `ITH_NAME_00${arr[i - 1]}` : `ITH_NAME_0${arr[i - 1]}`

         mp.game1.graphics.pushScaleformMovieFunction(sc, 'SET_HORSE');

         mp.game1.graphics.pushScaleformMovieFunctionParameterInt(i);

         mp.game1.invoke(NATIVES.BEGIN_TEXT_COMMAND_SCALEFORM_STRING, name);
         mp.game1.invoke(NATIVES.END_TEXT_COMMAND_SCALEFORM_STRING);

         mp.game1.invoke(NATIVES.SCALEFORM_MOVIE_METHOD_ADD_PARAM_PLAYER_NAME_STRING, `1/2`);

         mp.game1.graphics.pushScaleformMovieFunctionParameterInt(HORSE_STYLES[i][0]);
         mp.game1.graphics.pushScaleformMovieFunctionParameterInt(HORSE_STYLES[i][1]);
         mp.game1.graphics.pushScaleformMovieFunctionParameterInt(HORSE_STYLES[i][2]);
         mp.game1.graphics.pushScaleformMovieFunctionParameterInt(HORSE_STYLES[i][3]);

         mp.game1.graphics.popScaleformMovieFunctionVoid();
      }
   } catch (e) {
      logger.error(e);
   }
}

function GetRandomHorseName(){
   try {
      let rand = getRandomInt(99);

      let randomName = rand < 10 ? `ITH_NAME_00${rand}` : `ITH_NAME_0${rand}`;

      return randomName;
   } catch (e) {
      logger.error(e);
   }
}

function getRandomInt(max) {
   return Math.floor(Math.random() * max);
 }

function shuffle(array) {
   try {
      var currentIndex = array.length, temporaryValue, randomIndex;

      // While there remain elements to shuffle...
      while (0 !== currentIndex) {

         // Pick a remaining element...
         randomIndex = Math.floor(Math.random() * currentIndex);
         currentIndex -= 1;

         // And swap it with the current element.
         temporaryValue = array[currentIndex];
         array[currentIndex] = array[randomIndex];
         array[randomIndex] = temporaryValue;
      }

      return array;
   } catch (e) {
      logger.error(e);
   }
 }

 mp.events.add('enterCasinoWall', () => {

    try {
       if (!mp.game1.graphics.hasStreamedTextureDictLoaded("Prop_Screen_Vinewood")) {
          mp.game1.graphics.requestStreamedTextureDict("Prop_Screen_Vinewood", true);
          while (!mp.game1.graphics.hasStreamedTextureDictLoaded("Prop_Screen_Vinewood")) mp.game1.wait(0);
       }

       if (!mp.game1.ui.isNamedRendertargetRegistered("casinoscreen_01")) {
          mp.game1.ui.registerNamedRendertarget("casinoscreen_01", false);
          mp.game1.ui.linkNamedRendertarget(mp.game1.joaat(`vw_vwint01_video_overlay`));

       }

       while (!mp.game1.ui.isNamedRendertargetRegistered("casinoscreen_01") || !mp.game1.ui.isNamedRendertargetLinked(mp.game1.joaat(`vw_vwint01_video_overlay`))) {
          mp.game1.wait(0);
       }

       videoWallTarget = mp.game1.ui.getNamedRendertargetRenderId("casinoscreen_01");

       SetTvScreen();
    } catch (e) {
       logger.error(e);
    }

 });


 mp.events.add('exitCasinoWall', () => {

    try {
       mp.game1.invoke(NATIVES.RELEASE_NAMED_RENDERTARGET, "casinoscreen_01");

       videoWallTarget = null;
    } catch (e) {
       logger.error(e);
    }

});


 function SetTvScreen(){
    try {
       mp.game1.invoke("0xF7B38B8305F1FE8B", 0, 'CASINO_DIA_PL', true);

       mp.game1.graphics.setTvAudioFrontend(true);
       mp.game1.graphics.setTvVolume(-100);
       mp.game1.graphics.setTvChannel(0);
    } catch (e) {
       logger.error(e);
    }
 }
