using GTANetworkAPI;
using Newtonsoft.Json;
using System.Data;
using NeptuneEvo.GUI;
using NeptuneEvo.MoneySystem;
using NeptuneEvo.SDK;
using NeptuneEvo.Jobs;
using NeptuneEvo.Fractions;
using MySqlConnector;
using client.Systems.BattlePass;
using client.Fractions.Government.GOV;

namespace NeptuneEvo.Core
{
  class BusinessManager : Script
  {
    private static nLog Log = new nLog("BusinessManager");
    private static int lastBizID = -1;


    [ServerEvent(Event.ResourceStart)]
    public void onResourceStart()
    {
      try
      {
        var result = MySQL.QueryRead($"SELECT * FROM `businesses`");
        if (result == null || result.Rows.Count == 0)
        {
          Log.Write("DB biz return null result.", nLog.Type.Warn);
          return;
        }
        foreach (DataRow Row in result.Rows)
        {
          string owner = Row["owner"].ToString();
          int sellPrice = Convert.ToInt32(Row["sellprice"]);
          int type = Convert.ToInt32(Row["type"]);
          var products = JsonConvert.DeserializeObject<List<Product>>(Row["products"].ToString());
          int money = Convert.ToInt32(Row["money"]);
          int mafia = Convert.ToInt32(Row["mafia"]);
          var orders = JsonConvert.DeserializeObject<List<Order>>(Row["orders"].ToString());
          int cash = Convert.ToInt32(Row["cash"]);
          int markup = Convert.ToInt32(Row["markup"]);
          Vector3 enterpoint = JsonConvert.DeserializeObject<Vector3>(Row["enterpoint"].ToString());
          Vector3 unloadpoint = JsonConvert.DeserializeObject<Vector3>(Row["unloadpoint"].ToString());
          Vector3 managepoint = JsonConvert.DeserializeObject<Vector3>(Row["managepoint"].ToString());
          Vector3 sellpoint = JsonConvert.DeserializeObject<Vector3>(Row["sellpoint"].ToString());
          var lastbuy = ((DateTime)Row["last_buy"]);
          List<int> profit = new List<int>();

          profit = JsonConvert.DeserializeObject<List<int>>(Row["profit"].ToString());

          if (profit.Count < 31)
          {
            profit = new List<int>(new int[31]);
          }

          CollectorOrder collectorOrder;
          if (Row["collector_order"].ToString() == "-1" || Row["collector_order"].ToString().Length < 5 || Row["collector_order"].ToString() == "null")
          {
            collectorOrder = null;
          }
          else
          {
            collectorOrder = JsonConvert.DeserializeObject<CollectorOrder>(Row["collector_order"].ToString());
            Collections.Add(collectorOrder.UID, Convert.ToInt32(Row["id"]));
          }

          var id = Convert.ToInt32(Row["id"]);

          Business data = new Business(id, owner, sellPrice, type, products, enterpoint, unloadpoint, money, mafia, orders, managepoint, collectorOrder, cash, sellpoint, profit, markup);

          lastBizID = id;

          if (data.Type == 0)
          {
            //if (data.Products.Find(p => p.Name == "Связка ключей") == null)
            //{
            //    Product product = new Product(ProductsOrderPrice["Связка ключей"], 0, 0, "Связка ключей", false);
            //    data.Products.Add(product);
            //    Log.Write($"product Связка ключей was added to {data.ID} biz");
            //}
            data.Save();
          }
          BizList.Add(id, data);
        }
        UpdateBusinessBlips();
        fixbizprices();
        // Jobs.Truckers.SetGovermentOrders();
      }
      catch (Exception e)
      {
        Log.Write("EXCEPTION AT \"BUSINESSES\":\n" + e.ToString(), nLog.Type.Error);
      }
    }
    public static void fixbizprices()
    {
        try
        {
            foreach (var biz in BusinessManager.BizList.Values)
            {
                foreach (var item in biz.Products)
                {
                    item.Price = BusinessManager.ProductsOrderPrice[item.Name];
                }
            }
        }
        catch (Exception ex) { Log.Write(ex.StackTrace); }
    }
    public static void UpdateBusinessBlips()
      {
      NAPI.Task.Run(() => {
        foreach (var item in BizList.Values)
        {
          if (item.blip != null)
            if (item.Owner == "Государство" && BusinessManager.BizTypeHasOwner(item))
            {
              item.blip.Color = Convert.ToByte(1);
            }
            else item.blip.Color = Convert.ToByte(BusinessManager.BlipColorByType[item.Type]);
        }
      }, 0);
    }
    public static void SavingBusiness()
    {
      foreach (var b in BizList)
      {
        var biz = BizList[b.Key];
        biz.Save();
      }
      Log.Write("Businesses has been saved to DB", nLog.Type.Success);
    }

    [ServerEvent(Event.ResourceStop)]
    public void OnResourceStop()
    {
      try
      {
        SavingBusiness();
      }
      catch (Exception e) { Log.Write("ResourceStart: " + e.StackTrace, nLog.Type.Error); }
    }

    public static Dictionary<int, Business> BizList = new Dictionary<int, Business>();
    public static Dictionary<int, int> Orders = new Dictionary<int, int>(); // key - ID заказа, value - ID бизнеса,
    public static Dictionary<int, int> Collections = new Dictionary<int, int>(); // key - ID заказа, value - ID бизнеса,
    public static Dictionary<int, int> GovermentOrders = new Dictionary<int, int>();

    public enum BussinessTypes
    {
      ProductMarket = 0,
      GasStation,
      AutoPremium,
      AutoLux,
      AutoSimple,
      MotoSimple,
      GunShop,
      ClothesShop,
      BurgerShop,
      TattooShop,
      HairShop,
      MaskShop,
      TuningShop,
      CarWash,
      PetShop,
      RodShop,
      FishShop,
      AutoDonate,
      ElectroStation,
      TuningAtelie,
      AutoTrucker,
      BagsShop = 23,
      DonateAutoSalon = 24,
      DonateClothesShop = 25,
    }

    #region Businesses const`s
    public static List<string> BusinessTypeNames = new List<string>()
    {
        "24/7", // 0
	    "Заправка", // 1
	    "Авто-стандарт", // 2
	    "Авто-премиум", // 3
	    "Авто-эконом", // 4
	    "Мотомагазин", // 5
	    "Оружия", // 6
	    "Магазин одежды", // 7
	    "Бургерная", // 8
	    "Тату-салон", // 9
	    "Парикмахерская", // 10
	    "Магазин масок", // 11
	    "Тюнинг", // 12
	    "Автомойка", // 13
	    "Магазин животных", // 14
	    "Рыболовный магазин", // 15
	    "Скупка рыбы", // 16
	    "Реальные авто", // 17
        "Resale Cars", //18
        "Авто-спорт", //19
        "Авто-грузовые", //20
        "Электрозаправка", //21
        "Тюнинг-Ателье", //22
        "Магазин Сумок", //23 Bags Shop
        "Авто-Донат", //24 DonateAutoSalon
        "Донат магазин одежды" // 25
    };

    public static List<int> BlipByType = new List<int>()
    {
        52, // 24/7
        361, // petrol station
        820, // авто-стандарт
        825, // авто-премиум
        225, // авто-эконом
        522, // moto
        110, // gun shop
        73, // clothes shop
        106, // burger-shot
        75, // tattoo-salon
        71, // barber-shop
        362, // masks shop
        72, // ls customs
        569, // carwash
        251, // aero shop
        371, // FishShop
        628, // SellShop
        641, // авто-донат
        642, // ResaleCars
        523, //sport
        800, //грузовые
        354, // electro station
        756, //Тюнинг-Ателье
        765, // Bags Shop
        832, // Донат автосалон
        73, //DonateClothesShop
    };

    public static List<int> BlipColorByType = new List<int>()
    {
        4, // 24/7
        76, // petrol station
        45, // showroom
        45, // showroom
        45, // showroom
        45, // showroom
        76, // gun shop
        4, // clothes shop
        70, // burger-shot
        8, // tattoo-salon
        45, // barber-shop
        4, // masks shop
        40, // ls customs
        17, // carwash
        15, // aero shop
        3, // fishshop
        3, // sellshop
        60, // авто-донат
        30, // ResaleCars
        45, // showroom
        45, // showroom
        11, // electro station
        63, // Тюнинг-Ателье
        5, // Bags Shop
        11, // Донат автосалон
        2, //DonateClothesShop
    };

    private static List<string> FishProducts = new List<string>()
    {
        "Удочка",
        "Удочка MK2",
        "Наживка",
    };

    public static List<string> PetNames = new List<string>() {
        "Husky",
        "Poodle",
        "Pug",
        "Retriever",
        "Rottweiler",
        "Shepherd",
        "Westy",
        "Cat",
        "Rabbit",
    };

    public static List<int> PetHashes = new List<int>() {
        1318032802, // Husky
        1125994524,
        1832265812,
        882848737, // Retriever
        -1788665315,
        1126154828,
        -1384627013,
        1462895032,
        -541762431,
    };

    public static List<int> AutoSalonTypes = new List<int>() { 2, 3, 4, 5, 17, 18, 19, 20, 24 };

