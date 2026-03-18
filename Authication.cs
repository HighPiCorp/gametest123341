using GTANetworkAPI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using NeptuneEvo.Core.nAccount;
using NeptuneEvo.Core.Character;
using System.Linq;
using NeptuneEvo.SDK;
using System.Data;
using NeptuneEvo.Houses;
using MySqlConnector;
using System.Security.Cryptography;
using System.Text;

namespace NeptuneEvo.Core
{
  class Authication : Script
  {
    private static nLog Log = new nLog("Authication");

    [RemoteEvent("selectchar")]
    public async void ClientEvent_selectCharacter(Player player, params object[] arguments)
    {
      try
      {
        if (!Main.Accounts.ContainsKey(player)) return;
        await Log.WriteAsync($"{player.Name} select char");

        int slot = Convert.ToInt32(arguments[0].ToString());
        await SelecterCharacterOnTimer(player, player.Value, slot);
      }
      catch (Exception e) { Log.Write("newchar: " + e.StackTrace, nLog.Type.Error); }
    }

    public async Task SelecterCharacterOnTimer(Player player, int value, int slot)
    {
      try
      {
        if (player.Value != value) return;
        if (!Main.Accounts.ContainsKey(player)) return;

        Ban ban = Ban.Get2(Main.Accounts[player].Characters[slot]);
        if (ban != null)
        {
          Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, "Ты не пройдёшь!", 4000);
          return;
        }

        Core.Character.Character character = new Core.Character.Character();
        await character.Load(player, Main.Accounts[player].Characters[slot]);
        return;
      }
      catch (Exception e) { Log.Write("selectcharTimer: " + e.StackTrace, nLog.Type.Error); }
    }

    [RemoteEvent("newchar")]
    public async Task ClientEvent_newCharacter(Player player, params object[] arguments)
    {
      try
      {
        if (!Main.Accounts.ContainsKey(player)) return;

        int slot = Convert.ToInt32(arguments[0].ToString());
        string firstname = arguments[1].ToString();
        string lastname = arguments[2].ToString();
        string promo = arguments[3].ToString(); // todo

        Log.Write($"Register {firstname} {lastname} {promo}");

        await Main.Accounts[player].CreateCharacter(player, slot, firstname, lastname);
        return;
      }
      catch (Exception e) { Log.Write("newchar: " + e.StackTrace, nLog.Type.Error); }
    }

    [RemoteEvent("delchar")]
    public async Task ClientEvent_deleteCharacter(Player player, params object[] arguments)
    {
      try
      {
        if (!Main.Accounts.ContainsKey(player)) return;

        int slot = Convert.ToInt32(arguments[0].ToString());
        string firstname = arguments[1].ToString();
        string lastname = arguments[2].ToString();
        string pass = arguments[3].ToString();
        await Main.Accounts[player].DeleteCharacter(player, slot, firstname, lastname, pass);
        return;
      }
      catch (Exception e) { Log.Write("transferchar: " + e.StackTrace, nLog.Type.Error); }
    }

