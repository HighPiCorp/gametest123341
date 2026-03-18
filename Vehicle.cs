    using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using NeptuneEvo.GUI;
using Newtonsoft.Json;
using System.Linq;
using NeptuneEvo.SDK;
using MySqlConnector;
using System.Globalization;
using NeptuneEvo.Fractions;
using NeptuneEvo.Houses;
using client.Fractions.Utils;
using client.Systems;
using client.GUI;
using NeptuneEvo.Rent;
using NeptuneEvo.Families;
using client.Families;

namespace NeptuneEvo.Core
{
    public class VehicleManager : Script
    {
        private static nLog Log = new nLog("Vehicle");
        private static Random Rnd = new Random();
        //private static Timer fuelTimer;
        private Vehicle SpawnVeh = null;
        //private Vehicle SpawnVeh = NAPI.Vehicle.CreateVehicle(VehicleHash.Dinghy, new Vector3(3370.183, 5186.575, 0.6195515), new Vector3(0.3827768, 2.631065, 261.6981), 1, 1);

        #region Массивы
        public static SortedDictionary<int, VehicleData> Vehicles = new SortedDictionary<int, VehicleData>();

        public static SortedDictionary<int, int> Vehicletrunk = new SortedDictionary<int, int>()
        {
            { -1, 100 },
            { 0, 120 }, // compacts
            { 1, 150 }, // Sedans
            { 2, 200 }, // SUVs
            { 3, 100 }, // Coupes
            { 4, 130 }, // Muscle
            { 5, 150 }, // Sports
            { 6, 170 }, // Sports (classic?)
            { 7, 150 }, // Super
            { 8, 1 }, // Motorcycles
            { 9, 200 }, // Off-Road
            { 10, 150 }, // Industrial
            { 11, 150 }, // Utility
            { 12, 150 }, // Vans
            { 13, 1   }, // cycles
            { 14, 300 }, // Boats
            { 15, 1 }, // Helicopters
            { 16, 1 }, // Planes
            { 17, 130 }, // Service
            { 18, 200 }, // Emergency
            { 19, 150 }, // Military
            { 20, 150 }, // Commercial
            // 21 trains
        };
        public static SortedDictionary<int, int> VehicleTank = new SortedDictionary<int, int>()
        {
            { -1, 100 },
            { 0, 120 }, // compacts
            { 1, 150 }, // Sedans
            { 2, 200 }, // SUVs
            { 3, 100 }, // Coupes
            { 4, 130 }, // Muscle
            { 5, 150 }, // Sports
            { 6, 100 }, // Sports (classic?)
            { 7, 150 }, // Super
            { 8, 100 }, // Motorcycles
            { 9, 200 }, // Off-Road
            { 10, 150 }, // Industrial
            { 11, 150 }, // Utility
            { 12, 150 }, // Vans
            { 13, 1   }, // cycles
            { 14, 300 }, // Boats
            { 15, 400 }, // Helicopters
            { 16, 500 }, // Planes
            { 17, 130 }, // Service
            { 18, 200 }, // Emergency
            { 19, 150 }, // Military
            { 20, 150 }, // Commercial
            // 21 trains
        };
        public static SortedDictionary<int, int> VehicleRepairPrice = new SortedDictionary<int, int>()
        {
            { -1, 100 }, // compacts
            { 0, 100 }, // compacts
            { 1, 100 }, // Sedans
            { 2, 100 }, // SUVs
            { 3, 100 }, // Coupes
            { 4, 100 }, // Muscle
            { 5, 100 }, // Sports
            { 6, 100 }, // Sports (classic?)
            { 7, 100 }, // Super
            { 8, 100 }, // Motorcycles
            { 9, 100 }, // Off-Road
            { 10, 100 }, // Industrial
            { 11, 100 }, // Utility
            { 12, 100 }, // Vans
            { 13, 100 }, // 13 cycles
            { 14, 100 }, // Boats
            { 15, 100 }, // Helicopters
            { 16, 100 }, // Planes
            { 17, 100 }, // Service
            { 18, 100 }, // Emergency
            { 19, 100 }, // Military
            { 20, 100 }, // Commercial
            // 21 trains
        };

        public static List<int> VehicleCars = new List<int>()
        {
            -1,
            0,
             1,
             2,
             3,
             4,
             5,
             6,
             7,
             9,
             10,
             11,
             12,
             18,
             19,
             20,
        };

        public static List<int> VehicleMoto = new List<int>()
        {
            8,
        };

        public static List<int> VehicleHelicopter = new List<int>()
        {
            15,
        };

        public static List<int> VehicleBoats = new List<int>()
        {
            14,
        };

        private static SortedDictionary<int, int> PetrolRate = new SortedDictionary<int, int>()
        {
            { -1, 0 },
            { 0, 1 }, // compacts
            { 1, 1 }, // Sedans
            { 2, 1 }, // SUVs
            { 3, 1 }, // Coupes
            { 4, 1 }, // Muscle
            { 5, 1 }, // Sports
            { 6, 1 }, // Sports (classic?)
            { 7, 1 }, // Super
            { 8, 1 }, // Motorcycles
            { 9, 1 }, // Off-Road
            { 10, 1 }, // Industrial
            { 11, 1 }, // Utility
            { 12, 1 }, // Vans
            { 13, 0 }, // Cycles
            { 14, 1 }, // Boats
            { 15, 1 }, // Helicopters
            { 16, 1 }, // Planes
            { 17, 1 }, // Service
            { 18, 1 }, // Emergency
            { 19, 1 }, // Military
            { 20, 1 }, // Commercial
            // 21 trains
        };

        public static Dictionary<VehicleHash, float> TrunkCoords = new Dictionary<VehicleHash, float>()
        {
            { VehicleHash.Barracks, 5f },
            { VehicleHash.Youga, 3f },
            { VehicleHash.Youga2, 3f },
            { VehicleHash.Burrito3, 3f },
            { VehicleHash.Gburrito, 3f },
            { VehicleHash.Ambulance, 4.7f }
        };

        public static Dictionary<int, string> ColorNames = new Dictionary<int, string>()
        {
            { 0, "Черный" },
            { 1, "Графит" },
            { 2, "Черная сталь" },
            { 3, "Темная сталь" },
            { 4, "Серебро" },
            { 5, "Синеватое серебро" },
            { 6, "Катаная сталь" },
            { 7, "Темное серебро" },
            { 8, "Серебристо-каменный" },
            { 9, "Полуночное серебро" },
            { 10, "Серебристо-литой" },
            { 11, "Антрацит" },
            { 12, "Черный" },
            { 13, "Серый" },
            { 14, "Светло-серый" },
            { 27, "Красный" },
            { 28, "Красный Torino" },
            { 29, "Красный Formula" },
            { 30, "Огненно-красный" },
            { 31, "Красный Grace" },
            { 32, "Гранатовый" },
            { 33, "Закатно-красный" },
            { 34, "Каберне" },
            { 35, "Яблочно-красный" },
            { 36, "Рассветно-оранжевый" },
            { 38, "Оранжевый" },
            { 39, "Красный" },
            { 40, "Темно-красный" },
            { 41, "Оранжевый" },
            { 42, "Желтый" },
            { 49, "Темно-зеленый" },
            { 50, "Гоночный зеленый" },
            { 51, "Морской зеленый" },
            { 52, "Оливково-зеленый" },
            { 53, "Ярко-зеленый" },
            { 54, "Зеленый" },
            { 55, "Лаймово-зеленый" },
            { 61, "Космический синий" },
            { 62, "Темно-синий" },
            { 63, "Саксонский синий" },
            { 64, "Синий" },
            { 65, "Синий Mariner" },
            { 66, "Синий Harbour" },
            { 67, "Бриллиантовый синий" },
            { 68, "Синий Surf" },
            { 69, "Морской синий" },
            { 70, "Ультра-синий" },
            { 71, "Пурпурный Schafter" },
            { 72, "Пурпурный Spinnaker" },
            { 73, "Гоночный синий" },
            { 74, "Светло-синий" },
            { 82, "Темно-синий" },
            { 83, "Синий" },
            { 84, "Полуночный синий" },
            { 88, "Желтый" },
            { 89, "Гоночный желтый" },
            { 90, "Бронзовый" },
            { 91, "Желтый Dew" },
            { 92, "Лаймово-зеленый" },
            { 94, "Коричневый Feltzer" },
            { 95, "Коричневый Creek" },
            { 96, "Шоколадно-коричневый" },
            { 97, "Кленовый" },
            { 98, "Светло-коричневый" },
            { 99, "Золотой" },
            { 100, "Хаки" },
            { 101, "Коричневый Bison" },
            { 102, "Светло-бежевый" },
            { 103, "Буковый" },
            { 104, "Охра" },
            { 105, "Песочный" },
            { 106, "Выцветший коричневый" },
            { 107, "Кремовый" },
            { 111, "Ледяной белый" },
            { 112, "Белый иней" },
            { 117, "Сталь" },
            { 118, "Темная сталь" },
            { 119, "Алюминий" },
            { 120, "Хромированный" },
            { 128, "Зеленый" },
            { 131, "Ледяной белый" },
            { 135, "Ярко-розовый" },
            { 136, "Лососево-розовый" },
            { 137, "Розовый Pfister" },
            { 138, "Ярко-оранжевый" },
            { 141, "Полуночный синий" },
            { 142, "Полуночно-фиолетовый" },
            { 143, "Винно-красный" },
            { 145, "Ярко-пурпурный" },
            { 147, "Карбоновый черный" },
            { 148, "Пурпурный Schafter" },
            { 149, "Полуночно-фиолетовый" },
            { 150, "Лилово-красный" },
            { 151, "Лесной зеленый" },
            { 152, "Оливковый" },
            { 153, "Бежевый" },
            { 154, "undefined" },
            { 155, "Зеленая листва" },
            { 158, "Чистое золото" },
            { 159, "Начищенное золото" },
        };

        private static List<ColorName> ColorList = new List<ColorName>()
        {
            new ColorName("AliceBlue", 0xF0, 0xF8, 0xFF),
            new ColorName("AntiqueWhite", 0xFA, 0xEB, 0xD7),
            new ColorName("Aqua", 0x00, 0xFF, 0xFF),
            new ColorName("Aquamarine", 0x7F, 0xFF, 0xD4),
            new ColorName("Azure", 0xF0, 0xFF, 0xFF),
            new ColorName("Beige", 0xF5, 0xF5, 0xDC),
            new ColorName("Bisque", 0xFF, 0xE4, 0xC4),
            new ColorName("Black", 0x00, 0x00, 0x00),
            new ColorName("BlanchedAlmond", 0xFF, 0xEB, 0xCD),
            new ColorName("Blue", 0x00, 0x00, 0xFF),
            new ColorName("BlueViolet", 0x8A, 0x2B, 0xE2),
            new ColorName("Brown", 0xA5, 0x2A, 0x2A),
            new ColorName("BurlyWood", 0xDE, 0xB8, 0x87),
            new ColorName("CadetBlue", 0x5F, 0x9E, 0xA0),
            new ColorName("Chartreuse", 0x7F, 0xFF, 0x00),
            new ColorName("Chocolate", 0xD2, 0x69, 0x1E),
            new ColorName("Coral", 0xFF, 0x7F, 0x50),
            new ColorName("CornflowerBlue", 0x64, 0x95, 0xED),
            new ColorName("Cornsilk", 0xFF, 0xF8, 0xDC),
            new ColorName("Crimson", 0xDC, 0x14, 0x3C),
            new ColorName("Cyan", 0x00, 0xFF, 0xFF),
            new ColorName("DarkBlue", 0x00, 0x00, 0x8B),
            new ColorName("DarkCyan", 0x00, 0x8B, 0x8B),
            new ColorName("DarkGoldenRod", 0xB8, 0x86, 0x0B),
            new ColorName("DarkGray", 0xA9, 0xA9, 0xA9),
            new ColorName("DarkGreen", 0x00, 0x64, 0x00),
            new ColorName("DarkKhaki", 0xBD, 0xB7, 0x6B),
            new ColorName("DarkMagenta", 0x8B, 0x00, 0x8B),
            new ColorName("DarkOliveGreen", 0x55, 0x6B, 0x2F),
            new ColorName("DarkOrange", 0xFF, 0x8C, 0x00),
            new ColorName("DarkOrchid", 0x99, 0x32, 0xCC),
            new ColorName("DarkRed", 0x8B, 0x00, 0x00),
            new ColorName("DarkSalmon", 0xE9, 0x96, 0x7A),
            new ColorName("DarkSeaGreen", 0x8F, 0xBC, 0x8F),
            new ColorName("DarkSlateBlue", 0x48, 0x3D, 0x8B),
            new ColorName("DarkSlateGray", 0x2F, 0x4F, 0x4F),
            new ColorName("DarkTurquoise", 0x00, 0xCE, 0xD1),
            new ColorName("DarkViolet", 0x94, 0x00, 0xD3),
            new ColorName("DeepPink", 0xFF, 0x14, 0x93),
            new ColorName("DeepSkyBlue", 0x00, 0xBF, 0xFF),
            new ColorName("DimGray", 0x69, 0x69, 0x69),
            new ColorName("DodgerBlue", 0x1E, 0x90, 0xFF),
            new ColorName("FireBrick", 0xB2, 0x22, 0x22),
            new ColorName("FloralWhite", 0xFF, 0xFA, 0xF0),
            new ColorName("ForestGreen", 0x22, 0x8B, 0x22),
            new ColorName("Fuchsia", 0xFF, 0x00, 0xFF),
            new ColorName("Gainsboro", 0xDC, 0xDC, 0xDC),
            new ColorName("GhostWhite", 0xF8, 0xF8, 0xFF),
            new ColorName("Gold", 0xFF, 0xD7, 0x00),
            new ColorName("GoldenRod", 0xDA, 0xA5, 0x20),
            new ColorName("Gray", 0x80, 0x80, 0x80),
            new ColorName("Green", 0x00, 0x80, 0x00),
            new ColorName("GreenYellow", 0xAD, 0xFF, 0x2F),
            new ColorName("HoneyDew", 0xF0, 0xFF, 0xF0),
            new ColorName("HotPink", 0xFF, 0x69, 0xB4),
            new ColorName("IndianRed", 0xCD, 0x5C, 0x5C),
            new ColorName("Indigo", 0x4B, 0x00, 0x82),
            new ColorName("Ivory", 0xFF, 0xFF, 0xF0),
            new ColorName("Khaki", 0xF0, 0xE6, 0x8C),
            new ColorName("Lavender", 0xE6, 0xE6, 0xFA),
            new ColorName("LavenderBlush", 0xFF, 0xF0, 0xF5),
            new ColorName("LawnGreen", 0x7C, 0xFC, 0x00),
            new ColorName("LemonChiffon", 0xFF, 0xFA, 0xCD),
            new ColorName("LightBlue", 0xAD, 0xD8, 0xE6),
            new ColorName("LightCoral", 0xF0, 0x80, 0x80),
            new ColorName("LightCyan", 0xE0, 0xFF, 0xFF),
            new ColorName("LightGoldenRodYellow", 0xFA, 0xFA, 0xD2),
            new ColorName("LightGray", 0xD3, 0xD3, 0xD3),
            new ColorName("LightGreen", 0x90, 0xEE, 0x90),
            new ColorName("LightPink", 0xFF, 0xB6, 0xC1),
            new ColorName("LightSalmon", 0xFF, 0xA0, 0x7A),
            new ColorName("LightSeaGreen", 0x20, 0xB2, 0xAA),
            new ColorName("LightSkyBlue", 0x87, 0xCE, 0xFA),
            new ColorName("LightSlateGray", 0x77, 0x88, 0x99),
            new ColorName("LightSteelBlue", 0xB0, 0xC4, 0xDE),
            new ColorName("LightYellow", 0xFF, 0xFF, 0xE0),
            new ColorName("Lime", 0x00, 0xFF, 0x00),
            new ColorName("LimeGreen", 0x32, 0xCD, 0x32),
            new ColorName("Linen", 0xFA, 0xF0, 0xE6),
            new ColorName("Magenta", 0xFF, 0x00, 0xFF),
            new ColorName("Maroon", 0x80, 0x00, 0x00),
            new ColorName("MediumAquaMarine", 0x66, 0xCD, 0xAA),
            new ColorName("MediumBlue", 0x00, 0x00, 0xCD),
            new ColorName("MediumOrchid", 0xBA, 0x55, 0xD3),
            new ColorName("MediumPurple", 0x93, 0x70, 0xDB),
            new ColorName("MediumSeaGreen", 0x3C, 0xB3, 0x71),
            new ColorName("MediumSlateBlue", 0x7B, 0x68, 0xEE),
            new ColorName("MediumSpringGreen", 0x00, 0xFA, 0x9A),
            new ColorName("MediumTurquoise", 0x48, 0xD1, 0xCC),
            new ColorName("MediumVioletRed", 0xC7, 0x15, 0x85),
            new ColorName("MidnightBlue", 0x19, 0x19, 0x70),
            new ColorName("MintCream", 0xF5, 0xFF, 0xFA),
            new ColorName("MistyRose", 0xFF, 0xE4, 0xE1),
            new ColorName("Moccasin", 0xFF, 0xE4, 0xB5),
            new ColorName("NavajoWhite", 0xFF, 0xDE, 0xAD),
            new ColorName("Navy", 0x00, 0x00, 0x80),
            new ColorName("OldLace", 0xFD, 0xF5, 0xE6),
            new ColorName("Olive", 0x80, 0x80, 0x00),
            new ColorName("OliveDrab", 0x6B, 0x8E, 0x23),
            new ColorName("Orange", 0xFF, 0xA5, 0x00),
            new ColorName("OrangeRed", 0xFF, 0x45, 0x00),
            new ColorName("Orchid", 0xDA, 0x70, 0xD6),
            new ColorName("PaleGoldenRod", 0xEE, 0xE8, 0xAA),
            new ColorName("PaleGreen", 0x98, 0xFB, 0x98),
            new ColorName("PaleTurquoise", 0xAF, 0xEE, 0xEE),
            new ColorName("PaleVioletRed", 0xDB, 0x70, 0x93),
            new ColorName("PapayaWhip", 0xFF, 0xEF, 0xD5),
            new ColorName("PeachPuff", 0xFF, 0xDA, 0xB9),
            new ColorName("Peru", 0xCD, 0x85, 0x3F),
            new ColorName("Pink", 0xFF, 0xC0, 0xCB),
            new ColorName("Plum", 0xDD, 0xA0, 0xDD),
            new ColorName("PowderBlue", 0xB0, 0xE0, 0xE6),
            new ColorName("Purple", 0x80, 0x00, 0x80),
            new ColorName("Red", 0xFF, 0x00, 0x00),
            new ColorName("RosyBrown", 0xBC, 0x8F, 0x8F),
            new ColorName("RoyalBlue", 0x41, 0x69, 0xE1),
            new ColorName("SaddleBrown", 0x8B, 0x45, 0x13),
            new ColorName("Salmon", 0xFA, 0x80, 0x72),
            new ColorName("SandyBrown", 0xF4, 0xA4, 0x60),
            new ColorName("SeaGreen", 0x2E, 0x8B, 0x57),
            new ColorName("SeaShell", 0xFF, 0xF5, 0xEE),
            new ColorName("Sienna", 0xA0, 0x52, 0x2D),
            new ColorName("Silver", 0xC0, 0xC0, 0xC0),
            new ColorName("SkyBlue", 0x87, 0xCE, 0xEB),
            new ColorName("SlateBlue", 0x6A, 0x5A, 0xCD),
            new ColorName("SlateGray", 0x70, 0x80, 0x90),
            new ColorName("Snow", 0xFF, 0xFA, 0xFA),
            new ColorName("SpringGreen", 0x00, 0xFF, 0x7F),
            new ColorName("SteelBlue", 0x46, 0x82, 0xB4),
            new ColorName("Tan", 0xD2, 0xB4, 0x8C),
            new ColorName("Teal", 0x00, 0x80, 0x80),
            new ColorName("Thistle", 0xD8, 0xBF, 0xD8),
            new ColorName("Tomato", 0xFF, 0x63, 0x47),
            new ColorName("Turquoise", 0x40, 0xE0, 0xD0),
            new ColorName("Violet", 0xEE, 0x82, 0xEE),
            new ColorName("Wheat", 0xF5, 0xDE, 0xB3),
            new ColorName("White", 0xFF, 0xFF, 0xFF),
            new ColorName("WhiteSmoke", 0xF5, 0xF5, 0xF5),
            new ColorName("Yellow", 0xFF, 0xFF, 0x00),
            new ColorName("YellowGreen", 0x9A, 0xCD, 0x32),
        };

