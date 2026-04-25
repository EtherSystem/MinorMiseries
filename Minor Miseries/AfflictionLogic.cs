using static Minor_Miseries.Afflictions.OverconfidenceRisk;
using static Minor_Miseries.Afflictions.Overconfidence;
using static Minor_Miseries.Afflictions.NightTerror;
using static Minor_Miseries.Afflictions.StuckFood;
using static Minor_Miseries.Afflictions.SoreNeck;
using static Minor_Miseries.Afflictions.BackPain;
using static Minor_Miseries.Afflictions.BadDream;
using static Minor_Miseries.Afflictions.Blister;
using Minor_Miseries.Afflictions.Buffs;
using AfflictionComponent.Components;

namespace Minor_Miseries
{
    internal static class AfflictionLogic
    {
        private static bool SoreNeckWasSleeping = false;
        private static bool SoreNeckSleptInVehicle = false;
        private static bool SoreNeckSleptInShelter = false;

        private static readonly HashSet<string> IgnoredOverconfidenceAfflictionTypeNames = new(StringComparer.OrdinalIgnoreCase)
        {
            // MinorMiseries
            nameof(OverconfidenceRiskAffliction),
            nameof(OverconfidenceAffliction),
            nameof(ProtectedHandsBuff),
            nameof(ProtectedArmsBuff),
            nameof(PeaceOfMindBuff),

            // OxygenLevels
            "AcclimatizedBuff",

            // CatchColdMod
            "ColdResistance",

            // AfflictionsAndBuffs
            "BurningHeart",
            "Determination",
            "FogsEmbrace",
            "LittleHeart",
            "HowDidYouDoThat",

            //"RandomOtherNameForCustomBuff", <-- FOR FUTURE NEW BUFFS
        };

        private const float SORENECK_CAR_CHANCE = 60f;
        private const float SORENECK_SHELTER_CHANCE = 40f;
        private const float SORENECK_OVERC_BONUS = 20f;

        private const float CONF_THRESHOLD_HOURS = 96f;

        private const float BLIST_THRESHOLD_HOURS = 4f;
        private const float OVERC_BLIST_THRESHOLD_HOURS = 3f;

        private const float BACKPAIN_TRESHOLD_HOURS = 1f;
        private const float OVERC_BACKPAIN_TRESHOLD_HOURS = 0.5f;

        private const float STUCKFOOD_CHANCE = 5f;
        private const float OVERC_STUCKFOOD_CHANCE = 10f;

        private const float STRESS_WINDOW_HOURS = 24f;

        private const float STOP_TO_RESET_HOURS = 10f / 60f; // 10 min in-game

        private static bool IsIgnoredOverconfidenceAffliction(object affliction)
        {
            if (affliction == null)
                return false;

            Type type = affliction.GetType();

            if (IgnoredOverconfidenceAfflictionTypeNames.Contains(type.Name))
                return true;

            if (!string.IsNullOrEmpty(type.FullName) && IgnoredOverconfidenceAfflictionTypeNames.Contains(type.FullName))
                return true;

            return false;
        }

        public static bool HasAnyOtherCustomAfflictionForOverconfidence()
        {
            var mgr = AfflictionManager.GetAfflictionManagerInstance();
            if (mgr?.m_Afflictions == null)
                return false;

            foreach (var a in mgr.m_Afflictions)
            {
                if (a == null)
                    continue;

                if (IsIgnoredOverconfidenceAffliction(a))
                    continue;

                return true;
            }

            return false;
        }

        internal static void Tick(
            Core core,
            Condition cond,
            float gameHoursPassed,
            ref float badDreamRollTimer,
            ref float stoppedHours,
            ref bool hadAfflictionLastTick,
            ref bool wasEating,
            ref bool wasInStruggle,
            ref bool wasResting,
            ref bool dirty)
        {
            // Struggle stress + stress timer
            UpdateStress(core, gameHoursPassed, ref wasInStruggle, ref dirty);

            // Eating -> StuckFood
            UpdateStuckFood(ref wasEating);

            // Moving -> HoursSpentMoving + Blister
            UpdateMovingAndBlister(gameHoursPassed, ref stoppedHours, ref dirty);

            // Overload -> BackPain
            UpdateOverloadAndBackPain(gameHoursPassed, ref dirty);

            // Overconfidence tracking
            UpdateOverconfidenceTracking(cond, gameHoursPassed, ref hadAfflictionLastTick, ref dirty);

            // BadDream/NightTerror
            UpdateBadDreamOrNightTerror(core, gameHoursPassed, ref badDreamRollTimer, ref wasResting, ref dirty);

            // Sore Neck
            UpdateSoreNeck(core);

            // Overconfidence risk spawn
            TrySpawnOverconfidenceRisk(ref dirty);
        }

