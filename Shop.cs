using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;
using NeptuneEvo.SDK;
using Newtonsoft.Json;
using System.Data;
using NeptuneEvo.GUI;
using System.Diagnostics;
using System.Timers;
using NeptuneEvo.MoneySystem;
using static NeptuneEvo.Core.BusinessManager;

namespace NeptuneEvo.Core
{
    class Shop : Script
    {
        private static nLog Log = new nLog("Shop247"); //"24/7" "Бургерная" "Рыболовный магазин"

        #region Массивы
            #region
            public static Dictionary<string, ItemType> productItemTypes = new Dictionary<string, ItemType>() {
                {"Монтировка", ItemType.Crowbar},
                {"Фонарик", ItemType.Flashlight},
                {"Молоток", ItemType.Hammer},
                {"Гаечный ключ", ItemType.Wrench},
                {"Канистра бензина", ItemType.GasCan},
                {"Чипсы", ItemType.Сrisps},
                {"Пицца", ItemType.Pizza},
                {"Бургер", ItemType.Burger},
                {"Хот-Дог", ItemType.HotDog},
                {"Сэндвич", ItemType.Sandwich},
                {"eCola", ItemType.eCola},
                {"Sprunk", ItemType.Sprunk},
                {"Связка ключей", ItemType.KeyRing},
                {"Удочка", ItemType.Rod},
                {"Удочка MK2", ItemType.RodUpgrade},
                {"Наживка", ItemType.Naz},
                {"Рюкзак", ItemType.Bag},
                {"Сим-карта", ItemType.SimCard},
                {"Бутылка воды", ItemType.WaterBottle },
                {"Рем. набор", ItemType.RepairBox },
                {"Малая аптечка", ItemType.SmallHealthKit }
            };
            #endregion
        #endregion

        public static void openShop(Player player) {
            Business biz = BizList[player.GetMyData<int>("BIZ_ID")];
            var shopProductsList = new List<List<Dictionary<string, object>>>();

            var title = "Магазин 24/7";
            switch(biz.Type) {
                case 0:
                  title = "Магазин 24/7";
                  break;
                case 8:
                  title = "Бургерная";
                  break;
                case 15:
                  title = "Рыболовный магазин";
                  break;
            }

            var shopProductsPage = new List<Dictionary<string, object>>();
            int index = 0;
            foreach (var product in biz.Products)
            {
                try { 
                if (!productItemTypes.ContainsKey(product.Name)) {
                    index++;
                    continue;
                }

                var itemType = productItemTypes[product.Name];

                var item = new Dictionary<string, object>();
                var option = new List<object>();

                var weight = new Dictionary<string, object>() {
                    {"value", nInventory.ItemsWeight[itemType]},
                };

                option.Add(weight);

                if (EatManager.ConsumeItemEatAndWater.ContainsKey(itemType))
                {
                    var water = new Dictionary<string, object>();
                    water.Add("img", "drink");
                    water.Add("value", EatManager.ConsumeItemEatAndWater[itemType].Water);
                    option.Add(water);

                    var eat = new Dictionary<string, object>();
                    eat.Add("img", "food");
                    eat.Add("value", EatManager.ConsumeItemEatAndWater[itemType].Eat);
                    option.Add(eat);
                }

                item.Add("key", (int)itemType);
                item.Add("index", index);
                item.Add("title", product.Name);
                item.Add("option", option);

                item.Add("rare", 0);
                item.Add("price", biz.GetPriceWithMarkUpInt(product.Price));

                shopProductsPage.Add(item);
                index++;
                }
                catch (Exception ex) { Log.Write(ex.StackTrace); }
            }
            shopProductsList.Add(shopProductsPage);

            //Log.Write($"{JsonConvert.SerializeObject(shopProductsList)}");

            Log.Debug("shop: "+ JsonConvert.SerializeObject(shopProductsList));
            Trigger.ClientEvent(player, "CLIENT::shop:open", title, JsonConvert.SerializeObject(shopProductsList));
        }

