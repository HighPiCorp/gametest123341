using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Linq.Expressions;
using GTANetworkAPI;
using Newtonsoft.Json;
using System.Data;
using NeptuneEvo.Families;
using NeptuneEvo.SDK;
using MySqlConnector;
using NeptuneEvo.Houses;

namespace NeptuneEvo.Core
{
    class Resalecar : Script
    {
        private static nLog Log = new nLog("ResaleCar");

        #region массивы
        public static Dictionary<int,List<PlayerResalecars>> PlayersResalecars = new Dictionary<int, List<PlayerResalecars>>();
        public class PlayerResalecars
        {
            public string Name { get; set; }
            public uint Model { get; set; }
            public string ModelName { get; set; }
            public string Number { get; set; }
            public int VehID { get; set; }
            public int Slot { get; set; }
            public int Price { get; set; }
            public bool Family { get; set; }
            public int BizId { get; set; }

            public PlayerResalecars(string name, uint model, string modelName, string number, int slot, int price, bool family, int bizid, int vehId)
            {
                Name = name;
                Model = model;
                ModelName = modelName;
                Number = number;
                Slot = slot;
                Price = price;
                Family = family;
                BizId = bizid;
                VehID = vehId;

            }
        }
        public static List<int> slotstate = new List<int>();//0 free 1 busy 2 tempbusy
        private static List<Vector3> slotposition = new List<Vector3>()
        {
             new Vector3(69.44114, -1718.1122, 29.182661),
            new Vector3(67.354935, -1720.3345, 29.186052),
            new Vector3(65.673546, -1722.8243, 29.185263),
            new Vector3(58.410667, -1731.0848, 29.179752),
            new Vector3(56.513718, -1733.5532, 29.178928),
            new Vector3(49.841133, -1741.6846, 29.176624),
            new Vector3(47.795437, -1744.1501, 29.17606),
            new Vector3(41.588852, -1750.9869, 29.175467),
            new Vector3(39.905025, -1753.5919, 29.17557),
            new Vector3(37.867218, -1755.9373, 29.17509),
            new Vector3(36.01017, -1758.1345, 29.175386),
            new Vector3(33.937542, -1760.6799, 29.174934),
            new Vector3(31.977854, -1763.0364, 29.180616),
            new Vector3(29.847364, -1765.3147, 29.199635),
            new Vector3(24.26244, -1771.9316, 29.196663),
            new Vector3(22.543673, -1774.4694, 29.195421),
            new Vector3(20.56502, -1776.8146, 29.195026),
            new Vector3(50.421146, -1717.3253, 29.176346),
            new Vector3(48.678207, -1719.6965, 29.176117),
            new Vector3(46.353745, -1722.0016, 29.176094),
            new Vector3(44.630486, -1724.4557, 29.175926),
            new Vector3(42.560173, -1726.8776, 29.176348),
            new Vector3(40.819927, -1729.2295, 29.176273),
            new Vector3(38.519028, -1731.4993, 29.176294),
            new Vector3(36.905384, -1733.9143, 29.176258),
            new Vector3(34.669506, -1736.3627, 29.176405),
            new Vector3(32.651028, -1738.4484, 29.176506),
            new Vector3(30.76748, -1740.897, 29.176256),
            new Vector3(28.761759, -1743.3416, 29.176249),
            new Vector3(26.71432, -1745.3423, 29.176374),
            new Vector3(24.853615, -1747.8251, 29.175972),
            new Vector3(22.893068, -1750.1981, 29.176205),
            new Vector3(20.82157, -1752.7106, 29.17603),
            new Vector3(19.09902, -1754.8619, 29.175756),
            new Vector3(16.865038, -1757.4763, 29.175852),
            new Vector3(15.051952, -1759.8418, 29.17582),
            new Vector3(12.902478, -1761.8827, 29.176079),
            new Vector3(10.969537, -1764.4542, 29.176085),
            new Vector3(9.110252, -1766.7184, 29.175833),
            new Vector3(46.594612, -1714.0283, 29.1761),
            new Vector3(44.571705, -1716.4279, 29.175776),
            new Vector3(42.737103, -1718.793, 29.175884),
            new Vector3(40.783382, -1721.3064, 29.176218),
            new Vector3(38.69854, -1723.515, 29.175856),
            new Vector3(36.885433, -1725.9177, 29.175785),
            new Vector3(34.85828, -1728.2571, 29.175856),
            new Vector3(32.727444, -1730.6074, 29.176193),
            new Vector3(30.710949, -1732.9305, 29.175814),
            new Vector3(28.973513, -1735.177, 29.176165),
            new Vector3(27.021118, -1737.5378, 29.176208),
            new Vector3(24.898214, -1739.8527, 29.17639),
            new Vector3(23.019573, -1742.2334, 29.176098),
            new Vector3(20.961506, -1744.7024, 29.175867),
            new Vector3(19.022097, -1746.9059, 29.175877),
            new Vector3(16.974707, -1749.303, 29.175829),
            new Vector3(14.902537, -1751.5879, 29.175898),
            new Vector3(12.989951, -1754.0736, 29.176176),
            new Vector3(11.023102, -1756.4545, 29.176075),
            new Vector3(9.241685, -1758.7837, 29.17405),
            new Vector3(7.161616, -1761.2214, 29.172716),
            new Vector3(5.0553865, -1763.5984, 29.166002),
            new Vector3(10.207979, -1730.8013, 29.175982),
            new Vector3(8.3068695, -1733.1777, 29.176052),
            new Vector3(6.340274, -1735.5358, 29.176044),
            new Vector3(4.233033, -1737.8922, 29.17651),
            new Vector3(2.3773441, -1740.364, 29.17891),
            new Vector3(0.35337558, -1742.7859, 29.177917),
            new Vector3(-1.6435261, -1745.0663, 29.176046),
            new Vector3(-3.715188, -1747.3333, 29.176003),
            new Vector3(-5.6370964, -1749.785, 29.176111),
            new Vector3(-7.563104, -1752.05, 29.175978),
            new Vector3(-5.7383227, -1741.8259, 29.176245),
            new Vector3(-7.6046853, -1744.0687, 29.175823),
            new Vector3(-9.586215, -1746.3619, 29.17601),
            new Vector3(-11.535725, -1748.7928, 29.175785),
            new Vector3(-34.929478, -1732.7278, 29.181501),
            new Vector3(-32.018536, -1731.2976, 29.168308),
            new Vector3(-29.248066, -1730.2571, 29.168283),
            new Vector3(-26.559586, -1729.0863, 29.168718),
            new Vector3(-23.629295, -1727.9531, 29.170368),
            new Vector3(-20.91187, -1726.6615, 29.182697),
            new Vector3(-5.9627833, -1720.7361, 29.172663),
            new Vector3(-3.0679488, -1719.3168, 29.167837),
            new Vector3(-0.3184106, -1718.2516, 29.168512),
            new Vector3(2.5618963, -1717.327, 29.169304),
            new Vector3(5.4473057, -1715.8365, 29.16884),
            new Vector3(8.29214, -1714.7262, 29.169456),
            new Vector3(11.242241, -1713.4982, 29.169613),
            new Vector3(13.892045, -1712.4128, 29.169313),
            new Vector3(16.630276, -1711.2955, 29.169287),
            new Vector3(19.6996, -1710.2188, 29.169641),
            new Vector3(22.58626, -1708.8815, 29.169193),
            new Vector3(25.40361, -1707.8057, 29.16919),
            new Vector3(28.279133, -1706.4789, 29.169706),
            new Vector3(31.272926, -1705.487, 29.17008),
            new Vector3(33.792324, -1704.324, 29.170012),

        };
        #endregion

