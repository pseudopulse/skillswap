using BepInEx;
using R2API;
using R2API.Utils;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System;
using System.Linq;
using RoR2.Skills;
using System.Collections.Generic;

namespace SkillSwap {
    public class Skills {
        public static Dictionary<Skill, string> map = new();
        
        public static void SwapSkill(GenericSkill slot, Skill skill) {
            string guh = "";
            map.TryGetValue(skill, out guh);
            try {
                SkillDef def = Addressables.LoadAssetAsync<SkillDef>(guh).WaitForCompletion();
                slot.skillDef = def;
            }
            catch {
                try {
                    RailgunSkillDef def = Addressables.LoadAssetAsync<RailgunSkillDef>(guh).WaitForCompletion();
                    slot.skillDef = def;
                }
                catch {
                    try {
                        SteppedSkillDef def = Addressables.LoadAssetAsync<SteppedSkillDef>(guh).WaitForCompletion();
                        slot.skillDef = def;
                    }
                    catch {
                        try {
                            GroundedSkillDef def = Addressables.LoadAssetAsync<GroundedSkillDef>(guh).WaitForCompletion();
                            slot.skillDef = def;
                        }
                        catch {
                            SkillSwap.ModLogger.LogFatal("the j");
                        }
                    }
                }
            }
        }
        public static void PopulateMap() {
            // bandit
            map.Add(Skill.Shiv, "RoR2/Base/Bandit2/Bandit2SerratedShivs.asset");
            map.Add(Skill.LightOut, "RoR2/Base/Bandit2/ResetRevolver.asset");
            map.Add(Skill.Desperado, "RoR2/Base/Bandit2/SkullRevolver.asset");
            map.Add(Skill.Knife, "RoR2/Base/Bandit2/SlashBlade.asset");
            map.Add(Skill.SmokeBomb, "RoR2/Base/Bandit2/ThrowSmokebomb.asset");
            map.Add(Skill.Blast, "RoR2/Base/Bandit2/Bandit2Blast.asset");
            map.Add(Skill.Burst, "RoR2/Base/Bandit2/FireShotgun2.asset");

            // captain
            map.Add(Skill.Orbital, "RoR2/Base/Captain/CallAirstrike.asset");
            map.Add(Skill.Diablo, "RoR2/Base/Captain/CallAirstrikeAlt.asset");
            map.Add(Skill.Shotgun, " RoR2/Base/Captain/CaptainShotgun.asset");
            map.Add(Skill.Taser, "RoR2/Base/Captain/CaptainTazer.asset");

            // commando
            map.Add(Skill.SuppressiveFire, "RoR2/Base/Commando/CommandoBodyBarrage.asset");
            map.Add(Skill.PhaseRound, "RoR2/Base/Commando/CommandoBodyFireFMJ.asset");
            map.Add(Skill.PhaseBlast, "RoR2/Base/Commando/CommandoBodyFireShotgunBlast.asset");
            map.Add(Skill.TacticalDive, "RoR2/Base/Commando/CommandoBodyRoll.asset");
            map.Add(Skill.TacticalSlide, "RoR2/Base/Commando/CommandoSlide.asset");
            map.Add(Skill.FragGrenades, "RoR2/Base/Commando/ThrowGrenade.asset");
            map.Add(Skill.DoubleTap, "RoR2/Base/Commando/CommandoBodyFirePistol.asset");

            // acrid
            map.Add(Skill.Bite, "RoR2/Base/Croco/CrocoBite.asset");
            map.Add(Skill.Caustic, "RoR2/Base/Croco/CrocoLeap.asset");
            map.Add(Skill.Frenzy, "RoR2/Base/Croco/CrocoChainableLeap.asset");
            map.Add(Skill.Epidemic, "RoR2/Base/Croco/CrocoDisease.asset");
            map.Add(Skill.Spit, "RoR2/Base/Croco/CrocoSpit.asset");
            map.Add(Skill.Wounds, "RoR2/Base/Croco/CrocoSlash.asset");
            
            // engi
            map.Add(Skill.BounceGrenades, "RoR2/Base/Engi/EngiBodyFireGrenade.asset");
            map.Add(Skill.PressureMines, "RoR2/Base/Engi/EngiBodyPlaceMine.asset");
            map.Add(Skill.SpiderMines, "RoR2/Base/Engi/EngiBodyPlaceSpiderMine.asset");
            map.Add(Skill.Shield, "RoR2/Base/Engi/EngiBodyPlaceBubbleShield.asset");
            map.Add(Skill.Harpoons, "RoR2/Base/Engi/EngiHarpoons.asset");
            map.Add(Skill.Carbonizer, "RoR2/Base/Engi/EngiBodyPlaceWalkerTurret.asset");
            map.Add(Skill.Guass, "RoR2/Base/Engi/EngiBodyPlaceTurret.asset");

            // huntress
            map.Add(Skill.Ballista, "RoR2/Base/Huntress/AimArrowSnipe.asset");
            map.Add(Skill.ArrowRain, "RoR2/Base/Huntress/HuntressBodyArrowRain.asset");
            map.Add(Skill.Blink, "RoR2/Base/Huntress/HuntressBodyBlink.asset");
            map.Add(Skill.PhaseBlink, "RoR2/Base/Huntress/HuntressBodyMiniBlink.asset");
            map.Add(Skill.Flurry, "RoR2/Base/Huntress/FireFlurrySeekingArrow.asset");
            map.Add(Skill.Strafe, "RoR2/Base/Huntress/HuntressBodyFireSeekingArrow.asset");
            map.Add(Skill.LaserGlaive, "RoR2/Base/Huntress/HuntressBodyGlaive.asset");

            // lodr
            map.Add(Skill.Pylon, "RoR2/Base/Loader/ThrowPylon.asset");
            map.Add(Skill.Punch, "RoR2/Base/Loader/ChargeFist.asset");
            map.Add(Skill.ElectricPunch, "RoR2/Base/Loader/ChargeZapFist.asset");
            map.Add(Skill.Thunderslam, "RoR2/Base/Loader/GroundSlam.asset");
            map.Add(Skill.FistSwing, "RoR2/Base/Loader/SwingFist.asset");
            
            // arti
            map.Add(Skill.Flamethrower, "RoR2/Base/Mage/MageBodyFlamethrower.asset");
            map.Add(Skill.Spear, "RoR2/Base/Mage/MageBodyIceBomb.asset");
            map.Add(Skill.Bomb, "RoR2/Base/Mage/MageBodyNovaBomb.asset");
            map.Add(Skill.Snapfreeze, "RoR2/Base/Mage/MageBodyWall.asset");
            map.Add(Skill.FlameBolt, "RoR2/Base/Mage/MageBodyFireFirebolt.asset");
            map.Add(Skill.PlasmaBolt, "RoR2/Base/Mage/MageBodyFireLightningBolt.asset");

            // merc
            map.Add(Skill.Eviscerate, "RoR2/Base/Merc/MercBodyEvis.asset");
            map.Add(Skill.Slicing, "RoR2/Base/Merc/MercBodyEvisProjectile.asset");
            map.Add(Skill.Focused, "RoR2/Base/Merc/MercBodyFocusedAssault.asset");
            map.Add(Skill.RisingThunder, "RoR2/Base/Merc/MercBodyUppercut.asset");
            map.Add(Skill.Whirlwind, "RoR2/Base/Merc/MercBodyWhirlwind.asset");
            map.Add(Skill.Blinding, "RoR2/Base/Merc/MercBodyAssaulter.asset");

            // mul-t
            map.Add(Skill.Retool, "RoR2/Base/Toolbot/ToolbotBodySwap.asset");
            map.Add(Skill.TransportMode, "RoR2/Base/Toolbot/ToolbotBodyToolbotDash.asset");
            map.Add(Skill.PowerMode, "RoR2/Base/Toolbot/ToolbotDualWield.asset");
            map.Add(Skill.Scrap, "RoR2/Base/Toolbot/ToolbotBodyFireGrenadeLauncher.asset");
            map.Add(Skill.PowerSaw, "RoR2/Base/Toolbot/ToolbotBodyFireBuzzsaw.asset");
            map.Add(Skill.Nailgun, "RoR2/Base/Toolbot/ToolbotBodyFireNailgun.asset");
            map.Add(Skill.Rebar, "RoR2/Base/Toolbot/ToolbotBodyFireSpear.asset");
            map.Add(Skill.BlastCan, "RoR2/Base/Toolbot/ToolbotBodyStunDrone.asset");

            // rex
            map.Add(Skill.Barrage, "RoR2/Base/Treebot/TreebotBodyAimMortar2.asset");
            map.Add(Skill.Drill, "RoR2/Base/Treebot/TreebotBodyAimMortarRain.asset");
            map.Add(Skill.Growth, "RoR2/Base/Treebot/TreebotBodyFireFlower2.asset");
            map.Add(Skill.Harvest, "RoR2/Base/Treebot/TreebotBodyFireFruitSeed.asset");
            map.Add(Skill.Inject, "RoR2/Base/Treebot/TreebotBodyFireSyringe.asset");
            map.Add(Skill.Volley, "RoR2/Base/Treebot/TreebotBodyPlantSonicBoom.asset");
            map.Add(Skill.Disperse, "RoR2/Base/Treebot/TreebotBodySonicBoom.asset");

            // railgunner
            map.Add(Skill.Super, "RoR2/DLC1/Railgunner/RailgunnerBodyChargeSnipeSuper.asset");
            map.Add(Skill.Cryo, "RoR2/DLC1/Railgunner/RailgunnerBodyChargeSnipeCryo.asset");
            map.Add(Skill.SmartRounds, "RoR2/DLC1/Railgunner/RailgunnerBodyFirePistol.asset");
            map.Add(Skill.Mines, "RoR2/DLC1/Railgunner/RailgunnerBodyFireMineConcussive.asset");
            map.Add(Skill.Polar, "RoR2/DLC1/Railgunner/RailgunnerBodyFireMineBlinding.asset");
            map.Add(Skill.M99, "RoR2/DLC1/Railgunner/RailgunnerBodyScopeHeavy.asset");
            map.Add(Skill.H44, "RoR2/DLC1/Railgunner/RailgunnerBodyScopeLight.asset");

            // void fiend
            map.Add(Skill.Drown, "RoR2/DLC1/VoidSurvivor/FireHandBeam.asset");
            map.Add(Skill.DrownCorrupt, "RoR2/DLC1/VoidSurvivor/FireCorruptBeam.asset");
            map.Add(Skill.Flood, "RoR2/DLC1/VoidSurvivor/ChargeMegaBlaster.asset");
            map.Add(Skill.FloodCorrupt, "RoR2/DLC1/VoidSurvivor/FireCorruptDisk.asset");
            map.Add(Skill.Trespass, "RoR2/DLC1/VoidSurvivor/VoidBlinkUp.asset");
            map.Add(Skill.TrespassCorrupt, "RoR2/DLC1/VoidSurvivor/VoidBlinkDown.asset");
        }
    }

