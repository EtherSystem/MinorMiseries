using AfflictionComponent.Interfaces;
using AfflictionComponent.Components;
using AfflictionComponent.Enums;
using Minor_Miseries.Resources.Localization;

namespace Minor_Miseries.Afflictions
{
    internal class NightTerror
    {
        public class NightTerrorAffliction : CustomAffliction, IDuration, IRemedies, IInstance, IAfflictionProgressBar, ILocalizableAffliction
        {
            private const string NAME_KEY = "GAMEPLAY_NightTerrorName";
            private const string CAUSE_KEY = "GAMEPLAY_NightTerrorCause";
            private const string DESC_KEY = "GAMEPLAY_NightTerrorDescription";

            private const string ICON = "Minor_Miseries.Resources.Icons.Afflictions.Classic.NightTerror.png";
            private const string ALT_ICON = "Minor_Miseries.Resources.Icons.Afflictions.Alt.NightTerror_ALT.png";

            public InstanceType Type { get; set; } = InstanceType.Single;
            public void OnFoundExistingInstance(CustomAffliction existingAffliction)
            {
                //Core.Log("night terror duplication");
                if (existingAffliction is NightTerrorAffliction nightTerror)
                {
                    nightTerror.ResetAffliction(resetRemedies: false);
                    var now = GameManager.GetTimeOfDayComponent().GetHoursPlayedNotPaused();
                    nightTerror.EndTime = now + nightTerror.Duration;
                    nightTerror.RefreshProgressBar();
                }
            }

            public static bool IsNightTerrorActive { get; private set; }
            public float Duration { get; set; } = Settings.options.BadDreamDuration * 2;
            public float EndTime { get; set; }

            public Tuple<string, int, int>[] RemedyItems { get; set; } = Array.Empty<Tuple<string, int, int>>();
            public Tuple<string, int, int>[] AltRemedyItems { get; set; } = Array.Empty<Tuple<string, int, int>>();

            public bool InstantHeal { get; set; } = true;

            public float ProgressBar { get; set; } = 0f;
            public bool InvertProgressBar { get; set; } = true;

            public NightTerrorAffliction(AfflictionBodyArea bodyArea) : base(NAME_KEY, CAUSE_KEY, DESC_KEY, null, UnityEngine.Random.Range(0f, 100f) < Settings.options.AltAfflictionIconChance ? ALT_ICON : ICON, bodyArea, true)
            {
                TimeOfDay? tod = GameManager.GetTimeOfDayComponent();
                if (tod != null) EndTime = tod.GetHoursPlayedNotPaused() + Duration;
                RefreshProgressBar();
            }

            public void CureSymptoms()
            {
                //cure symptoms but not the affliction
            }

            public void OnCure()
            {
                IsNightTerrorActive = false;
            }

            public override void OnUpdate()
            {
                IsNightTerrorActive = true;
                RefreshProgressBar();
            }

            private void RefreshProgressBar()
            {
                TimeOfDay? tod = GameManager.GetTimeOfDayComponent();
                if (tod == null || Duration <= 0f)
                {
                    ProgressBar = 0f;
                    return;
                }

                if (EndTime <= 0f) EndTime = tod.GetHoursPlayedNotPaused() + Duration;

                float elapsedHours = Mathf.Clamp(Duration - (EndTime - tod.GetHoursPlayedNotPaused()), 0f, Duration);
                ProgressBar = Mathf.Clamp01(elapsedHours / Duration);
            }

            public void RefreshLocalization()
            {
                string oldName = m_Name;

                m_Name = Localization.Get(NAME_KEY);
                m_CauseText = Localization.Get(CAUSE_KEY);
                m_Description = Localization.Get(DESC_KEY);
                m_DescriptionNoHeal = null;

                Core.Log($"NightTerror refresh -> '{oldName}' => '{m_Name}'");
            }
        }
    }
}