using AfflictionComponent.Components;
using AfflictionComponent.Interfaces;
using AfflictionComponent.Enums;

namespace Minor_Miseries.Afflictions
{
    internal class SoreNeck
    {
        public class SoreNeckAffliction : CustomAffliction, IDuration, IRemedies, IInstance
        {
            public InstanceType Type { get; set; } = InstanceType.Single;
            public void OnFoundExistingInstance(CustomAffliction existingAffliction)
            {
                //MelonLogger.Msg("sore neck duplication");
                if (existingAffliction is SoreNeckAffliction soreNeck)
                {
                    soreNeck.ResetAffliction(resetRemedies: false);
                    var now = GameManager.GetTimeOfDayComponent().GetHoursPlayedNotPaused();
                    soreNeck.EndTime = now + soreNeck.Duration;
                }
            }

            public float Duration { get; set; } = Settings.options.SoreNeckDuration;
            public float EndTime { get; set; }

            public Tuple<string, int, int>[] RemedyItems { get; set; } =
            {
                Tuple.Create("GEAR_BottlePainKillers", 1, 1)
            };
            public Tuple<string, int, int>[] AltRemedyItems { get; set; } = Array.Empty<Tuple<string, int, int>>();

            public bool InstantHeal { get; set; } = true;

            public SoreNeckAffliction(AfflictionBodyArea bodyArea) : base("GAMEPLAY_SoreNeckName", "GAMEPLAY_SoreNeckCause", "GAMEPLAY_SoreNeckDescription", null, "Minor_Miseries.Resources.Icons.SoreNeck.png", bodyArea, true)
            {
            }

            public void CureSymptoms()
            {
                //cure symptoms but not the affliction
            }

            public void OnCure()
            {
                var cameraStatus = GameManager.GetCameraStatusEffects();
                cameraStatus.m_TriggerHeadachePulse = false;
            }

            public override void OnUpdate()
            {
                var cameraStatus = GameManager.GetCameraStatusEffects();
                cameraStatus.m_TriggerHeadachePulse = true;
            }
        }
    }
}