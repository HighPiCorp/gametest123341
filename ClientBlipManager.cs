using GTANetworkAPI;
using Newtonsoft.Json;
using NeptuneEvo.Core;


namespace NeptuneEvo.SDK
{
    public static class ClientBlipManager
    {
        static nLog Log = new nLog("ClientBlipManager", true);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="player"></param>
        /// <param name="NAME"></param>
        /// <param name="sprite"></param>
        /// <param name="position"></param>
        /// <param name="name"></param>
        /// <param name="color"></param>
        /// <param name="shortRange"></param>
        /// <param name="route"></param>
        /// <param name="autoDelete"></param>
        /// <param name="rangeAutoDelete"></param>
        /// <param name="dimension"></param>
        public static void CreateBlip(Player player, string NAME, int sprite, Vector3 position, string name, int color, bool shortRange, bool route, bool autoDelete = true, float rangeAutoDelete = 7, int dimension = -1, bool isInNavigator = false)
        {
            CreateBlip(player, NAME, sprite, position, name, 1, color, 255, shortRange, 0, dimension, 0, route, autoDelete, rangeAutoDelete, isInNavigator);
        }

        /// <summary>
        /// Отображения локаций (война за територию, зоны)
        /// </summary>
        /// <param name="player"></param>
        /// <param name="NAME"></param>
        /// <param name="sprite">5 - square 9 - circle</param>
        /// <param name="position"></param>
        /// <param name="color"></param>
        /// <param name="alpha"></param>
        /// <param name="rotation"></param>
        /// <param name="radius"></param>
        /// <param name="dimension"></param>
        public static void CreateBlip(Player player, string NAME, int sprite, Vector3 position, int color, int alpha, float rotation, float radius, int dimension = -1)
        {
            CreateBlip(player, NAME, sprite, position, "", 1, color, alpha, true, rotation, dimension, radius, false, false);
        }
        private static void CreateBlip(Player player, string NAME, int sprite, Vector3 position, string name, float scale, int color, int alpha, bool shortRange, float rotation, int dimension, float radius, bool route, bool autoDelete = true, float rangeAutoDelete = 7, bool isInNavigator = false)
        {
            try
            {
                NAPI.Task.Run(() => {
                    if (dimension == -1)
                        dimension = (int)player.Dimension;

                    object[] data = { sprite, new object[] { position.X, position.Y, position.Z }, name, scale, color, alpha, shortRange, rotation, dimension, radius, route };
                    string json = JsonConvert.SerializeObject(data);
                    //Log.Debug(json);
                    Trigger.PlayerEvent(player, "router", NAME, json);

                    if (player.HasMyData("ClientBlipColShape_" + NAME))
                    {
                        player.GetMyData<ColShape>("ClientBlipColShape_" + NAME).Delete();
                    }

                    if (isInNavigator)
                    {
                        player.SetMyData("GPS_ACTIVE", position);
                    }

                    if (autoDelete)
                    {
                        ColShape shape = NAPI.ColShape.CreateCylinderColShape(position, rangeAutoDelete, 10);
                        shape.OnEntityEnterColShape += (s, p) => {
                            if (p != player) return;
                            DeleteBlip(p, NAME, isInNavigator);
                            p.ResetMyData("ClientBlipColShape_" + NAME);
                            s.Delete();
                        };
                        player.SetMyData("ClientBlipColShape_" + NAME, shape);
                    }
                       
                });
                }
            catch (Exception e) { Log.Write("CreateBlip: " + e.StackTrace, nLog.Type.Error); }
        }

        public static void DeleteBlip(Player player, string NAME, bool isInNavigator = false)
        {
            try
            {
                if (player == null) return;
                if (!Main.Players.ContainsKey(player)) return;

                NAPI.Task.Run(() => {
                    Log.Debug($"Delete Blip {NAME}");
                    if (isInNavigator)
                    {
                        player.ResetMyData("GPS_ACTIVE");
                    }

                    if (player.HasMyData("ClientBlipColShape_" + NAME))
                    {
                        if (Thread.CurrentThread.Name == "Main")
                        {
                            player.GetMyData<ColShape>("ClientBlipColShape_" + NAME).Delete();
                            player.ResetMyData("ClientBlipColShape_" + NAME);
                        }
                        else { 
                            if (player == null) return;
                            player.GetMyData<ColShape>("ClientBlipColShape_" + NAME).Delete();
                            player.ResetMyData("ClientBlipColShape_" + NAME);
                        }
                    }
                    Trigger.PlayerEvent(player, "unrouter", NAME);
                });
            }
            catch (Exception e) { Log.Write("DeleteBlip: " + e.StackTrace, nLog.Type.Error); }
        }
    }
}
