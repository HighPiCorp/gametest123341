using GTANetworkAPI;

namespace NeptuneEvo.SDK
{
    public class Utils
    {
        public static AccountData GetAccount(Player player)
        {
            //player.GetExternalData<AccountData>(0);
            //return new AccountData();
            return player.GetData<AccountData>("AccData");
        }
        public static CharacterData GetCharacter(Player player)
        {
            //player.GetExternalData<CharacterData>(1);
            //return new CharacterData();
            return player.GetData<CharacterData>("CharData");
        }
    }

}
