using System;
using SkillSwap.Utils;

namespace SkillSwap {
    public class Components {
        private static List<SkillDef> corruptions = new();
        internal static void Perform() {
            On.RoR2.CharacterBody.Start += Huntress;
            On.RoR2.CharacterBody.Start += Viend;

            corruptions.Add(Utils.Paths.SkillDef.CrushHealth.Load<SkillDef>());
            corruptions.Add(Utils.Paths.VoidSurvivorSkillDef.CrushCorruption.Load<VoidSurvivorSkillDef>());
            corruptions.Add(Passives.corruption);
        }

        private static void Huntress(On.RoR2.CharacterBody.orig_Start orig, CharacterBody self) {
            orig(self);
            foreach (GenericSkill skill in self.GetComponents<GenericSkill>()) {
                if (skill.skillDef is HuntressTrackingSkillDef) {
                    HuntressTrackingSkillDef def = skill.skillDef as HuntressTrackingSkillDef;
                    HuntressTracker tracker = self.GetComponent<HuntressTracker>();
                    if (!tracker) {
                        tracker = self.gameObject.AddComponent<HuntressTracker>();
                        tracker.maxTrackingDistance = 60;
                        tracker.maxTrackingAngle = 30;
                        tracker.trackerUpdateFrequency = 10;
                    }

                    skill.skillInstanceData = new HuntressTrackingSkillDef.InstanceData {
                        huntressTracker = tracker,
                    };
                }
            }
        }

        private static void Viend(On.RoR2.CharacterBody.orig_Start orig, CharacterBody self) {
            orig(self);
            foreach (GenericSkill skill in self.GetComponents<GenericSkill>()) {
                if (corruptions.Contains(skill.skillDef)) {
                    VoidSurvivorController controller = self.GetComponent<VoidSurvivorController>();

                    if (!controller) {
                        controller = self.gameObject.AddComponent<VoidSurvivorController>();
                        controller.characterBody = self;
                        controller.characterAnimator = self.characterDirection.modelAnimator ?? null;
                        controller.bodyStateMachine = EntityStateMachine.FindByCustomName(self.gameObject, "Body");
                        controller.weaponStateMachine = EntityStateMachine.FindByCustomName(self.gameObject, "Weapon");
                        controller.corruptionModeStateMachine = EntityStateMachine.FindByCustomName(self.gameObject, "CorruptMode");
                        controller.corruptedBuffDef = DLC1Content.Buffs.VoidSurvivorCorruptMode;
                        controller.overlayPrefab = Utils.Paths.GameObject.VoidSurvivorCorruptionUISimplified.Load<GameObject>();
                        controller.maxCorruption = 100f;
                        controller.minimumCorruptionPerVoidItem = 2;
                        controller.corruptionPerSecondInCombat = 3;
                        controller.corruptionPerSecondOutOfCombat = 3;
                        controller.corruptionForFullDamage = 50;
                        controller.corruptionForFullHeal = -100;
                        controller.corruptionPerCrit = 2;
                        controller.corruptionFractionPerSecondWhileCorrupted = -0.067f;
                        controller.overlayChildLocatorEntry = "CrosshairExtras";
                        controller.OnDisable();
                        controller.OnEnable();
                    }

                    if (skill.skillDef is VoidSurvivorSkillDef) {
                        skill.skillInstanceData = new VoidSurvivorSkillDef.InstanceData {
                            voidSurvivorController = controller
                        };
                    }
                }
            }
        }
    }
}