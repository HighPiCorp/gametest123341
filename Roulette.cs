using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using client.Systems.BattlePass;
using GTANetworkAPI;
using NeptuneEvo;
using NeptuneEvo.Core;
using NeptuneEvo.SDK;
using Newtonsoft.Json;
using Trigger = NeptuneEvo.Trigger;

namespace client.Core.Casino
{
  class CasinoRoulette : Script
  {
    private static nLog Log = new nLog("Casino Roulette");

    #region Массивы

    public static List<int> RouletteTableBetMin = new List<int>()
    {
    };

    public static List<int> RouletteTableBetMax = new List<int>()
    {
    };

    public static List<List<Bet>> RouletteBets = new List<List<Bet>>()
        {
            new List<Bet>(){},
            new List<Bet>(){},
        };

    public static List<Vector3> RouletteTables = new List<Vector3>()
        {
             new Vector3(1144.814, 268.2634, -52.8409),
             new Vector3(1150.355, 262.7224, -52.8409),
            // new Vector3(1144.814, 268.2634, -52.8409), 
        };

    public static List<List<Vector3>> RouletteSeats = new List<List<Vector3>>()
        {
            new List<Vector3>(){
                new Vector3(1144.717, 267.277, -52.840),
                new Vector3(1143.748, 267.377, -52.840),
                new Vector3(1143.655, 268.335, -52.840),
                new Vector3(1144.342, 269.022, -52.840),
            },
            new List<Vector3>(){
                new Vector3(1150.45, 263.708, -52.84),
                new Vector3(1151.42, 263.608, -52.84),
                new Vector3(1151.51, 262.650, -52.84),
                new Vector3(1150.82, 261.963, -52.84),
            },
        };