        [ServerEvent(Event.ResourceStart)]
        public static void OnResourceStart()
        {
            for (int i = 0; i < slotposition.Count; i++)
            {
                slotstate.Add(0);
            }
            var result = MySQL.QueryRead($"SELECT * FROM `resalecars`");
            if (result == null || result.Rows.Count == 0)
            {
                Log.Write("DB return null result.", nLog.Type.Warn);
                return;
            }

            foreach (DataRow Row in result.Rows)
            {
                try
                {
                    var uuid = Convert.ToInt32(Row["uuid"].ToString());
                    var vehicledata = JsonConvert.DeserializeObject<List<PlayerResalecars>>(Row["vehiclesdata"].ToString());
                    //if (vehicledata.Count == 0) continue;

                    foreach (var cardata in vehicledata)
                    {
                        slotstate[cardata.Slot] = 1;
                        var veh = VehicleManager.Vehicles[cardata.VehID];
                        var Car = NAPI.Vehicle.CreateVehicle((VehicleHash)NAPI.Util.GetHashKey(veh.Model), slotposition[cardata.Slot] + new Vector3(0,0, -0.42), new Vector3(0, 0, -136.248), 0, 0);
                        Car.NumberPlate = cardata.Number;
                        Car.SetData("ACCESS", "RESALE");
                        Car.SetSharedData("PETROL", veh.Fuel);
                        Car.SetSharedData("FREEZE", true);
                        Car.SetData("OWNER", uuid);
                        Car.SetData("ID", cardata.VehID);
                        VehicleManager.ApplyCustomization(Car);
                        VehicleStreaming.SetEngineState(Car, false);
                        VehicleStreaming.SetLockStatus(Car, false);
                    }

                    PlayersResalecars.Add(uuid, vehicledata);
                }
                catch (Exception e)
                {
                    Log.Write(Row["uuid"].ToString() + e.StackTrace, nLog.Type.Error);
                }
            }
        }

