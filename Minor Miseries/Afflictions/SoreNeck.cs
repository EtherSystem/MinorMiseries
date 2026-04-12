using AfflictionComponent.Components;
using AfflictionComponent.Interfaces;
using AfflictionComponent.Enums;
using Minor_Miseries.Resources.Localization;

namespace Minor_Miseries.Afflictions
{
    internal class SoreNeck
    {
        public class SoreNeckAffliction : CustomAffliction, IDuration, IRemedies, IInstance, ILocalizableAffliction
        {
            private const string NAME_KEY = "GAMEPLAY_SoreNeckName";
            private const string CAUSE_KEY = "GAMEPLAY_SoreNeckCause";
            private const string DESC_KEY = "GAMEPLAY_SoreNeckDescription";

            private const string ICON = "Minor_Miseries.Resources.Icons.Afflictions.Classic.SoreNeck.png";
            private const string ALT_ICON = "Minor_Miseries.Resources.Icons.Afflictions.Alt.SoreNeck_ALT.png";

            public InstanceType Type { get; set; } = InstanceType.Single;
            public void OnFoundExistingInstance(CustomAffliction existingAffliction)
            {
                //Core.Log("sore neck duplication");
                if (existingAffliction is SoreNeckAffliction soreNeck)
                {
                    soreNeck.ResetAffliction(resetRemedies: false);
                    var now = GameManager.GetTimeOfDayComponent().GetHoursPlayedNotPaused();
                    soreNeck.EndTime = now + soreNeck.Duration;
                }
            }

            public float Duration { get; set; } = Settings.options.SoreNeckDuration;
            public float EndTime { get; set; }

            public Tuple<string, int, int>[] RemedyItems { get; set; } =
            {
                Tuple.Create("GEAR_BottlePainKillers", 1, 1)
            };
            public Tuple<string, int, int>[] AltRemedyItems { get; set; } = Array.Empty<Tuple<string, int, int>>();

            public bool InstantHeal { get; set; } = true;

            public SoreNeckAffliction(AfflictionBodyArea bodyArea) : base(NAME_KEY, CAUSE_KEY, DESC_KEY, null, UnityEngine.Random.Range(0f, 100f) < Settings.options.AltAfflictionIconChance ? ALT_ICON : ICON, bodyArea, true)
            {
            }

            public void CureSymptoms()
            {
                //cure symptoms but not the affliction
            }

            public void OnCure()
            {
                var cameraStatus = GameManager.GetCameraStatusEffects();
                cameraStatus.m_TriggerHeadachePulse = false;
            }

            public override void OnUpdate()
            {
                var cameraStatus = GameManager.GetCameraStatusEffects();
                cameraStatus.m_TriggerHeadachePulse = true;
            }

            public void RefreshLocalization()
            {
                string oldName = m_Name;

                m_Name = Localization.Get(NAME_KEY);
                m_CauseText = Localization.Get(CAUSE_KEY);
                m_Description = Localization.Get(DESC_KEY);
                m_DescriptionNoHeal = null;

                Core.Log($"SoreNeck refresh -> '{oldName}' => '{m_Name}'");
            }
        }
    }
}