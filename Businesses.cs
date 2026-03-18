using GTANetworkAPI;
using Newtonsoft.Json;
using NeptuneEvo.SDK;
using NeptuneEvo.GUI;
using client.Fractions.Government;
using client.Fractions.Utils;
using NeptuneEvo.Jobs;
using MySqlConnector;

namespace NeptuneEvo.Core
{
    public class Business
    {
        public int ID { get; set; }
        public string Owner { get; set; }
        public int SellPrice { get; set; }
        public int Type { get; set; }
        public string Address { get; set; }
        public List<Product> Products { get; set; }
        public int BankID { get; set; }
        public Vector3 EnterPoint { get; set; }
        public Vector3 UnloadPoint { get; set; }
        public int Mafia { get; set; }

        public List<Order> Orders { get; set; }

        public Vector3 ManagePoint { get; set; } = new Vector3();

        public CollectorOrder CollectorsOrder { get; set; }

        public int Cash { get; set; }
        public int Markup { get; set; } = 100;
        public List<int> Profit { get; set; }

        /* new */
        public Ped Ped { get; set; }
        public Vector3 SellPoint { get; set; } = new Vector3();
        public DateTime LastBuy { get; set; }
        //

        [JsonIgnore]
        public Blip blip = null;
        [JsonIgnore]
        private Marker marker = null;
        [JsonIgnore]
        public static Marker markerManageBussiness = null;
        [JsonIgnore]
        private ColShape shape = null;
        [JsonIgnore]
        public static ColShape shapeManageBussiness = null;
        [JsonIgnore]
        private ColShape truckerShape = null;
        //[JsonIgnore]
        //private ColShape sellShape = null;
        //[JsonIgnore]
        //private Marker sellMarker = null;

        private static nLog Log = new nLog("Bussiness");

        public Business(int id, string owner, int sellPrice, int type, List<Product> products, Vector3 enterPoint, Vector3 unloadPoint,
          int bankID, int mafia, List<Order> orders, Vector3 managePoint, CollectorOrder collectorOrder, int cash, Vector3 sellPoint, List<int> profit, int markup)
        {
            ID = id;
            Owner = owner;
            SellPrice = sellPrice;
            Type = type;
            Products = products;
            EnterPoint = enterPoint;
            UnloadPoint = unloadPoint;
            BankID = bankID;
            Mafia = mafia;
            Orders = orders;
            ManagePoint = managePoint;
            CollectorsOrder = collectorOrder;
            Cash = cash;
            SellPoint = sellPoint;
            if (DateTime.Now.Day == 1)
            {
                Profit = new List<int>(new int[31]);
            }
            else
                Profit = profit;
            Markup = markup;
            
            var random = new Random();
            Dictionary<int, int> dict = Owner.Equals("Государство") ? BusinessManager.GovermentOrders : BusinessManager.Orders;

            foreach (var o in orders)
            {
                do
                {
                    o.UID = random.Next(000000, 999999);
                } while (dict.ContainsKey(o.UID));
                dict.Add(o.UID, ID);
            }

            truckerShape = NAPI.ColShape.CreateCylinderColShape(unloadPoint - new Vector3(0, 0, 1), 2, 6, NAPI.GlobalDimension);
            truckerShape.SetData("BIZID", ID);

            truckerShape.OnEntityEnterColShape += (shape, player) =>
            {
                if (Main.Players.ContainsKey(player))
                {
                    if (player.HasMyData("ORDER_BIZ") && player.GetMyData<int>("ORDER_BIZ") == shape.GetData<int>("BIZID"))
                    {
                        if (player.HasMyData("FIRST") && player.IsInVehicle)
                        {
                            if (player.Vehicle != player.GetMyData<Vehicle>("WORK")) return;

                            if (player.GetMyData<int>("ORDER_TYPE") <= 1)
                            {
                                player.ResetMyData("FIRST");
                                player.SetMyData("BOX_UNLOAD", true);

                                player.SetMyData("UNLOAD_BIZID", shape.GetData<int>("BIZID"));
                                Truckers.PutBoxToBiz(player);
                            }
                            else
                            {
                                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, "Выгрузите все ящики из грузовика на метку", 3000);
                                player.ResetMyData("FIRST");
                               // Trigger.ClientEvent(player, "trucker::create_unloading_marker", ManagePoint, player.GetMyData<int>("BOX"), 0);
                                player.SetMyData("UNLOAD_BOX", 0);
                                player.SetMyData("BOX_UNLOAD", true);
                                Trigger.ClientEvent(player, "CLIENT::TRUCKERS:SET_UNLOAD_VEH", player.Vehicle.Handle);
                                Trigger.ClientEvent(player, "deleteCheckpoint", 46);
                                Trigger.ClientEvent(player, "createWorkBlip", ManagePoint, true);
                            }
                        }
                        else
                        {
                        }


                    }


                }
            };

