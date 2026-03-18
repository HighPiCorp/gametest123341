using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;
using NeptuneEvo.SDK;
using Newtonsoft.Json;
using System.Data;
using NeptuneEvo.GUI;
using System.Diagnostics;
using System.Timers;
using NeptuneEvo.MoneySystem;
using static NeptuneEvo.Core.BusinessManager;

namespace NeptuneEvo.Core
{
    class Ammunation : Script
    {
        private static nLog Log = new nLog("Ammunation");

        #region Массивы
            #region Патроны
            public static Dictionary<string, int> PriceOnAmmo = new Dictionary<string, int>()
            {
                {"PistolAmmo", 5},
                {"SMGAmmo", 10},
                {"RiflesAmmo", 20},
                {"SniperAmmo", 140},
                {"ShotgunsAmmo", 10}
            };

            public static Dictionary<int, string> AmmoIndexes = new Dictionary<int, string>() {
                {0, "PistolAmmo"},
                {1, "SMGAmmo"},
                {2, "RiflesAmmo"},
                {3, "SniperAmmo"},
                {4, "ShotgunsAmmo"},
            };

            public static List<ItemType> AmmoItemTypes = new List<ItemType> () {
                ItemType.PistolAmmo,
                ItemType.SMGAmmo,
                ItemType.RiflesAmmo,
                ItemType.SniperAmmo,
                ItemType.ShotgunsAmmo,
            };
            #endregion

