using BepInEx;
using RoR2;
using UnityEngine;
using BepInEx.Configuration;
using RoR2.Skills;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine.AddressableAssets;
using SkillSwap.Utils;

namespace SkillSwap {
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [BepInDependency("com.KingEnderBrine.ScrollableLobbyUI")]
    public class SkillSwap : BaseUnityPlugin {
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "pseudopulse";
        public const string PluginName = "SkillSwap";
        public const string PluginVersion = "1.0.0";
        public static BepInEx.Logging.ManualLogSource ModLogger;
        public static ConfigFile config;
        public void Awake() {
            // set logger
            ModLogger = Logger;
            config = Config;

            Passives.Setup();
            Melee.Perform();
            Transforms.Perform();
            Components.Perform();
            SkillHandler.Perform();
            RealPassives.Hook();
        }
    }
}
