using AfflictionComponent.Components;
using AfflictionComponent.Enums;
using AfflictionComponent.Interfaces;
using static Minor_Miseries.Afflictions.OverconfidenceRisk;

namespace Minor_Miseries.Afflictions
{
    internal class Overconfidence
    {
        public class OverconfidenceAffliction : CustomAffliction, IRemedies, IInstance
        {
            public InstanceType Type { get; set; } = InstanceType.SingleLocation;
            public void OnFoundExistingInstance(CustomAffliction existingAffliction)
            {
                return;//MelonLogger.Msg("splinter duplication");
            }

            private readonly float m_LastUpdateTime;
            public static bool IsOvercActive { get; private set; } = false;

            public Tuple<string, int, int>[] RemedyItems { get; set; }
            //= new Tuple<string, int, int>[] {
            //    Tuple.Create("GEAR_Knife", 1, 1)
            //};
            public Tuple<string, int, int>[] AltRemedyItems { get; set; }
            //= new Tuple<string, int, int>[] {
            //    Tuple.Create("GEAR_Name", 1, 1)
            //};

            public bool InstantHeal { get; set; } = true;

            public OverconfidenceAffliction(AfflictionBodyArea bodyArea) : base("Overconfidence", "Yourself", "You are far too confident, you are paying the price for your experience, and you take your survival for granted. Afflictions are now easier to contract.", null, "ico_injury_headache", bodyArea) //customsprite :Minor_Miseries.Resources.Icons.Overconfidence.png
            {
                m_LastUpdateTime = GameManager.GetTimeOfDayComponent().GetHoursPlayedNotPaused();
            }

            public void CureSymptoms()
            {
                //cure symptoms but not the affliction
            }

            public void OnCure()
            {
                IsOvercActive = false;
            }

            public override void OnUpdate()
            {
                IsOvercActive = true;
                var cond = GameManager.GetConditionComponent();
                bool hasAffliction = (cond != null && cond.HasAffliction());
                bool hasCustomAffliction = Minor_Miseries.Core.HasAnyOtherCustomAfflictionThan(typeof(OverconfidenceRiskAffliction), typeof(OverconfidenceAffliction));
                if (hasAffliction || hasCustomAffliction)
                {
                    Cure();
                    IsOvercActive = false;
                    return;
                }
            }
        }
    }
}