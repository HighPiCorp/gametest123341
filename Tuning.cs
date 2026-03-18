using client.Systems.BattlePass;
using GTANetworkAPI;
using NeptuneEvo.SDK;
using Newtonsoft.Json;

/*                                                               // FRom 5rp handle tuning
 *  mp.events.add("__client_lsCustomChip_set", (e, a) => {
          mp.vehicles.exists(this.currentVehicle) &&
            0 !== this.currentVehicle.handle &&
            this.currentVehicle.setHandling(e, a);
        }),
        mp.events.add("__client_lsCustomChip_driftHelper", (e) => {
          mp.vehicles.exists(this.currentVehicle) &&
            0 !== this.currentVehicle.handle &&
            this.currentVehicle.setReduceGrip(e);
        });
 *
 *
 *
 * */

namespace NeptuneEvo.Core
{
    class Tuning : Script
    {
        private static nLog Log = new nLog("Tuning");

    //private static Dictionary<string, string> carRaceName = BusinessManager.RealVehicles[5];

        public static Dictionary<int, Dictionary<string, double>> TuningCoefPricesTech = new Dictionary<int, Dictionary<string, double>>()
        {
            { 10, new Dictionary<string, double>() { // engine_menu
                { "-1", 0.06 },
                { "0", 0.17 },
                { "1", 0.2 },
                { "2", 0.23 },
                { "3", 0.29 },
            }},
            { 11, new Dictionary<string, double>() { // turbo_menu
                { "-1", 0.07 },
                { "0", 0.33 },
            }},
            { 13, new Dictionary<string, double>() { // transmission_menu
                { "-1", 0.05 },
                { "0", 0.19 },
                { "1", 0.22 },
                { "2", 0.28 },
            }},
            { 15, new Dictionary<string, double>() { // suspention_menu
                { "-1", 0.04 },
                { "0", 0.12 },
                { "1", 0.14 },
                { "2", 0.16 },
                { "3", 0.18 },
            }},
            { 16, new Dictionary<string, double>() { // brakes_menu
                { "-1", 0.03 },
                { "0", 0.11 },
                { "1", 0.13 },
                { "2", 0.15 },
            }},
        };

