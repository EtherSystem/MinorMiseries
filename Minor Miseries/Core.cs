using static Minor_Miseries.Afflictions.OverconfidenceRisk;
using static Minor_Miseries.Afflictions.Overconfidence;
using static Minor_Miseries.Afflictions.StuckFood;
using static Minor_Miseries.Afflictions.Splinter;
using static Minor_Miseries.Afflictions.BackPain;
using static Minor_Miseries.Afflictions.Blister;
using static Minor_Miseries.Afflictions.Scratch;
using static Minor_Miseries.Afflictions.BadDream;
using AfflictionComponent.Components;
using LocalizationUtilities;

[assembly: MelonInfo(typeof(Minor_Miseries.Core), "Minor Miseries", "1.1.1", "EtherSystem", null)]
[assembly: MelonGame("Hinterland", "TheLongDark")]

namespace Minor_Miseries
{
    public class Core : MelonMod
    {
        public static string? LoadEmbeddedJSON(string Localization)
        {
            string? result = null;

            Stream? stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Minor_Miseries.Resources.Localization.Localization.json");
            if (stream != null)
            {
                StreamReader reader = new StreamReader(stream);
                result = reader.ReadToEnd();
            }
            return result;
        }
        public override void OnInitializeMelon()
        {
            LocalizationManager.LoadJsonLocalization(LoadEmbeddedJSON("Localization.json"));
            LoggerInstance.Msg("Initialized.");
            Settings.OnLoad();

            //dev console commands
            uConsole.RegisterCommand("mm_afflictions", new Action(() =>
            {
                new SplinterAffliction(AfflictionBodyArea.HandLeft).Start();
                new StuckFoodAffliction(AfflictionBodyArea.Head).Start();
                new BlisterAffliction(AfflictionBodyArea.FootLeft).Start();
                new BackPainAffliction(AfflictionBodyArea.Chest).Start();
                new ScratchAffliction(AfflictionBodyArea.Chest).Start();
                new BadDreamAffliction(AfflictionBodyArea.Head).Start();
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

            uConsole.RegisterCommand("mm_afflictions_cure", new Action(() =>
            {
                var mgr = AfflictionManager.GetAfflictionManagerInstance();
                if (mgr?.m_Afflictions == null) return;

                for (int i = mgr.m_Afflictions.Count - 1; i >= 0; i--)
                {
                    var a = mgr.m_Afflictions[i];
                    if (a == null) continue;

                    if (a is SplinterAffliction || a is StuckFoodAffliction || a is BlisterAffliction || a is BackPainAffliction || a is ScratchAffliction || a is BadDreamAffliction)
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

        private readonly System.Random _random = new();
        private float updateTimer = 0f;
        private float hoursSinceLastAffliction = 0f;
        private float hoursSpentMoving = 0f;
        private float hoursOverloaded = 0f;
        private bool wasResting = false;
        private float badDreamRollTimer = 0f;
        private const float CONF_THRESHOLD_HOURS = 2f;
        private const float BLIST_THRESHOLD_HOURS = 4f;
        private const float OVERC_BLIST_THRESHOLD_HOURS = 3f;
        private const float BACKPAIN_TRESHOLD_HOURS = 1f;
        private const float OVERC_BACKPAIN_TRESHOLD_HOURS = 0.5f;
        private const float STUCKFOOD_CHANCE = 5f;
        private const float OVERC_STUCKFOOD_CHANCE = 10f;
        private const float BAD_DREAM_CHANCE = 4f;
        private const float OVERC_BAD_DREAM_CHANCE = 8f;
        private const float BAD_DREAM_ROLL_EVERY = 1f;
        private bool hadAfflictionLastTick = false;
        private bool wasEating = false;

        public static bool HasAnyOtherCustomAfflictionThan(Type ignoredType1, Type ignoredType2)
        {
            var mgr = AfflictionManager.GetAfflictionManagerInstance();
            if (mgr?.m_Afflictions == null) return false;

            foreach (var a in mgr.m_Afflictions)
            {
                if (a == null) continue;

                var t = a.GetType();
                if (t == ignoredType1 || t == ignoredType2)
                    continue;

                return true;
            }
            return false;
        }
        public static bool IsPlayerOverloaded()
        {
            var enc = GameManager.GetEncumberComponent();
            if (enc == null) return false;
            return enc.m_GearWeightKG > enc.m_MaxCarryCapacity;
        }

        public override void OnUpdate()
        {
            if (GameManager.m_Instance == null) return;
            string scene = GameManager.m_ActiveScene;
            if (scene == "MainMenu" || scene == "Boot" || scene == "Empty") return;

            var cond = GameManager.GetConditionComponent();

            var firstAid = InterfaceManager.GetPanel<Panel_FirstAid>();
            if (firstAid != null && firstAid.isActiveAndEnabled)
            {
                return;
            }

            //clock
            updateTimer += Time.deltaTime;
            if (updateTimer < 1.0f) return;
            float realTimeElapsed = updateTimer;
            updateTimer = 0f;
            float gameHoursPassed = GameManager.GetTimeOfDayComponent().GetTODHours(realTimeElapsed);
            if (cond == null) return;

            //StuckFood conditions
            var hunger = GameManager.GetHungerComponent();
            if (hunger == null) return;
            bool isEating = hunger.IsEatingInProgress();

            //Blister conditions
            bool isWalking = GameManager.GetPlayerManagerComponent().PlayerIsWalking();
            bool isSprinting = GameManager.GetPlayerManagerComponent().PlayerIsSprinting();
            bool isMoving = (isWalking || isSprinting);
            if (isMoving) hoursSpentMoving += gameHoursPassed;
            else hoursSpentMoving = 0f;

            //back pain conditions
            bool overloaded = IsPlayerOverloaded();
            if (overloaded) hoursOverloaded += gameHoursPassed;
            else hoursOverloaded = 0f;

            //Overconfidence conditions
            bool hasAfflictionNow = (cond.HasAffliction() || HasAnyOtherCustomAfflictionThan(typeof(OverconfidenceRiskAffliction), typeof(OverconfidenceAffliction)));
            if (!hadAfflictionLastTick && hasAfflictionNow) hoursSinceLastAffliction = 0f;
            if (!hasAfflictionNow) hoursSinceLastAffliction += gameHoursPassed;
            hadAfflictionLastTick = hasAfflictionNow;

            //stuck food
            if (Settings.options.IsStuckFood && wasEating && !isEating)
            {
                if (OverconfidenceAffliction.IsOvercActive)
                {
                    float roll = UnityEngine.Random.Range(0f, 100f);
                    if (roll < OVERC_STUCKFOOD_CHANCE)
                    {
                        //MelonLogger.Msg("some overc food got stuck");
                        new StuckFoodAffliction(AfflictionBodyArea.Head).Start();
                    }
                }
                else
                {
                    float roll = UnityEngine.Random.Range(0f, 100f);
                    if (roll < STUCKFOOD_CHANCE)
                    {
                        //MelonLogger.Msg("some food got stuck");
                        new StuckFoodAffliction(AfflictionBodyArea.Head).Start();
                    }
                }
            }
            wasEating = isEating;

            //blister
            if (Settings.options.IsBlister)
            {
                if (OverconfidenceAffliction.IsOvercActive && hoursSpentMoving >= OVERC_BLIST_THRESHOLD_HOURS)
                {
                    var side = _random.Next(0, 2) == 0
                        ? AfflictionBodyArea.FootLeft : AfflictionBodyArea.FootRight;
                    //MelonLogger.Msg("an overc blister appeared");
                    new BlisterAffliction(AfflictionBodyArea.FootLeft).Start();
                }
                else if (!OverconfidenceAffliction.IsOvercActive && hoursSpentMoving >= BLIST_THRESHOLD_HOURS)
                {
                    var side = _random.Next(0, 2) == 0
                        ? AfflictionBodyArea.FootLeft : AfflictionBodyArea.FootRight;
                    //MelonLogger.Msg("a blister appeared");
                    new BlisterAffliction(AfflictionBodyArea.FootRight).Start();
                }
            }

            //back pain
            if (Settings.options.IsBackPain)
            {
                if (OverconfidenceAffliction.IsOvercActive && (hoursOverloaded >= OVERC_BACKPAIN_TRESHOLD_HOURS))
                {
                    //MelonLogger.Msg("you have an overc back pain");
                    new BackPainAffliction(AfflictionBodyArea.Chest).Start();
                    hoursOverloaded = 0f;
                }
                else if (!OverconfidenceAffliction.IsOvercActive && (hoursOverloaded >= BACKPAIN_TRESHOLD_HOURS))
                {
                    //MelonLogger.Msg("you have back pain");
                    new BackPainAffliction(AfflictionBodyArea.Chest).Start();
                    hoursOverloaded = 0f;
                }
            }

            //bad dream
            if (Settings.options.IsBadDream)
            {
                var rest = GameManager.GetRestComponent();
                if (rest == null) return;

                bool isSleeping = rest.IsSleeping();

                if (isSleeping)
                {
                    badDreamRollTimer += realTimeElapsed;

                    if (badDreamRollTimer >= BAD_DREAM_ROLL_EVERY)
                    {
                        badDreamRollTimer = 0f;
                        float chance = OverconfidenceAffliction.IsOvercActive
                            ? OVERC_BAD_DREAM_CHANCE
                            : BAD_DREAM_CHANCE;

                        float roll = UnityEngine.Random.Range(0f, 100f);
                        if (roll < chance)
                        {
                            rest.m_InterruptionAfterSecondsSleeping = 1;
                            new BadDreamAffliction(AfflictionBodyArea.Head).Start();
                            HUDMessage.AddMessage(Localization.Get("GAMEPLAY_BadDreamWakeup"), 4, false);
                        }
                    }
                }
                else
                {
                    badDreamRollTimer = 0f;
                }
                wasResting = isSleeping;
            }

            //overconfidence
            if (Settings.options.IsOverconfidence && (hoursSinceLastAffliction >= CONF_THRESHOLD_HOURS) && (OverconfidenceAffliction.IsOvercActive == false))
            {
                //MelonLogger.Msg("you are far too confident");
                new OverconfidenceRiskAffliction(AfflictionBodyArea.Head).Start();
                hoursSinceLastAffliction = 0f;
            }
        }
    }
}