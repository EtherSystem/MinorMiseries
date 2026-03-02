using AfflictionComponent.Interfaces;
using AfflictionComponent.Components;
using AfflictionComponent.Enums;

namespace Minor_Miseries.Afflictions
{
    internal class ShoulderTrauma
    {
        public class ShoulderTraumaAffliction : CustomAffliction, IDuration, IRemedies, IInstance
        {
            public InstanceType Type { get; set; } = InstanceType.Single;
            public void OnFoundExistingInstance(CustomAffliction existingAffliction)
            {
                //MelonLogger.Msg("Shoulder Trauma duplication");
                if (existingAffliction is ShoulderTraumaAffliction shoulderTrauma)
                {
                    shoulderTrauma.ResetAffliction(resetRemedies: false);
                    var now = GameManager.GetTimeOfDayComponent().GetHoursPlayedNotPaused();
                    shoulderTrauma.EndTime = now + shoulderTrauma.Duration;
                }
            }

            public static bool IsShoulderTraumaActive { get; private set; } = false;
            public float Duration { get; set; } = Settings.options.ShoulderTraumaDuration;
            public float EndTime { get; set; }

            public Tuple<string, int, int>[] RemedyItems { get; set; } = Array.Empty<Tuple<string, int, int>>();
            public Tuple<string, int, int>[] AltRemedyItems { get; set; } = Array.Empty<Tuple<string, int, int>>();

            public bool InstantHeal { get; set; } = true;

            public ShoulderTraumaAffliction(AfflictionBodyArea bodyArea) : base("GAMEPLAY_ShoulderTraumaName", "GAMEPLAY_ShoulderTraumaCause", "GAMEPLAY_ShoulderTraumaDescription", null, "Minor_Miseries.Resources.Icons.ShoulderTrauma.png", bodyArea, true)
            {
            }

            public void CureSymptoms()
            {
                //apply this code when symptoms are cured
            }

            public void OnCure()
            {
                IsShoulderTraumaActive = false;
            }
            public override void OnUpdate()
            {
                IsShoulderTraumaActive = true;
            }
        }
    }
}