        public static void UpdateStress(Core core, float gameHoursPassed, ref bool wasInStruggle, ref bool dirty)
        {
            if (!Settings.options.IsBadDream)
            {
                bool changed = false;

                if (Core.State.AnimalStressScore > 0f)
                {
                    Core.State.AnimalStressScore = 0f;
                    changed = true;
                }

                if (Core.State.AnimalStressTimer != -1f)
                {
                    Core.State.AnimalStressTimer = -1f;
                    changed = true;
                }

                if (Core.State.HoursSinceLastWildlifeAttack != 0f)
                {
                    Core.State.HoursSinceLastWildlifeAttack = 0f;
                    changed = true;
                }

                if (wasInStruggle)
                {
                    wasInStruggle = false;
                }

                if (changed)
                {
                    BuffLogic.CurePeaceOfMind();
                    dirty = true;
                }
                return;
            }

            var struggle = GameManager.GetPlayerStruggleComponent();
            bool inStruggle = struggle != null && struggle.InStruggle();

            if (inStruggle && !wasInStruggle && struggle != null)
            {
                float addedScore = 0f;

                if (struggle.InStruggleWIthWolf()) addedScore = 1f;
                else if (struggle.InStruggleWithMoose()) addedScore = 2f;
                else if (struggle.InStruggleWithCougar()) addedScore = 3f;
                else if (struggle.InStruggleWIthBear()) addedScore = 4f;

                if (OverconfidenceAffliction.IsActive)
                {
                    addedScore += 1f;
                    Core.Log("Overconfidence amplified stress (+1)");
                }

                Core.State.HoursSinceLastWildlifeAttack = 0f;
                dirty = true;

                if (BuffLogic.HasPeaceOfMind())
                {
                    Core.Log($"Peace Of Mind absorbed wildlife stress (+{addedScore:0.###})");

                    BuffLogic.CurePeaceOfMind();
                }
                else
                {
                    Core.State.AnimalStressScore += addedScore;
                    Core.State.AnimalStressTimer = 0f;

                    Core.Log($"Stress Added → +{addedScore} | total:{Core.State.AnimalStressScore:0.###}");
                }
            }
            else if (!inStruggle)
            {
                float oldSinceAttack = Core.State.HoursSinceLastWildlifeAttack;
                Core.State.HoursSinceLastWildlifeAttack += gameHoursPassed;

                if (!Mathf.Approximately(oldSinceAttack, Core.State.HoursSinceLastWildlifeAttack))
                    dirty = true;
            }

            if (Core.State.AnimalStressTimer >= 0f)
            {
                Core.State.AnimalStressTimer += gameHoursPassed;
                dirty = true;

                if (Core.State.AnimalStressTimer >= STRESS_WINDOW_HOURS)
                {
                    Core.Log("Stress expired");

                    Core.State.AnimalStressScore = 0f;
                    Core.State.AnimalStressTimer = -1f;
                    dirty = true;
                }
            }

            wasInStruggle = inStruggle;
        }

        public static void UpdateStuckFood(ref bool wasEating)
        {
            var hunger = GameManager.GetHungerComponent();
            if (hunger == null) return;

            bool isEating = hunger.IsEatingInProgress();

            if (Settings.options.IsStuckFood && wasEating && !isEating)
            {
                float chance = OverconfidenceAffliction.IsActive ? OVERC_STUCKFOOD_CHANCE : STUCKFOOD_CHANCE;
                float roll = UnityEngine.Random.Range(0f, 100f);

                if (roll < chance)
                {
                    new StuckFoodAffliction(AfflictionBodyArea.Head).Start();
                    GameAudioManager.PlaySound(Il2CppAK.EVENTS.PLAY_VOBREATHMEDIUMINTENSITYNOSPRINT, GameManager.GetPlayerObject());
                    AfflictionSaveHelper.QueueSurvivalSave();
                }
            }
            wasEating = isEating;
        }

        public static void UpdateMovingAndBlister(float gameHoursPassed, ref float stoppedHours, ref bool dirty)
        {
            var pm = GameManager.GetPlayerManagerComponent();
            if (pm == null) return;

            bool isMoving = pm.PlayerIsWalking() || pm.PlayerIsSprinting();

            if (isMoving)
            {
                stoppedHours = 0f;
                Core.State.HoursSpentMoving += gameHoursPassed;
                dirty = true;
            }
            else
            {
                stoppedHours += gameHoursPassed;
                if (stoppedHours >= STOP_TO_RESET_HOURS && Core.State.HoursSpentMoving > 0f)
                {
                    Core.State.HoursSpentMoving = 0f;
                    dirty = true;
                }
            }

            if (!Settings.options.IsBlister) return;

            bool overc = OverconfidenceAffliction.IsActive;
            float threshold = overc ? OVERC_BLIST_THRESHOLD_HOURS : BLIST_THRESHOLD_HOURS;

            if (Core.State.HoursSpentMoving >= threshold)
            {
                new BlisterAffliction(AfflictionBodyArea.FootRight).Start();
                GameAudioManager.PlaySound(Il2CppAK.EVENTS.PLAY_EXERTIONLOW, GameManager.GetPlayerObject());
                AfflictionSaveHelper.QueueSurvivalSave();

                Core.State.HoursSpentMoving = 0f;
                stoppedHours = 0f;
                dirty = true;
            }
        }

