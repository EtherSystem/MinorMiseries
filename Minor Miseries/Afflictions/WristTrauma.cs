using AfflictionComponent.Interfaces;
using AfflictionComponent.Components;
using AfflictionComponent.Enums;

namespace Minor_Miseries.Afflictions
{
    internal class WristTrauma
    {
        public class WristTraumaAffliction : CustomAffliction, IDuration, IRemedies, IInstance
        {
            public InstanceType Type { get; set; } = InstanceType.Single;
            public void OnFoundExistingInstance(CustomAffliction existingAffliction)
            {
                //MelonLogger.Msg("Wrist Trauma duplication");
                if (existingAffliction is WristTraumaAffliction wristTrauma)
                {
                    wristTrauma.ResetAffliction(resetRemedies: false);
                    var now = GameManager.GetTimeOfDayComponent().GetHoursPlayedNotPaused();
                    wristTrauma.EndTime = now + wristTrauma.Duration;
                }
            }

            public static bool IsWristTraumaActive { get; private set; } = false;
            public float Duration { get; set; } = Settings.options.WristTraumaDuration;
            public float EndTime { get; set; }

            public Tuple<string, int, int>[] RemedyItems { get; set; } = Array.Empty<Tuple<string, int, int>>();
            public Tuple<string, int, int>[] AltRemedyItems { get; set; } = Array.Empty<Tuple<string, int, int>>();

            public bool InstantHeal { get; set; } = false;

            public WristTraumaAffliction(AfflictionBodyArea bodyArea) : base("GAMEPLAY_WristTraumaName", "GAMEPLAY_WristTraumaCause", "GAMEPLAY_WristTraumaDescription", null, "Minor_Miseries.Resources.Icons.WristTrauma.png", bodyArea, true)
            {
            }

            public void CureSymptoms()
            {
                //apply this code when symptoms are cured
            }

            public void OnCure()
            {
                IsWristTraumaActive = false;
            }

            public override void OnUpdate()
            {
                IsWristTraumaActive = true;
            }
        }
    }
}
