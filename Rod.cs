using GTANetworkAPI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using NeptuneEvo.GUI;
using NeptuneEvo.MoneySystem;
using NeptuneEvo.SDK;
using System.Threading;
using NeptuneEvo.Jobs;
using MySqlConnector;
using client.Systems.BattlePass;

namespace NeptuneEvo.Core
{
    class RodManager : Script
    {

        // Как сделать чтобы вещь выпадала с большим шансом?
        // Добавляем намного больше строчек одного и тоже предмета
        // напрмер предмет Щука, добавим 10 раз[цифры менять надо], значит
        // в 10 раз будет чаще выпадать

        [ServerEvent(Event.PlayerDeath)]
        public void OnPlayerDeath(Player player, Player killer, uint reason)
        {
            BasicSync.DetachObject(player);
        }

        public static Dictionary<int, Roding> RodingsArea = new Dictionary<int, Roding>();

        public static List<List<Tuple<int, ItemType>>> FishLevel = new List<List<Tuple<int, ItemType>>>()
        {
            new List<Tuple<int, ItemType>>(){
                new Tuple<int, ItemType>(33, ItemType.Keta),
                new Tuple<int, ItemType>(66, ItemType.Gorbysha),
                new Tuple<int, ItemType>(99, ItemType.Kyndja),
            },
            new List<Tuple<int, ItemType>>(){
                new Tuple<int, ItemType>(26, ItemType.Keta),
                new Tuple<int, ItemType>(52, ItemType.Gorbysha),
                new Tuple<int, ItemType>(78, ItemType.Kyndja),
                new Tuple<int, ItemType>(93, ItemType.Sig),
                new Tuple<int, ItemType>(98, ItemType.Omyl),
            },
            new List<Tuple<int, ItemType>>(){
                new Tuple<int, ItemType>(25, ItemType.Keta),
                new Tuple<int, ItemType>(55, ItemType.Gorbysha),
                new Tuple<int, ItemType>(80, ItemType.Kyndja),
                new Tuple<int, ItemType>(87, ItemType.Sig),
                new Tuple<int, ItemType>(92, ItemType.Omyl),
                new Tuple<int, ItemType>(97, ItemType.Nerka),
                new Tuple<int, ItemType>(100, ItemType.Forel),
            },
            new List<Tuple<int, ItemType>>(){
                new Tuple<int, ItemType>(20, ItemType.Keta),
                new Tuple<int, ItemType>(50, ItemType.Gorbysha),
                new Tuple<int, ItemType>(80, ItemType.Kyndja),
                new Tuple<int, ItemType>(85, ItemType.Sig),
                new Tuple<int, ItemType>(90, ItemType.Omyl),
                new Tuple<int, ItemType>(93, ItemType.Nerka),
                new Tuple<int, ItemType>(96, ItemType.Forel),
                new Tuple<int, ItemType>(98, ItemType.Ship),
                new Tuple<int, ItemType>(100, ItemType.Lopatonos),
            },
            new List<Tuple<int, ItemType>>(){
                new Tuple<int, ItemType>(15, ItemType.Keta),
                new Tuple<int, ItemType>(40, ItemType.Gorbysha),
                new Tuple<int, ItemType>(65, ItemType.Kyndja),
                new Tuple<int, ItemType>(80, ItemType.Sig),
                new Tuple<int, ItemType>(88, ItemType.Omyl),
                new Tuple<int, ItemType>(91, ItemType.Nerka),
                new Tuple<int, ItemType>(94, ItemType.Forel),
                new Tuple<int, ItemType>(96, ItemType.Ship),
                new Tuple<int, ItemType>(98, ItemType.Lopatonos),
                new Tuple<int, ItemType>(99, ItemType.Osetr),
                new Tuple<int, ItemType>(100, ItemType.Semga),
            },
            new List<Tuple<int, ItemType>>(){
                new Tuple<int, ItemType>(10, ItemType.Keta),
                new Tuple<int, ItemType>(35, ItemType.Gorbysha),
                new Tuple<int, ItemType>(60, ItemType.Kyndja),
                new Tuple<int, ItemType>(80, ItemType.Sig),
                new Tuple<int, ItemType>(86, ItemType.Omyl),
                new Tuple<int, ItemType>(89, ItemType.Nerka),
                new Tuple<int, ItemType>(92, ItemType.Forel),
                new Tuple<int, ItemType>(94, ItemType.Ship),
                new Tuple<int, ItemType>(96, ItemType.Lopatonos),
                new Tuple<int, ItemType>(97, ItemType.Osetr),
                new Tuple<int, ItemType>(98, ItemType.Semga),
                new Tuple<int, ItemType>(99, ItemType.Servyga),
                new Tuple<int, ItemType>(100, ItemType.Beluga),
            },
            new List<Tuple<int, ItemType>>(){
                new Tuple<int, ItemType>(20, ItemType.Gorbysha),
                new Tuple<int, ItemType>(40, ItemType.Kyndja),
                new Tuple<int, ItemType>(70, ItemType.Sig),
                new Tuple<int, ItemType>(78, ItemType.Omyl),
                new Tuple<int, ItemType>(82, ItemType.Nerka),
                new Tuple<int, ItemType>(86, ItemType.Forel),
                new Tuple<int, ItemType>(89, ItemType.Ship),
                new Tuple<int, ItemType>(92, ItemType.Lopatonos),
                new Tuple<int, ItemType>(94, ItemType.Osetr),
                new Tuple<int, ItemType>(96, ItemType.Semga),
                new Tuple<int, ItemType>(98, ItemType.Servyga),
                new Tuple<int, ItemType>(100, ItemType.Beluga),
            },
            new List<Tuple<int, ItemType>>(){
                new Tuple<int, ItemType>(80, ItemType.Sig),
                new Tuple<int, ItemType>(160, ItemType.Omyl),
                new Tuple<int, ItemType>(171, ItemType.Nerka),
                new Tuple<int, ItemType>(176, ItemType.Forel),
                new Tuple<int, ItemType>(181, ItemType.Ship),
                new Tuple<int, ItemType>(185, ItemType.Lopatonos),
                new Tuple<int, ItemType>(188, ItemType.Osetr),
                new Tuple<int, ItemType>(191, ItemType.Semga),
                new Tuple<int, ItemType>(193, ItemType.Servyga),
                new Tuple<int, ItemType>(195, ItemType.Beluga),
                new Tuple<int, ItemType>(197, ItemType.Taimen),
                new Tuple<int, ItemType>(198, ItemType.Sterlyad),
                new Tuple<int, ItemType>(199, ItemType.Ydilshik),
                new Tuple<int, ItemType>(200, ItemType.Hailiod),
            },
            new List<Tuple<int, ItemType>>(){
                new Tuple<int, ItemType>(80, ItemType.Omyl),
                new Tuple<int, ItemType>(160, ItemType.Nerka),
                new Tuple<int, ItemType>(166, ItemType.Forel),
                new Tuple<int, ItemType>(171, ItemType.Ship),
                new Tuple<int, ItemType>(175, ItemType.Lopatonos),
                new Tuple<int, ItemType>(180, ItemType.Osetr),
                new Tuple<int, ItemType>(184, ItemType.Semga),
                new Tuple<int, ItemType>(187, ItemType.Servyga),
                new Tuple<int, ItemType>(190, ItemType.Beluga),
                new Tuple<int, ItemType>(193, ItemType.Taimen),
                new Tuple<int, ItemType>(195, ItemType.Sterlyad),
                new Tuple<int, ItemType>(197, ItemType.Ydilshik),
                new Tuple<int, ItemType>(199, ItemType.Hailiod),
            },
        };

