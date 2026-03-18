using GTANetworkAPI;
using Newtonsoft.Json;
using NeptuneEvo.Core;

namespace NeptuneEvo.GUI
{
    class KeyLabel : Script
    {
        //public static nLog Log = new nLog("KeyLabel");
        public static void Show(Player player, string key, string text)
        {
            List<object> data = new List<object>()
            {
                key,
                text
            };

            player.SetMyData("KEY_PRESS", true);

            Console.WriteLine($"{JsonConvert.SerializeObject(data)}");

            Trigger.PlayerEvent(player, "client_press_key_to", "open", JsonConvert.SerializeObject(data));

            //Console.WriteLine($"{JsonConvert.SerializeObject(data)}");
        }

        public static void Hide(Player player)
        {
            if (!player.HasMyData("KEY_PRESS"))
                return;

            player.ResetMyData("KEY_PRESS");
            
            Trigger.PlayerEvent(player, "client_press_key_to", "close");
        }
    }
}
