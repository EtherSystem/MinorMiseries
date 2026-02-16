using AfflictionComponent.Components;
using AfflictionComponent.Interfaces;
using AfflictionComponent.Enums;

namespace Minor_Miseries.Afflictions
{
    internal class StuckFood
    {
        public class StuckFoodAffliction : CustomAffliction, IDuration, IRemedies, IInstance
        {
            public InstanceType Type { get; set; } = InstanceType.Single;
            public void OnFoundExistingInstance(CustomAffliction existingAffliction)
            {
                //MelonLogger.Msg("stuck food duplication");
                if (existingAffliction is StuckFoodAffliction stuckFood)
                {
                    stuckFood.ResetAffliction(resetRemedies: false);
                    var now = GameManager.GetTimeOfDayComponent().GetHoursPlayedNotPaused();
                    stuckFood.EndTime = now + stuckFood.Duration;
                }
            }

            public float Duration { get; set; } = Settings.options.StuckFoodDuration;
            public float EndTime { get; set; }

            public Tuple<string, int, int>[] RemedyItems { get; set; } = Array.Empty<Tuple<string, int, int>>();
            public Tuple<string, int, int>[] AltRemedyItems { get; set; } = Array.Empty<Tuple<string, int, int>>();

            public bool InstantHeal { get; set; } = true;

            public StuckFoodAffliction(AfflictionBodyArea bodyArea) : base("Stuck Food", "You ate too fast", "It seems that some food is stuck between your teeth", null, "ico_injury_scurvy", bodyArea) //customsprite :Minor_Miseries.Resources.Icons.StuckFood.png
            {
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