        [RemoteEvent("resalebuycar_server")]
        public static void BuyCar(Player player, bool isfamily, int type = 0)
        {
            try
            {
                if (!player.IsInVehicle || !player.Vehicle.HasData("ACCESS") || player.Vehicle.GetData<string>("ACCESS") != "RESALE")
                {
                    VehicleManager.WarpPlayerOutOfVehicle(player);
                    return;
                }
                //if (player.Vehicle.GetData<int>("OWNER") == Main.Players[player].UUID)
                //{
                //    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Это ваша машина. Вы можете снять ее с продажи и вернуться обратно.", 3000);
                //    VehicleManager.WarpPlayerOutOfVehicle(player);
                //    return;
                //}

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

                    var house = FamilyHouseManager.GetHouse(player);

                    if (house == null)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У вас нет купленого дома", 3000);
                        VehicleManager.WarpPlayerOutOfVehicle(player);
                        return;
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
                        return;
                    }
                }
                else
                {
                    var garage = NeptuneEvo.Houses.GarageManager.Garages[playerHouse.GarageID];
                    // С гаражем
                    if (VehicleManager.getAllPlayerVehicles(player.Name).Count >= NeptuneEvo.Houses.GarageManager.GarageTypes[garage.Type].MaxCars)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Ваши гаражи полны", 3000);
                        return;
                    }
                }

                var selleruuid = player.Vehicle.GetData<int>("OWNER");
                var vehId = player.Vehicle.GetData<int>("ID");
                var tempVehicle = PlayersResalecars[selleruuid].FirstOrDefault(v => v.VehID == vehId);
                if (tempVehicle == null) return;

