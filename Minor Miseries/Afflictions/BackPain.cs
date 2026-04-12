using AfflictionComponent.Interfaces;
using AfflictionComponent.Components;
using AfflictionComponent.Enums;
using Minor_Miseries.Resources.Localization;

namespace Minor_Miseries.Afflictions
{
    internal class BackPain
    {
        public class BackPainAffliction : CustomAffliction, IDuration, IInstance, IRemedies, ILocalizableAffliction
        {
            private const string NAME_KEY = "GAMEPLAY_BackPainName";
            private const string CAUSE_KEY = "GAMEPLAY_BackPainCause";
            private const string DESC_KEY = "GAMEPLAY_BackPainDescription";

            private const string ICON = "Minor_Miseries.Resources.Icons.Afflictions.Classic.BackPain.png";
            private const string ALT_ICON = "Minor_Miseries.Resources.Icons.Afflictions.Alt.BackPain_ALT.png";

            public InstanceType Type { get; set; } = InstanceType.Single;
            public void OnFoundExistingInstance(CustomAffliction existingAffliction)
            {
                //Core.Log("back pain duplication");
                if (existingAffliction is BackPainAffliction backPain)
                {
                    backPain.ResetAffliction(resetRemedies: false);
                    var now = GameManager.GetTimeOfDayComponent().GetHoursPlayedNotPaused();
                    backPain.EndTime = now + backPain.Duration;
                }
            }

            public static bool IsBackPainActive { get; private set; } = false;
            public float Duration { get; set; } = Settings.options.BackPainDuration;
            public float EndTime { get; set; }

            public Tuple<string, int, int>[] RemedyItems { get; set; } = Array.Empty<Tuple<string, int, int>>();
            public Tuple<string, int, int>[] AltRemedyItems { get; set; } = Array.Empty<Tuple<string, int, int>>();

            public bool InstantHeal { get; set; } = true;

            public BackPainAffliction(AfflictionBodyArea bodyArea) : base(NAME_KEY, CAUSE_KEY, DESC_KEY, null, UnityEngine.Random.Range(0f, 100f) < Settings.options.AltAfflictionIconChance ? ALT_ICON : ICON, bodyArea, true)
            {
            }

            public void CureSymptoms()
            {
                //cure symptoms but not the affliction
            }

            public void OnCure()
            {
                IsBackPainActive = false;
            }

            public override void OnUpdate()
            {
                IsBackPainActive = true;
            }

            public void RefreshLocalization()
            {
                string oldName = m_Name;

                m_Name = Localization.Get(NAME_KEY);
                m_CauseText = Localization.Get(CAUSE_KEY);
                m_Description = Localization.Get(DESC_KEY);
                m_DescriptionNoHeal = null;

                Core.Log($"BackPain refresh -> '{oldName}' => '{m_Name}'");
            }
        }
    }
}