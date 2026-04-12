using AfflictionComponent.Interfaces;
using AfflictionComponent.Components;
using AfflictionComponent.Enums;
using Minor_Miseries.Resources.Localization;

namespace Minor_Miseries.Afflictions
{
    internal class ShoulderTrauma
    {
        public class ShoulderTraumaAffliction : CustomAffliction, IDuration, IRemedies, IInstance, ILocalizableAffliction
        {
            private const string NAME_KEY = "GAMEPLAY_ShoulderTraumaName";
            private const string CAUSE_KEY = "GAMEPLAY_ShoulderTraumaCause";
            private const string DESC_KEY = "GAMEPLAY_ShoulderTraumaDescription";

            private const string ICON = "Minor_Miseries.Resources.Icons.Afflictions.Classic.ShoulderTrauma.png";
            private const string ALT_ICON = "Minor_Miseries.Resources.Icons.Afflictions.Alt.ShoulderTrauma_ALT.png";

            public InstanceType Type { get; set; } = InstanceType.Single;
            public void OnFoundExistingInstance(CustomAffliction existingAffliction)
            {
                //Core.Log("Shoulder Trauma duplication");
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

            public ShoulderTraumaAffliction(AfflictionBodyArea bodyArea) : base(NAME_KEY, CAUSE_KEY, DESC_KEY, null, UnityEngine.Random.Range(0f, 100f) < Settings.options.AltAfflictionIconChance ? ALT_ICON : ICON, bodyArea, true)
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

            public void RefreshLocalization()
            {
                string oldName = m_Name;

                m_Name = Localization.Get(NAME_KEY);
                m_CauseText = Localization.Get(CAUSE_KEY);
                m_Description = Localization.Get(DESC_KEY);
                m_DescriptionNoHeal = null;

                Core.Log($"ShoulderTrauma refresh -> '{oldName}' => '{m_Name}'");
            }
        }
    }
}