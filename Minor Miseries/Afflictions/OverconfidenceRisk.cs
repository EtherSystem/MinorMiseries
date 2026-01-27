using static Minor_Miseries.Afflictions.Overconfidence;
using AfflictionComponent.Components;
using AfflictionComponent.Interfaces;
using AfflictionComponent.Enums;

namespace Minor_Miseries.Afflictions
{
    internal class OverconfidenceRisk
    {
        public class OverconfidenceRiskAffliction : CustomAffliction, IRemedies, IInstance, IRiskPercentage
        {
            private static readonly bool IsOvercActive = Overconfidence.OverconfidenceAffliction.IsOvercActive;
            public InstanceType Type { get; set; } = InstanceType.SingleLocation;
            public void OnFoundExistingInstance(CustomAffliction existingAffliction)
            {
                return;//MelonLogger.Msg("splinter duplication");
            }

            private float m_RiskValue = 0f;
            private float m_LastUpdateTime;
            public bool Risk { get; set; } = true;

            public Tuple<string, int, int>[] RemedyItems { get; set; }
            //= new Tuple<string, int, int>[] {
            //    Tuple.Create("GEAR_Knife", 1, 1)
            //};
            public Tuple<string, int, int>[] AltRemedyItems { get; set; }
            //= new Tuple<string, int, int>[] {
            //    Tuple.Create("GEAR_Name", 1, 1)
            //};

            public bool InstantHeal { get; set; } = true;

            public OverconfidenceRiskAffliction(AfflictionBodyArea bodyArea) : base("Overconfidence Risk", "Yourself", "Everything seems so simple, doesn't it ?", null, "ico_injury_pain", bodyArea) //customsprite :Minor_Miseries.Resources.Icons.OverconfidenceRisk.png
            {
                m_LastUpdateTime = GameManager.GetTimeOfDayComponent().GetHoursPlayedNotPaused();
            }

            public void CureSymptoms()
            {
                //cure symptoms but not the affliction
            }

            public void OnCure()
            {
                //when the affliction is cured, apply this code
            }

            public float GetRiskValue() => m_RiskValue;

            public override void OnUpdate()
            {
                if (Risk)
                {
                    var cond = GameManager.GetConditionComponent();
                    bool hasAffliction = (cond != null && cond.HasAffliction());
                    bool hasCustomAffliction = Minor_Miseries.Core.HasAnyOtherCustomAfflictionThan(typeof(OverconfidenceRiskAffliction), typeof(OverconfidenceAffliction));

                    if (hasAffliction || hasCustomAffliction || (IsOvercActive == true))
                    {
                        Cure();
                        m_LastUpdateTime = GameManager.GetTimeOfDayComponent().GetHoursPlayedNotPaused();
                        return;
                    }
                    else if (GetRiskValue() >= 100)
                    {
                        Cure(false);
                        var overconfidenceaff = new OverconfidenceAffliction(AfflictionBodyArea.Head);
                        overconfidenceaff.Start();
                        return;
                    }
                    else if (GetRiskValue() < 0f)
                    {
                        Cure();
                        return;
                    }
                    UpdateRiskValue();
                }
            }
            public void UpdateRiskValue()
            {
                var currentTime = GameManager.GetTimeOfDayComponent().GetHoursPlayedNotPaused();
                var elapsedTime = currentTime - m_LastUpdateTime;

                var riskIncrease = elapsedTime * 60f;

                m_RiskValue = Mathf.Min(m_RiskValue + riskIncrease, 100f);
                m_LastUpdateTime = currentTime;

                // Mod.Logger.Log($"Risk for {m_AfflictionKey} increased to {m_RiskPercentage:F2}%", ComplexLogger.FlaggedLoggingLevel.Debug);
            }
        }
    }
}