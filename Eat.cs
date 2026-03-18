using GTANetworkAPI;
using System;
using System.Linq;
using NeptuneEvo.SDK;
using MySqlConnector;
using client.Systems.BattlePass;
using System.Net.Http.Headers;

namespace NeptuneEvo.Core
{
    class EatManager : Script
    {

        private static nLog Log = new nLog("EatManager");

        public class ObjectEatWaterHealth
        {
            public ObjectEatWaterHealth(int eat, int water, int health) {
                Eat = eat;
                Water = water;
                Health = health;
            }

            public int Eat { get; set; }
            public int Water { get; set; }
            public int Health { get; set; }
        }

        private const int EatDecay = -2;
        private const int EatDecayInVehicle = -5;

        private const int WaterDecay = -2;
        private const int WaterDecayInVehicle = -3;

        public static Dictionary<ItemType, ObjectEatWaterHealth> ConsumeItemEatAndWater = new Dictionary<ItemType, ObjectEatWaterHealth>()
        {
            {ItemType.Сrisps, new ObjectEatWaterHealth(15, -10, 0)},
            {ItemType.Beer, new ObjectEatWaterHealth(2, 12, 0)},
            {ItemType.Pizza, new ObjectEatWaterHealth(35, -10, 0)},
            {ItemType.Burger, new ObjectEatWaterHealth(18, -5, 0)},
            {ItemType.HotDog, new ObjectEatWaterHealth(15, -8, 0)},
            {ItemType.Sandwich, new ObjectEatWaterHealth(10, -3, 0)},
            {ItemType.eCola, new ObjectEatWaterHealth(3, 15, 0)},
            {ItemType.WaterBottle, new ObjectEatWaterHealth(0,20,0) },
            {ItemType.Sprunk, new ObjectEatWaterHealth(5, 25, 0)},
            {ItemType.Payek, new ObjectEatWaterHealth(100, 100, 0)}, // Имеет Кулдаун (Нельзя использовать если юзали medkit недавно)
        };

        [ServerEvent(Event.ResourceStart)]
        public void onResourceStart()
        {
            Log.Debug("Staring timers.", nLog.Type.Info);
            Timers.StartTask("checkwater", 450000, () => CheckWater());
            Timers.StartTask("checkeat", 600000, () => CheckEat());
            Log.Debug("Timers started.", nLog.Type.Success);
        }

        public static void SetEat(Player player, int change)
        {
            Main.Players[player].Eat = change;
            //MySQL.Query($"UPDATE characters SET eat={Main.Players[player].Eat} WHERE uuid={Main.Players[player].UUID}");

            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = "UPDATE `characters` SET `eat`=@eat WHERE `uuid`=@uuid";

            cmd.Parameters.AddWithValue("@eat", Main.Players[player].Eat);
            cmd.Parameters.AddWithValue("@uuid", Main.Players[player].UUID);
            MySQL.Query(cmd);

            GUI.Dashboard.sendStats(player);
            Trigger.ClientEvent(player, "UpdateEat", Main.Players[player].Eat, Convert.ToString(change));
        }
        public static void AddEat(Player player, int change)
        {
            if (change == 0) return;
            if (Main.Players[player].Eat + change > 100)
            {
                Main.Players[player].Eat = 100;

                #region BPКвест: 42 Полностью утолить голод

                #region BattlePass выполнение квеста
                BattlePass.updateBPQuestIteration(player, BattlePass.BPQuestType.AddEat);
                #endregion

                #endregion
            }
            else if(Main.Players[player].Eat + change < 0)
            {
                Main.Players[player].Eat = 0;
            }
            else
            {
                Main.Players[player].Eat += change;
            }

            if (Main.Players[player].Eat == 100 && Main.Players[player].Water == 100)
            {
                #region BPКвест: 43 Полностью утолить голод и жажду

                #region BattlePass выполнение квеста
                BattlePass.updateBPQuestIteration(player, BattlePass.BPQuestType.AddEatAndWater);
                #endregion

                #endregion
            }
            //MySQL.Query($"UPDATE characters SET eat={Main.Players[player].Eat} WHERE uuid={Main.Players[player].UUID}");

            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = "UPDATE `characters` SET `eat`=@eat WHERE `uuid`=@uuid";

            cmd.Parameters.AddWithValue("@eat", Main.Players[player].Eat);
            cmd.Parameters.AddWithValue("@uuid", Main.Players[player].UUID);
            MySQL.Query(cmd);

            Trigger.ClientEvent(player, "UpdateEat", Main.Players[player].Eat, Convert.ToString(change));
            GUI.Dashboard.sendStats(player);
        }
        public static void SetWater(Player player, int change)
        {
            Main.Players[player].Water = change;
            //MySQL.Query($"UPDATE characters SET water={Main.Players[player].Water} WHERE uuid={Main.Players[player].UUID}");

            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = "UPDATE `characters` SET `water`=@water WHERE `uuid`=@uuid";

            cmd.Parameters.AddWithValue("@water", Main.Players[player].Water);
            cmd.Parameters.AddWithValue("@uuid", Main.Players[player].UUID);
            MySQL.Query(cmd);

            Trigger.ClientEvent(player, "UpdateWater", Main.Players[player].Water, Convert.ToString(change));
            GUI.Dashboard.sendStats(player);
        }
        public static void AddWater(Player player, int change)
        {
            if (change == 0) return;

            if (Main.Players[player].Water + change > 100)
            {
                Main.Players[player].Water = 100;

                #region BPКвест: 41 Полностью утолить жажду

                #region BattlePass выполнение квеста
                BattlePass.updateBPQuestIteration(player, BattlePass.BPQuestType.AddWater);
                #endregion

                #endregion
            }
            else if (Main.Players[player].Water + change < 0)
            {
                Main.Players[player].Water = 0;
            }
            else
            {
                Main.Players[player].Water += change;
            }

            if (Main.Players[player].Eat == 100 && Main.Players[player].Water == 100)
            {
                #region BPКвест: 43 Полностью утолить голод и жажду

                #region BattlePass выполнение квеста
                BattlePass.updateBPQuestIteration(player, BattlePass.BPQuestType.AddEatAndWater);
                #endregion

                #endregion

                #region SBPКвест: 8 Полностью утолить голод и жажду 100 раз.

                #region BattlePass выполнение квеста
                BattlePass.updateBPSuperQuestIteration(player, BattlePass.BPSuperQuestType.EatAndWaterGetFullCountTimes);
                #endregion

                #endregion
            }
            //MySQL.Query($"UPDATE characters SET water={Main.Players[player].Water} WHERE uuid={Main.Players[player].UUID}");

            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = "UPDATE `characters` SET `water`=@water WHERE `uuid`=@uuid";

            cmd.Parameters.AddWithValue("@water", Main.Players[player].Water);
            cmd.Parameters.AddWithValue("@uuid", Main.Players[player].UUID);
            MySQL.Query(cmd);

            Trigger.ClientEvent(player, "UpdateWater", Main.Players[player].Water, Convert.ToString(change));
            GUI.Dashboard.sendStats(player);
        }

