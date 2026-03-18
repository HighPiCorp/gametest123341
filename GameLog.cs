using client.Systems.BattlePass;
using client.Systems.BattlePass.Rewards;
using NeptuneEvo.SDK;
using Newtonsoft.Json;

namespace NeptuneEvo.Core
{
    public class GameLog
    {

        private static Thread thread;
        private static nLog Log = new nLog("GameLog");
        private static Queue<string> queue = new Queue<string>();
        private static Dictionary<int, DateTime> OnlineQueue = new Dictionary<int, DateTime>();
        private static string timer = null;

        private static Config config = new Config("MySQL");

        private static string DBMain = config.TryGet<string>("DataBase", "");
        private static string DB = config.TryGet<string>("DataBase", "") + "logs";

        private static string insert = "insert into " + DB + ".{0}({1}) values ({2})";
        private static string select = "select * from " + DB + ".{0} {1}";

        public class Message
        {
            public Message(string author, bool isAdmin, string text)
            {
                Author = author;
                IsAdmin = isAdmin;
                Text = text;
            }
            public string Author;
            public bool IsAdmin;
            public string Text;

            public object[] ToObject()
            {
                return new object[] { IsAdmin, Author, Text };
            }
            public string ToJson()
            {
                return JsonConvert.SerializeObject(ToObject());
            }
        }

        public static void Votes(uint ElectionId, string Login, string VoteFor)
        {
            if (thread == null) return;
            queue.Enqueue(string.Format(
                insert, "votelog", "`election`,`login`,`votefor`,`time`", $"'{ElectionId}','{Login}','{VoteFor}','{DateTime.Now.ToString("s")}'"));
        }
        public static void Stock(int Frac, int Uuid, string Type, int Amount, bool In)
        {
            if (thread == null) return;
            queue.Enqueue(string.Format(
                insert, "stocklog", "`time`,`frac`,`uuid`,`type`,`amount`,`in`", $"'{DateTime.Now.ToString("s")}',{Frac},{Uuid},'{Type}',{Amount},{In}"));
        }
        public static void Admin(string Admin, string Action, string Player)
        {
            if (thread == null) return;
            queue.Enqueue(string.Format(
                insert, "adminlog", "`time`,`admin`,`action`,`player`", $"'{DateTime.Now.ToString("s")}','{Admin}','{Action}','{Player}'"));
        }
        public static void Promocode(string Player, string Login, string Promocode, string Action, string Description)
        {
            if (thread == null) return;
            queue.Enqueue(string.Format(
                insert, "promocodelog", "`time`,`promocode`,`action`,`player`,`login`,`description`", $"'{DateTime.Now.ToString("s")}','{Promocode}','{Action}','{Player}','{Login}','{Description}'"));
        }
        public static void BattlePassLog(string Player, int UUID, BPReward rewardItem, int bpType, string Action)
        {
            if (thread == null) return;
            if (rewardItem == null) return;

            var rewardLVL = rewardItem.ID;
            var rewardCount = rewardItem.Count;
            var rewardDesc = rewardItem.Description;
            var rewardType = rewardItem.RewardType;
            var rewardRare = rewardItem.Rare;
            var reward = rewardItem.Type;
            var rewardName = rewardItem.Name;
            var sprayEXP = rewardItem.SprayExp;
            var uuid = UUID;
            var playerName = Player;
            var addinitional = JsonConvert.SerializeObject(rewardItem.Additional);
            var rewardBPType = bpType;

            queue.Enqueue(string.Format(
                insert, "bp__rewardslog", "`time`,`playerName`,`uuid`,`rewardName`,`rewardCount`,`rewardLVL`,`rewardBPType`,`rewardAddinitional`,`action`",
                $"'{DateTime.Now.ToString("s")}','{playerName}','{UUID}','{rewardName}','{rewardCount}','{rewardLVL}','{rewardBPType}','{addinitional}','{Action}'"));
        }

