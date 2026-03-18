using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;
using NeptuneEvo.GUI;
using NeptuneEvo.SDK;
using NeptuneEvo.Rent;
using Newtonsoft.Json;

namespace NeptuneEvo.Core
{
    class Rentcar : Script
    {
        private static nLog Log = new nLog("Rentcar");
        public static List<CarInfo> CarInfos = new List<CarInfo>();

        private static List<Tuple<string, Vector3>> RentAreasTypes = new List<Tuple<string, Vector3>>()
        {
            //new Tuple<string, Vector3>("Авто", new Vector3(-526.91406, 61.327232, 51.459885)),
            //new Tuple<string, Vector3>("Скутеры", new Vector3(285.24445, -349.28256, 43.871384)),
            //new Tuple<string, Vector3>("Лодки", new Vector3(-853.62213, -1327.3962, 0.9336101)),

            // Newbie respawn
            //new Tuple<string, Vector3>("Авто", new Vector3(-989.4564, -2696.0186, 12.71069)),
            //new Tuple<string, Vector3>("Скутеры", new Vector3(-1029.6763, -2672.7966, 12.7107525)),


            //new Tuple<string, Vector3>("Авто", new Vector3(2493.2422, 4115.2075, 37.17832)),
            //new Tuple<string, Vector3>("Скутеры", new Vector3(-269.83328, 6059.1196, 30.344475)),
        };

        [ServerEvent(Event.ResourceStart)]
        public void onResourceStart()
        {
            try
            {
                foreach (var v in RentAreasTypes)
                {
                    var blipIcon = 0;
                    switch (v.Item1)
                    {
                        case "Авто":
                            blipIcon = 225;
                            break;
                        case "Лодки":
                            blipIcon = 404;
                            break;
                        case "Скутеры":
                            blipIcon = 661;
                            break;
                    }

                    NAPI.Blip.CreateBlip(blipIcon, v.Item2, 0.8f, 1, Main.StringToU16($"Аренда транспорта ({v.Item1})"), 255, 0, true, 0, 0);
                }
            }
            catch (Exception e) { Log.Write("ResourceStart: " + e.StackTrace, nLog.Type.Error); }
        }

        public static Dictionary<string, int> RentMotoList = new Dictionary<string, int>()
        {
            {"Hakuchou", 1000 }
        };

        public static Vector3 GetNearestRentArea(Vector3 position)
        {
            Vector3 nearesetArea = RentAreasTypes[0].Item2;

            foreach (var v in RentAreasTypes)
            {
                if (position.DistanceTo(v.Item2) > position.DistanceTo(nearesetArea)) continue;
                nearesetArea = v.Item2;
            }

            // Проверяем в новой системе Аренды
            foreach(var area in RentcarSystem.RentAreasTypes)
            {
                if (position.DistanceTo(area.Item2) > position.DistanceTo(nearesetArea)) continue;
                nearesetArea = area.Item2;
            }

            return nearesetArea;
        }

        public static void rentCarsSpawner()
        {
            //var random = new Random();
            var i = 0;
            foreach (var c in CarInfos)
            {
                //var veh = NAPI.Vehicle.CreateVehicle(c.Model, c.Position, c.Rotation, random.Next(0, 130), random.Next(0, 130));
                var veh = NAPI.Vehicle.CreateVehicle(c.Model, c.Position, c.Rotation, c.Color1, c.Color2);
                NAPI.Data.SetEntityData(veh, "ACCESS", "RENT");
                NAPI.Data.SetEntityData(veh, "NUMBER", i);
                NAPI.Data.SetEntityData(veh, "DRIVER", null);
                Core.VehicleStreaming.SetEngineState(veh, false);
                Core.VehicleStreaming.SetLockStatus(veh, false);
                i++;
            }
        }

        //[Command("testcar")]

        //public static void RespawnCar(Vehicle vehicle)
        //{

        //    var number = vehicle.GetData<int>("NUMBER");

        //    var random = new Random();
        //    NAPI.Entity.SetEntityPosition(vehicle, CarInfos[number].Position);
        //    NAPI.Entity.SetEntityRotation(vehicle, CarInfos[number].Rotation);
        //    Log.Write("VEHICLE HEALTH BEFORE: " + vehicle.Health, nLog.Type.Error);
        //    VehicleManager.RepairCar(vehicle);
        //    Log.Write("VEHICLE HEALTH: " + vehicle.Health, nLog.Type.Error);


        //    NAPI.Data.SetEntityData(vehicle, "ACCESS", "RENT");
        //    NAPI.Data.SetEntityData(vehicle, "NUMBER", number);
        //    NAPI.Data.SetEntityData(vehicle, "DRIVER", null);
        //    NAPI.Data.SetEntitySharedData(vehicle, "PETROL", 50);
        //    Core.VehicleStreaming.SetEngineState(vehicle, false);
        //    Core.VehicleStreaming.SetLockStatus(vehicle, false);
        //}

