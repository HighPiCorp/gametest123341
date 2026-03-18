using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Linq;

namespace NeptuneEvo.SDK
{
    public static class Trigger
    {

        public static void PlayerEvent(Player Player, string eventName, params object[] args)
        {
            ClientEvent(Player, eventName, args);
        }
        public static void PlayerEventInRange(Vector3 pos, float range, string eventName, params object[] args)
        {
            ClientEventInRange(pos, range, eventName, args);
        }

        public static void PlayerEventInRange2D(Vector3 pos, float range, string eventName, params object[] args)
        {
            PlayerEventToPlayers(NAPI.Pools.GetAllPlayers().ToList().FindAll(f => f.Position.DistanceTo2D(pos) <= range).ToArray(), eventName, args);
        }

        public static void PlayerEventInDimension(uint dim, string eventName, params object[] args)
        {
            ClientEventInDimension(dim, eventName, args);
        }
        public static void PlayerEventToPlayers(Player[] players, string eventName, params object[] args)
        {
            ClientEventToPlayers(players, eventName, args);
        }
        public static void ClientEvent(Player client, string eventName, params object[] args)
        {
            if (Thread.CurrentThread.Name == "Main")
            {
                NAPI.ClientEvent.TriggerClientEvent(client, eventName, args);
                return;
            }
            NAPI.Task.Run(() =>
            {
                if (client == null) return;
                NAPI.ClientEvent.TriggerClientEvent(client, eventName, args);
            });
        }
        public static void ClientEventInRange(Vector3 pos, float range, string eventName, params object[] args)
        {
            if (Thread.CurrentThread.Name == "Main")
            {
                NAPI.ClientEvent.TriggerClientEventInRange(pos, range, eventName, args);
                return;
            }
            NAPI.Task.Run(() =>
            {
                NAPI.ClientEvent.TriggerClientEventInRange(pos, range, eventName, args);
            });
        }
        public static void ClientEventInDimension(uint dim, string eventName, params object[] args)
        {
            if (Thread.CurrentThread.Name == "Main")
            {
                NAPI.ClientEvent.TriggerClientEventInDimension(dim, eventName, args);
                return;
            }
            NAPI.Task.Run(() =>
            {
                NAPI.ClientEvent.TriggerClientEventInDimension(dim, eventName, args);
            });
        }
        public static void ClientEventToPlayers(Player[] players, string eventName, params object[] args)
        {
            if (Thread.CurrentThread.Name == "Main")
            {
                NAPI.ClientEvent.TriggerClientEventToPlayers(players, eventName, args);
                return;
            }
            NAPI.Task.Run(() =>
            {
                NAPI.ClientEvent.TriggerClientEventToPlayers(players, eventName, args);
            });
        }
    }
}
