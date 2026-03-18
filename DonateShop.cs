using GTANetworkAPI;
using NeptuneEvo.Core.nAccount;
using NeptuneEvo.MoneySystem;
using NeptuneEvo.Houses;
using NeptuneEvo.SDK;
using NeptuneEvo;
using System;
using System.Collections.Generic;
using System.Text;
using NeptuneEvo.GUI;
using Newtonsoft.Json;
using System.Text.Json.Serialization;
using MySqlConnector;
using NeptuneEvo.Core;
using System.Net.Http.Headers;
using client.GUI;
using client.Systems;
using System.Drawing;
using System.Reflection;
using System.Security;
using ComponentItem = NeptuneEvo.Core.ComponentItem;
using AccessoryData = NeptuneEvo.Core.AccessoryData;
using NeptuneEvo.Fractions;
using NeptuneEvo.Families;

namespace client.Core
{
    class DonateShop : Script
    {
        private static nLog Log = new nLog("DonateShop");

        private const int changeNameCost = 100;
        public const int EditCharacterCost = 750;

        public static DonateShopClass shop = new DonateShopClass(
            new List<Service>()
            {
                new Service("name","changeName","Сменить имя",changeNameCost),
                new Service("name","unwarn","Снять варн",400),
                new Service("name","editCharacter","Сменить внешность",EditCharacterCost),
                new Service("name","changePhone","Сменить номер телефона",5000),
                new Service("name","buyNumberPlate","Купить автомобильный номер",10000),
                //new Service("name","editPromoCode","Изменить личный промокод",1000),
                new Service("name","buyArmyLicense","Купить военный билет",750),

            },
            new List<object>()
            { 
            
            },
            new List<Kit>()
            {
                new Kit("kit1","Стартовый","common",new List<string>()
                {   "licenses",
                    "silver",
                    "cash"
                },new List<Kit.Item>()
                {
                    new Kit.Item("Лицензия","B"),
                    new Kit.Item("VIP-SILVER","10 дней"),
                    new Kit.Item("20000$","")
                },150),
                new Kit("kit2","Стандартный","uncommon",new List<string>()
                {   "licenses",
                    "silver",
                    "cash"
                },new List<Kit.Item>()
                {
                    new Kit.Item("Лицензии","A, B"),
                    new Kit.Item("VIP-SILVER","30 дней"),
                    new Kit.Item("50000$","")
                },450),
                new Kit("kit3","Продвинутый","rare",new List<string>()
                {   "licenses",
                    "gold",
                    "cash"
                },new List<Kit.Item>()
                {
                    new Kit.Item("Лицензии","A, B, C"),
                    new Kit.Item("VIP-GOLD","30 дней"),
                    new Kit.Item("125000$","")
                },900),
                new Kit("kit4","Премиальный","mythical",new List<string>()
                {   "licenses",
                    "platinum",
                    "cash"
                },new List<Kit.Item>()
                {
                    new Kit.Item("Лицензии","A, B, C, рыбалка и оружие"),
                    new Kit.Item("VIP-PLATINUM","30 дней"),
                    new Kit.Item("250000$","")
                },1800),
                new Kit("kit5","Максимальный","epic",new List<string>()
                {   "licenses",
                    "platinum",
                    "cash"
                },new List<Kit.Item>()
                {
                    new Kit.Item("Лицензии","Все"),
                    new Kit.Item("VIP-PLATINUM","90 дней"),
                    new Kit.Item("1000000$","")
                },6000),

            }
        );


        [RemoteEvent("SERVER::DONATE:BUY_SHOP")]
        public static void BuyShop(Player player, string name)
        {
            if (!Main.Accounts.ContainsKey(player)) return;
 
            foreach(var item in shop.Services)
            {
                if(item.Name == name)
                {
                    ServiceBuy(player,item);
                    return;
                }
            }
            foreach (var item in shop.Kits)
            {
                if (item.Name == name)
                {
                    KitBuy(player, item);
                    return;
                }
            }
            //add cases
            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Ошибка! Обратитесь к администрации");
        }

