using AfflictionComponent.Interfaces;
using AfflictionComponent.Components;
using AfflictionComponent.Enums;
using Minor_Miseries.Resources.Localization;

namespace Minor_Miseries.Afflictions
{
    internal class SmallCut
    {
        public class SmallCutAffliction : CustomAffliction, IDuration, IRemedies, IInstance, ILocalizableAffliction
        {
            private const string NAME_KEY = "GAMEPLAY_SmallCutName";
            private const string CAUSE_KEY = "GAMEPLAY_SmallCutCause";
            private const string DESC_KEY = "GAMEPLAY_SmallCutDescription";

            private const string ICON = "Minor_Miseries.Resources.Icons.Afflictions.Classic.SmallCut.png";
            private const string ALT_ICON = "Minor_Miseries.Resources.Icons.Afflictions.Alt.SmallCut_ALT.png";

            public InstanceType Type { get; set; } = InstanceType.Single;
            public void OnFoundExistingInstance(CustomAffliction existingAffliction)
            {
                //Core.Log("bare skin duplication");
                if (existingAffliction is SmallCutAffliction smallCut)
                {
                    smallCut.ResetAffliction(resetRemedies: false);
                    var now = GameManager.GetTimeOfDayComponent().GetHoursPlayedNotPaused();
                    smallCut.EndTime = now + smallCut.Duration;
                }
            }

            private float m_StartTime;
            private bool m_InfectionRiskTriggered = false;
            private bool m_SymptomsCured = false;
            public static bool IsSmallCutActive { get; private set; } = false;
            public float Duration { get; set; } = Settings.options.SmallCutDuration;
            public float EndTime { get; set; }

            public Tuple<string, int, int>[] RemedyItems { get; set; } =
            {
                Tuple.Create("GEAR_HeavyBandage", 1, 1),
                Tuple.Create("GEAR_OldMansBeardDressing", 1, 1),
            };
            public Tuple<string, int, int>[] AltRemedyItems { get; set; } = Array.Empty<Tuple<string, int, int>>();

            public bool InstantHeal { get; set; } = false;

            public SmallCutAffliction(AfflictionBodyArea bodyArea) : base(NAME_KEY, CAUSE_KEY, DESC_KEY, null, UnityEngine.Random.Range(0f, 100f) < Settings.options.AltAfflictionIconChance ? ALT_ICON : ICON, bodyArea, true)
            {
                m_StartTime = GameManager.GetTimeOfDayComponent().GetHoursPlayedNotPaused();
            }

            public void CureSymptoms()
            {
                if (!NeedsRemedy()) m_SymptomsCured = true;
            }

            public void OnCure()
            {
                IsSmallCutActive = false;
                m_InfectionRiskTriggered = false;
                m_SymptomsCured = false;
            }

            public override void OnUpdate()
            {
                IsSmallCutActive = true;
                if (m_SymptomsCured) return;
                if (m_InfectionRiskTriggered) return;

                float now = GameManager.GetTimeOfDayComponent().GetHoursPlayedNotPaused();
                float elapsed = now - m_StartTime;

                if (elapsed >= (Settings.options.SmallCutDuration) / 2)
                {
                    GameManager.GetInfectionRiskComponent().InfectionRiskStart(Localization.Get("GAMEPLAY_SmallCutInfection"), AfflictionBodyArea.Chest, true);

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

                Core.Log($"SmallCut refresh -> '{oldName}' => '{m_Name}'");
            }
        }
    }
}