    [RemoteEvent("SERVER::auth:restorePassword")]
    public static void RemoteEvent_RestorePassword(Player player, string loginORemail)
    {
      try
      {
        string login = "";
        string email = "";
        string sc = "";

        MySqlCommand cmd = new MySqlCommand();
        string SQL = "SELECT `email`, `socialclub`, `login` FROM `accounts` WHERE ";

        if (Main.Emails.ContainsKey(loginORemail))
        {
          email = Main.Emails[loginORemail];
          SQL += "`email` = @email";

          cmd.Parameters.AddWithValue("@email", loginORemail);
        }
        else
        {
          SQL += "`login` = @login";
          cmd.Parameters.AddWithValue("@login", loginORemail);
        }

        cmd.CommandText = SQL;

        DataTable result = MySQL.QueryRead(cmd, true);

        if (result != null && result.Rows.Count != 0)
        {
          DataRow row = result.Rows[0];

          login = Convert.ToString(row["login"]);
          email = Convert.ToString(row["email"]);
          sc = row["socialclub"].ToString();

          if (sc != "" && sc != player.GetData<string>("RealSocialClub"))
          {
            Log.Debug($"SocialClub не соответствует SocialClub при регистрации", nLog.Type.Warn);

            Trigger.ClientEvent(player, "loginNotify", "SocialClub не соответствует SocialClub при регистрации", "[]");
            return;
          }
        }
        //md5(date("d.m.Y")."001".$login);

        DateTime todayDate = DateTime.Now;
        string formatedDate = todayDate.ToString("d.MM.yyyy");

        //if (login == "test") login = "fest1val";

        string stringForMD5 = formatedDate + "001" + login;

        string md5 = getMD5(formatedDate + "001" + login);

        //Log.Debug($"[RESTORE PASSWORD] String: {stringForMD5} MD5: {md5}");

        Trigger.ClientEvent(player, "CLIENT::auth:restorePassword", login, md5);

      } catch(Exception e) { Log.Write(e.StackTrace); }      
    }

