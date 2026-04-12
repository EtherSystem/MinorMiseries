using static Minor_Miseries.Afflictions.ShoulderTrauma;
using static Minor_Miseries.Afflictions.WristTrauma;
using static Minor_Miseries.Afflictions.Overconfidence;

namespace Minor_Miseries.Patches
{
    internal class FirearmsPatch
    {
        private static uint? _pendingAfflictionSound;

        public static void QueueAfflictionSound(uint eventId)
        {
            _pendingAfflictionSound = eventId;
        }

        public static void FlushPendingAfflictionSound()
        {
            if (!_pendingAfflictionSound.HasValue)
                return;

            var player = GameManager.GetPlayerObject();
            if (player == null)
                return;

            GameAudioManager.PlaySound(_pendingAfflictionSound.Value, player);
            _pendingAfflictionSound = null;
        }

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

                float chance = GetChance(skill.GetCurrentTierNumber());

                if (OverconfidenceAffliction.IsActive)
                    chance *= 2f;

                chance = Mathf.Clamp(chance, 0f, 100f);

                float roll = UnityEngine.Random.Range(0f, 100f);
                if (roll >= chance) return;

                new WristTraumaAffliction(AfflictionBodyArea.HandRight).Start();
                QueueAfflictionSound(Il2CppAK.EVENTS.PLAY_PLAYERDAMAGE);
                AfflictionSaveHelper.QueueSurvivalSave();
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

                float chance = GetChance(skill.GetCurrentTierNumber());

                if (OverconfidenceAffliction.IsActive)
                    chance *= 2f;

                chance = Mathf.Clamp(chance, 0f, 100f);

                float roll = UnityEngine.Random.Range(0f, 100f);
                if (roll >= chance) return;

                new ShoulderTraumaAffliction(AfflictionBodyArea.Chest).Start();
                QueueAfflictionSound(Il2CppAK.EVENTS.PLAY_EXERTIONMEDIUM);
                AfflictionSaveHelper.QueueSurvivalSave();
            }
        }

        private static float GetChance(int level)
        {
            if (level <= 1) return 16f;
            if (level == 2) return 8f;
            if (level == 3) return 4f;
            if (level == 4) return 2f;
            return 0f;
        }
    }
}