        public static Dictionary<int, ItemType> TypeRod = new Dictionary<int, ItemType>
        {
            {1, ItemType.Rod },
            {2, ItemType.RodUpgrade },
            {3, ItemType.RodMK2 },
        };

        public static List<int> RodLevel = new List<int>()
        {
            10,     // 2
            60,     // 3
            160,    // 4
            410,    // 5
            910,    // 6
            2410,   // 7
            5410,   // 8
            11410,  // 9
        };

        public static List<int> PirsRodType = new List<int>()
        {
            1,
            1,
            2,
            2,
            2,
            3,
            3,
            3,
        };
        public static List<Dictionary<int, RodingModalInfo>> RodModalData = new List<Dictionary<int, RodingModalInfo>>()
        {
            new Dictionary<int, RodingModalInfo>() // 1 pirs
            {
                {1, new RodingModalInfo(RodingModalType.Standart, 0.2f, 0.25f, 0.1f, 10, 20) },
                {2, new RodingModalInfo(RodingModalType.Standart, 0.15f, 0.25f, 0.1f, 10, 20) },
                {3, new RodingModalInfo(RodingModalType.Easy, 0.1f, 0.3f, 0.1f, 10, 20) },
                {4, new RodingModalInfo(RodingModalType.Easy, 0.1f, 0.3f, 0.1f, 10, 20) },
                {5, new RodingModalInfo(RodingModalType.VeryEasy, 0.1f, 0.35f, 0.1f, 8, 12) },
                {6, new RodingModalInfo(RodingModalType.VeryEasy, 0.1f, 0.35f, 0.1f, 5, 10) },
                {7, new RodingModalInfo(RodingModalType.VeryEasy, 0.1f, 0.35f, 0.1f, 5, 10) },
                {8, new RodingModalInfo(RodingModalType.VeryEasy, 0.1f, 0.35f, 0.1f, 5, 10) },
                {9, new RodingModalInfo(RodingModalType.VeryEasy, 0.1f, 0.35f, 0.1f, 5, 10) },
            },
            new Dictionary<int, RodingModalInfo>() // 2 pirs
            {
                {1, new RodingModalInfo(RodingModalType.Medium, 0.6f, 0.2f, 0.25f, 10, 20) },
                {2, new RodingModalInfo(RodingModalType.Standart, 0.4f, 0.2f, 0.25f, 10, 20) },
                {3, new RodingModalInfo(RodingModalType.Standart, 0.2f, 0.25f, 0.1f, 10, 20) },
                {4, new RodingModalInfo(RodingModalType.Easy, 0.1f, 0.3f, 0.1f, 10, 20) },
                {5, new RodingModalInfo(RodingModalType.Easy, 0.1f, 0.35f, 0.1f, 8, 12) },
                {6, new RodingModalInfo(RodingModalType.VeryEasy, 0.1f, 0.35f, 0.1f, 8, 12) },
                {7, new RodingModalInfo(RodingModalType.VeryEasy, 0.1f, 0.35f, 0.1f, 5, 10) },
                {8, new RodingModalInfo(RodingModalType.VeryEasy, 0.1f, 0.35f, 0.1f, 5, 10) },
                {9, new RodingModalInfo(RodingModalType.VeryEasy, 0.1f, 0.35f, 0.1f, 5, 10) },
            },
            new Dictionary<int, RodingModalInfo>() // 3 pirs
            {
                {1, new RodingModalInfo(RodingModalType.Hard, 1f, 0.1f, 0.4f, 10, 20) },
                {2, new RodingModalInfo(RodingModalType.Medium, 0.6f, 0.1f, 0.4f, 10, 20) },
                {3, new RodingModalInfo(RodingModalType.Standart, 0.4f, 0.2f, 0.25f, 10, 20) },
                {4, new RodingModalInfo(RodingModalType.Standart, 0.2f, 0.25f, 0.1f, 10, 20) },
                {5, new RodingModalInfo(RodingModalType.Easy, 0.1f, 0.3f, 0.1f, 10, 15) },
                {6, new RodingModalInfo(RodingModalType.Easy, 0.1f, 0.35f, 0.1f, 8, 12) },
                {7, new RodingModalInfo(RodingModalType.Easy, 0.1f, 0.35f, 0.1f, 8, 12) },
                {8, new RodingModalInfo(RodingModalType.VeryEasy, 0.1f, 0.35f, 0.1f, 5, 10) },
                {9, new RodingModalInfo(RodingModalType.VeryEasy, 0.1f, 0.35f, 0.1f, 5, 10) },
            },
            new Dictionary<int, RodingModalInfo>() // 4 pirs
            {
                {1, new RodingModalInfo(RodingModalType.Hard, 2f, 0.1f, 0.7f, 10, 20) },
                {2, new RodingModalInfo(RodingModalType.Hard, 1f, 0.1f, 0.7f, 10, 20) },
                {3, new RodingModalInfo(RodingModalType.Medium, 0.6f, 0.1f, 0.4f, 10, 20) },
                {4, new RodingModalInfo(RodingModalType.Standart, 0.4f, 0.2f, 0.25f, 10, 20) },
                {5, new RodingModalInfo(RodingModalType.Standart, 0.2f, 0.25f, 0.1f, 10, 20) },
                {6, new RodingModalInfo(RodingModalType.Easy, 0.1f, 0.3f, 0.1f, 10, 15) },
                {7, new RodingModalInfo(RodingModalType.Easy, 0.1f, 0.35f, 0.1f, 8, 12) },
                {8, new RodingModalInfo(RodingModalType.Easy, 0.1f, 0.35f, 0.1f, 8, 12) },
                {9, new RodingModalInfo(RodingModalType.VeryEasy, 0.1f, 0.35f, 0.1f, 5, 10) },
            },
            new Dictionary<int, RodingModalInfo>() // 5 pirs
            {
                {1, new RodingModalInfo(RodingModalType.Hard, 2f, 0.08f, 0.85f, 10, 20) },
                {2, new RodingModalInfo(RodingModalType.Hard, 2f, 0.08f, 0.85f, 10, 20) },
                {3, new RodingModalInfo(RodingModalType.Hard, 1f, 0.1f, 0.7f, 10, 20) },
                {4, new RodingModalInfo(RodingModalType.Medium, 0.6f, 0.1f, 0.4f, 10, 20) },
                {5, new RodingModalInfo(RodingModalType.Standart, 0.4f, 0.2f, 0.25f, 10, 20) },
                {6, new RodingModalInfo(RodingModalType.Standart, 0.2f, 0.25f, 0.1f, 10, 20) },
                {7, new RodingModalInfo(RodingModalType.Easy, 0.1f, 0.3f, 0.1f, 10, 15) },
                {8, new RodingModalInfo(RodingModalType.Easy, 0.1f, 0.35f, 0.1f, 8, 12) },
                {9, new RodingModalInfo(RodingModalType.Easy, 0.1f, 0.35f, 0.1f, 8, 12) },
            },
            new Dictionary<int, RodingModalInfo>() // 6 pirs
            {
                {1, new RodingModalInfo(RodingModalType.Hard, 2.3f, 0.06f, 0.95f, 10, 20) },
                {2, new RodingModalInfo(RodingModalType.Hard, 2f, 0.06f, 0.95f, 10, 20) },
                {3, new RodingModalInfo(RodingModalType.Hard, 2f, 0.08f, 0.85f, 10, 20) },
                {4, new RodingModalInfo(RodingModalType.Hard, 1f, 0.1f, 0.7f, 10, 20) },
                {5, new RodingModalInfo(RodingModalType.Medium, 0.6f, 0.1f, 0.4f, 10, 20) },
                {6, new RodingModalInfo(RodingModalType.Standart, 0.4f, 0.2f, 0.25f, 10, 20) },
                {7, new RodingModalInfo(RodingModalType.Standart, 0.2f, 0.25f, 0.1f, 10, 20) },
                {8, new RodingModalInfo(RodingModalType.Easy, 0.1f, 0.3f, 0.1f, 10, 15) },
                {9, new RodingModalInfo(RodingModalType.Easy, 0.1f, 0.35f, 0.1f, 8, 12) },
            },
            new Dictionary<int, RodingModalInfo>() // 7 pirs
            {
                {1, new RodingModalInfo(RodingModalType.Hard, 2.5f, 0.05f, 1f, 10, 20) },
                {2, new RodingModalInfo(RodingModalType.Hard, 2.3f, 0.05f, 1f, 10, 20) },
                {3, new RodingModalInfo(RodingModalType.Hard, 2f, 0.06f, 0.95f, 10, 20) },
                {4, new RodingModalInfo(RodingModalType.Hard, 2f, 0.08f, 0.85f, 10, 20) },
                {5, new RodingModalInfo(RodingModalType.Hard, 1f, 0.1f, 0.7f, 10, 20) },
                {6, new RodingModalInfo(RodingModalType.Medium, 0.6f, 0.1f, 0.4f, 10, 20) },
                {7, new RodingModalInfo(RodingModalType.Standart, 0.4f, 0.2f, 0.4f, 10, 20) },
                {8, new RodingModalInfo(RodingModalType.Standart, 0.2f, 0.25f, 0.1f, 10, 20) },
                {9, new RodingModalInfo(RodingModalType.Easy, 0.1f, 0.3f, 0.1f, 10, 15) },
            },
            new Dictionary<int, RodingModalInfo>() // 8 pirs
            {
                {1, new RodingModalInfo(RodingModalType.Hard, 2.5f, 0.02f, 1.5f, 10, 20) },
                {2, new RodingModalInfo(RodingModalType.Hard, 2.5f, 0.02f, 1.5f, 10, 20) },
                {3, new RodingModalInfo(RodingModalType.Hard, 2.3f, 0.05f, 1f, 10, 20) },
                {4, new RodingModalInfo(RodingModalType.Hard, 2f, 0.06f, 0.95f, 10, 20) },
                {5, new RodingModalInfo(RodingModalType.Hard, 2f, 0.08f, 0.85f, 10, 20) },
                {6, new RodingModalInfo(RodingModalType.Hard, 1f, 0.1f, 0.7f, 10, 20) },
                {7, new RodingModalInfo(RodingModalType.Hard, 0.6f, 0.1f, 0.7f, 10, 20) },
                {8, new RodingModalInfo(RodingModalType.Medium, 0.4f, 0.2f, 0.5f, 10, 20) },
                {9, new RodingModalInfo(RodingModalType.Standart, 0.2f, 0.25f, 0.1f, 10, 20) },
            },
        };

