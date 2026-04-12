using AfflictionComponent.Interfaces;
using AfflictionComponent.Components;
using AfflictionComponent.Enums;
using Minor_Miseries.Resources.Localization;

namespace Minor_Miseries.Afflictions
{
    internal class BareSkin
    {
        public class BareSkinAffliction : CustomAffliction, IDuration, IRemedies, IInstance, ILocalizableAffliction
        {
            private const string NAME_KEY = "GAMEPLAY_BareSkinName";
            private const string CAUSE_KEY = "GAMEPLAY_BareSkinCause";
            private const string DESC_KEY = "GAMEPLAY_BareSkinDescription";

            private const string ICON = "Minor_Miseries.Resources.Icons.Afflictions.Classic.BareSkin.png";
            private const string ALT_ICON = "Minor_Miseries.Resources.Icons.Afflictions.Alt.BareSkin_ALT.png";

            public InstanceType Type { get; set; } = InstanceType.Single;
            public void OnFoundExistingInstance(CustomAffliction existingAffliction)
            {
                if (existingAffliction is BareSkinAffliction bareSkin)
                {
                    bareSkin.ResetAffliction(resetRemedies: false);

                    var now = GameManager.GetTimeOfDayComponent().GetHoursPlayedNotPaused();
                    bareSkin.EndTime = now + bareSkin.Duration;

                    bareSkin.m_StartTime = now;
                    bareSkin.m_InfectionRiskTriggered = false;
                }
            }

            private float m_StartTime;
            private static bool m_SymptomsCured = false;
            internal static bool SymptomsCured => m_SymptomsCured;
            private bool m_InfectionRiskTriggered = false;
            public static bool IsBareSkinActive { get; private set; } = false;
            public float Duration { get; set; } = Settings.options.BareSkinDuration;
            public float EndTime { get; set; }

            public Tuple<string, int, int>[] RemedyItems { get; set; } =
            {
                Tuple.Create("GEAR_HeavyBandage", 1, 1),
                Tuple.Create("GEAR_OldMansBeardDressing", 1, 1),
            };
            public Tuple<string, int, int>[] AltRemedyItems { get; set; } = Array.Empty<Tuple<string, int, int>>();

            public bool InstantHeal { get; set; } = false;

            public BareSkinAffliction(AfflictionBodyArea bodyArea) : base(NAME_KEY, CAUSE_KEY, DESC_KEY, null, UnityEngine.Random.Range(0f, 100f) < Settings.options.AltAfflictionIconChance ? ALT_ICON : ICON, bodyArea, true)
            {
                m_StartTime = GameManager.GetTimeOfDayComponent().GetHoursPlayedNotPaused();
            }

            public void CureSymptoms()
            {
                if (!NeedsRemedy()) m_SymptomsCured = true;
            }

            public void OnCure()
            {
                IsBareSkinActive = false;
                m_SymptomsCured = false;
                m_InfectionRiskTriggered = false;
            }

            public override void OnUpdate()
            {
                IsBareSkinActive = true;
                if (m_SymptomsCured) return;
                if (m_InfectionRiskTriggered) return;

                float now = GameManager.GetTimeOfDayComponent().GetHoursPlayedNotPaused();
                float elapsed = now - m_StartTime;

                if (elapsed >= (Settings.options.BareSkinDuration) / 3)
                {
                    GameManager.GetInfectionRiskComponent().InfectionRiskStart(Localization.Get("GAMEPLAY_BareSkinInfection"), AfflictionBodyArea.FootRight, true);
                    m_InfectionRiskTriggered = true;
                }
            }

            public void RefreshLocalization()
            {
                string oldName = m_Name;

                m_Name = Localization.Get(NAME_KEY);
                m_CauseText = Localization.Get(CAUSE_KEY);
                m_Description = Localization.Get(DESC_KEY);
                m_DescriptionNoHeal = null;

                Core.Log($"BareSkin refresh -> '{oldName}' => '{m_Name}'");
            }
        }
    }
}