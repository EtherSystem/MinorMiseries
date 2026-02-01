using AfflictionComponent.Components;
using AfflictionComponent.Interfaces;
using AfflictionComponent.Enums;

namespace Minor_Miseries.Afflictions
{
    internal class Scratch
    {
        public class ScratchAffliction : CustomAffliction, IDuration, IRemedies, IInstance
        {
            private static readonly bool IsOvercActive = Overconfidence.OverconfidenceAffliction.IsOvercActive;
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

            private readonly float m_LastUpdateTime;
            public static bool IsScratchActive { get; private set; } = false;
            public float Duration { get; set; } = Settings.options.ScratchDuration;
            public float EndTime { get; set; }

            public Tuple<string, int, int>[] RemedyItems { get; set; } = Array.Empty<Tuple<string, int, int>>();
            public Tuple<string, int, int>[] AltRemedyItems { get; set; } = Array.Empty<Tuple<string, int, int>>();

            public bool InstantHeal { get; set; } = true;

            public ScratchAffliction(AfflictionBodyArea bodyArea) : base("Scratch", "Awkward gesture", "You've scratched your skin, nothing too serious", null, "ico_injury_minorBruising", bodyArea) //customsprite :Minor_Miseries.Resources.Icons.Scratch.png
            {
                m_LastUpdateTime = GameManager.GetTimeOfDayComponent().GetHoursPlayedNotPaused();
            }

            public void CureSymptoms()
            {
                //cure symptoms but not the affliction
            }

            public void OnCure()
            {
                IsScratchActive = false;
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

                    if (IsScratchActive)
                    {
                        if (IsOvercActive)
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