        public static void UpdateOverloadAndBackPain(float gameHoursPassed, ref bool dirty)
        {
            bool overloaded = IsPlayerOverloaded();

            float oldOver = Core.State.HoursOverloaded;
            Core.State.HoursOverloaded = overloaded ? (Core.State.HoursOverloaded + gameHoursPassed) : 0f;
            if (!Mathf.Approximately(oldOver, Core.State.HoursOverloaded)) dirty = true;

            if (!Settings.options.IsBackPain) return;

            bool overc = OverconfidenceAffliction.IsActive;
            float threshold = overc ? OVERC_BACKPAIN_TRESHOLD_HOURS : BACKPAIN_TRESHOLD_HOURS;

            if (Core.State.HoursOverloaded >= threshold)
            {
                new BackPainAffliction(AfflictionBodyArea.Chest).Start();
                GameAudioManager.PlaySound(Il2CppAK.EVENTS.PLAY_GENERALINJURYLOW, GameManager.GetPlayerObject());
                AfflictionSaveHelper.QueueSurvivalSave();
                Core.State.HoursOverloaded = 0f;
                dirty = true;
            }
        }

        public static void UpdateOverconfidenceTracking(Condition cond, float gameHoursPassed, ref bool hadAfflictionLastTick, ref bool dirty)
        {
            bool hasAfflictionNow = cond.HasAffliction() || HasAnyOtherCustomAfflictionForOverconfidence();

            float oldSince = Core.State.HoursSinceLastAffliction;

            if (!hadAfflictionLastTick && hasAfflictionNow) Core.State.HoursSinceLastAffliction = 0f;

            if (!hasAfflictionNow) Core.State.HoursSinceLastAffliction += gameHoursPassed;

            hadAfflictionLastTick = hasAfflictionNow;

            if (!Mathf.Approximately(oldSince, Core.State.HoursSinceLastAffliction)) dirty = true;
        }

        public static void UpdateBadDreamOrNightTerror(Core core, float gameHoursPassed, ref float badDreamRollTimer, ref bool wasResting, ref bool dirty)
        {
            if (!Settings.options.IsBadDream) return;

            var rest = GameManager.GetRestComponent();
            if (rest == null) return;

            bool isSleeping = rest.IsSleeping();

            if (isSleeping)
            {
                if (Core.State.AnimalStressScore > 0f)
                {
                    badDreamRollTimer += gameHoursPassed;

                    while (badDreamRollTimer >= 1f)
                    {
                        badDreamRollTimer -= 1f;

                        float badDreamChance = 0f;
                        float nightTerrorChance = 0f;

                        int score = Mathf.FloorToInt(Core.State.AnimalStressScore);

                        switch (score)
                        {
                            case 1: badDreamChance = 4f; nightTerrorChance = 0f; break;
                            case 2: badDreamChance = 5f; nightTerrorChance = 1f; break;
                            case 3: badDreamChance = 6f; nightTerrorChance = 4f; break;
                            case 4: badDreamChance = 5f; nightTerrorChance = 5f; break;
                            case 5: badDreamChance = 4f; nightTerrorChance = 6f; break;
                            case 6: badDreamChance = 2f; nightTerrorChance = 8f; break;
                            default:
                                if (score >= 7)
                                {
                                    badDreamChance = 0f;
                                    nightTerrorChance = 10f;
                                }
                                break;
                        }

                        if (BuffLogic.HasPeaceOfMind())
                        {
                            badDreamChance = 0f;
                            nightTerrorChance = 0f;
                        }

                        float rollNT = UnityEngine.Random.Range(0f, 100f);
                        if (nightTerrorChance > 0f && rollNT < nightTerrorChance)
                        {
                            rest.m_InterruptionAfterSecondsSleeping = 1;
                            new NightTerrorAffliction(AfflictionBodyArea.Head).Start();
                            GameAudioManager.PlaySound(Il2CppAK.EVENTS.PLAY_VOBREATHHIGHINTENSITYNOLOOP, GameManager.GetPlayerObject());
                            AfflictionSaveHelper.QueueSurvivalSave();
                            HUDMessage.AddMessage(Localization.Get("GAMEPLAY_NightTerrorWakeup"), 4, false);

                            Core.State.AnimalStressScore = 0f;
                            Core.State.AnimalStressTimer = -1f;
                            badDreamRollTimer = 0f;
                            dirty = true;

                            Core.Log("Stress cleared due to sleep event");
                            break;
                        }

                        float rollBD = UnityEngine.Random.Range(0f, 100f);
                        if (badDreamChance > 0f && rollBD < badDreamChance)
                        {
                            rest.m_InterruptionAfterSecondsSleeping = 1;
                            new BadDreamAffliction(AfflictionBodyArea.Head).Start();
                            GameAudioManager.PlaySound(Il2CppAK.EVENTS.PLAY_VOBREATHMEDIUMINTENSITYNOLOOP, GameManager.GetPlayerObject());
                            AfflictionSaveHelper.QueueSurvivalSave();
                            HUDMessage.AddMessage(Localization.Get("GAMEPLAY_BadDreamWakeup"), 4, false);

                            Core.State.AnimalStressScore = 0f;
                            Core.State.AnimalStressTimer = -1f;
                            badDreamRollTimer = 0f;
                            dirty = true;

                            Core.Log("Stress cleared due to sleep event");
                            break;
                        }
                    }
                }
                else
                {
                    badDreamRollTimer = 0f;
                }
            }
            else
            {
                badDreamRollTimer = 0f;
            }

            wasResting = isSleeping;
        }

