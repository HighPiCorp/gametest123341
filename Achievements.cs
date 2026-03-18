using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;
using NeptuneEvo;
using NeptuneEvo.Core;
using NeptuneEvo.GUI;
using NeptuneEvo.Houses;
using NeptuneEvo.SDK;
using Newtonsoft.Json;
using Trigger = NeptuneEvo.Trigger;

namespace client.Core
{
    public enum AchievementID
    {
        Barber,
        FirstTattoo,
        AllTattoo,
        RaceWinner,
        Newbie,
        Glutton,
        Water,
        Legalize,
        LvlUp,
        Calls,
        Meeting,
        CarLover,
        Startup,
        Criminal,
        BusMoney,
        MegaGamer,
        Rob,
        Arrest,
        Tickets,
        PoliceCalls,
        GoodDoctor,
        Reanim,
        MyZone,
        Terminator,
        InAllFractions,
        Electricity,
        TaxiCalls,
        Trucker,
        Clothes,
        WinEvents,
        TomasShelby,
        Worker,
        Stone,
        Lamw,
        Postal
    }

    public enum AchievementType
    {
        Normal,
        Lvl,
        Work,
        Hide
    }


    class Achievements : Script
    {
        private static nLog Log = new nLog("Achievements");

        public static Dictionary<AchievementID, Achievment> ServerAchivements = new Dictionary<AchievementID, Achievment>()
        {
            { AchievementID.Barber, new Achievment("Смена образа", new List<AchievmentStage>(){ new AchievmentStage("Сменить прическу в барбершопе", 1, money: 500)}) },
            { AchievementID.FirstTattoo, new Achievment("Первое тату", new List<AchievmentStage>(){ new AchievmentStage("Набить тату в тату-салоне", 1, money: 1000)}) },
            { AchievementID.AllTattoo, new Achievment("Живого места не осталось!", new List<AchievmentStage>(){ new AchievmentStage("Набить тату на каждой части тела", 1, donate: 100)}, AchievementType.Hide) },
            { AchievementID.RaceWinner, new Achievment("Шумахер", new List<AchievmentStage>(){ new AchievmentStage("Завершить гонку первым", 1, donate: 50)}, AchievementType.Hide) },
            { AchievementID.Newbie, new Achievment("Новичок", new List<AchievmentStage>(){ new AchievmentStage("Выполнить все начальные квесты", 1, money: 500)}) },
            { AchievementID.Glutton, new Achievment("Обжора", new List<AchievmentStage>(){
                    new AchievmentStage("Съесть 250 бургеров/пицц и прочей еды", 250, money: 5000),
                    new AchievmentStage("Съесть 750 бургеров/пицц и прочей еды", 750, money: 15000),
                    new AchievmentStage("Съесть 1500 бургеров/пицц и прочей еды", 1500, car: "stalion2"),
                }, AchievementType.Lvl) },
            { AchievementID.Water, new Achievment("Водохлеб", new List<AchievmentStage>(){
                    new AchievmentStage("Выпить 250 бутылок любой газировки", 250, money: 5000),
                    new AchievmentStage("Выпить 750 бутылок любой газировки", 750, money: 15000),
                    new AchievmentStage("Выпить 2000 бутылок любой газировки", 1500, car: "buffalo3")
                }, AchievementType.Lvl) },
            { AchievementID.Legalize, new Achievment("Legalize it!", new List<AchievmentStage>(){ new AchievmentStage("Скурить 420 косяков", 420, donate: 420)}) },
            { AchievementID.LvlUp, new Achievment("Все старше и старше", new List<AchievmentStage>(){
                    new AchievmentStage("Достигнуть 10 уровня", 10, donate: 200),
                    new AchievmentStage("Достигнуть 20 уровня", 20, car: "rrocket"),
                    new AchievmentStage("Достигнуть 30 уровня", 30, donate: 200),
                }, AchievementType.Lvl) },
            { AchievementID.Calls, new Achievment("Кто на другом проводе?", new List<AchievmentStage>(){ new AchievmentStage("Ответить на 100 звонков", 100, money: 2000)}) },
            { AchievementID.Meeting, new Achievment("Душа компании", new List<AchievmentStage>(){
                    new AchievmentStage("Познакомиться с 10-ю людьми ", 10, money: 500),
                    new AchievmentStage("Познакомиться с 50-ю людьми ", 50, money: 5000),
                    new AchievmentStage("Познакомиться с 150-ю людьми ", 150, donate: 50),
                }, AchievementType.Lvl) },
            { AchievementID.CarLover, new Achievment("Авто любитель", new List<AchievmentStage>(){
                    new AchievmentStage("Иметь в собественности 3 автомобиля", 3, money: 10000),
                    new AchievmentStage("Иметь в собественности 5 автомобилей", 5, donate: 100),
                    new AchievmentStage("Иметь в собественности 10 автомобилей", 10, donate: 100),
                }, AchievementType.Lvl) },
            { AchievementID.Startup, new Achievment("Старт-ап", new List<AchievmentStage>(){ new AchievmentStage("Приобрести бизнес", 1, donate: 200)}) },
            { AchievementID.Criminal, new Achievment("Криминальный авторитет", new List<AchievmentStage>(){ new AchievmentStage("Получить максимальный уровень розыска", 1, donate: 10)}) },
            { AchievementID.BusMoney, new Achievment("Копейка к копейке", new List<AchievmentStage>(){ new AchievmentStage("Потратить 5000 на поезди на автобусе", 5000, money: 1000)}) },
            { AchievementID.MegaGamer, new Achievment("Выпал из реальности", new List<AchievmentStage>(){ new AchievmentStage("Отыграть 23 часа за сутки", 1, money: 20000)}) },
            { AchievementID.Rob, new Achievment("А кому сейчас легко?", new List<AchievmentStage>(){
                    new AchievmentStage("Обокрасть 5 человек", 5, money: 1000),
                    new AchievmentStage("Обокрасть 25 человек", 25, money: 2500),
                    new AchievmentStage("Обокрасть 100 человек", 100, donate: 100),
                }, AchievementType.Lvl) },
            { AchievementID.Arrest, new Achievment("Я и есть закон", new List<AchievmentStage>(){ new AchievmentStage("Арестовать 100 правонарушителей", 100, donate: 150)}) },
            { AchievementID.Tickets, new Achievment("Непоколебимый коп", new List<AchievmentStage>(){ new AchievmentStage("Выписать 200 штрафов", 200, money: 12500)}) },
            { AchievementID.PoliceCalls, new Achievment("Пора бы и прибавку...", new List<AchievmentStage>(){ new AchievmentStage("Принять 400 полицейских вызовов", 400, money: 15000)}) },
            { AchievementID.GoodDoctor, new Achievment("Хороший доктор", new List<AchievmentStage>(){
                    new AchievmentStage("Вылечить 100 пациентов", 100, money: 2000),
                    new AchievmentStage("Вылечить 500 пациентов", 500, money: 10000),
                    new AchievmentStage("Вылечить 1000 пациентов", 1000, donate: 150),
                }, AchievementType.Lvl) },
            { AchievementID.Reanim, new Achievment("Не все герои носят плащи", new List<AchievmentStage>(){
                    new AchievmentStage("Реанимировать 50 человек", 50, money: 3500),
                    new AchievmentStage("Реанимировать 100 человек", 100, money: 7000),
                    new AchievmentStage("Реанимировать 250 человек", 250, money: 15000),
                }, AchievementType.Lvl) },
            { AchievementID.MyZone, new Achievment("Это моя территория", new List<AchievmentStage>(){ new AchievmentStage("Начать захват территории", 1, money: 3000)}) },
            { AchievementID.Terminator, new Achievment("Терминатор", new List<AchievmentStage>(){ new AchievmentStage("Убить 100 человек противоборствующей команды на захвате территории", 100, money: 10000)}) },
            { AchievementID.InAllFractions, new Achievment("Свой среди чужих", new List<AchievmentStage>(){ new AchievmentStage("Побывать во всех фракциях", 1, money: 1000)}, AchievementType.Hide) },
            { AchievementID.TaxiCalls, new Achievment("Крути баранку", new List<AchievmentStage>(){ new AchievmentStage("Принять 500 вызовов, работая таксистом", 500, money: 7500)}) },
            { AchievementID.Trucker, new Achievment("Где-то там далеко-далеко...", new List<AchievmentStage>(){
                    new AchievmentStage("Выполнить 25 заказов дальнобойщиком", 25, money: 5000),
                    new AchievmentStage("Выполнить 100 заказов дальнобойщиком", 100, car: "dloader"),
                    new AchievmentStage("Выполнить 225 заказов дальнобойщиком", 225, donate: 350),
                }, AchievementType.Lvl) },
            { AchievementID.Clothes, new Achievment("Модник", new List<AchievmentStage>(){
                    new AchievmentStage("Потратить на одежду 100000$", 100000, money: 5000),
                    new AchievmentStage("Потратить на одежду 500000$", 500000, donate: 100),
                    new AchievmentStage("Потратить на одежду 1000000$", 1000000, donate: 350),
                }, AchievementType.Lvl) },
           // { AchievementID.WinEvents, new Achievment("Мастер на все руки", new List<AchievmentStage>(){ new AchievmentStage("Победить в 75 крупных меропрятиях", 75, money: 3000)}) },
            { AchievementID.TomasShelby, new Achievment("Томас Шелби", new List<AchievmentStage>(){ new AchievmentStage("Убить 100 человек противоборствующей команды на Войне за бизнес", 100, money: 10000)}) },
            /*{ AchievementID.Worker, new Achievment("Парень работящий", new List<AchievmentStage>(){
                    new AchievmentStage("Перенести 100 ящиков", 100, money: 250),
                    new AchievmentStage("Перенести 500 ящиков", 500, money: 750),
                    new AchievmentStage("Перенести 1000 ящиков", 1000, money: 1500),
                }, AchievementType.Lvl) },
            { AchievementID.Stone, new Achievment("Работаю три дня и три ночи", new List<AchievmentStage>(){
                    new AchievmentStage("Перенести 100 камней", 100, money: 500),
                    new AchievmentStage("Перенести 500 камней", 500, money: 1250),
                    new AchievmentStage("Перенести 1000 камней", 1000, money: 2000),
                }, AchievementType.Lvl) },*/
           // { AchievementID.Lamw, new Achievment("Специалист по лужайкам", new List<AchievmentStage>(){ new AchievmentStage("Проехать через 12000 меток, работая газонокосильзиком", 12000, money: 7500)}) },
            { AchievementID.Postal, new Achievment("Ох уж этот Печкин", new List<AchievmentStage>(){ new AchievmentStage("Доставить 300 посылок", 300, money: 12000)}) },

        };

