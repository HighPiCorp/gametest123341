using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using GTANetworkAPI;
using NeptuneEvo.SDK;
using NeptuneEvo.GUI;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Newtonsoft.Json;
using MySqlConnector;
using System.Linq;
using NeptuneEvo.Houses;
using NeptuneEvo.Systems;
using System.Text.RegularExpressions;
using client.Systems.CraftSystem;
using NeptuneEvo.Families;

namespace NeptuneEvo.Core.nAccount
{
    public class Account : AccountData
    {
        public class ObjectChar
        {
            [JsonProperty("type")]
            public string type { get; set; }

            [JsonProperty("full_name")]
            public string full_name { get; set; }

            [JsonProperty("first_name")]
            public string first_name { get; set; }

            [JsonProperty("second_name")]
            public string second_name { get; set; }

            [JsonProperty("lvl")]
            public int lvl { get; set; }

            [JsonProperty("exp")]
            public int exp { get; set; }

            [JsonProperty("fraction_name")]
            public string fraction_name { get; set; }

            [JsonProperty("money")]
            public long money { get; set; }

            [JsonProperty("bank")]
            public long bank { get; set; }

            [JsonProperty("customization")]
            public object customization { get; set; }

            [JsonProperty("ban")]
            public List<object> ban { get; set; }
        };

        public class ObjectCharBan
        {
            [JsonProperty("name")]
            public string name { get; set; }
            [JsonProperty("lvl")]
            public string lvl { get; set; }
            [JsonProperty("exp")]
            public string exp { get; set; }
            [JsonProperty("type")]
            public string type { get; set; }

            [JsonProperty("reason")]
            public string reason { get; set; }

            [JsonProperty("byAdmin")]
            public string byAdmin { get; set; }

            [JsonProperty("banTime")]
            public string banTime { get; set; }

            [JsonProperty("banUntil")]
            public string banUntil { get; set; }
        };

        private static nLog Log = new nLog("Account");

        public static List<string> MultyRegisterSocialClubs = new List<string>()
        {
          "lFeST1VaLl",
        };

