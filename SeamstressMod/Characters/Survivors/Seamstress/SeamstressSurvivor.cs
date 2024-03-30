using BepInEx.Configuration;
using SeamstressMod.Modules;
using SeamstressMod.Modules.Characters;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using RoR2.UI;
using R2API;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using UnityEngine.UI;
using SeamstressMod.SkillStates;

namespace SeamstressMod.Survivors.Seamstress
{
    public class SeamstressSurvivor : SurvivorBase<SeamstressSurvivor>
    {
        //todo guide
        //used to load the assetbundle for this character. must be unique
        public override string assetBundleName => "seamstressassets"; //if you do not change this, you are giving permission to deprecate the mod

        //the name of the prefab we will create. conventionally ending in "Body". must be unique
        public override string bodyName => "SeamstressBody"; //if you do not change this, you get the point by now

        //name of the ai master for vengeance and goobo. must be unique
        public override string masterName => "SeamstressMonsterMaster"; //if you do not

        //the names of the prefabs you set up in unity that we will use to build your character
        public override string modelPrefabName => "mdlHenry";
        public override string displayPrefabName => "HenryDisplay";

        public const string SEAMSTRESS_PREFIX = SeamstressPlugin.DEVELOPER_PREFIX + "_SEAMSTRESS_";

        //used when registering your survivor's language tokens
        public override string survivorTokenPrefix => SEAMSTRESS_PREFIX;

        internal static GameObject characterPrefab;
        //store extra skills here

        public override BodyInfo bodyInfo => new BodyInfo
        {
            bodyName = bodyName,
            bodyNameToken = SEAMSTRESS_PREFIX + "NAME",
            subtitleNameToken = SEAMSTRESS_PREFIX + "SUBTITLE",

            characterPortrait = assetBundle.LoadAsset<Texture>("texHenryIcon"),
            bodyColor = new Color(155f / 255f, 55f / 255f, 55f / 255f),
            sortPosition = 100,

            crosshair = Modules.Assets.LoadCrosshair("SimpleDot"),
            podPrefab = null,
            initialStateType = new EntityStates.SerializableEntityStateType(typeof(SkillStates.SeamstressSpawnState)),

            maxHealth = 160f,
            healthRegen = 1f,
            armor = 10f,
            damage = 8f,

            damageGrowth = 0f,
            healthGrowth = 160f * 0.3f,

            jumpCount = 1,
        };

        public override CustomRendererInfo[] customRendererInfos => new CustomRendererInfo[]
        {

                new CustomRendererInfo
                {
                    childName = "SwordModel",
                    material = assetBundle.LoadMaterial("matHenry"),
                },
                new CustomRendererInfo
                {
                    childName = "GunModel",
                },
                new CustomRendererInfo
                {
                    childName = "Model",
                }

        };

        public override UnlockableDef characterUnlockableDef => SeamstressUnlockables.characterUnlockableDef;

        public override ItemDisplaysBase itemDisplays => new SeamstressItemDisplays();

        //set in base classes
        public override AssetBundle assetBundle { get; protected set; }
        public override GameObject bodyPrefab { get; protected set; }
        public override CharacterBody prefabCharacterBody { get; protected set; }
        public override GameObject characterModelObject { get; protected set; }
        public override CharacterModel prefabCharacterModel { get; protected set; }
        public override GameObject displayPrefab { get; protected set; }

        public override void Initialize()
        {

            //uncomment if you have multiple characters
            //ConfigEntry<bool> characterEnabled = Config.CharacterEnableConfig("Survivors", "Henry");

            //if (!characterEnabled.Value)
            //    return;

            //need the character unlockable before you initialize the survivordef

            base.Initialize();
        }

        public override void InitializeCharacter()
        {
            SeamstressUnlockables.Init();

            SeamstressCrosshair.Init(assetBundle);

            base.InitializeCharacter();

            DamageTypes.Init();

            SeamstressConfig.Init();
            SeamstressStates.Init();
            SeamstressTokens.Init();

            SeamstressAssets.Init(assetBundle);
            SeamstressBuffs.Init(assetBundle);

            Dots.Init();

            InitializeEntityStateMachines();
            InitializeSkills();
            InitializeSkins();
            InitializeCharacterMaster();

            AdditionalBodySetup();

            characterPrefab = this.bodyPrefab;
            AddHooks();
        }

