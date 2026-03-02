using static Minor_Miseries.Afflictions.BareSkin;
using AfflictionComponent.Components;
using AfflictionComponent.Interfaces;
using Random = UnityEngine.Random;
using AfflictionComponent.Enums;

namespace Minor_Miseries.Afflictions
{
    internal class Blister
    {
        public class BlisterAffliction : CustomAffliction, IDuration, IRemedies, IInstance
        {
            public InstanceType Type { get; set; } = InstanceType.Single;
            public void OnFoundExistingInstance(CustomAffliction existingAffliction)
            {
                //MelonLogger.Msg("blister duplication");
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

            public BlisterAffliction(AfflictionBodyArea bodyArea) : base("GAMEPLAY_BlisterName", "GAMEPLAY_BlisterCause", "GAMEPLAY_BlisterDescription", null, "Minor_Miseries.Resources.Icons.Blister.png", bodyArea, true)
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
                }
                m_SymptomsCured = false;
            }

            public override void OnUpdate()
            {
                IsBlisterActive = true;
            }
        }
    }
}