        public static void RespawnCar(Vehicle vehicle)
        {
            try
            {
                if (vehicle == null) return;
                var number = vehicle.GetData<int>("NUMBER");

                NAPI.Task.Run(() => {
                    if (vehicle == null) return;
                    NAPI.Entity.DeleteEntity(vehicle);
                    
                    var random = new Random();
                    var veh = NAPI.Vehicle.CreateVehicle(CarInfos[number].Model, CarInfos[number].Position, CarInfos[number].Rotation, CarInfos[number].Color1, CarInfos[number].Color2, numberPlate:"Rent");
                    NAPI.Data.SetEntityData(veh, "ACCESS", "RENT");
                    NAPI.Data.SetEntityData(veh, "NUMBER", number);
                    NAPI.Data.SetEntityData(veh, "DRIVER", null);
                    NAPI.Data.SetEntitySharedData(vehicle, "PETROL", 50);
                    Core.VehicleStreaming.SetEngineState(veh, false);
                    Core.VehicleStreaming.SetLockStatus(veh, false);
                });
                
            } catch(Exception e) { Log.Write("Respawn Car: " + e.StackTrace, nLog.Type.Error); }
        }



        [ServerEvent(Event.PlayerEnterVehicle)]
        public void Event_OnPlayerEnterVehicle(Player player, Vehicle vehicle, sbyte seatid)
        {

            try
            {
                if (!vehicle.HasData("ACCESS") || vehicle.GetData<string>("ACCESS") != "RENT" || seatid != 0) return;///seatid != -1
                if (vehicle.GetData<Player>("DRIVER") != null && vehicle.GetData<Player>("DRIVER") != player)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Этот транспорт уже арендован", 3000);
                    VehicleManager.WarpPlayerOutOfVehicle(player);
                    return;
                }

                int number = vehicle.GetData<int>("NUMBER");
                if (vehicle.GetData<Player>("DRIVER") == null)
                {
                    /*if (player.HasMyData("RENTED_CAR"))
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "У Вас уже оплачена аренда другого транспорта!", 3000);
                        VehicleManager.WarpPlayerOutOfVehicle(player);
                        return;
                    }*/

                    //if ((CarInfos[number].Model == VehicleHash.Faggio || CarInfos[number].Model == VehicleHash.Faggio2 || CarInfos[number].Model == VehicleHash.Faggio3) && Main.Players[player].LVL >= 2)
                    //{
                    //    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Этот транспорт предназначен только для новичков!", 3000);
                    //    VehicleManager.WarpPlayerOutOfVehicle(player);
                    //    return;
                    //}

                    //TODO ADD FOR MOTO CHECK LICENSES 0
                    //if (vehicle.Class && !Main.Players[player].Licenses[3])
                    //{
                    //    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "У вас нет лицензии на водный транспорт для заключения договора аренды!", 3000);
                    //    VehicleManager.WarpPlayerOutOfVehicle(player);
                    //    return;
                    //}

                    if (CarInfos[number].Model == VehicleHash.Hakuchou && !Main.Players[player].Licenses[0])
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "У вас нет лицензии на мото-транспорт для заключения договора аренды!", 3000);
                        VehicleManager.WarpPlayerOutOfVehicle(player);
                        return;
                    }

