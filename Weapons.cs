using GTANetworkAPI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.Threading;
using NeptuneEvo.SDK;
using MySqlConnector;

namespace NeptuneEvo.Core
{
    public class WeaponData
    {
        public string Serial { get; set; }
        public int AmmmoInClip { get; set; }
        public Dictionary<int, WeaponComponent> Components = new Dictionary<int, WeaponComponent>()
            {
                {1,new WeaponComponent() },//Clip
                {2,new WeaponComponent() },//Flashlight
                {3,new WeaponComponent() },//Scope
                {4,new WeaponComponent() },//Suppressor
                {5,new WeaponComponent() },//Texture
                {6,new WeaponComponent() },//Grip
            };

        public WeaponData(string serial, int ammoinclip)
        {
            Serial = serial;
            AmmmoInClip = ammoinclip;
        }
    }
    public class WeaponComponent
    {
        public int id { get; set; }
        public string content { get; set; }
        public string type { get; set; }
        public float weight { get; set; }
        public int count { get; set; }
        public int w { get; set; }
        public int h { get; set; }
        public string Сtype { get; set; }
    }
    class Weapons : Script
    {
        private static nLog Log = new nLog("Weapons");
        public enum WeaponHash : uint
        {
            Sniperrifle = 100416529,
            Fireextinguisher = 101631238,
            Compactlauncher = 125959754,
            Snowball = 126349499,
            Vintagepistol = 137902532,
            Combatpdw = 171789620,
            Heavysniper_mk2 = 177293209,
            Heavysniper = 205991906,
            Autoshotgun = 317205821,
            Microsmg = 324215364,
            Wrench = 419712736,
            Pistol = 453432689,
            Pumpshotgun = 487013001,
            Appistol = 584646201,
            Ball = 600439132,
            Molotov = 615608432,
            Ceramicpistol = 727643628,
            Smg = 736523883,
            Stickybomb = 741814745,
            Petrolcan = 883325847,
            Stungun = 911657153,
            Stone_hatchet = 940833800,
            Assaultrifle_mk2 = 961495388,
            Heavyshotgun = 984333226,
            Minigun = 1119849093,
            Golfclub = 1141786504,
            Raycarbine = 1198256469,
            Flaregun = 1198879012,
            Flare = 1233104067,
            Grenadelauncher_smoke = 1305664598,
            Hammer = 1317494643,
            Pumpshotgun_mk2 = 1432025498,
            Combatpistol = 1593441988,
            Gusenberg = 1627465347,
            Compactrifle = 1649403952,
            Hominglauncher = 1672152130,
            Nightstick = 1737195953,
            Marksmanrifle_mk2 = 1785463520,
            Railgun = 1834241177,
            Sawnoffshotgun = 2017895192,
            Smg_mk2 = 2024373456,
            Bullpuprifle = 2132975508,
            Firework = 2138347493,
            Combatmg = 2144741730,
            Carbinerifle = 2210333304,
            Crowbar = 2227010557,
            Bullpuprifle_mk2 = 2228681469,
            Snspistol_mk2 = 2285322324,
            Flashlight = 2343591895,
            Proximine = 2381443905,
            NavyRevolver = 2441047180,
            Dagger = 2460120199,
            Grenade = 2481070269,
            Poolcue = 2484171525,
            Bat = 2508868239,
            Specialcarbine_mk2 = 2526821735,
            Doubleaction = 2548703416,
            Pistol50 = 2578377531,
            Knife = 2578778090,
            Mg = 2634544996,
            Bullpupshotgun = 2640438543,
            Bzgas = 2694266206,
            Unarmed = 2725352035,
            Grenadelauncher = 2726580491,
            Musket = 2828843422,
            Advancedrifle = 2937143193,
            Raypistol = 2939590305,
            Rpg = 2982836145,
            Rayminigun = 3056410471,
            Pipebomb = 3125143736,
            HazardCan = 3126027122,
            Minismg = 3173288789,
            Snspistol = 3218215474,
            Pistol_mk2 = 3219281620,
            Assaultrifle = 3220176749,
            Specialcarbine = 3231910285,
            Revolver = 3249783761,
            Marksmanrifle = 3342088282,
            Revolver_mk2 = 3415619887,
            Battleaxe = 3441901897,
            Heavypistol = 3523564046,
            Knuckle = 3638508604,
            Machinepistol = 3675956304,
            Combatmg_mk2 = 3686625920,
            Marksmanpistol = 3696079510,
            Machete = 3713923289,
            Switchblade = 3756226112,
            Assaultshotgun = 3800352039,
            Dbshotgun = 4019527611,
            Assaultsmg = 4024951519,
            Hatchet = 4191993645,
            Bottle = 4192643659,
            Carbinerifle_mk2 = 4208062921,
            Parachute = 4222310262,
            Smokegrenade = 4256991824,
            Rod = 2384362703,
            RodMK2 = 2384362703,
            RodUpgrade = 2384362703,

            DigScanner = 520317490,
            DigScanner_mk2 = 520317490,
            DigScanner_mk3 = 520317490,

            DigShovel = 1594770590,
            DigShovel_mk2 = 1594770590,
            DigShovel_mk3 = 1594770590
        }
        internal enum Hash : Int32
        {
            /* Handguns */
            Knife = -1716189206,
            Nightstick = 1737195953,
            Hammer = 1317494643,
            Bat = -1786099057,
            Crowbar = -2067956739,
            Golfclub = 1141786504,
            Bottle = -102323637,
            Dagger = -1834847097,
            Hatchet = -102973651,
            Knuckleduster = -656458692,
            Machete = -581044007,
            Flashlight = -1951375401,
            Switchblade = -538741184,
            Poolcue = -1810795771,
            Wrench = 419712736,
            Battleaxe = -853065399,
            /* Pistols */
            Pistol = 453432689,
            Combatpistol = 1593441988,
            Pistol50 = -1716589765,
            Snspistol = -1076751822,
            Heavypistol = -771403250,
            Vintagepistol = 137902532,
            Marksmanpistol = -598887786,
            Revolver = -1045183535,
            Appistol = 584646201,
            Stungun = 911657153,
            Flaregun = 1198879012,
            Doubleaction,
            Pistolmk2,
            Snspistolmk2,
            Revolvermk2,
            /* SMG */
            Microsmg = 324215364,
            Machinepistol = -619010992,
            Smg = 736523883,
            Assaultsmg = -270015777,
            Combatpdw = 171789620,
            Mg = -1660422300,
            Combatmg = 2144741730,
            Gusenberg = 1627465347,
            Minismg = -1121678507,
            Smgmk2,
            Combatmgmk2,
            /* Rifles */
            Assaultrifle = -1074790547,
            Carbinerifle = -2084633992,
            Advancedrifle = -1357824103,
            Specialcarbine = -1063057011,
            Bullpuprifle = 2132975508,
            Compactrifle = 1649403952,
            Assaultriflemk2,
            Carbineriflemk2,
            Specialcarbinemk2,
            Bullpupriflemk2,
            /* Sniper */
            Sniperrifle = 100416529,
            Heavysniper = 205991906,
            Marksmanrifle = -952879014,
            Heavysnipermk2,
            Marksmanriflemk2,
            /* Shotguns */
            Pumpshotgun = 487013001,
            Sawnoffshotgun = 2017895192,
            Bullpupshotgun = -1654528753,
            Assaultshotgun = -494615257,
            Musket = -1466123874,
            Heavyshotgun = 984333226,
            Doublebarrelshotgun = -275439685,
            Sweepershotgun = 317205821,
            Pumpshotgunmk2,
            /* Heavy */
            Grenadelauncher = -1568386805,
            Rpg = -1312131151,
            Minigun = 1119849093,
            Firework = 2138347493,
            Railgun = 1834241177,
            Hominglauncher = 1672152130,
            Grenadelaunchersmoke = 1305664598,
            Compactgrenadelauncher = 125959754,
            /* Throwables & Misc */
            Grenade = -1813897027,
            Stickybomb = 741814745,
            Proximitymine = -1420407917,
            Bzgas = -1600701090,
            Molotov = 615608432,
            Fireextinguisher = 101631238,
            Petrolcan = 883325847,
            Flare = 1233104067,
            Ball = 600439132,
            Snowball = 126349499,
            Smokegrenade = -37975472,
            Pipebomb = -1169823560,
            Parachute = 615608432,
            Rod = -1910604593,
            RodMK2 = -1910604593,
            RodUpgrade = -1910604593,