        public static Dictionary<string, int> VehicleWeight = new Dictionary<string, int>()
        {
            //standart
            {"asterope", 20},
            {"issi2", 40},
            {"seminole", 80},
            {"virgo", 45},
            {"bjxl", 80},
            {"savestra", 30},
            {"sentinel2", 50},
            {"retinue", 30},
            {"sabregt", 45},
            {"sultan", 25},
            {"granger", 140},
            {"jackal", 40},
            {"sabregt2", 45},
            {"cavalcade2", 80},
            {"fusilade", 40},
            {"previon", 35},
            {"sentinel3", 25},
            {"tailgater", 40},
            {"warrener", 35},
            {"remus", 40},
            {"surge", 60},
            {"tulip", 40},
            {"baller", 75},
            {"fq2", 75},
            {"nightshade", 15},
            {"schwarzer", 30},
            {"gresley", 100},
            {"oracle2", 35},
            {"serrano", 80},
            {"brawler", 40},
            {"felon2", 20},
            {"sultan2", 40},
            {"vigero", 30},
            {"sultan3", 40},
            {"clique", 40},
            {"freecrawler", 80},
            {"ruiner", 50},
            {"baller2", 100},
            {"baller3", 100},
            {"calico", 30},
            {"exemplar", 10},
            {"rocoto", 80},
            {"zr350", 55},
            {"gauntlet4", 40},
            {"penumbra", 60},
            {"buffalo2", 35},
            {"novak", 50},
            {"imperator", 50},
            {"tropos", 5},
            {"schafter2", 50},
            {"penumbra2", 65},
            {"cogcabrio", 25},
            {"hermes", 70},
            {"rt3000", 25},
            {"zion3", 30},
            {"dominator", 20},
            {"elegy", 35},
            {"hustler", 65},
            {"komoda", 30},
            {"massacro", 25},
            {"cog55", 55},
            {"kuruma", 25},
            {"xls", 80},
            {"elegy2", 35},
            {"euros", 40},
            {"yosemite", 160},
            {"rapidgt3", 20},
            {"viseris", 15},
            {"gauntlet5", 30},
            {"windsor", 35},


            //premium
            {"baller4", 90},
            {"sc1", 15},
            {"specter", 10},
            {"alpha", 45},
            {"tailgater2", 40},
            {"cognoscenti", 45},
            {"revolter", 35},
            {"sugoi", 35},
            {"seven70", 15},
            {"dominator3", 20},
            {"casco", 40},
            {"hotknife", 50},
            {"rapidgt", 15},
            {"schafter4", 50},
            {"voltic", 10},
            {"zion2", 25},
            {"cypher", 35},
            {"khamelion", 40},
            {"surano", 25},
            {"drafter", 60},
            {"jester3", 35},
            {"lynx", 25},
            {"rapidgt2", 25},
            {"schafter3", 40},
            {"dominator7", 40},
            {"imorgon", 20},
            {"feltzer2", 40},
            {"jester4", 20},
            {"vstr", 35},
            {"ninef2", 15},
            {"pariah", 40},
            {"rebla", 80},
            {"dubsta", 100},
            {"dubsta2", 100},
            {"jugular", 40},
            {"neon", 25},
            {"streiter", 90},
            {"vectre", 45},
            {"huntley", 80},
            {"paragon", 35},
            {"patriot2", 70},
            {"verlierer2", 20},
            {"turismo2", 35},
            {"vacca", 30},
            {"windsor2", 15},
            {"schlagen", 35},
            {"stretch", 75},
            {"raiden", 65},
            {"italigto", 20},
            {"neo", 25},
            {"superd", 50},
            {"toros", 40},
            {"jb7002", 30},
            {"stinger", 45},
            {"swinger", 20},
            {"tempesta", 25},
            {"thrax", 5},
            {"torero", 15},
            {"btype", 15},
            {"dubsta3", 110},
            {"btype3", 15},
            {"btype2", 15},
            {"stafford", 40},





            //эконом
            {"tornado4", 35},
            {"voodoo2", 35},
            {"tornado3", 35},
            {"emperor2", 40},
            {"brioso2", 10},
            {"issi3", 15},
            {"surfer2", 160},
            {"regina", 85},
            {"voodoo", 35},
            {"ingot", 40},
            {"fagaloa", 65},
            {"asbo", 25},
            {"emperor", 40},
            {"tornado", 35},
            {"asea", 35},
            {"dynasty", 45},
            {"blade", 35},
            {"chino", 40},
            {"surfer", 160},
            {"tornado2", 35},
            {"blista", 25},
            {"intruder", 20},
            {"manana", 40},
            {"tornado5", 35},
            {"blista2", 30},
            {"brioso", 20},
            {"glendale", 25},
            {"blista3", 30},
            {"panto", 10},
            {"slamvan2", 25},
            {"weevil", 10},
            {"chino2", 40},
            {"manana2", 40},
            {"club", 20},
            {"glendale2", 25},
            {"seminole2", 50},
            {"rancherxl", 110},
            {"tampa", 35},
            {"dilettante", 25},
            {"nebula", 40},
            {"prairie", 45},
            {"premier", 30},
            {"stalion", 25},
            {"primo", 35},
            {"rebel", 130},
            {"rhapsody", 30},
            {"ellie", 30},
            {"stanier", 45},
            {"buccaneer", 30},
            {"dukes", 25},
            {"faction", 45},
            {"virgo2", 45},
            {"faction2", 20},
            {"buccaneer2", 15},
            {"stratum", 85},
            {"washington", 35},
            {"bfinjection", 15},
            {"futo", 30},
            {"michelli", 35},
            {"picador", 130},
            {"buffalo", 25},
            {"kanjo", 35},
            {"vamos", 35},
            {"futo2", 20},
            {"peyote", 50},
            {"minivan", 100},
            {"radi", 45},
            {"bifta", 15},
            {"oracle", 25},
            {"phoenix", 15},
            {"gauntlet", 30},
            {"peyote3", 40},
            {"cheburek", 30},
            {"mesa", 60},
            {"gauntlet3", 35},
            {"cavalcade", 80},
            {"landstalker", 75},
            {"habanero", 60},
            {"impaler", 20},
            {"fugitive", 40},
            {"hellion", 80},
            {"felon", 40},
            {"patriot", 90},
            {"landstalker2", 100},
            {"retinue2", 25},
            {"pigalle", 50},





            //moto
            {"faggio3", 5},
            {"faggio2", 5},
            {"faggio", 5},
            {"esskey", 0},
            {"bagger", 15},
            {"enduro", 0},
            {"daemon2", 0},
            {"wolfsbane", 0},
            {"pcj", 0},
            {"sanchez2", 0},
            {"daemon", 0},
            {"manchez", 0},
            {"vader", 0},
            {"blazer", 0},
            {"diablous2", 0},
            {"diablous", 0},
            {"avarus", 0},
            {"hexer", 0},
            {"sovereign", 15},
            {"blazer4", 0},
            {"fcr", 0},
            {"fcr2", 0},
            {"ruffian", 0},
            {"zombiea", 0},
            {"zombieb", 0},
            {"blazer3", 5},
            {"innovation", 0},
            {"nemesis", 0},
            {"deathbike", 0},
            {"deathbike3", 0},
            {"thrust", 0},
            {"verus", 10},
            {"vortex", 0},
            {"bati", 0},
            {"sanctus", 0},
            {"defiler", 0},
            {"gargoyle", 0},
            {"stryder", 0},
            {"double", 0},
            {"akuma", 0},
            {"carbonrs", 0},
            {"nightblade", 0},
            {"bf400", 0},
            {"hakuchou", 0},
            {"cliffhanger", 0},
            {"hakuchou2", 0},
            {"deathbike2", 0},
            {"manchez2", 0},
            {"shotaro", 0},


            //sport
            {"deviant", 30},
            {"issi7", 55},
            {"banshee", 15},
            {"omnis", 5},
            {"f620", 25},
            {"furoregt", 5},
            {"banshee2", 5},
            {"comet3", 35},
            {"coquette", 5},
            {"ruston", 5},
            {"bullet", 5},
            {"monroe", 20},
            {"comet4", 15},
            {"ninef", 25},
            {"locust", 5},
            {"comet2", 35},
            {"infernus", 5},
            {"jester", 5},
            {"carbonizzare", 20},
            {"gt500", 35},
            {"stingergt", 5},
            {"mamba", 40},
            {"bestiagts", 20},
            {"comet5", 35},
            {"reaper", 35},
            {"gp1", 10},
            {"coquette2", 40},
            {"penetrator", 15},
            {"adder", 30},
            {"entityxf", 10},
            {"infernus2", 15},
            {"osiris", 5},
            {"comet6", 20},
            {"coquette3", 40},
            {"pfister811", 5},
            {"cheetah", 5},
            {"entity2", 5},
            {"fmj", 5},
            {"cyclone", 15},
            {"italigtb", 25},
            {"t20", 5},
            {"growler", 40},
            {"cheetah2", 30},
            {"tigon", 5},
            {"emerus", 5},
            {"krieger", 5},
            {"feltzer3", 50},
            {"coquette4", 50},
            {"italirsx", 20},
            {"turismor", 5},
            {"nero", 25},
            {"tyrant", 20},
            {"furia", 20},
            {"tyrus", 5},
            {"xa21", 20},
            {"taipan", 5},
            {"autarch", 5},
            {"deveste", 10},
            {"visione", 5},
            {"zorrusso", 5},
            {"prototipo", 5},
            {"vagner", 5},
            {"zentorno", 5},
            {"tezeract", 5},
            {"ztype", 10},



            //грузовые
            {"bison", 170},
            {"bobcatxl", 165},
            {"bodhi2", 130},
            {"brutus", 170},
            {"burrito3", 250},
            {"caracara2", 135},
            {"contender", 150},
            {"dloader", 130},
            {"everon", 120},
            {"gburrito2", 250},
            {"kamacho", 115},
            {"moonbeam", 145},
            {"ratloader2", 80},
            {"rebel2", 135},
            {"riata", 105},
            {"sandking", 220},
            {"sandking2", 210},
            {"slamvan", 100},
            {"slamvan3", 115},
            {"speedo", 230},
            {"speedo4", 230},
            {"yosemite3", 105},
            {"youga2", 150},


            //Auto Donate
            {"apriora", 60},
            {"ae86", 50},
            {"w210", 80},
            {"benzc32", 80},
            {"s600", 100},
            {"bmwe38", 90},
            {"mark2", 100},
            {"gcmsentra20", 70},
            {"audia8", 110},
            {"rmodm5e34", 80},
            {"octavia18", 110},
            {"m3e46", 60},
            {"optima", 90},
            {"lancer", 70},
            {"m3e30", 60},
            {"lrdef17", 170},
            {"bnr32", 70},
            {"supra", 50},
            {"golf7r", 60},
            {"190e", 60},
            {"chevelle1970", 90},
            {"m5e60", 90},
            {"370z16", 50},
            {"kiastinger", 40},
            {"audia6", 70},
            {"boss302", 90},
            {"gl450", 150},
            {"camry18", 70},
            {"bmwg20", 50},
            {"rmodcharger69", 90},
            {"bmwm4", 60},
            {"bnr34", 70},
            {"evoque", 100},
            {"z48", 50},
            {"benzsl63", 50},
            {"volvoxc90", 140},
            {"glc2021", 110},
            {"lc200", 130},
            {"c63coupe", 60},
            {"m5", 100},
            {"durango18", 130},
            {"teslas", 70},
            {"rmodrs6", 100},
            {"mbgls63", 150},
            {"v447", 300},
            {"g65fresh", 170},
            {"bmwx7", 160},
            {"modena99", 40},
            {"rmodbmwi8", 50},
            {"rs72", 70},
            {"gtr", 70},
            {"porsche2021", 70},
            {"rmodm8c", 60},
            {"modelx", 150},
            {"dbx", 140},
            {"rmodlp570", 40},
            {"rmodbentleygt", 70},
            {"taycan", 60},
            {"bc5506d", 400},
            {"gls600", 160},
            {"600lt", 30},
            {"f458", 40},
            {"hevos", 40},
            {"488pista", 40},
            {"urus", 150},
            {"g63amg6x6", 220},
            {"f812", 40},
            {"p1", 30},
            {"lp700", 30},
            {"rculi", 180},
            {"f12berlinetta", 40},
            {"bugatti", 40},
            {"chiron17", 40},



            //Тюнинг салон
            {"rmodbiposto", 20},
            {"rmodm3e36", 70},
            {"svt00", 80},
            {"rmod240sx", 60},
            {"rmodz350pandem", 50},
            {"rmodsupra", 70},
            {"cam8tun", 70},
            {"rmodfordgt", 70},
            {"rmodsuprapandem", 50},
            {"m4comp", 70},
            {"rmodamgc63", 60},
            {"rmodm4gts", 60},
            {"rmodrover", 130},
            {"rmodmustang", 80},
            {"rmodjeep", 150},
            {"mcls53", 70},
            {"rmodgt63", 80},
            {"m5f90", 100},
            {"rmodx6", 140},
            {"rmodmi8lb", 50},
            {"rmodskyline", 80},
            {"pgt2", 50},
            {"hevo", 40},
            {"mbbs20", 50},
            {"rmodlp750", 40},


            //Сторонние тачки
            {"rs3", 60},
            {"x5om", 140},
            {"exige12", 50},
            {"bmwm7", 90},
            {"mere63amg", 80},
            {"explorer", 140},
            {"w223", 90},
            {"ocnetrongt", 40},
            {"gle63", 140},
            {"cruzzz", 170},
            {"aetron", 100},
            {"rmodrs7", 70},
            {"rmodm4", 60},
            {"rmodmi8", 50},
            {"63amg", 170},
            {"rmodi8mlb", 50},
            {"rmodi8ks", 40},
            {"fenyr", 10},
            {"roma", 50},
            {"rmodg65", 170},
            {"rmodgtr", 80},
            {"huracan", 40},
            {"f8t", 30},
            {"f40", 10},
            {"divo", 1},
            {"gt17", 40},
            {"r820", 1},
            {"rs7c8", 1},
            {"sti", 70},
            {"panamera17turbo", 90},
            {"subwrx", 1},
            {"stalion2", 40},
            {"buffalo3", 60},
            {"rrocket", 5},
            {"sultanrs", 45},

            {"al18", 300},
            {"stradale18", 40},
            {"ocnlamtmc", 20},
            {"amg1", 20},
            {"avtr", 20},
            {"huragucci", 20},
            {"mercedessl63", 20},
            {"sennasmso", 20},
            {"slr", 20},


            {"pwagon51",910 },
            {"2cv6naj",40 },
            {"vwbeetlenaj",45 },
            {"renault4",45 },
            {"simca1100",40 },
            {"forfalcxr",45 },
            {"impala72",50 },
            {"belair56",40 },
            {"zephyr41c",35 },
            {"ford34",30 },
            {"fleetline48",40 },
            {"celestem5",50 },
            {"ff1375",20 },
            {"jarama",35 },
            {"cu2", 60},
            {"dwrangler", 70},
            {"jeep392", 70},
            {"jimmy", 55},
            {"mitgalant92", 35},
            {"s15rb", 30},
            {"vwcaddy", 150},
            //exclusive
            {"jesko", 40},
            {"agera11", 30},
            {"rmodatlantic", 60},
            {"rmodbacalar", 80},
            {"rmodbugatti", 20},
            {"rmodchiron300", 20},
            {"rmodessenza", 30},
            {"sian", 20},
            {"tesla", 20},
            {"countach", 30},
            {"bugatticentodieci", 40},
            {"eleanor", 70},
            {"forgt50020", 80},
            {"rolls08", 100},
            {"fxxkevo", 30},
            {"m1procar", 60},
            {"rmodlp770", 40},
            {"rmodpagani", 40},
            {"rmodveneno", 40},

            {"500gtrlam", 20},
            {"monowheel", 5},
            {"monza", 25},
            {"morgan", 30},
            {"nc1", 15},
            {"noire19wb", 15},
            {"singer", 40},
            {"twizy", 10},
            {"vclass21", 200},
            {"x6mf16", 75},

            {"youga", 200},
            {"cog552", 80},
            {"mustangpolice", 90},
            {"policet", 250},
            {"policeb", 10},
            {"police", 60},
            {"police3", 70},
            {"riot", 300},
            {"ambulance", 150},
            {"fbi", 70},
            {"fbi2", 120},
            {"police4", 60},
            {"xls2", 100},
            {"riot2", 350},
            {"insurgent2", 150},
            {"barracks", 100},
            {"brickade", 250},
            {"rumpo", 200},
            {"rmodpolice", 60},
            {"sheriff2", 120},
            {"sheriff", 60},

            //новыq автосолон
            {"4runner", 80},
            {"16challenger", 25},
            {"21k5", 30},
            {"99viper", 25},
            {"ap2", 30},
            {"cobra", 20},
            {"fct", 20},
            {"m422", 30},
            {"mlmansory", 40},
            {"mustang68", 30},
            {"na6", 20},
            {"rs520", 30},
            {"rx811", 25},
            {"supraa90", 25},
            {"tricolore", 20},
            {"tur50", 55},
            {"wildtrak", 30},
            {"wraith", 40},
            {"x3gemwb", 30},

        };

