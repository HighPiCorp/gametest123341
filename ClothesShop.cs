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
using client.Systems.BattlePass;

namespace NeptuneEvo.Core
{
  class ClothesShop : Script
  {
    private static nLog Log = new nLog("ClothesShop");

    public static void openClothesShopMenu(Player player, Business biz)
    {
      if ((Main.Players[player].OnDuty && Fractions.Manager.FractionTypes[Main.Players[player].Fraction.FractionID] == 2) || player.GetMyData<bool>("ON_WORK"))
      {
        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы должны закончить рабочий день", 3000);
        return;
      }

      int bizPaymentType = 0;
      if (biz.Type == 25) bizPaymentType = 1;


      player.SetMyData("CLOTHES_SHOP", biz.ID);

      player.PlayAnimation("amb@world_human_guard_patrol@male@base", "base", 1);
      NAPI.Entity.SetEntityDimension(player, Dimensions.RequestPrivateDimension(player));

      var productPriceWithmarkup = biz.GetPriceWithMarkUpInt(biz.Products[0].Price);

      if (bizPaymentType == 1) productPriceWithmarkup = 100;

      Trigger.ClientEvent(player, "CLIENT::clothes:open", productPriceWithmarkup, bizPaymentType);
    }

    [RemoteEvent("SERVER::clothes:closeMenu")]
    public static void RemoteEvent_cancelClothes(Player player)
    {
      try
      {
        player.StopAnimation();
        Customization.ApplyCharacter(player);
        NAPI.Entity.SetEntityDimension(player, 0);
        Dimensions.DismissPrivateDimension(player);
      }
      catch (Exception e) { Log.Write("SERVER::clothes:closeMenu: " + e.StackTrace, nLog.Type.Error); }
    }

