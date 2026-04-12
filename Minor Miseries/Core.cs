using Minor_Miseries.Persistence;
using LocalizationUtilities;

[assembly: MelonInfo(typeof(Minor_Miseries.Core), "Minor Miseries", "1.5.0", "EtherSystem, Flower Field", null)]
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

        // -----------ModData persistence--------------------
        public static Core? Instance { get; private set; }
        internal static MMState State = new();
        private bool _dirty = false;

        // ----------------logging helper--------------------
        internal static void Log(string message, bool onlyWhenDebugEnabled = true)
        {
            if (onlyWhenDebugEnabled && !Settings.options.IsLogging) return;

            Instance?.LoggerInstance.Msg(message);
        }
        // --------------------------------------------------

        public override void OnInitializeMelon()
        {
            Instance = this;
            LocalizationManager.LoadJsonLocalization(LoadEmbeddedJSON("Localization.json"));
            Log("Initialized.", false);
            Settings.OnLoad();

            //dev console commands
            DevConsoleCommands.Register();

            uConsole.RegisterCommand("reset_hrsatt", new Action(() =>
            {
                Core.State.HoursSinceLastWildlifeAttack = 0f;
                _dirty = true;
                Log("Hrs since last attk reset to 0");
            }));
        }

        private float updateTimer = 0f;
        private bool wasResting = false;
        private float badDreamRollTimer = 0f;
        private float _stoppedHours = 0f;
        private bool hadAfflictionLastTick = false;
        private bool wasEating = false;
        private bool wasInStruggle = false;

        public void SaveIfDirty()
        {
            if (!_dirty) return;
            SaveDataManager.OnSave();
            _dirty = false;
        }

        public void ResetRuntime()
        {
            _dirty = false;

            updateTimer = 0f;
            badDreamRollTimer = 0f;
            _stoppedHours = 0f;

            hadAfflictionLastTick = false;
            wasEating = false;
            wasInStruggle = false;
            wasResting = false;

            BuffLogic.ResetRuntime();
        }

        public void OnStateLoaded()
        {
            bool changed = false;

            float oldScore = Core.State.AnimalStressScore;
            Core.State.AnimalStressScore = Mathf.Max(0f, Core.State.AnimalStressScore);
            if (!Mathf.Approximately(oldScore, Core.State.AnimalStressScore)) changed = true;

            if (float.IsPositiveInfinity(Core.State.AnimalStressTimer) || Core.State.AnimalStressTimer < 0f)
            {
                Core.State.AnimalStressTimer = -1f;
                changed = true;
            }

            float oldSince = Core.State.HoursSinceLastAffliction;
            float oldMove = Core.State.HoursSpentMoving;
            float oldOver = Core.State.HoursOverloaded;
            float oldWildlife = Core.State.HoursSinceLastWildlifeAttack;

            Core.State.HoursSinceLastAffliction = Mathf.Max(0f, Core.State.HoursSinceLastAffliction);
            Core.State.HoursSpentMoving = Mathf.Max(0f, Core.State.HoursSpentMoving);
            Core.State.HoursOverloaded = Mathf.Max(0f, Core.State.HoursOverloaded);
            Core.State.HoursSinceLastWildlifeAttack = Mathf.Max(0f, Core.State.HoursSinceLastWildlifeAttack);

            if (!Mathf.Approximately(oldSince, Core.State.HoursSinceLastAffliction)) changed = true;
            if (!Mathf.Approximately(oldMove, Core.State.HoursSpentMoving)) changed = true;
            if (!Mathf.Approximately(oldOver, Core.State.HoursOverloaded)) changed = true;
            if (!Mathf.Approximately(oldWildlife, Core.State.HoursSinceLastWildlifeAttack)) changed = true;

            if (changed) _dirty = true;
        }

        public void ResetAll()
        {
            SaveDataManager.OnNewGame();
            ResetRuntime();
        }

        public override void OnUpdate()
        {
            if (GameManager.m_Instance == null || GameManager.m_IsPaused) return;
            string scene = GameManager.m_ActiveScene;
            if (scene == "MainMenu" || scene == "Boot" || scene == "Empty") return;

            var cond = GameManager.GetConditionComponent();
            if (cond == null) return;

            // clock (1 tick / sec)
            updateTimer += Time.deltaTime;
            if (updateTimer < 1.0f) return;

            float realTimeElapsed = updateTimer;
            updateTimer = 0f;

            var tod = GameManager.GetTimeOfDayComponent();
            if (tod == null) return;

            float gameHoursPassed = tod.GetTODHours(realTimeElapsed);
            if (gameHoursPassed <= 0f) return;
            AfflictionLogic.Tick(this, cond, gameHoursPassed, ref badDreamRollTimer, ref _stoppedHours, ref hadAfflictionLastTick, ref wasEating, ref wasInStruggle, ref wasResting, ref _dirty);
            BuffLogic.Tick();

            Patches.FirearmsPatch.FlushPendingAfflictionSound();

            Resources.Localization.LocalizationRefresh.FlushPendingRefresh();

            Patches.AfflictionEffects.ForceRefresh();
        }
    }
}