using static Minor_Miseries.Afflictions.SensitiveHand;
using AfflictionComponent.Interfaces;
using AfflictionComponent.Components;
using Random = UnityEngine.Random;
using AfflictionComponent.Enums;

namespace Minor_Miseries.Afflictions
{
    internal class Splinter
    {
        public class SplinterAffliction : CustomAffliction, IDuration, IRemedies, IInstance
        {
            public InstanceType Type { get; set; } = InstanceType.Single;
            public void OnFoundExistingInstance(CustomAffliction existingAffliction)
            {
                //MelonLogger.Msg("splinter duplication");
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

            public SplinterAffliction(AfflictionBodyArea bodyArea): base("GAMEPLAY_SplinterName", "GAMEPLAY_SplinterCause", "GAMEPLAY_SplinterDescription", null, "Minor_Miseries.Resources.Icons.Splinter.png", bodyArea, true)
            {
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
                }
            }

            public override void OnUpdate()
            {
                IsSplinterActive = true;
            }
        }
    }
}