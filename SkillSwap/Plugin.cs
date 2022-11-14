using BepInEx;
using R2API;
using R2API.Utils;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Reflection;
using BepInEx.Configuration;
using RiskOfOptions.Options;
using RiskOfOptions;
using UnityEngine.Networking;

namespace SkillSwap {
    [BepInDependency(R2API.R2API.PluginGUID)]
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [R2APISubmoduleDependency(nameof(ItemAPI), nameof(LanguageAPI))]
    [BepInDependency("com.rune580.riskofoptions")]
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

            Primary = Config.Bind<Skill>("Skills:", "Primary", Skill.SuppressiveFire, "Primary skill to use.");
            Secondary = Config.Bind<Skill>("Skills:", "Secondary", Skill.SuppressiveFire, "Secondary skill to use.");
            Utility = Config.Bind<Skill>("Skills:", "Utility", Skill.SuppressiveFire, "Utility skill to use.");
            Special = Config.Bind<Skill>("Skills:", "Special", Skill.SuppressiveFire, "Special skill to use.");

            ModSettingsManager.AddOption(new ChoiceOption(Primary));
            ModSettingsManager.AddOption(new ChoiceOption(Secondary));
            ModSettingsManager.AddOption(new ChoiceOption(Utility));
            ModSettingsManager.AddOption(new ChoiceOption(Special));

            Skills.PopulateMap();

            On.RoR2.CharacterBody.RecalculateStats += (orig, self) => {
                if (NetworkServer.active && self.isPlayerControlled) {
                    SkillLocator sl = self.skillLocator;
                    Skills.SwapSkill(sl.primary, Primary.Value, self.gameObject);
                    Skills.SwapSkill(sl.secondary, Secondary.Value, self.gameObject);
                    Skills.SwapSkill(sl.utility, Utility.Value, self.gameObject);
                    Skills.SwapSkill(sl.special, Special.Value, self.gameObject);
                }
                orig(self);
            };
        }
    }
}