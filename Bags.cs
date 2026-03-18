using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Text;
using NeptuneEvo.Core;
using NeptuneEvo.SDK;
using NeptuneEvo.MoneySystem;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using NeptuneEvo.Houses;
using NeptuneEvo.Families;
using System.Linq;
using client.Jobs.Builder;
using System.ComponentModel;

namespace NeptuneEvo.Core
{
  public class nBags : Script
  {
    private static nLog Log = new nLog("nBags");
    public static List<Bag> BagsData = new List<Bag>()
    {
        new Bag("Сумка/Рюкзак", 0, 10, 10, 0), // Default Bag
        new Bag("Сумка/Рюкзак", 41, 10, 10, 1000),
        new Bag("Сумка/Рюкзак", 45, 20, 20, 2000),
        new Bag("Сумка/Рюкзак", 82, 30, 30, 3000),
        new Bag("Сумка/Рюкзак", 86, 40, 40, 4000),

        // DLC
        new Bag("Сумка/Рюкзак", 111, 20, 10, 984625), // 1
        new Bag("Сумка/Рюкзак", 112, 30, 20, 1487650), // 2
        new Bag("Сумка/Рюкзак", 113, 15, 15, 211860), // 4
        new Bag("Сумка/Рюкзак", 114, 15, 10, 473250), // 1
        new Bag("Сумка/Рюкзак", 118, 12, 10, 186410), // 4
        new Bag("Сумка/Рюкзак", 121, 10, 5, 521470), // 5
        new Bag("Сумка/Рюкзак", 123, 5, 5, 115485), // 2
        new Bag("Сумка/Рюкзак", 124, 25, 25, 743940), // 9
        //new Bag("Спортивная сумка", 125, 40, 40, 19000), // ?
    };
  }


  public class Bag
  {
    public string Name { get; set; }
    public int Variation { get; set; }
    public int maxWeight { get; set; }
    public int maxSlots { get; set; }
    public int Price { get; set; }

    public Bag(string name, int variation, int maxweight, int maxslots, int price)
    {
      Name = name;
      Variation = variation;
      maxWeight = maxweight;
      maxSlots = maxslots;
      Price = price;
    }
  }
}