    public static Dictionary<int, Dictionary<string, string>> RealVehicles = new Dictionary<int, Dictionary<string, string>>() {
        // Standart - Asterope
        {0, new Dictionary<string, string>() {
            {"asterope", "Karin Asterope"},
            {"issi2", "Weeny Issi"},
            {"seminole", "Canis Seminole"},
            {"virgo", "Albany Virgo"},
            {"bjxl", "Karin BeeJay XL"},
            {"savestra", "Annis Savestra"},
            {"sentinel2", "Übermacht Sentinel"},
            {"retinue", "Vapid Retinue"},
            {"sabregt", "Declasse Sabre Turbo"},
            {"sultan", "Karin Sultan Classic"},
            {"granger", "Declasse Granger"},
            {"jackal", "Ocelot Jackal"},
            {"sabregt2", "Declasse Sabre Turbo Custom"},
            {"cavalcade2", "Albany Cavalcade Second Gen"},
            {"fusilade", "Schyster Fusilade"},
            {"previon", "Karin Previon"},
            {"sentinel3", "Übermacht Sentinel Classic"},
            {"tailgater", "Obey Tailgater"},
            {"warrener", "Vulcar Warrener"},
            {"remus", "Annis Remus"},
            {"surge", "Cheval Surge"},
            {"tulip", "Declasse Tulip"},
            {"baller", "Gallivanter Baller First Gen"},
            {"fq2", "Fathom FQ 2"},
            {"nightshade", "Imponte Nightshade"},
            {"schwarzer", "Benefactor Schwarzer"},
            {"gresley", "Bravado Gresley"},
            {"oracle2", "Übermacht Oracle"},
            {"serrano", "Benefactor Serrano"},
            {"brawler", "Coil Brawler"},
            {"felon2", "Lampadati Felon GT"},
            {"sultan2", "Karin Sultan"},
            {"vigero", "Declasse Vigero"},
            {"sultan3", "Karin Sultan RS Classic"},
            {"clique", "Vapid Clique"},
            {"freecrawler", "Canis Freecrawler"},
            {"ruiner", "Imponte Ruiner"},
            {"baller2", "Gallivanter Baller Second Gen"},
            {"baller3", "Gallivanter Baller LE"},
            {"calico", "Karin Calico GTF"},
            {"exemplar", "Dewbauchee Exemplar"},
            {"rocoto", "Obey Rocoto"},
            {"zr350", "Annis ZR350"},
            {"gauntlet4", "Bravado Gauntlet Hellfire"},
            {"penumbra", "Maibatsu Penumbra"},
            {"buffalo2", "Bravado Buffalo S"},
            {"novak", "Lampadati Novak"},
            {"imperator", "Vapid Imperator"},
            {"tropos", "Lampadati Tropos"},
            {"schafter2", "Benefactor Schafter"},
            {"penumbra2", "Maibatsu Penumbra FF"},
            {"cogcabrio", "Enus Cognoscenti Cabrio"},
            {"hermes", "Albany Hermes"},
            {"rt3000", "Dinka RT3000"},
            {"zion3", "Übermacht Zion Classic"},
            {"dominator", "Vapid Dominator"},
            {"elegy", "Annis Elegy RH8"},
            {"hustler", "Vapid Hustler"},
            {"komoda", "Lampadati Komoda"},
            {"massacro", "Dewbauchee Massacro"},
            {"cog55", "Enus Cognoscenti 55"},
            {"kuruma", "Karin Kuruma"},
            {"xls", "Benefactor XLS"},
            {"elegy2", "Annis Elegy Retro Custom"},
            {"euros", "Annus Euros"},
            {"yosemite", "Declasse Yosemite"},
            {"rapidgt3", "Dewbauchee Rapid GT Classic"},
            {"viseris", "Lampadati Viseris"},
            {"gauntlet5", "Bravado Gauntlet Classic Custom"},
            {"windsor", "Enus Windsor"},

        }},

        // Premium - Baller4
        {1, new Dictionary<string, string>() {
            {"baller4", "Gallivanter Baller LE LWB"},
            {"sc1", "Übermacht SC1"},
            {"specter", "Dewbauchee Specter"},
            {"alpha", "Albany Alpha"},
            {"tailgater2", "Obey Tailgater S"},
            {"cognoscenti", "Enus Cognoscenti"},
            {"revolter", "Übermacht Revolter"},
            {"sugoi", "Dinka Sugoi"},
            {"seven70", "Dewbauchee Seven-70"},
            {"dominator3", "Vapid Dominator GTX"},
            {"casco", "Lampadati Casco"},
            {"hotknife", "Vapid Hotknife"},
            {"rapidgt", "Dewbauchee Rapid GT"},
            {"schafter4", "Benefactor Schafter LWB"},
            {"voltic", "Coil Voltic"},
            {"zion2", "Übermacht Zion Cabrio"},
            {"cypher", "Übermacht Cypher"},
            {"khamelion", "Hijak Khamelion"},
            {"surano", "Benefactor Surano"},
            {"drafter", "Obey 8F Drafter"},
            {"jester3", "Dinka Jester Classic"},
            {"lynx", "Ocelot Lynx"},
            {"rapidgt2", "Dewbauchee Rapid GT"},
            {"schafter3", "Benefactor Schafter V12"},
            {"dominator7", "Vapid Dominator ASP"},
            {"imorgon", "Overflod Imorgon"},
            {"feltzer2", "Benefactor Feltzer"},
            {"jester4", "Dinka Jester"},
            {"vstr", "Albany V-STR"},
            {"ninef2", "Obey 9F Cabrio"},
            {"pariah", "Ocelot Pariah"},
            {"rebla", "Übermacht Rebla GTS"},
            {"dubsta", "Benefactor Dubsta"},
            {"dubsta2", "Benefactor Dubsta Second Gen"},
            {"jugular", "Ocelot Jugular"},
            {"neon", "Pfister Neon"},
            {"streiter", "Benefactor Streiter"},
            {"vectre", "Emperor Vectre"},
            {"huntley", "Enus Huntley S"},
            {"paragon", "Enus Paragon R"},
            {"patriot2", "Mammoth Patriot Stretch"},
            {"verlierer2", "Bravado Verlierer"},
            {"turismo2", "Grotti Turismo Classic"},
            {"vacca", "Pegassi Vaca"},
            {"windsor2", "Enus Windsor Drop"},
            {"schlagen", "Benefactor Schlagen GT"},
            {"stretch", "Dundreary Stretch"},
            {"raiden", "Coil Raiden"},
            {"italigto", "Grotti Itali GTO"},
            {"neo", "Vysser Neo"},
            {"superd", "Enus Super Diamond"},
            {"toros", "Pegassi Toros"},
            {"jb7002", "Dewbauchee JB 700W"},
            {"stinger", "Grotti Stinger"},
            {"swinger", "Ocelot Swinger"},
            {"tempesta", "Pegassi Tempesta"},
            {"thrax", "Truffade Thrax"},
            {"torero", "Pegassi Torero"},
            {"btype", "Albany Roosevelt"},
            {"dubsta3", "Benefactor Dubsta 6x6"},
            {"btype3", "Albany Roosevelt Valor"},
            {"btype2", "Albany Fränken Stange"},
            {"stafford", "Enus Stafford"},


        }},

        // Econom - Tornado4
        {2, new Dictionary<string, string>() {
            {"tornado4", "Declasse Tornado Rusted Cabrio"},
            {"voodoo2", "Declasse Voodoo Rusted"},
            {"tornado3", "Declasse Tornado Rusted"},
            {"emperor2", "Albany Emperor Rusted"},
            {"brioso2", "Grotti Brioso 300"},
            {"issi3", "Weeny Apocalypse Issi"},
            {"surfer2", "BF Surfer"},
            {"regina", "Dundreary Regina"},
            {"voodoo", "Declasse Voodoo"},
            {"ingot", "Vulcar Ingot"},
            {"fagaloa", "Vulcar Fagaloa"},
            {"asbo", "Maxwell Asbo"},
            {"emperor", "Albany Emperor"},
            {"tornado", "Declasse Tornado"},
            {"asea", "Declasse Asea"},
            {"dynasty", "Weeny Dynasty"},
            {"blade", "Vapid Blade"},
            {"chino", "Vapid Chino"},
            {"surfer", "BF Surfer Rusted"},
            {"tornado2", "Declasse Tornado Cabrio"},
            {"blista", "Dinka Blista"},
            {"intruder", "Karin Intruder"},
            {"manana", "Albany Manana Cabrio"},
            {"tornado5", "Declasse Tornado Custom"},
            {"blista2", "Dinka Blista Compact"},
            {"brioso", "Grotti Brioso R/A"},
            {"glendale", "Benefactor Glendale"},
            {"blista3", "Go Go Monkey Blista"},
            {"panto", "Benefactor Panto"},
            {"slamvan2", "Vapid Lost Slamvan"},
            {"weevil", "BF Weevil"},
            {"chino2", "Vapid Chino Custom"},
            {"manana2", "Albany Manana"},
            {"club", "BF Club"},
            {"glendale2", "Benefactor Glendale V2"},
            {"seminole2", "Canis Seminole Frontier"},
            {"rancherxl", "Declasse Rancher XL"},
            {"tampa", "Declasse Tampa"},
            {"dilettante", "Karin Dilettante"},
            {"nebula", "Vulcar Nebula Turbo"},
            {"prairie", "Bollokan Prairie"},
            {"premier", "DeClasse Premier"},
            {"stalion", "Declasse Stallion "},
            {"primo", "Albany Primo"},
            {"rebel", "Karin Rebel"},
            {"rhapsody", "DeClasse Rhapsody"},
            {"ellie", "Vapid Ellie"},
            {"stanier", "Vapid Stanier"},
            {"buccaneer", "Albany Buccaneer"},
            {"dukes", "Imponte Dukes"},
            {"faction", "Willard Faction"},
            {"virgo2", "Dundreary Virgo Classic Custom"},
            {"faction2", "Willard Faction Custom"},
            {"buccaneer2", "Albany Buccaneer Custom"},
            {"stratum", "Zirconium Stratum"},
            {"washington", "Albany Washington"},
            {"bfinjection", "BF Injection"},
            {"futo", "Karin Futo"},
            {"michelli", "Lampadati Michelli GT"},
            {"picador", "Cheval Picador"},
            {"buffalo", "Bravado Buffalo"},
            {"kanjo", "Dinka Blista Kanjo Compact"},
            {"vamos", "Declasse Vamos"},
            {"futo2", "Karin Futo GTX"},
            {"peyote", "Vapid Peyote Cabrio"},
            {"minivan", "Vapid Minivan"},
            {"radi", "Vapid Radius"},
            {"bifta", "BF Bifta"},
            {"oracle", "Übermacht Oracle"},
            {"phoenix", "Imponte Phoenix"},
            {"gauntlet", "Bravado Gauntlet"},
            {"peyote3", "Vapid Peyote"},
            {"cheburek", "RUNE Cheburek"},
            {"mesa", "Canis Mesa"},
            {"gauntlet3", "Bravado Gauntlet Classic"},
            {"cavalcade", "Albany Cavalcade"},
            {"landstalker", "Dundreary Landstalker"},
            {"habanero", "Emperor Habanero"},
            {"impaler", "Declasse Impaler"},
            {"fugitive", "Cheval Fugitive"},
            {"hellion", "Annis Hellion"},
            {"felon", "Lampadati Felon"},
            {"patriot", "Mammoth Patriot"},
            {"landstalker2", "Dundreary Landstalker XL"},
            {"retinue2", "Vapid Retinue MkII"},
            {"pigalle", "Lampadati Pigalle"},

        }},

        // Moto - Faggio3
        {3, new Dictionary<string, string>() {
            {"faggio3", "Pegassi Faggio Mod"},
            {"faggio2", "Principe Faggio"},
            {"faggio", "Pegassi Faggio Sport"},
            {"esskey", "Pegassi Esskey"},
            {"bagger", "WMC Bagger"},
            {"enduro", "Dinka Enduro"},
            {"daemon2", "WMC Daemon"},
            {"wolfsbane", "Western Wolfsbane"},
            {"pcj", "Shitzu PCJ-600"},
            {"sanchez2", "Maibatsu Sanchez"},
            {"daemon", "WMC Daemon Custom"},
            {"manchez", "Maibatsu Manchez"},
            {"vader", "Shitzu Vader"},
            {"blazer", "Nagasaki Blazer"},
            {"diablous2", "Principe Diabolus Custom"},
            {"diablous", "Principe Diabolus"},
            {"avarus", "LCC Avarus"},
            {"hexer", "Liberty City Cycles Hexer"},
            {"sovereign", "WMC Sovereign"},
            {"blazer4", "Nagasaki Street Blazer"},
            {"fcr", "Pegassi FCR 1000"},
            {"fcr2", "Pegassi FCR 1000 Custom"},
            {"ruffian", "Pegassi Ruffian"},
            {"zombiea", "Western Zombie Bobber"},
            {"zombieb", "Western Zombie Chopper"},
            {"blazer3", "Nagasaki Hot Rod Blazer"},
            {"innovation", "Liberty City Cycles Innovation"},
            {"nemesis", "Principe Nemesis"},
            {"deathbike", "WMC Apocalypse Deathbike"},
            {"deathbike3", "WMC Nightmare Deathbike"},
            {"thrust", "Dinka Thrust"},
            {"verus", "Dinka Verus"},
            {"vortex", "Pegassi Vortex"},
            {"bati", "Pegassi Bati 801"},
            {"sanctus", "LCC Sanctus"},
            {"defiler", "Shitzu Defiler"},
            {"gargoyle", "WMC Gargoyle"},
            {"stryder", "Nagasaki Stryder"},
            {"double", "Dinka Double-T"},
            {"akuma", "Dinka Akuma"},
            {"carbonrs", "Nagasaki Carbon RS"},
            {"nightblade", "Western Nightblade"},
            {"bf400", "Nagasaki BF400"},
            {"hakuchou", "Shitzu Hakuchou"},
            {"cliffhanger", "WMC Cliffhanger"},
            {"hakuchou2", "Shitzu Hakuchou Drag"},
            {"deathbike2", "WMC Future Shock Deathbike"},
            {"manchez2", "Maibatsu Manchez Scout"},
            {"shotaro", "Nagasaki Shotaro"},
        }},

        // Aero - Buzzard2
        {4, new Dictionary<string, string>() {
            {"Buzzard2", "Buzzard2"},
            {"Mammatus", "Mammatus"},
            {"Luxor2", "Luxor2"},
        }},

        // DLC - Deluxo
        {5, new Dictionary<string, string>() {
            {"ae86", "Corolla AE86"},
            {"w210", "E-Class W210"},
            {"benzc32", "C32 AMG"},
            {"s600", "S600 W140"},
            {"bmwe38", "E38"},
            {"mark2", "Mark II"},
            {"gcmsentra20", "Bluebird Sylphy"},
            {"audia8", "A8"},
            {"rmodm5e34", "M5 e34"},
            {"octavia18", "Octavia"},
            {"m3e46", "M3 E46"},
            {"optima", "Optima SXL Turbo"},
            {"lancer", "Lancer Evolution X"},
            {"m3e30", "M3 E30"},
            {"lrdef17", "Defender 110"},
            {"bnr32", "Skyline R32"},
            {"supra", "Supra A80"},
            {"rs3", "RS 3 II"},
            {"golf7r", "Golf 7R"},
            {"190e", "190 Evo II"},
            {"chevelle1970", "Chevelle SS 454"},
            {"m5e60", "M5 E60"},
            {"370z16", "370Z"},
            {"kiastinger", "Stinger"},
            {"audia6", "A6"},
            {"boss302", "Mustang Boss 302"},
            {"gl450", "GL450"},
            {"sti", "STI"},
            {"camry18", "Camry"},
            {"bmwg20", "3-Series G20"},
            {"rmodcharger69", "Charger 1969"},
            {"bmwm4", "M4 Coupe"},
            {"bnr34", "Skyline R34"},
            {"evoque", "Evoque"},
            {"z48", "Z4 M"},
            {"benzsl63", "SL63 AMG"},
            {"volvoxc90", "XC90"},
            {"glc2021", "GLC 300"},
            {"lc200", "Land Cruiser 200"},
            {"c63coupe", "C63"},
            {"m5", "M5"},
            {"durango18", "Durango SRT Hellcat HPE1000"},
            {"teslas", "Model S"},
            {"rmodrs6", "RS 6 Avant"},
            {"cruzzz", "Land Cruiser 300"},
            {"panamera17turbo", "Panamera 17 Turbo"},
            {"mbgls63", "GLS 63"},
            {"w223", "S-Class W223"},
            {"v447", "Vito III"},
            {"g65fresh", "65 AMG"},
            {"bmwx7", "X7"},
            {"modena99", "360 Modena"},
            {"rmodbmwi8", "i8"},
            {"rs72", "RS 7"},
            {"gtr", "GT-R"},
            {"porsche2021", "911 Turbo S"},
            {"rmodm8c", "M8 Competition Cabrio"},
            {"modelx", "Model X"},
            {"dbx", "DBX"},
            {"rmodlp570", "Gallardo LP570-4"},
            {"rmodbentleygt", "Continental GT Speed"},
            {"taycan", "Taycan Turbo S"},
            {"bc5506d", "F-550 Super Duty"},
            {"gls600", "GLS 600"},
            {"600lt", "600LT"},
            {"f458", "F458 Italia"},
            {"hevos", "Huracan Spyder"},
            {"488pista", "488 Pista"},
            {"gt17", "GT17"},
            {"urus", "Urus"},
            {"g63amg6x6", "G 63 6x6"},
            {"f812", "812 Superfast"},
            {"p1", "P1"},
            {"lp700", "Aventador LP700-4"},
            {"rculi", "Cullinan"},
            {"f12berlinetta", "F12 Berlinetta"},
            {"bugatti", "Veyron"},
            {"chiron17", "Chiron"},
            {"cu2", "Accord CU2"},
            {"dwrangler", "Wrangler Rattle Trap"},
            {"jeep392", "Wrangler Rubicon"},
            {"mitgalant92", "Galant Vr-4"},
        }},

        // Sport - Deviant
        {6, new Dictionary<string, string>() {
            {"deviant", "Schyster Deviant"},
            {"issi7", "Weeny Issi Sport"},
            {"banshee", "Bravado Banshee"},
            {"omnis", "Obey Omnis"},
            {"f620", "Ocelot F620"},
            {"furoregt", "Lampadati Furore GT"},
            {"banshee2", "Bravado Banshee 900R"},
            {"comet3", "Pfister Comet Retro Custom"},
            {"coquette", "Invetero Coquette"},
            {"ruston", "Hijak Ruston"},
            {"bullet", "Vapid Bullet"},
            {"monroe", "Pegassi Monroe"},
            {"comet4", "Pfister Comet Safari"},
            {"ninef", "Obey 9F"},
            {"locust", "Ocelot Locust"},
            {"comet2", "Pfister Comet"},
            {"infernus", "Pegassi Infernus"},
            {"jester", "Dinka Jester"},
            {"carbonizzare", "Grotti Carbonizzare"},
            {"gt500", "Grotti GT500"},
            {"stingergt", "Grotti Stinger GT"},
            {"mamba", "Declasse Mamba"},
            {"bestiagts", "Grotti Bestia GTS"},
            {"comet5", "Pfister Comet SR"},
            {"reaper", "Pegassi Reaper"},
            {"gp1", "Progen GP1"},
            {"coquette2", "Invetero Coquette Classic"},
            {"penetrator", "Ocelot Penetrator"},
            {"adder", "Truffade Adder"},
            {"entityxf", "Överflöd Entity XF"},
            {"infernus2", "Pegassi Infernus Classic"},
            {"osiris", "Pegassi Osiris"},
            {"comet6", "Pfister Comet S2"},
            {"coquette3", "Invetero Coquette BlackFin"},
            {"pfister811", "Pfister 811"},
            {"cheetah", "Grotti Cheetah"},
            {"entity2", "Överflöd Entity XXR"},
            {"fmj", "Vapid FMJ"},
            {"cyclone", "Coil Cyclone"},
            {"italigtb", "Progen Itali GTB"},
            {"t20", "Progen T20"},
            {"growler", "Pfister Growler"},
            {"cheetah2", "Grotti Cheetah Classic"},
            {"tigon", "Lampadati Tigon"},
            {"emerus", "Progen Emerus"},
            {"krieger", "Benefactor Krieger"},
            {"feltzer3", "Benefactor Stirling GT"},
            {"coquette4", "Invetero Coquette D10"},
            {"italirsx", "Grotti Itali RSX"},
            {"turismor", "Grotti Turismo R"},
            {"nero", "Truffade Nero"},
            {"tyrant", "Overflod Tyrant"},
            {"furia", "Grotti Furia"},
            {"tyrus", "Progen Tyrus"},
            {"xa21", "Ocelot XA-21"},
            {"taipan", "Cheval Taipan"},
            {"autarch", "Overflod Autarch"},
            {"deveste", "Principe Deveste Eight"},
            {"visione", "Grotti Visione"},
            {"zorrusso", "Pegassi Zorrusso"},
            {"prototipo", "Grotti X80 Proto"},
            {"vagner", "Dewbauchee Vagner"},
            {"zentorno", "Pegassi Zentorno"},
            {"tezeract", "Pegassi Tezeract"},
            {"ztype", "Truffade Z-Type"},
        }},

        // Trucks - Dloader
        {7, new Dictionary<string, string>() {
            {"dloader", "Bravado Duneloader"},
            {"bodhi2", "Canis Bodhi"},
            {"bobcatxl", "Vapid Bobcat XL"},
            {"rebel2", "Karin Rebel"},
            {"brutus", "Declasse Apocalypse Brutus"},
            {"slamvan", "Vapid Slamvan"},
            {"slamvan3", "Vapid Slamvan Custom"},
            {"yosemite3", "Declasse Yosemite Rancher"},
            {"ratloader2", "Bravado Rat-Truck"},
            {"bison", "Bravado Bison"},
            {"speedo", "Vapid Speedo Custom"},
            {"speedo4", "Vapid Speedo"},
            {"burrito3", "Declasse Burrito"},
            {"youga2", "Bravado Youga Classic"},
            {"moonbeam", "Declasse Moonbeam"},
            {"kamacho", "Canis Kamacho"},
            {"gburrito2", "Declasse Burrito Custom"},
            {"riata", "Vapid Riata"},
            {"everon", "Karin Everon"},
            {"caracara2", "Vapid Caracara 4x4"},
            {"contender", "Vapid Contender"},
            {"sandking2", "Vapid Sandking SWB"},
            {"sandking", "Vapid Sandking XL"},
            {"jimmy", "Jimmy 4x4"},
            {"vwcaddy", "Caddy"}
        }},

        // Тюнинг-Ателье
        {8, new Dictionary<string, string>() {
            {"rmodbiposto", "595 Competizione"},
            {"rmodm3e36", "M3E36 Wide Body"},
            {"svt00", "Mustang SVT Cobra"},
            {"rmod240sx", "240sx by Rocket Bunny"},
            {"rmodz350pandem", "350z by Pandem"},
            {"rmodsupra", "Supra Paul Walker Edition"},
            {"cam8tun", "Camry 3.5 AT XSE"},
            {"rmodfordgt", "Mustang GT Wide Body"},
            {"rmodsuprapandem", "Supra A90 by Pandem"},
            {"m4comp", "M4 Competition"},
            {"rmodamgc63", "G63S Wide Body"},
            {"rmodm4gts", "M4 GTS"},
            {"rmodrover", "Sport SVR by Mansory"},
            {"rmodmustang", "Mustang GT Boss Edition"},
            {"rmodjeep", "Grand Cherokee Trackhawk"},
            {"mcls53", "CLS 53"},
            {"rmodgt63", "GT 63 S"},
            {"m5f90", "M5 F90"},
            {"rmodx6", "X6M"},
            {"rmodmi8lb", "i8 Cabrio LB&YZ Edition"},
            {"rmodskyline", "GT-R Nismo"},
            {"pgt2", "911 GT2"},
            {"hevo", "Huracan Spyder"},
            {"mbbs20", "GT RI"},
            {"rmodlp750", "Aventador LP750-4"},
            {"s15rb", "Silvia S15 Rocket Bunny"},
        }},

        {9, new Dictionary<string, string>() {
            {"4runner", "4runner"},
            {"16challenger", "Challenger SRT"},
            {"21k5", "K5"},
            {"99viper", "Viper GTS"},
            {"ap2", "s2000"},
            {"cobra", "Cobra 427"},
            {"fct", "California"},
            {"m422", "M4"},
            {"mlmansory", "Levante Mansory"},
            {"mustang68", "Mustang 1968"},
            {"na6", "Miata MX-5"},
            {"rs520", "RS5 Coupe"},
            {"rx811", "RX-8"},
            {"supraa90", "Supra A90"},
            {"tricolore", "Zonda Tricolore"},
            {"tur50", "Touareg R50"},
            {"wildtrak", "Bronco"},
            {"wraith",  "Wraith"},
            {"x3gemwb", "Gemera"}
        }},

        //Сторонние тачки
        {999, new Dictionary<string, string>() {
            {"apriora", "Priora"},
            {"x5om", "X5 M"},
            {"exige12", "Exige S"},
            {"bmwm7", "M760Li"},
            {"mere63amg", "E 63 S"},
            {"explorer", "Explorer"},
            {"ocnetrongt", "e-tron GT"},
            {"gle63", "GLE 63 S"},
            {"aetron", "e‑tron S"},
            {"rmodrs7", "RS7 Wide Body"},
            {"rmodm4", "M4 Wide Body"},
            {"rmodmi8", "i8 Cabrio"},
            {"63amg", "G63 AMG"},
            {"rmodi8mlb", "i8 LB&YZ Edition"},
            {"rmodi8ks", "i8 Coupe K.S. Edition"},
            {"fenyr", "Fenyr Super Sport"},
            {"roma", "Roma"},
            {"rmodg65", "G65"},
            {"rmodgtr", "GT-R Godzilla"},
            {"huracan", "Huracan"},
            {"f8t", "F8 Tributo"},
            {"divo", "Divo"},
            {"r820", "R8"},
            {"rs7c8", "RS7"},
            {"subwrx", "WRX"},
            {"stalion2", "Declasse Stallion Burger Shot Edition"},
            {"buffalo3", "Bravado Buffalo Sprunk Edition"},
            {"rrocket", "Rampant Rocket"},
            {"sultanrs", "Karin Sultan RS"},
            {"impaler4", "Declasse Apocalypse Impaler"},
            {"2015polstang", "Police Mustang GT"},
            {"rmodpolice", "Police Vehicle" },
            {"jesko", "Jesko"},
            {"agera11", "Agera"},
            {"rmodatlantic", "Atlantic 2019"},
            {"rmodbacalar", "Mulliner Bacalar"},
            {"rmodbugatti", "La Voiture Noire"},
            {"rmodchiron300", "Chiron Super Sport 300+"},
            {"rmodessenza", "Essenza SCV12"},
            {"sian", "Sian"},
            {"tesla", "Roadster"},
            {"countach", "Countach LPI 800-4"},
            {"bugatticentodieci", "Centodieci"},
            {"eleanor", "Shelby GT500 Eleanor"},
            {"forgt50020", "Mustang Shelby GT500"},
            {"rolls08", "Sweptail"},
            {"f40", "F40"},
            {"fxxkevo", "FXX-K Evo"},
            {"m1procar", "M1 Procar"},
            {"rmodlp770", "Centenario LP770-4"},
            {"rmodpagani", "Huayra"},
            {"rmodveneno", "Veneno"},
            {"2cv6naj","2cv 1939"},
            {"belair56","Bel Air 56"},
            {"celestem5","500K"},
            {"ff1375","375 F1"},
            {"fleetline48","Fleetline Aero Coupe 1948"},
            {"ford34","34 MT"},
            {"forfalcxr","Falcon XR GT 4-door Sedan 1967"},
            {"impala72","Impala 1972"},
            {"jarama","Jarama GT 400"},
            {"pwagon51","Power Wagon 1951"},
            {"renault4","4 1961"},
            {"simca1100","1100 1972"},
            {"vwbeetlenaj","Beetle 1963"},
            {"zephyr41c","Zephyr Convertible 1941"},
            {"amg1","ONE"},
            {"avtr","Vision AVTR"},
            {"huragucci","Huracan X Gucci"},
            {"mercedessl63","SL63"},
            {"sennasmso","Senna Carbon MSO 2018"},
            {"slr","SLR"},
            {"al18","Alphard Hybrid"},
            {"stradale18","GranTurismo MC Stradale"},
            {"ocnlamtmc","Terzo Millennio"},
            {"500gtrlam","Diablo GTR"},
            {"monowheel","Mono Wheel"},
            {"monza","Monza SP2"},
            {"morgan","Aero SuperSports"},
            {"nc1","NSX NC1"},
            {"noire19wb","La Voiture Noire Limited Edition"},
            {"singer","Singer"},
            {"twizy","Twizy"},
            {"vclass21","V-Classe"},
            {"x6mf16","X6M F16"},
        }},

    };

