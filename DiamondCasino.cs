using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Threading;
using NeptuneEvo.GUI;
using NeptuneEvo.SDK;
using NeptuneEvo.MoneySystem;
using Newtonsoft.Json;
using client.Core.Casino;
using CasinoRoulette = client.Core.Casino.CasinoRoulette;
using client.Systems.BattlePass;

namespace NeptuneEvo.Core
{
    class DiamondCasino : Script
    {
        private static nLog Log = new nLog("Casino");
        //private static Config config = new Config("Casino");

        #region Массивы

        private static readonly Random random = new Random();
        private static readonly byte[] webnums = new byte[37] { 0, 14, 31, 2, 33, 18, 27, 6, 21, 10, 19, 23, 4, 25, 12, 35, 16, 29, 8, 34, 13, 32, 9, 20, 17, 30, 1, 26, 5, 7, 22, 11, 36, 15, 28, 3, 24 }; // номера ячейки для каждого числа 0, 1, 2 и т.д.

        private static Dictionary<Player, (ushort, ushort, ushort)> PlayersList = new Dictionary<Player, (ushort, ushort, ushort)>();
        //RED. ZERO, BLACK
        //private static long minBalance = config.TryGet<long>("minBalance", 15000);
        //private static float colRadius = config.TryGet<float>("ShapeRadius", 2);
        private static int PseudoNumber = 0;

        public class ObjectPrizes
        {
            [JsonProperty("name")]
            public string name { get; set; }
            [JsonProperty("type")]
            public string type { get; set; }
        };

        private static Vector3 colPosition = new Vector3(927.6365, 44.86219, 80.9);

        private static Vector3 blipPosition = new Vector3(927.6365, 44.86219, 80.9);

        public static Dictionary<int, int> WheelPrizes = new Dictionary<int, int>(){
            { 1, 3 }, //Одежда
            { 2, 6 }, //prize
            { 3, 2 }, //Money
            { 4, 7 }, //Coins
            { 5, 4 }, //donat
            { 6, 6 }, //prize
            { 7, 2 }, //Money
            { 8, 7 }, //Coins
            { 9, 3 }, //Одежда
            { 10, 6 }, //prize
            { 11, 7 }, //Coins
            { 12, 1 }, //Mistery
            { 13, 3 }, //Одежда
            { 14, 6 }, //prize
            { 15, 2 }, //Money
            { 16, 7 }, //Coins
            { 17, 3 }, //Одежда
            { 18, 6 }, //prize
            { 19, 5 }, //car
            { 20, 2 }, //Money
        };

        public static Dictionary<int, List<int>> PrizesNums = new Dictionary<int, List<int>>()
        {
            {1, new List<int>(){ 2, 6, 10, 14, 18} },
            {2, new List<int>(){ 3, 5, 7, 15, 20} },
            {3, new List<int>(){ 1, 9, 13, 17} },
            {4, new List<int>(){ 4, 8, 11, 16} },
            {5, new List<int>(){ 19 } },
            {6, new List<int>(){ 12} }
        };

        public static List<string> PrizesType = new List<string>()
        {
            "-",
            "тайный приз",
            "деньги",
            "одежду",
            "SILVER VIP",
            "машину",
            "SWCoins",
            "фишки",
        };

        public static List<string> PrizesCar = new List<string>()
        {
            "cam8tun",
        };

        private static string PrizeVehicleHash = "cam8tun";

        //public static GTANetworkAPI.Object bWheel;
        public static GTANetworkAPI.Object Wheel;

        public static bool isRoll = false;
        public static Player rollPlayer;

        public static List<Thread> CasinoThreads = new List<Thread>();

        public static int ChipPrice = 100;
        public static int SellChipPrice = 90;

        public static float LastY = 0f;
        //public static int j = 0;

        #endregion

