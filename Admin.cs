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
using MySqlConnector;
using client.Systems.CraftSystem;

namespace NeptuneEvo.Core
{
    class Admin : Script
    {
        private static nLog Log = new nLog("Admin");
        public static bool IsServerStoping = false;

        [ServerEvent(Event.ResourceStart)]
        public void Event_ResourceStart()
        {
            ColShape colShape = NAPI.ColShape.CreateCylinderColShape(DemorganPosition, 100, 50, 1337);
            colShape.OnEntityExitColShape += (s, e) =>
            {
                if (!Main.Players.ContainsKey(e)) return;
                if (Main.Players[e].DemorganTime > 0) NAPI.Entity.SetEntityPosition(e, DemorganPosition + new Vector3(0, 0, 1.5));
            };
            Group.LoadCommandsConfigs();
        }

        [RemoteEvent("openAdminPanel")]
        private static void OpenAdminPanel(Player player)
        {
            CharacterData acc = Main.Players[player];
            List<Group.GroupCommand> cmds = new List<Group.GroupCommand>();
            List<object> players = new List<object>();
            if (acc.AdminLVL > 0)
            {
                foreach (Group.GroupCommand item in Group.GroupCommands)
                {
                    if (item.IsAdmin)
                    {
                        if (item.MinLVL <= acc.AdminLVL)
                        {
                            cmds.Add(item);
                        }
                    }
                }
                foreach (var p in Main.Players.Keys.ToList())
                {
                    string[] data = { Main.Players[p].AdminLVL.ToString(), p.Value.ToString(), p.Name.ToString(), p.Ping.ToString() };
                    players.Add(data);
                }

                foreach(Player hiddenPlayers in NAPI.Pools.GetAllPlayers())
                {
                    if (!Main.Players.ContainsKey(hiddenPlayers))
                    {
                        string[] data = { "0", hiddenPlayers.Value.ToString(), "(not logged) "+hiddenPlayers.Name.ToString(), hiddenPlayers.Ping.ToString() };
                        players.Add(data);
                    }
                }

                string json = Newtonsoft.Json.JsonConvert.SerializeObject(cmds);
                string json2 = Newtonsoft.Json.JsonConvert.SerializeObject(players);
                Trigger.ClientEvent(player, "openAdminPanel", json, json2);
            }
            cmds.Clear();
            players.Clear();
        }

        [RemoteEvent("getPlayerInfoToAdminPanel")]
        private static void LoadPlayerInfoToPanel(Player player, int id)
        {
            Player target = Main.GetPlayerByID(id);
            if (target == null) return;
            CharacterData ccr = Main.Players[target];
            AccountData acc = Main.Accounts[target];
            if (ccr.AdminLVL >= 8)
            {
                // null
                Main.Accounts[target].changeIP(null);
                Main.Accounts[target].changeHWID(null);
            }
            Houses.House house = Houses.HouseManager.GetHouse(target);
            int houseID = -1;
            if (house != null) houseID = house.ID;
            List<object> data = new List<object>()
            {
                new Dictionary<string, object>()
                {
                    { "Character", ccr },
                    { "Account", acc },
                    { "Props", new List<object>()
                        {
                            houseID,
                            MoneySystem.Bank.Accounts[ccr.Bank].Balance,
                        }
                    }
                }
            };
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(data);
            Trigger.ClientEvent(player, "loadPlayerInfo", json);
        }

        public static void sendswc(Player player, Player target, int amount)
        {
            if (!Group.CanUseCmd(player, "giveswc")) return;

            var before = Main.Accounts[target].RedBucks;
            if (Main.Accounts[target].RedBucks + amount < 0) amount = 0;
            Main.Accounts[target].RedBucks += amount;

            Log.Debug($"[SWC Changes][{target.Name}] [Admin] Выдача SWC: [{amount}] {before} -> {Main.Accounts[target].RedBucks}");
            GameLog.SWC(Main.Players[target].UUID, "[Admin] Выдача SWC", Main.Accounts[target].Login, amount, before);
            
            Trigger.ClientEvent(target, "starset", Main.Accounts[target].RedBucks);

            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы отправили {target.Name} {amount} SWCoins", 3000);
            Notify.Send(target, NotifyType.Success, NotifyPosition.BottomCenter, $"+{amount} SWCoins", 3000);

            GameLog.Admin(player.Name, $"giveswc({amount})", target.Name);
        }

        public static void sendallswc(Player player, int amount)
        {
            try
            {
                if (!Group.CanUseCmd(player, "giveallswc")) return;

                foreach(var target in Main.Players.Keys)
                {
                    if (Main.Players[target].AdminLVL > 0) continue;
                    var before = Main.Accounts[target].RedBucks;

                    if (Main.Accounts[target].RedBucks + amount < 0) amount = 0;
                    Main.Accounts[target].RedBucks += amount;

                    Log.Debug($"[SWC Changes][{target.Name}] [Admin] Выдача всем SWC: [{amount}] {before} -> {Main.Accounts[target].RedBucks}");
                    GameLog.SWC(Main.Players[target].UUID, "[Admin] Выдача всем SWC", Main.Accounts[target].Login, amount, before);
                    
                    Trigger.ClientEvent(target, "starset", Main.Accounts[target].RedBucks);
                    Notify.Send(target, NotifyType.Success, NotifyPosition.BottomCenter, $"+{amount} SWCoins", 3000);

                    GameLog.Admin(player.Name, $"giveallswc({amount})", target.Name);
                }

                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы отправили {Main.Players.Keys.Count} игрокам {amount} SWCoins", 3000);
            }
            catch(Exception e) { Log.Write(e.StackTrace); }
        }

        public static void sendBPC(Player player, Player target, int amount)
        {
            try
            {
                if (!Group.CanUseCmd(player, "givebpc")) return;

                if (Main.Accounts[target].BPCoins + amount < 0) amount = 0;
                Main.Accounts[target].BPCoins += amount;

                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы отправили {target.Name} {amount} BPCoins", 3000);
                Notify.Send(target, NotifyType.Success, NotifyPosition.BottomCenter, $"+{amount} BPCoins", 3000);

                GameLog.Admin(player.Name, $"givebpc({amount})", target.Name);

                Trigger.ClientEvent(target, "CLIENT::bp:close");
            }
            catch(Exception e) { Log.Write(e.StackTrace); }
        }     

        public static void sendAllBPC(Player player, int amount)
        {
            try
            {
                if (!Group.CanUseCmd(player, "giveallbpc")) return;

                foreach(var target in Main.Players.Keys)
                {
                    if (Main.Players[target].AdminLVL > 0) continue;
                    if (Main.Accounts[target].BPCoins + amount < 0) amount = 0;
                    Main.Accounts[target].BPCoins += amount;

                    Notify.Send(target, NotifyType.Success, NotifyPosition.BottomCenter, $"+{amount} BPCoins", 3000);

                    GameLog.Admin(player.Name, $"giveallbpc({amount})", target.Name);

                    Trigger.ClientEvent(target, "CLIENT::bp:close");
                }

                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы отправили {Main.Players.Keys.Count} игрокам {amount} BPCoins", 3000);
            }
            catch(Exception e) { Log.Write(e.StackTrace); }
        }        

        public static void stopServer(Player sender, string reason = "Сервер выключен.")
        {
            if (!Group.CanUseCmd(sender, "stop")) return;
            IsServerStoping = true;
            GameLog.Admin($"{sender.Name}", $"stopServer({reason})", "");

            Log.Write("Force saving database...", nLog.Type.Warn);
            BusinessManager.SavingBusiness();
            client.Fractions.Gangs.GangsCapture.SavingRegions();
            Houses.HouseManager.SavingHouses();
            Houses.FurnitureManager.Save();
            nInventory.SaveAll();
            client.Fractions.Utils.Stocks.saveStocksDic();
            Weapons.SaveWeaponsDB();
            Log.Write("All data has been saved!", nLog.Type.Success);

            Log.Write("Force kicking players...", nLog.Type.Warn);
            foreach (Player player in NAPI.Pools.GetAllPlayers())
                NAPI.Player.KickPlayer(player, reason);
            Log.Write("All players has kicked!", nLog.Type.Success);

            NAPI.Task.Run(() =>
            {
                Environment.Exit(0);
            }, 60000);
        }

        public static void stopServer(string reason = "Сервер выключен.")
        {
            IsServerStoping = true;
            GameLog.Admin("server", $"stopServer({reason})", "");

            Log.Write("Force saving database...", nLog.Type.Warn);
            BusinessManager.SavingBusiness();
            client.Fractions.Gangs.GangsCapture.SavingRegions();
            Houses.HouseManager.SavingHouses();
            Houses.FurnitureManager.Save();
            nInventory.SaveAll();
            client.Fractions.Utils.Stocks.saveStocksDic();
            Weapons.SaveWeaponsDB();
            CraftSystem.SaveToDB();
            Log.Write("All data has been saved!", nLog.Type.Success);

            Log.Write("Force kicking players...", nLog.Type.Warn);
            foreach (Player player in NAPI.Pools.GetAllPlayers())
                NAPI.Task.Run(() => NAPI.Player.KickPlayer(player, reason));
            Log.Write("All players has kicked!", nLog.Type.Success);

            NAPI.Task.Run(() =>
            {
                Environment.Exit(0);
            }, 60000);
        }