    public static List<List<string>> CarsNames = new List<List<string>>()
    {
        new List<string>() // standart
        {
            "asterope",
            "issi2",
            "seminole",
            "virgo",
            "bjxl",
            "savestra",
            "sentinel2",
            "retinue",
            "sabregt",
            "sultan",
            "granger",
            "jackal",
            "sabregt2",
            "cavalcade2",
            "fusilade",
            "previon",
            "sentinel3",
            "tailgater",
            "warrener",
            "remus",
            "surge",
            "tulip",
            "baller",
            "fq2",
            "nightshade",
            "schwarzer",
            "gresley",
            "oracle2",
            "serrano",
            "brawler",
            "felon2",
            "sultan2",
            "vigero",
            "sultan3",
            "clique",
            "freecrawler",
            "ruiner",
            "baller2",
            "baller3",
            "calico",
            "exemplar",
            "rocoto",
            "zr350",
            "gauntlet4",
            "penumbra",
            "buffalo2",
            "novak",
            "imperator",
            "tropos",
            "schafter2",
            "penumbra2",
            "cogcabrio",
            "hermes",
            "rt3000",
            "zion3",
            "dominator",
            "elegy",
            "hustler",
            "komoda",
            "massacro",
            "cog55",
            "kuruma",
            "xls",
            "elegy2",
            "euros",
            "yosemite",
            "rapidgt3",
            "viseris",
            "gauntlet5",
            "windsor",
        },
        new List<string>() // premium
        {
            "baller4",
            "sc1",
            "specter",
            "alpha",
            "tailgater2",
            "cognoscenti",
            "revolter",
            "sugoi",
            "seven70",
            "dominator3",
            "casco",
            "hotknife",
            "rapidgt",
            "schafter4",
            "voltic",
            "zion2",
            "cypher",
            "khamelion",
            "surano",
            "drafter",
            "jester3",
            "lynx",
            "rapidgt2",
            "schafter3",
            "dominator7",
            "imorgon",
            "feltzer2",
            "jester4",
            "vstr",
            "ninef2",
            "pariah",
            "rebla",
            "dubsta",
            "dubsta2",
            "jugular",
            "neon",
            "streiter",
            "vectre",
            "huntley",
            "paragon",
            "patriot2",
            "verlierer2",
            "turismo2",
            "vacca",
            "windsor2",
            "schlagen",
            "stretch",
            "raiden",
            "italigto",
            "neo",
            "superd",
            "toros",
            "jb7002",
            "stinger",
            "swinger",
            "tempesta",
            "thrax",
            "torero",
            "btype",
            "dubsta3",
            "btype3",
            "btype2",
            "stafford",

        }, // premium
        new List<string>() // эконом
        {
            "tornado4",
            "voodoo2",
            "tornado3",
            "emperor2",
            "brioso2",
            "issi3",
            "surfer2",
            "regina",
            "voodoo",
            "ingot",
            "fagaloa",
            "asbo",
            "emperor",
            "tornado",
            "asea",
            "dynasty",
            "blade",
            "chino",
            "surfer",
            "tornado2",
            "blista",
            "intruder",
            "manana",
            "tornado5",
            "blista2",
            "brioso",
            "glendale",
            "blista3",
            "panto",
            "slamvan2",
            "weevil",
            "chino2",
            "manana2",
            "club",
            "glendale2",
            "seminole2",
            "rancherxl",
            "tampa",
            "dilettante",
            "nebula",
            "prairie",
            "premier",
            "stalion",
            "primo",
            "rebel",
            "rhapsody",
            "ellie",
            "stanier",
            "buccaneer",
            "dukes",
            "faction",
            "virgo2",
            "faction2",
            "buccaneer2",
            "stratum",
            "washington",
            "bfinjection",
            "futo",
            "michelli",
            "picador",
            "buffalo",
            "kanjo",
            "vamos",
            "futo2",
            "peyote",
            "minivan",
            "radi",
            "bifta",
            "oracle",
            "phoenix",
            "gauntlet",
            "peyote3",
            "cheburek",
            "mesa",
            "gauntlet3",
            "cavalcade",
            "landstalker",
            "habanero",
            "impaler",
            "fugitive",
            "hellion",
            "felon",
            "patriot",
            "landstalker2",
            "retinue2",
            "pigalle",

        },
        new List<string>() // moto
        {
            "faggio3",
            "faggio2",
            "faggio",
            "esskey",
            "bagger",
            "enduro",
            "daemon2",
            "wolfsbane",
            "pcj",
            "sanchez2",
            "daemon",
            "manchez",
            "vader",
            "blazer",
            "diablous2",
            "diablous",
            "avarus",
            "hexer",
            "sovereign",
            "blazer4",
            "fcr",
            "fcr2",
            "ruffian",
            "zombiea",
            "zombieb",
            "blazer3",
            "innovation",
            "nemesis",
            "deathbike",
            "deathbike3",
            "thrust",
            "verus",
            "vortex",
            "bati",
            "sanctus",
            "defiler",
            "gargoyle",
            "stryder",
            "double",
            "akuma",
            "carbonrs",
            "nightblade",
            "bf400",
            "hakuchou",
            "cliffhanger",
            "hakuchou2",
            "deathbike2",
            "manchez2",
            "shotaro",

        }, // moto
        new List<string>() // aero room
        {
            "Buzzard2",
            "Mammatus",
            "Luxor2"
        }, // aero room
        new List<string>() // авто-донат
        {
            "ae86",
            "w210",
            "benzc32",
            "s600",
            "bmwe38",
            "mark2",
            "gcmsentra20",
            "audia8",
            "rmodm5e34",
            "octavia18",
            "m3e46",
            "optima",
            "lancer",
            "m3e30",
            "lrdef17",
            "bnr32",
            "supra",
            "rs3",
            "golf7r",
            "190e",
            "chevelle1970",
            "m5e60",
            "370z16",
            "kiastinger",
            "audia6",
            "boss302",
            "gl450",
            "sti",
            "camry18",
            "bmwg20",
            "rmodcharger69",
            "bmwm4",
            "bnr34",
            "evoque",
            "z48",
            "benzsl63",
            "volvoxc90",
            "glc2021",
            "lc200",
            "c63coupe",
            "m5",
            "durango18",
            "teslas",
            "rmodrs6",
            "cruzzz",
            "panamera17turbo",
            "mbgls63",
            "w223",
            "v447",
            "g65fresh",
            "bmwx7",
            "modena99",
            "rmodbmwi8",
            "rs72",
            "gtr",
            "porsche2021",
            "rmodm8c",
            "modelx",
            "dbx",
            "rmodlp570",
            "rmodbentleygt",
            "taycan",
            "bc5506d",
            "gls600",
            "600lt",
            "f458",
            "hevos",
            "488pista",
            "gt17",
            "urus",
            "g63amg6x6",
            "f812",
            "p1",
            "lp700",
            "rculi",
            "f12berlinetta",
            "bugatti",
            "chiron17",
            "cu2",
            "dwrangler",
            "jeep392",
            "mitgalant92",
        },

        new List<string>() // спорт
        {
            "deviant",
            "issi7",
            "banshee",
            "omnis",
            "f620",
            "furoregt",
            "banshee2",
            "comet3",
            "coquette",
            "ruston",
            "bullet",
            "monroe",
            "comet4",
            "ninef",
            "locust",
            "comet2",
            "infernus",
            "jester",
            "carbonizzare",
            "gt500",
            "stingergt",
            "mamba",
            "bestiagts",
            "comet5",
            "reaper",
            "gp1",
            "coquette2",
            "penetrator",
            "adder",
            "entityxf",
            "infernus2",
            "osiris",
            "comet6",
            "coquette3",
            "pfister811",
            "cheetah",
            "entity2",
            "fmj",
            "cyclone",
            "italigtb",
            "t20",
            "growler",
            "cheetah2",
            "tigon",
            "emerus",
            "krieger",
            "feltzer3",
            "coquette4",
            "italirsx",
            "turismor",
            "nero",
            "tyrant",
            "furia",
            "tyrus",
            "xa21",
            "taipan",
            "autarch",
            "deveste",
            "visione",
            "zorrusso",
            "prototipo",
            "vagner",
            "zentorno",
            "tezeract",
            "ztype",
        }, // спорт

        new List<string>() // грузовые(не забыть добавить в массив для дальнобоя)
        {
            "dloader",
            "bodhi2",
            "bobcatxl",
            "rebel2",
            "brutus",
            "slamvan",
            "slamvan3",
            "yosemite3",
            "ratloader2",
            "bison",
            "speedo",
            "speedo4",
            "burrito3",
            "youga2",
            "moonbeam",
            "kamacho",
            "gburrito2",
            "riata",
            "everon",
            "caracara2",
            "contender",
            "sandking2",
            "sandking",
            "jimmy",
            "vwcaddy"
        }, // грузовые

        new List<string>() // Тюнинг-Ателье
        {
            "rmodbiposto",
            "rmodm3e36",
            "svt00",
            "rmod240sx",
            "rmodz350pandem",
            "rmodsupra",
            "cam8tun",
            "rmodfordgt",
            "rmodsuprapandem",
            "m4comp",
            "rmodamgc63",
            "rmodm4gts",
            "rmodrover",
            "rmodmustang",
            "rmodjeep",
            "mcls53",
            "rmodgt63",
            "m5f90",
            "rmodx6",
            "rmodmi8lb",
            "rmodskyline",
            "pgt2",
            "hevo",
            "mbbs20",
            "rmodlp750",
            "s15rb",
        }, // Тюнинг-Ателье

        new List<string>() // DonateAutoSalonSWC
        {
             "4runner",
            "16challenger",
            "21k5",
            "99viper",
            "ap2",
            "cobra",
            "fct",
            "m422",
            "mlmansory",
            "mustang68",
            "na6",
            "rs520",
            "rx811",
            "supraa90",
            "tricolore",
            "tur50",
            "wildtrak",
            "wraith",
            "x3gemwb",
        },

        new List<string>() // Сторонние тачки
        {
            "apriora",
            "x5om",
            "exige12",
            "bmwm7",
            "mere63amg",
            "explorer",
            "ocnetrongt",
            "gle63",
            "aetron",
            "rmodrs7",
            "rmodm4",
            "rmodmi8",
            "63amg",
            "rmodi8mlb",
            "rmodi8ks",
            "fenyr",
            "roma",
            "rmodg65",
            "rmodgtr",
            "huracan",
            "f8t",
            "f40",
            "divo",
            "r820",
            "rs7c8",
            "subwrx",
            "pwagon51",
            "2cv6naj",
            "vwbeetlenaj",
            "renault4",
            "simca1100",
            "forfalcxr",
            "impala72",
            "belair56",
            "zephyr41c",
            "ford34",
            "fleetline48",
            "celestem5",
            "ff1375",
            "jarama",
            "al18",
            "stradale18",
            "ocnlamtmc",
            "amg1",
            "avtr",
            "huragucci",
            "mercedessl63",
            "sennasmso",
            "slr",
            "monowheel",
            "monza",
            "morgan",
            "nc1",
            "noire19wb",
            "singer",
            "twizy",
            "vclass21",
            "x6mf16",
        },
    };

    private static List<string> GunNames = new List<string>()
    {
        "Pistol",
        "Combatpistol",
        "Revolver",
        "Heavypistol",

        "Bullpupshotgun",

        "Combatpdw",
        "Machinepistol",
    };

    private static List<string> MarketProducts = new List<string>()
    {
        "Монтировка",
        "Фонарик",
        "Молоток",
        "Гаечный ключ",
        "Канистра бензина",
        "Чипсы",
        "eCola",
        //"Пицца",
        "Сим-карта",
        //"Связка ключей",
        //"Рюкзак",
        "Бутылка воды",
        "Рем. набор",
        "Малая аптечка"
    };

    private static List<string> BurgerProducts = new List<string>()
    {
        "Бургер",
        "Хот-Дог",
        "Сэндвич",
        "Пицца",
        "Sprunk",
    };

    public static Dictionary<string, int> ProductsCapacity = new Dictionary<string, int>()
    {
        { "Расходники", 5000 }, // tattoo shop
        { "Татуировки", 0 },
        { "Парики", 0 }, // barber-shop
        { "Бургер", 100}, // burger-shot
        { "Хот-Дог", 100},
        { "Сэндвич", 150},
        { "eCola", 100},
        { "Sprunk", 100},
        { "Монтировка", 50}, // market
        { "Фонарик", 50},
        { "Молоток", 50},
        { "Гаечный ключ", 50},
        { "Канистра бензина", 50},
        { "Чипсы", 100},
        { "Пицца", 100},
        { "Сим-карта", 50},
        //{ "Связка ключей", 50},
        { "Бензин", 7500}, // petrol
        { "Электро", 7500}, // petrol
        { "Одежда", 50000}, // clothes
        { "Рюкзаки", 50000}, // clothes
        { "Маски", 10000}, // masks
        { "Запчасти", 300000}, // ls customs
        { "Средство для мытья", 200 }, // carwash
        { "Корм для животных", 20 }, // petshop
        { "Рюкзак", 10 },
        { "Бутылка воды", 100 },
        { "Рем. набор", 50},
        { "Малая аптечка", 50},

        //Premium
        {"alpha", 10},
        {"baller4", 10},
        {"btype", 10},
        {"btype2", 10},
        {"btype3", 10},
        {"casco", 10},
        {"cognoscenti", 10},
        {"cypher", 10},
        {"dominator3", 10},
        {"dominator7", 10},
        {"drafter", 10},
        {"dubsta", 10},
        {"dubsta2", 10},
        {"feltzer2", 10},
        {"hotknife", 10},
        {"huntley", 10},
        {"imorgon", 10},
        {"italigto", 10},
        {"jb7002", 10},
        {"jester3", 10},
        {"jester4", 10},
        {"jugular", 10},
        {"khamelion", 10},
        {"lynx", 10},
        {"neo", 10},
        {"neon", 10},
        {"ninef2", 10},
        {"paragon", 10},
        {"pariah", 10},
        {"patriot2", 10},
        {"raiden", 10},
        {"rapidgt", 10},
        {"rapidgt2", 10},
        {"rebla", 10},
        {"revolter", 10},
        {"sc1", 10},
        {"schafter3", 10},
        {"schafter4", 10},
        {"schlagen", 10},
        {"seven70", 10},
        {"specter", 10},
        {"stafford", 10},
        {"streiter", 10},
        {"stretch", 10},
        {"stinger", 10},
        {"sugoi", 10},
        {"superd", 10},
        {"surano", 10},
        {"swinger", 10},
        {"tailgater2", 10},
        {"tempesta", 10},
        {"thrax", 10},
        {"torero", 10},
        {"toros", 10},
        {"turismo2", 10},
        {"vacca", 10},
        {"vectre", 10},
        {"verlierer2", 10},
        {"voltic", 10},
        {"vstr", 10},
        {"windsor2", 10},
        {"dubsta3", 10},
        {"zion2", 10},

        //Sport
        {"adder", 10},
        {"autarch", 10},
        {"banshee", 10},
        {"banshee2", 10},
        {"bestiagts", 10},
        {"bullet", 10},
        {"carbonizzare", 10},
        {"cheetah", 10},
        {"cheetah2", 10},
        {"comet2", 10},
        {"comet3", 10},
        {"comet4", 10},
        {"comet5", 10},
        {"comet6", 10},
        {"coquette", 10},
        {"coquette2", 10},
        {"coquette3", 10},
        {"coquette4", 10},
        {"cyclone", 10},
        {"deveste", 10},
        {"deviant", 10},
        {"emerus", 10},
        {"entity2", 10},
        {"entityxf", 10},
        {"f620", 10},
        {"feltzer3", 10},
        {"fmj", 10},
        {"furia", 10},
        {"furoregt", 10},
        {"gp1", 10},
        {"growler", 10},
        {"gt500", 10},
        {"infernus", 10},
        {"infernus2", 10},
        {"issi7", 10},
        {"italigtb", 10},
        {"italirsx", 10},
        {"jester", 10},
        {"krieger", 10},
        {"locust", 10},
        {"mamba", 10},
        {"monroe", 10},
        {"nero", 10},
        {"ninef", 10},
        {"omnis", 10},
        {"osiris", 10},
        {"penetrator", 10},
        {"pfister811", 10},
        {"prototipo", 10},
        {"reaper", 10},
        {"ruston", 10},
        {"stingergt", 10},
        {"t20", 10},
        {"taipan", 10},
        {"tezeract", 10},
        {"tigon", 10},
        {"turismor", 10},
        {"tyrant", 10},
        {"tyrus", 10},
        {"vagner", 10},
        {"visione", 10},
        {"xa21", 10},
        {"zorrusso", 10},
        {"ztype", 10},
        {"zentorno", 10},

        //Middle
        {"asbo", 10},
        {"asea", 10},
        {"bfinjection", 10},
        {"bifta", 10},
        {"blade", 10},
        {"blista", 10},
        {"blista2", 10},
        {"blista3", 10},
        {"brioso", 10},
        {"brioso2", 10},
        {"buccaneer", 10},
        {"buccaneer2", 10},
        {"buffalo", 10},
        {"cavalcade", 10},
        {"cheburek", 10},
        {"chino", 10},
        {"chino2", 10},
        {"club", 10},
        {"dilettante", 10},
        {"dukes", 10},
        {"dynasty", 10},
        {"ellie", 10},
        {"emperor", 10},
        {"emperor2", 10},
        {"faction", 10},
        {"faction2", 10},
        {"fagaloa", 10},
        {"felon", 10},
        {"fugitive", 10},
        {"futo", 10},
        {"futo2", 10},
        {"gauntlet", 10},
        {"gauntlet3", 10},
        {"glendale", 10},
        {"glendale2", 10},
        {"habanero", 10},
        {"hellion", 10},
        {"impaler", 10},
        {"ingot", 10},
        {"intruder", 10},
        {"issi3", 10},
        {"kanjo", 10},
        {"landstalker", 10},
        {"landstalker2", 10},
        {"manana", 10},
        {"manana2", 10},
        {"mesa", 10},
        {"michelli", 10},
        {"minivan", 10},
        {"nebula", 10},
        {"oracle", 10},
        {"panto", 10},
        {"patriot", 10},
        {"peyote", 10},
        {"peyote3", 10},
        {"phoenix", 10},
        {"picador", 10},
        {"pigalle", 10},
        {"prairie", 10},
        {"premier", 10},
        {"primo", 10},
        {"radi", 10},
        {"rancherxl", 10},
        {"rebel", 10},
        {"regina", 10},
        {"retinue2", 10},
        {"rhapsody", 10},
        {"seminole2", 10},
        {"slamvan2", 10},
        {"stalion", 10},
        {"stanier", 10},
        {"stratum", 10},
        {"surfer2", 10},
        {"surfer", 10},
        {"tampa", 10},
        {"tornado", 10},
        {"tornado2", 10},
        {"tornado3", 10},
        {"tornado4", 10},
        {"tornado5", 10},
        {"vamos", 10},
        {"virgo2", 10},
        {"voodoo", 10},
        {"voodoo2", 10},
        {"washington", 10},
        {"weevil", 10},

        //standart
        {"asterope", 10},
        {"baller", 10},
        {"baller2", 10},
        {"baller3", 10},
        {"bjxl", 10},
        {"brawler", 10},
        {"buffalo2", 10},
        {"calico", 10},
        {"cavalcade2", 10},
        {"clique", 10},
        {"cog55", 10},
        {"cogcabrio", 10},
        {"dominator", 10},
        {"elegy", 10},
        {"elegy2", 10},
        {"euros", 10},
        {"exemplar", 10},
        {"felon2", 10},
        {"fq2", 10},
        {"freecrawler", 10},
        {"fusilade", 10},
        {"gauntlet4", 10},
        {"gauntlet5", 10},
        {"granger", 10},
        {"gresley", 10},
        {"hermes", 10},
        {"hustler", 10},
        {"imperator", 10},
        {"issi2", 10},
        {"jackal", 10},
        {"komoda", 10},
        {"kuruma", 10},
        {"massacro", 10},
        {"nightshade", 10},
        {"novak", 10},
        {"oracle2", 10},
        {"penumbra", 10},
        {"penumbra2", 10},
        {"previon", 10},
        {"rapidgt3", 10},
        {"remus", 10},
        {"retinue", 10},
        {"rocoto", 10},
        {"rt3000", 10},
        {"ruiner", 10},
        {"sabregt", 10},
        {"sabregt2", 10},
        {"savestra", 10},
        {"schafter2", 10},
        {"schwarzer", 10},
        {"seminole", 10},
        {"sentinel2", 10},
        {"sentinel3", 10},
        {"serrano", 10},
        {"sultan2", 10},
        {"sultan", 10},
        {"sultan3", 10},
        {"surge", 10},
        {"tailgater", 10},
        {"tropos", 10},
        {"tulip", 10},
        {"vigero", 10},
        {"virgo", 10},
        {"viseris", 10},
        {"warrener", 10},
        {"windsor", 10},
        {"xls", 10},
        {"yosemite", 10},
        {"zion3", 10},
        {"zr350", 10},
        {"cu2", 10},
        {"dwrangler", 10},
        {"jeep392", 10},
        {"mitgalant92", 10},
        // moto
        {"akuma", 10},
        {"avarus", 10},
        {"bagger", 10},
        {"bati", 10},
        {"bf400", 10},
        {"blazer", 10},
        {"blazer3", 10},
        {"blazer4", 10},
        {"carbonrs", 10},
        {"cliffhanger", 10},
        {"daemon", 10},
        {"daemon2", 10},
        {"deathbike", 10},
        {"deathbike2", 10},
        {"deathbike3", 10},
        {"defiler", 10},
        {"diablous", 10},
        {"diablous2", 10},
        {"double", 10},
        {"enduro", 10},
        {"esskey", 10},
        {"faggio", 10},
        {"faggio2", 10},
        {"faggio3", 10},
        {"fcr", 10},
        {"fcr2", 10},
        {"gargoyle", 10},
        {"hakuchou", 10},
        {"hakuchou2", 10},
        {"hexer", 10},
        {"innovation", 10},
        {"manchez", 10},
        {"manchez2", 10},
        {"nemesis", 10},
        {"nightblade", 10},
        {"pcj", 10},
        {"ruffian", 10},
        {"sanchez2", 10},
        {"sanctus", 10},
        {"shotaro", 10},
        {"sovereign", 10},
        {"stryder", 10},
        {"thrust", 10},
        {"vader", 10},
        {"verus", 10},
        {"vortex", 10},
        {"wolfsbane", 10},
        {"zombiea", 10},
        {"zombieb", 10},

        //грузовые
        {"bison", 10},
        {"bobcatxl", 10},
        {"bodhi2", 10},
        {"brutus", 10},
        {"burrito3", 10},
        {"caracara2", 10},
        {"contender", 10},
        {"dloader", 10},
        {"everon", 10},
        {"gburrito2", 10},
        {"kamacho", 10},
        {"moonbeam", 10},
        {"ratloader2", 10},
        {"rebel2", 10},
        {"riata", 10},
        {"sandking", 10},
        {"sandking2", 10},
        {"slamvan", 10},
        {"slamvan3", 10},
        {"speedo", 10},
        {"speedo4", 10},
        {"yosemite3", 10},
        {"youga2", 10},
        {"jimmy", 10},
        {"vwcaddy", 10},

        { "Buzzard2", 10 },
        { "Mammatus", 10 },
        { "Luxor2", 10 },

        {"bmwx7", 10},
        {"durango18", 10},
        {"evoque", 10},
        {"explorer", 10},
        {"g63amg6x6", 10},
        {"g65fresh", 10},
        {"glc2021", 10},
        {"gle63", 10},
        {"gls600", 10},
        {"lc200", 10},
        {"mbgls63", 10},
        {"volvoxc90", 10},
        {"bc5506d", 10},
        {"modelx", 10},
        {"aetron", 10},
        {"s600", 10},
        {"apriora", 10},
        {"m3e46", 10},
        {"w210", 10},
        {"bmwe38", 10},
        {"audia8", 10},
        {"camry18", 10},
        {"gcmsentra20", 10},
        {"kiastinger", 10},
        {"optima", 10},
        {"octavia18", 10},
        {"audia6", 10},
        {"bmwg20", 10},
        {"bmwm7", 10},
        {"c63coupe", 10},
        {"mere63amg", 10},
        {"rmodm8c", 10},
        {"rs72", 10},
        {"v447", 10},
        {"bmwm4", 10},
        {"ocnetrongt", 10},
        {"taycan", 10},
        {"teslas", 10},
        {"ae86", 10},
        {"bnr32", 10},
        {"bnr34", 10},
        {"boss302", 10},
        {"chevelle1970", 10},
        {"lrdef17", 10},
        {"rmodcharger69", 10},
        {"mark2", 10},
        {"supra", 10},
        {"rmodrs6", 10},
        {"benzsl63", 10},
        {"rmodbmwi8", 10},
        {"rmodlp570", 10},
        {"rmodmi8", 10},
        {"rmodsian", 10},
        {"rmodm5e34", 10},
        {"p1", 10},
        {"porsche2021", 10},
        {"rmodbentleygt", 10},
        {"f12berlinetta", 10},
        {"f458", 10},
        {"roma", 10},
        {"190e", 10},
        {"370z16", 10},
        {"bugatti", 10},
        {"chiron17", 10},
        {"benzc32", 10},
        {"gl450", 10},
        {"gtr", 10},
        {"modena99", 10},
        {"488pista", 10},
        {"urus",10 },
        {"f8t", 10},
        {"lp700", 10},
        {"600lt", 10},
        {"63amg", 10},
        {"m5e60", 10},
        {"m5", 10},
        {"x5om", 10},
        {"golf7r", 10},
        {"lancer", 10},
        {"dbx", 10},
        {"rculi", 10},
        {"exige12", 10},
        {"f812", 10},
        {"hevos", 10},
        {"m3e30", 10},
        {"z48", 10},
        {"rmodrover", 10},
        {"rmodg65", 10},
        {"rmodbiposto", 10},
        {"m5f90", 10},
        {"mcls53", 10},
        {"m4comp", 10},
        {"rmod240sx", 10},
        {"rmodi8ks", 10},
        {"rmodjeep", 10},
        {"rm3e36", 10},
        {"rmodamgc63", 10},
        {"rmodi8mlb", 10},
        {"rmodfordgt", 10},
        {"rmodgtr", 10},
        {"rmodlp750", 10},
        {"rmodm3e36", 10},
        {"rmodm4", 10},
        {"rmodm4gts", 10},
        {"rmodmi8lb", 10},
        {"rmodmustang", 10},
        {"rmodrs7", 10},
        {"rmodskyline", 10},
        {"rmodsupra", 10},
        {"rmodx6", 10},
        {"rmodsuprapandem", 10},
        {"rmodz350pandem", 10},
        {"hevo", 10},
        {"huracan", 10},
        {"rmodgt63", 10},
        {"cam8tun", 10},
        {"mbbs20", 10},
        {"pgt2", 10},
        {"svt00", 10},
        {"rs3",10 },
        {"w223",10 },
        {"cruzzz",10 },
        {"gt17",10 },
        {"sti",10 },
        {"panamera17turbo",10 },
        {"pwagon51",10 },
        {"2cv6naj",10 },
        {"vwbeetlenaj",10 },
        {"renault4",10 },
        {"simca1100",10 },
        {"forfalcxr",10 },
        {"impala72",10 },
        {"belair56",10 },
        {"zephyr41c",10 },
        {"ford34",10 },
        {"fleetline48",10 },
        {"celestem5",10 },
        {"ff1375",10 },
        {"jarama",10 },
        {"s15rb", 10},

        //novie tachki
        {"4runner", 10},
        {"16challenger", 10},
        {"21k5", 10},
        {"99viper", 10},
        {"ap2", 10},
        {"cobra", 10},
        {"fct", 10},
        {"m422", 10},
        {"mlmansory", 10},
        {"mustang68", 10},
        {"na6", 10},
        {"rs520", 10},
        {"rx811", 10},
        {"supraa90", 10},
        {"tricolore", 10},
        {"tur50", 10},
        {"wildtrak", 10},
        {"wraith", 10},
        {"x3gemwb", 10},

        { "Pistol", 50}, // gun shop
        { "Combatpistol", 50},
        { "Revolver", 50},
        { "Heavypistol", 50},
        { "Bullpupshotgun", 50},
        { "Combatpdw", 50},
        { "Machinepistol", 50},
        { "Патроны", 15000},
        { "Модификации", 5000},


        //Resale Cars
        {"бланк договора",500},


        #region FishShop
        { "Удочка", 10 },
        { "Удочка MK2", 10 },
        { "Наживка", 10000 },
        #endregion FishShop
        #region SellShop
        { "Корюшка", 1 },
        { "Кунджа", 1 },
        { "Лосось", 1 },
        { "Окунь", 1 },
        { "Осётр", 1 },
        { "Скат", 1 },
        { "Тунец", 1 },
        { "Угорь", 1 },
        { "Чёрный амур", 1 },
        { "Щука", 1 }
        #endregion SellShop
    };