            #region Оружие в категориях
            /// <remarks>
            /// Если редактируешь weaponsInCategories, то и отредактируй тут ->>> <see cref="BussinessManager.GunNames" /> и <see cref="BussinessManager.ProductsCapacity" />
            /// </remarks>
            public static List<List<Dictionary<string, object>>> weaponsInCategories = new List<List<Dictionary<string, object>>>()
            {
                  // Пистолеты (4)
                  new List<Dictionary<string, object>>()
                  {
                      new Dictionary<string, object>()
                      {
                          {"itemtype", 0},
                          {"title", "Пистолет"},
                          {"product", "Pistol"},
                          {"option",
                              new List<Dictionary<string, object>>() {
                                  new Dictionary<string, object>()
                                  {
                                    {"value", nInventory.ItemsWeight[ItemType.Pistol]}
                                  },
                                  new Dictionary<string, object>()
                                  {
                                    {"value", "0 / " + Weapons.WeaponsClipsMax[ItemType.Pistol]}
                                  }
                              }
                          },
                          {"price", ProductsOrderPrice["Pistol"]},
                      },

                      new Dictionary<string, object>()
                      {
                          {"itemtype", 0},
                          {"title", "Боевой пистолет"},
                          {"product", "Combatpistol"},
                          {"option",
                              new List<Dictionary<string, object>>() {
                                  new Dictionary<string, object>()
                                  {
                                    {"value", nInventory.ItemsWeight[ItemType.Combatpistol]}
                                  },
                                  new Dictionary<string, object>()
                                  {
                                    {"value", "0 / " + Weapons.WeaponsClipsMax[ItemType.Combatpistol]}
                                  }
                              }
                          },
                          {"price", ProductsOrderPrice["Combatpistol"]},
                      },

                      new Dictionary<string, object>()
                      {
                          {"itemtype", 0},
                          {"title", "Револьвер"},
                          {"product", "Revolver"},
                          {"option",
                              new List<Dictionary<string, object>>() {
                                  new Dictionary<string, object>()
                                  {
                                    {"value", nInventory.ItemsWeight[ItemType.Revolver]}
                                  },
                                  new Dictionary<string, object>()
                                  {
                                    {"value", "0 / " + Weapons.WeaponsClipsMax[ItemType.Revolver]}
                                  }
                              }
                          },
                          {"price", ProductsOrderPrice["Revolver"]},
                      },

                      new Dictionary<string, object>()
                      {
                          {"itemtype", 0},
                          {"title", "Тяжелый пистолет"},
                          {"product", "Heavypistol"},
                          {"option",
                              new List<Dictionary<string, object>>() {
                                  new Dictionary<string, object>()
                                  {
                                    {"value", nInventory.ItemsWeight[ItemType.Heavypistol]}
                                  },
                                  new Dictionary<string, object>()
                                  {
                                    {"value", "0 / " + Weapons.WeaponsClipsMax[ItemType.Heavypistol]}
                                  }
                              }
                          },
                          {"price", ProductsOrderPrice["Heavypistol"]},
                      },
                      //"Pistol",
                      //"CombatPistol",
                      //"Revolver",
                      //"HeavyPistol",
                      //AP Pistol [MOD]
                      //Combat Pistol [MOD]
                      //Heavy Pistol [DLC] [MOD]
                      //Marksman Pistol [DLC]
                      //Pistol [MOD]
                      //Pistol .50 [CE] [MOD]
                      //SNS Pistol [DLC] [MOD]
                      //Stun Gun
                      //Vintage Pistol [DLC] [MOD]
                  },
                  // SMG (2)
                  new List<Dictionary<string, object>>()
                  {
                      new Dictionary<string, object>()
                      {
                          {"itemtype", 1},
                          {"title", "PDW"},
                          {"product", "Combatpdw"},
                          {"option",
                              new List<Dictionary<string, object>>() {
                                  new Dictionary<string, object>()
                                  {
                                    {"value", nInventory.ItemsWeight[ItemType.Combatpdw]}
                                  },
                                  new Dictionary<string, object>()
                                  {
                                    {"value", "0 / " + Weapons.WeaponsClipsMax[ItemType.Combatpdw]}
                                  }
                              }
                          },
                          {"price", ProductsOrderPrice["Combatpdw"]},
                      },

                      new Dictionary<string, object>()
                      {
                          {"itemtype", 1},
                          {"title", "Tec-9"},
                          {"product", "Machinepistol"},
                          {"option",
                              new List<Dictionary<string, object>>() {
                                  new Dictionary<string, object>()
                                  {
                                    {"value", nInventory.ItemsWeight[ItemType.Machinepistol]}
                                  },
                                  new Dictionary<string, object>()
                                  {
                                    {"value", "0 / " + Weapons.WeaponsClipsMax[ItemType.Machinepistol]}
                                  }
                              }
                          },
                          {"price", ProductsOrderPrice["Machinepistol"]},
                      },

                      //"CombatPDW",
                      //"MachinePistol",
                      //Assault SMG [R*] [MOD]
                      //Combat MG [MOD]
                      //Combat PDW [DLC] [MOD]
                      //Gusenberg Sweeper [DLC] [MOD]
                      //Machine Pistol [DLC] [EV] [MOD]
                      //MG [MOD]
                      //Micro SMG [MOD]
                      //SMG [MOD]
                  },
                  // Rifles
                  new List<Dictionary<string, object>>()
                  {
                      //new Dictionary<string, object>()
                      //{
                      //    {"itemtype", 2},
                      //    {"title", "CombatPDW"},
                      //{"product", ""},
                      //    {"option",
                      //        new List<Dictionary<string, object>>() {
                              //    new Dictionary<string, object>()
                              //    {
                              //      {"value", 1.5}
                              //    },
                              //    new Dictionary<string, object>()
                              //    {
                              //      {"value", "0 / 12"}
                              //    }
                              //}
                      //    },
                      //    {"price", 5000},
                      //},
                      //Advanced Rifle [MOD]
                      //Assault Rifle [MOD]
                      //Bullpup Rifle [DLC] [MOD]
                      //Carbine Rifle [MOD]
                      //Special Carbine [DLC] [MOD]
                  },
                  // Snipers
                  new List<Dictionary<string, object>>()
                  {
                      //new Dictionary<string, object>()
                      //{
                      //    {"itemtype", 3},
                      //    {"title", "CombatPDW"},
                      //{"product", ""},
                      //    {"option",
                      //        new List<Dictionary<string, object>>() {
                              //    new Dictionary<string, object>()
                              //    {
                              //      {"value", 1.5}
                              //    },
                              //    new Dictionary<string, object>()
                              //    {
                              //      {"value", "0 / 12"}
                              //    }
                              //}
                      //    },
                      //    {"price", 5000},
                      //},
                      //Heavy Sniper [MOD]
                      //Marksman Rifle [DLC] [MOD]
                      //Sniper Rifle [MOD]
                  },
                  // Shotguns (1)
                  new List<Dictionary<string, object>>()
                  {
                      new Dictionary<string, object>()
                      {
                          {"itemtype", 4},
                          {"title", "Дробовик-«буллпап»"},
                          {"product", "Bullpupshotgun"},
                          {"option",
                              new List<Dictionary<string, object>>() {
                                  new Dictionary<string, object>()
                                  {
                                    {"value", nInventory.ItemsWeight[ItemType.Bullpupshotgun]}
                                  },
                                  new Dictionary<string, object>()
                                  {
                                    {"value", "0 / " + Weapons.WeaponsClipsMax[ItemType.Bullpupshotgun]}
                                  }
                              }
                          },
                          {"price", ProductsOrderPrice["Bullpupshotgun"]},
                      },
                      //"BullpupShotgun",
                      //Assault Shotgun [MOD]
                      //Bullpup Shotgun [CE] [MOD]
                      //Heavy Shotgun [DLC] [MOD]
                      //Musket [DLC]
                      //Pump Shotgun [MOD]
                      //Sawed-Off Shotgun
                  },
                  // Melee
                  new List<Dictionary<string, object>>()
                  {
                      //new Dictionary<string, object>()
                      //{
                      //    {"itemtype", 5},
                      //    {"title", "BullpupShotgun"},
                      //{"product", ""},
                      //    {"option",
                      //        new List<Dictionary<string, object>>() {
                              //    new Dictionary<string, object>()
                              //    {
                              //      {"value", 1.5}
                              //    },
                              //    new Dictionary<string, object>()
                              //    {
                              //      {"value", "0 / 12"}
                              //    }
                              //}
                      //    },
                      //    {"price", 5000},
                      //},
                      //Antique Cavalry Dagger [DLC]
                      //Baseball Bat
                      //Broken Bottle [DLC]
                      //Crowbar
                      //Fist
                      //Golf Club
                      //Hammer [CE]
                      //Hatchet [RP] [EV]
                      //Knife
                      //Knuckledusters [DLC]
                      //Machete [DLC] [EV]
                      //Nightstick
                  },
                  // Ammo
                  new List<Dictionary<string, object>>()
                  {
                      new Dictionary<string, object>()
                      {
                          {"itemtype", 6},
                          {"title", "Калибр 9.19"},
                          {"product", "PistolAmmo"},
                          {"option",
                              new List<Dictionary<string, object>>() {
                                  new Dictionary<string, object>()
                                  {
                                    {"value", nInventory.ItemsWeight[ItemType.PistolAmmo]}
                                  }
                              }
                          },
                          {"price", PriceOnAmmo["PistolAmmo"]},
                      },
                      new Dictionary<string, object>()
                      {
                          {"itemtype", 6},
                          {"title", "Калибр 5.45"},
                          {"product", "SMGAmmo"},
                          {"option",
                              new List<Dictionary<string, object>>() {
                                  new Dictionary<string, object>()
                                  {
                                    {"value", nInventory.ItemsWeight[ItemType.SMGAmmo]}
                                  }
                              }
                          },
                          {"price", PriceOnAmmo["SMGAmmo"]},
                      },
                      new Dictionary<string, object>()
                      {
                          {"itemtype", 6},
                          {"title", "Калибр 7.62"},
                          {"product", "RiflesAmmo"},
                          {"option",
                              new List<Dictionary<string, object>>() {
                                  new Dictionary<string, object>()
                                  {
                                    {"value", nInventory.ItemsWeight[ItemType.RiflesAmmo]}
                                  }
                              }
                          },
                          {"price", PriceOnAmmo["RiflesAmmo"]},
                      },
                      new Dictionary<string, object>()
                      {
                          {"itemtype", 6},
                          {"title", "Калибр .50"},
                          {"product", "SniperAmmo"},
                          {"option",
                              new List<Dictionary<string, object>>() {
                                  new Dictionary<string, object>()
                                  {
                                    {"value", nInventory.ItemsWeight[ItemType.SniperAmmo]}
                                  }
                              }
                          },
                          {"price", PriceOnAmmo["SniperAmmo"]},
                      },
                      new Dictionary<string, object>()
                      {
                          {"itemtype", 6},
                          {"title", "Картечь Gauge.12"},
                          {"product", "ShotgunsAmmo"},
                          {"option",
                              new List<Dictionary<string, object>>() {
                                  new Dictionary<string, object>()
                                  {
                                    {"value", nInventory.ItemsWeight[ItemType.ShotgunsAmmo]}
                                  }
                              }
                          },
                          {"price", PriceOnAmmo["ShotgunsAmmo"]},
                      },
                  },
            };
            #endregion