    public enum Skill : int {
        // commando
        DoubleTap,
        PhaseRound,
        PhaseBlast,
        TacticalDive,
        TacticalSlide,
        SuppressiveFire,
        FragGrenades,

        // huntress
        Flurry,
        Strafe,
        LaserGlaive,
        Blink,
        PhaseBlink,
        ArrowRain,
        Ballista,

        // bandit
        Blast,
        Burst,
        Knife,
        Shiv,
        SmokeBomb,
        LightOut,
        Desperado,

        // mul-t
        Nailgun,
        Scrap,
        Rebar,
        Retool,
        BlastCan,
        TransportMode,
        PowerMode,
        PowerSaw,

        // engineer
        BounceGrenades,
        PressureMines,
        SpiderMines,
        Shield,
        Harpoons,
        Guass,
        Carbonizer,

        // merc
        LaserSword,
        Whirlwind,
        RisingThunder,
        Blinding,
        Focused,
        Eviscerate,
        Slicing,

        // arti
        FlameBolt,
        PlasmaBolt,
        Spear,
        Bomb,
        Snapfreeze,
        Flamethrower,

        // lodr
        FistSwing,
        Punch,
        ElectricPunch,
        Pylon,
        Thunderslam,

        // cap
        Shotgun,
        Taser,
        Orbital,
        Diablo,

        // rex
        Inject,
        Drill,
        Barrage,
        Disperse,
        Volley,
        Harvest,
        Growth,

        // viend
        Drown,
        DrownCorrupt,
        Flood,
        FloodCorrupt,
        Trespass,
        TrespassCorrupt,

        // railr
        SmartRounds,
        M99,
        H44,
        Mines,
        Polar,
        Super,
        Cryo,

        // acrid
        Wounds,
        Bite,
        Spit,
        Caustic,
        Frenzy,
        Epidemic
    }

}