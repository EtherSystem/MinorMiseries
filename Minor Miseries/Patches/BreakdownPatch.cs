using static Minor_Miseries.Afflictions.Overconfidence;
using static Minor_Miseries.Afflictions.Splinter;
using Random = UnityEngine.Random;

namespace Minor_Miseries.Patches
{
    internal class BreakdownPatches
    {
        public static float BASE_SPLINTER_CHANCE = 10f;
        public static float OVERC_SPLINTER_CHANCE = 20f;

        [HarmonyPatch(typeof(Panel_BreakDown), nameof(Panel_BreakDown.OnBreakDown))]
        internal static class OnBreakDownPatch
        {
            public static void Postfix()
            {
                var pm = GameManager.GetPlayerManagerComponent();
                if (pm == null) return;
                bool noGloves = pm.GetClothingInSlot(ClothingRegion.Hands, ClothingLayer.Base) == null;
                if (Settings.options.IsSplinter && noGloves)
                {
                    float roll = Random.Range(0f, 100f);
                    if (OverconfidenceAffliction.IsActive)
                    {
                        if (roll < OVERC_SPLINTER_CHANCE)
                        {
                            //MelonLogger.Msg("an overconfidente splinter has been applied");
                            new SplinterAffliction(AfflictionBodyArea.HandLeft).Start();
                        }
                    }
                    else
                    {
                        if (roll < BASE_SPLINTER_CHANCE)
                        {
                            //MelonLogger.Msg("a splinter has been applied");
                            new SplinterAffliction(AfflictionBodyArea.HandLeft).Start();
                        }
                    }
                }
            }
        }
    }
}