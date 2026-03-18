//using GTANetworkAPI;
//using System;
//using System.Collections.Generic;
//using NeptuneEvo.SDK;

//namespace NeptuneEvo.Core
//{
//    class GanGame : Script
//    {
//        private static Dictionary<int, ColShape> Cols = new Dictionary<int, ColShape>();
//        public static List<CarInfo> CarInfos = new List<CarInfo>();
//        private static List<Vector3> gangameCheckpoints = new List<Vector3>()
//        {
//            new Vector3(-621.48505, -1640.7035, 24.154979), //   ���� � �����
//            new Vector3(-1507.5887, -3002.5588, -84.155), // ����� �� �����
//            new Vector3(-1507.745, -2998.4485, -83.165), // �������� ���������
//            new Vector3(-1510.745, -2998.4485, -83.165), // �������� �����
//            new Vector3(-1504.745, -2998.4485, -83.165), // �������� PDW
//        };

//        private static uint privateDimension = 2;

//        private static nLog Log = new nLog("GanGame");

//        [ServerEvent(Event.ResourceStart)]
//        public void onResourceStart()
//        {
//            try
//                {


//                    Blip portblip = NAPI.Blip.CreateBlip(313, gangameCheckpoints[0], 0.91f, 5, Main.StringToU16("����� (GunGame)"), 254, 0, true, 0, 0);

//                    Cols.Add(0, NAPI.ColShape.CreateCylinderColShape(gangameCheckpoints[0], 1, 2, 0)); // ���� � �����
//                    Cols[0].SetData("INTERACT", 144);
//                    Cols[0].OnEntityEnterColShape += gangameShape_onEntityEnterColShape;
//                    Cols[0].OnEntityExitColShape += gangameShape_onEntityExitColShape;
//                    NAPI.TextLabel.CreateTextLabel(Main.StringToU16("������� � ��� �� ����� � �����"), new Vector3(gangameCheckpoints[0].X, gangameCheckpoints[0].Y, gangameCheckpoints[0].Z + 1.5), 20F, 0.5F, 0, new Color(255, 255, 255), false, privateDimension);

//                    Cols.Add(1, NAPI.ColShape.CreateCylinderColShape(gangameCheckpoints[1], 1, 2, privateDimension)); // ����� �� �����
//                    Cols[1].SetData("INTERACT", 145);
//                    Cols[1].OnEntityEnterColShape += gangameShape_onEntityEnterColShape;
//                    Cols[1].OnEntityExitColShape += gangameShape_onEntityExitColShape;
//                    NAPI.TextLabel.CreateTextLabel(Main.StringToU16("������� � ��� �� ����� �� �����"), new Vector3(gangameCheckpoints[1].X, gangameCheckpoints[1].Y, gangameCheckpoints[1].Z + 1.5), 20F, 0.5F, 0, new Color(255, 255, 255), false, privateDimension);

//                    Cols.Add(2, NAPI.ColShape.CreateCylinderColShape(gangameCheckpoints[2], 1, 2, privateDimension)); // ����
//                    Cols[2].SetData("INTERACT", 146);
//                    Cols[2].OnEntityEnterColShape += gangameShape_onEntityEnterColShape;
//                    Cols[2].OnEntityExitColShape += gangameShape_onEntityExitColShape;
//                    NAPI.TextLabel.CreateTextLabel(Main.StringToU16("Revolver \n ~o~��������� 50.000$"), new Vector3(gangameCheckpoints[2].X, gangameCheckpoints[2].Y, gangameCheckpoints[2].Z + 0.5), 20F, 0.5F, 0, new Color(255, 255, 255), false, privateDimension);

//                    Cols.Add(3, NAPI.ColShape.CreateCylinderColShape(gangameCheckpoints[3], 1, 2, privateDimension)); // ����
//                    Cols[3].SetData("INTERACT", 147);
//                    Cols[3].OnEntityEnterColShape += gangameShape_onEntityEnterColShape;
//                    Cols[3].OnEntityExitColShape += gangameShape_onEntityExitColShape;
//                    NAPI.TextLabel.CreateTextLabel(Main.StringToU16("Assault Rifle \n ~o~��������� 65.000$"), new Vector3(gangameCheckpoints[3].X, gangameCheckpoints[3].Y, gangameCheckpoints[3].Z + 0.5), 20F, 0.5F, 0, new Color(255, 255, 255), false, privateDimension);

//                    Cols.Add(4, NAPI.ColShape.CreateCylinderColShape(gangameCheckpoints[4], 1, 2, privateDimension)); // ����
//                    Cols[4].SetData("INTERACT", 148);
//                    Cols[4].OnEntityEnterColShape += gangameShape_onEntityEnterColShape;
//                    Cols[4].OnEntityExitColShape += gangameShape_onEntityExitColShape;
//                    NAPI.TextLabel.CreateTextLabel(Main.StringToU16("Combat PDW \n ~o~��������� 45.000$"), new Vector3(gangameCheckpoints[4].X, gangameCheckpoints[4].Y, gangameCheckpoints[4].Z + 0.5), 20F, 0.5F, 0, new Color(255, 255, 255), false, privateDimension);


