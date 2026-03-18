using GTANetworkAPI;
using System;
using NeptuneEvo.GUI;
using NeptuneEvo.Houses;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using NeptuneEvo.SDK;
using NeptuneEvo.Families;
using NeptuneEvo.Jobs;
using client.Systems;
using client.Fractions.Utils;
using client.Fractions.Government;
using client.Fractions.Gangs;
using client.GUI;
using NeptuneEvo.Fractions;
using client.Phone;
using client.Jobs.Builder;
using NeptuneEvo.MoneySystem;
using System.Drawing.Printing;
using client.Systems.BattlePass;

namespace NeptuneEvo.Core
{
    class Selecting : Script
    {
        private static nLog Log = new nLog("Selecting");

        [RemoteEvent("oSelected")]
        public static void objectSelected(Player player, GTANetworkAPI.Object entity)
        {
            try
            {
                // var entity = (GTANetworkAPI.Object)arguments[0]; // error "Object referance not set to an instance of an object"
                if (entity == null || player == null || !Main.Players.ContainsKey(player)) return;
                //  if (entity.GetSharedData<bool>("PICKEDT") == true)
                if (player.HasMyData("PICKEDT") && player.GetMyData<bool>("PICKEDT") == true)
                {
                    Commands.SendToAdmins(3, $"!{{#d35400}}[PICKUP-ITEMS-EXPLOIT] {player.Name} ({player.Value}) ");
                    return;
                }
                entity.SetSharedData("PICKEDT", true);
                var objType = entity.GetSharedData<string>("TYPE");
                switch (objType)
                {
                    case "DROPPED":
                        {
                            if (player.HasMyData("RodInHand") && player.GetMyData<bool>("RodInHand"))//поднятие сбивает анимку рыбалки
                                return; 
                            if (player.HasMyData("isRemoveObject"))
                            {
                                NAPI.Task.Run(() => {
                                    try
                                    {
                                        NAPI.Entity.DeleteEntity(entity);
                                    }
                                    catch (Exception e){ Log.Write(e.StackTrace, nLog.Type.Error); }
                                });
                                player.ResetMyData("isRemoveObject");
                                return;
                            }

                            var id = entity.GetData<int>("ID");
                            if (Items.InProcessering.Contains(id))
                            {
                                entity.SetSharedData("PICKEDT", false);
                                return;
                            }
                            Items.InProcessering.Add(id);

                            nItem item = NAPI.Data.GetEntityData(entity, "ITEM");
                            if (item.Type == ItemType.BodyArmor && nInventory.Find(Main.Players[player].UUID, ItemType.BodyArmor) != null)
                            {
                                entity.SetSharedData("PICKEDT", false);
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Недостаточно места в инвентаре", 3000); // ?
                                Items.InProcessering.Remove(id);
                                return;
                            }

                            var tryAdd = nInventory.TryAdd(player, item);

                            if (tryAdd == -1)
                            {
                                entity.SetSharedData("PICKEDT", false);
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Недостаточно места в инвентаре", 3000);
                                Items.InProcessering.Remove(id);
                                return;
                            }
                            //else if (tryAdd > 0)
                            //{
                            //    entity.SetSharedData("PICKEDT", false);
                            //    nInventory.Add(player, new nItem(item.Type, item.Count - tryAdd, item.Data));
                            //    GameLog.Items($"ground", $"player({Main.Players[player].UUID})", Convert.ToInt32(item.Type), item.Count - tryAdd, $"{item.Data}");
                            //    item.Count = tryAdd;
                            //    entity.SetData("ITEM", item);
                            //    Items.InProcessering.Remove(id);
                            //}
                            else
                            {
                                NAPI.Task.Run(() => { try { NAPI.Entity.DeleteEntity(entity); } catch (Exception e){ Log.Write(e.StackTrace, nLog.Type.Error); } });
                                nInventory.Add(player, item);
                                GameLog.Items($"ground", $"player({Main.Players[player].UUID})", Convert.ToInt32(item.Type), item.Count, $"{item.Data}");
                            }

                            Main.OnAntiAnim(player);
                            player.PlayAnimation("random@domestic", "pickup_low", 39);
                            NAPI.Task.Run(() => { try { player.StopAnimation(); Main.OffAntiAnim(player); } catch (Exception e){ Log.Write(e.StackTrace, nLog.Type.Error); } }, 1700);
                            return;
                        }
                    case "WeaponSafe":
                    case "SubjectSafe":
                    case "ClothesSafe":
                    case "Shelter":
                        {
                            entity.SetSharedData("PICKEDT", false);
                            if (Main.Players[player].InsideHouseID == -1) return;
                            int houseID = Main.Players[player].InsideHouseID;
                            House house = HouseManager.Houses.FirstOrDefault(h => h.ID == Main.Players[player].InsideHouseID);
                            if (house == null) return;
                            if (house.Owner != player.Name)
                            {
                                if (!house.Roommates.Contains(player.Name))
                                {
                                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Пользоваться мебелью может только владелец дома.", 3000);
                                    return;
                                }
                            }

                            int furnID = NAPI.Data.GetEntityData(entity, "ID");
                            HouseFurniture furniture = FurnitureManager.HouseFurnitures[houseID][furnID];
                            var items = FurnitureManager.FurnituresItems[houseID][furnID];
                            if (items == null) return;

                            player.SetMyData("OpennedSafe", furnID);
                            player.SetMyData("SHELTER_TYPE", furniture.ShelterType);
                            player.SetMyData("OPENOUT_TYPE", FurnitureManager.SafesType[furniture.Name]);
                            Dashboard.OpenOut(player, items, furniture.Name, FurnitureManager.SafesType[furniture.Name]);
                            return;
                        }
                    case "familyWeaponSafe":
                    case "familySubjectSafe":
                    case "familyClothesSafe":
                    case "familyShelter":
                        {
                            entity.SetSharedData("PICKEDT", false);
                            int houseID = NAPI.Data.GetEntityData(entity, "FAMILYHOUSE");
                            FamilyHouse house = FamilyHouseManager.FamilyHouse.FirstOrDefault(h => h.ID == houseID);
                            if (house == null) return;
                            if (!house.Owner.Equals(Family.GetFamilyName(player)) && Main.Players[player].FamilyRank < 8)
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Пользоваться мебелью может только владелец дома.", 3000);
                                return;
                            }
                            int furnID = NAPI.Data.GetEntityData(entity, "ID");
                            FamilyHouseFurniture furniture = FamilyFurnitureManager.FamilyHouseFurnitures[houseID][furnID];
                            var items = FamilyFurnitureManager.FamilyFurnituresItems[houseID][furnID];
                            if (items == null) return;
                            player.SetMyData("OpennedSafehouseID", houseID);
                            player.SetMyData("OpennedSafe", furnID);
                            player.SetMyData("OPENOUT_TYPE", FamilyFurnitureManager.SafesType[furniture.Name]);
                            player.SetMyData("SHELTER_TYPE", furniture.ShelterType);
                            Dashboard.OpenOut(player, items, furniture.Name, FamilyFurnitureManager.SafesType[furniture.Name]);
                            return;
                        }
                    case "MoneyBag":
                        {
                            if (player.HasMyData("HEIST_DRILL") || player.HasMyData("HAND_MONEY"))
                            {
                                entity.SetSharedData("PICKEDT", false);
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У Вас уже есть сумка", 3000);
                                return;
                            }

                            var money = NAPI.Data.GetEntityData(entity, "MONEY_IN_BAG");

                            player.SetClothes(5, 45, 0);
                            var item = new nItem(ItemType.BagWithMoney, 1, $"{money}");
                            nInventory.Items[Main.Players[player].UUID].Add(item);
                            Dashboard.sendItems(player);
                            player.SetMyData("HAND_MONEY", true);
                            NAPI.Task.Run(() => { try { NAPI.Entity.DeleteEntity(entity); } catch (Exception e){ Log.Write(e.StackTrace, nLog.Type.Error); } });
                            Main.OnAntiAnim(player);
                            player.PlayAnimation("random@domestic", "pickup_low", 39);
                            NAPI.Task.Run(() => { try { player.StopAnimation(); Main.OffAntiAnim(player); } catch (Exception e){ Log.Write(e.StackTrace, nLog.Type.Error); } }, 1700);
                            return;
                        }
                    case "DrillBag":
                        {
                            if (player.HasMyData("HEIST_DRILL") || player.HasMyData("HAND_MONEY"))
                            {
                                entity.SetSharedData("PICKEDT", false);
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У Вас уже есть дрель или деньги в руках", 3000);
                                return;
                            }

                            player.SetClothes(5, 41, 0);
                            nInventory.Add(player, new nItem(ItemType.BagWithDrill));
                            player.SetMyData("HEIST_DRILL", true);

                            NAPI.Task.Run(() => { try { NAPI.Entity.DeleteEntity(entity); } catch (Exception e){ Log.Write(e.StackTrace, nLog.Type.Error); } });
                            Main.OnAntiAnim(player);
                            player.PlayAnimation("random@domestic", "pickup_low", 39);
                            NAPI.Task.Run(() => { try { player.StopAnimation(); Main.OffAntiAnim(player); } catch (Exception e){ Log.Write(e.StackTrace, nLog.Type.Error); } }, 1700);
                            return;
                        }
                }
            }
            catch (Exception e) { Log.Write($"oSelected/: {e.ToString()}\n{e.StackTrace}", nLog.Type.Error); }
        }

