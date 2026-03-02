using static Minor_Miseries.Afflictions.Overconfidence;
using AfflictionComponent.Interfaces;
using AfflictionComponent.Components;
using AfflictionComponent.Enums;
using System.Collections;

namespace Minor_Miseries.Afflictions
{
    internal class OverconfidenceRisk
    {
        public class OverconfidenceRiskAffliction : CustomAffliction, IRemedies, IInstance, IRiskPercentage
        {
            private static bool IsActive => OverconfidenceAffliction.IsActive;

            public InstanceType Type { get; set; } = InstanceType.Single;
            public void OnFoundExistingInstance(CustomAffliction existingAffliction)
            {
                return;//MelonLogger.Msg("splinter duplication");
            }

            private float m_RiskValue = 0f;
            private float m_LastUpdateTime;
            public bool Risk { get; set; } = true;

            public Tuple<string, int, int>[] RemedyItems { get; set; } = Array.Empty<Tuple<string, int, int>>();
            public Tuple<string, int, int>[] AltRemedyItems { get; set; } = Array.Empty<Tuple<string, int, int>>();

            public bool InstantHeal { get; set; } = true;

            public OverconfidenceRiskAffliction(AfflictionBodyArea bodyArea) : base("GAMEPLAY_OverconfidenceRiskName", "GAMEPLAY_OverconfidenceRiskCause", "GAMEPLAY_OverconfidenceRiskDescription", null, "Minor_Miseries.Resources.Icons.OverconfidenceRisk.png", bodyArea, true)
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
                var firstAid = InterfaceManager.GetPanel<Panel_FirstAid>();
                if (firstAid != null && firstAid.isActiveAndEnabled)
                {
                    return;
                }

                if (!Risk)
                { 
                    return;
                }
                else if (Risk)
                {
                    var cond = GameManager.GetConditionComponent();
                    bool hasAffliction = (cond != null && cond.HasAffliction());
                    bool hasCustomAffliction = AfflictionLogic.HasAnyOtherCustomAfflictionThan(typeof(OverconfidenceRiskAffliction), typeof(OverconfidenceAffliction));

                    if (hasAffliction || hasCustomAffliction || (IsActive == true))
                    {
                        Cure();
                        m_LastUpdateTime = GameManager.GetTimeOfDayComponent().GetHoursPlayedNotPaused();
                        return;
                    }
                    else if (GetRiskValue() >= 100)
                    {
                        Cure(false);
                        MelonCoroutines.Start(StartOverconfidenceNextFrame());
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

            private IEnumerator StartOverconfidenceNextFrame()
            {
                yield return null;
                new OverconfidenceAffliction(AfflictionBodyArea.Head).Start();
            }
        }
    }
}