        public static Dictionary<int, float> RodTypeTime = new Dictionary<int, float>()
        {
            {1, 0f },
            {2 ,0.1f },
            {3, 0.2f }
        };


        // 1 - 2 0.2 0.25 10 - 20
        // 2 - 2 0.3 0.25 10 - 20

        public static int GetPlayerRodLevel(Player player)
        {
            if (!Main.Players.ContainsKey(player)) return 1;

            int lvl = 1;

            foreach(int exp in RodLevel)
            {
                if (Main.Players[player].RodExp >= exp) lvl++;
                else break;
            }

            return lvl;
        }

        public static int GetRodAreaLevel(int areaId)
        {
            if (!RodingsArea.ContainsKey(areaId)) return 1;

            return RodingsArea[areaId].Level;
        }

        public static ItemType GetRandomFish(Player player, int fishLevel)
        {
            try
            {
                List<Tuple<int, ItemType>> list = FishLevel[fishLevel - 1];

                int max = list.Last().Item1;

                int randNumber = WorkManager.rnd.Next(1, max);

                foreach (Tuple<int, ItemType> tuple in list)
                {
                    if (tuple.Item1 >= randNumber)
                    {
                        return tuple.Item2;
                    }
                }

                return ItemType.Debug;
            }
            catch(Exception ex)
            {
                Log.Write($"{ex.StackTrace}", nLog.Type.Error);
                return ItemType.Debug;
            }
        }

