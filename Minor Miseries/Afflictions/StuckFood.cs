using AfflictionComponent.Components;
using AfflictionComponent.Interfaces;
using AfflictionComponent.Enums;
using Minor_Miseries.Resources.Localization;

namespace Minor_Miseries.Afflictions
{
    internal class StuckFood
    {
        public class StuckFoodAffliction : CustomAffliction, IDuration, IRemedies, IInstance, ILocalizableAffliction
        {
            private const string NAME_KEY = "GAMEPLAY_StuckFoodName";
            private const string CAUSE_KEY = "GAMEPLAY_StuckFoodCause";
            private const string DESC_KEY = "GAMEPLAY_StuckFoodDescription";

            private const string ICON = "Minor_Miseries.Resources.Icons.Afflictions.Classic.StuckFood.png";
            private const string ALT_ICON = "Minor_Miseries.Resources.Icons.Afflictions.Alt.StuckFood_ALT.png";

            public InstanceType Type { get; set; } = InstanceType.Single;
            public void OnFoundExistingInstance(CustomAffliction existingAffliction)
            {
                //Core.Log("stuck food duplication");
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

            public StuckFoodAffliction(AfflictionBodyArea bodyArea) : base(NAME_KEY, CAUSE_KEY, DESC_KEY, null, UnityEngine.Random.Range(0f, 100f) < Settings.options.AltAfflictionIconChance ? ALT_ICON : ICON, bodyArea, true)
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

            public void RefreshLocalization()
            {
                string oldName = m_Name;

                m_Name = Localization.Get(NAME_KEY);
                m_CauseText = Localization.Get(CAUSE_KEY);
                m_Description = Localization.Get(DESC_KEY);
                m_DescriptionNoHeal = null;

                Core.Log($"StuckFood refresh -> '{oldName}' => '{m_Name}'");
            }
        }
    }
}