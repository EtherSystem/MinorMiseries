using AfflictionComponent.Interfaces;
using AfflictionComponent.Components;
using AfflictionComponent.Enums;

namespace Minor_Miseries.Afflictions
{
    internal class BackPain
    {
        public class BackPainAffliction : CustomAffliction, IDuration, IInstance, IRemedies
        {
            public InstanceType Type { get; set; } = InstanceType.Single;
            public void OnFoundExistingInstance(CustomAffliction existingAffliction)
            {
                //MelonLogger.Msg("back pain duplication");
                if (existingAffliction is BackPainAffliction backPain)
                {
                    backPain.ResetAffliction(resetRemedies: false);
                    var now = GameManager.GetTimeOfDayComponent().GetHoursPlayedNotPaused();
                    backPain.EndTime = now + backPain.Duration;
                }
            }

            public static bool IsBackPainActive { get; private set; } = false;
            public float Duration { get; set; } = Settings.options.BackPainDuration;
            public float EndTime { get; set; }

            public Tuple<string, int, int>[] RemedyItems { get; set; } = Array.Empty<Tuple<string, int, int>>();
            public Tuple<string, int, int>[] AltRemedyItems { get; set; } = Array.Empty<Tuple<string, int, int>>();

            public bool InstantHeal { get; set; } = true;

            public BackPainAffliction(AfflictionBodyArea bodyArea) : base("GAMEPLAY_BackPainName", "GAMEPLAY_BackPainCause", "GAMEPLAY_BackPainDescription", null, "Minor_Miseries.Resources.Icons.BackPain.png", bodyArea, true)
            {
            }

            public void CureSymptoms()
            {
                //cure symptoms but not the affliction
            }

            public void OnCure()
            {
                IsBackPainActive = false;
            }

            public override void OnUpdate()
            {
                IsBackPainActive = true;
            }
        }
    }
}