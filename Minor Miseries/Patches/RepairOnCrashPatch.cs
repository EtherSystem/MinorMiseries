//using AfflictionComponent.Components;

//namespace Minor_Miseries.Patches
//{
//    internal static class AfflictionComponentCrashGuards
//    {
//        private static bool _loggedColorGuard;
//        private static bool _loggedBodyGuard;

//        private static bool IsCustomIndexValid(int idx, out int customCount)
//        {
//            var mgr = AfflictionManager.GetAfflictionManagerInstance();
//            customCount = mgr?.m_Afflictions?.Count ?? 0;
//            return idx >= 0 && idx < customCount;
//        }

//        [HarmonyPatch]
//        internal static class Guard_AC_GetColorBasedOnAffliction_Postfix
//        {
//            private static MethodBase TargetMethod()
//            {
//                var t = AccessTools.TypeByName("AfflictionComponent.Patches.AfflictionButtonPatches.GetColorBasedOnAffliction+GetColorBasedOnCustomAffliction");
//                return AccessTools.Method(t, "Postfix");
//            }

//            private static bool Prefix(AfflictionButton __instance, AfflictionType m_AfflictionType)
//            {
//                if (m_AfflictionType != AfflictionType.Generic) return true;

//                int idx = __instance?.m_Index ?? -1;
//                if (!IsCustomIndexValid(idx, out int customCount))
//                {
//                    if (!_loggedColorGuard)
//                    {
//                        _loggedColorGuard = true;
//                        MelonLogger.Warning($"Skip GetColorBasedOnCustomAffliction.Postfix (idx={idx}, customCount={customCount})");
//                    }
//                    return false;
//                }
//                return true;
//            }
//        }

//        [HarmonyPatch]
//        internal static class Guard_AC_UpdateBodyIconColors_Postfix
//        {
//            private static MethodBase TargetMethod()
//            {
//                var t = AccessTools.TypeByName("AfflictionComponent.Patches.PanelFirstAidPatches.UpdateBodyIconColors+OverrideUpdateBodyIconColors");
//                return AccessTools.Method(t, "Postfix");
//            }

//            private static bool Prefix(Panel_FirstAid __instance, AfflictionButton afflictionButton, int bodyIconIndex)
//            {
//                if (__instance == null || afflictionButton == null) return false;

//                if (afflictionButton.m_AfflictionType != AfflictionType.Generic) return true;

//                int iconCount = __instance.m_BodyIconList?.Count ?? 0;
//                if (bodyIconIndex < 0 || bodyIconIndex >= iconCount)
//                {
//                    if (!_loggedBodyGuard)
//                    {
//                        _loggedBodyGuard = true;
//                        MelonLogger.Warning($"Skip UpdateBodyIconColors.Postfix (bodyIconIndex={bodyIconIndex}, iconCount={iconCount})");
//                    }
//                    return false;
//                }

//                int idx = afflictionButton.m_Index;
//                if (!IsCustomIndexValid(idx, out int customCount))
//                {
//                    if (!_loggedBodyGuard)
//                    {
//                        _loggedBodyGuard = true;
//                        MelonLogger.Warning($"Skip UpdateBodyIconColors.Postfix (idx={idx}, customCount={customCount})");
//                    }
//                    return false;
//                }
//                return true;
//            }
//        }
//    }
//}