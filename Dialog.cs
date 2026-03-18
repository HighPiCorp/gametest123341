using client.Fractions.Utils;
using client.Systems;
using client.Systems.CraftSystem;
using GTANetworkAPI;
using NeptuneEvo;
using NeptuneEvo.Core;
using NeptuneEvo.Houses;
using NeptuneEvo.Jobs;
using NeptuneEvo.SDK;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Trigger = NeptuneEvo.Trigger;

namespace client.GUI
{
    class Dialog : Script
    {
        private static nLog Log = new nLog("Dialog");
        public static void Send(Player player, string dialogID, string pedName, string penReply, List<Reply> replies)
        {
            if (!Main.Players.ContainsKey(player))
                return;
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("title", pedName);
            data.Add("descr", penReply);

            List<Dictionary<string, object>> btns = new List<Dictionary<string, object>>();

            foreach (var reply in replies) {
                btns.Add(new Dictionary<string, object>(){

                    {"text", reply.Text },
                    {"color", reply.Color }
                });
            }

            data.Add("btn", btns);

            player.SetMyData("DIALOG2_ID", dialogID);

            Trigger.PlayerEvent(player, "CLIENT::DIALOG:OPEN", JsonConvert.SerializeObject(data));
        }

        public static void Sendv2(Player player, string dialogID, string pedName, string penReply, List<Replyv2> replies)
        {
            if (!Main.Players.ContainsKey(player))
                return;
            List<object[]> data = new List<object[]>();
            foreach (var reply in replies)
                data.Add(new object[] { reply.Text, reply.Answer });
            Trigger.PlayerEvent(player, "openDialogNpc_v2", dialogID, pedName, penReply, data);
            player.SetMyData($"DIALOG_{dialogID}", replies);
        }

        public static void Close(Player player)
        {
            player.ResetMyData("DIALOG2_ID");
            Trigger.ClientEvent(player, "CLIENT::DIALOG:CLOSE");
        }

        [RemoteEvent("SERVER::DIALOG:CALLBACK")]
        public static void DialogCallBack(Player player, int answer)
        {
            if (!Main.Players.ContainsKey(player))
                return;

            if (!player.HasMyData("DIALOG2_ID")) return;

            string dialogID = player.GetMyData<string>("DIALOG2_ID");

            switch (dialogID)
            {
                case "RobberyNPC":
                    RobberyHouses.Dialog_callback(player, answer);
                    break;
                case "QUEST":
                    QuestSystem.Dialog_callback(player, answer);
                    break;
                case "CARTHEFT":
                    CarTheftManager.Dialog_CallBack(player, answer);
                    break;
                case "CARTHEFT2":
                    CarTheftManager.Dialog2_CallBack(player, answer);
                    break;
                case "BLACKMARKET":
                    BlackMarket.Dialog_CallBack(player, answer);
                    break;
                case "BLACKMARKET2":
                    BlackMarket.Dialog_CallBack2(player, answer);
                    break;
                case "CRAFTMARKET":
                    MetalDetectorSystem.Dialog_CallBack(player, answer);
                    break;
                case "CRAFTMARKET2":
                    MetalDetectorSystem.Dialog_CallBack2(player, answer);
                    break;
                case "BUSWAYSELECTOR":
                    Bus.Dialog_CallBack(player, answer);
                    break;
                case "GARBAGE1":
                    NeptuneEvo.Jobs.GarbageTruck.Dialog1_CallBack(player, answer);
                    break;
                case "GARBAGE2":
                    NeptuneEvo.Jobs.GarbageTruck.Dialog2_CallBack(player, answer);
                    break;
                case "POSTAL":
                    WorkManager.callback_gpStartMenu(player, answer);
                    break;
                case "POSTAL2":
                    WorkManager.callback_gpStartMenu2(player, answer);
                    break;
                case "BUYNUMBER":
                    BuyNumbers.DialogCallback(player, answer);
                    break;
                case "ROOMMATEGARAGE":
                    GarageManager.RoomMateGarageCallback(player, answer);
                    break;
                case "FRACTIONCLOTHES":
                  FractionClothes.FractionClothesCallBack(player,answer);
                  break;
                default:
                    if (player.HasMyData($"DIALOG_{dialogID}"))
                    {
                        Replyv2 reply = player.GetMyData<List<Replyv2>>($"DIALOG_{dialogID}").Find(f => f.Answer == Convert.ToString(answer));
                        player.ResetMyData($"DIALOG_{dialogID}");
                        if (reply != null)
                        {
                            reply.Action?.Invoke();
                            if (reply.DestroyCam)
                            {
                                Trigger.PlayerEvent(player, "DestroyCamToNPC");
                                Dialog.Close(player);
                            }
                        }
                        break;
                    }
                    Log.Write($"Dialog {dialogID} not have case", nLog.Type.Warn);
                    break;
            }
        }
    }

    class Reply
    {
        public string Text;
        public string Color;

        public Reply(string text, string color = "")
        {
            Text = text;
            Color = color;
        }
    }

    class Replyv2
    {
        public string Text;
        public string Answer;
        public Action Action;
        public bool DestroyCam;

        public Replyv2(string text, string answer = "", Action action = null, bool destroyCam = true)
        {
            Text = text;
            Answer = answer;
            Action = action;
            DestroyCam = destroyCam;
        }
    }
}