    public static Dictionary<int, List<int>> Spots = new Dictionary<int, List<int>>()
        {
            { 0, new List<int>{ 0 } },
            { 1, new List<int>{ -1 } },
            { 2, new List<int>{ 1 } },
            { 3, new List<int>{ 2 } },
            { 4, new List<int>{ 3 } },
            { 5, new List<int>{ 4 } },
            { 6, new List<int>{ 5 } },
            { 7, new List<int>{ 6 } },
            { 8, new List<int>{ 7 } },
            { 9, new List<int>{ 8 } },
            { 10, new List<int>{ 9 } },
            { 11, new List<int>{ 10 } },
            { 12, new List<int>{ 11 } },
            { 13, new List<int>{ 12 } },
            { 14, new List<int>{ 13 } },
            { 15, new List<int>{ 14 } },
            { 16, new List<int>{ 15 } },
            { 17, new List<int>{ 16 } },
            { 18, new List<int>{ 17 } },
            { 19, new List<int>{ 18 } },
            { 20, new List<int>{ 19 } },
            { 21, new List<int>{ 20 } },
            { 22, new List<int>{ 21 } },
            { 23, new List<int>{ 22 } },
            { 24, new List<int>{ 23 } },
            { 25, new List<int>{ 24 } },
            { 26, new List<int>{ 25 } },
            { 27, new List<int>{ 26 } },
            { 28, new List<int>{ 27 } },
            { 29, new List<int>{ 28 } },
            { 30, new List<int>{ 29 } },
            { 31, new List<int>{ 30 } },
            { 32, new List<int>{ 31 } },
            { 33, new List<int>{ 32 } },
            { 34, new List<int>{ 33 } },
            { 35, new List<int>{ 34 } },
            { 36, new List<int>{ 35 } },
            { 37, new List<int>{ 36 } },
            { 38, new List<int>{ 0, -1 } },
            { 39, new List<int>{ 1, 2 } },
            { 40, new List<int>{ 2, 3 } },
            { 41, new List<int>{ 4, 5 } },
            { 42, new List<int>{ 5, 6 } },
            { 43, new List<int>{ 7, 8 } },
            { 44, new List<int>{ 8, 9 } },
            { 45, new List<int>{ 10, 11 } },
            { 46, new List<int>{ 11, 12 } },
            { 47, new List<int>{ 13, 14 } },
            { 48, new List<int>{ 14, 15 } },
            { 49, new List<int>{ 16, 17 } },
            { 50, new List<int>{ 17, 18 } },
            { 51, new List<int>{ 19, 20 } },
            { 52, new List<int>{ 20, 21 } },
            { 53, new List<int>{ 22, 23 } },
            { 54, new List<int>{ 23, 24 } },
            { 55, new List<int>{ 25, 26 } },
            { 56, new List<int>{ 26, 27 } },
            { 57, new List<int>{ 28, 29 } },
            { 58, new List<int>{ 29, 30 } },
            { 59, new List<int>{ 31, 32 } },
            { 60, new List<int>{ 32, 33 } },
            { 61, new List<int>{ 34, 35 } },
            { 62, new List<int>{ 35, 36 } },
            { 63, new List<int>{ 1, 4 } },
            { 64, new List<int>{ 2, 5 } },
            { 65, new List<int>{ 3, 6 } },
            { 66, new List<int>{ 4, 7 } },
            { 67, new List<int>{ 5, 8 } },
            { 68, new List<int>{ 6, 9 } },
            { 69, new List<int>{ 7, 10 } },
            { 70, new List<int>{ 8, 11 } },
            { 71, new List<int>{ 9, 12 } },
            { 72, new List<int>{ 10, 13 } },
            { 73, new List<int>{ 11, 14 } },
            { 74, new List<int>{ 12, 15 } },
            { 75, new List<int>{ 13, 16 } },
            { 76, new List<int>{ 14, 17 } },
            { 77, new List<int>{ 15, 18 } },
            { 78, new List<int>{ 16, 19 } },
            { 79, new List<int>{ 17, 20 } },
            { 80, new List<int>{ 18, 21 } },
            { 81, new List<int>{ 19, 22 } },
            { 82, new List<int>{ 20, 23 } },
            { 83, new List<int>{ 21, 24 } },
            { 84, new List<int>{ 22, 25 } },
            { 85, new List<int>{ 23, 26 } },
            { 86, new List<int>{ 24, 27 } },
            { 87, new List<int>{ 25, 28 } },
            { 88, new List<int>{ 26, 29 } },
            { 89, new List<int>{ 27, 30 } },
            { 90, new List<int>{ 28, 31 } },
            { 91, new List<int>{ 29, 32 } },
            { 92, new List<int>{ 30, 33 } },
            { 93, new List<int>{ 31, 34 } },
            { 94, new List<int>{ 32, 35 } },
            { 95, new List<int>{ 33, 36 } },
            { 96, new List<int>{ 1, 2, 3 } },
            { 97, new List<int>{ 4, 5, 6 } },
            { 98, new List<int>{ 7, 8, 9 } },
            { 99, new List<int>{ 10, 11, 12 } },
            { 100, new List<int>{ 13, 14, 15 } },
            { 101, new List<int>{ 16, 17, 18 } },
            { 102, new List<int>{ 19, 20, 21 } },
            { 103, new List<int>{ 22, 23, 24 } },
            { 104, new List<int>{ 25, 26, 27 } },
            { 105, new List<int>{ 28, 29, 30 } },
            { 106, new List<int>{ 31, 32, 33 } },
            { 107, new List<int>{ 34, 35, 36 } },
            { 108, new List<int>{ 1, 2, 4, 5 } },
            { 109, new List<int>{ 2, 3, 5, 6 } },
            { 110, new List<int>{ 4, 5, 7, 8 } },
            { 111, new List<int>{ 5, 6, 8, 9 } },
            { 112, new List<int>{ 7, 8, 10, 11 } },
            { 113, new List<int>{ 8, 9, 11, 12 } },
            { 114, new List<int>{ 10, 11, 13, 14 } },
            { 115, new List<int>{ 11, 12, 14, 15 } },
            { 116, new List<int>{ 13, 14, 16, 17 } },
            { 117, new List<int>{ 14, 15, 17, 18 } },
            { 118, new List<int>{ 16, 17, 19, 20 } },
            { 119, new List<int>{ 17, 18, 20, 21 } },
            { 120, new List<int>{ 19, 20, 22, 23 } },
            { 121, new List<int>{ 20, 21, 23, 24 } },
            { 122, new List<int>{ 22, 23, 25, 26 } },
            { 123, new List<int>{ 23, 24, 26, 27 } },
            { 124, new List<int>{ 25, 26, 28, 29 } },
            { 125, new List<int>{ 26, 27, 29, 30 } },
            { 126, new List<int>{ 28, 29, 31, 32 } },
            { 127, new List<int>{ 29, 30, 32, 33 } },
            { 128, new List<int>{ 31, 32, 34, 35 } },
            { 129, new List<int>{ 32, 33, 35, 36 } },
            { 130, new List<int>{ 0, 00, 1, 2, 3 } },
            { 131, new List<int>{ 1, 2, 3, 4, 5, 6 } },
            { 132, new List<int>{ 4, 5, 6, 7, 8, 9 } },
            { 133, new List<int>{ 7, 8, 9, 10, 11, 12 } },
            { 134, new List<int>{ 10, 11, 12, 13, 14, 15 } },
            { 135, new List<int>{ 13, 14, 15, 16, 17, 18 } },
            { 136, new List<int>{ 16, 17, 18, 19, 20, 21 } },
            { 137, new List<int>{ 19, 20, 21, 22, 23, 24 } },
            { 138, new List<int>{ 22, 23, 24, 25, 26, 27 } },
            { 139, new List<int>{ 25, 26, 27, 28, 29, 30 } },
            { 140, new List<int>{ 28, 29, 30, 31, 32, 33 } },
            { 141, new List<int>{ 31, 32, 33, 34, 35, 36 } },
            { 142, new List<int>{ 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 } },
            { 143, new List<int>{ 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24 } },
            { 144, new List<int>{ 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36 } },
            { 145, new List<int>{ 1, 4, 7, 10, 13, 16, 19, 22, 25, 28, 31, 34} },
            { 146, new List<int>{ 2, 5, 8, 11, 14, 17, 20, 23, 26, 29, 32, 35} },
            { 147, new List<int>{ 3, 6, 9, 12, 15, 18, 21, 24, 27, 30, 33, 36} },
            { 148, new List<int>{ 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18} },
            { 149, new List<int>{ 2, 4, 6, 8, 10, 12, 14, 16, 18, 20, 22, 24, 26, 28, 30, 32, 34, 36} },
            { 150, new List<int>{ 1, 3, 5, 7, 9, 12, 14, 16, 18, 19, 21, 23, 25, 27, 30, 32, 34, 36} },
            { 151, new List<int>{ 2, 4, 6, 8, 10, 11, 13, 15, 17, 20, 22, 24, 26, 28, 29, 31, 33, 35} },
            { 152, new List<int>{ 1, 3, 5, 7, 9, 11, 13, 15, 17, 19, 21, 23, 25, 27, 29, 31, 33, 35} },
            { 153, new List<int>{ 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36} },
        };

