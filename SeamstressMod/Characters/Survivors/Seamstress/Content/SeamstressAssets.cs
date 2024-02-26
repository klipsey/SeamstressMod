using RoR2;
using UnityEngine;
using SeamstressMod.Modules;
using RoR2.Projectile;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using R2API;
using RoR2.UI;
using RoR2.Skills;
using UnityEngine.Events;
using HarmonyLib;

namespace SeamstressMod.Survivors.Seamstress
{
    public static class SeamstressAssets
    {
        //effects
        internal static GameObject parrySlashEffect;

        internal static GameObject flurryCharge;

        internal static GameObject clipSlashEffect;

        internal static GameObject expungeSlashEffect;

        internal static GameObject expungeSlashEffect2;

        internal static GameObject expungeSlashEffect3;

        internal static GameObject clipSwingEffect;

        internal static GameObject scissorsSwingEffect;

        internal static GameObject blinkPrefab;

        internal static GameObject blinkDestinationPrefab;

        internal static GameObject sewEffect;

        internal static GameObject clawsEffect;
        //internal static GameObject scissorsComboSwingEffect;

        internal static GameObject scissorsComboSwingEffect;

        internal static GameObject scissorsHitImpactEffect;

        internal static GameObject needleGhost;

        internal static GameObject impDash;

        internal static GameObject smallBlinkPrefab;

        internal static GameObject expungeEffect;

        internal static GameObject reapEndEffect;

        internal static GameObject stitchConsumeEffectPrefab;
        //Materials
        internal static Material destealthMaterial;

        internal static Material butcheredOverlayMat;

        internal static Material parryMat;
        //particle overlay effects
        internal static GameObject stitchEffect;
        //networked hit sounds
        internal static NetworkSoundEventDef scissorsHitSoundEvent;

        internal static NetworkSoundEventDef parrySuccessSoundEvent;
        //icons
        internal static Sprite primary;

        internal static Sprite primaryEmp;

        internal static Sprite weave;

        internal static Sprite utilityEmp;

        internal static Sprite secondaryDisabled;

        internal static Sprite special;

        internal static Sprite specialEmp;

        private static AssetBundle _assetBundle;
        //projectiles
        internal static GameObject needlePrefab;

        internal static GameObject needleButcheredPrefab;

        internal static GameObject scissorRPrefab;

        internal static GameObject scissorRButcheredPrefab;

        internal static GameObject scissorLPrefab;

        internal static GameObject scissorLButcheredPrefab;

        //extra
        //public static SkillDef weaveRecastSkillDef;

        public static SkillDef lockOutSkillDef;
        public static void Init(AssetBundle assetBundle)
        {

            _assetBundle = assetBundle;

            scissorsHitSoundEvent = Content.CreateAndAddNetworkSoundEventDef("Play_bandit2_m2_impact");

            parrySuccessSoundEvent = Content.CreateAndAddNetworkSoundEventDef("Play_voidman_m2_explode");

            CreateEffects();

            CreateProjectiles();
            #region scrapped
            /*
            weaveRecastSkillDef = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "WeaveDash",
                skillNameToken = SeamstressSurvivor.SEAMSTRESS_PREFIX + "SECONDARY_WEAVE_NAME",
                skillDescriptionToken = SeamstressSurvivor.SEAMSTRESS_PREFIX + "SECONDARY_WEAVE_DESCRIPTION",
                keywordTokens = new string[] { Tokens.stitchKeyword },
                skillIcon = SeamstressAssets.weave,

                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Weave)),
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

                isCombatSkill = false,
                canceledFromSprinting = false,
                cancelSprintingOnActivation = false,
                forceSprintDuringState = true,
            });
            */
            #endregion
            lockOutSkillDef = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "LockOut",
                skillNameToken = "Exhausted",
                skillDescriptionToken = "Recharging",
                keywordTokens = new string[] { },
                skillIcon = secondaryDisabled,

                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Exhaustion)),
                activationStateMachineName = "Weapon2",
                interruptPriority = EntityStates.InterruptPriority.Any,

                baseRechargeInterval = 0f,
                baseMaxStock = 10,

                rechargeStock = 0,
                requiredStock = 999999999,
                stockToConsume = 0,

                resetCooldownTimerOnUse = false,
                fullRestockOnAssign = false,
                dontAllowPastMaxStocks = false,
                beginSkillCooldownOnSkillEnd = false,
                mustKeyPress = true,

