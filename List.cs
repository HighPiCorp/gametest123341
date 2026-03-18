using GTANetworkAPI;
using NeptuneEvo;
using NeptuneEvo.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace client.GUI
{
    class ListSystem : Script
    {
        public static void Open(Player player, string name, List list)
        {
            if (!Main.Players.ContainsKey(player))
                return;

            List<string> texts = new List<string>();
            foreach (var item in list.Items)
                texts.Add(item.Text);
            player.SetMyData("MENULIST", list);
            Trigger.PlayerEvent(player, "list:open", JsonConvert.SerializeObject(texts), name);
        }

        [RemoteEvent("ListSelect")]
        public static void Event_Callback(Player player, int index)
        {
            if (!Main.Players.ContainsKey(player))
                return;

            if (!player.HasMyData("MENULIST"))
                return;

            List list = player.GetMyData<List>("MENULIST");
            player.ResetMyData("MENULIST");
            Item item = list.Items[index];
            list.Action.Invoke(item.Param);
        }

        public class List
        {
            public Action<object> Action;
            public List<Item> Items;

            public List(Action<object> action, List<Item> items)
            {
                Action = action;
                Items = items;
            }
        }
        public class Item
        {
            public string Text;
            public object Param;

            public Item(string text, object param)
            {
                Text = text;
                Param = param;
            }
        }
    }
}