        public static Dictionary<int, Dictionary<string, int>> TuningPricesVisual = new Dictionary<int, Dictionary<string, int>>()
        {
            { 0, new Dictionary<string, int>() { // Глушитель
                { "-1", 1500 },
                { "0", 1500 },
                { "1", 1500 },
                { "2", 1500 },
                { "3", 1500 },
                { "4", 1500 },
                { "5", 1500 },
                { "6", 1500 },
                { "7", 1500 },
                { "8", 1500 },
                { "9", 1500 },
                { "10", 1500 },
                { "11", 1500 },
                { "12", 1500 },
                { "13", 1500 },
                { "14", 1500 },
                { "15", 1500 },
            }},
            { 1, new Dictionary<string, int>() { //Пороги
                { "-1", 2100 },
                { "0", 2100 },
                { "1", 2100 },
                { "2", 2100 },
                { "3", 2100 },
                { "4", 2100 },
                { "5", 2100 },
                { "6", 2100 },
                { "7", 2100 },
                { "8", 2100 },
                { "9", 2100 },
                { "10", 2100 },
                { "11", 2100 },
                { "12", 2100 },
                { "13", 2100 },
                { "14", 2100 },
                { "15", 2100 },
                { "16", 2100 },
                { "17", 2100 },
                { "18", 2100 },
                { "19", 2100 },
                { "20", 2100 },
            }},
            { 2, new Dictionary<string, int>() { // Капот
                { "-1", 2650 },
                { "0", 2650 },
                { "1", 2650 },
                { "2", 2650 },
                { "3", 2650 },
                { "4", 2650 },
                { "5", 2650 },
                { "6", 2650 },
                { "7", 2650 },
                { "8", 2650 },
                { "9", 2650 },
                { "10", 2650 },
                { "11", 2650 },
                { "12", 2650 },
                { "13", 2650 },
                { "14", 2650 },
                { "15", 2650 },
                { "16", 2650 },
                { "17", 2650 },
                { "18", 2650 },
                { "19", 2650 },
                { "20", 2650 },
            }},
            { 3, new Dictionary<string, int>() { //Спойлер
                { "-1", 4300 },
                { "0", 4300 },
                { "1", 4300 },
                { "2", 4300 },
                { "3", 4300 },
                { "4", 4300 },
                { "5", 4300 },
                { "6", 4300 },
                { "7", 4300 },
                { "8", 4300 },
                { "9", 4300 },
                { "10", 4300 },
                { "11", 4300 },
                { "12", 4300 },
                { "13", 4300 },
                { "14", 4300 },
                { "15", 4300 },
                { "16", 4300 },
                { "17", 4300 },
                { "18", 4300 },
                { "19", 4300 },
                { "20", 4300 },
            }},
            { 4, new Dictionary<string, int>() { // Решетка / Сплиттеры
                { "-1", 1350 },
                { "0", 1350 },
                { "1", 1350 },
                { "2", 1350 },
                { "3", 1350 },
                { "4", 1350 },
                { "5", 1350 },
                { "6", 1350 },
                { "7", 1350 },
                { "8", 1350 },
                { "9", 1350 },
                { "10", 1350 },
                { "11", 1350 },
                { "12", 1350 },
                { "13", 1350 },
                { "14", 1350 },
                { "15", 1350 },
                { "16", 1350 },
                { "17", 1350 },
                { "18", 1350 },
                { "19", 1350 },
                { "20", 1350 },
            }},
            { 5, new Dictionary<string, int>() { // Крылья
                { "-1", 2840 },
                { "0", 2840 },
                { "1", 2840 },
                { "2", 2840 },
                { "3", 2840 },
                { "4", 2840 },
                { "5", 2840 },
                { "6", 2840 },
                { "7", 2840 },
                { "8", 2840 },
                { "9", 2840 },
                { "10", 2840 },
                { "11", 2840 },
                { "12", 2840 },
                { "13", 2840 },
                { "14", 2840 },
                { "15", 2840 },
                { "16", 2840 },
                { "17", 2840 },
                { "18", 2840 },
                { "19", 2840 },
                { "20", 2840 },
            }},
            { 101, new Dictionary<string, int>() { // Крылья задние
                { "-1", 2900 },
                { "0", 2900 },
                { "1", 2900 },
                { "2", 2900 },
                { "3", 2900 },
                { "4", 2900 },
                { "5", 2900 },
                { "6", 2900 },
                { "7", 2900 },
                { "8", 2900 },
                { "9", 2900 },
                { "10", 2900 },
                { "11", 2900 },
                { "12", 2900 },
                { "13", 2900 },
                { "14", 2900 },
                { "15", 2900 },
                { "16", 2900 },
                { "17", 2900 },
                { "18", 2900 },
                { "19", 2900 },
                { "20", 2900 },
            }},
            { 6, new Dictionary<string, int>() { // Крыша
                { "-1", 3000 },
                { "0", 3000 },
                { "1", 3000 },
                { "2", 3000 },
                { "3", 3000 },
                { "4", 3000 },
                { "5", 3000 },
                { "6", 3000 },
                { "7", 3000 },
            }},
            { 7, new Dictionary<string, int>() { //Винил
                { "-1", 5370 },
                { "0", 5370 },
                { "1", 5370 },
                { "2", 5370 },
                { "3", 5370 },
                { "4", 5370 },
                { "5", 5370 },
                { "6", 5370 },
                { "7", 5370 },
                { "8", 5370 },
                { "9", 5370 },
                { "10", 5370 },
                { "11", 5370 },
                { "12", 5370 },
                { "13", 5370 },
                { "14", 5370 },
                { "15", 5370 },
                { "16", 5370 },
                { "17", 5370 },
                { "18", 5370 },
                { "19", 5370 },
                { "20", 5370 },
            }},
            { 8, new Dictionary<string, int>() { // Пер Бампер
                { "-1", 2970 },
                { "0", 2970 },
                { "1", 2970 },
                { "2", 2970 },
                { "3", 2970 },
                { "4", 2970 },
                { "5", 2970 },
                { "6", 2970 },
                { "7", 2970 },
                { "8", 2970 },
                { "9", 2970 },
                { "10", 2970 },
                { "11", 2970 },
                { "12", 2970 },
                { "13", 2970 },
                { "14", 2970 },
                { "15", 2970 },
                { "16", 2970 },
                { "17", 2970 },
                { "18", 2970 },
                { "19", 2970 },
                { "20", 2970 },
            }},
            { 9, new Dictionary<string, int>() { // Зад бамп
                { "-1", 3130 },
                { "0", 3130 },
                { "1", 3130 },
                { "2", 3130 },
                { "3", 3130 },
                { "4", 3130 },
                { "5", 3130 },
                { "6", 3130 },
                { "7", 3130 },
                { "8", 3130 },
                { "9", 3130 },
                { "10", 3130 },
                { "11", 3130 },
                { "12", 3130 },
                { "13", 3130 },
                { "14", 3130 },
                { "15", 3130 },
                { "16", 3130 },
                { "17", 3130 },
                { "18", 3130 },
                { "19", 3130 },
                { "20", 3130 },
            }},

            { 12, new Dictionary<string, int>() { // horn_menu
                { "-1", 1000 },
                { "0", 1000 },
                { "1", 1000 },
                { "2", 1000 },
                { "3", 1000 },
                { "4", 1000 },
                { "5", 1000 },
                { "6", 1000 },
                { "7", 1000 },
                { "8", 1000 },
                { "9", 1000 },
                { "10", 1000 },
                { "11", 1000 },
                { "12", 1000 },
                { "13", 1000 },
                { "14", 1000 },
                { "15", 1000 },
                { "16", 1000 },
                { "17", 1000 },
                { "18", 1000 },
                { "19", 1000 },
                { "20", 1000 },
                { "21", 1000 },
                { "22", 1000 },
                { "23", 1000 },
                { "24", 1000 },
                { "25", 1000 },
                { "26", 1000 },
                { "27", 1000 },
                { "28", 1000 },
                { "29", 1000 },
                { "30", 1000 },
                { "31", 1000 },
                { "32", 1000 },
                { "33", 1000 },
                { "34", 1000 },
            }},

            { 14, new Dictionary<string, int>() { // glasses_menu
                { "-1", 580 },
                { "0", 580 },
                { "3", 580 },
                { "2", 580 },
                { "1", 580 },
            }},
            { 17, new Dictionary<string, int>() { // lights_menu
                { "-1", 730 },
                { "0", 730 },
                { "1", 730 },
                { "2", 730 },
                { "3", 730 },
                { "4", 730 },
                { "5", 730 },
                { "6", 730 },
                { "7", 730 },
                { "8", 730 },
                { "9", 730 },
                { "10", 730 },
                { "11", 730 },
                { "12", 730 },
            }},
            { 18, new Dictionary<string, int>() { // numbers_menu
                { "-1", 1500 },
                { "0", 1500 },
                { "1", 1500 },
                { "2", 1500 },
                { "3", 1500 },
                { "4", 1500 },
            }},
        };

