using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;
using NeptuneEvo.SDK;
using Newtonsoft.Json;
using NeptuneEvo.Jobs;
using System.Data;
using client.GUI;
using NeptuneEvo.Houses;
using MySqlConnector;
using System.Reflection;

namespace NeptuneEvo.Core
{
    class QuestSystem : Script
    {
        private static nLog Log = new nLog("QuestSystem");

        public static Dictionary<Player, QuestInfo> QuestPanel = new Dictionary<Player, QuestInfo>();
        public static Dictionary<Player, CurrentChapter> Chapter = new Dictionary<Player, CurrentChapter>();
        //public static CurrentQuest Quest = new CurrentQuest();

        #region Colshapes Coords

        private static Dictionary<int, ColShape> Cols = new Dictionary<int, ColShape>();
        public static Vector3 QuestStart = new Vector3(122.7863, 6627.422, 30.808537); // start     0
        public static Vector3 QuestToLS = new Vector3(145.58028, 6563.2944, 31.994741); // ToLS     1
        public static Vector3 QuestLS = new Vector3(303.4681, -766.4683, 28.39071); // LS       2
        public static Vector3 QuestToSimShop = new Vector3(77.886665, -226.0953, 54.642086); // QuestToSimShop     3

        #endregion

        #region NPC

        public static Dictionary<int, ObjectQuestNPC> QuestNPCObject = new Dictionary<int, ObjectQuestNPC>();

        public static string StartNPCName = "Тревор";
        public static Vector3 StartNPC = QuestStart;
        public static string DeliveryNPCName = "Джейкоб";
        public static Vector3 DeliveryNPC = new Vector3(145.0623, -1061.4994, 29.17235);
        public static string ClothesNPCName = "Майкл";
        public static Vector3 ClothesNPC = new Vector3(416.6589, -811.478, 29.315677);
        public static string RaceNPCName = "Митчел";
        public static Vector3 RaceNPC = new Vector3(-2294.3765, 4252.08, 42.61832);
        public static string RaceNPCEndName = "Дейв";
        public static Vector3 RaceNPCEnd = new Vector3(2577.7432, 305.9174, 108.60873);
        public static string AutoSalonName = "Призовой автосалон";
        public static Vector3 AutoSalon = Race.racePrizeColshape;
        public static string MeriaNPCName = "Барри";
        public static Vector3 MeriaNPC = new Vector3(-517.2929, -252.24718, 35.66219);

    #endregion

        [ServerEvent(Event.ResourceStart)]
        public void onResourceStart()
        {
            try
            {
                loadQuests();

                #region QuestInteractColshape
                    NAPI.TextLabel.CreateTextLabel(Main.StringToU16($"Нажмите Е\n~b~Тревор"), StartNPC + new Vector3(0, 0, 1.125), 5F, 0.8F, 0, new Color(255, 255, 255));
                    NAPI.TextLabel.CreateTextLabel(Main.StringToU16($"Нажмите Е\n~g~Смотритель гонки"), new Vector3(-2294.3765, 4252.08, 43.61832), 5F, 0.8F, 0, new Color(255, 255, 255), true, NAPI.GlobalDimension);

                    var blip = NAPI.Blip.CreateBlip(QuestStart, 0);
                    blip.ShortRange = true;
                    blip.Name = Main.StringToU16("Тревор");
                    blip.Sprite = 480;
                    blip.Color = 82;

                    blip = NAPI.Blip.CreateBlip(DeliveryNPC, 0);
                    blip.ShortRange = true;
                    blip.Name = Main.StringToU16(DeliveryNPCName);
                    blip.Sprite = 480;
                    blip.Color = 82;

                    var colIndex = 0;
                    Cols.Add(colIndex, NAPI.ColShape.CreateCylinderColShape(QuestStart, 1, 15, 0));
                    Cols[colIndex].SetData("INTERACT", 350);
                    Cols[colIndex].SetData("Q_NPC", StartNPCName);
                    Cols[colIndex].OnEntityEnterColShape += onEntityEnterColshape;
                    Cols[colIndex].OnEntityExitColShape += onEntityExitColshape;

                    colIndex++;
                    Cols.Add(colIndex, NAPI.ColShape.CreateCylinderColShape(QuestToLS, 1, 15, 0));
                    Cols[colIndex].SetData("INTERACT", 1060);
                    Cols[colIndex].OnEntityEnterColShape += onEntityEnterColshapeToLS;
                    Cols[colIndex].OnEntityExitColShape += onEntityExitColshapeToLS;

                    colIndex++;
                    Cols.Add(colIndex, NAPI.ColShape.CreateCylinderColShape(QuestLS, 1, 15, 0));

                    colIndex++;
                    Cols.Add(colIndex, NAPI.ColShape.CreateCylinderColShape(QuestToSimShop, 1, 15, 0));

                    DataTable questNPCResult = MySQL.QueryRead($"SELECT * FROM `quest_npc`");
                    if (questNPCResult != null)
                    {
                        foreach (DataRow Row in questNPCResult.Rows)
                        {
                            int id = Convert.ToInt32(Row["id"]);
                            string name = Convert.ToString(Row["name"]);
                            int interaction = Convert.ToInt32(Row["interaction"]);
                            Vector3 position = JsonConvert.DeserializeObject<Vector3>(Row["position"].ToString());

                            colIndex++;
                            Cols.Add(colIndex, NAPI.ColShape.CreateCylinderColShape(position, 1, 15, 0));
                            Cols[colIndex].SetData("INTERACT", interaction);
                            Cols[colIndex].SetData("Q_NPC", name);
                            Cols[colIndex].OnEntityEnterColShape += onEntityEnterColshape;
                            Cols[colIndex].OnEntityExitColShape += onEntityExitColshape;

                            QuestNPCObject.Add(id, new ObjectQuestNPC(id, name, interaction, position));
                        }
                    }


                #endregion
            }
            catch (Exception e) { Log.Write("ResourceStart: " + e.StackTrace, nLog.Type.Error); }
        }

        public static List<int> ServerQuests = new List<int>();
        public static Dictionary<int, ChapterInfo> ServerQuestChapters = new Dictionary<int, ChapterInfo>();
        public static Dictionary<int, int> ServerLastChapter = new Dictionary<int, int>();

        #region Собираем масивы квестов и частей.

        public static void loadQuests()
        {
            DataTable quests = MySQL.QueryRead($"SELECT * FROM `quests`");
            if (quests == null || quests.Rows.Count == 0) return;

            try
            {
                // Добавляем ID квестов. (Пока у нас 1)
                foreach (DataRow row in quests.Rows) {
                    int questID = Convert.ToInt32(row["id"]);
                    ServerQuests.Add(questID);

                    loadChaptersFromQuest(questID);
                }
            }
            catch (Exception e) { Log.Write($"[loadQuests] " + e.StackTrace, nLog.Type.Error); }
        }

        public static void loadChaptersFromQuest(int questID)
        {
            //DataTable quests_relation = MySQL.QueryRead($"SELECT * FROM `quest_relation` WHERE `id__quest` = {questID} ORDER BY `id__chapter` ASC");

            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = "SELECT * FROM `quest_relation` WHERE `id__quest`=@id__quest ORDER BY `id__chapter` ASC";

            cmd.Parameters.AddWithValue("@id__quest", questID);
            DataTable quests_relation = MySQL.QueryRead(cmd);

            if (quests_relation == null || quests_relation.Rows.Count == 0) return;

            try
            {
                // Добавляем части квестов.
                foreach (DataRow row in quests_relation.Rows) {
                    int chapterID = Convert.ToInt32(row["id__chapter"]);

                    //DataTable quest_chapter = MySQL.QueryRead($"SELECT * FROM `quest_chapters` WHERE `id` = '{chapterID}'");
                    MySqlCommand cmd2 = new MySqlCommand();
                    cmd2.CommandText = "SELECT * FROM `quest_chapters` WHERE `id`=@id";

                    cmd2.Parameters.AddWithValue("@id", chapterID);
                    DataTable quest_chapter = MySQL.QueryRead(cmd2);

                    if (quest_chapter == null || quest_chapter.Rows.Count == 0) continue;

                    int id = chapterID;
                    int bizID = Convert.ToInt32(quest_chapter.Rows[0]["bizID"]);
                    string colshape_name = Convert.ToString(quest_chapter.Rows[0]["colshape_name"]);
                    int colshape_interaction = Convert.ToInt32(quest_chapter.Rows[0]["colshape_interaction"]);
                    string uniq_name = Convert.ToString(quest_chapter.Rows[0]["uniq_name"]);
                    string name = Convert.ToString(quest_chapter.Rows[0]["name"]);
                    string description = Convert.ToString(quest_chapter.Rows[0]["description"]);
                    int progress = Convert.ToInt32(quest_chapter.Rows[0]["progress"]);
                    int reward = Convert.ToInt32(quest_chapter.Rows[0]["reward"]);
                    int rewardExp = Convert.ToInt32(quest_chapter.Rows[0]["reward_exp"]);
                    Vector3 waypoint = JsonConvert.DeserializeObject<Vector3>(quest_chapter.Rows[0]["enterpoint"].ToString());

                    var chapterInfo = new ChapterInfo(id, name, bizID, uniq_name, colshape_name, colshape_interaction, description, reward, rewardExp, progress, waypoint);

                    ServerQuestChapters.Add(chapterID, chapterInfo);

                    if (!ServerLastChapter.ContainsKey(questID)) ServerLastChapter.Add(questID, chapterID);
                    else if (ServerLastChapter[questID] < chapterID) ServerLastChapter[questID] = chapterID;
                }
            }
            catch (Exception e) { Log.Write($"[loadChaptersFromQuest] " + e.StackTrace, nLog.Type.Error); }
        }