        private void AdditionalBodySetup()
        {
            AddHitboxes();
            bool tempAdd(CharacterBody body) => body.HasBuff(SeamstressBuffs.manipulatedCd);
            bool tempAdd2(CharacterBody body) => body.HasBuff(SeamstressBuffs.manipulated);
            float pee(CharacterBody body) => 2f * body.radius;
            bodyPrefab.AddComponent<SeamstressController>();
            bodyPrefab.AddComponent<ScissorController>();
            bodyPrefab.AddComponent<Tracker>();
            TempVisualEffectAPI.AddTemporaryVisualEffect(SeamstressAssets.sewn1, pee, tempAdd);
            TempVisualEffectAPI.AddTemporaryVisualEffect(SeamstressAssets.sewn3, pee, tempAdd2);
            //TempVisualEffectAPI.AddTemporaryVisualEffect(SeamstressAssets.stitchTempEffectPrefab, tempAdd);
            //bodyPrefab.AddComponent<HuntressTrackerComopnent>();
            //anything else here
        }
        public void AddHitboxes()
        {
            Prefabs.SetupHitBoxGroup(characterModelObject, "Sword", "SwordHitbox");

            Prefabs.SetupHitBoxGroup(characterModelObject, "SwordBig", "SwordHitboxBig");

            Prefabs.SetupHitBoxGroup(characterModelObject, "Weave", "WeaveHitbox");

            Prefabs.SetupHitBoxGroup(characterModelObject, "WeaveBig", "WeaveHitboxBig");

            Prefabs.SetupHitBoxGroup(characterModelObject, "Right", "RightScissorHitbox");

            Prefabs.SetupHitBoxGroup(characterModelObject, "Left", "LeftScissorHitbox");
        }

        public override void InitializeEntityStateMachines()
        {
            //clear existing state machines from your cloned body (probably commando)
            //omit all this if you want to just keep theirs
            Prefabs.ClearEntityStateMachines(bodyPrefab);

            //if you set up a custom main characterstate, set it up here
            //don't forget to register custom entitystates in your HenryStates.cs
            //the main "body" state machine has some special properties
            Prefabs.AddMainEntityStateMachine(bodyPrefab, "Body", typeof(SkillStates.SeamstressMainState), typeof(EntityStates.SpawnTeleporterState));

            Prefabs.AddEntityStateMachine(bodyPrefab, "Passive", typeof(SkillStates.SeamstressJump), typeof(SkillStates.SeamstressJump));
            Prefabs.AddEntityStateMachine(bodyPrefab, "Weapon");
            Prefabs.AddEntityStateMachine(bodyPrefab, "Weapon2");
        }

        #region skills
        public override void InitializeSkills()
        {
            bodyPrefab.AddComponent<SeamstressPassive>();
            Skills.CreateSkillFamilies(bodyPrefab);
            AddPassiveSkills();
            AddPrimarySkills();
            AddSecondarySkills();
            AddUtilitySkills();
            AddSpecialSkills();
        }

