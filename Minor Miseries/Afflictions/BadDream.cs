using AfflictionComponent.Interfaces;
using AfflictionComponent.Components;
using AfflictionComponent.Enums;

namespace Minor_Miseries.Afflictions
{
    internal class BadDream
    {
        public class BadDreamAffliction : CustomAffliction, IDuration, IRemedies, IInstance
        {
            public InstanceType Type { get; set; } = InstanceType.Single;
            public void OnFoundExistingInstance(CustomAffliction existingAffliction)
            {
                //MelonLogger.Msg("bad dream duplication");
                if (existingAffliction is BadDreamAffliction badDream)
                {
                    badDream.ResetAffliction(resetRemedies: false);
                    var now = GameManager.GetTimeOfDayComponent().GetHoursPlayedNotPaused();
                    badDream.EndTime = now + badDream.Duration;
                }
            }

            private readonly float m_LastUpdateTime;
            public float Duration { get; set; } = Settings.options.BadDreamDuration;
            public float EndTime { get; set; }

            public Tuple<string, int, int>[] RemedyItems { get; set; } = Array.Empty<Tuple<string, int, int>>();
            public Tuple<string, int, int>[] AltRemedyItems { get; set; } = Array.Empty<Tuple<string, int, int>>();

            public bool InstantHeal { get; set; } = true;

            public BadDreamAffliction(AfflictionBodyArea bodyArea) : base("Bad Dream", "Troubled sleep", "A disturbing dream snaps you awake, your thoughts racing and sleep refusing to return", null, "ico_injury_headache", bodyArea) //customsprite :Minor_Miseries.Resources.Icons.BadDream.png
            {
                m_LastUpdateTime = GameManager.GetTimeOfDayComponent().GetHoursPlayedNotPaused();
            }

            public void CureSymptoms()
            {
                //cure symptoms but not the affliction
            }

            public void OnCure()
            {
                //when the affliction is cured, apply this code
            }

            public override void OnUpdate()
            {
                // yes theres no effects, its intended
            }
        }
    }
}
