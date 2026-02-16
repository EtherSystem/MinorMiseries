using static Minor_Miseries.Afflictions.Overconfidence;
using AfflictionComponent.Interfaces;
using AfflictionComponent.Components;
using AfflictionComponent.Enums;

namespace Minor_Miseries.Afflictions
{
    internal class ShoulderRecoilTrauma
    {
        public class ShoulderRecoilInjuryAffliction : CustomAffliction, IDuration, IRemedies, IInstance
        {
            public InstanceType Type { get; set; } = InstanceType.Single;
            public void OnFoundExistingInstance(CustomAffliction existingAffliction)
            {
                //MelonLogger.Msg("Shoulder Recoil duplication");
                if (existingAffliction is ShoulderRecoilInjuryAffliction ShoulderRecoil)
                {
                    ShoulderRecoil.ResetAffliction(resetRemedies: false);
                    var now = GameManager.GetTimeOfDayComponent().GetHoursPlayedNotPaused();
                    ShoulderRecoil.EndTime = now + ShoulderRecoil.Duration;
                }
            }

            public static bool IsShoulderRecoilActive { get; private set; } = false;
            public float Duration { get; set; } = Settings.options.ShoulderRecoilDuration;
            public float EndTime { get; set; }

            public Tuple<string, int, int>[] RemedyItems { get; set; } = Array.Empty<Tuple<string, int, int>>();
            public Tuple<string, int, int>[] AltRemedyItems { get; set; } = Array.Empty<Tuple<string, int, int>>();

            public bool InstantHeal { get; set; } = true;

            public ShoulderRecoilInjuryAffliction(AfflictionBodyArea bodyArea) : base("Shoulder Trauma", "Weapon recoil", "The recoil hurt your shoulder. Aiming and handling the weapon is more difficult.", null, "ico_injury_pain", bodyArea) //customsprite :Minor_Miseries.Resources.Icons.Splinter.png
            {
            }

            public void CureSymptoms()
            {
                //apply this code when symptoms are cured
            }

            public void OnCure()
            {
                IsShoulderRecoilActive = false;
            }
            public override void OnUpdate()
            {
                IsShoulderRecoilActive = true;
            }

            [HarmonyPatch(typeof(vp_FPSController), nameof(vp_FPSController.GetSlopeMultiplier))]
            internal static class MovementSpeedPatch
            {
                private static void Postfix(ref float __result)
                {
                    if (!IsShoulderRecoilActive) return;

                    var pm = GameManager.GetPlayerManagerComponent();
                    if (pm == null) return;
                    if (IsShoulderRecoilActive && (pm.PlayerIsClimbing()))
                    {
                        if (OverconfidenceAffliction.IsOvercActive)
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

            [HarmonyPatch(typeof(Panel_Crafting), nameof(Panel_Crafting.GetModifiedCraftingDuration))]
            private static class CraftingDurationPatch
            {
                private static void Postfix(ref int __result)
                {
                    if (!IsShoulderRecoilActive) return;

                    if (IsShoulderRecoilActive)
                    {
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