        public static void BattlePassLog(string Player, int UUID, BPPlayerReward playerRewardItem, int bpType, string Action)
        {
            if (thread == null) return;
            if (playerRewardItem == null) return;

            List<BPReward> rewardList = null;

            switch (bpType)
            {
              case 0:
                rewardList = BPRewards.SilverRewards.FindAll(r => r.Season == BattlePass.BPCurrentSeasonID);
                break;
              case 1:
                rewardList = BPRewards.GoldRewards.FindAll(r => r.Season == BattlePass.BPCurrentSeasonID);
                break;
              case 2:
                rewardList = BPRewards.PlatinumRewards.FindAll(r => r.Season == BattlePass.BPCurrentSeasonID);
                break;
            }

            if (rewardList == null) return;

            BPReward rewardItem = rewardList.FirstOrDefault(r => r.ID == playerRewardItem.ID);

            if(rewardItem == null) return;

            var rewardLVL = rewardItem.ID;
            var rewardCount = rewardItem.Count;
            var rewardDesc = rewardItem.Description;
            var rewardType = rewardItem.RewardType;
            var rewardRare = rewardItem.Rare;
            var reward = rewardItem.Type;
            var rewardName = rewardItem.Name;
            var sprayEXP = rewardItem.SprayExp;
            var uuid = UUID;
            var playerName = Player;
            var addinitional = JsonConvert.SerializeObject(rewardItem.Additional);
            var rewardBPType = bpType;

            queue.Enqueue(string.Format(
                insert, "bp__rewardslog", "`time`,`playerName`,`uuid`,`rewardName`,`rewardCount`,`rewardLVL`,`rewardBPType`,`rewardAddinitional`,`action`",
                $"'{DateTime.Now.ToString("s")}','{playerName}','{UUID}','{rewardName}','{rewardCount}','{rewardLVL}','{rewardBPType}','{addinitional}','{Action}'"));
        }
        public static void Donate(int DonateID, string Action, string PlayerLogin)
        {
            if (thread == null) return;
            queue.Enqueue(string.Format(
                insert, "donatelog", "`time`,`donateID`,`action`,`login`", $"'{DateTime.Now.ToString("s")}','{DonateID}','{Action}','{PlayerLogin}'"));
        }

