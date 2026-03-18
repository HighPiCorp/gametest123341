using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Text;
using GTANetworkAPI;
using NeptuneEvo;
using NeptuneEvo.Core;
using NeptuneEvo.Families;
using NeptuneEvo.Houses;
using NeptuneEvo.SDK;
using Newtonsoft.Json;
using Trigger = NeptuneEvo.Trigger;
using Color = GTANetworkAPI.Color;
using MySqlConnector;
using client.Systems.BattlePass;

namespace client.Core
{
    class ExclusiveSalon : Script
    {
        private static nLog Log = new nLog("ExclusiveSalon");

        public static int HoursToUpdate = 50;
        public static Random random = new Random();
        public static List<Vector3> Positions = new List<Vector3>()
        {
            new Vector3(-765.2657, -291.18433, 37.329952), // 1
            new Vector3(-766.9363, -299.13834, 37.330105), // 2
            new Vector3(-760.1377, -303.77814, 37.3301),// 3
            new Vector3(-754.6329, -306.0724, 37.33), // 4
            new Vector3(-755.1955, -294.1749, 37.32953),// 5
            new Vector3(-768.4582, -275.4513, 37.33004),
            new Vector3(-765.6023, -282.29453, 37.329453),
            new Vector3(-756.15875, -278.62357, 37.330177),
            new Vector3(-759.37616, -271.7749, 37.32984), // 9

        };

        public static Dictionary<string, ExclusiveVehicle> AllVehicles = new Dictionary<string, ExclusiveVehicle>();
        public static Dictionary<string, ExclusiveVehicle> VehiclesOnSale = new Dictionary<string, ExclusiveVehicle>();

        public static DateTime LastUpdate = DateTime.Now;

        [ServerEvent(Event.ResourceStart)]
        public void OnResourceStart()
        {
            var table = MySQL.QueryRead($"SELECT * FROM `exclusive_vehicles`");
            if (table == null || table.Rows.Count == 0)
            {
                Log.Write("Containers return null result.", nLog.Type.Warn);
                return;
            }
            int tmp = 0;
            foreach (DataRow Row in table.Rows)
            {
                ExclusiveVehicle veh = new ExclusiveVehicle(Convert.ToString(Row["model"]), Convert.ToBoolean(Row["on_sale"]), Convert.ToBoolean(Row["is_buy"]), Convert.ToInt32(Row["cars_count"]), Convert.ToInt32(Row["max_cars"]));

                AllVehicles.Add(Convert.ToString(Row["model"]), veh);

                if (veh.OnSale)
                {
                    LastUpdate = (DateTime)Row["last_update"];
                    AllVehicles[Convert.ToString(Row["model"])].SpawnVehicle(Positions[tmp]);
                    VehiclesOnSale.Add(Convert.ToString(Row["model"]), veh);
                    tmp++;
                }
            }

            Log.Write($"Загружено {VehiclesOnSale.Count} эксклюзивных авто");
            UpdateVehicles(true);
            NAPI.Task.Run(() => SetVehiclePrices(), 1000);
        }

        public static void UpdateVehicles(bool isFirst = false)
        {
            if(!isFirst)
                if (LastUpdate.AddHours(HoursToUpdate) > DateTime.Now) return;

            NAPI.Task.Run(() => {

                foreach (KeyValuePair<string, ExclusiveVehicle> pair in VehiclesOnSale)
                {
                    if (!pair.Value.IsBuy)
                    {
                        NAPI.Task.Run(() => {
                            pair.Value.Handle.Delete();
                            pair.Value.Handle = null;
                        });

                    }

                    pair.Value.IsBuy = false;
                    pair.Value.OnSale = false;

                    pair.Value.Save();
                }

                VehiclesOnSale.Clear();

                NAPI.Task.Run(() => {
                    int tmp = 0;

                    while (VehiclesOnSale.Count < 5)
                    {
                        int val = random.Next(0, AllVehicles.Count);

                        if (!VehiclesOnSale.ContainsKey(AllVehicles.ElementAt(val).Key) && AllVehicles.ElementAt(val).Value.MaxCars > AllVehicles.ElementAt(val).Value.CarsSaled)
                        {
                            VehiclesOnSale.Add(AllVehicles.ElementAt(val).Key, AllVehicles[AllVehicles.ElementAt(val).Key]);
                            VehiclesOnSale[AllVehicles.ElementAt(val).Key].OnSale = true;
                            VehiclesOnSale[AllVehicles.ElementAt(val).Key].Save();
                            VehiclesOnSale[AllVehicles.ElementAt(val).Key].SpawnVehicle(Positions[tmp]);
                            tmp++;
                        }
                    }
                }, 1000);
            });

            NAPI.Task.Run(() => SetVehiclePrices(), 2000);

        }

