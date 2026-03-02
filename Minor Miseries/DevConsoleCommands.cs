using static Minor_Miseries.Afflictions.ShoulderTrauma;
using static Minor_Miseries.Afflictions.OverconfidenceRisk;
using static Minor_Miseries.Afflictions.WristTrauma;
using static Minor_Miseries.Afflictions.Overconfidence;
using static Minor_Miseries.Afflictions.SensitiveHand;
using static Minor_Miseries.Afflictions.NightTerror;
using static Minor_Miseries.Afflictions.StuckFood;
using static Minor_Miseries.Afflictions.SoreNeck;
using static Minor_Miseries.Afflictions.BackPain;
using static Minor_Miseries.Afflictions.BadDream;
using static Minor_Miseries.Afflictions.BareSkin;
using static Minor_Miseries.Afflictions.SmallCut;
using static Minor_Miseries.Afflictions.Splinter;
using static Minor_Miseries.Afflictions.Blister;
using static Minor_Miseries.Afflictions.Scratch;
using AfflictionComponent.Components;

namespace Minor_Miseries
{
    internal static class DevConsoleCommands
    {
        internal static void Register()
        {
            uConsole.RegisterCommand("mm_afflictions", new Action(() =>
            {
                new SplinterAffliction(AfflictionBodyArea.HandLeft).Start();
                new StuckFoodAffliction(AfflictionBodyArea.Head).Start();
                new BlisterAffliction(AfflictionBodyArea.FootLeft).Start();
                new BackPainAffliction(AfflictionBodyArea.Chest).Start();
                new ScratchAffliction(AfflictionBodyArea.Chest).Start();
                new BadDreamAffliction(AfflictionBodyArea.Head).Start();
                new SensitiveHandAffliction(AfflictionBodyArea.HandLeft).Start();
                new BareSkinAffliction(AfflictionBodyArea.FootRight).Start();
                new SmallCutAffliction(AfflictionBodyArea.Chest).Start();
                new WristTraumaAffliction(AfflictionBodyArea.HandRight).Start();
                new ShoulderTraumaAffliction(AfflictionBodyArea.Chest).Start();
                new NightTerrorAffliction(AfflictionBodyArea.Head).Start();
                new SoreNeckAffliction(AfflictionBodyArea.Neck).Start();
            }));

            uConsole.RegisterCommand("splinter", new Action(() =>
            {
                new SplinterAffliction(AfflictionBodyArea.HandLeft).Start();
            }));

            uConsole.RegisterCommand("stuckfood", new Action(() =>
            {
                new StuckFoodAffliction(AfflictionBodyArea.Head).Start();
            }));

            uConsole.RegisterCommand("blister", new Action(() =>
            {
                new BlisterAffliction(AfflictionBodyArea.FootLeft).Start();
            }));

            uConsole.RegisterCommand("backpain", new Action(() =>
            {
                new BackPainAffliction(AfflictionBodyArea.Chest).Start();
            }));

            uConsole.RegisterCommand("scratch", new Action(() =>
            {
                new ScratchAffliction(AfflictionBodyArea.Chest).Start();
            }));

            uConsole.RegisterCommand("baddream", new Action(() =>
            {
                new BadDreamAffliction(AfflictionBodyArea.Head).Start();
            }));

            uConsole.RegisterCommand("nightterror", new Action(() =>
            {
                new NightTerrorAffliction(AfflictionBodyArea.Head).Start();
            }));

            uConsole.RegisterCommand("sensitivehand", new Action(() =>
            {
                new SensitiveHandAffliction(AfflictionBodyArea.HandLeft).Start();
            }));

            uConsole.RegisterCommand("bareskin", new Action(() =>
            {
                new BareSkinAffliction(AfflictionBodyArea.FootRight).Start();
            }));

            uConsole.RegisterCommand("smallcut", new Action(() =>
            {
                new SmallCutAffliction(AfflictionBodyArea.Chest).Start();
            }));

            uConsole.RegisterCommand("wristrecoil", new Action(() =>
            {
                new WristTraumaAffliction(AfflictionBodyArea.HandRight).Start();
            }));

            uConsole.RegisterCommand("shoulderrecoil", new Action(() =>
            {
                new ShoulderTraumaAffliction(AfflictionBodyArea.Chest).Start();
            }));

            uConsole.RegisterCommand("soreneck", new Action(() =>
            {
                new SoreNeckAffliction(AfflictionBodyArea.Neck).Start();
            }));

            uConsole.RegisterCommand("mm_afflictions_cure", new Action(() =>
            {
                var mgr = AfflictionManager.GetAfflictionManagerInstance();
                if (mgr?.m_Afflictions == null) return;

                for (int i = mgr.m_Afflictions.Count - 1; i >= 0; i--)
                {
                    var a = mgr.m_Afflictions[i];
                    if (a == null) continue;

                    if (a is SplinterAffliction
                        || a is StuckFoodAffliction
                        || a is BlisterAffliction
                        || a is BackPainAffliction
                        || a is ScratchAffliction
                        || a is BadDreamAffliction
                        || a is NightTerrorAffliction
                        || a is SensitiveHandAffliction
                        || a is BareSkinAffliction
                        || a is SmallCutAffliction
                        || a is WristTraumaAffliction
                        || a is ShoulderTraumaAffliction
                        || a is SoreNeckAffliction)
                    {
                        a.Cure();
                    }
                }
            }));

            uConsole.RegisterCommand("overcrisk", new Action(() =>
            {
                new OverconfidenceRiskAffliction(AfflictionBodyArea.Head).Start();
            }));

            uConsole.RegisterCommand("overc", new Action(() =>
            {
                new OverconfidenceAffliction(AfflictionBodyArea.Head).Start();
            }));

            uConsole.RegisterCommand("overc_cure", new Action(() =>
            {
                var mgr = AfflictionManager.GetAfflictionManagerInstance();
                if (mgr?.m_Afflictions == null) return;

                for (int i = mgr.m_Afflictions.Count - 1; i >= 0; i--)
                {
                    var a = mgr.m_Afflictions[i];
                    if (a == null) continue;

                    if (a is OverconfidenceRiskAffliction || a is OverconfidenceAffliction)
                        a.Cure();
                }
            }));
        }
    }
}