        public static List<Dictionary<string, object>> SeriliazeNormalAchivments(Player player)
        {
            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();

            foreach(KeyValuePair<AchievementID, Achievment> pair in ServerAchivements)
            {
                if(pair.Value.Type == AchievementType.Normal)
                {
                    list.Add(new Dictionary<string, object>()
                    {
                        {"title", pair.Value.Title },
                        {"descr", pair.Value.Stages[0].Description },
                        {"current", Main.Players[player].AchievementsScore[(int)pair.Key] >  pair.Value.Stages[0].Count ? pair.Value.Stages[0].Count : Main.Players[player].AchievementsScore[(int)pair.Key]  },
                        {"max", pair.Value.Stages[0].Count == 0 ? 1 : pair.Value.Stages[0].Count},
                        {"reward", GetAchivmentRewards(pair.Value.Stages[0]) }
                    });
                }
            }

            return list;
        }

        public static List<Dictionary<string, object>> SeriliazeLevelAchivments(Player player)
        {
            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();

            foreach (KeyValuePair<AchievementID, Achievment> pair in ServerAchivements)
            {
                if (pair.Value.Type == AchievementType.Lvl)
                {
                    list.Add(new Dictionary<string, object>()
                    {
                        {"title", pair.Value.Title },
                        {"stages",new Dictionary<string, object>(){
                            {"current", GetCurrentLevelStage(player, pair.Key, pair.Value) },
                            {"max", pair.Value.Stages.Count }
                        } },
                        {"current", Main.Players[player].AchievementsScore[(int)pair.Key] },
                        {"max", pair.Value.Stages[0].Count == 0 ? 1 : pair.Value.Stages[0].Count},
                        {"descr", pair.Value.Stages[0].Description },
                        {"reward", GetAchivmentRewardsStages(pair.Value.Stages) }
                    });
                }
            }

            return list;
        }

