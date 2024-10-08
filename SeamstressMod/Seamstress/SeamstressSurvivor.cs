using SeamstressMod.Modules;
using SeamstressMod.Modules.Characters;
using RoR2;
using RoR2.Skills;
using System.Collections.Generic;
using UnityEngine;
using RoR2.UI;
using R2API;
using UnityEngine.Networking;
using SeamstressMod.Seamstress.Components;
using SeamstressMod.Seamstress.Content;
using SeamstressMod.Seamstress.SkillStates;
using EmotesAPI;

namespace SeamstressMod.Seamstress
{
    public class SeamstressSurvivor : SurvivorBase<SeamstressSurvivor>
    {
        public override string assetBundleName => "seamstressassets"; 
        public override string bodyName => "SeamstressBody"; 
        public override string masterName => "SeamstressMonsterMaster"; 
        public override string modelPrefabName => "mdlSeamstress"; 
        public override string displayPrefabName => "SeamstressDisplay"; 

        public const string SEAMSTRESS_PREFIX = SeamstressPlugin.DEVELOPER_PREFIX + "_SEAMSTRESS_";
        public override string survivorTokenPrefix => SEAMSTRESS_PREFIX;

        internal static GameObject characterPrefab;

        public static SkillDef snapBackSkillDef;

        public static SkillDef scepterFireScissor;

        public override BodyInfo bodyInfo => new BodyInfo
        {
            bodyName = bodyName,
            bodyNameToken = SEAMSTRESS_PREFIX + "NAME",
            subtitleNameToken = SEAMSTRESS_PREFIX + "SUBTITLE",

            characterPortrait = assetBundle.LoadAsset<Texture>("texSeamstressIcon"),
            bodyColor = new Color(155f / 255f, 55f / 255f, 55f / 255f),
            sortPosition = 7f,

            crosshair = Modules.CharacterAssets.LoadCrosshair("SimpleDot"),
            podPrefab = null,
            initialStateType = new EntityStates.SerializableEntityStateType(typeof(SkillStates.SeamstressSpawnState)),

            maxHealth = 160f,
            healthRegen = 1f,
            armor = 0f,
            damage = SeamstressStaticValues.baseDamage,

            damageGrowth = 0f,
            healthGrowth = 160f * 0.3f,

            jumpCount = 1,
        };

        public override CustomRendererInfo[] customRendererInfos => new CustomRendererInfo[]
        {
                new CustomRendererInfo
                {
                    childName = "Model",
                },
                new CustomRendererInfo
                {
                    childName = "ScissorLModel",
                },
                new CustomRendererInfo
                {
                    childName = "ScissorRModel",
                },
                new CustomRendererInfo
                {
                    childName = "CrownModel",
                },
                new CustomRendererInfo
                {
                    childName = "HeartModel",
                }

        };

        public override UnlockableDef characterUnlockableDef => SeamstressUnlockables.characterUnlockableDef;

        public override ItemDisplaysBase itemDisplays => new SeamstressItemDisplays();
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
            SeamstressConfig.Init();

            SeamstressUnlockables.Init();

            base.InitializeCharacter();

            SeamstressCrosshair.Init(assetBundle);

            DamageTypes.Init();

            SeamstressStates.Init();
            SeamstressTokens.Init();

            SeamstressAssets.Init(assetBundle);
            SeamstressBuffs.Init(assetBundle);

            //Dots.Init();

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

            bool tempAdd(CharacterBody body) => body.HasBuff(SeamstressBuffs.ManipulatedCd);
            bool tempAdd2(CharacterBody body) => body.HasBuff(SeamstressBuffs.Manipulated);
            float pee(CharacterBody body) => 2f * body.radius;

            TempVisualEffectAPI.AddTemporaryVisualEffect(SeamstressAssets.sewnCdEffect, pee, tempAdd);
            TempVisualEffectAPI.AddTemporaryVisualEffect(SeamstressAssets.sewnEffect, pee, tempAdd2);