        #endregion

        public static void loadPlayerQuest(Player player)
        {
            if (Main.Players[player].Quest == 0) return;
            if (Main.Players[player].QuestChapter == 0 && Main.Players[player].QuestEndChapter == 0) return;

            // Если квест завершен, то зачем нам его грузить)
            if (Main.Players[player].QuestFinished == Main.Players[player].Quest) return;

            // Если квест существует.
            if (ServerQuests.Contains(Main.Players[player].Quest))
            {
                if (ServerQuestChapters.ContainsKey(Main.Players[player].QuestChapter))
                {
                    var lastChapterID = ServerLastChapter[Main.Players[player].Quest];

                    // Берем инфу о части квеста.
                    var chapter = ServerQuestChapters[Main.Players[player].QuestChapter];

                    if (!Chapter.ContainsKey(player)) Chapter.Add(player, new CurrentChapter());
                    Chapter[player].ID = chapter.ID;
                    Chapter[player].BizID = chapter.BizID;
                    Chapter[player].Colshape_name = chapter.Colshape_name;
                    Chapter[player].Colshape_interaction = chapter.Colshape_interaction;
                    Chapter[player].Name = chapter.Name;
                    Chapter[player].Description = chapter.Description;
                    Chapter[player].Progress = chapter.NeedProgress;
                    Chapter[player].Reward = chapter.Reward;
                    Chapter[player].RewardExp = chapter.RewardExp;
                    //TODO chapter step? for NPC

                    if (!QuestPanel.ContainsKey(player)) QuestPanel.Add(player, new QuestInfo());
                    QuestPanel[player].ID = chapter.ID;
                    QuestPanel[player].Name = chapter.Name;
                    QuestPanel[player].Description = chapter.Description;
                    QuestPanel[player].Need = chapter.NeedProgress;
                    QuestPanel[player].Progress = Main.Players[player].QuestProgress;

                    // При завершении части квеста, QuestChapter заносится в QuestEndChapter
                    if (Main.Players[player].QuestEndChapter == Main.Players[player].QuestChapter && Main.Players[player].QuestProgress == 0)
                    {
                        var NPC = QuestSystem.getNPCForReturnByName(Main.Players[player].QuestNPC);
                        var npcName = Main.Players[player].QuestNPC;
                        Vector3 npcCoords = null;

                        if (NPC != null)
                        {
                            npcName = NPC.Name;
                            npcCoords = NPC.Position;
                        }

                        QuestPanel[player].Text = $"{npcName} все еще ждет вас";

                        if (npcCoords != null) ClientBlipManager.CreateBlip(player, "QUEST", 0, new Vector3(npcCoords.X, npcCoords.Y, npcCoords.Z), "Квест", 11, false, true, true, 2, dimension: 0, true);
                    }

                    Main.Players[player].QuestUniqName = chapter.Uniq_name;
                    Main.Players[player].QuestInteraction = chapter.Colshape_interaction;

                    if (lastChapterID != 0) Main.Players[player].QuestLastChapter = lastChapterID;

                    Trigger.ClientEvent(player, "toggleQuestPanel", true);
                    UpdateQuestPanel(player, QuestPanel[player]);

                    player.SetMyData("PQ_NPC", Main.Players[player].QuestNPC);
                    player.SetMyData("PQ_STEP", Main.Players[player].QuestStep); // ?
                }
            }
        }

        public static void interactPressed(Player player, int id)
        {
            switch (id)
            {
                case 350:
                    {
                        try
                        {
                            OpenQuestMenu(player);
                        } catch (Exception e) { Log.Write("questInteractPressed: " + e.StackTrace, nLog.Type.Error); }
                    }
                    return;
                case 1060:
                    {
                        try
                        {
                            Trigger.ClientEvent(player, "screenFadeOut", 1000);

                            #region quest chapter iteration

                            QuestSystem.UpdatePlayerQuestIteration(player);

                            #endregion

                            //NAPI.Task.Run(() => {
                            //  NAPI.Entity.SetEntityDimension(player, 0);
                            //  NAPI.Entity.SetEntityPosition(player, QuestLS);
                            //}, 1000);

                            //NAPI.Task.Run(() => {
                            //  Trigger.ClientEvent(player, "screenFadeIn", 1000);
                            //}, 2000);

                            QuestSystem.cutScene_busEnter(player);
                        } catch (Exception e) { Log.Write("questInteractPressed: " + e.StackTrace, nLog.Type.Error); }
                    }
                    return;
            }

            Log.Write("QuestInteractPressed: " + id);
        }

        public static void SetCurrentQuest(Player player, int id)
        {
            if (player == null) return;
            if (!Main.Players.ContainsKey(player)) return;

            if (!ServerQuests.Contains(id)) return;

            Main.Players[player].Quest = id;
        }

        public static void SetCurrentQuestChapter(Player player, int id)
        {
            try
            {
                if (player == null) return;
                if (!Main.Players.ContainsKey(player)) return;

                if (!ServerQuestChapters.ContainsKey(id)) return;

                var lastChapterID = ServerLastChapter[Main.Players[player].Quest];

                // Берем инфу о части квеста.
                var chapter = ServerQuestChapters[id];

                if (!Chapter.ContainsKey(player)) Chapter.Add(player, new CurrentChapter());
                Chapter[player].ID = chapter.ID;
                Chapter[player].BizID = chapter.BizID;
                Chapter[player].Colshape_name = chapter.Colshape_name;
                Chapter[player].Colshape_interaction = chapter.Colshape_interaction;
                Chapter[player].Name = chapter.Name;
                Chapter[player].Description = chapter.Description;
                Chapter[player].Progress = chapter.NeedProgress;
                Chapter[player].Reward = chapter.Reward;
                Chapter[player].RewardExp = chapter.RewardExp;
                //TODO chapter step? for NPC

                if (!QuestPanel.ContainsKey(player)) QuestPanel.Add(player, new QuestInfo());
                QuestPanel[player].ID = chapter.ID;
                QuestPanel[player].Name = chapter.Name;
                QuestPanel[player].Description = chapter.Description;
                QuestPanel[player].Need = chapter.NeedProgress;
                QuestPanel[player].Progress = 0;
                QuestPanel[player].Text = "";

                ClientBlipManager.CreateBlip(player, "QUEST", 0, new Vector3(chapter.Waypoint.X, chapter.Waypoint.Y, chapter.Waypoint.Z), "Квест", 11, false, true, true, 2, dimension: 0, true);

                Trigger.ClientEvent(player, "toggleQuestPanel", true);
                Trigger.ClientEvent(player, "updateQuestPanel", JsonConvert.SerializeObject(QuestPanel[player]));

                Main.Players[player].QuestChapter = id;
                Main.Players[player].QuestUniqName = chapter.Uniq_name;
                Main.Players[player].QuestInteraction = chapter.Colshape_interaction;
                Main.Players[player].QuestLastChapter = lastChapterID;
                Main.Players[player].QuestChapterIsFinished = false;
                Main.Players[player].QuestProgress = 0;
                //Main.Players[player].QuestNPC = getNPCForReturn(id).Name; // ?

                //Автовыполнение если у игрока есть уже лицензия категории B
                if (chapter.Uniq_name == "autoschool" && Main.Players[player].Licenses[1] == true)
                {
                    #region quest chapter iteration

                    QuestSystem.UpdatePlayerQuestIteration(player, false);

                    #endregion
                }
            }
            catch (Exception e) { Log.Write("SetCurrentQuestChapter " + e.StackTrace, nLog.Type.Error); }
        }

        public static void UpdatePlayerQuest(Player player, int quest, int chapter)
        {
            try
            {
                //MySQL.Query($"UPDATE `characters` SET `quest` = {quest}, `quest_chapter` = {chapter}, `quest_progress` = 0 WHERE uuid={Main.Players[player].UUID}");

                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = "UPDATE `characters` SET " +
                  "`quest`=@quest," +
                  " `quest_chapter`=@quest_chapter," +
                  " `quest_progress`=@quest_progress" +
                  " WHERE `uuid`=@uuid";

                cmd.Parameters.AddWithValue("@quest", quest);
                cmd.Parameters.AddWithValue("@quest_chapter", chapter);
                cmd.Parameters.AddWithValue("@quest_progress", 0);
                cmd.Parameters.AddWithValue("@uuid", Main.Players[player].UUID);
                MySQL.Query(cmd);
            }
            catch (Exception e) { Log.Write("updatePlayerQuest " + e.StackTrace, nLog.Type.Error); }
        }