        [ServerEvent(Event.ResourceStart)]
        public static void OnResourceStart()
        {

            //NAPI.Marker.CreateMarker(1, new Vector3(1087.3715, 219.1609, -50.320377) + new Vector3(0.0, 0.0, 0.2), new Vector3(), new Vector3(), 1f, new Color(255, 255, 0));
            //NAPI.TextLabel.CreateTextLabel("Выдача приза", new Vector3(1087.3715, 219.1609, -49.520377), 10f, 1f, 0, new Color(255, 255, 255));

            var spinWheel = NAPI.ColShape.CreateCylinderColShape(new Vector3(1110.2363, 227.8508, -50.755817), 1, 2);
            spinWheel.OnEntityEnterColShape += (ColShape shape, Player Player) =>
            {
                Player.SetMyData("INTERACTIONCHECK", 656);
                Trigger.ClientEvent(Player, "client_press_key_to", "open", JsonConvert.SerializeObject(new List<object>() { "E", "крутить колесо фортуны" }));
            };

            spinWheel.OnEntityExitColShape += (ColShape shape, Player Player) =>
            {
                Player.SetMyData("INTERACTIONCHECK", 0);
                Trigger.ClientEvent(Player, "client_press_key_to", "close");
            };

            Vector3 _wheelPos = new Vector3(1109.76, 227.89, -49.64);
            Vector3 _baseWheelPos = new Vector3(1111.05, 229.81, -50.38);

            uint model = NAPI.Util.GetHashKey("vw_prop_vw_luckywheel_02a");
            uint baseWheelModel = NAPI.Util.GetHashKey("vw_prop_vw_luckywheel_01a");

            // bWheel = NAPI.Object.CreateObject(baseWheelModel, _baseWheelPos, new Vector3());
            Wheel = NAPI.Object.CreateObject(model, _baseWheelPos + new Vector3(0, 0, 1.25), new Vector3());

            CreateCasinoVehicle(PrizeVehicleHash);

            NAPI.Entity.SetEntityRotation(Wheel, new Vector3(0, 0, 0));

            NAPI.Marker.CreateMarker(29, new Vector3(1115.912, 219.99, -49.55512), new Vector3(), new Vector3(), 1, new Color(128, 0, 130, 175));
            var col = NAPI.ColShape.CreateCylinderColShape(new Vector3(1115.912, 219.99, -49.55512), 2f, 2f);
            col.OnEntityEnterColShape += (sahpe, player) =>
            {
                player.SetMyData("INTERACTIONCHECK", 662);
                //Trigger.ClientEvent(player, "client_press_key_to", "open", JsonConvert.SerializeObject(new List<object>() { "E", "купить/продать фишки" }));
            };
            col.OnEntityExitColShape += (sahpe, player) =>
            {
                player.SetMyData("INTERACTIONCHECK", 0);
                //Trigger.ClientEvent(player, "client_press_key_to", "close");
            };
        }

        #region Взаимодействие с фишками (Покупка, продажа итд)

        public static void CreateCasinoVehicle(string hash)
        {
            var veh = NAPI.Vehicle.CreateVehicle(VehicleHash.Zorrusso, new Vector3(1100.062, 219.9683, -48.412453), 310.266f, 150, 154, "PRIZE", 255, true, false, 0);

            veh.SetSharedData("BLOCKED", true);
            veh.SetSharedData("FIXGROUND", true);
            veh.SetSharedData("FIXGROUNDCASINO", true);
        }

        public static void OpenCashier(Player player)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();

            dict.Add("show", true);
            dict.Add("activeTab", "Buy");
            dict.Add("chips", DiamondCasino.GetAllChips(player));
            dict.Add("money", Main.Players[player].Money);
            dict.Add("price", new Dictionary<string, object>()
            {
                {"buy", ChipPrice },
                {"sell", SellChipPrice }
            });

            Trigger.ClientEvent(player, "CLIENT::CASINO:OPEN", JsonConvert.SerializeObject(dict), "CASH");

            Trigger.ClientEvent(player, "playerInteractionCheck", false);

            GUI.KeyLabel.Hide(player);
        }

        public static void UpdateChipsInfo(Player player)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();

            dict.Add("chips", DiamondCasino.GetAllChips(player));
            dict.Add("money", Main.Players[player].Money);