            bodyPrefab.AddComponent<SeamstressController>();
            bodyPrefab.AddComponent<ScissorController>();
            bodyPrefab.AddComponent<NeedleController>();
            bodyPrefab.AddComponent<Tracker>();
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
            if (SeamstressPlugin.scepterInstalled) InitializeScepter();
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
                keywordTokens = new string[] { Tokens.detailsKeyword },
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
                keywordTokens = new string[] { Tokens.needlesKeyword },
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
            SteppedSkillDef trimSkillDef = Skills.CreateSkillDef<SteppedSkillDef>(new SkillDefInfo
                (
                    "Trim",
                    SEAMSTRESS_PREFIX + "PRIMARY_TRIM_NAME",
                    SEAMSTRESS_PREFIX + "PRIMARY_TRIM_DESCRIPTION",
                    assetBundle.LoadAsset<Sprite>("texFlurryIcon"),
                    new EntityStates.SerializableEntityStateType(typeof(SkillStates.Trim)),
                    "Weapon"
                ));
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
                keywordTokens = new string[] { },
                skillIcon = assetBundle.LoadAsset<Sprite>("texClipIcon"),

                activationState = new EntityStates.SerializableEntityStateType(typeof(Clip)),

                activationStateMachineName = "Weapon",
                interruptPriority = EntityStates.InterruptPriority.Skill,

                baseMaxStock = 1,
                baseRechargeInterval = 6f,
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

            TrackingSkillDef planarManipulation = Skills.CreateSkillDef<TrackingSkillDef>(new SkillDefInfo
            {
                skillName = "PlanarManipulation",
                skillNameToken = SEAMSTRESS_PREFIX + "SECONDARY_PLANMAN_NAME",
                skillDescriptionToken = SEAMSTRESS_PREFIX + "SECONDARY_PLANMAN_DESCRIPTION",
                keywordTokens = new string[] { Tokens.crushKeyword },
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

                baseRechargeInterval = 6f,
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
                keywordTokens = new string[] { Tokens.insatiableKeyword },
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
                keywordTokens = new string[] { Tokens.symbioticKeyword },
                skillIcon = assetBundle.LoadAsset<Sprite>("texSkewerIcon"),

                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.FireScissor)),
                activationStateMachineName = "Weapon",
                interruptPriority = EntityStates.InterruptPriority.Skill,

                baseRechargeInterval = 14f,
                baseMaxStock = 2,

                rechargeStock = 2,
                requiredStock = 1,
                stockToConsume = 1,

                resetCooldownTimerOnUse = false,
                fullRestockOnAssign = true,
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

        private void InitializeScepter()
        {
            scepterFireScissor = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "FireScepterSeamstress",
                skillNameToken = SEAMSTRESS_PREFIX + "SPECIAL_SCEPTER_NAME",
                skillDescriptionToken = SEAMSTRESS_PREFIX + "SPECIAL_SCEPTER_DESCRIPTION",
                keywordTokens = new string[] { Tokens.symbioticKeyword },
                skillIcon = assetBundle.LoadAsset<Sprite>("texSkewerScepterIcon"),

                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.FireScissorScepter)),
                activationStateMachineName = "Weapon",
                interruptPriority = EntityStates.InterruptPriority.Skill,

                baseRechargeInterval = 14f,
                baseMaxStock = 2,

                rechargeStock = 2,
                requiredStock = 1,
                stockToConsume = 1,

                resetCooldownTimerOnUse = false,
                fullRestockOnAssign = true,
                dontAllowPastMaxStocks = true,
                mustKeyPress = true,
                beginSkillCooldownOnSkillEnd = false,