        public static void AddHealth(Player player, int change)
        {
            if (change == 0) return;

            if (player.Health + change > 100)
            {
                player.Health = 100;
            }
            else if (player.Health + change < 0)
            {
                player.Health = 0;
            }
            else
            {
                player.Health += change;
            }
        }

        public static void SetEatWaterDefault(Player player)
        {
            try
            {
                SetEat(player, 35);
                SetWater(player, 35);
            }
            catch (Exception e) { Log.Write("SetEatWaterDefault: " + e.StackTrace, nLog.Type.Error); }
        }

        public static void CheckEat()
        {
            NAPI.Task.Run(() =>
            {
                Log.Write("Check Eat.", nLog.Type.Info);
                foreach (Player player in Main.Players.Keys.ToList())
                {
                    try
                    {
                        if (player.HasMyData("AGM") && player.GetMyData<bool>("AGM")) continue;
                        if (Main.Players[player].AdminLVL > 0) continue;

                        if (player.Health > 0)
                        {
                            //var rnd = new Random();
                            //int intrnd = rnd.Next(2, 5);
                            if (Main.Players[player].Eat > 0 && Main.Players[player].Eat - 3 > 0)
                            {
                                if (player.IsInVehicle)
                                {
                                    AddEat(player, EatDecayInVehicle);
                                }
                                else
                                {
                                    AddEat(player, EatDecay);
                                }
                            }
                            else if (Main.Players[player].Eat - 3 <= 0)
                            {
                                SetEat(player, 0);
                            }

                            if (Main.Players[player].Eat == 0 && player.Health >= 20)
                            {
                                player.Health -= 2;
                            }
                            else if (Main.Players[player].Water == 0 && Main.Players[player].Eat == 0)
                            {
                                player.Health -= 4;
                            }

                            if (Main.Players[player].Eat >= 80 && Main.Players[player].Water >= 80)
                            {
                                if (player.Health + 2 > 100)
                                {
                                    player.Health = 100;
                                }
                                else
                                {
                                    player.Health += 2;
                                }
                            }
                        }
                    }
                    catch (Exception e) { Log.Write("CheckEat: " + e.StackTrace, nLog.Type.Error); }
                }
            });
        }
        public static void CheckWater()
        {
            NAPI.Task.Run(() =>
            {
                Log.Write("Check Water.", nLog.Type.Info);
                foreach (Player player in Main.Players.Keys.ToList())
                {
                    try
                    {
                        if (player.HasMyData("AGM") && player.GetMyData<bool>("AGM")) continue;
                        if (Main.Players[player].AdminLVL > 0) continue;

                        if (player.Health > 0)
                        {
                            if (Main.Players[player].Water > 0 && Main.Players[player].Water - 2 > 0)
                            {
                                if (player.IsInVehicle)
                                {
                                    AddWater(player, WaterDecayInVehicle);
                                }
                                else
                                {
                                    AddWater(player, WaterDecay);
                                }
                            }
                            else if (Main.Players[player].Water - 2 <= 0)
                            {
                                SetWater(player, 0);
                            }
                            if (Main.Players[player].Water == 0 && player.Health >= 20)
                            {
                                player.Health -= 2;
                            }
                        }
                    }
                    catch (Exception e) { Log.Write("CheckWater: " + e.StackTrace, nLog.Type.Error); }
                }
            });
        }

    }
}
