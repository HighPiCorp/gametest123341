using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using GTANetworkAPI;
using MySqlConnector;
using NeptuneEvo.SDK;
using Newtonsoft.Json;

namespace NeptuneEvo.Core
{
    public class BanData
    {
        public int UUID;
        public string Name;
        public string Account;
        public DateTime Time;
        public DateTime Until;
        public bool isHard;
        public string IP;
        public string SocialClub;
        public string HWID;
        public string Reason;
        public string ByAdmin;
    }
    class Ban : BanData
    {
        public static List<Ban> Banned = new List<Ban>();
        private static nLog Log = new nLog("BanSystem");

        // Синхронизация с базой
        public static void Sync()
        {
            lock (Banned)
            {
                Banned.Clear();
                DataTable result = MySQL.QueryRead("select * from banned");
                if (result == null || result.Rows.Count == 0) return;
                foreach (DataRow row in result.Rows)
                {
                    Banned.Add(new Ban()
                    {
                        UUID = Convert.ToInt32(row["uuid"]),
                        Name = Convert.ToString(row["name"]),
                        Account = Convert.ToString(row["account"]),
                        Time = Convert.ToDateTime(row["time"]),
                        Until = Convert.ToDateTime(row["until"]),
                        isHard = Convert.ToBoolean(row["ishard"]),
                        IP = Convert.ToString(row["ip"]),
                        SocialClub = Convert.ToString(row["socialclub"]),
                        HWID = Convert.ToString(row["hwid"]),
                        Reason = Convert.ToString(row["reason"]),
                        ByAdmin = Convert.ToString(row["byadmin"])
                    });
                }
            }
        }

        #region Всякие проверки
        // Проверяем на совпадение HWID и IP адресов
        public static Ban Get1(Player client)
        {
            lock (Banned)
            {
                Ban ban = null;
                if(client.HasMyData("RealSocialClub")) {
                    ban = Banned.FindLast(x => x.SocialClub == client.GetMyData<string>("RealSocialClub"));
                    if (ban != null) return ban;
                }
                ban = Banned.FindLast(x => x.IP == client.Address);
                if (ban != null) return ban;
                if(client.HasMyData("RealHWID")) ban = Banned.FindLast(x => x.HWID == client.GetMyData<string>("RealHWID"));
                return ban;
            }
        }

        // Поиск по UUID персонажа
        public static Ban Get2(int UUID)
        {
            lock (Banned)
            {
                Ban ban = null;
                ban = Banned.Find(x => x.UUID == UUID);
                return ban;
            }
        }
        // проверка даты или удаляем
        public bool CheckDate()
        {
            if (DateTime.Now <= Until)
            {
                return true;
            }
            else
            {
                //MySQL.Query($"DELETE FROM banned WHERE uuid={this.UUID}");

                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = "DELETE FROM `banned` WHERE `uuid`=@uuid";

                cmd.Parameters.AddWithValue("@uuid", this.UUID);
                MySQL.Query(cmd);

                lock (Banned)
                {
                    Banned.Remove(this);
                }
                return false;
            }
        }
        #endregion

