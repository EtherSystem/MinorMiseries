using static Minor_Miseries.Afflictions.Overconfidence;
using static Minor_Miseries.Afflictions.Splinter;
using Random = UnityEngine.Random;

namespace Minor_Miseries.Patches
{
    internal class BreakdownPatches
    {
        private const float DEFAULT_SPLINTER_CHANCE = 10f;
        private const float SPLINTER_CHANCE_MULTIPLIER = 0.5f;

        private static readonly Dictionary<string, float> SplinterChanceByTool = new(StringComparer.Ordinal)
        {
            ["GEAR_Hacksaw"] = 6f,
            ["GEAR_SurvivalKnife"] = 7f,
            ["GEAR_CougarClawKnife"] = 8f,
            ["GEAR_Knife"] = 10f,
            ["GEAR_Hatchet"] = 12f,
            ["GEAR_KnifeImprovised"] = 13f,
            ["GEAR_HatchetImprovised"] = 16f
        };

        private static string _selectedBreakdownToolId;

        [HarmonyPatch(typeof(Panel_BreakDown), nameof(Panel_BreakDown.OnBreakDown))]
        internal static class OnBreakDownPatch
        {
            public static void Prefix(Panel_BreakDown __instance)
            {
                var selectedTool = __instance?.GetSelectedTool();
                _selectedBreakdownToolId = selectedTool?.name;

                Core.Log($"Selected breakdown tool: {_selectedBreakdownToolId ?? "None"}");
            }

            public static void Postfix()
            {
                if (!Settings.options.IsSplinter)
                {
                    _selectedBreakdownToolId = null;
                    return;
                }

                float chance = GetSplinterChanceFromToolId(_selectedBreakdownToolId) * SPLINTER_CHANCE_MULTIPLIER;

                if (OverconfidenceAffliction.IsActive)
                    chance *= 2f;

                chance = Mathf.Clamp(chance, 0f, 100f);

                Core.Log($"Splinter chance: {_selectedBreakdownToolId ?? "None"} -> {chance:0.##}%");

                TryResolveSplinter(chance);
                _selectedBreakdownToolId = null;
            }
        }

        private static float GetSplinterChanceFromToolId(string toolId)
        {
            if (string.IsNullOrEmpty(toolId))
                return DEFAULT_SPLINTER_CHANCE;

            return SplinterChanceByTool.TryGetValue(toolId, out float chance)
                ? chance
                : DEFAULT_SPLINTER_CHANCE;
        }

        private static void TryResolveSplinter(float chance)
        {
            float roll = Random.Range(0f, 100f);
            if (roll >= chance) return;

            if (BuffLogic.TryAbsorbSplinterWithProtectedHands()) return;

            if (!Core.TryStartCustomAffliction(new SplinterAffliction(AfflictionBodyArea.HandLeft), "Splinter from breakdown")) return;

            GameAudioManager.PlaySound(Il2CppAK.EVENTS.PLAY_VOLANDLEVEL1, GameManager.GetPlayerObject());
            AfflictionSaveHelper.QueueSurvivalSave();
        }
    }
}