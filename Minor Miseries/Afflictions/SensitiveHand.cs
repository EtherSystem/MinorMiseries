using AfflictionComponent.Interfaces;
using AfflictionComponent.Components;
using AfflictionComponent.Enums;
using Minor_Miseries.Resources.Localization;

namespace Minor_Miseries.Afflictions
{
    internal class SensitiveHand
    {
        public class SensitiveHandAffliction : CustomAffliction, IDuration, IRemedies, IInstance, ILocalizableAffliction
        {
            private const string NAME_KEY = "GAMEPLAY_SensitiveHandName";
            private const string CAUSE_KEY = "GAMEPLAY_SensitiveHandCause";
            private const string DESC_KEY = "GAMEPLAY_SensitiveHandDescription";

            private const string ICON = "Minor_Miseries.Resources.Icons.Afflictions.Classic.SensitiveHand.png";
            private const string ALT_ICON = "Minor_Miseries.Resources.Icons.Afflictions.Alt.SensitiveHand_ALT.png";

            public InstanceType Type { get; set; } = InstanceType.Single;
            public void OnFoundExistingInstance(CustomAffliction existingAffliction)
            {
                //Core.Log("sensitive hand duplication");
                if (existingAffliction is SensitiveHandAffliction sensitiveHand)
                {
                    sensitiveHand.ResetAffliction(resetRemedies: false);
                    var now = GameManager.GetTimeOfDayComponent().GetHoursPlayedNotPaused();
                    sensitiveHand.EndTime = now + sensitiveHand.Duration;
                }
            }

            public static bool IsSensiActive { get; private set; } = false;
            public float Duration { get; set; } = Settings.options.SensiDuration;
            public float EndTime { get; set; }

            public Tuple<string, int, int>[] RemedyItems { get; set; } = Array.Empty<Tuple<string, int, int>>();
            public Tuple<string, int, int>[] AltRemedyItems { get; set; } = Array.Empty<Tuple<string, int, int>>();

            public bool InstantHeal { get; set; } = true;

            public SensitiveHandAffliction(AfflictionBodyArea bodyArea) : base(NAME_KEY, CAUSE_KEY, DESC_KEY, null, UnityEngine.Random.Range(0f, 100f) < Settings.options.AltAfflictionIconChance ? ALT_ICON : ICON, bodyArea, true)
            {
            }

            public void CureSymptoms()
            {
                //cure symptoms but not the affliction
            }

            public void OnCure()
            {
                IsSensiActive = false;
            }

            public override void OnUpdate()
            {
                IsSensiActive = true;
            }

            public void RefreshLocalization()
            {
                string oldName = m_Name;

                m_Name = Localization.Get(NAME_KEY);
                m_CauseText = Localization.Get(CAUSE_KEY);
                m_Description = Localization.Get(DESC_KEY);
                m_DescriptionNoHeal = null;

                Core.Log($"SensitiveHand refresh -> '{oldName}' => '{m_Name}'");
            }
        }
    }
}