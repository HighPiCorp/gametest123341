using System;
using System.Collections.Generic;
using System.Text;
using client.Systems.BattlePass;
using GTANetworkAPI;
using NeptuneEvo;
using NeptuneEvo.Core;
using NeptuneEvo.SDK;
using Newtonsoft.Json;
using static client.GUI.ListSystem;
using Trigger = NeptuneEvo.Trigger;

namespace NeptuneEvo.Core
{
  public class SellFishShop : Script
  {
        private static nLog Log = new nLog("SellFishShop");

        public static List<Dictionary<ItemType, int>> SellPrices = new List<Dictionary<ItemType, int>>()
        {
            new Dictionary<ItemType, int>(){
                { ItemType.Keta, 14 },
                { ItemType.Gorbysha, 14 },
                { ItemType.Kyndja, 14 },
                { ItemType.Sig, 25 },
                { ItemType.Omyl, 40 },
                { ItemType.Nerka, 52 },
                { ItemType.Forel, 65 },
                { ItemType.Ship, 81 },
                { ItemType.Lopatonos, 95 },
                { ItemType.Osetr, 95 },
                { ItemType.Semga, 100 },
                { ItemType.Servyga, 120 },
                { ItemType.Beluga, 135 },
                { ItemType.Taimen, 387 },
                { ItemType.Sterlyad, 942 },
                { ItemType.Ydilshik, 1965 },
                { ItemType.Hailiod, 2340 },
            },
            new Dictionary<ItemType, int>(){
                { ItemType.Keta, 14 },
                { ItemType.Gorbysha, 14 },
                { ItemType.Kyndja, 14 },
                { ItemType.Sig, 25 },
                { ItemType.Omyl, 40 },
                { ItemType.Nerka, 52 },
                { ItemType.Forel, 65 },
                { ItemType.Ship, 81 },
                { ItemType.Lopatonos, 95 },
                { ItemType.Osetr, 95 },
                { ItemType.Semga, 100 },
                { ItemType.Servyga, 120 },
                { ItemType.Beluga, 135 },
                { ItemType.Taimen, 387 },
                { ItemType.Sterlyad, 942 },
                { ItemType.Ydilshik, 1965 },
                { ItemType.Hailiod, 2340 },
            },
            new Dictionary<ItemType, int>(){
                { ItemType.Keta, 14 },
                { ItemType.Gorbysha, 14 },
                { ItemType.Kyndja, 14 },
                { ItemType.Sig, 25 },
                { ItemType.Omyl, 40 },
                { ItemType.Nerka, 52 },
                { ItemType.Forel, 65 },
                { ItemType.Ship, 81 },
                { ItemType.Lopatonos, 95 },
                { ItemType.Osetr, 95 },
                { ItemType.Semga, 100 },
                { ItemType.Servyga, 120 },
                { ItemType.Beluga, 135 },
                { ItemType.Taimen, 387 },
                { ItemType.Sterlyad, 942 },
                { ItemType.Ydilshik, 1965 },
                { ItemType.Hailiod, 2340 },
            }
        };

        private static Dictionary<ItemType, int> MinSellPrices = new Dictionary<ItemType, int>()
        {
            { ItemType.Keta, 29 },
            { ItemType.Gorbysha, 29 },
            { ItemType.Kyndja, 29 },
            { ItemType.Sig, 36 }, 
            { ItemType.Omyl, 45 },
            { ItemType.Nerka, 55 },
            { ItemType.Forel, 70 },
            { ItemType.Ship, 81 },
            { ItemType.Lopatonos, 95 }, 
            { ItemType.Osetr, 95 },
            { ItemType.Semga, 100 },
            { ItemType.Servyga, 120 },
            { ItemType.Beluga, 135 },
            { ItemType.Taimen, 387 },
            { ItemType.Sterlyad, 942 },
            { ItemType.Ydilshik, 1965 },
            { ItemType.Hailiod, 2340 },
        };

        private static Dictionary<ItemType, int> MaxSellPrices = new Dictionary<ItemType, int>()
        {
            { ItemType.Keta, 36 },
            { ItemType.Gorbysha, 36 },
            { ItemType.Kyndja, 36 },
            { ItemType.Sig, 45 },
            { ItemType.Omyl, 55 },
            { ItemType.Nerka, 70 },
            { ItemType.Forel, 80 },
            { ItemType.Ship, 90 },
            { ItemType.Lopatonos, 110 },
            { ItemType.Osetr, 129 },
            { ItemType.Semga, 135 },
            { ItemType.Servyga, 160 },
            { ItemType.Beluga, 174 },
            { ItemType.Taimen, 641 },
            { ItemType.Sterlyad, 2125 },
            { ItemType.Ydilshik, 4732 },
            { ItemType.Hailiod, 5000},
        };

        public static List<Vector3> SellFishShopPositions = new List<Vector3>() {
            new Vector3(4922.1714, -5240.954, 2.5234566),
            new Vector3(791.5422, 2176.4436, 52.64838),
            new Vector3(1429.4457, 6358.134, 23.98502),
        };