        public static ItemType GetSellingItemType(string name)
        {
            var type = ItemType.Naz;
            switch (name)
            {
                case "Кета":
                    type = ItemType.Keta;
                    break;
                case "Горбуша":
                    type = ItemType.Gorbysha;
                    break;
                case "Кунджа":
                    type = ItemType.Kyndja;
                    break;
                case "Сиг":
                    type = ItemType.Sig;
                    break;
                case "Омуль":
                    type = ItemType.Omyl;
                    break;
                case "Нерка":
                    type = ItemType.Nerka;
                    break;
                case "Форель":
                    type = ItemType.Forel;
                    break;
                case "Шип":
                    type = ItemType.Ship;
                    break;
                case "Лопатонос":
                    type = ItemType.Lopatonos;
                    break;
                case "Осетр":
                    type = ItemType.Osetr;
                    break;
                case "Семга":
                    type = ItemType.Semga;
                    break;
                case "Сервюга":
                    type = ItemType.Servyga;
                    break;
                case "Белуга":
                    type = ItemType.Beluga;
                    break;
                case "Таймень":
                    type = ItemType.Taimen;
                    break;
                case "Стерлядь":
                    type = ItemType.Sterlyad;
                    break;
                case "Удильщик":
                    type = ItemType.Ydilshik;
                    break;
                case "Хайлиод":
                    type = ItemType.Hailiod;
                    break;
            }
            return type;
        }

