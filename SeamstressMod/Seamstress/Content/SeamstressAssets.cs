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
using SeamstressMod.Seamstress.Components;
using static UnityEngine.ParticleSystem.PlaybackState;
using HG;
using UnityEngine.UIElements;

namespace SeamstressMod.Seamstress.Content
{
    public static class SeamstressAssets
    {
        //AssetBundle
        internal static AssetBundle _assetBundle;

        //Shader
        internal static Shader hotpoo = Resources.Load<Shader>("Shaders/Deferred/HGStandard");
        
        //Effects

        internal static GameObject spawnPrefab;

        internal static GameObject parrySlashEffect;
        internal static GameObject wideSlashEffect;
        internal static GameObject uppercutEffect;
        internal static GameObject clipSlashEffect;
        internal static GameObject scissorsSlashEffect;
        internal static GameObject scissorsComboSlashEffect;

        internal static GameObject clawSlashEffect;
        internal static GameObject clawSlashComboEffect;

        internal static GameObject blinkEffect;
        internal static GameObject blinkDestinationPrefab;

        internal static GameObject impDashEffect;
        internal static GameObject smallBlinkEffect;

        internal static GameObject groundClawsEffect;

        internal static GameObject instatiableEndEffect;

        internal static GameObject scissorsHitImpactEffect;
        internal static GameObject slamEffect;
        internal static GameObject genericImpactExplosionEffect;
        internal static GameObject bloodSplatterEffect;

        internal static GameObject sewnCdEffect;
        internal static GameObject sewnEffect;

        //Misc Prefabs
        internal static TeamAreaIndicator seamstressTeamAreaIndicator;

        internal static GameObject telekinesisTracker;
        internal static GameObject telekinesisCdTracker;

        internal static GameObject chainToHeart;
        internal static GameObject heartPrefab;

        //Overlay Effects
        internal static GameObject stitchEffect;

        //Materials
        internal static Material destealthMaterial;
        internal static Material insatiableOverlayMat;
        internal static Material parryMat;
        internal static Material crocoMat;

        //Networked Hit Sounds
        internal static NetworkSoundEventDef scissorsHitSoundEvent;
        internal static NetworkSoundEventDef parrySuccessSoundEvent;

        //Sprites
        internal static Sprite texCanGrab;
        internal static Sprite texCantGrab;

        //Projectiles
        internal static GameObject needlePrefab;
        internal static GameObject needleGhost;
        internal static GameObject needleButcheredPrefab;
        internal static GameObject scissorRPrefab;
        internal static GameObject scissorLPrefab;


        //Colors
        internal static Color coolRed = new Color(84f / 255f, 0f / 255f, 11f / 255f);
        internal static Color theRed = new Color(155f / 255f, 55f / 255f, 55f / 255f);
        public static void Init(AssetBundle assetBundle)
        {

            _assetBundle = assetBundle;

            CreateMaterials();

            CreateEffects();

            CreateProjectiles();

            CreateHeart();

            CreateSounds();
        }