        [ServerEvent(Event.ResourceStart)]
        public void ResourceStart()
        {
            for (int i = 0; i < SellFishShopPositions.Count; i++)
            {
                try { 
                NAPI.Marker.CreateMarker(1, SellFishShopPositions[i] - new Vector3(0, 0, 2 - 0.3f), new Vector3(), new Vector3(), 2, new Color(255, 255, 255, 220), false, 0);

                NAPI.Blip.CreateBlip(628, SellFishShopPositions[i], 1, Convert.ToByte(5), Main.StringToU16("Продажа рыбы"), 255, 0, true);

                var shape = NAPI.ColShape.CreateCylinderColShape(SellFishShopPositions[i] - new Vector3(0, 0, 1), 2, 4, NAPI.GlobalDimension);
                shape.SetData("ID", i);
                shape.OnEntityEnterColShape += (shape, entity) =>
                {
                    entity.SetMyData("INTERACTIONCHECK", 1300);
                    entity.SetMyData("SELL_ID", shape.GetData<int>("ID"));
                };
                shape.OnEntityExitColShape += (shape, entity) =>
                {
                    Trigger.ClientEvent(entity, "CLIENT::fishbuyer:close");
                    entity.SetMyData("INTERACTIONCHECK", 0);
                    entity.ResetMyData("SELL_ID");
                };
                }catch (Exception ex) { Log.Write(ex.StackTrace); }

            }

            SetFishPrices();
        }

        public static void SetFishPrices()
        {
            try { 
            Random rand = new Random();
            float priceX = 1.35f;
            for (int i = 0; i < SellFishShopPositions.Count; i++)
            {
                Dictionary<ItemType, int> dict = new Dictionary<ItemType, int>(SellPrices[i]);

                foreach (KeyValuePair<ItemType, int> pair in dict)
                {
                    SellPrices[i][pair.Key] = (int)Math.Floor(rand.Next(MinSellPrices[pair.Key], MaxSellPrices[pair.Key]) * priceX);
                    //Log.Write($"{nInventory.ItemsNames[(int)pair.Key]} {SellPrices[i][pair.Key]}");
                }
            }
            }catch (Exception ex) { Log.Write(ex.StackTrace); }
        }

        public static void OpenBizSellShopMenu(Player player)
        {
            try { 
                if (!player.HasMyData("SELL_ID")) return;

                int id = player.GetMyData<int>("SELL_ID");

                Log.Write($"{id}");

                var menu = new Dictionary<string, object>();
                var list = new List<Dictionary<string, object>>();

                foreach (ItemType type in nInventory.FishItems)
                {
                    var dict = new Dictionary<string, object>();

                    int cntVeh = nInventory.FindCount(player, type);
                    if (cntVeh > 0)
                    {
                        dict.Add("name", nInventory.ItemFname[type]);
                        dict.Add("key", (int)type);
                        dict.Add("count", cntVeh);
                        dict.Add("price", SellPrices[id][type]);

                        list.Add(dict);
                    }
                }

                //Log.Debug(JsonConvert.SerializeObject(list));

                Trigger.ClientEvent(player, "CLIENT::fishbuyer:open", JsonConvert.SerializeObject(list), 0);
            }
            catch(Exception e) { Log.Write("OpenBizSellShopMenu: " + e.StackTrace, nLog.Type.Error); }
        }

        [RemoteEvent("SERVER::fishbuyer:sellAll")]
        private static void RemoteEvent_sellAll(Player player)
        {
            try
            {
                if (!player.HasMyData("SELL_ID")) return;

                int id = player.GetMyData<int>("SELL_ID");

                var price = 0;

                foreach (ItemType type in nInventory.FishItems)
                {
                    var dict = new Dictionary<string, object>();

                    int cntVeh = nInventory.FindCount(player, type);
                    if (cntVeh > 0)
                    {
                        price += SellPrices[id][type] * cntVeh;
                        nInventory.Remove(player, type, cntVeh);
                    }
                }

                MoneySystem.Wallet.Change(player, +price);
                GameLog.Money($"server", $"player({Main.Players[player].UUID})", price, $"fishSellAll({price})");
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы продали всю рыбу за {price}$", 3000);

                #region GBPКвест: 30 Продать рыбы на 1.000.000$.

                #region BattlePass выполнение квеста
                BattlePass.updateBPGlobalQuestIteration(player, BattlePass.BPGlobalQuestType.SellFishOnCountMoney, price);
                #endregion

                #endregion
            }
            catch(Exception e) { Log.Write("SERVER::fishbuyer:sellAll " + e.StackTrace, nLog.Type.Error); }
        }

        [RemoteEvent("SERVER::fishbuyer:sell")]
        private static void RemoteEvent_sell(Player player, int key, int count)
        {
            try
            {
                if (!player.HasMyData("SELL_ID")) return;

                int id = player.GetMyData<int>("SELL_ID");

                var price = 0;
                var name = "";

                var type = (ItemType)key;

                //Log.Debug("type: " + type + " key: "+key);

                if (nInventory.FishItems.Contains(type))
                {
                    int cntVeh = nInventory.FindCount(player, type);
                    Log.Debug("cntVeh: " + cntVeh + " count: "+ count);
                    if (cntVeh >= count)
                    {
                        name = nInventory.ItemFname[type];
                        price = SellPrices[id][type] * count;
                        nInventory.Remove(player, type, count);
                    }

                    MoneySystem.Wallet.Change(player, +price);
                    GameLog.Money($"server", $"player({Main.Players[player].UUID})", price, $"fishSell({price})");
                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы продали {name} за {price}$", 3000);

                    #region GBPКвест: 30 Продать рыбы на 1.000.000$.

                    #region BattlePass выполнение квеста
                    BattlePass.updateBPGlobalQuestIteration(player, BattlePass.BPGlobalQuestType.SellFishOnCountMoney, price);
                    #endregion

                    #endregion
                }
            }
            catch(Exception e) { Log.Write("SERVER::fishbuyer:sellAll " + e.StackTrace, nLog.Type.Error); }
        }
    }
}
