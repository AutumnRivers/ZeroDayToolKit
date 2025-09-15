using Hacknet;
using System.Collections.Generic;

namespace ZeroDayToolKit.Patches
{
    [HarmonyLib.HarmonyPatch(typeof(TrackerCompleteSequence), nameof(TrackerCompleteSequence.CompShouldStartTrackerFromLogs))] // stricter log control
    public class TrackerCheckLogs
    {
        public static List<Computer> stricts = [];
        static bool Prefix(object osobj, Computer c, string targetIP, ref bool __result)
        {
            OS os = (OS)osobj;
            Folder log = c.files.root.searchForFolder("log");
            targetIP ??= os.thisComputer.ip;
            foreach (FileEntry file in log.files)
            {
                string data = file.data;
                if (data.Contains(targetIP) &&
                    stricts.Contains(c) &&
                    (data.Contains("Connection") || data.Contains("Disconnected")))
                {
                    __result = true; // tracker *should* fire
                    return false;
                }
            }
            return true; // let hacknet do its vanilla logic
        }

        public static void Init()
        {
            
        }
    }
}
