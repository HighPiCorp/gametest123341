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
  class BagsShop : Script
  {
    private static nLog Log = new nLog("BagsShop");

    public static void openBagsShop(Player player, Business biz)
    {
      try { 
      if ((Main.Players[player].OnDuty && Fractions.Manager.FractionTypes[Main.Players[player].Fraction.FractionID] == 2) || player.GetMyData<bool>("ON_WORK"))
      {
        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы должны закончить рабочий день", 3000);
        return;
      }

      player.SetMyData("BAGS_SHOP", biz.ID);
      Trigger.ClientEvent(player, "CLIENT::bags:open", biz.GetPriceWithMarkUpInt(biz.Products[0].Price));
      player.PlayAnimation("amb@world_human_guard_patrol@male@base", "base", 1);
      NAPI.Entity.SetEntityDimension(player, Dimensions.RequestPrivateDimension(player));
                //Customization.ApplyMaskFace(player);
      }catch (Exception ex) { Log.Write(ex.StackTrace); }
    }

    [RemoteEvent("SERVER::bags:closeMenu")]
    public static void RemoteEvent_closeMaskShop(Player player)
    {
      try
      {
        NAPI.Entity.SetEntityDimension(player, 0);
        player.StopAnimation();
        Customization.ApplyCharacter(player);
        Dimensions.DismissPrivateDimension(player);
      }
      catch (Exception e) { Log.Write("SERVER::bags:closeMenu: " + e.StackTrace, nLog.Type.Error); }
    }

    [RemoteEvent("SERVER::bags:buy")]
    public static void RemoteEvent_buy(Player player, int buyType, int variation, int texture)
    {
      try
      {
        Business biz = BizList[player.GetMyData<int>("BAGS_SHOP")];
        var prod = biz.Products[0];

        var bagItem = nBags.BagsData.FirstOrDefault(b => b.Variation == variation);

        if (bagItem == null)
        {
          Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Ошибка. Рюкзак не найден.", 3000);
          return;
        }

        var tempPrice = bagItem.Price;

        var price = Convert.ToInt32((tempPrice / 100.0) * biz.GetPriceWithMarkUpInt(prod.Price));

        var tryAdd = nInventory.TryAdd(player, new nItem(ItemType.Bag));
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

            if (!takeProd(biz.ID, 1, "Рюкзаки", price))
            {
              Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Недостаточно товара на складе", 3000);
              return;
            }

            Wallet.Change(player, -Convert.ToInt32(price));
            GameLog.Money($"player({Main.Players[player].UUID})", $"biz({biz.ID})", Convert.ToInt32(price), "buyBags by Cash");

            break;
          case 1:
            if (MoneySystem.Bank.Accounts[Main.Players[player].Bank].Balance < Convert.ToInt32(price))
            {
              Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Недостаточно средств", 3000);
              return;
            }

            if (!takeProd(biz.ID, 1, "Рюкзаки", price))
            {
              Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Недостаточно товара на складе", 3000);
              return;
            }

            Bank.Change(Main.Players[player].Bank, -Convert.ToInt32(price));
            GameLog.Money($"player({Main.Players[player].UUID})", $"biz({biz.ID})", Convert.ToInt32(price), "buyBags by Card");

            break;
        }


        //Customization.AddClothes(player, ItemType.Bag, variation, texture);

        int playerUUID = Main.Players[player].UUID;
        bool playerGender = Main.Players[player].Gender;

        var dataBag = $"{variation}_{texture}_{Main.Players[player].Gender}";

        nInventory.Add(player, new nItem(ItemType.Bag, 1, dataBag, isBag: true, bag: bagItem));

        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Вы купили новый рюкзак. Он был добавлен в Ваш инвентарь.", 3000);
        return;
      }
      catch (Exception e) { Log.Write("SERVER::bags:buy: " + e.StackTrace, nLog.Type.Error); }
    }

  }
}