        public static void SWC(int UUID, string Action, string PlayerLogin, int Count, long Before)
        {
            if (thread == null) return;
            queue.Enqueue(string.Format(
                insert, "swclog", "`date`,`uuid`,`action`,`login`,`count`, `before`", $"'{DateTime.Now.ToString("s")}','{UUID}','{Action}','{PlayerLogin}','{Count}','{Before.ToString()}'"));
        }
        /// <summary>
        /// Формат для From и To:
        /// Для игрока - player(UUID).
        /// Для бизнеса - biz(ID).
        /// Для банка - bank(UUID).
        /// Для сервисов и услуг - произвольно.
        /// Пример: Money("donate","player(1)",100500)
        /// </summary>
        public static void Money(string From, string To, long Amount, string Comment)
        {
            if (thread == null) return;
            queue.Enqueue(string.Format(
                insert, "moneylog", "`time`,`from`,`to`,`amount`,`comment`", $"'{DateTime.Now.ToString("s")}','{From}','{To}',{Amount.ToString()},'{Comment}'"));
        }
        public static void Items(string From, string To, int Type, int Amount, string Data)
        {
            if (thread == null) return;
            queue.Enqueue(string.Format(
                insert, "itemslog", "`time`,`from`,`to`,`type`,`amount`,`data`", $"'{DateTime.Now.ToString("s")}','{From}','{To}',{Type},{Amount},'{Data}'"));
        }
        public static void Name(int Uuid, string Old, string New)
        {
            if (thread == null) return;
            queue.Enqueue(string.Format(
                insert, "namelog", "`time`,`uuid`,`old`,`new`", $"'{DateTime.Now.ToString("s")}',{Uuid},'{Old}','{New}'"));
        }
        /// <summary>
        /// Лог банов
        /// </summary>
        /// <param name="Admin">UUID админа</param>
        /// <param name="Player">UUID игрока</param>
        public static void Ban(int Admin, int Player, DateTime Until, string Reason, bool isHard)
        {
            if (thread == null) return;
            queue.Enqueue(string.Format(
                insert, "banlog", "`time`,`admin`,`player`,`until`,`reason`,`ishard`", $"'{DateTime.Now.ToString("s")}',{Admin},{Player},'{Until.ToString("s")}','{Reason}',{isHard}"));
        }
        public static void Ticket(int player, int target, int sum, string reason, string pnick, string tnick)
        {
            if (thread == null) return;
            queue.Enqueue(string.Format(
                insert, "ticketlog", "`time`,`player`,`target`,`sum`,`reason`,`pnick`,`tnick`", $"'{DateTime.Now.ToString("s")}',{player},{target},{sum},'{reason}','{pnick}','{tnick}'"));
        }
        public static void Arrest(int player, int target, string reason, int stars, string pnick, string tnick)
        {
            if (thread == null) return;
            queue.Enqueue(string.Format(
                insert, "arrestlog", "`time`,`player`,`target`,`reason`,`stars`,`pnick`,`tnick`", $"'{DateTime.Now.ToString("s")}',{player},{target},'{reason}',{stars},'{pnick}','{tnick}'"));
        }
        public static void Connected(string Name, int Uuid, string SClub, string Hwid, int Id, string ip)
        {
            if (thread == null || OnlineQueue.ContainsKey(Uuid)) return;
            DateTime now = DateTime.Now;

            queue.Enqueue(string.Format(
                insert, "connlog", "`in`,`out`,`uuid`,`sclub`,`hwid`,`ip`", $"'{now.ToString("s")}',null,'{Uuid}','{SClub}','{Hwid}','{ip}'"));
            queue.Enqueue(string.Format(
                insert, "idlog", "`in`,`out`,`uuid`,`id`,`name`", $"'{now.ToString("s")}',null,'{Uuid}','{Id}','{Name}'"));
            queue.Enqueue($"insert into {DBMain}.online set `uuid`='{Uuid}', `ip`='{ip}', `in`='{now.ToString("s")}'");
            OnlineQueue.Add(Uuid, now);
        }
        public static void Disconnected(int Uuid)
        {
            if (thread == null || !OnlineQueue.ContainsKey(Uuid)) return;
            DateTime conn = OnlineQueue[Uuid];
            if (conn == null) return;
            OnlineQueue.Remove(Uuid);
            queue.Enqueue($"update {DB}.connlog set `out`='{DateTime.Now.ToString("s")}' WHERE `in`='{conn.ToString("s")}' and `uuid`={Uuid}");
            queue.Enqueue($"DELETE FROM {DBMain}.online WHERE `uuid`='{Uuid}' AND `in`='{conn.ToString("s")}'");
            //queue.Enqueue($"update masklog set `out`='{DateTime.Now.ToString("s")}' WHERE `in`='{conn.ToString("s")}' and `uuid`={Uuid}");
        }
        public static void CharacterDelete(string name, int uuid, string account)
        {
            if (thread == null) return;
            queue.Enqueue(string.Format(
                insert, "deletelog", "`time`,`uuid`,`name`,`account`", $"'{DateTime.Now.ToString("s")}',{uuid},'{name}','{account}'"));
        }
        public static void EventLogAdd(string AdmName, string EventName, ushort MembersLimit, string Started)
        {
            if (thread == null) return;
            queue.Enqueue(string.Format(
                insert, "eventslog", "`AdminStarted`,`EventName`,`MembersLimit`,`Started`", $"'{AdmName}','{EventName}','{MembersLimit}','{Started}'"));
        }
        public static void EventLogUpdate(string AdmName, int MembCount, string WinName, uint Reward, string Time, uint RewardLimit, ushort MemLimit, string EvName)
        {
            if (thread == null) return;
            queue.Enqueue($"update {DB}.eventslog set `AdminClosed`='{AdmName}',`Members`={MembCount},`Winner`='{WinName},`Reward`={Reward},`Ended`='{Time}',`RewardLimit`={RewardLimit} WHERE `Winner`='Undefined' AND `MembersLimit`={MemLimit} AND `EventName`='{EvName}'");
        }

        #region CityRP

          public static void Auction(string id, string str, int amount, string action)
          {
              if (thread == null) return;
              queue.Enqueue(string.Format(
                  insert, "auctionslog", "`time`,`id`,`str`,`amount`,`action`", $"'{DateTime.Now.ToString("s")}','{id}','{str}',{amount},'{action}'"));
          }
          /// <summary>
          /// Лог трейда
          /// </summary>
          /// <param name="Admin">UUID админа</param>
          /// <param name="Player">UUID игрока</param>
          public static void Trade(int firstId, string firstName, string firstIp, string firstItems, int firstMoney, int secondId, string secondName, string secondIp, string secondItems, int secondMoney)
          {
              if (thread == null) return;
              queue.Enqueue(string.Format(
                  insert, "tradelog", "`time`,`first_id`,`first_name`,`first_ip`,`first_items`,`first_money`,`second_id`,`second_name`,`second_ip`,`second_items`,`second_money`",
                  $"'{DateTime.Now.ToString("s")}','{firstId}','{firstName}','{firstIp}','{firstItems}','{firstMoney}','{secondId}','{secondName}','{secondIp}','{secondItems}','{secondMoney}'"));
          }

