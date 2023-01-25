using BepInEx;
using RoR2;
using UnityEngine;
using BepInEx.Configuration;
using RoR2.Skills;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine.AddressableAssets;

namespace SkillSwap {
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [BepInDependency("com.KingEnderBrine.ScrollableLobbyUI")]
    public class SkillSwap : BaseUnityPlugin {
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "pseudopulse";
        public const string PluginName = "SkillSwap";
        public const string PluginVersion = "1.0.0";
        public static ConfigEntry<Skill> Primary;
        public static ConfigEntry<Skill> Secondary;
        public static ConfigEntry<Skill> Utility;
        public static ConfigEntry<Skill> Special;
        public static BepInEx.Logging.ManualLogSource ModLogger;
        public void Awake() {
            // set logger
            ModLogger = Logger;


    
            [SystemInitializer(new Type[] {typeof(SkillCatalog), typeof(SurvivorCatalog)})]
            void Guh() {
                Debug.Log("guh ran");
                foreach (SurvivorDef survDef in SurvivorCatalog.survivorDefs) {
                    Debug.Log("surv " + survDef.displayNameToken);
                    GameObject surv = survDef.bodyPrefab;

                    string[] names = {
                        "Hook", "Pylon", "Mouth", "Jet", "Stealth", "Slide", "Skillswap", "Weapon2", "Scope", "Backpack", "Reload", "WeaponMine",
                        "CorruptionSS"
                    };

                    foreach (string name in names) {
                        EntityStateMachine machine = surv.AddComponent<EntityStateMachine>();
                        machine.customName = name;
                        machine.initialStateType = new EntityStates.SerializableEntityStateType(typeof(EntityStates.Idle));
                        machine.mainStateType = new EntityStates.SerializableEntityStateType(typeof(EntityStates.Idle));
                        NetworkStateMachine machines = surv.GetComponent<NetworkStateMachine>();
                        List<EntityStateMachine> esms = machines.stateMachines.ToList();
                        esms.Add(machine);
                        machines.stateMachines = esms.ToArray();
                    }

                    if (surv.GetComponent<SkillLocator>()) {
                        // Debug.Log("surv " + survDef.displayNameToken + " has a skill locator");
                        SkillLocator locator = surv.GetComponent<SkillLocator>();
                        GenericSkill primary = locator.primary;
                        GenericSkill secondary = locator.secondary;
                        GenericSkill utility = locator.utility;
                        GenericSkill special = locator.special;

                        GenericSkill toolBot2 = null;
                        // (primary.skillFamily as ScriptableObject).name = "ToolbotBodyPrimary1";

                        foreach (GenericSkill skill in surv.GetComponents<GenericSkill>()) {
                            if (skill.skillFamily && (skill.skillFamily as ScriptableObject).name == "ToolbotBodyPrimary2") {
                                toolBot2 = skill;
                            }
                        }

                        /*if (!toolBot2) {
                            toolBot2 = surv.AddComponent<GenericSkill>();
                            SkillFamily family = ScriptableObject.CreateInstance<SkillFamily>();
                            (family as ScriptableObject).name = "ToolbotBodyPrimary2";
                            family.variants = new SkillFamily.Variant[1];
                            toolBot2._skillFamily = family;
                            toolBot2.skillName = "FireSpear38";
                        } */

                        if (primary && primary.skillFamily) {
                            Debug.Log("primary and primary sm real");
                            foreach (SkillDef def in SkillCatalog.allSkillDefs) {
                                bool alreadyExists = false;
                                foreach (SkillFamily.Variant variant in primary.skillFamily.variants) {
                                    if (variant.skillDef == def) {
                                        Debug.Log("already exists");
                                        alreadyExists = true;
                                    }
                                }
                                if (def.icon == null || Language.GetString(def.skillDescriptionToken) == "") continue;
                                if (alreadyExists) continue;
                                SkillFamily skillFamily = primary.skillFamily;
                                Array.Resize(ref skillFamily.variants, skillFamily.variants.Length + 1);
                                skillFamily.variants[skillFamily.variants.Length - 1] = new SkillFamily.Variant
                                {
                                    skillDef = def,
                                    unlockableDef = null,
                                    viewableNode = new ViewablesCatalog.Node(def.skillNameToken, false, null)
                                };
                            }
                        }

                        if (toolBot2 && toolBot2.skillFamily) {
                            Debug.Log("primary and primary sm real");
                            foreach (SkillDef def in SkillCatalog.allSkillDefs) {
                                bool alreadyExists = false;
                                foreach (SkillFamily.Variant variant in toolBot2.skillFamily.variants) {
                                    if (variant.skillDef == def) {
                                        Debug.Log("already exists");
                                        alreadyExists = true;
                                    }
                                }
                                if (def.icon == null || Language.GetString(def.skillDescriptionToken) == "") continue;
                                if (def.skillName == "adasd" || def.skillNameToken == "adasd") continue;
                                if (Language.GetString(def.skillNameToken).Contains("TEMP") || Language.GetString(def.skillNameToken).Contains("NAME") || Language.GetString(def.skillNameToken).Contains("_")) continue;
                                if (alreadyExists) continue;
                                SkillFamily skillFamily = toolBot2.skillFamily;
                                Array.Resize(ref skillFamily.variants, skillFamily.variants.Length + 1);
                                skillFamily.variants[skillFamily.variants.Length - 1] = new SkillFamily.Variant
                                {
                                    skillDef = def,
                                    unlockableDef = null,
                                    viewableNode = new ViewablesCatalog.Node(def.skillNameToken, false, null)
                                };
                            }
                        }

                        if (secondary && secondary.skillFamily) {
                            foreach (SkillDef def in SkillCatalog.allSkillDefs) {
                                bool alreadyExists = false;
                                foreach (SkillFamily.Variant variant in secondary.skillFamily.variants) {
                                    if (variant.skillDef == def) {
                                        alreadyExists = true;
                                    }
                                }
                                if (def.icon == null || Language.GetString(def.skillDescriptionToken) == "") continue;
                                if (alreadyExists) continue;
                                SkillFamily skillFamily = secondary.skillFamily;
                                Array.Resize(ref skillFamily.variants, skillFamily.variants.Length + 1);
                                skillFamily.variants[skillFamily.variants.Length - 1] = new SkillFamily.Variant
                                {
                                    skillDef = def,
                                    unlockableDef = null,
                                    viewableNode = new ViewablesCatalog.Node(def.skillNameToken, false, null)
                                };
                            }
                        }

                        if (utility && utility.skillFamily) {
                            foreach (SkillDef def in SkillCatalog.allSkillDefs) {
                                bool alreadyExists = false;
                                foreach (SkillFamily.Variant variant in utility.skillFamily.variants) {
                                    if (variant.skillDef == def) {
                                        alreadyExists = true;
                                    }
                                }
                                if (def.icon == null || Language.GetString(def.skillDescriptionToken) == "") continue;
                                if (alreadyExists) continue;
                                SkillFamily skillFamily = utility.skillFamily;
                                Array.Resize(ref skillFamily.variants, skillFamily.variants.Length + 1);
                                skillFamily.variants[skillFamily.variants.Length - 1] = new SkillFamily.Variant
                                {
                                    skillDef = def,
                                    unlockableDef = null,
                                    viewableNode = new ViewablesCatalog.Node(def.skillNameToken, false, null)
                                };
                            }
                        }

                        if (special && special.skillFamily) {
                            foreach (SkillDef def in SkillCatalog.allSkillDefs) {
                                bool alreadyExists = false;
                                foreach (SkillFamily.Variant variant in special.skillFamily.variants) {
                                    if (variant.skillDef == def) {
                                        alreadyExists = true;
                                    }
                                }
                                if (def.icon == null || Language.GetString(def.skillDescriptionToken) == "") continue;
                                if (alreadyExists) continue;
                                SkillFamily skillFamily = special.skillFamily;
                                Array.Resize(ref skillFamily.variants, skillFamily.variants.Length + 1);
                                skillFamily.variants[skillFamily.variants.Length - 1] = new SkillFamily.Variant
                                {
                                    skillDef = def,
                                    unlockableDef = null,
                                    viewableNode = new ViewablesCatalog.Node(def.skillNameToken, false, null)
                                };
                            }
                        }
                    }
                }

                
            }


            On.ChildLocator.FindChild_string += (orig, self, str) => {
                if (orig(self, str) == null) {
                    return self.transform.Find("TheFunny");
                }
                else {
                    return orig(self, str);
                }
            };

            On.RoR2.CharacterBody.Start += (orig, surv) => {
                orig(surv);
                if (surv.GetComponent<ModelLocator>()) {
                    ModelLocator locator = surv.GetComponent<ModelLocator>();
                    if (locator.modelTransform) {
                        GameObject defHitbox = new("DefaultSSHitbox");
                        BoxCollider collider = defHitbox.AddComponent<BoxCollider>();
                        collider.size = new Vector3(60, 40, 60);
                        HitBox hitbox = defHitbox.AddComponent<HitBox>();
                        defHitbox.layer = LayerIndex.triggerZone.intVal;
                        collider.isTrigger = true;
                        defHitbox.transform.SetParent(locator.modelTransform);
                        defHitbox.transform.position = locator.modelTransform.position;
                        defHitbox.transform.localPosition = new Vector3(0, 1, 1.5f);
                        defHitbox.transform.localScale *= 3.5f;


                        HitBoxGroup group = locator.modelTransform.gameObject.AddComponent<HitBoxGroup>();
                        group.groupName = "DefaultSSGroup";
                        group.hitBoxes = new HitBox[] { hitbox };

                        GameObject theFunny = new("TheFunny");
                        theFunny.transform.SetParent(locator.modelTransform);
                        theFunny.transform.position = locator.modelTransform.position;
                    }
                }
            };

            On.EntityStates.BaseState.FindHitBoxGroup += (orig, self, str) => {
                if (orig(self, str)) {
                    Debug.Log("returning the real one");
                    return orig(self, str);
                }
                else {
                    Debug.Log("returning the default one");
                    return orig(self, "DefaultSSGroup");
                }
            };

            On.RoR2.SkillLocator.FindSkillByFamilyName += (orig, self, str) => {
                if (orig(self, str)) {
                    return orig(self, str);
                }
                else {
                    if (self.secondary) {
                        return self.secondary;
                    }
                    else {
                        return null;
                    }
                }
            };
        }
    }
}