            truckerShape.OnEntityExitColShape += (shape, entity) =>
            {
                entity.SetMyData("INTERACTIONCHECK", 0);
            };

            float range;
            if (Type == 1) range = 10f;
            else if (Type == 12) range = 5f;
            else if (Type == 21) range = 6f;
            else range = 1f;


            shape = NAPI.ColShape.CreateCylinderColShape(EnterPoint, range, 3, NAPI.GlobalDimension);

            shape.OnEntityEnterColShape += (s, player) =>
            {
                try
                {
                    player.SetMyData("INTERACTIONCHECK", 30);
                    player.SetMyData("BIZ_ID", ID);
                    player.SetMyData("LAST_BIZ_ID", ID);


                    player.SetMyData("COLSHAPE_TYPE", "Sim");
                    player.SetMyData("QUEST_COLSHAPE_INTERACTION", 30);

                    //#region quest chapter iteration

                    //QuestSystem.UpdatePlayerQuestIteration(player);

                    //#endregion

                    Main.Interactions[30] = true;
                }
                catch (Exception e) { Console.WriteLine("shape.OnEntityEnterColshape: " + e.StackTrace); }
            };

            shape.OnEntityExitColShape += (s, player) =>
            {
                try
                {
                    player.SetMyData("INTERACTIONCHECK", 0);
                    player.SetMyData("BIZ_ID", -1);
                    player.ResetMyData("COLSHAPE_TYPE");
                    //player.ResetMyData("QUEST_COLSHAPE_INTERACTION");

                    Main.Interactions[30] = false;
                }
                catch (Exception e) { Console.WriteLine("shape.OnEntityEnterColshape: " + e.StackTrace); }
            };

            if (Type != 17 && Type != 22 && Type != 24)
            {
                shapeManageBussiness = NAPI.ColShape.CreateCylinderColShape(ManagePoint, 1.2f, 3, NAPI.GlobalDimension);
                shapeManageBussiness.SetData("BIZID", ID);
                shapeManageBussiness.OnEntityEnterColShape += (s, player) => {
                    try
                    {
                        if (Main.Players[player].WorkID == (int)Jobs.JobTypes.Collector && player.HasMyData("COLLECTBIZ") && player.GetMyData<int>("COLLECTBIZ") == s.GetData<int>("BIZID"))
                        {
                            //NAPI.Data.SetEntityData(player, "BIZ_ID", ID);
                            player.SetMyData("INTERACTIONCHECK", 1014);
                            player.SetMyData("BUSSINESSMANAGE_ID", s.GetData<int>("BIZID"));
                            KeyLabel.Show(player, "E", "взять деньги с кассы");
                        }
                        else if (Main.Players[player].WorkID == (int)Jobs.JobTypes.Truckers && player.HasMyData("ORDER_BIZ") && player.GetMyData<int>("ORDER_BIZ") == s.GetData<int>("BIZID"))
                        {
                            player.SetMyData("UNLOAD_BIZID", s.GetData<int>("BIZID"));
                            player.SetMyData("INTERACTIONCHECK", 1018);
                        }
                        else
                        {
                            player.SetMyData("INTERACTIONCHECK", 1001);
                            player.SetMyData("BUSSINESSMANAGE_ID", s.GetData<int>("BIZID"));
                            player.SetMyData("SELECTEDBIZ", s.GetData<int>("BIZID"));
                        }

                        Log.Debug($"INTERACTIONCHECK IS {player.GetMyData<int>("INTERACTIONCHECK")} ENTER BUSSINESSMANAGE_ID {player.GetMyData<int>("BUSSINESSMANAGE_ID")}");
                    }
                    catch (Exception e) { Console.WriteLine("shapeManageBussiness.OnEntityEnterColshape: " + e.StackTrace); }
                };

                shapeManageBussiness.OnEntityExitColShape += (s, player) => {
                    try
                    {
                        player.SetMyData("BUSSINESSMANAGE_ID", -1);
                        player.SetMyData("SELECTEDBIZ", -1);
                        player.SetMyData("INTERACTIONCHECK", 0);

                        Main.Interactions[1001] = false;

                        Log.Debug($"INTERACTIONCHECK IS {player.GetMyData<int>("INTERACTIONCHECK")} LEAVE BUSSINESSMANAGE_ID {player.GetMyData<int>("BUSSINESSMANAGE_ID")}");
                    }
                    catch (Exception e) { Console.WriteLine("shapeManageBussiness.OnEntityEnterColshape: " + e.StackTrace); }
                };
            }

            if(Type != 17 && Type != 22 && Type != 24) markerManageBussiness = NAPI.Marker.CreateMarker(29, ManagePoint - new Vector3(0, 0, 0.2f), new Vector3(), new Vector3(), 1f, new Color(255, 0, 255), false, 0);

