using BepInEx.Configuration;
using SeamstressMod.Modules;
using SeamstressMod.Modules.Characters;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using static RoR2.MasterSpawnSlotController;
using RoR2.UI;
using R2API;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using static UnityEngine.UIElements.UxmlAttributeDescription;
using UnityEngine.UI;
using SeamstressMod.SkillStates;

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
            podPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/SurvivorPod"),

            maxHealth = 160f,
            healthRegen = 1f,
            armor = 0f,
            damage = 10f,

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

            
            AddHooks();
        }

        private void AdditionalBodySetup()
        {
            AddHitboxes();
            bool tempAdd(CharacterBody body) => body.HasBuff(SeamstressBuffs.stitchSetup);
            bodyPrefab.AddComponent<SeamstressController>();
            TempVisualEffectAPI.AddTemporaryVisualEffect(SeamstressAssets.stitchTempEffectPrefab, tempAdd);
            //bodyPrefab.AddComponent<HuntressTrackerComopnent>();
            //anything else here
        }
        public void AddHitboxes()
        {
            ChildLocator childLocator = characterModelObject.GetComponent<ChildLocator>();

            //example of how to create a hitbox
            Transform hitBoxTransform = childLocator.FindChild("SwordHitbox");
            Prefabs.SetupHitBoxGroup(characterModelObject, "Sword", hitBoxTransform);
            hitBoxTransform = childLocator.FindChild("SwordHitboxBig");
            Prefabs.SetupHitBoxGroup(characterModelObject, "SwordBig", hitBoxTransform);
            hitBoxTransform = childLocator.FindChild("SewHitbox");
            Prefabs.SetupHitBoxGroup(characterModelObject, "Sew", hitBoxTransform);
            hitBoxTransform = childLocator.FindChild("WeaveHitbox");
            Prefabs.SetupHitBoxGroup(characterModelObject, "Weave", hitBoxTransform);
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
            AddUtilitySkills();
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

        private void AddPrimarySkills()
        {
            //the primary skill is created using a constructor for a typical primary
            //it is also a SteppedSkillDef. Custom Skilldefs are very useful for custom behaviors related to casting a skill. see ror2's different skilldefs for reference
            SteppedSkillDef trimSkillDef = Skills.CreateSkillDef<SteppedSkillDef>(new SkillDefInfo
                (
                    "Trim",
                    SEAMSTRESS_PREFIX + "PRIMARY_TRIM_NAME",
                    SEAMSTRESS_PREFIX + "PRIMARY_TRIM_DESCRIPTION",
                    assetBundle.LoadAsset<Sprite>("texPrimaryIcon"),
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
                    assetBundle.LoadAsset<Sprite>("texPrimaryIcon"),
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
            SkillDef weaveSkillDef = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "Weave",
                skillNameToken = SEAMSTRESS_PREFIX + "SECONDARY_WEAVE_NAME",
                skillDescriptionToken = SEAMSTRESS_PREFIX + "SECONDARY_WEAVE_DESCRIPTION",
                keywordTokens = new string[] { Tokens.stitchKeyword },
                skillIcon = assetBundle.LoadAsset<Sprite>("texUtilityIcon"),

                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.WeaveLeap)),
                activationStateMachineName = "Weapon",
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,

                baseRechargeInterval = 7f,
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
                forceSprintDuringState = true,

            });

            Skills.AddSecondarySkills(bodyPrefab, weaveSkillDef);
        }

        private void AddUtilitySkills()
        {
            SkillDef blinkSeamstressSkillDef = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "BlinkSeamstress",
                skillNameToken = SEAMSTRESS_PREFIX + "UTILITY_BLINK_NAME",
                skillDescriptionToken = SEAMSTRESS_PREFIX + "UTILITY_BLINK_DESCRIPTION",
                keywordTokens = new string[] { Tokens.butcheredKeyword },
                skillIcon = assetBundle.LoadAsset<Sprite>("texUtilityIcon"),

                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.BlinkSeamstress)),
                activationStateMachineName = "Weapon",
                interruptPriority = EntityStates.InterruptPriority.Skill,

                baseRechargeInterval = 10f,
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
                cancelSprintingOnActivation = false,
                forceSprintDuringState = false,

            });

            Skills.AddUtilitySkills(bodyPrefab, blinkSeamstressSkillDef);

            GenericSkill reapRecast = Skills.CreateGenericSkillWithSkillFamily(bodyPrefab, "reapRecast", true);

            SkillDef reapRecastSkillDef = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "Expunge",
                skillNameToken = SeamstressSurvivor.SEAMSTRESS_PREFIX + "UTILITY_EXPUNGE_NAME",
                skillDescriptionToken = SeamstressSurvivor.SEAMSTRESS_PREFIX + "UTILITY_EXPUNGE_DESCRIPTION",
                keywordTokens = new string[] { Tokens.stitchKeyword },
                skillIcon = assetBundle.LoadAsset<Sprite>("texBoxingGlovesIcon"),

                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.ReapRecast)),
                activationStateMachineName = "Weapon2",
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,

                baseRechargeInterval = 1f,
                baseMaxStock = 1,

                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,

                resetCooldownTimerOnUse = false,
                fullRestockOnAssign = true,
                dontAllowPastMaxStocks = true,
                beginSkillCooldownOnSkillEnd = false,
                mustKeyPress = true,

                isCombatSkill = true,
                canceledFromSprinting = false,
                cancelSprintingOnActivation = false,
                forceSprintDuringState = false,
            });

            Skills.AddSkillsToFamily(reapRecast.skillFamily, reapRecastSkillDef);
        }

        private void AddSpecialSkills()
        {
            //a basic skill
            SkillDef sewSkillDef = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "Sew",
                skillNameToken = SEAMSTRESS_PREFIX + "SPECIAL_SEW_NAME",
                skillDescriptionToken = SEAMSTRESS_PREFIX + "SPECIAL_SEW_DESCRIPTION",
                keywordTokens = new string[] { Tokens.needleKeyword, Tokens.stitchKeyword },
                skillIcon = assetBundle.LoadAsset<Sprite>("texSpecialIcon"),

                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Sew)),
                //setting this to the "weapon2" EntityStateMachine allows us to cast this skill at the same time primary, which is set to the "weapon" EntityStateMachine
                activationStateMachineName = "Weapon",
                interruptPriority = EntityStates.InterruptPriority.Skill,

                baseMaxStock = SeamstressStaticValues.maxNeedleAmount,
                baseRechargeInterval = SeamstressStaticValues.needleGainInterval,
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
            R2API.RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
            On.RoR2.HealthComponent.Heal += new On.RoR2.HealthComponent.hook_Heal(HealthComponent_Heal);
            On.RoR2.CharacterModel.UpdateOverlays += new On.RoR2.CharacterModel.hook_UpdateOverlays(CharacterModel_UpdateOverlays);
        }
        private void CharacterModel_UpdateOverlays(On.RoR2.CharacterModel.orig_UpdateOverlays orig, CharacterModel self)
        {
            orig.Invoke(self);
            if (!self || !self.body || self.body.baseNameToken != "KENKO_SEAMSTRESS_NAME")
            {
                return;
            }
            SeamstressController s = self.body.GetComponent<SeamstressController>();
            if (self.body.HasBuff(SeamstressBuffs.butchered) && s.fuckYou == false)
            {
                TemporaryOverlay temporaryOverlay = self.gameObject.AddComponent<TemporaryOverlay>();
                temporaryOverlay.duration = SeamstressStaticValues.butcheredDuration;
                temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0.4f);
                temporaryOverlay.animateShaderAlpha = true;
                temporaryOverlay.destroyComponentOnEnd = true;
                temporaryOverlay.originalMaterial = SeamstressAssets.butcheredOverlayMat;
                temporaryOverlay.AddToCharacerModel(self);
                s.fuckYou = true;
            }
        }
        //calculate expunge healing
        private float HealthComponent_Heal(On.RoR2.HealthComponent.orig_Heal orig, HealthComponent self, float amount, ProcChainMask procChainMask, bool nonRegen = true)
        {
            if (self.body.HasBuff(SeamstressBuffs.butchered) && self.body.baseNameToken == "KENKO_SEAMSTRESS_NAME")
            {
                amount *= SeamstressStaticValues.healConversion;
            }
            var res = orig(self, amount, procChainMask, nonRegen);
            SeamstressController s = self.body.GetComponent<SeamstressController>();
            if (self.body.TryGetComponent<SeamstressController>(out s) && self.body.HasBuff(SeamstressBuffs.butchered))
            {
                s.ButcheredConversionCalc((res / SeamstressStaticValues.healConversion) * 1 - SeamstressStaticValues.healConversion);
            }
            return res;
        }
        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, R2API.RecalculateStatsAPI.StatHookEventArgs args)
        {
            if(sender.HasBuff(SeamstressBuffs.butchered))
            {
                args.baseMoveSpeedAdd += 3;
            }
        }
    }
}