        #region Выдача бана
        public static void Online(Player client, DateTime until, bool ishard, string reason, string admin)
        {
            var acc = Main.Players[client];

            if (acc == null) {
                Log.Write($"Can't ban player {client.Name}", nLog.Type.Error);
                return;
            }

            Ban ban = new Ban()
            {
                UUID = acc.UUID,
                Name = acc.FirstName + "_" + acc.LastName,
                Account = Main.Accounts[client].Login,
                Time = DateTime.Now,
                Until = until,
                isHard = ishard,
                IP = client.Address,
                SocialClub = client.GetData<string>("RealSocialClub"),
                HWID = client.GetData<string>("RealHWID"),
                Reason = reason,
                ByAdmin = admin
            };


            //MySQL.Query("INSERT INTO `banned`(`uuid`,`name`,`account`,`time`,`until`,`ishard`,`ip`,`socialclub`,`hwid`,`reason`,`byadmin`) " +
            //    $"VALUES ({ban.UUID},'{ban.Name}','{ban.Account}','{MySQL.ConvertTime(ban.Time)}','{MySQL.ConvertTime(ban.Until)}',{ban.isHard},'{ban.IP}','{ban.SocialClub}','{ban.HWID}','{ban.Reason}','{ban.ByAdmin}')");

            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = "INSERT INTO `banned` SET " +
              "`uuid`=@uuid," +
              "`name`=@name," +
              "`account`=@account," +
              "`time`=@time," +
              "`until`=@until," +
              "`ishard`=@ishard," +
              "`ip`=@ip," +
              "`socialclub`=@socialclub," +
              "`hwid`=@hwid," +
              "`reason`=@reason," +
              "`byadmin`=@byadmin";

            cmd.Parameters.AddWithValue("@uuid", ban.UUID);
            cmd.Parameters.AddWithValue("@name", ban.Name);
            cmd.Parameters.AddWithValue("@account", ban.Account);
            cmd.Parameters.AddWithValue("@time", MySQL.ConvertTime(ban.Time));
            cmd.Parameters.AddWithValue("@until", MySQL.ConvertTime(ban.Until));
            cmd.Parameters.AddWithValue("@ishard", ban.isHard);
            cmd.Parameters.AddWithValue("@ip", ban.IP);
            cmd.Parameters.AddWithValue("@socialclub", ban.SocialClub);
            cmd.Parameters.AddWithValue("@hwid", ban.HWID);
            cmd.Parameters.AddWithValue("@reason", ban.Reason);
            cmd.Parameters.AddWithValue("@byadmin", ban.ByAdmin);

            MySQL.Query(cmd);


            Banned.Add(ban);
        }

        public static void UpdateBan(int uuid)
        {
            var ban = Banned.FirstOrDefault(b => b.UUID == uuid);
            if (ban == null) return;

            //MySQL.Query($"UPDATE `banned` SET `account`='{ban.Account}',`hwid`='{ban.HWID}' WHERE `uuid`={uuid}");

            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = "UPDATE `banned` SET `account`=@account, `hwid`=@hwid WHERE `uuid`=@uuid";

            cmd.Parameters.AddWithValue("@account", ban.Account);
            cmd.Parameters.AddWithValue("@hwid", ban.HWID);
            cmd.Parameters.AddWithValue("@uuid", uuid);
            MySQL.Query(cmd);
        }
        public static bool Offline(string nickname, DateTime until, bool ishard, string reason, string admin)
        {
            if (Banned.FirstOrDefault(b => b.Name == nickname) != null) return false;

            if (!Main.PlayerUUIDs.ContainsKey(nickname)) return false;

            var uuid = Main.PlayerUUIDs[nickname];
            if (uuid == -1) return false;

            var ip = "";
            var socialclub = "";
            var account = "";
            var hwid = "";

            if (ishard)
            {
                //DataTable result = MySQL.QueryRead($"SELECT `hwid`,`socialclub`,`ip`,`login` FROM `accounts` WHERE `character1`={uuid} OR `character2`={uuid} OR `character3`={uuid}");
                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = "SELECT `hwid`,`socialclub`,`ip`,`login` FROM `accounts` WHERE `character1`=@uuid OR `character2`=@uuid OR `character3`=@uuid";

                cmd.Parameters.AddWithValue("@uuid", uuid);
                DataTable result = MySQL.QueryRead(cmd);

                var row = result.Rows[0];
                if (result == null || result.Rows.Count == 0) return false;
                ip = row["ip"].ToString();
                socialclub = row["socialclub"].ToString();
                account = row["login"].ToString();
                hwid = row["hwid"].ToString();
            }

            Ban ban = new Ban()
            {
                UUID = uuid,
                Name = nickname,
                Account = account,
                Time = DateTime.Now,
                Until = until,
                isHard = ishard,
                IP = ip,
                SocialClub = socialclub,
                HWID = hwid,
                Reason = reason,
                ByAdmin = admin
            };


            //MySQL.Query("INSERT INTO `banned`(`uuid`,`name`,`account`,`time`,`until`,`ishard`,`ip`,`socialclub`,`hwid`,`reason`,`byadmin`) " +
            //    $"VALUES ({ban.UUID},'{ban.Name}','{ban.Account}','{MySQL.ConvertTime(ban.Time)}','{MySQL.ConvertTime(ban.Until)}',{ban.isHard},'{ban.IP}','{ban.SocialClub}','{ban.HWID}','{ban.Reason}','{ban.ByAdmin}')");

            MySqlCommand cmd2 = new MySqlCommand();
            cmd2.CommandText = "INSERT INTO `banned` SET " +
              "`uuid`=@uuid," +
              "`name`=@name," +
              "`account`=@account," +
              "`time`=@time," +
              "`until`=@until," +
              "`ishard`=@ishard," +
              "`ip`=@ip," +
              "`socialclub`=@socialclub," +
              "`hwid`=@hwid," +
              "`reason`=@reason," +
              "`byadmin`=@byadmin";

            cmd2.Parameters.AddWithValue("@uuid", ban.UUID);
            cmd2.Parameters.AddWithValue("@name", ban.Name);
            cmd2.Parameters.AddWithValue("@account", ban.Account);
            cmd2.Parameters.AddWithValue("@time", MySQL.ConvertTime(ban.Time));
            cmd2.Parameters.AddWithValue("@until", MySQL.ConvertTime(ban.Until));
            cmd2.Parameters.AddWithValue("@ishard", ban.isHard);
            cmd2.Parameters.AddWithValue("@ip", ban.IP);
            cmd2.Parameters.AddWithValue("@socialclub", ban.SocialClub);
            cmd2.Parameters.AddWithValue("@hwid", ban.HWID);
            cmd2.Parameters.AddWithValue("@reason", ban.Reason);
            cmd2.Parameters.AddWithValue("@byadmin", ban.ByAdmin);

            MySQL.Query(cmd2);

            Banned.Add(ban);
            return true;
        }
        #endregion