        public static Dictionary<int, Dictionary<int, int>> TuningWheels = new Dictionary<int, Dictionary<int, int>>()
        {
            // спортивные
            { 0, new Dictionary<int, int>() {
                { -1, 3000 },
                { 0, 27600 },
                { 1, 39000 },
                { 2, 42000 },
                { 3, 39600 },
                { 4, 110000 },
                { 5, 42000 },
                { 6, 41400 },
                { 7, 36000 },
                { 8, 36300 },
                { 9, 39000 },
                { 10, 45900 },
                { 11, 36900 },
                { 12, 32700 },
                { 13, 39000 },
                { 14, 33600 },
                { 15, 39600 },
                { 16, 28200 },
                { 17, 4500 },
                { 18, 29700 },
                { 19, 4500 },
                { 20, 39600 },
                { 21, 42000 },
                { 22, 49800 },
                { 23, 36000 },
                { 24, 39000 },
            }},
            // маслкары
            { 1, new Dictionary<int, int>() {
                { -1, 3000 },
                { 0, 3000 },
                { 1, 15000 },
                { 2, 4950 },
                { 3, 18000 },
                { 4, 19500 },
                { 5, 16800 },
                { 6, 17700 },
                { 7, 21000 },
                { 8, 18000 },
                { 9, 21000 },
                { 10, 18000 },
                { 11, 4950 },
                { 12, 15000 },
                { 13, 18000 },
                { 14, 15000 },
                { 15, 18000 },
                { 16, 24000 },
                { 17, 21000 },
            }},
            // лоурайдер
            { 2, new Dictionary<int, int>() {
                { -1, 3000 },
                { 0, 18300 },
                { 1, 19500 },
                { 2, 18300 },
                { 3, 20700 },
                { 4, 21000 },
                { 5, 2160 },
                { 6, 22500 },
                { 7, 24000 },
                { 8, 25500 },
                { 9, 25500 },
                { 10, 4500 },
                { 11, 18000 },
                { 12, 18300 },
                { 13, 21000 },
                { 14, 24000 },
            }},
            // вездеход
            { 3, new Dictionary<int, int>() {
                { -1, 3000 },
                { 0, 18000 },
                { 1, 24000 },
                { 2, 27000 },
                { 3, 30300 },
                { 4, 17100 },
                { 5, 20100 },
                { 6, 26100 },
                { 7, 2160 },
                { 8, 26400 },
                { 9, 30000 },
            }},
            // внедорожник
            { 4, new Dictionary<int, int>() {
                { -1, 3000 },
                { 0, 18000 },
                { 1, 22500 },
                { 2, 18900 },
                { 3, 23700 },
                { 4, 24000 },
                { 5, 27600 },
                { 6, 18900 },
                { 7, 15600 },
                { 8, 26700 },
                { 9, 22200 },
                { 10, 18600 },
                { 11, 19800 },
                { 12, 24000 },
                { 13, 21000 },
                { 14, 24900 },
                { 15, 18600 },
                { 16, 110000 },
            }},
            // тюннер
            { 5, new Dictionary<int, int>() {
                { -1, 3000 },
                { 0, 2160 },
                { 1, 24000 },
                { 2, 24600 },
                { 3, 30600 },
                { 4, 27300 },
                { 5, 26100 },
                { 6, 27600 },
                { 7, 24300 },
                { 8, 27600 },
                { 9, 22500 },
                { 10, 30900 },
                { 11, 24300 },
                { 12, 27600 },
                { 13, 30000 },
                { 14, 29700 },
                { 15, 24600 },
                { 16, 27300 },
                { 17, 28500 },
                { 18, 24600 },
                { 19, 27900 },
                { 20, 28800 },
                { 21, 29100 },
                { 22, 24600 },
                { 23, 21900 },
            }},
            // эксклюзивные
            { 7, new Dictionary<int, int>() {
                { -1, 3000 },
                { 0, 36000 },
                { 1, 21000 },
                { 2, 246000 },
                { 3, 2160 },
                { 4, 24000 },
                { 5, 26400 },
                { 6, 36000 },
                { 7, 27000 },
                { 8, 30600 },
                { 9, 30000 },
                { 10, 110000 },
                { 11, 30300 },
                { 12, 36300 },
                { 13, 30300 },
                { 14, 39300 },
                { 15, 36030 },
                { 16, 36300 },
                { 17, 30300 },
                { 18, 110000 },
                { 19, 30300 },
            }},
        };

