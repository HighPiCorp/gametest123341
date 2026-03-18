using System.Collections.Generic;
using System;
using GTANetworkAPI;
using Newtonsoft.Json;
using NeptuneEvo.SDK;
using NeptuneEvo.GUI;
using NeptuneEvo.MoneySystem;
using client.Systems.BattlePass;
//using NeptuneEvo.Core.Quest;

namespace NeptuneEvo.Core
{
    class DrivingSchool : Script
    {
        public static Ped Ped = new Ped(new Vector3(-707.0482, -1307.227, 5.369364), 231.698f, "cs_andreas", "Инструктор");
        public static Vector3 enterSchool = new Vector3(-705.6069, -1308.207, 4.269361);
        public static Vector3 inspector = new Vector3(-701.53, -1311.013, 8.429047);

        public static uint PrivateDimension = 15000;

        private static List<Licenses> Licenses = new List<Licenses>() {
            new Licenses("Лицензия на мотоцикл", "Категория A", 500),
            new Licenses("Лицензия на легковые авто", "Категория B", 750),
            new Licenses("Лицензия на грузовые авто", "Категория C", 5000),
            new Licenses("Лицензия на водный транспорт", "Категория V", 10000),
            new Licenses("Лицензия на вертолеты", "Категория LV", 15000),
            new Licenses("Лицензия на самолеты", "Категория LS", 20000),
        };

        public static List<string> rules = new List<string>()
        {
            "1. Водитель автотранспорта обязан остановиться по первому требованию представителя правоохранительных органов." +
            "2. В случае ДТП, при котором пострадали (были ранены или погибли) люди, водитель не вправе перемещать предметы (улики), в т.ч. транспортные средства, на месте аварии, которые связаны с происшествием." +
            "3. Водителю ТС запрещается находиться за рулем в состоянии наркотического и/или алкогольного опьянения. Допустимый процент содержания алкоголя в крови - не более 0,008 промилле, что приравнивается к 1 бутылке пива." +
            "4. Владельцу транспортного средства запрещено передавать автомобиль третьим лицам, находящимся в состоянии алкогольного и/или наркотического опьянения, а также лицам не имеющим лицензии (права на вождение ТС)" +
            "5. Водителю ТС запрещается препятствовать движению кортежей и организованных колоннах, а также занимать в ней место." +
            "6. Водитель транспортного средства обязан обеспечить беспрепятственный проезд  автомобилям, оборудованным проблесковыми маячками красного или синего цвета." +
            "7. Пешеходам запрещено беспорядочное движение по проезжей части. Пешеходы обязаны двигаться по тротуару или обочине." +
            "8. Водитель транспортного средства вправе совершить правый поворот на красный сигнал светофора, если при этом не создается помеха для других участников движения." +
            "9. Водитель ТС обязан уступить дорогу другим участникам дорожного движения при выезде на проезжую часть с прилегающей территории." +
            "10. Водитель обязан уступить дорогу ТС, которые движутся без изменения направления движения." +
            "11. Водитель обязан уступить дорогу встречным ТС при повороте налево или развороте вне перекрестка" +
            "12. Водитель обязан пропустить ТС, приближающееся справа на нерегулируемом перекрестке." +
            "13. Разворот запрещен на мостах, ж/д переездах, в тоннелях, а также в местах с видимостью менее 100м." +
            "14. Водитель обязан включать дальний или ближний свет фар в темное время суток, а также в тоннелях." +
            "15. Водитель ТС обязан переключить дальний свет фар на ближний в населенных пунктах, где дорога освещена",
        };

        public static List<List<Question>> Tests = new List<List<Question>>()
        {
            new List<Question>{
                new Question("Обязан ли водитель предоставить права на владение транспортным средством представителю правоохранительных органов?",
                    new List<string>{ "Не обязан", "Обязан", "Обязан, если водитель нарушил ПДД"}, 1),
                new Question("В случае ДТП, в котором пострадали люди разрешается ли водителю самостоятельно передвигать транспортные средства и предметы на месте аварии?",
                    new List<string>{ "Запрещено", "Разрешено, с в случае если предметы и автомобили мешают проезду другим участникам движения", "Разрешено"}, 0),
                new Question("Разрешено ли водителю находиться за рулем в состоянии алкогольного и/или наркотического опьянения?",
                    new List<string>{ "Разрешено, если водитель никуда не двигается", "Запрещено", "Разрешено, если содержания алкоголя в крови не превышает 0,008 промилле (1 бутылка пива)."}, 2),
                new Question("Разрешается ли владельцу транспортного средства передавать автомобиль третьим лицам, находящимся в состоянии алкогольного и/или наркотического опьянения, а также лицам не имеющим лицензии?",
                    new List<string>{ "Запрещается", "Разрешается"}, 0),
                new Question("Разрешается ли водителю ТС препятствовать движению кортежей и организованных колоннах, а также занимать в них место?",
                    new List<string>{ "Разрешено, если водитель ТС не мешает кортежу или колонне", "Запрещено"}, 1),
                new Question("Обязан ли водитель транспортного средства обеспечить беспрепятственный проезд  автомобилям, оборудованным проблесковыми маячками красного или синего цвета?",
                    new List<string>{ "Не обязан", "Обязан"}, 1),
                new Question("Разрешено ли пешеходам передвигаться в любом месте проезжей части?",
                    new List<string>{ "Разрешено, в случае если на дорогах нет препятствующих передвижению ТС", "Запрещено"}, 1),
                new Question("Если загорелся красный сигнал светофора и на проезжей части нет других участников движения, мешающих проезду, в какую сторону разрешается сделать поворот?",
                    new List<string>{ "Разрешается правый поворот", "Разрешается левый поворот", "Верны оба варианта"}, 0),
                new Question("Обязан ли водитель ТС уступить дорогу другим участникам дорожного движения при выезде на проезжую часть с прилегающей территории?",
                    new List<string>{ "Обязан", "Не обязан"}, 0),
                new Question("Обязан ли водитель уступить дорогу ТС, которые движутся без изменения направления движения?",
                    new List<string>{ "Да. Обязан", "Нет. Не обязан"}, 0),
                new Question("В каком случае водитель обязан уступить дорогу встречным транспортным средствам?",
                    new List<string>{ "При повороте налево или развороте вне перекрестка", "При развороте направо или развороте вне перекрестка", "Верны оба варианта"}, 0),
                new Question("Обязан ли водитель пропустить ТС, которое приближается справа на нерегулируемом перекрестке. ",
                    new List<string>{ "Не обязан", "Обязан только автомобилям, оснащенным проблесковыми маячками", "Обязан"}, 2),
                new Question("Где запрещен разворот?",
                    new List<string>{ "На мостах", "В тоннелях", "На ж/д путях", "Верны все варианты"}, 3),
                new Question("Обязан ли водитель ТС включать дальний или ближний свет фар в темное время суток, а также в тоннелях?",
                    new List<string>{ "Не обязан", "Обязан"}, 1),
                new Question("Где водитель обязан переключить дальний свет фар на ближний?",
                    new List<string>{ "В тоннелях", "На шоссе", "В населенных пунктах, где дорога освещена"}, 2),
            },
        };

