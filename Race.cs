using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;
using NeptuneEvo.SDK;
using Newtonsoft.Json;
using System.Data;
using NeptuneEvo.GUI;
using System.Diagnostics;
using System.Timers;
using static NeptuneEvo.Core.VehicleManager;
using MySqlConnector;

namespace NeptuneEvo.Core
{
  class Race : Script
  {
    private static nLog Log = new nLog("Race");

    public static Dictionary<string, string> carRaceName = new Dictionary<string, string>() {
            // Standart - Asterope
            {"rmode63s", "Brabus 800 Mercedes-AMG E63 S6"},
            {"2020ss", "Chevrolet Camaro SS"},
            {"porsche2021", "Porsche 911 Turbo S"},
            {"m5f90", "BMW M5 F90"},
            {"rmodrs6", "Audi RS 6 Avant"},
            {"huracan", "Lamborghini Huracan"},
            {"kiastinger", "KIA Stinger"},
            {"optima", "Kia Optima SXL Turbo"},
            {"fpacel", "Jaguar F-PACE"},
            {"ae86", "Toyota Corolla AE86"},
            {"w210", "Mercedes-Benz E-Class W210"},
            {"benzc32", "Mercedes-Benz C32 AMG"},
            {"s600", "Mercedes-Benz S600 W140"},
            {"bmwe38", "BMW E38"},
            {"mark2", "Toyota Mark II"}
    };

    private static List<string> carRace = new List<string>() { "rmode63s", "2020ss", "porsche2021" };
    private static List<string> carPrizesGold = new List<string>() { "m5f90", "rmodrs6", "huracan" };
    private static List<string> carPrizesSilver = new List<string>() { "rmode63s", "2020ss", "porsche2021" };
    private static List<string> carPrizesBronze = new List<string>() { "kiastinger", "optima", "fpacel" };
    private static List<string> carPrizesForAll = new List<string>() { "s600", "bmwe38", "mark2" };

    public static int racePosition = 0;

    public static int prizeTimer = 10800;

    public class ObjectCars
    {
      [JsonProperty("name")]
      public string name { get; set; }
      [JsonProperty("model")]
      public string model { get; set; }
      [JsonProperty("number")]
      public string number { get; set; }
      [JsonProperty("hash")]
      public string hash { get; set; }
      [JsonProperty("selected")]
      public bool selected { get; set; }
    };

    private static Vector3 enterRace = new Vector3(-2293.2117, 4251.5957, 41.533737);
    private static Vector3 respawnRacePosition = new Vector3(-2293.2117, 4251.5957, 42.61832);

    private static List<Vector3> raceSpawnCar = new List<Vector3>()
    {
        new Vector3(-2291.043, 4261.37, 43.147144),
        new Vector3(-2288.6382, 4263.793, 43.463516),
        new Vector3(-2286.7327, 4266.791, 43.791042),
        new Vector3(-2284.912, 4269.334, 44.083035),
        new Vector3(-2283.0493, 4271.6567, 44.336174),
        new Vector3(-2279.2783, 4273.184, 44.64642),
        new Vector3(-2276.7227, 4276.2295, 44.94),
        new Vector3(-2274.7476, 4279.024, 45.235237),
        new Vector3(-2272.4717, 4282.172, 45.644054),
        new Vector3(-2271.1033, 4285.4756, 45.89421),
    };

    private static List<Vector3> raceSpawnCarRot = new List<Vector3>()
    {
        new Vector3(2.2783942, 5.2080703, -129.67351),
        new Vector3(1.3518718, 5.691748, -130.76971),
        new Vector3(0.93498015, 5.822025, -129.89253),
        new Vector3(0.5894772, 5.32079, -130.5073),
        new Vector3(0.6649919, 4.869418, -131.86966),
        new Vector3(0.9773001, 5.3911138, -142.32529),
        new Vector3(4.9489865, 5.613195, -136.24318),
        new Vector3(5.592992, 7.7035975, -140.04073),
        new Vector3(6.154824, 7.5261655, -138.10106),
        new Vector3(6.72615, 7.610861, -134.96512),
    };

    private static Vector3 raceStart = new Vector3(-2284.75, 4228.8086, 41.31676);
    private static Vector3 raceFinish = new Vector3(2590.4397, 329.53287, 107.26846); // 2590.4397, 329.53287, 108.388466

    private static Vector3 raceLSPoint = new Vector3(-2168.0999, -347.5866, 13.209317);

    public static Vector3 racePrizeColshape = new Vector3(-755.3446, -289.97913, 36.01531);
    private static List<Vector3> racePrizeSpawnCarPosition = new List<Vector3>()
    {
        new Vector3(-741.8491, -288.38513, 37.314724),
        new Vector3(-733.0585, -270.58466, 37.023163),
        new Vector3(-737.65753, -263.1359, 37.02535),
        new Vector3(-741.22266, -271.04974, 37.025475),
        new Vector3(-745.02264, -262.83185, 37.048134),
        new Vector3(-718.4672, -298.11224, 37.028603),
        new Vector3(-706.04474, -302.54913, 36.830563),
        new Vector3(-693.4171, -306.43777, 36.37513),
        new Vector3(-705.5673, -296.51895, 36.896038),
        new Vector3(-696.36993, -298.4295, 36.573387),
    };
    private static List<Vector3> racePrizeSpawnCarRotation = new List<Vector3>()
    {
        new Vector3(-0.1196927, 0.016880004, -68.322),
        new Vector3(-0.10085642, 0.03254995, 31.28726),
        new Vector3(-0.20121935, 0.023981467, 32.998737),
        new Vector3(-0.09672431, 0.008718585, 23.949236),
        new Vector3(-1.1343293, 1.7739534, 19.545574),
        new Vector3(-0.055696998, -0.15965934, -111.2702),
        new Vector3(-1.691897, -0.20319232, -108.32199),
        new Vector3(-1.8625692, 1.2012837, -67.29654),
        new Vector3(-1.9028894, 4.1564245, -106.44724),
        new Vector3(-2.0668354, 4.043164, -105.49616),
    };

    //private static ColShape raceShape;
    private static Marker raceMarker;
    private static TextLabel raceTextLabel;
    private static Blip raceBlip;
    private static ColShape startRaceShape;

    private static uint raceDimension = 1000;

    //Prize NPC
    private static ColShape racePrizeNPCShape;

    private static Marker racePrizeMarker;
    private static Blip racePrizeBlip;

    //private static Vehicle lastRaceVehicle = null;

    //private static System.Timers.Timer aTimer;
    //private static DateTime aTimerElapsed;
    //private static string aTimerFormatedTime;
    //private static Player Racer;
    //private static TimeSpan aTimerStartTime { get; set; } = TimeSpan.Zero;

    //private static Player DisconnectedPlayer;

    private static TimeSpan gold = new TimeSpan(0, 2, 42);
    private static TimeSpan silver = new TimeSpan(0, 2, 58);
    private static TimeSpan bronze = new TimeSpan(0, 3, 20);