        private void AddPassiveSkills()
        {
            SeamstressPassive passive = bodyPrefab.GetComponent<SeamstressPassive>();

            SkillLocator skillLocator = bodyPrefab.GetComponent<SkillLocator>();

            skillLocator.passiveSkill.enabled = false;

            passive.blinkPassive = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = SeamstressSurvivor.SEAMSTRESS_PREFIX + "PASSIVE_NAME",
                skillNameToken = SeamstressSurvivor.SEAMSTRESS_PREFIX + "PASSIVE_NAME",
                skillDescriptionToken = SeamstressSurvivor.SEAMSTRESS_PREFIX + "PASSIVE_DESCRIPTION",
                skillIcon = assetBundle.LoadAsset<Sprite>("texImpTouchedIcon"),
                keywordTokens = new string[] { Tokens.needleKeyword },
                activationState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.Idle)),
                activationStateMachineName = "",
                baseMaxStock = 1,
                baseRechargeInterval = 0f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Any,
                resetCooldownTimerOnUse = false,
                isCombatSkill = false,
                mustKeyPress = false,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 2,
                stockToConsume = 1
            });

            Modules.Skills.AddPassiveSkills(passive.passiveSkillSlot.skillFamily, passive.blinkPassive);

            passive.impGauge = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = SeamstressSurvivor.SEAMSTRESS_PREFIX + "GAUGE_NAME",
                skillNameToken = SeamstressSurvivor.SEAMSTRESS_PREFIX + "GAUGE_NAME",
                skillDescriptionToken = SeamstressSurvivor.SEAMSTRESS_PREFIX + "GAUGE_DESCRIPTION",
                skillIcon = assetBundle.LoadAsset<Sprite>("texItHungersIcon"),
                keywordTokens = new string[] { Tokens.butcheredKeyword, Tokens.cutKeyword },
                activationState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.Idle)),
                activationStateMachineName = "",
                baseMaxStock = 1,
                baseRechargeInterval = 0f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Any,
                resetCooldownTimerOnUse = false,
                isCombatSkill = false,
                mustKeyPress = false,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 2,
                stockToConsume = 1
            });

            Modules.Skills.AddPassiveSkills(passive.impGaugeSkillSlot.skillFamily, passive.impGauge);
        }

        private void AddPrimarySkills()
        {
            //the primary skill is created using a constructor for a typical primary
            //it is also a SteppedSkillDef. Custom Skilldefs are very useful for custom behaviors related to casting a skill. see ror2's different skilldefs for reference
            SteppedSkillDef trimSkillDef = Skills.CreateSkillDef<SteppedSkillDef>(new SkillDefInfo
                (
                    "Trim",
                    SEAMSTRESS_PREFIX + "PRIMARY_TRIM_NAME",
                    SEAMSTRESS_PREFIX + "PRIMARY_TRIM_DESCRIPTION",
                    assetBundle.LoadAsset<Sprite>("texFlurryIcon"),
                    new EntityStates.SerializableEntityStateType(typeof(SkillStates.Trim)),
                    "Weapon"
                ));
            //custom Skilldefs can have additional fields that you can set manually
            trimSkillDef.stepCount = 3;
            trimSkillDef.stepGraceDuration = 1f;

            Skills.AddPrimarySkills(bodyPrefab, trimSkillDef);

            SteppedSkillDef flurrySkillDef = Skills.CreateSkillDef<SteppedSkillDef>(new SkillDefInfo
                (
                    "Flurry",
                    SEAMSTRESS_PREFIX + "PRIMARY_FLURRY_NAME",
                    SEAMSTRESS_PREFIX + "PRIMARY_FLURRY_DESCRIPTION",
                    assetBundle.LoadAsset<Sprite>("texFlurryIcon"),
                    new EntityStates.SerializableEntityStateType(typeof(SkillStates.Flurry)),
                    "Weapon"
                ));
            //custom Skilldefs can have additional fields that you can set manually
            flurrySkillDef.stepCount = 2;
            flurrySkillDef.stepGraceDuration = 1f;

            Skills.AddPrimarySkills(bodyPrefab, flurrySkillDef);
        }

        private void AddSecondarySkills()
        {
            SkillDef Clip = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "Clip",
                skillNameToken = SEAMSTRESS_PREFIX + "SECONDARY_CLIP_NAME",
                skillDescriptionToken = SEAMSTRESS_PREFIX + "SECONDARY_CLIP_DESCRIPTION",
                keywordTokens = new string[] { Tokens.sentienceRangeKeyword, Tokens.needleKeyword },
                skillIcon = assetBundle.LoadAsset<Sprite>("texClipIcon"),

                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Clip)),

                activationStateMachineName = "Weapon",
                interruptPriority = EntityStates.InterruptPriority.Skill,

                baseMaxStock = 1,
                baseRechargeInterval = 6,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,

                resetCooldownTimerOnUse = false,
                fullRestockOnAssign = false,
                dontAllowPastMaxStocks = false,
                beginSkillCooldownOnSkillEnd = false,
                mustKeyPress = true,

                isCombatSkill = true,
                canceledFromSprinting = false,
                cancelSprintingOnActivation = true,
                forceSprintDuringState = false,
            });

            Skills.AddSecondarySkills(bodyPrefab, Clip);
            /*
            SkillDef planarShift = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "PlanarShift",
                skillNameToken = SEAMSTRESS_PREFIX + "SECONDARY_PLANARSHIFT_NAME",
                skillDescriptionToken = SEAMSTRESS_PREFIX + "SECONDARY_PLANARSHIFT_DESCRIPTION",
                keywordTokens = new string[] { Tokens.needleKeyword },
                skillIcon = assetBundle.LoadAsset<Sprite>("texSpecialIcon"),

                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.SecondaryBlink)),
                
                activationStateMachineName = "Weapon",
                interruptPriority = EntityStates.InterruptPriority.Skill,

                baseMaxStock = 1,
                baseRechargeInterval = 6,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,

                resetCooldownTimerOnUse = false,
                fullRestockOnAssign = false,
                dontAllowPastMaxStocks = false,
                beginSkillCooldownOnSkillEnd = false,
                mustKeyPress = true,

                isCombatSkill = true,
                canceledFromSprinting = false,
                cancelSprintingOnActivation = false,
                forceSprintDuringState = false,
            });

            Skills.AddSecondarySkills(bodyPrefab, planarShift);
            */
            TrackingSkillDef planarManipulation = Skills.CreateSkillDef<TrackingSkillDef>(new SkillDefInfo
            {
                skillName = "PlanarManipulation",
                skillNameToken = SEAMSTRESS_PREFIX + "SECONDARY_PLANMAN_NAME",
                skillDescriptionToken = SEAMSTRESS_PREFIX + "SECONDARY_PLANMAN_DESCRIPTION",
                keywordTokens = new string[] { Tokens.manipulateKeyword },
                skillIcon = assetBundle.LoadAsset<Sprite>("texSpecialIcon"),

                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Telekinesis)),

                activationStateMachineName = "Weapon2",
                interruptPriority = EntityStates.InterruptPriority.Skill,

                baseMaxStock = 1,
                baseRechargeInterval = 0.5f,
                rechargeStock = 1,
                requiredStock = 0,
                stockToConsume = 0,

                resetCooldownTimerOnUse = false,
                fullRestockOnAssign = false,
                dontAllowPastMaxStocks = false,
                beginSkillCooldownOnSkillEnd = false,
                mustKeyPress = true,

                isCombatSkill = true,
                canceledFromSprinting = false,
                cancelSprintingOnActivation = false,
                forceSprintDuringState = false,
            });

            Skills.AddSecondarySkills(bodyPrefab, planarManipulation);
        }

        private void AddUtilitySkills()
        {
            SkillDef heartTearSeamstressSkillDef = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "HeartDashSeamstress",
                skillNameToken = SEAMSTRESS_PREFIX + "UTILITY_HEARTDASH_NAME",
                skillDescriptionToken = SEAMSTRESS_PREFIX + "UTILITY_HEARTDASH_DESCRIPTION",
                keywordTokens = new string[] { Tokens.butcheredKeyword, Tokens.cutKeyword, Tokens.hemorrhageKeyword },
                skillIcon = assetBundle.LoadAsset<Sprite>("texUtilityIcon"),

                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.HealthCostDash)),
                activationStateMachineName = "Weapon",
                interruptPriority = EntityStates.InterruptPriority.Skill,

                baseRechargeInterval = 8f,
                baseMaxStock = 1,

                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,

                resetCooldownTimerOnUse = false,
                fullRestockOnAssign = false,
                dontAllowPastMaxStocks = false,
                mustKeyPress = true,
                beginSkillCooldownOnSkillEnd = false,

                isCombatSkill = true,
                canceledFromSprinting = false,
                cancelSprintingOnActivation = false,
                forceSprintDuringState = false,

            });

            Skills.AddUtilitySkills(bodyPrefab, heartTearSeamstressSkillDef);

            SkillDef parrySeamstressSkillDef = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "ParrySeamstress",
                skillNameToken = SeamstressSurvivor.SEAMSTRESS_PREFIX + "UTILITY_PARRY_NAME",
                skillDescriptionToken = SeamstressSurvivor.SEAMSTRESS_PREFIX + "UTILITY_PARRY_DESCRIPTION",
                keywordTokens = new string[] { Tokens.sentienceRangeKeyword, Tokens.butcheredKeyword, Tokens.cutKeyword },
                skillIcon = assetBundle.LoadAsset<Sprite>("texBoxingGlovesIcon"),

                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Parry)),
                activationStateMachineName = "Weapon",
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,

                baseRechargeInterval = 8f,
                baseMaxStock = 1,

                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,

                resetCooldownTimerOnUse = false,
                fullRestockOnAssign = true,
                dontAllowPastMaxStocks = false,
                mustKeyPress = true,
                beginSkillCooldownOnSkillEnd = false,

                isCombatSkill = true,
                canceledFromSprinting = false,
                cancelSprintingOnActivation = true,
                forceSprintDuringState = false,
            });

            Skills.AddUtilitySkills(bodyPrefab, parrySeamstressSkillDef);
        }

        private void AddSpecialSkills()
        {
            SkillDef fireScissor = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "FireSeamstress",
                skillNameToken = SeamstressSurvivor.SEAMSTRESS_PREFIX + "SPECIAL_FIRE_NAME",
                skillDescriptionToken = SeamstressSurvivor.SEAMSTRESS_PREFIX + "SPECIAL_FIRE_DESCRIPTION",
                keywordTokens = new string[] { Tokens.sentienceKeyword },
                skillIcon = assetBundle.LoadAsset<Sprite>("texBoxingGlovesIcon"),

                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.FireScissor)),
                activationStateMachineName = "Weapon",
                interruptPriority = EntityStates.InterruptPriority.Skill,

                baseRechargeInterval = 16f,
                baseMaxStock = 2,

                rechargeStock = 2,
                requiredStock = 1,
                stockToConsume = 1,

                resetCooldownTimerOnUse = false,
                fullRestockOnAssign = false,
                dontAllowPastMaxStocks = true,
                mustKeyPress = true,
                beginSkillCooldownOnSkillEnd = false,

                isCombatSkill = true,
                canceledFromSprinting = false,
                cancelSprintingOnActivation = true,
                forceSprintDuringState = false,
            });

            Skills.AddSpecialSkills(bodyPrefab, fireScissor);
        }
        #endregion skills

        #region skins
        public override void InitializeSkins()
        {
            ModelSkinController skinController = prefabCharacterModel.gameObject.AddComponent<ModelSkinController>();
            ChildLocator childLocator = prefabCharacterModel.GetComponent<ChildLocator>();

            CharacterModel.RendererInfo[] defaultRendererinfos = prefabCharacterModel.baseRendererInfos;

            List<SkinDef> skins = new List<SkinDef>();

            #region DefaultSkin
            //this creates a SkinDef with all default fields
            SkinDef defaultSkin = Skins.CreateSkinDef("DEFAULT_SKIN",
                assetBundle.LoadAsset<Sprite>("texMainSkin"),
                defaultRendererinfos,
                prefabCharacterModel.gameObject);

            //these are your Mesh Replacements. The order here is based on your CustomRendererInfos from earlier
            //pass in meshes as they are named in your assetbundle
            //currently not needed as with only 1 skin they will simply take the default meshes
            //uncomment this when you have another skin
            //defaultSkin.meshReplacements = Modules.Skins.getMeshReplacements(assetBundle, defaultRendererinfos,
            //    "meshHenrySword",
            //    "meshHenryGun",
            //    "meshHenry");

            //add new skindef to our list of skindefs. this is what we'll be passing to the SkinController
            skins.Add(defaultSkin);
            #endregion

            //uncomment this when you have a mastery skin
            #region MasterySkin

            ////creating a new skindef as we did before
            //SkinDef masterySkin = Modules.Skins.CreateSkinDef(HENRY_PREFIX + "MASTERY_SKIN_NAME",
            //    assetBundle.LoadAsset<Sprite>("texMasteryAchievement"),
            //    defaultRendererinfos,
            //    prefabCharacterModel.gameObject,
            //    HenryUnlockables.masterySkinUnlockableDef);

            ////adding the mesh replacements as above. 
            ////if you don't want to replace the mesh (for example, you only want to replace the material), pass in null so the order is preserved
            //masterySkin.meshReplacements = Modules.Skins.getMeshReplacements(assetBundle, defaultRendererinfos,
            //    "meshHenrySwordAlt",
            //    null,//no gun mesh replacement. use same gun mesh
            //    "meshHenryAlt");

            ////masterySkin has a new set of RendererInfos (based on default rendererinfos)
            ////you can simply access the RendererInfos' materials and set them to the new materials for your skin.
            //masterySkin.rendererInfos[0].defaultMaterial = assetBundle.LoadMaterial("matHenryAlt");
            //masterySkin.rendererInfos[1].defaultMaterial = assetBundle.LoadMaterial("matHenryAlt");
            //masterySkin.rendererInfos[2].defaultMaterial = assetBundle.LoadMaterial("matHenryAlt");

            ////here's a barebones example of using gameobjectactivations that could probably be streamlined or rewritten entirely, truthfully, but it works
            //masterySkin.gameObjectActivations = new SkinDef.GameObjectActivation[]
            //{
            //    new SkinDef.GameObjectActivation
            //    {
            //        gameObject = childLocator.FindChildGameObject("GunModel"),
            //        shouldActivate = false,
            //    }
            //};
            ////simply find an object on your child locator you want to activate/deactivate and set if you want to activate/deacitvate it with this skin

            //skins.Add(masterySkin);

            #endregion

            skinController.skins = skins.ToArray();
        }
        #endregion skins


        //Character Master is what governs the AI of your character when it is not controlled by a player (artifact of vengeance, goobo)
        public override void InitializeCharacterMaster()
        {
            //if you're lazy or prototyping you can simply copy the AI of a different character to be used
            //Modules.Prefabs.CloneDopplegangerMaster(bodyPrefab, masterName, "Merc");

            //how to set up AI in code
            SeamstressAI.Init(bodyPrefab, masterName);

            //how to load a master set up in unity, can be an empty gameobject with just AISkillDriver components
            //assetBundle.LoadMaster(bodyPrefab, masterName);
        }

        private void AddHooks()
        {
            RoR2.UI.HUD.onHudTargetChangedGlobal += HUDSetup;
            On.RoR2.HealthComponent.Heal += new On.RoR2.HealthComponent.hook_Heal(HealthComponent_Heal);
            On.RoR2.CharacterModel.UpdateOverlays += new On.RoR2.CharacterModel.hook_UpdateOverlays(CharacterModel_UpdateOverlays);
            On.RoR2.HealthComponent.TakeDamage += new On.RoR2.HealthComponent.hook_TakeDamage(HealthComponent_TakeDamage);
            On.RoR2.Orbs.LightningOrb.Begin += new On.RoR2.Orbs.LightningOrb.hook_Begin(LightningOrb_Begin);
            R2API.RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
            On.RoR2.MapZone.TryZoneStart += new On.RoR2.MapZone.hook_TryZoneStart(DisableOOBCheck);

            On.RoR2.UI.LoadoutPanelController.Rebuild += LoadoutPanelController_Rebuild;
        }
        private static void LoadoutPanelController_Rebuild(On.RoR2.UI.LoadoutPanelController.orig_Rebuild orig, LoadoutPanelController self)
        {
            orig(self);

            if (self.currentDisplayData.bodyIndex == BodyCatalog.FindBodyIndex("SeamstressBody"))
            {
                foreach (LanguageTextMeshController i in self.gameObject.GetComponentsInChildren<LanguageTextMeshController>())
                {
                    if (i && i.token == "LOADOUT_SKILL_MISC") i.token = "Passive";
                }
            }
        }
        private void DisableOOBCheck(On.RoR2.MapZone.orig_TryZoneStart orig, MapZone self, Collider other)
        {
            CharacterBody component = other.gameObject.GetComponent<CharacterBody>();
            if(component.baseNameToken == "KENKO_SEAMSTRESS_NAME")
            {
                if (!component.HasBuff(SeamstressBuffs.manipulated))
                {
                    orig.Invoke(self, other);
                }
            }
            else
            {
                orig.Invoke(self, other);
            }
        }
        private void LightningOrb_Begin(On.RoR2.Orbs.LightningOrb.orig_Begin orig, RoR2.Orbs.LightningOrb self)
        {
            GameObject zap = null;
            if (self.lightningType == RoR2.Orbs.LightningOrb.LightningType.Count && self.attacker.GetComponent<CharacterBody>().baseNameToken == "KENKO_SEAMSTRESS_NAME")
            {
                zap = LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/OrbEffects/BeamSphereOrbEffect");
                zap.transform.GetChild(0).GetComponent<LineRenderer>().material.SetColor("_TintColor", SeamstressAssets.coolRed);
                self.duration = 0.1f;
                EffectData effectData = new EffectData
                {
                    origin = self.origin,
                    genericFloat = self.duration
                };
                effectData.SetHurtBoxReference(self.target);
                EffectManager.SpawnEffect(zap, effectData, transmit: true);
            }
            else
            {
                orig.Invoke(self);
            }
        }
        private void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            if (!NetworkServer.active)
            {
                return;
            }
            CharacterBody victimBody = self.body;
            CharacterBody attackerBody = null;
            if (damageInfo.attacker)
            {
                attackerBody = damageInfo.attacker.GetComponent<CharacterBody>();
            }
            float damageCheck = damageInfo.damage;
            if (damageCheck > 0)
            {
                if (damageInfo.HasModdedDamageType(DamageTypes.CutDamage))
                {
                    if (victimBody.isBoss)
                    {
                        DotController.InflictDot(victimBody.gameObject, attackerBody.gameObject, Dots.SeamstressBossDot, SeamstressStaticValues.cutDuration, damageInfo.procCoefficient);
                    }
                    else
                    {
                        DotController.InflictDot(victimBody.gameObject, attackerBody.gameObject, Dots.SeamstressDot, SeamstressStaticValues.cutDuration, damageInfo.procCoefficient);
                    }
                }
                if (victimBody && victimBody.baseNameToken == "KENKO_SEAMSTRESS_NAME" && victimBody.HasBuff(SeamstressBuffs.parryStart) || victimBody.HasBuff(SeamstressBuffs.butchered))
                {
                    if (victimBody.HasBuff(SeamstressBuffs.parryStart))
                    {
                        victimBody.RemoveBuff(SeamstressBuffs.parryStart);
                        if (!victimBody.HasBuff(SeamstressBuffs.parrySuccess))
                        {
                            victimBody.AddBuff(SeamstressBuffs.parrySuccess);
                        }
                        victimBody.AddTimedBuff(RoR2Content.Buffs.Immune, SeamstressStaticValues.parryWindow + 0.5f);
                    }
                    else if (victimBody.HasBuff(SeamstressBuffs.butchered) && damageInfo.dotIndex != Dots.ButcheredDot)
                    {
                        SeamstressController s = victimBody.gameObject.GetComponent<SeamstressController>();
                        s.ImpGaugeCalc(-damageInfo.damage);
                        float num = s.ImpGaugeAmount() - damageInfo.damage;
                        if (num < 0)
                        {
                            if (victimBody.skillLocator.utility.skillDef == SeamstressAssets.snapBackSkillDef)
                            {
                                victimBody.skillLocator.utility.ExecuteIfReady();
                            }
                            orig.Invoke(self, damageInfo);
                        }
                    }
                    else
                    {
                        orig.Invoke(self, damageInfo);
                    }
                }
                else
                {
                    orig.Invoke(self, damageInfo);
                }
                if (victimBody && victimBody.baseNameToken == "KENKO_SEAMSTRESS_NAME")
                {
                    victimBody.RecalculateStats();
                }
            }
        }
        private void CharacterModel_UpdateOverlays(On.RoR2.CharacterModel.orig_UpdateOverlays orig, CharacterModel self)
        {
            orig.Invoke(self);
            if (!self || !self.body || self.body.baseNameToken != "KENKO_SEAMSTRESS_NAME")
            {
                return;
            }
            SeamstressController s = self.body.GetComponent<SeamstressController>();
            if (self.body.HasBuff(SeamstressBuffs.butchered) && s.inButchered == false)
            {
                TemporaryOverlay temporaryOverlay = self.gameObject.AddComponent<TemporaryOverlay>();
                temporaryOverlay.duration = SeamstressStaticValues.butcheredDuration;
                temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0.4f);
                temporaryOverlay.animateShaderAlpha = true;
                temporaryOverlay.destroyComponentOnEnd = true;
                temporaryOverlay.originalMaterial = SeamstressAssets.butcheredOverlayMat;
                temporaryOverlay.AddToCharacerModel(self);
                s.inButchered = true;
            }
            else if (!self.body.HasBuff(SeamstressBuffs.butchered) && s.inButchered == true)
            {
                s.inButchered = false;
                if (self.gameObject.GetComponent<TemporaryOverlay>() != null)
                {
                    UnityEngine.Object.Destroy(self.gameObject.GetComponent<TemporaryOverlay>());
                }
            }
            if (self.body.HasBuff(SeamstressBuffs.parryStart))
            {
                TemporaryOverlay temporaryOverlay = self.gameObject.AddComponent<TemporaryOverlay>();
                temporaryOverlay.duration = 0.4f;
                temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                temporaryOverlay.animateShaderAlpha = true;
                temporaryOverlay.destroyComponentOnEnd = true;
                temporaryOverlay.originalMaterial = SeamstressAssets.parryMat;
                temporaryOverlay.AddToCharacerModel(self);
            }
        }

        private float HealthComponent_Heal(On.RoR2.HealthComponent.orig_Heal orig, HealthComponent self, float amount, ProcChainMask procChainMask, bool nonRegen = true)
        {
            if (self.body.HasBuff(SeamstressBuffs.butchered) && self.body.baseNameToken == "KENKO_SEAMSTRESS_NAME")
            {
                amount *= SeamstressStaticValues.healConversion;
            }
            var res = orig(self, amount, procChainMask, nonRegen);
            if (self.body.baseNameToken == "KENKO_SEAMSTRESS_NAME")
            {
                SeamstressController s = self.body.GetComponent<SeamstressController>();
                if (self.body.TryGetComponent<SeamstressController>(out s) && self.body.HasBuff(SeamstressBuffs.butchered))
                {
                    if(self.health >= self.fullHealth) s.ImpGaugeCalc((amount / SeamstressStaticValues.healConversion) * (1 - SeamstressStaticValues.healConversion));
                    else s.ImpGaugeCalc((res / SeamstressStaticValues.healConversion) * (1 - SeamstressStaticValues.healConversion));
                }
            }
            return res;
        }
        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, R2API.RecalculateStatsAPI.StatHookEventArgs args)
        {
            if(sender == null) return;
            if (sender.baseNameToken == "KENKO_SEAMSTRESS_NAME")
            {
                SeamstressController s = sender.GetComponent<SeamstressController>();
                if (s != null)
                {
                    if (s.ImpGaugeAmount() > 0)
                    {
                        args.baseMoveSpeedAdd += (2f * s.ImpGaugeAmountPercent());
                    }
                }
                if (!sender.HasBuff(SeamstressBuffs.scissorLeftBuff))
                {
                    args.attackSpeedMultAdd += .1f;
                    args.baseMoveSpeedAdd += 1f;
                }
                if (!sender.HasBuff(SeamstressBuffs.scissorRightBuff))
                {
                    args.attackSpeedMultAdd += .1f;
                    args.baseMoveSpeedAdd += 1f;
                }
                if (sender.inventory.GetItemCount(DLC1Content.Items.EquipmentMagazineVoid) != 0)
                {
                    args.attackSpeedMultAdd += (.1f * sender.inventory.GetItemCount(DLC1Content.Items.EquipmentMagazineVoid));
                    args.baseMoveSpeedAdd += (1f * sender.inventory.GetItemCount(DLC1Content.Items.EquipmentMagazineVoid));
                }
            }
        }
        public static float GetICBMDamageMult(CharacterBody body)
        {
            float mult = 1f;
            if (body && body.inventory)
            {
                int itemcount = body.inventory.GetItemCount(DLC1Content.Items.MoreMissile);
                int stack = itemcount - 1;
                if (stack > 0) mult += stack * 0.5f;
            }
            return mult;
        }
        internal static void HUDSetup(RoR2.UI.HUD hud)
        {
            if (hud.targetBodyObject && hud.targetMaster.bodyPrefab == characterPrefab)
            {
                if (!hud.targetMaster.hasAuthority) return;

                Transform healthbarContainer = hud.transform.Find("MainContainer").Find("MainUIArea").Find("SpringCanvas").Find("BottomLeftCluster").Find("BarRoots").Find("LevelDisplayCluster");

                if (!hud.transform.Find("MainContainer").Find("MainUIArea").Find("SpringCanvas").Find("BottomLeftCluster").Find("ImpGauge"))
                {
                    GameObject impGauge = GameObject.Instantiate(healthbarContainer.gameObject, hud.transform.Find("MainContainer").Find("MainUIArea").Find("SpringCanvas").Find("BottomLeftCluster"));
                    impGauge.name = "ImpGauge";

                    GameObject.DestroyImmediate(impGauge.transform.GetChild(0).gameObject);
                    MonoBehaviour.Destroy(impGauge.GetComponentInChildren<LevelText>());
                    MonoBehaviour.Destroy(impGauge.GetComponentInChildren<ExpBar>());

                    ImpGauge impGaugeComponent = impGauge.AddComponent<ImpGauge>();
                    impGaugeComponent.targetHUD = hud;
                    impGaugeComponent.fillRectTransform = impGauge.transform.Find("ExpBarRoot").GetChild(0).GetChild(0).GetComponent<RectTransform>();

                    impGauge.transform.Find("LevelDisplayRoot").Find("ValueText").gameObject.SetActive(false);
                    impGauge.transform.Find("LevelDisplayRoot").Find("PrefixText").gameObject.SetActive(false);

                    impGauge.transform.Find("ExpBarRoot").GetChild(0).GetComponent<Image>().enabled = true;

                    impGauge.transform.Find("LevelDisplayRoot").GetComponent<RectTransform>().anchoredPosition = new Vector2(-12f, 0f);

                    RectTransform rect = impGauge.GetComponent<RectTransform>();
                    rect.anchorMax = new Vector2(1f, 1f);
                    rect.localPosition = new Vector2(740f, 430f);
                    rect.localScale = new Vector2(0.5f, 0.5f);
                }
                if (!hud.transform.Find("MainContainer").Find("MainUIArea").Find("CrosshairCanvas").Find("SeamstressCrosshair"))
                {
                    GameObject seamstressCrosshair = GameObject.Instantiate(SeamstressCrosshair.seamstressCrosshair, hud.transform.Find("MainContainer").Find("MainUIArea").Find("CrosshairCanvas"));
                    seamstressCrosshair.name = "SeamstressCrosshair";
                    seamstressCrosshair.gameObject.GetComponent<HudElement>().targetBodyObject = hud.targetBodyObject;
                    seamstressCrosshair.gameObject.GetComponent<HudElement>().targetCharacterBody = hud.targetBodyObject.GetComponent<CharacterBody>();
                }
            }
        }
    }
}