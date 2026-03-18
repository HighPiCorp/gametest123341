using System;
using System.Collections.Generic;

namespace NeptuneEvo.SDK
{
    public class AccountData
    {
        public string Login { get; protected set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public string HWID { get; protected set; }
        public string IP { get; protected set; }
        public string SocialClub { get; protected set; }

        public long RedBucks { get; set; }
        public long BPCoins { get; set; }
        //public int VipLvl { get; set; }
        //public DateTime VipDate { get; set; } = DateTime.Now;

        public string PersonalPromocode { get; set; }
        public Dictionary<string, ObjectPromocodeAccount> PromoCodes { get; set; } = new Dictionary<string, ObjectPromocodeAccount>();
        public Dictionary<string, ObjectPromocodeStatusAccount> PromoCodesStatus { get; set; } = new Dictionary<string, ObjectPromocodeStatusAccount>();
        public List<int> Characters { get; protected set; } // characters uuids
        public int? DaysFromLastOnline { get; set; } = null;
        public DateTime DateLastCheckLastOnline { get; set; } = DateTime.Now;
        public bool PresentGet { get; set; } = false;

        public bool isBonused { get; set; } = false;
    }

    public class ObjectDonationsList
    {
      public int id { get; set; }
      public DateTime create_date { get; set; }
      public string login { get; set; }
      public string name { get; set; }
      public int type { get; set; }
      public int value { get; set; }
      public bool isTaked { get; set; }
    }

    public class ObjectPromocode
    {
        public int id { get; set; }
        public string name { get; set; }
        //public string login { get; set; }
        public int count { get; set; }
        public int type { get; set; }
        public bool isRef { get; set; }
        public ObjectOwnerBonus ownerBonus { get; set; } = new ObjectOwnerBonus();
        public ObjectActivateConditions activateConditions { get; set; } = new ObjectActivateConditions();
        public ObjectMoneyBonus moneyBonus { get; set; } = new ObjectMoneyBonus();
        public ObjectVipBonus vipBonus { get; set; } = new ObjectVipBonus();
        public ObjectSWBonus SWBonus { get; set; } = new ObjectSWBonus();
        public ObjectDonateBonus donateBonus { get; set; } = new ObjectDonateBonus();
        public ObjectLimited Limited { get; set; } = new ObjectLimited();
    }

    #region Promocode Player
    public class ObjectPromocodePlayer
    {
        public ObjectPromocodePlayer(int Id, string Name, string Login, int Type, ObjectMoneyBonus MoneyBonus, ObjectVipBonus VipBonus)
        {
            id = Id;
            name = Name;
            login = Login;
            type = Type;
            moneyBonus = MoneyBonus;
            vipBonus = VipBonus;
        }

        public int id { get; set; }
        public string name { get; set; }
        public string login { get; set; }
        public int type { get; set; }
        public ObjectMoneyBonus moneyBonus { get; set; } = new ObjectMoneyBonus();
        public ObjectVipBonus vipBonus { get; set; } = new ObjectVipBonus();
    }

    public class ObjectPromocodeStatusPlayer
    {
        public ObjectPromocodeStatusPlayer(int Id, bool Completed, string Name, string Login, int Type, bool MoneyBonus, bool VipBonus)
        {
            id = Id;
            completed = Completed;
            name = Name;
            login = Login;
            type = Type;
            moneyBonus = MoneyBonus;
            vipBonus = VipBonus;
        }

        public int id { get; set; }
        public bool completed { get; set; }
        public string name { get; set; }
        public string login { get; set; }
        public int type { get; set; }
        public bool moneyBonus { get; set; }
        public bool vipBonus { get; set; }
    }
    #endregion

    #region Promocode Account
    public class ObjectPromocodeAccount
    {
        public ObjectPromocodeAccount(int Id, string Name, string Login, int Type, ObjectSWBonus SwBonus, ObjectDonateBonus DonateBonus, ObjectOwnerBonus OwnerBonus)
        {
            id = Id;
            name = Name;
            login = Login;
            type = Type;
            SWBonus = SwBonus;
            donateBonus = DonateBonus;
            ownerBonus = OwnerBonus;
        }