        public static Dictionary<string, int> VehicleFuel = new Dictionary<string, int>()
        {
            //standart
            {"asterope", 90},
            {"issi2", 70},
            {"seminole", 100},
            {"virgo", 90},
            {"bjxl", 100},
            {"savestra", 70},
            {"sentinel2", 90},
            {"retinue", 80},
            {"sabregt", 90},
            {"sultan", 90},
            {"granger", 120},
            {"jackal", 100},
            {"sabregt2", 90},
            {"cavalcade2", 100},
            {"fusilade", 75},
            {"previon", 85},
            {"sentinel3", 80},
            {"tailgater", 85},
            {"warrener", 85},
            {"remus", 75},
            {"surge", 130},
            {"tulip", 90},
            {"baller", 120},
            {"fq2", 110},
            {"nightshade", 90},
            {"schwarzer", 75},
            {"gresley", 100},
            {"oracle2", 90},
            {"serrano", 120},
            {"brawler", 120},
            {"felon2", 90},
            {"sultan2", 90},
            {"vigero", 90},
            {"sultan3", 90},
            {"clique", 100},
            {"freecrawler", 120},
            {"ruiner", 90},
            {"baller2", 120},
            {"baller3", 120},
            {"calico", 85},
            {"exemplar", 100},
            {"rocoto", 110},
            {"zr350", 80},
            {"gauntlet4", 90},
            {"penumbra", 80},
            {"buffalo2", 90},
            {"novak", 100},
            {"imperator", 90},
            {"tropos", 80},
            {"schafter2", 80},
            {"penumbra2", 80},
            {"cogcabrio", 90},
            {"hermes", 80},
            {"rt3000", 75},
            {"zion3", 85},
            {"dominator", 100},
            {"elegy", 85},
            {"hustler", 90},
            {"komoda", 85},
            {"massacro", 75},
            {"cog55", 90},
            {"kuruma", 95},
            {"xls", 100},
            {"elegy2", 90},
            {"euros", 90},
            {"yosemite", 120},
            {"rapidgt3", 90},
            {"viseris", 70},
            {"gauntlet5", 90},
            {"windsor", 100},

            //premium
            {"baller4", 120},
            {"sc1", 90},
            {"specter", 90},
            {"alpha", 105},
            {"tailgater2", 80},
            {"cognoscenti", 95},
            {"revolter", 95},
            {"sugoi", 90},
            {"seven70", 100},
            {"dominator3", 90},
            {"casco", 85},
            {"hotknife", 90},
            {"rapidgt", 90},
            {"schafter4", 110},
            {"voltic", 130},
            {"zion2", 90},
            {"cypher", 90},
            {"khamelion", 130},
            {"surano", 90},
            {"drafter", 110},
            {"jester3", 75},
            {"lynx", 90},
            {"rapidgt2", 90},
            {"schafter3", 100},
            {"dominator7", 90},
            {"imorgon", 130},
            {"feltzer2", 100},
            {"jester4", 75},
            {"vstr", 90},
            {"ninef2", 90},
            {"pariah", 90},
            {"rebla", 115},
            {"dubsta", 110},
            {"dubsta2", 110},
            {"jugular", 90},
            {"neon", 130},
            {"streiter", 110},
            {"vectre", 95},
            {"huntley", 110},
            {"paragon", 100},
            {"patriot2", 130},
            {"verlierer2", 90},
            {"turismo2", 90},
            {"vacca", 90},
            {"windsor2", 110},
            {"schlagen", 85},
            {"stretch", 120},
            {"raiden", 130},
            {"italigto", 90},
            {"neo", 100},
            {"superd", 100},
            {"toros", 110},
            {"jb7002", 110},
            {"stinger", 85},
            {"swinger", 90},
            {"tempesta", 80},
            {"thrax", 90},
            {"torero", 105},
            {"btype", 80},
            {"dubsta3", 110},
            {"btype3", 80},
            {"btype2", 100},
            {"stafford", 100},





            //эконом
            {"tornado4", 70},
            {"voodoo2", 90},
            {"tornado3", 70},
            {"emperor2", 70},
            {"brioso2", 120},
            {"issi3", 70},
            {"surfer2", 110},
            {"regina", 90},
            {"voodoo", 90},
            {"ingot", 60},
            {"fagaloa", 40},
            {"asbo", 80},
            {"emperor", 70},
            {"tornado", 70},
            {"asea", 85},
            {"dynasty", 90},
            {"blade", 65},
            {"chino", 85},
            {"surfer", 110},
            {"tornado2", 70},
            {"blista", 80},
            {"intruder", 80},
            {"manana", 90},
            {"tornado5", 70},
            {"blista2", 80},
            {"brioso", 100},
            {"glendale", 65},
            {"blista3", 80},
            {"panto", 100},
            {"slamvan2", 110},
            {"weevil", 80},
            {"chino2", 85},
            {"manana2", 90},
            {"club", 65},
            {"glendale2", 70},
            {"seminole2", 90},
            {"rancherxl", 100},
            {"tampa", 90},
            {"dilettante", 100},
            {"nebula", 60},
            {"prairie", 75},
            {"premier", 75},
            {"stalion", 90},
            {"primo", 70},
            {"rebel", 100},
            {"rhapsody", 65},
            {"ellie", 90},
            {"stanier", 80},
            {"buccaneer", 90},
            {"dukes", 90},
            {"faction", 65},
            {"virgo2", 90},
            {"faction2", 65},
            {"buccaneer2", 90},
            {"stratum", 90},
            {"washington", 70},
            {"bfinjection", 100},
            {"futo", 85},
            {"michelli", 80},
            {"picador", 80},
            {"buffalo", 95},
            {"kanjo", 90},
            {"vamos", 90},
            {"futo2", 85},
            {"peyote", 90},
            {"minivan", 75},
            {"radi", 95},
            {"bifta", 110},
            {"oracle", 75},
            {"phoenix", 100},
            {"gauntlet", 90},
            {"peyote3", 90},
            {"cheburek", 65},
            {"mesa", 80},
            {"gauntlet3", 90},
            {"cavalcade", 100},
            {"landstalker", 105},
            {"habanero", 110},
            {"impaler", 90},
            {"fugitive", 90},
            {"hellion", 110},
            {"felon", 90},
            {"patriot", 100},
            {"landstalker2", 100},
            {"retinue2", 85},
            {"pigalle", 80},





            //moto
            {"faggio3", 80},
            {"faggio2", 80},
            {"faggio", 80},
            {"esskey", 80},
            {"bagger", 80},
            {"enduro", 85},
            {"daemon2", 95},
            {"wolfsbane", 95},
            {"pcj", 80},
            {"sanchez2", 85},
            {"daemon", 95},
            {"manchez", 85},
            {"vader", 80},
            {"blazer", 100},
            {"diablous2", 80},
            {"diablous", 80},
            {"avarus", 95},
            {"hexer", 95},
            {"sovereign", 95},
            {"blazer4", 100},
            {"fcr", 90},
            {"fcr2", 90},
            {"ruffian", 80},
            {"zombiea", 95},
            {"zombieb", 95},
            {"blazer3", 100},
            {"innovation", 95},
            {"nemesis", 90},
            {"deathbike", 95},
            {"deathbike3", 95},
            {"thrust", 80},
            {"verus", 100},
            {"vortex", 90},
            {"bati", 90},
            {"sanctus", 95},
            {"defiler", 90},
            {"gargoyle", 95},
            {"stryder", 100},
            {"double", 90},
            {"akuma", 90},
            {"carbonrs", 90},
            {"nightblade", 95},
            {"bf400", 85},
            {"hakuchou", 90},
            {"cliffhanger", 95},
            {"hakuchou2", 90},
            {"deathbike2", 95},
            {"manchez2", 85},
            {"shotaro", 90},


            //sport
            {"deviant", 100},
            {"issi7", 90},
            {"banshee", 90},
            {"omnis", 90},
            {"f620", 100},
            {"furoregt", 100},
            {"banshee2", 90},
            {"comet3", 90},
            {"coquette", 90},
            {"ruston", 85},
            {"bullet", 90},
            {"monroe", 90},
            {"comet4", 105},
            {"ninef", 90},
            {"locust", 95},
            {"comet2", 90},
            {"infernus", 95},
            {"jester", 100},
            {"carbonizzare", 105},
            {"gt500", 100},
            {"stingergt", 90},
            {"mamba", 85},
            {"bestiagts", 110},
            {"comet5", 90},
            {"reaper", 100},
            {"gp1", 100},
            {"coquette2", 80},
            {"penetrator", 100},
            {"adder", 105},
            {"entityxf", 105},
            {"infernus2", 100},
            {"osiris", 105},
            {"comet6", 100},
            {"coquette3", 80},
            {"pfister811", 100},
            {"cheetah", 95},
            {"entity2", 105},
            {"fmj", 105},
            {"cyclone", 130},
            {"italigtb", 100},
            {"t20", 100},
            {"growler", 100},
            {"cheetah2", 95},
            {"tigon", 100},
            {"emerus", 105},
            {"krieger", 105},
            {"feltzer3", 80},
            {"coquette4", 100},
            {"italirsx", 105},
            {"turismor", 105},
            {"nero", 105},
            {"tyrant", 105},
            {"furia", 105},
            {"tyrus", 100},
            {"xa21", 105},
            {"taipan", 105},
            {"autarch", 100},
            {"deveste", 100},
            {"visione", 105},
            {"zorrusso", 105},
            {"prototipo", 105},
            {"vagner", 105},
            {"zentorno", 105},
            {"tezeract", 130},
            {"ztype", 80},



            //грузовые
            {"bison", 110},
            {"bobcatxl", 120},
            {"bodhi2", 110},
            {"brutus", 130},
            {"burrito3", 120},
            {"caracara2", 110},
            {"contender", 110},
            {"dloader", 100},
            {"everon", 110},
            {"gburrito2", 120},
            {"kamacho", 110},
            {"moonbeam", 120},
            {"ratloader2", 110},
            {"rebel2", 120},
            {"riata", 120},
            {"sandking", 130},
            {"sandking2", 130},
            {"slamvan", 110},
            {"slamvan3", 120},
            {"speedo", 120},
            {"speedo4", 120},
            {"yosemite3", 110},
            {"youga2", 100},



            //Auto Donate
            {"apriora", 80},
            {"ae86", 50},
            {"w210", 70},
            {"benzc32", 70},
            {"s600", 70},
            {"bmwe38", 70},
            {"mark2", 100},
            {"gcmsentra20", 60},
            {"audia8", 80},
            {"rmodm5e34", 70},
            {"octavia18", 70},
            {"m3e46", 60},
            {"optima", 70},
            {"lancer", 80},
            {"m3e30", 70},
            {"lrdef17", 130},
            {"bnr32", 70},
            {"supra", 50},
            {"golf7r", 60},
            {"190e", 70},
            {"chevelle1970", 80},
            {"m5e60", 80},
            {"370z16", 60},
            {"kiastinger", 50},
            {"audia6", 80},
            {"boss302", 80},
            {"gl450", 130},
            {"camry18", 70},
            {"bmwg20", 60},
            {"rmodcharger69", 80},
            {"bmwm4", 70},
            {"bnr34", 70},
            {"evoque", 80},
            {"z48", 60},
            {"benzsl63", 60},
            {"volvoxc90", 130},
            {"glc2021", 90},
            {"lc200", 120},
            {"c63coupe", 70},
            {"m5", 90},
            {"durango18", 130},
            {"teslas", 150},
            {"rmodrs6", 70},
            {"mbgls63", 130},
            {"v447", 110},
            {"g65fresh", 140},
            {"bmwx7", 140},
            {"modena99", 50},
            {"rmodbmwi8", 60},
            {"rs72", 80},
            {"gtr", 80},
            {"porsche2021", 60},
            {"rmodm8c", 80},
            {"modelx", 200},
            {"dbx", 130},
            {"rmodlp570", 50},
            {"rmodbentleygt", 70},
            {"taycan", 150},
            {"bc5506d", 200},
            {"gls600", 140},
            {"600lt", 40},
            {"f458", 50},
            {"hevos", 50},
            {"488pista", 50},
            {"urus", 130},
            {"g63amg6x6", 160},
            {"f812", 50},
            {"p1", 40},
            {"lp700", 40},
            {"rculi", 150},
            {"f12berlinetta", 50},
            {"bugatti", 50},
            {"chiron17", 50},



            //Тюнинг салон
            {"rmodbiposto", 40},
            {"rmodm3e36", 70},
            {"svt00", 80},
            {"rmod240sx", 50},
            {"rmodz350pandem", 50},
            {"rmodsupra", 70},
            {"cam8tun", 70},
            {"rmodfordgt", 70},
            {"rmodsuprapandem", 50},
            {"m4comp", 70},
            {"rmodamgc63", 70},
            {"rmodm4gts", 70},
            {"rmodrover", 120},
            {"rmodmustang", 90},
            {"rmodjeep", 130},
            {"mcls53", 70},
            {"rmodgt63", 70},
            {"m5f90", 90},
            {"rmodx6", 120},
            {"rmodmi8lb", 60},
            {"rmodskyline", 80},
            {"pgt2", 60},
            {"hevo", 40},
            {"mbbs20", 70},
            {"rmodlp750", 50},


            //Сторонние тачки
            {"rs3", 50},
            {"x5om", 120},
            {"exige12", 60},
            {"bmwm7", 100},
            {"mere63amg", 90},
            {"explorer", 120},
            {"w223", 90},
            {"ocnetrongt", 150},
            {"gle63", 120},
            {"cruzzz", 150},
            {"aetron", 100},
            {"rmodrs7", 80},
            {"rmodm4", 70},
            {"rmodmi8", 60},
            {"63amg", 140},
            {"rmodi8mlb", 60},
            {"rmodi8ks", 60},
            {"fenyr", 80},
            {"roma", 50},
            {"rmodg65", 150},
            {"rmodgtr", 80},
            {"huracan", 40},
            {"f8t", 40},
            {"f40", 70},
            {"divo", 1},
            {"gt17", 40},
            {"r820", 1},
            {"rs7c8", 1},
            {"sti", 70},
            {"panamera17turbo", 100},
            {"subwrx", 1},
            {"stalion2", 120},
            {"buffalo3", 70},
            {"rrocket", 50},
            {"sultanrs", 90},
            {"impaler4", 120},

            {"al18", 150},
            {"stradale18", 70},
            {"ocnlamtmc", 120},
            {"amg1", 70},
            {"avtr", 70},
            {"huragucci", 70},
            {"mercedessl63", 70},
            {"sennasmso", 70},
            {"slr", 70},
            {"cu2", 45},
            {"dwrangler", 80},
            {"jeep392", 65},
            {"jimmy", 50},
            {"mitgalant92", 40},
            {"s15rb", 50},
            {"vwcaddy", 60},
            {"pwagon51",55 },
            {"2cv6naj",30 },
            {"vwbeetlenaj",37 },
            {"renault4",40 },
            {"simca1100",35 },
            {"forfalcxr",45 },
            {"impala72",45 },
            {"belair56",45 },
            {"zephyr41c",40 },
            {"ford34",45 },
            {"fleetline48",35 },
            {"celestem5",35 },
            {"ff1375",40 },
            {"jarama",45 },

            //exclusive
            {"jesko", 50},
            {"agera11", 40},
            {"rmodatlantic", 50},
            {"rmodbacalar", 70},
            {"rmodbugatti", 40},
            {"rmodchiron300", 40},
            {"rmodessenza", 40},
            {"sian", 30},
            {"tesla", 130},
            {"countach", 40},
            {"bugatticentodieci", 50},
            {"eleanor", 80},
            {"forgt50020", 90},
            {"rolls08", 120},
            {"fxxkevo", 40},
            {"m1procar", 60},
            {"rmodlp770", 50},
            {"rmodpagani", 50},
            {"rmodveneno", 50},

            {"500gtrlam", 50},
            {"monowheel", 30},
            {"monza", 60},
            {"morgan", 70},
            {"nc1", 70},
            {"noire19wb", 60},
            {"singer", 70},
            {"twizy", 40},
            {"vclass21", 120},
            {"x6mf16", 90},

            //Fractions
            {"youga", 50},
            {"cog552", 45},
            {"mustangpolice", 60},
            {"policet", 50},
            {"policeb", 30},
            {"police", 45},
            {"police3", 45},
            {"riot", 80},
            {"ambulance", 80},
            {"fbi", 45},
            {"fbi2", 60},
            {"police4", 45},
            {"xls2", 55},
            {"riot2", 90},
            {"insurgent2", 70},
            {"barracks", 90},
            {"brickade", 90},
            {"rumpo", 50},
            {"rmodpolice", 60},
            {"sheriff2", 60},
            {"sheriff", 45},

                        //новыq автосолон
            {"4runner", 65},
            {"16challenger", 50},
            {"21k5", 45},
            {"99viper", 40},
            {"ap2", 40},
            {"cobra", 40},
            {"fct", 50},
            {"m422", 45},
            {"mlmansory", 60},
            {"mustang68", 45},
            {"na6", 40},
            {"rs520", 45},
            {"rx811", 40},
            {"supraa90", 45},
            {"tricolore", 50},
            {"tur50", 60},
            {"wildtrak", 55},
            {"wraith", 60},
            {"x3gemwb", 50},
        };

        private static Dictionary<int, float> VehicleSize = new Dictionary<int, float>() {
            { -1, 3f },
            { 0, 3f },
            { 1, 3f },
            { 2, 3f },
            { 3, 3f },
            { 4, 3f },
            { 5, 3f },
            { 6, 3f },
            { 7, 3f },
            { 8, 3f },
            { 9, 3f },
            { 10, 10f },
            { 11, 3f },
            { 12, 3f },
            { 13, 3f },
            { 14, 10f },
            { 15, 10f },
            { 16, 10f },
            { 17, 3f },
            { 18, 3f },
            { 19, 3f },
            { 20, 3f },
            { 21, 10f },

        };
        #endregion

        public class ColorName
        {
            public Color Color;
            public string Name;

            public ColorName(string name, int r, int g, int b)
            {
                Color = new Color(r, g, b);
                Name = name;
            }

            public int computeMSE(int pixR, int pixG, int pixB)
            {
                return (((pixR - Color.Red) * (pixR - Color.Red) + (pixG - Color.Green) * (pixG - Color.Green) + (pixB - Color.Blue) * (pixB - Color.Blue)) / 3);
            }

            public string GetName()
            {
                return Name;
            }
        }

        //public static int LastVehicleId = 0;
        public VehicleManager()
        {
            try
            {
                //fuelTimer = Main.StartT(30000, 30000, (o) => FuelControl(), "FUELCONTROL_TIMER");
                Timers.StartTask("fuel", 30000, () => FuelControl());
                Timers.StartTask("mile", 5000, () => Mile());

                Log.Write("Loading Vehicles...");
                DataTable result = MySQL.QueryRead("SELECT * FROM `vehicles`");
                if (result == null || result.Rows.Count == 0)
                {
                    Log.Write("DB return null result.", nLog.Type.Warn);
                    return;
                }
                int count = 0;
                foreach (DataRow Row in result.Rows)
                {
                    count++;
                    VehicleData data = new VehicleData();
                    data.ID = Convert.ToInt32(Row["id"]);
                    data.Holder = Convert.ToString(Row["holder"]);
                    data.OwnerID = Convert.ToInt32(Row["ownerid"]);
                    data.Model = Convert.ToString(Row["model"]);
                    data.Health = Convert.ToInt32(Row["health"]);
                    data.Fuel = Convert.ToInt32(Row["fuel"]);
                    data.Price = Convert.ToInt32(Row["price"]);
                    data.Number = Convert.ToString(Row["number"]);
                    data.Components = JsonConvert.DeserializeObject<VehicleCustomization>(Row["components"].ToString());
                    //if (Row["components"].ToString() == "null") data.Components = new VehicleCustomization();
                    data.Items = JsonConvert.DeserializeObject<List<nItem>>(Row["items"].ToString());
                    data.Slots = JsonConvert.DeserializeObject<List<bool>>(Row["slots"].ToString());
                    data.currentWeight = Convert.ToSingle(Row["currentWeight"], new CultureInfo("en-US"));

                    if (data.Slots == null)
                    {
                        List<bool> tempslots = new List<bool>();
                        for (int i = 0; i < VehicleInventory.CountSlots; i++)
                        {
                            tempslots.Add(true);
                        }
                        data.Slots = tempslots;
                    }

                    data.Position = Convert.ToString(Row["position"]);
                    data.Rotation = Convert.ToString(Row["rotation"]);
                    data.KeyNum = Convert.ToInt32(Row["keynum"]);
                    data.Mileage = Convert.ToInt32(Row["mileage"]);
                    data.Dirt = (float)Row["dirt"];
                    data.Passport = JsonConvert.DeserializeObject<TechnicalPassport>(Row["passport"].ToString());
                    data.Priority = Convert.ToInt32(Row["priority"]);
                    data.OtherData = JsonConvert.DeserializeObject<OtherVehicleData>(Row["other_data"].ToString());
                    data.Vehicle = null;

                    Vehicles.Add(data.ID, data);

                    //LastVehicleId = data.ID;
                }
                Log.Write($"Vehicles are loaded ({count})", nLog.Type.Success);
            }
            catch (Exception e) { Log.Write("ResourceStart: " + e.StackTrace, nLog.Type.Error); }
        }

        public static void CalcWeight(int number)
        {
            var weight = 0.0f;
            if (!Vehicles.ContainsKey(number)) {
                return;
            }

            foreach (var item in Vehicles[number].Items)
            {
                item.Weight = nInventory.ItemsWeight[item.Type] * item.Count;
                weight += item.Weight;
            }
            Vehicles[number].currentWeight = weight;
            //return weight;
        }

        public static int CheckWeight(int number, nItem additem)
        {
            var currentWeight = 0.0f;
            if (!Vehicles.ContainsKey(number)) {
                return -3;
            }

            var maxWeight = VehicleManager.VehicleWeight[Vehicles[number].Model.ToString().ToLower()];

            foreach (var item in Vehicles[number].Items)
            {
                item.Weight = nInventory.ItemsWeight[item.Type] * item.Count;
                currentWeight += item.Weight;
            }

            if (currentWeight + additem.Weight > maxWeight)
            {
                return -2;
            }

            return 0;
        }