    #region Выставляем метки и тригеры входа в колшейпы
    [ServerEvent(Event.ResourceStart)]
    public void onResourceStart()
    {
      try
      {
        racePrizeNPCShape = NAPI.ColShape.CreateCylinderColShape(racePrizeColshape, 1, 2, 0);
        racePrizeNPCShape.OnEntityEnterColShape += onPlayerEnterRacePrizeNPC;
        racePrizeNPCShape.OnEntityExitColShape += onPlayerExitRacePrizeNPC;

        racePrizeMarker = NAPI.Marker.CreateMarker(1, racePrizeColshape - new Vector3(0, 0, 0.7), new Vector3(), new Vector3(), 1, new Color(0, 255, 255));

        racePrizeBlip = NAPI.Blip.CreateBlip(racePrizeColshape, 0);
        racePrizeBlip.ShortRange = true;
        racePrizeBlip.Name = Main.StringToU16("Призовой автосалон");
        racePrizeBlip.Sprite = 792;
        racePrizeBlip.Color = 11;

        raceMarker = NAPI.Marker.CreateMarker(1, enterRace - new Vector3(0, 0, 0.7), new Vector3(), new Vector3(), 1, new Color(0, 255, 255));

        raceTextLabel = NAPI.TextLabel.CreateTextLabel(Main.StringToU16("~g~Гонка на время"), new Vector3(enterRace.X, enterRace.Y, enterRace.Z + 1), 5f, 0.3f, 0, new Color(255, 255, 255));
        raceBlip = NAPI.Blip.CreateBlip(enterRace, 0);
        raceBlip.ShortRange = true;
        raceBlip.Name = Main.StringToU16("Гонка на время");
        raceBlip.Sprite = 545;
        raceBlip.Color = 29;

        //Вход в гонку
        //raceShape = NAPI.ColShape.CreateCylinderColShape(enterRace, 1, 2, 0);
        //raceShape.OnEntityEnterColShape += onPlayerEnterRaceNPC;
        //raceShape.OnEntityExitColShape += onPlayerExitRaceNPC;

        //Создается колшейп куда подьехать на старт
        startRaceShape = NAPI.ColShape.CreateCylinderColShape(raceStart, 4, 5, raceDimension);
        startRaceShape.OnEntityEnterColShape += onPlayerStartRace;

        #region Quest

        //Main.Colshapes.Add(raceShape, "Race");

        #endregion
      }
      catch (Exception e) { Log.Write("ResourceStart: " + e.StackTrace, nLog.Type.Error); }
    }

    private static void onPlayerEnterRacePrizeNPC(ColShape shape, Player player)
    {
      try
      {
        player.SetMyData("INTERACTIONCHECK", 1008);
        //player.SetMyData("QUEST_COLSHAPE_INTERACTION", 1008);
      }
      catch (Exception e) { Log.Write("onPlayerEnterRacePrizeNPC: " + e.ToString(), nLog.Type.Error); }
    }

    private static void onPlayerExitRacePrizeNPC(ColShape shape, Player player)
    {
      player.SetMyData("INTERACTIONCHECK", 0);
    }

    public static void removeRaceMarker(Player player)
    {
      //NAPI.ColShape.DeleteColShape(raceShape);
      //NAPI.Entity.DeleteEntity(raceMarker);
      //NAPI.Entity.DeleteEntity(raceTextLabel);
      //NAPI.Entity.DeleteEntity(raceBlip);
    }

    //private static void onPlayerEnterRaceNPC(ColShape shape, Player player)
    //{
    //  try
    //  {
    //    player.SetMyData("COLSHAPE_TYPE", "Race");
    //    player.SetMyData("QUEST_COLSHAPE_INTERACTION", 1005);

    //    if (player.HasMyData("RACE_FAILED") && player.GetMyData<bool>("RACE_FAILED") == true)
    //    {
    //      Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"К сожалению, вы проиграли гонку. Вы не можете снова участвовать в ней!", 3000);
    //      return;
    //    }

    //    if (!QuestSystem.isActiveChapter(player, "race"))
    //    {
    //      Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы не можете участвовать в гонке. Попробуйте подойти к Митчелу и попросить его вам помочь.", 3000);
    //      return;
    //    }

    //    player.SetMyData("INTERACTIONCHECK", 1005);

    //  }
    //  catch (Exception e) { Log.Write("onPlayerEnterRaceNPC: " + e.ToString(), nLog.Type.Error); }
    //}
    //private static void onPlayerExitRaceNPC(ColShape shape, Player player)
    //{
    //  player.SetMyData("INTERACTIONCHECK", 0);
    //  player.ResetMyData("COLSHAPE_TYPE");
    //}
    #endregion

    #region Открываем меню гонки по нажатию E
    public static void openRaceMenu(Player player)
    {
      List<ObjectCars> carsList = new List<ObjectCars>();
      foreach (var car in carRace)
      {
        VehicleHash carHash = (VehicleHash)NAPI.Util.GetHashKey(car);
        string vehicleRealName = carRaceName[car];

        var carName = $"{vehicleRealName}";
        var carModel = car;
        var carNumber = "RACE";

        string carData = "{" +
            "'name':'" + carName + "', " +
            "'number':'" + carNumber + "', " +
            "'model':'" + carModel + "', " +
            "'selected':'" + false + "', " +
            "'hash':'" + carHash + "" +
        "'}";

        ObjectCars convertedcarData = JsonConvert.DeserializeObject<ObjectCars>(carData);

        carsList.Add(convertedcarData);
      }

      Trigger.ClientEvent(player, "raceOpenMenu", JsonConvert.SerializeObject(carsList));
    }
    #endregion

    #region Выбрали машину и начинаем спавн машины

    [RemoteEvent("raceMenuSpawnCar")]
    public static void RemoteEvent_raceMenuStart(Player player, string name)
    {
      try
      {
        Trigger.ClientEvent(player, "deleteCheckpoint", $"RACECOLSHAPEF {Main.Players[player].UUID}");
        Trigger.ClientEvent(player, "deleteCheckpoint", $"RACECOLSHAPEFINISH {Main.Players[player].UUID}");
        Trigger.ClientEvent(player, "blip_remove", $"Финиш {Main.Players[player].UUID}");
        Trigger.ClientEvent(player, "blip_remove", $"Старт гонки {Main.Players[player].UUID}");

        if (player.HasMyData("RACEVEH"))
        {
          var raceveh = player.GetMyData<Vehicle>("RACEVEH");
          NAPI.Task.Run(() =>
          {
            NAPI.Entity.DeleteEntity(raceveh);
          });
        }

        spawnInSelectedCar(player, name);
      }
      catch (Exception e) { Log.Write("onPlayerEnterRaceNPC: " + e.ToString(), nLog.Type.Error); }
    }

    #endregion