        public int id { get; set; }
        public string name { get; set; }
        public string login { get; set; }
        public int type { get; set; }
        public ObjectSWBonus SWBonus { get; set; } = new ObjectSWBonus();
        public ObjectDonateBonus donateBonus { get; set; } = new ObjectDonateBonus();
        public ObjectOwnerBonus ownerBonus { get; set; } = new ObjectOwnerBonus();
    }

    public class ObjectPromocodeStatusAccount
    {
        public ObjectPromocodeStatusAccount(int Id, bool Completed, string Name, string Login, int Type, bool SWbonus, bool DonateBonus, bool OwnerBonus)
        {
            id = Id;
            completed = Completed;
            name = Name;
            login = Login;
            type = Type;
            SWBonus = SWbonus;
            donateBonus = DonateBonus;
            ownerBonus = OwnerBonus;
        }

        public int id { get; set; }
        public bool completed { get; set; }
        public string name { get; set; }
        public string login { get; set; }
        public int type { get; set; }
        public bool SWBonus { get; set; }
        public bool donateBonus { get; set; }
        public bool ownerBonus { get; set; }
    }

    public class ObjectPersonalPromocode
    {
        public ObjectPersonalPromocode(string Promocode, List<string> Accounts)
        {
            promocode = Promocode;
            accounts = Accounts;
        }

        public string promocode { get; set; }
        public List<string> accounts { get; set; }
    }
    #endregion

    #region Helpers
    public class ObjectOwnerBonus
    {
        public bool active { get; set; }
        public string login { get; set; }
        public int payType { get; set; }
        public int percent { get; set; }
        public int SWCBonus { get; set; }
        public int moneyBonus { get; set; }
    }

    public class ObjectActivateConditions
  {
        public bool active { get; set; }
        public int daysInActive { get; set; }
    }

    public class ObjectMoneyBonus
    {
        public bool active { get; set; }
        public int level { get; set; }
        public int hours { get; set; }
        public int money { get; set; }
    }

    public class ObjectVipBonus
    {
        public bool active { get; set; }
        public int level { get; set; }
        public int vipLevel { get; set; }
        public int days { get; set; }
    }

    public class ObjectSWBonus
    {
        public bool active { get; set; }
        public int level { get; set; }
        public int hours { get; set; }
        public int money { get; set; }
    }

    public class ObjectDonateBonus
    {
        public bool active { get; set; }
        public int percent { get; set; }
    }

    public class ObjectLimited
    {
        public bool active { get; set; }
        public int count { get; set; }
        public DateTime date { get; set; }
    }
    #endregion

    public class ObjectOnlinePlayer {
        public int ID { get; set; }
        public int UUID { get; set; }
        public int Hours { get; set; } = 0;
        public int Minutes { get; set; } = 0;
        public int Seconds { get; set; } = 0;
        public int FractionID { get; set; } = 0;
        public DateTime InsertDateTime { get; set; } = DateTime.Now;
        public DateTime DateTime { get; set; } = DateTime.Now.Date;
        public DateTime LastUpdate { get; set; } = DateTime.Now;

        public ObjectOnlinePlayer(int id, int uuid, int hours, int minutes, int seconds, int fractionID, DateTime date, DateTime lastUpdate) {
            ID = id;
            UUID = uuid;
            Hours = hours;
            Minutes = minutes;
            Seconds = seconds;
            FractionID = fractionID;
            InsertDateTime = DateTime.Now;
            DateTime = date;
            LastUpdate = lastUpdate;
        }

        public ObjectOnlinePlayer(int id, int uuid, int fractionID) {
            ID = id;
            UUID = uuid;
            FractionID = fractionID;
        }
    }
}