        public static Dictionary<int, List<Tuple<int, string, int>>> TuningWheelsFull = new Dictionary<int, List<Tuple<int, string, int>>>()
        {
            { 0, new List<Tuple<int, string, int>>() {
                //new Tuple<int, string, int>(-1, "Стандартные", 2000),
                new Tuple<int, string, int>(0, "Inferno", 27600),
                new Tuple<int, string, int>(1, "Deep Five", 39000),
                new Tuple<int, string, int>(2, "Lozspeed Mk.V", 42000),
                new Tuple<int, string, int>(3, "Diamond Cut", 39600),
                new Tuple<int, string, int>(4, "Chrono", 41000),
                new Tuple<int, string, int>(5, "Feroci RR", 42000),
                new Tuple<int, string, int>(6, "FiftyNine", 41400),
                new Tuple<int, string, int>(7, "Mercie", 36000),
                new Tuple<int, string, int>(8, "Synthetic Z", 36300),
                new Tuple<int, string, int>(9, "Organic Type 0", 39000),
                new Tuple<int, string, int>(10, "Endo v.1", 45900),
                new Tuple<int, string, int>(11, "GT One", 36900),
                new Tuple<int, string, int>(12, "Duper 7", 32700),
                new Tuple<int, string, int>(13, "Uzer", 39000),
                new Tuple<int, string, int>(14, "GroundRide", 33600),
                new Tuple<int, string, int>(15, "S Racer", 39600),
                new Tuple<int, string, int>(16, "Venum", 28200),
                new Tuple<int, string, int>(17, "Cosmo", 45000),
                new Tuple<int, string, int>(18, "Dash VIP", 39700),
                new Tuple<int, string, int>(19, "Ice Kid", 45000),
                new Tuple<int, string, int>(20, "Ruff Weld", 39600),
                new Tuple<int, string, int>(21, "Wangan Master", 42000),
                new Tuple<int, string, int>(22, "Super Five", 49800),
                new Tuple<int, string, int>(23, "Endo v.2", 36000),
                new Tuple<int, string, int>(24, "Split Six", 39000),
            }},
            { 1, new List<Tuple<int, string, int>>() {
                //new Tuple<int, string, int>(-1, "Стандартные", 2000),
                new Tuple<int, string, int>(0, "Classic Five", 24000),
                new Tuple<int, string, int>(1, "Dukes", 23000),
                new Tuple<int, string, int>(2, "Muscle Freak", 24950),
                new Tuple<int, string, int>(3, "Kracka", 21000),
                new Tuple<int, string, int>(4, "Azreal", 26500),
                new Tuple<int, string, int>(5, "Mecha", 25800),
                new Tuple<int, string, int>(6, "Black Top", 23700),
                new Tuple<int, string, int>(7, "Drag SPL", 21000),
                new Tuple<int, string, int>(8, "Revolver", 23000),
                new Tuple<int, string, int>(9, "Classic Rod", 21000),
                new Tuple<int, string, int>(10, "Fairlie", 22000),
                new Tuple<int, string, int>(11, "Spooner", 20950),
                new Tuple<int, string, int>(12, "Five Star", 22000),
                new Tuple<int, string, int>(13, "Old School", 25000),
                new Tuple<int, string, int>(14, "El Jefe", 26000),
                new Tuple<int, string, int>(15, "Dodman", 27000),
                new Tuple<int, string, int>(16, "Six Gun", 24000),
                new Tuple<int, string, int>(17, "Mercenary", 21000),
            }},
            { 2, new List<Tuple<int, string, int>>() {
                //new Tuple<int, string, int>(-1, "Стандартные", 2000),
                new Tuple<int, string, int>(0, "Flare", 18300),
                new Tuple<int, string, int>(1, "Wired", 19500),
                new Tuple<int, string, int>(2, "Triple Golds", 18300),
                new Tuple<int, string, int>(3, "Big Worm", 20700),
                new Tuple<int, string, int>(4, "Seven Fives", 21000),
                new Tuple<int, string, int>(5, "Split Six", 21600),
                new Tuple<int, string, int>(6, "Fresh Mesh", 22500),
                new Tuple<int, string, int>(7, "Lead Sled", 24000),
                new Tuple<int, string, int>(8, "Turbine", 25500),
                new Tuple<int, string, int>(9, "Super Fin", 25500),
                new Tuple<int, string, int>(10, "Classic Rod", 20500),
                new Tuple<int, string, int>(11, "Dollar", 18000),
                new Tuple<int, string, int>(12, "Dukes", 18300),
                new Tuple<int, string, int>(13, "Low Five", 21000),
                new Tuple<int, string, int>(14, "Gooch", 24000),
            }},
            { 3, new List<Tuple<int, string, int>>() {
                //new Tuple<int, string, int>(-1, "Стандартные", 2000),
                new Tuple<int, string, int>(0, "Raider", 18000),
                new Tuple<int, string, int>(1, "Mudslinger", 24000),
                new Tuple<int, string, int>(2, "Nevis", 27000),
                new Tuple<int, string, int>(3, "Cairngorm", 20300),
                new Tuple<int, string, int>(4, "Amazon", 17100),
                new Tuple<int, string, int>(5, "Challenger", 20100),
                new Tuple<int, string, int>(6, "Dune Basher", 26100),
                new Tuple<int, string, int>(7, "Five Star", 21600),
                new Tuple<int, string, int>(8, "Rock Crawler", 26400),
                new Tuple<int, string, int>(9, "Mill Spec Steelie", 30000),
            }},
            { 4, new List<Tuple<int, string, int>>() {
                //new Tuple<int, string, int>(-1, "Стандартные", 2000),
                new Tuple<int, string, int>(0, "VIP", 18000),
                new Tuple<int, string, int>(1, "Benefactor", 19500),
                new Tuple<int, string, int>(2, "Cosmo", 18900),
                new Tuple<int, string, int>(3, "Bippu", 13700),
                new Tuple<int, string, int>(4, "Royal Six", 14000),
                new Tuple<int, string, int>(5, "Fagorme", 17600),
                new Tuple<int, string, int>(6, "Deluxe", 18900),
                new Tuple<int, string, int>(7, "Iced Out", 15600),
                new Tuple<int, string, int>(8, "Cognoscenti", 16700),
                new Tuple<int, string, int>(9, "LozSpeed Ten", 12200),
                new Tuple<int, string, int>(10, "Supernova", 18600),
                new Tuple<int, string, int>(11, "Obey RS", 19800),
                new Tuple<int, string, int>(12, "LozSpeed Baller", 14000),
                new Tuple<int, string, int>(13, "Extravaganzo", 18000),
                new Tuple<int, string, int>(14, "Split Six", 14900),
                new Tuple<int, string, int>(15, "Empowered", 18600),
                new Tuple<int, string, int>(16, "Sunrise", 15000),
                new Tuple<int, string, int>(17, "Dash VIP", 16200),
                new Tuple<int, string, int>(18, "Cutter", 12700),
            }},
            { 5, new List<Tuple<int, string, int>>() {
                //new Tuple<int, string, int>(-1, "Стандартные", 2000),
                new Tuple<int, string, int>(0, "Cosmo", 21600),
                new Tuple<int, string, int>(1, "Super Mesh", 24000),
                new Tuple<int, string, int>(2, "Outsider", 24600),
                new Tuple<int, string, int>(3, "Rollas", 30600),
                new Tuple<int, string, int>(4, "Driftmeister", 27300),
                new Tuple<int, string, int>(5, "Slicer", 26100),
                new Tuple<int, string, int>(6, "El Quatro", 27600),
                new Tuple<int, string, int>(7, "Dubbed", 24300),
                new Tuple<int, string, int>(8, "Five Star", 27600),
                new Tuple<int, string, int>(9, "Slideways", 22500),
                new Tuple<int, string, int>(10, "Apex", 30900),
                new Tuple<int, string, int>(11, "Stanced EG", 24300),
                new Tuple<int, string, int>(12, "Countersteer", 27600),
                new Tuple<int, string, int>(13, "Endo v.1", 30000),
                new Tuple<int, string, int>(14, "Endo v.2 Dish", 29700),
                new Tuple<int, string, int>(15, "Gruppe Z", 24600),
                new Tuple<int, string, int>(16, "Choku-Dori", 27300),
                new Tuple<int, string, int>(17, "Chicane", 28500),
                new Tuple<int, string, int>(18, "Saisoku", 24600),
                new Tuple<int, string, int>(19, "Dished Eight", 27900),
                new Tuple<int, string, int>(20, "Fujiwara", 28800),
                new Tuple<int, string, int>(21, "Zokusha", 29100),
                new Tuple<int, string, int>(22, "Battle VIII", 24600),
                new Tuple<int, string, int>(23, "Rally Master", 21900),
            }},
            { 7, new List<Tuple<int, string, int>>() {
                //new Tuple<int, string, int>(-1, "Стандартные", 2000),
                new Tuple<int, string, int>(0, "Shadow", 36000),
                new Tuple<int, string, int>(1, "Hyper", 31000),
                new Tuple<int, string, int>(2, "Blade", 24600),
                new Tuple<int, string, int>(3, "Diamond", 26000),
                new Tuple<int, string, int>(4, "Supa Gee", 34000),
                new Tuple<int, string, int>(5, "Chromatic Z", 26400),
                new Tuple<int, string, int>(6, "Mercie Ch.Lip", 26000),
                new Tuple<int, string, int>(7, "Obey RS", 32000),
                new Tuple<int, string, int>(8, "GT Chrome", 26000),
                new Tuple<int, string, int>(9, "Cheetah R", 30000),
                new Tuple<int, string, int>(10, "Solar", 38000),
                new Tuple<int, string, int>(11, "Split Ten", 35300),
                new Tuple<int, string, int>(12, "Dash VIP", 42300),
                new Tuple<int, string, int>(13, "LozSpeed Ten", 40300),
                new Tuple<int, string, int>(14, "Carbon Inferno", 45300),
                new Tuple<int, string, int>(15, "Carbon Shadow", 48030),
                new Tuple<int, string, int>(16, "Carbonic Z", 46300),
                new Tuple<int, string, int>(17, "Carbon Solar", 48300),
                new Tuple<int, string, int>(18, "Cheetah Carbon R", 54000),
                new Tuple<int, string, int>(19, "Carbon S Racer", 41300),
            }}
        };