            DigScanner = 520317490,
            DigScanner_mk2 = 520317490,
            DigScanner_mk3 = 520317490,

            DigShovel = 1594770590,
            DigShovel_mk2 = 1594770590,
            DigShovel_mk3 = 1594770590
        }
        public static WeaponHash GetWeaponHash(string name)
        {
            Log.Write($"GetWeaponHash: {name} {Convert.ToString((WeaponHash)Enum.Parse(typeof(WeaponHash), name))}", nLog.Type.Error);
            return (WeaponHash)Enum.Parse(typeof(WeaponHash), name);
        }
        public static Hash GetHash(string name)
        {

            //Log.Write($"GetHash: {name} {Convert.ToString((Hash)Enum.Parse(typeof(Hash), name))}", nLog.Type.Error);
            return (Hash)Enum.Parse(typeof(Hash), name.Replace("_", ""));
        }

        public static Dictionary<ItemType, ItemType> WeaponsAmmoTypes = new Dictionary<ItemType, ItemType>()
        {
            { ItemType.Pistol, ItemType.PistolAmmo },
            { ItemType.Combatpistol, ItemType.PistolAmmo },
            { ItemType.Pistol50, ItemType.PistolAmmo },
            { ItemType.Snspistol, ItemType.PistolAmmo },
            { ItemType.Heavypistol, ItemType.PistolAmmo },
            { ItemType.Vintagepistol, ItemType.PistolAmmo },
            { ItemType.Marksmanpistol, ItemType.PistolAmmo },
            { ItemType.Revolver, ItemType.PistolAmmo },
            { ItemType.Appistol, ItemType.PistolAmmo },
            { ItemType.Flaregun, ItemType.PistolAmmo },
            { ItemType.Doubleaction, ItemType.PistolAmmo },
            { ItemType.Pistol_mk2, ItemType.PistolAmmo },
            { ItemType.Snspistol_mk2, ItemType.PistolAmmo },
            { ItemType.Revolver_mk2, ItemType.PistolAmmo },

            { ItemType.Microsmg, ItemType.SMGAmmo },
            { ItemType.Machinepistol, ItemType.SMGAmmo },
            { ItemType.Smg, ItemType.SMGAmmo },
            { ItemType.Assaultsmg, ItemType.SMGAmmo },
            { ItemType.Combatpdw, ItemType.SMGAmmo },
            { ItemType.Mg, ItemType.SMGAmmo },
            { ItemType.Combatmg, ItemType.SMGAmmo },
            { ItemType.Gusenberg, ItemType.SMGAmmo },
            { ItemType.Minismg, ItemType.SMGAmmo },
            { ItemType.Smg_mk2, ItemType.SMGAmmo },
            { ItemType.Combatmg_mk2, ItemType.SMGAmmo },

            { ItemType.Assaultrifle, ItemType.RiflesAmmo },
            { ItemType.Carbinerifle, ItemType.RiflesAmmo },
            { ItemType.Advancedrifle, ItemType.RiflesAmmo },
            { ItemType.Specialcarbine, ItemType.RiflesAmmo },
            { ItemType.Bullpuprifle, ItemType.RiflesAmmo },
            { ItemType.Compactrifle, ItemType.RiflesAmmo },
            { ItemType.Assaultrifle_mk2, ItemType.RiflesAmmo },
            { ItemType.Carbinerifle_mk2, ItemType.RiflesAmmo },
            { ItemType.Specialcarbine_mk2, ItemType.RiflesAmmo },
            { ItemType.Bullpuprifle_mk2, ItemType.RiflesAmmo },

            { ItemType.Sniperrifle, ItemType.SniperAmmo },
            { ItemType.Heavysniper, ItemType.SniperAmmo },
            { ItemType.Marksmanrifle, ItemType.SniperAmmo },
            { ItemType.Heavysniper_mk2, ItemType.SniperAmmo },
            { ItemType.Marksmanrifle_mk2, ItemType.SniperAmmo },

            { ItemType.Pumpshotgun, ItemType.ShotgunsAmmo },
            { ItemType.Sawnoffshotgun, ItemType.ShotgunsAmmo },
            { ItemType.Bullpupshotgun, ItemType.ShotgunsAmmo },
            { ItemType.Assaultshotgun, ItemType.ShotgunsAmmo },
            { ItemType.Musket, ItemType.ShotgunsAmmo },
            { ItemType.Heavyshotgun, ItemType.ShotgunsAmmo },
            { ItemType.Dbshotgun, ItemType.ShotgunsAmmo },
            { ItemType.Autoshotgun, ItemType.ShotgunsAmmo },
            { ItemType.Pumpshotgun_mk2, ItemType.ShotgunsAmmo },
            { ItemType.Rpg, ItemType.ShotgunsAmmo },
        };
        public static Dictionary<ItemType, int> WeaponsClipsMax = new Dictionary<ItemType, int>()
        {
            { ItemType.Pistol, 12 },
            { ItemType.Combatpistol, 12 },
            { ItemType.Pistol50, 9 },
            { ItemType.Snspistol, 6 },
            { ItemType.Heavypistol, 18 },
            { ItemType.Vintagepistol, 7 },

            { ItemType.Ceramicpistol, 12 },

            { ItemType.Marksmanpistol, 1 },
            { ItemType.Revolver, 6 },
            { ItemType.Appistol, 18 },
            { ItemType.Stungun, 0 },
            { ItemType.Flaregun, 1 },
            { ItemType.Doubleaction, 6 }, // closed
            { ItemType.Pistol_mk2, 12 }, // closed
            { ItemType.Snspistol_mk2, 6 }, // closed
            { ItemType.Revolver_mk2, 6 }, // closed ? 24 rounds?

            { ItemType.Microsmg, 16 },
            { ItemType.Machinepistol, 12 },
            { ItemType.Smg, 30 },
            { ItemType.Assaultsmg, 30 },
            { ItemType.Combatpdw, 30 },
            { ItemType.Mg, 54 },
            { ItemType.Combatmg, 100 },
            { ItemType.Gusenberg, 30 },
            { ItemType.Minismg, 20 },
            { ItemType.Smg_mk2, 30 }, // closed
            { ItemType.Combatmg_mk2, 100 }, // closed

            { ItemType.Assaultrifle, 30 },
            { ItemType.Carbinerifle, 30 },
            { ItemType.Advancedrifle, 30 },
            { ItemType.Specialcarbine, 30 },
            { ItemType.Bullpuprifle, 30 },
            { ItemType.Compactrifle, 30 },
            { ItemType.Assaultrifle_mk2, 30 }, // closed
            { ItemType.Carbinerifle_mk2, 30 }, // closed
            { ItemType.Specialcarbine_mk2, 30 }, // closed
            { ItemType.Bullpuprifle_mk2, 30 }, // closed

            { ItemType.Sniperrifle, 10 },
            { ItemType.Heavysniper, 6 },
            { ItemType.Marksmanrifle, 8 },
            { ItemType.Heavysniper_mk2, 6 }, // closed
            { ItemType.Marksmanrifle_mk2, 8 }, // closed

            { ItemType.Pumpshotgun, 8 },
            { ItemType.Sawnoffshotgun, 8 },
            { ItemType.Bullpupshotgun, 14 },
            { ItemType.Assaultshotgun, 8 },
            { ItemType.Musket, 1 },
            { ItemType.Heavyshotgun, 6 },
            { ItemType.Dbshotgun, 2 },
            { ItemType.Autoshotgun, 10 },
            { ItemType.Pumpshotgun_mk2, 8 }, // closed
            { ItemType.Rpg, 1 },
        };

