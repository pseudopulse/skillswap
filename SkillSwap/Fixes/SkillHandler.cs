using System;
using RoR2.ContentManagement;
using System.Collections.Generic;
using SkillSwap.Utils;

namespace SkillSwap
{
    public class SkillHandler
    {
        private struct StateMachine {
            public string name;
            public SerializableEntityStateType initial;
            public SerializableEntityStateType main;
        }
        private static List<StateMachine> machines = new();
        private static List<SkillDef> primaries = new();
        private static List<SkillDef> secondaries = new();
        private static List<SkillDef> utilites = new();
        private static List<SkillDef> specials = new();
        private static List<SkillDef> all = new();
        private static SurvivorDef heretic;
        internal static void Perform()
        {
            heretic = Utils.Paths.SurvivorDef.Heretic.Load<SurvivorDef>();
            On.RoR2.ContentManagement.ContentManager.SetContentPacks += AddSkills;
            // fixes retool
            On.RoR2.SkillLocator.FindSkillByFamilyName += (orig, self, str) => {
                GenericSkill skill = orig(self, str);
                if (skill) {
                    return skill;
                }

                if (str == "ToolbotBodyPrimary1") {
                    return self.allSkills.First(x => x.skillName != null && (x.skillName.Contains("FireNailgun") ||  x.skillName.Contains("Primary")));
                }

                if (str == "ToolbotBodyPrimary2") {
                    return self.allSkills.First(x => x.skillName != null && x.skillName.Contains("FireSpear"));
                }

                return null;
            };

            // fixes supply drop
            On.RoR2.SkillLocator.FindSkill += (orig, self, str) => {
                GenericSkill skill = orig(self, str);
                if (skill) {
                    return skill;
                }

                if (str == "SupplyDrop1") {
                    return self.allSkills.First(x => x.skillName != null && x.skillName.Contains("SD1"));
                }

                if (str == "SupplyDrop2") {
                    return self.allSkills.First(x => x.skillName != null && x.skillName.Contains("SD2"));
                }

                return null;
            };
        }

        private static void AddSkills(On.RoR2.ContentManagement.ContentManager.orig_SetContentPacks orig, List<ReadOnlyContentPack> packs)
        {
            orig(packs);
            bool restrictSkills = SkillSwap.config.Bind<bool>("Configuration", "Proper Slots Only", true, "Skills can only be equipped in their usual slots (eg lodr grapple can only be on m2s)").Value;
            bool enableExtraSlots = SkillSwap.config.Bind<bool>("Configuration", "Enable Extra Slots", true, "Enables the extra skill slots needed by certain skills like Retool and Supply Beacon. Can get cluttery.").Value;

            foreach (SurvivorDef survivor in ContentManager.survivorDefs) { // first pass to collect skilldefs
                if (survivor == heretic) {
                    continue;
                }
                GameObject prefab = survivor.bodyPrefab;
                SkillLocator locator = prefab.GetComponent<SkillLocator>();
                CollectSkills(locator.primary.skillFamily, ref primaries);
                CollectSkills(locator.secondary.skillFamily, ref secondaries);
                CollectSkills(locator.utility.skillFamily, ref utilites);
                CollectSkills(locator.special.skillFamily, ref specials);
                CollectMachines(prefab);
            }

            foreach (SurvivorDef survivor in ContentManager.survivorDefs)
            {
                if (survivor == heretic) {
                    continue;
                }
                GameObject prefab = survivor.bodyPrefab;
                SkillLocator locator = prefab.GetComponent<SkillLocator>();

                SetupStateMachines(prefab);

                if (enableExtraSlots)
                {
                    HandleSpecialSkills(prefab);
                }

                if (restrictSkills)
                {
                    ApplySkills(primaries, ref locator.primary._skillFamily);
                    ApplySkills(secondaries, ref locator.secondary._skillFamily);
                    ApplySkills(utilites, ref locator.utility._skillFamily);
                    ApplySkills(specials, ref locator.special._skillFamily);
                }
                else
                {
                    ApplySkills(all, ref locator.primary._skillFamily);
                    ApplySkills(all, ref locator.secondary._skillFamily);
                    ApplySkills(all, ref locator.utility._skillFamily);
                    ApplySkills(all, ref locator.special._skillFamily);
                }

                if (prefab.name == "ToolbotBody") {
                    List<SkillDef> skills = restrictSkills ? primaries : all;
                    GenericSkill skill = prefab.GetComponents<GenericSkill>().First(x => x.skillName != null && x.skillName == "FireSpear");
                    ApplySkills(skills, ref skill._skillFamily);
                }

                Passives.SetupPassives(prefab);
            }
        }