        [RemoteEvent("vehicleSelected")]
        public static void vehicleSelected(Player player, params object[] arguments)
        {
            try
            {
                Vehicle vehicle = (Vehicle)arguments[0];
                string index = Convert.ToString(arguments[1]);
                if (vehicle == null || player.Position.DistanceTo(vehicle.Position) > 5)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Машина находится далеко от Вас", 3000);
                    return;
                }
                switch (index)
                {
                    case "hood":
                        if (player.IsInVehicle)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы не можете открыть/закрыть капот, находясь в машине", 3000);
                            return;
                        }
                        if (VehicleStreaming.GetDoorState(vehicle, DoorID.DoorHood) == DoorState.DoorClosed)
                        {
                            if (VehicleStreaming.GetLockState(vehicle))
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы не можете открыть капот, пока машина закрыта", 3000);
                                return;
                            }
                            VehicleStreaming.SetDoorState(vehicle, DoorID.DoorHood, DoorState.DoorOpen);
                        }
                        else VehicleStreaming.SetDoorState(vehicle, DoorID.DoorHood, DoorState.DoorClosed);
                        return;
                    case "carinvopen":
                        if (player.IsInVehicle)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы не можете открыть/закрыть багажник, находясь в машине", 3000);
                            return;
                        }
                        if (VehicleStreaming.GetDoorState(vehicle, DoorID.DoorTrunk) == DoorState.DoorOpen)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Бажник уже открыт!", 3000);
                            return;
                        }
                        else
                        {
                            if (vehicle.HasData("ACCESS") && (vehicle.GetData<string>("ACCESS") == "PERSONAL" || vehicle.GetData<string>("ACCESS") == "GARAGE"))
                            {
                                var access = VehicleManager.canAccessByNumber(player, vehicle.GetData<int>("ID"));
                                if (!access)
                                {
                                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У Вас нет ключей от этого транспорта", 3000);
                                    return;
                                }
                            }
                            if (vehicle.HasData("ACCESS") && vehicle.GetData<string>("ACCESS") == "FRACTION")
                            {
                                /*if (Main.Players[player].Fraction.FractionID != 8 && Main.Players[player].Fraction.FractionID != 14)
                                {
                                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы не можете открыть багажник у этой машины", 3000);
                                    return;
                                }*/

                                if (player.IsInVehicle)
                                {
                                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы не можете открыть/закрыть багажник, находясь в машине", 3000);
                                    return;
                                }
                                if (VehicleStreaming.GetDoorState(vehicle, DoorID.DoorTrunk) == DoorState.DoorClosed)
                                {

                                    if (vehicle.HasData("ACCESS") && (vehicle.GetData<string>("ACCESS") == "GARAGE"))
                                    {
                                        var access = VehicleManager.HasAccess(player, vehicle);
                                        if (!access)
                                        {
                                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У Вас нет ключей от этого транспорта", 3000);
                                            return;
                                        }
                                    }
                                    if (vehicle.HasData("ACCESS") && vehicle.GetData<string>("ACCESS") == "FRACTION")
                                    {
                                        if (vehicle.DisplayName == "Barracks")
                                        {
                                            if (Main.Players[player].Fraction.FractionID == 0)
                                            {
                                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы не можете открыть багажник у этой машины", 3000);
                                                return;
                                            }
                                        }
                                        else if (Main.Players[player].Fraction.FractionID != vehicle.GetData<int>("FRACTION") && (Main.Players[player].Fraction.FractionID != 7 && Main.Players[player].Fraction.FractionID != 9))
                                        {
                                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы не можете открыть багажник у этой машины", 3000);
                                            return;
                                        }
                                    }
                                    if (VehicleManager.TrunkCoords.ContainsKey((VehicleHash)vehicle.Model))
                                    {
                                        if (vehicle.EngineStatus)
                                        {
                                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Двигатель должен быть заглушен.", 3000);
                                            return;
                                        }
                                        Vector3 truckPos = VehicleManager.GetTrunkCoords(vehicle);
                                        var mark = NAPI.Marker.CreateMarker(0, truckPos, new Vector3(), new Vector3(), 0.8f, new Color(250, 255, 0, 170));
                                        TextLabel label;
                                        if ((VehicleHash)vehicle.Model == VehicleHash.Ambulance)
                                        {

                                            if (!vehicle.HasData("MEDICAMENTS"))
                                                vehicle.SetData("MEDICAMENTS", 0);

                                            label = NAPI.TextLabel.CreateTextLabel($"~w~Медикаменты\n\n~g~{vehicle.GetData<int>("MEDICAMENTS")}~w~ / {Stocks.maxMats[(VehicleHash)vehicle.Model]}", truckPos + new Vector3(0, 0, 0.2), 5f, 1f, 0, new Color(255, 255, 255));
                                        }
                                        else
                                        {
                                            if (!vehicle.HasData("MATERIALS"))
                                                vehicle.SetData("MATERIALS", 0);
                                            label = NAPI.TextLabel.CreateTextLabel($"~w~Материалы\n\n~g~{vehicle.GetData<int>("MATERIALS")}~w~ / {Stocks.maxMats[(VehicleHash)vehicle.Model]}", truckPos + new Vector3(0, 0, 0.2), 5f, 1f, 0, new Color(255, 255, 255));
                                        }

                                        var shape = NAPI.ColShape.CreateCylinderColShape(truckPos, 1, 3);
                                        shape.SetData("VEH", vehicle);
                                        shape.OnEntityEnterColShape += (shp, Player) =>
                                        {
                                            Player.SetMyData("INTERACTIONCHECK", 651);
                                            Player.SetMyData("TRUNK_VEHICLE", shp.GetData<Vehicle>("VEH"));
                                        };
                                        shape.OnEntityExitColShape += (shp, Player) =>
                                        {
                                            Player.SetMyData("INTERACTIONCHECK", 0);
                                            Player.ResetMyData("TRUNK_VEHICLE");
                                        };

                                        vehicle.SetData("TRUNK_MARKER", mark);
                                        vehicle.SetData("TRUNK_SHAPE", shape);
                                        vehicle.SetData("TRUNK_LABEL", label);
                                    }
                                    VehicleStreaming.SetDoorState(vehicle, DoorID.DoorTrunk, DoorState.DoorOpen);
                                    //Commands.RPChat("me", player, $"открыл(а) багажник");
                                }
                                else
                                {
                                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Багажник уже открыт", 3000);
                                    return;
                                }
                                return;
                            }
                            if (vehicle.HasData("ACCESS") && vehicle.GetData<string>("ACCESS") == "FAMILY" && vehicle.GetData<string>("FAMILY") != Main.Players[player].FamilyCID)
                            {
                                if (Main.Players[player].Fraction.FractionID != 7 && Main.Players[player].Fraction.FractionID != 9)
                                {
                                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы не можете открыть багажник у этой машины", 3000);
                                    return;
                                }
                            }
                            if (VehicleStreaming.GetLockState(vehicle))
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы не можете открыть багажник, пока машина закрыта", 3000);
                                return;
                            }
                            VehicleStreaming.SetDoorState(vehicle, DoorID.DoorTrunk, DoorState.DoorOpen);
                            Commands.RPChat("me", player, $"открыл(а) багажник");
                        }
                        return;
                    case "carinvclose":
                        if (VehicleStreaming.GetDoorState(vehicle, DoorID.DoorTrunk) == DoorState.DoorOpen)
                        {
                            Commands.RPChat("me", player, $"закрыл(а) багажник");
                            if (VehicleManager.TrunkCoords.ContainsKey((VehicleHash)vehicle.Model) && vehicle.HasData("TRUNK_MARKER"))
                            {
                                var marker = vehicle.GetData<Marker>("TRUNK_MARKER");
                                var shape = vehicle.GetData<ColShape>("TRUNK_SHAPE");
                                var label = vehicle.GetData<TextLabel>("TRUNK_LABEL");
                                marker.Delete();
                                shape.Delete();
                                label.Delete();
                                vehicle.ResetData("TRUNK_MARKER");
                                vehicle.ResetData("TRUNK_SHAPE");
                                vehicle.ResetData("TRUNK_LABEL");
                            }
                            VehicleStreaming.SetDoorState(vehicle, DoorID.DoorTrunk, DoorState.DoorClosed);
                            foreach (var p in Main.Players.Keys.ToList())
                            {
                                if (p == null || !Main.Players.ContainsKey(p)) continue;
                                if (p.HasMyData("OPENOUT_TYPE") && p.GetMyData<int>("OPENOUT_TYPE") == 2 && p.HasMyData("SELECTEDVEH") && p.GetMyData<Vehicle>("SELECTEDVEH") == vehicle) GUI.Dashboard.Close(p);
                            }
                        }
                        return;
                    case "doorsOpen":
                        if (!VehicleManager.GetVehicleDoorsStatus(vehicle))
                            VehicleManager.ChangeVehicleDoors(player, vehicle);
                        else
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Двери уже открыты", 3000);
                        return;
                    case "doorsClose":
                        if (VehicleManager.GetVehicleDoorsStatus(vehicle))
                            VehicleManager.ChangeVehicleDoors(player, vehicle);
                        else
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Двери уже закрыты", 3000);
                        return;
                    case "carinvshow":
                        if (player.IsInVehicle)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы не можете открыть инвентарь, находясь в машине", 3000);
                            return;
                        }
                        if (NAPI.Data.GetEntityData(vehicle, "ACCESS") == "WORK" || vehicle.Class == 13 || vehicle.Class == 8)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Эта транспортное средство не поддерживает инвентарь", 3000);
                            return;
                        }
                        if (vehicle.HasData("ACCESS") && vehicle.GetData<string>("ACCESS") == "PERSONAL")
                        {
                          if (vehicle.HasData("OWNER") && vehicle.GetData<Player>("OWNER") == player) { }
                          else
                          {
                            if (Main.Players[player].AdminLVL == 0 && VehicleStreaming.GetDoorState(vehicle, DoorID.DoorTrunk) == DoorState.DoorClosed)
                            {
                              Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы не можете открыть инвентарь машины, пока багажник закрыт", 3000);
                              return;
                            }
                          }
                        }
                        else if (vehicle.HasData("ACCESS") && vehicle.GetData<string>("ACCESS") == "FAMILY")
                        {
                          if (vehicle.HasData("FAMILY") && vehicle.GetData<string>("FAMILY") == Main.Players[player].FamilyCID) { }
                          else
                          {
                            if (Main.Players[player].AdminLVL == 0 && VehicleStreaming.GetDoorState(vehicle, DoorID.DoorTrunk) == DoorState.DoorClosed)
                            {
                              Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы не можете открыть инвентарь машины, пока багажник закрыт", 3000);
                              return;
                            }
                          }
                        }
                        else
                        {
                          if (Main.Players[player].AdminLVL == 0 && VehicleStreaming.GetDoorState(vehicle, DoorID.DoorTrunk) == DoorState.DoorClosed)
                          {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы не можете открыть инвентарь машины, пока багажник закрыт", 3000);
                            return;
                          }
                        }
                        if (vehicle.GetData<bool>("BAGINUSE") == true)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Дождитесь, пока другой человек перестанет пользоваться багажником.", 3000);
                            return;
                        }
                        //if (player.HasMyData("RobberyItem"))
                        //{
                        //    player.StopAnimation();
                        //    Main.OffAntiAnim(player);
                        //    BasicSync.DetachObject(player);
                        //    player.ResetMyData("RobberyItem");
                        //    House house = HouseManager.Houses.FirstOrDefault(h => h.ID == player.GetMyData<int>("ROBBERYHOUSE"));
                        //    if (house == null) return;
                        //    if (house.RobberyItems > 0)
                        //    {
                        //        house.RobberyItems -= 1;
                        //        MoneySystem.Bank.Accounts[house.BankID].Balance -= 487;
                        //        VehicleInventory.Add(vehicle, new nItem(ItemType.Subject, 1, house.ID));
                        //    }
                        //    if (house.RobberyItems == 0)
                        //    {

                        //        player.ResetMyData("ROBBERYHOUSE");
                        //        house.RobberyHouse = DateTime.Now.AddDays(1);
                        //        return;
                        //    }
                        //}
                        vehicle.SetData("BAGINUSE", true);
                        player.SetMyData("OPENOUT_TYPE", 2);
                        player.SetMyData("SELECTEDVEH", vehicle);
                        GUI.Dashboard.OpenOut(player, vehicle.GetData<List<nItem>>("ITEMS"), "Багажник", 2);

                        return;
                    //case 4: //вытащить задержаного
                    //    Fractions.FractionCommands.playerOutCar(player);
                    //    return;
                    case "getbox":
                        Truckers.GetBoxFromCar(player, vehicle);
                        return;
                    case "fill":
                        Jobs.AutoMechanic.OpenMechanicModal(player, ModalType.Refill, vehicle);
                        return;
                    case "repair":
                        Jobs.AutoMechanic.OpenMechanicModal(player, ModalType.Repair, vehicle);
                        return;
                    case "Продать машину в гос":
                    case "sellcargov":
                        if (!vehicle.HasData("ID")) return;

                        int vehId = vehicle.GetData<int>("ID");

                        if (VehicleManager.Vehicles[vehId].OwnerID != Main.Players[player].UUID)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы можете продать только свою машину", 3000);
                            return;
                        }

                        if (vehicle.GetData<string>("ACCESS") == "RESALE")
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Автомобиль находится в ResaleCar", 3000);
                            return;
                        }

                        var vData = VehicleManager.Vehicles[vehId];

                        player.SetMyData("CARSELLGOV", vehId);
                        int price = 0;
                        if (BusinessManager.ProductsOrderPrice.ContainsKey(vData.Model))
                        {
                            switch (Main.Players[player].VipLvl)
                            {
                                case 0: // None
                                    price = Convert.ToInt32(BusinessManager.ProductsOrderPrice[vData.Model] * 0.5);
                                    break;
                                case 1: // Bronze
                                    price = Convert.ToInt32(BusinessManager.ProductsOrderPrice[vData.Model] * 0.6);
                                    break;
                                case 2: // Silver
                                    price = Convert.ToInt32(BusinessManager.ProductsOrderPrice[vData.Model] * 0.6);
                                    break;
                                case 3: // Gold
                                    price = Convert.ToInt32(BusinessManager.ProductsOrderPrice[vData.Model] * 0.7);
                                    break;
                                case 4: // Platinum
                                    price = Convert.ToInt32(BusinessManager.ProductsOrderPrice[vData.Model] * 0.8);
                                    break;
                                default:
                                    price = Convert.ToInt32(BusinessManager.ProductsOrderPrice[vData.Model] * 0.5);
                                    break;
                            }
                        }

                        Trigger.ClientEvent(player, "popup::open", "CAR_SELL_TOGOV", $"Вы действительно хотите продать {vData.Model} с номером ({vehId}) за ${price}?");
                        return;
                    case "tossup":
                        if (!Fractions.Manager.canUseAccess(player, Fractions.Manager.FractionAccess.incar)) return;

                        if (!player.HasMyData("FOLLOWER"))
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"За вами никто не следует", 3000);
                            return;
                        }
                        Player target = player.GetMyData<Player>("FOLLOWER");


                        int seat = -1;
                        if (vehicle.MaxPassengers < 2)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Машина слишком мала", 3000);
                            return;
                        }

                        if (!vehicle.HasData("OCCUPANTS"))
                        {
                            List<Player> occupantsList = new List<Player>();
                            vehicle.SetData("OCCUPANTS", occupantsList);
                        }

                        List<Player> occupants = vehicle.GetData<List<Player>>("OCCUPANTS");
                        for (int i = vehicle.MaxPassengers; i > 0; i--)
                        {
                            Log.Debug($"tossup for {i} => {occupants.FindAll((p) => p.VehicleSeat == i).Count}");
                            //if (vehicle.Occupants.FindAll((p) => ((Player)p).VehicleSeat == i).Count == 0)
                            if (occupants.FindAll((p) => p.VehicleSeat == i).Count == 0)
                            {
                                seat = i;
                                break;
                            }
                        }
                        if (seat == -1)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"В машине нету места", 3000);
                            return;
                        }
                        Fractions.FractionCommands.unFollow(player, target);
                        NAPI.Player.SetPlayerIntoVehicle(target, vehicle, seat);
                        Log.Debug($"SetIntoVehicle seat = {seat}");
                        NAPI.Task.Run(() => {
                            NAPI.Player.SetPlayerIntoVehicle(target, vehicle, seat);
                        });

                        if (!vehicle.HasData("OCCUPANTS"))
                        {
                            List<Player> occupantsList = new List<Player>();
                            occupantsList.Add(player);
                            vehicle.SetData("OCCUPANTS", occupantsList);
                        }
                        else
                        {
                            if (!vehicle.GetData<List<Player>>("OCCUPANTS").Contains(player)) vehicle.GetData<List<Player>>("OCCUPANTS").Add(player);
                        }

                        Commands.RPChat("me", player, $"посадил в машину {target.Value}");
                        break;
                    case "takefromcar": // Вытащить из машины
                        if (!Fractions.Manager.canUseAccess(player, Fractions.Manager.FractionAccess.pull)) return;
                        //List<Entity> passengers = vehicle.Occupants;
                        if (!vehicle.HasData("OCCUPANTS"))
                        {
                            List<Player> occupantsList = new List<Player>();
                            vehicle.SetData("OCCUPANTS", occupantsList);
                        }
                        List<Player> passengers = vehicle.GetData<List<Player>>("OCCUPANTS");
                        Player targetInVeh = null;
                        foreach (Player p in passengers)
                        {
                            if (p.HasCuff())
                            {
                                targetInVeh = p;
                                break;
                            }
                        }
                        if (targetInVeh == null)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"В машине нету никого в наручниках", 3000);
                            return;
                        }
                        VehicleManager.WarpPlayerOutOfVehicle(targetInVeh);
                        //targetInVeh.WarpOutOfVehicle();
                        targetInVeh.Position = player.Position.Around(1f);

                        //BasicSync.AddAttachmnet(targetInVeh, "policeCuffs", true);
                        Main.PlayAnimation(targetInVeh, Main.Anim.InCuff);

                        if (!vehicle.HasData("OCCUPANTS"))
                        {
                            List<Player> occupantsList = new List<Player>();
                            vehicle.SetData("OCCUPANTS", occupantsList);
                        }
                        else
                        {
                            if (vehicle.GetData<List<Player>>("OCCUPANTS").Contains(player)) vehicle.GetData<List<Player>>("OCCUPANTS").Remove(player);
                        }

                        Commands.RPChat("me", player, $"вытащил из машины {targetInVeh.Value}");
                        return;
                    case "puttrunk":
                        if (!Fractions.Manager.GovIds.ContainsKey(Main.Players[player].Fraction.FractionID) && Main.Players[player].Fraction.FractionID != 0)
                        {
                            if (!Fractions.Manager.canUseAccess(player, Fractions.Manager.FractionAccess.intrunk))
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Нет доступа", 3000);
                                return;
                            }
                        }
                        else if (Main.Players[player].Fraction.FractionID == 0)
                        {
                            if (Families.Manager.isHaveFamily(player) && !Families.Manager.isHaveAccess(player, Families.Manager.FamilyAccess.Kidnap))
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Нет доступа", 3000);
                                return;
                            }
                        }

                        if (!player.HasMyData("FOLLOWER"))
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"За вами никто не следует", 3000);
                            return;
                        }

                        if (VehicleStreaming.GetDoorState(vehicle, DoorID.DoorTrunk) == DoorState.DoorClosed)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Багажник закрыт", 3000);
                            return;
                        }

                        if (vehicle.HasData("IN_TRUNK"))
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"В багажнике нету места", 3000);
                            return;
                        }

                        Player targetInTrunk = player.GetMyData<Player>("FOLLOWER");
                        Fractions.FractionCommands.unFollow(player, targetInTrunk);
                        VehicleManager.PutTrunkPlayer(targetInTrunk, vehicle);
                        vehicle.SetData("IN_TRUNK", targetInTrunk);
                        break;
                    case "outtrunk":
                        // if (!Fractions.Manager.canUseAccess(player, Fractions.Manager.FractionAccess.outtrunk)) return;
                        if (!Fractions.Manager.GovIds.ContainsKey(Main.Players[player].Fraction.FractionID) && Main.Players[player].Fraction.FractionID != 0)
                        {
                            if (!Fractions.Manager.canUseAccess(player, Fractions.Manager.FractionAccess.outtrunk))
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Нет доступа", 3000);
                                return;
                            }
                        }
                        else if(Main.Players[player].Fraction.FractionID == 0)
                        {
                            if (Families.Manager.isHaveFamily(player) && !Families.Manager.isHaveAccess(player, Families.Manager.FamilyAccess.Kidnap))
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Нет доступа", 3000);
                                return;
                            }
                        }

                        if (VehicleStreaming.GetDoorState(vehicle, DoorID.DoorTrunk) == DoorState.DoorClosed)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Багажник закрыт", 3000);
                            return;
                        }

                        if (!vehicle.HasData("IN_TRUNK"))
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "В багажнике никого нету", 3000);
                            return;
                        }

                        VehicleManager.OutTrunkPlayer(vehicle);
                        vehicle.ResetData("IN_TRUNK");
                        break;
                    case "intotrunk":
                        if (VehicleStreaming.GetDoorState(vehicle, DoorID.DoorTrunk) == DoorState.DoorClosed)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Багажник закрыт", 3000);
                            return;
                        }
                        if (vehicle.HasData("IN_TRUNK"))
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"В багажнике нету места", 3000);
                            return;
                        }
                        VehicleManager.PutTrunkPlayer(player, vehicle);
                        vehicle.SetData("IN_TRUNK", player);
                        break;
                    case "outoftrunk":
                        if (VehicleStreaming.GetDoorState(vehicle, DoorID.DoorTrunk) == DoorState.DoorClosed)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Багажник закрыт", 3000);
                            return;
                        }
                        if (!vehicle.HasData("IN_TRUNK"))
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "В багажнике никого нету", 3000);
                            return;
                        }
                        if (vehicle.GetData<Player>("IN_TRUNK") != player)
                        {
                            return;
                        }

                        VehicleManager.OutTrunkPlayer(vehicle);
                        vehicle.ResetData("IN_TRUNK");
                        break;
                    case "putnumber":
                        VehicleManager.PutNumber(player, vehicle);
                        break;
                    case "takenumber":
                        VehicleManager.TakeNumber(player, vehicle);
                        break;
                    case "hackcar":
                        if (vehicle.HasData("CARTHEFT"))
                            CarTheftManager.HackCar(player, vehicle);
                        break;
                    case "evacuation":
                        if (vehicle.HasData("ACCESS"))
                        {
                            if(vehicle.GetData<string>("ACCESS") == "FAMILY")
                            {
                                string fcid = vehicle.GetData<string>("FAMILY");

                                if (Main.Players[player].FamilyCID == fcid)
                                {
                                    if(!Families.Manager.isHaveAccess(player, Families.Manager.FamilyAccess.ReturnVehicles))
                                    {
                                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Нет прав", 3000);
                                        return;
                                    }

                                    FamilyHouseManager.SendVehicleInFamilyGarage(player, vehicle);
                                }
                            }
                            else if(vehicle.GetData<string>("ACCESS") == "PERSONAL")
                            {
                                if(vehicle.GetData<Player>("OWNER") == player)
                                    Phone.GetCar(player, vehicle.GetData<int>("ID"));

                            }
                        }
                        break;
                }
            }
            catch (Exception e) { Log.Write("vSelected: " + e.StackTrace, nLog.Type.Error); }
        }

        [RemoteEvent("selfSelected")]
        public static void SelfSelected(Player player, string action)
        {
            try
            {
                switch (action)
                {
                    case "docs":
                        Main.ShowPassport(player, player);
                        break;
                    case "licenses":
                        Main.ShowLicenses(player, player);
                        break;
                    case "showmedcard":
                        Main.ShowMedCard(player, player);
                        break;
                    case "showmedcardA":
                        Main.ShowMedCard(player, player, 1);
                        break;
                    case "fractionDocs":
                        Main.ShowFractionDoc(player, player);
                        break;
                    case "builderLic":
                        Main.ShowQualific(player, player);
                        break;
                    case "famstock":
                        Families.Manager.StockControl(player);
                        break;
                    case "TechnicalPassport":

                        List<VehicleManager.VehicleData> vehicles = new List<VehicleManager.VehicleData>();
                        var vehs = VehicleManager.getAllPlayerVehicles(player.Name);

                        foreach (int id in vehs)
                        {
                            if(VehicleManager.Vehicles[id].Passport != null)
                                if (VehicleManager.Vehicles[id].Passport.DateFrom != DateTime.MinValue) vehicles.Add(VehicleManager.Vehicles[id]);
                        }

                        if (vehicles.Count == 0)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "У вас нету зарегистрированых транспортных средств");
                            return;
                        }
                        else if (vehicles.Count == 1)
                        {
                            ShowTechnicalPassport(player, player, vehicles[0]);
                        }
                        else
                        {
                            List<ListSystem.Item> items = new List<ListSystem.Item>();
                            foreach (var veh in vehicles)
                                items.Add(new ListSystem.Item($"{VehicleManager.GetVehicleRealName(veh.Model)} {veh.Number}", veh));
                            ListSystem.Open(player, "Тех. паспорт", new ListSystem.List((veh) => ShowTechnicalPassport(player, player, (VehicleManager.VehicleData)veh), items));
                        }
                        break;
                    default:
                        Log.Write($"SelfSelected action {action}", nLog.Type.Warn);
                        break;
                }
            }
            catch (Exception e) { Log.Write($"SelfSelected: " + e.ToString() + "\n" + e.StackTrace, nLog.Type.Error); }
        }

        [RemoteEvent("pSelected")]
        public static void playerSelected(Player player, params object[] arguments)
        {
            try
            {
                var target = (Player)arguments[0];
                if (target == null || player.Position.DistanceTo(target.Position) > 2)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Игрок находится далеко от Вас", 3000);
                    return;
                }
                player.SetMyData("SELECTEDPLAYER", target);

                if (arguments.Length == 1) return;
                var action = arguments[1].ToString();
                switch (action)
                {
                    case "Аптечка":
                    case "medkit":
                        Ems.Healing(player, target);
                        return;
                    case "Перевязать":
                    case "bandage":
                        Ems.Dressing(player, target);
                        return;
                    case "Пригласить":
                    case "invite":
                        Fractions.FractionCommands.InviteToFraction(player, target);
                        return;
                    case "100$":
                        PlayerTransferMoneyToTarget(player, target, 100);
                        return;
                    case "500$":
                        PlayerTransferMoneyToTarget(player, target, 500);
                        return;
                    case "1000$":
                        PlayerTransferMoneyToTarget(player, target, 1000);
                        return;
                    case "Показать документы на авто":
                    case "showCarDocsPl":
                        Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, "To do", 3000);
                        return;
                    case "Предоставить паспорт":
                    case "giveDocs":
                        playerShowDocuments(player, target);
                        return;
                    case "Удостоверение фракции":
                    case "fractionDocs":
                        playerShowFractionDocuments(player, target);
                        return;
                    case "Предоставить лицензии":
                    case "licenses":
                        playerShowLicenses(player, target);
                        return;
                    case "showmedcard":
                    case "Предоставить мед.карту":
                        playerShowMedCard(player, target, 0);
                        return;
                    case "showmedcardA":
                    case "Предоставить мед.карту А":
                        playerShowMedCard(player, target, 1);
                        return;
                    case "Поздороваться":
                    case "sayhi":
                        if (player.IsInVehicle) return;
                        playerHandshakeTarget(player, target);
                        return;
                    case "Конвой"://
                    case "convoy":
                        if (player.IsInVehicle) return;
                        Fractions.FractionCommands.targetFollowPlayer(player, target);
                        return;
                    case "Ограбить":
                    case "rob":
                        if (player.IsInVehicle) return;
                        Fractions.FractionCommands.robberyTarget(player, target);
                        return;
                    case "Ограбить семья":
                    case "robfam":
                        if (player.IsInVehicle) return;
                        Fractions.FractionCommands.robberyTargetFamily(player, target);
                        return;
                    case "Развязать":
                    case "tie":
                        if (player.IsInVehicle) return;
                        Fractions.FractionCommands.playerPressCuffBut(player);
                        return;
                    case "Наручники":
                    case "handcuffs":
                        if (player.IsInVehicle) return;
                        Fractions.FractionCommands.playerPressCuffBut(player);
                        return;
                    case "Продать бизнес":
                    case "sellbiz":
                        if (player.IsInVehicle) return;
                        if (Main.Players[player].BizIDs.Count == 0)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У Вас нет бизнеса", 3000);
                            return;
                        }

                        Business biz = BusinessManager.BizList[Main.Players[player].BizIDs[0]];

                        if (biz == null)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У Вас нет бизнеса", 3000);
                            return;
                        }

                        Trigger.PlayerEvent(player, "popup::openInput", "Продать бизнес", "Цена $$$", 8, "client_offerbizsell");
                        return;
                    case "tossup":
                    case "Запихнуть в авто"://
                        Fractions.FractionCommands.playerInCar(player, target);
                        return;
                    case "Посмотреть документы при обыске"://
                        if (target.GetCuff() == Fractions.Cuff.None)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок не в наручниках", 3000);
                            return;
                        }
                        Main.ShowPassport(target, player);
                        Notify.Send(target, NotifyType.Info, NotifyPosition.BottomCenter, $"{Main.Players[player].FirstName}_{Main.Players[player].LastName} посмотрел ваши документы", 3000);
                        return;
                    case "Посмотреть лицензии":
                        if (target.GetCuff() == Fractions.Cuff.None)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок не в наручниках", 3000);
                            return;
                        }

                        Main.ShowLicenses(target, player);
                        Notify.Send(target, NotifyType.Info, NotifyPosition.BottomCenter, $"{Main.Players[player].FirstName}_{Main.Players[player].LastName} посмотрел ваши лицензии", 3000);
                        return;
                    case "unconvoy":
                    case "Отпустить":
                        if (player.IsInVehicle) return;
                        if (!target.HasMyData("FOLLOWING"))
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Этого игрока никто не тащит", 3000);
                            return;
                        }
                        if (!player.HasMyData("FOLLOWER") || player.GetMyData<Player>("FOLLOWER") != target)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Этого игрока тащит кто-то другой", 3000);
                            return;
                        }
                        Fractions.FractionCommands.unFollow(player, target);
                        return;
                    case "search":
                    case "Обыскать":
                        if (player.IsInVehicle) return;
                        {
                            //if (!target.GetMyData<bool>("CUFFED"))
                            //{
                            //    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок не в наручниках", 3000);
                            //    return;
                            //}

                            var items = nInventory.Items[Main.Players[target].UUID];
                            List<string> itemNames = new List<string>();
                            List<string> weapons = new List<string>();
                            List<string> illegalItems = new List<string>();
                            foreach (var i in items)
                            {
                                if (nInventory.ClothesItems.Contains(i.Type)) continue;
                                if (nInventory.WeaponsItems.Contains(i.Type))
                                    weapons.Add($"{nInventory.ItemsNames[(int)i.Type]} {i.Data}");
                                if (nInventory.IllegalItems.Contains(i.Type))
                                    illegalItems.Add($"{nInventory.ItemsNames[(int)i.Type]} {i.Data}");
                                else
                                    itemNames.Add($"{nInventory.ItemsNames[(int)i.Type]} x{i.Count}");
                            }

                            var data = new SearchObject();
                            data.Name = target.Name.Replace('_', ' ');
                            data.Weapons = weapons;
                            data.Items = itemNames;
                            data.Illegal = illegalItems;

                            #region BPКвест: 99 Обыскать 5 человек

                            #region BattlePass выполнение квеста
                            BattlePass.updateBPQuestIteration(player, BattlePass.BPQuestType.SearchIllegal);
                            #endregion

                            #endregion

                            Trigger.ClientEvent(player, "newPassport", target, Main.Players[target].UUID);
                            Trigger.ClientEvent(player, "bsearchOpen", JsonConvert.SerializeObject(data));
                            return;
                        }
                    case "Изъять оружие"://
                    case "takegun":
                        //if (player.IsInVehicle) return;
                        //playerTakeGuns(player, target);
                        //return;
                    case "takeillegal"://
                    case "Изъять нелегал":
                        if (player.IsInVehicle) return;
                        playerTakeIlleagal(player, target);
                        return;
                    case "Продать аптечку":
                        Trigger.ClientEvent(player, "popup::openInput", "Продать аптечку", "Цена $$$", 4, "player_medkit");
                        return;
                    case "Предложить лечение":
                    case "offerheal":
                        if (player.IsInVehicle) return;
                        Trigger.ClientEvent(player, "popup::openInput", "Предложить лечение", "Цена $$$", 4, "player_heal");
                        return;
                    case "Вылечить":
                    case "pheal":
                        if (player.IsInVehicle) return;
                        playerHealTarget(player, target);
                        return;
                    case "invitebrig":
                    case "Пригласить в бригаду":
                        client.Jobs.Builder.BuilderManager.InviteToBrigade(player, target);
                        return;
                    case "invitefam":
                    case "Пригласить в семью":
                        Families.Manager.InvitePlayerToFamily(player, target.Id);
                        return;
                    case "Продать машину":
                    case "sellcar":
                        VehicleManager.sellCar(player, target);
                        return;
                    case "Продать дом":
                    case "sellhouse":
                        House house = HouseManager.GetHouse(player, true);
                        if (house == null)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У Вас нет дома", 3000);
                            return;
                        }
                        Trigger.ClientEvent(player, "popup::openInput", "Продать дом", "Цена $$$", 8, "player_offerhousesell");
                        return;
                    case "roommate":
                    case "Заселить в дом":
                        HouseManager.InviteToRoom(player, target);
                        return;
                    case "Пригласить в дом":
                    case "invitehouse":
                        HouseManager.InvitePlayerToHouse(player, target);
                        return;
                    case "Передать деньги":
                        if (Main.Players[player].LVL < 1)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Перевод денег доступен после первого уровня", 3000);
                            return;
                        }
                        Trigger.ClientEvent(player, "popup::openInput", "Передать деньги", "Сумма $$$", 4, "player_givemoney");
                        return;
                    case "Предложить обмен":
                    case "offerexchange":
                        target.SetMyData("OFFER_MAKER", player);
                        target.SetMyData("REQUEST", "OFFER_ITEMS");
                        target.SetMyData("IS_REQUESTED", true);
                        Notify.Send(target, NotifyType.Warning, NotifyPosition.BottomCenter, $"Игрок ({player.Value}) предложил Вам обменяться предметами. Y/N - принять/отклонить", 3000);
                        Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы предложили игроку ({target.Value}) обменяться предметами.", 3000);
                        return;
                    case "Сорвать маску":
                    case "takemask":
                        if (player.IsInVehicle) return;
                        if (!Fractions.Manager.canUseAccess(player, Fractions.Manager.FractionAccess.pickofmask)) return;
                        Fractions.FractionCommands.playerTakeoffMask(player, target);
                        return;
                    case "Выписать штраф":
                    case "ticket":
                    case "fine":
                        if (player.IsInVehicle) return;
                        player.SetMyData("TICKETTARGET", target);
                        Trigger.ClientEvent(player, "popup::openInput", "Выписать штраф (сумма)", $"Сумма от 0 до {FractionCommands.MaxTicketVal}", 5, "player_ticketsum");
                        return;
                    case "Посадить в машину":
                        Fractions.FractionCommands.playerInCar(player, target);
                        return;
                    case "Посадить в КПЗ":
                    case "jail":
                        if (!player.HasMyData("PoliceCell"))
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы должны находиться около входа в камеру", 3000);
                            return;
                        }
                        DoorPolice door = player.GetMyData<DoorPolice>("PoliceCell");

                        FractionCommands.StartArrest(player, new Cell(door.Position, door.CamSpawnPos));
                        return;
                    case "inviteteam":
                    case "Пригласить в команду":
                        if (player.IsInVehicle) return;
                        GarbageTruck.AddPlayerToTeam(player, target.Value);
                        return;
                    case "setcont":
                    case "Назначить на контракт":
                        List < KeyValuePair<int, OrderContract> > list = BuilderManager.ServerContracts.Where((KeyValuePair<int, OrderContract> c) => c.Value.InvestorUUID == Main.Players[player].UUID && c.Value.ProrabUUID == -1).ToList();
                        if(list.Count == 0)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "У вас нет контрактов");
                            return;
                        }
                        else if (list.Count == 1)
                        {
                            BuilderManager.SetInvestorProrabContract(player, target, list[0].Key);
                        }
                        else
                        {
                            List<ListSystem.Item> items = new List<ListSystem.Item>();
                            foreach (var cont in list)
                                items.Add(new ListSystem.Item($"{cont.Value.Title}", cont.Value.ID));
                            ListSystem.Open(player, "Контракты", new ListSystem.List((cont) => BuilderManager.SetInvestorProrabContract(player, target, (int)cont), items));
                        }
                        return;
                    case "reanimate":
                    case "Реанимированить":
                        ReanimTarget(player, target);
                        break;
                    case "Выдать военный билет":
                    case "millitaryticket":
                        Army.GiveMillitaryTicket(player, target);
                        return;
                    case "Надеть/снять мешок":
                    case "pocket":
                        if (player.Position.DistanceTo(target.Position) > 2)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок слишком далеко", 3000);
                            return;
                        }
                        Fractions.FractionCommands.playerChangePocket(player, target);
                        return;
                    case "glicgun":
                    case "Выдать лицензию на оружие":
                        DialogInput.Open(player, "Введите стоимость лицензии(5000-50000$)", "$", 6, "5000", (int licPrice) => {
                            if (licPrice < 5000 || licPrice > 50000)
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Цена должна быть в диапазоне от 5000$ до 50000$");
                                return;
                            }
                            DialogQuery.OpenDialog(target, "GiveLicense9", $"Вам предложили купить лицензию {LicNames[6]} за {licPrice}", () => { GiveLic(player, target, 6, licPrice); });
                        });
                        break;
                    case "glicbiz":
                    case "Выдать лицензию на бизнес":
                        GiveLic(player, target, 8);
                        break;
                    case "glicrod":
                    case "Выдать лицензию на рыбалку":
                        int price = 15000;
                        DialogQuery.OpenDialog(target, "GiveLicense9", $"Вам предложили купить лицензию {LicNames[9]} за {price}", () => { GiveLic(player, target, 9, price); } );
                        break;
                    case "glicdrod":
                    case "Выдать лицензию на глубоководную рыбалку":
                        int price1 = 70000;
                        DialogQuery.OpenDialog(target, "GiveLicense9", $"Вам предложили купить лицензию {LicNames[11]} за {price1}", () => { GiveLic(player, target, 11, price1); });
                        break;
                    case "glicadv":
                    case "Выдать лицензию адвоката":
                        GiveLic(player, target, 12);
                        break;
                    case "tlicgun":
                    case "Забрать лицензию на оружие":
                        TakeLic(player, target, 6);
                        break;
                    case "tlicbiz":
                    case "Забрать лицензию на бизнес":
                        TakeLic(player, target, 8);
                        break;
                    case "tlicrob":
                    case "Забрать лицензию на рыбалку":
                        TakeLic(player, target, 9);
                        break;
                    case "tlicadv":
                    case "Забрать лицензию адвоката":
                        TakeLic(player, target, 12);
                        break;
                    case "tlicdrod":
                    case "Забрать лицензию на глубоководную рыбалку":
                        TakeLic(player, target, 11);
                        break;
                    case "bHandcuffs":
                    case "Взлом наручников":
                        Gangs.BreakCuff(player, target);
                        break;
                    case "Выдать мед карту":
                    case "sellmed":
                        if (!Fractions.Manager.canUseAccess(player, Fractions.Manager.FractionAccess.medcard)) return;
                        Ems.SellMedCard(player, target, 0);
                        return;
                    case "Выдать мед карту A":
                    case "sellmedA":
                        if (!Fractions.Manager.canUseAccess(player, Fractions.Manager.FractionAccess.medcard_A)) return;
                        Ems.SellMedCard(player, target, 1);
                        return;
                    case "releaseprision":
                    case "Выпусть из тюрьмы":
                        ReseasePrision(player, target);
                        break;
                    case "Квалификация строителя":
                    case "builderLic":
                        Main.ShowQualific(player, target);
                        break;
                    case "TechnicalPassport":

                        List<VehicleManager.VehicleData> vehicles = new List<VehicleManager.VehicleData>();
                        var vehs = VehicleManager.getAllPlayerVehicles(player.Name);

                        foreach(int id in vehs)
                        {
                            if (VehicleManager.Vehicles[id].Passport.DateFrom != DateTime.MinValue) vehicles.Add(VehicleManager.Vehicles[id]);
                        }

                        if (vehicles.Count == 0)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "У вас нету зарегистрированых транспортных средств");
                            return;
                        }
                        else if (vehicles.Count == 1)
                        {
                            ShowTechnicalPassport(player, target, vehicles[0]);
                        }
                        else
                        {
                            List<ListSystem.Item> items = new List<ListSystem.Item>();
                            foreach (var veh in vehicles)
                                items.Add(new ListSystem.Item($"{VehicleManager.GetVehicleRealName(veh.Model)} {veh.Number}", veh));
                            ListSystem.Open(player, "Тех. паспорт", new ListSystem.List((veh) => ShowTechnicalPassport(player, target, (VehicleManager.VehicleData)veh), items));
                        }
                        break;
                }
            }
            catch (Exception e) { Log.Write($"pSelected: " + e.ToString(), nLog.Type.Error); }
        }

        public static void playerShowDocuments(Player player, Player target)
        {
            if (player.HasCuff() || (player.HasSharedData("InDeath") && player.GetSharedData<bool>("InDeath") == true))
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Невозможно показать документы игроку в данный момент", 3000);
                return;
            }

            if (target.HasCuff() || (target.HasSharedData("InDeath") && target.GetSharedData<bool>("InDeath") == true))
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Невозможно показать документы игроку в данный момент", 3000);
                return;
            }

            target.SetMyData("REQUESTER", player);
            target.SetMyData("REQUEST", "playerShowDocuments");
            target.SetMyData("IS_REQUESTED", true);

            Trigger.ClientEvent(target, "showTargetInteraction", $"Игрок ({player.Value}) хочет показать вам паспорт.");
            //DialogQuery.OpenDialog(target, "DOCUMENTS", $"Игрок ({player.Value}) хочет показать вам паспорт.", () => Selecting.showDocumentsTarget(player, target));

            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы предложили игроку ({target.Value}) показать документы.", 3000);
        }
        public static void showDocumentsTarget(Player player)
        {
            Player target = null;
            if (player.HasMyData("REQUESTER")) target = player.GetMyData<Player>("REQUESTER");

            if (target == null || !Main.Players.ContainsKey(target) || !Main.Players.ContainsKey(player)) return;
            if (target.HasCuff() || (target.HasSharedData("InDeath") && target.GetSharedData<bool>("InDeath") == true))
            {
                return;
            }
            if (player.HasCuff() || (player.HasSharedData("InDeath") && player.GetSharedData<bool>("InDeath") == true))
            {
                return;
            }
            Main.ShowPassport(target, player);
            return;
        }
        public static void playerShowFractionDocuments(Player player, Player target)
        {
            if (player.HasCuff() || (player.HasSharedData("InDeath") && player.GetSharedData<bool>("InDeath") == true))
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Невозможно показать удостоверение игроку в данный момент", 3000);
                return;
            }

            if (target.HasCuff() || (target.HasSharedData("InDeath") && target.GetSharedData<bool>("InDeath") == true))
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Невозможно показать удостоверение игроку в данный момент", 3000);
                return;
            }

            if (!new List<int>() { 6, 7, 8, 9, 14, 15, 17, 18 }.Contains(Main.Players[player].Fraction.FractionID))
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "У вас нету удостовериния", 3000);
                return;
            }

            target.SetMyData("REQUESTER", player);
            target.SetMyData("REQUEST", "playerShowFractionDocuments");
            target.SetMyData("IS_REQUESTED", true);

            Trigger.ClientEvent(target, "showTargetInteraction", $"Игрок ({player.Value}) хочет показать вам удостоверение.");

            //DialogQuery.OpenDialog(target, "FRACTION_DOCUMENTS", $"Игрок ({player.Value}) хочет показать вам удостоверение.", () => ShowFractionDocumentsTarget(player, target));

            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы предложили игроку ({target.Value}) показать удостоверение.", 3000);
        }
        public static void ShowFractionDocumentsTarget(Player player)
        {
            Player target = null;
            if (player.HasMyData("REQUESTER")) target = player.GetMyData<Player>("REQUESTER");

            if (!Main.Players.ContainsKey(target) || !Main.Players.ContainsKey(player)) return;
            if (target.HasCuff() || (target.HasSharedData("InDeath") && target.GetSharedData<bool>("InDeath") == true))
            {
                return;
            }
            if (player.HasCuff() || (player.HasSharedData("InDeath") && player.GetSharedData<bool>("InDeath") == true))
            {
                return;
            }
            Main.ShowFractionDoc(target, player);
            return;
        }
        public static void playerShowLicenses(Player player, Player target)
        {
            if (player.HasCuff() || (player.HasSharedData("InDeath") && player.GetSharedData<bool>("InDeath") == true))
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Невозможно показать лицензии игроку в данный момент", 3000);
                return;
            }

            if (target.HasCuff() || (target.HasSharedData("InDeath") && target.GetSharedData<bool>("InDeath") == true))
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Невозможно показать лицензии игроку в данный момент", 3000);
                return;
            }

            target.SetMyData("REQUESTER", player);
            target.SetMyData("REQUEST", "playerShowLicenses");
            target.SetMyData("IS_REQUESTED", true);

            Trigger.ClientEvent(target, "showTargetInteraction", $"Игрок ({player.Value}) хочет показать вам лицензии.");

            //DialogQuery.OpenDialog(target, "LICENSES", $"Игрок ({player.Value}) хочет показать вам лицензии.", () => Selecting.ShowLicensesTarget(player, target));

            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы предложили игроку ({target.Value}) показать лицензии.", 3000);
        }
        public static void ShowLicensesTarget(Player player)
        {
            Player target = null;
            if (player.HasMyData("REQUESTER")) target = player.GetMyData<Player>("REQUESTER");
            if (target == null && player.HasMyData("REQUESTER_SHOW_LICENSE")) target = player.GetMyData<Player>("REQUESTER_SHOW_LICENSE");

            if (!Main.Players.ContainsKey(target) || !Main.Players.ContainsKey(player)) return;
            if (target.HasCuff() || (target.HasSharedData("InDeath") && target.GetSharedData<bool>("InDeath") == true))
            {
                return;
            }

            if (player.HasCuff() || (player.HasSharedData("InDeath") && player.GetSharedData<bool>("InDeath") == true))
            {
                return;
            }

            Main.ShowLicenses(target, player);
            return;
        }

        public static void playerShowMedCard(Player player, Player target, int medCard) // 0 - def | 1 - A
        {
            if (Main.Players[player].MedCards[medCard].ToDate == DateTime.MinValue)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У вас нет мед.карты{(medCard == 1 ? " категории А" : "")}", 3000);
                return;
            }
            if (player.HasCuff() || (player.HasSharedData("InDeath") && player.GetSharedData<bool>("InDeath") == true))
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Невозможно показать мед.карту{(medCard == 1 ? " категории А" : "")} игроку в данный момент", 3000);
                return;
            }
            if (target.HasCuff() || (target.HasSharedData("InDeath") && target.GetSharedData<bool>("InDeath") == true))
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Невозможно показать мед.карту{(medCard == 1 ? " категории А" : "")} игроку в данный момент", 3000);
                return;
            }

            target.SetMyData("REQUESTER", player);
            target.SetMyData("REQUESTER_MEDTYPE", medCard);
            target.SetMyData("REQUEST", "playerShowMedCard");
            target.SetMyData("IS_REQUESTED", true);

            Trigger.ClientEvent(target, "showTargetInteraction", $"Вы предложили игроку ({target.Value}) показать мед.карту{(medCard == 1 ? " категории А" : "")}");
            //DialogQuery.OpenDialog(target, "MEDCARD", $"Игрок ({player.Value}) хочет показать вам мед.карту{(medCard == 1 ? " категории А" : "")}", () => Selecting.ShowMedCardTarget(player, target, medCard));

            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы предложили игроку ({target.Value}) показать мед.карту{(medCard == 1 ? " категории А" : "")}", 3000);

        }
        public static void ShowMedCardTarget(Player player)
        {
            Player target = null;
            if (player.HasMyData("REQUESTER")) target = player.GetMyData<Player>("REQUESTER");

            if (!Main.Players.ContainsKey(target) || !Main.Players.ContainsKey(player)) return;
            if (target.HasCuff() || (target.HasSharedData("InDeath") && target.GetSharedData<bool>("InDeath") == true))
            {
                return;
            }
            if (player.HasCuff() || (player.HasSharedData("InDeath") && player.GetSharedData<bool>("InDeath") == true))
            {
                return;
            }

            int medCard = 0;
            if (player.HasMyData("REQUESTER_MEDTYPE")) medCard = player.GetMyData<int>("REQUESTER_MEDTYPE");
            Main.ShowMedCard(target, player, medCard);
            return;
        }

        public static void ShowTechnicalPassport(Player player, Player target, VehicleManager.VehicleData vehicle)
        {
            if (!Main.Players.ContainsKey(target) || !Main.Players.ContainsKey(player)) return;
            if (target.HasCuff() || (target.HasSharedData("InDeath") && target.GetSharedData<bool>("InDeath") == true))
            {
                return;
            }
            if (player.HasCuff() || (player.HasSharedData("InDeath") && player.GetSharedData<bool>("InDeath") == true))
            {
                return;
            }
            if (vehicle.Passport.DateFrom == DateTime.MinValue)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Нету тех. паспорта");
                return;
            }

            Main.ShowTechnicalPassport(target, vehicle);
        }


        public static Dictionary<int, string> LicNames = new Dictionary<int, string>()
        {
            { 6, "на оружие" },
            { 8, "на бизнес" },
            { 9, "на ловлю рыбы" },
            { 11, "на ловлю глубоководной рыбы" },
            { 12, "адвоката" },
        };

        public static Dictionary<int, Fractions.Manager.FractionAccess> givelicaccess = new Dictionary<int, Fractions.Manager.FractionAccess>()
        {
            { 6, Fractions.Manager.FractionAccess.givegunlic },
            { 8, Fractions.Manager.FractionAccess.givebizlic },
            { 9, Fractions.Manager.FractionAccess.givefishlic },
            { 11, Fractions.Manager.FractionAccess.givefishlic },
            { 12, Fractions.Manager.FractionAccess.giveadvlic },
        };

        public static Dictionary<int, Fractions.Manager.FractionAccess> takelicaccess = new Dictionary<int, Fractions.Manager.FractionAccess>()
        {
            { 6, Fractions.Manager.FractionAccess.takegunlic },
            { 8, Fractions.Manager.FractionAccess.takebizlic },
            { 9, Fractions.Manager.FractionAccess.takefishlic },
            { 11, Fractions.Manager.FractionAccess.takefishlic},
            { 12, Fractions.Manager.FractionAccess.takeadvlic },
        };

        public static void ReseasePrision(Player player, Player target)
        {
            if (!Fractions.Manager.canUseAccess(player, Fractions.Manager.FractionAccess.releaseprision))
                return;

            if (!Main.Players[target].Arrest.InPrision())
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Игрок не в тюрьме", 3000);
                return;
            }

            Main.Players[target].Arrest.Free();
            Trigger.PlayerEvent(target, "CLIENT::Timer:stopTimer");
            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Вы выпустили из тюрьмы игрока", 3000);
            Notify.Send(target, NotifyType.Info, NotifyPosition.BottomCenter, "Вас выпустили из тюрьмы", 3000);
        }

        public static void GiveLic(Player player, Player target, int lic)
        {
            if (!Fractions.Manager.canUseAccess(player, givelicaccess[lic]))
                return;

            if (player.Position.DistanceTo(target.Position) > 2)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок слишком далеко от Вас", 3000);
                return;
            }

            if (Main.Players[target].Licenses[lic])
            {
                Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, $"У игрока уже есть лицензия {LicNames[lic]}", 3000);
                return;
            }

            Main.Players[target].Licenses[lic] = true;
            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы выдали игроку ({target.Value}) лицензию {LicNames[lic]}", 3000);
            Notify.Send(target, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы получили лицензию {LicNames[lic]}", 3000);
        }
        public static void GiveLic(Player player, Player target, int lic, int price)
        {
            try
            {
                if (!Fractions.Manager.canUseAccess(player, givelicaccess[lic]))
                    return;

                if (player.Position.DistanceTo(target.Position) > 2)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок слишком далеко от Вас", 3000);
                    return;
                }

                if (Main.Players[target].Licenses[lic])
                {
                    Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, $"У игрока уже есть лицензия {LicNames[lic]}", 3000);
                    return;
                }
                if (!MoneySystem.Wallet.Change(target, -price))
                {
                    Notify.Send(target, NotifyType.Error, NotifyPosition.BottomCenter, $"Недостаточно средств", 3000);
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У игрока недостаточно средств", 3000);
                    return;
                }

                if (Main.Players[player].Fraction.FractionID == 0)
                {
                    Wallet.Change(player, +price);
                    Main.Players[target].Licenses[lic] = true;

                    #region BPКвест: 63 Приобрести лицензию рыбалки. / 64 Приобрести лицензию глубоководной рыбалки. / 67 Приобрести лицензию на оружие.

                    #region BattlePass выполнение квеста
                    if (lic == 6) BattlePass.updateBPQuestIteration(target, BattlePass.BPQuestType.GetLicenseWeapon);
                    if (lic == 9) BattlePass.updateBPQuestIteration(target, BattlePass.BPQuestType.GetLicenseFish);
                    if (lic == 11) BattlePass.updateBPQuestIteration(target, BattlePass.BPQuestType.GetLicenseDeepFish);
                    #endregion

                    #endregion

                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы выдали игроку ({target.Value}) лицензию {LicNames[lic]}", 3000);
                    Notify.Send(target, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы получили лицензию {LicNames[lic]}", 3000);
                    return;
                }

                int playerIncome = (int)(price * 0.35);
                int fractionIncome = price - playerIncome;

                Wallet.Change(player, playerIncome);
                Stocks.fracStocks[Main.Players[player].Fraction.FractionID].Money += fractionIncome;

                Main.Players[target].Licenses[lic] = true;

                #region BPКвест: 63 Приобрести лицензию рыбалки. / 64 Приобрести лицензию глубоководной рыбалки. / 67 Приобрести лицензию на оружие.

                #region BattlePass выполнение квеста
                if (lic == 6) BattlePass.updateBPQuestIteration(target, BattlePass.BPQuestType.GetLicenseWeapon);
                if (lic == 9) BattlePass.updateBPQuestIteration(target, BattlePass.BPQuestType.GetLicenseFish);
                if (lic == 11) BattlePass.updateBPQuestIteration(target, BattlePass.BPQuestType.GetLicenseDeepFish);
                #endregion

                #endregion

                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы выдали игроку ({target.Value}) лицензию {LicNames[lic]}", 3000);
                Notify.Send(target, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы получили лицензию {LicNames[lic]}", 3000);
            }
            catch (Exception ex) { Console.WriteLine(ex.StackTrace); }
        }

        public static void TakeLic(Player player, Player target, int lic)
        {
            if (!Fractions.Manager.canUseAccess(player, (Fractions.Manager.FractionAccess)takelicaccess[lic]))
                return;

            if (player.Position.DistanceTo(target.Position) > 2)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок слишком далеко от Вас", 3000);
                return;
            }

            if (!Main.Players[target].Licenses[lic])
            {
                Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, $"У игрока нет лицензии {LicNames[lic]}", 3000);
                return;
            }

            Main.Players[target].Licenses[lic] = false;

            #region BPКвест: 183 Забрать лицензию на глубокую рыбалку / 184 Забрать лицензию на рыбалку. / 185 Забрать лицензию на оружие.

            #region BattlePass выполнение квеста
            if (lic == 6) BattlePass.updateBPQuestIteration(player, BattlePass.BPQuestType.RemoveLicenseWeapon);
            if (lic == 9) BattlePass.updateBPQuestIteration(player, BattlePass.BPQuestType.RemoveLicenseFish);
            if (lic == 11) BattlePass.updateBPQuestIteration(player, BattlePass.BPQuestType.RemoveLicenseDeepFish);
            #endregion

            #endregion

            #region BPКвест: 213 Забрать лицензию на оружие у одного человека

            #region BattlePass выполнение квеста
            if (lic == 6) BattlePass.updateBPQuestIteration(player, BattlePass.BPQuestType.LSPDTakeGUNLic);
            #endregion

            #endregion

            #region BPКвест: 261 Забрать Лицензии на оружие троим людям

            #region BattlePass выполнение квеста
            if (lic == 6) BattlePass.updateBPQuestIteration(player, BattlePass.BPQuestType.FIBRemoveGunLic);
            #endregion

            #endregion

            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы забрали у игрока ({target.Value}) лицензию {LicNames[lic]}", 3000);
            Notify.Send(target, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы потеряли лицензию {LicNames[lic]}", 3000);
        }

        public static void PlayerTransferMoneyToTarget(Player player, Player target, int amount)
        {
            if (Main.Players[player].LVL < 2)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Передача денег будет доступна начиная с 2 уровня.", 3000);
                return;
            }

            if (amount < 1)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Введите корректные данные", 3000);
                return;
            }
            if (!Main.Players.ContainsKey(target) || player.Position.DistanceTo(target.Position) > 2)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок слишком далеко от Вас", 3000);
                return;
            }
            if (amount > Main.Players[player].Money)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У Вас недостаточно средств", 3000);
                return;
            }
            Notify.Send(target, NotifyType.Info, NotifyPosition.BottomCenter, $"Игрок ({player.Value}) передал Вам {amount}$", 3000);
            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы передали игроку ({target.Value}) {amount}$", 3000);
            MoneySystem.Wallet.Change(target, amount);
            MoneySystem.Wallet.Change(player, -amount);
            GameLog.Money($"player({Main.Players[player].UUID})", $"player({Main.Players[target].UUID})", amount, $"transfer");
            //Commands.RPChat("me", player, $"передал(а) {amount}$ " + "{name}", target);
        }

        public static void playerTransferMoney(Player player, string arg)
        {
            if (Main.Players[player].LVL < 1)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Передача денег будет доступна начиная с 1 уровня.", 3000);
                return;
            }
            try
            {
                Convert.ToInt32(arg);
            }
            catch
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Введите корректные данные", 3000);
                return;
            }
            var amount = Convert.ToInt32(arg);
            if (amount < 1)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Введите корректные данные", 3000);
                return;
            }
            Player target = player.GetMyData<Player>("SELECTEDPLAYER");
            if (!Main.Players.ContainsKey(target) || player.Position.DistanceTo(target.Position) > 2)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок слишком далеко от Вас", 3000);
                return;
            }
            if (amount > Main.Players[player].Money)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У Вас недостаточно средств", 3000);
                return;
            }
            if (player.HasMyData("NEXT_TRANSFERM") && DateTime.Now < player.GetMyData<DateTime>("NEXT_TRANSFERM") && Main.Players[player].AdminLVL == 0)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "С момента последней передачи денег прошло мало времени.", 3000);
                return;
            }
            player.SetMyData("NEXT_TRANSFERM", DateTime.Now.AddMinutes(1));
            Notify.Send(target, NotifyType.Info, NotifyPosition.BottomCenter, $"Игрок ({player.Value}) передал Вам {amount}$", 3000);
            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы передали игроку ({target.Value}) {amount}$", 3000);
            MoneySystem.Wallet.Change(target, amount);
            MoneySystem.Wallet.Change(player, -amount);
            GameLog.Money($"player({Main.Players[player].UUID})", $"player({Main.Players[target].UUID})", amount, $"transfer");
            Commands.RPChat("me", player, $"передал(а) {amount}$ " + "{name}", target);
        }
        public static void playerHealTarget(Player player, Player target)
        {
            try
            {
                if (player.Position.DistanceTo(target.Position) > 2)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок слишком далеко от Вас", 3000);
                    return;
                }
                var item = nInventory.Find(Main.Players[player].UUID, ItemType.HealthKit);
                if (item == null || item.Count < 1)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У Вас нет аптечки", 3000);
                    return;
                }

                nInventory.Remove(player, ItemType.HealthKit, 1);
                if (target.HasMyData("IS_DYING"))
                {
                    player.PlayAnimation("amb@medic@standing@tendtodead@idle_a", "idle_a", 39);
                    Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы начали реанимирование игрока ({target.Value})", 3000);
                    Notify.Send(target, NotifyType.Info, NotifyPosition.BottomCenter, $"Игрок ({player.Value}) начал реанимировать Вас", 3000);
                    NAPI.Task.Run(() =>
                    {
                        try
                        {
                            player.StopAnimation();
                            NAPI.Entity.SetEntityPosition(player, player.Position + new Vector3(0, 0, 0.5));

                            if (Main.Players[player].Fraction.FractionID != 8)
                            {
                                var random = new Random();
                                if (random.Next(0, 11) <= 5)
                                {
                                    Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"У Вас не вышло реанимировать игрока ({target.Value})", 3000);
                                    return;
                                }
                            }
                            else
                            {
                                if (!target.HasMyData("NEXT_DEATH_MONEY") || DateTime.Now > target.GetMyData<DateTime>("NEXT_DEATH_MONEY"))
                                {
                                    MoneySystem.Wallet.Change(player, 150);
                                    GameLog.Money($"server", $"player({Main.Players[player].UUID})", 150, $"revieve({Main.Players[target].UUID})");
                                    target.SetMyData("NEXT_DEATH_MONEY", DateTime.Now.AddMinutes(15));
                                }
                            }

                            target.StopAnimation();
                            NAPI.Entity.SetEntityPosition(target, target.Position + new Vector3(0, 0, 0.5));
                            target.SetSharedData("InDeath", false);
                            Trigger.ClientEvent(target, "DeathTimer", false);
                            target.Health = 50;
                            target.ResetMyData("IS_DYING");
                            target.ResetSharedData("IS_DYING");
                            Main.Players[target].IsAlive = true;
                            Main.OffAntiAnim(target);
                            if (target.HasMyData("DYING_TIMER"))
                            {
                                //Main.StopT(target.GetMyData<string>("DYING_TIMER"), "timer_18");
                                Timers.Stop(target.GetMyData<string>("DYING_TIMER"));
                                target.ResetMyData("DYING_TIMER");
                            }
                            Notify.Send(target, NotifyType.Info, NotifyPosition.BottomCenter, $"Игрок ({player.Value}) реанимировал Вас", 3000);
                            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы реанимировали игрока ({target.Value})", 3000);

                            #region GBPКвест: 15 Реанимировать 100 человек.

                            #region BattlePass выполнение квеста
                            BattlePass.updateBPGlobalQuestIteration(player, BattlePass.BPGlobalQuestType.RevivePlayers);
                            #endregion

                            #endregion

                            if (target.HasMyData("CALLEMS_BLIP"))
                            {
                                NAPI.Entity.DeleteEntity(target.GetMyData<Blip>("CALLEMS_BLIP"));
                            }
                            if (target.HasMyData("CALLEMS_COL"))
                            {
                                NAPI.ColShape.DeleteColShape(target.GetMyData<ColShape>("CALLEMS_COL"));
                            }
                        }
                        catch (Exception e) { Log.Write("playerHealedtarget: " + e.StackTrace, nLog.Type.Error); }
                    }, 15000);
                }
                else
                {
                    Notify.Send(target, NotifyType.Info, NotifyPosition.BottomCenter, $"Игрок ({player.Value}) вылечил Вас с помощью аптечки", 3000);
                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы вылечили игрока ({target.Value}) с помощью аптечки", 3000);
                    target.Health = 100;
                }
                return;
            }
            catch (Exception e) { Log.Write("playerHealTarget: " + e.StackTrace); }
        }
        //public static void playerTakeGuns(Player player, Player target)
        //{
        //    if (player.Position.DistanceTo(target.Position) > 2)
        //    {
        //        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок слишком далеко от Вас", 3000);
        //        return;
        //    }
        //    if (!Fractions.Manager.canUseAccess(player, Fractions.Manager.FractionAccess.takeguns)) return;
        //    Weapons.RemoveAll(target, true);
        //    Notify.Send(target, NotifyType.Warning, NotifyPosition.BottomCenter, $"Игрок ({player.Value}) изъял у Вас всё оружие", 3000);
        //    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы изъяли всё оружие у игрока ({target.Value})", 3000);
        //    return;
        //}
        public static void playerTakeIlleagal(Player player, Player target)
        {
            if (player.Position.DistanceTo(target.Position) > 2)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок слишком далеко от Вас", 3000);
                return;
            }

            int takeCount = 0;

            foreach (var itemType in nInventory.IllegalItems)
            {
                try
                {
                    nItem illegalItem = nInventory.Find(Main.Players[target].UUID, itemType);
                    if (illegalItem == null)
                        continue;
                    if (Main.Players[target].Licenses[6] == true && (nInventory.WeaponsItems.Contains(illegalItem.Type) || nInventory.AmmoItems.Contains(illegalItem.Type)))
                        continue;

                    nInventory.Remove(target, illegalItem);
                    nInventory.Add(player, illegalItem);
                    takeCount++;
                }
                catch (Exception e) { Log.Write(e.StackTrace); }
            }

            if(takeCount <= 0)
            {
                Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, $"У игрока нету запрещенных вещей.", 3000);
                return;
            }

            Notify.Send(target, NotifyType.Warning, NotifyPosition.BottomCenter, $"Игрок ({player.Value}) изъял у Вас запрещённые предметы", 3000);
            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы изъяили у игрока {target.Value} запрещённые предметы", 3000);
            return;
        }
        public static void playerOfferChangeItems(Player player)
        {
            if (!Main.Players.ContainsKey(player) || !player.HasMyData("OFFER_MAKER") || !Main.Players.ContainsKey(player.GetMyData<Player>("OFFER_MAKER"))) return;
            Player offerMaker = player.GetMyData<Player>("OFFER_MAKER");
            if (Main.Players[player].ArrestTime > 0 || Main.Players[offerMaker].ArrestTime > 0)
            {
                player.ResetMyData("OFFER_MAKER");
                return;
            }
            if (player.Position.DistanceTo(offerMaker.Position) > 2)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок слишком далеко", 3000);
                return;
            }

            player.SetMyData("CHANGE_WITH", offerMaker);
            offerMaker.SetMyData("CHANGE_WITH", player);
            player.SetMyData("CHANGE_WITH_ITEMS", new List<nItem>());
            offerMaker.SetMyData("CHANGE_WITH_ITEMS", new List<nItem>());
            Trigger.ClientEvent(player, "inventory", 9, Main.Players[player].Money);
            Trigger.ClientEvent(offerMaker, "inventory", 9, Main.Players[offerMaker].Money);
            List<bool> tempslots = new List<bool>();
            for (int i = 0; i < 10; i++)
            {
                tempslots.Add(true);
            }
            player.SetMyData("CHANGE_WITH_SLOTS", tempslots);
            offerMaker.SetMyData("CHANGE_WITH_SLOTS", tempslots);
            GUI.Dashboard.OpenTrade(player);
            GUI.Dashboard.OpenTrade(offerMaker);

            player.ResetMyData("OFFER_MAKER");
        }

        public static void ReanimTarget(Player player, Player target)
        {
            try
            {
                if (player.Position.DistanceTo(target.Position) > 2)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок слишком далеко от Вас", 3000);
                    return;
                }
                var item = nInventory.Find(Main.Players[player].UUID, ItemType.HealthKit);
                if (item == null || item.Count < 1)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У Вас нет аптечки", 3000);
                    return;
                }

                if (!target.HasMyData("IS_DYING"))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Игрок не нуждается в реанимации");
                    return;
                }

                if (target.HasMyData("REANIMING"))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Этого игрока уже кто-то реанимирует");
                    return;
                }

                nInventory.Remove(player, ItemType.HealthKit, 1);
                /*if (target.HasData("IS_DYING"))
                {*/
                player.PlayAnimation("amb@medic@standing@tendtodead@idle_a", "idle_a", 39);
                target.SetMyData("REANIMING", true);
                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы начали реанимирование игрока ({target.Value})", 3000);
                Notify.Send(target, NotifyType.Info, NotifyPosition.BottomCenter, $"Игрок ({player.Value}) начал реанимировать Вас", 3000);
                NAPI.Task.Run(() => {
                    try
                    {
                        player.StopAnimation();
                        NAPI.Entity.SetEntityPosition(player, player.Position + new Vector3(0, 0, 0.5));

                        if (Main.Players[player].Fraction.FractionID != 8)
                        {
                            var random = new Random();
                            if (random.Next(0, 11) <= 5)
                            {
                                target.ResetMyData("REANIMING");
                                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Игрок ({target.Value}) чуть ласты не склеил. У Вас не вышло его реанимировать", 3000);
                                return;
                            }
                        }
                        else
                        {
                            if (!target.HasMyData("NEXT_DEATH_MONEY") || DateTime.Now > target.GetMyData<DateTime>("NEXT_DEATH_MONEY"))
                            {
                                MoneySystem.Wallet.Change(player, 500);
                                Stocks.fracStocks[8].Money += 300;
                                GameLog.Money($"server", $"player({Main.Players[player].UUID})", 500, $"revieve({Main.Players[target].UUID})");
                                target.SetMyData("NEXT_DEATH_MONEY", DateTime.Now.AddMinutes(15));
                            }
                        }

                        target.StopAnimation();
                        NAPI.Entity.SetEntityPosition(target, target.Position + new Vector3(0, 0, 0.5));
                        target.SetSharedData("InDeath", false);
                        Trigger.PlayerEvent(target, "DeathTimer", false);
                        target.Health = 50;
                        target.ResetMyData("IS_DYING");
                        target.ResetMyData("REANIMING");
                        target.ResetSharedData("IS_DYING");
                        Main.Players[target].IsAlive = true;
                        Main.OffAntiAnim(target);
                        if (target.HasMyData("DYING_TIMER"))
                        {
                            //Main.StopT(target.GetData("DYING_TIMER"), "timer_18");
                            Timers.Stop(target.GetMyData<string>("DYING_TIMER"));
                            target.ResetMyData("DYING_TIMER");
                        }
                        Notify.Send(target, NotifyType.Info, NotifyPosition.BottomCenter, $"Игрок ({player.Value}) реанимировал Вас", 3000);
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы реанимировали игрока ({target.Value})", 3000);

                        #region GBPКвест: 15 Реанимировать 100 человек.

                        #region BattlePass выполнение квеста
                        BattlePass.updateBPGlobalQuestIteration(player, BattlePass.BPGlobalQuestType.RevivePlayers);
                        #endregion

                        #endregion

                        client.Core.Achievements.AddAchievementScore(player, client.Core.AchievementID.Reanim, 1);
                    }
                    catch (Exception e) { Log.Write("playerHealedtarget: " + e.StackTrace, nLog.Type.Error); }
                }, 15000);
                /*}
                else
                {
                    Notify.Send(target, NotifyType.Info, NotifyPosition.BottomCenter, $"Игрок ({player.Value}) вылечил Вас с помощью аптечки", 3000);
                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы вылечили игрока ({target.Value}) с помощью аптечки", 3000);
                    target.Health = 100;
                }*/
                return;
            }
            catch (Exception e) { Log.Write("playerHealTarget: " + e.StackTrace); }
        }

        public static void playerHandshakeTarget(Player player, Player target)
        {
            var playerCuffed = false;
            var playerInDeath = false;
            if(player.HasMyData("CUFFED") && player.GetMyData<bool>("CUFFED") == true) playerCuffed = true;
            if(player.HasSharedData("InDeath") && player.GetSharedData<bool>("InDeath") == true) playerInDeath = true;

            var targetCuffed = false;
            var targetInDeath = false;
            if(target.HasMyData("CUFFED") && target.GetMyData<bool>("CUFFED") == true) targetCuffed = true;
            if(target.HasSharedData("InDeath") && target.GetSharedData<bool>("InDeath") == true) targetInDeath = true;

            if (!playerCuffed && !playerInDeath)
            {
                if (!targetCuffed && !targetInDeath)
                {
                    target.SetMyData("HANDSHAKER", player);
                    target.SetMyData("REQUEST", "HANDSHAKE");
                    target.SetMyData("IS_REQUESTED", true);

                    var playerTextName = $"Неизвестный ({player.Value})";
                    if(Main.Players[target].Friends.Contains(player.Name)) playerTextName = player.Name + $" ({player.Value})";

                    var interactionText = playerTextName+ " предлагает Вам пожать руку";

                    Trigger.ClientEvent(target, "showTargetInteraction", interactionText, player.Value, player.Name);
                    //Notify.Send(target, NotifyType.Warning, NotifyPosition.BottomCenter, $"Игрок ({player.Value}) хочет пожать Вам руку. Y/N - принять/отклонить", 3000);
                    Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы предложили игроку ({target.Value}) пожать руку.", 3000);
                }
                else Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Невозможно пожать руку игроку в данный момент", 3000);
            }
            else Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Невозможно пожать руку игроку в данный момент", 3000);
        }

        public static void hanshakeTarget(Player player)
        {
            if (!Main.Players.ContainsKey(player) || !player.HasMyData("HANDSHAKER") || !Main.Players.ContainsKey(player.GetMyData<Player>("HANDSHAKER"))) return;
            Player target = player.GetMyData<Player>("HANDSHAKER");

            var playerCuffed = false;
            var playerInDeath = false;
            if(player.HasMyData("CUFFED") && player.GetMyData<bool>("CUFFED") == true) playerCuffed = true;
            if(player.HasSharedData("InDeath") && player.GetSharedData<bool>("InDeath") == true) playerInDeath = true;

            var targetCuffed = false;
            var targetInDeath = false;
            if(target.HasMyData("CUFFED") && target.GetMyData<bool>("CUFFED") == true) targetCuffed = true;
            if(target.HasSharedData("InDeath") && target.GetSharedData<bool>("InDeath") == true) targetInDeath = true;

            if (!playerCuffed && !playerInDeath)
            {
                if (!targetCuffed && !targetInDeath)
                {
                    player.PlayAnimation("mp_ped_interaction", "handshake_guy_a", 39);
                    target.PlayAnimation("mp_ped_interaction", "handshake_guy_a", 39);

                    Trigger.ClientEvent(player, "newFriend", target);
                    Trigger.ClientEvent(target, "newFriend", player);

                    Main.OnAntiAnim(player);
                    Main.OnAntiAnim(target);

                    if (!Main.Players[player].Friends.Contains(target.Name) && !Main.Players[target].Friends.Contains(player.Name)) {
                        Main.Players[player].Friends.Add(target.Name);
                        Main.Players[target].Friends.Add(player.Name);

                        client.Core.Achievements.AddAchievementScore(player, client.Core.AchievementID.Meeting, 1);
                        client.Core.Achievements.AddAchievementScore(target, client.Core.AchievementID.Meeting, 1);
                    }

                    NAPI.Task.Run(() => { try { Main.OffAntiAnim(player); Main.OffAntiAnim(target); player.StopAnimation(); target.StopAnimation(); } catch (Exception e){ Log.Write(e.StackTrace, nLog.Type.Error); } }, 4500);
                }
            }
        }

        internal class SearchObject
        {
            public string Name { get; set; }
            public List<string> Weapons { get; set; }
            public List<string> Items { get; set; }
            public List<string> Illegal { get; set; }
        }

        [RemoteEvent("SERVER::ANIMATION:TOGGLE")]
        public static void animationSelected(Player player, string name, bool toggle)
        {
            try
            {
                if (player.HasMyData("AntiAnimDown") || player.HasMyData("FOLLOWING") || player.IsInVehicle
                || Main.Players[player].ArrestTime > 0 || Main.Players[player].DemorganTime > 0) return;

                if (!EmotionsList.Contains(name) && !WalkingList.Contains(name) && !toggle)
                {
                    player.ResetMyData("HANDS_UP");
                    player.StopAnimation();
                    if (player.HasMyData("LastAnimFlag") && player.GetMyData<int>("LastAnimFlag") == 39)
                        NAPI.Entity.SetEntityPosition(player, player.Position + new Vector3(0, 0, 0.2));
                    return;
                }
                else if (EmotionsList.Contains(name) && !toggle)
                {
                    player.SetSharedData("playermood", 0);
                    NAPI.ClientEvent.TriggerClientEventInRange(player.Position, 250, "Player_SetMood", player, 0);
                    return;
                }
                else if (WalkingList.Contains(name) && !toggle)
                {
                    player.SetSharedData("playerws", 0);
                    NAPI.ClientEvent.TriggerClientEventInRange(player.Position, 250, "Player_SetWalkStyle", player, 0);
                    return;
                }
                else
                {
                    if (EmotionsList.Contains(name))
                    { // Лицевые эмоции
                        player.SetSharedData("playermood", EmotionsList.IndexOf(name));
                        NAPI.ClientEvent.TriggerClientEventInRange(player.Position, 250, "Player_SetMood", player, EmotionsList.IndexOf(name));
                        return;
                    }
                    else if (WalkingList.Contains(name))
                    { // Стили походки
                        player.SetSharedData("playerws", WalkingList.IndexOf(name));
                        NAPI.ClientEvent.TriggerClientEventInRange(player.Position, 250, "Player_SetWalkStyle", player, WalkingList.IndexOf(name));
                        return;
                    }
                    else
                    {
                        if (!AnimationDictionary.ContainsKey(name)) return;

                        player.PlayAnimation(AnimationDictionary[name].Dictionary, AnimationDictionary[name].Name, AnimationDictionary[name].Flag);
                        //if (category == 0 && animation == 0) NAPI.Entity.SetEntityPosition(player, player.Position - new Vector3(0, 0, 0.3));

                        if (AnimationDictionary[name].Dictionary == "random@arrests@busted" && AnimationDictionary[name].Name == "idle_c") player.SetMyData("HANDS_UP", true);

                        player.SetMyData("LastAnimFlag", AnimationDictionary[name].Flag);
                        if (AnimationDictionary[name].StopDelay != -1)
                        {
                            NAPI.Task.Run(() => {
                                try
                                {
                                    if (player != null && !player.HasMyData("AntiAnimDown") && !player.HasMyData("FOLLOWING"))
                                    {
                                        player.StopAnimation();
                                    }
                                }
                                catch (Exception e){ Log.Write(e.StackTrace, nLog.Type.Error); }
                            }, AnimationDictionary[name].StopDelay);
                        }
                    }
                }
            }
            catch (Exception e) { Log.Write("aSelected: " + e.StackTrace, nLog.Type.Error); }
        }

        public static Dictionary<string, Animation> AnimationDictionary = new Dictionary<string, Animation>()
        {
            { "Встать на колено", new Animation("amb@medic@standing@kneel@base", "base", 39)},
            { "Сидеть расслабленно", new Animation("mp_safehouse", "lap_dance_player", 39)},
            { "Сидеть на корточках", new Animation("amb@medic@standing@tendtodead@base", "base", 39)},
            { "Сидеть на чем либо", new Animation("misstrevor2", "gang_chatting_idle02_a", 39)},
            { "Сидеть в унынии", new Animation("anim@heists@ornate_bank@hostages@hit", "hit_loop_ped_b", 39)},
            { "Сидеть оглядываясь", new Animation("rcm_barry3", "barry_3_sit_loop", 39)},
            { "Сидеть облокотившись", new Animation("amb@world_human_picnic@male@base", "base", 39)},
            { "Сидеть в страхе", new Animation("amb@lo_res_idles@", "squat_lo_res_base", 39)},
            { "Сидеть полулежа", new Animation("amb@world_human_picnic@female@base", "base", 39)},
            { "Сидеть задумавшись", new Animation("amb@world_human_stupor@male@base", "base", 39)},
            { "Лежать на спине", new Animation("amb@world_human_sunbathe@male@back@base", "base", 39)},
            { "Лежать в стиле Рок-н-Ролл", new Animation("missfinale_c1@", "lying_dead_player0", 39)},
            { "Лежать схватившись за голову", new Animation("rcmtmom_2leadinout", "tmom_2_leadout_loop", 39)},
            { "Смотреть на солнце", new Animation("timetable@denice@ig_1", "base", 39)},
            { "Лежать пьяным", new Animation("amb@world_human_bum_slumped@male@laying_on_left_side@base", "base", 39)},
            { "Лежать на животе", new Animation("amb@world_human_sunbathe@female@front@idle_a", "idle_a", 39)},
            { "Развалиться на диване", new Animation("misslester1aig_5main", "idle_02_jaynorris", 39)},
            { "Притвориться мёртвым", new Animation("anim@melee@machete@streamed_core@", "victim_front_takedown", 39)},
            { "Поднять руки", new Animation("random@arrests@busted", "idle_c", 49) },
            { "Осмотреть и записать", new Animation("amb@medic@standing@timeofdeath@idle_a", "idle_a", 49)},
            { "Лайк", new Animation("anim@mp_player_intselfiethumbs_up", "idle_a", 49)},
            { "Воинское приветствие", new Animation("anim@mp_player_intuppersalute", "idle_a", 49)},
            { "Крутить у виска двумя руками", new Animation("anim@mp_player_intupperyou_loco", "idle_a", 49)},
            { "Королевское приветствие", new Animation("anim@mp_player_intupperwave", "idle_a", 49)},
            { "Понтоваться", new Animation("anim@mp_player_intupperv_sign", "idle_a", 49)},
            { "Двойной лайк", new Animation("anim@mp_player_intupperthumbs_up", "idle_a", 49) },
            { "Испугать", new Animation("anim@mp_player_intupperthumb_on_ears", "idle_a", 49) },
            { "Сдаться", new Animation("anim@mp_player_intuppersurrender", "idle_a", 49) },
            { "Медленно хлопать", new Animation("anim@mp_player_intupperslow_clap", "idle_a", 49) },
            { "Мир", new Animation("anim@mp_player_intupperpeace", "idle_a", 49) },
            { "Отказ", new Animation("anim@mp_player_intupperno_way", "idle_a", 49) },
            { "Радость", new Animation("anim@mp_player_intupperjazz_hands", "idle_a", 49) },
            { "Показать рыбку", new Animation("anim@mp_player_intupperfind_the_fish", "idle_a", 49)},
            { "Фэйспалм", new Animation("anim@mp_player_intupperface_palm", "idle_a", 49)},
            { "Показать курочку",new Animation("anim@mp_player_intupperchicken_taunt", "idle_a", 49)},
            { "ОК", new Animation("anim@mp_player_intselfiedock", "idle_a", 49)},
            { "Позвать за собой", new Animation("friends@frf@ig_1", "over_here_idle_b", 49)},
            { "РОК", new Animation("mp_player_int_upperrock", "mp_player_int_rock", 49)},
            { "Мир всем", new Animation("mp_player_int_upperpeace_sign", "mp_player_int_peace_sign", 49)},
            { "Руки за голову на коленях", new Animation("rcmbarry", "m_cower_01", 39)},
            { "Руки вверх на коленях", new Animation("random@arrests", "kneeling_arrest_idle", 39)},
            { "Сдаться 2", new Animation("random@mugging3", "handsup_standing_base", 39)},
            { "Сидеть в унынии 2", new Animation("anim@heists@ornate_bank@hostages@hit", "hit_loop_ped_b", 39)},
            { "Сильно нервничать дёргая ногой", new Animation("rcmme_tracey1", "nervous_loop", 39)},
            { "Саркастично хлопать №2", new Animation("anim@mp_player_intcelebrationmale@slow_clap", "slow_clap", 49)},
            { "Женское неодобрение", new Animation("anim@mp_player_intcelebrationfemale@no_way", "no_way", 39)},
            { "Ну на двоечку", new Animation("anim@mp_player_intcelebrationfemale@v_sign", "v_sign", 49)},
            { "Женское неодобрение 2", new Animation("anim@mp_player_intcelebrationfemale@finger", "finger", 39)},
            { "facepalm", new Animation("anim@mp_player_intcelebrationfemale@face_palm", "face_palm", 39)},
            { "Нет", new Animation("mp_player_int_upper_nod", "mp_player_int_nod_no", 39)},
            { "Ты сумашедший!", new Animation("anim@mp_player_intcelebrationfemale@you_loco", "you_loco", 39)},
            { "Отряхиваться", new Animation("reaction@shake_it_off@", "dustoff", 39)},
            { "Выразить респект", new Animation("anim@mp_player_intcelebrationfemale@bro_love", "bro_love", 39)},
            { "Отпрявлять воздушные поцелуи", new Animation("anim@mp_player_intcelebrationfemale@blow_kiss", "blow_kiss", 49)},
            { "Стучать в дверь (со звуком)", new Animation("timetable@jimmy@doorknock@", "knockdoor_idle", 39)},
            { "Ставить пальцы вверх", new Animation("anim@mp_player_intcelebrationfemale@thumbs_up", "thumbs_up", 49)},
            { "Ммм, брависимо", new Animation("anim@mp_player_intcelebrationfemale@finger_kiss", "finger_kiss", 39)},
            { "Махать руками привлекая внимание", new Animation("random@prisoner_lift", "arms_waving", 49)},
            { "Дождь из денег", new Animation("anim@mp_player_intcelebrationfemale@raining_cash", "raining_cash", 49)},
            { "Кидаться козявкой", new Animation("anim@mp_player_intcelebrationfemale@nose_pick", "nose_pick", 49)},
            { "Болельщик", new Animation("missmic_4premiere", "movie_prem_02_f_b", 49)},
            { "Как вы меня задолбали", new Animation("06266 mini@crp@chsr_a@crp_str", "crp_fail 25333", 49)},
            { "Зарядка 1", new Animation("amb@world_human_yoga@female@base", "base_a", 39)},
            { "Зарядка 2", new Animation("amb@world_human_yoga@male@base", "base_b", 39)},
            { "Качать пресс", new Animation("amb@world_human_sit_ups@male@base", "base", 39)},
            { "Отжиматься", new Animation("amb@world_human_push_ups@male@base", "base", 39)},
            { "Медитировать", new Animation("rcmcollect_paperleadinout@", "meditiate_idle", 39)},
            { "Показать средний палец", new Animation("anim@mp_player_intselfiethe_bird", "idle_a", 49)},
            { "Показать что-то ещё", new Animation("anim@mp_player_intincardockstd@ps@", "idle_a", 49)},
            { "Ковыряться в носу", new Animation("anim@mp_player_intuppernose_pick", "idle_a", 49)},
            { "Показать средний палец всем", new Animation("anim@mp_player_intupperfinger", "idle_a", 49)},
            { "Показать средний палец яростно", new Animation("mp_player_intfinger", "mp_player_int_finger", 39)},
            { "Стоять, руки на поясе", new Animation("amb@world_human_cop_idles@male@base", "base", 39)},
            { "Размять руки", new Animation("anim@mp_player_intupperknuckle_crunch", "idle_a", 49)},
            { "Скрестить руки на груди", new Animation("anim@amb@nightclub@peds@", "rcmme_amanda1_stand_loop_cop", 39)},
            { "Стоять, прогнать человека", new Animation("anim@amb@nightclub@peds@", "mini_strip_club_idles_bouncer_go_away_go_away", 39)},
            { "Стоять, отказать в проходе", new Animation("anim@amb@nightclub@peds@", "mini_strip_club_idles_bouncer_stop_stop", 39)},
            { "Показать бицепс 1", new Animation("anim@amb@nightclub@peds@", "amb_world_human_muscle_flex_arms_in_front_base", 39)},
            { "Показать бицепс 2", new Animation("amb@world_human_muscle_flex@arms_at_side@base", "base", 39)},
            { "Показать бицепс 3", new Animation("amb@world_human_muscle_flex@arms_at_side@idle_a", "idle_a", 39) },
            { "Показать бицепс 4", new Animation("amb@world_human_muscle_flex@arms_at_side@idle_a", "idle_c", 39)},
            { "Показать бицепс 5", new Animation("amb@world_human_muscle_flex@arms_in_front@idle_a", "idle_a", 39)},
            { "Показать бицепс 6", new Animation("amb@world_human_muscle_flex@arms_in_front@idle_a", "idle_b", 39)},
            { "Читать реп", new Animation("missfbi3_sniping", "dance_m_default", 39)},
            { "Танец волна", new Animation("anim@mp_player_intupperfind_the_fish", "idle_a", 39)},
            { "Лезгинка", new Animation("special_ped@mountain_dancer@monologue_3@monologue_3a", "mnt_dnc_buttwag", 39)},
            { "Танец зумбы", new Animation("timetable@tracy@ig_5@idle_a", "idle_b", 39)},
            { "Активный танец на месте", new Animation("anim@amb@nightclub@dancers@crowddance_facedj@", "hi_dance_facedj_13_v1_male^4", 39)},
            { "Танец качающийся (Танец кача)", new Animation("anim@amb@nightclub@dancers@crowddance_facedj@", "hi_dance_facedj_17_v1_male^5", 39)},
            { "Танец на месте №1", new Animation("anim@amb@nightclub@dancers@black_madonna_entourage@", "hi_dance_facedj_09_v2_male^5", 39)},
            { "Танец на месте №2", new Animation("anim@amb@nightclub@dancers@crowddance_facedj@", "hi_dance_facedj_11_v1_male^2", 39)},
            { "Танец на месте №3", new Animation("anim@amb@nightclub@dancers@crowddance_facedj@", "hi_dance_facedj_11_v1_male^1", 39)},
            { "Танец на месте №4", new Animation("anim@amb@nightclub@dancers@crowddance_facedj@", "hi_dance_facedj_17_v1_male^6", 39)},
            { "Танец на месте №5", new Animation("anim@amb@nightclub@dancers@crowddance_facedj@", "hi_dance_facedj_17_v1_male^3", 39)},
            { "Танец на месте №6", new Animation("anim@amb@nightclub@dancers@crowddance_facedj@", "mi_dance_facedj_17_v1_male^1", 39)},
            { "Скромный танец №1", new Animation("anim@amb@nightclub@peds@", "amb_world_human_partying_female_partying_beer_base", 39)},
            { "Скромный танец №2", new Animation("amb@world_human_partying@female@partying_beer@base", "base", 39)},
            { "Клубный танец №1", new Animation("anim@amb@nightclub@dancers@crowddance_facedj@", "hi_dance_facedj_09_v2_female^3", 39)},
            { "Танец Диско", new Animation("anim@amb@nightclub@lazlow@hi_podium@", "danceidle_mi_17_crotchgrab_laz", 39)},
            { "Танец Мачо", new Animation("anim@amb@nightclub@lazlow@hi_podium@", "danceidle_mi_17_teapotthrust_laz", 39)},
            { "Танец с палками", new Animation("anim@amb@nightclub@lazlow@hi_railing@", "ambclub_09_mi_hi_bellydancer_laz", 39)},
            { "Танец с палками №2", new Animation("anim@amb@nightclub@lazlow@hi_railing@", "ambclub_10_mi_hi_crotchhold_laz ", 39)},
            { "Танец с палками №3", new Animation("anim@amb@nightclub@lazlow@hi_railing@", "ambclub_12_mi_hi_bootyshake_laz", 39)},
            { "Танец на месте №7", new Animation("anim@amb@nightclub@mini@dance@dance_solo@female@var_a@", "med_center", 39)},
            { "Низкий флекс", new Animation("anim@amb@nightclub@mini@dance@dance_solo@female@var_b@", "med_center_down", 39)},
            { "Стриптиз 1", new Animation("mini@strip_club@lap_dance@ld_girl_a_song_a_p1", "ld_girl_a_song_a_p1_f", 39)},
            { "Стриптиз 2", new Animation("mini@strip_club@lap_dance_2g@ld_2g_p1", "ld_2g_p1_s2", 39)},
            { "Стриптиз 3", new Animation("mini@strip_club@lap_dance_2g@ld_2g_p2", "ld_2g_p2_s2", 39)},
            { "Стриптиз 4", new Animation("mini@strip_club@private_dance@part1", "priv_dance_p1", 39)},
            { "Стриптиз 5", new Animation("mini@strip_club@private_dance@part3", "priv_dance_p3", 39)},
            { "Стриптиз 6", new Animation("mini@strip_club@lap_dance@ld_girl_a_song_a_p1", "ld_girl_a_song_a_p1_f", 39)},
            { "Танец руками", new Animation("misschinese2_crystalmazemcs1_cs", "dance_loop_tao", 39)},
            { "Танец робота", new Animation("anim@amb@nightclub@lazlow@hi_podium@", "danceidle_mi_15_robot_laz", 39)},
            { "Танец бедрами", new Animation("anim@amb@nightclub@lazlow@hi_podium@", "danceidle_mi_15_shimmy_laz", 39)},
            { "Танец паучка", new Animation("anim@amb@nightclub@lazlow@hi_podium@", "danceidle_hi_17_spiderman_laz", 39)},
            { "Танец Лейла", new Animation("anim@amb@nightclub@dancers@crowddance_facedj@", "hi_dance_facedj_11_v1_female^3", 39)},
            { "Танец локтями", new Animation("anim@amb@nightclub@dancers@crowddance_facedj@", "hi_dance_facedj_13_v2_female^1", 39)},
            { "Энергосберегающий танец", new Animation("anim@amb@nightclub@dancers@crowddance_facedj@", "li_dance_facedj_13_v2_female^3", 39)},
            { "Танец 'В отрыв'", new Animation("anim@amb@nightclub@dancers@crowddance_facedj@", "hi_dance_facedj_11_v1_male^4", 39)},
            { "Шафл руками", new Animation("anim@amb@nightclub@mini@dance@dance_solo@female@var_a@", "high_center_down", 39)},
            { "Танец пингвина", new Animation("move_clown@p_m_two_idles@", "fidget_short_dance", 39)},
            { "Пританцовывать 2", new Animation("missfbi3_party_b", "talk_inside_loop_female", 39)},
            { "Пританцовывать 3", new Animation("missfbi3_sniping", "dance_m_default", 39)},
            { "Изображать dj", new Animation("anim@mp_player_intcelebrationfemale@dj", "dj", 39)},

        };

        public static List<string> EmotionsList = new List<string>()
        {
            "Очистить лицевую эмоцию",
            "Презрение",
            "Хмурость",
            "Подшофе",
            "Веселье",
            "Удивление",
            "Злость",
        };

        public static List<string> WalkingList = new List<string>()
        {
            "Очистить стиль походки",
            "Стремительный",
            "Уверенный",
            "Вразвалку",
            "Грустный",
            "Женственный",
            "Испуганный",
            "Быстрый",
        };

        /*public static List<List<Animation>> AnimList = new List<List<Animation>>()
        {
            new List<Animation>() // 0
            {
                new Animation("amb@world_human_picnic@female@base", "base", 39),
                new Animation("amb@medic@standing@tendtodead@base", "base", 39),
                new Animation("amb@world_human_stupor@male@base", "base", 39),
                new Animation("amb@world_human_sunbathe@male@back@base", "base", 39),
                new Animation("missfinale_c1@", "lying_dead_player0", 39),
                new Animation("amb@medic@standing@kneel@base", "base", 39),
                new Animation("mp_safehouse", "lap_dance_player", 39),
                new Animation("misstrevor2", "gang_chatting_idle02_a", 39),
            },
            new List<Animation>() // 1
            {
                new Animation("random@arrests@busted", "idle_c", 49),
                new Animation("amb@medic@standing@timeofdeath@idle_a", "idle_a", 49),
                new Animation("anim@mp_player_intselfiethumbs_up", "idle_a", 49),
                new Animation("anim@mp_player_intuppersalute", "idle_a", 49),

                new Animation("anim@mp_player_intupperyou_loco", "idle_a", 49),
                new Animation("anim@mp_player_intupperwave", "idle_a", 49),
                new Animation("anim@mp_player_intupperv_sign", "idle_a", 49),

            },
            new List<Animation>() /// 2
            {
                new Animation("amb@world_human_yoga@female@base", "base_a", 39),
                new Animation("amb@world_human_yoga@male@base", "base_b", 39),
                new Animation("amb@world_human_sit_ups@male@base", "base", 39),
                new Animation("amb@world_human_push_ups@male@base", "base", 39),
                new Animation("rcmcollect_paperleadinout@", "meditiate_idle", 39),
            },
            new List<Animation>() // 3
            {
                new Animation("anim@mp_player_intselfiethe_bird", "idle_a", 49),
                new Animation("anim@mp_player_intincardockstd@ps@", "idle_a", 49),
                new Animation("anim@mp_player_intuppernose_pick", "idle_a", 49),
                new Animation("anim@mp_player_intupperfinger", "idle_a", 49),
                new Animation("mp_player_intfinger", "mp_player_int_finger", 39),
            },
            new List<Animation>() // 4
            {
                new Animation("amb@world_human_cop_idles@male@base", "base", 39),
                new Animation("anim@mp_player_intupperknuckle_crunch", "idle_a", 49),
                new Animation("anim@amb@nightclub@peds@", "rcmme_amanda1_stand_loop_cop", 39),
                new Animation("anim@amb@nightclub@peds@", "mini_strip_club_idles_bouncer_go_away_go_away", 39),
                new Animation("anim@amb@nightclub@peds@", "mini_strip_club_idles_bouncer_stop_stop", 39),
                new Animation("anim@amb@nightclub@peds@", "amb_world_human_muscle_flex_arms_in_front_base", 39),
                new Animation("amb@world_human_muscle_flex@arms_at_side@base", "base", 39),
            },
            new List<Animation>() // 5
            {
                new Animation("amb@world_human_partying@female@partying_beer@base", "base", 39),
                new Animation("amb@world_human_strip_watch_stand@male_a@idle_a", "idle_c", 39),
                new Animation("mini@strip_club@idles@dj@idle_04", "idle_04", 39),
                new Animation("mini@strip_club@lap_dance@ld_girl_a_song_a_p1", "ld_girl_a_song_a_p1_f", 39),
                new Animation("special_ped@mountain_dancer@monologue_3@monologue_3a", "mnt_dnc_buttwag", 39),
                new Animation("mini@strip_club@private_dance@part1", "priv_dance_p1", 39),
                new Animation("anim@amb@nightclub@dancers@crowddance_facedj@", "hi_dance_facedj_09_v2_female^1", 39),
            },
            new List<Animation>() // 6
            {
                null,
            },
            new List<Animation>() // 7
            {
                new Animation("anim@amb@nightclub@dancers@crowddance_facedj@", "hi_dance_facedj_09_v2_female^3", 39),
                new Animation("anim@amb@nightclub@dancers@crowddance_facedj@", "hi_dance_facedj_09_v2_male^2", 39),
                new Animation("anim@amb@nightclub@dancers@crowddance_facedj@", "hi_dance_facedj_09_v2_male^4", 39),
                new Animation("anim@amb@nightclub@dancers@crowddance_groups@", "hi_dance_crowd_09_v1_female^1", 39),
                new Animation("anim@amb@nightclub@dancers@crowddance_groups@", "hi_dance_crowd_09_v2_female^1", 39),
                new Animation("anim@amb@nightclub@dancers@crowddance_groups@", "hi_dance_crowd_09_v2_female^3", 39),
                new Animation("anim@amb@nightclub@dancers@crowddance_groups@", "hi_dance_crowd_11_v1_female^1", 39),
            },
            new List<Animation>() // 8
            {
                new Animation("anim@amb@nightclub@dancers@crowddance_groups@", "hi_dance_crowd_13_v2_female^1", 39),
                new Animation("anim@amb@nightclub@lazlow@hi_podium@", "danceidle_mi_17_crotchgrab_laz", 39),
                new Animation("anim@amb@nightclub@lazlow@hi_podium@", "danceidle_mi_17_teapotthrust_laz", 39),
                new Animation("anim@amb@nightclub@lazlow@hi_railing@", "ambclub_09_mi_hi_bellydancer_laz", 39),
                new Animation("anim@amb@nightclub@lazlow@hi_railing@", "ambclub_10_mi_hi_crotchhold_laz", 39),
                new Animation("anim@amb@nightclub@lazlow@hi_railing@", "ambclub_12_mi_hi_bootyshake_laz", 39),
                new Animation("anim@amb@nightclub@lazlow@hi_railing@", "ambclub_13_mi_hi_sexualgriding_laz", 39),
            },
            new List<Animation>() // 9
            {
                new Animation("anim@amb@nightclub@mini@dance@dance_solo@female@var_a@", "med_center", 39),
                new Animation("anim@amb@nightclub@mini@dance@dance_solo@male@var_b@", "med_center", 39),
            },
            new List<Animation>() // 10
            {
                new Animation("anim@mp_player_intupperthumbs_up", "idle_a", 49),
                new Animation("anim@mp_player_intupperthumb_on_ears", "idle_a", 49),
                new Animation("anim@mp_player_intuppersurrender", "idle_a", 49),
                new Animation("anim@mp_player_intupperslow_clap", "idle_a", 49),
                new Animation("anim@mp_player_intupperpeace", "idle_a", 49),
                new Animation("anim@mp_player_intupperno_way", "idle_a", 49),
                new Animation("anim@mp_player_intupperjazz_hands", "idle_a", 49),
            },
            new List<Animation>() // 11
            {
                new Animation("anim@mp_player_intupperfind_the_fish", "idle_a", 49),
                new Animation("anim@mp_player_intupperface_palm", "idle_a", 49),
                new Animation("anim@mp_player_intupperchicken_taunt", "idle_a", 49),
                new Animation("anim@mp_player_intselfiedock", "idle_a", 49),
                new Animation("friends@frf@ig_1", "over_here_idle_b", 49),
                new Animation("mp_player_int_upperrock", "mp_player_int_rock", 49),
                new Animation("mp_player_int_upperpeace_sign", "mp_player_int_peace_sign", 49),
            },
            new List<Animation>() // NOT USED // 12
            {
                null,
            },
            new List<Animation>() // 13
            {
                new Animation("amb@world_human_muscle_flex@arms_at_side@idle_a", "idle_a", 39),
                new Animation("amb@world_human_muscle_flex@arms_at_side@idle_a", "idle_c", 39),
                new Animation("amb@world_human_muscle_flex@arms_in_front@idle_a", "idle_a", 39),
                new Animation("amb@world_human_muscle_flex@arms_in_front@idle_a", "idle_b", 39),
            },
        };*/
        internal class Animation
        {
            public string Dictionary { get; }
            public string Name { get; }
            public int Flag { get; }
            public int StopDelay { get; }

            public Animation(string dict, string name, int flag, int stopDelay = -1)
            {
                Dictionary = dict;
                Name = name;
                Flag = flag;
                StopDelay = stopDelay;
            }
        }
    }
}
