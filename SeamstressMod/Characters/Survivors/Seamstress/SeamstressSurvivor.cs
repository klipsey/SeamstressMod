﻿using BepInEx.Configuration;
using SeamstressMod.Modules;
using SeamstressMod.Modules.Characters;
using SeamstressMod.Survivors.Seamstress.Components;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace SeamstressMod.Survivors.Seamstress
{
    public class SeamstressSurvivor : SurvivorBase<SeamstressSurvivor>
    {
        //todo guide
        //used to load the assetbundle for this character. must be unique
        public override string assetBundleName => "henryassets"; //if you do not change this, you are giving permission to deprecate the mod

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
        
        public override BodyInfo bodyInfo => new BodyInfo
        {
            bodyName = bodyName,
            bodyNameToken = SEAMSTRESS_PREFIX + "NAME",
            subtitleNameToken = SEAMSTRESS_PREFIX + "SUBTITLE",

            characterPortrait = assetBundle.LoadAsset<Texture>("texHenryIcon"),
            bodyColor = Color.white,

            crosshair = Assets.LoadCrosshair("Standard"),
            podPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/SurvivorPod"),

            maxHealth = 200f,
            healthRegen = 1f,
            armor = 0f,
            damage = 10f,

            damageGrowth = 0f,
            healthGrowth = 48f,

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

        public override void InitializeCharacter()
        {
            //uncomment if you have multiple characters
            //ConfigEntry<bool> characterEnabled = Config.CharacterEnableConfig("Survivors", "Henry");

            //if (!characterEnabled.Value)
            //    return;

            //need the character unlockable before you initialize the survivordef
            SeamstressUnlockables.Init();

            base.InitializeCharacter();

            SeamstressConfig.Init();
            SeamstressStates.Init();
            SeamstressTokens.Init();

            SeamstressAssets.Init(assetBundle);
            SeamstressBuffs.Init(assetBundle);

            InitializeEntityStateMachines();
            InitializeSkills();
            InitializeSkins();
            InitializeCharacterMaster();

            AdditionalBodySetup();

            AddHooks();
        }

        private void AdditionalBodySetup()
        {
            AddHitboxes();
            bodyPrefab.AddComponent<SeamstressController>();
            //bodyPrefab.AddComponent<HuntressTrackerComopnent>();
            //anything else here
        }

        public void AddHitboxes()
        {
            ChildLocator childLocator = bodyPrefab.GetComponentInChildren<ChildLocator>();

            //example of how to create a hitbox
            Transform hitboxTransform = childLocator.FindChild("SwordHitbox");
            Prefabs.SetupHitbox(characterModelObject, hitboxTransform, "Sword");
            hitboxTransform = childLocator.FindChild("SwordHitboxBig");
            Prefabs.SetupHitbox(characterModelObject, hitboxTransform, "SwordBig");

        }

        public override void InitializeEntityStateMachines() 
        {
            //clear existing state machines from your cloned body (probably commando)
            //omit all this if you want to just keep theirs
            Prefabs.ClearEntityStateMachines(bodyPrefab);

            //if you set up a custom main characterstate, set it up here
                //don't forget to register custom entitystates in your HenryStates.cs
            //the main "body" state machine has some special properties
            Prefabs.AddMainEntityStateMachine(bodyPrefab, "Body", typeof(EntityStates.GenericCharacterMain), typeof(EntityStates.SpawnTeleporterState));
            
            Prefabs.AddEntityStateMachine(bodyPrefab, "Weapon");
            Prefabs.AddEntityStateMachine(bodyPrefab, "Weapon2");
        }

        #region skills
        public override void InitializeSkills()
        {
            Skills.CreateSkillFamilies(bodyPrefab);
            AddPassiveSkill(bodyPrefab);
            AddPrimarySkills();
            AddSecondarySkills();
            AddUtiitySkills();
            AddSpecialSkills();

        }

        private void AddPassiveSkill(GameObject bodyPrefab)
        {
            SkillLocator skillLocator = bodyPrefab.GetComponent<SkillLocator>();
            skillLocator.passiveSkill.enabled = true;
            skillLocator.passiveSkill.skillNameToken = SeamstressSurvivor.SEAMSTRESS_PREFIX + "PASSIVE_NAME";
            skillLocator.passiveSkill.skillDescriptionToken = SeamstressSurvivor.SEAMSTRESS_PREFIX + "PASSIVE_DESCRIPTION";
            skillLocator.passiveSkill.icon = assetBundle.LoadAsset<Sprite>("texSpecialIcon");
        }

        //let's look at secondary before primary because it is simpler
        private void AddSecondarySkills()
        {
            //here is a basic skill def with all fields accounted for
            SkillDef weaveSkillDef = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "SeamstressWeave",
                skillNameToken = SEAMSTRESS_PREFIX + "SECONDARY_WEAVE_NAME",
                skillDescriptionToken = SEAMSTRESS_PREFIX + "SECONDARY_WEAVE_DESCRIPTION",
                keywordTokens = new string[] { Tokens.bleedKeyword, Tokens.butcheredKeyword, Tokens.slayerKeyword },
                skillIcon = assetBundle.LoadAsset<Sprite>("texSecondaryIcon"),

                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Weave)),
                activationStateMachineName = "Weapon",
                interruptPriority = EntityStates.InterruptPriority.Skill,

                baseRechargeInterval = 5f,
                baseMaxStock = 1,

                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,

                resetCooldownTimerOnUse = false,
                fullRestockOnAssign = true,
                dontAllowPastMaxStocks = false,
                beginSkillCooldownOnSkillEnd = false,
                mustKeyPress = false,

                isCombatSkill = true,
                canceledFromSprinting = false,
                cancelSprintingOnActivation = false,
                forceSprintDuringState = true,

            });

            Skills.AddSecondarySkills(bodyPrefab, weaveSkillDef);
        }

        private void AddPrimarySkills()
        {
            //the primary skill is created using a constructor for a typical primary
            //it is also a SteppedSkillDef. Custom Skilldefs are very useful for custom behaviors related to casting a skill. see ror2's different skilldefs for reference
            SteppedSkillDef trimSkillDef = Skills.CreateSkillDef<SteppedSkillDef>(new SkillDefInfo
                (
                    "SeamstressSlash",
                    SEAMSTRESS_PREFIX + "PRIMARY_TRIM_NAME",
                    SEAMSTRESS_PREFIX + "PRIMARY_TRIM_DESCRIPTION",
                    assetBundle.LoadAsset<Sprite>("texPrimaryIcon"),
                    new EntityStates.SerializableEntityStateType(typeof(SkillStates.Trim)),
                    "Weapon",
                    true,true,true
                ));
            //custom Skilldefs can have additional fields that you can set manually
            trimSkillDef.stepCount = 3;
            trimSkillDef.stepGraceDuration = 0.5f;

            Skills.AddPrimarySkills(bodyPrefab, trimSkillDef);
        }

        private void AddUtiitySkills()
        {
            //here's a skilldef of a typical movement skill. some fields are omitted and will just have default values
            SkillDef reapSkillDef = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "SeamstressReap",
                skillNameToken = SEAMSTRESS_PREFIX + "UTILITY_REAP_NAME",
                skillDescriptionToken = SEAMSTRESS_PREFIX + "UTILITY_REAP_DESCRIPTION",
                keywordTokens = new string[] { Tokens.healthCostKeyword, Tokens.frenzyKeyword, Tokens.butcheredKeyword },
                skillIcon = assetBundle.LoadAsset<Sprite>("texUtilityIcon"),

                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Reap)),
                activationStateMachineName = "Weapon2",
                interruptPriority = EntityStates.InterruptPriority.Skill,

                baseRechargeInterval = 8f,
                baseMaxStock = 1,

                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,

                resetCooldownTimerOnUse = false,
                fullRestockOnAssign = true,
                dontAllowPastMaxStocks = false,
                beginSkillCooldownOnSkillEnd = false,
                mustKeyPress = false,

                isCombatSkill = true,
                canceledFromSprinting = false,
                cancelSprintingOnActivation = true,
                forceSprintDuringState = false,
            });

            Skills.AddUtilitySkills(bodyPrefab, reapSkillDef);
        }

        private void AddSpecialSkills()
        {
            //a basic skill
            SkillDef sewSkillDef = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "SeamstressSew",
                skillNameToken = SEAMSTRESS_PREFIX + "SPECIAL_SEW_NAME",
                skillDescriptionToken = SEAMSTRESS_PREFIX + "SPECIAL_SEW_DESCRIPTION",
                keywordTokens = new string[] { Tokens.bleedKeyword, Tokens.butcheredKeyword },
                skillIcon = assetBundle.LoadAsset<Sprite>("texPrimaryIcon"),

                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Sew)),
                activationStateMachineName = "Weapon", //setting this to the "weapon2" EntityStateMachine allows us to cast this skill at the same time primary, which is set to the "weapon" EntityStateMachine
                interruptPriority = EntityStates.InterruptPriority.Skill,

                baseMaxStock = 1,
                baseRechargeInterval = 12f,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,

                resetCooldownTimerOnUse = false,
                fullRestockOnAssign = false,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = false,
            });

            Skills.AddSpecialSkills(bodyPrefab, sewSkillDef);
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
            
            //creating a new skindef as we did before
            SkinDef masterySkin = Modules.Skins.CreateSkinDef(SEAMSTRESS_PREFIX + "MASTERY_SKIN_NAME",
                assetBundle.LoadAsset<Sprite>("texMasteryAchievement"),
                defaultRendererinfos,
                prefabCharacterModel.gameObject,
                SeamstressUnlockables.masterySkinUnlockableDef);

            //adding the mesh replacements as above. 
            //if you don't want to replace the mesh (for example, you only want to replace the material), pass in null so the order is preserved
            masterySkin.meshReplacements = Modules.Skins.getMeshReplacements(assetBundle, defaultRendererinfos,
                "meshHenrySwordAlt",
                null,//no gun mesh replacement. use same gun mesh
                "meshHenryAlt");

            //masterySkin has a new set of RendererInfos (based on default rendererinfos)
            //you can simply access the RendererInfos' materials and set them to the new materials for your skin.
            masterySkin.rendererInfos[0].defaultMaterial = assetBundle.LoadMaterial("matHenryAlt");
            masterySkin.rendererInfos[1].defaultMaterial = assetBundle.LoadMaterial("matHenryAlt");
            masterySkin.rendererInfos[2].defaultMaterial = assetBundle.LoadMaterial("matHenryAlt");

            //here's a barebones example of using gameobjectactivations that could probably be streamlined or rewritten entirely, truthfully, but it works
            masterySkin.gameObjectActivations = new SkinDef.GameObjectActivation[]
            {
                new SkinDef.GameObjectActivation
                {
                    gameObject = childLocator.FindChildGameObject("GunModel"),
                    shouldActivate = false,
                }
            };
            //simply find an object on your child locator you want to activate/deactivate and set if you want to activate/deacitvate it with this skin

            skins.Add(masterySkin);
            
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
            R2API.RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, R2API.RecalculateStatsAPI.StatHookEventArgs args)
        {

            if (sender.HasBuff(SeamstressBuffs.armorBuff))
            {
                args.armorAdd += 20;
            }
            if(sender.HasBuff(SeamstressBuffs.bloodBath))
            {
                args.baseMoveSpeedAdd += 3;
                args.attackSpeedMultAdd += 0.3f;
            }
        }
    }
}