        public static void PutTrunkPlayer(Player target, Vehicle vehicle)
        {
            Trigger.PlayerEventInRange(target.Position, 150f, "testtrunk", target, vehicle);
            vehicle.SetSharedData("trunkPlayer", target);
            Main.PlayAnimation(target, "anim@amb@nightclub@lazlow@lo_sofa@", "lowsofa_dlg_crying_laz", 39);
        }

        public static string GetVehicleRealName(string model)
        {
            string name = "-1";

            if (VehiclesInformation.VehiclesModels.ContainsKey(model))
            {
                return VehiclesInformation.VehiclesModels[model].Name;
            }

            foreach(KeyValuePair<int, Dictionary<string, string>> pair in BusinessManager.RealVehicles)
            {
                if (pair.Value.ContainsKey(model)) return pair.Value[model];
            }

            Log.Debug($"[CHECK THIS] NOTFOUND GetVehicleRealName: {model}");

            return name;
        }

        public static void OutTrunkPlayer(Vehicle vehicle)
        {
            if (!vehicle.HasData("IN_TRUNK")) return;

            Player player = vehicle.GetData<Player>("IN_TRUNK");

            vehicle.ResetSharedData("trunkPlayer");

            Main.StopAnimation(player);
            player.Position = (player.Position - vehicle.Position).Normalized * 1.5f + player.Position;

            Trigger.ClientEvent(player, "outTrunk");
        }

        public static List<int> GetFreePositions(List<Parkings.ParkingPlace> places)
        {
            if (places.Count == 0)
                return new List<int>();
            List<Vector3> positions = new List<Vector3>();
            foreach (var place in places)
                positions.Add(place.Position);
            return GetFreePositions(positions);
        }

        public static List<int> GetFreePositions(List<Vector3> positions)
        {
            if (positions.Count == 0)
                return new List<int>();
            List<int> indexes = new List<int>();
            List<Vehicle> vehicles = NAPI.Pools.GetAllVehicles();
            for (int i = 0; i < positions.Count; i++)
            {
                int count = vehicles.FindAll((v) => v.Position.DistanceTo(positions[i]) < VehicleSize[v.Class]).Count;
                if (count == 0)
                    indexes.Add(i);
            }
            return indexes;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="positions"></param>
        /// <returns>index | -1 when cant find</returns>
        public static int GetFreePosition(List<Vector3> positions)
        {
            if (positions.Count == 0) return -1;
            List<Vehicle> vehicles = NAPI.Pools.GetAllVehicles();
            for (int i = 0; i < positions.Count; i++)
            {
                int count = vehicles.FindAll((v) => v.Position.DistanceTo(positions[i]) < VehicleSize[v.Class]).Count;
                if (count == 0) return i;
            }
            return -1;
        }

        public static int GetFreePosition(List<Parkings.ParkingPlace> places)
        {
            if (places.Count == 0)
                return -1;
            List<Vector3> positions = new List<Vector3>();
            foreach (var place in places)
                positions.Add(place.Position);
            return GetFreePosition(positions);
        }

        private static void FuelControl()
        {
            NAPI.Task.Run(() =>
            {
                List<Vehicle> allVehicles = NAPI.Pools.GetAllVehicles();
                if (allVehicles.Count == 0) return;
                foreach (Vehicle veh in allVehicles)
                {
                    object f = null;
                    try
                    {
                        if (veh.NumberPlate == "ADMIN") continue;
                        if (!veh.HasSharedData("PETROL"))
                        {
                            if (VehicleTank.ContainsKey(veh.Class))
                                veh.SetSharedData("PETROL", VehicleTank[veh.ClassName]);
                        }
                        if (!Core.VehicleStreaming.GetEngineState(veh)) continue;

                        f = veh.GetSharedData<int>("PETROL");
                        int fuel = (int)f;

                        if (fuel == 0) continue;
                        if (fuel - PetrolRate[veh.Class] <= 0)
                        {
                            fuel = 0;
                            Core.VehicleStreaming.SetEngineState(veh, false);
                        }
                        else fuel -= PetrolRate[veh.Class];
                        veh.SetSharedData("PETROL", fuel);

                        if (veh.HasData("ID"))
                        {
                          int id = veh.GetData<int>("ID");
                          if (Vehicles.ContainsKey(id)) Vehicles[id].Fuel = fuel;
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Write($"FUELCONTROL_TIMER: {veh.NumberPlate} {f.ToString()}\n{e.StackTrace}", nLog.Type.Error);
                    }
                }
            });
        }

        static DateTime Now = DateTime.Now;

        public static void Mile()
        {
            NAPI.Task.Run(() => {
                Vehicle localveh = null;

                try
                {
                    foreach (Vehicle veh in NAPI.Pools.GetAllVehicles())
                    {
                        localveh = veh;

                        if (!veh.EngineStatus) continue;

                        Vector3 velocity = NAPI.Entity.GetEntityVelocity(veh.Handle);

                        double speeds = Math.Sqrt((velocity.X * velocity.X) + (velocity.Y * velocity.Y) + (velocity.Z * velocity.Z)) * 3.6;

                        float distance = (float)((float)speeds * ((DateTime.Now - Now).TotalSeconds / 1000) * 100 / 100);

                        if (veh.HasData("ID"))
                        {
                            int id = veh.GetData<int>("ID");
                            if (Vehicles.ContainsKey(id)) Vehicles[id].Mileage += distance / 5;

                            //Save(veh.NumberPlate);
                        }

                    }

                    Now = DateTime.Now;
                }
                catch (Exception e)
                {
                    Log.Write($"Mileage ({localveh.NumberPlate}): " + e.ToString(), nLog.Type.Error);
                }
            });
        }

        [ServerEvent(Event.PlayerEnterVehicleAttempt)]
        public void onPlayerEnterVehicleAttemptHandler(Player player, Vehicle vehicle, sbyte seatid)
        {
            try
            {
                if (SpawnVeh == vehicle) player.StopAnimation();
            }
            catch(Exception e) { Log.Write(e.StackTrace, nLog.Type.Error); }
        }

        public static void UpdateVehicleMaterials(Vehicle veh, int val)
        {
            TextLabel label = veh.GetData<TextLabel>("TRUNK_LABEL");
            //if (Fractions.Manager.MaterialsType == 1)
            if (veh.HasData("MATERIALS"))
            {
                int mats = veh.GetData<int>("MATERIALS");
                veh.SetData("MATERIALS", mats + val);
                label.Text = $"~w~Материалы\n\n~g~{veh.GetData<int>("MATERIALS")}~w~ / {Stocks.maxMats[(VehicleHash)veh.Model]}";
            }
            //else if (Fractions.Manager.MaterialsType == 0)
            else if (veh.HasData("MEDICAMENTS"))
            {
                int mats = veh.GetData<int>("MEDICAMENTS");
                veh.SetData("MEDICAMENTS", mats + val);
                label.Text = $"~w~Медикаменты\n\n~g~{veh.GetData<int>("MEDICAMENTS")}~w~ / {Stocks.maxMats[(VehicleHash)veh.Model]}";
            }
        }


        public static void PutOrGetMaterials(Player player)
        {
            try
            {
                if (player.Vehicle != null)
                    return;

                if (!Main.CanStartAnimation(player, Main.Anim.LoadBox))
                    return;
                /*if (Main.IsAnimation(player))
                    return;*/

                if (!player.HasMyData("TRUNK_VEHICLE"))
                    return;

                Vehicle veh = player.GetMyData<Vehicle>("TRUNK_VEHICLE");

                if (player.HasMyData("MATERIALS") || player.HasMyData("MEDICAMENTS"))
                {
                    if (!veh.HasData("MATERIALS") && !veh.HasData("MEDICAMENTS"))
                        return;

                    if (player.HasMyData("MATERIALS") && veh.HasData("MATERIALS"))
                    {
                        int mats = veh.GetData<int>("MATERIALS");
                        int playerMats = player.GetMyData<int>("MATERIALS");
                        if (mats + playerMats > Stocks.maxMats[(VehicleHash)veh.Model])
                        {
                            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, "Машина уже заполнена", 3000);
                            return;
                        }
                        player.Rotation = new Vector3(0, 0, Main.Angle(player.Position, veh.Position));
                        Main.PlayAnimation(player, Main.Anim.LoadBox);

                        NAPI.Task.Run(() => {
                            try
                            {
                                if (player != null && Main.Players.ContainsKey(player))
                                {
                                    if (!player.HasMyData("MATERIALS"))
                                        return;
                                    mats = veh.GetData<int>("MATERIALS");
                                    if (mats + playerMats >= Stocks.maxMats[(VehicleHash)veh.Model])
                                    {
                                        Main.PlayAnimation(player, Main.Anim.BoxCarry);
                                        Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, "Машина уже заполнена", 3000);
                                        return;
                                    }
                                    BasicSync.AddAttachment(player, "prop_cs_box_step", true);
                                    Main.StopAnimation(player);

                                    player.ResetMyData("MATERIALS");

                                    UpdateVehicleMaterials(veh, playerMats);
                                }
                            }
                            catch (Exception e)
                            {
                                Log.Write("task: " + e.StackTrace);
                            }
                        }, 2000);
                    }
                    else if (player.HasMyData("MEDICAMENTS") && veh.HasData("MEDICAMENTS"))
                    {
                        int mats = veh.GetData<int>("MEDICAMENTS");
                        int playerMats = player.GetMyData<int>("MEDICAMENTS");
                        if (mats + playerMats > Stocks.maxMats[(VehicleHash)veh.Model])
                        {
                            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, "Машина уже заполнена", 3000);
                            return;
                        }
                        player.Rotation = new Vector3(0, 0, Main.Angle(player.Position, veh.Position));
                        Main.PlayAnimation(player, Main.Anim.LoadBox);

                        NAPI.Task.Run(() => {
                            try
                            {
                                if (player != null && Main.Players.ContainsKey(player))
                                {
                                    if (!player.HasMyData("MEDICAMENTS"))
                                        return;
                                    mats = veh.GetData<int>("MATERIALS");
                                    if (mats + playerMats > Stocks.maxMats[(VehicleHash)veh.Model])
                                    {
                                        Main.PlayAnimation(player, Main.Anim.BoxCarry);
                                        Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, "Машина уже заполнена", 3000);
                                        return;
                                    }
                                    BasicSync.AddAttachment(player, "prop_cs_box_step", true);
                                    Main.StopAnimation(player);
                                  
                                    player.ResetMyData("MEDICAMENTS");

                                    UpdateVehicleMaterials(veh, playerMats);
                                }
                            }
                            catch (Exception e)
                            {
                                Log.Write("task: " + e.StackTrace);
                            }
                        }, 2000);
                    }
                }
                else
                {
                    if (Main.Players[player].Fraction.FractionID == 0)
                        return;

                    if (!veh.HasData("MATERIALS") && !veh.HasData("MEDICAMENTS"))
                        return;

                    if (veh.HasData("MATERIALS"))
                    {
                        int mats = veh.GetData<int>("MATERIALS");
                        if (mats <= 0)
                        {
                            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, "Машина пуста", 3000);
                            return;
                        }

                        player.SetMyData("MATERIALS", Fractions.Manager.boxMats);
                      BasicSync.AddAttachment(player, "prop_cs_box_step", false);
                      //BasicSync.AttachObjectToPlayer(player, NAPI.Util.GetHashKey("prop_box_ammo03a"), "BONETAG_R_PH_HAND", new Vector3(0, 0, -0.03), new Vector3(90, 0, 0));
                    }
                    else if (veh.HasData("MEDICAMENTS"))
                    {
                        int mats = veh.GetData<int>("MEDICAMENTS");
                        if (mats <= 0)
                        {
                            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, "Машина пуста", 3000);
                            return;
                        }

                        player.SetMyData("MEDICAMENTS", Fractions.Manager.boxMats);

                        //BasicSync.AttachObjectToPlayer(player, NAPI.Util.GetHashKey("prop_apple_box_01"), "BONETAG_R_PH_HAND", new Vector3(0, 0, -0.03), new Vector3(90, 0, 0));
                        BasicSync.AddAttachment(player, "prop_cs_box_step", false);
                    }
                    else
                        return;

                    Main.OnAntiAnim(player);
                    player.PlayAnimation("anim@heists@box_carry@", "idle", 49);
                    //NAPI.Player.PlayPlayerAnimation(player, (int)(AnimationFlags.Loop | AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.AllowPlayerControl), "anim@heists@box_carry@", "idle");