    public static List<int> RouletteColors = new List<int>()
        {
            -1,
            1,
            0,
            1,
            0,
            1,
            0,
            1,
            0,
            1,
            0,
            1,
            0,
            1,
            0,
            1,
            0,
            1,
            0,
            -1,
            0,
            1,
            0,
            1,
            0,
            1,
            0,
            1,
            0,
            1,
            0,
            1,
            0,
            1,
            0,
            1,
            0,
            1
        };

    public static Dictionary<int, int> RouletteNumbers = new Dictionary<int, int>()
        {
            {-1, 1 },
            {0, 20 },
            {1, 38 },
            {2, 19 },
            {3, 34 },
            {4, 15 },
            {5, 30 },
            {6, 11 },
            {7, 26 },
            {8, 7 },
            {9, 22 },
            {10, 3 },
            {11, 25 },
            {12, 6 },
            {13, 37 },
            {14, 18 },
            {15, 33 },
            {16, 14 },
            {17, 29 },
            {18, 10 },
            {19, 8 },
            {20, 27 },
            {21, 12 },
            {22, 31 },
            {23, 16 },
            {24, 35 },
            {25, 4 },
            {26, 23 },
            {27, 2},
            {28, 21 },
            {29, 5 },
            {30, 24 },
            {31, 9 },
            {32, 28 },
            {33, 13 },
            {34, 32 },
            {35, 17 },
            {36, 36 },
        };

    public static List<int> RouletteBetTime = new List<int>()
        {
            0, 0
        };