        public static string GetNameByItemType(ItemType tupe)
        {
            string type = "nope";
            switch (tupe)
            {
                case ItemType.Keta:
                    type = "Кета";
                    break;
                case ItemType.Gorbysha:
                    type = "Горбуша";
                    break;
                case ItemType.Kyndja:
                    type = "Кунджа";
                    break;
                case ItemType.Sig:
                    type = "Сиг";
                    break;
                case ItemType.Omyl:
                    type = "Омуль";
                    break;
                case ItemType.Nerka:
                    type = "Нерка";
                    break;
                case ItemType.Forel:
                    type = "Форель";
                    break;
                case ItemType.Ship:
                    type = "Шип";
                    break;
                case ItemType.Lopatonos:
                    type = "Лопатонос";
                    break;
                case ItemType.Osetr:
                    type = "Осетр";
                    break;
                case ItemType.Semga:
                    type = "Семга";
                    break;
                case ItemType.Servyga:
                    type = "Сервюга";
                    break;
                case ItemType.Beluga:
                    type = "Белуга";
                    break;
                case ItemType.Taimen:
                    type = "Таймень";
                    break;
                case ItemType.Sterlyad:
                    type = "Стерлядь";
                    break;
                case ItemType.Ydilshik:
                    type = "Удильщик";
                    break;
                case ItemType.Hailiod:
                    type = "Хайлиод";
                    break;
            }

            return type;
        }


        private static nLog Log = new nLog("RodManager");

        private static int lastRodID = -1;

        [ServerEvent(Event.ResourceStart)]
        public void onResourceStart()
        {
            try
            {
                var result = MySQL.QueryRead($"SELECT * FROM rodings");
                if (result == null || result.Rows.Count == 0)
                {
                    Log.Write("DB rod return null result.", nLog.Type.Warn);
                    return;
                }
                foreach (DataRow Row in result.Rows)
                {
                    Vector3 pos = JsonConvert.DeserializeObject<Vector3>(Row["pos"].ToString());

                    Roding data = new Roding(Convert.ToInt32(Row["id"]), pos, Convert.ToInt32(Row["radius"]), Convert.ToInt32(Row["level"]), Convert.ToBoolean(Row["is_blip"]));
                    int id = Convert.ToInt32(Row["id"]);
                    RodingsArea.Add(id, data);
                    lastRodID = id;
                }
            }
            catch (Exception e)
            {
                Log.Write("EXCEPTION AT \"RODINGS\":\n" + e.ToString(), nLog.Type.Error);
            }
        }


        public static void createRodAreaCommand(Player player, float radius, int level, bool isBlip)
        {
            if (!Group.CanUseCmd(player, "createrod")) return;

            var pos = player.Position;
            pos.Z -= 1.12F;

            ++lastRodID;
            Roding biz = new Roding(lastRodID, pos, radius, level, isBlip);
            RodingsArea.Add(lastRodID, biz);

            //MySQL.Query($"INSERT INTO rodings (id, pos, radius, level, is_blip) " + $"VALUES ({lastRodID}, '{JsonConvert.SerializeObject(pos)}', {radius}, {level}, {isBlip})");

            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = "INSERT INTO `rodings` SET " +
              "`id`=@id," +
              "`pos`=@pos," +
              "`radius`=@radius," +
              "`level`=@level," +
              "`is_blip`=@is_blip";

            cmd.Parameters.AddWithValue("@id", lastRodID);
            cmd.Parameters.AddWithValue("@pos", JsonConvert.SerializeObject(pos));
            cmd.Parameters.AddWithValue("@radius", radius);
            cmd.Parameters.AddWithValue("@level", level);
            cmd.Parameters.AddWithValue("@is_blip", isBlip);

            MySQL.Query(cmd);
        }

        public static void deleteRodAreaCommand(Player player, int id)
        {
            if (!Group.CanUseCmd(player, "createrod")) return;

            if (!RodingsArea.ContainsKey(id))
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Такого номера нет в списке");
                return;
            }

            RodingsArea[id].Destroy();
            RodingsArea.Remove(id);

            //MySQL.Query($"DELETE FROM `rodings` WHERE `id` = {id}");

            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = "DELETE FROM `rodings` WHERE `id`=@id";

            cmd.Parameters.AddWithValue("@id", id);
            MySQL.Query(cmd);

            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы удалили зону для рыбалки #{id}");
        }

        [Command("setlvlrod", Hide = true)]
        public static void setLevelRodAreaCommand(Player player, int id, int level)
        {
            if (!Group.CanUseCmd(player, "createrod")) return;

            if (!RodingsArea.ContainsKey(id))
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Такого номера нет в списке");
                return;
            }

            RodingsArea[id].Level = level;

            //MySQL.Query($"UPDATE `rodings` SET `level` = {level} WHERE `id` = {id}");
            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = "UPDATE `rodings` SET `level`=@level WHERE `id`=@id";

