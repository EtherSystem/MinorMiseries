using static Minor_Miseries.Afflictions.ShoulderTrauma;
using static Minor_Miseries.Afflictions.WristTrauma;
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
                if (!Settings.options.IsWristTrauma) return;

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

                if (OverconfidenceAffliction.IsActive)
                {
                    chance *= 2f;
                }

                float roll = UnityEngine.Random.Range(0f, 100f);

                if (roll < chance)
                {
                    new WristTraumaAffliction(AfflictionBodyArea.HandRight).Start();
                }
            }
        }

        [HarmonyPatch(typeof(GunItem), nameof(GunItem.Fired))]
        internal static class RifleFiredPatch
        {
            private static void Postfix(GunItem __instance)
            {
                if (!Settings.options.IsShoulderTrauma) return;

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

                if (OverconfidenceAffliction.IsActive)
                {
                    chance *= 2f;
                }

                float roll = UnityEngine.Random.Range(0f, 100f);

                if (roll < chance)
                {
                    new ShoulderTraumaAffliction(AfflictionBodyArea.Chest).Start();
                }
            }
        }
    }
}