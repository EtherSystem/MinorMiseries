using AfflictionComponent.Components;
using static Minor_Miseries.Afflictions.BackPain;
using static Minor_Miseries.Afflictions.Blister;
using static Minor_Miseries.Afflictions.OverconfidenceRisk;
using static Minor_Miseries.Afflictions.StuckFood;
using static Minor_Miseries.Afflictions.Overconfidence;

[assembly: MelonInfo(typeof(Minor_Miseries.Core), "Minor Miseries", "1.0.0", "EtherSystem", null)]
[assembly: MelonGame("Hinterland", "TheLongDark")]

namespace Minor_Miseries
{
    public class Core : MelonMod
    {
        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("Initialized.");
            Settings.OnLoad();
        }

        private static readonly bool IsOvercActive = OverconfidenceAffliction.IsOvercActive;
        private readonly System.Random _random = new();
        private float updateTimer = 0f;
        private float hoursSinceLastAffliction = 0f;
        private float hoursSpentMoving = 0f;
        private float hoursOverloaded = 0f;
        private const float CONF_THRESHOLD_HOURS = 96f;
        private const float BLIST_THRESHOLD_HOURS = 5f;
        private const float OVERC_BLIST_THRESHOLD_HOURS = 4f;
        private const float BACKPAIN_TRESHOLD_HOURS = 1f;
        private const float OVERC_BACKPAIN_TRESHOLD_HOURS = 0.5f;
        private const float STUCKFOOD_CHANCE = 10f;
        private const float OVERC_STUCKFOOD_CHANCE = 20f;
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
            var pm = GameManager.GetPlayerManagerComponent();

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
                if (IsOvercActive)
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
                if (IsOvercActive && hoursSpentMoving >= OVERC_BLIST_THRESHOLD_HOURS)
                {
                    var side = _random.Next(0, 2) == 0
                        ? AfflictionBodyArea.FootLeft : AfflictionBodyArea.FootRight;
                    //MelonLogger.Msg("an overc blister appeared");
                    new BlisterAffliction(side).Start();
                }
                else if (!IsOvercActive && hoursSpentMoving >= BLIST_THRESHOLD_HOURS)
                {
                    var side = _random.Next(0, 2) == 0
                        ? AfflictionBodyArea.FootLeft : AfflictionBodyArea.FootRight;
                    //MelonLogger.Msg("a blister appeared");
                    new BlisterAffliction(side).Start();
                }
            }

            //back pain
            if (Settings.options.IsBackPain)
            {
                if (IsOvercActive && (hoursOverloaded >= OVERC_BACKPAIN_TRESHOLD_HOURS))
                {
                    //MelonLogger.Msg("you have an overc back pain");
                    new BackPainAffliction(AfflictionBodyArea.Chest).Start();
                    hoursOverloaded = 0f;
                }
                else if (!IsOvercActive && (hoursOverloaded >= BACKPAIN_TRESHOLD_HOURS))
                {
                    //MelonLogger.Msg("you have back pain");
                    new BackPainAffliction(AfflictionBodyArea.Chest).Start();
                    hoursOverloaded = 0f;
                }
            }

            //overconfidence
            if (Settings.options.IsOverconfidence && (hoursSinceLastAffliction >= CONF_THRESHOLD_HOURS) && (IsOvercActive == false))
            {
                //MelonLogger.Msg("you are far too confident");
                new OverconfidenceRiskAffliction(AfflictionBodyArea.Head).Start();
                hoursSinceLastAffliction = 0f;
            }
        }
    }
}