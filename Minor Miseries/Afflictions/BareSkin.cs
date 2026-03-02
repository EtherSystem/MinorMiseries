using AfflictionComponent.Interfaces;
using AfflictionComponent.Components;
using AfflictionComponent.Enums;

namespace Minor_Miseries.Afflictions
{
    internal class BareSkin
    {
        public class BareSkinAffliction : CustomAffliction, IDuration, IRemedies, IInstance
        {
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

            public BareSkinAffliction(AfflictionBodyArea bodyArea) : base("GAMEPLAY_BareSkinName", "GAMEPLAY_BareSkinCause", "GAMEPLAY_BareSkinDescription", null, "Minor_Miseries.Resources.Icons.BareSkin.png", bodyArea, true)
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
        }
    }
}