        #region Снять хардбан
        public static bool PardonHard(string nickname)
        {
            lock (Banned)
            {
                int index = Banned.FindIndex(x => x.Name == nickname);
                Log.Debug($"BANNED [{nickname}]: "+JsonConvert.SerializeObject(Banned.FirstOrDefault(x => x.Name == nickname)));

                if (index == -1) return false;

                Banned[index].isHard = false;
                //MySQL.Query($"UPDATE banned SET ishard={false} WHERE name='{nickname}'");

                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = "UPDATE `banned` SET `ishard`=@ishard WHERE `name`=@name";

                cmd.Parameters.AddWithValue("@ishard", false);
                cmd.Parameters.AddWithValue("@name", nickname);
                MySQL.Query(cmd);
                return true;
            }
        }
        public static bool PardonHard(int uuid)
        {
            lock (Banned)
            {
                int index = Banned.FindIndex(x => x.UUID == uuid);
                if (index == -1) return false;

                Banned[index].isHard = false;
                //MySQL.Query($"UPDATE banned SET ishard={false} WHERE uuid={uuid}");

                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = "UPDATE `banned` SET `ishard`=@ishard WHERE `uuid`=@uuid";

                cmd.Parameters.AddWithValue("@ishard", false);
                cmd.Parameters.AddWithValue("@uuid", uuid);
                MySQL.Query(cmd);

                return true;
            }
        }
        #endregion

        #region Снять бан
        public static bool Pardon(string nickname)
        {
            lock (Banned)
            {
                int index = Banned.FindIndex(x => x.Name == nickname);
                if (index == -1) return false;

                Banned.RemoveAt(index);
                //MySQL.Query($"DELETE FROM banned WHERE name='{nickname}'");

                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = "DELETE FROM `banned` WHERE `name`=@name";

                cmd.Parameters.AddWithValue("@ishard", false);
                cmd.Parameters.AddWithValue("@name", nickname);
                MySQL.Query(cmd);

                return true;
            }
        }
        public static bool Pardon(int uuid)
        {
            lock (Banned)
            {
                int index = Banned.FindIndex(x => x.UUID == uuid);
                if (index == -1) return false;

                Banned.RemoveAt(index);
                //MySQL.Query($"DELETE FROM banned WHERE uuid={uuid}");

                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = "DELETE FROM `banned` WHERE `uuid`=@uuid";

                cmd.Parameters.AddWithValue("@ishard", false);
                cmd.Parameters.AddWithValue("@uuid", uuid);
                MySQL.Query(cmd);

                return true;
            }
        }
        #endregion
    }
}