        public static void interactionPressed(Player player)
        {
            if (!player.IsInVehicle || !player.Vehicle.HasData("ACCESS") || (player.Vehicle.GetData<string>("ACCESS") != "PERSONAL" && player.Vehicle.GetData<string>("ACCESS") != "FAMILY"))
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы должны находиться в личной машине", 3000);
                return;
            }
            if (player.Vehicle.Class == 13)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Велосипед не может быть затюнингован", 3000);
                return;
            }
            if (player.Vehicle.Class == 8)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Тюнинг пока что недоступен для мотоциклов :( Скоро исправим", 3000);
                return;
            }

            var occupants = VehicleManager.GetVehicleOccupants(player.Vehicle);
            foreach (var p in occupants.ToList())
            {
                if (p != player)
                {
                    VehicleManager.WarpPlayerOutOfVehicle(p);
                }
            }

            Trigger.ClientEvent(player, "tuningSeatsCheck");
        }

        #region remoteEvents

        [RemoteEvent("tuningSeatsCheck")]
        public static void RemoteEvent_tuningSeatsCheck(Player player)
        {
            try
            {
                if (!player.IsInVehicle || !player.Vehicle.HasData("ACCESS") || (player.Vehicle.GetData<string>("ACCESS") != "PERSONAL" && player.Vehicle.GetData<string>("ACCESS") != "FAMILY"))
                {

                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы должны находиться в личной машине", 3000);
                    return;
                }
                if (player.Vehicle.Class == 13)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Велосипед не может быть затюнингован", 3000);
                    return;
                }
                if (player.Vehicle.Class == 8)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Тюнинг пока что недоступен для мотоциклов :( Скоро исправим", 3000);
                    return;
                }

                if (player.GetMyData<int>("BIZ_ID") == -1) return;
                if (player.HasMyData("FOLLOWING"))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вас кто-то тащит за собой", 3000);
                    return;
                }

                Business biz = BusinessManager.BizList[player.GetMyData<int>("BIZ_ID")];
                Main.Players[player].TuningShop = biz.ID;

                var veh = player.Vehicle;
                var dim = Dimensions.RequestPrivateDimension(player);
                NAPI.Entity.SetEntityDimension(veh, dim);
                NAPI.Entity.SetEntityDimension(player, dim);

                player.SetIntoVehicle(veh, 0);

                NAPI.Entity.SetEntityPosition(veh, new Vector3(-337.7784, -136.5316, 39.4032));
                NAPI.Entity.SetEntityRotation(veh, new Vector3(0.04308624, 0.07037075, 148.9986));


                var id = player.Vehicle.GetData<int>("ID");

                var vehicleModel = VehicleManager.Vehicles[id].Model;
                Console.WriteLine(vehicleModel);
                var vehiclePrice = BusinessManager.ProductsOrderPrice[vehicleModel.ToLower()];
                var vehicleCoef = (vehiclePrice < 150000) ? 1 : 2;
                var vehicleComponents = JsonConvert.SerializeObject(VehicleManager.Vehicles[id].Components);

                var vehicleData = new
                {
                    vehicleModel,
                    vehicleCoef,
                    vehicleComponents
                };

                Trigger.ClientEvent(player, "openTuningMenu", false, biz.Products[0].Price, JsonConvert.SerializeObject(vehicleData), JsonConvert.SerializeObject(VehicleManager.Vehicles[id].Components));
            }
            catch (Exception e) { Log.Write("tuningSeatsCheck: " + e.StackTrace, nLog.Type.Error); }
        }

        [RemoteEvent("reopenTuningMenu")]
        public static void RemoteEvent_reopenTuningMenu(Player player)
        {
            try
            {
                if (player.GetMyData<int>("LAST_BIZ_ID") == -1) return;

                Business biz = BusinessManager.BizList[player.GetMyData<int>("LAST_BIZ_ID")];
                var id = player.Vehicle.GetData<int>("ID");
                var vehicleModel = VehicleManager.Vehicles[id].Model;
                var vehiclePrice = BusinessManager.ProductsOrderPrice[vehicleModel];
                var vehicleCoef = (vehiclePrice < 150000) ? 1 : 2;
                var vehicleComponents = JsonConvert.SerializeObject(VehicleManager.Vehicles[id].Components);

                var vehicleData = new
                {
                    vehicleModel,
                    vehicleCoef,
                    vehicleComponents
                };

                Trigger.ClientEvent(player, "openTuningMenu", true, biz.Products[0].Price, JsonConvert.SerializeObject(vehicleData), JsonConvert.SerializeObject(VehicleManager.Vehicles[id].Components));
                //Log.Debug("reopenTuningMenu: ", nLog.Type.Error);
            }
            catch (Exception e) { Log.Write("reopenTuningMenu: " + e.StackTrace, nLog.Type.Error); }
        }

        public static double getCoeficientTechTuning(int modelPrice)
        {
            //TuningPrices - 21.09.2022
            //До 200к         ---------- 1,1
            //От 200к до 500к ---------- 0,7
            //От 500к до 1кк  ---------- 0,4
            //От 1кк до 2кк   ---------- 0,25
            //От 2кк до 4кк   ---------- 0,19
            //От 4кк до 8кк   ---------- 0,12
            //От 8кк до 15кк  ---------- 0,07
            //От 15кк до 25кк ---------- 0,05

            double coef = 1;

            if (modelPrice < 200000) coef = 1.1;
            else if(modelPrice >= 200000 && modelPrice < 500000) coef = 0.7;
            else if(modelPrice >= 500000 && modelPrice < 1000000) coef = 0.4;
            else if (modelPrice >= 1000000 && modelPrice < 2000000) coef = 0.25;
            else if (modelPrice >= 2000000 && modelPrice < 4000000) coef = 0.19;
            else if (modelPrice >= 4000000 && modelPrice < 8000000) coef = 0.12;
            else if (modelPrice >= 8000000 && modelPrice < 15000000) coef = 0.07;
            else if (modelPrice >= 15000000) coef = 0.05;

            return coef;
        }

        public static double getCoeficientVisualTuning(int modelPrice)
        {
            //TuningPrices - 21.09.2022
            //До 200к         ---------- ХУЙ ЕГО ЗНАЕТ
            //От 200к до 500к ---------- 1
            //От 500к до 1кк  ---------- 1
            //От 1кк до 2кк   ---------- 0,7
            //От 2кк до 4кк   ---------- 0,6
            //От 4кк до 8кк   ---------- 0,5
            //От 8кк до 15кк  ---------- 0,7
            //От 15кк до 25кк ---------- 0,7

            double coef = 1;

            if (modelPrice < 200000) coef = 0;
            else if (modelPrice >= 200000 && modelPrice < 500000) coef = 1;
            else if (modelPrice >= 500000 && modelPrice < 1000000) coef = 1;
            else if (modelPrice >= 1000000 && modelPrice < 2000000) coef = 0.7;
            else if (modelPrice >= 2000000 && modelPrice < 4000000) coef = 0.6;
            else if (modelPrice >= 4000000 && modelPrice < 8000000) coef = 0.5;
            else if (modelPrice >= 8000000 && modelPrice < 15000000) coef = 0.7;
            else if (modelPrice >= 15000000) coef = 0.7;

            return coef;
        }


        public class AverageModelPrice
        {
            public int Price { get; set; } = 0;
            public int Type { get; set; } = 0;

            public AverageModelPrice(int price, int type)
            {
                Price = price;
                Type = type;
            }

        }

        public static AverageModelPrice getAverageModelPrice(int modelPrice)
        {
            var price = 0;
            var type = 0;


            if (modelPrice < 200000)
            {
              price = 75000;
              type = 0;
            }
            else if (modelPrice >= 200000 && modelPrice < 500000)
            {
              price = 290000;
              type = 1;
            }
            else if (modelPrice >= 500000 && modelPrice < 1000000)
            {
              price = 760000;
              type = 2;
            }
            else if (modelPrice >= 1000000 && modelPrice < 2000000)
            {
              price = 1430000;
              type = 3;
            }
            else if (modelPrice >= 2000000 && modelPrice < 4000000)
            {
              price = 2642000;
              type = 4;
            }
            else if (modelPrice >= 4000000 && modelPrice < 8000000)
            {
              price = 5500000;
              type = 5;
            }
            else if (modelPrice >= 8000000 && modelPrice < 15000000)
            {
              price = 10900000;
              type = 6;
            }
            else if (modelPrice >= 15000000)
            {
              price = 18600000;
              type = 7;
            }


            return new AverageModelPrice(price, type);
        }

        public static Dictionary<int, int> getAverageModelPriceByType = new Dictionary<int, int>()
        {
          { 0, 75000 },
          { 1, 290000 },
          { 2, 760000 },
          { 3, 1430000 },
          { 4, 2642000 },
          { 5, 5500000 },
          { 6, 10900000 },
          { 7, 18600000 },
        };

        public static Dictionary<int, int> getTuningPriceList(Business biz, int category, int modelPrice)
        {
            try
            {
                var list = new Dictionary<int, int>();

                if (TuningCoefPricesTech.ContainsKey(category))
                {
                    var modelCoef = getCoeficientTechTuning(modelPrice);
                    foreach(var item in TuningCoefPricesTech[category])
                    {
                        var index = Convert.ToInt32(item.Key);
                        var coef = item.Value;
                        int price = 0;

                        // 5650000 * 0.07 * 0.12 = 47460 / 100 = 474.60 * 400 (при 100% наценки 400, при 150 - 600)
                        // 13000 * 0.33 * 1.1  = 4 719 / 100 = 47.19 * 400 (при 100% наценки 400, при 150 - 600)
                        // octavia18 840000 * 0.33 * 0.4 = 110 880 / 100 = 1108.8 * 400
                        // ellie 41000 * 0.33 * 1.1 = 14 883 / 100 = 148.83 * 400
                        price = Convert.ToInt32((Math.Round(modelPrice * coef * modelCoef) / 100.0) * biz.GetPriceWithMarkUpInt(biz.Products[0].Price));

                        //Log.Debug($"[getTuningPriceList] category: {category} index: {index} coef: {coef} priceResult: {price} modelCoef: {modelCoef}", nLog.Type.Info);

                        list.Add(index, price);
                    }
                }

                if (TuningPricesVisual.ContainsKey(category))
                {

                    if (category == 18) // Номера
                    {
                        foreach(var item in TuningPricesVisual[category])
                        {
                            var index = Convert.ToInt32(item.Key);
                            var priceDefault = Convert.ToInt32((item.Value / 100.0) * biz.GetPriceWithMarkUpInt(biz.Products[0].Price));

                            list.Add(index, priceDefault);
                        }
                    }
                    else
                    {
                        foreach(var item in TuningPricesVisual[category])
                        {
                            var index = Convert.ToInt32(item.Key);
                            var price = getTuningPriceItem(category, Convert.ToInt32(index), modelPrice);
                            //Log.Debug($"[TuningPricesVisual] category: {category} index: {index} priceResult: {price}", nLog.Type.Info);

                            list.Add(index, price);
                        }
                    }
                }

                return list;
            }
            catch (Exception e) {
                Log.Write("getTuningPriceList: " + e.StackTrace, nLog.Type.Error);
                return null;
            }
        }

        public static int getTuningPriceItem(int category, int id, int modelPrice)
        {
            try
            {
                var price = 0;

                if (TuningCoefPricesTech.ContainsKey(category))
                {
                    var modelCoef = getCoeficientTechTuning(modelPrice);

                    //modelcoef: 0.12 modelPrice: 5 650 000 TuningCoefPricesTech: 0.07 FORMULA: 5650000 * 0.07 * 0.12 = 47.460
                    // 1.1 13000 0.33
                    price = Convert.ToInt32(Math.Round(modelPrice * TuningCoefPricesTech[category][id.ToString()] * modelCoef));
                }

                if (TuningPricesVisual.ContainsKey(category)) // Если визуал, то
                {

                    if (category == 18) // Если это номера, то
                    {
                        price = TuningPricesVisual[category][id.ToString()];
                    }
                    else
                    {
                        var modelVisualCoef = getCoeficientVisualTuning(modelPrice); // Берем коэфициент

                        if (modelVisualCoef == 0) // До 200 000
                        {
                            price = TuningPricesVisual[category][id.ToString()];
                        }
                        else
                        {
                            var prevAverageModelPrice = 0;
                            AverageModelPrice averageModel = getAverageModelPrice(modelPrice);
                            int averageModelPrice = averageModel.Price;

                            if (averageModel.Type - 1 <= 0) prevAverageModelPrice = getAverageModelPriceByType[0];
                            else prevAverageModelPrice = getAverageModelPriceByType[averageModel.Type - 1];

                            // Узнаем prevPrice
                            var prevPrice = 0;
                            foreach (var item in getAverageModelPriceByType)
                            {
                                if (item.Key == averageModel.Type) break;

                                var indexItem = item.Key;
                                var itemAverageModelPrice = item.Value;
                                var itemPrevAverageModelPrice = 0;

                                if (indexItem - 1 <= 0) itemPrevAverageModelPrice = getAverageModelPriceByType[0];
                                else itemPrevAverageModelPrice = getAverageModelPriceByType[indexItem - 1];

                                var itemModelVisualCoef = getCoeficientVisualTuning(itemAverageModelPrice);

                                if (itemModelVisualCoef == 0) // До 200 000
                                {
                                    prevPrice = TuningPricesVisual[category][id.ToString()];
                                    //Log.Debug("itemModelVisualCoef === 0 prevPrice: "+ prevPrice);
                                }
                                else
                                {
                                    //Log.Debug($"[formula] prevPrice * (itemAverageModelPrice / itemPrevAverageModelPrice) * itemModelVisualCoef -> {prevPrice} * ({itemAverageModelPrice} / {itemPrevAverageModelPrice}) * {itemModelVisualCoef}");
                                    prevPrice = Convert.ToInt32(Math.Round(prevPrice * (Convert.ToDouble(itemAverageModelPrice) / Convert.ToDouble(itemPrevAverageModelPrice)) * itemModelVisualCoef, 1, MidpointRounding.AwayFromZero));
                                }
                            }

                            //Log.Debug($"[!!!!!!! FINAL !!!!!!!] prevPrice * (averageModelPrice / prevAverageModelPrice) * modelVisualCoef -> {prevPrice} * ({averageModelPrice} / {prevAverageModelPrice}) * {modelVisualCoef}");
                            price = Convert.ToInt32(Math.Round(prevPrice * (Convert.ToDouble(averageModelPrice) / Convert.ToDouble(prevAverageModelPrice)) * modelVisualCoef, 1, MidpointRounding.AwayFromZero));
                        }
                    }
                }

                return price;
            }
            catch (Exception e) {
                Log.Write("getTuningPriceList: " + e.StackTrace, nLog.Type.Error);
                return 0;
            }
        }

        [RemoteEvent("getTuningList")]
        public static void RemoteEvent_getTuningList(Player player, string model, int index)
        {
            try
            {
                var id = player.Vehicle.GetData<int>("ID");
                var vehicleModel = VehicleManager.Vehicles[id].Model;
                var vehiclePrice = BusinessManager.ProductsOrderPrice[vehicleModel];

                if (!Main.Players.ContainsKey(player)) return;

                int bizID = Main.Players[player].TuningShop;
                Business biz = BusinessManager.BizList[bizID];
                var priceList = getTuningPriceList(biz, index, vehiclePrice);

                if (priceList != null)
                {
                    //Log.Debug("priceList: "+ JsonConvert.SerializeObject(priceList));
                    Trigger.ClientEvent(player, "updateTuningList", JsonConvert.SerializeObject(priceList), false);
                }
            }
            catch (Exception e) { Log.Write("RemoteEvent_getTuningList: " + e.StackTrace, nLog.Type.Error); }
        }

        [RemoteEvent("getTuningWheelsList")]
        public static void RemoteEvent_getTuningWheels(Player player, int index)
        {
            try
            {
                if (TuningWheelsFull.ContainsKey(index))
                {
                    Trigger.ClientEvent(player, "updateTuningList", JsonConvert.SerializeObject(TuningWheelsFull[index]), true);
                }
            }
            catch (Exception e) { Log.Write("getTuningWheelsList: " + e.StackTrace, nLog.Type.Error); }
        }


        [RemoteEvent("exitTuning")]
        public static void RemoteEvent_exitTuning(Player player)
        {
            try
            {
                int bizID = Main.Players[player].TuningShop;

                var veh = player.Vehicle;
                NAPI.Entity.SetEntityDimension(veh, 0);
                NAPI.Entity.SetEntityDimension(player, 0);

                player.SetIntoVehicle(veh, 0);

                NAPI.Entity.SetEntityPosition(veh, BusinessManager.BizList[bizID].EnterPoint + new Vector3(0, 0, 1.0));
                VehicleManager.ApplyCustomization(veh, player);
                Dimensions.DismissPrivateDimension(player);
                Main.Players[player].TuningShop = -1;
            }
            catch (Exception e) { Log.Write("ExitTuning: " + e.StackTrace, nLog.Type.Error); }
        }

        [RemoteEvent("resetTuningCustomization")]
        public static void RemoteEvennt_resetTuning(Player player)
        {
            try
            {
                var veh = player.Vehicle;
                VehicleManager.ApplyCustomization(veh, player);
            }
            catch (Exception e) { Log.Write("resetTuningCustomization: " + e.StackTrace, nLog.Type.Error); }
        }

        [RemoteEvent("buyTuning")]
        public static void RemoteEvent_buyTuning(Player player, params object[] arguments)
        {
            try
            {
                if (!Main.Players.ContainsKey(player)) return;

                int bizID = Main.Players[player].TuningShop;
                Business biz = BusinessManager.BizList[bizID];

                var cat = Convert.ToInt32(arguments[0].ToString());
                var id = Convert.ToInt32(arguments[1].ToString());

                var wheelsType = -1;
                var r = 0;
                var g = 0;
                var b = 0;
                var mod = 0;

                if (cat == 19)
                    wheelsType = Convert.ToInt32(arguments[2].ToString());
                else if (cat == 20)
                {
                    r = Convert.ToInt32(arguments[2].ToString());
                    g = Convert.ToInt32(arguments[3].ToString());
                    b = Convert.ToInt32(arguments[4].ToString());
                    mod = Convert.ToInt32(arguments[5].ToString());
                }

                var number = player.Vehicle.GetData<int>("ID");

                var vehModel = VehicleManager.Vehicles[number].Model;

                var modelPrice = BusinessManager.ProductsOrderPrice[vehModel];
                var modelPriceMod = (modelPrice < 150000) ? 1 : 2;

                var price = 0;

                if (cat <= 18 || cat == 101) // Весь тюнинг
                    price = Convert.ToInt32((getTuningPriceItem(cat, id, modelPrice) / 100.0) * biz.GetPriceWithMarkUpInt(biz.Products[0].Price));
                else if (cat == 19) // Колеса
                    price = Convert.ToInt32((TuningWheels[wheelsType][id] / 100.0) * biz.GetPriceWithMarkUpInt(biz.Products[0].Price));
                else // По сути цвета
                    price = Convert.ToInt32((5000 / 100.0) * biz.GetPriceWithMarkUpInt(biz.Products[0].Price));


                if (Main.Players[player].Money < price)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вам не хватает ещё {price - Main.Players[player].Money}$ для покупки этой модификации", 3000);
                    Trigger.ClientEvent(player, "tunBuySuccess", -2);
                    return;
                }


                var prod = biz.Products.FirstOrDefault(p => p.Name == "Запчасти");

                //price = 75400
                //amount = 75400 / 100 = 754 - 100% наценка
                //amount = 113100 / 150 = 754 - 150% наценка
                var amount = price / biz.GetPriceWithMarkUpInt(prod.Price);

                if (amount <= 0) amount = 1;
                if (!BusinessManager.takeProd(biz.ID, amount, prod.Name, price))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "В данной автомастерской закончились все запчасти", 3000);
                    Trigger.ClientEvent(player, "tunBuySuccess", -2);
                    return;
                }

                GameLog.Money($"player({Main.Players[player].UUID})", $"biz({biz.ID})", price, $"buyTuning({player.Vehicle.NumberPlate},{cat},{id})");
                MoneySystem.Wallet.Change(player, -price);
                Trigger.ClientEvent(player, "tunBuySuccess", id);

                #region BPКвест: 12 Потратить 1.000.000 на улучшения автомобиля.

                #region BattlePass выполнение квеста
                BattlePass.updateBPQuestIteration(player, BattlePass.BPQuestType.BuyTuning, price);
                #endregion

                #endregion

                #region SBPКвест: 1 Потратить 1.000.000 на улучшения автомобиля.

                #region BattlePass выполнение квеста
                BattlePass.updateBPSuperQuestIteration(player, BattlePass.BPSuperQuestType.SpendMoneyOnTuning, price);
                #endregion

                #endregion

                switch (cat)
                {
                    case 0:
                        VehicleManager.Vehicles[number].Components.Muffler = id;
                        break;
                    case 1:
                        VehicleManager.Vehicles[number].Components.SideSkirt = id;
                        break;
                    case 2:
                        VehicleManager.Vehicles[number].Components.Hood = id;
                        break;
                    case 3:
                        VehicleManager.Vehicles[number].Components.Spoiler = id;
                        break;
                    case 4:
                        VehicleManager.Vehicles[number].Components.Lattice = id;
                        break;
                    case 5:
                        VehicleManager.Vehicles[number].Components.Wings = id;
                        break;
                    case 101: // wings back Крылья задние
                        VehicleManager.Vehicles[number].Components.RearWings = id;
                        break;
                    case 6:
                        VehicleManager.Vehicles[number].Components.Roof = id;
                        break;
                    case 7:
                        VehicleManager.Vehicles[number].Components.Vinyls = id;
                        break;
                    case 8:
                        VehicleManager.Vehicles[number].Components.FrontBumper = id;
                        break;
                    case 9:
                        VehicleManager.Vehicles[number].Components.RearBumper = id;
                        break;
                    case 10:
                        VehicleManager.Vehicles[number].Components.Engine = id;
                        break;
                    case 11:
                        VehicleManager.Vehicles[number].Components.Turbo = id;
                        break;
                    case 12:
                        VehicleManager.Vehicles[number].Components.Horn = id;
                        break;
                    case 13:
                        VehicleManager.Vehicles[number].Components.Transmission = id;
                        break;
                    case 14:
                        VehicleManager.Vehicles[number].Components.WindowTint = id;
                        break;
                    case 15:
                        VehicleManager.Vehicles[number].Components.Suspension = id;
                        break;
                    case 16:
                        VehicleManager.Vehicles[number].Components.Brakes = id;
                        break;
                    case 17:
                        VehicleManager.Vehicles[number].Components.Headlights = id;
                        player.Vehicle.SetSharedData("hlcolor", id);
                        Trigger.ClientEvent(player, "VehStream_SetVehicleHeadLightColor", player.Vehicle.Handle, id);
                        break;
                    case 18:
                        VehicleManager.Vehicles[number].Components.NumberPlate = id;
                        break;
                    case 19:
                        VehicleManager.Vehicles[number].Components.Wheels = id;
                        VehicleManager.Vehicles[number].Components.WheelsType = wheelsType;
                        break;
                    case 20:
                        //Log.Debug("buyTuning color: id: "+id+" color: R:"+r+" G:"+g+" B:"+b+" Mod: "+mod);

                        if (id == 0)
                        {
                            VehicleManager.Vehicles[number].Components.PrimColor = new Color(r, g, b);
                            VehicleManager.Vehicles[number].Components.PrimModColor = mod;
                        }
                        else if (id == 1)
                        {
                            VehicleManager.Vehicles[number].Components.SecColor = new Color(r, g, b);
                            VehicleManager.Vehicles[number].Components.SecModColor = mod;
                        }
                        else
                            VehicleManager.Vehicles[number].Components.NeonColor = new Color(r, g, b);
                        break;
                    case 120:
                        //Log.Debug("buyTuning Pearl Extracolor: id: "+id);
                        VehicleManager.Vehicles[number].Components.PearlColor = id;
                        break;
                    case 121:
                        //Log.Debug("buyTuning WheelColor Extracolor: id: "+id);
                        VehicleManager.Vehicles[number].Components.WheelsColor = id;
                        break;

                }
                VehicleManager.Save(number);
                //VehicleManager.ApplyCustomization(player.Vehicle);
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Вы купили и установили данную модификацию", 3000);
                Trigger.ClientEvent(player, "tuningUpd", JsonConvert.SerializeObject(VehicleManager.Vehicles[number].Components));
            }
            catch (Exception e) { Log.Write("buyTuning: " + e.StackTrace, nLog.Type.Error); }
        }

        #endregion
    }
}
