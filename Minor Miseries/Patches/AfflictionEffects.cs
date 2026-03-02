using AfflictionComponent.Components;
using Minor_Miseries.Afflictions;
using Il2CppTLD.IntBackedUnit;

namespace Minor_Miseries.Patches
{
    internal static class AfflictionEffects
    {
        private const float REFRESH_INTERVAL_SECONDS = 0.25f;
        private static float _lastRefreshUnscaledTime = -999f;

        private struct Cache
        {
            public bool Overconfidence;

            public bool BackPain;
            public bool Blister;
            public bool BareSkin;
            public bool Splinter;
            public bool SensitiveHand;
            public bool Scratch;
            public bool WristTrauma;
            public bool ShoulderTrauma;
            public bool BadDream;
            public bool NightTerror;
            public bool BlisterSymptomsCured;
            public bool BareSkinSymptomsCured;
            public bool ScratchSymptomsCured;

            public void Reset()
            {
                this = default;
            }
        }

        private static Cache _cache;

        private static readonly FieldInfo? _blisterSymptomsCuredField = typeof(Blister.BlisterAffliction).GetField("m_SymptomsCured", BindingFlags.Instance | BindingFlags.NonPublic);

        private static readonly FieldInfo? _scratchSymptomsCuredField = typeof(Scratch.ScratchAffliction).GetField("m_SymptomsCured", BindingFlags.Instance | BindingFlags.NonPublic);

        private static readonly FieldInfo? _bareSkinSymptomsCuredField = typeof(BareSkin.BareSkinAffliction).GetField("m_SymptomsCured", BindingFlags.Static | BindingFlags.NonPublic);

        internal static void ForceRefresh()
        {
            _lastRefreshUnscaledTime = -999f;
            RefreshIfNeeded(force: true);
        }

        private static void RefreshIfNeeded(bool force = false)
        {
            float now = Time.unscaledTime;
            if (!force && (now - _lastRefreshUnscaledTime) < REFRESH_INTERVAL_SECONDS) return;
            _lastRefreshUnscaledTime = now;

            _cache.Reset();

            var mgr = AfflictionManager.GetAfflictionManagerInstance();
            var list = mgr?.m_Afflictions;
            if (list == null) return;

            for (int i = 0; i < list.Count; i++)
            {
                object? a;
                try { a = list[i]; }
                catch { break; }
                if (a == null) continue;

                switch (a)
                {
                    case Overconfidence.OverconfidenceAffliction:
                        _cache.Overconfidence = true;
                        break;

                    case BackPain.BackPainAffliction:
                        _cache.BackPain = true;
                        break;

                    case Blister.BlisterAffliction blister:
                        _cache.Blister = true;
                        _cache.BlisterSymptomsCured = ReadBoolField(blister, _blisterSymptomsCuredField);
                        break;

                    case BareSkin.BareSkinAffliction:
                        _cache.BareSkin = true;
                        _cache.BareSkinSymptomsCured = ReadStaticBoolField(_bareSkinSymptomsCuredField);
                        break;

                    case Splinter.SplinterAffliction:
                        _cache.Splinter = true;
                        break;

                    case SensitiveHand.SensitiveHandAffliction:
                        _cache.SensitiveHand = true;
                        break;

                    case Scratch.ScratchAffliction scratch:
                        _cache.Scratch = true;
                        _cache.ScratchSymptomsCured = ReadBoolField(scratch, _scratchSymptomsCuredField);
                        break;

                    case WristTrauma.WristTraumaAffliction:
                        _cache.WristTrauma = true;
                        break;

                    case ShoulderTrauma.ShoulderTraumaAffliction:
                        _cache.ShoulderTrauma = true;
                        break;

                    case BadDream.BadDreamAffliction:
                        _cache.BadDream = true;
                        break;

                    case NightTerror.NightTerrorAffliction:
                        _cache.NightTerror = true;
                        break;
                }
            }
        }

        private static bool ReadBoolField(object instance, FieldInfo? field)
        {
            if (field == null) return false;
            try
            {
                return field.GetValue(instance) is bool b && b;
            }
            catch
            {
                return false;
            }
        }