            #region Модификации в категориях
            public static List<List<List<List<Dictionary<string, object>>>>> modificationsInCategories = new List<List<List<List<Dictionary<string, object>>>>>()
            {
                // Пистолеты (4)
                new List<List<List<Dictionary<string, object>>>>()
                {
                    //Pistol
                    new List<List<Dictionary<string, object>>>()
                    {
                        // Обоймы
                        new List<Dictionary<string, object>>()
                        {
                            new Dictionary<string, object>()
                            {
                                {"title", nInventory.ItemFname[ItemType.PISTOL_CLIP_02]},
                                {"product", "PISTOL_CLIP_02"},
                                {"option",
                                    new List<Dictionary<string, object>>() {
                                        new Dictionary<string, object>()
                                        {
                                          {"value", nInventory.ItemsWeight[ItemType.PISTOL_CLIP_02]}
                                        }
                                    }
                                },
                                {"price", Modifications.ModificationsPricesByName["PISTOL_CLIP_02"]},
                            }
                        },
                        // Фонарик
                        new List<Dictionary<string, object>>()
                        {
                            new Dictionary<string, object>()
                            {
                                {"title", nInventory.ItemFname[ItemType.AT_PI_FLSH]},
                                {"product", "AT_PI_FLSH"},
                                {"option",
                                    new List<Dictionary<string, object>>() {
                                        new Dictionary<string, object>()
                                        {
                                          {"value", nInventory.ItemsWeight[ItemType.AT_PI_FLSH]}
                                        }
                                    }
                                },
                                {"price", Modifications.ModificationsPricesByName["AT_PI_FLSH"]},
                            }
                        },
                        // Пламегаситель
                        new List<Dictionary<string, object>>()
                        {
                            new Dictionary<string, object>()
                            {
                                {"title", nInventory.ItemFname[ItemType.AT_PI_SUPP_02]},
                                {"product", "AT_PI_SUPP_02"},
                                {"option",
                                    new List<Dictionary<string, object>>() {
                                        new Dictionary<string, object>()
                                        {
                                          {"value", nInventory.ItemsWeight[ItemType.AT_PI_SUPP_02]}
                                        }
                                    }
                                },
                                {"price", Modifications.ModificationsPricesByName["AT_PI_SUPP_02"]},
                            }
                        },
                        // Компенсатор
                        new List<Dictionary<string, object>>()
                        {},
                        // Прицел
                        new List<Dictionary<string, object>>()
                        {},
                        // Рукоятка
                        new List<Dictionary<string, object>>()
                        {},
                        // Раскраска
                        new List<Dictionary<string, object>>()
                        {
                            new Dictionary<string, object>()
                            {
                                {"title", nInventory.ItemFname[ItemType.PISTOL_VARMOD_LUXE]},
                                {"product", "PISTOL_VARMOD_LUXE"},
                                {"option",
                                    new List<Dictionary<string, object>>() {
                                        new Dictionary<string, object>()
                                        {
                                          {"value", nInventory.ItemsWeight[ItemType.PISTOL_VARMOD_LUXE]}
                                        }
                                    }
                                },
                                {"price", Modifications.ModificationsPricesByName["PISTOL_VARMOD_LUXE"]},
                            }
                        },
                    },
                    //Combatpistol
                    new List<List<Dictionary<string, object>>>() {
                        // Обоймы
                        new List<Dictionary<string, object>>()
                        {
                            new Dictionary<string, object>()
                            {
                                {"title", nInventory.ItemFname[ItemType.COMBATPISTOL_CLIP_02]},
                                {"product", "COMBATPISTOL_CLIP_02"},
                                {"option",
                                    new List<Dictionary<string, object>>() {
                                        new Dictionary<string, object>()
                                        {
                                          {"value", nInventory.ItemsWeight[ItemType.COMBATPISTOL_CLIP_02]}
                                        }
                                    }
                                },
                                {"price", Modifications.ModificationsPricesByName["COMBATPISTOL_CLIP_02"]},
                            }
                        },
                        // Фонарик
                        new List<Dictionary<string, object>>()
                        {
                            new Dictionary<string, object>()
                            {
                                {"title", nInventory.ItemFname[ItemType.AT_PI_FLSH]},
                                {"product", "AT_PI_FLSH"},
                                {"option",
                                    new List<Dictionary<string, object>>() {
                                        new Dictionary<string, object>()
                                        {
                                          {"value", nInventory.ItemsWeight[ItemType.AT_PI_FLSH]}
                                        }
                                    }
                                },
                                {"price", Modifications.ModificationsPricesByName["AT_PI_FLSH"]},
                            }
                        },
                        // Пламегаситель
                        new List<Dictionary<string, object>>()
                        {
                            new Dictionary<string, object>()
                            {
                                {"title", nInventory.ItemFname[ItemType.AT_PI_SUPP]},
                                {"product", "AT_PI_SUPP"},
                                {"option",
                                    new List<Dictionary<string, object>>() {
                                        new Dictionary<string, object>()
                                        {
                                          {"value", nInventory.ItemsWeight[ItemType.AT_PI_SUPP]}
                                        }
                                    }
                                },
                                {"price", Modifications.ModificationsPricesByName["AT_PI_SUPP"]},
                            }
                        },
                        // Компенсатор
                        new List<Dictionary<string, object>>()
                        {},
                        // Прицел
                        new List<Dictionary<string, object>>()
                        {},
                        // Рукоятка
                        new List<Dictionary<string, object>>()
                        {},
                        // Раскраска
                        new List<Dictionary<string, object>>()
                        {
                            new Dictionary<string, object>()
                            {
                                {"title", nInventory.ItemFname[ItemType.COMBATPISTOL_VARMOD_LOWRIDER]},
                                {"product", "COMBATPISTOL_VARMOD_LOWRIDER"},
                                {"option",
                                    new List<Dictionary<string, object>>() {
                                        new Dictionary<string, object>()
                                        {
                                          {"value", nInventory.ItemsWeight[ItemType.COMBATPISTOL_VARMOD_LOWRIDER]}
                                        }
                                    }
                                },
                                {"price", Modifications.ModificationsPricesByName["COMBATPISTOL_VARMOD_LOWRIDER"]},
                            }
                        },
                    },
                    //Revolver
                    new List<List<Dictionary<string, object>>>() {
                        // Обоймы
                        new List<Dictionary<string, object>>()
                        {},
                        // Фонарик
                        new List<Dictionary<string, object>>()
                        {},
                        // Пламегаситель
                        new List<Dictionary<string, object>>()
                        {},
                        // Компенсатор
                        new List<Dictionary<string, object>>()
                        {},
                        // Прицел
                        new List<Dictionary<string, object>>()
                        {},
                        // Рукоятка
                        new List<Dictionary<string, object>>()
                        {},
                        // Раскраска
                        new List<Dictionary<string, object>>()
                        {
                            new Dictionary<string, object>()
                            {
                                {"title", nInventory.ItemFname[ItemType.REVOLVER_VARMOD_GOON]},
                                {"product", "REVOLVER_VARMOD_GOON"},
                                {"option",
                                    new List<Dictionary<string, object>>() {
                                        new Dictionary<string, object>()
                                        {
                                          {"value", nInventory.ItemsWeight[ItemType.REVOLVER_VARMOD_GOON]}
                                        }
                                    }
                                },
                                {"price", Modifications.ModificationsPricesByName["REVOLVER_VARMOD_GOON"]},
                            },
                            new Dictionary<string, object>()
                            {
                                {"title", nInventory.ItemFname[ItemType.REVOLVER_VARMOD_BOSS]},
                                {"product", "REVOLVER_VARMOD_BOSS"},
                                {"option",
                                    new List<Dictionary<string, object>>() {
                                        new Dictionary<string, object>()
                                        {
                                          {"value", nInventory.ItemsWeight[ItemType.REVOLVER_VARMOD_BOSS]}
                                        }
                                    }
                                },
                                {"price", Modifications.ModificationsPricesByName["REVOLVER_VARMOD_BOSS"]},
                            }
                        },
                    },
                    //HeavyPistol
                    new List<List<Dictionary<string, object>>>() {
                        // Обоймы
                        new List<Dictionary<string, object>>()
                        {
                            new Dictionary<string, object>()
                            {
                                {"title", nInventory.ItemFname[ItemType.HEAVYPISTOL_CLIP_02]},
                                {"product", "HEAVYPISTOL_CLIP_02"},
                                {"option",
                                    new List<Dictionary<string, object>>() {
                                        new Dictionary<string, object>()
                                        {
                                          {"value", nInventory.ItemsWeight[ItemType.HEAVYPISTOL_CLIP_02]}
                                        }
                                    }
                                },
                                {"price", Modifications.ModificationsPricesByName["HEAVYPISTOL_CLIP_02"]},
                            },
                        },
                        // Фонарик
                        new List<Dictionary<string, object>>()
                        {
                            new Dictionary<string, object>()
                            {
                                {"title", nInventory.ItemFname[ItemType.AT_PI_FLSH]},
                                {"product", "AT_PI_FLSH"},
                                {"option",
                                    new List<Dictionary<string, object>>() {
                                        new Dictionary<string, object>()
                                        {
                                          {"value", nInventory.ItemsWeight[ItemType.AT_PI_FLSH]}
                                        }
                                    }
                                },
                                {"price", Modifications.ModificationsPricesByName["AT_PI_FLSH"]},
                            },
                        },
                        // Пламегаситель
                        new List<Dictionary<string, object>>()
                        {
                            new Dictionary<string, object>()
                            {
                                {"title", nInventory.ItemFname[ItemType.AT_PI_SUPP]},
                                {"product", "AT_PI_SUPP"},
                                {"option",
                                    new List<Dictionary<string, object>>() {
                                        new Dictionary<string, object>()
                                        {
                                          {"value", nInventory.ItemsWeight[ItemType.AT_PI_SUPP]}
                                        }
                                    }
                                },
                                {"price", Modifications.ModificationsPricesByName["AT_PI_SUPP"]},
                            },
                        },
                        // Компенсатор
                        new List<Dictionary<string, object>>()
                        {},
                        // Прицел
                        new List<Dictionary<string, object>>()
                        {},
                        // Рукоятка
                        new List<Dictionary<string, object>>()
                        {},
                        // Раскраска
                        new List<Dictionary<string, object>>()
                        {
                            new Dictionary<string, object>()
                            {
                                {"title", nInventory.ItemFname[ItemType.HEAVYPISTOL_VARMOD_LUXE]},
                                {"product", "HEAVYPISTOL_VARMOD_LUXE"},
                                {"option",
                                    new List<Dictionary<string, object>>() {
                                        new Dictionary<string, object>()
                                        {
                                          {"value", nInventory.ItemsWeight[ItemType.HEAVYPISTOL_VARMOD_LUXE]}
                                        }
                                    }
                                },
                                {"price", Modifications.ModificationsPricesByName["HEAVYPISTOL_VARMOD_LUXE"]},
                            },
                        },
                    },
                },
                // SMG (2)
                new List<List<List<Dictionary<string, object>>>>()
                {
                    //Combatpdw
                    new List<List<Dictionary<string, object>>>()
                    {
                        // Обоймы
                        new List<Dictionary<string, object>>()
                        {
                            new Dictionary<string, object>()
                            {
                                {"title", nInventory.ItemFname[ItemType.COMBATPDW_CLIP_02]},
                                {"product", "COMBATPDW_CLIP_02"},
                                {"option",
                                    new List<Dictionary<string, object>>() {
                                        new Dictionary<string, object>()
                                        {
                                          {"value", nInventory.ItemsWeight[ItemType.COMBATPDW_CLIP_02]}
                                        }
                                    }
                                },
                                {"price", Modifications.ModificationsPricesByName["COMBATPDW_CLIP_02"]},
                            },
                            new Dictionary<string, object>()
                            {
                                {"title", nInventory.ItemFname[ItemType.COMBATPDW_CLIP_03]},
                                {"product", "COMBATPDW_CLIP_03"},
                                {"option",
                                    new List<Dictionary<string, object>>() {
                                        new Dictionary<string, object>()
                                        {
                                          {"value", nInventory.ItemsWeight[ItemType.COMBATPDW_CLIP_03]}
                                        }
                                    }
                                },
                                {"price", Modifications.ModificationsPricesByName["COMBATPDW_CLIP_03"]},
                            },
                        },
                        // Фонарик
                        new List<Dictionary<string, object>>()
                        {
                            new Dictionary<string, object>()
                            {
                                {"title", nInventory.ItemFname[ItemType.AT_AR_FLSH]},
                                {"product", "AT_AR_FLSH"},
                                {"option",
                                    new List<Dictionary<string, object>>() {
                                        new Dictionary<string, object>()
                                        {
                                          {"value", nInventory.ItemsWeight[ItemType.AT_AR_FLSH]}
                                        }
                                    }
                                },
                                {"price", Modifications.ModificationsPricesByName["AT_AR_FLSH"]},
                            },
                        },
                        // Пламегаситель
                        new List<Dictionary<string, object>>()
                        {},
                        // Компенсатор
                        new List<Dictionary<string, object>>()
                        {},
                        // Прицел
                        new List<Dictionary<string, object>>()
                        {
                            new Dictionary<string, object>()
                            {
                                {"title", nInventory.ItemFname[ItemType.AT_SCOPE_SMALL]},
                                {"product", "AT_SCOPE_SMALL"},
                                {"option",
                                    new List<Dictionary<string, object>>() {
                                        new Dictionary<string, object>()
                                        {
                                          {"value", nInventory.ItemsWeight[ItemType.AT_SCOPE_SMALL]}
                                        }
                                    }
                                },
                                {"price", Modifications.ModificationsPricesByName["AT_SCOPE_SMALL"]},
                            },
                        },
                        // Рукоятка
                        new List<Dictionary<string, object>>()
                        {
                            new Dictionary<string, object>()
                            {
                                {"title", nInventory.ItemFname[ItemType.AT_AR_AFGRIP]},
                                {"product", "AT_AR_AFGRIP"},
                                {"option",
                                    new List<Dictionary<string, object>>() {
                                        new Dictionary<string, object>()
                                        {
                                          {"value", nInventory.ItemsWeight[ItemType.AT_AR_AFGRIP]}
                                        }
                                    }
                                },
                                {"price", Modifications.ModificationsPricesByName["AT_AR_AFGRIP"]},
                            },
                        },
                        // Раскраска
                        new List<Dictionary<string, object>>()
                        {},
                    },
                    //Machinepistol
                    new List<List<Dictionary<string, object>>>()
                    {
                        // Обоймы
                        new List<Dictionary<string, object>>()
                        {
                            new Dictionary<string, object>()
                            {
                                {"title", nInventory.ItemFname[ItemType.MACHINEPISTOL_CLIP_02]},
                                {"product", "MACHINEPISTOL_CLIP_02"},
                                {"option",
                                    new List<Dictionary<string, object>>() {
                                        new Dictionary<string, object>()
                                        {
                                          {"value", nInventory.ItemsWeight[ItemType.MACHINEPISTOL_CLIP_02]}
                                        }
                                    }
                                },
                                {"price", Modifications.ModificationsPricesByName["MACHINEPISTOL_CLIP_02"]},
                            },
                            new Dictionary<string, object>()
                            {
                                {"title", nInventory.ItemFname[ItemType.MACHINEPISTOL_CLIP_03]},
                                {"product", "MACHINEPISTOL_CLIP_03"},
                                {"option",
                                    new List<Dictionary<string, object>>() {
                                        new Dictionary<string, object>()
                                        {
                                          {"value", nInventory.ItemsWeight[ItemType.MACHINEPISTOL_CLIP_03]}
                                        }
                                    }
                                },
                                {"price", Modifications.ModificationsPricesByName["MACHINEPISTOL_CLIP_03"]},
                            },
                        },
                        // Фонарик
                        new List<Dictionary<string, object>>()
                        {},
                        // Пламегаситель
                        new List<Dictionary<string, object>>()
                        {
                            new Dictionary<string, object>()
                            {
                                {"title", nInventory.ItemFname[ItemType.AT_PI_SUPP]},
                                {"product", "AT_PI_SUPP"},
                                {"option",
                                    new List<Dictionary<string, object>>() {
                                        new Dictionary<string, object>()
                                        {
                                          {"value", nInventory.ItemsWeight[ItemType.AT_PI_SUPP]}
                                        }
                                    }
                                },
                                {"price", Modifications.ModificationsPricesByName["AT_PI_SUPP"]},
                            },
                        },
                        // Компенсатор
                        new List<Dictionary<string, object>>()
                        {},
                        // Прицел
                        new List<Dictionary<string, object>>()
                        {},
                        // Рукоятка
                        new List<Dictionary<string, object>>()
                        {},
                        // Раскраска
                        new List<Dictionary<string, object>>()
                        {},
                    },
                },
                // Rifles
                new List<List<List<Dictionary<string, object>>>>()
                {},
                // Snipers
                new List<List<List<Dictionary<string, object>>>>()
                {},
                // Shotguns (1)
                new List<List<List<Dictionary<string, object>>>>()
                {
                    //Bullpupshotgun
                    new List<List<Dictionary<string, object>>>()
                    {
                        // Обоймы
                        new List<Dictionary<string, object>>()
                        {},
                        // Фонарик
                        new List<Dictionary<string, object>>()
                        {
                            new Dictionary<string, object>()
                            {
                                {"title", nInventory.ItemFname[ItemType.AT_AR_FLSH]},
                                {"product", "AT_AR_FLSH"},
                                {"option",
                                    new List<Dictionary<string, object>>() {
                                        new Dictionary<string, object>()
                                        {
                                          {"value", nInventory.ItemsWeight[ItemType.AT_AR_FLSH]}
                                        }
                                    }
                                },
                                {"price", Modifications.ModificationsPricesByName["AT_AR_FLSH"]},
                            }
                        },
                        // Пламегаситель
                        new List<Dictionary<string, object>>()
                        {
                            new Dictionary<string, object>()
                            {
                                {"title", nInventory.ItemFname[ItemType.AT_AR_SUPP_02]},
                                {"product", "AT_AR_SUPP_02"},
                                {"option",
                                    new List<Dictionary<string, object>>() {
                                        new Dictionary<string, object>()
                                        {
                                          {"value", nInventory.ItemsWeight[ItemType.AT_AR_SUPP_02]}
                                        }
                                    }
                                },
                                {"price", Modifications.ModificationsPricesByName["AT_AR_SUPP_02"]},
                            }
                        },
                        // Компенсатор
                        new List<Dictionary<string, object>>()
                        {},
                        // Прицел
                        new List<Dictionary<string, object>>()
                        {},
                        // Рукоятка
                        new List<Dictionary<string, object>>()
                        {
                            new Dictionary<string, object>()
                            {
                                {"title", nInventory.ItemFname[ItemType.AT_AR_AFGRIP]},
                                {"product", "AT_AR_AFGRIP"},
                                {"option",
                                    new List<Dictionary<string, object>>() {
                                        new Dictionary<string, object>()
                                        {
                                          {"value", nInventory.ItemsWeight[ItemType.AT_AR_AFGRIP]}
                                        }
                                    }
                                },
                                {"price", Modifications.ModificationsPricesByName["AT_AR_AFGRIP"]},
                            }
                        },
                        // Раскраска
                        new List<Dictionary<string, object>>()
                        {},
                    },
                },
                // Melee
                new List<List<List<Dictionary<string, object>>>>()
                {},
                // Ammo
                new List<List<List<Dictionary<string, object>>>>()
                {},
            };
            #endregion