        public static void UpdatePlayerQuestIteration(Player player, bool check = true, bool forceFinish = false)
        {
            try
            {
                //QuestUniqName
                if(!Main.Players.ContainsKey(player)) return;
                if (Main.Players[player].QuestFinished == Main.Players[player].Quest) return;

                if(check == true)
                {
                    int playerIteraction = player.GetMyData<int>("QUEST_COLSHAPE_INTERACTION");
                    if (playerIteraction != Main.Players[player].QuestInteraction)
                    {
                        //Log.Debug($"[RETURN {player.Name}] -> UpdatePlayerQuestIteration:  playerIteraction: {playerIteraction} != {Main.Players[player].QuestInteraction} Main.Players[player].QuestInteraction", nLog.Type.Error);
                        return;
                    }
                }

                //MySQL.Query($"UPDATE `characters` SET `quest_progress` = `quest_progress`+1 WHERE uuid={Main.Players[player].UUID} AND `quest_chapter` = {Main.Players[player].QuestChapter}");

                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = "UPDATE `characters` SET " +
                  "`quest_progress`=`quest_progress`+1" +
                  " WHERE `uuid`=@uuid AND `quest_chapter`=@quest_chapter";

                cmd.Parameters.AddWithValue("@quest_chapter", Main.Players[player].QuestChapter);
                cmd.Parameters.AddWithValue("@quest_progress", 0);
                cmd.Parameters.AddWithValue("@uuid", Main.Players[player].UUID);
                MySQL.Query(cmd);

                Main.Players[player].QuestProgress += 1;
                QuestPanel[player].Progress = Main.Players[player].QuestProgress;

                if (Main.Players[player].QuestProgress >= Chapter[player].Progress && !Main.Players[player].QuestChapterIsFinished)
                {
                    FinishChapter(player, forceFinish);
                    Log.Write($"FINISH CHAPTER", nLog.Type.Warn);
                }
                else
                {
                    UpdateQuestPanel(player, QuestPanel[player]);
                    Log.Write($"UpdateQuestPanel ", nLog.Type.Warn);
                }

            }
            catch (Exception e) { Log.Write("UpdatePlayerQuestIteration " + e.StackTrace, nLog.Type.Error); }
        }

        public static void FinishChapter(Player player, bool forceFinish = false)
        {
            try
            {

                Main.Players[player].QuestEndChapter = Main.Players[player].QuestChapter;
                MoneySystem.Wallet.Change(player, Convert.ToInt32(Chapter[player].Reward));
                Utils.giveExp(player, Convert.ToInt32(Chapter[player].RewardExp));

                //MySQL.Query($"UPDATE `characters` SET `quest_progress` = 0, `quest_end_chapter` = {Main.Players[player].QuestEndChapter} WHERE uuid={Main.Players[player].UUID}");

                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = "UPDATE `characters` SET " +
                  "`quest_end_chapter`=@quest_end_chapter," +
                  " `quest_progress`=@quest_progress" +
                  " WHERE `uuid`=@uuid";

                cmd.Parameters.AddWithValue("@quest_end_chapter", Main.Players[player].QuestEndChapter);
                cmd.Parameters.AddWithValue("@quest_progress", 0);
                cmd.Parameters.AddWithValue("@uuid", Main.Players[player].UUID);
                MySQL.Query(cmd);

                int lastChapterID = getLastChapterInQuest(player);

                var NPC = getNPCForReturn(Main.Players[player].QuestChapter);
                string npcName = StartNPCName;
                Vector3 npcCoords = QuestStart;

                if (NPC != null)
                {
                    npcName = NPC.Name;
                    npcCoords = NPC.Position;
                }

                Log.Debug("NPC: "+npcName+" coords: "+npcCoords, nLog.Type.Warn);

                QuestPanel[player].Text = $"{npcName} ждет вас";
                UpdateQuestPanel(player, QuestPanel[player]);

                //Trigger.ClientEvent(player, "createWaypoint", npcCoords.X, npcCoords.Y);
                ClientBlipManager.CreateBlip(player, "QUEST", 0, new Vector3(npcCoords.X, npcCoords.Y, npcCoords.Z), "Квест", 11, false, true, true, 2, dimension: 0, true);

                if (lastChapterID == Main.Players[player].QuestChapter)
                {
                    Trigger.ClientEvent(player, "finishQuest", Main.Players[player].Quest);

                    //Trigger.ClientEvent(player, "toggleQuestPanel", false);
                    //QuestUniqName

                    //TODO FINISH QUEST INFROMATION
                    Main.Players[player].QuestUniqName = "";

                    client.Core.Achievements.AddAchievementScore(player, client.Core.AchievementID.Newbie, 1);
                }
                else
                {
                    Trigger.ClientEvent(player, "finishQuestChapter", Main.Players[player].QuestEndChapter);
                }

                if (player.HasMyData("PQ_STEP"))
                {
                    var step = player.GetMyData<int>("PQ_STEP") + 1;
                    Log.Debug("FinishChapter / Quest PQ_STEP + 1: " + step, nLog.Type.Warn);
                    ChangePlayerStep(player, step);
                    switch(step)
                    {
                        case 3:
                          ChangePlayerNPC(player, DeliveryNPCName);
                          break;
                        case 10:
                          ChangePlayerNPC(player, RaceNPCEndName);
                          break;
                        case 11:
                          ChangePlayerNPC(player, MeriaNPCName);
                          break;
                    }
                }

                Main.Players[player].QuestChapterIsFinished = true;

                //if (forceFinish)
                //{
                //    Trigger.ClientEvent(player, "toggleQuestPanel", false);
                //    changePlayerFinishQuest(player, Main.Players[player].Quest);
                //}

            }
            catch (Exception e) { Log.Write("FinishChapter " + e.StackTrace, nLog.Type.Error); }
        }

        public static void UpdateQuestPanel(Player player, QuestInfo info)
        {
            Log.Debug("UpdateQuestPanel info: " + JsonConvert.SerializeObject(info), nLog.Type.Warn);
            Trigger.ClientEvent(player, "updateQuestPanel", JsonConvert.SerializeObject(info));
        }

        #region Colshapes Enter / Exit

        public void onEntityEnterColshapeToLS(ColShape colshape, Player player)
        {
            if (player.HasMyData("PQ_STEP") && player.GetMyData<int>("PQ_STEP") != 0)
            {
                player.SetMyData("QUEST_COLSHAPE_INTERACTION", 1060);
                player.SetMyData("INTERACTIONCHECK", 1060);
            }
        }

        public void onEntityExitColshapeToLS(ColShape colshape, Player player)
        {
            player.SetMyData("INTERACTIONCHECK", 0);
        }

        public void onEntityEnterColshape(ColShape colshape, Player player)
        {
            if (!Main.Players.ContainsKey(player)) return;
            if (colshape.HasData("Q_NPC") && player.HasMyData("PQ_NPC")) //?
            {
                Log.Debug("Q_NPC: "+ colshape.GetData<string>("Q_NPC") + " PQ_NPC: "+ player.GetMyData<string>("PQ_NPC"), nLog.Type.Error);
                //Если у чувака другой квестовый NPC, то не даем ему говорить.
                if(colshape.GetData<string>("Q_NPC") != player.GetMyData<string>("PQ_NPC")) return;
            }
            else if(colshape.HasData("Q_NPC") && !player.HasMyData("PQ_NPC") && colshape.GetData<string>("Q_NPC") != "Тревор")
            {
                Log.Debug("Q_NPC: "+ colshape.GetData<string>("Q_NPC") + " PQ_NPC: "+ player.GetMyData<string>("PQ_NPC"), nLog.Type.Error);
                // Если у чувака нету в целом квестового NPC и он подошел не к начальному NPC.
                return;
            }

            Log.Debug("Q_NPC: "+ colshape.GetData<string>("Q_NPC") + " PQ_NPC: "+ player.GetMyData<string>("PQ_NPC"), nLog.Type.Error);

            if(colshape.HasData("INTERACT") && colshape.GetData<int>("INTERACT") != 350)
            {
                player.SetMyData("INTERACTIONCHECK", colshape.GetData<int>("INTERACT"));
            } else
            {
                player.SetMyData("INTERACTIONCHECK", 350);
            }


            if (Main.Colshapes.ContainsKey(colshape))
            {
                if (player.HasMyData("COLSHAPE_TYPE")) player.ResetMyData("COLSHAPE_TYPE");

                player.SetMyData<string>("COLSHAPE_TYPE", Main.Colshapes[colshape]);
            }
        }

        public void onEntityExitColshape(ColShape colshape, Player player)
        {
            player.SetMyData("INTERACTIONCHECK", 0);

            if (player.HasMyData("COLSHAPE_TYPE")) player.ResetMyData("COLSHAPE_TYPE");
        }

        #endregion

        #region Untils

        public static bool isActiveChapter(Player player, string uniq_name)
        {
            if (Main.Players.ContainsKey(player) && Main.Players[player].QuestUniqName != uniq_name) return false;

            return true;
        }

        public static int getLastChapterInQuest(Player player)
        {
            if(Main.Players[player].Quest != 0)
            {
                //DataTable result = MySQL.QueryRead($"SELECT `id__chapter` FROM `quest_relation` WHERE `id__quest` = {Main.Players[player].Quest} ORDER BY `id__chapter` DESC");

                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = "SELECT `id__chapter` FROM `quest_relation` WHERE `id__quest`=@id__quest ORDER BY `id__chapter` DESC";

                cmd.Parameters.AddWithValue("@id__quest", Main.Players[player].Quest);
                DataTable result = MySQL.QueryRead(cmd);

                DataRow row = result.Rows[0];

                return Convert.ToInt32(row["id__chapter"]);
            } else
            {
                return 0;
            }
        }