                isCombatSkill = true,
                canceledFromSprinting = false,
                cancelSprintingOnActivation = true,
                forceSprintDuringState = false,
            });

            AncientScepter.AncientScepterItem.instance.RegisterScepterSkill(scepterFireScissor, bodyName, SkillSlot.Special, 0);
        }
        #endregion skills
        public static Material CreateMaterial(AssetBundle assetBundle, string materialName, float emission, Color emissionColor, float normalStrength)
        {
            if (!SeamstressAssets.commandoMat) SeamstressAssets.commandoMat = Resources.Load<GameObject>("Prefabs/CharacterBodies/CommandoBody").GetComponentInChildren<CharacterModel>().baseRendererInfos[0].defaultMaterial;

            Material mat = UnityEngine.Object.Instantiate<Material>(SeamstressAssets.commandoMat);
            Material tempMat = assetBundle.LoadAsset<Material>(materialName);

            if (!tempMat) return SeamstressAssets.commandoMat;

            mat.name = materialName;
            mat.SetColor("_Color", tempMat.GetColor("_Color"));
            mat.SetTexture("_MainTex", tempMat.GetTexture("_MainTex"));
            mat.SetColor("_EmColor", emissionColor);
            mat.SetFloat("_EmPower", emission);
            mat.SetTexture("_EmTex", tempMat.GetTexture("_EmissionMap"));
            mat.SetFloat("_NormalStrength", normalStrength);

            return mat;
        }

        public static Material CreateMaterial(AssetBundle assetBundle, string materialName)
        {
            return CreateMaterial(assetBundle, materialName, 0f);
        }

        public static Material CreateMaterial(AssetBundle assetBundle, string materialName, float emission)
        {
            return CreateMaterial(assetBundle, materialName, emission, Color.black);
        }

        public static Material CreateMaterial(AssetBundle assetBundle, string materialName, float emission, Color emissionColor)
        {
            return CreateMaterial(assetBundle, materialName, emission, emissionColor, 0f);
        }
        #region skins
        public override void InitializeSkins()
        {
            ModelSkinController skinController = prefabCharacterModel.gameObject.AddComponent<ModelSkinController>();
            ChildLocator childLocator = prefabCharacterModel.GetComponent<ChildLocator>();

            CharacterModel.RendererInfo[] defaultRendererinfos = prefabCharacterModel.baseRendererInfos;

            List<SkinDef> skins = new List<SkinDef>();

            #region DefaultSkin
            //this creates a SkinDef with all default fields
            SkinDef defaultSkin = SeamstressSkins.CreateSkinDef("DEFAULT_SKIN",
                assetBundle.LoadAsset<Sprite>("texMainSkin"),
                defaultRendererinfos,
                prefabCharacterModel.gameObject);

            //these are your Mesh Replacements. The order here is based on your CustomRendererInfos from earlier
            //pass in meshes as they are named in your assetbundle
            //currently not needed as with only 1 skin they will simply take the default meshes
            //uncomment this when you have another skin
            defaultSkin.meshReplacements = Modules.SeamstressSkins.getMeshReplacements(assetBundle, defaultRendererinfos,
                "meshSeamstress",
                "meshScissorL",
                "meshScissorR",
                "meshSeamstressCrown",
                "meshHeart");

            defaultSkin.rendererInfos[0].defaultMaterial = assetBundle.LoadAsset<Material>("matSeamstress");
            defaultSkin.rendererInfos[1].defaultMaterial = assetBundle.LoadAsset<Material>("matScissors");
            defaultSkin.rendererInfos[2].defaultMaterial = assetBundle.LoadAsset<Material>("matScissors");
            defaultSkin.rendererInfos[3].defaultMaterial = assetBundle.LoadAsset<Material>("matSeamstress");
            defaultSkin.rendererInfos[4].defaultMaterial = assetBundle.LoadAsset<Material>("matSeamstress");

            //add new skindef to our list of skindefs. this is what we'll be passing to the SkinController
            skins.Add(defaultSkin);
            #endregion
            //uncomment this when you have a mastery skin
            #region MasterySkin
            
            ////creating a new skindef as we did before
            SkinDef masterySkin = Modules.SeamstressSkins.CreateSkinDef(SEAMSTRESS_PREFIX + "MASTERY_SKIN_NAME",
                assetBundle.LoadAsset<Sprite>("texMonsoonBlue"),
                defaultRendererinfos,
                prefabCharacterModel.gameObject
                , SeamstressUnlockables.masterySkinUnlockableDef);

            ////adding the mesh replacements as above. 
            ////if you don't want to replace the mesh (for example, you only want to replace the material), pass in null so the order is preserved
            masterySkin.meshReplacements = Modules.SeamstressSkins.getMeshReplacements(assetBundle, defaultRendererinfos,
                "meshPrincess",
                "meshPrincessSwordL",
                "meshPrincessSwordR",
                "meshPrincessCrown",
                null);

            ////masterySkin has a new set of RendererInfos (based on default rendererinfos)
            ////you can simply access the RendererInfos' materials and set them to the new materials for your skin.
            masterySkin.rendererInfos[0].defaultMaterial = assetBundle.LoadAsset<Material>("matPrincessBlue");
            masterySkin.rendererInfos[1].defaultMaterial = assetBundle.LoadAsset<Material>("matPrincessSword");
            masterySkin.rendererInfos[2].defaultMaterial = assetBundle.LoadAsset<Material>("matPrincessSword");
            masterySkin.rendererInfos[3].defaultMaterial = assetBundle.LoadAsset<Material>("matPrincessBlueEmissions");


            ////here's a barebones example of using gameobjectactivations that could probably be streamlined or rewritten entirely, truthfully, but it works
            masterySkin.gameObjectActivations = new SkinDef.GameObjectActivation[]
            {
                new SkinDef.GameObjectActivation
                {
                   gameObject = childLocator.FindChildGameObject("HeartModel"),
                    shouldActivate = false,
                }
            };
            ////simply find an object on your child locator you want to activate/deactivate and set if you want to activate/deacitvate it with this skin
            ///
            SkinDef masterySkin2 = Modules.SeamstressSkins.CreateSkinDef(SEAMSTRESS_PREFIX + "MASTERY_SKIN_NAME2",
                assetBundle.LoadAsset<Sprite>("texMonsoonRed"),
                defaultRendererinfos,
                prefabCharacterModel.gameObject,
                SeamstressUnlockables.masterySkinUnlockableDef);

            masterySkin2.meshReplacements = Modules.SeamstressSkins.getMeshReplacements(assetBundle, defaultRendererinfos,
                "meshPrincess",
                "meshPrincessSwordL",
                "meshPrincessSwordR",
                "meshPrincessCrown",
                null);

            masterySkin2.rendererInfos[0].defaultMaterial = assetBundle.LoadAsset<Material>("matPrincessRed");
            masterySkin2.rendererInfos[1].defaultMaterial = assetBundle.LoadAsset<Material>("matPrincessSwordAlt");
            masterySkin2.rendererInfos[2].defaultMaterial = assetBundle.LoadAsset<Material>("matPrincessSwordAlt");
            masterySkin2.rendererInfos[3].defaultMaterial = assetBundle.LoadAsset<Material>("matPrincessRedEmissions");

            masterySkin2.gameObjectActivations = new SkinDef.GameObjectActivation[]
            {
                new SkinDef.GameObjectActivation
                {
                   gameObject = childLocator.FindChildGameObject("HeartModel"),
                    shouldActivate = false,
                }
            };
            SkinDef masterySkin3 = Modules.SeamstressSkins.CreateSkinDef(SEAMSTRESS_PREFIX + "MASTERY_SKIN_NAME3",
                assetBundle.LoadAsset<Sprite>("ravenIcon"),
                defaultRendererinfos,
                prefabCharacterModel.gameObject, SeamstressUnlockables.masteryTyphoonSkinUnlockableDef);

            masterySkin3.meshReplacements = Modules.SeamstressSkins.getMeshReplacements(assetBundle, defaultRendererinfos,
                "meshRaven",
                "meshShadowClawsL",
                "meshShadowClawsR",
                "meshRavenCrown",
                null);

            masterySkin3.rendererInfos[0].defaultMaterial = assetBundle.LoadAsset<Material>("matRaven");
            masterySkin3.rendererInfos[1].defaultMaterial = assetBundle.LoadAsset<Material>("matRavenShadowClaws");
            masterySkin3.rendererInfos[2].defaultMaterial = assetBundle.LoadAsset<Material>("matRavenShadowClaws");
            masterySkin3.rendererInfos[3].defaultMaterial = assetBundle.LoadAsset<Material>("matRaven");

            masterySkin3.gameObjectActivations = new SkinDef.GameObjectActivation[]
            {
                new SkinDef.GameObjectActivation
                {
                   gameObject = childLocator.FindChildGameObject("HeartModel"),
                    shouldActivate = false,
                }
            };
            SkinDef masterySkin4 = Modules.SeamstressSkins.CreateSkinDef(SEAMSTRESS_PREFIX + "MASTERY_SKIN_NAME4",
            assetBundle.LoadAsset<Sprite>("ravenIcon"),
            defaultRendererinfos,
            prefabCharacterModel.gameObject, SeamstressUnlockables.masteryTyphoonSkinUnlockableDef);

            masterySkin4.meshReplacements = Modules.SeamstressSkins.getMeshReplacements(assetBundle, defaultRendererinfos,
                "meshRavenAlt",
                "meshShadowClawsL",
                "meshShadowClawsR",
                "meshRavenCrownAlt", 
                null);

            masterySkin4.rendererInfos[0].defaultMaterial = assetBundle.LoadAsset<Material>("matRavenAlt");
            masterySkin4.rendererInfos[1].defaultMaterial = assetBundle.LoadAsset<Material>("matRavenShadowClaws");
            masterySkin4.rendererInfos[2].defaultMaterial = assetBundle.LoadAsset<Material>("matRavenShadowClaws");
            masterySkin4.rendererInfos[3].defaultMaterial = assetBundle.LoadAsset<Material>("matRavenAltEmission");

            masterySkin4.gameObjectActivations = new SkinDef.GameObjectActivation[]
            {
                new SkinDef.GameObjectActivation
                {
                   gameObject = childLocator.FindChildGameObject("HeartModel"),
                    shouldActivate = false,
                }
            };
            skins.Add(masterySkin);
            skins.Add(masterySkin2);
            skins.Add(masterySkin3);
            skins.Add(masterySkin4);

            
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
            On.RoR2.HealthComponent.Heal += HealthComponent_Heal;
            On.RoR2.CharacterModel.UpdateOverlays += CharacterModel_UpdateOverlays;
            On.RoR2.CharacterModel.UpdateOverlayStates += CharacterModel_UpdateOverlayStates;
            On.RoR2.HealthComponent.TakeDamageProcess += HealthComponent_TakeDamageProcess;
            On.RoR2.UI.LoadoutPanelController.Rebuild += LoadoutPanelController_Rebuild;
            On.RoR2.GenericSkill.SetBonusStockFromBody += GenericSkill_SetBonusStockFromBody;
            On.RoR2.Items.ImmuneToDebuffBehavior.OverrideDot += ImmuneToDebuffBehavior_OverrideDot;

            R2API.RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;

            if (SeamstressPlugin.emotesInstalled) Emotes();
        }
        private void CharacterModel_UpdateOverlays(On.RoR2.CharacterModel.orig_UpdateOverlays orig, CharacterModel self)
        {
            orig.Invoke(self);

            if (!self || !self.body || self.body.bodyIndex != BodyCatalog.FindBodyIndex("SeamstressBody"))
            {
                return;
            }

            SeamstressController s = self.body.GetComponent<SeamstressController>();

            AddOverlay(s.blue ? SeamstressAssets.insatiableOverlayMat2 : SeamstressAssets.insatiableOverlayMat, self.body.HasBuff(SeamstressBuffs.SeamstressInsatiableBuff) && s.hasStartedInsatiable == false);

            if (self.body.HasBuff(SeamstressBuffs.SeamstressInsatiableBuff) && s.hasStartedInsatiable == false)
            {
                TemporaryOverlayInstance temporaryOverlayInstance = TemporaryOverlayManager.AddOverlay(self.gameObject);
                temporaryOverlayInstance.duration = 0.6f;
                temporaryOverlayInstance.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0.4f);
                temporaryOverlayInstance.animateShaderAlpha = true;
                temporaryOverlayInstance.destroyComponentOnEnd = true;
                temporaryOverlayInstance.originalMaterial = s.blue ? SeamstressAssets.insatiableOverlayMat2 : SeamstressAssets.insatiableOverlayMat;
            }

            if (self.body.HasBuff(SeamstressBuffs.ParryStart))
            {
                TemporaryOverlayInstance temporaryOverlayInstance = TemporaryOverlayManager.AddOverlay(self.gameObject);
                temporaryOverlayInstance.duration = 0.4f;
                temporaryOverlayInstance.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                temporaryOverlayInstance.animateShaderAlpha = true;
                temporaryOverlayInstance.destroyComponentOnEnd = true;
                temporaryOverlayInstance.originalMaterial = SeamstressAssets.parryMat;
            }

            void AddOverlay(Material overlayMaterial, bool condition)
            {
                if (self.activeOverlayCount < CharacterModel.maxOverlays && condition)
                {
                    self.currentOverlays[self.activeOverlayCount++] = overlayMaterial;
                }
            }
        }

        private bool CharacterModel_UpdateOverlayStates(On.RoR2.CharacterModel.orig_UpdateOverlayStates orig, CharacterModel self)
        {
            var flag = orig.Invoke(self);

            if (!self || !self.body || self.body.bodyIndex != BodyCatalog.FindBodyIndex("SeamstressBody"))
            {
                return flag;
            }

            SeamstressController s = self.body.GetComponent<SeamstressController>();

            SetOverlayFlag(self.baseRendererInfos.Length, self.body.HasBuff(SeamstressBuffs.SeamstressInsatiableBuff) && s.hasStartedInsatiable == false);

            return self.oldOverlays != self.activeOverlays || flag;

            void SetOverlayFlag(int index, bool condition)
            {
                if (condition)
                {
                    self.activeOverlays |= 1 << index;
                }
            }
        }

        private bool ImmuneToDebuffBehavior_OverrideDot(On.RoR2.Items.ImmuneToDebuffBehavior.orig_OverrideDot orig, InflictDotInfo inflictDotInfo)
        {
            CharacterBody characterBody = inflictDotInfo.victimObject?.GetComponent<CharacterBody>();
            if (characterBody && characterBody.bodyIndex != BodyCatalog.FindBodyIndex("SeamstressBody") && inflictDotInfo.dotIndex == Dots.SeamstressSelfBleed)
            {
                return false;
            }
            else return orig(inflictDotInfo);
        }

        private static void Emotes()
        {
            On.RoR2.SurvivorCatalog.Init += (orig) =>
            {
                orig();
                var skele = SeamstressAssets.mainAssetBundle.LoadAsset<GameObject>("seamstress_emoteskeleton");
                CustomEmotesAPI.ImportArmature(SeamstressSurvivor.characterPrefab, skele);
            };
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
        private void HealthComponent_TakeDamageProcess(On.RoR2.HealthComponent.orig_TakeDamageProcess orig, HealthComponent self, DamageInfo damageInfo)
        {
            float currentBarrier = 0;
            if (NetworkServer.active && self.alive || !self.godMode || self.ospTimer <= 0f)
            {
                CharacterBody victimBody = self.body;
                if (victimBody && victimBody.bodyIndex == BodyCatalog.FindBodyIndex("SeamstressBody"))
                {
                    if (victimBody.HasBuff(SeamstressBuffs.ParryStart) && damageInfo.damage > 0)
                    {
                        victimBody.RemoveBuff(SeamstressBuffs.ParryStart);
                        if (!victimBody.HasBuff(SeamstressBuffs.ParrySuccess))
                        {
                            victimBody.AddBuff(SeamstressBuffs.ParrySuccess);
                        }
                        victimBody.AddTimedBuff(RoR2Content.Buffs.Immune, SeamstressStaticValues.parryWindow + 0.5f);
                        damageInfo.rejected = true;
                    }
                    else if (victimBody.HasBuff(SeamstressBuffs.SeamstressInsatiableBuff) && damageInfo.dotIndex != Dots.SeamstressSelfBleed)
                    {
                        SeamstressController seamCom = victimBody.gameObject.GetComponent<SeamstressController>();
                        if (seamCom && !victimBody.HasBuff(RoR2Content.Buffs.HiddenInvincibility))
                        {
                            if (damageInfo.damage >= self.combinedHealth && victimBody.skillLocator.utility.skillDef == snapBackSkillDef)
                            {
                                victimBody.skillLocator.utility.ExecuteIfReady();
                            }
                            damageInfo.rejected = true;
                        }
                    }
                    else if(victimBody.HasBuff(SeamstressBuffs.SeamstressInsatiableBuff))
                    {
                        currentBarrier = victimBody.healthComponent.barrier;
                        if(currentBarrier > 0f) victimBody.healthComponent.AddBarrier(-currentBarrier);
                    }
                }
            }

            orig.Invoke(self, damageInfo);

            if(NetworkServer.active)
            {
                if (self.body && self.body.bodyIndex == BodyCatalog.FindBodyIndex("SeamstressBody"))
                {
                    if (currentBarrier > 0) self.body.healthComponent.AddBarrier(currentBarrier);
                    self.body.RecalculateStats();
                }
            }
        }
        private float HealthComponent_Heal(On.RoR2.HealthComponent.orig_Heal orig, HealthComponent self, float amount, ProcChainMask procChainMask, bool nonRegen = true)
        {
            if (self && self.body.HasBuff(SeamstressBuffs.SeamstressInsatiableBuff) && self.body.bodyIndex == BodyCatalog.FindBodyIndex("SeamstressBody"))
            {
                amount *= SeamstressStaticValues.healConversion;
            }
            var res = orig(self, amount, procChainMask, nonRegen);
            if (self.body.bodyIndex == BodyCatalog.FindBodyIndex("SeamstressBody"))
            {
                if (self.body.GetComponent<SeamstressController>() && self.body.HasBuff(SeamstressBuffs.SeamstressInsatiableBuff))
                {
                    if (self.health >= self.fullHealth) self.AddBarrier(amount / SeamstressStaticValues.healConversion * (1 - SeamstressStaticValues.healConversion));
                    else self.AddBarrier((res / SeamstressStaticValues.healConversion * (1 - SeamstressStaticValues.healConversion)) / 2f);
                }
            }
            return res;
        }
        public void GenericSkill_SetBonusStockFromBody(On.RoR2.GenericSkill.orig_SetBonusStockFromBody orig, GenericSkill self, int newBonusStockFromBody)
        {
            if (self.skillDef.dontAllowPastMaxStocks && self.skillDef.skillNameToken == SEAMSTRESS_PREFIX + "SPECIAL_SCEPTER_NAME" || self.skillDef.skillNameToken == SEAMSTRESS_PREFIX + "SPECIAL_FIRE_NAME")
            {
                return;
            }
            else orig.Invoke(self, newBonusStockFromBody);
        }


        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, R2API.RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.bodyIndex == BodyCatalog.FindBodyIndex("SeamstressBody"))
            {
                SeamstressController seamstressController = sender.GetComponent<SeamstressController>();
                HealthComponent healthComponent = sender.GetComponent<HealthComponent>();
                SkillLocator skillLocator = sender.GetComponent<SkillLocator>();
                if (seamstressController && healthComponent && skillLocator)
                {
                    float healthMissing = (healthComponent.fullCombinedHealth * sender.cursePenalty) - (healthComponent.health + healthComponent.shield / 2f);
                    float fakeHealthMissing = healthComponent.fullHealth * 0.66f;
                    if (sender.HasBuff(SeamstressBuffs.SeamstressInsatiableBuff) && skillLocator.utility.skillNameToken == SEAMSTRESS_PREFIX + "UTILITY_PARRY_NAME") args.baseDamageAdd += fakeHealthMissing * SeamstressStaticValues.passiveScaling + healthMissing * SeamstressStaticValues.passiveScaling;
                    else args.baseDamageAdd += healthMissing * SeamstressStaticValues.passiveScaling;
                }
                if (sender.HasBuff(SeamstressBuffs.SeamstressInsatiableBuff))
                {
                    args.attackSpeedMultAdd += .2f;
                    args.baseMoveSpeedAdd += 2f;
                }
                if (!sender.HasBuff(SeamstressBuffs.ScissorLeftBuff))
                {
                    args.attackSpeedMultAdd += .1f;
                }
                if (!sender.HasBuff(SeamstressBuffs.ScissorRightBuff))
                {
                    args.attackSpeedMultAdd += .1f;
                }
                if (sender.inventory)
                {
                    if (sender.inventory.GetItemCount(DLC1Content.Items.EquipmentMagazineVoid) != 0)
                    {
                        args.attackSpeedMultAdd += .1f * sender.inventory.GetItemCount(DLC1Content.Items.EquipmentMagazineVoid);
                    }
                }
            }
        }
        internal static void HUDSetup(HUD hud)
        {
            if (hud.targetBodyObject && hud.targetMaster.bodyPrefab == characterPrefab)
            {
                if (!hud.targetMaster.hasAuthority) return;

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