        public static Dictionary<ItemType, int> WeaponsComponentsClipsMax = new Dictionary<ItemType, int>()
        {
          { ItemType.PISTOL_CLIP_02, 16 },
          { ItemType.COMBATPISTOL_CLIP_02, 16 },
          { ItemType.APPISTOL_CLIP_02, 36 },
          { ItemType.PISTOL50_CLIP_02, 12 },
          { ItemType.SNSPISTOL_CLIP_02, 12 },
          { ItemType.HEAVYPISTOL_CLIP_02, 36 },
          { ItemType.REVOLVER_MK2_CLIP_TRACER, 6 },
          { ItemType.REVOLVER_MK2_CLIP_INCENDIARY, 6 },
          { ItemType.REVOLVER_MK2_CLIP_HOLLOWPOINT, 6 },
          { ItemType.REVOLVER_MK2_CLIP_FMJ, 6 },
          { ItemType.SNSPISTOL_MK2_CLIP_02, 12 },
          { ItemType.SNSPISTOL_MK2_CLIP_TRACER, 6 },
          { ItemType.SNSPISTOL_MK2_CLIP_INCENDIARY, 6 },
          { ItemType.SNSPISTOL_MK2_CLIP_HOLLOWPOINT, 6 },
          { ItemType.SNSPISTOL_MK2_CLIP_FMJ, 6 },
          { ItemType.PISTOL_MK2_CLIP_02, 16 },
          { ItemType.PISTOL_MK2_CLIP_TRACER, 12 },
          { ItemType.PISTOL_MK2_CLIP_INCENDIARY, 8 },
          { ItemType.PISTOL_MK2_CLIP_HOLLOWPOINT, 8 },
          { ItemType.PISTOL_MK2_CLIP_FMJ, 8 },
          { ItemType.VINTAGEPISTOL_CLIP_02, 14 },
          { ItemType.CERAMICPISTOL_CLIP_02, 17 },
          { ItemType.MICROSMG_CLIP_02, 30 },
          { ItemType.SMG_CLIP_02, 60 },
          { ItemType.SMG_CLIP_03, 100 },
          { ItemType.ASSAULTSMG_CLIP_02, 60 },
          { ItemType.MINISMG_CLIP_02, 30 },
          { ItemType.SMG_MK2_CLIP_02, 60 },
          { ItemType.SMG_MK2_CLIP_TRACER, 30 },
          { ItemType.SMG_MK2_CLIP_INCENDIARY, 20 },
          { ItemType.SMG_MK2_CLIP_HOLLOWPOINT, 20 },
          { ItemType.SMG_MK2_CLIP_FMJ, 20 },
          { ItemType.MACHINEPISTOL_CLIP_02, 20 },
          { ItemType.MACHINEPISTOL_CLIP_03, 30 },
          { ItemType.COMBATPDW_CLIP_02, 60 },
          { ItemType.COMBATPDW_CLIP_03, 100 },
          { ItemType.ASSAULTSHOTGUN_CLIP_02, 32 },
          { ItemType.PUMPSHOTGUN_MK2_CLIP_INCENDIARY, 8 }, // ?
          { ItemType.PUMPSHOTGUN_MK2_CLIP_ARMORPIERCING, 8 }, // ?
          { ItemType.PUMPSHOTGUN_MK2_CLIP_HOLLOWPOINT, 8 }, // ?
          { ItemType.PUMPSHOTGUN_MK2_CLIP_EXPLOSIVE, 8 }, // ?
          { ItemType.HEAVYSHOTGUN_CLIP_02, 12 },
          { ItemType.HEAVYSHOTGUN_CLIP_03, 30 },
          { ItemType.ASSAULTRIFLE_CLIP_02, 30 },
          { ItemType.ASSAULTRIFLE_CLIP_03, 60 },
          { ItemType.CARBINERIFLE_CLIP_02, 60 },
          { ItemType.CARBINERIFLE_CLIP_03, 100 },
          { ItemType.ADVANCEDRIFLE_CLIP_02, 60 },
          { ItemType.SPECIALCARBINE_CLIP_02, 60 },
          { ItemType.SPECIALCARBINE_CLIP_03, 100 },
          { ItemType.BULLPUPRIFLE_CLIP_02, 60 },
          { ItemType.BULLPUPRIFLE_MK2_CLIP_02, 60 },
          { ItemType.BULLPUPRIFLE_MK2_CLIP_TRACER, 30 },
          { ItemType.BULLPUPRIFLE_MK2_CLIP_INCENDIARY, 20 },
          { ItemType.BULLPUPRIFLE_MK2_CLIP_ARMORPIERCING, 20 },
          { ItemType.BULLPUPRIFLE_MK2_CLIP_FMJ, 20 },
          { ItemType.SPECIALCARBINE_MK2_CLIP_02, 60 },
          { ItemType.SPECIALCARBINE_MK2_CLIP_TRACER, 30 },
          { ItemType.SPECIALCARBINE_MK2_CLIP_INCENDIARY, 20 },
          { ItemType.SPECIALCARBINE_MK2_CLIP_ARMORPIERCING, 20 },
          { ItemType.SPECIALCARBINE_MK2_CLIP_FMJ, 20 },
          { ItemType.ASSAULTRIFLE_MK2_CLIP_02, 60 },
          { ItemType.ASSAULTRIFLE_MK2_CLIP_TRACER, 30 },
          { ItemType.ASSAULTRIFLE_MK2_CLIP_INCENDIARY, 20 },
          { ItemType.ASSAULTRIFLE_MK2_CLIP_ARMORPIERCING, 20 },
          { ItemType.ASSAULTRIFLE_MK2_CLIP_FMJ, 20 },
          { ItemType.CARBINERIFLE_MK2_CLIP_02, 60 },
          { ItemType.CARBINERIFLE_MK2_CLIP_TRACER, 30 },
          { ItemType.CARBINERIFLE_MK2_CLIP_INCENDIARY, 20 },
          { ItemType.CARBINERIFLE_MK2_CLIP_ARMORPIERCING, 20 },
          { ItemType.CARBINERIFLE_MK2_CLIP_FMJ, 20 },
          { ItemType.COMPACTRIFLE_CLIP_02, 60 },
          { ItemType.COMPACTRIFLE_CLIP_03, 100 },
          { ItemType.MILITARYRIFLE_CLIP_02, 45 },
          { ItemType.MG_CLIP_02, 100 },
          { ItemType.COMBATMG_CLIP_02, 200 },
          { ItemType.COMBATMG_MK2_CLIP_02, 200 },
          { ItemType.COMBATMG_MK2_CLIP_TRACER, 100 },
          { ItemType.COMBATMG_MK2_CLIP_INCENDIARY, 80 },
          { ItemType.COMBATMG_MK2_CLIP_ARMORPIERCING, 80 },
          { ItemType.COMBATMG_MK2_CLIP_FMJ, 80 },
          { ItemType.GUSENBERG_CLIP_02, 50 },
          { ItemType.MARKSMANRIFLE_MK2_CLIP_02, 16 },
          { ItemType.MARKSMANRIFLE_MK2_CLIP_TRACER, 8 },
          { ItemType.MARKSMANRIFLE_MK2_CLIP_INCENDIARY, 5 },
          { ItemType.MARKSMANRIFLE_MK2_CLIP_ARMORPIERCING, 5 },
          { ItemType.MARKSMANRIFLE_MK2_CLIP_FMJ, 5 },
          { ItemType.HEAVYSNIPER_MK2_CLIP_02, 8 },
          { ItemType.HEAVYSNIPER_MK2_CLIP_INCENDIARY, 4 },
          { ItemType.HEAVYSNIPER_MK2_CLIP_ARMORPIERCING, 4 },
          { ItemType.HEAVYSNIPER_MK2_CLIP_FMJ, 4 },
          { ItemType.HEAVYSNIPER_MK2_CLIP_EXPLOSIVE, 4 },
          { ItemType.MARKSMANRIFLE_CLIP_02, 16 },
        };