        public static int GetCurrentLevelStage(Player player, AchievementID aid, Achievment achiv)
        {
            int level = 0;

            for(int i = 0; i < achiv.Stages.Count; i++)
            {
                if (Main.Players[player].AchievementsScore[(int)aid] >= achiv.Stages[i].Count) level++;
            }

            return level;
        }

        public static List<Dictionary<string, object>> SeriliazeHideAchivments(Player player)
        {
            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();

            foreach (KeyValuePair<AchievementID, Achievment> pair in ServerAchivements)
            {
                if (pair.Value.Type == AchievementType.Hide)
                {
                    if (Main.Players[player].AchievementsScore[(int)pair.Key] == pair.Value.Stages[0].Count)
                    {
                        list.Add(new Dictionary<string, object>()
                        {
                            {"title", pair.Value.Title },
                            {"descr", pair.Value.Stages[0].Description },
                            {"current", Main.Players[player].AchievementsScore[(int)pair.Key] },
                            {"max", pair.Value.Stages[0].Count == 0 ? 1 : pair.Value.Stages[0].Count},
                            {"reward", GetAchivmentRewards(pair.Value.Stages[0]) }
                        });
                    }
                }
            }

            return list;
        }

        public static List<Dictionary<string, object>> GetAchivmentRewardsStages(List<AchievmentStage> stages)
        {
            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();

            foreach (AchievmentStage stage in stages)
            {
                if (stage.RewardMoney > 0)
                {

                    list.Add(new Dictionary<string, object>()
                    {
                        {"title", $"+ {stage.RewardMoney}$" },
                        {"max", stage.Count },
                        {"descr", stage.Description }
                    });
                }

                if (stage.RewardDonate > 0)
                {
                    list.Add(new Dictionary<string, object>()
                    {
                        {"title", $"+ {stage.RewardDonate} SWCoins" },
                        {"max", stage.Count },
                        {"descr", stage.Description },
                        {"imgSmall", true },
                    });
                }

                if (stage.RewardCar.Length > 1)
                {
                    list.Add(new Dictionary<string, object>()
                    {
                        {"title", $"{stage.RewardCar}" },
                        {"max", stage.Count },
                        {"descr", stage.Description },
                        {"img", stage.RewardCar.ToLower() },

                    });
                }
            }

            return list;
        }