            #region

            #endregion
        #endregion

        public static void OpenGunShopMenu(Player player)
        {
            try
            {
              List<List<int>> prices = new List<List<int>>();

              Business biz = BizList[player.GetMyData<int>("GUNSHOP")];

              var items = new List<Dictionary<string, object>>(weaponsInCategories[0]);
              var guns = new List<List<Dictionary<string, object>>>(weaponsInCategories);
              foreach(var item in items)
              {
                string prodName = (string)item["product"];
                var prod = ProductsOrderPrice[prodName];
                item["price"] = biz.GetPriceWithMarkUpInt(prod);
              }
              foreach (var item in guns)
              {
                foreach(var item2 in item)
                {
                  string prodName = (string)item2["product"];
                  var prod = ProductsOrderPrice[prodName];
                  item2["price"] = biz.GetPriceWithMarkUpInt(prod);
                }
              }
              Dictionary<string, object> dict = new Dictionary<string, object>();
          
              dict.Add("gun", guns);
              dict.Add("items", items);

              Log.Debug("CLIENT::gunshop:open: " + JsonConvert.SerializeObject(dict));

              Trigger.ClientEvent(player, "CLIENT::gunshop:open", biz.ID, JsonConvert.SerializeObject(dict));
            }
            catch (Exception e) { Log.Write("OpenGunShopMenu: " + e.StackTrace, nLog.Type.Error); }
        }