    public static Dictionary<string, int> ProductsOrderPrice = new Dictionary<string, int>()
    {
        {"Расходники",50},
        {"Татуировки",20},
        {"Парики",20},
        {"Бургер",94},
        {"Хот-Дог",87},
        {"Сэндвич",50},
        {"eCola",90},
        {"Sprunk",150},
        {"Монтировка",200},
        {"Фонарик",240},
        {"Молоток",200},
        {"Гаечный ключ",200},
        {"Канистра бензина",460},
        {"Чипсы",94},
        {"Пицца",188},
        {"Сим-карта",460},
        //{"Связка ключей",960},
        {"Бензин",12},
        {"Одежда",50},
        {"Рюкзаки",50},
        {"Маски",700},
        {"Запчасти",100},
        {"Средство для мытья",200},
        {"Корм для животных", 450000 }, // petshop
        { "Рюкзак", 1460},
        {"Электро",2},
        {"Бутылка воды", 80 },
        {"Рем. набор", 15000 },
        {"Малая аптечка", 5000 },
        //standart
        {"asterope", 75000},
        {"issi2", 80000},
        {"seminole", 80000},
        {"virgo", 80000},
        {"bjxl", 90000},
        {"savestra", 90000},
        {"sentinel2", 90000},
        {"retinue", 100000},
        {"sabregt", 100000},
        {"sultan", 100000},
        {"granger", 110000},
        {"jackal", 110000},
        {"sabregt2", 110000},
        {"cavalcade2", 115000},
        {"fusilade", 120000},
        {"previon", 120000},
        {"sentinel3", 120000},
        {"tailgater", 125000},
        {"warrener", 125000},
        {"remus", 135000},
        {"surge", 135000},
        {"tulip", 135000},
        {"baller", 140000},
        {"fq2", 140000},
        {"nightshade", 140000},
        {"schwarzer", 140000},
        {"gresley", 150000},
        {"oracle2", 150000},
        {"serrano", 150000},
        {"brawler", 170000},
        {"felon2", 170000},
        {"sultan2", 170000},
        {"vigero", 170000},
        {"sultan3", 175000},
        {"clique", 180000},
        {"freecrawler", 180000},
        {"ruiner", 180000},
        {"baller2", 190000},
        {"baller3", 195000},
        {"calico", 200000},
        {"exemplar", 200000},
        {"rocoto", 200000},
        {"zr350", 205000},
        {"gauntlet4", 210000},
        {"penumbra", 210000},
        {"buffalo2", 215000},
        {"novak", 220000},
        {"imperator", 230000},
        {"tropos", 230000},
        {"schafter2", 235000},
        {"penumbra2", 240000},
        {"cogcabrio", 250000},
        {"hermes", 250000},
        {"rt3000", 250000},
        {"zion3", 270000},
        {"dominator", 275000},
        {"elegy", 275000},
        {"hustler", 275000},
        {"komoda", 280000},
        {"massacro", 280000},
        {"cog55", 300000},
        {"kuruma", 300000},
        {"xls", 315000},
        {"elegy2", 330000},
        {"euros", 330000},
        {"yosemite", 330000},
        {"rapidgt3", 350000},
        {"viseris", 400000},
        {"gauntlet5", 450000},
        {"windsor", 525000},
        {"cu2", 3000000},
        {"dwrangler", 4500000},
        {"jeep392", 5500000},
        {"mitgalant92", 1700000},

        //premium
        {"baller4", 650000},
        {"sc1", 700000},
        {"specter", 700000},
        {"alpha", 750000},
        {"tailgater2", 750000},
        {"cognoscenti", 800000},
        {"revolter", 800000},
        {"sugoi", 800000},
        {"seven70", 850000},
        {"dominator3", 875000},
        {"casco", 900000},
        {"hotknife", 900000},
        {"rapidgt", 900000},
        {"schafter4", 900000},
        {"voltic", 900000},
        {"zion2", 900000},
        {"cypher", 950000},
        {"khamelion", 950000},
        {"surano", 1000000},
        {"drafter", 1100000},
        {"jester3", 1100000},
        {"lynx", 1100000},
        {"rapidgt2", 1100000},
        {"schafter3", 1115000},
        {"dominator7", 1200000},
        {"imorgon", 1200000},
        {"feltzer2", 1250000},
        {"jester4", 1250000},
        {"vstr", 1250000},
        {"ninef2", 1300000},
        {"pariah", 1300000},
        {"rebla", 1300000},
        {"dubsta", 1350000},
        {"dubsta2", 1400000},
        {"jugular", 1400000},
        {"neon", 1400000},
        {"streiter", 1400000},
        {"vectre", 1435000},
        {"huntley", 1455000},
        {"paragon", 1500000},
        {"patriot2", 1600000},
        {"verlierer2", 1600000},
        {"turismo2", 1700000},
        {"vacca", 1700000},
        {"windsor2", 1700000},
        {"schlagen", 1735000},
        {"stretch", 1750000},
        {"raiden", 1800000},
        {"italigto", 2000000},
        {"neo", 2000000},
        {"superd", 2000000},
        {"toros", 2000000},
        {"jb7002", 2250000},
        {"stinger", 2300000},
        {"swinger", 2300000},
        {"tempesta", 2375000},
        {"thrax", 2500000},
        {"torero", 2600000},
        {"btype", 3500000},
        {"dubsta3", 3500000},
        {"btype3", 3750000},
        {"btype2", 4000000},
        {"stafford", 4000000},





        //эконом
        {"tornado4", 10000},
        {"voodoo2", 11000},
        {"tornado3", 13000},
        {"emperor2", 14000},
        {"brioso2", 15000},
        {"issi3", 15000},
        {"surfer2", 15000},
        {"regina", 16000},
        {"voodoo", 16000},
        {"ingot", 17000},
        {"fagaloa", 18000},
        {"asbo", 20000},
        {"emperor", 22000},
        {"tornado", 22000},
        {"asea", 23000},
        {"dynasty", 24000},
        {"blade", 25000},
        {"chino", 25000},
        {"surfer", 25000},
        {"tornado2", 25000},
        {"blista", 26000},
        {"intruder", 26000},
        {"manana", 27000},
        {"tornado5", 27000},
        {"blista2", 28000},
        {"brioso", 28000},
        {"glendale", 29000},
        {"blista3", 30000},
        {"panto", 30000},
        {"slamvan2", 30000},
        {"weevil", 30000},
        {"chino2", 31000},
        {"manana2", 31000},
        {"club", 32000},
        {"glendale2", 33000},
        {"seminole2", 33000},
        {"rancherxl", 35000},
        {"tampa", 35000},
        {"dilettante", 37000},
        {"nebula", 37000},
        {"prairie", 37000},
        {"premier", 37000},
        {"stalion", 37000},
        {"primo", 40000},
        {"rebel", 40000},
        {"rhapsody", 40000},
        {"ellie", 41000},
        {"stanier", 42000},
        {"buccaneer", 43000},
        {"dukes", 43000},
        {"faction", 43000},
        {"virgo2", 45000},
        {"faction2", 46000},
        {"buccaneer2", 48000},
        {"stratum", 48000},
        {"washington", 49000},
        {"bfinjection", 50000},
        {"futo", 53000},
        {"michelli", 55000},
        {"picador", 55000},
        {"buffalo", 58000},
        {"kanjo", 60000},
        {"vamos", 60000},
        {"futo2", 61000},
        {"peyote", 62000},
        {"minivan", 65000},
        {"radi", 65000},
        {"bifta", 70000},
        {"oracle", 70000},
        {"phoenix", 73000},
        {"gauntlet", 75000},
        {"peyote3", 75000},
        {"cheburek", 80000},
        {"mesa", 80000},
        {"gauntlet3", 83000},
        {"cavalcade", 92000},
        {"landstalker", 97000},
        {"habanero", 100000},
        {"impaler", 100000},
        {"fugitive", 105000},
        {"hellion", 105000},
        {"felon", 110000},
        {"patriot", 112000},
        {"landstalker2", 115000},
        {"retinue2", 120000},
        {"pigalle", 130000},





        //moto
        {"faggio3", 7500},
        {"faggio2", 5000},
        {"faggio", 10000},
        {"esskey", 100000},
        {"bagger", 120000},
        {"enduro", 100000},
        {"daemon2", 150000},
        {"wolfsbane", 150000},
        {"pcj", 150000},
        {"sanchez2", 120000},
        {"daemon", 170000},
        {"manchez", 80000},
        {"vader", 150000},
        {"blazer", 200000},
        {"diablous2", 300000},
        {"diablous", 170000},
        {"avarus", 150000},
        {"hexer", 150000},
        {"sovereign", 120000},
        {"blazer4", 300000},
        {"fcr", 150000},
        {"fcr2", 230000},
        {"ruffian", 150000},
        {"zombiea", 150000},
        {"zombieb", 170000},
        {"blazer3", 300000},
        {"innovation", 500000},
        {"nemesis", 130000},
        {"deathbike", 3000000},
        {"deathbike3", 3000000},
        {"thrust", 145000},
        {"verus", 300000},
        {"vortex", 140000},
        {"bati", 400000},
        {"sanctus", 5000000},
        {"defiler", 150000},
        {"gargoyle", 185000},
        {"stryder", 450000},
        {"double", 400000},
        {"akuma", 180000},
        {"carbonrs", 550000},
        {"nightblade", 500000},
        {"bf400", 150000},
        {"hakuchou", 500000},
        {"cliffhanger", 170000},
        {"hakuchou2", 600000},
        {"deathbike2", 3000000},
        {"manchez2", 200000},
        {"shotaro", 10000000},


        //sport
        {"deviant", 400000},
        {"issi7", 400000},
        {"banshee", 550000},
        {"omnis", 600000},
        {"f620", 610000},
        {"furoregt", 685000},
        {"banshee2", 700000},
        {"comet3", 700000},
        {"coquette", 700000},
        {"ruston", 730000},
        {"bullet", 750000},
        {"monroe", 775000},
        {"comet4", 800000},
        {"ninef", 820000},
        {"locust", 880000},
        {"comet2", 900000},
        {"infernus", 975000},
        {"jester", 975000},
        {"carbonizzare", 1000000},
        {"gt500", 1050000},
        {"stingergt", 1100000},
        {"mamba", 1175000},
        {"bestiagts", 1200000},
        {"comet5", 1200000},
        {"reaper", 1275000},
        {"gp1", 1315000},
        {"coquette2", 1350000},
        {"penetrator", 1450000},
        {"adder", 1500000},
        {"entityxf", 1500000},
        {"infernus2", 1530000},
        {"osiris", 1600000},
        {"comet6", 1650000},
        {"coquette3", 1650000},
        {"pfister811", 1650000},
        {"cheetah", 1700000},
        {"entity2", 1700000},
        {"fmj", 1730000},
        {"cyclone", 1800000},
        {"italigtb", 1800000},
        {"t20", 1900000},
        {"growler", 1925000},
        {"cheetah2", 2000000},
        {"tigon", 2050000},
        {"emerus", 2100000},
        {"krieger", 2100000},
        {"feltzer3", 2200000},
        {"coquette4", 2300000},
        {"italirsx", 2425000},
        {"turismor", 2500000},
        {"nero", 2600000},
        {"tyrant", 2600000},
        {"furia", 2675000},
        {"tyrus", 2750000},
        {"xa21", 2800000},
        {"taipan", 2850000},
        {"autarch", 3000000},
        {"deveste", 3000000},
        {"visione", 3000000},
        {"zorrusso", 3000000},
        {"prototipo", 3100000},
        {"vagner", 3200000},
        {"zentorno", 3250000},
        {"tezeract", 4200000},
        {"ztype", 5000000},



        //грузовые
        {"dloader", 115000},
        {"youga2", 160000},
        {"slamvan", 315000},
        {"slamvan3", 321000},
        {"ratloader2", 433000},
        {"kamacho", 542000},
        {"bodhi2", 692000},
        {"rebel2", 746000},
        {"sandking", 823000},
        {"bobcatxl", 850000},
        {"sandking2", 876000},
        {"bison", 970000},
        {"riata", 1013000},
        {"yosemite3", 1080000},
        {"moonbeam", 1189000},
        {"burrito3", 1232000},
        {"speedo", 1291000},
        {"gburrito2", 1321000},
        {"speedo4", 1374000},
        {"everon", 1587000},
        {"caracara2", 1654000},
        {"contender", 1730000},
        {"brutus", 1863000},
        {"vwcaddy",3500000 },
        {"jimmy", 8300000 },


        { "Buzzard2", 1800000 },
        { "Mammatus", 3000000 },
        { "Luxor2", 7500000 },

        //Auto Donate
        {"apriora", 160000},
        {"ae86", 320000},
        {"w210", 350000},
        {"benzc32", 375000},
        {"s600", 390000},
        {"bmwe38", 400000},
        {"mark2", 450000},
        {"gcmsentra20", 720000},
        {"audia8", 800000},
        {"rmodm5e34", 800000},
        {"octavia18", 840000},
        {"m3e46", 850000},
        {"optima", 880000},
        {"lancer", 890000},
        {"m3e30", 900000},
        {"lrdef17", 1000000},
        {"bnr32", 1080000},
        {"supra", 1100000},
        {"golf7r", 1150000},
        {"190e", 1250000},
        {"chevelle1970", 1450000},
        {"m5e60", 1470000},
        {"370z16", 1500000},
        {"kiastinger", 1500000},
        {"audia6", 1600000},
        {"boss302", 1680000},
        {"gl450", 1800000},
        {"camry18", 2100000},
        {"bmwg20", 2150000},
        {"rmodcharger69", 2250000},
        {"bmwm4", 2300000},
        {"bnr34", 3000000},
        {"evoque", 3250000},
        {"z48", 3500000},
        {"benzsl63", 3500000},
        {"volvoxc90", 3600000},
        {"glc2021", 3800000},
        {"lc200", 4500000},
        {"c63coupe", 4500000},
        {"m5", 5550000},
        {"durango18", 6000000},
        {"teslas", 6375000},
        {"rmodrs6", 6750000},
        {"mbgls63", 8250000},
        {"v447", 9000000},
        {"g65fresh", 10000000},
        {"bmwx7", 10500000},
        {"modena99", 10500000},
        {"rmodbmwi8", 10800000},
        {"rs72", 10950000},
        {"gtr", 11250000},
        {"porsche2021", 11500000},
        {"rmodm8c", 11700000},
        {"modelx", 12000000},
        {"dbx", 12000000},
        {"rmodlp570", 12000000},
        {"rmodbentleygt", 12125000},
        {"taycan", 12500000},
        {"bc5506d", 12500000},
        {"gls600", 13500000},
        {"600lt", 14375000},
        {"f458", 16250000},
        {"hevos", 16250000},
        {"488pista", 16875000},
        {"urus", 17000000},
        {"g63amg6x6", 18000000},
        {"f812", 18125000},
        {"p1", 18750000},
        {"lp700", 19375000},
        {"rculi", 20625000},
        {"f12berlinetta", 21250000},
        {"bugatti", 25000000},
        {"chiron17", 35000000},





        // Тюнинг-Ателье
        {"rmodbiposto", 850000},
        {"rmodm3e36", 1700000},
        {"svt00", 1800000},
        {"rmod240sx", 2400000},
        {"rmodz350pandem", 2800000},
        {"rmodsupra", 4000000},
        {"cam8tun", 4600000},
        {"rmodfordgt", 5500000},
        {"rmodsuprapandem", 5650000},
        {"m4comp", 6100000},
        {"rmodamgc63", 6200000},
        {"rmodm4gts", 7000000},
        {"rmodrover", 7300000},
        {"rmodmustang", 7500000},
        {"rmodjeep", 7750000},
        {"mcls53", 8000000},
        {"rmodgt63", 8000000},
        {"m5f90", 8500000},
        {"rmodx6", 8500000},
        {"rmodmi8lb", 9000000},
        {"rmodskyline", 13500000},
        {"pgt2", 15000000},
        {"hevo", 18000000},
        {"mbbs20", 22150000},
        {"rmodlp750", 23900000},
        {"s15rb", 7000000 },


        //Эксклюзив
        {"jesko", 71190000},
        {"agera11", 39595000},
        {"rmodatlantic", 93790000},
        {"rmodbacalar", 103960000},
        {"rmodbugatti", 113000000},
        {"rmodchiron300", 83170000},
        {"rmodessenza", 83620000},
        {"sian", 36160000},
        {"tesla", 29131000},
        {"countach", 50850000},
        {"bugatticentodieci", 113000000},
        {"eleanor", 4746000},
        {"forgt50020", 8814000},
        {"rolls08", 167800000},
        {"fxxkevo", 47460000},
        {"m1procar", 2825000},
        {"rmodlp770", 72320000},
        {"rmodpagani", 41810000},
        {"rmodveneno", 113000000},



        //Сторонние тачки
        {"rs3", 1125000},
        {"x5om", 1300000},
        {"exige12", 1400000},
        {"bmwm7", 1950000},
        {"mere63amg", 2750000},
        {"explorer", 3300000},
        {"w223", 9000000},
        {"ocnetrongt", 4300000},
        {"gle63", 4500000},
        {"cruzzz", 6750000},
        {"aetron", 4800000},
        {"rmodrs7", 4800000},
        {"rmodm4", 5300000},
        {"rmodmi8", 7800000},
        {"63amg", 8000000},
        {"rmodi8mlb", 8000000},
        {"rmodi8ks", 10000000},
        {"fenyr", 10000000},
        {"roma", 11000000},
        {"rmodg65", 12000000},
        {"rmodgtr", 12000000},
        {"huracan", 13500000},
        {"f8t", 14000000},
        {"f40", 20340000},
        {"divo", 200000000},
        {"gt17", 17000000},
        {"r820", 200000000},
        {"rs7c8", 200000000},
        {"sti", 2000000},
        {"panamera17turbo", 8000000},
        {"subwrx", 200000000},
        {"stalion2", 200000},
        {"buffalo3", 320000},
        {"rrocket", 570000},
        {"sultanrs", 870000},
        {"impaler4", 540000},
        {"pwagon51",170000 },
        {"2cv6naj",250000 },
        {"vwbeetlenaj",300000 },
        {"renault4",370000 },
        {"simca1100",420000 },
        {"forfalcxr",700000 },
        {"impala72",830000 },
        {"belair56",900000 },
        {"zephyr41c",970000 },
        {"ford34",1200000 },
        {"fleetline48",1000000 },
        {"celestem5",1500000 },
        {"ff1375",2300000 },
        {"jarama",2700000 },
        {"al18", 3200000},
        {"stradale18", 7000000},
        {"ocnlamtmc", 50000000},
        {"amg1", 200000000},
        {"avtr", 200000000},
        {"huragucci", 20000000},
        {"mercedessl63", 20000000},
        {"sennasmso", 20000000},
        {"slr", 20000000},
        {"500gtrlam", 1000000},
        {"monowheel", 70000},
        {"monza", 1500000},
        {"morgan", 200000},
        {"nc1", 500000},
        {"noire19wb", 2000000},
        {"singer", 350000},
        {"twizy", 50000},
        {"vclass21", 300000},
        {"x6mf16", 1500000},

        //novie tachki
        {"4runner", 7000},
        {"16challenger", 15000},
        {"21k5", 5000},
        {"99viper", 20000},
        {"ap2", 10000},
        {"cobra", 17000},
        {"fct", 30000},
        {"m422", 16000},
        {"mlmansory", 23000},
        {"mustang68", 13000},
        {"na6", 8000},
        {"rs520", 18000},
        {"rx811", 9000},
        {"supraa90", 17000},
        {"tricolore", 50000},
        {"tur50", 12000},
        {"wildtrak", 8000},
        {"wraith", 25000},
        {"x3gemwb", 35000},

        {"Pistol",2000},
        {"Combatpistol",3000},
        {"Revolver",12000},
        {"Heavypistol",6000},
        {"Bullpupshotgun",10000},
        {"Combatpdw",9000},
        {"Machinepistol",5000},
        {"Патроны",5},
        {"PistolAmmo", 5},
        {"SMGAmmo", 10},
        {"RiflesAmmo", 20},
        {"SniperAmmo", 140},
        {"ShotgunsAmmo", 10},
        {"Модификации",100},



        //Resale Cars
        {"бланк договора",300},

        #region FishShop
        { "Удочка", 2000 },
        { "Удочка MK2", 5000 },
        { "Удочка MK3", 9000 },
        { "Наживка", 10 },
        #endregion FishShop
        #region SellShop
        {"Корюшка",13},
        {"Кунджа",16},
        {"Лосось",10},
        {"Окунь",4},
        {"Осётр",5},
        {"Скат",12},
        {"Тунец",18},
        {"Угорь",5},
        {"Чёрный амур",15},
        {"Щука",6},
        #endregion SellShop
    };