        #region heart
        private static void CreateHeart()
        {

            GameObject heartMdl = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/EliteEarth/AffixEarthHealerBody.prefab").WaitForCompletion().transform.GetChild(0).GetChild(0).gameObject.InstantiateClone("HeartMdl", false);
            heartMdl.transform.localScale /= 3f;
            Material eatMyButt = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/EliteEarth/AffixEarthCore.mat").WaitForCompletion();
            Material[] explodeAndDie = new Material[1];
            explodeAndDie[0] = eatMyButt;
            heartMdl.transform.GetChild(1).gameObject.GetComponent<SkinnedMeshRenderer>().materials = explodeAndDie;
            heartMdl.transform.GetChild(1).gameObject.GetComponent<SkinnedMeshRenderer>().materials[0].SetColor("_Color", coolRed);
            heartMdl.transform.GetChild(1).gameObject.GetComponent<SkinnedMeshRenderer>().materials[0].SetColor("_EmColor", coolRed);
            Object.Destroy(heartMdl.GetComponent<CharacterModel>());
            Object.Destroy(heartMdl.GetComponent<HurtBoxGroup>());
            Object.DestroyImmediate(heartMdl.transform.GetChild(2).gameObject);
            heartMdl.transform.GetChild(2).localScale = new Vector3(1.5f, 1.5f, 1.5f);
            heartMdl.transform.GetChild(2).GetChild(0).gameObject.GetComponent<Light>().color = theRed;
            heartMdl.transform.GetChild(2).GetChild(0).gameObject.GetComponent<Light>().range = 2f;
            heartMdl.transform.GetChild(2).GetChild(0).gameObject.GetComponent<LightIntensityCurve>().timeMax = SeamstressStaticValues.instatiableDuation;
            heartMdl.transform.GetChild(2).GetChild(1).gameObject.GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Junk/Common/VFX/matBloodParticle.mat").WaitForCompletion();
            var fard = heartMdl.transform.GetChild(2).GetChild(1).gameObject.GetComponent<ParticleSystem>().main;
            fard.duration = SeamstressStaticValues.instatiableDuation;
            heartMdl.transform.GetChild(2).GetChild(2).gameObject.SetActive(false);
            heartMdl.transform.GetChild(2).GetChild(3).gameObject.SetActive(false);
            fard = heartMdl.transform.GetChild(2).GetChild(4).gameObject.GetComponent<ParticleSystem>().main;
            fard.startColor = theRed;
            fard.duration = SeamstressStaticValues.instatiableDuation;
            fard = heartMdl.transform.GetChild(2).GetChild(5).gameObject.GetComponent<ParticleSystem>().main;
            fard.startColor = coolRed;
            fard.duration = SeamstressStaticValues.instatiableDuation;
            heartMdl.transform.GetChild(2).GetChild(6).gameObject.GetComponent<ParticleSystemRenderer>().material.SetColor("_TintColor", Color.red);
            fard = heartMdl.transform.GetChild(2).GetChild(6).gameObject.GetComponent<ParticleSystem>().main;
            fard.duration = SeamstressStaticValues.instatiableDuation;
            Material chains = Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpPortalEffectEdge.mat").WaitForCompletion();
            chains.SetColor("_EmColor", Color.red);
            chains.SetColor("_TintColor", Color.red);
            Material[] ballsackTickler = new Material[1];
            ballsackTickler[0] = chains;
            chainToHeart = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Treebot/EntangleOrbEffect.prefab").WaitForCompletion().InstantiateClone("HeartChains");
            chainToHeart.AddComponent<NetworkIdentity>();
            chainToHeart.AddComponent<DestroyOnCondition>();
            chainToHeart.transform.GetChild(0).GetComponent<LineRenderer>().materials = ballsackTickler;
            chainToHeart.transform.GetChild(0).GetComponent<LineRenderer>().startColor = Color.red;
            chainToHeart.transform.GetChild(0).GetComponent<LineRenderer>().startColor = coolRed;
            chainToHeart.transform.GetChild(0).GetComponent<LineRenderer>().shadowBias = 0.5f;
            chainToHeart.transform.localScale *= 0.5f;
            chainToHeart.transform.GetChild(0).GetChild(0).gameObject.GetComponent<ParticleSystemRenderer>().mesh = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ElementalRings/PickupFireRing.prefab").WaitForCompletion().transform.GetChild(0).gameObject.GetComponent<MeshFilter>().mesh;
            chainToHeart.transform.GetChild(0).GetChild(0).gameObject.GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpPortalEffectEdge.mat").WaitForCompletion();
            chainToHeart.gameObject.GetComponent<AkEvent>().enabled = false;
            chainToHeart.gameObject.GetComponent<AkGameObj>().enabled = false;
            SeamstressMod.Modules.Content.CreateAndAddEffectDef(chainToHeart);
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
                Object.DestroyImmediate(machines[i]);
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

        private static void CreateMaterials()
        {
            insatiableOverlayMat = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/VoidSurvivor/matVoidSurvivorCorruptOverlay.mat").WaitForCompletion();

            destealthMaterial = Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpBossDissolve.mat").WaitForCompletion();

            parryMat = Addressables.LoadAssetAsync<Material>("RoR2/Base/CritOnUse/matFullCrit.mat").WaitForCompletion();
        }

        #region effects
        private static void CreateEffects()
        {
            spawnPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ImpBoss/ImpBossDeathEffect.prefab").WaitForCompletion().InstantiateClone("StitchEffect");
            spawnPrefab.AddComponent<NetworkIdentity>();

            stitchEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/BleedEffect.prefab").WaitForCompletion().InstantiateClone("StitchEffect");
            stitchEffect.AddComponent<NetworkIdentity>();
            stitchEffect.transform.GetChild(1).gameObject.GetComponent<ParticleSystemRenderer>().material.SetColor("_EmissionColor", coolRed);

            sewnCdEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Nullifier/NullifyStack3Effect.prefab").WaitForCompletion().InstantiateClone("SewnNo", false);
            sewnCdEffect.AddComponent<NetworkIdentity>();
            sewnCdEffect.transform.GetChild(0).GetChild(0).gameObject.GetComponent<MeshRenderer>().materials[0].SetColor("_TintColor", Color.clear);
            sewnCdEffect.transform.GetChild(0).GetChild(1).gameObject.GetComponent<MeshRenderer>().materials[0].SetColor("_TintColor", Color.clear);
            sewnCdEffect.transform.GetChild(0).GetChild(2).gameObject.GetComponent<MeshRenderer>().materials[0].SetColor("_TintColor", Color.clear);

            sewnEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Nullifier/NullifyStack3Effect.prefab").WaitForCompletion().InstantiateClone("SewnYes");
            sewnEffect.AddComponent<NetworkIdentity>();
            sewnEffect.transform.GetChild(0).GetChild(0).gameObject.GetComponent<MeshRenderer>().materials[0].SetColor("_TintColor", coolRed);
            sewnEffect.transform.GetChild(0).GetChild(1).gameObject.GetComponent<MeshRenderer>().materials[0].SetColor("_TintColor", coolRed);
            sewnEffect.transform.GetChild(0).GetChild(2).gameObject.GetComponent<MeshRenderer>().materials[0].SetColor("_TintColor", coolRed);

            parrySlashEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidJailer/VoidJailerCaptureTracer.prefab").WaitForCompletion().InstantiateClone("ParrySlashEffect");
            parrySlashEffect.AddComponent<NetworkIdentity>();
            parrySlashEffect.transform.gameObject.GetComponent<EffectComponent>().soundName = "Play_huntress_R_snipe_shoot";
            parrySlashEffect.transform.GetChild(2).GetChild(0).gameObject.GetComponent<ParticleSystemRenderer>().material.SetColor("_TintColor", coolRed);
            parrySlashEffect.transform.GetChild(3).gameObject.GetComponent<LineRenderer>().material.SetColor("_TintColor", coolRed);
            Modules.Content.CreateAndAddEffectDef(parrySlashEffect);

            telekinesisTracker = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Huntress/HuntressTrackingIndicator.prefab").WaitForCompletion().InstantiateClone("SeamstressTracker", false);
            Material component = Addressables.LoadAssetAsync<Material>("RoR2/Base/UI/matUIOverbrighten2x.mat").WaitForCompletion();
            Object.DestroyImmediate(telekinesisTracker.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>());
            SpriteRenderer balls = telekinesisTracker.transform.GetChild(0).gameObject.AddComponent<SpriteRenderer>();
            balls.SetMaterial(component);
            texCanGrab = _assetBundle.LoadAsset<Sprite>("Grab");
            balls.sprite = texCanGrab;
            telekinesisTracker.transform.GetChild(1).gameObject.SetActive(false);
            Sprite sprite = Addressables.LoadAssetAsync<Sprite>("RoR2/Base/UI/texCrosshair2.png").WaitForCompletion();
            Material component2 = telekinesisTracker.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().material;
            Object.DestroyImmediate(telekinesisTracker.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>());
            SpriteRenderer balls2 = telekinesisTracker.transform.GetChild(2).gameObject.AddComponent<SpriteRenderer>();
            balls2.SetMaterial(component2);
            balls2.sprite = sprite;
            balls2.color = coolRed;

            telekinesisCdTracker = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Huntress/HuntressTrackingIndicator.prefab").WaitForCompletion().InstantiateClone("NotSeamstressTracker", false);
            component = Addressables.LoadAssetAsync<Material>("RoR2/Base/UI/matUIOverbrighten2x.mat").WaitForCompletion();
            Object.DestroyImmediate(telekinesisCdTracker.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>());
            balls = telekinesisCdTracker.transform.GetChild(0).gameObject.AddComponent<SpriteRenderer>();
            balls.SetMaterial(component);
            texCanGrab = _assetBundle.LoadAsset<Sprite>("NoGrab");
            balls.sprite = texCanGrab;
            telekinesisCdTracker.transform.GetChild(1).gameObject.SetActive(false);
            sprite = Addressables.LoadAssetAsync<Sprite>("RoR2/Base/UI/texCrosshair2.png").WaitForCompletion();
            component2 = telekinesisCdTracker.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().material;
            Object.DestroyImmediate(telekinesisCdTracker.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>());
            balls2 = telekinesisCdTracker.transform.GetChild(2).gameObject.AddComponent<SpriteRenderer>();
            balls2.SetMaterial(component2);
            balls2.sprite = sprite;
            balls2.color = coolRed;

            groundClawsEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ImpBoss/ImpBossGroundSlam.prefab").WaitForCompletion().InstantiateClone("GroundClawsEffect");
            groundClawsEffect.AddComponent<NetworkIdentity>();

            blinkEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ImpBoss/ImpBossBlink.prefab").WaitForCompletion().InstantiateClone("BlinkStart");
            blinkEffect.AddComponent<NetworkIdentity>();
            blinkEffect.GetComponent<EffectComponent>().applyScale = true;
            blinkEffect.transform.GetChild(0).localScale = Vector3.one * 0.5f;
            blinkEffect.transform.GetChild(1).localScale = Vector3.one * 0.5f;
            Modules.Content.CreateAndAddEffectDef(blinkEffect);

            smallBlinkEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Imp/ImpBlinkEffect.prefab").WaitForCompletion().InstantiateClone("BlinkSmall");
            smallBlinkEffect.AddComponent<NetworkIdentity>();
            Modules.Content.CreateAndAddEffectDef(smallBlinkEffect);

            blinkDestinationPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Imp/ImpBossBlinkDestination.prefab").WaitForCompletion().InstantiateClone("BlinkEnd");
            blinkDestinationPrefab.AddComponent<NetworkIdentity>();
            blinkDestinationPrefab.transform.GetChild(0).localScale = Vector3.one * 0.2f;
            blinkDestinationPrefab.transform.GetChild(1).localScale = Vector3.one * 0.2f;

            clawSlashEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Croco/CrocoSlash.prefab").WaitForCompletion().InstantiateClone("SeamstressSlash");
            clawSlashEffect.AddComponent<NetworkIdentity>();
            clawSlashEffect.transform.GetChild(0).localScale = new Vector3(1f, 1f, 1f);
            clawSlashEffect.GetComponent<ScaleParticleSystemDuration>().initialDuration = 0.5f;
            clawSlashEffect.transform.GetChild(0).GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Croco/matCrocoSlash.mat").WaitForCompletion();
            clawSlashEffect.transform.GetChild(0).GetComponent<ParticleSystemRenderer>().material.SetColor("_TintColor", new Color(22f / 255f, 0f, 0f));
            clawSlashEffect.transform.GetChild(1).gameObject.SetActive(false);


            clawSlashComboEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Croco/CrocoComboFinisherSlash.prefab").WaitForCompletion().InstantiateClone("SeamstressComboSlash");
            clawSlashComboEffect.AddComponent<NetworkIdentity>();
            clawSlashComboEffect.transform.GetChild(0).localScale = new Vector3(1.25f, 1.25f, 1.25f);
            clawSlashComboEffect.GetComponent<ScaleParticleSystemDuration>().initialDuration = 0.5f;
            clawSlashComboEffect.transform.GetChild(0).GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Croco/matCrocoSlash.mat").WaitForCompletion();
            clawSlashComboEffect.transform.GetChild(0).GetComponent<ParticleSystemRenderer>().material.SetColor("_TintColor", new Color(22f / 255f, 0f, 0f));
            clawSlashComboEffect.transform.GetChild(1).gameObject.SetActive(false);

            scissorsSlashEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/MercSwordFinisherSlash.prefab").WaitForCompletion().InstantiateClone("ScissorSwing");
            scissorsSlashEffect.AddComponent<NetworkIdentity>();
            scissorsSlashEffect.transform.GetChild(0).gameObject.SetActive(false);
            scissorsSlashEffect.transform.GetChild(1).gameObject.GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpSwipe.mat").WaitForCompletion();//Assets.LoadEffect("HenrySwordSwingEffect", true);
            var fard = scissorsSlashEffect.transform.GetChild(0).gameObject.GetComponent<ParticleSystem>().main;
            fard.startLifetimeMultiplier = 2f;

            scissorsComboSlashEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/MercSwordFinisherSlash.prefab").WaitForCompletion().InstantiateClone("ScissorSwing3");
            scissorsComboSlashEffect.AddComponent<NetworkIdentity>();
            scissorsComboSlashEffect.transform.GetChild(0).gameObject.SetActive(false);
            scissorsComboSlashEffect.transform.GetChild(1).gameObject.GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpSwipe.mat").WaitForCompletion();
            scissorsComboSlashEffect.transform.GetChild(1).localScale = new Vector3(1f, 1.5f, 1.5f);

            clipSlashEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/MercSwordFinisherSlash.prefab").WaitForCompletion().InstantiateClone("ClipSwing");
            clipSlashEffect.AddComponent<NetworkIdentity>();
            clipSlashEffect.transform.GetChild(0).gameObject.SetActive(false);
            Material material = Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpSwipe.mat").WaitForCompletion());
            clipSlashEffect.transform.GetChild(1).gameObject.GetComponent<ParticleSystemRenderer>().material = material;
            clipSlashEffect.transform.GetChild(1).localScale = new Vector3(0.5f, 0.75f, 0.5f);
            fard = clipSlashEffect.transform.GetChild(0).gameObject.GetComponent<ParticleSystem>().main;
            fard.startLifetimeMultiplier = 0.6f;

            wideSlashEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/MercSwordSlashWhirlwind.prefab").WaitForCompletion().InstantiateClone("SeamstressWideSwordSwing");
            wideSlashEffect.AddComponent<NetworkIdentity>();
            wideSlashEffect.transform.GetChild(0).localScale *= 1.5f;
            wideSlashEffect.transform.GetChild(0).gameObject.GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpSwipe.mat").WaitForCompletion();
            var sex = wideSlashEffect.transform.GetChild(0).gameObject.GetComponent<ParticleSystem>().main;
            sex.startLifetimeMultiplier = 0.6f;
            sex.startRotation3D = false;
            Object.Destroy(wideSlashEffect.GetComponent<EffectComponent>());

            uppercutEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/MercSwordSlashWhirlwind.prefab").WaitForCompletion().InstantiateClone("SeamstressUppercut");
            uppercutEffect.AddComponent<NetworkIdentity>();
            uppercutEffect.transform.GetChild(0).gameObject.GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpSwipe.mat").WaitForCompletion();
            sex = uppercutEffect.transform.GetChild(0).gameObject.GetComponent<ParticleSystem>().main;
            sex.startLifetimeMultiplier = 0.6f;
            sex.startRotation3D = false;
            Object.Destroy(uppercutEffect.GetComponent<EffectComponent>());


            scissorsHitImpactEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/OmniImpactVFXSlashMerc.prefab").WaitForCompletion().InstantiateClone("ScissorImpact", false);
            scissorsHitImpactEffect.AddComponent<NetworkIdentity>();
            scissorsHitImpactEffect.GetComponent<OmniEffect>().enabled = false;
            material = Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Merc/matOmniHitspark3Merc.mat").WaitForCompletion());
            material.SetColor("_TintColor", Color.red);
            scissorsHitImpactEffect.transform.GetChild(1).gameObject.GetComponent<ParticleSystemRenderer>().material = material;
            scissorsHitImpactEffect.transform.GetChild(2).localScale = Vector3.one * 1.5f;
            scissorsHitImpactEffect.transform.GetChild(2).gameObject.GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/VoidSurvivor/matVoidSurvivorBlasterFireCorrupted.mat").WaitForCompletion();
            material = Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpSlashImpact.mat").WaitForCompletion());
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

            impDashEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Huntress/HuntressBlinkEffect.prefab").WaitForCompletion().InstantiateClone("ImpDash");
            impDashEffect.AddComponent<NetworkIdentity>();
            material = Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpSwipe.mat").WaitForCompletion());
            impDashEffect.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
            impDashEffect.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
            impDashEffect.transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
            impDashEffect.transform.GetChild(0).GetChild(3).gameObject.SetActive(false);
            impDashEffect.transform.GetChild(0).GetChild(4).gameObject.GetComponent<ParticleSystemRenderer>().material = material;
            impDashEffect.transform.GetChild(0).GetChild(5).gameObject.GetComponent<ParticleSystemRenderer>().material = material;
            SeamstressMod.Modules.Content.CreateAndAddEffectDef(impDashEffect);

            instatiableEndEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/LunarSkillReplacements/LunarDetonatorConsume.prefab").WaitForCompletion().InstantiateClone("ReapEnd");
            instatiableEndEffect.AddComponent<NetworkIdentity>();
            var fart = instatiableEndEffect.transform.GetChild(0).gameObject.GetComponent<ParticleSystem>().main;
            fart.startColor = Color.black;
            fart = instatiableEndEffect.transform.GetChild(1).gameObject.GetComponent<ParticleSystem>().main;
            fart.startColor = Color.red;
            instatiableEndEffect.transform.GetChild(2).gameObject.GetComponent<ParticleSystemRenderer>().material.SetColor("_TintColor", Color.red);
            instatiableEndEffect.transform.GetChild(3).gameObject.SetActive(false);
            instatiableEndEffect.transform.GetChild(4).gameObject.GetComponent<ParticleSystemRenderer>().material.SetColor("_TintColor", Color.red);
            material = Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/LunarSkillReplacements/matLunarNeedleImpactEffect.mat").WaitForCompletion());
            material.SetColor("_TintColor", Color.red);
            instatiableEndEffect.transform.GetChild(5).gameObject.GetComponent<ParticleSystemRenderer>().material = material;
            instatiableEndEffect.transform.GetChild(6).gameObject.SetActive(false);
            Object.Destroy(instatiableEndEffect.GetComponent<EffectComponent>());

            genericImpactExplosionEffect = CreateImpactExplosionEffect("SeamstressScissorImpact", Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matBloodGeneric.mat").WaitForCompletion(), 2);
            bloodSplatterEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Brother/BrotherSlamImpact.prefab").WaitForCompletion().InstantiateClone("Splat", true);
            bloodSplatterEffect.AddComponent<NetworkIdentity>();
            bloodSplatterEffect.transform.GetChild(0).gameObject.SetActive(false);
            bloodSplatterEffect.transform.GetChild(1).gameObject.SetActive(false);
            bloodSplatterEffect.transform.GetChild(2).gameObject.SetActive(false);
            bloodSplatterEffect.transform.GetChild(3).gameObject.SetActive(false);
            bloodSplatterEffect.transform.GetChild(4).gameObject.SetActive(false);
            bloodSplatterEffect.transform.GetChild(5).gameObject.SetActive(false);
            bloodSplatterEffect.transform.GetChild(6).gameObject.SetActive(false);
            bloodSplatterEffect.transform.GetChild(7).gameObject.SetActive(false);
            bloodSplatterEffect.transform.GetChild(8).gameObject.SetActive(false);
            bloodSplatterEffect.transform.GetChild(9).gameObject.SetActive(false);
            bloodSplatterEffect.transform.GetChild(10).gameObject.SetActive(false);
            bloodSplatterEffect.transform.Find("Decal").GetComponent<Decal>().Material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpDecal.mat").WaitForCompletion();
            bloodSplatterEffect.transform.Find("Decal").GetComponent<AnimateShaderAlpha>().timeMax = 10f;
            bloodSplatterEffect.transform.GetChild(12).gameObject.SetActive(false);
            bloodSplatterEffect.transform.GetChild(13).gameObject.SetActive(false);
            bloodSplatterEffect.transform.GetChild(14).gameObject.SetActive(false);
            bloodSplatterEffect.transform.GetChild(15).gameObject.SetActive(false);
            bloodSplatterEffect.transform.localScale = Vector3.one;
            SeamstressMod.Modules.Content.CreateAndAddEffectDef(bloodSplatterEffect);

            slamEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ImpBoss/ImpBossGroundSlam.prefab").WaitForCompletion().InstantiateClone("SeamstressSlamEffect");
            slamEffect.AddComponent<NetworkIdentity>();
            SeamstressMod.Modules.Content.CreateAndAddEffectDef(slamEffect);

            GameObject impThing = LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/ImpVoidspikeProjectile");

            TeamAreaIndicator teamArea = PrefabAPI.InstantiateClone(impThing.transform.Find("ImpactEffect/TeamAreaIndicator, FullSphere").gameObject, "SeamstressTeamIndicator", false).GetComponent<TeamAreaIndicator>();
            
            teamArea.teamMaterialPairs[1].sharedMaterial = new Material(teamArea.teamMaterialPairs[1].sharedMaterial);
            teamArea.teamMaterialPairs[1].sharedMaterial.SetColor("_TintColor", Color.red);

            seamstressTeamAreaIndicator = teamArea;
        }

        #endregion

        #region projectiles
        private static void CreateProjectiles()
        {
            CreateNeedle();
            SeamstressMod.Modules.Content.AddProjectilePrefab(needlePrefab);

            CreateEmpoweredNeedle();
            SeamstressMod.Modules.Content.AddProjectilePrefab(needleButcheredPrefab);

            scissorRPrefab = CreateScissor("ScissorRightGhost", "ScissorR");

            scissorLPrefab = CreateScissor("ScissorLeftGhost", "ScissorL");
        }
        private static void CreateNeedle()
        {
            needlePrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/FMJRamping.prefab").WaitForCompletion().InstantiateClone("Needle");

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
            needleLap.impactEffect = scissorsHitImpactEffect;
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
            needleGhost = LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/MageIceBombProjectile").GetComponent<ProjectileController>().ghostPrefab.InstantiateClone("NeedleGhost", false);
            needleGhost.transform.GetChild(0).gameObject.SetActive(false);
            needleGhost.transform.GetChild(1).gameObject.SetActive(false);
            needleGhost.transform.GetChild(2).localScale = new Vector3(0.2f, 0.2f, 1.66f);
            needleGhost.transform.GetChild(2).gameObject.GetComponent<MeshFilter>().mesh = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ImpBoss/ImpVoidspikeProjectileGhost.prefab").WaitForCompletion().transform.GetChild(0).GetComponent<MeshFilter>().mesh;
            needleGhost.transform.GetChild(2).gameObject.GetComponent<MeshRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpClaw.mat").WaitForCompletion();
            needleGhost.transform.GetChild(3).gameObject.SetActive(false);
            needleGhost.transform.GetChild(4).localScale = new Vector3(.2f, .2f, .2f);
            needleGhost.transform.GetChild(4).GetChild(0).gameObject.GetComponent<TrailRenderer>().material = Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpPortalEffectEdge.mat").WaitForCompletion());
            needleGhost.transform.GetChild(4).GetChild(1).gameObject.GetComponent<TrailRenderer>().material = Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpPortalEffectEdge.mat").WaitForCompletion());
            needleGhost.transform.GetChild(4).GetChild(3).gameObject.SetActive(false);
            needleGhost = needleGhost.InstantiateClone("NeedleGhost");
            Object.Destroy(needleGhost.GetComponent<EffectComponent>());
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
            needleButcheredPrefab = needlePrefab.InstantiateClone("NeedleButchered");
            ProjectileHealOwnerOnDamageInflicted needleHeal = needleButcheredPrefab.AddComponent<ProjectileHealOwnerOnDamageInflicted>();
            needleHeal.fractionOfDamage = SeamstressStaticValues.insatiableLifesSteal;
            needleButcheredPrefab.AddComponent<DamageAPI.ModdedDamageTypeHolderComponent>().Add(DamageTypes.CutDamage);
        }
        
        private static GameObject CreateScissor(string modelName, string name)
        {
            GameObject scissorPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ImpBoss/ImpVoidspikeProjectile.prefab").WaitForCompletion().InstantiateClone(name, true);
            Rigidbody rigid = scissorPrefab.GetComponent<Rigidbody>();
            rigid.useGravity = true;
            rigid.freezeRotation = true;

            SphereCollider sphereCollider = scissorPrefab.GetComponent<SphereCollider>();
            sphereCollider.material.bounciness = 0;
            sphereCollider.material.staticFriction = 10000;
            sphereCollider.material.dynamicFriction = 10000;
            sphereCollider.radius = 1f;
            sphereCollider.enabled = false;

            scissorPrefab.transform.Find("ImpactEffect/TeamAreaIndicator, FullSphere").gameObject.SetActive(false);

            TeamAreaIndicator seamArea = UnityEngine.Object.Instantiate(seamstressTeamAreaIndicator, scissorPrefab.transform);
            seamArea.gameObject.transform.localScale = Vector3.one * 6f;
            seamArea.teamFilter = scissorPrefab.GetComponent<TeamFilter>();

            seamArea.gameObject.SetActive(false);

            ProjectileImpactExplosion impactAlly = scissorPrefab.GetComponent<ProjectileImpactExplosion>();
            impactAlly.blastDamageCoefficient = SeamstressStaticValues.scissorSlashDamageCoefficient;
            impactAlly.blastProcCoefficient = 1f;
            impactAlly.destroyOnEnemy = false;
            impactAlly.blastAttackerFiltering = AttackerFiltering.NeverHitSelf;
            impactAlly.lifetime = 16f;
            impactAlly.lifetimeAfterImpact = 16f;

            ProjectileDamage scissorDamage = scissorPrefab.GetComponent<ProjectileDamage>();
            scissorDamage.damageType = DamageType.Stun1s;

            ProjectileSimple simple = scissorPrefab.GetComponent<ProjectileSimple>();
            simple.desiredForwardSpeed = 120f;

            //changes team filter to only team
            PickupFilter scissorPickup = scissorPrefab.transform.GetChild(0).GetChild(5).gameObject.AddComponent<PickupFilter>();
            scissorPickup.myTeamFilter = scissorPrefab.GetComponent<TeamFilter>();
            scissorPickup.triggerEvents = scissorPrefab.transform.GetChild(0).GetChild(5).gameObject.GetComponent<MineProximityDetonator>().triggerEvents;
            Object.Destroy(scissorPrefab.transform.GetChild(0).GetChild(5).gameObject.GetComponent<MineProximityDetonator>());

            FuckImpact fuck = scissorPrefab.AddComponent<FuckImpact>();
            fuck.stickSoundString = scissorPrefab.GetComponent<ProjectileStickOnImpact>().stickSoundString;
            fuck.stickParticleSystem = scissorPrefab.GetComponent<ProjectileStickOnImpact>().stickParticleSystem;
            fuck.ignoreCharacters = false;
            fuck.ignoreWorld = false;
            fuck.stickEvent = scissorPrefab.GetComponent<ProjectileStickOnImpact>().stickEvent;
            fuck.alignNormals = true;
            Object.Destroy(scissorPrefab.GetComponent<ProjectileStickOnImpact>());

            scissorPrefab.transform.GetChild(0).GetChild(5).gameObject.GetComponent<SphereCollider>().radius = 6f;
            scissorPrefab.AddComponent<DamageAPI.ModdedDamageTypeHolderComponent>();

            GameObject travelEffect = LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/MageIceBombProjectile").GetComponent<ProjectileController>().ghostPrefab.transform.GetChild(4).gameObject.InstantiateClone("Spin", false);
            travelEffect.transform.GetChild(0).gameObject.GetComponent<TrailRenderer>().material = Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpPortalEffectEdge.mat").WaitForCompletion());
            travelEffect.transform.GetChild(1).gameObject.GetComponent<TrailRenderer>().material = Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpPortalEffectEdge.mat").WaitForCompletion());
            travelEffect.transform.GetChild(2).gameObject.SetActive(false);
            travelEffect.transform.GetChild(3).gameObject.GetComponent<ParticleSystemRenderer>().material.SetColor("_TintColor", theRed);

            ProjectileController scissorController = scissorPrefab.GetComponent<ProjectileController>();
            scissorController.procCoefficient = 1f;
            if (_assetBundle.LoadAsset<GameObject>(modelName) != null)
                scissorController.ghostPrefab = _assetBundle.CreateProjectileGhostPrefab(modelName);
            if (!scissorController.ghostPrefab.GetComponent<NetworkIdentity>())
                scissorController.ghostPrefab.AddComponent<NetworkIdentity>();
            if (!scissorController.ghostPrefab.GetComponent<VFXAttributes>()) scissorController.ghostPrefab.AddComponent<VFXAttributes>();
            scissorController.ghostPrefab.GetComponent<VFXAttributes>().vfxPriority = VFXAttributes.VFXPriority.Always;
            scissorController.ghostPrefab.GetComponent<VFXAttributes>().vfxIntensity = VFXAttributes.VFXIntensity.Medium;

            if (!scissorController.ghostPrefab.transform.Find("Spin")) travelEffect.transform.SetParent(scissorController.ghostPrefab.transform);

            SeamstressMod.Modules.Content.AddProjectilePrefab(scissorPrefab);

            return scissorPrefab;
        }
        #endregion

        #region sounds
        private static void CreateSounds()
        {
            scissorsHitSoundEvent = SeamstressMod.Modules.Content.CreateAndAddNetworkSoundEventDef("Play_merc_sword_impact");

            parrySuccessSoundEvent = SeamstressMod.Modules.Content.CreateAndAddNetworkSoundEventDef("Play_voidman_m2_explode");
        }
        #endregion

        #region helpers
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

            SeamstressMod.Modules.Content.CreateAndAddEffectDef(newEffect);

            return newEffect;
        }
        public static Material CreateMaterial(string materialName, float emission, Color emissionColor, float normalStrength)
        {
            if (!crocoMat) crocoMat = Resources.Load<GameObject>("Prefabs/CharacterBodies/CommandoBody").GetComponentInChildren<CharacterModel>().baseRendererInfos[0].defaultMaterial;

            Material mat = UnityEngine.Object.Instantiate<Material>(crocoMat);
            Material tempMat = _assetBundle.LoadAsset<Material>(materialName);

            if (!tempMat) return crocoMat;

            mat.name = materialName;
            mat.SetColor("_Color", tempMat.GetColor("_Color"));
            mat.SetTexture("_MainTex", tempMat.GetTexture("_MainTex"));
            mat.SetColor("_EmColor", emissionColor);
            mat.SetFloat("_EmPower", emission);
            mat.SetTexture("_EmTex", tempMat.GetTexture("_EmissionMap"));
            mat.SetFloat("_NormalStrength", normalStrength);

            return mat;
        }

        public static Material CreateMaterial(string materialName)
        {
            return CreateMaterial(materialName, 0f);
        }

        public static Material CreateMaterial(string materialName, float emission)
        {
            return CreateMaterial(materialName, emission, Color.black);
        }

        public static Material CreateMaterial(string materialName, float emission, Color emissionColor)
        {
            return CreateMaterial(materialName, emission, emissionColor, 0f);
        }
        #endregion
    }
}