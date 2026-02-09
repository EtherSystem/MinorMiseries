using static Minor_Miseries.Afflictions.Overconfidence;
using AfflictionComponent.Interfaces;
using AfflictionComponent.Components;
using AfflictionComponent.Enums;
using Il2CppTLD.IntBackedUnit;

namespace Minor_Miseries.Afflictions
{
    internal class BackPain
    {
        public class BackPainAffliction : CustomAffliction, IDuration, IInstance, IRemedies
        {
            public InstanceType Type { get; set; } = InstanceType.Single;
            public void OnFoundExistingInstance(CustomAffliction existingAffliction)
            {
                //MelonLogger.Msg("back pain duplication");
                if (existingAffliction is BackPainAffliction backPain)
                {
                    backPain.ResetAffliction(resetRemedies: false);
                    var now = GameManager.GetTimeOfDayComponent().GetHoursPlayedNotPaused();
                    backPain.EndTime = now + backPain.Duration;
                }
            }

            private readonly float m_LastUpdateTime;
            public static bool IsBackPainActive { get; private set; } = false;
            public float Duration { get; set; } = Settings.options.BackPainDuration;
            public float EndTime { get; set; }

            public Tuple<string, int, int>[] RemedyItems { get; set; } = Array.Empty<Tuple<string, int, int>>();
            public Tuple<string, int, int>[] AltRemedyItems { get; set; } = Array.Empty<Tuple<string, int, int>>();

            public bool InstantHeal { get; set; } = true;

            public BackPainAffliction(AfflictionBodyArea bodyArea) : base("Back Pain", "A too heavy backpack", "Being encumbered for too long has consequences...", null, "ico_injury_burdened", bodyArea) //customsprite :Minor_Miseries.Resources.Icons.BackPain.png
            {
                m_LastUpdateTime = GameManager.GetTimeOfDayComponent().GetHoursPlayedNotPaused();
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

            [HarmonyPatch(typeof(Encumber), nameof(Encumber.Update))]
            internal static class CarryCapacityPatch
            {
                private static void Postfix(Encumber __instance)
                {
                    if (!BackPainAffliction.IsBackPainActive) return;

                    EncumberUpdate(__instance);
                }
            }

            internal static void EncumberUpdate(Encumber encumber)
            {
                if (OverconfidenceAffliction.IsOvercActive)
                {
                    encumber.m_MaxCarryCapacity = ItemWeight.FromKilograms(30f - 7.5f);
                    encumber.m_MaxCarryCapacityWhenExhausted = ItemWeight.FromKilograms(15f - 7.5f);
                    encumber.m_NoSprintCarryCapacity = ItemWeight.FromKilograms(40f - 7.5f);
                    encumber.m_NoWalkCarryCapacity = ItemWeight.FromKilograms(60f - 7.5f);
                    encumber.m_EncumberLowThreshold = ItemWeight.FromKilograms(31f - 7.5f);
                    encumber.m_EncumberMedThreshold = ItemWeight.FromKilograms(40f - 7.5f);
                    encumber.m_EncumberHighThreshold = ItemWeight.FromKilograms(60f - 7.5f);
                }
                else
                {
                    encumber.m_MaxCarryCapacity = ItemWeight.FromKilograms(30f - 5f);
                    encumber.m_MaxCarryCapacityWhenExhausted = ItemWeight.FromKilograms(15f - 5f);
                    encumber.m_NoSprintCarryCapacity = ItemWeight.FromKilograms(40f - 5f);
                    encumber.m_NoWalkCarryCapacity = ItemWeight.FromKilograms(60f - 5f);
                    encumber.m_EncumberLowThreshold = ItemWeight.FromKilograms(31f - 5f);
                    encumber.m_EncumberMedThreshold = ItemWeight.FromKilograms(40f - 5f);
                    encumber.m_EncumberHighThreshold = ItemWeight.FromKilograms(60f - 5f);
                }
            }
        }
    }
}