        bool IsValidEmail(string strIn)
		{
			// Return true if strIn is in valid e-mail format.
			return Regex.IsMatch(strIn, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
		}

        public async Task<RegisterEvent> Register(Player client, string login_, string pass_, string email_)
        {
            try
            {
                if (Main.Accounts.ContainsKey(client)) Main.Accounts.Remove(client);

                //if (!MultyRegisterSocialClubs.Contains(client.SocialClubName) && !Main.SocialClubs.Contains(client.GetData<string>("RealSocialClub")))
                //{
                //    if (Main.SocialClubs.Contains(client.SocialClubName) || Main.SocialClubs.Contains(client.GetData<string>("RealSocialClub"))) return RegisterEvent.SocialReg;
                //}

                if (Main.SocialClubs.Contains(client.SocialClubName) || Main.SocialClubs.Contains(client.GetData<string>("RealSocialClub"))) {
                    if (!MultyRegisterSocialClubs.Contains(client.SocialClubName) && !MultyRegisterSocialClubs.Contains(client.GetData<string>("RealSocialClub"))) return RegisterEvent.SocialReg;
                }

                if (login_.Length < 1 || pass_.Length < 1 || email_.Length < 1) return RegisterEvent.DataError;
                if (Main.Usernames.Contains(login_)) return RegisterEvent.UserReg;

                if (!IsValidEmail(email_)) return RegisterEvent.BadEmail;

                if (Main.Emails.ContainsKey(email_)) return RegisterEvent.EmailReg;


                Password = GetSha256(pass_);
                Login = login_;
                Email = email_;

                PersonalPromocode = PromocodesSystem.generatePersonalPromocode(client, login_, null, true, true);
                //PromoCodes = new Dictionary<string, ObjectPromocode>();
                //PromoCodesStatus = new Dictionary<string, ObjectPromocodeStatus>();
                //...
                //if (!string.IsNullOrEmpty(promo_) && NeptuneEvo.Systems.PromocodesSystem.AllPromocodes.ContainsKey(promo_))
                //{
                //    var promocode = NeptuneEvo.Systems.PromocodesSystem.AllPromocodes[promo_];

                //    PromoCodes.Add(promo_, promocode);

                //    PromoCodesStatus.Add(promo_, new ObjectPromocodeStatus(promocode.id, false, promocode.name, promocode.login, promocode.type, false, false, false, false));
                //    MySQL.Query($"UPDATE `promocodes` SET count=count+1 WHERE name='{promo_}'");
                //}

                Characters = new List<int>() { -1, -1, -2 }; // -1 - empty slot, -2 - non-purchased slot

                HWID = client.Serial;
                IP = client.Address;
                SocialClub = client.SocialClubName;
                DaysFromLastOnline = 0;
                DateLastCheckLastOnline = DateTime.Now;

                //await MySQL.QueryAsync($"INSERT INTO `accounts` (`login`,`email`,`password`,`hwid`,`ip`,`personal_promocode`,`socialclub`,`redbucks`,`character1`,`character2`,`character3`) VALUES ('{Login}','{Email}','{Password}','{HWID}','{IP}','{PersonalPromocode}','{SocialClub}',0,-1,-1,-2)");

                MySqlCommand cmd = new MySqlCommand("INSERT INTO `accounts` SET " +
                  "`login`=@login," +
                  " `email`=@email," +
                  " `password`=@password," +
                  " `hwid`=@hwid," +
                  " `ip`=@ip," +
                  " `personal_promocode`=@personal_promocode," +
                  " `socialclub`=@socialclub," +
                  " `redbucks`=@swc," +
                  " `character1`=@ch1," +
                  " `character2`=@ch2," +
                  " `character3`=@ch3," +
                  " `daysFromLastOnline`=@daysFromLastOnline," +
                  " `dateLastCheckLastOnline`=@dateLastCheckLastOnline");

                cmd.Parameters.AddWithValue("@login", Login);
                cmd.Parameters.AddWithValue("@email", Email);
                cmd.Parameters.AddWithValue("@password", Password);
                cmd.Parameters.AddWithValue("@hwid", HWID);
                cmd.Parameters.AddWithValue("@ip", IP);
                cmd.Parameters.AddWithValue("@personal_promocode", PersonalPromocode);
                cmd.Parameters.AddWithValue("@socialclub", SocialClub);
                cmd.Parameters.AddWithValue("@swc", 0);
                cmd.Parameters.AddWithValue("@ch1", -1);
                cmd.Parameters.AddWithValue("@ch2", -1);
                cmd.Parameters.AddWithValue("@ch3", -2);
                cmd.Parameters.AddWithValue("@daysFromLastOnline", DaysFromLastOnline);
                cmd.Parameters.AddWithValue("@dateLastCheckLastOnline", DateLastCheckLastOnline);
                await MySQL.QueryAsync(cmd);

                Main.SocialClubs.Add(SocialClub);
                Main.Usernames.Add(Login);
                Main.Emails.Add(Email, Login);
                Main.Accounts.Add(client, this);

                MoneySystem.Donations.newNames.Enqueue(Login);
                LoadSlots(client);
                if (!Main.LoggedIn.ContainsKey(login_)) Main.LoggedIn.Add(login_, client);
                return RegisterEvent.Registered;
            }
            catch (Exception ex)
            {
                await Log.WriteAsync(ex.ToString(), nLog.Type.Error);
                return RegisterEvent.Error;
            }
        }

        public async Task<LoginEvent> LoginIn(Player client, string login_, string pass_)
        {
            try
            {
                //if (!Main.Usernames.Contains(login_)) return LoginEvent.Refused;
                // На всякий, ибо говнокод
                login_ = login_.ToLower();

                pass_ = GetSha256(pass_);

                MySqlCommand cmd = new MySqlCommand("SELECT * FROM `accounts` WHERE `login`=@lg AND `password`=@pw");
                cmd.Parameters.AddWithValue("@lg", login_);
                cmd.Parameters.AddWithValue("@pw", pass_);
                DataTable result = await MySQL.QueryReadAsync(cmd);

                //Если база не вернула таблицу, то отправляем сброс
                if (result == null || result.Rows.Count == 0) return LoginEvent.Refused;
                //Иначе, парсим строку
                DataRow row = result.Rows[0];
                //Далее делаем разбор и оперируем данными
                Login = Convert.ToString(row["login"]);
                Email = Convert.ToString(row["email"]);
                Password = pass_;
                //Служебные данные
                HWID = client.GetData<string>("RealHWID");
                NAPI.Task.Run(() => IP = client.Address);
                SocialClub = row["socialclub"].ToString();
                isBonused = (bool)row["isbonused"];
                if (client.HasData("RealSocialClub"))
                {
                    var rsc = client.GetData<string>("RealSocialClub");
                }

                if (row["socialclub"].ToString() != "")
                {
                    if (Main.SCCheck) {
                        Log.Debug($"[SocialClub Check: DB: {SocialClub} Client: {client.GetData<string>("RealSocialClub")}");

                        if (client.HasData("RealSocialClub") && SocialClub != client.GetData<string>("RealSocialClub"))
                        {
                          Log.Debug($"[SocialClub Check ERROR] DB: {SocialClub} Client: {client.GetData<string>("RealSocialClub")}", nLog.Type.Error);
                          return LoginEvent.SclubError;
                        }
                    }
                }
                else
                {
                    if (client.HasData("RealSocialClub") && CheckSocialClubInAccounts(client.GetData<string>("RealSocialClub"))) {
                        Log.Debug($"[SocialClub Check ERROR] EMPTY SC DB: {SocialClub} Client: {client.GetData<string>("RealSocialClub")}", nLog.Type.Error);

                        return LoginEvent.SclubError;
                    }

                    SocialClub = client.GetData<string>("RealSocialClub");
                }

                RedBucks = Convert.ToInt32(row["redbucks"]);
                BPCoins = Convert.ToInt32(row["bpcoins"]);

                Log.Debug($"[Login] current SWC: {RedBucks} current BPC: {BPCoins}");

                var personalpromocode = row["personal_promocode"].ToString();
                //Delete this
                if (personalpromocode != "" && personalpromocode != null)
                  PersonalPromocode = PromocodesSystem.generatePersonalPromocode(client, Login, personalpromocode, false, true);
                else
                  PersonalPromocode = PromocodesSystem.generatePersonalPromocode(client, Login, null, true, true);

                PromoCodes = JsonConvert.DeserializeObject<Dictionary<string, ObjectPromocodeAccount>>(row["promocodes"].ToString());
                PromoCodesStatus = JsonConvert.DeserializeObject<Dictionary<string, ObjectPromocodeStatusAccount>>(row["promocodes_status"].ToString());

                var char1 = Convert.ToInt32(row["character1"]);
                var char2 = Convert.ToInt32(row["character2"]);
                var char3 = Convert.ToInt32(row["character3"]);
                Characters = new List<int>() { char1, char2, char3 };

                PresentGet = Convert.ToBoolean(row["present"]);

                DaysFromLastOnline = Convert.ToInt32(row["daysFromLastOnline"]);
                DateLastCheckLastOnline = (DateTime)row["dateLastCheckLastOnline"];

                if (Main.LoggedIn.ContainsKey(login_)) return LoginEvent.Already;
                Main.LoggedIn.Add(login_, client);

                //if(Main.AdminSlots.ContainsKey(SocialClub)) Main.AdminSlots[SocialClub].Logged = true;

                if (Main.Accounts.ContainsKey(client)) return LoginEvent.Already;
                Main.Accounts.Add(client, this);

                return LoginEvent.Authorized;
            }
            catch(Exception ex)
            {
                await Log.WriteAsync(ex.ToString(), nLog.Type.Error);
                return LoginEvent.Error;
            }
        }

        public static void checkDaysInActiveAccount(Player player)
        {
            try
            {
                int? daysFromLastOnline = null;
                if (Main.Accounts.ContainsKey(player))
                {
                    if (Main.Accounts[player].DateLastCheckLastOnline.Date < DateTime.Now.Date)
                    {
                        foreach(var charUUID in Main.Accounts[player].Characters)
                        {
                            if (Main.PlayerSlotsInfo.ContainsKey(charUUID))
                            {
                                DateTime lastOnlineCharacter = Main.PlayerSlotsInfo[charUUID].Item5;

                                var pName = "";
                                if (Main.PlayerNames.ContainsKey(charUUID)) pName = Main.PlayerNames[charUUID];
                                //if (lastOnlineCharacter.Date != DateTime.Now.Date)
                                //{
                                    int daysOfflineCurrentCharacter = (DateTime.Now.Date - lastOnlineCharacter.Date).Days;
                                    Log.Debug($"[daysFromLastOnline][{player.Name}][{pName}] daysOfflineCurrentCharacter: " + daysOfflineCurrentCharacter);
                                    if (daysFromLastOnline == null)
                                    {
                                        Log.Debug($"[daysFromLastOnline][{player.Name}][{pName}] daysFromLastOnline == null, тогда "+ daysOfflineCurrentCharacter);
                                        daysFromLastOnline = daysOfflineCurrentCharacter;
                                        continue;
                                    }

                                    if (daysOfflineCurrentCharacter < daysFromLastOnline) {
                                        Log.Debug($"[daysFromLastOnline][{player.Name}][{pName}] {daysOfflineCurrentCharacter} > {daysFromLastOnline}");
                                        daysFromLastOnline = daysOfflineCurrentCharacter;
                                    }
                                //}
                                //else
                                //{
                                //    Log.Debug($"[daysFromLastOnline][{player.Name}][{pName}] {lastOnlineCharacter.Date} == {DateTime.Now.Date}");
                                //}
                            }
                        }

                        Log.Debug($"[daysFromLastOnline][{player.Name}] days: " + daysFromLastOnline);
                        Main.Accounts[player].DaysFromLastOnline = daysFromLastOnline;
                        Main.Accounts[player].DateLastCheckLastOnline = DateTime.Now;
                    }
                    else
                    {
                        Log.Debug($"[daysFromLastOnline][{player.Name}] DaysFromLastOnline: {Main.Accounts[player].DaysFromLastOnline} DateLastCheckLastOnline: {Main.Accounts[player].DateLastCheckLastOnline}");
                    }
                }
            }
            catch(Exception e) { Log.Debug(e.StackTrace); }
        }

        public static bool CheckSocialClubInAccounts(string SocialClub)
        {
            DataTable data = MySQL.QueryRead($"SELECT * FROM `accounts` WHERE 1");
            foreach (DataRow Row in data.Rows)
            {
                if (Row["socialclub"].ToString() == SocialClub)
                {
                    return true;
                }
            }
            return false;
        }

        public async Task<bool> Save(Player player)
        {
            try
            {
                var promocodes = JsonConvert.SerializeObject(PromoCodes);
                var promocodesStatus = JsonConvert.SerializeObject(PromoCodesStatus);

                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = "UPDATE `accounts` SET " +
                    "`password`=@pass," +
                    "`email`=@email," +
                    "`socialclub`=@sc," +
                    "`redbucks`=@red," +
                    "`bpcoins`=@bpc," +
                    "`hwid`=@hwid," +
                    "`ip`=@ip," +
                    "`character1`=@charf," +
                    "`character2`=@charn," +
                    "`character3`=@charm," +
                    "`promocodes`=@promocodes," +
                    "`promocodes_status`=@promocodes_status," +
                    "`present`=@pres," +
                    "`personal_promocode`=@personalPromocode, " +
                    "`isbonused`=@isBonused, " +
                    "`daysFromLastOnline`=@daysFromLastOnline, " +
                    "`dateLastCheckLastOnline`=@dateLastCheckLastOnline " +
                    "WHERE `login`=@login";

                cmd.Parameters.AddWithValue("@pass", Password);
                cmd.Parameters.AddWithValue("@email", Email);
                cmd.Parameters.AddWithValue("@sc", SocialClub);
                cmd.Parameters.AddWithValue("@red", RedBucks);
                cmd.Parameters.AddWithValue("@bpc", BPCoins);
                //cmd.Parameters.AddWithValue("@vipl", VipLvl);
                cmd.Parameters.AddWithValue("@hwid", HWID);
                cmd.Parameters.AddWithValue("@ip", IP);
                //cmd.Parameters.AddWithValue("@vipd", MySQL.ConvertTime(VipDate));

                cmd.Parameters.AddWithValue("@charf", Characters[0]);
                cmd.Parameters.AddWithValue("@charn", Characters[1]);
                cmd.Parameters.AddWithValue("@charm", Characters[2]);
                cmd.Parameters.AddWithValue("@pres", PresentGet);
                cmd.Parameters.AddWithValue("@login", Login);
                cmd.Parameters.AddWithValue("@personalPromocode", PersonalPromocode);
                cmd.Parameters.AddWithValue("@isBonused", isBonused);
                cmd.Parameters.AddWithValue("@promocodes", promocodes);
                cmd.Parameters.AddWithValue("@promocodes_status", promocodesStatus);
                cmd.Parameters.AddWithValue("@daysFromLastOnline", DaysFromLastOnline);
                cmd.Parameters.AddWithValue("@dateLastCheckLastOnline", DateLastCheckLastOnline);

                await MySQL.QueryAsync(cmd);

                Log.Debug($"[Save ACCOUNT] login: {Login} current SWC: {RedBucks} current BPC: {BPCoins}");

                return true;
            }
            catch (Exception e)
            {
                Log.Write("EXCEPTION AT \"Save\":\n" + e.ToString(), nLog.Type.Error);
                return false;
            }
        }

        public void LoadSlots(Player player)
        {
            try
            {
                List<object> data = new List<object>();
                foreach (int uuid in Characters)
                {
                    if (uuid > -1)
                    {
                        List<object> subData = new List<object>();

                        var ban = Ban.Get2(uuid);
                        if (ban != null && ban.CheckDate())
                        {
                            var name = "";
                            var lvl = 0;
                            var exp = 0;

                            if (Main.PlayerNames.ContainsKey(uuid) && Main.PlayerSlotsInfo.ContainsKey(uuid))
                            {
                                Tuple<int, int, int, long, DateTime> tuple = Main.PlayerSlotsInfo[uuid];
                                lvl = tuple.Item1;
                                exp = tuple.Item2;
                                name = Main.PlayerNames[uuid];
                            }

                            string reason = ban.Reason;
                            string byAdmin = ban.ByAdmin;
                            string banTime = $"{ban.Time.ToShortDateString()} {ban.Time.ToShortTimeString()}";
                            string banUntil = $"{ban.Until.ToShortDateString()} {ban.Until.ToShortTimeString()}";

                            string banData = "{" +
                                "'type':'ban', " +
                                "'full_name':'" + name + "', " +
                                "'lvl':'" + lvl + "', " +
                                "'exp':'" + exp + "', " +
                                "'reason':'" + reason + "', " +
                                "'byAdmin':'" + byAdmin + "', " +
                                "'banTime':'" + banTime + "', " +
                                "'banUntil':'" + banUntil + "" +
                            "'}";

                            ObjectCharBan convertedcharBanData = JsonConvert.DeserializeObject<ObjectCharBan>(banData);

                            data.Add(convertedcharBanData);
                        }
                        else
                        {
                            if (Main.PlayerNames.ContainsKey(uuid) && Main.PlayerSlotsInfo.ContainsKey(uuid))
                            {
                                string name = Main.PlayerNames[uuid];

                                string[] split = name.Split('_');
                                string firstname = split[0];
                                string secondname = split[1];

                                Tuple<int, int, int, long, DateTime> tuple = Main.PlayerSlotsInfo[uuid];
                                int lvl = tuple.Item1;
                                int exp = tuple.Item2;
                                string fractionname = Fractions.Manager.FractionNames[tuple.Item3];
                                long money = tuple.Item4;
                                long bank = 0;

                                if (Main.PlayerBankAccs.ContainsKey(name))
                                {
                                    bank = MoneySystem.Bank.Get(Main.PlayerBankAccs[name]).Balance;
                                }

                                var custom = Customization.CustomPlayerData[uuid];

                                bool gender = custom.Gender == 0 ? true : false; // GENDER IS BROKEN!?
                                var clothes = custom.Clothes;
                                int torsoV, torsoT = 0;
                                var noneGloves = Customization.CorrectTorso[gender][clothes.Top.Variation];

                                if (clothes.Gloves.Variation == 0)
                                    torsoV = noneGloves;
                                else
                                {
                                    torsoV = Customization.CorrectGloves[gender][clothes.Gloves.Variation][noneGloves];
                                    torsoT = clothes.Gloves.Texture;
                                }

                                var hasMask = true;
                                if (!Customization.MaskTypes.ContainsKey(clothes.Mask.Variation) || !Customization.MaskTypes[clothes.Mask.Variation].Item1) hasMask = false;

                                #region apperaceData
                                string apperancesData = "'Appearances':{";

                                for (int i = 0; i < custom.Appearance.Count(); i++)
                                {
                                    if(i == custom.Appearance.Count() - 1)
                                    {
                                        apperancesData += i + ":{" +
                                            "'Value':'" + custom.Appearance[i].Value + "', " +
                                            "'Opacity':'" + custom.Appearance[i].Opacity + "'" +
                                         "}";
                                    } else
                                    {
                                        apperancesData += i + ":{" +
                                            "'Value':'" + custom.Appearance[i].Value + "', " +
                                            "'Opacity':'" + custom.Appearance[i].Opacity + "'" +
                                         "},";
                                    }
                                }

                                apperancesData += "}, ";

                                #endregion

                                #region featuresData

                                string featuresData = "'Features':{";

                                for (int i = 0; i < custom.Features.Count(); i++)
                                {
                                    if (i == custom.Features.Count() - 1)
                                    {
                                        featuresData += i + ":{" +
                                            "'Value':'" + custom.Features[i] + "'" +
                                         "}";
                                    }
                                    else
                                    {
                                        featuresData += i + ":{" +
                                            "'Value':'" + custom.Features[i] + "'" +
                                         "},";
                                    }
                                }

                                featuresData += "}, ";

                                #endregion

                                #region clothesData
                                string clothesData =
                                    "'Clothes':{" +
                                        "0:{" + // mask
                                            "'ComponentID': 1, " +
                                            "'Texture':'" + custom.Clothes.Mask.Texture + "', " +
                                            "'Variation':'" + custom.Clothes.Mask.Variation + "" +
                                        "'}, " +
                                        //"'Gloves':{" + //Gloves
                                        //    "'Texture':'" + custom.Clothes.Gloves.Texture + "', " +
                                        //    "'Variation':'" + custom.Clothes.Gloves.Variation + "" +
                                        //"'}, " +
                                        "1:{" + //Torso
                                            "'ComponentID': 3, " +
                                            "'Texture':'" + torsoT + "', " +
                                            "'Variation':'" + torsoV + "" +
                                        "'}, " +
                                        "2:{" + //Leg
                                            "'ComponentID': 4, " +
                                            "'Texture':'" + custom.Clothes.Leg.Texture + "', " +
                                            "'Variation':'" + custom.Clothes.Leg.Variation + "" +
                                        "'}, " +
                                        "3:{" + //Bag
                                            "'ComponentID': 5, " +
                                            "'Texture':'" + custom.Clothes.Bag.Texture + "', " +
                                            "'Variation':'" + custom.Clothes.Bag.Variation + "" +
                                        "'}, " +
                                        "4:{" + //Feet
                                            "'ComponentID': 6, " +
                                            "'Texture':'" + custom.Clothes.Feet.Texture + "', " +
                                            "'Variation':'" + custom.Clothes.Feet.Variation + "" +
                                        "'}, " +
                                        "5:{" + //Accessory
                                            "'ComponentID': 7, " +
                                            "'Texture':'" + custom.Clothes.Accessory.Texture + "', " +
                                            "'Variation':'" + custom.Clothes.Accessory.Variation + "" +
                                        "'}, " +
                                        "6:{" + //Undershit
                                            "'ComponentID': 8, " +
                                            "'Texture':'" + custom.Clothes.Undershit.Texture + "', " +
                                            "'Variation':'" + custom.Clothes.Undershit.Variation + "" +
                                        "'}, " +
                                        "7:{" + //Bodyarmor
                                            "'ComponentID': 9, " +
                                            "'Texture':'" + custom.Clothes.Bodyarmor.Texture + "', " +
                                            "'Variation':'" + custom.Clothes.Bodyarmor.Variation + "" +
                                        "'}, " +
                                        "8:{" + //Decals
                                            "'ComponentID': 10, " +
                                            "'Texture':'" + custom.Clothes.Decals.Texture + "', " +
                                            "'Variation':'" + custom.Clothes.Decals.Variation + "" +
                                        "'}, " +
                                        "9:{" + //Top
                                            "'ComponentID': 11, " +
                                            "'Texture':'" + custom.Clothes.Top.Texture + "', " +
                                            "'Variation':'" + custom.Clothes.Top.Variation + "" +
                                        "'} " +
                                    "}, ";

                                #endregion

                                #region accessoryData
                                string accessoryData =
                                    "'Accessory':{" +
                                        "0:{" + //Hat
                                            "'ComponentID': 0, " +
                                            "'Texture':'" + custom.Accessory.Hat.Texture + "', " +
                                            "'Variation':'" + custom.Accessory.Hat.Variation + "" +
                                        "'}, " +
                                        "1:{" + //Glasses
                                            "'ComponentID': 1, " +
                                            "'Texture':'" + custom.Accessory.Glasses.Texture + "', " +
                                            "'Variation':'" + custom.Accessory.Glasses.Variation + "" +
                                        "'}, " +
                                        "2:{" + //Ear
                                            "'ComponentID': 2, " +
                                            "'Texture':'" + custom.Accessory.Ear.Texture + "', " +
                                            "'Variation':'" + custom.Accessory.Ear.Variation + "" +
                                        "'}, " +
                                        "3:{" + //Watches
                                            "'ComponentID': 6, " +
                                            "'Texture':'" + custom.Accessory.Watches.Texture + "', " +
                                            "'Variation':'" + custom.Accessory.Watches.Variation + "" +
                                        "'}, " +
                                        "4:{" + //Bracelets
                                            "'ComponentID': 7, " +
                                            "'Texture':'" + custom.Accessory.Bracelets.Texture + "', " +
                                            "'Variation':'" + custom.Accessory.Bracelets.Variation + "" +
                                        "'} " +
                                    "}, ";
                                #endregion

                                #region tattoosData
                                //{ "0":[],"1":[{ "Dictionary":"mpbusiness_overlays","Hash":"MP_Buis_M_Neck_002","Slots":[2]}],"2":[],"3":[],"4":[],"5":[]}
                                //{"0":[],"1":[{"Dictionary":"mpbusiness_overlays","Hash":"MP_Buis_M_Neck_002","Slots":[2]}],"2":[{"Dictionary":"mpstunt_overlays","Hash":"MP_MP_Stunt_Tat_023_M","Slots":[1,2]},{"Dictionary":"mpchristmas2_overlays","Hash":"MP_Xmas2_M_Tat_021","Slots":[0]}],"3":[],"4":[],"5":[]}
                                string tattoosData = "'Tattoos':{";

                                int index = 0;
                                foreach (var tattoo in custom.Tattoos.Values)
                                {
                                    tattoosData += index + ":{";
                                    for(int tIndex = 0; tIndex < tattoo.Count(); tIndex++)
                                    {
                                        var t = tattoo[tIndex];
                                        if (tIndex == tattoo.Count() - 1)
                                        {
                                            tattoosData +=
                                            "'Collection':'"+ NAPI.Util.GetHashKey(t.Dictionary) + "'," +
                                            "'Hash':'" + NAPI.Util.GetHashKey(t.Hash) + "'," +
                                            "'Slots':[" + NAPI.Util.GetHashKey(t.Slots.ToString()) + "]";
                                        }
                                        else
                                        {
                                           tattoosData +=
                                            "'Collection':'"+ NAPI.Util.GetHashKey(t.Dictionary) + "'," +
                                            "'Hash':'" + NAPI.Util.GetHashKey(t.Hash) + "'," +
                                            "'Slots':[" + NAPI.Util.GetHashKey(t.Slots.ToString()) + "],";
                                        }
                                    }
                                    //foreach (var t in tattoo)
                                    //{
                                    //    if (t == null) continue;
                                    //    tattoosData +=
                                    //        "'Collection':'"+ NAPI.Util.GetHashKey(t.Dictionary) + "'," +
                                    //        "'Hash':'" + NAPI.Util.GetHashKey(t.Hash) + "'," +
                                    //        "'Slots':[" + NAPI.Util.GetHashKey(t.Slots.ToString()) + "]";
                                    //}
                                    tattoosData += "}, ";

                                    index++;
                                }

                                tattoosData += "}, ";

                                #endregion

                                //Log.Write("tattoosData: ->>> " + tattoosData, nLog.Type.Error);
                                //Log.Write("Features Count: ->>> " + custom.Features.Count(), nLog.Type.Error);
                                //Log.Write("custom.Gender: ->>>" + custom.Gender);

                                string charData = "{" +
                                    "'type':'character', " +
                                    "'first_name':'" + firstname + "', " +
                                    "'second_name':'" + secondname + "', " +
                                    "'full_name':'" + name + "', " +
                                    "'lvl':'" + lvl + "', " +
                                    "'exp':'" + exp + "', " +
                                    "'fraction_name':'" + fractionname + "', " +
                                    "'money':'" + money + "', " +
                                    "'customization':{" +
                                       "'custom':{" +
                                            "'Gender':'" + custom.Gender + "', " +
                                            "'Parents':{" +
                                                "'Father':" + custom.Parents.Father + ", " +
                                                "'Mother':" + custom.Parents.Mother + ", " +
                                                "'Similarity':'" + custom.Parents.Similarity + "', " +
                                                "'SkinSimilarity':'" + custom.Parents.SkinSimilarity + "'" +
                                            "}, " +
                                            featuresData +
                                            apperancesData +
                                            "'Hair':{" +
                                                "'Hair':'" + custom.Hair.Hair + "', " +
                                                "'Color':'" + custom.Hair.Color + "', " +
                                                "'HighlightColor':'" + custom.Hair.HighlightColor + "" +
                                            "'}, " +
                                            clothesData +
                                            accessoryData +
                                            tattoosData +
                                            "'EyebrowColor':'" + custom.EyebrowColor + "', " +
                                            "'BeardColor':'" + custom.BeardColor + "', " +
                                            "'EyeColor':'" + custom.EyeColor + "', " +
                                            "'BlushColor':'" + custom.BlushColor + "', " +
                                            "'LipstickColor':'" + custom.LipstickColor + "', " +
                                            "'ChestHairColor':'" + custom.ChestHairColor + "', " +
                                            "'IsCreated':'" + custom.IsCreated + "" +
                                        "'}, " +
                                        "'torsoV':'" + torsoV + "', " +
                                        "'torsoT':'" + torsoT + "', " +
                                        "'hasMask':'" + hasMask + "" +
                                    "'}, " +
                                    "'bank':'" + bank + "" +
                                "'}";

                                ObjectChar convertedcharData = JsonConvert.DeserializeObject<ObjectChar>(charData);

                                data.Add(convertedcharData);
                            }
                            else
                            {
                                if (Main.LoggedIn.ContainsKey(Login)) Main.LoggedIn.Remove(Login);
                                //Notify.Send(player, NotifyType.Error, NotifyPosition.Center, $"К сожалению, невозможно получить данные о персонаже с номером паспорта {uuid}, обратитесь в тех.раздел на форуме.", 5000);
                                Trigger.ClientEvent(player, "loginNotify", $"К сожалению, невозможно получить данные о персонаже с номером паспорта {uuid}, обратитесь в тех.раздел на форуме.", "[]");
                                return;
                            }
                        }
                    }
                    else data.Add(uuid);

                }
                data.Add(RedBucks);
                data.Add(Login);
                Trigger.ClientEvent(player, "toslots", JsonConvert.SerializeObject(data));
            }
            catch (Exception e)
            {
                if (Main.LoggedIn.ContainsKey(Login)) Main.LoggedIn.Remove(Login);
                //Notify.Send(player, NotifyType.Error, NotifyPosition.Center, "К сожалению, невозможно получить данные о персонажах аккаунта, сообщите в тех.раздел на форуме.", 5000);
                Trigger.ClientEvent(player, "loginNotify", $"К сожалению, невозможно получить данные о персонажах аккаунта, обратитесь в тех.раздел на форуме.", "[]");
                Log.Write("EXCEPTION AT \"LoadSlots\":\n" + e.ToString(), nLog.Type.Error);
                return;
            }
        }

        public async Task CreateCharacter(Player player, int slot, string firstName, string lastName)
        {
            if (Characters[slot - 1] != -1) return;
            var character = new Character.Character();
            var result = await character.Create(player, firstName, lastName);
            if (result == -1) return;

            Characters[slot - 1] = result;

            MySqlCommand cmd = new MySqlCommand($"UPDATE `accounts` SET `character{slot}`=@character WHERE `login`=@login");
            cmd.Parameters.AddWithValue("@login", Login);
            cmd.Parameters.AddWithValue("@character", result);
            await MySQL.QueryAsync(cmd);

            Main.Players[player].Spawn(player);
        }

        public async Task DeleteCharacter(Player player, int slot, string firstName_, string lastName_, string password_)
        {
            Log.Debug("DeleteCharacter SLOT: "+slot);
            if (Characters[slot] == -1 || Characters[slot] == -2) return;

            MySqlCommand cmd = new MySqlCommand($"SELECT `firstname`,`lastname`,`biz`,`sim`,`bank` FROM `characters` WHERE `uuid`=@uuid");
            cmd.Parameters.AddWithValue("@uuid", Characters[slot]);
            var result = await MySQL.QueryReadAsync(cmd);


            if (result == null || result.Rows.Count == 0) return;

            Ban ban = Ban.Get2(Characters[slot]);
            if (ban != null && ban.CheckDate()) {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Невозможно удалить персонажа, который находится в бане.", 3000);
                return;
            }

            var row = result.Rows[0];
            var firstName = row["firstname"].ToString();
            var lastName = row["lastname"].ToString();
            var biz = JsonConvert.DeserializeObject<List<int>>(row["biz"].ToString());
            var sim = Convert.ToInt32(row["sim"]);
            var bank = Convert.ToInt32(row["bank"]);
            var uuid = Characters[slot];

            if (firstName != firstName_ || lastName != lastName_)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Имя и фамилия не соответствуют персонажу на выбранном слоте: {firstName} {lastName}", 3000);
                return;
            }

            password_ = GetSha256(password_);
            if (Password != password_)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Неправильный пароль от аккаунта", 3000);
                return;
            }

