using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using client.Systems.BattlePass;
using GTANetworkAPI;
using MySqlConnector;
using NeptuneEvo;
using NeptuneEvo.Core;
using NeptuneEvo.SDK;
using Newtonsoft.Json;
using Trigger = NeptuneEvo.SDK.Trigger;

namespace client.Core.Casino
{
  class CasinoSlots : Script
  {
    private static nLog Log = new nLog("Slots");

    public static List<Vector3> SlotsMachines = new List<Vector3>()
    {
        new Vector3(1100.483, 230.4082, -50.8409),
        new Vector3(1100.939, 231.0017, -50.8409),
        new Vector3(1101.221, 231.6943, -50.8409),
        new Vector3(1101.323, 232.4321, -50.8409),
        new Vector3(1101.229, 233.1719, -50.8409),
        new Vector3(1108.938, 239.4797, -50.8409),
        new Vector3(1109.536, 239.0278, -50.8409),
        new Vector3(1110.229, 238.7428, -50.8409),
        new Vector3(1110.974, 238.642, -50.8409),
        new Vector3(1111.716, 238.7384, -50.8409),
        new Vector3(1112.407, 239.0216, -50.8409 ),
        new Vector3( 1112.999, 239.4742, -50.8409 ),
        new Vector3( 1120.853, 233.1621, -50.8409 ),
        new Vector3( 1120.753, 232.4272, -50.8409 ),
        new Vector3( 1120.853, 231.6886, -50.8409 ),
        new Vector3( 1121.135, 230.9999, -50.8409 ),
        new Vector3( 1121.592, 230.4106, -50.8409 ),
        new Vector3( 1104.572, 229.4451, -50.8409 ),
        new Vector3( 1104.302, 230.3183, -50.8409 ),
        new Vector3(  1105.049, 230.845, -50.8409 ),
        new Vector3(  1105.781, 230.2973, -50.8409 ),
        new Vector3( 1105.486, 229.4322, -50.8409),
        new Vector3( 1108.005, 233.9177, -50.8409),
        new Vector3(  1107.735, 234.7909, -50.8409),
        new Vector3(1108.482, 235.3176, -50.8409),
        new Vector3(1109.214, 234.7699, -50.8409),
        new Vector3( 1108.919, 233.9048, -50.8409),
        new Vector3( 1113.64, 233.6755, -50.8409),
        new Vector3(1113.37, 234.5486, -50.8409),
        new Vector3(1114.117, 235.0753, -50.8409),
        new Vector3(1114.848, 234.5277, -50.8409),
        new Vector3(1114.554, 233.6625, -50.8409),
        new Vector3(1116.662, 228.8896, -50.8409),
        new Vector3(1116.392, 229.7628, -50.8409),
        new Vector3(1117.139, 230.2895, -50.8409),
        new Vector3(1117.871, 229.7419, -50.8409),
        new Vector3(1117.576, 228.8767, -50.8409),
        new Vector3( 1129.64, 250.451, -52.0409),
        new Vector3( 1130.376, 250.3577, -52.0409),
        new Vector3(1131.062, 250.0776, -52.0409),
        new Vector3(1131.655, 249.6264, -52.0409),
        new Vector3(1132.109, 249.0355, -52.0409),
        new Vector3(1132.396, 248.3382, -52.0409),
        new Vector3(1132.492, 247.5984, -52.0409),
        new Vector3(1133.952, 256.1037, -52.0409),
        new Vector3(1133.827, 256.9098, -52.0409),
        new Vector3(1134.556, 257.2778, -52.0409),
        new Vector3(1135.132, 256.699, -52.0409),
        new Vector3(1134.759, 255.9734, -52.0409),
        new Vector3(1138.195, 251.8611, -52.0409),
        new Vector3(1138.07, 252.6677, -52.0409),
        new Vector3(1138.799, 253.0363, -52.0409),
        new Vector3(1139.372, 252.4563, -52.0409),
        new Vector3(1139, 251.7306, -52.0409),
    };

    public static List<bool> SlotsStart = new List<bool>();

    public static List<List<int>> SlotsBets = new List<List<int>>();

    public static List<Dictionary<int, int>> SlotsPrizeData = new List<Dictionary<int, int>>();


    [ServerEvent(Event.ResourceStart)]
    public static void OnResourceStart()
    {
      try
      {

        var result = MySQL.QueryRead($"SELECT * FROM `slots`");
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
            var data = JsonConvert.DeserializeObject<Dictionary<int, int>>(Row["data"].ToString());
            SlotsBets.Add(new List<int>() { minbet, maxbet });

            SlotsPrizeData.Add(data);

          }
          catch (Exception e)
          {
            Log.Write(Row["id"].ToString() + e.ToString(), nLog.Type.Error);
          }

        }
      }
      catch
      {

      }

