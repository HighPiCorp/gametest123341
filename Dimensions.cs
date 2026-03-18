using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Text;
using NeptuneEvo.SDK;
using System.Linq;

namespace NeptuneEvo.Core
{
    class Dimensions : Script
    {
        private static nLog Log = new nLog("Dimensions");

        private static Dictionary<int, Entity> DimensionsInUse = new Dictionary<int, Entity>();
        private static ICollection<int> Keys = DimensionsInUse.Keys;

        public static uint RequestPrivateDimension(Player requester, bool get = false)
        {
            try
            {
                int firstUnusedDim = 10000;

                uint dim = 0;

                if (get)
                {
                    if (DimensionsInUse.Values.Contains(requester))
                    {
                        dim = (uint)DimensionsInUse.FirstOrDefault(d => d.Value == requester).Key;

                        if (dim == 0) Log.Write("[RequestPrivateDimension] ERROR: 0 DIM", nLog.Type.Error);

                        Log.Write($"[RequestPrivateDimension][{requester.Name}][SUCCESS] DIM:{dim}", nLog.Type.Success);

                        return dim;
                    }
                }

                lock (DimensionsInUse)
                {
                    while (DimensionsInUse.ContainsKey(--firstUnusedDim))
                    {
                    }
                    DimensionsInUse.Add(firstUnusedDim, requester);
                }



                Log.Debug($"Dimension {firstUnusedDim.ToString()} is registered for {requester.Name}.");

                dim = (uint)firstUnusedDim;

                if (dim == 0) Log.Write("[RequestPrivateDimension] ERROR: 0 DIM", nLog.Type.Error);

                return dim;
            }
            catch (Exception e) { Log.Write("RequestPrivateDimension ERROR: 0 DIM: " + e.StackTrace, nLog.Type.Error); return 0; }
        }

        public static void DismissPrivateDimension(Player requester)
        {
            try
            {
                foreach (KeyValuePair<int, Entity> dim in DimensionsInUse)
                {
                    if (dim.Value == requester.Handle)
                    {
                        DimensionsInUse.Remove(dim.Key);
                        Log.Debug($"Dimension {dim.Key} is DismissPrivateDimension for {requester.Name}.");
                        //break;
                    }
                }
            }
            catch (Exception e) { Log.Write("DismissPrivateDimension: " + e.StackTrace, nLog.Type.Error); }
        }
        public static uint GetPlayerDimension(Player player)
        {
            foreach (var key in Keys)
                if (DimensionsInUse[key] == player) return (uint)key;
            return 0;
        }
    }
}
