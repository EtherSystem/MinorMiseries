using Description = ModSettings.DescriptionAttribute;
using System.ComponentModel;

namespace Minor_Miseries
{
    internal class MMSettings : JsonModSettings
    {
        [Section("Affliction Settings")]

        [Name("Splinter")]
        [Description("base = yes")]
        public bool IsSplinter = true;

        [Name("Stuck food")]
        [Description("base = yes")]
        public bool IsStuckFood = true;

        [Name("Blister")]
        [Description("base = yes")]
        public bool IsBlister = true;

        [Name("Back pain")]
        [Description("base = yes")]
        public bool IsBackPain = true;

        [Name("Scratch")]
        [Description("base = yes")]
        public bool IsScratch = true;

        [Name("Bad Dream")]
        [Description("base = yes")]
        public bool IsBadDream = true;

        [Name("Overconfidence")]
        [Description("base = yes")]
        public bool IsOverconfidence = true;

        [Section("Affliction Duration")]

        [Name("Splinter duration")]
        [Description("base = 24 hours")]
        [Slider(1, 48, 48)]
        public float SplinterDuration = 24f;

        [Name("Stuck food duration")]
        [Description("base = 2 hours")]
        [Slider(1, 48, 48)]
        public float StuckFoodDuration = 2f;

        [Name("Blister duration")]
        [Description("base = 30 hours")]
        [Slider(1, 48, 48)]
        public float BlisterDuration = 30f;

        [Name("Back pain duration")]
        [Description("base = 6 hours")]
        [Slider(1, 48, 48)]
        public float BackPainDuration = 6f;

        [Name("Scratch duration")]
        [Description("base = 40 hours")]
        [Slider(1, 48, 48)]
        public float ScratchDuration = 40f;

        [Name("Bad Dream duration")]
        [Description("base = 0.25 hour (15min)")]
        [Slider (0.1f, 1f, 60)]
        public float BadDreamDuration = 0.25f;

        protected override void OnConfirm()
        {
            base.OnConfirm();
        }
    }
    internal static class Settings
    {
        public static MMSettings options;

        public static void OnLoad()
        {
            options = new MMSettings();
            options.AddToModSettings("Minor_Miseries");
        }
    }
}
