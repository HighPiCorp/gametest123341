using GTANetworkAPI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using NeptuneEvo.Core.nAccount;
using NeptuneEvo.Core.Character;
using System.Linq;
using NeptuneEvo.SDK;
using System.Data;
using NeptuneEvo.Houses;
using NeptuneEvo.Families;
using NeptuneEvo.GUI;
using MySqlConnector;
using client.Families;
using client.Systems.BattlePass;

namespace NeptuneEvo.Core
{
    class CarRoom : Script
    {
        private static nLog Log = new nLog("CARROOM");

        public static Vector3 PlayerPos = new Vector3(822.7455, -1384.1637, -51.671775); // Позиция игрока

        public static Dictionary<int, List<ParkPlace>> CarRoomPlaces = new Dictionary<int, List<ParkPlace>>();
        public static int CarRoomPlacesCount = 0;

        [ServerEvent(Event.ResourceStart)]
        public static void ResourceStart()
        {
            try
            {
                DataTable result = MySQL.QueryRead("SELECT * FROM `carroomplaces`");
                if (result == null || result.Rows.Count == 0)
                {
                    Log.Write("DB return null result.", nLog.Type.Warn);
                    return;
                }

                foreach (DataRow Row in result.Rows)
                {
                    int cId = Convert.ToInt32(Row["c_id"]);

                    if (!CarRoomPlaces.ContainsKey(cId))
                        CarRoomPlaces.Add(cId, new List<ParkPlace>());

                    CarRoomPlaces[cId].Add(new ParkPlace(
                        JsonConvert.DeserializeObject<Vector3>(Row["pos"].ToString()),
                        JsonConvert.DeserializeObject<Vector3>(Row["rot"].ToString()))
                    );

                    CarRoomPlacesCount++;
                }

            }
            catch (Exception ex)
            {
                Log.Write(ex.StackTrace);
            }
        }

        public static void EnterCarRoom(Player player)
        {
            if (player == null || !Main.Players.ContainsKey(player)) return;
            if (!player.HasMyData("CARROOMID")) return;

            if (NAPI.Player.IsPlayerInAnyVehicle(player)) return;

            player.ResetMyData("CARROOMTEST");

            uint dim = Dimensions.RequestPrivateDimension(player, true);

            NAPI.Entity.SetEntityDimension(player, dim);

            Main.Players[player].ExteriorPos = player.Position;

            NAPI.Entity.SetEntityPosition(player, new Vector3(822.7455, -1384.1637, -51.671775));
            NAPI.Entity.SetEntityRotation(player, new Vector3(0, 0, -87.0777051));

            //player.PlayAnimation("amb@prop_human_seat_chair_mp@male@generic@base", "base", 39);

            Trigger.ClientEvent(player, "freeze", true);
            player.SetMyData("INTERACTIONCHECK", 0);
            OpenCarromMenu(player, BusinessManager.BizList[player.GetMyData<int>("CARROOMID")].Type);
        }


        [ServerEvent(Event.PlayerExitVehicleAttempt)]
        public void Event_OnPlayerExitVehicle(Player player, Vehicle vehicle)
        {
            try
            {
                if (!player.HasMyData("CARROOMTEST")) return;

                Vehicle veh = player.GetMyData<Vehicle>("CARROOMTEST");

                NAPI.Task.Run(() => {
                    veh.Delete();
                });

                player.ResetMyData("CARROOMTEST");
                player.ResetMyData("TESTDRIVE");
                player.ResetMyData("BEFORE_TEST_HP");

                Trigger.ClientEvent(player, "screenFadeOut", 300);

                NAPI.Task.Run(() => {
                    Trigger.ClientEvent(player, "screenFadeIn", 1000);
                }, 700);

                NAPI.Task.Run(() => {
                    EnterCarRoom(player);
                }, 300);

            }
            catch (Exception e)
            {
                Log.Write("PlayerExitVehicle: " + e.StackTrace, nLog.Type.Error);
            }
        }