      int i = 0;

      foreach (Vector3 pos in SlotsMachines)
      {
        //NAPI.TextLabel.CreateTextLabel($"#{i}", pos.Add(new Vector3(0, 0, 2.8)), 20, 1.0f, 1, new Color(255, 255, 255), true, 0); // todo удалить
        var shape = NAPI.ColShape.CreateCylinderColShape(pos, 1f, 2f);
        shape.SetData("SLOT", i);
        shape.OnEntityEnterColShape += (shape, entity) => {
          entity.SetMyData("INTERACTIONCHECK", 1040);
          entity.SetMyData("SLOT", shape.GetData<int>("SLOT"));
          Trigger.PlayerEvent(entity, "client_press_key_to", "open", JsonConvert.SerializeObject(new List<object>() { "E", "сесть за слот машину" }));
        };
        shape.OnEntityExitColShape += (shape, entity) => {
          entity.SetMyData("INTERACTIONCHECK", 0);
          entity.ResetMyData("SLOT");
          Trigger.PlayerEvent(entity, "client_press_key_to", "close");
        };
        i++;
        SlotsStart.Add(false);
      }
    }

    public static void OpenSlot(Player player)
    {
      try
      {
        int slot = player.GetMyData<int>("SLOT");

        if (SlotsStart[slot])
          return;

        player.SetMyData("ON_SLOT", true);

        SlotsStart[slot] = true;

        Trigger.PlayerEvent(player, "client_press_key_to", "close");
        //Trigger.PlayerEvent(player, "client_slots_bet", "open", JsonConvert.SerializeObject(new List<int>() { SlotsBets[slot][0], SlotsBets[slot][0], SlotsBets[slot][1], DiamondCasino.GetAllChips(player) }));

        Dictionary<string, object> dict = new Dictionary<string, object>();

        dict.Add("show", true);
        dict.Add("activeTab", "Bet");
        dict.Add("bet", new Dictionary<string, object>()
        {
            {"count", SlotsBets[slot][0] },
            {"max", SlotsBets[slot][1] },
            {"min", SlotsBets[slot][0] },
            {"change", SlotsBets[slot][0]  }
        });

        dict.Add("chips", DiamondCasino.GetAllChips(player));
        dict.Add("money", Main.Players[player].Money);

        Trigger.ClientEvent(player, "CLIENT::CASINO:OPEN", JsonConvert.SerializeObject(dict), "SLOTS");

        Trigger.PlayerEvent(player, "casino_start_slot", slot);
      }
      catch (Exception ex) { Log.Write(ex.StackTrace); }
    }

    [RemoteEvent("SERVER::SLOTS:EXIT")]
    public static void ExitSlot(Player player)
    {
      try
      {
        int slot = player.GetMyData<int>("SLOT");

        SlotsStart[slot] = false;

        player.ResetMyData("ON_SLOT");

        Trigger.PlayerEvent(player, "casino_exit_slot");
        Trigger.ClientEvent(player, "CLIENT::casino:hidef");
      }
      catch (Exception ex) { Log.Write(ex.StackTrace); }
    }

    [RemoteEvent("SERVER::SLOTS:RUN")]
    public static void StartSlot(Player player, int chips)
    {
      try
      {
        if (!player.HasMyData("SLOT"))
          return;

        if (player.HasMyData("SLOT_STARTED"))
          return;

        var chip = nInventory.Find(Main.Players[player].UUID, ItemType.CasinoChips);

        if (chip == null)
        {
          Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "У вас нехватает фишек", 3000);
          return;
        }

        if (chip.Count < chips)
        {
          Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "У вас нехватает фишек", 3000);
          return;
        }

        int slot = player.GetMyData<int>("SLOT");

        nInventory.Remove(player, ItemType.CasinoChips, chips);

        #region BPКвест: 18 Сделайте ставок на 10.000 фишек в слотах. / 23 Оставьте в казино более чем 100.000 фишек.

        #region BattlePass выполнение квеста
        BattlePass.updateBPQuestIteration(player, BattlePass.BPQuestType.CasinoBetSlots, chips);
        BattlePass.updateBPQuestIteration(player, BattlePass.BPQuestType.CasinoPlaceCasinoCoins, chips);
        #endregion

        #endregion

        #region SBPКвест: 5 Оставьте в казино более чем 100.000 фишек.

        #region BattlePass выполнение квеста
        BattlePass.updateBPSuperQuestIteration(player, BattlePass.BPSuperQuestType.CasinoSpendMoreThenCoins, chips);
        #endregion

        #endregion

        player.SetMyData("SLOT_BET", chips);

        Random rand = new Random();
        int val = rand.Next(-150, 150);

        int win = -1;

        if (SlotsPrizeData[slot].ContainsKey(val))
        {
          win = val;
        }

        player.SetMyData("SLOT_STARTED", true);

        // Trigger.PlayerEvent(player, "updateSlotsChips", DiamondCasino.GetAllChips(player));
        DiamondCasino.UpdateChipsInfo(player);
        Trigger.PlayerEvent(player, "casino_spin_slot", win);
      }
      catch (Exception ex) { Log.Write(ex.StackTrace); }
    }

    [RemoteEvent("casino_stop_slot")]
    public static void StopSlot(Player player, int win)
    {
      try
      {
        int slot = player.GetMyData<int>("SLOT");

        if (SlotsPrizeData[slot].ContainsKey(win))
        {
          int chips = player.GetMyData<int>("SLOT_BET");

          nInventory.Add(player, new nItem(ItemType.CasinoChips, chips * SlotsPrizeData[slot][win]));

          DiamondCasino.UpdateChipsInfo(player);

          Notify.Send(player, NotifyType.Success, NotifyPosition.TopCenter, $"Вы выиграли {chips * SlotsPrizeData[slot][win]}!", 3000);

          Trigger.PlayerEvent(player, "updateSlotsChips", DiamondCasino.GetAllChips(player));
          Trigger.ClientEvent(player, "slots_win", true);

        }
        else
        {
          Trigger.ClientEvent(player, "slots_win", false);
          //Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Вы проиграли!", 3000);
        }

        player.ResetMyData("SLOT_STARTED");
      }
      catch (Exception ex) { Log.Write(ex.StackTrace); }
    }

    [Command("slotmnoj", Hide = true)]
    public static void EditSlotsPrize(Player player, int slot, int num, int mnoj)
    {
      try
      {
        if (!Group.CanUseCmd(player, "save")) return;

        if (SlotsPrizeData.Count <= slot) return;

        if (SlotsPrizeData[slot].Count <= num) return;

        KeyValuePair<int, int> pair = SlotsPrizeData[slot].ElementAt(num);

        SlotsPrizeData[slot][pair.Key] = mnoj;

        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы изменили множитель выпадения #{num} слота {slot} на X{mnoj}");

        //MySQL.Query($"UPDATE `slots` SET `data` = '{JsonConvert.SerializeObject(SlotsPrizeData[slot])}' WHERE `id` = {slot}");

        MySqlCommand cmd = new MySqlCommand();
        cmd.CommandText = "UPDATE `slots` SET `data`=@data WHERE `id`=@id";

        cmd.Parameters.AddWithValue("@data", JsonConvert.SerializeObject(SlotsPrizeData[slot]));
        cmd.Parameters.AddWithValue("@id", slot);
        MySQL.Query(cmd);
      }
      catch (Exception ex) { Log.Write(ex.StackTrace); }
    }

    [Command("slotbet", Hide = true)]
    public static void EditSlotsBet(Player player, int slot, int min, int max)
    {
      try
      {
        if (!Group.CanUseCmd(player, "save")) return;

        if (SlotsBets.Count <= slot) return;

        SlotsBets[slot][0] = min;
        SlotsBets[slot][1] = max;

        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы изменили ставки слота {slot} на MIN:{min} MAX:{max}");

        //MySQL.Query($"UPDATE `slots` SET `minbet` = {min}, `maxbet` = {max} WHERE `id` = {slot}");

        MySqlCommand cmd = new MySqlCommand();
        cmd.CommandText = "UPDATE `slots` SET `minbet`=@minbet, `maxbet`=@maxbet WHERE `id`=@id";

        cmd.Parameters.AddWithValue("@minbet", min);
        cmd.Parameters.AddWithValue("@maxbet", max);
        cmd.Parameters.AddWithValue("@id", slot);
        MySQL.Query(cmd);
      }
      catch (Exception ex) { Log.Write(ex.StackTrace); }
    }

    [ServerEvent(Event.PlayerDisconnected)]
    public static void onPlayerDisconnectedhandler(Player player, DisconnectionType type, string reason)
    {
      try
      {
        if (player.HasMyData("ON_SLOT"))
        {
          ExitSlot(player);
          return;
        }
      }
      catch (Exception e)
      {
        Log.Write("onPlayerDisconnectedhandler: " + e.StackTrace + "\n" + e.StackTrace, nLog.Type.Error);
      }
    }

  }


}
