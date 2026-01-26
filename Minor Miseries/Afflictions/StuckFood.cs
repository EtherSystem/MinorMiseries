using AfflictionComponent.Components;
using AfflictionComponent.Enums;
using AfflictionComponent.Interfaces;

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

            private readonly float m_LastUpdateTime;
            public float Duration { get; set; } = Settings.options.StuckFoodDuration;
            public float EndTime { get; set; }

            public Tuple<string, int, int>[] RemedyItems { get; set; }
            //= new Tuple<string, int, int>[] {
            //    Tuple.Create("GEAR_Knife", 1, 1)
            //};
            public Tuple<string, int, int>[] AltRemedyItems { get; set; }
            //= new Tuple<string, int, int>[] {
            //    Tuple.Create("GEAR_Name", 1, 1)
            //};

            public bool InstantHeal { get; set; } = true;

            public StuckFoodAffliction(AfflictionBodyArea bodyArea) : base("Stuck Food", "You ate too fast", "It seems that some food is stuck between your teeth", null, "ico_injury_scurvy", bodyArea) //customsprite :Minor_Miseries.Resources.Icons.StuckFood.png
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
                // affliction effects
            }
        }
    }
}