        [RemoteEvent("SERVER::gunshop:getGunsItems")]
        public static void RemoteEvent_getItemsByCategoryId(Player player, int itemtype) {
            try
            {
                Business biz = BizList[player.GetMyData<int>("GUNSHOP")];
                var items = new List<Dictionary<string, object>>(weaponsInCategories[itemtype]);
                foreach (var item in items)
                {
                  string prodName = (string)item["product"];
                  var prod = ProductsOrderPrice[prodName];
                  item["price"] = biz.GetPriceWithMarkUpInt(prod);
                }
                Dictionary<string, object> dict = new Dictionary<string, object>();

                //dict.Add("gun", weaponsInCategories);
                dict.Add("items", items);

                Trigger.ClientEvent(player, "CLIENT::gunshop:setGunsItems", biz.ID, JsonConvert.SerializeObject(dict));
            }
            catch (Exception e) { Log.Write("SERVER::gunshop:getGunsItems: " + e.StackTrace, nLog.Type.Error); }
        }

        [RemoteEvent("SERVER::gunshop:getModificationsItems")]
        public static void RemoteEvent_getModificationList(Player player, int itemtype, int itemIndex, int modificationIndex)
        {
            try
            {
              Business biz = BizList[player.GetMyData<int>("GUNSHOP")];
              var items = modificationsInCategories[itemtype][itemIndex][modificationIndex];
              //Log.Debug(JsonConvert.SerializeObject(items));

              Dictionary<string, object> dict = new Dictionary<string, object>();

              dict.Add("items", items);

              Trigger.ClientEvent(player, "CLIENT::gunshop:setModificationsItems", biz.ID, JsonConvert.SerializeObject(dict));
            }
            catch (Exception e) { Log.Write("SERVER::gunshop:getModificationsItems: " + e.StackTrace, nLog.Type.Error); }
        }

