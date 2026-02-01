using AfflictionComponent.Components;
using AfflictionComponent.Interfaces;
using AfflictionComponent.Enums;

namespace Minor_Miseries.Afflictions
{
    internal class Splinter
    {
        public class SplinterAffliction : CustomAffliction, IDuration, IRemedies, IInstance
        {
            private static readonly bool IsOvercActive = Overconfidence.OverconfidenceAffliction.IsOvercActive;
            public InstanceType Type { get; set; } = InstanceType.Single;
            public void OnFoundExistingInstance(CustomAffliction existingAffliction)
            {
                //MelonLogger.Msg("splinter duplication");
                if (existingAffliction is SplinterAffliction splinter)
                {
                    splinter.ResetAffliction(resetRemedies: false);
                    var now = GameManager.GetTimeOfDayComponent().GetHoursPlayedNotPaused();
                    splinter.EndTime = now + splinter.Duration;
                }
            }

            private readonly float m_LastUpdateTime;
            public static bool IsSplinterActive { get; private set; } = false;
            public float Duration { get; set; } = Settings.options.SplinterDuration;
            public float EndTime { get; set; }

            public Tuple<string, int, int>[] RemedyItems { get; set; } = Array.Empty<Tuple<string, int, int>>();
            public Tuple<string, int, int>[] AltRemedyItems { get; set; } = Array.Empty<Tuple<string, int, int>>();

            public bool InstantHeal { get; set; } = true;

            public SplinterAffliction(AfflictionBodyArea bodyArea): base("Splinter", "Unprotected hands", "A splinter is lodged in your skin", null, "ico_injury_sprainedWrist", bodyArea) //customsprite :Minor_Miseries.Resources.Icons.Splinter.png
            {
                m_LastUpdateTime = GameManager.GetTimeOfDayComponent().GetHoursPlayedNotPaused();
            }

            public void CureSymptoms()
            {
                //cure symptoms but not the affliction
            }

            public void OnCure()
            {
                IsSplinterActive = false;
            }

            public override void OnUpdate()
            {
                IsSplinterActive = true;
            }

            [HarmonyPatch(typeof(Panel_Crafting), nameof(Panel_Crafting.GetModifiedCraftingDuration))]
            private static class CraftingDurationPatch
            {
                private static void Postfix(ref int __result)
                {
                    if (!IsSplinterActive) return;

                    if (IsSplinterActive)
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
            [HarmonyPatch(typeof(vp_FPSController), nameof(vp_FPSController.GetSlopeMultiplier))]
            internal static class MovementSpeedPatch
            {
                private static void Postfix(ref float __result)
                {
                    if (!IsSplinterActive) return;

                    var pm = GameManager.GetPlayerManagerComponent();
                    if (IsSplinterActive && (pm.PlayerIsClimbing()))
                    {
                        if (IsOvercActive)
                        {
                            __result *= 0.8f;
                        }
                        else
                        {
                            __result *= 0.9f;
                        }
                    }
                }
            }
        }
    }
}