    private static Dictionary<int, Tuple<int, int>> MinMaxMarkup = new Dictionary<int, Tuple<int, int>>()
    {
        {0, new Tuple<int, int>(50,150) },
        {1, new Tuple<int, int>(50,400) },
        {2, new Tuple<int, int>(50,150) },
        {3, new Tuple<int, int>(50,150) },
        {4, new Tuple<int, int>(50,150) },
        {5, new Tuple<int, int>(50,150) },
        {6, new Tuple<int, int>(50,150) },
        {7, new Tuple<int, int>(50,150) },
        {8, new Tuple<int, int>(50,400) },
        {9, new Tuple<int, int>(50,400) },
        {10, new Tuple<int, int>(50,400) },
        {11, new Tuple<int, int>(50,150) },
        {12, new Tuple<int, int>(50,150) },
        {13, new Tuple<int, int>(50,150) },
        {14, new Tuple<int, int>(50,150) },
        {15, new Tuple<int, int>(50,150) },
        {16, new Tuple<int, int>(50,150) },
        {17, new Tuple<int, int>(50,150) },
        {18, new Tuple<int, int>(50,150) },
        {19, new Tuple<int, int>(50,150) },
        {20, new Tuple<int, int>(50,150) },
        {21, new Tuple<int, int>(50,150) },
        {22, new Tuple<int, int>(50,150) },
        {23, new Tuple<int, int>(50,150) },
        {24, new Tuple<int, int>(50,150) },
        {25, new Tuple<int, int>(50,150) },
    };

    private static List<int> clothesOutgo = new List<int>()
    {
        1, // Головные уборы
        4, // Верхняя одежда
        3, // Нижняя одежда
        2, // Треники abibas
        1, // Кеды нike
    };
    #endregion

    public static List<Product> fillProductList(int type, bool full = false)
    {
      List<Product> products_list = new List<Product>();
      switch (type)
      {
        case 0:
          foreach (var name in MarketProducts)
          {
            Product product = new Product(ProductsOrderPrice[name], full ? ProductsCapacity[name] : 0, 1, name, false);
            products_list.Add(product);
          }
          break;
        case 1:
          products_list.Add(new Product(ProductsOrderPrice["Бензин"], full ? ProductsCapacity["Бензин"] : 0, 0, "Бензин", false));
          break;
        case 2:
          foreach (var name in CarsNames[0])
          {
            Product product = new Product(ProductsOrderPrice[name], full ? ProductsCapacity[name] : 0, 0, name, false);
            products_list.Add(product);
          }
          break;
        case 3:
          foreach (var name in CarsNames[1])
          {
            Product product = new Product(ProductsOrderPrice[name], full ? ProductsCapacity[name] : 0, 0, name, false);
            products_list.Add(product);
          }
          break;
        case 4:
          foreach (var name in CarsNames[2])
          {
            Product product = new Product(ProductsOrderPrice[name], full ? ProductsCapacity[name] : 0, 0, name, false);
            products_list.Add(product);
          }
          break;
        case 5:
          foreach (var name in CarsNames[3])
          {
            Product product = new Product(ProductsOrderPrice[name], full ? ProductsCapacity[name] : 0, 0, name, false);
            products_list.Add(product);
          }
          break;
        case 6:
          foreach (var name in GunNames)
          {
            Product product = new Product(ProductsOrderPrice[name], full ? ProductsCapacity[name] : 0, 5, name, false);
            products_list.Add(product);
          }
          products_list.Add(new Product(ProductsOrderPrice["Патроны"], full ? ProductsCapacity["Патроны"] : 0, 5, "Патроны", false));
          products_list.Add(new Product(ProductsOrderPrice["Модификации"], full ? ProductsCapacity["Модификации"] : 0, 5, "Модификации", false));
          break;
        case 7:
          products_list.Add(new Product(ProductsOrderPrice["Одежда"], full ? ProductsCapacity["Одежда"] : 0, 10, "Одежда", false));
          break;
        case 8:
          foreach (var name in BurgerProducts)
          {
            Product product = new Product(ProductsOrderPrice[name], 10, 1, name, false);
            products_list.Add(product);
          }
          break;
        case 9:
          products_list.Add(new Product(ProductsOrderPrice["Расходники"], full ? ProductsCapacity["Расходники"] : 0, 0, "Расходники", false));
          break;
        case 10:
          products_list.Add(new Product(ProductsOrderPrice["Расходники"], full ? ProductsCapacity["Расходники"] : 0, 0, "Расходники", false));
          break;
        case 11:
          products_list.Add(new Product(ProductsOrderPrice["Маски"], full ? ProductsCapacity["Маски"] : 0, 0, "Маски", false));
          break;
        case 12:
          products_list.Add(new Product(ProductsOrderPrice["Запчасти"], full ? ProductsCapacity["Запчасти"] : 0, 0, "Запчасти", false));
          break;
        case 13:
          products_list.Add(new Product(ProductsOrderPrice["Средство для мытья"], full ? ProductsCapacity["Средство для мытья"] : 0, 0, "Средство для мытья", false));
          break;
        case 14:
          products_list.Add(new Product(500000, full ? 1000 : 0, 0, "Корм для животных", false));
          break;
        case 15:
          foreach (var name in FishProducts)
          {
            Product product = new Product(ProductsOrderPrice[name], full ? ProductsCapacity[name] : 0, 1, name, false);
            products_list.Add(product);
          }
          break;
        case 16:
          /*foreach (var name in SellProducts)
          {
              Product product = new Product(ProductsOrderPrice[name], full ? ProductsCapacity[name] : 0, 1, name, false);
              products_list.Add(product);
          }*/
          break;
        case 17:
          foreach (var name in CarsNames[5])
          {
            try
            {
              Product product = new Product(ProductsOrderPrice[name], full ? ProductsCapacity[name] : 0, 0, name, false);
              products_list.Add(product);
            }
            catch (Exception ex) { Log.Debug("fillgov CarNames[5]: " + ex, nLog.Type.Error); }
          }
          break;
        case 18:
          products_list.Add(new Product(2000, full ? 1000 : 0, 0, "бланк договора", false));
          break;
        case 19:
          foreach (var name in CarsNames[6])
          {
            Product product = new Product(ProductsOrderPrice[name], full ? ProductsCapacity[name] : 0, 0, name, false);
            products_list.Add(product);
          }
          break;
        case 20:
          foreach (var name in CarsNames[7])
          {
            Product product = new Product(ProductsOrderPrice[name], full ? ProductsCapacity[name] : 0, 0, name, false);
            products_list.Add(product);
          }
          break;
        case 21:
          products_list.Add(new Product(ProductsOrderPrice["Электро"], full ? ProductsCapacity["Электро"] : 0, 0, "Электро", false));
          break;
        case 22:
          foreach (var name in CarsNames[8])
          {
            try
            {
              Product product = new Product(ProductsOrderPrice[name], full ? ProductsCapacity[name] : 0, 0, name, false);
              products_list.Add(product);
            }
            catch (Exception ex) { Log.Write(ex.StackTrace, nLog.Type.Error); }
          }
          break;
        case 23:
          products_list.Add(new Product(100, full ? ProductsOrderPrice["Рюкзаки"] : 0, 0, "Рюкзаки", false));
          break;
        case 24:
          foreach (var name in CarsNames[9])
          {
            Product product = new Product(ProductsOrderPrice[name], full ? ProductsCapacity[name] : 0, 0, name, false);
            products_list.Add(product);
          }
          break;
        case 25:
          products_list.Add(new Product(ProductsOrderPrice["Одежда"], full ? ProductsCapacity["Одежда"] : 0, 10, "Одежда", false));
          break;
      }
      return products_list;
    }

    [Command("fillprods", Hide = true)]
    public static void FillProducts(Player player)
    {
      if (!Group.CanUseCmd(player, "fillprods")) return;

      foreach (Business biz in BusinessManager.BizList.Values.ToList())
      {
        biz.Products = fillProductList(biz.Type);
      }
    }

    public static void interactionPressedBussinessMenu(Player player, int id)
    {
      switch (id)
      {
        case 1001:
          {
            try
            {
              if (player.GetMyData<int>("BUSSINESSMANAGE_ID") == -1) return;
              if (player.HasMyData("FOLLOWING"))
              {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вас кто-то тащит за собой", 3000);
                return;
              }

              Business bussiness = BizList[player.GetMyData<int>("BUSSINESSMANAGE_ID")];

              if (bussiness.Owner != "Государство" && !Main.PlayerNames.ContainsValue(bussiness.Owner))
              {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Данный {BusinessTypeNames[bussiness.Type]} в данный момент не работает", 3000);
                return;
              }

              if (bussiness.Owner == player.Name)
              {
                OpenBussinessManageMenu(player);
              }
              else
              {
                int Bizid = BizList[player.GetMyData<int>("BUSSINESSMANAGE_ID")].Type;
                if (Bizid != 2 && Bizid != 3 && Bizid != 17 && Bizid != 19 && Bizid != 21 && Bizid != 24 && Bizid != 25)
                {
                  OpenBussinessBuyMenu(player);
                }
                else
                {
                  Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Данный бизнес не доступен для покупки.");
                }
              }
            }
            catch (Exception e) { Log.Write("enter bussiness: message " + e.StackTrace + " method " + e.TargetSite + " source " + e.Source + " stack " + e.StackTrace, nLog.Type.Error); }
            return;
          }

      }
    }
    public static bool BizTypeHasOwner(Business biz)
    {
      foreach (var item in BizList)
      {
        if (item.Value.Type == biz.Type && item.Value.Owner != "Государство")
          return true;
      }
      return false;
    }
    public static void interactionPressed(Player player)
    {
      if (player.GetMyData<int>("BIZ_ID") == -1) return;
      if (player.HasMyData("FOLLOWING"))
      {
        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вас кто-то тащит за собой", 3000);
        return;
      }

      Business biz = BizList[player.GetMyData<int>("BIZ_ID")];

      if (biz.Owner == "Государство" && BizTypeHasOwner(biz))
      {
        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Данный {BusinessTypeNames[biz.Type]} в данный момент не работает", 3000);
        return;
      }

      switch (biz.Type)
      {
        case 0:
          Shop.openShop(player);
          return;
        case 1:
          if (!player.IsInVehicle) return;
          Vehicle vehicle = player.Vehicle;
          if (vehicle == null) return; //check
          if (player.VehicleSeat != 0) return;
          if (Core.VehicleStreaming.GetEngineState(vehicle))
          {
            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Для начала заглушите транспорт.", 3000);
            return;
          }
          OpenPetrolMenu(player);
          return;
        case 2:
        case 3:
        case 4:
        case 5:
        case 17:
        case 19:
        case 20:
        case 22:
        case 24:
          if (player.HasMyData("FOLLOWER"))
          {
            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Отпустите человека", 3000);
            return;
          }
          player.SetMyData("CARROOMID", biz.ID);
          CarRoom.EnterCarRoom(player);
          return;
        case 6:
          player.SetMyData("GUNSHOP", biz.ID);
          Ammunation.OpenGunShopMenu(player);
          return;
        case 7:
        case 25:
          ClothesShop.openClothesShopMenu(player, biz);
          return;
        case 8:
          Shop.openShop(player);
          return;
        case 9:
          TattooSaloon.OpenTattooMenu(player, biz);
          return;
        case 10:
          BarberShop.openBarberShopMenu(player, biz);
          return;
        case 11:
          MaskShop.openMaskShop(player, biz);
          return;
        case 12:
          Tuning.interactionPressed(player);
          return;
        case 13:
          if (!player.IsInVehicle)
          {
            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы должны находиться в машине", 3000);
            return;
          }
          Trigger.ClientEvent(player, "popup::open", "CARWASH_PAY", $"Вы хотите помыть машину за ${biz.Products[0].Price}$?");
          return;
        case 14:
          if (player.HasMyData("FOLLOWER"))
          {
            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Отпустите человека", 3000);
            return;
          }
          player.SetMyData("PETSHOPID", biz.ID);
          enterPetShop(player, biz.Products[0].Name);
          return;
        case 15:
          Shop.openShop(player);
          return;
        case 16:
          //RodManager.OpenBizSellShopMenu(player);
          return;
        case 18:
          Resalecar.interactionPressed(player);
          return;
        case 21:
          if (!player.IsInVehicle) return;
          vehicle = player.Vehicle;
          if (vehicle == null) return; //check
          if (player.VehicleSeat != 0) return;
          OpenPetrolMenu(player);
          return;
        case 23:
          BagsShop.openBagsShop(player, biz);
          return;
      }
    }

    public static bool takeProd(int bizid, int amount, string prodname, int addMoney)
    {
      try
      {

        Business biz = BizList[bizid];

        if (biz.Type == 17 || biz.Type == 22 || biz.Type == 24 || biz.Type == 25) return true; // донат автосалон и тюнинг-ателье DonateAutoSloon DonateClothesShop

        foreach (var p in biz.Products)
        {
          if (p.Name != prodname) continue;
          if (p.Lefts - amount < 0)
            return false;

          p.Lefts -= amount;

          if (biz.Owner == "Государство") break;
          Bank.Data bData = Bank.Get(Main.PlayerBankAccs[biz.Owner]);
          if (bData.ID == 0)
          {
            Log.Write($"TakeProd BankID error: {bizid.ToString()}({biz.Owner}) {amount.ToString()} {prodname} {addMoney.ToString()}", nLog.Type.Error);
            return false;
          }
          /*if (!Bank.Change(bData.ID, addMoney, false))
          {
              Log.Write($"TakeProd error: {bizid.ToString()}({biz.Owner}) {amount.ToString()} {prodname} {addMoney.ToString()}", nLog.Type.Error);
              return false;
          }*/

          var prod = biz.Products.FirstOrDefault(p => p.Name == prodname);
          var govMoney = prod.Price * amount; // 100 * 754 = 75400 // ЦЕНА при 100% наценке.

          int tax = biz.TakeTax(govMoney);

          //tax = 75400 / 100 * 15 = 11 310
          //tax = 113100 / 100 * 15 = 16 965
          //tax = 287468 / 100 * 15 = 43 120

          //113100 - 11310 = 101 790 - 75400 = 26 390;
          biz.Cash += addMoney - tax;
          biz.Profit[DateTime.Now.Date.Day - 1] += addMoney - tax;
          GameLog.Money($"biz({bizid})", $"cash", addMoney - tax, $"bizProfit Markup: {biz.Markup}");
          Log.Write($"{biz.Owner}'s business get {addMoney - tax}$ for '{prodname}'", nLog.Type.Success);
          break;
        }
        return true;
      }
      catch
      {
        return false;
      }
    }

    public static int getPriceOfProd(int bizid, string prodname)
    {
      Business biz = BizList[bizid];
      var price = 0;
      foreach (var p in biz.Products)
      {
        if (p.Name == prodname)
        {
          price = p.Price;
          break;
        }
      }
      return price;
    }

    public static Vector3 getNearestBiz(Player player, int type)
    {
      Vector3 nearestBiz = new Vector3();
      foreach (var b in BizList)
      {
        Business biz = BizList[b.Key];
        if (biz.Type != type) continue;
        if (nearestBiz == null) nearestBiz = biz.EnterPoint;
        if (player.Position.DistanceTo(biz.EnterPoint) < player.Position.DistanceTo(nearestBiz))
          nearestBiz = biz.EnterPoint;
      }
      return nearestBiz;
    }
    public static void enterPetShop(Player player, string prodname)
    {
      Main.Players[player].ExteriorPos = player.Position;
      uint mydim = (uint)(player.Value + 500);
      NAPI.Entity.SetEntityDimension(player, mydim);
      NAPI.Entity.SetEntityPosition(player, new Vector3(-758.3929, 319.5044, 175.302));
      player.PlayAnimation("amb@world_human_sunbathe@male@back@base", "base", 39);
      //player.FreezePosition = true;
      player.SetMyData("INTERACTIONCHECK", 0);
      var prices = new List<int>();
      Business biz = BusinessManager.BizList[player.GetMyData<int>("PETSHOPID")];
      for (byte i = 0; i != 9; i++)
      {
        prices.Add(biz.GetPriceWithMarkUpInt(biz.Products[0].Price));
      }
      Trigger.ClientEvent(player, "openPetshop", JsonConvert.SerializeObject(PetNames), JsonConvert.SerializeObject(PetHashes), JsonConvert.SerializeObject(prices), mydim);
    }

    [RemoteEvent("petshopBuy")]
    public static void RemoteEvent_petshopBuy(Player player, string petName)
    {
      try
      {
        player.StopAnimation();
        Business biz = BusinessManager.BizList[player.GetMyData<int>("PETSHOPID")];
        NAPI.Entity.SetEntityPosition(player, new Vector3(biz.EnterPoint.X, biz.EnterPoint.Y, biz.EnterPoint.Z + 1.5));
        //player.FreezePosition = false;
        NAPI.Entity.SetEntityDimension(player, 0);
        Main.Players[player].ExteriorPos = new Vector3();
        Trigger.ClientEvent(player, "destroyCamera");
        Dimensions.DismissPrivateDimension(player);

        Houses.House house = Houses.HouseManager.GetHouse(player, true);
        if (house == null)
        {
          Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У Вас нет личного дома", 3000);
          return;
        }
        if (Houses.HouseManager.HouseTypeList[house.Type].PetPosition == null)
        {
          Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Ваше место проживания не подходит для жизни питомцев", 3000);
          return;
        }
        int price = biz.GetPriceWithMarkUpInt(biz.Products[0].Price);
        if (Main.Players[player].Money < price)
        {
          Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Недостаточно средств", 3000);
          return;
        }

        var prod = biz.Products.FirstOrDefault(p => p.Name == "Корм для животных");
        var amount = price / biz.GetPriceWithMarkUpInt(prod.Price);
        if (amount <= 0) amount = 1;

        if (!BusinessManager.takeProd(biz.ID, amount, "Корм для животных", price))
        {
          Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "К сожалению, питомцев данного пола пока что нет в магазине", 3000);
          return;
        }
        MoneySystem.Wallet.Change(player, -price);
        GameLog.Money($"player({Main.Players[player].UUID})", $"biz({biz.ID})", price, $"buyPet({petName})");
        house.PetName = petName;
        Main.Players[player].PetName = petName;
        Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Теперь Вы являетесь счастливым хозяином {petName}!", 3000);
      }
      catch (Exception e) { Log.Write("PetshopBuy: " + e.StackTrace, nLog.Type.Error); }
    }

    [RemoteEvent("petshopCancel")]
    public static void RemoteEvent_petshopCancel(Player player)
    {
      try
      {
        if (!player.HasMyData("PETSHOPID")) return;
        player.StopAnimation();
        var enterPoint = BusinessManager.BizList[player.GetMyData<int>("PETSHOPID")].EnterPoint;
        NAPI.Entity.SetEntityDimension(player, 0);
        NAPI.Entity.SetEntityPosition(player, new Vector3(enterPoint.X, enterPoint.Y, enterPoint.Z + 1.5));
        Main.Players[player].ExteriorPos = new Vector3();
        //player.FreezePosition = false;
        Dimensions.DismissPrivateDimension(player);
        player.ResetMyData("PETSHOPID");
        Trigger.ClientEvent(player, "destroyCamera");
      }
      catch (Exception e) { Log.Write("petshopCancel: " + e.StackTrace, nLog.Type.Error); }
    }

    public static void Carwash_Pay(Player player)
    {
      try
      {
        if (player.GetMyData<int>("BIZ_ID") == -1) return;
        Business biz = BizList[player.GetMyData<int>("BIZ_ID")];

        if (player.IsInVehicle)
        {
          if (player.VehicleSeat == 0)
          {
            if (VehicleStreaming.GetVehicleDirt(player.Vehicle) >= 0f)
            {
              int price = biz.GetPriceWithMarkUpInt(biz.Products[0].Price);
              if (Main.Players[player].Money < price)
              {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Недостаточно средств", 3000);
                return;
              }

              var prod = biz.Products.FirstOrDefault(p => p.Name == "Средство для мытья");
              var amount = price / biz.GetPriceWithMarkUpInt(prod.Price);
              if (amount <= 0) amount = 1;

              if (!takeProd(biz.ID, amount, prod.Name, price))
              {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Недостаточно товара на складе", 3000);
                return;
              }
              GameLog.Money($"player({Main.Players[player].UUID})", $"biz({biz.ID})", price, "carwash");
              MoneySystem.Wallet.Change(player, -price);

              VehicleStreaming.SetVehicleDirt(player.Vehicle, 0.0f);
              Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Ваш транспорт был помыт.", 3000);

              #region BPКвест: 55 Помыть автомобиль в мойке.

              #region BattlePass выполнение квеста
              BattlePass.updateBPQuestIteration(player, BattlePass.BPQuestType.WashCar);
              #endregion

              #endregion

              #region GBPКвест: 14 Воспользоваться автомойкой 500 раз.

              #region BattlePass выполнение квеста
              BattlePass.updateBPGlobalQuestIteration(player, BattlePass.BPGlobalQuestType.WashCar);
              #endregion

              #endregion
            }
            else
              Notify.Send(player, NotifyType.Alert, NotifyPosition.BottomCenter, "У вас чистый транспорт.", 3000);
          }
          else
            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Мыть транспорт может только водитель.", 3000);
        }
        return;
      }
      catch (Exception e)
      {
        Log.Write(e.ToString(), nLog.Type.Error);
        return;
      }
    }