        [RemoteEvent("SERVER::gunshop:buy")]
        public static void RemoteEvent_buy(Player player, int buyType, int categoryIndex, int weaponIndex, int count)
        {
            try
            {
                var bizid = player.GetMyData<int>("GUNSHOP");

                if (!Main.Players[player].Licenses[6])
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У Вас нет лицензии на оружие", 3000);
                    return;
                }

                var biz = BizList[bizid];
                var prodName = (string)weaponsInCategories[categoryIndex][weaponIndex]["product"];

                var prod = biz.Products.FirstOrDefault(p => p.Name == prodName);

                int summaryPrice = biz.GetPriceWithMarkUpInt(prod.Price) * count;

                ItemType wType = (ItemType)Enum.Parse(typeof(ItemType), prod.Name);

                var tryAdd = nInventory.TryAdd(player, new nItem(wType, count));
                if (tryAdd == -1)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Недостаточно места в инвентаре", 3000);
                    return;
                }

                switch (buyType) {
                    case 0:
                        if (Main.Players[player].Money < summaryPrice)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Недостаточно средств", 3000);
                            return;
                        }

                        if (!takeProd(bizid, count, prod.Name, summaryPrice))
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Недостаточно товара на складе", 3000);
                            return;
                        }

                        Wallet.Change(player, -summaryPrice);
                        GameLog.Money($"player({Main.Players[player].UUID})", $"biz({biz.ID})", summaryPrice, "buyGuns by Cash");