        public static List<Dictionary<string, object>> GetAchivmentRewards(AchievmentStage stage)
        {
            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();

            if (stage.RewardMoney > 0)
            {
                list.Add(new Dictionary<string, object>()
                {
                    {"text", $"+ {stage.RewardMoney}$" }
                });
            }

            if (stage.RewardDonate > 0)
            {
                list.Add(new Dictionary<string, object>()
                {
                    {"img", true },
                    {"text", $"+ {stage.RewardDonate} SWCoins" }
                });
            }

            if (stage.RewardCar.Length > 1)
            {
                list.Add(new Dictionary<string, object>()
                {
                    {"text", $"{stage.RewardCar}" }
                });
            }

            return list;
        }

        public static List<Dictionary<string, object>> SeriliazeRewards(Player player)
        {
            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();


            foreach(NeptuneEvo.SDK.Reward reward in Main.Players[player].RewardsData)
            {
                if (reward.Type == RewardType.Donate)
                {
                    list.Add(new Dictionary<string, object>()
                    {
                        {"status", reward.Status },
                        {"title", reward.Name },
                        {"type", (int)reward.Type },
                        {"img", "cup" },
                        {"list", new List<Dictionary<string, object>>()
                            {
                              new Dictionary<string, object>(){
                                {"img", true },
                                {"text", $"{reward.Data} SWCoins" }
                              }
                            }
                        }
                    });
                }
                else if(reward.Type == RewardType.Money)
                {
                    list.Add(new Dictionary<string, object>()
                    {
                        {"status", reward.Status },
                        {"title", reward.Name },
                        {"type", (int)reward.Type },
                        {"img", "cup" },
                        {"list", new List<Dictionary<string, object>>()
                            {
                              new Dictionary<string, object>(){
                                {"text", $"{reward.Data}$" }
                              }
                            }
                        }
                    });
                }
                else if (reward.Type == RewardType.Clothes)
                {
                    list.Add(new Dictionary<string, object>()
                    {
                        {"status", reward.Status },
                        {"title", reward.Name },
                        {"type", (int)reward.Type },
                        {"img", "cup" },
                        {"list", new List<Dictionary<string, object>>()
                            {
                              new Dictionary<string, object>(){
                                {"text", $"Одежда" }
                              }
                            }
                        }
                    });
                }
                else if (reward.Type == RewardType.SilverVIP)
                {
                    list.Add(new Dictionary<string, object>()
                    {
                        {"status", reward.Status },
                        {"title", reward.Name },
                        {"type", reward.Type },
                        {"img", "cup" },
                        {"list", new List<Dictionary<string, object>>()
                            {
                              new Dictionary<string, object>(){
                                {"text", $"Silver VIP ({reward.Data} дня)" }
                              }
                            }
                        }
                    });
                }
                else if (reward.Type == RewardType.GoldVIP)
                {
                    list.Add(new Dictionary<string, object>()
                    {
                        {"status", reward.Status },
                        {"title", reward.Name },
                        {"type", reward.Type },
                        {"img", "cup" },
                        {"list", new List<Dictionary<string, object>>()
                            {
                              new Dictionary<string, object>(){
                                {"text", $"Gold VIP ({reward.Data} дня)" }
                              }
                            }
                        }
                    });
                }
                else if (reward.Type == RewardType.PlatinumVIP)
                {
                    list.Add(new Dictionary<string, object>()
                    {
                        {"status", reward.Status },
                        {"title", reward.Name },
                        {"type", reward.Type },
                        {"img", "cup" },
                        {"list", new List<Dictionary<string, object>>()
                            {
                              new Dictionary<string, object>(){
                                {"text", $"Platinum VIP ({reward.Data} дня)" }
                              }
                            }
                        }
                    });
                }
                else if (reward.Type == RewardType.CasinoChips)
                {
                    list.Add(new Dictionary<string, object>()
                    {
                        {"status", reward.Status },
                        {"title", reward.Name },
                        {"type", reward.Type },
                        {"img", "cup" },
                        {"list", new List<Dictionary<string, object>>()
                            {
                              new Dictionary<string, object>(){
                                {"text", $"{reward.Data} фишек" }
                              }
                            }
                        }
                    });
                }
                else if (reward.Type == RewardType.InventoryItem)
                {
                    int i = 0;
                    if (!int.TryParse(reward.Data.ToString(), out i)) continue;
                    list.Add(new Dictionary<string, object>()
                    {
                        {"status", reward.Status },
                        {"title", reward.Name },
                        {"type", reward.Type },
                        {"img", "cup" },
                        {"list", new List<Dictionary<string, object>>()
                            {
                              new Dictionary<string, object>(){
                                {"text", $"{nInventory.ItemsNames[i].ToString()}" }
                              }
                            }
                        }
                    }) ;
                }
                else
                {
                    list.Add(new Dictionary<string, object>()
                    {
                        {"status", reward.Status },
                        {"title", reward.Name },
                        {"type", (int)reward.Type },
                        {"img", "cup" },
                        {"list", new List<Dictionary<string, object>>()
                            {
                              new Dictionary<string, object>(){
                                {"text", $"{reward.Data}" }
                              }
                            }
                        }
                    });
                }
            }

            return list;
        }


