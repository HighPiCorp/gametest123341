using GTANetworkAPI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace NeptuneEvo.SDK
{
    public enum RewardType
    {
        Money,
        Donate,
        Car,
        Clothes,
        CasinoChips,
        VIP,
        InventoryItem,
        SilverVIP,
        GoldVIP,
        PlatinumVIP
    }


    public class Reward
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Status { get; set; }
        public RewardType Type { get; set; }
        public dynamic Data { get; set; }

        public Reward(string name, string desc, bool status, RewardType type, dynamic data = null)
        {
            Name = name;
            Description = desc;
            Status = status;
            Type = type;
            Data = data;
        }

    }

    public class CharacterData
    {
        public int UUID { get; set; } = -1;
        public string PersonID { get; set; } = null;
        public Vector3 SpawnPos { get; set; } = new Vector3(0, 0, 0);
        public DateTime CreateDate { get; set; } = DateTime.Now;
        public DateTime BirthDate { get; set; } = DateTime.Now;
        public string FirstName { get; set; } = null;
        public string LastName { get; set; } = null;
        public Dictionary<int, List<RoulettePrizesInfo>> RoulettePrizes { get; set; } = null;
        public Arrest Arrest { get; set; } = new Arrest();
        public int Quest { get; set; } = 0;
        public int QuestFinished { get; set; } = 0;
        public int QuestChapter { get; set; } = 0;
        public int QuestProgress { get; set; } = 0;
        public int QuestEndChapter { get; set; } = 0;
        public int QuestInteraction { get; set; } = 0;
        public int QuestLastChapter { get; set; } = 0;
        public bool QuestChapterIsFinished { get; set; } = false;
        public string QuestUniqName { get; set; } = "";
        public string QuestNPC { get; set; } = "Тревор";
        public int QuestStep { get; set; } = 0;
        public int BrigadeId { get; set; } = -1;

        public bool RefPromoActivated { get; set; } = false;
        public string RefPromo { get; set; } = "";
        public Dictionary<string, ObjectPromocodePlayer> PromoCodes { get; set; } = new Dictionary<string, ObjectPromocodePlayer>();
        public Dictionary<string, ObjectPromocodeStatusPlayer> PromoCodesStatus { get; set; } = new Dictionary<string, ObjectPromocodeStatusPlayer>();

        public int VipLvl { get; set; }
        public DateTime VipDate { get; set; } = DateTime.Now;

        public DateTime LastSpinWheel;
        public int Prize = -1;

        public List<int> Prizes { get; set; } = new List<int>();

        public class RoulettePrizesInfo
        {
            public RoulettePrizesInfo(int id, int uuid, string item, string rare, string prize, bool isTaked, string date)
            {
                ID = id;
                UUID = uuid;
                Item = item;
                Rare = rare;
                Prize = prize;
                IsTaked = isTaked;
                Date = date;
            }

            public int ID { get; set;}
            public int UUID { get; set;}
            public string Item { get; set;}
            public string Rare { get; set;}
            public string Prize { get; set;}
            public bool IsTaked { get; set;}
            public string Date { get; set;}
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

        public int GarbageBags { get; set; } = 0;
        public List<int> GarbageSkills { get; set; } = new List<int>() { 0, 0, 0 };

        public static QuestInfo QuestPanel = new QuestInfo();

        public bool PrologIsEnd { get; set; } = false;
        public int PrologStep { get; set; } = 0;

        public bool RaceIsEnd { get; set; } = false;
        public int RacePosition { get; set; } = 0;
        public string RaceResultTime { get; set; } = "";
        public bool RaceIsActive { get; set; } = false;

        public int RacePrizeTimer = 0;
        public string RaceVehicleName { get; set; } = "";
        public Vector3 RaceVehiclePosition { get; set; } = new Vector3();
        public Vector3 RaceVehicleRotation { get; set; } = new Vector3();
        public int RaceVehicleTimer = 0;
        public Vehicle RaceVehicle;
        public bool Gender { get; set; } = true;
        public int Health { get; set; } = 100;
        public int Armor { get; set; } = 0;
        public int LVL { get; set; } = 1;
        public int EXP { get; set; } = 0;
        public long Money { get; set; } = 500;

        public int Bank { get; set; } = 0;
        public int WorkID { get; set; } = 0;
        public Fraction Fraction { get; set; } = new Fraction();
        public int ArrestTime { get; set; } = 0;
        public List<string> Friends { get; set; } = new List<string>();
        public int Water { get; set; } = 100;
        public int Eat { get; set; } = 100;
        public int DemorganTime { get; set; } = 0;
        public WantedLevel WantedLVL { get; set; } = null;
        public List<int> BizIDs { get; set; } = new List<int>();
        public int AdminLVL { get; set; } = 0;
        public List<bool> Licenses { get; set; } = new List<bool>();
        public DateTime Unwarn { get; set; } = DateTime.Now;
        public int Unmute { get; set; } = 0;
        public int Warns { get; set; } = 0;
        public int LastVeh { get; set; } = -1;
        public bool OnDuty { get; set; } = false;
        public int LastHourMin { get; set; } = 0;
        public int HotelID { get; set; } = -1;
        public int HotelLeft { get; set; } = 0;
        public int Sim { get; set; } = -1;
        public int SimBalance { get; set; } = 0;
        public string PetName { get; set; } = "null";
        public Dictionary<int, string> Contacts = new Dictionary<int, string>();
        public List<bool> Achievements = new List<bool>();
        public List<int> AchievementsScore = new List<int>();
        public List<Reward> RewardsData = new List<Reward>();

        public Dictionary<uint, List<string>> WeaponComponentsSync = new Dictionary<uint, List<string>>();
        public string currentWeapon { get; set; } = "Unarmed";
        public float currentWeight { get; set; } = 0.0f;
        public float MaxWeight { get; set; } = 10.0f;
        public string FamilyCID { get; set; } = "null";
        public int FamilyRank { get; set; } = 0;
        public List<bool> AttachWeapSlots { get; set; } = new List<bool>() { false, false, false, false };
        public int LastBonus { get; set; } = 0; //todo lastbonus
        public bool IsBonused { get; set; } = false; //todo lastbonus
        public List<MedCard> MedCards { get; set; } = null;

        //Robbery Houses
        public DateTime RobberyCooldown { get; set; }

        //afk
        public bool isAfk { get; set; } = false;

        public bool VoiceMuted = false;

        // temperory data
        public int InsideHouseID = -1;
        public int InsideGarageID = -1;
        public int InsideFamilyGarageID = -1;
        public Vector3 ExteriorPos = new Vector3();
        public int InsideHotelID = -1;
        public int InsideOfficeID = -1;
        public int InsideFamilyID = -1;
        public int InsideBizID = -1;
        public bool InsideCasino = false;
        public int TuningShop = -1;
        public bool IsAlive = false;
        public bool IsSpawned = false;

        public Dictionary<string, object> Data { get; set; } = new Dictionary<string, object>();

        public System.Timers.Timer aTimer;
        public DateTime aTimerElapsed;
        public string aTimerFormatedTime;
        public Player Racer;
        public TimeSpan aTimerStartTime { get; set; } = TimeSpan.Zero;

        public int TruckerExp { get; set; } = 0;
        public int TruckerPay { get; set; } = 0;

        public int BuilderExp { get; set; } = 0;
        public int BuilderLocksmith { get; set; } = 0;
        public int BuilderLoader { get; set; } = 0;
        public int BuilderSmallContracts { get; set; } = 0;
        public int BuilderBigContracts { get; set; } = 0;
        public int BuilderFailSmallContracts { get; set; } = 0;
        public int BuilderFailBigContracts { get; set; } = 0;
        public int BuilderForfeitBigContracts { get; set; } = 0;
        public int BuilderForfeitSmallContracts { get; set; } = 0;
        public int CollectorExp { get; set; } = 0;
        public int PlayerDayGameHours { get; set; } = 0;

        public List<int> InAllFractions { get; set; } = new List<int>();
        public List<Call> Calls { get; set; } = new List<Call>();
        public Dictionary<string, List<string>> ColorPickerPresets { get; set; } = new Dictionary<string, List<string>>();
        public int RodExp { get; set; } = 0;

        public DateTime BookMarkDrugCooldown { get; set; }
        public DateTime CarTheftCooldown { get; set; }

        public List<int> SearchPointIDs { get; set; } = new List<int>();

        public BPCharacterInfo BPInformation { get; set; } = null;

        public DateTime LastOnline { get; set; } = DateTime.Now;

        //public int BPCoins { get; set; } = 0;
    }

    public class Call
    {
        public int Number;
        public bool IsOutbound;
        public bool IsMissed;

        public Call(int number, bool isOutbound, bool isMissed)
        {
            Number = number;
            IsOutbound = isOutbound;
            IsMissed = isMissed;
        }
    }

    public class MedCard
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string Diagnosis { get; set; }
        public string DoctorName { get; set; }

        public void Give(string doctorName, DateTime toDate, string diagnosis)
        {
            FromDate = DateTime.Now;
            ToDate = toDate;
            Diagnosis = diagnosis;
            DoctorName = doctorName;
        }
    }

    public class Arrest
    {
        public int ArrestTime { get; set; }
        public dynamic Cell { get; set; }
        public Vector3 Position { get; set; } = new Vector3();
        public DateTime LastArrest { get; set; }
        public string LastReason { get; set; }

        public Arrest()
        {

        }

        public void Plant(int time, dynamic cell, string reason)
        {
            ArrestTime = time;
            Cell = cell;
            Position = Cell.CellPosition;
            LastArrest = DateTime.Now;
            LastReason = reason;
        }
        public void Free()
        {
            ArrestTime = 0;
            Cell = null;
        }
        public bool InPrision()
        {
            if (Cell == null)
                return false;
            return true;
        }
    }

    public class WantedLevel
    {
        public int Level { get; set; }
        public string WhoGive { get; set; }
        public DateTime Date { get; set; }
        public string Reason { get; set; }

        public WantedLevel(int level, string whoGive, DateTime date, string reason)
        {
            Level = level;
            WhoGive = whoGive;
            Date = date;
            Reason = reason;
        }
    }

    public class ComponentItem
    {
        public int Variation;
        public int Texture;

        public ComponentItem(int variation, int texture = 0)
        {
            Variation = variation;
            Texture = texture;
        }
    }

    public class ClothesData
    {
        /// <summary>
        /// 1
        /// </summary>
        public ComponentItem Mask { get; set; }
        public ComponentItem Gloves { get; set; }
        /// <summary>
        /// 3
        /// </summary>
        public ComponentItem Torso { get; set; }
        /// <summary>
        /// 4
        /// </summary>
        public ComponentItem Leg { get; set; }
        /// <summary>
        /// 5
        /// </summary>
        public ComponentItem Bag { get; set; }
        /// <summary>
        /// 6
        /// </summary>
        public ComponentItem Feet { get; set; }
        /// <summary>
        /// 7
        /// </summary>
        public ComponentItem Accessory { get; set; }
        /// <summary>
        /// 8 - ������ ID �� ������ Underwears (����� ���� �� ����� �������������� ����� variation)
        /// </summary>
        public ComponentItem Undershit { get; set; }
        /// <summary>
        /// 9
        /// </summary>
        public ComponentItem Bodyarmor { get; set; }
        /// <summary>
        /// 10
        /// </summary>
        public ComponentItem Decals { get; set; }
        /// <summary>
        /// 11
        /// </summary>
        public ComponentItem Top { get; set; }

        public ClothesData()
        {
            Mask = new ComponentItem(0, 0);
            Gloves = new ComponentItem(0, 0);
            Torso = new ComponentItem(15, 0);
            Leg = new ComponentItem(21, 0);
            Bag = new ComponentItem(0, 0);
            Feet = new ComponentItem(34, 0);
            Accessory = new ComponentItem(0, 0);
            Undershit = new ComponentItem(15, 0);
            Bodyarmor = new ComponentItem(0, 0);
            Decals = new ComponentItem(0, 0);
            Top = new ComponentItem(15, 0);
        }

        public void Dress(Player player, bool dressArmor = false)
        {
            player.SetClothes(1, Mask.Variation, Mask.Texture);
            player.SetClothes(3, Torso.Variation, Torso.Texture);
            player.SetClothes(4, Leg.Variation, Leg.Texture);
            player.SetClothes(5, Bag.Variation, Bag.Texture);
            player.SetClothes(6, Feet.Variation, Feet.Texture);
            player.SetClothes(7, Accessory.Variation, Accessory.Texture);
            player.SetClothes(8, Undershit.Variation, Undershit.Texture);
            if (dressArmor)
                player.SetClothes(9, Bodyarmor.Variation, Bodyarmor.Texture);
            player.SetClothes(10, Decals.Variation, Decals.Texture);
            player.SetClothes(11, Top.Variation, Top.Texture);

        }
    }

    public class AccessoryData
    {
        public ComponentItem Hat { get; set; }
        public ComponentItem Glasses { get; set; }
        public ComponentItem Ear { get; set; }
        public ComponentItem Watches { get; set; }
        public ComponentItem Bracelets { get; set; }

        public AccessoryData()
        {
            Hat = new ComponentItem(-1, 0);
            Glasses = new ComponentItem(-1, 0);
            Ear = new ComponentItem(-1, 0);
            Watches = new ComponentItem(-1, 0);
            Bracelets = new ComponentItem(-1, 0);
        }
  }

    public class Fraction
    {
        public int FractionID { get; set; }
        public int FractionRankID { get; set; }
        public DateTime FractionInvite { get; set; }
        public List<string> Reprimand { get; set; } = new List<string>();
        //public Clothes Clothes { get; set; } = new Clothes();
        public ClothesData Clothes { get; set; } = new ClothesData();
        public AccessoryData Accessory { get; set; } = new AccessoryData();
        public bool InClothes { get; set; }

        public void UnInvite()
        {
            FractionID = 0;
            FractionRankID = 0;
            FractionInvite = DateTime.MinValue;
            Reprimand = new List<string>();
            Clothes = new ClothesData();
            Accessory = new AccessoryData();
            InClothes = false;
        }
    }

    enum Licenses
    {
        catA = 0,// - Категория A
        catB = 1,// - Категория B
        catC = 2,// - Категория C
        catWater = 3,// - Категория водный транспорт
        catHeli = 4,// - Категория вертолеты
        catPlane = 5,// - Категория самолеты
        gun = 6,// - Оружие (Легкое)
        gun1 = 7,// - Оружие (Тяжелое)
        business = 8,// - Бизнес
        fish = 9,// - Риба
        MillitaryTicket = 10,// - Военый билет
        Hunt = 11,// - охота
        Advocat = 12,// - адвоката
        empty1 = 13,// - none
        empty2 = 14,// - none
    }

    public class BPCharacterInfo
    {
      public int EXP { get; set; } = 0;
      public int LVL { get; set; } = 1;
      public int CHAPTERID { get; set; } = 1;
      public int TYPE { get; set; } = 0;
      public DateTime LastSKIP { get; set; }
      public List<BPQuest> DailyQuests { get; set; } = null;
      public List<BPQuest> CyclicQuests { get; set; } = null;
      public BPSuperQuest SuperQuest { get; set; } = null;
      //public BPGlobalQuest GlobalQuest { get; set; } = null;

      public BPCharacterInfo(int exp, int lvl, int chapterid, List<BPQuest> dailyQuests, List<BPQuest> cyclicQuests, BPSuperQuest superQuest)
      {
        EXP = exp;
        LVL = lvl;
        CHAPTERID = chapterid;
        DailyQuests = dailyQuests;
        CyclicQuests = cyclicQuests;
        SuperQuest = superQuest;
        //GlobalQuest = globalQuest;

        //Console.WriteLine($"[BPCharacterInfo] Updated: exp: {exp} lvl: {lvl} chapterid: {chapterid} dailyQuests: {JsonConvert.SerializeObject(dailyQuests)} cyclicQuests: {JsonConvert.SerializeObject(cyclicQuests)} superQuest: {JsonConvert.SerializeObject(superQuest)}");
      }
    }

    public class BPSeason
    {
      public int ID { get; set; }
      public string Name { get; set; }
      public bool IsActive { get; set; }
      public bool IsFinished { get; set; }
      public int MaxLVL { get; set; }
      public DateTime DateStart { get; set; }
      public DateTime DateEnd { get; set; }

      public BPSeason(int id, string name, bool isActive, bool isFinished, int Maxlvl, DateTime dateStart, DateTime dateEnd)
      {
        ID = id;
        Name = name;
        IsActive = isActive;
        IsFinished = isFinished;
        MaxLVL = Maxlvl;
        DateStart = dateStart;
        DateEnd = dateEnd;
      }
    }

    //public class BPUpgrades
    //{
    //  public int ID { get; set; }
    //  public string Name { get; set; }
    //  public string Uniq_Name { get; set; }
    //  public string Description { get; set; }
    //  public int Price { get; set; }
    //  public int Discount { get; set; }
    //  public bool Show { get; set; }

    //  public BPUpgrades(int id, string name, string uniq_name, string description, int price, int discount, bool show)
    //  {
    //    ID = id;
    //    Name = name;
    //    Description = description;
    //    Price = price;
    //    Discount = discount;
    //    Show = show;
    //    Uniq_Name = uniq_name;
    //  }
    //}

    public class BPExp
    {
      public int ID { get; set; }
      public int Exp { get; set; }
      public int BPCoins { get; set; }

      public BPExp(int id, int exp, int bpcoins)
      {
        ID = id;
        Exp = exp;
        BPCoins = bpcoins;
      }
    }

    public class BPChapter
    {
      public int ID { get; set; }
      public int ChapterID { get; set; }
      public int Season { get; set; }
      public string Name { get; set; }
      public int Levels { get; set; }
      public int ExpForLevel { get; set; }

      public BPChapter(int id, int season, int chapterID, string name, int levels, int expForLevel)
      {
        ID = id;
        Season = season;
        ChapterID = chapterID;
        Name = name;
        Levels = levels;
        ExpForLevel = expForLevel;
      }
    }

    public class BPQuest
    {
      public int ID { get; set; }
      public string Name { get; set; }
      public string Description { get; set; }
      public int Progress { get; set; }
      public float CurrentProgress { get; set; } = 0;
      public int Difficult { get; set; }
      public bool HasConditions { get; set; }
      public bool IsCyclic { get; set; }
      public int Type { get; set; } //?
      public string ColshapeName { get; set; } = "";
      public bool IsComplete { get; set; } = false;
      public bool IsRewarded { get; set; } = false;
      public DateTime? Date_end { get; set; } = null;

      public BPQuest(int id, string name, string description, int progress, int difficult, bool hasConditions, bool isCyclic, int type, string colshapeName, DateTime? time = null)
      {
        ID = id;
        Name = name;
        Description = description;
        Progress = progress;
        Difficult = difficult;
        HasConditions = hasConditions;
        IsCyclic = isCyclic;
        Type = type;
        ColshapeName = colshapeName;
        if (time == null)
          Date_end = DateTime.Now.Date.AddHours(28);
        else Date_end = time;
      }
    }

    public class BPSuperQuest
    {
      public int ID { get; set; }
      public string Name { get; set; }
      public string Description { get; set; }
      public int IDChapter { get; set; }
      public int Progress { get; set; }
      public int RewardExp { get; set; }
      public float CurrentProgress { get; set; } = 0;
      public bool IsComplete { get; set; } = false;
      public bool IsActive { get; set; } = false;
      public bool IsRewarded { get; set; } = false;

      public int Type { get; set; } //?

      public BPSuperQuest(int id, string name, string description, int idChapter, int progress, int rewardExp, int type)
      {
        ID = id;
        Name = name;
        Description = description;
        IDChapter = idChapter;
        Progress = progress;
        RewardExp = rewardExp;
        Type = type;
      }
    }

    public class BPGlobalQuest
    {
      public int ID { get; set; }
      public string Name { get; set; }
      public string Description { get; set; }
      public int Progress { get; set; }
      public int Hours { get; set; }
      public int BudgetEXP { get; set; }
      public int BudgetMoney { get; set; }
      public bool HasExpReward { get; set; }
      public bool HasMoneyReward { get; set; }
      public float CurrentProgress { get; set; } = 0;
      public bool IsComplete { get; set; } = false;
      public bool isRewarded { get; set; } = false;
      public bool IsActive { get; set; } = false;
      public DateTime DateStart { get; set; }
      public DateTime DateEnd { get; set; }
      public int Type { get; set; } //?

      public BPGlobalQuest(int id, bool isActive, string name, string description, int progress, int hours, int budgetEXP, int budgetMoney, bool hasExpReward, bool hasMoneyReward, int type)
      {
        ID = id;
        IsActive = isActive;
        Name = name;
        Description = description;
        Progress = progress;
        Hours = hours;
        BudgetEXP = budgetEXP;
        BudgetMoney = budgetMoney;
        HasExpReward = hasExpReward;
        HasMoneyReward = hasMoneyReward;
        Type = type;
      }
    }

    public class BPGlobalQuestPlayer
    {
      public int UUID { get; set; }
      public string Name { get; set; }
      public string Login { get; set; }
      public float CurrentProgress { get; set; }
      public int BPType { get; set; }

      public BPGlobalQuestPlayer(int uuid, string login, string name, float currentProgress, int bpType)
      {
        UUID = uuid;
        Login = login;
        Name = name;
        CurrentProgress = currentProgress;
        BPType = bpType;
      }
    }

    public class BPReward
    {
      public int ID { get; set; }
      public int SprayExp { get; set; }
      public string Name { get; set; }
      public string Description { get; set; }
      public int Rare { get; set; }
      //public string Img { get; set; }
      public int Count { get; set; }
      public string Type { get; set; }
      public object Additional { get; set; }
      public string RewardType { get; set; }
      public bool isSprayed { get; set; } = true;
      public int Season { get; set; } = 1;

      public BPReward(int id, int season, int sprayExp, int count, string name, string desc, int rare, object type, string rewardType, object additional = null)
      {
        ID = id;
        Season = season;
        SprayExp = sprayExp;
        Name = name;
        Description = desc;
        Rare = rare;
        Count = count;
        Type = type.ToString();
        RewardType = rewardType;
        Additional = additional;
      }
    }

    public class BPPlayerReward
    {
      [JsonProperty("id")]
      public int ID { get; set; }
      [JsonProperty("season")]
      public int Season { get; set; }
      [JsonProperty("rare")]
      public int Rare { get; set; }
      [JsonProperty("img")]
      public string Img { get; set; }
      [JsonProperty("description")]
      public string Description { get; set; }
      [JsonProperty("exp")]
      public int SprayExp { get; set; }
      [JsonProperty("count")]
      public int Count { get; set; }
      [JsonProperty("bpType")]
      public int BpType { get; set; }
      [JsonProperty("lvl")]
      public int LVL { get; set; }
      [JsonProperty("isTaked")]
      public bool IsTaked { get; set; }
      [JsonProperty("isSprayed")]
      public bool IsSprayed { get; set; }
      [JsonProperty("canSprayed")]
      public bool CanSprayed { get; set; }
      [JsonProperty("rewardType")]
      public string RewardType { get; set; }

      public BPPlayerReward(int id, int season, int rare, string img, string desc, int sprayExp, int count, int bpType, int lvl, bool isTaked, bool isSprayed, bool canSprayed, string rewardType)
      {
        ID = id;
        Season = season;
        Rare = rare;
        Img = img;
        Description = desc;
        SprayExp = sprayExp;
        Count = count;
        BpType = bpType;
        LVL = lvl;
        IsTaked = isTaked;
        IsSprayed = isSprayed;
        CanSprayed = canSprayed;
        RewardType = rewardType;
      }
    }

    public class BPType
    {
      public int ID { get; set; }
      public string Name { get; set; }
      public string Uniq_Name { get; set; }
      public string Description { get; set; }
      public int Price { get; set; }
      public int Discount { get; set; }
      public bool Show { get; set; }
      public List<BPReward> RewardList { get; set; }

      public BPType(int id, string name, string uniq_name, string description, int price, int discount, bool show, List<BPReward> rewardList)
      {
        ID = id;
        Name = name;
        Description = description;
        Price = price;
        Discount = discount;
        Show = show;
        Uniq_Name = uniq_name;
        RewardList = rewardList;
      }
    }
}
