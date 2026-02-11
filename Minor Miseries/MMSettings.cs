using Description = ModSettings.DescriptionAttribute;
using System.ComponentModel;

namespace Minor_Miseries
{
    internal class MMSettings : JsonModSettings
    {
        [Section("Evolving Afflictions")]

        [Name("Splinter")]
        [Description("base = yes")]
        public bool IsSplinter = true;

        [Name("Sensitive Hand")]
        [Description("base = yes - Requires Splinter")]
        public bool IsSensi = true;

        [Name("Blister")]
        [Description("base = yes")]
        public bool IsBlister = true;

        [Name("Bare Skin")]
        [Description("base = yes - Requires Blister")]
        public bool IsBareSkin = true;

        [Name("Scratch")]
        [Description("base = yes")]
        public bool IsScratch = true;

        [Name("Small Cut")]
        [Description("base = yes - Requires Scratch")]
        public bool IsSmallCut = true;

        [Section("Afflictions")]

        [Name("Stuck Food")]
        [Description("base = yes")]
        public bool IsStuckFood = true;

        [Name("Back Pain")]
        [Description("base = yes")]
        public bool IsBackPain = true;

        [Name("Bad Dream")]
        [Description("base = yes")]
        public bool IsBadDream = true;

        [Name("Wrist Trauma")]
        [Description("base = yes")]
        public bool IsWristRecoil = true;

        [Name("Shoulder Trauma")]
        [Description("base = yes")]
        public bool IsShoulderRecoil = true;

        [Name("Overconfidence")]
        [Description("base = yes")]
        public bool IsOverconfidence = true;

        [Section("Affliction Duration")]

        [Name("Splinter duration")]
        [Description("base = 24 hours")]
        [Slider(1, 48, 48)]
        public float SplinterDuration = 24f;

        [Name("Sensitive Hand duration")]
        [Description("base = 24 hours")]
        [Slider(1, 48, 48)]
        public float SensiDuration = 24f;

        [Name("Stuck Food duration")]
        [Description("base = 2 hours")]
        [Slider(1, 48, 48)]
        public float StuckFoodDuration = 2f;

        [Name("Blister duration")]
        [Description("base = 30 hours")]
        [Slider(1, 48, 48)]
        public float BlisterDuration = 30f;

        [Name("Bare Skin duration")]
        [Description("base = 48 hours")]
        [Slider(1, 48, 48)]
        public float BareSkinDuration = 48f;

        [Name("Back Pain duration")]
        [Description("base = 6 hours")]
        [Slider(1, 48, 48)]
        public float BackPainDuration = 6f;

        [Name("Scratch duration")]
        [Description("base = 40 hours")]
        [Slider(1, 48, 48)]
        public float ScratchDuration = 40f;

        [Name("Small Cut duration")]
        [Description("base = 48 hours")]
        [Slider(1, 48, 48)]
        public float SmallCutDuration = 48f;

        [Name("Wrist Recoil Injury duration")]
        [Description("base = 50 hours")]
        [Slider(1, 50, 50)]
        public float WristRecoilDuration = 50f;

        [Name("Shoulder Recoil Injury duration")]
        [Description("base = 60 hours")]
        [Slider(1, 60, 60)]
        public float ShoulderRecoilDuration = 60f;

        [Name("Bad Dream duration")]
        [Description("base = 0.25 hour (15 min)")]
        [Slider(0.1f, 1f, 60)]
        public float BadDreamDuration = 0.25f;

        protected override void OnChange(FieldInfo field, object oldValue, object newValue)
        {
            if (field.Name == nameof(IsSplinter))
                Settings.ToggleSplinter((bool)newValue);

            if (field.Name == nameof(IsBlister))
                Settings.ToggleBlister((bool)newValue);

            if (field.Name == nameof(IsScratch))
                Settings.ToggleScratch((bool)newValue);

            base.OnConfirm();
        }
    }

    internal static class Settings
    {
        public static MMSettings options;

        public static void OnLoad()
        {
            options = new MMSettings();
            options.AddToModSettings("Minor Miseries");

            ToggleSplinter(options.IsSplinter);
            ToggleBlister(options.IsBlister);
            ToggleScratch(options.IsScratch);
        }

        internal static void ToggleSplinter(bool enabled)
        {
            if (!enabled)
            {
                options.IsSensi = false;
            }

            options.SetFieldVisible(nameof(options.IsSensi), enabled);
        }

        internal static void ToggleBlister(bool enabled)
        {
            if (!enabled)
            {
                options.IsBareSkin = false;
            }

            options.SetFieldVisible(nameof(options.IsBareSkin), enabled);
        }

        internal static void ToggleScratch(bool enabled)
        {
            if (!enabled)
            {
                options.IsSmallCut = false;
            }

            options.SetFieldVisible(nameof(options.IsSmallCut), enabled);
        }
    }
}