        public static void ServiceBuy(Player player, Service service)
        {
            var before = Main.Accounts[player].RedBucks;
            if (Main.Accounts[player].RedBucks < service.Price)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Недостаточно SWCoin");
                return;
            }
            switch (service.Type)
            {
                case "unwarn":
                    if (UnWarn(player))
                    {
                        Main.Accounts[player].RedBucks -= service.Price;

                        Log.Debug($"[SWC Changes][{player.Name}] [DonateShop] Снятие предупреждения: [{service.Price}] {before} -> {Main.Accounts[player].RedBucks}");
                        GameLog.SWC(Main.Players[player].UUID, "[DonateShop] Снятие предупреждения", Main.Accounts[player].Login, Convert.ToInt32(service.Price), before);
                    }
                    break;
                case "editCharacter":
                    FixInventory(player);
                    player.SetMyData<bool>("INDONATECREATORMENU", true);
                    Customization.SendToCreator(player);
                    NeptuneEvo.SDK.Trigger.ClientEvent(player, "CLIENT::character::create:open");
                    break;
                case "changePhone":
                    player.SetMyData<Service>("SELECTEDSERVICE", service);
                    NeptuneEvo.SDK.Trigger.ClientEvent(player, "popup::openInput", "Изменить номер телефона", "Введите новый номер(7 цифр)", 7, "donat_shop_change_phone");
                    break;
                case "buyNumberPlate":
                    player.SetMyData<Service>("SELECTEDSERVICE", service);
                    NeptuneEvo.SDK.Trigger.ClientEvent(player, "popup::openInput", "Купить автомобильный номер", "Введите новый номер(Пример: AA111AA)", 7, "donat_shop_buy_number_plate");
                    break;
                case "buyArmyLicense":
                    if(BuyArmyLicense(player))
                    {
                        Main.Accounts[player].RedBucks -= service.Price;

                        Log.Debug($"[SWC Changes][{player.Name}] [DonateShop] Покупка армейской лицензии: [{service.Price}] {before} -> {Main.Accounts[player].RedBucks}");
                        GameLog.SWC(Main.Players[player].UUID, "[DonateShop] Покупка армейской лицензии", Main.Accounts[player].Login, Convert.ToInt32(service.Price), before);
                    }
                    break;
                default:
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Ошибка! Обратитесь к администрации", 3000);
                    return;
            }
        }
        public static void FixInventory(Player target)
        {
            if (!Main.Players.ContainsKey(target))
                return;

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

            Dashboard.PsendItems(target, nInventory.Items[targetUUID], 2);
        }
        public static void KitBuy(Player player, Kit kit)
        {
            var before = Main.Accounts[player].RedBucks;
            if (Main.Accounts[player].RedBucks < kit.Price)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Недостаточно SWCoin");
                return;
            }
            switch (kit.Type)
            {
                case "kit1":
                    Main.Accounts[player].RedBucks -= kit.Price;

                    Log.Debug($"[SWC Changes][{player.Name}] [DonateShop] Покупка набора: {kit.Type}: [{kit.Price}] {before} -> {Main.Accounts[player].RedBucks}");
                    GameLog.SWC(Main.Players[player].UUID, $"[DonateShop] Покупка набора: {kit.Type}", Main.Accounts[player].Login, Convert.ToInt32(kit.Price), before);

                    Wallet.Change(player, 20000);

                    Main.Players[player].RewardsData.Add(new Reward($"{kit.Name} набор", "SilverVIP(10 дней)", true, RewardType.SilverVIP, 10));
                    Main.Players[player].Licenses[1] = true;

                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы купили набор '{kit.Name}'", 3000);
                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы можете забрать свой VIP статус в Меню статистики -> Награды", 3000);
                    break;
                case "kit2":
                    Main.Accounts[player].RedBucks -= kit.Price;

                    Log.Debug($"[SWC Changes][{player.Name}] [DonateShop] Покупка набора: {kit.Type}: [{kit.Price}] {before} -> {Main.Accounts[player].RedBucks}");
                    GameLog.SWC(Main.Players[player].UUID, $"[DonateShop] Покупка набора: {kit.Type}", Main.Accounts[player].Login, Convert.ToInt32(kit.Price), before);

                    Wallet.Change(player, 50000);

                    Main.Players[player].RewardsData.Add(new Reward($"{kit.Name} набор", "SilverVIP(30 дней)", true, RewardType.SilverVIP, 30));
                    Main.Players[player].Licenses[0] = true;
                    Main.Players[player].Licenses[1] = true;

                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы купили набор '{kit.Name}'", 3000);
                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы можете забрать свой VIP статус в Меню статистики -> Награды", 3000);
                    break;
                case "kit3":
                    Main.Accounts[player].RedBucks -= kit.Price;

                    Log.Debug($"[SWC Changes][{player.Name}] [DonateShop] Покупка набора: {kit.Type}: [{kit.Price}] {before} -> {Main.Accounts[player].RedBucks}");
                    GameLog.SWC(Main.Players[player].UUID, $"[DonateShop] Покупка набора: {kit.Type}", Main.Accounts[player].Login, Convert.ToInt32(kit.Price), before);

                    Wallet.Change(player, 125000);

                    Main.Players[player].RewardsData.Add(new Reward($"{kit.Name} набор", "GoldVIP(30 дней)", true, RewardType.GoldVIP, 30));

                    Main.Players[player].Licenses[0] = true;
                    Main.Players[player].Licenses[1] = true;
                    Main.Players[player].Licenses[2] = true;

                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы купили набор '{kit.Name}'", 3000);
                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы можете забрать свой VIP статус в Меню статистики -> Награды", 3000);
                    break;
                case "kit4":
                    Main.Accounts[player].RedBucks -= kit.Price;

                    Log.Debug($"[SWC Changes][{player.Name}] [DonateShop] Покупка набора: {kit.Type}: [{kit.Price}] {before} -> {Main.Accounts[player].RedBucks}");
                    GameLog.SWC(Main.Players[player].UUID, $"[DonateShop] Покупка набора: {kit.Type}", Main.Accounts[player].Login, Convert.ToInt32(kit.Price), before);

                    Wallet.Change(player, 250000);

                    Main.Players[player].RewardsData.Add(new Reward($"{kit.Name} набор", "PlatinumVIP(30 дней)", true, RewardType.PlatinumVIP, 30));

                    Main.Players[player].Licenses[0] = true;
                    Main.Players[player].Licenses[1] = true;
                    Main.Players[player].Licenses[2] = true;
                    Main.Players[player].Licenses[6] = true;
                    Main.Players[player].Licenses[9] = true;

                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы купили набор '{kit.Name}'", 3000);
                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы можете забрать свой VIP статус в Меню статистики -> Награды", 3000);
                    break;
                case "kit5":
                    Main.Accounts[player].RedBucks -= kit.Price;

                    Log.Debug($"[SWC Changes][{player.Name}] [DonateShop] Покупка набора: {kit.Type}: [{kit.Price}] {before} -> {Main.Accounts[player].RedBucks}");
                    GameLog.SWC(Main.Players[player].UUID, $"[DonateShop] Покупка набора: {kit.Type}", Main.Accounts[player].Login, Convert.ToInt32(kit.Price), before);

                    Wallet.Change(player, 1000000);

                    Main.Players[player].RewardsData.Add(new Reward($"{kit.Name} набор", "PlatinumVIP(90 дней)", true, RewardType.PlatinumVIP, 90));

                    Main.Players[player].Licenses[0] = true;
                    Main.Players[player].Licenses[1] = true;
                    Main.Players[player].Licenses[2] = true;
                    Main.Players[player].Licenses[3] = true;
                    Main.Players[player].Licenses[4] = true;
                    Main.Players[player].Licenses[6] = true;
                    Main.Players[player].Licenses[9] = true;
                    Main.Players[player].Licenses[10] = true;
                    Main.Players[player].Licenses[11] = true;

                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы купили набор '{kit.Name}'", 3000);
                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы можете забрать свой VIP статус в Меню статистики -> Награды", 3000);
                    break;
                default:
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Ошибка! Обратитесь к администрации", 3000);
                    return;
            }
        }

        public static bool BuyNumberPlate(Player player, string number)
        {
            if (!Main.Players.ContainsKey(player))
                return false;
            string alph = "QWERTYUIOPASDFGHJKLZXCVBNM";
            string dig = "1234567890";
            if (number.Count() != 7)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Неверный формат номера", 3000);
                return false;
            }
            if (alph.Contains(number[0]) && alph.Contains(number[1]) && alph.Contains(number[5]) && alph.Contains(number[6]) && dig.Contains(number[2]) && dig.Contains(number[3]) && dig.Contains(number[4]))
            {

            }
            else 
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Неверный формат номера", 3000);
                return false; 
            }
            if (BuyNumbers.NumberPlates.Contains(number))
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Номер уже занят", 3000);
                return false;
            }
            nItem item = new nItem(ItemType.NumberPlate, data: number);
            var tryAdd = nInventory.TryAdd(player, item);
            if (tryAdd == -1)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Нет места в инвентаре", 3000);
                return false;
            }
            BuyNumbers.AddNumberSQL(number);
            BuyNumbers.NumberPlates.Add(number);
            nInventory.Add(player, item);
            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы купили номер: {number}");
            return true;
        }

        public static bool BuyArmyLicense(Player player)
        {
            if (Main.Players[player].Licenses[10] == true)
            {
                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, "У вас уже есть военный билет", 3000);
                return false;
            }
            Main.Players[player].Licenses[10] = true;
            Dashboard.sendStats(player);
            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, "Вы купили военный билет", 3000);
            return true;
        }

        public static bool UnWarn(Player player)
        {
            if (Main.Players[player].Warns <= 0)
            {
                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, "У вас нету варнов", 3000);
                return false;
            }
            Main.Players[player].Warns--;
            Dashboard.sendStats(player);
            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, "Вы сняли 1 варн", 3000);
            return true;
        }

        public static bool ChangeNumberPhone(Player player, string number)
        {
            int parseNumber = 0;
            if(Int32.TryParse(number,out parseNumber))
            {
                if(parseNumber < 1000000 || parseNumber > 9999999)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Номер неверного формата");
                    return false;
                }
            }
            else
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Номер неверного формата");
                return false;
            }
            if (Main.SimCards.ContainsKey(parseNumber))
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Номер занят");
                return false;
            }

            if (Main.Players[player].Sim == -1)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "У вас нету сим-карты");
                return false;
            }

            Main.SimCards.Remove(Main.Players[player].Sim);

            var uuid = Main.Players[player].UUID;
            Main.SimCards.Add(parseNumber, uuid);

            Main.Players[player].Sim = parseNumber;

            Dashboard.sendStats(player);
            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы изменили номер телефона на {parseNumber}", 3000);
            return true;
        }

        [RemoteEvent("SERVER::stat:changeName")]
        public static void ChangeName(Player player, string newName)
        {
            try
            {
                if (!Main.Players.ContainsKey(player)) return;

                if (Main.Accounts[player].RedBucks < changeNameCost)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Недостаточно SWCoin");
                    return;
                }
                string oldName = Main.Players[player].FirstName + "_" + Main.Players[player].LastName;

                int Uuid = Main.PlayerUUIDs.GetValueOrDefault(oldName);
                if (Uuid <= 0)
                {
                    return;
                }

                string[] split = newName.Split("_");

                string alph = "qwertyuiopasdfghjklzxcvbnm";

                foreach(var item in split[0])
                {
                    if(!alph.Contains(item.ToString().ToLower()))
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Неверный формат", 3000);
                        return;
                    }
                }
                foreach (var item in split[1])
                {
                    if (!alph.Contains(item.ToString().ToLower()))
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Неверный формат", 3000);
                        return;
                    }
                }
                if (split[0].Length < 2 || split[1].Length < 2)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Слишком короткое Имя или Фамилия", 3000);
                    return;
                }
                split = newName.Split("_");
                if (split[0][0].ToString().ToUpper() != split[0][0].ToString() && split[1][0].ToString().ToUpper() != split[1][0].ToString())
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Имя и фамилия должны быть с заглавной буквы.", 3000);
                    return;
                }
                split = newName.Split("_");
                if (Main.PlayerNames.ContainsValue(newName))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Данное имя уже занято", 3000);
                    return;
                }
                split = newName.Split("_");
                if (split[1].Remove(0, 1).ToLower() != split[1].Remove(0, 1) || split[0].Remove(0, 1).ToLower() != split[0].Remove(0, 1))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Только первая буква имени или фамилии может быть заглавной", 3000);
                    return;
                }

                Main.PlayerNames[Uuid] = newName;
                Main.PlayerUUIDs.Remove(oldName);
                Main.PlayerUUIDs.Add(newName, Uuid);

                try
                {
                    if (Main.PlayerBankAccs.ContainsKey(oldName))
                    {
                        int bank = Main.PlayerBankAccs[oldName];
                        Main.PlayerBankAccs.Add(newName, bank);
                        Main.PlayerBankAccs.Remove(oldName);
                    }
                }
                catch (Exception e) { Log.Write(e.StackTrace, nLog.Type.Error); }

                var nameCommand = new MySqlCommand("UPDATE `characters` SET `firstname`=@fn, `lastname`=@ln WHERE `uuid`=@uuid");
                MySqlCommand cmd = nameCommand;

                cmd.Parameters.AddWithValue("@fn", split[0]);
                cmd.Parameters.AddWithValue("@ln", split[1]);
                cmd.Parameters.AddWithValue("@uuid", Uuid);
                MySQL.Query(cmd);

                NAPI.Task.Run(() => {
                    try
                    {
                        VehicleManager.changeOwner(oldName, newName);
                        BusinessManager.changeOwner(oldName, newName);
                        Bank.changeHolder(oldName, newName);
                        HouseManager.ChangeOwner(oldName, newName);
                        Donations.Rename(oldName, newName);

                        var family = NeptuneEvo.Families.Family.GetFamilyToCid(Main.Players[player].FamilyCID);
                        if (family is not null)
                        {
                            var member = family.Players.FirstOrDefault(x => x.Name == oldName);
                            if (member is not null)
                            {
                                member.Name = newName;
                                NeptuneEvo.Families.Family.SaveFamily(family);
                            }
                        }
                        if (NeptuneEvo.Fractions.Manager.isHaveFraction(player))
                        {
                            var fracmember = NeptuneEvo.Fractions.Manager.AllMembers.FirstOrDefault(member => member.Name == oldName);
                            fracmember.Name = newName;
                        }

                    }
                    catch (Exception e) { Console.WriteLine(e.StackTrace); }
                });

                GameLog.Name(Uuid, oldName, newName);

                var before = Main.Accounts[player].RedBucks;
                Main.Accounts[player].RedBucks -= changeNameCost;

                Log.Debug($"[SWC Changes][{player.Name}] [DonateShop] Покупка услуги смены имени: [{changeNameCost}] {before} -> {Main.Accounts[player].RedBucks}");
                GameLog.SWC(Main.Players[player].UUID, $"[DonateShop] Покупка услуги смены имени", Main.Accounts[player].Login, changeNameCost, before);
                
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Вы успешно сменили имя");
                player.Kick();
            }
            catch (Exception e)
            {
                Log.Write("EXCEPTION AT \"CHANGENAME\":\n" + e.ToString(), nLog.Type.Error);
            }
        }
    }

    class DonateShopClass
    {
        [JsonProperty("services")]
        public List<Service> Services { get; set; }
        [JsonProperty("case")]
        public List<object> Cases { get; set; }
        [JsonProperty("sets")]
        public List<Kit> Kits { get; set; }

        public DonateShopClass(List<Service> services,List<object> cases, List<Kit> sets)
        {
            this.Services = services;
            this.Cases = cases;
            this.Kits = sets;
        }
    }
    class Service
    {
        [JsonProperty("img")]
        public string Image { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("price")]
        public int Price { get; set; }

        public Service(string img, string type, string name, int price)
        {
            this.Image = img;
            this.Type = type;
            this.Name = name;
            this.Price = price;
        }
    }
    class Kit
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("rare")]
        public string Rare { get; set; }

        [JsonProperty("icons")]
        public List<string> Icons { get; set; }

        [JsonProperty("items")]
        public List<Item> Items { get; set; }

        [JsonProperty("price")]
        public int Price { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public List<List<int>> ServerItemData { get; set; }

        public Kit(string type, string name, string rare,List<string> icons, List<Item> items, int price)
        {
            this.Type = type;
            this.Name = name;
            this.Rare = rare;
            this.Icons = icons;
            this.Items = items;
            this.Price = price;
        }
        public class Item
        {
            public string key { get; set; }
            public string value { get; set; }

            public Item(string key, string value)
            {
                this.key = key;
                this.value = value;
            }
        }
    }

    enum KitItemType 
    {
        License,
        Cash,
        VIP
    }

    public delegate bool onBuy();
}
