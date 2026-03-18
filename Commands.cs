using GTANetworkAPI;
using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using NeptuneEvo.GUI;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using Newtonsoft.Json;
using NeptuneEvo.SDK;
using NeptuneEvo.Core.Character;
using NeptuneEvo.MoneySystem;
using System.Text.RegularExpressions;
using MySqlConnector;
using NeptuneEvo.Houses;
using NeptuneEvo.Voice;
using NeptuneEvo.Rent;
using client.Fractions.Utils;
using static NeptuneEvo.Core.Selecting;
using client.Jobs.Builder;
using System.Reflection;
using client.Core;

namespace NeptuneEvo.Core
{
    class Commands : Script
    {
        private static nLog Log = new nLog("Commands");
        private static Random rnd = new Random();

        #region Chat logic

        public static void SendToAdmins(ushort minLVL, string message)
        {
            foreach (var p in Main.Players.Keys.ToList())
            {
                if (!Main.Players.ContainsKey(p)) continue;
                if (Main.Players[p].AdminLVL >= minLVL)
                {
                    p.SendChatMessage(message);
                }
            }
        }

        private static string RainbowExploit(Player sender, string message)
        {
            if (message.Contains("!{"))
            {
                foreach (var p in Main.Players.Keys.ToList())
                {
                    if (!Main.Players.ContainsKey(p)) continue;
                    if (Main.Players[p].AdminLVL >= 1)
                    {
                        p.SendChatMessage($"~y~[CHAT-EXPLOIT] {sender.Name} ({sender.Value}) - {message}");
                    }
                }
                return Regex.Replace(message, "!", string.Empty);
            }
            return message;
        }

        [ServerEvent(Event.ChatMessage)]
        public void API_onChatMessage(Player sender, string message)
        {
            try
            {
                if (!Main.Players.ContainsKey(sender)) return;
                if (Main.Players[sender].Unmute > 0)
                {
                    Notify.Send(sender, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы замучены еще на {Main.Players[sender].Unmute} минут", 3000);
                    return;
                }
                else if (Main.Players[sender].VoiceMuted)
                {
                    NAPI.Task.Run(() =>
                    {
                        try
                        {
                            Main.Players[sender].VoiceMuted = false;
                            sender.SetSharedData("voice.muted", false);
                        }
                        catch (Exception e){ Log.Write(e.StackTrace, nLog.Type.Error); }
                    });
                }
                //await Log.WriteAsync($"<{sender.Name.ToString()}> {message}", nLog.Type.Info);

                message = RainbowExploit(sender, message);
                //message = Main.BlockSymbols(message);
                List<Player> players = Main.GetPlayersInRadiusOfPosition(sender.Position, 10f, sender.Dimension);
                NAPI.Task.Run(() =>
                {
                    try
                    {
                        int[] id = new int[] { sender.Value };
                        foreach (Player c in players)
                        {
                            Trigger.ClientEvent(c, "sendRPMessage", "chat", "{name}: " + message, id);
                        }

                        if (!sender.HasMyData("PhoneVoip")) return;
                        Voice.VoicePhoneMetaData phoneMeta = sender.GetMyData<VoicePhoneMetaData>("PhoneVoip");
                        if (phoneMeta.CallingState == "talk" && Main.Players.ContainsKey(phoneMeta.Target))
                        {
                            var pSim = Main.Players[sender].Sim;
                            var contactName = (Main.Players[phoneMeta.Target].Contacts.ContainsKey(pSim)) ? Main.Players[phoneMeta.Target].Contacts[pSim] : pSim.ToString();
                            phoneMeta.Target.SendChatMessage($"[В телефоне] {contactName}: {message}");
                        }
                    }
                    catch (Exception e) { Log.Write("ChatMessage_TaskRun: " + e.StackTrace, nLog.Type.Error); }
                });
                return;
            }
            catch (Exception e) { Log.Write("ChatMessage: " + e.StackTrace, nLog.Type.Error); }
        }
        #endregion Chat logic

        #region AdminCommands

        [Command("revive", Hide = true)] // Воскресить игрока после смерти (3 лвл)
        public static void CMD_revive(Player client, int id)
        {
            try
            {
                if (!Group.CanUseCmd(client, "revive")) return;
                Player target = Main.GetPlayerByID(id);
                if (target == null)
                {
                    Notify.Send(client, NotifyType.Error, NotifyPosition.BottomCenter, "Игрок с таким ID не найден", 3000);
                    return;
                }
                target.StopAnimation();
                NAPI.Entity.SetEntityPosition(target, target.Position + new Vector3(0, 0, 0.5));
                target.SetSharedData("InDeath", false);
                Trigger.ClientEvent(target, "DeathTimer", false);
                target.Health = 100;
                target.ResetMyData("IS_DYING");
                target.ResetSharedData("IS_DYING");
                Main.Players[target].IsAlive = true;
                Main.OffAntiAnim(target);
                if (target.HasMyData("DYING_TIMER"))
                {
                    Timers.Stop(target.GetMyData<string>("DYING_TIMER"));
                    target.ResetMyData("DYING_TIMER");
                }
                Notify.Send(target, NotifyType.Info, NotifyPosition.BottomCenter, $"Игрок ({client.Value}) реанимировал Вас", 3000);
                Notify.Send(client, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы реанимировали игрока ({target.Value})", 3000);

                if (target.HasMyData("CALLEMS_BLIP"))
                {
                    NAPI.Entity.DeleteEntity(target.GetMyData<Blip>("CALLEMS_BLIP"));
                }
                if (target.HasMyData("CALLEMS_COL"))
                {
                    NAPI.ColShape.DeleteColShape(target.GetMyData<ColShape>("CALLEMS_COL"));
                }
            }
            catch (Exception e)
            {
                Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error);
            }
        }

        #region Lastbonus
        [Command("getbonus", Hide = true)] // Получить информацию о бонусе игрока (3 лвл)
        public static void GetLastBonus(Player player, int id)
        {
            if (!Group.CanUseCmd(player, "getbonus")) return;

            var target = Main.GetPlayerByID(id);
            if (target == null)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок с таким ID не найден", 3000);
                return;
            }
            DateTime date = new DateTime((new DateTime().AddMinutes(Main.Players[target].LastBonus)).Ticks);
            var hour = date.Hour;
            var min = date.Minute;
            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Бонус игрока({id}): {hour} часов и {min} минут ({Main.Players[target].LastBonus})", 3000);
        }
        #endregion

        #region Lastbonus
        [Command("lastbonus", Hide = true)] // Получить информацию о бонусе игрока (3 лвл)
        public static void LastBonus(Player player)
        {
            if (!Group.CanUseCmd(player, "lastbonus")) return;
            DateTime date = new DateTime((new DateTime().AddMinutes(Main.oldconfig.LastBonusMin)).Ticks);
            var hour = date.Hour;
            var min = date.Minute;
            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Бонус составляет: {hour} часов и {min} минут", 2500);
        }
        #endregion

