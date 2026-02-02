namespace Minor_Miseries.Patches
{
    [HarmonyPatch(typeof(Panel_FirstAid), nameof(Panel_FirstAid.ClearAfflictionsAtLocationArray))]
    internal static class FirstAid_Clear_RepairOnCrash
    {
        private static bool _loggedOnce;

        private static Exception Finalizer(Panel_FirstAid __instance, Exception __exception)
        {
            if (__exception == null) return null;

            try
            {
                int icons = __instance?.m_BodyIconList != null ? __instance.m_BodyIconList.Count : -1;
                int arrLen = __instance?.m_AfflictionsAtLocationArray != null ? __instance.m_AfflictionsAtLocationArray.Length : -1;

                if (!_loggedOnce)
                {
                    _loggedOnce = true;
                    MelonLogger.Error($"[FirstAidRepair] ClearAfflictionsAtLocationArray crashed -> repairing. icons={icons} arrLen={arrLen} ({__exception.GetType().Name})");
                }

                if (__instance != null && icons > 0)
                {
                    __instance.m_AfflictionsAtLocationArray = new Panel_FirstAid.AfflictionsAtLocation[icons];
                    for (int i = 0; i < icons; i++)
                        __instance.m_AfflictionsAtLocationArray[i] = new Panel_FirstAid.AfflictionsAtLocation((AfflictionBodyArea)i);
                }
            }
            catch (Exception e)
            {
                MelonLogger.Warning($"[FirstAidRepair] Repair failed: {e.GetType().Name} - {e.Message}");
            }

            return null;
        }
    }
}