          public static void CasinoPlacedBet(string name, int uuid, ushort red, ushort zero, ushort black)
        {
            if (thread == null) return;
            queue.Enqueue(string.Format(
                insert, "casinobetlog", "`time`,`name`,`uuid`,`red`,`zero`,`black`",
                $"'{DateTime.Now.ToString("s")}','{name}',{uuid},'{red}','{zero}','{black}'"));
        }
        public static void CasinoEnd(string name, int uuid, byte casino, byte disctype)
        {
            if (thread == null) return;
            queue.Enqueue(string.Format(
                insert, "casinoendlog", "`time`,`name`,`uuid`,`state`,`type`",
                $"'{DateTime.Now.ToString("s")}','{name}',{uuid},{casino},{disctype}"));
        }
        public static void CasinoWinLose(string name, int uuid, int wonbet)
        {
            if (thread == null) return;
            queue.Enqueue(string.Format(
                insert, "casinowinloselog", "`time`,`name`,`uuid`,`wonbet`",
                $"'{DateTime.Now.ToString("s")}','{name}',{uuid},{wonbet}"));
        }
        public static void FractionMember(int fractionId, string playerName, string uuid, string targetName, string targetuuid, int type, string text)
        {
            if (thread == null) return;
            queue.Enqueue(string.Format(
                insert, "fractionmemberlog", "`date`,`fractionid`,`player`,`uuid`,`target`,`targetuuid`,`type`,`text`",
                $"'{DateTime.Now.ToString("s")}','{fractionId}','{playerName}','{uuid}','{targetName}','{targetuuid}','{type}','{text}'"));
        }
        public static void FractionMoney(int fractionId, string playerName, string uuid, int amount, string text)
        {
            if (thread == null) return;
            queue.Enqueue(string.Format(
                insert, "fractionmoneylog", "`date`,`fractionid`,`player`,`uuid`,`amount`,`text`",
                $"'{DateTime.Now.ToString("s")}','{fractionId}','{playerName}','{uuid}','{amount}','{text}'"));
        }
        public static void FractionStock(int fractionId, string playerName, string uuid, int itemType, int amount, int type, string text)
        {
            if (thread == null) return;
            queue.Enqueue(string.Format(
                insert, "fractionstocklog", "`date`,`fractionid`,`player`,`uuid`,`itemtype`,`amount`,`type`,`text`",
                $"'{DateTime.Now.ToString("s")}','{fractionId}','{playerName}','{uuid}','{itemType}','{amount}','{type}','{text}'"));
        }

        public static void Report(DateTime createDate, DateTime endDate, string uuid, string name, string adminUuid, string adminName, List<Message> messages, int star)
        {
            if (thread == null) return;
            string message = JsonConvert.SerializeObject(messages);
            while (message.Length > 65535)
            {
                messages.RemoveAt(0);
                message = JsonConvert.SerializeObject(messages);
            }
            queue.Enqueue(string.Format(
                insert, "reportlog", "`createdate`, `enddate`, `uuid`, `name`, `adminuuid`, `adminname`, `messages`, `star`",
                $"'{createDate.ToString("s")}', '{endDate.ToString("s")}', '{uuid}', '{name}', '{adminUuid}', '{adminName}', '{message}', {star}"));
        }

        public static System.Data.DataTable QueryRead(string table, string where = "")
        {
            return MySQL.QueryRead(string.Format(select, table, where));
        }

        #endregion




    #region Логика потока
    public static void Start()
        {
            thread = new Thread(new ThreadStart(Worker));
            thread.IsBackground = true;
            thread.Start();
        }
        private static void Worker()
        {
            string CMD = "";
            try
            {
                Log.Debug("Worker started");
                //while (true)
                //{
                //    if (queue.Count < 1) continue;
                //    else
                //        MySQL.Query(queue.Dequeue());
                //}

                timer = Timers.StartTask(500, () => TimerExec());
            }
            catch (Exception e)
            {
                Log.Write($"{e.ToString()}\n{CMD}", nLog.Type.Error);
            }
        }
        private static void TimerExec()
        {
            var list = queue.ToList();

            if (list.Any())
            {
                MySQL.Query(queue.Dequeue());
            }
        }
        public static void Stop()
        {
            Timers.Stop(timer);
            thread.Join();
        }
        #endregion
    }
}
