using static Minor_Miseries.Afflictions.BareSkin;
using AfflictionComponent.Components;
using AfflictionComponent.Interfaces;
using Random = UnityEngine.Random;
using AfflictionComponent.Enums;
using Minor_Miseries.Resources.Localization;

namespace Minor_Miseries.Afflictions
{
    internal class Blister
    {
        public class BlisterAffliction : CustomAffliction, IDuration, IRemedies, IInstance, ILocalizableAffliction
        {
            private const string NAME_KEY = "GAMEPLAY_BlisterName";
            private const string CAUSE_KEY = "GAMEPLAY_BlisterCause";
            private const string DESC_KEY = "GAMEPLAY_BlisterDescription";

            private const string ICON = "Minor_Miseries.Resources.Icons.Afflictions.Classic.Blister.png";
            private const string ALT_ICON = "Minor_Miseries.Resources.Icons.Afflictions.Alt.Blister_ALT.png";

            public InstanceType Type { get; set; } = InstanceType.Single;
            public void OnFoundExistingInstance(CustomAffliction existingAffliction)
            {
                //Core.Log("blister duplication");
                if (existingAffliction is BlisterAffliction blister)
                {
                    blister.ResetAffliction(resetRemedies: false);
                    var now = GameManager.GetTimeOfDayComponent().GetHoursPlayedNotPaused();
                    blister.EndTime = now + blister.Duration;
                }
            }

            private const float BLISTER_EVOLV_CHANCE = 60f;
            internal bool m_SymptomsCured = false;
            internal bool SymptomsCured => m_SymptomsCured;
            public static bool IsBlisterActive { get; private set; } = false;
            public float Duration { get; set; } = Settings.options.BlisterDuration;
            public float EndTime { get; set; }

            public Tuple<string, int, int>[] RemedyItems { get; set; } =
            {
                Tuple.Create("GEAR_HeavyBandage", 1, 1)
            };
            public Tuple<string, int, int>[] AltRemedyItems { get; set; } = Array.Empty<Tuple<string, int, int>>();

            public bool InstantHeal { get; set; } = false;

            public BlisterAffliction(AfflictionBodyArea bodyArea) : base(NAME_KEY, CAUSE_KEY, DESC_KEY, null, Random.Range(0f, 100f) < Settings.options.AltAfflictionIconChance ? ALT_ICON : ICON, bodyArea, true)
            {
            }

            public void CureSymptoms()
            {
                m_SymptomsCured = true;
            }

            public void OnCure()
            {
                IsBlisterActive = false;
                float roll = Random.Range(0f, 100f);
                if (!m_SymptomsCured && Settings.options.IsBareSkin && (roll < BLISTER_EVOLV_CHANCE))
                {
                    new BareSkinAffliction(AfflictionBodyArea.FootRight).Start();
                    GameAudioManager.PlaySound(Il2CppAK.EVENTS.PLAY_VOBREATHELOWINTENSITYNOLOOP, GameManager.GetPlayerObject());
                    AfflictionSaveHelper.QueueSurvivalSave();
                }
                m_SymptomsCured = false;
            }

            public override void OnUpdate()
            {
                IsBlisterActive = true;
            }

            public void RefreshLocalization()
            {
                string oldName = m_Name;

                m_Name = Localization.Get(NAME_KEY);
                m_CauseText = Localization.Get(CAUSE_KEY);
                m_Description = Localization.Get(DESC_KEY);
                m_DescriptionNoHeal = null;

                Core.Log($"Blister refresh -> '{oldName}' => '{m_Name}'");
            }
        }
    }
}