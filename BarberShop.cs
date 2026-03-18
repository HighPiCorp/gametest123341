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
    class BarberShop : Script
    {
        private static nLog Log = new nLog("BarberShop");

        #region Массивы тату

        public static Dictionary<string, List<int>> BarberHairPrices = new Dictionary<string, List<int>>()
        {
        { "male", new List<int>()
            {
            400,
            350,
            350,
            450,
            450,
            600,
            450,
            1100,
            450,
            600,
            600,
            400,
            350,
            2000,
            750,
            1500,
            450,
            600,
            600,
            400,
            350,
            2000,
            750,
            1500,
            1500,
            1500,
            1500,
            1500,
            1500,
            1500,
            1500,
            1500,
            1500,
            1500,
            1500,
            1500,
            1500,
            1500,
            3500,
            2000,
            2500,
            13750,
            15600,
            17300,
            16000,
            18700,
            18000,
            18000,
            32400,
            21200,
            26700,
            24300,
            31200,
            19700,
            23800,
            34500,
            25000,
            43400,
            40000,
            47500,
            28800,
            24300,
            43500,
            26300,
            34900,
            35000,
            27200,
            43800,
            47000

            }
        },
        { "female", new List<int>()
            {
            400,
            350,
            350,
            450,
            450,
            600,
            450,
            1100,
            450,
            600,
            600,
            400,
            350,
            2000,
            750,
            1500,
            450,
            600,
            600,
            400,
            350,
            2000,
            750,
            1500,
            1500,
            1500,
            3500,
            2500,
            1500,
            1500,
            1500,
            300,
            28900,
            13300,
            27000,
            29000,
            24500,
            32500,
            32500,
            31200,
            23600,
            24000,
            31200,
            25300,
            37800,
            45700,
            14000,
            42200,
            46700,
            23500,
            38100,
            16700,
            21200,
            26300,
            29800,
            52300,
            50900,
            50100,
            19200,
            26700,
            11300,
            17700,
            43700,
            43200,
            35100,
            27200,
            30000,
            26000,
            53000,
            16999,
            19800,
            42900,
            43100,
            16800,
            15000,
            44000,
            40000,
            45700,
            48900,
            47100,
            40200,
            23100,
            27800,
            25300,
            14000,
            45500,
            27700,
            38450,
            32300,
            43500,
            51900,
            40100,
            55200,
            47200,
            37300,
            38900,
            32700,
            29900,
            55500,
            52200,
            47800,
            48900,
            15400,
            17000,
            39100,
            34500,
            32100,
            16000,
            15000,
            16700,
            22300,
            41900,
            47800,
            29000,
            30000
            }
        },
        };
        public static Dictionary<string, List<int>> BarberPrices = new Dictionary<string, List<int>>()
        {
          { "beard", new List<int>() {
              120,
              120,
              120,
              120,
              120,
              160,
              160,
              160,
              120,
              120,
              240,
              240,
              120,
              120,
              240,
              200,
              120,
              160,
              380,
              360,
              360,
              180,
              180,
              260,
              120,
              120,
              240,
              200,
              120,
              160,
              380,
              360,
              360,
              180,
              180,
              260,
              120,
              180,
              180
          }},
          { "eyebrows", new List<int>() {
              100,
              100,
              100,
              100,
              100,
              100,
              100,
              100,
              100,
              100,
              100,
              100,
              100,
              100,
              100,
              100,
              100,
              100,
              100,
              100,
              100,
              100,
              100,
              100,
              100,
              100,
              100,
              100,
              100,
              100,
              100,
              100,
              100,
              100,
              100,
              100,
              100,
              100,
              100,
              100
          }},
          { "chesthair", new List<int>() {
              100,
              100,
              100,
              100,
              100,
              100,
              100,
              100,
              100,
              100,
              100,
              100,
              100,
              100,
              100,
              100,
              100,
              100,
              100,
              100,
              100,
              100,
              100,
              100
          }},
          { "lenses", new List<int>() {
              200,
              400,
              400,
              200,
              200,
              400,
              200,
              400,
              1000,
              1000
          }},
          { "lipstick", new List<int>() {
              200,
              400,
              400,
              200,
              200,
              400,
              200,
              400,
              1000,
              300
          }},
          { "blush", new List<int>() {
              200,
              400,
              400,
              200,
              200,
              400,
              200
          }},
          { "makeup", new List<int>() {
              120,
              120,
              120,
              120,
              120,
              160,
              160,
              160,
              120,
              120,
              240,
              240,
              120,
              120,
              240,
              200,
              120,
              160,
              380,
              360,
              360,
              180,
              180,
              260,
              120,
              120,
              240,
              200,
              120,
              160,
              380,
              360,
              360,
              180,
              180,
              260,
              120,
              180,
              180
          }},
      };
        #endregion

        public static void openBarberShopMenu(Player player, Business biz)
        { 
            var dim = Dimensions.RequestPrivateDimension(player);
            NAPI.Entity.SetEntityDimension(player, dim);
            player.PlayAnimation("amb@world_human_guard_patrol@male@base", "base", 1);
            Customization.ClearClothes(player, Main.Players[player].Gender);
            Main.Players[player].InsideBizID = biz.ID;

            Trigger.ClientEvent(player, "CLIENT::barber:openBarberMenu", biz.GetPriceWithMarkUpInt(biz.Products[0].Price), biz.Markup);
        }

        [RemoteEvent("SERVER::barber:closeMenu")]
        public static void RemoteEvent_cancelBarber(Player player)
        {
            try
            {
                Business biz = BusinessManager.BizList[Main.Players[player].InsideBizID];
                var spawnPos = biz.EnterPoint + new Vector3(0, 0, 1.12);

                Log.Debug($"Inside: {Main.Players[player].InsideBizID} EnterPoint: {JsonConvert.SerializeObject(biz.EnterPoint)} spawnPos: {spawnPos}");

                NAPI.Task.Run(() => {
                    NAPI.Entity.SetEntityDimension(player, 0);
                    NAPI.Entity.SetEntityPosition(player, spawnPos);
                    //player.Dimension = 0;
                    //player.Position = biz.EnterPoint + new Vector3(0, 0, 1.12);
                    
                    Customization.ApplyCharacter(player);
                    player.StopAnimation();
                    Log.Debug($"NAPI: currentPos: {player.Position}");
                }, 500);

                Main.Players[player].ExteriorPos = new Vector3();
                Main.Players[player].InsideBizID = -1;

                Log.Debug($"Result: currentPos: {player.Position}");
            }
            catch (Exception e) { Log.Write("SERVER::tattoo:closeMenu: " + e.StackTrace, nLog.Type.Error); }
        }

        [RemoteEvent("SERVER::barber:buyBarber")]
        public static void RemoteEvent_buyTattoo(Player player, int buyType, string id, int style, int color, int index)
        {

            //Log.Write("id: "+ id+ " style: "+style+" color: "+ color, nLog.Type.Error);
            try
            {
                Business biz = BusinessManager.BizList[Main.Players[player].InsideBizID];

                if ((id == "lipstick" || id == "blush" || id == "makeup") && Main.Players[player].Gender && style != 255)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Доступно только для персонажей женского пола", 3000);
                    return;
                }

                int price = 0;

                if (id == "hair")
                {
                    var gender = Main.Players[player].Gender ? "male" : "female";
                    //BarberHairPrices
                    //if (id == "hair" && style >= 23) price = biz.GetPriceWithMarkUpInt(BarberHairPrices[gender][23]);
                    if (style == 255) price = biz.GetPriceWithMarkUpInt(BarberHairPrices[gender][0]);
                    else price = biz.GetPriceWithMarkUpInt(BarberHairPrices[gender][index]);
                }
                else
                {
                    if (style == 255) price = biz.GetPriceWithMarkUpInt(BarberPrices[id][0]);
                    else price = biz.GetPriceWithMarkUpInt(BarberPrices[id][index]);
                }

                var prod = biz.Products.FirstOrDefault(p => p.Name == "Расходники");
                var amount = Convert.ToInt32(price) / biz.GetPriceWithMarkUpInt(prod.Price);
                if (amount <= 0) amount = 1;

                switch (buyType)
                {
                    case 0:
                        if (Main.Players[player].Money < Convert.ToInt32(price))
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Недостаточно средств", 3000);
                            return;
                        }

                        if (!takeProd(biz.ID, amount, prod.Name, Convert.ToInt32(price)))
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Этот барбер-шоп не может оказать эту услугу в данный момент", 3000);
                            return;
                        }

                        Wallet.Change(player, -Convert.ToInt32(price));
                        GameLog.Money($"player({Main.Players[player].UUID})", $"biz({biz.ID})", Convert.ToInt32(price), "buyBarber by Cash");

                        break;
                    case 1:
                        if (MoneySystem.Bank.Accounts[Main.Players[player].Bank].Balance < Convert.ToInt32(price))
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Недостаточно средств", 3000);
                            return;
                        }

                        if (!takeProd(biz.ID, amount, prod.Name, Convert.ToInt32(price)))
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Этот барбер-шоп не может оказать эту услугу в данный момент", 3000);
                            return;
                        }

                        Bank.Change(Main.Players[player].Bank, -Convert.ToInt32(price));
                        GameLog.Money($"player({Main.Players[player].UUID})", $"biz({biz.ID})", Convert.ToInt32(price), "buyBarber by Card");

                        break;
                }

                #region BPКвест: 105  Постричься. / 106/107  Потратить на стрижки 5.000$./35.000$

                #region BattlePass выполнение квеста
                BattlePass.updateBPQuestIteration(player, BattlePass.BPQuestType.BarberChangeHair);
                BattlePass.updateBPQuestIteration(player, BattlePass.BPQuestType.BarberSpendMoney, Convert.ToInt32(price));
                #endregion

                #endregion

                switch (id)
                {
                    case "hair":
                        Customization.CustomPlayerData[Main.Players[player].UUID].Hair = new HairData(style, color, color);
                        client.Core.Achievements.AddAchievementScore(player, client.Core.AchievementID.Barber, 1);
                        break;
                    case "beard":
                        Customization.CustomPlayerData[Main.Players[player].UUID].Appearance[1].Value = style;
                        Customization.CustomPlayerData[Main.Players[player].UUID].BeardColor = color;
                        break;
                    case "eyebrows":
                        Customization.CustomPlayerData[Main.Players[player].UUID].Appearance[2].Value = style;
                        Customization.CustomPlayerData[Main.Players[player].UUID].EyebrowColor = color;
                        break;
                    case "chesthair":
                        Customization.CustomPlayerData[Main.Players[player].UUID].Appearance[10].Value = style;
                        Customization.CustomPlayerData[Main.Players[player].UUID].ChestHairColor = color;
                        break;
                    case "lenses":
                        Customization.CustomPlayerData[Main.Players[player].UUID].EyeColor = style;
                        break;
                    case "lipstick":
                        Customization.CustomPlayerData[Main.Players[player].UUID].Appearance[8].Value = style;
                        Customization.CustomPlayerData[Main.Players[player].UUID].LipstickColor = color;
                        break;
                    case "blush":
                        Customization.CustomPlayerData[Main.Players[player].UUID].Appearance[5].Value = style;
                        Customization.CustomPlayerData[Main.Players[player].UUID].BlushColor = color;
                        break;
                    case "makeup":
                        Customization.CustomPlayerData[Main.Players[player].UUID].Appearance[4].Value = style;
                        break;
                }

                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы оплатили услугу Барбер-Шопа ({Convert.ToInt32(price)}$)", 3000);
                return;
            }
            catch (Exception e) { Log.Write("BuyTattoo: " + e.StackTrace, nLog.Type.Error); }
        }




    }
}
