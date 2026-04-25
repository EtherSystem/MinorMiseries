using static Minor_Miseries.Afflictions.Overconfidence;
using AfflictionComponent.Interfaces;
using AfflictionComponent.Components;
using AfflictionComponent.Enums;
using System.Collections;
using Minor_Miseries.Resources.Localization;

namespace Minor_Miseries.Afflictions
{
    internal class OverconfidenceRisk
    {
        public class OverconfidenceRiskAffliction : CustomAffliction, IRemedies, IInstance, IRiskPercentage, ILocalizableAffliction
        {
            private const string NAME_KEY = "GAMEPLAY_OverconfidenceRiskName";
            private const string CAUSE_KEY = "GAMEPLAY_OverconfidenceRiskCause";
            private const string DESC_KEY = "GAMEPLAY_OverconfidenceRiskDescription";

            private const string ICON = "Minor_Miseries.Resources.Icons.Afflictions.Classic.OverconfidenceRisk.png";
            private const string ALT_ICON = "Minor_Miseries.Resources.Icons.Afflictions.Alt.OverconfidenceRisk_ALT.png";

            private static bool IsActive => OverconfidenceAffliction.IsActive;

            public InstanceType Type { get; set; } = InstanceType.Single;
            public void OnFoundExistingInstance(CustomAffliction existingAffliction)
            {
                return;//Core.Log("splinter duplication");
            }

            private float m_RiskValue = 0f;
            private float m_LastUpdateTime;
            public bool Risk { get; set; } = true;

            public Tuple<string, int, int>[] RemedyItems { get; set; } = Array.Empty<Tuple<string, int, int>>();
            public Tuple<string, int, int>[] AltRemedyItems { get; set; } = Array.Empty<Tuple<string, int, int>>();

            public bool InstantHeal { get; set; } = true;

            public OverconfidenceRiskAffliction(AfflictionBodyArea bodyArea) : base(NAME_KEY, CAUSE_KEY, DESC_KEY, null, UnityEngine.Random.Range(0f, 100f) < Settings.options.AltAfflictionIconChance ? ALT_ICON : ICON, bodyArea, true)
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
                    bool hasAfflictionNow = cond.HasAffliction() || AfflictionLogic.HasAnyOtherCustomAfflictionForOverconfidence();

                    if (hasAfflictionNow || (IsActive == true))
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
                AfflictionSaveHelper.QueueSurvivalSave();
            }

            public void RefreshLocalization()
            {
                string oldName = m_Name;

                m_Name = Localization.Get(NAME_KEY);
                m_CauseText = Localization.Get(CAUSE_KEY);
                m_Description = Localization.Get(DESC_KEY);
                m_DescriptionNoHeal = null;

                Core.Log($"OverconfidenceRisk refresh -> '{oldName}' => '{m_Name}'");
            }
        }
    }
}