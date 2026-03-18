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
    class MaskShop : Script
    {
        private static nLog Log = new nLog("MaskShop");

        public static void openMaskShop(Player player, Business biz) {
            try { 
            if ((Main.Players[player].OnDuty && Fractions.Manager.FractionTypes[Main.Players[player].Fraction.FractionID] == 2) || player.GetMyData<bool>("ON_WORK"))
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы должны закончить рабочий день", 3000);
                return;
            }
            player.SetMyData("MASKS_SHOP", biz.ID);
            Trigger.ClientEvent(player, "CLIENT::mask:open", biz.GetPriceWithMarkUpInt(biz.Products[0].Price));
            player.PlayAnimation("amb@world_human_guard_patrol@male@base", "base", 1);
            NAPI.Entity.SetEntityDimension(player, Dimensions.RequestPrivateDimension(player));
            Customization.ApplyMaskFace(player);
            }catch (Exception ex) { Log.Write(ex.StackTrace); }
        }

        [RemoteEvent("SERVER::mask:closeMenu")]
        public static void RemoteEvent_closeMaskShop(Player player) {
            try
            {
                NAPI.Entity.SetEntityDimension(player, 0);
                player.StopAnimation();
                Customization.ApplyCharacter(player);
                Customization.SetMask(player, Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Mask.Variation, Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Mask.Texture);
                Dimensions.DismissPrivateDimension(player);
            }
            catch (Exception e) { Log.Write("SERVER::mask:closeMenu: " + e.StackTrace, nLog.Type.Error); }
        }

        [RemoteEvent("SERVER::mask:buy")]
        public static void RemoteEvent_buy(Player player, int buyType, int variation, int texture) {
            try
            {
                Business biz = BizList[player.GetMyData<int>("MASKS_SHOP")];
                var prod = biz.Products[0];

                var tempPrice = Customization.Masks.FirstOrDefault(f => f.Variation == variation).Price;

                var price = Convert.ToInt32((tempPrice / 100.0) * biz.GetPriceWithMarkUpInt(prod.Price));

                var amount = price / biz.GetPriceWithMarkUpInt(prod.Price);
                if (amount <= 0) amount = 1;

                var tryAdd = nInventory.TryAdd(player, new nItem(ItemType.Top));
                if (tryAdd == -1)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Недостаточно места в инвентаре", 3000);
                    return;
                }

                switch (buyType)
                {
                    case 0:
                        if (Main.Players[player].Money < Convert.ToInt32(price))
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Недостаточно средств", 3000);
                            return;
                        }

                        if (!takeProd(biz.ID, amount, prod.Name, price))
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Недостаточно товара на складе", 3000);
                            return;
                        }

                        Wallet.Change(player, -Convert.ToInt32(price));
                        GameLog.Money($"player({Main.Players[player].UUID})", $"biz({biz.ID})", Convert.ToInt32(price), "buyMasks by Cash");

                        break;
                    case 1:
                        if (MoneySystem.Bank.Accounts[Main.Players[player].Bank].Balance < Convert.ToInt32(price))
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Недостаточно средств", 3000);
                            return;
                        }

                        if (!takeProd(biz.ID, amount, prod.Name, price))
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Недостаточно товара на складе", 3000);
                            return;
                        }

                        Bank.Change(Main.Players[player].Bank, -Convert.ToInt32(price));
                        GameLog.Money($"player({Main.Players[player].UUID})", $"biz({biz.ID})", Convert.ToInt32(price), "buyMasks by Card");

                        break;
                }


                Customization.AddClothes(player, ItemType.Mask, variation, texture);

                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Вы купили новую маску. Она была добавлена в Ваш инвентарь.", 3000);
                return;
            }
            catch (Exception e) { Log.Write("SERVER::maks:buy: " + e.StackTrace, nLog.Type.Error); }
        }

    }
}
