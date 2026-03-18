using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Text;
using NeptuneEvo.Core;
using NeptuneEvo.SDK;
using NeptuneEvo.MoneySystem;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using NeptuneEvo.Houses;
using NeptuneEvo.Families;
using System.Linq;
using client.Jobs.Builder;
using System.ComponentModel;
using client.Fractions.Utils;
using City;
using client.Core;
using client.Systems.BattlePass;

namespace NeptuneEvo.GUI
{
    class Dashboard : Script
    {
        public static void Event_OnPlayerDisconnected(Player player, DisconnectionType type, string reason)
        {
            try
            {
                if (!isopen.ContainsKey(player)) return;
                isopen.Remove(player);
            }
            catch (Exception e) { Log.Write("PlayerDisconnected: " + e.StackTrace, nLog.Type.Error); }
        }

        private static nLog Log = new nLog("Dashboard");

        public static Dictionary<Player, bool> isopen = new Dictionary<Player, bool>();

        public static object gamePlayerData = null;

        private static Dictionary<int, string> Status = new Dictionary<int, string>
        {// Group id, Status
            {0, "Игрок" },
            {16, "Администратор" }
        };

        private static Dictionary<int, string> Gender = new Dictionary<int, string>
        {// Group id, Status
            {0, "Женский" },
            {1, "Мужской" }
        };

        public static Dictionary<int, int> ShelterMaxWeight = new Dictionary<int, int>
        {
            {1, 10000}, // Фракция 65 слотов
            {2, 100000}, // Семья 20 слотов
            {3, 500000}, // Семья 40 слотов
            {4, 1000000}, // Семья 60 слотов

            {5, 30}, // Шкафы 10 слотов
            {6, 20}, // Шкафы 10 слотов
            {7, 25}, // Шкафы 10 слотов
            {8, 25}, // Шкафы 10 слотов
            {9, 30}, // Шкафы 10 слотов
            {10, 30}, // Шкафы 10 слотов
            {11, 25}, // Шкафы 10 слотов
            {12, 25}, // Шкафы 10 слотов

            {13, 35}, // Шкафы 20 слотов
            {14, 35}, // Шкафы 20 слотов
            {15, 35}, // Шкафы 20 слотов
            {16, 30}, // Шкафы 20 слотов
            {17, 40}, // Шкафы 20 слотов
            {18, 40}, // Шкафы 20 слотов
            {19, 40}, // Шкафы 20 слотов
            {20, 40}, // Шкафы 20 слотов
            {21, 40}, // Шкафы 20 слотов

            {22, 65}, // Шкафы 30 слотов
            {23, 70}, // Шкафы 30 слотов
            {24, 45}, // Шкафы 30 слотов
            {25, 50}, // Шкафы 30 слотов
            {26, 45}, // Шкафы 30 слотов
            {27, 45}, // Шкафы 30 слотов
            {28, 45}, // Шкафы 30 слотов

            {29, 100}, // Шкафы 40 слотов
        };

        public static Dictionary<int, int> ShelterMaxSlots = new Dictionary<int, int>
        {
            {1, 65}, // Фракция 10000кг
            {2, 20}, // Семья 100000кг
            {3, 40}, // Семья 500000кг
            {4, 60}, // Семья 1000000кг

            {5, 10}, // Шкафы 10кг
            {6, 10}, // Шкафы 10кг
            {7, 10}, // Шкафы 10кг
            {8, 10}, // Шкафы 10кг
            {9, 10}, // Шкафы 10кг
            {10, 10}, // Шкафы 10кг
            {11, 10}, // Шкафы 10кг
            {12, 10}, // Шкафы 10кг

            {13, 20}, // Шкафы 35кг
            {14, 20}, // Шкафы 35кг
            {15, 20}, // Шкафы 35кг
            {16, 20}, // Шкафы 30кг
            {17, 20}, // Шкафы 40кг
            {18, 20}, // Шкафы 40кг
            {19, 20}, // Шкафы 40кг
            {20, 20}, // Шкафы 40кг
            {21, 20}, // Шкафы 40кг

            {22, 30}, // Шкафы 65кг
            {23, 30}, // Шкафы 70кг
            {24, 30}, // Шкафы 45кг
            {25, 30}, // Шкафы 50кг
            {26, 30}, // Шкафы 45кг
            {27, 30}, // Шкафы 45кг
            {28, 30}, // Шкафы 45кг

            {29, 40}, // Шкафы 100кг

        };

        public static void PsendItems(Player player, List<nItem> items, int type, nItem bag = null)
        {
            try
            {
                if (!Main.Players.ContainsKey(player)) return;
                //Log.Debug("PsendItems: " + JsonConvert.SerializeObject(items));
                if (type != 3) nInventory.CalcWeight(player);

                var maxWeight = 100;
                var name = "Багажник";
                var maxSlots = 65;
                //Log.Debug("PsendItems: type: "+type, nLog.Type.Error);

                #region Outside - внешние инвентари, шкафы, склады итд

                if (type == 4)
                {
                    //Log.Debug("PsendItems: OPENOUT_TYPE: " + player.GetMyData<int>("OPENOUT_TYPE"), nLog.Type.Error);
                    if (player.GetMyData<int>("OPENOUT_TYPE") == 2)
                    {

                        Vehicle veh = player.GetMyData<Vehicle>("SELECTEDVEH");
                        if (veh != null && veh.GetData<string>("ACCESS") == "PERSONAL" || veh.GetData<string>("ACCESS") == "GARAGE")
                        {
                            int id = veh.GetData<int>("ID");
                            if (!VehicleManager.Vehicles.ContainsKey(id)) return;

                            VehicleManager.CalcWeight(id);
                            //Log.Debug("veh.Class " + veh.Class);

                            var vehicleModel = VehicleManager.Vehicles[id].Model;
                            //Log.Debug("model: "+ vehicleModel);
                            maxWeight = VehicleManager.VehicleWeight[vehicleModel.ToString().ToLower()];
                            maxSlots = 65;
                            if (veh.Class == 8) maxSlots = 10;
                            name = "Багажник";
                        }
                        else if (veh != null && veh.GetData<string>("ACCESS") == "FRACTION")
                        {
                            int fracid = veh.GetData<int>("FRACTION");
                            if (!Configs.FractionVehicles[fracid].ContainsKey(veh.NumberPlate.ToString())) return;
                            uint carModel = veh.Model;
                            string carModelName = VehicleManager.VehicleFuel.Keys.FirstOrDefault(s => NAPI.Util.GetHashKey(s) == carModel);
                            if(carModelName == null)
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "У этого автомобиля нету багажника");
                                return;
                            }
                            maxWeight = VehicleManager.VehicleWeight[carModelName];
                            maxSlots = 65;
                            if (veh.Class == 8) maxSlots = 10;
                            name = "Багажник";
                        }
                        else if (veh != null && veh.GetData<string>("ACCESS") == "FAMILY")
                        {
                          int id = veh.GetData<int>("ID");
                          if (!VehicleManager.Vehicles.ContainsKey(id)) return;
                          if (!Manager.isHaveFamily(player))
                          {
                              Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "У вас нету доступа к этому багажнику");
                              return;
                          }
                          if(!Manager.isHaveAccess(player, Manager.FamilyAccess.TakeVehFromGarage))
                          {
                              Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "У вас нету доступа к этому багажнику");
                              return;
                          }
                          VehicleManager.CalcWeight(id);
                          //Log.Debug("veh.Class " + veh.Class);

                          var vehicleModel = VehicleManager.Vehicles[id].Model;
                          //Log.Debug("model: "+ vehicleModel);
                          maxWeight = VehicleManager.VehicleWeight[vehicleModel.ToString().ToLower()];
                          maxSlots = 65;
                          if (veh.Class == 8) maxSlots = 10;
                          name = "Багажник";
                        }
                        else return;
                    }

                    // m4ybe CHECK
                    if (player.GetMyData<int>("OPENOUT_TYPE") == 3)
                    {
                        name = "Шкаф с предметами";

                        maxWeight = ShelterMaxWeight[player.GetMyData<int>("SHELTER_TYPE")];
                        maxSlots = ShelterMaxSlots[player.GetMyData<int>("SHELTER_TYPE")];
                    }

                    if (player.GetMyData<int>("OPENOUT_TYPE") == 4)
                    {
                        name = "Шкаф с одеждой";
                        maxWeight = ShelterMaxWeight[player.GetMyData<int>("SHELTER_TYPE")];
                        maxSlots = ShelterMaxSlots[player.GetMyData<int>("SHELTER_TYPE")];
                        //Log.Debug("PsendItems: maxWeight: " + maxWeight+ " maxSlots: "+ maxSlots, nLog.Type.Error);
                    }

                    if (player.GetMyData<int>("OPENOUT_TYPE") == 8)
                    {
                        name = "Оружейный сейф";
                        maxWeight = ShelterMaxWeight[player.GetMyData<int>("SHELTER_TYPE")];
                        maxSlots = ShelterMaxSlots[player.GetMyData<int>("SHELTER_TYPE")];
                        //Log.Debug("PsendItems: maxWeight: " + maxWeight+ " maxSlots: "+ maxSlots, nLog.Type.Error);
                    }

                    if (player.GetMyData<int>("OPENOUT_TYPE") == 6)
                    {
                        name = "Склад";
                        maxWeight = ShelterMaxWeight[player.GetMyData<int>("SHELTER_TYPE")];
                        maxSlots = ShelterMaxSlots[player.GetMyData<int>("SHELTER_TYPE")];
                       // Log.Debug("PsendItems: maxWeight: " + maxWeight+ " maxSlots: "+ maxSlots, nLog.Type.Error);
                    }