    #region Заспавнили машину и поставили метку на старт
    public static void spawnInSelectedCar(Player player, string name)
    {
        try
        {
            NAPI.Task.Run(() => {
                Trigger.ClientEvent(player, "screenFadeOut", 500);
                NAPI.Entity.SetEntityDimension(player, raceDimension);
                VehicleHash vehicleHash = (VehicleHash)NAPI.Util.GetHashKey(name);

                var RandomPos = VehicleManager.GetFreePosition(raceSpawnCar);
                if (RandomPos == -1) RandomPos = 0;

                var vehicle = NAPI.Vehicle.CreateVehicle(vehicleHash, raceSpawnCar[RandomPos], raceSpawnCarRot[RandomPos], 30, 30);
                vehicle.NumberPlate = "RACE";
                vehicle.Locked = false;
                vehicle.EngineStatus = true;
                vehicle.Dimension = raceDimension;

                player.SetIntoVehicle(vehicle, 0);

                player.SetMyData("RACEVEH", vehicle);
                player.SetMyData("IS_DRIVING", true);
                player.SetMyData("IN_RACE_CAR", true);

                vehicle.SetData("ACCESS", "RACE");
                vehicle.SetData("DRIVER", player);


                Main.Players[player].RaceIsActive = true;
                Main.Players[player].RaceIsEnd = false;

                player.SetMyData<ColShape>("RACESTARTCOLSHAPE", startRaceShape);


                //Создается чекпоинт куда подьехать на старт
                Trigger.ClientEvent(player, "createCheckpoint", $"RACECOLSHAPE {Main.Players[player].UUID}", 1, raceStart - new Vector3(0, 0, 2), 4, raceDimension, 255, 0, 0, raceLSPoint);

                //Trigger.ClientEvent(player, "createWaypoint", raceStart.X, raceStart.Y);

                Trigger.ClientEvent(player, "blip_create_ext", $"Старт гонки {Main.Players[player].UUID}", raceStart, 1, 1, 0, false, $"Старт гонки {Main.Players[player].UUID}", raceDimension);
                Trigger.ClientEvent(player, "blip_setRoute", $"Старт гонки {Main.Players[player].UUID}", true);

                VehicleStreaming.SetEngineState(vehicle, true);
                player.SetIntoVehicle(vehicle, 0);
                Trigger.ClientEvent(player, "screenFadeIn", 1000);

            });
        }
      catch (Exception e) { Log.Write("spawnInSelectedCar: " + e.ToString(), nLog.Type.Error); }
    }
    #endregion

    #region Приехал на старт
    public static void onPlayerStartRace(ColShape shape, Player player)
    {
      //if (!player.HasMyData("RACESTARTCOLSHAPE")) return;

      //NAPI.ColShape.DeleteColShape(player.GetMyData<ColShape>("RACESTARTCOLSHAPE"));

      Trigger.ClientEvent(player, "freezeVeh", true);

      var timer = new List<string>();

      timer.Add("3");
      timer.Add("2");
      timer.Add("1");
      timer.Add("START");
      Trigger.ClientEvent(player, "CLIENT::timer:show", JsonConvert.SerializeObject(timer));
      Log.Debug("timer: " + JsonConvert.SerializeObject(timer));

      start(player);
    }
    #endregion

    [RemoteEvent("SERVER::timer:hide")]
    public static void RemoteEvent_TimerHide(Player player)
    {
      try
      {
        Trigger.ClientEvent(player, "freezeVeh", false);
        Trigger.ClientEvent(player, "CLIENT::race:startTimer");
      }
      catch (Exception e)
      {
        Log.Write("race start:\n" + e.StackTrace, nLog.Type.Error);
      }
    }

    #region Отсчет прошел, можно ехать
    public static void start(Player player)
    {
      try
      {
        if (!player.IsInVehicle || player.VehicleSeat != 0) return;
        if (!player.Vehicle.HasData("ACCESS") || player.Vehicle.GetData<string>("ACCESS") != "RACE") return;
        if (!player.HasMyData("IS_DRIVING")) return;
        if (player.Vehicle != player.GetMyData<Vehicle>("RACEVEH")) return;
        //Log.Write($"Start race 4 {Main.Players[player].FirstName} {Main.Players[player].LastName}", nLog.Type.Error);

        player.SetMyData("BEFORE_RACE_HP", player.Health);

        //Racer = player;

        //Создается колшейп где финиш
        var endRaceShape = NAPI.ColShape.CreateCylinderColShape(raceFinish, 12, 20, raceDimension);
        endRaceShape.OnEntityEnterColShape += onPlayerFinishRace;
        endRaceShape.SetData("RACEPLAYERONFINISH", true);
        //Log.Write($"Create Finish Colshape 5 {Main.Players[player].FirstName} {Main.Players[player].LastName}", nLog.Type.Error);

        player.SetMyData<ColShape>("RACEFINISHCOLSHAPE", endRaceShape);

        //Создаем промежуточный чекпоинт raceLSPoint
        Trigger.ClientEvent(player, "createCheckpoint", $"RACECOLSHAPEHALF {Main.Players[player].UUID}", 1, raceLSPoint - new Vector3(0, 0, 2), 20, raceDimension, 255, 0, 0, raceFinish);
        var halfRaceShape = NAPI.ColShape.CreateCylinderColShape(raceLSPoint, 20, 20, raceDimension);
        halfRaceShape.OnEntityEnterColShape += onPlayerHalfRace;
        halfRaceShape.SetData("RACEPLAYERONHALF", true);

        player.SetMyData<ColShape>("RACEHALFCOLSHAPE", halfRaceShape);
        player.SetMyData<bool>("HASRACEHALFCOLSHAPE", true);

        //if (player.HasMyData("RACESTARTCOLSHAPE"))
        //{
        //  NAPI.ColShape.DeleteColShape(player.GetMyData<ColShape>("RACESTARTCOLSHAPE"));
        //}

        //Удаляем блип и чекпоинт старта
        Trigger.ClientEvent(player, "blip_remove", $"Старт гонки {Main.Players[player].UUID}");
        Trigger.ClientEvent(player, "deleteCheckpoint", $"RACECOLSHAPE {Main.Players[player].UUID}");

        //Создаем блип для промежуточного чекпоинта
        Trigger.ClientEvent(player, "blip_create_ext", $"Половина гонки {Main.Players[player].UUID}", raceLSPoint, 1, 1, 0, false, $"Половина гонки {Main.Players[player].UUID}", raceDimension);
        Trigger.ClientEvent(player, "blip_setRoute", $"Половина гонки {Main.Players[player].UUID}", true);

        //Создается чекпоинт где финиш
        Trigger.ClientEvent(player, "createCheckpoint", $"RACECOLSHAPEF {Main.Players[player].UUID}", 1, raceFinish - new Vector3(0, 0, 5), 12, raceDimension, 255, 0, 0);
        Trigger.ClientEvent(player, "createCheckpoint", $"RACECOLSHAPEFINISH {Main.Players[player].UUID}", 4, raceFinish + new Vector3(0, 0, 7), 8, raceDimension, 255, 0, 0);

        NAPI.Task.Run(() => {
          Trigger.ClientEvent(player, "blip_remove", $"Половина гонки {Main.Players[player].UUID}");
          //Создаем через 30 блип финиша, чтобы был виден на карте
          Trigger.ClientEvent(player, "blip_create_ext", $"Финиш {Main.Players[player].UUID}", raceFinish, 1, 1, 0, false, $"Финиш гонки {Main.Players[player].UUID}", raceDimension);
          Trigger.ClientEvent(player, "blip_setRoute", $"Финиш {Main.Players[player].UUID}", true);
        }, 30000);

        //Log.Write($"Create Finish route 6 {Main.Players[player].FirstName} {Main.Players[player].LastName}", nLog.Type.Error);
      }
      catch (Exception e)
      {
        Log.Write("race start:\n" + e.StackTrace, nLog.Type.Error);
      }
    }

