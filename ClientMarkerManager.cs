using System;
using GTANetworkAPI;
using Newtonsoft.Json;
using System.Threading;
using NeptuneEvo.Core;

namespace NeptuneEvo.SDK
{
    class ClientMarkerManager
    {
        static nLog Log = new nLog("ClientMarkerManager", true);

        public static void CreateMarker(Player player, string NAME, int type, Vector3 position, float scale, Color color, bool autoDelete = false, float rangeAutoDelete = 7, uint dimension = UInt32.MinValue)
        {//NAME, [type, [x,y,z], scale, color, dimension]
            try
            {
                NAPI.Task.Run(() => {
                    if (dimension == 0)
                        dimension = player.Dimension;

                    object[] data = { type, new object[] { position.X, position.Y, position.Z }, scale, new object[] { color.Red, color.Green, color.Blue, color.Alpha }, dimension };
                    string json = JsonConvert.SerializeObject(data);
                    Log.Debug(json);
                    Trigger.PlayerEvent(player, "marker", NAME, json);

                    if (player.HasMyData("ClientMarkerColShape_" + NAME))
                    {
                        player.GetMyData<ColShape>("ClientMarkerColShape_" + NAME).Delete();
                    }

                    if (autoDelete)
                    {

                        ColShape shape = NAPI.ColShape.CreateCylinderColShape(position, rangeAutoDelete, 10);
                        shape.OnEntityEnterColShape += (s, p) => {
                            if (p != player) return;
                            DeleteMarker(p, NAME);
                            p.ResetMyData("ClientMarkerColShape_" + NAME);
                            s.Delete();
                        };
                        player.SetMyData("ClientMarkerColShape_" + NAME, shape);
                    }
                        
                });
            }
            catch (Exception e) { Log.Write("CreateMarker: " + e.StackTrace, nLog.Type.Error); }
        }

        public static void DeleteMarker(Player player, string NAME)
        {
            try
            {
                if (player == null || !Main.Players.ContainsKey(player) || NAME == "") return;

                NAPI.Task.Run(() =>
                {
                    try
                    {
                        Log.Debug($"Delete Marker {NAME}");
                        if (player.HasMyData("ClientMarkerColShape_" + NAME))
                        {
                            if (Thread.CurrentThread.Name == "Main")
                                player.GetMyData<ColShape>("ClientMarkerColShape_" + NAME).Delete();
                            else
                            {
                                if (player == null) return;
                                player.GetMyData<ColShape>("ClientMarkerColShape_" + NAME).Delete();
                            }
                        }
                        Trigger.PlayerEvent(player, "unmarker", NAME);
                    }
                    catch (Exception e) { Log.Write("DeleteMarker: " + e.StackTrace, nLog.Type.Error); }
                });
            }
            catch (Exception e) { Log.Write("DeleteMarker: " + e.StackTrace, nLog.Type.Error); }
        }
    }
}
