using Newtonsoft.Json;
using ModData;

namespace Minor_Miseries.Persistence
{
    internal static class SaveDataManager
    {
        private static readonly ModDataManager _manager = new("MinorMiseries", false);
        private const string SUFFIX = "mmdata";

        internal static void OnSave()
        {
            string json = JsonConvert.SerializeObject(Core.State);
            _manager.Save(json, SUFFIX);

            if (Settings.options.IsLogging && Core.Instance != null)
            {
                Core.Instance.LoggerInstance.Msg(
                    $"Saved → Stress:{Core.State.AnimalStressScore:0.###} | " +
                    $"Timer:{Core.State.AnimalStressTimer:0.###} | " +
                    $"HrsSinceAff:{Core.State.HoursSinceLastAffliction:0.###} | " +
                    $"Move:{Core.State.HoursSpentMoving:0.###} | Over:{Core.State.HoursOverloaded:0.###}"
                );
            }
        }

        internal static void OnLoad()
        {
            string json = _manager.Load(SUFFIX);

            if (string.IsNullOrEmpty(json))
            {
                Core.State = new MMState();
                if (Settings.options.IsLogging && Core.Instance != null)
                    Core.Instance.LoggerInstance.Msg("Loaded → empty data (fresh slot)");
                return;
            }

            MMState? loaded = null;
            try { loaded = JsonConvert.DeserializeObject<MMState>(json); }
            catch { /* corrupted data -> reset safe */ }

            Core.State = loaded ?? new MMState();
            ClampAndFix();

            if (Settings.options.IsLogging && Core.Instance != null)
            {
                Core.Instance.LoggerInstance.Msg($"Loaded → Stress:{Core.State.AnimalStressScore:0.###} | " + $"Timer:{Core.State.AnimalStressTimer:0.###}");
            }
        }

        internal static void OnNewGame()
        {
            Core.State = new MMState();

            if (Settings.options.IsLogging && Core.Instance != null)
                Core.Instance.LoggerInstance.Msg("Clearing data for new game");
        }

        private static void ClampAndFix()
        {
            Core.State.AnimalStressScore = Mathf.Max(0f, Core.State.AnimalStressScore);

            // timer: -1 = inactive
            if (Core.State.AnimalStressTimer < 0f)
                Core.State.AnimalStressTimer = -1f;

            // optionnels
            Core.State.HoursSinceLastAffliction = Mathf.Max(0f, Core.State.HoursSinceLastAffliction);
            Core.State.HoursSpentMoving = Mathf.Max(0f, Core.State.HoursSpentMoving);
            Core.State.HoursOverloaded = Mathf.Max(0f, Core.State.HoursOverloaded);
        }
    }

    // SAVE
    [HarmonyPatch(typeof(SaveGameSlots), nameof(SaveGameSlots.WriteSlotToDisk), new Type[] { typeof(SlotData), typeof(SaveGameSlots.Timestamp) })]
    internal class MinorMiseries_SavePatch
    {
        private static void Prefix()
        {
            Core.Instance?.SaveIfDirty();
        }
    }

    // LOAD
    [HarmonyPatch(typeof(GameManager), nameof(GameManager.LoadSaveGameSlot), new Type[] { typeof(string), typeof(int) })]
    internal class MinorMiseries_LoadPatch
    {
        private static void Postfix()
        {
            Core.Instance?.ResetRuntime();
            SaveDataManager.OnLoad();
            Core.Instance?.OnStateLoaded();
        }
    }

    // NEW GAME
    [HarmonyPatch(typeof(SaveGameSlots), nameof(SaveGameSlots.CreateSlot), new Type[] { typeof(string), typeof(SaveSlotType), typeof(uint), typeof(Episode) })]
    internal class MinorMiseries_NewGamePatch
    {
        private static void Postfix()
        {
            Core.Instance?.ResetAll();
        }
    }

    // MAIN MENU
    [HarmonyPatch(typeof(GameManager), nameof(GameManager.DoExitToMainMenu))]
    [HarmonyPatch(typeof(GameManager), nameof(GameManager.LoadMainMenu))]
    internal class MinorMiseries_MainMenuPatch
    {
        private static void Postfix()
        {
            Core.Instance?.ResetAll();
        }
    }
}