        #region Lastbonus
        [Command("setbonus", Hide = true)] // Выдать игроку время до получения бонуса (7 лвл)
        public static void SetLastBonus(Player player, int id, int count)
        {
            if (!Group.CanUseCmd(player, "setbonus")) return;

            var target = Main.GetPlayerByID(id);
            if (target == null)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок с таким ID не найден", 3000);
                return;
            }
            count = Convert.ToInt32(Math.Abs(count));
            if (count > Main.oldconfig.LastBonusMin)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Введенное число превышает значение максимума. Максимум: {Main.oldconfig.LastBonusMin}", 3000);
                return;
            }
            Main.Players[target].LastBonus = count;
            DateTime date = new DateTime((new DateTime().AddMinutes(Main.Players[target].LastBonus)).Ticks);
            var hour = date.Hour;
            var min = date.Minute;
            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Бонус для игрока({id}) установлен на {hour} часов и {min} минут ({Main.Players[target].LastBonus})", 3000);
        }
        #endregion

        [Command("testbnotify", Hide = true)] // Тестовая команда для новых уведомлений (8 лвл)
        public static void CMD_testbnotify(Player player, bool type, string header, string header1, string text, int a, int b, int c, string pic, int icon)
        {
            try
            {
                if (!Group.CanUseCmd(player, "testbnotify")) return;

                Trigger.ClientEvent(player, "BetterNotify", type, header, header1, text, a, b, c, pic, icon);
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }

        [Command("testnotify", Hide = true)] // Тестовая команда для новых уведомлений (8 лвл)
        public static void CMD_testnotify(Player player, int type, int pos, string text)
        {
            try
            {
                if (!Group.CanUseCmd(player, "testnotify")) return;

                var notyPos = NotifyPosition.BottomCenter;
                switch (pos)
                {
                    case 0:
                        notyPos = NotifyPosition.Top;
                        break;
                    case 1:
                        notyPos = NotifyPosition.TopLeft;
                        break;
                    case 2:
                        notyPos = NotifyPosition.TopCenter;
                        break;
                    case 3:
                        notyPos = NotifyPosition.TopRight;
                        break;
                    case 4:
                        notyPos = NotifyPosition.Center;
                        break;
                    case 5:
                        notyPos = NotifyPosition.CenterLeft;
                        break;
                    case 6:
                        notyPos = NotifyPosition.CenterRight;
                        break;
                    case 7:
                        notyPos = NotifyPosition.Bottom;
                        break;
                    case 8:
                        notyPos = NotifyPosition.BottomLeft;
                        break;
                    case 9:
                        notyPos = NotifyPosition.BottomCenter;
                        break;
                    case 10:
                        notyPos = NotifyPosition.BottomRight;
                        break;
                }

                switch(type)
                {
                    case 0:
                        Notify.Send(player, NotifyType.Alert, notyPos, text, 3000000);
                        return;
                    case 1:
                        Notify.Send(player, NotifyType.Error, notyPos, text, 3000000);
                        return;
                    case 2:
                        Notify.Send(player, NotifyType.Success, notyPos, text, 3000000);
                        return;
                    case 3:
                        Notify.Send(player, NotifyType.Info, notyPos, text, 3000000);
                        return;
                    case 4:
                        Notify.Send(player, NotifyType.Warning, notyPos, text, 3000000);
                        return;
                }



            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }

        [Command("createrod", Hide = true)] // Создать место для рыбалки (7 лвл)
        public static void CMD_createRod(Player player, float radius, int lvl, int isblip)
        {
            try
            {
                RodManager.createRodAreaCommand(player, radius, lvl, isblip == 1 ? true : false);
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }

        [Command("deleterod", Hide = true)] // Удалить место для рыбалки (7 лвл)
        public static void CMD_deleteRod(Player player, int id)
        {
            try
            {
                RodManager.deleteRodAreaCommand(player, id);
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }

        [Command("givelic", Hide = true)] // Выдать лицензии (7 лвл)
        public static void CMD_giveLicense(Player player, int id, int lic)
        {
            try
            {
                if (!Group.CanUseCmd(player, "givelic")) return;

                var target = Main.GetPlayerByID(id);
                if (target == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок с таким ID не найден", 3000);
                    return;
                }

                if (lic < 0 || lic >= Main.Players[target].Licenses.Count)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"lic = от 0 до {Main.Players[target].Licenses.Count - 1}", 3000);
                    return;
                }

                Main.Players[target].Licenses[lic] = true;
                Dashboard.sendStats(target);

                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Успешно выдано", 3000);
            }
            catch (Exception e){ Log.Write(e.StackTrace, nLog.Type.Error); }
        }

        [Command("removelic", Hide = true)] // Забрать лицензии (7 лвл)
        public static void CMD_removeLicense(Player player, int id, int lic)
        {
            try
            {
                if (!Group.CanUseCmd(player, "removelic")) return;

                var target = Main.GetPlayerByID(id);
                if (target == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок с таким ID не найден", 3000);
                    return;
                }

                if (lic < 0 || lic >= Main.Players[target].Licenses.Count)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"lic = от 0 до {Main.Players[target].Licenses.Count - 1}", 3000);
                    return;
                }

                Main.Players[target].Licenses[lic] = false;
                Dashboard.sendStats(target);

                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Успешно выдано", 3000);
            }
            catch (Exception e) { Log.Write(e.StackTrace, nLog.Type.Error); }
        }

        public static Vehicle FindVehicleByID(int ID)
        {
            List<Vehicle> vehs = NAPI.Pools.GetAllVehicles().ToList();
            foreach (Vehicle veh in vehs)
            {
                if (veh == null) continue;
                else if (veh.Value == ID) return veh;
            }
            return null;
        }

        [Command("getcar", Hide = true)]  //Телепорт транспорта к себе по ID (3 лвл)
        public static void ACMD_GetCar(Player player, int id)
        {
            try
            {
                if (!Group.CanUseCmd(player, "getcar")) return;
                if (player == null) return;
                // Проверяете на существующий класс персонажа, на админку и всё, что вам нужно
                Vehicle veh = FindVehicleByID(id);
                if (veh == null) return;
                veh.Position = player.Position;
                veh.Rotation = player.Rotation;
                veh.Dimension = player.Dimension;
            }
            catch (Exception e)
            {
                Console.WriteLine($"ACMD_GetCar Exception: {e.ToString()}");
            }
        }

        [Command("tocar", Hide = true)]  //Телепорт транспорта к себе по ID (3 лвл)
        public static void ACMD_ToCar(Player player, int id)
        {
            try
            {
                if (!Group.CanUseCmd(player, "getcar")) return;
                if (player == null) return;
                // Проверяете на существующий класс персонажа, на админку и всё, что вам нужно
                Vehicle veh = FindVehicleByID(id);
                if (veh == null) return;

                player.Position = veh.Position;
                player.Rotation = veh.Rotation;
                player.Dimension = veh.Dimension;
            }
            catch (Exception e)
            {
                Console.WriteLine($"ACMD_GetCar Exception: {e.ToString()}");
            }
        }

        [Command("vehchange", Hide = true)] // Сменить тип машины (7 лвл)
        public static void CMD_vehchage(Player client, string newmodel)
        {
            try
            {
                if (!Group.CanUseCmd(client, "vehchange")) return;

                if (!client.IsInVehicle) return;

                if (!client.Vehicle.HasData("ACCESS"))
                    return;
                else if (client.Vehicle.GetData<string>("ACCESS") == "PERSONAL")
                {
                    int id = client.Vehicle.GetData<int>("ID");
                    VehicleManager.Vehicles[id].Model = newmodel;
                    Notify.Send(client, NotifyType.Warning, NotifyPosition.BottomCenter, "Машина будет доступна после респавна", 3000);
                }
                else if (client.Vehicle.GetData<string>("ACCESS") == "WORK")
                    return;
                else if (client.Vehicle.GetData<string>("ACCESS") == "FRACTION")
                    return;
                else if (client.Vehicle.GetData<string>("ACCESS") == "GANGDELIVERY" || client.Vehicle.GetData<string>("ACCESS") == "MAFIADELIVERY")
                    return;
            }
            catch (Exception e) { Log.Write(e.StackTrace, nLog.Type.Error); }
        }

        [Command("bankfix", Hide = true)] // Удалить банковский счет (7 лвл)
        public static void CMD_bankfix(Player client, int bank)
        {
            try
            {
                if (!Group.CanUseCmd(client, "bankfix")) return;
                if (Bank.Accounts.ContainsKey(bank))
                {
                    Bank.RemoveByID(bank);
                    Notify.Send(client, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы успешно удалили банковский счёт номер {bank}", 3000);
                }
                else Notify.Send(client, NotifyType.Error, NotifyPosition.BottomCenter, $"Банковский счёт {bank} не найден", 3000);
            }
            catch (Exception e){ Log.Write(e.StackTrace, nLog.Type.Error); }
        }

        [Command("gethwid", Hide = true)] // Получить HWID игрока (7 лвл)
        public static void CMD_gethwid(Player client, int ID)
        {
            try
            {
                if (!Group.CanUseCmd(client, "gethwid")) return;
                Player target = Main.GetPlayerByID(ID);
                if (target == null)
                {
                    Notify.Send(client, NotifyType.Error, NotifyPosition.BottomCenter, "Игрок с таким ID не найден", 3000);
                    return;
                }
                client.SendChatMessage("Реальный HWID у " + target.Name + ": " + target.GetMyData<string>("RealHWID"));
            }
            catch (Exception e) { Log.Write(e.StackTrace, nLog.Type.Error); }
        }

        [Command("getsocialclub", Hide = true)] // Получить Social Club игрока (7 лвл)
        public static void CMD_getsc(Player client, int ID)
        {
            try
            {
                if (!Group.CanUseCmd(client, "getsocialclub")) return;
                Player target = Main.GetPlayerByID(ID);
                if (target == null)
                {
                    Notify.Send(client, NotifyType.Error, NotifyPosition.BottomCenter, "Игрок с таким ID не найден", 3000);
                    return;
                }
                client.SendChatMessage("Реальный SocialClub у " + target.Name + ": " + target.GetData<string>("RealSocialClub"));
            }
            catch (Exception e) { Log.Write(e.StackTrace, nLog.Type.Error); }
        }

        [Command("loggedinfix", Hide = true)] // Фикс авторизации (хз) (7 лвл)
        public static void CMD_loggedinfix(Player player, string login)
        {
            try
            {
                if (!Main.Players.ContainsKey(player)) return;
                if (!Group.CanUseCmd(player, "loggedinfix")) return;
                if (Main.LoggedIn.ContainsKey(login))
                {
                    if (NAPI.Player.IsPlayerConnected(Main.LoggedIn[login]))
                    {
                        Main.LoggedIn[login].Kick();
                        Main.LoggedIn.Remove(login);
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Вы кикнули персонажа с сервера, через минуту можно будет пытаться зайти в аккаунт.", 3000);
                    }
                    else
                    {
                        Main.LoggedIn.Remove(login);
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Персонаж был не в сети, аккаунт удалён из списка авторизовавшихся.", 3000);
                    }

                    var account = Main.Accounts.FirstOrDefault(x => x.Value.Login == login);
                    if (account.Value.Login == login) Main.Accounts.Remove(account.Key);
                }
                else Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Аккаунта в сети с логином {login} не найдено", 3000);
            }
            catch (Exception e){ Log.Write(e.StackTrace, nLog.Type.Error); }
        }

        [Command("giveammo", Hide = true)] // Выдать патроны (4 лвл)
        public static void CMD_ammo(Player client, int ID, int type, int amount = 1)
        {
            try
            {
                if (!Group.CanUseCmd(client, "giveammo")) return;

                var target = Main.GetPlayerByID(ID);
                if (target == null)
                {
                    Notify.Send(client, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок с таким ID не найден", 3000);
                    return;
                }

                List<ItemType> types = new List<ItemType>
                {
                    ItemType.PistolAmmo,
                    ItemType.RiflesAmmo,
                    ItemType.ShotgunsAmmo,
                    ItemType.SMGAmmo,
                    ItemType.SniperAmmo,
                    ItemType.Grenade,
                };

                if (type > types.Count || type < -1) return;

                var tryAdd = nInventory.TryAdd(target, new nItem(types[type], amount));
                if (tryAdd == -1)
                {
                    Notify.Send(client, NotifyType.Error, NotifyPosition.BottomCenter, "Недостаточно места в инвентаре", 3000);
                    return;
                }
                nInventory.Add(target, new nItem(types[type], amount));
            }
            catch (Exception e){ Log.Write(e.StackTrace, nLog.Type.Error); }
        }

        [Command("redname", Hide = true)] // Красный админский ник (1 лвл)
        public static void CMD_redname(Player player)
        {
            try
            {
                if (!Group.CanUseCmd(player, "redname")) return;

                if (!player.HasSharedData("REDNAME") || !player.GetSharedData<bool>("REDNAME"))
                {
                    player.SendChatMessage("~r~Redname ON");
                    player.SetSharedData("REDNAME", true);
                }
                else
                {
                    player.SendChatMessage("~r~Redname OFF");
                    player.SetSharedData("REDNAME", false);
                }

            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }

        [Command("hidenick", Hide = true)] // Скрыть ник (7 лвл)
        public static void CMD_hidenick(Player player)
        {
            if (!Group.CanUseCmd(player, "hidenick")) return;
            if (!player.HasSharedData("HideNick") || !player.GetSharedData<bool>("HideNick"))
            {
                player.SendChatMessage("~g~HideNick ON");
                player.SetSharedData("HideNick", true);
            }
            else
            {
                player.SendChatMessage("~g~HideNick OFF");
                player.SetSharedData("HideNick", false);
            }
        }

        [Command("giveswc", Hide = true)] // Выдать SWCoins (8 лвл)
        public static void CMD_giveswc(Player player, int id, int amount)
        {
            try
            {
                if (!Group.CanUseCmd(player, "giveswc")) return;

                var target = Main.GetPlayerByID(id);
                if (target == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок с таким ID не найден", 3000);
                    return;
                }
                Admin.sendswc(player, target, amount);
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }

        [Command("giveallswc", Hide = true)] // Выдать SWCoins (8 лвл)
        public static void CMD_giveallswc(Player player, int amount)
        {
            try
            {
                if (!Group.CanUseCmd(player, "giveallswc")) return;
                
                Admin.sendallswc(player, amount);
            }
            catch (Exception e) { Log.Write(e.StackTrace, nLog.Type.Error); }
        }

        [Command("checkprop", Hide = true)] // Узнать о имуществе игрока (3 лвл)
        public static void CMD_checkProperety(Player player, int id)
        {
            try
            {
                if (!Main.Players.ContainsKey(player)) return;
                if (!Group.CanUseCmd(player, "checkprop")) return;

                var target = Main.GetPlayerByID(id);
                if (target == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок с таким ID не найден", 3000);
                    return;
                }

                if (!Main.Players.ContainsKey(target)) return;
                var house = Houses.HouseManager.GetHouse(target);
                if (house != null)
                {
                    if (house.Owner == target.Name)
                    {
                        player.SendChatMessage($"Игрок имеет дом стоимостью ${house.Price} класса '{Houses.HouseManager.HouseTypeList[house.Type].Name}'");
                        var targetVehicles = VehicleManager.getAllPlayerVehicles(target.Name);
                        foreach (var num in targetVehicles)
                            player.SendChatMessage($"У игрока есть машина '{VehicleManager.Vehicles[num].Model}' с номером '{num}'");
                    }
                    else
                        player.SendChatMessage($"Игрок заселен в дом к {house.Owner} стоимостью ${house.Price}");
                }
                else
                    player.SendChatMessage("У игрока нет дома");
            }
            catch (Exception e)
            {
                Log.Write("checkprop: " + e.StackTrace, nLog.Type.Error);
            }
        }

        [Command("id", Hide = true)] // Найти игрока по персональному ID (1 лвл)
        public static void CMD_checkId(Player player, string target)
        {
            try
            {
                if (!Main.Players.ContainsKey(player)) return;
                if (!Group.CanUseCmd(player, "id")) return;

                int id;
                if (Int32.TryParse(target, out id))
                {
                    foreach (var p in Main.Players.Keys.ToList())
                    {
                        if (!Main.Players.ContainsKey(p)) continue;
                        if (p.Value == id)
                        {
                            player.SendChatMessage($"ID: {p.Value} | {p.Name}");
                            return;
                        }
                    }
                    player.SendChatMessage("Игрок с таким ID не найден");
                }
                else
                {
                    var players = 0;
                    foreach (var p in Main.Players.Keys.ToList())
                    {
                        if (!Main.Players.ContainsKey(p)) continue;
                        if (p.Name.ToUpper().Contains(target.ToUpper()))
                        {
                            player.SendChatMessage($"ID: {p.Value} | {p.Name}");
                            players++;
                        }
                    }
                    if (players == 0)
                        player.SendChatMessage("Не найдено игрока с таким именем");
                }
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\"/id/:\n" + e.ToString(), nLog.Type.Error); }
        }

        [Command("setdim", Hide = true)] // Установить виртуальный мир для игрока (3 лвл)
        public static void CMD_setDim(Player player, int id, int dim)
        {
            try
            {
                if (!Main.Players.ContainsKey(player)) return;
                if (!Group.CanUseCmd(player, "setdim")) return;

                var target = Main.GetPlayerByID(id);
                if (target == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок с таким ID не найден", 3000);
                    return;
                }

                if (!Main.Players.ContainsKey(target)) return;
                target.Dimension = Convert.ToUInt32(dim);
                GameLog.Admin($"{player.Name}", $"setDim({dim})", $"{target.Name}");
            }
            catch (Exception e)
            {
                Log.Write("setdim: " + e.StackTrace, nLog.Type.Error);
            }
        }

        [Command("checkdim", Hide = true)] // Узнать виртуальный мир игрока (3 лвл)
        public static void CMD_checkDim(Player player, int id)
        {
            try
            {
                if (!Main.Players.ContainsKey(player)) return;
                if (!Group.CanUseCmd(player, "checkdim")) return;

                var target = Main.GetPlayerByID(id);
                if (target == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок с таким ID не найден", 3000);
                    return;
                }

                if (!Main.Players.ContainsKey(target)) return;
                GameLog.Admin($"{player.Name}", $"checkDim", $"{target.Name}");
                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Измерение игрока - {target.Dimension.ToString()}", 4000);
            }
            catch (Exception e)
            {
                Log.Write("checkdim: " + e.StackTrace, nLog.Type.Error);
            }
        }

        [Command("setbizmafia", Hide = true)] // Выдать бизнес под контроль мафии (6 лвл)
        public static void CMD_setBizMafia(Player player, int mafia)
        {
            try
            {
                if (!Main.Players.ContainsKey(player)) return;
                if (!Group.CanUseCmd(player, "setbizmafia")) return;
                if (player.GetMyData<int>("BIZ_ID") == -1) return;
                if (mafia < 10 || mafia > 13) return;

                Business biz = BusinessManager.BizList[player.GetMyData<int>("BIZ_ID")];
                biz.Mafia = mafia;
                biz.UpdateLabel();
                GameLog.Admin($"{player.Name}", $"setBizMafia({biz.ID},{mafia})", $"");
                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"{mafia} теперь владеет бизнесом №{biz.ID}", 3000);
            }
            catch (Exception e) { Log.Write("setbizmafia: " + e.StackTrace, nLog.Type.Error); }
        }

        [Command("newsimcard", Hide = true)] // Новая симкарта с номером для игрока (8 лвл)
        public static void CMD_newsimcard(Player player, int id, int newnumber)
        {
            try
            {
                if (!Main.Players.ContainsKey(player)) return;
                if (!Group.CanUseCmd(player, "newsimcard")) return;

                var target = Main.GetPlayerByID(id);
                if (target == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок с таким ID не найден", 3000);
                    return;
                }

                if (!Main.Players.ContainsKey(target)) return;
                if (Main.SimCards.ContainsKey(newnumber))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Такой номер уже существует", 3000);
                    return;
                }

                Main.SimCards.Remove(newnumber);
                Main.SimCards.Add(newnumber, Main.Players[target].UUID);
                Main.Players[target].Sim = newnumber;
                GUI.Dashboard.sendStats(target);
                GameLog.Admin($"{player.Name}", $"newsim({newnumber})", $"{target.Name}");
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Новый номер для {target.Name} = {newnumber}", 3000);
            }
            catch (Exception e) { Log.Write("newsimcard: " + e.StackTrace, nLog.Type.Error); }
        }

        [Command("takeoffbiz", Hide = true)] // Отобрать бизнес у игрока (8 лвл)
        public static void CMD_takeOffBusiness(Player admin, int bizid, bool byaclear = false)
        {
            try
            {
                if (!Main.Players.ContainsKey(admin)) return;
                if (!Group.CanUseCmd(admin, "takeoffbiz")) return;

                var biz = BusinessManager.BizList[bizid];
                var owner = biz.Owner;
                var player = NAPI.Player.GetPlayerFromName(owner);

                if (player != null && Main.Players.ContainsKey(player))
                {
                    Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, $"Администратор отобрал у Вас бизнес", 3000);
                    MoneySystem.Wallet.Change(player, Convert.ToInt32(biz.SellPrice * 0.8));
                    Main.Players[player].BizIDs.Remove(biz.ID);
                }
                else
                {
                    var split = biz.Owner.Split('_');
                    //var data = MySQL.QueryRead($"SELECT biz,money FROM characters WHERE firstname='{split[0]}' AND lastname='{split[1]}'");
                    MySqlCommand cmd = new MySqlCommand();
                    cmd.CommandText = "SELECT `biz`, `money` FROM `characters` WHERE `firstname`=@firstname AND `lastname`=@lastname";

                    cmd.Parameters.AddWithValue("@firstname", split[0]);
                    cmd.Parameters.AddWithValue("@lastname", split[1]);
                    var data = MySQL.QueryRead(cmd);

                    List<int> ownerBizs = new List<int>();
                    var money = 0;

                    foreach (DataRow Row in data.Rows)
                    {
                        ownerBizs = JsonConvert.DeserializeObject<List<int>>(Row["biz"].ToString());
                        money = Convert.ToInt32(Row["money"]);
                    }

                    ownerBizs.Remove(biz.ID);
                    //MySQL.Query($"UPDATE characters SET biz='{JsonConvert.SerializeObject(ownerBizs)}',money={money + Convert.ToInt32(biz.SellPrice * 0.8)} WHERE firstname='{split[0]}' AND lastname='{split[1]}'");

                    MySqlCommand cmd2 = new MySqlCommand();
                    cmd2.CommandText = "UPDATE `characters` SET `biz`=@biz, `money`=@money WHERE `firstname`=@firstname AND `lastname`=@lastname";

                    cmd2.Parameters.AddWithValue("@biz", JsonConvert.SerializeObject(ownerBizs));
                    cmd2.Parameters.AddWithValue("@money", money + Convert.ToInt32(biz.SellPrice * 0.8));
                    cmd2.Parameters.AddWithValue("@firstname", split[0]);
                    cmd2.Parameters.AddWithValue("@lastname", split[1]);
                    MySQL.Query(cmd2);
                }
                var newOrders = new Dictionary<int, int>(BusinessManager.Orders);
                foreach (var item in Core.BusinessManager.Orders)
                {
                    if (item.Value == bizid)
                    {
                        newOrders.Remove(item.Key);
                    }
                }
                Core.BusinessManager.Orders = newOrders;
                MoneySystem.Bank.Accounts[biz.BankID].Balance = 0;

                biz.Owner = "Государство";
                biz.UpdateLabel();
                GameLog.Money($"server", $"player({Main.PlayerUUIDs[owner]})", Convert.ToInt32(biz.SellPrice * 0.8), $"takeoffBiz({biz.ID})");
                Notify.Send(admin, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы отобрали бизнес у {owner}", 3000);
                if (!byaclear) GameLog.Admin($"{player.Name}", $"takeoffBiz({biz.ID})", $"");
            }
            catch (Exception e) { Log.Write("takeoffbiz: " + e.StackTrace, nLog.Type.Error); }
        }

        [Command("paydaymultiplier", Hide = true)] // Установить множитель на PaydayMultiplier (8 лвл)
        public static void CMD_paydaymultiplier(Player player, int multi)
        {
            try
            {
                if (!Main.Players.ContainsKey(player)) return;
                if (!Group.CanUseCmd(player, "paydaymultiplier")) return;
                if (multi < 1 || multi > 5)
                {
                    Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Возможно установить только от 1 до 5", 3000);
                    return;
                }

                Main.oldconfig.PaydayMultiplier = multi;
                Configuration.saveServerSettings(Main.oldconfig.ExpMultiplier, Main.oldconfig.PaydayMultiplier);

                GameLog.Admin($"{player.Name}", $"paydayMultiplier({multi})", $"");
                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"PaydayMultiplier изменен на {multi}", 3000);
            }
            catch (Exception e) { Log.Write("paydaymultiplier: " + e.StackTrace, nLog.Type.Error); }
        }

        [Command("expmultiplier", Hide = true)] // Установить множитель на ExpMultiplier (7 лвл)
        public static void CMD_expmultiplier(Player player, int multi)
        {
            try
            {
                if (!Main.Players.ContainsKey(player)) return;
                if (!Group.CanUseCmd(player, "expmultiplier")) return;
                if (multi < 1 || multi > 5)
                {
                    Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Возможно установить только от 1 до 5", 3000);
                    return;
                }

                Main.oldconfig.ExpMultiplier = multi;
                Configuration.saveServerSettings(Main.oldconfig.ExpMultiplier, Main.oldconfig.PaydayMultiplier);

                GameLog.Admin($"{player.Name}", $"expMultiplier({multi})", $"");
                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"ExpMultiplier изменен на {multi}", 3000);
            }
            catch (Exception e) { Log.Write("paydaymultiplier: " + e.StackTrace, nLog.Type.Error); }
        }

        [Command("reloadsettings", Hide = true)] // Перезагрузить данные из базы в конфиги (10 лвл)
        public static void CMD_reloadsettings(Player player)
        {
            try
            {
                if (!Main.Players.ContainsKey(player)) return;
                if (!Group.CanUseCmd(player, "reloadsettings")) return;

                Configuration.loadServerSettings();
                GameLog.Admin($"{player.Name}", $"reloadsettings", $"");
                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Настройки успешно загружены из базы", 3000);
            }
            catch (Exception e) { Log.Write("reloadsettings: " + e.StackTrace, nLog.Type.Error); }
        }

        [Command("offdelfrac", Hide = true)] // Уволить игрока из фракции оффлайн (5 лвл)
        public static void CMD_offlineDelFraction(Player player, string name)
        {
            try
            {
                if (!Main.Players.ContainsKey(player)) return;
                if (!Group.CanUseCmd(player, "offdelfrac")) return;

                var split = name.Split('_');

                Fraction frac = new Fraction();
                frac.UnInvite();

                //MySQL.Query($"UPDATE `characters` SET fraction='{JsonConvert.SerializeObject(frac)}',fractionlvl=0 WHERE firstname='{split[0]}' AND lastname='{split[1]}'");
                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = "UPDATE `characters` SET `fraction`=@fraction, `fractionlvl`=@fractionlvl WHERE `firstname`=@firstname AND `lastname`=@lastname";

                cmd.Parameters.AddWithValue("@fraction", JsonConvert.SerializeObject(frac));
                cmd.Parameters.AddWithValue("@fractionlvl", 0);
                cmd.Parameters.AddWithValue("@firstname", split[0]);
                cmd.Parameters.AddWithValue("@lastname", split[1]);
                MySQL.Query(cmd);

                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы уволили игрока {name} из Вашей фракции", 3000);

                int index = Fractions.Manager.AllMembers.FindIndex(m => m.Name == name);
                if (index > -1) Fractions.Manager.AllMembers.RemoveAt(index);

                GameLog.Admin($"{player.Name}", $"delfrac", $"{name}");
                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы выгнали из фракции игрока {name}", 3000);
            }
            catch (Exception e) { Log.Write("offdelfrac: " + e.StackTrace, nLog.Type.Error); }
        }

        //[Command("removeobj")] // ??? (8 лвл)
        //public static void CMD_removeObject(Player player)
        //{
        //    try
        //    {
        //        if (!Main.Players.ContainsKey(player)) return;
        //        if (!Group.CanUseCmd(player, "removeobj")) return;

        //        player.SetMyData("isRemoveObject", true);
        //        Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Следующий подобранный предмет будет в бане", 3000);
        //    }
        //    catch (Exception e) { Log.Write("removeobj: " + e.StackTrace, nLog.Type.Error); }
        //}

        [Command("unwarn", Hide = true)] // Снять варн у игрока (3 лвл)
        public static void CMD_unwarn(Player player, int id)
        {
            try
            {
                if (!Main.Players.ContainsKey(player)) return;
                if (!Group.CanUseCmd(player, "unwarn")) return;

                var target = Main.GetPlayerByID(id);
                if (target == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок с таким ID не найден", 3000);
                    return;
                }

                if (!Main.Players.ContainsKey(target)) return;
                if (Main.Players[target].Warns <= 0)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У игрока нет варнов", 3000);
                    return;
                }

                Main.Players[target].Warns--;
                GUI.Dashboard.sendStats(target);

                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы сняли варн у игрока {target.Name}, у него {Main.Players[target].Warns} варнов", 3000);
                Notify.Send(target, NotifyType.Info, NotifyPosition.BottomCenter, $"У вас сняли варн, осталось {Main.Players[target].Warns} варнов", 3000);
                GameLog.Admin($"{player.Name}", $"unwarn", $"{target.Name}");
            }
            catch (Exception e) { Log.Write("unwarn: " + e.StackTrace, nLog.Type.Error); }
        }

        [Command("offunwarn", Hide = true)] // Снять варн у игрока в оффлайне (3 лвл)
        public static void CMD_offunwarn(Player player, string target)
        {
            try
            {
                if (!Main.Players.ContainsKey(player)) return;
                if (!Group.CanUseCmd(player, "offunwarn")) return;

                if (!Main.PlayerNames.ContainsValue(target))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок не найден", 3000);
                    return;
                }
                if (NAPI.Player.GetPlayerFromName(target) != null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок онлайн", 3000);
                    return;
                }

                var split = target.Split('_');
                //var data = MySQL.QueryRead($"SELECT warns FROM characters WHERE firstname='{split[0]}' AND lastname='{split[1]}'");

                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = "SELECT `warns` FROM `characters` WHERE `firstname`=@firstname AND `lastname`=@lastname";

                cmd.Parameters.AddWithValue("@firstname", split[0]);
                cmd.Parameters.AddWithValue("@lastname", split[1]);
                var data = MySQL.QueryRead(cmd);

                var warns = 0;
                foreach (System.Data.DataRow Row in data.Rows)
                {
                    warns = Convert.ToInt32(Row["warns"]);
                }

                if (warns <= 0)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У игрока нет варнов", 3000);
                    return;
                }

                warns--;
                GameLog.Admin($"{player.Name}", $"offUnwarn", $"{target}");
                //MySQL.Query($"UPDATE characters SET warns={warns} WHERE firstname='{split[0]}' AND lastname='{split[1]}'");

                MySqlCommand cmd2 = new MySqlCommand();
                cmd2.CommandText = "UPDATE `characters` SET `warns`=@warns WHERE `firstname`=@firstname AND `lastname`=@lastname";

                cmd2.Parameters.AddWithValue("@warns", warns);
                cmd2.Parameters.AddWithValue("@firstname", split[0]);
                cmd2.Parameters.AddWithValue("@lastname", split[1]);
                MySQL.Query(cmd2);

                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы сняли варн у игрока {target}, у него {warns} варнов", 3000);
            }
            catch (Exception e) { Log.Write("offunwarn: " + e.StackTrace, nLog.Type.Error); }
        }

        [Command("rescar", Hide = true)] // Респавн авто игрока (3 лвл)
        public static void CMD_respawnCar(Player player)
        {
            try
            {
                if (!Main.Players.ContainsKey(player)) return;
                if (!Group.CanUseCmd(player, "rescar")) return;
                if (!player.IsInVehicle) return;
                var vehicle = player.Vehicle;

                if (!vehicle.HasData("ACCESS"))
                    return;
                else if (vehicle.GetData<string>("ACCESS") == "PERSONAL")
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "На данный момент функция восстановления личной машины игрока отключена", 3000);
                else if (vehicle.GetData<string>("ACCESS") == "WORK")
                    Admin.RespawnWorkCar(vehicle);
                else if (vehicle.GetData<string>("ACCESS") == "FRACTION")
                    Admin.RespawnFractionCar(vehicle);
                else if (vehicle.GetData<string>("ACCESS") == "GANGDELIVERY" || vehicle.GetData<string>("ACCESS") == "MAFIADELIVERY")
                    NAPI.Entity.DeleteEntity(vehicle);

                GameLog.Admin($"{player.Name}", $"rescar", $"");
            }
            catch (Exception e) { Log.Write("ResCar: " + e.StackTrace, nLog.Type.Error); }
        }

        [Command("bansync", Hide = true)] // Синхронизация банов ??? (3 лвл)
        public static void CMD_banlistSync(Player client)
        {
            try
            {
                if (!Group.CanUseCmd(client, "bansync")) return;
                Notify.Send(client, NotifyType.Warning, NotifyPosition.BottomCenter, "Начинаю процедуру синхронизации...", 4000);
                Ban.Sync();
                Notify.Send(client, NotifyType.Success, NotifyPosition.BottomCenter, "Процедура завершена!", 3000);
            }
            catch (Exception e) { Log.Write("bansync: " + e.StackTrace, nLog.Type.Error); }
        }

        [Command("setcolour", Hide = true)] // Смена цвета территорий у банд (6 лвл)
        public static void CMD_setTerritoryColor(Player player, int gangid)
        {
            try
            {
                if (!Main.Players.ContainsKey(player)) return;
                if (!Group.CanUseCmd(player, "setcolour")) return;

                if (player.GetMyData<int>("GZ") == -1)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы не находитесь ни на одном из регионов", 3000);
                    return;
                }
                var terrid = player.GetMyData<int>("GZ");

                if (!client.Fractions.Gangs.GangsCapture.gangPointsColor.ContainsKey(gangid))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Банды с таким ID нет", 3000);
                    return;
                }

                client.Fractions.Gangs.GangsCapture.gangPoints[terrid].GangOwner = gangid;
                Main.ClientEventToAll("setZoneColor", client.Fractions.Gangs.GangsCapture.gangPoints[terrid].ID, client.Fractions.Gangs.GangsCapture.gangPointsColor[gangid]);
                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Теперь территорией №{terrid} владеет {Fractions.Manager.FractionNames[gangid]}", 3000);
                GameLog.Admin($"{player.Name}", $"setColour({terrid},{gangid})", $"");
            }
            catch (Exception e) { Log.Write("CMD_SetColour: " + e.StackTrace, nLog.Type.Error); }
        }

        [Command("sc", Hide = true)] // Выдать одежду (8 лвл)
        public static void CMD_setClothes(Player player, int id, int draw, int texture)
        {
            try
            {
                if (!Main.Players.ContainsKey(player)) return;
                if (!Group.CanUseCmd(player, "sc")) return;
                player.SetClothes(id, draw, texture);
                if (id == 11) player.SetClothes(3, Customization.CorrectTorso[Main.Players[player].Gender][draw], 0);
                if (id == 1) Customization.SetMask(player, draw, texture);
            }
            catch (Exception e){ Log.Write(e.StackTrace, nLog.Type.Error); }
        }

        [Command("sac", Hide = true)] // Выдать аксы (8 лвл)
        public static void CMD_setAccessories(Player player, int id, int draw, int texture)
        {
            if (!Main.Players.ContainsKey(player)) return;
            if (!Group.CanUseCmd(player, "sac")) return;
            if (draw > -1)
                player.SetAccessories(id, draw, texture);
            else
                player.ClearAccessory(id);

        }

        [Command("checkwanted", Hide = true)] // Узнать розыск игрока (8 лвл)
        public static void CMD_checkwanted(Player player, int id)
        {
            if (!Main.Players.ContainsKey(player)) return;
            if (!Group.CanUseCmd(player, "checkwanted")) return;
            var target = Main.GetPlayerByID(id);
            if (target == null)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Человек с таким ID не найден", 3000);
                return;
            }
            var stars = (Main.Players[target].WantedLVL == null) ? 0 : Main.Players[target].WantedLVL.Level;
            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Уровень розыска - {stars}", 3000);
        }

        [Command("fixcar", Hide = true)] // Починить авто (3 лвл)
        public static void CMD_fixcar(Player player)
        {
            try
            {
                if (!Group.CanUseCmd(player, "fixcar")) return;
                if (!player.IsInVehicle) return;
                VehicleManager.RepairCar(player.Vehicle);
            }
            catch (Exception e)
            {
                Log.Write("EXCEPTION AT \"CMD_fixcar\":" + e.ToString(), nLog.Type.Error);
            }
        }

        [Command("stats", Hide = true)] // Статистика игрока (2 лвл)
        public static void CMD_showPlayerStats(Player admin, int id)
        {
            try
            {
                if (!Group.CanUseCmd(admin, "stats")) return;

                if (Main.GetPlayerByID(id) == null)
                {
                    Notify.Send(admin, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок с таким ID не найден", 3000);
                    return;
                }

                Player player = Main.GetPlayerByID(id);
                if (player == admin) return;
                var acc = Main.Players[player];
                string status =
                    (acc.AdminLVL >= 1) ? "Администратор" :
                    (Main.Players[player].VipLvl > 0) ? $"{Group.GroupNames[Main.Players[player].VipLvl]} до {Main.Players[player].VipDate.ToString("dd.MM.yyyy")}" :
                    $"{Group.GroupNames[Main.Players[player].VipLvl]}";

                long bank = (acc.Bank != 0) ? MoneySystem.Bank.Accounts[acc.Bank].Balance : 0;

                var lic = "";
                for (int i = 0; i < acc.Licenses.Count; i++)
                    if (acc.Licenses[i]) lic += $"{Main.LicWords[i]} / ";
                if (lic == "") lic = "Отсутствуют";

                string work = (acc.WorkID > 0) ? Jobs.JobManager.JobListNames[acc.WorkID - 1] : "Безработный";
                string fraction = (acc.Fraction.FractionID > 0) ? Fractions.Manager.FractionNames[acc.Fraction.FractionID] : "Нет";

                var number = (acc.Sim == -1) ? "Нет сим-карты" : Main.Players[player].Sim.ToString();

                List<object> data = new List<object>
                {
                    acc.LVL,
                    $"{acc.EXP}/{3 + acc.LVL * 3}",
                    number,
                    status,
                    0,
                    acc.Warns,
                    lic,
                    acc.CreateDate.ToString("dd.MM.yyyy"),
                    acc.UUID,
                    acc.Bank,
                    work,
                    fraction,
                    acc.Fraction.FractionRankID,
                };

                string json = JsonConvert.SerializeObject(data);
                Trigger.ClientEvent(admin, "board", 2, json);
                admin.SetMyData("CHANGE_WITH", player);
                GUI.Dashboard.OpenOut(admin, nInventory.Items[acc.UUID], player.Name, 20);
            }
            catch (Exception e)
            {
                Log.Write("EXCEPTION AT \"CMD_showPlayerStats\":" + e.ToString(), nLog.Type.Error);
            }
        }

        [Command("admins", Hide = true)] // Список администраторов онлайн (2 лвл)
        public static void CMD_AllAdmins(Player client)
        {
            try
            {
                if (!Group.CanUseCmd(client, "admins")) return;

                client.SendChatMessage("=== ADMINS ONLINE ===");
                foreach (var p in Main.Players)
                {
                    if (p.Value.AdminLVL < 1) continue;
                    client.SendChatMessage($"[{p.Key.Value}] {p.Key.Name} - {p.Value.AdminLVL}");
                }
                client.SendChatMessage("=== ADMINS ONLINE ===");

            }
            catch (Exception e)
            {
                Log.Write("EXCEPTION AT \"CMD_AllAdmins\":" + e.ToString(), nLog.Type.Error);
            }
        }

        [Command("fixweaponsshops", Hide = true)] // Фикс ??? (8 лвл)
        public static void CMD_fixweaponsshops(Player client)
        {
            try
            {
                if (!Group.CanUseCmd(client, "fixweaponsshops")) return;

                foreach (var biz in BusinessManager.BizList.Values)
                {
                    if (biz.Type != 6) continue;
                    biz.Products = BusinessManager.fillProductList(6);

                    //var result = MySQL.QueryRead($"SELECT * FROM `weapons` WHERE id={biz.ID}");

                    MySqlCommand cmd = new MySqlCommand();
                    cmd.CommandText = "SELECT * FROM `weapons` WHERE `id`=@id";

                    cmd.Parameters.AddWithValue("@id", biz.ID);
                    var result = MySQL.QueryRead(cmd);

                    if (result != null) continue;

                    //MySQL.Query($"INSERT INTO weapons (id,lastserial) VALUES ({biz.ID},0)");
                    MySqlCommand cmd2 = new MySqlCommand();
                    cmd2.CommandText = "INSERT INTO `weapons` SET `id`=@id, `lastserial`=@lastserial";

                    cmd2.Parameters.AddWithValue("@id", biz.ID);
                    cmd2.Parameters.AddWithValue("@lastserial", 0);
                    MySQL.Query(cmd2);

                    Log.Debug($"Insert into weapons new business ({biz.ID})");
                }
            }
            catch (Exception e)
            {
                Log.Write("EXCEPTION AT \"CMD_fixweaponsshops\":\n" + e.ToString(), nLog.Type.Error);
            }
        }

        [Command("fixbizpricesbytype", Hide = true)] // Фикс цен в базе по типу бизнеса (8 лвл)
        public static void CMD_fixbizpricesbytype(Player client, int type)
        {
            try
            {
                if (!Group.CanUseCmd(client, "fixbizpricesbytype")) return;

                foreach (var biz in BusinessManager.BizList.Values)
                {
                    if(biz.Type != type) continue;
                    var products = BusinessManager.fillProductList(type);
                    biz.Products = products;

                    //MySQL.Query($"UPDATE `businesses` SET `products`='{JsonConvert.SerializeObject(products)}' WHERE id='{biz.ID}'");
                    MySqlCommand cmd = new MySqlCommand();
                    cmd.CommandText = "UPDATE `businesses` SET `products`=@products WHERE `id`=@id";

                    cmd.Parameters.AddWithValue("@products", JsonConvert.SerializeObject(products));
                    cmd.Parameters.AddWithValue("@id", biz.ID);
                    MySQL.Query(cmd);
                }
            }
            catch (Exception e)
            {
                Log.Write("EXCEPTION AT \"CMD_fixBizPricesByType\":\n" + e.StackTrace, nLog.Type.Error);
            }
        }
        [Command("fixbizprices", Hide = true)] // Фикс цен в базе по типу бизнеса (8 лвл)
        public static void CMD_fixbizprices(Player client)
        {
            try
            {
                if (!Group.CanUseCmd(client, "fixbizprices")) return;

                BusinessManager.fixbizprices();
            }
            catch (Exception e)
            {
                Log.Write("EXCEPTION AT \"CMD_fixBizPricesByType\":\n" + e.StackTrace, nLog.Type.Error);
            }
        }
        [Command("fillgovproducts", Hide = true)] // Пополнение продуктов в базе по типу бизнеса (8 лвл)
        public static void CMD_fillgovproducts(Player client)
        {
            try
            {
                if (!Group.CanUseCmd(client, "fixbizpricesbytype")) return;

                foreach (var biz in BusinessManager.BizList.Values)
                {
                    if (biz.Owner != "Государство") continue;
                    var products = BusinessManager.fillProductList(biz.Type, true);
                    biz.Products = products;

                    //MySQL.Query($"UPDATE `businesses` SET `products`='{JsonConvert.SerializeObject(products)}' WHERE id='{biz.ID}'");

                    MySqlCommand cmd = new MySqlCommand();
                    cmd.CommandText = "UPDATE `businesses` SET `products`=@products WHERE `id`=@id";

                    cmd.Parameters.AddWithValue("@products", JsonConvert.SerializeObject(products));
                    cmd.Parameters.AddWithValue("@id", biz.ID);
                    MySQL.Query(cmd);
                }

                Notify.Send(client, NotifyType.Success, NotifyPosition.BottomCenter, "Вы пополнили гос. бизнесы");
            }
            catch (Exception e)
            {
                Log.Write("EXCEPTION AT \"CMD_fixBizPricesByType\":\n" + e.StackTrace, nLog.Type.Error);
            }
        }

        //[Command("fixgovbizprices")] // Фикс ??? (8 лвл)
        //public static void CMD_fixgovbizprices(Player client)
        //{
        //    try
        //    {
        //        if (!Group.CanUseCmd(client, "fixgovbizprices")) return;

        //        foreach (var biz in BusinessManager.BizList.Values)
        //        {
        //            foreach (var p in biz.Products)
        //            {
        //                if (p.Name == "Расходники" || biz.Type == 7 || biz.Type == 11 || biz.Type == 12 || p.Name == "Татуировки" || p.Name == "Парики" || p.Name == "Патроны") continue;
        //                p.Price = BusinessManager.
        //
        //
        //                [p.Name];
        //            }
        //            biz.UpdateLabel();
        //        }

        //        foreach (var biz in BusinessManager.BizList.Values)
        //        {
        //            if (biz.Owner != "Государство") continue;
        //            foreach (var p in biz.Products)
        //            {
        //                if (p.Name == "Расходники") continue;
        //                double price = (biz.Type == 7 || biz.Type == 11 || biz.Type == 12 || p.Name == "Татуировки" || p.Name == "Парики" || p.Name == "Патроны") ? 125 : (biz.Type == 1) ? 6 : BusinessManager.ProductsOrderPrice[p.Name] * 1.25;
        //                p.Price = Convert.ToInt32(price);
        //                p.Lefts = Convert.ToInt32(BusinessManager.ProductsCapacity[p.Name] * 0.1);
        //            }
        //            biz.UpdateLabel();
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Log.Write("EXCEPTION AT \"CMD_fixgovbizprices\":\n" + e.ToString(), nLog.Type.Error);
        //    }
        //}

        [Command("selc")]
        public static void SelectCharacter(Player player)
        {
            player.TriggerEvent("selectCharacter");
        }

        [Command("fillproductbyindex", Hide = true)] // Выдать продукты в бизнес (8 лвл)
        public static void CMD_setproductbyindex(Player client, int id, int index, int product)
        {
            try
            {
                if (!Group.CanUseCmd(client, "setproductbyindex")) return;

                var biz = BusinessManager.BizList[id];
                biz.Products[index].Lefts = product;
            }
            catch (Exception e)
            {
                Log.Write("EXCEPTION AT \"CMD_fillproductbyindex\":\n" + e.ToString(), nLog.Type.Error);
            }
        }

        [Command("deleteproducts", Hide = true)] // Удалить продукты в бизнесе (8 лвл)
        public static void CMD_deleteproducts(Player client, int id)
        {
            try
            {
                if (!Group.CanUseCmd(client, "deleteproducts")) return;

                var biz = BusinessManager.BizList[id];
                foreach (var p in biz.Products)
                    p.Lefts = 0;
            }
            catch (Exception e)
            {
                Log.Write("EXCEPTION AT \"CMD_setproductbyindex\":\n" + e.ToString(), nLog.Type.Error);
            }
        }

        [Command("changebizprice", Hide = true)] // Изменить цену бизнеса (8 лвл)
        public static void CMD_changeBusinessPrice(Player player, int newPrice)
        {
            if (!Group.CanUseCmd(player, "changebizprice")) return;
            if (player.GetMyData<int>("BIZ_ID") == -1)
            {
                player.SendChatMessage("~r~Вы должны находиться на одном из бизнесов");
                return;
            }
            Business biz = BusinessManager.BizList[player.GetMyData<int>("BIZ_ID")];
            biz.SellPrice = newPrice;
            biz.UpdateLabel();
        }

        [Command("pa", Hide = true)] // Выполнить анимацию (8 лвл)
        public static void CMD_playAnimation(Player player, string dict, string anim, int flag)
        {
            if (!Group.CanUseCmd(player, "pa")) return;
            player.PlayAnimation(dict, anim, flag);
        }

        [Command("sa", Hide = true)] // Остановить анимацию (8 лвл)
        public static void CMD_stopAnimation(Player player)
        {
            if (!Group.CanUseCmd(player, "sa")) return;
            player.StopAnimation();
        }

        [Command("tpc", Hide = true)] // Телепорт по координатам (3 лвл)
        public static void CMD_tpCoord(Player player, double x, double y, double z)
        {
            if (!Group.CanUseCmd(player, "tpc")) return;
            NAPI.Entity.SetEntityPosition(player, new Vector3(x, y, z));
        }

        //[Command("ripl", Hide = true)]
        //public static void RequestIpl(Player player, string name)
        //{
        //    if (!Group.CanUseCmd(player, "tpc")) return;
        //    player.TriggerEvent("requestIpl", name);
        //}

        //[Command("dipl", Hide = true)]
        //public static void RemoveIpl(Player player, string name)
        //{
        //    if (!Group.CanUseCmd(player, "tpc")) return;
        //    player.TriggerEvent("removeIpl", name);
        //}

        [Command("tpp", GreedyArg = true, Hide = true)]
        public static void CMD_tpAtCoordString(Player player, string coords)
        {
            if (!Group.CanUseCmd(player, "tpp")) return;
            double[] elem = coords.Split(',', ' ').
                      Where(x => !string.IsNullOrWhiteSpace(x)).
                      Select(x => double.Parse(x.Replace('.', ','))).ToArray();
            if (elem.Length != 3) return;

            NAPI.Entity.SetEntityPosition(player, new Vector3(elem[0], elem[1], elem[2]));
        }

        [Command("inv", Hide = true)] // Невидимость (2 лвл)
        public static void CMD_ToogleInvisible(Player player)
        {
            if (!Main.Players.ContainsKey(player)) return;
            if (!Group.CanUseCmd(player, "inv")) return;

            BasicSync.SetInvisible(player, !BasicSync.GetInvisible(player));
        }

        [Command("delfrac", Hide = true)] // Удалить игрока из фракции (3 лвл)
        public static void CMD_delFrac(Player player, int id)
        {
            if (!Main.Players.ContainsKey(player)) return;
            if (Main.GetPlayerByID(id) == null)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок с таким ID не найден", 3000);
                return;
            }
            Admin.delFrac(player, Main.GetPlayerByID(id));
        }

        [Command("sendcreator", Hide = true)] // Отправить игрока в редактор внешности (5 лвл)
        public static void CMD_SendToCreator(Player player, int id)
        {
            if (!Main.Players.ContainsKey(player)) return;
            if (!Group.CanUseCmd(player, "sendcreator")) return;
            var target = Main.GetPlayerByID(id);
            if (target == null)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок с таким ID не найден", 3000);
                return;
            }
            Customization.SendToCreator(target);
            GameLog.Admin($"{player.Name}", $"sendCreator", $"{target.Name}");
        }

        [Command("afuel", Hide = true)] // Заправить авто (2 лвл)
        public static void CMD_setVehiclePetrol(Player player, int fuel)
        {
            try
            {
                if (!Group.CanUseCmd(player, "afuel")) return;
                if (!player.IsInVehicle) return;
                player.Vehicle.SetSharedData("PETROL", fuel);
                GameLog.Admin($"{player.Name}", $"afuel({fuel})", $"");
            }
            catch (Exception e) { Log.Write("afuel: " + e.StackTrace, nLog.Type.Error); }
        }

        [Command("changename", GreedyArg = true, Hide = true)] // Изменить имя игрока (3 лвл)
        public static void CMD_changeName(Player client, string curient, string newName)
        {
            try
            {
                if (!Group.CanUseCmd(client, "changename")) return;
                if (!Main.PlayerNames.ContainsValue(curient)) return;

                try
                {
                    string[] split = newName.Split("_");
                    Log.Debug($"SPLIT: {split[0]} {split[1]}");
                }
                catch (Exception e)
                {
                    Log.Write("ERROR ON CHANGENAME COMMAND\n" + e.ToString());
                }


                if (Main.PlayerNames.ContainsValue(newName))
                {
                    Notify.Send(client, NotifyType.Error, NotifyPosition.BottomCenter, "Такое имя уже существует!", 3000);
                    return;
                }

                Player target = NAPI.Player.GetPlayerFromName(curient);
                Character.Character.toChange.Add(curient, newName);

                if (target == null || target.IsNull)
                {
                    Notify.Send(client, NotifyType.Alert, NotifyPosition.BottomCenter, "Игрок оффлайн, меняем...", 3000);
                    Task changeTask = Character.Character.changeName(curient);
                }
                else
                {
                    Notify.Send(client, NotifyType.Alert, NotifyPosition.BottomCenter, "Игрок онлайн, кикаем...", 3000);
                    NAPI.Player.KickPlayer(target);
                }

                Notify.Send(client, NotifyType.Success, NotifyPosition.BottomCenter, "Ник изменен!", 3000);
                GameLog.Admin($"{client.Name}", $"changeName({newName})", $"{curient}");

            }
            catch (Exception e)
            {
                Log.Write("EXCEPTION AT \"CMD_CHANGENAME\":\n" + e.ToString(), nLog.Type.Error);
            }
        }

        [Command("startmatwars", Hide = true)] // Начать войну за маты (6 лвл)
        public static void CMD_startMatWars(Player player)
        {
            try
            {
                if (!Group.CanUseCmd(player, "startmatwars")) return;
                if (client.Fractions.Utils.MatsWar.isWar)
                {
                    player.SendChatMessage("~r~Война за маты уже идёт");
                    return;
                }
                client.Fractions.Utils.MatsWar.startMatWarTimer();
                player.SendChatMessage("~r~Начата война за маты");
                GameLog.Admin($"{player.Name}", $"startMatwars", $"");
            }
            catch (Exception e) { Log.Write("startmatwars: " + e.StackTrace, nLog.Type.Error); }
        }

        [Command("fixodejdi", Hide = true)] //
        public static void CMD_448Fix(Player player)
        {
          try
          {
            if (!Group.CanUseCmd(player, "fixodejdi")) return;
            foreach (var item in nInventory.Items)
            {
              foreach (var invItem in item.Value)
              {
                try
                {
                  if (invItem.Data.ToString().Contains("448_") && invItem.Type == ItemType.Top)
                  {
                    invItem.Data = "3_0_True";
                    Player player1 = Main.GetPlayerByUUID(item.Key);
                    if (player1 != null)
                    {
                      Wallet.Change(player1, 50000);
                      DonateShop.FixInventory(player1);
                      Dashboard.sendItems(player1);
                      Dashboard.sendStats(player1);
                    }
                    else
                    {
                      var custom = Customization.CustomPlayerData[item.Key];

                      custom.Clothes.Top = new ComponentItem(3, 0);
                            MySqlCommand cmd2 = new MySqlCommand();
                      cmd2.CommandText = "UPDATE `characters` SET " +
                        "`money`=`money` + @money" +
                        " WHERE `uuid`=@uuid;";

                      cmd2.Parameters.AddWithValue("@money", 50000);
                      cmd2.Parameters.AddWithValue("@uuid", item.Key);
                      MySQL.Query(cmd2);
                    }
                  }
                }catch(Exception ex) { Console.WriteLine(ex.StackTrace);}
              }
            }
          }
          catch (Exception e) { Log.Write("startmatwars: " + e.StackTrace, nLog.Type.Error); }
        }
        [Command("whitelistdel", Hide = true)] // Удалить игрока из WhiteList (8 лвл)
        public static void CMD_whitelistdel(Player player, string socialClub)
        {
            if (!Group.CanUseCmd(player, "whitelistdel")) return;

            try
            {
                if (CheckSocialClubInWhiteList(socialClub))
                {
                    //MySQL.Query("DELETE FROM `whiteList` WHERE `socialclub` = '" + socialClub + "';");

                    MySqlCommand cmd = new MySqlCommand();
                    cmd.CommandText = "DELETE FROM `whiteList` WHERE `socialclub`=@socialclub";

                    cmd.Parameters.AddWithValue("@socialclub", socialClub);
                    MySQL.Query(cmd);

                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Social club успешно удален из white list!", 3000);
                }
                else
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Данный social club не найден в white list!", 3000);
                }
                GameLog.Admin($"{player.Name}", $"whitelistdel", $"");
            }
            catch (Exception e) { Log.Write("whitelistdel: " + e.StackTrace, nLog.Type.Error); }
        }
        public static bool CheckSocialClubInWhiteList(string SocialClub)
        {
            DataTable data = MySQL.QueryRead($"SELECT * FROM `whiteList` WHERE 1");

            foreach (DataRow Row in data.Rows)
            {
                if (Row["socialclub"].ToString() == SocialClub)
                {
                    return true;
                }
            }
            return false;
        }

        [Command("whitelistadd", Hide = true)] // Добавить игрока из WhiteList (8 лвл)
        public static void CMD_whitelistadd(Player player, string socialClub)
        {
            if (!Group.CanUseCmd(player, "whitelistadd")) return;

            try
            {
                if (CheckSocialClubInAccounts(socialClub))
                {
                    if (!CheckSocialClubInWhiteList(socialClub))
                    {
                        //MySQL.Query("INSERT INTO `whiteList` (`socialclub`) VALUES ('" + socialClub + "');");
                        MySqlCommand cmd = new MySqlCommand();
                        cmd.CommandText = "INSERT INTO `whiteList` SET `socialclub`=@socialclub";

                        cmd.Parameters.AddWithValue("@socialclub", socialClub);
                        MySQL.Query(cmd);

                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Social club успешно добавлен в white list!", 3000);
                    }
                    else
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Данный social club уже состоит в white list!", 3000);
                    }
                }
                else
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Данный social club не найден!", 3000);
                }
                GameLog.Admin($"{player.Name}", $"whitelistadd", $"");
            }
            catch (Exception e) { Log.Write("whitelistadd: " + e.StackTrace, nLog.Type.Error); }
        }
        public static bool CheckSocialClubInAccounts(string SocialClub)
        {
            DataTable data = MySQL.QueryRead($"SELECT * FROM `accounts` WHERE 1");
            foreach (DataRow Row in data.Rows)
            {
                if (Row["socialclub"].ToString() == SocialClub)
                {
                    return true;
                }
            }
            return false;
        }

        [Command("giveexp", Hide = true)] // Выдать EXP игроку (6 лвл)
        public static void CMD_giveExp(Player player, int id, int exp)
        {
            try
            {
                if (!Group.CanUseCmd(player, "giveexp")) return;
                var target = Main.GetPlayerByID(id);
                if (target == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок с таким ID не найден", 3000);
                    return;
                }

                Utils.giveExp(target, exp);
            }
            catch (Exception e) { Log.Write("giveexp" + e.StackTrace, nLog.Type.Error); }
        }

        [Command("housetypeprice", Hide = true)] // установить единую цену выбранному типу домов (8 лвл)
        public static void CMD_replaceHousePrices(Player player, int type, int newPrice)
        {
            if (!Group.CanUseCmd(player, "housetypeprice")) return;
            foreach (var h in Houses.HouseManager.Houses)
            {
                if (h.Type != type) continue;
                if (h.IsHideForContract) continue;
                h.Price = newPrice;
                h.UpdateLabel();
                h.Save();
            }
        }

        [Command("delhouseowner", Hide = true)] // Удалить владельца у дома (8 лвл)
        public static void CMD_deleteHouseOwner(Player player)
        {
            if (!Group.CanUseCmd(player, "delhouseowner")) return;
            if (!player.HasMyData("HOUSEID"))
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы должны находиться на маркере дома", 3000);
                return;
            }

            Houses.House house = Houses.HouseManager.Houses.FirstOrDefault(h => h.ID == player.GetMyData<int>("HOUSEID"));
            if (house == null) return;
            if (house.IsHideForContract) return;
            house.SetOwner(null);
            house.UpdateLabel();
            house.Save();
            GameLog.Admin($"{player.Name}", $"delHouseOwner({house.ID})", $"");
        }

        [Command("stt", Hide = true)] // Буст машины / ускорение (6 лвл)
        public static void CMD_SetTurboTorque(Player player, float power, float torque)
        {
            try
            {
                if (!Group.CanUseCmd(player, "stt")) return;
                if (!player.IsInVehicle) return;
                Trigger.ClientEvent(player, "svem", power, torque);
            }
            catch (Exception e)
            {
                Log.Write("Error at \"STT\":" + e.ToString(), nLog.Type.Error);
            }
        }
        [Command("driftmode", Hide = true)] // Буст машины / ускорение (6 лвл)
        public static void CMD_driftmode(Player player)
        {
            try
            {
                if (!Group.CanUseCmd(player, "driftmode")) return;
                if (!player.IsInVehicle) return;
                Trigger.ClientEvent(player, "driftmode");
            }
            catch (Exception e)
            {
                Log.Write("Error at \"STT\":" + e.ToString(), nLog.Type.Error);
            }
        }
        [Command("svm", Hide = true)] // Модификация авто / тюнинг (8 лвл)
        public static void CMD_SetVehicleMod(Player player, int type, int index)
        {
            try
            {
                if (!Group.CanUseCmd(player, "svm")) return;
                if (!player.IsInVehicle) return;
                player.Vehicle.SetMod(type, index);

            }
            catch (Exception e)
            {
                Log.Write("Error at \"SVM\":" + e.ToString(), nLog.Type.Error);
            }
        }

        [Command("svn", Hide = true)] // Установить неон на авто (8 лвл)
        public static void CMD_SetVehicleNeon(Player player, byte r, byte g, byte b, byte alpha)
        {
            try
            {
                if (!Group.CanUseCmd(player, "svn")) return;
                if (!player.IsInVehicle) return;
                Vehicle v = player.Vehicle;
                if (alpha != 0)
                {
                    NAPI.Vehicle.SetVehicleNeonState(v, true);
                    NAPI.Vehicle.SetVehicleNeonColor(v, r, g, b);
                }
                else
                {
                    NAPI.Vehicle.SetVehicleNeonColor(v, 255, 255, 255);
                    NAPI.Vehicle.SetVehicleNeonState(v, false);
                }
            }
            catch (Exception e)
            {
                Log.Write("Error at \"SVN\":" + e.ToString(), nLog.Type.Error);
            }
        }

        [Command("svhid", Hide = true)] // Установить цвет фар на авто (8 лвл)
        public static void CMD_SetVehicleHeadlightColor(Player player, int hlcolor)
        {
            try
            {
                if (!Group.CanUseCmd(player, "svhid")) return;
                if (!player.IsInVehicle) return;
                Vehicle v = player.Vehicle;
                if (hlcolor >= 0 && hlcolor <= 12)
                {
                    v.SetSharedData("hlcolor", hlcolor);
                    Trigger.ClientEventInRange(v.Position, 250f, "VehStream_SetVehicleHeadLightColor", v.Handle, hlcolor);
                }
                else Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Цвет фар может быть от 0 до 12.", 3000);
            }
            catch (Exception e)
            {
                Log.Write("Error at \"SVN\":" + e.ToString(), nLog.Type.Error);
            }
        }

        [Command("svh", Hide = true)] // Установить здоровье транспорту / починить (8 лвл)
        public static void CMD_SetVehicleHealth(Player player, int health = 100)
        {
            try
            {
                if (!Group.CanUseCmd(player, "svh")) return;
                if (!player.IsInVehicle) return;
                Vehicle v = player.Vehicle;
                v.Repair();
                v.Health = health;

            }
            catch (Exception e)
            {
                Log.Write("Error at \"SVH\":" + e.ToString(), nLog.Type.Error);
            }

        }

        [Command("delacars", Hide = true)] // Удалить все созданные админские авто (5 лвл)
        public static void CMD_deleteAdminCars(Player player)
        {
            try
            {
                NAPI.Task.Run(() =>
                {
                    try
                    {
                        if (!Group.CanUseCmd(player, "delacars")) return;
                        foreach (var v in NAPI.Pools.GetAllVehicles())
                        {
                            if (v.HasData("ACCESS") && v.GetData<string>("ACCESS") == "ADMIN")
                                v.Delete();
                        }
                        GameLog.Admin($"{player.Name}", $"delacars", $"");
                    }
                    catch (Exception e){ Log.Write(e.StackTrace, nLog.Type.Error); }
                });
            }
            catch (Exception e) { Log.Write("delacars: " + e.StackTrace, nLog.Type.Error); }
        }

        [Command("delacar", Hide = true)] // Удалить конкретное админское авто (3 лвл)
        public static void CMD_deleteThisAdminCar(Player client)
        {
            if (!Group.CanUseCmd(client, "delacar")) return;
            if (!client.IsInVehicle) return;
            Vehicle veh = client.Vehicle;
            if (veh.HasData("ACCESS") && veh.GetData<string>("ACCESS") == "ADMIN")
                veh.Delete();
            GameLog.Admin($"{client.Name}", $"delacar", $"");
        }

        [Command("delmycars", Hide = true)] // Удалить все свои админские авто (3 лвл)
        public static void CMD_delMyCars(Player client)
        {
            try
            {
                NAPI.Task.Run(() =>
                {
                    try
                    {
                        if (!Group.CanUseCmd(client, "delmycars")) return;
                        foreach (var v in NAPI.Pools.GetAllVehicles())
                        {
                            if (v.HasData("ACCESS") && v.GetData<string>("ACCESS") == "ADMIN")
                            {
                                if (v.GetData<string>("BY") == client.Name)
                                    v.Delete();
                            }
                        }
                        GameLog.Admin($"{client.Name}", $"delmycars", $"");
                    }
                    catch (Exception e){ Log.Write(e.StackTrace, nLog.Type.Error); }
                });
            }
            catch (Exception e) { Log.Write("delacars: " + e.StackTrace, nLog.Type.Error); }
        }

        [Command("allspawncar", Hide = true)] // Зареспавнить все авто (8 лвл)
        public static void CMD_allSpawnCar(Player player)
        {
            Admin.respawnAllCars(player);
        }

        [Command("save", Hide = true)] // Сохранить координаты (8 лвл)
        public static void CMD_saveCoord(Player player, string name)
        {

            Admin.saveCoords(player, name);
        }

        [Command("namebypersonid", Hide = true)] 
        public static void GetPlayerNameByStaticId(Player player, string id)
        {
            try
            {
                if (!Group.CanUseCmd(player, "namebypersonid")) return;

                var result = MySQL.QueryRead($"SELECT * FROM `characters` WHERE `personid` = '{id}'");
                if (result == null || result.Rows.Count == 0)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Таких персонажей нету");
                    return;
                }
                var name = result.Rows[0]["firstname"] + "_" + result.Rows[0]["lastname"];
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"{id} - {name}");
            }
            catch (Exception e) { Log.Write(e.StackTrace.ToString()); }
        }

        [Command("newrentveh", Hide = true)] // Добавить машину для аренды (8 лвл)
        public static void newrentveh(Player player, string model, string number, int price, int c1, int c2)
        {
            try
            {
                if (!Group.CanUseCmd(player, "newrentveh")) return;
                VehicleHash vh = (VehicleHash)NAPI.Util.GetHashKey(model);
                if (vh == 0) throw null;
                var veh = NAPI.Vehicle.CreateVehicle(vh, player.Position, player.Rotation.Z, 0, 0);
                VehicleStreaming.SetEngineState(veh, true);
                veh.Dimension = player.Dimension;
                MySqlCommand cmd = new MySqlCommand
                {
                    CommandText = "INSERT INTO `othervehicles`(`type`, `number`, `model`, `position`, `rotation`, `color1`, `color2`, `price`) VALUES (@type, @number, @model, @pos, @rot, @c1, @c2, @price);"
                };
                cmd.Parameters.AddWithValue("@type", 0);
                cmd.Parameters.AddWithValue("@price", price);
                cmd.Parameters.AddWithValue("@model", model);
                cmd.Parameters.AddWithValue("@number", number);
                cmd.Parameters.AddWithValue("@c1", c1);
                cmd.Parameters.AddWithValue("@c2", c2);
                cmd.Parameters.AddWithValue("@pos", JsonConvert.SerializeObject(player.Position));
                cmd.Parameters.AddWithValue("@rot", JsonConvert.SerializeObject(player.Rotation));
                MySQL.Query(cmd);
                veh.PrimaryColor = c1;
                veh.SecondaryColor = c2;
                veh.NumberPlate = number;
                player.SendChatMessage("Вы добавили машину для аренды.");
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"newrentveh\":\n" + e.ToString(), nLog.Type.Error); }
        }

        [Command("newjobveh", Hide = true)] // Добавить машину на работу (8 лвл)
        public static void newjobveh(Player player, string typejob, string model, string number, int c1, int c2)
        {
            try
            {
                if (!Group.CanUseCmd(player, "newjobveh")) return;

                if(player.Vehicle == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы должны быть в автомобиле", 3000);
                    return;
                }

                VehicleHash vh = (VehicleHash)NAPI.Util.GetHashKey(model);
                if (vh == 0) throw null;
                int typeIdJob = 999;
                switch (typejob)
                {
                    case "Taxi":
                        typeIdJob = 3;
                        break;
                    case "Bus":
                        typeIdJob = 4;
                        break;
                    case "Lawnmower":
                        typeIdJob = 5;
                        break;
                    case "Truckers":
                        typeIdJob = 6;
                        break;
                    case "Collector":
                        typeIdJob = 7;
                        break;
                    case "AutoMechanic":
                        typeIdJob = 8;
                        break;
                }
                if (typeIdJob == 999)
                {
                    player.SendChatMessage("Выберите один тип работы из: Taxi, Bus, Lawnmower, Truckers, Collector, AutoMechanic");
                    throw null;
                }
                //var veh = NAPI.Vehicle.CreateVehicle(vh, player.Vehicle.Position, player.Vehicle.Rotation, 0, 0);
                //VehicleStreaming.SetEngineState(veh, true);
                //veh.Dimension = player.Dimension;
                MySqlCommand cmd = new MySqlCommand
                {
                    CommandText = "INSERT INTO `othervehicles`(`type`, `number`, `model`, `position`, `rotation`, `color1`, `color2`, `price`) VALUES (@type, @number, @model, @pos, @rot, @c1, @c2, '0');"
                };
                cmd.Parameters.AddWithValue("@type", typeIdJob);
                cmd.Parameters.AddWithValue("@model", model);
                cmd.Parameters.AddWithValue("@number", number);
                cmd.Parameters.AddWithValue("@c1", c1);
                cmd.Parameters.AddWithValue("@c2", c2);
                cmd.Parameters.AddWithValue("@pos", JsonConvert.SerializeObject(player.Vehicle.Position));
                cmd.Parameters.AddWithValue("@rot", JsonConvert.SerializeObject(player.Vehicle.Rotation));
                MySQL.Query(cmd);
                //veh.PrimaryColor = c1;
                //veh.SecondaryColor = c2;
                //veh.NumberPlate = number;
                player.SendChatMessage("Вы добавили рабочую машину.");
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"newjobveh\":\n" + e.ToString(), nLog.Type.Error); }
        }

        [Command("newfracveh", Hide = true)] // Добавить машину для фракции (8 лвл)
        public static void ACMD_newfracveh(Player player, string model, int fracid, string number, int c1, int c2) // add rank, number
        {
            try
            {
                if (!Group.CanUseCmd(player, "newfracveh")) return;
                if (player.Vehicle == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы должны быть в автомобиле", 3000);
                    return;
                }

                VehicleHash vh = (VehicleHash)NAPI.Util.GetHashKey(model);
                if (vh == 0) throw null;
                var veh = NAPI.Vehicle.CreateVehicle(vh, player.Vehicle.Position, player.Vehicle.Rotation, 0, 0);
                VehicleStreaming.SetEngineState(veh, true);
                veh.Dimension = player.Dimension;
                MySqlCommand cmd = new MySqlCommand
                {
                    CommandText = "INSERT INTO `fractionvehicles`(`fraction`, `number`, `components`, `model`, `position`, `rotation`, `rank`, `colorprim`, `colorsec`) VALUES (@idfrac, @number, '{}', @model, @pos, @rot, '1', @c1, @c2);"
                };
                cmd.Parameters.AddWithValue("@idfrac", fracid);
                cmd.Parameters.AddWithValue("@model", model);
                cmd.Parameters.AddWithValue("@number", number);
                cmd.Parameters.AddWithValue("@c1", c1);
                cmd.Parameters.AddWithValue("@c2", c2);
                cmd.Parameters.AddWithValue("@pos", JsonConvert.SerializeObject(player.Vehicle.Position));
                cmd.Parameters.AddWithValue("@rot", JsonConvert.SerializeObject(player.Vehicle.Rotation));
                MySQL.Query(cmd);
                veh.PrimaryColor = c1;
                veh.SecondaryColor = c2;
                veh.NumberPlate = number;
                VehicleManager.FracApplyCustomization(veh, fracid);
                player.SendChatMessage("Вы добавили машину фракции.");
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"ACMD_newfracveh\":\n" + e.ToString(), nLog.Type.Error); }
        }

        [Command("stop", Hide = true)] // Выключить сервер (11 лвл)
        public static void CMD_stopServer(Player player, string text = null)
        {
            Admin.stopServer(player, text);
        }

        [Command("payday", Hide = true)] // Выполнить PAYDAY (9 лвл)
        public static void payDay(Player player, string text = null)
        {
            if (!Group.CanUseCmd(player, "payday")) return;
            GameLog.Admin($"{player.Name}", $"payDay", "");
            Main.payDayTrigger();
        }

        [Command("setleader", Hide = true)] // Назначить игрока лидером (6 лвл)
        public static void CMD_setLeader(Player player, int id, int fracid)
        {
            try
            {
                if (Main.GetPlayerByID(id) == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок с таким ID не найден", 3000);
                    return;
                }
                Admin.setFracLeader(player, Main.GetPlayerByID(id), fracid);
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }

        [Command("sp", Hide = true)] // Следить за игроком (2 лвл)
        public static void CMD_spectateMode(Player player, int id)
        {
            if (!Group.CanUseCmd(player, "sp")) return;
            try
            {
                AdminSP.Spectate(player, id);
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }

        [Command("usp", Hide = true)] // Закончить следить за игроком (2 лвл)
        public static void CMD_unspectateMode(Player player)
        {
            if (!Group.CanUseCmd(player, "sp")) return;
            try
            {
                AdminSP.UnSpectate(player);
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }

        [Command("metp", Hide = true)] // Телепортировать игрока к себе (2 лвл)
        public static void CMD_teleportToMe(Player player, int id)
        {
            try
            {
                if (Main.GetPlayerByID(id) == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок с таким ID не найден", 3000);
                    return;
                }
                Admin.teleportTargetToPlayer(player, Main.GetPlayerByID(id), false);
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }

        [Command("gethere", Hide = true)] // Телепортировать игрока к себе вместе с машиной (2 лвл)
        public static void CMD_teleportVehToMe(Player player, int id)
        {
            try
            {
                if (Main.GetPlayerByID(id) == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок с таким ID не найден", 3000);
                    return;
                }
                Admin.teleportTargetToPlayer(player, Main.GetPlayerByID(id), true);
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }

        [Command("kill", Hide = true)] // Убить игрока (3 лвл)
        public static void CMD_kill(Player player, int id)
        {
            try
            {
                if (Main.GetPlayerByID(id) == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок с таким ID не найден", 3000);
                    return;
                }
                Admin.killTarget(player, Main.GetPlayerByID(id));
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }

        [Command("hp", Hide = true)] // Выдать здоровье игроку (2 лвл)
        public static void CMD_adminHeal(Player player, int id, int hp)
        {
            try
            {
                if (Main.GetPlayerByID(id) == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок с таким ID не найден", 3000);
                    return;
                }
                Admin.healTarget(player, Main.GetPlayerByID(id), hp);
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }

        [Command("eat", Hide = true)] // Выдать еду игроку (2 лвл)
        public static void CMD_adminEat(Player player, int id, int value)
        {
            try
            {
                if (Main.GetPlayerByID(id) == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок с таким ID не найден", 3000);
                    return;
                }
                Admin.eatTarget(player, Main.GetPlayerByID(id), value);
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }
        [Command("playerpaydays", Hide = true)] // Выдать еду игроку (2 лвл)
        public static void CMD_PlayerPayDays(Player player, int id)
        {
          try
          {
            if (Main.GetPlayerByID(id) == null)
            {
              Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок с таким ID не найден", 3000);
              return;
            }
            player.SendChatMessage(Main.Players[Main.GetPlayerByID(id)].PlayerDayGameHours.ToString() + " пейдеев получено.");
          }
          catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }
        [Command("water", Hide = true)] // Выдать воду игроку (2 лвл)
        public static void CMD_adminWater(Player player, int id, int value)
        {
            try
            {
                if (Main.GetPlayerByID(id) == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок с таким ID не найден", 3000);
                    return;
                }
                Admin.waterTarget(player, Main.GetPlayerByID(id), value);
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }

        [Command("ar", Hide = true)] // Выдать броню игроку (8 лвл)
        public static void CMD_adminArmor(Player player, int id, int ar)
        {
            try
            {
                if (Main.GetPlayerByID(id) == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок с таким ID не найден", 3000);
                    return;
                }
                Admin.armorTarget(player, Main.GetPlayerByID(id), ar);
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }

        [Command("fz", Hide = true)] // Заморозить игрока (3 лвл)
        public static void CMD_adminFreeze(Player player, int id)
        {
            try
            {
                if (Main.GetPlayerByID(id) == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок с таким ID не найден", 3000);
                    return;
                }
                Admin.freezeTarget(player, Main.GetPlayerByID(id));
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }

        [Command("ufz", Hide = true)] // Разморозить игрока (3 лвл)
        public static void CMD_adminUnFreeze(Player player, int id)
        {
            try
            {
                if (Main.GetPlayerByID(id) == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок с таким ID не найден", 3000);
                    return;
                }
                Admin.unFreezeTarget(player, Main.GetPlayerByID(id));
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }

        [Command("setadmin", Hide = true)] // Назначить игрока админом (8 лвл)
        public static void CMD_setAdmin(Player player, int id)
        {
            try
            {
                if (Main.GetPlayerByID(id) == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок с таким ID не найден", 3000);
                    return;
                }
                Admin.setPlayerAdminGroup(player, Main.GetPlayerByID(id));
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }

        //DELETE!
        //[Command("giveselfadminfortestandneeddeletethis", Hide = true)] // Назначить себя админом
        //public static void CMD_selfAdmin(Player player)
        //{
        //    try
        //    {
        //        Admin.setSelfAdminGroup(player);
        //    }
        //    catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        //}

        //DELETE!
        [Command("removeadmin", Hide = true)] // Назначить себя админом
        public static void CMD_removeadmin(Player player, int id)
        {
            try
            {
                if (Main.GetPlayerByID(id) == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок с таким ID не найден", 3000);
                    return;
                }

                Admin.delPlayerAdminGroup(player, Main.GetPlayerByID(id), true);
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }

        [Command("lsn", GreedyArg = true, Hide = true)] // Админское объявление (8 лвл)
        public static void CMD_adminLSnewsChat(Player player, string message)
        {
            try
            {
                Admin.adminLSnews(player, message);
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }

        [Command("tpcar", Hide = true)] // Телепорт игрока к себе (в авто) (2 лвл)
        public static void CMD_teleportToMeWithCar(Player player, int id)
        {
            try
            {
                Player Target = Main.GetPlayerByID(id);

                if (Target == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок с таким ID не найден", 3000);
                    return;
                }

                if (!Target.IsInVehicle)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок не в автомобиле", 3000);
                    return;
                }

                Admin.teleportTargetToPlayerWithCar(player, Target);
            }
            catch (Exception e)
            {
                Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error);
            }
        }

        [Command("deladmin", Hide = true)] // Снять полномочия админа с игрока (8 лвл)
        public static void CMD_delAdmin(Player player, int id)
        {
            try
            {
                if (Main.GetPlayerByID(id) == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок с таким ID не найден", 3000);
                    return;
                }
                Admin.delPlayerAdminGroup(player, Main.GetPlayerByID(id));
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }

        [Command("setadminrank", Hide = true)] // Установить админский ранг (8 лвл)
        public static void CMD_setAdminRank(Player player, int id, int rank)
        {
            try
            {
                if (Main.GetPlayerByID(id) == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок с таким ID не найден", 3000);
                    return;
                }
                Admin.setPlayerAdminRank(player, Main.GetPlayerByID(id), rank);
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }

        [Command("guns", Hide = true)] // Выдать оружие игроку  (7 лвл)
        public static void CMD_adminGuns(Player player, int id, string wname, string serial)
        {
            try
            {
                if (Main.GetPlayerByID(id) == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок с таким ID не найден", 3000);
                    return;
                }
                Admin.giveTargetGun(player, Main.GetPlayerByID(id), wname, serial);
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }

        [Command("giveclothes", Hide = true)] // Выдать одежду игроку (7 лвл)
        public static void CMD_adminClothes(Player player, int id, string itemType, string data)
        {
            try
            {
                if (Main.GetPlayerByID(id) == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок с таким ID не найден", 3000);
                    return;
                }
                Admin.giveTargetClothes(player, Main.GetPlayerByID(id), itemType, data);
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }

        [Command("setskin", Hide = true)] // Установить скин игроку (6 лвл)
        public static void CMD_adminSetSkin(Player player, int id, string pedModel)
        {
            try
            {
                if (Main.GetPlayerByID(id) == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок с таким ID не найден", 3000);
                    return;
                }
                Admin.giveTargetSkin(player, Main.GetPlayerByID(id), pedModel);
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }

        [Command("oguns", Hide = true)] // Забрать оружие у игрока (7 лвл)
        public static void CMD_adminOGuns(Player player, int id)
        {
            try
            {
                if (Main.GetPlayerByID(id) == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок с таким ID не найден", 3000);
                    return;
                }
                Admin.takeTargetGun(player, Main.GetPlayerByID(id));
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }

        [Command("givemoney", Hide = true)] // Напечатать денег (8 лвл)
        public static void CMD_adminGiveMoney(Player player, int id, int money)
        {
            try
            {
                if (Main.GetPlayerByID(id) == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок с таким ID не найден", 3000);
                    return;
                }
                Admin.giveMoney(player, Main.GetPlayerByID(id), money);
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }

        [Command("delleader", Hide = true)] // Снять лидера (6 лвл)
        public static void CMD_delleader(Player player, int id)
        {
            try
            {
                if (Main.GetPlayerByID(id) == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок с таким ID не найден", 3000);
                    return;
                }
                Admin.delFracLeader(player, Main.GetPlayerByID(id));
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }

        [Command("deljob", Hide = true)] // Уволить игрока с работы (3 лвл)
        public static void CMD_deljob(Player player, int id)
        {
            try
            {
                if (Main.GetPlayerByID(id) == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок с таким ID не найден", 3000);
                    return;
                }
                Admin.delJob(player, Main.GetPlayerByID(id));
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }

        [Command("vehc", Hide = true)] // Создать авто rgb (3 лвл)
        public static void CMD_createVehicleCustom(Player player, string name, int r, int g, int b)
        {
            try
            {
                if (player == null || !Main.Players.ContainsKey(player)) return;
                if (!Group.CanUseCmd(player, "vehc")) return;
                VehicleHash vh = (VehicleHash)NAPI.Util.GetHashKey(name);
                if (vh == 0) throw null;
                var veh = NAPI.Vehicle.CreateVehicle(vh, player.Position, player.Rotation.Z, 0, 0);
                veh.Dimension = player.Dimension;
                veh.NumberPlate = "ADMIN";
                veh.CustomPrimaryColor = new Color(r, g, b);
                veh.CustomSecondaryColor = new Color(r, g, b);
                veh.SetData("ACCESS", "ADMIN");
                veh.SetData("BY", player.Name);
                VehicleStreaming.SetEngineState(veh, true);
                Log.Debug($"vehc {name} {r} {g} {b}");
                GameLog.Admin($"{player.Name}", $"vehCreate({name})", $"");
            }
            catch (Exception e){ Log.Write(e.StackTrace, nLog.Type.Error); }
        }

        [Command("pos", Hide = true)] // Отобразить текущие координаты (1 лвл)
        public void HandlePos(Player c)
        {
            if (!Group.CanUseCmd(c, "pos")) return;

            Vector3 pos = c.Position;
            Vector3 rot = c.Rotation;

            //Console Anzeige der Positionen//
            //Console.WriteLine("Diese Positionsdaten wurden von " + c.Name + " angefordert:");
            //Console.WriteLine("Position");
            //Console.WriteLine("Pos: " + pos.X + "| " + pos.Y + "| " + pos.Z);
            //Console.WriteLine("Rotation");
            //Console.WriteLine("Z: " + rot.Z);

            //Console.WriteLine("---------------");
            c.SendChatMessage("---------------");

            c.SendChatMessage("Position");
            c.SendChatMessage("Pos: " + pos.X + "| " + pos.Y + "| " + pos.Z);
            c.SendChatMessage("Rotation");
            c.SendChatMessage("Z: " + rot.Z);
        }

        [Command("ped", Hide = true)] // ped sich selber geben / Стать хаскеном (1 лвл)
        public void HandlePad(Player c)
        {
            if (!Group.CanUseCmd(c, "ped")) return;
            c.SetSkin(PedHash.Husky);
        }

        [Command("restart", Hide = true)] // Перезагрузить сервер (9 лвл)
        public void HandleShutDown(Player cc, int second)
        {
            if (!Group.CanUseCmd(cc, "restart")) return;

            if (second < 5 || second > 900)
            {
                cc.SendNotification("Минимум 5 секунд и максимум 900 секунд!");
                return;
            }

            foreach (Player c in NAPI.Pools.GetAllPlayers())
            {
                //saveDatabase()
            }


            NAPI.Chat.SendChatMessageToAll("[~r~SERVER~w~]: Перезагрузка сервера через " + second + " Секунды. [ИСПРАВЛЕНИЕ ОШИБКИ] Пожалуйста, выйдите из системы заранее, чтобы ваши вещи были сохранены!");

            Task.Run(() =>
            {
                Task.Delay(1000 * second * 1).Wait();

                Main.saveDatabase();
                Environment.Exit(0);
            });
        }

        [Command("dim", Hide = true)] // dimension  command / Изменить виртуальный мир написавшего (8 лвл)
        public void HandleTp(Player c, uint d)
        {
            if (!Group.CanUseCmd(c, "dim")) return;
            c.Dimension = d;
        }

        [Command("mtp2", Hide = true)] // Телепорт по координатам(8 лвл)
        public void HandleTp(Player c, double x, double y, double z)
        {
            if (!Group.CanUseCmd(c, "mtp2")) return;
            c.Position = new Vector3(x, y, z);
        }

        [Command("veh", Hide = true)] // Создать авто (3 лвл)
        public static void CMD_createVehicle(Player player, string name, int a, int b)
        {
            try
            {
                if (player == null || !Main.Players.ContainsKey(player)) return;
                if (!Group.CanUseCmd(player, "veh")) return;
                VehicleHash vh = (VehicleHash)NAPI.Util.GetHashKey(name);
                player.SendChatMessage("vh " + vh);
                if (vh == 0)
                {
                    player.SendChatMessage("vh return");
                    return;
                }
                var veh = NAPI.Vehicle.CreateVehicle(vh, player.Position, player.Rotation.Z, 0, 0);
                veh.Dimension = player.Dimension;
                veh.NumberPlate = "ADMIN";
                veh.PrimaryColor = a;
                veh.SecondaryColor = b;
                veh.SetData("ACCESS", "ADMIN");
                veh.SetData("BY", player.Name);
                VehicleStreaming.SetEngineState(veh, true);
                player.SetIntoVehicle(veh, 0);
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD_veh\":\n" + e.ToString(), nLog.Type.Error); }
        }

        [Command("vehpool", Hide = true)] // Создать авто (3 лвл)
        public static void CMD_createVehiclePool(Player player, string name, int count)
        {
            try
            {
                if (player == null || !Main.Players.ContainsKey(player)) return;
                if (!Group.CanUseCmd(player, "veh")) return;
                VehicleHash vh = (VehicleHash)NAPI.Util.GetHashKey(name);
                player.SendChatMessage("vh " + vh);
                if (vh == 0)
                {
                    player.SendChatMessage("vh return");
                    return;
                }

                for (int i = 0; i < count; i++)
                {

                    var veh = NAPI.Vehicle.CreateVehicle(vh, player.Position.Around(10.0f), player.Rotation.Z, 0, 0);
                    veh.Dimension = player.Dimension;
                    veh.NumberPlate = "ADMIN";
                    veh.PrimaryColor = 0;
                    veh.SecondaryColor = 0;
                    veh.SetData("ACCESS", "ADMIN");
                    veh.SetData("BY", player.Name);
                    VehicleStreaming.SetEngineState(veh, true);
                    //player.SetIntoVehicle(veh, 0);
                }
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD_veh\":\n" + e.ToString(), nLog.Type.Error); }
        }

        [Command("vehhash", Hide = true)] // Создать авто по Хэшу (8 лвл)
        public static void CMD_createVehicleHash(Player player, uint name, int a, int b)
        {
            try
            {
                if (player == null || !Main.Players.ContainsKey(player)) return;
                if (!Group.CanUseCmd(player, "vehhash")) return;
                var veh = NAPI.Vehicle.CreateVehicle(name, player.Position, player.Rotation.Z, 0, 0);
                veh.Dimension = player.Dimension;
                veh.NumberPlate = "PROJECT";
                veh.PrimaryColor = a;
                veh.SecondaryColor = b;
                veh.SetData("ACCESS", "ADMIN");
                veh.SetData("BY", player.Name);
                VehicleStreaming.SetEngineState(veh, true);
            }
            catch (Exception e){ Log.Write(e.StackTrace, nLog.Type.Error); }
        }

        [Command("vehs", Hide = true)] // Создать авто ??? (8 лвл)
        public static void CMD_createVehicles(Player player, string name, int a, int b, int count)
        {
            try
            {
                if (player == null || !Main.Players.ContainsKey(player)) return;
                if (!Group.CanUseCmd(player, "vehs")) return;
                VehicleHash vh = (VehicleHash)NAPI.Util.GetHashKey(name);
                if (vh == 0) throw null;
                for (int i = count; i > 0; i--)
                {
                    var veh = NAPI.Vehicle.CreateVehicle(vh, player.Position, player.Rotation.Z, 0, 0);
                    veh.Dimension = player.Dimension;
                    veh.NumberPlate = "ADMIN";
                    veh.PrimaryColor = a;
                    veh.SecondaryColor = b;
                    veh.SetData("ACCESS", "ADMIN");
                    veh.SetData("BY", player.Name);
                    VehicleStreaming.SetEngineState(veh, true);
                }
                GameLog.Admin($"{player.Name}", $"vehsCreate({name})", $"");
            }
            catch (Exception e){ Log.Write(e.StackTrace, nLog.Type.Error); }
        }

        [Command("vehcs", Hide = true)] // Создать авто ??? (8 лвл)
        public static void CMD_createVehicleCustoms(Player player, string name, int r, int g, int b, int count)
        {
            try
            {
                if (player == null || !Main.Players.ContainsKey(player)) return;
                if (!Group.CanUseCmd(player, "vehcs")) return;
                VehicleHash vh = (VehicleHash)NAPI.Util.GetHashKey(name);
                if (vh == 0) throw null;
                for (int i = count; i > 0; i--)
                {
                    var veh = NAPI.Vehicle.CreateVehicle(vh, player.Position, player.Rotation.Z, 0, 0);
                    veh.Dimension = player.Dimension;
                    veh.NumberPlate = "ADMIN";
                    veh.CustomPrimaryColor = new Color(r, g, b);
                    veh.CustomSecondaryColor = new Color(r, g, b);
                    veh.SetData("ACCESS", "ADMIN");
                    veh.SetData("BY", player.Name);
                    VehicleStreaming.SetEngineState(veh, true);
                    Log.Debug($"vehc {name} {r} {g} {b}");
                }
                GameLog.Admin($"{player.Name}", $"vehsCreate({name})", $"");
            }
            catch (Exception e){ Log.Write(e.StackTrace, nLog.Type.Error); }
        }

        [Command("vehcustompcolor", Hide = true)] // Кастомная покраска на авто (8 лвл)
        public static void CMD_ApplyCustomPColor(Player client, int r, int g, int b, int mod = -1)
        {
            try
            {
                if (!Main.Players.ContainsKey(client)) return;
                if (!Group.CanUseCmd(client, "vehcustompcolor")) return;
                Color color = new Color(r, g, b);

                var number = client.Vehicle.NumberPlate;
                var id = client.Vehicle.GetData<int>("ID");

                VehicleManager.Vehicles[id].Components.PrimColor = color;
                VehicleManager.Vehicles[id].Components.PrimModColor = mod;

                VehicleManager.ApplyCustomization(client.Vehicle);

            }
            catch (Exception e){ Log.Write(e.StackTrace, nLog.Type.Error); }
        }

        [Command("aclear", Hide = true)] // Очистить аккаунт игрока (9 лвл)
        public static void ACMD_aclear(Player player, string target)
        {
            try
            {
                if (!Main.Players.ContainsKey(player)) return;
                if (!Group.CanUseCmd(player, "aclear")) return;
                if (!Main.PlayerNames.ContainsValue(target))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Игрок не найден", 3000);
                    return;
                }
                if (NAPI.Player.GetPlayerFromName(target) != null)
                {
                    Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, "Невозможно очистить персонажа, который находится в игре", 3000);
                    return;
                }
                string[] split = target.Split('_');
                int tuuid = 0;


                // CLEAR BIZ

                #region Bussiness
                //DataTable result = MySQL.QueryRead($"SELECT uuid,adminlvl,biz FROM `characters` WHERE firstname='{split[0]}' AND lastname='{split[1]}'");
                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = "SELECT `uuid`, `adminlvl`, `biz` FROM `characters` WHERE `firstname`=@firstname AND `lastname`=@lastname";

                cmd.Parameters.AddWithValue("@firstname", split[0]);
                cmd.Parameters.AddWithValue("@lastname", split[1]);
                DataTable result = MySQL.QueryRead(cmd);

                if (result != null && result.Rows.Count != 0)
                {
                    DataRow row = result.Rows[0];
                    if (Convert.ToInt32(row["adminlvl"]) >= Main.Players[player].AdminLVL)
                    {
                        SendToAdmins(3, $"!{{#d35400}}[ACLEAR-DENIED] {player.Name} ({player.Value}) попытался очистить {target} (offline), который имеет выше уровень администратора.");
                        return;
                    }
                    tuuid = Convert.ToInt32(row["uuid"]);
                    List<int> TBiz = JsonConvert.DeserializeObject<List<int>>(row["biz"].ToString());

                    if (TBiz.Count >= 1 && TBiz[0] >= 1)
                    {
                        var biz = BusinessManager.BizList[TBiz[0]];
                        var owner = biz.Owner;
                        var ownerplayer = NAPI.Player.GetPlayerFromName(owner);

                        if (ownerplayer != null && Main.Players.ContainsKey(player))
                        {
                            Notify.Send(ownerplayer, NotifyType.Warning, NotifyPosition.BottomCenter, $"Администратор отобрал у Вас бизнес", 3000);
                            MoneySystem.Wallet.Change(ownerplayer, Convert.ToInt32(biz.SellPrice * 0.8));
                            Main.Players[ownerplayer].BizIDs.Remove(biz.ID);
                        }
                        else
                        {
                            var split1 = biz.Owner.Split('_');
                            //var data = MySQL.QueryRead($"SELECT biz,money FROM characters WHERE firstname='{split1[0]}' AND lastname='{split1[1]}'");
                            MySqlCommand cmd2 = new MySqlCommand();
                            cmd2.CommandText = "SELECT `biz`, `money` FROM `characters` WHERE `firstname`=@firstname AND `lastname`=@lastname";

                            cmd2.Parameters.AddWithValue("@firstname", split[0]);
                            cmd2.Parameters.AddWithValue("@lastname", split[1]);
                            var data = MySQL.QueryRead(cmd2);

                            List<int> ownerBizs = new List<int>();
                            var money = 0;

                            foreach (DataRow Row in data.Rows)
                            {
                                ownerBizs = JsonConvert.DeserializeObject<List<int>>(Row["biz"].ToString());
                                money = Convert.ToInt32(Row["money"]);
                            }

                            ownerBizs.Remove(biz.ID);
                            //MySQL.Query($"UPDATE characters SET biz='{JsonConvert.SerializeObject(ownerBizs)}',money={money + Convert.ToInt32(biz.SellPrice * 0.8)} WHERE firstname='{split1[0]}' AND lastname='{split1[1]}'");

                            MySqlCommand cmd3 = new MySqlCommand();
                            cmd3.CommandText = "UPDATE `characters` SET `biz`=@biz, `money`=@money WHERE `firstname`=@firstname AND `lastname`=@lastname";

                            cmd3.Parameters.AddWithValue("@biz", JsonConvert.SerializeObject(ownerBizs));
                            cmd3.Parameters.AddWithValue("@money", money + Convert.ToInt32(biz.SellPrice * 0.8));
                            cmd3.Parameters.AddWithValue("@firstname", split[0]);
                            cmd3.Parameters.AddWithValue("@lastname", split[1]);
                            MySQL.Query(cmd3);
                        }

                        MoneySystem.Bank.Accounts[biz.BankID].Balance = 0;
                        biz.Owner = "Государство";
                        biz.UpdateLabel();
                        Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы отобрали бизнес у {owner}", 3000);
                    }
                }
                else
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Не удалось найти персонажа в базе данных", 3000);
                    return;
                }
                #endregion

                // CLEAR HOUSE

                #region House
                //result = MySQL.QueryRead($"SELECT id FROM `houses` WHERE `owner`='{target}'");

                MySqlCommand cmd4 = new MySqlCommand();
                cmd4.CommandText = "SELECT `id` FROM `houses` WHERE `owner`=@owner";

                cmd4.Parameters.AddWithValue("@owner", target);
                result = MySQL.QueryRead(cmd4);


                if (result != null && result.Rows.Count != 0)
                {
                    DataRow row = result.Rows[0];
                    int hid = Convert.ToInt32(row[0]);
                    Houses.House house = Houses.HouseManager.Houses.FirstOrDefault(h => h.ID == hid);
                    if (house != null)
                    {
                        house.SetOwner(null);
                        house.UpdateLabel();
                        house.Save();
                        Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы отобрали дом у {target}", 3000);

                        MySqlCommand cmd13 = new MySqlCommand();
                        cmd13.CommandText = "UPDATE `houses` SET `items`=@items, `slots`=@slots WHERE `id`=@id";

                        cmd13.Parameters.AddWithValue("@items", "[]");
                        cmd13.Parameters.AddWithValue("@slots", "[true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true]");
                        cmd13.Parameters.AddWithValue("@id", hid);
                        MySQL.Query(cmd13);
                    }
                }
                #endregion

                // CLEAR VEHICLES

                #region Vehicles
                //result = MySQL.QueryRead($"SELECT `id` FROM `vehicles` WHERE `drugs`='{target}'");
                MySqlCommand cmd5 = new MySqlCommand();
                cmd5.CommandText = "SELECT `id` FROM `vehicles` WHERE `holder`=@holder OR `ownerid` = @ownerid";

                cmd5.Parameters.AddWithValue("@holder", target);
                cmd5.Parameters.AddWithValue("@ownerid", tuuid);
                result = MySQL.QueryRead(cmd5);

                if (result != null && result.Rows.Count != 0)
                {
                    DataRowCollection rows = result.Rows;
                    foreach (DataRow row in rows)
                    {
                        VehicleManager.Remove(Convert.ToInt32(row[0]));
                    }
                    Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы отобрали у {target} все машины.", 3000);
                }

                MySqlCommand cmd9 = new MySqlCommand();
                cmd9.CommandText = "DELETE FROM `resalecars` WHERE `uuid`=@uuid";

                cmd9.Parameters.AddWithValue("@uuid", tuuid);
                result = MySQL.QueryRead(cmd9);

                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы отобрали у {target} все машины на продаже в ResaleCars.", 3000);

                #endregion

                // CLEAR MONEY, HOTEL, FRACTION, SIMCARD, PET
                //MySQL.Query($"UPDATE `characters` SET `money`=0,`fraction`=0,`fractionlvl`=0,`hotel`=-1,`hotelleft`=0,`sim`=-1,`PetName`='null' WHERE firstname='{split[0]}' AND lastname='{split[1]}'");
                Fraction frac = new Fraction();
                frac.UnInvite();

                MySqlCommand cmd6 = new MySqlCommand();
                cmd6.CommandText = "UPDATE `characters` SET " +
                "`money`=@money, " +
                //"`bp_information`=@bp_information, " +
                "`currentWeight`=@currentWeight, " +
                "`MaxWeight`=@MaxWeight, " +
                "`viplvl`=@viplvl, " +
                "`fraction`=@fraction, " +
                "`fractionlvl`=@fractionlvl, " +
                "`hotel`=@hotel, " +
                "`hotelleft`=@hotelleft, " +
                "`sim`=@sim, " +
                "`sim_balance`=@sim_balance, " +
                "`PetName`=@PetName" +
                " WHERE `firstname`=@firstname AND `lastname`=@lastname";

                cmd6.Parameters.AddWithValue("@money", 0);
                cmd6.Parameters.AddWithValue("@fraction", JsonConvert.SerializeObject(frac));
                cmd6.Parameters.AddWithValue("@fractionlvl", 0);
                cmd6.Parameters.AddWithValue("@hotel", -1);
                cmd6.Parameters.AddWithValue("@hotelleft", 0);
                cmd6.Parameters.AddWithValue("@sim", -1);
                cmd6.Parameters.AddWithValue("@sim_balance", 0);
                cmd6.Parameters.AddWithValue("@PetName", null);
                cmd6.Parameters.AddWithValue("@currentWeight", "0");
                cmd6.Parameters.AddWithValue("@MaxWeight", "10");
                cmd6.Parameters.AddWithValue("@viplvl", 0);
                //cmd6.Parameters.AddWithValue("@bp_information", "null");
                cmd6.Parameters.AddWithValue("@firstname", split[0]);
                cmd6.Parameters.AddWithValue("@lastname", split[1]);
                MySQL.Query(cmd6);

                MySqlCommand cmd10 = new MySqlCommand();
                cmd10.CommandText = "UPDATE `accounts` SET `redbucks`=@redbucks, `bpcoins`=@bpcoins WHERE `character1`=@uuid OR `character2`=@uuid OR `character3`=@uuid";

                cmd10.Parameters.AddWithValue("@redbucks", 0);
                cmd10.Parameters.AddWithValue("@bpcoins", 0);
                cmd10.Parameters.AddWithValue("@uuid", tuuid);
                MySQL.Query(cmd10);

                MySqlCommand cmd11 = new MySqlCommand();
                cmd11.CommandText = "DELETE FROM `bp__rewards` WHERE `uuid`=@uuid";

                cmd11.Parameters.AddWithValue("@uuid", tuuid);
                MySQL.Query(cmd11);

                MySqlCommand cmd12 = new MySqlCommand();
                cmd12.CommandText = "DELETE FROM `roulette` WHERE `uuid`=@uuid";

                cmd12.Parameters.AddWithValue("@uuid", tuuid);
                MySQL.Query(cmd12);

                // CLEAR BANK MONEY
                Bank.Data bankAcc = Bank.Accounts.FirstOrDefault(a => a.Value.Holder == target).Value;
                if (bankAcc != null)
                {
                    bankAcc.Balance = 0;
                    //MySQL.Query($"UPDATE `money` SET `balance`=0 WHERE `holder`='{target}'");

                    MySqlCommand cmd7 = new MySqlCommand();
                    cmd7.CommandText = "UPDATE `money` SET `balance`=@balance WHERE `holder`=@holder";

                    cmd7.Parameters.AddWithValue("@balance", 0);
                    cmd7.Parameters.AddWithValue("@holder", target);
                    MySQL.Query(cmd7);
                }

                // CLEAR ITEMS
                if (tuuid != 0) {
                    MySqlCommand cmd8 = new MySqlCommand();
                    cmd8.CommandText = "UPDATE `inventory` SET `items`=@items, `slots`=@slots WHERE `uuid`=@uuid";

                    List<bool> tempslots = new List<bool>();
                    for (int i = 0; i < 20; i++)
                    {
                        tempslots.Add(true);
                    }
                    
                    if (nInventory.Items.ContainsKey(tuuid)) nInventory.Items[tuuid] = new List<nItem>();
                    if (nInventory.ItemsSlots.ContainsKey(tuuid)) nInventory.ItemsSlots[tuuid] = tempslots;

                    //cmd8.Parameters.AddWithValue("@items", "[]");
                    //cmd8.Parameters.AddWithValue("@slots", "[true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true]");
                    cmd8.Parameters.AddWithValue("@items", JsonConvert.SerializeObject(new List<nItem>()));
                    cmd8.Parameters.AddWithValue("@slots", JsonConvert.SerializeObject(tempslots));
                    cmd8.Parameters.AddWithValue("@uuid", tuuid);
                    MySQL.Query(cmd8);
                }

                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы забрали у игрока все вещи, деньги с рук и банковского счёта у {target}", 3000);
                GameLog.Admin($"{player.Name}", $"aClear", $"{target}");
            }
            catch (Exception e) { Log.Write("EXCEPTION AT aclear\n" + e.ToString(), nLog.Type.Error); }
        }

        [Command("vehcustomscolor", Hide = true)] // Кастомная покраска на авто ??? (8 лвл)
        public static void CMD_ApplyCustomSColor(Player client, int r, int g, int b, int mod = -1)
        {
            try
            {
                if (!Main.Players.ContainsKey(client)) return;
                if (!Group.CanUseCmd(client, "vehcustomscolor")) return;
                Color color = new Color(r, g, b);

                var number = client.Vehicle.NumberPlate;
                var id = client.Vehicle.GetData<int>("ID");

                VehicleManager.Vehicles[id].Components.SecColor = color;
                VehicleManager.Vehicles[id].Components.SecModColor = mod;

                VehicleManager.ApplyCustomization(client.Vehicle);

            }
            catch (Exception e){ Log.Write(e.StackTrace, nLog.Type.Error); }
        }

        [Command("findbyveh", Hide = true)] // Найти авто по номеру (8 лвл)
        public static void CMD_FindByVeh(Player player, string number)
        {
            if (!Main.Players.ContainsKey(player)) return;
            if (!Group.CanUseCmd(player, "findbyveh")) return;
            if (number.Length > 8)
            {
                Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, "Количество символов в номерном знаке не может превышать 8.", 3000);
                return;
            }

            //if (VehicleManager.Vehicles.ContainsKey(number)) Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Номер машины: {number} | Модель: {VehicleManager.Vehicles[number].Model} | Владелец: {VehicleManager.Vehicles[number].Holder}", 6000);
            //else Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Не найдено машины с таким номерным знаком.", 3000);
        }

        [Command("vehcustom", Hide = true)] // ??? (8 лвл)
        public static void CMD_ApplyCustom(Player client, int cat = -1, int id = -1)
        {
            try
            {
                if (!Main.Players.ContainsKey(client)) return;
                if (!Group.CanUseCmd(client, "vehcustom")) return;

                if (!client.IsInVehicle) return;

                if (cat < 0)
                {
                    Core.VehicleManager.ApplyCustomization(client.Vehicle);
                    return;
                }

                var number = client.Vehicle.GetData<int>("ID");

                switch (cat)
                {
                    case 0:
                        VehicleManager.Vehicles[number].Components.Muffler = id;
                        break;
                    case 1:
                        VehicleManager.Vehicles[number].Components.SideSkirt = id;
                        break;
                    case 2:
                        VehicleManager.Vehicles[number].Components.Hood = id;
                        break;
                    case 3:
                        VehicleManager.Vehicles[number].Components.Spoiler = id;
                        break;
                    case 4:
                        VehicleManager.Vehicles[number].Components.Lattice = id;
                        break;
                    case 5:
                        VehicleManager.Vehicles[number].Components.Wings = id;
                        break;
                    case 101:
                        VehicleManager.Vehicles[number].Components.RearWings = id;
                        break;
                    case 6:
                        VehicleManager.Vehicles[number].Components.Roof = id;
                        break;
                    case 7:
                        VehicleManager.Vehicles[number].Components.Vinyls = id;
                        break;
                    case 8:
                        VehicleManager.Vehicles[number].Components.FrontBumper = id;
                        break;
                    case 9:
                        VehicleManager.Vehicles[number].Components.RearBumper = id;
                        break;
                    case 10:
                        VehicleManager.Vehicles[number].Components.Engine = id;
                        break;
                    case 11:
                        VehicleManager.Vehicles[number].Components.Turbo = id;
                        var turbo = (VehicleManager.Vehicles[number].Components.Turbo == 0);
                        client.Vehicle.SetSharedData("TURBO", turbo);
                        break;
                    case 12:
                        VehicleManager.Vehicles[number].Components.Horn = id;
                        break;
                    case 13:
                        VehicleManager.Vehicles[number].Components.Transmission = id;
                        break;
                    case 14:
                        VehicleManager.Vehicles[number].Components.WindowTint = id;
                        break;
                    case 15:
                        VehicleManager.Vehicles[number].Components.Suspension = id;
                        break;
                    case 16:
                        VehicleManager.Vehicles[number].Components.Brakes = id;
                        break;
                    case 17:
                        VehicleManager.Vehicles[number].Components.Headlights = id;
                        break;
                    case 18:
                        VehicleManager.Vehicles[number].Components.NumberPlate = id;
                        break;
                    case 19:
                        VehicleManager.Vehicles[number].Components.NeonColor.Red = id;
                        break;
                    case 20:
                        VehicleManager.Vehicles[number].Components.NeonColor.Green = id;
                        break;
                    case 21:
                        VehicleManager.Vehicles[number].Components.NeonColor.Blue = id;
                        break;
                    case 22:
                        VehicleManager.Vehicles[number].Components.NeonColor.Alpha = id;
                        break;
                    case 23:
                        VehicleManager.Vehicles[number].Components.WheelsType = id;
                        break;
                    case 24:
                        VehicleManager.Vehicles[number].Components.Wheels = id;
                        break;
                    case 25:
                        VehicleManager.Vehicles[number].Components.WheelsColor = id;
                        break;
                }

                Core.VehicleManager.ApplyCustomization(client.Vehicle);
            }
            catch (Exception e){ Log.Write(e.StackTrace, nLog.Type.Error); }
        }

        [Command("sw", Hide = true)] // Установить погоду (6 лвл)
        public static void CMD_setWeatherID(Player player, byte weather)
        {
            if (!Group.CanUseCmd(player, "sw")) return;
            Main.changeWeather(weather);
            GameLog.Admin($"{player.Name}", $"setWeather({weather})", $"");
        }


        [Command("st", Hide = true)] // Установить время (9 лвл)
        public static void CMD_setTime(Player player, int hours, int minutes, int seconds)
        {
            if (!Group.CanUseCmd(player, "st")) return;
            NAPI.World.SetTime(hours, minutes, seconds);
        }

        [Command("fst", Hide = true)] // Заморозить время (9 лвл)
        public static void CMD_FreezeTime(Player player)
        {
            if (!Group.CanUseCmd(player, "st")) return;
            Main.freezedTime = !Main.freezedTime;
        }

        [Command("tp", Hide = true)] // Телепорт к игроку (2 лвл)
        public static void CMD_teleport(Player player, int id)
        {
            try
            {
                if (!Group.CanUseCmd(player, "tp")) return;

                var target = Main.GetPlayerByID(id);
                if (target == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок с таким ID не найден", 3000);
                    return;
                }
                NAPI.Entity.SetEntityPosition(player, target.Position + new Vector3(1, 0, 1.5));
                NAPI.Entity.SetEntityDimension(player, NAPI.Entity.GetEntityDimension(target));
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }

        [Command("goto", Hide = true)] // Телепорт к игроку вместе с машиной (2 лвл)
        public static void CMD_teleportveh(Player player, int id)
        {
            try
            {
                if (!Group.CanUseCmd(player, "goto")) return;
                if (!player.IsInVehicle) return;
                var target = Main.GetPlayerByID(id);
                if (target == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок с таким ID не найден", 3000);
                    return;
                }
                NAPI.Entity.SetEntityDimension(player.Vehicle, NAPI.Entity.GetEntityDimension(target));
                NAPI.Entity.SetEntityPosition(player.Vehicle, target.Position + new Vector3(2, 2, 2));
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }

        [Command("flip", Hide = true)] // Перевернуть авто (2 лвл)
        public static void CMD_flipveh(Player player, int id)
        {
            try
            {
                if (!Group.CanUseCmd(player, "flip")) return;
                Player target = Main.GetPlayerByID(id);
                if (target == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок с таким ID не найден", 3000);
                    return;
                }
                if (!target.IsInVehicle)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок не находится в машине", 3000);
                    return;
                }
                NAPI.Entity.SetEntityPosition(target.Vehicle, target.Vehicle.Position + new Vector3(0, 0, 2.5f));
                NAPI.Entity.SetEntityRotation(target.Vehicle, new Vector3(0, 0, target.Vehicle.Rotation.Z));
                GameLog.Admin($"{player.Name}", $"flipVeh", $"{target.Name}");
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }

        [Command("mtp", Hide = true)] // Очередной телепорт ??? (8 лвл)
        public static void CMD_maskTeleport(Player player, int id)
        {
            try
            {
                if (!Group.CanUseCmd(player, "mtp")) return;

                if (!Main.MaskIds.ContainsKey(id))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Маска с таким ID не найдена", 3000);
                    return;
                }
                var target = Main.MaskIds[id];

                NAPI.Entity.SetEntityPosition(player, target.Position);
                NAPI.Entity.SetEntityDimension(player, NAPI.Entity.GetEntityDimension(target));
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }

        [Command("createbusiness", Hide = true)] // Создать бизнес (9 лвл)
        public static void CMD_createBiz(Player player, int govPrice, int type)
        {
            try
            {
                BusinessManager.createBusinessCommand(player, govPrice, type);
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }

        [Command("createunloadpoint", Hide = true)] // Создать точку разгрузки у бизнеса (8 лвл)
        public static void CMD_createUnloadPoint(Player player, int bizid)
        {
            try
            {
                BusinessManager.createBusinessUnloadpoint(player, bizid);
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }

        [Command("createmanagepoint", Hide = true)] // Создать точку разгрузки у бизнеса (8 лвл)
        public static void CMD_createManagePoint(Player player, int bizid)
        {
            try
            {
                BusinessManager.createBusinessManagepoint(player, bizid);
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }

        [Command("deletemanagepoint", Hide = true)] // Создать точку разгрузки у бизнеса (8 лвл)
        public static void CMD_deleteManagePoint(Player player, int bizid)
        {
            try
            {
                BusinessManager.deleteBusinessManagepoint(player, bizid);
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }

        [Command("createrentpoint", Hide = true)] // Создать точку аренды (8 лвл)
        public static void CMD_createRentPoint(Player player, int type, int timer, int color1, int color2)
        {
            try
            {
                RentcarSystem.createRentPoint(player, type, timer, color1, color2);
            }
            catch (Exception e) { Log.Write("EXCEPTION AT createrentpoint \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }

        [Command("createrentspawnpoint", Hide = true)] // Создать точку появления арендованого транспорта (8 лвл)
        public static void CMD_createSpawnPoint(Player player, int id)
        {
            try
            {
                RentcarSystem.createRentSpawnPoint(player, id);
            }
            catch (Exception e) { Log.Write("EXCEPTION AT createrentpoint \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }

        [Command("deleterentspawnpoint", Hide = true)] // Удалить точку появления арендованого транспорта (8 лвл)
        public static void CMD_deleteSpawnPoint(Player player, int id)
        {
            try
            {
                RentcarSystem.deleteRentSpawnPoint(player, id);
            }
            catch (Exception e) { Log.Write("EXCEPTION AT deleterentpoint \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }

        [Command("deleterentpoint", Hide = true)] // Удалить точку аренды (8 лвл)
        public static void CMD_deleteRentPoint(Player player, int id)
        {
            try
            {
                RentcarSystem.deleteRentPoint(player, id);
            }
            catch (Exception e) { Log.Write("EXCEPTION AT createrentpoint \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }

        [Command("deletebusiness", Hide = true)] // Удалить бизнес (9 лвл)
        public static void CMD_deleteBiz(Player player, int bizid)
        {
            try
            {
                BusinessManager.deleteBusinessCommand(player, bizid);
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }

        [Command("createsafe", GreedyArg = true, Hide = true)] // Создать сейф для ограблений (8 лвл)
        public static void CMD_createSafe(Player player, int id, float distance, int min, int max, string address)
        {
            try
            {
                SafeMain.CMD_CreateSafe(player, id, distance, min, max, address);
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }

        [Command("removesafe", Hide = true)] // Удалить сейф для ограблений (8 лвл)
        public static void CMD_removeSafe(Player player)
        {
            NAPI.Task.Run(() =>
            {
                try
                {
                    SafeMain.CMD_RemoveSafe(player);
                }
                catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
            });
        }

        //[Command("createstock", Hide = true)] // Создать склад (8 лвл)
        //public static void CMD_createStock(Player player, int frac, int drugs, int mats, int medkits, int money)
        //{
        //    if (!Group.CanUseCmd(player, "createstock")) return;

        //    try
        //    {
        //        //MySQL.Query($"INSERT INTO fractions (id,drugs,mats,medkits,money) VALUES ({frac},{drugs},{mats},{medkits},{money})");

        //        MySqlCommand cmd = new MySqlCommand();
        //        cmd.CommandText = "INSERT INTO `fractions` SET `id`=@id, `drugs`=@drugs, `mats`=@mats, `medkits`=@medkits, `money`=@money";

        //        cmd.Parameters.AddWithValue("@id", frac);
        //        cmd.Parameters.AddWithValue("@drugs", drugs);
        //        cmd.Parameters.AddWithValue("@mats", mats);
        //        cmd.Parameters.AddWithValue("@medkits", medkits);
        //        cmd.Parameters.AddWithValue("@money", money);
        //        MySQL.Query(cmd);
        //    }
        //    catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        //}

        [Command("demorgan", GreedyArg = true, Hide = true)] // Посадить игрока в деморган (2 лвл)
        public static void CMD_sendTargetToDemorgan(Player player, int id, int time, string typeTime, string reason)
        {
            try
            {
                if (time < 0)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Нельзя указывать время меньше 0", 3000);
                    return;
                }

                if (Main.GetPlayerByID(id) == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок с таким ID не найден", 3000);
                    return;
                }
                Admin.sendPlayerToDemorgan(player, Main.GetPlayerByID(id), time, typeTime, reason);
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }

        [Command("loadipl", Hide = true)] // Подгрузить всем игрокам IPL для карты (8 лвл)
        public static void CMD_LoadIPL(Player player, string ipl)
        {
            try
            {
                if (!Group.CanUseCmd(player, "loadipl")) return;
                NAPI.World.RequestIpl(ipl);
                player.SendChatMessage("Вы подгрузили IPL: " + ipl);
            }
            catch
            {
            }
        }

        [Command("unloadipl", Hide = true)] // Выгрузить у всех игроков IPL (8 лвл)
        public static void CMD_UnLoadIPL(Player player, string ipl)
        {
            try
            {
                if (!Group.CanUseCmd(player, "unloadipl")) return;
                NAPI.World.RemoveIpl(ipl);
                player.SendChatMessage("Вы выгрузили IPL: " + ipl);
            }
            catch
            {
            }
        }

        [Command("loadprop", Hide = true)] // Подгрузить себе PROP для карты (8 лвл)
        public static void CMD_LoadProp(Player player, double x, double y, double z, string prop)
        {
            try
            {
                if (!Group.CanUseCmd(player, "loadprop")) return;
                Trigger.ClientEvent(player, "loadProp", x, y, z, prop);
                player.SendChatMessage("Вы подгрузили Interior Prop: " + prop);
            }
            catch
            {
            }
        }

        [Command("unloadprop", Hide = true)]// Подгрузить у себя PROP (8 лвл)
        public static void CMD_UnLoadProp(Player player, double x, double y, double z, string prop)
        {
            try
            {
                if (!Group.CanUseCmd(player, "unloadprop")) return;
                Trigger.ClientEvent(player, "UnloadProp", x, y, z, prop);
                player.SendChatMessage("Вы выгрузили Interior Prop: " + prop);
            }
            catch
            {
            }
        }

        [Command("starteffect", Hide = true)] // Включить эффект ??? (8 лвл)
        public static void CMD_StartEffect(Player player, string effect, int dur = 0, bool loop = false)
        {
            try
            {
                if (!Group.CanUseCmd(player, "starteffect")) return;
                Trigger.ClientEvent(player, "startScreenEffect", effect, dur, loop);
                player.SendChatMessage("Вы включили Effect: " + effect);
            }
            catch
            {
            }
        }

        [Command("stopeffect", Hide = true)] // Выключить эффект ??? (8 лвл)
        public static void CMD_StopEffect(Player player, string effect)
        {
            try
            {
                if (!Group.CanUseCmd(player, "stopeffect")) return;
                Trigger.ClientEvent(player, "stopScreenEffect", effect);
                player.SendChatMessage("Вы выключили Effect: " + effect);
            }
            catch
            {
            }
        }

        [Command("undemorgan", Hide = true)] // Выпустить игрока из деморгана (4 лвл)
        public static void CMD_releaseTargetFromDemorgan(Player player, int id)
        {
            try
            {
                if (Main.GetPlayerByID(id) == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок с таким ID не найден", 3000);
                    return;
                }
                Admin.releasePlayerFromDemorgan(player, Main.GetPlayerByID(id));
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }

        //[Command("offjail", GreedyArg = true, Hide = true)] // Посадить игрока в оффлайне (3 лвл)
        //public static void CMD_offlineJailTarget(Player player, string target, int time, string typeTime, string reason)
        //{
        //    try
        //    {
        //        if (!Group.CanUseCmd(player, "offjail")) return;

        //        if (time < 0)
        //        {
        //            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Нельзя указывать время меньше 0", 3000);
        //            return;
        //        }

        //        if (!Main.PlayerNames.ContainsValue(target))
        //        {
        //            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Игрок не найден", 3000);
        //            return;
        //        }

        //        if (player.Name.Equals(target)) return;
        //        if (NAPI.Player.GetPlayerFromName(target) != null)
        //        {
        //            Admin.sendPlayerToDemorgan(player, NAPI.Player.GetPlayerFromName(target), time, typeTime, reason);
        //            Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, "Игрок был онлайн, поэтому offjail заменён на demorgan", 3000);
        //            return;
        //        }

        //        var firstTime = BanSystem.getMinutes(time, typeTime);
        //        var checkTime = firstTime;

        //        var deTimeMsg = "м";
        //        if (checkTime > 60)
        //        {
        //            deTimeMsg = "ч";
        //            checkTime /= 60;
        //            if (checkTime > 24)
        //            {
        //                deTimeMsg = "д";
        //                checkTime /= 24;
        //            }
        //        }

        //        var split = target.Split('_');
        //        //MySQL.QueryRead($"UPDATE `characters` SET `demorgan`={firstTime},`arrest`=0 WHERE firstname='{split[0]}' AND lastname='{split[1]}'");

        //        MySqlCommand cmd = new MySqlCommand();
        //        cmd.CommandText = "UPDATE `characters` SET `demorgan`=@demorgan, `arrest`=@arrest WHERE `firstname`=@firstname AND `lastname`=@lastname";

        //        cmd.Parameters.AddWithValue("@demorgan", firstTime);
        //        cmd.Parameters.AddWithValue("@arrest", 0);
        //        cmd.Parameters.AddWithValue("@firstname", split[0]);
        //        cmd.Parameters.AddWithValue("@lastname", split[1]);
        //        MySQL.Query(cmd);

        //        NAPI.Chat.SendChatMessageToAll($"~r~{player.Name} посадил игрока {target} в спец. тюрьму на {checkTime}{deTimeMsg} ({reason})");
        //        GameLog.Admin($"{player.Name}", $"demorgan({checkTime}{deTimeMsg},{reason})", $"{target}");
        //    }
        //    catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        //}

        [Command("offdemorgan", GreedyArg = true, Hide = true)] // Посадить игрока в оффлайне (3 лвл)
        public static void CMD_offlineJailTarget(Player player, string target, int time, string typeTime, string reason)
        {
            try
            {
                if (!Group.CanUseCmd(player, "offdemorgan")) return;

                if (time < 0)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Нельзя указывать время меньше 0", 3000);
                    return;
                }

                if (!Main.PlayerNames.ContainsValue(target))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Игрок не найден", 3000);
                    return;
                }

                if (player.Name.Equals(target)) return;
                if (NAPI.Player.GetPlayerFromName(target) != null)
                {
                    Admin.sendPlayerToDemorgan(player, NAPI.Player.GetPlayerFromName(target), time, typeTime, reason);
                    Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, "Игрок был онлайн, поэтому offjail заменён на demorgan", 3000);
                    return;
                }

                var firstTime = BanSystem.getMinutes(time, typeTime);
                var checkTime = firstTime;

                var deTimeMsg = "м";
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

                var split = target.Split('_');
                //MySQL.QueryRead($"UPDATE `characters` SET `demorgan`={firstTime},`arrest`=0 WHERE firstname='{split[0]}' AND lastname='{split[1]}'");

                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = "UPDATE `characters` SET `demorgan`=@demorgan WHERE `firstname`=@firstname AND `lastname`=@lastname";

                cmd.Parameters.AddWithValue("@demorgan", firstTime);
                cmd.Parameters.AddWithValue("@firstname", split[0]);
                cmd.Parameters.AddWithValue("@lastname", split[1]);
                MySQL.Query(cmd);

                NAPI.Chat.SendChatMessageToAll($"~r~{player.Name} посадил игрока {target} в спец. тюрьму на {checkTime}{deTimeMsg} ({reason})");
                GameLog.Admin($"{player.Name}", $"demorgan({checkTime}{deTimeMsg},{reason})", $"{target}");
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }
        [Command("offundemorgan", GreedyArg = true, Hide = true)] // Посадить игрока в оффлайне (3 лвл)
        public static void CMD_offlineUnJailTarget(Player player, string target)
        {
            try
            {
                if (!Group.CanUseCmd(player, "offundemorgan")) return;

                if (!Main.PlayerNames.ContainsValue(target))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Игрок не найден", 3000);
                    return;
                }

                if (player.Name.Equals(target)) return;
                if (NAPI.Player.GetPlayerFromName(target) != null)
                {
                    Admin.releasePlayerFromDemorgan(player, NAPI.Player.GetPlayerFromName(target));
                    Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, "Игрок был онлайн, поэтому offjail заменён на demorgan", 3000);
                    return;
                }


                var split = target.Split('_');
                //MySQL.QueryRead($"UPDATE `characters` SET `demorgan`={firstTime},`arrest`=0 WHERE firstname='{split[0]}' AND lastname='{split[1]}'");

                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = "UPDATE `characters` SET `demorgan`=@demorgan WHERE `firstname`=@firstname AND `lastname`=@lastname";

                cmd.Parameters.AddWithValue("@demorgan", 1);
                cmd.Parameters.AddWithValue("@firstname", split[0]);
                cmd.Parameters.AddWithValue("@lastname", split[1]);
                MySQL.Query(cmd);

                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Успешно");
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }
        [Command("offwarn", GreedyArg = true, Hide = true)] // Выдать игроку варн в оффлайне (3 лвл)
        public static void CMD_offlineWarnTarget(Player player, string target, string reason)
        {
            try
            {
                if (!Group.CanUseCmd(player, "offwarn")) return;
                if (!Main.PlayerNames.ContainsValue(target))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок не найден", 3000);
                    return;
                }
                if (player.Name.Equals(target)) return;
                if (NAPI.Player.GetPlayerFromName(target) != null)
                {
                    Admin.warnPlayer(player, NAPI.Player.GetPlayerFromName(target), reason);
                    Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, "Игрок был онлайн, поэтому offwarn был заменён на warn", 3000);
                    return;
                }
                else
                {
                    string[] split1 = target.Split('_');
                    //DataTable result = MySQL.QueryRead($"SELECT adminlvl FROM characters WHERE firstname='{split1[0]}' AND lastname='{split1[1]}'");

                    MySqlCommand cmd = new MySqlCommand();
                    cmd.CommandText = "SELECT `adminlvl` FROM `characters` WHERE `firstname`=@firstname AND `lastname`=@lastname";

                    cmd.Parameters.AddWithValue("@firstname", split1[0]);
                    cmd.Parameters.AddWithValue("@lastname", split1[1]);
                    DataTable result = MySQL.QueryRead(cmd);

                    DataRow row = result.Rows[0];
                    int targetadminlvl = Convert.ToInt32(row[0]);
                    if (targetadminlvl >= Main.Players[player].AdminLVL)
                    {
                        SendToAdmins(3, $"!{{#d35400}}[OFFWARN-DENIED] {player.Name} ({player.Value}) попытался забанить {target} (offline), который имеет выше уровень администратора.");
                        return;
                    }
                }


                var split = target.Split('_');
                //var data = MySQL.QueryRead($"SELECT warns FROM characters WHERE firstname='{split[0]}' AND lastname='{split[1]}'");

                MySqlCommand cmd2 = new MySqlCommand();
                cmd2.CommandText = "SELECT `warns` FROM `characters` WHERE `firstname`=@firstname AND `lastname`=@lastname";

                cmd2.Parameters.AddWithValue("@firstname", split[0]);
                cmd2.Parameters.AddWithValue("@lastname", split[1]);
                var data = MySQL.QueryRead(cmd2);

                var warns = Convert.ToInt32(data.Rows[0]["warns"]);
                warns++;

                if (warns >= 3)
                {
                    //MySQL.Query($"UPDATE `characters` SET `warns`=0 WHERE firstname='{split[0]}' AND lastname='{split[1]}'");
                    MySqlCommand cmd3 = new MySqlCommand();
                    cmd3.CommandText = "UPDATE `characters` SET `warns`=@warns WHERE `firstname`=@firstname AND `lastname`=@lastname";

                    cmd3.Parameters.AddWithValue("@warns", 0);
                    cmd3.Parameters.AddWithValue("@firstname", split[0]);
                    cmd3.Parameters.AddWithValue("@lastname", split[1]);
                    MySQL.Query(cmd3);

                    Ban.Offline(target, DateTime.Now.AddMinutes(43200), false, "Warns 3/3", "Server_Serverniy");
                }
                else
                {
                    //MySQL.Query($"UPDATE `characters` SET `unwarn`='{MySQL.ConvertTime(DateTime.Now.AddDays(14))}',`warns`={warns},`fraction`=0,`fractionlvl`=0 WHERE firstname='{split[0]}' AND lastname='{split[1]}'");
                    Fraction frac = new Fraction();
                    frac.UnInvite();

                    MySqlCommand cmd4 = new MySqlCommand();
                    cmd4.CommandText = "UPDATE `characters` SET `unwarn`=@unwarn, `warns`=@warns, `fraction`=@fraction, `fractionlvl`=@fractionlvl WHERE `firstname`=@firstname AND `lastname`=@lastname";

                    cmd4.Parameters.AddWithValue("@unwarn", MySQL.ConvertTime(DateTime.Now.AddDays(14)));
                    cmd4.Parameters.AddWithValue("@warns", warns);
                    cmd4.Parameters.AddWithValue("@fraction", JsonConvert.SerializeObject(frac));
                    cmd4.Parameters.AddWithValue("@fractionlvl", 0);
                    cmd4.Parameters.AddWithValue("@firstname", split[0]);
                    cmd4.Parameters.AddWithValue("@lastname", split[1]);
                    MySQL.Query(cmd4);
                }


                NAPI.Chat.SendChatMessageToAll($"~r~{player.Name} выдал предупреждение игроку {target} ({warns}/3 | {reason})");
                GameLog.Admin($"{player.Name}", $"warn({reason})", $"{target}");
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }

        [Command("ban", GreedyArg = true, Hide = true)] // Забанить игрока (3 лвл обычный бан) (8 лвл скрытый бан)
        public static void CMD_banTarget(Player player, string id, int time, string typeTime, string reason)
        {
            try
            {
                if (time < 0)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Нельзя указывать время меньше 0", 3000);
                    return;
                }

                var byID = Convert.ToInt32(id);
                var byLogin = id.ToLower();
                var bySocial = id.ToLower();

                if (Main.GetPlayerByID(byID) != null)
                {
                    Admin.banPlayer(player, Main.GetPlayerByID(byID), time, typeTime, reason, false);
                    return;
                }
                else if (Main.LoggedIn.ContainsKey(byLogin))
                {
                    Player target = Main.LoggedIn[byLogin];
                    Admin.banPlayer(player, target, time, typeTime, reason, false);
                    return;
                }
                else if (Main.Accounts.FirstOrDefault(p => p.Value.SocialClub.ToLower() == bySocial).Key != null) {
                    Player target = Main.Accounts.FirstOrDefault(p => p.Value.SocialClub.ToLower() == bySocial).Key;
                    Admin.banPlayer(player, target, time, typeTime, reason, false);
                    return;
                }
                else
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок с таким ID не найден", 3000);
                    return;
                }
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }

        [Command("stban")] // Забанить игрока (3 лвл обычный бан) (8 лвл скрытый бан)
        public static void CMD_stbanTarget(Player player, int id)
        {
            try
            {
                if (Main.GetPlayerFromPoolByID(id) == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок с таким ID не найден", 3000);
                    return;
                }
                BanSystem.storageBanPlayer(player, Main.GetPlayerFromPoolByID(id));
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }

        [Command("unstban")] // Забанить игрока (3 лвл обычный бан) (8 лвл скрытый бан)
        public static void CMD_unstbanTarget(Player player, int id)
        {
            try
            {
                if (Main.GetPlayerFromPoolByID(id) == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок с таким ID не найден", 3000);
                    return;
                }
                BanSystem.unstorageBanPlayer(player, Main.GetPlayerFromPoolByID(id));
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }

        [Command("hardban", GreedyArg = true, Hide = true)] // Жестко забанить игрока (5 лвл)
        public static void CMD_hardbanTarget(Player player, string id, int time, string typeTime, string reason)
        {
            try
            {
                if (time < 0)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Нельзя указывать время меньше 0", 3000);
                    return;
                }

                var byID = Convert.ToInt32(id);
                var byLogin = id.ToLower();
                var bySocial = id.ToLower();

                if (Main.GetPlayerByID(byID) != null)
                {
                    Admin.hardbanPlayer(player, Main.GetPlayerByID(byID), time, typeTime, reason);
                    return;
                }
                else if (Main.LoggedIn.ContainsKey(byLogin))
                {
                    Player target = Main.LoggedIn[byLogin];
                    Admin.hardbanPlayer(player, target, time, typeTime, reason);
                    return;
                }
                else if (Main.Accounts.FirstOrDefault(p => p.Value.SocialClub.ToLower() == bySocial).Key != null) {
                    Player target = Main.Accounts.FirstOrDefault(p => p.Value.SocialClub.ToLower() == bySocial).Key;
                    Admin.hardbanPlayer(player, target, time, typeTime, reason);
                    return;
                }
                else
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок с таким ID не найден", 3000);
                    return;
                }
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }

        [Command("offban", GreedyArg = true, Hide = true)] // Забанить игрока оффлайн (3 лвл)
        public static void CMD_offlineBanTarget(Player player, string name, int time, string typeTime, string reason)
        {
            try
            {
                if (time < 0)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Нельзя указывать время меньше 0", 3000);
                    return;
                }

                if (!Main.PlayerNames.ContainsValue(name))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрока с таким именем не найдено", 3000);
                    return;
                }
                Admin.offBanPlayer(player, name, time, typeTime, reason);
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }

        [Command("offhardban", GreedyArg = true, Hide = true)] // Жестко забанить игрока оффлайн (5 лвл)
        public static void CMD_offlineHardbanTarget(Player player, string name, int time, string typeTime, string reason)
        {
            try
            {
                if (time < 0)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Нельзя указывать время меньше 0", 3000);
                    return;
                }

                if (!Main.PlayerNames.ContainsValue(name))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрока с таким именем не найдено", 3000);
                    return;
                }
                Admin.offHardBanPlayer(player, name, time, typeTime, reason);
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }

        [Command("unban", GreedyArg = true, Hide = true)] // Разбанить игрока (3 лвл)
        public static void CMD_unbanTarget(Player player, string name)
        {
            if (!Group.CanUseCmd(player, "ban")) return;
            try
            {
                Admin.unbanPlayer(player, name);
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }

        [Command("unhardban", GreedyArg = true, Hide = true)] // Снять хардбан у игрока (5 лвл)
        public static void CMD_unhardbanTarget(Player player, string name)
        {
            if (!Group.CanUseCmd(player, "unhardban")) return;
            try
            {
                Admin.unhardbanPlayer(player, name);
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }

        [Command("offgiveswc", Hide = true)] // Выдать SWCoins в оффлайне (8 лвл)
        public static void CMD_offswc(Player client, string name, long amount)
        {
            if (!Group.CanUseCmd(client, "offgiveswc")) return;
            try
            {
                name = name.ToLower();
                KeyValuePair<Player, nAccount.Account> acc = Main.Accounts.FirstOrDefault(x => x.Value.Login == name);
                if (acc.Value != null)
                {
                    Notify.Send(client, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок онлайн! {acc.Key.Name}:{acc.Key.Value}", 8000);
                    return;
                }

                //MySQL.Query($"update `accounts` set `redbucks`=`redbucks`+{amount} where `login`='{name}'");

                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = "update `accounts` SET `redbucks`=`redbucks`+@swc WHERE `login`=@login";

                cmd.Parameters.AddWithValue("@swc", amount);
                cmd.Parameters.AddWithValue("@login", name);
                MySQL.Query(cmd);

                GameLog.Admin(client.Name, $"offgiveswc({amount})", name);
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }

        [Command("mute", GreedyArg = true, Hide = true)] // Выдать игроку мут (2 лвл)
        public static void CMD_muteTarget(Player player, int id, int time, string typeTime, string reason)
        {
            try
            {
                if (time < 0)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Нельзя указывать время меньше 0", 3000);
                    return;
                }

                if (Main.GetPlayerByID(id) == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок с таким ID не найден", 3000);
                    return;
                }
                Admin.mutePlayer(player, Main.GetPlayerByID(id), time, typeTime, reason);
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }

        [Command("offmute", GreedyArg = true, Hide = true)] // Выдать игроку мут в оффлайне (2 лвл)
        public static void CMD_offlineMuteTarget(Player player, string target, int time, string typeTime, string reason)
        {
            try
            {
                if (time < 0)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Нельзя указывать время меньше 0", 3000);
                    return;
                }

                if (!Main.PlayerNames.ContainsValue(target))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Игрок не найден", 3000);
                    return;
                }
                Admin.OffMutePlayer(player, target, time, typeTime, reason);
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }

        [Command("unmute", Hide = true)] // Снять мут у игрока (2 лвл)
        public static void CMD_muteTarget(Player player, int id)
        {
            try
            {
                if (Main.GetPlayerByID(id) == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок с таким ID не найден", 3000);
                    return;
                }
                Admin.unmutePlayer(player, Main.GetPlayerByID(id));
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }

        [Command("vmute", Hide = true)] // Замутить игрока в войс-чате (2 лвл)
        public static void CMD_voiceMuteTarget(Player player, int id)
        {
            try
            {
                var target = Main.GetPlayerByID(id);
                if (target == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок с таким ID не найден", 3000);
                    return;
                }

                if (!Group.CanUseCmd(player, "vmute")) return;
                target.SetSharedData("voice.muted", true);
                Trigger.ClientEvent(target, "voice.mute");
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }

        [Command("vunmute", Hide = true)] // Снять мут у игрока в войс-чате (2 лвл)
        public static void CMD_voiceUnMuteTarget(Player player, int id)
        {
            try
            {
                if (Main.GetPlayerByID(id) == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок с таким ID не найден", 3000);
                    return;
                }

                if (!Group.CanUseCmd(player, "vunmute")) return;
                player.SetSharedData("voice.muted", false);
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }

        [Command("sban", GreedyArg = true, Hide = true)] // Скрыто забанить игрока (8 лвл)
        public static void CMD_silenceBan(Player player, int id, int time, string typeTime)
        {
            try
            {
                if (time < 0)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Нельзя указывать время меньше 0", 3000);
                    return;
                }

                if (Main.GetPlayerByID(id) == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок с таким ID не найден", 3000);
                    return;
                }
                Admin.banPlayer(player, Main.GetPlayerByID(id), time, typeTime, "", true);
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }

        [Command("kick", GreedyArg = true, Hide = true)] // Кикнуть игрока (2 лвл)
        public static void CMD_kick(Player player, int id, string reason)
        {
            try
            {
                if (Main.GetPlayerByID(id) == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок с таким ID не найден", 3000);
                    return;
                }
                Admin.kickPlayer(player, Main.GetPlayerByID(id), reason, false);
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }

        [Command("skick", Hide = true)] // Скрытно кикнуть игрока (4 лвл)
        public static void CMD_silenceKick(Player player, int id)
        {
            try
            {
                if (Main.GetPlayerFromPoolByID(id) == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок с таким ID не найден", 3000);
                    return;
                }
                Admin.kickPlayer(player, Main.GetPlayerFromPoolByID(id), "Silence kick", true);
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }

        [Command("gm", Hide = true)] // Проверка на ГМ (2 лвл)
        public static void CMD_checkGamemode(Player player, int id)
        {
            try
            {
                if (Main.GetPlayerByID(id) == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок с таким ID не найден", 3000);
                    return;
                }
                Admin.checkGamemode(player, Main.GetPlayerByID(id));
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }

        [Command("agm", Hide = true)] // Админское бессмертие (2 лвл)
        public static void CMD_enableGodmode(Player player)
        {
            try
            {
                if (!Group.CanUseCmd(player, "agm")) return;
                if (!player.HasMyData("AGM") || !player.GetMyData<bool>("AGM"))
                {
                    Trigger.ClientEvent(player, "AGM", true);
                    player.SetMyData("AGM", true);
                }
                else
                {
                    Trigger.ClientEvent(player, "AGM", false);
                    player.SetMyData("AGM", false);
                }
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }

        [Command("warn", GreedyArg = true, Hide = true)] // Выдать варн игроку (3 лвл)
        public static void CMD_warnTarget(Player player, int id, string reason)
        {
            try
            {
                if (Main.GetPlayerByID(id) == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок с таким ID не найден", 3000);
                    return;
                }
                Admin.warnPlayer(player, Main.GetPlayerByID(id), reason);
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }

        [Command("asms", GreedyArg = true, Hide = true)] // Админское сообщение игроку (1 лвл)
        public static void CMD_adminSMS(Player player, int id, string msg)
        {
            try
            {
                if (Main.GetPlayerByID(id) == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок с таким ID не найден", 3000);
                    return;
                }
                Admin.adminSMS(player, Main.GetPlayerByID(id), msg);
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }

        [Command("ans", GreedyArg = true, Hide = true)] // Ответ игроку (1 лвл)
        public static void CMD_answer(Player player, int id, string answer)
        {
            try
            {
                var sender = Main.GetPlayerByID(id);
                if (sender == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок с таким ID не найден", 3000);
                    return;
                }
                Admin.answerReport(player, sender, answer);
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }

        [Command("global", GreedyArg = true, Hide = true)] // Глобальный чат (4 лвл)
        public static void CMD_adminGlobalChat(Player player, string message)
        {
            try
            {
                Admin.adminGlobal(player, message);
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }

        [Command("a", GreedyArg = true, Hide = true)] // Админский чат (1 лвл)
        public static void CMD_adminChat(Player player, string message)
        {
            try
            {
                Admin.adminChat(player, message);
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }

        [Command("setvip", Hide = true)] // Выдать VIP игроку (6 лвл)
        public static void CMD_setVip(Player player, int id, int rank)
        {
            try
            {
                if (Main.GetPlayerByID(id) == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок с таким ID не найден", 3000);
                    return;
                }
                Admin.setPlayerVipLvl(player, Main.GetPlayerByID(id), rank);
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }

        [Command("checkmoney", Hide = true)] // Проверить деньги игрока (3 лвл)
        public static void CMD_checkMoney(Player player, int id)
        {
            try
            {
                if (Main.GetPlayerByID(id) == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок с таким ID не найден", 3000);
                    return;
                }
                Admin.checkMoney(player, Main.GetPlayerByID(id));
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }
        #endregion


        #region VipCommands
        [Command("leave")]
        public static void CMD_leaveFraction(Player player)
        {
            try
            {
                if (Main.Players[player].VipLvl == 0) return;

                Fractions.Manager.UNLoad(player);

                int index = Fractions.Manager.AllMembers.FindIndex(m => m.Name == player.Name);
                if (index > -1) Fractions.Manager.AllMembers.RemoveAt(index);

                Main.Players[player].Fraction.FractionID = 0;
                Main.Players[player].Fraction.FractionRankID = 0;

                Customization.ApplyCharacter(player);
                if (player.HasMyData("HAND_MONEY")) player.SetClothes(5, 45, 0);
                else if (player.HasMyData("HEIST_DRILL")) player.SetClothes(5, 41, 0);

                Main.Players[player].OnDuty = false;
                //player.SetMyData("ON_DUTY", false);
                NAPI.Player.RemoveAllPlayerWeapons(player);

                Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, $"Вы покинули организацию", 3000);
                return;
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }
        #endregion

        [Command("ticket", GreedyArg = true, Hide = true)]
        public static void CMD_govTicket(Player player, int id, int sum, string reason)
        {
            try
            {
                var target = Main.GetPlayerByID(id);
                if (sum < 1) return;
                if (target == null || !Main.Players.ContainsKey(target))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок с таким ID не найден", 3000);
                    return;
                }
                if (target.Position.DistanceTo(player.Position) > 2)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок слишком далеко", 3000);
                    return;
                }
                Fractions.FractionCommands.ticketToTarget(player, target, sum, reason);
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }

        [Command("respawn", Hide = true)]
        public static void CMD_respawnFracCars(Player player)
        {
            try
            {
                Fractions.FractionCommands.respawnFractionCars(player);
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }

        [Command("givemedlic", Hide = true)]
        public static void CMD_givemedlic(Player player, int id)
        {
            try
            {
                var target = Main.GetPlayerByID(id);
                if (target == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок с таким ID не найден", 3000);
                    return;
                }
                if (target.Position.DistanceTo(player.Position) > 2)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок слишком далеко", 3000);
                    return;
                }
                Fractions.FractionCommands.giveMedicalLic(player, target);
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }

        [Command("time")]
        public static void CMD_checkPrisonTime(Player player)
        {
            try
            {
                if (Main.Players[player].ArrestTime != 0)
                    Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вам осталось сидеть {Convert.ToInt32(Main.Players[player].ArrestTime / 60.0)} минут", 3000);
                else if (Main.Players[player].DemorganTime != 0)
                    Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вам осталось сидеть {Convert.ToInt32(Main.Players[player].DemorganTime)} минут", 3000);
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }

        [Command("ptime")]
        public static void CMD_pcheckPrisonTime(Player player, int id)
        {
            try
            {
                if (!Group.CanUseCmd(player, "a")) return;
                Player target = Main.GetPlayerByID(id);
                if (target == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок с таким ID не найден", 3000);
                    return;
                }
                if (Main.Players[target].ArrestTime != 0)
                    Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Игроку {target.Name} осталось сидеть {Convert.ToInt32(Main.Players[target].ArrestTime / 60.0)} минут", 3000);
                else if (Main.Players[target].DemorganTime != 0)
                    Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Игроку {target.Name} осталось сидеть {Convert.ToInt32(Main.Players[target].DemorganTime)} минут", 3000);
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }

        [Command("sellcars", Hide = true)]
        public static void CMD_sellCars(Player player)
        {
            try
            {
                Houses.HouseManager.OpenHouseSaleCarsListMenu(player);
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }

        [Command("dep", GreedyArg = true)]
        public static void CMD_govFracChat(Player player, string msg)
        {
            try
            {
                Fractions.Manager.DepFractionChat(player, msg);
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }

        [Command("gov", GreedyArg = true)]
        public static void CMD_gov(Player player, string msg)
        {
            try
            {
                if (!Fractions.Manager.canUseAccess(player, Fractions.Manager.FractionAccess.gov)) return;
                int frac = Main.Players[player].Fraction.FractionID;
                int lvl = Main.Players[player].Fraction.FractionRankID;
                string[] split = player.Name.Split('_');

                NAPI.Chat.SendChatMessageToAll($"~y~[{Fractions.Manager.GovTags[frac]} | {split[0]} {split[1]}] {msg}");
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }

        [Command("call", GreedyArg = true)]
        public static void CMD_gov(Player player, int number, string msg)
        {
            try
            {
                if (number == 112)
                    client.Fractions.Government.Police.callPolice(player, msg);
                else if (number == 911)
                    client.Fractions.Government.Ems.callEms(player);
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }

        [Command("q", Hide = true)]
        public static void CMD_disconnect(Player player)
        {
            Trigger.ClientEvent(player, "quitcmd");
        }

        //[Command("report", GreedyArg = true, Hide = true)]
        //public static void CMD_report(Player player, string message)
        //{
        //    try
        //    {
        //        if (message.Length > 150)
        //        {
        //            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Слишком длинное сообщение", 3000);
        //            return;
        //        }
        //        if (Main.Players[player].VipLvl == 0 && player.HasMyData("NEXT_REPORT"))
        //        {
        //            DateTime nextReport = player.GetMyData<DateTime>("NEXT_REPORT");
        //            if (DateTime.Now < nextReport)
        //            {
        //                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Попробуйте отправить жалобу через некоторое время", 3000);
        //                return;
        //            }
        //        }
        //        if (player.HasMyData("MUTE_TIMER"))
        //            {
        //            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Нельзя отправлять репорт во время Mute, дождитесь его окончания", 3000);
        //            return;
        //            }
        //        //ReportSys.AddReport(player, message);
        //    }
        //    catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        //}

        [Command("givearmylic", Hide = true)]
        public static void CMD_GiveArmyLicense(Player player, int id)
        {
            try
            {
                var target = Main.GetPlayerByID(id);
                if (target == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок с таким ID не найден", 3000);
                    return;
                }
                if (!Fractions.Manager.canUseAccess(player, Fractions.Manager.FractionAccess.giveadvlic)) return;

                if (player.Position.DistanceTo(target.Position) > 2)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок слишком далеко от Вас", 3000);
                    return;
                }

                if (Main.Players[target].Licenses[10])
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У игрока уже есть военный билет", 3000);
                    return;
                }

                Main.Players[target].Licenses[10] = true;
                Dashboard.sendStats(target);
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы выдали игроку ({target.Value}) военный билет", 3000);
                Notify.Send(target, NotifyType.Success, NotifyPosition.BottomCenter, $"Игрок ({player.Value}) выдал вам военный билет", 3000);
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }

        [Command("takegunlic", Hide = true)]
        public static void CMD_takegunlic(Player player, int id)
        {
            try
            {
                if (Main.GetPlayerByID(id) == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок с таким ID не найден", 3000);
                    return;
                }
                Fractions.FractionCommands.takeGunLic(player, Main.GetPlayerByID(id));
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }

        [Command("givegunlic", Hide = true)]
        public static void CMD_givegunlic(Player player, int id, int price)
        {
            try
            {
                if (Main.GetPlayerByID(id) == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок с таким ID не найден", 3000);
                    return;
                }
                Fractions.FractionCommands.giveGunLic(player, Main.GetPlayerByID(id), price);
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }

        [Command("pd", Hide = true)]
        public static void CMD_policeAccept(Player player, int id)
        {
            try
            {
                if (Main.GetPlayerByID(id) == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок с таким ID не найден", 3000);
                    return;
                }
                client.Fractions.Government.Police.acceptCall(player, Main.GetPlayerByID(id));
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }

        [Command("eject", Hide = true)]
        public static void CMD_ejectTarget(Player player, int id)
        {
            try
            {
                var target = Main.GetPlayerByID(id);
                if (target == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок с таким ID не найден", 3000);
                    return;
                }
                if (!player.IsInVehicle || player.VehicleSeat != 0)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы не находитесь в машине или не на водительском месте", 3000);
                    return;
                }
                if (!target.IsInVehicle || player.Vehicle != target.Vehicle) return;
                Jobs.Taxi.CheckEjectTaxiDriver(player, target);
                VehicleManager.WarpPlayerOutOfVehicle(target);

                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы выкинули игрока ({target.Value}) из машины", 3000);
                Notify.Send(target, NotifyType.Warning, NotifyPosition.BottomCenter, $"Игрок ({player.Value}) выкинул Вас из машины", 3000);
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }

        [Command("ems", Hide = true)]
        public static void CMD_emsAccept(Player player, int id)
        {
            try
            {
                if (Main.GetPlayerByID(id) == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок с таким ID не найден", 3000);
                    return;
                }
                //client.Fractions.Government.Ems.acceptCall(player, Main.GetPlayerByID(id));
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }

        //[Command("pocket", Hide = true)]
        //public static void CMD_pocketTarget(Player player, int id)
        //{
        //    try
        //    {
        //        if (Main.GetPlayerByID(id) == null)
        //        {
        //            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок с таким ID не найден", 3000);
        //            return;
        //        }
        //        if (player.Position.DistanceTo(Main.GetPlayerByID(id).Position) > 2)
        //        {
        //            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок слишком далеко", 3000);
        //            return;
        //        }

        //        Fractions.FractionCommands.playerChangePocket(player, Main.GetPlayerByID(id));
        //    }
        //    catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        //}
        //[Command("buybiz", Hide = true)]
        //public static void CMD_buyBiz(Player player)
        //{
        //    try
        //    {
        //        if (player == null || !Main.Players.ContainsKey(player)) return;

        //        BusinessManager.buyBusinessCommand(player);
        //    }
        //    catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        //}

        //[Command("setrank", Hide = true)]
        //public static void CMD_setRank(Player player, int id, int newrank)
        //{
        //    try
        //    {
        //        if (Main.GetPlayerByID(id) == null)
        //        {
        //            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрока с таким ID не найден", 3000);
        //            return;
        //        }
        //        //Fractions.FractionCommands.SetFracRank(player, Main.GetPlayerByID(id), newrank);
        //    }
        //    catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        //}

        [Command("invite")]
        public static void CMD_inviteFrac(Player player, int id)
        {
            try
            {
                if (Main.GetPlayerByID(id) == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок с таким ID не найден", 3000);
                    return;
                }
                Fractions.FractionCommands.InviteToFraction(player, Main.GetPlayerByID(id));
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }

        [Command("uninvite")]
        public static void CMD_uninviteFrac(Player player, int id)
        {
            try
            {
                if (Main.GetPlayerByID(id) == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок с таким ID не найден", 3000);
                    return;
                }
                Fractions.FractionCommands.UnInviteFromFraction(player, Main.GetPlayerByID(id));
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }

        [Command("f", GreedyArg = true)]
        public static void CMD_fracChat(Player player, string msg)
        {
            try
            {
                Fractions.Manager.fractionChat(player, msg);
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }

        //[Command("arrest", Hide = true)]
        //public static void CMD_arrest(Player player, int id)
        //{
        //    try
        //    {
        //        if (Main.GetPlayerByID(id) == null)
        //        {
        //            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок с таким ID не найден", 3000);
        //            return;
        //        }
        //        Fractions.FractionCommands.arrestPlayer(Main.GetPlayerByID(id));
        //    }
        //    catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        //}

        [Command("rfp")]
        public static void CMD_rfp(Player player, int id)
        {
            try
            {
                if (Main.GetPlayerByID(id) == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок с таким ID не найден", 3000);
                    return;
                }
                Fractions.FractionCommands.releasePlayerFromPrison(player, Main.GetPlayerByID(id));
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }

        //[Command("follow")]
        //public static void CMD_follow(Player player, int id)
        //{
        //    try
        //    {
        //        if (Main.GetPlayerByID(id) == null)
        //        {
        //            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок с таким ID не найден", 3000);
        //            return;
        //        }
        //        Fractions.FractionCommands.targetFollowPlayer(player, Main.GetPlayerByID(id));
        //    }
        //    catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        //}

        //[Command("unfollow")]
        //public static void CMD_unfollow(Player player)
        //{
        //    try
        //    {
        //        Fractions.FractionCommands.targetUnFollowPlayer(player);
        //    }
        //    catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        //}

        [Command("su", GreedyArg = true)]
        public static void CMD_suByPassport(Player player, int pass, int stars, string reason)
        {
            try
            {
                Fractions.FractionCommands.suPlayer(player, pass, stars, reason);
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }

        [Command("c", Hide = true)]
        public static void CMD_getCoords(Player player)
        {
            try
            {
                if (!Group.CanUseCmd(player, "a")) return;
                NAPI.Chat.SendChatMessageToPlayer(player, "Coords", NAPI.Entity.GetEntityPosition(player).ToString());
                NAPI.Chat.SendChatMessageToPlayer(player, "Rot", NAPI.Entity.GetEntityRotation(player).ToString());
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }

        //[Command("incar")]
        //public static void CMD_inCar(Player player, int id)
        //{
        //    try
        //    {
        //        if (Main.GetPlayerByID(id) == null)
        //        {
        //            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок с таким ID не найден", 3000);
        //            return;
        //        }
        //        Fractions.FractionCommands.playerInCar(player, Main.GetPlayerByID(id));
        //    }
        //    catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        //}

        //[Command("pull")]
        //public static void CMD_pullOut(Player player, int id)
        //{
        //    try
        //    {
        //        if (Main.GetPlayerByID(id) == null)
        //        {
        //            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок с таким ID не найден", 3000);
        //            return;
        //        }
        //        Fractions.FractionCommands.playerOutCar(player, Main.GetPlayerByID(id));
        //    }
        //    catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        //}

        [Command("warg", Hide = true)]
        public static void CMD_warg(Player player)
        {
            try
            {
                Fractions.FractionCommands.setWargPoliceMode(player);
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }

        //[Command("medkit", Hide = true)]
        //public static void CMD_medkit(Player player, int id, int price)
        //{
        //    try
        //    {
        //        if (Main.GetPlayerByID(id) == null)
        //        {
        //            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок с таким ID не найден", 3000);
        //            return;
        //        }
        //        Fractions.FractionCommands.sellMedKitToTarget(player, Main.GetPlayerByID(id), price);
        //    }
        //    catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        //}

        [Command("accept", Hide = true)]
        public static void CMD_accept(Player player, int id)
        {
            try
            {
                if (Main.GetPlayerByID(id) == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок с таким ID не найден", 3000);
                    return;
                }
                Fractions.FractionCommands.acceptEMScall(player, Main.GetPlayerByID(id));
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }

        //[Command("heal", Hide = true)]
        //public static void CMD_heal(Player player, int id, int price)
        //{
        //    try
        //    {
        //        if (Main.GetPlayerByID(id) == null)
        //        {
        //            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок с таким ID не найден", 3000);
        //            return;
        //        }
        //        Fractions.FractionCommands.healTarget(player, Main.GetPlayerByID(id), price);
        //    }
        //    catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        //}

        [Command("capture")]
        public static void CMD_capture(Player player)
        {
            try
            {
                client.Fractions.Gangs.GangsCapture.CMD_startCapture(player);
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }

        [Command("buyfuel")]
        public static void CMD_mechanicBuyFuel(Player player, int fuel)
        {
            try
            {
                Jobs.AutoMechanic.buyFuel(player, fuel);
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }

        [Command("me", GreedyArg = true)]
        public static async Task CMD_chatMe(Player player, string msg)
        {
            if (Main.Players[player].Unmute > 0)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы замучены еще на {Main.Players[player].Unmute} минут", 3000);
                return;
            }
            msg = RainbowExploit(player, msg);
            await RPChatAsync("me", player, msg);
        }

        [Command("do", GreedyArg = true)]
        public static async Task CMD_chatDo(Player player, string msg)
        {
            if (Main.Players[player].Unmute > 0)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы замучены еще на {Main.Players[player].Unmute} минут", 3000);
                return;
            }
            msg = RainbowExploit(player, msg);
            await RPChatAsync("do", player, msg);
        }

        [Command("todo", GreedyArg = true)]
        public static async Task CMD_chatToDo(Player player, string msg)
        {
            if (Main.Players[player].Unmute > 0)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы замучены еще на {Main.Players[player].Unmute} минут", 3000);
                return;
            }
            msg = RainbowExploit(player, msg);
            await RPChatAsync("todo", player, msg);
        }

        [Command("s", GreedyArg = true)]
        public static async Task CMD_chatS(Player player, string msg)
        {
            if (Main.Players[player].Unmute > 0)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы замучены еще на {Main.Players[player].Unmute} минут", 3000);
                return;
            }
            msg = RainbowExploit(player, msg);
            await RPChatAsync("s", player, msg);
        }

        [Command("b", GreedyArg = true)]
        public static async Task CMD_chatB(Player player, string msg)
        {
            if (Main.Players[player].Unmute > 0)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы замучены еще на {Main.Players[player].Unmute} минут", 3000);
                return;
            }
            msg = RainbowExploit(player, msg);
            await RPChatAsync("b", player, msg);
        }

        //[Command("vh")]
        //public static async Task CMD_chatVh(Player player, string msg)
        //{
        //    if (Main.Players[player].Unmute > 0)
        //    {
        //        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы замучены еще на {Main.Players[player].Unmute} минут", 3000);
        //        return;
        //    }
        //    await RPChatAsync("vh", player, msg);
        //}

        [Command("m", GreedyArg = true)]
        public static async Task CMD_chatM(Player player, string msg)
        {
            if (Main.Players[player].Unmute > 0)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы замучены еще на {Main.Players[player].Unmute} минут", 3000);
                return;
            }
            msg = RainbowExploit(player, msg);
            await RPChatAsync("m", player, msg);
        }

        [Command("t", GreedyArg = true)]
        public static async Task CMD_chatT(Player player, string msg)
        {
            if (Main.Players[player].Unmute > 0)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы замучены еще на {Main.Players[player].Unmute} минут", 3000);
                return;
            }
            msg = RainbowExploit(player, msg);
            await RPChatAsync("t", player, msg);
        }

        [Command("try", GreedyArg = true)]
        public static void CMD_chatTry(Player player, string msg)
        {
            if (Main.Players[player].Unmute > 0)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы замучены еще на {Main.Players[player].Unmute} минут", 3000);
                return;
            }
            msg = RainbowExploit(player, msg);
            Try(player, msg);
        }

        #region Try command handler
        public static bool Try(Player sender, string message)
        {
            try
            {
                //Random
                int result = rnd.Next(5);
                Log.Debug("Random result: " + result.ToString());
                //
                switch (result)
                {
                    case 0:
                    case 1:
                        foreach (Player player in Main.GetPlayersInRadiusOfPosition(sender.Position, 10f, sender.Dimension))
                            Trigger.ClientEvent(player, "sendRPMessage", "try", "!{#00FF00}{name} " + message + " | !{#00FF00}" + " удачно", new int[] { sender.Value });
                        return true;
                    default:
                        foreach (Player player in Main.GetPlayersInRadiusOfPosition(sender.Position, 10f, sender.Dimension))
                            Trigger.ClientEvent(player, "sendRPMessage", "try", "!{#FF0000}{name} " + message + " | !{#FF0000}" + " неудачно", new int[] { sender.Value });
                        return false;
                }
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); return false; }
        }
        #endregion Try command handler

        #region RP Chat
        public static void RPChat(string cmd, Player sender, string message, Player target = null)
        {
            try
            {
                if (!Main.Players.ContainsKey(sender)) return;
                var names = new int[] { sender.Value };
                if (target != null) names = new int[] { sender.Value, target.Value };
                switch (cmd)
                {
                    case "me":
                        foreach (var player in Main.GetPlayersInRadiusOfPosition(sender.Position, 10f, sender.Dimension))
                            Trigger.ClientEvent(player, "sendRPMessage", "me", "!{#ffe666}{name} " + message, names);
                        return;
                    case "todo":
                        var args = message.Split('*');
                        var msg = args[0];
                        var action = args[1];
                        var genderCh = (Main.Players[sender].Gender) ? "" : "а";
                        foreach (var player in Main.GetPlayersInRadiusOfPosition(sender.Position, 10f, sender.Dimension))
                            Trigger.ClientEvent(player, "sendRPMessage", "todo", msg + "!{#ffe666} - сказал" + genderCh + " {name}, " + action, names);
                        return;
                    case "do":
                        foreach (var player in Main.GetPlayersInRadiusOfPosition(sender.Position, 10f, sender.Dimension))
                            Trigger.ClientEvent(player, "sendRPMessage", "do", "!{#ffe666}" + message + " ({name})", names);
                        return;
                    case "s":
                        foreach (var player in Main.GetPlayersInRadiusOfPosition(sender.Position, 30f, sender.Dimension))
                            Trigger.ClientEvent(player, "sendRPMessage", "s", "{name} кричит: " + message, names);
                        return;
                    case "b":
                        foreach (var player in Main.GetPlayersInRadiusOfPosition(sender.Position, 10f, sender.Dimension))
                            Trigger.ClientEvent(player, "sendRPMessage", "b", "(( {name}: " + message + " ))", names);
                        return;
                    case "m":
                        if ((Main.Players[sender].Fraction.FractionID != 7 && Main.Players[sender].Fraction.FractionID != 9) || !NAPI.Player.IsPlayerInAnyVehicle(sender)) return;
                        var vehicle = sender.Vehicle;
                        if (vehicle.GetData<string>("ACCESS") != "FRACTION") return;
                        if (vehicle.GetData<int>("FRACTION") != 7 && vehicle.GetData<int>("FRACTION") != 9) return;
                        foreach (var player in Main.GetPlayersInRadiusOfPosition(sender.Position, 120f, sender.Dimension))
                            Trigger.ClientEvent(player, "sendRPMessage", "m", "~r~[Мегафон] {name}: " + message, names);
                        return;
                    case "t":
                        if (!Main.Players.ContainsKey(sender) || Main.Players[sender].WorkID != 6) return;
                        foreach (var p in Main.Players.Keys.ToList())
                        {
                            if (p == null || !Main.Players.ContainsKey(p)) continue;

                            if (Main.Players[p].WorkID == 6)
                            {
                                if (p.HasMyData("ON_WORK") && p.GetMyData<bool>("ON_WORK") && p.IsInVehicle)
                                    p.SendChatMessage($"~y~[Рация дальнобойщиков] [{sender.Name}]: {message}");
                            }
                        }
                        return;
                }
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }
        public static Task RPChatAsync(string cmd, Player sender, string message)
        {
            NAPI.Task.Run(() =>
            {
                try
                {
                    if (!Main.Players.ContainsKey(sender)) return;
                    var names = new int[] { sender.Value };
                    switch (cmd)
                    {
                        case "me":
                            foreach (var player in Main.GetPlayersInRadiusOfPosition(sender.Position, 10f, sender.Dimension))
                                Trigger.ClientEvent(player, "sendRPMessage", "me", "!{#E066FF}{name} " + message, names);
                            return;
                        case "todo":
                            var args = message.Split('*');
                            var msg = args[0];
                            var action = args[1];
                            var genderCh = Main.Players[sender].Gender ? "" : "а";
                            foreach (var player in Main.GetPlayersInRadiusOfPosition(sender.Position, 10f, sender.Dimension))
                                Trigger.ClientEvent(player, "sendRPMessage", "todo", msg + "!{#E066FF} - сказал" + genderCh + " {name}, " + action, names);
                            return;
                        case "do":
                            foreach (var player in Main.GetPlayersInRadiusOfPosition(sender.Position, 10f, sender.Dimension))
                                Trigger.ClientEvent(player, "sendRPMessage", "do", "!{#F222DD}" + message + " ({name})", names);
                            return;
                        case "s":
                            foreach (var player in Main.GetPlayersInRadiusOfPosition(sender.Position, 30f, sender.Dimension))
                                Trigger.ClientEvent(player, "sendRPMessage", "s", "{name} кричит: " + message, names);
                            return;
                        case "b":
                            foreach (var player in Main.GetPlayersInRadiusOfPosition(sender.Position, 10f, sender.Dimension))
                                Trigger.ClientEvent(player, "sendRPMessage", "b", "(( {name}: " + message + " ))", names);
                            return;
                        case "m":
                            if (Main.Players[sender].Fraction.FractionID != 7 && Main.Players[sender].Fraction.FractionID != 9 || !NAPI.Player.IsPlayerInAnyVehicle(sender)) return;
                            var vehicle = sender.Vehicle;
                            if (vehicle.GetData<string>("ACCESS") != "FRACTION") return;
                            if (vehicle.GetData<int>("FRACTION") != 7 && vehicle.GetData<int>("FRACTION") != 9) return;
                            foreach (var player in Main.GetPlayersInRadiusOfPosition(sender.Position, 120f, sender.Dimension))
                                Trigger.ClientEvent(player, "sendRPMessage", "m", "!{#FF4D4D}[Мегафон] {name}: " + message, names);
                            return;
                        case "t":
                            if (!Main.Players.ContainsKey(sender) || Main.Players[sender].WorkID != 6) return;
                            foreach (var p in Main.Players.Keys.ToList())
                            {
                                if (p == null || !Main.Players.ContainsKey(p)) continue;

                                if (Main.Players[p].WorkID == 6)
                                {
                                    if (p.HasMyData("ON_WORK") && p.GetMyData<bool>("ON_WORK") && p.IsInVehicle)
                                        p.SendChatMessage($"~y~[Рация дальнобойщиков] [{sender.Name}]: {message}");
                                }
                            }
                            return;
                    }
                }
                catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
            });
            return Task.CompletedTask;
        }
        #endregion RP Chat
        [Command("roll")]
        public static void rollDice(Player player, int id, int money)
        {
            try
            {
                if (Main.GetPlayerByID(id) == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Человек с таким ID не найден", 3000);
                    return;
                }

                if (money <= 0)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Ставка должна быть выше 0", 3000);
                    return;
                }


                // Send request for a game
                Player target = Main.GetPlayerByID(id);
                target.SetMyData("DICE_PLAYER", player);
                target.SetMyData("DICE_VALUE", money);
                Trigger.ClientEvent(target, "popup::open", "DICE", $"Игрок ({player.Value}) хочет сыграть с вами в Кости на {money}$. Вы принимаете?");

                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы предложили ({target.Value}) сыграть в Кости на ${money}$.", 3000);
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }
        }
        #region Roll command handler
        public static int acceptDiceGame(Player playerTwo)
        {
            try
            {
                Player originPlayer = playerTwo.GetMyData<Player>("DICE_PLAYER");
                int money = playerTwo.GetMyData<int>("DICE_VALUE");

                if (money <= 0)
                {
                    Notify.Send(playerTwo, NotifyType.Error, NotifyPosition.BottomCenter, $"Ставка должна быть выше 0", 3000);
                    Notify.Send(originPlayer, NotifyType.Error, NotifyPosition.BottomCenter, $"Ставка должна быть выше 0", 3000);

                    return 0;
                }

                int playerOneResult = new Random().Next(1, 6);
                int playerTwoResult = new Random().Next(1, 6);

                while (playerOneResult == playerTwoResult)
                {
                    Notify.Send(playerTwo, NotifyType.Warning, NotifyPosition.BottomCenter, $"Играем снова, потому что у вас тот же кубик ${playerTwoResult}, что и у противника", 3000);
                    Notify.Send(originPlayer, NotifyType.Warning, NotifyPosition.BottomCenter, $"Играем снова, потому что у вас тот же кубик ${playerTwoResult}, что и у противника", 3000);

                    playerOneResult = new Random().Next(1, 6);
                    playerTwoResult = new Random().Next(1, 6);
                }


                Notify.Send(originPlayer, NotifyType.Info, NotifyPosition.BottomCenter, $"У вас ${playerOneResult}, а у вашего оппонента ${playerTwoResult}", 3000);
                Notify.Send(playerTwo, NotifyType.Info, NotifyPosition.BottomCenter, $"У вас ${playerOneResult}, а у вашего оппонента ${playerTwoResult}", 3000);

                if (playerOneResult > playerTwoResult)
                {
                    Notify.Send(originPlayer, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы выиграли у соперника ${money}$", 3000);
                    MoneySystem.Wallet.Change(originPlayer, money);
                    MoneySystem.Wallet.Change(playerTwo, -money);
                    return 1;
                }
                else
                {
                    Notify.Send(playerTwo, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы выиграли у соперника ${money}$", 3000);
                    MoneySystem.Wallet.Change(originPlayer, -money);
                    MoneySystem.Wallet.Change(playerTwo, money);
                    return 2;
                }
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"CMD\":\n" + e.ToString(), nLog.Type.Error); }

            return 0;
        }

        public static void rejectDiceGame(Player playerTwo)
        {
            Player originPlayer = playerTwo.GetMyData<Player>("DICE_PLAYER");

            Notify.Send(originPlayer, NotifyType.Warning, NotifyPosition.BottomCenter, $"Игрок (${playerTwo.Value}) отклонил предложение", 3000);

            playerTwo.ResetMyData("DICE_PLAYER");
            playerTwo.ResetMyData("DICE_VALUE");
        }
        #endregion Roll command handler

        [Command("ph")]
        public static void CMD_createPhotoCar(Player player, int r, int g, int b, string name)
        {
            try
            {
                if (player == null || !Main.Players.ContainsKey(player)) return;
                if (!Group.CanUseCmd(player, "ph")) return;
                VehicleHash vh = (VehicleHash)NAPI.Util.GetHashKey(name);
                if (vh == 0) throw null;

                Vector3 carPosition = new Vector3(-2159.0112, 2952.4053, 33.11771);
                Vector3 carRotation = new Vector3(0.8921013, 0.14665003, -50.45701);
                Vector3 camPosition = new Vector3(-2158.5408, 2960.5344, 33.867365); // Позиция камеры
                Vector3 camRotation = new Vector3(0, 0, 177.75484); // Rotation камеры
                Vector3 playerPosition = new Vector3(-2158.5408, 2960.5344, 31.867365);

                uint dim = Dimensions.RequestPrivateDimension(player);
                NAPI.Entity.SetEntityDimension(player, dim);
                NAPI.Entity.SetEntityPosition(player, playerPosition);
                Trigger.ClientEvent(player, "freeze", true);
                player.SetMyData("INTERACTIONCHECK", 0);

                if (player.HasMyData("PHOTOCAR") && player.GetMyData<Vehicle>("PHOTOCAR") != null)
                {
                    NAPI.Entity.DeleteEntity(player.GetMyData<Vehicle>("PHOTOCAR"));
                }

                Vehicle veh = NAPI.Vehicle.CreateVehicle(vh, carPosition, carRotation.Z, 0, 0);

                player.SetMyData("PHOTOCAR", veh);
                veh.Dimension = player.Dimension;
                veh.NumberPlate = "ADMIN";
                veh.CustomPrimaryColor = new Color(r, g, b);
                veh.CustomSecondaryColor = new Color(r, g, b);
                veh.SetData("ACCESS", "ADMIN");
                veh.SetData("BY", player.Name);
                VehicleStreaming.SetEngineState(veh, true);
                Log.Debug($"createPhotoCar {name} {r} {g} {b}");

                Trigger.ClientEvent(player, "createPhotoCar", carPosition.X, carPosition.Y, carPosition.Z);
            }
            catch { }
        }

        [Command("eph")]
        public static void CMD_exitCreatePhotoCar(Player player)
        {
            try
            {
                if (player == null || !Main.Players.ContainsKey(player)) return;
                if (!Group.CanUseCmd(player, "eph")) return;

                NAPI.Entity.SetEntityDimension(player, 0);
                Trigger.ClientEvent(player, "freeze", false);
                player.SetMyData("INTERACTIONCHECK", 0);

                if (player.HasMyData("PHOTOCAR") && player.GetMyData<Vehicle>("PHOTOCAR") != null)
                {
                    NAPI.Entity.DeleteEntity(player.GetMyData<Vehicle>("PHOTOCAR"));
                }

                Trigger.ClientEvent(player, "removePhotoCar");

                player.SetMyData<Vehicle>("PHOTOCAR", null);
                Dimensions.DismissPrivateDimension(player);
            }
            catch { }
        }

        [Command("fixinventory", Hide = true)]
        public void CMD_fixInventory(Player player, int id)
        {

            if (player == null || !Main.Players.ContainsKey(player)) return;
            if (!Group.CanUseCmd(player, "fixinventory")) return; //10lvl

            Player target = Main.GetPlayerByID(id);
            int targetUUID = Main.Players[target].UUID;
            bool targetGender = Main.Players[target].Gender;

            // 1 Очистить слоты

            List<bool> tempslots = new List<bool>();
            for (int i = 0; i < 20; i++) tempslots.Add(true);
            nInventory.ItemsSlots[targetUUID] = tempslots;

            for (int i = nInventory.Items[targetUUID].Count - 1; i >= 0; i--)
            {
                if (i >= nInventory.Items[targetUUID].Count) continue;

                // 2 Обнулить состояние слотов characterslotid fastslotid slotid isActive = false (сделать не активным)
                var item = nInventory.Items[targetUUID][i];
                item.IsActive = false;
                item.fast_slot_id = 0;
                item.character_slot_id = 0;
                item.slot_id = 0;

                // 3 CheckAdd передобавить в инвентарь slotid и fillslot
                int slot = nInventory.CheckAdd(nInventory.Items[targetUUID], item, nInventory.ItemsSlots[targetUUID], 5, true);
                item.slot_id = slot;
                //Log.Debug("checkADD: slot: "+slot, nLog.Type.Error);
                nInventory.FillSlot(nInventory.ItemsSlots[targetUUID], item, 5);
            }

            // 4 Обнулить Customization Player
            Customization.ClearClothes(target, targetGender);
            Customization.ClearAccessory(target);

            var custom = Customization.CustomPlayerData[targetUUID];

            custom.Clothes.Top = new ComponentItem(Customization.EmtptySlots[targetGender][11], 0);
            custom.Clothes.Undershit = new ComponentItem(Customization.EmtptySlots[targetGender][8], 0);
            custom.Clothes.Feet = new ComponentItem(Customization.EmtptySlots[targetGender][6], 0);

            custom.Clothes.Leg = new ComponentItem(Customization.EmtptySlots[targetGender][4], 0);
            custom.Clothes.Mask = new ComponentItem(Customization.EmtptySlots[targetGender][1], 0);
            custom.Clothes.Gloves = new ComponentItem(0, 0);
            custom.Clothes.Torso = new ComponentItem(Customization.EmtptySlots[targetGender][3], 0);

            custom.Clothes.Decals = new ComponentItem(Customization.EmtptySlots[targetGender][10], 0);
            custom.Clothes.Accessory = new ComponentItem(Customization.EmtptySlots[targetGender][7], 0); //?
            custom.Accessory = new AccessoryData();
            custom.Clothes.Bag = new ComponentItem(Customization.EmtptySlots[targetGender][5], 0);
            custom.Clothes.Bodyarmor = new ComponentItem(Customization.EmtptySlots[targetGender][9], 0);
            target.SetClothes(9, 0, 0); // BodyArmor

            GUI.Dashboard.PsendItems(target, nInventory.Items[targetUUID], 2);

            Notify.Send(target, NotifyType.Success, NotifyPosition.BottomCenter, $"Ваш инвентарь был обновлен", 3000);
            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Инвентарь игрока успешно обновлен", 3000);
        }

        [Command("leavebrig")]
        public static void leaveBriade(Player player)
        {
            try
            {
                var uuid = Main.Players[player].UUID;
                if (!BuilderManager.Brigades.ContainsKey(Main.Players[player].BrigadeId)) return;
                Brigade brigade = BuilderManager.Brigades[Main.Players[player].BrigadeId];

                var brigPlayer = brigade.Players.Last((t) => t.UUID == uuid);

                if (brigPlayer == null) return;

                if (brigade.Holder == player)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы не можете покинуть собственную бригаду");
                    return;
                }

                brigade.RemovePlayer(player);

                //MySQL.Query($"UPDATE `characters` SET `brigade_id` = -1 WHERE `uuid` = {uuid}");

                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = "UPDATE `characters` SET `brigade_id`=@brigade_id WHERE `uuid`=@uuid";

                cmd.Parameters.AddWithValue("@brigade_id", -1);
                cmd.Parameters.AddWithValue("@uuid", uuid);
                MySQL.Query(cmd);
            }
            catch (Exception ex)
            {
                Console.WriteLine("leavebrig: " + ex.StackTrace);
            }
        }

        [Command("tc")]
        public void CMD_testCutScene(Player player)
        {

            if (player == null || !Main.Players.ContainsKey(player)) return;
            if (!Group.CanUseCmd(player, "tc")) return; //10lvl

            QuestSystem.cutScene_busEnter(player);
        }

        [Command("tcc")]
        public void CMD_testCutSceneCams(Player player, string cutsceneName)
        {

            if (player == null || !Main.Players.ContainsKey(player)) return;
            if (!Group.CanUseCmd(player, "tcc")) return; //10lvl

            Trigger.ClientEvent(player, "CLIENT::cutscene:startFly", cutsceneName);
        }

        [Command("tc2")]
        public void CMD_testCutScene2(Player player)
        {

            if (player == null || !Main.Players.ContainsKey(player)) return;
            if (!Group.CanUseCmd(player, "tc2")) return; //10lvl

            QuestSystem.cutScene_busExit(player);
        }

        [Command("fdcam")]
        public void CMD_forceDestroyCam(Player player, int id)
        {

            if (player == null || !Main.Players.ContainsKey(player)) return;
            if (!Group.CanUseCmd(player, "fdcam")) return; //10lvl

            Player target = Main.GetPlayerByID(id);
            if (target == null)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Игрок с таким ID не найден", 3000);
                return;
            }

            Trigger.ClientEvent(target, "CLIENT::CAM:forceDestroy");
        }
    }
}
