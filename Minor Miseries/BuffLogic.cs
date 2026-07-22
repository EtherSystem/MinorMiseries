using AfflictionComponent.Components;
using Minor_Miseries.Afflictions.Buffs;
using static Minor_Miseries.Afflictions.BadDream;
using static Minor_Miseries.Afflictions.NightTerror;

namespace Minor_Miseries
{
    internal static class BuffLogic
    {
        private static bool _wasSleeping = false;

        private static readonly ClothingLayer[] ChestLayers =
        {
            ClothingLayer.Top2,
            ClothingLayer.Top,
            ClothingLayer.Mid,
            ClothingLayer.Base
        };

        private static readonly string[] ProtectedHandsGearIds =
        {
            "GEAR_WorkGloves",
            "GEAR_Gauntlets",
            "GEAR_TacticalGloves",
            "GEAR_DeerskinGloves_MOD"
        };

        private static readonly string[] ProtectedArmsGearIds =
        {
            "GEAR_BearSkinCoat",
            "GEAR_MooseHideCloak",
            "GEAR_JacketLeatherFlightA",
            "GEAR_MilitaryParka",
            "GEAR_MinersJacket",
            "GEAR_TacticalJacket",
            "GEAR_WolfSkinCape",
            "GEAR_QualityWinterCoat",
            "GEAR_DeerskinCoat_MOD"
        };

        internal static void Tick()
        {
            if (!Core.IsAfflictionComponentReady()) return;

            SyncSplinterProtection();
            SyncProtectedArms();
            SyncPeaceOfMind();
        }

        internal static void ResetRuntime()
        {
            _wasSleeping = false;
        }

        private static bool HasMMAffliction<T>() where T : class
        {
            var mgr = AfflictionManager.GetAfflictionManagerInstance();
            if (mgr?.m_Afflictions == null) return false;

            for (int i = 0; i < mgr.m_Afflictions.Count; i++)
            {
                if (mgr.m_Afflictions[i] is T)
                    return true;
            }

            return false;
        }

        private static bool MatchesGearId(string gearName, string[] validIds)
        {
            if (string.IsNullOrEmpty(gearName)) return false;

            for (int i = 0; i < validIds.Length; i++)
            {
                if (gearName == validIds[i])
                    return true;
            }

            return false;
        }

        private const float MIN_PROTECTIVE_CONDITION = 0.40f;

        private static bool HasEnoughConditionForProtection(GearItem item)
        {
            return item != null && item.GetNormalizedCondition() >= MIN_PROTECTIVE_CONDITION;
        }

        private static GearItem GetProtectedHandsGloves()
        {
            var pm = GameManager.GetPlayerManagerComponent();
            if (pm == null) return null;

            var gloves = pm.GetClothingInSlot(ClothingRegion.Hands, ClothingLayer.Base);
            if (gloves == null) return null;

            if (!MatchesGearId(gloves.name, ProtectedHandsGearIds))
                return null;

            if (!HasEnoughConditionForProtection(gloves))
                return null;

            return gloves;
        }

        internal static bool HasSplinterProtection()
        {
            if (!Settings.options.IsSplinter)
                return false;

            return GetProtectedHandsGloves() != null;
        }

        internal static bool TryAbsorbSplinterWithProtectedHands()
        {
            GearItem protectedGloves = GetProtectedHandsGloves();
            if (protectedGloves == null) return false;

            float before = protectedGloves.GetNormalizedCondition() * 100f;
            DegradeItemByMaxConditionPercent(protectedGloves, 5f);
            float after = protectedGloves.GetNormalizedCondition() * 100f;

            Core.Log($"ProtectedHands absorbed splinter: {protectedGloves.name} ({before:0.#}% -> {after:0.#}%)");
            return true;
        }

        private static void SyncSplinterProtection()
        {
            GearItem validGloves = GetProtectedHandsGloves();
            bool shouldHaveBuff = Settings.options.IsSplinter && validGloves != null;
            bool hasBuff = HasSplinterProtectionBuff();

            if (shouldHaveBuff)
            {
                if (!hasBuff)
                {
                    if (Core.TryStartCustomAffliction(new ProtectedHandsBuff(), "ProtectedHands")) Core.Log($"ProtectedHands applied: {validGloves.name}");
                }
            }
            else
            {
                if (!hasBuff) return;

                var mgr = AfflictionManager.GetAfflictionManagerInstance();
                if (mgr?.m_Afflictions == null) return;

                for (int i = mgr.m_Afflictions.Count - 1; i >= 0; i--)
                {
                    var a = mgr.m_Afflictions[i];
                    if (a == null) continue;

                    if (a is ProtectedHandsBuff)
                    {
                        a.Cure();
                    }
                }
            }
        }

        private static bool HasSplinterProtectionBuff()
        {
            var mgr = AfflictionManager.GetAfflictionManagerInstance();
            if (mgr?.m_Afflictions == null) return false;

            for (int i = 0; i < mgr.m_Afflictions.Count; i++)
            {
                var a = mgr.m_Afflictions[i];
                if (a is ProtectedHandsBuff)
                    return true;
            }

            return false;
        }

        private static GearItem GetProtectedArmsItem(out ClothingLayer layer)
        {
            layer = ClothingLayer.Base;

            var pm = GameManager.GetPlayerManagerComponent();
            if (pm == null) return null;

            for (int i = 0; i < ChestLayers.Length; i++)
            {
                var currentLayer = ChestLayers[i];
                var item = pm.GetClothingInSlot(ClothingRegion.Chest, currentLayer);
                if (item == null) continue;

                if (!MatchesGearId(item.name, ProtectedArmsGearIds))
                    continue;

                if (!HasEnoughConditionForProtection(item))
                    continue;

                layer = currentLayer;
                return item;
            }

            return null;
        }