        [RemoteEvent("carroomTestDrive")]
        public static void RemoteEvent_carroomTestDrive(Player player, string vName, string color1, string color2)
        {
            try
            {
                if (!player.HasMyData("CARROOMID")) return;

                Trigger.ClientEvent(player, "screenFadeOut", 500);

                player.StopAnimation();

                List<int> primaryColor = JsonConvert.DeserializeObject<List<int>>(color1);
                List<int> secondaryColor = JsonConvert.DeserializeObject<List<int>>(color2);

                var mydim = Dimensions.RequestPrivateDimension(player, true);
                NAPI.Entity.SetEntityDimension(player, mydim);
                VehicleHash vh = (VehicleHash)NAPI.Util.GetHashKey(vName);
                var veh = NAPI.Vehicle.CreateVehicle(vh, new Vector3(-1988.9528, -322.82516, 47.666397), new Vector3(-0.27027863, 0.0050534788, -108.07986).Z, 0, 0);
                NAPI.Vehicle.SetVehicleCustomSecondaryColor(veh, secondaryColor[0], secondaryColor[1], secondaryColor[2]);
                NAPI.Vehicle.SetVehicleCustomPrimaryColor(veh, primaryColor[0], primaryColor[1], primaryColor[2]);
                veh.Dimension = mydim;
                veh.NumberPlate = "TESTDRIVE";
                veh.SetData("BY", player.Name);

                Log.Debug("TESTDRIVE DIMENSION: " + mydim);

                player.SetMyData("CARROOMTEST", veh);
                player.SetMyData("TESTDRIVE", true);
                player.SetMyData("BEFORE_TEST_HP", player.Health);

                NAPI.Task.Run(() => {
                    player.Position = new Vector3(-1988.9528, -322.82516, 47.666397);
                    Trigger.ClientEvent(player, "autoshow::destroyCamera");
                    Trigger.ClientEvent(player, "freeze", false);
                }, 600);

                NAPI.Task.Run(() => {
                    VehicleStreaming.SetEngineState(veh, true);
                    player.SetIntoVehicle(veh, 0);
                    Trigger.ClientEvent(player, "screenFadeIn", 1000);

                    #region BPКвест: 47 Посмотреть в тест-драйве 30 машин.

                    #region BattlePass выполнение квеста
                    BattlePass.updateBPQuestIteration(player, BattlePass.BPQuestType.CheckVehicleInTestDrive);
                    #endregion

                    #endregion
                }, 1600);

            }
            catch (Exception e)
            {
                Log.Write("TestDrive: " + e.StackTrace, nLog.Type.Error);
            }
        }

        #region Menu
        private static Dictionary<string, Color> carColors = new Dictionary<string, Color>
        {
            { "Черный", new Color(0, 0, 0) },
            { "Белый", new Color(225, 225, 225) },
            { "Красный", new Color(230, 0, 0) },
            { "Оранжевый", new Color(255, 115, 0) },
            { "Желтый", new Color(240, 240, 0) },
            { "Зеленый", new Color(0, 230, 0) },
            { "Голубой", new Color(0, 205, 255) },
            { "Синий", new Color(0, 0, 230) },
            { "Фиолетовый", new Color(190, 60, 165) },
            { "Малиновый", new Color(220, 20, 60) },
            { "Серый", new Color(144, 144, 144) },
            { "Мятный", new Color(144, 255, 119) },
        };