            foreach (var b in biz)
                BusinessManager.changeOwner($"{firstName}_{lastName}", "Государство");

            var house = HouseManager.GetHouse($"{firstName}_{lastName}");
            if (house != null)
            {
                house.changeOwner("");
            }

            MySqlCommand cmd2 = new MySqlCommand($"DELETE FROM `customization` WHERE `uuid`=@uuid");
            cmd2.Parameters.AddWithValue("@uuid", uuid);
            await MySQL.QueryAsync(cmd2);

            nInventory.Items.Remove(uuid);

            MySqlCommand cmd3 = new MySqlCommand($"DELETE FROM `inventory` WHERE `uuid`=@uuid");
            cmd3.Parameters.AddWithValue("@uuid", uuid);
            await MySQL.QueryAsync(cmd3);

            MoneySystem.Bank.Remove(bank, $"{firstName}_{lastName}");

            var vehicles = VehicleManager.getAllPlayerVehicles($"{firstName}_{lastName}");
            foreach (var v in vehicles)
                VehicleManager.Remove(v);

            MySqlCommand cmd4 = new MySqlCommand($"DELETE FROM `characters` WHERE `uuid`=@uuid");
            cmd4.Parameters.AddWithValue("@uuid", uuid);
            await MySQL.QueryAsync(cmd4);

