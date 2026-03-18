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
using NeptuneEvo.Families;
using NeptuneEvo.GUI;

namespace NeptuneEvo.Core
{
  class VehiclesInformation : Script
  {
    private static nLog Log = new nLog("VehiclesInformation");

    public static Dictionary<string, Car> VehiclesModels = new Dictionary<string, Car>();

    [ServerEvent(Event.ResourceStart)]
    public static void onResourceStart()
    {
      try
      {
        var result = MySQL.QueryRead($"SELECT * FROM `vehicles_information`");
        if (result == null || result.Rows.Count == 0)
        {
          Log.Write("DB `vehicles_information` return null result.", nLog.Type.Warn);
          return;
        }

        var count = 0;

        foreach (DataRow Row in result.Rows)
        {
          var id = Convert.ToInt32(Row["id"]);
          var model = Row["model"].ToString();
          var type = Convert.ToInt32(Row["id"]);
          var name = Row["name"].ToString();
          var price = Convert.ToInt32(Row["price"]);
          var trunkWeight = Convert.ToInt32(Row["trunkWeight"]);
          var trunkFuel = Convert.ToInt32(Row["trunkFuel"]);

          count++;

          VehiclesModels.Add(model, new Car(id, model, type, name, price, trunkWeight, trunkFuel));
        }

        // Функция нужна была для первоначального переноса информации
        //updateInformation();

        Log.Write($"VehiclesInformation are loaded ({count})", nLog.Type.Success);
      }
      catch (Exception e)
      {
        Log.Write("VehiclesInformation: \n" + e.StackTrace, nLog.Type.Error);
      }
    }


    //public static void updateInformation()
    //{
    //  try
    //  {
    //    foreach(var modelName in Vehicles.Keys)
    //    {
    //      var vehicle = Vehicles[modelName];

    //      var id = vehicle.ID;
    //      var model = vehicle.Model;
    //      var type = vehicle.Type;
    //      var name = vehicle.Name;
    //      var price = vehicle.Price;
    //      var trunkWeight = vehicle.TrunkWeight;
    //      var trunkFuel = vehicle.TrunkFuel;

    //      var needUpdate = false;
          
    //      var priceSQL = "";
    //      var weightSQL = "";
    //      var fuelSQL = "";

    //      var newPrice = 0;
    //      if (BusinessManager.ProductsOrderPrice.ContainsKey(model.ToLower()))
    //        newPrice = BusinessManager.ProductsOrderPrice[model.ToLower()];

    //      priceSQL = $"`price` = {newPrice}";

    //      var newTrunkWeight = 0;
    //      if (VehicleManager.VehicleWeight.ContainsKey(model.ToLower()))
    //        newTrunkWeight = VehicleManager.VehicleWeight[model.ToLower()];

    //      weightSQL = $"`trunkWeight` = {newTrunkWeight}";

    //      var newTrunkfuel = 0;
    //      if (VehicleManager.VehicleFuel.ContainsKey(model.ToLower()))
    //        newTrunkfuel = VehicleManager.VehicleFuel[model.ToLower()];

    //      fuelSQL = $"`trunkFuel` = {newTrunkfuel}";

    //      if (needUpdate)
    //      {
    //        var sql = $"UPDATE `vehicles_information` SET {priceSQL}, {weightSQL}, {fuelSQL} WHERE `id` = {id}";

    //        MySQL.Query(sql);
    //      }
    //    }
    //  }
    //  catch (Exception e)
    //  {
    //    Log.Write("updateInformation: \n" + e.StackTrace, nLog.Type.Error);
    //  }
    //}
  }

  class Car { 
    public int ID { get; set; }
    public string Model { get; set; }
    public int Type { get; set; }
    public string Name { get; set; }
    public int Price { get; set; }
    public int TrunkWeight { get; set; }
    public int TrunkFuel{ get; set; }

    public Car(int id, string model, int type, string name, int price, int trunkWeight, int trunkFuel)
    {
      ID = id;
      Model = model;
      Type = type;
      Name = name;
      Price = price;
      TrunkWeight = trunkWeight;
      TrunkFuel = trunkFuel;
    }

    public VehicleHash getHash()
    {
      return (VehicleHash)NAPI.Util.GetHashKey(Model);
    }
  }
}