        public static ObjectQuestNPC getNPCForReturn(int chapterId)
        {
            //DataTable questNPCResult = MySQL.QueryRead($"SELECT `npc` FROM `quest_chapters` WHERE `id` = {chapterId}");

            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = "SELECT `npc` FROM `quest_chapters` WHERE `id`=@id";

            cmd.Parameters.AddWithValue("@id", chapterId);
            DataTable questNPCResult = MySQL.QueryRead(cmd);

            if (questNPCResult != null)
            {
                DataRow Row = questNPCResult.Rows[0];
                int npcId = Convert.ToInt32(Row["npc"]);
                Log.Write($"getNPCForReturn npcId: "+ npcId, nLog.Type.Warn);
                return QuestNPCObject.ContainsKey(npcId) ? QuestNPCObject[npcId] : null;
            }

            return null;
        }

        public static ObjectQuestNPC getNPCForReturnByName(string name)
        {
            var npc = QuestNPCObject.FirstOrDefault(n => n.Value.Name == name).Value;
            if (npc != null) return npc;

            return null;
        }

        public static void changePlayerFinishQuest(Player player, int quest)
        {
            try
            {
                Main.Players[player].QuestFinished = quest;

                //MySQL.Query($"UPDATE `characters` SET `quest_finished` = {quest} WHERE uuid={Main.Players[player].UUID}");

                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = "UPDATE `characters` SET `quest_finished`=@quest_finished WHERE `uuid`=@uuid";

                cmd.Parameters.AddWithValue("@quest_finished", quest);
                cmd.Parameters.AddWithValue("@uuid", Main.Players[player].UUID);
                MySQL.Query(cmd);
            }
            catch (Exception e) { Log.Write("ChangePlayerStep " + e.StackTrace, nLog.Type.Error); }
        }

        public static void ChangePlayerStep(Player player, int step)
        {
            try
            {
                Main.Players[player].QuestStep = step;
                player.SetMyData("PQ_STEP", step);
                //MySQL.Query($"UPDATE `characters` SET `quest_step_chapter` = {step} WHERE uuid={Main.Players[player].UUID}");

                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = "UPDATE `characters` SET `quest_step_chapter`=@quest_step_chapter WHERE `uuid`=@uuid";

                cmd.Parameters.AddWithValue("@quest_step_chapter", step);
                cmd.Parameters.AddWithValue("@uuid", Main.Players[player].UUID);
                MySQL.Query(cmd);
            }
            catch (Exception e) { Log.Write("ChangePlayerStep " + e.StackTrace, nLog.Type.Error); }
        }

        public static void ChangePlayerNPC(Player player, string npc)
        {
            try
            {
                if (npc == null)
                {
                  Main.Players[player].QuestNPC = null;
                  player.ResetMyData("PQ_NPC");
                  npc = "";
                }
                else
                {
                  Main.Players[player].QuestNPC = npc;
                  player.SetMyData("PQ_NPC", npc);
                }

                //MySQL.Query($"UPDATE `characters` SET `quest_npc` = '{npc}' WHERE uuid={Main.Players[player].UUID}");

                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = "UPDATE `characters` SET `quest_npc`=@quest_npc WHERE `uuid`=@uuid";

                cmd.Parameters.AddWithValue("@quest_npc", npc);
                cmd.Parameters.AddWithValue("@uuid", Main.Players[player].UUID);
                MySQL.Query(cmd);
            }
            catch (Exception e) { Log.Write("ChangePlayerStep " + e.StackTrace, nLog.Type.Error); }
        }

        public static void AcceptStartQuest(Player player, int quest, int chapter)
        {
            SetCurrentQuest(player, quest);
            SetCurrentQuestChapter(player, chapter);
            UpdatePlayerQuest(player, quest, chapter);
        }

        #endregion