        internal static void CollectSkills(SkillFamily family, ref List<SkillDef> list)
        {
            foreach (SkillFamily.Variant variant in family.variants)
            {
                list.Add(variant.skillDef);
                all.Add(variant.skillDef);
            }
        }

        internal static void ApplySkills(List<SkillDef> skills, ref SkillFamily family)
        {
            foreach (SkillDef skill in skills)
            {
                bool has = family.variants.Where(x => x.skillDef == skill).Count() != 0;
                if (has)
                {
                    continue;
                }

                SkillFamily.Variant variant = new SkillFamily.Variant
                {
                    skillDef = skill,
                    viewableNode = new(skill.skillNameToken, false, null)
                };

                HG.ArrayUtils.ArrayAppend(ref family.variants, in variant);
            }
        }

        internal static void HandleSpecialSkills(GameObject survivor)
        {
            bool isToolbot = survivor.name == "ToolbotBody";
            bool isCaptain = survivor.name == "CaptainBody";
            bool restrictSkills = SkillSwap.config.Bind<bool>("Configuration", "Proper Slots Only", true, "Skills can only be equipped in their usual slots (eg lodr grapple can only be on m2s)").Value;
            string name = survivor.name;
            if (!isToolbot)
            {
                List<SkillDef> skills = restrictSkills ? primaries : all;
                CreateGenericSkill(survivor, name + "FireSpear", skills);
            }

            if (!isCaptain)
            {
                List<SkillDef> beacons = new() {
                    Utils.Paths.SkillDef.CallSupplyDropHealing.Load<SkillDef>(),
                    Utils.Paths.SkillDef.CallSupplyDropShocking.Load<SkillDef>(),
                    Utils.Paths.SkillDef.CallSupplyDropEquipmentRestock.Load<SkillDef>(),
                    Utils.Paths.SkillDef.CallSupplyDropHacking.Load<SkillDef>(),
                };

                CreateGenericSkill(survivor, name + "SD1", beacons);
                CreateGenericSkill(survivor, name + "SD2", beacons);
            }
        }

        internal static void CreateGenericSkill(GameObject survivor, string name, List<SkillDef> skills)
        {
            GenericSkill skill = survivor.AddComponent<GenericSkill>();
            skill.skillName = name;
            skill.name = name;
            skill.hideInCharacterSelect = true;
            SkillFamily family = ScriptableObject.CreateInstance<SkillFamily>();
            (family as ScriptableObject).name = name + "Family";
            family.variants = new SkillFamily.Variant[] {
                new SkillFamily.Variant {
                    skillDef = skills[0],
                    viewableNode = new(skills[0].skillNameToken, false, null)
                }
            };
            skill._skillFamily = family;
            ApplySkills(skills, ref family);
        }

        internal static void CollectMachines(GameObject survivor) {
            foreach (EntityStateMachine machine in survivor.GetComponents<EntityStateMachine>()) {
                if (!ContainsMachine(machine)) {
                    StateMachine m = new();
                    m.name = machine.customName;
                    m.initial = machine.initialStateType;
                    m.main = machine.mainStateType;
                    if (m.name == "Body") {
                        m.initial = new(typeof(EntityStates.Mage.MageCharacterMain));
                        m.main = m.initial;
                    }
                    machines.Add(m);
                }
            }

            static bool ContainsMachine(EntityStateMachine machine) {
                if (machine.customName == null) {
                    return true;
                }

                foreach (StateMachine m in machines) {
                    if (m.name == machine.customName) {
                        return true;
                    }
                }

                return false;
            }
        }

        internal static void SetupStateMachines(GameObject survivor)
        {
            foreach (StateMachine machine in machines) {
                if (!HasMachine(survivor, machine.name)) {
                    NetworkStateMachine nsm = survivor.GetComponent<NetworkStateMachine>();
                    EntityStateMachine esm = survivor.AddComponent<EntityStateMachine>();
                    esm.customName = machine.name;
                    esm.initialStateType = machine.initial;
                    esm.mainStateType = machine.main;
                    List<EntityStateMachine> esms = nsm.stateMachines.ToList();
                    esms.Add(esm);
                    nsm.stateMachines = esms.ToArray();
                }
            }
        }

        internal static bool HasMachine(GameObject survivor, string name)
        {
            foreach (EntityStateMachine machine in survivor.GetComponents<EntityStateMachine>())
            {
                if (machine.customName != null && machine.customName == name)
                {
                    if (machine.customName == "Body") {
                        machine.initialStateType = new(typeof(EntityStates.Mage.MageCharacterMain));
                        machine.mainStateType = new(typeof(EntityStates.Mage.MageCharacterMain));
                    }
                    return true;
                }
            }

            return false;
        }
    }
}