        [RemoteEvent("SERVER::shop:buy")]
        public static void Event_ShopCallback(Player player, int buyType, int key, int index, int count)
        {
            try
            {
                if (!Main.Players.ContainsKey(player)) return;
                if (player.GetMyData<int>("BIZ_ID") == -1) return;

                if (count <= 0)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Указано неверное кол-во", 3000);
                    return;
                }

                Business biz = BizList[player.GetMyData<int>("BIZ_ID")];

                var prod = biz.Products[index];

                bool isSimCard = false;
                bool isBag = false;
                object dataBag = null;
                nItem item = null;

                if (prod.Name == "Сим-карта") {
                    isSimCard = true;
                    count = 1;
                }
                int summaryPrice = biz.GetPriceWithMarkUpInt(prod.Price) * count;

                var amount = summaryPrice / biz.GetPriceWithMarkUpInt(prod.Price);
                if (amount <= 0) amount = 1;

                var hasItemType = productItemTypes.ContainsKey(prod.Name);
                if (hasItemType)
                {
                    var itemType = productItemTypes[prod.Name];
                    var tryAdd = nInventory.TryAdd(player, new nItem((ItemType)itemType, count));
                    if (tryAdd == -1)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Ваш инвентарь больше не может вместить {prod.Name}", 3000);
                        return;
                    }
                    else
                    {
                        if (itemType == ItemType.Bag || itemType == ItemType.Bag1)
                        {
                            if (nInventory.ClothesItems.Contains(ItemType.Bag)) {
                                isBag = true;
                                dataBag = "85_0_" + Main.Players[player].Gender;
                            }
                        } else
                        {
                            item = ((ItemType)itemType == ItemType.KeyRing) ? new nItem(ItemType.KeyRing, 1, "") : new nItem((ItemType)itemType, count);
                        }
                    }
                }


                switch (buyType) {
                    case 0:
                        if (Main.Players[player].Money < summaryPrice)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Недостаточно средств", 3000);
                            return;
                        }

                        if (!takeProd(biz.ID, amount, prod.Name, summaryPrice))
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Недостаточно товара на складе", 3000);
                            return;
                        }

                        Wallet.Change(player, -summaryPrice);
                        GameLog.Money($"player({Main.Players[player].UUID})", $"biz({biz.ID})", summaryPrice, "buyInShop by Cash");

                        break;
                    case 1:
                        if (MoneySystem.Bank.Accounts[Main.Players[player].Bank].Balance < summaryPrice)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Недостаточно средств", 3000);
                            return;
                        }

                        if (!takeProd(biz.ID, amount, prod.Name, summaryPrice))
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Недостаточно товара на складе", 3000);
                            return;
                        }

                        Bank.Change(Main.Players[player].Bank, -summaryPrice);
                        GameLog.Money($"player({Main.Players[player].UUID})", $"biz({biz.ID})", summaryPrice, "buyInShop by Card");

                        break;
                }

                if (isSimCard) {
                    if (Main.Players[player].Sim != -1) Main.SimCards.Remove(Main.Players[player].Sim);

                    int sim = Main.GenerateSimcard(Main.Players[player].UUID);

                    nInventory.Add(player, new nItem(ItemType.SimCard, 1, $"{sim},{prod.Price}"));

                    GUI.Dashboard.sendStats(player);
                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы купили сим-карту с номером {sim} и балансом {prod.Price}$", 3000);

                    #region quest chapter iteration

                    QuestSystem.UpdatePlayerQuestIteration(player);

                    #endregion
                }
                else if(isBag)
                {
                    nInventory.Add(player, new nItem(ItemType.Bag, 1, dataBag));
                }
                else
                {
                    nInventory.Add(player, item);
                }

                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы купили {prod.Name} x{count} за {summaryPrice}$", 3000);

            }
            catch (Exception e) { Log.Write($"BuyShop: {e.ToString()}\n{e.StackTrace}", nLog.Type.Error); }
        }


























        public static void OpenBizShopMenu(Player player)
        {
            Business biz = BizList[player.GetMyData<int>("BIZ_ID")];
            List<List<string>> items = new List<List<string>>();

            foreach (var p in biz.Products)
            {
                List<string> item = new List<string>();
                item.Add(p.Name);
                item.Add($"{p.Price}$");
                items.Add(item);
            }
            string json = JsonConvert.SerializeObject(items);
            Trigger.ClientEvent(player, "shop", json);
        }



    }
}