                Business biz = BusinessManager.BizList[tempVehicle.BizId];
                if(type == 1)
                {
                    if (Main.Players[player].Money < tempVehicle.Price)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Недостаточно денег для покупки", 3000);
                        VehicleManager.WarpPlayerOutOfVehicle(player);
                        return;
                    }
                }
                else if(type == 2)
                {
                    if(MoneySystem.Bank.Accounts[Main.Players[player].Bank].Balance < tempVehicle.Price)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Недостаточно денег для покупки", 3000);
                        VehicleManager.WarpPlayerOutOfVehicle(player);
                        return;
                    }
                }
                if (isfamily)
                {
                    if (Main.Players[player].Money < tempVehicle.Price)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Недостаточно денег для покупки", 3000);
                        VehicleManager.WarpPlayerOutOfVehicle(player);
                        return;
                    }
                }
                if (!BusinessManager.takeProd(biz.ID, 1, "бланк договора", biz.Products[0].Price))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Недостаточно бланков договора на складе", 3000);
                   return;
                }
                GameLog.Money($"player({Main.Players[player].UUID})", $"biz({biz.ID})", biz.Products[0].Price, "resalecars");
                Player oldOwner = Main.GetPlayerByUUID(selleruuid);

                var veh = player.Vehicle;
                var name = player.Name;
                if (isfamily)
                {
                    veh.SetData("ACCESS", "FAMILY");
                    veh.SetData("FAMILY", family.FamilyCID);
                    name = family.FamilyCID;
                }
                else
                {
                    client.Core.Achievements.AddAchievementScore(player, client.Core.AchievementID.CarLover, 1);
                    veh.SetData("ACCESS", "PERSONAL");
                }

                veh.ResetSharedData("FREEZE");
                Trigger.ClientEvent(player, "freezeVeh", false);
                veh.SetData("OWNER", player);
                player.SetMyData("IN_RESALE_CAR", false);

                if (type != 2)
                {
                    MoneySystem.Wallet.Change(player, -tempVehicle.Price);
                }
                else
                {
                    MoneySystem.Bank.Change(Main.Players[player].Bank, -tempVehicle.Price);
                }

                VehicleManager.Vehicles[vehId].Holder = name;
                VehicleManager.Vehicles[vehId].OwnerID = Main.Players[player].UUID;

                //MySQL.Query($"UPDATE `vehicles` SET `holder` = '{name}', `ownerid` = '{Main.Players[player].UUID}' WHERE `id` = '{tempVehicle.VehID}'");
                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = "UPDATE `vehicles` SET " +
                  "`holder`=@holder," +
                  "`ownerid`=@ownerid" +
                  " WHERE `id`=@id";

                cmd.Parameters.AddWithValue("@holder", name);
                cmd.Parameters.AddWithValue("@ownerid", Main.Players[player].UUID);
                cmd.Parameters.AddWithValue("@id", tempVehicle.VehID);
                MySQL.Query(cmd);

                if (oldOwner != null && Main.Players.ContainsKey(oldOwner))
                {
                    MoneySystem.Wallet.Change(oldOwner, tempVehicle.Price - biz.Products[0].Price);
                    Notify.Send(oldOwner, NotifyType.Success, NotifyPosition.BottomCenter, $"Вашу машину купили на ResaleCar", 3000);
                    if(veh.GetData<string>("ACCESS") == "PERSONAL")
                    {
                        client.Core.Achievements.RemoveAchievementScore(oldOwner, client.Core.AchievementID.CarLover, 1);
                    }
                }
                else
                {
                    string[] split = Main.PlayerNames[selleruuid].Split('_');
                    //MySQL.Query($"UPDATE characters SET money=money+{Convert.ToInt32(tempVehicle.Price - biz.Products[0].Price)} WHERE firstname='{split[0]}' AND lastname='{split[1]}'");
                    MySqlCommand cmd2 = new MySqlCommand();
                    cmd2.CommandText = "UPDATE `characters` SET " +
                      "`money`=`money` + @money" +
                      " WHERE `firstname`=@firstname AND `lastname`=@lastname";

                    cmd2.Parameters.AddWithValue("@money", Convert.ToInt32(tempVehicle.Price - biz.Products[0].Price));
                    cmd2.Parameters.AddWithValue("@firstname", split[0]);
                    cmd2.Parameters.AddWithValue("@lastname", split[1]);
                    MySQL.Query(cmd2);
                }
                slotstate[tempVehicle.Slot] = 0;
                PlayersResalecars[selleruuid].Remove(tempVehicle);
                Save(selleruuid);
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы успешно купили машину", 3000);
                GameLog.Money($"player({Main.Players[player].UUID})", $"server", tempVehicle.Price, $"resaleCar");
            }
            catch (Exception e)
            {
                Log.Write("Enter resalecar" + e.StackTrace, nLog.Type.Error);
            }


        }

        [ServerEvent(Event.PlayerEnterVehicle)]
        public void onPlayerEnterVehicleHandler(Player player, Vehicle vehicle, sbyte seatid)
        {
            try
            {
                if (!vehicle.HasData("ACCESS") || vehicle.GetData<string>("ACCESS") != "RESALE" || seatid != 0) return;
                Log.Debug($"ACCESS VEH: {vehicle.GetData<string>("ACCESS")} OWNER: {vehicle.GetData<int>("OWNER")}");
                var uuid = vehicle.GetData<int>("OWNER");
                player.SetMyData("IN_RESALE_CAR", true);
                var tempVehicle = PlayersResalecars[vehicle.GetData<int>("OWNER")].FirstOrDefault(v => v.VehID == vehicle.GetData<int>("ID"));
                if (tempVehicle == null) return;
                var vehData = VehicleManager.Vehicles[tempVehicle.VehID].Components;

                var vehicleNumber = tempVehicle.VehID;
                var vehicleModel = VehicleManager.Vehicles[tempVehicle.VehID].Model;
                var vehicleName = vehicle.DisplayName;
                if (vehicleName == null)
                {
                    var vehicleData = VehicleManager.Vehicles[tempVehicle.VehID];
                    vehicleName = VehicleManager.GetVehicleRealName(VehicleManager.Vehicles[tempVehicle.VehID].Model);
                }


                if (BusinessManager.ProductsOrderPrice.ContainsKey(VehicleManager.Vehicles[tempVehicle.VehID].Model))
                {

                    List<object> data = new List<object>
                {
                    vehicleName,
                    vehicleNumber,
                    new List<int>
                    {
                      vehData.Engine+1,
                      vehData.Brakes+1,
                      vehData.Transmission+1,
                      vehData.Suspension+1,
                      vehData.Turbo+1
                    },
                    Main.PlayerNames[uuid],
                    tempVehicle.Price,
                    vehicleModel,
                };
                    string json = JsonConvert.SerializeObject(data);
                    Trigger.ClientEvent(player, "openbycar", json);
                }
                // Trigger.ClientEvent(player, "popup::open", "RESALE_CAR", $"Вы хотите купить этот транспорт за ${tempVehicle.Price}?");

            }
            catch (Exception e)
            {
                Log.Write("Enter resalecar"+ e.StackTrace, nLog.Type.Error);
            }

        }

        [ServerEvent(Event.PlayerExitVehicle)]
        public void Event_OnPlayerExitVehicle(Player player, Vehicle vehicle)
        {
            try
            {
                if (!vehicle.HasData("ACCESS") || vehicle.GetData<string>("ACCESS") != "RESALE" ) return;
                player.SetMyData("IN_RESALE_CAR", false);
            }
            catch (Exception e) { Log.Write("Player Exit  resalecar: " + e.StackTrace, nLog.Type.Error); }
        }

        [RemoteEvent("cancelSale_server")]
        public void cancelSale_server(Player player,int number)
        {
            try
            {
                var tempVehicle = PlayersResalecars[Main.Players[player].UUID].FirstOrDefault(v => v.VehID == number);
                if (tempVehicle == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Автомобиль с таким номером найти не удалось", 3000);
                    return;
                }
                var name = player.Name;
                Vehicle veh = null;

                var house = NeptuneEvo.Houses.HouseManager.GetHouse(player, true);
                if (house == null || house.GarageID == 0)
                {
                    // Без гаража
                    if (VehicleManager.getAllPlayerVehicles(player.Name).Count >= GarageManager.MAX_VEHICLES_WITHOUT_GARAGE)
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

                foreach (Vehicle vehicle in NAPI.Pools.GetAllVehicles())
                {
                    if(vehicle.HasData("ID"))
                    {
                        if (vehicle.GetData<int>("ID") == number) veh = vehicle;
                    }
                }

                if (tempVehicle.Family)
                {
                    name = Main.Players[player].FamilyCID;
                    veh.SetData("ACCESS", "FAMILY");
                    veh.SetData("FAMILY", name);
                }
                else
                {
                    veh.SetData("ACCESS", "PERSONAL");
                }

                veh.ResetSharedData("FREEZE");
                Trigger.ClientEvent(player, "freezeVehV2", veh, false);

                veh.SetData("OWNER", player);
                VehicleManager.Vehicles[tempVehicle.VehID].Holder = name;

                //MySQL.Query($"UPDATE `vehicles` SET `holder` = '{name}', `ownerid` = '{Main.Players[player].UUID}' WHERE `id` = '{tempVehicle.VehID}'");
                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = "UPDATE `vehicles` SET " +
                  "`holder`=@holder," +
                  "`ownerid`=@ownerid" +
                  " WHERE `id`=@id";

                cmd.Parameters.AddWithValue("@holder", name);
                cmd.Parameters.AddWithValue("@ownerid", Main.Players[player].UUID);
                cmd.Parameters.AddWithValue("@id", tempVehicle.VehID);
                MySQL.Query(cmd);

                slotstate[tempVehicle.Slot] = 0;
                PlayersResalecars[Main.Players[player].UUID].Remove(tempVehicle);
                Save(Main.Players[player].UUID);
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы сняли авто с продажи", 3000);

            }
            catch (Exception e) { Log.Write("salecar: message " + e.StackTrace + " method " + e.TargetSite + " source " + e.Source + " stack " + e.StackTrace, nLog.Type.Error); }
        }

        [RemoteEvent("SaletoGov_server")]
        public void SaletoGov_server(Player player, int number)
        {
            try
            {
                var selleruuid = Main.Players[player].UUID;
                var tempVehicle = PlayersResalecars[selleruuid].FirstOrDefault(v => v.VehID == number);
                if (tempVehicle == null) return;
                player.SetMyData("RESALECARSELLGOV", number);
                VehicleManager.VehicleData vData = VehicleManager.Vehicles[number];
                int price = 0;
                if (BusinessManager.ProductsOrderPrice.ContainsKey(vData.Model))
                {
                    switch (Main.Players[player].VipLvl)
                    {
                        case 0: // None
                            price = Convert.ToInt32(BusinessManager.ProductsOrderPrice[vData.Model] * 0.4);
                            break;
                        case 1: // Bronze
                            price = Convert.ToInt32(BusinessManager.ProductsOrderPrice[vData.Model] * 0.5);
                            break;
                        case 2: // Silver
                            price = Convert.ToInt32(BusinessManager.ProductsOrderPrice[vData.Model] * 0.5);
                            break;
                        case 3: // Gold
                            price = Convert.ToInt32(BusinessManager.ProductsOrderPrice[vData.Model] * 0.5);
                            break;
                        case 4: // Platinum
                            price = Convert.ToInt32(BusinessManager.ProductsOrderPrice[vData.Model] * 0.6);
                            break;
                        default:
                            price = Convert.ToInt32(BusinessManager.ProductsOrderPrice[vData.Model] * 0.4);
                            break;
                    }
                }
                Trigger.ClientEvent(player, "popup::open", "RESALECAR_SELL_TOGOV", $"Вы действительно хотите продать {vData.Model} с номером ({number}) за ${price}?");
            }
            catch (Exception e) { Log.Write("salecar: message " + e.StackTrace + " method " + e.TargetSite + " source " + e.Source + " stack " + e.StackTrace, nLog.Type.Error); }
        }
        [RemoteEvent("setsalecar_server")]
        public void setsalecar_server(Player player)
        {
            try
            {
                var freeslot = slotstate.FirstOrDefault(s => s == 0);
                if (freeslot != 0)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "На данный момент все места заняты, попробуйте позже", 3000);
                    return;
                }

                var index = slotstate.IndexOf(freeslot);
                slotstate[index] = 2;
                Console.WriteLine("set slot "+ index);
                player.SetMyData("RESALEBIZ_ID", player.GetMyData<int>("BIZ_ID"));
                Trigger.ClientEvent(player, "RouteResalecar", slotposition[index].X, slotposition[index].Y, slotposition[index].Z, index);

                NAPI.Task.Run(() => {
                    if (slotstate[index] == 2)
                    {
                        slotstate[index] = 0;
                        if (!Main.Players.ContainsKey(player)) return;
                        Trigger.ClientEvent(player, "cancelresale");
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы не выставляли машину на продажу {index+1}", 3000);

                    }
                }, 60000);


            }
            catch (Exception e) { Log.Write("salecar: message " + e.StackTrace + " method " + e.TargetSite + " source " + e.Source + " stack " + e.StackTrace, nLog.Type.Error); }
        }
        [RemoteEvent("setResalecar")]
        public void setResalecar(Player player,int slot)
        {
            try
            {
                if (!player.IsInVehicle)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы должны находиться в автомобиле", 3000);
                    Trigger.ClientEvent(player, "cancelresale");
                    return;
                }
                var veh = player.Vehicle;
                var name = player.Name;

                if(!veh.HasData("ID"))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Автомобиль не найден", 3000);
                    Trigger.ClientEvent(player, "cancelresale");
                    return;
                }

                var vehicleId = veh.GetData<int>("ID");
                if (!VehicleManager.Vehicles.ContainsKey(vehicleId))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Автомобиль не найден", 3000);
                    Trigger.ClientEvent(player, "cancelresale");
                    return;
                }

                if (veh.HasData("FAMILY") && Main.Players[player].FamilyCID != "null" && Main.Players[player].FamilyRank >= 10)
                {
                    name = Main.Players[player].FamilyCID;
                    Console.WriteLine("name owner" + name);
                }

                if (VehicleManager.Vehicles[vehicleId].Holder != name)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Это не Ваша машина", 3000);
                    Trigger.ClientEvent(player, "cancelresale");
                    return;
                }

                player.SetMyData("RESALE_CAR_SLOT", slot);
                Trigger.ClientEvent(player, "caninteraction", false);
                Trigger.ClientEvent(player, "popup::openInput", "Продажа автомобиля", "Укажите цену продажи", 9, "resale_car_set");

            }
            catch (Exception e) { Log.Write("setResalecar: " + e.StackTrace, nLog.Type.Error); }
        }

        public static void SetCar(Player player,int price)
        {
            try
            {
                int slot = player.GetMyData<int>("RESALE_CAR_SLOT");
                if (!player.IsInVehicle)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы должны находиться в автомобиле", 3000);
                    return;
                }
                var bizid = player.GetMyData<int>("RESALEBIZ_ID");

                if(bizid == -1)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы должны быть ближе к метке", 3000);
                    return;
                }
                Business biz = BusinessManager.BizList[bizid];
                if (price <= biz.Products[0].Price)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Невыгодная цена продажи {biz.Products[0].Price}", 3000);
                    player.ResetMyData("RESALEBIZ_ID");
                    return;
                }
                var veh = player.Vehicle;
                var name = player.Name;
                var v = VehicleManager.Vehicles.FirstOrDefault(v => v.Key == veh.GetData<int>("ID")).Key;

                int parkId = ParkingManager.GetCarInRentPark(veh.GetData<int>("ID"));

                if (parkId != -1)
                {
                    ParkingPlace place = ParkingManager.GetVehicleParkPlace(player, parkId, veh.GetData<int>("ID"));

                    place.CancelOrder(player);

                    Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, "Парковка для данного авто отменена!", 3000);
                }

                if (veh.HasData("FAMILY") && Main.Players[player].FamilyCID != "null" && Main.Players[player].FamilyRank >= 10)
                {
                    name = Main.Players[player].FamilyCID;
                    Console.WriteLine("name owner2" + name);
                }
                if (VehicleManager.Vehicles[v].Holder != name)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Это не Ваша машина", 3000);
                    Trigger.ClientEvent(player, "cancelresale");
                    return;
                }
                int uuid = Main.Players[player].UUID;
                veh.SetData("ACCESS", "RESALE");
                veh.SetData("OWNER", uuid);
                VehicleStreaming.SetEngineState(veh, false);
                VehicleManager.WarpPlayerOutOfVehicle(player);
                var number = veh.GetData<int>("ID");
                var vehicleHash = Convert.ToString(veh.HashCode);
                var vehicleName = veh.DisplayName;
                if (vehicleName == null)
                {
                    var vehicleData = VehicleManager.Vehicles[number];
                    vehicleName = VehicleManager.GetVehicleRealName(VehicleManager.Vehicles[number].Model);
                }

                var vehicleModel = veh.Model;
                VehicleManager.Vehicles[number].Holder = "RESALE";
                //MySQL.Query($"UPDATE `vehicles` SET `holder` = 'RESALE' WHERE `id` = '{number}'");

                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = "UPDATE `vehicles` SET " +
                  "`holder`=@holder" +
                  " WHERE `id`=@id";

                cmd.Parameters.AddWithValue("@holder", "RESALE");
                cmd.Parameters.AddWithValue("@id", number);
                MySQL.Query(cmd);

                var family = false;
                if (veh.HasData("FAMILY")) family = true;

                slotstate[slot] = 1;

                if (PlayersResalecars.ContainsKey(uuid))
                {
                    PlayersResalecars[uuid].Add(new PlayerResalecars(vehicleName, vehicleModel, VehicleManager.Vehicles[number].Model, veh.NumberPlate, slot, price, family, bizid, number));
                    Save(uuid);
                }
                else
                {
                    List<PlayerResalecars> temp = new List<PlayerResalecars>();
                    temp.Add(new PlayerResalecars(vehicleName, vehicleModel, VehicleManager.Vehicles[number].Model, veh.NumberPlate, slot, price, family, bizid, number));
                    PlayersResalecars.Add(uuid, temp);
                    //MySQL.Query($"INSERT INTO resalecars (uuid,vehiclesdata) VALUES ({uuid},'{JsonConvert.SerializeObject(PlayersResalecars[uuid])}')");

                    MySqlCommand cmd2 = new MySqlCommand();
                    cmd2.CommandText = "INSERT INTO `resalecars` SET " +
                      "`uuid`=@uuid," +
                      "`vehiclesdata`=@vehiclesdata";

                    cmd2.Parameters.AddWithValue("@uuid", uuid);
                    cmd2.Parameters.AddWithValue("@vehiclesdata", JsonConvert.SerializeObject(PlayersResalecars[uuid]));
                    MySQL.Query(cmd2);
                }

                var house = HouseManager.GetHouse(player, true);

                if(house != null)
                {
                    if(house.GarageID != -1)
                    {
                        if(GarageManager.Garages[house.GarageID].vehiclesOut.ContainsKey(number))
                            GarageManager.Garages[house.GarageID].vehiclesOut.Remove(number);
                    }
                }

                if (GarageManager.vehiclesOutPark.ContainsKey(number))
                {
                    GarageManager.vehiclesOutPark.Remove(number);
                }

                player.ResetMyData("RESALEBIZ_ID");
                Trigger.ClientEvent(player, "cancelresale");
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Вы выставили машину на продажу", 3000);


            }
            catch (Exception e)
            {
                Log.Write("Enter resalecar" + e.StackTrace, nLog.Type.Error);
            }
        }

        public static bool Save(int uuid)
        {
            if (!PlayersResalecars.ContainsKey(uuid)) return false;
            var items = JsonConvert.SerializeObject(PlayersResalecars[uuid]);
            if (string.IsNullOrEmpty(items) || items == null) items = "[]";
            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = "UPDATE `resalecars` SET vehiclesdata=@vehiclesdata WHERE uuid=@uuid";
            cmd.Parameters.AddWithValue("@uuid", uuid);
            cmd.Parameters.AddWithValue("@vehiclesdata", items);

            MySQL.Query(cmd);

            return true;
        }

        public static void interactionPressed(Player player)
        {
            Trigger.ClientEvent(player, "openResalecar", (PlayersResalecars.ContainsKey(Main.Players[player].UUID)) ? JsonConvert.SerializeObject(PlayersResalecars[Main.Players[player].UUID]) : "[]");
        }
    }
}
