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
using R2API.Networking;
using SeamstressMod.Seamstress.Components;
using SeamstressMod.Seamstress.Content;
using SeamstressMod.Seamstress.SkillStates;

namespace SeamstressMod.Seamstress
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

            crosshair = Assets.LoadCrosshair("SimpleDot"),
            podPrefab = null,
            initialStateType = new EntityStates.SerializableEntityStateType(typeof(SkillStates.SeamstressSpawnState)),

            maxHealth = 160f,
            healthRegen = 1f,
            armor = 0f,
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

        public static SkillDef snapBackSkillDef;
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

            characterPrefab = bodyPrefab;
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
            bodyPrefab.AddComponent<NeedleController>();
            bodyPrefab.AddComponent<Tracker>();
            TempVisualEffectAPI.AddTemporaryVisualEffect(SeamstressAssets.sewnCdEffect, pee, tempAdd);
            TempVisualEffectAPI.AddTemporaryVisualEffect(SeamstressAssets.sewnEffect, pee, tempAdd2);
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

            //if you set up a custom main characterstate, set it up here
            //don't forget to register custom entitystates in your HenryStates.cs
            //the main "body" state machine has some special properties
            bodyPrefab.GetComponent<CharacterBody>().preferredInitialStateType = new EntityStates.SerializableEntityStateType(typeof(SkillStates.SeamstressSpawnState));
            foreach (EntityStateMachine i in bodyPrefab.GetComponents<EntityStateMachine>())
            {
                if (i.customName == "Body") i.mainStateType = new EntityStates.SerializableEntityStateType(typeof(SkillStates.MainState));
            }
            EntityStateMachine passiveController = bodyPrefab.AddComponent<EntityStateMachine>();
            passiveController.initialStateType = new EntityStates.SerializableEntityStateType(typeof(SkillStates.SeamstressJump));
            passiveController.mainStateType = new EntityStates.SerializableEntityStateType(typeof(SkillStates.SeamstressJump));
            passiveController.customName = "Passive";

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

            passive.blinkPassive = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = SEAMSTRESS_PREFIX + "PASSIVE_NAME",
                skillNameToken = SEAMSTRESS_PREFIX + "PASSIVE_NAME",
                skillDescriptionToken = SEAMSTRESS_PREFIX + "PASSIVE_DESCRIPTION",
                skillIcon = assetBundle.LoadAsset<Sprite>("texItHungersIcon"),
                keywordTokens = new string[] { Tokens.insatiableKeyword, Tokens.whatCountsAsHealth },
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

            Skills.AddPassiveSkills(passive.passiveSkillSlot.skillFamily, passive.blinkPassive);

            passive.impGauge = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = SEAMSTRESS_PREFIX + "NEEDLE_NAME",
                skillNameToken = SEAMSTRESS_PREFIX + "NEEDLE_NAME",
                skillDescriptionToken = SEAMSTRESS_PREFIX + "NEEDLE_DESCRIPTION",
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

            Skills.AddPassiveSkills(passive.impGaugeSkillSlot.skillFamily, passive.impGauge);
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

                activationState = new EntityStates.SerializableEntityStateType(typeof(Clip)),

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
                skillIcon = assetBundle.LoadAsset<Sprite>("texPlanarManipulationIcon"),

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
                keywordTokens = new string[] { Tokens.insatiableKeyword },
                skillIcon = assetBundle.LoadAsset<Sprite>("texGlimpseOfCorruptionIcon"),

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
                skillNameToken = SEAMSTRESS_PREFIX + "UTILITY_PARRY_NAME",
                skillDescriptionToken = SEAMSTRESS_PREFIX + "UTILITY_PARRY_DESCRIPTION",
                keywordTokens = new string[] { Tokens.sentienceRangeKeyword, Tokens.insatiableKeyword },
                skillIcon = assetBundle.LoadAsset<Sprite>("texGlimpseOfPurityIcon"),

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

            snapBackSkillDef = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "SnapBack",
                skillNameToken = "SnapBack",
                skillDescriptionToken = "Snapback to core",
                keywordTokens = new string[] { },
                skillIcon = assetBundle.LoadAsset<Sprite>("texImpTouchedIcon"),

                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Snapback)),
                activationStateMachineName = "Body",
                interruptPriority = EntityStates.InterruptPriority.Pain,

                baseRechargeInterval = 0f,
                baseMaxStock = 1,

                rechargeStock = 0,
                requiredStock = 0,
                stockToConsume = 0,

                resetCooldownTimerOnUse = false,
                fullRestockOnAssign = true,
                dontAllowPastMaxStocks = true,
                beginSkillCooldownOnSkillEnd = false,
                mustKeyPress = true,

                isCombatSkill = false,
                canceledFromSprinting = false,
                cancelSprintingOnActivation = false,
                forceSprintDuringState = false,
            });
        }

        private void AddSpecialSkills()
        {
            SkillDef fireScissor = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "FireSeamstress",
                skillNameToken = SEAMSTRESS_PREFIX + "SPECIAL_FIRE_NAME",
                skillDescriptionToken = SEAMSTRESS_PREFIX + "SPECIAL_FIRE_DESCRIPTION",
                keywordTokens = new string[] { Tokens.sentienceKeyword },
                skillIcon = assetBundle.LoadAsset<Sprite>("texSkewerIcon"),

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
            HUD.onHudTargetChangedGlobal += HUDSetup;
            On.RoR2.HealthComponent.Heal += new On.RoR2.HealthComponent.hook_Heal(HealthComponent_Heal);
            On.RoR2.CharacterModel.UpdateOverlays += new On.RoR2.CharacterModel.hook_UpdateOverlays(CharacterModel_UpdateOverlays);
            On.RoR2.HealthComponent.TakeDamage += new On.RoR2.HealthComponent.hook_TakeDamage(HealthComponent_TakeDamage);
            On.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
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
        private void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            if (!NetworkServer.active)
            {
                return;
            }
            CharacterBody victimBody = self.body;
            if (victimBody && victimBody.baseNameToken == "KENKO_SEAMSTRESS_NAME")
            {
                if (victimBody.HasBuff(SeamstressBuffs.parryStart) && damageInfo.damage > 0)
                {
                    victimBody.RemoveBuff(SeamstressBuffs.parryStart);
                    if (!victimBody.HasBuff(SeamstressBuffs.parrySuccess))
                    {
                        victimBody.AddBuff(SeamstressBuffs.parrySuccess);
                    }
                    victimBody.AddTimedBuff(RoR2Content.Buffs.Immune, SeamstressStaticValues.parryWindow + 0.5f);
                    return;
                }
                else if (victimBody.HasBuff(SeamstressBuffs.instatiable) && damageInfo.dotIndex != Dots.SeamstressBleed)
                {
                    SeamstressController seamCom = victimBody.gameObject.GetComponent<SeamstressController>();
                    if (seamCom)
                    {
                        seamCom.FillHunger(-damageInfo.damage);
                        if (seamCom.fiendMeter - damageInfo.damage <= 0 && victimBody.skillLocator.utility.skillDef == snapBackSkillDef)
                        {
                            victimBody.skillLocator.utility.ExecuteIfReady();
                        }
                        return;
                    }
                }
            }
            orig.Invoke(self, damageInfo);
            if(victimBody && victimBody.baseNameToken == "KENKO_SEAMSTRESS_NAME") victimBody.RecalculateStats();
        }
        private void CharacterModel_UpdateOverlays(On.RoR2.CharacterModel.orig_UpdateOverlays orig, CharacterModel self)
        {
            orig.Invoke(self);
            if (!self || !self.body || self.body.baseNameToken != "KENKO_SEAMSTRESS_NAME")
            {
                return;
            }
            SeamstressController s = self.body.GetComponent<SeamstressController>();
            if (self.body.HasBuff(SeamstressBuffs.instatiable) && s.inInsatiable == false)
            {
                TemporaryOverlay temporaryOverlay = self.gameObject.AddComponent<TemporaryOverlay>();
                temporaryOverlay.duration = SeamstressStaticValues.butcheredDuration;
                temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0.4f);
                temporaryOverlay.animateShaderAlpha = true;
                temporaryOverlay.destroyComponentOnEnd = true;
                temporaryOverlay.originalMaterial = SeamstressAssets.butcheredOverlayMat;
                temporaryOverlay.AddToCharacerModel(self);
                s.inInsatiable = true;
            }
            else if (!self.body.HasBuff(SeamstressBuffs.instatiable) && s.inInsatiable == true)
            {
                s.inInsatiable = false;
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
            if (self && self.body.HasBuff(SeamstressBuffs.instatiable) && self.body.baseNameToken == "KENKO_SEAMSTRESS_NAME")
            {
                amount *= SeamstressStaticValues.healConversion;
            }
            var res = orig(self, amount, procChainMask, nonRegen);
            if (self.body.baseNameToken == "KENKO_SEAMSTRESS_NAME")
            {
                SeamstressController s = self.body.GetComponent<SeamstressController>();
                if (self.body.TryGetComponent(out s) && self.body.HasBuff(SeamstressBuffs.instatiable))
                {
                    if (self.health >= self.fullHealth) s.FillHunger(amount / SeamstressStaticValues.healConversion * (1 - SeamstressStaticValues.healConversion));
                    else s.FillHunger(res / SeamstressStaticValues.healConversion * (1 - SeamstressStaticValues.healConversion));
                }
            }
            return res;
        }
        private void CharacterBody_RecalculateStats(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
        {
            orig(self);

            if (!self || self.baseNameToken != "KENKO_SEAMSTRESS_NAME")
            {
                return;
            }
            SeamstressController s = self.GetComponent<SeamstressController>();
            HealthComponent healthComponent = self.GetComponent<HealthComponent>();
            SkillLocator skillLocator = self.GetComponent<SkillLocator>();
            if (s && healthComponent && skillLocator)
            {
                s.maxHunger = healthComponent.fullHealth * SeamstressStaticValues.maxFiendGaugeCoefficient;
                float healthMissing = healthComponent.fullHealth + (healthComponent.fullShield + healthComponent.barrier) / 2f + s.fiendMeter / 8 - (healthComponent.health);
                float fakeHealthMissing = healthComponent.fullHealth * 0.66f;
                if (s.inInsatiable && skillLocator.utility.skillNameToken == SEAMSTRESS_PREFIX + "UTILITY_PARRY_NAME") self.baseDamage = 8f + fakeHealthMissing * SeamstressStaticValues.passiveScaling + healthMissing * SeamstressStaticValues.passiveScaling;
                else self.baseDamage = 8f + healthMissing * SeamstressStaticValues.passiveScaling;
                if (s.fiendMeter > 0)
                {
                    self.moveSpeed += 2f * Util.Remap(s.fiendMeter, 0f, s.maxHunger, 0f, 2f);
                }
            }
            if (!self.HasBuff(SeamstressBuffs.scissorLeftBuff))
            {
                self.attackSpeed += .1f;
                self.moveSpeed += 1f;
            }
            if (!self.HasBuff(SeamstressBuffs.scissorRightBuff))
            {
                self.attackSpeed += .1f;
                self.moveSpeed += 1f;
            }
            if (self.inventory)
            {
                if (self.inventory.GetItemCount(DLC1Content.Items.EquipmentMagazineVoid) != 0)
                {
                    self.attackSpeed += .1f * self.inventory.GetItemCount(DLC1Content.Items.EquipmentMagazineVoid);
                    self.moveSpeed += 1f * self.inventory.GetItemCount(DLC1Content.Items.EquipmentMagazineVoid);
                }
            }
        }
        internal static void HUDSetup(HUD hud)
        {
            if (hud.targetBodyObject && hud.targetMaster.bodyPrefab == characterPrefab)
            {
                if (!hud.targetMaster.hasAuthority) return;

                Transform healthbarContainer = hud.transform.Find("MainContainer").Find("MainUIArea").Find("SpringCanvas").Find("BottomLeftCluster").Find("BarRoots").Find("LevelDisplayCluster");

                if (!hud.transform.Find("MainContainer").Find("MainUIArea").Find("SpringCanvas").Find("BottomLeftCluster").Find("ImpGauge"))
                {
                    GameObject impGauge = UnityEngine.Object.Instantiate(healthbarContainer.gameObject, hud.transform.Find("MainContainer").Find("MainUIArea").Find("SpringCanvas").Find("BottomLeftCluster"));
                    impGauge.name = "ImpGauge";

                    UnityEngine.Object.DestroyImmediate(impGauge.transform.GetChild(0).gameObject);
                    UnityEngine.Object.Destroy(impGauge.GetComponentInChildren<LevelText>());
                    UnityEngine.Object.Destroy(impGauge.GetComponentInChildren<ExpBar>());

                    ImpGauge impGaugeComponent = impGauge.AddComponent<ImpGauge>();
                    impGaugeComponent.targetHUD = hud;
                    impGaugeComponent.fillRectTransform = impGauge.transform.Find("ExpBarRoot").GetChild(0).GetChild(0).GetComponent<RectTransform>();

                    impGauge.transform.Find("LevelDisplayRoot").Find("ValueText").gameObject.SetActive(false);
                    impGauge.transform.Find("LevelDisplayRoot").Find("PrefixText").gameObject.SetActive(false);

                    impGauge.transform.Find("ExpBarRoot").GetChild(0).GetComponent<Image>().enabled = true;

                    impGauge.transform.Find("LevelDisplayRoot").GetComponent<RectTransform>().anchoredPosition = new Vector2(-12f, 0f);

                    RectTransform rect = impGauge.GetComponent<RectTransform>();
                    rect.anchorMax = new Vector2(1f, 1f);
                    rect.sizeDelta = new Vector2(1f, 1f);
                    rect.localPosition = new Vector2(740f, 430f);
                    rect.localScale = new Vector2(0.5f, 0.5f);
                }
                if (!hud.transform.Find("MainContainer").Find("MainUIArea").Find("CrosshairCanvas").Find("SeamstressCrosshair"))
                {
                    GameObject seamstressCrosshair = UnityEngine.Object.Instantiate(SeamstressCrosshair.seamstressCrosshair, hud.transform.Find("MainContainer").Find("MainUIArea").Find("CrosshairCanvas"));
                    seamstressCrosshair.name = "SeamstressCrosshair";
                    seamstressCrosshair.gameObject.GetComponent<HudElement>().targetBodyObject = hud.targetBodyObject;
                    seamstressCrosshair.gameObject.GetComponent<HudElement>().targetCharacterBody = hud.targetBodyObject.GetComponent<CharacterBody>();
                }
            }
        }
    }
}