                    if (CarInfos[number].Model == VehicleHash.Suntrap && !Main.Players[player].Licenses[3])
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "У вас нет лицензии на водный транспорт для заключения договора аренды!", 3000);
                        VehicleManager.WarpPlayerOutOfVehicle(player);
                        return;
                    }

                    int price = CarInfos[number].Price;
                    switch(Main.Players[player].VipLvl) {
                        case 0:
                            price = CarInfos[number].Price;
                            break;
                        case 1:
                            price = Convert.ToInt32(CarInfos[number].Price * 0.95);
                            break;
                        case 2:
                            price = Convert.ToInt32(CarInfos[number].Price * 0.9);
                            break;
                        case 3:
                            price = Convert.ToInt32(CarInfos[number].Price * 0.85);
                            break;
                        case 4:
                            price = Convert.ToInt32(CarInfos[number].Price * 0.8);
                            break;
                        default:
                            price = CarInfos[number].Price;
                            break;
                    }
                    Trigger.ClientEvent(player, "popup::open", "RENT_CAR", $"Вы хотите арендовать этот транспорт за ${price}?");
                }
                else
                {
                    player.SetMyData("IN_RENT_CAR", true);
                }
            }
            catch (Exception e) { Log.Write("PlayerEnterVehicle: " + e.StackTrace, nLog.Type.Error); }
        }

        [ServerEvent(Event.PlayerExitVehicle)]
        public void Event_OnPlayerExitVehicle(Player player, Vehicle vehicle)
        {
            try
            {
                if (!vehicle.HasData("ACCESS") || vehicle.GetData<string>("ACCESS") != "RENT" || vehicle.GetData<Player>("DRIVER") != player) return;
                //Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, $"Через 3 минуты аренда транспорта закончится, если вы снова не сядете в т/с.", 3000);
                //player.SetMyData("IN_RENT_CAR", false);
                //player.SetMyData("RENT_EXIT_TIMER_COUNT", 0);
                //player.SetMyData("RENT_CAR_EXIT_TIMER", Timers.Start(1000, () => timer_playerExitRentVehicle(player, vehicle)));

                player.SetMyData("IN_RENT_CAR", false);
                //player.SetMyData("RENT_EXIT_TIMER_COUNT", 0);
                //player.SetMyData("RENT_CAR_EXIT_TIMER", Timers.Start(1000, () => timer_playerExitRentVehicle(player, vehicle)));
            }
            catch (Exception e) { Log.Write("PlayerExitVehicle: " + e.StackTrace, nLog.Type.Error); }
        }

        /*private void timer_playerExitRentVehicle(Player player, Vehicle vehicle)
        {
            NAPI.Task.Run(() =>
            {
                try
                {
                    if (!player.HasMyData("RENT_CAR_EXIT_TIMER")) return;
                    if (player.GetMyData<bool>("IN_RENT_CAR"))
                    {
                        Timers.Stop(player.GetMyData<string>("RENT_CAR_EXIT_TIMER"));
                        player.ResetMyData("RENT_CAR_EXIT_TIMER");
                        return;
                    }
                    if (player.GetMyData<int>("RENT_EXIT_TIMER_COUNT") > 180)
                    {
                        Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Срок аренды закончился. Транспорт был отбуксирован на место стоянки.", 3000);
                        RespawnCar(vehicle);
                        player.ResetMyData("RENTED_CAR");
                        Timers.Stop(player.GetMyData<string>("RENT_CAR_EXIT_TIMER"));
                        player.ResetMyData("RENT_CAR_EXIT_TIMER");
                        return;
                    }
                    //player.SetMyData("RENT_EXIT_TIMER_COUNT", player.GetMyData<int>("RENT_EXIT_TIMER_COUNT") + 1);
                    player.SetMyData("RENT_EXIT_TIMER_COUNT", player.GetMyData<int>("RENT_EXIT_TIMER_COUNT") + 1);
                }
                catch (Exception e) { Log.Write("timerExitRentVehicle: " + e.StackTrace, nLog.Type.Error); }
            });
        }*/

        public static void Event_OnPlayerDisconnected(Player player)
        {
            try
            {
                if (!Main.Players.ContainsKey(player)) return;

                if (player.HasMyData("RENTED_CAR"))
                    RespawnCar(player.GetMyData<Vehicle>("RENTED_CAR"));
                if (player.HasMyData("RENT_CAR_EXIT_TIMER"))
                    Timers.Stop(player.GetMyData<string>("RENT_CAR_EXIT_TIMER"));
            }
            catch (Exception e) { Log.Write("PlayerDisconnected: " + e.StackTrace, nLog.Type.Error); }
        }


        public static void RentCar(Player player)
        {
            if (!player.IsInVehicle || !player.Vehicle.HasData("ACCESS") || player.Vehicle.GetData<string>("ACCESS") != "RENT" || player.Vehicle.GetData<Player>("DRIVER") != null)
            {
                VehicleManager.WarpPlayerOutOfVehicle(player);
                return;
            }

            if (player.HasMyData("RENTED_CAR"))
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "У вас уже есть арендованный транспорт", 3000);
                return;
            }

            int price = CarInfos[player.Vehicle.GetData<int>("NUMBER")].Price;
            switch(Main.Players[player].VipLvl) {
                case 0:
                    price = CarInfos[player.Vehicle.GetData<int>("NUMBER")].Price;
                    break;
                case 1:
                    price = Convert.ToInt32(CarInfos[player.Vehicle.GetData<int>("NUMBER")].Price * 0.95);
                    break;
                case 2:
                    price = Convert.ToInt32(CarInfos[player.Vehicle.GetData<int>("NUMBER")].Price * 0.9);
                    break;
                case 3:
                    price = Convert.ToInt32(CarInfos[player.Vehicle.GetData<int>("NUMBER")].Price * 0.85);
                    break;
                case 4:
                    price = Convert.ToInt32(CarInfos[player.Vehicle.GetData<int>("NUMBER")].Price * 0.8);
                    break;
                default:
                    price = CarInfos[player.Vehicle.GetData<int>("NUMBER")].Price;
                    break;
            }
            if (Main.Players[player].Money < price)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Недостаточно средств на аренду", 3000);
                VehicleManager.WarpPlayerOutOfVehicle(player);
                return;
            }
            player.Vehicle.SetData("DRIVER", player);
            player.SetMyData("RENTED_CAR", player.Vehicle);
            player.SetMyData("IN_RENT_CAR", true);

            MoneySystem.Wallet.Change(player, -price);
            GameLog.Money($"player({Main.Players[player].UUID})", $"server", price, $"rentCar");

            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Вы успешно арендовали транспорт (аренда транспорта закончится, если вы снова не сядете в т/с в течении 3 мин.)", 3000);
        }
    }
}