        public static void AddAchievementScore(Player player, AchievementID achiv, int point)
        {
            if (!Main.Players.ContainsKey(player)) return;

            if (!ServerAchivements.ContainsKey(achiv)) return;

            Main.Players[player].AchievementsScore[(int)achiv] += point;

            foreach(AchievmentStage stage in ServerAchivements[achiv].Stages)
            {
                if(Main.Players[player].AchievementsScore[(int)achiv] >= stage.Count)
                {
                    // add prize
                    AddReward(player, ServerAchivements[achiv].Title, stage);
                }
            }
        }

        public static void RemoveAchievementScore(Player player, AchievementID achiv, int point)
        {
            if (!Main.Players.ContainsKey(player)) return;

            if (!ServerAchivements.ContainsKey(achiv)) return;

            Main.Players[player].AchievementsScore[(int)achiv] -= point;
        }

        public static void AddReward(Player player, string name, AchievmentStage stage)
        {
            NeptuneEvo.SDK.Reward reward;

            foreach (Reward rew in Main.Players[player].RewardsData)
            {
                if (rew.Name == name && rew.Description == stage.Description) return;
            }

            if (stage.RewardCar.Length > 1)
                reward = new NeptuneEvo.SDK.Reward(name, stage.Description, true, NeptuneEvo.SDK.RewardType.Car, stage.RewardCar);
            else if (stage.RewardDonate > 0)
                reward = new NeptuneEvo.SDK.Reward(name, stage.Description, true, NeptuneEvo.SDK.RewardType.Donate, stage.RewardDonate);
            else if (stage.RewardClothes != "")
                reward = new NeptuneEvo.SDK.Reward(name, stage.Description, true, NeptuneEvo.SDK.RewardType.Clothes, stage.RewardClothes);
            else
                reward = new NeptuneEvo.SDK.Reward(name, stage.Description, true, NeptuneEvo.SDK.RewardType.Money, stage.RewardMoney);

            Main.Players[player].RewardsData.Add(reward);

            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы получили награду за выполение достижения {name}", 3000);
        }