        public static Dictionary<int, int> FractionsLastSerial = new Dictionary<int, int>()
        {
            { 1, 0 },
            { 2, 0 },
            { 3, 0 },
            { 4, 0 },
            { 5, 0 },
            { 6, 0 },
            { 7, 0 },
            { 8, 0 },
            { 9, 0 },
            { 10, 0 },
            { 11, 0 },
            { 12, 0 },
            { 13, 0 },
            { 14, 0 },
        };
        public static Dictionary<int, int> BusinessesLastSerial = new Dictionary<int, int>();

        [ServerEvent(Event.ResourceStart)]
        public void Event_ResourceStart()
        {
            try
            {
                var result = MySQL.QueryRead("SELECT * FROM `weapons`");
                if (result == null || result.Rows.Count == 0)
                {
                    Log.Write("Table 'weapons' returns null result", nLog.Type.Warn);
                    return;
                }
                foreach (DataRow Row in result.Rows)
                {
                    var id = Convert.ToInt32(Row["id"]);
                    var lastserial = Convert.ToInt32(Row["lastserial"]);

                    BusinessesLastSerial.Add(id, lastserial);
                }
            }
            catch (Exception e) { Log.Write("ResourceStart: " + e.StackTrace, nLog.Type.Error); }
        }

        public static void Event_PlayerDeath(Player player, Player killer, uint reason)
        {
            try
            {
                if (!Main.Players.ContainsKey(player)) return;
                var UUID = Main.Players[player].UUID;
                if (!player.HasMyData("LastActiveWeap")) return;
                var lastWeapon = player.GetMyData<nItem>("LastActiveWeap");
                var wType = lastWeapon.Type;

                if (wType == ItemType.Rod || wType == ItemType.RodMK2 || wType == ItemType.RodUpgrade) {
                    lastWeapon.IsActive = false;
                    return; // блокировка выпадения удочек
                }

                if (lastWeapon != null)
                {
                    // Найти именно это оружие в инвентаре, если его нету то return;
                    var weaponItem = nInventory.Items[Main.Players[player].UUID].FirstOrDefault(i => i == lastWeapon && i.IsActive);
                    if (weaponItem == null) return;

                    var dropItem = new nItem(lastWeapon.Type, 1, lastWeapon.Data, false, lastWeapon.WData);
                    dropItem.slot_id = lastWeapon.slot_id;
                    Items.onDrop(player, nInventory.ItemsSlots[UUID], dropItem, 1);
                    nInventory.Remove(player, lastWeapon);
                    Trigger.ClientEvent(player, "removeAllWeapons");
                    Trigger.ClientEvent(player, "CLIENT::HUD:weaponShow", false, null);
                    NAPI.Task.Run(() => { try { player.RemoveAllWeapons(); } catch (Exception e){ Log.Write(e.StackTrace, nLog.Type.Error); } }, 100);
                }

                player.ResetMyData("LastActiveWeap");
            }
            catch (Exception e) { Log.Write("PlayerDeath: " + e.StackTrace, nLog.Type.Error); }
        }

