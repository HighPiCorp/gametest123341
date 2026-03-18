using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;
using NeptuneEvo.SDK;

namespace NeptuneEvo.Core
{
    class SafeZones : Script
    {
        private static nLog Log = new nLog("SafeZones");
        public static void CreateSafeZone(Vector3 position, int height, int width)
        {
            var colShape = NAPI.ColShape.CreateCylinderColShape(position, width, height, 0);
            colShape.OnEntityEnterColShape += (shape, player) =>
            {
                try
                {
                    Trigger.ClientEvent(player, "safeZone", true);
                }
                catch (Exception e) { Log.Write($"SafeZoneEnter: {e.StackTrace}", nLog.Type.Error); }
                
            };
            colShape.OnEntityExitColShape += (shape, player) =>
            {
                try
                {
                    Trigger.ClientEvent(player, "safeZone", false);
                }
                catch (Exception e) { Log.Write($"SafeZoneExit: {e.StackTrace}", nLog.Type.Error); }
            };
        }

        [ServerEvent(Event.ResourceStart)]
        public void Event_onResourceStart()
        {
            //CreateSafeZone(new Vector3(445.07443, -983.2143, 32.569595), 70, 150); // полиция
            //CreateSafeZone(new Vector3(307.9767, -1432.9946, 28.846218), 70, 150); // ems safe zone
            //CreateSafeZone(new Vector3(1816.2535, 3678.3647, 33.156406), 70, 150); // ems ss safe zone
            //CreateSafeZone(new Vector3(-544.9742, -204.57057, 37.09586), 70, 200); // мерия
            //CreateSafeZone(new Vector3(136.0578, -761.8408, 44.75204), 70, 100); // fbi
            //CreateSafeZone(new Vector3(-433.6318, 5990.971, 30.69653), 70, 150); // sheriff
            //CreateSafeZone(new Vector3(-1078.168, -254.4076, 43.521), 70, 150); // ls news
            //CreateSafeZone(new Vector3(922.4949, 48.09872, 79.98549), 70, 150); // казик выход
            //CreateSafeZone(new Vector3(1132.3839, 252.03459, -52.15574), 70, 150); // казик центр

            //CreateSafeZone(new Vector3(-712.60284, -1298.5029, 3.9819198), 70, 150); // driving school

            //CreateSafeZone(new Vector3(-471.29346, -975.73987, 22.412676), 70, 150); // stroika ls
            //CreateSafeZone(new Vector3(93.178406, -402.56616, 39.615177), 70, 150); // stroika ls2
            //CreateSafeZone(new Vector3(73.92528, 6545.764, 31.10269), 70, 150); // stroika pb

           
            //CreateSafeZone(new Vector3(-712.2147, -1298.926, 4.101922), 70, 70); // driving school safe zone
        }

        [ServerEvent(Event.PlayerEnterColshape)]
        public static void SetEnterInteractionCheck(ColShape shape, Player player)
        {
            if (!Main.Players.ContainsKey(player)) return;
            if (player.HasMyData("INTERACTIONCHECK") && player.GetMyData<int>("INTERACTIONCHECK") <= 0) return;
            if (player.HasMyData("CUFFED") && player.GetMyData<bool>("CUFFED")) return;
            if (player.HasMyData("IS_DYING") || player.HasMyData("FOLLOWING")) return;

            if (player.HasMyData("GARAGEID"))
            {
                Houses.House house = Houses.HouseManager.GetHouse(player);
                if (house == null) return;
                if (player.GetMyData<int>("GARAGEID") != house.GarageID) return;
            }
            Trigger.ClientEvent(player, "playerInteractionCheck", true);
        }

        [ServerEvent(Event.PlayerExitColshape)]
        public static void SetExitInteractionCheck(ColShape shape, Player player)
        {
            if (!Main.Players.ContainsKey(player)) return;
            Trigger.ClientEvent(player, "playerInteractionCheck", false);
        }
    }
}
