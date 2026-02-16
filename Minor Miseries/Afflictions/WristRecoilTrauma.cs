using static Minor_Miseries.Afflictions.Overconfidence;
using AfflictionComponent.Interfaces;
using AfflictionComponent.Components;
using AfflictionComponent.Enums;

namespace Minor_Miseries.Afflictions
{
    internal class WristRecoilTrauma
    {
        public class WristRecoilInjuryAffliction : CustomAffliction, IDuration, IRemedies, IInstance
        {
            public InstanceType Type { get; set; } = InstanceType.Single;
            public void OnFoundExistingInstance(CustomAffliction existingAffliction)
            {
                //MelonLogger.Msg("Wrist Recoil duplication");
                if (existingAffliction is WristRecoilInjuryAffliction WristRecoil)
                {
                    WristRecoil.ResetAffliction(resetRemedies: false);
                    var now = GameManager.GetTimeOfDayComponent().GetHoursPlayedNotPaused();
                    WristRecoil.EndTime = now + WristRecoil.Duration;
                }
            }

            public static bool IsWristRecoilActive { get; private set; } = false;
            public float Duration { get; set; } = Settings.options.WristRecoilDuration;
            public float EndTime { get; set; }

            public Tuple<string, int, int>[] RemedyItems { get; set; } = Array.Empty<Tuple<string, int, int>>();
            public Tuple<string, int, int>[] AltRemedyItems { get; set; } = Array.Empty<Tuple<string, int, int>>();

            public bool InstantHeal { get; set; } = false;

            public WristRecoilInjuryAffliction(AfflictionBodyArea bodyArea) : base("Wrist Trauma", "Weapon recoil", "The recoil hurt your wrist. Aiming and handling the weapon is more difficult.", null, "ico_injury_pain", bodyArea) //customsprite :Minor_Miseries.Resources.Icons.Splinter.png
            {
            }

            public void CureSymptoms()
            {
                //apply this code when symptoms are cured
            }

            public void OnCure()
            {
                IsWristRecoilActive = false;
            }

            public override void OnUpdate()
            {
                IsWristRecoilActive = true;
            }

            [HarmonyPatch(typeof(vp_FPSController), nameof(vp_FPSController.GetSlopeMultiplier))]
            internal static class MovementSpeedPatch
            {
                private static void Postfix(ref float __result)
                {
                    if (!IsWristRecoilActive) return;

                    var pm = GameManager.GetPlayerManagerComponent();
                    if (pm == null) return;
                    if (IsWristRecoilActive && (pm.PlayerIsClimbing()))
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
                    if (!IsWristRecoilActive) return;

                    if (IsWristRecoilActive)
                    {
                        if (OverconfidenceAffliction.IsOvercActive)
                        {
                            __result = (int)(__result * 1.1f);
                        }
                        else
                        {
                            __result = (int)(__result * 1.05f);
                        }
                    }
                }
            }
        }
    }
}