    [RemoteEvent("petrol")]
    public static void fillCar(Player player, string petrol, string cardOrMoney)
    {
      try
      {
        if (player == null || !Main.Players.ContainsKey(player)) return;
        Vehicle vehicle = player.Vehicle;
        if (vehicle == null) return; //check
        if (player.VehicleSeat != 0) return;
        uint carModel = vehicle.Model;
        string carModelName = VehicleManager.VehicleFuel.Keys.FirstOrDefault(s => NAPI.Util.GetHashKey(s) == carModel);
        int lvl = 0;
        if (!int.TryParse(Convert.ToString(petrol), out lvl))
        {
          Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Возникла ошибка! Попробуйте еще раз.", 3000);
          return;
        }

        if (lvl <= 0)
        {
          Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Введите корректные данные", 3000);
          return;
        }
        if (!vehicle.HasSharedData("PETROL"))
        {
          Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Невозможно заправить эту машину", 3000);
          return;
        }
        if (Core.VehicleStreaming.GetEngineState(vehicle))
        {
          Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Для начала заглушите транспорт.", 3000);
          return;
        }

        int fuel = vehicle.GetSharedData<int>("PETROL");
        if (carModelName != null && VehicleManager.VehicleFuel.ContainsKey(carModelName))
        {
          if (fuel >= VehicleManager.VehicleFuel[carModelName])
          {
            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У транспорта полный бак", 3000);
            return;
          }
        }
        else if (fuel >= VehicleManager.VehicleTank[vehicle.Class])
        {
          Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У транспорта полный бак", 3000);
          return;
        }

        var isGov = false;
        if (lvl == 9999)
          lvl = VehicleManager.VehicleTank[vehicle.Class] - fuel;
        else if (lvl == 99999)
        {
          isGov = true;
          lvl = VehicleManager.VehicleTank[vehicle.Class] - fuel;
        }

        if (lvl < 0) return;

        int tfuel = fuel + lvl;

        if (carModelName != null && VehicleManager.VehicleFuel.ContainsKey(carModelName))
        {
          if (tfuel > VehicleManager.VehicleFuel[carModelName])
          {
            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Введите корректные данные", 3000);
            return;
          }
        }
        else
        {
          if (tfuel > VehicleManager.VehicleTank[vehicle.Class])
          {
            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Введите корректные данные", 3000);
            return;
          }
        }
        Business biz = BizList[player.GetMyData<int>("BIZ_ID")];
        int bankMoney = (int)MoneySystem.Bank.Get(Main.Players[player].Bank).Balance;
        int price = lvl * biz.GetPriceWithMarkUpInt(biz.Products[0].Price);
        if (isGov)
        {
          int frac = Main.Players[player].Fraction.FractionID;
          if (Fractions.Manager.FractionTypes[frac] != 2)
          {
            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Чтобы заправить транспорт за гос. счет, Вы должны состоять в гос. организации", 3000);
            return;
          }
          if (!vehicle.HasData("ACCESS") || vehicle.GetData<string>("ACCESS") != "FRACTION" || vehicle.GetData<int>("FRACTION") != frac)
          {
            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы не можете заправить за государственный счет не государственный транспорт", 3000);
            return;
          }
        }
        else
        {
          if (Main.Players[player].Money < price && cardOrMoney == "money")
          {
            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Недостаточно средств (не хватает {lvl * biz.Products[0].Price - Main.Players[player].Money}$)", 3000);
            return;
          }

          if (cardOrMoney == "card")
          {
            if (bankMoney < price)
            {
              Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Недостаточно средств. Денег на карте: {bankMoney}$", 3000);
              return;
            }
          }
        }
        bool isElectro = false;
        if (electroCars.Exists(s => NAPI.Util.GetHashKey(s) == carModel))
            isElectro = true;

        Product prod = new Product(1,1,1,"1",false);
        if(isElectro)
            prod = biz.Products.FirstOrDefault(p => p.Name == "Электро");
        else
            prod = biz.Products.FirstOrDefault(p => p.Name == "Бензин");

        var amount = price / biz.GetPriceWithMarkUpInt(prod.Price);
        if (amount <= 0) amount = 1;

        if (!takeProd(biz.ID, amount, prod.Name, price))
        {
          if(!isElectro)
            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"На заправке осталось {biz.Products[0].Lefts}л", 3000);
          else
            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"На заправке осталось {biz.Products[0].Lefts}КВ", 3000);
          return;
        }
        if (isGov)
        {
          if (client.Fractions.Utils.Stocks.fracStocks[(int)Fractions.Manager.FractionTypesEnum.Cityhall].Money < price)
          {
            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У фракции недостаточно средств для оплаты", 3000);
            return;
          }
          GameLog.Money($"frac(6)", $"biz({biz.ID})", price, "buyPetrol");
          client.Fractions.Utils.Stocks.fracStocks[(int)Fractions.Manager.FractionTypesEnum.Cityhall].Money -= price;
        }
        else
        {
          if (cardOrMoney == "money")
          {
            GameLog.Money($"player({Main.Players[player].UUID})", $"biz({biz.ID})", price, "buyPetrol");
            MoneySystem.Wallet.Change(player, -price);
          }
          else
          {
            GameLog.Money($"player({Main.Players[player].UUID})", $"biz({biz.ID})", price, "buyPetrol");
            MoneySystem.Bank.Change(Main.Players[player].Bank, -price);
          }
        }

        vehicle.SetSharedData("PETROL", tfuel);

        if (NAPI.Data.GetEntityData(vehicle, "ACCESS") == "PERSONAL")
        {
          var id = vehicle.GetData<int>("ID");
          var number = NAPI.Vehicle.GetVehicleNumberPlate(vehicle);
          VehicleManager.Vehicles[id].Fuel += lvl;
        }

        #region BPКвест: 114  Заправить авто на X долларов.

        #region BattlePass выполнение квеста
        BattlePass.updateBPQuestIteration(player, BattlePass.BPQuestType.SpendMoneyOnFuel, price);
        #endregion

        #endregion

        #region SBPКвест: 17 Заправить авто суммарно на 300.000 долларов.

        #region BattlePass выполнение квеста
        BattlePass.updateBPSuperQuestIteration(player, BattlePass.BPSuperQuestType.BuyFuelOnCountMoney, price);
        #endregion