        [RemoteEvent("SERVER::REWARD:GET")]
        public static void GetReward(Player player, string name, string prize, int type)
        {
            if (player == null || !Main.Players.ContainsKey(player)) return;

            List<Reward> rewards = Main.Players[player].RewardsData;

            foreach(Reward reward in rewards)
            {
                if(reward.Name == name && reward.Status && reward.Type == (RewardType)type)
                {
                    if(reward.Type == RewardType.Money || reward.Type == RewardType.Donate)
                    {
                        int count = reward.Type == RewardType.Donate ? Convert.ToInt32(prize.Split(" ")[0]) : reward.Type == RewardType.Money ? Convert.ToInt32(prize.Substring(0, prize.Length - 1)) : Convert.ToInt32(prize.Substring(0, prize.Length - 1));
                        if (count == reward.Data)
                        {
                            if(reward.Type == RewardType.Money)
                            {
                                NeptuneEvo.MoneySystem.Wallet.Change(player, +count);
                                GameLog.Money($"server", $"player({Main.Players[player].UUID})", reward.Data, $"Achiev({reward.Name})");
                                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы получили {reward.Data}$!", 3000);
                            }
                            else if (reward.Type == RewardType.Donate)
                            {
                                var before = Main.Accounts[player].RedBucks;
                                Main.Accounts[player].RedBucks += count;
                                Log.Debug($"[SWC Changes][{player.Name}] [Achievements] Награда: [{Convert.ToInt32(reward.Data)}] {before} -> {Main.Accounts[player].RedBucks}");
                                GameLog.SWC(Main.Players[player].UUID, "[Achievements] Награда", Main.Accounts[player].Login, Convert.ToInt32(reward.Data), before);
                                
                                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы получили {Convert.ToInt32(reward.Data)} SWCoins!", 3000);
                                //GameLog.Money($"server", $"player({Main.Players[player].UUID})", reward.Data, $"Achiev({reward.Name})");
                            }

                            reward.Status = false;
                        }
                    }
                    else if(reward.Type == RewardType.Car)
                    {
                        if(prize == reward.Data)
                        {
                            int vNumber = -1;

                            var house = HouseManager.GetHouse(player, true);
                            if (house == null || house.GarageID == 0)
                            {
                                if (VehicleManager.getAllPlayerVehicles(player.Name).Count >= GarageManager.MAX_VEHICLES_WITHOUT_GARAGE)
                                {
                                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Чтобы иметь более двух автомобилией, приобретите дом с местом для машины", 3000);
                                    return;
                                }

                                vNumber = VehicleManager.Create(player.Name, Main.Players[player].UUID, reward.Data, new Color(225, 225, 225), new Color(225, 225, 225), new Color(0, 0, 0));

                                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы получили {reward.Data}!", 3000);
                                reward.Status = false;
                            }
                            else
                            {
                                var garage = GarageManager.Garages[house.GarageID];
                                // Проверка свободного места в гараже
                                if (VehicleManager.getAllPlayerVehicles(player.Name).Count >= GarageManager.GarageTypes[garage.Type].MaxCars)
                                {
                                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Ваши гаражи полны", 3000);
                                    return;
                                }

                                vNumber = VehicleManager.Create(player.Name, Main.Players[player].UUID, reward.Data, new Color(225, 225, 225), new Color(225, 225, 225), new Color(0, 0, 0));
                                garage.SpawnCar(vNumber);
                                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы получили {reward.Data}! Машина заспавнена в гараже!", 3000);
                                reward.Status = false;
                            }
                        }
                    }
                    else if(reward.Type == RewardType.CasinoChips)
                    {

                            nInventory.Add(player, new nItem(ItemType.CasinoChips, Convert.ToInt32(reward.Data)));

                            reward.Status = false;
                            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы получили {reward.Data} фишек!", 3000);

                    }
                    else if (reward.Type == RewardType.Clothes)
                    {

                        var clothesData = reward.Data.ToString().Split('_');
                        Customization.AddClothes(player, ItemType.Top, Convert.ToInt32(clothesData[0]), Convert.ToInt32(clothesData[1]));

                        reward.Status = false;
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы получили одежду!", 3000);

                    }
                    else if (reward.Type == RewardType.VIP)
                    {
                        if(Main.Players[player].VipLvl > 2)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "У вас уже есть VIP лучше");
                            return;
                        }
                        Main.Players[player].VipLvl = 2;
                        Main.Players[player].VipDate = DateTime.Now.AddDays(reward.Data);

                        reward.Status = false;
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы получили Silver VIP!", 3000);

                    }
                    else if (reward.Type == RewardType.SilverVIP)
                    {
                        if (Main.Players[player].VipLvl > 2)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "У вас уже есть VIP лучше");
                            return;
                        }
                        else if (Main.Players[player].VipLvl == 2)
                        {
                            Main.Players[player].VipDate = Main.Players[player].VipDate.AddDays(reward.Data);
                        }
                        else
                        {
                            Main.Players[player].VipLvl = 2;
                            Main.Players[player].VipDate = DateTime.Now.AddDays(reward.Data);
                        }