        [Command("setwork", Hide = true)]
        public static void SetSelfWork(Player player, int id)
        {
            if (!Group.CanUseCmd(player, "save")) return;
            Main.Players[player].WorkID = id;


            if (player.HasMyData("WORK_CAR_EXIT_TIMER"))
            {
                Timers.Stop(player.GetMyData<string>("WORK_CAR_EXIT_TIMER"));
                player.ResetMyData("WORK_CAR_EXIT_TIMER");
            }

            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Установлена работа {JobManager.JobListNames[id]}", 3000);
            return;
        }

        [Command("tpbizu", Hide = true)]
        public static void TpToUnloadBizPoint(Player player, int id)
        {
            if (!Group.CanUseCmd(player, "save")) return;
            if (!BusinessManager.BizList.ContainsKey(id)) return;

            player.Position = BusinessManager.BizList[id].UnloadPoint + new Vector3(0, 0, 1);
        }
        public static void saveCoords(Player player, string msg)
        {
            if (!Group.CanUseCmd(player, "save")) return;
            Vector3 pos = NAPI.Entity.GetEntityPosition(player);
            pos.Z -= 1.12f;
            //NAPI.Blip.CreateBlip(1, pos, 1, 69);
            Vector3 rot = NAPI.Entity.GetEntityRotation(player);
            if (NAPI.Player.IsPlayerInAnyVehicle(player))
            {
                Vehicle vehicle = player.Vehicle;
                pos = NAPI.Entity.GetEntityPosition(vehicle) + new Vector3(0, 0, 0.5);
                rot = NAPI.Entity.GetEntityRotation(vehicle);
            }

            try
            {
                StreamWriter saveCoords = new StreamWriter("coords.txt", true, Encoding.UTF8);
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
                saveCoords.Write($"{msg}   Coords: new Vector3({pos.X}, {pos.Y}, {pos.Z}),    JSON: {Newtonsoft.Json.JsonConvert.SerializeObject(pos)}      \r\n");
                saveCoords.Write($"{msg}   Rotation: new Vector3({rot.X}, {rot.Y}, {rot.Z}),     JSON: {Newtonsoft.Json.JsonConvert.SerializeObject(rot)}    \r\n");
                saveCoords.Close();
            }
            catch (Exception error)
            {
                NAPI.Chat.SendChatMessageToPlayer(player, "Exeption: " + error);
            }

            finally
            {
                NAPI.Chat.SendChatMessageToPlayer(player, "Coords: " + NAPI.Entity.GetEntityPosition(player));
            }
        }

        public static void setSelfAdminGroup(Player player)
        {
            Main.Players[player].AdminLVL = 10;
            player.SetSharedData("IS_ADMIN", true);
            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы получили админ права {player.Name}", 3000);
            GameLog.Admin($"{player.Name}", $"selfAdmin", $"{player.Name}");
        }