        public static void OpenCarromMenu(Player player, int biztype)
        {
            if (player == null || !Main.Players.ContainsKey(player)) return;
            if (!player.HasMyData("CARROOMID")) return;

            var bizid = player.GetMyData<int>("CARROOMID");
            Business biz = BusinessManager.BizList[player.GetMyData<int>("CARROOMID")];

            var moneyType = 1; // $
            if (biz.Type == 24) moneyType = 2; // SWC

            Dictionary<string, object> dict = new Dictionary<string, object>();

            var prices = new List<int>();
            var trunks = new List<int>();
            var trunksWeight = new List<int>();
            var fuels = new List<int>();

            if (biz.Type == 17)
            {
                player.SetSharedData("CARROOM-DONATE", true);
                biztype = 5;
            }
            else if (biz.Type == 19)
            {
                player.SetSharedData("CARROOM-DONATE", false);
                biztype = 6;
            }
            else if (biz.Type == 20)
            {
                player.SetSharedData("CARROOM-DONATE", false);
                biztype = 7;
            }
            else if (biz.Type == 22)
            {
                player.SetSharedData("CARROOM-DONATE", false);
                biztype = 8;
            }
            else if (biz.Type == 24)
            {
                player.SetSharedData("CARROOM-DONATE", true);
                biztype = 9;
            }
            else
            {
                player.SetSharedData("CARROOM-DONATE", false);
                biztype -= 2;
            }

            foreach (var p in biz.Products)
            {
                prices.Add(biztype == 5 || biztype == 8 ? p.Price : biz.GetPriceWithMarkUpInt(p.Price));
                trunksWeight.Add(VehicleManager.VehicleWeight[p.Name.ToLower()]);
                fuels.Add(VehicleManager.VehicleFuel[p.Name.ToLower()]);
            }

            List<object> models = new List<object>();
            foreach (var vehicle in BusinessManager.RealVehicles[biztype])
            {
                Dictionary<string, string> vehicleDict = new Dictionary<string, string>();

                vehicleDict.Add("model", vehicle.Key);
                vehicleDict.Add("name", vehicle.Value);

                models.Add(vehicleDict);
            }

            dict.Add("models", models);
            dict.Add("prices", prices);
            dict.Add("trunksWeight", trunksWeight);
            dict.Add("fueltank", fuels);
            dict.Add("moneyType", moneyType);

            Trigger.ClientEvent(player, "openAuto", JsonConvert.SerializeObject(dict));
        }

        private static int BuyVehicle(Player player, Business biz, int type, string vName, string color1, string color2)
        {
            var prod = biz.Products.FirstOrDefault(p => p.Name == vName);
            int vNumber = -1;

            if (biz.Type != 24) {
                // Check products available
                if (type == 0)
                {
                    if (Main.Players[player].Money < biz.GetPriceWithMarkUpInt(prod.Price))
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Недостаточно средств", 3000);
                        return vNumber;
                    }

                    if (!BusinessManager.takeProd(biz.ID, 1, vName, biz.GetPriceWithMarkUpInt(prod.Price)))
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Транспортного средства больше нет на складе", 3000);
                        return vNumber;
                    }
                }
                else if (type == 1)
                {
                    if (!BusinessManager.takeProd(biz.ID, 1, vName, biz.GetPriceWithMarkUpInt(prod.Price)))
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Транспортного средства больше нет на складе", 3000);
                        return vNumber;
                    }

