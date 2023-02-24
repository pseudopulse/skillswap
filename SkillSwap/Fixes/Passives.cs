using RoR2;
using SkillSwap.Utils;

namespace SkillSwap {
    public class Passives {
        private static SkillDef backstab;
        private static SkillDef env;
        private static PassiveItemSkillDef accelerators;
        public static PassiveItemSkillDef corruption;
        private static SkillDef weaken;
        private static SkillDef scrap;
        private static SkillDef doublejump;
        private static PassiveItemSkillDef microbots;
        private static SkillDef poison;
        private static SkillDef blight;
        private static List<SkillDef> passives = new();

        internal static void Setup() {
            poison = Utils.Paths.SkillDef.CrocoPassivePoison.Load<SkillDef>();
            blight = Utils.Paths.SkillDef.CrocoPassiveBlight.Load<SkillDef>();
            accelerators = Utils.Paths.PassiveItemSkillDef.RailgunnerBodyPassiveConvertCrit.Load<PassiveItemSkillDef>();
            corruption = Utils.Paths.PassiveItemSkillDef.VoidSurvivorPassive.Load<PassiveItemSkillDef>();

            env = CreatePassive(Utils.Paths.GameObject.MageBody);
            backstab = CreatePassive(Utils.Paths.GameObject.Bandit2Body);
            weaken = CreatePassive(Utils.Paths.GameObject.TreebotBody);
            doublejump = CreatePassive(Utils.Paths.GameObject.MercBody);
            scrap = CreatePassive(Utils.Paths.GameObject.LoaderBody);
            microbots = CreatePassiveItem(Utils.Paths.GameObject.CaptainBody, Utils.Paths.ItemDef.CaptainDefenseMatrix.Load<ItemDef>());

            passives = new() { scrap, poison, blight, accelerators, corruption, env, backstab, weaken, doublejump, microbots};

            MakePassivesReal();
        }

        private static void MakePassivesReal() {
            CharacterBody loader = Utils.Paths.GameObject.LoaderBody.Load<GameObject>().GetComponent<CharacterBody>();
            CharacterBody merc = Utils.Paths.GameObject.MercBody.Load<GameObject>().GetComponent<CharacterBody>();
            CharacterBody captain = Utils.Paths.GameObject.CaptainBody.Load<GameObject>().GetComponent<CharacterBody>();
            CharacterBody bandit = Utils.Paths.GameObject.Bandit2Body.Load<GameObject>().GetComponent<CharacterBody>();
            CharacterBody arti = Utils.Paths.GameObject.MageBody.Load<GameObject>().GetComponent<CharacterBody>();

            bandit.bodyFlags &= ~CharacterBody.BodyFlags.HasBackstabPassive;
            loader.bodyFlags &= ~CharacterBody.BodyFlags.IgnoreFallDamage;
            merc.baseJumpCount -= 1;
            GameObject.Destroy(captain.GetComponent<CaptainDefenseMatrixController>());

            On.RoR2.CharacterBody.Start += (orig, self) => {
                orig(self);
                if (!self.isPlayerControlled) {
                    return;
                }
                if (HasSkillEquipped(self, backstab)) {
                    self.bodyFlags |= CharacterBody.BodyFlags.HasBackstabPassive;
                }

                if (HasSkillEquipped(self, scrap)) {
                    self.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;
                }

                if (HasSkillEquipped(self, doublejump)) {
                    self.baseJumpCount += 1;
                }

                if (HasSkillEquipped(self, env)) {
                    EntityStateMachine m = EntityStateMachine.FindByCustomName(self.gameObject, "Jet");
                    m.enabled = true;
                }
                else {
                    EntityStateMachine m = EntityStateMachine.FindByCustomName(self.gameObject, "Jet");
                    m.enabled = false;
                }
            };

            On.RoR2.GlobalEventManager.OnHitEnemy += (orig, self, info, obj) => {
                if (info.attacker) {
                    CharacterBody body = info.attacker.GetComponent<CharacterBody>();
                    if (body) {
                        if ((info.damageType & DamageType.WeakOnHit) != 0) {
                            if (!HasSkillEquipped(body, weaken)) {
                                info.damageType &= ~DamageType.WeakOnHit;
                            }
                        }
                    }
                }

                orig(self, info, obj);
            };
        }