        private static bool ReadStaticBoolField(FieldInfo? field)
        {
            if (field == null) return false;
            try
            {
                return field.GetValue(null) is bool b && b;
            }
            catch
            {
                return false;
            }
        }

        private static float GetMovementMultiplier()
        {
            var pm = GameManager.GetPlayerManagerComponent();
            if (pm == null) return 1f;

            bool climbing = pm.PlayerIsClimbing();
            bool movingOrClimbing = climbing || pm.PlayerIsSprinting() || pm.PlayerIsWalking() || pm.PlayerIsCrouched();

            float mult = 1f;

            // bare skin / blister affect "general movement"
            if (_cache.BareSkin && !_cache.BareSkinSymptomsCured && movingOrClimbing)
            {
                mult *= 0.7f;
            }

            if (_cache.Blister && !_cache.BlisterSymptomsCured && movingOrClimbing)
            {
                mult *= (_cache.Overconfidence ? 0.8f : 0.9f);
            }

            if (climbing)
            {
                if (_cache.Splinter)
                {
                    mult *= (_cache.Overconfidence ? 0.8f : 0.9f);
                }

                if (_cache.SensitiveHand)
                {
                    mult *= 0.7f;
                }

                if (_cache.WristTrauma)
                {
                    mult *= (_cache.Overconfidence ? 0.8f : 0.9f);
                }

                if (_cache.ShoulderTrauma)
                {
                    mult *= (_cache.Overconfidence ? 0.8f : 0.9f);
                }
            }
            return mult;
        }

        private static float GetCraftingMultiplier()
        {
            float mult = 1f;

            if (_cache.Splinter)
            {
                mult *= (_cache.Overconfidence ? 1.2f : 1.1f);
            }

            if (_cache.Scratch && !_cache.ScratchSymptomsCured)
            {
                mult *= (_cache.Overconfidence ? 1.2f : 1.1f);
            }

            if (_cache.SensitiveHand)
            {
                mult *= 1.3f;
            }

            if (_cache.WristTrauma)
            {
                mult *= (_cache.Overconfidence ? 1.1f : 1.05f);
            }

            if (_cache.ShoulderTrauma)
            {
                mult *= (_cache.Overconfidence ? 1.2f : 1.1f);
            }
            return mult;
        }

        // ----------------------------------------------------------------------------------------------------------------
        //                                                HARMONY PATCHES
        // ----------------------------------------------------------------------------------------------------------------

        [HarmonyPatch(typeof(vp_FPSController), nameof(vp_FPSController.GetSlopeMultiplier))]
        internal static class MovementSpeedPatch
        {
            private static void Postfix(ref float __result)
            {
                RefreshIfNeeded();

                float mult = GetMovementMultiplier();
                if (mult != 1f)
                {
                    __result *= mult;
                }
            }
        }

        [HarmonyPatch(typeof(Panel_Crafting), nameof(Panel_Crafting.GetModifiedCraftingDuration))]
        internal static class CraftingDurationPatch
        {
            private static void Postfix(ref int __result)
            {
                RefreshIfNeeded();

                float mult = GetCraftingMultiplier();
                if (mult != 1f)
                {
                    __result = (int)(__result * mult);
                }
            }
        }

        [HarmonyPatch(typeof(Panel_Rest), nameof(Panel_Rest.StartRest))]
        internal static class BlockRestPatch
        {
            private static bool Prefix()
            {
                RefreshIfNeeded();

                if (_cache.NightTerror)
                {
                    HUDMessage.AddMessage(Localization.Get("GAMEPLAY_CantSleep"), 4, false);
                    return false;
                }

                if (_cache.BadDream)
                {
                    HUDMessage.AddMessage(Localization.Get("GAMEPLAY_CantSleep"), 4, false);
                    return false;
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(GunItem), nameof(GunItem.Update))]
        internal static class GunAimStaminaPatch
        {
            private static readonly float BASE_INCREASE = 0.1f;
            private static readonly float BASE_DECREASE = 0.15f;

