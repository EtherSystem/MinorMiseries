using static Minor_Miseries.Afflictions.Overconfidence;
using static Minor_Miseries.Afflictions.SmallCut;
using AfflictionComponent.Interfaces;
using AfflictionComponent.Components;
using Random = UnityEngine.Random;
using AfflictionComponent.Enums;

namespace Minor_Miseries.Afflictions
{
    internal class Scratch
    {
        public class ScratchAffliction : CustomAffliction, IDuration, IRemedies, IInstance
        {
            public InstanceType Type { get; set; } = InstanceType.Single;
            public void OnFoundExistingInstance(CustomAffliction existingAffliction)
            {
                //MelonLogger.Msg("scratch duplication");
                if (existingAffliction is ScratchAffliction scratch)
                {
                    scratch.ResetAffliction(resetRemedies: false);
                    var now = GameManager.GetTimeOfDayComponent().GetHoursPlayedNotPaused();
                    scratch.EndTime = now + scratch.Duration;
                }
            }

            private bool m_SymptomsCured = false;
            public static float SCRATCH_EVOLV_CHANCE = 40f;
            public static bool IsScratchActive { get; private set; } = false;
            public float Duration { get; set; } = Settings.options.ScratchDuration;
            public float EndTime { get; set; }

            public Tuple<string, int, int>[] RemedyItems { get; set; } =
            {
                Tuple.Create("GEAR_HeavyBandage", 1, 1)
            };
            public Tuple<string, int, int>[] AltRemedyItems { get; set; } = Array.Empty<Tuple<string, int, int>>();

            public bool InstantHeal { get; set; } = false;

            public ScratchAffliction(AfflictionBodyArea bodyArea) : base("Scratch", "Awkward gesture", "You've scratched your skin, nothing too serious", null, "ico_injury_minorBruising", bodyArea) //customsprite :Minor_Miseries.Resources.Icons.Scratch.png
            {
            }

            public void CureSymptoms()
            {
                m_SymptomsCured = true;
            }

            public void OnCure()
            {
                IsScratchActive = false;
                float roll = Random.Range(0f, 100f);
                if (!m_SymptomsCured && Settings.options.IsSmallCut && (roll < SCRATCH_EVOLV_CHANCE))
                {
                    new SmallCutAffliction(AfflictionBodyArea.Chest).Start();
                }
                m_SymptomsCured = false;
            }

            public override void OnUpdate()
            {
                IsScratchActive = true;
            }

            [HarmonyPatch(typeof(Panel_Crafting), nameof(Panel_Crafting.GetModifiedCraftingDuration))]
            private static class CraftingDurationPatch
            {
                private static void Postfix(ref int __result)
                {
                    if (!IsScratchActive) return;

                    var afflictionManager = AfflictionManager.GetAfflictionManagerInstance();
                    if (afflictionManager == null || afflictionManager.m_Afflictions == null) return;

                    foreach (var affliction in afflictionManager.m_Afflictions)
                    {
                        if (affliction is ScratchAffliction scratchAffliction)
                        {
                            if (scratchAffliction.m_SymptomsCured) return;

                            if (OverconfidenceAffliction.IsOvercActive)
                            {
                                __result = (int)(__result * 1.2f);
                            }
                            else
                            {
                                __result = (int)(__result * 1.1f);
                            }
                        }
                    }
                }
            }
        }
    }
}