using AfflictionComponent.Interfaces;
using AfflictionComponent.Components;
using AfflictionComponent.Enums;

namespace Minor_Miseries.Afflictions
{
    internal class NightTerror
    {
        public class NightTerrorAffliction : CustomAffliction, IDuration, IRemedies, IInstance
        {
            public InstanceType Type { get; set; } = InstanceType.Single;
            public void OnFoundExistingInstance(CustomAffliction existingAffliction)
            {
                //MelonLogger.Msg("night terror duplication");
                if (existingAffliction is NightTerrorAffliction nightTerror)
                {
                    nightTerror.ResetAffliction(resetRemedies: false);
                    var now = GameManager.GetTimeOfDayComponent().GetHoursPlayedNotPaused();
                    nightTerror.EndTime = now + nightTerror.Duration;
                }
            }

            public static bool IsNightTerrorActive { get; private set; }
            public float Duration { get; set; } = Settings.options.BadDreamDuration * 2;
            public float EndTime { get; set; }

            public Tuple<string, int, int>[] RemedyItems { get; set; } = Array.Empty<Tuple<string, int, int>>();
            public Tuple<string, int, int>[] AltRemedyItems { get; set; } = Array.Empty<Tuple<string, int, int>>();

            public bool InstantHeal { get; set; } = true;

            public NightTerrorAffliction(AfflictionBodyArea bodyArea) : base("GAMEPLAY_NightTerrorName", "GAMEPLAY_NightTerrorCause", "GAMEPLAY_NightTerrorDescription", null, "Minor_Miseries.Resources.Icons.NightTerror.png", bodyArea, true)
            {
            }

            public void CureSymptoms()
            {
                //cure symptoms but not the affliction
            }

            public void OnCure()
            {
                IsNightTerrorActive = false;
            }

            public override void OnUpdate()
            {
                IsNightTerrorActive = true;
            }
        }
    }
}