        public static void Event_OnPlayerDisconnected(Player player)
        {
            if (!Main.Players.ContainsKey(player)) return;
            var weaponItem = nInventory.Items[Main.Players[player].UUID].FirstOrDefault(i => (nInventory.WeaponsItems.Contains(i.Type) || nInventory.MeleeWeaponsItems.Contains(i.Type)) && i.IsActive);
            if (weaponItem == null) return;
            int id = nInventory.FindIndex(Main.Players[player].UUID, weaponItem.Type);
            nInventory.Items[Main.Players[player].UUID][id].IsActive = false;
        }

        public static string GetSerial(bool isFraction, int id)
        {
            if (isFraction)
            {
                var fractionType = Fractions.Manager.FractionTypes[id];
                if (fractionType == 0 || fractionType == 1)
                {
                    return $"{1000 + id}xxxxx";
                }
                else
                {
                    var serial = 100000000 + id * 100000 + FractionsLastSerial[id];
                    FractionsLastSerial[id]++;

                    if (FractionsLastSerial[id] >= 99999)
                        FractionsLastSerial[id] = 0;

                    return serial.ToString();
                }
            }
            else
            {
                var serial = 200000000 + id * 100000 + BusinessesLastSerial[id];
                BusinessesLastSerial[id]++;

                if (BusinessesLastSerial[id] >= 99999)
                    BusinessesLastSerial[id] = 0;

                return serial.ToString();
            }
        }

        public static bool GiveWeapon(Player player, ItemType type, string serial)
        {
            /* var tryAdd = nInventory.TryAdd(player, new nItem(type));
             if (tryAdd == -1) return false;*/
            nInventory.Add(player, new nItem(type, 1, null, false, new WeaponData(serial, 0)));
            return true;
        }

        public static void RemoveAll(Player player, bool ammo)
        {
            if (!Main.Players.ContainsKey(player)) return;
            int UUID = Main.Players[player].UUID;
            for (int i = nInventory.Items[UUID].Count - 1; i >= 0; i--)
            {
                if (i >= nInventory.Items[UUID].Count) continue;
                if (nInventory.WeaponsItems.Contains(nInventory.Items[UUID][i].Type) || nInventory.Items[UUID][i].Type == ItemType.Stungun || (nInventory.Items[UUID][i].Type == ItemType.Mask && !nInventory.Items[UUID][i].IsActive) ||
                    nInventory.MeleeWeaponsItems.Contains(nInventory.Items[UUID][i].Type) || (ammo && nInventory.AmmoItems.Contains(nInventory.Items[UUID][i].Type)))
                    nInventory.Items[UUID].RemoveAt(i);
            }
            Trigger.ClientEvent(player, "removeAllWeapons");
            NAPI.Task.Run(() => { try { player.RemoveAllWeapons(); } catch (Exception e){ Log.Write(e.StackTrace, nLog.Type.Error); } }, 100);
            GUI.Dashboard.sendItems(player);
        }

        public static void SaveWeaponsDB()
        {
            foreach (var dict in BusinessesLastSerial)
            {
                //MySQL.Query($"UPDATE `weapons` SET `lastserial`={dict.Value} WHERE `id`={dict.Key}");

                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = "UPDATE `weapons` SET `lastserial`=@lastserial WHERE `id`=@id";

                cmd.Parameters.AddWithValue("@lastserial", dict.Value);
                cmd.Parameters.AddWithValue("@id", dict.Key);
                MySQL.Query(cmd);
            }
        }

        [RemoteEvent("SERVER::weapon:playerReload")]
        public static void RemoteEvent_playerReload(Player player, int hash/*, int ammoInClip*/)
        {
            try
            {
                /*  if (!Main.Players.ContainsKey(player) || !nInventory.Items.ContainsKey(Main.Players[player].UUID)) return;
                  Hash wHash = (Hash)hash;
                  var wName = wHash.ToString();
                  var wItemType = (ItemType)Enum.Parse(typeof(ItemType), wName);
                  if (!WeaponsAmmoTypes.ContainsKey(wItemType)) return;
                  if (ammoInClip == WeaponsClipsMax[wItemType]) return;

                  var wAmmoType = WeaponsAmmoTypes[wItemType];
                  var ammoItem = nInventory.Find(Main.Players[player].UUID, wAmmoType);
                  var ammoLefts = (ammoItem == null) ? 0 : ammoItem.Count;
                  if (ammoLefts == 0) return;

                  if (ammoInClip > WeaponsClipsMax[wItemType]) ammoInClip = WeaponsClipsMax[wItemType];
                  var ammo = (ammoLefts < WeaponsClipsMax[wItemType] - ammoInClip) ? ammoLefts : WeaponsClipsMax[wItemType] - ammoInClip;
                  nInventory.Remove(player, wAmmoType, ammo);
                  Trigger.ClientEvent(player, "wgive", hash, ammo, true, true);*/


                if (!Main.Players.ContainsKey(player) || !nInventory.Items.ContainsKey(Main.Players[player].UUID)) return;

                GTANetworkAPI.WeaponHash wHash = (GTANetworkAPI.WeaponHash)hash;

                var wName = wHash.ToString();
                var wItemType = (ItemType)Enum.Parse(typeof(ItemType), wName);
                var ammoInClip = NAPI.Player.GetPlayerWeaponAmmo(player, wHash);

                if (!WeaponsAmmoTypes.ContainsKey(wItemType)) return;

                if (player.HasMyData("NEWGUNGAME") && player.GetMyData<uint>("NEWGUNGAME") != 0)
                {
                    if (player.HasMyData("NEWGUNGAME_WEAPON") && player.GetMyData<int>("NEWGUNGAME_WEAPON") != -1 && player.HasMyData("NEWGUNGAME_WEAPONAMMO"))
                    {
                        var maxclip = WeaponsClipsMax[wItemType];
                        var gungamecurrentWeapon = player.GetMyData<int>("NEWGUNGAME_WEAPON");
                        var gungamecurrentAmmo = player.GetMyData<int>("NEWGUNGAME_WEAPONAMMO");

                        if (gungamecurrentAmmo == 0)
                        {
                            Trigger.ClientEvent(player, "CLIENT::HUD:weaponUpdate", ammoInClip, 0);
                            return;
                        }

                        if (ammoInClip > maxclip) ammoInClip = maxclip;
                        var ammo = (gungamecurrentAmmo < maxclip - ammoInClip) ? gungamecurrentAmmo : maxclip - ammoInClip;
                        var setAmmo = ammo + ammoInClip;
                        var newCurrentAmmo = gungamecurrentAmmo - ammo;

                        Log.Debug($"AmmoInClip: {ammoInClip} setAmmo: {setAmmo} newCurrentAmmo: {newCurrentAmmo}");

                        NAPI.Player.SetPlayerWeaponAmmo(player, wHash, 0);
                        NAPI.Player.GivePlayerWeapon(player, (GTANetworkAPI.WeaponHash)wHash, setAmmo);
                        player.SetSharedData("currentAmmo", setAmmo);
                        player.SetMyData<int>("NEWGUNGAME_WEAPONAMMO", newCurrentAmmo);

                        Trigger.ClientEvent(player, "CLIENT::HUD:weaponUpdate", setAmmo, newCurrentAmmo);
                        NAPI.Native.SendNativeToPlayer(player, 0x20AE33F3AC9C0033, player.Handle);
                    }
                } else
                {
                    var wItem = nInventory.Items[Main.Players[player].UUID].FirstOrDefault(i => i.Type == wItemType && i.IsActive);
                    if (wItem == null) return;

                    var maxclip = WeaponsClipsMax[wItemType];
                    if (wItem.WData.Components[1].id != 0)
                    {
                        if (WeaponsComponentsClipsMax.ContainsKey((ItemType)wItem.WData.Components[1].id))
                        {
                            maxclip = WeaponsComponentsClipsMax[(ItemType)wItem.WData.Components[1].id];
                        }
                    }

                    if (ammoInClip == maxclip) return;

                    var wAmmoType = WeaponsAmmoTypes[wItemType];
                    var ammoItem = nInventory.Find(Main.Players[player].UUID, wAmmoType);
                    var ammoLefts = (ammoItem == null) ? 0 : ammoItem.Count;
                    if (ammoLefts == 0)
                    {
                        Trigger.ClientEvent(player, "CLIENT::HUD:weaponUpdate", ammoInClip, 0);
                        return;
                    }

                    if (ammoInClip > maxclip) ammoInClip = maxclip;
                    var ammo = (ammoLefts < maxclip - ammoInClip) ? ammoLefts : maxclip - ammoInClip;
                    nInventory.Remove(player, wAmmoType, ammo);
                    wItem.Weight += nInventory.ItemsWeight[wAmmoType] * ammo;
                    var setAmmo = ammo + ammoInClip;
                    //  Trigger.ClientEvent(player, "wgive", hash, ammo, true, true);

                    Log.Debug($"AmmoInClip: {wItem.WData.AmmmoInClip} setAmmo: {setAmmo}");
                    wItem.WData.AmmmoInClip = setAmmo;
                    NAPI.Player.SetPlayerWeaponAmmo(player, wHash, 0);
                    NAPI.Player.GivePlayerWeapon(player, (GTANetworkAPI.WeaponHash)wHash, setAmmo);
                    player.SetSharedData("currentAmmo", setAmmo);

                    var countAmmo = nInventory.FindCount(player, wAmmoType);
                    Trigger.ClientEvent(player, "CLIENT::HUD:weaponUpdate", setAmmo, countAmmo);

                    NAPI.Native.SendNativeToPlayer(player, 0x20AE33F3AC9C0033, player.Handle);
                    GUI.Dashboard.sendItems(player);
                }

                
            }
            catch (Exception e) { Log.Write("PlayeReloadWeapon: " + e.StackTrace, nLog.Type.Error); }
        }
        //REQUIRED
        [RemoteEvent("staticAttachments.Add")]
        private void OnStaticAttachmentAdd(Player client, string hash)
        {
            client.AddAttachment(Base36Extensions.FromBase36(hash), false);
        }

