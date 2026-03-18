using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;
using System.IO;

namespace NeptuneEvo.Core
{
  class ClientLog : Script
  {
    private static string Folder = "ClientLogs";

    [RemoteEvent("WriteClientLog")]
    public static void WriteClientLog(Player player, string fileName, string log)
    {
      if (!Directory.Exists(Folder))
        Directory.CreateDirectory(Folder);
      string path = $"{Folder}/{fileName}";
      if (!File.Exists(path))
        File.Create(path).Close();
      TextWriter tw = new StreamWriter(path, true);
      tw.WriteLine(log);
      tw.Close();
    }
  }
}
