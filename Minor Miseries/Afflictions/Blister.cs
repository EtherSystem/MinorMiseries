using static Minor_Miseries.Afflictions.Overconfidence;
using static Minor_Miseries.Afflictions.BareSkin;
using AfflictionComponent.Components;
using AfflictionComponent.Interfaces;
using Random = UnityEngine.Random;
using AfflictionComponent.Enums;

namespace Minor_Miseries.Afflictions
{
    internal class Blister
    {
        public class BlisterAffliction : CustomAffliction, IDuration, IRemedies, IInstance
        {
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

            public static float BLISTER_EVOLV_CHANCE = 60f;
            private bool m_SymptomsCured = false;
            public static bool IsBlisterActive { get; private set; } = false;
            public float Duration { get; set; } = Settings.options.BlisterDuration;
            public float EndTime { get; set; }

            public Tuple<string, int, int>[] RemedyItems { get; set; } =
            {
                Tuple.Create("GEAR_HeavyBandage", 1, 1)
            };
            public Tuple<string, int, int>[] AltRemedyItems { get; set; } = Array.Empty<Tuple<string, int, int>>();

            public bool InstantHeal { get; set; } = false;

            public BlisterAffliction(AfflictionBodyArea bodyArea) : base("Blister", "Walked for too long", "You have made a sustained effort for too long", null, "ico_injury_sprainedAnkle", bodyArea) //customsprite :Minor_Miseries.Resources.Icons.Blister.png
            {
            }

            public void CureSymptoms()
            {
                m_SymptomsCured = true;
            }

            public void OnCure()
            {
                IsBlisterActive = false;
                float roll = Random.Range(0f, 100f);
                if (!m_SymptomsCured && Settings.options.IsBareSkin && (roll < BLISTER_EVOLV_CHANCE))
                {
                    new BareSkinAffliction(AfflictionBodyArea.FootRight).Start();
                }
                m_SymptomsCured = false;
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

                    var afflictionManager = AfflictionManager.GetAfflictionManagerInstance();
                    if (afflictionManager == null || afflictionManager.m_Afflictions == null) return;

                    BlisterAffliction activeBlisterAffliction = null;

                    foreach (var affliction in afflictionManager.m_Afflictions)
                    {
                        if (affliction is BlisterAffliction blisterAffliction)
                        {
                            activeBlisterAffliction = blisterAffliction;
                            break;
                        }
                    }

                    if (activeBlisterAffliction == null || activeBlisterAffliction.m_SymptomsCured) return;

                    var pm = GameManager.GetPlayerManagerComponent();
                    if (pm == null) return;

                    if (pm.PlayerIsClimbing() || pm.PlayerIsSprinting() || pm.PlayerIsWalking() || pm.PlayerIsCrouched())
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
        }
    }
}