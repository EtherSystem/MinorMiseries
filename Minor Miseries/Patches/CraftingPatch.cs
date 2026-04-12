using static Minor_Miseries.Afflictions.Overconfidence;
using static Minor_Miseries.Afflictions.Scratch;
using Random = UnityEngine.Random;

namespace Minor_Miseries.Patches
{
    internal class CraftingPatch
    {
        private const float DEFAULT_SCRATCH_CHANCE = 10f;

        private static readonly Dictionary<string, float> ScratchChanceByTool = new(StringComparer.Ordinal)
        {
            ["GEAR_WoodworkingTools"] = 2f,
            ["GEAR_SewingKit"] = 5f,
            ["GEAR_HookAndLine"] = 6f,
            ["GEAR_HighQualityTools"] = 7f,
            ["GEAR_SurvivalKnife"] = 8f,
            ["GEAR_SimpleTools"] = 9f,
            ["GEAR_Knife"] = 10f,
            ["GEAR_Hatchet"] = 12f,
            ["GEAR_CougarClawKnife"] = 14f,
            ["GEAR_HatchetImprovised"] = 15f,
            ["GEAR_KnifeImprovised"] = 16f
        };

        private static string _activeCraftToolId;

        [HarmonyPatch(typeof(Panel_Crafting), nameof(Panel_Crafting.Update))]
        internal static class CraftingToolTrackerPatch
        {
            public static void Postfix(Panel_Crafting __instance)
            {
                if (__instance == null) return;
                if (__instance.m_CraftingOperation == null) return;
                if (!__instance.m_CraftingOperation.InProgress) return;

                string toolId = GetSelectedCraftingToolId(__instance);
                if (!string.IsNullOrWhiteSpace(toolId) && toolId != _activeCraftToolId)
                {
                    _activeCraftToolId = toolId;
                    Core.Log($"Tracked crafting tool: {_activeCraftToolId}");
                }
            }
        }

        [HarmonyPatch(typeof(Panel_Crafting), nameof(Panel_Crafting.OnCraftingSuccess))]
        internal static class OnCraftingSucessPatch
        {
            public static void Postfix()
            {
                if (!Settings.options.IsScratch)
                {
                    _activeCraftToolId = null;
                    return;
                }

                float chance = GetScratchChanceFromToolId(_activeCraftToolId);

                if (OverconfidenceAffliction.IsActive)
                    chance *= 2f;

                chance = Mathf.Clamp(chance, 0f, 100f);

                string toolId = _activeCraftToolId ?? "None";
                Core.Log($"Scratch success chance: {toolId} -> {chance:0.##}%");

                TryResolveScratch(chance);
                _activeCraftToolId = null;
            }
        }

        [HarmonyPatch(typeof(Panel_Crafting), nameof(Panel_Crafting.OnCraftingInterrupted))]
        internal static class OnCraftingInterrupted
        {
            public static void Postfix()
            {
                if (!Settings.options.IsScratch)
                {
                    _activeCraftToolId = null;
                    return;
                }

                float chance = GetScratchChanceFromToolId(_activeCraftToolId) / 2f;

                if (OverconfidenceAffliction.IsActive)
                    chance *= 2f;

                chance = Mathf.Clamp(chance, 0f, 100f);

                string toolId = _activeCraftToolId ?? "None";
                Core.Log($"Scratch interrupted chance: {toolId} -> {chance:0.##}%");

                TryResolveScratch(chance);
                _activeCraftToolId = null;
            }
        }

        private static float GetScratchChanceFromToolId(string toolId)
        {
            if (string.IsNullOrWhiteSpace(toolId))
                return DEFAULT_SCRATCH_CHANCE;

            return ScratchChanceByTool.TryGetValue(toolId, out float chance)
                ? chance
                : DEFAULT_SCRATCH_CHANCE;
        }

        private static string GetSelectedCraftingToolId(Panel_Crafting panel)
        {
            var tool = panel?.m_RequirementContainer?.GetSelectedTool();
            return tool?.name;
        }

        private static void TryResolveScratch(float chance)
        {
            float roll = Random.Range(0f, 100f);
            if (roll >= chance) return;

            if (BuffLogic.TryAbsorbScratchWithProtectedArms())
            {
                AfflictionSaveHelper.QueueSurvivalSave();
                return;
            }

            new ScratchAffliction(AfflictionBodyArea.ArmLeft).Start();
            GameAudioManager.PlaySound(Il2CppAK.EVENTS.PLAY_EXERTIONLOW, GameManager.GetPlayerObject());
            AfflictionSaveHelper.QueueSurvivalSave();
        }
    }
}