        public static void SetupPassives(GameObject survivor) {
            GenericSkill skill = null;
            foreach (GenericSkill s in survivor.GetComponents<GenericSkill>()) {
                if (s.skillName != null && s.skillFamily && (s.skillFamily as ScriptableObject).name.ToLower().Contains("passive")) {
                    skill = s;
                    break;
                }
            }
            if (skill == null) {
                skill = survivor.AddComponent<GenericSkill>();
            }
            skill.hideInCharacterSelect = true;
            SkillLocator locator = survivor.GetComponent<SkillLocator>();
            SkillFamily family = null;
            if (skill._skillFamily == null) {
                family = ScriptableObject.CreateInstance<SkillFamily>();
                (family as ScriptableObject).name = survivor.name + "Passive";
            }
            else {
                family = skill._skillFamily;
            }
            List<SkillFamily.Variant> variants = new();

            locator.passiveSkill.enabled = false;

            foreach (SkillDef passive in passives) {
                variants.Add(new SkillFamily.Variant {
                    skillDef = passive,
                    unlockableDef = null,
                    viewableNode = new(passive.skillNameToken, false, null)
                });
            }

            family.variants = variants.ToArray();

            skill._skillFamily = family;
            skill.skillName = survivor.name + "Passive";

            ContentAddition.AddSkillFamily(family);

            CrocoDamageTypeController controller = survivor.GetComponent<CrocoDamageTypeController>();
            if (!controller) {
                controller = survivor.AddComponent<CrocoDamageTypeController>();
            }
            controller.blightSkillDef = blight;
            controller.poisonSkillDef = poison;
            controller.passiveSkillSlot = skill;

            VoidSurvivorController voidc = survivor.GetComponent<VoidSurvivorController>();
            if (voidc) {
                GameObject.Destroy(voidc);
            }
        }

        private static SkillDef CreatePassive(String str) {
            GameObject survivor = str.Load<GameObject>();
            SkillLocator locator = survivor.GetComponent<SkillLocator>();
            SkillLocator.PassiveSkill passive = locator.passiveSkill;

            SkillDef skill = ScriptableObject.CreateInstance<SkillDef>();
            skill.icon = passive.icon;
            skill.activationState = new SerializableEntityStateType(typeof(Idle));
            skill.activationStateMachineName = "Body";
            skill.canceledFromSprinting = false;
            skill.cancelSprintingOnActivation = false;
            skill.skillNameToken = passive.skillNameToken;
            skill.skillName = survivor.name + "PassiveName";
            (skill as ScriptableObject).name = survivor.name + "PassiveName";
            skill.skillDescriptionToken = passive.skillDescriptionToken;

            ContentAddition.AddSkillDef(skill);
            return skill;
        }

        private static PassiveItemSkillDef CreatePassiveItem(string str, ItemDef item) {
            GameObject survivor = str.Load<GameObject>();
            SkillLocator locator = survivor.GetComponent<SkillLocator>();
            SkillLocator.PassiveSkill passive = locator.passiveSkill;

            PassiveItemSkillDef skill = ScriptableObject.CreateInstance<PassiveItemSkillDef>();
            skill.icon = passive.icon;
            skill.activationState = new SerializableEntityStateType(typeof(Idle));
            skill.activationStateMachineName = "Body";
            skill.canceledFromSprinting = false;
            skill.cancelSprintingOnActivation = false;
            skill.skillNameToken = passive.skillNameToken;
            skill.skillName = survivor.name + "PassiveName";
            (skill as ScriptableObject).name = survivor.name + "PassiveName";
            skill.skillDescriptionToken = passive.skillDescriptionToken;
            skill.passiveItem = item;

            ContentAddition.AddSkillDef(skill);
            return skill;
        }

        public static bool HasSkillEquipped(CharacterBody body, SkillDef skill) {
            foreach (GenericSkill slot in body.GetComponents<GenericSkill>()) {
               //  Debug.Log(slot.skillDef);
                if (slot.skillDef == skill) {
                    // Debug.Log("trur");
                    return true;
                }
            }
            return false;
        }
    }
}