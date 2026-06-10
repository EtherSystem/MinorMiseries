using static Minor_Miseries.Afflictions.SensitiveHand;
using AfflictionComponent.Interfaces;
using AfflictionComponent.Components;
using Random = UnityEngine.Random;
using AfflictionComponent.Enums;
using Minor_Miseries.Resources.Localization;

namespace Minor_Miseries.Afflictions
{
    internal class Splinter
    {
        public class SplinterAffliction : CustomAffliction, IDuration, IRemedies, IInstance, ISpecialTreatment, ILocalizableAffliction
        {
            private const string NAME_KEY = "GAMEPLAY_SplinterName";
            private const string CAUSE_KEY = "GAMEPLAY_SplinterCause";
            private const string DESC_KEY = "GAMEPLAY_SplinterDescription";
            private const string SPECIAL_TREATMENT_KEY = "GAMEPLAY_SplinterSpecialTreatment";

            private const string ICON = "Minor_Miseries.Resources.Icons.Afflictions.Classic.Splinter.png";
            private const string ALT_ICON = "Minor_Miseries.Resources.Icons.Afflictions.Alt.Splinter_ALT.png";

            public InstanceType Type { get; set; } = InstanceType.Single;
            public void OnFoundExistingInstance(CustomAffliction existingAffliction)
            {
                //Core.Log("splinter duplication");
                if (existingAffliction is SplinterAffliction splinter)
                {
                    splinter.ResetAffliction(resetRemedies: false);
                    var now = GameManager.GetTimeOfDayComponent().GetHoursPlayedNotPaused();
                    splinter.EndTime = now + splinter.Duration;
                }
            }

            public static float SPLINTER_EVOLV_CHANCE = 50f;
            public static bool IsSplinterActive { get; private set; } = false;
            public float Duration { get; set; } = Settings.options.SplinterDuration;
            public float EndTime { get; set; }

            public Tuple<string, int, int>[] RemedyItems { get; set; } = Array.Empty<Tuple<string, int, int>>();
            public Tuple<string, int, int>[] AltRemedyItems { get; set; } = Array.Empty<Tuple<string, int, int>>();

            public bool InstantHeal { get; set; } = true;

            public string SpecialTreatmentText { get; set; } = string.Empty;

            public SplinterAffliction(AfflictionBodyArea bodyArea) : base(NAME_KEY, CAUSE_KEY, DESC_KEY, null, Random.Range(0f, 100f) < Settings.options.AltAfflictionIconChance ? ALT_ICON : ICON, bodyArea, true)
            {
                SpecialTreatmentText = Localization.Get(SPECIAL_TREATMENT_KEY);
            }

            public void CureSymptoms()
            {
                //cure symptoms but not the affliction
            }

            public void OnCure()
            {
                IsSplinterActive = false;
                float roll = Random.Range(0f, 100f);
                if (Settings.options.IsSensi && (roll < SPLINTER_EVOLV_CHANCE))
                {
                    new SensitiveHandAffliction(AfflictionBodyArea.HandLeft).Start();
                    GameAudioManager.PlaySound(Il2CppAK.EVENTS.PLAY_VOBREATHELOWINTENSITYNOLOOP, GameManager.GetPlayerObject());
                    AfflictionSaveHelper.QueueSurvivalSave();
                }
            }

            public override void OnUpdate()
            {
                IsSplinterActive = true;
            }

            public void RefreshLocalization()
            {
                string oldName = m_Name;

                m_Name = Localization.Get(NAME_KEY);
                m_CauseText = Localization.Get(CAUSE_KEY);
                m_Description = Localization.Get(DESC_KEY);
                m_DescriptionNoHeal = null;
                SpecialTreatmentText = Localization.Get(SPECIAL_TREATMENT_KEY);

                Core.Log($"Splinter refresh -> '{oldName}' => '{m_Name}'");
            }
        }
    }
}