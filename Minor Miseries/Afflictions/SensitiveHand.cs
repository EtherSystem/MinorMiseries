using AfflictionComponent.Interfaces;
using AfflictionComponent.Components;
using AfflictionComponent.Enums;

namespace Minor_Miseries.Afflictions
{
    internal class SensitiveHand
    {
        public class SensitiveHandAffliction : CustomAffliction, IDuration, IRemedies, IInstance
        {
            public InstanceType Type { get; set; } = InstanceType.Single;
            public void OnFoundExistingInstance(CustomAffliction existingAffliction)
            {
                //MelonLogger.Msg("sensitive hand duplication");
                if (existingAffliction is SensitiveHandAffliction sensitiveHand)
                {
                    sensitiveHand.ResetAffliction(resetRemedies: false);
                    var now = GameManager.GetTimeOfDayComponent().GetHoursPlayedNotPaused();
                    sensitiveHand.EndTime = now + sensitiveHand.Duration;
                }
            }

            private readonly float m_LastUpdateTime;
            public static bool IsSensiActive { get; private set; } = false;
            public float Duration { get; set; } = Settings.options.SensiDuration;
            public float EndTime { get; set; }

            public Tuple<string, int, int>[] RemedyItems { get; set; } = Array.Empty<Tuple<string, int, int>>();
            public Tuple<string, int, int>[] AltRemedyItems { get; set; } = Array.Empty<Tuple<string, int, int>>();

            public bool InstantHeal { get; set; } = true;

            public SensitiveHandAffliction(AfflictionBodyArea bodyArea) : base("Sensitive Hand", "Local inflammation", "The splinter in your hand has caused irritation, your hand remains sensitive and easily irritated, making your actions more difficult.", null, "ico_injury_sprainedWrist", bodyArea) //customsprite :Minor_Miseries.Resources.Icons.StuckFood.png
            {
                m_LastUpdateTime = GameManager.GetTimeOfDayComponent().GetHoursPlayedNotPaused();
            }

            public void CureSymptoms()
            {
                //cure symptoms but not the affliction
            }

            public void OnCure()
            {
                IsSensiActive = false;
            }

            public override void OnUpdate()
            {
                IsSensiActive = true;
            }

            [HarmonyPatch(typeof(Panel_Crafting), nameof(Panel_Crafting.GetModifiedCraftingDuration))]
            private static class CraftingDurationPatch
            {
                private static void Postfix(ref int __result)
                {
                    if (!IsSensiActive) return;

                    if (IsSensiActive)
                    {
                        __result = (int)(__result * 1.3f);
                    }
                }
            }
            [HarmonyPatch(typeof(vp_FPSController), nameof(vp_FPSController.GetSlopeMultiplier))]
            internal static class MovementSpeedPatch
            {
                private static void Postfix(ref float __result)
                {
                    if (!IsSensiActive) return;

                    var pm = GameManager.GetPlayerManagerComponent();
                    if (IsSensiActive && (pm.PlayerIsClimbing()))
                    {
                        __result *= 0.7f;
                    }
                }
            }
        }
    }
}