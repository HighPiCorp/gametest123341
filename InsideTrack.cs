using System;
using System.Collections.Generic;
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
  class InsideTrack : Script
  {
    private static nLog Log = new nLog("InsideTrack");

    public static Vector3 ScreenPosition = new Vector3(1092.75, 264.56, -51.24);

    public static int Timer = 0;
    public static bool WaitBets = false;
    public static bool IsStartRace = false;
    public static int LastHorse = -1;
    public static int ShowHorse = -1;

    public static int WinnerHorse = -1;
    public static int LastWinnerHorse = -1;

    public static List<int> Horses = new List<int>() { 1, 2, 3, 4, 5, 6 };
    public static List<int> StyleHorses = new List<int>() { 1, 2, 3, 4, 5, 6 };

    public static List<ITBet> Bets = new List<ITBet>() { };

    public static List<Vector3> Seats = new List<Vector3>() {
            new Vector3(1091.418, 257.544, -52.2409),
            new Vector3(1092.072, 258.1984, -52.2409),
            new Vector3(1094.473, 260.5995, -52.2409),
            new Vector3(1095.127, 261.254, -52.2409),
            new Vector3(1095.852, 261.978, -52.2409),
            new Vector3(1096.506, 262.6327, -52.2409),
            new Vector3(1098.902, 265.0287, -52.2409),
            new Vector3(1099.556, 265.683, -52.2409),
            new Vector3(1092.974, 255.211, -52.2409f),
            new Vector3(1093.628, 255.865, -52.2409),
            new Vector3(1096.417, 258.6552, -52.2409),
            new Vector3(1097.072, 259.309, -52.2409),
            new Vector3(1097.797, 260.034, -52.2409),
            new Vector3(1098.451, 260.6878, -52.2409),
            new Vector3(1101.236, 263.473, -52.2409),
            new Vector3(1101.89, 264.127, -52.2409),
        };

    public static List<bool> CheckSeats = new List<bool>();

    [ServerEvent(Event.ResourceStart)]
    public static void OnResourceStart()
    {
      try
      {
        Bets.Clear();

        int i = 0;
        foreach (Vector3 pos in Seats)
        {
          var sp = NAPI.ColShape.CreateCylinderColShape(pos, 1f, 2f);
          sp.SetData("NUM", i);
          sp.OnEntityEnterColShape += (shape, player) => {
            player.SetMyData("INTERACTIONCHECK", 1050);
            player.SetMyData("COMP_NUM", shape.GetData<int>("NUM"));

            if (!player.HasMyData("OPEN_IT"))
            {
              if (player.HasMyData("IT_SEAT"))
              {
                Trigger.PlayerEvent(player, "client_press_key_to", "open", JsonConvert.SerializeObject(new List<object>() { "SPACE", "открыть меню ставок" }));
              }
              else
              {
                Trigger.PlayerEvent(player, "client_press_key_to", "open", JsonConvert.SerializeObject(new List<object>() { "E", "сесть за стул" }));
              }
            }
          };
          sp.OnEntityExitColShape += (shape, player) => {
            player.SetMyData("INTERACTIONCHECK", 0);
            player.ResetMyData("COMP_NUM");
            Trigger.PlayerEvent(player, "client_press_key_to", "close");
          };
          i++;

          CheckSeats.Add(false);
        }

        var shape = NAPI.ColShape.CreateCylinderColShape(ScreenPosition, 25f, 10f);
        shape.OnEntityEnterColShape += (shape, player) => {
          ShowMainScreen(player);
        };
        shape.OnEntityExitColShape += (shape, player) => {
          HideMainScreen(player);
        };

        Thread thd = new Thread(StartRaceInsideTrack);
        thd.Start();

        //GetRandomHorse();
      }
      catch (Exception ex) { Log.Write(ex.StackTrace.ToString()); }
    }


    [RemoteEvent("openInsideTrack")]
    public static void OpenInsideTrack(Player player)
    {
      try
      {
        Trigger.PlayerEvent(player, "client_press_key_to", "close");
        player.SetMyData("OPEN_IT", true);
      }
      catch (Exception ex) { Log.Write(ex.StackTrace.ToString()); }
    }

    [RemoteEvent("hideInsideTrack")]
    public static void HideInsideTrack(Player player)
    {
      try
      {
        Trigger.PlayerEvent(player, "client_press_key_to", "open", JsonConvert.SerializeObject(new List<object>() { "SPACE", "открыть меню ставок" }));
        player.ResetMyData("OPEN_IT");
      }
      catch (Exception ex) { Log.Write(ex.StackTrace.ToString()); }
    }

    public static void SeatAtTable(Player player)
    {
      try
      {
        int seat = player.GetMyData<int>("COMP_NUM");

        if (CheckSeats[seat])
        {
          return;
        }

        player.SetMyData("IT_SEAT", true);
        player.SetMyData("SEAT", seat);

        CheckSeats[seat] = true;

        // Trigger.PlayerEvent(player, "client_press_key_to", "close");
        Trigger.PlayerEvent(player, "setHorses", JsonConvert.SerializeObject(StyleHorses));
        Trigger.PlayerEvent(player, "client_press_key_to", "open", JsonConvert.SerializeObject(new List<object>() { "SPACE", "открыть меню ставок" }));
        Trigger.PlayerEventInRange(player.Position, 30f, "seatAtComp", player.Handle, Seats[seat].X, Seats[seat].Y, Seats[seat].Z);
      }
      catch (Exception ex) { Log.Write(ex.StackTrace.ToString()); }
    }

    public static void EixtTable(Player player)
    {
      try
      {
        if (player.HasMyData("OPEN_IT"))
          return;

        player.ResetMyData("IT_SEAT");

        int seat = player.GetData<int>("SEAT");

        CheckSeats[seat] = false;

        player.ResetMyData("SEAT");

        Trigger.PlayerEventInRange(player.Position, 30f, "exitComp", player.Handle);

        Trigger.PlayerEvent(player, "client_press_key_to", "close");
      }
      catch (Exception ex) { Log.Write(ex.StackTrace.ToString()); }
    }

    public static void StartRaceInsideTrack()
    {
      try
      {
        while (true)
        {
          Timer = 300;
          WaitBets = true;
          WinnerHorse = -1;
          ShowHorse = -1;
          LastHorse = -1;

          StyleHorses = GetRandomArr(99);

          SetHorses();

          ShowAllHorses();

          while (Timer > 0)
          {
            Timer--;

            if (Timer % 10 == 0)
            {
              if (ShowHorse == -1)
              {
                if (LastHorse == -1)
                {
                  ShowHorse = 1;
                  LastHorse = 1;
                  ShowHorseBigScreen(1);
                }
                else if (LastHorse >= 6)
                {
                  ShowHorse = 1;
                  LastHorse = 1;
                  ShowHorseBigScreen(1);
                }
                else
                {
                  LastHorse += 1;
                  ShowHorse = LastHorse;
                  ShowHorseBigScreen(LastHorse);
                }
              }
              else
              {
                ShowHorse = -1;
                ShowAllHorses();
              }
            }

            UpdateCountdown();
            Thread.Sleep(1000);

          }

          WinnerHorse = GetRandomHorse();
          LastWinnerHorse = WinnerHorse;

          StartRace();
          IsStartRace = true;

          Timer = 55;

          while (Timer > 0)
          {
            Timer--;
            Thread.Sleep(1000);
          }

          IsStartRace = false;

          Payout();

          ClearPlayers();

        }
      }
      catch (Exception ex) { Log.Write(ex.StackTrace.ToString()); }
    }


    public static void UpdateCountdown()
    {
      try
      {
        Trigger.PlayerEventInRange(ScreenPosition, 30f, "updateCountdown", Timer);
      }
      catch (Exception ex) { Log.Write(ex.StackTrace.ToString()); }
    }

    public static int GetRandomHorse()
    {
      try
      {
        Shuffle(Horses);

        while (Horses[0] == LastWinnerHorse)
        {
          Shuffle(Horses);
        }

        return Horses[0];
      }
      catch (Exception ex) { Log.Write(ex.StackTrace.ToString()); return Horses[0]; }
    }

    public static void Payout()
    {
      try
      {
        foreach (ITBet bet in Bets)
        {
          if (bet.Horse != WinnerHorse)
          {
            try
            {
              Notify.Send(bet.Pl, NotifyType.Info, NotifyPosition.BottomCenter, $"Ваша ставка проиграла", 3000);
            }
            catch (Exception e) { Log.Write(e.StackTrace, nLog.Type.Error); }
            continue;
          }

          try
          {
            nInventory.Add(bet.Pl, new nItem(ItemType.CasinoChips, bet.Payout));

            #region BPКвест: 95 Выиграть в крупном заезде Inside Track.

            #region BattlePass выполнение квеста
            BattlePass.updateBPQuestIteration(bet.Pl, BattlePass.BPQuestType.CasinoWinInBigEventInsideTruck);
            #endregion

            #endregion

            Notify.Send(bet.Pl, NotifyType.Success, NotifyPosition.BottomCenter, $"Ваша лошадка победила, выигрыш {bet.Payout}", 3000);

            UpdateBalance(bet.Pl);
          }
          catch (Exception e) { Log.Write(e.StackTrace, nLog.Type.Error); }
        }
      }
      catch (Exception ex) { Log.Write(ex.StackTrace.ToString()); }
    }

    public static int GetAllChips(Player player)
    {
      try
      {
        if (!Main.Players.ContainsKey(player))
          return 0;

        var item = nInventory.Find(Main.Players[player].UUID, ItemType.CasinoChips);

        int count = 0;

        if (item != null)
        {
          count = item.Count;
        }

        return count;
      }
      catch (Exception ex) { Log.Write(ex.StackTrace.ToString()); return 0; }
    }


    [RemoteEvent("addbet")]
    public static void AddBet(Player player, int horse, int bet)
    {
      try
      {
        if (GetAllChips(player) < bet)
        {
          Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У вас нехватает фишек", 3000);
          return;
        }

        nInventory.Remove(player, ItemType.CasinoChips, bet);

        #region BPКвест: 19 Сделайте ставок на 5.000 фишек в Inside Track. / 23 Оставьте в казино более чем 100.000 фишек. / 94 Принять участие в крупном заезде Inside Track.

        #region BattlePass выполнение квеста
        BattlePass.updateBPQuestIteration(player, BattlePass.BPQuestType.CasinoBetInsideTrack, bet);
        BattlePass.updateBPQuestIteration(player, BattlePass.BPQuestType.CasinoPlaceCasinoCoins, bet);
        BattlePass.updateBPQuestIteration(player, BattlePass.BPQuestType.CasinoEnterInBigEventInsideTruck);
        #endregion

        #endregion

        #region SBPКвест: 5 Оставьте в казино более чем 100.000 фишек.

        #region BattlePass выполнение квеста
        BattlePass.updateBPSuperQuestIteration(player, BattlePass.BPSuperQuestType.CasinoSpendMoreThenCoins, bet);
        #endregion

        #endregion

        Bets.Add(new ITBet(player, horse, bet, bet * 2));


        Trigger.PlayerEventInRange(ScreenPosition, 30f, "addBet", player.Name, horse, bet);
        Trigger.PlayerEvent(player, "setbet", true);

        UpdateBalance(player);

        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Ставка принята на лошадку #{horse}", 3000);
      }
      catch (Exception ex) { Log.Write(ex.StackTrace.ToString()); }
    }

    [RemoteEvent("winnerBet")]
    public static void WinnerBet(Player player, int bet)
    {
      try
      {
        nInventory.Add(player, new nItem(ItemType.CasinoChips, bet * 2));

        UpdateBalance(player);

        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Ваша лошадка победила. Выигрыш {bet * 2}", 3000);

        //player.SendChatMessage($"Ваша лошадка победила. Выигрыш {bet * 2}");
      }
      catch (Exception ex) { Log.Write(ex.StackTrace.ToString()); }
    }

    [RemoteEvent("trySingleBet")]
    public static void TryBet(Player player, int bet)
    {
      try
      {
        if (GetAllChips(player) < bet)
        {
          Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У вас нехватает фишек", 3000);
          return;
        }

        nInventory.Remove(player, ItemType.CasinoChips, bet);

        UpdateBalance(player);

        #region BPКвест: 19 Сделайте ставок на 5.000 фишек в Inside Track. / 23 Оставьте в казино более чем 100.000 фишек.

        #region BattlePass выполнение квеста
        BattlePass.updateBPQuestIteration(player, BattlePass.BPQuestType.CasinoBetInsideTrack, bet);
        BattlePass.updateBPQuestIteration(player, BattlePass.BPQuestType.CasinoPlaceCasinoCoins, bet);
        #endregion

        #endregion

        #region SBPКвест: 5 Оставьте в казино более чем 100.000 фишек.

        #region BattlePass выполнение квеста
        BattlePass.updateBPSuperQuestIteration(player, BattlePass.BPSuperQuestType.CasinoSpendMoreThenCoins, bet);
        #endregion

        #endregion

        Trigger.PlayerEvent(player, "startSingleRace");

        //player.SendChatMessage($"Ваша лошадка победила. Выигрыш {bet * 2}");
      }
      catch (Exception ex) { Log.Write(ex.StackTrace.ToString()); }
    }


    public static void ShowHorseBigScreen(int num)
    {
      try
      {
        Trigger.PlayerEventInRange(ScreenPosition, 30f, "showHorse", num);
      }
      catch (Exception ex) { Log.Write(ex.StackTrace.ToString()); }
    }

    public static void UpdateBalance(Player player)
    {
      try
      {
        Trigger.PlayerEvent(player, "updateBalance", GetAllChips(player));
      }
      catch (Exception ex) { Log.Write(ex.StackTrace.ToString()); }
    }

    public static void ClearPlayers()
    {
      try
      {
        Bets.Clear();
        Trigger.PlayerEventInRange(ScreenPosition, 30f, "clearPlayers");
        Trigger.PlayerEventInRange(ScreenPosition, 30f, "setbet", false);
      }
      catch (Exception ex) { Log.Write(ex.StackTrace.ToString()); }
    }


    public static void SetHorses()
    {
      try
      {
        Trigger.PlayerEventInRange(ScreenPosition, 30f, "setHorses", JsonConvert.SerializeObject(StyleHorses));
      }
      catch (Exception ex) { Log.Write(ex.StackTrace.ToString()); }
    }

    public static void ShowAllHorses()
    {
      try
      {
        Trigger.PlayerEventInRange(ScreenPosition, 30f, "showMain");
      }
      catch (Exception ex) { Log.Write(ex.StackTrace.ToString()); }
    }

    public static void StartRace()
    {
      try
      {
        Trigger.PlayerEventInRange(ScreenPosition, 30f, "startRace", JsonConvert.SerializeObject(Horses));
      }
      catch (Exception ex) { Log.Write(ex.StackTrace.ToString()); }
    }

    public static void ShowMainScreen(Player player)
    {
      try
      {
        if (IsStartRace)
        {

          Trigger.PlayerEvent(player, "showMain");
          Trigger.PlayerEvent(player, "setHorses", JsonConvert.SerializeObject(StyleHorses));
          Trigger.PlayerEvent(player, "setMainEvent", true);
        }
        else
        {
          Trigger.PlayerEvent(player, "setHorses", JsonConvert.SerializeObject(StyleHorses));

          Trigger.PlayerEvent(player, "addBetsInside", JsonConvert.SerializeObject(Bets));

          if (ShowHorse != -1)
          {
            Trigger.PlayerEvent(player, "showHorse", ShowHorse);
          }

        }

        UpdateBalance(player);
      }
      catch (Exception ex) { Log.Write(ex.StackTrace.ToString()); }
    }

    public static void HideMainScreen(Player player)
    {
      try
      {
        Trigger.PlayerEvent(player, "clearPlayers");
      }
      catch (Exception ex) { Log.Write(ex.StackTrace.ToString()); }
    }

    public static void Shuffle(List<int> list)
    {
      try
      {
        Random rng = new Random();
        int n = list.Count;
        while (n > 1)
        {
          n--;
          int k = rng.Next(n + 1);
          int value = list[k];
          list[k] = list[n];
          list[n] = value;
        }
      }
      catch (Exception ex) { Log.Write(ex.StackTrace.ToString()); }
    }


    public static List<int> GetRandomArr(int max)
    {
      try
      {
        Random rand = new Random();

        List<int> arr = new List<int>();

        for (int i = 0; i < 6; i++)
        {
          int val = rand.Next(1, max);

          while (arr.Contains(val))
            val = rand.Next(1, max);

          arr.Add(val);
        }

        return arr;
      }
      catch (Exception ex) { Log.Write(ex.StackTrace.ToString()); return new List<int>(); }
    }

    [ServerEvent(Event.PlayerDisconnected)]
    public static void onPlayerDisconnectedhandler(Player player, DisconnectionType type, string reason)
    {
      try
      {
        if (player.HasMyData("IT_SEAT"))
        {
          InsideTrack.EixtTable(player);
          return;
        }
      }
      catch (Exception e)
      {
        Log.Write("onPlayerDisconnectedhandler: " + e.StackTrace + "\n" + e.StackTrace, nLog.Type.Error);
      }
    }
  }

  class ITBet
  {
    public int Horse = -1;
    public int BetSize = 0;
    public int Payout = 0;
    public string Name;

    [JsonIgnore]
    public Player Pl = null;

    public ITBet(Player player, int horse, int bet, int payout)
    {
      Horse = horse;
      BetSize = bet;
      Payout = payout;
      Pl = player;
      Name = player.Name;
    }
  }
}