        //REQUIRED
        [RemoteEvent("staticAttachments.Remove")]
        private void OnStaticAttachmentRemove(Player client, string hash)
        {
            client.AddAttachment(Base36Extensions.FromBase36(hash), true);
        }

        [RemoteEvent("SetComponentFix")]
        public static void SetComponentFix(Player player)
        {
            try
            {
                if (!Main.Players.ContainsKey(player) || !nInventory.Items.ContainsKey(Main.Players[player].UUID)) return;
                GTANetworkAPI.WeaponHash wHash = (GTANetworkAPI.WeaponHash)GetWeaponHash(Main.Players[player].currentWeapon);

                var wName = wHash.ToString();
                if (wName == "Unarmed") return;

                Log.Debug("wName: "+ wName);

                var wItemType = (ItemType)Enum.Parse(typeof(ItemType), wName, true);

                var wItem = nInventory.Items[Main.Players[player].UUID].FirstOrDefault(i => i.Type == wItemType && i.IsActive);
                if (wItem == null) return;
                List<string> tempComponents = new List<string>();
                foreach (var ComponentType in wItem.WData.Components.Values)
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
            catch (Exception e) { Log.Write("SetComponentFix: " + e.StackTrace, nLog.Type.Error); }
        }
        [RemoteEvent("checkAmmo")]
        public static void checkAmmo(Player player, int hash, int ammoInClip)
        {
            try
            {
                if (!Main.Players.ContainsKey(player) || !nInventory.Items.ContainsKey(Main.Players[player].UUID)) return;
                GTANetworkAPI.WeaponHash wHash = (GTANetworkAPI.WeaponHash)GetWeaponHash(Main.Players[player].currentWeapon);
                if (ammoInClip <= 1 /*&& hash == -1569615261*/)
                {
                    // NAPI.Player.GivePlayerWeapon(player, wHash, 0);
                    NAPI.Player.SetPlayerWeaponAmmo(player, wHash, 0);
                    Log.Write("SetPlayerWeaponAmmo: wHash: " + wHash, nLog.Type.Error);
                    ammoInClip = 0;
                }
                var wName = wHash.ToString();
                var wItemType = (ItemType)Enum.Parse(typeof(ItemType), wName);

                var wItem = nInventory.Items[Main.Players[player].UUID].FirstOrDefault(i => i.Type == wItemType && i.IsActive);
                if (wItem == null) return;
                wItem.Weight = nInventory.ItemsWeight[wItemType] + nInventory.ItemsWeight[WeaponsAmmoTypes[wItemType]] * ammoInClip;
                if (!WeaponsAmmoTypes.ContainsKey(wItemType)) return;
                wItem.WData.AmmmoInClip = ammoInClip;
                player.SetSharedData("currentAmmo", ammoInClip);

                var countAmmo = nInventory.FindCount(player, Weapons.WeaponsAmmoTypes[wItemType]);
                Trigger.ClientEvent(player, "CLIENT::HUD:weaponUpdate", ammoInClip, countAmmo);

            }
            catch (Exception e) { Log.Write("PlayeShot_checkAmmo: " + e.StackTrace, nLog.Type.Error); }
        }
        public static void PlayerTakeOffWeapon(Player player, nItem item) // вызывается, если игрок убрал оружие самостоятельно
        {
            try
            {
                if (!Main.Players.ContainsKey(player) || !nInventory.Items.ContainsKey(Main.Players[player].UUID)) return;
                var wHash = GetWeaponHash(item.Type.ToString());
                var ammoInClip = NAPI.Player.GetPlayerWeaponAmmo(player, (GTANetworkAPI.WeaponHash)wHash);
                NAPI.Player.SetPlayerWeaponAmmo(player, (GTANetworkAPI.WeaponHash)wHash, 0);
                NAPI.Player.RemovePlayerWeapon(player, (GTANetworkAPI.WeaponHash)wHash);
                Log.Write("PlayerTakeOffWeapon RemovePlayerWeapon -> " + wHash, nLog.Type.Error);
                //var currentweapon = NAPI.Player.GetPlayerCurrentWeapon(player);
                //Log.Write("PlayerTakeOffWeapon currentweapon -> " + currentweapon, nLog.Type.Error);
                item.IsActive = false;

                var maxclip = WeaponsClipsMax[item.Type];

                if (item.WData.Components[1].id != 0)
                {
                    if (WeaponsComponentsClipsMax.ContainsKey((ItemType)item.WData.Components[1].id))
                    {
                        maxclip = WeaponsComponentsClipsMax[(ItemType)item.WData.Components[1].id];
                    }
                }

                if (!WeaponsAmmoTypes.ContainsKey(item.Type)) return;
                if (ammoInClip > maxclip) ammoInClip = maxclip;

                item.WData.AmmmoInClip = ammoInClip;
                player.SetSharedData("currentAmmo", 0);

                Trigger.ClientEvent(player, "CLIENT::HUD:weaponUpdate", 0, 0);
                GUI.Dashboard.sendItems(player);
                /*  var aType = WeaponsAmmoTypes[item.Type];

                  var tryAdd = nInventory.TryAdd(player, new nItem(aType, ammoInClip));
                  if (tryAdd == -1) tryAdd = ammoInClip;

                  if (ammoInClip > WeaponsClipsMax[item.Type]) ammoInClip = WeaponsClipsMax[item.Type];
                  if (ammoInClip - tryAdd > 0)
                      nInventory.Add(player, new nItem(aType, ammoInClip - tryAdd));

                  if (tryAdd > 0)
                  {
                      if (tryAdd > WeaponsClipsMax[item.Type]) tryAdd = 1;
                      Items.onDrop(player, new nItem(aType, tryAdd), 1);
                  }*/
            }
            catch (Exception e) { Log.Write("PlayeTakeoffWeapon: " + e.StackTrace, nLog.Type.Error); }
        }

        [RemoteEvent("changeweap")]
        public static void RemoteEvent_changeWeapon(Player player, int key)
        {
            try
            {
                if (!Main.Players.ContainsKey(player) || !nInventory.Items.ContainsKey(Main.Players[player].UUID)) return;
                var UUID = Main.Players[player].UUID;
                switch (key)
                {
                    case 1:
                        {
                            var wItem = nInventory.Items[UUID].FirstOrDefault(i => nInventory.WeaponsItems.Contains(i.Type) && WeaponsAmmoTypes[i.Type] == ItemType.PistolAmmo);
                            if (wItem != null) Items.onUse(player, wItem, -1);
                        }
                        return;
                    case 2:
                        {
                            var wItem = nInventory.Items[UUID].FirstOrDefault(i => nInventory.WeaponsItems.Contains(i.Type) && WeaponsAmmoTypes[i.Type] == ItemType.SMGAmmo);
                            if (wItem != null) Items.onUse(player, wItem, -1);
                        }
                        return;
                    case 3:
                        {
                            var wItem = nInventory.Items[UUID].FirstOrDefault(i => i.Type == ItemType.Stungun);
                            if (wItem != null) Items.onUse(player, wItem, -1);
                        }
                        return;
                }
            }
            catch (Exception e) { Log.Write("changeweap: " + e.StackTrace, nLog.Type.Error); }
        }
    }
    class ArmedBody : Script
    {
        static string[] WeaponKeys = { "WEAPON_OBJ_PISTOL", "WEAPON_OBJ_SMG", "WEAPON_OBJ_BACK_RIGHT", "WEAPON_OBJ_BACK_LEFT" };

