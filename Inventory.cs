using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Linq;
using System.Threading;
using GTANetworkAPI;
using Newtonsoft.Json;
using NeptuneEvo.SDK;
using NeptuneEvo.Core.Character;
using NeptuneEvo.Core.nAccount;
using static System.Reflection.Metadata.BlobBuilder;
using NeptuneEvo.GUI;
using System.Reflection;
using client.Systems.CraftSystem;
using MySqlConnector;
using client.Systems.BattlePass;

namespace NeptuneEvo.Core
{
    public enum ItemType
    {
        Mask = -1, // Маска
        Gloves = -3, // Перчатки
        Leg = -4, // Штанишки
        Bag = -5, // Рюкзачок
        Bag1 = -15, // Рюкзачок
        Feet = -6, // Обуточки
        Jewelry = -7, // Аксессуарчики всякие там
        Undershit = -8, // Рубашечки
        BodyArmor = -9, // Бронька
        BodyArmorgov1 = -16, // Бронька
        BodyArmorgov2 = -17, // Бронька
        BodyArmorgov3 = -18, // Бронька
        BodyArmorgov4 = -19, // Бронька
        Unknown = -10, // Вообще хер пойми что это
        Top = -11, // Верх
        Hat = -12, // Шляпы
        Glasses = -13, // Очочки
        Accessories = -14, // Часы/Браслеты

        Debug = 0,
        BagWithMoney = 12,// Сумка с деньгами
        Material = 13,    // Материалы
        Drugs = 14,       // Наркота
        BagWithDrill = 15,// Сумка с дрелью
        HealthKit = 1,    // Аптечка
        GasCan = 2,       // Канистра
        Сrisps = 3,       // Чипсы
        Beer = 4,         // Пиво
        Pizza = 5,        // Пицца
        Burger = 6,       // Бургер
        HotDog = 7,       // Хот-Дог
        Sandwich = 8,     // Сэндвич
        eCola = 9,        // Кока-Кола
        Sprunk = 10,      // Спрайт
        Lockpick = 11,    // Отмычка для замка
        ArmyLockpick = 16,// Военная отмычка
        Pocket = 17,      // Мешок
        Cuffs = 18,       // Стяжки
        CarKey = 19,      // Ключи от личной машины
        Present = 40,     // Подарок
        KeyRing = 41,     // Связка ключей

        /* Drinks */
        RusDrink1 = 20,
        RusDrink2 = 21,
        RusDrink3 = 22,

        YakDrink1 = 23,
        YakDrink2 = 24,
        YakDrink3 = 25,

        LcnDrink1 = 26,
        LcnDrink2 = 27,
        LcnDrink3 = 28,

        ArmDrink1 = 29,
        ArmDrink2 = 30,
        ArmDrink3 = 31,

        WaterBottle = 32,
        RepairBox = 33,
        SmallHealthKit = 34,

        /* Weapons */
        /* Pistols */
        Pistol = 100,
        Combatpistol = 101,
        Pistol50 = 102,
        Snspistol = 103,
        Heavypistol = 104,
        Vintagepistol = 105,
        Marksmanpistol = 106,
        Revolver = 107,
        Appistol = 108,
        Flaregun = 110,
        Doubleaction = 111,
        Pistol_mk2 = 112,
        Snspistol_mk2 = 113,
        Revolver_mk2 = 114,
        Heavyrevolver_mk2 = 10115,
        /* Smg */
        Microsmg = 115,
        Machinepistol = 116,
        Smg = 117,
        Assaultsmg = 118,
        Combatpdw = 119,
        Mg = 120,
        Combatmg = 121,
        Gusenberg = 122,
        Minismg = 123,
        Smg_mk2 = 124,
        Combatmg_mk2 = 125,
        /* Rifles */
        Assaultrifle = 126,
        Carbinerifle = 127,
        Advancedrifle = 128,
        Specialcarbine = 129,
        Bullpuprifle = 130,
        Compactrifle = 131,
        Militaryrifle = 10132,
        Assaultrifle_mk2 = 132,
        Carbinerifle_mk2 = 133,
        Specialcarbine_mk2 = 134,
        Bullpuprifle_mk2 = 135,
        /* Sniper */
        Sniperrifle = 136,
        Heavysniper = 137,
        Marksmanrifle = 138,
        Heavysniper_mk2 = 139,
        Marksmanrifle_mk2 = 140,
        /* Shotguns */
        Pumpshotgun = 141,
        Sawnoffshotgun = 142,
        Bullpupshotgun = 143,
        Assaultshotgun = 144,
        Musket = 145,
        Heavyshotgun = 146,
        Dbshotgun = 147,
        Autoshotgun = 148,
        Pumpshotgun_mk2 = 149,
        Combatshotgun = 10150,
        //havy
        Rpg = 150,
        /* MELEE WEAPONS */
        Stungun = 109,
        Knife = 180,
        Nightstick = 181,
        Hammer = 182,
        Bat = 183,
        Crowbar = 184,
        Golfclub = 185,
        Bottle = 186,
        Dagger = 187,
        Hatchet = 188,
        Knuckle = 189,
        Machete = 190,
        Flashlight = 191,
        Switchblade = 192,
        Poolcue = 193,
        Wrench = 194,
        Battleaxe = 195,
        /* Ammo */
        PistolAmmo = 200,
        SMGAmmo = 201,
        RiflesAmmo = 202,
        SniperAmmo = 203,
        ShotgunsAmmo = 204,

        /* Fishing */
        Rod = 1205, // Удочка
        RodUpgrade = 1206, // Улучшенная удочка
        RodMK2 = 1207, // Удочка MK2
        Bait = 1219, // Наживка
        Naz = 1208, // Наживка

        Kyndja = 1209, // Корюшка
        Sig = 1210, // Кунджа
        Omyl = 1211, // Лосось
        Nerka = 1212, // Окунь
        Forel = 1213, // Осётр
        Ship = 1214, // Скат
        Lopatonos = 1215, // Тунец
        Osetr = 1216, // Угорь
        Semga = 1217, // Чёрный амур
        Servyga = 1218, // Щука
        Beluga = 1220,
        Taimen = 1221,
        Sterlyad = 1222,
        Ydilshik = 1223,
        Hailiod = 1224,
        Keta = 1225,
        Gorbysha = 1226,


        //Farmer Jobs Items
        Hay = 1234,
        Seed = 1235,

        Fireextinguisher = 205,
        Compactlauncher = 206,
        Snowball = 207,
        Ball = 208,
        Molotov = 209,
        Ceramicpistol = 210,
        Stickybomb = 211,
        Petrolcan = 212,
        Stone_hatchet = 213,
        Minigun = 214,
        Raycarbine = 215,
        Flare = 216,
        Grenadelauncher_smoke = 217,
        Hominglauncher = 218,
        Railgun = 219,
        Firework = 220,
        Proximine = 221,
        NavyRevolver = 222,
        Grenade = 223,
        Bzgas = 224,
        Grenadelauncher = 225,
        Raypistol = 226,
        Rayminigun = 227,
        Pipebomb = 228,
        HazardCan = 229,
        Parachute = 230,
        Smokegrenade = 231,

        Subject = 316,
        Payek = 317,

        LSPDDrone = 847,
        Drone = 848,
        Bandage = 849,
        Spray = 850,
        Weed = 851,
        CocaLeaves = 852,
        Cocaine = 853,
        DrugBookMark = 854,

        CasinoChips = 1001,
        NumberPlate = 1002,
        SimCard = 1003,

        ProductBox = 1004,
        TrashBag = 1005,

        ADVANCEDRIFLE_CLIP_02 = 5001,
        ADVANCEDRIFLE_VARMOD_LUXE = 5002,
        APPISTOL_CLIP_02 = 5003,
        APPISTOL_VARMOD_LUXE = 5004,
        ASSAULTRIFLE_CLIP_02 = 5005,
        ASSAULTRIFLE_CLIP_03 = 5006,
        ASSAULTRIFLE_MK2_CAMO = 5007,
        ASSAULTRIFLE_MK2_CAMO_02 = 5008,
        ASSAULTRIFLE_MK2_CAMO_03 = 5009,
        ASSAULTRIFLE_MK2_CAMO_04 = 5010,
        ASSAULTRIFLE_MK2_CAMO_05 = 5011,
        ASSAULTRIFLE_MK2_CAMO_06 = 5012,
        ASSAULTRIFLE_MK2_CAMO_07 = 5013,
        ASSAULTRIFLE_MK2_CAMO_08 = 5014,
        ASSAULTRIFLE_MK2_CAMO_09 = 5015,
        ASSAULTRIFLE_MK2_CAMO_10 = 5016,
        ASSAULTRIFLE_MK2_CAMO_IND_01 = 5017,
        ASSAULTRIFLE_MK2_CLIP_02 = 5018,
        ASSAULTRIFLE_MK2_CLIP_ARMORPIERCING = 5019,
        ASSAULTRIFLE_MK2_CLIP_FMJ = 5020,
        ASSAULTRIFLE_MK2_CLIP_INCENDIARY = 5021,
        ASSAULTRIFLE_MK2_CLIP_TRACER = 5022,
        ASSAULTRIFLE_VARMOD_LUXE = 5023,
        ASSAULTSHOTGUN_CLIP_02 = 5024,
        ASSAULTSMG_CLIP_02 = 5025,
        ASSAULTSMG_VARMOD_LOWRIDER = 5026,
        AT_AR_AFGRIP = 5027,
        AT_AR_AFGRIP_02 = 5028,
        AT_AR_BARREL_02 = 5029,
        AT_AR_FLSH = 5030,
        AT_AR_SUPP = 5031,
        AT_AR_SUPP_02 = 5032,
        AT_BP_BARREL_02 = 5033,
        AT_CR_BARREL_02 = 5034,
        AT_MG_BARREL_02 = 5035,
        AT_MRFL_BARREL_02 = 5036,
        AT_MUZZLE_01 = 5037,
        AT_MUZZLE_02 = 5038,
        AT_MUZZLE_03 = 5039,
        AT_MUZZLE_04 = 5040,
        AT_MUZZLE_05 = 5041,
        AT_MUZZLE_06 = 5042,
        AT_MUZZLE_07 = 5043,
        AT_MUZZLE_08 = 5044,
        AT_MUZZLE_09 = 5045,
        AT_PI_COMP = 5046,
        AT_PI_COMP_02 = 5047,
        AT_PI_COMP_03 = 5048,
        AT_PI_FLSH = 5049,
        AT_PI_FLSH_02 = 5050,
        AT_PI_FLSH_03 = 5051,
        AT_PI_RAIL = 5052,
        AT_PI_RAIL_02 = 5053,
        AT_PI_SUPP = 5054,
        AT_PI_SUPP_02 = 5055,
        AT_SB_BARREL_02 = 5056,
        AT_SC_BARREL_02 = 5057,
        AT_SCOPE_LARGE = 5058,
        AT_SCOPE_LARGE_FIXED_ZOOM = 5059,
        AT_SCOPE_LARGE_FIXED_ZOOM_MK2 = 5060,
        AT_SCOPE_LARGE_MK2 = 5061,
        AT_SCOPE_MACRO = 5062,
        AT_SCOPE_MACRO_02 = 5063,
        AT_SCOPE_MACRO_02_MK2 = 5064,
        AT_SCOPE_MACRO_02_SMG_MK2 = 5065,
        AT_SCOPE_MACRO_MK2 = 5066,
        AT_SCOPE_MAX = 5067,
        AT_SCOPE_MEDIUM = 5068,
        AT_SCOPE_MEDIUM_MK2 = 5069,
        AT_SCOPE_NV = 5070,
        AT_SCOPE_SMALL = 5071,
        AT_SCOPE_SMALL_02 = 5072,
        AT_SCOPE_SMALL_MK2 = 5073,
        AT_SCOPE_SMALL_SMG_MK2 = 5074,
        AT_SCOPE_THERMAL = 5075,
        AT_SIGHTS = 5076,
        AT_SIGHTS_SMG = 5077,
        AT_SR_BARREL_02 = 5078,
        AT_SR_SUPP = 5079,
        AT_SR_SUPP_03 = 5080,
        BULLPUPRIFLE_CLIP_02 = 5081,
        BULLPUPRIFLE_MK2_CAMO = 5082,
        BULLPUPRIFLE_MK2_CAMO_02 = 5083,
        BULLPUPRIFLE_MK2_CAMO_03 = 5084,
        BULLPUPRIFLE_MK2_CAMO_04 = 5085,
        BULLPUPRIFLE_MK2_CAMO_05 = 5086,
        BULLPUPRIFLE_MK2_CAMO_06 = 5087,
        BULLPUPRIFLE_MK2_CAMO_07 = 5088,
        BULLPUPRIFLE_MK2_CAMO_08 = 5089,
        BULLPUPRIFLE_MK2_CAMO_09 = 5090,
        BULLPUPRIFLE_MK2_CAMO_10 = 5091,
        BULLPUPRIFLE_MK2_CAMO_IND_01 = 5092,
        BULLPUPRIFLE_MK2_CLIP_02 = 5093,
        BULLPUPRIFLE_MK2_CLIP_ARMORPIERCING = 5094,
        BULLPUPRIFLE_MK2_CLIP_FMJ = 5095,
        BULLPUPRIFLE_MK2_CLIP_INCENDIARY = 5096,
        BULLPUPRIFLE_MK2_CLIP_TRACER = 5097,
        BULLPUPRIFLE_VARMOD_LOW = 5098,
        CARBINERIFLE_CLIP_02 = 5099,
        CARBINERIFLE_CLIP_03 = 5100,
        CARBINERIFLE_MK2_CAMO = 5101,
        CARBINERIFLE_MK2_CAMO_02 = 5102,
        CARBINERIFLE_MK2_CAMO_03 = 5103,
        CARBINERIFLE_MK2_CAMO_04 = 5104,
        CARBINERIFLE_MK2_CAMO_05 = 5105,
        CARBINERIFLE_MK2_CAMO_06 = 5106,
        CARBINERIFLE_MK2_CAMO_07 = 5107,
        CARBINERIFLE_MK2_CAMO_08 = 5108,
        CARBINERIFLE_MK2_CAMO_09 = 5109,
        CARBINERIFLE_MK2_CAMO_10 = 5110,
        CARBINERIFLE_MK2_CAMO_IND_01 = 5111,
        CARBINERIFLE_MK2_CLIP_02 = 5112,
        CARBINERIFLE_MK2_CLIP_ARMORPIERCING = 5113,
        CARBINERIFLE_MK2_CLIP_FMJ = 5114,
        CARBINERIFLE_MK2_CLIP_INCENDIARY = 5115,
        CARBINERIFLE_MK2_CLIP_TRACER = 5116,
        CARBINERIFLE_VARMOD_LUXE = 5117,
        CERAMICPISTOL_CLIP_02 = 5118,
        CERAMICPISTOL_SUPP = 5119,
        COMBATMG_CLIP_02 = 5120,
        COMBATMG_MK2_CAMO = 5121,
        COMBATMG_MK2_CAMO_02 = 5122,
        COMBATMG_MK2_CAMO_03 = 5123,
        COMBATMG_MK2_CAMO_04 = 5124,
        COMBATMG_MK2_CAMO_05 = 5125,
        COMBATMG_MK2_CAMO_06 = 5126,
        COMBATMG_MK2_CAMO_07 = 5127,
        COMBATMG_MK2_CAMO_08 = 5128,
        COMBATMG_MK2_CAMO_09 = 5129,
        COMBATMG_MK2_CAMO_10 = 5130,
        COMBATMG_MK2_CAMO_IND_01 = 5131,
        COMBATMG_MK2_CLIP_02 = 5132,
        COMBATMG_MK2_CLIP_ARMORPIERCING = 5133,
        COMBATMG_MK2_CLIP_FMJ = 5134,
        COMBATMG_MK2_CLIP_INCENDIARY = 5135,
        COMBATMG_MK2_CLIP_TRACER = 5136,
        COMBATMG_VARMOD_LOWRIDER = 5137,
        COMBATPDW_CLIP_02 = 5138,
        COMBATPDW_CLIP_03 = 5139,
        COMBATPISTOL_CLIP_02 = 5140,
        COMBATPISTOL_VARMOD_LOWRIDER = 5141,
        COMPACTRIFLE_CLIP_02 = 5142,
        COMPACTRIFLE_CLIP_03 = 5143,
        GUSENBERG_CLIP_02 = 5144,
        HEAVYPISTOL_CLIP_02 = 5145,
        HEAVYPISTOL_VARMOD_LUXE = 5146,
        HEAVYSHOTGUN_CLIP_02 = 5147,
        HEAVYSHOTGUN_CLIP_03 = 5148,
        HEAVYSNIPER_MK2_CAMO = 5149,
        HEAVYSNIPER_MK2_CAMO_02 = 5150,
        HEAVYSNIPER_MK2_CAMO_03 = 5151,
        HEAVYSNIPER_MK2_CAMO_04 = 5152,
        HEAVYSNIPER_MK2_CAMO_05 = 5153,
        HEAVYSNIPER_MK2_CAMO_06 = 5154,
        HEAVYSNIPER_MK2_CAMO_07 = 5155,
        HEAVYSNIPER_MK2_CAMO_08 = 5156,
        HEAVYSNIPER_MK2_CAMO_09 = 5157,
        HEAVYSNIPER_MK2_CAMO_10 = 5158,
        HEAVYSNIPER_MK2_CAMO_IND_01 = 5159,
        HEAVYSNIPER_MK2_CLIP_02 = 5160,
        HEAVYSNIPER_MK2_CLIP_ARMORPIERCING = 5161,
        HEAVYSNIPER_MK2_CLIP_EXPLOSIVE = 5162,
        HEAVYSNIPER_MK2_CLIP_FMJ = 5163,
        HEAVYSNIPER_MK2_CLIP_INCENDIARY = 5164,
        KNUCKLE_VARMOD_BALLAS = 5165,
        KNUCKLE_VARMOD_DIAMOND = 5166,
        KNUCKLE_VARMOD_DOLLAR = 5167,
        KNUCKLE_VARMOD_HATE = 5168,
        KNUCKLE_VARMOD_KING = 5169,
        KNUCKLE_VARMOD_LOVE = 5170,
        KNUCKLE_VARMOD_PIMP = 5171,
        KNUCKLE_VARMOD_PLAYER = 5172,
        KNUCKLE_VARMOD_VAGOS = 5173,
        MACHINEPISTOL_CLIP_02 = 5174,
        MACHINEPISTOL_CLIP_03 = 5175,
        MARKSMANRIFLE_CLIP_02 = 5176,
        MARKSMANRIFLE_MK2_CAMO = 5177,
        MARKSMANRIFLE_MK2_CAMO_02 = 5178,
        MARKSMANRIFLE_MK2_CAMO_03 = 5179,
        MARKSMANRIFLE_MK2_CAMO_04 = 5180,
        MARKSMANRIFLE_MK2_CAMO_05 = 5181,
        MARKSMANRIFLE_MK2_CAMO_06 = 5182,
        MARKSMANRIFLE_MK2_CAMO_07 = 5183,
        MARKSMANRIFLE_MK2_CAMO_08 = 5184,
        MARKSMANRIFLE_MK2_CAMO_09 = 5185,
        MARKSMANRIFLE_MK2_CAMO_10 = 5186,
        MARKSMANRIFLE_MK2_CAMO_IND_01 = 5187,
        MARKSMANRIFLE_MK2_CLIP_02 = 5188,
        MARKSMANRIFLE_MK2_CLIP_ARMORPIERCING = 5189,
        MARKSMANRIFLE_MK2_CLIP_FMJ = 5190,
        MARKSMANRIFLE_MK2_CLIP_INCENDIARY = 5191,
        MARKSMANRIFLE_MK2_CLIP_TRACER = 5192,
        MARKSMANRIFLE_VARMOD_LUXE = 5193,
        MG_CLIP_02 = 5194,
        MG_VARMOD_LOWRIDER = 5195,
        MICROSMG_CLIP_02 = 5196,
        MICROSMG_VARMOD_LUXE = 5197,
        MILITARYRIFLE_CLIP_02 = 5198,
        MILITARYRIFLE_SIGHT_01 = 5199,
        MINISMG_CLIP_02 = 5200,
        PISTOL50_CLIP_02 = 5201,
        PISTOL50_VARMOD_LUXE = 5202,
        PISTOL_CLIP_02 = 5203,
        PISTOL_MK2_CAMO = 5204,
        PISTOL_MK2_CAMO_02 = 5205,
        PISTOL_MK2_CAMO_02_SLIDE = 5206,
        PISTOL_MK2_CAMO_03 = 5207,
        PISTOL_MK2_CAMO_03_SLIDE = 5208,
        PISTOL_MK2_CAMO_04 = 5209,
        PISTOL_MK2_CAMO_04_SLIDE = 5210,
        PISTOL_MK2_CAMO_05 = 5211,
        PISTOL_MK2_CAMO_05_SLIDE = 5212,
        PISTOL_MK2_CAMO_06 = 5213,
        PISTOL_MK2_CAMO_06_SLIDE = 5214,
        PISTOL_MK2_CAMO_07 = 5215,
        PISTOL_MK2_CAMO_07_SLIDE = 5216,
        PISTOL_MK2_CAMO_08 = 5217,
        PISTOL_MK2_CAMO_08_SLIDE = 5218,
        PISTOL_MK2_CAMO_09 = 5219,
        PISTOL_MK2_CAMO_09_SLIDE = 5220,
        PISTOL_MK2_CAMO_10 = 5221,
        PISTOL_MK2_CAMO_10_SLIDE = 5222,
        PISTOL_MK2_CAMO_IND_01 = 5223,
        PISTOL_MK2_CAMO_IND_01_SLIDE = 5224,
        PISTOL_MK2_CAMO_SLIDE = 5225,
        PISTOL_MK2_CLIP_02 = 5226,
        PISTOL_MK2_CLIP_FMJ = 5227,
        PISTOL_MK2_CLIP_HOLLOWPOINT = 5228,
        PISTOL_MK2_CLIP_INCENDIARY = 5229,
        PISTOL_MK2_CLIP_TRACER = 5230,
        PISTOL_VARMOD_LUXE = 5231,
        PUMPSHOTGUN_MK2_CAMO = 5232,
        PUMPSHOTGUN_MK2_CAMO_02 = 5233,
        PUMPSHOTGUN_MK2_CAMO_03 = 5234,
        PUMPSHOTGUN_MK2_CAMO_04 = 5235,
        PUMPSHOTGUN_MK2_CAMO_05 = 5236,
        PUMPSHOTGUN_MK2_CAMO_06 = 5237,
        PUMPSHOTGUN_MK2_CAMO_07 = 5238,
        PUMPSHOTGUN_MK2_CAMO_08 = 5239,
        PUMPSHOTGUN_MK2_CAMO_09 = 5240,
        PUMPSHOTGUN_MK2_CAMO_10 = 5241,
        PUMPSHOTGUN_MK2_CAMO_IND_01 = 5242,
        PUMPSHOTGUN_MK2_CLIP_ARMORPIERCING = 5243,
        PUMPSHOTGUN_MK2_CLIP_EXPLOSIVE = 5244,
        PUMPSHOTGUN_MK2_CLIP_HOLLOWPOINT = 5245,
        PUMPSHOTGUN_MK2_CLIP_INCENDIARY = 5246,
        PUMPSHOTGUN_VARMOD_LOWRIDER = 5247,
        RAYPISTOL_VARMOD_XMAS18 = 5248,
        REVOLVER_MK2_CAMO = 5249,
        REVOLVER_MK2_CAMO_02 = 5250,
        REVOLVER_MK2_CAMO_03 = 5251,
        REVOLVER_MK2_CAMO_04 = 5252,
        REVOLVER_MK2_CAMO_05 = 5253,
        REVOLVER_MK2_CAMO_06 = 5254,
        REVOLVER_MK2_CAMO_07 = 5255,
        REVOLVER_MK2_CAMO_08 = 5256,
        REVOLVER_MK2_CAMO_09 = 5257,
        REVOLVER_MK2_CAMO_10 = 5258,
        REVOLVER_MK2_CAMO_IND_01 = 5259,
        REVOLVER_MK2_CLIP_FMJ = 5260,
        REVOLVER_MK2_CLIP_HOLLOWPOINT = 5261,
        REVOLVER_MK2_CLIP_INCENDIARY = 5262,
        REVOLVER_MK2_CLIP_TRACER = 5263,
        REVOLVER_VARMOD_BOSS = 5264,
        REVOLVER_VARMOD_GOON = 5265,
        SAWNOFFSHOTGUN_VARMOD_LUXE = 5266,
        SMG_CLIP_02 = 5267,
        SMG_CLIP_03 = 5268,
        SMG_MK2_CAMO = 5269,
        SMG_MK2_CAMO_02 = 5270,
        SMG_MK2_CAMO_03 = 5271,
        SMG_MK2_CAMO_04 = 5272,
        SMG_MK2_CAMO_05 = 5273,
        SMG_MK2_CAMO_06 = 5274,
        SMG_MK2_CAMO_07 = 5275,
        SMG_MK2_CAMO_08 = 5276,
        SMG_MK2_CAMO_09 = 5277,
        SMG_MK2_CAMO_10 = 5278,
        SMG_MK2_CAMO_IND_01 = 5279,
        SMG_MK2_CLIP_02 = 5280,
        SMG_MK2_CLIP_FMJ = 5281,
        SMG_MK2_CLIP_HOLLOWPOINT = 5282,
        SMG_MK2_CLIP_INCENDIARY = 5283,
        SMG_MK2_CLIP_TRACER = 5284,
        SMG_VARMOD_LUXE = 5285,
        SNIPERRIFLE_VARMOD_LUXE = 5286,
        SNSPISTOL_CLIP_02 = 5287,
        SNSPISTOL_MK2_CAMO = 5288,
        SNSPISTOL_MK2_CAMO_02 = 5289,
        SNSPISTOL_MK2_CAMO_02_SLIDE = 5290,
        SNSPISTOL_MK2_CAMO_03 = 5291,
        SNSPISTOL_MK2_CAMO_03_SLIDE = 5292,
        SNSPISTOL_MK2_CAMO_04 = 5293,
        SNSPISTOL_MK2_CAMO_04_SLIDE = 5294,
        SNSPISTOL_MK2_CAMO_05 = 5295,
        SNSPISTOL_MK2_CAMO_05_SLIDE = 5296,
        SNSPISTOL_MK2_CAMO_06 = 5297,
        SNSPISTOL_MK2_CAMO_06_SLIDE = 5298,
        SNSPISTOL_MK2_CAMO_07 = 5299,
        SNSPISTOL_MK2_CAMO_07_SLIDE = 5300,
        SNSPISTOL_MK2_CAMO_08 = 5301,
        SNSPISTOL_MK2_CAMO_08_SLIDE = 5302,
        SNSPISTOL_MK2_CAMO_09 = 5303,
        SNSPISTOL_MK2_CAMO_09_SLIDE = 5304,
        SNSPISTOL_MK2_CAMO_10 = 5305,
        SNSPISTOL_MK2_CAMO_10_SLIDE = 5306,
        SNSPISTOL_MK2_CAMO_IND_01 = 5307,
        SNSPISTOL_MK2_CAMO_IND_01_SLIDE = 5308,
        SNSPISTOL_MK2_CAMO_SLIDE = 5309,
        SNSPISTOL_MK2_CLIP_02 = 5310,
        SNSPISTOL_MK2_CLIP_FMJ = 5311,
        SNSPISTOL_MK2_CLIP_HOLLOWPOINT = 5312,
        SNSPISTOL_MK2_CLIP_INCENDIARY = 5313,
        SNSPISTOL_MK2_CLIP_TRACER = 5314,
        SNSPISTOL_VARMOD_LOWRIDER = 5315,
        SPECIALCARBINE_CLIP_02 = 5316,
        SPECIALCARBINE_CLIP_03 = 5317,
        SPECIALCARBINE_MK2_CAMO = 5318,
        SPECIALCARBINE_MK2_CAMO_02 = 5319,
        SPECIALCARBINE_MK2_CAMO_03 = 5320,
        SPECIALCARBINE_MK2_CAMO_04 = 5321,
        SPECIALCARBINE_MK2_CAMO_05 = 5322,
        SPECIALCARBINE_MK2_CAMO_06 = 5323,
        SPECIALCARBINE_MK2_CAMO_07 = 5324,
        SPECIALCARBINE_MK2_CAMO_08 = 5325,
        SPECIALCARBINE_MK2_CAMO_09 = 5326,
        SPECIALCARBINE_MK2_CAMO_10 = 5327,
        SPECIALCARBINE_MK2_CAMO_IND_01 = 5328,
        SPECIALCARBINE_MK2_CLIP_02 = 5329,
        SPECIALCARBINE_MK2_CLIP_ARMORPIERCING = 5330,
        SPECIALCARBINE_MK2_CLIP_FMJ = 5331,
        SPECIALCARBINE_MK2_CLIP_INCENDIARY = 5332,
        SPECIALCARBINE_MK2_CLIP_TRACER = 5333,
        SPECIALCARBINE_VARMOD_LOWRIDER = 5334,
        SWITCHBLADE_VARMOD_VAR1 = 5335,
        SWITCHBLADE_VARMOD_VAR2 = 5336,
        VINTAGEPISTOL_CLIP_02 = 5337,


        //craft items
        DigScanner = 6000,
        DigScanner_mk2 = 6001,
        DigScanner_mk3 = 6002,

        DigShovel = 6010,
        DigShovel_mk2 = 6011,
        DigShovel_mk3 = 6012,

        CraftOldCoin = 6020,
        CraftShell = 6021,
        CraftCap = 6022,
        CraftScrapMetal = 6023,
        CraftCopperNugget = 6024,
        CraftIronNugget = 6025,
        CraftTinNugget = 6026,

        CraftCopperWire = 6030,
        CraftOldJewerly = 6031,
        CraftGoldNugget = 6032,
        CraftСollectibleCoin = 6033,
        CraftAncientStatuette = 6034,
        CraftGoldHorseShoe = 6035,
        CraftRelic = 6036,

        CraftIronPart = 6040,
        CraftCopperPart = 6041,
        CraftTinPart = 6042,
        CraftBronzePart = 6043,

        CraftWorkBench = 6050,
        CraftSmelter = 6051,
        CraftPercolator = 6052,
        CraftPartsCollector = 6053,
        CraftWorkBenchUpgrade = 6054,
        CraftWorkBenchUpgrade2 = 6055,
    }

    public class nItem
    {
        public int ID { get; internal set; }
        public ItemType Type { get; internal set; }
        public int Count { get; set; }
        public float Weight { get; set; }
        public bool IsActive { get; set; }
        public bool IsAttach { get; set; } = false;
        public dynamic Data;
        public WeaponData WData;
        public List<nItem> Items { get; set; }
        public List<bool> Slots { get; set; }
        public bool IsBag { get; set; } = false;
        public Bag Bag { get; set; }
        public int slot_id { get; set; }
        public int fast_slot_id { get; set; }
        public int character_slot_id { get; set; }

        public nItem(ItemType type, int count = 1, object data = null, bool isActive = false, WeaponData wd = null, bool isBag = false, Bag bag = null)
        {
            ID = Convert.ToInt32(type);
            Type = type;
            Count = count;
            Data = data;
            IsActive = isActive;
            Weight = nInventory.ItemsWeight[type] * count;
            WData = wd;

            if (isBag == true) {
                IsBag = isBag;
                Bag = bag;
            }
        }
    }

    class nInventory : Script
    {
        public static Dictionary<ItemType, int> ClothesItemSlots = new Dictionary<ItemType, int>()
        {
            { ItemType.Mask,1 },
            {ItemType.Glasses,2 },
            {ItemType.Bag,12 },
            {ItemType.Bag1,12 },
            {ItemType.Jewelry,7 },
            {ItemType.Accessories,9 },
            { ItemType.Gloves,4 },
            {ItemType.Hat,3 },
            {ItemType.Top,6 },
            {ItemType.Undershit,5 },
            {ItemType.Leg ,8},
            {ItemType.BodyArmor,10 },
            {ItemType.Feet,11 },
            {ItemType.BodyArmorgov1,10 },
            {ItemType.BodyArmorgov2,10 },
            {ItemType.BodyArmorgov3,10 },
            {ItemType.BodyArmorgov4,10 },

        };
        public static List<ItemType> ClothesItems = new List<ItemType>()
        {
            ItemType.Mask,
            ItemType.Gloves,
            ItemType.Leg,
            ItemType.Bag,
            ItemType.Bag1,
            ItemType.Feet,
            ItemType.Jewelry,
            ItemType.Undershit,
            ItemType.BodyArmor,
            ItemType.BodyArmorgov4,
            ItemType.BodyArmorgov3,
            ItemType.BodyArmorgov1,
            ItemType.BodyArmorgov2,
            ItemType.Unknown,
            ItemType.Top,
            ItemType.Hat,
            ItemType.Glasses,
            ItemType.Accessories,
        };

        public static Dictionary<int, string> ItemsNames = new Dictionary<int, string>
        {
            {-1, "Маска" },
            {-3, "Перчатки" },
            {-4, "Штаны"},
            {-5, "Рюкзак"},
            {-6, "Обувь"},
            {-7, "Аксессуар"},
            {-8, "Нижняя одежда"},
            {-9, "Бронежилет"},
            {-10, "Украшения"},
            {-11, "Верхняя одежда" },
            {-12, "Головной убор" },
            {-13, "Очки" },
            {-14, "Аксессуар" },
            {849, "Бинт" },
            {1, "Аптечка"},
            {2, "Канистра"},
            {3, "Чипсы"},
            {4, "Пиво"},
            {5, "Пицца"},
            {6, "Бургер"},
            {7, "Хот-Дог"},
            {8, "Сэндвич"},
            {9, "eCola"},
            {10, "Sprunk"},
            {11, "Отмычка для замков"},
            {12, "Сумка с деньгами"},
            {13, "Материалы"},
            {14, "Наркотики"},
            {15, "Сумка с дрелью"},
            {16, "Военная отмычка"},
            {17, "Мешок"},
            {18, "Стяжки"},
            {19, "Ключи от машины"},
            {40, "Подарок"},
            {41, "Связка ключей"},

            {20, "«На корке лимона»"},
            {21, "«На бруснике»"},
            {22, "«Русский стандарт»"},
            {23, "«Asahi»"},
            {24, "«Midori»"},
            {25, "«Yamazaki»"},
            {26, "«Martini Asti»"},
            {27, "«Sambuca»"},
            {28, "«Campari»"},
            {29, "«Дживан»"},
            {30, "«Арарат»"},
            {31, "«Noyan Tapan»"},
            {32, "Бутылка воды" },
            {33, "Рем. набор" },
            {34, "Малая аптечка" },

            {210, "Ceramic Pistol"},
            {10132, "Military Rifle"},
            {10150, "Combat Shotgun"},

            {100, "Pistol" },
            {101, "Combat Pistol" },
            {102, "Pistol 50" },
            {103, "SNS Pistol" },
            {104, "Heavy Pistol" },
            {105, "Vintage Pistol" },
            {106, "Marksman Pistol" },
            {107, "Revolver" },
            {108, "AP Pistol" },
            {109, "Stun Gun" },
            {110, "Flare Gun" },
            {111, "Double Action" },
            {112, "Pistol Mk2" },
            {113, "SNSPistol Mk2" },
            {114, "Revolver Mk2" },

            {115, "Micro SMG" },
            {116, "Machine Pistol" },
            {117, "SMG" },
            {118, "Assault SMG" },
            {119, "Combat PDW" },
            {120, "MG" },
            {121, "Combat MG" },
            {122, "Gusenberg" },
            {123, "Mini SMG" },
            {124, "SMG Mk2" },
            {125, "Combat MG Mk2" },

            {126, "Assault Rifle" },
            {127, "Carbine Rifle" },
            {128, "Advanced Rifle" },
            {129, "Special Carbine" },
            {130, "Bullpup Rifle" },
            {131, "Compact Rifle" },
            {132, "Assault Rifle Mk2" },
            {133, "Carbine Rifle Mk2" },
            {134, "Special Carbine Mk2" },
            {135, "Bullpup Rifle Mk2" },

            {136, "Sniper Rifle" },
            {137, "Heavy Sniper" },
            {138, "Marksman Rifle" },
            {139, "Heavy Sniper Mk2" },
            {140, "Marksman Rifle Mk2" },

            {141, "Pump Shotgun" },
            {142, "SawnOff Shotgun" },
            {143, "Bullpup Shotgun" },
            {144, "Assault Shotgun" },
            {145, "Musket" },
            {146, "Heavy Shotgun" },
            {147, "Double Barrel Shotgun" },
            {148, "Sweeper Shotgun" },
            {149, "Pump Shotgun Mk2" },

            {180, "Нож" },
            {181, "Дубинка" },
            {182, "Молоток" },
            {183, "Бита" },
            {184, "Лом" },
            {185, "Гольф клюшка" },
            {186, "Бутылка" },
            {187, "Кинжал" },
            {188, "Топор" },
            {189, "Кастет" },
            {190, "Мачете" },
            {191, "Фонарик" },
            {192, "Швейцарский нож" },
            {193, "Кий" },
            {194, "Ключ" },
            {195, "Боевой топор" },

            {200, "Пистолетный калибр" },
            {201, "Малый калибр" },
            {202, "Автоматный калибр" },
            {203, "Снайперский калибр" },
            {204, "Дробь" },
            {223, "Граната" },
            {1001, "Фишки" },

            {1205, "Удочка" },
            {1206, "Удочка МК2" },
            {1207, "Удочка MK3" },
            {1208, "Наживка" },
            {1209, "Кунджа" },
            {1210, "Сиг" },
            {1211, "Омуль" },
            {1212, "Нерка" },
            {1213, "Форель" },
            {1214, "Шип" },
            {1215, "Лопатонос" },
            {1216, "Осетр" },
            {1217, "Семга" },
            {1218, "Сервуга" },
            {1220, "Белуга" },
            {1221, "Таймень" },
            {1222, "Стерлядь" },
            {1223, "Удьлищик" },
            {1224, "Хайлоид" },
            {1225, "Кета" },
            {1226, "Горбуша" },

            {234, "Урожай" },
            {235, "Семена" },

            {317, "Сух.Паёк" },

            {740,""},
            {741,""},
            {742,""},
            {743,""},
            {744,""},
            {745,""},
            {746,""},
            {747,""},
            {748,""},
            {749,""},
            {750,""},
            {751,""},
            {752,""},
            {753,""},
            {754,""},
            {755,""},
            {756,""},
            {757,""},
            {758,""},
            {759,""},
            {760,""},
            {761,""},
            {762,""},
            {763,""},
            {764,""},
            {765,""},
            {766,""},
            {767,""},
            {768,""},
            {769,""},
            {770,""},
            {771,""},
            {772,""},
            {773,""},
            {774,""},
            {775,""},
            {776,""},
            {777,""},
            {778,""},
            {779,""},
            {780,""},
            {781,""},
            {782,""},
            {783,""},
            {784,""},
            {785,""},
            {786,""},
            {787,""},
            {788,""},
            {789,""},
            {790,""},
            {791,""},
            {792,""},
            {793,""},
            {794,""},
            {795,""},
            {796,""},
            {797,""},
            {798,""},
            {799,""},
            {800,""},
            {801,""},
            {802,""},
            {803,""},
            {804,""},
            {805,""},
            {806,""},
            {807,""},
            {808,""},
            {809,""},
            {810,""},
            {811,""},
            {812,""},
            {813,""},
            {814,""},
            {815,""},
            {816,""},
            {817,""},
            {818,""},
            {819,""},
            {820,""},
            {821,""},
            {822,""},
            {823,""},
            {824,""},
            {825,""},
            {826,""},
            {827,""},
            {828,""},
            {829,""},
            {830,""},
            {831,""},
            {832,""},
            {833,""},
            {834,""},
            {835,""},
            {836,""},
            {837,""},
            {838,""},
            {839,""},
            {840,""},
            {841,""},
            {842,""},
            {843,""},
            {844,""},
            {845,""},
            {846,""},
            {847, "Полицейский квадрокоптер" },
            {848, "Квадрокоптер" },
            {851, "Пачка травы" },
            {852, "Листья коки"},
            {853, "Кокаин"},
            {854, "Закладка" },
            {1002, "Номер автомобиля" },
            {1003, "Сим-карта" },
            {1004, "Продукты" },
            {1005, "Пакет с мусором" },
            {316, "Предмет" },

            // [Modification] Миша, Дима, Изменить. Имена модификаций оружия (Сделано!)
            {5001, "Расширенный магазин"},
            {5002, "Золотая расцветка"},
            {5003, "Расширенный магазин"},
            {5004, "Золотая расцветка"},
            {5005, "Расширенный магазин"},
            {5006, "Расширенный магазин"},
            {5007, "Цифровой камуфляж"},
            {5008, "Мазковый камуфляж"},
            {5009, "Лесной камуфляж"},
            {5010, "Череп"},
            {5011, "Sessanta Nove"},
            {5012, "Perseus"},
            {5013, "Леопард"},
            {5014, "Зебра"},
            {5015, "Геометрический"},
            {5016, "Boom!"},
            {5017, "Флаг США"},
            {5018, "Расширенный магазин"},
            {5019, "Бронебойные патроны"},
            {5020, "Патроны FMJ"},
            {5021, "Зажигательные патроны"},
            {5022, "Трассирующие патроны"},
            {5023, "Золотая расцветка"},
            {5024, "Расширенный магазин"},
            {5025, "Расширенный магазин"},
            {5026, "Золотая расцветка"},
            {5027, "Рукоять"},
            {5028, "Рукоять"},
            {5029, "Улучшенный ствол"},
            {5030, "Фонарик"},
            {5031, "Глушитель"},
            {5032, "Глушитель"},
            {5033, "Улучшенный ствол"},
            {5034, "Улучшенный ствол"},
            {5035, "Улучшенный ствол"},
            {5036, "Улучшенный ствол"},
            {5037, "Пламегаситель"},
            {5038, "Пламегаситель"},
            {5039, "Пламегаситель"},
            {5040, "Пламегаситель"},
            {5041, "Пламегаситель"},
            {5042, "Пламегаситель"},
            {5043, "Пламегаситель"},
            {5044, "Пламегаситель"},
            {5045, "Пламегаситель"},
            {5046, "Пламегаситель"},
            {5047, "Пламегаситель"},
            {5048, "Пламегаситель"},
            {5049, "Фонарик"},
            {5050, "Фонарик"},
            {5051, "Фонарик"},
            {5052, "Коллиматорный прицел"},
            {5053, "Коллиматорный прицел"},
            {5054, "Глушитель"},
            {5055, "Глушитель"},
            {5056, "Улучшенный ствол"},
            {5057, "Улучшенный ствол"},
            {5058, "Стандартный снайперский прицел"},
            {5059, "Стандартный снайперский фиксированный прицел"},
            {5060, "Улучшенный фиксированный высокоточный прицел"},
            {5061, "Улучшенный высокоточный прицел"},
            {5062, "Коллиматорный прицел"},
            {5063, "Голографический коллиматорный прицел"},
            {5064, "Голографический коллиматорный прицел"},
            {5065, "Голографический коллиматорный прицел"},
            {5066, "Голографический коллиматорный прицел"},
            {5067, "Улучшенный снайперский прицел"},
            {5068, "Голографический коллиматорный прицел"},
            {5069, "Голографический коллиматорный прицел"},
            {5070, "Прицел ночного видения"},
            {5071, "Коллиматорный прицел"},
            {5072, "Голографический коллиматорный прицел"},
            {5073, "Улучшенный голографический коллиматорный прицел"},
            {5074, "Улучшенный голографический коллиматорный прицел"},
            {5075, "Прицел с тепловизором"},
            {5076, "Коллиматорный прицел"},
            {5077, "Коллиматорный прицел"},
            {5078, "Улучшенный ствол"},
            {5079, "Глушитель"},
            {5080, "Глушитель"},
            {5081, "Расширенный магазин"},
            {5082, "Цифровой камуфляж"},
            {5083, "Мазковый камуфляж"},
            {5084, "Лесной камуфляж"},
            {5085, "Череп"},
            {5086, "Sessanta Nove"},
            {5087, "Perseus"},
            {5088, "Леопард"},
            {5089, "Зебра"},
            {5090, "Геометрический"},
            {5091, "Boom!"},
            {5092, "Флаг США"},
            {5093, "Расширенный магазин"},
            {5094, "Бронебойные патроны"},
            {5095, "Патроны FMJ"},
            {5096, "Зажигательные патроны"},
            {5097, "Трассирующие патроны"},
            {5098, "Золотая расцветка"},
            {5099, "Расширенный магазин"},
            {5100, "Расширенный магазин"},
            {5101, "Цифровой камуфляж"},
            {5102, "Мазковый камуфляж"},
            {5103, "Лесной камуфляж"},
            {5104, "Череп"},
            {5105, "Sessanta Nove"},
            {5106, "Perseus"},
            {5107, "Леопард"},
            {5108, "Зебра"},
            {5109, "Геометрический"},
            {5110, "Boom!"},
            {5111, "Флаг США"},
            {5112, "Расширенный магазин"},
            {5113, "Бронебойные патроны"},
            {5114, "Патроны FMJ"},
            {5115, "Зажигательные патроны"},
            {5116, "Трассирующие патроны"},
            {5117, "Золотая расцветка"},
            {5118, "Расширенный магазин"},
            {5119, "Глушитель"},
            {5120, "Расширенный магазин"},
            {5121, "Цифровой камуфляж"},
            {5122, "Мазковый камуфляж"},
            {5123, "Лесной камуфляж"},
            {5124, "Череп"},
            {5125, "Sessanta Nove"},
            {5126, "Perseus"},
            {5127, "Леопард"},
            {5128, "Зебра"},
            {5129, "Геометрический"},
            {5130, "Boom!"},
            {5131, "Флаг США"},
            {5132, "Расширенный магазин"},
            {5133, "Бронебойные патроны"},
            {5134, "Патроны FMJ"},
            {5135, "Зажигательные патроны"},
            {5136, "Трассирующие патроны"},
            {5137, "Расцветка с черепами"},
            {5138, "Расширенный магазин"},
            {5139, "Расширенный магазин"},
            {5140, "Расширенный магазин"},
            {5141, "Золотая расцветка"},
            {5142, "Расширенный магазин"},
            {5143, "Расширенный магазин"},
            {5144, "Расширенный магазин"},
            {5145, "Расширенный магазин"},
            {5146, "Серебристая расцветка"},
            {5147, "Расширенный магазин"},
            {5148, "Расширенный магазин"},
            {5149, "Цифровой камуфляж"},
            {5150, "Мазковый камуфляж"},
            {5151, "Лесной камуфляж"},
            {5152, "Череп"},
            {5153, "Sessanta Nove"},
            {5154, "Perseus"},
            {5155, "Леопард"},
            {5156, "Зебра"},
            {5157, "Геометрический"},
            {5158, "Boom!"},
            {5159, "Флаг США"},
            {5160, "Расширенный магазин"},
            {5161, "Бронебойные патроны"},
            {5162, "Взрывные патроны"},
            {5163, "Патроны FMJ"},
            {5164, "Зажигательные патроны"},
            {5165, "Расцветка Ballas"},
            {5166, "Расцветка Diamond"},
            {5167, "Расцветка Dollar"},
            {5168, "Расцветка Hate"},
            {5169, "Расцветка King"},
            {5170, "Расцветка Love"},
            {5171, "Расцветка Pimp"},
            {5172, "Расцветка Player"},
            {5173, "Расцветка Vagos"},
            {5174, "Расширенный магазин"},
            {5175, "Расширенный магазин"},
            {5176, "Расширенный магазин"},
            {5177, "Цифровой камуфляж"},
            {5178, "Мазковый камуфляж"},
            {5179, "Лесной камуфляж"},
            {5180, "Череп"},
            {5181, "Sessanta Nove"},
            {5182, "Perseus"},
            {5183, "Леопард"},
            {5184, "Зебра"},
            {5185, "Геометрический"},
            {5186, "Boom!"},
            {5187, "Флаг США"},
            {5188, "Расширенный магазин"},
            {5189, "Бронебойные патроны"},
            {5190, "Патроны FMJ"},
            {5191, "Зажигательные патроны"},
            {5192, "Трассирующие патроны"},
            {5193, "Золотая расцветка"},
            {5194, "Расширенный магазин"},
            {5195, "Золотая расцветка"},
            {5196, "Расширенный магазин"},
            {5197, "Золотая расцветка"},
            {5198, "Расширенный магазин"},
            {5199, "Стандартный прицел"},
            {5200, "Расширенный магазин"},
            {5201, "Расширенный магазин"},
            {5202, "Платиновая расцветка"},
            {5203, "Расширенный магазин"},
            {5204, "Цифровой камуфляж"},
            {5205, "Мазковый камуфляж"},
            {5206, "Мазковый камуфляж S"},
            {5207, "Лесной камуфляж"},
            {5208, "Лесной камуфляж S"},
            {5209, "Череп"},
            {5210, "Череп S"},
            {5211, "Sessanta Nove"},
            {5212, "Sessanta Nove S"},
            {5213, "Perseus"},
            {5214, "Perseus S"},
            {5215, "Леопард"},
            {5216, "Леопард S"},
            {5217, "Зебра"},
            {5218, "Зебра S"},
            {5219, "Геометрический"},
            {5220, "Геометрический S"},
            {5221, "Boom!"},
            {5222, "Boom! S"},
            {5223, "Флаг США"},
            {5224, "Флаг США S"},
            {5225, "Цифровой камуфляж S"},
            {5226, "Расширенный магазин"},
            {5227, "Патроны FMJ"},
            {5228, "Экспансивные патроны"},
            {5229, "Зажигательные патроны"},
            {5230, "Трассирующие патроны"},
            {5231, "Золотая расцветка"},
            {5232, "Цифровой камуфляж"},
            {5233, "Мазковый камуфляж"},
            {5234, "Лесной камуфляж"},
            {5235, "Череп"},
            {5236, "Sessanta Nove"},
            {5237, "Perseus"},
            {5238, "Леопард"},
            {5239, "Зебра"},
            {5240, "Геометрический"},
            {5241, "Boom!"},
            {5242, "Флаг США"},
            {5243, "Бронебойные патроны"},
            {5244, "Взрывные патроны"},
            {5245, "Экспансивные патроны"},
            {5246, "Зажигательные патроны"},
            {5247, "Золотая расцветка"},
            {5248, "Снежная расцветка"},
            {5249, "Цифровой камуфляж"},
            {5250, "Мазковый камуфляж"},
            {5251, "Лесной камуфляж"},
            {5252, "Череп"},
            {5253, "Sessanta Nove"},
            {5254, "Perseus"},
            {5255, "Леопард"},
            {5256, "Зебра"},
            {5257, "Геометрический"},
            {5258, "Boom!"},
            {5259, "Флаг США"},
            {5260, "Патроны FMJ"},
            {5261, "Экспансивные патроны"},
            {5262, "Зажигательные патроны"},
            {5263, "Трассирующие патроны"},
            {5264, "Расцветка BOSS"},
            {5265, "РАсцветка GOON"},
            {5266, "Золотая расцветка"},
            {5267, "Расширенный магазин"},
            {5268, "Расширенный магазин"},
            {5269, "Цифровой камуфляж"},
            {5270, "Мазковый камуфляж"},
            {5271, "Лесной камуфляж"},
            {5272, "Череп"},
            {5273, "Sessanta Nove"},
            {5274, "Perseus"},
            {5275, "Леопард"},
            {5276, "Зебра"},
            {5277, "Геометрический"},
            {5278, "Boom!"},
            {5279, "Флаг США"},
            {5280, "Расширенный магазин"},
            {5281, "Патроны FMJ"},
            {5282, "Экспансивные патроны"},
            {5283, "Зажигательные патроны"},
            {5284, "Трассирующие патроны"},
            {5285, "Золотая расцветка"},
            {5286, "Уникальная белая расцветка"},
            {5287, "Расширенный магазин"},
            {5288, "Цифровой камуфляж"},
            {5289, "Мазковый камуфляж"},
            {5290, "Мазковый камуфляж S"},
            {5291, "Лесной камуфляж"},
            {5292, "Лесной камуфляж S"},
            {5293, "Череп"},
            {5294, "Череп S"},
            {5295, "Sessanta Nove"},
            {5296, "Sessanta Nove S"},
            {5297, "Perseus"},
            {5298, "Perseus S"},
            {5299, "Леопард"},
            {5300, "Леопард S"},
            {5301, "Зебра"},
            {5302, "Зебра S"},
            {5303, "Геометрический"},
            {5304, "Геометрический S"},
            {5305, "Boom!"},
            {5306, "Boom! S"},
            {5307, "Флаг США"},
            {5308, "Флаг США S"},
            {5309, "Цифровой камуфляж S"},
            {5310, "Расширенный магазин"},
            {5311, "Патроны FMJ"},
            {5312, "Экспансивные патроны"},
            {5313, "Зажигательные патроны"},
            {5314, "Трассирующие патроны"},
            {5315, "Древесная отделка"},
            {5316, "Расширенный магазин"},
            {5317, "Расширенный магазин"},
            {5318, "Цифровой камуфляж"},
            {5319, "Мазковый камуфляж"},
            {5320, "Лесной камуфляж"},
            {5321, "Череп"},
            {5322, "Sessanta Nove"},
            {5323, "Perseus"},
            {5324, "Леопард"},
            {5325, "Зебра"},
            {5326, "Геометрический"},
            {5327, "Boom!"},
            {5328, "Флаг США"},
            {5329, "Расширенный магазин"},
            {5330, "Бронебойные патроны"},
            {5331, "Патроны FMJ"},
            {5332, "Зажигательные патроны"},
            {5333, "Трассирующие патроны"},
            {5334, "Расцветка с черепами"},
            {5335, "Золотая расцветка"},
            {5336, "Расцветка с черепом"},
            {5337, "Расширенный магазин"},

            {6000, "Металодетектор"},
            {6001, "Металодетектор MK2" },
            {6002, "Металодетектор MK3" },

            {6010, "Лопата" },
            {6011, "Лопата MK2" },
            {6012, "Лопата MK3" },

            {6020, "Старая монета" },
            {6021, "Гильза" },
            {6022, "Крышка" },
            {6023, "Металлолом" },
            {6024, "Медный самородок" },
            {6025, "Железный самородок" },
            {6026, "Оловянный самородок" },

            {6030,"Медный провод" },
            {6031, "Старое украшение"},
            {6032, "Золотой самородок" },
            {6033, "Коллекционная монета"},
            {6034, "Древняя статуэтка"},
            {6035, "Золотая подкова" },
            {6036, "Реликвия" },

            {6040, "Железная запчасть" },
            {6041, "Медная запчасть" },
            {6042, "Оловянная запчасть" },
            {6043, "Бронзовая запчасть" },

            {6050, "Станок" },
            {6051 , "Плавильня"},
            {6052, "Прецеживатель" },
            {6053, "Сборщик деталей" },
            {6054, "Улучшение станка I"},
            {6055, "Улучшение станка II"},
        };
        public static Dictionary<int, string> ItemsDescriptions = new Dictionary<int, string>();
        public static Dictionary<int, List<bool>> ItemsSlots = new Dictionary<int, List<bool>>();
        public static Dictionary<ItemType, uint> ItemModels = new Dictionary<ItemType, uint>()
        {
            { ItemType.Hat, 1619813869 },
            { ItemType.Mask, 3887136870 },
            { ItemType.Gloves, 3125389411 },
            { ItemType.Leg, 2086911125 },
            { ItemType.Bag, NAPI.Util.GetHashKey("hei_p_m_bag_var18_bus_s") },
            { ItemType.Feet, 1682675077 },
            { ItemType.Jewelry, 2329969874 },
            { ItemType.Undershit, 578126062 },
            { ItemType.BodyArmor, 701173564 },
            { ItemType.Unknown, 0000000 },
            { ItemType.Top, 3038378640 },
            { ItemType.Glasses, 2329969874 },
            { ItemType.Accessories, 2329969874 },

            { ItemType.Drugs, 4293279169 },
            { ItemType.Material, 3045218749 },
            { ItemType.Debug, 0000000 },
            { ItemType.Bandage, 678958360 },
            { ItemType.HealthKit, 678958360 },
            { ItemType.GasCan, 786272259 },
            { ItemType.Сrisps, 2564432314 },
            { ItemType.Beer, 1940235411 },
            { ItemType.Pizza, 604847691 },
            { ItemType.Burger, 2240524752 },
            { ItemType.HotDog, 2565741261 },
            { ItemType.Sandwich, 987331897 },
            { ItemType.eCola, 144995201 },
            { ItemType.Sprunk, 2973713592 },
            { ItemType.Lockpick, 977923025 },
            { ItemType.ArmyLockpick, 977923025 },
            { ItemType.Pocket, 3887136870 },
            { ItemType.Cuffs, 3887136870 },
            { ItemType.CarKey, 977923025 },
            { ItemType.Present, NAPI.Util.GetHashKey("prop_box_ammo07a") },
            { ItemType.KeyRing, 977923025 },

            { ItemType.RusDrink1, NAPI.Util.GetHashKey("prop_vodka_bottle") },
            { ItemType.RusDrink2, NAPI.Util.GetHashKey("prop_vodka_bottle") },
            { ItemType.RusDrink3, NAPI.Util.GetHashKey("prop_vodka_bottle") },
            { ItemType.YakDrink1, NAPI.Util.GetHashKey("prop_cs_beer_bot_02") },
            { ItemType.YakDrink2, NAPI.Util.GetHashKey("prop_wine_red") },
            { ItemType.YakDrink3, NAPI.Util.GetHashKey("p_whiskey_bottle_s") },
            { ItemType.LcnDrink1, NAPI.Util.GetHashKey("prop_wine_white") },
            { ItemType.LcnDrink2, NAPI.Util.GetHashKey("prop_vodka_bottle") },
            { ItemType.LcnDrink3, NAPI.Util.GetHashKey("prop_wine_red") },
            { ItemType.ArmDrink1, NAPI.Util.GetHashKey("prop_bottle_cognac") },
            { ItemType.ArmDrink2, NAPI.Util.GetHashKey("prop_bottle_cognac") },
            { ItemType.ArmDrink3, NAPI.Util.GetHashKey("prop_bottle_cognac") },
            { ItemType.WaterBottle, NAPI.Util.GetHashKey("prop_bottle_cognac") },
            { ItemType.RepairBox, NAPI.Util.GetHashKey("w_am_case") },
            { ItemType.SmallHealthKit, 678958360 },

            { ItemType.CasinoChips, 3045218749 },
            {ItemType.Grenade,NAPI.Util.GetHashKey("prop_vodka_bottle")},

            { ItemType.Pistol, NAPI.Util.GetHashKey("w_pi_pistol") },
            { ItemType.Combatpistol, NAPI.Util.GetHashKey("w_pi_Combatpistol") },
            { ItemType.Pistol50, NAPI.Util.GetHashKey("w_pi_pistol50") },
            { ItemType.Snspistol, NAPI.Util.GetHashKey("w_pi_sns_pistol") },
            { ItemType.Heavypistol, NAPI.Util.GetHashKey("w_pi_Heavypistol") },
            { ItemType.Vintagepistol, NAPI.Util.GetHashKey("w_pi_vintage_pistol") },
            { ItemType.Marksmanpistol, NAPI.Util.GetHashKey("w_pi_singleshot") },
            { ItemType.Revolver, NAPI.Util.GetHashKey("w_pi_revolver") },
            { ItemType.Appistol, NAPI.Util.GetHashKey("w_pi_Appistol") },
            { ItemType.Stungun, NAPI.Util.GetHashKey("w_pi_Stungun") },
            { ItemType.Flaregun, NAPI.Util.GetHashKey("w_pi_Flaregun") },
            { ItemType.Doubleaction, NAPI.Util.GetHashKey("mk2") },
            { ItemType.Pistol_mk2, NAPI.Util.GetHashKey("w_pi_pistolmk2") },
            { ItemType.Snspistol_mk2, NAPI.Util.GetHashKey("w_pi_sns_pistolmk2") },
            { ItemType.Revolver_mk2, NAPI.Util.GetHashKey("w_pi_revolvermk2") },
            { ItemType.Microsmg, NAPI.Util.GetHashKey("w_sb_Microsmg") },
            { ItemType.Machinepistol, NAPI.Util.GetHashKey("w_sb_compactSmg") },
            { ItemType.Smg, NAPI.Util.GetHashKey("w_sb_Smg") },
            { ItemType.Assaultsmg, NAPI.Util.GetHashKey("w_sb_Assaultsmg") },
            { ItemType.Combatpdw, NAPI.Util.GetHashKey("w_sb_pdw") },
            { ItemType.Mg, NAPI.Util.GetHashKey("w_mg_mg") },
            { ItemType.Combatmg, NAPI.Util.GetHashKey("w_mg_Combatmg") },
            { ItemType.Gusenberg, NAPI.Util.GetHashKey("w_sb_gusenberg") },
            { ItemType.Minismg, NAPI.Util.GetHashKey("w_sb_Minismg") },
            { ItemType.Smg_mk2, NAPI.Util.GetHashKey("w_sb_smgmk2") },
            { ItemType.Combatmg_mk2, NAPI.Util.GetHashKey("w_mg_combatmgmk2") },
            { ItemType.Assaultrifle, NAPI.Util.GetHashKey("w_ar_Assaultrifle") },
            { ItemType.Carbinerifle, NAPI.Util.GetHashKey("w_ar_Carbinerifle") },
            { ItemType.Advancedrifle, NAPI.Util.GetHashKey("w_ar_Advancedrifle") },
            { ItemType.Specialcarbine, NAPI.Util.GetHashKey("w_ar_Specialcarbine") },
            { ItemType.Bullpuprifle, NAPI.Util.GetHashKey("w_ar_Bullpuprifle") },
            { ItemType.Compactrifle, NAPI.Util.GetHashKey("w_ar_Assaultrifle_Smg") },
            { ItemType.Assaultrifle_mk2, NAPI.Util.GetHashKey("w_ar_assaultriflemk2") },
            { ItemType.Carbinerifle_mk2, NAPI.Util.GetHashKey("w_ar_carbineriflemk2") },
            { ItemType.Specialcarbine_mk2, NAPI.Util.GetHashKey("w_ar_specialcarbinemk2") },
            { ItemType.Bullpuprifle_mk2, NAPI.Util.GetHashKey("w_ar_bullpupriflemk2") },
            { ItemType.Sniperrifle, NAPI.Util.GetHashKey("w_sr_Sniperrifle") },
            { ItemType.Heavysniper, NAPI.Util.GetHashKey("w_sr_Heavysniper") },
            { ItemType.Marksmanrifle, NAPI.Util.GetHashKey("w_sr_Marksmanrifle") },
            { ItemType.Heavysniper_mk2, NAPI.Util.GetHashKey("w_sr_heavysnipermk2") },
            { ItemType.Marksmanrifle_mk2, NAPI.Util.GetHashKey("w_sr_marksmanriflemk2") },
            { ItemType.Pumpshotgun, NAPI.Util.GetHashKey("w_sg_Pumpshotgun") },
            { ItemType.Sawnoffshotgun, NAPI.Util.GetHashKey("w_sg_sawnoff") },
            { ItemType.Bullpupshotgun, NAPI.Util.GetHashKey("w_sg_Bullpupshotgun") },
            { ItemType.Assaultshotgun, NAPI.Util.GetHashKey("w_sg_Assaultshotgun") },
            { ItemType.Musket, NAPI.Util.GetHashKey("w_ar_musket") },
            { ItemType.Heavyshotgun, NAPI.Util.GetHashKey("w_sg_Heavyshotgun") },
            { ItemType.Dbshotgun, NAPI.Util.GetHashKey("w_sg_doublebarrel") },
            { ItemType.Autoshotgun, NAPI.Util.GetHashKey("mk2") },
            { ItemType.Pumpshotgun_mk2, NAPI.Util.GetHashKey("w_sg_pumpshotgunmk2") },
            { ItemType.Rpg, NAPI.Util.GetHashKey("w_lr_rpg")},
            { ItemType.Knife, NAPI.Util.GetHashKey("w_me_knife_01") },
            { ItemType.Nightstick, NAPI.Util.GetHashKey("w_me_nightstick") },
            { ItemType.Hammer, NAPI.Util.GetHashKey("w_me_hammer") },
            { ItemType.Bat, NAPI.Util.GetHashKey("w_me_bat") },
            { ItemType.Crowbar, NAPI.Util.GetHashKey("w_me_crowbar") },
            { ItemType.Golfclub, NAPI.Util.GetHashKey("w_me_gclub") },
            { ItemType.Bottle, NAPI.Util.GetHashKey("w_me_bottle") },
            { ItemType.Dagger, NAPI.Util.GetHashKey("w_me_dagger") },
            { ItemType.Hatchet, NAPI.Util.GetHashKey("w_me_hatchet") },
            { ItemType.Knuckle, NAPI.Util.GetHashKey("w_me_knuckle") },
            { ItemType.Machete, NAPI.Util.GetHashKey("prop_ld_w_me_machette") },
            { ItemType.Flashlight, NAPI.Util.GetHashKey("w_me_flashlight") },
            { ItemType.Switchblade, NAPI.Util.GetHashKey("w_me_Switchblade") },
            { ItemType.Poolcue, NAPI.Util.GetHashKey("prop_pool_cue") },
            { ItemType.Wrench, NAPI.Util.GetHashKey("prop_cs_wrench") },
            { ItemType.Battleaxe, NAPI.Util.GetHashKey("w_me_battleaxe") },

            { ItemType.Ceramicpistol, NAPI.Util.GetHashKey("w_pi_ceramic_pistol") },
            { ItemType.Combatshotgun, NAPI.Util.GetHashKey("w_sg_pumpshotgunh4") },
            { ItemType.Militaryrifle, NAPI.Util.GetHashKey("w_ar_bullpuprifleh4") },


            { ItemType.PistolAmmo, NAPI.Util.GetHashKey("w_am_case") },
            { ItemType.RiflesAmmo, NAPI.Util.GetHashKey("w_am_case") },
            { ItemType.ShotgunsAmmo, NAPI.Util.GetHashKey("w_am_case") },
            { ItemType.SMGAmmo, NAPI.Util.GetHashKey("w_am_case") },
            { ItemType.SniperAmmo, NAPI.Util.GetHashKey("w_am_case") },

            /* Fishing */
            { ItemType.Rod, NAPI.Util.GetHashKey("prop_fishing_rod_01") },
            { ItemType.RodUpgrade, NAPI.Util.GetHashKey("prop_fishing_rod_01") },
            { ItemType.RodMK2, NAPI.Util.GetHashKey("prop_fishing_rod_01") },
            { ItemType.Bait, NAPI.Util.GetHashKey("prop_cs_clothes_box") },
            { ItemType.Naz, NAPI.Util.GetHashKey("ng_proc_paintcan02a") },
            { ItemType.Kyndja, NAPI.Util.GetHashKey("prop_starfish_01") },
            { ItemType.Sig, NAPI.Util.GetHashKey("prop_starfish_01") },
            { ItemType.Omyl, NAPI.Util.GetHashKey("prop_starfish_01") },
            { ItemType.Nerka, NAPI.Util.GetHashKey("prop_starfish_01") },
            { ItemType.Forel, NAPI.Util.GetHashKey("prop_starfish_01") },
            { ItemType.Ship, NAPI.Util.GetHashKey("prop_starfish_01") },
            { ItemType.Lopatonos, NAPI.Util.GetHashKey("prop_starfish_01") },
            { ItemType.Osetr, NAPI.Util.GetHashKey("prop_starfish_01") },
            { ItemType.Semga, NAPI.Util.GetHashKey("prop_starfish_01") },
            { ItemType.Servyga, NAPI.Util.GetHashKey("prop_starfish_01") },
            { ItemType.Beluga, NAPI.Util.GetHashKey("prop_starfish_01") },
            { ItemType.Taimen, NAPI.Util.GetHashKey("prop_starfish_01") },
            { ItemType.Sterlyad, NAPI.Util.GetHashKey("prop_starfish_01") },
            { ItemType.Ydilshik, NAPI.Util.GetHashKey("prop_starfish_01") },
            { ItemType.Hailiod, NAPI.Util.GetHashKey("prop_starfish_01") },
            { ItemType.Keta, NAPI.Util.GetHashKey("prop_starfish_01") },
            { ItemType.Gorbysha, NAPI.Util.GetHashKey("prop_starfish_01") },

            //Farmer Job Items
            { ItemType.Hay, NAPI.Util.GetHashKey("prop_haybale_01") },
            { ItemType.Seed, NAPI.Util.GetHashKey("ch_prop_ch_moneybag_01a") },

            { ItemType.Payek, NAPI.Util.GetHashKey("prop_ff_noodle_02") },

            { ItemType.LSPDDrone, NAPI.Util.GetHashKey("ch_prop_casino_drone_02a") },
            { ItemType.Drone, NAPI.Util.GetHashKey("ch_prop_casino_drone_01a") },

            {ItemType.NumberPlate, 0000000000 },
            {ItemType.SimCard, 0000000000 },
            {ItemType.ProductBox, NAPI.Util.GetHashKey("v_ind_meatboxsml_02") },
            {ItemType.TrashBag, NAPI.Util.GetHashKey("hei_prop_heist_binbag") },
            {ItemType.CocaLeaves, 000000000},
            {ItemType.Cocaine, 000000000},
            {ItemType.DrugBookMark, 000000000},

            { ItemType.Subject, 000000000 },


            // [Modification] Миша, Дима, Проверить, дропнуть модификацию, как будет выглядеть. Найдет ли... иначе заменить на какой нить бокс
            {ItemType.ADVANCEDRIFLE_CLIP_02, NAPI.Util.GetHashKey("COMPONENT_ADVANCEDRIFLE_CLIP_02")},
            {ItemType.ADVANCEDRIFLE_VARMOD_LUXE, NAPI.Util.GetHashKey("COMPONENT_ADVANCEDRIFLE_VARMOD_LUXE")},
            {ItemType.APPISTOL_CLIP_02, NAPI.Util.GetHashKey("COMPONENT_APPISTOL_CLIP_02")},
            {ItemType.APPISTOL_VARMOD_LUXE, NAPI.Util.GetHashKey("COMPONENT_APPISTOL_VARMOD_LUXE")},
            {ItemType.ASSAULTRIFLE_CLIP_02, NAPI.Util.GetHashKey("COMPONENT_ASSAULTRIFLE_CLIP_02")},
            {ItemType.ASSAULTRIFLE_CLIP_03, NAPI.Util.GetHashKey("COMPONENT_ASSAULTRIFLE_CLIP_03")},
            {ItemType.ASSAULTRIFLE_MK2_CAMO, NAPI.Util.GetHashKey("COMPONENT_ASSAULTRIFLE_MK2_CAMO")},
            {ItemType.ASSAULTRIFLE_MK2_CAMO_02, NAPI.Util.GetHashKey("COMPONENT_ASSAULTRIFLE_MK2_CAMO_02")},
            {ItemType.ASSAULTRIFLE_MK2_CAMO_03, NAPI.Util.GetHashKey("COMPONENT_ASSAULTRIFLE_MK2_CAMO_03")},
            {ItemType.ASSAULTRIFLE_MK2_CAMO_04, NAPI.Util.GetHashKey("COMPONENT_ASSAULTRIFLE_MK2_CAMO_04")},
            {ItemType.ASSAULTRIFLE_MK2_CAMO_05, NAPI.Util.GetHashKey("COMPONENT_ASSAULTRIFLE_MK2_CAMO_05")},
            {ItemType.ASSAULTRIFLE_MK2_CAMO_06, NAPI.Util.GetHashKey("COMPONENT_ASSAULTRIFLE_MK2_CAMO_06")},
            {ItemType.ASSAULTRIFLE_MK2_CAMO_07, NAPI.Util.GetHashKey("COMPONENT_ASSAULTRIFLE_MK2_CAMO_07")},
            {ItemType.ASSAULTRIFLE_MK2_CAMO_08, NAPI.Util.GetHashKey("COMPONENT_ASSAULTRIFLE_MK2_CAMO_08")},
            {ItemType.ASSAULTRIFLE_MK2_CAMO_09, NAPI.Util.GetHashKey("COMPONENT_ASSAULTRIFLE_MK2_CAMO_09")},
            {ItemType.ASSAULTRIFLE_MK2_CAMO_10, NAPI.Util.GetHashKey("COMPONENT_ASSAULTRIFLE_MK2_CAMO_10")},
            {ItemType.ASSAULTRIFLE_MK2_CAMO_IND_01, NAPI.Util.GetHashKey("COMPONENT_ASSAULTRIFLE_MK2_CAMO_IND_01")},
            {ItemType.ASSAULTRIFLE_MK2_CLIP_02, NAPI.Util.GetHashKey("COMPONENT_ASSAULTRIFLE_MK2_CLIP_02")},
            {ItemType.ASSAULTRIFLE_MK2_CLIP_ARMORPIERCING, NAPI.Util.GetHashKey("COMPONENT_ASSAULTRIFLE_MK2_CLIP_ARMORPIERCING")},
            {ItemType.ASSAULTRIFLE_MK2_CLIP_FMJ, NAPI.Util.GetHashKey("COMPONENT_ASSAULTRIFLE_MK2_CLIP_FMJ")},
            {ItemType.ASSAULTRIFLE_MK2_CLIP_INCENDIARY, NAPI.Util.GetHashKey("COMPONENT_ASSAULTRIFLE_MK2_CLIP_INCENDIARY")},
            {ItemType.ASSAULTRIFLE_MK2_CLIP_TRACER, NAPI.Util.GetHashKey("COMPONENT_ASSAULTRIFLE_MK2_CLIP_TRACER")},
            {ItemType.ASSAULTRIFLE_VARMOD_LUXE, NAPI.Util.GetHashKey("COMPONENT_ASSAULTRIFLE_VARMOD_LUXE")},
            {ItemType.ASSAULTSHOTGUN_CLIP_02, NAPI.Util.GetHashKey("COMPONENT_ASSAULTSHOTGUN_CLIP_02")},
            {ItemType.ASSAULTSMG_CLIP_02, NAPI.Util.GetHashKey("COMPONENT_ASSAULTSMG_CLIP_02")},
            {ItemType.ASSAULTSMG_VARMOD_LOWRIDER, NAPI.Util.GetHashKey("COMPONENT_ASSAULTSMG_VARMOD_LOWRIDER")},
            {ItemType.AT_AR_AFGRIP, NAPI.Util.GetHashKey("COMPONENT_AT_AR_AFGRIP")},
            {ItemType.AT_AR_AFGRIP_02, NAPI.Util.GetHashKey("COMPONENT_AT_AR_AFGRIP_02")},
            {ItemType.AT_AR_BARREL_02, NAPI.Util.GetHashKey("COMPONENT_AT_AR_BARREL_02")},
            {ItemType.AT_AR_FLSH, NAPI.Util.GetHashKey("COMPONENT_AT_AR_FLSH")},
            {ItemType.AT_AR_SUPP, NAPI.Util.GetHashKey("COMPONENT_AT_AR_SUPP")},
            {ItemType.AT_AR_SUPP_02, NAPI.Util.GetHashKey("COMPONENT_AT_AR_SUPP_02")},
            {ItemType.AT_BP_BARREL_02, NAPI.Util.GetHashKey("COMPONENT_AT_BP_BARREL_02")},
            {ItemType.AT_CR_BARREL_02, NAPI.Util.GetHashKey("COMPONENT_AT_CR_BARREL_02")},
            {ItemType.AT_MG_BARREL_02, NAPI.Util.GetHashKey("COMPONENT_AT_MG_BARREL_02")},
            {ItemType.AT_MRFL_BARREL_02, NAPI.Util.GetHashKey("COMPONENT_AT_MRFL_BARREL_02")},
            {ItemType.AT_MUZZLE_01, NAPI.Util.GetHashKey("COMPONENT_AT_MUZZLE_01")},
            {ItemType.AT_MUZZLE_02, NAPI.Util.GetHashKey("COMPONENT_AT_MUZZLE_02")},
            {ItemType.AT_MUZZLE_03, NAPI.Util.GetHashKey("COMPONENT_AT_MUZZLE_03")},
            {ItemType.AT_MUZZLE_04, NAPI.Util.GetHashKey("COMPONENT_AT_MUZZLE_04")},
            {ItemType.AT_MUZZLE_05, NAPI.Util.GetHashKey("COMPONENT_AT_MUZZLE_05")},
            {ItemType.AT_MUZZLE_06, NAPI.Util.GetHashKey("COMPONENT_AT_MUZZLE_06")},
            {ItemType.AT_MUZZLE_07, NAPI.Util.GetHashKey("COMPONENT_AT_MUZZLE_07")},
            {ItemType.AT_MUZZLE_08, NAPI.Util.GetHashKey("COMPONENT_AT_MUZZLE_08")},
            {ItemType.AT_MUZZLE_09, NAPI.Util.GetHashKey("COMPONENT_AT_MUZZLE_09")},
            {ItemType.AT_PI_COMP, NAPI.Util.GetHashKey("COMPONENT_AT_PI_COMP")},
            {ItemType.AT_PI_COMP_02, NAPI.Util.GetHashKey("COMPONENT_AT_PI_COMP_02")},
            {ItemType.AT_PI_COMP_03, NAPI.Util.GetHashKey("COMPONENT_AT_PI_COMP_03")},
            {ItemType.AT_PI_FLSH, NAPI.Util.GetHashKey("COMPONENT_AT_PI_FLSH")},
            {ItemType.AT_PI_FLSH_02, NAPI.Util.GetHashKey("COMPONENT_AT_PI_FLSH_02")},
            {ItemType.AT_PI_FLSH_03, NAPI.Util.GetHashKey("COMPONENT_AT_PI_FLSH_03")},
            {ItemType.AT_PI_RAIL, NAPI.Util.GetHashKey("COMPONENT_AT_PI_RAIL")},
            {ItemType.AT_PI_RAIL_02, NAPI.Util.GetHashKey("COMPONENT_AT_PI_RAIL_02")},
            {ItemType.AT_PI_SUPP, NAPI.Util.GetHashKey("COMPONENT_AT_PI_SUPP")},
            {ItemType.AT_PI_SUPP_02, NAPI.Util.GetHashKey("COMPONENT_AT_PI_SUPP_02")},
            {ItemType.AT_SB_BARREL_02, NAPI.Util.GetHashKey("COMPONENT_AT_SB_BARREL_02")},
            {ItemType.AT_SC_BARREL_02, NAPI.Util.GetHashKey("COMPONENT_AT_SC_BARREL_02")},
            {ItemType.AT_SCOPE_LARGE, NAPI.Util.GetHashKey("COMPONENT_AT_SCOPE_LARGE")},
            {ItemType.AT_SCOPE_LARGE_FIXED_ZOOM, NAPI.Util.GetHashKey("COMPONENT_AT_SCOPE_LARGE_FIXED_ZOOM")},
            {ItemType.AT_SCOPE_LARGE_FIXED_ZOOM_MK2, NAPI.Util.GetHashKey("COMPONENT_AT_SCOPE_LARGE_FIXED_ZOOM_MK2")},
            {ItemType.AT_SCOPE_LARGE_MK2, NAPI.Util.GetHashKey("COMPONENT_AT_SCOPE_LARGE_MK2")},
            {ItemType.AT_SCOPE_MACRO, NAPI.Util.GetHashKey("COMPONENT_AT_SCOPE_MACRO")},
            {ItemType.AT_SCOPE_MACRO_02, NAPI.Util.GetHashKey("COMPONENT_AT_SCOPE_MACRO_02")},
            {ItemType.AT_SCOPE_MACRO_02_MK2, NAPI.Util.GetHashKey("COMPONENT_AT_SCOPE_MACRO_02_MK2")},
            {ItemType.AT_SCOPE_MACRO_02_SMG_MK2, NAPI.Util.GetHashKey("COMPONENT_AT_SCOPE_MACRO_02_SMG_MK2")},
            {ItemType.AT_SCOPE_MACRO_MK2, NAPI.Util.GetHashKey("COMPONENT_AT_SCOPE_MACRO_MK2")},
            {ItemType.AT_SCOPE_MAX, NAPI.Util.GetHashKey("COMPONENT_AT_SCOPE_MAX")},
            {ItemType.AT_SCOPE_MEDIUM, NAPI.Util.GetHashKey("COMPONENT_AT_SCOPE_MEDIUM")},
            {ItemType.AT_SCOPE_MEDIUM_MK2, NAPI.Util.GetHashKey("COMPONENT_AT_SCOPE_MEDIUM_MK2")},
            {ItemType.AT_SCOPE_NV, NAPI.Util.GetHashKey("COMPONENT_AT_SCOPE_NV")},
            {ItemType.AT_SCOPE_SMALL, NAPI.Util.GetHashKey("COMPONENT_AT_SCOPE_SMALL")},
            {ItemType.AT_SCOPE_SMALL_02, NAPI.Util.GetHashKey("COMPONENT_AT_SCOPE_SMALL_02")},
            {ItemType.AT_SCOPE_SMALL_MK2, NAPI.Util.GetHashKey("COMPONENT_AT_SCOPE_SMALL_MK2")},
            {ItemType.AT_SCOPE_SMALL_SMG_MK2, NAPI.Util.GetHashKey("COMPONENT_AT_SCOPE_SMALL_SMG_MK2")},
            {ItemType.AT_SCOPE_THERMAL, NAPI.Util.GetHashKey("COMPONENT_AT_SCOPE_THERMAL")},
            {ItemType.AT_SIGHTS, NAPI.Util.GetHashKey("COMPONENT_AT_SIGHTS")},
            {ItemType.AT_SIGHTS_SMG, NAPI.Util.GetHashKey("COMPONENT_AT_SIGHTS_SMG")},
            {ItemType.AT_SR_BARREL_02, NAPI.Util.GetHashKey("COMPONENT_AT_SR_BARREL_02")},
            {ItemType.AT_SR_SUPP, NAPI.Util.GetHashKey("COMPONENT_AT_SR_SUPP")},
            {ItemType.AT_SR_SUPP_03, NAPI.Util.GetHashKey("COMPONENT_AT_SR_SUPP_03")},
            {ItemType.BULLPUPRIFLE_CLIP_02, NAPI.Util.GetHashKey("COMPONENT_BULLPUPRIFLE_CLIP_02")},
            {ItemType.BULLPUPRIFLE_MK2_CAMO, NAPI.Util.GetHashKey("COMPONENT_BULLPUPRIFLE_MK2_CAMO")},
            {ItemType.BULLPUPRIFLE_MK2_CAMO_02, NAPI.Util.GetHashKey("COMPONENT_BULLPUPRIFLE_MK2_CAMO_02")},
            {ItemType.BULLPUPRIFLE_MK2_CAMO_03, NAPI.Util.GetHashKey("COMPONENT_BULLPUPRIFLE_MK2_CAMO_03")},
            {ItemType.BULLPUPRIFLE_MK2_CAMO_04, NAPI.Util.GetHashKey("COMPONENT_BULLPUPRIFLE_MK2_CAMO_04")},
            {ItemType.BULLPUPRIFLE_MK2_CAMO_05, NAPI.Util.GetHashKey("COMPONENT_BULLPUPRIFLE_MK2_CAMO_05")},
            {ItemType.BULLPUPRIFLE_MK2_CAMO_06, NAPI.Util.GetHashKey("COMPONENT_BULLPUPRIFLE_MK2_CAMO_06")},
            {ItemType.BULLPUPRIFLE_MK2_CAMO_07, NAPI.Util.GetHashKey("COMPONENT_BULLPUPRIFLE_MK2_CAMO_07")},
            {ItemType.BULLPUPRIFLE_MK2_CAMO_08, NAPI.Util.GetHashKey("COMPONENT_BULLPUPRIFLE_MK2_CAMO_08")},
            {ItemType.BULLPUPRIFLE_MK2_CAMO_09, NAPI.Util.GetHashKey("COMPONENT_BULLPUPRIFLE_MK2_CAMO_09")},
            {ItemType.BULLPUPRIFLE_MK2_CAMO_10, NAPI.Util.GetHashKey("COMPONENT_BULLPUPRIFLE_MK2_CAMO_10")},
            {ItemType.BULLPUPRIFLE_MK2_CAMO_IND_01, NAPI.Util.GetHashKey("COMPONENT_BULLPUPRIFLE_MK2_CAMO_IND_01")},
            {ItemType.BULLPUPRIFLE_MK2_CLIP_02, NAPI.Util.GetHashKey("COMPONENT_BULLPUPRIFLE_MK2_CLIP_02")},
            {ItemType.BULLPUPRIFLE_MK2_CLIP_ARMORPIERCING, NAPI.Util.GetHashKey("COMPONENT_BULLPUPRIFLE_MK2_CLIP_ARMORPIERCING")},
            {ItemType.BULLPUPRIFLE_MK2_CLIP_FMJ, NAPI.Util.GetHashKey("COMPONENT_BULLPUPRIFLE_MK2_CLIP_FMJ")},
            {ItemType.BULLPUPRIFLE_MK2_CLIP_INCENDIARY, NAPI.Util.GetHashKey("COMPONENT_BULLPUPRIFLE_MK2_CLIP_INCENDIARY")},
            {ItemType.BULLPUPRIFLE_MK2_CLIP_TRACER, NAPI.Util.GetHashKey("COMPONENT_BULLPUPRIFLE_MK2_CLIP_TRACER")},
            {ItemType.BULLPUPRIFLE_VARMOD_LOW, NAPI.Util.GetHashKey("COMPONENT_BULLPUPRIFLE_VARMOD_LOW")},
            {ItemType.CARBINERIFLE_CLIP_02, NAPI.Util.GetHashKey("COMPONENT_CARBINERIFLE_CLIP_02")},
            {ItemType.CARBINERIFLE_CLIP_03, NAPI.Util.GetHashKey("COMPONENT_CARBINERIFLE_CLIP_03")},
            {ItemType.CARBINERIFLE_MK2_CAMO, NAPI.Util.GetHashKey("COMPONENT_CARBINERIFLE_MK2_CAMO")},
            {ItemType.CARBINERIFLE_MK2_CAMO_02, NAPI.Util.GetHashKey("COMPONENT_CARBINERIFLE_MK2_CAMO_02")},
            {ItemType.CARBINERIFLE_MK2_CAMO_03, NAPI.Util.GetHashKey("COMPONENT_CARBINERIFLE_MK2_CAMO_03")},
            {ItemType.CARBINERIFLE_MK2_CAMO_04, NAPI.Util.GetHashKey("COMPONENT_CARBINERIFLE_MK2_CAMO_04")},
            {ItemType.CARBINERIFLE_MK2_CAMO_05, NAPI.Util.GetHashKey("COMPONENT_CARBINERIFLE_MK2_CAMO_05")},
            {ItemType.CARBINERIFLE_MK2_CAMO_06, NAPI.Util.GetHashKey("COMPONENT_CARBINERIFLE_MK2_CAMO_06")},
            {ItemType.CARBINERIFLE_MK2_CAMO_07, NAPI.Util.GetHashKey("COMPONENT_CARBINERIFLE_MK2_CAMO_07")},
            {ItemType.CARBINERIFLE_MK2_CAMO_08, NAPI.Util.GetHashKey("COMPONENT_CARBINERIFLE_MK2_CAMO_08")},
            {ItemType.CARBINERIFLE_MK2_CAMO_09, NAPI.Util.GetHashKey("COMPONENT_CARBINERIFLE_MK2_CAMO_09")},
            {ItemType.CARBINERIFLE_MK2_CAMO_10, NAPI.Util.GetHashKey("COMPONENT_CARBINERIFLE_MK2_CAMO_10")},
            {ItemType.CARBINERIFLE_MK2_CAMO_IND_01, NAPI.Util.GetHashKey("COMPONENT_CARBINERIFLE_MK2_CAMO_IND_01")},
            {ItemType.CARBINERIFLE_MK2_CLIP_02, NAPI.Util.GetHashKey("COMPONENT_CARBINERIFLE_MK2_CLIP_02")},
            {ItemType.CARBINERIFLE_MK2_CLIP_ARMORPIERCING, NAPI.Util.GetHashKey("COMPONENT_CARBINERIFLE_MK2_CLIP_ARMORPIERCING")},
            {ItemType.CARBINERIFLE_MK2_CLIP_FMJ, NAPI.Util.GetHashKey("COMPONENT_CARBINERIFLE_MK2_CLIP_FMJ")},
            {ItemType.CARBINERIFLE_MK2_CLIP_INCENDIARY, NAPI.Util.GetHashKey("COMPONENT_CARBINERIFLE_MK2_CLIP_INCENDIARY")},
            {ItemType.CARBINERIFLE_MK2_CLIP_TRACER, NAPI.Util.GetHashKey("COMPONENT_CARBINERIFLE_MK2_CLIP_TRACER")},
            {ItemType.CARBINERIFLE_VARMOD_LUXE, NAPI.Util.GetHashKey("COMPONENT_CARBINERIFLE_VARMOD_LUXE")},
            {ItemType.CERAMICPISTOL_CLIP_02, NAPI.Util.GetHashKey("COMPONENT_CERAMICPISTOL_CLIP_02")},
            {ItemType.CERAMICPISTOL_SUPP, NAPI.Util.GetHashKey("COMPONENT_CERAMICPISTOL_SUPP")},
            {ItemType.COMBATMG_CLIP_02, NAPI.Util.GetHashKey("COMPONENT_COMBATMG_CLIP_02")},
            {ItemType.COMBATMG_MK2_CAMO, NAPI.Util.GetHashKey("COMPONENT_COMBATMG_MK2_CAMO")},
            {ItemType.COMBATMG_MK2_CAMO_02, NAPI.Util.GetHashKey("COMPONENT_COMBATMG_MK2_CAMO_02")},
            {ItemType.COMBATMG_MK2_CAMO_03, NAPI.Util.GetHashKey("COMPONENT_COMBATMG_MK2_CAMO_03")},
            {ItemType.COMBATMG_MK2_CAMO_04, NAPI.Util.GetHashKey("COMPONENT_COMBATMG_MK2_CAMO_04")},
            {ItemType.COMBATMG_MK2_CAMO_05, NAPI.Util.GetHashKey("COMPONENT_COMBATMG_MK2_CAMO_05")},
            {ItemType.COMBATMG_MK2_CAMO_06, NAPI.Util.GetHashKey("COMPONENT_COMBATMG_MK2_CAMO_06")},
            {ItemType.COMBATMG_MK2_CAMO_07, NAPI.Util.GetHashKey("COMPONENT_COMBATMG_MK2_CAMO_07")},
            {ItemType.COMBATMG_MK2_CAMO_08, NAPI.Util.GetHashKey("COMPONENT_COMBATMG_MK2_CAMO_08")},
            {ItemType.COMBATMG_MK2_CAMO_09, NAPI.Util.GetHashKey("COMPONENT_COMBATMG_MK2_CAMO_09")},
            {ItemType.COMBATMG_MK2_CAMO_10, NAPI.Util.GetHashKey("COMPONENT_COMBATMG_MK2_CAMO_10")},
            {ItemType.COMBATMG_MK2_CAMO_IND_01, NAPI.Util.GetHashKey("COMPONENT_COMBATMG_MK2_CAMO_IND_01")},
            {ItemType.COMBATMG_MK2_CLIP_02, NAPI.Util.GetHashKey("COMPONENT_COMBATMG_MK2_CLIP_02")},
            {ItemType.COMBATMG_MK2_CLIP_ARMORPIERCING, NAPI.Util.GetHashKey("COMPONENT_COMBATMG_MK2_CLIP_ARMORPIERCING")},
            {ItemType.COMBATMG_MK2_CLIP_FMJ, NAPI.Util.GetHashKey("COMPONENT_COMBATMG_MK2_CLIP_FMJ")},
            {ItemType.COMBATMG_MK2_CLIP_INCENDIARY, NAPI.Util.GetHashKey("COMPONENT_COMBATMG_MK2_CLIP_INCENDIARY")},
            {ItemType.COMBATMG_MK2_CLIP_TRACER, NAPI.Util.GetHashKey("COMPONENT_COMBATMG_MK2_CLIP_TRACER")},
            {ItemType.COMBATMG_VARMOD_LOWRIDER, NAPI.Util.GetHashKey("COMPONENT_COMBATMG_VARMOD_LOWRIDER")},
            {ItemType.COMBATPDW_CLIP_02, NAPI.Util.GetHashKey("COMPONENT_COMBATPDW_CLIP_02")},
            {ItemType.COMBATPDW_CLIP_03, NAPI.Util.GetHashKey("COMPONENT_COMBATPDW_CLIP_03")},
            {ItemType.COMBATPISTOL_CLIP_02, NAPI.Util.GetHashKey("COMPONENT_COMBATPISTOL_CLIP_02")},
            {ItemType.COMBATPISTOL_VARMOD_LOWRIDER, NAPI.Util.GetHashKey("COMPONENT_COMBATPISTOL_VARMOD_LOWRIDER")},
            {ItemType.COMPACTRIFLE_CLIP_02, NAPI.Util.GetHashKey("COMPONENT_COMPACTRIFLE_CLIP_02")},
            {ItemType.COMPACTRIFLE_CLIP_03, NAPI.Util.GetHashKey("COMPONENT_COMPACTRIFLE_CLIP_03")},
            {ItemType.GUSENBERG_CLIP_02, NAPI.Util.GetHashKey("COMPONENT_GUSENBERG_CLIP_02")},
            {ItemType.HEAVYPISTOL_CLIP_02, NAPI.Util.GetHashKey("COMPONENT_HEAVYPISTOL_CLIP_02")},
            {ItemType.HEAVYPISTOL_VARMOD_LUXE, NAPI.Util.GetHashKey("COMPONENT_HEAVYPISTOL_VARMOD_LUXE")},
            {ItemType.HEAVYSHOTGUN_CLIP_02, NAPI.Util.GetHashKey("COMPONENT_HEAVYSHOTGUN_CLIP_02")},
            {ItemType.HEAVYSHOTGUN_CLIP_03, NAPI.Util.GetHashKey("COMPONENT_HEAVYSHOTGUN_CLIP_03")},
            {ItemType.HEAVYSNIPER_MK2_CAMO, NAPI.Util.GetHashKey("COMPONENT_HEAVYSNIPER_MK2_CAMO")},
            {ItemType.HEAVYSNIPER_MK2_CAMO_02, NAPI.Util.GetHashKey("COMPONENT_HEAVYSNIPER_MK2_CAMO_02")},
            {ItemType.HEAVYSNIPER_MK2_CAMO_03, NAPI.Util.GetHashKey("COMPONENT_HEAVYSNIPER_MK2_CAMO_03")},
            {ItemType.HEAVYSNIPER_MK2_CAMO_04, NAPI.Util.GetHashKey("COMPONENT_HEAVYSNIPER_MK2_CAMO_04")},
            {ItemType.HEAVYSNIPER_MK2_CAMO_05, NAPI.Util.GetHashKey("COMPONENT_HEAVYSNIPER_MK2_CAMO_05")},
            {ItemType.HEAVYSNIPER_MK2_CAMO_06, NAPI.Util.GetHashKey("COMPONENT_HEAVYSNIPER_MK2_CAMO_06")},
            {ItemType.HEAVYSNIPER_MK2_CAMO_07, NAPI.Util.GetHashKey("COMPONENT_HEAVYSNIPER_MK2_CAMO_07")},
            {ItemType.HEAVYSNIPER_MK2_CAMO_08, NAPI.Util.GetHashKey("COMPONENT_HEAVYSNIPER_MK2_CAMO_08")},
            {ItemType.HEAVYSNIPER_MK2_CAMO_09, NAPI.Util.GetHashKey("COMPONENT_HEAVYSNIPER_MK2_CAMO_09")},
            {ItemType.HEAVYSNIPER_MK2_CAMO_10, NAPI.Util.GetHashKey("COMPONENT_HEAVYSNIPER_MK2_CAMO_10")},
            {ItemType.HEAVYSNIPER_MK2_CAMO_IND_01, NAPI.Util.GetHashKey("COMPONENT_HEAVYSNIPER_MK2_CAMO_IND_01")},
            {ItemType.HEAVYSNIPER_MK2_CLIP_02, NAPI.Util.GetHashKey("COMPONENT_HEAVYSNIPER_MK2_CLIP_02")},
            {ItemType.HEAVYSNIPER_MK2_CLIP_ARMORPIERCING, NAPI.Util.GetHashKey("COMPONENT_HEAVYSNIPER_MK2_CLIP_ARMORPIERCING")},
            {ItemType.HEAVYSNIPER_MK2_CLIP_EXPLOSIVE, NAPI.Util.GetHashKey("COMPONENT_HEAVYSNIPER_MK2_CLIP_EXPLOSIVE")},
            {ItemType.HEAVYSNIPER_MK2_CLIP_FMJ, NAPI.Util.GetHashKey("COMPONENT_HEAVYSNIPER_MK2_CLIP_FMJ")},
            {ItemType.HEAVYSNIPER_MK2_CLIP_INCENDIARY, NAPI.Util.GetHashKey("COMPONENT_HEAVYSNIPER_MK2_CLIP_INCENDIARY")},
            {ItemType.KNUCKLE_VARMOD_BALLAS, NAPI.Util.GetHashKey("COMPONENT_KNUCKLE_VARMOD_BALLAS")},
            {ItemType.KNUCKLE_VARMOD_DIAMOND, NAPI.Util.GetHashKey("COMPONENT_KNUCKLE_VARMOD_DIAMOND")},
            {ItemType.KNUCKLE_VARMOD_DOLLAR, NAPI.Util.GetHashKey("COMPONENT_KNUCKLE_VARMOD_DOLLAR")},
            {ItemType.KNUCKLE_VARMOD_HATE, NAPI.Util.GetHashKey("COMPONENT_KNUCKLE_VARMOD_HATE")},
            {ItemType.KNUCKLE_VARMOD_KING, NAPI.Util.GetHashKey("COMPONENT_KNUCKLE_VARMOD_KING")},
            {ItemType.KNUCKLE_VARMOD_LOVE, NAPI.Util.GetHashKey("COMPONENT_KNUCKLE_VARMOD_LOVE")},
            {ItemType.KNUCKLE_VARMOD_PIMP, NAPI.Util.GetHashKey("COMPONENT_KNUCKLE_VARMOD_PIMP")},
            {ItemType.KNUCKLE_VARMOD_PLAYER, NAPI.Util.GetHashKey("COMPONENT_KNUCKLE_VARMOD_PLAYER")},
            {ItemType.KNUCKLE_VARMOD_VAGOS, NAPI.Util.GetHashKey("COMPONENT_KNUCKLE_VARMOD_VAGOS")},
            {ItemType.MACHINEPISTOL_CLIP_02, NAPI.Util.GetHashKey("COMPONENT_MACHINEPISTOL_CLIP_02")},
            {ItemType.MACHINEPISTOL_CLIP_03, NAPI.Util.GetHashKey("COMPONENT_MACHINEPISTOL_CLIP_03")},
            {ItemType.MARKSMANRIFLE_CLIP_02, NAPI.Util.GetHashKey("COMPONENT_MARKSMANRIFLE_CLIP_02")},
            {ItemType.MARKSMANRIFLE_MK2_CAMO, NAPI.Util.GetHashKey("COMPONENT_MARKSMANRIFLE_MK2_CAMO")},
            {ItemType.MARKSMANRIFLE_MK2_CAMO_02, NAPI.Util.GetHashKey("COMPONENT_MARKSMANRIFLE_MK2_CAMO_02")},
            {ItemType.MARKSMANRIFLE_MK2_CAMO_03, NAPI.Util.GetHashKey("COMPONENT_MARKSMANRIFLE_MK2_CAMO_03")},
            {ItemType.MARKSMANRIFLE_MK2_CAMO_04, NAPI.Util.GetHashKey("COMPONENT_MARKSMANRIFLE_MK2_CAMO_04")},
            {ItemType.MARKSMANRIFLE_MK2_CAMO_05, NAPI.Util.GetHashKey("COMPONENT_MARKSMANRIFLE_MK2_CAMO_05")},
            {ItemType.MARKSMANRIFLE_MK2_CAMO_06, NAPI.Util.GetHashKey("COMPONENT_MARKSMANRIFLE_MK2_CAMO_06")},
            {ItemType.MARKSMANRIFLE_MK2_CAMO_07, NAPI.Util.GetHashKey("COMPONENT_MARKSMANRIFLE_MK2_CAMO_07")},
            {ItemType.MARKSMANRIFLE_MK2_CAMO_08, NAPI.Util.GetHashKey("COMPONENT_MARKSMANRIFLE_MK2_CAMO_08")},
            {ItemType.MARKSMANRIFLE_MK2_CAMO_09, NAPI.Util.GetHashKey("COMPONENT_MARKSMANRIFLE_MK2_CAMO_09")},
            {ItemType.MARKSMANRIFLE_MK2_CAMO_10, NAPI.Util.GetHashKey("COMPONENT_MARKSMANRIFLE_MK2_CAMO_10")},
            {ItemType.MARKSMANRIFLE_MK2_CAMO_IND_01, NAPI.Util.GetHashKey("COMPONENT_MARKSMANRIFLE_MK2_CAMO_IND_01")},
            {ItemType.MARKSMANRIFLE_MK2_CLIP_02, NAPI.Util.GetHashKey("COMPONENT_MARKSMANRIFLE_MK2_CLIP_02")},
            {ItemType.MARKSMANRIFLE_MK2_CLIP_ARMORPIERCING, NAPI.Util.GetHashKey("COMPONENT_MARKSMANRIFLE_MK2_CLIP_ARMORPIERCING")},
            {ItemType.MARKSMANRIFLE_MK2_CLIP_FMJ, NAPI.Util.GetHashKey("COMPONENT_MARKSMANRIFLE_MK2_CLIP_FMJ")},
            {ItemType.MARKSMANRIFLE_MK2_CLIP_INCENDIARY, NAPI.Util.GetHashKey("COMPONENT_MARKSMANRIFLE_MK2_CLIP_INCENDIARY")},
            {ItemType.MARKSMANRIFLE_MK2_CLIP_TRACER, NAPI.Util.GetHashKey("COMPONENT_MARKSMANRIFLE_MK2_CLIP_TRACER")},
            {ItemType.MARKSMANRIFLE_VARMOD_LUXE, NAPI.Util.GetHashKey("COMPONENT_MARKSMANRIFLE_VARMOD_LUXE")},
            {ItemType.MG_CLIP_02, NAPI.Util.GetHashKey("COMPONENT_MG_CLIP_02")},
            {ItemType.MG_VARMOD_LOWRIDER, NAPI.Util.GetHashKey("COMPONENT_MG_VARMOD_LOWRIDER")},
            {ItemType.MICROSMG_CLIP_02, NAPI.Util.GetHashKey("COMPONENT_MICROSMG_CLIP_02")},
            {ItemType.MICROSMG_VARMOD_LUXE, NAPI.Util.GetHashKey("COMPONENT_MICROSMG_VARMOD_LUXE")},
            {ItemType.MILITARYRIFLE_CLIP_02, NAPI.Util.GetHashKey("COMPONENT_MILITARYRIFLE_CLIP_02")},
            {ItemType.MILITARYRIFLE_SIGHT_01, NAPI.Util.GetHashKey("COMPONENT_MILITARYRIFLE_SIGHT_01")},
            {ItemType.MINISMG_CLIP_02, NAPI.Util.GetHashKey("COMPONENT_MINISMG_CLIP_02")},
            {ItemType.PISTOL50_CLIP_02, NAPI.Util.GetHashKey("COMPONENT_PISTOL50_CLIP_02")},
            {ItemType.PISTOL50_VARMOD_LUXE, NAPI.Util.GetHashKey("COMPONENT_PISTOL50_VARMOD_LUXE")},
            {ItemType.PISTOL_CLIP_02, NAPI.Util.GetHashKey("COMPONENT_PISTOL_CLIP_02")},
            {ItemType.PISTOL_MK2_CAMO, NAPI.Util.GetHashKey("COMPONENT_PISTOL_MK2_CAMO")},
            {ItemType.PISTOL_MK2_CAMO_02, NAPI.Util.GetHashKey("COMPONENT_PISTOL_MK2_CAMO_02")},
            {ItemType.PISTOL_MK2_CAMO_02_SLIDE, NAPI.Util.GetHashKey("COMPONENT_PISTOL_MK2_CAMO_02_SLIDE")},
            {ItemType.PISTOL_MK2_CAMO_03, NAPI.Util.GetHashKey("COMPONENT_PISTOL_MK2_CAMO_03")},
            {ItemType.PISTOL_MK2_CAMO_03_SLIDE, NAPI.Util.GetHashKey("COMPONENT_PISTOL_MK2_CAMO_03_SLIDE")},
            {ItemType.PISTOL_MK2_CAMO_04, NAPI.Util.GetHashKey("COMPONENT_PISTOL_MK2_CAMO_04")},
            {ItemType.PISTOL_MK2_CAMO_04_SLIDE, NAPI.Util.GetHashKey("COMPONENT_PISTOL_MK2_CAMO_04_SLIDE")},
            {ItemType.PISTOL_MK2_CAMO_05, NAPI.Util.GetHashKey("COMPONENT_PISTOL_MK2_CAMO_05")},
            {ItemType.PISTOL_MK2_CAMO_05_SLIDE, NAPI.Util.GetHashKey("COMPONENT_PISTOL_MK2_CAMO_05_SLIDE")},
            {ItemType.PISTOL_MK2_CAMO_06, NAPI.Util.GetHashKey("COMPONENT_PISTOL_MK2_CAMO_06")},
            {ItemType.PISTOL_MK2_CAMO_06_SLIDE, NAPI.Util.GetHashKey("COMPONENT_PISTOL_MK2_CAMO_06_SLIDE")},
            {ItemType.PISTOL_MK2_CAMO_07, NAPI.Util.GetHashKey("COMPONENT_PISTOL_MK2_CAMO_07")},
            {ItemType.PISTOL_MK2_CAMO_07_SLIDE, NAPI.Util.GetHashKey("COMPONENT_PISTOL_MK2_CAMO_07_SLIDE")},
            {ItemType.PISTOL_MK2_CAMO_08, NAPI.Util.GetHashKey("COMPONENT_PISTOL_MK2_CAMO_08")},
            {ItemType.PISTOL_MK2_CAMO_08_SLIDE, NAPI.Util.GetHashKey("COMPONENT_PISTOL_MK2_CAMO_08_SLIDE")},
            {ItemType.PISTOL_MK2_CAMO_09, NAPI.Util.GetHashKey("COMPONENT_PISTOL_MK2_CAMO_09")},
            {ItemType.PISTOL_MK2_CAMO_09_SLIDE, NAPI.Util.GetHashKey("COMPONENT_PISTOL_MK2_CAMO_09_SLIDE")},
            {ItemType.PISTOL_MK2_CAMO_10, NAPI.Util.GetHashKey("COMPONENT_PISTOL_MK2_CAMO_10")},
            {ItemType.PISTOL_MK2_CAMO_10_SLIDE, NAPI.Util.GetHashKey("COMPONENT_PISTOL_MK2_CAMO_10_SLIDE")},
            {ItemType.PISTOL_MK2_CAMO_IND_01, NAPI.Util.GetHashKey("COMPONENT_PISTOL_MK2_CAMO_IND_01")},
            {ItemType.PISTOL_MK2_CAMO_IND_01_SLIDE, NAPI.Util.GetHashKey("COMPONENT_PISTOL_MK2_CAMO_IND_01_SLIDE")},
            {ItemType.PISTOL_MK2_CAMO_SLIDE, NAPI.Util.GetHashKey("COMPONENT_PISTOL_MK2_CAMO_SLIDE")},
            {ItemType.PISTOL_MK2_CLIP_02, NAPI.Util.GetHashKey("COMPONENT_PISTOL_MK2_CLIP_02")},
            {ItemType.PISTOL_MK2_CLIP_FMJ, NAPI.Util.GetHashKey("COMPONENT_PISTOL_MK2_CLIP_FMJ")},
            {ItemType.PISTOL_MK2_CLIP_HOLLOWPOINT, NAPI.Util.GetHashKey("COMPONENT_PISTOL_MK2_CLIP_HOLLOWPOINT")},
            {ItemType.PISTOL_MK2_CLIP_INCENDIARY, NAPI.Util.GetHashKey("COMPONENT_PISTOL_MK2_CLIP_INCENDIARY")},
            {ItemType.PISTOL_MK2_CLIP_TRACER, NAPI.Util.GetHashKey("COMPONENT_PISTOL_MK2_CLIP_TRACER")},
            {ItemType.PISTOL_VARMOD_LUXE, NAPI.Util.GetHashKey("COMPONENT_PISTOL_VARMOD_LUXE")},
            {ItemType.PUMPSHOTGUN_MK2_CAMO, NAPI.Util.GetHashKey("COMPONENT_PUMPSHOTGUN_MK2_CAMO")},
            {ItemType.PUMPSHOTGUN_MK2_CAMO_02, NAPI.Util.GetHashKey("COMPONENT_PUMPSHOTGUN_MK2_CAMO_02")},
            {ItemType.PUMPSHOTGUN_MK2_CAMO_03, NAPI.Util.GetHashKey("COMPONENT_PUMPSHOTGUN_MK2_CAMO_03")},
            {ItemType.PUMPSHOTGUN_MK2_CAMO_04, NAPI.Util.GetHashKey("COMPONENT_PUMPSHOTGUN_MK2_CAMO_04")},
            {ItemType.PUMPSHOTGUN_MK2_CAMO_05, NAPI.Util.GetHashKey("COMPONENT_PUMPSHOTGUN_MK2_CAMO_05")},
            {ItemType.PUMPSHOTGUN_MK2_CAMO_06, NAPI.Util.GetHashKey("COMPONENT_PUMPSHOTGUN_MK2_CAMO_06")},
            {ItemType.PUMPSHOTGUN_MK2_CAMO_07, NAPI.Util.GetHashKey("COMPONENT_PUMPSHOTGUN_MK2_CAMO_07")},
            {ItemType.PUMPSHOTGUN_MK2_CAMO_08, NAPI.Util.GetHashKey("COMPONENT_PUMPSHOTGUN_MK2_CAMO_08")},
            {ItemType.PUMPSHOTGUN_MK2_CAMO_09, NAPI.Util.GetHashKey("COMPONENT_PUMPSHOTGUN_MK2_CAMO_09")},
            {ItemType.PUMPSHOTGUN_MK2_CAMO_10, NAPI.Util.GetHashKey("COMPONENT_PUMPSHOTGUN_MK2_CAMO_10")},
            {ItemType.PUMPSHOTGUN_MK2_CAMO_IND_01, NAPI.Util.GetHashKey("COMPONENT_PUMPSHOTGUN_MK2_CAMO_IND_01")},
            {ItemType.PUMPSHOTGUN_MK2_CLIP_ARMORPIERCING, NAPI.Util.GetHashKey("COMPONENT_PUMPSHOTGUN_MK2_CLIP_ARMORPIERCING")},
            {ItemType.PUMPSHOTGUN_MK2_CLIP_EXPLOSIVE, NAPI.Util.GetHashKey("COMPONENT_PUMPSHOTGUN_MK2_CLIP_EXPLOSIVE")},
            {ItemType.PUMPSHOTGUN_MK2_CLIP_HOLLOWPOINT, NAPI.Util.GetHashKey("COMPONENT_PUMPSHOTGUN_MK2_CLIP_HOLLOWPOINT")},
            {ItemType.PUMPSHOTGUN_MK2_CLIP_INCENDIARY, NAPI.Util.GetHashKey("COMPONENT_PUMPSHOTGUN_MK2_CLIP_INCENDIARY")},
            {ItemType.PUMPSHOTGUN_VARMOD_LOWRIDER, NAPI.Util.GetHashKey("COMPONENT_PUMPSHOTGUN_VARMOD_LOWRIDER")},
            {ItemType.RAYPISTOL_VARMOD_XMAS18, NAPI.Util.GetHashKey("COMPONENT_RAYPISTOL_VARMOD_XMAS18")},
            {ItemType.REVOLVER_MK2_CAMO, NAPI.Util.GetHashKey("COMPONENT_REVOLVER_MK2_CAMO")},
            {ItemType.REVOLVER_MK2_CAMO_02, NAPI.Util.GetHashKey("COMPONENT_REVOLVER_MK2_CAMO_02")},
            {ItemType.REVOLVER_MK2_CAMO_03, NAPI.Util.GetHashKey("COMPONENT_REVOLVER_MK2_CAMO_03")},
            {ItemType.REVOLVER_MK2_CAMO_04, NAPI.Util.GetHashKey("COMPONENT_REVOLVER_MK2_CAMO_04")},
            {ItemType.REVOLVER_MK2_CAMO_05, NAPI.Util.GetHashKey("COMPONENT_REVOLVER_MK2_CAMO_05")},
            {ItemType.REVOLVER_MK2_CAMO_06, NAPI.Util.GetHashKey("COMPONENT_REVOLVER_MK2_CAMO_06")},
            {ItemType.REVOLVER_MK2_CAMO_07, NAPI.Util.GetHashKey("COMPONENT_REVOLVER_MK2_CAMO_07")},
            {ItemType.REVOLVER_MK2_CAMO_08, NAPI.Util.GetHashKey("COMPONENT_REVOLVER_MK2_CAMO_08")},
            {ItemType.REVOLVER_MK2_CAMO_09, NAPI.Util.GetHashKey("COMPONENT_REVOLVER_MK2_CAMO_09")},
            {ItemType.REVOLVER_MK2_CAMO_10, NAPI.Util.GetHashKey("COMPONENT_REVOLVER_MK2_CAMO_10")},
            {ItemType.REVOLVER_MK2_CAMO_IND_01, NAPI.Util.GetHashKey("COMPONENT_REVOLVER_MK2_CAMO_IND_01")},
            {ItemType.REVOLVER_MK2_CLIP_FMJ, NAPI.Util.GetHashKey("COMPONENT_REVOLVER_MK2_CLIP_FMJ")},
            {ItemType.REVOLVER_MK2_CLIP_HOLLOWPOINT, NAPI.Util.GetHashKey("COMPONENT_REVOLVER_MK2_CLIP_HOLLOWPOINT")},
            {ItemType.REVOLVER_MK2_CLIP_INCENDIARY, NAPI.Util.GetHashKey("COMPONENT_REVOLVER_MK2_CLIP_INCENDIARY")},
            {ItemType.REVOLVER_MK2_CLIP_TRACER, NAPI.Util.GetHashKey("COMPONENT_REVOLVER_MK2_CLIP_TRACER")},
            {ItemType.REVOLVER_VARMOD_BOSS, NAPI.Util.GetHashKey("COMPONENT_REVOLVER_VARMOD_BOSS")},
            {ItemType.REVOLVER_VARMOD_GOON, NAPI.Util.GetHashKey("COMPONENT_REVOLVER_VARMOD_GOON")},
            {ItemType.SAWNOFFSHOTGUN_VARMOD_LUXE, NAPI.Util.GetHashKey("COMPONENT_SAWNOFFSHOTGUN_VARMOD_LUXE")},
            {ItemType.SMG_CLIP_02, NAPI.Util.GetHashKey("COMPONENT_SMG_CLIP_02")},
            {ItemType.SMG_CLIP_03, NAPI.Util.GetHashKey("COMPONENT_SMG_CLIP_03")},
            {ItemType.SMG_MK2_CAMO, NAPI.Util.GetHashKey("COMPONENT_SMG_MK2_CAMO")},
            {ItemType.SMG_MK2_CAMO_02, NAPI.Util.GetHashKey("COMPONENT_SMG_MK2_CAMO_02")},
            {ItemType.SMG_MK2_CAMO_03, NAPI.Util.GetHashKey("COMPONENT_SMG_MK2_CAMO_03")},
            {ItemType.SMG_MK2_CAMO_04, NAPI.Util.GetHashKey("COMPONENT_SMG_MK2_CAMO_04")},
            {ItemType.SMG_MK2_CAMO_05, NAPI.Util.GetHashKey("COMPONENT_SMG_MK2_CAMO_05")},
            {ItemType.SMG_MK2_CAMO_06, NAPI.Util.GetHashKey("COMPONENT_SMG_MK2_CAMO_06")},
            {ItemType.SMG_MK2_CAMO_07, NAPI.Util.GetHashKey("COMPONENT_SMG_MK2_CAMO_07")},
            {ItemType.SMG_MK2_CAMO_08, NAPI.Util.GetHashKey("COMPONENT_SMG_MK2_CAMO_08")},
            {ItemType.SMG_MK2_CAMO_09, NAPI.Util.GetHashKey("COMPONENT_SMG_MK2_CAMO_09")},
            {ItemType.SMG_MK2_CAMO_10, NAPI.Util.GetHashKey("COMPONENT_SMG_MK2_CAMO_10")},
            {ItemType.SMG_MK2_CAMO_IND_01, NAPI.Util.GetHashKey("COMPONENT_SMG_MK2_CAMO_IND_01")},
            {ItemType.SMG_MK2_CLIP_02, NAPI.Util.GetHashKey("COMPONENT_SMG_MK2_CLIP_02")},
            {ItemType.SMG_MK2_CLIP_FMJ, NAPI.Util.GetHashKey("COMPONENT_SMG_MK2_CLIP_FMJ")},
            {ItemType.SMG_MK2_CLIP_HOLLOWPOINT, NAPI.Util.GetHashKey("COMPONENT_SMG_MK2_CLIP_HOLLOWPOINT")},
            {ItemType.SMG_MK2_CLIP_INCENDIARY, NAPI.Util.GetHashKey("COMPONENT_SMG_MK2_CLIP_INCENDIARY")},
            {ItemType.SMG_MK2_CLIP_TRACER, NAPI.Util.GetHashKey("COMPONENT_SMG_MK2_CLIP_TRACER")},
            {ItemType.SMG_VARMOD_LUXE, NAPI.Util.GetHashKey("COMPONENT_SMG_VARMOD_LUXE")},
            {ItemType.SNIPERRIFLE_VARMOD_LUXE, NAPI.Util.GetHashKey("COMPONENT_SNIPERRIFLE_VARMOD_LUXE")},
            {ItemType.SNSPISTOL_CLIP_02, NAPI.Util.GetHashKey("COMPONENT_SNSPISTOL_CLIP_02")},
            {ItemType.SNSPISTOL_MK2_CAMO, NAPI.Util.GetHashKey("COMPONENT_SNSPISTOL_MK2_CAMO")},
            {ItemType.SNSPISTOL_MK2_CAMO_02, NAPI.Util.GetHashKey("COMPONENT_SNSPISTOL_MK2_CAMO_02")},
            {ItemType.SNSPISTOL_MK2_CAMO_02_SLIDE, NAPI.Util.GetHashKey("COMPONENT_SNSPISTOL_MK2_CAMO_02_SLIDE")},
            {ItemType.SNSPISTOL_MK2_CAMO_03, NAPI.Util.GetHashKey("COMPONENT_SNSPISTOL_MK2_CAMO_03")},
            {ItemType.SNSPISTOL_MK2_CAMO_03_SLIDE, NAPI.Util.GetHashKey("COMPONENT_SNSPISTOL_MK2_CAMO_03_SLIDE")},
            {ItemType.SNSPISTOL_MK2_CAMO_04, NAPI.Util.GetHashKey("COMPONENT_SNSPISTOL_MK2_CAMO_04")},
            {ItemType.SNSPISTOL_MK2_CAMO_04_SLIDE, NAPI.Util.GetHashKey("COMPONENT_SNSPISTOL_MK2_CAMO_04_SLIDE")},
            {ItemType.SNSPISTOL_MK2_CAMO_05, NAPI.Util.GetHashKey("COMPONENT_SNSPISTOL_MK2_CAMO_05")},
            {ItemType.SNSPISTOL_MK2_CAMO_05_SLIDE, NAPI.Util.GetHashKey("COMPONENT_SNSPISTOL_MK2_CAMO_05_SLIDE")},
            {ItemType.SNSPISTOL_MK2_CAMO_06, NAPI.Util.GetHashKey("COMPONENT_SNSPISTOL_MK2_CAMO_06")},
            {ItemType.SNSPISTOL_MK2_CAMO_06_SLIDE, NAPI.Util.GetHashKey("COMPONENT_SNSPISTOL_MK2_CAMO_06_SLIDE")},
            {ItemType.SNSPISTOL_MK2_CAMO_07, NAPI.Util.GetHashKey("COMPONENT_SNSPISTOL_MK2_CAMO_07")},
            {ItemType.SNSPISTOL_MK2_CAMO_07_SLIDE, NAPI.Util.GetHashKey("COMPONENT_SNSPISTOL_MK2_CAMO_07_SLIDE")},
            {ItemType.SNSPISTOL_MK2_CAMO_08, NAPI.Util.GetHashKey("COMPONENT_SNSPISTOL_MK2_CAMO_08")},
            {ItemType.SNSPISTOL_MK2_CAMO_08_SLIDE, NAPI.Util.GetHashKey("COMPONENT_SNSPISTOL_MK2_CAMO_08_SLIDE")},
            {ItemType.SNSPISTOL_MK2_CAMO_09, NAPI.Util.GetHashKey("COMPONENT_SNSPISTOL_MK2_CAMO_09")},
            {ItemType.SNSPISTOL_MK2_CAMO_09_SLIDE, NAPI.Util.GetHashKey("COMPONENT_SNSPISTOL_MK2_CAMO_09_SLIDE")},
            {ItemType.SNSPISTOL_MK2_CAMO_10, NAPI.Util.GetHashKey("COMPONENT_SNSPISTOL_MK2_CAMO_10")},
            {ItemType.SNSPISTOL_MK2_CAMO_10_SLIDE, NAPI.Util.GetHashKey("COMPONENT_SNSPISTOL_MK2_CAMO_10_SLIDE")},
            {ItemType.SNSPISTOL_MK2_CAMO_IND_01, NAPI.Util.GetHashKey("COMPONENT_SNSPISTOL_MK2_CAMO_IND_01")},
            {ItemType.SNSPISTOL_MK2_CAMO_IND_01_SLIDE, NAPI.Util.GetHashKey("COMPONENT_SNSPISTOL_MK2_CAMO_IND_01_SLIDE")},
            {ItemType.SNSPISTOL_MK2_CAMO_SLIDE, NAPI.Util.GetHashKey("COMPONENT_SNSPISTOL_MK2_CAMO_SLIDE")},
            {ItemType.SNSPISTOL_MK2_CLIP_02, NAPI.Util.GetHashKey("COMPONENT_SNSPISTOL_MK2_CLIP_02")},
            {ItemType.SNSPISTOL_MK2_CLIP_FMJ, NAPI.Util.GetHashKey("COMPONENT_SNSPISTOL_MK2_CLIP_FMJ")},
            {ItemType.SNSPISTOL_MK2_CLIP_HOLLOWPOINT, NAPI.Util.GetHashKey("COMPONENT_SNSPISTOL_MK2_CLIP_HOLLOWPOINT")},
            {ItemType.SNSPISTOL_MK2_CLIP_INCENDIARY, NAPI.Util.GetHashKey("COMPONENT_SNSPISTOL_MK2_CLIP_INCENDIARY")},
            {ItemType.SNSPISTOL_MK2_CLIP_TRACER, NAPI.Util.GetHashKey("COMPONENT_SNSPISTOL_MK2_CLIP_TRACER")},
            {ItemType.SNSPISTOL_VARMOD_LOWRIDER, NAPI.Util.GetHashKey("COMPONENT_SNSPISTOL_VARMOD_LOWRIDER")},
            {ItemType.SPECIALCARBINE_CLIP_02, NAPI.Util.GetHashKey("COMPONENT_SPECIALCARBINE_CLIP_02")},
            {ItemType.SPECIALCARBINE_CLIP_03, NAPI.Util.GetHashKey("COMPONENT_SPECIALCARBINE_CLIP_03")},
            {ItemType.SPECIALCARBINE_MK2_CAMO, NAPI.Util.GetHashKey("COMPONENT_SPECIALCARBINE_MK2_CAMO")},
            {ItemType.SPECIALCARBINE_MK2_CAMO_02, NAPI.Util.GetHashKey("COMPONENT_SPECIALCARBINE_MK2_CAMO_02")},
            {ItemType.SPECIALCARBINE_MK2_CAMO_03, NAPI.Util.GetHashKey("COMPONENT_SPECIALCARBINE_MK2_CAMO_03")},
            {ItemType.SPECIALCARBINE_MK2_CAMO_04, NAPI.Util.GetHashKey("COMPONENT_SPECIALCARBINE_MK2_CAMO_04")},
            {ItemType.SPECIALCARBINE_MK2_CAMO_05, NAPI.Util.GetHashKey("COMPONENT_SPECIALCARBINE_MK2_CAMO_05")},
            {ItemType.SPECIALCARBINE_MK2_CAMO_06, NAPI.Util.GetHashKey("COMPONENT_SPECIALCARBINE_MK2_CAMO_06")},
            {ItemType.SPECIALCARBINE_MK2_CAMO_07, NAPI.Util.GetHashKey("COMPONENT_SPECIALCARBINE_MK2_CAMO_07")},
            {ItemType.SPECIALCARBINE_MK2_CAMO_08, NAPI.Util.GetHashKey("COMPONENT_SPECIALCARBINE_MK2_CAMO_08")},
            {ItemType.SPECIALCARBINE_MK2_CAMO_09, NAPI.Util.GetHashKey("COMPONENT_SPECIALCARBINE_MK2_CAMO_09")},
            {ItemType.SPECIALCARBINE_MK2_CAMO_10, NAPI.Util.GetHashKey("COMPONENT_SPECIALCARBINE_MK2_CAMO_10")},
            {ItemType.SPECIALCARBINE_MK2_CAMO_IND_01, NAPI.Util.GetHashKey("COMPONENT_SPECIALCARBINE_MK2_CAMO_IND_01")},
            {ItemType.SPECIALCARBINE_MK2_CLIP_02, NAPI.Util.GetHashKey("COMPONENT_SPECIALCARBINE_MK2_CLIP_02")},
            {ItemType.SPECIALCARBINE_MK2_CLIP_ARMORPIERCING, NAPI.Util.GetHashKey("COMPONENT_SPECIALCARBINE_MK2_CLIP_ARMORPIERCING")},
            {ItemType.SPECIALCARBINE_MK2_CLIP_FMJ, NAPI.Util.GetHashKey("COMPONENT_SPECIALCARBINE_MK2_CLIP_FMJ")},
            {ItemType.SPECIALCARBINE_MK2_CLIP_INCENDIARY, NAPI.Util.GetHashKey("COMPONENT_SPECIALCARBINE_MK2_CLIP_INCENDIARY")},
            {ItemType.SPECIALCARBINE_MK2_CLIP_TRACER, NAPI.Util.GetHashKey("COMPONENT_SPECIALCARBINE_MK2_CLIP_TRACER")},
            {ItemType.SPECIALCARBINE_VARMOD_LOWRIDER, NAPI.Util.GetHashKey("COMPONENT_SPECIALCARBINE_VARMOD_LOWRIDER")},
            {ItemType.SWITCHBLADE_VARMOD_VAR1, NAPI.Util.GetHashKey("COMPONENT_SWITCHBLADE_VARMOD_VAR1")},
            {ItemType.SWITCHBLADE_VARMOD_VAR2, NAPI.Util.GetHashKey("COMPONENT_SWITCHBLADE_VARMOD_VAR2")},
            {ItemType.VINTAGEPISTOL_CLIP_02, NAPI.Util.GetHashKey("COMPONENT_VINTAGEPISTOL_CLIP_02")},

            {ItemType.DigScanner , NAPI.Util.GetHashKey("w_am_digiscanner")},
            {ItemType.DigScanner_mk2 , NAPI.Util.GetHashKey("w_am_digiscanner")},
            {ItemType.DigScanner_mk3 , NAPI.Util.GetHashKey("w_am_digiscanner")},

            {ItemType.DigShovel , NAPI.Util.GetHashKey("prop_tool_shovel006")},
            {ItemType.DigShovel_mk2 , NAPI.Util.GetHashKey("prop_tool_shovel006")},
            {ItemType.DigShovel_mk3 , NAPI.Util.GetHashKey("prop_tool_shovel006")},

            {ItemType.CraftOldCoin, 000000000},
            {ItemType.CraftCap, 000000000},
            {ItemType.CraftShell, 000000000},
            {ItemType.CraftScrapMetal, 000000000},
            {ItemType.CraftCopperNugget,  000000000},
            {ItemType.CraftIronNugget,  000000000},
            {ItemType.CraftTinNugget,  000000000},

            {ItemType.CraftCopperWire, 000000000 },
            {ItemType.CraftOldJewerly, 000000000 },
            {ItemType.CraftGoldNugget, 000000000 },
            {ItemType.CraftСollectibleCoin, 000000000 },
            {ItemType.CraftAncientStatuette, 000000000 },
            {ItemType.CraftGoldHorseShoe, 000000000 },
            {ItemType.CraftRelic, 000000000 },

            {ItemType.CraftIronPart, 000000000 },
            {ItemType.CraftCopperPart, 000000000 },
            {ItemType.CraftTinPart, 000000000 },
            {ItemType.CraftBronzePart, 000000000 },

            {ItemType.CraftWorkBench, 000000000 },
            {ItemType.CraftPercolator, 000000000 },
            {ItemType.CraftSmelter, 000000000 },
            {ItemType.CraftPartsCollector, 000000000 },
            {ItemType.CraftWorkBenchUpgrade, 000000000 },
            {ItemType.CraftWorkBenchUpgrade2, 000000000 },
        };

        public static Dictionary<ItemType, Vector3> ItemsPosOffset = new Dictionary<ItemType, Vector3>()
        {
            { ItemType.Hat, new Vector3(0, 0, -0.93) },
            { ItemType.Mask, new Vector3(0, 0, -1) },
            { ItemType.Gloves, new Vector3(0, 0, -1) },
            { ItemType.Leg, new Vector3(0, 0, -0.85) },
            { ItemType.Bag, new Vector3(0, 0, -0.99) },
            { ItemType.Feet, new Vector3(0, 0, -0.95) },
            { ItemType.Jewelry, new Vector3(0, 0, -0.98) },
            { ItemType.Undershit, new Vector3(0, 0, -0.98) },
            { ItemType.BodyArmor, new Vector3(0, 0, -0.88) },
            { ItemType.Unknown, new Vector3() },
            { ItemType.Top, new Vector3(0, 0, -0.96) },
            { ItemType.Glasses, new Vector3(0, 0, -0.98) },
            { ItemType.Accessories, new Vector3(0, 0, -0.98) },
            { ItemType.CasinoChips, new Vector3(0, 0, -0.6) },
             { ItemType.Grenade, new Vector3(0, 0, -0.6) },
            { ItemType.Drugs, new Vector3(0, 0, -0.95) },
            { ItemType.Material, new Vector3(0, 0, -0.6) },
            { ItemType.Debug, new Vector3() },
            { ItemType.HealthKit, new Vector3(0, 0, -0.9) },
            { ItemType.GasCan, new Vector3(0, 0, -1) },
            { ItemType.Сrisps, new Vector3(0, 0, -1) },
            { ItemType.Beer, new Vector3(0, 0, -1) },
            { ItemType.Pizza, new Vector3(0, 0, -1) },
            { ItemType.Burger, new Vector3(0, 0, -0.97) },
            { ItemType.HotDog, new Vector3(0, 0, -0.97) },
            { ItemType.Sandwich, new Vector3(0, 0, -0.99) },
            { ItemType.eCola, new Vector3(0, 0, -1) },
            { ItemType.Sprunk, new Vector3(0, 0, -1) },
            { ItemType.Lockpick, new Vector3(0, 0, -0.98) },
            { ItemType.ArmyLockpick, new Vector3(0, 0, -0.98) },
            { ItemType.Pocket, new Vector3(0, 0, -0.98) },
            { ItemType.Cuffs, new Vector3(0, 0, -0.98) },
            { ItemType.CarKey, new Vector3(0, 0, -0.98) },
            { ItemType.Present, new Vector3(0, 0, -0.98) },
            { ItemType.KeyRing, new Vector3(0, 0, -0.98) },

            { ItemType.RusDrink1, new Vector3(0, 0, -1) },
            { ItemType.RusDrink2, new Vector3(0, 0, -1) },
            { ItemType.RusDrink3, new Vector3(0, 0, -1) },
            { ItemType.YakDrink1, new Vector3(0, 0, -0.87) },
            { ItemType.YakDrink2, new Vector3(0, 0, -1) },
            { ItemType.YakDrink3, new Vector3(0, 0, -0.87) },
            { ItemType.LcnDrink1, new Vector3(0, 0, -1) },
            { ItemType.LcnDrink2, new Vector3(0, 0, -1) },
            { ItemType.LcnDrink3, new Vector3(0, 0, -1) },
            { ItemType.ArmDrink1, new Vector3(0, 0, -1) },
            { ItemType.ArmDrink2, new Vector3(0, 0, -1) },
            { ItemType.ArmDrink3, new Vector3(0, 0, -1) },
            { ItemType.WaterBottle,  new Vector3(0, 0, -1)},
            { ItemType.RepairBox, new Vector3(0, 0, -1) },
            { ItemType.SmallHealthKit, new Vector3(0, 0, -1) },

             { ItemType.Pistol, new Vector3(0, 0, -0.99) },
            { ItemType.Combatpistol, new Vector3(0, 0, -0.99) },
            { ItemType.Pistol50, new Vector3(0, 0, -0.99) },
            { ItemType.Snspistol, new Vector3(0, 0, -0.99) },
            { ItemType.Heavypistol, new Vector3(0, 0, -0.99) },
            { ItemType.Vintagepistol, new Vector3(0, 0, -0.99) },
            { ItemType.Marksmanpistol, new Vector3(0, 0, -0.99) },
            { ItemType.Revolver, new Vector3(0, 0, -0.99) },
            { ItemType.Appistol, new Vector3(0, 0, -0.99) },
            { ItemType.Stungun, new Vector3(0, 0, -0.99) },
            { ItemType.Flaregun, new Vector3(0, 0, -0.99) },
            { ItemType.Doubleaction, new Vector3(0, 0, -0.99) },
            { ItemType.Pistol_mk2, new Vector3(0, 0, -0.99) },
            { ItemType.Snspistol_mk2, new Vector3(0, 0, -0.99) },
            { ItemType.Revolver_mk2, new Vector3(0, 0, -0.99) },
            { ItemType.Microsmg, new Vector3(0, 0, -0.99) },
            { ItemType.Machinepistol, new Vector3(0, 0, -0.99) },
            { ItemType.Smg, new Vector3(0, 0, -0.99) },
            { ItemType.Assaultsmg, new Vector3(0, 0, -0.99) },
            { ItemType.Combatpdw, new Vector3(0, 0, -0.99) },
            { ItemType.Mg, new Vector3(0, 0, -0.99) },
            { ItemType.Combatmg, new Vector3(0, 0, -0.99) },
            { ItemType.Gusenberg, new Vector3(0, 0, -0.99) },
            { ItemType.Minismg, new Vector3(0, 0, -0.99) },
            { ItemType.Smg_mk2, new Vector3(0, 0, -0.99) },
            { ItemType.Combatmg_mk2, new Vector3(0, 0, -0.99) },
            { ItemType.Assaultrifle, new Vector3(0, 0, -0.99) },
            { ItemType.Carbinerifle, new Vector3(0, 0, -0.99) },
            { ItemType.Advancedrifle, new Vector3(0, 0, -0.99) },
            { ItemType.Specialcarbine, new Vector3(0, 0, -0.99) },
            { ItemType.Bullpuprifle, new Vector3(0, 0, -0.99) },
            { ItemType.Compactrifle, new Vector3(0, 0, -0.99) },
            { ItemType.Assaultrifle_mk2, new Vector3(0, 0, -0.99) },
            { ItemType.Carbinerifle_mk2, new Vector3(0, 0, -0.99) },
            { ItemType.Specialcarbine_mk2, new Vector3(0, 0, -0.99) },
            { ItemType.Bullpuprifle_mk2, new Vector3(0, 0, -0.99) },
            { ItemType.Sniperrifle, new Vector3(0, 0, -0.99) },
            { ItemType.Heavysniper, new Vector3(0, 0, -0.99) },
            { ItemType.Marksmanrifle, new Vector3(0, 0, -0.99) },
            { ItemType.Heavysniper_mk2, new Vector3(0, 0, -0.99) },
            { ItemType.Marksmanrifle_mk2, new Vector3(0, 0, -0.99) },
            { ItemType.Pumpshotgun, new Vector3(0, 0, -0.99) },
            { ItemType.Sawnoffshotgun, new Vector3(0, 0, -0.99) },
            { ItemType.Bullpupshotgun, new Vector3(0, 0, -0.99) },
            { ItemType.Assaultshotgun, new Vector3(0, 0, -0.99) },
            { ItemType.Musket, new Vector3(0, 0, -0.99) },
            { ItemType.Heavyshotgun, new Vector3(0, 0, -0.99) },
            { ItemType.Dbshotgun, new Vector3(0, 0, -0.99) },
            { ItemType.Autoshotgun, new Vector3(0, 0, -0.99) },
            { ItemType.Pumpshotgun_mk2, new Vector3(0, 0, -0.99) },
            { ItemType.Rpg, new Vector3(0, 0, -0.99) },
            { ItemType.Knife, new Vector3(0, 0, -0.99) },
            { ItemType.Nightstick, new Vector3(0, 0, -0.99) },
            { ItemType.Hammer, new Vector3(0, 0, -0.99) },
            { ItemType.Bat, new Vector3(0, 0, -0.99) },
            { ItemType.Crowbar, new Vector3(0, 0, -0.99) },
            { ItemType.Golfclub, new Vector3(0, 0, -0.99) },
            { ItemType.Bottle, new Vector3(0, 0, -0.99) },
            { ItemType.Dagger, new Vector3(0, 0, -0.99) },
            { ItemType.Hatchet, new Vector3(0, 0, -0.99) },
            { ItemType.Knuckle, new Vector3(0, 0, -0.99) },
            { ItemType.Machete, new Vector3(0, 0, -0.99) },
            { ItemType.Flashlight, new Vector3(0, 0, -0.99) },
            { ItemType.Switchblade, new Vector3(0, 0, -0.99) },
            { ItemType.Poolcue, new Vector3(0, 0, -0.99) },
            { ItemType.Wrench, new Vector3(0, 0, -0.985) },
            { ItemType.Battleaxe, new Vector3(0, 0, -0.99) },
            { ItemType.Ceramicpistol, new Vector3(0, 0, -0.99) },
            { ItemType.Combatshotgun, new Vector3(0, 0, -0.99) },
            { ItemType.Militaryrifle, new Vector3(0, 0, -0.99) },

            { ItemType.PistolAmmo, new Vector3(0, 0, -1) },
            { ItemType.RiflesAmmo, new Vector3(0, 0, -1) },
            { ItemType.ShotgunsAmmo, new Vector3(0, 0, -1) },
            { ItemType.SMGAmmo, new Vector3(0, 0, -1) },
            { ItemType.SniperAmmo, new Vector3(0, 0, -1) },

            /* Fishing */
            { ItemType.Rod, new Vector3(0.13, 0, 0.02) },
            { ItemType.RodUpgrade, new Vector3(0.13, 0, 0.02) },
            { ItemType.RodMK2, new Vector3(0.13, 0, 0.02) },
            { ItemType.Bait, new Vector3(0, 0, -0.99) },
            { ItemType.Naz, new Vector3(0, 0, -0.99) },
            { ItemType.Kyndja, new Vector3(0, 0, -0.99) },
            { ItemType.Sig, new Vector3(0, 0, -0.99) },
            { ItemType.Omyl, new Vector3(0, 0, -0.99) },
            { ItemType.Nerka, new Vector3(0, 0, -0.99) },
            { ItemType.Forel, new Vector3(0, 0, -0.99) },
            { ItemType.Ship, new Vector3(0, 0, -0.99) },
            { ItemType.Lopatonos, new Vector3(0, 0, -0.99) },
            { ItemType.Osetr, new Vector3(0, 0, -0.99) },
            { ItemType.Semga, new Vector3(0, 0, -0.99) },
            { ItemType.Servyga, new Vector3(0, 0, -0.99) },
            { ItemType.Beluga, new Vector3(0, 0, -0.99) },
            { ItemType.Taimen, new Vector3(0, 0, -0.99) },
            { ItemType.Sterlyad, new Vector3(0, 0, -0.99) },
            { ItemType.Ydilshik, new Vector3(0, 0, -0.99) },
            { ItemType.Hailiod, new Vector3(0, 0, -0.99) },
            { ItemType.Keta, new Vector3(0, 0, -0.99) },
            { ItemType.Gorbysha, new Vector3(0, 0, -0.99) },

            //Farmer Job Items
            { ItemType.Hay, new Vector3(0, 0, -0.99) },
            { ItemType.Seed, new Vector3(0, 0, -0.99) },

            { ItemType.Payek, new Vector3(0, 0, -1) },

            { ItemType.LSPDDrone, new Vector3(0, 0, -0.99) },
            { ItemType.Drone, new Vector3(0, 0, -0.99) },
            { ItemType.NumberPlate, new Vector3(0, 0, -0.99) },

            { ItemType.SimCard, new Vector3(0, 0, -0.99) },
            { ItemType.ProductBox, new Vector3(0, 0, -0.99) },
            { ItemType.TrashBag, new Vector3(0, 0, -0.99) },
            { ItemType.CocaLeaves, new Vector3(0, 0, -0.99) },
            { ItemType.Cocaine, new Vector3(0, 0, -0.99) },
            { ItemType.DrugBookMark, new Vector3(0, 0, -0.99) },
            { ItemType.Subject, new Vector3(0, 0, -0.99) },


            // [Modification]
            {ItemType.ADVANCEDRIFLE_CLIP_02, new Vector3(0, 0, -0.95)},
            {ItemType.ADVANCEDRIFLE_VARMOD_LUXE, new Vector3(0, 0, -0.95)},
            {ItemType.APPISTOL_CLIP_02, new Vector3(0, 0, -0.95)},
            {ItemType.APPISTOL_VARMOD_LUXE, new Vector3(0, 0, -0.95)},
            {ItemType.ASSAULTRIFLE_CLIP_02, new Vector3(0, 0, -0.95)},
            {ItemType.ASSAULTRIFLE_CLIP_03, new Vector3(0, 0, -0.95)},
            {ItemType.ASSAULTRIFLE_MK2_CAMO, new Vector3(0, 0, -0.95)},
            {ItemType.ASSAULTRIFLE_MK2_CAMO_02, new Vector3(0, 0, -0.95)},
            {ItemType.ASSAULTRIFLE_MK2_CAMO_03, new Vector3(0, 0, -0.95)},
            {ItemType.ASSAULTRIFLE_MK2_CAMO_04, new Vector3(0, 0, -0.95)},
            {ItemType.ASSAULTRIFLE_MK2_CAMO_05, new Vector3(0, 0, -0.95)},
            {ItemType.ASSAULTRIFLE_MK2_CAMO_06, new Vector3(0, 0, -0.95)},
            {ItemType.ASSAULTRIFLE_MK2_CAMO_07, new Vector3(0, 0, -0.95)},
            {ItemType.ASSAULTRIFLE_MK2_CAMO_08, new Vector3(0, 0, -0.95)},
            {ItemType.ASSAULTRIFLE_MK2_CAMO_09, new Vector3(0, 0, -0.95)},
            {ItemType.ASSAULTRIFLE_MK2_CAMO_10, new Vector3(0, 0, -0.95)},
            {ItemType.ASSAULTRIFLE_MK2_CAMO_IND_01, new Vector3(0, 0, -0.95)},
            {ItemType.ASSAULTRIFLE_MK2_CLIP_02, new Vector3(0, 0, -0.95)},
            {ItemType.ASSAULTRIFLE_MK2_CLIP_ARMORPIERCING, new Vector3(0, 0, -0.95)},
            {ItemType.ASSAULTRIFLE_MK2_CLIP_FMJ, new Vector3(0, 0, -0.95)},
            {ItemType.ASSAULTRIFLE_MK2_CLIP_INCENDIARY, new Vector3(0, 0, -0.95)},
            {ItemType.ASSAULTRIFLE_MK2_CLIP_TRACER, new Vector3(0, 0, -0.95)},
            {ItemType.ASSAULTRIFLE_VARMOD_LUXE, new Vector3(0, 0, -0.95)},
            {ItemType.ASSAULTSHOTGUN_CLIP_02, new Vector3(0, 0, -0.95)},
            {ItemType.ASSAULTSMG_CLIP_02, new Vector3(0, 0, -0.95)},
            {ItemType.ASSAULTSMG_VARMOD_LOWRIDER, new Vector3(0, 0, -0.95)},
            {ItemType.AT_AR_AFGRIP, new Vector3(0, 0, -0.95)},
            {ItemType.AT_AR_AFGRIP_02, new Vector3(0, 0, -0.95)},
            {ItemType.AT_AR_BARREL_02, new Vector3(0, 0, -0.95)},
            {ItemType.AT_AR_FLSH, new Vector3(0, 0, -0.95)},
            {ItemType.AT_AR_SUPP, new Vector3(0, 0, -0.95)},
            {ItemType.AT_AR_SUPP_02, new Vector3(0, 0, -0.95)},
            {ItemType.AT_BP_BARREL_02, new Vector3(0, 0, -0.95)},
            {ItemType.AT_CR_BARREL_02, new Vector3(0, 0, -0.95)},
            {ItemType.AT_MG_BARREL_02, new Vector3(0, 0, -0.95)},
            {ItemType.AT_MRFL_BARREL_02, new Vector3(0, 0, -0.95)},
            {ItemType.AT_MUZZLE_01, new Vector3(0, 0, -0.95)},
            {ItemType.AT_MUZZLE_02, new Vector3(0, 0, -0.95)},
            {ItemType.AT_MUZZLE_03, new Vector3(0, 0, -0.95)},
            {ItemType.AT_MUZZLE_04, new Vector3(0, 0, -0.95)},
            {ItemType.AT_MUZZLE_05, new Vector3(0, 0, -0.95)},
            {ItemType.AT_MUZZLE_06, new Vector3(0, 0, -0.95)},
            {ItemType.AT_MUZZLE_07, new Vector3(0, 0, -0.95)},
            {ItemType.AT_MUZZLE_08, new Vector3(0, 0, -0.95)},
            {ItemType.AT_MUZZLE_09, new Vector3(0, 0, -0.95)},
            {ItemType.AT_PI_COMP, new Vector3(0, 0, -0.95)},
            {ItemType.AT_PI_COMP_02, new Vector3(0, 0, -0.95)},
            {ItemType.AT_PI_COMP_03, new Vector3(0, 0, -0.95)},
            {ItemType.AT_PI_FLSH, new Vector3(0, 0, -0.95)},
            {ItemType.AT_PI_FLSH_02, new Vector3(0, 0, -0.95)},
            {ItemType.AT_PI_FLSH_03, new Vector3(0, 0, -0.95)},
            {ItemType.AT_PI_RAIL, new Vector3(0, 0, -0.95)},
            {ItemType.AT_PI_RAIL_02, new Vector3(0, 0, -0.95)},
            {ItemType.AT_PI_SUPP, new Vector3(0, 0, -0.95)},
            {ItemType.AT_PI_SUPP_02, new Vector3(0, 0, -0.95)},
            {ItemType.AT_SB_BARREL_02, new Vector3(0, 0, -0.95)},
            {ItemType.AT_SC_BARREL_02, new Vector3(0, 0, -0.95)},
            {ItemType.AT_SCOPE_LARGE, new Vector3(0, 0, -0.95)},
            {ItemType.AT_SCOPE_LARGE_FIXED_ZOOM, new Vector3(0, 0, -0.95)},
            {ItemType.AT_SCOPE_LARGE_FIXED_ZOOM_MK2, new Vector3(0, 0, -0.95)},
            {ItemType.AT_SCOPE_LARGE_MK2, new Vector3(0, 0, -0.95)},
            {ItemType.AT_SCOPE_MACRO, new Vector3(0, 0, -0.95)},
            {ItemType.AT_SCOPE_MACRO_02, new Vector3(0, 0, -0.95)},
            {ItemType.AT_SCOPE_MACRO_02_MK2, new Vector3(0, 0, -0.95)},
            {ItemType.AT_SCOPE_MACRO_02_SMG_MK2, new Vector3(0, 0, -0.95)},
            {ItemType.AT_SCOPE_MACRO_MK2, new Vector3(0, 0, -0.95)},
            {ItemType.AT_SCOPE_MAX, new Vector3(0, 0, -0.95)},
            {ItemType.AT_SCOPE_MEDIUM, new Vector3(0, 0, -0.95)},
            {ItemType.AT_SCOPE_MEDIUM_MK2, new Vector3(0, 0, -0.95)},
            {ItemType.AT_SCOPE_NV, new Vector3(0, 0, -0.95)},
            {ItemType.AT_SCOPE_SMALL, new Vector3(0, 0, -0.95)},
            {ItemType.AT_SCOPE_SMALL_02, new Vector3(0, 0, -0.95)},
            {ItemType.AT_SCOPE_SMALL_MK2, new Vector3(0, 0, -0.95)},
            {ItemType.AT_SCOPE_SMALL_SMG_MK2, new Vector3(0, 0, -0.95)},
            {ItemType.AT_SCOPE_THERMAL, new Vector3(0, 0, -0.95)},
            {ItemType.AT_SIGHTS, new Vector3(0, 0, -0.95)},
            {ItemType.AT_SIGHTS_SMG, new Vector3(0, 0, -0.95)},
            {ItemType.AT_SR_BARREL_02, new Vector3(0, 0, -0.95)},
            {ItemType.AT_SR_SUPP, new Vector3(0, 0, -0.95)},
            {ItemType.AT_SR_SUPP_03, new Vector3(0, 0, -0.95)},
            {ItemType.BULLPUPRIFLE_CLIP_02, new Vector3(0, 0, -0.95)},
            {ItemType.BULLPUPRIFLE_MK2_CAMO, new Vector3(0, 0, -0.95)},
            {ItemType.BULLPUPRIFLE_MK2_CAMO_02, new Vector3(0, 0, -0.95)},
            {ItemType.BULLPUPRIFLE_MK2_CAMO_03, new Vector3(0, 0, -0.95)},
            {ItemType.BULLPUPRIFLE_MK2_CAMO_04, new Vector3(0, 0, -0.95)},
            {ItemType.BULLPUPRIFLE_MK2_CAMO_05, new Vector3(0, 0, -0.95)},
            {ItemType.BULLPUPRIFLE_MK2_CAMO_06, new Vector3(0, 0, -0.95)},
            {ItemType.BULLPUPRIFLE_MK2_CAMO_07, new Vector3(0, 0, -0.95)},
            {ItemType.BULLPUPRIFLE_MK2_CAMO_08, new Vector3(0, 0, -0.95)},
            {ItemType.BULLPUPRIFLE_MK2_CAMO_09, new Vector3(0, 0, -0.95)},
            {ItemType.BULLPUPRIFLE_MK2_CAMO_10, new Vector3(0, 0, -0.95)},
            {ItemType.BULLPUPRIFLE_MK2_CAMO_IND_01, new Vector3(0, 0, -0.95)},
            {ItemType.BULLPUPRIFLE_MK2_CLIP_02, new Vector3(0, 0, -0.95)},
            {ItemType.BULLPUPRIFLE_MK2_CLIP_ARMORPIERCING, new Vector3(0, 0, -0.95)},
            {ItemType.BULLPUPRIFLE_MK2_CLIP_FMJ, new Vector3(0, 0, -0.95)},
            {ItemType.BULLPUPRIFLE_MK2_CLIP_INCENDIARY, new Vector3(0, 0, -0.95)},
            {ItemType.BULLPUPRIFLE_MK2_CLIP_TRACER, new Vector3(0, 0, -0.95)},
            {ItemType.BULLPUPRIFLE_VARMOD_LOW, new Vector3(0, 0, -0.95)},
            {ItemType.CARBINERIFLE_CLIP_02, new Vector3(0, 0, -0.95)},
            {ItemType.CARBINERIFLE_CLIP_03, new Vector3(0, 0, -0.95)},
            {ItemType.CARBINERIFLE_MK2_CAMO, new Vector3(0, 0, -0.95)},
            {ItemType.CARBINERIFLE_MK2_CAMO_02, new Vector3(0, 0, -0.95)},
            {ItemType.CARBINERIFLE_MK2_CAMO_03, new Vector3(0, 0, -0.95)},
            {ItemType.CARBINERIFLE_MK2_CAMO_04, new Vector3(0, 0, -0.95)},
            {ItemType.CARBINERIFLE_MK2_CAMO_05, new Vector3(0, 0, -0.95)},
            {ItemType.CARBINERIFLE_MK2_CAMO_06, new Vector3(0, 0, -0.95)},
            {ItemType.CARBINERIFLE_MK2_CAMO_07, new Vector3(0, 0, -0.95)},
            {ItemType.CARBINERIFLE_MK2_CAMO_08, new Vector3(0, 0, -0.95)},
            {ItemType.CARBINERIFLE_MK2_CAMO_09, new Vector3(0, 0, -0.95)},
            {ItemType.CARBINERIFLE_MK2_CAMO_10, new Vector3(0, 0, -0.95)},
            {ItemType.CARBINERIFLE_MK2_CAMO_IND_01, new Vector3(0, 0, -0.95)},
            {ItemType.CARBINERIFLE_MK2_CLIP_02, new Vector3(0, 0, -0.95)},
            {ItemType.CARBINERIFLE_MK2_CLIP_ARMORPIERCING, new Vector3(0, 0, -0.95)},
            {ItemType.CARBINERIFLE_MK2_CLIP_FMJ, new Vector3(0, 0, -0.95)},
            {ItemType.CARBINERIFLE_MK2_CLIP_INCENDIARY, new Vector3(0, 0, -0.95)},
            {ItemType.CARBINERIFLE_MK2_CLIP_TRACER, new Vector3(0, 0, -0.95)},
            {ItemType.CARBINERIFLE_VARMOD_LUXE, new Vector3(0, 0, -0.95)},
            {ItemType.CERAMICPISTOL_CLIP_02, new Vector3(0, 0, -0.95)},
            {ItemType.CERAMICPISTOL_SUPP, new Vector3(0, 0, -0.95)},
            {ItemType.COMBATMG_CLIP_02, new Vector3(0, 0, -0.95)},
            {ItemType.COMBATMG_MK2_CAMO, new Vector3(0, 0, -0.95)},
            {ItemType.COMBATMG_MK2_CAMO_02, new Vector3(0, 0, -0.95)},
            {ItemType.COMBATMG_MK2_CAMO_03, new Vector3(0, 0, -0.95)},
            {ItemType.COMBATMG_MK2_CAMO_04, new Vector3(0, 0, -0.95)},
            {ItemType.COMBATMG_MK2_CAMO_05, new Vector3(0, 0, -0.95)},
            {ItemType.COMBATMG_MK2_CAMO_06, new Vector3(0, 0, -0.95)},
            {ItemType.COMBATMG_MK2_CAMO_07, new Vector3(0, 0, -0.95)},
            {ItemType.COMBATMG_MK2_CAMO_08, new Vector3(0, 0, -0.95)},
            {ItemType.COMBATMG_MK2_CAMO_09, new Vector3(0, 0, -0.95)},
            {ItemType.COMBATMG_MK2_CAMO_10, new Vector3(0, 0, -0.95)},
            {ItemType.COMBATMG_MK2_CAMO_IND_01, new Vector3(0, 0, -0.95)},
            {ItemType.COMBATMG_MK2_CLIP_02, new Vector3(0, 0, -0.95)},
            {ItemType.COMBATMG_MK2_CLIP_ARMORPIERCING, new Vector3(0, 0, -0.95)},
            {ItemType.COMBATMG_MK2_CLIP_FMJ, new Vector3(0, 0, -0.95)},
            {ItemType.COMBATMG_MK2_CLIP_INCENDIARY, new Vector3(0, 0, -0.95)},
            {ItemType.COMBATMG_MK2_CLIP_TRACER, new Vector3(0, 0, -0.95)},
            {ItemType.COMBATMG_VARMOD_LOWRIDER, new Vector3(0, 0, -0.95)},
            {ItemType.COMBATPDW_CLIP_02, new Vector3(0, 0, -0.95)},
            {ItemType.COMBATPDW_CLIP_03, new Vector3(0, 0, -0.95)},
            {ItemType.COMBATPISTOL_CLIP_02, new Vector3(0, 0, -0.95)},
            {ItemType.COMBATPISTOL_VARMOD_LOWRIDER, new Vector3(0, 0, -0.95)},
            {ItemType.COMPACTRIFLE_CLIP_02, new Vector3(0, 0, -0.95)},
            {ItemType.COMPACTRIFLE_CLIP_03, new Vector3(0, 0, -0.95)},
            {ItemType.GUSENBERG_CLIP_02, new Vector3(0, 0, -0.95)},
            {ItemType.HEAVYPISTOL_CLIP_02, new Vector3(0, 0, -0.95)},
            {ItemType.HEAVYPISTOL_VARMOD_LUXE, new Vector3(0, 0, -0.95)},
            {ItemType.HEAVYSHOTGUN_CLIP_02, new Vector3(0, 0, -0.95)},
            {ItemType.HEAVYSHOTGUN_CLIP_03, new Vector3(0, 0, -0.95)},
            {ItemType.HEAVYSNIPER_MK2_CAMO, new Vector3(0, 0, -0.95)},
            {ItemType.HEAVYSNIPER_MK2_CAMO_02, new Vector3(0, 0, -0.95)},
            {ItemType.HEAVYSNIPER_MK2_CAMO_03, new Vector3(0, 0, -0.95)},
            {ItemType.HEAVYSNIPER_MK2_CAMO_04, new Vector3(0, 0, -0.95)},
            {ItemType.HEAVYSNIPER_MK2_CAMO_05, new Vector3(0, 0, -0.95)},
            {ItemType.HEAVYSNIPER_MK2_CAMO_06, new Vector3(0, 0, -0.95)},
            {ItemType.HEAVYSNIPER_MK2_CAMO_07, new Vector3(0, 0, -0.95)},
            {ItemType.HEAVYSNIPER_MK2_CAMO_08, new Vector3(0, 0, -0.95)},
            {ItemType.HEAVYSNIPER_MK2_CAMO_09, new Vector3(0, 0, -0.95)},
            {ItemType.HEAVYSNIPER_MK2_CAMO_10, new Vector3(0, 0, -0.95)},
            {ItemType.HEAVYSNIPER_MK2_CAMO_IND_01, new Vector3(0, 0, -0.95)},
            {ItemType.HEAVYSNIPER_MK2_CLIP_02, new Vector3(0, 0, -0.95)},
            {ItemType.HEAVYSNIPER_MK2_CLIP_ARMORPIERCING, new Vector3(0, 0, -0.95)},
            {ItemType.HEAVYSNIPER_MK2_CLIP_EXPLOSIVE, new Vector3(0, 0, -0.95)},
            {ItemType.HEAVYSNIPER_MK2_CLIP_FMJ, new Vector3(0, 0, -0.95)},
            {ItemType.HEAVYSNIPER_MK2_CLIP_INCENDIARY, new Vector3(0, 0, -0.95)},
            {ItemType.KNUCKLE_VARMOD_BALLAS, new Vector3(0, 0, -0.95)},
            {ItemType.KNUCKLE_VARMOD_DIAMOND, new Vector3(0, 0, -0.95)},
            {ItemType.KNUCKLE_VARMOD_DOLLAR, new Vector3(0, 0, -0.95)},
            {ItemType.KNUCKLE_VARMOD_HATE, new Vector3(0, 0, -0.95)},
            {ItemType.KNUCKLE_VARMOD_KING, new Vector3(0, 0, -0.95)},
            {ItemType.KNUCKLE_VARMOD_LOVE, new Vector3(0, 0, -0.95)},
            {ItemType.KNUCKLE_VARMOD_PIMP, new Vector3(0, 0, -0.95)},
            {ItemType.KNUCKLE_VARMOD_PLAYER, new Vector3(0, 0, -0.95)},
            {ItemType.KNUCKLE_VARMOD_VAGOS, new Vector3(0, 0, -0.95)},
            {ItemType.MACHINEPISTOL_CLIP_02, new Vector3(0, 0, -0.95)},
            {ItemType.MACHINEPISTOL_CLIP_03, new Vector3(0, 0, -0.95)},
            {ItemType.MARKSMANRIFLE_CLIP_02, new Vector3(0, 0, -0.95)},
            {ItemType.MARKSMANRIFLE_MK2_CAMO, new Vector3(0, 0, -0.95)},
            {ItemType.MARKSMANRIFLE_MK2_CAMO_02, new Vector3(0, 0, -0.95)},
            {ItemType.MARKSMANRIFLE_MK2_CAMO_03, new Vector3(0, 0, -0.95)},
            {ItemType.MARKSMANRIFLE_MK2_CAMO_04, new Vector3(0, 0, -0.95)},
            {ItemType.MARKSMANRIFLE_MK2_CAMO_05, new Vector3(0, 0, -0.95)},
            {ItemType.MARKSMANRIFLE_MK2_CAMO_06, new Vector3(0, 0, -0.95)},
            {ItemType.MARKSMANRIFLE_MK2_CAMO_07, new Vector3(0, 0, -0.95)},
            {ItemType.MARKSMANRIFLE_MK2_CAMO_08, new Vector3(0, 0, -0.95)},
            {ItemType.MARKSMANRIFLE_MK2_CAMO_09, new Vector3(0, 0, -0.95)},
            {ItemType.MARKSMANRIFLE_MK2_CAMO_10, new Vector3(0, 0, -0.95)},
            {ItemType.MARKSMANRIFLE_MK2_CAMO_IND_01, new Vector3(0, 0, -0.95)},
            {ItemType.MARKSMANRIFLE_MK2_CLIP_02, new Vector3(0, 0, -0.95)},
            {ItemType.MARKSMANRIFLE_MK2_CLIP_ARMORPIERCING, new Vector3(0, 0, -0.95)},
            {ItemType.MARKSMANRIFLE_MK2_CLIP_FMJ, new Vector3(0, 0, -0.95)},
            {ItemType.MARKSMANRIFLE_MK2_CLIP_INCENDIARY, new Vector3(0, 0, -0.95)},
            {ItemType.MARKSMANRIFLE_MK2_CLIP_TRACER, new Vector3(0, 0, -0.95)},
            {ItemType.MARKSMANRIFLE_VARMOD_LUXE, new Vector3(0, 0, -0.95)},
            {ItemType.MG_CLIP_02, new Vector3(0, 0, -0.95)},
            {ItemType.MG_VARMOD_LOWRIDER, new Vector3(0, 0, -0.95)},
            {ItemType.MICROSMG_CLIP_02, new Vector3(0, 0, -0.95)},
            {ItemType.MICROSMG_VARMOD_LUXE, new Vector3(0, 0, -0.95)},
            {ItemType.MILITARYRIFLE_CLIP_02, new Vector3(0, 0, -0.95)},
            {ItemType.MILITARYRIFLE_SIGHT_01, new Vector3(0, 0, -0.95)},
            {ItemType.MINISMG_CLIP_02, new Vector3(0, 0, -0.95)},
            {ItemType.PISTOL50_CLIP_02, new Vector3(0, 0, -0.95)},
            {ItemType.PISTOL50_VARMOD_LUXE, new Vector3(0, 0, -0.95)},
            {ItemType.PISTOL_CLIP_02, new Vector3(0, 0, -0.95)},
            {ItemType.PISTOL_MK2_CAMO, new Vector3(0, 0, -0.95)},
            {ItemType.PISTOL_MK2_CAMO_02, new Vector3(0, 0, -0.95)},
            {ItemType.PISTOL_MK2_CAMO_02_SLIDE, new Vector3(0, 0, -0.95)},
            {ItemType.PISTOL_MK2_CAMO_03, new Vector3(0, 0, -0.95)},
            {ItemType.PISTOL_MK2_CAMO_03_SLIDE, new Vector3(0, 0, -0.95)},
            {ItemType.PISTOL_MK2_CAMO_04, new Vector3(0, 0, -0.95)},
            {ItemType.PISTOL_MK2_CAMO_04_SLIDE, new Vector3(0, 0, -0.95)},
            {ItemType.PISTOL_MK2_CAMO_05, new Vector3(0, 0, -0.95)},
            {ItemType.PISTOL_MK2_CAMO_05_SLIDE, new Vector3(0, 0, -0.95)},
            {ItemType.PISTOL_MK2_CAMO_06, new Vector3(0, 0, -0.95)},
            {ItemType.PISTOL_MK2_CAMO_06_SLIDE, new Vector3(0, 0, -0.95)},
            {ItemType.PISTOL_MK2_CAMO_07, new Vector3(0, 0, -0.95)},
            {ItemType.PISTOL_MK2_CAMO_07_SLIDE, new Vector3(0, 0, -0.95)},
            {ItemType.PISTOL_MK2_CAMO_08, new Vector3(0, 0, -0.95)},
            {ItemType.PISTOL_MK2_CAMO_08_SLIDE, new Vector3(0, 0, -0.95)},
            {ItemType.PISTOL_MK2_CAMO_09, new Vector3(0, 0, -0.95)},
            {ItemType.PISTOL_MK2_CAMO_09_SLIDE, new Vector3(0, 0, -0.95)},
            {ItemType.PISTOL_MK2_CAMO_10, new Vector3(0, 0, -0.95)},
            {ItemType.PISTOL_MK2_CAMO_10_SLIDE, new Vector3(0, 0, -0.95)},
            {ItemType.PISTOL_MK2_CAMO_IND_01, new Vector3(0, 0, -0.95)},
            {ItemType.PISTOL_MK2_CAMO_IND_01_SLIDE, new Vector3(0, 0, -0.95)},
            {ItemType.PISTOL_MK2_CAMO_SLIDE, new Vector3(0, 0, -0.95)},
            {ItemType.PISTOL_MK2_CLIP_02, new Vector3(0, 0, -0.95)},
            {ItemType.PISTOL_MK2_CLIP_FMJ, new Vector3(0, 0, -0.95)},
            {ItemType.PISTOL_MK2_CLIP_HOLLOWPOINT, new Vector3(0, 0, -0.95)},
            {ItemType.PISTOL_MK2_CLIP_INCENDIARY, new Vector3(0, 0, -0.95)},
            {ItemType.PISTOL_MK2_CLIP_TRACER, new Vector3(0, 0, -0.95)},
            {ItemType.PISTOL_VARMOD_LUXE, new Vector3(0, 0, -0.95)},
            {ItemType.PUMPSHOTGUN_MK2_CAMO, new Vector3(0, 0, -0.95)},
            {ItemType.PUMPSHOTGUN_MK2_CAMO_02, new Vector3(0, 0, -0.95)},
            {ItemType.PUMPSHOTGUN_MK2_CAMO_03, new Vector3(0, 0, -0.95)},
            {ItemType.PUMPSHOTGUN_MK2_CAMO_04, new Vector3(0, 0, -0.95)},
            {ItemType.PUMPSHOTGUN_MK2_CAMO_05, new Vector3(0, 0, -0.95)},
            {ItemType.PUMPSHOTGUN_MK2_CAMO_06, new Vector3(0, 0, -0.95)},
            {ItemType.PUMPSHOTGUN_MK2_CAMO_07, new Vector3(0, 0, -0.95)},
            {ItemType.PUMPSHOTGUN_MK2_CAMO_08, new Vector3(0, 0, -0.95)},
            {ItemType.PUMPSHOTGUN_MK2_CAMO_09, new Vector3(0, 0, -0.95)},
            {ItemType.PUMPSHOTGUN_MK2_CAMO_10, new Vector3(0, 0, -0.95)},
            {ItemType.PUMPSHOTGUN_MK2_CAMO_IND_01, new Vector3(0, 0, -0.95)},
            {ItemType.PUMPSHOTGUN_MK2_CLIP_ARMORPIERCING, new Vector3(0, 0, -0.95)},
            {ItemType.PUMPSHOTGUN_MK2_CLIP_EXPLOSIVE, new Vector3(0, 0, -0.95)},
            {ItemType.PUMPSHOTGUN_MK2_CLIP_HOLLOWPOINT, new Vector3(0, 0, -0.95)},
            {ItemType.PUMPSHOTGUN_MK2_CLIP_INCENDIARY, new Vector3(0, 0, -0.95)},
            {ItemType.PUMPSHOTGUN_VARMOD_LOWRIDER, new Vector3(0, 0, -0.95)},
            {ItemType.RAYPISTOL_VARMOD_XMAS18, new Vector3(0, 0, -0.95)},
            {ItemType.REVOLVER_MK2_CAMO, new Vector3(0, 0, -0.95)},
            {ItemType.REVOLVER_MK2_CAMO_02, new Vector3(0, 0, -0.95)},
            {ItemType.REVOLVER_MK2_CAMO_03, new Vector3(0, 0, -0.95)},
            {ItemType.REVOLVER_MK2_CAMO_04, new Vector3(0, 0, -0.95)},
            {ItemType.REVOLVER_MK2_CAMO_05, new Vector3(0, 0, -0.95)},
            {ItemType.REVOLVER_MK2_CAMO_06, new Vector3(0, 0, -0.95)},
            {ItemType.REVOLVER_MK2_CAMO_07, new Vector3(0, 0, -0.95)},
            {ItemType.REVOLVER_MK2_CAMO_08, new Vector3(0, 0, -0.95)},
            {ItemType.REVOLVER_MK2_CAMO_09, new Vector3(0, 0, -0.95)},
            {ItemType.REVOLVER_MK2_CAMO_10, new Vector3(0, 0, -0.95)},
            {ItemType.REVOLVER_MK2_CAMO_IND_01, new Vector3(0, 0, -0.95)},
            {ItemType.REVOLVER_MK2_CLIP_FMJ, new Vector3(0, 0, -0.95)},
            {ItemType.REVOLVER_MK2_CLIP_HOLLOWPOINT, new Vector3(0, 0, -0.95)},
            {ItemType.REVOLVER_MK2_CLIP_INCENDIARY, new Vector3(0, 0, -0.95)},
            {ItemType.REVOLVER_MK2_CLIP_TRACER, new Vector3(0, 0, -0.95)},
            {ItemType.REVOLVER_VARMOD_BOSS, new Vector3(0, 0, -0.95)},
            {ItemType.REVOLVER_VARMOD_GOON, new Vector3(0, 0, -0.95)},
            {ItemType.SAWNOFFSHOTGUN_VARMOD_LUXE, new Vector3(0, 0, -0.95)},
            {ItemType.SMG_CLIP_02, new Vector3(0, 0, -0.95)},
            {ItemType.SMG_CLIP_03, new Vector3(0, 0, -0.95)},
            {ItemType.SMG_MK2_CAMO, new Vector3(0, 0, -0.95)},
            {ItemType.SMG_MK2_CAMO_02, new Vector3(0, 0, -0.95)},
            {ItemType.SMG_MK2_CAMO_03, new Vector3(0, 0, -0.95)},
            {ItemType.SMG_MK2_CAMO_04, new Vector3(0, 0, -0.95)},
            {ItemType.SMG_MK2_CAMO_05, new Vector3(0, 0, -0.95)},
            {ItemType.SMG_MK2_CAMO_06, new Vector3(0, 0, -0.95)},
            {ItemType.SMG_MK2_CAMO_07, new Vector3(0, 0, -0.95)},
            {ItemType.SMG_MK2_CAMO_08, new Vector3(0, 0, -0.95)},
            {ItemType.SMG_MK2_CAMO_09, new Vector3(0, 0, -0.95)},
            {ItemType.SMG_MK2_CAMO_10, new Vector3(0, 0, -0.95)},
            {ItemType.SMG_MK2_CAMO_IND_01, new Vector3(0, 0, -0.95)},
            {ItemType.SMG_MK2_CLIP_02, new Vector3(0, 0, -0.95)},
            {ItemType.SMG_MK2_CLIP_FMJ, new Vector3(0, 0, -0.95)},
            {ItemType.SMG_MK2_CLIP_HOLLOWPOINT, new Vector3(0, 0, -0.95)},
            {ItemType.SMG_MK2_CLIP_INCENDIARY, new Vector3(0, 0, -0.95)},
            {ItemType.SMG_MK2_CLIP_TRACER, new Vector3(0, 0, -0.95)},
            {ItemType.SMG_VARMOD_LUXE, new Vector3(0, 0, -0.95)},
            {ItemType.SNIPERRIFLE_VARMOD_LUXE, new Vector3(0, 0, -0.95)},
            {ItemType.SNSPISTOL_CLIP_02, new Vector3(0, 0, -0.95)},
            {ItemType.SNSPISTOL_MK2_CAMO, new Vector3(0, 0, -0.95)},
            {ItemType.SNSPISTOL_MK2_CAMO_02, new Vector3(0, 0, -0.95)},
            {ItemType.SNSPISTOL_MK2_CAMO_02_SLIDE, new Vector3(0, 0, -0.95)},
            {ItemType.SNSPISTOL_MK2_CAMO_03, new Vector3(0, 0, -0.95)},
            {ItemType.SNSPISTOL_MK2_CAMO_03_SLIDE, new Vector3(0, 0, -0.95)},
            {ItemType.SNSPISTOL_MK2_CAMO_04, new Vector3(0, 0, -0.95)},
            {ItemType.SNSPISTOL_MK2_CAMO_04_SLIDE, new Vector3(0, 0, -0.95)},
            {ItemType.SNSPISTOL_MK2_CAMO_05, new Vector3(0, 0, -0.95)},
            {ItemType.SNSPISTOL_MK2_CAMO_05_SLIDE, new Vector3(0, 0, -0.95)},
            {ItemType.SNSPISTOL_MK2_CAMO_06, new Vector3(0, 0, -0.95)},
            {ItemType.SNSPISTOL_MK2_CAMO_06_SLIDE, new Vector3(0, 0, -0.95)},
            {ItemType.SNSPISTOL_MK2_CAMO_07, new Vector3(0, 0, -0.95)},
            {ItemType.SNSPISTOL_MK2_CAMO_07_SLIDE, new Vector3(0, 0, -0.95)},
            {ItemType.SNSPISTOL_MK2_CAMO_08, new Vector3(0, 0, -0.95)},
            {ItemType.SNSPISTOL_MK2_CAMO_08_SLIDE, new Vector3(0, 0, -0.95)},
            {ItemType.SNSPISTOL_MK2_CAMO_09, new Vector3(0, 0, -0.95)},
            {ItemType.SNSPISTOL_MK2_CAMO_09_SLIDE, new Vector3(0, 0, -0.95)},
            {ItemType.SNSPISTOL_MK2_CAMO_10, new Vector3(0, 0, -0.95)},
            {ItemType.SNSPISTOL_MK2_CAMO_10_SLIDE, new Vector3(0, 0, -0.95)},
            {ItemType.SNSPISTOL_MK2_CAMO_IND_01, new Vector3(0, 0, -0.95)},
            {ItemType.SNSPISTOL_MK2_CAMO_IND_01_SLIDE, new Vector3(0, 0, -0.95)},
            {ItemType.SNSPISTOL_MK2_CAMO_SLIDE, new Vector3(0, 0, -0.95)},
            {ItemType.SNSPISTOL_MK2_CLIP_02, new Vector3(0, 0, -0.95)},
            {ItemType.SNSPISTOL_MK2_CLIP_FMJ, new Vector3(0, 0, -0.95)},
            {ItemType.SNSPISTOL_MK2_CLIP_HOLLOWPOINT, new Vector3(0, 0, -0.95)},
            {ItemType.SNSPISTOL_MK2_CLIP_INCENDIARY, new Vector3(0, 0, -0.95)},
            {ItemType.SNSPISTOL_MK2_CLIP_TRACER, new Vector3(0, 0, -0.95)},
            {ItemType.SNSPISTOL_VARMOD_LOWRIDER, new Vector3(0, 0, -0.95)},
            {ItemType.SPECIALCARBINE_CLIP_02, new Vector3(0, 0, -0.95)},
            {ItemType.SPECIALCARBINE_CLIP_03, new Vector3(0, 0, -0.95)},
            {ItemType.SPECIALCARBINE_MK2_CAMO, new Vector3(0, 0, -0.95)},
            {ItemType.SPECIALCARBINE_MK2_CAMO_02, new Vector3(0, 0, -0.95)},
            {ItemType.SPECIALCARBINE_MK2_CAMO_03, new Vector3(0, 0, -0.95)},
            {ItemType.SPECIALCARBINE_MK2_CAMO_04, new Vector3(0, 0, -0.95)},
            {ItemType.SPECIALCARBINE_MK2_CAMO_05, new Vector3(0, 0, -0.95)},
            {ItemType.SPECIALCARBINE_MK2_CAMO_06, new Vector3(0, 0, -0.95)},
            {ItemType.SPECIALCARBINE_MK2_CAMO_07, new Vector3(0, 0, -0.95)},
            {ItemType.SPECIALCARBINE_MK2_CAMO_08, new Vector3(0, 0, -0.95)},
            {ItemType.SPECIALCARBINE_MK2_CAMO_09, new Vector3(0, 0, -0.95)},
            {ItemType.SPECIALCARBINE_MK2_CAMO_10, new Vector3(0, 0, -0.95)},
            {ItemType.SPECIALCARBINE_MK2_CAMO_IND_01, new Vector3(0, 0, -0.95)},
            {ItemType.SPECIALCARBINE_MK2_CLIP_02, new Vector3(0, 0, -0.95)},
            {ItemType.SPECIALCARBINE_MK2_CLIP_ARMORPIERCING, new Vector3(0, 0, -0.95)},
            {ItemType.SPECIALCARBINE_MK2_CLIP_FMJ, new Vector3(0, 0, -0.95)},
            {ItemType.SPECIALCARBINE_MK2_CLIP_INCENDIARY, new Vector3(0, 0, -0.95)},
            {ItemType.SPECIALCARBINE_MK2_CLIP_TRACER, new Vector3(0, 0, -0.95)},
            {ItemType.SPECIALCARBINE_VARMOD_LOWRIDER, new Vector3(0, 0, -0.95)},
            {ItemType.SWITCHBLADE_VARMOD_VAR1, new Vector3(0, 0, -0.95)},
            {ItemType.SWITCHBLADE_VARMOD_VAR2, new Vector3(0, 0, -0.95)},
            {ItemType.VINTAGEPISTOL_CLIP_02, new Vector3(0, 0, -0.95)},

            {ItemType.DigScanner , new Vector3(0,0,-0.99)},
            {ItemType.DigScanner_mk2 , new Vector3(0,0,-0.99)},
            {ItemType.DigScanner_mk3 , new Vector3(0,0,-0.99)},

            {ItemType.DigShovel , new Vector3(0,0,-0.99)},
            {ItemType.DigShovel_mk2 , new Vector3(0,0,-0.99)},
            {ItemType.DigShovel_mk3 , new Vector3(0,0,-0.99)},

            {ItemType.CraftCap , new Vector3()},
            {ItemType.CraftOldCoin , new Vector3()},
            {ItemType.CraftShell , new Vector3()},
            {ItemType.CraftScrapMetal, new Vector3()},
            {ItemType.CraftCopperNugget, new Vector3()},
            {ItemType.CraftIronNugget, new Vector3()},
            {ItemType.CraftTinNugget, new Vector3()},

            {ItemType.CraftCopperWire, new Vector3()},
            {ItemType.CraftOldJewerly, new Vector3()},
            {ItemType.CraftGoldNugget, new Vector3()},
            {ItemType.CraftСollectibleCoin, new Vector3()},
            {ItemType.CraftAncientStatuette, new Vector3()},
            {ItemType.CraftGoldHorseShoe, new Vector3()},
            {ItemType.CraftRelic, new Vector3()},

            {ItemType.CraftIronPart, new Vector3() },
            {ItemType.CraftCopperPart, new Vector3() },
            {ItemType.CraftTinPart, new Vector3() },
            {ItemType.CraftBronzePart, new Vector3() },

            {ItemType.CraftWorkBench, new Vector3() },
            {ItemType.CraftPercolator, new Vector3() },
            {ItemType.CraftSmelter, new Vector3() },
            {ItemType.CraftPartsCollector, new Vector3() },
            {ItemType.CraftWorkBenchUpgrade, new Vector3() },
            {ItemType.CraftWorkBenchUpgrade2, new Vector3() },
        };
        public static Dictionary<ItemType, Vector3> ItemsRotOffset = new Dictionary<ItemType, Vector3>()
        {
            { ItemType.Hat, new Vector3() },
            { ItemType.Mask, new Vector3() },
            { ItemType.Gloves, new Vector3(90, 0, 0) },
            { ItemType.Leg, new Vector3() },
            { ItemType.Bag, new Vector3() },
            { ItemType.Feet, new Vector3() },
            { ItemType.Jewelry, new Vector3() },
            { ItemType.Undershit, new Vector3() },
            { ItemType.BodyArmor, new Vector3(90, 90, 0) },
            { ItemType.Unknown, new Vector3() },
            { ItemType.Top, new Vector3() },
            { ItemType.Glasses, new Vector3() },
            { ItemType.Accessories, new Vector3() },
            { ItemType.CasinoChips, new Vector3() },
            { ItemType.Grenade, new Vector3() },

            { ItemType.Drugs, new Vector3() },
            { ItemType.Material, new Vector3() },
            { ItemType.Debug, new Vector3() },
            { ItemType.HealthKit, new Vector3() },
            { ItemType.GasCan, new Vector3() },
            { ItemType.Сrisps, new Vector3(90, 90, 0) },
            { ItemType.Beer, new Vector3() },
            { ItemType.Pizza, new Vector3() },
            { ItemType.Burger, new Vector3() },
            { ItemType.HotDog, new Vector3() },
            { ItemType.Sandwich, new Vector3() },
            { ItemType.eCola, new Vector3() },
            { ItemType.Sprunk, new Vector3() },
            { ItemType.Lockpick, new Vector3() },
            { ItemType.ArmyLockpick, new Vector3() },
            { ItemType.Pocket, new Vector3() },
            { ItemType.Cuffs, new Vector3() },
            { ItemType.CarKey, new Vector3() },
            { ItemType.Present, new Vector3() },
            { ItemType.KeyRing, new Vector3() },

            { ItemType.RusDrink1, new Vector3() },
            { ItemType.RusDrink2, new Vector3() },
            { ItemType.RusDrink3, new Vector3() },
            { ItemType.YakDrink1, new Vector3() },
            { ItemType.YakDrink2, new Vector3() },
            { ItemType.YakDrink3, new Vector3() },
            { ItemType.LcnDrink1, new Vector3() },
            { ItemType.LcnDrink2, new Vector3() },
            { ItemType.LcnDrink3, new Vector3() },
            { ItemType.ArmDrink1, new Vector3() },
            { ItemType.ArmDrink2, new Vector3() },
            { ItemType.ArmDrink3, new Vector3() },
            { ItemType.WaterBottle, new Vector3() },
            { ItemType.RepairBox, new Vector3(0, 0, -1) },
            { ItemType.SmallHealthKit, new Vector3(0, 0, -1) },

            { ItemType.Pistol, new Vector3(90, 0, 0) },
            { ItemType.Combatpistol, new Vector3(90, 0, 0) },
            { ItemType.Pistol50, new Vector3(90, 0, 0) },
            { ItemType.Snspistol, new Vector3(90, 0, 0) },
            { ItemType.Heavypistol, new Vector3(90, 0, 0) },
            { ItemType.Vintagepistol, new Vector3(90, 0, 0) },
            { ItemType.Marksmanpistol, new Vector3(90, 0, 0) },
            { ItemType.Revolver, new Vector3(90, 0, 0) },
            { ItemType.Appistol, new Vector3(90, 0, 0) },
            { ItemType.Stungun, new Vector3(90, 0, 0) },
            { ItemType.Flaregun, new Vector3(90, 0, 0) },
            { ItemType.Doubleaction, new Vector3(90, 0, 0) },
            { ItemType.Pistol_mk2, new Vector3(90, 0, 0) },
            { ItemType.Snspistol_mk2, new Vector3(90, 0, 0) },
            { ItemType.Revolver_mk2, new Vector3(90, 0, 0) },
            { ItemType.Microsmg, new Vector3(90, 0, 0) },
            { ItemType.Machinepistol, new Vector3(90, 0, 0) },
            { ItemType.Smg, new Vector3(90, 0, 0) },
            { ItemType.Assaultsmg, new Vector3(90, 0, 0) },
            { ItemType.Combatpdw, new Vector3(90, 0, 0) },
            { ItemType.Mg, new Vector3(90, 0, 0) },
            { ItemType.Combatmg, new Vector3(90, 0, 0) },
            { ItemType.Gusenberg, new Vector3(90, 0, 0) },
            { ItemType.Minismg, new Vector3(90, 0, 0) },
            { ItemType.Smg_mk2, new Vector3(90, 0, 0) },
            { ItemType.Combatmg_mk2, new Vector3(90, 0, 0) },
            { ItemType.Assaultrifle, new Vector3(90, 0, 0) },
            { ItemType.Carbinerifle, new Vector3(90, 0, 0) },
            { ItemType.Advancedrifle, new Vector3(90, 0, 0) },
            { ItemType.Specialcarbine, new Vector3(90, 0, 0) },
            { ItemType.Bullpuprifle, new Vector3(90, 0, 0) },
            { ItemType.Compactrifle, new Vector3(90, 0, 0) },
            { ItemType.Assaultrifle_mk2, new Vector3(90, 0, 0) },
            { ItemType.Carbinerifle_mk2, new Vector3(90, 0, 0) },
            { ItemType.Specialcarbine_mk2, new Vector3(90, 0, 0) },
            { ItemType.Bullpuprifle_mk2, new Vector3(90, 0, 0) },
            { ItemType.Sniperrifle, new Vector3(90, 0, 0) },
            { ItemType.Heavysniper, new Vector3(90, 0, 0) },
            { ItemType.Marksmanrifle, new Vector3(90, 0, 0) },
            { ItemType.Heavysniper_mk2, new Vector3(90, 0, 0) },
            { ItemType.Marksmanrifle_mk2, new Vector3(90, 0, 0) },
            { ItemType.Pumpshotgun, new Vector3(90, 0, 0) },
            { ItemType.Sawnoffshotgun, new Vector3(90, 0, 0) },
            { ItemType.Bullpupshotgun, new Vector3(90, 0, 0) },
            { ItemType.Assaultshotgun, new Vector3(90, 0, 0) },
            { ItemType.Musket, new Vector3(90, 0, 0) },
            { ItemType.Heavyshotgun, new Vector3(90, 0, 0) },
            { ItemType.Dbshotgun, new Vector3(90, 0, 0) },
            { ItemType.Autoshotgun, new Vector3(90, 0, 0) },
            { ItemType.Pumpshotgun_mk2, new Vector3(90, 0, 0) },
            { ItemType.Rpg, new Vector3(90, 0, 0) },
            { ItemType.Knife, new Vector3(90, 0, 0) },
            { ItemType.Nightstick, new Vector3(90, 0, 0) },
            { ItemType.Hammer, new Vector3(90, 0, 0) },
            { ItemType.Bat, new Vector3(90, 0, 0) },
            { ItemType.Crowbar, new Vector3(90, 0, 0) },
            { ItemType.Golfclub, new Vector3(90, 0, 0) },
            { ItemType.Bottle, new Vector3(90, 0, 0) },
            { ItemType.Dagger, new Vector3(90, 0, 0) },
            { ItemType.Hatchet, new Vector3(90, 0, 0) },
            { ItemType.Knuckle, new Vector3(90, 0, 0) },
            { ItemType.Machete, new Vector3(90, 0, 0) },
            { ItemType.Flashlight, new Vector3(90, 0, 0) },
            { ItemType.Switchblade, new Vector3(90, 0, 0) },
            { ItemType.Poolcue, new Vector3(90, 0, 0) },
            { ItemType.Wrench, new Vector3(-12, 0, 0) },
            { ItemType.Battleaxe, new Vector3(90, 0, 0) },
            { ItemType.Ceramicpistol, new Vector3(90, 0, 0) },
            { ItemType.Combatshotgun, new Vector3(90, 0, 0) },
            { ItemType.Militaryrifle, new Vector3(90, 0, 0) },

            { ItemType.PistolAmmo, new Vector3(90, 0, 0) },
            { ItemType.RiflesAmmo, new Vector3(90, 0, 0) },
            { ItemType.ShotgunsAmmo, new Vector3(90, 0, 0) },
            { ItemType.SMGAmmo, new Vector3(90, 0, 0) },
            { ItemType.SniperAmmo, new Vector3(90, 0, 0) },

            /* Fishing */
            { ItemType.Rod, new Vector3(-12, 100, 90) },
            { ItemType.RodUpgrade, new Vector3(-12, 100, 90) },
            { ItemType.RodMK2, new Vector3(-12, 100, 90) },
            { ItemType.Bait, new Vector3(90, 0, 0) },
            { ItemType.Naz, new Vector3(90, 0, 0) },
            { ItemType.Kyndja, new Vector3(90, 0, 0) },
            { ItemType.Sig, new Vector3(90, 0, 0) },
            { ItemType.Omyl, new Vector3(90, 0, 0) },
            { ItemType.Nerka, new Vector3(90, 0, 0) },
            { ItemType.Forel, new Vector3(90, 0, 0) },
            { ItemType.Ship, new Vector3(90, 0, 0) },
            { ItemType.Lopatonos, new Vector3(90, 0, 0) },
            { ItemType.Osetr, new Vector3(90, 0, 0) },
            { ItemType.Semga, new Vector3(90, 0, 0) },
            { ItemType.Servyga, new Vector3(90, 0, 0) },
            { ItemType.Beluga, new Vector3(90, 0, 0) },
            { ItemType.Taimen, new Vector3(90, 0, 0) },
            { ItemType.Sterlyad, new Vector3(90, 0, 0) },
            { ItemType.Ydilshik, new Vector3(90, 0, 0) },
            { ItemType.Hailiod, new Vector3(90, 0, 0) },
            { ItemType.Keta, new Vector3(90, 0, 0) },
            { ItemType.Gorbysha, new Vector3(90, 0, 0) },

            //Farmer Job Items
            { ItemType.Hay, new Vector3(0, 0, 0) },
            { ItemType.Seed, new Vector3(0, 0, 0) },

            { ItemType.Payek, new Vector3() },

            { ItemType.LSPDDrone, new Vector3() },
            { ItemType.Drone, new Vector3() },
            { ItemType.NumberPlate, new Vector3() },
            { ItemType.SimCard, new Vector3() },
            { ItemType.ProductBox, new Vector3() },
            { ItemType.TrashBag, new Vector3() },
            { ItemType.CocaLeaves, new Vector3() },
            { ItemType.Cocaine, new Vector3() },
            { ItemType.DrugBookMark, new Vector3() },
            { ItemType.Subject, new Vector3() },

            // [Modification]
            {ItemType.ADVANCEDRIFLE_CLIP_02, new Vector3()},
            {ItemType.ADVANCEDRIFLE_VARMOD_LUXE, new Vector3()},
            {ItemType.APPISTOL_CLIP_02, new Vector3()},
            {ItemType.APPISTOL_VARMOD_LUXE, new Vector3()},
            {ItemType.ASSAULTRIFLE_CLIP_02, new Vector3()},
            {ItemType.ASSAULTRIFLE_CLIP_03, new Vector3()},
            {ItemType.ASSAULTRIFLE_MK2_CAMO, new Vector3()},
            {ItemType.ASSAULTRIFLE_MK2_CAMO_02, new Vector3()},
            {ItemType.ASSAULTRIFLE_MK2_CAMO_03, new Vector3()},
            {ItemType.ASSAULTRIFLE_MK2_CAMO_04, new Vector3()},
            {ItemType.ASSAULTRIFLE_MK2_CAMO_05, new Vector3()},
            {ItemType.ASSAULTRIFLE_MK2_CAMO_06, new Vector3()},
            {ItemType.ASSAULTRIFLE_MK2_CAMO_07, new Vector3()},
            {ItemType.ASSAULTRIFLE_MK2_CAMO_08, new Vector3()},
            {ItemType.ASSAULTRIFLE_MK2_CAMO_09, new Vector3()},
            {ItemType.ASSAULTRIFLE_MK2_CAMO_10, new Vector3()},
            {ItemType.ASSAULTRIFLE_MK2_CAMO_IND_01, new Vector3()},
            {ItemType.ASSAULTRIFLE_MK2_CLIP_02, new Vector3()},
            {ItemType.ASSAULTRIFLE_MK2_CLIP_ARMORPIERCING, new Vector3()},
            {ItemType.ASSAULTRIFLE_MK2_CLIP_FMJ, new Vector3()},
            {ItemType.ASSAULTRIFLE_MK2_CLIP_INCENDIARY, new Vector3()},
            {ItemType.ASSAULTRIFLE_MK2_CLIP_TRACER, new Vector3()},
            {ItemType.ASSAULTRIFLE_VARMOD_LUXE, new Vector3()},
            {ItemType.ASSAULTSHOTGUN_CLIP_02, new Vector3()},
            {ItemType.ASSAULTSMG_CLIP_02, new Vector3()},
            {ItemType.ASSAULTSMG_VARMOD_LOWRIDER, new Vector3()},
            {ItemType.AT_AR_AFGRIP, new Vector3()},
            {ItemType.AT_AR_AFGRIP_02, new Vector3()},
            {ItemType.AT_AR_BARREL_02, new Vector3()},
            {ItemType.AT_AR_FLSH, new Vector3()},
            {ItemType.AT_AR_SUPP, new Vector3()},
            {ItemType.AT_AR_SUPP_02, new Vector3()},
            {ItemType.AT_BP_BARREL_02, new Vector3()},
            {ItemType.AT_CR_BARREL_02, new Vector3()},
            {ItemType.AT_MG_BARREL_02, new Vector3()},
            {ItemType.AT_MRFL_BARREL_02, new Vector3()},
            {ItemType.AT_MUZZLE_01, new Vector3()},
            {ItemType.AT_MUZZLE_02, new Vector3()},
            {ItemType.AT_MUZZLE_03, new Vector3()},
            {ItemType.AT_MUZZLE_04, new Vector3()},
            {ItemType.AT_MUZZLE_05, new Vector3()},
            {ItemType.AT_MUZZLE_06, new Vector3()},
            {ItemType.AT_MUZZLE_07, new Vector3()},
            {ItemType.AT_MUZZLE_08, new Vector3()},
            {ItemType.AT_MUZZLE_09, new Vector3()},
            {ItemType.AT_PI_COMP, new Vector3()},
            {ItemType.AT_PI_COMP_02, new Vector3()},
            {ItemType.AT_PI_COMP_03, new Vector3()},
            {ItemType.AT_PI_FLSH, new Vector3()},
            {ItemType.AT_PI_FLSH_02, new Vector3()},
            {ItemType.AT_PI_FLSH_03, new Vector3()},
            {ItemType.AT_PI_RAIL, new Vector3()},
            {ItemType.AT_PI_RAIL_02, new Vector3()},
            {ItemType.AT_PI_SUPP, new Vector3()},
            {ItemType.AT_PI_SUPP_02, new Vector3()},
            {ItemType.AT_SB_BARREL_02, new Vector3()},
            {ItemType.AT_SC_BARREL_02, new Vector3()},
            {ItemType.AT_SCOPE_LARGE, new Vector3()},
            {ItemType.AT_SCOPE_LARGE_FIXED_ZOOM, new Vector3()},
            {ItemType.AT_SCOPE_LARGE_FIXED_ZOOM_MK2, new Vector3()},
            {ItemType.AT_SCOPE_LARGE_MK2, new Vector3()},
            {ItemType.AT_SCOPE_MACRO, new Vector3()},
            {ItemType.AT_SCOPE_MACRO_02, new Vector3()},
            {ItemType.AT_SCOPE_MACRO_02_MK2, new Vector3()},
            {ItemType.AT_SCOPE_MACRO_02_SMG_MK2, new Vector3()},
            {ItemType.AT_SCOPE_MACRO_MK2, new Vector3()},
            {ItemType.AT_SCOPE_MAX, new Vector3()},
            {ItemType.AT_SCOPE_MEDIUM, new Vector3()},
            {ItemType.AT_SCOPE_MEDIUM_MK2, new Vector3()},
            {ItemType.AT_SCOPE_NV, new Vector3()},
            {ItemType.AT_SCOPE_SMALL, new Vector3()},
            {ItemType.AT_SCOPE_SMALL_02, new Vector3()},
            {ItemType.AT_SCOPE_SMALL_MK2, new Vector3()},
            {ItemType.AT_SCOPE_SMALL_SMG_MK2, new Vector3()},
            {ItemType.AT_SCOPE_THERMAL, new Vector3()},
            {ItemType.AT_SIGHTS, new Vector3()},
            {ItemType.AT_SIGHTS_SMG, new Vector3()},
            {ItemType.AT_SR_BARREL_02, new Vector3()},
            {ItemType.AT_SR_SUPP, new Vector3()},
            {ItemType.AT_SR_SUPP_03, new Vector3()},
            {ItemType.BULLPUPRIFLE_CLIP_02, new Vector3()},
            {ItemType.BULLPUPRIFLE_MK2_CAMO, new Vector3()},
            {ItemType.BULLPUPRIFLE_MK2_CAMO_02, new Vector3()},
            {ItemType.BULLPUPRIFLE_MK2_CAMO_03, new Vector3()},
            {ItemType.BULLPUPRIFLE_MK2_CAMO_04, new Vector3()},
            {ItemType.BULLPUPRIFLE_MK2_CAMO_05, new Vector3()},
            {ItemType.BULLPUPRIFLE_MK2_CAMO_06, new Vector3()},
            {ItemType.BULLPUPRIFLE_MK2_CAMO_07, new Vector3()},
            {ItemType.BULLPUPRIFLE_MK2_CAMO_08, new Vector3()},
            {ItemType.BULLPUPRIFLE_MK2_CAMO_09, new Vector3()},
            {ItemType.BULLPUPRIFLE_MK2_CAMO_10, new Vector3()},
            {ItemType.BULLPUPRIFLE_MK2_CAMO_IND_01, new Vector3()},
            {ItemType.BULLPUPRIFLE_MK2_CLIP_02, new Vector3()},
            {ItemType.BULLPUPRIFLE_MK2_CLIP_ARMORPIERCING, new Vector3()},
            {ItemType.BULLPUPRIFLE_MK2_CLIP_FMJ, new Vector3()},
            {ItemType.BULLPUPRIFLE_MK2_CLIP_INCENDIARY, new Vector3()},
            {ItemType.BULLPUPRIFLE_MK2_CLIP_TRACER, new Vector3()},
            {ItemType.BULLPUPRIFLE_VARMOD_LOW, new Vector3()},
            {ItemType.CARBINERIFLE_CLIP_02, new Vector3()},
            {ItemType.CARBINERIFLE_CLIP_03, new Vector3()},
            {ItemType.CARBINERIFLE_MK2_CAMO, new Vector3()},
            {ItemType.CARBINERIFLE_MK2_CAMO_02, new Vector3()},
            {ItemType.CARBINERIFLE_MK2_CAMO_03, new Vector3()},
            {ItemType.CARBINERIFLE_MK2_CAMO_04, new Vector3()},
            {ItemType.CARBINERIFLE_MK2_CAMO_05, new Vector3()},
            {ItemType.CARBINERIFLE_MK2_CAMO_06, new Vector3()},
            {ItemType.CARBINERIFLE_MK2_CAMO_07, new Vector3()},
            {ItemType.CARBINERIFLE_MK2_CAMO_08, new Vector3()},
            {ItemType.CARBINERIFLE_MK2_CAMO_09, new Vector3()},
            {ItemType.CARBINERIFLE_MK2_CAMO_10, new Vector3()},
            {ItemType.CARBINERIFLE_MK2_CAMO_IND_01, new Vector3()},
            {ItemType.CARBINERIFLE_MK2_CLIP_02, new Vector3()},
            {ItemType.CARBINERIFLE_MK2_CLIP_ARMORPIERCING, new Vector3()},
            {ItemType.CARBINERIFLE_MK2_CLIP_FMJ, new Vector3()},
            {ItemType.CARBINERIFLE_MK2_CLIP_INCENDIARY, new Vector3()},
            {ItemType.CARBINERIFLE_MK2_CLIP_TRACER, new Vector3()},
            {ItemType.CARBINERIFLE_VARMOD_LUXE, new Vector3()},
            {ItemType.CERAMICPISTOL_CLIP_02, new Vector3()},
            {ItemType.CERAMICPISTOL_SUPP, new Vector3()},
            {ItemType.COMBATMG_CLIP_02, new Vector3()},
            {ItemType.COMBATMG_MK2_CAMO, new Vector3()},
            {ItemType.COMBATMG_MK2_CAMO_02, new Vector3()},
            {ItemType.COMBATMG_MK2_CAMO_03, new Vector3()},
            {ItemType.COMBATMG_MK2_CAMO_04, new Vector3()},
            {ItemType.COMBATMG_MK2_CAMO_05, new Vector3()},
            {ItemType.COMBATMG_MK2_CAMO_06, new Vector3()},
            {ItemType.COMBATMG_MK2_CAMO_07, new Vector3()},
            {ItemType.COMBATMG_MK2_CAMO_08, new Vector3()},
            {ItemType.COMBATMG_MK2_CAMO_09, new Vector3()},
            {ItemType.COMBATMG_MK2_CAMO_10, new Vector3()},
            {ItemType.COMBATMG_MK2_CAMO_IND_01, new Vector3()},
            {ItemType.COMBATMG_MK2_CLIP_02, new Vector3()},
            {ItemType.COMBATMG_MK2_CLIP_ARMORPIERCING, new Vector3()},
            {ItemType.COMBATMG_MK2_CLIP_FMJ, new Vector3()},
            {ItemType.COMBATMG_MK2_CLIP_INCENDIARY, new Vector3()},
            {ItemType.COMBATMG_MK2_CLIP_TRACER, new Vector3()},
            {ItemType.COMBATMG_VARMOD_LOWRIDER, new Vector3()},
            {ItemType.COMBATPDW_CLIP_02, new Vector3()},
            {ItemType.COMBATPDW_CLIP_03, new Vector3()},
            {ItemType.COMBATPISTOL_CLIP_02, new Vector3()},
            {ItemType.COMBATPISTOL_VARMOD_LOWRIDER, new Vector3()},
            {ItemType.COMPACTRIFLE_CLIP_02, new Vector3()},
            {ItemType.COMPACTRIFLE_CLIP_03, new Vector3()},
            {ItemType.GUSENBERG_CLIP_02, new Vector3()},
            {ItemType.HEAVYPISTOL_CLIP_02, new Vector3()},
            {ItemType.HEAVYPISTOL_VARMOD_LUXE, new Vector3()},
            {ItemType.HEAVYSHOTGUN_CLIP_02, new Vector3()},
            {ItemType.HEAVYSHOTGUN_CLIP_03, new Vector3()},
            {ItemType.HEAVYSNIPER_MK2_CAMO, new Vector3()},
            {ItemType.HEAVYSNIPER_MK2_CAMO_02, new Vector3()},
            {ItemType.HEAVYSNIPER_MK2_CAMO_03, new Vector3()},
            {ItemType.HEAVYSNIPER_MK2_CAMO_04, new Vector3()},
            {ItemType.HEAVYSNIPER_MK2_CAMO_05, new Vector3()},
            {ItemType.HEAVYSNIPER_MK2_CAMO_06, new Vector3()},
            {ItemType.HEAVYSNIPER_MK2_CAMO_07, new Vector3()},
            {ItemType.HEAVYSNIPER_MK2_CAMO_08, new Vector3()},
            {ItemType.HEAVYSNIPER_MK2_CAMO_09, new Vector3()},
            {ItemType.HEAVYSNIPER_MK2_CAMO_10, new Vector3()},
            {ItemType.HEAVYSNIPER_MK2_CAMO_IND_01, new Vector3()},
            {ItemType.HEAVYSNIPER_MK2_CLIP_02, new Vector3()},
            {ItemType.HEAVYSNIPER_MK2_CLIP_ARMORPIERCING, new Vector3()},
            {ItemType.HEAVYSNIPER_MK2_CLIP_EXPLOSIVE, new Vector3()},
            {ItemType.HEAVYSNIPER_MK2_CLIP_FMJ, new Vector3()},
            {ItemType.HEAVYSNIPER_MK2_CLIP_INCENDIARY, new Vector3()},
            {ItemType.KNUCKLE_VARMOD_BALLAS, new Vector3()},
            {ItemType.KNUCKLE_VARMOD_DIAMOND, new Vector3()},
            {ItemType.KNUCKLE_VARMOD_DOLLAR, new Vector3()},
            {ItemType.KNUCKLE_VARMOD_HATE, new Vector3()},
            {ItemType.KNUCKLE_VARMOD_KING, new Vector3()},
            {ItemType.KNUCKLE_VARMOD_LOVE, new Vector3()},
            {ItemType.KNUCKLE_VARMOD_PIMP, new Vector3()},
            {ItemType.KNUCKLE_VARMOD_PLAYER, new Vector3()},
            {ItemType.KNUCKLE_VARMOD_VAGOS, new Vector3()},
            {ItemType.MACHINEPISTOL_CLIP_02, new Vector3()},
            {ItemType.MACHINEPISTOL_CLIP_03, new Vector3()},
            {ItemType.MARKSMANRIFLE_CLIP_02, new Vector3()},
            {ItemType.MARKSMANRIFLE_MK2_CAMO, new Vector3()},
            {ItemType.MARKSMANRIFLE_MK2_CAMO_02, new Vector3()},
            {ItemType.MARKSMANRIFLE_MK2_CAMO_03, new Vector3()},
            {ItemType.MARKSMANRIFLE_MK2_CAMO_04, new Vector3()},
            {ItemType.MARKSMANRIFLE_MK2_CAMO_05, new Vector3()},
            {ItemType.MARKSMANRIFLE_MK2_CAMO_06, new Vector3()},
            {ItemType.MARKSMANRIFLE_MK2_CAMO_07, new Vector3()},
            {ItemType.MARKSMANRIFLE_MK2_CAMO_08, new Vector3()},
            {ItemType.MARKSMANRIFLE_MK2_CAMO_09, new Vector3()},
            {ItemType.MARKSMANRIFLE_MK2_CAMO_10, new Vector3()},
            {ItemType.MARKSMANRIFLE_MK2_CAMO_IND_01, new Vector3()},
            {ItemType.MARKSMANRIFLE_MK2_CLIP_02, new Vector3()},
            {ItemType.MARKSMANRIFLE_MK2_CLIP_ARMORPIERCING, new Vector3()},
            {ItemType.MARKSMANRIFLE_MK2_CLIP_FMJ, new Vector3()},
            {ItemType.MARKSMANRIFLE_MK2_CLIP_INCENDIARY, new Vector3()},
            {ItemType.MARKSMANRIFLE_MK2_CLIP_TRACER, new Vector3()},
            {ItemType.MARKSMANRIFLE_VARMOD_LUXE, new Vector3()},
            {ItemType.MG_CLIP_02, new Vector3()},
            {ItemType.MG_VARMOD_LOWRIDER, new Vector3()},
            {ItemType.MICROSMG_CLIP_02, new Vector3()},
            {ItemType.MICROSMG_VARMOD_LUXE, new Vector3()},
            {ItemType.MILITARYRIFLE_CLIP_02, new Vector3()},
            {ItemType.MILITARYRIFLE_SIGHT_01, new Vector3()},
            {ItemType.MINISMG_CLIP_02, new Vector3()},
            {ItemType.PISTOL50_CLIP_02, new Vector3()},
            {ItemType.PISTOL50_VARMOD_LUXE, new Vector3()},
            {ItemType.PISTOL_CLIP_02, new Vector3()},
            {ItemType.PISTOL_MK2_CAMO, new Vector3()},
            {ItemType.PISTOL_MK2_CAMO_02, new Vector3()},
            {ItemType.PISTOL_MK2_CAMO_02_SLIDE, new Vector3()},
            {ItemType.PISTOL_MK2_CAMO_03, new Vector3()},
            {ItemType.PISTOL_MK2_CAMO_03_SLIDE, new Vector3()},
            {ItemType.PISTOL_MK2_CAMO_04, new Vector3()},
            {ItemType.PISTOL_MK2_CAMO_04_SLIDE, new Vector3()},
            {ItemType.PISTOL_MK2_CAMO_05, new Vector3()},
            {ItemType.PISTOL_MK2_CAMO_05_SLIDE, new Vector3()},
            {ItemType.PISTOL_MK2_CAMO_06, new Vector3()},
            {ItemType.PISTOL_MK2_CAMO_06_SLIDE, new Vector3()},
            {ItemType.PISTOL_MK2_CAMO_07, new Vector3()},
            {ItemType.PISTOL_MK2_CAMO_07_SLIDE, new Vector3()},
            {ItemType.PISTOL_MK2_CAMO_08, new Vector3()},
            {ItemType.PISTOL_MK2_CAMO_08_SLIDE, new Vector3()},
            {ItemType.PISTOL_MK2_CAMO_09, new Vector3()},
            {ItemType.PISTOL_MK2_CAMO_09_SLIDE, new Vector3()},
            {ItemType.PISTOL_MK2_CAMO_10, new Vector3()},
            {ItemType.PISTOL_MK2_CAMO_10_SLIDE, new Vector3()},
            {ItemType.PISTOL_MK2_CAMO_IND_01, new Vector3()},
            {ItemType.PISTOL_MK2_CAMO_IND_01_SLIDE, new Vector3()},
            {ItemType.PISTOL_MK2_CAMO_SLIDE, new Vector3()},
            {ItemType.PISTOL_MK2_CLIP_02, new Vector3()},
            {ItemType.PISTOL_MK2_CLIP_FMJ, new Vector3()},
            {ItemType.PISTOL_MK2_CLIP_HOLLOWPOINT, new Vector3()},
            {ItemType.PISTOL_MK2_CLIP_INCENDIARY, new Vector3()},
            {ItemType.PISTOL_MK2_CLIP_TRACER, new Vector3()},
            {ItemType.PISTOL_VARMOD_LUXE, new Vector3()},
            {ItemType.PUMPSHOTGUN_MK2_CAMO, new Vector3()},
            {ItemType.PUMPSHOTGUN_MK2_CAMO_02, new Vector3()},
            {ItemType.PUMPSHOTGUN_MK2_CAMO_03, new Vector3()},
            {ItemType.PUMPSHOTGUN_MK2_CAMO_04, new Vector3()},
            {ItemType.PUMPSHOTGUN_MK2_CAMO_05, new Vector3()},
            {ItemType.PUMPSHOTGUN_MK2_CAMO_06, new Vector3()},
            {ItemType.PUMPSHOTGUN_MK2_CAMO_07, new Vector3()},
            {ItemType.PUMPSHOTGUN_MK2_CAMO_08, new Vector3()},
            {ItemType.PUMPSHOTGUN_MK2_CAMO_09, new Vector3()},
            {ItemType.PUMPSHOTGUN_MK2_CAMO_10, new Vector3()},
            {ItemType.PUMPSHOTGUN_MK2_CAMO_IND_01, new Vector3()},
            {ItemType.PUMPSHOTGUN_MK2_CLIP_ARMORPIERCING, new Vector3()},
            {ItemType.PUMPSHOTGUN_MK2_CLIP_EXPLOSIVE, new Vector3()},
            {ItemType.PUMPSHOTGUN_MK2_CLIP_HOLLOWPOINT, new Vector3()},
            {ItemType.PUMPSHOTGUN_MK2_CLIP_INCENDIARY, new Vector3()},
            {ItemType.PUMPSHOTGUN_VARMOD_LOWRIDER, new Vector3()},
            {ItemType.RAYPISTOL_VARMOD_XMAS18, new Vector3()},
            {ItemType.REVOLVER_MK2_CAMO, new Vector3()},
            {ItemType.REVOLVER_MK2_CAMO_02, new Vector3()},
            {ItemType.REVOLVER_MK2_CAMO_03, new Vector3()},
            {ItemType.REVOLVER_MK2_CAMO_04, new Vector3()},
            {ItemType.REVOLVER_MK2_CAMO_05, new Vector3()},
            {ItemType.REVOLVER_MK2_CAMO_06, new Vector3()},
            {ItemType.REVOLVER_MK2_CAMO_07, new Vector3()},
            {ItemType.REVOLVER_MK2_CAMO_08, new Vector3()},
            {ItemType.REVOLVER_MK2_CAMO_09, new Vector3()},
            {ItemType.REVOLVER_MK2_CAMO_10, new Vector3()},
            {ItemType.REVOLVER_MK2_CAMO_IND_01, new Vector3()},
            {ItemType.REVOLVER_MK2_CLIP_FMJ, new Vector3()},
            {ItemType.REVOLVER_MK2_CLIP_HOLLOWPOINT, new Vector3()},
            {ItemType.REVOLVER_MK2_CLIP_INCENDIARY, new Vector3()},
            {ItemType.REVOLVER_MK2_CLIP_TRACER, new Vector3()},
            {ItemType.REVOLVER_VARMOD_BOSS, new Vector3()},
            {ItemType.REVOLVER_VARMOD_GOON, new Vector3()},
            {ItemType.SAWNOFFSHOTGUN_VARMOD_LUXE, new Vector3()},
            {ItemType.SMG_CLIP_02, new Vector3()},
            {ItemType.SMG_CLIP_03, new Vector3()},
            {ItemType.SMG_MK2_CAMO, new Vector3()},
            {ItemType.SMG_MK2_CAMO_02, new Vector3()},
            {ItemType.SMG_MK2_CAMO_03, new Vector3()},
            {ItemType.SMG_MK2_CAMO_04, new Vector3()},
            {ItemType.SMG_MK2_CAMO_05, new Vector3()},
            {ItemType.SMG_MK2_CAMO_06, new Vector3()},
            {ItemType.SMG_MK2_CAMO_07, new Vector3()},
            {ItemType.SMG_MK2_CAMO_08, new Vector3()},
            {ItemType.SMG_MK2_CAMO_09, new Vector3()},
            {ItemType.SMG_MK2_CAMO_10, new Vector3()},
            {ItemType.SMG_MK2_CAMO_IND_01, new Vector3()},
            {ItemType.SMG_MK2_CLIP_02, new Vector3()},
            {ItemType.SMG_MK2_CLIP_FMJ, new Vector3()},
            {ItemType.SMG_MK2_CLIP_HOLLOWPOINT, new Vector3()},
            {ItemType.SMG_MK2_CLIP_INCENDIARY, new Vector3()},
            {ItemType.SMG_MK2_CLIP_TRACER, new Vector3()},
            {ItemType.SMG_VARMOD_LUXE, new Vector3()},
            {ItemType.SNIPERRIFLE_VARMOD_LUXE, new Vector3()},
            {ItemType.SNSPISTOL_CLIP_02, new Vector3()},
            {ItemType.SNSPISTOL_MK2_CAMO, new Vector3()},
            {ItemType.SNSPISTOL_MK2_CAMO_02, new Vector3()},
            {ItemType.SNSPISTOL_MK2_CAMO_02_SLIDE, new Vector3()},
            {ItemType.SNSPISTOL_MK2_CAMO_03, new Vector3()},
            {ItemType.SNSPISTOL_MK2_CAMO_03_SLIDE, new Vector3()},
            {ItemType.SNSPISTOL_MK2_CAMO_04, new Vector3()},
            {ItemType.SNSPISTOL_MK2_CAMO_04_SLIDE, new Vector3()},
            {ItemType.SNSPISTOL_MK2_CAMO_05, new Vector3()},
            {ItemType.SNSPISTOL_MK2_CAMO_05_SLIDE, new Vector3()},
            {ItemType.SNSPISTOL_MK2_CAMO_06, new Vector3()},
            {ItemType.SNSPISTOL_MK2_CAMO_06_SLIDE, new Vector3()},
            {ItemType.SNSPISTOL_MK2_CAMO_07, new Vector3()},
            {ItemType.SNSPISTOL_MK2_CAMO_07_SLIDE, new Vector3()},
            {ItemType.SNSPISTOL_MK2_CAMO_08, new Vector3()},
            {ItemType.SNSPISTOL_MK2_CAMO_08_SLIDE, new Vector3()},
            {ItemType.SNSPISTOL_MK2_CAMO_09, new Vector3()},
            {ItemType.SNSPISTOL_MK2_CAMO_09_SLIDE, new Vector3()},
            {ItemType.SNSPISTOL_MK2_CAMO_10, new Vector3()},
            {ItemType.SNSPISTOL_MK2_CAMO_10_SLIDE, new Vector3()},
            {ItemType.SNSPISTOL_MK2_CAMO_IND_01, new Vector3()},
            {ItemType.SNSPISTOL_MK2_CAMO_IND_01_SLIDE, new Vector3()},
            {ItemType.SNSPISTOL_MK2_CAMO_SLIDE, new Vector3()},
            {ItemType.SNSPISTOL_MK2_CLIP_02, new Vector3()},
            {ItemType.SNSPISTOL_MK2_CLIP_FMJ, new Vector3()},
            {ItemType.SNSPISTOL_MK2_CLIP_HOLLOWPOINT, new Vector3()},
            {ItemType.SNSPISTOL_MK2_CLIP_INCENDIARY, new Vector3()},
            {ItemType.SNSPISTOL_MK2_CLIP_TRACER, new Vector3()},
            {ItemType.SNSPISTOL_VARMOD_LOWRIDER, new Vector3()},
            {ItemType.SPECIALCARBINE_CLIP_02, new Vector3()},
            {ItemType.SPECIALCARBINE_CLIP_03, new Vector3()},
            {ItemType.SPECIALCARBINE_MK2_CAMO, new Vector3()},
            {ItemType.SPECIALCARBINE_MK2_CAMO_02, new Vector3()},
            {ItemType.SPECIALCARBINE_MK2_CAMO_03, new Vector3()},
            {ItemType.SPECIALCARBINE_MK2_CAMO_04, new Vector3()},
            {ItemType.SPECIALCARBINE_MK2_CAMO_05, new Vector3()},
            {ItemType.SPECIALCARBINE_MK2_CAMO_06, new Vector3()},
            {ItemType.SPECIALCARBINE_MK2_CAMO_07, new Vector3()},
            {ItemType.SPECIALCARBINE_MK2_CAMO_08, new Vector3()},
            {ItemType.SPECIALCARBINE_MK2_CAMO_09, new Vector3()},
            {ItemType.SPECIALCARBINE_MK2_CAMO_10, new Vector3()},
            {ItemType.SPECIALCARBINE_MK2_CAMO_IND_01, new Vector3()},
            {ItemType.SPECIALCARBINE_MK2_CLIP_02, new Vector3()},
            {ItemType.SPECIALCARBINE_MK2_CLIP_ARMORPIERCING, new Vector3()},
            {ItemType.SPECIALCARBINE_MK2_CLIP_FMJ, new Vector3()},
            {ItemType.SPECIALCARBINE_MK2_CLIP_INCENDIARY, new Vector3()},
            {ItemType.SPECIALCARBINE_MK2_CLIP_TRACER, new Vector3()},
            {ItemType.SPECIALCARBINE_VARMOD_LOWRIDER, new Vector3()},
            {ItemType.SWITCHBLADE_VARMOD_VAR1, new Vector3()},
            {ItemType.SWITCHBLADE_VARMOD_VAR2, new Vector3()},
            {ItemType.VINTAGEPISTOL_CLIP_02, new Vector3()},

            {ItemType.DigScanner , new Vector3(90,0,0)},
            {ItemType.DigScanner_mk2 , new Vector3(90,0,0)},
            {ItemType.DigScanner_mk3 , new Vector3(90,0,0)},

            {ItemType.DigShovel , new Vector3(90,0,0)},
            {ItemType.DigShovel_mk2 , new Vector3(90,0,0)},
            {ItemType.DigShovel_mk3 , new Vector3(90,0,0)},

            {ItemType.CraftCap , new Vector3()},
            {ItemType.CraftOldCoin , new Vector3()},
            {ItemType.CraftShell , new Vector3()},
            {ItemType.CraftScrapMetal, new Vector3()},
            {ItemType.CraftCopperNugget,  new Vector3()},
            {ItemType.CraftIronNugget,  new Vector3()},
            {ItemType.CraftTinNugget,  new Vector3()},

            {ItemType.CraftCopperWire, new Vector3()},
            {ItemType.CraftOldJewerly, new Vector3()},
            {ItemType.CraftGoldNugget, new Vector3()},
            {ItemType.CraftСollectibleCoin, new Vector3()},
            {ItemType.CraftAncientStatuette, new Vector3()},
            {ItemType.CraftGoldHorseShoe, new Vector3()},
            {ItemType.CraftRelic, new Vector3()},

            {ItemType.CraftIronPart, new Vector3() },
            {ItemType.CraftCopperPart, new Vector3() },
            {ItemType.CraftTinPart, new Vector3() },
            {ItemType.CraftBronzePart, new Vector3() },

            {ItemType.CraftWorkBench, new Vector3() },
            {ItemType.CraftPercolator, new Vector3() },
            {ItemType.CraftSmelter, new Vector3() },
            {ItemType.CraftPartsCollector, new Vector3() },
            {ItemType.CraftWorkBenchUpgrade, new Vector3() },
            {ItemType.CraftWorkBenchUpgrade2, new Vector3() },
        };

        public static Dictionary<ItemType, int> ItemsStacks = new Dictionary<ItemType, int>()
        {
            { ItemType.BagWithMoney, 1 },
            { ItemType.Material, 999 },
            { ItemType.Drugs, 999 },
            { ItemType.BagWithDrill, 1 },
            { ItemType.Debug, 10000 },
            { ItemType.Bandage, 10 },
            { ItemType.HealthKit, 10 },
            { ItemType.GasCan, 1 },
            { ItemType.Сrisps, 30 },
            { ItemType.Beer, 30 },
            { ItemType.Pizza, 30 },
            { ItemType.Burger, 30 },
            { ItemType.HotDog, 30 },
            { ItemType.Sandwich, 30 },
            { ItemType.eCola, 30 },
            { ItemType.Sprunk, 30 },
            { ItemType.Lockpick, 50 },
            { ItemType.ArmyLockpick, 20 },
            { ItemType.Pocket, 10 },
            { ItemType.Cuffs, 10 },
            { ItemType.CarKey, 1 },
            { ItemType.Present, 1 },
            { ItemType.KeyRing, 1 },
            { ItemType.CasinoChips, 1000000 },
            { ItemType.Grenade, 10 },

            { ItemType.Mask, 1 },
            { ItemType.Gloves, 1 },
            { ItemType.Leg, 1 },
            { ItemType.Bag, 1 },
            { ItemType.Feet, 1 },
            { ItemType.Jewelry, 1 },
            { ItemType.Undershit, 1 },
            { ItemType.BodyArmor, 1 },
            { ItemType.Unknown, 1 },
            { ItemType.Top, 1 },
            { ItemType.Hat, 1 },
            { ItemType.Glasses, 1 },
            { ItemType.Accessories, 1 },

            { ItemType.RusDrink1, 10 },
            { ItemType.RusDrink2, 10 },
            { ItemType.RusDrink3, 10 },

            { ItemType.YakDrink1, 10 },
            { ItemType.YakDrink2, 10 },
            { ItemType.YakDrink3, 10 },

            { ItemType.LcnDrink1, 10 },
            { ItemType.LcnDrink2, 10 },
            { ItemType.LcnDrink3, 10 },

            { ItemType.ArmDrink1, 10 },
            { ItemType.ArmDrink2, 10 },
            { ItemType.ArmDrink3, 10 },
            {ItemType.WaterBottle,  30},
            { ItemType.RepairBox, 1 },
            { ItemType.SmallHealthKit, 10 },
            { ItemType.Pistol, 1 },
            { ItemType.Combatpistol, 1 },
            { ItemType.Pistol50, 1 },
            { ItemType.Snspistol, 1 },
            { ItemType.Heavypistol, 1 },
            { ItemType.Vintagepistol, 1 },
            { ItemType.Marksmanpistol, 1 },
            { ItemType.Revolver, 1 },
            { ItemType.Appistol, 1 },
            { ItemType.Stungun, 1 },
            { ItemType.Flaregun, 1 },
            { ItemType.Doubleaction, 1 },
            { ItemType.Pistol_mk2, 1 },
            { ItemType.Snspistol_mk2, 1 },
            { ItemType.Revolver_mk2, 1 },
            { ItemType.Microsmg, 1 },
            { ItemType.Machinepistol, 1 },
            { ItemType.Smg, 1 },
            { ItemType.Assaultsmg, 1 },
            { ItemType.Combatpdw, 1 },
            { ItemType.Mg, 1 },
            { ItemType.Combatmg, 1 },
            { ItemType.Gusenberg, 1 },
            { ItemType.Minismg, 1 },
            { ItemType.Smg_mk2, 1 },
            { ItemType.Combatmg_mk2, 1 },
            { ItemType.Assaultrifle, 1 },
            { ItemType.Carbinerifle, 1 },
            { ItemType.Advancedrifle, 1 },
            { ItemType.Specialcarbine, 1 },
            { ItemType.Bullpuprifle, 1 },
            { ItemType.Compactrifle, 1 },
            { ItemType.Assaultrifle_mk2, 1 },
            { ItemType.Carbinerifle_mk2, 1 },
            { ItemType.Specialcarbine_mk2, 1 },
            { ItemType.Bullpuprifle_mk2, 1 },
            { ItemType.Sniperrifle, 1 },
            { ItemType.Heavysniper, 1 },
            { ItemType.Marksmanrifle, 1 },
            { ItemType.Heavysniper_mk2, 1 },
            { ItemType.Marksmanrifle_mk2, 1 },
            { ItemType.Pumpshotgun, 1 },
            { ItemType.Sawnoffshotgun, 1 },
            { ItemType.Bullpupshotgun, 1 },
            { ItemType.Assaultshotgun, 1 },
            { ItemType.Musket, 1 },
            { ItemType.Heavyshotgun, 1 },
            { ItemType.Dbshotgun, 1 },
            { ItemType.Autoshotgun, 1 },
            { ItemType.Pumpshotgun_mk2, 1 },
            { ItemType.Rpg, 1 },
            { ItemType.Knife, 1 },
            { ItemType.Nightstick, 1 },
            { ItemType.Hammer, 1 },
            { ItemType.Bat, 1 },
            { ItemType.Crowbar, 1 },
            { ItemType.Golfclub, 1 },
            { ItemType.Bottle, 1 },
            { ItemType.Dagger, 1 },
            { ItemType.Hatchet, 1 },
            { ItemType.Knuckle, 1 },
            { ItemType.Machete, 1 },
            { ItemType.Flashlight, 1 },
            { ItemType.Switchblade, 1 },
            { ItemType.Poolcue, 1 },
            { ItemType.Wrench, 1 },
            { ItemType.Battleaxe, 1 },

            { ItemType.Ceramicpistol, 1 },
            { ItemType.Militaryrifle, 1 },
            { ItemType.Combatshotgun, 1 },

            { ItemType.PistolAmmo, 999 },
            { ItemType.RiflesAmmo, 999 },
            { ItemType.ShotgunsAmmo, 999 },
            { ItemType.SMGAmmo, 999 },
            { ItemType.SniperAmmo, 999 },

            /* Fishing */
            { ItemType.Rod, 1 },
            { ItemType.RodUpgrade, 1 },
            { ItemType.RodMK2, 1 },
            { ItemType.Bait, 100 },
            { ItemType.Naz, 100 },
            { ItemType.Kyndja, 100 },
            { ItemType.Sig, 100 },
            { ItemType.Omyl, 100 },
            { ItemType.Nerka, 100 },
            { ItemType.Forel, 100 },
            { ItemType.Ship, 100 },
            { ItemType.Lopatonos, 100 },
            { ItemType.Osetr, 100 },
            { ItemType.Semga, 100 },
            { ItemType.Servyga, 100 },
            { ItemType.Beluga, 100 },
            { ItemType.Taimen, 100 },
            { ItemType.Sterlyad, 100 },
            { ItemType.Ydilshik, 100 },
            { ItemType.Hailiod, 100 },
            { ItemType.Keta, 100 },
            { ItemType.Gorbysha, 100 },

            //Farmer Job Items
            { ItemType.Hay, 60 }, //60 урожая всего в инвентаре
            { ItemType.Seed, 100 }, //100 семян всего в инвентаре (максимум)

            { ItemType.Payek, 10 },

            { ItemType.LSPDDrone, 1 },
            { ItemType.Drone, 1 },
            { ItemType.NumberPlate, 1 },
            { ItemType.SimCard, 1 },
            { ItemType.ProductBox, 10 },
            { ItemType.TrashBag, 1 },
            { ItemType.CocaLeaves, 10 },
            { ItemType.Cocaine, 10 },
            { ItemType.DrugBookMark, 10 },
            { ItemType.Weed, 10 },
            { ItemType.Subject, 1 },

            // [Modification]
            {ItemType.ADVANCEDRIFLE_CLIP_02, 1},
            {ItemType.ADVANCEDRIFLE_VARMOD_LUXE, 1},
            {ItemType.APPISTOL_CLIP_02, 1},
            {ItemType.APPISTOL_VARMOD_LUXE, 1},
            {ItemType.ASSAULTRIFLE_CLIP_02, 1},
            {ItemType.ASSAULTRIFLE_CLIP_03, 1},
            {ItemType.ASSAULTRIFLE_MK2_CAMO, 1},
            {ItemType.ASSAULTRIFLE_MK2_CAMO_02, 1},
            {ItemType.ASSAULTRIFLE_MK2_CAMO_03, 1},
            {ItemType.ASSAULTRIFLE_MK2_CAMO_04, 1},
            {ItemType.ASSAULTRIFLE_MK2_CAMO_05, 1},
            {ItemType.ASSAULTRIFLE_MK2_CAMO_06, 1},
            {ItemType.ASSAULTRIFLE_MK2_CAMO_07, 1},
            {ItemType.ASSAULTRIFLE_MK2_CAMO_08, 1},
            {ItemType.ASSAULTRIFLE_MK2_CAMO_09, 1},
            {ItemType.ASSAULTRIFLE_MK2_CAMO_10, 1},
            {ItemType.ASSAULTRIFLE_MK2_CAMO_IND_01, 1},
            {ItemType.ASSAULTRIFLE_MK2_CLIP_02, 1},
            {ItemType.ASSAULTRIFLE_MK2_CLIP_ARMORPIERCING, 1},
            {ItemType.ASSAULTRIFLE_MK2_CLIP_FMJ, 1},
            {ItemType.ASSAULTRIFLE_MK2_CLIP_INCENDIARY, 1},
            {ItemType.ASSAULTRIFLE_MK2_CLIP_TRACER, 1},
            {ItemType.ASSAULTRIFLE_VARMOD_LUXE, 1},
            {ItemType.ASSAULTSHOTGUN_CLIP_02, 1},
            {ItemType.ASSAULTSMG_CLIP_02, 1},
            {ItemType.ASSAULTSMG_VARMOD_LOWRIDER, 1},
            {ItemType.AT_AR_AFGRIP, 1},
            {ItemType.AT_AR_AFGRIP_02, 1},
            {ItemType.AT_AR_BARREL_02, 1},
            {ItemType.AT_AR_FLSH, 1},
            {ItemType.AT_AR_SUPP, 1},
            {ItemType.AT_AR_SUPP_02, 1},
            {ItemType.AT_BP_BARREL_02, 1},
            {ItemType.AT_CR_BARREL_02, 1},
            {ItemType.AT_MG_BARREL_02, 1},
            {ItemType.AT_MRFL_BARREL_02, 1},
            {ItemType.AT_MUZZLE_01, 1},
            {ItemType.AT_MUZZLE_02, 1},
            {ItemType.AT_MUZZLE_03, 1},
            {ItemType.AT_MUZZLE_04, 1},
            {ItemType.AT_MUZZLE_05, 1},
            {ItemType.AT_MUZZLE_06, 1},
            {ItemType.AT_MUZZLE_07, 1},
            {ItemType.AT_MUZZLE_08, 1},
            {ItemType.AT_MUZZLE_09, 1},
            {ItemType.AT_PI_COMP, 1},
            {ItemType.AT_PI_COMP_02, 1},
            {ItemType.AT_PI_COMP_03, 1},
            {ItemType.AT_PI_FLSH, 1},
            {ItemType.AT_PI_FLSH_02, 1},
            {ItemType.AT_PI_FLSH_03, 1},
            {ItemType.AT_PI_RAIL, 1},
            {ItemType.AT_PI_RAIL_02, 1},
            {ItemType.AT_PI_SUPP, 1},
            {ItemType.AT_PI_SUPP_02, 1},
            {ItemType.AT_SB_BARREL_02, 1},
            {ItemType.AT_SC_BARREL_02, 1},
            {ItemType.AT_SCOPE_LARGE, 1},
            {ItemType.AT_SCOPE_LARGE_FIXED_ZOOM, 1},
            {ItemType.AT_SCOPE_LARGE_FIXED_ZOOM_MK2, 1},
            {ItemType.AT_SCOPE_LARGE_MK2, 1},
            {ItemType.AT_SCOPE_MACRO, 1},
            {ItemType.AT_SCOPE_MACRO_02, 1},
            {ItemType.AT_SCOPE_MACRO_02_MK2, 1},
            {ItemType.AT_SCOPE_MACRO_02_SMG_MK2, 1},
            {ItemType.AT_SCOPE_MACRO_MK2, 1},
            {ItemType.AT_SCOPE_MAX, 1},
            {ItemType.AT_SCOPE_MEDIUM, 1},
            {ItemType.AT_SCOPE_MEDIUM_MK2, 1},
            {ItemType.AT_SCOPE_NV, 1},
            {ItemType.AT_SCOPE_SMALL, 1},
            {ItemType.AT_SCOPE_SMALL_02, 1},
            {ItemType.AT_SCOPE_SMALL_MK2, 1},
            {ItemType.AT_SCOPE_SMALL_SMG_MK2, 1},
            {ItemType.AT_SCOPE_THERMAL, 1},
            {ItemType.AT_SIGHTS, 1},
            {ItemType.AT_SIGHTS_SMG, 1},
            {ItemType.AT_SR_BARREL_02, 1},
            {ItemType.AT_SR_SUPP, 1},
            {ItemType.AT_SR_SUPP_03, 1},
            {ItemType.BULLPUPRIFLE_CLIP_02, 1},
            {ItemType.BULLPUPRIFLE_MK2_CAMO, 1},
            {ItemType.BULLPUPRIFLE_MK2_CAMO_02, 1},
            {ItemType.BULLPUPRIFLE_MK2_CAMO_03, 1},
            {ItemType.BULLPUPRIFLE_MK2_CAMO_04, 1},
            {ItemType.BULLPUPRIFLE_MK2_CAMO_05, 1},
            {ItemType.BULLPUPRIFLE_MK2_CAMO_06, 1},
            {ItemType.BULLPUPRIFLE_MK2_CAMO_07, 1},
            {ItemType.BULLPUPRIFLE_MK2_CAMO_08, 1},
            {ItemType.BULLPUPRIFLE_MK2_CAMO_09, 1},
            {ItemType.BULLPUPRIFLE_MK2_CAMO_10, 1},
            {ItemType.BULLPUPRIFLE_MK2_CAMO_IND_01, 1},
            {ItemType.BULLPUPRIFLE_MK2_CLIP_02, 1},
            {ItemType.BULLPUPRIFLE_MK2_CLIP_ARMORPIERCING, 1},
            {ItemType.BULLPUPRIFLE_MK2_CLIP_FMJ, 1},
            {ItemType.BULLPUPRIFLE_MK2_CLIP_INCENDIARY, 1},
            {ItemType.BULLPUPRIFLE_MK2_CLIP_TRACER, 1},
            {ItemType.BULLPUPRIFLE_VARMOD_LOW, 1},
            {ItemType.CARBINERIFLE_CLIP_02, 1},
            {ItemType.CARBINERIFLE_CLIP_03, 1},
            {ItemType.CARBINERIFLE_MK2_CAMO, 1},
            {ItemType.CARBINERIFLE_MK2_CAMO_02, 1},
            {ItemType.CARBINERIFLE_MK2_CAMO_03, 1},
            {ItemType.CARBINERIFLE_MK2_CAMO_04, 1},
            {ItemType.CARBINERIFLE_MK2_CAMO_05, 1},
            {ItemType.CARBINERIFLE_MK2_CAMO_06, 1},
            {ItemType.CARBINERIFLE_MK2_CAMO_07, 1},
            {ItemType.CARBINERIFLE_MK2_CAMO_08, 1},
            {ItemType.CARBINERIFLE_MK2_CAMO_09, 1},
            {ItemType.CARBINERIFLE_MK2_CAMO_10, 1},
            {ItemType.CARBINERIFLE_MK2_CAMO_IND_01, 1},
            {ItemType.CARBINERIFLE_MK2_CLIP_02, 1},
            {ItemType.CARBINERIFLE_MK2_CLIP_ARMORPIERCING, 1},
            {ItemType.CARBINERIFLE_MK2_CLIP_FMJ, 1},
            {ItemType.CARBINERIFLE_MK2_CLIP_INCENDIARY, 1},
            {ItemType.CARBINERIFLE_MK2_CLIP_TRACER, 1},
            {ItemType.CARBINERIFLE_VARMOD_LUXE, 1},
            {ItemType.CERAMICPISTOL_CLIP_02, 1},
            {ItemType.CERAMICPISTOL_SUPP, 1},
            {ItemType.COMBATMG_CLIP_02, 1},
            {ItemType.COMBATMG_MK2_CAMO, 1},
            {ItemType.COMBATMG_MK2_CAMO_02, 1},
            {ItemType.COMBATMG_MK2_CAMO_03, 1},
            {ItemType.COMBATMG_MK2_CAMO_04, 1},
            {ItemType.COMBATMG_MK2_CAMO_05, 1},
            {ItemType.COMBATMG_MK2_CAMO_06, 1},
            {ItemType.COMBATMG_MK2_CAMO_07, 1},
            {ItemType.COMBATMG_MK2_CAMO_08, 1},
            {ItemType.COMBATMG_MK2_CAMO_09, 1},
            {ItemType.COMBATMG_MK2_CAMO_10, 1},
            {ItemType.COMBATMG_MK2_CAMO_IND_01, 1},
            {ItemType.COMBATMG_MK2_CLIP_02, 1},
            {ItemType.COMBATMG_MK2_CLIP_ARMORPIERCING, 1},
            {ItemType.COMBATMG_MK2_CLIP_FMJ, 1},
            {ItemType.COMBATMG_MK2_CLIP_INCENDIARY, 1},
            {ItemType.COMBATMG_MK2_CLIP_TRACER, 1},
            {ItemType.COMBATMG_VARMOD_LOWRIDER, 1},
            {ItemType.COMBATPDW_CLIP_02, 1},
            {ItemType.COMBATPDW_CLIP_03, 1},
            {ItemType.COMBATPISTOL_CLIP_02, 1},
            {ItemType.COMBATPISTOL_VARMOD_LOWRIDER, 1},
            {ItemType.COMPACTRIFLE_CLIP_02, 1},
            {ItemType.COMPACTRIFLE_CLIP_03, 1},
            {ItemType.GUSENBERG_CLIP_02, 1},
            {ItemType.HEAVYPISTOL_CLIP_02, 1},
            {ItemType.HEAVYPISTOL_VARMOD_LUXE, 1},
            {ItemType.HEAVYSHOTGUN_CLIP_02, 1},
            {ItemType.HEAVYSHOTGUN_CLIP_03, 1},
            {ItemType.HEAVYSNIPER_MK2_CAMO, 1},
            {ItemType.HEAVYSNIPER_MK2_CAMO_02, 1},
            {ItemType.HEAVYSNIPER_MK2_CAMO_03, 1},
            {ItemType.HEAVYSNIPER_MK2_CAMO_04, 1},
            {ItemType.HEAVYSNIPER_MK2_CAMO_05, 1},
            {ItemType.HEAVYSNIPER_MK2_CAMO_06, 1},
            {ItemType.HEAVYSNIPER_MK2_CAMO_07, 1},
            {ItemType.HEAVYSNIPER_MK2_CAMO_08, 1},
            {ItemType.HEAVYSNIPER_MK2_CAMO_09, 1},
            {ItemType.HEAVYSNIPER_MK2_CAMO_10, 1},
            {ItemType.HEAVYSNIPER_MK2_CAMO_IND_01, 1},
            {ItemType.HEAVYSNIPER_MK2_CLIP_02, 1},
            {ItemType.HEAVYSNIPER_MK2_CLIP_ARMORPIERCING, 1},
            {ItemType.HEAVYSNIPER_MK2_CLIP_EXPLOSIVE, 1},
            {ItemType.HEAVYSNIPER_MK2_CLIP_FMJ, 1},
            {ItemType.HEAVYSNIPER_MK2_CLIP_INCENDIARY, 1},
            {ItemType.KNUCKLE_VARMOD_BALLAS, 1},
            {ItemType.KNUCKLE_VARMOD_DIAMOND, 1},
            {ItemType.KNUCKLE_VARMOD_DOLLAR, 1},
            {ItemType.KNUCKLE_VARMOD_HATE, 1},
            {ItemType.KNUCKLE_VARMOD_KING, 1},
            {ItemType.KNUCKLE_VARMOD_LOVE, 1},
            {ItemType.KNUCKLE_VARMOD_PIMP, 1},
            {ItemType.KNUCKLE_VARMOD_PLAYER, 1},
            {ItemType.KNUCKLE_VARMOD_VAGOS, 1},
            {ItemType.MACHINEPISTOL_CLIP_02, 1},
            {ItemType.MACHINEPISTOL_CLIP_03, 1},
            {ItemType.MARKSMANRIFLE_CLIP_02, 1},
            {ItemType.MARKSMANRIFLE_MK2_CAMO, 1},
            {ItemType.MARKSMANRIFLE_MK2_CAMO_02, 1},
            {ItemType.MARKSMANRIFLE_MK2_CAMO_03, 1},
            {ItemType.MARKSMANRIFLE_MK2_CAMO_04, 1},
            {ItemType.MARKSMANRIFLE_MK2_CAMO_05, 1},
            {ItemType.MARKSMANRIFLE_MK2_CAMO_06, 1},
            {ItemType.MARKSMANRIFLE_MK2_CAMO_07, 1},
            {ItemType.MARKSMANRIFLE_MK2_CAMO_08, 1},
            {ItemType.MARKSMANRIFLE_MK2_CAMO_09, 1},
            {ItemType.MARKSMANRIFLE_MK2_CAMO_10, 1},
            {ItemType.MARKSMANRIFLE_MK2_CAMO_IND_01, 1},
            {ItemType.MARKSMANRIFLE_MK2_CLIP_02, 1},
            {ItemType.MARKSMANRIFLE_MK2_CLIP_ARMORPIERCING, 1},
            {ItemType.MARKSMANRIFLE_MK2_CLIP_FMJ, 1},
            {ItemType.MARKSMANRIFLE_MK2_CLIP_INCENDIARY, 1},
            {ItemType.MARKSMANRIFLE_MK2_CLIP_TRACER, 1},
            {ItemType.MARKSMANRIFLE_VARMOD_LUXE, 1},
            {ItemType.MG_CLIP_02, 1},
            {ItemType.MG_VARMOD_LOWRIDER, 1},
            {ItemType.MICROSMG_CLIP_02, 1},
            {ItemType.MICROSMG_VARMOD_LUXE, 1},
            {ItemType.MILITARYRIFLE_CLIP_02, 1},
            {ItemType.MILITARYRIFLE_SIGHT_01, 1},
            {ItemType.MINISMG_CLIP_02, 1},
            {ItemType.PISTOL50_CLIP_02, 1},
            {ItemType.PISTOL50_VARMOD_LUXE, 1},
            {ItemType.PISTOL_CLIP_02, 1},
            {ItemType.PISTOL_MK2_CAMO, 1},
            {ItemType.PISTOL_MK2_CAMO_02, 1},
            {ItemType.PISTOL_MK2_CAMO_02_SLIDE, 1},
            {ItemType.PISTOL_MK2_CAMO_03, 1},
            {ItemType.PISTOL_MK2_CAMO_03_SLIDE, 1},
            {ItemType.PISTOL_MK2_CAMO_04, 1},
            {ItemType.PISTOL_MK2_CAMO_04_SLIDE, 1},
            {ItemType.PISTOL_MK2_CAMO_05, 1},
            {ItemType.PISTOL_MK2_CAMO_05_SLIDE, 1},
            {ItemType.PISTOL_MK2_CAMO_06, 1},
            {ItemType.PISTOL_MK2_CAMO_06_SLIDE, 1},
            {ItemType.PISTOL_MK2_CAMO_07, 1},
            {ItemType.PISTOL_MK2_CAMO_07_SLIDE, 1},
            {ItemType.PISTOL_MK2_CAMO_08, 1},
            {ItemType.PISTOL_MK2_CAMO_08_SLIDE, 1},
            {ItemType.PISTOL_MK2_CAMO_09, 1},
            {ItemType.PISTOL_MK2_CAMO_09_SLIDE, 1},
            {ItemType.PISTOL_MK2_CAMO_10, 1},
            {ItemType.PISTOL_MK2_CAMO_10_SLIDE, 1},
            {ItemType.PISTOL_MK2_CAMO_IND_01, 1},
            {ItemType.PISTOL_MK2_CAMO_IND_01_SLIDE, 1},
            {ItemType.PISTOL_MK2_CAMO_SLIDE, 1},
            {ItemType.PISTOL_MK2_CLIP_02, 1},
            {ItemType.PISTOL_MK2_CLIP_FMJ, 1},
            {ItemType.PISTOL_MK2_CLIP_HOLLOWPOINT, 1},
            {ItemType.PISTOL_MK2_CLIP_INCENDIARY, 1},
            {ItemType.PISTOL_MK2_CLIP_TRACER, 1},
            {ItemType.PISTOL_VARMOD_LUXE, 1},
            {ItemType.PUMPSHOTGUN_MK2_CAMO, 1},
            {ItemType.PUMPSHOTGUN_MK2_CAMO_02, 1},
            {ItemType.PUMPSHOTGUN_MK2_CAMO_03, 1},
            {ItemType.PUMPSHOTGUN_MK2_CAMO_04, 1},
            {ItemType.PUMPSHOTGUN_MK2_CAMO_05, 1},
            {ItemType.PUMPSHOTGUN_MK2_CAMO_06, 1},
            {ItemType.PUMPSHOTGUN_MK2_CAMO_07, 1},
            {ItemType.PUMPSHOTGUN_MK2_CAMO_08, 1},
            {ItemType.PUMPSHOTGUN_MK2_CAMO_09, 1},
            {ItemType.PUMPSHOTGUN_MK2_CAMO_10, 1},
            {ItemType.PUMPSHOTGUN_MK2_CAMO_IND_01, 1},
            {ItemType.PUMPSHOTGUN_MK2_CLIP_ARMORPIERCING, 1},
            {ItemType.PUMPSHOTGUN_MK2_CLIP_EXPLOSIVE, 1},
            {ItemType.PUMPSHOTGUN_MK2_CLIP_HOLLOWPOINT, 1},
            {ItemType.PUMPSHOTGUN_MK2_CLIP_INCENDIARY, 1},
            {ItemType.PUMPSHOTGUN_VARMOD_LOWRIDER, 1},
            {ItemType.RAYPISTOL_VARMOD_XMAS18, 1},
            {ItemType.REVOLVER_MK2_CAMO, 1},
            {ItemType.REVOLVER_MK2_CAMO_02, 1},
            {ItemType.REVOLVER_MK2_CAMO_03, 1},
            {ItemType.REVOLVER_MK2_CAMO_04, 1},
            {ItemType.REVOLVER_MK2_CAMO_05, 1},
            {ItemType.REVOLVER_MK2_CAMO_06, 1},
            {ItemType.REVOLVER_MK2_CAMO_07, 1},
            {ItemType.REVOLVER_MK2_CAMO_08, 1},
            {ItemType.REVOLVER_MK2_CAMO_09, 1},
            {ItemType.REVOLVER_MK2_CAMO_10, 1},
            {ItemType.REVOLVER_MK2_CAMO_IND_01, 1},
            {ItemType.REVOLVER_MK2_CLIP_FMJ, 1},
            {ItemType.REVOLVER_MK2_CLIP_HOLLOWPOINT, 1},
            {ItemType.REVOLVER_MK2_CLIP_INCENDIARY, 1},
            {ItemType.REVOLVER_MK2_CLIP_TRACER, 1},
            {ItemType.REVOLVER_VARMOD_BOSS, 1},
            {ItemType.REVOLVER_VARMOD_GOON, 1},
            {ItemType.SAWNOFFSHOTGUN_VARMOD_LUXE, 1},
            {ItemType.SMG_CLIP_02, 1},
            {ItemType.SMG_CLIP_03, 1},
            {ItemType.SMG_MK2_CAMO, 1},
            {ItemType.SMG_MK2_CAMO_02, 1},
            {ItemType.SMG_MK2_CAMO_03, 1},
            {ItemType.SMG_MK2_CAMO_04, 1},
            {ItemType.SMG_MK2_CAMO_05, 1},
            {ItemType.SMG_MK2_CAMO_06, 1},
            {ItemType.SMG_MK2_CAMO_07, 1},
            {ItemType.SMG_MK2_CAMO_08, 1},
            {ItemType.SMG_MK2_CAMO_09, 1},
            {ItemType.SMG_MK2_CAMO_10, 1},
            {ItemType.SMG_MK2_CAMO_IND_01, 1},
            {ItemType.SMG_MK2_CLIP_02, 1},
            {ItemType.SMG_MK2_CLIP_FMJ, 1},
            {ItemType.SMG_MK2_CLIP_HOLLOWPOINT, 1},
            {ItemType.SMG_MK2_CLIP_INCENDIARY, 1},
            {ItemType.SMG_MK2_CLIP_TRACER, 1},
            {ItemType.SMG_VARMOD_LUXE, 1},
            {ItemType.SNIPERRIFLE_VARMOD_LUXE, 1},
            {ItemType.SNSPISTOL_CLIP_02, 1},
            {ItemType.SNSPISTOL_MK2_CAMO, 1},
            {ItemType.SNSPISTOL_MK2_CAMO_02, 1},
            {ItemType.SNSPISTOL_MK2_CAMO_02_SLIDE, 1},
            {ItemType.SNSPISTOL_MK2_CAMO_03, 1},
            {ItemType.SNSPISTOL_MK2_CAMO_03_SLIDE, 1},
            {ItemType.SNSPISTOL_MK2_CAMO_04, 1},
            {ItemType.SNSPISTOL_MK2_CAMO_04_SLIDE, 1},
            {ItemType.SNSPISTOL_MK2_CAMO_05, 1},
            {ItemType.SNSPISTOL_MK2_CAMO_05_SLIDE, 1},
            {ItemType.SNSPISTOL_MK2_CAMO_06, 1},
            {ItemType.SNSPISTOL_MK2_CAMO_06_SLIDE, 1},
            {ItemType.SNSPISTOL_MK2_CAMO_07, 1},
            {ItemType.SNSPISTOL_MK2_CAMO_07_SLIDE, 1},
            {ItemType.SNSPISTOL_MK2_CAMO_08, 1},
            {ItemType.SNSPISTOL_MK2_CAMO_08_SLIDE, 1},
            {ItemType.SNSPISTOL_MK2_CAMO_09, 1},
            {ItemType.SNSPISTOL_MK2_CAMO_09_SLIDE, 1},
            {ItemType.SNSPISTOL_MK2_CAMO_10, 1},
            {ItemType.SNSPISTOL_MK2_CAMO_10_SLIDE, 1},
            {ItemType.SNSPISTOL_MK2_CAMO_IND_01, 1},
            {ItemType.SNSPISTOL_MK2_CAMO_IND_01_SLIDE, 1},
            {ItemType.SNSPISTOL_MK2_CAMO_SLIDE, 1},
            {ItemType.SNSPISTOL_MK2_CLIP_02, 1},
            {ItemType.SNSPISTOL_MK2_CLIP_FMJ, 1},
            {ItemType.SNSPISTOL_MK2_CLIP_HOLLOWPOINT, 1},
            {ItemType.SNSPISTOL_MK2_CLIP_INCENDIARY, 1},
            {ItemType.SNSPISTOL_MK2_CLIP_TRACER, 1},
            {ItemType.SNSPISTOL_VARMOD_LOWRIDER, 1},
            {ItemType.SPECIALCARBINE_CLIP_02, 1},
            {ItemType.SPECIALCARBINE_CLIP_03, 1},
            {ItemType.SPECIALCARBINE_MK2_CAMO, 1},
            {ItemType.SPECIALCARBINE_MK2_CAMO_02, 1},
            {ItemType.SPECIALCARBINE_MK2_CAMO_03, 1},
            {ItemType.SPECIALCARBINE_MK2_CAMO_04, 1},
            {ItemType.SPECIALCARBINE_MK2_CAMO_05, 1},
            {ItemType.SPECIALCARBINE_MK2_CAMO_06, 1},
            {ItemType.SPECIALCARBINE_MK2_CAMO_07, 1},
            {ItemType.SPECIALCARBINE_MK2_CAMO_08, 1},
            {ItemType.SPECIALCARBINE_MK2_CAMO_09, 1},
            {ItemType.SPECIALCARBINE_MK2_CAMO_10, 1},
            {ItemType.SPECIALCARBINE_MK2_CAMO_IND_01, 1},
            {ItemType.SPECIALCARBINE_MK2_CLIP_02, 1},
            {ItemType.SPECIALCARBINE_MK2_CLIP_ARMORPIERCING, 1},
            {ItemType.SPECIALCARBINE_MK2_CLIP_FMJ, 1},
            {ItemType.SPECIALCARBINE_MK2_CLIP_INCENDIARY, 1},
            {ItemType.SPECIALCARBINE_MK2_CLIP_TRACER, 1},
            {ItemType.SPECIALCARBINE_VARMOD_LOWRIDER, 1},
            {ItemType.SWITCHBLADE_VARMOD_VAR1, 1},
            {ItemType.SWITCHBLADE_VARMOD_VAR2, 1},
            {ItemType.VINTAGEPISTOL_CLIP_02, 1},

            {ItemType.DigScanner , 1},
            {ItemType.DigScanner_mk2 , 1},
            {ItemType.DigScanner_mk3 , 1},

            {ItemType.DigShovel , 1},
            {ItemType.DigShovel_mk2 , 1},
            {ItemType.DigShovel_mk3 , 1},

            {ItemType.CraftCap , 100},
            {ItemType.CraftOldCoin , 100},
            {ItemType.CraftShell , 100},
            {ItemType.CraftScrapMetal, 100},
            {ItemType.CraftCopperNugget,  100},
            {ItemType.CraftIronNugget,  100},
            {ItemType.CraftTinNugget,  100},

            {ItemType.CraftCopperWire, 100},
            {ItemType.CraftOldJewerly, 100},
            {ItemType.CraftGoldNugget, 100},
            {ItemType.CraftСollectibleCoin, 100},
            {ItemType.CraftAncientStatuette, 100},
            {ItemType.CraftGoldHorseShoe, 100},
            {ItemType.CraftRelic, 10},

            {ItemType.CraftIronPart, 100 },
            {ItemType.CraftCopperPart, 100 },
            {ItemType.CraftTinPart, 100 },
            {ItemType.CraftBronzePart, 100 },

            {ItemType.CraftWorkBench, 1 },
            {ItemType.CraftPercolator, 1 },
            {ItemType.CraftSmelter, 1 },
            {ItemType.CraftPartsCollector, 1 },
            {ItemType.CraftWorkBenchUpgrade, 1 },
            {ItemType.CraftWorkBenchUpgrade2, 1 },
        };
        public static Dictionary<ItemType, int> ItemSizeW = new Dictionary<ItemType, int>()
        {
            { ItemType.BagWithMoney, 1 },
            { ItemType.Material, 1 },
            { ItemType.Drugs, 1 },
            { ItemType.BagWithDrill, 1 },
            { ItemType.Debug, 10000 },
            { ItemType.Bandage, 1},
            { ItemType.HealthKit, 1 },
            { ItemType.GasCan, 1 },
            { ItemType.Сrisps, 1 },
            { ItemType.Beer, 1 },
            { ItemType.Pizza, 1 },
            { ItemType.Burger, 1 },
            { ItemType.HotDog, 1 },
            { ItemType.Sandwich, 1 },
            { ItemType.eCola, 1 },
            { ItemType.Sprunk, 1 },
            { ItemType.Lockpick, 1 },
            { ItemType.ArmyLockpick, 1 },
            { ItemType.Pocket, 1},
            { ItemType.Cuffs, 1 },
            { ItemType.CarKey, 1 },
            { ItemType.Present, 1 },
            { ItemType.KeyRing, 1 },
            //{ ItemType.Radio, 1 },
            { ItemType.Grenade, 1 },
            { ItemType.Cocaine, 1 },

            { ItemType.Mask, 1 },
            { ItemType.Gloves, 1 },
            { ItemType.Leg, 1 },
            { ItemType.Bag, 1 },
            { ItemType.Bag1, 1 },
            { ItemType.Feet, 1 },
            { ItemType.Jewelry, 1 },
            { ItemType.Undershit, 1 },
            { ItemType.BodyArmor, 1 },
            { ItemType.BodyArmorgov1, 1 },
            { ItemType.BodyArmorgov2, 1 },
            { ItemType.BodyArmorgov3, 1 },
            { ItemType.BodyArmorgov4, 1 },
            { ItemType.Unknown, 1 },
            { ItemType.Top, 1 },
            { ItemType.Hat, 1 },
            { ItemType.Glasses, 1 },
            { ItemType.Accessories, 1 },
            { ItemType.RusDrink1, 1 },
            { ItemType.RusDrink2, 1 },
            { ItemType.RusDrink3, 1 },
            { ItemType.YakDrink1, 1 },
            { ItemType.YakDrink2, 1 },
            { ItemType.YakDrink3, 1 },
            { ItemType.LcnDrink1, 1 },
            { ItemType.LcnDrink2, 1 },
            { ItemType.LcnDrink3, 1 },
            { ItemType.ArmDrink1, 1 },
            { ItemType.ArmDrink2, 1 },
            { ItemType.ArmDrink3, 1 },
            {ItemType.WaterBottle, 1 },

            { ItemType.RepairBox, 1 },
            { ItemType.SmallHealthKit, 1 },
            { ItemType.Pistol, 1 },
            { ItemType.Combatpistol, 1 },
            { ItemType.Pistol50, 1 },
            { ItemType.Snspistol, 1 },
            { ItemType.Heavypistol, 1 },
            { ItemType.Vintagepistol, 1 },
            { ItemType.Marksmanpistol, 1 },
            { ItemType.Revolver, 1 },
            { ItemType.Appistol, 1 },
            { ItemType.Stungun, 1 },
            { ItemType.Flaregun, 1 },
            { ItemType.Doubleaction, 1 },
            { ItemType.Pistol_mk2, 1 },
            { ItemType.Snspistol_mk2, 1 },
            { ItemType.Revolver_mk2, 1 },
            { ItemType.Microsmg, 1 },
            { ItemType.Machinepistol, 1 },
            { ItemType.Smg, 1 },
            { ItemType.Assaultsmg, 1 },
            { ItemType.Combatpdw, 1 },
            { ItemType.Mg, 1 },
            { ItemType.Combatmg, 1 },
            { ItemType.Gusenberg, 1 },
            { ItemType.Minismg, 1 },
            { ItemType.Smg_mk2, 1 },
            { ItemType.Combatmg_mk2, 1 },
            { ItemType.Assaultrifle, 1 },
            { ItemType.Carbinerifle, 1 },
            { ItemType.Advancedrifle, 1 },
            { ItemType.Specialcarbine, 1 },
            { ItemType.Bullpuprifle, 1 },
            { ItemType.Compactrifle, 1 },
            { ItemType.Assaultrifle_mk2, 1 },
            { ItemType.Carbinerifle_mk2, 1 },
            { ItemType.Specialcarbine_mk2, 1 },
            { ItemType.Bullpuprifle_mk2, 1 },
            { ItemType.Sniperrifle, 1 },
            { ItemType.Heavysniper, 1 },
            { ItemType.Marksmanrifle, 1 },
            { ItemType.Heavysniper_mk2, 1 },
            { ItemType.Marksmanrifle_mk2, 1 },
            { ItemType.Pumpshotgun, 1 },
            { ItemType.Sawnoffshotgun, 1 },
            { ItemType.Bullpupshotgun, 1 },
            { ItemType.Assaultshotgun, 1 },
            { ItemType.Musket, 1 },
            { ItemType.Heavyshotgun, 1 },
            { ItemType.Dbshotgun, 1 },
            { ItemType.Autoshotgun, 1 },
            { ItemType.Pumpshotgun_mk2, 1 },
            { ItemType.Rpg, 1 },
            { ItemType.Knife, 1 },
            { ItemType.Nightstick, 1 },
            { ItemType.Hammer, 1 },
            { ItemType.Bat, 1 },
            { ItemType.Crowbar, 1 },
            { ItemType.Golfclub, 1 },
            { ItemType.Bottle, 1 },
            { ItemType.Dagger, 1 },
            { ItemType.Hatchet, 1 },
            { ItemType.Knuckle, 1 },
            { ItemType.Machete, 1 },
            { ItemType.Flashlight, 1 },
            { ItemType.Switchblade, 1 },
            { ItemType.Poolcue, 1 },
            { ItemType.Wrench, 1 },
            { ItemType.Battleaxe, 1 },
            { ItemType.Rod, 1 },
            { ItemType.RodMK2, 1 },
            { ItemType.RodUpgrade, 1 },
            { ItemType.Bait, 1 },
            { ItemType.Naz, 1 },

            { ItemType.Kyndja, 1},
            { ItemType.Sig, 1},
            { ItemType.Omyl, 1},
            { ItemType.Nerka, 1},
            { ItemType.Forel, 1},
            { ItemType.Ship, 1},
            { ItemType.Lopatonos, 1},
            { ItemType.Osetr, 1},
            { ItemType.Semga, 1},
            { ItemType.Servyga, 1},
            { ItemType.Beluga, 1},
            { ItemType.Taimen, 1},
            { ItemType.Sterlyad, 1},
            { ItemType.Ydilshik, 1},
            { ItemType.Hailiod, 1},
            { ItemType.Keta, 1},
            { ItemType.Gorbysha, 1},
            //{ ItemType.Losos,2 },
            //{ ItemType.Osetr,2 },
            //{ ItemType.GiveBox, 1 },
            //{ ItemType.Box1, 1 },
            //{ ItemType.Box2, 1 },
            //{ ItemType.Box3, 1 },
            //{ ItemType.Box4, 1 },
            //{ ItemType.CarLow, 1 },
            //{ ItemType.CarPremium, 1 },
            //{ ItemType.CarSport, 1 },
            { ItemType.PistolAmmo, 1 },
            { ItemType.RiflesAmmo, 1 },
            { ItemType.ShotgunsAmmo, 1 },
            { ItemType.SMGAmmo, 1 },
            { ItemType.SniperAmmo, 1 },
            //{ ItemType.Pistol_Ammo_Box, 1 },
            //{ ItemType.Rifle_Ammo_Box, 1 },
            //{ ItemType.Smg_Ammo_Box, 1 },
            //{ ItemType.Shotgun_Ammo_Box, 1 },
            //{ ItemType.Sniper_Ammo_Box, 1 },

            { ItemType.Ceramicpistol, 1 },
            { ItemType.Militaryrifle, 1 },
            { ItemType.Combatshotgun, 1 },

            {ItemType.CasinoChips,1 },
            { ItemType.LSPDDrone, 1 },
            { ItemType.Drone, 1 },
            { ItemType.NumberPlate, 1 },
            { ItemType.SimCard, 1 },
            { ItemType.ProductBox, 1 },
            { ItemType.TrashBag, 1 },
            { ItemType.CocaLeaves, 1 },
            { ItemType.DrugBookMark, 1 },
            { ItemType.Weed, 1 },
            { ItemType.Subject, 1 },

            // [Modification]
            {ItemType.ADVANCEDRIFLE_CLIP_02, 1},
            {ItemType.ADVANCEDRIFLE_VARMOD_LUXE, 1},
            {ItemType.APPISTOL_CLIP_02, 1},
            {ItemType.APPISTOL_VARMOD_LUXE, 1},
            {ItemType.ASSAULTRIFLE_CLIP_02, 1},
            {ItemType.ASSAULTRIFLE_CLIP_03, 1},
            {ItemType.ASSAULTRIFLE_MK2_CAMO, 1},
            {ItemType.ASSAULTRIFLE_MK2_CAMO_02, 1},
            {ItemType.ASSAULTRIFLE_MK2_CAMO_03, 1},
            {ItemType.ASSAULTRIFLE_MK2_CAMO_04, 1},
            {ItemType.ASSAULTRIFLE_MK2_CAMO_05, 1},
            {ItemType.ASSAULTRIFLE_MK2_CAMO_06, 1},
            {ItemType.ASSAULTRIFLE_MK2_CAMO_07, 1},
            {ItemType.ASSAULTRIFLE_MK2_CAMO_08, 1},
            {ItemType.ASSAULTRIFLE_MK2_CAMO_09, 1},
            {ItemType.ASSAULTRIFLE_MK2_CAMO_10, 1},
            {ItemType.ASSAULTRIFLE_MK2_CAMO_IND_01, 1},
            {ItemType.ASSAULTRIFLE_MK2_CLIP_02, 1},
            {ItemType.ASSAULTRIFLE_MK2_CLIP_ARMORPIERCING, 1},
            {ItemType.ASSAULTRIFLE_MK2_CLIP_FMJ, 1},
            {ItemType.ASSAULTRIFLE_MK2_CLIP_INCENDIARY, 1},
            {ItemType.ASSAULTRIFLE_MK2_CLIP_TRACER, 1},
            {ItemType.ASSAULTRIFLE_VARMOD_LUXE, 1},
            {ItemType.ASSAULTSHOTGUN_CLIP_02, 1},
            {ItemType.ASSAULTSMG_CLIP_02, 1},
            {ItemType.ASSAULTSMG_VARMOD_LOWRIDER, 1},
            {ItemType.AT_AR_AFGRIP, 1},
            {ItemType.AT_AR_AFGRIP_02, 1},
            {ItemType.AT_AR_BARREL_02, 1},
            {ItemType.AT_AR_FLSH, 1},
            {ItemType.AT_AR_SUPP, 1},
            {ItemType.AT_AR_SUPP_02, 1},
            {ItemType.AT_BP_BARREL_02, 1},
            {ItemType.AT_CR_BARREL_02, 1},
            {ItemType.AT_MG_BARREL_02, 1},
            {ItemType.AT_MRFL_BARREL_02, 1},
            {ItemType.AT_MUZZLE_01, 1},
            {ItemType.AT_MUZZLE_02, 1},
            {ItemType.AT_MUZZLE_03, 1},
            {ItemType.AT_MUZZLE_04, 1},
            {ItemType.AT_MUZZLE_05, 1},
            {ItemType.AT_MUZZLE_06, 1},
            {ItemType.AT_MUZZLE_07, 1},
            {ItemType.AT_MUZZLE_08, 1},
            {ItemType.AT_MUZZLE_09, 1},
            {ItemType.AT_PI_COMP, 1},
            {ItemType.AT_PI_COMP_02, 1},
            {ItemType.AT_PI_COMP_03, 1},
            {ItemType.AT_PI_FLSH, 1},
            {ItemType.AT_PI_FLSH_02, 1},
            {ItemType.AT_PI_FLSH_03, 1},
            {ItemType.AT_PI_RAIL, 1},
            {ItemType.AT_PI_RAIL_02, 1},
            {ItemType.AT_PI_SUPP, 1},
            {ItemType.AT_PI_SUPP_02, 1},
            {ItemType.AT_SB_BARREL_02, 1},
            {ItemType.AT_SC_BARREL_02, 1},
            {ItemType.AT_SCOPE_LARGE, 1},
            {ItemType.AT_SCOPE_LARGE_FIXED_ZOOM, 1},
            {ItemType.AT_SCOPE_LARGE_FIXED_ZOOM_MK2, 1},
            {ItemType.AT_SCOPE_LARGE_MK2, 1},
            {ItemType.AT_SCOPE_MACRO, 1},
            {ItemType.AT_SCOPE_MACRO_02, 1},
            {ItemType.AT_SCOPE_MACRO_02_MK2, 1},
            {ItemType.AT_SCOPE_MACRO_02_SMG_MK2, 1},
            {ItemType.AT_SCOPE_MACRO_MK2, 1},
            {ItemType.AT_SCOPE_MAX, 1},
            {ItemType.AT_SCOPE_MEDIUM, 1},
            {ItemType.AT_SCOPE_MEDIUM_MK2, 1},
            {ItemType.AT_SCOPE_NV, 1},
            {ItemType.AT_SCOPE_SMALL, 1},
            {ItemType.AT_SCOPE_SMALL_02, 1},
            {ItemType.AT_SCOPE_SMALL_MK2, 1},
            {ItemType.AT_SCOPE_SMALL_SMG_MK2, 1},
            {ItemType.AT_SCOPE_THERMAL, 1},
            {ItemType.AT_SIGHTS, 1},
            {ItemType.AT_SIGHTS_SMG, 1},
            {ItemType.AT_SR_BARREL_02, 1},
            {ItemType.AT_SR_SUPP, 1},
            {ItemType.AT_SR_SUPP_03, 1},
            {ItemType.BULLPUPRIFLE_CLIP_02, 1},
            {ItemType.BULLPUPRIFLE_MK2_CAMO, 1},
            {ItemType.BULLPUPRIFLE_MK2_CAMO_02, 1},
            {ItemType.BULLPUPRIFLE_MK2_CAMO_03, 1},
            {ItemType.BULLPUPRIFLE_MK2_CAMO_04, 1},
            {ItemType.BULLPUPRIFLE_MK2_CAMO_05, 1},
            {ItemType.BULLPUPRIFLE_MK2_CAMO_06, 1},
            {ItemType.BULLPUPRIFLE_MK2_CAMO_07, 1},
            {ItemType.BULLPUPRIFLE_MK2_CAMO_08, 1},
            {ItemType.BULLPUPRIFLE_MK2_CAMO_09, 1},
            {ItemType.BULLPUPRIFLE_MK2_CAMO_10, 1},
            {ItemType.BULLPUPRIFLE_MK2_CAMO_IND_01, 1},
            {ItemType.BULLPUPRIFLE_MK2_CLIP_02, 1},
            {ItemType.BULLPUPRIFLE_MK2_CLIP_ARMORPIERCING, 1},
            {ItemType.BULLPUPRIFLE_MK2_CLIP_FMJ, 1},
            {ItemType.BULLPUPRIFLE_MK2_CLIP_INCENDIARY, 1},
            {ItemType.BULLPUPRIFLE_MK2_CLIP_TRACER, 1},
            {ItemType.BULLPUPRIFLE_VARMOD_LOW, 1},
            {ItemType.CARBINERIFLE_CLIP_02, 1},
            {ItemType.CARBINERIFLE_CLIP_03, 1},
            {ItemType.CARBINERIFLE_MK2_CAMO, 1},
            {ItemType.CARBINERIFLE_MK2_CAMO_02, 1},
            {ItemType.CARBINERIFLE_MK2_CAMO_03, 1},
            {ItemType.CARBINERIFLE_MK2_CAMO_04, 1},
            {ItemType.CARBINERIFLE_MK2_CAMO_05, 1},
            {ItemType.CARBINERIFLE_MK2_CAMO_06, 1},
            {ItemType.CARBINERIFLE_MK2_CAMO_07, 1},
            {ItemType.CARBINERIFLE_MK2_CAMO_08, 1},
            {ItemType.CARBINERIFLE_MK2_CAMO_09, 1},
            {ItemType.CARBINERIFLE_MK2_CAMO_10, 1},
            {ItemType.CARBINERIFLE_MK2_CAMO_IND_01, 1},
            {ItemType.CARBINERIFLE_MK2_CLIP_02, 1},
            {ItemType.CARBINERIFLE_MK2_CLIP_ARMORPIERCING, 1},
            {ItemType.CARBINERIFLE_MK2_CLIP_FMJ, 1},
            {ItemType.CARBINERIFLE_MK2_CLIP_INCENDIARY, 1},
            {ItemType.CARBINERIFLE_MK2_CLIP_TRACER, 1},
            {ItemType.CARBINERIFLE_VARMOD_LUXE, 1},
            {ItemType.CERAMICPISTOL_CLIP_02, 1},
            {ItemType.CERAMICPISTOL_SUPP, 1},
            {ItemType.COMBATMG_CLIP_02, 1},
            {ItemType.COMBATMG_MK2_CAMO, 1},
            {ItemType.COMBATMG_MK2_CAMO_02, 1},
            {ItemType.COMBATMG_MK2_CAMO_03, 1},
            {ItemType.COMBATMG_MK2_CAMO_04, 1},
            {ItemType.COMBATMG_MK2_CAMO_05, 1},
            {ItemType.COMBATMG_MK2_CAMO_06, 1},
            {ItemType.COMBATMG_MK2_CAMO_07, 1},
            {ItemType.COMBATMG_MK2_CAMO_08, 1},
            {ItemType.COMBATMG_MK2_CAMO_09, 1},
            {ItemType.COMBATMG_MK2_CAMO_10, 1},
            {ItemType.COMBATMG_MK2_CAMO_IND_01, 1},
            {ItemType.COMBATMG_MK2_CLIP_02, 1},
            {ItemType.COMBATMG_MK2_CLIP_ARMORPIERCING, 1},
            {ItemType.COMBATMG_MK2_CLIP_FMJ, 1},
            {ItemType.COMBATMG_MK2_CLIP_INCENDIARY, 1},
            {ItemType.COMBATMG_MK2_CLIP_TRACER, 1},
            {ItemType.COMBATMG_VARMOD_LOWRIDER, 1},
            {ItemType.COMBATPDW_CLIP_02, 1},
            {ItemType.COMBATPDW_CLIP_03, 1},
            {ItemType.COMBATPISTOL_CLIP_02, 1},
            {ItemType.COMBATPISTOL_VARMOD_LOWRIDER, 1},
            {ItemType.COMPACTRIFLE_CLIP_02, 1},
            {ItemType.COMPACTRIFLE_CLIP_03, 1},
            {ItemType.GUSENBERG_CLIP_02, 1},
            {ItemType.HEAVYPISTOL_CLIP_02, 1},
            {ItemType.HEAVYPISTOL_VARMOD_LUXE, 1},
            {ItemType.HEAVYSHOTGUN_CLIP_02, 1},
            {ItemType.HEAVYSHOTGUN_CLIP_03, 1},
            {ItemType.HEAVYSNIPER_MK2_CAMO, 1},
            {ItemType.HEAVYSNIPER_MK2_CAMO_02, 1},
            {ItemType.HEAVYSNIPER_MK2_CAMO_03, 1},
            {ItemType.HEAVYSNIPER_MK2_CAMO_04, 1},
            {ItemType.HEAVYSNIPER_MK2_CAMO_05, 1},
            {ItemType.HEAVYSNIPER_MK2_CAMO_06, 1},
            {ItemType.HEAVYSNIPER_MK2_CAMO_07, 1},
            {ItemType.HEAVYSNIPER_MK2_CAMO_08, 1},
            {ItemType.HEAVYSNIPER_MK2_CAMO_09, 1},
            {ItemType.HEAVYSNIPER_MK2_CAMO_10, 1},
            {ItemType.HEAVYSNIPER_MK2_CAMO_IND_01, 1},
            {ItemType.HEAVYSNIPER_MK2_CLIP_02, 1},
            {ItemType.HEAVYSNIPER_MK2_CLIP_ARMORPIERCING, 1},
            {ItemType.HEAVYSNIPER_MK2_CLIP_EXPLOSIVE, 1},
            {ItemType.HEAVYSNIPER_MK2_CLIP_FMJ, 1},
            {ItemType.HEAVYSNIPER_MK2_CLIP_INCENDIARY, 1},
            {ItemType.KNUCKLE_VARMOD_BALLAS, 1},
            {ItemType.KNUCKLE_VARMOD_DIAMOND, 1},
            {ItemType.KNUCKLE_VARMOD_DOLLAR, 1},
            {ItemType.KNUCKLE_VARMOD_HATE, 1},
            {ItemType.KNUCKLE_VARMOD_KING, 1},
            {ItemType.KNUCKLE_VARMOD_LOVE, 1},
            {ItemType.KNUCKLE_VARMOD_PIMP, 1},
            {ItemType.KNUCKLE_VARMOD_PLAYER, 1},
            {ItemType.KNUCKLE_VARMOD_VAGOS, 1},
            {ItemType.MACHINEPISTOL_CLIP_02, 1},
            {ItemType.MACHINEPISTOL_CLIP_03, 1},
            {ItemType.MARKSMANRIFLE_CLIP_02, 1},
            {ItemType.MARKSMANRIFLE_MK2_CAMO, 1},
            {ItemType.MARKSMANRIFLE_MK2_CAMO_02, 1},
            {ItemType.MARKSMANRIFLE_MK2_CAMO_03, 1},
            {ItemType.MARKSMANRIFLE_MK2_CAMO_04, 1},
            {ItemType.MARKSMANRIFLE_MK2_CAMO_05, 1},
            {ItemType.MARKSMANRIFLE_MK2_CAMO_06, 1},
            {ItemType.MARKSMANRIFLE_MK2_CAMO_07, 1},
            {ItemType.MARKSMANRIFLE_MK2_CAMO_08, 1},
            {ItemType.MARKSMANRIFLE_MK2_CAMO_09, 1},
            {ItemType.MARKSMANRIFLE_MK2_CAMO_10, 1},
            {ItemType.MARKSMANRIFLE_MK2_CAMO_IND_01, 1},
            {ItemType.MARKSMANRIFLE_MK2_CLIP_02, 1},
            {ItemType.MARKSMANRIFLE_MK2_CLIP_ARMORPIERCING, 1},
            {ItemType.MARKSMANRIFLE_MK2_CLIP_FMJ, 1},
            {ItemType.MARKSMANRIFLE_MK2_CLIP_INCENDIARY, 1},
            {ItemType.MARKSMANRIFLE_MK2_CLIP_TRACER, 1},
            {ItemType.MARKSMANRIFLE_VARMOD_LUXE, 1},
            {ItemType.MG_CLIP_02, 1},
            {ItemType.MG_VARMOD_LOWRIDER, 1},
            {ItemType.MICROSMG_CLIP_02, 1},
            {ItemType.MICROSMG_VARMOD_LUXE, 1},
            {ItemType.MILITARYRIFLE_CLIP_02, 1},
            {ItemType.MILITARYRIFLE_SIGHT_01, 1},
            {ItemType.MINISMG_CLIP_02, 1},
            {ItemType.PISTOL50_CLIP_02, 1},
            {ItemType.PISTOL50_VARMOD_LUXE, 1},
            {ItemType.PISTOL_CLIP_02, 1},
            {ItemType.PISTOL_MK2_CAMO, 1},
            {ItemType.PISTOL_MK2_CAMO_02, 1},
            {ItemType.PISTOL_MK2_CAMO_02_SLIDE, 1},
            {ItemType.PISTOL_MK2_CAMO_03, 1},
            {ItemType.PISTOL_MK2_CAMO_03_SLIDE, 1},
            {ItemType.PISTOL_MK2_CAMO_04, 1},
            {ItemType.PISTOL_MK2_CAMO_04_SLIDE, 1},
            {ItemType.PISTOL_MK2_CAMO_05, 1},
            {ItemType.PISTOL_MK2_CAMO_05_SLIDE, 1},
            {ItemType.PISTOL_MK2_CAMO_06, 1},
            {ItemType.PISTOL_MK2_CAMO_06_SLIDE, 1},
            {ItemType.PISTOL_MK2_CAMO_07, 1},
            {ItemType.PISTOL_MK2_CAMO_07_SLIDE, 1},
            {ItemType.PISTOL_MK2_CAMO_08, 1},
            {ItemType.PISTOL_MK2_CAMO_08_SLIDE, 1},
            {ItemType.PISTOL_MK2_CAMO_09, 1},
            {ItemType.PISTOL_MK2_CAMO_09_SLIDE, 1},
            {ItemType.PISTOL_MK2_CAMO_10, 1},
            {ItemType.PISTOL_MK2_CAMO_10_SLIDE, 1},
            {ItemType.PISTOL_MK2_CAMO_IND_01, 1},
            {ItemType.PISTOL_MK2_CAMO_IND_01_SLIDE, 1},
            {ItemType.PISTOL_MK2_CAMO_SLIDE, 1},
            {ItemType.PISTOL_MK2_CLIP_02, 1},
            {ItemType.PISTOL_MK2_CLIP_FMJ, 1},
            {ItemType.PISTOL_MK2_CLIP_HOLLOWPOINT, 1},
            {ItemType.PISTOL_MK2_CLIP_INCENDIARY, 1},
            {ItemType.PISTOL_MK2_CLIP_TRACER, 1},
            {ItemType.PISTOL_VARMOD_LUXE, 1},
            {ItemType.PUMPSHOTGUN_MK2_CAMO, 1},
            {ItemType.PUMPSHOTGUN_MK2_CAMO_02, 1},
            {ItemType.PUMPSHOTGUN_MK2_CAMO_03, 1},
            {ItemType.PUMPSHOTGUN_MK2_CAMO_04, 1},
            {ItemType.PUMPSHOTGUN_MK2_CAMO_05, 1},
            {ItemType.PUMPSHOTGUN_MK2_CAMO_06, 1},
            {ItemType.PUMPSHOTGUN_MK2_CAMO_07, 1},
            {ItemType.PUMPSHOTGUN_MK2_CAMO_08, 1},
            {ItemType.PUMPSHOTGUN_MK2_CAMO_09, 1},
            {ItemType.PUMPSHOTGUN_MK2_CAMO_10, 1},
            {ItemType.PUMPSHOTGUN_MK2_CAMO_IND_01, 1},
            {ItemType.PUMPSHOTGUN_MK2_CLIP_ARMORPIERCING, 1},
            {ItemType.PUMPSHOTGUN_MK2_CLIP_EXPLOSIVE, 1},
            {ItemType.PUMPSHOTGUN_MK2_CLIP_HOLLOWPOINT, 1},
            {ItemType.PUMPSHOTGUN_MK2_CLIP_INCENDIARY, 1},
            {ItemType.PUMPSHOTGUN_VARMOD_LOWRIDER, 1},
            {ItemType.RAYPISTOL_VARMOD_XMAS18, 1},
            {ItemType.REVOLVER_MK2_CAMO, 1},
            {ItemType.REVOLVER_MK2_CAMO_02, 1},
            {ItemType.REVOLVER_MK2_CAMO_03, 1},
            {ItemType.REVOLVER_MK2_CAMO_04, 1},
            {ItemType.REVOLVER_MK2_CAMO_05, 1},
            {ItemType.REVOLVER_MK2_CAMO_06, 1},
            {ItemType.REVOLVER_MK2_CAMO_07, 1},
            {ItemType.REVOLVER_MK2_CAMO_08, 1},
            {ItemType.REVOLVER_MK2_CAMO_09, 1},
            {ItemType.REVOLVER_MK2_CAMO_10, 1},
            {ItemType.REVOLVER_MK2_CAMO_IND_01, 1},
            {ItemType.REVOLVER_MK2_CLIP_FMJ, 1},
            {ItemType.REVOLVER_MK2_CLIP_HOLLOWPOINT, 1},
            {ItemType.REVOLVER_MK2_CLIP_INCENDIARY, 1},
            {ItemType.REVOLVER_MK2_CLIP_TRACER, 1},
            {ItemType.REVOLVER_VARMOD_BOSS, 1},
            {ItemType.REVOLVER_VARMOD_GOON, 1},
            {ItemType.SAWNOFFSHOTGUN_VARMOD_LUXE, 1},
            {ItemType.SMG_CLIP_02, 1},
            {ItemType.SMG_CLIP_03, 1},
            {ItemType.SMG_MK2_CAMO, 1},
            {ItemType.SMG_MK2_CAMO_02, 1},
            {ItemType.SMG_MK2_CAMO_03, 1},
            {ItemType.SMG_MK2_CAMO_04, 1},
            {ItemType.SMG_MK2_CAMO_05, 1},
            {ItemType.SMG_MK2_CAMO_06, 1},
            {ItemType.SMG_MK2_CAMO_07, 1},
            {ItemType.SMG_MK2_CAMO_08, 1},
            {ItemType.SMG_MK2_CAMO_09, 1},
            {ItemType.SMG_MK2_CAMO_10, 1},
            {ItemType.SMG_MK2_CAMO_IND_01, 1},
            {ItemType.SMG_MK2_CLIP_02, 1},
            {ItemType.SMG_MK2_CLIP_FMJ, 1},
            {ItemType.SMG_MK2_CLIP_HOLLOWPOINT, 1},
            {ItemType.SMG_MK2_CLIP_INCENDIARY, 1},
            {ItemType.SMG_MK2_CLIP_TRACER, 1},
            {ItemType.SMG_VARMOD_LUXE, 1},
            {ItemType.SNIPERRIFLE_VARMOD_LUXE, 1},
            {ItemType.SNSPISTOL_CLIP_02, 1},
            {ItemType.SNSPISTOL_MK2_CAMO, 1},
            {ItemType.SNSPISTOL_MK2_CAMO_02, 1},
            {ItemType.SNSPISTOL_MK2_CAMO_02_SLIDE, 1},
            {ItemType.SNSPISTOL_MK2_CAMO_03, 1},
            {ItemType.SNSPISTOL_MK2_CAMO_03_SLIDE, 1},
            {ItemType.SNSPISTOL_MK2_CAMO_04, 1},
            {ItemType.SNSPISTOL_MK2_CAMO_04_SLIDE, 1},
            {ItemType.SNSPISTOL_MK2_CAMO_05, 1},
            {ItemType.SNSPISTOL_MK2_CAMO_05_SLIDE, 1},
            {ItemType.SNSPISTOL_MK2_CAMO_06, 1},
            {ItemType.SNSPISTOL_MK2_CAMO_06_SLIDE, 1},
            {ItemType.SNSPISTOL_MK2_CAMO_07, 1},
            {ItemType.SNSPISTOL_MK2_CAMO_07_SLIDE, 1},
            {ItemType.SNSPISTOL_MK2_CAMO_08, 1},
            {ItemType.SNSPISTOL_MK2_CAMO_08_SLIDE, 1},
            {ItemType.SNSPISTOL_MK2_CAMO_09, 1},
            {ItemType.SNSPISTOL_MK2_CAMO_09_SLIDE, 1},
            {ItemType.SNSPISTOL_MK2_CAMO_10, 1},
            {ItemType.SNSPISTOL_MK2_CAMO_10_SLIDE, 1},
            {ItemType.SNSPISTOL_MK2_CAMO_IND_01, 1},
            {ItemType.SNSPISTOL_MK2_CAMO_IND_01_SLIDE, 1},
            {ItemType.SNSPISTOL_MK2_CAMO_SLIDE, 1},
            {ItemType.SNSPISTOL_MK2_CLIP_02, 1},
            {ItemType.SNSPISTOL_MK2_CLIP_FMJ, 1},
            {ItemType.SNSPISTOL_MK2_CLIP_HOLLOWPOINT, 1},
            {ItemType.SNSPISTOL_MK2_CLIP_INCENDIARY, 1},
            {ItemType.SNSPISTOL_MK2_CLIP_TRACER, 1},
            {ItemType.SNSPISTOL_VARMOD_LOWRIDER, 1},
            {ItemType.SPECIALCARBINE_CLIP_02, 1},
            {ItemType.SPECIALCARBINE_CLIP_03, 1},
            {ItemType.SPECIALCARBINE_MK2_CAMO, 1},
            {ItemType.SPECIALCARBINE_MK2_CAMO_02, 1},
            {ItemType.SPECIALCARBINE_MK2_CAMO_03, 1},
            {ItemType.SPECIALCARBINE_MK2_CAMO_04, 1},
            {ItemType.SPECIALCARBINE_MK2_CAMO_05, 1},
            {ItemType.SPECIALCARBINE_MK2_CAMO_06, 1},
            {ItemType.SPECIALCARBINE_MK2_CAMO_07, 1},
            {ItemType.SPECIALCARBINE_MK2_CAMO_08, 1},
            {ItemType.SPECIALCARBINE_MK2_CAMO_09, 1},
            {ItemType.SPECIALCARBINE_MK2_CAMO_10, 1},
            {ItemType.SPECIALCARBINE_MK2_CAMO_IND_01, 1},
            {ItemType.SPECIALCARBINE_MK2_CLIP_02, 1},
            {ItemType.SPECIALCARBINE_MK2_CLIP_ARMORPIERCING, 1},
            {ItemType.SPECIALCARBINE_MK2_CLIP_FMJ, 1},
            {ItemType.SPECIALCARBINE_MK2_CLIP_INCENDIARY, 1},
            {ItemType.SPECIALCARBINE_MK2_CLIP_TRACER, 1},
            {ItemType.SPECIALCARBINE_VARMOD_LOWRIDER, 1},
            {ItemType.SWITCHBLADE_VARMOD_VAR1, 1},
            {ItemType.SWITCHBLADE_VARMOD_VAR2, 1},
            {ItemType.VINTAGEPISTOL_CLIP_02, 1},

            {ItemType.DigScanner , 1},
            {ItemType.DigScanner_mk2 , 1},
            {ItemType.DigScanner_mk3 , 1},

            {ItemType.DigShovel , 1},
            {ItemType.DigShovel_mk2 , 1},
            {ItemType.DigShovel_mk3 , 1},

            {ItemType.CraftCap , 1},
            {ItemType.CraftOldCoin , 1},
            {ItemType.CraftShell , 1},
            {ItemType.CraftScrapMetal, 1},
            {ItemType.CraftCopperNugget,  1},
            {ItemType.CraftIronNugget,  1},
            {ItemType.CraftTinNugget,  1},

            {ItemType.CraftCopperWire, 1},
            {ItemType.CraftOldJewerly, 1},
            {ItemType.CraftGoldNugget, 1},
            {ItemType.CraftСollectibleCoin, 1},
            {ItemType.CraftAncientStatuette, 1},
            {ItemType.CraftGoldHorseShoe, 1},
            {ItemType.CraftRelic, 1},

            {ItemType.CraftIronPart, 1 },
            {ItemType.CraftCopperPart, 1 },
            {ItemType.CraftTinPart, 1 },
            {ItemType.CraftBronzePart, 1 },

            {ItemType.CraftWorkBench, 1 },
            {ItemType.CraftPercolator, 1 },
            {ItemType.CraftSmelter, 1 },
            {ItemType.CraftPartsCollector, 1 },
            {ItemType.CraftWorkBenchUpgrade, 1 },
            {ItemType.CraftWorkBenchUpgrade2, 1 },
        };
        public static Dictionary<ItemType, int> ItemSizeH = new Dictionary<ItemType, int>()
        {
            { ItemType.BagWithMoney, 1 },
            { ItemType.Material, 1 },
            { ItemType.Drugs, 1 },
            { ItemType.BagWithDrill, 1 },
            { ItemType.Bandage, 1 },
            { ItemType.Debug, 10000 },
            { ItemType.HealthKit, 1 },
            { ItemType.GasCan, 1 },
            { ItemType.Сrisps, 1 },
            { ItemType.Beer, 1 },
            { ItemType.Pizza, 1 },
            { ItemType.Burger, 1 },
            { ItemType.HotDog, 1 },
            { ItemType.Sandwich, 1 },
            { ItemType.eCola, 1 },
            { ItemType.Sprunk, 1 },
            { ItemType.Lockpick, 1 },
            { ItemType.ArmyLockpick, 1 },
            { ItemType.Pocket, 1 },
            { ItemType.Cuffs, 1 },
            { ItemType.CarKey, 1 },
            { ItemType.Present, 1 },
            { ItemType.KeyRing, 1 },
            //{ ItemType.Radio, 1 },
            { ItemType.Grenade, 1 },
            { ItemType.Cocaine, 1 },


            { ItemType.Mask, 1 },
            { ItemType.Gloves, 1 },
            { ItemType.Leg, 1 },
            { ItemType.Bag, 1 },
            { ItemType.Bag1, 1 },
            { ItemType.Feet, 1 },
            { ItemType.Jewelry, 1 },
            { ItemType.Undershit, 1 },
            { ItemType.BodyArmor, 1 },
            { ItemType.BodyArmorgov1, 1 },
            { ItemType.BodyArmorgov2, 1 },
            { ItemType.BodyArmorgov3, 1 },
            { ItemType.BodyArmorgov4, 1 },
            { ItemType.Unknown, 1 },
            { ItemType.Top, 1 },
            { ItemType.Hat, 1 },
            { ItemType.Glasses, 1 },
            { ItemType.Accessories, 1 },
            { ItemType.RusDrink1, 1 },
            { ItemType.RusDrink2, 1 },
            { ItemType.RusDrink3, 1 },
            { ItemType.YakDrink1, 1 },
            { ItemType.YakDrink2, 1 },
            { ItemType.YakDrink3, 1 },
            { ItemType.LcnDrink1, 1 },
            { ItemType.LcnDrink2, 1 },
            { ItemType.LcnDrink3, 1 },
            { ItemType.ArmDrink1, 1 },
            { ItemType.ArmDrink2, 1 },
            { ItemType.ArmDrink3, 1 },
            {ItemType.WaterBottle,  1},
            { ItemType.RepairBox, 1 },
            { ItemType.SmallHealthKit, 1 },
            { ItemType.Pistol, 1 },
            { ItemType.Combatpistol, 1 },
            { ItemType.Pistol50, 1 },
            { ItemType.Snspistol, 1 },
            { ItemType.Heavypistol, 1 },
            { ItemType.Vintagepistol, 1 },
            { ItemType.Marksmanpistol, 1 },
            { ItemType.Revolver, 1 },
            { ItemType.Appistol, 1 },
            { ItemType.Stungun, 1 },
            { ItemType.Flaregun, 1 },
            { ItemType.Doubleaction, 1 },
            { ItemType.Pistol_mk2, 1 },
            { ItemType.Snspistol_mk2, 1 },
            { ItemType.Revolver_mk2, 1 },
            { ItemType.Microsmg, 1 },
            { ItemType.Machinepistol, 1 },
            { ItemType.Smg, 1 },
            { ItemType.Assaultsmg, 1 },
            { ItemType.Combatpdw, 1 },
            { ItemType.Mg, 1 },
            { ItemType.Combatmg, 1 },
            { ItemType.Gusenberg, 1 },
            { ItemType.Minismg, 1 },
            { ItemType.Smg_mk2, 1 },
            { ItemType.Combatmg_mk2, 1 },
            { ItemType.Assaultrifle, 1 },
            { ItemType.Carbinerifle, 1 },
            { ItemType.Advancedrifle, 1 },
            { ItemType.Specialcarbine, 1 },
            { ItemType.Bullpuprifle, 1 },
            { ItemType.Compactrifle, 1 },
            { ItemType.Assaultrifle_mk2, 1 },
            { ItemType.Carbinerifle_mk2, 1 },
            { ItemType.Specialcarbine_mk2, 1 },
            { ItemType.Bullpuprifle_mk2, 1 },
            { ItemType.Sniperrifle, 1 },
            { ItemType.Heavysniper, 1 },
            { ItemType.Marksmanrifle, 1 },
            { ItemType.Heavysniper_mk2, 1},
            { ItemType.Marksmanrifle_mk2, 1 },
            { ItemType.Pumpshotgun, 1 },
            { ItemType.Sawnoffshotgun, 1 },
            { ItemType.Bullpupshotgun, 1 },
            { ItemType.Assaultshotgun, 1 },
            { ItemType.Musket, 1 },
            { ItemType.Heavyshotgun, 1 },
            { ItemType.Dbshotgun, 1 },
            { ItemType.Autoshotgun, 1 },
            { ItemType.Pumpshotgun_mk2, 1 },
            { ItemType.Rpg, 1 },
            { ItemType.Knife, 1 },
            { ItemType.Nightstick, 1 },
            { ItemType.Hammer, 1 },
            { ItemType.Bat, 1 },
            { ItemType.Crowbar, 1 },
            { ItemType.Golfclub, 1 },
            { ItemType.Bottle, 1 },
            { ItemType.Dagger, 1 },
            { ItemType.Hatchet, 1 },
            { ItemType.Knuckle, 1 },
            { ItemType.Machete, 1 },
            { ItemType.Flashlight, 1 },
            { ItemType.Switchblade, 1 },
            { ItemType.Poolcue, 1 },
            { ItemType.Wrench, 1 },
            { ItemType.Battleaxe, 1 },
            { ItemType.Rod, 1 },
            { ItemType.RodMK2, 1 },
            { ItemType.RodUpgrade, 1 },
            { ItemType.Bait, 1 },
            { ItemType.Naz, 1 },

            { ItemType.Kyndja, 1},
            { ItemType.Sig, 1},
            { ItemType.Omyl, 1},
            { ItemType.Nerka, 1},
            { ItemType.Forel, 1},
            { ItemType.Ship, 1},
            { ItemType.Lopatonos, 1},
            { ItemType.Osetr, 1},
            { ItemType.Semga, 1},
            { ItemType.Servyga, 1},
            { ItemType.Beluga, 1},
            { ItemType.Taimen, 1},
            { ItemType.Sterlyad, 1},
            { ItemType.Ydilshik, 1},
            { ItemType.Hailiod, 1},
            { ItemType.Keta, 1},
            { ItemType.Gorbysha, 1},
            //{ ItemType.GiveBox, 1 },
            //{ ItemType.Box1, 1 },
            //{ ItemType.Box2, 1 },
            //{ ItemType.Box3, 1 },
            //{ ItemType.Box4, 1 },
            //{ ItemType.CarLow, 1 },
            //{ ItemType.CarPremium, 1 },
            //{ ItemType.CarSport, 1 },
            { ItemType.PistolAmmo, 1 },
            { ItemType.RiflesAmmo, 1},
            { ItemType.ShotgunsAmmo, 1 },
            { ItemType.SMGAmmo, 1 },
            { ItemType.SniperAmmo, 1 },
            //{ ItemType.Pistol_Ammo_Box, 1 },
            //{ ItemType.Rifle_Ammo_Box, 1 },
            //{ ItemType.Smg_Ammo_Box, 1 },
            //{ ItemType.Shotgun_Ammo_Box, 1 },
            //{ ItemType.Sniper_Ammo_Box, 1 },

            { ItemType.Ceramicpistol, 1 },
            { ItemType.Militaryrifle, 1 },
            { ItemType.Combatshotgun, 1 },

            {ItemType.CasinoChips,1 },
            { ItemType.LSPDDrone, 1 },
            { ItemType.Drone, 1 },
            { ItemType.NumberPlate, 1 },
            { ItemType.SimCard, 1 },
            { ItemType.ProductBox, 1 },
            { ItemType.TrashBag, 1 },
            { ItemType.CocaLeaves, 1 },
            { ItemType.DrugBookMark, 1 },
            { ItemType.Weed, 1 },
            { ItemType.Subject, 1 },


            // [Modification]
            {ItemType.ADVANCEDRIFLE_CLIP_02, 1},
            {ItemType.ADVANCEDRIFLE_VARMOD_LUXE, 1},
            {ItemType.APPISTOL_CLIP_02, 1},
            {ItemType.APPISTOL_VARMOD_LUXE, 1},
            {ItemType.ASSAULTRIFLE_CLIP_02, 1},
            {ItemType.ASSAULTRIFLE_CLIP_03, 1},
            {ItemType.ASSAULTRIFLE_MK2_CAMO, 1},
            {ItemType.ASSAULTRIFLE_MK2_CAMO_02, 1},
            {ItemType.ASSAULTRIFLE_MK2_CAMO_03, 1},
            {ItemType.ASSAULTRIFLE_MK2_CAMO_04, 1},
            {ItemType.ASSAULTRIFLE_MK2_CAMO_05, 1},
            {ItemType.ASSAULTRIFLE_MK2_CAMO_06, 1},
            {ItemType.ASSAULTRIFLE_MK2_CAMO_07, 1},
            {ItemType.ASSAULTRIFLE_MK2_CAMO_08, 1},
            {ItemType.ASSAULTRIFLE_MK2_CAMO_09, 1},
            {ItemType.ASSAULTRIFLE_MK2_CAMO_10, 1},
            {ItemType.ASSAULTRIFLE_MK2_CAMO_IND_01, 1},
            {ItemType.ASSAULTRIFLE_MK2_CLIP_02, 1},
            {ItemType.ASSAULTRIFLE_MK2_CLIP_ARMORPIERCING, 1},
            {ItemType.ASSAULTRIFLE_MK2_CLIP_FMJ, 1},
            {ItemType.ASSAULTRIFLE_MK2_CLIP_INCENDIARY, 1},
            {ItemType.ASSAULTRIFLE_MK2_CLIP_TRACER, 1},
            {ItemType.ASSAULTRIFLE_VARMOD_LUXE, 1},
            {ItemType.ASSAULTSHOTGUN_CLIP_02, 1},
            {ItemType.ASSAULTSMG_CLIP_02, 1},
            {ItemType.ASSAULTSMG_VARMOD_LOWRIDER, 1},
            {ItemType.AT_AR_AFGRIP, 1},
            {ItemType.AT_AR_AFGRIP_02, 1},
            {ItemType.AT_AR_BARREL_02, 1},
            {ItemType.AT_AR_FLSH, 1},
            {ItemType.AT_AR_SUPP, 1},
            {ItemType.AT_AR_SUPP_02, 1},
            {ItemType.AT_BP_BARREL_02, 1},
            {ItemType.AT_CR_BARREL_02, 1},
            {ItemType.AT_MG_BARREL_02, 1},
            {ItemType.AT_MRFL_BARREL_02, 1},
            {ItemType.AT_MUZZLE_01, 1},
            {ItemType.AT_MUZZLE_02, 1},
            {ItemType.AT_MUZZLE_03, 1},
            {ItemType.AT_MUZZLE_04, 1},
            {ItemType.AT_MUZZLE_05, 1},
            {ItemType.AT_MUZZLE_06, 1},
            {ItemType.AT_MUZZLE_07, 1},
            {ItemType.AT_MUZZLE_08, 1},
            {ItemType.AT_MUZZLE_09, 1},
            {ItemType.AT_PI_COMP, 1},
            {ItemType.AT_PI_COMP_02, 1},
            {ItemType.AT_PI_COMP_03, 1},
            {ItemType.AT_PI_FLSH, 1},
            {ItemType.AT_PI_FLSH_02, 1},
            {ItemType.AT_PI_FLSH_03, 1},
            {ItemType.AT_PI_RAIL, 1},
            {ItemType.AT_PI_RAIL_02, 1},
            {ItemType.AT_PI_SUPP, 1},
            {ItemType.AT_PI_SUPP_02, 1},
            {ItemType.AT_SB_BARREL_02, 1},
            {ItemType.AT_SC_BARREL_02, 1},
            {ItemType.AT_SCOPE_LARGE, 1},
            {ItemType.AT_SCOPE_LARGE_FIXED_ZOOM, 1},
            {ItemType.AT_SCOPE_LARGE_FIXED_ZOOM_MK2, 1},
            {ItemType.AT_SCOPE_LARGE_MK2, 1},
            {ItemType.AT_SCOPE_MACRO, 1},
            {ItemType.AT_SCOPE_MACRO_02, 1},
            {ItemType.AT_SCOPE_MACRO_02_MK2, 1},
            {ItemType.AT_SCOPE_MACRO_02_SMG_MK2, 1},
            {ItemType.AT_SCOPE_MACRO_MK2, 1},
            {ItemType.AT_SCOPE_MAX, 1},
            {ItemType.AT_SCOPE_MEDIUM, 1},
            {ItemType.AT_SCOPE_MEDIUM_MK2, 1},
            {ItemType.AT_SCOPE_NV, 1},
            {ItemType.AT_SCOPE_SMALL, 1},
            {ItemType.AT_SCOPE_SMALL_02, 1},
            {ItemType.AT_SCOPE_SMALL_MK2, 1},
            {ItemType.AT_SCOPE_SMALL_SMG_MK2, 1},
            {ItemType.AT_SCOPE_THERMAL, 1},
            {ItemType.AT_SIGHTS, 1},
            {ItemType.AT_SIGHTS_SMG, 1},
            {ItemType.AT_SR_BARREL_02, 1},
            {ItemType.AT_SR_SUPP, 1},
            {ItemType.AT_SR_SUPP_03, 1},
            {ItemType.BULLPUPRIFLE_CLIP_02, 1},
            {ItemType.BULLPUPRIFLE_MK2_CAMO, 1},
            {ItemType.BULLPUPRIFLE_MK2_CAMO_02, 1},
            {ItemType.BULLPUPRIFLE_MK2_CAMO_03, 1},
            {ItemType.BULLPUPRIFLE_MK2_CAMO_04, 1},
            {ItemType.BULLPUPRIFLE_MK2_CAMO_05, 1},
            {ItemType.BULLPUPRIFLE_MK2_CAMO_06, 1},
            {ItemType.BULLPUPRIFLE_MK2_CAMO_07, 1},
            {ItemType.BULLPUPRIFLE_MK2_CAMO_08, 1},
            {ItemType.BULLPUPRIFLE_MK2_CAMO_09, 1},
            {ItemType.BULLPUPRIFLE_MK2_CAMO_10, 1},
            {ItemType.BULLPUPRIFLE_MK2_CAMO_IND_01, 1},
            {ItemType.BULLPUPRIFLE_MK2_CLIP_02, 1},
            {ItemType.BULLPUPRIFLE_MK2_CLIP_ARMORPIERCING, 1},
            {ItemType.BULLPUPRIFLE_MK2_CLIP_FMJ, 1},
            {ItemType.BULLPUPRIFLE_MK2_CLIP_INCENDIARY, 1},
            {ItemType.BULLPUPRIFLE_MK2_CLIP_TRACER, 1},
            {ItemType.BULLPUPRIFLE_VARMOD_LOW, 1},
            {ItemType.CARBINERIFLE_CLIP_02, 1},
            {ItemType.CARBINERIFLE_CLIP_03, 1},
            {ItemType.CARBINERIFLE_MK2_CAMO, 1},
            {ItemType.CARBINERIFLE_MK2_CAMO_02, 1},
            {ItemType.CARBINERIFLE_MK2_CAMO_03, 1},
            {ItemType.CARBINERIFLE_MK2_CAMO_04, 1},
            {ItemType.CARBINERIFLE_MK2_CAMO_05, 1},
            {ItemType.CARBINERIFLE_MK2_CAMO_06, 1},
            {ItemType.CARBINERIFLE_MK2_CAMO_07, 1},
            {ItemType.CARBINERIFLE_MK2_CAMO_08, 1},
            {ItemType.CARBINERIFLE_MK2_CAMO_09, 1},
            {ItemType.CARBINERIFLE_MK2_CAMO_10, 1},
            {ItemType.CARBINERIFLE_MK2_CAMO_IND_01, 1},
            {ItemType.CARBINERIFLE_MK2_CLIP_02, 1},
            {ItemType.CARBINERIFLE_MK2_CLIP_ARMORPIERCING, 1},
            {ItemType.CARBINERIFLE_MK2_CLIP_FMJ, 1},
            {ItemType.CARBINERIFLE_MK2_CLIP_INCENDIARY, 1},
            {ItemType.CARBINERIFLE_MK2_CLIP_TRACER, 1},
            {ItemType.CARBINERIFLE_VARMOD_LUXE, 1},
            {ItemType.CERAMICPISTOL_CLIP_02, 1},
            {ItemType.CERAMICPISTOL_SUPP, 1},
            {ItemType.COMBATMG_CLIP_02, 1},
            {ItemType.COMBATMG_MK2_CAMO, 1},
            {ItemType.COMBATMG_MK2_CAMO_02, 1},
            {ItemType.COMBATMG_MK2_CAMO_03, 1},
            {ItemType.COMBATMG_MK2_CAMO_04, 1},
            {ItemType.COMBATMG_MK2_CAMO_05, 1},
            {ItemType.COMBATMG_MK2_CAMO_06, 1},
            {ItemType.COMBATMG_MK2_CAMO_07, 1},
            {ItemType.COMBATMG_MK2_CAMO_08, 1},
            {ItemType.COMBATMG_MK2_CAMO_09, 1},
            {ItemType.COMBATMG_MK2_CAMO_10, 1},
            {ItemType.COMBATMG_MK2_CAMO_IND_01, 1},
            {ItemType.COMBATMG_MK2_CLIP_02, 1},
            {ItemType.COMBATMG_MK2_CLIP_ARMORPIERCING, 1},
            {ItemType.COMBATMG_MK2_CLIP_FMJ, 1},
            {ItemType.COMBATMG_MK2_CLIP_INCENDIARY, 1},
            {ItemType.COMBATMG_MK2_CLIP_TRACER, 1},
            {ItemType.COMBATMG_VARMOD_LOWRIDER, 1},
            {ItemType.COMBATPDW_CLIP_02, 1},
            {ItemType.COMBATPDW_CLIP_03, 1},
            {ItemType.COMBATPISTOL_CLIP_02, 1},
            {ItemType.COMBATPISTOL_VARMOD_LOWRIDER, 1},
            {ItemType.COMPACTRIFLE_CLIP_02, 1},
            {ItemType.COMPACTRIFLE_CLIP_03, 1},
            {ItemType.GUSENBERG_CLIP_02, 1},
            {ItemType.HEAVYPISTOL_CLIP_02, 1},
            {ItemType.HEAVYPISTOL_VARMOD_LUXE, 1},
            {ItemType.HEAVYSHOTGUN_CLIP_02, 1},
            {ItemType.HEAVYSHOTGUN_CLIP_03, 1},
            {ItemType.HEAVYSNIPER_MK2_CAMO, 1},
            {ItemType.HEAVYSNIPER_MK2_CAMO_02, 1},
            {ItemType.HEAVYSNIPER_MK2_CAMO_03, 1},
            {ItemType.HEAVYSNIPER_MK2_CAMO_04, 1},
            {ItemType.HEAVYSNIPER_MK2_CAMO_05, 1},
            {ItemType.HEAVYSNIPER_MK2_CAMO_06, 1},
            {ItemType.HEAVYSNIPER_MK2_CAMO_07, 1},
            {ItemType.HEAVYSNIPER_MK2_CAMO_08, 1},
            {ItemType.HEAVYSNIPER_MK2_CAMO_09, 1},
            {ItemType.HEAVYSNIPER_MK2_CAMO_10, 1},
            {ItemType.HEAVYSNIPER_MK2_CAMO_IND_01, 1},
            {ItemType.HEAVYSNIPER_MK2_CLIP_02, 1},
            {ItemType.HEAVYSNIPER_MK2_CLIP_ARMORPIERCING, 1},
            {ItemType.HEAVYSNIPER_MK2_CLIP_EXPLOSIVE, 1},
            {ItemType.HEAVYSNIPER_MK2_CLIP_FMJ, 1},
            {ItemType.HEAVYSNIPER_MK2_CLIP_INCENDIARY, 1},
            {ItemType.KNUCKLE_VARMOD_BALLAS, 1},
            {ItemType.KNUCKLE_VARMOD_DIAMOND, 1},
            {ItemType.KNUCKLE_VARMOD_DOLLAR, 1},
            {ItemType.KNUCKLE_VARMOD_HATE, 1},
            {ItemType.KNUCKLE_VARMOD_KING, 1},
            {ItemType.KNUCKLE_VARMOD_LOVE, 1},
            {ItemType.KNUCKLE_VARMOD_PIMP, 1},
            {ItemType.KNUCKLE_VARMOD_PLAYER, 1},
            {ItemType.KNUCKLE_VARMOD_VAGOS, 1},
            {ItemType.MACHINEPISTOL_CLIP_02, 1},
            {ItemType.MACHINEPISTOL_CLIP_03, 1},
            {ItemType.MARKSMANRIFLE_CLIP_02, 1},
            {ItemType.MARKSMANRIFLE_MK2_CAMO, 1},
            {ItemType.MARKSMANRIFLE_MK2_CAMO_02, 1},
            {ItemType.MARKSMANRIFLE_MK2_CAMO_03, 1},
            {ItemType.MARKSMANRIFLE_MK2_CAMO_04, 1},
            {ItemType.MARKSMANRIFLE_MK2_CAMO_05, 1},
            {ItemType.MARKSMANRIFLE_MK2_CAMO_06, 1},
            {ItemType.MARKSMANRIFLE_MK2_CAMO_07, 1},
            {ItemType.MARKSMANRIFLE_MK2_CAMO_08, 1},
            {ItemType.MARKSMANRIFLE_MK2_CAMO_09, 1},
            {ItemType.MARKSMANRIFLE_MK2_CAMO_10, 1},
            {ItemType.MARKSMANRIFLE_MK2_CAMO_IND_01, 1},
            {ItemType.MARKSMANRIFLE_MK2_CLIP_02, 1},
            {ItemType.MARKSMANRIFLE_MK2_CLIP_ARMORPIERCING, 1},
            {ItemType.MARKSMANRIFLE_MK2_CLIP_FMJ, 1},
            {ItemType.MARKSMANRIFLE_MK2_CLIP_INCENDIARY, 1},
            {ItemType.MARKSMANRIFLE_MK2_CLIP_TRACER, 1},
            {ItemType.MARKSMANRIFLE_VARMOD_LUXE, 1},
            {ItemType.MG_CLIP_02, 1},
            {ItemType.MG_VARMOD_LOWRIDER, 1},
            {ItemType.MICROSMG_CLIP_02, 1},
            {ItemType.MICROSMG_VARMOD_LUXE, 1},
            {ItemType.MILITARYRIFLE_CLIP_02, 1},
            {ItemType.MILITARYRIFLE_SIGHT_01, 1},
            {ItemType.MINISMG_CLIP_02, 1},
            {ItemType.PISTOL50_CLIP_02, 1},
            {ItemType.PISTOL50_VARMOD_LUXE, 1},
            {ItemType.PISTOL_CLIP_02, 1},
            {ItemType.PISTOL_MK2_CAMO, 1},
            {ItemType.PISTOL_MK2_CAMO_02, 1},
            {ItemType.PISTOL_MK2_CAMO_02_SLIDE, 1},
            {ItemType.PISTOL_MK2_CAMO_03, 1},
            {ItemType.PISTOL_MK2_CAMO_03_SLIDE, 1},
            {ItemType.PISTOL_MK2_CAMO_04, 1},
            {ItemType.PISTOL_MK2_CAMO_04_SLIDE, 1},
            {ItemType.PISTOL_MK2_CAMO_05, 1},
            {ItemType.PISTOL_MK2_CAMO_05_SLIDE, 1},
            {ItemType.PISTOL_MK2_CAMO_06, 1},
            {ItemType.PISTOL_MK2_CAMO_06_SLIDE, 1},
            {ItemType.PISTOL_MK2_CAMO_07, 1},
            {ItemType.PISTOL_MK2_CAMO_07_SLIDE, 1},
            {ItemType.PISTOL_MK2_CAMO_08, 1},
            {ItemType.PISTOL_MK2_CAMO_08_SLIDE, 1},
            {ItemType.PISTOL_MK2_CAMO_09, 1},
            {ItemType.PISTOL_MK2_CAMO_09_SLIDE, 1},
            {ItemType.PISTOL_MK2_CAMO_10, 1},
            {ItemType.PISTOL_MK2_CAMO_10_SLIDE, 1},
            {ItemType.PISTOL_MK2_CAMO_IND_01, 1},
            {ItemType.PISTOL_MK2_CAMO_IND_01_SLIDE, 1},
            {ItemType.PISTOL_MK2_CAMO_SLIDE, 1},
            {ItemType.PISTOL_MK2_CLIP_02, 1},
            {ItemType.PISTOL_MK2_CLIP_FMJ, 1},
            {ItemType.PISTOL_MK2_CLIP_HOLLOWPOINT, 1},
            {ItemType.PISTOL_MK2_CLIP_INCENDIARY, 1},
            {ItemType.PISTOL_MK2_CLIP_TRACER, 1},
            {ItemType.PISTOL_VARMOD_LUXE, 1},
            {ItemType.PUMPSHOTGUN_MK2_CAMO, 1},
            {ItemType.PUMPSHOTGUN_MK2_CAMO_02, 1},
            {ItemType.PUMPSHOTGUN_MK2_CAMO_03, 1},
            {ItemType.PUMPSHOTGUN_MK2_CAMO_04, 1},
            {ItemType.PUMPSHOTGUN_MK2_CAMO_05, 1},
            {ItemType.PUMPSHOTGUN_MK2_CAMO_06, 1},
            {ItemType.PUMPSHOTGUN_MK2_CAMO_07, 1},
            {ItemType.PUMPSHOTGUN_MK2_CAMO_08, 1},
            {ItemType.PUMPSHOTGUN_MK2_CAMO_09, 1},
            {ItemType.PUMPSHOTGUN_MK2_CAMO_10, 1},
            {ItemType.PUMPSHOTGUN_MK2_CAMO_IND_01, 1},
            {ItemType.PUMPSHOTGUN_MK2_CLIP_ARMORPIERCING, 1},
            {ItemType.PUMPSHOTGUN_MK2_CLIP_EXPLOSIVE, 1},
            {ItemType.PUMPSHOTGUN_MK2_CLIP_HOLLOWPOINT, 1},
            {ItemType.PUMPSHOTGUN_MK2_CLIP_INCENDIARY, 1},
            {ItemType.PUMPSHOTGUN_VARMOD_LOWRIDER, 1},
            {ItemType.RAYPISTOL_VARMOD_XMAS18, 1},
            {ItemType.REVOLVER_MK2_CAMO, 1},
            {ItemType.REVOLVER_MK2_CAMO_02, 1},
            {ItemType.REVOLVER_MK2_CAMO_03, 1},
            {ItemType.REVOLVER_MK2_CAMO_04, 1},
            {ItemType.REVOLVER_MK2_CAMO_05, 1},
            {ItemType.REVOLVER_MK2_CAMO_06, 1},
            {ItemType.REVOLVER_MK2_CAMO_07, 1},
            {ItemType.REVOLVER_MK2_CAMO_08, 1},
            {ItemType.REVOLVER_MK2_CAMO_09, 1},
            {ItemType.REVOLVER_MK2_CAMO_10, 1},
            {ItemType.REVOLVER_MK2_CAMO_IND_01, 1},
            {ItemType.REVOLVER_MK2_CLIP_FMJ, 1},
            {ItemType.REVOLVER_MK2_CLIP_HOLLOWPOINT, 1},
            {ItemType.REVOLVER_MK2_CLIP_INCENDIARY, 1},
            {ItemType.REVOLVER_MK2_CLIP_TRACER, 1},
            {ItemType.REVOLVER_VARMOD_BOSS, 1},
            {ItemType.REVOLVER_VARMOD_GOON, 1},
            {ItemType.SAWNOFFSHOTGUN_VARMOD_LUXE, 1},
            {ItemType.SMG_CLIP_02, 1},
            {ItemType.SMG_CLIP_03, 1},
            {ItemType.SMG_MK2_CAMO, 1},
            {ItemType.SMG_MK2_CAMO_02, 1},
            {ItemType.SMG_MK2_CAMO_03, 1},
            {ItemType.SMG_MK2_CAMO_04, 1},
            {ItemType.SMG_MK2_CAMO_05, 1},
            {ItemType.SMG_MK2_CAMO_06, 1},
            {ItemType.SMG_MK2_CAMO_07, 1},
            {ItemType.SMG_MK2_CAMO_08, 1},
            {ItemType.SMG_MK2_CAMO_09, 1},
            {ItemType.SMG_MK2_CAMO_10, 1},
            {ItemType.SMG_MK2_CAMO_IND_01, 1},
            {ItemType.SMG_MK2_CLIP_02, 1},
            {ItemType.SMG_MK2_CLIP_FMJ, 1},
            {ItemType.SMG_MK2_CLIP_HOLLOWPOINT, 1},
            {ItemType.SMG_MK2_CLIP_INCENDIARY, 1},
            {ItemType.SMG_MK2_CLIP_TRACER, 1},
            {ItemType.SMG_VARMOD_LUXE, 1},
            {ItemType.SNIPERRIFLE_VARMOD_LUXE, 1},
            {ItemType.SNSPISTOL_CLIP_02, 1},
            {ItemType.SNSPISTOL_MK2_CAMO, 1},
            {ItemType.SNSPISTOL_MK2_CAMO_02, 1},
            {ItemType.SNSPISTOL_MK2_CAMO_02_SLIDE, 1},
            {ItemType.SNSPISTOL_MK2_CAMO_03, 1},
            {ItemType.SNSPISTOL_MK2_CAMO_03_SLIDE, 1},
            {ItemType.SNSPISTOL_MK2_CAMO_04, 1},
            {ItemType.SNSPISTOL_MK2_CAMO_04_SLIDE, 1},
            {ItemType.SNSPISTOL_MK2_CAMO_05, 1},
            {ItemType.SNSPISTOL_MK2_CAMO_05_SLIDE, 1},
            {ItemType.SNSPISTOL_MK2_CAMO_06, 1},
            {ItemType.SNSPISTOL_MK2_CAMO_06_SLIDE, 1},
            {ItemType.SNSPISTOL_MK2_CAMO_07, 1},
            {ItemType.SNSPISTOL_MK2_CAMO_07_SLIDE, 1},
            {ItemType.SNSPISTOL_MK2_CAMO_08, 1},
            {ItemType.SNSPISTOL_MK2_CAMO_08_SLIDE, 1},
            {ItemType.SNSPISTOL_MK2_CAMO_09, 1},
            {ItemType.SNSPISTOL_MK2_CAMO_09_SLIDE, 1},
            {ItemType.SNSPISTOL_MK2_CAMO_10, 1},
            {ItemType.SNSPISTOL_MK2_CAMO_10_SLIDE, 1},
            {ItemType.SNSPISTOL_MK2_CAMO_IND_01, 1},
            {ItemType.SNSPISTOL_MK2_CAMO_IND_01_SLIDE, 1},
            {ItemType.SNSPISTOL_MK2_CAMO_SLIDE, 1},
            {ItemType.SNSPISTOL_MK2_CLIP_02, 1},
            {ItemType.SNSPISTOL_MK2_CLIP_FMJ, 1},
            {ItemType.SNSPISTOL_MK2_CLIP_HOLLOWPOINT, 1},
            {ItemType.SNSPISTOL_MK2_CLIP_INCENDIARY, 1},
            {ItemType.SNSPISTOL_MK2_CLIP_TRACER, 1},
            {ItemType.SNSPISTOL_VARMOD_LOWRIDER, 1},
            {ItemType.SPECIALCARBINE_CLIP_02, 1},
            {ItemType.SPECIALCARBINE_CLIP_03, 1},
            {ItemType.SPECIALCARBINE_MK2_CAMO, 1},
            {ItemType.SPECIALCARBINE_MK2_CAMO_02, 1},
            {ItemType.SPECIALCARBINE_MK2_CAMO_03, 1},
            {ItemType.SPECIALCARBINE_MK2_CAMO_04, 1},
            {ItemType.SPECIALCARBINE_MK2_CAMO_05, 1},
            {ItemType.SPECIALCARBINE_MK2_CAMO_06, 1},
            {ItemType.SPECIALCARBINE_MK2_CAMO_07, 1},
            {ItemType.SPECIALCARBINE_MK2_CAMO_08, 1},
            {ItemType.SPECIALCARBINE_MK2_CAMO_09, 1},
            {ItemType.SPECIALCARBINE_MK2_CAMO_10, 1},
            {ItemType.SPECIALCARBINE_MK2_CAMO_IND_01, 1},
            {ItemType.SPECIALCARBINE_MK2_CLIP_02, 1},
            {ItemType.SPECIALCARBINE_MK2_CLIP_ARMORPIERCING, 1},
            {ItemType.SPECIALCARBINE_MK2_CLIP_FMJ, 1},
            {ItemType.SPECIALCARBINE_MK2_CLIP_INCENDIARY, 1},
            {ItemType.SPECIALCARBINE_MK2_CLIP_TRACER, 1},
            {ItemType.SPECIALCARBINE_VARMOD_LOWRIDER, 1},
            {ItemType.SWITCHBLADE_VARMOD_VAR1, 1},
            {ItemType.SWITCHBLADE_VARMOD_VAR2, 1},
            {ItemType.VINTAGEPISTOL_CLIP_02, 1},

            {ItemType.DigScanner , 1},
            {ItemType.DigScanner_mk2 , 1},
            {ItemType.DigScanner_mk3 , 1},

            {ItemType.DigShovel , 1},
            {ItemType.DigShovel_mk2 , 1},
            {ItemType.DigShovel_mk3 , 1},

            {ItemType.CraftCap , 1},
            {ItemType.CraftOldCoin , 1},
            {ItemType.CraftShell , 1},
            {ItemType.CraftScrapMetal, 1},
            {ItemType.CraftCopperNugget,  1},
            {ItemType.CraftIronNugget,  1},
            {ItemType.CraftTinNugget,  1},

            {ItemType.CraftCopperWire, 1},
            {ItemType.CraftOldJewerly, 1},
            {ItemType.CraftGoldNugget, 1},
            {ItemType.CraftСollectibleCoin, 1},
            {ItemType.CraftAncientStatuette, 1},
            {ItemType.CraftGoldHorseShoe, 1},
            {ItemType.CraftRelic, 1},

            {ItemType.CraftIronPart, 1 },
            {ItemType.CraftCopperPart, 1 },
            {ItemType.CraftTinPart, 1 },
            {ItemType.CraftBronzePart, 1 },

            {ItemType.CraftWorkBench, 1 },
            {ItemType.CraftPercolator, 1 },
            {ItemType.CraftSmelter, 1 },
            {ItemType.CraftPartsCollector, 1 },
            {ItemType.CraftWorkBenchUpgrade, 1 },
            {ItemType.CraftWorkBenchUpgrade2, 1 },
        };
        public static Dictionary<ItemType, float> ItemsWeight = new Dictionary<ItemType, float>()
        {
            { ItemType.BagWithMoney, 5.0f },
            { ItemType.Bandage, 0.01f },
            { ItemType.Material, 0.01f },
            { ItemType.Drugs, 0.1f },
            { ItemType.BagWithDrill, 5.0f },
            { ItemType.Debug, 500.0f },
            { ItemType.HealthKit, 0.5f },
            { ItemType.GasCan, 2.0f },
            { ItemType.Сrisps, 0.05f },
            { ItemType.Beer, 0.05f },
            { ItemType.Pizza, 0.05f },
            { ItemType.Burger, 0.05f },
            { ItemType.HotDog, 0.05f },
            { ItemType.Sandwich, 0.05f },
            { ItemType.eCola, 0.05f },
            { ItemType.Sprunk, 0.05f },
            { ItemType.Lockpick, 0.5f },
            { ItemType.ArmyLockpick, 0.5f },
            { ItemType.Pocket, 0.4f },
            { ItemType.Cuffs, 0.3f },
            { ItemType.CarKey, 0.3f },
            { ItemType.Present, 0.3f },
            { ItemType.KeyRing, 0.3f },
            //{ ItemType.Radio, 0.3f },


            { ItemType.Mask, 0.1f },
            { ItemType.Gloves, 0.1f },
            { ItemType.Leg, 0.2f },
            { ItemType.Bag, 1.5f },
            { ItemType.Bag1, 1.5f },
            { ItemType.Feet, 0.1f},
            { ItemType.Jewelry, 0.1f },
            { ItemType.Undershit, 0.2f },
            { ItemType.BodyArmor, 1f },
            { ItemType.BodyArmorgov1, 1f },
            { ItemType.BodyArmorgov2, 1f },
            { ItemType.BodyArmorgov3, 1f },
            { ItemType.BodyArmorgov4, 1f },
            { ItemType.Unknown, 0.1f },
            { ItemType.Top, 0.2f },
            { ItemType.Hat, 0.1f },
            { ItemType.Glasses, 0.1f },
            { ItemType.Accessories, 0.1f },
            { ItemType.RusDrink1, 0.1f },
            { ItemType.RusDrink2, 0.1f },
            { ItemType.RusDrink3, 0.1f },
            { ItemType.YakDrink1, 0.1f },
            { ItemType.YakDrink2, 0.1f },
            { ItemType.YakDrink3, 0.1f },
            { ItemType.LcnDrink1, 0.1f },
            { ItemType.LcnDrink2, 0.1f },
            { ItemType.LcnDrink3, 0.1f },
            { ItemType.ArmDrink1, 0.1f },
            { ItemType.ArmDrink2, 0.1f },
            { ItemType.ArmDrink3, 0.1f },
            {ItemType.WaterBottle,  0.05f},
            { ItemType.RepairBox, 1 },
            { ItemType.SmallHealthKit, 0.2f },
            { ItemType.Fireextinguisher, 1.2f },

            { ItemType.Ball, 1.2f },
            { ItemType.Hominglauncher, 1.2f },
            { ItemType.Proximine, 1.2f },
            { ItemType.NavyRevolver, 1.2f },


            { ItemType.Pistol, 1.2f },
            { ItemType.Combatpistol, 1.2f },
            { ItemType.Pistol50, 1.2f },
            { ItemType.Snspistol, 1.2f },
            { ItemType.Heavypistol, 1.2f },
            { ItemType.Vintagepistol, 1.2f },
            { ItemType.Marksmanpistol, 1.2f },
            { ItemType.Revolver, 1.2f },
            { ItemType.Appistol, 1.2f },
            { ItemType.Stungun, 1.2f },
            { ItemType.Flaregun, 1.2f },
            { ItemType.Doubleaction, 1.2f },
            { ItemType.Pistol_mk2, 1.2f },
            { ItemType.Snspistol_mk2, 1.2f },
            { ItemType.Revolver_mk2, 1.2f },
            { ItemType.Microsmg, 1.8f },
            { ItemType.Machinepistol, 1.8f },
            { ItemType.Smg, 1.8f },
            { ItemType.Assaultsmg, 1.8f },
            { ItemType.Combatpdw, 1.8f },
            { ItemType.Mg, 1.8f },
            { ItemType.Combatmg, 1.8f },
            { ItemType.Gusenberg, 1.8f },
            { ItemType.Minismg, 1.8f },
            { ItemType.Smg_mk2, 1.8f },
            { ItemType.Combatmg_mk2, 2.8f },
            { ItemType.Assaultrifle, 2.8f },
            { ItemType.Carbinerifle, 2.8f },
            { ItemType.Advancedrifle, 2.8f },
            { ItemType.Specialcarbine, 2.8f },
            { ItemType.Bullpuprifle, 2.8f },
            { ItemType.Compactrifle, 2.8f },
            { ItemType.Assaultrifle_mk2, 2.8f },
            { ItemType.Carbinerifle_mk2, 2.8f },
            { ItemType.Specialcarbine_mk2, 2.8f },
            { ItemType.Bullpuprifle_mk2, 2.8f },
            { ItemType.Sniperrifle, 2.8f },
            { ItemType.Heavysniper, 2.8f },
            { ItemType.Marksmanrifle, 2.8f },
            { ItemType.Heavysniper_mk2, 2.8f },
            { ItemType.Marksmanrifle_mk2, 2.8f },
            { ItemType.Pumpshotgun, 3.8f },
            { ItemType.Sawnoffshotgun, 3.8f },
            { ItemType.Bullpupshotgun, 3.8f },
            { ItemType.Assaultshotgun, 3.8f },
            { ItemType.Musket, 3.8f },
            { ItemType.Heavyshotgun, 3.8f },
            { ItemType.Dbshotgun, 3.8f },
            { ItemType.Autoshotgun, 3.8f },
            { ItemType.Pumpshotgun_mk2, 3.8f },
            { ItemType.Rpg, 3.8f },
            { ItemType.Knife, 0.1f },
            { ItemType.Nightstick, 0.2f },
            { ItemType.Hammer, 0.5f },
            { ItemType.Bat, 0.5f },
            { ItemType.Crowbar, 0.5f },
            { ItemType.Golfclub, 0.5f },
            { ItemType.Bottle, 0.1f },
            { ItemType.Dagger, 0.1f },
            { ItemType.Hatchet, 0.2f },
            { ItemType.Knuckle, 0.2f },
            { ItemType.Machete, 0.2f },
            { ItemType.Flashlight, 0.1f },
            { ItemType.Switchblade, 0.5f },
            { ItemType.Poolcue, 0.5f },
            { ItemType.Wrench, 0.5f },
            { ItemType.Battleaxe, 1.5f },
            //{ ItemType.FishingRod1, 1.0f },
            //{ ItemType.Bait, 0.01f },
            //{ ItemType.FishingRod2, 1.0f },
            //{ ItemType.FishingRod3, 1.0f },
            //{ ItemType.Seld,0.01f },
            //{ ItemType.Okun,0.01f },
            //{ ItemType.Ugor,0.01f },
            //{ ItemType.Sterlyad,0.01f},
            //{ ItemType.Shuka,0.01f },
            //{ ItemType.Semga,0.01f },
            //{ ItemType.BlackAmur,0.01f },
            //{ ItemType.Skat,0.01f },
            //{ ItemType.Tunec,0.01f },
            //{ ItemType.Losos,0.01f },
            //{ ItemType.Osetr,0.01f },
            { ItemType.Rod, 1.0f },
            { ItemType.RodMK2, 1.0f },
            { ItemType.RodUpgrade, 1.0f },
            { ItemType.Bait, 0.01f },
            { ItemType.Naz, 0.01f },

            { ItemType.Kyndja, 0.35f},
            { ItemType.Sig, 0.4f},
            { ItemType.Omyl, 0.4f},
            { ItemType.Nerka, 0.5f},
            { ItemType.Forel, 0.551f},
            { ItemType.Ship, 0.55f},
            { ItemType.Lopatonos, 0.6f},
            { ItemType.Osetr, 0.7f},
            { ItemType.Semga, 0.7f},
            { ItemType.Servyga, 0.8f},
            { ItemType.Beluga, 0.8f},
            { ItemType.Taimen, 0.9f},
            { ItemType.Sterlyad, 0.9f},
            { ItemType.Ydilshik, 1.5f},
            { ItemType.Hailiod, 1.5f},
            { ItemType.Keta, 0.35f},
            { ItemType.Gorbysha, 0.35f},

            //{ ItemType.GiveBox, 0.01f },
            //{ ItemType.Box1, 0.01f },
            //{ ItemType.Box2, 0.01f },
            //{ ItemType.Box3, 0.01f },
            //{ ItemType.Box4, 0.01f },
            //{ ItemType.CarLow, 0.01f },
            //{ ItemType.CarPremium, 0.01f },
            //{ ItemType.CarSport, 0.01f },
            { ItemType.PistolAmmo, 0.006f },
            { ItemType.RiflesAmmo, 0.008f  },
            { ItemType.ShotgunsAmmo, 0.008f  },
            { ItemType.SMGAmmo, 0.007f  },
            { ItemType.SniperAmmo, 0.009f  },
            //{ ItemType.Pistol_Ammo_Box, 0.2f },
            //{ ItemType.Rifle_Ammo_Box, 0.2f },
            //{ ItemType.Smg_Ammo_Box, 0.2f },
            //{ ItemType.Shotgun_Ammo_Box, 0.2f },
            //{ ItemType.Sniper_Ammo_Box, 0.2f },

            { ItemType.Ceramicpistol, 1.2f },
            { ItemType.Militaryrifle, 2.8f },
            { ItemType.Combatshotgun, 3.8f },

            { ItemType.CasinoChips, 0.000f },
            { ItemType.Grenade, 0.3f },
            { ItemType.LSPDDrone, 0.1f },
            { ItemType.Drone, 0.1f },
            { ItemType.NumberPlate, 0.1f },
            { ItemType.SimCard, 0.1f },
            { ItemType.ProductBox, 15.0f },
            { ItemType.TrashBag, 1.0f },
            { ItemType.CocaLeaves, 0.1f },
            { ItemType.Cocaine, 0.1f },
            { ItemType.DrugBookMark, 0.1f },
            { ItemType.Weed, 0.1f },
            { ItemType.Subject, 4.0f },

            // [Modification]
            {ItemType.ADVANCEDRIFLE_CLIP_02, 0.3f},
            {ItemType.ADVANCEDRIFLE_VARMOD_LUXE, 0.3f},
            {ItemType.APPISTOL_CLIP_02, 0.3f},
            {ItemType.APPISTOL_VARMOD_LUXE, 0.3f},
            {ItemType.ASSAULTRIFLE_CLIP_02, 0.3f},
            {ItemType.ASSAULTRIFLE_CLIP_03, 0.3f},
            {ItemType.ASSAULTRIFLE_MK2_CAMO, 0.3f},
            {ItemType.ASSAULTRIFLE_MK2_CAMO_02, 0.3f},
            {ItemType.ASSAULTRIFLE_MK2_CAMO_03, 0.3f},
            {ItemType.ASSAULTRIFLE_MK2_CAMO_04, 0.3f},
            {ItemType.ASSAULTRIFLE_MK2_CAMO_05, 0.3f},
            {ItemType.ASSAULTRIFLE_MK2_CAMO_06, 0.3f},
            {ItemType.ASSAULTRIFLE_MK2_CAMO_07, 0.3f},
            {ItemType.ASSAULTRIFLE_MK2_CAMO_08, 0.3f},
            {ItemType.ASSAULTRIFLE_MK2_CAMO_09, 0.3f},
            {ItemType.ASSAULTRIFLE_MK2_CAMO_10, 0.3f},
            {ItemType.ASSAULTRIFLE_MK2_CAMO_IND_01, 0.3f},
            {ItemType.ASSAULTRIFLE_MK2_CLIP_02, 0.3f},
            {ItemType.ASSAULTRIFLE_MK2_CLIP_ARMORPIERCING, 0.3f},
            {ItemType.ASSAULTRIFLE_MK2_CLIP_FMJ, 0.3f},
            {ItemType.ASSAULTRIFLE_MK2_CLIP_INCENDIARY, 0.3f},
            {ItemType.ASSAULTRIFLE_MK2_CLIP_TRACER, 0.3f},
            {ItemType.ASSAULTRIFLE_VARMOD_LUXE, 0.3f},
            {ItemType.ASSAULTSHOTGUN_CLIP_02, 0.3f},
            {ItemType.ASSAULTSMG_CLIP_02, 0.3f},
            {ItemType.ASSAULTSMG_VARMOD_LOWRIDER, 0.3f},
            {ItemType.AT_AR_AFGRIP, 0.3f},
            {ItemType.AT_AR_AFGRIP_02, 0.3f},
            {ItemType.AT_AR_BARREL_02, 0.3f},
            {ItemType.AT_AR_FLSH, 0.3f},
            {ItemType.AT_AR_SUPP, 0.3f},
            {ItemType.AT_AR_SUPP_02, 0.3f},
            {ItemType.AT_BP_BARREL_02, 0.3f},
            {ItemType.AT_CR_BARREL_02, 0.3f},
            {ItemType.AT_MG_BARREL_02, 0.3f},
            {ItemType.AT_MRFL_BARREL_02, 0.3f},
            {ItemType.AT_MUZZLE_01, 0.3f},
            {ItemType.AT_MUZZLE_02, 0.3f},
            {ItemType.AT_MUZZLE_03, 0.3f},
            {ItemType.AT_MUZZLE_04, 0.3f},
            {ItemType.AT_MUZZLE_05, 0.3f},
            {ItemType.AT_MUZZLE_06, 0.3f},
            {ItemType.AT_MUZZLE_07, 0.3f},
            {ItemType.AT_MUZZLE_08, 0.3f},
            {ItemType.AT_MUZZLE_09, 0.3f},
            {ItemType.AT_PI_COMP, 0.3f},
            {ItemType.AT_PI_COMP_02, 0.3f},
            {ItemType.AT_PI_COMP_03, 0.3f},
            {ItemType.AT_PI_FLSH, 0.3f},
            {ItemType.AT_PI_FLSH_02, 0.3f},
            {ItemType.AT_PI_FLSH_03, 0.3f},
            {ItemType.AT_PI_RAIL, 0.3f},
            {ItemType.AT_PI_RAIL_02, 0.3f},
            {ItemType.AT_PI_SUPP, 0.3f},
            {ItemType.AT_PI_SUPP_02, 0.3f},
            {ItemType.AT_SB_BARREL_02, 0.3f},
            {ItemType.AT_SC_BARREL_02, 0.3f},
            {ItemType.AT_SCOPE_LARGE, 0.3f},
            {ItemType.AT_SCOPE_LARGE_FIXED_ZOOM, 0.3f},
            {ItemType.AT_SCOPE_LARGE_FIXED_ZOOM_MK2, 0.3f},
            {ItemType.AT_SCOPE_LARGE_MK2, 0.3f},
            {ItemType.AT_SCOPE_MACRO, 0.3f},
            {ItemType.AT_SCOPE_MACRO_02, 0.3f},
            {ItemType.AT_SCOPE_MACRO_02_MK2, 0.3f},
            {ItemType.AT_SCOPE_MACRO_02_SMG_MK2, 0.3f},
            {ItemType.AT_SCOPE_MACRO_MK2, 0.3f},
            {ItemType.AT_SCOPE_MAX, 0.3f},
            {ItemType.AT_SCOPE_MEDIUM, 0.3f},
            {ItemType.AT_SCOPE_MEDIUM_MK2, 0.3f},
            {ItemType.AT_SCOPE_NV, 0.3f},
            {ItemType.AT_SCOPE_SMALL, 0.3f},
            {ItemType.AT_SCOPE_SMALL_02, 0.3f},
            {ItemType.AT_SCOPE_SMALL_MK2, 0.3f},
            {ItemType.AT_SCOPE_SMALL_SMG_MK2, 0.3f},
            {ItemType.AT_SCOPE_THERMAL, 0.3f},
            {ItemType.AT_SIGHTS, 0.3f},
            {ItemType.AT_SIGHTS_SMG, 0.3f},
            {ItemType.AT_SR_BARREL_02, 0.3f},
            {ItemType.AT_SR_SUPP, 0.3f},
            {ItemType.AT_SR_SUPP_03, 0.3f},
            {ItemType.BULLPUPRIFLE_CLIP_02, 0.3f},
            {ItemType.BULLPUPRIFLE_MK2_CAMO, 0.3f},
            {ItemType.BULLPUPRIFLE_MK2_CAMO_02, 0.3f},
            {ItemType.BULLPUPRIFLE_MK2_CAMO_03, 0.3f},
            {ItemType.BULLPUPRIFLE_MK2_CAMO_04, 0.3f},
            {ItemType.BULLPUPRIFLE_MK2_CAMO_05, 0.3f},
            {ItemType.BULLPUPRIFLE_MK2_CAMO_06, 0.3f},
            {ItemType.BULLPUPRIFLE_MK2_CAMO_07, 0.3f},
            {ItemType.BULLPUPRIFLE_MK2_CAMO_08, 0.3f},
            {ItemType.BULLPUPRIFLE_MK2_CAMO_09, 0.3f},
            {ItemType.BULLPUPRIFLE_MK2_CAMO_10, 0.3f},
            {ItemType.BULLPUPRIFLE_MK2_CAMO_IND_01, 0.3f},
            {ItemType.BULLPUPRIFLE_MK2_CLIP_02, 0.3f},
            {ItemType.BULLPUPRIFLE_MK2_CLIP_ARMORPIERCING, 0.3f},
            {ItemType.BULLPUPRIFLE_MK2_CLIP_FMJ, 0.3f},
            {ItemType.BULLPUPRIFLE_MK2_CLIP_INCENDIARY, 0.3f},
            {ItemType.BULLPUPRIFLE_MK2_CLIP_TRACER, 0.3f},
            {ItemType.BULLPUPRIFLE_VARMOD_LOW, 0.3f},
            {ItemType.CARBINERIFLE_CLIP_02, 0.3f},
            {ItemType.CARBINERIFLE_CLIP_03, 0.3f},
            {ItemType.CARBINERIFLE_MK2_CAMO, 0.3f},
            {ItemType.CARBINERIFLE_MK2_CAMO_02, 0.3f},
            {ItemType.CARBINERIFLE_MK2_CAMO_03, 0.3f},
            {ItemType.CARBINERIFLE_MK2_CAMO_04, 0.3f},
            {ItemType.CARBINERIFLE_MK2_CAMO_05, 0.3f},
            {ItemType.CARBINERIFLE_MK2_CAMO_06, 0.3f},
            {ItemType.CARBINERIFLE_MK2_CAMO_07, 0.3f},
            {ItemType.CARBINERIFLE_MK2_CAMO_08, 0.3f},
            {ItemType.CARBINERIFLE_MK2_CAMO_09, 0.3f},
            {ItemType.CARBINERIFLE_MK2_CAMO_10, 0.3f},
            {ItemType.CARBINERIFLE_MK2_CAMO_IND_01, 0.3f},
            {ItemType.CARBINERIFLE_MK2_CLIP_02, 0.3f},
            {ItemType.CARBINERIFLE_MK2_CLIP_ARMORPIERCING, 0.3f},
            {ItemType.CARBINERIFLE_MK2_CLIP_FMJ, 0.3f},
            {ItemType.CARBINERIFLE_MK2_CLIP_INCENDIARY, 0.3f},
            {ItemType.CARBINERIFLE_MK2_CLIP_TRACER, 0.3f},
            {ItemType.CARBINERIFLE_VARMOD_LUXE, 0.3f},
            {ItemType.CERAMICPISTOL_CLIP_02, 0.3f},
            {ItemType.CERAMICPISTOL_SUPP, 0.3f},
            {ItemType.COMBATMG_CLIP_02, 0.3f},
            {ItemType.COMBATMG_MK2_CAMO, 0.3f},
            {ItemType.COMBATMG_MK2_CAMO_02, 0.3f},
            {ItemType.COMBATMG_MK2_CAMO_03, 0.3f},
            {ItemType.COMBATMG_MK2_CAMO_04, 0.3f},
            {ItemType.COMBATMG_MK2_CAMO_05, 0.3f},
            {ItemType.COMBATMG_MK2_CAMO_06, 0.3f},
            {ItemType.COMBATMG_MK2_CAMO_07, 0.3f},
            {ItemType.COMBATMG_MK2_CAMO_08, 0.3f},
            {ItemType.COMBATMG_MK2_CAMO_09, 0.3f},
            {ItemType.COMBATMG_MK2_CAMO_10, 0.3f},
            {ItemType.COMBATMG_MK2_CAMO_IND_01, 0.3f},
            {ItemType.COMBATMG_MK2_CLIP_02, 0.3f},
            {ItemType.COMBATMG_MK2_CLIP_ARMORPIERCING, 0.3f},
            {ItemType.COMBATMG_MK2_CLIP_FMJ, 0.3f},
            {ItemType.COMBATMG_MK2_CLIP_INCENDIARY, 0.3f},
            {ItemType.COMBATMG_MK2_CLIP_TRACER, 0.3f},
            {ItemType.COMBATMG_VARMOD_LOWRIDER, 0.3f},
            {ItemType.COMBATPDW_CLIP_02, 0.3f},
            {ItemType.COMBATPDW_CLIP_03, 0.3f},
            {ItemType.COMBATPISTOL_CLIP_02, 0.3f},
            {ItemType.COMBATPISTOL_VARMOD_LOWRIDER, 0.3f},
            {ItemType.COMPACTRIFLE_CLIP_02, 0.3f},
            {ItemType.COMPACTRIFLE_CLIP_03, 0.3f},
            {ItemType.GUSENBERG_CLIP_02, 0.3f},
            {ItemType.HEAVYPISTOL_CLIP_02, 0.3f},
            {ItemType.HEAVYPISTOL_VARMOD_LUXE, 0.3f},
            {ItemType.HEAVYSHOTGUN_CLIP_02, 0.3f},
            {ItemType.HEAVYSHOTGUN_CLIP_03, 0.3f},
            {ItemType.HEAVYSNIPER_MK2_CAMO, 0.3f},
            {ItemType.HEAVYSNIPER_MK2_CAMO_02, 0.3f},
            {ItemType.HEAVYSNIPER_MK2_CAMO_03, 0.3f},
            {ItemType.HEAVYSNIPER_MK2_CAMO_04, 0.3f},
            {ItemType.HEAVYSNIPER_MK2_CAMO_05, 0.3f},
            {ItemType.HEAVYSNIPER_MK2_CAMO_06, 0.3f},
            {ItemType.HEAVYSNIPER_MK2_CAMO_07, 0.3f},
            {ItemType.HEAVYSNIPER_MK2_CAMO_08, 0.3f},
            {ItemType.HEAVYSNIPER_MK2_CAMO_09, 0.3f},
            {ItemType.HEAVYSNIPER_MK2_CAMO_10, 0.3f},
            {ItemType.HEAVYSNIPER_MK2_CAMO_IND_01, 0.3f},
            {ItemType.HEAVYSNIPER_MK2_CLIP_02, 0.3f},
            {ItemType.HEAVYSNIPER_MK2_CLIP_ARMORPIERCING, 0.3f},
            {ItemType.HEAVYSNIPER_MK2_CLIP_EXPLOSIVE, 0.3f},
            {ItemType.HEAVYSNIPER_MK2_CLIP_FMJ, 0.3f},
            {ItemType.HEAVYSNIPER_MK2_CLIP_INCENDIARY, 0.3f},
            {ItemType.KNUCKLE_VARMOD_BALLAS, 0.3f},
            {ItemType.KNUCKLE_VARMOD_DIAMOND, 0.3f},
            {ItemType.KNUCKLE_VARMOD_DOLLAR, 0.3f},
            {ItemType.KNUCKLE_VARMOD_HATE, 0.3f},
            {ItemType.KNUCKLE_VARMOD_KING, 0.3f},
            {ItemType.KNUCKLE_VARMOD_LOVE, 0.3f},
            {ItemType.KNUCKLE_VARMOD_PIMP, 0.3f},
            {ItemType.KNUCKLE_VARMOD_PLAYER, 0.3f},
            {ItemType.KNUCKLE_VARMOD_VAGOS, 0.3f},
            {ItemType.MACHINEPISTOL_CLIP_02, 0.3f},
            {ItemType.MACHINEPISTOL_CLIP_03, 0.3f},
            {ItemType.MARKSMANRIFLE_CLIP_02, 0.3f},
            {ItemType.MARKSMANRIFLE_MK2_CAMO, 0.3f},
            {ItemType.MARKSMANRIFLE_MK2_CAMO_02, 0.3f},
            {ItemType.MARKSMANRIFLE_MK2_CAMO_03, 0.3f},
            {ItemType.MARKSMANRIFLE_MK2_CAMO_04, 0.3f},
            {ItemType.MARKSMANRIFLE_MK2_CAMO_05, 0.3f},
            {ItemType.MARKSMANRIFLE_MK2_CAMO_06, 0.3f},
            {ItemType.MARKSMANRIFLE_MK2_CAMO_07, 0.3f},
            {ItemType.MARKSMANRIFLE_MK2_CAMO_08, 0.3f},
            {ItemType.MARKSMANRIFLE_MK2_CAMO_09, 0.3f},
            {ItemType.MARKSMANRIFLE_MK2_CAMO_10, 0.3f},
            {ItemType.MARKSMANRIFLE_MK2_CAMO_IND_01, 0.3f},
            {ItemType.MARKSMANRIFLE_MK2_CLIP_02, 0.3f},
            {ItemType.MARKSMANRIFLE_MK2_CLIP_ARMORPIERCING, 0.3f},
            {ItemType.MARKSMANRIFLE_MK2_CLIP_FMJ, 0.3f},
            {ItemType.MARKSMANRIFLE_MK2_CLIP_INCENDIARY, 0.3f},
            {ItemType.MARKSMANRIFLE_MK2_CLIP_TRACER, 0.3f},
            {ItemType.MARKSMANRIFLE_VARMOD_LUXE, 0.3f},
            {ItemType.MG_CLIP_02, 0.3f},
            {ItemType.MG_VARMOD_LOWRIDER, 0.3f},
            {ItemType.MICROSMG_CLIP_02, 0.3f},
            {ItemType.MICROSMG_VARMOD_LUXE, 0.3f},
            {ItemType.MILITARYRIFLE_CLIP_02, 0.3f},
            {ItemType.MILITARYRIFLE_SIGHT_01, 0.3f},
            {ItemType.MINISMG_CLIP_02, 0.3f},
            {ItemType.PISTOL50_CLIP_02, 0.3f},
            {ItemType.PISTOL50_VARMOD_LUXE, 0.3f},
            {ItemType.PISTOL_CLIP_02, 0.3f},
            {ItemType.PISTOL_MK2_CAMO, 0.3f},
            {ItemType.PISTOL_MK2_CAMO_02, 0.3f},
            {ItemType.PISTOL_MK2_CAMO_02_SLIDE, 0.3f},
            {ItemType.PISTOL_MK2_CAMO_03, 0.3f},
            {ItemType.PISTOL_MK2_CAMO_03_SLIDE, 0.3f},
            {ItemType.PISTOL_MK2_CAMO_04, 0.3f},
            {ItemType.PISTOL_MK2_CAMO_04_SLIDE, 0.3f},
            {ItemType.PISTOL_MK2_CAMO_05, 0.3f},
            {ItemType.PISTOL_MK2_CAMO_05_SLIDE, 0.3f},
            {ItemType.PISTOL_MK2_CAMO_06, 0.3f},
            {ItemType.PISTOL_MK2_CAMO_06_SLIDE, 0.3f},
            {ItemType.PISTOL_MK2_CAMO_07, 0.3f},
            {ItemType.PISTOL_MK2_CAMO_07_SLIDE, 0.3f},
            {ItemType.PISTOL_MK2_CAMO_08, 0.3f},
            {ItemType.PISTOL_MK2_CAMO_08_SLIDE, 0.3f},
            {ItemType.PISTOL_MK2_CAMO_09, 0.3f},
            {ItemType.PISTOL_MK2_CAMO_09_SLIDE, 0.3f},
            {ItemType.PISTOL_MK2_CAMO_10, 0.3f},
            {ItemType.PISTOL_MK2_CAMO_10_SLIDE, 0.3f},
            {ItemType.PISTOL_MK2_CAMO_IND_01, 0.3f},
            {ItemType.PISTOL_MK2_CAMO_IND_01_SLIDE, 0.3f},
            {ItemType.PISTOL_MK2_CAMO_SLIDE, 0.3f},
            {ItemType.PISTOL_MK2_CLIP_02, 0.3f},
            {ItemType.PISTOL_MK2_CLIP_FMJ, 0.3f},
            {ItemType.PISTOL_MK2_CLIP_HOLLOWPOINT, 0.3f},
            {ItemType.PISTOL_MK2_CLIP_INCENDIARY, 0.3f},
            {ItemType.PISTOL_MK2_CLIP_TRACER, 0.3f},
            {ItemType.PISTOL_VARMOD_LUXE, 0.3f},
            {ItemType.PUMPSHOTGUN_MK2_CAMO, 0.3f},
            {ItemType.PUMPSHOTGUN_MK2_CAMO_02, 0.3f},
            {ItemType.PUMPSHOTGUN_MK2_CAMO_03, 0.3f},
            {ItemType.PUMPSHOTGUN_MK2_CAMO_04, 0.3f},
            {ItemType.PUMPSHOTGUN_MK2_CAMO_05, 0.3f},
            {ItemType.PUMPSHOTGUN_MK2_CAMO_06, 0.3f},
            {ItemType.PUMPSHOTGUN_MK2_CAMO_07, 0.3f},
            {ItemType.PUMPSHOTGUN_MK2_CAMO_08, 0.3f},
            {ItemType.PUMPSHOTGUN_MK2_CAMO_09, 0.3f},
            {ItemType.PUMPSHOTGUN_MK2_CAMO_10, 0.3f},
            {ItemType.PUMPSHOTGUN_MK2_CAMO_IND_01, 0.3f},
            {ItemType.PUMPSHOTGUN_MK2_CLIP_ARMORPIERCING, 0.3f},
            {ItemType.PUMPSHOTGUN_MK2_CLIP_EXPLOSIVE, 0.3f},
            {ItemType.PUMPSHOTGUN_MK2_CLIP_HOLLOWPOINT, 0.3f},
            {ItemType.PUMPSHOTGUN_MK2_CLIP_INCENDIARY, 0.3f},
            {ItemType.PUMPSHOTGUN_VARMOD_LOWRIDER, 0.3f},
            {ItemType.RAYPISTOL_VARMOD_XMAS18, 0.3f},
            {ItemType.REVOLVER_MK2_CAMO, 0.3f},
            {ItemType.REVOLVER_MK2_CAMO_02, 0.3f},
            {ItemType.REVOLVER_MK2_CAMO_03, 0.3f},
            {ItemType.REVOLVER_MK2_CAMO_04, 0.3f},
            {ItemType.REVOLVER_MK2_CAMO_05, 0.3f},
            {ItemType.REVOLVER_MK2_CAMO_06, 0.3f},
            {ItemType.REVOLVER_MK2_CAMO_07, 0.3f},
            {ItemType.REVOLVER_MK2_CAMO_08, 0.3f},
            {ItemType.REVOLVER_MK2_CAMO_09, 0.3f},
            {ItemType.REVOLVER_MK2_CAMO_10, 0.3f},
            {ItemType.REVOLVER_MK2_CAMO_IND_01, 0.3f},
            {ItemType.REVOLVER_MK2_CLIP_FMJ, 0.3f},
            {ItemType.REVOLVER_MK2_CLIP_HOLLOWPOINT, 0.3f},
            {ItemType.REVOLVER_MK2_CLIP_INCENDIARY, 0.3f},
            {ItemType.REVOLVER_MK2_CLIP_TRACER, 0.3f},
            {ItemType.REVOLVER_VARMOD_BOSS, 0.3f},
            {ItemType.REVOLVER_VARMOD_GOON, 0.3f},
            {ItemType.SAWNOFFSHOTGUN_VARMOD_LUXE, 0.3f},
            {ItemType.SMG_CLIP_02, 0.3f},
            {ItemType.SMG_CLIP_03, 0.3f},
            {ItemType.SMG_MK2_CAMO, 0.3f},
            {ItemType.SMG_MK2_CAMO_02, 0.3f},
            {ItemType.SMG_MK2_CAMO_03, 0.3f},
            {ItemType.SMG_MK2_CAMO_04, 0.3f},
            {ItemType.SMG_MK2_CAMO_05, 0.3f},
            {ItemType.SMG_MK2_CAMO_06, 0.3f},
            {ItemType.SMG_MK2_CAMO_07, 0.3f},
            {ItemType.SMG_MK2_CAMO_08, 0.3f},
            {ItemType.SMG_MK2_CAMO_09, 0.3f},
            {ItemType.SMG_MK2_CAMO_10, 0.3f},
            {ItemType.SMG_MK2_CAMO_IND_01, 0.3f},
            {ItemType.SMG_MK2_CLIP_02, 0.3f},
            {ItemType.SMG_MK2_CLIP_FMJ, 0.3f},
            {ItemType.SMG_MK2_CLIP_HOLLOWPOINT, 0.3f},
            {ItemType.SMG_MK2_CLIP_INCENDIARY, 0.3f},
            {ItemType.SMG_MK2_CLIP_TRACER, 0.3f},
            {ItemType.SMG_VARMOD_LUXE, 0.3f},
            {ItemType.SNIPERRIFLE_VARMOD_LUXE, 0.3f},
            {ItemType.SNSPISTOL_CLIP_02, 0.3f},
            {ItemType.SNSPISTOL_MK2_CAMO, 0.3f},
            {ItemType.SNSPISTOL_MK2_CAMO_02, 0.3f},
            {ItemType.SNSPISTOL_MK2_CAMO_02_SLIDE, 0.3f},
            {ItemType.SNSPISTOL_MK2_CAMO_03, 0.3f},
            {ItemType.SNSPISTOL_MK2_CAMO_03_SLIDE, 0.3f},
            {ItemType.SNSPISTOL_MK2_CAMO_04, 0.3f},
            {ItemType.SNSPISTOL_MK2_CAMO_04_SLIDE, 0.3f},
            {ItemType.SNSPISTOL_MK2_CAMO_05, 0.3f},
            {ItemType.SNSPISTOL_MK2_CAMO_05_SLIDE, 0.3f},
            {ItemType.SNSPISTOL_MK2_CAMO_06, 0.3f},
            {ItemType.SNSPISTOL_MK2_CAMO_06_SLIDE, 0.3f},
            {ItemType.SNSPISTOL_MK2_CAMO_07, 0.3f},
            {ItemType.SNSPISTOL_MK2_CAMO_07_SLIDE, 0.3f},
            {ItemType.SNSPISTOL_MK2_CAMO_08, 0.3f},
            {ItemType.SNSPISTOL_MK2_CAMO_08_SLIDE, 0.3f},
            {ItemType.SNSPISTOL_MK2_CAMO_09, 0.3f},
            {ItemType.SNSPISTOL_MK2_CAMO_09_SLIDE, 0.3f},
            {ItemType.SNSPISTOL_MK2_CAMO_10, 0.3f},
            {ItemType.SNSPISTOL_MK2_CAMO_10_SLIDE, 0.3f},
            {ItemType.SNSPISTOL_MK2_CAMO_IND_01, 0.3f},
            {ItemType.SNSPISTOL_MK2_CAMO_IND_01_SLIDE, 0.3f},
            {ItemType.SNSPISTOL_MK2_CAMO_SLIDE, 0.3f},
            {ItemType.SNSPISTOL_MK2_CLIP_02, 0.3f},
            {ItemType.SNSPISTOL_MK2_CLIP_FMJ, 0.3f},
            {ItemType.SNSPISTOL_MK2_CLIP_HOLLOWPOINT, 0.3f},
            {ItemType.SNSPISTOL_MK2_CLIP_INCENDIARY, 0.3f},
            {ItemType.SNSPISTOL_MK2_CLIP_TRACER, 0.3f},
            {ItemType.SNSPISTOL_VARMOD_LOWRIDER, 0.3f},
            {ItemType.SPECIALCARBINE_CLIP_02, 0.3f},
            {ItemType.SPECIALCARBINE_CLIP_03, 0.3f},
            {ItemType.SPECIALCARBINE_MK2_CAMO, 0.3f},
            {ItemType.SPECIALCARBINE_MK2_CAMO_02, 0.3f},
            {ItemType.SPECIALCARBINE_MK2_CAMO_03, 0.3f},
            {ItemType.SPECIALCARBINE_MK2_CAMO_04, 0.3f},
            {ItemType.SPECIALCARBINE_MK2_CAMO_05, 0.3f},
            {ItemType.SPECIALCARBINE_MK2_CAMO_06, 0.3f},
            {ItemType.SPECIALCARBINE_MK2_CAMO_07, 0.3f},
            {ItemType.SPECIALCARBINE_MK2_CAMO_08, 0.3f},
            {ItemType.SPECIALCARBINE_MK2_CAMO_09, 0.3f},
            {ItemType.SPECIALCARBINE_MK2_CAMO_10, 0.3f},
            {ItemType.SPECIALCARBINE_MK2_CAMO_IND_01, 0.3f},
            {ItemType.SPECIALCARBINE_MK2_CLIP_02, 0.3f},
            {ItemType.SPECIALCARBINE_MK2_CLIP_ARMORPIERCING, 0.3f},
            {ItemType.SPECIALCARBINE_MK2_CLIP_FMJ, 0.3f},
            {ItemType.SPECIALCARBINE_MK2_CLIP_INCENDIARY, 0.3f},
            {ItemType.SPECIALCARBINE_MK2_CLIP_TRACER, 0.3f},
            {ItemType.SPECIALCARBINE_VARMOD_LOWRIDER, 0.3f},
            {ItemType.SWITCHBLADE_VARMOD_VAR1, 0.3f},
            {ItemType.SWITCHBLADE_VARMOD_VAR2, 0.3f},
            {ItemType.VINTAGEPISTOL_CLIP_02, 0.3f},

            {ItemType.DigScanner , 2f},
            {ItemType.DigScanner_mk2 , 2},
            {ItemType.DigScanner_mk3 , 2},

            {ItemType.DigShovel , 2f},
            {ItemType.DigShovel_mk2 , 2f},
            {ItemType.DigShovel_mk3 , 2f},

            {ItemType.CraftCap , 0.01f},
            {ItemType.CraftOldCoin , 0.02f},
            {ItemType.CraftShell , 0.01f},
            {ItemType.CraftScrapMetal, 0.3f},
            {ItemType.CraftCopperNugget,  0.05f},
            {ItemType.CraftIronNugget,  0.05f},
            {ItemType.CraftTinNugget,  0.05f},

            {ItemType.CraftCopperWire, 0.1f},
            {ItemType.CraftOldJewerly, 0.05f},
            {ItemType.CraftGoldNugget, 0.05f},
            {ItemType.CraftСollectibleCoin, 0.02f},
            {ItemType.CraftAncientStatuette, 2f},
            {ItemType.CraftGoldHorseShoe, 3f},
            {ItemType.CraftRelic, 3f},

            {ItemType.CraftIronPart, 0.1f },
            {ItemType.CraftCopperPart, 0.1f },
            {ItemType.CraftTinPart, 0.1f },
            {ItemType.CraftBronzePart, 0.1f },

            {ItemType.CraftWorkBench, 1f },
            {ItemType.CraftPercolator, 1f },
            {ItemType.CraftSmelter, 1f },
            {ItemType.CraftPartsCollector, 1f },
            {ItemType.CraftWorkBenchUpgrade, 1f },
            {ItemType.CraftWorkBenchUpgrade2, 1f },
        };
        public static Dictionary<ItemType, string> ItemFtype = new Dictionary<ItemType, string>()
        {
            //Если FType одинаковый и Stack > 1, то их можно обьеденить. Должен быть УНИКАЛЬНЫЙ!!!!
            { ItemType.BagWithMoney, "material" },
            { ItemType.Material, "material" },
            { ItemType.Drugs, "drug" },
            { ItemType.BagWithDrill, "misc0" },
            { ItemType.Debug, "debug" },
            { ItemType.HealthKit, "medkit" },
            { ItemType.Bandage, "bandage" },
            { ItemType.GasCan, "petrol" },
            { ItemType.Сrisps, "Drink1" },
            { ItemType.Beer, "Drink2" },
            { ItemType.Pizza, "Drink3" },
            { ItemType.Burger, "Drink4" },
            { ItemType.HotDog, "Drink5" },
            { ItemType.Sandwich, "Drink6" },
            { ItemType.eCola, "Drink7" },
            { ItemType.Sprunk, "Drink8" },
            { ItemType.Lockpick, "misc1" },
            { ItemType.ArmyLockpick, "misc2" },
            { ItemType.Pocket, "misc3" },
            { ItemType.Cuffs, "misc4" },
            { ItemType.CarKey, "CarKey" },
            { ItemType.Present, "misc5" },
            { ItemType.KeyRing, "misc6" },
            //{ ItemType.Radio, "radio" },

            { ItemType.Mask, "Mask" },
            { ItemType.Gloves, "Gloves" },
            { ItemType.Leg, "Leg" },
            { ItemType.Bag, "Bag" },
            { ItemType.Bag1, "Bag" },
            { ItemType.Feet, "Feet" },
            { ItemType.Jewelry, "Jewelry" },
            { ItemType.Undershit, "Undershirt" },
            { ItemType.BodyArmor, "BodyArmor" },
            { ItemType.BodyArmorgov1, "BodyArmor" },
            { ItemType.BodyArmorgov2, "BodyArmor" },
            { ItemType.BodyArmorgov3, "BodyArmor" },
            { ItemType.BodyArmorgov4, "BodyArmor" },
            { ItemType.Unknown, "unknown" },
            { ItemType.Top, "Top" },
            { ItemType.Hat, "Hat" },
            { ItemType.Glasses, "Glasses" },
            { ItemType.Accessories, "Accessories" },
            { ItemType.RusDrink1, "vodka1" },
            { ItemType.RusDrink2, "vodka2" },
            { ItemType.RusDrink3, "vodka3" },
            { ItemType.YakDrink1, "vodka4" },
            { ItemType.YakDrink2, "vodka5" },
            { ItemType.YakDrink3, "vodka6" },
            { ItemType.LcnDrink1, "vodka7" },
            { ItemType.LcnDrink2, "vodka8" },
            { ItemType.LcnDrink3, "vodka9" },
            { ItemType.ArmDrink1, "vodka10" },
            { ItemType.ArmDrink2, "vodka11" },
            { ItemType.ArmDrink3, "vodka12" },
            {ItemType.WaterBottle,  "vodka13"},
            { ItemType.RepairBox, "repairbox" },
            { ItemType.SmallHealthKit, "smallmedkit" },
            { ItemType.Pistol, "gun" },
            { ItemType.Combatpistol, "gun" },
            { ItemType.Pistol50, "gun" },
            { ItemType.Snspistol, "gun" },
            { ItemType.Heavypistol, "gun" },
            { ItemType.Vintagepistol, "gun" },
            { ItemType.Marksmanpistol, "gun" },
            { ItemType.Revolver, "gun" },
            { ItemType.Appistol, "gun" },
            { ItemType.Stungun, "gun" },
            { ItemType.Flaregun, "gun" },
            { ItemType.Doubleaction, "gun" },
            { ItemType.Pistol_mk2, "gun" },
            { ItemType.Snspistol_mk2, "gun" },
            { ItemType.Revolver_mk2, "gun" },
            { ItemType.Microsmg, "gun" },
            { ItemType.Machinepistol, "gun"},
            { ItemType.Smg, "gun" },
            { ItemType.Assaultsmg, "gun" },
            { ItemType.Combatpdw, "gun" },
            { ItemType.Mg, "gun" },
            { ItemType.Combatmg, "gun" },
            { ItemType.Gusenberg, "gun" },
            { ItemType.Minismg, "gun" },
            { ItemType.Smg_mk2, "gun" },
            { ItemType.Combatmg_mk2, "gun" },
            { ItemType.Assaultrifle, "gun" },
            { ItemType.Carbinerifle, "gun" },
            { ItemType.Advancedrifle, "gun" },
            { ItemType.Specialcarbine, "gun" },
            { ItemType.Bullpuprifle, "gun" },
            { ItemType.Compactrifle, "gun" },
            { ItemType.Assaultrifle_mk2, "gun" },
            { ItemType.Carbinerifle_mk2, "gun" },
            { ItemType.Specialcarbine_mk2, "gun" },
            { ItemType.Bullpuprifle_mk2, "gun" },
            { ItemType.Sniperrifle,"gun" },
            { ItemType.Heavysniper, "gun" },
            { ItemType.Marksmanrifle, "gun" },
            { ItemType.Heavysniper_mk2, "gun" },
            { ItemType.Marksmanrifle_mk2, "gun" },
            { ItemType.Pumpshotgun, "gun" },
            { ItemType.Sawnoffshotgun, "gun" },
            { ItemType.Bullpupshotgun, "gun" },
            { ItemType.Assaultshotgun, "gun" },
            { ItemType.Musket, "gun" },
            { ItemType.Heavyshotgun, "gun" },
            { ItemType.Dbshotgun, "gun" },
            { ItemType.Autoshotgun, "gun" },
            { ItemType.Pumpshotgun_mk2, "gun" },
            { ItemType.Rpg, "gun" },
            { ItemType.Knife, "gun" },
            { ItemType.Nightstick, "gun" },
            { ItemType.Hammer, "gun" },
            { ItemType.Bat, "gun" },
            { ItemType.Crowbar, "gun" },
            { ItemType.Golfclub, "gun" },
            { ItemType.Bottle, "gun" },
            { ItemType.Dagger, "gun" },
            { ItemType.Hatchet, "gun" },
            { ItemType.Knuckle, "gun" },
            { ItemType.Machete, "gun" },
            { ItemType.Flashlight, "gun" },
            { ItemType.Switchblade, "gun" },
            { ItemType.Poolcue, "gun" },
            { ItemType.Wrench, "gun" },
            { ItemType.Battleaxe, "gun" },
            { ItemType.Rod, "gun" },
            { ItemType.RodMK2, "gun" },
            { ItemType.RodUpgrade, "gun" },
            { ItemType.Bait, "bait" },
            { ItemType.Naz, "bait2" },

            { ItemType.Kyndja, "fish1" },
            { ItemType.Sig, "fish2" },
            { ItemType.Omyl, "fish3" },
            { ItemType.Nerka, "fish4" },
            { ItemType.Forel, "fish5" },
            { ItemType.Ship, "fish6" },
            { ItemType.Lopatonos, "fish7" },
            { ItemType.Osetr, "fish8" },
            { ItemType.Semga, "fish9" },
            { ItemType.Servyga, "fish10" },
            { ItemType.Beluga, "fish11" },
            { ItemType.Taimen, "fish12" },
            { ItemType.Sterlyad, "fish13" },
            { ItemType.Ydilshik, "fish14" },
            { ItemType.Hailiod, "fish15" },
            { ItemType.Keta, "fish16" },
            { ItemType.Gorbysha, "fish17" },
            //{ ItemType.GiveBox, "donate" },
            //{ ItemType.Box1, "donate1" },
            //{ ItemType.Box2, "donate2" },
            //{ ItemType.Box3, "donate3" },
            //{ ItemType.Box4, "donate4" },
            //{ ItemType.CarLow, "donate66"},
            //{ ItemType.CarPremium, "donate77" },
            //{ ItemType.CarSport, "donate88" },
            { ItemType.PistolAmmo, "ammo1" },
            { ItemType.RiflesAmmo, "ammo2" },
            { ItemType.ShotgunsAmmo, "ammo3" },
            { ItemType.SMGAmmo, "ammo4" },
            { ItemType.SniperAmmo, "ammo5" },
            //{ ItemType.Pistol_Ammo_Box, "ammobox1" },
            //{ ItemType.Rifle_Ammo_Box, "ammobox2" },
            //{ ItemType.Smg_Ammo_Box, "ammobox3" },
            //{ ItemType.Shotgun_Ammo_Box, "ammobox4" },
            //{ ItemType.Sniper_Ammo_Box, "ammobox5" },

            { ItemType.Ceramicpistol, "gun" },
            { ItemType.Militaryrifle, "gun" },
            { ItemType.Combatshotgun, "gun" },


            { ItemType.CasinoChips, "casino" },
            { ItemType.Grenade, "guns" },
            { ItemType.LSPDDrone, "misc7" },
            { ItemType.Drone, "misc8" },
            { ItemType.NumberPlate, "misc9" },
            { ItemType.SimCard, "misc10" },
            { ItemType.ProductBox, "misc11" },
            { ItemType.TrashBag, "misc12" },
            { ItemType.CocaLeaves, "drugs" },
            { ItemType.Cocaine, "Cocaine" },
            { ItemType.DrugBookMark, "drugBookMark" },
            { ItemType.Weed, "weed" },
            { ItemType.Subject, "subject" },

            #region Modification
            // [Modification] Миша, Дима, Надо проверить некоторые позиции. Не понятно в какой слот они идут. У нас по сути есть только 5 слотов в модификациях. НО GRIP и BARRELL хз куда.
            {ItemType.ADVANCEDRIFLE_CLIP_02, "1"}, // Clip
            {ItemType.ADVANCEDRIFLE_VARMOD_LUXE, "5"}, // Texture
            {ItemType.APPISTOL_CLIP_02, "1"}, // Clip
            {ItemType.APPISTOL_VARMOD_LUXE, "5"}, // Texture
            {ItemType.ASSAULTRIFLE_CLIP_02, "1"}, // Clip
            {ItemType.ASSAULTRIFLE_CLIP_03, "1"}, // Clip
            {ItemType.ASSAULTRIFLE_MK2_CAMO, "5"}, // Texture
            {ItemType.ASSAULTRIFLE_MK2_CAMO_02, "5"}, // Texture
            {ItemType.ASSAULTRIFLE_MK2_CAMO_03, "5"}, // Texture
            {ItemType.ASSAULTRIFLE_MK2_CAMO_04, "5"}, // Texture
            {ItemType.ASSAULTRIFLE_MK2_CAMO_05, "5"}, // Texture
            {ItemType.ASSAULTRIFLE_MK2_CAMO_06, "5"}, // Texture
            {ItemType.ASSAULTRIFLE_MK2_CAMO_07, "5"}, // Texture
            {ItemType.ASSAULTRIFLE_MK2_CAMO_08, "5"}, // Texture
            {ItemType.ASSAULTRIFLE_MK2_CAMO_09, "5"}, // Texture
            {ItemType.ASSAULTRIFLE_MK2_CAMO_10, "5"}, // Texture
            {ItemType.ASSAULTRIFLE_MK2_CAMO_IND_01, "5"}, // Texture
            {ItemType.ASSAULTRIFLE_MK2_CLIP_02, "1"}, // Clip
            {ItemType.ASSAULTRIFLE_MK2_CLIP_ARMORPIERCING, "1"}, // Clip
            {ItemType.ASSAULTRIFLE_MK2_CLIP_FMJ, "1"}, // Clip
            {ItemType.ASSAULTRIFLE_MK2_CLIP_INCENDIARY, "1"}, // Clip
            {ItemType.ASSAULTRIFLE_MK2_CLIP_TRACER, "1"}, // Clip
            {ItemType.ASSAULTRIFLE_VARMOD_LUXE, "5"}, // Texture
            {ItemType.ASSAULTSHOTGUN_CLIP_02, "1"}, // Clip
            {ItemType.ASSAULTSMG_CLIP_02, "1"}, // Clip
            {ItemType.ASSAULTSMG_VARMOD_LOWRIDER, "5"}, // Texture
            {ItemType.AT_AR_AFGRIP, "6"}, // Grip? [Modification NEED CHECK SLOT NUMBER]
            {ItemType.AT_AR_AFGRIP_02, "6"}, // Grip? [Modification NEED CHECK SLOT NUMBER]
            {ItemType.AT_AR_BARREL_02, "4"}, // Barrel / Compensator? [Modification Need CHECK SLOT NUMBER]
            {ItemType.AT_AR_FLSH, "2"}, // FlashLight
            {ItemType.AT_AR_SUPP, "4"}, // Suppressor
            {ItemType.AT_AR_SUPP_02, "4"}, // Suppressor
            {ItemType.AT_BP_BARREL_02, "4"}, // Barrel / Compensator? [Modification Need CHECK SLOT NUMBER]
            {ItemType.AT_CR_BARREL_02, "4"}, // Barrel / Compensator? [Modification Need CHECK SLOT NUMBER]
            {ItemType.AT_MG_BARREL_02, "4"}, // Barrel / Compensator? [Modification Need CHECK SLOT NUMBER]
            {ItemType.AT_MRFL_BARREL_02, "4"}, // Barrel / Compensator? [Modification Need CHECK SLOT NUMBER]
            {ItemType.AT_MUZZLE_01, "4"}, // Suppressor
            {ItemType.AT_MUZZLE_02, "4"}, // Suppressor
            {ItemType.AT_MUZZLE_03, "4"}, // Suppressor
            {ItemType.AT_MUZZLE_04, "4"}, // Suppressor
            {ItemType.AT_MUZZLE_05, "4"}, // Suppressor
            {ItemType.AT_MUZZLE_06, "4"}, // Suppressor
            {ItemType.AT_MUZZLE_07, "4"}, // Suppressor
            {ItemType.AT_MUZZLE_08, "4"}, // Suppressor
            {ItemType.AT_MUZZLE_09, "4"}, // Suppressor
            {ItemType.AT_PI_COMP, "4"}, // Suppressor / Compensator
            {ItemType.AT_PI_COMP_02, "4"}, // Suppressor / Compensator
            {ItemType.AT_PI_COMP_03, "4"}, // Suppressor / Compensator
            {ItemType.AT_PI_FLSH, "2"}, // FlashLight
            {ItemType.AT_PI_FLSH_02, "2"}, // FlashLight
            {ItemType.AT_PI_FLSH_03, "2"}, // FlashLight
            {ItemType.AT_PI_RAIL, "3"}, // Scope / Mounted Scope
            {ItemType.AT_PI_RAIL_02, "3"}, // Scope / Mounted Scope
            {ItemType.AT_PI_SUPP, "4"}, // Suppressor
            {ItemType.AT_PI_SUPP_02, "4"}, // Suppressor
            {ItemType.AT_SB_BARREL_02, "4"}, // Barrel / Compensator? [Modification Need CHECK SLOT NUMBER]
            {ItemType.AT_SC_BARREL_02, "4"}, // Barrel / Compensator? [Modification Need CHECK SLOT NUMBER]
            {ItemType.AT_SCOPE_LARGE, "3"}, // Scope
            {ItemType.AT_SCOPE_LARGE_FIXED_ZOOM, "3"}, // Scope
            {ItemType.AT_SCOPE_LARGE_FIXED_ZOOM_MK2, "3"}, // Scope
            {ItemType.AT_SCOPE_LARGE_MK2, "3"}, // Scope
            {ItemType.AT_SCOPE_MACRO, "3"}, // Scope
            {ItemType.AT_SCOPE_MACRO_02, "3"}, // Scope
            {ItemType.AT_SCOPE_MACRO_02_MK2, "3"}, // Scope
            {ItemType.AT_SCOPE_MACRO_02_SMG_MK2, "3"}, // Scope
            {ItemType.AT_SCOPE_MACRO_MK2, "3"}, // Scope
            {ItemType.AT_SCOPE_MAX, "3"}, // Scope
            {ItemType.AT_SCOPE_MEDIUM, "3"}, // Scope
            {ItemType.AT_SCOPE_MEDIUM_MK2, "3"}, // Scope
            {ItemType.AT_SCOPE_NV, "3"}, // Scope
            {ItemType.AT_SCOPE_SMALL, "3"}, // Scope
            {ItemType.AT_SCOPE_SMALL_02, "3"}, // Scope
            {ItemType.AT_SCOPE_SMALL_MK2, "3"}, // Scope
            {ItemType.AT_SCOPE_SMALL_SMG_MK2, "3"}, // Scope
            {ItemType.AT_SCOPE_THERMAL, "3"}, // Scope
            {ItemType.AT_SIGHTS, "3"}, // Scope / Sights
            {ItemType.AT_SIGHTS_SMG, "3"}, // Scope / Sights
            {ItemType.AT_SR_BARREL_02, "4"}, // Barrel / Compensator? [Modification Need CHECK SLOT NUMBER]
            {ItemType.AT_SR_SUPP, "4"}, // Suppressor
            {ItemType.AT_SR_SUPP_03, "4"}, // Suppressor
            {ItemType.BULLPUPRIFLE_CLIP_02, "1"}, // Clip
            {ItemType.BULLPUPRIFLE_MK2_CAMO, "5"}, // Texture
            {ItemType.BULLPUPRIFLE_MK2_CAMO_02, "5"}, // Texture
            {ItemType.BULLPUPRIFLE_MK2_CAMO_03, "5"}, // Texture
            {ItemType.BULLPUPRIFLE_MK2_CAMO_04, "5"}, // Texture
            {ItemType.BULLPUPRIFLE_MK2_CAMO_05, "5"}, // Texture
            {ItemType.BULLPUPRIFLE_MK2_CAMO_06, "5"}, // Texture
            {ItemType.BULLPUPRIFLE_MK2_CAMO_07, "5"}, // Texture
            {ItemType.BULLPUPRIFLE_MK2_CAMO_08, "5"}, // Texture
            {ItemType.BULLPUPRIFLE_MK2_CAMO_09, "5"}, // Texture
            {ItemType.BULLPUPRIFLE_MK2_CAMO_10, "5"}, // Texture
            {ItemType.BULLPUPRIFLE_MK2_CAMO_IND_01, "5"}, // Texture
            {ItemType.BULLPUPRIFLE_MK2_CLIP_02, "1"}, // Clip
            {ItemType.BULLPUPRIFLE_MK2_CLIP_ARMORPIERCING, "1"}, // Clip
            {ItemType.BULLPUPRIFLE_MK2_CLIP_FMJ, "1"}, // Clip
            {ItemType.BULLPUPRIFLE_MK2_CLIP_INCENDIARY, "1"}, // Clip
            {ItemType.BULLPUPRIFLE_MK2_CLIP_TRACER, "1"}, // Clip
            {ItemType.BULLPUPRIFLE_VARMOD_LOW, "5"}, // Texture
            {ItemType.CARBINERIFLE_CLIP_02, "1"}, // Clip
            {ItemType.CARBINERIFLE_CLIP_03, "1"}, // Clip
            {ItemType.CARBINERIFLE_MK2_CAMO, "5"}, // Texture
            {ItemType.CARBINERIFLE_MK2_CAMO_02, "5"}, // Texture
            {ItemType.CARBINERIFLE_MK2_CAMO_03, "5"}, // Texture
            {ItemType.CARBINERIFLE_MK2_CAMO_04, "5"}, // Texture
            {ItemType.CARBINERIFLE_MK2_CAMO_05, "5"}, // Texture
            {ItemType.CARBINERIFLE_MK2_CAMO_06, "5"}, // Texture
            {ItemType.CARBINERIFLE_MK2_CAMO_07, "5"}, // Texture
            {ItemType.CARBINERIFLE_MK2_CAMO_08, "5"}, // Texture
            {ItemType.CARBINERIFLE_MK2_CAMO_09, "5"}, // Texture
            {ItemType.CARBINERIFLE_MK2_CAMO_10, "5"}, // Texture
            {ItemType.CARBINERIFLE_MK2_CAMO_IND_01, "5"}, // Texture
            {ItemType.CARBINERIFLE_MK2_CLIP_02, "1"}, // Clip
            {ItemType.CARBINERIFLE_MK2_CLIP_ARMORPIERCING, "1"}, // Clip
            {ItemType.CARBINERIFLE_MK2_CLIP_FMJ, "1"}, // Clip
            {ItemType.CARBINERIFLE_MK2_CLIP_INCENDIARY, "1"}, // Clip
            {ItemType.CARBINERIFLE_MK2_CLIP_TRACER, "1"}, // Clip
            {ItemType.CARBINERIFLE_VARMOD_LUXE, "5"}, // Texture
            {ItemType.CERAMICPISTOL_CLIP_02, "1"}, // Clip
            {ItemType.CERAMICPISTOL_SUPP, "4"}, // Suppressor
            {ItemType.COMBATMG_CLIP_02, "1"}, // Clip
            {ItemType.COMBATMG_MK2_CAMO, "5"}, // Texture
            {ItemType.COMBATMG_MK2_CAMO_02, "5"}, // Texture
            {ItemType.COMBATMG_MK2_CAMO_03, "5"}, // Texture
            {ItemType.COMBATMG_MK2_CAMO_04, "5"}, // Texture
            {ItemType.COMBATMG_MK2_CAMO_05, "5"}, // Texture
            {ItemType.COMBATMG_MK2_CAMO_06, "5"}, // Texture
            {ItemType.COMBATMG_MK2_CAMO_07, "5"}, // Texture
            {ItemType.COMBATMG_MK2_CAMO_08, "5"}, // Texture
            {ItemType.COMBATMG_MK2_CAMO_09, "5"}, // Texture
            {ItemType.COMBATMG_MK2_CAMO_10, "5"}, // Texture
            {ItemType.COMBATMG_MK2_CAMO_IND_01, "5"}, // Texture
            {ItemType.COMBATMG_MK2_CLIP_02, "1"}, // Clip
            {ItemType.COMBATMG_MK2_CLIP_ARMORPIERCING, "1"}, // Clip
            {ItemType.COMBATMG_MK2_CLIP_FMJ, "1"}, // Clip
            {ItemType.COMBATMG_MK2_CLIP_INCENDIARY, "1"}, // Clip
            {ItemType.COMBATMG_MK2_CLIP_TRACER, "1"}, // Clip
            {ItemType.COMBATMG_VARMOD_LOWRIDER, "5"}, // Texture
            {ItemType.COMBATPDW_CLIP_02, "1"}, // Clip
            {ItemType.COMBATPDW_CLIP_03, "1"}, // Clip
            {ItemType.COMBATPISTOL_CLIP_02, "1"}, // Clip
            {ItemType.COMBATPISTOL_VARMOD_LOWRIDER, "5"}, // Texture
            {ItemType.COMPACTRIFLE_CLIP_02, "1"}, // Clip
            {ItemType.COMPACTRIFLE_CLIP_03, "1"}, // Clip
            {ItemType.GUSENBERG_CLIP_02, "1"}, // Clip
            {ItemType.HEAVYPISTOL_CLIP_02, "1"}, // Clip
            {ItemType.HEAVYPISTOL_VARMOD_LUXE, "5"}, // Texture
            {ItemType.HEAVYSHOTGUN_CLIP_02, "1"}, // Clip
            {ItemType.HEAVYSHOTGUN_CLIP_03, "1"}, // Clip
            {ItemType.HEAVYSNIPER_MK2_CAMO, "5"}, // Texture
            {ItemType.HEAVYSNIPER_MK2_CAMO_02, "5"}, // Texture
            {ItemType.HEAVYSNIPER_MK2_CAMO_03, "5"}, // Texture
            {ItemType.HEAVYSNIPER_MK2_CAMO_04, "5"}, // Texture
            {ItemType.HEAVYSNIPER_MK2_CAMO_05, "5"}, // Texture
            {ItemType.HEAVYSNIPER_MK2_CAMO_06, "5"}, // Texture
            {ItemType.HEAVYSNIPER_MK2_CAMO_07, "5"}, // Texture
            {ItemType.HEAVYSNIPER_MK2_CAMO_08, "5"}, // Texture
            {ItemType.HEAVYSNIPER_MK2_CAMO_09, "5"}, // Texture
            {ItemType.HEAVYSNIPER_MK2_CAMO_10, "5"}, // Texture
            {ItemType.HEAVYSNIPER_MK2_CAMO_IND_01, "5"}, // Texture
            {ItemType.HEAVYSNIPER_MK2_CLIP_02, "1"}, // Clip
            {ItemType.HEAVYSNIPER_MK2_CLIP_ARMORPIERCING, "1"}, // Clip
            {ItemType.HEAVYSNIPER_MK2_CLIP_EXPLOSIVE, "1"}, // Clip
            {ItemType.HEAVYSNIPER_MK2_CLIP_FMJ, "1"}, // Clip
            {ItemType.HEAVYSNIPER_MK2_CLIP_INCENDIARY, "1"}, // Clip
            {ItemType.KNUCKLE_VARMOD_BALLAS, "5"}, // Texture
            {ItemType.KNUCKLE_VARMOD_DIAMOND, "5"}, // Texture
            {ItemType.KNUCKLE_VARMOD_DOLLAR, "5"}, // Texture
            {ItemType.KNUCKLE_VARMOD_HATE, "5"}, // Texture
            {ItemType.KNUCKLE_VARMOD_KING, "5"}, // Texture
            {ItemType.KNUCKLE_VARMOD_LOVE, "5"}, // Texture
            {ItemType.KNUCKLE_VARMOD_PIMP, "5"}, // Texture
            {ItemType.KNUCKLE_VARMOD_PLAYER, "5"}, // Texture
            {ItemType.KNUCKLE_VARMOD_VAGOS, "5"}, // Texture
            {ItemType.MACHINEPISTOL_CLIP_02, "1"}, // Clip
            {ItemType.MACHINEPISTOL_CLIP_03, "1"}, // Clip
            {ItemType.MARKSMANRIFLE_CLIP_02, "1"}, // Clip
            {ItemType.MARKSMANRIFLE_MK2_CAMO, "5"}, // Texture
            {ItemType.MARKSMANRIFLE_MK2_CAMO_02, "5"}, // Texture
            {ItemType.MARKSMANRIFLE_MK2_CAMO_03, "5"}, // Texture
            {ItemType.MARKSMANRIFLE_MK2_CAMO_04, "5"}, // Texture
            {ItemType.MARKSMANRIFLE_MK2_CAMO_05, "5"}, // Texture
            {ItemType.MARKSMANRIFLE_MK2_CAMO_06, "5"}, // Texture
            {ItemType.MARKSMANRIFLE_MK2_CAMO_07, "5"}, // Texture
            {ItemType.MARKSMANRIFLE_MK2_CAMO_08, "5"}, // Texture
            {ItemType.MARKSMANRIFLE_MK2_CAMO_09, "5"}, // Texture
            {ItemType.MARKSMANRIFLE_MK2_CAMO_10, "5"}, // Texture
            {ItemType.MARKSMANRIFLE_MK2_CAMO_IND_01, "5"}, // Texture
            {ItemType.MARKSMANRIFLE_MK2_CLIP_02, "1"}, // Clip
            {ItemType.MARKSMANRIFLE_MK2_CLIP_ARMORPIERCING, "1"}, // Clip
            {ItemType.MARKSMANRIFLE_MK2_CLIP_FMJ, "1"}, // Clip
            {ItemType.MARKSMANRIFLE_MK2_CLIP_INCENDIARY, "1"}, // Clip
            {ItemType.MARKSMANRIFLE_MK2_CLIP_TRACER, "1"}, // Clip
            {ItemType.MARKSMANRIFLE_VARMOD_LUXE, "5"}, // Texture
            {ItemType.MG_CLIP_02, "1"}, // Clip
            {ItemType.MG_VARMOD_LOWRIDER, "5"}, // Texture
            {ItemType.MICROSMG_CLIP_02, "1"}, // Clip
            {ItemType.MICROSMG_VARMOD_LUXE, "5"}, // Texture
            {ItemType.MILITARYRIFLE_CLIP_02, "1"}, // Clip
            {ItemType.MILITARYRIFLE_SIGHT_01, "1"},
            {ItemType.MINISMG_CLIP_02, "1"}, // Clip
            {ItemType.PISTOL50_CLIP_02, "1"}, // Clip
            {ItemType.PISTOL50_VARMOD_LUXE, "5"}, // Texture
            {ItemType.PISTOL_CLIP_02, "1"}, // Clip
            {ItemType.PISTOL_MK2_CAMO, "5"}, // Texture
            {ItemType.PISTOL_MK2_CAMO_02, "5"}, // Texture
            {ItemType.PISTOL_MK2_CAMO_02_SLIDE, "5"}, // Texture
            {ItemType.PISTOL_MK2_CAMO_03, "5"}, // Texture
            {ItemType.PISTOL_MK2_CAMO_03_SLIDE, "5"}, // Texture
            {ItemType.PISTOL_MK2_CAMO_04, "5"}, // Texture
            {ItemType.PISTOL_MK2_CAMO_04_SLIDE, "5"}, // Texture
            {ItemType.PISTOL_MK2_CAMO_05, "5"}, // Texture
            {ItemType.PISTOL_MK2_CAMO_05_SLIDE, "5"}, // Texture
            {ItemType.PISTOL_MK2_CAMO_06, "5"}, // Texture
            {ItemType.PISTOL_MK2_CAMO_06_SLIDE, "5"}, // Texture
            {ItemType.PISTOL_MK2_CAMO_07, "5"}, // Texture
            {ItemType.PISTOL_MK2_CAMO_07_SLIDE, "5"}, // Texture
            {ItemType.PISTOL_MK2_CAMO_08, "5"}, // Texture
            {ItemType.PISTOL_MK2_CAMO_08_SLIDE, "5"}, // Texture
            {ItemType.PISTOL_MK2_CAMO_09, "5"}, // Texture
            {ItemType.PISTOL_MK2_CAMO_09_SLIDE, "5"}, // Texture
            {ItemType.PISTOL_MK2_CAMO_10, "5"}, // Texture
            {ItemType.PISTOL_MK2_CAMO_10_SLIDE, "5"}, // Texture
            {ItemType.PISTOL_MK2_CAMO_IND_01, "5"}, // Texture
            {ItemType.PISTOL_MK2_CAMO_IND_01_SLIDE, "5"}, // Texture
            {ItemType.PISTOL_MK2_CAMO_SLIDE, "5"}, // Texture
            {ItemType.PISTOL_MK2_CLIP_02, "1"}, // Clip
            {ItemType.PISTOL_MK2_CLIP_FMJ, "1"}, // Clip
            {ItemType.PISTOL_MK2_CLIP_HOLLOWPOINT, "1"}, // Clip
            {ItemType.PISTOL_MK2_CLIP_INCENDIARY, "1"}, // Clip
            {ItemType.PISTOL_MK2_CLIP_TRACER, "1"}, // Clip
            {ItemType.PISTOL_VARMOD_LUXE, "5"}, // Texture
            {ItemType.PUMPSHOTGUN_MK2_CAMO, "5"}, // Texture
            {ItemType.PUMPSHOTGUN_MK2_CAMO_02, "5"}, // Texture
            {ItemType.PUMPSHOTGUN_MK2_CAMO_03, "5"}, // Texture
            {ItemType.PUMPSHOTGUN_MK2_CAMO_04, "5"}, // Texture
            {ItemType.PUMPSHOTGUN_MK2_CAMO_05, "5"}, // Texture
            {ItemType.PUMPSHOTGUN_MK2_CAMO_06, "5"}, // Texture
            {ItemType.PUMPSHOTGUN_MK2_CAMO_07, "5"}, // Texture
            {ItemType.PUMPSHOTGUN_MK2_CAMO_08, "5"}, // Texture
            {ItemType.PUMPSHOTGUN_MK2_CAMO_09, "5"}, // Texture
            {ItemType.PUMPSHOTGUN_MK2_CAMO_10, "5"}, // Texture
            {ItemType.PUMPSHOTGUN_MK2_CAMO_IND_01, "5"}, // Texture
            {ItemType.PUMPSHOTGUN_MK2_CLIP_ARMORPIERCING, "1"}, // Clip
            {ItemType.PUMPSHOTGUN_MK2_CLIP_EXPLOSIVE, "1"}, // Clip
            {ItemType.PUMPSHOTGUN_MK2_CLIP_HOLLOWPOINT, "1"}, // Clip
            {ItemType.PUMPSHOTGUN_MK2_CLIP_INCENDIARY, "1"}, // Clip
            {ItemType.PUMPSHOTGUN_VARMOD_LOWRIDER, "5"}, // Texture
            {ItemType.RAYPISTOL_VARMOD_XMAS18, "5"}, // Texture
            {ItemType.REVOLVER_MK2_CAMO, "5"}, // Texture
            {ItemType.REVOLVER_MK2_CAMO_02, "5"}, // Texture
            {ItemType.REVOLVER_MK2_CAMO_03, "5"}, // Texture
            {ItemType.REVOLVER_MK2_CAMO_04, "5"}, // Texture
            {ItemType.REVOLVER_MK2_CAMO_05, "5"}, // Texture
            {ItemType.REVOLVER_MK2_CAMO_06, "5"}, // Texture
            {ItemType.REVOLVER_MK2_CAMO_07, "5"}, // Texture
            {ItemType.REVOLVER_MK2_CAMO_08, "5"}, // Texture
            {ItemType.REVOLVER_MK2_CAMO_09, "5"}, // Texture
            {ItemType.REVOLVER_MK2_CAMO_10, "5"}, // Texture
            {ItemType.REVOLVER_MK2_CAMO_IND_01, "5"}, // Texture
            {ItemType.REVOLVER_MK2_CLIP_FMJ, "1"}, // Clip
            {ItemType.REVOLVER_MK2_CLIP_HOLLOWPOINT, "1"}, // Clip
            {ItemType.REVOLVER_MK2_CLIP_INCENDIARY, "1"}, // Clip
            {ItemType.REVOLVER_MK2_CLIP_TRACER, "1"}, // Clip
            {ItemType.REVOLVER_VARMOD_BOSS, "5"}, // Texture
            {ItemType.REVOLVER_VARMOD_GOON, "5"}, // Texture
            {ItemType.SAWNOFFSHOTGUN_VARMOD_LUXE, "5"}, // Texture
            {ItemType.SMG_CLIP_02, "1"}, // Clip
            {ItemType.SMG_CLIP_03, "1"}, // Clip
            {ItemType.SMG_MK2_CAMO, "5"}, // Texture
            {ItemType.SMG_MK2_CAMO_02, "5"}, // Texture
            {ItemType.SMG_MK2_CAMO_03, "5"}, // Texture
            {ItemType.SMG_MK2_CAMO_04, "5"}, // Texture
            {ItemType.SMG_MK2_CAMO_05, "5"}, // Texture
            {ItemType.SMG_MK2_CAMO_06, "5"}, // Texture
            {ItemType.SMG_MK2_CAMO_07, "5"}, // Texture
            {ItemType.SMG_MK2_CAMO_08, "5"}, // Texture
            {ItemType.SMG_MK2_CAMO_09, "5"}, // Texture
            {ItemType.SMG_MK2_CAMO_10, "5"}, // Texture
            {ItemType.SMG_MK2_CAMO_IND_01, "5"}, // Texture
            {ItemType.SMG_MK2_CLIP_02, "1"}, // Clip
            {ItemType.SMG_MK2_CLIP_FMJ, "1"}, // Clip
            {ItemType.SMG_MK2_CLIP_HOLLOWPOINT, "1"}, // Clip
            {ItemType.SMG_MK2_CLIP_INCENDIARY, "1"}, // Clip
            {ItemType.SMG_MK2_CLIP_TRACER, "1"}, // Clip
            {ItemType.SMG_VARMOD_LUXE, "5"}, // Texture
            {ItemType.SNIPERRIFLE_VARMOD_LUXE, "5"}, // Texture
            {ItemType.SNSPISTOL_CLIP_02, "1"}, // Clip
            {ItemType.SNSPISTOL_MK2_CAMO, "5"}, // Texture
            {ItemType.SNSPISTOL_MK2_CAMO_02, "5"}, // Texture
            {ItemType.SNSPISTOL_MK2_CAMO_02_SLIDE, "5"}, // Texture
            {ItemType.SNSPISTOL_MK2_CAMO_03, "5"}, // Texture
            {ItemType.SNSPISTOL_MK2_CAMO_03_SLIDE, "5"}, // Texture
            {ItemType.SNSPISTOL_MK2_CAMO_04, "5"}, // Texture
            {ItemType.SNSPISTOL_MK2_CAMO_04_SLIDE, "5"}, // Texture
            {ItemType.SNSPISTOL_MK2_CAMO_05, "5"}, // Texture
            {ItemType.SNSPISTOL_MK2_CAMO_05_SLIDE, "5"}, // Texture
            {ItemType.SNSPISTOL_MK2_CAMO_06, "5"}, // Texture
            {ItemType.SNSPISTOL_MK2_CAMO_06_SLIDE, "5"}, // Texture
            {ItemType.SNSPISTOL_MK2_CAMO_07, "5"}, // Texture
            {ItemType.SNSPISTOL_MK2_CAMO_07_SLIDE, "5"}, // Texture
            {ItemType.SNSPISTOL_MK2_CAMO_08, "5"}, // Texture
            {ItemType.SNSPISTOL_MK2_CAMO_08_SLIDE, "5"}, // Texture
            {ItemType.SNSPISTOL_MK2_CAMO_09, "5"}, // Texture
            {ItemType.SNSPISTOL_MK2_CAMO_09_SLIDE, "5"}, // Texture
            {ItemType.SNSPISTOL_MK2_CAMO_10, "5"}, // Texture
            {ItemType.SNSPISTOL_MK2_CAMO_10_SLIDE, "5"}, // Texture
            {ItemType.SNSPISTOL_MK2_CAMO_IND_01, "5"}, // Texture
            {ItemType.SNSPISTOL_MK2_CAMO_IND_01_SLIDE, "5"}, // Texture
            {ItemType.SNSPISTOL_MK2_CAMO_SLIDE, "5"}, // Texture
            {ItemType.SNSPISTOL_MK2_CLIP_02, "1"}, // Clip
            {ItemType.SNSPISTOL_MK2_CLIP_FMJ, "1"}, // Clip
            {ItemType.SNSPISTOL_MK2_CLIP_HOLLOWPOINT, "1"}, // Clip
            {ItemType.SNSPISTOL_MK2_CLIP_INCENDIARY, "1"}, // Clip
            {ItemType.SNSPISTOL_MK2_CLIP_TRACER, "1"}, // Clip
            {ItemType.SNSPISTOL_VARMOD_LOWRIDER, "5"}, // Texture
            {ItemType.SPECIALCARBINE_CLIP_02, "1"}, // Clip
            {ItemType.SPECIALCARBINE_CLIP_03, "1"}, // Clip
            {ItemType.SPECIALCARBINE_MK2_CAMO, "5"}, // Texture
            {ItemType.SPECIALCARBINE_MK2_CAMO_02, "5"}, // Texture
            {ItemType.SPECIALCARBINE_MK2_CAMO_03, "5"}, // Texture
            {ItemType.SPECIALCARBINE_MK2_CAMO_04, "5"}, // Texture
            {ItemType.SPECIALCARBINE_MK2_CAMO_05, "5"}, // Texture
            {ItemType.SPECIALCARBINE_MK2_CAMO_06, "5"}, // Texture
            {ItemType.SPECIALCARBINE_MK2_CAMO_07, "5"}, // Texture
            {ItemType.SPECIALCARBINE_MK2_CAMO_08, "5"}, // Texture
            {ItemType.SPECIALCARBINE_MK2_CAMO_09, "5"}, // Texture
            {ItemType.SPECIALCARBINE_MK2_CAMO_10, "5"}, // Texture
            {ItemType.SPECIALCARBINE_MK2_CAMO_IND_01, "5"}, // Texture
            {ItemType.SPECIALCARBINE_MK2_CLIP_02, "1"}, // Clip
            {ItemType.SPECIALCARBINE_MK2_CLIP_ARMORPIERCING, "1"}, // Clip
            {ItemType.SPECIALCARBINE_MK2_CLIP_FMJ, "1"}, // Clip
            {ItemType.SPECIALCARBINE_MK2_CLIP_INCENDIARY, "1"}, // Clip
            {ItemType.SPECIALCARBINE_MK2_CLIP_TRACER, "1"}, // Clip
            {ItemType.SPECIALCARBINE_VARMOD_LOWRIDER, "5"}, // Texture
            {ItemType.SWITCHBLADE_VARMOD_VAR1, "5"}, // Texture
            {ItemType.SWITCHBLADE_VARMOD_VAR2, "5"}, // Texture
            {ItemType.VINTAGEPISTOL_CLIP_02, "1"}, // Clip
          #endregion;
            {ItemType.DigScanner , "gun"},
            {ItemType.DigScanner_mk2 , "gun"},
            {ItemType.DigScanner_mk3 , "gun"},

            {ItemType.DigShovel , "gun"},
            {ItemType.DigShovel_mk2 , "gun"},
            {ItemType.DigShovel_mk3 , "gun"},

            {ItemType.CraftCap ,"craft1"},
            {ItemType.CraftOldCoin , "craft2"},
            {ItemType.CraftShell , "craft3"},
            {ItemType.CraftScrapMetal, "craft5"},
            {ItemType.CraftCopperNugget,  "craft6"},
            {ItemType.CraftIronNugget,  "craft7"},
            {ItemType.CraftTinNugget,  "craft8"},

            {ItemType.CraftCopperWire, "craft9"},
            {ItemType.CraftOldJewerly, "craft10"},
            {ItemType.CraftGoldNugget, "craft11"},
            {ItemType.CraftСollectibleCoin, "craft12"},
            {ItemType.CraftAncientStatuette, "craft13"},
            {ItemType.CraftGoldHorseShoe, "craft14"},
            {ItemType.CraftRelic, "craft15"},

            {ItemType.CraftIronPart, "craft16" },
            {ItemType.CraftCopperPart, "craft17" },
            {ItemType.CraftTinPart, "craft18" },
            {ItemType.CraftBronzePart, "craft19" },

            {ItemType.CraftWorkBench, "craft20" },
            {ItemType.CraftPercolator, "craft21" },
            {ItemType.CraftSmelter, "craft22" },
            {ItemType.CraftPartsCollector, "craft23" },
            {ItemType.CraftWorkBenchUpgrade, "craft24" },
            {ItemType.CraftWorkBenchUpgrade2, "craft25" },
        };
        public static Dictionary<ItemType, string> ItemFdesc = new Dictionary<ItemType, string>()
        {
            { ItemType.BagWithMoney, "Мешок с деньгами" },
            { ItemType.Material, "Материалы используется организациями для создания оружия." },
            { ItemType.Drugs, "Наркотики являются очень опасными для вашего организма!" },
            { ItemType.BagWithDrill, "Сумка с дрелью" },
            { ItemType.Debug, "Дебаг." },
            { ItemType.Bandage, "Поможет восстановить здоровье в моменты, когда никто по близости не может вам помочь." },
            { ItemType.HealthKit, "Поможет восстановить здоровье в моменты, когда никто по близости не может вам помочь." },
            { ItemType.GasCan, "Позволяет хранить бензин и поможет Вам в случае большой удаленности от заправок." },
            { ItemType.Сrisps, "Шуршащая упаковка, наполненная тонко нарезанными ломтиками жаренного картофеля с большим количеством специи." },
            { ItemType.Beer, "Отличное решение в жаркий летний день. Помните: чрезмерное употребление алкоголя вредит Вашему здоровью!" },
            { ItemType.Pizza, "Вряд ли кто-то откажется от пары кусочков пиццы! К тому же, она отлично утоляет чувство голода." },
            { ItemType.Burger, "Легенда, которая не нуждается в представлении. Бургер составит хорошую пару Коле и прекрасно насыщает!" },
            { ItemType.HotDog, "Жареная сосиска в разрезе мягкой булочки, политая горчицей с кетчупом." },
            { ItemType.Sandwich, "Легкий перекус, который будет весьма кстати, если вы немного проголодались." },
            { ItemType.eCola, "Настоящая классика среди газировок." },
            { ItemType.Sprunk, "Лучший напиток утоляющий жажду!" },
            { ItemType.Lockpick, "Поможет вам взломать разного рода замки. Является незаконным предметом." },
            { ItemType.ArmyLockpick, "Взламывает армейский военный транспорт. Является незаконным предметом." },
            { ItemType.Pocket, "Ограничивает обзор другому человеку. Является незаконным предметом." },
            { ItemType.Cuffs, "Позволяют сковать другого человека. Являются незаконным предметом." },
            { ItemType.CarKey, "Необходим для открытия дверей Вашего транспорта." },
            { ItemType.Present, "Содержит разного рода полезные ресурсы." },
            { ItemType.KeyRing, "Служит для хранения нескольких ключей и экономии места в Ваших карманах." },
            //{ ItemType.Radio, "Радио." },

            { ItemType.Mask, "Скрывает Вашу личность." },
            { ItemType.Gloves, "Дополнят Ваш образ и согреют в морозный день." },
            { ItemType.Leg, "Штаны, разновидность одежды прикрывающая обе ноги отдельно, некоторые модели штанов имеют необычный дезайн, который поможет подчеркнуть Вашу индивидуальность." },
            { ItemType.Bag, "Данная модель поможет переносить с собой на 5 килограмм больше вещей. " },
            { ItemType.Bag1, "Данная модель поможет переносить с собой на 10 килограмм больше вещей" },
            { ItemType.Feet, "Обувь, может быть ботинками, тапочками, кроссовками и т.д" },
            { ItemType.Jewelry, "Хорошо подчеркнут Ваш финансовый статус." },
            { ItemType.Undershit, "Базовая футболка. Неотъемлимая часть любого гардероба." },
            { ItemType.BodyArmor, "Защитит от пуль и нежеланной смерти." },
            { ItemType.BodyArmorgov1, "Улучшенная версия стандартного бронижелета." },
            { ItemType.BodyArmorgov2, "Броня государственного образца. Является незаконным предметом. " },
            { ItemType.BodyArmorgov3, "Броня государственного образца. Является незаконным предметом." },
            { ItemType.BodyArmorgov4, "Броня государственного образца. Является незаконным предметом." },
            { ItemType.Unknown, "Неизвестно." },
            { ItemType.Top, "Топ, может быть футболкой, рубашкой, курткой и т.д" },
            { ItemType.Hat, "Защитит Вашу голову от солнца, ветра и холода." },
            { ItemType.Glasses, "Предотвращают прямое попадание солнечных лучей в Ваши глаза." },
            { ItemType.Accessories, "Дополняют Ваш образ и добавляют индивидуальности." },
            { ItemType.RusDrink1, "სასმელი" },
            { ItemType.RusDrink2, "სასმელი" },
            { ItemType.RusDrink3, "სასმელი" },
            { ItemType.YakDrink1, "სასმელი" },
            { ItemType.YakDrink2, "სასმელი" },
            { ItemType.YakDrink3, "სასმელი" },
            { ItemType.LcnDrink1, "სასმელი" },
            { ItemType.LcnDrink2, "სასმელი" },
            { ItemType.LcnDrink3, "სასმელი" },
            { ItemType.ArmDrink1, "სასმელი" },
            { ItemType.ArmDrink2, "სასმელი" },
            { ItemType.ArmDrink3, "სასმელი" },
            {ItemType.WaterBottle,  "Бутылка воды"},
            { ItemType.RepairBox, "Набор инструментов для починки автомобиля"},
            { ItemType.SmallHealthKit, "Маленькая аптечка" },
            { ItemType.Pistol, "Стандартный пистолет с магазином на 12 патронов калибра 9.19, ёмкость которого может быть увеличена до 16." },
            { ItemType.Combatpistol, "Компактный легкий полуавтоматический пистолет, предназначенный для самообороны и использования силами охраны правопорядка. Использующий магазин на 12 патронов калибра 9.19, емкость может быть увеличена до 16." },
            { ItemType.Pistol50, "Мощный пистолет с очень сильной отдачей. Магазин вмещает 9 патронов калибра 9.19." },
            { ItemType.Snspistol, "Лучшая вещь, которую можно положить в сумочку. Если вы хотите сделать свой субботний вечер особенным, это оружие для вас. Только не забудьте положить вместе с ним пару магазинов калибра 9.19!" },
            { ItemType.Heavypistol, "Чемпион мира среди полуавтоматического оружия имеющий калибр 9.19. Обеспечивает высочайшую точность и обладает большой отдачей." },
            { ItemType.Vintagepistol, "По-настоящему уникальный пистолет, который поможет Вам выделиться из толпы. Использует калибр 9.19." },
            { ItemType.Marksmanpistol, "Пистолет для тех, кто любит риск, ведь после каждого выстрела Вас ждёт перезарядка. Использует калибр 9.19." },
            { ItemType.Revolver, "Револьвер с такой убоиной силой, что свалит бешенного носорога, используя при этом калибр 9.19." },
            { ItemType.Appistol, "Полностью автоматический пистолет с высокой пробивной способностью. Магазин на 18 патронов калибра 9.19, ёмкость может быть увеличена до 36." },
            { ItemType.Stungun, "Выстреливает электрод, который поражает противника электричеством, на некоторое время оглушая его. После выстрела на перезарядку уходит около 4 секунд." },
            { ItemType.Flaregun, "Использовать для подачи сигнала, или во время опьянения. Прямое попадание вызывает непроизвольное воспламенение." },
            { ItemType.Doubleaction, "Потому что иногда месть - это блюдо, которое следует подавать холодным. Быстро, шесть раз подряд, точно между глаз. Скорость и точность этому утончённому револьверу придает калибр 9.19." },
            { ItemType.Pistol_mk2, "Улучшенная версия стандартного пистолета. Имеет возмозность модификации. Как и его предшественник использует калибр 9.19." },
            { ItemType.Snspistol_mk2, "Улучшенная версия карманного пистолета. Имеет возмозность модификации. Как и его предшественник использует калибр 9.19." },
            { ItemType.Revolver_mk2, "Улучшенная версия стандартного револьвера. Имеет возмозность модификации. Как и его предшественник использует калибр 9.19." },
            { ItemType.Microsmg, "Этот ПП совмещает компактный дизайн с высокой скорострельностью (в среднем 700-900 выстрелов в минуту). Использует калибр 5.45." },
            { ItemType.Machinepistol, "Для TEC-9 характерен быстрый темп стрельбы и малый магазин: 12 патронов. Этот пистолет-пулемёт обычно используется бандами. Использует калибр 5.45."},
            { ItemType.Smg, "Легкий и точный пистолет-пулемёт с магазином на 30 патронов, не имеющий ярко выраженных недостатков. Использует калибр 5.45." },
            { ItemType.Assaultsmg, "Пистолет-пулемёт, обладающий большой мощностью. Вмещает до 30 патронов в одном магазине. Использует калибр 5.45." },
            { ItemType.Combatpdw, "Персональное оружие не к лицу военным? Конгресс тут ни при чём — благодарите лоббистов. Оснащён глушителем. Использует калибр 5.45." },
            { ItemType.Mg, "Пулемёт общего назначения, сочетающий в себе простоту дизайна и надёжность. Пули калибра 5.45 сохраняют высокую пробивную силу даже на большой дистанции." },
            { ItemType.Combatmg, "Компактный пулемёт с высокой скорострельностью и великолепной разрушительной силой. Использует калибр 5.45." },
            { ItemType.Gusenberg, "Дополните ваш образ оружием эпохи “сухого закона”. Оно отлично смотрится высунутым из окна Albany Roosevelt или вкупе с полосатым костюмом. Использует калибр 5.45." },
            { ItemType.Minismg, "Этот пистолет-пулемёт стал популярным после того, как отдел маркетинга обратил внимание на группы с низким доходом. Использует калибр 5.45." },
            { ItemType.Smg_mk2, "Улучшенная версия пистолета-пулемёта. Имеет возмозность модификации. Как и его предшественник использует калибр 5.45." },
            { ItemType.Combatmg_mk2, "Улучшенная версия стандартного тяжелого пулемёта. Имеет возмозность модификации.Как и его предшественник использует калибр 5.45." },
            { ItemType.Assaultrifle, "Оптимальная переработка нестареющей классики: всё таки красота действительно может быть убийственной. Такой убийственной красоте способствует калибр 7.62." },
            { ItemType.Carbinerifle, "Сочетая точность стрельбы на большом расстоянии с магазином большой емкости, данное оружие является одной из лучших универсальных винтовок. Универсальность данной винтовке придает калибр 7.62." },
            { ItemType.Advancedrifle, "Наиболее лёгкая и компактная из всех штурмовых винтовок, не имеющая себе равных в точности и мощности. Превосходство винтовке придает калибр 7.62." },
            { ItemType.Specialcarbine, "Эта чрезвычайно надежная штурмовая винтовка, сочетает в себе точность, дальнобойность, малый вес, высокую пробиваемость и низкую отдачу. Использует калибр 7.62." },
            { ItemType.Bullpuprifle, "Штурмовая винтовка обладающая неплохой скорострельностью и дальностью стрельбы. Использует калибр 7.62." },
            { ItemType.Compactrifle, "Размер — вдвое меньше, мощь — такая же, а отдача — вдвое сильнее. Использует калибр 7.62." },
            { ItemType.Assaultrifle_mk2, "Улучшенная версия штурмовой винтовки. Имеет возмозность модификации. Как и его предшественник использует калибр 7.62." },
            { ItemType.Carbinerifle_mk2, "Улучшенная версия автоматической винтовки. Имеет возмозность модификации. Как и его предшественник использует калибр 7.62." },
            { ItemType.Specialcarbine_mk2, "Улучшенная версия особого карабина. Имеет возмозность модификации. Как и его предшественник использует калибр 7.62." },
            { ItemType.Bullpuprifle_mk2, "Улучшенная версия Винтовка-«Буллпап». Имеет возмозность модификации. Как и его предшественник использует калибр 7.62." },
            { ItemType.Sniperrifle,"Стандартная снайперская винтовка, идеальная для поражения целей на дальних дистанциях. Использует калибр .50" },
            { ItemType.Heavysniper, "Использует бронебойные патроны, наносящие огромный урон. В стандартную комплектацию входит лазерный прицел. Использует калибр .50" },
            { ItemType.Marksmanrifle, "Неважно, вблизи ли вы, или пугающе далеко - это оружие справится с задачей. Ствол с широким спектром применения. Использует калибр .50" },
            { ItemType.Heavysniper_mk2, "Улучшенная версия тяжелой снайперской винтовки. Имеет возмозность модификации. Как и его предшественник использует калибр .50." },
            { ItemType.Marksmanrifle_mk2, "Улучшенная версия высокоточной снайперской винтовки. Имеет возмозность модификации. Как и его предшественник использует калибр .50." },
            { ItemType.Pumpshotgun, "Стандартный дробовик использующий калибр Gauge.12, идеально подходит для малой дальности боя. Спред высокого снаряда составляет для его более низкую точность на больших расстояниях." },
            { ItemType.Sawnoffshotgun, "Одноствольный укороченный дробовик. Низкая точность и малый боекомплект компенсируются потрясающей эффективностью в ближнем бою. Потрясающая эффективность в ближнем бою придается при помощи калибра Gauge.12." },
            { ItemType.Bullpupshotgun, "Компенсирует низкую скорострельность высокими дальностью стрельбы, разлётом дроби и убойной силой. Использует калибр Gauge.12." },
            { ItemType.Assaultshotgun, "Полностью автоматический скорострельный дробовик с магазином калибра Gauge.12 на 8 патронов." },
            { ItemType.Musket, "Вооружённые только мушкетами и комплексом превосходства, британцы захватили полмира. Приобретите оружие, построившее империю. На сегодняшний день, это оружие использует калибр Gauge.12." },
            { ItemType.Heavyshotgun, "Если хотите разнести всех в кровавое месиво, это оружие для вас. Используйте на легко отмываемых поверхностях, потому-что калибр Gauge.12 оставляет после себя много крови." },
            { ItemType.Dbshotgun, "Кому нужна высокая скорострельность, когда можно первым же выстрелом распылить врага на атомы. Для распыления на атомы используйте калибр Gauge.12." },
            { ItemType.Autoshotgun, "Сколько эффективных инструментов управления толпой можно засунуть себе в штаны? Ну ладно, два. И это - второй. Только не забудьте выделить себе место в штанах под несколько магазинов калибра Gauge.12!" },
            { ItemType.Pumpshotgun_mk2, "Улучшенная версия помпового дробовика. Имеет возмозность модификации. Как и его предшественник использует калибр Gauge.12." },
            { ItemType.Rpg, "Портативное противотанковое оружие, которое стреляет взрывными боеголовками. Очень эффективен для уничтожения транспортных средств или больших групп нападающих." },
            { ItemType.Knife, "Нож, оснащённый семидюймовым обоюдоострым зазубренным лезвием из высокоуглеродистой стали, обладает повышенной проникающей способностью." },
            { ItemType.Nightstick, "24-дюймовая полицейская дубинка из поликарбоната." },
            { ItemType.Hammer, "Надёжный универсальный молоток с деревянной ручкой и изогнутым гвоздодёром — нестареющая классика." },
            { ItemType.Bat, "Алюминиевая бейсбольная бита с кожаной рукояткой. Лёгкое, но грозное оружие в сильных руках." },
            { ItemType.Crowbar, "Прочный ломик из качественной закалённой стали — надёжное орудие для любого дела." },
            { ItemType.Golfclub, "Стандартный айрон для гольфа с резиновой рукояткой для быстрой и смертоносной игры." },
            { ItemType.Bottle, "Не особенно умно, да и выглядит не очень, но против типа, бросающегося на вас с ножом — самое то. Главное — результат." },
            { ItemType.Dagger, "Хотите быть пиратом? Не хватает подходящего оружия? Тогда не пропустите этот кинжал с гардой." },
            { ItemType.Hatchet, "Добавьте к своему арсеналу оружия добрый старый топорик, чтобы всегда иметь оружие про запас, если с боеприпасами туго." },
            { ItemType.Knuckle, "Подойдёт как для выбивания золотых зубов, так и в качестве подарка богатому папику, у которого всё есть." },
            { ItemType.Machete, "Торговля оружием в Африке — не только его распространение. Этот тесак поможет вам познать заново простоту жизни." },
            { ItemType.Flashlight, "Ваш страх темноты станет еще интенсивнее с этим источником света. Может быть полезен для нанесения ударов." },
            { ItemType.Switchblade, "Вот он в кармане, а вот — меж рёбер противника! Выкидные ножи никогда не выйдут из моды." },
            { ItemType.Poolcue, "О, что может быть приятнее хруста, с которым ломается что-то тяжелое, особенно если это чужой позвоночник." },
            { ItemType.Wrench, "Любимая вещь агрессивных папаш и параноидальных выживальщиков. Говорят, его можно использовать и как инструмент." },
            { ItemType.Battleaxe, "Это оружие средневековых солдат, пограничников и наглых городских домохозяек, а значит, оно подойдёт и вам." },
            { ItemType.Rod, "Необходима для ловли рыбы в специализированных водоёмах. Использует наживку." },
            { ItemType.RodMK2, "Улучшенная версия стандартной удочки. Необходима для ловли рыбы в специализированных водоёмах. Использует наживку." },
            { ItemType.RodUpgrade, "Улучшенная версия удочки MK2. Необходима для ловли рыбы в специализированных водоёмах. Использует наживку." },
            { ItemType.Bait, "Используется в паре с удочкой для ловли рыбы в стандартных водоемах." },
            { ItemType.Naz, "Используется в паре с удочкой для ловли рыбы в улучшенных водоемах." },

            { ItemType.Kyndja, "Можно продать скупщику рыбы."},
            { ItemType.Sig, "Можно продать скупщику рыбы."},
            { ItemType.Omyl, "Можно продать скупщику рыбы."},
            { ItemType.Nerka, "Можно продать скупщику рыбы."},
            { ItemType.Forel, "Можно продать скупщику рыбы."},
            { ItemType.Ship,  "Можно продать скупщику рыбы."},
            { ItemType.Lopatonos, "Можно продать скупщику рыбы."},
            { ItemType.Osetr, "Можно продать скупщику рыбы."},
            { ItemType.Semga, "Можно продать скупщику рыбы."},
            { ItemType.Servyga, "Можно продать скупщику рыбы."},
            { ItemType.Beluga, "Можно продать скупщику рыбы."},
            { ItemType.Taimen, "Можно продать скупщику рыбы."},
            { ItemType.Sterlyad, "Можно продать скупщику рыбы."},
            { ItemType.Ydilshik, "Можно продать скупщику рыбы."},
            { ItemType.Hailiod, "Можно продать скупщику рыбы."},
            { ItemType.Keta, "Можно продать скупщику рыбы."},
            { ItemType.Gorbysha, "Можно продать скупщику рыбы."},
            //{ ItemType.GiveBox, "დონატ ქეისი" },
            //{ ItemType.Box1, "დონატ ნაკრები" },
            //{ ItemType.Box2, "დონატ ნაკრები" },
            //{ ItemType.Box3, "დონატ ნაკრები" },
            //{ ItemType.Box4, "დონატ ნაკრები" },
            //{ ItemType.CarLow, "donate66"},
            //{ ItemType.CarPremium, "donate77" },
            //{ ItemType.CarSport, "donate88" },
            { ItemType.PistolAmmo, "Совместимы с оружием, использующим калибр 9.19" },
            { ItemType.RiflesAmmo, "Совместимы с оружием, использующим калибр 7.62" },
            { ItemType.ShotgunsAmmo, "Совместимы с оружием, использующим калибр Gauge.12" },
            { ItemType.SMGAmmo, "Совместимы с оружием, использующим калибр 5.45" },
            { ItemType.SniperAmmo, "Совместимы с оружием, использующим калибр .50" },
            //{ ItemType.Pistol_Ammo_Box, "Емкость для упаковки свинца калибра 9.19 - 60 пуль" },
            //{ ItemType.Rifle_Ammo_Box, "Емкость для упаковки свинца калибра 7.62 - 60 пуль" },
            //{ ItemType.Smg_Ammo_Box, "Емкость для упаковки свинца калибра 5.45 - 60 пуль" },
            //{ ItemType.Shotgun_Ammo_Box, "Емкость для упаковки свинца калибра Gauge.12 - 60 пуль" },
            //{ ItemType.Sniper_Ammo_Box, "Емкость для упаковки свинца калибра .50 - 20 пуль" },

            { ItemType.CasinoChips, "С их помощью Вы можете сделать ставку в казино штата" },
            { ItemType.Grenade, "Граната" },
            { ItemType.LSPDDrone, "Используется полицией для патрулирования территории" },
            { ItemType.Drone, "Дрон" },
            { ItemType.NumberPlate, "Номерной знак, алюминиевая табличка содержащая информацию о номере автомобиля." },
            { ItemType.SimCard, "Сим-карта, просто вставте её в телефон, чтобы пользоватся связью нашего штата, только не забывайте вовремя пополнять баланс." },
            { ItemType.ProductBox, "Ящик продуктов, который может содержать в себе все что угодно." },
            { ItemType.TrashBag, "Пакет с мусором, содержание данного пакета неизвестно." },
            { ItemType.CocaLeaves, "Листья коки, используется для крафта кокайна" },
            { ItemType.Weed, "Пачка трава, при использование которой вас может привести к плохим последствиям." },
            { ItemType.Cocaine, "Кокаин, мощное стимулирующее вещество распространяющиеся в виде порошка." },
            { ItemType.DrugBookMark, "Странный пакетик с порошков внутри обмотанный изолентой." },
            { ItemType.Subject, "Предмет вынесенный из чужого дома, который явно принадлежит не вам." },

            // [Modification] Миша, Дима, Надо прописать описание
            {ItemType.ADVANCEDRIFLE_CLIP_02, "Вмещает в себя большее количество патронов, совместимо с Улучшенной винтовкой"}, // Clip
            {ItemType.ADVANCEDRIFLE_VARMOD_LUXE, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.APPISTOL_CLIP_02, "Вмещает в себя большее количество патронов, совместимо с Бронебойным пистолетом"}, // Clip
            {ItemType.APPISTOL_VARMOD_LUXE, "Данное улучшение позволяет менять расцветку Вашего оружия."}, // Texture
            {ItemType.ASSAULTRIFLE_CLIP_02, "Вмещает в себя 60 патронов, совместимо с Штурмовой винтовкой."}, // Clip
            {ItemType.ASSAULTRIFLE_CLIP_03, "Вмещает в себя 100 патронов, совместимо с Штурмовой винтовкой."}, // Clip
            {ItemType.ASSAULTRIFLE_MK2_CAMO, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.ASSAULTRIFLE_MK2_CAMO_02, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.ASSAULTRIFLE_MK2_CAMO_03, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.ASSAULTRIFLE_MK2_CAMO_04, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.ASSAULTRIFLE_MK2_CAMO_05, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.ASSAULTRIFLE_MK2_CAMO_06, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.ASSAULTRIFLE_MK2_CAMO_07, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.ASSAULTRIFLE_MK2_CAMO_08, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.ASSAULTRIFLE_MK2_CAMO_09, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.ASSAULTRIFLE_MK2_CAMO_10, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.ASSAULTRIFLE_MK2_CAMO_IND_01, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.ASSAULTRIFLE_MK2_CLIP_02, "Вмещает в себя большее количество патронов, совместимо с Автоматической винтовкой MK2"}, // Clip
            {ItemType.ASSAULTRIFLE_MK2_CLIP_ARMORPIERCING, "Наносят больший урон врагам в броне."}, // Clip
            {ItemType.ASSAULTRIFLE_MK2_CLIP_FMJ, "Пробивают броню автомобиля, защищенные шины и прочую броню."}, // Clip
            {ItemType.ASSAULTRIFLE_MK2_CLIP_INCENDIARY, "Есть вероятность поджечь врага при попадании."}, // Clip
            {ItemType.ASSAULTRIFLE_MK2_CLIP_TRACER, "Делает траекторию снаряда видимой невооруженным глазом при дневном свете и очень яркой при ночной стрельбе."}, // Clip
            {ItemType.ASSAULTRIFLE_VARMOD_LUXE, "Данное улучшение позволяет менять расцветку Вашего оружия."}, // Texture
            {ItemType.ASSAULTSHOTGUN_CLIP_02, "Вмещает в себя большее количество патронов, совместимо с Штурмовым дробовиком."}, // Clip
            {ItemType.ASSAULTSMG_CLIP_02, "Вмещает в себя большее количество патронов, совместимо с Штурмовым ПП."}, // Clip
            {ItemType.ASSAULTSMG_VARMOD_LOWRIDER, "Данное улучшение позволяет менять расцветку Вашего оружия."}, // Texture
            {ItemType.AT_AR_AFGRIP, "Повышает точность стрельбы оружия."}, // Grip? [Modification NEED CHECK SLOT NUMBER]
            {ItemType.AT_AR_AFGRIP_02, "Повышает точность стрельбы оружия."}, // Grip? [Modification NEED CHECK SLOT NUMBER]
            {ItemType.AT_AR_BARREL_02, "Увеличивает урон при стрельбе на дальние расстояния."}, // Barrel / Compensator? [Modification Need CHECK SLOT NUMBER]
            {ItemType.AT_AR_FLSH, "Данное улучшение позволяет Вам освещать помещение в условиях ограниченной видимости."}, // FlashLight
            {ItemType.AT_AR_SUPP, "Данное улучшение позволяет уменьшить громкость Ваших выстрелов. Совместимо с Автоматической винтовкой."}, // Suppressor
            {ItemType.AT_AR_SUPP_02, "Данное улучшение позволяет уменьшить громкость Ваших выстрелов."}, // Suppressor
            {ItemType.AT_BP_BARREL_02, "Увеличивает урон при стрельбе на дальние расстояния."}, // Barrel / Compensator? [Modification Need CHECK SLOT NUMBER]
            {ItemType.AT_CR_BARREL_02, "Увеличивает урон при стрельбе на дальние расстояния."}, // Barrel / Compensator? [Modification Need CHECK SLOT NUMBER]
            {ItemType.AT_MG_BARREL_02, "Увеличивает урон при стрельбе на дальние расстояния."}, // Barrel / Compensator? [Modification Need CHECK SLOT NUMBER]
            {ItemType.AT_MRFL_BARREL_02, "Увеличивает урон при стрельбе на дальние расстояния."}, // Barrel / Compensator? [Modification Need CHECK SLOT NUMBER]
            {ItemType.AT_MUZZLE_01, "Уменьшает отдачу во время стрельбы очередями."}, // Suppressor
            {ItemType.AT_MUZZLE_02, "Уменьшает отдачу во время стрельбы очередями."}, // Suppressor
            {ItemType.AT_MUZZLE_03, "Уменьшает отдачу во время стрельбы очередями."}, // Suppressor
            {ItemType.AT_MUZZLE_04, "Уменьшает отдачу во время стрельбы очередями."}, // Suppressor
            {ItemType.AT_MUZZLE_05, "Уменьшает отдачу во время стрельбы очередями."}, // Suppressor
            {ItemType.AT_MUZZLE_06, "Уменьшает отдачу во время стрельбы очередями."}, // Suppressor
            {ItemType.AT_MUZZLE_07, "Уменьшает отдачу во время стрельбы очередями."}, // Suppressor
            {ItemType.AT_MUZZLE_08, "Уменьшает отдачу во время стрельбы очередями."}, // Suppressor
            {ItemType.AT_MUZZLE_09, "Уменьшает отдачу во время стрельбы очередями."}, // Suppressor
            {ItemType.AT_PI_COMP, "Уменьшает отдачу во время стрельбы очередями."}, // Suppressor / Compensator
            {ItemType.AT_PI_COMP_02, "Уменьшает отдачу во время стрельбы очередями."}, // Suppressor / Compensator
            {ItemType.AT_PI_COMP_03, "Уменьшает отдачу во время стрельбы очередями."}, // Suppressor / Compensator
            {ItemType.AT_PI_FLSH, "Данное улучшение позволяет Вам освещать помещение в условиях ограниченной видимости."}, // FlashLight
            {ItemType.AT_PI_FLSH_02, "Данное улучшение позволяет Вам освещать помещение в условиях ограниченной видимости."}, // FlashLight
            {ItemType.AT_PI_FLSH_03, "Данное улучшение позволяет Вам освещать помещение в условиях ограниченной видимости."}, // FlashLight
            {ItemType.AT_PI_RAIL, "Позволяет более точно наводиться на противника на дальних дистанциях."}, // Scope / Mounted Scope
            {ItemType.AT_PI_RAIL_02, "Позволяет более точно наводиться на противника на дальних дистанциях."}, // Scope / Mounted Scope
            {ItemType.AT_PI_SUPP, "Данное улучшение позволяет уменьшить громкость Ваших выстрелов."}, // Suppressor
            {ItemType.AT_PI_SUPP_02, "Данное улучшение позволяет уменьшить громкость Ваших выстрелов."}, // Suppressor
            {ItemType.AT_SB_BARREL_02, "Увеличивает урон при стрельбе на дальние расстояния, совместимо с Пистолетом-Пулемётом MK2."}, // Barrel / Compensator? [Modification Need CHECK SLOT NUMBER]
            {ItemType.AT_SC_BARREL_02, "Увеличивает урон при стрельбе на дальние расстояния."}, // Barrel / Compensator? [Modification Need CHECK SLOT NUMBER]
            {ItemType.AT_SCOPE_LARGE, "Позволяет более точно наводиться на противника на дальних дистанциях."}, // Scope
            {ItemType.AT_SCOPE_LARGE_FIXED_ZOOM, "Позволяет более точно наводиться на противника на дальних дистанциях."}, // Scope
            {ItemType.AT_SCOPE_LARGE_FIXED_ZOOM_MK2, "Позволяет более точно наводиться на противника на дальних дистанциях."}, // Scope
            {ItemType.AT_SCOPE_LARGE_MK2, "Позволяет более точно наводиться на противника на дальних дистанциях."}, // Scope
            {ItemType.AT_SCOPE_MACRO, "Позволяет более точно наводиться на противника на дальних дистанциях."}, // Scope
            {ItemType.AT_SCOPE_MACRO_02, "Позволяет более точно наводиться на противника на дальних дистанциях."}, // Scope
            {ItemType.AT_SCOPE_MACRO_02_MK2, "Позволяет более точно наводиться на противника на дальних дистанциях."}, // Scope
            {ItemType.AT_SCOPE_MACRO_02_SMG_MK2, "Позволяет более точно наводиться на противника на дальних дистанциях."}, // Scope
            {ItemType.AT_SCOPE_MACRO_MK2, "Позволяет более точно наводиться на противника на дальних дистанциях."}, // Scope
            {ItemType.AT_SCOPE_MAX, "Позволяет более точно наводиться на противника на дальних дистанциях."}, // Scope
            {ItemType.AT_SCOPE_MEDIUM, "Позволяет более точно наводиться на противника на дальних дистанциях."}, // Scope
            {ItemType.AT_SCOPE_MEDIUM_MK2, "Позволяет более точно наводиться на противника на дальних дистанциях."}, // Scope
            {ItemType.AT_SCOPE_NV, "Позволяет лучше видеть ночью."}, // Scope
            {ItemType.AT_SCOPE_SMALL, "Позволяет более точно наводиться на противника на дальних дистанциях."}, // Scope
            {ItemType.AT_SCOPE_SMALL_02, "Позволяет более точно наводиться на противника на дальних дистанциях."}, // Scope
            {ItemType.AT_SCOPE_SMALL_MK2, "Позволяет более точно наводиться на противника на дальних дистанциях."}, // Scope
            {ItemType.AT_SCOPE_SMALL_SMG_MK2, "Позволяет более точно наводиться на противника на дальних дистанциях."}, // Scope
            {ItemType.AT_SCOPE_THERMAL, "Позволяет обнаружить врага благодаря его тепловому следу."}, // Scope
            {ItemType.AT_SIGHTS, "Позволяет более точно наводиться на противника на дальних дистанциях."}, // Scope / Sights
            {ItemType.AT_SIGHTS_SMG, "Позволяет более точно наводиться на противника на дальних дистанциях."}, // Scope / Sights
            {ItemType.AT_SR_BARREL_02, "Увеличивает урон при стрельбе на дальние расстояния."}, // Barrel / Compensator? [Modification Need CHECK SLOT NUMBER]
            {ItemType.AT_SR_SUPP, "Данное улучшение позволяет уменьшить громкость Ваших выстрелов."}, // Suppressor
            {ItemType.AT_SR_SUPP_03, "Данное улучшение позволяет уменьшить громкость Ваших выстрелов."}, // Suppressor
            {ItemType.BULLPUPRIFLE_CLIP_02, "Вмещает в себя большее количество патронов, совместимо с Винтовкой-«Буллпап»."}, // Clip
            {ItemType.BULLPUPRIFLE_MK2_CAMO, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.BULLPUPRIFLE_MK2_CAMO_02, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.BULLPUPRIFLE_MK2_CAMO_03, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.BULLPUPRIFLE_MK2_CAMO_04, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.BULLPUPRIFLE_MK2_CAMO_05, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.BULLPUPRIFLE_MK2_CAMO_06, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.BULLPUPRIFLE_MK2_CAMO_07, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.BULLPUPRIFLE_MK2_CAMO_08, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.BULLPUPRIFLE_MK2_CAMO_09, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.BULLPUPRIFLE_MK2_CAMO_10, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.BULLPUPRIFLE_MK2_CAMO_IND_01, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.BULLPUPRIFLE_MK2_CLIP_02, "Вмещает в себя большее количество патронов, совместимо с Винтовкой-«Буллпап» MK2."}, // Clip
            {ItemType.BULLPUPRIFLE_MK2_CLIP_ARMORPIERCING, "Наносят больший урон врагам в броне."}, // Clip
            {ItemType.BULLPUPRIFLE_MK2_CLIP_FMJ, "Пробивают броню автомобиля, защищенные шины и прочую броню."}, // Clip
            {ItemType.BULLPUPRIFLE_MK2_CLIP_INCENDIARY, "Есть вероятность поджечь врага при попадании."}, // Clip
            {ItemType.BULLPUPRIFLE_MK2_CLIP_TRACER, "Делает траекторию снаряда видимой невооруженным глазом при дневном свете и очень яркой при ночной стрельбе."}, // Clip
            {ItemType.BULLPUPRIFLE_VARMOD_LOW, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.CARBINERIFLE_CLIP_02, "Вмещает в себя 60 патронов, совместимо с Автоматической винтовкой."}, // Clip
            {ItemType.CARBINERIFLE_CLIP_03, "Вмещает в себя 100 патронов, совместимо с Автоматической винтовкой."}, // Clip
            {ItemType.CARBINERIFLE_MK2_CAMO, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.CARBINERIFLE_MK2_CAMO_02, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.CARBINERIFLE_MK2_CAMO_03, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.CARBINERIFLE_MK2_CAMO_04, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.CARBINERIFLE_MK2_CAMO_05, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.CARBINERIFLE_MK2_CAMO_06, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.CARBINERIFLE_MK2_CAMO_07, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.CARBINERIFLE_MK2_CAMO_08, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.CARBINERIFLE_MK2_CAMO_09, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.CARBINERIFLE_MK2_CAMO_10, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.CARBINERIFLE_MK2_CAMO_IND_01, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.CARBINERIFLE_MK2_CLIP_02, "Вмещает в себя большее количество патронов, совместимо с Автоматической винтовкой."}, // Clip
            {ItemType.CARBINERIFLE_MK2_CLIP_ARMORPIERCING, "Наносят больший урон врагам в броне."}, // Clip
            {ItemType.CARBINERIFLE_MK2_CLIP_FMJ, "Пробивают броню автомобиля, защищенные шины и прочую броню."}, // Clip
            {ItemType.CARBINERIFLE_MK2_CLIP_INCENDIARY, "Есть вероятность поджечь врага при попадании."}, // Clip
            {ItemType.CARBINERIFLE_MK2_CLIP_TRACER, "Делает траекторию снаряда видимой невооруженным глазом при дневном свете и очень яркой при ночной стрельбе."}, // Clip
            {ItemType.CARBINERIFLE_VARMOD_LUXE, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.CERAMICPISTOL_CLIP_02, "Вмещает в себя большее количество патронов, совместимо с Керамическим пистолетом."}, // Clip
            {ItemType.CERAMICPISTOL_SUPP, "Данное улучшение позволяет уменьшить громкость Ваших выстрелов."}, // Suppressor
            {ItemType.COMBATMG_CLIP_02, "Вмещает в себя большее количество патронов, совместимо с Тяжелым пулемётом."}, // Clip
            {ItemType.COMBATMG_MK2_CAMO, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.COMBATMG_MK2_CAMO_02, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.COMBATMG_MK2_CAMO_03, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.COMBATMG_MK2_CAMO_04, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.COMBATMG_MK2_CAMO_05, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.COMBATMG_MK2_CAMO_06, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.COMBATMG_MK2_CAMO_07, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.COMBATMG_MK2_CAMO_08, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.COMBATMG_MK2_CAMO_09, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.COMBATMG_MK2_CAMO_10, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.COMBATMG_MK2_CAMO_IND_01, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.COMBATMG_MK2_CLIP_02, "Вмещает в себя большее количество патронов, совместимо с Пулемётом MK2."}, // Clip
            {ItemType.COMBATMG_MK2_CLIP_ARMORPIERCING, "Наносят больший урон врагам в броне."}, // Clip
            {ItemType.COMBATMG_MK2_CLIP_FMJ, "Пробивают броню автомобиля, защищенные шины и прочую броню."}, // Clip
            {ItemType.COMBATMG_MK2_CLIP_INCENDIARY, "Есть вероятность поджечь врага при попадании."}, // Clip
            {ItemType.COMBATMG_MK2_CLIP_TRACER, "Делает траекторию снаряда видимой невооруженным глазом при дневном свете и очень яркой при ночной стрельбе."}, // Clip
            {ItemType.COMBATMG_VARMOD_LOWRIDER, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.COMBATPDW_CLIP_02, "Вмещает в себя 60 патронов, совместимо с ПОС."}, // Clip
            {ItemType.COMBATPDW_CLIP_03, "Вмещает в себя 100 патронов, совместимо с ПОС."}, // Clip
            {ItemType.COMBATPISTOL_CLIP_02, "Вмещает в себя большее количество патронов, совместимо с Боевым пистолетом."}, // Clip
            {ItemType.COMBATPISTOL_VARMOD_LOWRIDER, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.COMPACTRIFLE_CLIP_02, "Вмещает в себя 60 патронов, совместимо с Укороченной винтовкой."}, // Clip
            {ItemType.COMPACTRIFLE_CLIP_03, "Вмещает в себя 100 патронов, совместимо с Укороченной винтовкой."}, // Clip
            {ItemType.GUSENBERG_CLIP_02, "Вмещает в себя большее количество патронов, совместимо с ПП Томпсона."}, // Clip
            {ItemType.HEAVYPISTOL_CLIP_02, "Вмещает в себя большее количество патронов, совместимо с Тяжелым пистолетом."}, // Clip
            {ItemType.HEAVYPISTOL_VARMOD_LUXE, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.HEAVYSHOTGUN_CLIP_02, "Вмещает в себя большее количество патронов, совместимо с Тяжелым дробовиком."}, // Clip
            {ItemType.HEAVYSHOTGUN_CLIP_03, "Вмещает в себя большее количество патронов, совместимо с Тяжелым дробовиком."}, // Clip
            {ItemType.HEAVYSNIPER_MK2_CAMO, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.HEAVYSNIPER_MK2_CAMO_02, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.HEAVYSNIPER_MK2_CAMO_03, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.HEAVYSNIPER_MK2_CAMO_04, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.HEAVYSNIPER_MK2_CAMO_05, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.HEAVYSNIPER_MK2_CAMO_06, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.HEAVYSNIPER_MK2_CAMO_07, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.HEAVYSNIPER_MK2_CAMO_08, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.HEAVYSNIPER_MK2_CAMO_09, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.HEAVYSNIPER_MK2_CAMO_10, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.HEAVYSNIPER_MK2_CAMO_IND_01, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.HEAVYSNIPER_MK2_CLIP_02, "Вмещает в себя большее количество патронов, совместимо с Тяжелой снайперской винтовкой."}, // Clip
            {ItemType.HEAVYSNIPER_MK2_CLIP_ARMORPIERCING, "Наносят больший урон врагам в броне."}, // Clip
            {ItemType.HEAVYSNIPER_MK2_CLIP_EXPLOSIVE, "Взрывается при попадании во врага."}, // Clip
            {ItemType.HEAVYSNIPER_MK2_CLIP_FMJ, "Пробивают броню автомобиля, защищенные шины и прочую броню."}, // Clip
            {ItemType.HEAVYSNIPER_MK2_CLIP_INCENDIARY, "Есть вероятность поджечь врага при попадании."}, // Clip
            {ItemType.KNUCKLE_VARMOD_BALLAS, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.KNUCKLE_VARMOD_DIAMOND, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.KNUCKLE_VARMOD_DOLLAR, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.KNUCKLE_VARMOD_HATE, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.KNUCKLE_VARMOD_KING, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.KNUCKLE_VARMOD_LOVE, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.KNUCKLE_VARMOD_PIMP, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.KNUCKLE_VARMOD_PLAYER, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.KNUCKLE_VARMOD_VAGOS, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.MACHINEPISTOL_CLIP_02, "Вмещает в себя большее количество патронов, совместимо с Малым ПП."}, // Clip
            {ItemType.MACHINEPISTOL_CLIP_03, "Вмещает в себя большее количество патронов, совместимо с Малым ПП."}, // Clip
            {ItemType.MARKSMANRIFLE_CLIP_02, "Вмещает в себя большее количество патронов, совместимо с Высокоточной винтовкой."}, // Clip
            {ItemType.MARKSMANRIFLE_MK2_CAMO, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.MARKSMANRIFLE_MK2_CAMO_02, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.MARKSMANRIFLE_MK2_CAMO_03, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.MARKSMANRIFLE_MK2_CAMO_04, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.MARKSMANRIFLE_MK2_CAMO_05, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.MARKSMANRIFLE_MK2_CAMO_06, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.MARKSMANRIFLE_MK2_CAMO_07, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.MARKSMANRIFLE_MK2_CAMO_08, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.MARKSMANRIFLE_MK2_CAMO_09, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.MARKSMANRIFLE_MK2_CAMO_10, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.MARKSMANRIFLE_MK2_CAMO_IND_01, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.MARKSMANRIFLE_MK2_CLIP_02, "Вмещает в себя большее количество патронов, совместимо с Выскоточной снайперской винтовкой MK2."}, // Clip
            {ItemType.MARKSMANRIFLE_MK2_CLIP_ARMORPIERCING, "Наносят больший урон врагам в броне."}, // Clip
            {ItemType.MARKSMANRIFLE_MK2_CLIP_FMJ, "Пробивают броню автомобиля, защищенные шины и прочую броню."}, // Clip
            {ItemType.MARKSMANRIFLE_MK2_CLIP_INCENDIARY, "Есть вероятность поджечь врага при попадании."}, // Clip
            {ItemType.MARKSMANRIFLE_MK2_CLIP_TRACER, "Делает траекторию снаряда видимой невооруженным глазом при дневном свете и очень яркой при ночной стрельбе."}, // Clip
            {ItemType.MARKSMANRIFLE_VARMOD_LUXE, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.MG_CLIP_02, "Вмещает в себя большее количество патронов, совместимо с Пулемётом."}, // Clip
            {ItemType.MG_VARMOD_LOWRIDER, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.MICROSMG_CLIP_02, "Вмещает в себя большее количество патронов, совместимо с Микро ПП."}, // Clip
            {ItemType.MICROSMG_VARMOD_LUXE, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.MILITARYRIFLE_CLIP_02, "Вмещает в себя большее количество патронов."}, // Clip
            {ItemType.MILITARYRIFLE_SIGHT_01, "Позволяет более точно наводиться на противника на дальних дистанциях."},
            {ItemType.MINISMG_CLIP_02, "Вмещает в себя большее количество патронов, совместимо с Мини ПП."}, // Clip
            {ItemType.PISTOL50_CLIP_02, "Вмещает в себя большее количество патронов, совместимо с Пистолетом .50"}, // Clip
            {ItemType.PISTOL50_VARMOD_LUXE, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.PISTOL_CLIP_02, "Вмещает в себя большее количество патронов, совместимо с Пистолетом."}, // Clip
            {ItemType.PISTOL_MK2_CAMO, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.PISTOL_MK2_CAMO_02, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.PISTOL_MK2_CAMO_02_SLIDE, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.PISTOL_MK2_CAMO_03, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.PISTOL_MK2_CAMO_03_SLIDE, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.PISTOL_MK2_CAMO_04, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.PISTOL_MK2_CAMO_04_SLIDE, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.PISTOL_MK2_CAMO_05, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.PISTOL_MK2_CAMO_05_SLIDE, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.PISTOL_MK2_CAMO_06, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.PISTOL_MK2_CAMO_06_SLIDE, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.PISTOL_MK2_CAMO_07, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.PISTOL_MK2_CAMO_07_SLIDE, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.PISTOL_MK2_CAMO_08, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.PISTOL_MK2_CAMO_08_SLIDE, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.PISTOL_MK2_CAMO_09, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.PISTOL_MK2_CAMO_09_SLIDE, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.PISTOL_MK2_CAMO_10, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.PISTOL_MK2_CAMO_10_SLIDE, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.PISTOL_MK2_CAMO_IND_01, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.PISTOL_MK2_CAMO_IND_01_SLIDE, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.PISTOL_MK2_CAMO_SLIDE, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.PISTOL_MK2_CLIP_02, "Вмещает в себя большее количество патронов, совместимо с Пистолетом MK2."}, // Clip
            {ItemType.PISTOL_MK2_CLIP_FMJ, "Пробивают броню автомобиля, защищенные шины и прочую броню."}, // Clip
            {ItemType.PISTOL_MK2_CLIP_HOLLOWPOINT, "Пули, конструкция которых предусматривает существенное увеличение диаметра при попадании во врага."}, // Clip
            {ItemType.PISTOL_MK2_CLIP_INCENDIARY, "Есть вероятность поджечь врага при попадании."}, // Clip
            {ItemType.PISTOL_MK2_CLIP_TRACER, "Делает траекторию снаряда видимой невооруженным глазом при дневном свете и очень яркой при ночной стрельбе."}, // Clip
            {ItemType.PISTOL_VARMOD_LUXE, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.PUMPSHOTGUN_MK2_CAMO, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.PUMPSHOTGUN_MK2_CAMO_02, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.PUMPSHOTGUN_MK2_CAMO_03, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.PUMPSHOTGUN_MK2_CAMO_04, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.PUMPSHOTGUN_MK2_CAMO_05, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.PUMPSHOTGUN_MK2_CAMO_06, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.PUMPSHOTGUN_MK2_CAMO_07, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.PUMPSHOTGUN_MK2_CAMO_08, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.PUMPSHOTGUN_MK2_CAMO_09, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.PUMPSHOTGUN_MK2_CAMO_10, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.PUMPSHOTGUN_MK2_CAMO_IND_01, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.PUMPSHOTGUN_MK2_CLIP_ARMORPIERCING, "Наносят больший урон врагам в броне."}, // Clip
            {ItemType.PUMPSHOTGUN_MK2_CLIP_EXPLOSIVE, "Взрывается при попадании во врага."}, // Clip
            {ItemType.PUMPSHOTGUN_MK2_CLIP_HOLLOWPOINT, "Пули, конструкция которых предусматривает существенное увеличение диаметра при попадании во врага."}, // Clip
            {ItemType.PUMPSHOTGUN_MK2_CLIP_INCENDIARY, "Есть вероятность поджечь врага при выстреле."}, // Clip
            {ItemType.PUMPSHOTGUN_VARMOD_LOWRIDER, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.RAYPISTOL_VARMOD_XMAS18, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.REVOLVER_MK2_CAMO, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.REVOLVER_MK2_CAMO_02, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.REVOLVER_MK2_CAMO_03, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.REVOLVER_MK2_CAMO_04, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.REVOLVER_MK2_CAMO_05, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.REVOLVER_MK2_CAMO_06, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.REVOLVER_MK2_CAMO_07, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.REVOLVER_MK2_CAMO_08, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.REVOLVER_MK2_CAMO_09, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.REVOLVER_MK2_CAMO_10, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.REVOLVER_MK2_CAMO_IND_01, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.REVOLVER_MK2_CLIP_FMJ, "Пробивают броню автомобиля, защищенные шины и прочую броню."}, // Clip
            {ItemType.REVOLVER_MK2_CLIP_HOLLOWPOINT, "Пули, конструкция которых предусматривает существенное увеличение диаметра при попадании во врага."}, // Clip
            {ItemType.REVOLVER_MK2_CLIP_INCENDIARY, "Есть вероятность поджечь врага при попадании."}, // Clip
            {ItemType.REVOLVER_MK2_CLIP_TRACER, "Делает траекторию снаряда видимой невооруженным глазом при дневном свете и очень яркой при ночной стрельбе."}, // Clip
            {ItemType.REVOLVER_VARMOD_BOSS, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.REVOLVER_VARMOD_GOON, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.SAWNOFFSHOTGUN_VARMOD_LUXE, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.SMG_CLIP_02, "Вмещает в себя большее количество патронов, совместимо с Пистолетом-Пулемётом."}, // Clip
            {ItemType.SMG_CLIP_03, "Вмещает в себя большее количество патронов, совместимо с Пистолетом-Пулемётом."}, // Clip
            {ItemType.SMG_MK2_CAMO, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.SMG_MK2_CAMO_02, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.SMG_MK2_CAMO_03, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.SMG_MK2_CAMO_04, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.SMG_MK2_CAMO_05, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.SMG_MK2_CAMO_06, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.SMG_MK2_CAMO_07, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.SMG_MK2_CAMO_08, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.SMG_MK2_CAMO_09, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.SMG_MK2_CAMO_10, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.SMG_MK2_CAMO_IND_01, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.SMG_MK2_CLIP_02, "Вмещает в себя большее количество патронов, совместимо с Пистолетом-Пулемётом MK2."}, // Clip
            {ItemType.SMG_MK2_CLIP_FMJ, "Пробивают броню автомобиля, защищенные шины и прочую броню."}, // Clip
            {ItemType.SMG_MK2_CLIP_HOLLOWPOINT, "Пули, конструкция которых предусматривает существенное увеличение диаметра при попадании во врага."}, // Clip
            {ItemType.SMG_MK2_CLIP_INCENDIARY, "Есть вероятность поджечь врага при попадании."}, // Clip
            {ItemType.SMG_MK2_CLIP_TRACER, "Делает траекторию снаряда видимой невооруженным глазом при дневном свете и очень яркой при ночной стрельбе."}, // Clip
            {ItemType.SMG_VARMOD_LUXE, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.SNIPERRIFLE_VARMOD_LUXE, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.SNSPISTOL_CLIP_02, "Вмещает в себя большее количество патронов, совместимо с Карманным пистолетом."}, // Clip
            {ItemType.SNSPISTOL_MK2_CAMO, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.SNSPISTOL_MK2_CAMO_02, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.SNSPISTOL_MK2_CAMO_02_SLIDE, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.SNSPISTOL_MK2_CAMO_03, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.SNSPISTOL_MK2_CAMO_03_SLIDE, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.SNSPISTOL_MK2_CAMO_04, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.SNSPISTOL_MK2_CAMO_04_SLIDE, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.SNSPISTOL_MK2_CAMO_05, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.SNSPISTOL_MK2_CAMO_05_SLIDE, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.SNSPISTOL_MK2_CAMO_06, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.SNSPISTOL_MK2_CAMO_06_SLIDE, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.SNSPISTOL_MK2_CAMO_07, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.SNSPISTOL_MK2_CAMO_07_SLIDE, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.SNSPISTOL_MK2_CAMO_08, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.SNSPISTOL_MK2_CAMO_08_SLIDE, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.SNSPISTOL_MK2_CAMO_09, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.SNSPISTOL_MK2_CAMO_09_SLIDE, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.SNSPISTOL_MK2_CAMO_10, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.SNSPISTOL_MK2_CAMO_10_SLIDE, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.SNSPISTOL_MK2_CAMO_IND_01, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.SNSPISTOL_MK2_CAMO_IND_01_SLIDE, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.SNSPISTOL_MK2_CAMO_SLIDE, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.SNSPISTOL_MK2_CLIP_02, "Вмещает в себя большее количество патронов, совместимо с Карманным пистолетом MK2."}, // Clip
            {ItemType.SNSPISTOL_MK2_CLIP_FMJ, "Пробивают броню автомобиля, защищенные шины и прочую броню."}, // Clip
            {ItemType.SNSPISTOL_MK2_CLIP_HOLLOWPOINT, "Пули, конструкция которых предусматривает существенное увеличение диаметра при попадании во врага."}, // Clip
            {ItemType.SNSPISTOL_MK2_CLIP_INCENDIARY, "Есть вероятность поджечь врага при попадании."}, // Clip
            {ItemType.SNSPISTOL_MK2_CLIP_TRACER, "Делает траекторию снаряда видимой невооруженным глазом при дневном свете и очень яркой при ночной стрельбе."}, // Clip
            {ItemType.SNSPISTOL_VARMOD_LOWRIDER, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.SPECIALCARBINE_CLIP_02, "Вмещает в себя 60 патронов, совместимо с Особым карабином."}, // Clip
            {ItemType.SPECIALCARBINE_CLIP_03, "Вмещает в себя 100 патронов, совместимо с Особым карабином."}, // Clip
            {ItemType.SPECIALCARBINE_MK2_CAMO, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.SPECIALCARBINE_MK2_CAMO_02, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.SPECIALCARBINE_MK2_CAMO_03, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.SPECIALCARBINE_MK2_CAMO_04, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.SPECIALCARBINE_MK2_CAMO_05, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.SPECIALCARBINE_MK2_CAMO_06, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.SPECIALCARBINE_MK2_CAMO_07, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.SPECIALCARBINE_MK2_CAMO_08, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.SPECIALCARBINE_MK2_CAMO_09, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.SPECIALCARBINE_MK2_CAMO_10, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.SPECIALCARBINE_MK2_CAMO_IND_01, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.SPECIALCARBINE_MK2_CLIP_02, "Вмещает в себя большее количество патронов, совместимо с Особым карабином MK2."}, // Clip
            {ItemType.SPECIALCARBINE_MK2_CLIP_ARMORPIERCING, "Наносят больший урон врагам в броне."}, // Clip
            {ItemType.SPECIALCARBINE_MK2_CLIP_FMJ, "Пробивают броню автомобиля, защищенные шины и прочую броню."}, // Clip
            {ItemType.SPECIALCARBINE_MK2_CLIP_INCENDIARY, "Есть вероятность поджечь врага при попадании."}, // Clip
            {ItemType.SPECIALCARBINE_MK2_CLIP_TRACER, "Делает траекторию снаряда видимой невооруженным глазом при дневном свете и очень яркой при ночной стрельбе."}, // Clip
            {ItemType.SPECIALCARBINE_VARMOD_LOWRIDER, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.SWITCHBLADE_VARMOD_VAR1, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.SWITCHBLADE_VARMOD_VAR2, "Поместив данный предмет в слот улучшения Вы поменяете расцветку своего оружия."}, // Texture
            {ItemType.VINTAGEPISTOL_CLIP_02, "Вмещает в себя большее количество патронов, совместимо с Винтажным пистолетом."}, // Clip

            {ItemType.DigScanner , "Простой металлодетектор с маленьким радиусом обнаружения сокровищ"},
            {ItemType.DigScanner_mk2 , "Продвинутый металлодетектор со средким радиусом обнаружения сокровищ"},
            {ItemType.DigScanner_mk3 , "Профессиональный металлодетектор с большим шансом обнаружения сокровищ"},

            {ItemType.DigShovel , "Ржавая старенькая лопата. Ей довольно тяжело копать"},
            {ItemType.DigShovel_mk2 , "Лопата обыкновенная, ей можно вполне шустро выкопать клад"},
            {ItemType.DigShovel_mk3 , "Профессиональная лопата, орудовать ей - сплошное удовольствие. Выкопать клад можно на раз"},

            {ItemType.CraftCap ,"Обычная крышка от какой-то бутылки"},
            {ItemType.CraftOldCoin , "Обычная старая монета"},
            {ItemType.CraftShell , "Пустая гильза от патрона"},
            {ItemType.CraftScrapMetal, "Смесь разных металлов"},
            {ItemType.CraftCopperNugget,  "Маленький кусочек меди"},
            {ItemType.CraftIronNugget, "Маленький кусочек железа"},
            {ItemType.CraftTinNugget, "Оловянный самородок"},

            {ItemType.CraftCopperWire, "Самая популярная вещь для продажи"},
            {ItemType.CraftOldJewerly, " Какое-то старое украшение, которое можно продать"},
            {ItemType.CraftGoldNugget, "Маленький кусочек золота, который можно продать по хорошей цене"},
            {ItemType.CraftСollectibleCoin, "Редкая монета, которую любят коллекционеры"},
            {ItemType.CraftAncientStatuette, "Эту странную статую можно продать за неплохие деньги"},
            {ItemType.CraftGoldHorseShoe, "И пусть вам повезет! Можете оставить на память или продать"},
            {ItemType.CraftRelic, "Эту вещицу надо хранить в музее! Ее можно продать за крупную сумму"},


            {ItemType.CraftIronPart, "Железная запчасть" },
            {ItemType.CraftCopperPart, "Медная запчасть" },
            {ItemType.CraftTinPart, "Оловянная запчасть" },
            {ItemType.CraftBronzePart, "Бронзовая запчасть" },

            {ItemType.CraftWorkBench, "Станок" },
            {ItemType.CraftPercolator, "Процеживатель" },
            {ItemType.CraftSmelter, "Плавильня" },
            {ItemType.CraftPartsCollector, "Сборщик детайлей" },
            {ItemType.CraftWorkBenchUpgrade, "Улучшение станка I" },
            {ItemType.CraftWorkBenchUpgrade2, "Улучшение станка II" },
        };

        public static Dictionary<ItemType, string> ItemFname = new Dictionary<ItemType, string>()
        {
            { ItemType.BagWithMoney, "Сумка с деньгами." },
            { ItemType.Material, "Материалы." },
            { ItemType.Drugs, "Наркотики." },
            { ItemType.BagWithDrill, "Сумка с дрелью." },
            { ItemType.Debug, "Дебаг." },
            { ItemType.Bandage, "Бинт" },
            { ItemType.HealthKit, "Аптечка первой помощи." },
            { ItemType.GasCan, "Канистра с топливом." },
            { ItemType.Сrisps, "Чипсы." },
            { ItemType.Beer, "Пиво." },
            { ItemType.Pizza, "Пицца." },
            { ItemType.Burger, "Бургер." },
            { ItemType.HotDog, "Хот-Дог." },
            { ItemType.Sandwich, "Сэндвич." },
            { ItemType.eCola, "eCola." },
            { ItemType.Sprunk, "Sprunk." },
            { ItemType.Lockpick, "Отмычка." },
            { ItemType.ArmyLockpick, "Военная отмычка." },
            { ItemType.Pocket, "Мешок." },
            { ItemType.Cuffs, "Стяжки." },
            { ItemType.CarKey, "Ключи от машины." },
            { ItemType.Present, "Подарок." },
            { ItemType.KeyRing, "Связка ключей." },
            //{ ItemType.Radio, "Радио." },

            { ItemType.Mask, "Маска." },
            { ItemType.Gloves, "Перчатки." },
            { ItemType.Leg, "Штаны." },
            { ItemType.Bag, "Рюкзак" },
            { ItemType.Bag1, "Рюкзак" },
            { ItemType.Feet, "Обувь." },
            { ItemType.Jewelry, "Ювелирные украшения." },
            { ItemType.Undershit, "Футболка" },
            { ItemType.BodyArmor, "Бронежилет." },
            { ItemType.BodyArmorgov1, "Государственный бронижелет 1-го типа" },
            { ItemType.BodyArmorgov2, "Государственный бронижелет 2-го типа" },
            { ItemType.BodyArmorgov3, "Государственный бронижелет 3-го типа" },
            { ItemType.BodyArmorgov4, "Государственный бронижелет 4-го типа" },
            { ItemType.Unknown, "Неизвестно." },
            { ItemType.Top, "Кофта." },
            { ItemType.Hat, "Кепка." },
            { ItemType.Glasses, "Очки." },
            { ItemType.Accessories, "Аксессуары." },
            { ItemType.RusDrink1, "Водка." },
            { ItemType.RusDrink2, "Напиток." },
            { ItemType.RusDrink3, "Напиток." },
            { ItemType.YakDrink1, "Напиток." },
            { ItemType.YakDrink2, "Напиток." },
            { ItemType.YakDrink3, "Напиток." },
            { ItemType.LcnDrink1, "Напиток." },
            { ItemType.LcnDrink2, "Напиток." },
            { ItemType.LcnDrink3, "Напиток." },
            { ItemType.ArmDrink1, "Напиток." },
            { ItemType.ArmDrink2, "Напиток." },
            { ItemType.ArmDrink3, "Напиток." },
            {ItemType.WaterBottle,  "Бутылка воды"},
            { ItemType.RepairBox, "Рем. набор"},
            { ItemType.SmallHealthKit, "Маленькая аптечка" },
            { ItemType.Pistol, "Пистолет" },
            { ItemType.Combatpistol, "Боевой пистолет" },
            { ItemType.Pistol50, "Пистолет .50" },
            { ItemType.Snspistol, "Карманный пистолет" },
            { ItemType.Heavypistol, "Тяжелый пистолет" },
            { ItemType.Vintagepistol, "Винтажный пистолет" },
            { ItemType.Marksmanpistol, "Пистоль" },
            { ItemType.Revolver, "Револьвер" },
            { ItemType.Appistol, "Бронебойный пистолет" },
            { ItemType.Stungun, "Шокер" },
            { ItemType.Flaregun, "Сигнальный пистолет" },
            { ItemType.Doubleaction, "Самовзодный револьвер" },
            { ItemType.Pistol_mk2, "Пистолет MK2" },
            { ItemType.Snspistol_mk2, "Карманный пистолет MK2" },
            { ItemType.Revolver_mk2, "Револьвер MK2" },
            { ItemType.Microsmg, "Микро ПП" },
            { ItemType.Machinepistol, "Малый ПП"},
            { ItemType.Smg, "Пистолет-Пулемёт" },
            { ItemType.Assaultsmg, "Штурмовой ПП" },
            { ItemType.Combatpdw, "ПОС" },
            { ItemType.Mg, "Пулемёт" },
            { ItemType.Combatmg, "Тяжелый пулемёт" },
            { ItemType.Gusenberg, "ПП Томпсона" },
            { ItemType.Minismg, "Мини ПП" },
            { ItemType.Smg_mk2, "Пистолет-Пулемёт MK2" },
            { ItemType.Combatmg_mk2, "Пулемёт MK2" },
            { ItemType.Assaultrifle, "Штурмовая винтовка" },
            { ItemType.Carbinerifle, "Автоматическая винтовка" },
            { ItemType.Advancedrifle, "Улучшенная винтовка" },
            { ItemType.Specialcarbine, "Особый карабин" },
            { ItemType.Bullpuprifle, "Винтовка-«Буллпап»" },
            { ItemType.Compactrifle, "Укороченная винтовка" },
            { ItemType.Assaultrifle_mk2, "Штурмовая винтовка MK2" },
            { ItemType.Carbinerifle_mk2, "Автоматическая винтовка MK2" },
            { ItemType.Specialcarbine_mk2, "Особый карабин MK2" },
            { ItemType.Bullpuprifle_mk2, "Винтовка-«Буллпап» MK2" },
            { ItemType.Sniperrifle,"Снайперская винтовка" },
            { ItemType.Heavysniper, "Тяжелая снайперская винтовка" },
            { ItemType.Marksmanrifle, "Высокоточная винтовка" },
            { ItemType.Heavysniper_mk2, "Тяжелая снайперская винтовка MK2" },
            { ItemType.Marksmanrifle_mk2, "Высокоточная винтовка MK2" },
            { ItemType.Militaryrifle, "Высокоточная винтовка" },
            { ItemType.Pumpshotgun, "Помповый дробовик" },
            { ItemType.Sawnoffshotgun, "Дробовик-Обрез" },
            { ItemType.Bullpupshotgun, "Дробовик-«Буллпап»" },
            { ItemType.Assaultshotgun, "Штурмовой дробовик" },
            { ItemType.Musket, "Мушкет" },
            { ItemType.Heavyshotgun, "Тяжелый дробовик" },
            { ItemType.Dbshotgun, "Двуствольный обрез" },
            { ItemType.Autoshotgun, "Автоматический дробовик" },
            { ItemType.Pumpshotgun_mk2, "Помповый дробовик MK2" },
            { ItemType.Rpg, "РПГ" },
            { ItemType.Knife, "Нож" },
            { ItemType.Nightstick, "Дубинка" },
            { ItemType.Hammer, "Молоток" },
            { ItemType.Bat, "Бейсбольная бита" },
            { ItemType.Crowbar, "Монтировка" },
            { ItemType.Golfclub, "Клюшка для гольфа" },
            { ItemType.Bottle, "Бутылка" },
            { ItemType.Dagger, "Кавалерийский кинжал" },
            { ItemType.Hatchet, "Топорик" },
            { ItemType.Knuckle, "Кастет" },
            { ItemType.Machete, "Мачете" },
            { ItemType.Flashlight, "Фонарик" },
            { ItemType.Switchblade, "Выкидной нож" },
            { ItemType.Poolcue, "Бильярдный кий" },
            { ItemType.Wrench, "Разводной ключ" },
            { ItemType.Battleaxe, "Боевой топор" },
            { ItemType.Rod, "Удочка" },
            { ItemType.RodMK2, "Удочка MK3" },
            { ItemType.RodUpgrade, "Удочка MK2" },
            { ItemType.Bait, "Наживка" },
            { ItemType.Naz, "Улучшенная наживка" },

            { ItemType.Kyndja, "Рыба: Кунджа"},
            { ItemType.Sig, "Рыба: Сиг"},
            { ItemType.Omyl, "Рыба: Омуль"},
            { ItemType.Nerka, "Рыба: Нерка"},
            { ItemType.Forel, "Рыба: Форель"},
            { ItemType.Ship,  "Рыба: Шип"},
            { ItemType.Lopatonos, "Рыба: Лопатонос"},
            { ItemType.Osetr, "Рыба: Осетр"},
            { ItemType.Semga, "Рыба: Семга"},
            { ItemType.Servyga, "Рыба: Сервюга"},
            { ItemType.Beluga, "Рыба: Белуга"},
            { ItemType.Taimen, "Рыба: Таймень"},
            { ItemType.Sterlyad, "Рыба: Стерлядь"},
            { ItemType.Ydilshik, "Рыба: Удильщик"},
            { ItemType.Hailiod, "Рыба: Хайлиод"},
            { ItemType.Keta, "Рыба: Кета"},
            { ItemType.Gorbysha, "Рыба: Горбуша"},
            //{ ItemType.GiveBox, "დონატ ქეისი" },
            //{ ItemType.Box1, "დონატ ნაკრები" },
            //{ ItemType.Box2, "დონატ ნაკრები" },
            //{ ItemType.Box3, "დონატ ნაკრები" },
            //{ ItemType.Box4, "დონატ ნაკრები" },
            //{ ItemType.CarLow, "donate66"},
            //{ ItemType.CarPremium, "donate77" },
            //{ ItemType.CarSport, "donate88" },
            { ItemType.PistolAmmo, "Патроны калибра 9.19" },
            { ItemType.RiflesAmmo, "Патроны калибра 7.62" },
            { ItemType.ShotgunsAmmo, "Патроны калибра Gauge.12" },
            { ItemType.SMGAmmo, "Патроны калибра 5.45" },
            { ItemType.SniperAmmo, "Патроны калибра .50" },
            //{ ItemType.Pistol_Ammo_Box, "Коробка с патронами калибра 9.19" },
            //{ ItemType.Rifle_Ammo_Box, "Коробка с патронами калибра 7.62" },
            //{ ItemType.Smg_Ammo_Box, "Коробка с патронами калибра 5.45" },
            //{ ItemType.Shotgun_Ammo_Box, "Коробка с патронами калибра Gauge.12" },
            //{ ItemType.Sniper_Ammo_Box, "Коробка с патронами калибра .50" },

            { ItemType.CasinoChips, "Фишки" },
            { ItemType.Grenade, "Граната" },
            { ItemType.LSPDDrone, "LSPD квадрокоптер" },
            { ItemType.Drone, "Квадрокоптер" },
            { ItemType.NumberPlate, "Номер автомобиля" },
            { ItemType.SimCard, "Сим-карта" },
            { ItemType.ProductBox, "Ящик продуктов" },
            { ItemType.TrashBag, "Пакет с мусором" },
            { ItemType.CocaLeaves, "Листья коки" },
            { ItemType.Cocaine, "Кокаин" },
            { ItemType.Weed, "Пачка травы" },
            { ItemType.DrugBookMark, "Закладка" },
            { ItemType.Subject, "Предмет" },

            // [Modification] Миша, Дима, надо прописать названия.

            {ItemType.ADVANCEDRIFLE_CLIP_02, "Расширенный магазин"},
            {ItemType.ADVANCEDRIFLE_VARMOD_LUXE, "Золотая расцветка"},
            {ItemType.APPISTOL_CLIP_02, "Расширенный магазин"},
            {ItemType.APPISTOL_VARMOD_LUXE, "Золотая расцветка"},
            {ItemType.ASSAULTRIFLE_CLIP_02, "Расширенный магазин"},
            {ItemType.ASSAULTRIFLE_CLIP_03, "Расширенный магазин"},
            {ItemType.ASSAULTRIFLE_MK2_CAMO, "Цифровой камуфляж"},
            {ItemType.ASSAULTRIFLE_MK2_CAMO_02, "Мазковый камуфляж"},
            {ItemType.ASSAULTRIFLE_MK2_CAMO_03, "Лесной камуфляж"},
            {ItemType.ASSAULTRIFLE_MK2_CAMO_04, "Череп"},
            {ItemType.ASSAULTRIFLE_MK2_CAMO_05, "Sessanta Nove"},
            {ItemType.ASSAULTRIFLE_MK2_CAMO_06, "Perseus"},
            {ItemType.ASSAULTRIFLE_MK2_CAMO_07, "Леопард"},
            {ItemType.ASSAULTRIFLE_MK2_CAMO_08, "Зебра"},
            {ItemType.ASSAULTRIFLE_MK2_CAMO_09, "Геометрический"},
            {ItemType.ASSAULTRIFLE_MK2_CAMO_10, "Boom!"},
            {ItemType.ASSAULTRIFLE_MK2_CAMO_IND_01, "Флаг США"},
            {ItemType.ASSAULTRIFLE_MK2_CLIP_02, "Расширенный магазин"},
            {ItemType.ASSAULTRIFLE_MK2_CLIP_ARMORPIERCING, "Бронебойные патроны"},
            {ItemType.ASSAULTRIFLE_MK2_CLIP_FMJ, "Патроны FMJ"},
            {ItemType.ASSAULTRIFLE_MK2_CLIP_INCENDIARY, "Зажигательные патроны"},
            {ItemType.ASSAULTRIFLE_MK2_CLIP_TRACER, "Трассирующие патроны"},
            {ItemType.ASSAULTRIFLE_VARMOD_LUXE, "Золотая расцветка"},
            {ItemType.ASSAULTSHOTGUN_CLIP_02, "Расширенный магазин"},
            {ItemType.ASSAULTSMG_CLIP_02, "Расширенный магазин"},
            {ItemType.ASSAULTSMG_VARMOD_LOWRIDER, "Золотая расцветка"},
            {ItemType.AT_AR_AFGRIP, "Рукоять"},
            {ItemType.AT_AR_AFGRIP_02, "Рукоять"},
            {ItemType.AT_AR_BARREL_02, "Улучшенный ствол"},
            {ItemType.AT_AR_FLSH, "Фонарик"},
            {ItemType.AT_AR_SUPP, "Глушитель"},
            {ItemType.AT_AR_SUPP_02, "Глушитель"},
            {ItemType.AT_BP_BARREL_02, "Улучшенный ствол"},
            {ItemType.AT_CR_BARREL_02, "Улучшенный ствол"},
            {ItemType.AT_MG_BARREL_02, "Улучшенный ствол"},
            {ItemType.AT_MRFL_BARREL_02, "Улучшенный ствол"},
            {ItemType.AT_MUZZLE_01, "Пламегаситель"},
            {ItemType.AT_MUZZLE_02, "Пламегаситель"},
            {ItemType.AT_MUZZLE_03, "Пламегаситель"},
            {ItemType.AT_MUZZLE_04, "Пламегаситель"},
            {ItemType.AT_MUZZLE_05, "Пламегаситель"},
            {ItemType.AT_MUZZLE_06, "Пламегаситель"},
            {ItemType.AT_MUZZLE_07, "Пламегаситель"},
            {ItemType.AT_MUZZLE_08, "Пламегаситель"},
            {ItemType.AT_MUZZLE_09, "Пламегаситель"},
            {ItemType.AT_PI_COMP, "Пламегаситель"},
            {ItemType.AT_PI_COMP_02, "Пламегаситель"},
            {ItemType.AT_PI_COMP_03, "Пламегаситель"},
            {ItemType.AT_PI_FLSH, "Фонарик"},
            {ItemType.AT_PI_FLSH_02, "Фонарик"},
            {ItemType.AT_PI_FLSH_03, "Фонарик"},
            {ItemType.AT_PI_RAIL, "Коллиматорный прицел"},
            {ItemType.AT_PI_RAIL_02, "Коллиматорный прицел"},
            {ItemType.AT_PI_SUPP, "Глушитель"},
            {ItemType.AT_PI_SUPP_02, "Глушитель"},
            {ItemType.AT_SB_BARREL_02, "Улучшенный ствол"},
            {ItemType.AT_SC_BARREL_02, "Улучшенный ствол"},
            {ItemType.AT_SCOPE_LARGE, "Стандартный снайперский прицел"},
            {ItemType.AT_SCOPE_LARGE_FIXED_ZOOM, "Стандартный снайперский фиксированный прицел"},
            {ItemType.AT_SCOPE_LARGE_FIXED_ZOOM_MK2, "Улучшенный фиксированный высокоточный прицел"},
            {ItemType.AT_SCOPE_LARGE_MK2, "Улучшенный высокоточный прицел"},
            {ItemType.AT_SCOPE_MACRO, "Коллиматорный прицел"},
            {ItemType.AT_SCOPE_MACRO_02, "Голографический коллиматорный прицел"},
            {ItemType.AT_SCOPE_MACRO_02_MK2, "Голографический коллиматорный прицел"},
            {ItemType.AT_SCOPE_MACRO_02_SMG_MK2, "Голографический коллиматорный прицел"},
            {ItemType.AT_SCOPE_MACRO_MK2, "Голографический коллиматорный прицел"},
            {ItemType.AT_SCOPE_MAX, "Улучшенный снайперский прицел"},
            {ItemType.AT_SCOPE_MEDIUM, "Голографический коллиматорный прицел"},
            {ItemType.AT_SCOPE_MEDIUM_MK2, "Голографический коллиматорный прицел"},
            {ItemType.AT_SCOPE_NV, "Прицел ночного видения"},
            {ItemType.AT_SCOPE_SMALL, "Коллиматорный прицел"},
            {ItemType.AT_SCOPE_SMALL_02, "Голографический коллиматорный прицел"},
            {ItemType.AT_SCOPE_SMALL_MK2, "Улучшенный голографический коллиматорный прицел"},
            {ItemType.AT_SCOPE_SMALL_SMG_MK2, "Улучшенный голографический коллиматорный прицел"},
            {ItemType.AT_SCOPE_THERMAL, "Прицел с тепловизором"},
            {ItemType.AT_SIGHTS, "Коллиматорный прицел"},
            {ItemType.AT_SIGHTS_SMG, "Коллиматорный прицел"},
            {ItemType.AT_SR_BARREL_02, "Улучшенный ствол"},
            {ItemType.AT_SR_SUPP, "Глушитель"},
            {ItemType.AT_SR_SUPP_03, "Глушитель"},
            {ItemType.BULLPUPRIFLE_CLIP_02, "Расширенный магазин"},
            {ItemType.BULLPUPRIFLE_MK2_CAMO, "Цифровой камуфляж"},
            {ItemType.BULLPUPRIFLE_MK2_CAMO_02, "Мазковый камуфляж"},
            {ItemType.BULLPUPRIFLE_MK2_CAMO_03, "Лесной камуфляж"},
            {ItemType.BULLPUPRIFLE_MK2_CAMO_04, "Череп"},
            {ItemType.BULLPUPRIFLE_MK2_CAMO_05, "Sessanta Nove"},
            {ItemType.BULLPUPRIFLE_MK2_CAMO_06, "Perseus"},
            {ItemType.BULLPUPRIFLE_MK2_CAMO_07, "Леопард"},
            {ItemType.BULLPUPRIFLE_MK2_CAMO_08, "Зебра"},
            {ItemType.BULLPUPRIFLE_MK2_CAMO_09, "Геометрический"},
            {ItemType.BULLPUPRIFLE_MK2_CAMO_10, "Boom!"},
            {ItemType.BULLPUPRIFLE_MK2_CAMO_IND_01, "Флаг США"},
            {ItemType.BULLPUPRIFLE_MK2_CLIP_02, "Расширенный магазин"},
            {ItemType.BULLPUPRIFLE_MK2_CLIP_ARMORPIERCING, "Бронебойные патроны"},
            {ItemType.BULLPUPRIFLE_MK2_CLIP_FMJ, "Патроны FMJ"},
            {ItemType.BULLPUPRIFLE_MK2_CLIP_INCENDIARY, "Зажигательные патроны"},
            {ItemType.BULLPUPRIFLE_MK2_CLIP_TRACER, "Трассирующие патроны"},
            {ItemType.BULLPUPRIFLE_VARMOD_LOW, "Золотая расцветка"},
            {ItemType.CARBINERIFLE_CLIP_02, "Расширенный магазин"},
            {ItemType.CARBINERIFLE_CLIP_03, "Расширенный магазин"},
            {ItemType.CARBINERIFLE_MK2_CAMO, "Цифровой камуфляж"},
            {ItemType.CARBINERIFLE_MK2_CAMO_02, "Мазковый камуфляж"},
            {ItemType.CARBINERIFLE_MK2_CAMO_03, "Лесной камуфляж"},
            {ItemType.CARBINERIFLE_MK2_CAMO_04, "Череп"},
            {ItemType.CARBINERIFLE_MK2_CAMO_05, "Sessanta Nove"},
            {ItemType.CARBINERIFLE_MK2_CAMO_06, "Perseus"},
            {ItemType.CARBINERIFLE_MK2_CAMO_07, "Леопард"},
            {ItemType.CARBINERIFLE_MK2_CAMO_08, "Зебра"},
            {ItemType.CARBINERIFLE_MK2_CAMO_09, "Геометрический"},
            {ItemType.CARBINERIFLE_MK2_CAMO_10, "Boom!"},
            {ItemType.CARBINERIFLE_MK2_CAMO_IND_01, "Флаг США"},
            {ItemType.CARBINERIFLE_MK2_CLIP_02, "Расширенный магазин"},
            {ItemType.CARBINERIFLE_MK2_CLIP_ARMORPIERCING, "Бронебойные патроны"},
            {ItemType.CARBINERIFLE_MK2_CLIP_FMJ, "Патроны FMJ"},
            {ItemType.CARBINERIFLE_MK2_CLIP_INCENDIARY, "Зажигательные патроны"},
            {ItemType.CARBINERIFLE_MK2_CLIP_TRACER, "Трассирующие патроны"},
            {ItemType.CARBINERIFLE_VARMOD_LUXE, "Золотая расцветка"},
            {ItemType.CERAMICPISTOL_CLIP_02, "Расширенный магазин"},
            {ItemType.CERAMICPISTOL_SUPP, "Глушитель"},
            {ItemType.COMBATMG_CLIP_02, "Расширенный магазин"},
            {ItemType.COMBATMG_MK2_CAMO, "Цифровой камуфляж"},
            {ItemType.COMBATMG_MK2_CAMO_02, "Мазковый камуфляж"},
            {ItemType.COMBATMG_MK2_CAMO_03, "Лесной камуфляж"},
            {ItemType.COMBATMG_MK2_CAMO_04, "Череп"},
            {ItemType.COMBATMG_MK2_CAMO_05, "Sessanta Nove"},
            {ItemType.COMBATMG_MK2_CAMO_06, "Perseus"},
            {ItemType.COMBATMG_MK2_CAMO_07, "Леопард"},
            {ItemType.COMBATMG_MK2_CAMO_08, "Зебра"},
            {ItemType.COMBATMG_MK2_CAMO_09, "Геометрический"},
            {ItemType.COMBATMG_MK2_CAMO_10, "Boom!"},
            {ItemType.COMBATMG_MK2_CAMO_IND_01, "Флаг США"},
            {ItemType.COMBATMG_MK2_CLIP_02, "Расширенный магазин"},
            {ItemType.COMBATMG_MK2_CLIP_ARMORPIERCING, "Бронебойные патроны"},
            {ItemType.COMBATMG_MK2_CLIP_FMJ, "Патроны FMJ"},
            {ItemType.COMBATMG_MK2_CLIP_INCENDIARY, "Зажигательные патроны"},
            {ItemType.COMBATMG_MK2_CLIP_TRACER, "Трассирующие патроны"},
            {ItemType.COMBATMG_VARMOD_LOWRIDER, "Расцветка с черепами"},
            {ItemType.COMBATPDW_CLIP_02, "Расширенный магазин"},
            {ItemType.COMBATPDW_CLIP_03, "Расширенный магазин"},
            {ItemType.COMBATPISTOL_CLIP_02, "Расширенный магазин"},
            {ItemType.COMBATPISTOL_VARMOD_LOWRIDER, "Золотая расцветка"},
            {ItemType.COMPACTRIFLE_CLIP_02, "Расширенный магазин"},
            {ItemType.COMPACTRIFLE_CLIP_03, "Расширенный магазин"},
            {ItemType.GUSENBERG_CLIP_02, "Расширенный магазин"},
            {ItemType.HEAVYPISTOL_CLIP_02, "Расширенный магазин"},
            {ItemType.HEAVYPISTOL_VARMOD_LUXE, "Серебристая расцветка"},
            {ItemType.HEAVYSHOTGUN_CLIP_02, "Расширенный магазин"},
            {ItemType.HEAVYSHOTGUN_CLIP_03, "Расширенный магазин"},
            {ItemType.HEAVYSNIPER_MK2_CAMO, "Цифровой камуфляж"},
            {ItemType.HEAVYSNIPER_MK2_CAMO_02, "Мазковый камуфляж"},
            {ItemType.HEAVYSNIPER_MK2_CAMO_03, "Лесной камуфляж"},
            {ItemType.HEAVYSNIPER_MK2_CAMO_04, "Череп"},
            {ItemType.HEAVYSNIPER_MK2_CAMO_05, "Sessanta Nove"},
            {ItemType.HEAVYSNIPER_MK2_CAMO_06, "Perseus"},
            {ItemType.HEAVYSNIPER_MK2_CAMO_07, "Леопард"},
            {ItemType.HEAVYSNIPER_MK2_CAMO_08, "Зебра"},
            {ItemType.HEAVYSNIPER_MK2_CAMO_09, "Геометрический"},
            {ItemType.HEAVYSNIPER_MK2_CAMO_10, "Boom!"},
            {ItemType.HEAVYSNIPER_MK2_CAMO_IND_01, "Флаг США"},
            {ItemType.HEAVYSNIPER_MK2_CLIP_02, "Расширенный магазин"},
            {ItemType.HEAVYSNIPER_MK2_CLIP_ARMORPIERCING, "Бронебойные патроны"},
            {ItemType.HEAVYSNIPER_MK2_CLIP_EXPLOSIVE, "Взрывные патроны"},
            {ItemType.HEAVYSNIPER_MK2_CLIP_FMJ, "Патроны FMJ"},
            {ItemType.HEAVYSNIPER_MK2_CLIP_INCENDIARY, "Зажигательные патроны"},
            {ItemType.KNUCKLE_VARMOD_BALLAS, "Расцветка Ballas"},
            {ItemType.KNUCKLE_VARMOD_DIAMOND, "Расцветка Diamond"},
            {ItemType.KNUCKLE_VARMOD_DOLLAR, "Расцветка Dollar"},
            {ItemType.KNUCKLE_VARMOD_HATE, "Расцветка Hate"},
            {ItemType.KNUCKLE_VARMOD_KING, "Расцветка King"},
            {ItemType.KNUCKLE_VARMOD_LOVE, "Расцветка Love"},
            {ItemType.KNUCKLE_VARMOD_PIMP, "Расцветка Pimp"},
            {ItemType.KNUCKLE_VARMOD_PLAYER, "Расцветка Player"},
            {ItemType.KNUCKLE_VARMOD_VAGOS, "Расцветка Vagos"},
            {ItemType.MACHINEPISTOL_CLIP_02, "Расширенный магазин"},
            {ItemType.MACHINEPISTOL_CLIP_03, "Расширенный магазин"},
            {ItemType.MARKSMANRIFLE_CLIP_02, "Расширенный магазин"},
            {ItemType.MARKSMANRIFLE_MK2_CAMO, "Цифровой камуфляж"},
            {ItemType.MARKSMANRIFLE_MK2_CAMO_02, "Мазковый камуфляж"},
            {ItemType.MARKSMANRIFLE_MK2_CAMO_03, "Лесной камуфляж"},
            {ItemType.MARKSMANRIFLE_MK2_CAMO_04, "Череп"},
            {ItemType.MARKSMANRIFLE_MK2_CAMO_05, "Sessanta Nove"},
            {ItemType.MARKSMANRIFLE_MK2_CAMO_06, "Perseus"},
            {ItemType.MARKSMANRIFLE_MK2_CAMO_07, "Леопард"},
            {ItemType.MARKSMANRIFLE_MK2_CAMO_08, "Зебра"},
            {ItemType.MARKSMANRIFLE_MK2_CAMO_09, "Геометрический"},
            {ItemType.MARKSMANRIFLE_MK2_CAMO_10, "Boom!"},
            {ItemType.MARKSMANRIFLE_MK2_CAMO_IND_01, "Флаг США"},
            {ItemType.MARKSMANRIFLE_MK2_CLIP_02, "Расширенный магазин"},
            {ItemType.MARKSMANRIFLE_MK2_CLIP_ARMORPIERCING, "Бронебойные патроны"},
            {ItemType.MARKSMANRIFLE_MK2_CLIP_FMJ, "Патроны FMJ"},
            {ItemType.MARKSMANRIFLE_MK2_CLIP_INCENDIARY, "Зажигательные патроны"},
            {ItemType.MARKSMANRIFLE_MK2_CLIP_TRACER, "Трассирующие патроны"},
            {ItemType.MARKSMANRIFLE_VARMOD_LUXE, "Золотая расцветка"},
            {ItemType.MG_CLIP_02, "Расширенный магазин"},
            {ItemType.MG_VARMOD_LOWRIDER, "Золотая расцветка"},
            {ItemType.MICROSMG_CLIP_02, "Расширенный магазин"},
            {ItemType.MICROSMG_VARMOD_LUXE, "Золотая расцветка"},
            {ItemType.MILITARYRIFLE_CLIP_02, "Расширенный магазин"},
            {ItemType.MILITARYRIFLE_SIGHT_01, "Стандартный прицел"},
            {ItemType.MINISMG_CLIP_02, "Расширенный магазин"},
            {ItemType.PISTOL50_CLIP_02, "Расширенный магазин"},
            {ItemType.PISTOL50_VARMOD_LUXE, "Платиновая расцветка"},
            {ItemType.PISTOL_CLIP_02, "Расширенный магазин"},
            {ItemType.PISTOL_MK2_CAMO, "Цифровой камуфляж"},
            {ItemType.PISTOL_MK2_CAMO_02, "Мазковый камуфляж"},
            {ItemType.PISTOL_MK2_CAMO_02_SLIDE, "Мазковый камуфляж S"},
            {ItemType.PISTOL_MK2_CAMO_03, "Лесной камуфляж"},
            {ItemType.PISTOL_MK2_CAMO_03_SLIDE, "Лесной камуфляж S"},
            {ItemType.PISTOL_MK2_CAMO_04, "Череп"},
            {ItemType.PISTOL_MK2_CAMO_04_SLIDE, "Череп S"},
            {ItemType.PISTOL_MK2_CAMO_05, "Sessanta Nove"},
            {ItemType.PISTOL_MK2_CAMO_05_SLIDE, "Sessanta Nove S"},
            {ItemType.PISTOL_MK2_CAMO_06, "Perseus"},
            {ItemType.PISTOL_MK2_CAMO_06_SLIDE, "Perseus S"},
            {ItemType.PISTOL_MK2_CAMO_07, "Леопард"},
            {ItemType.PISTOL_MK2_CAMO_07_SLIDE, "Леопард S"},
            {ItemType.PISTOL_MK2_CAMO_08, "Зебра"},
            {ItemType.PISTOL_MK2_CAMO_08_SLIDE, "Зебра S"},
            {ItemType.PISTOL_MK2_CAMO_09, "Геометрический"},
            {ItemType.PISTOL_MK2_CAMO_09_SLIDE, "Геометрический S"},
            {ItemType.PISTOL_MK2_CAMO_10, "Boom!"},
            {ItemType.PISTOL_MK2_CAMO_10_SLIDE, "Boom! S"},
            {ItemType.PISTOL_MK2_CAMO_IND_01, "Флаг США"},
            {ItemType.PISTOL_MK2_CAMO_IND_01_SLIDE, "Флаг США S"},
            {ItemType.PISTOL_MK2_CAMO_SLIDE, "Цифровой камуфляж S"},
            {ItemType.PISTOL_MK2_CLIP_02, "Расширенный магазин"},
            {ItemType.PISTOL_MK2_CLIP_FMJ, "Патроны FMJ"},
            {ItemType.PISTOL_MK2_CLIP_HOLLOWPOINT, "Экспансивные патроны"},
            {ItemType.PISTOL_MK2_CLIP_INCENDIARY, "Зажигательные патроны"},
            {ItemType.PISTOL_MK2_CLIP_TRACER, "Трассирующие патроны"},
            {ItemType.PISTOL_VARMOD_LUXE, "Золотая расцветка"},
            {ItemType.PUMPSHOTGUN_MK2_CAMO, "Цифровой камуфляж"},
            {ItemType.PUMPSHOTGUN_MK2_CAMO_02, "Мазковый камуфляж"},
            {ItemType.PUMPSHOTGUN_MK2_CAMO_03, "Лесной камуфляж"},
            {ItemType.PUMPSHOTGUN_MK2_CAMO_04, "Череп"},
            {ItemType.PUMPSHOTGUN_MK2_CAMO_05, "Sessanta Nove"},
            {ItemType.PUMPSHOTGUN_MK2_CAMO_06, "Perseus"},
            {ItemType.PUMPSHOTGUN_MK2_CAMO_07, "Леопард"},
            {ItemType.PUMPSHOTGUN_MK2_CAMO_08, "Зебра"},
            {ItemType.PUMPSHOTGUN_MK2_CAMO_09, "Геометрический"},
            {ItemType.PUMPSHOTGUN_MK2_CAMO_10, "Boom!"},
            {ItemType.PUMPSHOTGUN_MK2_CAMO_IND_01, "Флаг США"},
            {ItemType.PUMPSHOTGUN_MK2_CLIP_ARMORPIERCING, "Бронебойные патроны"},
            {ItemType.PUMPSHOTGUN_MK2_CLIP_EXPLOSIVE, "Взрывные патроны"},
            {ItemType.PUMPSHOTGUN_MK2_CLIP_HOLLOWPOINT, "Экспансивные патроны"},
            {ItemType.PUMPSHOTGUN_MK2_CLIP_INCENDIARY, "Зажигательные патроны"},
            {ItemType.PUMPSHOTGUN_VARMOD_LOWRIDER, "Золотая расцветка"},
            {ItemType.RAYPISTOL_VARMOD_XMAS18, "Снежная расцветка"},
            {ItemType.REVOLVER_MK2_CAMO, "Цифровой камуфляж"},
            {ItemType.REVOLVER_MK2_CAMO_02, "Мазковый камуфляж"},
            {ItemType.REVOLVER_MK2_CAMO_03, "Лесной камуфляж"},
            {ItemType.REVOLVER_MK2_CAMO_04, "Череп"},
            {ItemType.REVOLVER_MK2_CAMO_05, "Sessanta Nove"},
            {ItemType.REVOLVER_MK2_CAMO_06, "Perseus"},
            {ItemType.REVOLVER_MK2_CAMO_07, "Леопард"},
            {ItemType.REVOLVER_MK2_CAMO_08, "Зебра"},
            {ItemType.REVOLVER_MK2_CAMO_09, "Геометрический"},
            {ItemType.REVOLVER_MK2_CAMO_10, "Boom!"},
            {ItemType.REVOLVER_MK2_CAMO_IND_01, "Флаг США"},
            {ItemType.REVOLVER_MK2_CLIP_FMJ, "Патроны FMJ"},
            {ItemType.REVOLVER_MK2_CLIP_HOLLOWPOINT, "Экспансивные патроны"},
            {ItemType.REVOLVER_MK2_CLIP_INCENDIARY, "Зажигательные патроны"},
            {ItemType.REVOLVER_MK2_CLIP_TRACER, "Трассирующие патроны"},
            {ItemType.REVOLVER_VARMOD_BOSS, "Расцветка BOSS"},
            {ItemType.REVOLVER_VARMOD_GOON, "РАсцветка GOON"},
            {ItemType.SAWNOFFSHOTGUN_VARMOD_LUXE, "Золотая расцветка"},
            {ItemType.SMG_CLIP_02, "Расширенный магазин"},
            {ItemType.SMG_CLIP_03, "Расширенный магазин"},
            {ItemType.SMG_MK2_CAMO, "Цифровой камуфляж"},
            {ItemType.SMG_MK2_CAMO_02, "Мазковый камуфляж"},
            {ItemType.SMG_MK2_CAMO_03, "Лесной камуфляж"},
            {ItemType.SMG_MK2_CAMO_04, "Череп"},
            {ItemType.SMG_MK2_CAMO_05, "Sessanta Nove"},
            {ItemType.SMG_MK2_CAMO_06, "Perseus"},
            {ItemType.SMG_MK2_CAMO_07, "Леопард"},
            {ItemType.SMG_MK2_CAMO_08, "Зебра"},
            {ItemType.SMG_MK2_CAMO_09, "Геометрический"},
            {ItemType.SMG_MK2_CAMO_10, "Boom!"},
            {ItemType.SMG_MK2_CAMO_IND_01, "Флаг США"},
            {ItemType.SMG_MK2_CLIP_02, "Расширенный магазин"},
            {ItemType.SMG_MK2_CLIP_FMJ, "Патроны FMJ"},
            {ItemType.SMG_MK2_CLIP_HOLLOWPOINT, "Экспансивные патроны"},
            {ItemType.SMG_MK2_CLIP_INCENDIARY, "Зажигательные патроны"},
            {ItemType.SMG_MK2_CLIP_TRACER, "Трассирующие патроны"},
            {ItemType.SMG_VARMOD_LUXE, "Золотая расцветка"},
            {ItemType.SNIPERRIFLE_VARMOD_LUXE, "Уникальная белая расцветка"},
            {ItemType.SNSPISTOL_CLIP_02, "Расширенный магазин"},
            {ItemType.SNSPISTOL_MK2_CAMO, "Цифровой камуфляж"},
            {ItemType.SNSPISTOL_MK2_CAMO_02, "Мазковый камуфляж"},
            {ItemType.SNSPISTOL_MK2_CAMO_02_SLIDE, "Мазковый камуфляж S"},
            {ItemType.SNSPISTOL_MK2_CAMO_03, "Лесной камуфляж"},
            {ItemType.SNSPISTOL_MK2_CAMO_03_SLIDE, "Лесной камуфляж S"},
            {ItemType.SNSPISTOL_MK2_CAMO_04, "Череп"},
            {ItemType.SNSPISTOL_MK2_CAMO_04_SLIDE, "Череп S"},
            {ItemType.SNSPISTOL_MK2_CAMO_05, "Sessanta Nove"},
            {ItemType.SNSPISTOL_MK2_CAMO_05_SLIDE, "Sessanta Nove S"},
            {ItemType.SNSPISTOL_MK2_CAMO_06, "Perseus"},
            {ItemType.SNSPISTOL_MK2_CAMO_06_SLIDE, "Perseus S"},
            {ItemType.SNSPISTOL_MK2_CAMO_07, "Леопард"},
            {ItemType.SNSPISTOL_MK2_CAMO_07_SLIDE, "Леопард S"},
            {ItemType.SNSPISTOL_MK2_CAMO_08, "Зебра"},
            {ItemType.SNSPISTOL_MK2_CAMO_08_SLIDE, "Зебра S"},
            {ItemType.SNSPISTOL_MK2_CAMO_09, "Геометрический"},
            {ItemType.SNSPISTOL_MK2_CAMO_09_SLIDE, "Геометрический S"},
            {ItemType.SNSPISTOL_MK2_CAMO_10, "Boom!"},
            {ItemType.SNSPISTOL_MK2_CAMO_10_SLIDE, "Boom! S"},
            {ItemType.SNSPISTOL_MK2_CAMO_IND_01, "Флаг США"},
            {ItemType.SNSPISTOL_MK2_CAMO_IND_01_SLIDE, "Флаг США S"},
            {ItemType.SNSPISTOL_MK2_CAMO_SLIDE, "Цифровой камуфляж S"},
            {ItemType.SNSPISTOL_MK2_CLIP_02, "Расширенный магазин"},
            {ItemType.SNSPISTOL_MK2_CLIP_FMJ, "Патроны FMJ"},
            {ItemType.SNSPISTOL_MK2_CLIP_HOLLOWPOINT, "Экспансивные патроны"},
            {ItemType.SNSPISTOL_MK2_CLIP_INCENDIARY, "Зажигательные патроны"},
            {ItemType.SNSPISTOL_MK2_CLIP_TRACER, "Трассирующие патроны"},
            {ItemType.SNSPISTOL_VARMOD_LOWRIDER, "Древесная отделка"},
            {ItemType.SPECIALCARBINE_CLIP_02, "Расширенный магазин"},
            {ItemType.SPECIALCARBINE_CLIP_03, "Расширенный магазин"},
            {ItemType.SPECIALCARBINE_MK2_CAMO, "Цифровой камуфляж"},
            {ItemType.SPECIALCARBINE_MK2_CAMO_02, "Мазковый камуфляж"},
            {ItemType.SPECIALCARBINE_MK2_CAMO_03, "Лесной камуфляж"},
            {ItemType.SPECIALCARBINE_MK2_CAMO_04, "Череп"},
            {ItemType.SPECIALCARBINE_MK2_CAMO_05, "Sessanta Nove"},
            {ItemType.SPECIALCARBINE_MK2_CAMO_06, "Perseus"},
            {ItemType.SPECIALCARBINE_MK2_CAMO_07, "Леопард"},
            {ItemType.SPECIALCARBINE_MK2_CAMO_08, "Зебра"},
            {ItemType.SPECIALCARBINE_MK2_CAMO_09, "Геометрический"},
            {ItemType.SPECIALCARBINE_MK2_CAMO_10, "Boom!"},
            {ItemType.SPECIALCARBINE_MK2_CAMO_IND_01, "Флаг США"},
            {ItemType.SPECIALCARBINE_MK2_CLIP_02, "Расширенный магазин"},
            {ItemType.SPECIALCARBINE_MK2_CLIP_ARMORPIERCING, "Бронебойные патроны"},
            {ItemType.SPECIALCARBINE_MK2_CLIP_FMJ, "Патроны FMJ"},
            {ItemType.SPECIALCARBINE_MK2_CLIP_INCENDIARY, "Зажигательные патроны"},
            {ItemType.SPECIALCARBINE_MK2_CLIP_TRACER, "Трассирующие патроны"},
            {ItemType.SPECIALCARBINE_VARMOD_LOWRIDER, "Расцветка с черепами"},
            {ItemType.SWITCHBLADE_VARMOD_VAR1, "Золотая расцветка"},
            {ItemType.SWITCHBLADE_VARMOD_VAR2, "Расцветка с черепом"},
            {ItemType.VINTAGEPISTOL_CLIP_02, "Расширенный магазин"},

            {ItemType.DigScanner , "Металодетектор"},
            {ItemType.DigScanner_mk2 , "Металодетектор MK2"},
            {ItemType.DigScanner_mk3 , "Металодетектор MK3"},

            {ItemType.DigShovel , "Лопата"},
            {ItemType.DigShovel_mk2 , "Лопата MK2"},
            {ItemType.DigShovel_mk3 , "Лопата MK3"},

            {ItemType.CraftCap ,"Крышка"},
            {ItemType.CraftOldCoin , "Старая монета"},
            {ItemType.CraftShell , "Гильза"},
            {ItemType.CraftScrapMetal, "Металлолом"},
            {ItemType.CraftCopperNugget,  "Медный самородок"},
            {ItemType.CraftIronNugget, "Железный самородок"},
            {ItemType.CraftTinNugget, "Оловянный самородок"},

            {ItemType.CraftCopperWire, "Медный провод"},
            {ItemType.CraftOldJewerly, "Старое украшение"},
            {ItemType.CraftGoldNugget, "Золотой самородок"},
            {ItemType.CraftСollectibleCoin, "Коллекционная монета"},
            {ItemType.CraftAncientStatuette, "Древняя статуэтка"},
            {ItemType.CraftGoldHorseShoe, "Золотая подкова"},
            {ItemType.CraftRelic, "Реликвия"},

            {ItemType.CraftIronPart, "Железная запчасть" },
            {ItemType.CraftCopperPart, "Медная запчасть" },
            {ItemType.CraftTinPart, "Оловянная запчасть" },
            {ItemType.CraftBronzePart, "Бронзовая запчасть" },

            {ItemType.CraftWorkBench, "Станок" },
            {ItemType.CraftPercolator, "Процеживатель" },
            {ItemType.CraftSmelter, "Плавильня" },
            {ItemType.CraftPartsCollector, "Сборщик детайлей" },
            {ItemType.CraftWorkBenchUpgrade, "Улучшение станка I" },
            {ItemType.CraftWorkBenchUpgrade2, "Улучшение станка II" },
        };

        public static Dictionary<string, ItemType> ComponentsType = new Dictionary<string, ItemType>()
        {
            // [Modification]
            {"0x8EC1C979", ItemType.ADVANCEDRIFLE_CLIP_02},
            {"0x377CD377", ItemType.ADVANCEDRIFLE_VARMOD_LUXE},
            {"0x249A17D5", ItemType.APPISTOL_CLIP_02},
            {"0x9B76C72C", ItemType.APPISTOL_VARMOD_LUXE},
            {"0xB1214F9B", ItemType.ASSAULTRIFLE_CLIP_02},
            {"0xDBF0A53D", ItemType.ASSAULTRIFLE_CLIP_03},
            {"0x911B24AF", ItemType.ASSAULTRIFLE_MK2_CAMO},
            {"0x37E5444B", ItemType.ASSAULTRIFLE_MK2_CAMO_02},
            {"0x538B7B97", ItemType.ASSAULTRIFLE_MK2_CAMO_03},
            {"0x25789F72", ItemType.ASSAULTRIFLE_MK2_CAMO_04},
            {"0xC5495F2D", ItemType.ASSAULTRIFLE_MK2_CAMO_05},
            {"0xCF8B73B1", ItemType.ASSAULTRIFLE_MK2_CAMO_06},
            {"0xA9BB2811", ItemType.ASSAULTRIFLE_MK2_CAMO_07},
            {"0xFC674D54", ItemType.ASSAULTRIFLE_MK2_CAMO_08},
            {"0x7C7FCD9B", ItemType.ASSAULTRIFLE_MK2_CAMO_09},
            {"0xA5C38392", ItemType.ASSAULTRIFLE_MK2_CAMO_10},
            {"0xB9B15DB0", ItemType.ASSAULTRIFLE_MK2_CAMO_IND_01},
            {"0xD12ACA6F", ItemType.ASSAULTRIFLE_MK2_CLIP_02},
            {"0xA7DD1E58", ItemType.ASSAULTRIFLE_MK2_CLIP_ARMORPIERCING},
            {"0x63E0A098", ItemType.ASSAULTRIFLE_MK2_CLIP_FMJ},
            {"0xFB70D853", ItemType.ASSAULTRIFLE_MK2_CLIP_INCENDIARY},
            {"0xEF2C78C1", ItemType.ASSAULTRIFLE_MK2_CLIP_TRACER},
            {"0x4EAD7533", ItemType.ASSAULTRIFLE_VARMOD_LUXE},
            {"0x86BD7F72", ItemType.ASSAULTSHOTGUN_CLIP_02},
            {"0xBB46E417", ItemType.ASSAULTSMG_CLIP_02},
            {"0x278C78AF", ItemType.ASSAULTSMG_VARMOD_LOWRIDER},
            {"0xC164F53", ItemType.AT_AR_AFGRIP},
            {"0x9D65907A", ItemType.AT_AR_AFGRIP_02},
            {"0x5646C26A", ItemType.AT_AR_BARREL_02},
            {"0x7BC4CDDC", ItemType.AT_AR_FLSH},
            {"0x837445AA", ItemType.AT_AR_SUPP},
            {"0xA73D4664", ItemType.AT_AR_SUPP_02},
            {"0x3BF26DC7", ItemType.AT_BP_BARREL_02},
            {"0x8B3C480B", ItemType.AT_CR_BARREL_02},
            {"0xB5E2575B", ItemType.AT_MG_BARREL_02},
            {"0x68373DDC", ItemType.AT_MRFL_BARREL_02},
            {"0xB99402D4", ItemType.AT_MUZZLE_01},
            {"0xC867A07B", ItemType.AT_MUZZLE_02},
            {"0xDE11CBCF", ItemType.AT_MUZZLE_03},
            {"0xEC9068CC", ItemType.AT_MUZZLE_04},
            {"0x2E7957A", ItemType.AT_MUZZLE_05},
            {"0x347EF8AC", ItemType.AT_MUZZLE_06},
            {"0x4DB62ABE", ItemType.AT_MUZZLE_07},
            {"0x5F7DCE4D", ItemType.AT_MUZZLE_08},
            {"0x6927E1A1", ItemType.AT_MUZZLE_09},
            {"0x21E34793", ItemType.AT_PI_COMP},
            {"0xAA8283BF", ItemType.AT_PI_COMP_02},
            {"0x27077CCB", ItemType.AT_PI_COMP_03},
            {"0x359B7AAE", ItemType.AT_PI_FLSH},
            {"0x43FD595B", ItemType.AT_PI_FLSH_02},
            {"0x4A4965F3", ItemType.AT_PI_FLSH_03},
            {"0x8ED4BB70", ItemType.AT_PI_RAIL},
            {"0x47DE9258", ItemType.AT_PI_RAIL_02},
            {"0xC304849A", ItemType.AT_PI_SUPP},
            {"0x65EA7EBB", ItemType.AT_PI_SUPP_02},
            {"0xA564D78B", ItemType.AT_SB_BARREL_02},
            {"0xF97F783B", ItemType.AT_SC_BARREL_02},
            {"0xD2443DDC", ItemType.AT_SCOPE_LARGE},
            {"0x1C221B1A", ItemType.AT_SCOPE_LARGE_FIXED_ZOOM},
            {"0x5B1C713C", ItemType.AT_SCOPE_LARGE_FIXED_ZOOM_MK2},
            {"0x82C10383", ItemType.AT_SCOPE_LARGE_MK2},
            {"0x9D2FBF29", ItemType.AT_SCOPE_MACRO},
            {"0x3CC6BA57", ItemType.AT_SCOPE_MACRO_02},
            {"0xC7ADD105", ItemType.AT_SCOPE_MACRO_02_MK2},
            {"0xE502AB6B", ItemType.AT_SCOPE_MACRO_02_SMG_MK2},
            {"0x49B2945", ItemType.AT_SCOPE_MACRO_MK2},
            {"0xBC54DA77", ItemType.AT_SCOPE_MAX},
            {"0xA0D89C42", ItemType.AT_SCOPE_MEDIUM},
            {"0xC66B6542", ItemType.AT_SCOPE_MEDIUM_MK2},
            {"0xB68010B0", ItemType.AT_SCOPE_NV},
            {"0xAA2C45B4", ItemType.AT_SCOPE_SMALL},
            {"0x3C00AFED", ItemType.AT_SCOPE_SMALL_02},
            {"0x3F3C8181", ItemType.AT_SCOPE_SMALL_MK2},
            {"0x3DECC7DA", ItemType.AT_SCOPE_SMALL_SMG_MK2},
            {"0x2E43DA41", ItemType.AT_SCOPE_THERMAL},
            {"0x420FD713", ItemType.AT_SIGHTS},
            {"0x9FDB5652", ItemType.AT_SIGHTS_SMG},
            {"0x108AB09E", ItemType.AT_SR_BARREL_02},
            {"0xE608B35E", ItemType.AT_SR_SUPP},
            {"0xAC42DF71", ItemType.AT_SR_SUPP_03},
            {"0xB3688B0F", ItemType.BULLPUPRIFLE_CLIP_02},
            {"0xAE4055B7", ItemType.BULLPUPRIFLE_MK2_CAMO},
            {"0xB905ED6B", ItemType.BULLPUPRIFLE_MK2_CAMO_02},
            {"0xA6C448E8", ItemType.BULLPUPRIFLE_MK2_CAMO_03},
            {"0x9486246C", ItemType.BULLPUPRIFLE_MK2_CAMO_04},
            {"0x8A390FD2", ItemType.BULLPUPRIFLE_MK2_CAMO_05},
            {"0x2337FC5", ItemType.BULLPUPRIFLE_MK2_CAMO_06},
            {"0xEFFFDB5E", ItemType.BULLPUPRIFLE_MK2_CAMO_07},
            {"0xDDBDB6DA", ItemType.BULLPUPRIFLE_MK2_CAMO_08},
            {"0xCB631225", ItemType.BULLPUPRIFLE_MK2_CAMO_09},
            {"0xA87D541E", ItemType.BULLPUPRIFLE_MK2_CAMO_10},
            {"0xC5E9AE52", ItemType.BULLPUPRIFLE_MK2_CAMO_IND_01},
            {"0xEFB00628", ItemType.BULLPUPRIFLE_MK2_CLIP_02},
            {"0xFAA7F5ED", ItemType.BULLPUPRIFLE_MK2_CLIP_ARMORPIERCING},
            {"0x43621710", ItemType.BULLPUPRIFLE_MK2_CLIP_FMJ},
            {"0xA99CF95A", ItemType.BULLPUPRIFLE_MK2_CLIP_INCENDIARY},
            {"0x822060A9", ItemType.BULLPUPRIFLE_MK2_CLIP_TRACER},
            {"0xA857BC78", ItemType.BULLPUPRIFLE_VARMOD_LOW},
            {"0x91109691", ItemType.CARBINERIFLE_CLIP_02},
            {"0xBA62E935", ItemType.CARBINERIFLE_CLIP_03},
            {"0x4BDD6F16", ItemType.CARBINERIFLE_MK2_CAMO},
            {"0x406A7908", ItemType.CARBINERIFLE_MK2_CAMO_02},
            {"0x2F3856A4", ItemType.CARBINERIFLE_MK2_CAMO_03},
            {"0xE50C424D", ItemType.CARBINERIFLE_MK2_CAMO_04},
            {"0xD37D1F2F", ItemType.CARBINERIFLE_MK2_CAMO_05},
            {"0x86268483", ItemType.CARBINERIFLE_MK2_CAMO_06},
            {"0xF420E076", ItemType.CARBINERIFLE_MK2_CAMO_07},
            {"0xAAE14DF8", ItemType.CARBINERIFLE_MK2_CAMO_08},
            {"0x9893A95D", ItemType.CARBINERIFLE_MK2_CAMO_09},
            {"0x6B13CD3E", ItemType.CARBINERIFLE_MK2_CAMO_10},
            {"0xDA55CD3F", ItemType.CARBINERIFLE_MK2_CAMO_IND_01},
            {"0x5DD5DBD5", ItemType.CARBINERIFLE_MK2_CLIP_02},
            {"0x255D5D57", ItemType.CARBINERIFLE_MK2_CLIP_ARMORPIERCING},
            {"0x44032F11", ItemType.CARBINERIFLE_MK2_CLIP_FMJ},
            {"0x3D25C2A7", ItemType.CARBINERIFLE_MK2_CLIP_INCENDIARY},
            {"0x1757F566", ItemType.CARBINERIFLE_MK2_CLIP_TRACER},
            {"0xD89B9658", ItemType.CARBINERIFLE_VARMOD_LUXE},
            {"0x81786CA9", ItemType.CERAMICPISTOL_CLIP_02},
            {"0x9307D6FA", ItemType.CERAMICPISTOL_SUPP},
            {"0xD6C59CD6", ItemType.COMBATMG_CLIP_02},
            {"0x4A768CB5", ItemType.COMBATMG_MK2_CAMO},
            {"0xCCE06BBD", ItemType.COMBATMG_MK2_CAMO_02},
            {"0xBE94CF26", ItemType.COMBATMG_MK2_CAMO_03},
            {"0x7609BE11", ItemType.COMBATMG_MK2_CAMO_04},
            {"0x48AF6351", ItemType.COMBATMG_MK2_CAMO_05},
            {"0x9186750A", ItemType.COMBATMG_MK2_CAMO_06},
            {"0x84555AA8", ItemType.COMBATMG_MK2_CAMO_07},
            {"0x1B4C088B", ItemType.COMBATMG_MK2_CAMO_08},
            {"0xE046DFC", ItemType.COMBATMG_MK2_CAMO_09},
            {"0x28B536E", ItemType.COMBATMG_MK2_CAMO_10},
            {"0xD703C94D", ItemType.COMBATMG_MK2_CAMO_IND_01},
            {"0x17DF42E9", ItemType.COMBATMG_MK2_CLIP_02},
            {"0x29882423", ItemType.COMBATMG_MK2_CLIP_ARMORPIERCING},
            {"0x57EF1CC8", ItemType.COMBATMG_MK2_CLIP_FMJ},
            {"0xC326BDBA", ItemType.COMBATMG_MK2_CLIP_INCENDIARY},
            {"0xF6649745", ItemType.COMBATMG_MK2_CLIP_TRACER},
            {"0x92FECCDD", ItemType.COMBATMG_VARMOD_LOWRIDER},
            {"0x334A5203", ItemType.COMBATPDW_CLIP_02},
            {"0x6EB8C8DB", ItemType.COMBATPDW_CLIP_03},
            {"0xD67B4F2D", ItemType.COMBATPISTOL_CLIP_02},
            {"0xC6654D72", ItemType.COMBATPISTOL_VARMOD_LOWRIDER},
            {"0x59FF9BF8", ItemType.COMPACTRIFLE_CLIP_02},
            {"0xC607740E", ItemType.COMPACTRIFLE_CLIP_03},
            {"0xEAC8C270", ItemType.GUSENBERG_CLIP_02},
            {"0x64F9C62B", ItemType.HEAVYPISTOL_CLIP_02},
            {"0x7A6A7B7B", ItemType.HEAVYPISTOL_VARMOD_LUXE},
            {"0x971CF6FD", ItemType.HEAVYSHOTGUN_CLIP_02},
            {"0x88C7DA53", ItemType.HEAVYSHOTGUN_CLIP_03},
            {"0xF8337D02", ItemType.HEAVYSNIPER_MK2_CAMO},
            {"0xC5BEDD65", ItemType.HEAVYSNIPER_MK2_CAMO_02},
            {"0xE9712475", ItemType.HEAVYSNIPER_MK2_CAMO_03},
            {"0x13AA78E7", ItemType.HEAVYSNIPER_MK2_CAMO_04},
            {"0x26591E50", ItemType.HEAVYSNIPER_MK2_CAMO_05},
            {"0x302731EC", ItemType.HEAVYSNIPER_MK2_CAMO_06},
            {"0xAC722A78", ItemType.HEAVYSNIPER_MK2_CAMO_07},
            {"0xBEA4CEDD", ItemType.HEAVYSNIPER_MK2_CAMO_08},
            {"0xCD776C82", ItemType.HEAVYSNIPER_MK2_CAMO_09},
            {"0xABC5ACC7", ItemType.HEAVYSNIPER_MK2_CAMO_10},
            {"0x6C32D2EB", ItemType.HEAVYSNIPER_MK2_CAMO_IND_01},
            {"0x2CD8FF9D", ItemType.HEAVYSNIPER_MK2_CLIP_02},
            {"0xF835D6D4", ItemType.HEAVYSNIPER_MK2_CLIP_ARMORPIERCING},
            {"0x89EBDAA7", ItemType.HEAVYSNIPER_MK2_CLIP_EXPLOSIVE},
            {"0x3BE948F6", ItemType.HEAVYSNIPER_MK2_CLIP_FMJ},
            {"0xEC0F617", ItemType.HEAVYSNIPER_MK2_CLIP_INCENDIARY},
            {"0xEED9FD63", ItemType.KNUCKLE_VARMOD_BALLAS},
            {"0x9761D9DC", ItemType.KNUCKLE_VARMOD_DIAMOND},
            {"0x50910C31", ItemType.KNUCKLE_VARMOD_DOLLAR},
            {"0x7DECFE30", ItemType.KNUCKLE_VARMOD_HATE},
            {"0xE28BABEF", ItemType.KNUCKLE_VARMOD_KING},
            {"0x3F4E8AA6", ItemType.KNUCKLE_VARMOD_LOVE},
            {"0xC613F685", ItemType.KNUCKLE_VARMOD_PIMP},
            {"0x8B808BB", ItemType.KNUCKLE_VARMOD_PLAYER},
            {"0x7AF3F785", ItemType.KNUCKLE_VARMOD_VAGOS},
            {"0xB92C6979", ItemType.MACHINEPISTOL_CLIP_02},
            {"0xA9E9CAF4", ItemType.MACHINEPISTOL_CLIP_03},
            {"0xCCFD2AC5", ItemType.MARKSMANRIFLE_CLIP_02},
            {"0x9094FBA0", ItemType.MARKSMANRIFLE_MK2_CAMO},
            {"0x7320F4B2", ItemType.MARKSMANRIFLE_MK2_CAMO_02},
            {"0x60CF500F", ItemType.MARKSMANRIFLE_MK2_CAMO_03},
            {"0xFE668B3F", ItemType.MARKSMANRIFLE_MK2_CAMO_04},
            {"0xF3757559", ItemType.MARKSMANRIFLE_MK2_CAMO_05},
            {"0x193B40E8", ItemType.MARKSMANRIFLE_MK2_CAMO_06},
            {"0x107D2F6C", ItemType.MARKSMANRIFLE_MK2_CAMO_07},
            {"0xC4E91841", ItemType.MARKSMANRIFLE_MK2_CAMO_08},
            {"0x9BB1C5D3", ItemType.MARKSMANRIFLE_MK2_CAMO_09},
            {"0x3B61040B", ItemType.MARKSMANRIFLE_MK2_CAMO_10},
            {"0xB7A316DA", ItemType.MARKSMANRIFLE_MK2_CAMO_IND_01},
            {"0xE6CFD1AA", ItemType.MARKSMANRIFLE_MK2_CLIP_02},
            {"0xF46FD079", ItemType.MARKSMANRIFLE_MK2_CLIP_ARMORPIERCING},
            {"0xE14A9ED3", ItemType.MARKSMANRIFLE_MK2_CLIP_FMJ},
            {"0x6DD7A86E", ItemType.MARKSMANRIFLE_MK2_CLIP_INCENDIARY},
            {"0xD77A22D2", ItemType.MARKSMANRIFLE_MK2_CLIP_TRACER},
            {"0x161E9241", ItemType.MARKSMANRIFLE_VARMOD_LUXE},
            {"0x82158B47", ItemType.MG_CLIP_02},
            {"0xD6DABABE", ItemType.MG_VARMOD_LOWRIDER},
            {"0x10E6BA2B", ItemType.MICROSMG_CLIP_02},
            {"0x487AAE09", ItemType.MICROSMG_VARMOD_LUXE},
            {"0x684ACE42", ItemType.MILITARYRIFLE_CLIP_02},
            {"0x6B82F395", ItemType.MILITARYRIFLE_SIGHT_01},
            {"0x937ED0B7", ItemType.MINISMG_CLIP_02},
            {"0xD9D3AC92", ItemType.PISTOL50_CLIP_02},
            {"0x77B8AB2F", ItemType.PISTOL50_VARMOD_LUXE},
            {"0xED265A1C", ItemType.PISTOL_CLIP_02},
            {"0x5C6C749C", ItemType.PISTOL_MK2_CAMO},
            {"0x15F7A390", ItemType.PISTOL_MK2_CAMO_02},
            {"0x1A1F1260", ItemType.PISTOL_MK2_CAMO_02_SLIDE},
            {"0x968E24DB", ItemType.PISTOL_MK2_CAMO_03},
            {"0xE4E00B70", ItemType.PISTOL_MK2_CAMO_03_SLIDE},
            {"0x17BFA99", ItemType.PISTOL_MK2_CAMO_04},
            {"0x2C298B2B", ItemType.PISTOL_MK2_CAMO_04_SLIDE},
            {"0xF2685C72", ItemType.PISTOL_MK2_CAMO_05},
            {"0xDFB79725", ItemType.PISTOL_MK2_CAMO_05_SLIDE},
            {"0xDD2231E6", ItemType.PISTOL_MK2_CAMO_06},
            {"0x6BD7228C", ItemType.PISTOL_MK2_CAMO_06_SLIDE},
            {"0xBB43EE76", ItemType.PISTOL_MK2_CAMO_07},
            {"0x9DDBCF8C", ItemType.PISTOL_MK2_CAMO_07_SLIDE},
            {"0x4D901310", ItemType.PISTOL_MK2_CAMO_08},
            {"0xB319A52C", ItemType.PISTOL_MK2_CAMO_08_SLIDE},
            {"0x5F31B653", ItemType.PISTOL_MK2_CAMO_09},
            {"0xC6836E12", ItemType.PISTOL_MK2_CAMO_09_SLIDE},
            {"0x697E19A0", ItemType.PISTOL_MK2_CAMO_10},
            {"0x43B1B173", ItemType.PISTOL_MK2_CAMO_10_SLIDE},
            {"0x930CB951", ItemType.PISTOL_MK2_CAMO_IND_01},
            {"0x4ABDA3FA", ItemType.PISTOL_MK2_CAMO_IND_01_SLIDE},
            {"0xB4FC92B0", ItemType.PISTOL_MK2_CAMO_SLIDE},
            {"0x5ED6C128", ItemType.PISTOL_MK2_CLIP_02},
            {"0x4F37DF2A", ItemType.PISTOL_MK2_CLIP_FMJ},
            {"0x85FEA109", ItemType.PISTOL_MK2_CLIP_HOLLOWPOINT},
            {"0x2BBD7A3A", ItemType.PISTOL_MK2_CLIP_INCENDIARY},
            {"0x25CAAEAF", ItemType.PISTOL_MK2_CLIP_TRACER},
            {"0xD7391086", ItemType.PISTOL_VARMOD_LUXE},
            {"0xE3BD9E44", ItemType.PUMPSHOTGUN_MK2_CAMO},
            {"0x17148F9B", ItemType.PUMPSHOTGUN_MK2_CAMO_02},
            {"0x24D22B16", ItemType.PUMPSHOTGUN_MK2_CAMO_03},
            {"0xF2BEC6F0", ItemType.PUMPSHOTGUN_MK2_CAMO_04},
            {"0x85627D", ItemType.PUMPSHOTGUN_MK2_CAMO_05},
            {"0xDC2919C5", ItemType.PUMPSHOTGUN_MK2_CAMO_06},
            {"0xE184247B", ItemType.PUMPSHOTGUN_MK2_CAMO_07},
            {"0xD8EF9356", ItemType.PUMPSHOTGUN_MK2_CAMO_08},
            {"0xEF29BFCA", ItemType.PUMPSHOTGUN_MK2_CAMO_09},
            {"0x67AEB165", ItemType.PUMPSHOTGUN_MK2_CAMO_10},
            {"0x46411A1D", ItemType.PUMPSHOTGUN_MK2_CAMO_IND_01},
            {"0x4E65B425", ItemType.PUMPSHOTGUN_MK2_CLIP_ARMORPIERCING},
            {"0x3BE4465D", ItemType.PUMPSHOTGUN_MK2_CLIP_EXPLOSIVE},
            {"0xE9582927", ItemType.PUMPSHOTGUN_MK2_CLIP_HOLLOWPOINT},
            {"0x9F8A1BF5", ItemType.PUMPSHOTGUN_MK2_CLIP_INCENDIARY},
            {"0xA2D79DDB", ItemType.PUMPSHOTGUN_VARMOD_LOWRIDER},
            {"0xD7DBF707", ItemType.RAYPISTOL_VARMOD_XMAS18},
            {"0xC03FED9F", ItemType.REVOLVER_MK2_CAMO},
            {"0xB5DE24", ItemType.REVOLVER_MK2_CAMO_02},
            {"0xA7FF1B8", ItemType.REVOLVER_MK2_CAMO_03},
            {"0xF2E24289", ItemType.REVOLVER_MK2_CAMO_04},
            {"0x11317F27", ItemType.REVOLVER_MK2_CAMO_05},
            {"0x17C30C42", ItemType.REVOLVER_MK2_CAMO_06},
            {"0x257927AE", ItemType.REVOLVER_MK2_CAMO_07},
            {"0x37304B1C", ItemType.REVOLVER_MK2_CAMO_08},
            {"0x48DAEE71", ItemType.REVOLVER_MK2_CAMO_09},
            {"0x20ED9B5B", ItemType.REVOLVER_MK2_CAMO_10},
            {"0xD951E867", ItemType.REVOLVER_MK2_CAMO_IND_01},
            {"0xDC8BA3F", ItemType.REVOLVER_MK2_CLIP_FMJ},
            {"0x10F42E8F", ItemType.REVOLVER_MK2_CLIP_HOLLOWPOINT},
            {"0xEFBF25", ItemType.REVOLVER_MK2_CLIP_INCENDIARY},
            {"0xC6D8E476", ItemType.REVOLVER_MK2_CLIP_TRACER},
            {"0x16EE3040", ItemType.REVOLVER_VARMOD_BOSS},
            {"0x9493B80D", ItemType.REVOLVER_VARMOD_GOON},
            {"0x85A64DF9", ItemType.SAWNOFFSHOTGUN_VARMOD_LUXE},
            {"0x350966FB", ItemType.SMG_CLIP_02},
            {"0x79C77076", ItemType.SMG_CLIP_03},
            {"0xC4979067", ItemType.SMG_MK2_CAMO},
            {"0x3815A945", ItemType.SMG_MK2_CAMO_02},
            {"0x4B4B4FB0", ItemType.SMG_MK2_CAMO_03},
            {"0xEC729200", ItemType.SMG_MK2_CAMO_04},
            {"0x48F64B22", ItemType.SMG_MK2_CAMO_05},
            {"0x35992468", ItemType.SMG_MK2_CAMO_06},
            {"0x24B782A5", ItemType.SMG_MK2_CAMO_07},
            {"0xA2E67F01", ItemType.SMG_MK2_CAMO_08},
            {"0x2218FD68", ItemType.SMG_MK2_CAMO_09},
            {"0x45C5C3C5", ItemType.SMG_MK2_CAMO_10},
            {"0x399D558F", ItemType.SMG_MK2_CAMO_IND_01},
            {"0xB9835B2E", ItemType.SMG_MK2_CLIP_02},
            {"0xB5A715F", ItemType.SMG_MK2_CLIP_FMJ},
            {"0x3A1BD6FA", ItemType.SMG_MK2_CLIP_HOLLOWPOINT},
            {"0xD99222E5", ItemType.SMG_MK2_CLIP_INCENDIARY},
            {"0x7FEA36EC", ItemType.SMG_MK2_CLIP_TRACER},
            {"0x27872C90", ItemType.SMG_VARMOD_LUXE},
            {"0x4032B5E7", ItemType.SNIPERRIFLE_VARMOD_LUXE},
            {"0x7B0033B3", ItemType.SNSPISTOL_CLIP_02},
            {"0xF7BEEDD", ItemType.SNSPISTOL_MK2_CAMO},
            {"0x8A612EF6", ItemType.SNSPISTOL_MK2_CAMO_02},
            {"0x29366D21", ItemType.SNSPISTOL_MK2_CAMO_02_SLIDE},
            {"0x76FA8829", ItemType.SNSPISTOL_MK2_CAMO_03},
            {"0x3ADE514B", ItemType.SNSPISTOL_MK2_CAMO_03_SLIDE},
            {"0xA93C6CAC", ItemType.SNSPISTOL_MK2_CAMO_04},
            {"0xE64513E9", ItemType.SNSPISTOL_MK2_CAMO_04_SLIDE},
            {"0x9C905354", ItemType.SNSPISTOL_MK2_CAMO_05},
            {"0xCD7AEB9A", ItemType.SNSPISTOL_MK2_CAMO_05_SLIDE},
            {"0x4DFA3621", ItemType.SNSPISTOL_MK2_CAMO_06},
            {"0xFA7B27A6", ItemType.SNSPISTOL_MK2_CAMO_06_SLIDE},
            {"0x42E91FFF", ItemType.SNSPISTOL_MK2_CAMO_07},
            {"0xE285CA9A", ItemType.SNSPISTOL_MK2_CAMO_07_SLIDE},
            {"0x54A8437D", ItemType.SNSPISTOL_MK2_CAMO_08},
            {"0x2B904B19", ItemType.SNSPISTOL_MK2_CAMO_08_SLIDE},
            {"0x68C2746", ItemType.SNSPISTOL_MK2_CAMO_09},
            {"0x22C24F9C", ItemType.SNSPISTOL_MK2_CAMO_09_SLIDE},
            {"0x2366E467", ItemType.SNSPISTOL_MK2_CAMO_10},
            {"0x8D0D5ECD", ItemType.SNSPISTOL_MK2_CAMO_10_SLIDE},
            {"0x441882E6", ItemType.SNSPISTOL_MK2_CAMO_IND_01},
            {"0x1F07150A", ItemType.SNSPISTOL_MK2_CAMO_IND_01_SLIDE},
            {"0xE7EE68EA", ItemType.SNSPISTOL_MK2_CAMO_SLIDE},
            {"0xCE8C0772", ItemType.SNSPISTOL_MK2_CLIP_02},
            {"0xC111EB26", ItemType.SNSPISTOL_MK2_CLIP_FMJ},
            {"0x8D107402", ItemType.SNSPISTOL_MK2_CLIP_HOLLOWPOINT},
            {"0xE6AD5F79", ItemType.SNSPISTOL_MK2_CLIP_INCENDIARY},
            {"0x902DA26E", ItemType.SNSPISTOL_MK2_CLIP_TRACER},
            {"0x8033ECAF", ItemType.SNSPISTOL_VARMOD_LOWRIDER},
            {"0x7C8BD10E", ItemType.SPECIALCARBINE_CLIP_02},
            {"0x6B59AEAA", ItemType.SPECIALCARBINE_CLIP_03},
            {"0xD40BB53B", ItemType.SPECIALCARBINE_MK2_CAMO},
            {"0x431B238B", ItemType.SPECIALCARBINE_MK2_CAMO_02},
            {"0x34CF86F4", ItemType.SPECIALCARBINE_MK2_CAMO_03},
            {"0xB4C306DD", ItemType.SPECIALCARBINE_MK2_CAMO_04},
            {"0xEE677A25", ItemType.SPECIALCARBINE_MK2_CAMO_05},
            {"0xDF90DC78", ItemType.SPECIALCARBINE_MK2_CAMO_06},
            {"0xA4C31EE", ItemType.SPECIALCARBINE_MK2_CAMO_07},
            {"0x89CFB0F7", ItemType.SPECIALCARBINE_MK2_CAMO_08},
            {"0x7B82145C", ItemType.SPECIALCARBINE_MK2_CAMO_09},
            {"0x899CAF75", ItemType.SPECIALCARBINE_MK2_CAMO_10},
            {"0x5218C819", ItemType.SPECIALCARBINE_MK2_CAMO_IND_01},
            {"0xDE1FA12C", ItemType.SPECIALCARBINE_MK2_CLIP_02},
            {"0x51351635", ItemType.SPECIALCARBINE_MK2_CLIP_ARMORPIERCING},
            {"0x503DEA90", ItemType.SPECIALCARBINE_MK2_CLIP_FMJ},
            {"0xDE011286", ItemType.SPECIALCARBINE_MK2_CLIP_INCENDIARY},
            {"0x8765C68A", ItemType.SPECIALCARBINE_MK2_CLIP_TRACER},
            {"0x730154F2", ItemType.SPECIALCARBINE_VARMOD_LOWRIDER},
            {"0x5B3E7DB6", ItemType.SWITCHBLADE_VARMOD_VAR1},
            {"0xE7939662", ItemType.SWITCHBLADE_VARMOD_VAR2},
            {"0x33BA12E8", ItemType.VINTAGEPISTOL_CLIP_02},
        };
        public static Dictionary<ItemType, string> ItemCType = new Dictionary<ItemType, string>()
        {
            // [Modification]
            {ItemType.ADVANCEDRIFLE_CLIP_02, "0x8EC1C979"},
            {ItemType.ADVANCEDRIFLE_VARMOD_LUXE, "0x377CD377"},
            {ItemType.APPISTOL_CLIP_02, "0x249A17D5"},
            {ItemType.APPISTOL_VARMOD_LUXE, "0x9B76C72C"},
            {ItemType.ASSAULTRIFLE_CLIP_02, "0xB1214F9B"},
            {ItemType.ASSAULTRIFLE_CLIP_03, "0xDBF0A53D"},
            {ItemType.ASSAULTRIFLE_MK2_CAMO, "0x911B24AF"},
            {ItemType.ASSAULTRIFLE_MK2_CAMO_02, "0x37E5444B"},
            {ItemType.ASSAULTRIFLE_MK2_CAMO_03, "0x538B7B97"},
            {ItemType.ASSAULTRIFLE_MK2_CAMO_04, "0x25789F72"},
            {ItemType.ASSAULTRIFLE_MK2_CAMO_05, "0xC5495F2D"},
            {ItemType.ASSAULTRIFLE_MK2_CAMO_06, "0xCF8B73B1"},
            {ItemType.ASSAULTRIFLE_MK2_CAMO_07, "0xA9BB2811"},
            {ItemType.ASSAULTRIFLE_MK2_CAMO_08, "0xFC674D54"},
            {ItemType.ASSAULTRIFLE_MK2_CAMO_09, "0x7C7FCD9B"},
            {ItemType.ASSAULTRIFLE_MK2_CAMO_10, "0xA5C38392"},
            {ItemType.ASSAULTRIFLE_MK2_CAMO_IND_01, "0xB9B15DB0"},
            {ItemType.ASSAULTRIFLE_MK2_CLIP_02, "0xD12ACA6F"},
            {ItemType.ASSAULTRIFLE_MK2_CLIP_ARMORPIERCING, "0xA7DD1E58"},
            {ItemType.ASSAULTRIFLE_MK2_CLIP_FMJ, "0x63E0A098"},
            {ItemType.ASSAULTRIFLE_MK2_CLIP_INCENDIARY, "0xFB70D853"},
            {ItemType.ASSAULTRIFLE_MK2_CLIP_TRACER, "0xEF2C78C1"},
            {ItemType.ASSAULTRIFLE_VARMOD_LUXE, "0x4EAD7533"},
            {ItemType.ASSAULTSHOTGUN_CLIP_02, "0x86BD7F72"},
            {ItemType.ASSAULTSMG_CLIP_02, "0xBB46E417"},
            {ItemType.ASSAULTSMG_VARMOD_LOWRIDER, "0x278C78AF"},
            {ItemType.AT_AR_AFGRIP, "0xC164F53"},
            {ItemType.AT_AR_AFGRIP_02, "0x9D65907A"},
            {ItemType.AT_AR_BARREL_02, "0x5646C26A"},
            {ItemType.AT_AR_FLSH, "0x7BC4CDDC"},
            {ItemType.AT_AR_SUPP, "0x837445AA"},
            {ItemType.AT_AR_SUPP_02, "0xA73D4664"},
            {ItemType.AT_BP_BARREL_02, "0x3BF26DC7"},
            {ItemType.AT_CR_BARREL_02, "0x8B3C480B"},
            {ItemType.AT_MG_BARREL_02, "0xB5E2575B"},
            {ItemType.AT_MRFL_BARREL_02, "0x68373DDC"},
            {ItemType.AT_MUZZLE_01, "0xB99402D4"},
            {ItemType.AT_MUZZLE_02, "0xC867A07B"},
            {ItemType.AT_MUZZLE_03, "0xDE11CBCF"},
            {ItemType.AT_MUZZLE_04, "0xEC9068CC"},
            {ItemType.AT_MUZZLE_05, "0x2E7957A"},
            {ItemType.AT_MUZZLE_06, "0x347EF8AC"},
            {ItemType.AT_MUZZLE_07, "0x4DB62ABE"},
            {ItemType.AT_MUZZLE_08, "0x5F7DCE4D"},
            {ItemType.AT_MUZZLE_09, "0x6927E1A1"},
            {ItemType.AT_PI_COMP, "0x21E34793"},
            {ItemType.AT_PI_COMP_02, "0xAA8283BF"},
            {ItemType.AT_PI_COMP_03, "0x27077CCB"},
            {ItemType.AT_PI_FLSH, "0x359B7AAE"},
            {ItemType.AT_PI_FLSH_02, "0x43FD595B"},
            {ItemType.AT_PI_FLSH_03, "0x4A4965F3"},
            {ItemType.AT_PI_RAIL, "0x8ED4BB70"},
            {ItemType.AT_PI_RAIL_02, "0x47DE9258"},
            {ItemType.AT_PI_SUPP, "0xC304849A"},
            {ItemType.AT_PI_SUPP_02, "0x65EA7EBB"},
            {ItemType.AT_SB_BARREL_02, "0xA564D78B"},
            {ItemType.AT_SC_BARREL_02, "0xF97F783B"},
            {ItemType.AT_SCOPE_LARGE, "0xD2443DDC"},
            {ItemType.AT_SCOPE_LARGE_FIXED_ZOOM, "0x1C221B1A"},
            {ItemType.AT_SCOPE_LARGE_FIXED_ZOOM_MK2, "0x5B1C713C"},
            {ItemType.AT_SCOPE_LARGE_MK2, "0x82C10383"},
            {ItemType.AT_SCOPE_MACRO, "0x9D2FBF29"},
            {ItemType.AT_SCOPE_MACRO_02, "0x3CC6BA57"},
            {ItemType.AT_SCOPE_MACRO_02_MK2, "0xC7ADD105"},
            {ItemType.AT_SCOPE_MACRO_02_SMG_MK2, "0xE502AB6B"},
            {ItemType.AT_SCOPE_MACRO_MK2, "0x49B2945"},
            {ItemType.AT_SCOPE_MAX, "0xBC54DA77"},
            {ItemType.AT_SCOPE_MEDIUM, "0xA0D89C42"},
            {ItemType.AT_SCOPE_MEDIUM_MK2, "0xC66B6542"},
            {ItemType.AT_SCOPE_NV, "0xB68010B0"},
            {ItemType.AT_SCOPE_SMALL, "0xAA2C45B4"},
            {ItemType.AT_SCOPE_SMALL_02, "0x3C00AFED"},
            {ItemType.AT_SCOPE_SMALL_MK2, "0x3F3C8181"},
            {ItemType.AT_SCOPE_SMALL_SMG_MK2, "0x3DECC7DA"},
            {ItemType.AT_SCOPE_THERMAL, "0x2E43DA41"},
            {ItemType.AT_SIGHTS, "0x420FD713"},
            {ItemType.AT_SIGHTS_SMG, "0x9FDB5652"},
            {ItemType.AT_SR_BARREL_02, "0x108AB09E"},
            {ItemType.AT_SR_SUPP, "0xE608B35E"},
            {ItemType.AT_SR_SUPP_03, "0xAC42DF71"},
            {ItemType.BULLPUPRIFLE_CLIP_02, "0xB3688B0F"},
            {ItemType.BULLPUPRIFLE_MK2_CAMO, "0xAE4055B7"},
            {ItemType.BULLPUPRIFLE_MK2_CAMO_02, "0xB905ED6B"},
            {ItemType.BULLPUPRIFLE_MK2_CAMO_03, "0xA6C448E8"},
            {ItemType.BULLPUPRIFLE_MK2_CAMO_04, "0x9486246C"},
            {ItemType.BULLPUPRIFLE_MK2_CAMO_05, "0x8A390FD2"},
            {ItemType.BULLPUPRIFLE_MK2_CAMO_06, "0x2337FC5"},
            {ItemType.BULLPUPRIFLE_MK2_CAMO_07, "0xEFFFDB5E"},
            {ItemType.BULLPUPRIFLE_MK2_CAMO_08, "0xDDBDB6DA"},
            {ItemType.BULLPUPRIFLE_MK2_CAMO_09, "0xCB631225"},
            {ItemType.BULLPUPRIFLE_MK2_CAMO_10, "0xA87D541E"},
            {ItemType.BULLPUPRIFLE_MK2_CAMO_IND_01, "0xC5E9AE52"},
            {ItemType.BULLPUPRIFLE_MK2_CLIP_02, "0xEFB00628"},
            {ItemType.BULLPUPRIFLE_MK2_CLIP_ARMORPIERCING, "0xFAA7F5ED"},
            {ItemType.BULLPUPRIFLE_MK2_CLIP_FMJ, "0x43621710"},
            {ItemType.BULLPUPRIFLE_MK2_CLIP_INCENDIARY, "0xA99CF95A"},
            {ItemType.BULLPUPRIFLE_MK2_CLIP_TRACER, "0x822060A9"},
            {ItemType.BULLPUPRIFLE_VARMOD_LOW, "0xA857BC78"},
            {ItemType.CARBINERIFLE_CLIP_02, "0x91109691"},
            {ItemType.CARBINERIFLE_CLIP_03, "0xBA62E935"},
            {ItemType.CARBINERIFLE_MK2_CAMO, "0x4BDD6F16"},
            {ItemType.CARBINERIFLE_MK2_CAMO_02, "0x406A7908"},
            {ItemType.CARBINERIFLE_MK2_CAMO_03, "0x2F3856A4"},
            {ItemType.CARBINERIFLE_MK2_CAMO_04, "0xE50C424D"},
            {ItemType.CARBINERIFLE_MK2_CAMO_05, "0xD37D1F2F"},
            {ItemType.CARBINERIFLE_MK2_CAMO_06, "0x86268483"},
            {ItemType.CARBINERIFLE_MK2_CAMO_07, "0xF420E076"},
            {ItemType.CARBINERIFLE_MK2_CAMO_08, "0xAAE14DF8"},
            {ItemType.CARBINERIFLE_MK2_CAMO_09, "0x9893A95D"},
            {ItemType.CARBINERIFLE_MK2_CAMO_10, "0x6B13CD3E"},
            {ItemType.CARBINERIFLE_MK2_CAMO_IND_01, "0xDA55CD3F"},
            {ItemType.CARBINERIFLE_MK2_CLIP_02, "0x5DD5DBD5"},
            {ItemType.CARBINERIFLE_MK2_CLIP_ARMORPIERCING, "0x255D5D57"},
            {ItemType.CARBINERIFLE_MK2_CLIP_FMJ, "0x44032F11"},
            {ItemType.CARBINERIFLE_MK2_CLIP_INCENDIARY, "0x3D25C2A7"},
            {ItemType.CARBINERIFLE_MK2_CLIP_TRACER, "0x1757F566"},
            {ItemType.CARBINERIFLE_VARMOD_LUXE, "0xD89B9658"},
            {ItemType.CERAMICPISTOL_CLIP_02, "0x81786CA9"},
            {ItemType.CERAMICPISTOL_SUPP, "0x9307D6FA"},
            {ItemType.COMBATMG_CLIP_02, "0xD6C59CD6"},
            {ItemType.COMBATMG_MK2_CAMO, "0x4A768CB5"},
            {ItemType.COMBATMG_MK2_CAMO_02, "0xCCE06BBD"},
            {ItemType.COMBATMG_MK2_CAMO_03, "0xBE94CF26"},
            {ItemType.COMBATMG_MK2_CAMO_04, "0x7609BE11"},
            {ItemType.COMBATMG_MK2_CAMO_05, "0x48AF6351"},
            {ItemType.COMBATMG_MK2_CAMO_06, "0x9186750A"},
            {ItemType.COMBATMG_MK2_CAMO_07, "0x84555AA8"},
            {ItemType.COMBATMG_MK2_CAMO_08, "0x1B4C088B"},
            {ItemType.COMBATMG_MK2_CAMO_09, "0xE046DFC"},
            {ItemType.COMBATMG_MK2_CAMO_10, "0x28B536E"},
            {ItemType.COMBATMG_MK2_CAMO_IND_01, "0xD703C94D"},
            {ItemType.COMBATMG_MK2_CLIP_02, "0x17DF42E9"},
            {ItemType.COMBATMG_MK2_CLIP_ARMORPIERCING, "0x29882423"},
            {ItemType.COMBATMG_MK2_CLIP_FMJ, "0x57EF1CC8"},
            {ItemType.COMBATMG_MK2_CLIP_INCENDIARY, "0xC326BDBA"},
            {ItemType.COMBATMG_MK2_CLIP_TRACER, "0xF6649745"},
            {ItemType.COMBATMG_VARMOD_LOWRIDER, "0x92FECCDD"},
            {ItemType.COMBATPDW_CLIP_02, "0x334A5203"},
            {ItemType.COMBATPDW_CLIP_03, "0x6EB8C8DB"},
            {ItemType.COMBATPISTOL_CLIP_02, "0xD67B4F2D"},
            {ItemType.COMBATPISTOL_VARMOD_LOWRIDER, "0xC6654D72"},
            {ItemType.COMPACTRIFLE_CLIP_02, "0x59FF9BF8"},
            {ItemType.COMPACTRIFLE_CLIP_03, "0xC607740E"},
            {ItemType.GUSENBERG_CLIP_02, "0xEAC8C270"},
            {ItemType.HEAVYPISTOL_CLIP_02, "0x64F9C62B"},
            {ItemType.HEAVYPISTOL_VARMOD_LUXE, "0x7A6A7B7B"},
            {ItemType.HEAVYSHOTGUN_CLIP_02, "0x971CF6FD"},
            {ItemType.HEAVYSHOTGUN_CLIP_03, "0x88C7DA53"},
            {ItemType.HEAVYSNIPER_MK2_CAMO, "0xF8337D02"},
            {ItemType.HEAVYSNIPER_MK2_CAMO_02, "0xC5BEDD65"},
            {ItemType.HEAVYSNIPER_MK2_CAMO_03, "0xE9712475"},
            {ItemType.HEAVYSNIPER_MK2_CAMO_04, "0x13AA78E7"},
            {ItemType.HEAVYSNIPER_MK2_CAMO_05, "0x26591E50"},
            {ItemType.HEAVYSNIPER_MK2_CAMO_06, "0x302731EC"},
            {ItemType.HEAVYSNIPER_MK2_CAMO_07, "0xAC722A78"},
            {ItemType.HEAVYSNIPER_MK2_CAMO_08, "0xBEA4CEDD"},
            {ItemType.HEAVYSNIPER_MK2_CAMO_09, "0xCD776C82"},
            {ItemType.HEAVYSNIPER_MK2_CAMO_10, "0xABC5ACC7"},
            {ItemType.HEAVYSNIPER_MK2_CAMO_IND_01, "0x6C32D2EB"},
            {ItemType.HEAVYSNIPER_MK2_CLIP_02, "0x2CD8FF9D"},
            {ItemType.HEAVYSNIPER_MK2_CLIP_ARMORPIERCING, "0xF835D6D4"},
            {ItemType.HEAVYSNIPER_MK2_CLIP_EXPLOSIVE, "0x89EBDAA7"},
            {ItemType.HEAVYSNIPER_MK2_CLIP_FMJ, "0x3BE948F6"},
            {ItemType.HEAVYSNIPER_MK2_CLIP_INCENDIARY, "0xEC0F617"},
            {ItemType.KNUCKLE_VARMOD_BALLAS, "0xEED9FD63"},
            {ItemType.KNUCKLE_VARMOD_DIAMOND, "0x9761D9DC"},
            {ItemType.KNUCKLE_VARMOD_DOLLAR, "0x50910C31"},
            {ItemType.KNUCKLE_VARMOD_HATE, "0x7DECFE30"},
            {ItemType.KNUCKLE_VARMOD_KING, "0xE28BABEF"},
            {ItemType.KNUCKLE_VARMOD_LOVE, "0x3F4E8AA6"},
            {ItemType.KNUCKLE_VARMOD_PIMP, "0xC613F685"},
            {ItemType.KNUCKLE_VARMOD_PLAYER, "0x8B808BB"},
            {ItemType.KNUCKLE_VARMOD_VAGOS, "0x7AF3F785"},
            {ItemType.MACHINEPISTOL_CLIP_02, "0xB92C6979"},
            {ItemType.MACHINEPISTOL_CLIP_03, "0xA9E9CAF4"},
            {ItemType.MARKSMANRIFLE_CLIP_02, "0xCCFD2AC5"},
            {ItemType.MARKSMANRIFLE_MK2_CAMO, "0x9094FBA0"},
            {ItemType.MARKSMANRIFLE_MK2_CAMO_02, "0x7320F4B2"},
            {ItemType.MARKSMANRIFLE_MK2_CAMO_03, "0x60CF500F"},
            {ItemType.MARKSMANRIFLE_MK2_CAMO_04, "0xFE668B3F"},
            {ItemType.MARKSMANRIFLE_MK2_CAMO_05, "0xF3757559"},
            {ItemType.MARKSMANRIFLE_MK2_CAMO_06, "0x193B40E8"},
            {ItemType.MARKSMANRIFLE_MK2_CAMO_07, "0x107D2F6C"},
            {ItemType.MARKSMANRIFLE_MK2_CAMO_08, "0xC4E91841"},
            {ItemType.MARKSMANRIFLE_MK2_CAMO_09, "0x9BB1C5D3"},
            {ItemType.MARKSMANRIFLE_MK2_CAMO_10, "0x3B61040B"},
            {ItemType.MARKSMANRIFLE_MK2_CAMO_IND_01, "0xB7A316DA"},
            {ItemType.MARKSMANRIFLE_MK2_CLIP_02, "0xE6CFD1AA"},
            {ItemType.MARKSMANRIFLE_MK2_CLIP_ARMORPIERCING, "0xF46FD079"},
            {ItemType.MARKSMANRIFLE_MK2_CLIP_FMJ, "0xE14A9ED3"},
            {ItemType.MARKSMANRIFLE_MK2_CLIP_INCENDIARY, "0x6DD7A86E"},
            {ItemType.MARKSMANRIFLE_MK2_CLIP_TRACER, "0xD77A22D2"},
            {ItemType.MARKSMANRIFLE_VARMOD_LUXE, "0x161E9241"},
            {ItemType.MG_CLIP_02, "0x82158B47"},
            {ItemType.MG_VARMOD_LOWRIDER, "0xD6DABABE"},
            {ItemType.MICROSMG_CLIP_02, "0x10E6BA2B"},
            {ItemType.MICROSMG_VARMOD_LUXE, "0x487AAE09"},
            {ItemType.MILITARYRIFLE_CLIP_02, "0x684ACE42"},
            {ItemType.MILITARYRIFLE_SIGHT_01, "0x6B82F395"},
            {ItemType.MINISMG_CLIP_02, "0x937ED0B7"},
            {ItemType.PISTOL50_CLIP_02, "0xD9D3AC92"},
            {ItemType.PISTOL50_VARMOD_LUXE, "0x77B8AB2F"},
            {ItemType.PISTOL_CLIP_02, "0xED265A1C"},
            {ItemType.PISTOL_MK2_CAMO, "0x5C6C749C"},
            {ItemType.PISTOL_MK2_CAMO_02, "0x15F7A390"},
            {ItemType.PISTOL_MK2_CAMO_02_SLIDE, "0x1A1F1260"},
            {ItemType.PISTOL_MK2_CAMO_03, "0x968E24DB"},
            {ItemType.PISTOL_MK2_CAMO_03_SLIDE, "0xE4E00B70"},
            {ItemType.PISTOL_MK2_CAMO_04, "0x17BFA99"},
            {ItemType.PISTOL_MK2_CAMO_04_SLIDE, "0x2C298B2B"},
            {ItemType.PISTOL_MK2_CAMO_05, "0xF2685C72"},
            {ItemType.PISTOL_MK2_CAMO_05_SLIDE, "0xDFB79725"},
            {ItemType.PISTOL_MK2_CAMO_06, "0xDD2231E6"},
            {ItemType.PISTOL_MK2_CAMO_06_SLIDE, "0x6BD7228C"},
            {ItemType.PISTOL_MK2_CAMO_07, "0xBB43EE76"},
            {ItemType.PISTOL_MK2_CAMO_07_SLIDE, "0x9DDBCF8C"},
            {ItemType.PISTOL_MK2_CAMO_08, "0x4D901310"},
            {ItemType.PISTOL_MK2_CAMO_08_SLIDE, "0xB319A52C"},
            {ItemType.PISTOL_MK2_CAMO_09, "0x5F31B653"},
            {ItemType.PISTOL_MK2_CAMO_09_SLIDE, "0xC6836E12"},
            {ItemType.PISTOL_MK2_CAMO_10, "0x697E19A0"},
            {ItemType.PISTOL_MK2_CAMO_10_SLIDE, "0x43B1B173"},
            {ItemType.PISTOL_MK2_CAMO_IND_01, "0x930CB951"},
            {ItemType.PISTOL_MK2_CAMO_IND_01_SLIDE, "0x4ABDA3FA"},
            {ItemType.PISTOL_MK2_CAMO_SLIDE, "0xB4FC92B0"},
            {ItemType.PISTOL_MK2_CLIP_02, "0x5ED6C128"},
            {ItemType.PISTOL_MK2_CLIP_FMJ, "0x4F37DF2A"},
            {ItemType.PISTOL_MK2_CLIP_HOLLOWPOINT, "0x85FEA109"},
            {ItemType.PISTOL_MK2_CLIP_INCENDIARY, "0x2BBD7A3A"},
            {ItemType.PISTOL_MK2_CLIP_TRACER, "0x25CAAEAF"},
            {ItemType.PISTOL_VARMOD_LUXE, "0xD7391086"},
            {ItemType.PUMPSHOTGUN_MK2_CAMO, "0xE3BD9E44"},
            {ItemType.PUMPSHOTGUN_MK2_CAMO_02, "0x17148F9B"},
            {ItemType.PUMPSHOTGUN_MK2_CAMO_03, "0x24D22B16"},
            {ItemType.PUMPSHOTGUN_MK2_CAMO_04, "0xF2BEC6F0"},
            {ItemType.PUMPSHOTGUN_MK2_CAMO_05, "0x85627D"},
            {ItemType.PUMPSHOTGUN_MK2_CAMO_06, "0xDC2919C5"},
            {ItemType.PUMPSHOTGUN_MK2_CAMO_07, "0xE184247B"},
            {ItemType.PUMPSHOTGUN_MK2_CAMO_08, "0xD8EF9356"},
            {ItemType.PUMPSHOTGUN_MK2_CAMO_09, "0xEF29BFCA"},
            {ItemType.PUMPSHOTGUN_MK2_CAMO_10, "0x67AEB165"},
            {ItemType.PUMPSHOTGUN_MK2_CAMO_IND_01, "0x46411A1D"},
            {ItemType.PUMPSHOTGUN_MK2_CLIP_ARMORPIERCING, "0x4E65B425"},
            {ItemType.PUMPSHOTGUN_MK2_CLIP_EXPLOSIVE, "0x3BE4465D"},
            {ItemType.PUMPSHOTGUN_MK2_CLIP_HOLLOWPOINT, "0xE9582927"},
            {ItemType.PUMPSHOTGUN_MK2_CLIP_INCENDIARY, "0x9F8A1BF5"},
            {ItemType.PUMPSHOTGUN_VARMOD_LOWRIDER, "0xA2D79DDB"},
            {ItemType.RAYPISTOL_VARMOD_XMAS18, "0xD7DBF707"},
            {ItemType.REVOLVER_MK2_CAMO, "0xC03FED9F"},
            {ItemType.REVOLVER_MK2_CAMO_02, "0xB5DE24"},
            {ItemType.REVOLVER_MK2_CAMO_03, "0xA7FF1B8"},
            {ItemType.REVOLVER_MK2_CAMO_04, "0xF2E24289"},
            {ItemType.REVOLVER_MK2_CAMO_05, "0x11317F27"},
            {ItemType.REVOLVER_MK2_CAMO_06, "0x17C30C42"},
            {ItemType.REVOLVER_MK2_CAMO_07, "0x257927AE"},
            {ItemType.REVOLVER_MK2_CAMO_08, "0x37304B1C"},
            {ItemType.REVOLVER_MK2_CAMO_09, "0x48DAEE71"},
            {ItemType.REVOLVER_MK2_CAMO_10, "0x20ED9B5B"},
            {ItemType.REVOLVER_MK2_CAMO_IND_01, "0xD951E867"},
            {ItemType.REVOLVER_MK2_CLIP_FMJ, "0xDC8BA3F"},
            {ItemType.REVOLVER_MK2_CLIP_HOLLOWPOINT, "0x10F42E8F"},
            {ItemType.REVOLVER_MK2_CLIP_INCENDIARY, "0xEFBF25"},
            {ItemType.REVOLVER_MK2_CLIP_TRACER, "0xC6D8E476"},
            {ItemType.REVOLVER_VARMOD_BOSS, "0x16EE3040"},
            {ItemType.REVOLVER_VARMOD_GOON, "0x9493B80D"},
            {ItemType.SAWNOFFSHOTGUN_VARMOD_LUXE, "0x85A64DF9"},
            {ItemType.SMG_CLIP_02, "0x350966FB"},
            {ItemType.SMG_CLIP_03, "0x79C77076"},
            {ItemType.SMG_MK2_CAMO, "0xC4979067"},
            {ItemType.SMG_MK2_CAMO_02, "0x3815A945"},
            {ItemType.SMG_MK2_CAMO_03, "0x4B4B4FB0"},
            {ItemType.SMG_MK2_CAMO_04, "0xEC729200"},
            {ItemType.SMG_MK2_CAMO_05, "0x48F64B22"},
            {ItemType.SMG_MK2_CAMO_06, "0x35992468"},
            {ItemType.SMG_MK2_CAMO_07, "0x24B782A5"},
            {ItemType.SMG_MK2_CAMO_08, "0xA2E67F01"},
            {ItemType.SMG_MK2_CAMO_09, "0x2218FD68"},
            {ItemType.SMG_MK2_CAMO_10, "0x45C5C3C5"},
            {ItemType.SMG_MK2_CAMO_IND_01, "0x399D558F"},
            {ItemType.SMG_MK2_CLIP_02, "0xB9835B2E"},
            {ItemType.SMG_MK2_CLIP_FMJ, "0xB5A715F"},
            {ItemType.SMG_MK2_CLIP_HOLLOWPOINT, "0x3A1BD6FA"},
            {ItemType.SMG_MK2_CLIP_INCENDIARY, "0xD99222E5"},
            {ItemType.SMG_MK2_CLIP_TRACER, "0x7FEA36EC"},
            {ItemType.SMG_VARMOD_LUXE, "0x27872C90"},
            {ItemType.SNIPERRIFLE_VARMOD_LUXE, "0x4032B5E7"},
            {ItemType.SNSPISTOL_CLIP_02, "0x7B0033B3"},
            {ItemType.SNSPISTOL_MK2_CAMO, "0xF7BEEDD"},
            {ItemType.SNSPISTOL_MK2_CAMO_02, "0x8A612EF6"},
            {ItemType.SNSPISTOL_MK2_CAMO_02_SLIDE, "0x29366D21"},
            {ItemType.SNSPISTOL_MK2_CAMO_03, "0x76FA8829"},
            {ItemType.SNSPISTOL_MK2_CAMO_03_SLIDE, "0x3ADE514B"},
            {ItemType.SNSPISTOL_MK2_CAMO_04, "0xA93C6CAC"},
            {ItemType.SNSPISTOL_MK2_CAMO_04_SLIDE, "0xE64513E9"},
            {ItemType.SNSPISTOL_MK2_CAMO_05, "0x9C905354"},
            {ItemType.SNSPISTOL_MK2_CAMO_05_SLIDE, "0xCD7AEB9A"},
            {ItemType.SNSPISTOL_MK2_CAMO_06, "0x4DFA3621"},
            {ItemType.SNSPISTOL_MK2_CAMO_06_SLIDE, "0xFA7B27A6"},
            {ItemType.SNSPISTOL_MK2_CAMO_07, "0x42E91FFF"},
            {ItemType.SNSPISTOL_MK2_CAMO_07_SLIDE, "0xE285CA9A"},
            {ItemType.SNSPISTOL_MK2_CAMO_08, "0x54A8437D"},
            {ItemType.SNSPISTOL_MK2_CAMO_08_SLIDE, "0x2B904B19"},
            {ItemType.SNSPISTOL_MK2_CAMO_09, "0x68C2746"},
            {ItemType.SNSPISTOL_MK2_CAMO_09_SLIDE, "0x22C24F9C"},
            {ItemType.SNSPISTOL_MK2_CAMO_10, "0x2366E467"},
            {ItemType.SNSPISTOL_MK2_CAMO_10_SLIDE, "0x8D0D5ECD"},
            {ItemType.SNSPISTOL_MK2_CAMO_IND_01, "0x441882E6"},
            {ItemType.SNSPISTOL_MK2_CAMO_IND_01_SLIDE, "0x1F07150A"},
            {ItemType.SNSPISTOL_MK2_CAMO_SLIDE, "0xE7EE68EA"},
            {ItemType.SNSPISTOL_MK2_CLIP_02, "0xCE8C0772"},
            {ItemType.SNSPISTOL_MK2_CLIP_FMJ, "0xC111EB26"},
            {ItemType.SNSPISTOL_MK2_CLIP_HOLLOWPOINT, "0x8D107402"},
            {ItemType.SNSPISTOL_MK2_CLIP_INCENDIARY, "0xE6AD5F79"},
            {ItemType.SNSPISTOL_MK2_CLIP_TRACER, "0x902DA26E"},
            {ItemType.SNSPISTOL_VARMOD_LOWRIDER, "0x8033ECAF"},
            {ItemType.SPECIALCARBINE_CLIP_02, "0x7C8BD10E"},
            {ItemType.SPECIALCARBINE_CLIP_03, "0x6B59AEAA"},
            {ItemType.SPECIALCARBINE_MK2_CAMO, "0xD40BB53B"},
            {ItemType.SPECIALCARBINE_MK2_CAMO_02, "0x431B238B"},
            {ItemType.SPECIALCARBINE_MK2_CAMO_03, "0x34CF86F4"},
            {ItemType.SPECIALCARBINE_MK2_CAMO_04, "0xB4C306DD"},
            {ItemType.SPECIALCARBINE_MK2_CAMO_05, "0xEE677A25"},
            {ItemType.SPECIALCARBINE_MK2_CAMO_06, "0xDF90DC78"},
            {ItemType.SPECIALCARBINE_MK2_CAMO_07, "0xA4C31EE"},
            {ItemType.SPECIALCARBINE_MK2_CAMO_08, "0x89CFB0F7"},
            {ItemType.SPECIALCARBINE_MK2_CAMO_09, "0x7B82145C"},
            {ItemType.SPECIALCARBINE_MK2_CAMO_10, "0x899CAF75"},
            {ItemType.SPECIALCARBINE_MK2_CAMO_IND_01, "0x5218C819"},
            {ItemType.SPECIALCARBINE_MK2_CLIP_02, "0xDE1FA12C"},
            {ItemType.SPECIALCARBINE_MK2_CLIP_ARMORPIERCING, "0x51351635"},
            {ItemType.SPECIALCARBINE_MK2_CLIP_FMJ, "0x503DEA90"},
            {ItemType.SPECIALCARBINE_MK2_CLIP_INCENDIARY, "0xDE011286"},
            {ItemType.SPECIALCARBINE_MK2_CLIP_TRACER, "0x8765C68A"},
            {ItemType.SPECIALCARBINE_VARMOD_LOWRIDER, "0x730154F2"},
            {ItemType.SWITCHBLADE_VARMOD_VAR1, "0x5B3E7DB6"},
            {ItemType.SWITCHBLADE_VARMOD_VAR2, "0xE7939662"},
            {ItemType.VINTAGEPISTOL_CLIP_02, "0x33BA12E8"},
        };
        public static Dictionary<uint, int> WeaponAttachSlot = new Dictionary<uint, int>()
        {
            {100416529, 3 },//Sniperrifle
            {101631238, 3 },//Fireextinguisher
            { 125959754, 3 },//Compactlauncher
            { 126349499, 0 },//Snowball
            { 137902532, 0 },//Vintagepistol
            { 171789620, 1 },//Combatpdw
            { 177293209, 3 },//Heavysniper_mk2
            { 205991906, 3 },//Heavysniper
            { 317205821, 2 },//Autoshotgun
            { 324215364, 1 },//Microsmg
            { 419712736, 0 },//Wrench
            { 453432689, 0 },//Pistol
            { 487013001, 2 },//Pumpshotgun
            { 584646201, 0 },//Appistol
            { 600439132, 0 },//Ball
            { 615608432, 0 },//Molotov
            { 727643628, 0 },//CeramicPistol
            { 736523883, 1 },//Smg
            { 741814745, 0 },//Stickybomb
            { 883325847, 0 },//Petrolcan
            { 911657153, 0 },//Stungun
            { 940833800, 0 },//Stone_hatchet
            { 961495388, 3 },//Assaultrifle_mk2
            { 984333226, 2 },//Heavyshotgun
            { 1119849093, 3 },//Minigun
            { 1141786504, 0 },//Golfclub
            { 1198256469, 3 },//Raycarbine
            { 1198879012, 0 },//Flaregun
            { 1233104067, 0 },//Flare
            { 1305664598, 3 },//Grenadelauncher_smoke
            { 1317494643, 0 },//Hammer
            { 1432025498, 2 },//Pumpshotgun_mk2
            { 1593441988, 0 },//Combatpistol
            { 1627465347, 3 },//Gusenberg
            { 1649403952, 3 },//Compactrifle
            { 1672152130, 3 },//Hominglauncher
            { 1737195953, 0 },//Nightstick
            { 1785463520, 3 },//Marksmanrifle_mk2
            { 1834241177, 3 },//Railgun
            { 2017895192, 2 },//Sawnoffshotgun
            { 2024373456, 1 },//Smg_mk2
            { 2132975508, 3 },//Bullpuprifle
            { 2138347493, 0 },//Firework
            { 2144741730, 3 },//Combatmg
            { 2210333304, 3 },//Carbinerifle
            { 2227010557, 0 },//Crowbar
            { 2228681469, 3 },//Bullpuprifle_mk2
            { 2285322324, 0 },//Snspistol_mk2
            { 2343591895, 0 },//Flashlight
            { 2381443905, 0 },//Proximine
            { 2441047180, 0 },//NavyRevolver
            { 2460120199, 0 },//Dagger
            { 2481070269, 0 },//Grenade
            { 2484171525, 0 },//Poolcue
            { 2508868239, 0 },//Bat
            { 2526821735, 3 },//Specialcarbine_mk2
            { 2548703416, 0 },//Doubleaction
            { 2578377531, 0 },//Pistol50
            { 2578778090, 0 },//Knife
            { 2634544996, 3 },//Mg
            { 2640438543, 2 },//Bullpupshotgun
            { 2694266206, 0 },//Bzgas
            { 2726580491, 3 },//Grenadelauncher
            { 2828843422, 0 },//Musket
            { 2937143193, 3 },//Advancedrifle
            { 2939590305, 0 },//Raypistol
            { 2982836145, 3 },//Rpg
            { 3056410471, 3 },//Rayminigun
            { 3125143736, 0 },//Pipebomb
            { 3126027122, 0 },//HazardCan
            { 3173288789, 1 },//Minismg
            { 3218215474, 0 },//Snspistol
            { 3219281620, 0 },//Pistol_mk2
            { 3220176749, 3 },//Assaultrifle
            { 3231910285, 3 },//Specialcarbine
            { 3249783761, 0 },//Revolver
            { 3342088282, 3 },//Marksmanrifle
            { 3415619887, 0 },//Revolver_mk2
            { 3441901897, 0 },//Battleaxe
            { 3523564046, 0 },//Heavypistol
            { 3638508604, 0 },//Knuckle
            { 3675956304, 0 },//Machinepistol
            { 3686625920, 3 },//Combatmg_mk2
            { 3696079510, 0 },//Marksmanpistol
            { 3713923289, 0 },//Machete
            { 3756226112, 0 },//Switchblade
            { 3800352039, 2 },//Assaultshotgun
            { 4019527611, 2 },//Dbshotgun
            { 4024951519, 1 },//Assaultsmg
            { 4191993645, 0 },//Hatchet
            { 4192643659, 0 },//Bottle
            { 4208062921, 3 },//Carbinerifle_mk2
            { 4222310262, 3 },//Parachute
            { 4256991824, 0 }//Smokegrenade
        };

        // [Modification]
        public static Dictionary<ItemType, List<ItemType>> ComponentsTypetoType = new Dictionary<ItemType, List<ItemType>>()
        {
            {ItemType.ADVANCEDRIFLE_CLIP_02, new List<ItemType> { ItemType.Advancedrifle } },
            {ItemType.ADVANCEDRIFLE_VARMOD_LUXE, new List<ItemType> { ItemType.Advancedrifle } },
            {ItemType.APPISTOL_CLIP_02, new List<ItemType> { ItemType.Appistol } },
            {ItemType.APPISTOL_VARMOD_LUXE, new List<ItemType> { ItemType.Appistol } },
            {ItemType.ASSAULTRIFLE_CLIP_02, new List<ItemType> { ItemType.Assaultrifle } },
            {ItemType.ASSAULTRIFLE_CLIP_03, new List<ItemType> { ItemType.Assaultrifle } },
            {ItemType.ASSAULTRIFLE_MK2_CAMO, new List<ItemType> { ItemType.Assaultrifle_mk2 } },
            {ItemType.ASSAULTRIFLE_MK2_CAMO_02, new List<ItemType> { ItemType.Assaultrifle_mk2 } },
            {ItemType.ASSAULTRIFLE_MK2_CAMO_03, new List<ItemType> { ItemType.Assaultrifle_mk2 } },
            {ItemType.ASSAULTRIFLE_MK2_CAMO_04, new List<ItemType> { ItemType.Assaultrifle_mk2 } },
            {ItemType.ASSAULTRIFLE_MK2_CAMO_05, new List<ItemType> { ItemType.Assaultrifle_mk2 } },
            {ItemType.ASSAULTRIFLE_MK2_CAMO_06, new List<ItemType> { ItemType.Assaultrifle_mk2 } },
            {ItemType.ASSAULTRIFLE_MK2_CAMO_07, new List<ItemType> { ItemType.Assaultrifle_mk2 } },
            {ItemType.ASSAULTRIFLE_MK2_CAMO_08, new List<ItemType> { ItemType.Assaultrifle_mk2 } },
            {ItemType.ASSAULTRIFLE_MK2_CAMO_09, new List<ItemType> { ItemType.Assaultrifle_mk2 } },
            {ItemType.ASSAULTRIFLE_MK2_CAMO_10, new List<ItemType> { ItemType.Assaultrifle_mk2 } },
            {ItemType.ASSAULTRIFLE_MK2_CAMO_IND_01, new List<ItemType> { ItemType.Assaultrifle_mk2 } },
            {ItemType.ASSAULTRIFLE_MK2_CLIP_02, new List<ItemType> { ItemType.Assaultrifle_mk2 } },
            {ItemType.ASSAULTRIFLE_MK2_CLIP_ARMORPIERCING, new List<ItemType> { ItemType.Assaultrifle_mk2 } },
            {ItemType.ASSAULTRIFLE_MK2_CLIP_FMJ, new List<ItemType> { ItemType.Assaultrifle_mk2 } },
            {ItemType.ASSAULTRIFLE_MK2_CLIP_INCENDIARY, new List<ItemType> { ItemType.Assaultrifle_mk2 } },
            {ItemType.ASSAULTRIFLE_MK2_CLIP_TRACER, new List<ItemType> { ItemType.Assaultrifle_mk2 } },
            {ItemType.ASSAULTRIFLE_VARMOD_LUXE, new List<ItemType> { ItemType.Assaultrifle } },
            {ItemType.ASSAULTSHOTGUN_CLIP_02, new List<ItemType> { ItemType.Assaultshotgun } },
            {ItemType.ASSAULTSMG_CLIP_02, new List<ItemType> { ItemType.Assaultsmg } },
            {ItemType.ASSAULTSMG_VARMOD_LOWRIDER, new List<ItemType> { ItemType.Assaultsmg } },

            {ItemType.AT_AR_AFGRIP, new List<ItemType> { ItemType.Combatpdw, ItemType.Assaultshotgun, ItemType.Bullpupshotgun, ItemType.Heavyshotgun, ItemType.Assaultrifle, ItemType.Carbinerifle, ItemType.Specialcarbine, ItemType.Bullpuprifle, ItemType.Combatmg, ItemType.Marksmanrifle } },
            {ItemType.AT_AR_AFGRIP_02, new List<ItemType> { ItemType.Assaultrifle_mk2, ItemType.Carbinerifle_mk2, ItemType.Specialcarbine_mk2, ItemType.Bullpuprifle_mk2, ItemType.Combatmg_mk2, ItemType.Marksmanrifle_mk2 } },
            {ItemType.AT_AR_BARREL_02, new List<ItemType> { ItemType.Assaultrifle_mk2 } },
            {ItemType.AT_AR_FLSH, new List<ItemType> { ItemType.Smg, ItemType.Assaultsmg, ItemType.Smg_mk2, ItemType.Combatpdw, ItemType.Pumpshotgun, ItemType.Assaultshotgun, ItemType.Bullpupshotgun, ItemType.Pumpshotgun_mk2, ItemType.Heavyshotgun, ItemType.Combatshotgun, ItemType.Assaultrifle, ItemType.Carbinerifle, ItemType.Advancedrifle, ItemType.Specialcarbine, ItemType.Bullpuprifle, ItemType.Bullpuprifle_mk2, ItemType.Specialcarbine_mk2, ItemType.Assaultrifle_mk2, ItemType.Carbinerifle_mk2, ItemType.Militaryrifle, ItemType.Marksmanrifle_mk2, ItemType.Marksmanrifle } },
            {ItemType.AT_AR_SUPP, new List<ItemType> { ItemType.Assaultshotgun, ItemType.Combatshotgun, ItemType.Carbinerifle, ItemType.Advancedrifle, ItemType.Bullpuprifle, ItemType.Bullpuprifle_mk2, ItemType.Carbinerifle_mk2, ItemType.Militaryrifle, ItemType.Marksmanrifle_mk2, ItemType.Marksmanrifle } },
            {ItemType.AT_AR_SUPP_02, new List<ItemType> { ItemType.Sniperrifle, ItemType.Pistol50, ItemType.Microsmg, ItemType.Assaultsmg, ItemType.Bullpuprifle, ItemType.Heavyshotgun, ItemType.Assaultrifle, ItemType.Specialcarbine, ItemType.Specialcarbine_mk2 } },
            {ItemType.AT_BP_BARREL_02, new List<ItemType> { ItemType.Bullpuprifle_mk2 } },
            {ItemType.AT_CR_BARREL_02, new List<ItemType> { ItemType.Carbinerifle_mk2 } },
            {ItemType.AT_MG_BARREL_02, new List<ItemType> { ItemType.Combatmg_mk2 } },
            {ItemType.AT_MRFL_BARREL_02, new List<ItemType> { ItemType.Marksmanrifle_mk2 } },
            {ItemType.AT_MUZZLE_01, new List<ItemType> { ItemType.Marksmanrifle_mk2, ItemType.Smg_mk2, ItemType.Bullpuprifle_mk2, ItemType.Specialcarbine_mk2, ItemType.Assaultrifle_mk2, ItemType.Carbinerifle_mk2, ItemType.Combatmg } },
            {ItemType.AT_MUZZLE_02, new List<ItemType> { ItemType.Marksmanrifle_mk2, ItemType.Smg_mk2, ItemType.Bullpuprifle_mk2, ItemType.Specialcarbine_mk2, ItemType.Assaultrifle_mk2, ItemType.Carbinerifle_mk2, ItemType.Combatmg } },
            {ItemType.AT_MUZZLE_03, new List<ItemType> { ItemType.Marksmanrifle_mk2, ItemType.Smg_mk2, ItemType.Bullpuprifle_mk2, ItemType.Specialcarbine_mk2, ItemType.Assaultrifle_mk2, ItemType.Carbinerifle_mk2, ItemType.Combatmg } },
            {ItemType.AT_MUZZLE_04, new List<ItemType> { ItemType.Marksmanrifle_mk2, ItemType.Smg_mk2, ItemType.Bullpuprifle_mk2, ItemType.Specialcarbine_mk2, ItemType.Assaultrifle_mk2, ItemType.Carbinerifle_mk2, ItemType.Combatmg } },
            {ItemType.AT_MUZZLE_05, new List<ItemType> { ItemType.Marksmanrifle_mk2, ItemType.Smg_mk2, ItemType.Bullpuprifle_mk2, ItemType.Specialcarbine_mk2, ItemType.Assaultrifle_mk2, ItemType.Carbinerifle_mk2, ItemType.Combatmg } },
            {ItemType.AT_MUZZLE_06, new List<ItemType> { ItemType.Marksmanrifle_mk2, ItemType.Smg_mk2, ItemType.Bullpuprifle_mk2, ItemType.Specialcarbine_mk2, ItemType.Assaultrifle_mk2, ItemType.Carbinerifle_mk2, ItemType.Combatmg } },
            {ItemType.AT_MUZZLE_07, new List<ItemType> { ItemType.Marksmanrifle_mk2, ItemType.Smg_mk2, ItemType.Bullpuprifle_mk2, ItemType.Specialcarbine_mk2, ItemType.Assaultrifle_mk2, ItemType.Carbinerifle_mk2, ItemType.Combatmg } },
            {ItemType.AT_MUZZLE_08, new List<ItemType> { ItemType.Marksmanrifle_mk2, ItemType.Smg_mk2, ItemType.Bullpuprifle_mk2, ItemType.Specialcarbine_mk2, ItemType.Assaultrifle_mk2, ItemType.Carbinerifle_mk2, ItemType.Combatmg } },
            {ItemType.AT_MUZZLE_09, new List<ItemType> { ItemType.Marksmanrifle_mk2, ItemType.Smg_mk2, ItemType.Bullpuprifle_mk2, ItemType.Specialcarbine_mk2, ItemType.Assaultrifle_mk2, ItemType.Carbinerifle_mk2, ItemType.Combatmg } },
            {ItemType.AT_PI_COMP, new List<ItemType> { ItemType.Pistol_mk2 } },
            {ItemType.AT_PI_COMP_02, new List<ItemType> { ItemType.Snspistol_mk2 } },
            {ItemType.AT_PI_COMP_03, new List<ItemType> { ItemType.Heavyrevolver_mk2 } },
            {ItemType.AT_PI_FLSH, new List<ItemType> { ItemType.Pistol, ItemType.Combatpistol, ItemType.Appistol, ItemType.Pistol50, ItemType.Heavypistol, ItemType.Heavyrevolver_mk2, ItemType.Microsmg } },
            {ItemType.AT_PI_FLSH_02, new List<ItemType> { ItemType.Pistol_mk2 } },
            {ItemType.AT_PI_FLSH_03, new List<ItemType> { ItemType.Snspistol_mk2 } },
            {ItemType.AT_PI_RAIL, new List<ItemType> { ItemType.Pistol_mk2 } },
            {ItemType.AT_PI_RAIL_02, new List<ItemType> { ItemType.Snspistol_mk2 } },
            {ItemType.AT_PI_SUPP, new List<ItemType> { ItemType.Combatpistol, ItemType.Appistol, ItemType.Heavypistol, ItemType.Vintagepistol, ItemType.Smg, ItemType.Smg_mk2, ItemType.Machinepistol } },
            {ItemType.AT_PI_SUPP_02, new List<ItemType> { ItemType.Pistol, ItemType.Snspistol_mk2, ItemType.Pistol_mk2 } },
            {ItemType.AT_SB_BARREL_02, new List<ItemType> { ItemType.Smg_mk2 } },
            {ItemType.AT_SC_BARREL_02, new List<ItemType> { ItemType.Specialcarbine_mk2 } },
            {ItemType.AT_SCOPE_LARGE, new List<ItemType> { ItemType.Sniperrifle, ItemType.Heavysniper } },
            {ItemType.AT_SCOPE_LARGE_FIXED_ZOOM, new List<ItemType> { ItemType.Marksmanrifle } },
            {ItemType.AT_SCOPE_LARGE_FIXED_ZOOM_MK2, new List<ItemType> { ItemType.Marksmanrifle_mk2 } },
            {ItemType.AT_SCOPE_LARGE_MK2, new List<ItemType> { ItemType.Heavysniper_mk2 } },
            {ItemType.AT_SCOPE_MACRO, new List<ItemType> { ItemType.Microsmg, ItemType.Assaultsmg, ItemType.Assaultrifle } },
            {ItemType.AT_SCOPE_MACRO_02, new List<ItemType> { ItemType.Smg } },
            {ItemType.AT_SCOPE_MACRO_02_MK2, new List<ItemType> { ItemType.Bullpuprifle_mk2 } },
            {ItemType.AT_SCOPE_MACRO_02_SMG_MK2, new List<ItemType> { ItemType.Smg_mk2 } },
            {ItemType.AT_SCOPE_MACRO_MK2, new List<ItemType> { ItemType.Heavyrevolver_mk2, ItemType.Pumpshotgun_mk2, ItemType.Specialcarbine_mk2, ItemType.Assaultrifle_mk2, ItemType.Carbinerifle_mk2 } },
            {ItemType.AT_SCOPE_MAX, new List<ItemType> { ItemType.Sniperrifle, ItemType.Heavysniper, ItemType.Heavysniper_mk2 } },
            {ItemType.AT_SCOPE_MEDIUM, new List<ItemType> { ItemType.Carbinerifle, ItemType.Specialcarbine, ItemType.Combatmg } },
            {ItemType.AT_SCOPE_MEDIUM_MK2, new List<ItemType> { ItemType.Specialcarbine_mk2, ItemType.Assaultrifle_mk2, ItemType.Carbinerifle_mk2, ItemType.Combatmg_mk2, ItemType.Marksmanrifle_mk2 } },
            {ItemType.AT_SCOPE_NV, new List<ItemType> { ItemType.Heavysniper_mk2 } },
            {ItemType.AT_SCOPE_SMALL, new List<ItemType> { ItemType.Combatpdw, ItemType.Advancedrifle, ItemType.Bullpuprifle, ItemType.Militaryrifle } },
            {ItemType.AT_SCOPE_SMALL_02, new List<ItemType> { ItemType.Mg } },
            {ItemType.AT_SCOPE_SMALL_MK2, new List<ItemType> { ItemType.Pumpshotgun_mk2, ItemType.Bullpuprifle_mk2, ItemType.Combatmg_mk2 } },
            {ItemType.AT_SCOPE_SMALL_SMG_MK2, new List<ItemType> { ItemType.Smg_mk2 } },
            {ItemType.AT_SCOPE_THERMAL, new List<ItemType> { ItemType.Heavysniper_mk2 } },
            {ItemType.AT_SIGHTS, new List<ItemType> { ItemType.Heavyrevolver_mk2, ItemType.Pumpshotgun_mk2, ItemType.Bullpuprifle_mk2, ItemType.Specialcarbine_mk2, ItemType.Assaultrifle_mk2, ItemType.Carbinerifle_mk2, ItemType.Combatmg_mk2, ItemType.Marksmanrifle_mk2 } },
            {ItemType.AT_SIGHTS_SMG, new List<ItemType> { ItemType.Smg_mk2 } },
            {ItemType.AT_SR_BARREL_02, new List<ItemType> { ItemType.Heavysniper_mk2 } },
            {ItemType.AT_SR_SUPP, new List<ItemType> { ItemType.Pumpshotgun } },
            {ItemType.AT_SR_SUPP_03, new List<ItemType> { ItemType.Pumpshotgun_mk2, ItemType.Heavysniper_mk2 } },

            {ItemType.BULLPUPRIFLE_CLIP_02, new List<ItemType> { ItemType.Bullpuprifle } },
            {ItemType.BULLPUPRIFLE_MK2_CAMO, new List<ItemType> { ItemType.Bullpuprifle_mk2 } },
            {ItemType.BULLPUPRIFLE_MK2_CAMO_02, new List<ItemType> { ItemType.Bullpuprifle_mk2 } },
            {ItemType.BULLPUPRIFLE_MK2_CAMO_03, new List<ItemType> { ItemType.Bullpuprifle_mk2 } },
            {ItemType.BULLPUPRIFLE_MK2_CAMO_04, new List<ItemType> { ItemType.Bullpuprifle_mk2 } },
            {ItemType.BULLPUPRIFLE_MK2_CAMO_05, new List<ItemType> { ItemType.Bullpuprifle_mk2 } },
            {ItemType.BULLPUPRIFLE_MK2_CAMO_06, new List<ItemType> { ItemType.Bullpuprifle_mk2 } },
            {ItemType.BULLPUPRIFLE_MK2_CAMO_07, new List<ItemType> { ItemType.Bullpuprifle_mk2 } },
            {ItemType.BULLPUPRIFLE_MK2_CAMO_08, new List<ItemType> { ItemType.Bullpuprifle_mk2 } },
            {ItemType.BULLPUPRIFLE_MK2_CAMO_09, new List<ItemType> { ItemType.Bullpuprifle_mk2 } },
            {ItemType.BULLPUPRIFLE_MK2_CAMO_10, new List<ItemType> { ItemType.Bullpuprifle_mk2 } },
            {ItemType.BULLPUPRIFLE_MK2_CAMO_IND_01, new List<ItemType> { ItemType.Bullpuprifle_mk2 } },
            {ItemType.BULLPUPRIFLE_MK2_CLIP_02, new List<ItemType> { ItemType.Bullpuprifle_mk2 } },
            {ItemType.BULLPUPRIFLE_MK2_CLIP_ARMORPIERCING, new List<ItemType> { ItemType.Bullpuprifle_mk2 } },
            {ItemType.BULLPUPRIFLE_MK2_CLIP_FMJ, new List<ItemType> { ItemType.Bullpuprifle_mk2 } },
            {ItemType.BULLPUPRIFLE_MK2_CLIP_INCENDIARY, new List<ItemType> { ItemType.Bullpuprifle_mk2 } },
            {ItemType.BULLPUPRIFLE_MK2_CLIP_TRACER, new List<ItemType> { ItemType.Bullpuprifle_mk2 } },
            {ItemType.BULLPUPRIFLE_VARMOD_LOW, new List<ItemType> { ItemType.Bullpuprifle } },
            {ItemType.CARBINERIFLE_CLIP_02, new List<ItemType> { ItemType.Carbinerifle } },
            {ItemType.CARBINERIFLE_CLIP_03, new List<ItemType> { ItemType.Carbinerifle } },
            {ItemType.CARBINERIFLE_MK2_CAMO, new List<ItemType> { ItemType.Carbinerifle_mk2 } },
            {ItemType.CARBINERIFLE_MK2_CAMO_02, new List<ItemType> { ItemType.Carbinerifle_mk2 } },
            {ItemType.CARBINERIFLE_MK2_CAMO_03, new List<ItemType> { ItemType.Carbinerifle_mk2 } },
            {ItemType.CARBINERIFLE_MK2_CAMO_04, new List<ItemType> { ItemType.Carbinerifle_mk2 } },
            {ItemType.CARBINERIFLE_MK2_CAMO_05, new List<ItemType> { ItemType.Carbinerifle_mk2 } },
            {ItemType.CARBINERIFLE_MK2_CAMO_06, new List<ItemType> { ItemType.Carbinerifle_mk2 } },
            {ItemType.CARBINERIFLE_MK2_CAMO_07, new List<ItemType> { ItemType.Carbinerifle_mk2 } },
            {ItemType.CARBINERIFLE_MK2_CAMO_08, new List<ItemType> { ItemType.Carbinerifle_mk2 } },
            {ItemType.CARBINERIFLE_MK2_CAMO_09, new List<ItemType> { ItemType.Carbinerifle_mk2 } },
            {ItemType.CARBINERIFLE_MK2_CAMO_10, new List<ItemType> { ItemType.Carbinerifle_mk2 } },
            {ItemType.CARBINERIFLE_MK2_CAMO_IND_01, new List<ItemType> { ItemType.Carbinerifle_mk2 } },
            {ItemType.CARBINERIFLE_MK2_CLIP_02, new List<ItemType> { ItemType.Carbinerifle_mk2 } },
            {ItemType.CARBINERIFLE_MK2_CLIP_ARMORPIERCING, new List<ItemType> { ItemType.Carbinerifle_mk2 } },
            {ItemType.CARBINERIFLE_MK2_CLIP_FMJ, new List<ItemType> { ItemType.Carbinerifle_mk2 } },
            {ItemType.CARBINERIFLE_MK2_CLIP_INCENDIARY, new List<ItemType> { ItemType.Carbinerifle_mk2 } },
            {ItemType.CARBINERIFLE_MK2_CLIP_TRACER, new List<ItemType> { ItemType.Carbinerifle_mk2 } },
            {ItemType.CARBINERIFLE_VARMOD_LUXE, new List<ItemType> { ItemType.Carbinerifle } },
            {ItemType.CERAMICPISTOL_CLIP_02, new List<ItemType> { ItemType.Ceramicpistol } },
            {ItemType.CERAMICPISTOL_SUPP, new List<ItemType> { ItemType.Ceramicpistol } },
            {ItemType.COMBATMG_CLIP_02, new List<ItemType> { ItemType.Combatmg } },
            {ItemType.COMBATMG_MK2_CAMO, new List<ItemType> { ItemType.Combatmg_mk2 } },
            {ItemType.COMBATMG_MK2_CAMO_02, new List<ItemType> { ItemType.Combatmg_mk2 } },
            {ItemType.COMBATMG_MK2_CAMO_03, new List<ItemType> { ItemType.Combatmg_mk2 } },
            {ItemType.COMBATMG_MK2_CAMO_04, new List<ItemType> { ItemType.Combatmg_mk2 } },
            {ItemType.COMBATMG_MK2_CAMO_05, new List<ItemType> { ItemType.Combatmg_mk2 } },
            {ItemType.COMBATMG_MK2_CAMO_06, new List<ItemType> { ItemType.Combatmg_mk2 } },
            {ItemType.COMBATMG_MK2_CAMO_07, new List<ItemType> { ItemType.Combatmg_mk2 } },
            {ItemType.COMBATMG_MK2_CAMO_08, new List<ItemType> { ItemType.Combatmg_mk2 } },
            {ItemType.COMBATMG_MK2_CAMO_09, new List<ItemType> { ItemType.Combatmg_mk2 } },
            {ItemType.COMBATMG_MK2_CAMO_10, new List<ItemType> { ItemType.Combatmg_mk2 } },
            {ItemType.COMBATMG_MK2_CAMO_IND_01, new List<ItemType> { ItemType.Combatmg_mk2 } },
            {ItemType.COMBATMG_MK2_CLIP_02, new List<ItemType> { ItemType.Combatmg_mk2 } },
            {ItemType.COMBATMG_MK2_CLIP_ARMORPIERCING, new List<ItemType> { ItemType.Combatmg_mk2 } },
            {ItemType.COMBATMG_MK2_CLIP_FMJ, new List<ItemType> { ItemType.Combatmg_mk2 } },
            {ItemType.COMBATMG_MK2_CLIP_INCENDIARY, new List<ItemType> { ItemType.Combatmg_mk2 } },
            {ItemType.COMBATMG_MK2_CLIP_TRACER, new List<ItemType> { ItemType.Combatmg_mk2 } },
            {ItemType.COMBATMG_VARMOD_LOWRIDER, new List<ItemType> { ItemType.Combatmg } },
            {ItemType.COMBATPDW_CLIP_02, new List<ItemType> { ItemType.Combatpdw } },
            {ItemType.COMBATPDW_CLIP_03, new List<ItemType> { ItemType.Combatpdw } },
            {ItemType.COMBATPISTOL_CLIP_02, new List<ItemType> { ItemType.Combatpistol } },
            {ItemType.COMBATPISTOL_VARMOD_LOWRIDER, new List<ItemType> { ItemType.Combatpistol } },
            {ItemType.COMPACTRIFLE_CLIP_02, new List<ItemType> { ItemType.Compactrifle } },
            {ItemType.COMPACTRIFLE_CLIP_03, new List<ItemType> { ItemType.Compactrifle } },
            {ItemType.GUSENBERG_CLIP_02, new List<ItemType> { ItemType.Gusenberg } },
            {ItemType.HEAVYPISTOL_CLIP_02, new List<ItemType> { ItemType.Heavypistol } },
            {ItemType.HEAVYPISTOL_VARMOD_LUXE, new List<ItemType> { ItemType.Heavypistol } },
            {ItemType.HEAVYSHOTGUN_CLIP_02, new List<ItemType> { ItemType.Heavyshotgun } },
            {ItemType.HEAVYSHOTGUN_CLIP_03, new List<ItemType> { ItemType.Heavyshotgun } },
            {ItemType.HEAVYSNIPER_MK2_CAMO, new List<ItemType> { ItemType.Heavysniper_mk2 } },
            {ItemType.HEAVYSNIPER_MK2_CAMO_02, new List<ItemType> { ItemType.Heavysniper_mk2 } },
            {ItemType.HEAVYSNIPER_MK2_CAMO_03, new List<ItemType> { ItemType.Heavysniper_mk2 } },
            {ItemType.HEAVYSNIPER_MK2_CAMO_04, new List<ItemType> { ItemType.Heavysniper_mk2 } },
            {ItemType.HEAVYSNIPER_MK2_CAMO_05, new List<ItemType> { ItemType.Heavysniper_mk2 } },
            {ItemType.HEAVYSNIPER_MK2_CAMO_06, new List<ItemType> { ItemType.Heavysniper_mk2 } },
            {ItemType.HEAVYSNIPER_MK2_CAMO_07, new List<ItemType> { ItemType.Heavysniper_mk2 } },
            {ItemType.HEAVYSNIPER_MK2_CAMO_08, new List<ItemType> { ItemType.Heavysniper_mk2 } },
            {ItemType.HEAVYSNIPER_MK2_CAMO_09, new List<ItemType> { ItemType.Heavysniper_mk2 } },
            {ItemType.HEAVYSNIPER_MK2_CAMO_10, new List<ItemType> { ItemType.Heavysniper_mk2 } },
            {ItemType.HEAVYSNIPER_MK2_CAMO_IND_01, new List<ItemType> { ItemType.Heavysniper_mk2 } },
            {ItemType.HEAVYSNIPER_MK2_CLIP_02, new List<ItemType> { ItemType.Heavysniper_mk2 } },
            {ItemType.HEAVYSNIPER_MK2_CLIP_ARMORPIERCING, new List<ItemType> { ItemType.Heavysniper_mk2 } },
            {ItemType.HEAVYSNIPER_MK2_CLIP_EXPLOSIVE, new List<ItemType> { ItemType.Heavysniper_mk2 } },
            {ItemType.HEAVYSNIPER_MK2_CLIP_FMJ, new List<ItemType> { ItemType.Heavysniper_mk2 } },
            {ItemType.HEAVYSNIPER_MK2_CLIP_INCENDIARY, new List<ItemType> { ItemType.Heavysniper_mk2 } },
            {ItemType.KNUCKLE_VARMOD_BALLAS, new List<ItemType> { ItemType.Knuckle } },
            {ItemType.KNUCKLE_VARMOD_DIAMOND, new List<ItemType> { ItemType.Knuckle } },
            {ItemType.KNUCKLE_VARMOD_DOLLAR, new List<ItemType> { ItemType.Knuckle } },
            {ItemType.KNUCKLE_VARMOD_HATE, new List<ItemType> { ItemType.Knuckle } },
            {ItemType.KNUCKLE_VARMOD_KING, new List<ItemType> { ItemType.Knuckle } },
            {ItemType.KNUCKLE_VARMOD_LOVE, new List<ItemType> { ItemType.Knuckle } },
            {ItemType.KNUCKLE_VARMOD_PIMP, new List<ItemType> { ItemType.Knuckle } },
            {ItemType.KNUCKLE_VARMOD_PLAYER, new List<ItemType> { ItemType.Knuckle } },
            {ItemType.KNUCKLE_VARMOD_VAGOS, new List<ItemType> { ItemType.Knuckle } },
            {ItemType.MACHINEPISTOL_CLIP_02, new List<ItemType> { ItemType.Machinepistol } },
            {ItemType.MACHINEPISTOL_CLIP_03, new List<ItemType> { ItemType.Machinepistol } },
            {ItemType.MARKSMANRIFLE_CLIP_02, new List<ItemType> { ItemType.Marksmanrifle } },
            {ItemType.MARKSMANRIFLE_MK2_CAMO, new List<ItemType> { ItemType.Marksmanrifle_mk2 } },
            {ItemType.MARKSMANRIFLE_MK2_CAMO_02, new List<ItemType> { ItemType.Marksmanrifle_mk2 } },
            {ItemType.MARKSMANRIFLE_MK2_CAMO_03, new List<ItemType> { ItemType.Marksmanrifle_mk2 } },
            {ItemType.MARKSMANRIFLE_MK2_CAMO_04, new List<ItemType> { ItemType.Marksmanrifle_mk2 } },
            {ItemType.MARKSMANRIFLE_MK2_CAMO_05, new List<ItemType> { ItemType.Marksmanrifle_mk2 } },
            {ItemType.MARKSMANRIFLE_MK2_CAMO_06, new List<ItemType> { ItemType.Marksmanrifle_mk2 } },
            {ItemType.MARKSMANRIFLE_MK2_CAMO_07, new List<ItemType> { ItemType.Marksmanrifle_mk2 } },
            {ItemType.MARKSMANRIFLE_MK2_CAMO_08, new List<ItemType> { ItemType.Marksmanrifle_mk2 } },
            {ItemType.MARKSMANRIFLE_MK2_CAMO_09, new List<ItemType> { ItemType.Marksmanrifle_mk2 } },
            {ItemType.MARKSMANRIFLE_MK2_CAMO_10, new List<ItemType> { ItemType.Marksmanrifle_mk2 } },
            {ItemType.MARKSMANRIFLE_MK2_CAMO_IND_01, new List<ItemType> { ItemType.Marksmanrifle_mk2 } },
            {ItemType.MARKSMANRIFLE_MK2_CLIP_02, new List<ItemType> { ItemType.Marksmanrifle_mk2 } },
            {ItemType.MARKSMANRIFLE_MK2_CLIP_ARMORPIERCING, new List<ItemType> { ItemType.Marksmanrifle_mk2 } },
            {ItemType.MARKSMANRIFLE_MK2_CLIP_FMJ, new List<ItemType> { ItemType.Marksmanrifle_mk2 } },
            {ItemType.MARKSMANRIFLE_MK2_CLIP_INCENDIARY, new List<ItemType> { ItemType.Marksmanrifle_mk2 } },
            {ItemType.MARKSMANRIFLE_MK2_CLIP_TRACER, new List<ItemType> { ItemType.Marksmanrifle_mk2 } },
            {ItemType.MARKSMANRIFLE_VARMOD_LUXE, new List<ItemType> { ItemType.Marksmanrifle } },
            {ItemType.MG_CLIP_02, new List<ItemType> { ItemType.Mg } },
            {ItemType.MG_VARMOD_LOWRIDER, new List<ItemType> { ItemType.Mg } },
            {ItemType.MICROSMG_CLIP_02, new List<ItemType> { ItemType.Microsmg } },
            {ItemType.MICROSMG_VARMOD_LUXE, new List<ItemType> { ItemType.Microsmg } },
            {ItemType.MILITARYRIFLE_CLIP_02, new List<ItemType> { ItemType.Militaryrifle } },
            {ItemType.MILITARYRIFLE_SIGHT_01, new List<ItemType> { ItemType.Militaryrifle } },
            {ItemType.MINISMG_CLIP_02, new List<ItemType> { ItemType.Minismg } },
            {ItemType.PISTOL50_CLIP_02, new List<ItemType> { ItemType.Minismg } },
            {ItemType.PISTOL50_VARMOD_LUXE, new List<ItemType> { ItemType.Minismg } },
            {ItemType.PISTOL_CLIP_02, new List<ItemType> { ItemType.Pistol } },
            {ItemType.PISTOL_MK2_CAMO, new List<ItemType> { ItemType.Pistol_mk2 } },
            {ItemType.PISTOL_MK2_CAMO_02, new List<ItemType> { ItemType.Pistol_mk2 } },
            {ItemType.PISTOL_MK2_CAMO_02_SLIDE, new List<ItemType> { ItemType.Pistol_mk2 } },
            {ItemType.PISTOL_MK2_CAMO_03, new List<ItemType> { ItemType.Pistol_mk2 } },
            {ItemType.PISTOL_MK2_CAMO_03_SLIDE, new List<ItemType> { ItemType.Pistol_mk2 } },
            {ItemType.PISTOL_MK2_CAMO_04, new List<ItemType> { ItemType.Pistol_mk2 } },
            {ItemType.PISTOL_MK2_CAMO_04_SLIDE, new List<ItemType> { ItemType.Pistol_mk2 } },
            {ItemType.PISTOL_MK2_CAMO_05, new List<ItemType> { ItemType.Pistol_mk2 } },
            {ItemType.PISTOL_MK2_CAMO_05_SLIDE, new List<ItemType> { ItemType.Pistol_mk2 } },
            {ItemType.PISTOL_MK2_CAMO_06, new List<ItemType> { ItemType.Pistol_mk2 } },
            {ItemType.PISTOL_MK2_CAMO_06_SLIDE, new List<ItemType> { ItemType.Pistol_mk2 } },
            {ItemType.PISTOL_MK2_CAMO_07, new List<ItemType> { ItemType.Pistol_mk2 } },
            {ItemType.PISTOL_MK2_CAMO_07_SLIDE, new List<ItemType> { ItemType.Pistol_mk2 } },
            {ItemType.PISTOL_MK2_CAMO_08, new List<ItemType> { ItemType.Pistol_mk2 } },
            {ItemType.PISTOL_MK2_CAMO_08_SLIDE, new List<ItemType> { ItemType.Pistol_mk2 } },
            {ItemType.PISTOL_MK2_CAMO_09, new List<ItemType> { ItemType.Pistol_mk2 } },
            {ItemType.PISTOL_MK2_CAMO_09_SLIDE, new List<ItemType> { ItemType.Pistol_mk2 } },
            {ItemType.PISTOL_MK2_CAMO_10, new List<ItemType> { ItemType.Pistol_mk2 } },
            {ItemType.PISTOL_MK2_CAMO_10_SLIDE, new List<ItemType> { ItemType.Pistol_mk2 } },
            {ItemType.PISTOL_MK2_CAMO_IND_01, new List<ItemType> { ItemType.Pistol_mk2 } },
            {ItemType.PISTOL_MK2_CAMO_IND_01_SLIDE, new List<ItemType> { ItemType.Pistol_mk2 } },
            {ItemType.PISTOL_MK2_CAMO_SLIDE, new List<ItemType> { ItemType.Pistol_mk2 } },
            {ItemType.PISTOL_MK2_CLIP_02, new List<ItemType> { ItemType.Pistol_mk2 } },
            {ItemType.PISTOL_MK2_CLIP_FMJ, new List<ItemType> { ItemType.Pistol_mk2 } },
            {ItemType.PISTOL_MK2_CLIP_HOLLOWPOINT, new List<ItemType> { ItemType.Pistol_mk2 } },
            {ItemType.PISTOL_MK2_CLIP_INCENDIARY, new List<ItemType> { ItemType.Pistol_mk2 } },
            {ItemType.PISTOL_MK2_CLIP_TRACER, new List<ItemType> { ItemType.Pistol_mk2 } },
            {ItemType.PISTOL_VARMOD_LUXE, new List<ItemType> { ItemType.Pistol } },
            {ItemType.PUMPSHOTGUN_MK2_CAMO, new List<ItemType> { ItemType.Pumpshotgun_mk2 } },
            {ItemType.PUMPSHOTGUN_MK2_CAMO_02, new List<ItemType> { ItemType.Pumpshotgun_mk2 } },
            {ItemType.PUMPSHOTGUN_MK2_CAMO_03, new List<ItemType> { ItemType.Pumpshotgun_mk2 } },
            {ItemType.PUMPSHOTGUN_MK2_CAMO_04, new List<ItemType> { ItemType.Pumpshotgun_mk2 } },
            {ItemType.PUMPSHOTGUN_MK2_CAMO_05, new List<ItemType> { ItemType.Pumpshotgun_mk2 } },
            {ItemType.PUMPSHOTGUN_MK2_CAMO_06, new List<ItemType> { ItemType.Pumpshotgun_mk2 } },
            {ItemType.PUMPSHOTGUN_MK2_CAMO_07, new List<ItemType> { ItemType.Pumpshotgun_mk2 } },
            {ItemType.PUMPSHOTGUN_MK2_CAMO_08, new List<ItemType> { ItemType.Pumpshotgun_mk2 } },
            {ItemType.PUMPSHOTGUN_MK2_CAMO_09, new List<ItemType> { ItemType.Pumpshotgun_mk2 } },
            {ItemType.PUMPSHOTGUN_MK2_CAMO_10, new List<ItemType> { ItemType.Pumpshotgun_mk2 } },
            {ItemType.PUMPSHOTGUN_MK2_CAMO_IND_01, new List<ItemType> { ItemType.Pumpshotgun_mk2 } },
            {ItemType.PUMPSHOTGUN_MK2_CLIP_ARMORPIERCING, new List<ItemType> { ItemType.Pumpshotgun_mk2 } },
            {ItemType.PUMPSHOTGUN_MK2_CLIP_EXPLOSIVE, new List<ItemType> { ItemType.Pumpshotgun_mk2 } },
            {ItemType.PUMPSHOTGUN_MK2_CLIP_HOLLOWPOINT, new List<ItemType> { ItemType.Pumpshotgun_mk2 } },
            {ItemType.PUMPSHOTGUN_MK2_CLIP_INCENDIARY, new List<ItemType> { ItemType.Pumpshotgun_mk2 } },
            {ItemType.PUMPSHOTGUN_VARMOD_LOWRIDER, new List<ItemType> { ItemType.Pumpshotgun } },
            {ItemType.RAYPISTOL_VARMOD_XMAS18, new List<ItemType> { ItemType.Raypistol } },
            {ItemType.REVOLVER_MK2_CAMO, new List<ItemType> { ItemType.Revolver_mk2 } },
            {ItemType.REVOLVER_MK2_CAMO_02, new List<ItemType> { ItemType.Revolver_mk2 } },
            {ItemType.REVOLVER_MK2_CAMO_03, new List<ItemType> { ItemType.Revolver_mk2 } },
            {ItemType.REVOLVER_MK2_CAMO_04, new List<ItemType> { ItemType.Revolver_mk2 } },
            {ItemType.REVOLVER_MK2_CAMO_05, new List<ItemType> { ItemType.Revolver_mk2 } },
            {ItemType.REVOLVER_MK2_CAMO_06, new List<ItemType> { ItemType.Revolver_mk2 } },
            {ItemType.REVOLVER_MK2_CAMO_07, new List<ItemType> { ItemType.Revolver_mk2 } },
            {ItemType.REVOLVER_MK2_CAMO_08, new List<ItemType> { ItemType.Revolver_mk2 } },
            {ItemType.REVOLVER_MK2_CAMO_09, new List<ItemType> { ItemType.Revolver_mk2 } },
            {ItemType.REVOLVER_MK2_CAMO_10, new List<ItemType> { ItemType.Revolver_mk2 } },
            {ItemType.REVOLVER_MK2_CAMO_IND_01, new List<ItemType> { ItemType.Revolver_mk2 } },
            {ItemType.REVOLVER_MK2_CLIP_FMJ, new List<ItemType> { ItemType.Revolver_mk2 } },
            {ItemType.REVOLVER_MK2_CLIP_HOLLOWPOINT, new List<ItemType> { ItemType.Revolver_mk2 } },
            {ItemType.REVOLVER_MK2_CLIP_INCENDIARY, new List<ItemType> { ItemType.Revolver_mk2 } },
            {ItemType.REVOLVER_MK2_CLIP_TRACER, new List<ItemType> { ItemType.Revolver_mk2 } },
            {ItemType.REVOLVER_VARMOD_BOSS, new List<ItemType> { ItemType.Revolver } },
            {ItemType.REVOLVER_VARMOD_GOON, new List<ItemType> { ItemType.Revolver } },
            {ItemType.SAWNOFFSHOTGUN_VARMOD_LUXE, new List<ItemType> { ItemType.Sawnoffshotgun } },
            {ItemType.SMG_CLIP_02, new List<ItemType> { ItemType.Smg } },
            {ItemType.SMG_CLIP_03, new List<ItemType> { ItemType.Smg } },
            {ItemType.SMG_MK2_CAMO, new List<ItemType> { ItemType.Smg_mk2 } },
            {ItemType.SMG_MK2_CAMO_02, new List<ItemType> { ItemType.Smg_mk2 } },
            {ItemType.SMG_MK2_CAMO_03, new List<ItemType> { ItemType.Smg_mk2 } },
            {ItemType.SMG_MK2_CAMO_04, new List<ItemType> { ItemType.Smg_mk2 } },
            {ItemType.SMG_MK2_CAMO_05, new List<ItemType> { ItemType.Smg_mk2 } },
            {ItemType.SMG_MK2_CAMO_06, new List<ItemType> { ItemType.Smg_mk2 } },
            {ItemType.SMG_MK2_CAMO_07, new List<ItemType> { ItemType.Smg_mk2 } },
            {ItemType.SMG_MK2_CAMO_08, new List<ItemType> { ItemType.Smg_mk2 } },
            {ItemType.SMG_MK2_CAMO_09, new List<ItemType> { ItemType.Smg_mk2 } },
            {ItemType.SMG_MK2_CAMO_10, new List<ItemType> { ItemType.Smg_mk2 } },
            {ItemType.SMG_MK2_CAMO_IND_01, new List<ItemType> { ItemType.Smg_mk2 } },
            {ItemType.SMG_MK2_CLIP_02, new List<ItemType> { ItemType.Smg_mk2 } },
            {ItemType.SMG_MK2_CLIP_FMJ, new List<ItemType> { ItemType.Smg_mk2 } },
            {ItemType.SMG_MK2_CLIP_HOLLOWPOINT, new List<ItemType> { ItemType.Smg_mk2 } },
            {ItemType.SMG_MK2_CLIP_INCENDIARY, new List<ItemType> { ItemType.Smg_mk2 } },
            {ItemType.SMG_MK2_CLIP_TRACER, new List<ItemType> { ItemType.Smg_mk2 } },
            {ItemType.SMG_VARMOD_LUXE, new List<ItemType> { ItemType.Smg } },
            {ItemType.SNIPERRIFLE_VARMOD_LUXE, new List<ItemType> { ItemType.Sniperrifle } },
            {ItemType.SNSPISTOL_CLIP_02, new List<ItemType> { ItemType.Snspistol } },
            {ItemType.SNSPISTOL_MK2_CAMO, new List<ItemType> { ItemType.Snspistol_mk2 } },
            {ItemType.SNSPISTOL_MK2_CAMO_02, new List<ItemType> { ItemType.Snspistol_mk2 } },
            {ItemType.SNSPISTOL_MK2_CAMO_02_SLIDE, new List<ItemType> { ItemType.Snspistol_mk2 } },
            {ItemType.SNSPISTOL_MK2_CAMO_03, new List<ItemType> { ItemType.Snspistol_mk2 } },
            {ItemType.SNSPISTOL_MK2_CAMO_03_SLIDE, new List<ItemType> { ItemType.Snspistol_mk2 } },
            {ItemType.SNSPISTOL_MK2_CAMO_04, new List<ItemType> { ItemType.Snspistol_mk2 } },
            {ItemType.SNSPISTOL_MK2_CAMO_04_SLIDE, new List<ItemType> { ItemType.Snspistol_mk2 } },
            {ItemType.SNSPISTOL_MK2_CAMO_05, new List<ItemType> { ItemType.Snspistol_mk2 } },
            {ItemType.SNSPISTOL_MK2_CAMO_05_SLIDE, new List<ItemType> { ItemType.Snspistol_mk2 } },
            {ItemType.SNSPISTOL_MK2_CAMO_06, new List<ItemType> { ItemType.Snspistol_mk2 } },
            {ItemType.SNSPISTOL_MK2_CAMO_06_SLIDE, new List<ItemType> { ItemType.Snspistol_mk2 } },
            {ItemType.SNSPISTOL_MK2_CAMO_07, new List<ItemType> { ItemType.Snspistol_mk2 } },
            {ItemType.SNSPISTOL_MK2_CAMO_07_SLIDE, new List<ItemType> { ItemType.Snspistol_mk2 } },
            {ItemType.SNSPISTOL_MK2_CAMO_08, new List<ItemType> { ItemType.Snspistol_mk2 } },
            {ItemType.SNSPISTOL_MK2_CAMO_08_SLIDE, new List<ItemType> { ItemType.Snspistol_mk2 } },
            {ItemType.SNSPISTOL_MK2_CAMO_09, new List<ItemType> { ItemType.Snspistol_mk2 } },
            {ItemType.SNSPISTOL_MK2_CAMO_09_SLIDE, new List<ItemType> { ItemType.Snspistol_mk2 } },
            {ItemType.SNSPISTOL_MK2_CAMO_10, new List<ItemType> { ItemType.Snspistol_mk2 } },
            {ItemType.SNSPISTOL_MK2_CAMO_10_SLIDE, new List<ItemType> { ItemType.Snspistol_mk2 } },
            {ItemType.SNSPISTOL_MK2_CAMO_IND_01, new List<ItemType> { ItemType.Snspistol_mk2 } },
            {ItemType.SNSPISTOL_MK2_CAMO_IND_01_SLIDE, new List<ItemType> { ItemType.Snspistol_mk2 } },
            {ItemType.SNSPISTOL_MK2_CAMO_SLIDE, new List<ItemType> { ItemType.Snspistol_mk2 } },
            {ItemType.SNSPISTOL_MK2_CLIP_02, new List<ItemType> { ItemType.Snspistol_mk2 } },
            {ItemType.SNSPISTOL_MK2_CLIP_FMJ, new List<ItemType> { ItemType.Snspistol_mk2 } },
            {ItemType.SNSPISTOL_MK2_CLIP_HOLLOWPOINT, new List<ItemType> { ItemType.Snspistol_mk2 } },
            {ItemType.SNSPISTOL_MK2_CLIP_INCENDIARY, new List<ItemType> { ItemType.Snspistol_mk2 } },
            {ItemType.SNSPISTOL_MK2_CLIP_TRACER, new List<ItemType> { ItemType.Snspistol_mk2 } },
            {ItemType.SNSPISTOL_VARMOD_LOWRIDER, new List<ItemType> { ItemType.Snspistol } },
            {ItemType.SPECIALCARBINE_CLIP_02, new List<ItemType> { ItemType.Specialcarbine } },
            {ItemType.SPECIALCARBINE_CLIP_03, new List<ItemType> { ItemType.Specialcarbine } },
            {ItemType.SPECIALCARBINE_MK2_CAMO, new List<ItemType> { ItemType.Specialcarbine_mk2 } },
            {ItemType.SPECIALCARBINE_MK2_CAMO_02, new List<ItemType> { ItemType.Specialcarbine_mk2 } },
            {ItemType.SPECIALCARBINE_MK2_CAMO_03, new List<ItemType> { ItemType.Specialcarbine_mk2 } },
            {ItemType.SPECIALCARBINE_MK2_CAMO_04, new List<ItemType> { ItemType.Specialcarbine_mk2 } },
            {ItemType.SPECIALCARBINE_MK2_CAMO_05, new List<ItemType> { ItemType.Specialcarbine_mk2 } },
            {ItemType.SPECIALCARBINE_MK2_CAMO_06, new List<ItemType> { ItemType.Specialcarbine_mk2 } },
            {ItemType.SPECIALCARBINE_MK2_CAMO_07, new List<ItemType> { ItemType.Specialcarbine_mk2 } },
            {ItemType.SPECIALCARBINE_MK2_CAMO_08, new List<ItemType> { ItemType.Specialcarbine_mk2 } },
            {ItemType.SPECIALCARBINE_MK2_CAMO_09, new List<ItemType> { ItemType.Specialcarbine_mk2 } },
            {ItemType.SPECIALCARBINE_MK2_CAMO_10, new List<ItemType> { ItemType.Specialcarbine_mk2 } },
            {ItemType.SPECIALCARBINE_MK2_CAMO_IND_01, new List<ItemType> { ItemType.Specialcarbine_mk2 } },
            {ItemType.SPECIALCARBINE_MK2_CLIP_02, new List<ItemType> { ItemType.Specialcarbine_mk2 } },
            {ItemType.SPECIALCARBINE_MK2_CLIP_ARMORPIERCING, new List<ItemType> { ItemType.Specialcarbine_mk2 } },
            {ItemType.SPECIALCARBINE_MK2_CLIP_FMJ, new List<ItemType> { ItemType.Specialcarbine_mk2 } },
            {ItemType.SPECIALCARBINE_MK2_CLIP_INCENDIARY, new List<ItemType> { ItemType.Specialcarbine_mk2 } },
            {ItemType.SPECIALCARBINE_MK2_CLIP_TRACER, new List<ItemType> { ItemType.Specialcarbine_mk2 } },
            {ItemType.SPECIALCARBINE_VARMOD_LOWRIDER, new List<ItemType> { ItemType.Specialcarbine } },
            {ItemType.SWITCHBLADE_VARMOD_VAR1, new List<ItemType> { ItemType.Switchblade } },
            {ItemType.SWITCHBLADE_VARMOD_VAR2, new List<ItemType> { ItemType.Switchblade } },
            {ItemType.VINTAGEPISTOL_CLIP_02, new List<ItemType> { ItemType.Vintagepistol } },
        };

        public static List<ItemType> WeaponsItems = new List<ItemType>()
        {
            ItemType.Pistol,
            ItemType.Combatpistol,
            ItemType.Pistol50,
            ItemType.Snspistol,
            ItemType.Heavypistol,
            ItemType.Vintagepistol,
            ItemType.Marksmanpistol,
            ItemType.Revolver,

            ItemType.Stungun,

            ItemType.Appistol,
            ItemType.Flaregun,
            ItemType.Doubleaction,
            ItemType.Ceramicpistol,
            ItemType.Pistol_mk2,
            ItemType.Snspistol_mk2,
            ItemType.Revolver_mk2,
            ItemType.Heavyrevolver_mk2,

            ItemType.Microsmg,
            ItemType.Machinepistol,
            ItemType.Smg,
            ItemType.Assaultsmg,
            ItemType.Combatpdw,
            ItemType.Mg,
            ItemType.Combatmg,
            ItemType.Gusenberg,
            ItemType.Minismg,
            ItemType.Smg_mk2,
            ItemType.Combatmg_mk2,

            ItemType.Assaultrifle,
            ItemType.Carbinerifle,
            ItemType.Advancedrifle,
            ItemType.Specialcarbine,
            ItemType.Bullpuprifle,
            ItemType.Compactrifle,
            ItemType.Militaryrifle,
            ItemType.Assaultrifle_mk2,
            ItemType.Carbinerifle_mk2,
            ItemType.Specialcarbine_mk2,
            ItemType.Bullpuprifle_mk2,

            ItemType.Sniperrifle,
            ItemType.Heavysniper,
            ItemType.Marksmanrifle,
            ItemType.Heavysniper_mk2,
            ItemType.Marksmanrifle_mk2,

            ItemType.Pumpshotgun,
            ItemType.Sawnoffshotgun,
            ItemType.Bullpupshotgun,
            ItemType.Assaultshotgun,
            ItemType.Musket,
            ItemType.Heavyshotgun,
            ItemType.Dbshotgun,
            ItemType.Autoshotgun,
            ItemType.Combatshotgun,
            ItemType.Pumpshotgun_mk2,

            ItemType.Rpg,
        };
        public static List<ItemType> MeleeWeaponsItems = new List<ItemType>()
        {
            ItemType.Knife,
            ItemType.Nightstick,
            ItemType.Hammer,
            ItemType.Bat,
            ItemType.Crowbar,
            ItemType.Golfclub,
            ItemType.Bottle,
            ItemType.Dagger,
            ItemType.Hatchet,
            ItemType.Knuckle,
            ItemType.Machete,
            ItemType.Flashlight,
            ItemType.Switchblade,
            ItemType.Poolcue,
            ItemType.Wrench,
            ItemType.Battleaxe,
            ItemType.Stungun,
            ItemType.Rod,
            ItemType.RodMK2,
            ItemType.RodUpgrade,
            ItemType.DigScanner,
            ItemType.DigScanner_mk2,
            ItemType.DigScanner_mk3,
            ItemType.DigShovel,
            ItemType.DigShovel_mk2,
            ItemType.DigShovel_mk3
        };
        public static List<ItemType> AmmoItems = new List<ItemType>()
        {
            ItemType.PistolAmmo,
            ItemType.RiflesAmmo,
            ItemType.ShotgunsAmmo,
            ItemType.SMGAmmo,
            ItemType.SniperAmmo
        };

        public static List<ItemType> IllegalItems = new List<ItemType>(AmmoItems.Concat(WeaponsItems))
        {
            ItemType.Lockpick,
            ItemType.ArmyLockpick,
            ItemType.CocaLeaves,
            ItemType.Cocaine,
            ItemType.Weed,
            ItemType.Drugs,
            ItemType.DrugBookMark
        };

        public static List<ItemType> AlcoItems = new List<ItemType>()
        {
            ItemType.LcnDrink1,
            ItemType.LcnDrink2,
            ItemType.LcnDrink3,
            ItemType.RusDrink1,
            ItemType.RusDrink2,
            ItemType.RusDrink3,
            ItemType.YakDrink1,
            ItemType.YakDrink2,
            ItemType.YakDrink3,
            ItemType.ArmDrink1,
            ItemType.ArmDrink2,
            ItemType.ArmDrink3,
        };
        public static List<ItemType> FishItems = new List<ItemType>()
        {
            ItemType.Kyndja,
            ItemType.Sig,
            ItemType.Omyl,
            ItemType.Nerka,
            ItemType.Forel,
            ItemType.Ship,
            ItemType.Lopatonos,
            ItemType.Osetr,
            ItemType.Semga,
            ItemType.Servyga,
            ItemType.Beluga,
            ItemType.Taimen,
            ItemType.Sterlyad,
            ItemType.Ydilshik,
            ItemType.Hailiod,
            ItemType.Keta,
            ItemType.Gorbysha
        };

        #region ItemType.Present
        public static readonly List<Tuple<int, int>> PresentsTypes = new List<Tuple<int, int>>()
        {
            new Tuple<int, int>(0, 5),
            new Tuple<int, int>(1, 4),
            new Tuple<int, int>(2, 3),
            new Tuple<int, int>(5, 0),
            new Tuple<int, int>(4, 1),
            new Tuple<int, int>(3, 2),
        };
        public static readonly List<int> TypesCounts = new List<int>()
        {
            10, 25, 50, 1000, 5000, 10000
        };
        #endregion

        // UUID, Items by index
        public static Dictionary<int, List<nItem>> Items = new Dictionary<int, List<nItem>>();
        private static nLog Log = new nLog("nInventory");
        private static Timer SaveTimer;

        public static int FindCount(Player player, ItemType itemType)
        {
            int count = 0;
            foreach (var item in nInventory.Items[Main.Players[player].UUID])
            {
                if (item.Type == itemType)
                    count += item.Count;
            }
            return count;
        }

        #region Constructor
        [ServerEvent(Event.ResourceStart)]
        public void onResourceStart()
        {
            try
            {
                Log.Write("Loading player items...", nLog.Type.Info);

                // // //
                var result = MySQL.QueryRead($"SELECT * FROM `inventory`");
                if (result == null || result.Rows.Count == 0)
                {
                    Log.Write("DB return null result", nLog.Type.Warn);
                    return;
                }
                foreach (DataRow Row in result.Rows)
                {
                    int UUID = Convert.ToInt32(Row["uuid"]);
                    string json = Convert.ToString(Row["items"]);
                    string jsont = Convert.ToString(Row["slots"]);
                    List<nItem> items = JsonConvert.DeserializeObject<List<nItem>>(json);
                    List<bool> slotisEmpty = JsonConvert.DeserializeObject<List<bool>>(jsont);
                    ItemsSlots.Add(UUID, slotisEmpty);
                    Items.Add(UUID, items);
                }
                SaveTimer = new Timer(new TimerCallback(SaveAll), null, 0, 1800000);
                Log.Write("Items loaded.", nLog.Type.Success);
            }
            catch (Exception e)
            {
                Log.Write("EXCEPTION AT \"INVENTORY_CONSTRUCT\":\n" + e.ToString(), nLog.Type.Error);
            }
        }
        #endregion

        #region Add/Remove item

        public static void CalcWeightBag(Player player, List<nItem> Items, int maxWeight)
        {
            if (player == null) return;
            if (!Main.Players.ContainsKey(player)) return;

            var weight = 0.0f;
            foreach (var item in Items)
            {
                item.Weight = ItemsWeight[item.Type] * item.Count;
                weight += item.Weight;
            }

            if (weight > maxWeight)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Рюкзак очень тяжелый", 3000);
                Trigger.ClientEvent(player, "freezeplayer", true);
            }
            else
            {
                Trigger.ClientEvent(player, "freezeplayer", false);
            }
        }

        public static void CalcWeight(Player player)
        {
            var weight = 0.0f;
            foreach (var item in nInventory.Items[Main.Players[player].UUID])
            {
                if (item.character_slot_id != 0 && item.IsActive) continue;

                item.Weight = ItemsWeight[item.Type] * item.Count;
                weight += item.Weight;
            }

            Main.Players[player].currentWeight = weight;
            //Log.Debug("Inventory.cs - currentweight " + Main.Players[player].currentWeight + " maxweight " + Main.Players[player].MaxWeight);

            if (Main.Players[player].currentWeight > Main.Players[player].MaxWeight)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Инвентарь переполнен", 3000);
                Trigger.ClientEvent(player, "freezeplayer", true);
            }
            else
            {
                Trigger.ClientEvent(player, "freezeplayer", false);
            }
        }

        public static int CheckCalcWeight(Player player, nItem tryItem)
        {
            var weight = 0.0f;
            foreach (var item in nInventory.Items[Main.Players[player].UUID])
            {
                if (item.character_slot_id != 0 && item.IsActive) continue;
                item.Weight = ItemsWeight[item.Type] * item.Count;
                weight += item.Weight;
            }

            //Log.Debug("[CheckCalcWeight] Weight: "+weight);
            weight += tryItem.Weight;
            //Log.Debug("[CheckCalcWeight] after Weight: "+weight);

            if (weight > Main.Players[player].MaxWeight) return -2;
            return 0;
        }

        public static bool tryAddCheckFreeSlot(Player player, nItem item)
        {
            int UUID = Main.Players[player].UUID;
            int slot = CheckAdd(Items[UUID], item, ItemsSlots[UUID]);

            if (slot == -1) return false;
            return true;
        }

        public static void Add(Player player, nItem item)
        {
            if(player == null)
            {
                return;
            }

            try
            {
                if (!Main.Players.ContainsKey(player))
                    return;
                int UUID = Main.Players[player].UUID;
                int slot = CheckAdd(Items[UUID], item, ItemsSlots[UUID]);
                //Log.Debug("playerSlots: "+ JsonConvert.SerializeObject(ItemsSlots[UUID]), nLog.Type.Warn);
                if (slot == -1)
                {
                    Core.Items.onDrop(player, ItemsSlots[UUID], item, item.Data);
                    Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, $"Недостаточно места в инвентаре, предмет упал на землю", 3000);
                    return;
                }
                if (ClothesItems.Contains(item.Type) || WeaponsItems.Contains(item.Type) || item.Type == ItemType.CarKey || item.Type == ItemType.KeyRing)
                {
                    item.slot_id = slot;
                    Items[UUID].Add(item);
                    FillSlot(ItemsSlots[UUID], item, 5);
                }
                else
                {
                    int index = Items[UUID].FindIndex(x => x.Type == item.Type && (x.Count + item.Count <= ItemsStacks[x.Type]));
                    if (index != -1)
                    {
                        //Log.Debug("add to stack: inventory count: "+ Items[UUID][index].Count+" item count: "+ item.Count);
                        Items[UUID][index].Count = Items[UUID][index].Count + item.Count;
                        //Log.Debug($"Added existing item! {UUID.ToString()}:{index.ToString()}");
                    }
                    else
                    {
                        int count = item.Count;
                        var items = Items[UUID];
                        for (int i = 0; i < items.Count; i++)
                        {
                            if (i >= items.Count) break;
                            if (items[i].Type == item.Type && items[i].Count < nInventory.ItemsStacks[item.Type])
                            {
                                int temp = nInventory.ItemsStacks[item.Type] - items[i].Count;
                                if (count < temp) temp = count;
                                items[i].Count += temp;
                                count -= temp;
                                //Log.Debug("[FOUND] item to stack and ADD new item type: "+ item.Type + " item ID: " + item.ID);
                            }
                        }

                        //Log.Debug("[WHILE] Count: "+count);

                        while(count > 0)
                        {
                            if (count >= nInventory.ItemsStacks[item.Type])
                            {
                                var newStackItem = new nItem(item.Type, nInventory.ItemsStacks[item.Type], item.Data);
                                slot = CheckAdd(Items[UUID], newStackItem, ItemsSlots[UUID]);
                                //Log.Debug("[WHILE] slot: "+slot, nLog.Type.Error);

                                newStackItem.slot_id = slot;

                                count -= nInventory.ItemsStacks[item.Type];
                                if (slot == -1)
                                {
                                    Log.Debug($"/opt/ [{player.Name}] 1 Ошибка, тут херня со слотами, он почему то занят стал!", nLog.Type.Error);
                                    return;
                                }

                                items.Add(newStackItem);
                                FillSlot(ItemsSlots[UUID], newStackItem, 5);
                                //Log.Debug("[WHILE] add new STACK type: "+ item.Type + " item ID: " + item.ID);
                            }
                            else
                            {
                                var newStackItem = new nItem(item.Type, count, item.Data);
                                slot = CheckAdd(Items[UUID], newStackItem, ItemsSlots[UUID]);
                                //Log.Debug("[WHILE] slot: "+slot, nLog.Type.Error);

                                newStackItem.slot_id = slot;

                                count = 0;
                                if (slot == -1)
                                {
                                    Log.Debug($"/opt/ [{player.Name}] 2 Ошибка, тут херня со слотами, он почему то занят стал!", nLog.Type.Error);
                                    return;
                                }

                                items.Add(newStackItem);
                                FillSlot(ItemsSlots[UUID], newStackItem, 5);
                                //Log.Debug("[WHILE] add new not full STACK TAIL type: "+ item.Type + " item ID: " + item.ID);
                            }
                        }
                    }
                }

                GUI.Dashboard.PsendItems(player, Items[UUID], 2);
            }
            catch (Exception e)
            {
                Log.Write("EXCEPTION AT \"INVENTORY_ADD\":\n" + e.ToString(), nLog.Type.Error);
            }
        }

        public static int TryAdd(Player player, nItem item)
        {
            try
            {
                int UUID = Main.Players[player].UUID;
                //int index = FindIndex(UUID, item.Type);
                //int tail = 0;
                //var isLimitWeight = CheckCalcWeight(player, item);
                //if (isLimitWeight == -2) return -2; // -2

                return CheckAddStacks(Items[UUID], item, ItemsSlots[UUID]); // -1
            }
            catch (Exception e)
            {
                Log.Write("EXCEPTION AT \"INVENTORY_ADD\":\n" + e.ToString(), nLog.Type.Error);
                return 0;
            }
        }

        public static void Remove(Player player, ItemType type, int amount)
        {
            try
            {
                int UUID = Main.Players[player].UUID;
                var index = FindIndex(UUID, type);

                if (index != -1)
                {
                    int temp = Items[UUID][index].Count - amount;
                    if (temp > 0)
                    {
                        Items[UUID][index].Count = temp;
                    }
                    else
                    {
                        amount -= Items[UUID][index].Count;
                        //Log.Debug("CLEAR!? Remove");
                        ClearSlot(ItemsSlots[UUID], Items[UUID][index], 5);
                        Items[UUID].RemoveAt(index);

                        while (amount != 0)
                        {
                            index = Items[UUID].FindIndex(i => i.Type == type);

                            if (index == -1) break;

                            temp = Items[UUID][index].Count - amount;

                            if (temp > 0)
                            {
                                Items[UUID][index].Count = temp;

                                amount = 0;
                            }
                            else
                            {
                                amount -= Items[UUID][index].Count;

                                //Log.Debug("CLEAR!? Remove2 else");
                                ClearSlot(ItemsSlots[UUID], Items[UUID][index], 5);
                                Items[UUID].RemoveAt(index);
                            }
                        }
                    }
                }

                /*if (Index != -1)
                {
                    int temp = Items[UUID][Index].Count - count;
                    if (temp > 0)
                    {
                        if (FishOcean.Contains(type) || FishRiver.Contains(type) || FishPeers.Contains(type))
                        {
                            var data = Items[UUID][Index].Data;
                            if (data != null)
                            {
                                Items[UUID][Index].Data = (float)Items[UUID][Index].Data - ((float)data / Items[UUID][Index].Count * count);
                            }
                        }
                        Items[UUID][Index].Count = temp;
                    }
                    else
                    {
                        ClearSlot(ItemsSlots[UUID], Items[UUID][Index], 5);
                        Items[UUID].RemoveAt(Index);
                    }
                }*/
                GUI.Dashboard.PsendItems(player, Items[UUID], 2);
                Log.Debug($"Item removed. {player.Name} ({UUID.ToString()}):{index.ToString()}");
                GameLog.Items($"player({Main.Players[player].UUID})", "removed", Convert.ToInt32(type), amount, $"{nInventory.ItemsNames[(int)type]}");
                return;
            }
            catch (Exception e)
            {
                Log.Write("EXCEPTION AT \"INVENTORY_REMOVE\":\n" + e.ToString(), nLog.Type.Error);
            }

        }
        public static void Remove(Player player, nItem item)
        {
            try
            {
                int UUID = Main.Players[player].UUID;

                if (ClothesItems.Contains(item.Type) || WeaponsItems.Contains(item.Type) || MeleeWeaponsItems.Contains(item.Type) || item.Type == ItemType.BagWithDrill
                    || item.Type == ItemType.BagWithMoney || item.Type == ItemType.CarKey || item.Type == ItemType.KeyRing || item.Type == ItemType.NumberPlate)
                {
                    //Log.Debug("CLEAR!? Remove3");
                    ClearSlot(ItemsSlots[UUID], item, 5);
                    Items[UUID].Remove(item);

                    //Log.Debug($"Item removed. {UUID.ToString()}:TYPE {(int)item.Type}");
                    GameLog.Items($"player({Main.Players[player].UUID})", "removed", Convert.ToInt32(item.Type), item.Count, $"{item.Data} - {nInventory.ItemsNames[(int)item.Type]}");
                }
                else
                {
                    int Index = FindIndex(UUID, item.Type);
                    if (Index != -1)
                    {

                        int temp = Items[UUID][Index].Count - item.Count;
                        if (temp > 0)
                        {
                          /*  if (FishOcean.Contains(item.Type) || FishRiver.Contains(item.Type) || FishPeers.Contains(item.Type))
                            {
                                var data = Items[UUID][Index].Data;
                                if (data != null)
                                {
                                    Items[UUID][Index].Data = (float)Items[UUID][Index].Data - ((float)data / Items[UUID][Index].Count * item.Count);
                                }
                            }*/
                            Items[UUID][Index].Count = temp;
                            GameLog.Items($"player({Main.Players[player].UUID})", "removed count", Convert.ToInt32(item.Type), item.Count, $"{item.Data} - {nInventory.ItemsNames[(int)item.Type]} - {temp}");
                        }
                        else
                        {
                            //Log.Debug("CLEAR!? Remove4");
                            ClearSlot(ItemsSlots[UUID], Items[UUID][Index], 5);
                            Items[UUID].RemoveAt(Index);
                            GameLog.Items($"player({Main.Players[player].UUID})", "removed", Convert.ToInt32(item.Type), item.Count, $"{item.Data} - {nInventory.ItemsNames[(int)item.Type]}");
                        }
                    }
                    Log.Debug($"Item removed. {UUID.ToString()}:{Index.ToString()}");
                    GameLog.Items($"player({Main.Players[player].UUID})", "removed", Convert.ToInt32(item.Type), item.Count, $"{item.Data} - {nInventory.ItemsNames[(int)item.Type]}");
                }
                GUI.Dashboard.PsendItems(player, Items[UUID], 2);
                return;
            }
            catch (Exception e)
            {
                Log.Write("EXCEPTION AT \"INVENTORY_REMOVE\":\n" + e.StackTrace, nLog.Type.Error);
            }

        }
        #endregion

        #region Save items to db
        public static void SaveAll(object state = null)
        {
            try
            {
                Log.Write("Saving items...", nLog.Type.Info);
                if (Items.Count == 0) return;
                Dictionary<int, List<nItem>> cItems = new Dictionary<int, List<nItem>>(Items);
                Dictionary<int, List<bool>> sItems = new Dictionary<int, List<bool>>(ItemsSlots);
                foreach (KeyValuePair<int, List<nItem>> kvp in cItems)
                {
                    int UUID = kvp.Key;
                    string json = JsonConvert.SerializeObject(kvp.Value);
                    //MySQL.Query($"UPDATE `inventory` SET items='{json}' WHERE uuid={UUID}");

                    MySqlCommand cmd = new MySqlCommand();
                    cmd.CommandText = "UPDATE `inventory` SET `items`=@items WHERE `uuid`=@uuid";

                    cmd.Parameters.AddWithValue("@items", json);
                    cmd.Parameters.AddWithValue("@uuid", UUID);
                    MySQL.Query(cmd);
                }
                foreach (KeyValuePair<int, List<bool>> kvp in sItems)
                {
                    int UUID = kvp.Key;
                    string json = JsonConvert.SerializeObject(kvp.Value);
                    //MySQL.Query($"UPDATE `inventory` SET slots='{json}' WHERE uuid={UUID}");

                    MySqlCommand cmd2 = new MySqlCommand();
                    cmd2.CommandText = "UPDATE `inventory` SET `slots`=@slots WHERE `uuid`=@uuid";

                    cmd2.Parameters.AddWithValue("@slots", json);
                    cmd2.Parameters.AddWithValue("@uuid", UUID);
                    MySQL.Query(cmd2);
                }
                Log.Write("Items has been saved to DB.", nLog.Type.Success);
            }
            catch (Exception e)
            {
                Log.Write("EXCEPTION AT \"INVENTORY_SAVEALL\":\n" + e.ToString(), nLog.Type.Error);
            }
        }
        public static void Save(int UUID)
        {
            try
            {
                if (!Items.ContainsKey(UUID)) return;
                Log.Write($"Saving items for {UUID}", nLog.Type.Info);
                string json = JsonConvert.SerializeObject(Items[UUID]);
                string jsons = JsonConvert.SerializeObject(ItemsSlots[UUID]);

                //MySQL.Query($"UPDATE `inventory` SET items='{json}',slots='{jsons}' WHERE uuid={UUID}");

                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = "UPDATE `inventory` SET `items`=@items, `slots`=@slots WHERE `uuid`=@uuid";

                cmd.Parameters.AddWithValue("@items", json);
                cmd.Parameters.AddWithValue("@slots", jsons);
                cmd.Parameters.AddWithValue("@uuid", UUID);
                MySQL.Query(cmd);
                Log.Write("Items has been saved to DB.", nLog.Type.Success);
            }
            catch (Exception e)
            {
                Log.Write("EXCEPTION AT \"INVENTORY_SAVE\":\n" + e.ToString(), nLog.Type.Error);
            }
        }
        #endregion

        #region SPECIAL
        public static nItem Find(int UUID, ItemType type)
        {
            List<nItem> items = Items[UUID];
            nItem result = items.Find(i => i.Type == type);
            return result;
        }
        public static List<nItem> FindAll(int UUID, ItemType itemType)
        {
            List<nItem> items = Items[UUID];
            return items.FindAll(f => f.Type == itemType);
        }
        public static int FindIndex(int UUID, ItemType type)
        {
            List<nItem> items = Items[UUID];
            int result = items.FindIndex(i => i.Type == type);
            return result;
        }

        public static bool isFull(int UUID)
        {
            if (Items[UUID].Count >= 20) return true;
            else return false;
        }

        public static void Check(int uuid)
        { //if items dict does not contains account uuid, then add him
            if (!Items.ContainsKey(uuid))
            {
                List<bool> tempslots = new List<bool>();
                for (int i = 0; i < 20; i++)
                {
                    tempslots.Add(true);
                }
                Items.Add(uuid, new List<nItem>());
                ItemsSlots.Add(uuid, tempslots);
                //MySQL.Query($"INSERT INTO `inventory` (`uuid`,`items`,`slots`) VALUES ({uuid},'{JsonConvert.SerializeObject(new List<nItem>())}','{JsonConvert.SerializeObject(tempslots)}')");

                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = "INSERT INTO `inventory` SET `uuid`=@uuid, `items`=@items, `slots`=@slots";

                cmd.Parameters.AddWithValue("@uuid", uuid);
                cmd.Parameters.AddWithValue("@items", JsonConvert.SerializeObject(new List<nItem>()));
                cmd.Parameters.AddWithValue("@slots", JsonConvert.SerializeObject(tempslots));
                MySQL.Query(cmd);

                Log.Debug("Player added");
            }
        }
        public static bool CheckFreeSlot(nItem item, int slot, List<bool> slots, int rowCells)
        {
            try
            {
                var w = ItemSizeW[item.Type];
                var h = ItemSizeH[item.Type];

                //Проверяем объект по длине полей
                var fieldNumber = ((slot % rowCells) > 0) ? (slot % rowCells) : rowCells;
                if (fieldNumber + (w - 1) > rowCells)
                {
                    return false;
                }

                //Проверяем обект по высоте полей
                if (slot + ((h - 1) * rowCells) > slots.Count)
                {
                    return false;
                }

                for (int i = 0; i < h; i++)
                {
                    for (int j = 0; j < w; j++)
                    {
                        int fillId = slot + j + (rowCells * i);
                        //Log.Debug($"[CheckFreeSlot] fillId: {fillId} slot free? slots[fillId - 1]: {slots[fillId - 1]}");
                        //Log.Debug("slots: "+JsonConvert.SerializeObject(slots));
                        if (!slots[fillId - 1])
                        {
                            return false;
                        }
                    }
                }

                //Log.Debug("[CheckFreeSlot] slot: "+ slot+" isFree");

                return true;
            }
            catch (Exception e)
            {
                Log.Write(e.ToString(), nLog.Type.Error);
                return false;
            }
        }
        public static int FindFreeSlot(nItem item, List<bool> slots, int rowCells)
        {
            //Log.Debug("[FindFreeSlot]");
            for (int i = 1; i <= slots.Count; i++)
            {
                if (CheckFreeSlot(item, i, slots, rowCells))
                {
                    return i;
                }
            }
            return -1;

        }
        public static int CheckAdd(List<nItem> items, nItem item, List<bool> slots, int rowCells = 5, bool clear = false)
        {
            nItem oldItem = items.Find(x => x.Type == item.Type && (x.Count + item.Count <= ItemsStacks[x.Type]));
            //Log.Debug("[CheckAdd] oldItem found? "+oldItem);
            if (oldItem != null && !clear)
            {
                return oldItem.slot_id;
            }
            else
            {
                return FindFreeSlot(item, slots, rowCells);
            }
        }

        public static int CheckAddStacks(List<nItem> items, nItem item, List<bool> slots, int rowCells = 5)
        {
            //var test = Convert.ToDecimal(item.Count) / Convert.ToDecimal(ItemsStacks[item.Type]);
            var countStacks = Math.Ceiling(Convert.ToDecimal(item.Count) / Convert.ToDecimal(ItemsStacks[item.Type])); // 11 / 10 = 1.1 --- 1
            //Log.Debug("countStacks: "+countStacks+" " + Convert.ToDecimal(item.Count) + " / " + Convert.ToDecimal(ItemsStacks[item.Type])+" = "+ test);


            nItem oldItem = items.Find(x => x.Type == item.Type && (x.Count + item.Count <= ItemsStacks[x.Type]));
            if (oldItem != null)
            {
                countStacks--;
            }

            for (int i = 1; i <= slots.Count; i++)
            {
                if (countStacks == 0) break;
                if (CheckFreeSlot(item, i, slots, rowCells))
                {
                    countStacks--;
                }

                //Log.Debug("Перебираем слоты осталось: "+countStacks);
            }

            if (countStacks != 0) return -1;
            else return 0;
        }

        public static void ClearSlot(List<bool> slots, nItem item, int rowCells)
        {
            try
            {
                Log.Debug($"[ClearSlot] item: id: {item.ID} type: {item.Type} slot_id: {item.slot_id}", nLog.Type.Info);
                if (item.slot_id == 0) return;

                var arrFilled = new List<int>();
                if (ItemSizeH[item.Type] > 1 || ItemSizeW[item.Type] > 1)
                {
                    for (int i = 0; i < ItemSizeH[item.Type]; i++)
                    {
                        for (int j = 0; j < ItemSizeW[item.Type]; j++)
                        {
                            var fillId = item.slot_id + j + (rowCells * i);
                            arrFilled.Add(fillId);
                        }
                    }
                }
                else
                    slots[(int)item.slot_id - 1] = true;

                foreach (int fillId in arrFilled)
                    slots[fillId - 1] = true;

                //Log.Debug($"[ClearSlot] slots: {JsonConvert.SerializeObject(slots)}", nLog.Type.Info);
            }
            catch (Exception e)
            {
                Log.Write($"ClearSlot: slots: {JsonConvert.SerializeObject(slots)} \n" + e.StackTrace, nLog.Type.Error);
            }
        }
        public static void FillSlot(List<bool> slots, nItem item, int rowCells)
        {
            try
            {
                var arrFilled = new List<int>();
                if (ItemSizeH[item.Type] > 1 || ItemSizeW[item.Type] > 1)
                {
                    for (int i = 0; i < ItemSizeH[item.Type]; i++)
                    {
                        for (int j = 0; j < ItemSizeW[item.Type]; j++)
                        {
                            var fillId = item.slot_id + j + (rowCells * i);
                            arrFilled.Add(fillId);
                        }
                    }
                }
                else
                    slots[item.slot_id - 1] = false;

                foreach (int fillId in arrFilled)
                    slots[fillId - 1] = false;
            }
            catch (Exception e)
            {
                Log.Write("FillSlot: " + e.StackTrace, nLog.Type.Error);
            }
        }
        public static void UnActiveItem(Player player, ItemType type)
        {
            var items = Items[Main.Players[player].UUID];
            foreach (var i in items)
                if (i.Type == type && i.IsActive)
                {
                    i.IsActive = false;
                    GUI.Dashboard.Update(player, i, items.IndexOf(i));
                }
            Items[Main.Players[player].UUID] = items;
        }
        public static void ClearWithoutClothes(Player player)
        {
            try
            {
                int uuid = Main.Players[player].UUID;
                List<nItem> items = Items[uuid];
                List<nItem> upd = new List<nItem>();
                foreach (nItem item in items)
                    if (ClothesItems.Contains(item.Type) || item.Type == ItemType.CarKey || item.Type == ItemType.KeyRing) upd.Add(item);

                Items[uuid] = upd;
                GUI.Dashboard.sendItems(player);
            }
            catch (Exception e)
            {
                Log.Write(e.ToString(), nLog.Type.Error);
            }
        }
        public static void ClearAllClothes(Player player)
        {
            try
            {
                //List<nItem> items = new List<nItem>();
                int index = 0;
                foreach (var item in nInventory.Items[Main.Players[player].UUID])
                {
                    if (item.IsActive)
                    {
                        nInventory.Items[Main.Players[player].UUID][index].IsActive = false;
                        Dashboard.Update(player, item, nInventory.Items[Main.Players[player].UUID].IndexOf(item));
                    }
                    index++;
                }

            }
            catch (Exception e)
            {
                Log.Write(e.ToString(), nLog.Type.Error);
            }
        }
        #endregion
    }

    class Items : Script
    {
        private static nLog Log = new nLog("Items");

        public static List<int> ItemsDropped = new List<int>();
        public static List<int> InProcessering = new List<int>();
        [ServerEvent(Event.EntityDeleted)]
        public void Event_OnEntityDeleted(Entity entity)
        {
            try
            {
                if (NAPI.Entity.GetEntityType(entity) == EntityType.Object && NAPI.Data.HasEntityData(entity, "DELETETIMER"))
                {
                    Timers.Stop(NAPI.Data.GetEntityData(entity, "DELETETIMER"));
                    ItemsDropped.Remove(NAPI.Data.GetEntityData(entity, "ID"));
                    InProcessering.Remove(NAPI.Data.GetEntityData(entity, "ID"));
                }
            }
            catch (Exception e)
            {
                Log.Write("Event_OnEntityDeleted: " + e.StackTrace, nLog.Type.Error);
            }
        }

        public static void deleteObject(GTANetworkAPI.Object obj)
        {
            NAPI.Task.Run(() =>
            {
                try
                {
                    //Main.StopT(obj.GetData<object>("DELETETIMER"), "timer_33");
                    obj.ResetData("DELETETIMER");
                    ItemsDropped.Remove(obj.GetData<int>("ID"));
                    InProcessering.Remove(obj.GetData<int>("ID"));
                    obj.Delete();
                }
                catch (Exception e)
                {
                    Log.Write("UpdateObject: " + e.StackTrace, nLog.Type.Error);
                }
            }, 0);
        }

        public static void onUse(Player player, nItem item, int index)
        {
            try
            {
                if(player.HasMyData("NEWGUNGAME_INMATCH") && player.GetMyData<bool>("NEWGUNGAME_INMATCH")) return;

                var UUID = Main.Players[player].UUID;
                if (nInventory.ClothesItems.Contains(item.Type) && item.Type != ItemType.BodyArmor && item.Type != ItemType.Mask)
                {
                    var data = (string)item.Data;
                    var clothesGender = Convert.ToBoolean(data.Split('_')[2]);
                    if (clothesGender != Main.Players[player].Gender)
                    {
                        var error_gender = (clothesGender) ? "мужская" : "женская";
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Это {error_gender} одежда", 3000);
                        GUI.Dashboard.Close(player);
                        return;
                    }

                    //TODO CHECK
                    if ((Main.Players[player].OnDuty && Fractions.Manager.FractionTypes[Main.Players[player].Fraction.FractionID] == 2 && Main.Players[player].Fraction.FractionID != 9) || player.GetMyData<bool>("ON_WORK"))
                    {

                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы не можете использовать это сейчас", 3000);
                        GUI.Dashboard.PsendItems(player, nInventory.Items[UUID], 2);
                        GUI.Dashboard.Close(player);
                        return;
                    }
                }

                if (nInventory.WeaponsItems.Contains(item.Type) || nInventory.MeleeWeaponsItems.Contains(item.Type))
                {
                    if (item.IsActive)
                    {
                        Log.Write("Weapon is Active", nLog.Type.Error);
                        if (item.Type == ItemType.Rod || item.Type == ItemType.RodUpgrade || item.Type == ItemType.RodMK2)
                        {
                            BasicSync.AddAttachment(player, "roding", true);
                            player.SetMyData("RodInHand", false);
                            item.IsActive = false;
                        }
                        else if (item.Type == ItemType.DigScanner || item.Type == ItemType.DigScanner_mk2 || item.Type == ItemType.DigScanner_mk3)
                        {
                            player.SetSharedData("DigScannerInHand", -1);
                            BasicSync.AddAttachment(player, "DigScannerActive", true);
                            item.IsActive = false;
                        }
                        else if(item.Type == ItemType.DigShovel || item.Type == ItemType.DigShovel_mk2 || item.Type == ItemType.DigShovel_mk3)
                        {
                            player.SetSharedData("DigShovelInHand", -1);
                            BasicSync.AddAttachment(player, "DigShovelActive", true);
                            item.IsActive = false;
                        }
                        else
                        {
                            Weapons.PlayerTakeOffWeapon(player, item);
                            Log.Write("Weapon is Deactivated item: -> " + item.Type, nLog.Type.Error);

                        }
                        Main.Players[player].currentWeapon = "Unarmed";
                        if (nInventory.WeaponsItems.Contains(item.Type) && !item.IsActive && item.fast_slot_id > 0) GUI.Dashboard.SyncAttachComp(player, item, false);
                        GUI.Dashboard.Update(player, item, index);
                        //Commands.RPChat("me", player, $"убрал {nInventory.ItemsNames[(int)item.Type]}");
                        Trigger.ClientEvent(player, "CLIENT::HUD:weaponShow", false, null);
                    }
                    else
                    {
                        var oldwItem = nInventory.Items[UUID].FirstOrDefault(i => (nInventory.WeaponsItems.Contains(i.Type) || nInventory.MeleeWeaponsItems.Contains(i.Type)) && i.IsActive);
                        if (oldwItem != null)
                        {
                            if (oldwItem.Type == ItemType.Rod || oldwItem.Type == ItemType.RodUpgrade || oldwItem.Type == ItemType.RodMK2)
                            {
                                BasicSync.AddAttachment(player, "roding", true);
                                player.SetMyData("RodInHand", false);
                            }
                            else if (oldwItem.Type == ItemType.DigScanner || oldwItem.Type == ItemType.DigScanner_mk2 || oldwItem.Type == ItemType.DigScanner_mk3)
                            {
                                player.SetSharedData("DigScannerInHand", -1);
                                BasicSync.AddAttachment(player, "DigScannerActive", true);
                            }
                            else if (oldwItem.Type == ItemType.DigShovel || oldwItem.Type == ItemType.DigShovel_mk2 || oldwItem.Type == ItemType.DigShovel_mk3)
                            {
                                player.SetSharedData("DigShovelInHand", -1);
                                BasicSync.AddAttachment(player, "DigShovelActive", true);
                            }
                            else
                            {

                                Weapons.PlayerTakeOffWeapon(player, oldwItem);
                                Log.Write("Weapon is Deactivated 2", nLog.Type.Error);
                            }

                            oldwItem.IsActive = false;
                            if (nInventory.WeaponsItems.Contains(oldwItem.Type) && !oldwItem.IsActive && oldwItem.fast_slot_id > 0) GUI.Dashboard.SyncAttachComp(player, oldwItem, false);
                            GUI.Dashboard.Update(player, oldwItem, nInventory.Items[UUID].IndexOf(oldwItem));
                            //Commands.RPChat("me", player, $"удалил {nInventory.ItemsNames[(int)oldwItem.Type]}");

                            Trigger.ClientEvent(player, "CLIENT::HUD:weaponShow", false, null);
                        }


                        var wHash = Weapons.GetWeaponHash(item.Type.ToString());
                        if (Weapons.WeaponsAmmoTypes.ContainsKey(item.Type))
                        {
                            var ammo = item.WData.AmmmoInClip;
                            Log.Debug("SetAmmoinActive " + ammo);
                            player.SetSharedData("currentAmmo", (int)ammo);

                            var countAmmo = nInventory.FindCount(player, Weapons.WeaponsAmmoTypes[item.Type]);
                            Trigger.ClientEvent(player, "CLIENT::HUD:weaponUpdate", (int)ammo, countAmmo);

                            NAPI.Player.GivePlayerWeapon(player, (GTANetworkAPI.WeaponHash)wHash, (int)ammo);

                            List<string> tempComponents = new List<string>();
                            foreach (var ComponentType in item.WData.Components.Values)
                            {
                                tempComponents.Add(ComponentType.Сtype);
                            }

                            player.SetSharedData("currentWeaponComponents", (uint)wHash + "." + string.Join("|", tempComponents.ToArray()));

                            var pos = player.Position;
                            var pId = player.Id;
                            foreach (var comp in tempComponents)
                            {
                                if (comp != null)
                                    Trigger.ClientEventInRange(pos, 500.0f, "updatePlayerWeaponComponent", pId, (GTANetworkAPI.WeaponHash)wHash, comp, false);
                            }
                        }
                        else
                        {
                            if (item.Type == ItemType.Rod || item.Type == ItemType.RodUpgrade || item.Type == ItemType.RodMK2)
                            {
                                //57005 rhand lhand18905  PH_R_Hand 28422  PH_L_Hand 60309 // oldvalue 18905
                               // BasicSync.AttachObjectToPlayer(player, nInventory.ItemModels[item.Type], 18905, nInventory.ItemsPosOffset[item.Type], nInventory.ItemsRotOffset[item.Type]);
                                player.SetMyData("RodInHand", true);
                                BasicSync.AddAttachment(player, "roding", false);
                                Log.Write("1. Model -> "+ nInventory.ItemModels[item.Type] + " hand -> 18905" + " PosOffset -> "+ nInventory.ItemsPosOffset[item.Type]+ " RotOffset -> " + nInventory.ItemsRotOffset[item.Type], nLog.Type.Error);
                            }
                            else if (item.Type == ItemType.DigScanner || item.Type == ItemType.DigScanner_mk2 || item.Type == ItemType.DigScanner_mk3)
                            {
                                player.SetSharedData("DigScannerInHand", MetalDetectorSystem.GetItemLevelByItemType(item.Type));
                                BasicSync.AddAttachment(player, "DigScannerActive", false);
                                item.IsActive = false;
                            }
                            else if (item.Type == ItemType.DigShovel || item.Type == ItemType.DigShovel_mk2 || item.Type == ItemType.DigShovel_mk3)
                            {
                                player.SetSharedData("DigShovelInHand", MetalDetectorSystem.GetItemLevelByItemType(item.Type));
                                BasicSync.AddAttachment(player, "DigShovelActive", false);
                                item.IsActive = false;
                            }
                            else
                            {
                                if (item.Type == ItemType.Stungun) item.WData.AmmmoInClip = 1;
                                player.SetSharedData("currentAmmo", 1);

                                Trigger.ClientEvent(player, "CLIENT::HUD:weaponUpdate", 1, 1);

                                NAPI.Player.GivePlayerWeapon(player, (GTANetworkAPI.WeaponHash)wHash, 1);
                                //  Trigger.ClientEvent(player, "wgive", (int)wHash, 1, false, true);
                                Log.Write("Weapon is GivePlayerWeapon hash: -> "+ (GTANetworkAPI.WeaponHash)wHash, nLog.Type.Error);
                            }
                        }
                        Main.Players[player].currentWeapon = wHash.ToString();
                        Log.Debug("SetcurrentWeapon " + wHash.ToString(), nLog.Type.Warn);
                        //Commands.RPChat("me", player, $"достал {nInventory.ItemsNames[(int)item.Type]}");
                        item.IsActive = true;
                        if (nInventory.WeaponsItems.Contains(item.Type) && item.fast_slot_id > 0) GUI.Dashboard.SyncAttachComp(player, item, true);
                        //player.SetMyData("LastActiveWeap", item.Type);
                        player.SetMyData("LastActiveWeap", item);

                        Trigger.ClientEvent(player, "CLIENT::HUD:weaponShow", true, item.Type);
                        GUI.Dashboard.Update(player, item, index);
                        GUI.Dashboard.Close(player);
                    }
                    return;
                }

                var gender = Main.Players[player].Gender;

                if (nInventory.ClothesItemSlots.ContainsKey(item.Type) && nInventory.ClothesItemSlots[item.Type] == 12)
                {
                    if (item.Items == null || item.Items.Count == 0)
                    {
                        var countSlots = 20;
                        if (item.Bag != null) countSlots = item.Bag.maxSlots;

                        item.Items = new List<nItem>();
                        List<bool> tempslots = new List<bool>();
                        for (int i = 0; i < countSlots; i++)
                        {
                            tempslots.Add(true);
                        }
                        item.Slots = tempslots;
                    }

                    if (item.IsActive)
                    {
                        Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Bag = new ComponentItem(Customization.EmtptySlots[gender][10], 0);

                        nInventory.Items[UUID][index].IsActive = false;
                        GUI.Dashboard.Update(player, item, index);
                        //Console.WriteLine("watafuck1");
                    }
                    else
                    {
                        var itemData = (string)item.Data;
                        var variation = Convert.ToInt32(itemData.Split('_')[0]);
                        var texture = Convert.ToInt32(itemData.Split('_')[1]);
                        Log.Debug($"{texture} {variation}");
                        Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Bag = new ComponentItem(variation, texture);

                        // nInventory.UnActiveItem(player, item.Type);
                        nInventory.Items[UUID][index].IsActive = true;
                        GUI.Dashboard.Update(player, item, index);
                        //Console.WriteLine("watafuck2");
                    }
                    player.SetClothes(5, Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Bag.Variation, Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Bag.Texture);
                    return;
                }

                if (nInventory.AmmoItems.Contains(item.Type)) return;

                if (nInventory.AlcoItems.Contains(item.Type))
                {
                    int stage = Convert.ToInt32(item.Type.ToString().Split("Drink")[1]);
                    int curStage = player.GetMyData<int>("RESIST_STAGE");

                    if (player.HasMyData("RESIST_BAN"))
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы пьяны до такой степени, что не можете открыть бутылку", 3000);
                        return;
                    }

                    var stageTimes = new List<int>() { 0, 300, 420, 600 };

                    if (curStage == 0 || curStage == stage)
                    {
                        player.SetMyData("RESIST_STAGE", stage);
                        player.SetMyData("RESIST_TIME", player.GetMyData<int>("RESIST_TIME") + stageTimes[stage]);
                    }
                    else if (curStage < stage)
                    {
                        player.SetMyData("RESIST_STAGE", stage);
                    }
                    else if (curStage > stage)
                    {
                        player.SetMyData("RESIST_TIME", player.GetMyData<int>("RESIST_TIME") + stageTimes[stage]);
                    }

                    if (player.GetMyData<int>("RESIST_TIME") >= 1500)
                        player.SetMyData("RESIST_BAN", true);

                    Trigger.ClientEvent(player, "setResistStage", player.GetMyData<int>("RESIST_STAGE"));
                    //BasicSync.AttachObjectToPlayer(player, nInventory.ItemModels[item.Type], 57005, Fractions.AlcoFabrication.AlcoPosOffset[item.Type], Fractions.AlcoFabrication.AlcoRotOffset[item.Type]);

                    Main.OnAntiAnim(player);
                    player.PlayAnimation("amb@world_human_drinking@beer@male@idle_a", "idle_c", 49);
                    NAPI.Task.Run(() => {
                        try
                        {
                            if (player != null)
                            {
                                if (!player.IsInVehicle) player.StopAnimation();
                                else player.SetMyData("ToResetAnimPhone", true);
                                Main.OffAntiAnim(player);
                                Trigger.ClientEvent(player, "startScreenEffect", "PPFilter", player.GetMyData<int>("RESIST_TIME") * 1000, false);
                                BasicSync.DetachObject(player);
                            }
                        } catch (Exception e){ Log.Write(e.StackTrace, nLog.Type.Error); }
                    }, 5000);

                    /*if (!player.HasMyData("RESIST_TIMER"))
                        player.SetMyData("RESIST_TIMER", Timers.Start(1000, () => Fractions.AlcoFabrication.ResistTimer(player.Name)));*/

                    //Commands.RPChat("me", player, "выпил бутылку " + nInventory.ItemsNames[(int)item.Type]);
                    GameLog.Items($"player({Main.Players[player].UUID})", "use", Convert.ToInt32(item.Type), 1, $"{item.Data}");
                }


                Log.Debug($"item used Type: {item.Type} {JsonConvert.SerializeObject(item)}");
                switch (item.Type)
                {
                    #region Clothes
                    case ItemType.Glasses:
                        {
                            /*if (Main.Players[player].OnDuty || Main.Players[player].Fraction.InClothes)
                            {
                                GUI.Dashboard.Update(player, item, index);
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы должны закончить рабочий день");
                                return;
                            }*/
                            if (item.IsActive)
                            {
                                Customization.CustomPlayerData[Main.Players[player].UUID].Accessory.Glasses.Variation = -1;
                                player.ClearAccessory(1);
                                nInventory.Items[UUID][index].IsActive = false;
                                GUI.Dashboard.Update(player, item, index);
                            }
                            else
                            {
                                var mask = Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Mask.Variation;
                                if (Customization.MaskTypes.ContainsKey(mask) && Customization.MaskTypes[mask].Item3)
                                {
                                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы не можете надеть эти очки с маской", 3000);
                                    return;
                                }
                                var itemData = (string)item.Data;
                                var variation = Convert.ToInt32(itemData.Split('_')[0]);
                                var texture = Convert.ToInt32(itemData.Split('_')[1]);
                                Customization.CustomPlayerData[Main.Players[player].UUID].Accessory.Glasses = new ComponentItem(variation, texture);
                                player.SetAccessories(1, variation, texture);

                                nInventory.UnActiveItem(player, item.Type);
                                nInventory.Items[UUID][index].IsActive = true;
                                GUI.Dashboard.Update(player, item, index);
                            }
                            return;
                        }
                    case ItemType.Hat:
                        {
                            /*if (Main.Players[player].OnDuty || Main.Players[player].Fraction.InClothes)
                            {
                                GUI.Dashboard.Update(player, item, index);
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы должны закончить рабочий день");
                                return;
                            }*/
                            if (item.IsActive)
                            {
                                Customization.CustomPlayerData[Main.Players[player].UUID].Accessory.Hat.Variation = -1;

                                nInventory.Items[UUID][index].IsActive = false;
                                GUI.Dashboard.Update(player, item, index);
                            }
                            else
                            {
                                var mask = Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Mask.Variation;
                                if (Customization.MaskTypes.ContainsKey(mask) && Customization.MaskTypes[mask].Item2)
                                {
                                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы не можете надеть этот головной убор с маской", 3000);
                                    return;
                                }
                                var itemData = (string)item.Data;
                                var variation = Convert.ToInt32(itemData.Split('_')[0]);
                                var texture = Convert.ToInt32(itemData.Split('_')[1]);
                                Customization.CustomPlayerData[Main.Players[player].UUID].Accessory.Hat = new ComponentItem(variation, texture);

                                nInventory.UnActiveItem(player, item.Type);
                                nInventory.Items[UUID][index].IsActive = true;
                                GUI.Dashboard.Update(player, item, index);
                            }
                            Customization.SetHat(player, Customization.CustomPlayerData[Main.Players[player].UUID].Accessory.Hat.Variation, Customization.CustomPlayerData[Main.Players[player].UUID].Accessory.Hat.Texture);
                            return;
                        }
                    case ItemType.Mask:
                        {
                            /*if (Main.Players[player].OnDuty || Main.Players[player].Fraction.InClothes)
                            {
                                GUI.Dashboard.Update(player, item, index);
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы должны закончить рабочий день");
                                return;
                            }*/
                            if (item.IsActive)
                            {
                                Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Mask = new ComponentItem(Customization.EmtptySlots[gender][1], 0);

                                nInventory.Items[UUID][index].IsActive = false;
                                GUI.Dashboard.Update(player, item, index);
                            }
                            else
                            {
                                var itemData = (string)item.Data;
                                var variation = Convert.ToInt32(itemData.Split('_')[0]);
                                var texture = Convert.ToInt32(itemData.Split('_')[1]);
                                Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Mask = new ComponentItem(variation, texture);

                                if (Customization.MaskTypes.ContainsKey(variation))
                                {
                                    if (Customization.MaskTypes[variation].Item1)
                                    {
                                        player.SetClothes(2, 0, 0);
                                    }
                                    if (Customization.MaskTypes[variation].Item2)
                                    {
                                        Customization.CustomPlayerData[Main.Players[player].UUID].Accessory.Hat.Variation = -1;
                                        nInventory.UnActiveItem(player, ItemType.Hat);
                                        Customization.SetHat(player, -1, 0);
                                    }
                                    if (Customization.MaskTypes[variation].Item3)
                                    {
                                        Customization.CustomPlayerData[Main.Players[player].UUID].Accessory.Glasses.Variation = -1;
                                        nInventory.UnActiveItem(player, ItemType.Glasses);
                                        player.ClearAccessory(1);
                                    }
                                }

                                nInventory.UnActiveItem(player, item.Type);
                                nInventory.Items[UUID][index].IsActive = true;
                                GUI.Dashboard.Update(player, item, index);
                            }
                            Customization.SetMask(player, Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Mask.Variation, Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Mask.Texture);
                            return;
                        }
                    case ItemType.Gloves:
                        {
                            /*if (Main.Players[player].OnDuty || Main.Players[player].Fraction.InClothes)
                            {
                                GUI.Dashboard.Update(player, item, index);
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы должны закончить рабочий день");
                                return;
                            }*/
                            if (item.IsActive)
                            {
                                Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Gloves = new ComponentItem(0, 0);
                                Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Torso = new ComponentItem(Customization.CorrectTorso[gender][Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Top.Variation], 0);

                                nInventory.Items[UUID][index].IsActive = false;
                                GUI.Dashboard.Update(player, item, index);
                            }
                            else
                            {
                                var itemData = (string)item.Data;
                                var variation = Convert.ToInt32(itemData.Split('_')[0]);
                                var texture = Convert.ToInt32(itemData.Split('_')[1]);
                                if (!Customization.CorrectGloves[gender][variation].ContainsKey(Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Torso.Variation)) return; // WTF!? m4ybe
                                Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Gloves = new ComponentItem(variation, texture);
                                Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Torso = new ComponentItem(Customization.CorrectGloves[gender][variation][Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Torso.Variation], texture);

                                nInventory.UnActiveItem(player, item.Type);
                                nInventory.Items[UUID][index].IsActive = true;
                                GUI.Dashboard.Update(player, item, index);
                            }
                            player.SetClothes(3, Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Torso.Variation, Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Torso.Texture);
                            return;
                        }
                    case ItemType.Leg:
                        {
                            /*if (Main.Players[player].OnDuty || Main.Players[player].Fraction.InClothes)
                            {
                                GUI.Dashboard.Update(player, item, index);
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы должны закончить рабочий день");
                                return;
                            }*/
                            if (item.IsActive)
                            {
                                Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Leg = new ComponentItem(Customization.EmtptySlots[gender][4], 0);

                                nInventory.Items[UUID][index].IsActive = false;
                                GUI.Dashboard.Update(player, item, index);
                            }
                            else
                            {
                                var itemData = (string)item.Data;
                                var variation = Convert.ToInt32(itemData.Split('_')[0]);
                                var texture = Convert.ToInt32(itemData.Split('_')[1]);
                                Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Leg = new ComponentItem(variation, texture);

                                nInventory.UnActiveItem(player, item.Type);
                                nInventory.Items[UUID][index].IsActive = true;
                                GUI.Dashboard.Update(player, item, index);
                            }
                            player.SetClothes(4, Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Leg.Variation, Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Leg.Texture);
                            return;
                        }
                    case ItemType.Feet:
                        {
                            /*if (Main.Players[player].OnDuty || Main.Players[player].Fraction.InClothes)
                            {
                                GUI.Dashboard.Update(player, item, index);
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы должны закончить рабочий день");
                                return;
                            }*/
                            if (item.IsActive)
                            {
                                Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Feet = new ComponentItem(Customization.EmtptySlots[gender][6], 0);

                                nInventory.Items[UUID][index].IsActive = false;
                                GUI.Dashboard.Update(player, item, index);
                            }
                            else
                            {
                                var itemData = (string)item.Data;
                                var variation = Convert.ToInt32(itemData.Split('_')[0]);
                                var texture = Convert.ToInt32(itemData.Split('_')[1]);
                                Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Feet = new ComponentItem(variation, texture);

                                nInventory.UnActiveItem(player, item.Type);
                                nInventory.Items[UUID][index].IsActive = true;
                                GUI.Dashboard.Update(player, item, index);
                            }
                            player.SetClothes(6, Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Feet.Variation, Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Feet.Texture);
                            return;
                        }
                    case ItemType.Jewelry:
                        {
                            /*if (Main.Players[player].OnDuty || Main.Players[player].Fraction.InClothes)
                            {
                                GUI.Dashboard.Update(player, item, index);
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы должны закончить рабочий день");
                                return;
                            }*/
                            if (item.IsActive)
                            {
                                Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Accessory = new ComponentItem(Customization.EmtptySlots[gender][7], 0);

                                nInventory.Items[UUID][index].IsActive = false;
                                GUI.Dashboard.Update(player, item, index);
                            }
                            else
                            {
                                var itemData = (string)item.Data;
                                var variation = Convert.ToInt32(itemData.Split('_')[0]);
                                var texture = Convert.ToInt32(itemData.Split('_')[1]);
                                Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Accessory = new ComponentItem(variation, texture);

                                nInventory.UnActiveItem(player, item.Type);
                                nInventory.Items[UUID][index].IsActive = true;
                                GUI.Dashboard.Update(player, item, index);
                            }
                            player.SetClothes(7, Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Accessory.Variation, Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Accessory.Texture);
                            return;
                        }
                    case ItemType.Accessories:
                        {
                            /*if (Main.Players[player].OnDuty || Main.Players[player].Fraction.InClothes)
                            {
                                GUI.Dashboard.Update(player, item, index);
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы должны закончить рабочий день");
                                return;
                            }*/
                            var itemData = (string)item.Data;
                            var variation = Convert.ToInt32(itemData.Split('_')[0]);
                            var texture = Convert.ToInt32(itemData.Split('_')[1]);

                            if (item.IsActive)
                            {
                                var watchesSlot = Customization.CustomPlayerData[Main.Players[player].UUID].Accessory.Watches;
                                if (watchesSlot.Variation == variation && watchesSlot.Texture == texture)
                                {
                                    Customization.CustomPlayerData[Main.Players[player].UUID].Accessory.Watches = new ComponentItem(-1, 0);
                                    player.ClearAccessory(6);
                                }
                                else
                                {
                                    Customization.CustomPlayerData[Main.Players[player].UUID].Accessory.Bracelets = new ComponentItem(-1, 0);
                                    player.ClearAccessory(7);
                                }

                                nInventory.Items[UUID][index].IsActive = false;
                                GUI.Dashboard.Update(player, item, index);
                            }
                            else
                            {
                                if (Customization.CustomPlayerData[Main.Players[player].UUID].Accessory.Watches.Variation == -1)
                                {
                                    Customization.CustomPlayerData[Main.Players[player].UUID].Accessory.Watches = new ComponentItem(variation, texture);
                                    player.SetAccessories(6, variation, texture);

                                    nInventory.Items[UUID][index].IsActive = true;
                                    GUI.Dashboard.Update(player, item, index);
                                }
                                else if (Customization.AccessoryRHand[gender].ContainsKey(variation))
                                {
                                    if (Customization.CustomPlayerData[Main.Players[player].UUID].Accessory.Bracelets.Variation == -1)
                                    {
                                        Customization.CustomPlayerData[Main.Players[player].UUID].Accessory.Bracelets = new ComponentItem(Customization.AccessoryRHand[gender][variation], texture);
                                        player.SetAccessories(7, Customization.AccessoryRHand[gender][variation], texture);

                                        nInventory.Items[UUID][index].IsActive = true;
                                        GUI.Dashboard.Update(player, item, index);
                                    }
                                    else
                                    {
                                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Заняты обе руки", 3000);
                                        return;
                                    }
                                }
                                else
                                {
                                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "На Вас уже надеты Часы", 3000);
                                    return;
                                }
                            }
                            return;
                        }
                    case ItemType.Undershit:
                        {
                            /*if (Main.Players[player].OnDuty || Main.Players[player].Fraction.InClothes)
                            {
                                GUI.Dashboard.Update(player, item, index);
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы должны закончить рабочий день");
                                return;
                            }*/
                            var itemData = (string)item.Data;
                            var underwearID = Convert.ToInt32(itemData.Split('_')[0]);
                            var underwear = Customization.Underwears[gender][underwearID];
                            var texture = Convert.ToInt32(itemData.Split('_')[1]);
                            if (item.IsActive)
                            {
                                if (underwear.Top == Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Top.Variation)
                                    Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Top = new ComponentItem(Customization.EmtptySlots[gender][11], 0);
                                Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Undershit = new ComponentItem(Customization.EmtptySlots[gender][8], 0);

                                nInventory.Items[UUID][index].IsActive = false;
                                GUI.Dashboard.Update(player, item, index);
                            }
                            else
                            {
                                if (Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Top.Variation == Customization.EmtptySlots[gender][11])
                                {
                                    if (underwear.Top == -1)
                                    {
                                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Эту одежду можно надеть только вместе с верхней", 3000);
                                        return;
                                    }
                                    Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Top = new ComponentItem(underwear.Top, texture);

                                    nInventory.UnActiveItem(player, item.Type);
                                    nInventory.Items[UUID][index].IsActive = true;
                                    GUI.Dashboard.Update(player, item, index);
                                }
                                else
                                {
                                    var nowTop = Customization.Tops[gender].FirstOrDefault(t => t.Variation == Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Top.Variation);
                                    if (nowTop != null)
                                    {
                                        var topType = nowTop.Type;
                                        if (!underwear.UndershirtIDs.ContainsKey(topType))
                                        {
                                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Эта одежда несовместима с Вашей верхней одеждой", 3000);
                                            return;
                                        }
                                        Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Undershit = new ComponentItem(underwear.UndershirtIDs[topType], texture);

                                        nInventory.UnActiveItem(player, item.Type);
                                        nInventory.Items[UUID][index].IsActive = true;
                                        GUI.Dashboard.Update(player, item, index);
                                    }
                                    else
                                    {
                                        if (underwear.Top == -1)
                                        {
                                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Эту одежду можно надеть только вместе с верхней", 3000);
                                            return;
                                        }
                                        Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Top = new ComponentItem(underwear.Top, texture);

                                        nInventory.UnActiveItem(player, item.Type);
                                        nInventory.Items[UUID][index].IsActive = true;
                                        GUI.Dashboard.Update(player, item, index);
                                    }
                                }
                            }

                            var gloves = Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Gloves.Variation;
                            if (gloves != 0 &&
                                !Customization.CorrectGloves[gender][gloves].ContainsKey(Customization.CorrectTorso[gender][Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Top.Variation]))
                            {
                                nInventory.UnActiveItem(player, ItemType.Gloves);
                                Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Gloves = new ComponentItem(0, 0);
                            }

                            player.SetClothes(8, Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Undershit.Variation, Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Undershit.Texture);
                            player.SetClothes(11, Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Top.Variation, Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Top.Texture);
                            var noneGloves = Customization.CorrectTorso[gender][Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Top.Variation];
                            if (Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Gloves.Variation == 0)
                            {
                                Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Torso = new ComponentItem(noneGloves, 0);
                                player.SetClothes(3, noneGloves, 0);
                            }
                            else
                                player.SetClothes(3, Customization.CorrectGloves[gender][Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Gloves.Variation][noneGloves], Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Gloves.Texture);
                            return;
                        }
                    case ItemType.BodyArmor:
                        {
                            var itemData = (string)item.Data;
                            //Log.Debug("itemData: "+ itemData);
                            var variation = Convert.ToInt32(itemData.Split('_')[0]);
                            var texture = Convert.ToInt32(itemData.Split('_')[1]);
                            var genderItem = Convert.ToBoolean(itemData.Split('_')[2]);
                            var armorCount = Convert.ToInt32(itemData.Split('_')[3]);

                            if (item.IsActive)
                            {
                                item.Data = $"{variation}_{texture}_{Main.Players[player].Gender}_{player.Armor.ToString()}"; ;
                                player.Armor = 0;
                                player.ResetSharedData("HASARMOR");

                                Customization.CustomPlayerData[UUID].Clothes.Bodyarmor = new ComponentItem(Customization.EmtptySlots[genderItem][9], 0);

                                nInventory.Items[UUID][index].IsActive = false;
                                GUI.Dashboard.Update(player, item, index);

                                Log.Debug("ARMOR СНЯЛИ?");
                            }
                            else
                            {
                                var armor = Convert.ToInt32(armorCount);

                                player.Armor = armor;
                                player.SetSharedData("HASARMOR", true);

                                //TODO Добавить разные вариации бронежилета для разных фракций и банд

                                Customization.CustomPlayerData[UUID].Clothes.Bodyarmor = new ComponentItem(variation, texture);

                                nInventory.UnActiveItem(player, item.Type);
                                nInventory.Items[UUID][index].IsActive = true;
                                GUI.Dashboard.Update(player, item, index);
                                Log.Debug("ARMOR ОДЕЛИ?");
                            }

                            player.SetClothes(9, Customization.CustomPlayerData[UUID].Clothes.Bodyarmor.Variation, Customization.CustomPlayerData[UUID].Clothes.Bodyarmor.Texture);
                            Log.Debug("ARMOR RETURN?");
                            return;
                        }
                    case ItemType.Unknown:
                        {
                            /*if (Main.Players[player].OnDuty || Main.Players[player].Fraction.InClothes)
                            {
                                GUI.Dashboard.Update(player, item, index);
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы должны закончить рабочий день");
                                return;
                            }*/
                            if (item.IsActive)
                            {
                                Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Decals = new ComponentItem(Customization.EmtptySlots[gender][10], 0);
                            }
                            else
                            {
                                var itemData = (string)item.Data;
                                var variation = Convert.ToInt32(itemData.Split('_')[0]);
                                var texture = Convert.ToInt32(itemData.Split('_')[1]);
                                Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Decals = new ComponentItem(variation, texture);
                            }
                            player.SetClothes(10, Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Decals.Variation, Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Decals.Texture);
                            return;
                        }
                    case ItemType.Top:
                        {
                            if (Main.Players[player].OnDuty || Main.Players[player].Fraction.InClothes)
                            {
                                GUI.Dashboard.Update(player, item, index);
                                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы должны закончить рабочий день");
                                return;
                            }
                            if (item.IsActive)
                            {
                                if (Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Undershit.Variation == Customization.EmtptySlots[gender][8] || (!gender && Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Undershit.Variation == 15))
                                    Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Top = new ComponentItem(Customization.EmtptySlots[gender][11], 0);
                                else
                                {
                                    var underwearID = Customization.Undershirts[gender][Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Undershit.Variation];
                                    var underwear = Customization.Underwears[gender][underwearID];
                                    if (underwear.Top == -1)
                                        Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Top = new ComponentItem(Customization.EmtptySlots[gender][11], 0);
                                    else
                                        Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Top = new ComponentItem(underwear.Top, Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Undershit.Texture);
                                    Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Undershit = new ComponentItem(Customization.EmtptySlots[gender][8], 0);
                                }

                                nInventory.Items[UUID][index].IsActive = false;
                                GUI.Dashboard.Update(player, item, index);
                            }
                            else
                            {
                                var itemData = (string)item.Data;
                                var variation = Convert.ToInt32(itemData.Split('_')[0]);
                                var texture = Convert.ToInt32(itemData.Split('_')[1]);

                                if (Customization.Tops[gender].FirstOrDefault(t => t.Variation == Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Top.Variation) != null || Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Top.Variation == Customization.EmtptySlots[gender][11])
                                {
                                    if (Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Undershit.Variation == Customization.EmtptySlots[gender][8] || (!gender && Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Undershit.Variation == 15))
                                        Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Top = new ComponentItem(variation, texture);
                                    else
                                    {
                                        var underwearID = Customization.Undershirts[gender][Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Undershit.Variation];
                                        var underwear = Customization.Underwears[gender][underwearID];
                                        var underwearTexture = Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Undershit.Texture;
                                        var topType = Customization.Tops[gender].FirstOrDefault(t => t.Variation == variation).Type;
                                        Log.Debug($"UnderwearID: {underwearID} | TopType: {topType}");
                                        if (!underwear.UndershirtIDs.ContainsKey(topType))
                                        {
                                            Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Undershit = new ComponentItem(Customization.EmtptySlots[gender][8], 0);
                                            nInventory.UnActiveItem(player, ItemType.Undershit);
                                        }
                                        else
                                            Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Undershit = new ComponentItem(underwear.UndershirtIDs[topType], underwearTexture);
                                        Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Top = new ComponentItem(variation, texture);
                                    }
                                }
                                else
                                {
                                    var underwearID = 0;
                                    var underwear = Customization.Underwears[gender].Values.FirstOrDefault(u => u.Top == Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Top.Variation);
                                    var underwearTexture = Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Top.Texture;
                                    if (underwear != null)
                                    {
                                        var topType = Customization.Tops[gender].FirstOrDefault(t => t.Variation == variation).Type;
                                        Log.Debug($"UnderwearID: {underwearID} | TopType: {topType}");
                                        if (!underwear.UndershirtIDs.ContainsKey(topType))
                                        {
                                            Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Undershit = new ComponentItem(Customization.EmtptySlots[gender][8], 0);
                                            nInventory.UnActiveItem(player, ItemType.Undershit);
                                        }
                                        else
                                            Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Undershit = new ComponentItem(underwear.UndershirtIDs[topType], underwearTexture);
                                    }
                                    Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Top = new ComponentItem(variation, texture);
                                }

                                nInventory.UnActiveItem(player, item.Type);
                                nInventory.Items[UUID][index].IsActive = true;
                                GUI.Dashboard.Update(player, item, index);
                            }

                            var gloves = Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Gloves.Variation;
                            if (gloves != 0 &&
                                !Customization.CorrectGloves[gender][gloves].ContainsKey(Customization.CorrectTorso[gender][Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Top.Variation]))
                            {
                                nInventory.UnActiveItem(player, ItemType.Gloves);
                                Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Gloves = new ComponentItem(0, 0);
                            }

                            player.SetClothes(8, Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Undershit.Variation, Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Undershit.Texture);
                            player.SetClothes(11, Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Top.Variation, Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Top.Texture);
                            var noneGloves = Customization.CorrectTorso[gender][Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Top.Variation];
                            if (Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Gloves.Variation == 0)
                            {
                                Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Torso = new ComponentItem(noneGloves, 0);
                                player.SetClothes(3, noneGloves, 0);
                            }
                            else
                                player.SetClothes(3, Customization.CorrectGloves[gender][Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Gloves.Variation][noneGloves], Customization.CustomPlayerData[Main.Players[player].UUID].Clothes.Gloves.Texture);
                            return;
                        }
                    #endregion
                    case ItemType.BagWithDrill:
                    case ItemType.BagWithMoney:
                    case ItemType.Pocket:
                    case ItemType.Cuffs:
                    case ItemType.CarKey:
                    case ItemType.CasinoChips:
                    case ItemType.ProductBox:
                    case ItemType.TrashBag:
                    case ItemType.NumberPlate:
                        return;
                    case ItemType.SimCard:
                        if(Main.Players[player].Sim != -1)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"В телефоне уже установлена сим-карта с номером {Main.Players[player].Sim}!", 3000);
                            return;
                        }
                        string data = item.Data;

                        var strings = data.Split(',');

                        Main.Players[player].Sim = Convert.ToInt32(strings[0]);
                        Main.Players[player].SimBalance = Convert.ToInt32(strings[1]);

                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы вставили в телефон сим-карту {strings[0]} !", 3000);
                        break;
                    case ItemType.KeyRing:
                        List<nItem> items = new List<nItem>();
                        data = item.Data;
                        List<string> keys = (data.Length == 0) ? new List<string>() : new List<string>(data.Split('/'));
                        if (keys.Count > 0 && string.IsNullOrEmpty(keys[keys.Count - 1]))
                            keys.RemoveAt(keys.Count - 1);

                        foreach (var key in keys)
                            items.Add(new nItem(ItemType.CarKey, 1, key));
                        player.SetMyData("KEYRING", nInventory.Items[Main.Players[player].UUID].IndexOf(item));
                        GUI.Dashboard.OpenOut(player, items, "Связка ключей", 7);
                        return;
                    case ItemType.Material:
                        //Trigger.ClientEvent(player, "board", "close");
                        //GUI.Dashboard.isopen[player] = false;
                        //GUI.Dashboard.Close(player);
                        //Fractions.Manager.OpenGunCraftMenu(player);
                        return;
                    case ItemType.Beer:
                        if (player.HasMyData("USEFOOD")) return;
                        BasicSync.AddAttachment(player, "obj_sh_beer_pissh_01", false);
                        //Commands.RPChat("me", player, $"выпил(а) {nInventory.ItemsNames[(int)item.Type]}");
                        player.PlayAnimation("amb@world_human_drinking@beer@male@idle_a", "idle_b", 49);
                        player.SetMyData("USEFOOD", true);
                        NAPI.Task.Run(() => {
                            if (!Main.Players.ContainsKey(player)) return;
                            if (EatManager.ConsumeItemEatAndWater.ContainsKey(ItemType.Beer))
                            {
                                var eatItem = EatManager.ConsumeItemEatAndWater[ItemType.Beer];
                                EatManager.AddHealth(player, eatItem.Health);
                                EatManager.AddWater(player, eatItem.Water);
                                EatManager.AddEat(player, eatItem.Eat);
                            }
                            player.ResetMyData("USEFOOD");
                            client.Core.Achievements.AddAchievementScore(player, client.Core.AchievementID.Water, 1);
                            player.StopAnimation();
                            BasicSync.AddAttachment(player, "obj_sh_beer_pissh_01", true);
                        }, 6000);
                        break;
                    case ItemType.Burger:
                        if (player.HasMyData("USEFOOD")) return;
                        BasicSync.AddAttachment(player, "obj_cs_burger_01", false);
                        player.PlayAnimation("amb@code_human_wander_eating_donut@female@idle_a", "idle_b", 49);
                        //Commands.RPChat("me", player, $"съел(а) {nInventory.ItemsNames[(int)item.Type]}");
                        player.SetMyData("USEFOOD", true);
                        NAPI.Task.Run(() => {
                            if (!Main.Players.ContainsKey(player)) return;
                            if (EatManager.ConsumeItemEatAndWater.ContainsKey(ItemType.Burger))
                            {
                                var eatItem = EatManager.ConsumeItemEatAndWater[ItemType.Burger];
                                EatManager.AddHealth(player, eatItem.Health);
                                EatManager.AddWater(player, eatItem.Water);
                                EatManager.AddEat(player, eatItem.Eat);
                            }
                            player.ResetMyData("USEFOOD");
                            if (player.GetMyData<int>("RESIST_TIME") < 600) Trigger.ClientEvent(player, "stopScreenEffect", "PPFilter");
                            client.Core.Achievements.AddAchievementScore(player, client.Core.AchievementID.Glutton, 1);
                            player.StopAnimation();
                            BasicSync.AddAttachment(player, "obj_cs_burger_01", true);
                        }, 6000);
                        break;
                    case ItemType.eCola:
                        if (player.HasMyData("USEFOOD")) return;
                        BasicSync.AddAttachment(player, "obj_food_juice01", false);
                        player.PlayAnimation("amb@world_human_drinking@beer@male@idle_a", "idle_b", 49);
                        //Commands.RPChat("me", player, $"выпил(а) {nInventory.ItemsNames[(int)item.Type]}");
                        player.SetMyData("USEFOOD", true);
                        NAPI.Task.Run(() => {
                            if (!Main.Players.ContainsKey(player)) return;
                            if (EatManager.ConsumeItemEatAndWater.ContainsKey(ItemType.eCola))
                            {
                                var eatItem = EatManager.ConsumeItemEatAndWater[ItemType.eCola];
                                EatManager.AddHealth(player, eatItem.Health);
                                EatManager.AddWater(player, eatItem.Water);
                                EatManager.AddEat(player, eatItem.Eat);
                            }
                            player.ResetMyData("USEFOOD");
                            client.Core.Achievements.AddAchievementScore(player, client.Core.AchievementID.Water, 1);
                            player.StopAnimation();

                            BasicSync.AddAttachment(player, "obj_food_juice01", true);
                        }, 6000);
                        break;
                    case ItemType.HotDog:
                        if (player.HasMyData("USEFOOD")) return;
                        BasicSync.AddAttachment(player, "obj_cs_hotdog_01", false);
                        player.PlayAnimation("amb@code_human_wander_eating_donut@female@idle_a", "idle_b", 49);
                        //Commands.RPChat("me", player, $"съел(а) {nInventory.ItemsNames[(int)item.Type]}");
                        player.SetMyData("USEFOOD", true);
                        NAPI.Task.Run(() => {
                            if (!Main.Players.ContainsKey(player)) return;
                            if (EatManager.ConsumeItemEatAndWater.ContainsKey(ItemType.HotDog))
                            {
                                var eatItem = EatManager.ConsumeItemEatAndWater[ItemType.HotDog];
                                EatManager.AddHealth(player, eatItem.Health);
                                EatManager.AddWater(player, eatItem.Water);
                                EatManager.AddEat(player, eatItem.Eat);
                            }
                            player.ResetMyData("USEFOOD");
                            if (player.GetMyData<int>("RESIST_TIME") < 600) Trigger.ClientEvent(player, "stopScreenEffect", "PPFilter");

                            client.Core.Achievements.AddAchievementScore(player, client.Core.AchievementID.Glutton, 1);
                            player.StopAnimation();

                            BasicSync.AddAttachment(player, "obj_cs_hotdog_01", true);
                        }, 6000);
                        break;
                    case ItemType.Pizza:
                        if (player.HasMyData("USEFOOD")) return;
                        BasicSync.AddAttachment(player, "v_res_tt_pizzaplate", false);
                        player.PlayAnimation("amb@code_human_wander_eating_donut@female@idle_a", "idle_b", 49);
                        //Commands.RPChat("me", player, $"съел(а) {nInventory.ItemsNames[(int)item.Type]}");
                        player.SetMyData("USEFOOD", true);
                        NAPI.Task.Run(() => {
                            if (!Main.Players.ContainsKey(player)) return;
                            if (EatManager.ConsumeItemEatAndWater.ContainsKey(ItemType.Pizza))
                            {
                                var eatItem = EatManager.ConsumeItemEatAndWater[ItemType.Pizza];
                                EatManager.AddHealth(player, eatItem.Health);
                                EatManager.AddWater(player, eatItem.Water);
                                EatManager.AddEat(player, eatItem.Eat);
                            }
                            player.ResetMyData("USEFOOD");
                            if (player.GetMyData<int>("RESIST_TIME") < 600) Trigger.ClientEvent(player, "stopScreenEffect", "PPFilter");

                            client.Core.Achievements.AddAchievementScore(player, client.Core.AchievementID.Glutton, 1);
                            player.StopAnimation();

                            BasicSync.AddAttachment(player, "v_res_tt_pizzaplate", true);
                        }, 6000);

                        #region GBPКвест: 21 Съесть 1000 пицц.

                        #region BattlePass выполнение квеста
                        BattlePass.updateBPGlobalQuestIteration(player, BattlePass.BPGlobalQuestType.EatPizza);
                        #endregion

                        #endregion
                        break;
                    case ItemType.Sandwich:
                        if (player.HasMyData("USEFOOD")) return;
                        //Commands.RPChat("me", player, $"съел(а) {nInventory.ItemsNames[(int)item.Type]}");
                        BasicSync.AddAttachment(player, "obj_sandwich_01", false);
                        player.PlayAnimation("amb@code_human_wander_eating_donut@female@idle_a", "idle_b", 49);
                        player.SetMyData("USEFOOD", true);
                        NAPI.Task.Run(() => {
                            if (!Main.Players.ContainsKey(player)) return;
                            if (EatManager.ConsumeItemEatAndWater.ContainsKey(ItemType.Sandwich))
                            {
                                var eatItem = EatManager.ConsumeItemEatAndWater[ItemType.Sandwich];
                                EatManager.AddHealth(player, eatItem.Health);
                                EatManager.AddWater(player, eatItem.Water);
                                EatManager.AddEat(player, eatItem.Eat);
                            }

                            player.ResetMyData("USEFOOD");
                            if (player.GetMyData<int>("RESIST_TIME") < 600) Trigger.ClientEvent(player, "stopScreenEffect", "PPFilter");
                        //
                            client.Core.Achievements.AddAchievementScore(player, client.Core.AchievementID.Glutton, 1);
                            player.StopAnimation();

                            BasicSync.AddAttachment(player, "obj_sandwich_01", true);
                        }, 6000);
                        break;
                    case ItemType.Sprunk:
                        if (player.HasMyData("USEFOOD")) return;
                        //Commands.RPChat("me", player, $"выпил(а) {nInventory.ItemsNames[(int)item.Type]}");
                        BasicSync.AddAttachment(player, "obj_ecola_can", false);
                        player.PlayAnimation("amb@world_human_drinking@beer@male@idle_a", "idle_b", 49);
                        player.SetMyData("USEFOOD", true);
                        NAPI.Task.Run(() => {
                            if (!Main.Players.ContainsKey(player)) return;
                            if (EatManager.ConsumeItemEatAndWater.ContainsKey(ItemType.Sprunk))
                            {
                                var eatItem = EatManager.ConsumeItemEatAndWater[ItemType.Sprunk];
                                EatManager.AddHealth(player, eatItem.Health);
                                EatManager.AddWater(player, eatItem.Water);
                                EatManager.AddEat(player, eatItem.Eat);
                            }
                            player.ResetMyData("USEFOOD");
                            if (player.GetMyData<int>("RESIST_TIME") < 600) Trigger.ClientEvent(player, "stopScreenEffect", "PPFilter");
                        //
                            client.Core.Achievements.AddAchievementScore(player, client.Core.AchievementID.Water, 1);
                            player.StopAnimation();

                            BasicSync.AddAttachment(player, "obj_ecola_can", true);
                        }, 6000);

                        #region GBPКвест: 22 Выпить 1000 Спранков.

                        #region BattlePass выполнение квеста
                        BattlePass.updateBPGlobalQuestIteration(player, BattlePass.BPGlobalQuestType.EatSprunk);
                        #endregion

                        #endregion
                        break;
                    case ItemType.WaterBottle:
                        if (player.HasMyData("USEFOOD")) return;
                        //Commands.RPChat("me", player, $"выпил(а) {nInventory.ItemsNames[(int)item.Type]}");
                        BasicSync.AddAttachment(player, "obj_ld_flow_bottle", false);
                        player.PlayAnimation("amb@world_human_drinking@beer@male@idle_a", "idle_b", 49);
                        player.SetMyData("USEFOOD", true);
                        NAPI.Task.Run(() => {
                            if (!Main.Players.ContainsKey(player)) return;
                            if (EatManager.ConsumeItemEatAndWater.ContainsKey(ItemType.WaterBottle))
                            {
                                var eatItem = EatManager.ConsumeItemEatAndWater[ItemType.WaterBottle];
                                EatManager.AddHealth(player, eatItem.Health);
                                EatManager.AddWater(player, eatItem.Water);
                                EatManager.AddEat(player, eatItem.Eat);
                            }
                            player.ResetMyData("USEFOOD");
                            if (player.GetMyData<int>("RESIST_TIME") < 600) Trigger.ClientEvent(player, "stopScreenEffect", "PPFilter");
                            //
                            client.Core.Achievements.AddAchievementScore(player, client.Core.AchievementID.Water, 1);
                            player.StopAnimation();

                            BasicSync.AddAttachment(player, "obj_ld_flow_bottle", true);
                        }, 6000);
                        break;
                    case ItemType.Сrisps:
                        if (player.HasMyData("USEFOOD")) return;
                        //Commands.RPChat("me", player, $"съел(а) {nInventory.ItemsNames[(int)item.Type]}");
                        BasicSync.AddAttachment(player, "obj_food_bs_chips", false);
                        player.PlayAnimation("amb@code_human_wander_eating_donut@female@idle_a", "idle_b", 49);
                        player.SetMyData("USEFOOD", true);
                        NAPI.Task.Run(() => {
                            if (!Main.Players.ContainsKey(player)) return;
                              if (EatManager.ConsumeItemEatAndWater.ContainsKey(ItemType.Сrisps))
                              {
                                var eatItem = EatManager.ConsumeItemEatAndWater[ItemType.Сrisps];
                                EatManager.AddHealth(player, eatItem.Health);
                                EatManager.AddWater(player, eatItem.Water);
                                EatManager.AddEat(player, eatItem.Eat);
                              }
                            player.ResetMyData("USEFOOD");
                            if (player.GetMyData<int>("RESIST_TIME") < 600) Trigger.ClientEvent(player, "stopScreenEffect", "PPFilter");
                            player.StopAnimation();
                            BasicSync.AddAttachment(player, "obj_food_bs_chips", true);
                            client.Core.Achievements.AddAchievementScore(player, client.Core.AchievementID.Glutton, 1);
                        }, 6000);
                        break;
                    //case ItemType.Rod:
                    //    RodManager.useInventory(player, 1);
                    //    break;
                    //case ItemType.RodUpgrade:
                    //    RodManager.useInventory(player, 2);
                    //    break;
                    //case ItemType.RodMK2:
                    //    RodManager.useInventory(player, 3);
                        //break;
                    case ItemType.Drugs:
                        if (!player.HasMyData("USE_DRUGS") || DateTime.Now > player.GetMyData<DateTime>("USE_DRUGS"))
                        {
                            player.PlayAnimation("amb@code_human_wander_smoking_fat@male@idle_a", "idle_b", 49);
                            NAPI.Task.Run(() => {
                                player.StopAnimation();
                                Main.OffAntiAnim(player);
                            }, 5000);
                            player.Health = (player.Health + 50 > 100) ? 100 : player.Health + 50;
                            Trigger.ClientEvent(player, "startScreenEffect", "DrugsTrevorClownsFight", 300000, false);
                            //Commands.RPChat("me", player, $"закурил(а) косяк");
                            player.SetMyData("USE_DRUGS", DateTime.Now.AddMinutes(3));
                            client.Core.Achievements.AddAchievementScore(player, client.Core.AchievementID.Legalize, 1);
                        }
                        else
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Попробуйте использовать позже", 3000);
                            return;
                        }
                        break;
                    case ItemType.Cocaine:
                        if (!player.HasMyData("USE_DRUGS") || DateTime.Now > player.GetMyData<DateTime>("USE_DRUGS"))
                        {
                            player.PlayAnimation("amb@code_human_wander_smoking_fat@male@idle_a", "idle_b", 49);
                            NAPI.Task.Run(() => {
                                player.StopAnimation();
                                Main.OffAntiAnim(player);
                            }, 5000);
                            player.Health = (player.Health + 50 > 100) ? 100 : player.Health + 50;
                            Trigger.ClientEvent(player, "startScreenEffect", "DrugsTrevorClownsFight", 300000, false);
                            //Commands.RPChat("me", player, $"закурил(а) косяк");
                            player.SetMyData("USE_DRUGS", DateTime.Now.AddMinutes(3));
                            client.Core.Achievements.AddAchievementScore(player, client.Core.AchievementID.Legalize, 1);
                        }
                        else
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Попробуйте использовать позже", 3000);
                            return;
                        }
                        break;
                    case ItemType.GasCan:
                        if (!player.IsInVehicle)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы должны находиться в машине", 3000);
                            GUI.Dashboard.Close(player);
                            return;
                        }
                        var veh = player.Vehicle;
                        if (!veh.HasSharedData("PETROL")) return;
                        var fuel = veh.GetSharedData<int>("PETROL");
                        if (fuel == VehicleManager.VehicleTank[veh.Class])
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"В машине полный бак", 3000);
                            GUI.Dashboard.Close(player);
                            return;
                        }
                        fuel += 30;
                        if (fuel > VehicleManager.VehicleTank[veh.Class]) fuel = VehicleManager.VehicleTank[veh.Class];
                        veh.SetSharedData("PETROL", fuel);
                        if (player.Vehicle.HasData("ACCESS") && player.Vehicle.GetData<string>("ACCESS") == "GARAGE")
                        {
                            int id = player.Vehicle.GetData<int>("ID");
                            var number = player.Vehicle.NumberPlate;
                            VehicleManager.Vehicles[id].Fuel = fuel;
                        }
                        break;
                    case ItemType.HealthKit:
                        if (!player.HasMyData("USE_MEDKIT") || DateTime.Now > player.GetMyData<DateTime>("USE_MEDKIT"))
                        {
                            player.Health = 100;
                            player.SetMyData("USE_MEDKIT", DateTime.Now.AddMinutes(5));
                            Main.OnAntiAnim(player);
                            player.PlayAnimation("amb@code_human_wander_texting_fat@female@enter", "enter", 49);
                            NAPI.Task.Run(() => {
                                try
                                {
                                    if (player == null) return;
                                    if (!player.IsInVehicle) player.StopAnimation();
                                    else player.SetMyData("ToResetAnimPhone", true);
                                    Main.OffAntiAnim(player);
                                    Trigger.ClientEvent(player, "stopScreenEffect", "PPFilter");
                                } catch (Exception e){ Log.Write(e.StackTrace, nLog.Type.Error); }
                            }, 5000);
                            Commands.RPChat("me", player, $"использовал(а) аптечку");
                        }
                        else
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Попробуйте использовать позже", 3000);
                            return;
                        }
                        break;
                    case ItemType.SmallHealthKit:
                        if (!player.HasMyData("USE_MEDKIT") || DateTime.Now > player.GetMyData<DateTime>("USE_MEDKIT"))
                        {
                            player.Health = (player.Health + 50 > 100) ? 100 : player.Health + 50;
                            player.SetMyData("USE_MEDKIT", DateTime.Now.AddMinutes(5));
                            Main.OnAntiAnim(player);
                            player.PlayAnimation("amb@code_human_wander_texting_fat@female@enter", "enter", 49);
                            NAPI.Task.Run(() => {
                                try
                                {
                                    if (player == null) return;
                                    if (!player.IsInVehicle) player.StopAnimation();
                                    else player.SetMyData("ToResetAnimPhone", true);
                                    Main.OffAntiAnim(player);
                                    Trigger.ClientEvent(player, "stopScreenEffect", "PPFilter");
                                }
                                catch (Exception e) { Log.Write(e.StackTrace, nLog.Type.Error); }
                            }, 5000);
                            Commands.RPChat("me", player, $"использовал(а) аптечку");
                        }
                        else
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Попробуйте использовать позже", 3000);
                            return;
                        }
                        break;
                    case ItemType.Bandage:
                      if (!player.HasMyData("USE_BANDAGE") || DateTime.Now > player.GetMyData<DateTime>("USE_BANDAGE"))
                      {


                        player.Health = (player.Health + 15 > 100) ? 100 : player.Health + 15;
                        player.SetMyData("USE_BANDAGE", DateTime.Now.AddMinutes(1));
                        Main.OnAntiAnim(player);
                        player.PlayAnimation("amb@code_human_wander_texting_fat@female@enter", "enter", 49);
                        NAPI.Task.Run(() => {
                          try
                          {
                            if (player == null) return;
                            if (!player.IsInVehicle) player.StopAnimation();
                            else player.SetMyData("ToResetAnimPhone", true);
                            Main.OffAntiAnim(player);
                            Trigger.ClientEvent(player, "stopScreenEffect", "PPFilter");
                          }
                          catch (Exception e) { Log.Write(e.StackTrace, nLog.Type.Error); }
                        }, 5000);
                        Commands.RPChat("me", player, $"использовал(а) аптечку");
                      }
                      else
                      {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Попробуйте использовать позже", 3000);
                        return;
                      }
                      break;
                    case ItemType.Lockpick:
                        if (player.GetMyData<int>("INTERACTIONCHECK") != 3)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Невозможно использовать в данный момент", 3000);
                            GUI.Dashboard.Close(player);
                            return;
                        }
                        //player.SetMyData("LOCK_TIMER", Main.StartT(10000, 999999, (o) => SafeMain.lockCrack(player, player.Name), "LOCK_TIMER"));
                        player.SetMyData("LOCK_TIMER", Timers.StartOnce(10000, () => SafeMain.lockCrack(player, player.Name)));
                        //player.FreezePosition = true;
                        Trigger.ClientEvent(player, "showLoader", "Идёт взлом", 1);
                        Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы начали взламывать дверь", 3000);
                        break;
                    case ItemType.ArmyLockpick:
                        if (!player.IsInVehicle || player.Vehicle.DisplayName != "BARRACKS")
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы должны находиться в военном перевозчике материалов", 3000);
                            return;
                        }
                        if (VehicleStreaming.GetEngineState(player.Vehicle))
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Машина уже заведена", 3000);
                            return;
                        }
                        VehicleStreaming.SetEngineState(player.Vehicle, true);
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"У Вас получилось завести машину", 3000);
                        break;
                    case ItemType.RepairBox:
                        if (!player.IsInVehicle)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы должны находиться в машине", 3000);
                            return;
                        }
                        if (VehicleStreaming.GetEngineState(player.Vehicle))
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Сначало заглушите автомобиль", 3000);
                            return;
                        }
                        VehicleManager.RepairCar(player.Vehicle);
                        break;
                    case ItemType.Present:
                        player.Health = (player.Health + 10 > 100) ? 100 : player.Health + 10;
                        MoneySystem.Wallet.Change(player, 5000);

                        var before = Main.Accounts[player].RedBucks;
                        Main.Accounts[player].RedBucks += 100;

                        Log.Debug($"[SWC Changes][{player.Name}] [Inventory] Награда из подарка в инвентаре: [100] {before} -> {Main.Accounts[player].RedBucks}");
                        GameLog.SWC(Main.Players[player].UUID, $"[Inventory] Награда из подарка в инвентаре", Main.Accounts[player].Login, 100, before);

                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы получили 100 SWCoins и 5000$", 3000);
                        //Commands.RPChat("me", player, $"открыл(а) подарок {types.Item1} + {types.Item2}");
                        break;
                    case ItemType.Payek:
                        if (player.VehicleSeat == 0)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы не можете сьесть Сух.Паёк находясь за рулем.", 3000);
                            return;
                        }
                        if (!player.HasMyData("USE_MEDKIT") || DateTime.Now > player.GetMyData<DateTime>("USE_MEDKIT"))
                        {
                            if (EatManager.ConsumeItemEatAndWater.ContainsKey(ItemType.Payek))
                            {
                              var eatItem = EatManager.ConsumeItemEatAndWater[ItemType.Payek];
                              EatManager.AddHealth(player, eatItem.Health);
                              EatManager.AddWater(player, eatItem.Water);
                              EatManager.AddEat(player, eatItem.Eat);
                            }

                            NAPI.Task.Run(() => {
                                try
                                {
                                    if (player == null) return;
                                    else player.SetMyData("ToResetAnimPhone", true);
                                    Trigger.ClientEvent(player, "stopScreenEffect", "PPFilter");
                                }
                                catch (Exception e){ Log.Write(e.StackTrace, nLog.Type.Error); }
                            }, 5000);
                            //Commands.RPChat("me", player, $"сьел(а) Сух.Паёк");
                            client.Core.Achievements.AddAchievementScore(player, client.Core.AchievementID.Glutton, 1);
                        }
                        else
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Попробуйте использовать позже", 3000);
                            return;
                        }
                        break;
                    case ItemType.CraftSmelter:
                        if (!Main.Players.ContainsKey(player))
                            return;
                        if (!CraftSystem.PlayersHomeCraftData.ContainsKey(Main.Players[player].UUID))
                            return;
                        if (CraftSystem.PlayersHomeCraftData[Main.Players[player].UUID].showedMenus.Contains(0))
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "У вас уже есть сборщик деталей");
                            return;
                        }
                        CraftSystem.PlayersHomeCraftData[Main.Players[player].UUID].showedMenus.Add(0);
                            break;
                    case ItemType.CraftPercolator:
                        if (!Main.Players.ContainsKey(player))
                            return;
                        if (!CraftSystem.PlayersHomeCraftData.ContainsKey(Main.Players[player].UUID))
                            return;
                        if (CraftSystem.PlayersHomeCraftData[Main.Players[player].UUID].showedMenus.Contains(1))
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "У вас уже есть сборщик деталей");
                            return;
                        }
                        CraftSystem.PlayersHomeCraftData[Main.Players[player].UUID].showedMenus.Add(1);
                        break;
                    case ItemType.CraftPartsCollector:
                        if (!Main.Players.ContainsKey(player))
                            return;
                        if (!CraftSystem.PlayersHomeCraftData.ContainsKey(Main.Players[player].UUID))
                            return;
                        if (CraftSystem.PlayersHomeCraftData[Main.Players[player].UUID].showedMenus.Contains(2))
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "У вас уже есть сборщик деталей");
                            return;
                        }
                        CraftSystem.PlayersHomeCraftData[Main.Players[player].UUID].showedMenus.Add(2);
                        break;
                    case ItemType.CraftWorkBench:
                        if (!Main.Players.ContainsKey(player))
                            return;
                        if (!CraftSystem.PlayersHomeCraftData.ContainsKey(Main.Players[player].UUID))
                            return;
                        if(CraftSystem.PlayersHomeCraftData[Main.Players[player].UUID].WorkBench == null)
                        {
                            CraftSystem.PlayersHomeCraftData[Main.Players[player].UUID].WorkBench = new WorkBench(-1, WorkBenchCraft.bench1lvl.Items, WorkBenchCraft.bench1lvl.CurrectRecept);
                        }
                        if (CraftSystem.PlayersHomeCraftData[Main.Players[player].UUID].WorkBench.Level >= 0)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "У вас уже есть станок");
                            return;
                        }
                        CraftSystem.PlayersHomeCraftData[Main.Players[player].UUID].WorkBench.Level = 0;
                        break;
                    case ItemType.CraftWorkBenchUpgrade:
                        if (!Main.Players.ContainsKey(player))
                            return;
                        if (!CraftSystem.PlayersHomeCraftData.ContainsKey(Main.Players[player].UUID))
                            return;
                        if (CraftSystem.PlayersHomeCraftData[Main.Players[player].UUID].WorkBench == null)
                        {
                            CraftSystem.PlayersHomeCraftData[Main.Players[player].UUID].WorkBench = new WorkBench(-1, WorkBenchCraft.bench1lvl.Items, WorkBenchCraft.bench1lvl.CurrectRecept);
                        }
                        if (CraftSystem.PlayersHomeCraftData[Main.Players[player].UUID].WorkBench.Level != 0)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Нужен станок 1-го уровня");
                            return;
                        }
                        CraftSystem.PlayersHomeCraftData[Main.Players[player].UUID].WorkBench.Level = 1;
                        break;
                    case ItemType.CraftWorkBenchUpgrade2:
                        if (!Main.Players.ContainsKey(player))
                            return;
                        if (!CraftSystem.PlayersHomeCraftData.ContainsKey(Main.Players[player].UUID))
                            return;
                        if (CraftSystem.PlayersHomeCraftData[Main.Players[player].UUID].WorkBench == null)
                        {
                            CraftSystem.PlayersHomeCraftData[Main.Players[player].UUID].WorkBench = new WorkBench(-1, WorkBenchCraft.bench1lvl.Items, WorkBenchCraft.bench1lvl.CurrectRecept);
                        }
                        if (CraftSystem.PlayersHomeCraftData[Main.Players[player].UUID].WorkBench.Level != 1)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Нужен станок 2-го уровня");
                            return;
                        }
                        CraftSystem.PlayersHomeCraftData[Main.Players[player].UUID].WorkBench.Level = 2;
                        break;
                    #region not used items
                    #region fish
                    case ItemType.Bait:
                        case ItemType.Naz:
                        case ItemType.Kyndja:
                        case ItemType.Sig:
                        case ItemType.Omyl:
                        case ItemType.Nerka:
                        case ItemType.Forel:
                        case ItemType.Ship:
                        case ItemType.Lopatonos:
                        case ItemType.Osetr:
                        case ItemType.Semga:
                        case ItemType.Servyga:
                        case ItemType.Beluga:
                        case ItemType.Taimen:
                        case ItemType.Sterlyad:
                        case ItemType.Ydilshik:
                        case ItemType.Hailiod:
                        case ItemType.Keta:
                        case ItemType.Gorbysha:
                        #endregion
                        #region weapon components
                        case ItemType.ADVANCEDRIFLE_CLIP_02:
                                    case ItemType.ADVANCEDRIFLE_VARMOD_LUXE:
                                    case ItemType.APPISTOL_CLIP_02:
                                    case ItemType.APPISTOL_VARMOD_LUXE:
                                    case ItemType.ASSAULTRIFLE_CLIP_02:
                                    case ItemType.ASSAULTRIFLE_CLIP_03:
                                    case ItemType.ASSAULTRIFLE_MK2_CAMO:
                                    case ItemType.ASSAULTRIFLE_MK2_CAMO_02:
                                    case ItemType.ASSAULTRIFLE_MK2_CAMO_03:
                                    case ItemType.ASSAULTRIFLE_MK2_CAMO_04:
                                    case ItemType.ASSAULTRIFLE_MK2_CAMO_05:
                                    case ItemType.ASSAULTRIFLE_MK2_CAMO_06:
                                    case ItemType.ASSAULTRIFLE_MK2_CAMO_07:
                                    case ItemType.ASSAULTRIFLE_MK2_CAMO_08:
                                    case ItemType.ASSAULTRIFLE_MK2_CAMO_09:
                                    case ItemType.ASSAULTRIFLE_MK2_CAMO_10:
                                    case ItemType.ASSAULTRIFLE_MK2_CAMO_IND_01:
                                    case ItemType.ASSAULTRIFLE_MK2_CLIP_02:
                                    case ItemType.ASSAULTRIFLE_MK2_CLIP_ARMORPIERCING:
                                    case ItemType.ASSAULTRIFLE_MK2_CLIP_FMJ:
                                    case ItemType.ASSAULTRIFLE_MK2_CLIP_INCENDIARY:
                                    case ItemType.ASSAULTRIFLE_MK2_CLIP_TRACER:
                                    case ItemType.ASSAULTRIFLE_VARMOD_LUXE:
                                    case ItemType.ASSAULTSHOTGUN_CLIP_02:
                                    case ItemType.ASSAULTSMG_CLIP_02:
                                    case ItemType.ASSAULTSMG_VARMOD_LOWRIDER:
                                    case ItemType.AT_AR_AFGRIP:
                                    case ItemType.AT_AR_AFGRIP_02:
                                    case ItemType.AT_AR_BARREL_02:
                                    case ItemType.AT_AR_FLSH:
                                    case ItemType.AT_AR_SUPP:
                                    case ItemType.AT_AR_SUPP_02:
                                    case ItemType.AT_BP_BARREL_02:
                                    case ItemType.AT_CR_BARREL_02:
                                    case ItemType.AT_MG_BARREL_02:
                                    case ItemType.AT_MRFL_BARREL_02:
                                    case ItemType.AT_MUZZLE_01:
                                    case ItemType.AT_MUZZLE_02:
                                    case ItemType.AT_MUZZLE_03:
                                    case ItemType.AT_MUZZLE_04:
                                    case ItemType.AT_MUZZLE_05:
                                    case ItemType.AT_MUZZLE_06:
                                    case ItemType.AT_MUZZLE_07:
                                    case ItemType.AT_MUZZLE_08:
                                    case ItemType.AT_MUZZLE_09:
                                    case ItemType.AT_PI_COMP:
                                    case ItemType.AT_PI_COMP_02:
                                    case ItemType.AT_PI_COMP_03:
                                    case ItemType.AT_PI_FLSH:
                                    case ItemType.AT_PI_FLSH_02:
                                    case ItemType.AT_PI_FLSH_03:
                                    case ItemType.AT_PI_RAIL:
                                    case ItemType.AT_PI_RAIL_02:
                                    case ItemType.AT_PI_SUPP:
                                    case ItemType.AT_PI_SUPP_02:
                                    case ItemType.AT_SB_BARREL_02:
                                    case ItemType.AT_SC_BARREL_02:
                                    case ItemType.AT_SCOPE_LARGE:
                                    case ItemType.AT_SCOPE_LARGE_FIXED_ZOOM:
                                    case ItemType.AT_SCOPE_LARGE_FIXED_ZOOM_MK2:
                                    case ItemType.AT_SCOPE_LARGE_MK2:
                                    case ItemType.AT_SCOPE_MACRO:
                                    case ItemType.AT_SCOPE_MACRO_02:
                                    case ItemType.AT_SCOPE_MACRO_02_MK2:
                                    case ItemType.AT_SCOPE_MACRO_02_SMG_MK2:
                                    case ItemType.AT_SCOPE_MACRO_MK2:
                                    case ItemType.AT_SCOPE_MAX:
                                    case ItemType.AT_SCOPE_MEDIUM:
                                    case ItemType.AT_SCOPE_MEDIUM_MK2:
                                    case ItemType.AT_SCOPE_NV:
                                    case ItemType.AT_SCOPE_SMALL:
                                    case ItemType.AT_SCOPE_SMALL_02:
                                    case ItemType.AT_SCOPE_SMALL_MK2:
                                    case ItemType.AT_SCOPE_SMALL_SMG_MK2:
                                    case ItemType.AT_SCOPE_THERMAL:
                                    case ItemType.AT_SIGHTS:
                                    case ItemType.AT_SIGHTS_SMG:
                                    case ItemType.AT_SR_BARREL_02:
                                    case ItemType.AT_SR_SUPP:
                                    case ItemType.AT_SR_SUPP_03:
                                    case ItemType.BULLPUPRIFLE_CLIP_02:
                                    case ItemType.BULLPUPRIFLE_MK2_CAMO:
                                    case ItemType.BULLPUPRIFLE_MK2_CAMO_02:
                                    case ItemType.BULLPUPRIFLE_MK2_CAMO_03:
                                    case ItemType.BULLPUPRIFLE_MK2_CAMO_04:
                                    case ItemType.BULLPUPRIFLE_MK2_CAMO_05:
                                    case ItemType.BULLPUPRIFLE_MK2_CAMO_06:
                                    case ItemType.BULLPUPRIFLE_MK2_CAMO_07:
                                    case ItemType.BULLPUPRIFLE_MK2_CAMO_08:
                                    case ItemType.BULLPUPRIFLE_MK2_CAMO_09:
                                    case ItemType.BULLPUPRIFLE_MK2_CAMO_10:
                                    case ItemType.BULLPUPRIFLE_MK2_CAMO_IND_01:
                                    case ItemType.BULLPUPRIFLE_MK2_CLIP_02:
                                    case ItemType.BULLPUPRIFLE_MK2_CLIP_ARMORPIERCING:
                                    case ItemType.BULLPUPRIFLE_MK2_CLIP_FMJ:
                                    case ItemType.BULLPUPRIFLE_MK2_CLIP_INCENDIARY:
                                    case ItemType.BULLPUPRIFLE_MK2_CLIP_TRACER:
                                    case ItemType.BULLPUPRIFLE_VARMOD_LOW:
                                    case ItemType.CARBINERIFLE_CLIP_02:
                                    case ItemType.CARBINERIFLE_CLIP_03:
                                    case ItemType.CARBINERIFLE_MK2_CAMO:
                                    case ItemType.CARBINERIFLE_MK2_CAMO_02:
                                    case ItemType.CARBINERIFLE_MK2_CAMO_03:
                                    case ItemType.CARBINERIFLE_MK2_CAMO_04:
                                    case ItemType.CARBINERIFLE_MK2_CAMO_05:
                                    case ItemType.CARBINERIFLE_MK2_CAMO_06:
                                    case ItemType.CARBINERIFLE_MK2_CAMO_07:
                                    case ItemType.CARBINERIFLE_MK2_CAMO_08:
                                    case ItemType.CARBINERIFLE_MK2_CAMO_09:
                                    case ItemType.CARBINERIFLE_MK2_CAMO_10:
                                    case ItemType.CARBINERIFLE_MK2_CAMO_IND_01:
                                    case ItemType.CARBINERIFLE_MK2_CLIP_02:
                                    case ItemType.CARBINERIFLE_MK2_CLIP_ARMORPIERCING:
                                    case ItemType.CARBINERIFLE_MK2_CLIP_FMJ:
                                    case ItemType.CARBINERIFLE_MK2_CLIP_INCENDIARY:
                                    case ItemType.CARBINERIFLE_MK2_CLIP_TRACER:
                                    case ItemType.CARBINERIFLE_VARMOD_LUXE:
                                    case ItemType.CERAMICPISTOL_CLIP_02:
                                    case ItemType.CERAMICPISTOL_SUPP:
                                    case ItemType.COMBATMG_CLIP_02:
                                    case ItemType.COMBATMG_MK2_CAMO:
                                    case ItemType.COMBATMG_MK2_CAMO_02:
                                    case ItemType.COMBATMG_MK2_CAMO_03:
                                    case ItemType.COMBATMG_MK2_CAMO_04:
                                    case ItemType.COMBATMG_MK2_CAMO_05:
                                    case ItemType.COMBATMG_MK2_CAMO_06:
                                    case ItemType.COMBATMG_MK2_CAMO_07:
                                    case ItemType.COMBATMG_MK2_CAMO_08:
                                    case ItemType.COMBATMG_MK2_CAMO_09:
                                    case ItemType.COMBATMG_MK2_CAMO_10:
                                    case ItemType.COMBATMG_MK2_CAMO_IND_01:
                                    case ItemType.COMBATMG_MK2_CLIP_02:
                                    case ItemType.COMBATMG_MK2_CLIP_ARMORPIERCING:
                                    case ItemType.COMBATMG_MK2_CLIP_FMJ:
                                    case ItemType.COMBATMG_MK2_CLIP_INCENDIARY:
                                    case ItemType.COMBATMG_MK2_CLIP_TRACER:
                                    case ItemType.COMBATMG_VARMOD_LOWRIDER:
                                    case ItemType.COMBATPDW_CLIP_02:
                                    case ItemType.COMBATPDW_CLIP_03:
                                    case ItemType.COMBATPISTOL_CLIP_02:
                                    case ItemType.COMBATPISTOL_VARMOD_LOWRIDER:
                                    case ItemType.COMPACTRIFLE_CLIP_02:
                                    case ItemType.COMPACTRIFLE_CLIP_03:
                                    case ItemType.GUSENBERG_CLIP_02:
                                    case ItemType.HEAVYPISTOL_CLIP_02:
                                    case ItemType.HEAVYPISTOL_VARMOD_LUXE:
                                    case ItemType.HEAVYSHOTGUN_CLIP_02:
                                    case ItemType.HEAVYSHOTGUN_CLIP_03:
                                    case ItemType.HEAVYSNIPER_MK2_CAMO:
                                    case ItemType.HEAVYSNIPER_MK2_CAMO_02:
                                    case ItemType.HEAVYSNIPER_MK2_CAMO_03:
                                    case ItemType.HEAVYSNIPER_MK2_CAMO_04:
                                    case ItemType.HEAVYSNIPER_MK2_CAMO_05:
                                    case ItemType.HEAVYSNIPER_MK2_CAMO_06:
                                    case ItemType.HEAVYSNIPER_MK2_CAMO_07:
                                    case ItemType.HEAVYSNIPER_MK2_CAMO_08:
                                    case ItemType.HEAVYSNIPER_MK2_CAMO_09:
                                    case ItemType.HEAVYSNIPER_MK2_CAMO_10:
                                    case ItemType.HEAVYSNIPER_MK2_CAMO_IND_01:
                                    case ItemType.HEAVYSNIPER_MK2_CLIP_02:
                                    case ItemType.HEAVYSNIPER_MK2_CLIP_ARMORPIERCING:
                                    case ItemType.HEAVYSNIPER_MK2_CLIP_EXPLOSIVE:
                                    case ItemType.HEAVYSNIPER_MK2_CLIP_FMJ:
                                    case ItemType.HEAVYSNIPER_MK2_CLIP_INCENDIARY:
                                    case ItemType.KNUCKLE_VARMOD_BALLAS:
                                    case ItemType.KNUCKLE_VARMOD_DIAMOND:
                                    case ItemType.KNUCKLE_VARMOD_DOLLAR:
                                    case ItemType.KNUCKLE_VARMOD_HATE:
                                    case ItemType.KNUCKLE_VARMOD_KING:
                                    case ItemType.KNUCKLE_VARMOD_LOVE:
                                    case ItemType.KNUCKLE_VARMOD_PIMP:
                                    case ItemType.KNUCKLE_VARMOD_PLAYER:
                                    case ItemType.KNUCKLE_VARMOD_VAGOS:
                                    case ItemType.MACHINEPISTOL_CLIP_02:
                                    case ItemType.MACHINEPISTOL_CLIP_03:
                                    case ItemType.MARKSMANRIFLE_CLIP_02:
                                    case ItemType.MARKSMANRIFLE_MK2_CAMO:
                                    case ItemType.MARKSMANRIFLE_MK2_CAMO_02:
                                    case ItemType.MARKSMANRIFLE_MK2_CAMO_03:
                                    case ItemType.MARKSMANRIFLE_MK2_CAMO_04:
                                    case ItemType.MARKSMANRIFLE_MK2_CAMO_05:
                                    case ItemType.MARKSMANRIFLE_MK2_CAMO_06:
                                    case ItemType.MARKSMANRIFLE_MK2_CAMO_07:
                                    case ItemType.MARKSMANRIFLE_MK2_CAMO_08:
                                    case ItemType.MARKSMANRIFLE_MK2_CAMO_09:
                                    case ItemType.MARKSMANRIFLE_MK2_CAMO_10:
                                    case ItemType.MARKSMANRIFLE_MK2_CAMO_IND_01:
                                    case ItemType.MARKSMANRIFLE_MK2_CLIP_02:
                                    case ItemType.MARKSMANRIFLE_MK2_CLIP_ARMORPIERCING:
                                    case ItemType.MARKSMANRIFLE_MK2_CLIP_FMJ:
                                    case ItemType.MARKSMANRIFLE_MK2_CLIP_INCENDIARY:
                                    case ItemType.MARKSMANRIFLE_MK2_CLIP_TRACER:
                                    case ItemType.MARKSMANRIFLE_VARMOD_LUXE:
                                    case ItemType.MG_CLIP_02:
                                    case ItemType.MG_VARMOD_LOWRIDER:
                                    case ItemType.MICROSMG_CLIP_02:
                                    case ItemType.MICROSMG_VARMOD_LUXE:
                                    case ItemType.MILITARYRIFLE_CLIP_02:
                                    case ItemType.MILITARYRIFLE_SIGHT_01:
                                    case ItemType.MINISMG_CLIP_02:
                                    case ItemType.PISTOL50_CLIP_02:
                                    case ItemType.PISTOL50_VARMOD_LUXE:
                                    case ItemType.PISTOL_CLIP_02:
                                    case ItemType.PISTOL_MK2_CAMO:
                                    case ItemType.PISTOL_MK2_CAMO_02:
                                    case ItemType.PISTOL_MK2_CAMO_02_SLIDE:
                                    case ItemType.PISTOL_MK2_CAMO_03:
                                    case ItemType.PISTOL_MK2_CAMO_03_SLIDE:
                                    case ItemType.PISTOL_MK2_CAMO_04:
                                    case ItemType.PISTOL_MK2_CAMO_04_SLIDE:
                                    case ItemType.PISTOL_MK2_CAMO_05:
                                    case ItemType.PISTOL_MK2_CAMO_05_SLIDE:
                                    case ItemType.PISTOL_MK2_CAMO_06:
                                    case ItemType.PISTOL_MK2_CAMO_06_SLIDE:
                                    case ItemType.PISTOL_MK2_CAMO_07:
                                    case ItemType.PISTOL_MK2_CAMO_07_SLIDE:
                                    case ItemType.PISTOL_MK2_CAMO_08:
                                    case ItemType.PISTOL_MK2_CAMO_08_SLIDE:
                                    case ItemType.PISTOL_MK2_CAMO_09:
                                    case ItemType.PISTOL_MK2_CAMO_09_SLIDE:
                                    case ItemType.PISTOL_MK2_CAMO_10:
                                    case ItemType.PISTOL_MK2_CAMO_10_SLIDE:
                                    case ItemType.PISTOL_MK2_CAMO_IND_01:
                                    case ItemType.PISTOL_MK2_CAMO_IND_01_SLIDE:
                                    case ItemType.PISTOL_MK2_CAMO_SLIDE:
                                    case ItemType.PISTOL_MK2_CLIP_02:
                                    case ItemType.PISTOL_MK2_CLIP_FMJ:
                                    case ItemType.PISTOL_MK2_CLIP_HOLLOWPOINT:
                                    case ItemType.PISTOL_MK2_CLIP_INCENDIARY:
                                    case ItemType.PISTOL_MK2_CLIP_TRACER:
                                    case ItemType.PISTOL_VARMOD_LUXE:
                                    case ItemType.PUMPSHOTGUN_MK2_CAMO:
                                    case ItemType.PUMPSHOTGUN_MK2_CAMO_02:
                                    case ItemType.PUMPSHOTGUN_MK2_CAMO_03:
                                    case ItemType.PUMPSHOTGUN_MK2_CAMO_04:
                                    case ItemType.PUMPSHOTGUN_MK2_CAMO_05:
                                    case ItemType.PUMPSHOTGUN_MK2_CAMO_06:
                                    case ItemType.PUMPSHOTGUN_MK2_CAMO_07:
                                    case ItemType.PUMPSHOTGUN_MK2_CAMO_08:
                                    case ItemType.PUMPSHOTGUN_MK2_CAMO_09:
                                    case ItemType.PUMPSHOTGUN_MK2_CAMO_10:
                                    case ItemType.PUMPSHOTGUN_MK2_CAMO_IND_01:
                                    case ItemType.PUMPSHOTGUN_MK2_CLIP_ARMORPIERCING:
                                    case ItemType.PUMPSHOTGUN_MK2_CLIP_EXPLOSIVE:
                                    case ItemType.PUMPSHOTGUN_MK2_CLIP_HOLLOWPOINT:
                                    case ItemType.PUMPSHOTGUN_MK2_CLIP_INCENDIARY:
                                    case ItemType.PUMPSHOTGUN_VARMOD_LOWRIDER:
                                    case ItemType.RAYPISTOL_VARMOD_XMAS18:
                                    case ItemType.REVOLVER_MK2_CAMO:
                                    case ItemType.REVOLVER_MK2_CAMO_02:
                                    case ItemType.REVOLVER_MK2_CAMO_03:
                                    case ItemType.REVOLVER_MK2_CAMO_04:
                                    case ItemType.REVOLVER_MK2_CAMO_05:
                                    case ItemType.REVOLVER_MK2_CAMO_06:
                                    case ItemType.REVOLVER_MK2_CAMO_07:
                                    case ItemType.REVOLVER_MK2_CAMO_08:
                                    case ItemType.REVOLVER_MK2_CAMO_09:
                                    case ItemType.REVOLVER_MK2_CAMO_10:
                                    case ItemType.REVOLVER_MK2_CAMO_IND_01:
                                    case ItemType.REVOLVER_MK2_CLIP_FMJ:
                                    case ItemType.REVOLVER_MK2_CLIP_HOLLOWPOINT:
                                    case ItemType.REVOLVER_MK2_CLIP_INCENDIARY:
                                    case ItemType.REVOLVER_MK2_CLIP_TRACER:
                                    case ItemType.REVOLVER_VARMOD_BOSS:
                                    case ItemType.REVOLVER_VARMOD_GOON:
                                    case ItemType.SAWNOFFSHOTGUN_VARMOD_LUXE:
                                    case ItemType.SMG_CLIP_02:
                                    case ItemType.SMG_CLIP_03:
                                    case ItemType.SMG_MK2_CAMO:
                                    case ItemType.SMG_MK2_CAMO_02:
                                    case ItemType.SMG_MK2_CAMO_03:
                                    case ItemType.SMG_MK2_CAMO_04:
                                    case ItemType.SMG_MK2_CAMO_05:
                                    case ItemType.SMG_MK2_CAMO_06:
                                    case ItemType.SMG_MK2_CAMO_07:
                                    case ItemType.SMG_MK2_CAMO_08:
                                    case ItemType.SMG_MK2_CAMO_09:
                                    case ItemType.SMG_MK2_CAMO_10:
                                    case ItemType.SMG_MK2_CAMO_IND_01:
                                    case ItemType.SMG_MK2_CLIP_02:
                                    case ItemType.SMG_MK2_CLIP_FMJ:
                                    case ItemType.SMG_MK2_CLIP_HOLLOWPOINT:
                                    case ItemType.SMG_MK2_CLIP_INCENDIARY:
                                    case ItemType.SMG_MK2_CLIP_TRACER:
                                    case ItemType.SMG_VARMOD_LUXE:
                                    case ItemType.SNIPERRIFLE_VARMOD_LUXE:
                                    case ItemType.SNSPISTOL_CLIP_02:
                                    case ItemType.SNSPISTOL_MK2_CAMO:
                                    case ItemType.SNSPISTOL_MK2_CAMO_02:
                                    case ItemType.SNSPISTOL_MK2_CAMO_02_SLIDE:
                                    case ItemType.SNSPISTOL_MK2_CAMO_03:
                                    case ItemType.SNSPISTOL_MK2_CAMO_03_SLIDE:
                                    case ItemType.SNSPISTOL_MK2_CAMO_04:
                                    case ItemType.SNSPISTOL_MK2_CAMO_04_SLIDE:
                                    case ItemType.SNSPISTOL_MK2_CAMO_05:
                                    case ItemType.SNSPISTOL_MK2_CAMO_05_SLIDE:
                                    case ItemType.SNSPISTOL_MK2_CAMO_06:
                                    case ItemType.SNSPISTOL_MK2_CAMO_06_SLIDE:
                                    case ItemType.SNSPISTOL_MK2_CAMO_07:
                                    case ItemType.SNSPISTOL_MK2_CAMO_07_SLIDE:
                                    case ItemType.SNSPISTOL_MK2_CAMO_08:
                                    case ItemType.SNSPISTOL_MK2_CAMO_08_SLIDE:
                                    case ItemType.SNSPISTOL_MK2_CAMO_09:
                                    case ItemType.SNSPISTOL_MK2_CAMO_09_SLIDE:
                                    case ItemType.SNSPISTOL_MK2_CAMO_10:
                                    case ItemType.SNSPISTOL_MK2_CAMO_10_SLIDE:
                                    case ItemType.SNSPISTOL_MK2_CAMO_IND_01:
                                    case ItemType.SNSPISTOL_MK2_CAMO_IND_01_SLIDE:
                                    case ItemType.SNSPISTOL_MK2_CAMO_SLIDE:
                                    case ItemType.SNSPISTOL_MK2_CLIP_02:
                                    case ItemType.SNSPISTOL_MK2_CLIP_FMJ:
                                    case ItemType.SNSPISTOL_MK2_CLIP_HOLLOWPOINT:
                                    case ItemType.SNSPISTOL_MK2_CLIP_INCENDIARY:
                                    case ItemType.SNSPISTOL_MK2_CLIP_TRACER:
                                    case ItemType.SNSPISTOL_VARMOD_LOWRIDER:
                                    case ItemType.SPECIALCARBINE_CLIP_02:
                                    case ItemType.SPECIALCARBINE_CLIP_03:
                                    case ItemType.SPECIALCARBINE_MK2_CAMO:
                                    case ItemType.SPECIALCARBINE_MK2_CAMO_02:
                                    case ItemType.SPECIALCARBINE_MK2_CAMO_03:
                                    case ItemType.SPECIALCARBINE_MK2_CAMO_04:
                                    case ItemType.SPECIALCARBINE_MK2_CAMO_05:
                                    case ItemType.SPECIALCARBINE_MK2_CAMO_06:
                                    case ItemType.SPECIALCARBINE_MK2_CAMO_07:
                                    case ItemType.SPECIALCARBINE_MK2_CAMO_08:
                                    case ItemType.SPECIALCARBINE_MK2_CAMO_09:
                                    case ItemType.SPECIALCARBINE_MK2_CAMO_10:
                                    case ItemType.SPECIALCARBINE_MK2_CAMO_IND_01:
                                    case ItemType.SPECIALCARBINE_MK2_CLIP_02:
                                    case ItemType.SPECIALCARBINE_MK2_CLIP_ARMORPIERCING:
                                    case ItemType.SPECIALCARBINE_MK2_CLIP_FMJ:
                                    case ItemType.SPECIALCARBINE_MK2_CLIP_INCENDIARY:
                                    case ItemType.SPECIALCARBINE_MK2_CLIP_TRACER:
                                    case ItemType.SPECIALCARBINE_VARMOD_LOWRIDER:
                                    case ItemType.SWITCHBLADE_VARMOD_VAR1:
                                    case ItemType.SWITCHBLADE_VARMOD_VAR2:
                                    case ItemType.VINTAGEPISTOL_CLIP_02:
                                        return;
                    #endregion
                        #region craft
                        case ItemType.CraftСollectibleCoin:
                        case ItemType.CraftTinNugget:
                        case ItemType.CraftShell:
                        case ItemType.CraftScrapMetal:
                        case ItemType.CraftRelic:
                        case ItemType.CraftOldJewerly:
                        case ItemType.CraftOldCoin:
                        case ItemType.CraftIronNugget:
                        case ItemType.CraftGoldNugget:
                        case ItemType.CraftGoldHorseShoe:
                        case ItemType.CraftCopperWire:
                        case ItemType.CraftCopperNugget:
                        case ItemType.CraftCap:
                        case ItemType.CraftAncientStatuette:
                    case ItemType.CraftIronPart:
                    case ItemType.CraftCopperPart:
                    case ItemType.CraftTinPart:
                    case ItemType.CraftBronzePart:
                            return;
                    #endregion
                    case ItemType.DrugBookMark:
                        return;
                    #endregion
                    case ItemType.LSPDDrone:
                    case ItemType.Drone:
                        if (Main.Players[player].Fraction.FractionID != 7 && Main.Players[player].Fraction.FractionID != 9)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы не сотрудник!", 3000);
                            return;
                        }
                        if (!Main.Players[player].OnDuty)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы должны начать рабочий день", 3000);
                            return;
                        }

                        Trigger.ClientEvent(player, "client:StartLSPDDrone");
                        break;
                }
                Log.Debug("ARMOR REMOVE?");
                nInventory.Remove(player, item.Type, 1);
                Log.Debug("ARMOR REMOVE?");
                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы использовали {nInventory.ItemsNames[item.ID]}", 3000);
                GameLog.Items($"player({Main.Players[player].UUID})", "use", Convert.ToInt32(item.Type), 1, $"{item.Data}");
                GUI.Dashboard.Close(player);
            }
            catch (Exception e)
            {
                Log.Write($"EXCEPTION AT\"ITEM_USE\"/{item.Type}/{index}/{player.Name}/:\n" + e.ToString(), nLog.Type.Error);
            }
        }

        // TO DELETE
        private static List<int> TypesCounts = new List<int>()
        {
            5, 10, 15, 3000, 5000, 10000
        };
        private static List<Tuple<int, int>> PresentsTypes = new List<Tuple<int, int>>()
        {
            new Tuple<int, int>(0, 5),
            new Tuple<int, int>(1, 4),
            new Tuple<int, int>(2, 3),
            new Tuple<int, int>(5, 0),
            new Tuple<int, int>(4, 1),
            new Tuple<int, int>(3, 2),
        };
        //
        public static void onDrop(Player player, List<bool> slots, nItem item, dynamic data)
        {
            try
            {
                //Log.Debug($"[onDrop] slot_id: {item.slot_id}", nLog.Type.Info);
                if (player.IsInVehicle) return;
                //Log.Debug($"[onDrop] weapon?: {nInventory.WeaponsItems.Contains(item.Type)} isActive: {item.IsActive} fastSlotId: {item.fast_slot_id}", nLog.Type.Info);

                if (item.slot_id > 0) {
                    //Log.Debug("CLEAR!? onDrop");
                    nInventory.ClearSlot(slots, item, 5);
                }

                //А когда это выполнится?
                if (nInventory.WeaponsItems.Contains(item.Type) && !item.IsActive && item.fast_slot_id > 0) {
                    GUI.Dashboard.SyncAttachComp(player, item, true);
                }

                item.slot_id = 0;
                item.fast_slot_id = 0;
                item.character_slot_id = 0;
                var rnd = new Random();
                //if (data != null && (int)data != 1)
                //    Commands.RPChat("me", player, $"выкинул(а) {nInventory.ItemsNames[(int)item.Type]}");

                GameLog.Items($"player({Main.Players[player].UUID})", "ground", Convert.ToInt32(item.Type), 1, $"{item.Data} - {nInventory.ItemsNames[(int)item.Type]}");

                Dictionary<string, object> ItemData = new Dictionary<string, object>()
                {
                    {"id",item.ID },
                    {"content", nInventory.ItemFname[item.Type]},
                    {"weight",item.Weight},
                    {"count",item.Count },
                };
                if (!nInventory.ClothesItems.Contains(item.Type) && !nInventory.WeaponsItems.Contains(item.Type) && item.Type != ItemType.CarKey && item.Type != ItemType.KeyRing)
                {
                    foreach (var o in NAPI.Pools.GetAllObjects())
                    {
                        if (player.Position.DistanceTo(o.Position) > 2) continue;
                        if (!o.HasSharedData("TYPE") || o.GetSharedData<string>("TYPE") != "DROPPED" || !o.HasData("ITEM")) continue;
                        nItem oItem = o.GetData<nItem>("ITEM");
                        if (oItem.Type == item.Type)
                        {
                            oItem.Count += item.Count;
                            o.SetData("ITEM", oItem);
                            o.SetData("WILL_DELETE", DateTime.Now.AddMinutes(2));
                            o.SetSharedData("ItemData", JsonConvert.SerializeObject(ItemData));
                            return;
                        }
                    }
                }
                item.IsActive = false;

                var xrnd = rnd.NextDouble();
                var yrnd = rnd.NextDouble();
                var obj = NAPI.Object.CreateObject(nInventory.ItemModels[item.Type], player.Position + nInventory.ItemsPosOffset[item.Type] + new Vector3(xrnd, yrnd, 0), player.Rotation + nInventory.ItemsRotOffset[item.Type], 255, player.Dimension);
                obj.SetSharedData("TYPE", "DROPPED");
                obj.SetSharedData("PICKEDT", false);
                obj.SetData("ITEM", item);

                obj.SetSharedData("ItemData", JsonConvert.SerializeObject(ItemData));
                var id = rnd.Next(100000, 999999);
                while (ItemsDropped.Contains(id)) id = rnd.Next(100000, 999999);
                obj.SetData("ID", id);

                //obj.SetData("DELETETIMER", Main.StartT(14400000, 99999999, (o) => deleteObject(obj), "ODELETE_TIMER"));
                obj.SetData("DELETETIMER", Timers.StartOnce(14400000, () => deleteObject(obj)));
            }
            catch (Exception e) { Log.Write("onDrop: " + e.StackTrace, nLog.Type.Error); }
        }
        public static void onTransfer(Player player, nItem item, dynamic data)
        {
            //
        }
    }
}
