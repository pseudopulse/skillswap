using System;

namespace SkillSwap {
    public class Melee {
        internal static void Perform() {
            On.RoR2.CharacterBody.Start += (orig, self) => {
                orig(self);
                ModelLocator locator = self.GetComponent<ModelLocator>();
                if (locator.modelTransform) {
                    GameObject defHitbox = new("DefaultSSHitbox");
                    BoxCollider collider = defHitbox.AddComponent<BoxCollider>();
                    collider.size = new Vector3(240, 180, 240);
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
                }    
            };

            On.EntityStates.BaseState.FindHitBoxGroup += (orig, self, str) => {
                HitBoxGroup group = orig(self, str);
                if (group) {
                    return group;
                }
                
                return orig(self, "DefaultSSGroup");
            };

            On.RoR2.OverlapAttack.Fire += (orig, self, res) => {
                if (self.hitBoxGroup == null) {
                    if (self.attacker) {
                        CharacterBody body = self.attacker.GetComponent<CharacterBody>();
                        if (body && body.modelLocator && body.modelLocator.modelTransform) {
                            HitBoxGroup[] groups = body.modelLocator.modelTransform.GetComponents<HitBoxGroup>();
                            self.hitBoxGroup = Array.Find(groups, x => x.groupName == "DefaultSSGroup");
                            Debug.Log(self.hitBoxGroup.groupName);
                        }
                    }
                }

                return orig(self, res);
            };
        }
    }
}