    public static List<List<Player>> RoulettePlayers = new List<List<Player>>() {
            new List<Player>(){ },
            new List<Player>(){ },
        };

    public static List<int> RouletteStatus = new List<int>() {
            0,
            0
        };

    public static List<int> RouletteNum = new List<int>() {
            0,
            0
        };

    public static List<List<Player>> RouletteSeatsPlayers = new List<List<Player>>() {
            new List<Player>(){ },
            new List<Player>(){ },
        };

    public static List<Thread> CasinoThreads = new List<Thread>();

    #endregion

    [ServerEvent(Event.ResourceStart)]
    public void ResourceStart()
    {
      try
      {
        var result = MySQL.QueryRead($"SELECT * FROM `casinoroulette`");
        if (result == null || result.Rows.Count == 0)
        {
          Log.Write("DB return null result.", nLog.Type.Warn);
          return;
        }
        foreach (DataRow Row in result.Rows)
        {
          try
          {
            var id = Convert.ToInt32(Row["id"].ToString());
            var minbet = Convert.ToInt32(Row["minbet"]);
            var maxbet = Convert.ToInt32(Row["maxbet"]);

            RouletteTableBetMin.Add(minbet);
            RouletteTableBetMax.Add(maxbet);

          }
          catch (Exception e)
          {
            Log.Write(Row["id"].ToString() + e.ToString(), nLog.Type.Error);
          }

        }

        for (int i = 0; i < RouletteTables.Count; i++)
        {
          var colshape = NAPI.ColShape.CreateCylinderColShape(RouletteTables[i], 2, 2);
          colshape.SetData("TABLE", i);
          colshape.OnEntityEnterColShape += (shape, player) => {
            player.SetMyData("INTERACTIONCHECK", 664);
            player.SetMyData("TABLE", shape.GetData<int>("TABLE"));
            Trigger.PlayerEvent(player, "client_press_key_to", "open", JsonConvert.SerializeObject(new List<object>() { "E", "сесть за стол" }));
          };
          colshape.OnEntityExitColShape += (shape, player) => {
            player.SetMyData("INTERACTIONCHECK", 0);
            player.ResetMyData("TABLE");
            Trigger.PlayerEvent(player, "client_press_key_to", "close");
          };
        }


        for (int i = 0; i < 2; i++)
        {

          Thread thd = new Thread(new ParameterizedThreadStart(StartRoulette)) { IsBackground = true };
          thd.Start(i);
          CasinoThreads.Add(thd);

        }
        Log.Write($"Roulette loaded", nLog.Type.Success);
      }
      catch (Exception ex)
      {
        Log.Write($"{ex.StackTrace}");
      }
    }

    [ServerEvent(Event.PlayerDisconnected)]
    public static void PlayerDisconnect(Player player, DisconnectionType type, string reason)
    {
      try
      {
        if (player.HasMyData("RL_TABLE"))
        {
          var table = player.GetMyData<int>("RL_TABLE");
          if (RoulettePlayers[table].Contains(player))
          {
            if (RouletteSeatsPlayers[table].Contains(player))
            {
              RouletteSeatsPlayers[table].Remove(player);
              RoulettePlayers[table].Remove(player);
            }
          }
          else if (RouletteSeatsPlayers[table].Contains(player))
          {
            RouletteSeatsPlayers[table].Remove(player);
          }

          Log.Debug($"PlayerDisconnect [table -> {table}] ->>> \n\n\n RouletteSeatsPlayers[table]: {JsonConvert.SerializeObject(RouletteSeatsPlayers[table])} \n\n\n RoulettePlayers[table]: {JsonConvert.SerializeObject(RoulettePlayers[table])}\n\n\n", nLog.Type.Warn);
        }
      }
      catch (Exception ex)
      {
        Log.Write($"{ex.StackTrace}");
      }
    }

