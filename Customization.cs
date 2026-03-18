using GTANetworkAPI;
using Newtonsoft.Json;
using NeptuneEvo.SDK;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using client.Core;
using MySqlConnector;

namespace NeptuneEvo.Core
{
    #region Tattoo
    public enum TattooZones
    {
        Torso = 0,
        Head = 1,
        LeftArm = 2,
        RightArm = 3,
        LeftLeg = 4,
        RightLeg = 5,
    }

    public class Tattoo
    {
        public string Dictionary { get; set; }
        public string Hash { get; set; }
        public List<int> Slots { get; set; }

        public Tattoo(string dictionary, string hash, List<int> slots)
        {
            Dictionary = dictionary;
            Hash = hash;
            Slots = slots;
        }
    }
    #endregion

    #region ComponentItem

    public class ComponentItem
    {
        public int Variation;
        public int Texture;

        public ComponentItem(int variation, int texture)
        {
            Variation = variation;
            Texture = texture;
        }
    }

    #endregion

    #region ClothesData
    public class ClothesData
    {
        public ComponentItem Mask { get; set; }
        public ComponentItem Gloves { get; set; }
        public ComponentItem Torso { get; set; }
        public ComponentItem Leg { get; set; }
        public ComponentItem Bag { get; set; }
        public ComponentItem Feet { get; set; }
        public ComponentItem Accessory { get; set; }
        public ComponentItem Undershit { get; set; }
        public ComponentItem Bodyarmor { get; set; }
        public ComponentItem Decals { get; set; }
        public ComponentItem Top { get; set; }

        public ClothesData()
        {
            Mask = new ComponentItem(0, 0);
            Gloves = new ComponentItem(0, 0);
            Torso = new ComponentItem(15, 0);
            Leg = new ComponentItem(21, 0);
            Bag = new ComponentItem(0, 0);
            Feet = new ComponentItem(34, 0);
            Accessory = new ComponentItem(0, 0);
            Undershit = new ComponentItem(15, 0);
            Bodyarmor = new ComponentItem(0, 0);
            Decals = new ComponentItem(0, 0);
            Top = new ComponentItem(15, 0);
        }
    }
    #endregion

    #region AccessoryData
    public class AccessoryData
    {
        public ComponentItem Hat { get; set; }
        public ComponentItem Glasses { get; set; }
        public ComponentItem Ear { get; set; }
        public ComponentItem Watches { get; set; }
        public ComponentItem Bracelets { get; set; }

        public AccessoryData()
        {
            Hat = new ComponentItem(-1, 0);
            Glasses = new ComponentItem(-1, 0);
            Ear = new ComponentItem(-1, 0);
            Watches = new ComponentItem(-1, 0);
            Bracelets = new ComponentItem(-1, 0);
        }
    }
    #endregion

    #region ParentData
    public class ParentData
    {
        public int Father;
        public int Mother;
        public float Similarity;
        public float SkinSimilarity;

        public ParentData(int father, int mother, float similarity, float skinsimilarity)
        {
            Father = father;
            Mother = mother;
            Similarity = similarity;
            SkinSimilarity = skinsimilarity;
        }
    }
    #endregion

    #region AppearanceItem
    public class AppearanceItem
    {
        public int Value;
        public float Opacity;

        public AppearanceItem(int value, float opacity)
        {
            Value = value;
            Opacity = opacity;
        }
    }
    #endregion

    #region HairData
    public class HairData
    {
        public int Hair;
        public int Color;
        public int HighlightColor;

        public HairData(int hair, int color, int highlightcolor)
        {
            Hair = hair;
            Color = color;
            HighlightColor = highlightcolor;
        }
    }
    #endregion

    #region PlayerCustomization Class
    public class PlayerCustomization
    {
        // Player
        public int Gender;

        // Parents
        public ParentData Parents;

        // Features
        public float[] Features = new float[20];

        // Appearance
        public AppearanceItem[] Appearance = new AppearanceItem[10];

        // Hair & Colors
        public HairData Hair;

        public ClothesData Clothes = new ClothesData();

        public AccessoryData Accessory = new AccessoryData();

        public Dictionary<int, List<Tattoo>> Tattoos = new Dictionary<int, List<Tattoo>>()
        {
            { 0, new List<Tattoo>() },
            { 1, new List<Tattoo>() },
            { 2, new List<Tattoo>() },
            { 3, new List<Tattoo>() },
            { 4, new List<Tattoo>() },
            { 5, new List<Tattoo>() },
        };


        public int EyebrowColor;
        public int BeardColor;
        public int EyeColor;
        public int BlushColor;
        public int LipstickColor;
        public int ChestHairColor;

        public bool IsCreated = false;

        public PlayerCustomization()
        {
            Gender = 0;
            Parents = new ParentData(0, 0, 1.0f, 1.0f);
            for (int i = 0; i < Features.Length; i++) Features[i] = 0f;
            for (int i = 0; i < Appearance.Length; i++) Appearance[i] = new AppearanceItem(255, 1.0f);
            Hair = new HairData(0, 0, 0);
        }
    }
    #endregion

    #region Underwear Class
    class Underwear
    {
        public Underwear(int top, int price, List<int> colors)
        {
            Top = top;
            Price = price;
            Colors = colors;
        }
        public Underwear(int top, int price, Dictionary<int, int> undershirtIDs, List<int> colors)
        {
            Top = top;
            UndershirtIDs = undershirtIDs;
            Price = price;
            Colors = colors;
        }

        public int Top { get; }
        public int Price { get; }
        public Dictionary<int, int> UndershirtIDs { get; } = new Dictionary<int, int>(); // key - тип undershirt'а, value - id-шник
        public List<int> Colors { get; }
    }
    #endregion

    #region Clothes Class
    class Clothes
    {
        public Clothes(int variation, List<int> colors, int price, int type = -1, int bodyArmor = -1)
        {
            Variation = variation;
            Colors = colors;
            Price = price;
            Type = type;
            BodyArmor = bodyArmor;
        }

        public int Variation { get; }
        public List<int> Colors { get; }
        public int Price { get; }
        public int Type { get; }
        public int BodyArmor { get; }
    }
    #endregion

    class Customization : Script
    {
        public Customization()
        {
            var result = MySQL.QueryRead($"SELECT * FROM customization");
            if (result == null || result.Rows.Count == 0)
            {
                Log.Write("DB return null result.", nLog.Type.Warn);
                return;
            }
            foreach (DataRow Row in result.Rows)
            {
                var uuid = Convert.ToInt32(Row["uuid"]);
                CustomPlayerData.Add(uuid, new PlayerCustomization());

                CustomPlayerData[uuid].Gender = Convert.ToInt32(Row["gender"]);
                CustomPlayerData[uuid].Parents = JsonConvert.DeserializeObject<ParentData>(Row["parents"].ToString());
                CustomPlayerData[uuid].Features = JsonConvert.DeserializeObject<float[]>(Row["features"].ToString());
                CustomPlayerData[uuid].Appearance = JsonConvert.DeserializeObject<AppearanceItem[]>(Row["appearance"].ToString());
                CustomPlayerData[uuid].Hair = JsonConvert.DeserializeObject<HairData>(Row["hair"].ToString());
                CustomPlayerData[uuid].Clothes = JsonConvert.DeserializeObject<ClothesData>(Row["clothes"].ToString());
                CustomPlayerData[uuid].Accessory = JsonConvert.DeserializeObject<AccessoryData>(Row["accessory"].ToString());
                CustomPlayerData[uuid].Tattoos = JsonConvert.DeserializeObject<Dictionary<int, List<Tattoo>>>(Row["tattoos"].ToString());
                CustomPlayerData[uuid].EyebrowColor = Convert.ToInt32(Row["eyebrowc"]);
                CustomPlayerData[uuid].BeardColor = Convert.ToInt32(Row["beardc"]);
                CustomPlayerData[uuid].EyeColor = Convert.ToInt32(Row["eyec"]);
                CustomPlayerData[uuid].BlushColor = Convert.ToInt32(Row["blushc"]);
                CustomPlayerData[uuid].LipstickColor = Convert.ToInt32(Row["lipstickc"]);
                CustomPlayerData[uuid].ChestHairColor = Convert.ToInt32(Row["chesthairc"]);
                CustomPlayerData[uuid].IsCreated = Convert.ToBoolean(Row["iscreated"]);

                //CustomPlayerData[uuid].Clothes.Bag.Variation = 0;

                //CustomPlayerData.Add(Row["name"].ToString(), JsonConvert.DeserializeObject<PlayerCustomization>(Row["data"].ToString()));
            }
        }

        [ServerEvent(Event.ResourceStart)]
        public void onResourceStart()
        {
            try
            {
                var col = NAPI.ColShape.CreateCylinderColShape(new Vector3(403.1231, -1000.107, -100.1241), 1, 2, NAPI.GlobalDimension);
                col.OnEntityEnterColShape += (s, e) =>
                {
                    Commands.SendToAdmins(3, $"!{{#d35400}}[CHAR-CREATOR-EXPLOIT] {e.Name} ({e.Value})"); // Будет Exploit, если игрок сам спрыгнул  в fix-creator

                };
                NAPI.Marker.CreateMarker(1, new Vector3(403.1231, -1000.107, -100.1241), new Vector3(), new Vector3(), 1, new Color(255, 255, 255), false, NAPI.GlobalDimension);
                NAPI.TextLabel.CreateTextLabel(Main.StringToU16("Fix creator"), new Vector3(403.1231, -1000.107, -99.1241), 20F, 0.3F, 0, new Color(0, 180, 0));
            }
            catch (Exception e) { Log.Write("ResourceStart: " + e.StackTrace, nLog.Type.Error); }
        }

        private static nLog Log = new nLog("Character");

        public static Dictionary<int, PlayerCustomization> CustomPlayerData = new Dictionary<int, PlayerCustomization>();

        public static Dictionary<bool, Dictionary<int, int>> CorrectTorso = new Dictionary<bool, Dictionary<int, int>>()
        {
            {
                true, new Dictionary<int, int>()
                {
                    {0, 0},
                    {1, 0},
                    {2, 2},
                    {3, 14},
                    {4, 14},
                    {5, 5},
                    {6, 14},
                    {7, 14},
                    {8, 8},
                    {9, 0},
                    {10, 14},
                    {11, 15},
                    {12, 12},
                    {13, 11},
                    {14, 12},
                    {15, 15},
                    {16, 0},
                    {17, 5},
                    {18, 0},
                    {19, 14},
                    {20, 14},
                    {21, 15},
                    {22, 0},
                    {23, 14},
                    {24, 14},
                    {25, 15},
                    {26, 11},
                    {27, 14},
                    {28, 14},
                    {29, 14},
                    {30, 14},
                    {31, 14},
                    {32, 14},
                    {33, 0},
                    {34, 0},
                    {35, 14},
                    {36, 5},
                    {37, 14},
                    {38, 8},
                    {39, 0},
                    {40, 15},
                    {41, 12},
                    {42, 11},
                    {43, 11},
                    {44, 0},
                    {45, 15},
                    {46, 14},
                    {47, 0},
                    {48, 1},
                    {49, 1},
                    {50, 1},
                    {51, 1},
                    {52, 2},
                    {53, 0},
                    {54, 1},
                    {55, 0},
                    {56, 0},
                    {57, 0},
                    {58, 14},
                    {59, 14},
                    {60, 15},
                    {61, 1},
                    {62, 14},
                    {63, 5},
                    {64, 14},
                    {65, 14},
                    {66, 15},
                    {67, 1},
                    {68, 14},
                    {69, 14},
                    {70, 14},
                    {71, 0},
                    {72, 14},
                    {73, 0},
                    {74, 14},
                    {75, 11},
                    {76, 14},
                    {77, 14},
                    {78, 14},
                    {79, 14},
                    {80, 0},
                    {81, 0},
                    {82, 0},
                    {83, 0},
                    {84, 1},
                    {85, 1},
                    {86, 1},
                    {87, 1},
                    {88, 14},
                    {89, 14},
                    {90, 14},
                    {91, 15},
                    {92, 6},
                    {93, 0},
                    {94, 0},
                    {95, 11},
                    {96, 11},
                    {97, 0},
                    {98, 0},
                    {99, 14},
                    {100, 14},
                    {101, 14},
                    {102, 14},
                    {103, 14},
                    {104, 14},
                    {105, 11},
                    {106, 14},
                    {107, 14},
                    {108, 14},
                    {109, 5},
                    {110, 1},
                    {111, 4},
                    {112, 14},
                    {113, 6},
                    {114, 14},
                    {115, 14},
                    {116, 14},
                    {117, 6},
                    {118, 14},
                    {119, 14},
                    {120, 6},
                    {121, 14},
                    {122, 14},
                    {123, 11},
                    {124, 14},
                    {125, 14},
                    {126, 4},
                    {127, 14},
                    {128, 0},
                    {129, 0},
                    {130, 14},
                    {131, 0},
                    {132, 0},
                    {133, 11},
                    {134, 0},
                    {135, 0},
                    {136, 14},
                    {137, 6},
                    {138, 14},
                    {139, 12},
                    {140, 14},
                    {141, 6},
                    {142, 14},
                    {143, 14},
                    {144, 6},
                    {145, 14},
                    {146, 0},
                    {147, 4},
                    {148, 4},
                    {149, 14},
                    {150, 14},
                    {151, 14},
                    {152, 14},
                    {153, 14},
                    {154, 14},
                    {155, 14},
                    {156, 14},
                    {157, 15},
                    {158, 15},
                    {159, 15},
                    {160, 15},
                    {161, 14},
                    {162, 15},
                    {163, 14},
                    {164, 0},
                    {165, 0},
                    {166, 14},
                    {167, 14},
                    {168, 14},
                    {169, 14},
                    {170, 15},
                    {171, 1},
                    {172, 14},
                    {173, 15},
                    {174, 14},
                    {175, 15},
                    {176, 15},
                    {177, 15},
                    {178, 1},
                    {179, 15},
                    {180, 15},
                    {181, 14},
                    {182, 1},
                    {183, 14},
                    {184, 14},
                    {185, 14},
                    {186, 14},
                    {187, 14},
                    {188, 14},
                    {189, 14},
                    {190, 14},
                    {191, 14},
                    {192, 14},
                    {193, 0},
                    {194, 1},
                    {195, 1},
                    {196, 1},
                    {197, 1},
                    {198, 1},
                    {199, 1},
                    {200, 1},
                    {201, 3},
                    {202, 4},
                    {203, 1},
                    {204, 6},
                    {205, 5},
                    {206, 5},
                    {207, 5},
                    {208, 0},
                    {209, 0},
                    {210, 0},
                    {211, 0},
                    {212, 14},
                    {213, 2},
                    {214, 14},
                    {215, 14},
                    {216, 15},
                    {217, 14},
                    {218, 14},
                    {219, 15},
                    {220, 14},
                    {221, 14},
                    {222, 11},
                    {223, 5},
                    {224, 1},
                    {225, 8},
                    {226, 0},
                    {227, 4},
                    {228, 4},
                    {229, 14},
                    {230, 14},
                    {231, 4},
                    {232, 14},
                    {233, 14},
                    {234, 11},
                    {235, 0},
                    {236, 0},
                    {237, 5},
                    {238, 2},
                    {239, 2},
                    {240, 14},
                    {241, 2},
                    {242, 2},
                    {243, 4},
                    {244, 6},
                    {245, 4},
                    {246, 3},
                    {247, 2},
                    {248, 6},
                    {249, 6},
                    {250, 0},
                    {251, 12},
                    {252, 0},
                    {253, 12},
                    {254, 12},
                    {255, 4},
                    {256, 0},
                    {257, 0},
                    {258, 0},
                    {259, 0},
                    {260, 0},
                    {261, 14},
                    {262, 4},
                    {263, 4},
                    {264, 6},
                    {265, 4},
                    {266, 14},
                    {267, 14},
                    {268, 14},
                    {269, 14},
                    {270, 4},
                    {271, 0},
                    {272, 1},
                    {273, 0},
                    {274, 3},
                    {275, 4},
                    {276, 4},
                    {277, 164},
                    {278, 165},
                    {279, 4},
                    {280, 4},
                    {281, 6},
                    {282, 0},
                    {283, 166},
                    {284, 4},
                    {285, 17},
                    {286, 167},
                    {287, 3},
                    {288, 6},
                    {289, 2},
                    {290, 5},
                    {291, 168},
                    {292, 14},
                    {293, 14},
                    {294, 14},
                    {295, 14},
                    {296, 4},
                    {297, 4},
                    {298, 4},
                    {299, 0},
                    {300, 6},
                    {301, 6},
                    {302, 6},
                    {303, 14},
                    {304, 14},
                    {305, 4},
                    {306, 4},
                    {307, 6},
                    {308, 4},
                    {309, 14},
                    {310, 14},
                    {311, 14},
                    {312, 14},
                    {313, 0},
                    {314, 4},
                    {315, 4},
                    {316, 1},
                    {317, 1},
                    {318, 11},
                    {319, 11},
                    {320, 17},
                    {321, 1},
                    {322, 1},
                    {323, 0},
                    {324, 4},
                    {325, 0},
                    {326, 6},
                    {327, 113},
                    {328, 4},
                    {329, 4},
                    {330, 4},
                    {331, 4},
                    {332, 4},
                    {333, -1},
                    {334, 0},
                    {335, 8},
                    {336, 4},
                    {337, 11},
                    {338, 14},
                    {339, 14},
                    {340, 14},
                    {341, 4},
                    {342, 4},
                    {343, 4},
                    {344, 14},
                    {345, 0},
                    {346, 184},
                    {347, 184},
                    {348, 1},
                    {349, 4},
                    {350, 0},
                    {351, 0},
                    {352, 4},
                    {353, 4},
                    {354, 11},
                    {355, 184},
                    {356, 8},
                    {357, 2},
                    {358, 6},
                    {359, 4},
                    {360, 14},
                    {361, 4},
                    {362, 14},
                    {363, 4},
                    {364, 14},
                    {365, 114},
                    {366, 114},
                    {367, 114},
                    {368, 6},
                    {369, 2},
                    {370, 12},
                    {371, 4},
                    {372, 3},
                    {373, 4},
                    {374, 4},
                    {375, 4},
                    {376, 14},
                    {377, 0},
                    {378, 12},
                    {379, 4},
                    {380, 4},
                    {381, 14},
                    {382, 0},
                    {383, 4},
                    {384, 0},
                    {385, 4},
                    {386, 14},
                    {387, 14},
                    {388, 4},
                    {389, 4},
                    {390, 14},
                    {391, 14},
                    {392, 0},
                    {393, 6},
                    {394, 5},
                    {395, 6},
                    {396, 5},
                    {398, 6},
                    {399, 5},
                    {400, 1},
                    {401, 1},
                    {402, 1},
                    {403, 14},
                    {404, 15},
                    {405, 15},
                    {406, 0},
                    {407, 15},
                    {408, 1},
                    {409, 14},
                    {410, 1},
                    {411, 6},
                    {412, 5},
                    {413, 6},
                    {414, 14},
                    {415, 15},
                    {416, 6},
                    {417, 2},
                    {418, 0},
                    {419, 15},
                    {420, 15},
                    {421, 14},
                    {422, 14},
                    {423, 1},
                    {424, 11},
                    {425, 0},
                    {426, 14},
                    {427, 14},
                    {428, 1},
                    {429, 0},
                    {430, 0},
                    {431, 1},
                    {432, 14},
                    {433, 14},
                    {434, 14},
                    {435, 0},
                    {436, 0},
                    {437, 1},
                    {438, 1},
                    {439, 14},
                    {440, 14},
                    {441, 1},

                    {495, 0},
                    {496, 1},
                    {497, 14},
                    {498, 1},
                    {499, 1},
                    {500, 1},
                    {501, 1},
                    {502, 1},
                    {503, 1},
                    {504, 1},
                    {505, 1},
                    {506, 1},
                    {507, 1},
                    {508, 1},
                    {509, 1},
                    {510, 4},
                    {511, 1},
                    {512, 4},
                    {513, 1},
                    {514, 1},
                    {515, 1},
                    {516, 1},



                }
            },
            {
                false, new Dictionary<int, int>()
                {
                    {0, 0},
                    {1, 5},
                    {2, 2},
                    {3, 3},
                    {4, 4},
                    {5, 4},
                    {6, 5},
                    {7, 5},
                    {8, 5},
                    {9, 0},
                    {10, 5},
                    {11, 4},
                    {12, 12},
                    {13, 15},
                    {14, 14},
                    {15, 15},
                    {16, 15},
                    {17, 0},
                    {18, 15},
                    {19, 15},
                    {20, 5},
                    {21, 4},
                    {22, 4},
                    {23, 4},
                    {24, 5},
                    {25, 5},
                    {26, 12},
                    {27, 0},
                    {28, 15},
                    {29, 9},
                    {30, 2},
                    {31, 5},
                    {32, 4},
                    {33, 4},
                    {34, 6},
                    {35, 5},
                    {36, 4},
                    {37, 4},
                    {38, 2},
                    {39, 1},
                    {40, 2},
                    {41, 5},
                    {42, 5},
                    {43, 3},
                    {44, 3},
                    {45, 3},
                    {46, 3},
                    {47, 3},
                    {48, 14},
                    {49, 14},
                    {50, 14},
                    {51, 6},
                    {52, 6},
                    {53, 5},
                    {54, 9},
                    {55, 5},
                    {56, 14},
                    {57, 5},
                    {58, 5},
                    {59, 5},
                    {60, 14},
                    {61, 3},
                    {62, 5},
                    {63, 5},
                    {64, 5},
                    {65, 5},
                    {66, 6},
                    {67, 2},
                    {68, 0},
                    {69, 9},
                    {70, 0},
                    {71, 0},
                    {72, 0},
                    {73, 14},
                    {74, 15},
                    {75, 9},
                    {76, 9},
                    {77, 9},
                    {78, 9},
                    {79, 9},
                    {80, 9},
                    {81, 9},
                    {82, 15},
                    {83, 9},
                    {84, 14},
                    {85, 14},
                    {86, 9},
                    {87, 9},
                    {88, 0},
                    {89, 0},
                    {90, 6},
                    {91, 6},
                    {92, 5},
                    {93, 5},
                    {94, 5},
                    {95, 5},
                    {96, 4},
                    {97, 5},
                    {98, 5},
                    {99, 5},
                    {100, 11},
                    {101, 15},
                    {102, 3},
                    {103, 3},
                    {104, 5},
                    {105, 4},
                    {106, 6},
                    {107, 6},
                    {108, 6},
                    {109, 6},
                    {110, 6},
                    {111, 4},
                    {112, 4},
                    {113, 4},
                    {114, 4},
                    {115, 4},
                    {116, 4},
                    {117, 11},
                    {118, 11},
                    {119, 11},
                    {120, 6},
                    {121, 6},
                    {122, 2},
                    {123, 3},
                    {124, 0},
                    {125, 14},
                    {126, 14},
                    {127, 14},
                    {128, 14},
                    {129, 14},
                    {130, 0},
                    {131, 3},
                    {132, 2},
                    {133, 5},
                    {134, 0},
                    {135, 3},
                    {136, 3},
                    {137, 5},
                    {138, 6},
                    {139, 5},
                    {140, 5},
                    {141, 14},
                    {142, 9},
                    {143, 5},
                    {144, 3},
                    {145, 3},
                    {146, 7},
                    {147, 1},
                    {148, 5},
                    {149, 5},
                    {150, 0},
                    {151, 0},
                    {152, 7},
                    {153, 5},
                    {154, 15},
                    {155, 15},
                    {156, 15},
                    {157, 15},
                    {158, 15},
                    {159, 15},
                    {160, 6},
                    {161, 11},
                    {162, 0},
                    {163, 5},
                    {164, 5},
                    {165, 5},
                    {166, 5},
                    {167, 15},
                    {168, 15},
                    {169, 15},
                    {170, 15},
                    {171, 15},
                    {172, 14},
                    {173, 15},
                    {174, 15},
                    {175, 15},
                    {176, 15},
                    {177, 15},
                    {178, 15},
                    {179, 11},
                    {180, 3},
                    {181, 15},
                    {182, 15},
                    {183, 15},
                    {184, 14},
                    {185, 6},
                    {186, 6},
                    {187, 6},
                    {188, 6},
                    {189, 6},
                    {190, 6},
                    {191, 6},
                    {192, 5},
                    {193, 5},
                    {194, 6},
                    {195, 4},
                    {196, 1},
                    {197, 1},
                    {198, 1},
                    {199, 1},
                    {200, 1},
                    {201, 1},
                    {202, 2},
                    {203, 8},
                    {204, 4},
                    {205, 2},
                    {206, 1},
                    {207, 4},
                    {208, 11},
                    {209, 11},
                    {210, 11},
                    {211, 11},
                    {212, 0},
                    {213, 1},
                    {214, 1},
                    {215, 1},
                    {216, 5},
                    {217, 4},
                    {218, 0},
                    {219, 5},
                    {220, 15},
                    {221, 15},
                    {222, 15},
                    {223, 15},
                    {224, 14},
                    {225, 15},
                    {226, 11},
                    {227, 3},
                    {228, 3},
                    {229, 4},
                    {230, 0},
                    {231, 0},
                    {232, 0},
                    {233, 11},
                    {234, 6},
                    {235, 1},
                    {236, 14},
                    {237, 3},
                    {238, 3},
                    {239, 3},
                    {240, 5},
                    {241, 3},
                    {242, 6},
                    {243, 6},
                    {244, 9},
                    {245, 14},
                    {246, 14},
                    {247, 4},
                    {248, 5},
                    {249, 14},
                    {250, 4},
                    {251, 4},
                    {252, 1},
                    {253, 4},
                    {254, 13},
                    {255, 11},
                    {256, 3},
                    {257, 3},
                    {258, 14},
                    {259, 4},
                    {260, 3},
                    {261, 3},
                    {262, 9},
                    {263, 3},
                    {264, 3},
                    {265, 3},
                    {266, 3},
                    {267, 1},
                    {268, 1},
                    {269, 14},
                    {270, 6},
                    {271, 3},
                    {272, 3},
                    {273, 6},
                    {274, 3},
                    {275, 6},
                    {276, 6},
                    {277, 6},
                    {278, 6},
                    {279, 15},
                    {280, 14},
                    {281, 14},
                    {282, 3},
                    {283, 12},
                    {284, 4},
                    {285, 3},
                    {286, 14},
                    {287, 13},
                    {288, 4},
                    {289, 4},
                    {290, 4},
                    {291, 4},
                    {292, 3},
                    {293, 3},
                    {294, 1},
                    {295, 14},
                    {296, 4},
                    {297, 4},
                    {298, 4},
                    {299, 4},
                    {300, 4},
                    {301, 1},
                    {302, 4},
                    {303, 131},
                    {304, 4},
                    {305, 6},
                    {306, 6},
                    {307, 3},
                    {308, 3},
                    {309, 3},
                    {310, 14},
                    {311, 3},
                    {312, 3},
                    {313, 3},
                    {314, 6},
                    {315, 6},
                    {316, 3},
                    {317, 3},
                    {318, 1},
                    {319, 3},
                    {320, 6},
                    {321, 4},
                    {322, 11},
                    {323, 11},
                    {324, 14},
                    {325, 4},
                    {326, 4},
                    {327, 3},
                    {328, 3},
                    {329, 9},
                    {330, 9},
                    {331, 4},
                    {332, 3},
                    {333, 3},
                    {334, 11},
                    {335, 14},
                    {336, 3},
                    {337, 14},
                    {338, 14},
                    {339, 5},
                    {340, 5},
                    {341, 1},
                    {342, 11},
                    {343, 3},
                    {344, 3},
                    {345, 3},
                    {346, 3},
                    {347, 3},
                    {348, 4},
                    {349, 0},
                    {350, 1},
                    {351, 3},
                    {352, 0},
                    {353, 6},
                    {354, 6},
                    {355, 6},
                    {356, 3},
                    {357, 0},
                    {358, 0},
                    {359, 0},
                    {360, 0},
                    {361, 3},
                    {362, 3},
                    {363, 6},
                    {364, 15},
                    {365, 15},
                    {366, 3},
                    {367, 3},
                    {368, 14},
                    {369, 14},
                    {370, 3},
                    {371, 3},
                    {372, 14},
                    {373, 15},
                    {374, 3},
                    {375, 161},
                    {376, 9},
                    {377, 14},
                    {378, 3},
                    {379, 6},
                    {380, 3},
                    {381, 15},
                    {382, 9},
                    {383, 153},
                    {384, 4},
                    {385, 15},
                    {386, 4},
                    {387, 7},
                    {388, 4},
                    {389, 3},
                    {390, 3},
                    {391, 8},
                    {392, 3},
                    {393, 3},
                    {394, 3},
                    {395, 14},
                    {396, 9},
                    {397, 3},
                    {398, 153},
                    {399, 6},
                    {400, 14},
                    {401, 4},
                    {402, 3},
                    {403, 6},
                    {404, 11},
                    {405, 11},
                    {406, 1},
                    {407, 3},
                    {408, 3},
                    {409, 3},
                    {410, 3},
                    {411, 6},
                    {412, 6},
                    {413, 14},
                    {414, 14},
                    {415, 12},
                    {416, 7},
                    {417, 4},
                    {418, 7},
                    {419, 4},
                    {421, 7},
                    {422, 4},
                    {423, 9},
                    {424, 7},
                    {425, 4},
                    {426, 3},
                    {427, 6},
                    {428, 4},
                    {429, 4},
                    {430, 14},
                    {431, 14},
                    {432, 4},
                    {433, 4},
                    {434, 4},
                    {435, 3},
                    {436, 3},
                    {437, 12},
                    {438, 12},
                    {439, 3},
                    {440, 15},
                    {441, 6},
                    {442, 4},
                    {443, 1},
                    {444, 12},
                    {445, 0},
                    {446, 12},
                    {447, 0},
                    {448, 15},
                    {449, 15},
                    {450, 6},
                    {451, 6},
                    {452, 9},
                    {453, 9},
                    {454, 0},
                    {455, 0},
                    {456, 1},
                    {457, 1},
                    {458, 3},
                    {459, 0},
                    {460, 0},
                    {461, 0},
                    {462, 3},
                    {463, 6},
                    {464, 6},
                    {465, 6},
                    {466, 9},
                    {467, 9},
                    {468, 3},
                    {469, 3},
                    {470, 6},
                    {471, 6},
                    {472, 3},

                    {534, 14},
                    {535, 4},
                    {536, 3},
                    {537, 3},
                    {538, 4},
                    {539, 4},
                    {540, 3},
                    {541, 15},
                    {542, 4},
                    {543, 3},
                    {544, 3},
                    {545, 1},
                    {546, 1},
                    {547, 4},
                    {548, 4},
                    {550, 12},
                    {551, 15},
                    {552, 3},
                    {553, 15},
                    {554, 3},
                    {555, 15},
                    {556, 15},
                    {557, 5},
                    {558, 15},
                    {559, 15},
                    {560, 5},
                    {561, 15},
                    {562, 15},
                    {563, 9},
                    {564, 15},
                    {565, 15},
                    {566, 15},
                    {567, 15},
                    {568, 15},
                    {569, 15},
                    {570, 15},
                    {571, 15},
                    {572, 15},
                    {573, 15},
                    {574, 15},
                    {575, 15},
                    {576, 15},
                    {577, 15},
                    {578, 15},
                    {579, 15},
                    {580, 15},
                    {581, 15},
                    {582, 15},
                    {583, 15},
                    {584, 15},
                    {585, 15},
                    {586, 15},
                    {587, 15},
                    {588, 15},
                    {589, 15},
                    {590, 15},
                    {591, 15},
                }
            },
        };