            CraftSystem.PlayersCraftData.Remove(uuid);
            CraftSystem.PlayersHomeCraftData.Remove(uuid);
            MySqlCommand cmd5 = new MySqlCommand($"DELETE FROM `craftdata` WHERE `uuid`=@uuid");
            cmd5.Parameters.AddWithValue("@uuid", uuid);
            await MySQL.QueryAsync(cmd5);

            try
            {
                foreach(var item in NeptuneEvo.Families.Manager.Families)
                {
                    var member = item.Players.FirstOrDefault(x => x.Name == $"{firstName}_{lastName}");
                    if (member != null)
                    {
                        item.Players.Remove(member);
                        NeptuneEvo.Families.Family.SaveFamily(item);
                        
                    }
                }
                if (NeptuneEvo.Fractions.Manager.isHaveFraction(player))
                {
                    var fracmember = NeptuneEvo.Fractions.Manager.AllMembers.FirstOrDefault(member => member.Name == $"{firstName}_{lastName}");
                    NeptuneEvo.Fractions.Manager.AllMembers.Remove(fracmember);
                }
            }catch(Exception ex) { Log.Debug(ex.StackTrace.ToString()); }

            Main.UUIDs.Remove(uuid);
            Main.PlayerNames.Remove(uuid);
            Main.PlayerUUIDs.Remove($"{firstName}_{lastName}");
            Main.PlayerBankAccs.Remove($"{firstName}_{lastName}");
            Main.SimCards.Remove(sim);
            Main.PlayerSlotsInfo.Remove(uuid);
            Customization.CustomPlayerData.Remove(uuid);