            cmd.Parameters.AddWithValue("@level", level);
            cmd.Parameters.AddWithValue("@id", id);
            MySQL.Query(cmd);

            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы установили зоне #{id} для рыбалки {level} уровень");
        }

        public enum AnimationFlags
        {
            Loop = 1 << 0,
            StopOnLastFrame = 1 << 1,
            OnlyAnimateUpperBody = 1 << 4,
            AllowPlayerControl = 1 << 5,
            Cancellable = 1 << 7
        }

        public static void setallow(Player player)
        {
            Trigger.ClientEvent(player, "CLIENT::FISHING", true);

            player.SetMyData("FISHING", true);
            Main.OnAntiAnim(player);
            player.PlayAnimation("amb@world_human_stand_fishing@base", "base", 31);
            //BasicSync.DetachObject(player);
            //BasicSync.AttachObjectToPlayer(player, NAPI.Util.GetHashKey("prop_fishing_rod_01"), 60309, new Vector3(0.03, 0, 0.02), new Vector3(0, 0, 50));
            BasicSync.AddAttachment(player, "roding", true);
            BasicSync.AddAttachment(player, "roding", false);

            int areaLvl = GetRodAreaLevel(player.GetMyData<int>("ROD_ID"));
            int playerRodLvl = GetPlayerRodLevel(player);
            int rodLvl = player.GetMyData<int>("FISHLEVEL");

            /*if (PirsRodType[areaLvl - 1] > rodLvl)
            {
                int rand = 0;

                if(PirsRodType[areaLvl - 1] - rodLvl == 2)
                {
                    rand = WorkManager.rnd.Next(0, 2);

                    switch (rand)
                    {
                        case 0:
                            BasicSync.DetachObject(player);
                            if (rodLvl == 1)
                                nInventory.Remove(player, ItemType.Rod, 1);
                            else if (rodLvl == 2)
                                nInventory.Remove(player, ItemType.RodUpgrade, 1);
                            else if (rodLvl == 3)
                                nInventory.Remove(player, ItemType.RodMK2, 1);

                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "У вас сломалась удочка!");
                            player.StopAnimation();
                            Main.OffAntiAnim(player);
                            player.SetMyData("FISHING", false);
                            var oldwItem = nInventory.Items[Main.Players[player].UUID].FirstOrDefault(i => (i.Type == ItemType.Rod || i.Type == ItemType.RodMK2 || i.Type == ItemType.RodUpgrade) && i.IsActive);
                            BasicSync.DetachObject(player);
                            player.SetMyData("RodInHand", false);

                            oldwItem.IsActive = false;

                            if (nInventory.WeaponsItems.Contains(oldwItem.Type) && !oldwItem.IsActive && oldwItem.fast_slot_id > 0) GUI.Dashboard.SyncAttachComp(player, oldwItem, false);
                            GUI.Dashboard.Update(player, oldwItem, nInventory.Items[Main.Players[player].UUID].IndexOf(oldwItem));
                            return;
                    }
                }
                else
                {
                    rand = WorkManager.rnd.Next(0, 9);

                    switch (rand)
                    {
                        case 0:
                            BasicSync.DetachObject(player);
                            if (rodLvl == 1)
                                nInventory.Remove(player, ItemType.Rod, 1);
                            else if (rodLvl == 2)
                                nInventory.Remove(player, ItemType.RodUpgrade, 1);
                            else if (rodLvl == 3)
                                nInventory.Remove(player, ItemType.RodMK2, 1);

                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "У вас сломалась удочка!");
                            player.StopAnimation();
                            Main.OffAntiAnim(player);
                            player.SetMyData("FISHING", false);
                            var oldwItem = nInventory.Items[Main.Players[player].UUID].FirstOrDefault(i => (i.Type == ItemType.Rod || i.Type == ItemType.RodMK2 || i.Type == ItemType.RodUpgrade) && i.IsActive);
                            BasicSync.DetachObject(player);
                            player.SetMyData("RodInHand", false);

                            oldwItem.IsActive = false;

                            if (nInventory.WeaponsItems.Contains(oldwItem.Type) && !oldwItem.IsActive && oldwItem.fast_slot_id > 0) GUI.Dashboard.SyncAttachComp(player, oldwItem, false);
                            GUI.Dashboard.Update(player, oldwItem, nInventory.Items[Main.Players[player].UUID].IndexOf(oldwItem));
                            return;
                    }
                }
            }*/

            int time = WorkManager.rnd.Next(RodModalData[areaLvl - 1][playerRodLvl].MinWaiting, RodModalData[areaLvl - 1][playerRodLvl].MaxWaiting);

            NAPI.Task.Run(() => {
                try
                {
                    if (player != null && Main.Players.ContainsKey(player))
                    {
                        RodManager.allowfish(player);
                    }
                }
                catch (Exception e){ Log.Write(e.StackTrace, nLog.Type.Error); }
            }, time);

            Trigger.ClientEvent(player, "playerInteractionCheck", false);
        }

        public static void allowfish(Player player)
        {
            player.PlayAnimation("amb@world_human_stand_fishing@idle_a", "idle_c", 31);
            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Что-то клюнуло", 1000);

            int areaLvl = GetRodAreaLevel(player.GetMyData<int>("ROD_ID"));
            int playerRodLvl = GetPlayerRodLevel(player);
            int rodLvl = player.GetMyData<int>("FISHLEVEL");


            var dict = new Dictionary<string, object>
            {
                {"type", (int)RodModalData[areaLvl - 1][playerRodLvl].ModalType },
                {"speed", RodModalData[areaLvl - 1][playerRodLvl].Speed },
                {"speedProgress", RodModalData[areaLvl - 1][playerRodLvl].SpeedFish + RodTypeTime[rodLvl]},
                {"speedDownProgress", RodModalData[areaLvl - 1][playerRodLvl].SpeedFishDown },
            };

            Trigger.ClientEvent(player, "CLIENT::fising:startGame", JsonConvert.SerializeObject(dict));
        }

        /*[Command("sfish")]
        public static void startfish(Player player, int type, float speed, float speedProgress, float speedDownProgress)
        {
            player.PlayAnimation("amb@world_human_stand_fishing@idle_a", "idle_c", 31);
            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Что-то клюнуло", 1000);

            //player.TriggerEvent("fishingBaitTaken");

            var dict = new Dictionary<string, object>
            {
                {"type", type },
                {"speed", speed },
                {"speedDownProgress",speedDownProgress },
                { "speedProgress", speedProgress },
            };
            Trigger.ClientEvent(player, "CLIENT::fising:startGame", JsonConvert.SerializeObject(dict));
        }*/

        public static void crashpros(Player player)
        {
            player.StopAnimation();
            Main.OffAntiAnim(player);

            var UUID = Main.Players[player].UUID;
            var item = nInventory.Items[UUID].FirstOrDefault(i => (i.Type == ItemType.Rod || i.Type == ItemType.RodMK2 || i.Type == ItemType.RodUpgrade) && i.IsActive);
            if(item != null)
            {
                //BasicSync.DetachObject(player);
                //BasicSync.AttachObjectToPlayer(player, nInventory.ItemModels[item.Type], 18905, nInventory.ItemsPosOffset[item.Type], nInventory.ItemsRotOffset[item.Type]);
                BasicSync.AddAttachment(player, "roding", true);
                BasicSync.AddAttachment(player, "roding", false);
            }

            player.SetMyData("FISHING", false);
        }

        //[Command("setrodlvl", Hide = true)]
        //public static void SetRodLvl(Player player, int value)
        //{
        //    if (!Group.CanUseCmd(player, "createrod")) return;
        //    Main.Players[player].RodExp = value;
        //}

        //[Command("rodtest", Hide = true)]
        //public static void testRodBug(Player player)
        //{
        //    giveRandomFish(player);
        //}

        [RemoteEvent("SERVER::ROD:GIVE_FISH")]
        public static void giveRandomFish(Player player)
        {
            if (player == null || !Main.Players.ContainsKey(player)) return;
            if (!player.HasMyData("ROD_ID") || !player.HasMyData("ALLOWFISHING") || player.GetMyData<bool>("ALLOWFISHING") == false) return;

            int areaLvl = GetRodAreaLevel(player.GetMyData<int>("ROD_ID"));
            int playerRodLvl = GetPlayerRodLevel(player);

            int fishLevel = 1;

            if (areaLvl == 1 && playerRodLvl > 1) fishLevel = 2;
            else if (areaLvl == 1 && playerRodLvl == 1) fishLevel = 1;
            else if (areaLvl > 1) fishLevel = areaLvl + 1;

            ItemType item = GetRandomFish(player, fishLevel);

            var tryAdd = nInventory.TryAdd(player, new nItem(item));
            if (tryAdd == -1)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Недостаточно места в инвентаре", 3000);
                RodManager.crashpros(player);
                return;
            }

            nInventory.Add(player, new nItem(item, 1));
            Main.Players[player].RodExp++;

            #region GBPКвест: 2 Выловить 3500 омулей. / 3 Выловить 2300 шипов.

            #region BattlePass выполнение квеста
            if (item == ItemType.Omyl) BattlePass.updateBPGlobalQuestIteration(player, BattlePass.BPGlobalQuestType.RodFishOmyl);
            if (item == ItemType.Ship) BattlePass.updateBPGlobalQuestIteration(player, BattlePass.BPGlobalQuestType.RodFishShip);
            BattlePass.updateBPGlobalQuestIteration(player, BattlePass.BPGlobalQuestType.RodCountFish);
            #endregion

            #endregion

            if(GetPlayerRodLevel(player) != playerRodLvl)
            {
                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Ваш навык рыбалки улучшен!", 6000);

                if (GetPlayerRodLevel(player) == 3 && playerRodLvl == 4)
                {
                    #region BPКвест: 122  Повысить уровень рыбалки до 4 уровня.

                    #region BattlePass выполнение квеста
                    BattlePass.updateBPQuestIteration(player, BattlePass.BPQuestType.RodLVLUP4, playerRodLvl);
                    #endregion

                    #endregion
                }

                if (GetPlayerRodLevel(player) == 6 && playerRodLvl == 7)
                {
                    #region BPКвест: 121  Повысить уровень рыбалки до 7 уровня.

                    #region BattlePass выполнение квеста
                    BattlePass.updateBPQuestIteration(player, BattlePass.BPQuestType.RodLVLUP7, playerRodLvl);
                    #endregion

                    #endregion
                }

                if (GetPlayerRodLevel(player) == 8 && playerRodLvl == 9)
                {
                    #region SBPКвест: 18 Получить максимальный уровень рыбалки.

                    #region BattlePass выполнение квеста
                    BattlePass.updateBPSuperQuestIteration(player, BattlePass.BPSuperQuestType.GetMaxLVLRod);
                    #endregion

                    #endregion
                }
            }

            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы поймали рыбу {GetNameByItemType(item)}", 3000);

            RodManager.crashpros(player);
        }

        [RemoteEvent("SERVER::ROD:FAILED")]
        public static void stopFishDrop(Player player)
        {
            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Рыба сошла с крючка!", 3000);
            RodManager.crashpros(player);
        }

        public static void interactionPressed(Player player)
        {
            var UUID = Main.Players[player].UUID;
            var item = nInventory.Items[UUID].FirstOrDefault(i => (i.Type == ItemType.Rod || i.Type == ItemType.RodMK2 || i.Type == ItemType.RodUpgrade) && i.IsActive);

            if(item != null)
            {
                switch(item.Type)
                {
                    case ItemType.Rod:
                        startFishing(player, 1);
                        break;
                    case ItemType.RodUpgrade:
                        startFishing(player, 2);
                        break;
                    case ItemType.RodMK2:
                        startFishing(player, 3);
                        break;
                }
            }
        }

        public static void startFishing(Player player, int level)
        {
            if (player.IsInVehicle)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы не должны находится в машине!", 3000);
                GUI.Dashboard.Close(player);
                return;
            }
            if (player.HasMyData("FISHING") && player.GetMyData<bool>("FISHING") == true)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы уже рыбачите!", 3000);
                return;
            }
            var aItem = nInventory.Find(Main.Players[player].UUID, ItemType.Naz);
            if (aItem == null)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "У Вас нет наживки", 3000);
                return;
            }
            //Log.Write("allowfishing -> " + player.GetMyData<bool>("ALLOWFISHING"), nLog.Type.Error);
            if (player.HasMyData("ALLOWFISHING") && player.GetMyData<bool>("ALLOWFISHING") == false)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "В данном месте нельзя рыбачить", 3000);
                return;
            }

            nInventory.Remove(player, ItemType.Naz, 1);
            player.SetMyData("FISHLEVEL", level);
            RodManager.setallow(player);
            //Commands.RPChat("me", player, $"Начал(а) рыбачить");
        }

        public enum RodingModalType
        {
            VeryEasy = 0,
            Easy = 1,
            Standart = 2,
            Medium = 3,
            Hard = 4,
        }

        public class RodingModalInfo
        {
            public RodingModalType ModalType { get; set; }
            public float Speed { get; set; }
            public float SpeedFish { get; set; }
            public float SpeedFishDown { get; set; }
            public int MinWaiting { get; set; }
            public int MaxWaiting { get; set; }

            public RodingModalInfo(RodingModalType type, float speed, float speedFish, float speedFishDown, int min, int max)
            {
                ModalType = type;
                Speed = speed;
                SpeedFish = speedFish;
                SpeedFishDown = speedFishDown;
                MinWaiting = min * 1000;
                MaxWaiting = max * 1000;
            }
        }

        public class Roding
        {
            public int ID { get; set; }
            public float Radius { get; set; }
            public Vector3 AreaPoint { get; set; }
            public bool IsBlipped { get; set; }
            public int Level { get; set; }


            [JsonIgnore]
            private Blip blip = null;
            [JsonIgnore]
            private ColShape shape = null;
            //[JsonIgnore]
            //private TextLabel label = null;
            //[JsonIgnore]
            //private Marker marker = null;

            public Roding(int id, Vector3 areapoint, float radius, int level, bool isBlipped = false)
            {
                // var
                ID = id;
                AreaPoint = areapoint;
                Radius = radius;
                Level = level;
                IsBlipped = isBlipped;

                // Create blip
                if(IsBlipped) blip = NAPI.Blip.CreateBlip(68, AreaPoint, 1, 67, "Место для рыбалки", 255, 0, true);

                //Create shape
                shape = NAPI.ColShape.CreateCylinderColShape(AreaPoint, Radius, 3, 0);

                //Shape rules
                shape.OnEntityEnterColShape += (s, entity) =>
                {
                    try
                    {
                        if (!Main.Players.ContainsKey(entity)) return;
                        entity.SetMyData("ALLOWFISHING", true);
                        entity.SetMyData("ROD_ID", ID);
                        entity.SetMyData("INTERACTIONCHECK", 1009);
                    }
                    catch (Exception e) { Console.WriteLine("shape.OnEntityEnterColshape: " + e.StackTrace); }
                };
                shape.OnEntityExitColShape += (s, entity) =>
                {
                    try
                    {
                        //Set Data
                        entity.SetMyData("ALLOWFISHING", false);
                        entity.SetMyData("INTERACTIONCHECK", 0);
                        entity.ResetMyData("ROD_ID");
                        //Debug
                        //Log.Write("Player exit in ColShape.", nLog.Type.Info);
                        //Stop Animation
                        //NAPI.Player.StopPlayerAnimation(entity);
                        //Remove object from left hand
                        //BasicSync.DetachObject(entity);
                    }
                    catch (Exception e) { Console.WriteLine("shape.OnEntityEnterColshape: " + e.StackTrace); }
                };

                //Debug place
                //label = NAPI.TextLabel.CreateTextLabel("Место ловли рыбы", new Vector3(AreaPoint.X, AreaPoint.Y, AreaPoint.Z + 1.5), 20F, 0.5F, 0, new Color(255, 255, 255), true, 0);

                //Create marker
                //marker = NAPI.Marker.CreateMarker(1, AreaPoint - new Vector3(0, 0, 1f - 0.3f), new Vector3(), new Vector3(), Radius, new Color(255, 255, 255, 220), false, 0);

            }

            public void Destroy()
            {
                NAPI.Task.Run(() => {
                    if(blip != null) blip.Delete();
                    shape.Delete();
                });
            }

        }



    }
}
