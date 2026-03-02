using static Minor_Miseries.Afflictions.Overconfidence;
using static Minor_Miseries.Afflictions.Scratch;
using Random = UnityEngine.Random;

namespace Minor_Miseries.Patches
{
    internal class CraftingPatch
    {
        public static float BASE_SUCCESS_SCRATCH_CHANCE = 10f;
        public static float OVERC_SUCCESS_SCRATCH_CHANCE = 20f;

        [HarmonyPatch(typeof(Panel_Crafting), nameof(Panel_Crafting.OnCraftingSuccess))]
        internal static class OnCraftingSucessPatch
        {
            public static void Postfix()
            {
                if (Settings.options.IsScratch)
                {
                    float roll = Random.Range(0f, 100f);
                    if (OverconfidenceAffliction.IsActive)
                    {
                        if (roll < OVERC_SUCCESS_SCRATCH_CHANCE)
                        {
                            //MelonLogger.Msg("you have an overconfidente scratch");
                            new ScratchAffliction(AfflictionBodyArea.Chest).Start();
                        }
                    }
                    else
                    {
                        if (roll < BASE_SUCCESS_SCRATCH_CHANCE)
                        {
                            //MelonLogger.Msg("you have a scratch");
                            new ScratchAffliction(AfflictionBodyArea.Chest).Start();
                        }
                    }
                }
            }
        }

        public static float BASE_INTERRUPTED_SCRATCH_CHANCE = 5f;
        public static float OVERC_INTERRUPTED_SCRATCH_CHANCE = 10f;

        [HarmonyPatch(typeof(Panel_Crafting), nameof(Panel_Crafting.OnCraftingInterrupted))]
        internal static class OnCraftingInterrupted
        {
            public static void Postfix()
            {
                if (Settings.options.IsScratch)
                {
                    float roll = Random.Range(0f, 100f);
                    if (OverconfidenceAffliction.IsActive)
                    {
                        if (roll < OVERC_INTERRUPTED_SCRATCH_CHANCE)
                        {
                            //MelonLogger.Msg("you have an overconfidente scratch");
                            new ScratchAffliction(AfflictionBodyArea.Chest).Start();
                        }
                    }
                    else
                    {
                        if (roll < BASE_INTERRUPTED_SCRATCH_CHANCE)
                        {
                            //MelonLogger.Msg("you have a scratch");
                            new ScratchAffliction(AfflictionBodyArea.Chest).Start();
                        }
                    }
                }
            }
        }
    }
}