    [RemoteEvent("SERVER::CASINO:EXIT_ROULETTE")]
    public static void ExitRoulette(Player player)
    {
      try
      {
        if (player.HasMyData("RL_TABLE"))
        {
          int table = player.GetMyData<int>("RL_TABLE");

          if (RoulettePlayers[table].Contains(player))
          {
            Trigger.ClientEvent(player, "CLIENT::casino:return_roulette");
            Dictionary<string, object> dict = new Dictionary<string, object>();

            dict.Add("show", true);
            dict.Add("activeTab", "Waiting");
            dict.Add("chips", DiamondCasino.GetAllChips(player));
            dict.Add("maxTime", 15);
            dict.Add("time", RouletteBetTime[table]);
            dict.Add("betStatus", Convert.ToBoolean(RouletteStatus[table]));
            dict.Add("bet", new Dictionary<string, object>()
            {
                {"count", RouletteTableBetMin[table]},
                {"max", RouletteTableBetMax[table]},
                {"min", RouletteTableBetMin[table]},
                {"change", RouletteTableBetMin[table]},
            });

            Trigger.ClientEvent(player, "CLIENT::CASINO:OPEN", JsonConvert.SerializeObject(dict), "ROULETTE");
            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomRight, "Дождитесь окончания игры!", 3000);
            return;
          }

          RouletteSeatsPlayers[table].Remove(player);
          Log.Debug($"SERVER::CASINO:EXIT_ROULETTE [table -> {table}] ->>> \n\n\n RouletteSeatsPlayers[table]: {JsonConvert.SerializeObject(RouletteSeatsPlayers[table])} \n\n\n RoulettePlayers[table]: {JsonConvert.SerializeObject(RoulettePlayers[table])}\n\n\n", nLog.Type.Warn);

          Trigger.PlayerEvent(player, "exit_roulette");
          Trigger.ClientEvent(player, "CLIENT::casino:hidef");
          Trigger.ClientEvent(player, "CLIENT::casino:hide_roulette");
          player.ResetMyData("RL_TABLE");
          return;
        }
      }
      catch (Exception ex) { Log.Write(ex.StackTrace); }
    }