//                    NAPI.Marker.CreateMarker(1, gangameCheckpoints[0] - new Vector3(0, 0, -0.4), new Vector3(), new Vector3(), 1, new Color(0, 255, 255));
//                    NAPI.Marker.CreateMarker(1, gangameCheckpoints[1] - new Vector3(), new Vector3(), new Vector3(), 1, new Color(0, 255, 255));
//                    NAPI.Marker.CreateMarker(27, gangameCheckpoints[2] - new Vector3(), new Vector3(), new Vector3(), 1, new Color(0, 255, 255));
//                    NAPI.Marker.CreateMarker(27, gangameCheckpoints[3] - new Vector3(), new Vector3(), new Vector3(), 1, new Color(0, 255, 255));
//                    NAPI.Marker.CreateMarker(27, gangameCheckpoints[4] - new Vector3(), new Vector3(), new Vector3(), 1, new Color(0, 255, 255));

//            }
//            catch (Exception e) { Log.Write("ResourceStart: " + e.StackTrace, nLog.Type.Error); }
//        }
//        #region ColShape
//        private void gangameShape_onEntityEnterColShape(ColShape shape, Player player)
//        {
//            try
//            {
//                //entity.SetMyData("INTERACTIONCHECK", shape.GetData<int>("INTERACT"));
//                player.SetMyData("INTERACTIONCHECK", shape.GetData<int>("INTERACT"));
//            }
//            catch (Exception ex) { Log.Write("svalkaShape_onEntityEnterColShape: " + ex.StackTrace, nLog.Type.Error); }
//        }

//        private void gangameShape_onEntityExitColShape(ColShape shape, Player player)
//        {
//            try
//            {
//                //entity.SetMyData("INTERACTIONCHECK", 0);
//                player.SetMyData("INTERACTIONCHECK", 0);
//            }
//            catch (Exception ex) { Log.Write("svalkaShape_onEntityExitColShape: " + ex.StackTrace, nLog.Type.Error); }
//        }
//        #endregion

//        public static void interactPressed(Player player, int interact)
//        {
//            switch (interact)
//            {
//                case 145:
//                case 144:
//                    if (player.IsInVehicle) return;

//                    if (player.HasMyData("FOLLOWING"))
//                    {
//                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"��� ���-�� ����� �� �����", 3000);
//                        return;
//                    }



//                    if (interact == 144)
//                    {
//                        NAPI.Entity.SetEntityDimension(player, privateDimension);

//                        NAPI.Entity.SetEntityPosition(player, gangameCheckpoints[1] + new Vector3(0, 0, 1.12));
//                    }
//                    else if (interact == 145)
//                    {
//                        NAPI.Entity.SetEntityPosition(player, gangameCheckpoints[0] + new Vector3(0, 0, 1.12));
//                        NAPI.Entity.SetEntityDimension(player, 0);
//                    }
//                    return;
//                case 146:
//                    if (player.IsInVehicle) return;

//                    if (player.HasMyData("FOLLOWING"))
//                    {
//                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"��� ���-�� ����� �� �����", 3000);
//                        return;
//                    }
//                    NAPI.Entity.SetEntityPosition(player, new Vector3(-302.13345, -737.589, 122.877464) + new Vector3(0, 0, 1.12));
//                    //player.SetMyData("GanGame", true);
//                    player.SetMyData("GanGame", true);
//                    NAPI.Entity.SetEntityDimension(player, 2);
//                    var wHash = Weapons.GetHash(ItemType.Revolver.ToString());
//                    Trigger.ClientEvent(player, "wgive", (int)wHash, 120, false, true);
//                    MoneySystem.Wallet.Change(player, -50000);
//                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"�� ������������ � ����.", 3000);
//                    player.SendChatMessage($"�����e ~g~ Revolver.");
//                    player.SendChatMessage($"��� �� ��������� � ����, ����������� ������� ~g~ /arena.");
//                    return;
//                case 147:
//                    if (player.IsInVehicle) return;

//                    if (player.HasMyData("FOLLOWING"))
//                    {
//                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"��� ���-�� ����� �� �����", 3000);
//                        return;
//                    }
//                    NAPI.Entity.SetEntityPosition(player, new Vector3(-302.13345, -737.589, 122.877464) + new Vector3(0, 0, 1.12));
//                    //player.SetMyData("GanGame1", true);
//                    player.SetMyData("GanGame1", true);
//                    NAPI.Entity.SetEntityDimension(player, 3);

//                    wHash = Weapons.GetHash(ItemType.Assaultrifle.ToString());
//                    Trigger.ClientEvent(player, "wgive", (int)wHash, 250, false, true);
//                    MoneySystem.Wallet.Change(player, -65000);
//                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"�� ������������ � ����.", 3000);
//                    player.SendChatMessage($"�����e ~g~ Assault Rifle.");
//                    player.SendChatMessage($"��� �� ��������� � ����, ����������� ������� ~g~ /arena.");
//                    return;
//                case 148:
//                    if (player.IsInVehicle) return;

//                    if (player.HasMyData("FOLLOWING"))
//                    {
//                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"��� ���-�� ����� �� �����", 3000);
//                        return;
//                    }
//                    NAPI.Entity.SetEntityPosition(player, new Vector3(-302.13345, -737.589, 122.877464) + new Vector3(0, 0, 1.12));
//                    //player.SetMyData("GanGame2", true);
//                    player.SetMyData("GanGame2", true);
//                    NAPI.Entity.SetEntityDimension(player, 4);
//                    wHash = Weapons.GetHash(ItemType.Combatpdw.ToString());
//                    Trigger.ClientEvent(player, "wgive", (int)wHash, 250, false, true);
//                    MoneySystem.Wallet.Change(player, -45000);
//                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"�� ������������ � ����.", 3000);
//                    player.SendChatMessage($"�����e ~g~ Combat PDW.");
//                    player.SendChatMessage($"��� �� ��������� � ����, ����������� ������� ~g~ /arena.");
//                    return;
//            }
//        }
//    }
//}
