using static Minor_Miseries.Afflictions.Splinter;
using Random = UnityEngine.Random;

namespace Minor_Miseries.Patches
{
    internal class BreakdownPatches
    {
        public static float BASE_SPLINTER_CHANCE = 10f;
        public static float OVERC_SPLINTER_CHANCE = 20f;
        private static readonly bool IsOvercActive = Afflictions.Overconfidence.OverconfidenceAffliction.IsOvercActive;

        [HarmonyPatch(typeof(Panel_BreakDown), nameof(Panel_BreakDown.OnBreakDown))]
        internal static class OnBreakDownPatch
        {
            public static void Postfix()
            {
                bool noGloves = GameManager.GetPlayerManagerComponent().GetClothingInSlot(ClothingRegion.Hands, ClothingLayer.Base) == null;
                if (Settings.options.IsSplinter && noGloves)
                {
                    float roll = Random.Range(0f, 100f);
                    if (IsOvercActive)
                    {
                        if (roll < OVERC_SPLINTER_CHANCE)
                        {
                            var side = Random.Range(0, 2) == 0
                                ? AfflictionBodyArea.HandLeft : AfflictionBodyArea.HandRight;
                            //MelonLogger.Msg("an overconfidente splinter has been applied");
                            new SplinterAffliction(side).Start();
                        }
                    }
                    else
                    {
                        if (roll < BASE_SPLINTER_CHANCE)
                        {
                            var side = Random.Range(0, 2) == 0
                                ? AfflictionBodyArea.HandLeft : AfflictionBodyArea.HandRight;
                            //MelonLogger.Msg("a splinter has been applied");
                            new SplinterAffliction(side).Start();
                        }
                    }
                }
            }
        }
    }
}