            Characters[slot] = -1;

            MySqlCommand cmd6 = new MySqlCommand($"UPDATE `accounts` SET `character{slot + 1}`=@ch WHERE `login`=@login");
            cmd6.Parameters.AddWithValue("@login", Login);
            cmd6.Parameters.AddWithValue("@ch", -1);
            await MySQL.QueryAsync(cmd6);

            GameLog.CharacterDelete($"{firstName}_{lastName}", uuid, Login);

            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Персонаж {firstName} {lastName} успешно удален", 3000);
            NAPI.Task.Run(() => Trigger.ClientEvent(player, "delCharSuccess", slot));
        }

        public void changePassword(string newPass)
        {
            Password = GetSha256(newPass);
            //TODO: Logging ths action
        }

        public void changeIP(string newAdress)
        {
            IP = newAdress;
        }

        public void changeHWID(string newHWID)
        {
            HWID = newHWID;
        }

        public static string GetSha256(string strData)
        {
            var message = Encoding.ASCII.GetBytes(strData);
            var hashString = new SHA256Managed();
            var hex = "";

            var hashValue = hashString.ComputeHash(message);
            foreach (var x in hashValue)
                hex += string.Format("{0:x2}", x);
            return hex;
        }
    }

    public enum LoginEvent
    {
        Already,
        Authorized,
        Refused,
        SclubError,
        Error
    }
    public enum RegisterEvent
    {
        Registered,
        SocialReg,
        UserReg,
        EmailReg,
        DataError,
        Error,
        BadEmail
    }
}
