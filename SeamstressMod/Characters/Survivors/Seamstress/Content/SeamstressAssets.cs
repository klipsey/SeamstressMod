using RoR2;
using UnityEngine;
using SeamstressMod.Modules;
using RoR2.Projectile;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using R2API;
using RoR2.Skills;
using UnityEngine.Rendering.PostProcessing;
using TMPro;
using ThreeEyedGames;
using UnityEngine.UIElements;

namespace SeamstressMod.Survivors.Seamstress
{
    public static class SeamstressAssets
    {
        internal static Color theRed = new Color(155f / 255f, 55f / 255f, 55f / 255f);
        //effects
        internal static GameObject parrySlashEffect;

        internal static GameObject clipSlashEffect;

        internal static GameObject expungeSlashEffect;

        internal static GameObject expungeSlashEffect2;

        internal static GameObject expungeSlashEffect3;

        internal static GameObject clipSwingEffect;

        internal static GameObject scissorsSwingEffect;

        internal static GameObject slashEffect;

        internal static GameObject slashComboEffect;

        internal static GameObject blinkPrefab;

        internal static GameObject blinkDestinationPrefab;

        internal static GameObject sewEffect;

        internal static GameObject clawsEffect;

        internal static GameObject genericImpactExplosionEffect;

        internal static GameObject scissorsComboSwingEffect;

        internal static GameObject scissorsHitImpactEffect;

        internal static GameObject needleGhost;

        internal static GameObject impDash;

        internal static GameObject smallBlinkPrefab;

        internal static GameObject expungeEffect;

        internal static GameObject reapEndEffect;

        //internal static GameObject stitchConsumeEffectPrefab;

        internal static GameObject sewn1;

        internal static GameObject sewn2;

        internal static GameObject sewn3;

        internal static GameObject trackingTelekinesis;

        internal static GameObject notTrackingTelekinesis;

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

        internal static Sprite secondary;

        internal static Sprite utilityEmp;

        internal static Sprite secondaryDisabled;

        internal static Sprite special;

        internal static Sprite specialEmp;

        internal static Sprite grab;

        internal static Sprite noGrab;

        private static AssetBundle _assetBundle;
        //projectiles
        internal static GameObject needlePrefab;

        internal static GameObject needleButcheredPrefab;

        internal static GameObject scissorRPrefab;

        internal static GameObject scissorLPrefab;
        //extra
        internal static Color coolRed = coolRed;