        public static void OpenQuestMenu(Player player)
        {
            //Trigger.ClientEvent(player, "openQuestMenu");

            try
            {
                if (Main.Players[player].QuestFinished != 0) {
                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы уже проходили этот квест", 3000);
                    return;
                }


                if (!player.HasMyData("PQ_STEP") || (player.HasMyData("PQ_STEP") && player.GetMyData<int>("PQ_STEP") == 0))
                {
                    Trigger.PlayerEvent(player, "CamToNPC", StartNPC + new Vector3(0, 0, 1.2), -130.9147);
                    if(!player.HasMyData("PQ_NULLSTEP"))
                    {
                        Dialog.Send(player, "QUEST", StartNPCName, "Привет с прибытием! У меня есть для тебя неплохое предложение!", new List<Reply>()
                        {
                            new Reply($"Отлично! Расскажи по подробнее", "blue"),
                            new Reply($"Мне не нужна работа, спасибо!", "white"),
                        });
                        player.SetMyData("PQ_STEPNAME", "QUEST0");
                        //ChangePlayerNPC(player, StartNPCName);
                    } else
                    {
                        Dialog.Send(player, "QUEST", StartNPCName, "Привет! Ты Передумал? Мое предложение все еще актуально!", new List<Reply>()
                        {
                            new Reply($"Ладно, расскажи по подробнее", "blue"),
                            new Reply($"Нет, хотел просто еще раз поздороваться", "white"),
                        });
                        player.SetMyData("PQ_STEPNAME", "QUEST0RETURN");
                        //ChangePlayerNPC(player, StartNPCName);
                    }
                }
                else
                {
                    Log.Debug("openQuestMenu step: "+ player.GetMyData<int>("PQ_STEP"), nLog.Type.Error);
                    switch (player.GetMyData<int>("PQ_STEP"))
                    {
                        case 1:
                          Trigger.PlayerEvent(player, "CamToNPC", StartNPC + new Vector3(0, 0, 1.2), -130.9147);
                          Dialog.Send(player, "QUEST", StartNPCName, "Так как ты скорее всего недавно приехал в штат, тебе надо немного подзаработать, у меня есть знакомые которым нужны пару свободных рук.", new List<Reply>()
                          {
                              new Reply($"Деньги мне не помешают. Подскажешь где его найти?", "blue"),
                              new Reply("Пока меня это не интересует", "white"),
                          });
                          player.SetMyData("PQ_STEPNAME", "QUEST1");
                          //ChangePlayerNPC(player, StartNPCName);
                          break;
                        case 2:
                          Trigger.PlayerEvent(player, "CamToNPC", StartNPC + new Vector3(0, 0, 1.2), -130.9147);
                          Dialog.Send(player, "QUEST", StartNPCName, "Привет! Ты не собираешься в город? Мне надо передать посылку, поможешь?", new List<Reply>()
                          {
                              new Reply($"Как раз в ближайшие дни собирался", "blue"),
                              new Reply("Я не собираюсь в город, извини", "white"),
                          });
                          player.SetMyData("PQ_STEPNAME", "QUEST2");
                          //ChangePlayerNPC(player, DeliveryNPCName);
                          break;
                        case 3:
                          Trigger.PlayerEvent(player, "CamToNPC", DeliveryNPC + new Vector3(0, 0, 0.12), -200.79105);
                          Dialog.Send(player, "QUEST", DeliveryNPCName, "Хэллоу! Чем могу помочь?", new List<Reply>()
                          {
                              new Reply($"Тревор просил передать вам посылку", "blue"),
                              new Reply("Ничем", "white"),
                          });
                          player.SetMyData("PQ_STEPNAME", "QUEST3");
                          //ChangePlayerNPC(player, DeliveryNPCName);
                          break;
                        case 4:
                          Trigger.PlayerEvent(player, "CamToNPC", DeliveryNPC + new Vector3(0, 0, 0.12), -200.79105);
                          Dialog.Send(player, "QUEST", DeliveryNPCName, $"Да, {StartNPCName} звонил, спасибо за помощь! Слушай, у меня есть знакомые которые ищут быстрого водилу, но для этого тебе надо купить SIM-карту", new List<Reply>()
                          {
                              new Reply($"Хм, интересно, подскажешь где я могу ее купить?", "blue"),
                              new Reply("Мне неинтересно", "white"),
                          });
                          player.SetMyData("PQ_STEPNAME", "QUEST4");
                          //ChangePlayerNPC(player, DeliveryNPCName);
                          break;
                        case 5:
                          Trigger.PlayerEvent(player, "CamToNPC", DeliveryNPC + new Vector3(0, 0, 0.12), -200.79105);
                          Dialog.Send(player, "QUEST", DeliveryNPCName, "Хэллоу! Помнишь я говорил про поиск водителя? Тебе все еще интересно?", new List<Reply>()
                          {
                              new Reply($"Да, что мне еще потребуется?", "blue"),
                              new Reply("Нет", "white"),
                          });
                          player.SetMyData("PQ_STEPNAME", "QUEST5");
                          //ChangePlayerNPC(player, DeliveryNPCName);
                          break;
                        case 6:
                          Trigger.PlayerEvent(player, "CamToNPC", DeliveryNPC + new Vector3(0, 0, 0.12), -200.79105);
                          Dialog.Send(player, "QUEST", DeliveryNPCName, "Ну раз у тебя есть такое стремление, то тебе надо еще получить хотя бы права категории B", new List<Reply>()
                          {
                              new Reply($"Подскажешь где находится автошкола?", "blue"),
                              new Reply("Позже займусь этим", "white"),
                          });
                          player.SetMyData("PQ_STEPNAME", "QUEST6");
                          //ChangePlayerNPC(player, DeliveryNPCName);
                          break;
                        case 7:
                          Trigger.PlayerEvent(player, "CamToNPC", DeliveryNPC + new Vector3(0, 0, 0.12), -200.79105);
                          Dialog.Send(player, "QUEST", DeliveryNPCName, "Так, значит права у тебя уже есть?", new List<Reply>()
                          {
                              new Reply($"Да", "blue"),
                              new Reply("У меня нет на это времени", "white"),
                          });
                          player.SetMyData("PQ_STEPNAME", "QUEST7");
                          //ChangePlayerNPC(player, DeliveryNPCName);
                          break;
                        case 8:
                          Trigger.PlayerEvent(player, "CamToNPC", DeliveryNPC + new Vector3(0, 0, 0.12), -200.79105);
                          Dialog.Send(player, "QUEST", DeliveryNPCName, $"Я уже связался со знакомыми и записал тебя в список участников, отправляйся к {RaceNPCName}у и порви там всех!", new List<Reply>()
                          {
                              new Reply($"Хорошо, спасибо за рекомендацию!", "blue"),
                              new Reply("Я еще не готов к такому...", "white"),
                          });
                          player.SetMyData("PQ_STEPNAME", "QUEST8");
                          //ChangePlayerNPC(player, RaceNPCName);
                          break;
                        case 9:
                          Trigger.PlayerEvent(player, "CamToNPC", RaceNPC + new Vector3(0, 0, 0.12), -113.45668);
                          Dialog.Send(player, "QUEST", RaceNPCName, $"Привет, тебя порекомендовал {DeliveryNPCName}? Готов? Мы хотим узнать максимальный потенциал некоторых автомобилей в заезде", new List<Reply>()
                          {
                              new Reply($"Всегда готов! Покажи какие есть автомобили", "blue"),
                              new Reply("Я еще не готов, зайду позже", "white"),
                          });
                          player.SetMyData("PQ_STEPNAME", "QUEST9");
                          //ChangePlayerNPC(player, RaceNPCEndName);
                          break;
                        case 10:
                          Trigger.PlayerEvent(player, "CamToNPC", RaceNPCEnd + new Vector3(0, 0, 0.12), -25);
                          if(!player.HasMyData("PQ_WINRACE"))
                          {
                              Dialog.Send(player, "QUEST", RaceNPCEndName, "Это было неплохо! Спасибо за участие! Жаль, но кто-то был быстрее тебя, но не расстраивайся! Держи сертификат на прокат авто", new List<Reply>()
                              {
                                  new Reply($"Я показал максимум, спасибо за возможность поучаствовать", "blue")
                              });
                              player.SetMyData("PQ_STEPNAME", "QUEST10");
                              //ChangePlayerNPC(player, null);
                          } else
                          {
                              Dialog.Send(player, "QUEST", RaceNPCEndName, "Это было неплохо! Спасибо за участие! Ты стал одним из самых быстрых, держи сертификат на прокат авто", new List<Reply>()
                              {
                                  new Reply($"Оу, спасибо! Обязательно воспользуюсь!", "blue")
                              });
                              player.SetMyData("PQ_STEPNAME", "QUEST10");
                              //ChangePlayerNPC(player, null);
                          }

                          break;
                        case 11:
                          Trigger.PlayerEvent(player, "CamToNPC", MeriaNPC + new Vector3(0, 0, 0.12), -148.5389);

                          Dialog.Send(player, "QUEST", MeriaNPCName, $"Приветствую {player.Name}, {RaceNPCEndName} говорил тебе нужна подработка?", new List<Reply>()
                          {
                              new Reply($"Да", "blue"),
                              new Reply("Сейчас нету времени, извините", "white"),
                          });
                          player.SetMyData("PQ_STEPNAME", "QUEST11");
                          //ChangePlayerNPC(player, null);


                          break;
                        case 12:
                          Trigger.PlayerEvent(player, "CamToNPC", MeriaNPC + new Vector3(0, 0, 0.12), -148.5389);

                          Dialog.Send(player, "QUEST", MeriaNPCName, $"Моему другу не хватает рук, он курирует почтальонов. Да работа не та, о которой ты мечтал, но я буду благодарен если немного поможешь", new List<Reply>()
                          {
                              new Reply($"Хорошо, как мне устроится почтальоном?", "blue"),
                              new Reply("Извините, у меня есть идеи, где я смогу подзаработать, спасибо за предложение", "red"),
                          });
                          player.SetMyData("PQ_STEPNAME", "QUEST12");
                          //ChangePlayerNPC(player, null);


                          break;
                        case 13:
                          Trigger.PlayerEvent(player, "CamToNPC", MeriaNPC + new Vector3(0, 0, 0.12), -148.5389);

                          Dialog.Send(player, "QUEST", MeriaNPCName, $"Позади меня находится здание правительства, зайди туда и у стойки информации тебе подскажут", new List<Reply>()
                          {
                              new Reply($"Спасибо, до встречи!", "blue")
                          });
                          player.SetMyData("PQ_STEPNAME", "QUEST13");
                          //ChangePlayerNPC(player, null);


                          break;
                        case 14:
                          Trigger.PlayerEvent(player, "CamToNPC", MeriaNPC + new Vector3(0, 0, 0.12), -148.5389);

                          Dialog.Send(player, "QUEST", MeriaNPCName, $"Спасибо за помощь! Как видишь, многие готовы взять тебя на работу без особых навыков, так что найди, чем хочешь заниматься, желаю удачи тебе и до встречи!", new List<Reply>()
                          {
                              new Reply($"Спасибо. И тебе удачи!", "blue")
                          });
                          player.SetMyData("PQ_STEPNAME", "QUEST14");
                          //ChangePlayerNPC(player, null);


                          break;
                    }

                }
            }
            catch (Exception e) { Log.Write("openQuestMenu: " + e.StackTrace, nLog.Type.Error); }
        }