    public static string getMD5(string message)
    {
      using (MD5 md5 = MD5.Create())
      {
        byte[] input = Encoding.ASCII.GetBytes(message);
        byte[] hash = md5.ComputeHash(input);

        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < hash.Length; i++)
        {
          sb.Append(hash[i].ToString("X2"));
        }
        return sb.ToString().ToLower();
      }
    }

    //[RemoteEvent("restorepass")]
    //public async void RestorePassword_event(Player client, byte state, string loginorcode)
    //{
    //  try
    //  {
    //    if (state == 0)
    //    { // Отправка кода
    //      if (Main.Emails.ContainsKey(loginorcode)) loginorcode = Main.Emails[loginorcode];
    //      else loginorcode = loginorcode.ToLower();

    //      //DataTable result = MySQL.QueryRead($"SELECT email, socialclub FROM `accounts` WHERE `login`='{loginorcode}'");

    //      MySqlCommand cmd = new MySqlCommand();
    //      cmd.CommandText = "SELECT `email`, `socialclub` FROM `accounts` WHERE `login`=@login";

    //      cmd.Parameters.AddWithValue("@login", loginorcode);
    //      DataTable result = MySQL.QueryRead(cmd);

    //      if (result == null || result.Rows.Count == 0)
    //      {
    //        Log.Debug($"Ошибка при попытке восстановить пароль от аккаунта!", nLog.Type.Warn);
    //        return;
    //      }
    //      DataRow row = result.Rows[0];
    //      string email = Convert.ToString(row["email"]);
    //      string sc = row["socialclub"].ToString();
    //      if (sc != client.GetData<string>("RealSocialClub"))
    //      {
    //        Log.Debug($"SocialClub не соответствует SocialClub при регистрации", nLog.Type.Warn);
    //        Trigger.ClientEvent(client, "loginNotify", "SocialClub не соответствует SocialClub при регистрации", "[]");
    //        return;
    //      }
    //      int mycode = Main.rnd.Next(1000, 10000);
    //      if (Main.RestorePass.ContainsKey(client)) Main.RestorePass.Remove(client);
    //      Main.RestorePass.Add(client, new Tuple<int, string, string, string>(mycode, loginorcode, client.GetData<string>("RealSocialClub"), email));
    //      await Task.Run(() => {
    //        PasswordRestore.SendEmail(0, email, mycode); // Отправляем сообщение на емейл с кодом для смены пароля
    //                                                     //Notify.Send(client, NotifyType.Success, NotifyPosition.Center, "Ваш пароль был сброшен, новый пароль должен прийти в сообщении на почту, смените его сразу же после входа через команду /password", 10000);
    //        Trigger.ClientEvent(client, "loginNotify", "Ваш пароль был сброшен, новый пароль должен прийти в сообщении на почту, смените его сразу же после входа через команду /password", "[]");
    //      });
    //    }
    //    else
    //    { // Ввод кода и проверка
    //      if (Main.RestorePass.ContainsKey(client))
    //      {
    //        if (client.GetData<string>("RealSocialClub") == Main.RestorePass[client].Item3)
    //        {
    //          if (Convert.ToInt32(loginorcode) == Main.RestorePass[client].Item1)
    //          {
    //            Log.Debug($"{client.GetData<string>("RealSocialClub")} удачно восстановил пароль!", nLog.Type.Info);
    //            int newpas = Main.rnd.Next(1000000, 9999999);
    //            await Task.Run(() => {
    //              PasswordRestore.SendEmail(1, Main.RestorePass[client].Item4, newpas); // Отправляем сообщение на емейл с новым паролем
    //            });
    //            //Notify.Send(client, NotifyType.Success, NotifyPosition.Center, "Ваш пароль был сброшен, новый пароль должен прийти в сообщении на почту, смените его сразу же после входа через команду /password", 10000);
    //            Trigger.ClientEvent(client, "loginNotify", "Ваш пароль был сброшен, новый пароль должен прийти в сообщении на почту, смените его сразу же после входа через команду /password", "[]");
    //            //MySQL.Query($"UPDATE `accounts` SET `password`='{Account.GetSha256(newpas.ToString())}' WHERE `login`='{Main.RestorePass[client].Item2}' AND `socialclub`='{Main.RestorePass[client].Item3}'");

    //            MySqlCommand cmd2 = new MySqlCommand();
    //            cmd2.CommandText = "UPDATE `accounts` SET `password`=@password WHERE `login`=@login AND `socialclub`=@socialclub";

    //            cmd2.Parameters.AddWithValue("@password", Account.GetSha256(newpas.ToString()));
    //            cmd2.Parameters.AddWithValue("@login", Main.RestorePass[client].Item2);
    //            cmd2.Parameters.AddWithValue("@socialclub", Main.RestorePass[client].Item3);
    //            MySQL.Query(cmd2);

    //            await SignInOnTimer(client, Main.RestorePass[client].Item2, newpas.ToString());  // Отправляем в логин по этим данным
    //            Main.RestorePass.Remove(client); // Удаляем из списка тех, кто восстанавливает пароль
    //          } // тут можно else { // и считать сколько раз он ввёл неправильные данные
    //        }
    //        else client.Kick(); // Если SocialClub не совпадает, то кикаем от сбоев.
    //      }
    //      else client.Kick(); // Если его не было найдено в списке, то кикаем от сбоев.
    //    }
    //  }
    //  catch (Exception ex)
    //  {
    //    Log.Write("EXCEPTION AT \"RestorePass\":\n" + ex.ToString(), nLog.Type.Error);
    //    return;
    //  }
    //}

    [RemoteEvent("signin")]
    public async void ClientEvent_signin(Player player, params object[] arguments)
    {
      try
      {
        string nickname = NAPI.Player.GetPlayerName(player);
        await Log.WriteAsync($"{nickname} try to signin step 1");
        string login = arguments[0].ToString();
        string pass = arguments[1].ToString();

        await SignInOnTimer(player, login, pass);
      }
      catch (Exception e) { Log.Write("signin: " + e.StackTrace, nLog.Type.Error); }
    }

    public async Task SignInOnTimer(Player player, string login, string pass)
    {
      try
      {
        string nickname = NAPI.Player.GetPlayerName(player);

        if (Main.Emails.ContainsKey(login))
          login = Main.Emails[login];
        else
          login = login.ToLower();

        Ban ban = Ban.Get1(player);
        if (ban != null)
        {
          if (ban.isHard && ban.CheckDate())
          {
            player.Kick($"Вы заблокированы до {ban.Until.ToString()}. Причина: {ban.Reason} ({ban.ByAdmin})");  
            NAPI.Task.Run(() => Trigger.ClientEvent(player, "kick", $"Вы заблокированы до {ban.Until.ToString()}. Причина: {ban.Reason} ({ban.ByAdmin})"));
            return;
          }
        }
        await Log.WriteAsync($"{nickname} try to signin step 2");
        Account user = new Account();
        LoginEvent result = await user.LoginIn(player, login, pass);
        if (result == LoginEvent.Authorized)
        {
          user.LoadSlots(player);
        }
        //else if (result == LoginEvent.Already)
        //{
        //  //Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Аккаунт уже авторизован.", 3000);
        //  Trigger.ClientEvent(player, "loginNotify", "Аккаунт уже авторизован.", "[]");
        //}
        else if (result == LoginEvent.Refused)
        {
          //Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Данные введены неверно", 3000);
          Trigger.ClientEvent(player, "loginNotify", "Данные введены неверно", JsonConvert.SerializeObject(new List<int>() { 0, 1 }));
        }
        if (result == LoginEvent.SclubError)
        {
          //Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "SocialClub, с которого Вы подключены, не совпадает с тем, который привязан к аккаунту.", 3000);
          Trigger.ClientEvent(player, "loginNotify", "SocialClub, с которого Вы подключены, не совпадает с тем, который привязан к аккаунту.", "[]");
        }
        await Log.WriteAsync($"{nickname} try to signin step 3");
        return;
      }
      catch (Exception e) { Log.Write("signin: " + e.StackTrace, nLog.Type.Error); }
    }

    [RemoteEvent("signup")]
    public async Task ClientEvent_signup(Player player, params object[] arguments)
    {
      try
      {
        string nickname = NAPI.Player.GetPlayerName(player);

        if (player.HasMyData("CheatTrigger"))
        {
          int cheatCode = player.GetMyData<int>("CheatTrigger");
          if (cheatCode > 1)
          {
            //Log.Write($"CheatKick: {((Cheat)cheatCode).ToString()} on {player.Name} ", nLog.Type.Warn);
            Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, "Непредвиденная ошибка! Попробуйте перезайти.", 10000);
            player.Kick();
            return;
          }
        }

        Log.Write($"{nickname} try to signup step 1");

        string login = arguments[0].ToString().ToLower();
        string pass = arguments[1].ToString();
        string email = arguments[2].ToString();


        Ban ban = Ban.Get1(player);
        if (ban != null)
        {
          if (ban.isHard && ban.CheckDate())
          {
            NAPI.Task.Run(() => Trigger.ClientEvent(player, "kick", $"Вы заблокированы до {ban.Until.ToString()}. Причина: {ban.Reason} ({ban.ByAdmin})"));
            return;
          }
        }

        Log.Write($"{nickname} try to signup step 2");
        Account user = new Account();
        RegisterEvent result = await user.Register(player, login, pass, email);
        if (result == RegisterEvent.Error)
          // Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Непредвиденная ошибка!", 3000);
          Trigger.ClientEvent(player, "loginNotify", "Непредвиденная ошибка.", "[]");
        else if (result == RegisterEvent.SocialReg)
          //Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "На этот SocialClub уже зарегистрирован игровой аккаунт!", 3000);
          Trigger.ClientEvent(player, "loginNotify", "На этот SocialClub уже зарегистрирован игровой аккаунт!", "[]");
        else if (result == RegisterEvent.UserReg)
          //Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Данное имя пользователя уже занято!", 3000);
          Trigger.ClientEvent(player, "loginNotify", "Такой логин уже существует!", "[1]");
        else if (result == RegisterEvent.EmailReg)
          //Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Данный email уже занят!", 3000);
          Trigger.ClientEvent(player, "loginNotify", "Данный email уже занят!", "[0]");
        else if (result == RegisterEvent.DataError)
          //Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Ошибка в заполнении полей!", 3000);
          Trigger.ClientEvent(player, "loginNotify", "Ошибка в заполнении полей!", "[0,1,2,3]");
        else if (result == RegisterEvent.BadEmail)
          Trigger.ClientEvent(player, "loginNotify", "Неверный формат Email", "[0]");

        Log.Write($"{nickname} try to signup step 3");
        return;
      }
      catch (Exception e) { Log.Write("signup: " + e.StackTrace, nLog.Type.Error); }
    }
  }
}
