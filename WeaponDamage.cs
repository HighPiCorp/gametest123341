using client.Fractions.Government;
using client.Systems.Arena.Games;
using GTANetworkAPI;
using NeptuneEvo;
using NeptuneEvo.Core;
using NeptuneEvo.SDK;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace client.Core
{
    class WeaponDamage : Script
    {
        private static nLog Log = new nLog("WeaponDamage");

        [RemoteEvent("InconmingDamage")]
        public static void onDamagePlayer(Player from, Player target, int damage)
        {
            if (target == null && from == null) return;

            if (!Main.Players.ContainsKey(target)) return;
            if (!Main.Players.ContainsKey(from)) return;

            if (target.HasMyData("AMG") && target.GetMyData<bool>("AGM")) return;
            if (from.HasMyData("ARENADYING") && from.GetMyData<bool>("ARENADYING"))return;
            NAPI.Task.Run(() => {
                try
                {
                    if (!nInventory.Items.ContainsKey(Main.Players[target].UUID)) return;

                    nItem aItem = nInventory.Items[Main.Players[target].UUID].FirstOrDefault(item => item.Type == ItemType.BodyArmor && item.IsActive == true);
                    //Log.Debug($"[InconmingDamage] FROM: {from.Name} TARGET: {target.Name} DMG: {damage} BodyArmorItem: {JsonConvert.SerializeObject(aItem)}");
                    if (aItem != null)
                    {
                        if (damage >= target.Armor)
                        {
                            NAPI.Player.SetPlayerArmor(target, 0);
                            var dmgInHealth = damage - target.Armor;
                            if (dmgInHealth > 0)
                            {
                                NAPI.Player.SetPlayerHealth(target, target.Health - dmgInHealth);
                            }

                            aItem.Data = $"12_1_{Main.Players[target].Gender}_{target.Armor}";

                            nInventory.Remove(target, aItem);

                            target.TriggerEvent("AGMS", false);
                            target.ResetSharedData("HASARMOR");
                        }
                        else
                        {
                            aItem.Data = $"12_1_{Main.Players[target].Gender}_{target.Armor - damage}";
                            NAPI.Player.SetPlayerArmor(target, target.Armor - damage);
                            target.TriggerEvent("AGMS", false);
                        }
                    }
                    else
                    {
                        if (damage >= target.Health)
                        {
                            if (target.HasMyData("NEWGUNGAME"))
                            {
                                if (target.HasMyData("ARENADYING") && target.GetMyData<bool>("ARENADYING")) 
                                    return;
                                target.SetMyData<bool>("ARENADYING", true);
                                GameManager.Games[target.GetMyData<uint>("NEWGUNGAME")].onPlayerDeath(target, from);
                            }
                            else
                            {
                                target.TriggerEvent("AGMS", false);
                                target.Kill();
                            }
                        }
                        else
                        {
                            NAPI.Player.SetPlayerHealth(target, target.Health - damage);
                            target.TriggerEvent("AGMS", false);
                        }
                    }
                }catch(Exception ex) { Log.Debug(ex.StackTrace.ToString()); }
            });
        }
    }
}
