using client.Fractions.Government;
using client.Fractions.Government.GOV;
using GTANetworkAPI;
using NeptuneEvo;
using NeptuneEvo.Core;
using NeptuneEvo.Core.Character;
using NeptuneEvo.Fractions;
using NeptuneEvo.SDK;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Trigger = NeptuneEvo.Trigger;

namespace client.GUI
{
    class DialogInput : Script
    {
        private static nLog Log = new nLog("DialogInput");

        public static void Open<T>(Player player, string title, string inputText, int charCount, string value, Action<T> action)
        {
            Open(player, title, new List<InfoMenu.Text>(), inputText, charCount, value, action);
        }
        public static void Open<T>(Player player, string title, List<InfoMenu.Text> texts, string inputText, int charCount, string value, Action<T> action)
        {
            List<object> dataText = new List<object>();
            foreach (var item in texts)
                dataText.Add(item.ToObject());

            player.SetMyData("INPUTCALLBACKTYPE", typeof(T));
            player.SetMyData("INPUTCALLBACK", action);
            Trigger.PlayerEvent(player, "openInputv2", title, JsonConvert.SerializeObject(dataText), inputText, charCount, value);
        }

        //[RemoteEvent("inputCallback")]
        public void PlayerEvent_inputCallback(Player player, params object[] arguments)
        {
            string callback = "";
            try
            {
                callback = arguments[0].ToString();
                string text = arguments[1].ToString();

                switch (callback)
                {
                    case "fuelcontrol_city":
                    case "fuelcontrol_police":
                    case "fuelcontrol_ems":
                    case "fuelcontrol_fib":
                    case "fuelcontrol_army":
                    case "fuelcontrol_news":
                        int limit = 0;
                        if (!Int32.TryParse(text, out limit) || limit <= 0)
                        {
                            Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, "Введите корректные данные", 3000);
                            return;
                        }

                        string fracName = "";
                        int fracID = 6;
                        if (callback == "fuelcontrol_city")
                        {
                            fracName = "Мэрия";
                            fracID = 6;
                        }
                        else if (callback == "fuelcontrol_police")
                        {
                            fracName = "Полиция";
                            fracID = 7;
                        }
                        else if (callback == "fuelcontrol_ems")
                        {
                            fracName = "EMS";
                            fracID = 8;
                        }
                        else if (callback == "fuelcontrol_fib")
                        {
                            fracName = "FIB";
                            fracID = 9;
                        }
                        else if (callback == "fuelcontrol_army")
                        {
                            fracName = "Армия";
                            fracID = 14;
                        }
                        else if (callback == "fuelcontrol_news")
                        {
                            fracName = "News";
                            fracID = 15;
                        }

                        client.Fractions.Utils.Stocks.fracStocks[fracID].FuelLimit = limit;
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы установили дневной лимит топлива в ${limit} для {fracName}", 3000);
                        return;
                  
                    case "mayor_take":
                        if (!Manager.isLeader(player, 6)) return;

                        int amount = 0;
                        try
                        {
                            amount = Convert.ToInt32(text);
                            if (amount <= 0) return;
                        }
                        catch { return; }

                        if (amount > Fractions.Government.Cityhall.canGetMoney)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы не можете получить больше {Fractions.Government.Cityhall.canGetMoney}$ сегодня", 3000);
                            return;
                        }

                        if (Fractions.Utils.Stocks.fracStocks[6].Money < amount)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Недостаточно средств в казне", 3000);
                            return;
                        }
                        NeptuneEvo.MoneySystem.Bank.Change(Main.Players[player].Bank, amount);
                        Fractions.Utils.Stocks.fracStocks[6].Money -= amount;
                        GameLog.Money($"frac(6)", $"bank({Main.Players[player].Bank})", amount, "treasureTake");
                        return;
                    case "mayor_put":
                        if (!Manager.isLeader(player, 6)) return;

                        amount = 0;
                        try
                        {
                            amount = Convert.ToInt32(text);
                            if (amount <= 0) return;
                        }
                        catch { return; }

