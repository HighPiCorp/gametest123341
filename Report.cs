using System;
using System.Data;
using GTANetworkAPI;
using Newtonsoft.Json;
using MySqlConnector;
using NeptuneEvo.SDK;
using System.Collections.Generic;
using NeptuneEvo.GUI;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace NeptuneEvo.Core
{
    class ReportSys : Script
    {
        public class ReportMessage
        {
            public string Name { get; set; }
            public string Text { get; set; }
            public DateTime Date { get; set; }
            public bool IsAdmin { get; set; }

        }
        public class Report
        {
            public int ID { get; set; }
            public string Author { get; set; }
            public string AuthorPersonID { get; set; }
            public List<ReportMessage> Messages { get; set; }
            public string BlockedBy { get; set; }

            public DateTime OpenedDate { get; set; }
            public DateTime ClosedDate { get; set; }

            public bool Status { get; set; }
            public string Admin { get; set; } = null;
            public bool ClosedByAdmin { get; set; } = false;
            public bool Hide { get; set; } = false;

            public void Send(Player someone = null)
            {
                if (someone == null)
                {
                    foreach (Player target in NAPI.Pools.GetAllPlayers())
                    {
                        if (!Main.Players.ContainsKey(target)) continue;
                        if (Main.Players[target].AdminLVL < adminLvL) continue;

                        //T/rigger.ClientEvent(target, "addreport", ID, Author, Question);
                    }
                }
                else
                {
                    if (!Main.Players.ContainsKey(someone)) return;
                    if (Main.Players[someone].AdminLVL < adminLvL) return;

                    //rigger.ClientEvent(someone, "addreport", ID, Author, Question);
                }
            }
        }
        public static Dictionary<int, Report> Reports;
        private static nLog Log = new nLog("ReportSys");
        private static Config conf = new Config("ReportSys");

        private static int adminLvL = conf.TryGet<int>("AdminLvL", 1);

        public static void Init()
        {
            try
            {
                Reports = new Dictionary<int, Report>();

                string cmd = @"SELECT * FROM questions;";

                DataTable result = MySQL.QueryRead(cmd);
                if (result is null) return;
                foreach(DataRow row in result.Rows)
                {
                    //if (Convert.ToBoolean((sbyte)row[7]) != false) continue;

                    Reports.Add((int)row[0], new Report
                    {
                        ID = (int)row[0],
                        Author = row[1].ToString(),
                        BlockedBy = row[2].ToString(),
                        OpenedDate = (DateTime)row[3],
                        ClosedDate = (DateTime)row[4],
                        Status = Convert.ToBoolean((sbyte)row[5]),
                        Messages = JsonConvert.DeserializeObject<List<ReportMessage>>(row[6].ToString()),
                        Hide = Convert.ToBoolean((sbyte)row[7]),
                        AuthorPersonID = row[8].ToString(),
                    });
                }

            } catch(Exception e)
            {
                Log.Write("Init: " + e.ToString(), nLog.Type.Error);
            }
        }
        public static void onAdminLoad(Player client)
        {
            try
            {
                foreach (Report report in Reports.Values)
                {
                    report.Send(client);
                }

            } catch(Exception e)
            {
                Log.Write("onAdminLoad: " + e.ToString(), nLog.Type.Error);
            }
        }

        #region Remote Events
        //Админ взял репорт на себя
        [RemoteEvent("takereport")]
        public void ReportTake(Player client, int id, bool retrn = false)
        {
            if (Main.Players[client].AdminLVL <= 0) return;
            Log.Debug($"Report take: {id} {retrn}");
            if (!Reports.ContainsKey(id))
            {
                Remove(id, client);
                return;
            }

            if (Reports[id].Status)
            {
                Remove(id, client);
                return;
            }

            foreach (Player target in NAPI.Pools.GetAllPlayers())
            {
                if (!Main.Players.ContainsKey(target)) continue;
                if (Main.Players[target].AdminLVL < adminLvL) continue;

                if (retrn) Trigger.ClientEvent(target, "setreport", id, "");
                else Trigger.ClientEvent(target, "setreport", id, client.Name);
            }
        }
        [RemoteEvent("sendreport")]
        public void ReportSend(Player player, int ID, string answer)
        {
            if (Main.Players[player].AdminLVL <= 0) return;
            Log.Debug($"Report send: {ID} {answer}");
            if (!Reports.ContainsKey(ID)) return;
            if (!Reports[ID].Status)
            {
                AddAnswer(player, ID, answer);
            }
            else
            {
                player.SendChatMessage("Эта жалоба более недоступна для изменения.");
                Remove(ID, player);
            }
        }
        #endregion


        [Command("report", GreedyArg = true)]
        public static void CreateReportCmd(Player player, string str)
        {
            CreateReport(player, str);
        }

        [RemoteEvent("SERVER::REPORT:CHANGE_REPORT")]
        public static void ChangeReport(Player player, int id)
        {
            player.SetMyData("SELECTED_REPORT", id);

            UpdatePlayerReport(player);
        }

        /*[Command("createreport")]
        public static void CreateRep(Player player)
        {
            CreateReport(player);
        }*/

        [RemoteEvent("SERVER::REPORT:CREATE_REPORT")]
        public static void CreateReport(Player player, string text = "Мне нужна помощь")
        {
            // message = Main.BlockSymbols(message);
            if (player == null) return;
            if (!Main.Players.ContainsKey(player)) return;

            if(player.HasMyData("NEXT_REPORT"))
                if(player.GetMyData<DateTime>("NEXT_REPORT") > DateTime.Now)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Попробуйте позже", 3000);
                    return;
                }   

            player.SetMyData("NEXT_REPORT", DateTime.Now.AddMinutes(1));
            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы отправили репорт", 3000);
            player.SetMyData("IS_REPORT", true);

            List<ReportMessage> list = new List<ReportMessage>();

            list.Add(new ReportMessage
            {
                Name = player.Name,
                Text = text,
                IsAdmin = false,
                Date = DateTime.Now,
            });

            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = "INSERT INTO `questions` (`Author`,`Messages`,`Opened`,`Closed`,`AuthorPID`) VALUES (@pn,@q,@time,@ntime,@pid); SELECT LAST_INSERT_ID();";
            cmd.Parameters.AddWithValue("@pn", player.Name);
            cmd.Parameters.AddWithValue("@q", JsonConvert.SerializeObject(list));
            cmd.Parameters.AddWithValue("@time", MySQL.ConvertTime(DateTime.Now));
            cmd.Parameters.AddWithValue("@ntime", MySQL.ConvertTime(DateTime.MinValue));
            cmd.Parameters.AddWithValue("@pid", Main.Players[player].PersonID);
            DataTable dt = MySQL.QueryRead(cmd);

            int id = Convert.ToInt32(dt.Rows[0][0]);
            Report report = new Report
            {
                ID = id,
                Author = player.Name,
                AuthorPersonID = Main.Players[player].PersonID,
                BlockedBy = "",
                Messages = list,
                Status = false,
                OpenedDate = DateTime.Now,
                ClosedDate = DateTime.MinValue
            };
            report.Send();
            Reports.Add(id, report);


            UpdatePlayerReport(player);

            foreach (Player p in Main.Players.Keys.ToList())
            {
                if (!Main.Players.ContainsKey(p)) continue;
                if (Main.Players[p].AdminLVL >= 1)
                {
                    p.SendChatMessage($"~r~Репорт от {player.Name.Replace('_', ' ')} (ID {player.Value})");
                    p.TriggerEvent("CLIENT::REPORT:PLAY_SOUND");
                    UpdatePlayerReport(p);
                }
            }
        }

        [RemoteEvent("SERVER::REPORT:GET_WORK")]
        public static void GetWorkReport(Player player, int id)
        {
            if (!Reports.ContainsKey(id)) return;

            if (Reports[id].Admin == null)
            {
                Reports[id].Admin = player.Name;

                Player target = NAPI.Player.GetPlayerFromName(Reports[id].Author);

                if(target != null) UpdatePlayerReport(target, true);

                foreach (Player p in Main.Players.Keys.ToList())
                {
                    if (!Main.Players.ContainsKey(p)) continue;
                    if (Main.Players[p].AdminLVL >= 1)
                    {
                        UpdatePlayerReport(p);
                    }
                }
            }
        }


        [RemoteEvent("SERVER::REPORT:SPECTATE")]
        public static void SpectatePlayer(Player player)
        {
            if (!player.HasMyData("SELECTED_REPORT")) return;

            int id = player.GetMyData<int>("SELECTED_REPORT");

            Player target = NAPI.Player.GetPlayerFromName(Reports[id].Author);

            if (target is null) return;

            AdminSP.Spectate(player, target.Value);
        }

        [RemoteEvent("SERVER::REPORT:TP")]
        public static void TeleportToplayer(Player player)
        {
            if (!player.HasMyData("SELECTED_REPORT")) return;

            int id = player.GetMyData<int>("SELECTED_REPORT");

            Player target = NAPI.Player.GetPlayerFromName(Reports[id].Author);

            if (target is null) return;

            Commands.CMD_teleport(player, target.Value);
        }

        [RemoteEvent("SERVER::REPORT:SEND_MESSAGE")]
        public static void AddReport(Player player, string message)
        {
            try
            {
                if (!player.HasMyData("SELECTED_REPORT")) return;

                int id = player.GetMyData<int>("SELECTED_REPORT");

                if (!Reports.ContainsKey(id)) return;

                if (Main.Players[player].AdminLVL > 0)
                {
                    AddAnswer(player, id, message);
                }
                else
                {

                    Report report = Reports[id];

                    if (report.Status) return;

                    report.Messages.Add(new ReportMessage
                    {
                        Name = player.Name,
                        Text = message,
                        IsAdmin = false,
                        Date = DateTime.Now,
                    });

                    report.Send();

                    foreach (var p in Main.Players.Keys.ToList())
                    {
                        if (Main.Players[p].AdminLVL >= 1)
                        {
                            UpdatePlayerReport(p);
                        }
                    }

                    if(report.Admin != null) UpdatePlayerReport(report.Admin, true);

                    UpdatePlayerReport(player);
                }
            }
            catch (Exception e)
            {
                Log.Write(e.ToString(), nLog.Type.Error);
            }
        }

        private static void AddAnswer(Player player, int repID, string response)
        {
            try
            {
                response = Main.BlockSymbols(response);

                if (!Main.Players.ContainsKey(player)) return;
                if (Main.Players[player].AdminLVL < adminLvL) return;

                if (!Reports.ContainsKey(repID)) return;

                if (Reports[repID].Admin != null && Reports[repID].Admin != player.Name) return;

                DateTime now = DateTime.Now;

                try
                {
                    Player target = NAPI.Player.GetPlayerFromName(Reports[repID].Author);
                    if (target is null)
                    {
                        Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, "Игрок не найден!", 3000);
                    }
                    else
                    {
                       // target.SendChatMessage($"~r~Администратор ответил на ваш репорт");
                        Notify.Send(target, NotifyType.Info, NotifyPosition.BottomCenter, $"Администратор ответил на ваш репорт", 5000);
                        foreach (var p in Main.Players.Keys.ToList())
                        {
                            if (Main.Players[p].AdminLVL >= adminLvL)
                            {
                                p.SendChatMessage($"~y~[ANSWER] {player.Name}({player.Value})->{target.Name}({target.Value}): {response}");
                            }
                        }
                        GameLog.Admin(player.Name, $"answer({response})", target.Name);
                    }
                }
                catch (Exception ex)
                {
                    Log.Write($"PlayerAnswer:\n" + ex.ToString(), nLog.Type.Error);
                }


                Reports[repID].Messages.Add(new ReportMessage
                {
                    Name = player.Name,
                    Text = response,
                    IsAdmin = true,
                    Date = DateTime.Now,
                });

                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = "UPDATE questions SET Respondent=@resp,Messages=@mes,Status=@st WHERE ID=@repid LIMIT 1";
                cmd.Parameters.AddWithValue("@resp", player.Name);
                cmd.Parameters.AddWithValue("@mes", JsonConvert.SerializeObject(Reports[repID].Messages));
                cmd.Parameters.AddWithValue("@st", false);
                cmd.Parameters.AddWithValue("@time", MySQL.ConvertTime(now));
                cmd.Parameters.AddWithValue("@repid", repID);
                MySQL.Query(cmd);

                UpdatePlayerReport(Reports[repID].Author, true);

                UpdatePlayerReport(player);
            }
            catch (Exception e)
            {
                Log.Write(e.ToString(), nLog.Type.Error);
            }
        }

        [RemoteEvent("SERVER::REPORT:ROLLBACK")]
        private static void RollbackReport(Player player)
        {
            if (!player.HasMyData("SELECTED_REPORT")) return;

            int id = player.GetMyData<int>("SELECTED_REPORT");

            if (!Reports.ContainsKey(id)) return;

            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = "UPDATE questions SET Respondent=@resp,Messages=@mes,Status=@st,Closed=@time WHERE ID=@repid LIMIT 1";
            cmd.Parameters.AddWithValue("@resp", player.Name);
            cmd.Parameters.AddWithValue("@mes", JsonConvert.SerializeObject(Reports[id].Messages));
            cmd.Parameters.AddWithValue("@st", false);
            cmd.Parameters.AddWithValue("@time", MySQL.ConvertTime(DateTime.Now));
            cmd.Parameters.AddWithValue("@repid", id);
            MySQL.Query(cmd);

            Reports[id].ClosedDate = DateTime.Now;
            Reports[id].Status = false;
            Reports[id].Admin = null;

           /* ReportMessage mes = new ReportMessage
            {
                Name = Reports[id].Messages[0].Name, Text = Reports[id].Messages[0].Text, IsAdmin = Reports[id].Messages[0].IsAdmin, Date = Reports[id].Messages[0].Date
            };

            Reports[id].Messages.Clear();

            Reports[id].Messages.Add(mes);*/

            Player target = NAPI.Player.GetPlayerFromName(Reports[id].Admin);
            Player sender = NAPI.Player.GetPlayerFromName(Reports[id].Author);

            if (target != null) UpdatePlayerReport(target);

            if(sender != null) UpdatePlayerReport(sender);

            foreach (var p in Main.Players.Keys.ToList())
            {
                if (Main.Players[p].AdminLVL >= 1)
                {
                    UpdatePlayerReport(p);
                }
            }

        }


        [RemoteEvent("SERVER::REPORT:CLOSE")]
        private static void CloseReport(Player player, int id)
        {
            if (!Reports.ContainsKey(id)) return;

            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = "UPDATE questions SET Respondent=@resp,Messages=@mes,Status=@st,Closed=@time WHERE ID=@repid LIMIT 1";
            cmd.Parameters.AddWithValue("@resp", player.Name);
            cmd.Parameters.AddWithValue("@mes", JsonConvert.SerializeObject(Reports[id].Messages));
            cmd.Parameters.AddWithValue("@st", true);
            cmd.Parameters.AddWithValue("@time", MySQL.ConvertTime(DateTime.Now));
            cmd.Parameters.AddWithValue("@repid", id);
            MySQL.Query(cmd);

            if (Main.Players[player].AdminLVL > 0)
            {
                Reports[id].ClosedByAdmin = true;
            }
            else
            {
                Reports[id].ClosedByAdmin = false;
            }

            Reports[id].ClosedDate = DateTime.Now;
            Reports[id].Status = true;

            Player target = NAPI.Player.GetPlayerFromName(Reports[id].Admin);
            Player sender = NAPI.Player.GetPlayerFromName(Reports[id].Author);

            if (target != null)
            {
                target.ResetMyData("SELECTED_REPORT");
                UpdatePlayerReport(target);
            }

            if (sender != null)
            {
                sender.ResetMyData("SELECTED_REPORT");
                UpdatePlayerReport(sender);
            }


            foreach (var p in Main.Players.Keys.ToList())
            {
                if (Main.Players[p].AdminLVL >= 1)
                {
                    UpdatePlayerReport(p);
                }
            }

        }

        [RemoteEvent("SERVER::REPORT:DELETE")]
        private static void DeleteReport(Player player, int id)
        {
            if (!Reports.ContainsKey(id)) return;

            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = "UPDATE questions SET Respondent=@resp,Messages=@mes,Status=@st,Closed=@time,Hide=@hide WHERE ID=@repid LIMIT 1";
            cmd.Parameters.AddWithValue("@resp", player.Name);
            cmd.Parameters.AddWithValue("@mes", JsonConvert.SerializeObject(Reports[id].Messages));
            cmd.Parameters.AddWithValue("@st", true);
            cmd.Parameters.AddWithValue("@time", MySQL.ConvertTime(DateTime.Now));
            cmd.Parameters.AddWithValue("@repid", id);
            cmd.Parameters.AddWithValue("@hide", true);
            MySQL.Query(cmd);

            Reports[id].ClosedDate = DateTime.Now;
            Reports[id].Hide = true;
            Reports[id].Status = true;

            Player target = NAPI.Player.GetPlayerFromName(Reports[id].Admin);
            Player sender = NAPI.Player.GetPlayerFromName(Reports[id].Author);

            if (target != null)
            {
                target.ResetMyData("SELECTED_REPORT");
                UpdatePlayerReport(target);
            }

            if (sender != null)
            {
                sender.ResetMyData("SELECTED_REPORT");
                UpdatePlayerReport(sender);
            }

            foreach (var p in Main.Players.Keys.ToList())
            {
                if (Main.Players[p].AdminLVL >= 1)
                {
                    UpdatePlayerReport(p);
                }
            }
        }

        public static void UpdatePlayerReport(Player player, bool sound = false)
        {
            if (!Main.Players.ContainsKey(player)) return;

            Dictionary<string, object> dict = new Dictionary<string, object>();

            dict.Add("report", Dashboard.GetSerializeReports(player));

            Trigger.ClientEvent(player, "CLIENT::STATS:UPDATE", JsonConvert.SerializeObject(dict));

            if (sound) player.TriggerEvent("CLIENT::REPORT:PLAY_SOUND");
        }

        public static void UpdatePlayerReport(string name, bool sound = false)
        {
            Player target = NAPI.Player.GetPlayerFromName(name);

            if (target is null) return;

            Dictionary<string, object> dict = new Dictionary<string, object>();

            dict.Add("report", Dashboard.GetSerializeReports(target));

            Trigger.ClientEvent(target, "CLIENT::STATS:UPDATE", JsonConvert.SerializeObject(dict));

            if(sound) target.TriggerEvent("CLIENT::REPORT:PLAY_SOUND");
        }

        private static void Remove(int ID_, Player someone = null)
        {
            try
            {
                Log.Debug($"Remove {ID_}");
                if (someone == null)
                {
                    foreach (Player target in NAPI.Pools.GetAllPlayers())
                    {
                        if (!Main.Players.ContainsKey(target)) continue;
                        if (Main.Players[target].AdminLVL < adminLvL) continue;

                        Trigger.ClientEvent(target, "delreport", ID_);
                    }
                }
                else
                {
                    if (!Main.Players.ContainsKey(someone)) return;
                    if (Main.Players[someone].AdminLVL < adminLvL) return;

                    Trigger.ClientEvent(someone, "delreport", ID_);
                }
                Reports.Remove(ID_);
            }
            catch (Exception e)
            {
                Log.Write(e.ToString(), nLog.Type.Error);
            }
        }
    }
}
