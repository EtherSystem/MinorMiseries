using Newtonsoft.Json;
using ModData;

namespace Minor_Miseries.Persistence
{
    internal static class SaveDataManager
    {
        private static readonly ModDataManager _manager = new("MinorMiseries", false);
        private const string SUFFIX = "mmdata";

        private static void LogState(string prefix)
        {
            Core.Log(
                $"{prefix} → " +
                $"Stress:{Core.State.AnimalStressScore:0.###} | " +
                $"Timer:{Core.State.AnimalStressTimer:0.###} | " +
                $"HrsSinceAtt:{Core.State.HoursSinceLastWildlifeAttack:0.###} | " +
                $"HrsSinceAff:{Core.State.HoursSinceLastAffliction:0.###} | " +
                $"HrsMove:{Core.State.HoursSpentMoving:0.###} | " +
                $"HrsOverL:{Core.State.HoursOverloaded:0.###}"
            );
        }

        internal static void OnSave()
        {
            string json = JsonConvert.SerializeObject(Core.State);
            _manager.Save(json, SUFFIX);

            LogState("Saved");
        }

        internal static void OnLoad()
        {
            string json = _manager.Load(SUFFIX);

            if (string.IsNullOrEmpty(json))
            {
                Core.State = new MMState();

                Core.Log("Loaded → empty data (fresh slot)");

                LogState("Loaded");
                return;
            }

            MMState? loaded = null;
            try
            {
                loaded = JsonConvert.DeserializeObject<MMState>(json);
            }
            catch (Exception ex)
            {
                Core.Warn($"Load failed, resetting state: {ex.Message}");
            }

            Core.State = loaded ?? new MMState();
            ClampAndFix();

            LogState("Loaded");
        }

        internal static void OnNewGame()
        {
            Core.State = new MMState();

            Core.Log("Clearing data for new game");

            LogState("NewGame");
        }

        private static void ClampAndFix()
        {
            Core.State.AnimalStressScore = Mathf.Max(0f, Core.State.AnimalStressScore);

            // timer: -1 = inactive
            if (float.IsNaN(Core.State.AnimalStressTimer) || float.IsInfinity(Core.State.AnimalStressTimer) || Core.State.AnimalStressTimer < 0f)
            {
                Core.State.AnimalStressTimer = -1f;
            }

            Core.State.HoursSinceLastWildlifeAttack = Mathf.Max(0f, Core.State.HoursSinceLastWildlifeAttack);
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