    [RemoteEvent("SERVER::clothes:buy")]
    public static void RemoteEvent_buyClothes(Player player, int buyType, int type, int variation, int texture)
    {
      try
      {
        Business biz = BizList[player.GetMyData<int>("CLOTHES_SHOP")];
        var prod = biz.Products[0];

        int bizPaymentType = 0;
        if (biz.Type == 25) bizPaymentType = 1;

        var tempPrice = 0;
        switch (type)
        {
          case 0: // -11 Верхняя одежда
            tempPrice = Customization.Tops[Main.Players[player].Gender].FirstOrDefault(t => t.Variation == variation).Price;
            break;
          case 1: // -8 Майки
            tempPrice = Customization.Underwears[Main.Players[player].Gender].FirstOrDefault(h => h.Value.Top == variation).Value.Price;
            break;
          case 2: // -4 Брюки
            tempPrice = Customization.Legs[Main.Players[player].Gender].FirstOrDefault(l => l.Variation == variation).Price;
            break;
          case 3: // -6 Обувь
            tempPrice = Customization.Feets[Main.Players[player].Gender].FirstOrDefault(f => f.Variation == variation).Price;
            break;
          case 4:  // -12 Шляпы
            tempPrice = Customization.Hats[Main.Players[player].Gender].FirstOrDefault(h => h.Variation == variation).Price;
            break;
          case 5: // -7 Аксессуары
            tempPrice = Customization.Jewerly[Main.Players[player].Gender].FirstOrDefault(f => f.Variation == variation).Price;
            break;
          case 6: // -14 Часы
            tempPrice = Customization.Accessories[Main.Players[player].Gender].FirstOrDefault(f => f.Variation == variation).Price;
            break;
          case 7: // -13 Очки
            tempPrice = Customization.Glasses[Main.Players[player].Gender].FirstOrDefault(f => f.Variation == variation).Price;
            break;
          case 8: // -3 Перчатки
            tempPrice = Customization.Gloves[Main.Players[player].Gender].FirstOrDefault(f => f.Variation == variation).Price;
            break;
          //case 9: // -14 Браслеты
          //  tempPrice = Customization.Bracelets[Main.Players[player].Gender].FirstOrDefault(f => f.Variation == variation).Price;
          //  break;
        }

        var productPriceWithmarkup = biz.GetPriceWithMarkUpInt(prod.Price);
        if (bizPaymentType == 1) productPriceWithmarkup = 100;

        var price = Convert.ToInt32((tempPrice / 100.0) * productPriceWithmarkup);

        var tryAdd = nInventory.TryAdd(player, new nItem(ItemType.Top));
        if (tryAdd == -1)
        {
          Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Недостаточно места в инвентаре", 3000);
          return;
        }

        //var amount = Convert.ToInt32(price * 0.75 / 50);
        //price = 287468
        //amount = 287468 / 50 = 5749 матов. при 100%
        //5749 * 50 = 287468
        //431202 / 75 = 5749 матов при 150%
        var amount = price / productPriceWithmarkup;

        if (amount <= 0) amount = 1;

        if (bizPaymentType == 0) {
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
                GameLog.Money($"player({Main.Players[player].UUID})", $"biz({biz.ID})", Convert.ToInt32(price), "buyClothes by Cash");

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
                GameLog.Money($"player({Main.Players[player].UUID})", $"biz({biz.ID})", Convert.ToInt32(price), "buyClothes by Card");

                break;
            }
        }
        else
        {
          if (Main.Accounts[player].RedBucks < Convert.ToInt32(price))
          {
            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Недостаточно SWC", 3000);
            return;
          }

          var before = Main.Accounts[player].RedBucks;
          Main.Accounts[player].RedBucks -= Convert.ToInt32(price);
          Log.Debug($"[SWC Changes][{player.Name}] [DonateClothesShop] Покупка одежды: [{Convert.ToInt32(price)}] {before} -> {Main.Accounts[player].RedBucks}");
          GameLog.Money($"player({Main.Players[player].UUID})", $"biz({biz.ID})", Convert.ToInt32(price), "buyClothes by SWC");
        }

        #region BPКвест: 13 Потратить 500.000 на одежду.

        #region BattlePass выполнение квеста
        BattlePass.updateBPQuestIteration(player, BattlePass.BPQuestType.BuyClothes, Convert.ToInt32(price));
        #endregion

        #endregion

        #region SBPКвест: 2 Потратить 500.000 на одежду.

        #region BattlePass выполнение квеста
        BattlePass.updateBPSuperQuestIteration(player, BattlePass.BPSuperQuestType.SpendMoneyOnClothes, Convert.ToInt32(price));
        #endregion

        #endregion

        switch (type)
        {

          case 0:
            Customization.AddClothes(player, ItemType.Top, variation, texture);
            break;
          case 1:
            var id = Customization.Underwears[Main.Players[player].Gender].FirstOrDefault(u => u.Value.Top == variation);
            Customization.AddClothes(player, ItemType.Undershit, id.Key, texture);
            break;
          case 2:
            Customization.AddClothes(player, ItemType.Leg, variation, texture);
            break;
          case 3:
            Customization.AddClothes(player, ItemType.Feet, variation, texture);
            break;
          case 4:
            Customization.AddClothes(player, ItemType.Hat, variation, texture);
            break;
          case 5:
            Customization.AddClothes(player, ItemType.Jewelry, variation, texture);
            break;
          case 6:
            Customization.AddClothes(player, ItemType.Accessories, variation, texture);
            break;
          case 7:
            Customization.AddClothes(player, ItemType.Glasses, variation, texture);
            break;
          case 8:
            Customization.AddClothes(player, ItemType.Gloves, variation, texture);
            break;
          case 9:
            Customization.AddClothes(player, ItemType.Accessories, variation, texture);
            break;
        }

        client.Core.Achievements.AddAchievementScore(player, client.Core.AchievementID.Clothes, price);
        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Вы купили новую одежду. Она была добавлена в Ваш инвентарь.", 3000);
        return;
      }
      catch (Exception e) { Log.Write("SERVER::clothes:buy: " + e.StackTrace, nLog.Type.Error); }
    }

  }
}