    public static void StartRoulette(object table)
    {
      try
      {
        int tb = Convert.ToInt32(table);

        while (true)
        {
          if (RouletteBetTime[tb] != 0)
          {
            try
            {
              if (RouletteSeatsPlayers[tb].Count != 0)
              {
                for (int i = 0; i < RouletteSeatsPlayers[tb].Count; i++)
                {
                  if (!Main.Players.ContainsKey(RouletteSeatsPlayers[tb][i])) continue;
                  //UpdateCasinoTimeGUI(RouletteSeatsPlayers[tb][i]);
                  Dictionary<string, object> dict = new Dictionary<string, object>();

                  dict.Add("show", true);
                  dict.Add("activeTab", "Waiting");
                  dict.Add("chips", DiamondCasino.GetAllChips(RouletteSeatsPlayers[tb][i]));
                  dict.Add("maxTime", 15);
                  dict.Add("time", RouletteBetTime[tb]);

                  Trigger.ClientEvent(RouletteSeatsPlayers[tb][i], "CLIENT::CASINO:UPDATE", JsonConvert.SerializeObject(dict));
                }
              }

              RouletteBetTime[tb] -= 1;

              Thread.Sleep(1000);

              if (RouletteBetTime[tb] != 0)
                continue;
            }
            catch (Exception ex)
            {
              Log.Write("StartRoulette 1: " + ex.StackTrace, nLog.Type.Error);
            }
          }

          if (RoulettePlayers[tb].Count == 0 && RouletteBetTime[tb] == 0)
          {
            RouletteBetTime[tb] = 15;
            Thread.Sleep(1000);
            continue;
          }

          for (int i = 0; i < RouletteSeatsPlayers[tb].Count; i++)
          {
            if (!Main.Players.ContainsKey(RouletteSeatsPlayers[tb][i])) continue;
            //UpdateCasinoTimeGUI(RouletteSeatsPlayers[tb][i]);
            Dictionary<string, object> dict = new Dictionary<string, object>();

            dict.Add("activeTab", "Waiting");
            dict.Add("chips", DiamondCasino.GetAllChips(RouletteSeatsPlayers[tb][i]));
            dict.Add("maxTime", 15);
            dict.Add("time", 15);

            Trigger.ClientEvent(RouletteSeatsPlayers[tb][i], "CLIENT::CASINO:UPDATE", JsonConvert.SerializeObject(dict));
          }

          RouletteStatus[tb] = 1;

          Spin(tb);

          for (int i = 0; i < RoulettePlayers[tb].Count; i++)
          {
            try
            {
              if (!Main.Players.ContainsKey(RoulettePlayers[tb][i])) continue;

              Dictionary<string, object> dict = new Dictionary<string, object>();
              //dict.Add("show", false);
              dict.Add("activeTab", "Waiting");
              dict.Add("betStatus", true);
              dict.Add("chips", DiamondCasino.GetAllChips(RouletteSeatsPlayers[tb][i]));
              dict.Add("maxTime", 15);
              dict.Add("time", 15);

              Trigger.ClientEvent(RouletteSeatsPlayers[tb][i], "CLIENT::CASINO:UPDATE", JsonConvert.SerializeObject(dict));

              Trigger.PlayerEvent(RoulettePlayers[tb][i], "start_roulette");
              //Notify.Send(RoulettePlayers[tb][i], NotifyType.Info, NotifyPosition.BottomCenter, "Ставки приняты", 3000);
            }
            catch (Exception e)
            {
              Log.Write("Start Roulette 2: " + e.StackTrace, nLog.Type.Error);
            }
          }

          Thread.Sleep(25000);

          for (int i = 0; i < RoulettePlayers[tb].Count; i++)
          {
            if (!Main.Players.ContainsKey(RoulettePlayers[tb][i])) continue;

            try
            {
              Trigger.PlayerEvent(RoulettePlayers[tb][i], "stop_roulette", tb);
            }
            catch (Exception e)
            {
              Log.Write("Start Roulette 3: " + e.StackTrace, nLog.Type.Error);
            }
          }

          Trigger.PlayerEventInRange(RouletteTables[tb], 40, "clean_chips", tb);

          foreach (Bet bet in RouletteBets[tb])
          {
            if (Spots[bet.Spot].Contains(RouletteNum[tb]))
            {
              int mnoj = 2;

              if (bet.Spot <= 37)
                mnoj = 36;
              else if (bet.Spot >= 38 && bet.Spot <= 95)
                mnoj = 18;
              else if (bet.Spot >= 96 && bet.Spot <= 107)
                mnoj = 12;
              else if (bet.Spot >= 108 && bet.Spot <= 129)
                mnoj = 9;
              else if (bet.Spot >= 142 && bet.Spot <= 147)
                mnoj = 3;
              else if (bet.Spot >= 130)
                mnoj = 2;

              if (Main.Players.ContainsKey(bet.Player)) bet.Player.SetMyData("RL_WIN", bet.Player.GetMyData<int>("RL_WIN") + (bet.BetAmount * mnoj));
            }
          }

          for (int i = 0; i < RoulettePlayers[tb].Count; i++)
          {

            try
            {

              if (!Main.Players.ContainsKey(RoulettePlayers[tb][i])) continue;

              string winStr = "";

              if (RouletteNum[tb] == -1)
                winStr = "00";
              else
                winStr = $"{RouletteNum[tb]}";

              if (RoulettePlayers[tb][i].GetMyData<int>("RL_WIN") <= 0)
              {
                Notify.Send(RoulettePlayers[tb][i], NotifyType.Error, NotifyPosition.BottomRight, $"Выпало: {winStr}. Ваши ставки проиграли", 3000);
              }
              else
              {
                int prize = RoulettePlayers[tb][i].GetMyData<int>("RL_WIN");
                Notify.Send(RoulettePlayers[tb][i], NotifyType.Success, NotifyPosition.BottomRight, $"Выпало: {winStr}. Вы выиграли {prize} фишек!", 3000);
                nInventory.Add(RoulettePlayers[tb][i], new nItem(ItemType.CasinoChips, prize));
              }

              Dictionary<string, object> dict = new Dictionary<string, object>();

              dict.Add("activeTab", "Waiting");
              dict.Add("chips", DiamondCasino.GetAllChips(RouletteSeatsPlayers[tb][i]));
              dict.Add("maxTime", 30);
              dict.Add("betStatus", false);
              dict.Add("time", 30);

              Trigger.ClientEvent(RouletteSeatsPlayers[tb][i], "CLIENT::CASINO:OPEN", JsonConvert.SerializeObject(dict));

              //UpdateCasinoChipsGUI(RoulettePlayers[tb][i]);
              RoulettePlayers[tb][i].SetMyData("RL_WIN", 0);

            }
            catch (Exception e)
            {
              Log.Write("Start Roulette 4: " + e.StackTrace, nLog.Type.Error);
            }
          }

          RouletteBets[tb].Clear();
          RoulettePlayers[tb].Clear();
          RouletteStatus[tb] = 0;
          RouletteBetTime[tb] = 30;

          Thread.Sleep(2000);
        }
      }
      catch (Exception ex) { Log.Write(ex.StackTrace); }
    }

