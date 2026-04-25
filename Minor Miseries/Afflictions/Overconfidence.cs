using AfflictionComponent.Interfaces;
using AfflictionComponent.Components;
using AfflictionComponent.Enums;
using Minor_Miseries.Resources.Localization;

namespace Minor_Miseries.Afflictions
{
    internal class Overconfidence
    {
        public class OverconfidenceAffliction : CustomAffliction, IRemedies, IInstance, ILocalizableAffliction
        {
            private const string NAME_KEY = "GAMEPLAY_OverconfidenceName";
            private const string CAUSE_KEY = "GAMEPLAY_OverconfidenceCause";
            private const string DESC_KEY = "GAMEPLAY_OverconfidenceDescription";

            private const string ICON = "Minor_Miseries.Resources.Icons.Afflictions.Classic.Overconfidence.png";
            private const string ALT_ICON = "Minor_Miseries.Resources.Icons.Afflictions.Alt.Overconfidence_ALT.png";

            public InstanceType Type { get; set; } = InstanceType.Single;
            public void OnFoundExistingInstance(CustomAffliction existingAffliction)
            {
                return;//Core.Log("splinter duplication");
            }

            public static bool IsActive { get; private set; } = false;

            public Tuple<string, int, int>[] RemedyItems { get; set; } = Array.Empty<Tuple<string, int, int>>();
            public Tuple<string, int, int>[] AltRemedyItems { get; set; } = Array.Empty<Tuple<string, int, int>>();

            public bool InstantHeal { get; set; } = true;

            public OverconfidenceAffliction(AfflictionBodyArea bodyArea) : base(NAME_KEY, CAUSE_KEY, DESC_KEY, null, UnityEngine.Random.Range(0f, 100f) < Settings.options.AltAfflictionIconChance ? ALT_ICON : ICON, bodyArea, true)
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
                bool hasAfflictionNow = cond.HasAffliction() || AfflictionLogic.HasAnyOtherCustomAfflictionForOverconfidence();
                if (hasAfflictionNow)
                {
                    Cure();
                    IsActive = false;
                    return;
                }
            }

            public void RefreshLocalization()
            {
                string oldName = m_Name;

                m_Name = Localization.Get(NAME_KEY);
                m_CauseText = Localization.Get(CAUSE_KEY);
                m_Description = Localization.Get(DESC_KEY);
                m_DescriptionNoHeal = null;

                Core.Log($"Overconfidence refresh -> '{oldName}' => '{m_Name}'");
            }
        }
    }
}