                        reward.Status = false;
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы получили Silver VIP!", 3000);

                    }
                    else if (reward.Type == RewardType.GoldVIP)
                    {
                        if (Main.Players[player].VipLvl > 3)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "У вас уже есть VIP лучше");
                            return;
                        }
                        else if (Main.Players[player].VipLvl == 3)
                        {
                            Main.Players[player].VipDate = Main.Players[player].VipDate.AddDays(reward.Data);
                        }
                        else
                        {
                            Main.Players[player].VipLvl = 3;
                            Main.Players[player].VipDate = DateTime.Now.AddDays(reward.Data);
                        }

                        reward.Status = false;
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы получили Gold VIP!", 3000);

                    }
                    else if (reward.Type == RewardType.PlatinumVIP)
                    {
                        if (Main.Players[player].VipLvl > 4)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "У вас уже есть VIP лучше");
                            return;
                        }
                        else if (Main.Players[player].VipLvl == 4)
                        {
                            Main.Players[player].VipDate = Main.Players[player].VipDate.AddDays(reward.Data);
                        }
                        else
                        {
                            Main.Players[player].VipLvl = 4;
                            Main.Players[player].VipDate = DateTime.Now.AddDays(reward.Data);
                        }

                        reward.Status = false;
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы получили Platinum VIP!", 3000);

                    }
                    else if (reward.Type == RewardType.InventoryItem)
                    {
                        int i = 0;
                        if (!int.TryParse(reward.Data.ToString(), out i)) return;
                        nItem item = new nItem((ItemType)i, 1);
                        nInventory.Add(player, item);

                        reward.Status = false;

                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы получили {nInventory.ItemsNames[i]}!", 3000);

                    }

                    break;
                }
            }

            Dashboard.sendStats(player);
            Dictionary<string, object> dict = new Dictionary<string, object>();

            dict.Add("reward", client.Core.Achievements.SeriliazeRewards(player));

            Trigger.ClientEvent(player, "CLIENT::STATS:UPDATE", JsonConvert.SerializeObject(dict));

        }

        //public static void
    }

    class Achievment
    {
        public string Title;

        public AchievementType Type;

        public List<AchievmentStage> Stages;

        public Achievment(string title, List<AchievmentStage> stages, AchievementType type = AchievementType.Normal)
        {
            Title = title;
            Stages = stages;
            Type = type;
        }
    }

    class AchievmentStage
    {
        public string Description;

        public int Count;

        public string RewardCar;

        public int RewardMoney;

        public int RewardDonate;

        public string RewardClothes;
        public AchievmentStage(string desc, int count, string car = "", int money = 0, int donate = 0, string clothes = "")
        {
            Description = desc;
            Count = count;
            RewardCar = car;
            RewardMoney = money;
            RewardDonate = donate;
            RewardClothes = clothes;
        }
    }
}