    public static void Spin(int table)
    {
      try
      {
        Random rand = new Random();

        int num = rand.Next(-1, 37);
        RouletteNum[table] = num;

        //if(num == -1) player.SendChatMessage($"Выпадет 00");
        //else player.SendChatMessage($"Выпадет {num}");
        Trigger.PlayerEventInRange(RouletteTables[table], 50, "spin_wheel", table, 3, $"exit_{RouletteNumbers[num]}_wheel", $"exit_{RouletteNumbers[num]}_ball", RouletteNumbers[num]);
      }
      catch (Exception ex) { Log.Write(ex.StackTrace); }
    }

    public static void Roulette(Player player, int tb)
    {
      try
      {
        player.SetMyData("RL_TABLE", tb);
        player.SetMyData("RL_BET", 0);
        player.SetMyData("RL_SETBET", RouletteTableBetMin[tb]);

        RouletteSeatsPlayers[tb].Add(player);
      }
      catch (Exception ex) { Log.Write(ex.StackTrace); }
    }

    [RemoteEvent("server_make_roulette_bet")]
    public static void MakeRouletteBet(Player player, int spot)
    {
      try
      {
        if (!Main.Players.ContainsKey(player))
          return;
        if (!player.HasMyData("RL_TABLE"))
          return;
        if (!player.HasMyData("RL_SETBET"))
          return;

        if (RouletteStatus[player.GetMyData<int>("RL_TABLE")] != 0)
        {
          return;
        }

        RlBet(player, spot, player.GetMyData<int>("RL_SETBET"));
      }
      catch (Exception ex) { Log.Write(ex.StackTrace); }
    }

