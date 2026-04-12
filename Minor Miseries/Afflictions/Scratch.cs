using static Minor_Miseries.Afflictions.SmallCut;
using AfflictionComponent.Interfaces;
using AfflictionComponent.Components;
using Random = UnityEngine.Random;
using AfflictionComponent.Enums;
using Minor_Miseries.Resources.Localization;

namespace Minor_Miseries.Afflictions
{
    internal class Scratch
    {
        public class ScratchAffliction : CustomAffliction, IDuration, IRemedies, IInstance, ILocalizableAffliction
        {
            private const string NAME_KEY = "GAMEPLAY_ScratchName";
            private const string CAUSE_KEY = "GAMEPLAY_ScratchCause";
            private const string DESC_KEY = "GAMEPLAY_ScratchDescription";

            private const string ICON = "Minor_Miseries.Resources.Icons.Afflictions.Classic.Scratch.png";
            private const string ALT_ICON = "Minor_Miseries.Resources.Icons.Afflictions.Alt.Scratch_ALT.png";

            public InstanceType Type { get; set; } = InstanceType.Single;
            public void OnFoundExistingInstance(CustomAffliction existingAffliction)
            {
                //Core.Log("scratch duplication");
                if (existingAffliction is ScratchAffliction scratch)
                {
                    scratch.ResetAffliction(resetRemedies: false);
                    var now = GameManager.GetTimeOfDayComponent().GetHoursPlayedNotPaused();
                    scratch.EndTime = now + scratch.Duration;
                }
            }

            public static float SCRATCH_EVOLV_CHANCE = 40f;
            private bool m_SymptomsCured = false;
            internal bool SymptomsCured => m_SymptomsCured;
            public static bool IsScratchActive { get; private set; } = false;
            public float Duration { get; set; } = Settings.options.ScratchDuration;
            public float EndTime { get; set; }

            public Tuple<string, int, int>[] RemedyItems { get; set; } =
            {
                Tuple.Create("GEAR_HeavyBandage", 1, 1)
            };
            public Tuple<string, int, int>[] AltRemedyItems { get; set; } = Array.Empty<Tuple<string, int, int>>();

            public bool InstantHeal { get; set; } = false;

            public ScratchAffliction(AfflictionBodyArea bodyArea) : base(NAME_KEY, CAUSE_KEY, DESC_KEY, null, Random.Range(0f, 100f) < Settings.options.AltAfflictionIconChance ? ALT_ICON : ICON, bodyArea, true)
            {
            }

            public void CureSymptoms()
            {
                m_SymptomsCured = true;
            }

            public void OnCure()
            {
                IsScratchActive = false;
                float roll = Random.Range(0f, 100f);
                if (!m_SymptomsCured && Settings.options.IsSmallCut && (roll < SCRATCH_EVOLV_CHANCE))
                {
                    new SmallCutAffliction(AfflictionBodyArea.ArmLeft).Start();
                    GameAudioManager.PlaySound(Il2CppAK.EVENTS.PLAY_VOBREATHELOWINTENSITYNOLOOP, GameManager.GetPlayerObject());
                    AfflictionSaveHelper.QueueSurvivalSave();
                }
                m_SymptomsCured = false;
            }

            public override void OnUpdate()
            {
                IsScratchActive = true;
            }

            public void RefreshLocalization()
            {
                string oldName = m_Name;

                m_Name = Localization.Get(NAME_KEY);
                m_CauseText = Localization.Get(CAUSE_KEY);
                m_Description = Localization.Get(DESC_KEY);
                m_DescriptionNoHeal = null;

                Core.Log($"Scratch refresh -> '{oldName}' => '{m_Name}'");
            }
        }
    }
}