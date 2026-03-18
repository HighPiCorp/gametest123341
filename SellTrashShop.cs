using System;
using System.Collections.Generic;
using System.Text;
using client.Systems.CraftSystem;
using GTANetworkAPI;
using NeptuneEvo;
using NeptuneEvo.Core;
using NeptuneEvo.SDK;
using Newtonsoft.Json;
using static client.GUI.ListSystem;
using Trigger = NeptuneEvo.Trigger;

namespace NeptuneEvo.Core
{
    public class SellTrashShop : Script
    {
        private static nLog Log = new nLog("SellTrashShop");

        public static Dictionary<ItemType, int> SellPrices = new Dictionary<ItemType, int>();

        private static Dictionary<ItemType, int> MinSellPrices = new Dictionary<ItemType, int>()
        {
            { ItemType.CraftCopperWire, 700 },
            { ItemType.CraftOldJewerly, 850 },
            { ItemType.CraftGoldNugget, 1350 },
            { ItemType.CraftСollectibleCoin, 2540 },
            { ItemType.CraftAncientStatuette, 2960},
            { ItemType.CraftGoldHorseShoe, 6400 },
            { ItemType.CraftRelic, 21560 },
        };

        private static Dictionary<ItemType, int> MaxSellPrices = new Dictionary<ItemType, int>()
        {
            { ItemType.CraftCopperWire, 1200 },
            { ItemType.CraftOldJewerly, 1400 },
            { ItemType.CraftGoldNugget, 2850 },
            { ItemType.CraftСollectibleCoin, 5700 },
            { ItemType.CraftAncientStatuette,  7850},
            { ItemType.CraftGoldHorseShoe, 12400 },
            { ItemType.CraftRelic, 29700 },
        };


        [ServerEvent(Event.ResourceStart)]
        public void ResourceStart()
        {
            SetTrashPrices();
        }

        public static void SetTrashPrices()
        {
            try { 
            Random rand = new Random();
            Dictionary<ItemType, int> maxSellPrices = new Dictionary<ItemType, int>();
            foreach (var pair in MinSellPrices)
            {
                maxSellPrices.Add(pair.Key,rand.Next(MinSellPrices[pair.Key], MaxSellPrices[pair.Key]));
                //Log.Write($"{nInventory.ItemsNames[(int)pair.Key]} {SellPrices[i][pair.Key]}");
            }
            SellPrices = new Dictionary<ItemType, int>(maxSellPrices);
            }catch (Exception ex) { Log.Write(ex.StackTrace); }
        }

        public static void OpenBizSellShopMenu(Player player)
        {
            try
            {
                if (!player.HasMyData("SELL_ID")) return;

                int id = player.GetMyData<int>("SELL_ID");

                Log.Write($"{id}");

                var menu = new Dictionary<string, object>();
                var list = new List<Dictionary<string, object>>();

                foreach (ItemType type in SellPrices.Keys)
                {
                    var dict = new Dictionary<string, object>();

                    int cntVeh = nInventory.FindCount(player, type);
                    if (cntVeh > 0)
                    {
                        dict.Add("name", nInventory.ItemFname[type]);
                        dict.Add("key", (int)type);
                        dict.Add("count", cntVeh);
                        dict.Add("price", SellPrices[type]);

                        list.Add(dict);
                    }
                }

                Log.Debug(JsonConvert.SerializeObject(list));
                string name = "Скупщик сокровищ";
                Trigger.ClientEvent(player, "CLIENT::fishbuyer:open", JsonConvert.SerializeObject(list), 1,name);
            }
            catch (Exception e) { Log.Write("OpenBizSellShopMenu: " + e.StackTrace, nLog.Type.Error); }
        }

        [RemoteEvent("SERVER::trashbuyer:sellAll")]
        private static void RemoteEvent_sellAll(Player player)
        {
            try
            {
                if (!player.HasMyData("SELL_ID")) return;

                int id = player.GetMyData<int>("SELL_ID");

                var price = 0;

                foreach (ItemType type in SellPrices.Keys)
                {
                    var dict = new Dictionary<string, object>();

                    int cntVeh = nInventory.FindCount(player, type);
                    if (cntVeh > 0)
                    {
                        price += SellPrices[type] * cntVeh;
                        nInventory.Remove(player, type, cntVeh);
                    }
                }

                MoneySystem.Wallet.Change(player, +price);
                GameLog.Money($"server", $"player({Main.Players[player].UUID})", price, $"fishSellAll({price})");
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы продали всё за {price}$", 3000);

            }
            catch (Exception e) { Log.Write("SERVER::fishbuyer:sellAll " + e.StackTrace, nLog.Type.Error); }
        }

        [RemoteEvent("SERVER::trashbuyer:sell")]
        private static void RemoteEvent_sell(Player player, int key, int count)
        {
            try
            {
                if (!player.HasMyData("SELL_ID")) return;

                int id = player.GetMyData<int>("SELL_ID");

                var price = 0;
                var name = "";

                var type = (ItemType)key;

                Log.Debug("type: " + type + " key: " + key);

                if (SellPrices.ContainsKey(type))
                {
                    int cntVeh = nInventory.FindCount(player, type);
                    Log.Debug("cntVeh: " + cntVeh + " count: " + count);
                    if (cntVeh >= count)
                    {
                        name = nInventory.ItemFname[type];
                        price = SellPrices[type] * count;
                        nInventory.Remove(player, type, count);
                    }

                    MoneySystem.Wallet.Change(player, +price);
                    GameLog.Money($"server", $"player({Main.Players[player].UUID})", price, $"fishSell({price})");
                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы продали {name} за {price}$", 3000);
                }
            }
            catch (Exception e) { Log.Write("SERVER::fishbuyer:sellAll " + e.StackTrace, nLog.Type.Error); }
        }
    }
}