    public static void RlBet(Player player, int spot, int chips)
    {
      try
      {
        if (!player.HasMyData("RL_TABLE"))
          return;

        int table = player.GetMyData<int>("RL_TABLE");

        if (RouletteStatus[table] != 0)
        {
          Notify.Send(player, NotifyType.Error, NotifyPosition.BottomRight, "В данный момент нельзя сделать ставку", 3000);
          return;
        }

        player.SetMyData("RL_WIN", 0);

        var chipsCount = nInventory.FindCount(player, ItemType.CasinoChips);

        /*if (item == null)
        {
            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "У Вас недостаточно фишек", 3000);
            return;
        }*/
        if (chipsCount < chips)
        {
          Notify.Send(player, NotifyType.Error, NotifyPosition.BottomRight, "У Вас недостаточно фишек", 3000);
          return;
        }

        if (RouletteBets[player.GetMyData<int>("RL_TABLE")].Count(t => t.Player == player && t.Spot == spot) > 0)
        {
          Notify.Send(player, NotifyType.Error, NotifyPosition.BottomRight, "Вы уже поставили на этот спот", 3000);
          return;
        }

        if (RouletteBets[player.GetMyData<int>("RL_TABLE")].Count(t => t.Player == player) >= 3)
        {
          Notify.Send(player, NotifyType.Error, NotifyPosition.BottomRight, "Максимум 3 ставки", 3000);
          return;
        }

        if (!RoulettePlayers[player.GetMyData<int>("RL_TABLE")].Contains(player))
          RoulettePlayers[player.GetMyData<int>("RL_TABLE")].Add(player);

        nInventory.Remove(player, ItemType.CasinoChips, chips);

        #region BPКвест: 20 Сделайте ставок на 1.000 фишек в рулетке. / 23 Оставьте в казино более чем 100.000 фишек.

        #region BattlePass выполнение квеста
        BattlePass.updateBPQuestIteration(player, BattlePass.BPQuestType.CasinoBetRoulette, chips);
        BattlePass.updateBPQuestIteration(player, BattlePass.BPQuestType.CasinoPlaceCasinoCoins, chips);
        #endregion

        #endregion

        #region SBPКвест: 5 Оставьте в казино более чем 100.000 фишек.

        #region BattlePass выполнение квеста
        BattlePass.updateBPSuperQuestIteration(player, BattlePass.BPSuperQuestType.CasinoSpendMoreThenCoins, chips);
        #endregion

        #endregion

        RouletteBets[player.GetMyData<int>("RL_TABLE")].Add(new Bet(player, chips, spot));
        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomRight, $"Вы сделали ставку в размере {chips} фишек", 3000);
        //UpdateCasinoChipsGUI(player);
        Trigger.PlayerEvent(player, "bet_roulette", player.GetMyData<int>("RL_TABLE"), spot);

        Dictionary<string, object> dict = new Dictionary<string, object>();

        dict.Add("chips", DiamondCasino.GetAllChips(player));

        Trigger.ClientEvent(player, "CLIENT::CASINO:UPDATE", JsonConvert.SerializeObject(dict));
        //Trigger.PlayerEvent(player, "seat_to_blackjack_table", 1, 0);
      }
      catch (Exception ex) { Log.Write(ex.StackTrace); }
    }

    [RemoteEvent("serverSetRouletteBet")]
    public static void SetBet(Player player, int val)
    {
      try
      {
        if (!player.HasMyData("RL_TABLE"))
          return;

        player.SetMyData("RL_SETBET", val);

        //Trigger.PlayerEvent(player, "client_casino_bet", "close");
        Notify.Send(player, NotifyType.Info, NotifyPosition.BottomRight, $"Вы установили ставку в размере {val} фишек. Что бы изменить ставку, нажмите Ё", 3000);
      }
      catch (Exception ex) { Log.Write(ex.StackTrace); }

    }

    [RemoteEvent("serverChangeRouletteBet")]
    public static void CahngeBet(Player player)
    {
      try
      {
        if (!player.HasMyData("RL_TABLE"))
          return;


        int table = player.GetMyData<int>("RL_TABLE");



        if (RouletteStatus[table] != 0)
          return;

        //Trigger.PlayerEvent(player, "client_casino_bet", "open", JsonConvert.SerializeObject(new List<object>() { RouletteTableBetMin[table], player.GetData<int>("RL_SETBET"), RouletteTableBetMax[table] }));
      }
      catch (Exception ex) { Log.Write(ex.StackTrace); }
    }

    public static void SeatAtRoulette(Player player, int table)
    {
      try
      {
        if (player.HasMyData("RL_TABLE"))
        {
          ExitRoulette(player);
          return;
        }

        Trigger.ClientEvent(player, "client_press_key_to", "close");

        Roulette(player, table);

        Dictionary<string, object> dict = new Dictionary<string, object>();

        dict.Add("show", true);
        dict.Add("activeTab", "Waiting");
        dict.Add("chips", DiamondCasino.GetAllChips(player));
        dict.Add("maxTime", 15);
        dict.Add("time", RouletteBetTime[table]);
        dict.Add("betStatus", Convert.ToBoolean(RouletteStatus[table]));
        dict.Add("bet", new Dictionary<string, object>()
        {
            {"count", RouletteTableBetMin[table] },
            {"max", RouletteTableBetMax[table]},
            {"min", RouletteTableBetMin[table]},
            {"change", RouletteTableBetMin[table]},
        });


        Trigger.ClientEvent(player, "seat_to_roulette_table", table);
        Trigger.ClientEvent(player, "CLIENT::CASINO:OPEN", JsonConvert.SerializeObject(dict), "ROULETTE");
        //Trigger.PlayerEvent(player, "client_casino_bet", "open", JsonConvert.SerializeObject(new List<object>() { RouletteTableBetMin[table], RouletteTableBetMin[table], RouletteTableBetMax[table], DiamondCasino.GetAllChips(player) }));
        //Trigger.PlayerEvent(player, "seat_to_blackjack_table", 1, 0);
      }
      catch (Exception ex) { Log.Write(ex.StackTrace); }
    }
  }
}
