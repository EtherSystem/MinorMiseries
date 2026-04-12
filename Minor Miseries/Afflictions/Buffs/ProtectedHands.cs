using AfflictionComponent.Components;
using AfflictionComponent.Enums;
using AfflictionComponent.Interfaces;
using Minor_Miseries.Resources.Localization;

namespace Minor_Miseries.Afflictions.Buffs
{
    public class ProtectedHandsBuff : CustomAffliction, IInstance, IBuff, ILocalizableAffliction
    {
        private const string NAME_KEY = "GAMEPLAY_ProtectedHandsName";
        private const string CAUSE_KEY = "GAMEPLAY_ProtectedHandsCause";
        private const string DESC_KEY = "GAMEPLAY_ProtectedHandsDescription";

        private const string ICON = "Minor_Miseries.Resources.Icons.Buffs.Classic.ProtectedHands.png";
        private const string ALT_ICON = "Minor_Miseries.Resources.Icons.Buffs.Alt.ProtectedHands_ALT.png";

        public void OnFoundExistingInstance(CustomAffliction existing)
        {
            IsActive = true;
        }

        public InstanceType Type { get; set; } = InstanceType.Single;
        public bool Buff { get; set; } = true;
        public bool BuffCold { get; set; }
        public bool BuffFatigue { get; set; }
        public bool BuffHunger { get; set; }
        public bool BuffThirst { get; set; }
        public static bool IsActive { get; private set; }

        public ProtectedHandsBuff() : base(NAME_KEY, CAUSE_KEY, DESC_KEY, null, UnityEngine.Random.Range(0f, 100f) < Settings.options.AltAfflictionIconChance ? ALT_ICON : ICON, AfflictionBodyArea.HandLeft, true)
        {
        }

        public void OnCure()
        {
            IsActive = false;
        }

        public override void OnUpdate()
        {
            IsActive = true;
        }

        public void RefreshLocalization()
        {
            string oldName = m_Name;

            m_Name = Localization.Get(NAME_KEY);
            m_CauseText = Localization.Get(CAUSE_KEY);
            m_Description = Localization.Get(DESC_KEY);
            m_DescriptionNoHeal = null;

            Core.Log($"ProtectedHands refresh -> '{oldName}' => '{m_Name}'");
        }
    }
}