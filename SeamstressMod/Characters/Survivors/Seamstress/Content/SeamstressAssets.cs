using RoR2;
using RoR2.CharacterAI;
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
using static RoR2.Skills.ComboSkillDef;
using EntityStates.AffixEarthHealer;
using SeamstressMod.SkillStates;

namespace SeamstressMod.Survivors.Seamstress
{
    public static class SeamstressAssets
    {
        internal static Shader hotpoo = Resources.Load<Shader>("Shaders/Deferred/HGStandard");
        internal static Material commandoMat;
        //effects
        internal static GameObject pullShit;

        internal static GameObject wideSlashEffect;

        internal static GameObject splat;

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

        internal static GameObject sewn3;

        internal static GameObject trackingTelekinesis;

        internal static GameObject notTrackingTelekinesis;

        internal static GameObject slamEffect;

        internal static GameObject heartMdl;

        internal static GameObject chainToHeart;

        internal static GameObject heartPrefab;
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
        internal static Color coolRed = new Color(84f / 255f, 0f / 255f, 11f / 255f);

        internal static Color theRed = new Color(155f / 255f, 55f / 255f, 55f / 255f);

        public static SkillDef lockOutSkillDef;

        public static SkillDef snapBackSkillDef;
        public static void Init(AssetBundle assetBundle)
        {

            _assetBundle = assetBundle;

            scissorsHitSoundEvent = Content.CreateAndAddNetworkSoundEventDef("Play_merc_sword_impact");

            parrySuccessSoundEvent = Content.CreateAndAddNetworkSoundEventDef("Play_voidman_m2_explode");

            CreateEffects();

            CreateProjectiles();

            CreateHeart();

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

            snapBackSkillDef = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "SnapBack",
                skillNameToken = "SnapBack",
                skillDescriptionToken = "Snapback to core",
                keywordTokens = new string[] { },
                skillIcon = utilityEmp,

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
        #region heart
        private static void CreateHeart()
        {
            
            heartMdl = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/EliteEarth/AffixEarthHealerBody.prefab").WaitForCompletion().transform.GetChild(0).GetChild(0).gameObject.InstantiateClone("HeartMdl");
            heartMdl.transform.localScale /= 3f;
            Material eatMyButt = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/EliteEarth/AffixEarthCore.mat").WaitForCompletion();
            eatMyButt.SetColor("_Color", coolRed);
            eatMyButt.SetColor("_EmColor", coolRed);
            Material[] explodeAndDie = new Material[1];
            explodeAndDie[0] = eatMyButt;
            heartMdl.transform.GetChild(1).gameObject.GetComponent<SkinnedMeshRenderer>().materials = explodeAndDie;
            Object.DestroyImmediate(heartMdl.GetComponent<CharacterModel>());
            Object.DestroyImmediate(heartMdl.GetComponent<HurtBoxGroup>());
            Object.DestroyImmediate(heartMdl.transform.GetChild(2).gameObject);
            heartMdl.transform.GetChild(2).localScale = new Vector3(1.5f, 1.5f, 1.5f);
            heartMdl.transform.GetChild(2).GetChild(0).gameObject.GetComponent<Light>().color = theRed;
            heartMdl.transform.GetChild(2).GetChild(0).gameObject.GetComponent<Light>().range = 2f;
            heartMdl.transform.GetChild(2).GetChild(0).gameObject.GetComponent<LightIntensityCurve>().timeMax = SeamstressStaticValues.butcheredDuration;
            heartMdl.transform.GetChild(2).GetChild(1).gameObject.GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Junk/Common/VFX/matBloodParticle.mat").WaitForCompletion();
            var fard = heartMdl.transform.GetChild(2).GetChild(1).gameObject.GetComponent<ParticleSystem>().main;
            fard.duration = SeamstressStaticValues.butcheredDuration;
            heartMdl.transform.GetChild(2).GetChild(2).gameObject.SetActive(false);
            heartMdl.transform.GetChild(2).GetChild(3).gameObject.SetActive(false);
            fard = heartMdl.transform.GetChild(2).GetChild(4).gameObject.GetComponent<ParticleSystem>().main;
            fard.startColor = theRed;
            fard.duration = SeamstressStaticValues.butcheredDuration;
            fard = heartMdl.transform.GetChild(2).GetChild(5).gameObject.GetComponent<ParticleSystem>().main;
            fard.startColor = coolRed;
            fard.duration = SeamstressStaticValues.butcheredDuration;
            heartMdl.transform.GetChild(2).GetChild(6).gameObject.GetComponent<ParticleSystemRenderer>().material.SetColor("_TintColor", Color.red);
            fard = heartMdl.transform.GetChild(2).GetChild(6).gameObject.GetComponent<ParticleSystem>().main;
            fard.duration = SeamstressStaticValues.butcheredDuration;
            Material chains = Addressables.LoadAssetAsync<Material>("RoR2/Base/Gravekeeper/matGravekeeperHookChain.mat").WaitForCompletion();
            chains.SetColor("_TintColor", coolRed);
            Material[] ballsackTickler = new Material[2];
            ballsackTickler[0] = chains;
            ballsackTickler[1] = chains;
            chainToHeart = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Treebot/EntangleOrbEffect.prefab").WaitForCompletion().InstantiateClone("HeartChains");
            chainToHeart.transform.GetChild(0).GetComponent<LineRenderer>().materials = ballsackTickler;
            chainToHeart.transform.localScale *= 0.5f;
            chainToHeart.transform.GetChild(0).GetChild(0).gameObject.GetComponent <ParticleSystemRenderer>().mesh = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ElementalRings/PickupFireRing.prefab").WaitForCompletion().transform.GetChild(0).gameObject.GetComponent<MeshFilter>().mesh;
            chainToHeart.transform.GetChild(0).GetChild(0).gameObject.GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Gravekeeper/matGravekeeperHookChain.mat").WaitForCompletion();
            Object.Destroy(chainToHeart.gameObject.GetComponent<AkEvent>());
            Object.Destroy(chainToHeart.gameObject.GetComponent<AkGameObj>());
            Content.CreateAndAddEffectDef(chainToHeart);
            heartPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Treebot/TreebotFlower2.prefab").WaitForCompletion().InstantiateClone("HeartPrefab");
            heartPrefab.transform.localRotation = Quaternion.identity;
            CleanChildren(heartPrefab.transform.GetChild(0));
            heartPrefab.transform.GetChild(0).localRotation = new Quaternion(6.643471e-24f, 4.689353e-39f, 2.914701e-43f, Quaternion.identity.w);
            heartPrefab.transform.localPosition = Vector3.zero;
            heartMdl.transform.SetParent(heartPrefab.transform.GetChild(0));
            heartPrefab.gameObject.GetComponent<ModelLocator>().modelTransform = heartPrefab.transform.GetChild(0).GetChild(0);
            heartPrefab.gameObject.GetComponent<ProjectileDamage>().enabled = false;
            EntityStateMachine[] machines = heartPrefab.GetComponents<EntityStateMachine>();

            for (int i = machines.Length - 1; i >= 0; i--)
            {
                UnityEngine.Object.DestroyImmediate(machines[i]);
            }
            Prefabs.AddMainEntityStateMachine(heartPrefab, "Main", typeof(SkillStates.HeartStandBy), typeof(SkillStates.HeartSpawnState));
        }

        private static void CleanChildren(Transform startingTrans)
        {
            for (int num = startingTrans.childCount - 1; num >= 0; num--)
            {
                if (startingTrans.GetChild(num).childCount > 0)
                {
                    CleanChildren(startingTrans.GetChild(num));
                }
                Object.DestroyImmediate(startingTrans.GetChild(num).gameObject);
            }
        }
        #endregion
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
            sewn1.transform.GetChild(0).GetChild(0).gameObject.GetComponent<MeshRenderer>().materials[0].SetColor("_TintColor", Color.clear);
            sewn1.transform.GetChild(0).GetChild(1).gameObject.GetComponent<MeshRenderer>().materials[0].SetColor("_TintColor", Color.clear);
            sewn1.transform.GetChild(0).GetChild(2).gameObject.GetComponent<MeshRenderer>().materials[0].SetColor("_TintColor", Color.clear);

            sewn3 = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Nullifier/NullifyStack3Effect.prefab").WaitForCompletion().InstantiateClone("SewnYes");
            sewn3.AddComponent<NetworkIdentity>();
            sewn3.transform.GetChild(0).GetChild(0).gameObject.GetComponent<MeshRenderer>().materials[0].SetColor("_TintColor", coolRed);
            sewn3.transform.GetChild(0).GetChild(1).gameObject.GetComponent<MeshRenderer>().materials[0].SetColor("_TintColor", coolRed);
            sewn3.transform.GetChild(0).GetChild(2).gameObject.GetComponent<MeshRenderer>().materials[0].SetColor("_TintColor", coolRed);

            pullShit = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidJailer/VoidJailerCaptureTracer.prefab").WaitForCompletion().InstantiateClone("De");
            pullShit.AddComponent<NetworkIdentity>();
            pullShit.transform.gameObject.GetComponent<EffectComponent>().soundName = "Play_huntress_R_snipe_shoot";
            pullShit.transform.GetChild(2).GetChild(0).gameObject.GetComponent<ParticleSystemRenderer>().material.SetColor("_TintColor", coolRed);
            pullShit.transform.GetChild(3).gameObject.GetComponent<LineRenderer>().material.SetColor("_TintColor", coolRed);
            Modules.Content.CreateAndAddEffectDef(pullShit);

            trackingTelekinesis = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Huntress/HuntressTrackingIndicator.prefab").WaitForCompletion().InstantiateClone("SeamstressTracker");
            Material component = Addressables.LoadAssetAsync<Material>("RoR2/Base/UI/matUIOverbrighten2x.mat").WaitForCompletion();
            Object.DestroyImmediate(trackingTelekinesis.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>());
            SpriteRenderer balls = trackingTelekinesis.transform.GetChild(0).gameObject.AddComponent<SpriteRenderer>();
            balls.SetMaterial(component);
            grab = _assetBundle.LoadAsset<Sprite>("Grab");
            balls.sprite = grab;
            trackingTelekinesis.transform.GetChild(1).gameObject.SetActive(false);
            Sprite sprite = Addressables.LoadAssetAsync<Sprite>("texCrosshair2").WaitForCompletion();
            Material component2 = Addressables.LoadAssetAsync<Material>("Sprites-Default").WaitForCompletion();
            Object.DestroyImmediate(trackingTelekinesis.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>());
            SpriteRenderer balls2 = trackingTelekinesis.transform.GetChild(2).gameObject.AddComponent<SpriteRenderer>();
            balls2.SetMaterial(component2);
            balls2.sprite = sprite;
            balls2.color = coolRed;

            notTrackingTelekinesis = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Huntress/HuntressTrackingIndicator.prefab").WaitForCompletion().InstantiateClone("SeamstressTracker");
            component = Addressables.LoadAssetAsync<Material>("RoR2/Base/UI/matUIOverbrighten2x.mat").WaitForCompletion();
            Object.DestroyImmediate(notTrackingTelekinesis.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>());
            balls = notTrackingTelekinesis.transform.GetChild(0).gameObject.AddComponent<SpriteRenderer>();
            balls.SetMaterial(component);
            grab = _assetBundle.LoadAsset<Sprite>("NoGrab");
            balls.sprite = grab;
            notTrackingTelekinesis.transform.GetChild(1).gameObject.SetActive(false);
            sprite = Addressables.LoadAssetAsync<Sprite>("texCrosshair2").WaitForCompletion();
            component2 = Addressables.LoadAssetAsync<Material>("Sprites-Default").WaitForCompletion();
            Object.DestroyImmediate(notTrackingTelekinesis.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>());
            balls2 = notTrackingTelekinesis.transform.GetChild(2).gameObject.AddComponent<SpriteRenderer>();
            balls2.SetMaterial(component2);
            balls2.sprite = sprite;
            balls2.color = coolRed;

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

            wideSlashEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/MercSwordSlashWhirlwind.prefab").WaitForCompletion().InstantiateClone("SeamstressWideSwordSwing");
            wideSlashEffect.AddComponent<NetworkIdentity>();
            wideSlashEffect.transform.GetChild(0).localScale *= 1f;
            wideSlashEffect.transform.GetChild(0).gameObject.GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpSwipe.mat").WaitForCompletion();
            var sex = wideSlashEffect.transform.GetChild(0).gameObject.GetComponent<ParticleSystem>().main;
            sex.startLifetimeMultiplier = 0.6f;
            Object.Destroy(wideSlashEffect.GetComponent<EffectComponent>());

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

            impDash = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Huntress/HuntressBlinkEffect.prefab").WaitForCompletion().InstantiateClone("ImpDash");
            impDash.AddComponent<NetworkIdentity>();
            material = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpSwipe.mat").WaitForCompletion());
            impDash.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
            impDash.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
            impDash.transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
            impDash.transform.GetChild(0).GetChild(3).gameObject.SetActive(false);
            impDash.transform.GetChild(0).GetChild(4).gameObject.GetComponent<ParticleSystemRenderer>().material = material;
            impDash.transform.GetChild(0).GetChild(5).gameObject.GetComponent<ParticleSystemRenderer>().material = material;
            Content.CreateAndAddEffectDef(impDash);

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
            splat = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Brother/BrotherSlamImpact.prefab").WaitForCompletion().InstantiateClone("Splat", true);
            splat.AddComponent<NetworkIdentity>();
            splat.transform.GetChild(0).gameObject.SetActive(false);
            splat.transform.GetChild(1).gameObject.SetActive(false);
            splat.transform.GetChild(2).gameObject.SetActive(false);
            splat.transform.GetChild(3).gameObject.SetActive(false);
            splat.transform.GetChild(4).gameObject.SetActive(false);
            splat.transform.GetChild(5).gameObject.SetActive(false);
            splat.transform.GetChild(6).gameObject.SetActive(false);
            splat.transform.GetChild(7).gameObject.SetActive(false);
            splat.transform.GetChild(8).gameObject.SetActive(false);
            splat.transform.GetChild(9).gameObject.SetActive(false);
            splat.transform.GetChild(10).gameObject.SetActive(false);
            splat.transform.Find("Decal").GetComponent<Decal>().Material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpDecal.mat").WaitForCompletion();
            splat.transform.Find("Decal").GetComponent<AnimateShaderAlpha>().timeMax = 10f;
            splat.transform.GetChild(12).gameObject.SetActive(false);
            splat.transform.GetChild(13).gameObject.SetActive(false);
            splat.transform.GetChild(14).gameObject.SetActive(false);
            splat.transform.GetChild(15).gameObject.SetActive(false);
            splat.transform.localScale = Vector3.one;
            Content.CreateAndAddEffectDef(splat);

            slamEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ImpBoss/ImpBossGroundSlam.prefab").WaitForCompletion().InstantiateClone("SeamstressSlamEffect");
            slamEffect.AddComponent<NetworkIdentity>();
            Content.CreateAndAddEffectDef(slamEffect);
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
            needleGhost.transform.GetChild(2).gameObject.GetComponent<MeshFilter>().mesh = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ImpBoss/ImpVoidspikeProjectileGhost.prefab").WaitForCompletion().transform.GetChild(0).GetComponent<MeshFilter>().mesh;
            needleGhost.transform.GetChild(2).gameObject.GetComponent<MeshRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpClaw.mat").WaitForCompletion();
            needleGhost.transform.GetChild(3).gameObject.SetActive(false);
            needleGhost.transform.GetChild(4).localScale = new Vector3(.2f, .2f, .2f);
            needleGhost.transform.GetChild(4).GetChild(0).gameObject.GetComponent<TrailRenderer>().material.SetColor("_TintColor", Color.red);
            needleGhost.transform.GetChild(4).GetChild(1).gameObject.GetComponent<TrailRenderer>().material.SetColor("_TintColor", Color.red);
            needleGhost.transform.GetChild(4).GetChild(3).gameObject.SetActive(false);
            needleGhost = PrefabAPI.InstantiateClone(needleGhost, "NeedleGhost");
            if (needleGhost)
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