        private static nLog Log = new nLog("Armedbody");

        public enum WeaponAttachmentType
        {
            RightLeg = 0,
            LeftLeg,
            RightBack,
            LeftBack
        }
        internal class WeaponAttachmentInfo
        {
            public string Model;
            public WeaponAttachmentType Type;

            public WeaponAttachmentInfo(string model, WeaponAttachmentType type)
            {
                Model = model;
                Type = type;
            }
        }
        static Dictionary<WeaponHash, WeaponAttachmentInfo> WeaponData = new Dictionary<WeaponHash, WeaponAttachmentInfo>
        {
            // pistols
            { WeaponHash.Pistol, new WeaponAttachmentInfo("w_pi_pistol", WeaponAttachmentType.RightLeg) },
            { WeaponHash.Combatpistol, new WeaponAttachmentInfo("w_pi_combatpistol", WeaponAttachmentType.RightLeg) },
            { WeaponHash.Pistol50, new WeaponAttachmentInfo("w_pi_pistol50", WeaponAttachmentType.RightLeg) },
            { WeaponHash.Snspistol, new WeaponAttachmentInfo("w_pi_sns_pistol", WeaponAttachmentType.RightLeg) },
            { WeaponHash.Heavypistol, new WeaponAttachmentInfo("w_pi_heavypistol", WeaponAttachmentType.RightLeg) },
            { WeaponHash.Vintagepistol, new WeaponAttachmentInfo("w_pi_vintage_pistol", WeaponAttachmentType.RightLeg) },
            { WeaponHash.Marksmanpistol, new WeaponAttachmentInfo("w_pi_singleshot", WeaponAttachmentType.RightLeg) },
            { WeaponHash.Revolver, new WeaponAttachmentInfo("w_pi_revolver", WeaponAttachmentType.RightLeg) },
            { WeaponHash.Appistol, new WeaponAttachmentInfo("w_pi_appistol", WeaponAttachmentType.RightLeg) },
            { WeaponHash.Stungun, new WeaponAttachmentInfo("w_pi_stungun", WeaponAttachmentType.RightLeg) },
            { WeaponHash.Flaregun, new WeaponAttachmentInfo("w_pi_flaregun", WeaponAttachmentType.RightLeg) },

            // smgs
            { WeaponHash.Microsmg, new WeaponAttachmentInfo("w_sb_microsmg", WeaponAttachmentType.LeftLeg) },
            { WeaponHash.Machinepistol, new WeaponAttachmentInfo("w_sb_compactsmg", WeaponAttachmentType.LeftLeg) },
            { WeaponHash.Minismg, new WeaponAttachmentInfo("w_sb_minismg", WeaponAttachmentType.LeftLeg) },

            // big smgs
            { WeaponHash.Smg, new WeaponAttachmentInfo("w_sb_smg", WeaponAttachmentType.RightBack) },
            { WeaponHash.Assaultsmg, new WeaponAttachmentInfo("w_sb_assaultsmg", WeaponAttachmentType.RightBack) },
            { WeaponHash.Combatpdw, new WeaponAttachmentInfo("w_sb_pdw", WeaponAttachmentType.RightBack) },
            { WeaponHash.Gusenberg, new WeaponAttachmentInfo("w_sb_gusenberg", WeaponAttachmentType.RightBack) },

            // shotguns
            { WeaponHash.Pumpshotgun, new WeaponAttachmentInfo("w_sg_pumpshotgun", WeaponAttachmentType.LeftBack) },
            //{ WeaponHash.SawnoffShotgun, new WeaponAttachmentInfo("w_sg_sawnoff", WeaponAttachmentType.LeftBack) },
            { WeaponHash.Bullpupshotgun, new WeaponAttachmentInfo("w_sg_bullpupshotgun", WeaponAttachmentType.LeftBack) },
            { WeaponHash.Assaultshotgun, new WeaponAttachmentInfo("w_sg_assaultshotgun", WeaponAttachmentType.LeftBack) },
            { WeaponHash.Heavyshotgun, new WeaponAttachmentInfo("w_sg_heavyshotgun", WeaponAttachmentType.LeftBack) },
            { WeaponHash.Doubleaction, new WeaponAttachmentInfo("w_sg_doublebarrel", WeaponAttachmentType.LeftBack) },

            // assault rifles
            { WeaponHash.Assaultrifle, new WeaponAttachmentInfo("w_ar_assaultrifle", WeaponAttachmentType.RightBack) },
            { WeaponHash.Carbinerifle, new WeaponAttachmentInfo("w_ar_carbinerifle", WeaponAttachmentType.RightBack) },
            { WeaponHash.Advancedrifle, new WeaponAttachmentInfo("w_ar_advancedrifle", WeaponAttachmentType.RightBack) },
            { WeaponHash.Specialcarbine, new WeaponAttachmentInfo("w_ar_specialcarbine", WeaponAttachmentType.RightBack) },
            { WeaponHash.Bullpuprifle, new WeaponAttachmentInfo("w_ar_bullpuprifle", WeaponAttachmentType.RightBack) },
            { WeaponHash.Compactrifle, new WeaponAttachmentInfo("w_ar_assaultrifle_smg", WeaponAttachmentType.RightBack) },

            // sniper rifles
            { WeaponHash.Marksmanrifle, new WeaponAttachmentInfo("w_sr_marksmanrifle", WeaponAttachmentType.RightBack) },
            { WeaponHash.Sniperrifle, new WeaponAttachmentInfo("w_sr_sniperrifle", WeaponAttachmentType.RightBack) },
            { WeaponHash.Heavysniper, new WeaponAttachmentInfo("w_sr_heavysniper", WeaponAttachmentType.RightBack) },

            // lmgs
            { WeaponHash.Mg, new WeaponAttachmentInfo("w_mg_mg", WeaponAttachmentType.LeftBack) },
            { WeaponHash.Combatmg, new WeaponAttachmentInfo("w_mg_combatmg", WeaponAttachmentType.LeftBack) }
        };