                isCombatSkill = false,
                canceledFromSprinting = false,
                cancelSprintingOnActivation = false,
                forceSprintDuringState = false,
            });
        }


        #region effects
        private static void CreateEffects()
        {
            Color theRed = new Color(155f / 255f, 55f / 255f, 55f / 255f);
            primary = _assetBundle.LoadAsset<Sprite>("texPrimaryIcon");
            weave = _assetBundle.LoadAsset<Sprite>("texSecondaryIcon");
            special = _assetBundle.LoadAsset<Sprite>("texSpecialIcon");

            primaryEmp = _assetBundle.LoadAsset<Sprite>("texStingerIcon");
            secondaryDisabled = _assetBundle.LoadAsset<Sprite>("texBazookaOutIcon");
            utilityEmp = _assetBundle.LoadAsset<Sprite>("texPistolIcon");
            specialEmp = _assetBundle.LoadAsset<Sprite>("texScepterSpecialIcon");

            stitchEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/BleedEffect.prefab").WaitForCompletion().InstantiateClone("StitchEffect");
            stitchEffect.AddComponent<NetworkIdentity>();
            stitchEffect.transform.GetChild(1).gameObject.GetComponent<ParticleSystemRenderer>().material.SetColor("_EmissionColor", new Color(84f / 255f, 0f / 255f, 11f / 255f));

            clawsEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Imp/WIPImpEffect.prefab").WaitForCompletion().InstantiateClone("ClawsEffect");
            clawsEffect.AddComponent<NetworkIdentity>();
            var claws = clawsEffect.transform.GetChild(0).gameObject.GetComponent<ParticleSystem>().main;
            claws.startLifetimeMultiplier = 0.5f;
            #region scrapped
            /*
            Material material = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/DeathMark/matDeathMarkSkull.mat").WaitForCompletion()); 
            stitchTempEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/MercExposeEffect.prefab").WaitForCompletion().InstantiateClone("StitchEffectPrefab");
            stitchTempEffectPrefab.AddComponent<NetworkIdentity>();
            stitchTempEffectPrefab.transform.GetChild(0).GetChild(0).gameObject.GetComponent<ParticleSystemRenderer>().material = material;
            stitchTempEffectPrefab.transform.GetChild(0).GetChild(1).gameObject.GetComponent<ParticleSystemRenderer>().material.SetColor("_TintColor", new Color(84f / 255f, 0f / 255f, 11f / 255f));


            material = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpSwipe.mat").WaitForCompletion());
            stitchConsumeEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/MercExposeConsumeEffect.prefab").WaitForCompletion().InstantiateClone("StitchConsumeEffect");
            stitchConsumeEffectPrefab.AddComponent<NetworkIdentity>();
            stitchConsumeEffectPrefab.transform.GetChild(0).GetChild(0).gameObject.GetComponent<ParticleSystemRenderer>().material = material;
            stitchConsumeEffectPrefab.transform.GetChild(0).GetChild(1).gameObject.GetComponent<ParticleSystemRenderer>().material.SetColor("_TintColor", new Color(84f / 255f, 0f / 255f, 11f / 255f));
            stitchConsumeEffectPrefab.transform.GetChild(0).GetChild(2).gameObject.GetComponent<ParticleSystemRenderer>().material.SetColor("_TintColor", theRed);
            stitchConsumeEffectPrefab.GetComponent<EffectComponent>().soundName = "Play_imp_overlord_teleport_end";
            Modules.Content.CreateAndAddEffectDef(stitchConsumeEffectPrefab);
            */
            #endregion
            butcheredOverlayMat = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/VoidSurvivor/matVoidSurvivorCorruptOverlay.mat").WaitForCompletion();

            destealthMaterial = Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpBossDissolve.mat").WaitForCompletion();

            parryMat = Addressables.LoadAssetAsync<Material>("RoR2/Base/CritOnUse/matFullCrit.mat").WaitForCompletion();

            blinkPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ImpBoss/ImpBossBlink.prefab").WaitForCompletion().InstantiateClone("BlinkStart");
            blinkPrefab.AddComponent<NetworkIdentity>();
            blinkPrefab.GetComponent<EffectComponent>().applyScale = true;
            blinkPrefab.transform.GetChild(0).localScale = Vector3.one * 0.2f;
            blinkPrefab.transform.GetChild(1).localScale = Vector3.one * 0.2f;
            Modules.Content.CreateAndAddEffectDef(blinkPrefab);

            smallBlinkPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Imp/ImpBlinkEffect.prefab").WaitForCompletion().InstantiateClone("BlinkSmall");
            smallBlinkPrefab.AddComponent<NetworkIdentity>();
            Modules.Content.CreateAndAddEffectDef(smallBlinkPrefab);

            blinkDestinationPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Imp/ImpBossBlinkDestination.prefab").WaitForCompletion().InstantiateClone("BlinkEnd");
            blinkDestinationPrefab.AddComponent<NetworkIdentity>();
            blinkDestinationPrefab.transform.GetChild(0).localScale = Vector3.one * 0.2f;
            blinkDestinationPrefab.transform.GetChild(1).localScale = Vector3.one * 0.2f;

            scissorsSwingEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/MercSwordFinisherSlash.prefab").WaitForCompletion().InstantiateClone("ScissorSwing");
            scissorsSwingEffect.AddComponent<NetworkIdentity>();
            scissorsSwingEffect.transform.GetChild(0).gameObject.SetActive(false);
            scissorsSwingEffect.transform.GetChild(1).gameObject.GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpSwipe.mat").WaitForCompletion();//Assets.LoadEffect("HenrySwordSwingEffect", true);
            scissorsSwingEffect.transform.GetChild(1).localScale = Vector3.one;
            var fard = scissorsSwingEffect.transform.GetChild(0).gameObject.GetComponent<ParticleSystem>().main;
            fard.startLifetimeMultiplier = 1.8f;

            scissorsComboSwingEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/MercSwordFinisherSlash.prefab").WaitForCompletion().InstantiateClone("ScissorSwing3");
            scissorsComboSwingEffect.AddComponent<NetworkIdentity>();
            scissorsComboSwingEffect.transform.GetChild(0).gameObject.SetActive(false);
            scissorsComboSwingEffect.transform.GetChild(1).gameObject.GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpSwipe.mat").WaitForCompletion();
            scissorsComboSwingEffect.transform.GetChild(1).localScale = new Vector3(1f, 1.5f, 1.5f);
            UnityEngine.Object.Destroy(scissorsComboSwingEffect.GetComponent<EffectComponent>());

            clipSlashEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/MercSwordFinisherSlash.prefab").WaitForCompletion().InstantiateClone("ClipSwing");
            clipSlashEffect.AddComponent<NetworkIdentity>();
            clipSlashEffect.transform.GetChild(0).gameObject.SetActive(false);
            Material material = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpSwipe.mat").WaitForCompletion());
            clipSlashEffect.transform.GetChild(1).gameObject.GetComponent<ParticleSystemRenderer>().material = material;
            clipSlashEffect.transform.GetChild(1).localScale = new Vector3(0.5f, 1f, 0.5f);
            fard = clipSlashEffect.transform.GetChild(0).gameObject.GetComponent<ParticleSystem>().main;
            fard.startLifetimeMultiplier = 1.8f;

            parrySlashEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/MercSwordSlash.prefab").WaitForCompletion().InstantiateClone("ParrySlash");
            parrySlashEffect.AddComponent<NetworkIdentity>();
            parrySlashEffect.transform.GetChild(0).gameObject.GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpSwipe.mat").WaitForCompletion();
            parrySlashEffect.transform.GetChild(0).localScale = Vector3.one * 2f;
            fard = parrySlashEffect.transform.GetChild(0).gameObject.GetComponent<ParticleSystem>().main;
            fard.startLifetimeMultiplier = 0.6f;
            SeamstressPlugin.Destroy(parrySlashEffect.GetComponent<EffectComponent>());

            flurryCharge = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidSurvivor/VoidSurvivorChargeCrabCannon.prefab").WaitForCompletion().InstantiateClone("FlurryCharge");
            flurryCharge.AddComponent<NetworkIdentity>();
            flurryCharge.transform.GetChild(2).gameObject.SetActive(false);

            expungeEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/LunarSkillReplacements/LunarDetonatorConsume.prefab").WaitForCompletion().InstantiateClone("ExpungeEffect");
            expungeEffect.AddComponent<NetworkIdentity>();
            material = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Merc/matMercExposedBackdrop.mat").WaitForCompletion());
            material.SetColor("_TintColor", new Color(84f / 255f, 0f / 255f, 11f / 255f));
            expungeEffect.transform.GetChild(0).gameObject.GetComponent<ParticleSystemRenderer>().material = material;
            expungeEffect.transform.GetChild(1).gameObject.SetActive(false);
            expungeEffect.transform.GetChild(2).gameObject.GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpSwipe.mat").WaitForCompletion();
            expungeEffect.transform.GetChild(3).gameObject.GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpSwipe.mat").WaitForCompletion();
            expungeEffect.transform.GetChild(3).gameObject.transform.localScale = new Vector3(.1f, .1f, .1f);
            expungeEffect.transform.GetChild(4).gameObject.SetActive(false);
            material = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpSlashImpact.mat").WaitForCompletion());
            expungeEffect.transform.GetChild(5).gameObject.GetComponent<ParticleSystemRenderer>().material = material;
            expungeEffect.transform.GetChild(5).gameObject.transform.localScale = new Vector3(.1f, .1f, .1f);
            expungeEffect.transform.GetChild(6).gameObject.SetActive(false);
            Content.CreateAndAddEffectDef(expungeEffect);

            expungeSlashEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/MercSwordSlashWhirlwind.prefab").WaitForCompletion().InstantiateClone("ExpungeSlash");
            expungeSlashEffect.AddComponent<NetworkIdentity>();
            expungeSlashEffect.transform.GetChild(0).gameObject.GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpSwipe.mat").WaitForCompletion();
            expungeSlashEffect.transform.GetChild(0).localRotation = new Quaternion(0f, 90f, 90f, expungeSlashEffect.transform.GetChild(0).localRotation.w);
            expungeSlashEffect.transform.GetChild(0).localScale = Vector3.one * 2.5f;
            fard = expungeSlashEffect.transform.GetChild(0).gameObject.GetComponent<ParticleSystem>().main;
            fard.startLifetimeMultiplier = 0.6f;
            SeamstressPlugin.Destroy(expungeSlashEffect.GetComponent<EffectComponent>());

            expungeSlashEffect2 = expungeSlashEffect.InstantiateClone("ExpungeSlash2");
            expungeSlashEffect.transform.GetChild(0).localRotation = new Quaternion(0f, 180f, 180f, expungeSlashEffect.transform.GetChild(0).localRotation.w);

            expungeSlashEffect3 = expungeSlashEffect.InstantiateClone("ExpungeSlash3");
            expungeSlashEffect.transform.GetChild(0).localRotation = new Quaternion(90f, 180f, 180f, expungeSlashEffect.transform.GetChild(0).localRotation.w);

            //final hit
            /*
            scissorsComboSwingEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/MercSwordFinisherSlash.prefab").WaitForCompletion().InstantiateClone("ScissorSwing4");
            scissorsComboSwingEffect.AddComponent<NetworkIdentity>();
            scissorsComboSwingEffect.transform.GetChild(0).gameObject.SetActive(false);
            //ParticleSystem.MainModule main2 = scissorsComboSwingEffect.transform.GetChild(1).gameObject.GetComponent<ParticleSystem>().main;
            //main2.startLifetimeMultiplier = 0.1f;
            scissorsComboSwingEffect.transform.GetChild(1).localScale = Vector3.one * 1.0f;
            UnityEngine.Object.Destroy(scissorsComboSwingEffect.GetComponent<EffectComponent>());
            */

            //final hit 

            scissorsHitImpactEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/OmniImpactVFXSlashMerc.prefab").WaitForCompletion().InstantiateClone("ScissorImpact", false);
            scissorsHitImpactEffect.AddComponent<NetworkIdentity>();
            scissorsHitImpactEffect.GetComponent<OmniEffect>().enabled = false;
            material = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Merc/matOmniHitspark3Merc.mat").WaitForCompletion());
            material.SetColor("_TintColor", Color.red);
            scissorsHitImpactEffect.transform.GetChild(1).gameObject.GetComponent<ParticleSystemRenderer>().material = material;
            scissorsHitImpactEffect.transform.GetChild(2).localScale = Vector3.one * 1.5f;
            scissorsHitImpactEffect.transform.GetChild(2).gameObject.GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/VoidSurvivor/matVoidSurvivorBlasterFireCorrupted.mat").WaitForCompletion();
            material = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Merc/matOmniRadialSlash1Merc.mat").WaitForCompletion());
            material.SetColor("_TintColor", Color.red);
            scissorsHitImpactEffect.transform.GetChild(5).gameObject.GetComponent<ParticleSystemRenderer>().material = material;
            scissorsHitImpactEffect.transform.GetChild(4).localScale = Vector3.one * 3f;
            scissorsHitImpactEffect.transform.GetChild(4).gameObject.GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpDust.mat").WaitForCompletion();
            scissorsHitImpactEffect.transform.GetChild(6).GetChild(0).gameObject.GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/Common/Void/matOmniHitspark1Void.mat").WaitForCompletion();
            scissorsHitImpactEffect.transform.GetChild(6).gameObject.GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/Common/Void/matOmniHitspark2Void.mat").WaitForCompletion();
            scissorsHitImpactEffect.transform.GetChild(1).localScale = Vector3.one * 1.5f;
            scissorsHitImpactEffect.transform.GetChild(1).gameObject.SetActive(true);
            scissorsHitImpactEffect.transform.GetChild(2).gameObject.SetActive(true);
            scissorsHitImpactEffect.transform.GetChild(3).gameObject.SetActive(true);
            scissorsHitImpactEffect.transform.GetChild(4).gameObject.SetActive(true);
            scissorsHitImpactEffect.transform.GetChild(5).gameObject.SetActive(true);
            scissorsHitImpactEffect.transform.GetChild(6).gameObject.SetActive(true);
            scissorsHitImpactEffect.transform.GetChild(6).GetChild(0).gameObject.SetActive(true);
            scissorsHitImpactEffect.transform.GetChild(6).transform.localScale = new Vector3(1f, 1f, 3f);
            scissorsHitImpactEffect.transform.localScale = Vector3.one * 1.5f;
            Modules.Content.CreateAndAddEffectDef(scissorsHitImpactEffect);

            sewEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/BleedOnHitAndExplode/BleedOnHitAndExplode_Impact.prefab").WaitForCompletion().InstantiateClone("SewSplosion");
            sewEffect.AddComponent<NetworkIdentity>();
            material = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpSwipe.mat").WaitForCompletion());
            sewEffect.transform.GetChild(0).localScale = Vector3.one * 1f;
            sewEffect.transform.GetChild(0).GetComponent<ParticleSystemRenderer>().material = material;
            sewEffect.transform.GetChild(1).localScale = Vector3.one * 1f;
            sewEffect.transform.GetChild(1).GetComponent<ParticleSystemRenderer>().material = material;
            sewEffect.transform.GetChild(2).localScale = Vector3.one * 1f;
            sewEffect.transform.GetChild(2).GetComponent<ParticleSystemRenderer>().material = material;

            impDash = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/MercAssaulterEffect.prefab").WaitForCompletion().InstantiateClone("ImpDash");
            impDash.AddComponent<NetworkIdentity>();
            material = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpSwipe.mat").WaitForCompletion());
            material.SetColor("_TintColor", theRed);
            impDash.transform.GetChild(5).gameObject.SetActive(false);
            impDash.transform.GetChild(6).gameObject.SetActive(false);
            impDash.transform.GetChild(9).gameObject.GetComponent<TrailRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpSwipe.mat").WaitForCompletion();
            impDash.transform.GetChild(10).GetChild(0).gameObject.GetComponent<TrailRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpSwipe.mat").WaitForCompletion();
            impDash.transform.GetChild(10).GetChild(1).gameObject.GetComponent<TrailRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpSwipe.mat").WaitForCompletion();
            impDash.transform.GetChild(10).GetChild(2).gameObject.GetComponent<TrailRenderer>().material = material;
            impDash.transform.GetChild(10).GetChild(3).gameObject.GetComponent<TrailRenderer>().material = material;

            reapEndEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/LunarSkillReplacements/LunarDetonatorConsume.prefab").WaitForCompletion().InstantiateClone("ReapEnd");
            reapEndEffect.AddComponent<NetworkIdentity>();
            var fart = reapEndEffect.transform.GetChild(0).gameObject.GetComponent<ParticleSystem>().main;
            fart.startColor = Color.black;
            fart = reapEndEffect.transform.GetChild(1).gameObject.GetComponent<ParticleSystem>().main;
            fart.startColor = Color.red;
            reapEndEffect.transform.GetChild(2).gameObject.GetComponent<ParticleSystemRenderer>().material.SetColor("_TintColor", Color.red);
            reapEndEffect.transform.GetChild(3).gameObject.SetActive(false);
            reapEndEffect.transform.GetChild(4).gameObject.GetComponent<ParticleSystemRenderer>().material.SetColor("_TintColor", Color.red);
            material = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/LunarSkillReplacements/matLunarNeedleImpactEffect.mat").WaitForCompletion());
            material.SetColor("_TintColor", Color.red);
            reapEndEffect.transform.GetChild(5).gameObject.GetComponent<ParticleSystemRenderer>().material = material;
            reapEndEffect.transform.GetChild(6).gameObject.SetActive(false);

        }

        #endregion effects

        #region projectiles
        private static void CreateProjectiles()
        {
            CreateNeedle();
            Content.AddProjectilePrefab(needlePrefab);

            CreateEmpoweredNeedle();
            Content.AddProjectilePrefab(needleButcheredPrefab);

            CreateScissorR();
            Content.AddProjectilePrefab(scissorRPrefab);

            CreateEmpoweredScissorR();
            Content.AddProjectilePrefab(scissorRButcheredPrefab);

            CreateScissorL();
            Content.AddProjectilePrefab(scissorRPrefab);

            CreateEmpoweredScissorL();
            Content.AddProjectilePrefab(scissorRButcheredPrefab);
        }
        private static void CreateNeedle()
        {
            Color theRed = new Color(155f / 255f, 55f / 255f, 55f / 255f);
            needlePrefab = Assets.CloneProjectilePrefab("FMJ", "Needle");

            ProjectileSimple needleSimple = needlePrefab.GetComponent<ProjectileSimple>();
            needleSimple.desiredForwardSpeed = 125f;
            needleSimple.lifetime = 3f;
            needleSimple.updateAfterFiring = true;

            ProjectileDamage needleDamage = needlePrefab.GetComponent<ProjectileDamage>();
            needleDamage.damageType = DamageType.Generic;

            needlePrefab.AddComponent<ProjectileTargetComponent>();

            ProjectileSteerTowardTarget needleSteer = needlePrefab.AddComponent<ProjectileSteerTowardTarget>();
            needleSteer.yAxisOnly = false;
            needleSteer.rotationSpeed = 700f;

            ProjectileOverlapAttack needleLap = needlePrefab.GetComponent<ProjectileOverlapAttack>();
            needleLap.resetInterval = 0.5f;
            needleLap.overlapProcCoefficient = SeamstressStaticValues.needleProcCoefficient;

            ProjectileDirectionalTargetFinder needleFinder = needlePrefab.AddComponent<ProjectileDirectionalTargetFinder>();
            needleFinder.lookRange = 35f;
            needleFinder.lookCone = 110f;
            needleFinder.targetSearchInterval = 0.2f;
            needleFinder.onlySearchIfNoTarget = false;
            needleFinder.allowTargetLoss = true;
            needleFinder.testLoS = true;
            needleFinder.ignoreAir = false;
            needleFinder.flierAltitudeTolerance = Mathf.Infinity;

            ProjectileController needleController = needlePrefab.GetComponent<ProjectileController>();
            needleController.procCoefficient = 1f;
            needleController.allowPrediction = false;
            needleGhost = RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/MageIceBombProjectile").GetComponent<ProjectileController>().ghostPrefab;
            needleGhost.transform.GetChild(0).gameObject.SetActive(false);
            needleGhost.transform.GetChild(1).gameObject.SetActive(false);
            needleGhost.transform.GetChild(2).localScale = new Vector3(0.2f, 0.2f, 1.66f);
            needleGhost.transform.GetChild(2).gameObject.GetComponent<MeshRenderer>().material.SetColor("_Color", new Color(84f / 255f, 0f / 255f, 11f / 255f));
            needleGhost.transform.GetChild(3).gameObject.SetActive(false);
            needleGhost.transform.GetChild(4).localScale = new Vector3(.2f, .2f, .2f);
            //REACTIVATE NEEDLE TRAILS!!!!!
            needleGhost.transform.GetChild(4).GetChild(0).gameObject.GetComponent<TrailRenderer>().material.SetColor("_TintColor", Color.red);
            needleGhost.transform.GetChild(4).GetChild(1).gameObject.GetComponent<TrailRenderer>().material.SetColor("_TintColor", Color.red);
            needleGhost.transform.GetChild(4).GetChild(3).gameObject.GetComponent<ParticleSystemRenderer>().material.SetColor("_TintColor", theRed);
            needleGhost = PrefabAPI.InstantiateClone(needleGhost, "NeedleGhost");
            if (RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/MageIceBombProjectile") != null)
                needleController.ghostPrefab = needleGhost;
            if (!needleController.ghostPrefab.GetComponent<NetworkIdentity>())
                needleController.ghostPrefab.AddComponent<NetworkIdentity>();
            if (!needleController.ghostPrefab.GetComponent<ProjectileGhostController>())
                needleController.ghostPrefab.AddComponent<ProjectileGhostController>();
            needleController.startSound = "";
        }
        private static void CreateEmpoweredNeedle()
        {
            needleButcheredPrefab = needlePrefab;
            ProjectileHealOwnerOnDamageInflicted needleHeal = needleButcheredPrefab.AddComponent<ProjectileHealOwnerOnDamageInflicted>();
            needleHeal.fractionOfDamage = SeamstressStaticValues.butcheredLifeSteal;
            needleButcheredPrefab.AddComponent<DamageAPI.ModdedDamageTypeHolderComponent>().Add(DamageTypes.CutDamage);
            needleButcheredPrefab = PrefabAPI.InstantiateClone(needleButcheredPrefab, "NeedleButchered");
        }

        private static void CreateScissorR()
        {
            scissorRPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ImpBoss/ImpVoidspikeProjectile.prefab").WaitForCompletion().InstantiateClone("ScissorR");

            Rigidbody rigid = scissorRPrefab.GetComponent<Rigidbody>();
            rigid.useGravity = true;
            rigid.freezeRotation = true;

            SphereCollider sphereCollider = scissorRPrefab.GetComponent <SphereCollider>();
            sphereCollider.material.bounciness = 0;
            sphereCollider.material.staticFriction = 10000;
            sphereCollider.material.dynamicFriction = 10000;
            sphereCollider.radius = 0.1f;

            ProjectileImpactExplosion impactAlly = scissorRPrefab.GetComponent<ProjectileImpactExplosion>();
            impactAlly.blastDamageCoefficient = 0f;
            impactAlly.blastProcCoefficient = 0f;
            impactAlly.destroyOnEnemy = false;
            impactAlly.blastAttackerFiltering = AttackerFiltering.NeverHitSelf;
            impactAlly.lifetime = 16f;
            impactAlly.lifetimeAfterImpact = 16f;

            ProjectileController scissorController = scissorRPrefab.GetComponent<ProjectileController>();
            scissorController.procCoefficient = 1f;

            ProjectileDamage scissorDamage = scissorRPrefab.GetComponent<ProjectileDamage>();
            scissorDamage.damageType = DamageType.Stun1s;

            ProjectileSimple simple = scissorRPrefab.GetComponent<ProjectileSimple>();
            simple.desiredForwardSpeed = 120f;

            ProjectileProximityBeamController prox = scissorRPrefab.AddComponent<ProjectileProximityBeamController>();
            prox.attackFireCount = 1;
            prox.attackInterval = 0.01f;
            prox.listClearInterval = 10f;
            prox.attackRange = 8f;
            prox.minAngleFilter = 0f;
            prox.maxAngleFilter = 180;
            prox.procCoefficient = 1f;
            prox.damageCoefficient = SeamstressStaticValues.scissorDamageCoefficient;
            prox.bounces = 0;
            prox.lightningType = RoR2.Orbs.LightningOrb.LightningType.MageLightning;

            scissorRPrefab.transform.GetChild(0).GetChild(4).localScale = Vector3.one * 6f;

            //changes team filter to only team
            PickupFilter scissorPickup = scissorRPrefab.transform.GetChild(0).GetChild(5).gameObject.AddComponent<PickupFilter>();
            scissorPickup.myTeamFilter = scissorRPrefab.GetComponent<TeamFilter>();
            scissorPickup.triggerEvents = scissorRPrefab.transform.GetChild(0).GetChild(5).gameObject.GetComponent<MineProximityDetonator>().triggerEvents;
            UnityEngine.Object.Destroy(scissorRPrefab.transform.GetChild(0).GetChild(5).gameObject.GetComponent<MineProximityDetonator>());

            FuckImpact fuck = scissorRPrefab.AddComponent<FuckImpact>();
            fuck.stickSoundString = scissorRPrefab.GetComponent<ProjectileStickOnImpact>().stickSoundString;
            fuck.stickParticleSystem = scissorRPrefab.GetComponent<ProjectileStickOnImpact>().stickParticleSystem;
            fuck.ignoreCharacters = true;
            fuck.ignoreWorld = false;
            fuck.stickEvent = scissorRPrefab.GetComponent<ProjectileStickOnImpact>().stickEvent;
            fuck.alignNormals = true;
            Object.Destroy(scissorRPrefab.GetComponent<ProjectileStickOnImpact>());

            scissorRPrefab.transform.GetChild(0).GetChild(5).gameObject.GetComponent<SphereCollider>().radius = 6f;

            ProjectileController controller = scissorRPrefab.GetComponent<ProjectileController>();
            if (!controller.ghostPrefab.GetComponent<ProjectileGhostController>())
                controller.ghostPrefab.AddComponent<ProjectileGhostController>();
            controller.ghostPrefab.GetComponent<ProjectileGhostController>().inheritScaleFromProjectile = false;
            if (_assetBundle.LoadAsset<GameObject>("ScissorRightGhost") != null)
                controller.ghostPrefab = _assetBundle.CreateProjectileGhostPrefab("ScissorRightGhost");
            if (!controller.ghostPrefab.GetComponent<NetworkIdentity>())
                controller.ghostPrefab.AddComponent<NetworkIdentity>();
        }
        private static void CreateEmpoweredScissorR()
        {
            scissorRButcheredPrefab = scissorRPrefab;
            ProjectileHealOwnerOnDamageInflicted scissorHeal = scissorRButcheredPrefab.AddComponent<ProjectileHealOwnerOnDamageInflicted>();
            scissorHeal.fractionOfDamage = SeamstressStaticValues.butcheredLifeSteal;
            scissorRButcheredPrefab.AddComponent<DamageAPI.ModdedDamageTypeHolderComponent>().Add(DamageTypes.CutDamage);
            scissorRButcheredPrefab = PrefabAPI.InstantiateClone(scissorRButcheredPrefab, "ScissorRButchered");
        }
        private static void CreateScissorL()
        {
            scissorLPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ImpBoss/ImpVoidspikeProjectile.prefab").WaitForCompletion().InstantiateClone("ScissorL");

            Rigidbody rigid = scissorLPrefab.GetComponent<Rigidbody>();
            rigid.useGravity = true;
            rigid.freezeRotation = true;

            SphereCollider sphereCollider = scissorLPrefab.GetComponent<SphereCollider>();
            sphereCollider.material.bounciness = 0;
            sphereCollider.material.staticFriction = 10000;
            sphereCollider.material.dynamicFriction = 10000;
            sphereCollider.radius = 0.1f;

            ProjectileOverlapAttack attack = scissorLPrefab.AddComponent<ProjectileOverlapAttack>();
            attack.damageCoefficient = 1f;
            attack.impactEffect = scissorsHitImpactEffect;
            attack.forceVector = Vector3.zero;
            attack.overlapProcCoefficient = 1f;
            attack.maximumOverlapTargets = 100;
            attack.fireFrequency = 60f;
            attack.resetInterval = -1;

            ProjectileImpactExplosion impactAlly = scissorLPrefab.GetComponent<ProjectileImpactExplosion>();
            impactAlly.blastDamageCoefficient = 0f;
            impactAlly.blastProcCoefficient = 0f;
            impactAlly.destroyOnEnemy = false;
            impactAlly.blastAttackerFiltering = AttackerFiltering.NeverHitSelf;
            impactAlly.lifetime = 16f;
            impactAlly.lifetimeAfterImpact = 16f;

            ProjectileController scissorController = scissorLPrefab.GetComponent<ProjectileController>();
            scissorController.procCoefficient = 1f;

            ProjectileDamage scissorDamage = scissorLPrefab.GetComponent<ProjectileDamage>();
            scissorDamage.damageType = DamageType.Stun1s;

            ProjectileSimple simple = scissorLPrefab.GetComponent<ProjectileSimple>();
            simple.desiredForwardSpeed = 120f;

            ProjectileProximityBeamController prox = scissorLPrefab.AddComponent<ProjectileProximityBeamController>();
            prox.attackFireCount = 1;
            prox.attackInterval = 0.01f;
            prox.listClearInterval = 10f;
            prox.attackRange = 8f;
            prox.minAngleFilter = 0f;
            prox.maxAngleFilter = 180;
            prox.procCoefficient = 1f;
            prox.damageCoefficient = SeamstressStaticValues.scissorDamageCoefficient;
            prox.bounces = 0;
            prox.lightningType = RoR2.Orbs.LightningOrb.LightningType.MageLightning;

            scissorLPrefab.transform.GetChild(0).GetChild(4).localScale = Vector3.one * 6f;

            //changes team filter to only team
            PickupFilter scissorPickup = scissorLPrefab.transform.GetChild(0).GetChild(5).gameObject.AddComponent<PickupFilter>();
            scissorPickup.myTeamFilter = scissorLPrefab.GetComponent<TeamFilter>();
            scissorPickup.triggerEvents = scissorLPrefab.transform.GetChild(0).GetChild(5).gameObject.GetComponent<MineProximityDetonator>().triggerEvents;
            UnityEngine.Object.Destroy(scissorLPrefab.transform.GetChild(0).GetChild(5).gameObject.GetComponent<MineProximityDetonator>());

            FuckImpact fuck = scissorLPrefab.AddComponent<FuckImpact>();
            fuck.stickSoundString = scissorLPrefab.GetComponent<ProjectileStickOnImpact>().stickSoundString;
            fuck.stickParticleSystem = scissorLPrefab.GetComponent<ProjectileStickOnImpact>().stickParticleSystem;
            fuck.ignoreCharacters = true;
            fuck.ignoreWorld = false;
            fuck.stickEvent = scissorLPrefab.GetComponent<ProjectileStickOnImpact>().stickEvent;
            fuck.alignNormals = true;
            Object.Destroy(scissorLPrefab.GetComponent<ProjectileStickOnImpact>());

            scissorLPrefab.transform.GetChild(0).GetChild(5).gameObject.GetComponent<SphereCollider>().radius = 6f;

            ProjectileController controller = scissorLPrefab.GetComponent<ProjectileController>();
            if (!controller.ghostPrefab.GetComponent<ProjectileGhostController>())
                controller.ghostPrefab.AddComponent<ProjectileGhostController>();
            controller.ghostPrefab.GetComponent<ProjectileGhostController>().inheritScaleFromProjectile = false;
            if (_assetBundle.LoadAsset<GameObject>("ScissorLeftGhost") != null)
                controller.ghostPrefab = _assetBundle.CreateProjectileGhostPrefab("ScissorLeftGhost");
            if (!controller.ghostPrefab.GetComponent<NetworkIdentity>())
                controller.ghostPrefab.AddComponent<NetworkIdentity>();
        }
        private static void CreateEmpoweredScissorL()
        {
            scissorLButcheredPrefab = scissorLPrefab;
            ProjectileHealOwnerOnDamageInflicted scissorHeal = scissorLButcheredPrefab.AddComponent<ProjectileHealOwnerOnDamageInflicted>();
            scissorHeal.fractionOfDamage = SeamstressStaticValues.butcheredLifeSteal;
            scissorLButcheredPrefab.AddComponent<DamageAPI.ModdedDamageTypeHolderComponent>().Add(DamageTypes.CutDamage);
            scissorLButcheredPrefab = PrefabAPI.InstantiateClone(scissorLButcheredPrefab, "ScissorLButchered");
        }

        #endregion projectiles
    }
}