        public static void Dialog_callback(Player player, int answer)
        {
            if(!player.HasMyData("PQ_STEPNAME"))
            {
              Log.Debug("PQ_STEPNAME NULL!", nLog.Type.Error);
              return;
            }

            switch(player.GetMyData<string>("PQ_STEPNAME"))
            {
                case "QUEST0":
                case "QUEST0RETURN":
                  switch (answer)
                  {
                      case 0:
                        //Dialog.Close(player);

                        ChangePlayerStep(player, 1);
                        OpenQuestMenu(player);
                        break;
                      case 1:
                        Trigger.PlayerEvent(player, "DestroyCamToNPC");
                        Dialog.Close(player);
                        break;
                  }
                  break;
                case "QUEST1":
                  switch(answer)
                  {
                      case 0:
                        Trigger.PlayerEvent(player, "DestroyCamToNPC");
                        Dialog.Close(player);

                        AcceptStartQuest(player, 1, 1);
                        break;
                      case 1:
                        Trigger.PlayerEvent(player, "DestroyCamToNPC");
                        Dialog.Close(player);
                        break;
                  }
                  break;
                case "QUEST2":
                  switch(answer)
                  {
                      case 0:
                        Trigger.PlayerEvent(player, "DestroyCamToNPC");
                        Dialog.Close(player);

                        AcceptStartQuest(player, 1, 2);
                        Trigger.PlayerEvent(player, "2QUEST::CREATEMARKER");
                        break;
                      case 1:
                        Trigger.PlayerEvent(player, "DestroyCamToNPC");
                        Dialog.Close(player);
                        break;
                  }
                  break;
                case "QUEST3":
                  switch(answer)
                  {
                      case 0:
                        //Dialog.Close(player);

                        //Заново открываем продолжение диалога с другим текстом и кнопками
                        ChangePlayerStep(player, 4);
                        OpenQuestMenu(player);
                        Trigger.PlayerEvent(player, "2QUEST::DELETEMARKER");
                        break;
                      case 1:
                        Trigger.PlayerEvent(player, "DestroyCamToNPC");
                        Dialog.Close(player);
                        break;
                  }
                  break;
                case "QUEST4":
                  switch(answer)
                  {
                      case 0:
                        Trigger.PlayerEvent(player, "DestroyCamToNPC");
                        Dialog.Close(player);

                        AcceptStartQuest(player, 1, 3);
                        break;
                      case 1:
                        Trigger.PlayerEvent(player, "DestroyCamToNPC");
                        Dialog.Close(player);
                        break;
                  }
                  break;
                case "QUEST5":
                  switch(answer)
                  {
                      case 0:
                        //Dialog.Close(player);

                        //Заново открываем продолжение диалога с другим текстом и кнопками
                        //Если есть права, то пропускаем диалог, открываем следующий
                        if(Main.Players[player].Licenses[1] == true) {
                            Log.Debug($"Перепрыгиваем Квест с автошколой, тк у нас есть права! ({player.Name})", nLog.Type.Error);
                            AcceptStartQuest(player, 1, 4);
                            ChangePlayerStep(player, 7); //и без этого станет 7?
                            //player.SetMyData("PQ_STEPNAME", "QUEST7");
                        }
                        else ChangePlayerStep(player, 6);

                        OpenQuestMenu(player);
                        break;
                      case 1:
                        Trigger.PlayerEvent(player, "DestroyCamToNPC");
                        Dialog.Close(player);
                        break;
                  }
                  break;
                case "QUEST6":
                  switch(answer)
                  {
                      case 0:
                        Trigger.PlayerEvent(player, "DestroyCamToNPC");
                        Dialog.Close(player);

                        AcceptStartQuest(player, 1, 4);
                        break;
                      case 1:
                        Trigger.PlayerEvent(player, "DestroyCamToNPC");
                        Dialog.Close(player);
                        break;
                  }
                  break;
                case "QUEST7":
                  switch(answer)
                  {
                      case 0:
                        //Dialog.Close(player);

                        //Заново открываем продолжение диалога с другим текстом и кнопками
                        ChangePlayerStep(player, 8);
                        OpenQuestMenu(player);
                        break;
                      case 1:
                        Trigger.PlayerEvent(player, "DestroyCamToNPC");
                        Dialog.Close(player);
                        break;
                  }
                  break;
                case "QUEST8":
                  switch(answer)
                  {
                      case 0:
                        Trigger.PlayerEvent(player, "DestroyCamToNPC");
                        Dialog.Close(player);

                        ChangePlayerStep(player, 9);
                        ChangePlayerNPC(player, RaceNPCName);

                        QuestPanel[player].Text = $"{RaceNPCName} ждет вас";
                        UpdateQuestPanel(player, QuestPanel[player]);
                        ClientBlipManager.CreateBlip(player, "QUEST", 0, new Vector3(RaceNPC.X, RaceNPC.Y, RaceNPC.Z), "Квест", 11, false, true, true, 2, dimension: 0, true);
                        //Trigger.ClientEvent(player, "createWaypoint", RaceNPC.X, RaceNPC.Y);

                            break;
                      case 1:
                        Trigger.PlayerEvent(player, "DestroyCamToNPC");
                        Dialog.Close(player);
                        break;
                  }
                  break;
                case "QUEST9":
                  switch(answer)
                  {
                      case 0:
                        Trigger.PlayerEvent(player, "DestroyCamToNPC");
                        Dialog.Close(player);

                        AcceptStartQuest(player, 1, 5);

                        //Открываем меню гонки
                        NAPI.Task.Run(() => {
                          Race.openRaceMenu(player);
                        }, 1000);

                        break;
                      case 1:
                        Trigger.PlayerEvent(player, "DestroyCamToNPC");
                        Dialog.Close(player);
                        break;
                  }
                  break;
                case "QUEST10":
                  switch(answer)
                  {
                      case 0:
                        Trigger.PlayerEvent(player, "DestroyCamToNPC");
                        Dialog.Close(player);

                        Race.setWaypoinToPrizeColshape(player);
                        Trigger.ClientEvent(player, "screenFadeOut", 1000);

                        NAPI.Task.Run(() => {
                          NAPI.Entity.SetEntityDimension(player, 0);
                          NAPI.Entity.SetEntityPosition(player, Race.racePrizeColshape + new Vector3(0, 0, 1.2));
                        }, 1000);

                        //ChangePlayerStep(player, 11);
                        //QuestPanel[player].Text = $"Призовой автосалон ждет вас";
                        //UpdateQuestPanel(player, QuestPanel[player]);
                        AcceptStartQuest(player, 1, 6);

                        NAPI.Task.Run(() => {
                          Trigger.ClientEvent(player, "screenFadeIn", 1000);
                        }, 2000);

                        break;
                  }
                  break;
                case "QUEST11":
                  switch(answer)
                  {
                      case 0:
                        ChangePlayerStep(player, 12);
                        OpenQuestMenu(player);

                        break;
                      case 1:
                        Trigger.PlayerEvent(player, "DestroyCamToNPC");
                        Dialog.Close(player);
                        break;
                  }
                  break;
                case "QUEST12":
                  switch(answer)
                  {
                      case 0:
                        ChangePlayerStep(player, 13);
                        OpenQuestMenu(player);

                        break;
                      case 1:
                        Trigger.PlayerEvent(player, "DestroyCamToNPC");
                        Dialog.Close(player);
                        break;
                  }
                  break;
                case "QUEST13":
                  switch(answer)
                  {
                      case 0:
                        Trigger.PlayerEvent(player, "DestroyCamToNPC");
                        Dialog.Close(player);

                        AcceptStartQuest(player, 1, 7);

                        break;
                      case 1:
                        Trigger.PlayerEvent(player, "DestroyCamToNPC");
                        Dialog.Close(player);
                        break;
                  }
                  break;
                case "QUEST14":
                  switch(answer)
                  {
                      case 0:
                        Trigger.PlayerEvent(player, "DestroyCamToNPC");
                        Dialog.Close(player);
                        Trigger.ClientEvent(player, "toggleQuestPanel", false);
                        changePlayerFinishQuest(player, 1);
                        break;
                  }
                  break;

                //case "QUEST10NOTWIN":
                //  switch(answer)
                //  {
                //      case 0:
                //        Trigger.PlayerEvent(player, "DestroyCamToNPC");
                //        Dialog.Close(player);
                //        Trigger.ClientEvent(player, "toggleQuestPanel", false);
                //        changePlayerFinishQuest(player, 1);
                //        break;
                //  }
                //  break;
                //case "QUEST10WIN":
                //  switch(answer)
                //  {
                //      case 0:
                //        Trigger.PlayerEvent(player, "DestroyCamToNPC");
                //        Dialog.Close(player);

                //        Race.setWaypoinToPrizeColshape(player);
                //        Trigger.ClientEvent(player, "toggleQuestPanel", false);
                //        changePlayerFinishQuest(player, 1);
                //        break;
                //  }
                //  break;
            }
        }

        #region Commands
        [Command("resetquest", Hide = true)]
        public void resetQuest(Player player, int playerId, int quest)
        {
            if (!Group.CanUseCmd(player, "setquest")) return;

            Player target = Main.GetPlayerByID(playerId);
            if (target == null)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Ошибка игрок не найден.", 3000);
                return;
            }

            if (target.HasMyData("RACE_DESTROYED_CAR")) target.ResetMyData("RACE_DESTROYED_CAR");
            if (target.HasMyData("RACE_FAILED")) target.ResetMyData("RACE_FAILED");
            if (Main.Players[target].QuestFinished != 0) Main.Players[target].QuestFinished = 0;

            if (quest == 1)
            {
                ChangePlayerNPC(target, StartNPCName);
                ChangePlayerStep(target, 1);

                SetCurrentQuest(target, quest);
                SetCurrentQuestChapter(target, 1);
                UpdatePlayerQuest(target, quest, 1);

                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы успешно изменили игроку квест.", 3000);
                return;
            }

            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Ошибка", 3000);
            return;
        }

        [Command("setquest", Hide = true)]
        public void setQuestChapter(Player player, int playerId, int quest, int chapter)
        {
            if (!Group.CanUseCmd(player, "setquest")) return;

            Player target = Main.GetPlayerByID(playerId);
            if (target == null)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Ошибка игрок не найден.", 3000);
                return;
            }

            if (Main.Players[target].QuestLastChapter < chapter)
            {
                Log.Debug("[SetQuest] QuestLastChapter: "+ Main.Players[target].QuestLastChapter);
                Notify.Send(target, NotifyType.Error, NotifyPosition.BottomCenter, $"Ошибка номера части квеста.", 3000);
                return;
            }

            if (target.HasMyData("RACE_DESTROYED_CAR")) target.ResetMyData("RACE_DESTROYED_CAR");
            if (target.HasMyData("RACE_FAILED")) target.ResetMyData("RACE_FAILED");

            // Если будет 2 квеста и больше, то надо еще одну перменную которая хранит последний выполненный квест.
            // в QuestFinished хранится ID квеста
            if (Main.Players[target].QuestFinished != 0) Main.Players[target].QuestFinished = 0;


            if (quest == 1)
            {
                switch(chapter)
                {
                    case 1:
                      ChangePlayerNPC(target, StartNPCName);
                      ChangePlayerStep(target, 1);
                      break;
                    case 2:
                      ChangePlayerNPC(target, StartNPCName);
                      ChangePlayerStep(target, 2);
                      break;
                    case 3:
                      ChangePlayerNPC(target, DeliveryNPCName);
                      ChangePlayerStep(target, 3);
                      break;
                    case 4:
                      ChangePlayerNPC(target, DeliveryNPCName);
                      ChangePlayerStep(target, 4);
                      break;
                    case 5:
                      ChangePlayerNPC(target, DeliveryNPCName);
                      ChangePlayerStep(target, 8);
                      break;
                    case 6:
                      ChangePlayerNPC(target, AutoSalonName);
                      ChangePlayerStep(target, 11);
                      break;
                    case 7:
                      ChangePlayerNPC(target, MeriaNPCName);
                      ChangePlayerStep(target, 13);
                      break;
                }
            }