            Trigger.ClientEvent(player, "CLIENT::CASINO:OPEN", JsonConvert.SerializeObject(dict), "CASH");
        }

        public static void OpenBuyChips(Player player)
        {
            player.SetMyData("DIALOG_BUYCHIPS", 2);
            Trigger.ClientEvent(player, "openDialogNpc", "Кассир", $"Сколько вам нужно? Курс {ChipPrice}$ = 1 фишка", new List<string> { "10", "100", "1000", "Назад" });
        }

        public static void OpenSellChips(Player player)
        {
            player.SetMyData("DIALOG_BUYCHIPS", 3);
            Trigger.ClientEvent(player, "openDialogNpc", "Кассир", $"Сколько хотите обменять? Курс 1 фишка = {SellChipPrice}$", new List<string> { "10", "100", "1000", "Назад" });
        }

        [RemoteEvent("SERVER::CASINO:BUY_CHIPS")]
        public static void BuyChips(Player player, int amount)
        {
            Log.Write($"BUYING {amount} CAPS ->>>", nLog.Type.Error);

            if(amount < 1 || amount > 1000000)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Нельзя купить меньше 1 фишки и больше 1000000", 3000);
                return;
            }

            int price = amount * ChipPrice;

            if (!Wallet.Change(player, -price))
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Недостаточно средств", 3000);
                return;
            }

            nInventory.Add(player, new nItem(ItemType.CasinoChips, amount, null));

            #region GBPКвест: 8 Потратить в казино 100.000.000$

            #region BattlePass выполнение квеста
            BattlePass.updateBPGlobalQuestIteration(player, BattlePass.BPGlobalQuestType.CasinoSpendMoney, price);
            #endregion

            #endregion

            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы купили {amount} фишек за {price}$", 3000);

            UpdateChipsInfo(player);

            //OpenBuyChipsMenu(player);
        }

        [RemoteEvent("SERVER::CASINO:SELL_CHIPS")]
        public static void SellChips(Player player, int amount)
        {
            int price = amount * SellChipPrice;

            int emptyIndex = nInventory.FindIndex(Main.Players[player].UUID, ItemType.CasinoChips);

            if (emptyIndex <= 0)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Недостаточно фишек", 3000);
                return;
            }

            if (nInventory.Items[Main.Players[player].UUID][emptyIndex].Count < amount)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Недостаточно фишек", 3000);
                return;
            }

            Wallet.Change(player, price);
            nInventory.Remove(player, ItemType.CasinoChips, amount);

            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы продали {amount} фишек за {price}$", 3000);

            UpdateChipsInfo(player);

            // OpenChangeChipsMenu(player);
        }

        public static int GetAllChips(Player player)
        {
            if (!Main.Players.ContainsKey(player)) return 0;

            var item = nInventory.Find(Main.Players[player].UUID, ItemType.CasinoChips);

            int count = 0;

            if (item != null)
            {
                count = item.Count;
            }

            return count;

        }

        #endregion

        #region Выдача приза

        public static void GivePrize(Player player, int value, int index)
        {
            Trigger.ClientEvent(player, "client_press_key_to", "close");

            switch (value)
            {
                case 1:
                    return;
                case 2:
                    GiveRandomMoney(player, index);
                    break;
                case 3:
                    GiveRandomClothes(player, index);
                    break;
                case 4:
                    GiveRandomDonate(player, index);
                    break;
                case 5:
                    GiveRandomCar(player, index);
                    return;
                case 6:
                    GivePrizeBox(player, index);
                    break;
                case 7:
                    GiveCoins(player, index);
                    break;
            }
        }

        public static void GiveRandomCar(Player player, int index)
        {

            //
            Random rand = new Random();

            int car = rand.Next(0, PrizesCar.Count);

            string carName = PrizesCar[car];

            var house = Houses.HouseManager.GetHouse(player, true);
            if (house == null)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Что бы забрать машину вам нужен дом с гаражем!", 3000);
                return;
            }
            else if (house.GarageID == 0)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "У вашего дома нет гаража!", 3000);
                return;
            }
            else
            {
                var garage = Houses.GarageManager.Garages[house.GarageID];
                // Проверка свободного места в гараже
                if (VehicleManager.getAllPlayerVehicles(player.Name).Count >= NeptuneEvo.Houses.GarageManager.GarageTypes[garage.Type].MaxCars)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Ваши гаражи полны", 3000);
                    return;
                }
                int vNumber = VehicleManager.Create(player.Name, Main.Players[player].UUID, carName, new Color(255, 255, 255), new Color(255, 255, 255), new Color(0, 0, 0));
                if (vNumber != -1)
                {
                    garage.SpawnCar(vNumber);
                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Ваша {carName} доставлена в гараж!", 3000);
                    //Main.Players[player].Prizes = -1;

                    if(Main.Players[player].Prizes.Contains(index)) Main.Players[player].Prizes.Remove(index);
                }
            }
        }

        public static void GiveRandomMoney(Player player, int index)
        {
            Random rand = new Random();

            int money = rand.Next(500, 5001);

            Wallet.Change(player, money);
            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы получили {money}$", 3000);

            if(Main.Players[player].Prizes.Contains(index)) Main.Players[player].Prizes.Remove(index);



            return;
        }

        public static void GiveRandomEat(Player player, int index)
        {
            Random rand = new Random();

            int type = rand.Next(0, 8);

            ItemType item = ItemType.eCola;

            switch (type)
            {
                case 0:
                    item = ItemType.eCola;
                    break;
                case 1:
                    item = ItemType.Burger;
                    break;
                case 2:
                    item = ItemType.HotDog;
                    break;
                case 3:
                    item = ItemType.Sandwich;
                    break;
                case 4:
                    item = ItemType.Pizza;
                    break;
                case 5:
                    item = ItemType.Beer;
                    break;
                case 6:
                    item = ItemType.Sandwich;
                    break;
                case 7:
                    item = ItemType.RusDrink1;
                    break;

            }

            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы получили {nInventory.ItemsNames[(int)item]}", 3000);

            var tryAdd = nInventory.TryAdd(player, new nItem(item, 1));
            if (tryAdd == -1)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Недостаточно места в инвентаре", 3000);
                return;
            }

            nInventory.Add(player, new nItem(item, 1));

            if(Main.Players[player].Prizes.Contains(index)) Main.Players[player].Prizes.Remove(index);

            return;
        }

        //TODO
        public static void GiveCoins(Player player, int index)
        {

            Random rand = new Random();

            int coins = rand.Next(100, 500);

            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы получили {coins} Casino Chips", 3000);



            var tryAdd = nInventory.TryAdd(player, new nItem(ItemType.CasinoChips, coins));
            if (tryAdd == -1)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Недостаточно места в инвентаре", 3000);
                return;
            }

            nInventory.Add(player, new nItem(ItemType.CasinoChips, coins));

            if(Main.Players[player].Prizes.Contains(index)) Main.Players[player].Prizes.Remove(index);

            return;
        }

        public static void GivePrizeBox(Player player, int index)
        {
            Random rand = new Random();

            int type = rand.Next(0, 8);

            ItemType item = ItemType.eCola;

            switch (type)
            {
                case 0:
                    item = ItemType.eCola;
                    break;
                case 1:
                    item = ItemType.Burger;
                    break;
                case 2:
                    item = ItemType.HotDog;
                    break;
                case 3:
                    item = ItemType.Sandwich;
                    break;
                case 4:
                    item = ItemType.Pizza;
                    break;
                case 5:
                    item = ItemType.Beer;
                    break;
                case 6:
                    item = ItemType.Sandwich;
                    break;
                case 7:
                    item = ItemType.RusDrink1;
                    break;

            }

            var tryAdd = nInventory.TryAdd(player, new nItem(item, 1));
            if (tryAdd == -1)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Недостаточно места в инвентаре", 3000);
                return;
            }

            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы получили {nInventory.ItemsNames[(int)item]}", 3000);

            nInventory.Add(player, new nItem(item, 1));

            if(Main.Players[player].Prizes.Contains(index)) Main.Players[player].Prizes.Remove(index);

            return;
        }

        public static void GiveRandomClothes(Player player, int index)
        {
            Random rand = new Random();

            int type = rand.Next(0, 5);


            //TODO CHECK THIS m4ybe
            switch (type)
            {
                case 0:
                    Customization.AddClothes(player, ItemType.Top, 19, 1);
                    break;
                case 1:
                    Customization.AddClothes(player, ItemType.Top, 40, 0);
                    break;
                case 2:
                    Customization.AddClothes(player, ItemType.Top, 75, 0);
                    break;
                case 3:
                    Customization.AddClothes(player, ItemType.Top, 75, 1);
                    break;
                case 4:
                    Customization.AddClothes(player, ItemType.Top, 78, 0);
                    break;

            }

            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы получили {nInventory.ItemsNames[(int)ItemType.Top]}", 3000);

            if(Main.Players[player].Prizes.Contains(index)) Main.Players[player].Prizes.Remove(index);

            return;
        }

        public static void GiveRandomDonate(Player player, int index)
        {
            Random rand = new Random();

            int val = rand.Next(20, 50);


            var before = Main.Accounts[player].RedBucks;
            Main.Accounts[player].RedBucks += val;

            Log.Debug($"[SWC Changes][{player.Name}] [Diamond Casino] Колесо фортуны: [{val}] {before} -> {Main.Accounts[player].RedBucks}");
            GameLog.SWC(Main.Players[player].UUID, "[Diamond Casino] Колесо фортуны", Main.Accounts[player].Login, val, before);

            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы получили {val} SWCoins", 3000);

            Trigger.ClientEvent(player, "redset", Main.Accounts[player].RedBucks);

            if(Main.Players[player].Prizes.Contains(index)) Main.Players[player].Prizes.Remove(index);

            return;
        }

        #endregion

        [RemoteEvent("SERVER::CASINO:EXIT_MENU")]
        public static void ExitMenuCasino(Player player)
        {
            Log.Write($"Exit roullete");
            if (player.HasMyData("BJ_TABLE"))
            {
                BlackJack.Exit(player);
                return;
            }

            if (player.HasMyData("RL_TABLE"))
            {
                CasinoRoulette.ExitRoulette(player);
                return;
            }

            if (player.HasMyData("IT_SEAT"))
            {
                InsideTrack.EixtTable(player);
                return;
            }

            if (player.HasMyData("ON_SLOT"))
            {
                Log.Write($"Exit roullete");
                if (player.HasMyData("SLOT_STARTED"))
                    return;
                Log.Write($"Exit slot");
                CasinoSlots.ExitSlot(player);
                return;
            }
        }

        #region Колесо удачи

        public static void UpdateLuckyWheelTime(Player player)
        {
            if (!Main.Players.ContainsKey(player)) return;

            if (Main.Players[player].LastSpinWheel >= DateTime.Now.AddHours(-24))
            {
                TimeSpan date = DateTime.Now - Main.Players[player].LastSpinWheel;

                int minutes = (60 * 24) - Convert.ToInt32(date.TotalMinutes);
                int hours = Convert.ToInt32(minutes / 60);

                Trigger.ClientEvent(player, "showFortune", hours, Convert.ToInt32(minutes - (hours * 60)));
            }
            else
            {
                Trigger.ClientEvent(player, "showFortune", 0, 0);
            }
        }

        public async static void Roll()
        {

            Log.Debug($"[ROLLWHEEL] [{rollPlayer.Name}] ROLL thread start", nLog.Type.Error);
            int speedIntCnt = 1;

            Random rand = new Random();

            int _priceIndex = 0;

            int percent = rand.Next(1, 1000);

            if (percent == 1)//car
            {
                _priceIndex = 19;
            }
            else if (percent > 1 && percent <= 51)//mistery
            {
                _priceIndex = 12;
            }
            else if (percent > 51 && percent <= 251)//casino chips
            {
                int a = rand.Next(1, 4);
                switch (a)
                {
                    case 1:
                        _priceIndex = 4;
                        break;
                    case 2:
                        _priceIndex = 8;
                        break;
                    case 3:
                        _priceIndex = 11;
                        break;
                    case 4:
                        _priceIndex = 16;
                        break;
                }
            }
            else if (percent > 251 && percent <= 571)//money
            {
                int a = rand.Next(1, 4);
                switch (a)
                {
                    case 1:
                        _priceIndex = 3;
                        break;
                    case 2:
                        _priceIndex = 7;
                        break;
                    case 3:
                        _priceIndex = 15;
                        break;
                    case 4:
                        _priceIndex = 20;
                        break;
                }
            }
            else if (percent > 571 && percent <= 671)//swcoins
            {
                int a = rand.Next(1, 5);
                switch (a)
                {
                    case 1:
                        _priceIndex = 2;
                        break;
                    case 2:
                        _priceIndex = 6;
                        break;
                    case 3:
                        _priceIndex = 10;
                        break;
                    case 4:
                        _priceIndex = 14;
                        break;
                    case 5:
                        _priceIndex = 18;
                        break;
                }
            }
            else if (percent > 671 && percent <= 951)//clothes
            {
                int a = rand.Next(1, 4);
                switch (a)
                {
                    case 1:
                        _priceIndex = 1;
                        break;
                    case 2:
                        _priceIndex = 9;
                        break;
                    case 3:
                        _priceIndex = 13;
                        break;
                    case 4:
                        _priceIndex = 17;
                        break;
                }
            }
            else if (percent > 951 && percent <= 1000)
            {
                _priceIndex = 5;//discount
            }

            if(PseudoNumber != 0)
            {
                _priceIndex = PseudoNumber;
                PseudoNumber = 0;
            }

            int _winAngle = (_priceIndex - 1) * 18;

            float _rollAngle = _winAngle + (360 * 8);
            float _midLength = (_rollAngle / 2);
            float rollspeed = 1;

            while (speedIntCnt > 0)
            {
                var retval = await NAPI.Task.RunReturn(() => NAPI.Entity.GetEntityRotation(Wheel));

                if (_rollAngle > _midLength)
                    speedIntCnt++;
                else
                {


                    speedIntCnt--;
                    if (speedIntCnt < 0)
                        speedIntCnt = 0;
                }


                rollspeed = speedIntCnt / 10f;

                float _y = retval.Y - rollspeed;
                _rollAngle -= rollspeed;

                if (rollspeed == 0)
                {
                    if (_rollAngle < 20 && _rollAngle > 0)
                    {
                        while (_rollAngle > 0)
                        {
                            var pos = await NAPI.Task.RunReturn(() => NAPI.Entity.GetEntityRotation(Wheel));
                            _y = _rollAngle > 1 ? pos.Y - 0.2f : pos.Y - 0.1f;
                            NAPI.Task.Run(() =>
                            {
                                NAPI.Entity.SetEntityRotation(Wheel, new Vector3(0.0, _y, 0.0));
                            });
                            _rollAngle -= _rollAngle > 1 ? 0.2f : 0.1f;
                            Thread.Sleep(15);
                        }
                    }
                    else if (_rollAngle > -20 && _rollAngle < 0)
                    {
                        while (_rollAngle < 0)
                        {
                            var pos = await NAPI.Task.RunReturn(() => NAPI.Entity.GetEntityRotation(Wheel));

                            _y = _rollAngle < -1 ? pos.Y + 0.2f : pos.Y + 0.1f;
                            NAPI.Task.Run(() =>
                            {
                                NAPI.Entity.SetEntityRotation(Wheel, new Vector3(0.0, _y, 0.0));
                            });
                            _rollAngle += _rollAngle < -1 ? 0.2f : 0.1f;
                            Thread.Sleep(15);
                        }
                    }
                }

                NAPI.Task.Run(() =>
                {
                    NAPI.Entity.SetEntityRotation(Wheel, new Vector3(0.0, _y, 0.0));
                });
                Thread.Sleep(15);
            }

            try
            {
                Log.Write($"[ROLLWHEEL] [{rollPlayer.Name}] ROLL try END", nLog.Type.Error);

                isRoll = false;
                EndRoll(_priceIndex);
            }
            catch (Exception e)
            {
                Log.Debug("LuckyWheel: " + e.StackTrace);
            }
        }

        public static void RollWheel(Player player)
        {
            try
            {
                Trigger.ClientEvent(player, "client_press_key_to", "close");
                Log.Debug($"[ROLLWHEEL] [{player.Name}] LastSpinWheel: {Main.Players[player].LastSpinWheel} > {DateTime.Now.AddHours(-24)} -> {Main.Players[player].LastSpinWheel > DateTime.Now.AddHours(-24)}");
                if (Main.Players[player].LastSpinWheel > DateTime.Now.AddHours(-24))
                {
                    TimeSpan dates = DateTime.Now - Main.Players[player].LastSpinWheel;

                    int minutess = (60 * 24) - Convert.ToInt32(dates.TotalMinutes);
                    int hourss = Convert.ToInt32(minutess / 60);

                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Следующий спин возможен через {minutess} минут", 3000);

                    Trigger.ClientEvent(player, "showFortune", hourss, Convert.ToInt32(minutess - (hourss * 60)));

                    return;
                }

                if(isRoll && !Main.Players.ContainsKey(rollPlayer))
                {
                    Log.Debug($"[ROLLWHEEL ERROR RESET] [{player.Name}] isRoll: -> {isRoll} rollPlayer: -> {rollPlayer.Name}");

                    isRoll = false;
                    rollPlayer = null;
                }

                if (isRoll)
                {
                    Log.Debug($"[ROLLWHEEL ERROR] [{player.Name}] isRoll: -> {isRoll} rollPlayer: -> {rollPlayer.Name}");
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Колесо кто то уже крутит", 3000);

                    return;
                }

                isRoll = true;
                rollPlayer = player;

                Log.Debug($"[ROLLWHEEL] [{player.Name}] START ROLLWHEEL");

                NAPI.Task.Run(() => {
                  //Trigger.ClientEvent(player, "luckyWheel", player.Value);
                  
                  startRoll(player);

                  NAPI.Entity.SetEntityRotation(Wheel, new Vector3(0, 0, 0));
                });

            } catch (Exception e) { Log.Write("ROLLWHEEL: " + e.StackTrace, nLog.Type.Error); }
        }

        [RemoteEvent("SERVER::CASINO:startRoll")]
        public static void startRoll(Player player)
        {
            try
            {
                if (!Main.Players.ContainsKey(player))
                {
                    isRoll = false;
                    rollPlayer = null;

                    Log.Write($"[ROLLWHEEL] [{player.Name}] startRoll NOTFOUND PLAYER", nLog.Type.Error);
                    return;
                }

                //NAPI.Entity.SetEntityRotation(Wheel, new Vector3(0, 0, 0));
                //isRoll = true;

                //Main.Players[player].LastSpinWheel = DateTime.Now;

                //rollPlayer = player;

                Log.Debug($"[ROLLWHEEL] [{player.Name}] SERVER::CASINO:startRoll");

                Thread thread1 = new Thread(Roll);
                thread1.Start();
            }
            catch (Exception e)
            {
                Log.Debug("startRoll: "+e.StackTrace, nLog.Type.Error);
            }
        }

        public static void EndRoll(int prize)
        {
            try
            {
                Log.Debug($"[ROLLWHEEL] [{rollPlayer.Name}] EndRoll START");

                if (rollPlayer == null) {
                    Log.Write($"[ROLLWHEEL] [{rollPlayer.Name}] EndRoll NOTFOUND rollPlayer", nLog.Type.Error);
                    return;
                }

                if (!Main.Players.ContainsKey(rollPlayer))
                {
                    rollPlayer = null;

                    Log.Write($"[ROLLWHEEL] [{rollPlayer.Name}] EndRoll NOTFOUND PLAYER", nLog.Type.Error);
                    return;
                }

                //Trigger.ClientEvent(rollPlayer, "delWheelCam");

                //Main.Players[rollPlayer].Prizes = WheelPrizes[prize];
                //Main.Players[rollPlayer].Prizes.Add(WheelPrizes[prize]);

                Random rand = new Random();

                //TODO add prize menu

                switch (PrizesType[WheelPrizes[prize]])
                {
                    case "деньги":
                        Main.Players[rollPlayer].RewardsData.Add(new Reward("Колесо-фортуны", "Деньги от колеса фортуны", true, RewardType.Money, rand.Next(4500, 18000)));
                        break;
                    case "SILVER VIP":
                        Main.Players[rollPlayer].RewardsData.Add(new Reward("Колесо-фортуны", "Донат от колеса фортуны", true, RewardType.SilverVIP, 3));
                        break;
                     case "одежду":
                        int type = rand.Next(0, 5);

                        nItem item = new nItem(ItemType.Top);
                        //TODO CHECK THIS m4ybe
                        switch (type)
                        {
                            case 0:
                                item = new nItem(ItemType.Top, 1, $"50_1_{Main.Players[rollPlayer].Gender}", false);
                                break;
                            case 1:
                                item = new nItem(ItemType.Top, 1, $"50_1_{Main.Players[rollPlayer].Gender}", false);
                                break;
                            case 2:
                                item = new nItem(ItemType.Top, 1, $"111_4_{Main.Players[rollPlayer].Gender}", false);
                                break;
                            case 3:
                                item = new nItem(ItemType.Top, 1, $"279_5_{Main.Players[rollPlayer].Gender}", false);
                                break;
                            case 4:
                                item = new nItem(ItemType.Top, 1, $"305_7_{Main.Players[rollPlayer].Gender}", false);
                                break;

                        }

                        Main.Players[rollPlayer].RewardsData.Add(new Reward("Колесо-фортуны", "Одежда от колеса фортуны", true, RewardType.Clothes, $"{item.Data}"));
                        break;
                    case "машину":
                        int car = rand.Next(0, PrizesCar.Count -1);

                        string carName = PrizesCar[car];
                        Main.Players[rollPlayer].RewardsData.Add(new Reward("Колесо-фортуны", "Машина от колеса фортуны", true, RewardType.Car, $"{carName}"));
                        break;
                    case "SWCoins":  // donate
                        Main.Players[rollPlayer].RewardsData.Add(new Reward("Колесо-фортуны", "Донат от колеса фортуны", true, RewardType.Donate, rand.Next(50, 200)));
                        break;
                    case "фишки":  // ?
                        Main.Players[rollPlayer].RewardsData.Add(new Reward("Колесо-фортуны", "Фишки от колеса фортуны", true, RewardType.CasinoChips, rand.Next(25, 250)));
                        break;
                    case "тайный приз":
                        int percent = rand.Next(1, 100);

                        if (percent == 1)
                        {
                            Main.Players[rollPlayer].RewardsData.Add(MysteryRewards[3]);
                        }
                        else if (percent > 1 && percent <= 8)
                        {
                            Main.Players[rollPlayer].RewardsData.Add(MysteryRewards[2]);
                        }
                        else if (percent > 8 && percent <= 18)
                        {
                            Main.Players[rollPlayer].RewardsData.Add(MysteryRewards[1]);
                        }
                        else if (percent > 18 && percent <= 20)
                        {
                            Main.Players[rollPlayer].RewardsData.Add(MysteryRewards[0]);
                        }
                        else if(percent > 20 && percent <= 30)
                        {
                            Main.Players[rollPlayer].RewardsData.Add(MysteryRewards[7]);
                        }
                        else if (percent > 30 && percent <= 46)
                        {
                            Main.Players[rollPlayer].RewardsData.Add(MysteryRewards[6]);
                        }
                        else if (percent > 46 && percent <= 65)
                        {
                            Main.Players[rollPlayer].RewardsData.Add(MysteryRewards[5]);
                        }
                        else if (percent > 65 && percent <= 85)
                        {
                            Main.Players[rollPlayer].RewardsData.Add(MysteryRewards[4]);
                        }
                        else if (percent > 85 && percent <= 100)
                        {
                            Main.Players[rollPlayer].RewardsData.Add(MysteryRewards[8]);
                        }

                        break;
                }

                Main.Players[rollPlayer].LastSpinWheel = DateTime.Now;

                DiamondCasino.UpdateLuckyWheelTime(rollPlayer);

                #region BPКвест: 21 Прокрутите колесо удачи

                #region BattlePass выполнение квеста
                BattlePass.updateBPQuestIteration(rollPlayer, BattlePass.BPQuestType.CasinoRollLuckyWheel);
                #endregion

                #endregion

                #region SBPКвест: 6 Прокрутите колесо удачи 10 раз.

                #region BattlePass выполнение квеста
                BattlePass.updateBPSuperQuestIteration(rollPlayer, BattlePass.BPSuperQuestType.CasinoRollLuckyWheel);
                #endregion

                #endregion

                Log.Debug($"[ROLLWHEEL] [{rollPlayer.Name}] LuckyWheelPrize: {PrizesType[WheelPrizes[prize]]}");
                Notify.Send(rollPlayer, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы получили награду {PrizesType[WheelPrizes[prize]]}(награда в Меню статистики -> Награды).", 3000);

                //await Commands.CMD_chatMe(rollPlayer, $"выиграл {PrizesType[WheelPrizes[prize]]}");
                Log.Debug($"[ROLLWHEEL] [{rollPlayer.Name}] EndRoll END");
                rollPlayer = null;

            }
            catch (Exception e) { Log.Write("EndRoll: " + e.ToString(), nLog.Type.Error); }
        }

        public static List<Reward> MysteryRewards = new List<Reward>()
        {
            new Reward("Колесо-фортуны", "Машина от колеса фортуны", true, RewardType.Car, $"{"sultanrs"}"),
            new Reward("Колесо-фортуны", "Машина от колеса фортуны", true, RewardType.Car, $"{"primo"}"),
            new Reward("Колесо-фортуны", "Машина от колеса фортуны", true, RewardType.Car, $"{"impaler4"}"),
            new Reward("Колесо-фортуны", "Машина от колеса фортуны", true, RewardType.Car, $"{"apriora"}"),

            new Reward("Колесо-фортуны", "Машина от колеса фортуны", true, RewardType.InventoryItem, (int)ItemType.Pizza),
            new Reward("Колесо-фортуны", "Машина от колеса фортуны", true, RewardType.InventoryItem, (int)ItemType.Nerka),
            new Reward("Колесо-фортуны", "Машина от колеса фортуны", true, RewardType.InventoryItem, (int)ItemType.Servyga),
            new Reward("Колесо-фортуны", "Машина от колеса фортуны", true, RewardType.InventoryItem, (int)ItemType.TrashBag),
            new Reward("Колесо-фортуны", "Машина от колеса фортуны", true, RewardType.InventoryItem, (int)ItemType.Hailiod),
        };

    #endregion

    //[Command("testroll")]
    //public static void CasinoTestRoll(Player player, int number)
    //{
    //  try
    //  {
    //    if (number == 0) return;
    //    Main.Players[player].LastSpinWheel = DateTime.Now.AddHours(-25);
    //    //PseudoNumber = number;
    //    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Следующий спин доступен с номером: {number}", 3000);
    //  }
    //  catch (Exception e)
    //  {
    //    Log.Write("testroll: " + e.StackTrace, nLog.Type.Error);
    //  }
    //}

    [Command("giveroll")]
    public static void CasinoGiveRoll(Player player, int playerId)
    {
      try
      {
        if (player == null || !Main.Players.ContainsKey(player)) return;
        if (!Group.CanUseCmd(player, "giveroll")) return; //10lvl

        Player target = Main.GetPlayerByID(playerId);

        if (target == null || !Main.Players.ContainsKey(target))
        {
          Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок не найден", 3000);
          return;
        }

        Main.Players[target].LastSpinWheel = DateTime.Now.AddHours(-25);
        UpdateLuckyWheelTime(target);

        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Время Колеса удачи успешно сброшено!", 3000);
      }
      catch (Exception e)
      {
        Log.Write("giveroll: " + e.StackTrace, nLog.Type.Error);
      }
    }

    [Command("fixroll")]
        public static void CasinoFixRoll(Player player)
        {
            if (!Group.CanUseCmd(player, "fixroll")) return;

            isRoll = false;
            rollPlayer = null;

            Log.Debug($"[ROLLWHEEL] [{player.Name}] /fixroll");

            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы обнулили крутящего колесо удачи", 3000);
            return;
        }
  }




  class Bet
    {
        public Player Player;
        public int BetAmount = 0;
        public int Spot;

        public Bet(Player player, int money, int spot)
        {
            Player = player;
            BetAmount = money;
            Spot = spot;
        }
    }
}