        internal static bool HasProtectedArms()
        {
            return GetProtectedArmsItem(out _) != null;
        }

        internal static bool TryAbsorbScratchWithProtectedArms()
        {
            GearItem protectedItem = GetProtectedArmsItem(out ClothingLayer layer);
            if (protectedItem == null) return false;

            float before = protectedItem.GetNormalizedCondition() * 100f;
            DegradeItemByMaxConditionPercent(protectedItem, 5f);
            float after = protectedItem.GetNormalizedCondition() * 100f;

            Core.Log($"ProtectedArms absorbed scratch: {protectedItem.name} | layer: {layer} | {before:0.#}% -> {after:0.#}%");

            return true;
        }

        private static void SyncProtectedArms()
        {
            GearItem validChestItem = GetProtectedArmsItem(out ClothingLayer layer);

            bool shouldHaveBuff = validChestItem != null;
            bool hasBuff = HasProtectedArmsBuff();

            if (shouldHaveBuff)
            {
                if (!hasBuff)
                {
                    if (Core.TryStartCustomAffliction(new ProtectedArmsBuff(), "ProtectedArms")) Core.Log($"ProtectedArms applied: {validChestItem.name} | layer: {layer}");
                }
            }
            else
            {
                if (!hasBuff) return;

                var mgr = AfflictionManager.GetAfflictionManagerInstance();
                if (mgr?.m_Afflictions == null) return;

                for (int i = mgr.m_Afflictions.Count - 1; i >= 0; i--)
                {
                    var a = mgr.m_Afflictions[i];
                    if (a == null) continue;

                    if (a is ProtectedArmsBuff)
                    {
                        a.Cure();
                    }
                }
            }
        }

        private static bool HasProtectedArmsBuff()
        {
            var mgr = AfflictionManager.GetAfflictionManagerInstance();
            if (mgr?.m_Afflictions == null) return false;

            for (int i = 0; i < mgr.m_Afflictions.Count; i++)
            {
                var a = mgr.m_Afflictions[i];
                if (a is ProtectedArmsBuff)
                    return true;
            }

            return false;
        }

        private static void DegradeItemByMaxConditionPercent(GearItem item, float percentOfMaxCondition)
        {
            if (item == null || percentOfMaxCondition <= 0f) return;

            float start = item.GetNormalizedCondition();
            if (start <= 0f) return;

            float target = Mathf.Max(0f, start - (percentOfMaxCondition / 100f));
            DegradeItemUntilTargetCondition(item, target);
        }

        private static void DegradeItemUntilTargetCondition(GearItem item, float targetNormalizedCondition)
        {
            if (item == null) return;

            float[] degradeSteps = {5f, 1f, 0.25f, 0.05f, 0.01f};

            int safety = 0;

            for (int s = 0; s < degradeSteps.Length; s++)
            {
                float step = degradeSteps[s];

                while (item.GetNormalizedCondition() > targetNormalizedCondition && safety++ < 2000)
                {
                    float before = item.GetNormalizedCondition();
                    item.Degrade(step);
                    float after = item.GetNormalizedCondition();

                    if (after >= before)
                        return;
                }

                if (item.GetNormalizedCondition() <= targetNormalizedCondition)
                    return;
            }
        }

        private static void SyncPeaceOfMind()
        {
            if (!Settings.options.IsBadDream)
            {
                CurePeaceOfMind();
                _wasSleeping = false;
                return;
            }

            var rest = GameManager.GetRestComponent();
            if (rest == null)
            {
                _wasSleeping = false;
                return;
            }

            bool isSleeping = rest.IsSleeping();

            if (!isSleeping && _wasSleeping)
            {
                if (CanGainPeaceOfMind() && !HasPeaceOfMind())
                {
                    Core.TryStartCustomAffliction(new PeaceOfMindBuff(), "PeaceOfMind");
                }
            }

            _wasSleeping = isSleeping;
        }

        internal static void CurePeaceOfMind()
        {
            var mgr = AfflictionManager.GetAfflictionManagerInstance();
            if (mgr?.m_Afflictions == null) return;

            for (int i = mgr.m_Afflictions.Count - 1; i >= 0; i--)
            {
                var a = mgr.m_Afflictions[i];
                if (a == null) continue;

                if (a is PeaceOfMindBuff)
                {
                    a.Cure();
                }
            }
        }

        internal static bool HasPeaceOfMind()
        {
            var mgr = AfflictionManager.GetAfflictionManagerInstance();
            if (mgr?.m_Afflictions == null) return false;

            for (int i = 0; i < mgr.m_Afflictions.Count; i++)
            {
                var a = mgr.m_Afflictions[i];
                if (a is PeaceOfMindBuff) return true;
            }

            return false;
        }

        private static bool CanGainPeaceOfMind()
        {
            bool thresholdReached = Core.State.HoursSinceLastWildlifeAttack >= Settings.options.PeaceOfMindThreshold;

            bool hasActiveWildlifeStress = Core.State.AnimalStressScore > 0f || Core.State.AnimalStressTimer >= 0f;

            bool hasBadDream = HasMMAffliction<BadDreamAffliction>();
            bool hasNightTerror = HasMMAffliction<NightTerrorAffliction>();

            return thresholdReached && !hasActiveWildlifeStress && !hasBadDream && !hasNightTerror;
        }
    }
}