using AfflictionComponent.Interfaces;
using AfflictionComponent.Components;
using AfflictionComponent.Enums;
using Minor_Miseries.Resources.Localization;

namespace Minor_Miseries.Afflictions
{
    internal class BadDream
    {
        public class BadDreamAffliction : CustomAffliction, IDuration, IRemedies, IInstance, ILocalizableAffliction
        {
            private const string NAME_KEY = "GAMEPLAY_BadDreamName";
            private const string CAUSE_KEY = "GAMEPLAY_BadDreamCause";
            private const string DESC_KEY = "GAMEPLAY_BadDreamDescription";

            private const string ICON = "Minor_Miseries.Resources.Icons.Afflictions.Classic.BadDream.png";
            private const string ALT_ICON = "Minor_Miseries.Resources.Icons.Afflictions.Alt.BadDream_ALT.png";

            public InstanceType Type { get; set; } = InstanceType.Single;
            public void OnFoundExistingInstance(CustomAffliction existingAffliction)
            {
                //Core.Log("bad dream duplication");
                if (existingAffliction is BadDreamAffliction badDream)
                {
                    badDream.ResetAffliction(resetRemedies: false);
                    var now = GameManager.GetTimeOfDayComponent().GetHoursPlayedNotPaused();
                    badDream.EndTime = now + badDream.Duration;
                }
            }

            public static bool IsBadDreamActive { get; private set; }
            public float Duration { get; set; } = Settings.options.BadDreamDuration;
            public float EndTime { get; set; }

            public Tuple<string, int, int>[] RemedyItems { get; set; } = Array.Empty<Tuple<string, int, int>>();
            public Tuple<string, int, int>[] AltRemedyItems { get; set; } = Array.Empty<Tuple<string, int, int>>();

            public bool InstantHeal { get; set; } = true;

            public BadDreamAffliction(AfflictionBodyArea bodyArea) : base(NAME_KEY, CAUSE_KEY, DESC_KEY, null, UnityEngine.Random.Range(0f, 100f) < Settings.options.AltAfflictionIconChance ? ALT_ICON : ICON, bodyArea, true)
            {
            }

            public void CureSymptoms()
            {
                //cure symptoms but not the affliction
            }

            public void OnCure()
            {
                IsBadDreamActive = false;
            }

            public override void OnUpdate()
            {
                IsBadDreamActive = true;
            }

            public void RefreshLocalization()
            {
                string oldName = m_Name;

                m_Name = Localization.Get(NAME_KEY);
                m_CauseText = Localization.Get(CAUSE_KEY);
                m_Description = Localization.Get(DESC_KEY);
                m_DescriptionNoHeal = null;

                Core.Log($"BadDream refresh -> '{oldName}' => '{m_Name}'");
            }
        }
    }
}