            private static void Postfix(GunItem __instance)
            {
                if (__instance == null) return;

                bool wrist = WristTrauma.WristTraumaAffliction.IsWristTraumaActive;
                bool shoulder = ShoulderTrauma.ShoulderTraumaAffliction.IsShoulderTraumaActive;

                float increase = BASE_INCREASE;
                float decrease = BASE_DECREASE;

                if (wrist)
                {
                    increase *= 1.6f;
                    decrease *= 0.6f;
                }

                if (shoulder)
                {
                    increase *= 1.8f;
                    decrease *= 0.5f;
                }

                if (__instance.m_SwayIncreasePerSecond != increase) __instance.m_SwayIncreasePerSecond = increase;
                if (__instance.m_SwayDecreasePerSecond != decrease) __instance.m_SwayDecreasePerSecond = decrease;
            }
        }

        [HarmonyPatch(typeof(Encumber), nameof(Encumber.Update))]
        internal static class BackPainCarryCapacityPatch
        {
            private struct EncumberSnapshot
            {
                public ItemWeight MaxCarryCapacity;
                public ItemWeight MaxCarryCapacityWhenExhausted;
                public ItemWeight NoSprintCarryCapacity;
                public ItemWeight NoWalkCarryCapacity;
                public ItemWeight EncumberLowThreshold;
                public ItemWeight EncumberMedThreshold;
                public ItemWeight EncumberHighThreshold;
            }

            private static bool _hasSnapshot;
            private static Encumber? _snapshotOwner;
            private static EncumberSnapshot _snapshot;

            private static void Prefix(Encumber __instance)
            {
                if (!_hasSnapshot) return;
                if (__instance == null) { _hasSnapshot = false; _snapshotOwner = null; return; }
                if (!ReferenceEquals(__instance, _snapshotOwner)) { _hasSnapshot = false; _snapshotOwner = null; return; }

                __instance.m_MaxCarryCapacity = _snapshot.MaxCarryCapacity;
                __instance.m_MaxCarryCapacityWhenExhausted = _snapshot.MaxCarryCapacityWhenExhausted;
                __instance.m_NoSprintCarryCapacity = _snapshot.NoSprintCarryCapacity;
                __instance.m_NoWalkCarryCapacity = _snapshot.NoWalkCarryCapacity;
                __instance.m_EncumberLowThreshold = _snapshot.EncumberLowThreshold;
                __instance.m_EncumberMedThreshold = _snapshot.EncumberMedThreshold;
                __instance.m_EncumberHighThreshold = _snapshot.EncumberHighThreshold;

                _hasSnapshot = false;
                _snapshotOwner = null;
            }

            private static void Postfix(Encumber __instance)
            {
                if (__instance == null) return;

                RefreshIfNeeded();
                if (!_cache.BackPain) return;

                _snapshotOwner = __instance;
                _snapshot = new EncumberSnapshot
                {
                    MaxCarryCapacity = __instance.m_MaxCarryCapacity,
                    MaxCarryCapacityWhenExhausted = __instance.m_MaxCarryCapacityWhenExhausted,
                    NoSprintCarryCapacity = __instance.m_NoSprintCarryCapacity,
                    NoWalkCarryCapacity = __instance.m_NoWalkCarryCapacity,
                    EncumberLowThreshold = __instance.m_EncumberLowThreshold,
                    EncumberMedThreshold = __instance.m_EncumberMedThreshold,
                    EncumberHighThreshold = __instance.m_EncumberHighThreshold,
                };
                _hasSnapshot = true;

                float deltaKg = _cache.Overconfidence ? 7.5f : 5f;
                ItemWeight delta = ItemWeight.FromKilograms(deltaKg);

                __instance.m_MaxCarryCapacity = _snapshot.MaxCarryCapacity - delta;
                __instance.m_MaxCarryCapacityWhenExhausted = _snapshot.MaxCarryCapacityWhenExhausted - delta;
                __instance.m_NoSprintCarryCapacity = _snapshot.NoSprintCarryCapacity - delta;
                __instance.m_NoWalkCarryCapacity = _snapshot.NoWalkCarryCapacity - delta;
                __instance.m_EncumberLowThreshold = _snapshot.EncumberLowThreshold - delta;
                __instance.m_EncumberMedThreshold = _snapshot.EncumberMedThreshold - delta;
                __instance.m_EncumberHighThreshold = _snapshot.EncumberHighThreshold - delta;
            }
        }
    }
}