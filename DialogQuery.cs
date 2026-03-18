using client.Fractions.Government;
using client.Fractions.Utils;
using GTANetworkAPI;
using NeptuneEvo;
using NeptuneEvo.Core;
using NeptuneEvo.Fractions;
using NeptuneEvo.SDK;
using System;
using System.Collections.Generic;
using System.Text;
using Trigger = NeptuneEvo.Trigger;

namespace client.GUI
{
    class DialogQuery : Script
    {
        private static nLog Log = new nLog("DialogQuery");

        public static void OpenDialog(Player player, string type, string text, Action yes, Action no = null)
        {
            Trigger.PlayerEvent(player, "popup::open", type, text);
            player.SetMyData($"DIALOGQUERY_{type}", new Query(yes, no));
        }

        public static void Callback(Player player, string type, bool yes)
        {
            if (!player.HasMyData($"DIALOGQUERY_{type}"))
                return;
            Query query = player.GetMyData<Query>($"DIALOGQUERY_{type}");
            if (yes)
                query.Yes?.Invoke();
            else
                query.No?.Invoke();

            player.ResetMyData($"DIALOGQUERY_{type}");
        }

        class Query
        {
            public Action Yes;
            public Action No;

            public Query(Action yes, Action no)
            {
                Yes = yes;
                No = no;
            }
        }

        //[RemoteEvent("dialogCallback")]
        public void RemoteEvent_DialogCallback(Player player, string callback, bool yes)
        {
            try
            {
                if (yes)
                {
                    switch (callback)
                    {
                       
                        case "PAY_MEDKIT":
                            Ems.payMedkit(player);
                            return;
                       
                        case "INVITED":
                            {
                                int fracid = player.GetData<int>("INVITEFRACTION");

                                Main.Players[player].Fraction.FractionID = fracid;
                                Main.Players[player].Fraction.FractionRankID = Configs.GetMinFracRankID(fracid);//1;
                                Main.Players[player].Fraction.FractionInvite = DateTime.Now;
                                Main.Players[player].Fraction.Clothes = Fractions.Utils.FractionClothes.FractionDefaultClothes[Main.Players[player].Gender][fracid];
                                Main.Players[player].Fraction.Accessory = Fractions.Utils.FractionClothes.FractionDefaultAccessorys[Main.Players[player].Gender][fracid];
                                Main.Players[player].Fraction.Reprimand = new List<string>();
                                Main.Players[player].WorkID = 0;

                                Main.Players[player].InAllFractions[fracid] = 1;

                                int count = 0;

                                foreach(int num in Main.Players[player].InAllFractions)
                                {
                                    if (num == 1) count++;
                                }

                                if(count == 17)
                                {
                                    client.Core.Achievements.AddAchievementScore(player, Core.AchievementID.InAllFractions, 1);
                                }

                                Manager.Load(player, Main.Players[player].Fraction.FractionID, Main.Players[player].Fraction.FractionRankID);
                                if (Manager.FractionTypes[fracid] == 1) client.Fractions.Gangs.GangsCapture.LoadBlips(player);
                                if (fracid == 15)
                                {
                                    Trigger.PlayerEvent(player, "enableadvert", true);
                                    LSNews.onLSNPlayerLoad(player); // Загрузка всех объявлений в F7
                                }
                                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы вступили в {Manager.FractionNames[fracid]}", 3000);
                                try
                                {
                                    Notify.Send(player.GetData<Player>("SENDERFRAC"), NotifyType.Success, NotifyPosition.BottomCenter, $"{player.Name} принял приглашение вступить в Вашу фракцию", 3000);
                                }
                                catch (Exception e){ Log.Write(e.StackTrace, nLog.Type.Error); }
                                FractionLogs.FractionMember(fracid, MemberOperationType.invite, player.GetData<Player>("SENDERFRAC").Name, Main.Players[player.GetData<Player>("SENDERFRAC")].UUID, player.Name, Main.Players[player].UUID);
                                return;
                            }
                       
                        case "GUN_LIC":
                            FractionCommands.acceptGunLic(player);
                            return;
                        case "TICKET":
                            FractionCommands.ticketConfirm(player, true);
                            return;
                        case "BUY_MEDCARD":
                            Ems.BuyMedCard(player, yes);
                            return;
                    }
                }
                else
                {
                    switch (callback)
                    {
                        case "TICKET":
                            FractionCommands.ticketConfirm(player, false);
                            return;
                        case "BUY_MEDCARD":
                            Ems.BuyMedCard(player, yes);
                            return;
                    }
                }
                DialogQuery.Callback(player, callback, yes);
            }
            catch (Exception e) { Log.Write($"dialogCallback ({callback} yes: {yes}): " + e.StackTrace + "\n" + e.StackTrace, nLog.Type.Error); }
        }
    }
}