                        break;
                    case 1:
                        if (MoneySystem.Bank.Accounts[Main.Players[player].Bank].Balance < summaryPrice)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Недостаточно средств", 3000);
                            return;
                        }

                        if (!takeProd(bizid, count, prod.Name, summaryPrice))
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Недостаточно товара на складе", 3000);
                            return;
                        }

                        Bank.Change(Main.Players[player].Bank, -summaryPrice);
                        GameLog.Money($"player({Main.Players[player].UUID})", $"biz({biz.ID})", summaryPrice, "buyGuns by Card");

                        break;
                }

                for(int i = 1; i <= count; i++) {
                    Weapons.GiveWeapon(player, wType, Weapons.GetSerial(false, biz.ID));
                }


                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы купили {prod.Name} x{count} за {summaryPrice}$", 3000);
                return;
            }
            catch (Exception e) { Log.Write("SERVER::gunshop:buy: " + e.StackTrace, nLog.Type.Error); }
        }

        [RemoteEvent("SERVER::gunshop:buyModification")]
        public static void RemoteEvent_buyModification(Player player, int buyType, int categoryIndex, int weaponIndex, int modIndex, int itemIndex, int count)
        {
            try
            {
                var bizid = player.GetMyData<int>("GUNSHOP");

                if (!Main.Players[player].Licenses[6])
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У Вас нет лицензии на оружие", 3000);
                    return;
                }

                var biz = BizList[bizid];
                var prodName = (string)modificationsInCategories[categoryIndex][weaponIndex][modIndex][itemIndex]["product"];

                var prod = biz.Products.FirstOrDefault(p => p.Name == "Модификации");

                int summaryPrice = biz.GetPriceWithMarkUpInt(Modifications.ModificationsPricesByName[prodName]) * count;
                var totalcountprod = Convert.ToInt32(Modifications.ModificationsPricesByName[prodName] / 10.0 * count);
                if (totalcountprod <= 1)
                  totalcountprod = 1;
                ItemType wType = (ItemType)Enum.Parse(typeof(ItemType), prodName);

                var tryAdd = nInventory.TryAdd(player, new nItem(wType, count));
                if (tryAdd == -1)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Недостаточно места в инвентаре", 3000);
                    return;
                }

                switch (buyType) {
                    case 0:
                        if (Main.Players[player].Money < summaryPrice)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Недостаточно средств", 3000);
                            return;
                        }

                        if (!takeProd(bizid, totalcountprod, prod.Name, summaryPrice))
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Недостаточно товара на складе", 3000);
                            return;
                        }

                        Wallet.Change(player, -summaryPrice);
                        GameLog.Money($"player({Main.Players[player].UUID})", $"biz({biz.ID})", summaryPrice, "buyGuns by Cash");

                        break;
                    case 1:
                        if (MoneySystem.Bank.Accounts[Main.Players[player].Bank].Balance < summaryPrice)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Недостаточно средств", 3000);
                            return;
                        }

                        if (!takeProd(bizid, totalcountprod, prod.Name, summaryPrice))
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Недостаточно товара на складе", 3000);
                            return;
                        }

                        Bank.Change(Main.Players[player].Bank, -summaryPrice);
                        GameLog.Money($"player({Main.Players[player].UUID})", $"biz({biz.ID})", summaryPrice, "buyGuns by Card");

                        break;
                }

                for(int i = 1; i <= count; i++) {
                    Weapons.GiveWeapon(player, wType, Weapons.GetSerial(false, biz.ID));
                }


                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы купили {prod.Name} x{count} за {summaryPrice}$", 3000);
                return;
            }
            catch (Exception e) { Log.Write("SERVER::gunshop:buy: " + e.StackTrace, nLog.Type.Error); }
        }

        [RemoteEvent("SERVER::gunshop:buyAmmo")]
        public static void RemoteEvent_buyAmmo(Player player, int buyType, int ammoIndex, int count)
        {
            try
            {
                var bizid = player.GetMyData<int>("GUNSHOP");

                if (!Main.Players[player].Licenses[6])
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У Вас нет лицензии на оружие", 3000);
                    return;
                }

                var biz = BizList[bizid];
                var prod = biz.Products.FirstOrDefault(p => p.Name == "Патроны");

                int Price = PriceOnAmmo[AmmoIndexes[ammoIndex]] * count;//7 * 1 = 7 без наценки
                int summaryPrice = biz.GetPriceWithMarkUpInt(PriceOnAmmo[AmmoIndexes[ammoIndex]]) * count;// 7 * 1 = 7||10 с наценкой 100%/150%
                var totalcountprod = Convert.ToInt32(Price / prod.Price);//7 / 7 = 1 кол-во матов 
                if (totalcountprod <= 1)
                  totalcountprod = 1;

                var tryAdd = nInventory.TryAdd(player, new nItem(AmmoItemTypes[ammoIndex], count));
                if (tryAdd == -1)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Недостаточно места в инвентаре", 3000);
                    return;
                }

                switch (buyType) {
                    case 0:
                        if (Main.Players[player].Money < summaryPrice)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Недостаточно средств", 3000);
                            return;
                        }

                        if (!takeProd(bizid, totalcountprod, prod.Name, summaryPrice))
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Недостаточно товара на складе", 3000);
                            return;
                        }

                        Wallet.Change(player, -summaryPrice);
                        GameLog.Money($"player({Main.Players[player].UUID})", $"biz({biz.ID})", summaryPrice, "buyAmmoGuns by Cash");

                        break;
                    case 1:
                        if (MoneySystem.Bank.Accounts[Main.Players[player].Bank].Balance < summaryPrice)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Недостаточно средств", 3000);
                            return;
                        }

                        if (!takeProd(bizid, totalcountprod, prod.Name, summaryPrice))
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Недостаточно товара на складе", 3000);
                            return;
                        }

                        Bank.Change(Main.Players[player].Bank, -summaryPrice);
                        GameLog.Money($"player({Main.Players[player].UUID})", $"biz({biz.ID})", summaryPrice, "buyAmmoGuns by Card");

                        break;
                }

                nInventory.Add(player, new nItem(AmmoItemTypes[ammoIndex], count));
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы купили {prod.Name} x{count} за {summaryPrice}$", 3000);
                return;
            }
            catch (Exception e) { Log.Write("SERVER::gunshop:buyAmmo: " + e.StackTrace, nLog.Type.Error); }
        }
    }
}
