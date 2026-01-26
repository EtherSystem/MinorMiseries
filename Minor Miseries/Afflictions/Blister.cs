using AfflictionComponent.Components;
using AfflictionComponent.Enums;
using AfflictionComponent.Interfaces;

namespace Minor_Miseries.Afflictions
{
    internal class Blister
    {
        public class BlisterAffliction : CustomAffliction, IDuration, IRemedies, IInstance
        {
            private static readonly bool IsOvercActive = Overconfidence.OverconfidenceAffliction.IsOvercActive;
            public InstanceType Type { get; set; } = InstanceType.Single;
            public void OnFoundExistingInstance(CustomAffliction existingAffliction)
            {
                //MelonLogger.Msg("blister duplication");
                if (existingAffliction is BlisterAffliction blister)
                {
                    blister.ResetAffliction(resetRemedies: false);
                    var now = GameManager.GetTimeOfDayComponent().GetHoursPlayedNotPaused();
                    blister.EndTime = now + blister.Duration;
                }
            }

            private readonly float m_LastUpdateTime;
            public static bool IsBlisterActive { get; private set; } = false;
            public float Duration { get; set; } = Settings.options.BlisterDuration;
            public float EndTime { get; set; }

            public Tuple<string, int, int>[] RemedyItems { get; set; }
            //= new Tuple<string, int, int>[] {
            //    Tuple.Create("GEAR_HeavyBandage", 1, 1)
            //};
            public Tuple<string, int, int>[] AltRemedyItems { get; set; }
            //= new Tuple<string, int, int>[] {
            //    Tuple.Create("GEAR_Name", 1, 1)
            //};

            public bool InstantHeal { get; set; } = false;

            public BlisterAffliction(AfflictionBodyArea bodyArea) : base("Blister", "Walked for too long", "You have made a sustained effort for too long", null, "ico_injury_sprainedAnkle", bodyArea) //customsprite :Minor_Miseries.Resources.Icons.Blister.png
            {
                m_LastUpdateTime = GameManager.GetTimeOfDayComponent().GetHoursPlayedNotPaused();
            }

            public void CureSymptoms()
            {
                //cure symptoms but not the affliction
            }

            public void OnCure()
            {
                IsBlisterActive = false;
            }

            public override void OnUpdate()
            {
                IsBlisterActive = true;
            }

            [HarmonyPatch(typeof(vp_FPSController), nameof(vp_FPSController.GetSlopeMultiplier))]
            internal static class MovementSpeedPatch
            {
                private static void Postfix(ref float __result)
                {
                    if (!IsBlisterActive) return;

                    var pm = GameManager.GetPlayerManagerComponent();
                    if (IsBlisterActive && (pm.PlayerIsClimbing() || pm.PlayerIsSprinting() || pm.PlayerIsWalking() || pm.PlayerIsCrouched()))
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