            blip = NAPI.Blip.CreateBlip(Convert.ToUInt32(BusinessManager.BlipByType[Type]), EnterPoint, 1, Convert.ToByte(BusinessManager.BlipColorByType[Type]), Main.StringToU16(BusinessManager.BusinessTypeNames[Type]), 255, 0, true);
            var textrange = (Type == 1) ? 5F : 20F;
           // label = NAPI.TextLabel.CreateTextLabel(Main.StringToU16("Business"), new Vector3(EnterPoint.X, EnterPoint.Y, EnterPoint.Z + 1.5), textrange, 0.5F, 0, new Color(255, 255, 255), true, 0);
            //mafiaLabel = NAPI.TextLabel.CreateTextLabel(Main.StringToU16("Mafia: none"), new Vector3(EnterPoint.X, EnterPoint.Y, EnterPoint.Z + 2), 5F, 0.5F, 0, new Color(255, 255, 255), true, 0);
            UpdateLabel();
            if (Type != 1 && Type != 21 && Type != 12) marker = NAPI.Marker.CreateMarker(1, EnterPoint - new Vector3(0, 0, 0.4f), new Vector3(), new Vector3(), range, new Color(75, 60, 255), false, 0);
        }

        public int TakeTax(int amount)
        {
            //6000 / 100 * 0
            int govTax = (amount / 100) * Cityhall.GovernmentTax;


            Stocks.fracStocks[6].Money += govTax;
            FractionLogs.FractionMoney(6, "", "", govTax, "налоги (НДС)");

            return govTax;
        }

        public void UpdateLabel()
        {
            /*string text = $"~w~{BusinessManager.BusinessTypeNames[Type]}\n~w~Владелец: ~b~{Owner}\n";

            if (Owner != "Государство") text += $"~b~ID: ~w~{ID}\n";
            else text += $"~w~Цена: ~b~{SellPrice}$\n~w~ID: ~b~{ID}\n";

            if (Type == 1)
            {
                text += $"~b~Цена за литр: {GetPriceWithMarkUpInt(Products[0].Price)}$\n";
                text += "~b~Нажмите Е\n";
            }
            label.Text = Main.StringToU16(text);

            if (Mafia != -1) mafiaLabel.Text = $"~w~Крыша: ~b~{Fractions.Manager.getName(Mafia)}";
            else mafiaLabel.Text = "~w~Крыша: ~b~Нет";*/
        }
        public int GetPriceWithMarkUpInt(int price)
        {
            double pr = Convert.ToDouble(price) / 100;

            long ff = Convert.ToInt64(pr * Markup);

            return Convert.ToInt32(ff);
        }

        public void Destroy()
        {
            NAPI.Task.Run(() =>
            {
                try
                {
                    blip.Delete();
                    marker.Delete();
                   // label.Delete();
                    shape.Delete();
                    shapeManageBussiness.Delete();
                    truckerShape.Delete();
                }
                catch (Exception e){ Log.Write(e.StackTrace, nLog.Type.Error); }
            });
        }

        public void Save()
        {
            //MySQL.Query($"UPDATE businesses SET owner='{this.Owner}',sellprice={this.SellPrice}, products='{JsonConvert.SerializeObject(this.Products)}',money={this.BankID},mafia={this.Mafia},orders='{JsonConvert.SerializeObject(this.Orders)}', profit='{JsonConvert.SerializeObject(this.Profit)}', cash={this.Cash}, markup={this.Markup}, last_buy='{MySQL.ConvertTime(this.LastBuy)}' WHERE id={this.ID}");
            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = "UPDATE `businesses` SET " +
              "`owner`=@owner," +
              "`sellprice`=@sellprice," +
              "`products`=@products," +
              "`money`=@money," +
              "`mafia`=@mafia," +
              "`orders`=@orders," +
              "`profit`=@profit," +
              "`cash`=@cash," +
              "`markup`=@markup," +
              "`last_buy`=@last_buy " +
              "WHERE id=@id";
            cmd.Parameters.AddWithValue("@owner", this.Owner);
            cmd.Parameters.AddWithValue("@sellprice", this.SellPrice);
            cmd.Parameters.AddWithValue("@products", JsonConvert.SerializeObject(this.Products));
            cmd.Parameters.AddWithValue("@money", this.BankID);
            cmd.Parameters.AddWithValue("@mafia", this.Mafia);
            cmd.Parameters.AddWithValue("@orders", JsonConvert.SerializeObject(this.Orders));
            cmd.Parameters.AddWithValue("@profit", JsonConvert.SerializeObject(this.Profit));
            cmd.Parameters.AddWithValue("@cash", this.Cash);
            cmd.Parameters.AddWithValue("@markup", this.Markup);
            cmd.Parameters.AddWithValue("@last_buy", MySQL.ConvertTime(this.LastBuy));
            cmd.Parameters.AddWithValue("@id", this.ID);
            MySQL.Query(cmd);

            MoneySystem.Bank.Save(this.BankID);
        }
    }
}