        public static void SetVehiclePrices()
        {
            foreach(KeyValuePair<string, ExclusiveVehicle> pair in VehiclesOnSale)
            {
                pair.Value.Handle.SetSharedData("exclusivePrice", BusinessManager.ProductsOrderPrice.ContainsKey(pair.Key) ? BusinessManager.ProductsOrderPrice[pair.Key] : 10000000);
                pair.Value.Handle.SetData("VEH_MODEL", pair.Key);
                pair.Value.Handle.SetData("ACCESS", "EXCLUSIVE");
                pair.Value.Handle.SetSharedData("FREEZE", true);
            }
        }

        [ServerEvent(Event.PlayerEnterVehicle)]
        public void onPlayerEnterVehicleHandler(Player player, Vehicle vehicle, sbyte seatid)
        {
            try
            {
                if (!vehicle.HasData("ACCESS") || vehicle.GetData<string>("ACCESS") != "EXCLUSIVE" || seatid != 0) return;

                player.SetMyData("IN_EXCLUSIVE_CAR", true);

                string model = vehicle.GetData<string>("VEH_MODEL");

                List<object> data = new List<object>
                {
                    VehicleManager.GetVehicleRealName(model),
                     BusinessManager.ProductsOrderPrice.ContainsKey(model) ? BusinessManager.ProductsOrderPrice[model] : 1000000 ,
                    model,
                    VehiclesOnSale[model].CarsSaled,
                    VehiclesOnSale[model].MaxCars,
                };
                string json = JsonConvert.SerializeObject(data);
                Trigger.ClientEvent(player, "CLIENT::EXCLUSIVE:OPEN", json);


            }
            catch (Exception e)
            {
                Log.Write("Enter ExclusiveCar" + e.StackTrace, nLog.Type.Error);
            }

        }

        [ServerEvent(Event.PlayerExitVehicle)]
        public void Event_OnPlayerExitVehicle(Player player, Vehicle vehicle)
        {
            try
            {
                if (!vehicle.HasData("ACCESS") || vehicle.GetData<string>("ACCESS") != "EXCLUSIVE") return;
                player.SetMyData("IN_EXCLUSIVE_CAR", false);
            }
            catch (Exception e) { Log.Write("Player Exit  ExclusiveCar: " + e.StackTrace, nLog.Type.Error); }
        }

        [RemoteEvent("SERVER::EXCLUSIVE:BUY_CAR")]
        public static void BuyCar(Player player, bool isfamily, int type = 0)
        {
            try
            {
                if (!player.IsInVehicle || !player.Vehicle.HasData("ACCESS") || player.Vehicle.GetData<string>("ACCESS") != "EXCLUSIVE")
                {
                    VehicleManager.WarpPlayerOutOfVehicle(player);
                    return;
                }


                Family family = Family.GetFamilyToCid(player);
                if (isfamily)
                {
                    if (family == null)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы не член семьи", 3000);
                        VehicleManager.WarpPlayerOutOfVehicle(player);
                        return;
                    }
                    if (family.Leader != Main.Players[player].UUID)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Только лидер может покупать машины", 3000);
                        VehicleManager.WarpPlayerOutOfVehicle(player);
                        return;
                    }
                    var famhouse = FamilyHouseManager.GetHouse(player);

                    if (famhouse == null)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У вас нет купленого дома", 3000);
                        VehicleManager.WarpPlayerOutOfVehicle(player);
                        return;
                    }
                }