        #endregion

        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Транспорт заправлен", 3000);
        Commands.RPChat("me", player, $"заправил(а) транспортное средство");
      }
      catch (Exception e) { Log.Write("Petrol: " + e.StackTrace, nLog.Type.Error); }
    }

    public static void bizNewPrice(Player player, int price, int BizID)
    {
      if (!Main.Players[player].BizIDs.Contains(BizID)) return;
      Business biz = BizList[BizID];
      var prodName = player.GetMyData<string>("SELECTPROD");

      double minPrice = (biz.Type == 7 || biz.Type == 11 || biz.Type == 12 || prodName == "Татуировки" || prodName == "Парики"
          || prodName == "Патроны") ? 80 : (biz.Type == 1) ? 2 : ProductsOrderPrice[player.GetMyData<string>("SELECTPROD")] * 0.8;
      double maxPrice = (biz.Type == 7 || biz.Type == 11 || biz.Type == 12 || prodName == "Татуировки" || prodName == "Парики"
          || prodName == "Патроны") ? 150 : (biz.Type == 1) ? 7 : ProductsOrderPrice[player.GetMyData<string>("SELECTPROD")] * 1.2;

      if (price < minPrice || price > maxPrice)
      {
        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Невозможно установить такую цену", 3000);
        OpenBizProductsMenu(player);
        return;
      }
      foreach (var p in biz.Products)
      {
        if (p.Name == prodName)
        {
          p.Price = price;
          string ch = (biz.Type == 7 || biz.Type == 11 || biz.Type == 12 || p.Name == "Татуировки" || p.Name == "Парики") ? "%" : "$";

          Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Теперь {p.Name} стоит {p.Price}{ch}", 3000);
          if (p.Name == "Бензин") biz.UpdateLabel();
          OpenBizProductsMenu(player);
          return;
        }
      }
    }

    public static void bizOrder(Player player, int amount, int BizID)
    {
      if (!Main.Players[player].BizIDs.Contains(BizID)) return;
      Business biz = BizList[BizID];

      if (amount < 1)
      {
        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Неверное значение", 3000);
        OpenBizProductsMenu(player);
        return;
      }

      foreach (var p in biz.Products)
      {
        if (p.Name == player.GetMyData<string>("SELECTPROD"))
        {
          if (p.Ordered)
          {
            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы уже заказали этот товар", 3000);
            OpenBizProductsMenu(player);
            return;
          }

          if (biz.Type >= 2 && biz.Type <= 5)
          {
            if (amount > 10)
            {
              Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Укажите значение от 1 до 10", 3000);
              OpenBizProductsMenu(player);
              return;
            }
          }
          else if (biz.Type == 17 || biz.Type == 19 || biz.Type == 20)
          {
            if (amount > 10)
            {
              Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Укажите значение от 1 до 10", 3000);
              OpenBizProductsMenu(player);
              return;
            }
          }
          else if (biz.Type == 14)
          {
            if (amount < 1 || p.Lefts + amount > ProductsCapacity[p.Name])
            {
              Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Укажите значение от 1 до {ProductsCapacity[p.Name] - p.Lefts}", 3000);
              OpenBizProductsMenu(player);
              return;
            }
          }
          else
          {
            if (amount < 10 || p.Lefts + amount > ProductsCapacity[p.Name])
            {
              var text = "";
              if (ProductsCapacity[p.Name] - p.Lefts < 10) text = "У Вас достаточно товаров на складе";
              else text = $"Укажите от 10 до {ProductsCapacity[p.Name] - p.Lefts}";

              Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, text, 3000);
              OpenBizProductsMenu(player);
              return;
            }
          }

          var price = (p.Name == "Патроны") ? 4 : ProductsOrderPrice[p.Name];
          double orderPriceByProduct = 5000 / biz.Products.Count;
          int finishPrice = (amount * price) + (int)Math.Round(orderPriceByProduct);
          if (!Bank.Change(Main.Players[player].Bank, -finishPrice))
          {
            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Недостаточно средств на счету", 3000);
            return;
          }
          GameLog.Money($"bank({Main.Players[player].Bank})", $"server", finishPrice, "bizOrder");
          var order = new Order(p.Name, amount, false);
          p.Ordered = true;

          var random = new Random();
          do
          {
            order.UID = random.Next(000000, 999999);
          } while (BusinessManager.Orders.ContainsKey(order.UID));

          BusinessManager.Orders.Add(order.UID, biz.ID);
          biz.Orders.Add(order);

          Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы заказали {p.Name} в количестве {amount}. №{order.UID}", 3000);
          player.SendChatMessage($"Номер Вашего заказа: {order.UID}");
          return;
        }
      }
    }

    // заказ инкассации кассы
    public static void bizOrderCollector(Player player, int BizID)
    {
      if (!Main.Players[player].BizIDs.Contains(BizID)) return;
      Business biz = BizList[BizID];

      if (biz.CollectorsOrder != null)
      {
        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У вас уже есть заказ на инкассацию", 3000);
        //OpenBizProductsMenu(player);
        return;
      }

      if (biz.Cash <= 1000)
      {
        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"В кассе недостаточно денег", 3000);
        return;
      }

      CollectorOrder collectorOrder = new CollectorOrder();

      var random = new Random();
      do
      {
        collectorOrder.UID = random.Next(000000, 999999);
      } while (BusinessManager.Collections.ContainsKey(collectorOrder.UID));

      BusinessManager.Collections.Add(collectorOrder.UID, biz.ID);

      biz.CollectorsOrder = collectorOrder;

      Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы заказали услуги инкассации", 3000);
      player.SendChatMessage($"Номер Вашего заказа: {collectorOrder.UID}");

    }

    // отмена заказа инкассации
    public static void cancelCollectorOrder(Player player, int bizId)
    {
      if (!Main.Players[player].BizIDs.Contains(bizId)) return;
      Business biz = BizList[bizId];

      if (biz.CollectorsOrder == null)
      {
        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У вас нет заказа на инкассацию", 3000);
        //OpenBizProductsMenu(player);
        return;
      }

      if (biz.CollectorsOrder.Taked)
      {
        Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Инкассация уже выполняется", 3000);
        //OpenBizProductsMenu(player);
        return;
      }

      BusinessManager.Collections.Remove(biz.CollectorsOrder.UID);
      biz.CollectorsOrder = null;

      Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы отменили заказ на инкассацию.", 3000);

      return;
    }

    public static void createBusinessCommand(Player player, int govPrice, int type)
    {
      if (!Group.CanUseCmd(player, "createbusiness")) return;
      var pos = player.Position;
      pos.Z -= 1.12F;
      string productlist = "";
      List<Product> products_list = BusinessManager.fillProductList(type);
      productlist = JsonConvert.SerializeObject(products_list);
      lastBizID++;

      var bankID = MoneySystem.Bank.Create("", 3, 1000);
      //MySQL.Query($"INSERT INTO businesses (id, owner, sellprice, type, products, enterpoint, unloadpoint, money, mafia, orders, managepoint) " +
      //    $"VALUES ({lastBizID}, 'Государство', {govPrice}, {type}, '{productlist}', '{JsonConvert.SerializeObject(pos)}', '{JsonConvert.SerializeObject(new Vector3())}', {bankID}, -1, '{JsonConvert.SerializeObject(new List<Order>())}', '{JsonConvert.SerializeObject(new Vector3())}')");

      MySqlCommand cmd = new MySqlCommand();
      cmd.CommandText = "INSERT INTO `businesses` SET " +
        "`id`=@id," +
        "`owner`=@owner," +
        "`sellprice`=@sellprice," +
        "`type`=@type," +
        "`products`=@products," +
        "`enterpoint`=@enterpoint," +
        "`unloadpoint`=@unloadpoint," +
        "`money`=@money," +
        "`mafia`=@mafia," +
        "`orders`=@orders," +
        "`managepoint`=@managepoint";

      cmd.Parameters.AddWithValue("@id", lastBizID);
      cmd.Parameters.AddWithValue("@owner", "Государство");
      cmd.Parameters.AddWithValue("@sellprice", govPrice);
      cmd.Parameters.AddWithValue("@type", type);
      cmd.Parameters.AddWithValue("@products", productlist);
      cmd.Parameters.AddWithValue("@enterpoint", JsonConvert.SerializeObject(pos));
      cmd.Parameters.AddWithValue("@unloadpoint", JsonConvert.SerializeObject(new Vector3()));
      cmd.Parameters.AddWithValue("@money", bankID);
      cmd.Parameters.AddWithValue("@mafia", -1);
      cmd.Parameters.AddWithValue("@orders", JsonConvert.SerializeObject(new List<Order>()));
      cmd.Parameters.AddWithValue("@managepoint", JsonConvert.SerializeObject(new Vector3()));
      MySQL.Query(cmd);

      Business biz = new Business(lastBizID, "Государство", govPrice, type, products_list, pos, new Vector3(), bankID, -1, new List<Order>(), new Vector3(), null, 0, new Vector3(), new List<int>(new int[31]), 100);
      biz.UpdateLabel();
      BizList.Add(lastBizID, biz);

      if (type == 6)
      {
        //MySQL.Query($"INSERT INTO `weapons`(`id`,`lastserial`) VALUES({biz.ID},0)");
        MySqlCommand cmd2 = new MySqlCommand();
        cmd2.CommandText = "INSERT INTO `weapons` SET " +
          "`id`=@id," +
          "`lastserial`=@lastserial";

        cmd2.Parameters.AddWithValue("@id", biz.ID);
        cmd2.Parameters.AddWithValue("@lastserial", 0);
        MySQL.Query(cmd2);
      }
      Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы создали бизнес {BusinessManager.BusinessTypeNames[type]}", 3000);
    }

    public static void createBusinessUnloadpoint(Player player, int bizid)
    {
      if (!Group.CanUseCmd(player, "createunloadpoint")) return;
      var pos = player.Position;
      BizList[bizid].UnloadPoint = pos;

      //MySQL.Query($"UPDATE businesses SET unloadpoint='{JsonConvert.SerializeObject(pos)}' WHERE id={bizid}");

      MySqlCommand cmd = new MySqlCommand();
      cmd.CommandText = "UPDATE `businesses` SET " +
        "`unloadpoint`=@unloadpoint" +
        " WHERE `id`=@id";

      cmd.Parameters.AddWithValue("@id", bizid);
      cmd.Parameters.AddWithValue("@unloadpoint", JsonConvert.SerializeObject(pos));
      MySQL.Query(cmd);

      Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Успешно создана точка разгрузки для бизнеса ID: {bizid}", 3000);
    }

    public static void createBusinessManagepoint(Player player, int bizid)
    {
      if (!Group.CanUseCmd(player, "createmanagepoint")) return;
      var pos = player.Position;
      BizList[bizid].ManagePoint = pos;

      //MySQL.Query($"UPDATE businesses SET managepoint='{JsonConvert.SerializeObject(pos)}' WHERE id={bizid}");

      MySqlCommand cmd = new MySqlCommand();
      cmd.CommandText = "UPDATE `businesses` SET " +
        "`managepoint`=@managepoint" +
        " WHERE `id`=@id";

      cmd.Parameters.AddWithValue("@id", bizid);
      cmd.Parameters.AddWithValue("@managepoint", JsonConvert.SerializeObject(pos));
      MySQL.Query(cmd);

      Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Успешно создана точка управления бизнесом ID: {bizid}", 3000);
    }

    public static void deleteBusinessManagepoint(Player player, int bizid)
    {
      if (!Group.CanUseCmd(player, "deletemanagepoint")) return;

      BizList[bizid].ManagePoint = new Vector3();
      //MySQL.Query($"UPDATE businesses SET managepoint='{JsonConvert.SerializeObject(new Vector3())}' WHERE id={bizid}");

      MySqlCommand cmd = new MySqlCommand();
      cmd.CommandText = "UPDATE `businesses` SET " +
        "`managepoint`=@managepoint" +
        " WHERE `id`=@id";

      cmd.Parameters.AddWithValue("@id", bizid);
      cmd.Parameters.AddWithValue("@managepoint", JsonConvert.SerializeObject(new Vector3()));
      MySQL.Query(cmd);

      Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Успешно удалена точка управления бизнесом ID: {bizid}", 3000);
    }

    public static void deleteBusinessCommand(Player player, int id)
    {
      if (!Group.CanUseCmd(player, "deletebusiness")) return;
      if (!BizList.ContainsKey(id)) return;
      //MySQL.Query($"DELETE FROM businesses WHERE id={id}");

      MySqlCommand cmd = new MySqlCommand();
      cmd.CommandText = "DELETE FROM `businesses` WHERE `id`=@id";

      cmd.Parameters.AddWithValue("@id", id);
      MySQL.Query(cmd);

      Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы удалили бизнес", 3000);
      Business biz = BusinessManager.BizList.FirstOrDefault(b => b.Value.ID == id).Value;

      if (biz.Type == 6)
      {
        //MySQL.Query($"DELETE FROM `weapons` WHERE id={id}");

        MySqlCommand cmd2 = new MySqlCommand();
        cmd2.CommandText = "DELETE FROM `weapons` WHERE `id`=@id";

        cmd2.Parameters.AddWithValue("@id", id);
        MySQL.Query(cmd2);
      }

      var owner = NAPI.Player.GetPlayerFromName(biz.Owner);
      if (owner == null)
      {
        var split = biz.Owner.Split('_');
        //var data = MySQL.QueryRead($"SELECT biz FROM characters WHERE firstname='{split[0]}' AND lastname='{split[1]}'");

        MySqlCommand cmd3 = new MySqlCommand();
        cmd3.CommandText = "SELECT `biz` FROM `characters` WHERE `firstname`=@firstname AND `lastname`=@lastname";

        cmd3.Parameters.AddWithValue("@firstname", split[0]);
        cmd3.Parameters.AddWithValue("@lastname", split[1]);
        var data = MySQL.QueryRead(cmd3);

        List<int> ownerBizs = new List<int>();
        foreach (DataRow Row in data.Rows)
          ownerBizs = JsonConvert.DeserializeObject<List<int>>(Row["biz"].ToString());
        ownerBizs.Remove(biz.ID);

        //MySQL.Query($"UPDATE characters SET biz='{JsonConvert.SerializeObject(ownerBizs)}' WHERE firstname='{split[0]}' AND lastname='{split[1]}'");

        MySqlCommand cmd4 = new MySqlCommand();
        cmd4.CommandText = "UPDATE `characters` SET `biz`=@biz WHERE `firstname`=@firstname AND `lastname`=@lastname";

        cmd4.Parameters.AddWithValue("@biz", JsonConvert.SerializeObject(ownerBizs));
        cmd4.Parameters.AddWithValue("@firstname", split[0]);
        cmd4.Parameters.AddWithValue("@lastname", split[1]);
        MySQL.Query(cmd4);
      }
      else
      {
        Main.Players[owner].BizIDs.Remove(id);
        MoneySystem.Wallet.Change(owner, biz.SellPrice);
      }
      biz.Destroy();
      BizList.Remove(biz.ID);
    }

    public static void sellBusinessCommand(Player player, Player target, int price)
    {
      if (!Main.Players.ContainsKey(player) || !Main.Players.ContainsKey(target)) return;

      if (player.Position.DistanceTo(target.Position) > 2)
      {
        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок слишком далеко", 3000);
        return;
      }

      if (Main.Players[player].BizIDs.Count == 0)
      {
        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У Вас нет бизнеса", 3000);
        return;
      }

      if (Main.Players[target].BizIDs.Count >= Group.GroupMaxBusinesses[Main.Players[target].VipLvl])
      {
        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок купил максимум бизнесов", 3000);
        return;
      }

      var biz = BizList[Main.Players[player].BizIDs[0]];
      if (price < biz.SellPrice / 2 || price > biz.SellPrice * 3)
      {
        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Невозможно продать бизнес за такую цену. Укажите цену от {biz.SellPrice / 2}$ до {biz.SellPrice * 3}$", 3000);
        return;
      }

      if (Main.Players[target].Money < price)
      {
        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У игрока недостаточно денег", 3000);
        return;
      }

      Trigger.ClientEvent(target, "popup::open", "BUSINESS_BUY", $"{player.Name} предложил Вам купить {BusinessTypeNames[biz.Type]} за ${price}");
      target.SetMyData("SELLER", player);
      target.SetMyData("SELLPRICE", price);
      target.SetMyData("SELLBIZID", biz.ID);

      Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы предложили игроку ({target.Value}) купить Ваш бизнес за {price}$", 3000);
    }

    public static void acceptBuyBusiness(Player player)
    {
      Player seller = player.GetMyData<Player>("SELLER");
      if (!Main.Players.ContainsKey(seller) || !Main.Players.ContainsKey(player)) return;

      if (player.Position.DistanceTo(seller.Position) > 2)
      {
        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок слишком далеко", 3000);
        return;
      }

      var price = player.GetMyData<int>("SELLPRICE");
      if (Main.Players[player].Money < price)
      {
        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У Вас недостаточно денег", 3000);
        return;
      }

      Business biz = BizList[player.GetMyData<int>("SELLBIZID")];
      if (!Main.Players[seller].BizIDs.Contains(biz.ID))
      {
        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Бизнес больше не принадлежит игроку", 3000);
        return;
      }

      if (Main.Players[player].BizIDs.Count >= Group.GroupMaxBusinesses[Main.Players[player].VipLvl])
      {
        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У Вас максимальное кол-во бизнесов", 3000);
        return;
      }

      Main.Players[player].BizIDs.Add(biz.ID);
      Main.Players[seller].BizIDs.Remove(biz.ID);

      biz.Owner = player.Name.ToString();
      var split1 = seller.Name.Split('_');
      var split2 = player.Name.Split('_');
      //MySQL.Query($"UPDATE characters SET biz='{JsonConvert.SerializeObject(Main.Players[seller].BizIDs)}' WHERE firstname='{split1[0]}' AND lastname='{split1[1]}'");
      //MySQL.Query($"UPDATE characters SET biz='{JsonConvert.SerializeObject(Main.Players[player].BizIDs)}' WHERE firstname='{split2[0]}' AND lastname='{split2[1]}'");
      //MySQL.Query($"UPDATE businesses SET owner='{biz.Owner}' WHERE id='{biz.ID}'");

      MySqlCommand cmd = new MySqlCommand();
      cmd.CommandText = "UPDATE `characters` SET `biz`=@biz WHERE `firstname`=@firstname AND `lastname`=@lastname";

      cmd.Parameters.AddWithValue("@biz", JsonConvert.SerializeObject(Main.Players[seller].BizIDs));
      cmd.Parameters.AddWithValue("@firstname", split1[0]);
      cmd.Parameters.AddWithValue("@lastname", split1[1]);
      MySQL.Query(cmd);

      MySqlCommand cmd2 = new MySqlCommand();
      cmd2.CommandText = "UPDATE `characters` SET `biz`=@biz WHERE `firstname`=@firstname AND `lastname`=@lastname";

      cmd2.Parameters.AddWithValue("@biz", JsonConvert.SerializeObject(Main.Players[player].BizIDs));
      cmd2.Parameters.AddWithValue("@firstname", split2[0]);
      cmd2.Parameters.AddWithValue("@lastname", split2[1]);
      MySQL.Query(cmd2);

      MySqlCommand cmd3 = new MySqlCommand();
      cmd3.CommandText = "UPDATE `businesses` SET `owner`=@owner WHERE `id`=@id";

      cmd3.Parameters.AddWithValue("@owner", biz.Owner);
      cmd3.Parameters.AddWithValue("@id", biz.ID);
      MySQL.Query(cmd3);


      biz.UpdateLabel();
      biz.LastBuy = DateTime.Now;
      MoneySystem.Wallet.Change(player, -price);
      MoneySystem.Wallet.Change(seller, price);
      GameLog.Money($"player({Main.Players[player].UUID})", $"player({Main.Players[seller].UUID})", price, $"buyBiz({biz.ID})");

      Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы купили у {seller.Name.Replace('_', ' ')} {BusinessTypeNames[biz.Type]} за {price}$", 3000);
      Notify.Send(seller, NotifyType.Info, NotifyPosition.BottomCenter, $"{player.Name.Replace('_', ' ')} купил у Вас {BusinessTypeNames[biz.Type]} за {price}$", 3000);
    }

    #region Menus
    #region manage biz
    public static void OpenBizListMenu(Player player)
    {
      if (Main.Players[player].BizIDs.Count == 0)
      {
        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У Вас нет ни одного бизнеса", 3000);
        return;
      }

      Menu menu = new Menu("bizlist", false, false);
      menu.Callback = callback_bizlist;

      Menu.Item menuItem = new Menu.Item("header", Menu.MenuItem.Header);
      menuItem.Text = "Ваши бизнесы";
      menu.Add(menuItem);

      foreach (var id in Main.Players[player].BizIDs)
      {
        menuItem = new Menu.Item(id.ToString(), Menu.MenuItem.Button);
        menuItem.Text = BusinessManager.BusinessTypeNames[BusinessManager.BizList[id].Type];
        menu.Add(menuItem);
      }

      menuItem = new Menu.Item("close", Menu.MenuItem.Button);
      menuItem.Text = "Закрыть";
      menu.Add(menuItem);

      menu.Open(player);
    }
    private static void callback_bizlist(Player player, Menu menu, Menu.Item item, string eventName, dynamic data)
    {
      switch (item.ID)
      {
        case "close":
          MenuManager.Close(player);
          return;
        default:
          OpenBizManageMenu(player, Convert.ToInt32(item.ID));
          player.SetMyData("SELECTEDBIZ", Convert.ToInt32(item.ID));
          return;
      }
    }

    public static void OpenBizManageMenu(Player player, int id)
    {
      if (!Main.Players[player].BizIDs.Contains(id))
      {
        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У Вас больше нет этого бизнеса", 3000);
        return;
      }

      Menu menu = new Menu("bizmanage", false, false);
      menu.Callback = callback_bizmanage;

      Menu.Item menuItem = new Menu.Item("header", Menu.MenuItem.Header);
      menuItem.Text = "Управление бизнесом";
      menu.Add(menuItem);

      menuItem = new Menu.Item("products", Menu.MenuItem.Button);
      menuItem.Text = "Товары";
      menu.Add(menuItem);

      Business biz = BizList[id];
      menuItem = new Menu.Item("tax", Menu.MenuItem.Card);
      menuItem.Text = $"Налог: {Convert.ToInt32(biz.SellPrice / 100 * Configuration.bizTaxMod)}$/ч";
      menu.Add(menuItem);

      menuItem = new Menu.Item("money", Menu.MenuItem.Card);
      menuItem.Text = $"Счёт бизнеса: {MoneySystem.Bank.Accounts[biz.BankID].Balance}$";
      menu.Add(menuItem);

      menuItem = new Menu.Item("sell", Menu.MenuItem.Button);
      menuItem.Text = "Продать бизнес";
      menu.Add(menuItem);

      menuItem = new Menu.Item("close", Menu.MenuItem.Button);
      menuItem.Text = "Закрыть";
      menu.Add(menuItem);

      menu.Open(player);
    }
    private static void callback_bizmanage(Player client, Menu menu, Menu.Item item, string eventName, dynamic data)
    {
      switch (item.ID)
      {
        case "products":
          MenuManager.Close(client);
          OpenBizProductsMenu(client);
          return;
        case "sell":
          MenuManager.Close(client);
          OpenBizSellMenu(client);
          return;
        case "close":
          MenuManager.Close(client);
          return;
      }
    }

    public static void OpenBizSellMenu(Player player)
    {
      Menu menu = new Menu("bizsell", false, false);
      menu.Callback = callback_bizsell;

      Menu.Item menuItem = new Menu.Item("header", Menu.MenuItem.Header);
      menuItem.Text = "Продажа";
      menu.Add(menuItem);

      var bizID = player.GetMyData<int>("SELECTEDBIZ");
      Business biz = BizList[bizID];
      var price = biz.SellPrice / 100 * 70;
      menuItem = new Menu.Item("govsell", Menu.MenuItem.Button);
      menuItem.Text = $"Продать государству (${price})";
      menu.Add(menuItem);

      menuItem = new Menu.Item("back", Menu.MenuItem.Button);
      menuItem.Text = "Назад";
      menu.Add(menuItem);

      menu.Open(player);
    }
    private static void callback_bizsell(Player client, Menu menu, Menu.Item item, string eventName, dynamic data)
    {
      if (!client.HasMyData("SELECTEDBIZ") || !Main.Players[client].BizIDs.Contains(client.GetMyData<int>("SELECTEDBIZ")))
      {
        MenuManager.Close(client);
        return;
      }

      var bizID = client.GetMyData<int>("SELECTEDBIZ");
      Business biz = BizList[bizID];
      switch (item.ID)
      {
        case "govsell":
          var price = biz.SellPrice / 100 * 70;
          MoneySystem.Wallet.Change(client, price);
          GameLog.Money($"server", $"player({Main.Players[client].UUID})", price, $"sellBiz({biz.ID})");

          Main.Players[client].BizIDs.Remove(bizID);
          biz.Cash = 0;
          MoneySystem.Bank.Accounts[biz.BankID].Balance = 0;
          biz.Owner = "Государство";
          biz.UpdateLabel();
          var newOrders = new Dictionary<int, int>(BusinessManager.Orders);
          foreach (var itemBiz in Core.BusinessManager.Orders)
          {
            if (itemBiz.Value == biz.ID)
            {
              newOrders.Remove(itemBiz.Key);
            }
          }
          Core.BusinessManager.Orders = newOrders;

          Notify.Send(client, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы продали бизнес государству за {price}$", 3000);

          MenuManager.Close(client);
          return;
        case "back":
          MenuManager.Close(client);
          OpenBizManageMenu(client, bizID);
          return;
      }
    }

    public static void OpenBizProductsMenu(Player player)
    {
      if (!player.HasMyData("SELECTEDBIZ") || !Main.Players[player].BizIDs.Contains(player.GetMyData<int>("SELECTEDBIZ")))
      {
        MenuManager.Close(player);
        return;
      }

      Menu menu = new Menu("bizproducts", false, false);
      menu.Callback = callback_bizprod;

      Menu.Item menuItem = new Menu.Item("header", Menu.MenuItem.Header);
      menuItem.Text = "Товары";
      menu.Add(menuItem);

      var bizID = player.GetMyData<int>("SELECTEDBIZ");

      Business biz = BizList[bizID];
      foreach (var p in biz.Products)
      {
        menuItem = new Menu.Item(p.Name, Menu.MenuItem.Button);
        menuItem.Text = p.Name;
        menu.Add(menuItem);
      }

      menuItem = new Menu.Item("back", Menu.MenuItem.Button);
      menuItem.Text = "Назад";
      menu.Add(menuItem);

      menu.Open(player);
    }
    private static void callback_bizprod(Player client, Menu menu, Menu.Item item, string eventName, dynamic data)
    {
      switch (item.ID)
      {
        case "back":
          MenuManager.Close(client);
          OpenBizManageMenu(client, client.GetMyData<int>("SELECTEDBIZ"));
          return;
        default:
          MenuManager.Close(client);
          OpenBizSettingMenu(client, item.ID);
          return;
      }
    }

    public static void OpenBizSettingMenu(Player player, string product)
    {
      if (!player.HasMyData("SELECTEDBIZ") || !Main.Players[player].BizIDs.Contains(player.GetMyData<int>("SELECTEDBIZ")))
      {
        MenuManager.Close(player);
        return;
      }

      Menu menu = new Menu("bizsetting", false, false);
      menu.Callback = callback_bizsetting;

      Menu.Item menuItem = new Menu.Item("header", Menu.MenuItem.Header);
      menuItem.Text = product;
      menu.Add(menuItem);

      var bizID = player.GetMyData<int>("SELECTEDBIZ");
      Business biz = BizList[bizID];

      foreach (var p in biz.Products)
        if (p.Name == product)
        {
          string ch = (biz.Type == 7 || biz.Type == 11 || biz.Type == 12 || product == "Татуировки" || product == "Парики" || product == "Патроны") ? "%" : "$";
          menuItem = new Menu.Item("price", Menu.MenuItem.Card);
          menuItem.Text = $"Текущая цена: {p.Price}{ch}";
          menu.Add(menuItem);

          menuItem = new Menu.Item("lefts", Menu.MenuItem.Card);
          menuItem.Text = $"Кол-во на складе: {p.Lefts}";
          menu.Add(menuItem);

          menuItem = new Menu.Item("capacity", Menu.MenuItem.Card);
          menuItem.Text = $"Вместимость склада: {ProductsCapacity[p.Name]}";
          menu.Add(menuItem);

          menuItem = new Menu.Item("setprice", Menu.MenuItem.Button);
          menuItem.Text = "Установить цену";
          menu.Add(menuItem);

          var price = (product == "Патроны") ? 4 : ProductsOrderPrice[product];
          menuItem = new Menu.Item("order", Menu.MenuItem.Button);
          menuItem.Text = $"Заказать: {price}$/шт";
          menu.Add(menuItem);

          menuItem = new Menu.Item("cancel", Menu.MenuItem.Button);
          menuItem.Text = "Отменить заказ";
          menu.Add(menuItem);

          menuItem = new Menu.Item("back", Menu.MenuItem.Button);
          menuItem.Text = "Назад";
          menu.Add(menuItem);

          player.SetMyData("SELECTPROD", product);
          menu.Open(player);
          return;
        }
    }
    private static void callback_bizsetting(Player client, Menu menu, Menu.Item item, string eventName, dynamic data)
    {

      var bizID = client.GetMyData<int>("SELECTEDBIZ");
      switch (item.ID)
      {
        case "setprice":
          MenuManager.Close(client);
          if (client.GetMyData<string>("SELECTPROD") == "Расходники")
          {
            Notify.Send(client, NotifyType.Error, NotifyPosition.BottomCenter, $"Невозможно установить цену на этот товар", 3000);
            return;
          }
          Main.OpenInputMenu(client, "Введите новую цену:", "biznewprice");
          return;
        case "order":
          MenuManager.Close(client);
          if (client.GetMyData<string>("SELECTPROD") == "Татуировки" || client.GetMyData<string>("SELECTPROD") == "Парики")
          {
            Notify.Send(client, NotifyType.Error, NotifyPosition.BottomCenter, $"Если Вы хотите возобновить продажу услуг, то закажите расходные материалы", 3000);
            return;
          }
          Main.OpenInputMenu(client, "Введите кол-во:", "bizorder");
          return;
        case "cancel":
          Business biz = BizList[bizID];
          var prodName = client.GetMyData<string>("SELECTPROD");

          foreach (var p in biz.Products)
          {
            if (p.Name != prodName) continue;
            if (p.Ordered)
            {
              var order = biz.Orders.FirstOrDefault(o => o.Name == prodName);
              if (order == null)
              {
                p.Ordered = false;
                return;
              }
              if (order.Taked)
              {
                Notify.Send(client, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы не можете отменить заказ, пока его доставляют", 3000);
                return;
              }
              biz.Orders.Remove(order);
              p.Ordered = false;

              MoneySystem.Wallet.Change(client, order.Amount * ProductsOrderPrice[prodName]);
              GameLog.Money($"server", $"player({Main.Players[client].UUID})", order.Amount * ProductsOrderPrice[prodName], $"orderCancel");
              Notify.Send(client, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы отменили заказ на {prodName}", 3000);
            }
            else Notify.Send(client, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы не заказывали этот товар", 3000);
            return;
          }
          return;
        case "back":
          MenuManager.Close(client);
          OpenBizManageMenu(client, bizID);
          return;
      }
    }
    #endregion

    public static List<string> electroCars = new List<string>() { "dilettante", "surge", "imorgon", "khamelion", "neon", "raiden", "cyclone", "tezeract", "voltic", "aetron", "ocnetrongt", "taycan", "modelx", "tesla", "teslas", "avtr", "ocnlamtmc" };

    public static void OpenPetrolMenu(Player player)
    {
      Business biz = BizList[player.GetMyData<int>("BIZ_ID")];
      Product prod = biz.Products[0];
      Vehicle vehicle = player.Vehicle;
      Log.Debug($"{vehicle.Model}");
      uint carModel = vehicle.Model;
      string carModelName = VehicleManager.VehicleFuel.Keys.FirstOrDefault(s => NAPI.Util.GetHashKey(s) == carModel);
      int fuel = vehicle.GetSharedData<int>("PETROL");
      int full = 0;
      if (carModelName != null && VehicleManager.VehicleFuel.ContainsKey(carModelName))
      {
        full = VehicleManager.VehicleFuel[carModelName];
      }
      else
      {
        full = VehicleManager.VehicleTank[vehicle.Class];
      }
      if (prod.Name == "Электро")
      {
        if (electroCars.Exists(s => NAPI.Util.GetHashKey(s) == carModel))
        {
          Trigger.ClientEvent(player, "openPetrol", "кв", fuel, full, biz.GetPriceWithMarkUpInt(prod.Price));
          Notify.Send(player, NotifyType.Info, NotifyPosition.TopCenter, $"Цена за КИЛОВАТТ: {biz.GetPriceWithMarkUpInt(prod.Price)}$", 7000);
        }
        else
        {
          Notify.Send(player, NotifyType.Info, NotifyPosition.TopCenter, $"Вам нужно на АЗС", 7000);
        }
      }
      else if (prod.Name == "Бензин")
      {
        if (!electroCars.Exists(s => NAPI.Util.GetHashKey(s) == carModel))
        {
          Trigger.ClientEvent(player, "openPetrol", "л", fuel, full, biz.GetPriceWithMarkUpInt(prod.Price));
          Notify.Send(player, NotifyType.Info, NotifyPosition.TopCenter, $"Цена за литр: {biz.GetPriceWithMarkUpInt(prod.Price)}$", 7000);
        }
        else
        {
          Notify.Send(player, NotifyType.Info, NotifyPosition.TopCenter, $"Вам нужно на Электрозаправку", 7000);
        }
      }
    }
    private static void callback_petrol(Player client, Menu menu, Menu.Item item, string eventName, dynamic data)
    {
      switch (item.ID)
      {
        case "fill":
          MenuManager.Close(client);
          Main.OpenInputMenu(client, "Введите кол-во литров:", "fillcar");
          return;
        case "close":
          MenuManager.Close(client);
          return;
      }
    }

    #endregion

    public static void changeOwner(string oldName, string newName)
    {
      List<int> toChange = new List<int>();
      lock (BizList)
      {
        foreach (KeyValuePair<int, Business> biz in BizList)
        {
          if (biz.Value.Owner != oldName) continue;
          Log.Write($"The biz was found! [{biz.Key}]");
          toChange.Add(biz.Key);
        }
        foreach (int id in toChange)
        {
          BizList[id].Owner = newName;
          BizList[id].UpdateLabel();
          BizList[id].Save();
        }
      }
    }


    #region new Bussiness menu

    #region methods
    public static Business GetBussiness(Player player)
    {
      Business bussiness = BizList.FirstOrDefault(biz => biz.Value.Owner == player.Name).Value;
      if (bussiness != null)
      {
        return bussiness;
      }
      else
        return null;
    }

    public static Business GetBussinessByBank(Player player, int bankID)
    {
      Business bussiness = BizList.FirstOrDefault(x => x.Value.BankID == bankID).Value;
      if (bussiness != null)
      {
        return bussiness;
      }
      else
        return null;
    }

    public static bool isBussinessOwner(Player player)
    {
      Business bussiness = BusinessManager.BizList.FirstOrDefault(biz => biz.Value.Owner == player.Name).Value;
      if (bussiness != null) return true;

      return false;
    }

    public static string formatedDays(int days)
    {
      string[] words = { "День", "Дней", "Дня" };
      string ending = "";

      days = days % 100;
      if (days >= 11 && days <= 19)
      {
        ending = words[1];
      }
      else
      {
        int i = days % 10;
        switch (i)
        {
          case (1): ending = words[0]; break;
          case (2): ending = words[2]; break;
          case (3): ending = words[2]; break;
          case (4): ending = words[2]; break;
          default: ending = words[1]; break;
        }
      }

      return days + " " + ending;
    }

    public static void acceptSellBussinessToGov(Player player)
    {
      if (!player.HasMyData("BUSSINESSMANAGE_ID")) return;

      Business bussiness = GetBussiness(player);
      if (bussiness == null) return;

      var price = bussiness.SellPrice / 100 * 70;
      MoneySystem.Wallet.Change(player, price);
      GameLog.Money($"server", $"player({Main.Players[player].UUID})", price, $"sellBiz({bussiness.ID})");

      Main.Players[player].BizIDs.Remove(bussiness.ID);
      bussiness.Cash = 0;
      MoneySystem.Bank.Accounts[bussiness.BankID].Balance = 0;
      bussiness.Owner = "Государство";
      bussiness.UpdateLabel();
      var newOrders = new Dictionary<int, int>(BusinessManager.Orders);
      foreach (var item in Core.BusinessManager.Orders)
      {
        if (item.Value == bussiness.ID)
        {
          newOrders.Remove(item.Key);
        }
      }
      Core.BusinessManager.Orders = newOrders;


      Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы продали бизнес государству за {price}$", 3000);
      MenuManager.Close(player);
    }

    #endregion

    #region menu
    public static void OpenBussinessBuyMenu(Player player)
    {
      try
      {
        if (!player.HasMyData("BUSSINESSMANAGE_ID")) return;

        int id = player.GetMyData<int>("BUSSINESSMANAGE_ID");
        Business bussiness = BusinessManager.BizList[id];
        if (bussiness == null) return;

        var bussinessMafia = Fractions.Manager.getName(bussiness.Mafia);

        bool canWar = false;
        Dictionary<string, object> dict = new Dictionary<string, object>();

        if (Main.Players[player].Fraction.FractionID != bussiness.Mafia && Manager.canUseAccess(player, Manager.FractionAccess.bizwar, false)) canWar = true;

        dict.Add("show", true);
        dict.Add("business", new Dictionary<string, object>()
        {
            {"name", BusinessManager.BusinessTypeNames[bussiness.Type]},
            {"owner", bussiness.Owner == "Государство" ? "" : bussiness.Owner },
            {"type", bussiness.ID },
            {"statePrice", bussiness.SellPrice },
            {"rentalPrice", Convert.ToInt32(bussiness.SellPrice / 100 * Configuration.bizTaxMod) * 24 },
            {"fees", Convert.ToInt32(bussiness.SellPrice / 100 * Configuration.bizTaxMod) },
            {"criminalRoof", bussinessMafia.Length < 2 ? "Нет" : bussinessMafia },
            {"canWar", canWar }

        });

        //Log.Write($"{ JsonConvert.SerializeObject(dict)}");
        Trigger.ClientEvent(player, "CLIENT::BUSINESS:OPEN_MENU", JsonConvert.SerializeObject(dict));

      }
      catch (Exception e) { Log.Write("OpenHouseAboutMenu: " + e.StackTrace, nLog.Type.Error); }
    }

    [RemoteEvent("SERVER::BUSINESS:OPEN_MANAGE_MENU")]
    public static void OpenBussinessManageMenu(Player player, bool remote = false, int bizId = -1)
    {
        try
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            int id;

            if (remote && Main.Players[player].BizIDs.Count > 0)
            {
                id = Main.Players[player].BizIDs[0];
            }
            else
            {
                if (bizId == -1)
                {
                    if (!player.HasMyData("BUSSINESSMANAGE_ID") || player.GetMyData<int>("BUSSINESSMANAGE_ID") == -1)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы должны находиться около бизнеса", 3000);
                        return;
                    }

                    id = player.GetMyData<int>("BUSSINESSMANAGE_ID");
                }
                else
                {
                    id = bizId;
                }
            }


            Business bussiness = BusinessManager.BizList[id];

            var bussinessMafia = Fractions.Manager.getName(bussiness.Mafia);

            dict.Add("show", true);
            dict.Add("business", new Dictionary<string, object>()
            {
                {"name", BusinessManager.BusinessTypeNames[bussiness.Type]},
                {"owner", bussiness.Owner == "Государство" ? "" : bussiness.Owner },
                {"type", bussiness.ID },
                {"statePrice", Convert.ToInt32(bussiness.SellPrice / 100 * 70) },
                {"rentalPrice",  Convert.ToInt32(bussiness.SellPrice / 100 * Configuration.bizTaxMod) * 24 },
                {"fees", Convert.ToInt32(bussiness.SellPrice / 100 * Configuration.bizTaxMod) },
                {"criminalRoof", bussinessMafia.Length < 2 ? "Нет" : bussinessMafia},
                {"balance", bussiness.Cash }
            });

            List<Dictionary<string, object>> products = new List<Dictionary<string, object>>();

            int temp = 0;



            foreach (Product prod in bussiness.Products)
            {
                products.Add(new Dictionary<string, object>()
                {
                    { "key", temp },
                    { "name",  AutoSalonTypes.Contains((int)bussiness.Type) ? VehicleManager.GetVehicleRealName(prod.Name) : prod.Name },
                    { "originalPrice", ProductsOrderPrice[prod.Name] },
                    { "remainingPieces", prod.Lefts },
                    { "remainingLimit", ProductsCapacity[prod.Name] },
                    { "price", bussiness.GetPriceWithMarkUpInt(ProductsOrderPrice[prod.Name]) },
                    { "orderPieces",0 },
                });
                temp++;
            }

            DateTime date = DateTime.Now.AddDays(-DateTime.Now.Day + 1);

            List<Dictionary<string, object>> dates = new List<Dictionary<string, object>>();
            int d = 0;
            foreach (int profit in bussiness.Profit)
            {
                dates.Add(new Dictionary<string, object>()
                {
                { "x",  $"{date.AddDays(d).Date.ToString("dd.MM")}" },
                { "y",  profit },
                });
                d++;
            }

            dict.Add("businessManagment", new Dictionary<string, object>()
            {
                {"items", products },
                {"extraCharge", new Dictionary<string, object>(){
                    {"min", MinMaxMarkup[bussiness.Type].Item1 - 100 },
                    {"max", MinMaxMarkup[bussiness.Type].Item2 - 100 },
                    {"value", bussiness.Markup - 100 },
                    {"inputValue", bussiness.Markup - 100}
                } },

            });
            dict.Add("stat", dates);

            Trigger.ClientEvent(player, "CLIENT::BUSINESS:OPEN_MANAGE_MENU", JsonConvert.SerializeObject(dict));
            KeyLabel.Hide(player);
        }catch(Exception ex) { Log.Debug(ex.StackTrace.ToString()); }
    }

    #endregion

    #region RemoteEvents

    [RemoteEvent("SERVER::BUSSINESS:buyBussiness")]
    public static void RemoteEvent_buyBussiness(Player player)
    {
      if (!player.HasMyData("BUSSINESSMANAGE_ID") || player.GetMyData<int>("BUSSINESSMANAGE_ID") == -1)
      {
        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы должны находиться около бизнеса", 3000);
        return;
      }
      int id = player.GetMyData<int>("BUSSINESSMANAGE_ID");
      Business biz = BusinessManager.BizList[id];
      if (Main.Players[player].BizIDs.Count >= Group.GroupMaxBusinesses[Main.Players[player].VipLvl])
      {
        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы не можете приобрести больше {Group.GroupMaxBusinesses[Main.Players[player].VipLvl]} бизнесов", 3000);
        return;
      }
      if (biz.Owner == "Государство")
      {
        if (!Wallet.Change(player, -biz.SellPrice))
        {
          Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У Вас не хватает средств", 3000);
          return;
        }
        GameLog.Money($"player({Main.Players[player].UUID})", $"server", biz.SellPrice, $"buyBiz({biz.ID})");
        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Поздравляем! Вы купили {BusinessManager.BusinessTypeNames[biz.Type]}, не забудьте внести налог за него в банкомате", 3000);
        biz.Owner = player.Name.ToString();
        client.Core.Achievements.AddAchievementScore(player, client.Core.AchievementID.Startup, 1);
      }
      else if (biz.Owner == player.Name.ToString())
      {
        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Этот бизнес принадлежит Вам", 3000);
        return;
      }
      else if (biz.Owner != player.Name.ToString())
      {
        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Этот бизнес принадлежит другому игроку", 3000);
        return;
      }

      biz.UpdateLabel();
      foreach (var p in biz.Products)
      {
        p.Ordered = false;
        p.Lefts = 0;
      }

      var newOrders = new List<Order>();
      foreach (var o in biz.Orders)
      {
        if (o.Taked) newOrders.Add(o);
        else Orders.Remove(o.UID);
      }
      biz.Orders = newOrders;

      Main.Players[player].BizIDs.Add(id);
      var tax = Convert.ToInt32(biz.SellPrice / 10000);
      MoneySystem.Bank.Accounts[biz.BankID].Balance = tax * 2;
      biz.LastBuy = DateTime.Now;
      var split = biz.Owner.Split('_');
      //MySQL.Query($"UPDATE characters SET biz='{JsonConvert.SerializeObject(Main.Players[player].BizIDs)}' WHERE firstname='{split[0]}' AND lastname='{split[1]}'");
      //MySQL.Query($"UPDATE businesses SET owner='{biz.Owner}' WHERE id='{biz.ID}'");


      MySqlCommand cmd = new MySqlCommand();
      cmd.CommandText = "UPDATE `characters` SET `biz`=@biz WHERE `firstname`=@firstname AND `lastname`=@lastname";

      cmd.Parameters.AddWithValue("@biz", JsonConvert.SerializeObject(Main.Players[player].BizIDs));
      cmd.Parameters.AddWithValue("@firstname", split[0]);
      cmd.Parameters.AddWithValue("@lastname", split[1]);
      MySQL.Query(cmd);

      MySqlCommand cmd2 = new MySqlCommand();
      cmd2.CommandText = "UPDATE `businesses` SET `owner`=@owner WHERE `id`=@id";

      cmd2.Parameters.AddWithValue("@owner", biz.Owner);
      cmd2.Parameters.AddWithValue("@id", biz.ID);
      MySQL.Query(cmd2);
    }

    public class ObjectProduct
    {
      [JsonProperty("name")]
      public string name { get; set; }
    }

    [RemoteEvent("SERVER::BUSINESS:ORDER_PRODUCT")]
    public static void RemoteEvent_orderProduct(Player player, int key, int productAmount)
    {
      try
      {
        if (!player.HasMyData("SELECTEDBIZ") || !Main.Players[player].BizIDs.Contains(player.GetMyData<int>("SELECTEDBIZ")))
        {
          return;
        }

        var bizID = player.GetMyData<int>("SELECTEDBIZ");
        Business biz = BizList[bizID];

        if (productAmount < 1)
        {
          Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Неверное значение", 3000);
          return;
        }

        var product = biz.Products[key];


        if (product.Ordered)
        {
          Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы уже заказали этот товар", 3000);

          return;
        }

        if (biz.Type >= 2 && biz.Type <= 5)
        {
          if (productAmount > 10)
          {
            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Укажите значение от 1 до 10", 3000);

            return;
          }
        }
        else if (biz.Type == 17 || biz.Type == 19 || biz.Type == 20)
        {
          if (productAmount > 10)
          {
            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Укажите значение от 1 до 10", 3000);

            return;
          }
        }
        else if (biz.Type == 14)
        {
          if (productAmount < 1 || product.Lefts + productAmount > ProductsCapacity[product.Name])
          {
            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Укажите значение от 1 до {ProductsCapacity[product.Name] - product.Lefts}", 3000);

            return;
          }
        }
        else
        {
          if (productAmount < 10 || product.Lefts + productAmount > ProductsCapacity[product.Name])
          {
            var text = "";
            if (ProductsCapacity[product.Name] - product.Lefts < 10) text = "У Вас достаточно товаров на складе";
            else text = $"Укажите от 10 до {ProductsCapacity[product.Name] - product.Lefts}";

            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, text, 3000);

            return;
          }
        }

        var price = (product.Name == "Патроны") ? 4 : ProductsOrderPrice[product.Name];
        double orderPriceByProduct = 5000 / biz.Products.Count;
        int finishPrice = (productAmount * price) + (int)Math.Round(orderPriceByProduct);

        if (!Bank.Change(Main.Players[player].Bank, -finishPrice))
        {
          Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Недостаточно средств на счету", 3000);
          return;
        }
        GameLog.Money($"bank({Main.Players[player].Bank})", $"server", finishPrice, "bizOrder");
        var order = new Order(product.Name, productAmount, false);
        product.Ordered = true;

        var random = new Random();
        do
        {
          order.UID = random.Next(000000, 999999);
        } while (BusinessManager.Orders.ContainsKey(order.UID));

        BusinessManager.Orders.Add(order.UID, biz.ID);
        biz.Orders.Add(order);

        Truckers.UpdateOrders();

        if (AutoSalonTypes.Contains((int)biz.Type))
          Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы заказали {VehicleManager.GetVehicleRealName(product.Name)} в количестве {productAmount}. №{order.UID}", 3000);
        else
          Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы заказали {product.Name} в количестве {productAmount}. №{order.UID}", 3000);

        return;


      }
      catch (Exception e) { Log.Write("orderProduct: " + e.StackTrace + e.StackTrace, nLog.Type.Error); }
    }

    [RemoteEvent("cancelOrder")]
    public static void RemoteEvent_cancelOrder(Player player, string productName)
    {
      try
      {
        if (!player.HasMyData("SELECTEDBIZ") || !Main.Players[player].BizIDs.Contains(player.GetMyData<int>("SELECTEDBIZ")))
        {
          return;
        }

        var bizID = player.GetMyData<int>("SELECTEDBIZ");
        Business biz = BizList[bizID];

        foreach (var product in biz.Products)
        {
          if (product.Name != productName) continue;
          if (product.Ordered)
          {
            var order = biz.Orders.FirstOrDefault(o => o.Name == productName);
            if (order == null)
            {
              product.Ordered = false;
              return;
            }
            if (order.Taked)
            {
              Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы не можете отменить заказ, пока его доставляют", 3000);
              return;
            }
            biz.Orders.Remove(order);
            product.Ordered = false;

            MoneySystem.Wallet.Change(player, order.Amount * ProductsOrderPrice[productName]);
            GameLog.Money($"server", $"player({Main.Players[player].UUID})", order.Amount * ProductsOrderPrice[productName], $"orderCancel");
            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы отменили заказ на {productName}", 3000);
          }
          else Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы не заказывали этот товар", 3000);
          return;
        }
        return;
      }
      catch (Exception e) { Log.Write("cancelOrder: " + e.StackTrace + e.StackTrace, nLog.Type.Error); }
    }
    [Command("cancelbizorders")]
    public static void CMD_CanselBizOrders(Player player, int ID)
    {
        try
        {
            if (!Group.CanUseCmd(player, "cancelbizorders")) return;
            Business biz = BizList[ID];

            Log.Debug("canselbizorders\n" + JsonConvert.SerializeObject(biz.Products));

            foreach (var product in biz.Products)
            {
                if (product.Ordered)
                {
                    var order = biz.Orders.FirstOrDefault(o => o.Name == product.Name);
                    if (order == null)
                    {
                        product.Ordered = false;
                        continue;
                    }
                    if (order.Taked)
                    {
                        continue;
                    }
                    biz.Orders.Remove(order);
                    product.Ordered = false;

                    MoneySystem.Wallet.Change(player, order.Amount * ProductsOrderPrice[product.Name]);
                    GameLog.Money($"server", $"player({biz.Owner})", order.Amount * ProductsOrderPrice[product.Name], $"orderCancel");
                }
            }
            var newOrders = new Dictionary<int, int>(BusinessManager.Orders);
            foreach (var item in Core.BusinessManager.Orders)
            {
                if (item.Value == biz.ID)
                {
                    newOrders.Remove(item.Key);
                }
            }
            Core.BusinessManager.Orders = newOrders;
        }
        catch (Exception e) { Log.Write("cancelOrder: " + e.StackTrace + e.StackTrace, nLog.Type.Error); }
    }
    [RemoteEvent("sellBussinessToPlayer")]
    public static void RemoteEvent_sellBusinessToPlayer(Player player, Player target, int price)
    {
      if (!Main.Players.ContainsKey(player) || !Main.Players.ContainsKey(target)) return;

      if (player.Position.DistanceTo(target.Position) > 2)
      {
        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок слишком далеко", 3000);
        return;
      }

      if (Main.Players[player].BizIDs.Count == 0)
      {
        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У Вас нет бизнеса", 3000);
        return;
      }

      if (Main.Players[target].BizIDs.Count >= Group.GroupMaxBusinesses[Main.Players[target].VipLvl])
      {
        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок купил максимум бизнесов", 3000);
        return;
      }

      var biz = BizList[Main.Players[player].BizIDs[0]];
      if (price < biz.SellPrice / 2 || price > biz.SellPrice * 3)
      {
        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Невозможно продать бизнес за такую цену. Укажите цену от {biz.SellPrice / 2}$ до {biz.SellPrice * 3}$", 3000);
        return;
      }

      if (Main.Players[target].Money < price)
      {
        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У игрока недостаточно денег", 3000);
        return;
      }

      Trigger.ClientEvent(target, "popup::open", "BUSINESS_BUY", $"{player.Name} предложил Вам купить {BusinessTypeNames[biz.Type]} за ${price}");
      target.SetMyData("SELLER", player);
      target.SetMyData("SELLPRICE", price);
      target.SetMyData("SELLBIZID", biz.ID);

      Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы предложили игроку ({target.Value}) купить Ваш бизнес за {price}$", 3000);
    }



    [RemoteEvent("SERVER::BUSINESS:WIDTHDRAW")]
    public static void RemotEvent_WidthdrawMoney(Player player)
    {
      if (!Main.Players.ContainsKey(player)) return;

      if (Main.Players[player].BizIDs.Count == 0)
      {
        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У Вас нет бизнеса", 3000);
        return;
      }

      var biz = BizList[Main.Players[player].BizIDs[0]];

      Trigger.ClientEvent(player, "popup::openInput", $"В кассе: {biz.Cash}$", $"Сумма от 0 до {biz.Cash}$", 10, "player_bizwidthdraw");
    }

    public static void WitdrawMoney(Player player, int value)
    {

      if (Main.Players[player].BizIDs.Count == 0)
      {
        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У Вас нет бизнеса", 3000);
        return;
      }

      var biz = BizList[Main.Players[player].BizIDs[0]];

      if (biz.Cash < value || value <= 0)
      {
        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"В кассе недостаточно денег", 3000);
        return;
      }

      biz.Cash -= value;
      MoneySystem.Wallet.Change(player, +value);
      OpenBussinessManageMenu(player);

      // Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы сняли {value}$ из кассы бизнеса", 3000);
    }

    [RemoteEvent("SERVER::BUSINESS:TOPUP")]
    public static void RemotEvent_TopupMoney(Player player)
    {

      if (!Main.Players.ContainsKey(player)) return;

      if (Main.Players[player].BizIDs.Count == 0)
      {
        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У Вас нет бизнеса", 3000);
        return;
      }

      var biz = BizList[Main.Players[player].BizIDs[0]];

      Trigger.ClientEvent(player, "popup::openInput", $"Наличные: {Main.Players[player].Money}$", $"Сумма от 0 до {Main.Players[player].Money}$", 10, "player_biztopup");
    }

    [RemoteEvent("SERVER::BUSINESS:SET_MARKUP")]
    public static void BusinessSave(Player player, int markup)
    {
      try
      {
        if (!player.HasMyData("BUSSINESSMANAGE_ID") || !Main.Players[player].BizIDs.Contains(player.GetMyData<int>("BUSSINESSMANAGE_ID")))
          return;
        markup += 100;
        var bizID = player.GetMyData<int>("BUSSINESSMANAGE_ID");

        Business biz = BizList[bizID];

        if (markup > MinMaxMarkup[biz.Type].Item2 || markup < MinMaxMarkup[biz.Type].Item1)
        {
          Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"От {MinMaxMarkup[biz.Type].Item1 - 100} до {MinMaxMarkup[biz.Type].Item2 - 100}", 3000);
          return;
        }

        biz.Markup = markup;

        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Наценка на товары установлена на {biz.Markup}%", 3000);


      }
      catch (Exception e)
      {
        Log.Write("BusinessSave: " + e.StackTrace);
      }
    }

    public static void TopupMoney(Player player, int value)
    {

      if (Main.Players[player].BizIDs.Count == 0)
      {
        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У Вас нет бизнеса", 3000);
        return;
      }

      var biz = BizList[Main.Players[player].BizIDs[0]];

      if (Main.Players[player].Money < value || value <= 0)
      {
        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У вас недостаточно денег", 3000);
        return;
      }

      biz.Cash += value;
      MoneySystem.Wallet.Change(player, -value);
      OpenBussinessManageMenu(player);
      //Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы положили в кассу бизнеса {value}$", 3000);
    }


    [RemoteEvent("SERVER::BUSINESS:SELL_BUSINESS")]
    public static void RemoteEvent_openSellBussinessMenu(Player player)
    {
      var bussinessID = player.GetMyData<int>("BUSSINESSMANAGE_ID");
      Business bussiness = BizList[bussinessID];
      var bussinessPrice = bussiness.SellPrice / 100 * 70;

      BusinessManager.acceptSellBussinessToGov(player);
    }

    #endregion

    #endregion
  }

  public class Order
  {
    public Order(string name, int amount, bool isGov, bool taked = false)
    {
      Name = name;
      Amount = amount;
      Taked = taked;
      IsGOV = isGov;
    }

    public string Name { get; set; }
    public int Amount { get; set; }

    public int Length;

    public bool IsGOV { get; set; } = false;

    [JsonIgnore]
    public bool Taked { get; set; }

    [JsonIgnore]
    public int UID { get; set; }
  }

  public class CollectorOrder
  {
    public CollectorOrder(bool taked = false)
    {
      Taked = taked;
    }

    public int UID { get; set; }
    [JsonIgnore]
    public bool Taked { get; set; }

  }

  public class Product
  {
    public Product(int price, int left, int autosell, string name, bool ordered)
    {
      Price = price;
      Lefts = left;
      Autosell = autosell;
      Name = name;
      Ordered = ordered;
    }

    public int Price { get; set; }
    public int Lefts { get; set; }
    public int Autosell { get; set; }
    public string Name { get; set; }
    public bool Ordered { get; set; }
  }
}