        private static List<Vector3> startCourseCoord = new List<Vector3>()
        {
            new Vector3(-712.8968, -1272.6619, 4.7835803),
            new Vector3(-710.6047, -1274.7465, 4.7836556),
            new Vector3(-708.17114, -1276.872, 4.783617),
            new Vector3(-705.5837, -1279.0485, 4.7838273),
            new Vector3(-718.8506, -1290.6876, 4.7835064),
            new Vector3(-720.9272, -1293.2275, 4.7837048),
            new Vector3(-722.9923, -1295.8157, 4.783622),
            new Vector3(-725.09235, -1298.3031, 4.7838516),
            new Vector3(-738.0276, -1303.15, 4.7834926),
            new Vector3(-740.0059, -1305.6495, 4.7837725),
            new Vector3(-742.129, -1308.1387, 4.7831025),
            new Vector3(-744.1879, -1310.5803, 4.7834783),
            new Vector3(-746.2246, -1313.0673, 4.783197),
            new Vector3(-748.12665, -1315.5856, 4.783529),
            new Vector3(-781.91736, -1295.1791, 4.7836723),
            new Vector3(-785.07166, -1294.6146, 4.7836437),
            new Vector3(-788.1498, -1294.1062, 4.7837443),
            new Vector3(-791.0957, -1292.978, 4.784126),
            new Vector3(-794.5669, -1293.0303, 4.783782),
            new Vector3(-797.5679, -1292.3635, 4.7831516),
        };
        private static List<Vector3> startCourseRot = new List<Vector3>()
        {
            new Vector3(-0.1587727, -0.0013740149, 140.07411),
            new Vector3(-0.1867619, 0.00019927614, 140.41443),
            new Vector3(-0.2010473, 0.00021380356, 140.4761),
            new Vector3(-0.21950717, 0.02820272, 140.30725),
            new Vector3(-0.20303166, -0.0018768802, 50.484177),
            new Vector3(-0.16763324, -0.00045310042, 50.297035),
            new Vector3(-0.2262201, 0.032875113, 51.53162),
            new Vector3(-0.21591216, 0.016745633, 50.295296),
            new Vector3(-0.20272979, -0.00020787379, 50.256027),
            new Vector3(-0.21587865, 0.0403129, 51.90553),
            new Vector3(-0.21019433, 0.0040433374, 50.759594),
            new Vector3(-0.27420768, 0.0020026993, 50.26655),
            new Vector3(-0.21080822, -0.014129334, 50.16892),
            new Vector3(-0.21588117, 0.03760259, 50.420094),
            new Vector3(-0.15739344, 0.0013965032, -9.535209),
            new Vector3(-0.25440842, -0.0012301409, -8.966266),
            new Vector3(-0.24735112, -0.011949584, -8.719713),
            new Vector3(-0.20265792, 0.015866814, -9.703681),
            new Vector3(-0.21526112, 0.0150051415, -9.933091),
            new Vector3(-0.21908322, 0.016154561, -9.877547),
        };
        private static List<Vector3> drivingCoords = new List<Vector3>()
        {
            new Vector3(-704.6472, -1247.343, 10.67442),     //as1
            new Vector3(-686.6135, -1252.129, 11.03791),     //as2
            new Vector3(-668.1112, -1277.741, 11.11878),     //as3
            new Vector3(-647.9838, -1297.883, 11.11266),     //as4
            new Vector3(-631.2891, -1299.402, 11.10439),     //as5
            new Vector3(-612.2523, -1283.693, 11.10192),     //as6
            new Vector3(-582.577, -1247.959, 13.36889),     //as7
            new Vector3(-551.6786, -1204.123, 18.13077),     //as8
            new Vector3(-540.2383, -1181.224, 19.18878),     //as9
            new Vector3(-510.3582, -1179.808, 20.25262),     //as10
            new Vector3(-489.511, -1218.502, 21.78694),     //as11
            new Vector3(-515.7674, -1296.293, 28.07829),     //as12
            new Vector3(-534.27, -1349.218, 29.60077),     //as13
            new Vector3(-519.2154, -1400.633, 29.65829),     //as14
            new Vector3(-474.7718, -1426.947, 29.6368),     //as15
            new Vector3(-438.2497, -1434.056, 29.6976),     //as16
            new Vector3(-392.2361, -1440.421, 29.69909),     //as17
            new Vector3(-355.8693, -1440.931, 29.81322),     //as18
            new Vector3(-302.8323, -1441.081, 31.73903),     //as19
            new Vector3(-286.3747, -1453.478, 31.72387),     //as20
            new Vector3(-292.7627, -1507.852, 29.64017),     //as21
            new Vector3(-329.7639, -1587.84, 22.43151),     //as22
            new Vector3(-383.8394, -1681.795, 19.32527),     //as23
            new Vector3(-411.5873, -1749.223, 20.53662),     //as24
            new Vector3(-428.1123, -1765.016, 20.96091),     //as25
            new Vector3(-484.4346, -1776.815, 21.43759),     //as26
            new Vector3(-550.9955, -1747.087, 22.35854),     //as27
            new Vector3(-609.9813, -1705.323, 24.43946),     //as28
            new Vector3(-654.9661, -1664.971, 25.63946),     //as29
            new Vector3(-701.2487, -1616.159, 23.39617),     //as30
            new Vector3(-715.5563, -1603.546, 22.83745),     //as31
            new Vector3(-736.1861, -1612.059, 24.43959),     //as32
            new Vector3(-763.5643, -1642.022, 27.34238),     //as33
            new Vector3(-771.7167, -1694.855, 29.66684),     //as34
            new Vector3(-770.2577, -1722.702, 29.6922),     //as35
            new Vector3(-762.8403, -1745.841, 29.74304),     //as36
            new Vector3(-754.4462, -1759.328, 29.74207),     //as37
            new Vector3(-728.3046, -1759.528, 30.03871),     //as38
            new Vector3(-661.0937, -1753.61, 37.98396),     //as39
            new Vector3(-559.1539, -1723.255, 37.78191),     //as40
            new Vector3(-510.9143, -1702.245, 37.25971),     //as41
            new Vector3(-444.6659, -1643.36, 32.99823),     //as42
            new Vector3(-392.0374, -1524.149, 25.89943),     //as43
            new Vector3(-384.0456, -1396.677, 22.64989),     //as44
            new Vector3(-389.9767, -1305.895, 22.36889),     //as45
            new Vector3(-390.8521, -1190.269, 21.19579),     //as46
            new Vector3(-390.8799, -1070.524, 22.41175),     //as47
            new Vector3(-382.655, -914.3037, 35.94333),     //as48
            new Vector3(-384.1105, -832.9343, 39.32611),     //as49
            new Vector3(-392.9055, -739.7156, 37.61785),     //as50
            new Vector3(-393.7488, -666.5231, 37.62029),     //as51
            new Vector3(-393.5527, -604.5363, 33.92249),     //as52
            new Vector3(-404.7963, -508.064, 34.13367),     //as53
            new Vector3(-441.523, -476.5334, 33.31015),     //as54
            new Vector3(-523.574, -468.2849, 32.58505),     //as55
            new Vector3(-609.9857, -475.7358, 35.1473),     //as56
            new Vector3(-632.9848, -477.7071, 35.26155),     //as57
            new Vector3(-644.2718, -491.5341, 35.19373),     //as58
            new Vector3(-644.6234, -541.7328, 35.1782),     //as59
            new Vector3(-645.2361, -570.4226, 35.4185),     //as60
            new Vector3(-640.3406, -635.2014, 32.45972),     //as61
            new Vector3(-640.1005, -683.8199, 31.69821),     //as62
            new Vector3(-640.896, -779.8826, 25.85054),     //as63
            new Vector3(-640.9213, -809.8345, 25.52384),     //as64
            new Vector3(-645.9667, -859.3205, 25.0564),     //as65
            new Vector3(-646.494, -939.6482, 22.5421),     //as66
            new Vector3(-649.717, -1000.834, 20.64941),     //as67
            new Vector3(-752.1108, -1101.364, 11.19446),     //as68
            new Vector3(-769.0291, -1141.633, 11.00855),     //as69
            new Vector3(-731.3914, -1191.951, 11.05298),     //as70
            new Vector3(-703.2067, -1227.076, 11.04342),     //as71
            new Vector3(-709.2208, -1240.626, 10.70718),     //as72
            new Vector3(-725.301, -1298.095, 3.880265),
        };