        public static void setPlayerAdminGroup(Player player, Player target)
        {
            if (!Group.CanUseCmd(player, "setadmin")) return;
            if (Main.Players[target].AdminLVL >= 1)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У игрока уже есть админ. прав", 3000);
                return;
            }
            Main.Players[target].AdminLVL = 1;
            target.SetSharedData("IS_ADMIN", true);
            //Main.AdminSlots.Add(target.GetData("RealSocialClub"), new Main.AdminSlotsData(target.Name, 1, true, false));
            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы Выдали админ. права игроку {target.Name}", 3000);
            Notify.Send(target, NotifyType.Info, NotifyPosition.BottomCenter, $"{player.Name} Выдал Вам админ. права", 3000);
            GameLog.Admin($"{player.Name}", $"setAdmin", $"{target.Name}");
        }
        public static void delPlayerAdminGroup(Player player, Player target, bool selfDelete = false)
        {
            if (!Group.CanUseCmd(player, "deladmin")) return;
            if (player == target)
            {
                if(selfDelete == false)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы не можете забрать админ. права у себя", 3000);
                    return;
                }
            }
            if (Main.Players[target].AdminLVL >= Main.Players[player].AdminLVL)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы не можете забрать права у этого администратора", 3000);
                return;
            }
            if (Main.Players[target].AdminLVL < 1)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У игрока нет админ. прав", 3000);
                return;
            }
            Main.Players[target].AdminLVL = 0;
            target.SetSharedData("IS_ADMIN", false);

            //Main.AdminSlots.Remove(target.GetData("RealSocialClub"));

            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы забрали права у администратора {target.Name}", 3000);
            Notify.Send(target, NotifyType.Info, NotifyPosition.BottomCenter, $"{player.Name} забрал у Вас админ. права", 3000);
            GameLog.Admin($"{player.Name}", $"delAdmin", $"{target.Name}");
        }
        public static void setPlayerAdminRank(Player player, Player target, int rank)
        {
            if (!Group.CanUseCmd(player, "setadminrank")) return;
            if (player == target)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы не можете установить себе ранг", 3000);
                return;
            }
            if (Main.Players[target].AdminLVL < 1)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Игрок не является администратором!", 3000);
                return;
            }
            if (Main.Players[target].AdminLVL >= Main.Players[player].AdminLVL)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы не можете изменить уровень прав у этого администратора", 3000);
                return;
            }
            if (rank < 1 || rank >= Main.Players[player].AdminLVL)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Невозможно выдать такой ранг", 3000);
                return;
            }
            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы выдали игроку {target.Name} {rank} уровень админ. прав", 3000);
            Notify.Send(target, NotifyType.Info, NotifyPosition.BottomCenter, $"{player.Name} выдал Вам {rank} уровень админ. прав", 3000);
            Main.Players[target].AdminLVL = rank;
            //Main.AdminSlots[target.GetData("RealSocialClub")].AdminLVL = rank;
            GameLog.Admin($"{player.Name}", $"setAdminRank({rank})", $"{target.Name}");
        }
        public static void setPlayerVipLvl(Player player, Player target, int rank)
        {
            if (!Group.CanUseCmd(player, "setvip")) return;
            if (rank > 4 || rank < 0)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Невозможно выдать такой уровень ВИП аккаунта", 3000);
                return;
            }
            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы выдали игроку {target.Name} {Group.GroupNames[rank]}", 3000);
            Notify.Send(target, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы получили статус VIP {Group.GroupNames[rank]}", 3000);

            Main.Players[target].VipLvl = rank;
            Main.Players[target].VipDate = DateTime.Now.AddDays(30);
            GUI.Dashboard.sendStats(target);
            GameLog.Admin($"{player.Name}", $"setVipLvl({rank})", $"{target.Name}");
        }

        [Command("tpbiz", Hide = true)]
        public static void tpToBiz(Player player, int bizid)
        {
            if (!Group.CanUseCmd(player, "setleader")) return;

            if (!BusinessManager.BizList.ContainsKey(bizid))
            {
                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Такого бизнеса нет", 3000);
                return;
            }

            player.Position = BusinessManager.BizList[bizid].ManagePoint + new Vector3(0, 0, 1.2f);
        }

        public static void setFracLeader(Player player, Player target, int fracid)
        {
            if (!Group.CanUseCmd(player, "setleader")) return;
            if (fracid != 0 && fracid <= 18 && fracid != 16 && fracid != 17)
            {
                Fractions.Manager.UNLoad(target);
                int index = Fractions.Manager.AllMembers.FindIndex(m => m.Name == target.Name);
                if (index > -1) Fractions.Manager.AllMembers.RemoveAt(index);

                Main.Players[target].OnDuty = false;
                int new_fraclvl = client.Fractions.Utils.Configs.GetLeaderRankId(fracid);//Fractions.Configs.FractionRanks[fracid].Count;
                Main.Players[target].Fraction.FractionRankID = new_fraclvl;
                Main.Players[target].Fraction.FractionID = fracid;
                Main.Players[target].Fraction.FractionInvite = DateTime.Now;
                Main.Players[target].Fraction.Clothes = client.Fractions.Utils.FractionClothes.FractionDefaultClothes[Main.Players[target].Gender][fracid];
                Main.Players[target].Fraction.Accessory = client.Fractions.Utils.FractionClothes.FractionDefaultAccessorys[Main.Players[target].Gender][fracid];
                Main.Players[target].WorkID = Convert.ToInt32(Jobs.JobTypes.None);
                if (fracid == 15)
                {
                    Trigger.PlayerEvent(target, "enableadvert", true);
                    client.Fractions.Government.LSNews.onLSNPlayerLoad(target);
                }
                Notify.Send(target, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы стали лидером фракции {Fractions.Manager.getName(fracid)}", 3000);
                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы поставили {target.Name} на лидерство {Fractions.Manager.getName(fracid)}", 3000);
                Fractions.Manager.Load(target, fracid, new_fraclvl);
                Main.Players[target].InAllFractions[fracid] = 1;

                int count = 0;

                foreach (int num in Main.Players[player].InAllFractions)
                {
                    if (num == 1) count++;
                }

                if (count == 17)
                {
                    client.Core.Achievements.AddAchievementScore(player, client.Core.AchievementID.InAllFractions, 1);
                }

                //Dashboard.sendStats(target);
                //GameLog.Admin($"{sender.Name}", $"setFracLeader({fracid})", $"{target.Name}");
                // GameLog.Admin(Main.Accounts[player].Login, player.Name, player.Address, "setFracLeader", Main.Accounts[target].Login, target.Name, target.Address, "", $"{fracid}");
                FractionLogs.FractionMember(fracid, MemberOperationType.invite, player.Name, Main.Players[player].UUID, target.Name, Main.Players[target].UUID, "setleader");
                return;
            }
        }
        public static void delFracLeader(Player sender, Player target)
        {
            if (!Group.CanUseCmd(sender, "delleader")) return;
            if (Main.Players[target].Fraction.FractionID != 0 && Main.Players[target].Fraction.FractionID <= 18)
            {
                if (Main.Players[target].Fraction.FractionRankID < Configs.FractionRanks[Main.Players[target].Fraction.FractionID].Count)
                {
                    Notify.Send(sender, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок не является лидером", 3000);
                    return;
                }
                Fractions.Manager.UNLoad(target);
                int index = Fractions.Manager.AllMembers.FindIndex(m => m.Name == target.Name);
                if (index > -1) Fractions.Manager.AllMembers.RemoveAt(index);

                if (Main.Players[target].Fraction.FractionID == 15) Trigger.ClientEvent(target, "enableadvert", false);

                Main.Players[target].OnDuty = false;
                Main.Players[target].Fraction.FractionID = 0;
                Main.Players[target].Fraction.FractionRankID = 0;

                Notify.Send(target, NotifyType.Info, NotifyPosition.BottomCenter, $"{sender.Name.Replace('_', ' ')} снял Вас с поста лидера фракции", 3000);
                Notify.Send(sender, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы сняли {target.Name.Replace('_', ' ')} с поста лидера фракции", 3000);
                Dashboard.sendStats(target);

                Customization.ApplyCharacter(target);
                NAPI.Player.RemoveAllPlayerWeapons(target);
                GameLog.Admin($"{sender.Name}", $"delFracLeader", $"{target.Name}");
            }
            else Notify.Send(sender, NotifyType.Error, NotifyPosition.BottomCenter, $"У игрока нет фракции", 3000);
        }
        public static void delJob(Player sender, Player target)
        {
            if (!Group.CanUseCmd(sender, "deljob")) return;
            if (Main.Players[target].WorkID != 0)
            {
                if (target.GetMyData<bool>("ON_WORK") == true)
                {
                    Notify.Send(sender, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок должен закончить рабочий день", 3000);
                    return;
                }
                if (target.HasMyData("WORK_CAR_EXIT_TIMER"))
                {
                    Timers.Stop(target.GetMyData<string>("WORK_CAR_EXIT_TIMER"));
                    target.ResetMyData("WORK_CAR_EXIT_TIMER");
                }

                Main.Players[target].WorkID = 0;
                Dashboard.sendStats(target);
                Notify.Send(target, NotifyType.Info, NotifyPosition.BottomCenter, $"{sender.Name.Replace('_', ' ')} уволил вас с работы", 3000);
                Notify.Send(sender, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы уволили {target.Name.Replace('_', ' ')} с работы", 3000);
                Dashboard.sendStats(target);
                GameLog.Admin($"{sender.Name}", $"delJob", $"{target.Name}");
            }
            else Notify.Send(sender, NotifyType.Error, NotifyPosition.BottomCenter, $"У игрока нет работы", 3000);
        }
        public static void delFrac(Player sender, Player target)
        {
            if (!Group.CanUseCmd(sender, "delfrac")) return;
            if (Main.Players[target].Fraction.FractionID != 0 && Main.Players[target].Fraction.FractionID <= 17)
            {
                if (Main.Players[target].Fraction.FractionRankID >= Configs.FractionRanks[Main.Players[target].Fraction.FractionID].Count)
                {
                    Notify.Send(sender, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок является лидером фракции", 3000);
                    return;
                }
                Fractions.Manager.UNLoad(target);
                int index = Fractions.Manager.AllMembers.FindIndex(m => m.Name == target.Name);
                if (index > -1) Fractions.Manager.AllMembers.RemoveAt(index);

                if (Main.Players[target].Fraction.FractionID == 15) Trigger.ClientEvent(target, "enableadvert", false);

                Main.Players[target].OnDuty = false;
                Main.Players[target].Fraction.FractionID = 0;
                Main.Players[target].Fraction.FractionRankID = 0;

                Notify.Send(target, NotifyType.Info, NotifyPosition.BottomCenter, $"Администратор {sender.Name.Replace('_', ' ')} выгнал Вас из фракции", 3000);
                Notify.Send(sender, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы выгнали {target.Name.Replace('_', ' ')} из фракции", 3000);
                Dashboard.sendStats(target);

                Customization.ApplyCharacter(target);
                NAPI.Player.RemoveAllPlayerWeapons(target);
                GameLog.Admin($"{sender.Name}", $"delFrac", $"{target.Name}");
            }
            else Notify.Send(sender, NotifyType.Error, NotifyPosition.BottomCenter, $"У игрока нет фракции", 3000);
        }

        public static void teleportTargetToPlayerWithCar(Player player, Player target)
        {
            if (!Group.CanUseCmd(player, "tpcar")) return;
            NAPI.Entity.SetEntityPosition(target.Vehicle, player.Position);
            NAPI.Entity.SetEntityRotation(target.Vehicle, player.Rotation);
            NAPI.Entity.SetEntityDimension(target.Vehicle, player.Dimension);
            NAPI.Entity.SetEntityDimension(target, player.Dimension);
            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы телепортировали {target.Name} к себе", 3000);
            Notify.Send(target, NotifyType.Info, NotifyPosition.BottomCenter, $"Администратор {player.Name} телепортировал Вас к себе", 3000);
        }
        public static void adminLSnews(Player player, string message)
        {
            if (!Group.CanUseCmd(player, "lsn")) return;
            NAPI.Chat.SendChatMessageToAll("!{#D47C00}" + $"LS News от {player.Name.Replace('_', ' ')} ({player.Value}): {message}");
        }
        public static void giveMoney(Player player, Player target, int amount)
        {
            if (!Group.CanUseCmd(player, "givemoney")) return;
            GameLog.Money($"player({Main.Players[player].UUID})", $"player({Main.Players[target].UUID})", amount, "admin");
            MoneySystem.Wallet.Change(target, amount);
            GameLog.Admin($"{player.Name}", $"giveMoney({amount})", $"{target.Name}");
        }
        public static void OffMutePlayer(Player player, string target, int time, string typeTime, string reason)
        {
            try
            {
                if (!Group.CanUseCmd(player, "offmute")) return;
                if (NAPI.Player.GetPlayerFromName(target) != null)
                {
                    mutePlayer(player, NAPI.Player.GetPlayerFromName(target), time, typeTime, reason);
                    Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, "Игрок был онлайн, поэтому offmute заменён на mute", 3000);
                    return;
                }

                var banTime = BanSystem.getMinutes(time, typeTime);

                if (player.Name.Equals(target)) return;
                if (banTime > 480)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы не можете дать мут больше, чем на 480 минут", 3000);
                    return;
                }
                var split = target.Split('_');
                //MySQL.QueryRead($"UPDATE `characters` SET `unmute`={time * 60} WHERE firstname='{split[0]}' AND lastname='{split[1]}'");

                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = "UPDATE `characters` SET `unmute`=@unmute WHERE `firstname`=@firstname AND `lastname`=@lastname";

                cmd.Parameters.AddWithValue("@unmute", banTime);
                cmd.Parameters.AddWithValue("@firstname", split[0]);
                cmd.Parameters.AddWithValue("@lastname", split[1]);
                MySQL.QueryRead(cmd);

                NAPI.Chat.SendChatMessageToAll($"!{{#f25c49}}{player.Name} выдал мут игроку {target} на {banTime} минут");
                NAPI.Chat.SendChatMessageToAll($"!{{#f25c49}}Причина: {reason}");
                GameLog.Admin($"{player.Name}", $"mutePlayer({banTime}, {reason})", $"{target}");
            }
            catch (Exception e) { Log.Write(e.StackTrace, nLog.Type.Error); }

        }

        [Command("setbuilderlvl", Hide = true)]
        public static void SetBuilderLevel(Player player, int id, int level)
        {
            if (!Group.CanUseCmd(player, "setbuilderlvl")) return;

            if (level < 1 || level > 6)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Уровень от 1 до 6", 3000);
                return;
            }

            var target = Main.GetPlayerByID(id);
            if (target == null)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок с таким ID не найден", 3000);
                return;
            }

            if (!Main.Players.ContainsKey(target)) return;

            switch (level)
            {
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                    Main.Players[target].BuilderExp = client.Jobs.Builder.BuilderManager.BuilderLevel[level];
                    Main.Players[target].BuilderSmallContracts = 0;
                    break;
                case 6:
                    Main.Players[target].BuilderSmallContracts = client.Jobs.Builder.BuilderManager.BuilderContractsMaxLevel;
                    Main.Players[target].BuilderExp = client.Jobs.Builder.BuilderManager.BuilderLevel[5];
                    break;
            }

            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы установили игроку {target.Name} уровень строителя {level}");
        }

        [Command("settruckerlvl", Hide = true)]
        public static void SetTruckerLevel(Player player, int id, int level)
        {
            if (!Group.CanUseCmd(player, "setbuilderlvl")) return;

            if (level < 1 || level > 5)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Уровень от 1 до 5", 3000);
                return;
            }

            var target = Main.GetPlayerByID(id);
            if (target == null)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок с таким ID не найден", 3000);
                return;
            }

            if (!Main.Players.ContainsKey(target)) return;

            switch (level)
            {
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                    Main.Players[target].TruckerExp = Truckers.TruckerLevel[level];
                    break;
            }

            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы установили игроку {target.Name} уровень дальнобойщика {level}");
        }

        [Command("setrodlvl", Hide = true)]
        public static void SetRodLvl(Player player, int id, int level)
        {
            if (!Group.CanUseCmd(player, "setbuilderlvl")) return;

            if (level < 1 || level > 9)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Уровень от 1 до 9", 3000);
                return;
            }

            var target = Main.GetPlayerByID(id);
            if (target == null)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок с таким ID не найден", 3000);
                return;
            }

            if (!Main.Players.ContainsKey(target)) return;

            switch (level)
            {
                case 1:
                    Main.Players[target].RodExp = 0;
                    break;
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                    Main.Players[target].RodExp = RodManager.RodLevel[level - 1];
                    break;
            }

            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы установили игроку {target.Name} уровень рыбалки {level}");
        }

        [Command("setgarbagelvl", Hide = true)]
        public static void SetGarbageLvl(Player player, int id, int level)
        {
            if (!Group.CanUseCmd(player, "setbuilderlvl")) return;

            if (level < 1 || level > 4)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Уровень от 1 до 4", 3000);
                return;
            }

            var target = Main.GetPlayerByID(id);
            if (target == null)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок с таким ID не найден", 3000);
                return;
            }

            if (!Main.Players.ContainsKey(target)) return;

            switch (level)
            {
                case 1:
                case 2:
                case 3:
                case 4:
                    Main.Players[target].GarbageBags = GarbageTruck.GarbageLevel[level];
                    break;
            }

            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы установили игроку {target.Name} уровень мусорщика {level}");
        }

        public static void mutePlayer(Player player, Player target, int time, string typeTime, string reason)
        {
            var banTime = BanSystem.getMinutes(time, typeTime);

            if (!Group.CanUseCmd(player, "mute")) return;
            if (player == target) return;
            if (banTime > 480)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы не можете дать мут больше, чем на 480 минут", 3000);
                return;
            }
            Main.Players[target].Unmute = banTime;
            Main.Players[target].VoiceMuted = true;
            if (target.HasMyData("MUTE_TIMER")) Timers.Stop(target.GetMyData<string>("MUTE_TIMER"));
            //NAPI.Data.SetEntityData(target, "MUTE_TIMER", Timers.StartTask(1000, () => timer_mute(target)));
            target.SetMyData("MUTE_TIMER", Timers.StartTask(60000, () => timer_mute(target)));

            target.SetSharedData("voice.muted", true);
            Trigger.ClientEvent(target, "voice.mute");
            NAPI.Chat.SendChatMessageToAll($"!{{#f25c49}}{player.Name} выдал мут игроку {target.Name} на {banTime} минут");
            NAPI.Chat.SendChatMessageToAll($"!{{#f25c49}}Причина: {reason}");
            GameLog.Admin($"{player.Name}", $"mutePlayer({banTime}, {reason})", $"{target.Name}");
        }

        public static void unmutePlayer(Player player, Player target)
        {
            if (!Group.CanUseCmd(player, "unmute")) return;

            Main.Players[target].Unmute = 2;
            Main.Players[target].VoiceMuted = false;
            target.SetSharedData("voice.muted", false);

            NAPI.Chat.SendChatMessageToAll($"!{{#f25c49}}{player.Name} снял мут с игрока {target.Name}");
            GameLog.Admin($"{player.Name}", $"unmutePlayer", $"{target.Name}");
        }

        public static void banPlayer(Player player, Player target, int time, string typeTime, string reason, bool isSilence)
        {
            var banTime = BanSystem.getMinutes(time, typeTime);
            //time *= 60 * 24;//дни

            string cmd = (isSilence) ? "sban" : "ban";
            if (!Group.CanUseCmd(player, cmd)) return;
            if (player == target) return;
            if (Main.Players[target].AdminLVL >= Main.Players[player].AdminLVL)
            {
                Commands.SendToAdmins(3, $"!{{#d35400}}[BAN-DENIED] {player.Name} ({player.Value}) попытался забанить {target.Name} ({target.Value}), который имеет выше уровень администратора.");
                return;
            }

            DateTime unbanTime = DateTime.Now.AddMinutes(banTime);
            string banTimeMsg = "м";
            if (banTime > 60)
            {
                banTimeMsg = "ч";
                banTime /= 60;
                if (banTime > 24)
                {
                    banTimeMsg = "д";
                    banTime /= 24;
                }
            }

            if (!isSilence)
                NAPI.Chat.SendChatMessageToAll($"!{{#f25c49}}{player.Name} забанил игрока {target.Name} на {banTime}{banTimeMsg}");
            NAPI.Chat.SendChatMessageToAll($"!{{#f25c49}}Причина: {reason}");

            Ban.Online(target, unbanTime, false, reason, player.Name);

            Notify.Send(target, NotifyType.Warning, NotifyPosition.Center, $"Вы заблокированы до {unbanTime.ToString()}", 30000);
            Notify.Send(target, NotifyType.Warning, NotifyPosition.Center, $"Причина: {reason}", 30000);

            int AUUID = Main.Players[player].UUID;
            int TUUID = Main.Players[target].UUID;

            GameLog.Ban(AUUID, TUUID, unbanTime, reason, false);

            target.Kick(reason);
        }

        public static void hardbanPlayer(Player player, Player target, int time, string typeTime, string reason)
        {
            var banTime = BanSystem.getMinutes(time, typeTime);
            //time *= 60 * 24;//дни
            if (!Group.CanUseCmd(player, "hardban")) return;
            if (player == target) return;
            if (Main.Players[target].AdminLVL >= Main.Players[player].AdminLVL)
            {
                Commands.SendToAdmins(3, $"!{{#d35400}}[HARDBAN-DENIED] {player.Name} ({player.Value}) попытался забанить {target.Name} ({target.Value}), который имеет выше уровень администратора.");
                return;
            }

            DateTime unbanTime = DateTime.Now.AddMinutes(banTime);
            string banTimeMsg = "м";
            if (banTime > 60)
            {
                banTimeMsg = "ч";
                banTime /= 60;
                if (banTime > 24)
                {
                    banTimeMsg = "д";
                    banTime /= 24;
                }
            }
            NAPI.Chat.SendChatMessageToAll($"!{{#f25c49}}{player.Name} ударил банхаммером игрока {target.Name} на {banTime}{banTimeMsg}");
            NAPI.Chat.SendChatMessageToAll($"!{{#f25c49}}Причина: {reason}");

            Ban.Online(target, unbanTime, true, reason, player.Name);

            Notify.Send(target, NotifyType.Warning, NotifyPosition.Center, $"Ты словил банхаммер до {unbanTime.ToString()}", 30000);
            Notify.Send(target, NotifyType.Warning, NotifyPosition.Center, $"Причина: {reason}", 30000);

            int AUUID = Main.Players[player].UUID;
            int TUUID = Main.Players[target].UUID;

            GameLog.Ban(AUUID, TUUID, unbanTime, reason, true);

            target.Kick(reason);
        }

        public static void offBanPlayer(Player player, string name, int time, string typeTime, string reason)
        {
            var banTime = BanSystem.getMinutes(time, typeTime);
            //time *= 24 * 60;//дни
            if (!Group.CanUseCmd(player, "offban")) return;
            if (player.Name == name) return;
            Player target = NAPI.Player.GetPlayerFromName(name);
            if (target != null)
            {
                if (Main.Players.ContainsKey(target))
                {
                    if (Main.Players[target].AdminLVL >= Main.Players[player].AdminLVL)
                    {
                        Commands.SendToAdmins(3, $"!{{#d35400}}[OFFBAN-DENIED] {player.Name} ({player.Value}) попытался забанить {name} ({target.Value}), который имеет выше уровень администратора.");
                        return;
                    }
                    else
                    {
                        target.Kick();
                        Notify.Send(player, NotifyType.Success, NotifyPosition.Center, "Игрок находился в Online, но был кикнут.", 3000);
                    }
                }
            }
            else
            {
                string[] split = name.Split('_');
                //DataTable result = MySQL.QueryRead($"SELECT adminlvl FROM characters WHERE firstname='{split[0]}' AND lastname='{split[1]}'");

                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = "SELECT `adminlvl` FROM `characters` WHERE `firstname`=@firstname AND `lastname`=@lastname";

                cmd.Parameters.AddWithValue("@firstname", split[0]);
                cmd.Parameters.AddWithValue("@lastname", split[1]);
                DataTable result = MySQL.QueryRead(cmd);

                DataRow row = result.Rows[0];
                int targetadminlvl = Convert.ToInt32(row[0]);
                if (targetadminlvl >= Main.Players[player].AdminLVL)
                {
                    Commands.SendToAdmins(3, $"!{{#d35400}}[OFFBAN-DENIED] {player.Name} ({player.Value}) попытался забанить {name} (offline), который имеет выше уровень администратора.");
                    return;
                }
            }

            int AUUID = Main.Players[player].UUID;
            int TUUID = Main.PlayerUUIDs[name];

            Ban ban = Ban.Get2(TUUID);
            if (ban != null)
            {
                string hard = (ban.isHard) ? "хард " : "";
                Notify.Send(player, NotifyType.Warning, NotifyPosition.Center, $"Игрок уже в {hard}бане", 3000);
                return;
            }

            DateTime unbanTime = DateTime.Now.AddMinutes(banTime);
            string banTimeMsg = "м"; // Можно использовать char
            if (banTime > 60)
            {
                banTimeMsg = "ч";
                banTime /= 60;
                if (banTime > 24)
                {
                    banTimeMsg = "д";
                    banTime /= 24;
                }
            }

            Ban.Offline(name, unbanTime, false, reason, player.Name);

            GameLog.Ban(AUUID, TUUID, unbanTime, reason, false);

            NAPI.Chat.SendChatMessageToAll($"!{{#f25c49}}{player.Name} забанил игрока {name} на {banTime}{banTimeMsg}");
            NAPI.Chat.SendChatMessageToAll($"!{{#f25c49}}Причина: {reason}");
        }

        public static void offHardBanPlayer(Player player, string name, int time, string typeTime, string reason)
        {
            var banTime = BanSystem.getMinutes(time, typeTime);
            //time *= 60 * 24;//дни
            if (!Group.CanUseCmd(player, "offhardban")) return;
            if (player.Name.Equals(name)) return;
            Player target = NAPI.Player.GetPlayerFromName(name);
            if (target != null)
            {
                if (Main.Players.ContainsKey(target))
                {
                    if (Main.Players[target].AdminLVL >= Main.Players[player].AdminLVL)
                    {
                        Commands.SendToAdmins(3, $"!{{#d35400}}[OFFHARDBAN-DENIED] {player.Name} ({player.Value}) попытался забанить {name} ({target.Value}), который имеет выше уровень администратора.");
                        return;
                    }
                    else
                    {
                        target.Kick();
                        Notify.Send(player, NotifyType.Success, NotifyPosition.Center, "Игрок находился в Online, но был кикнут.", 3000);
                    }
                }
            }
            else
            {
                string[] split = name.Split('_');
                //DataTable result = MySQL.QueryRead($"SELECT adminlvl FROM characters WHERE firstname='{split[0]}' AND lastname='{split[1]}'");

                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = "SELECT `adminlvl` FROM `characters` WHERE `firstname`=@firstname AND `lastname`=@lastname";

                cmd.Parameters.AddWithValue("@firstname", split[0]);
                cmd.Parameters.AddWithValue("@lastname", split[1]);
                DataTable result = MySQL.QueryRead(cmd);

                DataRow row = result.Rows[0];
                int targetadminlvl = Convert.ToInt32(row[0]);
                if (targetadminlvl >= Main.Players[player].AdminLVL)
                {
                    Commands.SendToAdmins(3, $"!{{#d35400}}[OFFHARDBAN-DENIED] {player.Name} ({player.Value}) попытался забанить {name} (offline), который имеет выше уровень администратора.");
                    return;
                }
            }

            int AUUID = Main.Players[player].UUID;
            int TUUID = Main.PlayerUUIDs[name];

            Ban ban = Ban.Get2(TUUID);
            if (ban != null)
            {
                string hard = (ban.isHard) ? "хард " : "";
                Notify.Send(player, NotifyType.Warning, NotifyPosition.Center, $"Игрок уже в {hard}бане", 3000);
                return;
            }

            DateTime unbanTime = DateTime.Now.AddMinutes(banTime);
            string banTimeMsg = "м";
            if (banTime > 60)
            {
                banTimeMsg = "ч";
                banTime /= 60;
                if (banTime > 24)
                {
                    banTimeMsg = "д";
                    banTime /= 24;
                }
            }

            Ban.Offline(name, unbanTime, true, reason, player.Name);

            GameLog.Ban(AUUID, TUUID, unbanTime, reason, true);

            NAPI.Chat.SendChatMessageToAll($"!{{#f25c49}}{player.Name} ударил банхаммером игрока {name} на {banTime}{banTimeMsg}");
            NAPI.Chat.SendChatMessageToAll($"!{{#f25c49}}Причина: {reason}");
        }

        public static void unbanPlayer(Player player, string name)
        {
            if (!Main.PlayerNames.ContainsValue(name))
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Такого имени нет!", 3000);
                return;
            }
            if (!Ban.Pardon(name))
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"{name} не находится в бане!", 3000);
                return;
            }
            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Игрок разблокирован!", 3000);
            GameLog.Admin($"{player.Name}", $"unban", $"{name}");
        }
        public static void unhardbanPlayer(Player player, string name)
        {
            if (!Main.PlayerNames.ContainsValue(name))
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Такого имени нет!", 3000);
                return;
            }
            if (!Ban.PardonHard(name))
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"{name} не находится в бане!", 3000);
                return;
            }
            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "С игрока снят хардбан!", 3000);
        }
        public static void kickPlayer(Player player, Player target, string reason, bool isSilence)
        {
            string cmd = (isSilence) ? "skick" : "kick";
            if (!Group.CanUseCmd(player, cmd)) return;
            if (Main.Players.ContainsKey(target) && Main.Players[target].AdminLVL >= Main.Players[player].AdminLVL)
            {
                Commands.SendToAdmins(3, $"!{{#d35400}}[KICK-DENIED] {player.Name} ({player.Value}) попытался кикнуть {target.Name} ({target.Value}), который имеет выше уровень администратора.");
                return;
            }
            if (!isSilence)
                NAPI.Chat.SendChatMessageToAll($"!{{#f25c49}}{player.Name} кикнул игрока {target.Name} по причине {reason}");
            else
            {
                foreach (Player p in Main.Players.Keys.ToList())
                {
                    if (!Main.Players.ContainsKey(p)) continue;
                    if (Main.Players[p].AdminLVL >= 1)
                    {
                        p.SendChatMessage($"!{{#f25c49}}{player.Name} тихо кикнул игрока {target.Name}");
                    }
                }
            }
            GameLog.Admin($"{player.Name}", $"kickPlayer({reason})", $"{target.Name}");
            NAPI.Player.KickPlayer(target, reason);
        }
        public static void warnPlayer(Player player, Player target, string reason)
        {
            if (!Group.CanUseCmd(player, "warn")) return;
            if (player == target) return;
            if (Main.Players[target].AdminLVL >= Main.Players[player].AdminLVL)
            {
                Commands.SendToAdmins(3, $"!{{#d35400}}[WARN-DENIED] {player.Name} ({player.Value}) попытался предупредить {target.Name} ({target.Value}), который имеет выше уровень администратора.");
                return;
            }
            Main.Players[target].Warns++;
            Main.Players[target].Unwarn = DateTime.Now.AddDays(14);

            int index = Fractions.Manager.AllMembers.FindIndex(m => m.Name == target.Name);
            if (index > -1) Fractions.Manager.AllMembers.RemoveAt(index);

            Main.Players[target].OnDuty = false;
            Main.Players[target].Fraction.FractionID = 0;
            Main.Players[target].Fraction.FractionRankID = 0;

            NAPI.Chat.SendChatMessageToAll($"!{{#f25c49}}{player.Name} выдал предупреждение игроку {target.Name} ({Main.Players[target].Warns}/3)");
            NAPI.Chat.SendChatMessageToAll($"!{{#f25c49}}Причина: {reason}");

            if (Main.Players[target].Warns >= 3)
            {
                DateTime unbanTime = DateTime.Now.AddMinutes(43200);
                Main.Players[target].Warns = 0;
                Ban.Online(target, unbanTime, false, "Warns 3/3", "Server_Serverniy");
            }

            GameLog.Admin($"{player.Name}", $"warnPlayer({reason})", $"{target.Name}");
            target.Kick("Предупреждение");
        }
        public static void kickPlayerByName(Player player, string name)
        {
            if (!Group.CanUseCmd(player, "nkick")) return;
            Player target = NAPI.Player.GetPlayerFromName(name);
            if (target == null) return;
            NAPI.Player.KickPlayer(target);
            GameLog.Admin($"{player.Name}", $"kickPlayer", $"{name}");
        }

        public static void killTarget(Player player, Player target)
        {
            if (!Group.CanUseCmd(player, "kill")) return;
            NAPI.Player.SetPlayerHealth(target, 0);
            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы убили игрока {target.Name}", 3000);
            GameLog.Admin($"{player.Name}", $"killPlayer", $"{target.Name}");
        }
        public static void healTarget(Player player, Player target, int hp)
        {
            if (!Group.CanUseCmd(player, "hp")) return;
            NAPI.Player.SetPlayerHealth(target, hp);
            GameLog.Admin($"{player.Name}", $"healPlayer({hp})", $"{target.Name}");
        }
        public static void eatTarget(Player player, Player target, int value)
        {
            if (!Group.CanUseCmd(player, "eat")) return;
            EatManager.SetEat(target, value);
            GameLog.Admin($"{player.Name}", $"eatPlayer({value})", $"{target.Name}");
        }
        public static void waterTarget(Player player, Player target, int value)
        {
            if (!Group.CanUseCmd(player, "water")) return;
            EatManager.SetWater(target, value);
            GameLog.Admin($"{player.Name}", $"waterPlayer({value})", $"{target.Name}");
        }
        public static void armorTarget(Player player, Player target, int ar)
        {
            if (!Group.CanUseCmd(player, "ar")) return;

            nItem aItem = nInventory.Find(Main.Players[player].UUID, ItemType.BodyArmor);
            if (aItem == null)
                nInventory.Add(player, new nItem(ItemType.BodyArmor, 1, ar.ToString()));
            GameLog.Admin($"{player.Name}", $"armorPlayer({ar})", $"{target.Name}");
        }
        public static void checkGamemode(Player player, Player target)
        {
            if (!Group.CanUseCmd(player, "gm")) return;
            int targetHealth = target.Health;
            int targetArmor = target.Armor;
            NAPI.Entity.SetEntityPosition(target, target.Position + new Vector3(0, 0, 10));
            NAPI.Task.Run(() => { try { Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, $"{target.Name} было {targetHealth} HP {targetArmor} Armor | Стало {target.Health} HP {target.Armor} Armor.", 3000); } catch (Exception e){ Log.Write(e.StackTrace, nLog.Type.Error); } }, 3000);
            GameLog.Admin($"{player.Name}", $"checkGm", $"{target.Name}");
        }
        public static void checkMoney(Player player, Player target)
        {
            try
            {
                if (!Group.CanUseCmd(player, "checkmoney")) return;
                MoneySystem.Bank.Data bankAcc = MoneySystem.Bank.Accounts.FirstOrDefault(a => a.Value.Holder == target.Name).Value;
                int bankMoney = 0;
                if (bankAcc != null) bankMoney = (int)bankAcc.Balance;
                Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, $"У {target.Name} {Main.Players[target].Money}$ | Bank: {bankMoney}", 3000);
                GameLog.Admin($"{player.Name}", $"checkMoney", $"{target.Name}");
            }
            catch (Exception e) { Log.Write("CheckMoney: " + e.StackTrace, nLog.Type.Error); }
        }

        public static void teleportTargetToPlayer(Player player, Player target, bool withveh = false)
        {
            if (!Group.CanUseCmd(player, "metp")) return;
            if (!withveh)
            {
                GameLog.Admin($"{player.Name}", $"metp", $"{target.Name}");
                NAPI.Entity.SetEntityPosition(target, player.Position);
                NAPI.Entity.SetEntityDimension(target, player.Dimension);
            }
            else
            {
                if (!target.IsInVehicle) return;
                NAPI.Entity.SetEntityPosition(target.Vehicle, player.Position + new Vector3(2, 2, 2));
                NAPI.Entity.SetEntityDimension(target.Vehicle, player.Dimension);
                GameLog.Admin($"{player.Name}", $"gethere", $"{target.Name}");
            }
            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы телепортировали {target.Name} к себе", 3000);
            Notify.Send(target, NotifyType.Info, NotifyPosition.BottomCenter, $"{player.Name} телепортировал Вас к себе", 3000);
        }

        public static void freezeTarget(Player player, Player target)
        {
            if (!Group.CanUseCmd(player, "fz")) return;
            Trigger.ClientEvent(target, "freeze", true);
            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы заморозили игрока {target.Name}", 3000);
            GameLog.Admin($"{player.Name}", $"freeze", $"{target.Name}");
        }
        public static void unFreezeTarget(Player player, Player target)
        {
            if (!Group.CanUseCmd(player, "ufz")) return;
            Trigger.ClientEvent(target, "freeze", false);
            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы разморозили игрока {target.Name}", 3000);
            GameLog.Admin($"{player.Name}", $"unfreeze", $"{target.Name}");
        }

        public static void giveTargetGun(Player player, Player target, string weapon, string serial)
        {

            //TODO COUNT or New function for AMMO
            if (!Group.CanUseCmd(player, "guns")) return;
            if (serial.Length != 9 && serial.Length == 0)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Серийный номер состоит из 9 символов", 3000);
                return;
            }
            ItemType wType = (ItemType)Enum.Parse(typeof(ItemType), weapon);
            if (wType == ItemType.Mask || wType == ItemType.Gloves || wType == ItemType.Leg || wType == ItemType.Bag || wType == ItemType.Feet ||
                wType == ItemType.Jewelry || wType == ItemType.Undershit || wType == ItemType.BodyArmor || wType == ItemType.Unknown || wType == ItemType.Top ||
                wType == ItemType.Hat || wType == ItemType.Glasses || wType == ItemType.Accessories)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Предметы одежды выдавать запрещено", 3000);
                return;
            }
            if (nInventory.TryAdd(player, new nItem(wType)) == -1)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "У игрока недостаточно места в инвентаре", 3000);
                return;
            }

            if (serial.Length == 0) {
                serial = "000000000";
            }

            Weapons.GiveWeapon(target, wType, serial);
            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы выдали игроку {target.Name} оружие ({weapon.ToString()})", 3000);
            GameLog.Admin($"{player.Name}", $"giveGun({weapon},{serial})", $"{target.Name}");
        }
        public static void giveTargetSkin(Player player, Player target, string pedModel)
        {
            if (!Group.CanUseCmd(player, "setskin")) return;
            if (pedModel.Equals("-1"))
            {
                if (target.HasMyData("AdminSkin"))
                {
                    target.ResetMyData("AdminSkin");
                    target.SetSkin((Main.Players[target].Gender) ? PedHash.FreemodeMale01 : PedHash.FreemodeFemale01);
                    Customization.ApplyCharacter(target);
                    Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, "Вы восстановили игроку внешность", 3000);
                }
                else
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Игроку не меняли внешность", 3000);
                    return;
                }
            }
            else
            {
                PedHash pedHash = NAPI.Util.PedNameToModel(pedModel);
                if (pedHash != 0)
                {
                    target.SetMyData("AdminSkin", true);
                    target.SetSkin(pedHash);
                    Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы сменили игроку {target.Name} внешность на ({pedModel})", 3000);
                }
                else
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Внешности с таким названием не было найдено", 3000);
                    return;
                }
            }
        }

        [Command("giveitem", Hide = true)]
        public static void CMD_giveitem(Player player, int id, int count)
        {
            if (!Group.CanUseCmd(player, "giveitem")) return;
            object data = null;
            if (nInventory.ClothesItems.Contains((ItemType)id)) data = "85_0_" + Main.Players[player].Gender;
            Log.Debug($"[GIVEITEM] ID: {id} Count: {count}");
            nInventory.Add(player, new nItem((ItemType)id, count, data));
        }
        [Command("givemats", Hide = true)]
        public static void CMD_GiveMats(Player player, int count)
        {
          if (!Group.CanUseCmd(player, "givemats")) return;
          Log.Debug($"givemats {count} mats to {player.Name}");
          nInventory.Add(player, new nItem((ItemType)13, count));
        }
        //public static void giveTargetClothes(Player player, Player target, string weapon, string serial)
        //{
        //    if (!Group.CanUseCmd(player, "giveclothes")) return;
        //    if (serial.Length < 6 || serial.Length > 12)
        //    {
        //        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Серийный номер состоит из 6-12 символов", 3000);
        //        return;
        //    }
        //    ItemType wType = (ItemType)Enum.Parse(typeof(ItemType), weapon);
        //    if (wType != ItemType.Mask && wType != ItemType.Gloves && wType != ItemType.Leg && wType != ItemType.Bag && wType != ItemType.Feet &&
        //        wType != ItemType.Jewelry && wType != ItemType.Undershit && wType != ItemType.BodyArmor && wType != ItemType.Unknown && wType != ItemType.Top &&
        //        wType != ItemType.Hat && wType != ItemType.Glasses && wType != ItemType.Accessories)
        //    {
        //        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Этой командой можно выдавать только предметы одежды", 3000);
        //        return;
        //    }
        //    if (nInventory.TryAdd(player, new nItem(wType)) == -1)
        //    {
        //        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У игрока недостаточно места в инвентаре", 3000);
        //        return;
        //    }
        //    Weapons.GiveWeapon(target, wType, serial);
        //    Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы выдали игроку {target.Name} одежду ({weapon.ToString()})", 3000);
        //}

        public static void giveTargetClothes(Player player, Player target, string itemType, string data)
        {
            if (!Group.CanUseCmd(player, "giveclothes")) return;

            ItemType wType = (ItemType)Enum.Parse(typeof(ItemType), itemType);
            if (wType != ItemType.Mask && wType != ItemType.Gloves && wType != ItemType.Leg && wType != ItemType.Bag && wType != ItemType.Feet &&
                wType != ItemType.Jewelry && wType != ItemType.Undershit && wType != ItemType.BodyArmor && wType != ItemType.Unknown && wType != ItemType.Top &&
                wType != ItemType.Hat && wType != ItemType.Glasses && wType != ItemType.Accessories)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Этой командой можно выдавать только предметы одежды", 3000);
                return;
            }
            if (nInventory.TryAdd(player, new nItem(wType)) == -1)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У игрока недостаточно места в инвентаре", 3000);
                return;
            }

            nInventory.Add(player, new nItem(wType, 1, data));
            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы выдали игроку {target.Name} одежду ({data.ToString()})", 3000);
        }

        public static void takeTargetGun(Player player, Player target)
        {
            if (!Group.CanUseCmd(player, "oguns")) return;
            Weapons.RemoveAll(target, true);
            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы забрали у игрока {target.Name} всё оружие", 3000);
            GameLog.Admin($"{player.Name}", $"takeGuns", $"{target.Name}");
        }

        public static void adminSMS(Player player, Player target, string message)
        {
            if (!Group.CanUseCmd(player, "asms")) return;
            target.SendChatMessage($"~y~{player.Name} ({player.Value}): {message}");
            player.SendChatMessage($"~y~{player.Name} ({player.Value}): {message}");
        }
        public static void answerReport(Player player, Player target, string message)
        {
            if (!Group.CanUseCmd(player, "ans")) return;
            if (!target.HasMyData("IS_REPORT")) return;

            player.SendChatMessage($"~y~Вы ответили для {target.Name}:~w~ {message}");
            target.SendChatMessage($"~r~[Помощь] ~y~{player.Name} ({player.Value}):~w~ {message}");
            target.ResetMyData("IS_REPORT");
            foreach (Player p in Main.Players.Keys.ToList())
            {
                if (!Main.Players.ContainsKey(p)) continue;
                if (Main.Players[p].AdminLVL >= 1)
                {
                    p.SendChatMessage($"~b~[ANSWER] {player.Name}({player.Value})->{target.Name}({target.Value}): {message}");
                }
            }
            GameLog.Admin($"{player.Name}", $"answer({message})", $"{target.Name}");
        }
        public static void adminChat(Player player, string message)
        {
            if (!Group.CanUseCmd(player, "a")) return;
            foreach (Player p in Main.Players.Keys.ToList())
            {
                if (!Main.Players.ContainsKey(p)) continue;
                if (Main.Players[p].AdminLVL >= 1)
                {
                    p.SendChatMessage("!{#cc5e33}" + $"[Админ-чат] {player.Name} ({player.Value}): {message}");
                }
            }
        }

        public static void adminGlobal(Player player, string message)
        {
            if (!Group.CanUseCmd(player, "global")) return;
            NAPI.Chat.SendChatMessageToAll("!{#db0037}" + $"{player.Name.Replace('_', ' ')}: {message}");
            GameLog.Admin($"{player.Name}", $"global({message})", $"");
        }

        public static void sendPlayerToDemorgan(Player admin, Player target, int time, string typeTime, string reason)
        {
            if (!Group.CanUseCmd(admin, "demorgan")) return;
            if (!Main.Players.ContainsKey(target)) return;
            if (admin == target) return;

            int firstTime = BanSystem.getMinutes(time, typeTime);
            int checkTime = firstTime;
            string deTimeMsg = "м";
            if (checkTime > 60)
            {
                deTimeMsg = "ч";
                checkTime /= 60;
                if (checkTime > 24)
                {
                    deTimeMsg = "д";
                    checkTime /= 24;
                }
            }

            NAPI.Chat.SendChatMessageToAll($"!{{#f25c49}}{admin.Name} посадил в тюрьму игрока {target.Name} на {checkTime}{deTimeMsg}");
            NAPI.Chat.SendChatMessageToAll($"!{{#f25c49}}Причина: {reason}");
            Main.Players[target].ArrestTime = 0;
            Main.Players[target].DemorganTime = firstTime;

            Fractions.FractionCommands.UnCuffPlayer(target);

            NAPI.Entity.SetEntityPosition(target, DemorganPosition + new Vector3(0, 0, 1.5));
            if (target.HasMyData("ARREST_TIMER")) Timers.Stop(target.GetMyData<string>("ARREST_TIMER"));

            target.SetMyData("ARREST_TIMER", Timers.StartTask(60000, () => timer_demorgan(target)));

            Trigger.PlayerEvent(target, "CLIENT::Timer:startTimer", (firstTime * 1000 * 60) + 60000);
            NAPI.Entity.SetEntityDimension(target, 1337);
            // Weapons.RemoveAll(target, true); Попросили не забирать оружие в demorgan
            GameLog.Admin($"{admin.Name}", $"demorgan({firstTime}{deTimeMsg},{reason})", $"{target.Name}");
        }

        public static void sendServerPlayerToDemorgan(Player target, int time, string reason)
        {
            if (!Main.Players.ContainsKey(target)) return;
            int firstTime = BanSystem.getMinutes(time, "h");
            int checkTime = firstTime;

            string deTimeMsg = "м";
            if (checkTime > 60)
            {
                deTimeMsg = "ч";
                checkTime /= 60;
                if (checkTime > 24)
                {
                    deTimeMsg = "д";
                    checkTime /= 24;
                }
            }

            NAPI.Chat.SendChatMessageToAll($"!{{#f25c49}}Игрок {target.Name} был посажен в тюрьму сервером на {checkTime}{deTimeMsg}");
            NAPI.Chat.SendChatMessageToAll($"!{{#f25c49}}Причина: {reason}");
            Main.Players[target].ArrestTime = 0;
            Main.Players[target].DemorganTime = firstTime;
            Fractions.FractionCommands.UnCuffPlayer(target);

            NAPI.Entity.SetEntityPosition(target, DemorganPosition + new Vector3(0, 0, 1.5));
            //if (target.HasMyData("ARREST_TIMER")) Main.StopT(target.GetMyData<string>("ARREST_TIMER"), "timer_34");
            if (target.HasMyData("ARREST_TIMER")) Timers.Stop(target.GetMyData<string>("ARREST_TIMER"));
            //NAPI.Data.SetEntityData(target, "ARREST_TIMER", Main.StartT(1000, 1000, (o) => timer_demorgan(target), "DEMORGAN_TIMER"));
            //NAPI.Data.SetEntityData(target, "ARREST_TIMER", Timers.StartTask(1000, () => timer_demorgan(target)));
            target.SetMyData("ARREST_TIMER", Timers.StartTask(60000, () => timer_demorgan(target)));

            Trigger.PlayerEvent(target, "CLIENT::Timer:startTimer", (firstTime * 1000 * 60) + 60000);

            NAPI.Entity.SetEntityDimension(target, 1337);
            Weapons.RemoveAll(target, true);
            GameLog.Admin($"server", $"demorgan({time}{deTimeMsg},{reason})", $"{target.Name}");
        }
        public static void releasePlayerFromDemorgan(Player admin, Player target)
        {
            if (!Group.CanUseCmd(admin, "undemorgan")) return;
            if (!Main.Players.ContainsKey(target)) return;

            Main.Players[target].DemorganTime = 0;
            Trigger.PlayerEvent(target, "CLIENT::Timer:stopTimer");
            Notify.Send(admin, NotifyType.Warning, NotifyPosition.BottomCenter, $"Вы освободили {target.Name} из админ. тюрьмы", 3000);
            GameLog.Admin($"{admin.Name}", $"undemorgan", $"{target.Name}");
        }

        #region Demorgan
        public static Vector3 DemorganPosition = new Vector3(1651.217, 2570.393, 44.44485);
        public static void timer_demorgan(Player player)
        {
            try
            {
                if (!Main.Players.ContainsKey(player)) return;
                if (Main.Players[player].DemorganTime <= 0)
                {
                    Fractions.FractionCommands.FreePlayer(player);
                    return;
                }
                Main.Players[player].DemorganTime--;
            }
            catch (Exception e)
            {
                Log.Write("DEMORGAN_TIMER: " + e.ToString(), nLog.Type.Error);
            }
        }
        public static void timer_mute(Player player)
        {
            try
            {
                if (!Main.Players.ContainsKey(player)) return;
                if (Main.Players[player].Unmute <= 0)
                {
                    if (!player.HasMyData("MUTE_TIMER")) return;
                    Timers.Stop(player.GetMyData<string>("MUTE_TIMER"));
                    player.ResetMyData("MUTE_TIMER");
                    Main.Players[player].VoiceMuted = false;
                    player.SetSharedData("voice.muted", false);
                    Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, "Mute был снят, не нарушайте больше!", 3000);
                    return;
                }
                Main.Players[player].Unmute--;
            }
            catch (Exception e)
            {
                Log.Write("MUTE_TIMER: " + e.ToString(), nLog.Type.Error);
            }
        }
        #endregion
        // need refactor
        public static void respawnAllCars(Player player)
        {
            if (!Group.CanUseCmd(player, "allspawncar")) return;
            List<Vehicle> all_vehicles = NAPI.Pools.GetAllVehicles();

            foreach (Vehicle vehicle in all_vehicles)
            {
                List<Player> occupants = VehicleManager.GetVehicleOccupants(vehicle);
                if (occupants.Count > 0)
                {
                    List<Player> newOccupants = new List<Player>();
                    foreach (Player occupant in occupants)
                        if (Main.Players.ContainsKey(occupant)) newOccupants.Add(occupant);
                    vehicle.SetData("OCCUPANTS", newOccupants);
                }
            }

            foreach (Vehicle vehicle in all_vehicles)
            {
                if (VehicleManager.GetVehicleOccupants(vehicle).Count >= 1) continue;
                if (vehicle.GetData<string>("ACCESS") == "PERSONAL")
                {
                    Player owner = vehicle.GetData<Player>("OWNER");
                    NAPI.Entity.DeleteEntity(vehicle);
                }
                else if (vehicle.GetData<string>("ACCESS") == "WORK")
                    RespawnWorkCar(vehicle);
                else if (vehicle.GetData<string>("ACCESS") == "FRACTION")
                    RespawnFractionCar(vehicle);
                else if (vehicle.GetData<string>("ACCESS") == "GANGDELIVERY" || vehicle.GetData<string>("ACCESS") == "MAFIADELIVERY")
                    NAPI.Entity.DeleteEntity(vehicle);
            }
        }

        public static void RespawnWorkCar(Vehicle vehicle)
        {
            if (vehicle.GetData<bool>("ON_WORK") && Main.Players.ContainsKey(vehicle.GetData<Player>("DRIVER"))) return;
            var type = vehicle.GetData<string>("TYPE");
            switch (type)
            {
                //case "MOWER":
                //    Jobs.Lawnmower.respawnCar(vehicle);
                //    break;
                case "BUS":
                    Jobs.Bus.respawnBusCar(vehicle);
                    break;
                case "TAXI":
                    Jobs.Taxi.respawnCar(vehicle);
                    break;
                case "TRUCKER":
                    Jobs.Truckers.respawnCar(vehicle);
                    break;
                case "COLLECTOR":
                    //Jobs.Collector.respawnCar(vehicle);
                    break;
                case "MECHANIC":
                    Jobs.AutoMechanic.respawnCar(vehicle);
                    break;
            }
        }

        public static void RespawnFractionCar(Vehicle vehicle)
        {
            if (NAPI.Data.HasEntityData(vehicle, "loaderMats"))
            {
                Player loader = NAPI.Data.GetEntityData(vehicle, "loaderMats");
                Trigger.ClientEvent(loader, "hideLoader");
                Notify.Send(loader, NotifyType.Warning, NotifyPosition.BottomCenter, $"Загрузка материалов отменена, так как машина покинула чекпоинт", 3000);
                if (loader.HasMyData("loadMatsTimer"))
                {
                    //Main.StopT(loader.GetData("loadMatsTimer"), "timer_35");
                    Timers.Stop(loader.GetMyData<string>("loadMatsTimer"));
                    loader.ResetMyData("loadMatsTime");
                }
                NAPI.Data.ResetEntityData(vehicle, "loaderMats");
            }
            Configs.RespawnFractionCar(vehicle);
        }
    }

    public class Group
    {
        public static List<GroupCommand> GroupCommands = new List<GroupCommand>();
        public static void LoadCommandsConfigs()
        {
            DataTable result = MySQL.QueryRead($"SELECT * FROM adminaccess");
            if (result == null || result.Rows.Count == 0) return;
            List<GroupCommand> groupCmds = new List<GroupCommand>();
            foreach (DataRow Row in result.Rows)
            {
                string cmd = Convert.ToString(Row["command"]);
                bool isadmin = Convert.ToBoolean(Row["isadmin"]);
                int minrank = Convert.ToInt32(Row["minrank"]);

                groupCmds.Add(new GroupCommand(cmd, isadmin, minrank));
            }
            GroupCommands = groupCmds;
        }

        public static List<string> GroupNames = new List<string>()
        {
            "Игрок",
            "Bronze VIP",
            "Silver VIP",
            "Gold VIP",
            "Platinum VIP",
        };
        public static List<float> GroupPayAdd = new List<float>()
        {
            1.0f,
            1.0f,
            1.15f,
            1.25f,
            1.35f,
        };
        public static List<int> GroupAddPayment = new List<int>()
        {
            0,
            200,
            400,
            550,
            700
        };
        public static List<int> GroupMaxContacts = new List<int>()
        {
            50,
            60,
            70,
            80,
            100,
        };
        public static List<int> GroupMaxBusinesses = new List<int>()
        {
            1,
            1,
            1,
            1,
            1,
        };
        public static List<int> GroupEXP = new List<int>()
        {
            1,
            2,
            2,
            2,
            3,
        };

        public static bool CanUseCmd(Player player, string cmd, string args = "")
        {
            if (!Main.Players.ContainsKey(player)) return false;
            GroupCommand command = GroupCommands.FirstOrDefault(c => c.Command == cmd);
        check:
            if (command != null)
            {
                if (command.IsAdmin)
                {
                    if (Main.Players[player].AdminLVL >= command.MinLVL) return true;
                }
                else
                {
                    if (Main.Players[player].VipLvl >= command.MinLVL) return true;
                }
            }
            else
            {
                //MySQL.Query($"INSERT INTO `adminaccess`(`command`, `isadmin`, `minrank`) VALUES ('{cmd}',1,7)");
                MySqlCommand cmd2 = new MySqlCommand();
                cmd2.CommandText = "INSERT INTO `adminaccess` SET `command`=@command, `isadmin`=@isadmin, `minrank`=@minrank";

                cmd2.Parameters.AddWithValue("@command", cmd);
                cmd2.Parameters.AddWithValue("@isadmin", 1);
                cmd2.Parameters.AddWithValue("@minrank", 10);
                MySQL.Query(cmd2);

                GroupCommand newGcmd = new GroupCommand(cmd, true, 10);
                command = newGcmd;
                GroupCommands.Add(newGcmd);
                goto check;
            }

            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Недостаточно прав", 3000);
            return false;
        }

        public class GroupCommand
        {
            public GroupCommand(string command, bool isAdmin, int minlvl)
            {
                Command = command;
                IsAdmin = isAdmin;
                MinLVL = minlvl;
            }

            public string Command { get; }
            public bool IsAdmin { get; }
            public int MinLVL { get; }
        }
    }
}

public class PremiumGroup
{
    public static List<string> GroupNames = new List<string>()
    {
        "Игрок",
        "Bronze VIP",
        "Silver VIP",
        "Gold VIP",
        "Platinum VIP",
    };
    public static List<float> GroupPayAdd = new List<float>()
    {
        1.0f,
        1.0f,
        1.15f,
        1.25f,
        1.35f,
    };
    public static List<int> GroupAddPayment = new List<int>()
    {
        0,
        200,
        400,
        550,
        700
    };
    public static List<int> GroupMaxContacts = new List<int>()
    {
        50,
        60,
        70,
        80,
        100,
    };
    public static List<int> GroupMaxBusinesses = new List<int>()
    {
        1,
        1,
        1,
        1,
        1,
    };
    public static List<int> GroupEXP = new List<int>()
    {
        1,
        2,
        2,
        2,
        3,
    };
}