                    if (MoneySystem.Bank.Count(player) < biz.GetPriceWithMarkUpInt(prod.Price))
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Недостаточно средств", 3000);
                        return vNumber;
                    }
                }
            }
            else
            {
                if (Main.Accounts[player].RedBucks < biz.GetPriceWithMarkUpInt(prod.Price))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Недостаточно SWC", 3000);
                    return vNumber;
                }
            }

            var playerHouse = NeptuneEvo.Houses.HouseManager.GetHouse(player, true);
            if (playerHouse == null || playerHouse.GarageID == 0)
            {
                // Без гаража
                if (VehicleManager.getAllPlayerVehicles(player.Name).Count >= GarageManager.MAX_VEHICLES_WITHOUT_GARAGE)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Чтобы иметь более двух автомобилией, приобретите дом с местом для машины", 3000);
                    VehicleManager.WarpPlayerOutOfVehicle(player);
                    return -1;
                }
            }
            else
            {
                var garage = NeptuneEvo.Houses.GarageManager.Garages[playerHouse.GarageID];
                // С гаражем
                if (VehicleManager.getAllPlayerVehicles(player.Name).Count >= NeptuneEvo.Houses.GarageManager.GarageTypes[garage.Type].MaxCars)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Ваши гаражи полны", 3000);
                    return -1;
                }
            }

            if (biz.Type != 24) {
              if (type == 0) MoneySystem.Wallet.Change(player, -biz.GetPriceWithMarkUpInt(prod.Price));
              if (type == 1) MoneySystem.Bank.Change(Main.Players[player].Bank, -biz.GetPriceWithMarkUpInt(prod.Price), true);
            }
            else
            {
              var before = Main.Accounts[player].RedBucks;
              Log.Debug($"[SWC Changes][{player.Name}] [DonateCarroom] Покупка автомобиля за SWC: [{biz.GetPriceWithMarkUpInt(prod.Price)}] {before} -> {Main.Accounts[player].RedBucks}");
              Main.Accounts[player].RedBucks -= biz.GetPriceWithMarkUpInt(prod.Price);
            }

            GameLog.Money($"player({Main.Players[player].UUID})", $"biz({biz.ID})", biz.GetPriceWithMarkUpInt(prod.Price), $"buyCar({vName}) Markup: {biz.Markup}");
            //}
            //else if (biz.Type == 17)
            //{
            //    Account acc = Main.Accounts[player];

            //    if (acc.RedBucks < prod.Price)
            //    {
            //        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Недостаточно SWCoins!", 3000);
            //        return vNumber;
            //    }
            //    acc.RedBucks -= prod.Price;
            //    GameLog.Money(acc.Login, "server", prod.Price, "donateAutoroom");
            //}

            vNumber = VehicleManager.Create(player.Name, Main.Players[player].UUID, vName, carColors[color1], carColors[color2], new Color(0, 0, 0, 0));

            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы купили {vName} с идентификатором {vNumber} ", 3000);
            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Автомобиль на стоянке автосалона. У вас есть 3 минуты что бы забрать транспорт", 5000);

            client.Core.Achievements.AddAchievementScore(player, client.Core.AchievementID.CarLover, 1);

            if (biz.Type != 24)
            {
                #region GBPКвест: 16 Купить автомобилей на 200.000.000$

            #region BattlePass выполнение квеста
            BattlePass.updateBPGlobalQuestIteration(player, BattlePass.BPGlobalQuestType.BuyCars, biz.GetPriceWithMarkUpInt(prod.Price));
            #endregion

            #endregion
            }

            return vNumber;
        }

        public static void SetCarAtPlace(Vehicle vehicle, int carroom, int place)
        {
            if (!CarRoomPlaces[carroom][place].Employ)
            {
                CarRoomPlaces[carroom][place].Employ = true;
                CarRoomPlaces[carroom][place].FinishTime = DateTime.Now.AddMinutes(3);
                CarRoomPlaces[carroom][place].Vehicle = vehicle;
            }
        }

        public static void RemoveCar(int cid, int place)
        {
            if (CarRoomPlaces[cid][place].Employ)
            {
                CarRoomPlaces[cid][place].FinishTime = DateTime.Now.AddSeconds(30);

                NAPI.Task.Run(() => {
                    CarRoomPlaces[cid][place].Employ = false;
                    CarRoomPlaces[cid][place].FinishTime = DateTime.Now;
                    CarRoomPlaces[cid][place].Vehicle = null;
                }, 10000);
            }
        }

        [RemoteEvent("carroomBuy")]
        public static void RemoteEvent_carroomBuy(Player player, string vName, string color1, string color2, int payType)
        {
            try
            {
                Business biz = BusinessManager.BizList[player.GetMyData<int>("CARROOMID")];

                if (!CarRoomPlaces.ContainsKey(biz.ID))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Нет парковочных мест на стоянке автосалона (Обратитесь к админам)", 3000);
                    return;
                }

                if (CarRoomPlaces[biz.ID].Count((t) => !t.Employ) == 0)
                {
                    Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, "К сожалению все парковочные места сейчас заняты, подождите минутку", 3000);
                    return;
                }

                if (payType == 2)
                {
                    BuyFamilyCar(player, vName, color1, color2);
                    return;
                }

                int place = CarRoomPlaces[biz.ID].FindIndex((t) => !t.Employ);

                var house = Houses.HouseManager.GetHouse(player, true);
                if (house == null || house.GarageID == 0)
                {
                    // Player without garage
                    int vNumber = BuyVehicle(player, biz, payType, vName, color1, color2);
                    if (vNumber != -1)
                    {

                        var veh = VehicleManager.Spawn(vNumber, CarRoomPlaces[biz.ID][place].Position, CarRoomPlaces[biz.ID][place].Rotation.Z, player);
                        veh.SetData("CAR_ROOM", biz.ID);
                        veh.SetData("CAR_PLACE", place);
                        SetCarAtPlace(veh, biz.ID, place);
                        Trigger.ClientEvent(player, "createWorkBlip", CarRoomPlaces[biz.ID][place].Position, true);
                    }
                }
                else
                {
                    var garage = Houses.GarageManager.Garages[house.GarageID];
                    // Проверка свободного места в гараже
                    if (VehicleManager.getAllPlayerVehicles(player.Name).Count >= Houses.GarageManager.GarageTypes[garage.Type].MaxCars)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Ваши гаражи полны", 3000);
                        return;
                    }
                    int vNumber = BuyVehicle(player, biz, payType, vName, color1, color2);
                    if (vNumber != -1)
                    {
                        var veh = garage.SpawnCarAtPosition(player, vNumber, CarRoomPlaces[biz.ID][place].Position, CarRoomPlaces[biz.ID][place].Rotation);
                        veh.SetData("CAR_ROOM", biz.ID);
                        veh.SetData("CAR_PLACE", place);
                        SetCarAtPlace(veh, biz.ID, place);
                        Trigger.ClientEvent(player, "createWorkBlip", CarRoomPlaces[biz.ID][place].Position, true);
                    }
                }


            }
            catch (Exception e) { Log.Write("CarroomBuy: " + e.StackTrace, nLog.Type.Error); }
        }

        [RemoteEvent("carroomCancel")]
        public static void RemoteEvent_carroomCancel(Player player)
        {
            try
            {
                if (!player.HasMyData("CARROOMID")) return;
                player.StopAnimation();
                var enterPoint = BusinessManager.BizList[player.GetMyData<int>("CARROOMID")].EnterPoint;
                if(BusinessManager.BizList[player.GetMyData<int>("CARROOMID")].Type == 22)
                {
                    NAPI.Entity.SetEntityPosition(player, new Vector3(-41.071426, -1676.0267, 28.310987));
                }
                NAPI.Entity.SetEntityPosition(player, new Vector3(enterPoint.X, enterPoint.Y, enterPoint.Z + 1.5));
                Main.Players[player].ExteriorPos = new Vector3();
                Trigger.ClientEvent(player, "freeze", false);
                //player.FreezePosition = false;
                NAPI.Entity.SetEntityDimension(player, 0);
                Dimensions.DismissPrivateDimension(player);
                player.ResetMyData("CARROOMID");

                if (!player.HasMyData("CARROOMTEST")) Trigger.ClientEvent(player, "autoshow::destroyCamera");
            }
            catch (Exception e) { Log.Write("carroomCancel: " + e.StackTrace, nLog.Type.Error); }
        }

        public static void BuyFamilyCar(Player player, string vName, string color1, string color2)
        {
            Business biz = BusinessManager.BizList[player.GetMyData<int>("CARROOMID")];

            Main.Players[player].ExteriorPos = new Vector3();

            Family family = Family.GetFamilyToCid(player);
            if (family == null)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы не состоите в семье", 3000);
                return;
            }

            if (!Manager.isHaveAccess(player, Manager.FamilyAccess.BuyVeh))
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы не можете покупать авто для семейного гаража", 3000);
                return;
            }

            var house = Families.FamilyHouseManager.GetHouse(player);
            if (house == null)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У вас нет дома", 3000);
                return;
            }

            if (VehicleManager.getAllPlayerVehicles(family.FamilyCID).Count >= FamilyHouseGarage.GarageTypes[house.Type].MaxCars)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Ваши гаражи полны", 3000);
                return;
            }

            var prod = biz.Products.FirstOrDefault(p => p.Name == vName);

            if (biz.Type != 24) {
                if (Main.Players[player].Money < biz.GetPriceWithMarkUpInt(prod.Price))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Не хватает денег", 3000);
                    return;
                }

                if (!BusinessManager.takeProd(biz.ID, 1, vName, biz.GetPriceWithMarkUpInt(prod.Price)))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Этого автомобиля нет на складе", 3000);
                    return;
                }

                MoneySystem.Wallet.Change(player, -biz.GetPriceWithMarkUpInt(prod.Price));

                #region GBPКвест: 16 Купить автомобилей на 200.000.000$

                #region BattlePass выполнение квеста
                BattlePass.updateBPGlobalQuestIteration(player, BattlePass.BPGlobalQuestType.BuyCars, biz.GetPriceWithMarkUpInt(prod.Price));
                #endregion

                #endregion
            }
            else
            {
                if (Main.Accounts[player].RedBucks < biz.GetPriceWithMarkUpInt(prod.Price))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Недостаточно SWC", 3000);
                    return;
                }

                Main.Accounts[player].RedBucks -= biz.GetPriceWithMarkUpInt(prod.Price);
            }

            GameLog.Money($"player({Main.Players[player].UUID})", $"biz({biz.ID})", biz.GetPriceWithMarkUpInt(prod.Price), $"buyCar({vName})");

            var vNumber = VehicleManager.Create(family.FamilyCID, Main.Players[player].UUID, vName, carColors[color1], carColors[color2], new Color(0, 0, 0));

            int place = CarRoomPlaces[biz.ID].FindIndex((t) => !t.Employ);

            Trigger.ClientEvent(player, "createWorkBlip", CarRoomPlaces[biz.ID][place].Position, true);

            NAPI.Task.Run(() =>
            {
                try
                {
                    var vData = VehicleManager.Vehicles[vNumber];
                    VehicleManager.Vehicles[vNumber].FamilyInGarage = false;
                    var veh = NAPI.Vehicle.CreateVehicle(NAPI.Util.GetHashKey(vData.Model), CarRoomPlaces[biz.ID][place].Position, CarRoomPlaces[biz.ID][place].Rotation.Z, 0, 0);
                    veh.Dimension = 0;
                    veh.NumberPlate = "";
                    NAPI.Entity.SetEntityPosition(veh, CarRoomPlaces[biz.ID][place].Position);
                    NAPI.Entity.SetEntityDimension(veh, 0);
                    VehicleStreaming.SetEngineState(veh, false);
                    VehicleStreaming.SetLockStatus(veh, true);
                    veh.SetSharedData("PETROL", vData.Fuel);
                    veh.SetData("ACCESS", "FAMILY");
                    veh.SetData("FAMILY", family.FamilyCID);
                    veh.SetData("OWNER", player);
                    veh.SetData("CAR_ROOM", biz.ID);
                    veh.SetData("CAR_PLACE", place);
                    veh.SetData("ID", vData.ID);
                    veh.SetData("ITEMS", vData.Items);
                    veh.SetData("SLOTS", vData.Slots);
                    SetCarAtPlace(veh, biz.ID, place);

                    vData.Vehicle = veh;
                    VehicleManager.Save(vNumber);
                    VehicleManager.ApplyCustomization(veh);

                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы успешно купили машину!", 3000);
                }
                catch (Exception e){ Log.Write(e.StackTrace, nLog.Type.Error); }
            }, 1000);

        }

        [Command("createpp", Hide = true)]
        public static void CreateCarRoomPark(Player player, int bizId)
        {
            if (!Group.CanUseCmd(player, "createpp")) return;

            if (!BusinessManager.BizList.ContainsKey(bizId)) return;

            if (!player.IsInVehicle)
            {
                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, "Вы должны быть в авто", 3000);
                return;
            }

            Vector3 pos = player.Vehicle.Position;
            Vector3 rot = player.Vehicle.Rotation;

            CarRoomPlacesCount++;

            //DataTable data = MySQL.QueryRead($"INSERT INTO `carroomplaces` (`c_id`,`pos`,`rot`) VALUES ({bizId}, '{JsonConvert.SerializeObject(pos)}', '{JsonConvert.SerializeObject(rot)}'); SELECT LAST_INSERT_ID();");
            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = "INSERT INTO `carroomplaces` SET `c_id`=@c_id, `pos`=@pos, `rot`=@rot; SELECT LAST_INSERT_ID();";

            cmd.Parameters.AddWithValue("@c_id", bizId);
            cmd.Parameters.AddWithValue("@pos", JsonConvert.SerializeObject(pos));
            cmd.Parameters.AddWithValue("@rot", JsonConvert.SerializeObject(rot));
            DataTable data = MySQL.QueryRead(cmd);

            if (!CarRoomPlaces.ContainsKey(bizId)) CarRoomPlaces.Add(bizId, new List<ParkPlace>());

            CarRoomPlaces[bizId].Add(new ParkPlace(pos, rot));

            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, "Точка добавлена", 3000);
        }

        #endregion
    }
    class ParkPlace
    {
        public bool Employ { get; set; }
        public DateTime FinishTime { get; set; }

        public Vehicle Vehicle { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }

        private static nLog Log = new nLog("CARROOM ParkPlace");

        public ParkPlace(Vector3 pos, Vector3 rot)
        {
            Employ = false;
            Position = pos;
            Rotation = rot;
        }

        public void RemoveCarToParkfine()
        {
            if (Employ)
            {

                NAPI.Task.Run(() => {
                    int num = this.Vehicle.GetData<int>("ID");

                    if (this.Vehicle != null && this.Vehicle.Exists)
                    {
                        this.Vehicle.Delete();
                    }

                    Employ = false;
                    FinishTime = DateTime.Now;
                    Vehicle = null;

                    if (VehicleManager.Vehicles.ContainsKey(num))
                    {
                        string holder = VehicleManager.Vehicles[num].Holder;

                        //VehicleManager.Vehicles[num].ParkFine = true;

                        var player = NAPI.Player.GetPlayerFromName(holder);

                        if (player != null)
                        {
                            Trigger.ClientEvent(player, "deleteWorkBlip");
                            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, "Ваш транспорт отправлен в гараж/парковку", 3000);


                            var house = HouseManager.GetHouse(player, true);

                            if (house == null) return;
                            if (house.GarageID == -1) return;

                            if (GarageManager.Garages[house.GarageID].vehiclesOut.ContainsKey(num))
                                GarageManager.Garages[house.GarageID].vehiclesOut.Remove(num);

                            GarageManager.Garages[house.GarageID].SpawnCar(num);
                        } else
                        {
                            var familys = Manager.Families.Where((t) => t.FamilyCID == holder).ToList();
                            if (familys.Count != 0)
                            {
                                var p = Main.GetPlayerByUUID(VehicleManager.Vehicles[num].OwnerID);
                                if (p != null && Main.Players.ContainsKey(p))
                                {
                                    Trigger.ClientEvent(p, "deleteWorkBlip");
                                    Notify.Send(p, NotifyType.Info, NotifyPosition.BottomCenter, "Ваш транспорт отправлен в гараж особняка", 3000);
                                }
                                VehicleManager.Vehicles[num].FamilyInGarage = true;
                            }
                        }

                    }
                });

            }
        }

        public static void ReleaseParkPlace(Vehicle vehicle)
        {
            NAPI.Task.Run(() => {
                string num = vehicle.NumberPlate;

                if (vehicle.HasData("CAR_ROOM"))
                {
                    int bId = vehicle.GetData<int>("CAR_ROOM");
                    int place = vehicle.GetData<int>("CAR_PLACE");

                    int id = vehicle.GetData<int>("ID");

                    CarRoom.RemoveCar(bId, place);

                    string holder = VehicleManager.Vehicles[id].Holder;

                    var player = NAPI.Player.GetPlayerFromName(holder);

                    if (player != null)
                    {
                        Trigger.ClientEvent(player, "deleteWorkBlip");
                    }

                    vehicle.ResetData("CAR_ROOM");
                    vehicle.ResetData("CAR_PLACE");
                }
            });
        }

    }
}