        public static Dictionary<bool, Dictionary<int, int>> EmtptySlots = new Dictionary<bool, Dictionary<int, int>>()
        {
            { true, new Dictionary<int, int>() {
                { 1, 0 },
                { 3, 15 },
                { 4, 21 },
                { 5, 0 },
                { 6, 34 },
                { 7, 0 },
                { 8, 15 },
                { 9, 0 },
                { 10, 0 },
                { 11, 15 },
            }},
            { false, new Dictionary<int, int>() {
                { 1, 0 },
                { 3, 15 },
                { 4, 15 },
                { 5, 0 },
                { 6, 35 },
                { 7, 0 },
                { 8, 6 },
                { 9, 0 },
                { 10, 0 },
                { 11, 15 },
            }}
        };

        public static Dictionary<bool, Dictionary<int, int>> EmtptySlotsAccessory = new Dictionary<bool, Dictionary<int, int>>()
        {
            { true, new Dictionary<int, int>() {
                { 0, 8 }, // Hats
                { 1, 0 }, // Glasses
                { 2, 3 }, // Ears
                { 6, 2 }, // Watches
                //{ 7, 0 }, // Bracelets Нету вариции
            }},
            { false, new Dictionary<int, int>() {
                { 0, 57 }, // Hats
                { 1, 5 }, // Glasses
                //{ 2, 3 }, // Ears Нету вариции
                { 6, 1 }, // Watches
                //{ 7, 0 }, // Bracelets Нету вариции
            }}
        };

        public static Dictionary<bool, Dictionary<int, Dictionary<int, int>>> CorrectGloves = new Dictionary<bool, Dictionary<int, Dictionary<int, int>>>()
        {
            { true, new Dictionary<int, Dictionary<int, int>>() {
                { 1, new Dictionary<int, int>() {
                    { 4, 16 },
                }},
                { 2, new Dictionary<int, int>() {
                    { 4, 17 },
                }},
                { 3, new Dictionary<int, int>() {
                    { 4, 18 },
                }},
                { 4, new Dictionary<int, int>() {
                    { 0, 19 },
                    { 1, 20 },
                    { 2, 21 },
                    { 4, 22 },
                    { 5, 23 },
                    { 6, 24 },
                    { 8, 25 },
                    { 11, 26 },
                    { 12, 27 },
                    { 14, 28 },
                    { 15, 29 },
                    { 112, 115 },
                    { 113, 122 },
                    { 114, 129 },
                }},
                { 5, new Dictionary<int, int>() {
                    { 0, 30 },
                    { 1, 31 },
                    { 2, 32 },
                    { 4, 33 },
                    { 5, 34 },
                    { 6, 35 },
                    { 8, 36 },
                    { 11, 37 },
                    { 12, 38 },
                    { 14, 39 },
                    { 15, 40 },
                    { 112, 116 },
                    { 113, 123 },
                    { 114, 130 },
                }},
                { 6, new Dictionary<int, int>() {
                    { 0, 41 },
                    { 1, 42 },
                    { 2, 43 },
                    { 4, 44 },
                    { 5, 45 },
                    { 6, 46 },
                    { 8, 47 },
                    { 11, 48 },
                    { 12, 49 },
                    { 14, 50 },
                    { 15, 51 },
                    { 112, 117 },
                    { 113, 124 },
                    { 114, 131 },
                }},
                { 7, new Dictionary<int, int>() {
                    { 0, 52 },
                    { 1, 53 },
                    { 2, 54 },
                    { 4, 55 },
                    { 5, 56 },
                    { 6, 57 },
                    { 8, 58 },
                    { 11, 59 },
                    { 12, 60 },
                    { 14, 61 },
                    { 15, 62 },
                    { 112, 118 },
                    { 113, 125 },
                    { 114, 132 },
                }},
                { 8, new Dictionary<int, int>() {
                    { 0, 63 },
                    { 1, 64 },
                    { 2, 65 },
                    { 4, 66 },
                    { 5, 67 },
                    { 6, 68 },
                    { 8, 69 },
                    { 11, 70 },
                    { 12, 71 },
                    { 14, 72 },
                    { 15, 73 },
                    { 112, 119 },
                    { 113, 126 },
                    { 114, 133 },
                }},
                { 9, new Dictionary<int, int>() {
                    { 0, 74 },
                    { 1, 75 },
                    { 2, 76 },
                    { 4, 77 },
                    { 5, 78 },
                    { 6, 79 },
                    { 8, 80 },
                    { 11, 81 },
                    { 12, 82 },
                    { 14, 83 },
                    { 15, 84 },
                    { 112, 120 },
                    { 113, 127 },
                    { 114, 134 },
                }},
                { 10, new Dictionary<int, int>() {
                    { 0, 85 },
                    { 1, 86 },
                    { 2, 87 },
                    { 4, 88 },
                    { 5, 89 },
                    { 6, 90 },
                    { 8, 91 },
                    { 11, 92 },
                    { 12, 93 },
                    { 14, 94 },
                    { 15, 95 },
                    { 112, 121 },
                    { 113, 128 },
                    { 114, 135 },
                }},
                { 11, new Dictionary<int, int>() {
                    { 4, 96 },
                }},
                { 12, new Dictionary<int, int>() {
                    { 4, 97 },
                }},
                { 13, new Dictionary<int, int>() {
                    { 0, 99 },
                    { 1, 100 },
                    { 2, 101 },
                    { 4, 102 },
                    { 5, 103 },
                    { 6, 104 },
                    { 8, 105 },
                    { 11, 106 },
                    { 12, 107 },
                    { 14, 108 },
                    { 15, 109 },
                }},
                { 14, new Dictionary<int, int>() {
                    { 4, 110 },
                }},
            }},
            { false, new Dictionary<int, Dictionary<int, int>>() {
                { 1, new Dictionary<int, int>(){
                    { 3, 17 },
                }},
                { 2, new Dictionary<int, int>(){
                    { 3, 18 },
                }},
                { 3, new Dictionary<int, int>(){
                    { 3, 19 },
                }},
                { 4, new Dictionary<int, int>(){
                    { 0, 20 },
                    { 1, 21 },
                    { 2, 22 },
                    { 3, 23 },
                    { 4, 24 },
                    { 5, 25 },
                    { 6, 26 },
                    { 7, 27 },
                    { 9, 28 },
                    { 11, 29 },
                    { 12, 30 },
                    { 14, 31 },
                    { 15, 32 },
                    { 129, 132 },
                    { 130, 139 },
                    { 131, 146 },
                    { 153, 154 },
                    { 161, 162 },
                }},
                { 5, new Dictionary<int, int>(){
                    { 0, 33 },
                    { 1, 34 },
                    { 2, 35 },
                    { 3, 36 },
                    { 4, 37 },
                    { 5, 38 },
                    { 6, 39 },
                    { 7, 40 },
                    { 9, 41 },
                    { 11, 42 },
                    { 12, 43 },
                    { 14, 44 },
                    { 15, 45 },
                    { 129, 133 },
                    { 130, 140 },
                    { 131, 147 },
                    { 153, 155 },
                    { 161, 163 },
                }},
                { 6, new Dictionary<int, int>(){
                    { 0, 46 },
                    { 1, 47 },
                    { 2, 48 },
                    { 3, 49 },
                    { 4, 50 },
                    { 5, 51 },
                    { 6, 52 },
                    { 7, 53 },
                    { 9, 54 },
                    { 11, 55 },
                    { 12, 56 },
                    { 14, 57 },
                    { 15, 58 },
                    { 129, 134 },
                    { 130, 141 },
                    { 131, 148 },
                    { 153, 156 },
                    { 161, 164 },
                }},
                { 7, new Dictionary<int, int>(){
                    { 0, 59 },
                    { 1, 60 },
                    { 2, 61 },
                    { 3, 62 },
                    { 4, 63 },
                    { 5, 64 },
                    { 6, 65 },
                    { 7, 66 },
                    { 9, 67 },
                    { 11, 68 },
                    { 12, 69 },
                    { 14, 70 },
                    { 15, 71 },
                    { 129, 135 },
                    { 130, 142 },
                    { 131, 149 },
                    { 153, 157 },
                    { 161, 165 },
                }},
                { 8, new Dictionary<int, int>(){
                    { 0, 72 },
                    { 1, 73 },
                    { 2, 74 },
                    { 3, 75 },
                    { 4, 76 },
                    { 5, 77 },
                    { 6, 78 },
                    { 7, 79 },
                    { 9, 80 },
                    { 11, 81 },
                    { 12, 82 },
                    { 14, 83 },
                    { 15, 84 },
                    { 129, 136 },
                    { 130, 143 },
                    { 131, 150 },
                    { 153, 158 },
                    { 161, 166 },
                }},
                { 9, new Dictionary<int, int>(){
                    { 0, 85 },
                    { 1, 86 },
                    { 2, 87 },
                    { 3, 88 },
                    { 4, 89 },
                    { 5, 90 },
                    { 6, 91 },
                    { 7, 92 },
                    { 9, 93 },
                    { 11, 94 },
                    { 12, 95 },
                    { 14, 96 },
                    { 15, 97 },
                    { 129, 137 },
                    { 130, 144 },
                    { 131, 151 },
                    { 153, 159 },
                    { 161, 167 },
                }},
                { 10, new Dictionary<int, int>(){
                    { 0, 98 },
                    { 1, 99 },
                    { 2, 100 },
                    { 3, 101 },
                    { 4, 102 },
                    { 5, 103 },
                    { 6, 104 },
                    { 7, 105 },
                    { 9, 106 },
                    { 11, 107 },
                    { 12, 108 },
                    { 14, 109 },
                    { 15, 110 },
                    { 129, 138 },
                    { 130, 145 },
                    { 131, 152 },
                    { 153, 160 },
                    { 161, 168 },
                }},
                { 11, new Dictionary<int, int>(){
                    { 3, 111 },
                }},
                { 12, new Dictionary<int, int>(){
                    { 0, 114 },
                    { 1, 115 },
                    { 2, 116 },
                    { 3, 117 },
                    { 4, 118 },
                    { 5, 119 },
                    { 6, 120 },
                    { 7, 121 },
                    { 9, 122 },
                    { 11, 123 },
                    { 12, 124 },
                    { 14, 125 },
                    { 15, 126 },
                }},
            }},
        };

        // словарь, в котором находятся соответствующие для key undershirts IDшники Underwears
        public static Dictionary<bool, Dictionary<int, int>> Undershirts = new Dictionary<bool, Dictionary<int, int>>()
        {
            { true, new Dictionary<int, int>(){
                { 0 , 0 },
                { 2 , 0 },
                { 1 , 1 },
                { 14 , 1 },
                { 5 , 2 },
                { 8 , 3 },
                { 9 , 4 },
                { 12 , 5 },
                { 13 , 6 },
                { 29 , 7 },
                { 30 , 7 },
                { 16 , 8 },
                { 18 , 8 },
                { 17 , 9 },
                { 19 , 10 },
                { 20 , 10 },
                { 23 , 11 },
                { 24 , 11 },
                { 27 , 12 },
                { 37 , 13 },
                { 39 , 13 },
                { 38 , 14 },
                { 44 , 14 },
                { 40 , 15 },
                { 41 , 16 },
                { 42 , 17 },
                { 43 , 18 },
                { 45 , 19 },
                { 46 , 20 },
                { 47 , 21 },
                { 48 , 21 },
                { 52 , 22 },
                { 53 , 23 },
                { 54 , 23 },
                { 67 , 24 },
                { 68 , 24 },
                { 89 , 24 },
                { 65 , 25 },
                { 66 , 25 },
                { 74 , 27 },
                { 75 , 27 },
                { 76 , 28 },
                { 77 , 28 },
                { 85 , 28 },
                { 79 , 29 },
                { 80 , 29 },
                { 100 , 30 },
                { 101 , 30 },
                { 102 , 30 },
                { 105 , 31 },
                { 106 , 31 },
                { 107 , 31 },
                { 109 , 32 },
                { 110 , 33 },
                { 111 , 34 },
                { 114 , 35 },
                { 115 , 35 },
                { 116 , 35 },
                { 119 , 36 },
                { 120 , 36 },
                { 121 , 36 },
                { 134 , 37 },
                { 135 , 37 },
                { 136 , 37 },
                { 140 , 38 },
                { 141 , 38 },
                { 142 , 38 },
                { 150 , 39 },
                { 157 , 39 },
                { 158 , 40 },
                { 178 , 40 },
                { 175 , 41 },
                { 176 , 41 },
                { 177 , 41 },
                { 186 , 42 },
                { 187 , 42 },
                { 188 , 42 },

            }},
            { false, new Dictionary<int, int>(){
                { 0 , 0 },
                { 1 , 0 },
                { 11 , 1 },
                { 13 , 2 },
                { 16 , 3 },
                { 18 , 4 },
                { 19 , 4 },
                { 22 , 5 },
                { 20 , 6 },
                { 21 , 6 },
                { 23 , 7 },
                { 24 , 8 },
                { 47 , 9 },
                { 27 , 10 },
                { 29 , 11 },
                { 30 , 12 },
                { 31 , 13 },
                { 51 , 14 },
                { 48 , 15 },
                { 45 , 16 },
                { 46 , 16 },
                { 57 , 17 },
                { 58 , 17 },
                { 60 , 18 },
                { 61 , 19 },
                { 62 , 19 },
                { 68 , 20 },
                { 69 , 21 },
                { 70 , 21 },
                { 72 , 22 },
                { 73 , 22 },
                { 76 , 23 },
                { 77 , 23 },
                { 78 , 24 },
                { 83 , 25 },
                { 84 , 25 },
                { 106 , 26 },
                { 107 , 26 },
                { 110 , 26 },
                { 112 , 27 },
                { 113 , 27 },
                { 116 , 27 },
                { 120 , 28 },
                { 122 , 28 },
                { 123 , 28 },
                { 126 , 29 },
                { 128 , 29 },
                { 129 , 29 },
                { 130 , 30 },
                { 131 , 30 },
                { 134 , 30 },
                { 136 , 31 },
                { 137 , 31 },
                { 140 , 31 },
                { 144 , 32 },
                { 146 , 32 },
                { 147 , 32 },
                { 150 , 33 },
                { 151 , 34 },
                { 164 , 35 },
                { 166 , 35 },
                { 167 , 35 },
                { 170 , 36 },
                { 172 , 36 },
                { 173 , 36 },
                { 176 , 37 },
                { 175 , 38 },
                { 180 , 39 },
                { 182 , 39 },
                { 183 , 39 },
                { 217 , 40 },
                { 194 , 41 },
                { 216 , 41 },
                { 203 , 42 },
                { 205 , 42 },
                { 206 , 42 },
                { 210 , 43 },
                { 212 , 43 },
                { 214 , 44 },
                { 215 , 44 },
                { 224 , 45 },
                { 226 , 45 },
                { 227 , 45 },
                { 230 , 46 },
                { 232 , 46 },
                { 233 , 46 },



            }},
        };