                    UpdateVehicleMaterials(veh, -Fractions.Manager.boxMats);

                }
            }
            catch (Exception e)
            {
                Log.Write($"PutOrGetMaterials: " + e.StackTrace);
            }
        }

        public static Vector3 GetTrunkCoords(Vehicle vehicle)
        {
            float longVeh = 0f;
            if (TrunkCoords.ContainsKey((VehicleHash)vehicle.Model))
                longVeh = TrunkCoords[(VehicleHash)vehicle.Model];

            return Main.OffsetPosition(vehicle.Position, vehicle.Rotation.Z + 180, longVeh);
        }

        [ServerEvent(Event.PlayerEnterVehicle)]
        public void onPlayerEnterVehicleHandler(Player player, Vehicle vehicle, sbyte seatid)
        {
            try
            {
                if (SpawnVeh == vehicle)
                {
                    player.WarpOutOfVehicle();
                    return;
                }
                if (!vehicle.HasData("OCCUPANTS"))
                {
                    List<Player> occupantsList = new List<Player>();
                    occupantsList.Add(player);
                    vehicle.SetData("OCCUPANTS", occupantsList);
                }
                else
                {
                    if (!vehicle.GetData<List<Player>>("OCCUPANTS").Contains(player)) vehicle.GetData<List<Player>>("OCCUPANTS").Add(player);
                }

                if (player.HasMyData("RodInHand"))
                {
                    if (player.GetMyData<bool>("RodInHand"))
                    {
                        var oldwItem = nInventory.Items[Main.Players[player].UUID].FirstOrDefault(i => (i.Type == ItemType.Rod || i.Type == ItemType.RodMK2 || i.Type == ItemType.RodUpgrade) && i.IsActive);
                        BasicSync.AddAttachment(player, "roding", true);
                        player.SetMyData("RodInHand", false);

                        oldwItem.IsActive = false;

                        if (nInventory.WeaponsItems.Contains(oldwItem.Type) && !oldwItem.IsActive && oldwItem.fast_slot_id > 0) GUI.Dashboard.SyncAttachComp(player, oldwItem, false);
                        GUI.Dashboard.Update(player, oldwItem, nInventory.Items[Main.Players[player].UUID].IndexOf(oldwItem));
                    }
                }

                if (player.VehicleSeat == 0)
                {
                    if (NAPI.Data.GetEntityData(vehicle, "ACCESS") == "FRACTION")
                    {
                        if (NAPI.Data.GetEntityData(vehicle, "FRACTION") == 14 && vehicle.DisplayName == "BARRACKS")
                        {


                            int fracid = Main.Players[player].Fraction.FractionID;
                            if ((fracid >= 1 && fracid <= 5) || (fracid >= 10 && fracid <= 13))
                            {
                                if (DateTime.Now.Hour < 10)
                                {
                                    Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Нелегальные организации не могут сесть в машину с 00:00 до 10:00", 3000);
                                    return;
                                }
                                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Чтобы завести двигатель, используйте военную отмычку", 3000);
                                return;
                            }
                            else if (fracid == 14)
                            {
                                if (Main.Players[player].Fraction.FractionRankID < NAPI.Data.GetEntityData(vehicle, "MINRANK"))
                                {
                                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы не имеете доступа к этому транспорту", 3000);
                                    WarpPlayerOutOfVehicle(player);
                                    return;
                                }
                                //Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Чтобы завести двигатель, нажмите B", 3000);
                                Trigger.ClientEvent(player, "CLIENT::keyBind:engine_notify");
                                return;
                            }
                            else
                            {
                                WarpPlayerOutOfVehicle(player);
                            }
                        }
                        if (NAPI.Data.GetEntityData(vehicle, "FRACTION") == Main.Players[player].Fraction.FractionID)
                        {
                            if (!HasAccess(player, vehicle))
                            {
                                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Нет доступа", 3000);
                                WarpPlayerOutOfVehicle(player);
                                return;
                            }
                            //Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Чтобы завести двигатель, нажмите B", 3000);
                            Trigger.ClientEvent(player, "CLIENT::keyBind:engine_notify");
                        }
                        else
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы не имеете доступа к этому транспорту", 3000);
                            WarpPlayerOutOfVehicle(player);
                            return;
                        }
                    }
                    else if (NAPI.Data.GetEntityData(vehicle, "ACCESS") == "WORK" && player.GetMyData<Vehicle>("WORK") == vehicle)
                        Trigger.ClientEvent(player, "CLIENT::keyBind:engine_notify");
                }
            }
            catch (Exception e) { Log.Write("PlayerEnterVehicle: " + e.StackTrace, nLog.Type.Error); }
        }
        public static bool HasAccess(Player player, Vehicle vehicle)
        {
            if (!Main.Players.ContainsKey(player))
                return false;

            if (vehicle.GetData<string>("ACCESS").Equals("FRACTION"))
            {
                if (vehicle.GetData<int>("FRACTION") == Main.Players[player].Fraction.FractionID && Configs.FractionRanks[Main.Players[player].Fraction.FractionID][Main.Players[player].Fraction.FractionRankID].VehicleAccess[vehicle.NumberPlate])
                {
                    return true;
                }
                else return false;
            }
            return false;
        }

        [ServerEvent(Event.PlayerExitVehicleAttempt)]
        public void onPlayerExitVehicleHandler(Player player, Vehicle vehicle)
        {
            try
            {
                if (vehicle == null || !Main.Players.ContainsKey(player)) return;
                if (!vehicle.HasData("OCCUPANTS"))
                {
                    List<Player> occupantsList = new List<Player>();
                    vehicle.SetData("OCCUPANTS", occupantsList);
                }
                else
                {
                    if (vehicle.GetData<List<Player>>("OCCUPANTS").Contains(player)) vehicle.GetData<List<Player>>("OCCUPANTS").Remove(player);
                }
            }
            catch (Exception e) { Log.Write("PlayerExitVehicleAttempt: " + e.StackTrace, nLog.Type.Error); }
        }

        public static void API_onPlayerDisconnected(Player player, DisconnectionType type, string reason)
        {
            try
            {

                if (!Main.Players.ContainsKey(player)) return;
                if (player.IsInVehicle)
                {
                    Vehicle vehicle = player.Vehicle;
                    if (!vehicle.HasData("OCCUPANTS"))
                    {
                        List<Player> occupantsList = new List<Player>();
                        vehicle.SetData("OCCUPANTS", occupantsList);
                    }
                    else
                    {
                        if (vehicle.GetData<List<Player>>("OCCUPANTS").Contains(player)) vehicle.GetData<List<Player>>("OCCUPANTS").Remove(player);
                    }
                }

                if (player.HasMyData("WORK_CAR_EXIT_TIMER"))
                    //Main.StopT(player.GetMyData<string>("WORK_CAR_EXIT_TIMER"), "WORK_CAR_EXIT_TIMER_vehicle");
                    Timers.Stop(player.GetMyData<string>("WORK_CAR_EXIT_TIMER"));
            }
            catch (Exception e) { Log.Write("PlayerDisconnected: " + e.StackTrace, nLog.Type.Error); }
        }

        public static void WarpPlayerOutOfVehicle(Player player)
        {
            Vehicle vehicle = player.Vehicle;
            if (vehicle == null) return;

            if (!vehicle.HasData("OCCUPANTS"))
            {
                List<Player> occupantsList = new List<Player>();
                vehicle.SetData("OCCUPANTS", occupantsList);
            }
            else
            {
                if (vehicle.GetData<List<Player>>("OCCUPANTS").Contains(player)) vehicle.GetData<List<Player>>("OCCUPANTS").Remove(player);
            }
            player.WarpOutOfVehicle();
        }

        public static List<Player> GetVehicleOccupants(Vehicle vehicle)
        {
            if (!vehicle.HasData("OCCUPANTS"))
                return new List<Player>();
            else
                return vehicle.GetData<List<Player>>("OCCUPANTS");
        }

        public static void RepairCar(Vehicle vehicle)
        {
            vehicle.Repair();
            VehicleStreaming.UpdateVehicleSyncData(vehicle, new VehicleStreaming.VehicleSyncData());
        }

        public static int Create(string Holder, int ownerid, string Model, Color Color1, Color Color2, Color Color3, int Health = 1000, int Fuel = 100, int Price = 0)
        {
            List<bool> tempslots = new List<bool>();
            for (int i = 0; i < VehicleInventory.CountSlots; i++)
            {
                tempslots.Add(true);
            }

            var lastVehicleID = 1;
            if (Vehicles.Count() > 0)
            {
                lastVehicleID = Vehicles.Keys.Last() + 1;
            }

            if (Vehicles.ContainsKey(lastVehicleID))
            {
                while(Vehicles.ContainsKey(lastVehicleID))
                {
                    lastVehicleID += 1;
                }
            }

            VehicleData data = new VehicleData();
            data.ID = lastVehicleID;
            data.Holder = Holder;
            data.Model = Model;
            data.Health = Health;
            data.Fuel = Fuel;
            data.Price = Price;
            data.Components = new VehicleCustomization();
            data.Components.PrimColor = Color1;
            data.Components.SecColor = Color2;
            data.Components.NeonColor = new Color(0, 0, 0, 0);
            data.Items = new List<nItem>();
            data.Slots = tempslots;
            data.currentWeight = 0.0f;
            data.Dirt = 0.0F;
            data.Number = "";
            data.OwnerID = ownerid;
            data.Priority = 999;

            Log.Debug("data.ID: "+ data.ID + " Vehicles.Count: "+ Vehicles.Count);
            Vehicles.Add(data.ID, data);

            var currentWeight = data.currentWeight.ToString("0.000", CultureInfo.GetCultureInfo("en-US"));

            //MySQL.Query("INSERT INTO `vehicles` " +
            //"(" +
            //    "`id`, " +
            //    "`number`, " +
            //    "`holder`, " +
            //    "`model`, " +
            //    "`health`, " +
            //    "`fuel`, " +
            //    "`price`, " +
            //    "`components`, " +
            //    "`items`, " +
            //    "`slots`, " +
            //    "`currentWeight`, " +
            //    "`keynum`, " +
            //    "`ownerid`, " +
            //    "`dirt`" +
            //")" +
            //$" VALUES (" +
            //    $"{data.ID}," +
            //    $"'{data.Number}'," +
            //    $"'{Holder}'," +
            //    $"'{Model}'," +
            //    $"{Health}," +
            //    $"{Fuel}," +
            //    $"{Price}," +
            //    $"'{JsonConvert.SerializeObject(data.Components)}'," +
            //    $"'{JsonConvert.SerializeObject(data.Items)}'," +
            //    $"'{JsonConvert.SerializeObject(tempslots)}'," +
            //    $"'{currentWeight}'," +
            //    $"{data.KeyNum}," +
            //    $"{ownerid}," +
            //    $"{(byte)data.Dirt}" +
            //$")");

            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = "INSERT INTO `vehicles` SET " +
              "`id`=@id," +
              "`number`=@number," +
              "`holder`=@holder," +
              "`model`=@model," +
              "`health`=@health," +
              "`fuel`=@fuel," +
              "`price`=@price," +
              "`components`=@components," +
              "`items`=@items," +
              "`slots`=@slots," +
              "`currentWeight`=@currentWeight," +
              "`keynum`=@keynum," +
              "`ownerid`=@ownerid," +
              "`dirt`=@dirt";

            cmd.Parameters.AddWithValue("@id", data.ID);
            cmd.Parameters.AddWithValue("@number", data.Number);
            cmd.Parameters.AddWithValue("@holder", Holder);
            cmd.Parameters.AddWithValue("@model", Model);
            cmd.Parameters.AddWithValue("@health", Health);
            cmd.Parameters.AddWithValue("@fuel", Fuel);
            cmd.Parameters.AddWithValue("@price", Price);
            cmd.Parameters.AddWithValue("@components", JsonConvert.SerializeObject(data.Components));
            cmd.Parameters.AddWithValue("@items", JsonConvert.SerializeObject(data.Items));
            cmd.Parameters.AddWithValue("@slots", JsonConvert.SerializeObject(tempslots));
            cmd.Parameters.AddWithValue("@currentWeight", currentWeight);
            cmd.Parameters.AddWithValue("@keynum", data.KeyNum);
            cmd.Parameters.AddWithValue("@ownerid", ownerid);
            cmd.Parameters.AddWithValue("@dirt", (byte)data.Dirt);
            MySQL.Query(cmd);

            Log.Debug("Vehicle.cs [Create Number: "+ data.Number + "] - currentWeight = " + currentWeight);
            Log.Write("Created new vehicle with number: " + data.Number);

            return data.ID;
        }

        public static void Remove(int Number, Player player = null)
        {
            try
            {
                if (!Vehicles.ContainsKey(Number)) return;
                Houses.House house = Houses.HouseManager.GetHouse(Vehicles[Number].Holder, true);
                if (house != null)
                {
                    Houses.Garage garage = Houses.GarageManager.Garages[house.GarageID];
                    garage.DeleteCar(Number);
                }

                Vehicles.Remove(Number);
                //MySQL.Query($"DELETE FROM `vehicles` WHERE id='{Number}'");

                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = "DELETE FROM `vehicles` WHERE `id`=@id";

                cmd.Parameters.AddWithValue("@id", Number);
                MySQL.Query(cmd);
            }
            catch(Exception e) { Log.Write("VehicleManager remove: " + e.StackTrace, nLog.Type.Error); }
        }

        public static Vehicle Spawn(int Number, Vector3 Pos, float Rot, Player player)
        {
            if (!Vehicles.ContainsKey(Number))
            {
                Log.Write("Failed to spawn vehicle " + Number);
                return null;
            }

            VehicleData data = Vehicles[Number];
            // VehicleHash model = (VehicleHash)NAPI.Util.GetHashKey(data.Model);
            VehicleHash model = (VehicleHash)NAPI.Util.GetHashKey(data.Model);
            Vehicle veh = NAPI.Vehicle.CreateVehicle(model, Pos, Rot, 0, 0);

            veh.Health = data.Health;
            veh.NumberPlate = data.Number;
            veh.SetSharedData("PETROL", data.Fuel);
            veh.SetData("ACCESS", "PERSONAL");
            veh.SetData("OWNER", player);
            veh.SetData("ITEMS", data.Items);
            veh.SetData("SLOTS", data.Slots);
            veh.SetData("ID", data.ID);

            NAPI.Vehicle.SetVehicleNumberPlate(veh, data.Number);
            VehicleStreaming.SetEngineState(veh, false);
            VehicleStreaming.SetLockStatus(veh, true);
            VehicleManager.ApplyCustomization(veh);

            return veh;
        }

        public static bool Save(int Number)
        {
            if (!Vehicles.ContainsKey(Number)) return false;
            VehicleData data = Vehicles[Number];
            string items = JsonConvert.SerializeObject(data.Items);
            if (string.IsNullOrEmpty(items) || items == null) items = "[]";

            var currentWeight = data.currentWeight.ToString("0.000", CultureInfo.GetCultureInfo("en-US"));

            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = "UPDATE `vehicles` SET " +
              "number=@number," +
              "holder=@hold," +
              "model=@model," +
              "health=@hp," +
              "fuel=@fuel," +
              "components=@comp," +
              "items=@it," +
              "position=@pos," +
              "rotation=@rot," +
              "keynum=@keyn," +
              "dirt=@dirt," +
              "mileage=@mileage," +
              "priority=@priority," +
              "slots=@slots," +
              "currentWeight=@currentWeight," +
              "other_data=@otherdata" +
              " WHERE id=@id";

            cmd.Parameters.AddWithValue("@number", data.Number);
            cmd.Parameters.AddWithValue("@hold", data.Holder);
            cmd.Parameters.AddWithValue("@model", data.Model);
            cmd.Parameters.AddWithValue("@hp", data.Health);
            cmd.Parameters.AddWithValue("@fuel", data.Fuel);
            cmd.Parameters.AddWithValue("@comp", JsonConvert.SerializeObject(data.Components));
            cmd.Parameters.AddWithValue("@it", items);
            cmd.Parameters.AddWithValue("@pos", data.Position);
            cmd.Parameters.AddWithValue("@rot", data.Rotation);
            cmd.Parameters.AddWithValue("@keyn", data.KeyNum);
            cmd.Parameters.AddWithValue("@dirt", (byte)data.Dirt);
            cmd.Parameters.AddWithValue("@mileage", data.Mileage);
            cmd.Parameters.AddWithValue("@priority", data.Priority);
            cmd.Parameters.AddWithValue("@id", data.ID);
            cmd.Parameters.AddWithValue("@slots", JsonConvert.SerializeObject(data.Slots));
            cmd.Parameters.AddWithValue("@otherdata", JsonConvert.SerializeObject(data.OtherData));
            cmd.Parameters.AddWithValue("@currentWeight", currentWeight);

            //Log.Debug("[Save] Vehicle.cs [Number: "+Number+"] - currentWeight = " + currentWeight + " components: "+ JsonConvert.SerializeObject(data.Components));
            MySQL.Query(cmd);

            return true;
        }

        public static bool isHaveAccess(Player Player, Vehicle Vehicle)
        {
            if (NAPI.Data.GetEntityData(Vehicle, "ACCESS") == "WORK")
            {
                if (Player.GetMyData<Vehicle>("WORK") != Vehicle)
                    return false;
                else
                    return true;
            }
            else if (NAPI.Data.GetEntityData(Vehicle, "ACCESS") == "FRACTION")
            {
                if (Main.Players[Player].Fraction.FractionID != NAPI.Data.GetEntityData(Vehicle, "FRACTION"))
                    return false;
                else
                    return true;
            }
            else if (NAPI.Data.GetEntityData(Vehicle, "ACCESS") == "FAMILY")
            {
                if (Main.Players[Player].FamilyCID != NAPI.Data.GetEntityData(Vehicle, "FAMILY"))
                    return false;
                else
                    return true;
            }
            else if (NAPI.Data.GetEntityData(Vehicle, "ACCESS") == "PERSONAL")
            {
                bool access = canAccessByNumber(Player, Vehicle.GetData<int>("ID"));
                if (access)
                    return true;
                else
                    return false;
            }
            else if (NAPI.Data.GetEntityData(Vehicle, "ACCESS") == "GARAGE")
            {
                bool access = canAccessByNumber(Player, Vehicle.GetData<int>("ID"));
                if (access)
                    return true;
                else
                    return false;
            }
            else if (NAPI.Data.GetEntityData(Vehicle, "ACCESS") == "HOTEL")
            {
                if (Player.HasMyData("HOTELCAR") && Player.GetMyData<Vehicle>("HOTELCAR") == Vehicle)
                {
                    return true;
                }
                else
                    return false;
            }
            else if (NAPI.Data.GetEntityData(Vehicle, "ACCESS") == "RENT")
            {
                if (NAPI.Data.GetEntityData(Vehicle, "DRIVER") == Player)
                {
                    return true;
                }
                else
                    return false;
            }
            else if (NAPI.Data.GetEntityData(Vehicle, "ACCESS") == "LOADER")
            {
                if (NAPI.Data.GetEntityData(Vehicle, "DRIVER") == Player)
                {
                    return true;
                }
                else
                    return false;
            }
            else if (NAPI.Data.GetEntityData(Vehicle, "ACCESS") == "BUILDRENT")
            {
                if (NAPI.Data.GetEntityData(Vehicle, "DRIVER") == Player)
                {
                    return true;
                }
                else
                    return false;
            }
            return true;
        }

        public static Vehicle getNearestVehicle(Player player, int radius)
        {
            List<Vehicle> all_vehicles = NAPI.Pools.GetAllVehicles();
            Vehicle nearest_vehicle = null;
            foreach (Vehicle v in all_vehicles)
            {
                if (v.Dimension != player.Dimension) continue;
                if (nearest_vehicle == null && player.Position.DistanceTo(v.Position) < radius)
                {
                    nearest_vehicle = v;
                    continue;
                }
                else if (nearest_vehicle != null)
                {
                    if (player.Position.DistanceTo(v.Position) < player.Position.DistanceTo(nearest_vehicle.Position))
                    {
                        nearest_vehicle = v;
                        continue;
                    }
                }
            }
            return nearest_vehicle;
        }

        public static List<int> getAllPlayerVehicles(string playername)
        {
            List<int> all_number = new List<int>();
            foreach (KeyValuePair<int, VehicleData> accVehicle in Vehicles)
                if (accVehicle.Value.Holder == playername)
                {
                    all_number.Add(accVehicle.Key);
                }
            return all_number;
        }

        public static int GetVehicleIdByNumber(string num)
        {
            foreach(KeyValuePair<int, VehicleData> pair in VehicleManager.Vehicles)
            {
                if (pair.Value.Number == num) return pair.Key;
            }

            return -1;
        }

        public static void sellCar(Player player, Player target)
        {
            player.SetMyData("SELLCARFOR", target);
            OpenSellCarMenu(player);
        }

        #region Selling Menu
        public static void OpenSellCarMenu(Player player)
        {

            List<VehicleManager.VehicleData> vehicles = new List<VehicleManager.VehicleData>();
            var vehs = VehicleManager.getAllPlayerVehicles(player.Name);

            foreach (int id in vehs) vehicles.Add(VehicleManager.Vehicles[id]);

            if (vehicles.Count == 0)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "У вас нет транспортных средств");
                return;
            }
            else if (vehicles.Count == 1)
            {
                SellCarToTarget(player, player, vehicles[0]);
            }
            else
            {
                List<ListSystem.Item> items = new List<ListSystem.Item>();
                foreach (var veh in vehicles)
                    items.Add(new ListSystem.Item($"{VehicleManager.GetVehicleRealName(veh.Model)} {veh.Number}", veh));
                ListSystem.Open(player, "Продажа", new ListSystem.List((veh) => SellCarToTarget(player, player, (VehicleManager.VehicleData)veh), items));
            }
            return;
        }

        private static void SellCarToTarget(Player player, Player target, VehicleData data)
        {
            player.SetMyData("SELLCARNUMBER", Convert.ToInt32(data.ID));
            Trigger.ClientEvent(player, "popup::openInput", "Продать машину", "Введите цену", 8, "sellcar");
        }

        public static void TakeNumber(Player player, Vehicle vehicle) // Снять
        {
            try
            {
                if (!vehicle.HasData("ACCESS") || vehicle.GetData<string>("ACCESS") != "PERSONAL" && vehicle.GetData<string>("ACCESS") != "GARAGE" && vehicle.GetData<string>("ACCESS") != "FAMILY")
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Нельзя снять номера с этой машины", 3000);
                    return;
                }

                VehicleData vData = VehicleManager.Vehicles[vehicle.GetData<int>("ID")];

                if (vData.OwnerID != Main.Players[player].UUID)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы не можете снять номера с этой машины", 3000);
                    return;
                }

                if (vData.Number == "")
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "В машине не установлены номера", 3000);
                    return;
                }
                nItem item = new nItem(ItemType.NumberPlate, data: vehicle.NumberPlate);

                var tryAddH = nInventory.TryAdd(player, item);
                if (tryAddH == -1)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Недостаточно места в инвентаре", 3000);
                    return;
                }
                //else if (tryAddH == -2)
                //{
                //    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Недостаточно места в инвентаре (вес)", 3000);
                //    return;
                //}

                nInventory.Add(player, item);
                vData.Number = "";
                vehicle.NumberPlate = "";
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы сняли номерные знаки {item.Data} с машины", 3000);
                VehicleManager.Save(vData.ID);
            }
            catch (Exception e)
            {
                Log.Write("TakeNumber: " + e.StackTrace + "\n" + e.StackTrace, nLog.Type.Error);
            }
        }

        public static void PutNumber(Player player, Vehicle vehicle) // Установить
        {
            try
            {
                if (!vehicle.HasData("ACCESS") || (vehicle.GetData<string>("ACCESS") != "PERSONAL" && vehicle.GetData<string>("ACCESS") != "GARAGE" && vehicle.GetData<string>("ACCESS") != "FAMILY"))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Нельзя установить номера на эту машины", 3000);
                    return;
                }

                if (!Vehicles.ContainsKey(vehicle.GetData<int>("ID")))
                    return;

                VehicleData vData = VehicleManager.Vehicles[vehicle.GetData<int>("ID")];

                if (vData.OwnerID != Main.Players[player].UUID)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы не можете установить номера на эту машины", 3000);
                    return;
                }

                if (vData.Number != "")
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Номера уже установлены в машине", 3000);
                    return;
                }


                List<nItem> items = nInventory.FindAll(Main.Players[player].UUID, ItemType.NumberPlate);
                if (items.Count == 0)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "У вас нету номерных знаков", 3000);
                    return;
                }
                List<ListSystem.Item> numbers = new List<ListSystem.Item>();
                for (int i = 0; i < items.Count; i++)
                    numbers.Add(new ListSystem.Item(items[i].Data.ToString(), items[i]));

                ListSystem.Open(player, "Выбор номерного знака", new ListSystem.List((obj) => {
                    nItem number = (nItem)obj;
                    nInventory.Remove(player, number);
                    vData.Number = number.Data.ToString();
                    vehicle.NumberPlate = number.Data.ToString();
                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы установили номерные знаки {number.Data} на машину", 3000);
                    VehicleManager.Save(vData.ID);
                }, numbers));
            }
            catch (Exception e)
            {
                Log.Write("PutNumber: " + e.StackTrace + "\n" + e.StackTrace, nLog.Type.Error);
            }
        }

        #endregion

        public static void FracApplyCustomization(Vehicle veh, int fraction)
        {
            try
            {
                if (veh != null)
                {
                    if (!Configs.FractionVehicles[fraction].ContainsKey(veh.NumberPlate)) return;

                    FractionVehicleCustomization data = Configs.FractionVehicles[fraction][veh.NumberPlate].Components;

                    if (data.NeonColor.Alpha != 0)
                    {
                        NAPI.Vehicle.SetVehicleNeonState(veh, true);
                        NAPI.Vehicle.SetVehicleNeonColor(veh, data.NeonColor.Red, data.NeonColor.Green, data.NeonColor.Blue);
                    }

                    veh.SetMod(0, data.Spoiler);
                    veh.SetMod(1, data.FrontBumper);
                    veh.SetMod(2, data.RearBumper);
                    veh.SetMod(3, data.SideSkirt);
                    veh.SetMod(4, data.Muffler);
                    veh.SetMod(6, data.Lattice);
                    veh.SetMod(7, data.Hood);
                    veh.SetMod(8, data.Wings);
                    veh.SetMod(9, data.RearWings);
                    veh.SetMod(10, data.Roof);
                    veh.SetMod(11, data.Engine);
                    veh.SetMod(12, data.Brakes);
                    veh.SetMod(13, data.Transmission);
                    veh.SetMod(14, data.Horn);
                    veh.SetMod(15, data.Suspension);
                    veh.SetMod(18, data.Turbo);
                    veh.SetMod(48, data.Vinyls);

                    veh.WindowTint = data.WindowTint;
                    veh.NumberPlateStyle = data.NumberPlate;

                    if (data.Headlights >= 0)
                    {
                        veh.SetMod(22, 0);
                        veh.SetSharedData("hlcolor", data.Headlights);
                        Trigger.ClientEventInRange(veh.Position, 250f, "VehStream_SetVehicleHeadLightColor", veh.Handle, data.Headlights);
                    }
                    else
                    {
                        veh.SetMod(22, -1);
                        veh.SetSharedData("hlcolor", 0);
                    }

                    veh.WheelType = data.Wheels.Type;
                    veh.SetMod(23, data.Wheels.Id);
                }
            }
            catch (Exception e) { Log.Write("ApplyCustomization: " + e.StackTrace, nLog.Type.Error); }
        }

        public static void ApplyCustomization(Vehicle veh, Player player = null)
        {
            try
            {
                if (veh != null)
                {
                    if (!veh.HasData("ID")) return;
                    int id = veh.GetData<int>("ID");

                    if (!Vehicles.ContainsKey(id)) return;

                    VehicleCustomization data = Vehicles[id].Components;

                    NAPI.Task.Run(() =>
                    {

                        var vehicleColors = new Dictionary<string, object>();
                        NAPI.Vehicle.SetVehicleCustomPrimaryColor(veh, data.PrimColor.Red, data.PrimColor.Green, data.PrimColor.Blue);
                        NAPI.Vehicle.SetVehicleCustomSecondaryColor(veh, data.SecColor.Red, data.SecColor.Green, data.SecColor.Blue);


                        vehicleColors.Add("PrimColor", data.PrimColor);
                        vehicleColors.Add("SecColor", data.SecColor);

                        NAPI.Vehicle.SetVehiclePearlescentColor(veh, data.PearlColor);
                        NAPI.Vehicle.SetVehicleWheelColor(veh, data.WheelsColor);

                        vehicleColors.Add("PearlColor", data.PearlColor);
                        vehicleColors.Add("WheelsColor", data.WheelsColor);

                        //Log.Debug("Vehicle "+Vehicles[id].Model+" COLOR: Primary: R:"+data.PrimColor.Red+" G:"+data.PrimColor.Green+" B:"+data.PrimColor.Blue);
                        //Log.Debug("Vehicle "+Vehicles[id].Model+" COLOR: Secondary: R:"+data.SecColor.Red+" G:"+data.SecColor.Green+" B:"+data.SecColor.Blue);
                        //Log.Debug("Vehicle "+Vehicles[id].Model+" COLOR: Pearl:"+data.PearlColor);
                        //Log.Debug("Vehicle "+Vehicles[id].Model+" COLOR: WheelColor:"+data.WheelsColor);
                        //Log.Debug("Vehicle "+Vehicles[id].Model+ " COLORTYPE: PrimModColor:" + data.PrimModColor);
                        //Log.Debug("Vehicle "+Vehicles[id].Model+ " COLORTYPE: SecModColor:" + data.SecModColor);

                        if (data.PrimModColor == -1) data.PrimModColor = 0;
                        if (data.SecModColor == -1) data.SecModColor = 0;

                        vehicleColors.Add("PrimModColor", data.PrimModColor);
                        vehicleColors.Add("SecModColor", data.SecModColor);

                        veh.SetSharedData("VehicleColors", vehicleColors);

                        if (data.NeonColor.Alpha != 0)
                        {
                            NAPI.Vehicle.SetVehicleNeonState(veh, true);
                            NAPI.Vehicle.SetVehicleNeonColor(veh, data.NeonColor.Red, data.NeonColor.Green, data.NeonColor.Blue);
                            veh.SetSharedData("NeonColor", data.NeonColor);

                            //Log.Debug("Vehicle "+Vehicles[id].Model+" COLOR: NEON: R:"+data.NeonColor.Red+" G:"+data.NeonColor.Green+" B:"+data.NeonColor.Blue);
                        } else
                        {
                            NAPI.Vehicle.SetVehicleNeonState(veh, false);
                            NAPI.Vehicle.SetVehicleNeonColor(veh, data.NeonColor.Red, data.NeonColor.Green, data.NeonColor.Blue);
                            veh.SetSharedData("NeonColor", data.NeonColor);
                        }



                        if (data.Headlights >= 0)
                        {
                            veh.SetMod(22, 0);
                            veh.SetSharedData("hlcolor", data.Headlights);
                            Trigger.ClientEventInRange(veh.Position, 250f, "VehStream_SetVehicleHeadLightColor", veh.Handle, data.Headlights);
                        }
                        else
                        {
                            veh.SetMod(22, -1);
                            veh.SetSharedData("hlcolor", 0);
                        }
                    }, 200);

                    veh.SetMod(0, data.Spoiler);
                    veh.SetMod(1, data.FrontBumper);
                    veh.SetMod(2, data.RearBumper);
                    veh.SetMod(3, data.SideSkirt);
                    veh.SetMod(4, data.Muffler);
                    veh.SetMod(6, data.Lattice);
                    veh.SetMod(7, data.Hood);
                    veh.SetMod(8, data.Wings);
                    veh.SetMod(9, data.RearWings);
                    veh.SetMod(10, data.Roof);
                    veh.SetMod(11, data.Engine);
                    veh.SetMod(12, data.Brakes);
                    veh.SetMod(13, data.Transmission);
                    veh.SetMod(14, data.Horn);
                    veh.SetMod(15, data.Suspension);
                    veh.SetMod(18, data.Turbo);
                    veh.SetMod(48, data.Vinyls);

                    veh.WindowTint = data.WindowTint;
                    veh.NumberPlateStyle = data.NumberPlate;

                    veh.WheelType = data.WheelsType;
                    veh.SetMod(23, data.Wheels);

                    VehicleStreaming.SetVehicleDirt(veh, Vehicles[id].Dirt);

                    if (player != null) {
                        //Log.Debug("CLIENT::Vehicle:applyCustomization TRIGGERED! " + JsonConvert.SerializeObject(data), nLog.Type.Error);
                        Trigger.ClientEvent(player, "CLIENT::Vehicle:applyCustomization", veh, JsonConvert.SerializeObject(data));
                    }
                }
            }
            catch (Exception e) { Log.Write("ApplyCustomization: " + e.StackTrace, nLog.Type.Error); }
        }

        [RemoteEvent("setAnchor")]
        public static void Event_SetAnchor(Player player, bool toggle)
        {
            if (player.IsInVehicle)
            {
                player.SetMyData("ANCHOR", toggle);
                if (player.GetMyData<bool>("ANCHOR"))
                {
                    //Commands.RPChat("me", player, "опустил(а) якорь");
                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Вы опустили якорь", 3000);
                }
                else
                {
                    //Commands.RPChat("me", player, "поднял(а) якорь");
                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Вы подняли якорь", 3000);
                }
                player.Vehicle.SetSharedData("boatAnchor", toggle);
                Trigger.PlayerEventInRange(player.Position, 250f, "VehStream_SetVehicleAnchor", player.Vehicle.Handle, toggle);

            }
        }

        public static Vehicle GetNearestVehicle(Vector3 position, int radius, uint dimension = 0)
        {
            Vehicle nearest_vehicle = null;
            foreach (Vehicle v in NAPI.Pools.GetAllVehicles())
            {
                if (v.Dimension != dimension) continue;
                if (nearest_vehicle == null && position.DistanceTo(v.Position) < radius)
                {
                    nearest_vehicle = v;
                    continue;
                }
                else if (nearest_vehicle != null)
                {
                    if (position.DistanceTo(v.Position) < position.DistanceTo(nearest_vehicle.Position))
                    {
                        nearest_vehicle = v;
                        continue;
                    }
                }
            }
            return nearest_vehicle;
        }

        public static Vehicle GetNearestVehicle(Player player, int radius)
        {
            return GetNearestVehicle(player.Position, radius, player.Dimension);
        }

        public static bool GetVehicleDoorsStatus(Vehicle vehicle)
        {
            if (Core.VehicleStreaming.GetLockState(vehicle))
                return false;
            else
                return true;
        }

        public static void ChangeVehicleDoors(Player player, Vehicle vehicle)
        {
            switch (NAPI.Data.GetEntityData(vehicle, "ACCESS"))
            {
                case "HOTEL":
                    if (NAPI.Data.GetEntityData(vehicle, "OWNER") != player && Main.Players[player].AdminLVL < 3)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У Вас нет ключей от этого транспорта", 3000);
                        return;
                    }
                    if (Core.VehicleStreaming.GetLockState(vehicle))
                    {
                        Core.VehicleStreaming.SetLockStatus(vehicle, false);
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы открыли двери машины", 3000);
                    }
                    else
                    {
                        Core.VehicleStreaming.SetLockStatus(vehicle, true);
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы закрыли двери машины", 3000);
                    }
                    break;
                case "RENT":
                    if (NAPI.Data.GetEntityData(vehicle, "DRIVER") != player && Main.Players[player].AdminLVL < 3)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У Вас нет ключей от этого транспорта", 3000);
                        return;
                    }
                    if (Core.VehicleStreaming.GetLockState(vehicle))
                    {
                        Core.VehicleStreaming.SetLockStatus(vehicle, false);
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы открыли двери машины", 3000);
                    }
                    else
                    {
                        Core.VehicleStreaming.SetLockStatus(vehicle, true);
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы закрыли двери машины", 3000);
                    }
                    break;
                case "LOADER":
                    if (NAPI.Data.GetEntityData(vehicle, "DRIVER") != player && Main.Players[player].AdminLVL < 3)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У Вас нет ключей от этого транспорта", 3000);
                        return;
                    }
                    if (Core.VehicleStreaming.GetLockState(vehicle))
                    {
                        Core.VehicleStreaming.SetLockStatus(vehicle, false);
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы открыли двери машины", 3000);
                    }
                    else
                    {
                        Core.VehicleStreaming.SetLockStatus(vehicle, true);
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы закрыли двери машины", 3000);
                    }
                    break;
                case "BUILDRENT":
                    if (NAPI.Data.GetEntityData(vehicle, "DRIVER") != player && Main.Players[player].AdminLVL < 3)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У Вас нет ключей от этого транспорта", 3000);
                        return;
                    }
                    if (Core.VehicleStreaming.GetLockState(vehicle))
                    {
                        Core.VehicleStreaming.SetLockStatus(vehicle, false);
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы открыли двери машины", 3000);
                    }
                    else
                    {
                        Core.VehicleStreaming.SetLockStatus(vehicle, true);
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы закрыли двери машины", 3000);
                    }
                    break;
                case "WORK":
                    if (player.GetMyData<Vehicle>("WORK") != vehicle && Main.Players[player].AdminLVL < 3)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У Вас нет ключей от этого транспорта", 3000);
                        return;
                    }
                    if (Core.VehicleStreaming.GetLockState(vehicle))
                    {
                        Core.VehicleStreaming.SetLockStatus(vehicle, false);
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы открыли двери машины", 3000);
                    }
                    else
                    {
                        Core.VehicleStreaming.SetLockStatus(vehicle, true);
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы закрыли двери машины", 3000);
                    }
                    break;
                case "PERSONAL":
                    bool access = canAccessByNumber(player, vehicle.GetData<int>("ID"));
                    if (!access && Main.Players[player].AdminLVL < 3)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У Вас нет ключей от этого транспорта", 3000);
                        return;
                    }

                    if (Core.VehicleStreaming.GetLockState(vehicle))
                    {
                        Core.VehicleStreaming.SetLockStatus(vehicle, false);
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы открыли двери машины", 3000);
                        return;
                    }
                    else
                    {
                        Core.VehicleStreaming.SetLockStatus(vehicle, true);
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы закрыли двери машины", 3000);
                        return;
                    }
                case "GARAGE":

                    access = canAccessByNumber(player, vehicle.GetData<int>("ID"));
                    if (!access && Main.Players[player].AdminLVL < 3)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У Вас нет ключей от этого транспорта", 3000);
                        return;
                    }

                    if (Core.VehicleStreaming.GetLockState(vehicle))
                    {
                        Core.VehicleStreaming.SetLockStatus(vehicle, false);
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы открыли двери машины", 3000);
                        return;
                    }
                    else
                    {
                        Core.VehicleStreaming.SetLockStatus(vehicle, true);
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы закрыли двери машины", 3000);
                        return;
                    }
                case "FAMILY":
                    if (NAPI.Data.GetEntityData(vehicle, "FAMILY") != Main.Players[player].FamilyCID && Main.Players[player].AdminLVL < 3)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У вас нет ключа от этого транспорта", 3000);
                        return;
                    }
                    if (Core.VehicleStreaming.GetLockState(vehicle))
                    {
                        Core.VehicleStreaming.SetLockStatus(vehicle, false);
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы открыли двери машины", 3000);
                    }
                    else
                    {
                        Core.VehicleStreaming.SetLockStatus(vehicle, true);
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы закрыли двери машины", 3000);
                    }
                    break;
                case "ADMIN":
                    if (Main.Players[player].AdminLVL == 0)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У Вас нет ключей от этого транспорта", 3000);
                        return;
                    }

                    if (Core.VehicleStreaming.GetLockState(vehicle))
                    {
                        Core.VehicleStreaming.SetLockStatus(vehicle, false);
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы открыли двери машины", 3000);
                        return;
                    }
                    else
                    {
                        Core.VehicleStreaming.SetLockStatus(vehicle, true);
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы закрыли двери машины", 3000);
                        return;
                    }
                default:
                    return;
            }
            return;
        }
        public static bool canAccessByNumber(Player player, int number)
        {
            List<nItem> items = nInventory.Items[Main.Players[player].UUID];

            bool access = false;
            if (VehicleManager.Vehicles[number].Holder.Equals(player.Name)) access = true;
            return access;
        }

        // ///// need refactoring //// //
        public static void onClientEvent(Player sender, string eventName, params object[] args)
        {
            switch (eventName)
            {

                case "engineCarPressed":
                    #region Engine button
                    if (!NAPI.Player.IsPlayerInAnyVehicle(sender)) return;
                    if (sender.VehicleSeat != 0)
                    {
                        Notify.Send(sender, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы должны быть на водительском месте", 3000);
                        return;
                    }
                    Vehicle vehicle = sender.Vehicle;
                    if (vehicle.Class == 13 && Main.Players[sender].InsideGarageID == -1) return;

                    int fuel = vehicle.GetSharedData<int>("PETROL");
                    if (fuel <= 0)
                    {
                        Notify.Send(sender, NotifyType.Error, NotifyPosition.BottomCenter, $"Топливный бак пуст, невозможно завести машину", 3000);
                        return;
                    }
                    switch (NAPI.Data.GetEntityData(vehicle, "ACCESS"))
                    {
                        case "HOTEL":
                            if (NAPI.Data.GetEntityData(vehicle, "OWNER") != sender && Main.Players[sender].AdminLVL < 3)
                            {
                                Notify.Send(sender, NotifyType.Error, NotifyPosition.BottomCenter, $"У Вас нет ключей от этого транспорта", 3000);
                                return;
                            }
                            if (Core.VehicleStreaming.GetEngineState(vehicle))
                            {
                                Core.VehicleStreaming.SetEngineState(vehicle, false);
                                Notify.Send(sender, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы заглушили двигатель машины", 3000);
                            }
                            else
                            {
                                Core.VehicleStreaming.SetEngineState(vehicle, true);
                                Notify.Send(sender, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы завели машину", 3000);
                            }
                            break;
                        case "SCHOOL":
                            if (NAPI.Data.GetEntityData(vehicle, "DRIVER") != sender && Main.Players[sender].AdminLVL < 3)
                            {
                                Notify.Send(sender, NotifyType.Error, NotifyPosition.BottomCenter, $"У Вас нет ключей от этого транспорта", 3000);
                                return;
                            }
                            if (Core.VehicleStreaming.GetEngineState(vehicle))
                            {
                                Core.VehicleStreaming.SetEngineState(vehicle, false);
                                Notify.Send(sender, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы заглушили двигатель машины", 3000);
                            }
                            else
                            {
                                Core.VehicleStreaming.SetEngineState(vehicle, true);
                                Notify.Send(sender, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы завели машину", 3000);
                            }
                            break;
                        case "beltCarPressed":
                            if (!NAPI.Player.IsPlayerInAnyVehicle(sender)) return;

                            bool beltstate = Convert.ToBoolean(args[0]);

                            if (!beltstate) Commands.RPChat("me", sender, "пристегнул(а) ремень безопасности");
                            else Commands.RPChat("me", sender, "отслегнул(а) ремень безопасности");

                            break;
                        case "RENT":
                        case "RENTVEHICLE":
                        case "RACE":
                        case "PRIZEVEHICLE":
                            if (NAPI.Data.GetEntityData(vehicle, "DRIVER") != sender && Main.Players[sender].AdminLVL < 3 && (NAPI.Data.GetEntityData(vehicle, "ACCESS") != "RENTVEHICLE" && NAPI.Data.GetEntityData(vehicle, "DRIVERNAME") != sender.Name))
                            {
                                Notify.Send(sender, NotifyType.Error, NotifyPosition.BottomCenter, $"У Вас нет ключей от этого транспорта", 3000);
                                return;
                            }
                            if (Core.VehicleStreaming.GetEngineState(vehicle))
                            {
                                Core.VehicleStreaming.SetEngineState(vehicle, false);
                                Notify.Send(sender, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы заглушили двигатель машины", 3000);
                            }
                            else
                            {
                                Core.VehicleStreaming.SetEngineState(vehicle, true);
                                Notify.Send(sender, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы завели машину", 3000);
                            }
                            break;
                        case "LOADER":
                            if (vehicle.GetData<Player>("DRIVER") != sender)
                            {
                                Notify.Send(sender, NotifyType.Error, NotifyPosition.BottomCenter, $"У Вас нет ключей от этого транспорта", 3000);
                                return;
                            }
                            if (Core.VehicleStreaming.GetEngineState(vehicle))
                            {
                                Core.VehicleStreaming.SetEngineState(vehicle, false);
                                Notify.Send(sender, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы заглушили двигатель машины", 3000);
                            }
                            else
                            {
                                Core.VehicleStreaming.SetEngineState(vehicle, true);
                                Notify.Send(sender, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы завели машину", 3000);
                            }
                            break;
                        case "BUILDRENT":
                            if (vehicle.GetData<Player>("DRIVER") != sender)
                            {
                                Notify.Send(sender, NotifyType.Error, NotifyPosition.BottomCenter, $"У Вас нет ключей от этого транспорта", 3000);
                                return;
                            }
                            if (Core.VehicleStreaming.GetEngineState(vehicle))
                            {
                                Core.VehicleStreaming.SetEngineState(vehicle, false);
                                Notify.Send(sender, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы заглушили двигатель машины", 3000);
                            }
                            else
                            {
                                Core.VehicleStreaming.SetEngineState(vehicle, true);
                                Notify.Send(sender, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы завели машину", 3000);
                            }
                            break;
                        case "WORK":
                            if (vehicle.HasData("TRUCKER_BREAK")) return;
                            if (sender.GetMyData<Vehicle>("WORK") != vehicle && Main.Players[sender].AdminLVL < 3)
                            {
                                Notify.Send(sender, NotifyType.Error, NotifyPosition.BottomCenter, $"У Вас нет ключей от этого транспорта", 3000);
                                return;
                            }
                            if (Core.VehicleStreaming.GetEngineState(vehicle))
                            {
                                Core.VehicleStreaming.SetEngineState(vehicle, false);
                                Notify.Send(sender, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы заглушили двигатель машины", 3000);
                            }
                            else
                            {
                                Core.VehicleStreaming.SetEngineState(vehicle, true);
                                Notify.Send(sender, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы завели машину", 3000);
                            }
                            break;
                        case "FRACTION":
                            if (Main.Players[sender].Fraction.FractionID != NAPI.Data.GetEntityData(vehicle, "FRACTION"))
                            {
                                Notify.Send(sender, NotifyType.Error, NotifyPosition.BottomCenter, $"У Вас нет ключей от этого транспорта", 3000);
                                return;
                            }
                            if (Core.VehicleStreaming.GetEngineState(vehicle))
                            {
                                Core.VehicleStreaming.SetEngineState(vehicle, false);
                                Notify.Send(sender, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы заглушили двигатель машины", 3000);
                            }
                            else
                            {
                                if (TrunkCoords.ContainsKey((VehicleHash)vehicle.Model) && vehicle.HasData("TRUNK_MARKER"))
                                {
                                    var marker = vehicle.GetData<Marker>("TRUNK_MARKER");
                                    var shape = vehicle.GetData<ColShape>("TRUNK_SHAPE");
                                    var label = vehicle.GetData<TextLabel>("TRUNK_LABEL");
                                    marker.Delete();
                                    shape.Delete();
                                    label.Delete();
                                    vehicle.ResetData("TRUNK_MARKER");
                                    vehicle.ResetData("TRUNK_SHAPE");
                                    vehicle.ResetData("TRUNK_LABEL");
                                    VehicleStreaming.SetDoorState(vehicle, DoorID.DoorTrunk, DoorState.DoorClosed);
                                }
                                Core.VehicleStreaming.SetEngineState(vehicle, true);
                                Notify.Send(sender, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы завели машину", 3000);
                            }
                            break;
                        case "FAMILY":
                            if (Main.Players[sender].FamilyCID != NAPI.Data.GetEntityData(vehicle, "FAMILY"))
                            {
                                Notify.Send(sender, NotifyType.Error, NotifyPosition.BottomCenter, $"У Вас нет ключей от этого транспорта", 3000);
                                return;
                            }

                            if (!vehicle.HasData("FAMILY")) return;

                            if(!vehicle.GetData<bool>("ISTAKED") && sender.HasMyData("IN_FAM_GARAGE"))
                            {
                                Family family = Family.GetFamilyToCid(sender);
                                FamilyHouse house = FamilyHouseManager.FamilyHouse.FirstOrDefault(h => h.Owner == family.Name);

                                if (sender.GetMyData<FamilyHouse>("IN_FAM_GARAGE") == house)
                                {
                                    vehicle.Position = house.GaragePosition;
                                    NAPI.Task.Run(() => {
                                        sender.Dimension = 0;
                                        vehicle.Dimension = 0;
                                        sender.SetIntoVehicle(vehicle, 0);
                                    }, 500);
                                    vehicle.SetData("ISTAKED", true);

                                    VehicleManager.Vehicles[vehicle.GetData<int>("ID")].FamilyInGarage = false;

                                    Log.Debug("FamilyCAR TP: "+ house.GaragePosition);
                                }
                            }

                            if (vehicle.GetData<string>("FAMILY") == Main.Players[sender].FamilyCID)
                            {
                                if (Core.VehicleStreaming.GetEngineState(vehicle))
                                {
                                    Core.VehicleStreaming.SetEngineState(vehicle, false);
                                    Notify.Send(sender, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы заглушили двигатель машины", 3000);
                                }
                                else
                                {
                                    Core.VehicleStreaming.SetEngineState(vehicle, true);
                                    Notify.Send(sender, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы завели машину", 3000);
                                }
                            }


                            if (vehicle.HasData("CAR_ROOM"))
                            {
                                ParkPlace.ReleaseParkPlace(vehicle);
                                int bId = vehicle.GetData<int>("CAR_ROOM");
                                int place = vehicle.GetData<int>("CAR_PLACE");

                                CarRoom.RemoveCar(bId, place);

                                Trigger.ClientEvent(sender, "deleteWorkBlip");

                                vehicle.ResetData("CAR_ROOM");
                                vehicle.ResetData("CAR_PLACE");
                            }

                            break;
                        case "PERSONAL":
                            if (vehicle.HasData("TRUCKER_BREAK")) return;
                            bool access = canAccessByNumber(sender, vehicle.GetData<int>("ID"));
                            if (!access && Main.Players[sender].AdminLVL < 3)
                            {
                                Notify.Send(sender, NotifyType.Error, NotifyPosition.BottomCenter, $"У Вас нет ключей от этого транспорта", 3000);
                                return;
                            }

                            if (Core.VehicleStreaming.GetEngineState(vehicle))
                            {
                                Core.VehicleStreaming.SetEngineState(vehicle, false);
                                Notify.Send(sender, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы заглушили двигатель машины", 3000);
                            }
                            else
                            {
                                Core.VehicleStreaming.SetEngineState(vehicle, true);
                                Notify.Send(sender, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы завели машину", 3000);
                            }

                            if (sender.HasMyData("P_MARKER"))
                            {
                                sender.ResetMyData("P_MARKER");

                                Trigger.ClientEvent(sender, "deleteWorkBlip");
                                Trigger.ClientEvent(sender, "deleteParkCheckpoint");
                            }

                            if (vehicle.HasData("CAR_ROOM"))
                            {
                                ParkPlace.ReleaseParkPlace(vehicle);
                                int bId = vehicle.GetData<int>("CAR_ROOM");
                                int place = vehicle.GetData<int>("CAR_PLACE");

                                CarRoom.RemoveCar(bId, place);

                                Trigger.ClientEvent(sender, "deleteWorkBlip");

                                vehicle.ResetData("CAR_ROOM");
                                vehicle.ResetData("CAR_PLACE");
                            }

                            break;
                        case "GARAGE":
                            if (Main.Players[sender].InsideGarageID == -1) return;
                            string number = NAPI.Vehicle.GetVehicleNumberPlate(vehicle);

                            Houses.Garage garage = Houses.GarageManager.Garages[Main.Players[sender].InsideGarageID];

                            garage.GetVehicleFromGarage(sender, vehicle.GetData<int>("ID"));
                            break;
                        case "QUEST":
                        case "MAFIADELIVERY":
                        case "GANGDELIVERY":
                            if (Core.VehicleStreaming.GetEngineState(vehicle))
                            {
                                Core.VehicleStreaming.SetEngineState(vehicle, false);
                                Notify.Send(sender, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы заглушили двигатель машины", 3000);
                            }
                            else
                            {
                                Core.VehicleStreaming.SetEngineState(vehicle, true);
                                Notify.Send(sender, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы завели машину", 3000);
                            }
                            break;
                        case "ADMIN":
                            if (Main.Players[sender].AdminLVL == 0)
                            {
                                Notify.Send(sender, NotifyType.Error, NotifyPosition.BottomCenter, $"У Вас нет ключей от этого транспорта", 3000);
                                return;
                            }
                            if (Core.VehicleStreaming.GetEngineState(vehicle))
                            {
                                Core.VehicleStreaming.SetEngineState(vehicle, false);
                                Notify.Send(sender, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы заглушили двигатель машины", 3000);
                            }
                            else
                            {
                                Core.VehicleStreaming.SetEngineState(vehicle, true);
                                Notify.Send(sender, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы завели машину", 3000);
                            }
                            break;
                    }
                    //if (Core.VehicleStreaming.GetEngineState(vehicle)) Commands.RPChat("me", sender, "завел(а) транспортное средство");
                    //else Commands.RPChat("me", sender, "заглушил(а) транспортное средство");
                    return;
                #endregion Engine button
                case "lockCarPressed":
                    #region inVehicle
                    if (NAPI.Player.IsPlayerInAnyVehicle(sender) && sender.VehicleSeat == 0)
                    {
                        vehicle = sender.Vehicle;
                        ChangeVehicleDoors(sender, vehicle);
                        return;
                    }
                    #endregion
                    #region outVehicle
                    vehicle = getNearestVehicle(sender, 10);
                    if (vehicle != null)
                        ChangeVehicleDoors(sender, vehicle);
                    #endregion
                    break;
            }
        }
        // ////////////////////////// //

        [ServerEvent(Event.VehicleDeath)]
        public void Event_vehicleDeath(Vehicle vehicle)
        {
            try
            {
                if (!vehicle.HasData("ACCESS") || vehicle.GetData<string>("ACCESS") == "ADMIN") return;
                Log.Debug("Udalena tachka mb s pod igroka3 " + vehicle.DisplayName);
                string access = vehicle.GetData<string>("ACCESS");
                switch (access)
                {
                    case "RACE":
                        Player raceDriver = vehicle.GetData<Player>("DRIVER");
                        raceDriver.SetMyData("RACE_DESTROYED_CAR", true);
                        Race.playerFailedRace(raceDriver);
                        break;
                    case "CARTHEFT":

                        if (vehicle.HasData("CARTHEFT"))
                        {
                            CarTheft theft = vehicle.GetData<CarTheft>("CARTHEFT");

                            theft.FailEnd();
                        }
                        break;
                    case "RENTVEHICLE":
                        if (vehicle.HasData("DRIVER"))
                        {
                            Player driver = vehicle.GetData<Player>("DRIVER");

                            if(driver != null && Main.Players.ContainsKey(driver))
                            {
                                RentcarSystem.CancelRent(driver);
                            }
                            else
                            {
                                NAPI.Task.Run(() =>
                                {
                                    try
                                    {
                                        NAPI.Entity.DeleteEntity(vehicle);
                                    }
                                    catch (Exception e) { Log.Write(e.StackTrace, nLog.Type.Error); }
                                });
                            }

                        }
                        break;
                    case "PERSONAL":
                        {
                            Player owner = vehicle.GetData<Player>("OWNER");
                            string number = vehicle.NumberPlate;

                            if (vehicle.HasData("ON_WORK"))
                            {
                                if (owner.HasMyData("ORDER"))
                                {
                                    Jobs.Truckers.cancelOrder(owner);
                                    Notify.Send(owner, NotifyType.Alert, NotifyPosition.BottomCenter, "Заказ отменен", 3000);
                                }

                                vehicle.ResetData("ON_WORK");
                                vehicle.ResetData("DRIVER");
                                owner.SetMyData<Vehicle>("WORK", null);
                                owner.SetMyData("ON_WORK", false);
                            }

                            Notify.Send(owner, NotifyType.Alert, NotifyPosition.BottomCenter, "Ваша машина уничтожена", 3000);

                            Vehicles[vehicle.GetData<int>("ID")].Items = vehicle.GetData<List<nItem>>("ITEMS");
                            Vehicles[vehicle.GetData<int>("ID")].Health = 0;

                            vehicle.Delete();

                            if (GarageManager.vehiclesOutPark.ContainsKey(vehicle.GetData<int>("ID")))
                            {
                                GarageManager.vehiclesOutPark.Remove(vehicle.GetData<int>("ID"));
                            }
                        }
                        return;
                    case "FAMILY":
                        {
                            vehicle.Delete();
                            //
                            //
                            //
                            //
                            //vehicle.SetData("ISTAKED", false);
                        }
                        return;
                    case "WORK":
                        Player player = vehicle.GetData<Player>("DRIVER");
                        if (player != null)
                        {
                            string paymentMsg = (player.GetMyData<int>("PAYMENT") == 0) ? "" : $"Вы получили зарплату в {player.GetMyData<int>("PAYMENT")}$";
                            Notify.Send(player, NotifyType.Alert, NotifyPosition.BottomCenter, "Ваш рабочий транспорт был уничтожен. " + paymentMsg, 3000);
                            player.SetMyData("ON_WORK", false);
                            Customization.ApplyCharacter(player);
                        }
                        string work = vehicle.GetData<string>("TYPE");
                        switch (work)
                        {
                            case "BUS":
                                Jobs.Bus.respawnBusCar(vehicle);
                                return;
                            //case "MOWER":
                            //    Jobs.Lawnmower.respawnCar(vehicle);
                            //    //player.SetMyData("ON_WORK", false);
                            //    player.SetMyData<Vehicle>("WORK", null);
                            //    Trigger.ClientEvent(player, "deleteCheckpoint", 4, 0);
                            //    if (player.HasMyData("WORK_CAR_EXIT_TIMER"))
                            //    {
                            //        Timers.Stop(player.GetMyData<string>("WORK_CAR_EXIT_TIMER"));
                            //        player.ResetMyData("WORK_CAR_EXIT_TIMER");
                            //    }
                            //    //Customization.ApplyCharacter(player);
                            //    return;
                            case "TAXI":
                                if (player != null) Jobs.Taxi.cancelTaxi(player);
                                player.SetMyData<Vehicle>("WORK", null);
                                //Trigger.ClientEvent(player, "deleteCheckpoint", 10);
                                VehicleManager.WarpPlayerOutOfVehicle(player);
                                Jobs.Taxi.respawnCar(vehicle);
                                return;
                            case "MECHANIC":
                                if (player != null) Jobs.AutoMechanic.StopMechanicWork(player);
                                VehicleManager.WarpPlayerOutOfVehicle(player);
                                player.SetMyData<Vehicle>("WORK", null);
                                //Trigger.ClientEvent(player, "deleteCheckpoint", 10);

                               // Jobs.AutoMechanic.respawnCar(vehicle);
                                return;
                            case "TRUCKER":
                                if (player != null) Jobs.Truckers.cancelOrder(player);
                                player.SetMyData<Vehicle>("WORK", null);
                                //Trigger.ClientEvent(player, "deleteCheckpoint", 10);
                                VehicleManager.WarpPlayerOutOfVehicle(player);
                                Jobs.Truckers.respawnCar(vehicle);
                                return;
                            case "COLLECTOR":
                                player.SetMyData<Vehicle>("WORK", null);
                                VehicleManager.WarpPlayerOutOfVehicle(player);
                                Jobs.Collector.StopWork(player);
                                return;
                            case "GOPOSTAL":
                                Trigger.ClientEvent(player, "deleteWorkBlip");
                                BasicSync.AddAttachment(player, "prop_drug_package_02", true);
                                return;
                        }
                        return;
                    case "SCHOOL":
                        {
                            Player driver = vehicle.GetData<Player>("DRIVER");
                            NAPI.Task.Run(() =>
                            {
                                try
                                {
                                    NAPI.Entity.DeleteEntity(vehicle);
                                }
                                catch (Exception e) { Log.Write(e.StackTrace, nLog.Type.Error); }
                            });
                            Notify.Send(driver, NotifyType.Alert, NotifyPosition.BottomCenter, "Ваш транспорт был уничтожен. Вы провалили экзамен.", 3000);
                            Trigger.ClientEvent(driver, "deleteCheckpoint", 12, 0);
                            Trigger.ClientEvent(driver, "busUnRoute");
                            driver.ResetMyData("IS_DRIVING");
                            driver.ResetMyData("SCHOOLVEH");
                            driver.Dimension = 0;
                            Timers.Stop(driver.GetMyData<string>("SCHOOL_TIMER"));
                            driver.ResetMyData("SCHOOL_TIMER");
                        }
                        return;
                    case "FRACTION":
                        {

                             Configs.RespawnFractionCar(vehicle);
                        }
                        return;
                }
            }
            catch (Exception e) { Log.Write("VehicleDeath: " + e.StackTrace, nLog.Type.Error); }
        }

        public static string GetColorNameByID(int id)
        {
            if (ColorNames.ContainsKey(id))
                return ColorNames[id];
            return "";
        }

        public static string GetColorNameFromRgb(Color rgb)
        {
            ColorName closestMatch = null;
            int minMSE = int.MaxValue;
            int mse;
            foreach (var c in ColorList)
            {
                mse = c.computeMSE(rgb.Red, rgb.Green, rgb.Blue);
                if (mse < minMSE)
                {
                    minMSE = mse;
                    closestMatch = c;
                }
            }

            if (closestMatch != null)
                return closestMatch.GetName();
            else
                return "Цвет";
        }

        /*public static string Generate
         * Number()
        {
            string number;
            do
            {
                number = "";
                number += (char)Rnd.Next(0x0041, 0x005A);
                for (int i = 0; i < 3; i++)
                    number += (char)Rnd.Next(0x0030, 0x0039);
                number += (char)Rnd.Next(0x0041, 0x005A);

            } while (Vehicles.ContainsKey(number));
            return number;
        }*/

        public class VehicleCreate
        {
            public static GTANetworkAPI.Vehicle CreateVehicle(Player player, uint model, Vector3 pos, float rot, int color1, int color2, string numberPlate = "", byte alpha = byte.MaxValue, bool locked = false, bool engine = true, uint dimension = 0u)
            {
                Vehicle vehicle = NAPI.Vehicle.CreateVehicle(model, pos, rot, color1, color2, numberPlate, alpha, locked, engine, dimension);
                if (vehicle == null) Log.Debug("VEHICLE NOT CREATED FROM NAPI. CHECK THIS. MAYBE REFACTOR.", nLog.Type.Error);
                else createVehicle(player, vehicle, pos.X, pos.Y, pos.Z, RZ: rot);

                return vehicle;
            }

            public static GTANetworkAPI.Vehicle CreateVehicle(Player player, VehicleHash model, Vector3 pos, float rot, Color color1, Color color2, string numberPlate = "", byte alpha = byte.MaxValue, bool locked = false, bool engine = true, uint dimension = 0u)
            {
                Vehicle vehicle = NAPI.Vehicle.CreateVehicle(model, pos, rot, color1, color2, numberPlate, alpha, locked, engine, dimension);
                if (vehicle == null) Log.Debug("VEHICLE NOT CREATED FROM NAPI. CHECK THIS. MAYBE REFACTOR.", nLog.Type.Error);
                else createVehicle(player, vehicle, pos.X, pos.Y, pos.Z, RZ: rot);

                return vehicle;
            }

            public static GTANetworkAPI.Vehicle CreateVehicle(Player player, VehicleHash model, Vector3 pos, float rot, int color1, int color2, string numberPlate = "", byte alpha = byte.MaxValue, bool locked = false, bool engine = true, uint dimension = 0u)
            {
                Vehicle vehicle = NAPI.Vehicle.CreateVehicle(model, pos, rot, color1, color2, numberPlate, alpha, locked, engine, dimension);
                if (vehicle == null) Log.Debug("VEHICLE NOT CREATED FROM NAPI. CHECK THIS. MAYBE REFACTOR.", nLog.Type.Error);
                else createVehicle(player, vehicle, pos.X, pos.Y, pos.Z, RZ: rot);

                return vehicle;
            }

            public static GTANetworkAPI.Vehicle CreateVehicle(Player player, int model, Vector3 pos, float rot, int color1, int color2, string numberPlate = "", byte alpha = byte.MaxValue, bool locked = false, bool engine = true, uint dimension = 0u)
            {
                Vehicle vehicle = NAPI.Vehicle.CreateVehicle(model, pos, rot, color1, color2, numberPlate, alpha, locked, engine, dimension);
                if (vehicle == null) Log.Debug("VEHICLE NOT CREATED FROM NAPI. CHECK THIS. MAYBE REFACTOR.", nLog.Type.Error);
                else createVehicle(player, vehicle, pos.X, pos.Y, pos.Z, RZ: rot);

                return vehicle;
            }

            public static GTANetworkAPI.Vehicle CreateVehicle(Player player, int model, Vector3 pos, Vector3 rot, int color1, int color2, string numberPlate = "", byte alpha = byte.MaxValue, bool locked = false, bool engine = true, uint dimension = 0u)
            {
                Vehicle vehicle = NAPI.Vehicle.CreateVehicle(model, pos, rot, color1, color2, numberPlate, alpha, locked, engine, dimension);
                if (vehicle == null) Log.Debug("VEHICLE NOT CREATED FROM NAPI. CHECK THIS. MAYBE REFACTOR.", nLog.Type.Error);
                else createVehicle(player, vehicle, pos.X, pos.Y, pos.Z, rot.X, rot.Y, rot.Z);

                return vehicle;
            }

            public static GTANetworkAPI.Vehicle CreateVehicle(Player player, VehicleHash model, Vector3 pos, Vector3 rot, int color1, int color2, string numberPlate = "", byte alpha = byte.MaxValue, bool locked = false, bool engine = true, uint dimension = 0u)
            {
                Vehicle vehicle = NAPI.Vehicle.CreateVehicle(model, pos, rot, color1, color2, numberPlate, alpha, locked, engine, dimension);
                if (vehicle == null) Log.Debug("VEHICLE NOT CREATED FROM NAPI. CHECK THIS. MAYBE REFACTOR.", nLog.Type.Error);
                else createVehicle(player, vehicle, pos.X, pos.Y, pos.Z, rot.X, rot.Y, rot.Z);

                return vehicle;
            }

            private static void createVehicle(Player player, Vehicle vehicle, float X, float Y, float Z, float RX = 0, float RY = 0, float RZ = 0)
            {
                //CLIENT::vehicle:create
                Trigger.ClientEvent(player, "CLIENT::vehicle:create", vehicle, X, Y, Z, RX, RY, RZ);
            }

        }

        public class VehicleData
        {
            public int ID { get; set; }

            private Vehicle vehicle = null;
            private string number;
            public string Number { get => number; set { number = value; if (Vehicle != null) Vehicle.NumberPlate = value; } }
            public string Holder { get; set; }
            public int OwnerID { get; set; }
            public string Model { get; set; }
            public int Health { get; set; }
            public int Fuel { get; set; }
            public int Price { get; set; }
            public VehicleCustomization Components { get; set; }
            public List<nItem> Items { get; set; }
            public List<bool> Slots { get; set; }
            public float currentWeight { get; set; } = 0.0f;
            public string Position { get; set; }
            public string Rotation { get; set; }
            public int KeyNum { get; set; }
            public float Dirt { get; set; }
            public float Mileage { get; set; }
            public int Priority { get; set; } = 999;

            public OtherVehicleData OtherData { get; set; } = new OtherVehicleData();

            //public bool FamilySpawned { get; set; } = false; // временное решение я сегодня тупой

            public bool FamilyInGarage { get; set; } = true;

            public Vehicle Vehicle {
                get {
                    if (vehicle == null)
                        return vehicle;
                    if (vehicle.Exists)
                        return vehicle;
                    return null;
                }
                set => vehicle = value;
            }

            public TechnicalPassport Passport { get; set; }

            public bool IsOwner(Player player)
            {
                if (player == null || !Main.Players.ContainsKey(player))
                    return false;
                return Main.Players[player].UUID == OwnerID;
            }
        }

        public class TechnicalPassport
        {
            public string Number;
            public DateTime DateFrom;
            public DateTime DateTo;
            public string Color;
            public string Customization;

            public TechnicalPassport()
            {

            }
            public TechnicalPassport(VehicleData vehicle)
            {
                Number = vehicle.Number;
                DateFrom = DateTime.Now;
                DateTo = DateTime.Now.AddMonths(1);
                string primColor = GetColorNameFromRgb(vehicle.Components.PrimColor); //GetColorNameFromRgb(vehicle.Components.PrimColorRGB);
                string secColor = GetColorNameFromRgb(vehicle.Components.SecColor); //GetColorNameFromRgb(vehicle.Components.SecColorRGB);
                if (primColor == secColor)
                    Color = primColor;
                else
                    Color = primColor + ", " + secColor;

                Customization = vehicle.Components.GetTuning();
            }
        }

        public class VehicleCustomization
        {
            public Color PrimColor = new Color(0, 0, 0);
            public Color SecColor = new Color(0, 0, 0);
            public Color NeonColor = new Color(0, 0, 0, 0);

            public int PrimModColor = 0;
            public int SecModColor = 0;
            public int PearlColor = -1;

            public int Muffler = -1;
            public int SideSkirt = -1;
            public int Hood = -1;
            public int Spoiler = -1;
            public int Lattice = -1;
            public int Wings = -1;
            public int RearWings = -1;
            public int Roof = -1;
            public int Vinyls = -1;
            public int FrontBumper = -1;
            public int RearBumper = -1;

            public int Engine = -1;
            public int Turbo = -1;
            public int Horn = -1;
            public int Transmission = -1;
            public int WindowTint = 0;
            public int Suspension = -1;
            public int Brakes = -1;
            public int Headlights = -1;
            //public int HeadlightColor = 0;
            public int NumberPlate = 0;

            public int Wheels = -1;
            public int WheelsType = 0;
            public int WheelsColor = 0;



            public int Armor = -1;


            public bool WheelsArmor = true; // можно ли пробить шину
            public Color SmokeColor = new Color(255, 255, 255, 255);

            public List<int> NeonPosition = new List<int>();

            public string GetTuning()
            {
                List<string> tuning = new List<string>();
                if (Engine != -1)
                    tuning.Add($"Двигатель {Engine + 1}");
                if (Turbo != -1)
                    tuning.Add("Турбо тюнинг");
                if (Transmission != -1)
                {
                    if (Transmission == 0)
                        tuning.Add("Коробка: полу-спортивная");
                    if (Transmission == 1)
                        tuning.Add("Коробка: спортивная");
                    if (Transmission == 2)
                        tuning.Add("Коробка: гоночная");
                }
                if (Suspension != -1)
                {
                    if (Suspension == 0)
                        tuning.Add("Подвеска: заниженная");
                    if (Suspension == 1)
                        tuning.Add("Подвеска: полу-спортивная");
                    if (Suspension == 2)
                        tuning.Add("Подвеска: спортивная");
                    if (Suspension == 3)
                        tuning.Add("Подвеска: раллийная");
                }
                if (SideSkirt != -1)
                    tuning.Add("Пороги");
                if (Brakes != -1)
                {
                    if (Brakes == 0)
                        tuning.Add("Тормоза: полу-спортивные");
                    if (Brakes == 1)
                        tuning.Add("Тормоза: спортивные");
                    if (Brakes == 2)
                        tuning.Add("Тормоза: гоночные");
                }
                if (FrontBumper != -1)
                    tuning.Add("Передний бампер");
                if (RearBumper != -1)
                    tuning.Add("Задний бампер");
                if (Hood != -1)
                    tuning.Add("Капот");
                if (Spoiler != -1)
                    tuning.Add("Спойлер");
                if (Wings != -1)
                    tuning.Add("Крылья");
                if (Roof != -1)
                    tuning.Add("Крыша");

                return string.Join(", ", tuning);
            }

        }

        public class OtherVehicleData
        {
            public int ExclusiveNumber { get; set; } = -1;
        }

        internal class FractionVehicleCustomization
        {
            public Color PrimColorRGB = new Color(0, 0, 0);
            public Color SecColorRGB = new Color(0, 0, 0);
            public Color NeonColor = new Color(0, 0, 0, 0);

            public int PrimModColor = -1;
            public int SecModColor = -1;

            public int Muffler = -1;
            public int SideSkirt = -1;
            public int Hood = -1;
            public int Spoiler = -1;
            public int Lattice = -1;
            public int Wings = -1;
            public int RearWings = -1;
            public int Roof = -1;
            public int Vinyls = -1;
            public int FrontBumper = -1;
            public int RearBumper = -1;

            public int Engine = -1;
            public int Turbo = -1;
            public int Horn = -1;
            public int Transmission = -1;
            public int WindowTint = 0;
            public int Suspension = -1;
            public int Brakes = -1;
            public int Headlights = -1;
            //public int HeadlightColor = 0;
            public int NumberPlate = 0;
            public WheelsС Wheels = new WheelsС();



            public int Armor = -1;


            public bool WheelsArmor = true; // можно ли пробить шину
            public Color SmokeColor = new Color(255, 255, 255, 255);

            public List<int> NeonPosition = new List<int>();
            public class WheelsС
            {
                public int Id = -1;
                public int Type = 0;

                public WheelsС() { }

                public WheelsС(int id, int type)
                {
                    Id = id;
                    Type = type;
                }
            }

        }

        public static void changeOwner(string oldName, string newName)
        {
            List<int> toChange = new List<int>();
            lock (Vehicles)
            {
                foreach (KeyValuePair<int, VehicleData> vd in Vehicles)
                {
                    if (vd.Value.Holder != oldName) continue;
                    Log.Write($"The car was found! [{vd.Key}]");
                    toChange.Add(vd.Key);
                }
                foreach (int num in toChange)
                {
                    if (Vehicles.ContainsKey(num)) Vehicles[num].Holder = newName;
                }
                // // //
                //MySQL.Query($"UPDATE `vehicles` SET `holder`='{newName}' WHERE `holder`='{oldName}'");

                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = "UPDATE `vehicles` SET `holder`=@newholder WHERE `holder`=@oldholder";

                cmd.Parameters.AddWithValue("@newholder", newName);
                cmd.Parameters.AddWithValue("@oldholder", oldName);
                MySQL.Query(cmd);
            }
        }
    }

    class VehicleInventory : Script
    {
        public static nLog Log = new nLog("VehicleInventory");
        public static int CountSlots = 65;//TODO

        public static void Add(Vehicle vehicle, nItem item)
        {
            if (!vehicle.HasData("ITEMS")) return;
            if (!vehicle.HasData("SLOTS")) return;
            List<bool> slots = vehicle.GetData<List<bool>>("SLOTS");
            List<nItem> items = vehicle.GetData<List<nItem>>("ITEMS");

            int slot = nInventory.CheckAdd(items, item, slots);
            if (slot == -1)
            {
                Log.Write("Недостаточно места в инвентаре!", nLog.Type.Warn);
                return;
            }

            if (nInventory.ClothesItems.Contains(item.Type) || nInventory.WeaponsItems.Contains(item.Type)
                || nInventory.MeleeWeaponsItems.Contains(item.Type) || item.Type == ItemType.CarKey || item.Type == ItemType.KeyRing || item.Type == ItemType.NumberPlate)
            {
                items.Add(item);
                item.slot_id = slot;
                nInventory.FillSlot(slots, item, 5);
            } else
            {
                    int index = items.FindIndex(x => x.Type == item.Type && (x.Count + item.Count <= nInventory.ItemsStacks[x.Type]));
                    if (index != -1)
                    {
                        Console.WriteLine("add to stack: inventory count: "+ items[index].Count+" item count: "+ item.Count);
                        items[index].Count = items[index].Count + item.Count;
                        Log.Debug($"Added existing item!:{index.ToString()}");
                    }
                    else
                    {
                        int count = item.Count;
                        for (int i = 0; i < items.Count; i++)
                        {
                            if (i >= items.Count) break;
                            if (items[i].Type == item.Type && items[i].Count < nInventory.ItemsStacks[item.Type])
                            {
                                int temp = nInventory.ItemsStacks[item.Type] - items[i].Count;
                                if (count < temp) temp = count;
                                items[i].Count += temp;
                                count -= temp;
                                Console.WriteLine("[FOUND] item to stack and ADD new item type: "+ item.Type + " item ID: " + item.ID);
                            }
                        }

                        while(count > 0)
                        {
                            if (count >= nInventory.ItemsStacks[item.Type])
                            {
                                var newStackItem = new nItem(item.Type, nInventory.ItemsStacks[item.Type], item.Data);
                                slot = nInventory.CheckAdd(items, newStackItem, slots);

                                newStackItem.slot_id = slot;

                                count -= nInventory.ItemsStacks[item.Type];
                                if (slot == -1)
                                {
                                    Log.Debug("1 Ошибка, тут херня со слотами, он почему то занят стал!", nLog.Type.Error);
                                    return;
                                }

                                items.Add(newStackItem);
                                nInventory.FillSlot(slots, newStackItem, 5);
                                Console.WriteLine("[WHILE] add new STACK type: "+ item.Type + " item ID: " + item.ID);
                            }
                            else
                            {
                                var newStackItem = new nItem(item.Type, count, item.Data);
                                slot = nInventory.CheckAdd(items, newStackItem, slots);

                                newStackItem.slot_id = slot;

                                count = 0;
                                if (slot == -1)
                                {
                                    Log.Debug("2 Ошибка, тут херня со слотами, он почему то занят стал!", nLog.Type.Error);
                                    return;
                                }

                                items.Add(newStackItem);
                                nInventory.FillSlot(slots, newStackItem, 5);
                                Console.WriteLine("[WHILE] add new not full STACK TAIL type: "+ item.Type + " item ID: " + item.ID);
                            }
                        }
                    }
                }

                vehicle.SetData("ITEMS", items);
                vehicle.SetData("SLOTS", slots);

                if (vehicle.GetData<string>("ACCESS") == "PERSONAL" || vehicle.GetData<string>("ACCESS") == "GARAGE")
                    VehicleManager.Vehicles[vehicle.GetData<int>("ID")].Items = items;

                foreach (Player p in Main.Players.Keys.ToList())
                {
                    if (p == null || !Main.Players.ContainsKey(p)) continue;
                    if (p.HasMyData("OPENOUT_TYPE") && p.GetMyData<int>("OPENOUT_TYPE") == 2 && p.HasMyData("SELECTEDVEH") && p.GetMyData<Vehicle>("SELECTEDVEH") == vehicle) GUI.Dashboard.OpenOut(p, items, "Багажник", 2);
                }

            //else
            //{
            //    int count = item.Count;
            //    for (int i = 0; i < items.Count; i++)
            //    {
            //        if (i >= items.Count) break;
            //        if (items[i].Type == item.Type && items[i].Count < nInventory.ItemsStacks[item.Type])
            //        {
            //            int temp = nInventory.ItemsStacks[item.Type] - items[i].Count;
            //            if (count < temp) temp = count;
            //            items[i].Count += temp;
            //            count -= temp;
            //        }
            //    }

            //    while (count > 0)
            //    {
            //        if (count >= nInventory.ItemsStacks[item.Type])
            //        {
            //            items.Add(new nItem(item.Type, nInventory.ItemsStacks[item.Type], item.Data));
            //            count -= nInventory.ItemsStacks[item.Type];
            //        }
            //        else
            //        {
            //            items.Add(new nItem(item.Type, count, item.Data));
            //            count = 0;
            //        }
            //    }
            //}
        }

        public static int TryAdd(Vehicle vehicle, nItem item)
        {
            if (!vehicle.HasData("ITEMS")) return -1;
            if (!vehicle.HasData("SLOTS")) return -1;
            List<bool> slots = vehicle.GetData<List<bool>>("SLOTS");
            List<nItem> items = vehicle.GetData<List<nItem>>("ITEMS");

            var checkStacks = nInventory.CheckAddStacks(items, item, slots);
            var checkWeight = VehicleManager.CheckWeight(vehicle.GetData<int>("ID"), item);

            if (checkStacks == -1) return -1; // Нет слотов
            if (checkWeight == -3) return -3; // Тачка отстутствует в массиве машин
            if (checkWeight == -2) return -2; // Превышен лимит веса

            return 0;
            //int tail = 0;
            //if (nInventory.ClothesItems.Contains(item.Type) || nInventory.WeaponsItems.Contains(item.Type) || nInventory.MeleeWeaponsItems.Contains(item.Type) ||
            //    item.Type == ItemType.CarKey || item.Type == ItemType.KeyRing)
            //{
            //    if (items.Count >= CountSlots) return -1;
            //}
            //else
            //{
            //    int count = 0;
            //    foreach (nItem i in items)
            //        if (i.Type == item.Type) count += nInventory.ItemsStacks[i.Type] - i.Count;

            //    //m4ybe
            //    int maxCapacity = (CountSlots - items.Count) * nInventory.ItemsStacks[item.Type] + count;
            //    if (item.Count > maxCapacity) tail = item.Count - maxCapacity;
            //}
            //return tail;
        }

        public static int GetCountOfType(Vehicle vehicle, ItemType type)
        {
            //TODO DEN
            if (!vehicle.HasData("ITEMS")) return 0;
            List<nItem> items = vehicle.GetData<List<nItem>>("ITEMS");

            int count = 0;

            for (int i = 0; i < items.Count; i++)
            {
                if (i >= items.Count) break;
                if (items[i].Type == type) count += items[i].Count;
            }

            return count;
        }

        public static void Remove(Vehicle vehicle, ItemType type, int amount)
        {
            if (!vehicle.HasData("ITEMS")) return;
            if (!vehicle.HasData("SLOTS")) return;
            List<bool> slots = vehicle.GetData<List<bool>>("SLOTS");
            List<nItem> items = vehicle.GetData<List<nItem>>("ITEMS");

            var index = items.FindIndex(i => i.Type == type);

            if (index != -1)
            {
                int temp = items[index].Count - amount;
                if (temp > 0)
                {
                    items[index].Count = temp;
                }
                else
                {
                    amount -= items[index].Count;

                    nInventory.ClearSlot(slots, items[index], 5);
                    items.RemoveAt(index);

                    while(amount != 0)
                    {
                        index = items.FindIndex(i => i.Type == type);

                        if (index == -1) break;

                        temp = items[index].Count - amount;

                        if (temp > 0)
                        {
                            items[index].Count = temp;

                            amount = 0;
                        }
                        else
                        {
                            amount -= items[index].Count;

                            nInventory.ClearSlot(slots, items[index], 5);
                            items.RemoveAt(index);
                        }
                    }
                }
            }



            //for (int i = items.Count - 1; i >= 0; i--)
            //{
            //    if (i >= items.Count) continue;
            //    if (items[i].Type != type) continue;
            //    if (items[i].Count <= amount)
            //    {
            //        amount -= items[i].Count;
            //        items.RemoveAt(i);
            //    }
            //    else
            //    {
            //        items[i].Count -= amount;
            //        amount = 0;
            //        break;
            //    }
            //}

            if (vehicle.GetData<string>("ACCESS") == "PERSONAL" || vehicle.GetData<string>("ACCESS") == "GARAGE")
                VehicleManager.Vehicles[vehicle.GetData<int>("ID")].Items = items;

            foreach (Player p in Main.Players.Keys.ToList())
            {
                if (p == null || !Main.Players.ContainsKey(p)) continue;
                if (p.HasMyData("OPENOUT_TYPE") && p.GetMyData<int>("OPENOUT_TYPE") == 2 && p.HasMyData("SELECTEDVEH") && p.GetMyData<Vehicle>("SELECTEDVEH") == vehicle) GUI.Dashboard.OpenOut(p, items, "Багажник", 2);
            }
        }

        public static void Remove(Vehicle vehicle, nItem item)
        {
            if (!vehicle.HasData("ITEMS")) return;
            if (!vehicle.HasData("SLOTS")) return;
            List<bool> slots = vehicle.GetData<List<bool>>("SLOTS");
            List<nItem> items = vehicle.GetData<List<nItem>>("ITEMS");

            if (nInventory.ClothesItems.Contains(item.Type) || nInventory.WeaponsItems.Contains(item.Type) || nInventory.MeleeWeaponsItems.Contains(item.Type) ||
                item.Type == ItemType.BagWithDrill || item.Type == ItemType.BagWithMoney || item.Type == ItemType.CarKey || item.Type == ItemType.KeyRing || item.Type == ItemType.NumberPlate)
            {
                nInventory.ClearSlot(slots, item, 5);
                items.Remove(item);

                Log.Debug($"Item removed. Vehicle:TYPE {(int)item.Type}");
                GameLog.Items($"Vehicle", "removed", Convert.ToInt32(item.Type), item.Count, $"{item.Data} - {nInventory.ItemsNames[(int)item.Type]}");
            }
            else
            {
                var index = items.FindIndex(i => i.Type == item.Type);

                if (index != -1)
                {
                    int temp = items[index].Count - item.Count;
                    if (temp > 0)
                    {
                        items[index].Count = temp;
                    }
                    else
                    {
                        nInventory.ClearSlot(slots, items[index], 5);
                        items.RemoveAt(index);
                    }
                }

                //for (int i = items.Count - 1; i >= 0; i--)
                //{
                //    if (i >= items.Count) continue;
                //    if (items[i].Type != item.Type) continue;
                //    if (items[i].Count <= item.Count)
                //    {
                //        item.Count -= items[i].Count;
                //        items.RemoveAt(i);
                //    }
                //    else
                //    {
                //        items[i].Count -= item.Count;
                //        item.Count = 0;
                //        break;
                //    }
                //}
            }

            if (vehicle.GetData<string>("ACCESS") == "PERSONAL" || vehicle.GetData<string>("ACCESS") == "GARAGE")
                VehicleManager.Vehicles[vehicle.GetData<int>("ID")].Items = items;

            foreach (Player p in Main.Players.Keys.ToList())
            {
                if (p == null || !Main.Players.ContainsKey(p)) continue;
                if (p.HasMyData("OPENOUT_TYPE") && p.GetMyData<int>("OPENOUT_TYPE") == 2 && p.HasMyData("SELECTEDVEH") && p.GetMyData<Vehicle>("SELECTEDVEH") == vehicle) GUI.Dashboard.OpenOut(p, items, "Багажник", 2);
            }
        }
    }
}