        #region Methods
        public static void CreateWeaponProp(Player player, WeaponHash weapon)
        {
            if (!WeaponData.ContainsKey(weapon)) return;
            RemoveWeaponProp(player, WeaponData[weapon].Type);

            // make sure player has the weapon
            if (Array.IndexOf(player.Weapons, weapon) == -1) return;

            Vector3 offset = new Vector3(0.0, 0.0, 0.0);
            Vector3 rotation = new Vector3(0.0, 0.0, 0.0);

            switch (WeaponData[weapon].Type)
            {
                case WeaponAttachmentType.RightLeg:
                    offset = new Vector3(0.02, 0.06, 0.1);
                    rotation = new Vector3(-100.0, 0.0, 0.0);
                    break;

                case WeaponAttachmentType.LeftLeg:
                    offset = new Vector3(0.08, 0.03, -0.1);
                    rotation = new Vector3(-80.77, 0.0, 0.0);
                    break;

                case WeaponAttachmentType.RightBack:
                    offset = new Vector3(-0.1, -0.15, -0.13);
                    rotation = new Vector3(0.0, 0.0, 3.5);
                    break;

                case WeaponAttachmentType.LeftBack:
                    offset = new Vector3(-0.1, -0.15, 0.11);
                    rotation = new Vector3(-180.0, 0.0, 0.0);
                    break;
            }

            GTANetworkAPI.Object temp_handle = NAPI.Object.CreateObject(NAPI.Util.GetHashKey(WeaponData[weapon].Model), player.Position, player.Rotation, 255, 0);
            //temp_handle.AttachTo(player, bone, offset, rotation);

            //player.SetMyData(WeaponKeys[(int)WeaponData[weapon].Type], temp_handle);
            player.SetMyData(WeaponKeys[(int)WeaponData[weapon].Type], temp_handle);
        }

        public static void RemoveWeaponProp(Player player, WeaponAttachmentType type)
        {
            int type_int = (int)type;
            if (!player.HasMyData(WeaponKeys[type_int])) return;

            GTANetworkAPI.Object obj = player.GetMyData<GTANetworkAPI.Object>(WeaponKeys[type_int]);
            obj.Delete();

            player.ResetMyData(WeaponKeys[type_int]);
        }

        public static void RemoveWeaponProps(Player player)
        {
            foreach (string key in WeaponKeys)
            {
                if (!player.HasMyData(key)) continue;

                GTANetworkAPI.Object obj = player.GetMyData<GTANetworkAPI.Object>(key);
                obj.Delete();

                player.ResetMyData(key);
            }
        }
        #endregion

        #region Exported Methods
        public void RemovePlayerWeapon(Player player, WeaponHash weapon)
        {
            if (WeaponData.ContainsKey(weapon))
            {
                string key = WeaponKeys[(int)WeaponData[weapon].Type];

                if (player.HasMyData(key))
                {
                    GTANetworkAPI.Object obj = player.GetMyData<GTANetworkAPI.Object>(key);

                    if (obj.Model == NAPI.Util.GetHashKey(WeaponData[weapon].Model))
                    {
                        obj.Delete();
                        player.ResetMyData(key);
                    }
                }
            }
            NAPI.Player.RemovePlayerWeapon(player, weapon);
        }

        public void RemoveAllPlayerWeapons(Player player)
        {
            RemoveWeaponProps(player);
            NAPI.Player.RemoveAllPlayerWeapons(player);
        }
        #endregion

        #region Events
        [ServerEvent(Event.ResourceStop)]
        public void ArmedBody_Exit()
        {
            try
            {
                foreach (Player player in NAPI.Pools.GetAllPlayers()) RemoveWeaponProps(player);
            }
            catch (Exception e) { Log.Write("ResourceStop: " + e.StackTrace, nLog.Type.Error); }
        }
        #endregion
    }
}