    public static void changeRouteBlip(Player player)
    {

    }

    public static void onPlayerHalfRace(ColShape shape, Player player)
    {
      if (!player.HasMyData("RACEHALFCOLSHAPE")) return;

      NAPI.ColShape.DeleteColShape(player.GetMyData<ColShape>("RACEHALFCOLSHAPE"));

      player.SetMyData<bool>("HASRACEHALFCOLSHAPE", false);

      //Удаляем чекпоинт и блип половины гонки
      Trigger.ClientEvent(player, "deleteCheckpoint", $"RACECOLSHAPEHALF {Main.Players[player].UUID}");

    }
    #endregion

    #region ФИНИШ!
    public static void onPlayerFinishRace(ColShape shape, Player player)
    {
      if (!player.HasMyData("RACEFINISHCOLSHAPE")) return;

      NAPI.ColShape.DeleteColShape(player.GetMyData<ColShape>("RACEFINISHCOLSHAPE"));

      Trigger.ClientEvent(player, "CLIENT::race:stopTimer");
    }

    [RemoteEvent("SERVER::race:end")]
    public static void RemoteEvent_finish(Player player, int elapsedTime)
    {
      try
      {
        //Log.Debug("ElapsedTime: " + elapsedTime); // 21000

        if (!player.IsInVehicle || player.VehicleSeat != 0) return;
        if (!player.Vehicle.HasData("ACCESS") || player.Vehicle.GetData<string>("ACCESS") != "RACE") return;
        if (!player.HasMyData("IS_DRIVING")) return;
        if (player.Vehicle != player.GetMyData<Vehicle>("RACEVEH")) return;

        //Log.Write("FINISH RACE 7", nLog.Type.Error);

        Main.Players[player].RaceIsActive = false;
        Main.Players[player].RaceIsEnd = true;

        removeRaceMarker(player);
        giveRaceResult(player, elapsedTime);

        //Log.Write($"FINISH RACE giveRaceResult 8 {Main.Players[player].FirstName} {Main.Players[player].LastName}", nLog.Type.Error);

        if (player.HasMyData("RACE_CAR_EXIT_TIMER"))
        {
          Timers.Stop(player.GetMyData<string>("RACE_CAR_EXIT_TIMER"));
          player.ResetMyData("RACE_CAR_EXIT_TIMER");
        }

        //player.ResetMyData("RACE_TIMEREXITCAR");
        //Main.Players[player].aTimer = null;
        //Main.Players[player].aTimerStartTime = TimeSpan.Zero;
        ////Log.Write($"FINISH RACE aTimerStop 9 {Main.Players[player].FirstName} {Main.Players[player].LastName}", nLog.Type.Error);

        NAPI.Task.Run(() =>
        {
          try
          {
            NAPI.Entity.DeleteEntity(player.GetMyData<Vehicle>("RACEVEH"));
            //Log.Write("FINISH RACE Remove RACEVEH 10", nLog.Type.Error);
          }
          catch (Exception e){ Log.Write(e.StackTrace, nLog.Type.Error); }
        });

        NAPI.Entity.SetEntityDimension(player, 0);

        player.ResetMyData("IS_DRIVING");

        #region quest chapter iteration

        QuestSystem.UpdatePlayerQuestIteration(player, false);

        #endregion


        if (Main.Players[player].RacePosition > 0)
        {
            player.SetMyData("PQ_WINRACE", true);

            if(Main.Players[player].RacePosition == 1) client.Core.Achievements.AddAchievementScore(player, client.Core.AchievementID.RaceWinner, 1);

            //setWaypoinToPrizeColshape(player);
        }
        else
        {
          //Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы успешно завершили гонку", 3000);
        }

        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы успешно завершили гонку. Дейв ожидает вас!", 10000);
        Trigger.ClientEvent(player, "createWaypoint", 2577.7432, 305.9174);

        //Удаляем чекпоинт финиша
        Trigger.ClientEvent(player, "deleteCheckpoint", $"RACECOLSHAPE {Main.Players[player].UUID}");
        Trigger.ClientEvent(player, "deleteCheckpoint", $"RACECOLSHAPEFINISH {Main.Players[player].UUID}");

        Trigger.ClientEvent(player, "blip_remove", $"Финиш {Main.Players[player].UUID}");

        //Log.Write("FINISH RACE ALL 11", nLog.Type.Error);

        return;
      }
      catch (Exception e)
      {
        Log.Write("ENTERDRIVE:\n" + e.ToString(), nLog.Type.Error);
      }
    }

    public static void setWaypoinToPrizeColshape(Player player)
    {
        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы можете получить призовое Т/C в автосалоне по установленной метке GPS", 10000);
        Trigger.ClientEvent(player, "createWaypoint", racePrizeColshape.X, racePrizeColshape.Y);
    }

    [ServerEvent(Event.PlayerEnterVehicle)]
    public void Event_OnPlayerEnterVehicle(Player player, Vehicle vehicle, sbyte seatid)
    {
        try
        {
            if (!Main.Players.ContainsKey(player)) return;

            if (player.HasMyData("RACE_DESTROYED_CAR")) return;

            if (NAPI.Data.GetEntityData(vehicle, "ACCESS") == "RACE" && player.GetMyData<Vehicle>("RACEVEH") == vehicle)
            {
                player.SetMyData("IN_RACE_CAR", true);
            }
        }
        catch(Exception ex)
        {
             Log.Write("Event_OnPlayerEnterVehicle" + ex.StackTrace);
        }
     }

    [ServerEvent(Event.PlayerExitVehicle)]
    public void onPlayerExitVehicleHandler(Player player, Vehicle vehicle)
    {
      try
      {
        if (!Main.Players.ContainsKey(player)) return;

        if (player.HasMyData("RACE_DESTROYED_CAR")) return;

        if (NAPI.Data.GetEntityData(vehicle, "ACCESS") == "RACE" && player.GetMyData<Vehicle>("RACEVEH") == vehicle)
        {
            if (!player.HasMyData("IS_DRIVING")) return;

          Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, $"Если Вы не сядете в транспорт через 5 минут, то гонка завершится", 3000);

          player.SetMyData("IN_RACE_CAR", false);
          if (player.HasMyData("RACE_CAR_EXIT_TIMER"))
          {
            Timers.Stop(player.GetMyData<string>("RACE_CAR_EXIT_TIMER"));
          }

          player.SetMyData("RACE_EXIT_TIMER_COUNT", 0);
          player.SetMyData("RACE_CAR_EXIT_TIMER", Timers.StartTask(1000, () => timer_playerExitRaceVehicle(player, vehicle)));
        }
      }
      catch (Exception e) { Log.Write("PlayerExit: " + e.StackTrace, nLog.Type.Error); }
    }

