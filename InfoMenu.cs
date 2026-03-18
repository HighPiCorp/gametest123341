using GTANetworkAPI;
using NeptuneEvo;
using NeptuneEvo.Core;
using NeptuneEvo.Fractions;
using NeptuneEvo.SDK;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Trigger = NeptuneEvo.Trigger;

namespace client.GUI
{
    class InfoMenu : Script
    {
        const string Data = "INFODATA";
        const string DataCancel = "INFODATACANCEL";

        public static void Open(Player player, string title, List<Text> text, List<Button> buttons, Action cancel = null)
        {
            List<object> dataText = new List<object>();
            foreach (var item in text)
                dataText.Add(item.ToObject());
            List<object> dataButton = new List<object>();
            foreach (var item in buttons)
                dataButton.Add(item.Title);

            player.SetMyData(Data, buttons);
            player.SetMyData(DataCancel, cancel);

            Trigger.PlayerEvent(player, "info:open", title, JsonConvert.SerializeObject(dataText), JsonConvert.SerializeObject(dataButton));
        }

        [RemoteEvent("info:callback")]
        public static void Event_CallBack(Player player, int button)
        {
            if (!player.HasMyData(Data) || !player.HasMyData(DataCancel))
                return;

            if (button == -1)
            {
                player.GetMyData<Action>(DataCancel)?.Invoke();
            }
            else
            {
                List<Button> buttons = player.GetMyData<List<Button>>(Data);

                buttons[button].Action?.Invoke();
            }

            player.ResetMyData(Data);
            player.ResetMyData(DataCancel);
        }


        public class Text
        {
            string Title;
            string _Text;
            public Text(string title, string text)
            {
                Title = title;
                _Text = text;
            }

            public Text(string title, int text)
            {
                Title = title;
                _Text = text.ToString();
            }
            public object ToObject()
            {
                return new object[] { Title, _Text };
            }
        }

        public class Button
        {
            public string Title;
            public Action Action;
            public Button(string title, Action action = null)
            {
                Title = title;
                Action = action;
            }
            public object ToObject()
            {
                return new object[] { Title };
            }
        }
    }
}