                    if (player.GetMyData<int>("OPENOUT_TYPE") == 1070)
                    {
                        name = "Домашний шкаф";
                        maxWeight = ShelterMaxWeight[player.GetMyData<int>("SHELTER_TYPE")];
                        maxSlots = ShelterMaxSlots[player.GetMyData<int>("SHELTER_TYPE")];
                        //Log.Debug("PsendItems: maxWeight: " + maxWeight+ " maxSlots: "+ maxSlots, nLog.Type.Error);
                    }
                    if (player.GetMyData<int>("OPENOUT_TYPE") == 1080)
                    {
                        name = "Склад семьи";
                        maxWeight = ShelterMaxWeight[player.GetMyData<int>("SHELTER_TYPE")];
                        maxSlots = ShelterMaxSlots[player.GetMyData<int>("SHELTER_TYPE")];
                        //Log.Debug("PsendItems: maxWeight: " + maxWeight + " maxSlots: " + maxSlots, nLog.Type.Error);
                    }
                    if (player.GetMyData<int>("OPENOUT_TYPE") == 1081)
                    {
                        name = "Склад семьи";
                        maxWeight = ShelterMaxWeight[player.GetMyData<int>("SHELTER_TYPE")];
                        maxSlots = ShelterMaxSlots[player.GetMyData<int>("SHELTER_TYPE")];
                        //Log.Debug("PsendItems: maxWeight: " + maxWeight + " maxSlots: " + maxSlots, nLog.Type.Error);
                    }
                    if (player.GetMyData<int>("OPENOUT_TYPE") == 2000)
                    {
                        name = "Аирдроп";
                        maxWeight = ShelterMaxWeight[player.GetMyData<int>("SHELTER_TYPE")];
                        maxSlots = ShelterMaxSlots[player.GetMyData<int>("SHELTER_TYPE")];
                        //Log.Debug("PsendItems: maxWeight: " + maxWeight + " maxSlots: " + maxSlots, nLog.Type.Error);
                    }
                }

                #endregion

                if (type == 2)
                {
                    var bagItem = items.Find(i => nInventory.ClothesItems.Contains(i.Type) && nInventory.ClothesItemSlots[i.Type] == 12 && i.IsActive);
                    if (bagItem == null)
                    {
                        //Log.Debug("inventory backpack  null");
                        Trigger.ClientEvent(player, "inventory", 3, "close");
                    }
                    else
                    {
                        //Log.Debug("inventory backpack found");
                        //Log.Debug(JsonConvert.SerializeObject(bagItem));
                        PsendItems(player, bagItem.Items, 3, bag: bagItem);
                    }
                }

                if (type == 3)
                {

                    var bagDefaultItem = nBags.BagsData[0];

                    name = bagDefaultItem.Name;
                    maxWeight = bagDefaultItem.maxWeight;
                    maxSlots = bagDefaultItem.maxSlots;



                    if (bag != null && bag.Bag != null)
                    {
                        name = bag.Bag.Name;
                        maxWeight = bag.Bag.maxWeight;
                        maxSlots = bag.Bag.maxSlots;
                    }
                    else if (bag != null)
                    {
                        var bagData = (string)bag.Data;
                        var bagVariation = Convert.ToInt32(bagData.Split('_')[0]);
                        var bagTexture = Convert.ToInt32(bagData.Split('_')[1]);

                        var bagItem = nBags.BagsData.FirstOrDefault(b => b.Variation == bagVariation);

                        name = bagItem.Name;
                        maxWeight = bagItem.maxWeight;
                        maxSlots = bagItem.maxSlots;

                        bag.Bag = bagItem;
                        bag.IsBag = true;
                    }

                    nInventory.CalcWeightBag(player, items, maxWeight);
                }

                List<object> data = new List<object>();
                foreach (nItem item in items)
                {
                    try
                    {
                        var itemId = item.ID;
                        var itemFname = nInventory.ItemFname.ContainsKey(item.Type) ? nInventory.ItemFname[item.Type] : "";
                        var itemFType = nInventory.ItemFtype.ContainsKey(item.Type) ? nInventory.ItemFtype[item.Type] : "unknownitem";
                        var itemFdesc = nInventory.ItemFdesc.ContainsKey(item.Type) ? nInventory.ItemFdesc[item.Type] : "";

                        var serialNumber = (nInventory.WeaponsItems.Contains(item.Type) || item.Type == ItemType.Stungun) ? item.WData.Serial : null;
                        var itemDescription = "";

                        if (nInventory.WeaponsItems.Contains(item.Type) || item.Type == ItemType.Stungun)
                          itemDescription += $"Патрон: {item.WData.AmmmoInClip.ToString()} Вес:{item.Weight.ToString()} {itemFdesc}";

                        if (item.Type == ItemType.CarKey)
                          itemDescription += $"{Convert.ToString(item.Data).Split('_')[0]} Вес:{item.Weight.ToString()} {itemFdesc}";

                        if (item.Type == ItemType.SimCard)
                          itemDescription += $"Вес:{item.Weight.ToString()} Номер: {item.Data.Split(',')[0]}";

                        if (item.Type == ItemType.NumberPlate)
                          itemDescription += $"Вес:{item.Weight.ToString()} {item.Data}";

                        if (itemDescription == "") itemDescription += $"Вес:{item.Weight.ToString()} {itemFdesc}";

                        //var itemDescription = (nInventory.WeaponsItems.Contains(item.Type) || item.Type == ItemType.Stungun) ? "Патрон:" + item.WData.AmmmoInClip.ToString() + " Вес:" + item.Weight.ToString() + " " + itemFdesc
                        //        : (item.Type == ItemType.CarKey) ? $"{Convert.ToString(item.Data).Split('_')[0]}" + " Вес:" + item.Weight.ToString() + " " + itemFdesc
                        //        : (item.Type == ItemType.SimCard) ? " Вес:" + item.Weight.ToString() + " Номер: " + item.Data.Split(',')[0]
                        //        : (item.Type == ItemType.NumberPlate) ? " Вес:" + item.Weight.ToString() + " " + item.Data : " Вес:" + item.Weight.ToString() + " " + itemFdesc;

                        //Log.Write("item: ->> " + item.ID, nLog.Type.Error);

                        List<ItemType> valid = null;
                        if (nInventory.ComponentsTypetoType.ContainsKey(item.Type))
                        {
                            valid = nInventory.ComponentsTypetoType[item.Type];
                        }

                        var width = nInventory.ItemSizeW.ContainsKey(item.Type) ? nInventory.ItemSizeW[item.Type] : 1;
                        var height = nInventory.ItemSizeH.ContainsKey(item.Type) ? nInventory.ItemSizeH[item.Type] : 1;

                        Dictionary<string, object> ItemData = new Dictionary<string, object>()
                        {
                            {"id", itemId},
                            {"valid", valid},
                            {"original_type", item.Type},
                            {"content", itemFname},
                            {"type", itemFType},
                            {"weight", item.character_slot_id != 0 && item.IsActive ? 0:item.Weight},
                            {"active", (item.IsActive) ? true : false },
                            {"count", item.Count},
                            {"slot_id", item.slot_id},
                            {"fast_slot_id",item.fast_slot_id},
                            {"WData", item.WData},
                            {"info", new Dictionary<string, object>()
                              {
                                {"serialNumber", serialNumber},
                                {"description", itemDescription},
                              }
                            },
                            {"w", width},
                            {"h", height},
                        };

                        //Log.Debug("itemData: ->> " + JsonConvert.SerializeObject(ItemData), nLog.Type.Error);
                        data.Add(ItemData);

                    }
                    catch (Exception e)
                    {
                        //Log.Debug("EXCEPTION data: ->> " + JsonConvert.SerializeObject(data), nLog.Type.Error);
                        Log.Write("EXCEPTION AT \"DASHBOARD_SENDITEMS\":\n" + e.StackTrace, nLog.Type.Error);
                    }
                }

                string json = JsonConvert.SerializeObject(data);
                //Log.Debug(json);
                //Log.Write("type: -> " + type, nLog.Type.Error);
                Trigger.ClientEvent(player, "inventory", type, json, maxWeight, name, maxSlots);
                data.Clear();
            }
            catch (Exception e) { Log.Write("EXCEPTION AT \"DASHBOARD_SENDITEMS\":\n" + e.StackTrace + e.StackTrace, nLog.Type.Error); }
        }

        //[RemoteEvent("Inventory")]
        //public void ClientEvent_Inventory(Player player, params object[] arguments)
        //{
        //    try
        //    {
        //        if (player == null || !Main.Players.ContainsKey(player)) return;
        //        if (arguments.Length < 3) return;
        //        int type = Convert.ToInt32(arguments[0]);
        //        int index = Convert.ToInt32(arguments[1]);
        //        string data = Convert.ToString(arguments[2]);
        //        Log.Debug($"Type: {type} | Index: {index} | Data: {data}");
        //        Core.Character.Character acc = Main.Players[player];
        //        List<nItem> items;
        //        nItem item;
        //        switch (type)
        //        {
        //            case 0:
        //                {// self inventory
        //                    items = nInventory.Items[acc.UUID];
        //                    item = items[index];
        //                    if (data == "drop")
        //                    {//remove one item from player inventory
        //                        if (item.Type == ItemType.GasCan)
        //                        {
        //                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Возможность выкладывать канистры временно отключена", 3000);
        //                            return;
        //                        }
        //                        else if (item.Type == ItemType.BagWithDrill)
        //                        {
        //                            SafeMain.dropDrillBag(player);
        //                            return;
        //                        }
        //                        else if (item.Type == ItemType.BagWithMoney)
        //                        {
        //                            SafeMain.dropMoneyBag(player);
        //                            return;
        //                        }
        //                        else if (nInventory.ClothesItems.Contains(item.Type))
        //                        {
        //                            if (item.IsActive)
        //                            {
        //                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы должны сначала снять эту одежду", 3000);
        //                                return;
        //                            }
        //                            items.RemoveAt(index);
        //                            Items.onDrop(player, nInventory.ItemsSlots[Main.Players[player].UUID], new nItem(item.Type, 1, item.Data), null);
        //                            sendItems(player);
        //                            return;
        //                        }
        //                        else if (nInventory.WeaponsItems.Contains(item.Type) || nInventory.MeleeWeaponsItems.Contains(item.Type) || item.Type == ItemType.Stungun)
        //                        {
        //                            if (item.IsActive)
        //                            {
        //                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы должны убрать оружие из рук", 3000);
        //                                return;
        //                            }
        //                            items.RemoveAt(index);
        //                            Items.onDrop(player, nInventory.ItemsSlots[Main.Players[player].UUID], new nItem(item.Type, 1, item.Data), null);
        //                            sendItems(player);
        //                            return;
        //                        }
        //                        else if (item.Type == ItemType.CarKey)
        //                        {
        //                            items.RemoveAt(index);
        //                            Items.onDrop(player, nInventory.ItemsSlots[Main.Players[player].UUID], new nItem(item.Type, 1, item.Data), null);
        //                            sendItems(player);
        //                            return;
        //                        }
        //                        if (player.IsInVehicle)
        //                        {
        //                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Нельзя выбрасывать вещи, находясь в машине", 3000);
        //                            return;
        //                        }
        //                        if (item.Count > 1)
        //                        {
        //                            Close(player);
        //                            player.SetMyData("ITEMTYPE", item.Type);
        //                            player.SetMyData("ITEMINDEX", index);
        //                            Trigger.ClientEvent(player, "popup::openInput", "Выбросить предмет", "Введите количество", 3, "item_drop");
        //                            return;
        //                        }
        //                        nInventory.Remove(player, item.Type, 1);
        //                        Items.onDrop(player, nInventory.ItemsSlots[Main.Players[player].UUID], new nItem(item.Type, 1, item.Data), null);
        //                    }
        //                    else if (data == "use")
        //                    {
        //                        try
        //                        {
        //                            Log.Debug($"ItemID: {item.ID} | ItemType: {item.Type} | ItemData: {item.Data} | ItemName: {nInventory.ItemsNames[(int)item.Type]}");
        //                            if (player.HasMyData("CHANGE_WITH"))
        //                            {
        //                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Чтобы использовать вещи нужно завершить обмен вещами", 3000);
        //                                return;
        //                            }
        //                            Items.onUse(player, item, index);
        //                            return;
        //                        }
        //                        catch (Exception e)
        //                        {
        //                            Log.Write(e.ToString(), nLog.Type.Error);
        //                        }
        //                    }
        //                    else if (data == "transfer")
        //                    {
        //                        if (!player.HasMyData("OPENOUT_TYPE")) return;
        //                        if (nInventory.ClothesItems.Contains(item.Type) && item.IsActive == true)
        //                        {
        //                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы должны сначала снять эту одежду", 3000);
        //                            return;
        //                        }
        //                        else if ((nInventory.WeaponsItems.Contains(item.Type) || nInventory.MeleeWeaponsItems.Contains(item.Type)) && item.IsActive == true)
        //                        {
        //                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы должны убрать оружие из рук", 3000);
        //                            return;
        //                        }
        //                        switch (player.GetMyData<int>("OPENOUT_TYPE"))
        //                        {
        //                            case 1:
        //                                return;
        //                            case 2:
        //                                {
        //                                    Vehicle veh = player.GetMyData<Vehicle>("SELECTEDVEH");
        //                                    if (veh is null) return;
        //                                    if (veh.Dimension != player.Dimension)
        //                                    {
        //                                        NeptuneEvo.Core.Commands.SendToAdmins(3, $"!{{#d35400}}[CAR-INVENTORY-EXPLOIT] {player.Name} ({player.Value}) dimension");
        //                                        return;
        //                                    }
        //                                    if (veh.Position.DistanceTo(player.Position) > 10f)
        //                                    {
        //                                        NeptuneEvo.Core.Commands.SendToAdmins(3, $"!{{#d35400}}[CAR-INVENTORY-EXPLOIT] {player.Name} ({player.Value}) distance");
        //                                        return;
        //                                    }

        //                                    int tryAdd = VehicleInventory.TryAdd(veh, new nItem(item.Type, item.Count));
        //                                    if (tryAdd == -1)
        //                                    {
        //                                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "В машине недостаточно места", 3000);
        //                                        return;
        //                                    }

        //                                    if (item.Type == ItemType.BagWithDrill)
        //                                    {
        //                                        player.SetClothes(5, 0, 0);
        //                                        player.ResetMyData("HEIST_DRILL");
        //                                    }
        //                                    else if (item.Type == ItemType.BagWithMoney)
        //                                    {
        //                                        player.SetClothes(5, 0, 0);
        //                                        player.ResetMyData("HAND_MONEY");
        //                                    }

        //                                    if (item.Count > 1)
        //                                    {
        //                                        Close(player);
        //                                        player.SetMyData("ITEMTYPE", item.Type);
        //                                        player.SetMyData("ITEMINDEX", index);
        //                                        Trigger.ClientEvent(player, "popup::openInput", "Переложить предмет", "Введите количество", 3, "item_transfer_toveh");
        //                                        return;
        //                                    }
        //                                    if (item.Type == ItemType.Material)
        //                                    {
        //                                        int maxMats = (client.Fractions.Utils.Stocks.maxMats.ContainsKey((VehicleHash)veh.Model)) ? client.Fractions.Utils.Stocks.maxMats[(VehicleHash)veh.Model] : 600;
        //                                        if (VehicleInventory.GetCountOfType(veh, ItemType.Material) + 1 > maxMats)
        //                                        {
        //                                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Невозможно загрузить такое кол-во матов", 3000);
        //                                            return;
        //                                        }
        //                                    }

        //                                    VehicleInventory.Add(veh, new nItem(item.Type, 1, item.Data));
        //                                    nInventory.Remove(player, item);
        //                                    GameLog.Items($"player({Main.Players[player].UUID})", $"vehicle({veh.NumberPlate})", Convert.ToInt32(item.Type), 1, $"{item.Data}");
        //                                    return;
        //                                }
        //                            case 3:
        //                                {
        //                                    if (item.Type == ItemType.BagWithDrill || item.Type == ItemType.BagWithMoney || item.Type == ItemType.CarKey || item.Type == ItemType.KeyRing || nInventory.ClothesItems.Contains(item.Type) || nInventory.WeaponsItems.Contains(item.Type))
        //                                    {
        //                                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Эта вещь не предназначена для этого шкафа", 3000);
        //                                        return;
        //                                    }
        //                                    if (Main.Players[player].InsideHouseID == -1) return;
        //                                    int houseID = Main.Players[player].InsideHouseID;
        //                                    int furnID = player.GetMyData<int>("OpennedSafe");
        //                                    if (item.Count > 1)
        //                                    {
        //                                        Close(player);
        //                                        player.SetMyData("ITEMTYPE", item.Type);
        //                                        player.SetMyData("ITEMINDEX", index);
        //                                        Trigger.ClientEvent(player, "popup::openInput", "Переложить предмет", "Введите количество", 3, "item_transfer_tosafe");
        //                                        return;
        //                                    }

        //                                    int tryAdd = Houses.FurnitureManager.TryAdd(houseID, furnID, item);
        //                                    if (tryAdd == -1)
        //                                    {
        //                                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Недостаточно места в инвентаре", 3000);
        //                                        return;
        //                                    }
        //                                    GameLog.Items($"player({Main.Players[player].UUID})", $"itemSafe({furnID} | house: {houseID})", Convert.ToInt32(item.Type), 1, $"{item.Data}");
        //                                    nInventory.Remove(player, item.Type, 1);
        //                                    sendItems(player);
        //                                    Houses.FurnitureManager.Add(houseID, furnID, new nItem(item.Type));
        //                                    return;
        //                                }
        //                            case 4:
        //                                {
        //                                    if (!nInventory.ClothesItems.Contains(item.Type))
        //                                    {
        //                                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Шкаф для одежды может хранить только одежду", 3000);
        //                                        return;
        //                                    }
        //                                    if (Main.Players[player].InsideHouseID == -1) return;
        //                                    int houseID = Main.Players[player].InsideHouseID;
        //                                    int furnID = player.GetMyData<int>("OpennedSafe");

        //                                    int tryAdd = Houses.FurnitureManager.TryAdd(houseID, furnID, item);
        //                                    if (tryAdd == -1)
        //                                    {
        //                                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Недостаточно места в инвентаре", 3000);
        //                                        return;
        //                                    }
        //                                    GameLog.Items($"player({Main.Players[player].UUID})", $"clothSafe({furnID} | house: {houseID})", Convert.ToInt32(item.Type), 1, $"{item.Data}");
        //                                    nInventory.Items[acc.UUID].Remove(item);
        //                                    sendItems(player);
        //                                    Houses.FurnitureManager.Add(houseID, furnID, new nItem(item.Type, 1, item.Data));
        //                                    return;
        //                                }
        //                            case 5:
        //                                {
        //                                    if (!player.HasMyData("CHANGE_WITH"))
        //                                    {
        //                                        Close(player);
        //                                        return;
        //                                    }
        //                                    Player target = player.GetMyData<Player>("CHANGE_WITH");
        //                                    if (!Main.Players.ContainsKey(target) || player.Position.DistanceTo(target.Position) > 2)
        //                                    {
        //                                        Close(player);
        //                                        return;
        //                                    }

        //                                    int tryAdd = nInventory.TryAdd(target, new nItem(item.Type, 1));
        //                                    if (tryAdd == -1)
        //                                    {
        //                                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "У игрока недостаточно места", 3000);
        //                                        return;
        //                                    }

        //                                    if (item.Type == ItemType.BodyArmor && nInventory.Find(Main.Players[target].UUID, ItemType.BodyArmor) != null)
        //                                    {
        //                                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Недостаточно места в инвентаре", 3000); // ?
        //                                        return;
        //                                    }

        //                                    if (item.Type == ItemType.BagWithDrill)
        //                                    {
        //                                        if (target.HasMyData("HEIST_DRILL") || target.HasMyData("HAND_MONEY"))
        //                                        {
        //                                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У игрока уже есть дрель или деньги в руках", 3000);
        //                                            return;
        //                                        }

        //                                        target.SetClothes(5, 41, 0);
        //                                        target.SetMyData("HEIST_DRILL", true);
        //                                        player.SetClothes(5, 0, 0);
        //                                        player.ResetMyData("HEIST_DRILL");
        //                                    }
        //                                    else if (item.Type == ItemType.BagWithMoney)
        //                                    {
        //                                        if (target.HasMyData("HEIST_DRILL") || target.HasMyData("HAND_MONEY"))
        //                                        {
        //                                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У Вас уже есть сумка", 3000);
        //                                            return;
        //                                        }

        //                                        target.SetClothes(5, 45, 0);
        //                                        target.SetMyData("HAND_MONEY", true);
        //                                        player.SetClothes(5, 0, 0);
        //                                        player.ResetMyData("HAND_MONEY");
        //                                    }

        //                                    if (item.Count > 1)
        //                                    {
        //                                        Close(player, true);
        //                                        player.SetMyData("ITEMTYPE", item.Type);
        //                                        player.SetMyData("ITEMINDEX", index);
        //                                        Trigger.ClientEvent(player, "popup::openInput", "Передать предмет", "Введите количество", 3, "item_transfer_toplayer");
        //                                        return;
        //                                    }

        //                                    nInventory.Add(target, item);
        //                                    nInventory.Remove(player, item);
        //                                    GameLog.Items($"player({Main.Players[player].UUID})", $"player({Main.Players[target].UUID})", Convert.ToInt32(item.Type), 1, $"{item.Data}");
        //                                    return;
        //                                }
        //                            case 6:
        //                                {
        //                                    if (!nInventory.WeaponsItems.Contains(item.Type) && !nInventory.AmmoItems.Contains(item.Type)) return;
        //                                    int onFraction = player.GetMyData<int>("ONFRACSTOCK");

        //                                    if (onFraction == 0) return;

        //                                    if (client.Fractions.Utils.Stocks.TryAdd(onFraction, new nItem(item.Type, 1)) != 0)
        //                                    {
        //                                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "На складе недостаточно места", 3000);
        //                                        return;
        //                                    }

        //                                    if (item.Count > 1)
        //                                    {
        //                                        Close(player, true);
        //                                        player.SetMyData("ITEMTYPE", item.Type);
        //                                        player.SetMyData("ITEMINDEX", index);
        //                                        Trigger.ClientEvent(player, "popup::openInput", "Передать предмет", "Введите количество", 3, "item_transfer_tofracstock");
        //                                        return;
        //                                    }

        //                                    string serial = (nInventory.WeaponsItems.Contains(item.Type)) ? $"({(string)item.Data})" : "";
        //                                    GameLog.Stock(Main.Players[player].Fraction.FractionID, Main.Players[player].UUID, $"{nInventory.ItemsNames[(int)item.Type]}{serial}", 1, false);
        //                                    client.Fractions.Utils.Stocks.Add(onFraction, item);
        //                                    nInventory.Remove(player, item);
        //                                    GameLog.Items($"player({Main.Players[player].UUID})", $"fracstock({onFraction})", Convert.ToInt32(item.Type), 1, $"{item.Data}");
        //                                    return;
        //                                }
        //                            case 7:
        //                                {
        //                                    nItem keyring = nInventory.Items[Main.Players[player].UUID][player.GetMyData<int>("KEYRING")];
        //                                    string keysData = Convert.ToString(keyring.Data);
        //                                    List<string> keys = (keysData.Length == 0) ? new List<string>() : new List<string>(keysData.Split('/'));
        //                                    if (keys.Count > 0 && string.IsNullOrEmpty(keys[keys.Count - 1]))
        //                                        keys.RemoveAt(keys.Count - 1);

        //                                    if (keys.Count >= 5)
        //                                    {
        //                                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Максимум 5 ключей", 3000);
        //                                        return;
        //                                    }

        //                                    if (item.Type != ItemType.CarKey)
        //                                    {
        //                                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Применимо только для ключей", 3000);
        //                                        return;
        //                                    }

        //                                    keys.Add(item.Data);
        //                                    keysData = "";
        //                                    foreach (string key in keys)
        //                                        keysData += $"{key}/";
        //                                    keyring.Data = keysData; // ¯\_(ツ)_/¯
        //                                    nInventory.Items[Main.Players[player].UUID][player.GetMyData<int>("KEYRING")] = keyring;

        //                                    nInventory.Remove(player, item);

        //                                    List<nItem> keyringItems = new List<nItem>();
        //                                    foreach (string key in keys)
        //                                        keyringItems.Add(new nItem(ItemType.CarKey, 1, key));

        //                                    player.SetMyData("KEYRING", nInventory.Items[Main.Players[player].UUID].IndexOf(keyring));
        //                                    OpenOut(player, keyringItems, "Связка ключей", 7);
        //                                    return;
        //                                }
        //                            case 8: // Оружейный сейф
        //                                {
        //                                    if (!nInventory.WeaponsItems.Contains(item.Type) && !nInventory.MeleeWeaponsItems.Contains(item.Type))
        //                                    {
        //                                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Оружейный сейф может хранить только оружие", 3000);
        //                                        return;
        //                                    }
        //                                    if (Main.Players[player].InsideHouseID == -1) return;
        //                                    int houseID = Main.Players[player].InsideHouseID;
        //                                    int furnID = player.GetMyData<int>("OpennedSafe");

        //                                    int tryAdd = Houses.FurnitureManager.TryAdd(houseID, furnID, item);
        //                                    if (tryAdd == -1)
        //                                    {
        //                                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Недостаточно места в инвентаре", 3000);
        //                                        return;
        //                                    }
        //                                    GameLog.Items($"player({Main.Players[player].UUID})", $"weapSafe({furnID} | house: {houseID})", Convert.ToInt32(item.Type), 1, $"{item.Data}");
        //                                    nInventory.Items[acc.UUID].Remove(item);
        //                                    sendItems(player);
        //                                    Houses.FurnitureManager.Add(houseID, furnID, new nItem(item.Type, 1, item.Data));
        //                                    return;
        //                                }
        //                        }
        //                        Items.onTransfer(player, item, null);
        //                        Close(player);
        //                        return;
        //                    }
        //                    break;
        //                }
        //            case 1:
        //                { // droped items
        //                  //TODO
        //                    break;
        //                }
        //            case 2:
        //                { // in car items
        //                    Vehicle veh = player.GetMyData<Vehicle>("SELECTEDVEH");
        //                    if (veh is null) return;
        //                    if (veh.Dimension != player.Dimension)
        //                    {
        //                        NeptuneEvo.Core.Commands.SendToAdmins(3, $"!{{#d35400}}[CAR-INVENTORY-EXPLOIT] {player.Name} ({player.Value}) dimension");
        //                        return;
        //                    }
        //                    if (veh.Position.DistanceTo(player.Position) > 10f)
        //                    {
        //                        NeptuneEvo.Core.Commands.SendToAdmins(3, $"!{{#d35400}}[CAR-INVENTORY-EXPLOIT] {player.Name} ({player.Value}) distance");
        //                        return;
        //                    }
        //                    items = veh.GetData<List<nItem>>("ITEMS");
        //                    item = items[index];

        //                    int tryAdd = nInventory.TryAdd(player, new nItem(item.Type));
        //                    if (tryAdd == -1)
        //                    {
        //                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Недостаточно места в инвентаре", 3000);
        //                        return;
        //                    }

        //                    if (item.Type == ItemType.BodyArmor && nInventory.Find(Main.Players[player].UUID, ItemType.BodyArmor) != null)
        //                    {
        //                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Недостаточно места в инвентаре", 3000); // ?
        //                        return;
        //                    }

        //                    if (item.Type == ItemType.BagWithDrill)
        //                    {
        //                        if (player.HasMyData("HEIST_DRILL") || player.HasMyData("HAND_MONEY"))
        //                        {
        //                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У Вас уже есть дрель или деньги в руках", 3000);
        //                            return;
        //                        }

        //                        player.SetClothes(5, 41, 0);
        //                        player.SetMyData("HEIST_DRILL", true);
        //                    }
        //                    else if (item.Type == ItemType.BagWithMoney)
        //                    {
        //                        if (player.HasMyData("HEIST_DRILL") || player.HasMyData("HAND_MONEY"))
        //                        {
        //                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У Вас уже есть сумка", 3000);
        //                            return;
        //                        }

        //                        player.SetClothes(5, 45, 0);
        //                        player.SetMyData("HAND_MONEY", true);
        //                    }

        //                    if (item.Count > 1)
        //                    {
        //                        Close(player);
        //                        player.SetMyData("ITEMTYPE", item.Type);
        //                        player.SetMyData("ITEMINDEX", index);
        //                        Trigger.ClientEvent(player, "popup::openInput", "Взять предмет", "Введите количество", 3, "item_transfer_fromveh");
        //                        return;
        //                    }

        //                    VehicleInventory.Remove(veh, item);
        //                    nInventory.Add(player, new nItem(item.Type, 1, item.Data));
        //                    GameLog.Items($"vehicle({veh.NumberPlate})", $"player({Main.Players[player].UUID})", Convert.ToInt32(item.Type), 1, $"{item.Data}");
        //                    break;
        //                }
        //            case 3: // Взять
        //                {
        //                    if (Main.Players[player].InsideHouseID == -1) return;
        //                    int houseID = Main.Players[player].InsideHouseID;
        //                    int furnID = player.GetMyData<int>("OpennedSafe");
        //                    Houses.HouseFurniture furniture = Houses.FurnitureManager.HouseFurnitures[houseID][furnID];
        //                    items = Houses.FurnitureManager.FurnituresItems[houseID][furnID];
        //                    item = items[index];

        //                    int tryAdd = nInventory.TryAdd(player, new nItem(item.Type));
        //                    if (tryAdd == -1)
        //                    {
        //                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Недостаточно места в инвентаре", 3000);
        //                        return;
        //                    }
        //                    if (item.Count > 1)
        //                    {
        //                        Close(player);
        //                        player.SetMyData("ITEMTYPE", item.Type);
        //                        player.SetMyData("ITEMINDEX", index);
        //                        Trigger.ClientEvent(player, "popup::openInput", "Взять предмет", "Введите количество", 3, "item_transfer_fromsafe");
        //                        return;
        //                    }
        //                    GameLog.Items($"itemSafe({furnID} | house: {houseID})", $"player({Main.Players[player].UUID})", Convert.ToInt32(item.Type), 1, $"{item.Data}");
        //                    items.RemoveAt(index);
        //                    Houses.FurnitureManager.FurnituresItems[houseID][furnID] = items;
        //                    nInventory.Add(player, new nItem(item.Type, 1, item.Data));
        //                    sendItems(player);
        //                    foreach (Player p in NAPI.Pools.GetAllPlayers())
        //                    {
        //                        if (p == null || !Main.Players.ContainsKey(p)) continue;
        //                        if ((p.HasMyData("OPENOUT_TYPE") && p.GetMyData<int>("OPENOUT_TYPE") == 3) && (Main.Players[p].InsideHouseID != -1 && Main.Players[p].InsideHouseID == houseID) && (p.HasMyData("OpennedSafe") && p.GetMyData<int>("OpennedSafe") == furnID))
        //                            GUI.Dashboard.OpenOut(p, items, furniture.Name, 3);
        //                    }
        //                    break;
        //                }
        //            case 4:
        //                {
        //                    if (Main.Players[player].InsideHouseID == -1) return;
        //                    int houseID = Main.Players[player].InsideHouseID;
        //                    int furnID = player.GetMyData<int>("OpennedSafe");
        //                    Houses.HouseFurniture furniture = Houses.FurnitureManager.HouseFurnitures[houseID][furnID];
        //                    items = Houses.FurnitureManager.FurnituresItems[houseID][furnID];
        //                    item = items[index];

        //                    int tryAdd = nInventory.TryAdd(player, new nItem(item.Type));
        //                    if (tryAdd == -1)
        //                    {
        //                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Недостаточно места в инвентаре", 3000);
        //                        return;
        //                    }

        //                    if (item.Type == ItemType.BodyArmor && nInventory.Find(Main.Players[player].UUID, ItemType.BodyArmor) != null)
        //                    {
        //                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Недостаточно места в инвентаре", 3000); // ?
        //                        return;
        //                    }
        //                    GameLog.Items($"clothSafe({furnID} | house: {houseID})", $"player({Main.Players[player].UUID})", Convert.ToInt32(item.Type), 1, $"{item.Data}");
        //                    nInventory.Items[Main.Players[player].UUID].Add(item);
        //                    sendItems(player);

        //                    items.RemoveAt(index);
        //                    Houses.FurnitureManager.FurnituresItems[houseID][furnID] = items;
        //                    foreach (Player p in NAPI.Pools.GetAllPlayers())
        //                    {
        //                        if (p == null || !Main.Players.ContainsKey(p)) continue;
        //                        if ((p.HasMyData("OPENOUT_TYPE") && p.GetMyData<int>("OPENOUT_TYPE") == 4) && (Main.Players[p].InsideHouseID != -1 && Main.Players[p].InsideHouseID == houseID) && (p.HasMyData("OpennedSafe") && p.GetMyData<int>("OpennedSafe") == furnID))
        //                            GUI.Dashboard.OpenOut(p, items, furniture.Name, 4);
        //                    }
        //                    break;
        //                }
        //            case 6:
        //                {
        //                    if (!player.HasMyData("ONFRACSTOCK") || player.GetMyData<int>("ONFRACSTOCK") == 0) return;
        //                    int onFrac = player.GetMyData<int>("ONFRACSTOCK");
        //                    items = client.Fractions.Utils.Stocks.fracStocks[onFrac].Weapons;
        //                    item = items[index];

        //                    int tryAdd = nInventory.TryAdd(player, new nItem(item.Type, 1));
        //                    if (tryAdd == -1)
        //                    {
        //                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Недостаточно места в инвентаре", 3000);
        //                        return;
        //                    }

        //                    if (item.Count > 1)
        //                    {
        //                        Close(player);
        //                        player.SetMyData("ITEMTYPE", item.Type);
        //                        player.SetMyData("ITEMINDEX", index);
        //                        Trigger.ClientEvent(player, "popup::openInput", "Взять предмет", "Введите количество", 3, "item_transfer_fromfracstock");
        //                        return;
        //                    }

        //                    nInventory.Add(player, item);
        //                    client.Fractions.Utils.Stocks.Remove(onFrac, item);
        //                    string serial = (nInventory.WeaponsItems.Contains(item.Type)) ? $"({(string)item.Data})" : "";
        //                    GameLog.Stock(Main.Players[player].Fraction.FractionID, Main.Players[player].UUID, $"{nInventory.ItemsNames[(int)item.Type]}{serial}", 1, true);
        //                    GameLog.Items($"fracstock({onFrac})", $"player({Main.Players[player].UUID})", Convert.ToInt32(item.Type), 1, $"{item.Data}");
        //                    break;
        //                }
        //            case 7:
        //                { // keyring items
        //                    nItem keyring = nInventory.Items[Main.Players[player].UUID][player.GetMyData<int>("KEYRING")];
        //                    string keysData = Convert.ToString(keyring.Data);
        //                    List<string> keys = (keysData.Length == 0) ? new List<string>() : new List<string>(keysData.Split('/'));
        //                    if (keys.Count > 0 && keys[keys.Count - 1] == "")
        //                        keys.RemoveAt(keys.Count - 1);

        //                    item = new nItem(ItemType.CarKey, 1, keys[index]);
        //                    int tryAdd = nInventory.TryAdd(player, item);
        //                    if (tryAdd == -1)
        //                    {
        //                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "У Вас недостаточно места", 3000);
        //                        return;
        //                    }

        //                    keys.RemoveAt(index);
        //                    nInventory.Add(player, new nItem(item.Type, 1, item.Data));

        //                    keysData = "";
        //                    foreach (string key in keys)
        //                        keysData += $"{key}/";
        //                    keyring.Data = keysData; // ¯\_(ツ)_/¯
        //                    nInventory.Items[Main.Players[player].UUID][player.GetMyData<int>("KEYRING")] = keyring;

        //                    List<nItem> keyringItems = new List<nItem>();
        //                    foreach (string key in keys)
        //                        keyringItems.Add(new nItem(ItemType.CarKey, 1, key));
        //                    OpenOut(player, keyringItems, "Связка ключей", 7);
        //                    break;
        //                }
        //            case 8: // Взять
        //                {
        //                    if (Main.Players[player].InsideHouseID == -1) return;
        //                    int houseID = Main.Players[player].InsideHouseID;
        //                    int furnID = player.GetMyData<int>("OpennedSafe");
        //                    Houses.HouseFurniture furniture = Houses.FurnitureManager.HouseFurnitures[houseID][furnID];
        //                    items = Houses.FurnitureManager.FurnituresItems[houseID][furnID];
        //                    item = items[index];

        //                    int tryAdd = nInventory.TryAdd(player, new nItem(item.Type));
        //                    if (tryAdd == -1)
        //                    {
        //                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Недостаточно места в инвентаре", 3000);
        //                        return;
        //                    }

        //                    if (item.Type == ItemType.BodyArmor && nInventory.Find(Main.Players[player].UUID, ItemType.BodyArmor) != null)
        //                    {
        //                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Недостаточно места в инвентаре", 3000); // ?
        //                        return;
        //                    }
        //                    GameLog.Items($"weapSafe({furnID} | house: {houseID})", $"player({Main.Players[player].UUID})", Convert.ToInt32(item.Type), 1, $"{item.Data}");
        //                    nInventory.Items[Main.Players[player].UUID].Add(item);
        //                    sendItems(player);

        //                    items.RemoveAt(index);
        //                    Houses.FurnitureManager.FurnituresItems[houseID][furnID] = items;
        //                    foreach (Player p in NAPI.Pools.GetAllPlayers())
        //                    {
        //                        if (p == null || !Main.Players.ContainsKey(p)) continue;
        //                        if ((p.HasMyData("OPENOUT_TYPE") && p.GetMyData<int>("OPENOUT_TYPE") == 8) && (Main.Players[p].InsideHouseID != -1 && Main.Players[p].InsideHouseID == houseID) && (p.HasMyData("OpennedSafe") && p.GetMyData<int>("OpennedSafe") == furnID))
        //                            GUI.Dashboard.OpenOut(p, items, furniture.Name, 8);
        //                    }
        //                    break;
        //                }
        //            case 20:
        //                {
        //                    if (Main.Players[player].AdminLVL >= 6 && Main.Players[player].InsideHouseID == -1)
        //                    {
        //                        if (!player.HasMyData("CHANGE_WITH"))
        //                        {
        //                            Close(player);
        //                            return;
        //                        }
        //                        Player target = player.GetMyData<Player>("CHANGE_WITH");
        //                        if (!Main.Players.ContainsKey(target))
        //                        {
        //                            Close(player);
        //                            return;
        //                        }
        //                        items = nInventory.Items[Main.Players[target].UUID];
        //                        item = items[index];
        //                        if (nInventory.ClothesItems.Contains(item.Type) && item.IsActive == true)
        //                        {
        //                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Игрок должен снять эту одежду", 3000);
        //                            return;
        //                        }
        //                        else if ((nInventory.WeaponsItems.Contains(item.Type) || nInventory.MeleeWeaponsItems.Contains(item.Type)) && item.IsActive == true)
        //                        {
        //                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Игрок должен убрать это оружие из рук", 3000);
        //                            return;
        //                        }
        //                        int tryAdd1 = nInventory.TryAdd(player, new nItem(item.Type, 1));
        //                        if (tryAdd1 == -1 || tryAdd1 > 0)
        //                        {
        //                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "У Вас недостаточно места", 3000);
        //                            return;
        //                        }
        //                        if (item.Type == ItemType.BodyArmor && nInventory.Find(Main.Players[player].UUID, ItemType.BodyArmor) != null)
        //                        {
        //                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Недостаточно места в инвентаре", 3000); // ?
        //                            return;
        //                        }
        //                        if (item.Count > 1)
        //                        {
        //                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Такие вещи нельзя забрать", 3000);
        //                            return;
        //                        }
        //                        nInventory.Add(player, item);
        //                        nInventory.Remove(target, item);
        //                        Close(player);
        //                        NeptuneEvo.Core.Commands.CMD_showPlayerStats(player, target.Value); // reopen target inventory
        //                        GameLog.Admin(player.Name, $"takeItem({item.Type} | {item.Data})", target.Name);
        //                        return;
        //                    }
        //                    break;
        //                }
        //        }
        //    }
        //    catch (Exception e) { Log.Write("Inventory: " + e.StackTrace, nLog.Type.Error); }
        //}

        [RemoteEvent("openInventory")]
        public void ClientEvent_openInventory(Player player, params object[] arguments)
        {
            try
            {
                if (isopen[player])
                {
                    Close(player);
                }
                else
                    Open(player);
            }
            catch (Exception e) { Log.Write("openInventory: " + e.StackTrace, nLog.Type.Error); }
        }

        [RemoteEvent("closeInventory")]
        public void ClientEvent_closeInventory(Player player, params object[] arguments)
        {
            try
            {
                if (player.HasMyData("OPENOUT_TYPE") && player.GetMyData<int>("OPENOUT_TYPE") == 20) sendItems(player);

                if (player.HasMyData("SELECTEDVEH"))
                {
                    Vehicle vehicle = player.GetMyData<Vehicle>("SELECTEDVEH");
                    vehicle.SetData("BAGINUSE", false);
                }

                player.ResetMyData("OPENOUT_TYPE");

                if (player.HasMyData("CHANGE_WITH") && Main.Players.ContainsKey(player.GetMyData<Player>("CHANGE_WITH")))
                {
                    if (player.HasMyData("CHANGE_WITH_ITEMS") && player.GetMyData<List<nItem>>("CHANGE_WITH_ITEMS").Count > 0 && (!player.HasMyData("Trade_Accept") || !player.GetMyData<Player>("CHANGE_WITH").HasMyData("Trade_Accept")))
                    {
                        Log.Debug("tut");
                        foreach (var item in player.GetMyData<List<nItem>>("CHANGE_WITH_ITEMS"))
                            nInventory.Add(player, item);
                    }
                    if (player.GetMyData<Player>("CHANGE_WITH").HasMyData("CHANGE_WITH"))
                        Close(player.GetMyData<Player>("CHANGE_WITH"));
                    player.ResetMyData("CHANGE_WITH");
                }
            }
            catch (Exception e) { Log.Write($"CloseInventory: " + e.StackTrace, nLog.Type.Error); }
        }

        public static void Close(Player player, bool resetOpenOut = false)
        {
            Trigger.ClientEvent(player, "inventory", 1);
            int data = (resetOpenOut) ? 11 : 1;
            Trigger.ClientEvent(player, "board", data);
            player.ResetMyData("OPENOUT_TYPE");
        }

        //[RemoteEvent("getPlayerStats")]
        //public static void getPlayerStats(Player player) {
        //    sendStats(player);

        //    Log.Write("getPlayerStats", nLog.Type.Info);

        //    Trigger.ClientEvent(player, "updateGamePlayerStats", JsonConvert.SerializeObject(gamePlayerData));
        //}

        public static void sendStats(Player player)
        {
            try
            {
                if (!Main.Players.ContainsKey(player)) return;
                Core.Character.Character acc = Main.Players[player];

                string status =
                    (acc.AdminLVL >= 1) ? "Администратор" :
                    (Main.Players[player].VipLvl > 0) ? $"{Group.GroupNames[Main.Players[player].VipLvl]} до {Main.Players[player].VipDate.ToString("dd.MM.yyyy")}" :
                    $"{Group.GroupNames[Main.Players[player].VipLvl]}";

                long bank = (acc.Bank != 0) ? Bank.Accounts[acc.Bank].Balance : 0;

                string lic = "";
                for (int i = 0; i < acc.Licenses.Count; i++)
                    if (acc.Licenses[i]) lic += $"{Main.LicWords[i]} / ";
                if (lic == "") lic = "Отсутствуют";

                //Log.Write("workID " + acc.WorkID, nLog.Type.Error);
                string work = (acc.WorkID > 0) ? Jobs.JobManager.JobListNames[acc.WorkID] : "Безработный";
                //Log.Write("work: " + work + " workID " + acc.WorkID, nLog.Type.Error);

                string fraction = (acc.Fraction.FractionID > 0) ? Fractions.Manager.FractionNames[acc.Fraction.FractionID] : "Нет";

                string number = (acc.Sim == -1) ? "Нет сим-карты" : Main.Players[player].Sim.ToString();

                var family = Family.GetFamilyToCid(acc.FamilyCID);

                var playerLVL = acc.LVL;
                var playerEXP = acc.EXP;
                var playerTotalEXP = 3 + acc.LVL * 3; // ? CHECK THIS ARTEM
                var playerPhoneNumber = number;
                var playerStatus = status;
                var playerWarns = acc.Warns;
                var playerLicenses = lic;
                var playerCreateData = acc.CreateDate.ToString("dd.MM.yyyy");
                var playerWorkName = work;
                var playerFraction = fraction;
                var playerFractionRang = acc.Fraction.FractionRankID;
                var playerName = acc.FirstName;
                var playerLastName = acc.LastName;
                var playerPassportNumber = acc.UUID;
                var playerBankNumber = acc.Bank;
                var playerEat = acc.Eat;
                var playerWater = acc.Water;


                var playerWheelTimer = "";
				var playerWheelCount = "";
				var playerMarriedStatus = "";
				var playerTotalTimePlaying = "";
				var playerWorkLVL = "";

                //var playerFamily = family.Name;
                //var playerFamilyLeader = family.Leader;
                //var playerFamilyRang = acc.FamilyRank;
                //var playerFamilyCID = acc.FamilyCID;


                List<object> data = new List<object>
                {
                    acc.LVL,
                    $"{acc.EXP}/{3 + acc.LVL * 3}", //1
                    number, //2
                    status, //3
                    acc.Warns,//4
                    lic,//5
                    acc.CreateDate.ToString("dd.MM.yyyy"),//6
                    work,//7
                    fraction,//8
                    acc.Fraction.FractionID,//9
                    acc.FirstName,//10
                    acc.LastName,//11
                    acc.UUID,//12
                    acc.Bank,//13
                    acc.Eat,//14
                    acc.Water//15
                };

                gamePlayerData = new
                {
                     playerLVL,
                     playerEXP,
                     playerTotalEXP,
                     playerPhoneNumber,
                     playerStatus,
                     playerWarns,
                     playerLicenses,
                     playerCreateData,
                     playerWorkName,
                     playerFraction,
                     playerFractionRang,
                     playerName,
                     playerLastName,
                     playerBankNumber,
                     playerEat,
                     playerWater,

                     playerWheelTimer,
                     playerWheelCount,
                     playerPassportNumber,
                     playerMarriedStatus,
                     playerTotalTimePlaying,
                     playerWorkLVL
                };

                string json = JsonConvert.SerializeObject(data);
                //Log.Debug("data is: " + json.ToString());

                //Log.Debug("gamePlayerData is: " + gamePlayerData.ToString());
                Trigger.ClientEvent(player, "board", 2, json);

                data.Clear();

            }
            catch (Exception e)
            {
                Log.Write("EXCEPTION AT \"DASHBOARD_SENDSTATS\":\n" + e.StackTrace, nLog.Type.Error);
            }
        }

        [RemoteEvent("SERVER::OPEN_STATS")]
        public static void SendStatInfo(Player player, int activeTab = 0)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            Dictionary<string, object> builderInfo = new Dictionary<string, object>();
            if (client.Jobs.Builder.BuilderManager.GetPlayerBuilderLevel(player) == 5)
            {
                builderInfo = new Dictionary<string, object>()
                {
                    {"name", "Строитель"},
                    {"level", client.Jobs.Builder.BuilderManager.GetPlayerBuilderLevel(player)},
                    {"currentExp", Main.Players[player].BuilderSmallContracts},
                    {"maxExp", BuilderManager.BuilderContractsMaxLevel}
                };
            }
            else
            {
                builderInfo = new Dictionary<string, object>()
                {
                    {"name", "Строитель"},
                    {"level", client.Jobs.Builder.BuilderManager.GetPlayerBuilderLevel(player)},
                    {"currentExp", Main.Players[player].BuilderExp},
                    {"maxExp", client.Jobs.Builder.BuilderManager.BuilderLevel.ContainsKey(client.Jobs.Builder.BuilderManager.GetPlayerBuilderLevel(player) + 1) ? client.Jobs.Builder.BuilderManager.BuilderLevel[client.Jobs.Builder.BuilderManager.GetPlayerBuilderLevel(player) + 1] : 9999}
                };
            }

            dict.Add("show", true);
            dict.Add("activeTab", activeTab);
            dict.Add("chat", false);
            dict.Add("player", new Dictionary<string, object>()
            {
                {"name", player.Name },
                {"vip", Main.Players[player].VipLvl == 0 ? "VIP Статус не активен" : $"{Group.GroupNames[Main.Players[player].VipLvl]} до {Main.Players[player].VipDate.ToString("dd.MM.yyyy")}" },
                {"hungry", Main.Players[player].Eat },
                {"drink", Main.Players[player].Water },
                {"lvl", Main.Players[player].LVL },
                {"maxExp", Main.Players[player].LVL <= 15 ? Main.Players[player].LVL * 3 : 15 * 3 + 2 * (Main.Players[player].LVL - 15)},
                {"exp", Main.Players[player].EXP },
                {"cash", Main.Players[player].Money },
                {"bank", Bank.Accounts[Main.Players[player].Bank].Balance },
                {"lic", new Dictionary<string, object>()
                    {
                        {"a", Main.Players[player].Licenses[0] },
                        {"b", Main.Players[player].Licenses[1] },
                        {"c", Main.Players[player].Licenses[2] },
                        {"water", Main.Players[player].Licenses[3] },
                        {"air", Main.Players[player].Licenses[4] },
                        {"gun", Main.Players[player].Licenses[6] },
                        {"med", Main.Players[player].Licenses[7] },
                        {"fishing", Main.Players[player].Licenses[9] },
                        {"voen", Main.Players[player].Licenses[10] },
                        {"deepFishing", Main.Players[player].Licenses[11] },
                    }
                },
                {"fractionName", Main.Players[player].Fraction.FractionID <= 0 ? "Отсутствует" : Fractions.Manager.getName(Main.Players[player].Fraction.FractionID)},
                {"workName", Jobs.JobManager.JobListNames.ContainsKey(Main.Players[player].WorkID) ? Jobs.JobManager.JobListNames[Main.Players[player].WorkID] : "Отсутствует"},
                {"skills", new Dictionary<string, object>()
                    {
                        {"fishing", new Dictionary<string, object>()
                          {
                            {"name", "Рыбалка"},
                            {"level", RodManager.GetPlayerRodLevel(player)},
                            {"currentExp", Main.Players[player].RodExp},
                            {"maxExp", RodManager.RodLevel[RodManager.GetPlayerRodLevel(player) - 1]}
                          }
                        },
                        {"builder", builderInfo},
                        {"garbage", new Dictionary<string, object>()
                          {
                            {"name", "Мусорщик"},
                            {"level", Jobs.GarbageTruck.GetPlayerGarbageLevel(Main.Players[player].GarbageBags)},
                            {"currentExp", Main.Players[player].GarbageBags},
                            {"maxExp", Jobs.GarbageTruck.GarbageLevel.ContainsKey(Jobs.GarbageTruck.GetPlayerGarbageLevel(Main.Players[player].GarbageBags) + 1) ? Jobs.GarbageTruck.GarbageLevel[Jobs.GarbageTruck.GetPlayerGarbageLevel(Main.Players[player].GarbageBags) + 1] : 9999}
                          }
                        },
                        {"trucker", new Dictionary<string, object>()
                          {
                            {"name", "Дальнобойщик"},
                            {"level", Jobs.Truckers.GetTruckerLevel(player)},
                            {"currentExp", Main.Players[player].TruckerExp},
                            {"maxExp", Jobs.Truckers.TruckerLevel.ContainsKey(Jobs.Truckers.GetTruckerLevel(player) + 1) ? Jobs.Truckers.TruckerLevel[Jobs.Truckers.GetTruckerLevel(player) + 1] : 9999}
                          }
                        },
                        {"collector", new Dictionary<string, object>()
                          {
                            {"name", "Инкассатор"},
                            {"level", Jobs.Collector.GetCollectorLevel(player)},
                            {"currentExp", Main.Players[player].CollectorExp},
                            {"maxExp", Jobs.Collector.CollectorLevel.ContainsKey(Jobs.Collector.GetCollectorLevel(player) + 1) ? Jobs.Collector.CollectorLevel[Jobs.Collector.GetCollectorLevel(player) + 1] : 99999}
                          }
                        },
                    }
                },
            });

            dict.Add("vipstatus", GetVipStatus(player));

            //Log.Write($"OPEN: {JsonConvert.SerializeObject(dict)}");
            Trigger.ClientEvent(player, "CLIENT::STATS:OPEN", JsonConvert.SerializeObject(dict));
        }

        public static List<Dictionary<string, object>> GetSereliazeGeneralTab()
        {
            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>()
            {
              new Dictionary<string, object>()
                    {
                        {"title", "Вступление" },
                        {"text", "Уважаемый игрок, данный раздел меню статистики создан для того, чтобы ознакомить Вас с нашим штатом. Здесь вы можете найти краткое описание функционала, ознакомиться с азами РП процесса и ответить на интересующие Вас вопросы касаемо тех или иных работ." }
                    },
                    new Dictionary<string, object>()
                    {
                        {"title", "Потребности" },
                        {"text", "После создания персонажа каждый из вас окунается в увлекательный мир РП, но не стоит забывать, что тут, как и в реальной жизни, присутствуют физиологические потребности. Если за ними не следить, то можно потерять внушительное количество здоровья и впоследствии оказаться в больнице."+
                            "Для поддержания уровня Жажды и Голода можно использовать разную еду и напитки, которые вы можете приобрести в магазине 24/7."+
                            "Будьте внимательны! Разные блюда пополняют шкалу голода и жажды по-разному! Читайте описание перед тем как приобрести тот или иной предмет."}
                    },
                    new Dictionary<string, object>()
                    {
                        {"title", "Магазин 24/7 и прочие магазины" },
                        {"text", "В разделе “Потребности” Вы прочитали о том, что еду и напитки можно приобрести в магазине 24/7, но что это за место и какие магазины ещё есть на сервере?"+
                            "Магазин 24/7, рыболовный магазин и Аммунация призваны помочь игроку в приобретении предметов первой необходимости, оружия и модификаций для него, рыболовных снастей и еды."+
                            "Для  поиска ближайшего магазина Вы можете воспользоваться картой или же поставить GPS метку в своем телефоне." }
                    },
                    new Dictionary<string, object>()
                    {
                        {"title", "Телефон" },
                        {"text", "Телефон – универсальное средство для игрока. Благодаря ему вы можете совершать звонки, писать сообщения своим товарищам и выкладывать объявления через соответствующие вкладки."+
                            "Если Вы заблудились или же не можете найти на карте нужный объект, то советуем Вам воспользоваться вкладкой “GPS”. Он поможет проложить маршрут до любого интересующего Вас места."+
                            "В случае, если Ваша машина далеко и арендовать поблизости скутер или любой другой транспорт не представляется возможным, Вы можете воспользоваться услугами “Такси”. Для этого зайдите в соответствующие приложение и нажмите кнопку “Заказать такси”. После этого Вам нужно всего лишь ожидать водителя не покидая зону вызова."+
                            "Для счастливых обладателей автомобиля есть вкладка “MyCar”. С её помощью Вы можете эвакуировать свой автомобиль и отобразить его меткой на карте. В случае, если автомобиль арендован, Вы с легкостью можете закончить его аренду через это приложение."+
                            "И на сладкое – Forbes. Это список самых состоятельных игроков в котором рано или поздно можете оказаться и Вы!" }
                    },
                    new Dictionary<string, object>()
                    {
                        {"title", "Аренда транспорта" },
                        {"text", "Удобная функция, позволяющая арендовать транспорт в разных точках нашего штата. Будет полезна для новичков и для тех игроков, чей транспорт находится вне пешей доступности."+
                            "Ищите на карте красные машинки и скутеры, именно там Вы сможете арендовать для себя транспорт!" }
                    },
                    new Dictionary<string, object>()
                    {
                        {"title", "Покупка автомобиля" },
                        {"text", "Ваш бюджет позволяет владеть личным автомобилем? Отлично, значит нужно отправиться в один из автосалонов, расположенных в нашем штате и ознакомиться с ассортиментом."+
                            "Автосалоны делятся на несколько типов, так что игрок любого достатка сможет спокойно присмотреть себе автомобиль по душе."+
                            "Для приобретения своей первой машины найдите подходящий для Вашего бюджета автосалон -> выберите модель и её цвет -> нажимайте кнопку “купить”."+
                            "Готово! Поздравляем с приобретением первого автомобиля. Не забывайте соблюдать правила ПДД нашего штата."+
                            "Важно! После покупки автомобиль необходимо зарегистрировать и поставить на него номера. Сделать это можно самостоятельно у главного офиса LSPD (ищите соответствующий значок на карте)." }
                    },
                    new Dictionary<string, object>()
                    {
                        {"title", "Парковка" },
                        {"text", "Приобрели автомобиль, но поставить его некуда? С этим вам помогу парковки."+
                            "В любом уголке штата есть парковочные зоны (зелёная “P” на карте) и в каждой из зон есть некоторое количество доступных парковочных мест."+
                            "Максимальное время аренды парковки – 24 часа. Все это время Вы можете использовать парковочное место для эвакуации своего транспорта"}
                    },
                    new Dictionary<string, object>()
                    {
                        {"title", "Риелторское агенство" },
                        {"text", "Задумываетесь над покупкой собственного жилья? Тогда риелторское агенство поможет сделать первый шаг в этом вопросе!"+
                            "Оно расположено неподалёку от автошколы (зелёный домик со значком домика)."+
                            "Для ознакомления с ассортиментом подойдите к любому из агентов (они сидят в кабинетах), нажмите на кнопку взаимодействия и выберите тот класс дома, который вы сможете себе позволить исходя их собственного бюджета."+
                            "После агент предложит вам купить информацию о том доме, который вас заинтересовал. В информации указана стоимость интересующего дома, его адрес, сумма ежедневного налога и количество гаражных мест. Если дом понравился, устраивает его расположение и прочие пункты, то нажимайте на кнопку “Поставить метку” и можете смело ехать покупать своё первое жилье."+
                            "Важно! После покупки дома не забывайте оплачивать налог в банкомате, в противном случае дом снова отправиться государству, а Вы получите лишь половину от его первоначальной стоимости."}
                    },
                    new Dictionary<string, object>()
                    {
                        {"title", "Бизнес" },
                        {"text", "Автомобиль и своя недвижимость – столпы успешного человека, но есть ещё один, который помогает приумножить успех – это бизнес!"+
                            "При успешном управлении Вы можете заработать куда больше, чем на обычной работе или подработке, а попробовать можно себя как в роли управляющего магазином 24/7, так и директором Los Santos Customs."+
                            "Помните, на проекте можно иметь не более одного бизнеса!" },
                    },
                    new Dictionary<string, object>()
                    {
                        {"title", "Казино" },
                        {"text", "Если Вы устали от маленьких зарплат или же хотите отдохнуть после тяжелого рабочего дня, то двери нашего казино всегда для вас открыты!"+
                            "На выбор есть несколько видов азартных игр: слоты, блекджек, рулетка и Inside Track(если вы захотели посетить казино с друзьями)."+
                            "Ежедневно Вам доступно колесо удачи, которое расположено в главном зале.В качестве бонуса Вы можете получить пару тысяч долларов, опыт, одежду или главный приз – автомобиль!"+
                            "Попытайте удачу вместе с главным казино нашего штата." },
                    },
                    new Dictionary<string, object>()
                    {
                        {"title", "Рыбалка" },
                        {"text", "Рыба – ваше признание? Устали от вечной городской суеты? Берите удочку и в бой!"+
                            "В нашем штате есть множество мест, где Вы можете поймать рыбу совершенно разного калибра. Для начала необходимо купить удочку в рыболовном магазине (там же, к слову, можно найти наживку) и Вы готовы покорять океан!"+
                            "Отправляйтесь на один из 8 пирсов, расположенных на карте (значок голубого крючка) и начините увлекательный процесс ловли рыбы бок о бок с другими жителями нашего штата."+
                            "Важно! Для более детального ознакомления с механическими процессами рыбалки посетите форум штата и ознакомьтесь с более подробной информацией." },
                    }
            };

            return list;
        }

        public static List<Dictionary<string, object>> GetSereliazeJobsTab()
        {
            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>()
            {
                new Dictionary<string, object>()
                {
                    {"title", "Строитель" },
                    {"text", "Работа строителем доступна с первого уровня. " +
                        "Сама работа довольно-таки обширна и захватывает много областей: от обычного отделочного ремонта до создания новых домой в штате! " +
                        "Если вы устали возить полеты на погрузчике или стучать молотком, то можете стать бригадиром, собрать свою команду и начать выполнять контракты. " +
                        "А если вы чувствуете, что хотите расти и развиваться ещё больше, то на последнем уровне работы Вам будут доступны крупные контракты. Помимо заработка это приносит и пользу штату, т.к. после выполнения одного такого контракта появляется один новый дом." },
                },
                new Dictionary<string, object>()
                {
                    {"title", "Почтальон" },
                        {"text", "Работа почтальоном доступна со второго уровня."+
                        "Суть работы заключается в развозке писем и мелких посылок."+
                        "Работать можно как пешком, так и на личном или арендованном транспорте."+
                        "Заработок зависит напрямую от скорости выполнения заказов."+
                        "Пример: доставляя почтовые посылки на обычном скутере Вы будете получать заведомо меньше денег, чем на арендованном автомобиле(в силу того, что скорость скутера заведомо ниже)."+
                        "Помните! На эту работу необходимо устраиваться в мэрии." },
                },
                new Dictionary<string, object>()
                {
                    {"title", "Таксист" },
                    {"text", "Работа таксистом доступна со второго уровня."+
                        "Суть работы проста как 2+2, всего лишь нужно арендовать автомобиль и начать принимать заказы в рабочем меню (Кнопка “M” – Рабочее меню)."+
                        "После принятия заказа отправляйтесь к пассажиру, примите от него метку и выставите цену, которую считаете справедливой для того расстояния, что указал пассажир."+
                        "Помните! На эту работу необходимо устраиваться в мэрии." },
                },
                new Dictionary<string, object>()
                {
                    {"title", "Водитель автобуса" },
                    {"text", "Работа водителем автобуса доступна с третьего уровня. " +
                            "Все предельно просто – садитесь за руль автобуса, арендуйте его и можете сколько угодно наматывать круги вокруг нашего штата." +
                            "Всего есть 7 маршрутов, которые случайным образом распределяются между игроками, самый короткий из них занимает около 7 минут на прохождение, а самый длинный – около 18 минут." +
                            "Помните! На эту работу необходимо устраиваться в мэрии." },
                },
                new Dictionary<string, object>()
                {
                    {"title", "Мусорщик" },
                    {"text", "Работа мусорщиком доступна с третьего уровня." +
                            "Довольно-таки сложная и интересная система, которая доступна абсолютно любому игроку, который любит просчитывать стратегию своего заработка, разбираться с навыками персонажа и приятно проводить время в компании своих друзей." +
                            "За простым забором мусора скрывается нечто большее – возможность скооперироваться в команду со своими друзьями, найти лучшую комбинацию навыков и сделать свою работу максимально выгодной и многое другое." +
                            "Более подробное описание ищи на форуме нашего штата!" +
                            "Требуется категория С." +
                            "Помните! На эту работу необходимо устраиваться в мэрии." },
                },
                new Dictionary<string, object>()
                {
                    {"title", "Механик" },
                    {"text", "Работа механиком доступна с четвертого уровня." +
                            "Вы были рождены для починки и заправки автомобилей? Тогда эта работа определенно для Вас!" +
                            "Уровень заработка зависит исключительно от Вашей скорости, реакции и предприимчивости." +
                            "Увидели игрока, у которого закончился бензин или разбита машина? Ненавязчиво предложите ему свои услуги!" +
                            "Помимо патрулирования вы можете принимать заказы от игроков в рабочем меню (Кнопка “M” – Рабочее меню)." +
                            "Помните! На эту работу необходимо устраиваться в мэрии." },
                },
                new Dictionary<string, object>()
                {
                    {"title", "Инкассатор" },
                    {"text", "Работа инкассатором доступна с пятого уровня.Ответственное и весьма опасное занятие, которое подойдет для опытного игрока. " +
                        "Суть работы заключается в том, что вам нужно развозить деньги по различным банкоматам нашего штата, но её опасность в том, что в определенных зонах Вас могут ограбить." +
                        "Будьте внимательны и ознакомьтесь с зонами, в которых на вас может быть совершено нападение." +
                        "Требуется категория С." +
                        "Помните! На эту работу необходимо устраиваться в мэрии." },
                },
                new Dictionary<string, object>()
                {
                    {"title", "Дальнобойщик" },
                    {"text", "Работа дальнобойщиком доступна с восьмого уровня." +
                        "Если длинные расстояния в Вашей крови, а управлять крупногабаритным транспортом – мечта с детства, то эта работа идеально подойдет для Вас." +
                        "Изначально она делиться на несколько уровней:" +
                        "На первом доступна развозка государственных грузов." +
                        "На втором Вы можете брать заказы и более быструю машину." +
                        "На третьем – использовать личный транспорт из грузового автосалона для развозки грузов." +
                        "И на последнем доступны заказы на доставку автомобилей и автозапчастей (самые прибыльные из всех)" +
                        "Требуется категория С." +
                        "Помните! На эту работу необходимо устраиваться в мэрии." },
                },
            };

            return list;
        }

        public static List<Dictionary<string, object>> GetSereliazeFractionTab()
        {
            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>()
            {
                new Dictionary<string, object>()
                {
                    {"title", "Грув" },
                    {"text", "Баскетбольный клуб. Одна из пяти уличных криминальных организаций, орудующая на трассах и улицах нашего штата. " +
                    "Их главное отличие – зелёная форма, которую нужно носить абсолютно всем членам данной группировки." },
                },
                new Dictionary<string, object>()
                {
                    {"title", "Баллас" },
                    {"text", "Помните! Все преступные организации скрываются от государственных деятелей, потому не называют себя мафиями и бандами. " +
                    "Внимательно читайте предложения о вступлении в ряды “виноделен” или “пиццерий” и НИКОГДА не говорите " +
                    "что хотите вступить преступную организацию на собеседовании, иначе получите моментальный отказ." },
                },
                new Dictionary<string, object>()
                {
                    {"title", "Вагос" },
                    {"text", "Автомастерская. Помните! Все преступные организации скрываются от государственных деятелей, " +
                    "потому не называют себя мафиями и бандами. Внимательно читайте предложения о вступлении в ряды “виноделен” или “пиццерий” и" +
                    " НИКОГДА не говорите что хотите вступить преступную организацию на собеседовании, иначе получите моментальный отказ." },
                },
                new Dictionary<string, object>()
                {
                    {"title", "Марабунта" },
                    {"text", "Помните! Все преступные организации скрываются от государственных деятелей, потому не называют себя мафиями и бандами. " +
                    "Внимательно читайте предложения о вступлении в ряды “виноделен” или “пиццерий” и " +
                    "НИКОГДА не говорите что хотите вступить преступную организацию на собеседовании, иначе получите моментальный отказ." },
                },
                new Dictionary<string, object>()
                {
                    {"title", "Блуд" },
                    {"text", "Боксерский клуб. Помните! Все преступные организации скрываются от государственных деятелей, потому не называют себя мафиями и бандами." +
                    " Внимательно читайте предложения о вступлении в ряды “виноделен” или “пиццерий” и " +
                    "НИКОГДА не говорите что хотите вступить преступную организацию на собеседовании, иначе получите моментальный отказ." },
                },
                new Dictionary<string, object>()
                {
                    {"title", "Мэрия" },
                    {"text", "Сердце штата и цитадель всей жизни. " +
                    "Хотите стать высококлассным адвокатом? Вам в мэрию." +
                    "Хотите обеспечивать все логистические процессы штата или курировать те или иные фракции? Отправляйтесь в мэрию!" +
                    "Огромный список вакансий, которые подойдут как для новичков, так и для игроков, искушенных и опытных." +
                    "Приходите на собеседования в часы, объявленные в новостях, или оставляйте свои заявки на форуме штата." +
                    "Как знать, может быть именно вы станете нашим следующим мэром…" },
                },
                new Dictionary<string, object>()
                {
                    {"title", "LSPD" },
                    {"text", "Блюстители закона, которые всегда готовы приехать к вам на помощь где бы вы не находились. " +
                    "Увидели нарушение или стали жертвой ограбления? Смело обращайтесь к сотрудникам LSPD и они вам помогут!" +
                    "В случае, если вы решились пойти на столь ответственную работу – обращайтесь к отделу по подбору персонала в рабочие часы" +
                    " (их, как и набор, оглашают в новостном канале нашего штата)." },
                },
                new Dictionary<string, object>()
                {
                    {"title", "EMS" },
                    {"text", "Спасать людей – призвание и не каждый справиться со столь ответственной задачей." +
                    "В нашем штате представлена обширная сеть EMS и две больницы (одна в городе, вторая – в Сэнди-Шорс. Кареты скорой помощи стоят наготове и в любой момент могут помочь гражданину, чей организм дал сбой." +
                    "" },
                },
                new Dictionary<string, object>()
                {
                    {"title", "FIB" },
                    {"text", "Одна из самых секретных и сложных для новичков фракций." +
                    "FIB представляет из себя секретное подразделение, занимающееся самыми важными стратегическими задачами (от планирования разного рода операций до сопровождения и защиты мэра)." +
                    "Вступление в FIB требует идеального знания буквы закона, умения конспирироваться и оказываться именно там, где того требуют обстоятельства." +
                    "Набор в данную организацию проходит относительно нечасто, поэтому следите за новостями или оставьте заявку на Форуме." },
                },
                new Dictionary<string, object>()
                {
                    {"title", "La Cosa Nostra" },
                    {"text", "Опера, ароматные виноградники и знойные летние дни – предметы гордости любого итальянца, но как же этого не хватает на другом континенте…" +
                    "Италия по праву считается родиной мафии и кодекса чести, который эти люди чтят и помнят по сей день. Они готовы изо дня в день бороться за своё величие и доказывать, что кроме Европы они способны захватить ещё один континент – Северную Америку. Помните! Все преступные организации скрываются от государственных деятелей, потому не называют себя мафиями и бандами." +
                    " Внимательно читайте предложения о вступлении в ряды “виноделен” или “пиццерий” и НИКОГДА не говорите что хотите вступить преступную организацию на собеседовании, иначе получите моментальный отказ." },
                },
                new Dictionary<string, object>()
                {
                    {"title", "Русская мафия" },
                    {"text", "Мерседесы, золото, строгие черные костюмы… Что ещё нужно выходцам из стран бывшего СССР в Америке? Только достаток, благополучие и большие доходы с преступных действий." +
                    "Представители русской мафии очень аккуратны, солидны и не любят разбрасываться словами по чём зря." +
                    "Будьте аккуратны и старайтесь не переходить им дорогу, а если хотите заручиться их поддержкой, то вступайте в их ряды." +
                    "Помните! Все преступные организации скрываются от государственных деятелей, потому не называют себя мафиями и бандами. Внимательно читайте предложения о вступлении в ряды “виноделен” или “пиццерий” и НИКОГДА не говорите что хотите вступить преступную организацию на собеседовании, иначе получите моментальный отказ." },
                },
                new Dictionary<string, object>()
                {
                    {"title", "Якудза" },
                    {"text", "В противостоянии западного и восточного мира никогда не было победителя и вряд ли будет, но…. То количество жертв, которое приносит это противостояние являет собой ужасные цифры. В случае с людьми из Якудза это оправдано самоотверженностью её представителей." +
                    "Эти люди чтят свои нравы и законы, привыкли работать тихо, но от того не менее жестоко и эффективно. Вполне вероятно, что война с другими мафиями будет длиться бесконечно, но нельзя недооценивать выходцев из Японии…" +
                    "Если вы мигрировали в США из страны восходящего солнца, то можете смело примкнуть к их рядам и добиваться величия бок-о-бок со своими уроженцами." +
                    "Помните! Все преступные организации скрываются от государственных деятелей, потому не называют себя мафиями и бандами. Внимательно читайте предложения о вступлении в ряды “виноделен” или “пиццерий” и НИКОГДА не говорите что хотите вступить преступную организацию на собеседовании, иначе получите моментальный отказ." },
                },
                new Dictionary<string, object>()
                {
                    {"title", "Чеченская мафия" },
                    {"text", "С одной стороны, русская мафия – их главный союзник, а с другой – все их враги." +
                    "Беспощадные и очень серьезные люди, состоящие в рядах Чеченской мафии, готовы идти по головам ради достижения величия и наведения своих порядков во всем штате." +
                    "Не нужно их недооценивать, несмотря на их немногочисленные ряды, эти люди максимально самоотверженны." +
                    "Если вы думаете, что ваши стремления схожи – можете вступить к ним и стать частью огромной преступной системы." +
                    "Помните! Все преступные организации скрываются от государственных деятелей, потому не называют себя мафиями и бандами. Внимательно читайте предложения о вступлении в ряды “виноделен” или “пиццерий” и НИКОГДА не говорите что хотите вступить преступную организацию на собеседовании, иначе получите моментальный отказ." },
                },
                new Dictionary<string, object>()
                {
                    {"title", "Армия" },
                    {"text", "Если вы мечтаете о полицейской карьере или о выполнении важнейших стратегических задач в нашем штате, то армия будет идеальным выбором." +
                    "После обычной службы и получения военного билета для вас откроется совершенно новый и очень интересный мир." +
                    "Конвоирования первых лиц штата, организация перевозок материалов, управление самолетами и организация военных фестивалей – все это лишь малая часть того, что вас ждет!" +
                    "Если же вам интересна лишь военная карьера, то вы можете дослужиться до высших чинов и активно участвовать во всей жизни штата." +
                    "Записывайтесь в часы, когда армия проводит набор или оставляйте свои заявки на форуме штата." +
                    "Примечание. Военный билет нужен для продвижения по службе в LSPD, BCSD и FIB." },
                },
                new Dictionary<string, object>()
                {
                    {"title", "News" },
                    {"text", "Хотите попробовать себя в роли организатора мероприятий или же всегда мечтали о креативной должности? News – идеальный вариант для вступления." +
                    "Помимо модерации новостей и монотонной работы на стартовом этапе есть и куда более креативные должности (начиная от организатора мероприятий - заканчивая режиссером и оператором)." +
                    "Подавайте свою заявку в любое время, когда ведётся набор или же оставляйте ее на форуме штата." },
                },
                new Dictionary<string, object>()
                {
                    {"title", "BCSD" },
                    {"text", "Данная фракция отлично подойдет для любителей вестернов 60-ых, преданных своему делу полицейских и для тех, кто устал от городской суеты." +
                    "BCSD – фракция, расположенная на севере штата, в городке Палето-Бей." +
                    "Зона ответственности работников – северный округ штата, а также трассы и леса, доступ к которым участники LSPD не имеют." +
                    "Если вы идеально владеете транспортом, знаете каждую кочку горы Чилиад и уверены в своих силах, то быстрее вступайте и попробуйте стать шерифом!" +
                    "Заявки принимают как в часы набора, так и на форуме штата. Будьте внимательны и не пропускайте новости о наборе!" },
                },
            };

            return list;
        }

        public static List<object> GetSerealizeHelpTitle()
        {
            List<object> list = new List<object>() { "Общие", "Работы и подработки", "Фракции" };

            return list;
        }

        [RemoteEvent("SERVER::STATS:CHANGE_TAB")]
        public static void RemoteEvent_ChangeStatsTab(Player player, int id)
        {
          if (player == null || !Main.Players.ContainsKey(player)) return;

          Dictionary<string, object> dict = new Dictionary<string, object>();

          //"Личные",
          //"Имущество",
          //"Задания",
          //"Достижения",
          //"Магазин",
          //"Настройки",
          //"Помощь",
          //"Награды",
          //"Репорты",
          //"VIP Status",

          switch(id)
          {
              case 0: // Личные
                { 
                    Dictionary<string, object> builderInfo = new Dictionary<string, object>();
                    if (client.Jobs.Builder.BuilderManager.GetPlayerBuilderLevel(player) == 5)
                    {
                        builderInfo = new Dictionary<string, object>()
                        {
                            {"name", "Строитель"},
                            {"level", client.Jobs.Builder.BuilderManager.GetPlayerBuilderLevel(player)},
                            {"currentExp", Main.Players[player].BuilderSmallContracts},
                            {"maxExp", BuilderManager.BuilderContractsMaxLevel}
                        };
                    }
                    else
                    {
                        builderInfo = new Dictionary<string, object>()
                        {
                            {"name", "Строитель"},
                            {"level", client.Jobs.Builder.BuilderManager.GetPlayerBuilderLevel(player)},
                            {"currentExp", Main.Players[player].BuilderExp},
                            {"maxExp", client.Jobs.Builder.BuilderManager.BuilderLevel.ContainsKey(client.Jobs.Builder.BuilderManager.GetPlayerBuilderLevel(player) + 1) ? client.Jobs.Builder.BuilderManager.BuilderLevel[client.Jobs.Builder.BuilderManager.GetPlayerBuilderLevel(player) + 1] : 9999}
                        };
                    }

                    dict.Add("player", new Dictionary<string, object>()
                    {
                        {"name", player.Name },
                        {"vip", Main.Players[player].VipLvl == 0 ? "VIP Статус не активен" : $"{Group.GroupNames[Main.Players[player].VipLvl]} до {Main.Players[player].VipDate.ToString("dd.MM.yyyy")}" },
                        {"hungry", Main.Players[player].Eat },
                        {"drink", Main.Players[player].Water },
                        {"lvl", Main.Players[player].LVL },
                        {"maxExp", Main.Players[player].LVL <= 15 ? Main.Players[player].LVL * 3 : 15 * 3 + 2 * (Main.Players[player].LVL - 15)},
                        {"exp", Main.Players[player].EXP },
                        {"cash", Main.Players[player].Money },
                        {"bank", Bank.Accounts[Main.Players[player].Bank].Balance },
                        {"lic", new Dictionary<string, object>()
                            {
                                {"a", Main.Players[player].Licenses[0] },
                                {"b", Main.Players[player].Licenses[1] },
                                {"c", Main.Players[player].Licenses[2] },
                                {"water", Main.Players[player].Licenses[3] },
                                {"air", Main.Players[player].Licenses[4] },
                                {"gun", Main.Players[player].Licenses[6] },
                                {"med", Main.Players[player].Licenses[7] },
                                {"fishing", Main.Players[player].Licenses[9] },
                                {"voen", Main.Players[player].Licenses[10] },
                                {"deepFishing", Main.Players[player].Licenses[11] },
                            }
                        },
                        {"fractionName", Main.Players[player].Fraction.FractionID <= 0 ? "Отсутствует" : Fractions.Manager.getName(Main.Players[player].Fraction.FractionID)},
                        {"workName", Jobs.JobManager.JobListNames.ContainsKey(Main.Players[player].WorkID) ? Jobs.JobManager.JobListNames[Main.Players[player].WorkID] : "Отсутствует"},
                        {"skills", new Dictionary<string, object>()
                            {
                                {"fishing", new Dictionary<string, object>()
                                  {
                                    {"name", "Рыбалка"},
                                    {"level", RodManager.GetPlayerRodLevel(player)},
                                    {"currentExp", Main.Players[player].RodExp},
                                    {"maxExp", RodManager.RodLevel[RodManager.GetPlayerRodLevel(player) - 1]}
                                  }
                                },
                                {"builder", builderInfo},
                                {"garbage", new Dictionary<string, object>()
                                  {
                                    {"name", "Мусорщик"},
                                    {"level", Jobs.GarbageTruck.GetPlayerGarbageLevel(Main.Players[player].GarbageBags)},
                                    {"currentExp", Main.Players[player].GarbageBags},
                                    {"maxExp", Jobs.GarbageTruck.GarbageLevel.ContainsKey(Jobs.GarbageTruck.GetPlayerGarbageLevel(Main.Players[player].GarbageBags) + 1) ? Jobs.GarbageTruck.GarbageLevel[Jobs.GarbageTruck.GetPlayerGarbageLevel(Main.Players[player].GarbageBags) + 1] : 9999}
                                  }
                                },
                                {"trucker", new Dictionary<string, object>()
                                  {
                                    {"name", "Дальнобойщик"},
                                    {"level", Jobs.Truckers.GetTruckerLevel(player)},
                                    {"currentExp", Main.Players[player].TruckerExp},
                                    {"maxExp", Jobs.Truckers.TruckerLevel.ContainsKey(Jobs.Truckers.GetTruckerLevel(player) + 1) ? Jobs.Truckers.TruckerLevel[Jobs.Truckers.GetTruckerLevel(player) + 1] : 9999}
                                  }
                                },
                                {"collector", new Dictionary<string, object>()
                                  {
                                    {"name", "Инкассатор"},
                                    {"level", Jobs.Collector.GetCollectorLevel(player)},
                                    {"currentExp", Main.Players[player].CollectorExp},
                                    {"maxExp", Jobs.Collector.CollectorLevel.ContainsKey(Jobs.Collector.GetCollectorLevel(player) + 1) ? Jobs.Collector.CollectorLevel[Jobs.Collector.GetCollectorLevel(player) + 1] : 99999}
                                  }
                                },
                            }
                        },
                    });

                    dict.Add("vipstatus", GetVipStatus(player));
                    break;
                }
              case 1: // Имущество 
              	{
                    dict.Add("estate", new Dictionary<string, object>()
                    {
                        {"house", GetPlayerHouse(player) },

                        {"business", GetPlayerBusiness(player) },

                        {"transport", new Dictionary<string, object>(){
                            {"auto", GetPlayerCars(player, VehicleManager.VehicleCars) },
                            {"bike", GetPlayerCars(player, VehicleManager.VehicleMoto) },
                            {"boat", GetPlayerCars(player, VehicleManager.VehicleBoats) },
                            {"helicopter", GetPlayerCars(player, VehicleManager.VehicleHelicopter) }
                        } }
                    });
              		break;
              	}
              case 2: // Задания
              	{
                    dict.Add("quest", new Dictionary<string, object>()
                    {
                        {"main", GetMainQuestStatus(player) },
                        {"daily", GetDailyQuestStatus(player) },
                        {"dop", new List<object>() }
                    });
              		break;
              	}
              case 3: // Достижения
              	{
                    dict.Add("achivment", new Dictionary<string, object>()
                    {
                        {"main", new Dictionary<string, object>(){
                            {"lvl", client.Core.Achievements.SeriliazeLevelAchivments(player) },
                            {"normal", client.Core.Achievements.SeriliazeNormalAchivments(player) }
                        }},
                        {"hide", new Dictionary<string, object>(){
                            {"normal", client.Core.Achievements.SeriliazeHideAchivments(player) },
                            }
                        }
                    });
              		break;
              	}
              case 4: // Магазин
              	{
                    dict.Add("shop", GetSerializeShop(player));
              		break;
              	}
              case 5: // Настройки
              	{
                    dict.Add("setting", new Dictionary<string, object>()
                    {
                        {"interface", new Dictionary<string, object>()
                            {
                            {"toogle", new Dictionary<string, object>(){
                                 {"help", false },
                                 {"hud", false },
                                 {"quest", false },
                                 {"interaction", false },
                                 {"playersName", false },
                                 {"transparent", false },
                                 {"timeMessage", false },
                                 {"notifySound", false },

                            } },
                            {"select", new Dictionary<string, object>(){
                                 {"mutLvl", new Dictionary<string, object>(){
                                     {"min", 0 },
                                     {"max", 999 },
                                     {"val", 1 },
                                 } },
                                 {"heightChat", new Dictionary<string, object>(){
                                     {"min", 1 },
                                     {"max", 2 },
                                     {"val", 1 },
                                 } },
                                 {"heightStringChat", new Dictionary<string, object>(){
                                     {"min", 1 },
                                     {"max", 3 },
                                     {"val", 1 },
                                 } },
                                 {"heightFontChat", new Dictionary<string, object>(){
                                     {"min", 1 },
                                     {"max", 3 },
                                     {"val", 1 },
                                 } },

                            } },

                            }
                        },
                        {"aim", new Dictionary<string, object>(){
                            {"type", 0 },
                            {"select", new Dictionary<string, object>()
                            {
                                {"size", new Dictionary<string, object>()
                                {
                                   {"min", 0 },
                                   {"max", 10 },
                                   {"val", 0 },
                                } },
                                {"rotate", new Dictionary<string, object>()
                                {
                                   {"min", 0 },
                                   {"max", 10 },
                                   {"val", 0 },
                                } }
                            } },
                            { "color", "#FFFB0D" },
                            { "newColor", "" },
                        } },
                        {"control", new Dictionary<string, object>(){
                                 {"micro", "k" },
                                 {"reload", "x" },
                                 {"engine", "2" },
                                 {"belt", "3" },
                                 {"cruise", "5" },
                                 {"signalsL","<" },
                                 {"emergency", "m" },
                                 {"signalsR", ">" },
                                 {"lockVehicle", "s" },
                                 {"phone", "o" },
                        } },
                    });
              		break;
              	}
              case 6: // Помощь
              	{
                    dict.Add("help", new Dictionary<string, object>()
                    {
                        {"titles", GetSerealizeHelpTitle() },
                        {"content", GetSereliazeGeneralTab()}
                    });
              		break;
              	}
              case 7: // Награды
              	{
                    dict.Add("reward", client.Core.Achievements.SeriliazeRewards(player));
              		break;
              	}
              case 8: // Репорты
              	{
                    dict.Add("report", GetSerializeReports(player));
              		break;
              	}
              case 9: // VIP Status
              	{
                    dict.Add("vipstatus", GetVipStatus(player));
              		break;
              	}
            }

            if (dict != new Dictionary<string, object>())
            {
                //Log.Write($"CHANGE TAB: {id} -> {JsonConvert.SerializeObject(dict)}");

                if (id == 5) Trigger.ClientEvent(player, "CLIENT::STATS:UPDATE_SETTINGS", JsonConvert.SerializeObject(dict));
                else Trigger.ClientEvent(player, "CLIENT::STATS:UPDATE", JsonConvert.SerializeObject(dict));
            }
        }

        [RemoteEvent("SERVER::HELP:CHANGE_TAB")]
        public static void ChangeHelpTab(Player player, int id)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();


            List<Dictionary<string, object>> helpList = new List<Dictionary<string, object>>();

            switch (id)
            {
                case 0:
                    helpList = GetSereliazeGeneralTab();
                    break;
                case 1:
                    helpList = GetSereliazeJobsTab();
                    break;
                case 2:
                    helpList = GetSereliazeFractionTab();
                    break;
                default:
                    helpList = GetSereliazeGeneralTab();
                    break;
            }

            dict.Add("help", new Dictionary<string, object>()
            {
                {"titles", GetSerealizeHelpTitle() },
                {"content", helpList}
            });

            Trigger.ClientEvent(player, "CLIENT::STATS:UPDATE", JsonConvert.SerializeObject(dict));
        }

        public static Dictionary<string, object> GetVipStatus(Player player)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();

            dict.Add("name", new List<string>() { "Silver", "Gold", "Platinum" });
            dict.Add("price", new List<int> { 300, 500, 900 });
            dict.Add("itemList", new List<string>() {
                "Зарплата",
                "Опыт",
                "Контакты",
                "Продажа дома",
                "Продажа авто",
                "Аренда транспорта",
                "Покинуть фракцию",
            });
            dict.Add("listItem", new List<List<string>>()
            {
                new List<string>(){ "x1.15", "2 EXP", "70", "x0.6" , "x0.6", "x0.9"},
                new List<string>(){ "x1.25", "2 EXP", "80", "x0.7", "x0.7", "x0.85"},
                new List<string>(){ "x1.35", "3 EXP", "100", "x0.8", "x0.8", "x0.8"},

            });

            return dict;
        }

        public static Dictionary<string, object> GetSerializeShop(Player player)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>()
            {
                {"login", Main.Accounts[player].Login },
                {"coin", Main.Accounts[player].RedBucks },
                {"change", Donations.ChangeDonateToMoney },
                {"item", DonateShop.shop }
            };

            return dict;
        }

        public static Dictionary<string, object> GetSerializeReports(Player player)
        {
            try
            {
                Dictionary<string, object> dict = new Dictionary<string, object>();

                dict.Add("admin", Main.Players[player].AdminLVL > 0 ? true : false);

                if (Main.Players[player].AdminLVL > 0)
                {
                    List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();

                    List<ReportSys.Report> reports = new List<ReportSys.Report>();
                    // reports.Reverse();

                    reports.AddRange(ReportSys.Reports.Values.ToList().Where((r) => (r.Admin == player.Name && !r.Status)).ToList());
                    reports.AddRange(ReportSys.Reports.Values.ToList().Where((r) => (!r.Status && !reports.Contains(r))).ToList());

                    int temp = 0;
                    foreach (ReportSys.Report rep in reports)
                    {
                        //if (rep.Admin != null && rep.Admin != player.Name) continue;

                        //if (rep.Status) continue;

                        list.Add(new Dictionary<string, object>()
                    {
                        {"id", rep.ID },
                        {"playerName",rep.Author },
                        {"lastMessage", rep.Messages.Last().Text },
                    });

                        if (rep.Admin != null && !rep.Status)
                        {
                            list[temp].Add("status", rep.Admin);
                        }
                        else if (rep.Status)
                        {
                            list[temp].Add("status", true);
                        }

                        Player target = NAPI.Player.GetPlayerFromName(rep.Author);

                        if (target != null)
                        {
                            list[temp].Add("online", true);
                        }
                        else
                        {
                            list[temp].Add("online", false);
                        }

                        list[temp].Add("playerId", rep.AuthorPersonID);

                        temp++;

                    }

                    dict.Add("reportList", list);
                }
                else
                {
                    List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();

                    List<ReportSys.Report> reports = ReportSys.Reports.Values.Where((t) => t.Author == player.Name).ToList();

                    reports.Reverse();

                    int tmp = 0;

                    foreach (ReportSys.Report rep in reports)
                    {
                        if (rep.Hide) continue;

                        list.Add(new Dictionary<string, object>()
                    {
                        {"id", rep.ID },
                        {"playerName",rep.Author },
                        {"lastMessage", rep.Messages.Last().Text },
                    });

                        if (rep.Admin != null || rep.Status)
                        {
                            if (rep.Status)
                                list[tmp].Add("status", true);
                            else
                                list[tmp].Add("status", rep.Admin);
                        }

                        tmp++;
                    }

                    dict.Add("reportList", list);
                }

                if (player.HasMyData("SELECTED_REPORT"))
                {
                    dict.Add("activeReport", GetSerealizeActiveReport(player));
                }
                else
                {
                    dict.Add("activeReport", new Dictionary<string, object>());
                }


                return dict;
            }
            catch(Exception ex)
            {
                Log.Write($"GetSerializeReports: " + ex.StackTrace);

                return new Dictionary<string, object>();
            }
        }

        public static Dictionary<string, object> GetSerealizeActiveReport(Player player)
        {
            try
            {
                int repId = player.GetMyData<int>("SELECTED_REPORT");

                Dictionary<string, object> dict = new Dictionary<string, object>();

                ReportSys.Report report = ReportSys.Reports[repId];

                dict.Add("id", report.ID);
                dict.Add("date", report.OpenedDate.ToString());
                if (Main.Players[player].AdminLVL > 0)
                {
                    Player target = NAPI.Player.GetPlayerFromName(report.Author);

                    if (target != null)
                        dict.Add("playerName", $"{report.Author} ({target.Value})");
                    else
                        dict.Add("playerName", $"{report.Author}");

                    if (target != null) dict.Add("playerId", Main.Players[target].PersonID);
                    else dict.Add("playerId", report.AuthorPersonID);
                    dict.Add("status", report.Status);
                    dict.Add("online", target is null ? false : true);

                }

                List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();

                bool first = true;
                foreach (ReportSys.ReportMessage mes in report.Messages)
                {
                    if (!mes.IsAdmin)
                    {
                        list.Add(new Dictionary<string, object>()
                    {
                       {"player", true },
                       {"date", mes.Date.ToString() },
                       {"text", mes.Text }
                    });

                    }
                    else if (mes.IsAdmin && !first)
                    {
                        list.Add(new Dictionary<string, object>()
                    {
                       {"player", false },
                       {"adminName", mes.Name },
                       {"date", mes.Date.ToString() },
                       {"text", mes.Text }
                    });
                    }
                    if (report.Admin != null && first)
                    {
                        first = false;

                        list.Add(new Dictionary<string, object>()
                    {
                       {"status", report.Admin },
                    });

                    }

                }


                if (report.Status)
                {
                    list.Add(new Dictionary<string, object>()
                    {
                       {"status", true },
                    });
                }


                dict.Add("messageList", list);

                return dict;
            }
            catch(Exception ex)
            {
                Log.Write($"GetSerealizeActiveReport: " + ex.StackTrace);

                return new Dictionary<string, object>();
            }

        }

        [RemoteEvent("SERVER::BINDER:RESET_BUTTONS")]
        public static void ResetButtons(Player player)
        {
            SendStatInfo(player);

            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Вы сбросили кнопки биндера", 3000);

            return;
        }

        [RemoteEvent("SERVER::STAT:RESET_AIM")]
        public static void ResetAim(Player player)
        {
            SendStatInfo(player, 5);

            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Вы сбросили прицел", 3000);

            return;
        }

        public static List<Dictionary<string, object>> GetPlayerHouse(Player player)
        {
            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();

            foreach(House house in HouseManager.Houses)
            {
                if(house.Owner == player.Name)
                {
                    list.Add(new Dictionary<string, object>()
                    {
                        {"class", HouseManager.HouseTypeList[house.Type].Name },
                        {"number", house.ID },
                        {"park", house.GarageID == -1 ? 0 : GarageManager.GarageTypes[GarageManager.Garages[house.GarageID].Type].MaxCars },
                        {"price", house.Price },
                        {"date", house.LastBuy.ToString("MM/dd/yyyy HH:mm") },
                    });
                }
            }

            return list;
        }

        public static List<Dictionary<string, object>> GetPlayerBusiness(Player player)
        {
            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();

            if (Main.Players[player].BizIDs.Count == 0) return list;

            int bizId = Main.Players[player].BizIDs[0];

            if (BusinessManager.BizList[bizId].Owner == player.Name)
            {
                Business biz = BusinessManager.BizList[bizId];
                list.Add(new Dictionary<string, object>()
                {
                    {"name",$"{BusinessManager.BusinessTypeNames[biz.Type]} #{biz.ID}"},
                    {"price", biz.SellPrice },
                    {"date", biz.LastBuy.ToString("MM/dd/yyyy HH:mm") },
                });
            }
            //{
            //  name: "Магазин 24/7 #727",
            //  price: 680000,
            //  date: "12.12.2000 12:00",
            //},

            return list;
        }

        public static List<Dictionary<string, object>> GetPlayerCars(Player player, List<int> vehType)
        {
            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();

            foreach(KeyValuePair<int, VehicleManager.VehicleData> pair in VehicleManager.Vehicles)
            {
                if(pair.Value.Holder == player.Name)
                {
                    if (vehType.Contains(NAPI.Vehicle.GetVehicleClass((VehicleHash)NAPI.Util.GetHashKey(pair.Value.Model))))
                    {
                        list.Add(new Dictionary<string, object>()
                        {
                            {"img", pair.Value.Model },
                            {"class",  NAPI.Vehicle.GetVehicleClassName(NAPI.Vehicle.GetVehicleClass((VehicleHash)NAPI.Util.GetHashKey(pair.Value.Model)))},
                            {"name", GetVehicleRealName(pair.Value.Model.ToLower()) },
                            {"price",  BusinessManager.ProductsOrderPrice.ContainsKey(pair.Value.Model) ? BusinessManager.ProductsOrderPrice[pair.Value.Model] : 0},
                            {"km", Convert.ToInt32(pair.Value.Mileage) }
                        });
                    }
                }
            }

            return list;
        }

        public static string GetVehicleRealName(string model)
        {
            string name = model;

            foreach(KeyValuePair<int, Dictionary<string, string>> pair in BusinessManager.RealVehicles)
            {
                if(pair.Value.ContainsKey(model))
                    return pair.Value[model];
            }

            return name;
        }

        public static List<Dictionary<string, object>> GetMainQuestStatus(Player player)
        {
            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();

            if(Main.Players[player].QuestChapter != 0 && Main.Players[player].QuestFinished != 1)
            {
                list.Add(new Dictionary<string, object>()
                {
                       {"status", false },
                       {"name", QuestSystem.Chapter[player].Name },
                       {"price", QuestSystem.Chapter[player].Reward },
                       {"list", new List<string>(){ QuestSystem.Chapter[player].Description }
                    }
                });
            }

            return list;
        }

        public static List<Dictionary<string, object>> GetDailyQuestStatus(Player player)
        {
            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();

            if (Manager.isHaveFamily(player))
            {
                Family family = Family.GetFamilyToCid(player);

                if (family.ActiveMission != null)
                {
                    list.Add(new Dictionary<string, object>()
                    {
                           {"status", false },
                           {"name", family.ActiveMission.Name },
                           {"price", family.ActiveMission.Money },
                           {"text", family.ActiveMission.GetMissionText(family)},

                    });
                }
            }

            return list;
        }

        public static Task SendStatsAsync(Player player)
        {
            try
            {
                if (!Main.Players.ContainsKey(player))
                    return Task.CompletedTask;
                Core.Character.Character acc = Main.Players[player];

                string status =
                    acc.AdminLVL >= 1 ? "Администратор" :
                    Main.Players[player].VipLvl > 0 ? $"{Group.GroupNames[Main.Players[player].VipLvl]} до {Main.Players[player].VipDate.ToString("dd.MM.yyyy")}" :
                    $"{Group.GroupNames[Main.Players[player].VipLvl]}";

                long bank = acc.Bank != 0 ? Bank.Accounts[acc.Bank].Balance : 0;

                string lic = "";
                for (int i = 0; i < acc.Licenses.Count; i++)
                    if (acc.Licenses[i]) lic += $"{Main.LicWords[i]} / ";
                if (lic == "") lic = "Отсутствуют";

                string work = acc.WorkID > 0 ? Jobs.JobManager.JobListNames[acc.WorkID - 1] : "Отсутствует";
                string fraction = acc.Fraction.FractionID > 0 ? Fractions.Manager.FractionNames[acc.Fraction.FractionID] : "Отсутствует";

                string number = acc.Sim == -1 ? "Нет сим-карты" : Main.Players[player].Sim.ToString();



                List<object> data = new List<object>
                {
                    acc.LVL, //0
                    $"{acc.EXP}/{3 + acc.LVL * 3}", //1
                    number, //2
                    status, //3
                    acc.Warns,//4
                    lic,//5
                    acc.CreateDate.ToString("dd.MM.yyyy"),//6
                    work,//7
                    fraction,//8
                    acc.Fraction.FractionRankID,//9
                    acc.FirstName,//10
                    acc.LastName,//11
                    acc.UUID,//12
                    acc.Bank,//13
                    acc.Eat,
                    acc.Water
                };

                string json = JsonConvert.SerializeObject(data);
                Log.Debug("data is: " + json.ToString());
                Trigger.ClientEvent(player, "board", 2, json);

                data.Clear();
            }
            catch (Exception e)
            {
                Log.Write("EXCEPTION AT \"DASHBOARD_SENDSTATS SendStatsAsync\":\n" + e.ToString(), nLog.Type.Error);
            }

            return Task.CompletedTask;
        }

        public static void sendItems(Player player)
        {
            try
            {
                if (!Main.Players.ContainsKey(player)) return;
                int UUID = Main.Players[player].UUID;
                PsendItems(player, nInventory.Items[UUID], 2);
                //Log.Debug("sendItems: "+ JsonConvert.SerializeObject(nInventory.Items[UUID]));
            }
            catch (Exception e)
            {
                Log.Write("EXCEPTION AT \"DASHBOARD_SENDITEMS\":\n" + e.ToString(), nLog.Type.Error);
            }
        }

        public static async Task SendItemsAsync(Player player)
        {
            try
            {
                if (!Main.Players.ContainsKey(player)) return;
                int UUID = Main.Players[player].UUID;

                if (!nInventory.Items.ContainsKey(UUID)) return;
                List<nItem> items = new List<nItem>(nInventory.Items[UUID]);

                List<object> data = new List<object>();
                foreach (nItem item in items)
                {
                    List<object> idata = new List<object>
                    {
                        item.ID,
                        item.Count,
                        (item.IsActive) ? 1 : 0,
                        (nInventory.WeaponsItems.Contains(item.Type) || item.Type == ItemType.Stungun) ? "Serial: " + item.Data : (item.Type == ItemType.CarKey) ? $"{(string)item.Data.Split('_')[0]}" : ""
                    };
                    data.Add(idata);
                }

                string json = JsonConvert.SerializeObject(data);
                await Log.DebugAsync(json);
                NAPI.Task.Run(() => Trigger.ClientEvent(player, "board", 3, json));

                items.Clear();
                data.Clear();
            }
            catch (Exception e)
            {
                Log.Write("EXCEPTION AT \"DASHBOARD_SENDITEMS\":\n" + e.ToString(), nLog.Type.Error);
            }
        }

        public static void Open(Player Player)
        {
            Trigger.ClientEvent(Player, "board", 0);
        }

        public static void OpenOut(Player Player, List<nItem> items, string title, int type = 1)
        {
            try
            {
                if (type == 0) return;
                Player.SetMyData("OPENOUT_TYPE", type);
                PsendItems(Player, items, 4);
                Trigger.ClientEvent(Player, "inventory", 0);
            }
            catch (Exception e)
            {
                Log.Write("EXCEPTION AT \"DASHBOARD_OPENOUT\":\n" + e.ToString(), nLog.Type.Error);
            }
        }

        public static void OpenTrade(Player player)
        {
            try
            {
                PsendItems(player, new List<nItem>(), 5);
                PsendItems(player, new List<nItem>(), 6);
                Trigger.ClientEvent(player, "inventory", 9, Main.Players[player].Money);
                Trigger.ClientEvent(player, "inventory", 0);

                Dictionary<string, object> mydata = new Dictionary<string, object>()
                {
                   {"targetName",player.GetMyData<Player>("CHANGE_WITH").Name},
                };
                string json = JsonConvert.SerializeObject(mydata);
                Trigger.ClientEvent(player, "inventory", 10, json);
            }
            catch (Exception e)
            {
                Log.Write("EXCEPTION AT \"DASHBOARD_OPENTRADE\":\n" + e.StackTrace, nLog.Type.Error);
            }
        }

        public static void Update(Player Player, nItem item, int index)
        {
            PsendItems(Player, nInventory.Items[Main.Players[Player].UUID], 2);

        }

        public static Task UpdateAsync(Player Player, nItem item, int index)
        {
            try
            {
                List<object> idata = new List<object>
                    {
                        item.ID,
                        item.Count,
                        item.IsActive ? 1 : 0,
                        nInventory.WeaponsItems.Contains(item.Type) || item.Type == ItemType.Stungun ? "Serial: " + item.Data : item.Type == ItemType.CarKey ? $"{(string)item.Data.Split('_')[0]}" : ""
                    };
                string json = JsonConvert.SerializeObject(idata);
                NAPI.Task.Run(() => Trigger.ClientEvent(Player, "board", 6, json, index));
            }
            catch (Exception e) { Log.Write("UpdateAsync: " + e.StackTrace); }

            return Task.CompletedTask;
        }

        public static List<nItem> getChangeItems(Player player)
        {
            if (player.HasMyData("OPENOUT_TYPE") && (player.GetMyData<int>("OPENOUT_TYPE") == 8 || player.GetMyData<int>("OPENOUT_TYPE") == 4 || player.GetMyData<int>("OPENOUT_TYPE") == 3))
            {
                int houseID = Main.Players[player].InsideHouseID;
                int furnID = player.GetMyData<int>("OpennedSafe");
                List<nItem> items = FurnitureManager.FurnituresItems[houseID][furnID];
                return items;
            }
            else if(player.HasMyData("OPENOUT_TYPE") && player.GetMyData<int>("OPENOUT_TYPE") == 1070)
            {
                int houseID = Main.Players[player].InsideHouseID;
                List<nItem> items = HouseManager.GetHouse(player).Items;
                return items;
            }
            else if (player.HasMyData("OPENOUT_TYPE") && player.GetMyData<int>("OPENOUT_TYPE") == 6)
            {
                int onFrac = player.GetMyData<int>("ONFRACSTOCK");
                List<nItem> items = client.Fractions.Utils.Stocks.fracStocks[onFrac].Items;
                return items;
            }
            else if (player.HasMyData("OPENOUT_TYPE") && (player.GetMyData<int>("OPENOUT_TYPE") == 80 || player.GetMyData<int>("OPENOUT_TYPE") == 81 || player.GetMyData<int>("OPENOUT_TYPE") == 82))
            {
                int houseID = player.GetMyData<int>("OpennedSafehouseID");
                int furnID = player.GetMyData<int>("OpennedSafe");
                List<nItem> items = FamilyFurnitureManager.FamilyFurnituresItems[houseID][furnID];
                return items;
            }
            else if (player.HasMyData("OPENOUT_TYPE") && player.GetMyData<int>("OPENOUT_TYPE") == 1080)
            {
                if (player == null || !Main.Players.ContainsKey(player)) return null;
                if (!player.HasMyData("ON_FAM_STOCK")) return null;

                string onFrac = player.GetMyData<string>("ON_FAM_STOCK");
                List<nItem> items = client.Families.FamilyOfficeManager.Officies[onFrac].Items;
                return items;
            }
            else if (player.HasMyData("OPENOUT_TYPE") && player.GetMyData<int>("OPENOUT_TYPE") == 1081)
            {
                FamilyHouse house = FamilyHouseManager.GetHouse(player);
                List<nItem> items = house.Items;
                return items;
            }
            else if (player.HasMyData("OPENOUT_TYPE") && player.GetMyData<int>("OPENOUT_TYPE") == 2000)
            {
                return AirDrop.AirDropItems;
            }
            else if (player.HasMyData("CHANGE_WITH"))
            {
                List<nItem> items = player.GetMyData<List<nItem>>("CHANGE_WITH_ITEMS");
                return items;
            }
            else
            {
                Vehicle veh = player.GetMyData<Vehicle>("SELECTEDVEH");
                if(veh != null && veh.GetData<string>("ACCESS") == "FRACTION")
                {
                    if (!Configs.FractionVehicles[Main.Players[player].Fraction.FractionID].ContainsKey(veh.NumberPlate.ToString())) return null;
                    return Configs.FractionVehicles[Main.Players[player].Fraction.FractionID][veh.NumberPlate.ToString()].Items;
                }
                else
                {
                    int id = veh.GetData<int>("ID");
                    return VehicleManager.Vehicles[id].Items;/*veh.GetData<List<nItem>>("ITEMS")*/;
                }
            }
        }

        public static List<bool> getChangeSlots(Player player)
        {
            if (player.HasMyData("OPENOUT_TYPE") && (player.GetMyData<int>("OPENOUT_TYPE") == 8 || player.GetMyData<int>("OPENOUT_TYPE") == 4 || player.GetMyData<int>("OPENOUT_TYPE") == 3))
            {
                int houseID = Main.Players[player].InsideHouseID;
                int furnID = player.GetMyData<int>("OpennedSafe");
                List<bool> slots = FurnitureManager.FurnituresSlots[houseID][furnID];
                return slots;
            }
            else if(player.HasMyData("OPENOUT_TYPE") && player.GetMyData<int>("OPENOUT_TYPE") == 1070)
            {
                int houseID = Main.Players[player].InsideHouseID;
                House house = HouseManager.GetHouse(player);
                List<bool> slots = house.Slots;
                return slots;
            }
            else if (player.HasMyData("OPENOUT_TYPE") && player.GetMyData<int>("OPENOUT_TYPE") == 6)
            {
                int onFrac = player.GetMyData<int>("ONFRACSTOCK");
                List<bool> slots = client.Fractions.Utils.Stocks.fracStocks[onFrac].Slots;
                return slots;
            }
            else if (player.HasMyData("OPENOUT_TYPE") && (player.GetMyData<int>("OPENOUT_TYPE") == 80 || player.GetMyData<int>("OPENOUT_TYPE") == 81 || player.GetMyData<int>("OPENOUT_TYPE") == 82))
            {
                int houseID = player.GetMyData<int>("OpennedSafehouseID");
                int furnID = player.GetMyData<int>("OpennedSafe");
                List<bool> slots = FamilyFurnitureManager.FamilyFurnituresSlots[houseID][furnID];
                return slots;
            }
            else if (player.HasMyData("CHANGE_WITH"))
            {
                List<bool> slots = player.GetMyData<List<bool>>("CHANGE_WITH_SLOTS");
                return slots;
            }
            else if (player.HasMyData("OPENOUT_TYPE") && player.GetMyData<int>("OPENOUT_TYPE") == 1080)
            {
                string onOffice = player.GetMyData<string>("ON_FAM_STOCK");
                if (player == null || !Main.Players.ContainsKey(player)) return null;
                if (!player.HasMyData("ON_FAM_STOCK")) return null;
                List<bool> slots = client.Families.FamilyOfficeManager.Officies[onOffice].Slots;
                return slots;
            }
            else if (player.HasMyData("OPENOUT_TYPE") && player.GetMyData<int>("OPENOUT_TYPE") == 1081)
            {
                FamilyHouse house = FamilyHouseManager.GetHouse(player);
                List<bool> slots = house.Slots;
                return slots;
            }
            else if (player.HasMyData("OPENOUT_TYPE") && player.GetMyData<int>("OPENOUT_TYPE") == 2000)
            {
                List<bool> tempslots = new List<bool>();
                for (int i = 0; i < 65; i++)
                {
                    tempslots.Add(true);
                }
                return tempslots;
            }
            else
            {
                Vehicle veh = player.GetMyData<Vehicle>("SELECTEDVEH");
                if (veh != null && veh.GetData<string>("ACCESS") == "FRACTION")
                {
                    if (!Configs.FractionVehicles[Main.Players[player].Fraction.FractionID].ContainsKey(veh.NumberPlate.ToString())) return null;
                    return Configs.FractionVehicles[Main.Players[player].Fraction.FractionID][veh.NumberPlate.ToString()].Slots;
                }
                else
                {
                    int id = veh.GetData<int>("ID");
                    return VehicleManager.Vehicles[id].Slots;/*veh.GetData<List<nItem>>("ITEMS")*/;
                }
            }
        }

        [RemoteEvent("inventoryConcat_Server")]
        public void inventoryConcat_Server(Player player, int sourceSlotId, int targetSlotId, string targetGroupCode, string sourceGroupCode, int count)
        {
            try
            {
                int UUID = Main.Players[player].UUID;
                List<nItem> SourceItems = nInventory.Items[UUID];
                List<bool> SourceSlots = nInventory.ItemsSlots[UUID];
                List<nItem> TargetItems = nInventory.Items[UUID];
                List<bool> TargetSlots = nInventory.ItemsSlots[UUID];
                switch (sourceGroupCode)
                {
                    case "inventory":
                        SourceItems = nInventory.Items[UUID];
                        SourceSlots = nInventory.ItemsSlots[UUID];
                        break;
                    case "outside":
                        SourceItems = getChangeItems(player);
                        SourceSlots = getChangeSlots(player);
                        break;
                    case "bug":
                        SourceItems = nInventory.Items[UUID].Find(i => nInventory.ClothesItemSlots.ContainsKey(i.Type) && i.IsActive && nInventory.ClothesItemSlots[i.Type] == 12).Items;
                        SourceSlots = nInventory.Items[UUID].Find(i => nInventory.ClothesItemSlots.ContainsKey(i.Type) && i.IsActive && nInventory.ClothesItemSlots[i.Type] == 12).Slots;
                        break;
                    case "fastSlot":
                        SourceItems = nInventory.Items[UUID];
                        SourceSlots = nInventory.ItemsSlots[UUID];
                        break;
                    case "trade":
                        SourceItems = getChangeItems(player);
                        SourceSlots = getChangeSlots(player);
                        break;
                }
                switch (targetGroupCode)
                {
                    case "inventory":
                        TargetItems = nInventory.Items[UUID];
                        TargetSlots = nInventory.ItemsSlots[UUID];
                        break;
                    case "outside":
                        TargetItems = getChangeItems(player);
                        TargetSlots = getChangeSlots(player);
                        break;
                    case "bug":
                        TargetItems = nInventory.Items[UUID].Find(i => nInventory.ClothesItemSlots.ContainsKey(i.Type) && i.IsActive && nInventory.ClothesItemSlots[i.Type] == 12).Items;
                        TargetSlots = nInventory.Items[UUID].Find(i => nInventory.ClothesItemSlots.ContainsKey(i.Type) && i.IsActive && nInventory.ClothesItemSlots[i.Type] == 12).Slots;
                        break;
                    case "fastSlot":
                        TargetItems = nInventory.Items[UUID];
                        TargetSlots = nInventory.ItemsSlots[UUID];
                        break;
                    case "trade":
                        TargetItems = getChangeItems(player);
                        TargetSlots = getChangeSlots(player);
                        break;
                }

                if (SourceItems == null || SourceSlots == null) {
                    Log.Debug($"[{player.Name}] NOT FOUND Items or SLOTS");
                    return;
                }

                if (TargetItems == null || TargetSlots == null) {
                    Log.Debug($"[{player.Name}] NOT FOUND Items or SLOTS");
                    return;
                }

                nItem TargetItem = null;
                nItem sourceItem = null;

                if (sourceGroupCode == "fastSlot") {
                    TargetItem = TargetItems.Find(i => i.slot_id == targetSlotId);
                    sourceItem = SourceItems.Find(i => i.fast_slot_id == sourceSlotId);
                } else {
                    TargetItem = TargetItems.Find(i => i.slot_id == targetSlotId);
                    sourceItem = SourceItems.Find(i => i.slot_id == sourceSlotId);
                }

                if (TargetItem == null || sourceItem == null)
                {
                    Log.Debug($"[{player.Name}] NOT FOUND ITEM");
                    return;
                }


                if (TargetItem.Count + count <= nInventory.ItemsStacks[sourceItem.Type] && count <= sourceItem.Count && count > 0)
                {
                    if (count == sourceItem.Count) {
                        //Log.Debug("CLEAR!? Concat");
                        nInventory.ClearSlot(SourceSlots, sourceItem, 5);
                        SourceItems.Remove(sourceItem);
                    } else
                    {
                        sourceItem.Count -= count;
                    }
                    TargetItem.Count = TargetItem.Count + count;
                    TargetItem.Weight = nInventory.ItemsWeight[TargetItem.Type] * TargetItem.Count;
                }

                PsendItems(player, nInventory.Items[UUID], 2);

                if (targetGroupCode == "trade" || sourceGroupCode == "trade")
                {
                    PsendItems(player, player.GetMyData<List<nItem>>("CHANGE_WITH_ITEMS"), 5);
                    PsendItems(player.GetMyData<Player>("CHANGE_WITH"), player.GetMyData<List<nItem>>("CHANGE_WITH_ITEMS"), 6);
                }

                if (targetGroupCode == "outside" || sourceGroupCode == "outside")
                {

                    if (player.GetMyData<int>("OPENOUT_TYPE") == 2)
                    {
                        Vehicle veh = player.GetMyData<Vehicle>("SELECTEDVEH");
                        veh.SetData("ITEMS", getChangeItems(player).ToList());
                        // VehicleManager.Vehicles[veh.NumberPlate].Items = TargetItems.ToList();

                    }

                    PsendItems(player, getChangeItems(player), 4);
                }
                GameLog.Items($"player({Main.Players[player].UUID + " in " + sourceGroupCode})", $"out ({targetGroupCode})", Convert.ToInt32(sourceItem.Type), 1, $"{TargetItem.Data}");

            }
            catch (Exception e) { Log.Write("openInventory: " + e.StackTrace, nLog.Type.Error); }
        }

        // [Command("addattachment")]
        private static void AddAttachment(Player player, Weapons.WeaponHash attachname)
        {
            Log.Debug($"[AddAttachment] player: {player.Name} - {attachname}", nLog.Type.Info);
            // Make sure you've registered this attachment on the client-side, check rootcause's example in ragempdev's package to see how.
            if (player.HasAttachment(attachname))
                player.AddAttachment(attachname, true);
            else
                player.AddAttachment(attachname, false);
        }

        public static void SyncAttachComp(Player player, nItem item, bool remove)
        {
            try
            {
                var wHash = Weapons.GetWeaponHash(item.Type.ToString());
                if (remove)
                {
                    if (!item.IsAttach) return;
                    Main.Players[player].WeaponComponentsSync.Remove(Convert.ToUInt32(wHash));
                    Main.Players[player].AttachWeapSlots[nInventory.WeaponAttachSlot[Convert.ToUInt32(wHash)]] = false;
                    item.IsAttach = false;
                    player.SetSharedData("WeaponComponentsSync", JsonConvert.SerializeObject(Main.Players[player].WeaponComponentsSync));
                    Log.Debug("[SyncAttachComp] remove TRUE", nLog.Type.Info);
                    AddAttachment(player, wHash);
                }
                else
                {
                    if (Main.Players[player].AttachWeapSlots[nInventory.WeaponAttachSlot[Convert.ToUInt32(wHash)]]) return;
                    List<string> Components = new List<string>();
                    foreach (var comp in item.WData.Components.Values)
                    {
                        if (comp.Сtype != null)
                            Components.Add(comp.Сtype);
                        else
                            Components.Add("0");
                    }
                    Log.Debug("[SyncAttachComp] remove FALSE", nLog.Type.Info);
                    Log.Debug("wHash " + wHash + "  wHash uint " + Convert.ToUInt32(wHash) + " Components " + Components.Count + " WeaponComponentsSync " + Main.Players[player].WeaponComponentsSync.Count);
                    Main.Players[player].WeaponComponentsSync.Add(Convert.ToUInt32(wHash), Components);
                    Main.Players[player].AttachWeapSlots[nInventory.WeaponAttachSlot[Convert.ToUInt32(wHash)]] = true;
                    item.IsAttach = true;
                    player.SetSharedData("WeaponComponentsSync", JsonConvert.SerializeObject(Main.Players[player].WeaponComponentsSync));
                    AddAttachment(player, wHash);
                }


            }
            catch (Exception e)
            {
                Log.Write("EXCEPTION AT \"DASHBOARD_SyncAttachComp\":\n" + e.StackTrace, nLog.Type.Error);
            }

        }

        [RemoteEvent("inventoryFastSlot_Server")]
        public void inventoryFastSlot_Server(Player player, string targetGroupCode, int sourceSlotId, int targetSlotId)
        {
            try
            {
                int UUID = Main.Players[player].UUID;
                nItem item = nInventory.Items[UUID].Find(i => i.slot_id == sourceSlotId);
                //TODO: Efast
                //if (!nInventory.WeaponsItems.Contains(item.Type) && !item.IsActive)
                //{
                //    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Нельзя переместить этот предмет", 3000);
                //    return;
                //}

                item.fast_slot_id = targetSlotId;
                //Log.Debug("CLEAR!? FastSlot");
                nInventory.ClearSlot(nInventory.ItemsSlots[UUID], item, 5);
                if (nInventory.WeaponsItems.Contains(item.Type) && !item.IsActive) SyncAttachComp(player, item, false);
                item.slot_id = 0;
                GameLog.Items($"player({Main.Players[player].UUID + " in fastslot"})", $"out ({targetGroupCode})", Convert.ToInt32(item.Type), 1, $"{item.Data}");
                PsendItems(player, nInventory.Items[UUID], 2);
            }
            catch (Exception e) { Log.Write("inventoryFastSlot_Server: " + e.StackTrace, nLog.Type.Error); }

        }

        [RemoteEvent("inventoryFastSlotOut_Server")]
        public void inventoryFastSlotOut_Server(Player player, string targetGroupCode, int sourceSlotId, int targetSlotId)
        {
            try
            {
                int UUID = Main.Players[player].UUID;
                if (targetGroupCode == "fastSlot")
                    nInventory.Items[UUID].Find(i => i.fast_slot_id == sourceSlotId).fast_slot_id = targetSlotId;
                if (targetGroupCode == "character")
                {
                    var fItem = nInventory.Items[UUID].FirstOrDefault(i => i.fast_slot_id == 2);
                    if (fItem != null)
                    {
                        var index = nInventory.Items[UUID].IndexOf(fItem);

                        if (nInventory.ClothesItems.Contains(fItem.Type))
                        {
                            var hasActiveItem = nInventory.Items[UUID].FirstOrDefault(i => i.character_slot_id == nInventory.ClothesItemSlots[fItem.Type]);
                            if (hasActiveItem == null)
                            {
                                fItem.fast_slot_id = 0;
                                fItem.character_slot_id = nInventory.ClothesItemSlots[fItem.Type];
                            } else
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы не можете одеть это", 3000);
                                return;
                            }
                            //fItem.fast_slot_id = 0;
                            //fItem.character_slot_id = nInventory.ClothesItemSlots[fItem.Type];
                        }
                        //Items.onUse(player, fItem, index);
                    }
                }
                else
                {
                    nItem item = nInventory.Items[UUID].Find(i => i.fast_slot_id == sourceSlotId);
                    if (nInventory.WeaponsItems.Contains(item.Type) && !item.IsActive) SyncAttachComp(player, item, true);
                    item.fast_slot_id = 0;
                    item.slot_id = targetSlotId;
                    nInventory.FillSlot(nInventory.ItemsSlots[UUID], item, 5);
                }
                GameLog.Items($"player({Main.Players[player].UUID + " in fastslot"})", $"out ({targetGroupCode})", sourceSlotId, 1, $"{targetSlotId}");
                PsendItems(player, nInventory.Items[UUID], 2);
            }
            catch (Exception e) { Log.Write("inventoryFastSlotOut_Server: " + e.StackTrace, nLog.Type.Error); }
        }

        [RemoteEvent("inventoryTransfer_Server")]
        public void inventoryTransfer_Server(Player player, string sourceGroupCode, string targetGroupCode, int sourceSlotId, int targetSlotId)
        {
            try
            {
                Log.Write($"{sourceGroupCode} {targetGroupCode}");
                int UUID = Main.Players[player].UUID;
                List<nItem> SourceItems = nInventory.Items[UUID];
                List<bool> SourceSlots = nInventory.ItemsSlots[UUID];
                List<nItem> TargetItems = nInventory.Items[UUID];
                List<bool> TargetSlots = nInventory.ItemsSlots[UUID];
                switch (sourceGroupCode)
                {
                    case "inventory":
                        SourceItems = nInventory.Items[UUID];
                        SourceSlots = nInventory.ItemsSlots[UUID];
                        break;
                    case "outside":
                        SourceItems = getChangeItems(player);
                        SourceSlots = getChangeSlots(player);
                        break;
                    case "bug":
                        SourceItems = nInventory.Items[UUID].Find(i => nInventory.ClothesItemSlots.ContainsKey(i.Type) && i.IsActive && nInventory.ClothesItemSlots[i.Type] == 12).Items;
                        SourceSlots = nInventory.Items[UUID].Find(i => nInventory.ClothesItemSlots.ContainsKey(i.Type) && i.IsActive && nInventory.ClothesItemSlots[i.Type] == 12).Slots;
                        break;
                    case "fastSlot":
                        SourceItems = nInventory.Items[UUID];
                        SourceSlots = nInventory.ItemsSlots[UUID];
                        break;
                    case "modify":
                        SourceItems = nInventory.Items[UUID];
                        SourceSlots = nInventory.ItemsSlots[UUID];
                        break;
                    case "trade":
                        SourceItems = getChangeItems(player);
                        SourceSlots = getChangeSlots(player);
                        break;

                }
                switch (targetGroupCode)
                {
                    case "inventory":
                        TargetItems = nInventory.Items[UUID];
                        TargetSlots = nInventory.ItemsSlots[UUID];
                        break;
                    case "outside":
                        TargetItems = getChangeItems(player);
                        TargetSlots = getChangeSlots(player);
                        break;
                    case "bug":
                        TargetItems = nInventory.Items[UUID].Find(i => nInventory.ClothesItemSlots.ContainsKey(i.Type) && i.IsActive && nInventory.ClothesItemSlots[i.Type] == 12).Items;
                        TargetSlots = nInventory.Items[UUID].Find(i => nInventory.ClothesItemSlots.ContainsKey(i.Type) && i.IsActive && nInventory.ClothesItemSlots[i.Type] == 12).Slots;
                        break;
                    case "modify":
                        TargetItems = nInventory.Items[UUID];
                        TargetSlots = nInventory.ItemsSlots[UUID];
                        break;
                    case "trade":
                        TargetItems = getChangeItems(player);
                        TargetSlots = getChangeSlots(player);
                        break;
                }

                if (SourceItems == null || SourceSlots == null) {
                    Log.Debug($"[{player.Name}] NOT FOUND Items or SLOTS");
                    return;
                }

                if (TargetItems == null || TargetSlots == null) {
                    Log.Debug($"[{player.Name}] NOT FOUND Items or SLOTS");
                    return;
                }

                nItem sourceItem;
                if (sourceGroupCode == "fastSlot")
                {
                    sourceItem = SourceItems.Find(i => i.fast_slot_id == sourceSlotId);

                }
                else if (sourceGroupCode == "modify")
                {
                    var weapon = SourceItems.Find(i => nInventory.WeaponsItems.Contains(i.Type) && i.IsActive);
                    if (weapon == null)
                    {
                        PsendItems(player, nInventory.Items[UUID], 2);
                        return;
                    }



                    //Log.Write("sourceSlotId : -> " + sourceSlotId, nLog.Type.Error);

                    switch (sourceSlotId)
                    {
                        case 1:
                            sourceSlotId = 4;
                            break;
                        case 4:
                            sourceSlotId = 1;
                            break;
                    }

                    //Log.Debug("new sourceSlotId : -> " + sourceSlotId, nLog.Type.Error);

                    //Log.Debug("weapon.WData.Componentse : -> " + JsonConvert.SerializeObject(weapon.WData.Components), nLog.Type.Error);
                    //Log.Debug("weapon.WData.Components[sourceSlotId].Сtype : -> " + weapon.WData.Components[sourceSlotId].Сtype, nLog.Type.Error);

                    sourceItem = new nItem(nInventory.ComponentsType[weapon.WData.Components[sourceSlotId].Сtype]);
                    //Log.Debug("sourceItem : -> " + sourceItem, nLog.Type.Success);

                    weapon.WData.Components[sourceSlotId] = new Core.WeaponComponent();
                    var wHash = Weapons.GetWeaponHash(Main.Players[player].currentWeapon);
                    //Log.Write("transfer: wHash: " + wHash, nLog.Type.Error);
                    List<string> tempComponents = new List<string>();
                    foreach (var ComponentType in weapon.WData.Components.Values)
                    {
                        tempComponents.Add(ComponentType.Сtype);
                    }
                    player.SetSharedData("currentWeaponComponents", (uint)wHash + "." + string.Join("|", tempComponents.ToArray()));
                    var pos = player.Position;
                    var pId = player.Id;
                    foreach (var comp in tempComponents)
                    {
                        if (comp != null)
                            Trigger.ClientEventInRange(pos, 500.0f, "updatePlayerWeaponComponent", pId, (GTANetworkAPI.WeaponHash)wHash, comp, false);
                    }
                    sourceItem.slot_id = targetSlotId;
                    TargetItems.Add(sourceItem);
                    nInventory.FillSlot(TargetSlots, sourceItem, 5);
                    PsendItems(player, nInventory.Items[UUID], 2);
                    return;
                }
                else
                {
                    sourceItem = SourceItems.Find(i => i.slot_id == sourceSlotId);
                }

                if (targetSlotId == -1)
                {
                    int slot = nInventory.CheckAdd(TargetItems, sourceItem, TargetSlots);
                    if (slot == -1) return;
                    targetSlotId = slot;
                    Log.Debug("targetslotid "+ targetSlotId);
                }

                if (sourceItem.IsActive)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Нельзя переместить активный предмет", 3000);
                    PsendItems(player, nInventory.Items[UUID], 2);
                    if (targetGroupCode == "outside" || sourceGroupCode == "outside")
                    {
                        PsendItems(player, getChangeItems(player), 4);
                    }
                    return;
                }
                if (targetGroupCode == "modify")
                {
                    Log.Debug("sourceItem.Type: -> " + sourceItem.Type, nLog.Type.Error);
                    var temp = nInventory.Items[UUID].FirstOrDefault(i => nInventory.ComponentsTypetoType[sourceItem.Type].Contains(i.Type) && i.IsActive == true);
                    Log.Debug(sourceItem.Type + " type " + temp);
                    var activesmg = nInventory.Items[UUID].FirstOrDefault(i => nInventory.ComponentsTypetoType[sourceItem.Type].Contains(i.Type) && i.IsActive == true);
                    if (activesmg == null)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Это сюда не подходит", 3000);
                        PsendItems(player, nInventory.Items[UUID], 2);
                        return;
                    }
                }
                if (sourceGroupCode == "fastSlot")
                {
                    sourceItem.fast_slot_id = 0;
                    if (nInventory.WeaponsItems.Contains(sourceItem.Type) && !sourceItem.IsActive) SyncAttachComp(player, sourceItem, true);
                }
                else
                {
                    //Log.Debug($"CLEAR!? transfer SourceSlots: {JsonConvert.SerializeObject(SourceSlots)} sourceItem: slot_id: {sourceItem.slot_id}");
                    nInventory.ClearSlot(SourceSlots, sourceItem, 5);
                }
                SourceItems.Remove(sourceItem);
                if (targetGroupCode == "modify")
                {
                    var weapon = SourceItems.Find(i => nInventory.WeaponsItems.Contains(i.Type) && i.IsActive);
                    if (weapon == null)
                    {
                        PsendItems(player, nInventory.Items[UUID], 2);
                        return;
                    }


                    var tempComponent = new Core.WeaponComponent();
                    tempComponent.id = sourceItem.ID;
                    tempComponent.content = nInventory.ItemFdesc[sourceItem.Type];
                    tempComponent.type = nInventory.ItemFtype[sourceItem.Type];
                    tempComponent.weight = nInventory.ItemsWeight[sourceItem.Type];
                    tempComponent.count = 1;
                    tempComponent.w = nInventory.ItemSizeW[sourceItem.Type];
                    tempComponent.h = nInventory.ItemSizeH[sourceItem.Type];
                    tempComponent.Сtype = nInventory.ItemCType[sourceItem.Type];
                    weapon.WData.Components[Convert.ToInt32(nInventory.ItemFtype[sourceItem.Type])] = tempComponent;
                    var wHash = Weapons.GetWeaponHash(Main.Players[player].currentWeapon);
                    Log.Write("transfer 2: wHash: " + wHash, nLog.Type.Error);

                    List<string> tempComponents = new List<string>();




                    foreach (var ComponentType in weapon.WData.Components.Values) { tempComponents.Add(ComponentType.Сtype); }



                    player.SetSharedData("currentWeaponComponents", (uint)wHash + "." + string.Join("|", tempComponents.ToArray()));
                    var pos = player.Position;
                    var pId = player.Id;
                    foreach (var comp in tempComponents)
                    {
                        if (comp != null)
                            Trigger.ClientEventInRange(pos, 500.0f, "updatePlayerWeaponComponent", pId, (GTANetworkAPI.WeaponHash)wHash, comp, false);
                    }

                }
                else
                {
                    sourceItem.slot_id = targetSlotId;
                    TargetItems.Add(sourceItem);
                    nInventory.FillSlot(TargetSlots, sourceItem, 5);
                }

                PsendItems(player, nInventory.Items[UUID], 2);
                if (targetGroupCode == "trade" || sourceGroupCode == "trade")
                {
                    PsendItems(player, player.GetMyData<List<nItem>>("CHANGE_WITH_ITEMS"), 5);
                    PsendItems(player.GetMyData<Player>("CHANGE_WITH"), player.GetMyData<List<nItem>>("CHANGE_WITH_ITEMS"), 6);
                }
                if (targetGroupCode == "outside" || sourceGroupCode == "outside")
                {
                    if (player.GetMyData<int>("OPENOUT_TYPE") == 2)
                    {
                        Vehicle veh = player.GetMyData<Vehicle>("SELECTEDVEH");
                        veh.SetData("ITEMS", getChangeItems(player).ToList());
                        // VehicleManager.Vehicles[veh.NumberPlate].Items = TargetItems.ToList();

                    }

                    if (player.GetMyData<int>("OPENOUT_TYPE") == 6)
                    {
                        //Fraclog
                        var itemFname = nInventory.ItemFname.ContainsKey(sourceItem.Type) ? nInventory.ItemFname[sourceItem.Type] : sourceItem.Type.ToString();
                        var text = sourceGroupCode == "outside" ? $"Взял {itemFname} в количестве {sourceItem.Count}шт" : $"Положил {itemFname} в количестве {sourceItem.Count}шт";
                        Log.Debug("inventoryChangeSlot_Server FRACSTOCK: "+ text);
                        FractionLogs.FractionStock(Main.Players[player].Fraction.FractionID, player.Name, Main.Players[player].UUID.ToString(), sourceItem.Type, sourceItem.Count, StockOperationType.take, text);
                    }

                    PsendItems(player, getChangeItems(player), 4);
                }
                GameLog.Items($"player({Main.Players[player].UUID + " in " + sourceGroupCode})", $"out ({targetGroupCode})", sourceSlotId, 1, $"{targetSlotId}");
            }
            catch (Exception e) { Log.Write("inventoryTransfer_Server: " + e.StackTrace + e.Source + e.StackTrace, nLog.Type.Error); }
        }

        [RemoteEvent("inventoryDrop_Server")]
        public void inventoryDrop_Server(Player player, string sourceGroupCode, int sourceSlotId)
        {
            try
            {
                int UUID = Main.Players[player].UUID;
                List<nItem> SourceItems = nInventory.Items[UUID];
                List<bool> SourceSlots = nInventory.ItemsSlots[UUID];
                switch (sourceGroupCode)
                {
                    case "inventory":
                        SourceItems = nInventory.Items[UUID];
                        SourceSlots = nInventory.ItemsSlots[UUID];
                        break;
                    case "outside":
                        SourceItems = getChangeItems(player);
                        SourceSlots = getChangeSlots(player);
                        break;
                    case "bug":
                        SourceItems = nInventory.Items[UUID].Find(i => nInventory.ClothesItemSlots.ContainsKey(i.Type) && i.IsActive && nInventory.ClothesItemSlots[i.Type] == 12).Items;
                        SourceSlots = nInventory.Items[UUID].Find(i => nInventory.ClothesItemSlots.ContainsKey(i.Type) && i.IsActive && nInventory.ClothesItemSlots[i.Type] == 12).Slots;
                        break;
                }
                nItem sourceItem = SourceItems.Find(i => i.slot_id == sourceSlotId);
                if (sourceItem.IsActive)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Нельзя выкинуть активный предмет", 3000);
                    PsendItems(player, nInventory.Items[UUID], 2);
                    if (sourceGroupCode == "outside")
                    {
                        PsendItems(player, getChangeItems(player), 4);
                    }
                    return;
                }

                //Log.Debug($"CLEAR!? onDrop2 sourceGroupCode: {sourceGroupCode}");
                nInventory.ClearSlot(SourceSlots, sourceItem, 5);
                SourceItems.Remove(sourceItem);
                Items.onDrop(player, SourceSlots, sourceItem, null);
                PsendItems(player, nInventory.Items[UUID], 2);
                if (sourceGroupCode == "outside")
                {
                    if (player.GetMyData<int>("OPENOUT_TYPE") == 2)
                    {
                        Vehicle veh = player.GetMyData<Vehicle>("SELECTEDVEH");
                        veh.SetData("ITEMS", getChangeItems(player).ToList());
                        // VehicleManager.Vehicles[veh.NumberPlate].Items = TargetItems.ToList();

                    }
                    PsendItems(player, getChangeItems(player), 4);
                }
                GameLog.Items($"player({Main.Players[player].UUID + " in " + sourceGroupCode})", $"out (drop)", sourceSlotId, 1, $"ss");
            }
            catch (Exception e) { Log.Write("inventoryDrop_Server: " + e.StackTrace, nLog.Type.Error); }
        }

        [RemoteEvent("inventoryUse_Server")]
        public void inventoryUse_Server(Player player, string sourceGroupCode, int sourceSlotId, int targetSlotId)
        {
            try
            {
                int UUID = Main.Players[player].UUID;
                List<nItem> SourceItems = nInventory.Items[UUID];
                List<bool> SourceSlots = nInventory.ItemsSlots[UUID];
                nItem sourceItem;
                int index;

                Log.Debug("SoruceSlotID " + sourceSlotId);
                Log.Debug("targetSlotId " + targetSlotId);

                Log.Debug("OnDUTY: " + Main.Players[player].OnDuty);

                if (sourceGroupCode == "character")
                {
                    sourceItem = SourceItems.Find(i => i.character_slot_id == sourceSlotId);

                    if (nInventory.ClothesItems.Contains(sourceItem.Type) && sourceItem.Type != ItemType.BodyArmor && sourceItem.Type != ItemType.Mask)
                    {
                        if ((Main.Players[player].OnDuty && Fractions.Manager.FractionTypes[Main.Players[player].Fraction.FractionID] == 2 && Main.Players[player].Fraction.FractionID != 9) || player.GetMyData<bool>("ON_WORK"))
                        {

                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы не можете использовать это сейчас", 3000);
                            PsendItems(player, nInventory.Items[UUID], 2);
                            Close(player);
                            return;
                        }
                    }

                    index = SourceItems.IndexOf(sourceItem);
                    sourceItem.slot_id = targetSlotId;
                    sourceItem.character_slot_id = 0;
                    nInventory.FillSlot(SourceSlots, sourceItem, 5);
                    Log.Debug("index snyal" + index);
                    Items.onUse(player, sourceItem, index);
                }
                else
                {
                    sourceItem = SourceItems.Find(i => i.slot_id == sourceSlotId);

                    if (nInventory.ClothesItems.Contains(sourceItem.Type) && sourceItem.Type != ItemType.BodyArmor && sourceItem.Type != ItemType.Mask && sourceItem.Type != ItemType.BodyArmorgov1 && sourceItem.Type != ItemType.BodyArmorgov2 && sourceItem.Type != ItemType.BodyArmorgov3 && sourceItem.Type != ItemType.BodyArmorgov4)
                    {
                        if ((Main.Players[player].OnDuty && Main.Players[player].Fraction.FractionID != 0 && Fractions.Manager.FractionTypes[Main.Players[player].Fraction.FractionID] == 2 && Main.Players[player].Fraction.FractionID != 9) || player.GetMyData<bool>("ON_WORK"))
                        {

                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы не можете использовать это сейчас", 3000);
                            PsendItems(player, nInventory.Items[UUID], 2);
                            Close(player);
                            return;
                        }

                        var data = (string)sourceItem.Data;
                        var clothesGender = Convert.ToBoolean(data.Split('_')[2]);
                        if (clothesGender != Main.Players[player].Gender)
                        {
                            var error_gender = (clothesGender) ? "мужская" : "женская";
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Это {error_gender} одежда", 3000);
                            PsendItems(player, nInventory.Items[UUID], 2);
                            return;
                        }
                    }

                    if (sourceItem.Type == ItemType.Glasses)
                    {
                        if (!sourceItem.IsActive)
                        {
                            var mask = Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Mask.Variation;
                            if (Customization.MaskTypes.ContainsKey(mask) && Customization.MaskTypes[mask].Item3)
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы не можете надеть эти очки с маской", 3000);
                                PsendItems(player, nInventory.Items[UUID], 2);
                                return;
                            }
                        }
                    }
                    if (sourceItem.Type == ItemType.Hat)
                    {
                        if (!sourceItem.IsActive)
                        {
                            var mask = Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Mask.Variation;
                            if (Customization.MaskTypes.ContainsKey(mask) && Customization.MaskTypes[mask].Item2)
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы не можете надеть этот головной убор с маской", 3000);
                                PsendItems(player, nInventory.Items[UUID], 2);
                                return;
                            }
                        }
                    }

                    var gender = Main.Players[player].Gender;

                    if (nInventory.ClothesItems.Contains(sourceItem.Type))
                    {
                        var itemData = (string)sourceItem.Data;

                        if (sourceItem.Type == ItemType.Accessories)
                        {
                            if (!sourceItem.IsActive)
                            {
                                var variation = Convert.ToInt32(itemData.Split('_')[0]);
                                if (Customization.CustomPlayerData[Main.Players[player].UUID].Accessory.Watches.Variation != -1 && Customization.AccessoryRHand[gender].ContainsKey(variation))
                                {
                                    if (Customization.CustomPlayerData[Main.Players[player].UUID].Accessory.Bracelets.Variation != -1)
                                    {
                                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Заняты обе руки", 3000);
                                        PsendItems(player, nInventory.Items[UUID], 2);
                                        return;
                                    }
                                }
                                else
                                {
                                    if (Customization.CustomPlayerData[Main.Players[player].UUID].Accessory.Watches.Variation != -1)
                                    {
                                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "На вас уже надеты часы", 3000);
                                        PsendItems(player, nInventory.Items[UUID], 2);
                                        return;
                                    }
                                }
                            }
                        }

                        if (sourceItem.Type == ItemType.Undershit)
                        {
                            var underwearID = Convert.ToInt32(itemData.Split('_')[0]);
                            var underwear = Customization.Underwears[gender][underwearID];
                            if (!sourceItem.IsActive)
                            {
                                if (Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Top.Variation == Customization.EmtptySlots[gender][11])
                                {
                                    if (underwear.Top == -1)
                                    {
                                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Эту одежду можно надеть только под верхнюю", 3000);
                                        PsendItems(player, nInventory.Items[UUID], 2);
                                        return;
                                    }
                                }
                                else
                                {
                                    var nowTop = Customization.Tops[gender].FirstOrDefault(t => t.Variation == Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Top.Variation);
                                    if (nowTop != null)
                                    {
                                        var topType = nowTop.Type;
                                        if (!underwear.UndershirtIDs.ContainsKey(topType))
                                        {
                                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Эта одежда несовместима с Вашей верхней одеждой", 3000);
                                            PsendItems(player, nInventory.Items[UUID], 2);
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        if (underwear.Top == -1)
                                        {
                                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Эту одежду можно надеть только под верхнюю", 3000);
                                            PsendItems(player, nInventory.Items[UUID], 2);
                                            return;
                                        }
                                    }
                                }
                            }
                        }

                        if (sourceItem.Type == ItemType.Top)
                        {
                            if (!sourceItem.IsActive)
                            {
                                var variation = Convert.ToInt32(itemData.Split('_')[0]);
                                var undershirtVariation = Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Undershit.Variation;

                                var underwear = Customization.Underwears[gender].Values.FirstOrDefault(u => u.Top == Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Top.Variation);
                                if (underwear != null)
                                {
                                    var topType = Customization.Tops[gender].FirstOrDefault(t => t.Variation == variation).Type;
                                    if (!underwear.UndershirtIDs.ContainsKey(topType))
                                    {
                                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "На вас надета верхняя одежда или рубашка", 3000);
                                        PsendItems(player, nInventory.Items[UUID], 2);
                                        return;
                                    }
                                }

                            }
                        }

                        var tempitem = SourceItems.Find(i => nInventory.ClothesItemSlots.ContainsKey(i.Type) && i.IsActive && nInventory.ClothesItemSlots[i.Type] == nInventory.ClothesItemSlots[sourceItem.Type]);
                        if (tempitem != null)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Такая вещь уже надета", 3000);
                            return;
                        }
                    }

                    index = SourceItems.IndexOf(sourceItem);
                    Log.Debug("index odel -> " + index);
                    if (targetSlotId > 0)
                    {
                        sourceItem.character_slot_id = targetSlotId;
                        //Log.Debug("CLEAR!? Use");
                        nInventory.ClearSlot(SourceSlots, sourceItem, 5);
                        sourceItem.slot_id = 0;
                    }
                    Items.onUse(player, sourceItem, index);
                }
                //  PsendItems(player);
                GameLog.Items($"player({Main.Players[player].UUID + " in " + sourceGroupCode})", $"out (use)", sourceSlotId, 1, $"{targetSlotId}");
            }
            catch (Exception e) { Log.Write($"inventoryUse_Server player: {player.Name} : " + e.StackTrace, nLog.Type.Error); }
        }

        [RemoteEvent("inventoryChangeSlot_Server")]
        public void inventoryChangeSlot_Server(Player player, string sourceGroupCode, int sourceSlotId, int targetSlotId)
        {
            try
            {
                int UUID = Main.Players[player].UUID;
                List<nItem> SourceItems = nInventory.Items[UUID];
                List<bool> SourceSlots = nInventory.ItemsSlots[UUID];
                switch (sourceGroupCode)
                {
                    case "inventory":
                        SourceItems = nInventory.Items[UUID];
                        SourceSlots = nInventory.ItemsSlots[UUID];
                        break;
                    case "outside":
                        SourceItems = getChangeItems(player);
                        SourceSlots = getChangeSlots(player);
                        break;
                    case "bug":
                        SourceItems = nInventory.Items[UUID].Find(i => nInventory.ClothesItemSlots.ContainsKey(i.Type) && i.IsActive && nInventory.ClothesItemSlots[i.Type] == 12).Items;
                        SourceSlots = nInventory.Items[UUID].Find(i => nInventory.ClothesItemSlots.ContainsKey(i.Type) && i.IsActive && nInventory.ClothesItemSlots[i.Type] == 12).Slots;
                        break;
                }
                nItem sourceItem = SourceItems.Find(i => i.slot_id == sourceSlotId);
                //Log.Debug("CLEAR!? ChangeSlot");

                if (sourceItem == null) return;

                nInventory.ClearSlot(SourceSlots, sourceItem, 5);
                sourceItem.slot_id = targetSlotId;
                nInventory.FillSlot(SourceSlots, sourceItem, 5);
                PsendItems(player, nInventory.Items[UUID], 2);
                if (sourceGroupCode == "outside")
                {
                    if (player.GetMyData<int>("OPENOUT_TYPE") == 2)
                    {
                        Vehicle veh = player.GetMyData<Vehicle>("SELECTEDVEH");
                        veh.SetData("ITEMS", getChangeItems(player).ToList());
                        // VehicleManager.Vehicles[veh.NumberPlate].Items = TargetItems.ToList();
                    }
                    PsendItems(player, getChangeItems(player), 4);
                }
                GameLog.Items($"player({Main.Players[player].UUID + " in " + sourceGroupCode})", $"out (changeslot)", sourceSlotId, 1, $"{targetSlotId}");
            }
            catch (Exception e) { Log.Write("inventoryChangeSlot_Server: " + e.StackTrace, nLog.Type.Error); }
        }

        [RemoteEvent("inventorySeparate_Server")]
        public void inventorySeparate_Server(Player player, int sourceSlotId, int count, int targetSlotId, string targetGroupCode, string sourceGroupCode)
        {
            try
            {
                Log.Debug($"[Separate_Server][{player.Name}] sourceSlotId: {sourceSlotId} targetSlotId: {targetSlotId} count: {count} targetGroupCode: {targetGroupCode} sourceGroupCode: {sourceGroupCode}");
                int UUID = Main.Players[player].UUID;
                List<nItem> SourceItems = nInventory.Items[UUID];
                List<bool> SourceSlots = nInventory.ItemsSlots[UUID];
                List<nItem> TargetItems = nInventory.Items[UUID];
                List<bool> TargetSlots = nInventory.ItemsSlots[UUID];

                if (count < 0) return;

                switch (sourceGroupCode)
                {
                    case "inventory":
                        SourceItems = nInventory.Items[UUID];
                        SourceSlots = nInventory.ItemsSlots[UUID];
                        break;
                    case "outside":
                        SourceItems = getChangeItems(player);
                        SourceSlots = getChangeSlots(player);
                        break;
                    case "bug":
                        SourceItems = nInventory.Items[UUID].Find(i => nInventory.ClothesItemSlots.ContainsKey(i.Type) && i.IsActive && nInventory.ClothesItemSlots[i.Type] == 12).Items;
                        SourceSlots = nInventory.Items[UUID].Find(i => nInventory.ClothesItemSlots.ContainsKey(i.Type) && i.IsActive && nInventory.ClothesItemSlots[i.Type] == 12).Slots;
                        break;
                    case "trade":
                        SourceItems = getChangeItems(player);
                        SourceSlots = getChangeSlots(player);
                        break;
                }
                switch (targetGroupCode)
                {
                    case "inventory":
                        TargetItems = nInventory.Items[UUID];
                        TargetSlots = nInventory.ItemsSlots[UUID];
                        break;
                    case "outside":
                        TargetItems = getChangeItems(player);
                        TargetSlots = getChangeSlots(player);
                        break;
                    case "bug":
                        TargetItems = nInventory.Items[UUID].Find(i => nInventory.ClothesItemSlots.ContainsKey(i.Type) && i.IsActive && nInventory.ClothesItemSlots[i.Type] == 12).Items;
                        TargetSlots = nInventory.Items[UUID].Find(i => nInventory.ClothesItemSlots.ContainsKey(i.Type) && i.IsActive && nInventory.ClothesItemSlots[i.Type] == 12).Slots;
                        break;
                    case "trade":
                        TargetItems = getChangeItems(player);
                        TargetSlots = getChangeSlots(player);
                        break;
                }

                if (SourceItems == null || SourceSlots == null) {
                    Log.Debug($"[{player.Name}] NOT FOUND Items or SLOTS");
                    return;
                }

                if (TargetItems == null || TargetSlots == null) {
                    Log.Debug($"[{player.Name}] NOT FOUND Items or SLOTS");
                    return;
                }

                nItem sourceItem = SourceItems.Find(i => i.slot_id == sourceSlotId);

                if (sourceItem == null) return;
                if (sourceItem.Count - count < 0) return;

                sourceItem.Count = sourceItem.Count - count;
                sourceItem.Weight -= nInventory.ItemsWeight[sourceItem.Type] * count;
                nItem newitem = new nItem(sourceItem.Type, count);
                newitem.slot_id = targetSlotId;
                TargetItems.Add(newitem);
                nInventory.FillSlot(TargetSlots, newitem, 5);
                PsendItems(player, nInventory.Items[UUID], 2);
                if (targetGroupCode == "trade" || sourceGroupCode == "trade")
                {
                    PsendItems(player, player.GetMyData<List<nItem>>("CHANGE_WITH_ITEMS"), 5);
                    PsendItems(player.GetMyData<Player>("CHANGE_WITH"), player.GetMyData<List<nItem>>("CHANGE_WITH_ITEMS"), 6);
                }
                if (targetGroupCode == "outside" || sourceGroupCode == "outside")
                {
                    if (player.GetMyData<int>("OPENOUT_TYPE") == 2)
                    {
                        Vehicle veh = player.GetMyData<Vehicle>("SELECTEDVEH");
                        veh.SetData("ITEMS", getChangeItems(player).ToList());
                        // VehicleManager.Vehicles[veh.NumberPlate].Items = TargetItems.ToList();

                    }
                    PsendItems(player, getChangeItems(player), 4);
                }
                GameLog.Items($"player({Main.Players[player].UUID + " separate in " + sourceGroupCode})", $"out ({targetGroupCode})", sourceSlotId, 1, $"{targetSlotId}");
            }
            catch (Exception e) { Log.Write("inventorySeparate_Server: " + e.StackTrace + e.StackTrace, nLog.Type.Error); }
        }

        [RemoteEvent("fastSlot")]
        public static void RemoteEvent_fastSlot(Player player, int key)
        {
            try
            {
                if (!Main.Players.ContainsKey(player) || !nInventory.Items.ContainsKey(Main.Players[player].UUID) || player.IsInVehicle) return;
                var UUID = Main.Players[player].UUID;
                switch (key)
                {
                    case 1:
                        {
                            var fItem = nInventory.Items[UUID].FirstOrDefault(i => i.fast_slot_id == 1);
                            if (fItem != null)
                            {
                                var index = nInventory.Items[UUID].IndexOf(fItem);

                                if (nInventory.ClothesItems.Contains(fItem.Type))
                                {
                                    var hasActiveItem = nInventory.Items[UUID].FirstOrDefault(i => i.character_slot_id == nInventory.ClothesItemSlots[fItem.Type]);
                                    if (hasActiveItem == null)
                                    {
                                        fItem.fast_slot_id = 0;
                                        fItem.character_slot_id = nInventory.ClothesItemSlots[fItem.Type];
                                    } else
                                    {
                                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы не можете одеть это", 3000);
                                        return;
                                    }
                                    //fItem.fast_slot_id = 0;
                                    //fItem.character_slot_id = nInventory.ClothesItemSlots[fItem.Type];
                                }
                                Items.onUse(player, fItem, index);
                            }
                        }
                        return;
                    case 2:
                        {
                            var fItem = nInventory.Items[UUID].FirstOrDefault(i => i.fast_slot_id == 2);
                            if (fItem != null)
                            {
                                var index = nInventory.Items[UUID].IndexOf(fItem);

                                if (nInventory.ClothesItems.Contains(fItem.Type))
                                {
                                    var hasActiveItem = nInventory.Items[UUID].FirstOrDefault(i => i.character_slot_id == nInventory.ClothesItemSlots[fItem.Type]);
                                    if (hasActiveItem == null)
                                    {
                                        fItem.fast_slot_id = 0;
                                        fItem.character_slot_id = nInventory.ClothesItemSlots[fItem.Type];
                                    } else
                                    {
                                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы не можете одеть это", 3000);
                                        return;
                                    }
                                    //fItem.fast_slot_id = 0;
                                    //fItem.character_slot_id = nInventory.ClothesItemSlots[fItem.Type];
                                }
                                Items.onUse(player, fItem, index);
                            }
                        }
                        return;
                    case 3:
                        {
                            var fItem = nInventory.Items[UUID].FirstOrDefault(i => i.fast_slot_id == 3);
                            if (fItem != null)
                            {
                                var index = nInventory.Items[UUID].IndexOf(fItem);

                                if (nInventory.ClothesItems.Contains(fItem.Type))
                                {
                                    var hasActiveItem = nInventory.Items[UUID].FirstOrDefault(i => i.character_slot_id == nInventory.ClothesItemSlots[fItem.Type]);
                                    if (hasActiveItem == null)
                                    {
                                        fItem.fast_slot_id = 0;
                                        fItem.character_slot_id = nInventory.ClothesItemSlots[fItem.Type];
                                    } else
                                    {
                                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы не можете одеть это", 3000);
                                        return;
                                    }
                                    //fItem.fast_slot_id = 0;
                                    //fItem.character_slot_id = nInventory.ClothesItemSlots[fItem.Type];
                                }
                                Items.onUse(player, fItem, index);
                            }
                        }
                        return;
                    case 4:
                        {
                            var fItem = nInventory.Items[UUID].FirstOrDefault(i => i.fast_slot_id == 4);
                            if (fItem != null)
                            {
                                var index = nInventory.Items[UUID].IndexOf(fItem);

                                if (nInventory.ClothesItems.Contains(fItem.Type))
                                {
                                    var hasActiveItem = nInventory.Items[UUID].FirstOrDefault(i => i.character_slot_id == nInventory.ClothesItemSlots[fItem.Type]);
                                    if (hasActiveItem == null)
                                    {
                                        fItem.fast_slot_id = 0;
                                        fItem.character_slot_id = nInventory.ClothesItemSlots[fItem.Type];
                                    } else
                                    {
                                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы не можете одеть это", 3000);
                                        return;
                                    }
                                    //fItem.fast_slot_id = 0;
                                    //fItem.character_slot_id = nInventory.ClothesItemSlots[fItem.Type];
                                }
                                Items.onUse(player, fItem, index);
                            }
                        }
                        return;
                }
            }
            catch (Exception e) { Log.Write("fastSlot: " + e.StackTrace, nLog.Type.Error); }
        }

        [RemoteEvent("inventorySendtrademoney_Server")]
        public void inventorySendtrademoney_Server(Player player, params object[] arguments)
        {
            try
            {
                if (!player.HasMyData("CHANGE_WITH"))
                {
                    Close(player);
                    return;
                }
                Player target = player.GetMyData<Player>("CHANGE_WITH");
                if (!Main.Players.ContainsKey(target) || player.Position.DistanceTo(target.Position) > 2)
                {
                    Close(player);
                    return;
                }
                if (Convert.ToInt32(arguments[0]) > Main.Players[player].Money)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У вас нет столько денег", 3000);
                    return;
                }
                player.SetMyData("trademoney", Convert.ToInt32(arguments[0]));
                Trigger.ClientEvent(target, "inventory", 8, arguments[0]);
            }
            catch (Exception e) { Log.Write("inventorySendtrademoney_Server: " + e.StackTrace, nLog.Type.Error); }
        }

        [RemoteEvent("inventorytradeAccept_Server")]
        public void inventorytradeAccept_Server(Player player)
        {
            try
            {
                if (!player.HasMyData("CHANGE_WITH"))
                {
                    Close(player);
                    return;
                }

                Player target = player.GetMyData<Player>("CHANGE_WITH");
                if (!Main.Players.ContainsKey(target) || player.Position.DistanceTo(target.Position) > 2)
                {
                    Close(player);
                    return;
                }
                if (!target.HasMyData("Trade_Accept"))
                {
                    player.SetMyData("Trade_Accept", true);
                    Trigger.ClientEvent(target, "inventory", 7);
                }
                else
                {
                    if (player.HasMyData("trademoney") && player.GetMyData<int>("trademoney") > Main.Players[player].Money)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У вас нет столько денег", 3000);
                        Close(player);
                        return;
                    }
                    if (target.HasMyData("trademoney") && target.GetMyData<int>("trademoney") > Main.Players[target].Money)
                    {
                        Notify.Send(target, NotifyType.Error, NotifyPosition.BottomCenter, $"У вас нет столько денег", 3000);
                        Close(target);
                        return;
                    }
                    var tempslotsplayer = new List<bool>(nInventory.ItemsSlots[Main.Players[player].UUID]);
                    foreach (var item in target.GetMyData<List<nItem>>("CHANGE_WITH_ITEMS"))
                    {
                        int slot = nInventory.FindFreeSlot(item, tempslotsplayer, 10);
                        if (slot == -1)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Нет места в инвентаре", 3000);
                            Close(player);
                            return;
                        }
                        else
                        {
                            nInventory.FillSlot(tempslotsplayer, item, 10);
                        }
                    }
                    var tempslotstarget = new List<bool>(nInventory.ItemsSlots[Main.Players[target].UUID]);
                    foreach (var item in player.GetMyData<List<nItem>>("CHANGE_WITH_ITEMS"))
                    {
                        int slot = nInventory.FindFreeSlot(item, tempslotstarget, 10);
                        if (slot == -1)
                        {
                            Notify.Send(target, NotifyType.Error, NotifyPosition.BottomCenter, $"Нет места в инвентаре", 3000);
                            Close(player);
                            return;
                        }
                        else
                        {
                            nInventory.FillSlot(tempslotstarget, item, 10);
                        }
                    }
                    player.SetMyData("Trade_Accept", true);
                    if (player.HasMyData("trademoney"))
                    {
                        Wallet.Change(player, -player.GetMyData<int>("trademoney"));
                        Wallet.Change(target, player.GetMyData<int>("trademoney"));
                    }
                    if (target.HasMyData("trademoney"))
                    {
                        Wallet.Change(target, -target.GetMyData<int>("trademoney"));
                        Wallet.Change(player, target.GetMyData<int>("trademoney"));
                    }
                    if (player.GetMyData<List<nItem>>("CHANGE_WITH_ITEMS").Count > 0)
                    {
                        foreach (var item in player.GetMyData<List<nItem>>("CHANGE_WITH_ITEMS"))
                            nInventory.Add(target, item);
                    }
                    if (target.GetMyData<List<nItem>>("CHANGE_WITH_ITEMS").Count > 0)
                    {
                        foreach (var item in target.GetMyData<List<nItem>>("CHANGE_WITH_ITEMS"))
                            nInventory.Add(player, item);
                    }
                    player.ResetMyData("CHANGE_WITH_ITEMS");
                    player.ResetMyData("CHANGE_WITH_SLOTS");
                    player.ResetMyData("Trade_Accept");
                    player.ResetMyData("trademoney");
                    target.ResetMyData("CHANGE_WITH_ITEMS");
                    target.ResetMyData("CHANGE_WITH_SLOTS");
                    target.ResetMyData("Trade_Accept");
                    target.ResetMyData("trademoney");
                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Обмен завершен", 3000);
                    Notify.Send(target, NotifyType.Success, NotifyPosition.BottomCenter, $"Обмен завершен", 3000);

                    #region BPКвест: 117  Совершить обмен предметами с игроком.

                    #region BattlePass выполнение квеста
                    BattlePass.updateBPQuestIteration(player, BattlePass.BPQuestType.TradeWithPlayer);
                    BattlePass.updateBPQuestIteration(target, BattlePass.BPQuestType.TradeWithPlayer);
                    #endregion

                    #endregion

                    Close(player);
                }

            }
            catch (Exception e) { Log.Write("tradeAccept: " + e.StackTrace, nLog.Type.Error); }
        }
    }
}