        private static nLog Log = new nLog("DrivingSc");

        [ServerEvent(Event.ResourceStart)]
        public void onResourceStart()
        {
            try
            {
                new Ped(new Vector3(-690.1148, -1291.703, 5.369365), 85.9252f, "g_m_m_armboss_01");

                ColShape shape = NAPI.ColShape.CreateCylinderColShape(Main.OffsetPosition(Ped.Position, Ped.Rotation, 2), 1, 2, 0);
                shape.OnEntityEnterColShape += onPlayerEnterSchool;
                shape.OnEntityExitColShape += onPlayerExitSchool;

                //NAPI.Marker.CreateMarker(1, enterSchool - new Vector3(0, 0, 0.7), new Vector3(), new Vector3(), 1, new Color(255, 255, 255, 220));
                //NAPI.TextLabel.CreateTextLabel(Main.StringToU16("Автошкола"), new Vector3(enterSchool.X, enterSchool.Y, enterSchool.Z + 1), 5f, 0.3f, 0, new Color(255, 255, 255));
                var blip = NAPI.Blip.CreateBlip(545, Ped.Position, 1, 67, "Автошкола", shortRange: true, dimension: 0);

                for (int i = 0; i < drivingCoords.Count; i++)
                {
                    var colshape = NAPI.ColShape.CreateCylinderColShape(drivingCoords[i], 4, 5, (uint)PrivateDimension);
                    colshape.OnEntityEnterColShape += onPlayerEnterDrive;
                    colshape.SetData("NUMBER", i);
                }

                var rulesCol = NAPI.ColShape.CreateCylinderColShape(new Vector3(-692.5759, -1292.077, 4.269363), 1, 2, 0);
                rulesCol.OnEntityEnterColShape += (s, player) => {
                    player.SetMyData("INTERACTIONCHECK", 1015);
                };
                rulesCol.OnEntityExitColShape += (s, player) => {
                    player.SetMyData("INTERACTIONCHECK", 0);
                };
                NAPI.Marker.CreateMarker(32, new Vector3(-692.5759, -1292.077, 5.069363), new Vector3(), new Vector3(), 0.8f, new Color(255, 255, 0, 150));

                var testCol = NAPI.ColShape.CreateCylinderColShape(new Vector3(-704.2104, -1298.276, 8.429054), 1, 2, 0);
                testCol.OnEntityEnterColShape += (s, player) => {
                    Log.Debug("1016", nLog.Type.Error);
                    player.SetMyData("INTERACTIONCHECK", 1016);
                    //player.SetMyData<Action>(Main.InteractionString, () => OpenTestDialog(player)); // 615
                };
                testCol.OnEntityExitColShape += (s, player) => {
                    Log.Debug("0", nLog.Type.Error);
                    player.SetMyData("INTERACTIONCHECK", 0);
                    //player.ResetMyData(Main.InteractionString);
                };

            }
            catch (Exception e) { Log.Write("ResourceStart: " + e.StackTrace, nLog.Type.Error); }
        }


