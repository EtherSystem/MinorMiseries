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

        [Name("Sore Neck")]
        [Description("base = yes")]
        public bool IsSoreNeck = true;

        [Name("Bad Dream & Night Terror")]
        [Description("base = yes")]
        public bool IsBadDream = true;

        [Name("Wrist Trauma")]
        [Description("base = yes")]
        public bool IsWristTrauma = true;

        [Name("Shoulder Trauma")]
        [Description("base = yes")]
        public bool IsShoulderTrauma = true;

        [Name("Overconfidence")]
        [Description("base = yes")]
        public bool IsOverconfidence = true;

        [Section("Affliction Duration")]

        [Name("Splinter duration")]
        [Description("base = 24 hours")]
        [Slider(1, 48, 47, NumberFormat = "{0:0}h")]
        public int SplinterDuration = 24;

        [Name("Sensitive Hand duration")]
        [Description("base = 24 hours")]
        [Slider(1, 48, 47, NumberFormat = "{0:0}h")]
        public int SensiDuration = 24;

        [Name("Stuck Food duration")]
        [Description("base = 2 hours")]
        [Slider(1, 48, 47, NumberFormat = "{0:0}h")]
        public int StuckFoodDuration = 2;

        [Name("Blister duration")]
        [Description("base = 30 hours")]
        [Slider(1, 48, 47, NumberFormat = "{0:0}h")]
        public int BlisterDuration = 30;

        [Name("Bare Skin duration")]
        [Description("base = 48 hours")]
        [Slider(1, 48, 47, NumberFormat = "{0:0}h")]
        public int BareSkinDuration = 48;

        [Name("Back Pain duration")]
        [Description("base = 6 hours")]
        [Slider(1, 48, 47, NumberFormat = "{0:0}h")]
        public int BackPainDuration = 6;

        [Name("Sore Neck duration")]
        [Description("base = 6 hours")]
        [Slider (1, 24, 23, NumberFormat = "{0:0}h")]
        public int SoreNeckDuration = 6;

        [Name("Scratch duration")]
        [Description("base = 40 hours")]
        [Slider(1, 48, 47, NumberFormat = "{0:0}h")]
        public int ScratchDuration = 40;

        [Name("Small Cut duration")]
        [Description("base = 48 hours")]
        [Slider(1, 48, 47, NumberFormat = "{0:0}h")]
        public int SmallCutDuration = 48;

        [Name("Wrist Recoil Injury duration")]
        [Description("base = 50 hours")]
        [Slider(1, 50, 49, NumberFormat = "{0:0}h")]
        public int WristTraumaDuration = 50;

        [Name("Shoulder Recoil Injury duration")]
        [Description("base = 60 hours")]
        [Slider(1, 60, 59, NumberFormat = "{0:0}h")]
        public int ShoulderTraumaDuration = 60;

        [Name("Bad Dream duration")]
        [Description("base = 15 min - consider that Night Terror lasts twice as long")]
        [Slider(1f, 60f, 59, NumberFormat = "{0:0}min")]
        public int BadDreamDurationMinutes = 15;

        public float BadDreamDuration => BadDreamDurationMinutes / 60f;

        [Section("Advanced")]

        [Name("ML Logging")]
        [Description("Add logs for ModData in the ML console.")]
        public bool IsLogging = false;

        protected override void OnChange(FieldInfo field, object? oldValue, object? newValue)
        {
            if (field.Name == nameof(IsSplinter) && newValue is bool splinter) Settings.ToggleSplinter(splinter);

            if (field.Name == nameof(IsBlister) && newValue is bool blister) Settings.ToggleBlister(blister);

            if (field.Name == nameof(IsScratch) && newValue is bool scratch) Settings.ToggleScratch(scratch);

            base.OnChange(field, oldValue, newValue);
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
