using static Minor_Miseries.Afflictions.ShoulderRecoilTrauma;
using static Minor_Miseries.Afflictions.WristRecoilTrauma;
using static Minor_Miseries.Afflictions.Overconfidence;

namespace Minor_Miseries.Patches
{
    internal class FirearmsPatch
    {
        [HarmonyPatch(typeof(GunItem), nameof(GunItem.Fired))]
        internal static class RevolverFiredPatch
        {
            private static void Postfix(GunItem __instance)
            {
                if (!Settings.options.IsWristRecoil) return;

                if (__instance == null) return;

                if (!__instance.name.Contains("GEAR_Revolver")) return;

                var skill = GameManager.GetSkillRevolver();
                if (skill == null) return;

                int level = skill.GetCurrentTierNumber();

                float chance = 0f;

                if (level <= 1) chance = 16f;
                else if (level == 2) chance = 8f;
                else if (level == 3) chance = 4f;
                else if (level == 4) chance = 2f;
                else if (level > 4) chance = 0f;

                if (OverconfidenceAffliction.IsOvercActive)
                {
                    chance *= 2f;
                }

                float roll = UnityEngine.Random.Range(0f, 100f);

                if (roll < chance)
                {
                    new WristRecoilInjuryAffliction(AfflictionBodyArea.HandRight).Start();
                }
            }
        }

        [HarmonyPatch(typeof(GunItem), nameof(GunItem.Fired))]
        internal static class RifleFiredPatch
        {
            private static void Postfix(GunItem __instance)
            {
                if (!Settings.options.IsShoulderRecoil) return;

                if (__instance == null) return;

                if (!__instance.name.Contains("GEAR_Rifle")) return;

                var skill = GameManager.GetSkillRifle();
                if (skill == null) return;

                int level = skill.GetCurrentTierNumber();

                float chance = 0f;

                if (level <= 1) chance = 16f;
                else if (level == 2) chance = 8f;
                else if (level == 3) chance = 4f;
                else if (level == 4) chance = 2f;
                else if (level > 4) chance = 0f;

                if (OverconfidenceAffliction.IsOvercActive)
                {
                    chance *= 2f;
                }

                float roll = UnityEngine.Random.Range(0f, 100f);

                if (roll < chance)
                {
                    new ShoulderRecoilInjuryAffliction(AfflictionBodyArea.Chest).Start();
                }
            }
        }

        [HarmonyPatch(typeof(GunItem), nameof(GunItem.Update))]
        internal static class GunAimStaminaPatch
        {
            private static void Postfix(GunItem __instance)
            {
                if (__instance == null) return;

                const float BASE_INCREASE = 0.1f;
                const float BASE_DECREASE = 0.15f;

                bool wrist = WristRecoilInjuryAffliction.IsWristRecoilActive;
                bool shoulder = ShoulderRecoilInjuryAffliction.IsShoulderRecoilActive;

                if (!wrist && !shoulder)
                {
                    __instance.m_SwayIncreasePerSecond = BASE_INCREASE;
                    __instance.m_SwayDecreasePerSecond = BASE_DECREASE;
                    return;
                }

                float increaseMult = 1f;
                float decreaseMult = 1f;

                if (wrist)
                {
                    increaseMult *= 1.6f;
                    decreaseMult *= 0.6f;
                }

                if (shoulder)
                {
                    increaseMult *= 1.8f;
                    decreaseMult *= 0.5f;
                }

                __instance.m_SwayIncreasePerSecond = BASE_INCREASE * increaseMult;
                __instance.m_SwayDecreasePerSecond = BASE_DECREASE * decreaseMult;
            }
        }
    }
}