        [RemoteEvent("choiceNpcAutoschool")]
        public static void AutoschoolСallback(Player player, int id, int paytype)
        {
            if (player.HasMyData("DIALOG_SCHOOL"))
            {
                if (player.GetMyData<int>("DIALOG_SCHOOL") == 2)
                {
                    CheckTest(player, id);
                    return;
                }
                else if (player.GetMyData<int>("DIALOG_SCHOOL") == 3)
                {
                    if (id == 1)
                    {
                        player.SetMyData("AUTO_SCHOOL_PAYTYPE", paytype);
                        SendPlayerToRules(player, id);
                        Trigger.PlayerEvent(player, "client_autoscoolTableClose");
                    }
                    else if (id == 0 || id >= 2 && id <= 5)
                    {
                        player.SetMyData("AUTO_SCHOOL_PAYTYPE", paytype);  //paytype 1 - карта 0 - наличка
                        startDrivingCourse(player, id);
                    }
                }
            }
        }

        [RemoteEvent("autoscoolTableClose")]
        public static void AutoScoolTableClose(Player player)
        {
            if (player.HasMyData("DIALOG_SCHOOL"))
            {
                if (player.GetMyData<int>("DIALOG_SCHOOL") == 1 || player.GetMyData<int>("DIALOG_SCHOOL") == 2)
                {
                    player.ResetMyData("DIALOG_SCHOOL");
                    player.ResetMyData("TEST_NUM");
                    player.ResetMyData("TEST_ERROR");
                    player.ResetMyData("TEST_ARRAY");
                    player.ResetMyData("DIALOG_SCHOOL");
                    player.ResetMyData("DRIVE_PAY");
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы провалили тест", 3000);
                    return;
                }
                else if (player.GetMyData<int>("DIALOG_SCHOOL") == 3)
                {
                    player.ResetMyData("DIALOG_SCHOOL");
                    return;
                }
            }
            else if (player.HasMyData("RULE_READ"))
            {
                SendPlayerToTest(player);
            }
        }

        public static void OpenLicensesDialog(Player player)
        {
            if (player.HasMyData("SCHOOL_DRIVE"))
            {
                SendPlayerToDrive(player);
                Trigger.PlayerEvent(player, "delMarker");
                Trigger.PlayerEvent(player, "deleteWorkBlip");
                player.ResetMyData("SCHOOL_DRIVE");
                return;
            }

            player.SetMyData("DIALOG_SCHOOL", 3);

            List<List<object>> Lic = new List<List<object>>();
            foreach (var lic in Licenses)
            {
                Lic.Add(new List<object> { lic.Type, lic.Category, lic.Price });
            }

            Trigger.PlayerEvent(player, "setAutoscoolTable", "licenses", JsonConvert.SerializeObject(Lic));
        }


        public static void SendPlayerToRules(Player player, int id)
        {
            if (Main.Players[player].Licenses[id])
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У Вас уже есть эта лицензия", 3000);
                return;
            }
            if (player.HasMyData("DRIVE_PAY"))
            {
                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, "Вы уже оплатили. Проходите учить правила.", 3000);
                return;
            }

            int paytype = player.GetMyData<int>("AUTO_SCHOOL_PAYTYPE");
            if (!CanBuyLic(player, id)) return;