    private static void timer_playerExitRaceVehicle(Player player, Vehicle vehicle)
    {
      NAPI.Task.Run(() => {
        try
        {
          if (!player.HasMyData("RACE_CAR_EXIT_TIMER")) return;
          if (player.GetMyData<bool>("IN_RACE_CAR"))
          {
            Timers.Stop(player.GetMyData<string>("RACE_CAR_EXIT_TIMER"));
            player.ResetMyData("RACE_CAR_EXIT_TIMER");
            return;
          }
          if (player.GetMyData<int>("RACE_EXIT_TIMER_COUNT") > 300)
          {
            Timers.Stop(player.GetMyData<string>("RACE_CAR_EXIT_TIMER"));
            player.ResetMyData("RACE_CAR_EXIT_TIMER");

            playerFailedRace(player);

            return;
          }

          player.SetMyData("RACE_EXIT_TIMER_COUNT", player.GetMyData<int>("RACE_EXIT_TIMER_COUNT") + 1);
        }
        catch (Exception e) { Log.Write("race_exitVehicleTimer: " + e.StackTrace); }
      });
    }

    public static void playerFailedRace(Player player)
    {
      try
      {
        if (!Main.Players[player].RaceIsActive || Main.Players[player].RaceIsEnd) return;

        Trigger.ClientEvent(player, "screenFadeOut", 1000);

        Main.Players[player].RaceIsActive = false;
        Main.Players[player].RaceIsEnd = true;
        Main.Players[player].RacePosition = 4;

        removeRaceMarker(player);

        NAPI.Task.Run(() => {
          try
          {
            NAPI.Entity.DeleteEntity(player.GetMyData<Vehicle>("RACEVEH"));
            //Log.Write("FINISH RACE Remove RACEVEH 10", nLog.Type.Error);
          }
          catch (Exception e){ Log.Write(e.StackTrace, nLog.Type.Error); }
        });

        //NAPI.Entity.SetEntityDimension(player, 0);

        player.ResetMyData("IS_DRIVING");

        #region quest chapter iteration

        QuestSystem.UpdatePlayerQuestIteration(player, false);

        #endregion

        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"К сожалению, вы проиграли гонку", 3000);

        if (player.HasData("RACEHALFCOLSHAPE"))
        {
          NAPI.ColShape.DeleteColShape(player.GetMyData<ColShape>("RACEHALFCOLSHAPE"));

          //Удаляем чекпоинт промежуточный
          if (player.HasMyData("HASRACEHALFCOLSHAPE") && player.GetMyData<bool>("HASRACEHALFCOLSHAPE") == true)
          {
            Trigger.ClientEvent(player, "deleteCheckpoint", $"RACECOLSHAPEHALF {Main.Players[player].UUID}");
            Trigger.ClientEvent(player, "blip_remove", $"Половина гонки {Main.Players[player].UUID}");
            player.SetMyData<bool>("HASRACEHALFCOLSHAPE", false);

          }
        }

        //Удаляем чекпоинт финиша
        Trigger.ClientEvent(player, "deleteCheckpoint", $"RACECOLSHAPE {Main.Players[player].UUID}");
        Trigger.ClientEvent(player, "deleteCheckpoint", $"RACECOLSHAPEFINISH {Main.Players[player].UUID}");

        Trigger.ClientEvent(player, "blip_remove", $"Финиш {Main.Players[player].UUID}");

        if(player.HasData("RACEFINISHCOLSHAPE"))
        {
          NAPI.ColShape.DeleteColShape(player.GetMyData<ColShape>("RACEFINISHCOLSHAPE"));
        }

        Trigger.ClientEvent(player, "CLIENT::race:stopTimer");

        NAPI.Task.Run(() => {
          NAPI.Entity.SetEntityDimension(player, 0);
          NAPI.Entity.SetEntityPosition(player, QuestSystem.RaceNPCEnd); // Дейв
        }, 1000);

        NAPI.Task.Run(() => {
          Trigger.ClientEvent(player, "screenFadeIn", 1000);
        }, 2000);

        player.SetMyData("RACE_FAILED", true);
      }
      catch (Exception e) { Log.Write("Race Failed: " + e.StackTrace, nLog.Type.Error); }
    }

    public static void OnPlayerDeath(Player player, Player killer, uint reason)
    {
      try
      {
          if (!Main.Players[player].RaceIsActive || Main.Players[player].RaceIsEnd) return;

          player.SetSharedData("InDeath", false);
          Trigger.ClientEvent(player, "DeathTimer", false);
          player.ResetMyData("IS_DYING");
          player.ResetSharedData("IS_DYING");
          Main.Players[player].IsAlive = true;

          NAPI.Player.SpawnPlayer(player, player.Position);
          //Trigger.ClientEvent(player, "screenFadeOut", 1000);

          var hp = player.GetMyData<int>("BEFORE_RACE_HP");
          player.StopAnimation();
          NAPI.Player.SetPlayerHealth(player, hp);
          Main.OffAntiAnim(player);

          playerFailedRace(player);
      }
      catch (Exception e) { Log.Write("Race PlayerDeath: " + e.StackTrace, nLog.Type.Error); }
    }

    #endregion

    private static void OnTimedEvent(object source, ElapsedEventArgs e, Player player)
    {
      //var time = e.SignalTime;

      //if (Main.Players[player].aTimerStartTime == TimeSpan.Zero)
      //{
      //    var timeofday = DateTime.Now;
      //    var timeofdayFormated = string.Format("{0:HH:mm:ss}", timeofday);

      //    Main.Players[player].aTimerStartTime = TimeSpan.Parse(timeofdayFormated);
      //}

      //Main.Players[player].aTimerElapsed = time - Main.Players[player].aTimerStartTime;

      //Main.Players[player].aTimerFormatedTime = string.Format("{0:HH:mm:ss}", Main.Players[player].aTimerElapsed);


      //Trigger.ClientEvent(player, "raceUpdateTimer", Main.Players[player].aTimerFormatedTime);
    }

    private static TimeSpan DateTimeToTimeSpan(DateTime? ts)
    {
      if (!ts.HasValue) return TimeSpan.Zero;
      else return new TimeSpan(0, ts.Value.Hour, ts.Value.Minute, ts.Value.Second, ts.Value.Millisecond);
    }

    private static int getPlayerPosition(Player player, TimeSpan elapsedTime)
    {
      int position = 0;

      if (elapsedTime <= gold) position = 1;
      else if (elapsedTime <= silver) position = 2;
      else if (elapsedTime <= bronze) position = 3;
      else if (elapsedTime > bronze) position = 4;

      return position;
    }

    private static void giveRaceResult(Player player, int elapsedTime)
    {
      TimeSpan elapsedTimeSpan = TimeSpan.FromMilliseconds(elapsedTime);
      var elapsedTimeFormated = string.Format("{0:D2}:{1:D2}:{2:D2}", elapsedTimeSpan.Hours, elapsedTimeSpan.Minutes, elapsedTimeSpan.Seconds);
      Main.Players[player].RacePosition = getPlayerPosition(player, elapsedTimeSpan);
      Main.Players[player].RaceResultTime = elapsedTimeFormated;

      Trigger.ClientEvent(player, "notificationTOP", elapsedTimeFormated, 7000, "race", Main.Players[player].RacePosition);
    }

    #region Открываем меню призов

    public static void openRacePrizeMenu(Player player)
    {
      if (Main.Players[player].RacePosition == 0)
      {
        Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы не можете получить приз за гонку так как вы не заняли призового места", 3000);
        return;
      }

      List<ObjectCars> carsList = new List<ObjectCars>();
      List<string> prizeCarsList = new List<string>();

      switch (Main.Players[player].RacePosition)
      {
        case 1:
          prizeCarsList = carPrizesGold;
          break;
        case 2:
          prizeCarsList = carPrizesSilver;
          break;
        case 3:
          prizeCarsList = carPrizesBronze;
          break;
        case 4:
          prizeCarsList = carPrizesForAll;
          break;
      }

      bool hasSelectedCar = false;
      var selectedCarIndex = 0;
      int index = 1;

      foreach (var car in prizeCarsList)
      {
        VehicleHash carHash = (VehicleHash)NAPI.Util.GetHashKey(car);
        string vehicleRealName = carRaceName[car];

        var carName = $"{vehicleRealName}";
        var carModel = car;
        var carNumber = "RACE";
        bool selected = false;

        if (!hasSelectedCar)
        {
          selected = true;
        }

        if (Main.Players[player].RaceVehicleName == car)
        {
          hasSelectedCar = true;
          selectedCarIndex = index;
          selected = true;

          int timer = player.GetMyData<int>("PRIZEVEHICLETIMER") - player.GetMyData<int>("PRIZE_VEHICLE_TIMER_COUNT");
          if (timer <= 0)
          {
            selected = false;
          }
        }

        string carData = "{" +
            "'name':'" + carName + "', " +
            "'number':'" + carNumber + "', " +
            "'model':'" + carModel + "', " +
            "'selected':'" + selected + "', " +
            "'hash':'" + carHash + "" +
        "'}";

        ObjectCars convertedcarData = JsonConvert.DeserializeObject<ObjectCars>(carData);

        carsList.Add(convertedcarData);
        index++;
      }

      Trigger.ClientEvent(player, "raceOpenPrizeMenu", JsonConvert.SerializeObject(carsList), hasSelectedCar, selectedCarIndex, Main.Players[player].RacePosition);
    }



    #endregion

    [RemoteEvent("race:givePrizeCar")]
    public static void givePrizeVehicle(Player player, string vehicleName)
    {
      List<string> prizeCarsList = new List<string>();

      switch (Main.Players[player].RacePosition)
      {
        case 1:
          prizeCarsList = carPrizesGold;
          break;
        case 2:
          prizeCarsList = carPrizesSilver;
          break;
        case 3:
          prizeCarsList = carPrizesBronze;
          break;
        case 4:
          prizeCarsList = carPrizesForAll;
          break;
      }

      if (!prizeCarsList.Contains(vehicleName)) return;

      if (Main.Players[player].RaceVehicleName == "")
      {
        var RandomPos = VehicleManager.GetFreePosition(racePrizeSpawnCarPosition);
        if (RandomPos == -1)
        {
          RandomPos = 0;
        }
        var veh = NAPI.Vehicle.CreateVehicle((VehicleHash)NAPI.Util.GetHashKey(vehicleName), racePrizeSpawnCarPosition[RandomPos], racePrizeSpawnCarRotation[RandomPos].Z, 0, 0, "BONUSCAR");
        player.SetIntoVehicle(veh, 0);
        NAPI.Data.SetEntityData(veh, "ACCESS", "PRIZEVEHICLE");
        NAPI.Data.SetEntityData(veh, "DRIVER", player);
        VehicleStreaming.SetEngineState(veh, true);

        player.SetMyData("PRIZE_VEHICLE", player.Vehicle);
        player.SetMyData("LAST_PRIZE_VEHICLE", veh);
        player.SetMyData("IN_PRIZE_VEHICLE", true);
        player.SetMyData("PRIZEVEHICLETIMER", prizeTimer);
        player.SetMyData("PRIZE_VEHICLE_TIMER_COUNT", 0);

        if (player.HasMyData("PRIZE_VEHICLE_TIMER")) Timers.Stop(player.GetMyData<string>("PRIZE_VEHICLE_TIMER"));

        if (player.HasMyData("PRIZE_UPDATE_TIMER")) Timers.Stop(player.GetMyData<string>("PRIZE_UPDATE_TIMER"));

        NAPI.Task.Run(() =>
        {
          try
          {
            Log.Debug("\n[givePrizeVehicle] -> PRIZE_VEHICLE_TIMER START\n[givePrizeVehicle] -> PRIZE_UPDATE_TIMER START");

            player.SetMyData("PRIZE_VEHICLE_TIMER", Timers.Start(1000, () => timer_playerPrizeVehicle(player)));
            player.SetMyData("PRIZE_UPDATE_TIMER", Timers.Start(60000, () => timer_updatePlayerPrizeTimer(player)));
          }
          catch (Exception e) { Log.Write("loadTimer PRIZE_VEHICLE_TIMER START: " + e.ToString(), nLog.Type.Error); }
        }, 250);

        Main.Players[player].RaceVehicleName = vehicleName;
        Main.Players[player].RaceVehiclePosition = veh.Position;
        Main.Players[player].RaceVehicleRotation = veh.Position;
        Main.Players[player].RaceVehicleTimer = prizeTimer;
        Main.Players[player].RaceVehicle = veh;

        #region quest chapter iteration

        QuestSystem.UpdatePlayerQuestIteration(player, false);

        #endregion

        try
        {
            //MySQL.Query($"INSERT INTO `race_prizes` SET " +
            //  $"`uuid` = {Main.Players[player].UUID}," +
            //  $" `timer` = {prizeTimer}," +
            //  $" `vehicle` = '{vehicleName}'," +
            //  $" `vehicle_position` = '{JsonConvert.SerializeObject(veh.Position)}'," +
            //  $" `vehicle_rotation` = '{JsonConvert.SerializeObject(veh.Rotation)}'," +
            //  $" `result` = '{Main.Players[player].aTimerFormatedTime}'," +
            //  $" `result_position` = {racePosition}," +
            //  $" `characterName` = '{Main.Players[player].FirstName} {Main.Players[player].LastName}'");

            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = "INSERT INTO `race_prizes` SET " +
              "`uuid`=@uuid," +
              "`timer`=@timer," +
              "`vehicle`=@vehicle," +
              "`vehicle_position`=@vehicle_position," +
              "`vehicle_rotation`=@vehicle_rotation," +
              //"`result`=@result," +
              //"`result_position`=@result_position," +
              "`characterName`=@characterName";

            cmd.Parameters.AddWithValue("@uuid", Main.Players[player].UUID);
            cmd.Parameters.AddWithValue("@timer", prizeTimer);
            cmd.Parameters.AddWithValue("@vehicle", vehicleName);
            cmd.Parameters.AddWithValue("@vehicle_position", JsonConvert.SerializeObject(veh.Position));
            cmd.Parameters.AddWithValue("@vehicle_rotation", JsonConvert.SerializeObject(veh.Rotation));
            //cmd.Parameters.AddWithValue("@result", Main.Players[player].aTimerFormatedTime);
            //cmd.Parameters.AddWithValue("@result_position", racePosition);
            cmd.Parameters.AddWithValue("@characterName", $"{Main.Players[player].FirstName} {Main.Players[player].LastName}");
            MySQL.Query(cmd);
        }
        catch (Exception e) { Log.Write("INSERT INTO `race_prizes:\n" + e.ToString(), nLog.Type.Error); }

        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы получили транспорт во временное владение (Оставшееся время владения транспортом {convertTimeToHours(Main.Players[player].RaceVehicleTimer)}. Если вы уничтожите транспорт вы не сможете его восстановить)", 3000);
      }
      else
      {
        if (Main.Players[player].RaceVehicle != null && Main.Players[player].RaceVehicle.Position == new Vector3())
        {

          var RandomPos = VehicleManager.GetFreePosition(racePrizeSpawnCarPosition);
          if (RandomPos == -1)
          {
            RandomPos = 0;
          }
          NAPI.Task.Run(() => {
            var veh = NAPI.Vehicle.CreateVehicle((VehicleHash)NAPI.Util.GetHashKey(vehicleName), racePrizeSpawnCarPosition[RandomPos], racePrizeSpawnCarRotation[RandomPos].Z, 0, 0, "BONUSCAR");
            player.SetIntoVehicle(veh, 0);

            NAPI.Data.SetEntityData(veh, "ACCESS", "PRIZEVEHICLE");
            NAPI.Data.SetEntityData(veh, "DRIVER", player);
            VehicleStreaming.SetEngineState(veh, true);

            player.SetMyData("IN_PRIZE_VEHICLE", true);
            player.SetMyData("PRIZE_VEHICLE", player.Vehicle);
            player.SetMyData("LAST_PRIZE_VEHICLE", veh);

            Main.Players[player].RaceVehicleName = vehicleName;
            Main.Players[player].RaceVehiclePosition = veh.Position;
            Main.Players[player].RaceVehicleRotation = veh.Position;
            Main.Players[player].RaceVehicle = veh;
          });
        }
        else
        {
          NAPI.Task.Run(() =>
          {
            if (Main.Players[player].RaceVehicle != null)
            {
              NAPI.Entity.DeleteEntity(Main.Players[player].RaceVehicle);
            }


            var RandomPos = VehicleManager.GetFreePosition(racePrizeSpawnCarPosition);
            if (RandomPos == -1)
            {
              RandomPos = 0;
            }
            var veh = NAPI.Vehicle.CreateVehicle((VehicleHash)NAPI.Util.GetHashKey(Main.Players[player].RaceVehicleName), racePrizeSpawnCarPosition[RandomPos], racePrizeSpawnCarRotation[RandomPos].Z, 0, 0, "BONUSCAR");
            NAPI.Data.SetEntityData(veh, "ACCESS", "PRIZEVEHICLE");
            NAPI.Data.SetEntityData(veh, "DRIVER", player);
            player.SetIntoVehicle(veh, 0);
            VehicleStreaming.SetEngineState(veh, true);

            player.SetMyData("IN_PRIZE_VEHICLE", true);
            player.SetMyData("PRIZE_VEHICLE", player.Vehicle);
            player.SetMyData("LAST_PRIZE_VEHICLE", veh);

            Main.Players[player].RaceVehiclePosition = veh.Position;
            Main.Players[player].RaceVehicleRotation = veh.Position;
            Main.Players[player].RaceVehicle = veh;
          });

        }


      }
    }

    public static string convertTimeToMin(int timer)
    {
      int result = timer / 60;

      return result + "мин";
    }

    public static string convertTimeToHours(int timer)
    {
      int result = timer / 60 / 60;

      if (result < 1)
      {
        return convertTimeToMin(timer);
      }

      return result + "ч";
    }

    public static void loadTimer(Player player)
    {
      //DataTable race_prizes = MySQL.QueryRead($"SELECT * FROM `race_prizes` WHERE `uuid` = {Main.Players[player].UUID}");

      MySqlCommand cmd = new MySqlCommand();
      cmd.CommandText = "SELECT * FROM `race_prizes` WHERE `uuid`=@uuid";

      cmd.Parameters.AddWithValue("@uuid", Main.Players[player].UUID);
      DataTable race_prizes = MySQL.QueryRead(cmd);

      if (race_prizes != null && race_prizes.Rows.Count != 0)
      {
        string vehicleName = Convert.ToString(race_prizes.Rows[0]["vehicle"]);
        Vector3 vehiclePosition = JsonConvert.DeserializeObject<Vector3>(race_prizes.Rows[0]["vehicle_position"].ToString());
        Vector3 vehicleRotation = JsonConvert.DeserializeObject<Vector3>(race_prizes.Rows[0]["vehicle_rotation"].ToString());
        int vehicleTimer = Convert.ToInt32(race_prizes.Rows[0]["timer"]);

        if (vehicleTimer > 0)
        {
          player.SetMyData("PRIZEVEHICLETIMER", vehicleTimer);
          player.SetMyData("PRIZE_VEHICLE_TIMER_COUNT", 0);

          if (player.HasMyData("PRIZE_VEHICLE_TIMER")) Timers.Stop(player.GetMyData<string>("PRIZE_VEHICLE_TIMER"));
          if (player.HasMyData("PRIZE_UPDATE_TIMER")) Timers.Stop(player.GetMyData<string>("PRIZE_UPDATE_TIMER"));

          NAPI.Task.Run(() =>
          {
            try
            {
              Log.Debug($"\n[loadTimer {player.Name}] -> PRIZE_VEHICLE_TIMER START\n[loadTimer {player.Name}] -> PRIZE_UPDATE_TIMER START");

              player.SetMyData("PRIZE_VEHICLE_TIMER", Timers.Start(1000, () => timer_playerPrizeVehicle(player)));
              player.SetMyData("PRIZE_UPDATE_TIMER", Timers.Start(60000, () => timer_updatePlayerPrizeTimer(player)));
            }
            catch (Exception e) { Log.Write("loadTimer PRIZE_VEHICLE_TIMER START: " + e.ToString(), nLog.Type.Error); }
          }, 250);

          Main.Players[player].RaceVehiclePosition = vehiclePosition;
          Main.Players[player].RaceVehicleRotation = vehicleRotation;
        }

        Main.Players[player].RaceVehicleTimer = vehicleTimer;
        Main.Players[player].RaceVehicleName = vehicleName;

      }
    }

    public static void timer_playerPrizeVehicle(Player player)
    {
      NAPI.Task.Run(() =>
      {
        try
        {
          if (!player.HasMyData("PRIZE_VEHICLE_TIMER")) return; // Если не установлен таймер, то не считаем
          if (!player.HasMyData("PRIZEVEHICLETIMER")) return; // Если нету конечного времени, то не считаем
          if (!player.HasMyData("LAST_PRIZE_VEHICLE") || !player.HasMyData("PRIZE_VEHICLE")) return; // Если чувак не вызывал тачку, то не считаем

          var rentTImer = player.GetMyData<int>("PRIZEVEHICLETIMER");

          if (player.GetMyData<int>("PRIZE_VEHICLE_TIMER_COUNT") > rentTImer)
          {
            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Срок временного владения транспортом закончился.", 3000);

            if (player.HasMyData("LAST_PRIZE_VEHICLE") && player.HasMyData("PRIZE_VEHICLE"))
            {
              NAPI.Task.Run(() =>
              {
                try
                {
                  NAPI.Entity.DeleteEntity(player.GetMyData<Vehicle>("LAST_PRIZE_VEHICLE"));
                  player.ResetMyData("PRIZE_VEHICLE");
                }
                catch (Exception e) { Log.Write("timer_playerPrizeVehicle REMOVE VEHICLE: " + e.ToString(), nLog.Type.Error); }
              });
            }

            Timers.Stop(player.GetMyData<string>("PRIZE_VEHICLE_TIMER"));
            Timers.Stop(player.GetMyData<string>("PRIZE_UPDATE_TIMER"));
            player.ResetMyData("PRIZE_VEHICLE_TIMER");
            player.ResetMyData("PRIZE_UPDATE_TIMER");

            //player.ResetMyData("PRIZE_VEHICLE_TIMER");
            //player.ResetMyData("PRIZE_UPDATE_TIMER");
            //player.ResetMyData("PRIZE_VEHICLE_TIMER_COUNT");
            //player.ResetMyData("PRIZEVEHICLETIMER");

            //Log.Debug("Prize VEHICLE REMOVED? TIMER STOPED?", nLog.Type.Warn);

            //MySQL.Query($"UPDATE `race_prizes` SET `timer` = 0 WHERE uuid={Main.Players[player].UUID}");

            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = "UPDATE `race_prizes` SET `timer`=@timer WHERE `uuid`=@uuid";

            cmd.Parameters.AddWithValue("@timer", 0);
            cmd.Parameters.AddWithValue("@uuid", Main.Players[player].UUID);
            MySQL.Query(cmd);

            return;
          }

          player.SetMyData("PRIZE_VEHICLE_TIMER_COUNT", player.GetMyData<int>("PRIZE_VEHICLE_TIMER_COUNT") + 1, false);
        }
        catch (Exception e) { Log.Write("timer_playerPrizeVehicle: " + e.ToString(), nLog.Type.Error); }
      });
    }

    public static void timer_updatePlayerPrizeTimer(Player player)
    {
      NAPI.Task.Run(() =>
      {
        try
        {

          if (player.HasMyData("PRIZEVEHICLETIMER") && player.HasMyData("PRIZE_VEHICLE_TIMER_COUNT") && player.HasMyData("LAST_PRIZE_VEHICLE"))
          {
            Vehicle veh = player.GetMyData<Vehicle>("LAST_PRIZE_VEHICLE");
            Vector3 vehicle_position = veh.Position;
            Vector3 vehicle_rotation = veh.Rotation;

            int timer = player.GetMyData<int>("PRIZEVEHICLETIMER") - player.GetMyData<int>("PRIZE_VEHICLE_TIMER_COUNT");

            //MySQL.Query($"UPDATE `race_prizes` SET `timer` = {timer}," +
            //  $" `vehicle_position` = '{JsonConvert.SerializeObject(vehicle_position)}'," +
            //  $" `vehicle_rotation` = '{JsonConvert.SerializeObject(vehicle_rotation)}' WHERE uuid={Main.Players[player].UUID}");

            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = "UPDATE `race_prizes` SET " +
            "`timer`=@timer," +
            "`vehicle_position`=@vehicle_position," +
            "`vehicle_rotation`=@vehicle_rotation" +
            " WHERE `uuid`=@uuid";

            cmd.Parameters.AddWithValue("@timer", timer);
            cmd.Parameters.AddWithValue("@vehicle_position", JsonConvert.SerializeObject(vehicle_position));
            cmd.Parameters.AddWithValue("@vehicle_rotation", JsonConvert.SerializeObject(vehicle_rotation));
            cmd.Parameters.AddWithValue("@uuid", Main.Players[player].UUID);
            MySQL.Query(cmd);

            Log.Debug($"RacePrize timer Changed: current: {timer}", nLog.Type.Info);
          }
        }
        catch (Exception e) { Log.Write("timer_updatePlayerPrizeTimer: " + e.StackTrace, nLog.Type.Error); }
      });
    }

    public static void Event_OnPlayerDisconnected(Player player)
    {
      try
      {
        if (!Main.Players.ContainsKey(player)) return;

        if (player.HasMyData("RACEVEH"))
        {
            var raceveh = player.GetMyData<Vehicle>("RACEVEH");
            NAPI.Task.Run(() =>
            {
                NAPI.Entity.DeleteEntity(raceveh);
            });
        }

        if (player.HasMyData("PRIZE_VEHICLE_TIMER") && player.HasMyData("PRIZEVEHICLETIMER") && player.HasMyData("LAST_PRIZE_VEHICLE"))
        {
          Vehicle veh = player.GetMyData<Vehicle>("LAST_PRIZE_VEHICLE");
          Vector3 vehicle_position = veh.Position;
          Vector3 vehicle_rotation = veh.Rotation;

          NAPI.Task.Run(() =>
          {
            try
            {
              if (player.HasMyData("LAST_PRIZE_VEHICLE")) NAPI.Entity.DeleteEntity(player.GetMyData<Vehicle>("LAST_PRIZE_VEHICLE"));
              player.ResetMyData("PRIZE_VEHICLE");
            }
            catch (Exception e) { Log.Write("timer_playerPrizeVehicle REMOVE VEHICLE: " + e.ToString(), nLog.Type.Error); }
          });

          Timers.Stop(player.GetMyData<string>("PRIZE_VEHICLE_TIMER"));
          int timer = player.GetMyData<int>("PRIZEVEHICLETIMER") - player.GetMyData<int>("PRIZE_VEHICLE_TIMER_COUNT");

          //MySQL.Query($"UPDATE `race_prizes` SET `timer` = {timer}, `vehicle_position` = '{JsonConvert.SerializeObject(vehicle_position)}', `vehicle_rotation` = '{JsonConvert.SerializeObject(vehicle_rotation)}' WHERE uuid={Main.Players[player].UUID}");

          MySqlCommand cmd = new MySqlCommand();
          cmd.CommandText = "UPDATE `race_prizes` SET " +
          "`timer`=@timer," +
          "`vehicle_position`=@vehicle_position," +
          "`vehicle_rotation`=@vehicle_rotation" +
          " WHERE `uuid`=@uuid";

          cmd.Parameters.AddWithValue("@timer", timer);
          cmd.Parameters.AddWithValue("@vehicle_position", JsonConvert.SerializeObject(vehicle_position));
          cmd.Parameters.AddWithValue("@vehicle_rotation", JsonConvert.SerializeObject(vehicle_rotation));
          cmd.Parameters.AddWithValue("@uuid", Main.Players[player].UUID);
          MySQL.Query(cmd);

          Log.Debug($"Player Disconnected RacePrize timer Changed: current: {timer}", nLog.Type.Info);
        }
      }
      catch (Exception e) { Log.Write("PlayerDisconnected: " + e.StackTrace, nLog.Type.Error); }
    }

  }
}
