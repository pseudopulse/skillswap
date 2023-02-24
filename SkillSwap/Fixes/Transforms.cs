using System;
using System.Linq;

namespace SkillSwap {
    public class Transforms {
        internal static void Perform() {
            On.RoR2.CharacterBody.Start += (orig, self) => {
                orig(self);
                ModelLocator locator = self.GetComponent<ModelLocator>();
                if (locator && locator.modelTransform) {
                    GameObject fallback = new("FallbackTransform");
                    fallback.transform.SetParent(locator.modelTransform);
                }    
            };

            On.ChildLocator.FindChild_string += (orig, self, str) => {
                Transform transform = orig(self, str);
                if (transform) {
                    return transform;
                }

                List<string> muzzles = new();
                self.transformPairs.ToList().ForEach(x => {
                    if (x.name.ToLower().Contains("muzzle")) {
                        muzzles.Add(x.name);
                    }
                });

                if (muzzles.Count >= 1) {
                    // string toFind = muzzles[UnityEngine.Random.Range(0, muzzles.Count)];
                    string toFind = muzzles[0];
                    return orig(self, toFind);
                }

                return self.transform.Find("FallbackTransform");
            };

            On.ChildLocator.FindChildIndex_string += (orig, self, str) => {
                int c = orig(self, str);
                if (c != -1) {
                    return c;
                }

                List<string> muzzles = new();
                self.transformPairs.ToList().ForEach(x => {
                    if (x.name.ToLower().Contains("muzzle")) {
                        muzzles.Add(x.name);
                    }
                });

                if (muzzles.Count >= 1) {
                    // string toFind = muzzles[UnityEngine.Random.Range(0, muzzles.Count)];
                    string toFind = muzzles[0];
                    return orig(self, toFind);
                }
                return -1;
            };
        }
    }
}