                        if (!NeptuneEvo.MoneySystem.Bank.Change(Main.Players[player].Bank, -amount))
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Недостаточно средств", 3000);
                            return;
                        }
                        Fractions.Utils.Stocks.fracStocks[6].Money += amount;
                        GameLog.Money($"bank({Main.Players[player].Bank})", $"frac(6)", amount, "treasurePut");
                        return;
                    case "call_police":
                        if (text.Length == 0)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Введите причину", 3000);
                            return;
                        }
                        Fractions.Government.Police.callPolice(player, text);
                        break;
                    case "client_medkit":
                        try
                        {
                            Convert.ToInt32(text);
                        }
                        catch
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Введите корректные данные", 3000);
                            return;
                        }
                        if (!player.HasData("SELECTEDPLAYER") || player.GetData<Player>("SELECTEDPLAYER") == null || !Main.Players.ContainsKey(player.GetData<Player>("SELECTEDPLAYER"))) return;
                        FractionCommands.sellMedKitToTarget(player, player.GetData<Player>("SELECTEDPLAYER"), Convert.ToInt32(text));
                        return;
                   
                    case "item_transfer_tofracstock":
                        {
                            int index = player.GetData<int>("ITEMINDEX");
                            ItemType type = player.GetData<ItemType>("ITEMTYPE");
                            Character acc = Main.Players[player];
                            List<nItem> items = nInventory.Items[acc.UUID];
                            if (items.Count <= index) return;
                            nItem item = items[index];
                            if (item.Type != type) return;

                            int transferAmount = Convert.ToInt32(text);
                            if (transferAmount <= 0) return;
                            if (item.Count < transferAmount)
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У Вас нет столько {nInventory.ItemsNames[(int)item.Type]}", 3000);
                                return;
                            }

                            if (!player.HasData("ONFRACSTOCK")) return;
                            int onFraction = player.GetData<int>("ONFRACSTOCK");
                            if (onFraction == 0) return;

                            int tryAdd = Fractions.Utils.Stocks.TryAdd(onFraction, item);
                            if (tryAdd == -1)
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Недостаточно места на складе", 3000);
                                return;
                            }

                            nInventory.Remove(player, item.Type, transferAmount);
                            Fractions.Utils.Stocks.Add(onFraction, new nItem(item.Type, transferAmount), player);
                            GameLog.Items($"player({Main.Players[player].UUID})", $"fracstock({onFraction})", Convert.ToInt32(item.Type), transferAmount, $"{item.Data}");
                            GameLog.Stock(Main.Players[player].Fraction.FractionID, Main.Players[player].UUID, $"{nInventory.ItemsNames[(int)item.Type]}", transferAmount, false);
                        }
                        return;
                   
                    case "item_transfer_fromfracstock":
                        {
                            int index = player.GetData<int>("ITEMINDEX");
                            ItemType type = player.GetData<ItemType>("ITEMTYPE");

                            if (!player.HasData("ONFRACSTOCK")) return;
                            int onFraction = player.GetData<int>("ONFRACSTOCK");
                            if (onFraction == 0) return;

                            List<nItem> items = Fractions.Utils.Stocks.fracStocks[onFraction].Weapons;
                            if (items.Count <= index) return;
                            nItem item = items[index];
                            if (item.Type != type) return;

                            int count = Fractions.Utils.Stocks.GetCountOfType(onFraction, item.Type);
                            int transferAmount = Convert.ToInt32(text);
                            if (transferAmount <= 0) return;
                            if (count < transferAmount)
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"На складе нет столько {nInventory.ItemsNames[(int)item.Type]}", 3000);
                                return;
                            }
                            int tryAdd = nInventory.TryAdd(player, new nItem(item.Type, transferAmount));
                            if (tryAdd == -1)
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Недостаточно места в инвентаре", 3000);
                                return;
                            }
                            else if (tryAdd == -2)
                            {
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Недостаточно места в инвентаре (вес)", 3000);
                                return;
                            }

                            nInventory.Add(player, new nItem(item.Type, transferAmount));
                            Fractions.Utils.Stocks.Remove(onFraction, new nItem(item.Type, transferAmount));
                            GameLog.Stock(Main.Players[player].Fraction.FractionID, Main.Players[player].UUID, $"{nInventory.ItemsNames[(int)item.Type]}", transferAmount, true);
                            GameLog.Items($"fracstock({onFraction})", $"player({Main.Players[player].UUID})", Convert.ToInt32(item.Type), transferAmount, $"{item.Data}");
                        }
                        return;
                    case "weaptransfer":
                        {
                            int ammo = 0;
                            if (!Int32.TryParse(text, out ammo))
                            {
                                Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, "Введите корректные данные", 3000);
                                return;
                            }
                            if (ammo <= 0) return;

                        }
                        return;
                   
                    case "client_ticketsum":
                        int sum = 0;
                        if (!Int32.TryParse(text, out sum))
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Некорректные данные", 3000);
                            return;
                        }
                        player.SetMyData("TICKETSUM", sum);
                        Trigger.PlayerEvent(player, "openInput", "Выписать штраф (причина)", "Причина", 50, "client_ticketreason");
                        break;
                    case "client_ticketreason":
                        FractionCommands.ticketToTarget(player, player.GetData<Player>("TICKETTARGET"), player.GetData<int>("TICKETSUM"), text);
                        break;
                    case "fraction_order_mats":
                        int val = 0;
                        if (!Int32.TryParse(text, out val))
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Некорректные данные", 3000);
                            return;
                        }
                        GovernmentOrder.BuyMats(player, val);
                        /*if (val < 1000 || val > 20000)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Некорректные данные", 3000);
                            return;
                        }

                        int priceMats = val * 4;

                        if (Fractions.Stocks.fracStocks[6].Money < priceMats)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "В казне нехватает денег", 3000);
                            return;
                        }

                        Fractions.Stocks.fracStocks[6].Money -= priceMats;

                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы заказали {val} шт. материалов", 3000);

                        player.SendChatMessage("Подлодка прибудет на пирс Сэнди-Шорс через 20 минут.");

                        NAPI.Task.Run(() =>
                        {
                            player.SendChatMessage("Подлодка прибыла на пирс Сэнди-Шорс. У вас есть 10 минут.");
                            Manager.StartGetMaterials(val);
                        }, 6000);*/

                        break;
                    case "fraction_order_meds":
                        int meds = 0;
                        if (!Int32.TryParse(text, out meds))
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Некорректные данные", 3000);
                            return;
                        }
                        GovernmentOrder.BuyMeds(player, meds);
                        /*if (meds < 1000 || meds > 20000)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Некорректные данные", 3000);
                            return;
                        }


                        int priceMeds = meds * 4;

                        if (Fractions.Stocks.fracStocks[6].Money < priceMeds)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "В казне нехватает денег", 3000);
                            return;
                        }

                        Fractions.Stocks.fracStocks[6].Money -= priceMeds;

                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы заказали {meds} шт. медикаментов", 3000);

                        player.SendChatMessage("Самолет прибудет в аэропорт Лос-Сантоса через 5 минут.");

                        NAPI.Task.Run(() =>
                        {
                            player.SendChatMessage("Самолет прибыл в аэропорт Лос-Сантоса. У вас есть 10 минут.");
                            Manager.StartGetMedikaments(meds);
                        }, 6000);//20 мин 60000 * 2*/

                        break;
                    case "client_policecall_reason":
                        //Trigger.PlayerEvent(player, "phoneHR");
                        if (text.Length == 0)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Введите причину вызова полиции", 3000);
                            return;
                        }
                        PoliceCallsManager.AddCall(player, text);
                        break;
                    default:
                        if (player.HasMyData("INPUTCALLBACK") && player.HasMyData("INPUTCALLBACKTYPE"))
                        {
                            TypeCode typeCode = System.Type.GetTypeCode(player.GetMyData<System.Type>("INPUTCALLBACKTYPE"));
                            player.ResetMyData("INPUTCALLBACKTYPE");
                            switch (typeCode)
                            {
                                case TypeCode.Int32:
                                    {
                                        Action<int> action = player.GetMyData<Action<int>>("INPUTCALLBACK");
                                        player.ResetMyData("INPUTCALLBACK");
                                        try
                                        {
                                            int valueInt = Convert.ToInt32(arguments[1]);
                                            action.Invoke(valueInt);
                                        }
                                        catch (Exception a)
                                        {
                                            Console.WriteLine(a.Message.ToString());
                                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Введите корректные данные", 3000);
                                            action.Invoke(0);
                                        }
                                        break;
                                    }
                                case TypeCode.String:
                                    {
                                        Action<string> action = player.GetMyData<Action<string>>("INPUTCALLBACK");
                                        player.ResetMyData("INPUTCALLBACK");
                                        try
                                        {
                                            string valueString = Convert.ToString(arguments[1]);
                                            action.Invoke(valueString);
                                        }
                                        catch (Exception)
                                        {
                                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Введите корректные данные", 3000);
                                            action.Invoke("");
                                        }
                                        break;
                                    }
                                default:
                                    {
                                        Log.Write($"Type {typeCode} not supported", nLog.Type.Warn);
                                        Action<object> action = player.GetMyData<Action<object>>("INPUTCALLBACK");
                                        player.ResetMyData("INPUTCALLBACK");
                                        try
                                        {
                                            action.Invoke(arguments[1]);
                                        }
                                        catch (Exception)
                                        {
                                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Введите корректные данные", 3000);
                                            action.Invoke(default(object));
                                        }
                                        break;
                                    }
                            }


                        }
                        break;
                }
            }
            catch (Exception e)
            {
                Log.Write($"inputCallback/{callback}/: {e.ToString()}\n{e.StackTrace}", nLog.Type.Error);
            }
        }
    }
}