        public static void TrySpawnOverconfidenceRisk(ref bool dirty)
        {
            if (Settings.options.IsOverconfidence && Core.State.HoursSinceLastAffliction >= CONF_THRESHOLD_HOURS && OverconfidenceAffliction.IsActive == false)
            {
                new OverconfidenceRiskAffliction(AfflictionBodyArea.Head).Start();
                AfflictionSaveHelper.QueueSurvivalSave();
                Core.State.HoursSinceLastAffliction = 0f;
                dirty = true;
            }
        }

        public static bool IsPlayerOverloaded()
        {
            var enc = GameManager.GetEncumberComponent();
            if (enc == null) return false;

            return enc.GetGearWeightKG() > enc.GetEffectiveCarryCapacityKG();
        }

        public static void UpdateSoreNeck(Core core)
        {
            if (!Settings.options.IsSoreNeck) return;

            var rest = GameManager.GetRestComponent();
            if (rest == null) return;

            bool sleeping = rest.IsSleeping();

            if (sleeping && !SoreNeckWasSleeping)
            {
                SoreNeckSleptInVehicle = false;
                SoreNeckSleptInShelter = false;
            }

            if (sleeping)
            {
                var piv = GameManager.GetPlayerInVehicle();
                bool inVehicle = piv != null && piv.IsInside();

                bool inShelter = false;
                try { inShelter = GameManager.GetSnowShelterManager().PlayerInShelter(); } catch { }

                if (inVehicle) SoreNeckSleptInVehicle = true;
                else if (inShelter) SoreNeckSleptInShelter = true;
            }

            if (!sleeping && SoreNeckWasSleeping)
            {
                float chance = -1f;
                string ctx = "";

                if (SoreNeckSleptInVehicle)
                {
                    chance = SORENECK_CAR_CHANCE;
                    ctx = "vehicle";
                }
                else if (SoreNeckSleptInShelter)
                {
                    chance = SORENECK_SHELTER_CHANCE;
                    ctx = "snowshelter";
                }

                if (chance > 0f)
                {
                    if (OverconfidenceAffliction.IsActive)
                    {
                        chance += SORENECK_OVERC_BONUS;
                        if (chance > 100f) chance = 100f;

                        Core.Log($"Overconfidence amplified Sore Neck chance (+{SORENECK_OVERC_BONUS:0.##}) -> {chance:0.##}%");
                    }

                    float roll = UnityEngine.Random.Range(0f, 100f);
                    if (roll < chance)
                    {
                        new SoreNeckAffliction(AfflictionBodyArea.Neck).Start();
                        GameAudioManager.PlaySound(Il2CppAK.EVENTS.PLAY_EXERTIONLOW, GameManager.GetPlayerObject());
                        AfflictionSaveHelper.QueueSurvivalSave();
                        Core.Log($"Sore Neck triggered ({ctx}) roll={roll:0.##} < {chance:0.##}");
                    }
                    else
                    {
                        Core.Log($"Sore Neck avoided ({ctx}) roll={roll:0.##} >= {chance:0.##}");
                    }
                }
                SoreNeckSleptInVehicle = false;
                SoreNeckSleptInShelter = false;
            }
            SoreNeckWasSleeping = sleeping;
        }
    }
}