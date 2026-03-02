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

            public static bool IsActive { get; private set; } = false;

            public Tuple<string, int, int>[] RemedyItems { get; set; } = Array.Empty<Tuple<string, int, int>>();
            public Tuple<string, int, int>[] AltRemedyItems { get; set; } = Array.Empty<Tuple<string, int, int>>();

            public bool InstantHeal { get; set; } = true;

            public OverconfidenceAffliction(AfflictionBodyArea bodyArea) : base("GAMEPLAY_OverconfidenceName", "GAMEPLAY_OverconfidenceCause", "GAMEPLAY_OverconfidenceDescription", null, "Minor_Miseries.Resources.Icons.Overconfidence.png", bodyArea, true)
            {
            }

            public void CureSymptoms()
            {
                //cure symptoms but not the affliction
            }

            public void OnCure()
            {
                IsActive = false;
            }

            public override void OnUpdate()
            {
                var firstAid = InterfaceManager.GetPanel<Panel_FirstAid>();
                if (firstAid != null && firstAid.isActiveAndEnabled)
                {
                    return;
                }

                IsActive = true;
                var cond = GameManager.GetConditionComponent();
                bool hasAffliction = (cond != null && cond.HasAffliction());
                bool hasCustomAffliction = AfflictionLogic.HasAnyOtherCustomAfflictionThan(typeof(OverconfidenceRiskAffliction), typeof(OverconfidenceAffliction));
                if (hasAffliction || hasCustomAffliction)
                {
                    Cure();
                    IsActive = false;
                    return;
                }
            }
        }
    }
}