                string model = player.Vehicle.GetData<string>("VEH_MODEL");

                ExclusiveVehicle tempVehicle = VehiclesOnSale[model];

                int price = BusinessManager.ProductsOrderPrice.ContainsKey(model) ? BusinessManager.ProductsOrderPrice[model] : 1000000;

                var house = NeptuneEvo.Houses.HouseManager.GetHouse(player, true);
                if (house == null || house.GarageID == 0)
                {
                    // Без гаража
                    if(VehicleManager.getAllPlayerVehicles(player.Name).Count >= GarageManager.MAX_VEHICLES_WITHOUT_GARAGE)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Чтобы иметь более двух автомобилией, приобретите дом с местом для машины", 3000);
                        VehicleManager.WarpPlayerOutOfVehicle(player);
                        return;
                    }
                }
                else
                {
                    var garage = NeptuneEvo.Houses.GarageManager.Garages[house.GarageID];
                    // С гаражем
                    if (VehicleManager.getAllPlayerVehicles(player.Name).Count >= NeptuneEvo.Houses.GarageManager.GarageTypes[garage.Type].MaxCars)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Ваши гаражи полны", 3000);
                        return;
                    }
                }

                if (type == 1)
                {
                    if (Main.Players[player].Money < price)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Недостаточно денег для покупки", 3000);
                        VehicleManager.WarpPlayerOutOfVehicle(player);
                        return;
                    }
                }
                else if (type == 2)
                {
                    if (NeptuneEvo.MoneySystem.Bank.Accounts[Main.Players[player].Bank].Balance < price)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Недостаточно денег для покупки", 3000);
                        VehicleManager.WarpPlayerOutOfVehicle(player);
                        return;
                    }
                } else {
                    if (Main.Players[player].Money < price)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Недостаточно денег для покупки", 3000);
                        VehicleManager.WarpPlayerOutOfVehicle(player);
                        return;
                    }
                }

                var name = player.Name;
                if (isfamily)
                {
                    tempVehicle.Handle.SetData("ACCESS", "FAMILY");
                    tempVehicle.Handle.SetData("FAMILY", family.FamilyCID);
                    name = family.FamilyCID;
                }
                else
                {
                    client.Core.Achievements.AddAchievementScore(player, client.Core.AchievementID.CarLover, 1);
                    tempVehicle.Handle.SetData("ACCESS", "PERSONAL");
                }

                tempVehicle.Handle.SetData("OWNER", player);
                tempVehicle.Handle.ResetSharedData("FREEZE");
                Trigger.ClientEvent(player, "freezeVeh", false);

                player.SetMyData("IN_EXCLUSIVE_CAR", false);

                if (type != 2)
                {
                    NeptuneEvo.MoneySystem.Wallet.Change(player, -price);
                }
                else
                {
                    NeptuneEvo.MoneySystem.Bank.Change(Main.Players[player].Bank, -price);
                }

                int id = VehicleManager.Create(player.Name, Main.Players[player].UUID, model, tempVehicle.Handle.CustomPrimaryColor, tempVehicle.Handle.CustomSecondaryColor, new Color(0, 0, 0, 0));

                tempVehicle.Handle.SetData("ID", id);
                tempVehicle.Handle.NumberPlate = "";
                tempVehicle.Handle.ResetSharedData("exclusivePrice");

                VehicleManager.Vehicles[id].OtherData.ExclusiveNumber = tempVehicle.CarsSaled + 1;

                tempVehicle.Buy();

                if (!isfamily)
                {
                    if (house != null && house.GarageID != 0)
                    {
                        GarageManager.Garages[house.GarageID].vehiclesOut.Add(id, tempVehicle.Handle);
                    }
                }

                GameLog.Money($"player({Main.Players[player].UUID})", $"exclusiveCar", price, "exclusivecars");

                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы купили авто {VehicleManager.GetVehicleRealName(model)} за {price}$");

                #region GBPКвест: 16 Купить автомобилей на 200.000.000$

                #region BattlePass выполнение квеста
                BattlePass.updateBPGlobalQuestIteration(player, BattlePass.BPGlobalQuestType.BuyCars, price);
                #endregion

                #endregion

            }
            catch (Exception e)
            {
                Log.Write("Enter resalecar" + e.StackTrace, nLog.Type.Error);
            }
        }

    }

    public class ExclusiveVehicle
    {
        public string Model { get; set; }
        public bool IsBuy { get; set; }
        public bool OnSale { get; set; }
        public int MaxCars { get; set; }
        public int CarsSaled { get; set; }
        public Vehicle Handle { get; set; }

        public ExclusiveVehicle(string model, bool onSale, bool isBuy, int carsSale, int maxcars)
        {
            //Handle = NAPI.Vehicle.CreateVehicle(NAPI.Util.GetHashKey(model), pos, Convert.ToSingle(ExclusiveSalon.random.NextDouble() * 360.0), ExclusiveSalon.random.Next(0, 158), ExclusiveSalon.random.Next(0, 158), "EXCLUSIVE", dimension: 0);

            Model = model;
            IsBuy = isBuy;
            OnSale = onSale;
            CarsSaled = carsSale;
            MaxCars = maxcars;
        }

        public void SpawnVehicle(Vector3 pos)
        {
            NAPI.Task.Run(() => {
                int color = ExclusiveSalon.random.Next(0, 158);

                Handle = NAPI.Vehicle.CreateVehicle(NAPI.Util.GetHashKey(Model), pos, Convert.ToSingle(ExclusiveSalon.random.NextDouble() * 360.0), color, color);
                Handle.NumberPlate = "EXCLUSIVE";
                Handle.Dimension = 0;

                Handle.SetSharedData("exclusivePrice", BusinessManager.ProductsOrderPrice.ContainsKey(Model) ? BusinessManager.ProductsOrderPrice[Model] : 10000000);
                Handle.SetData("VEH_MODEL", Model);
                Handle.SetData("ACCESS", "EXCLUSIVE");
                Handle.SetSharedData("FREEZE", true);
            }, 0);
        }

        public void Buy()
        {
            IsBuy = true;
            CarsSaled++;

           /* if(CarsSaled == MaxCars)
            {
                MySQL.Query($"DELETE FROM `exclusive_vehicles` WHERE `model` = '{Model}'");
            }*/

            //MySQL.Query($"UPDATE `exclusive_vehicles` SET `is_buy` = {Convert.ToInt32(IsBuy)}, `cars_count` = {CarsSaled} WHERE `model` = '{Model}'");

            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = "UPDATE `exclusive_vehicles` SET " +
              "`is_buy`=@is_buy," +
              "`cars_count`=@cars_count" +
              " WHERE `model`=@model";

            cmd.Parameters.AddWithValue("@is_buy", Convert.ToInt32(IsBuy));
            cmd.Parameters.AddWithValue("@cars_count", CarsSaled);
            cmd.Parameters.AddWithValue("@model", Model);
            MySQL.Query(cmd);
        }

        public void Save()
        {
            //MySQL.Query($"UPDATE `exclusive_vehicles` SET `on_sale` = {Convert.ToInt32(OnSale)},`is_buy` = {Convert.ToInt32(IsBuy)}, `cars_count` = {CarsSaled} WHERE `model` = '{Model}'");

            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = "UPDATE `exclusive_vehicles` SET " +
              "`on_sale`=@on_sale," +
              "`is_buy`=@is_buy," +
              "`cars_count`=@cars_count" +
              " WHERE `model`=@model";

            cmd.Parameters.AddWithValue("@on_sale", Convert.ToInt32(OnSale));
            cmd.Parameters.AddWithValue("@is_buy", Convert.ToInt32(IsBuy));
            cmd.Parameters.AddWithValue("@cars_count", CarsSaled);
            cmd.Parameters.AddWithValue("@model", Model);
            MySQL.Query(cmd);
        }


    }
}
