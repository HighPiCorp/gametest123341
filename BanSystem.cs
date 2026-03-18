using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Data;
using NeptuneEvo.GUI;
using NeptuneEvo.SDK;
using client.Fractions.Utils;
using NeptuneEvo.Jobs;

namespace NeptuneEvo.Core
{
  class BanSystem : Script
  {
      private static nLog Log = new nLog("BanSystem");

      [ServerEvent(Event.PlayerConnected)]
      public void OnPlayerConnected(Player player)
      {
        try
        {
          Trigger.ClientEvent(player, "CLIENT::ban:checkStorage", player.Value);
        }
        catch (Exception e) { Log.Write("storageBanPlayer: " + e.StackTrace, nLog.Type.Error); }
      }

      public static void storageBanPlayer(Player player, Player target)
      {
        try
        {
          if (!Group.CanUseCmd(player, "stban")) return;
          if (player == target) return;

          Trigger.ClientEvent(target, "CLIENT::ban:addInStorage", target.Value);
        }
        catch (Exception e) { Log.Write("storageBanPlayer: " + e.StackTrace, nLog.Type.Error); }
      }

      [RemoteEvent("SERVER::ban:kickPlayer")]
      public void kickBannedPlayer(Player player)
      {
        try
        {
          Commands.SendToAdmins(3, $"!{{#d35400}}[BANNED-DETECTED] Попытался зайти забаненный игрок -> {player.Name} ({player.Value}), но был кикнут");
          player.Kick("KICK");
        } catch(Exception e) { Log.Write("storageBanPlayer: " + e.StackTrace, nLog.Type.Error); }
      }

      public static void unstorageBanPlayer(Player player, Player target)
      {
        if (!Group.CanUseCmd(player, "unstban")) return;
        if (player == target) return;

        Trigger.ClientEvent(target, "CLIENT::ban:removeFromStorage");
      }

      public static int getMinutes(int time, string typeTime)
      {
        var resultTime = 0;
        switch(typeTime) {
          case "m":
            resultTime = time;
            break;
          case "h":
            resultTime = time * 60;
            break;
          case "d":
            resultTime = time * 60 * 24;
            break;
          default:
            resultTime = time * 60 * 24;
            break;
        }

        return resultTime;
      }
  }
}