            if (paytype == 0)
            {
                if (!Wallet.Change(player, -Licenses[id].Price))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Недостаточно денег", 3000);
                    return;
                }
            }
            else if (paytype == 1)
            {
                if (!Bank.Change(Main.Players[player].Bank, -Licenses[id].Price))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Недостаточно денег", 3000);
                    return;
                }
            }

            client.Fractions.Utils.Stocks.fracStocks[(int)Fractions.Manager.FractionTypesEnum.Cityhall].Money += Licenses[1].Price;
            FractionLogs.FractionMoney((int)Fractions.Manager.FractionTypesEnum.Cityhall, player.Name, Main.Players[player].UUID.ToString(), Licenses[1].Price, "покупка лицензии на категории В");
            GameLog.Money($"player({Main.Players[player].UUID})", $"frac({(int)Fractions.Manager.FractionTypesEnum.Cityhall})", Licenses[1].Price, $"buyLic");

            player.SetMyData("DIALOG_SCHOOL", 3);
            player.SetMyData("RULE_MARKER", true);
            player.SetMyData("DRIVE_PAY", true);
            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, "Для начала прочтите правила", 3000);
            Trigger.PlayerEvent(player, "createWorkBlip", new Vector3(-692.5759, -1292.077, 4.269363));
        }

        public static void SendPlayerToTest(Player player)
        {
            Vector3 pcpos = new Vector3(-704.2104, -1298.276, 8.429054);

            player.ResetMyData("RULE_READ");
            player.ResetMyData("DIALOG_SCHOOL");
            player.ResetMyData("DIALOG_AUTOROOLS");

            if (Main.Players[player].Licenses[1])
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы уже сдали тест", 3000);
                return;
            }
            else
            {
                if (player.HasMyData("DRIVE_PAY"))
                {
                    Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, "Теперь вам нужно пройти тест. (второй этаж) ", 3000);
                    ClientBlipManager.CreateBlip(player, "DriveSchoolTest", 0, pcpos, "Тест", 49, false, false, false);
                    ClientMarkerManager.CreateMarker(player, "DriveSchoolTest", 1, pcpos, 1, new Color(247, 202, 24, 190));

                    player.SetMyData("CANSTARTTEST", true);
                }
            }
        }

        public static void SendPlayerToDrive(Player player)
        {
            player.ResetMyData("DRIVE_PAY");
            Trigger.PlayerEvent(player, "screenFadeOut", 1000);

            NAPI.Task.Run(() => {
                startDrivingCourse(player, 1);
            }, 1200);

            NAPI.Task.Run(() => {
                Trigger.PlayerEvent(player, "screenFadeIn", 1000);
            }, 3000);
        }

        public static void OpenTestDialog(Player player)
        {
            Log.Debug("Open test dialog canstarttest: " + player.HasMyData("CANSTARTTEST"), nLog.Type.Error);
            if (!player.HasMyData("CANSTARTTEST"))
                return;
            player.ResetMyData("CANSTARTTEST");

            ClientBlipManager.DeleteBlip(player, "DriveSchoolTest");
            ClientMarkerManager.DeleteMarker(player, "DriveSchoolTest");

            StartTest(player);
        }

        public static void StartTest(Player player)
        {
            Log.Debug("StartTest: ", nLog.Type.Error);
            player.SetMyData("DIALOG_SCHOOL", 2);
            player.SetMyData("TEST_ERROR", 0);
            player.SetMyData("TEST_COUNT", 0);

            int[] arr = new int[15] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14 };

            Shuffle(arr);

            player.SetMyData("TEST_ARRAY", arr);
            player.SetMyData("TEST_NUM", arr[0]);

            Trigger.PlayerEvent(player, "setAutoscoolTable", "autotest", JsonConvert.SerializeObject(new List<object> { 1, 10, 0, 2, Tests[0][arr[0]].QuestionText, Tests[0][arr[0]].Answers }));
        }

        public static void CheckTest(Player player, int answer)
        {
            if (!player.HasMyData("TEST_NUM"))
                return;

            if (player.HasMyData("SCHOOL_DRIVE"))
                return;

            int testnum = player.GetMyData<int>("TEST_NUM");
            int error = player.GetMyData<int>("TEST_ERROR");
            int count = player.GetMyData<int>("TEST_COUNT");
            int[] tests = player.GetMyData<int[]>("TEST_ARRAY");

            if (Tests[0][testnum].TrueAnswer == answer)
            {
                if (count == 9)
                {

                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Вы успешно сдали тест! Подойдите к инструктору что бы начать практическую часть", 3000);
                    player.ResetMyData("TEST_NUM");
                    player.ResetMyData("TEST_ERROR");
                    player.ResetMyData("TEST_ARRAY");
                    player.SetMyData("SCHOOL_DRIVE", true);
                    player.ResetMyData("DIALOG_SCHOOL");
                    Trigger.PlayerEvent(player, "createWorkBlip", Ped.Position);
                    Trigger.PlayerEvent(player, "setMarker", Ped.Position.X, Ped.Position.Y, Ped.Position.Z);
                    Trigger.PlayerEvent(player, "client_autoscoolTableClose");
                    return;
                }
                player.SetMyData("DIALOG_SCHOOL", 2);
                player.SetMyData("TEST_COUNT", player.GetMyData<int>("TEST_COUNT") + 1);
                count = player.GetMyData<int>("TEST_COUNT");
                player.SetMyData("TEST_NUM", tests[count]);
                //Trigger.PlayerEvent(player, "openNpcAutoschoolDialog", $"Тест. Ошибки {error}/2", $"Вопрос {count + 1}/10. {Tests[type][tests[count]].QuestionText}", Tests[type][tests[count]].Answers);
                Trigger.PlayerEvent(player, "setAutoscoolTable", "autotest", JsonConvert.SerializeObject(new List<object> { count + 1, 10, error, 2, Tests[0][tests[count]].QuestionText, Tests[0][tests[count]].Answers }));
            }
            else
            {
                if (error == 2)
                {
                    player.ResetMyData("TEST_NUM");
                    player.ResetMyData("TEST_ERROR");
                    player.ResetMyData("TEST_ARRAY");
                    player.ResetMyData("DIALOG_SCHOOL");
                    player.ResetMyData("DRIVE_PAY");
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы допустили 3 ошибки и провалили тест", 3000);
                    Trigger.PlayerEvent(player, "client_autoscoolTableClose");
                    return;
                }
                if (count == 9)
                {
                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Вы успешно сдали тест! Подойдите к инструктору что бы начать практическую часть", 3000);
                    player.ResetMyData("TEST_NUM");
                    player.ResetMyData("TEST_ERROR");
                    player.ResetMyData("TEST_ARRAY");
                    player.ResetMyData("DIALOG_SCHOOL");
                    player.SetMyData("SCHOOL_DRIVE", true);
                    Trigger.PlayerEvent(player, "createWorkBlip", Ped.Position);
                    Trigger.PlayerEvent(player, "setMarker", Ped.Position.X, Ped.Position.Y, Ped.Position.Z);
                    Trigger.PlayerEvent(player, "client_autoscoolTableClose");
                    return;
                }
                player.SetMyData("DIALOG_SCHOOL", 2);
                player.SetMyData("TEST_COUNT", player.GetMyData<int>("TEST_COUNT") + 1);
                count = player.GetMyData<int>("TEST_COUNT");
                player.SetMyData("TEST_NUM", tests[count]);
                player.SetMyData("TEST_ERROR", error + 1);
                //Trigger.PlayerEvent(player, "openNpcAutoschoolDialog", $"Тест. Ошибки {error + 1}/2", $"Вопрос {count + 1}/10. {Tests[type][tests[count]].QuestionText}", Tests[type][tests[count]].Answers);
                Trigger.PlayerEvent(player, "setAutoscoolTable", "autotest", JsonConvert.SerializeObject(new List<object> { count + 1, 10, error + 1, 2, Tests[0][tests[count]].QuestionText, Tests[0][tests[count]].Answers }));
            }
        }

        [ServerEvent(Event.PlayerExitVehicle)]
        public void Event_OnPlayerExitVehicle(Player player, Vehicle vehicle)
        {
            try
            {
                if (player.HasMyData("SCHOOLVEH") && player.GetMyData<Vehicle>("SCHOOLVEH") == vehicle)
                {
                    //player.SetMyData("SCHOOL_TIMER", Main.StartT(60000, 99999999, (o) => timer_exitVehicle(player), "SCHOOL_TIMER"));
                    player.SetMyData("SCHOOL_TIMER", Timers.StartOnce(60000, () => timer_exitVehicle(player)));

                    Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, $"Если вы не сядете в машину в течение 60 секунд, то провалите экзамен", 3000);
                    return;
                }
            }
            catch (Exception e) { Log.Write("PlayerExitVehicle: " + e.StackTrace, nLog.Type.Error); }
        }

        private void timer_exitVehicle(Player player)
        {
            NAPI.Task.Run(() => {
                try
                {
                    if (!Main.Players.ContainsKey(player)) return;
                    if (!player.HasMyData("SCHOOLVEH")) return;
                    if (player.IsInVehicle && player.Vehicle == player.GetMyData<Vehicle>("SCHOOLVEH")) return;
                    Vehicle veh = player.GetMyData<Vehicle>("SCHOOLVEH");
                    veh.Delete();
                    Trigger.PlayerEvent(player, "deleteCheckpoint", 12, 0);
                    player.ResetMyData("IS_DRIVING");
                    player.ResetMyData("SCHOOLVEH");
                    //Main.StopT(player.GetMyData("SCHOOL_TIMER"), "timer_36");
                    Timers.Stop(player.GetMyData<string>("SCHOOL_TIMER"));
                    player.ResetMyData("SCHOOL_TIMER");
                    Trigger.PlayerEvent(player, "busUnRoute");
                    NAPI.Task.Run(() => {
                        NAPI.Entity.SetEntityDimension(player, 0);
                        player.Position = inspector;
                    });
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы провалили экзмен", 3000);
                }
                catch (Exception e) { Log.Write("TimerDrivingSchool: " + e.StackTrace, nLog.Type.Error); }
            });
        }

        public static void onPlayerDisconnected(Player player, DisconnectionType type, string reason)
        {
            if (!Main.Players.ContainsKey(player)) return;
            NAPI.Task.Run(() => {
                try
                {
                    if (!Main.Players.ContainsKey(player)) return;
                    if (player.HasMyData("SCHOOLVEH")) {
                        Vehicle veh = player.GetMyData<Vehicle>("SCHOOLVEH");
                        veh.Delete();
                    }
                }
                catch (Exception e) { Log.Write("PlayerDisconnected: " + e.StackTrace, nLog.Type.Error); }
            }, 0);
        }

        public static void openRules(Player player)
        {
            if (player.HasMyData("RULE_MARKER"))
            {
                Trigger.PlayerEvent(player, "deleteWorkBlip");
                player.ResetMyData("RULE_MARKER");
            }

            player.SetMyData("RULE_READ", true);

            Trigger.PlayerEvent(player, "setAutoscoolTable", "autorules");
            Log.Debug("Open Rules", nLog.Type.Info);
        }

        public static bool CanBuyLic(Player player, int index)
        {
            int paytype = player.GetMyData<int>("AUTO_SCHOOL_PAYTYPE");
            if (paytype == -1) return false;
            if (paytype == 0)
            {
                if (Main.Players[player].Money < Licenses[index].Price)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "У Вас недостаточно денег, чтобы купить эту лицензию", 3000);
                    return false;
                }
            }
            else if (paytype == 1)
            {
                if (Bank.Count(Main.Players[player].Bank) < Licenses[index].Price)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "У Вас недостаточно денег на карте, чтобы купить эту лицензию", 3000);
                    return false;
                }
            }
            return true;
        }

        public static void startDrivingCourse(Player player, int index)
        {
            if (player.HasMyData("IS_DRIVING")) {//("IS_DRIVING") очистить
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы должны закончить получения прав категории В", 3000);
                return;
            }
            //if (Main.Players[player].WorkID != (int)Jobs.JobTypes.None)
            //{
            //    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы не можете сделать это сейчас, вы устроены на работу", 3000);
            //    return;
            //}
            if (Main.Players[player].Licenses[index])
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У Вас уже есть эта лицензия", 3000);
                return;
            }


            int paytype = player.GetMyData<int>("AUTO_SCHOOL_PAYTYPE");
            if (index != 1 && !CanBuyLic(player, index))
                return;

            if (index != 1)
            {
                switch (paytype)
                {
                    case 0:
                        Wallet.Change(player, -Licenses[index].Price);
                        break;
                    case 1:
                        Bank.Change(Main.Players[player].Bank, -Licenses[index].Price);
                        break;
                }
            }

            var RandomPos = VehicleManager.GetFreePosition(startCourseCoord);
            if (RandomPos == -1)
            {
                RandomPos = 0;
            }

            if (index == 1)
            {
                player.Dimension = PrivateDimension;
                var vehicle = NAPI.Vehicle.CreateVehicle(VehicleHash.Dilettante, startCourseCoord[RandomPos], startCourseRot[RandomPos].Z, 30, 30);
                vehicle.Dimension = PrivateDimension;
                vehicle.NumberPlate = "SCHOOL";
                Trigger.ClientEvent(player, "createschoolBlip", startCourseCoord[RandomPos], PrivateDimension);
                Trigger.ClientEvent(player, "createschoolpoint", startCourseCoord[RandomPos], PrivateDimension);

                player.SetMyData("SCHOOLVEH", vehicle);
                vehicle.SetData("ACCESS", "SCHOOL");
                vehicle.SetData("DRIVER", player);
                player.SetMyData("IS_DRIVING", true);
                player.SetMyData("LICENSE", 1);
                Trigger.PlayerEvent(player, "busRoute", drivingCoords[0].X, drivingCoords[0].Y, drivingCoords[0].Z - 2f, drivingCoords[1].X, drivingCoords[1].Y, drivingCoords[1].Z - 2f);
                player.SetMyData("CHECK", 0);
                VehicleStreaming.SetEngineState(vehicle, false);
                //Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Чтобы завести транспорт, нажмите B", 3000);
                Trigger.ClientEvent(player, "CLIENT::keyBind:engine_notify");
                /*NAPI.Task.Run(() =>
                {
                    player.SetIntoVehicle(vehicle, 0);
                }, 200);*/

            }
            else if (index == 2)
            {
                player.Dimension = PrivateDimension;
                var vehicle = NAPI.Vehicle.CreateVehicle(VehicleHash.Mule, startCourseCoord[RandomPos], startCourseRot[RandomPos].Z, 30, 30);
                vehicle.Dimension = PrivateDimension;
                Trigger.ClientEvent(player, "createschoolBlip", startCourseCoord[RandomPos], PrivateDimension);
                Trigger.ClientEvent(player, "createschoolpoint", startCourseCoord[RandomPos], PrivateDimension);

                vehicle.NumberPlate = "SCHOOL";
                player.SetMyData("SCHOOLVEH", vehicle);
                vehicle.SetData("ACCESS", "SCHOOL");
                vehicle.SetData("DRIVER", player);
                player.SetMyData("IS_DRIVING", true);
                player.SetMyData("LICENSE", 2);
                Trigger.PlayerEvent(player, "busRoute", drivingCoords[0].X, drivingCoords[0].Y, drivingCoords[0].Z - 2f, drivingCoords[1].X, drivingCoords[1].Y, drivingCoords[1].Z - 2f);
                player.SetMyData("CHECK", 0);
                VehicleStreaming.SetEngineState(vehicle, false);
                //Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Чтобы завести транспорт, нажмите B", 3000);
                Trigger.ClientEvent(player, "CLIENT::keyBind:engine_notify");
                Trigger.PlayerEvent(player, "client_autoscoolTableClose");
                /*NAPI.Task.Run(() =>
                {
                    player.SetIntoVehicle(vehicle, 0);
                }, 200);*/
            }

            else if (index == 0)
            {
                player.Dimension = PrivateDimension;
                var vehicle = NAPI.Vehicle.CreateVehicle(VehicleHash.Bagger, startCourseCoord[RandomPos], startCourseRot[RandomPos].Z, 30, 30);
                vehicle.Dimension = PrivateDimension;
                vehicle.NumberPlate = "SCHOOL";

                Trigger.ClientEvent(player, "createschoolBlip", startCourseCoord[RandomPos], PrivateDimension);
                Trigger.ClientEvent(player, "createschoolpoint", startCourseCoord[RandomPos], PrivateDimension);

                player.SetMyData("SCHOOLVEH", vehicle);
                vehicle.SetData("ACCESS", "SCHOOL");
                vehicle.SetData("DRIVER", player);
                player.SetMyData("IS_DRIVING", true);
                player.SetMyData("LICENSE", 0);
                Trigger.PlayerEvent(player, "busRoute", drivingCoords[0].X, drivingCoords[0].Y, drivingCoords[0].Z - 2f, drivingCoords[1].X, drivingCoords[1].Y, drivingCoords[1].Z - 2f);
                player.SetMyData("CHECK", 0);
                VehicleStreaming.SetEngineState(vehicle, false);
                //Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Чтобы завести транспорт, нажмите B", 3000);
                Trigger.ClientEvent(player, "CLIENT::keyBind:engine_notify");
                Trigger.PlayerEvent(player, "client_autoscoolTableClose");
                /*NAPI.Task.Run(() =>
                {
                    player.SetIntoVehicle(vehicle, 0);
                }, 200);*/
            }

            else if (new List<int>() { 3, 4, 5 }.Contains(index))
            {
                Main.Players[player].Licenses[index] = true;

                client.Fractions.Utils.Stocks.fracStocks[(int)Fractions.Manager.FractionTypesEnum.Cityhall].Money += Licenses[index].Price;
                GameLog.Money($"player({Main.Players[player].UUID})", $"frac({(int)Fractions.Manager.FractionTypesEnum.Cityhall})", Licenses[index].Price, $"buyLic");
                FractionLogs.FractionMoney((int)Fractions.Manager.FractionTypesEnum.Cityhall, player.Name, Main.Players[player].UUID.ToString(), Licenses[index].Price, Licenses[index].Category);

                #region BPКвест: 68 Приобрести лицензию на водный транспорт. / 69 Приобрести лицензию на воздушный транспорт.

                #region BattlePass выполнение квеста
                if (index == 3) BattlePass.updateBPQuestIteration(player, BattlePass.BPQuestType.GetLicenseBoat);
                if (index == 4 || index == 5) BattlePass.updateBPQuestIteration(player, BattlePass.BPQuestType.GetLicenseAir);
                
                #endregion

                #endregion

                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"{Licenses[index].Type} успешно куплена", 3000);
            }

            player.ResetMyData("AUTO_SCHOOL_PAYTYPE");
        }
        private void onPlayerEnterSchool(ColShape shape, Player player)
        {
            try
            {
                player.SetMyData("INTERACTIONCHECK", 39);
                player.SetMyData("COLSHAPE_TYPE", "Autoschool");
                player.SetMyData("QUEST_COLSHAPE_INTERACTION", 39);

                if (player.HasMyData("SCHOOL_DRIVE"))
                    KeyLabel.Show(player, "E", "начать практическую часть");
                else
                    KeyLabel.Show(player, "E", "открыть меню");
            }
            catch (Exception e) { Log.Write("onPlayerEnterSchool: " + e.ToString(), nLog.Type.Error); }
        }
        private void onPlayerExitSchool(ColShape shape, Player player)
        {
            player.SetMyData("INTERACTIONCHECK", 0);
            player.ResetMyData("COLSHAPE_TYPE");
            KeyLabel.Hide(player);
        }

        [ServerEvent(Event.PlayerEnterVehicle)]
        public static void playerEnterSchoolVeh(Player player,Vehicle veh, int seatId)
        {
            if (!player.IsInVehicle || player.VehicleSeat != 0) return;
            if (!player.Vehicle.HasData("ACCESS") || player.Vehicle.GetData<string>("ACCESS") != "SCHOOL") return;
            if (!player.HasMyData("IS_DRIVING")) return;
            if (player.Vehicle != player.GetMyData<Vehicle>("SCHOOLVEH")) return;
            Trigger.ClientEvent(player, "deleteschoolBlip");
            Trigger.ClientEvent(player, "deleteschoolCheckpoint");
        }

        private void onPlayerEnterDrive(ColShape shape, Player player)
        {
            try
            {
                if (!player.IsInVehicle || player.VehicleSeat != 0) return;
                if (!player.Vehicle.HasData("ACCESS") || player.Vehicle.GetData<string>("ACCESS") != "SCHOOL") return;
                if (!player.HasMyData("IS_DRIVING")) return;
                if (player.Vehicle != player.GetMyData<Vehicle>("SCHOOLVEH")) return;
                if (shape.GetData<int>("NUMBER") != player.GetMyData<int>("CHECK")) return;
                //Trigger.PlayerEvent(player, "deleteCheckpoint", 12, 0);
                var check = player.GetMyData<int>("CHECK");
                if (check == drivingCoords.Count - 1)
                {
                    Trigger.PlayerEvent(player, "screenFadeOut", 1000);
                    player.ResetMyData("IS_DRIVING");
                    player.ResetMyData("SCHOOLVEH");
                    var vehHP = player.Vehicle.Health;

                    NAPI.Task.Run(() => {
                        try
                        {
                            //NAPI.Entity.DeleteEntity(player.Vehicle);
                            player.Vehicle.Delete();
                            NAPI.Entity.SetEntityPosition(player, new Vector3(-715.3574, -1296.043, 5.092063));
                            NAPI.Entity.SetEntityRotation(player, new Vector3(0.0, 0.0, 48.06606));
                        }
                        catch (Exception e){ Log.Write(e.StackTrace, nLog.Type.Error); }
                    }, 1200);

                    NAPI.Task.Run(() => {
                        NAPI.Entity.SetEntityDimension(player, 0);

                        Trigger.PlayerEvent(player, "screenFadeIn", 1000);
                        if (vehHP < 500)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы провалили экзамен", 3000);
                            return;
                        }

                        Main.Players[player].Licenses[player.GetMyData<int>("LICENSE")] = true;
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Вы успешно получили лицензию!", 3000);

                        //QUEST
                        //if (Main.Players[player].QuestType == QuestType.LicenceB)
                        //{
                        //    Main.Players[player].QuestData.ShowText(player);
                        //}

                        #region quest chapter iteration

                        if (player.GetMyData<int>("LICENSE") == 1)  {
                            QuestSystem.UpdatePlayerQuestIteration(player);
                            //player.ResetMyData("QUEST_COLSHAPE_INTERACTION"); // need test
                        }

                        #endregion

                        Trigger.PlayerEvent(player, "busUnRoute");
                    }, 2000);
                    return;
                }
                Trigger.PlayerEvent(player, "busUnRoute");
                player.SetMyData("CHECK", check + 1);
                if (check + 2 < drivingCoords.Count)
                    Trigger.PlayerEvent(player, "busRoute", drivingCoords[check + 1].X, drivingCoords[check + 1].Y, drivingCoords[check + 1].Z - 2f , drivingCoords[check + 2].X, drivingCoords[check + 2].Y, drivingCoords[check + 2].Z - 2f );
                else
                    Trigger.PlayerEvent(player, "busRoute", drivingCoords[check + 1].X, drivingCoords[check + 1].Y, drivingCoords[check + 1].Z - 2f , 0, 0, 0);
            }
            catch (Exception e)
            {
                Log.Write("ENTERDRIVE:\n" + e.ToString(), nLog.Type.Error);
            }
        }

        public static void Shuffle<T>(IList<T> list)
        {
            int n = list.Count;
            Random rand = new Random();
            while (n > 1)
            {
                n--;
                int k = rand.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

    }

    class Question
    {
        public string QuestionText;
        public List<string> Answers = new List<string>();
        public int TrueAnswer;

        public Question(string text, List<string> answers, int answer)
        {
            QuestionText = text;
            Answers = answers;
            TrueAnswer = answer;
        }
    }

    class Licenses
    {
        public string Type;
        public string Category;
        public int Price;

        public Licenses(string type, string category, int price)
        {
            Type = type;
            Category = category;
            Price = price;
        }
    }
}
