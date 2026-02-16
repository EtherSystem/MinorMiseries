using static Minor_Miseries.Afflictions.OverconfidenceRisk;
using AfflictionComponent.Interfaces;
using AfflictionComponent.Components;
using AfflictionComponent.Enums;

namespace Minor_Miseries.Afflictions
{
    internal class Overconfidence
    {
        public class OverconfidenceAffliction : CustomAffliction, IRemedies, IInstance
        {
            public InstanceType Type { get; set; } = InstanceType.Single;
            public void OnFoundExistingInstance(CustomAffliction existingAffliction)
            {
                return;//MelonLogger.Msg("splinter duplication");
            }

            public static bool IsOvercActive { get; private set; } = false;

            public Tuple<string, int, int>[] RemedyItems { get; set; } = Array.Empty<Tuple<string, int, int>>();
            public Tuple<string, int, int>[] AltRemedyItems { get; set; } = Array.Empty<Tuple<string, int, int>>();

            public bool InstantHeal { get; set; } = true;

            public OverconfidenceAffliction(AfflictionBodyArea bodyArea) : base("Overconfidence", "Yourself", "You are far too confident, you are paying the price for your experience, and you take your survival for granted. Afflictions are now easier to contract.", null, "ico_injury_headache", bodyArea) //customsprite :Minor_Miseries.Resources.Icons.Overconfidence.png
            {
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
                var firstAid = InterfaceManager.GetPanel<Panel_FirstAid>();
                if (firstAid != null && firstAid.isActiveAndEnabled)
                {
                    return;
                }

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