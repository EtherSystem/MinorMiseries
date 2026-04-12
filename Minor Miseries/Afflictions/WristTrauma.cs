using AfflictionComponent.Interfaces;
using AfflictionComponent.Components;
using AfflictionComponent.Enums;
using Minor_Miseries.Resources.Localization;

namespace Minor_Miseries.Afflictions
{
    internal class WristTrauma
    {
        public class WristTraumaAffliction : CustomAffliction, IDuration, IRemedies, IInstance, ILocalizableAffliction
        {
            private const string NAME_KEY = "GAMEPLAY_WristTraumaName";
            private const string CAUSE_KEY = "GAMEPLAY_WristTraumaCause";
            private const string DESC_KEY = "GAMEPLAY_WristTraumaDescription";

            private const string ICON = "Minor_Miseries.Resources.Icons.Afflictions.Classic.WristTrauma.png";
            private const string ALT_ICON = "Minor_Miseries.Resources.Icons.Afflictions.Alt.WristTrauma_ALT.png";

            public InstanceType Type { get; set; } = InstanceType.Single;
            public void OnFoundExistingInstance(CustomAffliction existingAffliction)
            {
                //Core.Log("Wrist Trauma duplication");
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

            public WristTraumaAffliction(AfflictionBodyArea bodyArea) : base(NAME_KEY, CAUSE_KEY, DESC_KEY, null, UnityEngine.Random.Range(0f, 100f) < Settings.options.AltAfflictionIconChance ? ALT_ICON : ICON, bodyArea, true)
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

            public void RefreshLocalization()
            {
                string oldName = m_Name;

                m_Name = Localization.Get(NAME_KEY);
                m_CauseText = Localization.Get(CAUSE_KEY);
                m_Description = Localization.Get(DESC_KEY);
                m_DescriptionNoHeal = null;

                Core.Log($"WristTrauma refresh -> '{oldName}' => '{m_Name}'");
            }
        }
    }
}