        public static Dictionary<bool, Dictionary<int, Underwear>> Underwears = new Dictionary<bool, Dictionary<int, Underwear>>()
        {
            { true, new Dictionary<int, Underwear>(){
                { 0, new Underwear(0, 2870, new Dictionary<int, int>(){ { 0, 0 }, { 1, 2 } }, new List<int>() { 0, 1, 2, 3, 4, 5, 7, 8, 11 }) },
                { 1, new Underwear(1, 3960, new Dictionary<int, int>(){ { 0, 1 }, { 1, 14 } }, new List<int>() { 0, 1, 3, 4, 5, 6, 7, 8, 11, 12, 14 }) },
                { 2, new Underwear(5, 1830, new Dictionary<int, int>(){ { 0, 5 } }, new List<int>() { 0, 1, 2, 7 }) },
                { 3, new Underwear(8, 3370, new Dictionary<int, int>(){ { 0, 8 } }, new List<int>() { 0, 10, 13, 14 }) },
                { 4, new Underwear(9, 4930, new Dictionary<int, int>(){ { 0, 9 } }, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 10, 11, 12, 13, 14, 15 }) },
                { 5, new Underwear(12, 3580, new Dictionary<int, int>(){ { 0, 12 } }, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 }) },
                { 6, new Underwear(13, 3580, new Dictionary<int, int>(){ { 0, 13 } }, new List<int>() { 0, 1, 2, 3, 5, 13 }) },
                { 7, new Underwear(14, 5125, new Dictionary<int, int>(){ { 0, 29 }, { 1, 30 } }, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 }) },
                { 8, new Underwear(16, 3960, new Dictionary<int, int>(){ { 0, 16 }, { 1, 18 } }, new List<int>() { 0, 1, 2 }) },
                { 9, new Underwear(17, 3195, new Dictionary<int, int>(){ { 0, 17 } }, new List<int>() { 0, 1, 2, 3, 4, 5 }) },
                { 10, new Underwear(18, 4770, new Dictionary<int, int>(){ { 0, 19 }, { 1, 20 } }, new List<int>() { 0, 1, 2, 3 }) },
                { 11, new Underwear(22, 4130, new Dictionary<int, int>(){ { 0, 23 }, { 1, 24 } }, new List<int>() { 0, 1, 2 }) },
                { 12, new Underwear(26, 575, new Dictionary<int, int>(){ { 0, 27 } }, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }) },
                { 13, new Underwear(33, 1180, new Dictionary<int, int>(){ { 0, 37 }, { 1, 39 } }, new List<int>() { 0 }) },
                { 14, new Underwear(34, 1320, new Dictionary<int, int>(){ { 0, 38 }, { 1, 44 } }, new List<int>() { 0, 1 }) },
                { 15, new Underwear(36, 3235, new Dictionary<int, int>(){ { 0, 40 } }, new List<int>() { 0, 1, 2, 3, 4, 5 }) },
                { 16, new Underwear(38, 5780, new Dictionary<int, int>(){ { 0, 41 } }, new List<int>() { 0, 1, 2, 3, 4 }) },
                { 17, new Underwear(39, 4190, new Dictionary<int, int>(){ { 0, 42 } }, new List<int>() { 0, 1 }) },
                { 18, new Underwear(41, 5020, new Dictionary<int, int>(){ { 0, 43 } }, new List<int>() { 0, 1, 2, 3 }) },
                { 19, new Underwear(42, 980, new Dictionary<int, int>(){ { 0, 45 } }, new List<int>() { 0 }) },
                { 20, new Underwear(43, 980, new Dictionary<int, int>(){ { 0, 46 } }, new List<int>() { 0 }) },
                { 21, new Underwear(44, 5310, new Dictionary<int, int>(){ { 0, 47 }, { 1, 48 } }, new List<int>() { 0, 1, 2, 3 }) },
                { 22, new Underwear(45, 4595, new Dictionary<int, int>(){ { 0, 52 } }, new List<int>() { 0, 1, 2 }) },
                { 23, new Underwear(47, 3420, new Dictionary<int, int>(){ { 0, 53 }, { 1, 54 } }, new List<int>() { 0, 1 }) },
                { 24, new Underwear(71, 11380, new Dictionary<int, int>(){ { 0, 67 }, { 1, 68 }, { 2, 89 } }, new List<int>() { 0 }) },
                { 25, new Underwear(73, 17530, new Dictionary<int, int>(){ { 0, 65 }, { 1, 66 } }, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18 }) },
                { 27, new Underwear(139, 3945, new Dictionary<int, int>(){ { 0, 75 }, { 1, 74 } }, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7 }) },
                { 28, new Underwear(146, 3390, new Dictionary<int, int>(){ { 0, 76 }, { 1, 77 }, { 2, 85 } }, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8 }) },
                { 29, new Underwear(152, 17250, new Dictionary<int, int>(){ { 0, 79 }, { 1, 80 } }, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 }) },
                { 30, new Underwear(208, 12430, new Dictionary<int, int>(){ { 0, 101 }, { 1, 102 }, { 2, 100 } }, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23 }) },
                { 31, new Underwear(226, 1355, new Dictionary<int, int>(){ { 0, 107 }, { 1, 106 }, { 2, 105 } }, new List<int>() { 0 }) },
                { 32, new Underwear(235, 5390, new Dictionary<int, int>(){ { 0, 109 } }, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 }) },
                { 33, new Underwear(236, 5390, new Dictionary<int, int>(){ { 0, 110 } }, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 }) },
                { 34, new Underwear(237, 3340, new Dictionary<int, int>(){ { 0, 111 } }, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25 }) },
                { 35, new Underwear(238, 1945, new Dictionary<int, int>(){ { 0, 115 }, { 1, 116 }, { 2, 114 } }, new List<int>() { 0, 1, 2, 3, 4, 5 }) },
                { 36, new Underwear(239, 12430, new Dictionary<int, int>(){ { 0, 120 }, { 1, 121 }, { 2, 119 } }, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23 }) },
                { 37, new Underwear(271, 1610, new Dictionary<int, int>(){ { 0, 135 }, { 1, 136 }, { 2, 134 } }, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 }) },
                { 38, new Underwear(273, 31625, new Dictionary<int, int>(){ { 0, 141 }, { 1, 142 }, { 2, 140 } }, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21 }) },
                { 39, new Underwear(321, 23000, new Dictionary<int, int>(){ { 0, 150 }, { 2, 157 } }, new List<int>() { 0 }) },
                { 40, new Underwear(322, 23000, new Dictionary<int, int>(){ { 0, 178 }, { 2, 158 } }, new List<int>() { 0 }) },
                { 41, new Underwear(351, 13800, new Dictionary<int, int>(){ { 0, 176 }, { 1, 177 }, { 2, 175 } }, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }) },
                { 42, new Underwear(392, 12650, new Dictionary<int, int>(){ { 0, 187 }, { 1, 188 }, { 2, 186 } }, new List<int>() { 0, 1, 2, 3, 4 }) },






            }},
            { false, new Dictionary<int, Underwear>(){
                { 0, new Underwear(0, 4370, new Dictionary<int, int>(){ { 0, 0 }, { 1, 1 } }, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 }) },
                { 1, new Underwear(11, 3250, new Dictionary<int, int>(){ { 0, 11 } }, new List<int>() { 0, 1, 2, 10, 11, 15 }) },
                { 2, new Underwear(13, 11730, new Dictionary<int, int>(){ { 0, 13 } }, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 }) },
                { 3, new Underwear(16, 2185, new Dictionary<int, int>(){ { 0, 16 } }, new List<int>() { 0, 1, 2, 3, 4, 5, 6 }) },
                { 4, new Underwear(19, 3185, new Dictionary<int, int>(){ { 0, 18 }, { 1, 19 } }, new List<int>() { 0, 1, 2, 3 }) },
                { 5, new Underwear(22, 16675, new Dictionary<int, int>(){ { 0, 22 } }, new List<int>() { 0, 1, 2, 3, 4 }) },
                { 6, new Underwear(23, 2730, new Dictionary<int, int>(){ { 0, 20 }, { 1, 21 } }, new List<int>() { 0, 1, 2 }) },
                { 7, new Underwear(26, 8970, new Dictionary<int, int>(){ { 0, 23 } }, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 }) },
                { 8, new Underwear(27, 3170, new Dictionary<int, int>(){ { 2, 24 } }, new List<int>() { 0, 1, 2, 3, 4, 5 }) },
                { 9, new Underwear(30, 3325, new Dictionary<int, int>(){ { 0, 47 } }, new List<int>() { 0, 1, 2 }) },
                { 10, new Underwear(32, 3680, new Dictionary<int, int>(){ { 0, 27 } }, new List<int>() { 0, 1, 2 }) },
                { 11, new Underwear(36, 5290, new Dictionary<int, int>(){ { 0, 29 } }, new List<int>() { 0, 1, 2, 3, 4 }) },
                { 12, new Underwear(38, 3520, new Dictionary<int, int>(){ { 0, 30 } }, new List<int>() { 0, 1, 2, 3 }) },
                { 13, new Underwear(40, 3675, new Dictionary<int, int>(){ { 0, 31 } }, new List<int>() { 0, 1 }) },
                { 14, new Underwear(49, 2175, new Dictionary<int, int>(){ { 0, 51 } }, new List<int>() { 0, 1 }) },
                { 15, new Underwear(67, 11380, new Dictionary<int, int>(){ { 0, 48 } }, new List<int>() { 0 }) },
                { 16, new Underwear(68, 17530, new Dictionary<int, int>(){ { 0, 45 }, { 1, 46 } }, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19 }) },
                { 17, new Underwear(73, 1840, new Dictionary<int, int>(){ { 0, 57 }, { 1, 58 } }, new List<int>() { 0, 1, 2 }) },
                { 18, new Underwear(74, 1420, new Dictionary<int, int>(){ { 0, 60 } }, new List<int>() { 0, 1, 2 }) },
                { 19, new Underwear(75, 2190, new Dictionary<int, int>(){ { 0, 61 }, { 1, 62 } }, new List<int>() { 0, 1, 2, 3 }) },
                { 20, new Underwear(111, 19780, new Dictionary<int, int>(){ { 0, 68 } }, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 }) },
                { 21, new Underwear(117, 1840, new Dictionary<int, int>(){ { 0, 69 }, { 1, 70 } }, new List<int>() { 0, 1, 2 }) },
                { 22, new Underwear(118, 1840, new Dictionary<int, int>(){ { 0, 72 }, { 1, 73 } }, new List<int>() { 0, 1, 2 }) },
                { 23, new Underwear(136, 3945, new Dictionary<int, int>(){ { 0, 77 }, { 1, 76 } }, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7 }) },
                { 24, new Underwear(141, 3490, new Dictionary<int, int>(){ { 0, 78 } }, new List<int>() { 0, 1, 2, 3, 4, 5 }) },
                { 25, new Underwear(149, 17250, new Dictionary<int, int>(){ { 0, 84 }, { 1, 83 } }, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 }) },
                { 26, new Underwear(208, 22730, new Dictionary<int, int>(){ { 0, 106 }, { 1, 107 }, { 2, 110 } }, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 }) },
                { 27, new Underwear(209, 22730, new Dictionary<int, int>(){ { 0, 112 }, { 1, 113 }, { 2, 116 } }, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 }) },
                { 28, new Underwear(212, 12430, new Dictionary<int, int>(){ { 0, 123 }, { 1, 122 }, { 2, 120 } }, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23 }) },
                { 29, new Underwear(224, 12430, new Dictionary<int, int>(){ { 0, 128 }, { 1, 129 }, { 2, 126 } }, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23 }) },
                { 30, new Underwear(225, 12430, new Dictionary<int, int>(){ { 0, 130 }, { 1, 131 }, { 2, 134 } }, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23 }) },
                { 31, new Underwear(226, 12430, new Dictionary<int, int>(){ { 0, 136 }, { 1, 137 }, { 2, 140 } }, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23 }) },
                { 32, new Underwear(236, 1355, new Dictionary<int, int>(){ { 0, 147 }, { 1, 146 }, { 2, 144 } }, new List<int>() { 0 }) },
                { 33, new Underwear(246, 5390, new Dictionary<int, int>(){ { 0, 150 } }, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 }) },
                { 34, new Underwear(247, 3340, new Dictionary<int, int>(){ { 0, 151 } }, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25 }) },
                { 35, new Underwear(280, 1610, new Dictionary<int, int>(){ { 0, 167 }, { 1, 166 }, { 2, 164 } }, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 }) },
                { 36, new Underwear(281, 1610, new Dictionary<int, int>(){ { 0, 172 }, { 1, 173 }, { 2, 170 } }, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 }) },
                { 37, new Underwear(283, 7645, new Dictionary<int, int>(){ { 0, 176 } }, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 }) },
                { 38, new Underwear(284, 14390, new Dictionary<int, int>(){ { 0, 175 } }, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 }) },
                { 39, new Underwear(286, 4600, new Dictionary<int, int>(){ { 0, 183 }, { 1, 182 }, { 2, 180 } }, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21 }) },
                { 40, new Underwear(332, 23000, new Dictionary<int, int>(){ { 0, 217 } }, new List<int>() { 0 }) },
                { 41, new Underwear(333, 23000, new Dictionary<int, int>(){ { 0, 216 }, { 2, 194 } }, new List<int>() { 0 }) },
                { 42, new Underwear(369, 5175, new Dictionary<int, int>(){ { 0, 203 }, { 1, 205 }, { 2, 206 } }, new List<int>() { 0, 1, 2, 3, 4 }) },
                { 43, new Underwear(377, 5175, new Dictionary<int, int>(){ { 0, 210 }, { 2, 212 } }, new List<int>() { 0, 1, 2, 3, 4, 5 }) },
                { 44, new Underwear(395, 8625, new Dictionary<int, int>(){ { 0, 215 }, { 1, 214 } }, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 }) },
                { 45, new Underwear(413, 12650, new Dictionary<int, int>(){ { 0, 227 }, { 1, 226 }, { 2, 224 } }, new List<int>() { 0, 1, 2, 3, 4 }) },
                { 46, new Underwear(414, 12650, new Dictionary<int, int>(){ { 0, 232 }, { 1, 233 }, { 2, 230 } }, new List<int>() { 0, 1, 2, 3, 4 }) },



                // trash
                //{ 27, new Underwear(9, 2000, new Dictionary<int, int>() { }, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14 }) },
                //{ 28, new Underwear(83, 4000, new Dictionary<int, int>() { }, new List<int>() { 0, 1, 2, 3, 4, 5, 6 }) },
                //{ 29, new Underwear(142, 7000, new Dictionary<int, int>() { }, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 }) },
                //{ 30, new Underwear(171, 2500, new Dictionary<int, int>() { }, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7 }) },
            }},
        };

        public static int newHats = 14;

        public static Dictionary<bool, List<Clothes>> Hats = new Dictionary<bool, List<Clothes>>()
        {
            { true, new List<Clothes>(){
                new Clothes(2, new List<int>() {0,1,2,3,4,5,6,7}, 650, -1),
                new Clothes(4, new List<int>() {0,1}, 550, -1),
                new Clothes(5, new List<int>() {0,1}, 500, -1),
                new Clothes(7, new List<int>() {0,1,2,3,4,5,6,7}, 950, -1),
                new Clothes(12, new List<int>() {0,1,2,4,6,7}, 2500, -1),
                new Clothes(13, new List<int>() {0,1,2,3,4,5,6,7}, 4500, -1),
                new Clothes(14, new List<int>() {0,1,2,3,4,5,6,7}, 3200, -1),
                new Clothes(20, new List<int>() {0,1,2,3,4,5}, 2400, -1),
                new Clothes(21, new List<int>() {0,1,2,3,4,5,6,7}, 4500, -1),
                new Clothes(25, new List<int>() {0,1,2}, 6500, -1),
                new Clothes(26, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13}, 4000, -1),
                new Clothes(28, new List<int>() {0,1,2,3,4,5}, 1800, -1),
                new Clothes(29, new List<int>() {0,1,2,3,4,5,6,7}, 9500, -1),
                new Clothes(30, new List<int>() {0,1}, 7800, -1),
                new Clothes(31, new List<int>() {0}, 10000, -1),
                new Clothes(34, new List<int>() {0}, 8000, -1),
                new Clothes(36, new List<int>() {0}, 6000, -1),
                new Clothes(37, new List<int>() {0,1,2,3,4,5}, 6300, -1),
                new Clothes(40, new List<int>() {0,1,2,3,4,5,6,7}, 4000, -1),
                new Clothes(42, new List<int>() {0,1,2,3}, 4300, -1),
                new Clothes(44, new List<int>() {0,1,2,3,4,5,6,7}, 3200, -1),
                new Clothes(45, new List<int>() {0,1,2,3,4,5,6,7}, 3200, -1),
                new Clothes(109, new List<int>() {0,1,2,3,4,5,6,7,8,9,10}, 5100, -1),
                new Clothes(110, new List<int>() {0,1,2,3,4,5,6,7,8,9,10}, 5100, -1),
                new Clothes(132, new List<int>() {0,1,2,3}, 4900, -1),
                new Clothes(135, new List<int>() {0,1,2,3,4,5,6,7,8,9,10}, 8100, -1),
                new Clothes(135, new List<int>() {11,12,13,14,15,16,17,18,19,20,21,22,23}, 19200, -1),
                new Clothes(136, new List<int>() {0,1,2,3,4,5,6,7,8,9,10}, 8100, -1),
                new Clothes(136, new List<int>() {11,12,13,14,15,16,17,18,19,20,21,22,23}, 19200, -1),
                new Clothes(139, new List<int>() {0,1,2}, 4400, -1),
                new Clothes(140, new List<int>() {0,1,2}, 4400, -1),
                new Clothes(142, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 13500, -1),
                new Clothes(143, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 13500, -1),
                new Clothes(146, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13}, 11500, -1),
                new Clothes(151, new List<int>() {0,1,2,3,4,5,6,7}, 11700, -1),
                new Clothes(152, new List<int>() {0,1,2,3,4,5,6,7}, 11700, -1),
                new Clothes(153, new List<int>() {0}, 7500, -1),
                new Clothes(154, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13}, 14200, -1),
                new Clothes(155, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13}, 14200, -1),
                new Clothes(162, new List<int>() {0,1,2,3,4,5,6,7,8}, 16500, -1),
                new Clothes(163, new List<int>() {0,1,2,3,4,5,6,7,8}, 16500, -1),
                new Clothes(164, new List<int>() {0}, 9300, -1),
                new Clothes(165, new List<int>() {0}, 9300, -1),
                new Clothes(166, new List<int>() {0,1}, 6900, -1),
                new Clothes(167, new List<int>() {0}, 7400, -1),
                new Clothes(168, new List<int>() {0,1}, 6900, -1),
                new Clothes(169, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 12300, -1),
                new Clothes(170, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13}, 27000, -1),
                new Clothes(171, new List<int>() {0}, 7400, -1),
                new Clothes(172, new List<int>() {0}, 7500, -1),
                new Clothes(173, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 8100, -1),
                new Clothes(174, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 8100, -1),
                new Clothes(175, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 4500, -1),
                new Clothes(176, new List<int>() {0}, 3300, -1),
                new Clothes(177, new List<int>() {0}, 3300, -1),
                new Clothes(178, new List<int>() {0,1}, 14000, -1),
                new Clothes(179, new List<int>() {0}, 11600, -1),
                new Clothes(180, new List<int>() {0}, 14300, -1),
                new Clothes(181, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 32000, -1),
                new Clothes(182, new List<int>() {0,1,2}, 30000, -1),
                new Clothes(183, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 32000, -1),
                new Clothes(184, new List<int>() {0,1,2}, 30000, -1),
                new Clothes(185, new List<int>() {0}, 15000, -1),
                new Clothes(186, new List<int>() {0}, 15000, -1),



                /*Trash Hats
                //new Clothes(156, new List<int>() { 0, 1, 2, 3, 4 }, 100),
                //new Clothes(157, new List<int>() { 0, 1, 2, 3, 4 }, 100),
                //new Clothes(158, new List<int>() { 0, 1, 2, 3, 4 }, 100),
                //new Clothes(159, new List<int>() { 0, 1, 2, 3, 4 }, 100),
                //new Clothes(160, new List<int>() { 0, 1, 2, 3, 4 }, 100),
                //new Clothes(161, new List<int>() { 0, 1, 2, 3, 4 }, 100),
                //new Clothes(162, new List<int>() { 0, 1, 2, 3, 4 }, 100),
                //new Clothes(163, new List<int>() { 0, 1, 2, 3, 4 }, 100),
                //new Clothes(164, new List<int>() { 0, 1, 2, 3, 4 }, 100),
                //new Clothes(169, new List<int>() { 0, 1, 2, 3, 4 }, 100),
                //new Clothes(170, new List<int>() { 0, 1, 2, 3, 4 }, 100),
                //new Clothes(147, new List<int>() { 0, 1, 2, 3, 4 }, 100),
                //new Clothes(148, new List<int>() { 0, 1, 2, 3, 4 }, 100),
                //new Clothes(149, new List<int>() { 0, 1, 2, 3, 4 }, 100),
                //new Clothes(150, new List<int>() { 0, 1, 2, 3, 4 }, 100),
                //new Clothes(144, new List<int>() { 0, 1, 2, 3, 4 }, 100),
                //new Clothes(145, new List<int>() { 0, 1, 2, 3, 4 }, 100),
                //new Clothes(141, new List<int>() { 0, 1, 2, 3, 4 }, 100),
                //new Clothes(137, new List<int>() { 0, 1, 2, 3, 4 }, 100),
                //new Clothes(138, new List<int>() { 0, 1, 2, 3, 4 }, 100),
                //new Clothes(133, new List<int>() { 0, 1, 2, 3, 4 }, 100),
                //new Clothes(134, new List<int>() { 0, 1, 2, 3, 4 }, 100),
                 */

            }},
            { false, new List<Clothes>(){
                new Clothes(4, new List<int>() {0,1,2,3,4,5,6,7}, 550, -1),
                new Clothes(5, new List<int>() {0,1,2,3,4,5,6,7}, 500, -1),
                new Clothes(6, new List<int>() {0,1,2,3,4,5,6,7}, 2500, -1),
                new Clothes(7, new List<int>() {0,1,2,3,4,5,6,7}, 2200, -1),
                new Clothes(13, new List<int>() {0,1,2,3,4,5,6,7}, 2800, -1),
                new Clothes(14, new List<int>() {0,1,2,3,4,5,6,7}, 3600, -1),
                new Clothes(20, new List<int>() {0,1,2,3,4,5,6}, 1400, -1),
                new Clothes(21, new List<int>() {0,1,2,3,4,5,6}, 2400, -1),
                new Clothes(22, new List<int>() {0,1,2,3,4,5,6}, 5200, -1),
                new Clothes(27, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13}, 4000, -1),
                new Clothes(29, new List<int>() {0,1,2,3,4}, 1800, -1),
                new Clothes(30, new List<int>() {0}, 10000, -1),
                new Clothes(33, new List<int>() {0}, 8000, -1),
                new Clothes(35, new List<int>() {0}, 6000, -1),
                new Clothes(36, new List<int>() {0,1,2,3,4,5}, 6300, -1),
                new Clothes(39, new List<int>() {0,1,2,3,4,5,6,7}, 4000, -1),
                new Clothes(41, new List<int>() {0,1,2,3}, 4300, -1),
                new Clothes(43, new List<int>() {0,1,2,3,4,5,6,7}, 3200, -1),
                new Clothes(44, new List<int>() {0,1,2,3,4,5,6,7}, 3200, -1),
                new Clothes(54, new List<int>() {0,1,2,3,4,5,6,7}, 6300, -1),
                new Clothes(94, new List<int>() {0,1,2,3,4,5,6,7,8,9}, 2500, -1),
                new Clothes(96, new List<int>() {0,1,2,3,4,5,6,7}, 25000, -1),
                new Clothes(98, new List<int>() {0,1,2,3}, 28000, -1),
                new Clothes(108, new List<int>() {0,1,2,3,4,5,6,7,8,9,10}, 5100, -1),
                new Clothes(109, new List<int>() {0,1,2,3,4,5,6,7,8,9,10}, 5100, -1),
                new Clothes(131, new List<int>() {0,1,2,3}, 4900, -1),
                new Clothes(134, new List<int>() {0,1,2,3,4,5,6,7,8,9,10}, 8100, -1),
                new Clothes(134, new List<int>() {11,12,13,14,15,16,17,18,19,20,21,22,23}, 19200, -1),
                new Clothes(135, new List<int>() {0,1,2,3,4,5,6,7,8,9,10}, 8100, -1),
                new Clothes(135, new List<int>() {11,12,13,14,15,16,17,18,19,20,21,22,23}, 19200, -1),
                new Clothes(138, new List<int>() {0,1,2}, 4400, -1),
                new Clothes(139, new List<int>() {0,1,2}, 4400, -1),
                new Clothes(141, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 13500, -1),
                new Clothes(142, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 13500, -1),
                new Clothes(145, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13}, 11500, -1),
                new Clothes(150, new List<int>() {0,1,2,3,4,5,6,7}, 11700, -1),
                new Clothes(151, new List<int>() {0,1,2,3,4,5,6,7}, 11700, -1),
                new Clothes(152, new List<int>() {0}, 7500, -1),
                new Clothes(153, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13}, 14200, -1),
                new Clothes(154, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13}, 14200, -1),
                new Clothes(161, new List<int>() {0,1,2,3,4,5,6,7,8}, 16500, -1),
                new Clothes(162, new List<int>() {0,1,2,3,4,5,6,7,8}, 16500, -1),
                new Clothes(163, new List<int>() {0}, 9300, -1),
                new Clothes(164, new List<int>() {0}, 9300, -1),
                new Clothes(165, new List<int>() {0,1}, 6900, -1),
                new Clothes(166, new List<int>() {0}, 7400, -1),
                new Clothes(167, new List<int>() {0,1}, 6900, -1),
                new Clothes(168, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 12300, -1),
                new Clothes(169, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13}, 27000, -1),
                new Clothes(170, new List<int>() {0}, 7400, -1),
                new Clothes(171, new List<int>() {0}, 7500, -1),
                new Clothes(172, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 8100, -1),
                new Clothes(173, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 8100, -1),
                new Clothes(174, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 4500, -1),
                new Clothes(175, new List<int>() {0}, 3300, -1),
                new Clothes(176, new List<int>() {0}, 3300, -1),
                new Clothes(177, new List<int>() {0,1}, 14000, -1),
                new Clothes(178, new List<int>() {0}, 11600, -1),
                new Clothes(179, new List<int>() {0}, 14300, -1),
                new Clothes(180, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 32000, -1),
                new Clothes(181, new List<int>() {0,1,2}, 30000, -1),
                new Clothes(182, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 32000, -1),
                new Clothes(183, new List<int>() {0,1,2}, 30000, -1),
                new Clothes(184, new List<int>() {0}, 15000, -1),
                new Clothes(185, new List<int>() {0}, 15000, -1),






            }},
        };

        public static int newMaleLegs = 12;
        public static int newFemaleLegs = 13;

        public static Dictionary<bool, List<Clothes>> Legs = new Dictionary<bool, List<Clothes>>()
        {
            { true, new List<Clothes>(){
                new Clothes(0, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 3120, -1),
                new Clothes(1, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 2040, -1),
                new Clothes(3, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 1440, -1),
                new Clothes(4, new List<int>() {0,1,2,4}, 1800, -1),
                new Clothes(5, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 840, -1),
                new Clothes(6, new List<int>() {0,1,2,10}, 1560, -1),
                new Clothes(7, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 3480, -1),
                new Clothes(8, new List<int>() {0,3,4,14}, 1920, -1),
                new Clothes(9, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 9120, -1),
                new Clothes(10, new List<int>() {0,1,2}, 7440, -1),
                new Clothes(12, new List<int>() {0,4,5,7,12}, 900, -1),
                new Clothes(13, new List<int>() {0,1,2}, 4800, -1),
                new Clothes(14, new List<int>() {0,1,3,12}, 540, -1),
                new Clothes(15, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 3360, -1),
                new Clothes(16, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 5880, -1),
                new Clothes(17, new List<int>() {0,1,2,3,4,5,6,7,8,9,10}, 4560, -1),
                new Clothes(18, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 8880, -1),
                new Clothes(19, new List<int>() {0,1}, 6000, -1),
                new Clothes(20, new List<int>() {0,1,2,3}, 7800, -1),
                new Clothes(22, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12}, 11400, -1),
                new Clothes(23, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12}, 11040, -1),
                new Clothes(24, new List<int>() {0,1,2,3,4,5,6}, 5880, -1),
                new Clothes(25, new List<int>() {0,1,2,3,4,5,6}, 5880, -1),
                new Clothes(26, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 18000, -1),
                new Clothes(27, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 3720, -1),
                new Clothes(28, new List<int>() {1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 13800, -1),
                new Clothes(29, new List<int>() {0,1,2}, 8400, -1),
                new Clothes(32, new List<int>() {0,1,2,3}, 8160, -1),
                new Clothes(34, new List<int>() {0}, 6000, -1),
                new Clothes(35, new List<int>() {0}, 4800, -1),
                new Clothes(37, new List<int>() {0,1,2,3}, 3360, -1),
                new Clothes(41, new List<int>() {0}, 10800, -1),
                new Clothes(42, new List<int>() {0,1,2,3,4,5,6,7}, 1080, -1),
                new Clothes(43, new List<int>() {0,1}, 1020, -1),
                new Clothes(45, new List<int>() {0,1,2,3,4,5,6}, 32400, -1),
                new Clothes(46, new List<int>() {0,1}, 6000, -1),
                new Clothes(47, new List<int>() {0,1}, 3600, -1),
                new Clothes(48, new List<int>() {0,1,2,3,4}, 11400, -1),
                new Clothes(49, new List<int>() {0,1,2,3,4}, 11400, -1),
                new Clothes(50, new List<int>() {0,1,2,3}, 14400, -1),
                new Clothes(51, new List<int>() {0}, 24000, -1),
                new Clothes(52, new List<int>() {0,1,2,3}, 14400, -1),
                new Clothes(53, new List<int>() {0}, 24000, -1),
                new Clothes(54, new List<int>() {0,1,2,3,4,5,6}, 32400, -1),
                new Clothes(55, new List<int>() {0,1,2,3}, 540, -1),
                new Clothes(58, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 15000, -1),
                new Clothes(60, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 12000, -1),
                new Clothes(61, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13}, 17400, -1),
                new Clothes(62, new List<int>() {0,1,2,3}, 1680, -1),
                new Clothes(63, new List<int>() {0}, 3720, -1),
                new Clothes(64, new List<int>() {0,1,2,3,4,5,6,7,8,9,10}, 1020, -1),
                new Clothes(65, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13}, 25800, -1),
                new Clothes(69, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17}, 24000, -1),
                new Clothes(71, new List<int>() {0,1,2,3,4,5}, 17400, -1),
                new Clothes(73, new List<int>() {0,1,2,3,4,5}, 18600, -1),
                new Clothes(75, new List<int>() {0,1,2,3,4,5,6,7}, 2100, -1),
                new Clothes(77, new List<int>() {0,1,2,3,4,5,6,7,8,9,10}, 120000, -1),
                new Clothes(78, new List<int>() {0,1,2,3,4,5,6,7}, 10920, -1),
                new Clothes(79, new List<int>() {0,1,2}, 20760, -1),
                new Clothes(80, new List<int>() {0,1,2,3,4,5,6,7}, 13200, -1),
                new Clothes(81, new List<int>() {0,1,2}, 20760, -1),
                new Clothes(82, new List<int>() {0,1,2,3,4,5,6,7,8,9}, 2940, -1),
                new Clothes(83, new List<int>() {0,1,2,3}, 20520, -1),
                new Clothes(85, new List<int>() {0,1,2}, 96000, -1),
                new Clothes(86, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23}, 23400, -1),
                new Clothes(88, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23}, 23400, -1),
                new Clothes(89, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 21000, -1),
                new Clothes(90, new List<int>() {0,1,2,3,4,5,6,7,8,9}, 1320, -1),
                new Clothes(91, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13}, 22800, -1),
                new Clothes(92, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19}, 17040, -1),
                new Clothes(93, new List<int>() {0}, 10800, -1),
                new Clothes(94, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 16560, -1),
                new Clothes(95, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 90000, -1),
                new Clothes(96, new List<int>() {0,1}, 6000, -1),
                new Clothes(98, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 19200, -1),
                new Clothes(99, new List<int>() {0,1,2,3,4,5,6}, 4800, -1),
                new Clothes(100, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13}, 34200, -1),
                new Clothes(101, new List<int>() {0,1}, 4800, -1),
                new Clothes(102, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17}, 28200, -1),
                new Clothes(103, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17}, 28200, -1),
                new Clothes(104, new List<int>() {0}, 2280, -1),
                new Clothes(105, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 18600, -1),
                new Clothes(106, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19}, 144000, -1),
                new Clothes(116, new List<int>() {0,1,2,3,4,5,6,7,8,9}, 37200, -1),
                new Clothes(117, new List<int>() {0,1,2,3,4,5,6,7,8,9,10}, 33000, -1),
                new Clothes(118, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 66000, -1),
                new Clothes(119, new List<int>() {0,1,2,3,4,5,6,7,8,9,10}, 44400, -1),
                new Clothes(122, new List<int>() {0}, 14400, -1),
                new Clothes(124, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19}, 30000, -1),
                new Clothes(125, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19}, 36000, -1),
                new Clothes(126, new List<int>() {0}, 2280, -1),
                new Clothes(128, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 25800, -1),
                new Clothes(129, new List<int>() {0,1,2,3,4,5,6,7}, 2760, -1),
                new Clothes(131, new List<int>() {0}, 25200, -1),
                new Clothes(132, new List<int>() {0,1,2}, 8640, -1),
                new Clothes(133, new List<int>() {0}, 19800, -1),
                new Clothes(136, new List<int>() {0,1}, 26400, -1),
                new Clothes(137, new List<int>() {0,1,2,3,4,5,6,7}, 16800, -1),
                new Clothes(138, new List<int>() {0,1,2,3,4,5,6,7,8,9}, 3000, -1),
                new Clothes(138, new List<int>() {10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 22200, -1),
                new Clothes(139, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 23160, -1),
                new Clothes(140, new List<int>() {0,1,2}, 6360, -1),
                new Clothes(141, new List<int>() {0,1,2,3,4,5,6}, 7800, -1),
                new Clothes(141, new List<int>() {7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 29400, -1),
                new Clothes(142, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21}, 34200, -1),
                new Clothes(143, new List<int>() {0,1,2,3,4,5}, 9000, -1),
                new Clothes(143, new List<int>() {6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 48000, -1),
                new Clothes(144, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 9500, -1),
                new Clothes(146, new List<int>() {0,1}, 4900, -1),
                new Clothes(147, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20}, 9900, -1),
                new Clothes(148, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 21000, -1),
                new Clothes(149, new List<int>() {0,1}, 11300, -1),
                new Clothes(150, new List<int>() {0}, 14500, -1),
                new Clothes(151, new List<int>() {0}, 27000, -1),
                new Clothes(152, new List<int>() {0}, 27000, -1),
                new Clothes(153, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 5100, -1),
                new Clothes(154, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18}, 41000, -1),
                new Clothes(155, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 51000, -1),
                new Clothes(156, new List<int>() {0,1,2,3}, 35000, -1),
                new Clothes(157, new List<int>() {0,1}, 19000, -1),
                new Clothes(158, new List<int>() {0,1}, 24000, -1),
                new Clothes(159, new List<int>() {0}, 21000, -1),

                // 160 -> 176 Обновление 13.06
                new Clothes(177, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19}, 370000, -1),
                new Clothes(178, new List<int>() {0,1,2,3,4,5}, 415000, -1),
                new Clothes(179, new List<int>() {0}, 390000, -1),
                new Clothes(180, new List<int>() {0,1,2,3,4,5,6,7}, 440000, -1),
                new Clothes(181, new List<int>() {0,1,2,3,4,5}, 425000, -1),
                new Clothes(182, new List<int>() {0}, 302485, -1),
                new Clothes(183, new List<int>() {0,1}, 468155, -1),
                new Clothes(184, new List<int>() {0,1,2,3,4,5,6,7,8,9}, 289400, -1),


                /* Trash Legs
                //new Clothes(107, new List<int>()  {0,1,2,3,4,5,6}, 100),
                //new Clothes(108, new List<int>()  {0,1,2,3,4,5,6}, 100),
                //new Clothes(109, new List<int>()  {0,1,2,3,4,5,6}, 100),
                //new Clothes(110, new List<int>()  {0,1,2,3,4,5,6}, 100),
                //new Clothes(111, new List<int>()  {0,1,2,3,4,5,6}, 100),
                //new Clothes(112, new List<int>()  {0,1,2,3,4,5,6}, 100),
                //new Clothes(113, new List<int>()  {0,1,2,3,4,5,6}, 100),
                //new Clothes(114, new List<int>()  {0,1,2,3,4,5,6}, 100),
                //new Clothes(115, new List<int>()  {0,1,2,3,4,5,6}, 100),
                //new Clothes(143, new List<int>()  {0,1,2,3,4,5,6}, 100),
                //new Clothes(119, new List<int>()  {0,1,2,3,4,5,6}, 100),
                //new Clothes(120, new List<int>()  {0,1,2,3,4,5,6}, 100),
                //new Clothes(127, new List<int>()  {0,1,2,3,4,5,6}, 100),
                //new Clothes(125, new List<int>()  {0,1,2,3,4,5,6}, 100),
                */
            }},
            { false, new List<Clothes>(){
                new Clothes(0, new List<int>() {0,1,2}, 1080, -1),
                new Clothes(0, new List<int>() {3,4,5,6,7,8,9,10,11,12,13,14,15}, 8400, -1),
                new Clothes(1, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 2040, -1),
                new Clothes(2, new List<int>() {0,1,2}, 960, -1),
                new Clothes(3, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 3480, -1),
                new Clothes(4, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 3720, -1),
                new Clothes(6, new List<int>() {0,1,2}, 7440, -1),
                new Clothes(7, new List<int>() {0,1,2}, 3840, -1),
                new Clothes(8, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,14,15}, 6000, -1),
                new Clothes(9, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 18000, -1),
                new Clothes(10, new List<int>() {0,1,2}, 840, -1),
                new Clothes(11, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 8760, -1),
                new Clothes(12, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 9840, -1),
                new Clothes(14, new List<int>() {0,1,8,9}, 1320, -1),
                new Clothes(16, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 2640, -1),
                new Clothes(17, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 1560, -1),
                new Clothes(18, new List<int>() {0,1}, 6000, -1),
                new Clothes(19, new List<int>() {0,1,2,3,4}, 5400, -1),
                new Clothes(20, new List<int>() {0,1,2}, 15000, -1),
                new Clothes(22, new List<int>() {0,1,2}, 15000, -1),
                new Clothes(23, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12}, 15840, -1),
                new Clothes(24, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12}, 9000, -1),
                new Clothes(25, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12}, 6120, -1),
                new Clothes(26, new List<int>() {0}, 9600, -1),
                new Clothes(27, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 25200, -1),
                new Clothes(28, new List<int>() {0}, 6000, -1),
                new Clothes(30, new List<int>() {0,1,2,3,4}, 4800, -1),
                new Clothes(31, new List<int>() {0,1,2,3}, 8160, -1),
                new Clothes(33, new List<int>() {0}, 6000, -1),
                new Clothes(36, new List<int>() {0,1,2,3}, 8400, -1),
                new Clothes(37, new List<int>() {0,1,2,3,4,5,6}, 5880, -1),
                new Clothes(41, new List<int>() {0,1,2,3}, 3360, -1),
                new Clothes(43, new List<int>() {0,1,2,3,4}, 11160, -1),
                new Clothes(44, new List<int>() {0,1,2,3,4}, 11160, -1),
                new Clothes(45, new List<int>() {0,1,2,3}, 3960, -1),
                new Clothes(47, new List<int>() {0,1,2,3,4,5,6}, 32400, -1),
                new Clothes(49, new List<int>() {0,1}, 3600, -1),
                new Clothes(50, new List<int>() {0,1,2,3,4}, 11400, -1),
                new Clothes(51, new List<int>() {0,1,2,3,4}, 11400, -1),
                new Clothes(52, new List<int>() {0,1,2,3}, 14400, -1),
                new Clothes(53, new List<int>() {0}, 24000, -1),
                new Clothes(54, new List<int>() {0,1,2,3}, 14400, -1),
                new Clothes(55, new List<int>() {0}, 24000, -1),
                new Clothes(56, new List<int>() {0,1,2,3,4,5}, 20400, -1),
                new Clothes(57, new List<int>() {0,1,2,3,4,5,6,7}, 10800, -1),
                new Clothes(58, new List<int>() {0,1,2,3}, 540, -1),
                new Clothes(60, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 15000, -1),
                new Clothes(62, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 10200, -1),
                new Clothes(63, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 22200, -1),
                new Clothes(64, new List<int>() {0,1,2,3}, 3240, -1),
                new Clothes(66, new List<int>() {0,1,2,3,4,5,6,7,8,9,10}, 1560, -1),
                new Clothes(67, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13}, 25800, -1),
                new Clothes(71, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17}, 24000, -1),
                new Clothes(73, new List<int>() {0,1,2,3,4,5}, 2100, -1),
                new Clothes(74, new List<int>() {0,1,2,3,4,5}, 2940, -1),
                new Clothes(75, new List<int>() {0,1,2}, 9600, -1),
                new Clothes(76, new List<int>() {0,1,2}, 9600, -1),
                new Clothes(77, new List<int>() {0,1,2}, 9600, -1),
                new Clothes(78, new List<int>() {0,1,2,3}, 7320, -1),
                new Clothes(79, new List<int>() {0,1,2,3,4,5,6,7,8,9,10}, 2645, -1),
                new Clothes(80, new List<int>() {0,1,2,3,4,5,6,7}, 10920, -1),
                new Clothes(81, new List<int>() {0,1,2}, 20760, -1),
                new Clothes(82, new List<int>() {0,1,2,3,4,5,6,7}, 13200, -1),
                new Clothes(83, new List<int>() {0,1,2}, 20760, -1),
                new Clothes(84, new List<int>() {0,1,2,3,4,5,6,7,8,9}, 2940, -1),
                new Clothes(85, new List<int>() {0,1,2,3}, 20520, -1),
                new Clothes(87, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 18600, -1),
                new Clothes(88, new List<int>() {0,1,2}, 2645, -1),
                new Clothes(89, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23}, 23400, -1),
                new Clothes(91, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23}, 23400, -1),
                new Clothes(95, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19}, 17040, -1),
                new Clothes(96, new List<int>() {0}, 10800, -1),
                new Clothes(97, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 16560, -1),
                new Clothes(98, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 2645, -1),
                new Clothes(99, new List<int>() {0,1}, 6000, -1),
                new Clothes(101, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 19200, -1),
                new Clothes(102, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20}, 20760, -1),
                new Clothes(104, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13}, 34200, -1),
                new Clothes(106, new List<int>() {0,1,2,3,4,5,6,7}, 11160, -1),
                new Clothes(107, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 8760, -1),
                new Clothes(108, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 40200, -1),
                new Clothes(109, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17}, 28200, -1),
                new Clothes(110, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17}, 28200, -1),
                new Clothes(111, new List<int>() {0}, 2280, -1),
                new Clothes(112, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 18600, -1),
                new Clothes(113, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19}, 2645, -1),
                new Clothes(123, new List<int>() {0,1,2,3,4,5,6,7,8,9,10}, 33000, -1),
                new Clothes(124, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 66000, -1),
                new Clothes(125, new List<int>() {0,1,2,3,4,5,6,7,8,9,10}, 44400, -1),
                new Clothes(128, new List<int>() {0}, 14400, -1),
                new Clothes(130, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19}, 30000, -1),
                new Clothes(131, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19}, 36000, -1),
                new Clothes(133, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24}, 24000, -1),
                new Clothes(134, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 25800, -1),
                new Clothes(135, new List<int>() {0,1,2,3,4,5,6,7}, 2760, -1),
                new Clothes(137, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 3240, -1),
                new Clothes(138, new List<int>() {0}, 25200, -1),
                new Clothes(139, new List<int>() {0,1,2,3}, 8640, -1),
                new Clothes(140, new List<int>() {0}, 19800, -1),
                new Clothes(143, new List<int>() {0,1}, 26400, -1),
                new Clothes(144, new List<int>() {0,1,2,3,4,5,6,7}, 16800, -1),
                new Clothes(145, new List<int>() {0,1,2,3,4,5,6,7,8,9}, 3000, -1),
                new Clothes(145, new List<int>() {10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 22200, -1),
                new Clothes(146, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 23160, -1),
                new Clothes(147, new List<int>() {0,1,2}, 6360, -1),
                new Clothes(148, new List<int>() {0,1,2,3,4,5,6}, 7800, -1),
                new Clothes(148, new List<int>() {7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 29400, -1),
                new Clothes(149, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21}, 34200, -1),
                new Clothes(150, new List<int>() {0,1,2,3,4,5}, 9000, -1),
                new Clothes(150, new List<int>() {6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 48000, -1),
                new Clothes(151, new List<int>() {0,1}, 3500, -1),
                new Clothes(153, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 13200, -1),
                new Clothes(154, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 12100, -1),
                new Clothes(155, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20}, 9900, -1),
                new Clothes(156, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 7700, -1),
                new Clothes(157, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 13900, -1),
                new Clothes(158, new List<int>() {0,1,2}, 11300, -1),
                new Clothes(159, new List<int>() {0}, 14500, -1),
                new Clothes(160, new List<int>() {0}, 27000, -1),
                new Clothes(161, new List<int>() {0}, 27000, -1),
                new Clothes(162, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 5100, -1),
                new Clothes(163, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18}, 41000, -1),
                new Clothes(164, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 51000, -1),
                new Clothes(165, new List<int>() {0,1,2,3}, 35000, -1),
                new Clothes(166, new List<int>() {0,1}, 19000, -1),
                new Clothes(167, new List<int>() {0,1}, 24000, -1),
                new Clothes(168, new List<int>() {0}, 21000, -1),

                //22
                new Clothes(191, new List<int>() {0,1,2,3,4,5}, 415000, -1),
                new Clothes(192, new List<int>() {0,1,2}, 425000, -1),
                new Clothes(193, new List<int>() {0,1,2,3,4,5,6,7}, 445000, -1),
                new Clothes(194, new List<int>() {0}, 440000, -1),
                new Clothes(195, new List<int>() {0}, 420000, -1),
                new Clothes(196, new List<int>() {0,1,2,3,4,5,6,7}, 430000, -1),
                new Clothes(197, new List<int>() {0,1}, 468155, -1),
                new Clothes(198, new List<int>() {0,1,2,3,4,5,6}, 378500, -1),
                new Clothes(199, new List<int>() {0,1,2,3,4,5,6,7,8}, 341600, -1),
                new Clothes(200, new List<int>() {0,1,2,3,4}, 534600, -1),
                new Clothes(201, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 446000, -1),
                new Clothes(202, new List<int>() {0,1,2,3,4,5,6}, 378500, -1),
                new Clothes(203, new List<int>() {0,1,2}, 315800, -1),
                new Clothes(204, new List<int>() {0,1}, 524900, -1),
                new Clothes(205, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 184600, -1),
                new Clothes(206, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23}, 420000, -1),
                new Clothes(207, new List<int>() {0,1,2,3,4,5,6,7,8}, 479200, -1),

            }},
        };

        public static int newMaleFeets = 17;
        public static int newFemaleFeets = 17;
        public static Dictionary<bool, List<Clothes>> Feets = new Dictionary<bool, List<Clothes>>()
        {
            { true, new List<Clothes>(){
                new Clothes(1, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 6250, -1),
                new Clothes(3, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 8125, -1),
                new Clothes(4, new List<int>() {0,1,2,4}, 1938, -1),
                new Clothes(5, new List<int>() {0,1,2,3}, 438, -1),
                new Clothes(6, new List<int>() {0,1}, 438, -1),
                new Clothes(7, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 9125, -1),
                new Clothes(8, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 13750, -1),
                new Clothes(9, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 13750, -1),
                new Clothes(10, new List<int>() {0,7,12,14}, 12500, -1),
                new Clothes(12, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 15000, -1),
                new Clothes(14, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 5625, -1),
                new Clothes(15, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 16875, -1),
                new Clothes(16, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 1063, -1),
                new Clothes(17, new List<int>() {0}, 12500, -1),
                new Clothes(18, new List<int>() {0,1}, 15625, -1),
                new Clothes(19, new List<int>() {0}, 23125, -1),
                new Clothes(20, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 26875, -1),
                new Clothes(21, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 14625, -1),
                new Clothes(22, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 6625, -1),
                new Clothes(23, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 9000, -1),
                new Clothes(24, new List<int>() {0}, 6000, -1),
                new Clothes(25, new List<int>() {0}, 12000, -1),
                new Clothes(26, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 6625, -1),
                new Clothes(27, new List<int>() {0}, 938, -1),
                new Clothes(28, new List<int>() {0,1,2,3,4,5}, 15625, -1),
                new Clothes(29, new List<int>() {0}, 28125, -1),
                new Clothes(30, new List<int>() {0,1}, 1375, -1),
                new Clothes(31, new List<int>() {0,1,2,3,4}, 21875, -1),
                new Clothes(32, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 19375, -1),
                new Clothes(36, new List<int>() {0,1,2,3}, 1375, -1),
                new Clothes(40, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 26875, -1),
                new Clothes(41, new List<int>() {0}, 1125, -1),
                new Clothes(42, new List<int>() {0,1,2,3,4,5,6,7,8,9}, 6000, -1),
                new Clothes(43, new List<int>() {0,1,2,3,4,5,6,7}, 1688, -1),
                new Clothes(44, new List<int>() {0,1,2,3,4,5,6,7,8,9,10}, 16375, -1),
                new Clothes(46, new List<int>() {0,1,2,3,4,5,6,7,8,9}, 4625, -1),
                new Clothes(48, new List<int>() {0,1}, 1188, -1),
                new Clothes(49, new List<int>() {0,1}, 1188, -1),
                new Clothes(50, new List<int>() {0,1,2,3,4,5}, 3125, -1),
                new Clothes(51, new List<int>() {0,1,2,3,4,5}, 3125, -1),
                new Clothes(55, new List<int>() {0,1,2,3,4,5,6,7,8,9,10}, 43750, -1),
                new Clothes(56, new List<int>() {0,1}, 4125, -1),
                new Clothes(57, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 4750, -1),
                new Clothes(58, new List<int>() {0,1,2}, 40625, -1),
                new Clothes(59, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 20375, -1),
                new Clothes(61, new List<int>() {0,1,2,3,4,5,6,7}, 5125, -1),
                new Clothes(64, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13}, 11500, -1),
                new Clothes(65, new List<int>() {0,1,2,3,4,5,6}, 6625, -1),
                new Clothes(66, new List<int>() {0,1,2,3,4,5,6}, 6625, -1),
                new Clothes(69, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 14500, -1),
                new Clothes(70, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 22500, -1),
                new Clothes(71, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 22500, -1),
                new Clothes(72, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 23125, -1),
                new Clothes(73, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 23125, -1),
                new Clothes(74, new List<int>() {0,1}, 10000, -1),
                new Clothes(75, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 26625, -1),
                new Clothes(76, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 26625, -1),
                new Clothes(77, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 87500, -1),
                new Clothes(79, new List<int>() {0,1}, 10375, -1),
                new Clothes(80, new List<int>() {0,1}, 10375, -1),
                new Clothes(92, new List<int>() {0,1,2,3,4,5,6,7}, 31250, -1),
                new Clothes(93, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14}, 40375, -1),
                new Clothes(94, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14}, 40375, -1),
                new Clothes(95, new List<int>() {0}, 4625, -1),
                new Clothes(96, new List<int>() {0}, 7750, -1),
                new Clothes(97, new List<int>() {0}, 7750, -1),
                new Clothes(98, new List<int>() {0}, 8750, -1),
                new Clothes(99, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23}, 17875, -1),
                new Clothes(100, new List<int>() {0,1}, 37500, -1),
                new Clothes(101, new List<int>() {0,1,2,3,4,5,6,7}, 11875, -1),
                new Clothes(102, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13}, 35000, -1),
                new Clothes(103, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13}, 31250, -1),
                new Clothes(104, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 37500, -1),
                new Clothes(105, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23}, 3375, -1),
                new Clothes(106, new List<int>() {0,1,2,3,4,5,6,7,8,9,10}, 17000, -1),
                new Clothes(107, new List<int>() {0,1,2,3,4,5,6,7,8}, 21875, -1),
                new Clothes(108, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14}, 28125, -1),
                new Clothes(109, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 14500, -1),
                new Clothes(110, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13}, 13000, -1),
                new Clothes(111, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13}, 13000, -1),
                new Clothes(112, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 6500, -1),
                new Clothes(113, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 6500, -1),
                new Clothes(114, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 8900, -1),
                new Clothes(115, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 37000, -1),
                new Clothes(116, new List<int>() {0,1,2,3}, 37000, -1),
                new Clothes(117, new List<int>() {0}, 16000, -1),
                new Clothes(118, new List<int>() {0}, 16000, -1),
                new Clothes(119, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 55000, -1),
                new Clothes(120, new List<int>() {0,1}, 43000, -1),
                new Clothes(121, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 55000, -1),
                new Clothes(122, new List<int>() {0,1}, 43000, -1),
                new Clothes(123, new List<int>() {0,1,2,3,4,5,6,7}, 49000, -1),
                new Clothes(124, new List<int>() {0,1,2,3,4,5,6,7}, 49000, -1),
                new Clothes(125, new List<int>() {0}, 16000, -1),

                // 126 -> 134 Обновление 13.06
                new Clothes(135, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17}, 220000, -1),
                new Clothes(136, new List<int>() {0,1,2,3,4,5,6,7,8,9}, 260000, -1),
                new Clothes(137, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20}, 310000, -1),
                new Clothes(138, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13}, 245000, -1),
                new Clothes(139, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14}, 230000, -1),
                new Clothes(140, new List<int>() {0,1,2,3,4,5,6,7,8,9}, 280000, -1),
                new Clothes(142, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12}, 245000, -1),
                new Clothes(143, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13}, 280000, -1),
                new Clothes(144, new List<int>() {0,1,2,3,4,5,6,7,8,9}, 230000, -1),
                //Проебали 145 когда то.

                /* Trash Feets
                //new Clothes(86, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 8400),
                //new Clothes(87, new List<int>() {0,1,2,3,4,5}, 100),
                //new Clothes(88, new List<int>() {0,1,2,3,4,5}, 100),
                //new Clothes(89, new List<int>() {0,1,2,3,4,5}, 100),
                //new Clothes(90, new List<int>() {0,1,2,3,4,5}, 100),
                //new Clothes(91, new List<int>() {0,1,2,3,4,5}, 100),
                */
            }},
            { false, new List<Clothes>(){
                new Clothes(0, new List<int>() {0,1,2,3}, 1750, -1),
                new Clothes(1, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 18375, -1),
                new Clothes(2, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 9125, -1),
                new Clothes(3, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 16875, -1),
                new Clothes(4, new List<int>() {0,1,2,3}, 4625, -1),
                new Clothes(5, new List<int>() {0,1,10,13}, 438, -1),
                new Clothes(6, new List<int>() {0,1,2,3}, 9375, -1),
                new Clothes(7, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 14375, -1),
                new Clothes(8, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 16875, -1),
                new Clothes(9, new List<int>() {0,1,2,3,11,12}, 11625, -1),
                new Clothes(10, new List<int>() {0,1,2,3}, 5625, -1),
                new Clothes(11, new List<int>() {0,1,2,3}, 6500, -1),
                new Clothes(13, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 6250, -1),
                new Clothes(14, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 16500, -1),
                new Clothes(15, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 20625, -1),
                new Clothes(16, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 1063, -1),
                new Clothes(17, new List<int>() {0}, 12500, -1),
                new Clothes(18, new List<int>() {0,1,2}, 10000, -1),
                new Clothes(19, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 15625, -1),
                new Clothes(20, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 21625, -1),
                new Clothes(21, new List<int>() {0,1,2,3,4,5,6,7,8,9}, 15625, -1),
                new Clothes(22, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 28375, -1),
                new Clothes(23, new List<int>() {0,1,2}, 12250, -1),
                new Clothes(24, new List<int>() {0}, 6000, -1),
                new Clothes(25, new List<int>() {0}, 12000, -1),
                new Clothes(26, new List<int>() {0}, 938, -1),
                new Clothes(27, new List<int>() {0}, 1500, -1),
                new Clothes(28, new List<int>() {0}, 2375, -1),
                new Clothes(29, new List<int>() {0,1,2}, 12500, -1),
                new Clothes(30, new List<int>() {0}, 9750, -1),
                new Clothes(31, new List<int>() {0}, 28125, -1),
                new Clothes(32, new List<int>() {0,1,2,3,4}, 21875, -1),
                new Clothes(33, new List<int>() {0,1,2,3,4,5,6,7}, 5375, -1),
                new Clothes(37, new List<int>() {0,1,2,3}, 1375, -1),
                new Clothes(41, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 28125, -1),
                new Clothes(42, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 25000, -1),
                new Clothes(43, new List<int>() {0,1,2,3,4,5,6,7}, 11000, -1),
                new Clothes(44, new List<int>() {0,1,2,3,4,5,6,7}, 11000, -1),
                new Clothes(45, new List<int>() {0,1,2,3,4,5,6,7,8,9,10}, 16375, -1),
                new Clothes(47, new List<int>() {0,1,2,3,4,5,6,7,8,9}, 4625, -1),
                new Clothes(49, new List<int>() {0,1}, 1188, -1),
                new Clothes(50, new List<int>() {0,1}, 1188, -1),
                new Clothes(51, new List<int>() {0,1,2,3,4,5}, 3125, -1),
                new Clothes(52, new List<int>() {0,1,2,3,4,5}, 3125, -1),
                new Clothes(56, new List<int>() {0,1,2}, 3875, -1),
                new Clothes(58, new List<int>() {0,1,2,3,4,5,6,7,8,9,10}, 43750, -1),
                new Clothes(59, new List<int>() {0,1}, 4125, -1),
                new Clothes(60, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 4750, -1),
                new Clothes(61, new List<int>() {0,1,2}, 40625, -1),
                new Clothes(62, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 20375, -1),
                new Clothes(64, new List<int>() {0,1,2,3,4,5,6,7}, 5125, -1),
                new Clothes(67, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13}, 11500, -1),
                new Clothes(68, new List<int>() {0,1,2,3,4,5,6}, 6625, -1),
                new Clothes(69, new List<int>() {0,1,2,3,4,5,6}, 6625, -1),
                new Clothes(72, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 14500, -1),
                new Clothes(73, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 22500, -1),
                new Clothes(74, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 22500, -1),
                new Clothes(75, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 23125, -1),
                new Clothes(76, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 23125, -1),
                new Clothes(77, new List<int>() {0,1,2,3,4,5,6,7,8}, 25000, -1),
                new Clothes(78, new List<int>() {0,1}, 10000, -1),
                new Clothes(79, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 26625, -1),
                new Clothes(80, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 26625, -1),
                new Clothes(81, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 87500, -1),
                new Clothes(83, new List<int>() {0,1}, 10375, -1),
                new Clothes(84, new List<int>() {0,1}, 10375, -1),
                new Clothes(96, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14}, 40375, -1),
                new Clothes(97, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14}, 40375, -1),
                new Clothes(98, new List<int>() {0,1,2,3,4,5,6,7}, 31250, -1),
                new Clothes(99, new List<int>() {0}, 4625, -1),
                new Clothes(100, new List<int>() {0}, 7750, -1),
                new Clothes(101, new List<int>() {0}, 7750, -1),
                new Clothes(102, new List<int>() {0}, 8750, -1),
                new Clothes(103, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23}, 17875, -1),
                new Clothes(104, new List<int>() {0,1}, 37500, -1),
                new Clothes(105, new List<int>() {0,1,2,3,4,5,6,7}, 11875, -1),
                new Clothes(106, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13}, 35000, -1),
                new Clothes(107, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13}, 31250, -1),
                new Clothes(108, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 37500, -1),
                new Clothes(109, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23}, 3375, -1),
                new Clothes(110, new List<int>() {0,1,2,3,4,5,6,7,8,9,10}, 17000, -1),
                new Clothes(111, new List<int>() {0,1,2,3,4,5,6,7,8}, 21875, -1),
                new Clothes(112, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14}, 28125, -1),
                new Clothes(113, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 14500, -1),
                new Clothes(114, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13}, 13000, -1),
                new Clothes(115, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13}, 13000, -1),
                new Clothes(116, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 6500, -1),
                new Clothes(117, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 6500, -1),
                new Clothes(118, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 8900, -1),
                new Clothes(119, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 37000, -1),
                new Clothes(120, new List<int>() {0,1,2,3}, 37000, -1),
                new Clothes(121, new List<int>() {0}, 16000, -1),
                new Clothes(122, new List<int>() {0}, 16000, -1),
                new Clothes(123, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 55000, -1),
                new Clothes(124, new List<int>() {0,1}, 43000, -1),
                new Clothes(125, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 55000, -1),
                new Clothes(126, new List<int>() {0,1}, 43000, -1),
                new Clothes(127, new List<int>() {0,1,2,3,4,5,6,7}, 49000, -1),
                new Clothes(128, new List<int>() {0,1,2,3,4,5,6,7}, 49000, -1),
                new Clothes(129, new List<int>() {0}, 16000, -1),

                //12
                new Clothes(142, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17}, 220000, -1),
                new Clothes(143, new List<int>() {0,1,2,3,4,5,6,7,8,9}, 260000, -1),
                new Clothes(144, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20}, 310000, -1),
                new Clothes(145, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13}, 245000, -1),
                new Clothes(147, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14}, 230000, -1),
                new Clothes(148, new List<int>() {0,1,2,3,4,5,6,7,8,9}, 280000, -1),
                new Clothes(149, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12}, 245000, -1),
                new Clothes(150, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 280000, -1),
                new Clothes(151, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13}, 230000, -1),
                new Clothes(152, new List<int>() {0,1,2,3,4,6,7}, 310000, -1),
                new Clothes(153, new List<int>() {0}, 315900, -1),
                new Clothes(154, new List<int>() {0,1,2,3,4,5,6,7,8}, 305400, -1),
                new Clothes(155, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14}, 367400, -1),
                new Clothes(156, new List<int>() {0,1,2,3,4,5,6,7}, 214600, -1),
                new Clothes(157, new List<int>() {0,1,2,3}, 285300, -1),
                new Clothes(158, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14}, 196700, -1),
                new Clothes(159, new List<int>() {0,1,2,3,4,5,6}, 497800, -1),
                new Clothes(160, new List<int>() {0,1,2,3,4,5}, 355600, -1),
                new Clothes(161, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13}, 247800, -1),
            }},
        };

        internal static void ApplyCorrectArmor(Player player)
        {
            throw new NotImplementedException();
        }

        public static int newMaleTops = 53;
        public static int newFemaleTops = 33;

        public static Dictionary<bool, List<Clothes>> Tops = new Dictionary<bool, List<Clothes>>()
        {
            { true, new List<Clothes>(){

                new Clothes(3, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 977, 0),
                new Clothes(4, new List<int>() {0,2,3,11,14}, 9775, 0),
                new Clothes(6, new List<int>() {0,1,3,4,5,6,8,9,11}, 2185, 1),
                new Clothes(7, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 805, 0),
                new Clothes(10, new List<int>() {0,1,2}, 4025, 1),
                new Clothes(11, new List<int>() {0,1,7,14}, 6325, 2),
                new Clothes(19, new List<int>() {0,1}, 6900, 1),
                new Clothes(20, new List<int>() {0,1,2,3}, 5405, 1),
                new Clothes(21, new List<int>() {0,1,2,3}, 14950, 2),
                new Clothes(23, new List<int>() {0,1,2,3}, 14950, 0),
                new Clothes(24, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12}, 10925, 1),
                new Clothes(25, new List<int>() {0,1,2,3,4,5,6,7,8,9}, 11500, 2),
                new Clothes(27, new List<int>() {0,1,2}, 1495, 1),
                new Clothes(28, new List<int>() {0,1,2}, 2760, 1),
                new Clothes(29, new List<int>() {0,1,2,3,4,5,6,7}, 6900, 0),
                new Clothes(30, new List<int>() {0,1,2,3,4,5,6,7}, 6900, 1),
                new Clothes(35, new List<int>() {0,1,2,3,4,5,6}, 20700, 0),
                new Clothes(37, new List<int>() {0,1,2}, 1150, 1),
                new Clothes(40, new List<int>() {0,1}, 15525, 2),
                new Clothes(46, new List<int>() {0,1,2}, 13800, 0),
                new Clothes(49, new List<int>() {0,1,2,3,4}, 20700, -1),
                new Clothes(50, new List<int>() {0,1,2,3,4}, 9200, -1),
                new Clothes(51, new List<int>() {0,1,2}, 3680, -1),
                new Clothes(52, new List<int>() {0,1,2,3}, 3680, -1),
                new Clothes(53, new List<int>() {0,1,2,3}, 9200, -1),
                new Clothes(54, new List<int>() {0}, 17250, -1),
                new Clothes(57, new List<int>() {0}, 575, -1),
                new Clothes(58, new List<int>() {0}, 5175, 0),
                new Clothes(59, new List<int>() {0,1,2,3}, 1495, 0),
                new Clothes(61, new List<int>() {0,1,2,3}, 4025, -1),
                new Clothes(62, new List<int>() {0}, 5750, 0),
                new Clothes(63, new List<int>() {0}, 460, -1),
                new Clothes(64, new List<int>() {0}, 2070, 1),
                new Clothes(68, new List<int>() {0,1,2,3,4,5}, 920, -1),
                new Clothes(69, new List<int>() {0,1,2,3,4,5}, 920, -1),
                new Clothes(70, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 14950, 0),
                new Clothes(72, new List<int>() {0,1,2,3}, 14375, 1),
                new Clothes(74, new List<int>() {0,1,2,3,4,5,6,7,8,9,10}, 48875, 0),
                new Clothes(75, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 48875, -1),
                new Clothes(76, new List<int>() {0,1,2,3,4}, 11500, 1),
                new Clothes(77, new List<int>() {0,1,2,3}, 10350, 0),
                new Clothes(78, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 19550, -1),
                new Clothes(79, new List<int>() {0}, 13800, -1),
                new Clothes(80, new List<int>() {0,1,2}, 862, -1),
                new Clothes(81, new List<int>() {0,1,2}, 862, -1),
                new Clothes(82, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 1035, -1),
                new Clothes(83, new List<int>() {0,1,2,3,4}, 1092, -1),
                new Clothes(84, new List<int>() {0,1,2,3,4,5}, 1150, -1),
                new Clothes(85, new List<int>() {0}, 805, -1),
                new Clothes(86, new List<int>() {0,1,2,3,4}, 1725, -1),
                new Clothes(87, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 6325, -1),
                new Clothes(88, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 6325, 0),
                new Clothes(89, new List<int>() {0,1,2,3}, 1150, -1),
                new Clothes(90, new List<int>() {0}, 1725, -1),
                new Clothes(92, new List<int>() {0,1,2,3,4,5,6}, 9200, -1),
                new Clothes(93, new List<int>() {0,1,2}, 920, -1),
                new Clothes(94, new List<int>() {0,1,2}, 920, -1),
                new Clothes(95, new List<int>() {0,1,2}, 805, -1),
                new Clothes(96, new List<int>() {0}, 862, -1),
                new Clothes(98, new List<int>() {0,1}, 6900, -1),
                new Clothes(99, new List<int>() {0,1,2,3,4}, 7475, 0),
                new Clothes(105, new List<int>() {0}, 2875, -1),
                new Clothes(106, new List<int>() {0}, 4025, 0),
                new Clothes(107, new List<int>() {0,1,2,3,4}, 14950, -1),
                new Clothes(108, new List<int>() {0,1,2,3,4,5,6,7,8,9,10}, 28750, 1),
                new Clothes(109, new List<int>() {0}, 2300, -1),
                new Clothes(110, new List<int>() {0}, 1495, -1),
                new Clothes(111, new List<int>() {0,1,2,3,4,5}, 5520, -1),
                new Clothes(112, new List<int>() {0}, 4600, 1),
                new Clothes(113, new List<int>() {0,1,2,3}, 575, -1),
                new Clothes(115, new List<int>() {0}, 2875, 0),
                new Clothes(117, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 9775, -1),
                new Clothes(118, new List<int>() {0,1,2,3,4,5,6,7,8,9}, 3450, 0),
                new Clothes(119, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 3335, 1),
                new Clothes(121, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 1495, -1),
                new Clothes(122, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13}, 7820, 0),
                new Clothes(123, new List<int>() {0,1,2}, 1725, -1),
                new Clothes(124, new List<int>() {0}, 805, 1),
                new Clothes(125, new List<int>() {0}, 1495, -1),
                new Clothes(126, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14}, 1380, -1),
                new Clothes(127, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14}, 1380, 0),
                new Clothes(128, new List<int>() {0,1,2,3,4,5,6,7,8,9}, 1495, -1),
                new Clothes(129, new List<int>() {0}, 1725, -1),
                new Clothes(130, new List<int>() {0}, 1725, 0),
                new Clothes(131, new List<int>() {0}, 920, -1),
                new Clothes(132, new List<int>() {0}, 920, -1),
                new Clothes(133, new List<int>() {0}, 2070, -1),
                new Clothes(134, new List<int>() {0,1,2}, 2070, -1),
                new Clothes(135, new List<int>() {0,1,2,3,4,5,6}, 4600, -1),
                new Clothes(136, new List<int>() {0,1,2,3,4,5,6}, 4025, 0),
                new Clothes(138, new List<int>() {0,1,2}, 1380, -1),
                new Clothes(140, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14}, 8050, 1),
                new Clothes(141, new List<int>() {0,1,2,3,4,5,6,7,8,9,10}, 690, -1),
                new Clothes(142, new List<int>() {0,1,2}, 5750, 0),
                new Clothes(143, new List<int>() {0,1,2,3,4,5,6,7,8,9}, 4025, -1),
                new Clothes(144, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13}, 7475, -1),
                new Clothes(145, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13}, 34500, 1),
                new Clothes(147, new List<int>() {0,1,2,3,4,5,6,7,8,9}, 2875, -1),
                new Clothes(148, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 6325, -1),
                new Clothes(150, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 1150, -1),
                new Clothes(151, new List<int>() {0,1,2,3,4,5}, 1035, 0),
                new Clothes(153, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 2645, -1),
                new Clothes(154, new List<int>() {0,1,2,3,4,5,6,7}, 2300, -1),
                new Clothes(156, new List<int>() {0,1,2,3,4,5}, 1380, 0),
                new Clothes(157, new List<int>() {0,1,2,3}, 3220, -1),
                new Clothes(160, new List<int>() {0,1}, 1380, -1),
                new Clothes(161, new List<int>() {0,1,2,3}, 1725, -1),
                new Clothes(162, new List<int>() {0,1,2,3}, 1725, -1),
                new Clothes(163, new List<int>() {0}, 3105, 0),
                new Clothes(164, new List<int>() {0,1,2}, 2300, -1),
                new Clothes(165, new List<int>() {0,1,2,3,4,5,6,7}, 6900, -1),
                new Clothes(166, new List<int>() {0,1,2,3,4,5}, 3450, 0),
                new Clothes(167, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 24150, 0),
                new Clothes(169, new List<int>() {0,1,2,3}, 1495, 0),
                new Clothes(170, new List<int>() {0,1,2,3}, 1610, -1),
                new Clothes(171, new List<int>() {0,1}, 1092, -1),
                new Clothes(172, new List<int>() {0,1,2,3}, 2300, 0),
                new Clothes(173, new List<int>() {0,1,2,3}, 2070, -1),
                new Clothes(174, new List<int>() {0,1,2,3}, 2070, -1),
                new Clothes(175, new List<int>() {0,1,2,3}, 2185, -1),
                new Clothes(177, new List<int>() {0,1,2,3,4,5,6}, 5175, -1),
                new Clothes(179, new List<int>() {0,1,2,3}, 3680, -1),
                new Clothes(181, new List<int>() {0,1,2,3,4,5}, 3220, 0),
                new Clothes(183, new List<int>() {0,1,2,3,4,5}, 6900, 1),
                new Clothes(184, new List<int>() {0,1,2,3}, 977, -1),
                new Clothes(185, new List<int>() {0,1,2,3}, 977, 0),
                new Clothes(187, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12}, 3220, -1),
                new Clothes(188, new List<int>() {0,1,2,3,4,5,6,7,8,9,10}, 3220, -1),
                new Clothes(189, new List<int>() {0,1,2,3,4,5,6,7,8,9,10}, 3220, 0),
                new Clothes(190, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 16100, -1),
                new Clothes(191, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 57500, 0),
                new Clothes(192, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 10350, 0),
                new Clothes(193, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 10925, -1),
                new Clothes(194, new List<int>() {0,1,2}, 23000, -1),
                new Clothes(196, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 57500, -1),
                new Clothes(197, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 8050, -1),
                new Clothes(198, new List<int>() {0,1,2,3,4,5,6,7}, 34500, -1),
                new Clothes(199, new List<int>() {0,1,2,3,4,5,6,7}, 8050, -1),
                new Clothes(200, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 23000, -1),
                new Clothes(203, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 23000, -1),
                new Clothes(204, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12}, 3220, -1),
                new Clothes(205, new List<int>() {0,1,2,3,4}, 1495, -1),
                new Clothes(206, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23}, 10350, -1),
                new Clothes(207, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23}, 10350, -1),
                new Clothes(209, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23}, 14950, -1),
                new Clothes(210, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23}, 14950, -1),
                new Clothes(211, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23}, 14950, -1),
                new Clothes(212, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23}, 16100, 0),
                new Clothes(213, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23}, 10350, -1),
                new Clothes(214, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23}, 14950, -1),
                new Clothes(215, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23}, 17250, 0),
                new Clothes(216, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23}, 19550, -1),
                new Clothes(217, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14}, 4025, -1),
                new Clothes(218, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14}, 4025, -1),
                new Clothes(219, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 5175, -1),
                new Clothes(223, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 16675, -1),
                new Clothes(224, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 17250, -1),
                new Clothes(225, new List<int>() {0,1}, 1495, -1),
                new Clothes(227, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13}, 18975, -1),
                new Clothes(229, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 9775, -1),
                new Clothes(230, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 9775, 0),
                new Clothes(232, new List<int>() {0,1,2,3,4,5,6,7,8,9}, 5520, 1),
                new Clothes(233, new List<int>() {0,1,2,3,4,5,6,7,8,9}, 5750, 0),
                new Clothes(234, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 13800, -1),
                new Clothes(243, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 13225, -1),
                new Clothes(244, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 6325, -1),
                new Clothes(245, new List<int>() {0,1,2,3,4,5,6,7,8,9}, 11500, -1),
                new Clothes(248, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 7475, -1),
                new Clothes(249, new List<int>() {0,1}, 1092, -1),
                new Clothes(250, new List<int>() {0,1}, 2070, -1),
                new Clothes(251, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 14950, -1),
                new Clothes(253, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 14950, -1),
                new Clothes(254, new List<int>() {0,1,2,3,4,5,6}, 6900, -1),
                new Clothes(255, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 15525, -1),
                new Clothes(256, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 32200, -1),
                new Clothes(257, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23}, 14375, -1),
                new Clothes(258, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14}, 9200, -1),
                new Clothes(259, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 13225, -1),
                new Clothes(260, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 20700, -1),
                new Clothes(261, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 32200, 0),
                new Clothes(262, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 17480, -1),
                new Clothes(263, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 17480, -1),
                new Clothes(264, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 13225, -1),
                new Clothes(265, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17}, 34500, -1),
                new Clothes(266, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17}, 34500, 0),
                new Clothes(267, new List<int>() {0,1,2,3,4}, 7820, 1),
                new Clothes(268, new List<int>() {0,1,2,3,4}, 7820, 0),
                new Clothes(269, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 74750, 0),
                new Clothes(270, new List<int>() {0,1}, 6325, -1),
                new Clothes(272, new List<int>() {0}, 1725, -1),
                new Clothes(274, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19}, 138000, -1),
                new Clothes(279, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17}, 8625, -1),
                new Clothes(280, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17}, 8625, -1),
                new Clothes(281, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23}, 10925, -1),
                new Clothes(282, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 862, -1),
                new Clothes(288, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13}, 11500, -1),
                new Clothes(290, new List<int>() {0}, 12075, 2),
                new Clothes(292, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 74750, 0),
                new Clothes(293, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 74750, 1),
                new Clothes(294, new List<int>() {0,1,2,3,4,5,6,7,8,9}, 23000, 0),
                new Clothes(295, new List<int>() {0,1,2,3,4,5,6,7,8,9}, 23000, 1),
                new Clothes(296, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 32200, -1),
                new Clothes(297, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 32200, -1),
                new Clothes(298, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24}, 48300, -1),
                new Clothes(299, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 20125, -1),
                new Clothes(300, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 63250, -1),
                new Clothes(301, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 63250, -1),
                new Clothes(302, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 63250, -1),
                new Clothes(303, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 66125, 0),
                new Clothes(304, new List<int>() {0,1,2,3,4,5,6,7,8,9}, 80500, 0),
                new Clothes(305, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 28750, -1),
                new Clothes(306, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 28750, -1),
                new Clothes(307, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23}, 49450, -1),
                new Clothes(308, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 24150, -1),
                new Clothes(309, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13}, 80500, 0),
                new Clothes(310, new List<int>() {0,1,2,3,4,5,6,7,8,9,10}, 40250, -1),
                new Clothes(311, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 69000, 0),
                new Clothes(312, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 69000, -1),
                new Clothes(313, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20}, 11500, 1),
                new Clothes(316, new List<int>() {0,1,2,3,4,5,6,7,8}, 12650, -1),
                new Clothes(317, new List<int>() {0,1,2,3,4,5,6,7,8}, 12650, -1),
                new Clothes(318, new List<int>() {0,1,2,3,4,5,6,7,8}, 10350, -1),
                new Clothes(319, new List<int>() {0,1,2,3,4,5,6,7,8}, 10350, -1),
                new Clothes(323, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 2875, -1),
                new Clothes(324, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19}, 23000, -1),
                new Clothes(325, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23}, 6900, -1),
                new Clothes(326, new List<int>() {0}, 4025, -1),
                new Clothes(328, new List<int>() {0}, 7475, -1),
                new Clothes(329, new List<int>() {0}, 1725, -1),
                new Clothes(330, new List<int>() {0}, 2300, -1),
                new Clothes(331, new List<int>() {0}, 2300, -1),
                new Clothes(332, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 16675, -1),
                new Clothes(334, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 2530, -1),
                new Clothes(335, new List<int>() {0,1,2,3,4,5}, 3105, -1),
                new Clothes(336, new List<int>() {0,1,2,3,4,5,6,7,8}, 8280, -1),
                new Clothes(337, new List<int>() {0,1,2,3,4,5,6,7,8}, 8280, -1),
                new Clothes(338, new List<int>() {0,1,2,3,4,5}, 23000, 0),
                new Clothes(339, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14}, 1380, 0),
                new Clothes(339, new List<int>() {15,16,17,18,19,20,21,22,23}, 12650, 0),
                new Clothes(340, new List<int>() {0,1,2,3,4,5,6,7,8}, 11500, 0),
                new Clothes(341, new List<int>() {0,1,2,3,4,5,6,7,8}, 12650, -1),
                new Clothes(342, new List<int>() {0,1,2,3,4,5,6}, 6325, -1),
                new Clothes(343, new List<int>() {0,1,2,3,4,5,6}, 6325, -1),
                new Clothes(344, new List<int>() {0,1,2,3,4,5,6}, 6325, 0),
                new Clothes(345, new List<int>() {0,1,2,3,4,5,6,7,8,9,10}, 5175, -1),
                new Clothes(346, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 16330, -1),
                new Clothes(347, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 20700, -1),
                new Clothes(348, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19}, 25300, -1),
                new Clothes(349, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19}, 25300, -1),
                new Clothes(350, new List<int>() {0,1,2,3,4,5,6,7,8,9}, 6900, -1),
                new Clothes(352, new List<int>() {0,1,2}, 4600, -1),
                new Clothes(353, new List<int>() {0,1,2,3,4}, 4600, -1),
                new Clothes(354, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 13800, -1),
                new Clothes(355, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 13800, -1),
                new Clothes(356, new List<int>() {0}, 1150, -1),
                new Clothes(357, new List<int>() {0,1}, 1380, -1),
                new Clothes(358, new List<int>() {0,1,2,3,4,5,6,7,8,9}, 28750, -1),
                new Clothes(359, new List<int>() {0}, 1150, -1),
                new Clothes(360, new List<int>() {0}, 1150, 0),
                new Clothes(361, new List<int>() {0}, 1150, -1),
                new Clothes(362, new List<int>() {0}, 28750, 0),
                new Clothes(364, new List<int>() {0,1}, 1725, -1),
                new Clothes(369, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 74750, -1),
                new Clothes(370, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 77625, -1),
                new Clothes(371, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 20700, -1),
                new Clothes(372, new List<int>() {0,1}, 126500, -1),
                new Clothes(373, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20}, 23000, -1),
                new Clothes(374, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20}, 23000, -1),
                new Clothes(375, new List<int>() {0,1,2}, 5175, -1),
                new Clothes(376, new List<int>() {0,1,2}, 5175, 0),
                new Clothes(377, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20}, 8625, -1),
                new Clothes(378, new List<int>() {0,1,2,3,4,5,6,7,8,9}, 8625, -1),
                new Clothes(379, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 9200, -1),
                new Clothes(380, new List<int>() {6,7,0,1,2,3,4,5}, 34500, -1),
                new Clothes(381, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 9200, 0),
                new Clothes(382, new List<int>() {0,1,2,3,4,5,6,7}, 3680, -1),
                new Clothes(386, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13}, 7475, -1),
                new Clothes(387, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13}, 7475, 0),
                new Clothes(388, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 37375, -1),
                new Clothes(389, new List<int>() {0,1,2}, 10925, -1),
                new Clothes(390, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 37375, 0),
                new Clothes(391, new List<int>() {0,1,2}, 10925, 0),
                new Clothes(392, new List<int>() {0,1,2,3,4}, 4000, -1),
                new Clothes(393, new List<int>() {0}, 24000, -1),
                new Clothes(394, new List<int>() {0}, 24000, -1),
                new Clothes(395, new List<int>() {0}, 28000, -1),
                new Clothes(396, new List<int>() {0}, 28000, -1),
                new Clothes(398, new List<int>() {0}, 26000, -1),
                new Clothes(399, new List<int>() {0}, 26000, -1),
                new Clothes(400, new List<int>() {0}, 3500, -1),
                new Clothes(401, new List<int>() {0}, 3500, -1),
                new Clothes(402, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13}, 17000, -1),
                new Clothes(403, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13}, 17000, -1),
                new Clothes(404, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 7500, -1),
                new Clothes(405, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19}, 31000, -1),
                new Clothes(406, new List<int>() {0}, 8500, -1),
                new Clothes(407, new List<int>() {0}, 11500, -1),
                new Clothes(408, new List<int>() {0,1,2}, 12500, -1),
                new Clothes(409, new List<int>() {0,1,2}, 12500, -1),
                new Clothes(410, new List<int>() {0}, 6500, -1),
                new Clothes(411, new List<int>() {0}, 29000, -1),
                new Clothes(412, new List<int>() {0}, 29000, -1),
                new Clothes(413, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 10500, -1),
                new Clothes(414, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 13000, 0),
                new Clothes(415, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13}, 25000, -1),
                new Clothes(416, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13}, 25000, -1),
                new Clothes(417, new List<int>() {0,1}, 11200, -1),
                new Clothes(418, new List<int>() {0,1}, 11200, -1),
                new Clothes(419, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 7500, -1),
                new Clothes(420, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19}, 31000, -1),
                new Clothes(421, new List<int>() {0}, 43000, 1),
                new Clothes(422, new List<int>() {0}, 43000, 0),
                new Clothes(423, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17}, 23000, -1),
                new Clothes(424, new List<int>() {0}, 10000, -1),
                new Clothes(425, new List<int>() {0}, 6700, -1),
                new Clothes(426, new List<int>() {0}, 17000, -1),
                new Clothes(427, new List<int>() {0}, 17000, -1),
                new Clothes(428, new List<int>() {0}, 9000, -1),
                new Clothes(429, new List<int>() {0}, 15000, -1),
                new Clothes(430, new List<int>() {0}, 3200, -1),
                new Clothes(431, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 6800, -1),
                new Clothes(432, new List<int>() {0}, 31000, -1),
                new Clothes(433, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 53000, -1),
                new Clothes(434, new List<int>() {0,1,2}, 32000, -1),
                new Clothes(435, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 47000, -1),
                new Clothes(436, new List<int>() {0,1,2}, 27000, -1),
                new Clothes(437, new List<int>() {0,1}, 19000, -1),
                new Clothes(438, new List<int>() {0,1}, 24000, -1),
                new Clothes(439, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 51000, -1),
                new Clothes(440, new List<int>() {0}, 29000, -1),
                new Clothes(441, new List<int>() {0,1}, 21000, -1),

                // 442 -> 494 Обновление 13.06
                new Clothes(495, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13}, 415000, -1),
                new Clothes(496, new List<int>() {0,1,2,3,4,5,6,7,8}, 550000, -1),
                new Clothes(497, new List<int>() {0}, 470000, -1),
                new Clothes(498, new List<int>() {0,1,2,3,4,5,6}, 530000, -1),
                new Clothes(499, new List<int>() {0,1,2,3}, 345000, -1),
                new Clothes(500, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12}, 460000, -1),
                new Clothes(501, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13}, 615000, -1),
                new Clothes(502, new List<int>() {0,1,2}, 369442, -1),
                new Clothes(503, new List<int>() {0,1}, 536185, -1),
                new Clothes(504, new List<int>() {0,1}, 444861, -1),
                new Clothes(505, new List<int>() {0}, 318995, -1),
                new Clothes(506, new List<int>() {0,1}, 287468, -1),
                new Clothes(507, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14}, 723465, -1),
                new Clothes(508, new List<int>() {0}, 264810, -1),
                new Clothes(509, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13}, 734500, -1),
                new Clothes(510, new List<int>() {0,1,2}, 214300, -1),
                new Clothes(511, new List<int>() {0,1,2}, 247900, -1),
                new Clothes(512, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 137900, -1),

            }},
            { false, new List<Clothes>(){
                new Clothes(1, new List<int>() {0,1,2,4,5,6,9,11,14}, 7820, -1),
                new Clothes(2, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 4255, -1),
                new Clothes(3, new List<int>() {0,1,2,3,4}, 805, -1),
                new Clothes(3, new List<int>() {14,11,12,13,10}, 9660, -1),
                new Clothes(6, new List<int>() {0,1,2,4}, 15180, 0),
                new Clothes(7, new List<int>() {0,1,2,8}, 17020, 0),
                new Clothes(8, new List<int>() {0,1,2,12}, 8280, -1),
                new Clothes(9, new List<int>() {0,1,2,3,4,5,6,7,8,10,12,13}, 575, -1),
                new Clothes(9, new List<int>() {14,11,9}, 9890, -1),
                new Clothes(17, new List<int>() {0}, 9890, -1),
                new Clothes(20, new List<int>() {0,1}, 6900, 0),
                new Clothes(21, new List<int>() {0,1,2,3,4,5}, 6325, -1),
                new Clothes(24, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 32775, 0),
                new Clothes(31, new List<int>() {0,1,2,3,4,5,6}, 14375, -1),
                new Clothes(34, new List<int>() {0}, 20700, 0),
                new Clothes(37, new List<int>() {0,1,2,3,4,5}, 11730, -1),
                new Clothes(39, new List<int>() {0}, 18975, -1),
                new Clothes(42, new List<int>() {0,1,2,3,4}, 20700, -1),
                new Clothes(43, new List<int>() {0,1,2,3,4}, 9200, -1),
                new Clothes(44, new List<int>() {0,1,2}, 3680, -1),
                new Clothes(45, new List<int>() {0,1,2,3}, 3680, -1),
                new Clothes(46, new List<int>() {0,1,2,3}, 9200, -1),
                new Clothes(47, new List<int>() {0}, 17250, -1),
                new Clothes(50, new List<int>() {0}, 575, -1),
                new Clothes(51, new List<int>() {0}, 5175, 0),
                new Clothes(54, new List<int>() {0,1,2,3}, 4025, -1),
                new Clothes(56, new List<int>() {0}, 460, -1),
                new Clothes(62, new List<int>() {0,1,2,3,4,5}, 920, -1),
                new Clothes(63, new List<int>() {0,1,2,3,4,5}, 920, -1),
                new Clothes(64, new List<int>() {0,1,2,3,4}, 10350, 0),
                new Clothes(65, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 14950, -1),
                new Clothes(69, new List<int>() {0}, 12650, -1),
                new Clothes(70, new List<int>() {0,1,2,3,4}, 11500, -1),
                new Clothes(71, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 19550, -1),
                new Clothes(72, new List<int>() {0}, 13800, -1),
                new Clothes(76, new List<int>() {0,1,2,3,4}, 1093, -1),
                new Clothes(77, new List<int>() {0}, 805, -1),
                new Clothes(78, new List<int>() {0,1,2,3,4,5,6,7}, 1725, -1),
                new Clothes(79, new List<int>() {0,1,2,3}, 1150, -1),
                new Clothes(80, new List<int>() {0}, 1725, -1),
                new Clothes(81, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 6325, -1),
                new Clothes(83, new List<int>() {0,1,2,3,4,5,6}, 9200, -1),
                new Clothes(84, new List<int>() {0,1,2}, 920, -1),
                new Clothes(85, new List<int>() {0,1,2}, 920, -1),
                new Clothes(86, new List<int>() {0,1}, 805, -1),
                new Clothes(86, new List<int>() {2}, 9660, -1),
                new Clothes(87, new List<int>() {0}, 863, -1),
                new Clothes(89, new List<int>() {0,1}, 6900, -1),
                new Clothes(90, new List<int>() {0,1,2,3,4}, 7475, 0),
                new Clothes(96, new List<int>() {0}, 2875, -1),
                new Clothes(97, new List<int>() {0}, 4025, 0),
                new Clothes(98, new List<int>() {0,1,2,3,4}, 14950, -1),
                new Clothes(99, new List<int>() {0,1,2,3,4,5,6,7,8,9,10}, 28750, 1),
                new Clothes(100, new List<int>() {0}, 2300, -1),
                new Clothes(102, new List<int>() {0}, 1495, -1),
                new Clothes(103, new List<int>() {0,1,2,3,4,5}, 5520, -1),
                new Clothes(104, new List<int>() {0}, 4600, 1),
                new Clothes(106, new List<int>() {0,1,2,3}, 575, -1),
                new Clothes(107, new List<int>() {0}, 2875, 0),
                new Clothes(109, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 9775, -1),
                new Clothes(110, new List<int>() {0,1,2,3,4,5,6,7,8,9}, 3450, -1),
                new Clothes(112, new List<int>() {0,1,2}, 8625, -1),
                new Clothes(113, new List<int>() {0,1,2}, 9775, -1),
                new Clothes(114, new List<int>() {0,1,2}, 8970, -1),
                new Clothes(115, new List<int>() {0,1,2}, 9200, -1),
                new Clothes(116, new List<int>() {0,1,2}, 10925, -1),
                new Clothes(119, new List<int>() {0,1,2}, 1725, -1),
                new Clothes(120, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16}, 1380, 0),
                new Clothes(121, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16}, 1380, -1),
                new Clothes(122, new List<int>() {0}, 1495, -1),
                new Clothes(123, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 1495, -1),
                new Clothes(125, new List<int>() {0,1,2,3,4,5,6,7,8,9}, 1495, -1),
                new Clothes(126, new List<int>() {0,1,2}, 863, -1),
                new Clothes(127, new List<int>() {0}, 1725, -1),
                new Clothes(128, new List<int>() {0}, 920, -1),
                new Clothes(129, new List<int>() {0}, 920, -1),
                new Clothes(131, new List<int>() {0,1,2}, 2070, -1),
                new Clothes(132, new List<int>() {0,1,2,3,4,5,6}, 4600, -1),
                new Clothes(133, new List<int>() {0,1,2,3,4,5,6}, 4025, 0),
                new Clothes(135, new List<int>() {0,1,2}, 1380, -1),
                new Clothes(137, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14}, 8050, 1),
                new Clothes(138, new List<int>() {0,1,2,3,4,5,6,7,8,9,10}, 690, -1),
                new Clothes(139, new List<int>() {0,1,2}, 5750, 0),
                new Clothes(140, new List<int>() {0,1,2,3,4,5,6,7,8,9}, 4025, -1),
                new Clothes(142, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13}, 4600, -1),
                new Clothes(143, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13}, 34500, 1),
                new Clothes(144, new List<int>() {0,1,2,3,4,5,6,7,8,9}, 2875, -1),
                new Clothes(145, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 6325, -1),
                new Clothes(147, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 1150, -1),
                new Clothes(148, new List<int>() {0,1,2,3,4,5}, 1035, 0),
                new Clothes(150, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 2645, -1),
                new Clothes(151, new List<int>() {0,1,2,3,4,5,6,75}, 2300, -1),
                new Clothes(153, new List<int>() {0,1,2,3,4,5}, 1380, 0),
                new Clothes(154, new List<int>() {0,1,2,3}, 3220, 0),
                new Clothes(157, new List<int>() {0,1}, 1380, 0),
                new Clothes(158, new List<int>() {0,1,2,3}, 1725, -1),
                new Clothes(159, new List<int>() {0,1,2,3}, 1725, -1),
                new Clothes(160, new List<int>() {0}, 3105, 0),
                new Clothes(161, new List<int>() {0,1,2}, 2300, 0),
                new Clothes(162, new List<int>() {0,1,2,3,4,5,6}, 6900, -1),
                new Clothes(163, new List<int>() {0,1,2,3,4,5}, 3450, -1),
                new Clothes(164, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 24150, 0),
                new Clothes(166, new List<int>() {0,1,2,3}, 1495, 0),
                new Clothes(167, new List<int>() {0,1,2,3}, 1610, 0),
                new Clothes(172, new List<int>() {0,1}, 1093, -1),
                new Clothes(174, new List<int>() {0,1,2,3}, 2300, 0),
                new Clothes(175, new List<int>() {0,1,2,3}, 2070, 0),
                new Clothes(176, new List<int>() {0,1,2,3}, 2070, -1),
                new Clothes(177, new List<int>() {0,1,2,3}, 2185, -1),
                new Clothes(179, new List<int>() {0,1,2,3,4,5,6}, 5175, -1),
                new Clothes(181, new List<int>() {0,1,2,3}, 3680, 0),
                new Clothes(183, new List<int>() {0,1,2,3,4,5}, 3220, 0),
                new Clothes(185, new List<int>() {0,1,2,3,4,5}, 6900, 1),
                new Clothes(186, new List<int>() {0,1,2,3}, 978, -1),
                new Clothes(187, new List<int>() {0,1,2,3}, 978, 0),
                new Clothes(189, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12}, 3220, -1),
                new Clothes(190, new List<int>() {0,1,2,3,4,5,6,7,8,9,10}, 3220, -1),
                new Clothes(191, new List<int>() {0,1,2,3,4,5,6,7,8,9,10}, 3220, 0),
                new Clothes(192, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 16100, -1),
                new Clothes(193, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 57500, 0),
                new Clothes(194, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 10350, 0),
                new Clothes(195, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 20125, -1),
                new Clothes(196, new List<int>() {0,1,2}, 23000, -1),
                new Clothes(198, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 57500, -1),
                new Clothes(199, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 8050, -1),
                new Clothes(200, new List<int>() {0,1,2,3,4,5,6,7}, 34500, -1),
                new Clothes(201, new List<int>() {0,1,2,3,4,5,6,7}, 8050, -1),
                new Clothes(202, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 23000, -1),
                new Clothes(205, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 23000, -1),
                new Clothes(206, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12}, 3220, -1),
                new Clothes(207, new List<int>() {0,1,2,3,4}, 1495, -1),
                new Clothes(210, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23}, 10350, -1),
                new Clothes(211, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23}, 10350, -1),
                new Clothes(213, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23}, 14950, -1),
                new Clothes(214, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23}, 14950, -1),
                new Clothes(215, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23}, 14950, -1),
                new Clothes(216, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23}, 16100, 0),
                new Clothes(217, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23}, 10350, -1),
                new Clothes(218, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23}, 14950, -1),
                new Clothes(219, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23}, 17250, 0),
                new Clothes(220, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23}, 19550, 0),
                new Clothes(227, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14}, 4025, -1),
                new Clothes(228, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14}, 4025, -1),
                new Clothes(229, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 5175, -1),
                new Clothes(233, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 16675, -1),
                new Clothes(234, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 17250, -1),
                new Clothes(235, new List<int>() {0,1}, 1495, -1),
                new Clothes(237, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13}, 18975, -1),
                new Clothes(239, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 9775, -1),
                new Clothes(240, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 9775, 0),
                new Clothes(242, new List<int>() {0,1,2,3,4,5,6,7,8,9}, 5520, 1),
                new Clothes(243, new List<int>() {0,1,2,3,4,5,6,7,8,9}, 5750, 0),
                new Clothes(244, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 13800, -1),
                new Clothes(251, new List<int>() {0,1,2,3,4,5,6,7,8,10,11,12,13,14,15,17,18,19,22,23,24,25}, 13225, -1),
                new Clothes(251, new List<int>() {9,16,19,20,21}, 18975, -1),
                new Clothes(252, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 6325, -1),
                new Clothes(253, new List<int>() {0,1,2,3,4,5,6,7,8,9}, 11500, -1),
                new Clothes(256, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 7475, -1),
                new Clothes(257, new List<int>() {0,1}, 1093, -1),
                new Clothes(258, new List<int>() {0,1}, 2070, -1),
                new Clothes(259, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 14950, -1),
                new Clothes(261, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 14950, -1),
                new Clothes(263, new List<int>() {0,1,2,3,4,5,6}, 6900, -1),
                new Clothes(264, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 15525, -1),
                new Clothes(265, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 32200, -1),
                new Clothes(266, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23}, 14375, -1),
                new Clothes(267, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14}, 9200, -1),
                new Clothes(268, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 13225, -1),
                new Clothes(269, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 20700, -1),
                new Clothes(270, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 32200, 0),
                new Clothes(271, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 15525, -1),
                new Clothes(272, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 15525, -1),
                new Clothes(274, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17}, 34500, -1),
                new Clothes(275, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17}, 34500, 0),
                new Clothes(276, new List<int>() {0,1,2,3,4}, 7820, 1),
                new Clothes(277, new List<int>() {0,1,2,3,4}, 7820, 0),
                new Clothes(278, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 74750, 0),
                new Clothes(282, new List<int>() {0,1}, 6325, -1),
                new Clothes(285, new List<int>() {0}, 1725, -1),
                new Clothes(287, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19}, 138000, -1),
                new Clothes(292, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17}, 8625, -1),
                new Clothes(293, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17}, 8625, -1),
                new Clothes(294, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23}, 10925, -1),
                new Clothes(295, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 863, -1),
                new Clothes(301, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13}, 11500, -1),
                new Clothes(303, new List<int>() {0}, 12075, 2),
                new Clothes(305, new List<int>() {0,1,2,3,4,5,6,7,8,9}, 23000, 0),
                new Clothes(306, new List<int>() {0,1,2,3,4,5,6,7,8,9}, 23000, 1),
                new Clothes(307, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 32200, -1),
                new Clothes(308, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 32200, -1),
                new Clothes(309, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24}, 48300, -1),
                new Clothes(310, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 20125, -1),
                new Clothes(311, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 63250, -1),
                new Clothes(312, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 63250, -1),
                new Clothes(313, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 63250, -1),
                new Clothes(314, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 66125, 0),
                new Clothes(315, new List<int>() {0,1,2,3,4,5,6,7,8,9}, 80500, -1),
                new Clothes(316, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 28750, -1),
                new Clothes(317, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 28750, -1),
                new Clothes(318, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23}, 49450, -1),
                new Clothes(319, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 24150, -1),
                new Clothes(320, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13}, 80500, 0),
                new Clothes(321, new List<int>() {0,1,2,3,4,5,6,7,8,9,10}, 40250, -1),
                new Clothes(322, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 42550, -1),
                new Clothes(323, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 40825, -1),
                new Clothes(324, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20}, 11500, -1),
                new Clothes(327, new List<int>() {0,1,2,3,4,5,6,7,8}, 12650, -1),
                new Clothes(328, new List<int>() {0,1,2,3,4,5,6,7,8}, 12650, -1),
                new Clothes(329, new List<int>() {0,1,2,3,4,5,6,7,8}, 10350, -1),
                new Clothes(330, new List<int>() {0,1,2,3,4,5,6,7,8}, 10350, -1),
                new Clothes(334, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24}, 6325, 2),
                new Clothes(335, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 2875, -1),
                new Clothes(336, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19}, 23000, -1),
                new Clothes(337, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17}, 6900, -1),
                new Clothes(339, new List<int>() {0,1,2,3,4,5,6,7}, 6900, 0),
                new Clothes(340, new List<int>() {0,1,2,3,4,5,6,7}, 6900, 1),
                new Clothes(341, new List<int>() {0}, 4025, -1),
                new Clothes(343, new List<int>() {0}, 7475, -1),
                new Clothes(344, new List<int>() {0}, 1725, -1),
                new Clothes(345, new List<int>() {0}, 2300, -1),
                new Clothes(346, new List<int>() {0}, 2300, -1),
                new Clothes(347, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 13800, -1),
                new Clothes(349, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14}, 2530, -1),
                new Clothes(350, new List<int>() {0,1,2,3,4,5}, 3105, -1),
                new Clothes(351, new List<int>() {0,1,2,3,4,5,6,7,8}, 8280, -1),
                new Clothes(352, new List<int>() {0,1,2,3,4,5,6,7,8}, 8280, -1),
                new Clothes(353, new List<int>() {0,1,2,3,4,5}, 23000, 0),
                new Clothes(354, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16}, 1380, 0),
                new Clothes(354, new List<int>() {17,18,19,20,21,22,23,24}, 12650, 0),
                new Clothes(355, new List<int>() {0,1,2,3,4,5,6,7}, 11500, 0),
                new Clothes(356, new List<int>() {0,1,2,3,4,5,6,7}, 12650, -1),
                new Clothes(357, new List<int>() {0,1,2,3,4,5,6}, 3105, -1),
                new Clothes(358, new List<int>() {0,1,2,3,4,5,6}, 3105, -1),
                new Clothes(359, new List<int>() {0,1,2,3,4,5,6}, 3105, -1),
                new Clothes(360, new List<int>() {0,1,2,3,4,5,6}, 3105, -1),
                new Clothes(361, new List<int>() {0,1,2,3,4,5,6}, 6325, -1),
                new Clothes(362, new List<int>() {0,1,2,3,4,5,6}, 6325, -1),
                new Clothes(363, new List<int>() {0,1,2,3,4,5,6}, 6325, 0),
                new Clothes(364, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 13800, 0),
                new Clothes(365, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 20700, 0),
                new Clothes(366, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19}, 25300, -1),
                new Clothes(367, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19}, 25300, -1),
                new Clothes(368, new List<int>() {0,1,2,3,4,5,6,7,8,9}, 6900, -1),
                new Clothes(370, new List<int>() {0,1,2}, 4600, -1),
                new Clothes(371, new List<int>() {0,1,2,3,4}, 4600, -1),
                new Clothes(372, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 13800, -1),
                new Clothes(373, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 13800, 0),
                new Clothes(374, new List<int>() {0}, 1150, -1),
                new Clothes(375, new List<int>() {0,1,2}, 1380, -1),
                new Clothes(376, new List<int>() {0,1,2,3,4,5,6,7,8,9}, 28750, -1),
                new Clothes(378, new List<int>() {0}, 1150, -1),
                new Clothes(379, new List<int>() {0}, 1150, 0),
                new Clothes(380, new List<int>() {0}, 1150, -1),
                new Clothes(381, new List<int>() {0}, 28750, 0),
                new Clothes(384, new List<int>() {0}, 1955, 2),
                new Clothes(385, new List<int>() {0}, 1955, 0),
                new Clothes(386, new List<int>() {0,1,2,3,4,5,6,7}, 2645, 1),
                new Clothes(387, new List<int>() {0,1,2,3,4,5,6,7}, 2875, 1),
                new Clothes(388, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 74750, -1),
                new Clothes(389, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 77625, -1),
                new Clothes(390, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 20700, -1),
                new Clothes(391, new List<int>() {0,1}, 126500, -1),
                new Clothes(392, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20}, 23000, -1),
                new Clothes(393, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20}, 23000, -1),
                new Clothes(394, new List<int>() {0,1,2}, 5175, -1),
                new Clothes(396, new List<int>() {0,1,2,3,4,5,6,7,8,9}, 8625, -1),
                new Clothes(397, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 9200, -1),
                new Clothes(398, new List<int>() {6,7,1,2,3,4,5}, 34500, -1),
                new Clothes(399, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 9200, 0),
                new Clothes(400, new List<int>() {0,1,2,3,4,5,6,7}, 3680, -1),
                new Clothes(402, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13}, 7475, -1),
                new Clothes(403, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13}, 7475, 0),
                new Clothes(404, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 23000, -1),
                new Clothes(405, new List<int>() {0,1,2,3,4,5,6,7,8}, 27025, -1),
                new Clothes(406, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 4600, -1),
                new Clothes(407, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 5175, -1),
                new Clothes(408, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 5175, -1),
                new Clothes(409, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 37375, -1),
                new Clothes(410, new List<int>() {0,1,2}, 10925, -1),
                new Clothes(411, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 37375, 0),
                new Clothes(412, new List<int>() {0,1,2}, 10925, 0),
                new Clothes(413, new List<int>() {0,1,2,3,4,5}, 4000, -1),
                new Clothes(414, new List<int>() {0,1,2,3,4,5}, 4000, -1),
                new Clothes(415, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 7900, -1),
                new Clothes(416, new List<int>() {0}, 26000, -1),
                new Clothes(417, new List<int>() {0}, 26000, -1),
                new Clothes(418, new List<int>() {0}, 24000, -1),
                new Clothes(419, new List<int>() {0}, 24000, -1),
                new Clothes(421, new List<int>() {0}, 28000, -1),
                new Clothes(422, new List<int>() {0}, 28000, -1),
                new Clothes(423, new List<int>() {0}, 6500, -1),
                new Clothes(424, new List<int>() {0}, 25000, -1),
                new Clothes(425, new List<int>() {0}, 25000, -1),
                new Clothes(426, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13}, 17000, -1),
                new Clothes(427, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13}, 17000, -1),
                new Clothes(428, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 7500, -1),
                new Clothes(429, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19}, 31000, -1),
                new Clothes(430, new List<int>() {0}, 7500, -1),
                new Clothes(431, new List<int>() {0}, 7500, -1),
                new Clothes(432, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 8500, -1),
                new Clothes(433, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21}, 34000, -1),
                new Clothes(434, new List<int>() {0}, 11500, -1),
                new Clothes(435, new List<int>() {0,1}, 12500, -1),
                new Clothes(436, new List<int>() {0}, 3500, -1),
                new Clothes(437, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 41000, -1),
                new Clothes(438, new List<int>() {0}, 34000, -1),
                new Clothes(439, new List<int>() {0}, 3500, -1),
                new Clothes(441, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 13000, -1),
                new Clothes(442, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13}, 25000, -1),
                new Clothes(443, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13}, 25000, -1),
                new Clothes(444, new List<int>() {0,1}, 11200, -1),
                new Clothes(445, new List<int>() {0,1}, 11200, -1),
                new Clothes(446, new List<int>() {0,1}, 11200, -1),
                new Clothes(447, new List<int>() {0,1}, 11200, -1),
                new Clothes(448, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 7500, -1),
                new Clothes(449, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19}, 31000, -1),
                new Clothes(450, new List<int>() {0}, 43000, -1),
                new Clothes(451, new List<int>() {0}, 43000, -1),
                new Clothes(452, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17}, 23000, -1),
                new Clothes(453, new List<int>() {0}, 10000, -1),
                new Clothes(454, new List<int>() {0}, 6700, -1),
                new Clothes(455, new List<int>() {0}, 6700, -1),
                new Clothes(456, new List<int>() {0}, 17000, -1),
                new Clothes(457, new List<int>() {0}, 17000, -1),
                new Clothes(458, new List<int>() {0}, 9000, -1),
                new Clothes(459, new List<int>() {0}, 15000, -1),
                new Clothes(460, new List<int>() {0}, 15000, -1),
                new Clothes(461, new List<int>() {0}, 3200, -1),
                new Clothes(462, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 6800, -1),
                new Clothes(463, new List<int>() {0}, 31000, 1),
                new Clothes(464, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 53000, 0),
                new Clothes(465, new List<int>() {0,1,2}, 32000, 0),
                new Clothes(466, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 47000, -1),
                new Clothes(467, new List<int>() {0,1,2}, 27000, -1),
                new Clothes(468, new List<int>() {0,1}, 19000, -1),
                new Clothes(469, new List<int>() {0,1}, 24000, -1),
                new Clothes(470, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 51000, 0),
                new Clothes(471, new List<int>() {0}, 29000, -1),
                new Clothes(472, new List<int>() {0,1}, 21000, -1),

                //533 - 534
                new Clothes(534, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12}, 415000, -1),
                new Clothes(535, new List<int>() {0,1,2,3,4,5,6,7,8}, 450000, -1),
                new Clothes(536, new List<int>() {0,1,2}, 430000, -1),
                new Clothes(537, new List<int>() {0,1,2,3,4,5,6}, 490000, -1),
                new Clothes(538, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12}, 510000, -1),
                new Clothes(539, new List<int>() {0,1,2,3,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21}, 540000, -1),
                new Clothes(540, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20}, 560000, -1),
                new Clothes(541, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 410000, -1),
                new Clothes(542, new List<int>() {0,1,2,3}, 460000, -1),
                new Clothes(543, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19}, 475000, -1),
                new Clothes(544, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19}, 50000, -1),
                new Clothes(545, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14}, 723465, -1),
                new Clothes(546, new List<int>() {0,1}, 536185, -1),
                new Clothes(547, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14}, 476850, -1),
                new Clothes(548, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 515285, -1),
                new Clothes(550, new List<int>() {0,1,2}, 549600, -1),
                new Clothes(551, new List<int>() {0,1,2}, 415000, -1),
                new Clothes(552, new List<int>() {0}, 230000, -1),
                new Clothes(553, new List<int>() {0,1,2}, 46000, -1),
                new Clothes(554, new List<int>() {0,1,2}, 87000, -1),
                new Clothes(555, new List<int>() {0,1,2}, 37000, -1),
                new Clothes(556, new List<int>() {0,1,2,3,5,6,7,8,9,10,11,12,13,14}, 225000, -1),
                new Clothes(557, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19}, 330000, -1),
                new Clothes(558, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20}, 499000, -1),
                new Clothes(559, new List<int>() {0,1,2}, 210000, -1),
                new Clothes(560, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19}, 180000, -1),
                new Clothes(561, new List<int>() {0,1,2,3,4,5,6,7,8,9,10}, 525000, -1),
                new Clothes(562, new List<int>() {0,1,2}, 375000, 0),
                new Clothes(563, new List<int>() {0,1,2,3,4,5,6}, 740000, 0),
                new Clothes(564, new List<int>() {0,1,2,3,4,5,6}, 435000, 0),
                new Clothes(565, new List<int>() {0,1,2,3,4,5,6}, 398000, 0),
                new Clothes(566, new List<int>() {0,1,2}, 549600, 0),
                new Clothes(567, new List<int>() {0,1,2,3}, 540100,  0),
                new Clothes(568, new List<int>() {0,1,2,3,4}, 785000,  0),
                new Clothes(569, new List<int>() {0,1,2,3,4,5,6,7,8,9,10}, 435000,  0),
                new Clothes(570, new List<int>() {0,1,2,3,4,5}, 175400,  0),
                new Clothes(571, new List<int>() {0,1,2,3}, 519600,  0),
                new Clothes(572, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 325400,  0),
                new Clothes(573, new List<int>() {0,1,2,3,4,5,6,7,8,9}, 470300,  0),
                new Clothes(574, new List<int>() {0,1,2,3,4,5,6,7,8,9}, 174200,  0),
                new Clothes(575, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14}, 623000,  0),
                new Clothes(576, new List<int>() {0,1,2}, 264000,  0),
                new Clothes(577, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18}, 369442,  0),
                new Clothes(578, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21}, 134000,  0),
                new Clothes(579, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14}, 310400,  0),
                new Clothes(580, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14}, 239800,  0),
                new Clothes(581, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19}, 278200,  0),
                new Clothes(582, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13}, 241300,  0),
                new Clothes(583, new List<int>() {0,1,2,3,4,5,6,7,8,9}, 605450,  0),
                new Clothes(584, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19}, 780450,  0),
                new Clothes(585, new List<int>() {0,1,2,3,4,5,6,7,8}, 254700,  0),
                new Clothes(586, new List<int>() {0,1,2}, 148900,  0),
                new Clothes(587, new List<int>() {0,1,2}, 148900,  0),
                new Clothes(588, new List<int>() {0,1,2,3,4,5,6,7,8,9,10}, 437900,  0),
                new Clothes(589, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24}, 578410,  0),
                new Clothes(590, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22}, 628700,  0),
                new Clothes(591, new List<int>() {0,1,2,3,4,5,6,7,8,9}, 754900,  0),

            }},
        };

        //Загрузил с index.js global.clothesTops
        public static Dictionary<bool, List<Clothes>> Gloves = new Dictionary<bool, List<Clothes>>()
        {
            { true, new List<Clothes>(){
                new Clothes(4, new List<int>() { 0, 1 }, 4000),
                new Clothes(5, new List<int>() { 0, 1 }, 4200),
                new Clothes(6, new List<int>() { 0, 1 }, 4200),
                new Clothes(7, new List<int>() { 0, 1 }, 4200),
                new Clothes(9, new List<int>() { 0 }, 4200),
                new Clothes(10, new List<int>() { 0, 1 }, 3000),
                new Clothes(13, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, 3000),
            }},
            { false, new List<Clothes>(){
                new Clothes(4, new List<int>() { 0, 1 }, 3000),
                new Clothes(5, new List<int>() { 0, 1 }, 3500),
                new Clothes(6, new List<int>() { 0 }, 3000),
                new Clothes(7, new List<int>() { 0, 1 }, 3000),
                new Clothes(8, new List<int>() { 0 }, 2550),
                new Clothes(9, new List<int>() { 0 }, 2000),
                new Clothes(10, new List<int>() { 0, 1 }, 11000),
                new Clothes(12, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7,8,9 }, 8000),
            }},
        };

        public static Dictionary<bool, List<Clothes>> Accessories = new Dictionary<bool, List<Clothes>>()
        {
            { true, new List<Clothes>(){
                new Clothes(0, new List<int>() {0}, 6110),
                new Clothes(1, new List<int>() {0}, 6370),
                new Clothes(3, new List<int>() {0,1,2,3,4}, 12090),
                new Clothes(4, new List<int>() {0,1,2,3}, 6630),
                new Clothes(5, new List<int>() {0,1,2,3}, 8320),
                new Clothes(6, new List<int>() {0,1,2}, 7670),
                new Clothes(7, new List<int>() {0,1,2}, 8450),
                new Clothes(8, new List<int>() {0,1,2}, 10660),
                new Clothes(9, new List<int>() {0,1,2}, 11050),
                new Clothes(10, new List<int>() {0,1,2}, 10270),
                new Clothes(11, new List<int>() {0,1,2}, 8710),
                new Clothes(12, new List<int>() {0,1,2}, 4810),
                new Clothes(13, new List<int>() {0,1,2}, 9230),
                new Clothes(14, new List<int>() {0,1,2}, 5070),
                new Clothes(15, new List<int>() {0,1,2}, 12610),
                new Clothes(16, new List<int>() {0,1,2}, 13260),
                new Clothes(17, new List<int>() {0,1,2}, 9100),
                new Clothes(18, new List<int>() {0,1,2}, 10530),
                new Clothes(19, new List<int>() {0,1,2}, 14560),
                new Clothes(20, new List<int>() {0,1,2}, 13650),
                new Clothes(21, new List<int>() {0,1,2}, 10140),
                new Clothes(22, new List<int>() {0}, 2210),
                new Clothes(23, new List<int>() {0}, 6370),
                new Clothes(24, new List<int>() {0}, 8710),
                new Clothes(25, new List<int>() {0}, 7150),
                new Clothes(26, new List<int>() {0}, 9490),
                new Clothes(27, new List<int>() {0}, 6890),
                new Clothes(28, new List<int>() {0}, 3770),
                new Clothes(29, new List<int>() {0,1,2,3}, 1560),
                new Clothes(30, new List<int>() {0,1,2,3,4,5}, 18330),
                new Clothes(31, new List<int>() {0,1,2,3,4,5}, 15600),
                new Clothes(32, new List<int>() {0,1,2,3,4,5,6,7,8,9}, 19500),
                new Clothes(33, new List<int>() {0,1,2,3,4,5}, 18850),
                new Clothes(34, new List<int>() {0,1,2,3,4,5}, 16770),
                new Clothes(35, new List<int>() {0,1,2,3,4,5}, 12870),
                new Clothes(36, new List<int>() {0,1,2,3,4,5}, 13910),
                new Clothes(37, new List<int>() {0,1,2,3,4,5}, 14690),
                new Clothes(38, new List<int>() {0,1,2}, 19890),
                new Clothes(39, new List<int>() {0,1,2}, 19890),
                new Clothes(40, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 20410),
                new Clothes(41, new List<int>() {0,1,2}, 17030),
                new Clothes(42, new List<int>() {0,1,2}, 23660),
                new Clothes(43, new List<int>() {0,1,2}, 9880),
                new Clothes(44, new List<int>() {0,1,2}, 23140),

                new Clothes(45, new List<int>() {0,1,2,3,4}, 14650, -1),
                new Clothes(46, new List<int>() {0,1,2,3,4}, 12485, -1),
                new Clothes(47, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 38500, -1),
                new Clothes(48, new List<int>() {0}, 20410, -1),
                new Clothes(49, new List<int>() {0,1,2}, 44000, -1),
                new Clothes(50, new List<int>() {0}, 38450, -1),
                new Clothes(51, new List<int>() {0,1}, 19000, -1),
                new Clothes(52, new List<int>() {0}, 20150, -1),

            }},
            { false, new List<Clothes>(){
                new Clothes(0, new List<int>() {5}, 6110),
                new Clothes(2, new List<int>() {0,1,2,3}, 11570),
                new Clothes(3, new List<int>() {0,1,2}, 7930),
                new Clothes(4, new List<int>() {0,1,2}, 11960),
                new Clothes(5, new List<int>() {0,1,2}, 4810),
                new Clothes(6, new List<int>() {0,1,2}, 9230),
                new Clothes(7, new List<int>() {0,1,2}, 5590),
                new Clothes(8, new List<int>() {0,1,2}, 12740),
                new Clothes(9, new List<int>() {0,1,2}, 11830),
                new Clothes(10, new List<int>() {0,1,2}, 10140),
                new Clothes(11, new List<int>() {0}, 2210),
                new Clothes(12, new List<int>() {0}, 6370),
                new Clothes(13, new List<int>() {0}, 8710),
                new Clothes(14, new List<int>() {0}, 7150),
                new Clothes(15, new List<int>() {0}, 9490),
                new Clothes(16, new List<int>() {0}, 6890),
                new Clothes(17, new List<int>() {0}, 3770),
                new Clothes(18, new List<int>() {0,1,2,3}, 1560),
                new Clothes(19, new List<int>() {0,1,2,3,4,5}, 18330),
                new Clothes(20, new List<int>() {0,1,2,3,4,5}, 15600),
                new Clothes(21, new List<int>() {0,1,2,3,4,5,6,7,8,9}, 19500),
                new Clothes(22, new List<int>() {0,1,2,3,4,5}, 18850),
                new Clothes(23, new List<int>() {0,1,2,3,4,5}, 16770),
                new Clothes(24, new List<int>() {0,1,2,3,4,5}, 12870),
                new Clothes(25, new List<int>() {0,1,2,3,4,5}, 13910),
                new Clothes(26, new List<int>() {0,1,2,3,4,5}, 14690),
                new Clothes(27, new List<int>() {0,1,2}, 19890),
                new Clothes(28, new List<int>() {0,1,2}, 19890),
                new Clothes(29, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 20410),
                new Clothes(30, new List<int>() {0,1,2}, 17030),
                new Clothes(31, new List<int>() {0,1,2}, 23660),
                new Clothes(32, new List<int>() {0,1,2}, 9880),
                new Clothes(33, new List<int>() {0,1,2}, 23140),

                new Clothes(34, new List<int>() {0,1,2,3,4}, 14650, -1),
                new Clothes(35, new List<int>() {0,1,2,3,4}, 12485, -1),
                new Clothes(36, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 38500, -1),
                new Clothes(37, new List<int>() {0}, 20410, -1),
                new Clothes(38, new List<int>() {0,1,2}, 44000, -1),



            }},
        };

        public static int newGlasses = 5;

        public static Dictionary<bool, List<Clothes>> Glasses = new Dictionary<bool, List<Clothes>>()
        {
            { true, new List<Clothes>(){
                new Clothes(1, new List<int>() {2}, 11830),
                new Clothes(2, new List<int>() {0,1,2,3,4,5}, 13390),
                new Clothes(3, new List<int>() {0,1,2,3,4,5,6,7,8,9,10}, 8970),
                new Clothes(4, new List<int>() {0,1,2,3,4,5,6,7,8,9,10}, 10790),
                new Clothes(5, new List<int>() {0,1,2,3,4,5,6,7,8,9,10}, 12350),
                new Clothes(7, new List<int>() {0,1,2,3,4,5,6,7,8,9,10}, 6890),
                new Clothes(8, new List<int>() {0,1,2,3,4,5,6,7,8,9,10}, 6500),
                new Clothes(9, new List<int>() {0,1,2,3,4,5,6,7,8,9,10}, 5980),
                new Clothes(10, new List<int>() {0,1,2,3,4,5,6,7,8,9,10}, 9360),
                new Clothes(12, new List<int>() {0,1,2,3,4,5,6,7,8,9,10}, 5460),
                new Clothes(13, new List<int>() {0,1,2,3,4,5,6,7,8,9,10}, 5850),
                new Clothes(15, new List<int>() {0,1,2,3,4,5,6,7,8,9,10}, 4290),
                new Clothes(16, new List<int>() {0,1,2,3,4,5,6,7,8,9}, 12870),
                new Clothes(17, new List<int>() {0,1,2,3,4,5,6,7,8,9,10}, 7670),
                new Clothes(18, new List<int>() {0,1,2,3,4,5,6,7,8,9,10}, 9360),
                new Clothes(19, new List<int>() {0,1,2,3,4,5,6,7,8,9}, 5200),
                new Clothes(20, new List<int>() {0,1,2,3,4,5,6,7,8,9}, 9880),
                new Clothes(21, new List<int>() {0}, 5460),
                new Clothes(22, new List<int>() {0}, 6110),
                new Clothes(23, new List<int>() {0,1,2,3,4,5,6,7,8,9}, 5070),
                new Clothes(24, new List<int>() {0,1,2,3,4,5}, 10140),
                new Clothes(25, new List<int>() {0,1,2,3,4,5,6,7}, 17290),
                new Clothes(28, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 18720),
                new Clothes(29, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19}, 20800),
                new Clothes(30, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 24050),
                new Clothes(31, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 14820),
                new Clothes(32, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 8710),
                new Clothes(33, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 12610),
                new Clothes(34, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 9750),
                new Clothes(35, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 9100),
                new Clothes(36, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 10920),
                new Clothes(37, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 17030),
                new Clothes(38, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 14300),
                new Clothes(39, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 15860),
                new Clothes(41, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19}, 17680),

                new Clothes(42, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 14512, -1),
                new Clothes(43, new List<int>() {0}, 18455, -1),
                new Clothes(45, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 45780, -1),
                new Clothes(46, new List<int>() {0,1,2,3}, 45780, -1),
                new Clothes(47, new List<int>() {0,1,2,3,4}, 65450, -1),
                new Clothes(48, new List<int>() {0,1,2,3,4,5,6}, 65450, -1),
                new Clothes(49, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 69500, -1),
                new Clothes(50, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 111450, -1),
                new Clothes(51, new List<int>() {0}, 194540, -1),
                new Clothes(52, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 65450, -1),

            }},
            { false, new List<Clothes>(){
                new Clothes(1, new List<int>() {0,1,2,3,4,5,6,7,8,9,10}, 4160),
                new Clothes(2, new List<int>() {0,1,2,3,4,5,6,7,8,9,10}, 5070),
                new Clothes(3, new List<int>() {0,1,2,3,4,5,6,7,8,9,10}, 11570),
                new Clothes(4, new List<int>() {0,1,2,3,4,5,6,7,8,9,10}, 5850),
                new Clothes(6, new List<int>() {0,8,9,10}, 17030),
                new Clothes(7, new List<int>() {0,1,2,3,4,5,6,7,8,9,10}, 17550),
                new Clothes(8, new List<int>() {0,8,9,10}, 3640),
                new Clothes(9, new List<int>() {0,1,2,3,4,5,6,7,8,9,10}, 4290),
                new Clothes(10, new List<int>() {0,1,2,3,4,5,6,7,8,9,10}, 5980),
                new Clothes(11, new List<int>() {0,1,2,3,4,5,6,7}, 6110),
                new Clothes(14, new List<int>() {0,1,2,3,4,5,6,7,8,9,10}, 10660),
                new Clothes(16, new List<int>() {0,1,2,3,4,5,6,7,8,9}, 18590),
                new Clothes(17, new List<int>() {0,1,2,3,4,5,6,7,8,9}, 6630),
                new Clothes(18, new List<int>() {0,1,2,3,4,5,6,7}, 8970),
                new Clothes(19, new List<int>() {0,1,2,3,4,5,6,7,8,9,10}, 9490),
                new Clothes(20, new List<int>() {0,1,2,3,4,5,6,7}, 13130),
                new Clothes(21, new List<int>() {0,1,2,3,4,5,6,7}, 9360),
                new Clothes(22, new List<int>() {0}, 5460),
                new Clothes(23, new List<int>() {0}, 6110),
                new Clothes(24, new List<int>() {0,1,2,3,4,5,6,7,8,9,10}, 6890),
                new Clothes(25, new List<int>() {0,1,2,3,4,5,6,7,8,9}, 5070),
                new Clothes(26, new List<int>() {0,1,2,3,4,5}, 10140),
                new Clothes(27, new List<int>() {0,1,2,3,4,5,6,7}, 17290),
                new Clothes(30, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 18720),
                new Clothes(31, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19}, 20800),
                new Clothes(32, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 24050),
                new Clothes(33, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 14820),
                new Clothes(34, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 8710),
                new Clothes(35, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 12610),
                new Clothes(36, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 9750),
                new Clothes(37, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 9100),
                new Clothes(38, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 10920),
                new Clothes(39, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 17030),
                new Clothes(40, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 14300),
                new Clothes(41, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 15860),
                new Clothes(43, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19}, 17680),

                new Clothes(44, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 14512, -1),
                new Clothes(45, new List<int>() {0}, 18455, -1),
                new Clothes(47, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 45780, -1),
                new Clothes(48, new List<int>() {0,1,2,3}, 45780, -1),
                new Clothes(49, new List<int>() {0,1,2,3,4}, 65450, -1),
                new Clothes(50, new List<int>() {0,1,2,3,4,5,6}, 65450, -1),
                new Clothes(51, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 69500, -1),
                new Clothes(52, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 111450, -1),
                new Clothes(53, new List<int>() {0}, 194540, -1),
                new Clothes(54, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 65450, -1),

            }},
        };

        public static Dictionary<bool, List<Clothes>> Jewerly = new Dictionary<bool, List<Clothes>>()
        {
            { true, new List<Clothes>(){
                new Clothes(10, new List<int>() {0,1,2}, 910),
                new Clothes(11, new List<int>() {2}, 715),
                new Clothes(12, new List<int>() {0,1,2}, 910),
                new Clothes(16, new List<int>() {0,1,2}, 3640),
                new Clothes(17, new List<int>() {0,1,2}, 3900),
                new Clothes(18, new List<int>() {0}, 1235),
                new Clothes(19, new List<int>() {0}, 1235),
                new Clothes(20, new List<int>() {0,1,2,3,4}, 1755),
                new Clothes(21, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12}, 4160),
                new Clothes(22, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14}, 3640),
                new Clothes(23, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12}, 4160),
                new Clothes(24, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 4160),
                new Clothes(25, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 4160),
                new Clothes(26, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 4160),
                new Clothes(27, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 4160),
                new Clothes(28, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 4160),
                new Clothes(29, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 4160),
                new Clothes(30, new List<int>() {0,1,2,3,4,5}, 1560),
                new Clothes(31, new List<int>() {0,1,2,3,4,5}, 1560),
                new Clothes(32, new List<int>() {0,1,2}, 5200),
                new Clothes(34, new List<int>() {0,1,2,3}, 2990),
                new Clothes(35, new List<int>() {0,1,2,3}, 2990),
                new Clothes(36, new List<int>() {0}, 845),
                new Clothes(37, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 8450),
                new Clothes(38, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 8450),
                new Clothes(39, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 8450),
                new Clothes(42, new List<int>() {0,1}, 5850),
                new Clothes(43, new List<int>() {0,1}, 5460),
                new Clothes(44, new List<int>() {0}, 4940),
                new Clothes(45, new List<int>() {0,1}, 5850),
                new Clothes(46, new List<int>() {0,1}, 5850),
                new Clothes(47, new List<int>() {0,1}, 8190),
                new Clothes(48, new List<int>() {0,1}, 6240),
                new Clothes(49, new List<int>() {0,1}, 6760),
                new Clothes(50, new List<int>() {0,1}, 5460),
                new Clothes(51, new List<int>() {0}, 4940),
                new Clothes(52, new List<int>() {0,1}, 5850),
                new Clothes(53, new List<int>() {0,1}, 5850),
                new Clothes(54, new List<int>() {0,1}, 8190),
                new Clothes(55, new List<int>() {0,1}, 6240),
                new Clothes(74, new List<int>() {0,1}, 3900),
                new Clothes(75, new List<int>() {0,1}, 5460),
                new Clothes(76, new List<int>() {0,1}, 7150),
                new Clothes(77, new List<int>() {0,1}, 8710),
                new Clothes(78, new List<int>() {0,1}, 8710),
                new Clothes(79, new List<int>() {0,1}, 10140),
                new Clothes(80, new List<int>() {0,1}, 10400),
                new Clothes(81, new List<int>() {0,1}, 10920),
                new Clothes(82, new List<int>() {0,1}, 11960),
                new Clothes(83, new List<int>() {0,1}, 11960),
                new Clothes(85, new List<int>() {0,1}, 3900),
                new Clothes(86, new List<int>() {0,1}, 5460),
                new Clothes(87, new List<int>() {0,1}, 7150),
                new Clothes(88, new List<int>() {0,1}, 8710),
                new Clothes(89, new List<int>() {0,1}, 8710),
                new Clothes(90, new List<int>() {0,1}, 10140),
                new Clothes(91, new List<int>() {0,1}, 10400),
                new Clothes(92, new List<int>() {0,1}, 10920),
                new Clothes(93, new List<int>() {0,1}, 11960),
                new Clothes(94, new List<int>() {0,1}, 11960),
                new Clothes(110, new List<int>() {0,1}, 11830),
                new Clothes(111, new List<int>() {0,1}, 11830),
                new Clothes(112, new List<int>() {0,1,2}, 5200),
                new Clothes(113, new List<int>() {0}, 5070),
                new Clothes(114, new List<int>() {0}, 3510),
                new Clothes(115, new List<int>() {0,1}, 6500),
                new Clothes(116, new List<int>() {0,1,2,3,4,5,6,7,8,9}, 5070),
                new Clothes(117, new List<int>() {0,1,2,3,4,5,6,7,8,9}, 5070),
                new Clothes(118, new List<int>() {0}, 3900),
                new Clothes(119, new List<int>() {0,1}, 11830),
                new Clothes(120, new List<int>() {0,1}, 11830),
                new Clothes(121, new List<int>() {0,1}, 12480),
                new Clothes(122, new List<int>() {0,1}, 12480),
                new Clothes(123, new List<int>() {0,1}, 6110),
                new Clothes(124, new List<int>() {0,1}, 4030),
                new Clothes(129, new List<int>() {0}, 4550),
                new Clothes(130, new List<int>() {0}, 4550),
                new Clothes(131, new List<int>() {0}, 4550),
                new Clothes(132, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 7410),
                new Clothes(134, new List<int>() {0}, 10140),
                new Clothes(135, new List<int>() {0}, 10140),
                new Clothes(136, new List<int>() {0,1,2}, 14950),
                new Clothes(137, new List<int>() {0,1,2}, 14950),
                new Clothes(138, new List<int>() {0,1,2}, 16640),
                new Clothes(139, new List<int>() {0,1,2}, 16640),
                new Clothes(140, new List<int>() {0,1,2}, 14950),
                new Clothes(141, new List<int>() {0,1,2}, 14950),
                new Clothes(142, new List<int>() {0,1,2}, 15600),
                new Clothes(143, new List<int>() {0,1,2}, 15600),
                new Clothes(144, new List<int>() {0,1,2}, 13650),
                new Clothes(145, new List<int>() {0,1,2}, 13650),
                new Clothes(149, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 19500),
                new Clothes(150, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 19500),
                new Clothes(151, new List<int>() {0}, 4030),

                new Clothes(155, new List<int>() {0}, 41500, -1),
                new Clothes(157, new List<int>() {0,1,2,3,4,5,6,7,8,9}, 17640, -1),
                new Clothes(159, new List<int>() {0}, 21450, -1),
                new Clothes(161, new List<int>() {0,1,2,3,4}, 19500, -1),
                new Clothes(163, new List<int>() {0,1,2,3,4}, 19500, -1),
                new Clothes(165, new List<int>() {0,1,2,3,4}, 21050, -1),
                new Clothes(166, new List<int>() {0,1}, 12400, -1),

                //Donate Clothes Shop
                new Clothes(175, new List<int>() {0}, 6900, -1),
                new Clothes(176, new List<int>() {0}, 9000, -1),
                new Clothes(177, new List<int>() {0}, 10000, -1),
                new Clothes(178, new List<int>() {0}, 9500, -1),
                new Clothes(179, new List<int>() {0}, 6000, -1),
                new Clothes(180, new List<int>() {0}, 1500, -1),
                new Clothes(181, new List<int>() {0}, 6500, -1),
                new Clothes(182, new List<int>() {0}, 4500, -1),
                new Clothes(183, new List<int>() {0}, 5500, -1),
                new Clothes(184, new List<int>() {0}, 7500, -1),
                new Clothes(185, new List<int>() {0}, 2000 , -1),
                new Clothes(186, new List<int>() {0}, 2000, -1),
                new Clothes(187, new List<int>() {0}, 8500, -1),
                new Clothes(188, new List<int>() {0}, 13000, -1),
                new Clothes(189, new List<int>() {0}, 1000, -1),
            }},
            { false, new List<Clothes>(){
                new Clothes(1, new List<int>() {0,1,2,3,4,5}, 12610),
                new Clothes(2, new List<int>() {0,1,2,3,4,5}, 4160),
                new Clothes(3, new List<int>() {0,1,2,3,4,5}, 6240),
                new Clothes(6, new List<int>() {0,1,2,3,4,5}, 17030),
                new Clothes(7, new List<int>() {0,1}, 10270),
                new Clothes(9, new List<int>() {0}, 5200),
                new Clothes(10, new List<int>() {0,1,2,3}, 18590),
                new Clothes(11, new List<int>() {0,1,2,3}, 12220),
                new Clothes(12, new List<int>() {0,1,2}, 9230),
                new Clothes(13, new List<int>() {0,1,2,3,4,5}, 6890),
                new Clothes(14, new List<int>() {0,1,2,3}, 4940),
                new Clothes(15, new List<int>() {0,1,2,3,4}, 6370),
                new Clothes(17, new List<int>() {0,1,2,3}, 2990),
                new Clothes(18, new List<int>() {0,1,2,3}, 2990),
                new Clothes(19, new List<int>() {0}, 845),
                new Clothes(20, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 8450),
                new Clothes(21, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 4160),
                new Clothes(22, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 8450),
                new Clothes(23, new List<int>() {0,1,2}, 3640),
                new Clothes(27, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 4160),
                new Clothes(28, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 8450),
                new Clothes(29, new List<int>() {0,1}, 5850),
                new Clothes(30, new List<int>() {0,1}, 5460),
                new Clothes(31, new List<int>() {0}, 4940),
                new Clothes(32, new List<int>() {0,1}, 5850),
                new Clothes(33, new List<int>() {0,1}, 5850),
                new Clothes(34, new List<int>() {0,1}, 8190),
                new Clothes(35, new List<int>() {0,1}, 6240),
                new Clothes(36, new List<int>() {0,1}, 6760),
                new Clothes(37, new List<int>() {0,1}, 5460),
                new Clothes(38, new List<int>() {0}, 4940),
                new Clothes(39, new List<int>() {0,1}, 5850),
                new Clothes(40, new List<int>() {0,1}, 5850),
                new Clothes(41, new List<int>() {0,1}, 8190),
                new Clothes(42, new List<int>() {0,1}, 6240),
                new Clothes(53, new List<int>() {0,1}, 3900),
                new Clothes(54, new List<int>() {0,1}, 5460),
                new Clothes(55, new List<int>() {0,1}, 7150),
                new Clothes(56, new List<int>() {0,1}, 8710),
                new Clothes(57, new List<int>() {0,1}, 8710),
                new Clothes(58, new List<int>() {0,1}, 10140),
                new Clothes(59, new List<int>() {0,1}, 10400),
                new Clothes(60, new List<int>() {0,1}, 10920),
                new Clothes(61, new List<int>() {0,1}, 11960),
                new Clothes(62, new List<int>() {0,1}, 11960),
                new Clothes(64, new List<int>() {0,1}, 3900),
                new Clothes(65, new List<int>() {0,1}, 5460),
                new Clothes(66, new List<int>() {0,1}, 7150),
                new Clothes(67, new List<int>() {0,1}, 8710),
                new Clothes(68, new List<int>() {0,1}, 8710),
                new Clothes(69, new List<int>() {0,1}, 10140),
                new Clothes(70, new List<int>() {0,1}, 10400),
                new Clothes(71, new List<int>() {0,1}, 10920),
                new Clothes(72, new List<int>() {0,1}, 11960),
                new Clothes(73, new List<int>() {0,1}, 11960),
                new Clothes(81, new List<int>() {0,1}, 11830),
                new Clothes(82, new List<int>() {0,1}, 11830),
                new Clothes(83, new List<int>() {0,1,2}, 5200),
                new Clothes(84, new List<int>() {0}, 5070),
                new Clothes(85, new List<int>() {0}, 3510),
                new Clothes(86, new List<int>() {0,1}, 6500),
                new Clothes(87, new List<int>() {0,1,2,3,4,5,6,7,8,9}, 5070),
                new Clothes(89, new List<int>() {0,1}, 11830),
                new Clothes(90, new List<int>() {0,1}, 11830),
                new Clothes(91, new List<int>() {0,1}, 12480),
                new Clothes(92, new List<int>() {0,1}, 12480),
                new Clothes(93, new List<int>() {0,1}, 6110),
                new Clothes(94, new List<int>() {0,1}, 4030),
                new Clothes(99, new List<int>() {0}, 4550),
                new Clothes(100, new List<int>() {0}, 4550),
                new Clothes(101, new List<int>() {0}, 4550),
                new Clothes(103, new List<int>() {0}, 10140),
                new Clothes(104, new List<int>() {0}, 10140),
                new Clothes(105, new List<int>() {0,1,2}, 14950),
                new Clothes(106, new List<int>() {0,1,2}, 14950),
                new Clothes(107, new List<int>() {0,1,2}, 16640),
                new Clothes(108, new List<int>() {0,1,2}, 16640),
                new Clothes(109, new List<int>() {0,1,2}, 14950),
                new Clothes(110, new List<int>() {0,1,2}, 14950),
                new Clothes(111, new List<int>() {0,1,2}, 15600),
                new Clothes(112, new List<int>() {0,1,2}, 15600),
                new Clothes(113, new List<int>() {0,1,2}, 13650),
                new Clothes(114, new List<int>() {0,1,2}, 13650),
                new Clothes(118, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 19500),
                new Clothes(119, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 19500),
                new Clothes(120, new List<int>() {0}, 4030),

                new Clothes(124, new List<int>() {0}, 41500, -1),
                new Clothes(126, new List<int>() {0,1,2,3,4,5,6,7,8,9}, 17640, -1),
                new Clothes(128, new List<int>() {0}, 21450, -1),
                new Clothes(130, new List<int>() {0,1,2,3,4}, 19500, -1),
                new Clothes(132, new List<int>() {0,1,2,3,4}, 19500, -1),
                new Clothes(134, new List<int>() {0,1,2,3,4}, 21050, -1),
                new Clothes(135, new List<int>() {0,1}, 12400, -1),

                //Donate Clothes Shop
                new Clothes(145, new List<int>() {0}, 1500, -1),
                new Clothes(146, new List<int>() {0}, 6500, -1),
                new Clothes(147, new List<int>() {0}, 4500, -1),
                new Clothes(148, new List<int>() {0}, 5500, -1),
                new Clothes(149, new List<int>() {0}, 7500, -1),
                new Clothes(150, new List<int>() {0}, 6900, -1),
                new Clothes(151, new List<int>() {0}, 9000, -1),
                new Clothes(152, new List<int>() {0}, 10000, -1),
                new Clothes(153, new List<int>() {0}, 9500, -1),
                new Clothes(155, new List<int>() {0}, 2000, -1),
                new Clothes(156, new List<int>() {0}, 2000, -1),
                new Clothes(157, new List<int>() {0}, 8500, -1),
                new Clothes(158, new List<int>() {0}, 13000, -1),
                new Clothes(159, new List<int>() {0}, 1000, -1),

            }},
        };

        public static List<Clothes> Masks = new List<Clothes>()
        {
            new Clothes(1, new List<int>() {0,1,2,3}, 5000),
            new Clothes(1, new List<int>() {0,1,2,3}, 5000),
            new Clothes(2, new List<int>() {0,1,2,3}, 7500),
            new Clothes(3, new List<int>() {0}, 6500),
            new Clothes(4, new List<int>() {0,1,2,3}, 10000),
            new Clothes(5, new List<int>() {0,1,2,3}, 9000),
            new Clothes(6, new List<int>() {0,1,2,3}, 11000),
            new Clothes(7, new List<int>() {0,1,2,3}, 5000),
            new Clothes(8, new List<int>() {0,1,2}, 10000),
            new Clothes(9, new List<int>() {0}, 16000),
            new Clothes(10, new List<int>() {0}, 6500),
            new Clothes(11, new List<int>() {0,1,2}, 18000),
            new Clothes(12, new List<int>() {0,1,2}, 23000),
            new Clothes(13, new List<int>() {0}, 7000),
            new Clothes(14, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 20000),
            new Clothes(15, new List<int>() {0,1,2,3}, 12500),
            new Clothes(16, new List<int>() {0,1,2,3,4,5,6,7,8}, 23000),
            new Clothes(17, new List<int>() {0,1}, 25000),
            new Clothes(18, new List<int>() {0,1}, 25000),
            new Clothes(19, new List<int>() {0,1}, 25000),
            new Clothes(20, new List<int>() {0,1}, 27000),
            new Clothes(21, new List<int>() {0,1}, 21000),
            new Clothes(22, new List<int>() {0,1}, 18000),
            new Clothes(23, new List<int>() {0,1}, 20000),
            new Clothes(24, new List<int>() {0,1}, 25000),
            new Clothes(25, new List<int>() {0,1}, 19000),
            new Clothes(26, new List<int>() {0,1}, 25000),
            new Clothes(28, new List<int>() {0,1,2,3,4}, 15000),
            new Clothes(29, new List<int>() {0,1,2,3,4}, 16000),
            new Clothes(30, new List<int>() {0}, 13000),
            new Clothes(31, new List<int>() {0}, 10000),
            new Clothes(32, new List<int>() {0}, 4000),
            new Clothes(33, new List<int>() {0}, 20000),
            new Clothes(34, new List<int>() {0,1,2}, 7000),
            new Clothes(35, new List<int>() {0}, 5000),
            new Clothes(37, new List<int>() {0}, 3500),
            new Clothes(38, new List<int>() {0}, 8000),
            new Clothes(39, new List<int>() {0,1}, 14000),
            new Clothes(40, new List<int>() {0,1}, 18000),
            new Clothes(41, new List<int>() {0,1}, 19500),
            new Clothes(42, new List<int>() {0,1}, 12500),
            new Clothes(43, new List<int>() {0}, 10000),
            new Clothes(44, new List<int>() {0}, 25000),
            new Clothes(45, new List<int>() {0}, 20000),
            new Clothes(47, new List<int>() {0,1,2,3}, 6000),
            new Clothes(48, new List<int>() {0,1,2,3}, 3500),
            new Clothes(49, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 6500),
            new Clothes(50, new List<int>() {0,1,2,3,4,5,6,7,8,9}, 6500),
            new Clothes(51, new List<int>() {0,1,2,3,4,5,6,7,8,9}, 5000),
            new Clothes(52, new List<int>() {0,1,2,3,4,5,6,7,8,9,10}, 7500),
            new Clothes(53, new List<int>() {0,1,2,3,4,5,6,7,8}, 11500),
            new Clothes(54, new List<int>() {0,1,2,3,4,5,6,7,8,9,10}, 13000),
            new Clothes(55, new List<int>() {0,1}, 7000),
            new Clothes(56, new List<int>() {0,1,2,3,4,5,6,7,8}, 10000),
            new Clothes(57, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21}, 35000),
            new Clothes(58, new List<int>() {0,1,2,3,4,5,6,7,8,9}, 15000),
            new Clothes(59, new List<int>() {0}, 14000),
            new Clothes(60, new List<int>() {0,1,2}, 18000),
            new Clothes(61, new List<int>() {0,1,2}, 12500),
            new Clothes(62, new List<int>() {0,1,2}, 20000),
            new Clothes(63, new List<int>() {0,1,2}, 25000),
            new Clothes(64, new List<int>() {0,1,2}, 25000),
            new Clothes(65, new List<int>() {0,1,2}, 18000),
            new Clothes(66, new List<int>() {0,1,2}, 18000),
            new Clothes(67, new List<int>() {0,1,2}, 7500),
            new Clothes(68, new List<int>() {0,1,2}, 25000),
            new Clothes(69, new List<int>() {0,1,2}, 10000),
            new Clothes(70, new List<int>() {0,1,2}, 27000),
            new Clothes(71, new List<int>() {0,1,2}, 10000),
            new Clothes(72, new List<int>() {0,1,2}, 18000),
            new Clothes(74, new List<int>() {0,1,2}, 23000),
            new Clothes(75, new List<int>() {0,1,2}, 24000),
            new Clothes(76, new List<int>() {0,1,2}, 13500),
            new Clothes(77, new List<int>() {0,1,2,3,4,5}, 13000),
            new Clothes(78, new List<int>() {0,1}, 15000),
            new Clothes(79, new List<int>() {0,1,2}, 17500),
            new Clothes(80, new List<int>() {0,1,2}, 20000),
            new Clothes(81, new List<int>() {0,1,2}, 18000),
            new Clothes(82, new List<int>() {0,1,2}, 15000),
            new Clothes(83, new List<int>() {0,1,2}, 14000),
            new Clothes(84, new List<int>() {0}, 14000),
            new Clothes(85, new List<int>() {0,1,2}, 20000),
            new Clothes(86, new List<int>() {0,1,2}, 15000),
            new Clothes(87, new List<int>() {0,1,2}, 22000),
            new Clothes(88, new List<int>() {0,1,2}, 10000),
            new Clothes(89, new List<int>() {0,1,2,3,4}, 15000),
            new Clothes(90, new List<int>() {0,1,2,3,4,5,6,7}, 13000),
            new Clothes(92, new List<int>() {0,1,2,3,4,5}, 27000),
            new Clothes(93, new List<int>() {0,1,2,3,4,5}, 19000),
            new Clothes(94, new List<int>() {0,1,2,3,4,5}, 30000),
            new Clothes(95, new List<int>() {0,1,2,3,4,5,6,7}, 20000),
            new Clothes(96, new List<int>() {0,1,2,3}, 12500),
            new Clothes(97, new List<int>() {0,1,2,3,4,5}, 18000),
            new Clothes(98, new List<int>() {0}, 23500),
            new Clothes(99, new List<int>() {0,1,2,3,4,5}, 32500),
            new Clothes(100, new List<int>() {0,1,2,3,4,5}, 25000),
            new Clothes(101, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 33000),
            new Clothes(102, new List<int>() {0,1,2}, 50000),
            new Clothes(103, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 20000),
            new Clothes(104, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 30000),
            new Clothes(105, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23}, 40000),
            new Clothes(106, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 18000),
            new Clothes(107, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23}, 25000),
            new Clothes(108, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23}, 50000),
            new Clothes(110, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 65000),
            new Clothes(111, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 32000),
            new Clothes(112, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 27500),
            new Clothes(113, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21}, 40000),
            new Clothes(115, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 28000),
            new Clothes(116, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 30000),
            new Clothes(117, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20}, 22000),
            new Clothes(118, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 40000),
            new Clothes(119, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24}, 35000),
            new Clothes(126, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17}, 31000),
            new Clothes(128, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 27000),
            new Clothes(131, new List<int>() {0,1,2,3}, 15000),
            new Clothes(133, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16}, 60000),
            new Clothes(134, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19}, 80000),
            new Clothes(136, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 25000),
            new Clothes(137, new List<int>() {0,1,2,3,4,5,6,7}, 30000),
            new Clothes(138, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 40000),
            new Clothes(139, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 36000),
            new Clothes(140, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 45000),
            new Clothes(141, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 40000),
            new Clothes(142, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 23000),
            new Clothes(144, new List<int>() {0}, 10000),
            new Clothes(147, new List<int>() {0}, 12000),
            new Clothes(149, new List<int>() {0}, 15500),
            new Clothes(150, new List<int>() {0}, 15500),
            new Clothes(151, new List<int>() {0}, 15500),
            new Clothes(152, new List<int>() {0}, 15500),
            new Clothes(153, new List<int>() {0}, 15500),
            new Clothes(154, new List<int>() {0}, 35000),
            new Clothes(155, new List<int>() {0,1,2,3}, 25000),
            new Clothes(156, new List<int>() {0,1,2,3}, 25500),
            new Clothes(157, new List<int>() {0,1,2,3}, 26000),
            new Clothes(158, new List<int>() {0,1,2,3}, 24000),
            new Clothes(159, new List<int>() {0,1,2,3}, 30000),
            new Clothes(160, new List<int>() {0}, 10000),
            new Clothes(161, new List<int>() {0}, 15000),
            new Clothes(162, new List<int>() {0}, 7000),
            new Clothes(163, new List<int>() {0}, 8000),
            new Clothes(164, new List<int>() {0}, 10000),
            new Clothes(165, new List<int>() {0}, 7000),
            new Clothes(167, new List<int>() {0}, 10000),
            new Clothes(168, new List<int>() {0}, 12000),
            new Clothes(169, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 33000),
            new Clothes(170, new List<int>() {0}, 17000),
            new Clothes(171, new List<int>() {0}, 17500),
            new Clothes(172, new List<int>() {0,1,2}, 18000),
            new Clothes(173, new List<int>() {0}, 16000),
            new Clothes(174, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24}, 30000),
            new Clothes(176, new List<int>() {0,1,2,3}, 17000),
            new Clothes(177, new List<int>() {0}, 43000),
            new Clothes(178, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24}, 30000),
            new Clothes(179, new List<int>() {0,1,2,3,4,5,6,7}, 30000),
            new Clothes(180, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12}, 40000),
            new Clothes(181, new List<int>() {0,1,2,3}, 15000),
            new Clothes(182, new List<int>() {0,1,2,3}, 25000),
            new Clothes(183, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}, 65000),
            new Clothes(184, new List<int>() {0,1,2,3}, 15000),
            new Clothes(185, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 33000),
            new Clothes(186, new List<int>() {0,1,2,3,4,5,6,7,8}, 35000),
            new Clothes(187, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 27500),
            new Clothes(188, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11}, 40000),
            new Clothes(189, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19}, 90000),
            new Clothes(190, new List<int>() {0,1,2,3}, 18500),
            new Clothes(191, new List<int>() {0,1,2,3}, 30000),
            new Clothes(192, new List<int>() {0,1,2,3}, 25000),
            new Clothes(193, new List<int>() {0}, 7000),
            new Clothes(194, new List<int>() {0,1}, 20000),
            new Clothes(195, new List<int>() {0}, 4000),
            new Clothes(197, new List<int>() {0,1,2,3,4,5,6,7}, 30000),
            new Clothes(198, new List<int>() {0,1,2,3,4,5,6,7}, 32000),
            new Clothes(199, new List<int>() {0,1,2,3,4,5,6,7}, 27000),
            new Clothes(201, new List<int>() {0,1,2,3,4,5,6,7,8}, 35000),
            new Clothes(202, new List<int>() {0,1,2,3,4,5,6}, 34000),
            new Clothes(203, new List<int>() {0,1,2,3,4,5,6}, 34000),
            new Clothes(204, new List<int>() {0,1,2,3,4,5,6,7,8,9}, 38000),
            new Clothes(205, new List<int>() {0,1,2,3,4,5,6,7}, 30000),
            new Clothes(206, new List<int>() {0,1,2,3,4,5,6,7}, 35000),
            new Clothes(207, new List<int>() {0,1,2,3,4,5,6,7,8}, 45000),
            new Clothes(208, new List<int>() {0,1,2,3,4,5,6,7}, 40000),
            new Clothes(209, new List<int>() {0,1,2}, 49000),
            new Clothes(210, new List<int>() {0}, 23000),
            new Clothes(211, new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}, 51000),
            new Clothes(212, new List<int>() {0,1,2,3}, 35000),
            new Clothes(213, new List<int>() {0}, 30000),
            new Clothes(214, new List<int>() {0,1}, 27000),
            new Clothes(215, new List<int>() {0}, 25000),

            new Clothes(226, new List<int>() {0,1,2,3,4,5,6,7}, 115450, -1),
            new Clothes(227, new List<int>() {0}, 50000, -1),
            new Clothes(228, new List<int>() {0}, 135450, -1),
            new Clothes(229, new List<int>() {0,1,2}, 211445, -1),



        };

        public static Dictionary<int, Tuple<bool, bool, bool>> MaskTypes = new Dictionary<int, Tuple<bool, bool, bool>>()
        {

            { 1, new Tuple<bool, bool, bool>(false, false, false) },
            { 2, new Tuple<bool, bool, bool>(false, false, false) },
            { 3, new Tuple<bool, bool, bool>(false, false, false) },
            { 4, new Tuple<bool, bool, bool>(false, false, false) },
            { 5, new Tuple<bool, bool, bool>(false, false, false) },
            { 6, new Tuple<bool, bool, bool>(false, false, false) },
            { 7, new Tuple<bool, bool, bool>(false, false, false) },
            { 8, new Tuple<bool, bool, bool>(false, false, false) },
            { 9, new Tuple<bool, bool, bool>(false, false, false) },
            { 10, new Tuple<bool, bool, bool>(false, false, false) },
            { 11, new Tuple<bool, bool, bool>(false, false, false) },
            { 12, new Tuple<bool, bool, bool>(false, false, false) },
            { 13, new Tuple<bool, bool, bool>(false, false, false) },
            { 14, new Tuple<bool, bool, bool>(false, false, false) },
            { 15, new Tuple<bool, bool, bool>(false, false, false) },
            { 16, new Tuple<bool, bool, bool>(false, false, false) },
            { 17, new Tuple<bool, bool, bool>(false, false, false) },
            { 18, new Tuple<bool, bool, bool>(false, false, false) },
            { 19, new Tuple<bool, bool, bool>(false, false, false) },
            { 20, new Tuple<bool, bool, bool>(false, false, false) },
            { 21, new Tuple<bool, bool, bool>(false, false, false) },
            { 22, new Tuple<bool, bool, bool>(false, false, false) },
            { 23, new Tuple<bool, bool, bool>(false, false, false) },
            { 24, new Tuple<bool, bool, bool>(false, false, false) },
            { 25, new Tuple<bool, bool, bool>(false, false, false) },
            { 26, new Tuple<bool, bool, bool>(false, false, false) },
            { 28, new Tuple<bool, bool, bool>(false, false, false) },
            { 29, new Tuple<bool, bool, bool>(false, false, false) },
            { 30, new Tuple<bool, bool, bool>(false, false, false) },
            { 31, new Tuple<bool, bool, bool>(false, false, false) },
            { 32, new Tuple<bool, bool, bool>(false, false, false) },
            { 33, new Tuple<bool, bool, bool>(false, false, false) },
            { 34, new Tuple<bool, bool, bool>(false, false, false) },
            { 35, new Tuple<bool, bool, bool>(false, false, false) },
            { 37, new Tuple<bool, bool, bool>(false, false, false) },
            { 38, new Tuple<bool, bool, bool>(false, false, false) },
            { 39, new Tuple<bool, bool, bool>(false, false, false) },
            { 40, new Tuple<bool, bool, bool>(false, false, false) },
            { 41, new Tuple<bool, bool, bool>(false, false, false) },
            { 42, new Tuple<bool, bool, bool>(false, false, false) },
            { 43, new Tuple<bool, bool, bool>(false, false, false) },
            { 44, new Tuple<bool, bool, bool>(false, false, false) },
            { 45, new Tuple<bool, bool, bool>(false, false, false) },
            { 47, new Tuple<bool, bool, bool>(false, false, false) },
            { 48, new Tuple<bool, bool, bool>(false, false, false) },
            { 49, new Tuple<bool, bool, bool>(false, false, false) },
            { 50, new Tuple<bool, bool, bool>(false, false, false) },
            { 51, new Tuple<bool, bool, bool>(false, false, false) },
            { 52, new Tuple<bool, bool, bool>(false, false, false) },
            { 53, new Tuple<bool, bool, bool>(false, false, false) },
            { 54, new Tuple<bool, bool, bool>(false, false, false) },
            { 55, new Tuple<bool, bool, bool>(false, false, false) },
            { 56, new Tuple<bool, bool, bool>(false, false, false) },
            { 57, new Tuple<bool, bool, bool>(false, false, false) },
            { 58, new Tuple<bool, bool, bool>(false, false, false) },
            { 59, new Tuple<bool, bool, bool>(false, false, false) },
            { 60, new Tuple<bool, bool, bool>(false, false, false) },
            { 61, new Tuple<bool, bool, bool>(false, false, false) },
            { 62, new Tuple<bool, bool, bool>(false, false, false) },
            { 63, new Tuple<bool, bool, bool>(false, false, false) },
            { 64, new Tuple<bool, bool, bool>(false, false, false) },
            { 65, new Tuple<bool, bool, bool>(false, false, false) },
            { 66, new Tuple<bool, bool, bool>(false, false, false) },
            { 67, new Tuple<bool, bool, bool>(false, false, false) },
            { 68, new Tuple<bool, bool, bool>(false, false, false) },
            { 69, new Tuple<bool, bool, bool>(false, false, false) },
            { 71, new Tuple<bool, bool, bool>(false, false, false) },
            { 72, new Tuple<bool, bool, bool>(false, false, false) },
            { 74, new Tuple<bool, bool, bool>(false, false, false) },
            { 75, new Tuple<bool, bool, bool>(false, false, false) },
            { 76, new Tuple<bool, bool, bool>(false, false, false) },
            { 77, new Tuple<bool, bool, bool>(false, false, false) },
            { 78, new Tuple<bool, bool, bool>(false, false, false) },
            { 79, new Tuple<bool, bool, bool>(false, false, false) },
            { 80, new Tuple<bool, bool, bool>(false, false, false) },
            { 81, new Tuple<bool, bool, bool>(false, false, false) },
            { 82, new Tuple<bool, bool, bool>(false, false, false) },
            { 83, new Tuple<bool, bool, bool>(false, false, false) },
            { 84, new Tuple<bool, bool, bool>(false, false, false) },
            { 85, new Tuple<bool, bool, bool>(false, false, false) },
            { 86, new Tuple<bool, bool, bool>(false, false, false) },
            { 87, new Tuple<bool, bool, bool>(false, false, false) },
            { 88, new Tuple<bool, bool, bool>(false, false, false) },
            { 89, new Tuple<bool, bool, bool>(false, false, false) },
            { 90, new Tuple<bool, bool, bool>(false, false, false) },
            { 92, new Tuple<bool, bool, bool>(false, false, false) },
            { 93, new Tuple<bool, bool, bool>(false, false, false) },
            { 94, new Tuple<bool, bool, bool>(false, false, false) },
            { 95, new Tuple<bool, bool, bool>(false, false, false) },
            { 96, new Tuple<bool, bool, bool>(false, false, false) },
            { 97, new Tuple<bool, bool, bool>(false, false, false) },
            { 98, new Tuple<bool, bool, bool>(false, false, false) },
            { 99, new Tuple<bool, bool, bool>(false, false, false) },
            { 100, new Tuple<bool, bool, bool>(false, false, false) },
            { 101, new Tuple<bool, bool, bool>(false, false, false) },
            { 102, new Tuple<bool, bool, bool>(false, false, false) },
            { 103, new Tuple<bool, bool, bool>(false, false, false) },
            { 104, new Tuple<bool, bool, bool>(false, false, false) },
            { 105, new Tuple<bool, bool, bool>(false, false, false) },
            { 106, new Tuple<bool, bool, bool>(false, false, false) },
            { 107, new Tuple<bool, bool, bool>(false, false, false) },
            { 108, new Tuple<bool, bool, bool>(false, false, false) },
            { 110, new Tuple<bool, bool, bool>(false, false, false) },
            { 111, new Tuple<bool, bool, bool>(false, false, false) },
            { 112, new Tuple<bool, bool, bool>(false, false, false) },
            { 113, new Tuple<bool, bool, bool>(false, false, false) },
            { 115, new Tuple<bool, bool, bool>(false, false, false) },
            { 116, new Tuple<bool, bool, bool>(false, false, false) },
            { 117, new Tuple<bool, bool, bool>(false, false, false) },
            { 118, new Tuple<bool, bool, bool>(false, false, false) },
            { 119, new Tuple<bool, bool, bool>(false, false, false) },
            { 126, new Tuple<bool, bool, bool>(false, false, false) },
            { 128, new Tuple<bool, bool, bool>(false, false, false) },
            { 131, new Tuple<bool, bool, bool>(false, false, false) },
            { 133, new Tuple<bool, bool, bool>(false, false, false) },
            { 134, new Tuple<bool, bool, bool>(false, false, false) },
            { 136, new Tuple<bool, bool, bool>(false, false, false) },
            { 137, new Tuple<bool, bool, bool>(false, false, false) },
            { 138, new Tuple<bool, bool, bool>(false, false, false) },
            { 139, new Tuple<bool, bool, bool>(false, false, false) },
            { 140, new Tuple<bool, bool, bool>(false, false, false) },
            { 141, new Tuple<bool, bool, bool>(false, false, false) },
            { 142, new Tuple<bool, bool, bool>(false, false, false) },
            { 144, new Tuple<bool, bool, bool>(false, false, false) },
            { 147, new Tuple<bool, bool, bool>(false, false, false) },
            { 149, new Tuple<bool, bool, bool>(false, false, false) },
            { 150, new Tuple<bool, bool, bool>(false, false, false) },
            { 151, new Tuple<bool, bool, bool>(false, false, false) },
            { 152, new Tuple<bool, bool, bool>(false, false, false) },
            { 153, new Tuple<bool, bool, bool>(false, false, false) },
            { 154, new Tuple<bool, bool, bool>(false, false, false) },
            { 155, new Tuple<bool, bool, bool>(false, false, false) },
            { 156, new Tuple<bool, bool, bool>(false, false, false) },
            { 157, new Tuple<bool, bool, bool>(false, false, false) },
            { 158, new Tuple<bool, bool, bool>(false, false, false) },
            { 159, new Tuple<bool, bool, bool>(false, false, false) },
            { 160, new Tuple<bool, bool, bool>(false, false, false) },
            { 161, new Tuple<bool, bool, bool>(false, false, false) },
            { 162, new Tuple<bool, bool, bool>(false, false, false) },
            { 163, new Tuple<bool, bool, bool>(false, false, false) },
            { 164, new Tuple<bool, bool, bool>(false, false, false) },
            { 165, new Tuple<bool, bool, bool>(false, false, false) },
            { 167, new Tuple<bool, bool, bool>(false, false, false) },
            { 168, new Tuple<bool, bool, bool>(false, false, false) },
            { 169, new Tuple<bool, bool, bool>(false, false, false) },
            { 170, new Tuple<bool, bool, bool>(false, false, false) },
            { 171, new Tuple<bool, bool, bool>(false, false, false) },
            { 172, new Tuple<bool, bool, bool>(false, false, false) },
            { 173, new Tuple<bool, bool, bool>(false, false, false) },
            { 174, new Tuple<bool, bool, bool>(false, false, false) },
            { 176, new Tuple<bool, bool, bool>(false, false, false) },
            { 178, new Tuple<bool, bool, bool>(false, false, false) },
            { 179, new Tuple<bool, bool, bool>(false, false, false) },
            { 180, new Tuple<bool, bool, bool>(false, false, false) },
            { 181, new Tuple<bool, bool, bool>(false, false, false) },
            { 182, new Tuple<bool, bool, bool>(false, false, false) },
            { 183, new Tuple<bool, bool, bool>(false, false, false) },
            { 184, new Tuple<bool, bool, bool>(false, false, false) },
            { 185, new Tuple<bool, bool, bool>(false, false, false) },
            { 186, new Tuple<bool, bool, bool>(false, false, false) },
            { 187, new Tuple<bool, bool, bool>(false, false, false) },
            { 188, new Tuple<bool, bool, bool>(false, false, false) },
            { 189, new Tuple<bool, bool, bool>(false, false, false) },
            { 190, new Tuple<bool, bool, bool>(false, false, false) },
            { 191, new Tuple<bool, bool, bool>(false, false, false) },
            { 192, new Tuple<bool, bool, bool>(false, false, false) },
            { 193, new Tuple<bool, bool, bool>(false, false, false) },
            { 194, new Tuple<bool, bool, bool>(false, false, false) },
            { 195, new Tuple<bool, bool, bool>(false, false, false) },
            { 196, new Tuple<bool, bool, bool>(false, false, false) },
            { 197, new Tuple<bool, bool, bool>(false, false, false) },
            { 198, new Tuple<bool, bool, bool>(false, false, false) },
            { 199, new Tuple<bool, bool, bool>(false, false, false) },
            { 200, new Tuple<bool, bool, bool>(false, false, false) },
            { 201, new Tuple<bool, bool, bool>(false, false, false) },
            { 202, new Tuple<bool, bool, bool>(false, false, false) },
            { 203, new Tuple<bool, bool, bool>(false, false, false) },
            { 204, new Tuple<bool, bool, bool>(false, false, false) },
            { 205, new Tuple<bool, bool, bool>(false, false, false) },
            { 206, new Tuple<bool, bool, bool>(false, false, false) },
            { 207, new Tuple<bool, bool, bool>(false, false, false) },
            { 208, new Tuple<bool, bool, bool>(false, false, false) },
            { 209, new Tuple<bool, bool, bool>(false, false, false) },
            { 210, new Tuple<bool, bool, bool>(false, false, false) },
            { 211, new Tuple<bool, bool, bool>(false, false, false) },
            { 212, new Tuple<bool, bool, bool>(false, false, false) },
            { 213, new Tuple<bool, bool, bool>(false, false, false) },
            { 214, new Tuple<bool, bool, bool>(false, false, false) },
            { 215, new Tuple<bool, bool, bool>(false, false, false) },
            { 216, new Tuple<bool, bool, bool>(false, false, false) },
            { 217, new Tuple<bool, bool, bool>(false, false, false) },
            { 218, new Tuple<bool, bool, bool>(false, false, false) },
            { 219, new Tuple<bool, bool, bool>(false, false, false) },
            { 220, new Tuple<bool, bool, bool>(false, false, false) },
            { 221, new Tuple<bool, bool, bool>(false, false, false) },
            { 222, new Tuple<bool, bool, bool>(false, false, false) },
            { 223, new Tuple<bool, bool, bool>(false, false, false) },
            { 224, new Tuple<bool, bool, bool>(false, false, false) },
            { 225, new Tuple<bool, bool, bool>(false, false, false) },
            { 226, new Tuple<bool, bool, bool>(false, false, false) },
            { 227, new Tuple<bool, bool, bool>(false, false, false) },
            { 228, new Tuple<bool, bool, bool>(false, false, false) },
            { 229, new Tuple<bool, bool, bool>(false, false, false) },
            { 230, new Tuple<bool, bool, bool>(false, false, false) },
            { 231, new Tuple<bool, bool, bool>(false, false, false) },
            { 232, new Tuple<bool, bool, bool>(false, false, false) },
            { 233, new Tuple<bool, bool, bool>(false, false, false) },
            { 234, new Tuple<bool, bool, bool>(false, false, false) },
            { 235, new Tuple<bool, bool, bool>(false, false, false) },
            { 236, new Tuple<bool, bool, bool>(false, false, false) }

        };

        public static Dictionary<bool, Dictionary<int, int>> AccessoryRHand = new Dictionary<bool, Dictionary<int, int>>()
        {
            { true, new Dictionary<int, int>(){
                { 22, 0 },
                { 23, 1 },
                { 24, 2 },
                { 25, 3 },
                { 26, 4 },
                { 27, 5 },
                { 28, 6 },
                { 29, 7 },
            }},
            { false, new Dictionary<int, int>(){
                { 11, 7 },
                { 12, 8 },
                { 13, 9 },
                { 14, 10 },
                { 15, 11 },
                { 16, 12 },
                { 17, 13 },
                { 18, 14 },
            }},
        };

        public static Vector3 CreatorCharPos = new Vector3(402.8664, -996.4108, -99.00027);
        public static Vector3 CreatorPos = new Vector3(402.8664, -997.5515, -98.5);
        public static Vector3 CameraLookAtPos = new Vector3(402.8664, -996.4108, -98.5);
        public static float FacingAngle = -185.0f;
        public static int DimensionID = 1;

        #region Methods

        public static bool ApplyCharacter(Player player)
        {
            if (!Main.Players.ContainsKey(player)) return false;
            if (!CustomPlayerData.ContainsKey(Main.Players[player].UUID)) return false;

            var custom = CustomPlayerData[Main.Players[player].UUID];
            var gender = Main.Players[player].Gender;
            player.SetSharedData("GENDER", gender); //TODO check

            var clothes = custom.Clothes;
            for (int i = 0; i <= 8; i++) player.ClearAccessory(i);

            int torsoV, torsoT = 0;
            var noneGloves = CorrectTorso[gender][clothes.Top.Variation];

            if (clothes.Gloves.Variation == 0)
                torsoV = noneGloves;
            else
            {
                torsoV = CorrectGloves[gender][clothes.Gloves.Variation][noneGloves];
                torsoT = clothes.Gloves.Texture;
            }

            if (!MaskTypes.ContainsKey(clothes.Mask.Variation) || !MaskTypes[clothes.Mask.Variation].Item1) player.SetClothes(2, custom.Hair.Hair, 0); NAPI.Player.SetPlayerHairColor(player, (byte)custom.Hair.Color, (byte)custom.Hair.HighlightColor);
            player.SetClothes(3, torsoV, torsoT);
            player.SetClothes(4, clothes.Leg.Variation, clothes.Leg.Texture);
            player.SetClothes(5, clothes.Bag.Variation, clothes.Bag.Texture);
            player.SetClothes(6, clothes.Feet.Variation, clothes.Feet.Texture);
            player.SetClothes(7, clothes.Accessory.Variation, clothes.Accessory.Texture);
            player.SetClothes(8, clothes.Undershit.Variation, clothes.Undershit.Texture);
            player.SetClothes(9, clothes.Bodyarmor.Variation, clothes.Bodyarmor.Texture);
            player.SetClothes(10, clothes.Decals.Variation, clothes.Decals.Texture);
            player.SetClothes(11, clothes.Top.Variation, clothes.Top.Texture);

            //Log.Debug($"[APPLYCHARACTER] 5 Bag: var: {clothes.Bag.Variation} tex: {clothes.Bag.Texture}", nLog.Type.Error);

            // loading tattoos
            foreach (var list in custom.Tattoos.Values)
            {
                foreach (var t in list)
                {
                    if (t == null) continue;
                    var decoration = new Decoration();
                    decoration.Collection = NAPI.Util.GetHashKey(t.Dictionary);
                    decoration.Overlay = NAPI.Util.GetHashKey(t.Hash);
                    player.SetDecoration(decoration);
                }
            }

            player.SetSharedData("TATTOOS", JsonConvert.SerializeObject(custom.Tattoos));

            var accesory = custom.Accessory;
            SetHat(player, accesory.Hat.Variation, accesory.Hat.Texture);
            if (accesory.Glasses.Variation != -1 && !player.HasMyData("HEAD_POCKET")) player.SetAccessories(1, accesory.Glasses.Variation, accesory.Glasses.Texture);
            if (accesory.Ear.Variation != -1) player.SetAccessories(2, accesory.Ear.Variation, accesory.Ear.Texture);
            if (accesory.Watches.Variation != -1) player.SetAccessories(6, accesory.Watches.Variation, accesory.Watches.Texture);
            if (accesory.Bracelets.Variation != -1) player.SetAccessories(7, accesory.Bracelets.Variation, accesory.Bracelets.Texture);

            ApplyCharacterFace(player);

            if (!player.HasMyData("HEAD_POCKET") && clothes.Mask.Variation != 0)
                SetMask(player, clothes.Mask.Variation, clothes.Mask.Texture);
            return true;
        }

        public static void ApplyCharacterFace(Player player)
        {
            var custom = CustomPlayerData[Main.Players[player].UUID];

            var parents = custom.Parents;
            var headBlend = new HeadBlend();
            headBlend.ShapeFirst = (byte)parents.Mother;
            headBlend.ShapeSecond = (byte)parents.Father;
            headBlend.ShapeThird = 0;

            headBlend.SkinFirst = (byte)parents.Mother;
            headBlend.SkinSecond = (byte)parents.Father;
            headBlend.SkinThird = 0;

            headBlend.ShapeMix = parents.Similarity;
            headBlend.SkinMix = parents.SkinSimilarity;
            headBlend.ThirdMix = 0.0f;

            NAPI.Player.SetPlayerHeadBlend(player, headBlend);
            for (int i = 0; i < custom.Features.Count(); i++) NAPI.Player.SetPlayerFaceFeature(player, i, custom.Features[i]);
            for (int i = 0; i < custom.Appearance.Count(); i++)
            {
                var headOverlay = new HeadOverlay();
                headOverlay.Index = (byte)custom.Appearance[i].Value;
                headOverlay.Opacity = (byte)custom.Appearance[i].Opacity;
                if (i == 1) headOverlay.Color = (byte)custom.BeardColor;
                else if (i == 2) headOverlay.Color = (byte)custom.EyebrowColor;
                else if (i == 5) headOverlay.Color = (byte)custom.BlushColor;
                else if (i == 8) headOverlay.Color = (byte)custom.LipstickColor;
                else if (i == 10) headOverlay.Color = (byte)custom.ChestHairColor;
                headOverlay.SecondaryColor = 100;
                NAPI.Player.SetPlayerHeadOverlay(player, i, headOverlay);
            }
            NAPI.Player.SetPlayerEyeColor(player, (byte)custom.EyeColor);
        }

        public static void SaveCharacter(Player player)
        {
            if (!CustomPlayerData.ContainsKey(Main.Players[player].UUID)) return;

            var data = CustomPlayerData[Main.Players[player].UUID];
            //Log.Write("save to db gender: "+ data.Gender);

            var Gender = data.Gender;
            var Parents = JsonConvert.SerializeObject(data.Parents);
            var Features = JsonConvert.SerializeObject(data.Features);
            var Appearance = JsonConvert.SerializeObject(data.Appearance);
            var Hair = JsonConvert.SerializeObject(data.Hair);
            var Clothes = JsonConvert.SerializeObject(data.Clothes);
            var Accessory = JsonConvert.SerializeObject(data.Accessory);
            var Tattoos = JsonConvert.SerializeObject(data.Tattoos);
            var EyebrowColor = data.EyebrowColor;
            var BeardColor = data.BeardColor;
            var EyeColor = data.EyeColor;
            var BlushColor = data.BlushColor;
            var LipstickColor = data.LipstickColor;
            var ChestHairColor = data.ChestHairColor;
            var IsCreated = data.IsCreated;

            //MySQL.Query($"UPDATE customization SET gender={Gender},parents='{Parents}',features='{Features}',appearance='{Appearance}',hair='{Hair}',clothes='{Clothes}'," +
            //        $"accessory='{Accessory}',tattoos='{Tattoos}',eyebrowc={EyebrowColor},beardc={BeardColor},eyec={EyeColor},blushc={BlushColor}," +
            //        $"lipstickc={LipstickColor},chesthairc={ChestHairColor},iscreated={IsCreated} WHERE uuid={Main.Players[player].UUID}");

            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = "UPDATE `customization` SET " +
              "`gender`=@gender, " +
              "`parents`=@parents, " +
              "`features`=@features, " +
              "`appearance`=@appearance, " +
              "`hair`=@hair, " +
              "`clothes`=@clothes, " +
              "`accessory`=@accessory, " +
              "`tattoos`=@tattoos, " +
              "`eyebrowc`=@eyebrowc, " +
              "`beardc`=@beardc, " +
              "`eyec`=@eyec, " +
              "`blushc`=@blushc, " +
              "`lipstickc`=@lipstickc, " +
              "`chesthairc`=@chesthairc, " +
              "`iscreated`=@iscreated" +
              " WHERE `uuid`=@uuid";

            cmd.Parameters.AddWithValue("@gender", Gender);
            cmd.Parameters.AddWithValue("@parents", Parents);
            cmd.Parameters.AddWithValue("@features", Features);
            cmd.Parameters.AddWithValue("@appearance", Appearance);
            cmd.Parameters.AddWithValue("@hair", Hair);
            cmd.Parameters.AddWithValue("@clothes", Clothes);
            cmd.Parameters.AddWithValue("@accessory", Accessory);
            cmd.Parameters.AddWithValue("@tattoos", Tattoos);
            cmd.Parameters.AddWithValue("@eyebrowc", EyebrowColor);
            cmd.Parameters.AddWithValue("@beardc", BeardColor);
            cmd.Parameters.AddWithValue("@eyec", EyeColor);
            cmd.Parameters.AddWithValue("@blushc", BlushColor);
            cmd.Parameters.AddWithValue("@lipstickc", LipstickColor);
            cmd.Parameters.AddWithValue("@chesthairc", ChestHairColor);
            cmd.Parameters.AddWithValue("@iscreated", IsCreated);
            cmd.Parameters.AddWithValue("@uuid", Main.Players[player].UUID);
            MySQL.Query(cmd);
        }

        public static void CreateCharacter(Player player)
        {
            player.SetMyData("Creator_PrevPos", player.Position);
            player.SetMyData("inCreator", true);

            if (!CustomPlayerData.ContainsKey(Main.Players[player].UUID))
            {
                CustomPlayerData.Add(Main.Players[player].UUID, new PlayerCustomization());

                var data = CustomPlayerData[Main.Players[player].UUID];
                var Gender = data.Gender;
                var Parents = JsonConvert.SerializeObject(data.Parents);
                var Features = JsonConvert.SerializeObject(data.Features);
                var Appearance = JsonConvert.SerializeObject(data.Appearance);
                var Hair = JsonConvert.SerializeObject(data.Hair);
                var Clothes = JsonConvert.SerializeObject(data.Clothes);
                var Accessory = JsonConvert.SerializeObject(data.Accessory);
                var Tattoos = JsonConvert.SerializeObject(data.Tattoos);
                var EyebrowColor = data.EyebrowColor;
                var BeardColor = data.BeardColor;
                var EyeColor = data.EyeColor;
                var BlushColor = data.BlushColor;
                var LipstickColor = data.LipstickColor;
                var ChestHairColor = data.ChestHairColor;
                var IsCreated = data.IsCreated;

                //MySQL.Query($"INSERT INTO `customization` (`uuid`,`gender`,`parents`,`features`,`appearance`,`hair`,`clothes`,`accessory`,`tattoos`,`eyebrowc`,`beardc`,`eyec`,`blushc`,`lipstickc`,`chesthairc`,`iscreated`) " +
                //        $"VALUES ({Main.Players[player].UUID},{Gender},'{Parents}','{Features}','{Appearance}','{Hair}','{Clothes}','{Accessory}','{Tattoos}',{EyebrowColor},{BeardColor},{EyeColor},{BlushColor},{LipstickColor},{ChestHairColor},{IsCreated})");

                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = "INSERT INTO `customization` SET " +
                  "`uuid`=@uuid, " +
                  "`gender`=@gender, " +
                  "`parents`=@parents, " +
                  "`features`=@features, " +
                  "`appearance`=@appearance, " +
                  "`hair`=@hair, " +
                  "`clothes`=@clothes, " +
                  "`accessory`=@accessory, " +
                  "`tattoos`=@tattoos, " +
                  "`eyebrowc`=@eyebrowc, " +
                  "`beardc`=@beardc, " +
                  "`eyec`=@eyec, " +
                  "`blushc`=@blushc, " +
                  "`lipstickc`=@lipstickc, " +
                  "`chesthairc`=@chesthairc, " +
                  "`iscreated`=@iscreated";

                cmd.Parameters.AddWithValue("@uuid", Main.Players[player].UUID);
                cmd.Parameters.AddWithValue("@gender", Gender);
                cmd.Parameters.AddWithValue("@parents", Parents);
                cmd.Parameters.AddWithValue("@features", Features);
                cmd.Parameters.AddWithValue("@appearance", Appearance);
                cmd.Parameters.AddWithValue("@hair", Hair);
                cmd.Parameters.AddWithValue("@clothes", Clothes);
                cmd.Parameters.AddWithValue("@accessory", Accessory);
                cmd.Parameters.AddWithValue("@tattoos", Tattoos);
                cmd.Parameters.AddWithValue("@eyebrowc", EyebrowColor);
                cmd.Parameters.AddWithValue("@beardc", BeardColor);
                cmd.Parameters.AddWithValue("@eyec", EyeColor);
                cmd.Parameters.AddWithValue("@blushc", BlushColor);
                cmd.Parameters.AddWithValue("@lipstickc", LipstickColor);
                cmd.Parameters.AddWithValue("@chesthairc", ChestHairColor);
                cmd.Parameters.AddWithValue("@iscreated", IsCreated);

                MySQL.Query(cmd);
            }

            NAPI.Task.Run(() =>
            {
                NAPI.Entity.SetEntityDimension(player, Convert.ToUInt32(DimensionID));
                player.Rotation = new Vector3(0f, 0f, FacingAngle);
                NAPI.Entity.SetEntityPosition(player, CreatorCharPos);

                var gender = Main.Players[player].Gender;
                SetDefaultFeatures(player, gender);
                //Trigger.ClientEvent(player, "CreatorCamera");
                Log.Debug("CLIENT::character::create:open", nLog.Type.Error);
                Trigger.ClientEvent(player, "CLIENT::character::create:open");
                DimensionID++;
            });
        }

        public static void SendToCreator(Player player)
        {
            if (player.HasMyData("inCreator")) return;
            player.SetMyData("Creator_PrevPos", player.Position);
            player.SetMyData("inCreator", true);

            NAPI.Entity.SetEntityDimension(player, Convert.ToUInt32(DimensionID));
            player.Rotation = new Vector3(0f, 0f, FacingAngle);
            NAPI.Entity.SetEntityPosition(player, CreatorCharPos);

            var gender = Main.Players[player].Gender;

            player.SetMyData("CHANGING_CHARACTER", true);
            var tattoos = CustomPlayerData[Main.Players[player].UUID].Tattoos;
            CustomPlayerData[Main.Players[player].UUID] = new PlayerCustomization();
            player.SetMyData("CHANGING_TATTOOS", tattoos);
            SetCreatorClothes(player, gender);

            Trigger.ClientEvent(player, "CLIENT::character::create:creatorCamera");
            DimensionID++;
        }

        public static void SendBackToWorld(Player player)
        {
            player.ResetMyData("inCreator");
            player.ResetMyData("Creator_PrevPos");
            if(player.HasMyData("INDONATECREATORMENU") && player.GetMyData<bool>("INDONATECREATORMENU"))
            {
                var before = Main.Accounts[player].RedBucks;
                Main.Accounts[player].RedBucks -= DonateShop.EditCharacterCost;

                Log.Debug($"[SWC Changes][{player.Name}] [DonateShop] Изменение персонажа: [{DonateShop.EditCharacterCost}] {before} -> {Main.Accounts[player].RedBucks}");
                GameLog.SWC(Main.Players[player].UUID, "[DonateShop] Изменение персонажа", Main.Accounts[player].Login, DonateShop.EditCharacterCost, before);

                player.ResetMyData("INDONATECREATORMENU");
            }
            NAPI.Task.Run(() => {
              NAPI.Entity.SetEntityPosition(player, Main.Players[player].SpawnPos);
              NAPI.Entity.SetEntityDimension(player, 0);
              NAPI.Entity.SetEntityRotation(player, new Vector3(0, 0, -113.932236));
            }, 500);
            player.SetSkin((Main.Players[player].Gender) ? PedHash.FreemodeMale01 : PedHash.FreemodeFemale01);

            Main.Players[player].IsSpawned = true;
            Trigger.ClientEvent(player, "CLIENT::character::create:destroyCamera");
        }

        public static void SetDefaultFeatures(Player player, bool gender, bool reset = false)
        {
            if (reset)
            {
                CustomPlayerData[Main.Players[player].UUID] = new PlayerCustomization();

                CustomPlayerData[Main.Players[player].UUID].Parents.Father = 0;
                CustomPlayerData[Main.Players[player].UUID].Parents.Mother = 21;
                CustomPlayerData[Main.Players[player].UUID].Parents.Similarity = (gender) ? 1.0f : 0.0f;
                CustomPlayerData[Main.Players[player].UUID].Parents.SkinSimilarity = (gender) ? 1.0f : 0.0f;
            }

            // will apply the resetted data
            ApplyCharacter(player);

            // clothes
            SetCreatorClothes(player, gender);
        }

        public static void SetCreatorClothes(Player player, bool gender)
        {
            if (!CustomPlayerData.ContainsKey(Main.Players[player].UUID)) return;

            // clothes
            //player.SetDefaultClothes();
            for (int i = 0; i < 10; i++) player.ClearAccessory(i);

            player.SetClothes(3, EmtptySlots[gender][3], 0);
            player.SetClothes(4, EmtptySlots[gender][4], 0);
            player.SetClothes(6, EmtptySlots[gender][6], 0);
            player.SetClothes(7, EmtptySlots[gender][7], 0);
            player.SetClothes(8, EmtptySlots[gender][8], 0);
            player.SetClothes(11, EmtptySlots[gender][11], 0);
            player.SetClothes(5, EmtptySlots[gender][5], 0);

            player.SetClothes(2, CustomPlayerData[Main.Players[player].UUID].Hair.Hair, 0);
        }

        public static void ClearClothes(Player player, bool gender)
        {
            player.SetClothes(3, EmtptySlots[gender][3], 0);
            player.SetClothes(4, EmtptySlots[gender][4], 0);
            player.SetClothes(6, EmtptySlots[gender][6], 0);
            player.SetClothes(7, EmtptySlots[gender][7], 0);
            player.SetClothes(8, EmtptySlots[gender][8], 0);
            player.SetClothes(11, EmtptySlots[gender][11], 0);
            player.SetClothes(5, EmtptySlots[gender][5], 0);
            player.SetClothes(9, EmtptySlots[gender][9], 0); // BodyArmor

            if (!player.HasMyData("HEAD_POCKET"))
            {
                player.SetClothes(1, 0, 0);
                SetHat(player, -1, 0);
                for (int i = 0; i <= 3; i++) player.ClearAccessory(i);
            }
        }

        public static void ClearAccessory(Player player)
        {
            try
            {
                NAPI.Task.Run(() => {
                    NAPI.Player.ClearPlayerAccessory(player, 0);
                    NAPI.Player.ClearPlayerAccessory(player, 1);
                    NAPI.Player.ClearPlayerAccessory(player, 2);
                    NAPI.Player.ClearPlayerAccessory(player, 6);
                    NAPI.Player.ClearPlayerAccessory(player, 7);
                });
            }
            catch (Exception e) { Log.Write("ClearAccessory: " + e.StackTrace, nLog.Type.Error); }

        }

        public static void AddClothes(Player player, ItemType type, int variation, int texture, bool isActive = false)
        {
            if (!Main.Players.ContainsKey(player)) return;
            if (!nInventory.Items.ContainsKey(Main.Players[player].UUID)) return;

            var item = new nItem(type, 1, $"{variation}_{texture}_{Main.Players[player].Gender}", isActive);
            nInventory.Add(player, item);

            if (isActive)
            {
                //Log.Debug("CLEAR!? AddClothes");
                nInventory.ClearSlot(nInventory.ItemsSlots[Main.Players[player].UUID], item, 5);
                item.character_slot_id = nInventory.ClothesItemSlots[type];
                item.slot_id = 0;
            }

            //nInventory.Items[Main.Players[player].UUID].Add(item);

            GUI.Dashboard.Update(player, item, nInventory.Items[Main.Players[player].UUID].IndexOf(item));
        }

        public static void SetMask(Player player, int variation, int texture)
        {
            if (variation == 0)
            {
                player.SetSharedData("IS_MASK", false);
                ApplyCharacterFace(player);

                player.SetClothes(2, CustomPlayerData[Main.Players[player].UUID].Hair.Hair, 0);
                NAPI.Player.SetPlayerHairColor(player, (byte)CustomPlayerData[Main.Players[player].UUID].Hair.Color, (byte)CustomPlayerData[Main.Players[player].UUID].Hair.HighlightColor);
            }
            else
            {
                player.SetSharedData("IS_MASK", true);
                ApplyMaskFace(player);
            }
            player.SetClothes(1, variation, texture);
        }

        public static void ApplyMaskFace(Player player)
        {
            var parents = CustomPlayerData[Main.Players[player].UUID].Parents;
            var headBlend = new HeadBlend();
            headBlend.ShapeFirst = (byte)parents.Mother;
            headBlend.ShapeSecond = (byte)parents.Father;
            headBlend.ShapeThird = 0;

            headBlend.SkinFirst = (byte)parents.Mother;
            headBlend.SkinSecond = (byte)parents.Father;
            headBlend.SkinThird = 0;

            headBlend.ShapeMix = 0.0f;
            headBlend.SkinMix = parents.SkinSimilarity;
            headBlend.ThirdMix = 0.0f;

            NAPI.Player.SetPlayerHeadBlend(player, headBlend);

            NAPI.Player.SetPlayerFaceFeature(player, 0, -1.5f);
            NAPI.Player.SetPlayerFaceFeature(player, 2, 1.5f);
            NAPI.Player.SetPlayerFaceFeature(player, 9, -1.5f);
            NAPI.Player.SetPlayerFaceFeature(player, 10, -1.5f);
            NAPI.Player.SetPlayerFaceFeature(player, 13, -1.5f);
            NAPI.Player.SetPlayerFaceFeature(player, 14, -1.5f);
            NAPI.Player.SetPlayerFaceFeature(player, 15, -1.5f);
            NAPI.Player.SetPlayerFaceFeature(player, 16, -1.5f);
            NAPI.Player.SetPlayerFaceFeature(player, 17, -1.5f);
            NAPI.Player.SetPlayerFaceFeature(player, 18, 1.5f);

            for (int i = 0; i < CustomPlayerData[Main.Players[player].UUID].Appearance.Count(); i++)
            {
                if (i != 2 && i != 10)
                {
                    var headOverlay = new HeadOverlay();
                    headOverlay.Index = 255;
                    headOverlay.Opacity = 0;
                    headOverlay.SecondaryColor = 100;
                    NAPI.Player.SetPlayerHeadOverlay(player, i, headOverlay);
                }
            }
        }

        public static void SetHat(Player player, int variation, int texture)
        {
            player.SetAccessories(0, variation, texture);
            player.SetSharedData("HAT_DATA", JsonConvert.SerializeObject(new List<int>() { variation, texture }));
        }
        #endregion

        #region Events

        [RemoteEvent("SaveCharacter")]
        public void ClientEvent_saveCharacter(Player player, params object[] args)
        {
            try
            {
                if (args.Length < 8 || !CustomPlayerData.ContainsKey(Main.Players[player].UUID)) return;

                //player.SetDefaultClothes();

                // gender
                var gender = (Convert.ToInt32(args[0]) == 0); // 0 мужчина 1 женщина

                Log.Write("GENDER: SAVE CHARACTER: "+gender+" conv: "+ Convert.ToInt32(args[0]) + " non conv" + args[0] + " ->>> 0 мужчина 1 женщина");

                var isChanging = player.HasMyData("CHANGING_CHARACTER");
                var genderChanged = true;
                if (isChanging && Main.Players[player].Gender == gender) genderChanged = false;

                Main.Players[player].Gender = gender;
                var skin = (Main.Players[player].Gender) ? "FreemodeMale01" : "FreemodeFemale01";

                Log.Write("GENDER: skin:" + skin);

                // Gender
                CustomPlayerData[Main.Players[player].UUID].Gender = Convert.ToInt32(args[0]);

                // parents
                CustomPlayerData[Main.Players[player].UUID].Parents.Father = Convert.ToInt32(args[1]);
                CustomPlayerData[Main.Players[player].UUID].Parents.Mother = Convert.ToInt32(args[2]);
                CustomPlayerData[Main.Players[player].UUID].Parents.Similarity = (float)Convert.ToDouble(args[3]);
                CustomPlayerData[Main.Players[player].UUID].Parents.SkinSimilarity = (float)Convert.ToDouble(args[4]);

                // features
                float[] feature_data = JsonConvert.DeserializeObject<float[]>(args[5].ToString());
                CustomPlayerData[Main.Players[player].UUID].Features = feature_data;

                // appearance
                AppearanceItem[] appearance_data = JsonConvert.DeserializeObject<AppearanceItem[]>(args[6].ToString());
                CustomPlayerData[Main.Players[player].UUID].Appearance = appearance_data;

                // hair & colors
                int[] hair_and_color_data = JsonConvert.DeserializeObject<int[]>(args[7].ToString());
                for (int i = 0; i < hair_and_color_data.Length; i++)
                {
                    switch (i)
                    {
                        // Hair
                        case 0:
                            {
                                CustomPlayerData[Main.Players[player].UUID].Hair.Hair = hair_and_color_data[i];
                                break;
                            }

                        // Hair Color
                        case 1:
                            {
                                CustomPlayerData[Main.Players[player].UUID].Hair.Color = hair_and_color_data[i];
                                break;
                            }

                        // Hair Highlight Color
                        case 2:
                            {
                                CustomPlayerData[Main.Players[player].UUID].Hair.HighlightColor = hair_and_color_data[i];
                                break;
                            }

                        // Eyebrow Color
                        case 3:
                            {
                                CustomPlayerData[Main.Players[player].UUID].EyebrowColor = hair_and_color_data[i];
                                break;
                            }

                        // Beard Color
                        case 4:
                            {
                                CustomPlayerData[Main.Players[player].UUID].BeardColor = hair_and_color_data[i];
                                break;
                            }

                        // Eye Color
                        case 5:
                            {
                                CustomPlayerData[Main.Players[player].UUID].EyeColor = hair_and_color_data[i];
                                break;
                            }

                        // Blush Color
                        case 6:
                            {
                                CustomPlayerData[Main.Players[player].UUID].BlushColor = hair_and_color_data[i];
                                break;
                            }

                        // Lipstick Color
                        case 7:
                            {
                                CustomPlayerData[Main.Players[player].UUID].LipstickColor = hair_and_color_data[i];
                                break;
                            }

                        // Chest Hair Color
                        case 8:
                            {
                                CustomPlayerData[Main.Players[player].UUID].ChestHairColor = hair_and_color_data[i];
                                break;
                            }
                    }
                }

                if (!gender)
                {
                    CustomPlayerData[Main.Players[player].UUID].Clothes.Leg = new ComponentItem(10, 0);
                    CustomPlayerData[Main.Players[player].UUID].Clothes.Feet = new ComponentItem(35, 0);
                }

                // clothes

                try
                {
                    if (!genderChanged && isChanging && player.HasMyData("CHANGING_TATTOOS"))
                        CustomPlayerData[Main.Players[player].UUID].Tattoos = player.GetMyData<Dictionary<int, List<Tattoo>>>("CHANGING_TATTOOS");
                }
                catch (Exception e){ Log.Write(e.StackTrace, nLog.Type.Error); }

                if (genderChanged)
                {
                    //if (!isChanging) nInventory.ClearAllClothes(player);

                    int color = 0;
                    // Выдача одежды при спавне
                    switch (Main.Players[player].Gender)
                    {
                        case true: // Мужик
                            color = Main.rnd.Next(0, 10);
                            AddClothes(player, ItemType.Top, 26, color, true);
                            CustomPlayerData[Main.Players[player].UUID].Clothes.Top = new ComponentItem(26, color);
                            color = Main.rnd.Next(0, 6);
                            AddClothes(player, ItemType.Leg, 103, color, true);
                            CustomPlayerData[Main.Players[player].UUID].Clothes.Leg = new ComponentItem(103, color);
                            color = Main.rnd.Next(0, 4);
                            AddClothes(player, ItemType.Feet, 5, color, true);
                            CustomPlayerData[Main.Players[player].UUID].Clothes.Feet = new ComponentItem(5, color);
                            break;
                        case false: // Женщина
                            color = Main.rnd.Next(0, 6);
                            AddClothes(player, ItemType.Top, 27, color, true);
                            CustomPlayerData[Main.Players[player].UUID].Clothes.Top = new ComponentItem(27, color);
                            color = Main.rnd.Next(0, 16);
                            AddClothes(player, ItemType.Leg, 4, color, true);
                            CustomPlayerData[Main.Players[player].UUID].Clothes.Leg = new ComponentItem(4, color);
                            color = Main.rnd.Next(0, 16);
                            AddClothes(player, ItemType.Feet, 13, color, true);
                            CustomPlayerData[Main.Players[player].UUID].Clothes.Feet = new ComponentItem(13, color);
                            break;
                    }
                }

                if (!isChanging)
                {
                    player.SendChatMessage("~y~Добро пожаловать в штат!");
                    //if (!Main.Accounts[player].PresentGet && Main.Accounts[player].PromoCodes[0].Equals("loveu"))
                    //{
                    //    Main.Accounts[player].PresentGet = true;
                    //    GameLog.Money($"server", $"player({Main.Players[player].UUID})", 7500, $"loveu");
                    //    AddClothes(player, ItemType.Hat, 44, 3);
                    //    nInventory.Add(player, new nItem(ItemType.Sprunk, 3));
                    //    nInventory.Add(player, new nItem(ItemType.Сrisps, 3));
                    //    Main.Players[player].LVL = 1;
                    //    MoneySystem.Wallet.Change(player, 5000);
                    //    Main.Accounts[player].VipLvl = 3;
                    //    Main.Accounts[player].VipDate = DateTime.Now.AddDays(3);
                    //    GUI.Dashboard.sendStats(player);
                    //    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Добро пожаловать в штат! Вы получили первый уровень, Gold VIP на 3 дня и 5000$!", 6000);
                    //    NAPI.Task.Run(() => { try { Trigger.ClientEvent(player, "disabledmg", false); } catch (Exception e){ Log.Write(e.StackTrace, nLog.Type.Error); } }, 5000);
                    //}
                }
                CustomPlayerData[Main.Players[player].UUID].IsCreated = true;

                SendBackToWorld(player);
                ApplyCharacter(player);
                SaveCharacter(player);
                Trigger.ClientEvent(player, "stopAndStartScreenEffect", "MinigameTransitionIn", "MinigameTransitionOut", 0, false);
                GUI.Dashboard.PsendItems(player, nInventory.Items[Main.Players[player].UUID], 2);
                return;
            }
            catch (Exception e) { Log.Write("SaveCharacter: " + e.StackTrace, nLog.Type.Error); }
        }
        #endregion

        /*public static void changeName(string oldName, string newName)
        {
            lock (CustomPlayerData)
            {
                if (!CustomPlayerData.ContainsKey(oldName))
                {
                    Log.Write($"Can't find old name! [{oldName}]", nLog.Type.Warn);
                    return;
                }
                PlayerCustomization pc = CustomPlayerData[oldName];
                CustomPlayerData.Add(newName, pc);
                CustomPlayerData.Remove(oldName);
                // // //
                MySQL.Query($"UPDATE `customization` SET `name`='{newName}' WHERE `name`='{oldName}'");
            }
        }*/
    }
}