            SetCurrentQuest(target, quest);
            SetCurrentQuestChapter(target, chapter);
            UpdatePlayerQuest(target, quest, chapter);

            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы успешно изменили игроку квест.", 3000);
            return;
        }

        [Command("addnpcchapter", Hide = true)]
        public void AddNpcChapter(Player player, int npcId, int chapterId)
        {
            try
            {
              if (!Group.CanUseCmd(player, "addnpcchapter")) return;
              if (Main.Players.ContainsKey(player))
              {
                //MySQL.Query($"UPDATE `quest_chapters` SET `npc`={npcId} WHERE `id` = {chapterId}");

                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = "UPDATE `quest_chapters` SET `npc`=@npc WHERE `id`=@id";

                cmd.Parameters.AddWithValue("@npc", npcId);
                cmd.Parameters.AddWithValue("@id", chapterId);
                MySQL.Query(cmd);

                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"NPC успешно привязан к части квеста.", 3000);
              }
            }
            catch (Exception e) { Log.Write("addnpcchapter: " + e.StackTrace, nLog.Type.Error); }
        }

        [Command("createquestnpc", Hide = true)]
        public void CreateQuestNPC(Player player, string name, int interaction)
        {
            try
            {
              if (!Group.CanUseCmd(player, "createquestnpc")) return;
              if (Main.Players.ContainsKey(player))
              {
                //MySQL.Query($"INSERT INTO `quest_npc` SET `name`='{name}', `interaction` = '{interaction}', `position`='{JsonConvert.SerializeObject(player.Position)}'");

                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = "INSERT INTO `quest_npc` SET `name`=@name, `interaction`=@interaction, `position`=@position WHERE `id`=@id";

                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@interaction", interaction);
                cmd.Parameters.AddWithValue("@position", JsonConvert.SerializeObject(player.Position));
                MySQL.Query(cmd);

                DataTable lastID = MySQL.QueryRead("SELECT MAX(id) FROM `quest_npc`");
                int lastId = Convert.ToInt32(lastID.Rows[0]["MAX(id)"]);
                QuestNPCObject.Add(lastId, new ObjectQuestNPC(lastId, name, interaction, player.Position));
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"NPC успешно создан.", 3000);
              }
            }
            catch (Exception e) { Log.Write("createquestnpc: " + e.StackTrace, nLog.Type.Error); }
        }

        [Command("createquestchapter", "Используйте: /createquestchapter [Название части квеста] [Уникальное название части] [Описание части квеста] [Кол-во итераций для выполнения] [Денег за выполнение] [EXP за выполнение]", Hide = true)]
        public void CreateQuestChapter(Player player, string name, string uniq_name, string description, int progress, int reward, int exp)
        {
            if (!Group.CanUseCmd(player, "createquestchapter")) return;

            if (Main.Players.ContainsKey(player))
            {
                if (!Group.CanUseCmd(player, "createquestchapter")) return;

                // /createquestchapter test test testdesc 1 1000

                try
                {

                    if (!player.HasMyData("COLSHAPE_TYPE"))
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы должны находиться около бизнеса или маркера работы", 3000);
                        return;
                    }

                    var colshape_name = player.GetMyData<string>("COLSHAPE_TYPE");
                    int colshape_interaction = player.GetMyData<int>("QUEST_COLSHAPE_INTERACTION");

                    if (name.Length < 50)
                    {
                        MySqlCommand cmd = new MySqlCommand();
                        var sql = $"`creator`=@creator," +
                                  $"`name`=@name," +
                                  $" `uniq_name`=@uniq_name," +
                                  $" `colshape_interaction`=@colshape_interaction," +
                                  $" `description`=@description," +
                                  $" `progress`=@progress," +
                                  $" `enterpoint`=@enterpoint," +
                                  $" `reward`=@reward," +
                                  $" `reward_exp`=@reward_exp," +
                                  $" `vehicles`=@vehicles";

                        cmd.Parameters.AddWithValue("@creator", player.Name);
                        cmd.Parameters.AddWithValue("@name", name);
                        cmd.Parameters.AddWithValue("@uniq_name", uniq_name);
                        cmd.Parameters.AddWithValue("@colshape_interaction", colshape_interaction);
                        cmd.Parameters.AddWithValue("@description", description);
                        cmd.Parameters.AddWithValue("@progress", progress);
                        cmd.Parameters.AddWithValue("@enterpoint", JsonConvert.SerializeObject(player.Position));
                        cmd.Parameters.AddWithValue("@reward", reward);
                        cmd.Parameters.AddWithValue("@reward_exp", exp);
                        cmd.Parameters.AddWithValue("@vehicles", JsonConvert.SerializeObject(new List<Vehicle>()));

                        if (player.HasMyData("BIZ_ID") && player.GetMyData<int>("BIZ_ID") != -1)
                        {
                            int bizID = player.GetMyData<int>("BIZ_ID");

                            Business biz = BusinessManager.BizList[bizID];
                            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Тип бизнеса: {biz.Type}", 3000);

                            sql += $", `bizID`=@bizID";
                            cmd.Parameters.AddWithValue("@bizID", bizID);
                        }

                        if (player.HasMyData("COLSHAPE_TYPE"))
                        {

                            sql += $", `colshape_name`=@colshape_name";
                            cmd.Parameters.AddWithValue("@colshape_name", colshape_name);
                        }

                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"SQL: {sql}", 3000);

                        //MySQL.Query($"INSERT INTO `quest_chapters` SET {sql}");

                        cmd.CommandText = $"INSERT INTO `quest_chapters` SET {sql}";

                        MySQL.Query(cmd);

                        //AddChapterLog();
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Квестовая часть успешно создана. Не забудьте ее привязать к квесту!", 3000);
                    }
                    else Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Слишком длинное название, придумайте покороче.", 3000);
                }
                catch (Exception e) { Log.Write("CreateQuestChapter: " + e.StackTrace, nLog.Type.Error); }

            }
        }

        [Command("createquest", "Используйте: /createquest [Название части квеста] [Описание части квеста] [Денег за выполнение] [EXP за выполнение]", Hide = true)]
        public void CreateQuest(Player player, string name, string description, int reward, int exp)
        {
            if (!Group.CanUseCmd(player, "createquest")) return;

            if (Main.Players.ContainsKey(player))
            {
                if (!Group.CanUseCmd(player, "createquest")) return;

                if (name.Length < 50)
                {
                    try
                    {
                        #region Create Quest
                        //var sql = $"`name` = '{name}', `description` = '{description}', `reward` = '{reward}', `reward_exp` = '{exp}'";

                        //MySQL.Query($"INSERT INTO `quests` SET {sql}");

                        MySqlCommand cmd = new MySqlCommand();
                        cmd.CommandText = "INSERT INTO `quests` SET `name`=@name, `description`=@description, `reward`=@reward, `reward_exp`=@reward_exp";

                        cmd.Parameters.AddWithValue("@name", name);
                        cmd.Parameters.AddWithValue("@description", description);
                        cmd.Parameters.AddWithValue("@reward", reward);
                        cmd.Parameters.AddWithValue("@reward_exp", exp);
                        MySQL.Query(cmd);

                        //DataTable lastID = MySQL.QueryRead("SELECT MAX(id) FROM `quests`");
                        //#endregion

                        //#region Create Relation
                        //if (lastID != null)
                        //{
                        //    foreach (int id_chapter in ids)
                        //    {
                        //        var sqlRelation = $"`id__quest` = '{lastID.Rows[0]["MAX(id)"]}', `id__chapter` = '{id_chapter}'";

                        //        MySQL.Query($"INSERT INTO `quest_relation` SET {sqlRelation}");

                        //        NAPI.Util.ConsoleOutput(sqlRelation);
                        //    }
                        //}

                        //NAPI.Util.ConsoleOutput($"LAST INSERT ID: {lastID}");
                        #endregion

                        //AddChapterLog();
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Квест успешно создан.", 3000);

                    }
                    catch (Exception e) { Log.Write("CreateQuestChapter: " + e.StackTrace, nLog.Type.Error); }
                }
                else Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Слишком длинное название, придумайте покороче.", 3000);

            }
        }

        [Command("addquestchapter", "Используйте: /addquestchapter [ID Квеста] [ID части квеста]", Hide = true)]
        public void AddQuestChapter(Player player, uint idQuest, uint idChapter)
        {
            if (!Group.CanUseCmd(player, "addquestchapter")) return;

            if (Main.Players.ContainsKey(player))
            {
                try
                {
                    //var sql = $"`id__quest` = '{idQuest}', `id__chapter` = '{idChapter}'";

                    //MySQL.Query($"INSERT INTO `quest_relation` SET {sql}");

                    MySqlCommand cmd = new MySqlCommand();
                    cmd.CommandText = "INSERT INTO `quest_relation` SET `id__quest`=@id__quest, `id__chapter`=@id__chapter";

                    cmd.Parameters.AddWithValue("@id__quest", idQuest);
                    cmd.Parameters.AddWithValue("@id__chapter", idChapter);
                    MySQL.Query(cmd);

                    //AddChapterLog();
                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Квест успешно связан с его частью!", 3000);

                }
                catch (Exception e) { Log.Write("CreateQuestChapter: " + e.StackTrace, nLog.Type.Error); }
            }
        }

    #endregion

        #region CUTSCENE

        public static async void cutScene_busEnter(Player player)
        {
            try
            {
                Vector3 spawnPlayer = new Vector3(158.99527, 6555.081, 31.981355);
                uint dimensionPlayer = Dimensions.RequestPrivateDimension(player, true);

                var busName = "coach";
                var busHash = -2072933068;
                VehicleHash getHashKey = (VehicleHash)NAPI.Util.GetHashKey(busName);
                var busHashUint = 2222034228;
                var busSpawnPos = new Vector3(143.19595, 6570.6895, 31.944184);
                var busSpawnRot = new Vector3(0, 0, -134.70195);

                Log.Debug($"[CUTSCENE][BUSENTER][{player.Name}] -> busName: {busName} busHash: {busHash} busHashDefault: {VehicleHash.Coach} GetHashKey: {getHashKey} default = uint: {getHashKey == VehicleHash.Coach}");

                Vehicle busEntity = null;

                busEntity = await NAPI.Task.RunReturn(() => {
                    try
                    {
                        Log.Debug("Dimension: " + dimensionPlayer, nLog.Type.Error);
                        NAPI.Entity.SetEntityDimension(player, dimensionPlayer);
                        NAPI.Entity.SetEntityPosition(player, spawnPlayer);

                        // Создаем автобус https://wiki.rage.mp/images/thumb/9/9d/Coach.png/164px-Coach.png
                        var entity = NAPI.Vehicle.CreateVehicle(VehicleHash.Coach, busSpawnPos, busSpawnRot.Z, 30, 30);
                        entity.NumberPlate = "BUS";
                        entity.Locked = false;
                        entity.EngineStatus = true;
                        entity.Dimension = dimensionPlayer;

                        return entity;
                    }
                    catch(Exception ex) { Log.Debug(ex.StackTrace); return null; }
                });

                if (busEntity == null)
                {
                    busEntity = await NAPI.Task.RunReturn(() => {
                        try
                        {
                            Log.Debug("ReCreate busEntity Dimension: " + dimensionPlayer, nLog.Type.Error);

                            // Создаем автобус https://wiki.rage.mp/images/thumb/9/9d/Coach.png/164px-Coach.png
                            var entity = NAPI.Vehicle.CreateVehicle(busHashUint, busSpawnPos, busSpawnRot.Z, 30, 30);
                            entity.NumberPlate = "BUS";
                            entity.Locked = false;
                            entity.EngineStatus = true;
                            entity.Dimension = dimensionPlayer;

                            return entity;
                        }
                        catch(Exception ex) { Log.Debug(ex.StackTrace); return null; }
                    });
                }

                player.SetMyData("CUTSCENE_BUS", busEntity);

                //Log.Debug("busEntity: "+ busEntity);

                //var busPed = NAPI.Ped.CreatePed(PedHash.SalvaGoon02GMY, spawnPlayer, 0, dimensionPlayer);
                //busPed.

                Trigger.ClientEvent(player, "CLIENT::cutscene:busStart", busEntity);
            }
            catch (Exception e) { Log.Write("CLIENT::cutscene:busStart: " + e.StackTrace, nLog.Type.Error); }
        }

        [RemoteEvent("SERVER::cutscene:busEnd")]
        public static async void cutScene_busExit(Player player)
        {
            try
            {
                uint dimensionPlayer = Dimensions.RequestPrivateDimension(player, true);
                var spawnPlayer = new Vector3(308.8741, -740.9353, 29.30414);

                var busName = "coach";
                var busHash = -2072933068;
                VehicleHash getHashKey = (VehicleHash)NAPI.Util.GetHashKey(busName);
                var busHashUint = 2222034228;
                //var busSpawnPos = new Vector3(321.4018, -727.21075, 29.28672);
                var busSpawnPos = new Vector3(316.30627, -738.3746, 29.225984);
                var busSpawnRot = new Vector3(0, 0, 160.5432);

                Vehicle busEntity = null;

                busEntity = await NAPI.Task.RunReturn(() => {
                    try
                    {
                        Log.Debug("Dimension: " + dimensionPlayer, nLog.Type.Error);
                        NAPI.Entity.SetEntityDimension(player, dimensionPlayer);
                        NAPI.Entity.SetEntityPosition(player, spawnPlayer);

                        Log.Debug($"[CUTSCENE][BUSEXIT][{player.Name}] -> busName: {busName} busHash: {busHash} busHashDefault: {VehicleHash.Coach} GetHashKey: {getHashKey} default = uint: {getHashKey == VehicleHash.Coach}");

                        // Создаем автобус https://wiki.rage.mp/images/thumb/9/9d/Coach.png/164px-Coach.png
                        var entity = NAPI.Vehicle.CreateVehicle(VehicleHash.Coach, busSpawnPos, busSpawnRot.Z, 30, 30);
                        entity.NumberPlate = "BUS";
                        entity.Locked = false;
                        entity.EngineStatus = true;
                        entity.Dimension = dimensionPlayer;

                        return entity;
                    }
                    catch (Exception ex) { Log.Debug(ex.StackTrace); return null; }
                });

                if (busEntity == null)
                {
                    busEntity = await NAPI.Task.RunReturn(() => {
                        try
                        {
                            Log.Debug("ReCreate busEntity Dimension: " + dimensionPlayer, nLog.Type.Error);

                            // Создаем автобус https://wiki.rage.mp/images/thumb/9/9d/Coach.png/164px-Coach.png
                            var entity = NAPI.Vehicle.CreateVehicle(busHashUint, busSpawnPos, busSpawnRot.Z, 30, 30);
                            entity.NumberPlate = "BUS";
                            entity.Locked = false;
                            entity.EngineStatus = true;
                            entity.Dimension = dimensionPlayer;

                            return entity;
                        }
                        catch(Exception ex) { Log.Debug(ex.StackTrace); return null; }
                    });
                }

                player.SetMyData("CUTSCENE_BUS", busEntity);

                Trigger.ClientEvent(player, "CLIENT::cutscene:busEnd", busEntity);
            }
            catch (Exception e) { Log.Write("CLIENT::cutscene:busStart: " + e.StackTrace, nLog.Type.Error); }
        }

        [RemoteEvent("SERVER::cutscene:deleteBus")]
        public static void cutScene_deleteBus(Player player)
        {
            try
            {
                if (player == null || !Main.Players.ContainsKey(player)) return;
                if (!player.HasMyData("CUTSCENE_BUS")) return;

                Vehicle BUS = player.GetMyData<Vehicle>("CUTSCENE_BUS");

                NAPI.Task.Run(() => {
                  NAPI.Entity.DeleteEntity(BUS);
                });

                NAPI.Task.Run(() => {
                  Log.Debug("Exits BUS?: " + NAPI.Entity.DoesEntityExist(BUS));
                  if (!NAPI.Entity.DoesEntityExist(BUS)) player.ResetMyData("CUTSCENE_BUS");
                }, 3000);

            } catch(Exception e) { Log.Write("SERVER::cutscene:deleteBus: " + e.StackTrace, nLog.Type.Error); }
        }

        [RemoteEvent("SERVER::cutscene:returnPlayer")]
        public static void cutScene_returnPlayer(Player player)
        {
            try
            {
                if (player == null || !Main.Players.ContainsKey(player)) return;

                var spawnPlayerPos = new Vector3(300.29175, -777.128, 29.30891);
                var spawnPlayerRot = new Vector3(0, 0, 117.336426);

                NAPI.Task.Run(() => {
                  NAPI.Entity.SetEntityDimension(player, 0);
                  NAPI.Entity.SetEntityPosition(player, spawnPlayerPos);
                  NAPI.Entity.SetEntityRotation(player, spawnPlayerRot);
                });

                #region quest chapter iteration

                QuestSystem.UpdatePlayerQuestIteration(player);

                #endregion

                Dimensions.DismissPrivateDimension(player);

            } catch(Exception e) { Log.Write("SERVER::cutscene:returnPlayer: " + e.StackTrace, nLog.Type.Error); }
        }

        #endregion
  }

    #region Обьекты
    //public class CurrentQuest
    //{
    //    public int ID { get; set; }
    //    public string Name { get; set; }
    //    public string Description { get; set; }
    //    public int Reward { get; set; }
    //}

    public class CurrentChapter
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int BizID { get; set; }
        public int Type { get; set; }
        public string Colshape_name { get; set; }
        public int Colshape_interaction { get; set; }
        public string Description { get; set; }
        public int Reward { get; set; }
        public int RewardExp { get; set; }
        public int Progress { get; set; }
        public byte ChapterState { get; set; } = 0;
    }

    public class ChapterInfo
    {
        public ChapterInfo(int id, string name, int bizID, string uniqName, string colshapeName, int colshapeInteraction, string desc, int reward, int rewardExp, int needProgress, Vector3 waypoint)
        {
            ID = id;
            Name = name;
            BizID = bizID;
            Uniq_name = uniqName;
            Colshape_name = colshapeName;
            Colshape_interaction = colshapeInteraction;
            Description = desc;
            Reward = reward;
            RewardExp = rewardExp;
            NeedProgress = needProgress;
            Waypoint = waypoint;
        }

        public int ID { get; set; }
        public string Name { get; set; }
        public int BizID { get; set; }
        public string Uniq_name { get; set; }
        public string Colshape_name { get; set; }
        public int Colshape_interaction { get; set; }
        public string Description { get; set; }
        public int Reward { get; set; }
        public int RewardExp { get; set; }
        public int NeedProgress { get; set; }
        public Vector3 Waypoint { get; set; }
    }

    public class QuestInfo
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Progress { get; set; }
        public int Need { get; set; }
        public string Text { get; set; }
    }

    public class ObjectQuestNPC {
        public ObjectQuestNPC(int id, string name, int interaction, Vector3 position )
        {
            Id = Id;
            Name = name;
            Interaction = interaction;
            Position = position;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int Interaction { get; set; }
        public Vector3 Position { get; set; }
    }

    #endregion
}