        public static SkillDef lockOutSkillDef;
        public static void Init(AssetBundle assetBundle)
        {

            _assetBundle = assetBundle;

            scissorsHitSoundEvent = Content.CreateAndAddNetworkSoundEventDef("Play_merc_sword_impact");

            parrySuccessSoundEvent = Content.CreateAndAddNetworkSoundEventDef("Play_voidman_m2_explode");

            CreateEffects();

            CreateProjectiles();

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
            primary = _assetBundle.LoadAsset<Sprite>("texPrimaryIcon");
            secondary = _assetBundle.LoadAsset<Sprite>("texSecondaryIcon");
            special = _assetBundle.LoadAsset<Sprite>("texSpecialIcon");

            primaryEmp = _assetBundle.LoadAsset<Sprite>("texStingerIcon");
            secondaryDisabled = _assetBundle.LoadAsset<Sprite>("texBazookaOutIcon");
            utilityEmp = _assetBundle.LoadAsset<Sprite>("texPistolIcon");
            specialEmp = _assetBundle.LoadAsset<Sprite>("texScepterSpecialIcon");

            stitchEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/BleedEffect.prefab").WaitForCompletion().InstantiateClone("StitchEffect");
            stitchEffect.AddComponent<NetworkIdentity>();
            stitchEffect.transform.GetChild(1).gameObject.GetComponent<ParticleSystemRenderer>().material.SetColor("_EmissionColor", coolRed);

            sewn1 = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Nullifier/NullifyStack3Effect.prefab").WaitForCompletion().InstantiateClone("SewnNo");
            sewn1.AddComponent<NetworkIdentity>();
            sewn1.transform.GetChild(0).GetChild(0).gameObject.GetComponent<MeshRenderer>().materials[0].SetColor("_TintColor", coolRed);
            sewn1.transform.GetChild(0).GetChild(0).gameObject.GetComponent<MeshRenderer>().materials[0].DisableKeyword("_RemapTex");
            sewn1.transform.GetChild(0).GetChild(1).gameObject.GetComponent<MeshRenderer>().materials[0].SetColor("_TintColor", coolRed);
            sewn1.transform.GetChild(0).GetChild(1).gameObject.GetComponent<MeshRenderer>().materials[0].DisableKeyword("_RemapTex");
            sewn1.transform.GetChild(0).GetChild(2).gameObject.GetComponent<MeshRenderer>().materials[0].SetColor("_TintColor", coolRed);
            sewn1.transform.GetChild(0).GetChild(2).gameObject.GetComponent<MeshRenderer>().materials[0].DisableKeyword("_RemapTex");

            sewn3 = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Nullifier/NullifyStack3Effect.prefab").WaitForCompletion().InstantiateClone("SewnYes");
            sewn3.AddComponent<NetworkIdentity>();

            trackingTelekinesis = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Huntress/HuntressTrackingIndicator.prefab").WaitForCompletion().InstantiateClone("SeamstressTracker");
            Material component = Addressables.LoadAssetAsync<Material>("RoR2/Base/UI/matUIOverbrighten2x.mat").WaitForCompletion();
            Object.DestroyImmediate(trackingTelekinesis.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>());
            SpriteRenderer balls = trackingTelekinesis.transform.GetChild(0).gameObject.AddComponent<SpriteRenderer>();
            balls.SetMaterial(component);
            grab = _assetBundle.LoadAsset<Sprite>("Grab");
            balls.sprite = grab;
            Log.Debug("gyugjretrgrgghj");
            trackingTelekinesis.transform.GetChild(1).gameObject.SetActive(false);

            notTrackingTelekinesis = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Huntress/HuntressTrackingIndicator.prefab").WaitForCompletion().InstantiateClone("SeamstressTracker2");
            Object.DestroyImmediate(notTrackingTelekinesis.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>());
            SpriteRenderer balls2 = notTrackingTelekinesis.transform.GetChild(0).gameObject.AddComponent<SpriteRenderer>();
            balls2.SetMaterial(component);
            noGrab = _assetBundle.LoadAsset<Sprite>("NoGrab");
            balls2.sprite = noGrab;
            notTrackingTelekinesis.transform.GetChild(1).gameObject.SetActive(false);

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
            stitchTempEffectPrefab.transform.GetChild(0).GetChild(1).gameObject.GetComponent<ParticleSystemRenderer>().material.SetColor("_TintColor", coolRed);


            material = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpSwipe.mat").WaitForCompletion());
            stitchConsumeEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/MercExposeConsumeEffect.prefab").WaitForCompletion().InstantiateClone("StitchConsumeEffect");
            stitchConsumeEffectPrefab.AddComponent<NetworkIdentity>();
            stitchConsumeEffectPrefab.transform.GetChild(0).GetChild(0).gameObject.GetComponent<ParticleSystemRenderer>().material = material;
            stitchConsumeEffectPrefab.transform.GetChild(0).GetChild(1).gameObject.GetComponent<ParticleSystemRenderer>().material.SetColor("_TintColor", coolRed);
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
            blinkPrefab.transform.GetChild(0).localScale = Vector3.one * 0.5f;
            blinkPrefab.transform.GetChild(1).localScale = Vector3.one * 0.5f;
            Modules.Content.CreateAndAddEffectDef(blinkPrefab);

            smallBlinkPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Imp/ImpBlinkEffect.prefab").WaitForCompletion().InstantiateClone("BlinkSmall");
            smallBlinkPrefab.AddComponent<NetworkIdentity>();
            Modules.Content.CreateAndAddEffectDef(smallBlinkPrefab);

            blinkDestinationPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Imp/ImpBossBlinkDestination.prefab").WaitForCompletion().InstantiateClone("BlinkEnd");
            blinkDestinationPrefab.AddComponent<NetworkIdentity>();
            blinkDestinationPrefab.transform.GetChild(0).localScale = Vector3.one * 0.2f;
            blinkDestinationPrefab.transform.GetChild(1).localScale = Vector3.one * 0.2f;

            slashEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/MercSwordFinisherSlash.prefab").WaitForCompletion().InstantiateClone("SeamstressSlash");
            slashEffect.AddComponent<NetworkIdentity>();
            slashEffect.transform.localScale = new Vector3(0.5f, 1f, 0.5f);
            SeamstressPlugin.Destroy(slashEffect.GetComponent<EffectComponent>());

            scissorsSwingEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/MercSwordFinisherSlash.prefab").WaitForCompletion().InstantiateClone("ScissorSwing");
            scissorsSwingEffect.AddComponent<NetworkIdentity>();
            scissorsSwingEffect.transform.GetChild(0).gameObject.SetActive(false);
            scissorsSwingEffect.transform.GetChild(1).gameObject.GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpSwipe.mat").WaitForCompletion();//Assets.LoadEffect("HenrySwordSwingEffect", true);
            scissorsSwingEffect.transform.GetChild(1).localScale = Vector3.one;
            var fard = scissorsSwingEffect.transform.GetChild(0).gameObject.GetComponent<ParticleSystem>().main;
            fard.startLifetimeMultiplier = 1.8f;
            UnityEngine.Object.Destroy(scissorsSwingEffect.GetComponent<EffectComponent>());

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

            expungeEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/LunarSkillReplacements/LunarDetonatorConsume.prefab").WaitForCompletion().InstantiateClone("ExpungeEffect");
            expungeEffect.AddComponent<NetworkIdentity>();
            expungeEffect.GetComponent<EffectComponent>().positionAtReferencedTransform = true;
            material = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Merc/matMercExposedBackdrop.mat").WaitForCompletion());
            material.SetColor("_TintColor", coolRed);
            expungeEffect.transform.GetChild(0).gameObject.GetComponent<ParticleSystemRenderer>().material = material;
            expungeEffect.transform.GetChild(1).gameObject.SetActive(false);
            expungeEffect.transform.GetChild(2).gameObject.GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpSwipe.mat").WaitForCompletion();
            expungeEffect.transform.GetChild(3).gameObject.GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpSwipe.mat").WaitForCompletion();
            fard = expungeEffect.transform.GetChild(3).gameObject.GetComponent<ParticleSystem>().main;
            fard.cullingMode = ParticleSystemCullingMode.AlwaysSimulate;
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

            genericImpactExplosionEffect = CreateImpactExplosionEffect("SeamstressScissorImpact", Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matBloodGeneric.mat").WaitForCompletion(), 2);

        }
        //love ya rob
        private static GameObject CreateImpactExplosionEffect(string effectName, Material bloodMat, float scale = 1f)
        {
            GameObject newEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Brother/BrotherSlamImpact.prefab").WaitForCompletion().InstantiateClone(effectName, true);

            newEffect.transform.Find("Spikes, Small").gameObject.SetActive(false);

            newEffect.transform.Find("PP").gameObject.SetActive(false);
            newEffect.transform.Find("Point light").gameObject.SetActive(false);
            newEffect.transform.Find("Flash Lines").GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matOpaqueDustLargeDirectional.mat").WaitForCompletion();

            newEffect.transform.GetChild(3).GetComponent<ParticleSystemRenderer>().material = bloodMat;
            newEffect.transform.Find("Flash Lines, Fire").GetComponent<ParticleSystemRenderer>().material = bloodMat;
            newEffect.transform.GetChild(6).GetComponent<ParticleSystemRenderer>().material = bloodMat;
            newEffect.transform.Find("Fire").GetComponent<ParticleSystemRenderer>().material = bloodMat;

            var boom = newEffect.transform.Find("Fire").GetComponent<ParticleSystem>().main;
            boom.startLifetimeMultiplier = 0.5f;
            boom = newEffect.transform.Find("Flash Lines, Fire").GetComponent<ParticleSystem>().main;
            boom.startLifetimeMultiplier = 0.3f;
            boom = newEffect.transform.GetChild(6).GetComponent<ParticleSystem>().main;
            boom.startLifetimeMultiplier = 0.4f;

            newEffect.transform.Find("Physics").GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/MagmaWorm/matFracturedGround.mat").WaitForCompletion();

            newEffect.transform.Find("Decal").GetComponent<Decal>().Material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpDecal.mat").WaitForCompletion();
            newEffect.transform.Find("Decal").GetComponent<AnimateShaderAlpha>().timeMax = 10f;

            newEffect.transform.Find("FoamSplash").gameObject.SetActive(false);
            newEffect.transform.Find("FoamBilllboard").gameObject.SetActive(false);
            newEffect.transform.Find("Dust").gameObject.SetActive(false);
            newEffect.transform.Find("Dust, Directional").gameObject.SetActive(false);

            newEffect.transform.localScale = Vector3.one * scale;   
      
            newEffect.AddComponent<NetworkIdentity>();

            ParticleSystemColorFromEffectData PSCFED = newEffect.AddComponent<ParticleSystemColorFromEffectData>();
            PSCFED.particleSystems = new ParticleSystem[]
            {
                newEffect.transform.Find("Fire").GetComponent<ParticleSystem>(),
                newEffect.transform.Find("Flash Lines, Fire").GetComponent<ParticleSystem>(),
                newEffect.transform.GetChild(6).GetComponent<ParticleSystem>(),
                newEffect.transform.GetChild(3).GetComponent<ParticleSystem>()
            };
            PSCFED.effectComponent = newEffect.GetComponent<EffectComponent>();

            Content.CreateAndAddEffectDef(newEffect);

            return newEffect;
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
            
            CreateScissorL();
            Content.AddProjectilePrefab(scissorLPrefab);
        }
        private static void CreateNeedle()
        {
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
            needleGhost = RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/MageIceBombProjectile").GetComponent<ProjectileController>().ghostPrefab;
            needleGhost.transform.GetChild(0).gameObject.SetActive(false);
            needleGhost.transform.GetChild(1).gameObject.SetActive(false);
            needleGhost.transform.GetChild(2).localScale = new Vector3(0.2f, 0.2f, 1.66f);
            needleGhost.transform.GetChild(2).gameObject.GetComponent<MeshRenderer>().material.SetColor("_Color", coolRed);
            needleGhost.transform.GetChild(3).gameObject.SetActive(false);
            needleGhost.transform.GetChild(4).localScale = new Vector3(.2f, .2f, .2f);
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
            needleButcheredPrefab = PrefabAPI.InstantiateClone(needlePrefab, "NeedleButchered");
            ProjectileHealOwnerOnDamageInflicted needleHeal = needleButcheredPrefab.AddComponent<ProjectileHealOwnerOnDamageInflicted>();
            needleHeal.fractionOfDamage = SeamstressStaticValues.needleLifeSteal;
            needleButcheredPrefab.AddComponent<DamageAPI.ModdedDamageTypeHolderComponent>().Add(DamageTypes.CutDamage);
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
            sphereCollider.radius = 1f;
            sphereCollider.enabled = false;

            ProjectileImpactExplosion impactAlly = scissorRPrefab.GetComponent<ProjectileImpactExplosion>();
            impactAlly.blastDamageCoefficient = SeamstressStaticValues.scissorDamageCoefficient;
            impactAlly.blastProcCoefficient = 1f;
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
            prox.inheritDamageType = true;
            prox.attackInterval = 0.001f;
            prox.listClearInterval = 16f;
            prox.attackRange = 8f;
            prox.minAngleFilter = 0f;
            prox.maxAngleFilter = 180;
            prox.procCoefficient = 1f;
            prox.damageCoefficient = SeamstressStaticValues.scissorDamageCoefficient;
            prox.bounces = 0;
            prox.lightningType = RoR2.Orbs.LightningOrb.LightningType.Count;

            ProjectileHealOwnerOnDamageInflicted heal = scissorRPrefab.AddComponent<ProjectileHealOwnerOnDamageInflicted>();
            heal.fractionOfDamage = SeamstressStaticValues.butcheredLifeSteal;
            heal.enabled = false;

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

            GameObject travelEffect = RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/MageIceBombProjectile").GetComponent<ProjectileController>().ghostPrefab.transform.GetChild(4).gameObject.InstantiateClone("Spin", false);
            travelEffect.transform.GetChild(0).gameObject.GetComponent<TrailRenderer>().material.SetColor("_TintColor", Color.red);
            travelEffect.transform.GetChild(1).gameObject.GetComponent<TrailRenderer>().material.SetColor("_TintColor", Color.red);
            travelEffect.transform.GetChild(2).gameObject.SetActive(false);
            travelEffect.transform.GetChild(3).gameObject.GetComponent<ParticleSystemRenderer>().material.SetColor("_TintColor", theRed);

            ProjectileController controller = scissorRPrefab.GetComponent<ProjectileController>();
            if (_assetBundle.LoadAsset<GameObject>("ScissorRightGhost") != null)
                controller.ghostPrefab = _assetBundle.CreateProjectileGhostPrefab("ScissorRightGhost");

            if (!controller.ghostPrefab.GetComponent<EffectComponent>()) controller.ghostPrefab.AddComponent<EffectComponent>();
            controller.ghostPrefab.GetComponent<EffectComponent>().positionAtReferencedTransform = true;
            controller.ghostPrefab.GetComponent<EffectComponent>().parentToReferencedTransform = true;

            if(!controller.ghostPrefab.GetComponent<VFXAttributes>()) controller.ghostPrefab.AddComponent<VFXAttributes>();
            controller.ghostPrefab.GetComponent<VFXAttributes>().vfxPriority = VFXAttributes.VFXPriority.Always;
            controller.ghostPrefab.GetComponent<VFXAttributes>().vfxIntensity = VFXAttributes.VFXIntensity.Medium;

            if (!controller.ghostPrefab.transform.Find("Spin")) travelEffect.transform.SetParent(controller.ghostPrefab.transform);
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
            sphereCollider.radius = 1f;
            sphereCollider.enabled = false;

            ProjectileImpactExplosion impactAlly = scissorLPrefab.GetComponent<ProjectileImpactExplosion>();
            impactAlly.blastDamageCoefficient = SeamstressStaticValues.scissorDamageCoefficient;
            impactAlly.blastProcCoefficient = 1f;
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
            prox.inheritDamageType = true;
            prox.attackInterval = 0.001f;
            prox.listClearInterval = 16f;
            prox.attackRange = 8f;
            prox.minAngleFilter = 0f;
            prox.maxAngleFilter = 180;
            prox.procCoefficient = 1f;
            prox.damageCoefficient = SeamstressStaticValues.scissorDamageCoefficient;
            prox.bounces = 0;
            prox.lightningType = RoR2.Orbs.LightningOrb.LightningType.Count;

            ProjectileHealOwnerOnDamageInflicted heal = scissorLPrefab.AddComponent<ProjectileHealOwnerOnDamageInflicted>();
            heal.fractionOfDamage = SeamstressStaticValues.butcheredLifeSteal;
            heal.enabled = false;

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

            GameObject travelEffect = RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/MageIceBombProjectile").GetComponent<ProjectileController>().ghostPrefab.transform.GetChild(4).gameObject.InstantiateClone("Spin", false);
            travelEffect.transform.GetChild(0).gameObject.GetComponent<TrailRenderer>().material.SetColor("_TintColor", Color.red);
            travelEffect.transform.GetChild(1).gameObject.GetComponent<TrailRenderer>().material.SetColor("_TintColor", Color.red);
            travelEffect.transform.GetChild(2).gameObject.SetActive(false);
            travelEffect.transform.GetChild(3).gameObject.GetComponent<ParticleSystemRenderer>().material.SetColor("_TintColor", theRed);

            ProjectileController controller = scissorLPrefab.GetComponent<ProjectileController>();
            if (_assetBundle.LoadAsset<GameObject>("ScissorLeftGhost") != null)
                controller.ghostPrefab = _assetBundle.CreateProjectileGhostPrefab("ScissorLeftGhost");

            if (!controller.ghostPrefab.GetComponent<EffectComponent>()) controller.ghostPrefab.AddComponent<EffectComponent>();
            controller.ghostPrefab.GetComponent<EffectComponent>().positionAtReferencedTransform = true;
            controller.ghostPrefab.GetComponent<EffectComponent>().parentToReferencedTransform = true;

            if (!controller.ghostPrefab.GetComponent<VFXAttributes>()) controller.ghostPrefab.AddComponent<VFXAttributes>();
            controller.ghostPrefab.GetComponent<VFXAttributes>().vfxPriority = VFXAttributes.VFXPriority.Always;
            controller.ghostPrefab.GetComponent<VFXAttributes>().vfxIntensity = VFXAttributes.VFXIntensity.Medium;

            if (!controller.ghostPrefab.transform.Find("Spin")) travelEffect.transform.SetParent(controller.ghostPrefab.transform);
        }
        #endregion projectiles
    }
}