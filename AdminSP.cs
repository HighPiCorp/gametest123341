using GTANetworkAPI;
using NeptuneEvo.SDK;

namespace NeptuneEvo.Core
{
    class AdminSP : Script
    {
        [RemoteEvent("SpectateSelect")]
        public static void SpectatePrevNext(Player player, bool state)
        {
            if (!Main.Players.ContainsKey(player)) return;
            if (!Group.CanUseCmd(player, "sp")) return;
            int target = player.GetMyData<int>("spclient"); // Лучше вызвать GetData<object> один раз, чем несколько, т.к. SetData/GetData<object> работают медленно.
            if (target != -1)
            {
                int id = 0;
                if (!state)
                {
                    id = (target - 1);
                    if (id == player.Value) id--; // Пропускаем свой ID, т.к. следить за собой мы не можем
                }
                else
                {
                    id = (target + 1);
                    if (id == player.Value) id++; // Пропускаем свой ID, т.к. следить за собой мы не можем
                }
                Spectate(player, id);
            }
            else player.SendChatMessage("Невозможно переключиться на другого игрока.");
        }

        public static void Spectate(Player player, int id)
        {
            if (Main.Players.ContainsKey(player))
            {
                if (id >= 0 && id < NAPI.Server.GetMaxPlayers())
                {
                    Player target = Main.GetPlayerByID(id);
                    if (target != null)
                    {
                        if (target != player)
                        {
                            if (Main.Players.ContainsKey(target))
                            {
                                if (target.GetMyData<bool>("spmode") == false)
                                {
                                    if (player.GetMyData<bool>("spmode") == false)
                                    { // Не сохраняем новые данные о позиции, если мы уже в режиме слежки
                                        player.SetMyData("sppos", player.Position);
                                        player.SetMyData("spdim", player.Dimension);
                                    }
                                    else NAPI.ClientEvent.TriggerClientEvent(player, "spmode", null, false); // Если уже за кем-то SPшит и потом на другюго, то сначала deattach
                                    player.SetSharedData("INVISIBLE", true); // Ваша переменная с Вашей системы инвизов, чтобы игроки не видели ника над головой
                                    player.SetMyData("spmode", true);
                                    player.SetMyData("spclient", target.Value);
                                    player.Transparency = 0; // Сначала устанавливаем игроку полную прозрачность, а только потом телепортируем к игроку
                                    player.Dimension = target.Dimension;
                                    player.Position = new Vector3(target.Position.X, target.Position.Y, (target.Position.Z + 3)); // Сначала телепортируем к игроку, чтобы он загрузился
                                    NAPI.ClientEvent.TriggerClientEvent(player, "spmode", target, true); // И только потом аттачим админа к игроку
                                    player.SendChatMessage("Вы наблюдаете за " + target.Name + " [ID: " + target.Value + "].");
                                }
                            }
                            else player.SendChatMessage("Игрок под данным ID еще не авторизовался.");
                        }
                    }
                    else player.SendChatMessage("Игрок под ID " + id + " отсутствует.");
                }
                else player.SendChatMessage("ID игрока недействительно (меньше 0 или больше количества слотов).");
            }
        }

        [RemoteEvent("UnSpectate")]
        public static void RemoteUnSpectate(Player player)
        {
            if (!Main.Players.ContainsKey(player)) return;
            if (!Group.CanUseCmd(player, "sp")) return;
            UnSpectate(player);
        }

        public static void UnSpectate(Player player)
        {
            if (Main.Players.ContainsKey(player))
            {
                if (player.GetMyData<bool>("spmode") == true)
                {
                    NAPI.ClientEvent.TriggerClientEvent(player, "spmode", null, false);
                    player.SetMyData("spclient", -1);
                    Timers.StartOnce(400, () => {
                        NAPI.Task.Run(() => {
                            player.Dimension = player.GetMyData<uint>("spdim");
                            player.Position = player.GetMyData<Vector3>("sppos"); // Сначала возвращаем игрока на исходное местоположение, а только потом восстанавливаем прозрачность
                            player.Transparency = 255;
                            player.SetSharedData("INVISIBLE", false); // Включаем видимость ника и отключаем отображение хп всех игроков рядом
                            player.SetMyData("spmode", false);
                            player.SendChatMessage("Вы вышли из режима наблюдателя.");
                        });
                    });
                }
                else player.SendChatMessage("Вы не находитесь в режиме наблюдателя.");
            }
        }
    }
}
