using RoR2;
using UnityEngine;
using SeamstressMod.Modules;
using RoR2.Projectile;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using R2API;
using RoR2.UI;

namespace SeamstressMod.Survivors.Seamstress
{
    public static class SeamstressAssets
    {
        //effects
        internal static GameObject expungeSlashEffect;

        internal static GameObject expungeSlashEffect2;

        internal static GameObject expungeSlashEffect3;

        internal static GameObject scissorsSwingEffect;

        internal static GameObject scissorsSwingEffect2;

        internal static GameObject scissorsButcheredSwingEffect;

        internal static GameObject blinkPrefab;

        internal static GameObject blinkPrefabBig;

        internal static GameObject blinkDestinationPrefab;

        internal static GameObject blinkDestinationPrefabBig;

        internal static GameObject stitchTempEffectPrefab;

        internal static GameObject sewEffect;

        internal static GameObject sewButcheredEffect;

        internal static GameObject scissorsComboSwingEffect;

        internal static GameObject scissorsButcheredComboSwingEffect;

        internal static GameObject scissorsHitImpactEffect;

        internal static GameObject scissorsButcheredHitImpactEffect;

        internal static GameObject needleGhost;

        internal static GameObject needleButcheredGhost;

        internal static GameObject weaveDash;

        internal static GameObject weaveDashButchered;

        internal static GameObject expungeEffect;

        internal static GameObject reapBleedEffect;

        internal static GameObject reapEndEffect;
        //Materials
        internal static Material destealthMaterial;

        internal static Material butcheredOverlayMat;
        // particle effects
        internal static GameObject stitchEffect;
        // networked hit sounds
        internal static NetworkSoundEventDef scissorsHitSoundEvent;

        internal static NetworkSoundEventDef sewHitSoundEvent;

        private static AssetBundle _assetBundle;
        //crosshairs

        internal static GameObject needlePrefab;

        internal static GameObject needleButcheredPrefab;
        public static void Init(AssetBundle assetBundle)
        {

            _assetBundle = assetBundle;

            scissorsHitSoundEvent = Content.CreateAndAddNetworkSoundEventDef("Play_bandit2_m2_impact");

            sewHitSoundEvent = Content.CreateAndAddNetworkSoundEventDef("Play_imp_overlord_attack2_tell");

            CreateEffects();

            CreateProjectiles();
        }


        #region effects
        private static void CreateEffects()
        {
            stitchEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/BleedEffect.prefab").WaitForCompletion().InstantiateClone("StitchEffect");
            stitchEffect.AddComponent<NetworkIdentity>();

            Material material = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Merc/matMercExposed.mat").WaitForCompletion());
            material.SetColor("_TintColor", Color.red);
            stitchTempEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/MercExposeEffect.prefab").WaitForCompletion().InstantiateClone("StitchEffectPrefab");
            stitchTempEffectPrefab.AddComponent<NetworkIdentity>();
            stitchTempEffectPrefab.transform.GetChild(0).GetChild(0).gameObject.GetComponent<ParticleSystemRenderer>().material = material;

            butcheredOverlayMat = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/VoidSurvivor/matVoidSurvivorCorruptOverlay.mat").WaitForCompletion();

            destealthMaterial = Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpBossDissolve.mat").WaitForCompletion();

            blinkPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ImpBoss/ImpBossBlink.prefab").WaitForCompletion().InstantiateClone("BlinkStart");
            blinkPrefab.AddComponent<NetworkIdentity>();
            blinkPrefab.transform.GetChild(0).localScale = Vector3.one * 0.5f;
            blinkPrefab.transform.GetChild(1).localScale = Vector3.one * 0.5f;

            blinkPrefabBig = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ImpBoss/ImpBossBlink.prefab").WaitForCompletion().InstantiateClone("BlinkStartBig");
            blinkPrefabBig.AddComponent<NetworkIdentity>();

            blinkDestinationPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Imp/ImpBossBlinkDestination.prefab").WaitForCompletion().InstantiateClone("BlinkEnd");
            blinkDestinationPrefab.AddComponent<NetworkIdentity>();
            blinkDestinationPrefab.transform.GetChild(0).localScale = Vector3.one * 0.5f;
            blinkDestinationPrefab.transform.GetChild(1).localScale = Vector3.one * 0.5f;

            blinkDestinationPrefabBig = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Imp/ImpBossBlinkDestination.prefab").WaitForCompletion().InstantiateClone("BlinkEndEmpowered");
            blinkDestinationPrefabBig.AddComponent<NetworkIdentity>();

            scissorsButcheredSwingEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/MercSwordFinisherSlash.prefab").WaitForCompletion().InstantiateClone("ScissorSwing");
            scissorsButcheredSwingEffect.AddComponent<NetworkIdentity>();
            scissorsButcheredSwingEffect.transform.GetChild(0).gameObject.SetActive(value: false);
            scissorsButcheredSwingEffect.transform.GetChild(1).gameObject.GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpSwipe.mat").WaitForCompletion();//Assets.LoadEffect("HenrySwordSwingEffect", true);
            scissorsButcheredSwingEffect.transform.GetChild(1).localScale = Vector3.one * 0.75f;

            scissorsSwingEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/MercSwordFinisherSlash.prefab").WaitForCompletion().InstantiateClone("ScissorSwing2");
            scissorsSwingEffect.AddComponent<NetworkIdentity>();
            scissorsSwingEffect.transform.GetChild(0).gameObject.SetActive(value: false);
            scissorsSwingEffect.transform.GetChild(1).localScale = Vector3.one * 0.75f;
            //second swing
            scissorsSwingEffect2 = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/MercSwordFinisherSlash.prefab").WaitForCompletion().InstantiateClone("ScissorSwing2");
            scissorsSwingEffect2.AddComponent<NetworkIdentity>();
            scissorsSwingEffect2.transform.GetChild(0).gameObject.SetActive(value: false);
            material = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Merc/matOmniRadialSlash1Merc.mat").WaitForCompletion());
            material.SetColor("_TintColor", Color.red);
            scissorsSwingEffect2.transform.GetChild(1).gameObject.GetComponent<ParticleSystemRenderer>().material = material;
            scissorsSwingEffect2.transform.GetChild(1).localScale = Vector3.one * 0.75f;

            expungeSlashEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/MercSwordSlashWhirlwind.prefab").WaitForCompletion().InstantiateClone("ExpungeSlash");
            expungeSlashEffect.AddComponent<NetworkIdentity>();
            expungeSlashEffect.transform.GetChild(0).gameObject.GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpSwipe.mat").WaitForCompletion();
            expungeSlashEffect.transform.GetChild(0).localRotation = new Quaternion(0f, 90f, 90f, expungeSlashEffect.transform.GetChild(0).localRotation.w);
            expungeSlashEffect.transform.GetChild(0).localScale = Vector3.one * 2.5f;
            var fard = expungeSlashEffect.transform.Find("SwingTrail").GetComponent<ParticleSystem>().main;
            fard.startLifetimeMultiplier = 1f;

            expungeSlashEffect2 = expungeSlashEffect.InstantiateClone("ExpungeSlash2");
            expungeSlashEffect.transform.GetChild(0).localRotation = new Quaternion(0f, 180f, 180f, expungeSlashEffect.transform.GetChild(0).localRotation.w);

            expungeSlashEffect3 = expungeSlashEffect.InstantiateClone("ExpungeSlash3");
            expungeSlashEffect.transform.GetChild(0).localRotation = new Quaternion(90f, 180f, 180f, expungeSlashEffect.transform.GetChild(0).localRotation.w);

            //final hit
            /*
            scissorsComboSwingEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/MercSwordFinisherSlash.prefab").WaitForCompletion().InstantiateClone("ScissorSwing4");
            scissorsComboSwingEffect.AddComponent<NetworkIdentity>();
            scissorsComboSwingEffect.transform.GetChild(0).gameObject.SetActive(value: false);
            //ParticleSystem.MainModule main2 = scissorsComboSwingEffect.transform.GetChild(1).gameObject.GetComponent<ParticleSystem>().main;
            //main2.startLifetimeMultiplier = 0.1f;
            scissorsComboSwingEffect.transform.GetChild(1).localScale = Vector3.one * 1.0f;
            UnityEngine.Object.Destroy(scissorsComboSwingEffect.GetComponent<EffectComponent>());
            */

            //final hit butchered
            scissorsButcheredComboSwingEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/MercSwordFinisherSlash.prefab").WaitForCompletion().InstantiateClone("ScissorSwing3");
            scissorsButcheredComboSwingEffect.AddComponent<NetworkIdentity>();
            scissorsButcheredComboSwingEffect.transform.GetChild(0).gameObject.SetActive(value: false);
            scissorsButcheredComboSwingEffect.transform.GetChild(1).gameObject.GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpSwipe.mat").WaitForCompletion();
            //ParticleSystem.MainModule main = scissorsButcheredComboSwingEffect.transform.GetChild(1).gameObject.GetComponent<ParticleSystem>().main;
            //main.startLifetimeMultiplier = 0.1f;
            scissorsButcheredComboSwingEffect.transform.GetChild(1).localScale = Vector3.one * 1.5f;
            UnityEngine.Object.Destroy(scissorsButcheredComboSwingEffect.GetComponent<EffectComponent>());

            scissorsHitImpactEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/OmniImpactVFXSlashMerc.prefab").WaitForCompletion().InstantiateClone("ScissorImpact", false);
            scissorsHitImpactEffect.AddComponent<NetworkIdentity>();
            scissorsHitImpactEffect.GetComponent<OmniEffect>().enabled = false;
            material = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Merc/matOmniHitspark3Merc.mat").WaitForCompletion());
            material.SetColor("_TintColor", Color.white);
            scissorsHitImpactEffect.transform.GetChild(1).gameObject.GetComponent<ParticleSystemRenderer>().material = material;
            scissorsHitImpactEffect.transform.GetChild(2).localScale = Vector3.one * 1.5f;
            scissorsHitImpactEffect.transform.GetChild(2).gameObject.GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/VoidSurvivor/matVoidSurvivorBlasterFireCorrupted.mat").WaitForCompletion();
            material = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Merc/matOmniRadialSlash1Merc.mat").WaitForCompletion());
            material.SetColor("_TintColor", Color.white);
            scissorsHitImpactEffect.transform.GetChild(5).gameObject.GetComponent<ParticleSystemRenderer>().material = material;
            scissorsHitImpactEffect.transform.GetChild(4).localScale = Vector3.one * 3f;
            scissorsHitImpactEffect.transform.GetChild(1).localScale = Vector3.one * 1.5f;
            scissorsHitImpactEffect.transform.GetChild(1).gameObject.SetActive(value: true);
            scissorsHitImpactEffect.transform.GetChild(2).gameObject.SetActive(value: true);
            scissorsHitImpactEffect.transform.GetChild(3).gameObject.SetActive(value: true);
            scissorsHitImpactEffect.transform.GetChild(4).gameObject.SetActive(value: true);
            scissorsHitImpactEffect.transform.GetChild(5).gameObject.SetActive(value: true);
            scissorsHitImpactEffect.transform.GetChild(6).gameObject.SetActive(value: true);
            scissorsHitImpactEffect.transform.GetChild(6).GetChild(0).gameObject.SetActive(value: true);
            scissorsHitImpactEffect.transform.GetChild(6).transform.localScale = new Vector3(1f, 1f, 3f);
            scissorsHitImpactEffect.transform.localScale = Vector3.one * 1.5f;
            Modules.Content.CreateAndAddEffectDef(scissorsHitImpactEffect);

            scissorsButcheredHitImpactEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/OmniImpactVFXSlashMerc.prefab").WaitForCompletion().InstantiateClone("ScissorImpact", false);
            scissorsButcheredHitImpactEffect.AddComponent<NetworkIdentity>();
            scissorsButcheredHitImpactEffect.GetComponent<OmniEffect>().enabled = false;
            material = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Merc/matOmniHitspark3Merc.mat").WaitForCompletion());
            material.SetColor("_TintColor", Color.red);
            scissorsButcheredHitImpactEffect.transform.GetChild(1).gameObject.GetComponent<ParticleSystemRenderer>().material = material;
            scissorsButcheredHitImpactEffect.transform.GetChild(2).localScale = Vector3.one * 1.5f;
            scissorsButcheredHitImpactEffect.transform.GetChild(2).gameObject.GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/VoidSurvivor/matVoidSurvivorBlasterFireCorrupted.mat").WaitForCompletion();
            material = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Merc/matOmniRadialSlash1Merc.mat").WaitForCompletion());
            material.SetColor("_TintColor", Color.red);
            scissorsButcheredHitImpactEffect.transform.GetChild(5).gameObject.GetComponent<ParticleSystemRenderer>().material = material;
            scissorsButcheredHitImpactEffect.transform.GetChild(4).localScale = Vector3.one * 3f;
            scissorsButcheredHitImpactEffect.transform.GetChild(4).gameObject.GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpDust.mat").WaitForCompletion();
            scissorsButcheredHitImpactEffect.transform.GetChild(6).GetChild(0).gameObject.GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/Common/Void/matOmniHitspark1Void.mat").WaitForCompletion();
            scissorsButcheredHitImpactEffect.transform.GetChild(6).gameObject.GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/Common/Void/matOmniHitspark2Void.mat").WaitForCompletion();
            scissorsButcheredHitImpactEffect.transform.GetChild(1).localScale = Vector3.one * 1.5f;
            scissorsButcheredHitImpactEffect.transform.GetChild(1).gameObject.SetActive(value: true);
            scissorsButcheredHitImpactEffect.transform.GetChild(2).gameObject.SetActive(value: true);
            scissorsButcheredHitImpactEffect.transform.GetChild(3).gameObject.SetActive(value: true);
            scissorsButcheredHitImpactEffect.transform.GetChild(4).gameObject.SetActive(value: true);
            scissorsButcheredHitImpactEffect.transform.GetChild(5).gameObject.SetActive(value: true);
            scissorsButcheredHitImpactEffect.transform.GetChild(6).gameObject.SetActive(value: true);
            scissorsButcheredHitImpactEffect.transform.GetChild(6).GetChild(0).gameObject.SetActive(value: true);
            scissorsButcheredHitImpactEffect.transform.GetChild(6).transform.localScale = new Vector3(1f, 1f, 3f);
            scissorsButcheredHitImpactEffect.transform.localScale = Vector3.one * 1.5f;
            Modules.Content.CreateAndAddEffectDef(scissorsButcheredHitImpactEffect);

            sewEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/BleedOnHitAndExplode/BleedOnHitAndExplode_Impact.prefab").WaitForCompletion().InstantiateClone("SewSplosion");
            sewEffect.AddComponent<NetworkIdentity>();
            material = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Merc/matMercSwipe1.mat").WaitForCompletion());
            sewEffect.transform.GetChild(0).localScale = Vector3.one * 1f;
            sewEffect.transform.GetChild(0).GetComponent<ParticleSystemRenderer>().material = material;
            sewEffect.transform.GetChild(1).localScale = Vector3.one * 1f;
            sewEffect.transform.GetChild(1).GetComponent<ParticleSystemRenderer>().material = material;
            sewEffect.transform.GetChild(2).localScale = Vector3.one * 1f;
            sewEffect.transform.GetChild(2).GetComponent<ParticleSystemRenderer>().material = material;

            sewButcheredEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/BleedOnHitAndExplode/BleedOnHitAndExplode_Impact.prefab").WaitForCompletion().InstantiateClone("SewSplosion");
            sewButcheredEffect.AddComponent<NetworkIdentity>();
            material = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpSwipe.mat").WaitForCompletion());
            sewButcheredEffect.transform.GetChild(0).localScale = Vector3.one * 1f;
            sewButcheredEffect.transform.GetChild(0).GetComponent<ParticleSystemRenderer>().material = material;
            sewButcheredEffect.transform.GetChild(1).localScale = Vector3.one * 1f;
            sewButcheredEffect.transform.GetChild(1).GetComponent<ParticleSystemRenderer>().material = material;
            sewButcheredEffect.transform.GetChild(2).localScale = Vector3.one * 1f;
            sewButcheredEffect.transform.GetChild(2).GetComponent<ParticleSystemRenderer>().material = material;

            weaveDash = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/MercAssaulterEffect.prefab").WaitForCompletion().InstantiateClone("WeaveDash");
            weaveDash.AddComponent<NetworkIdentity>();
            material = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpSwipe.mat").WaitForCompletion());
            material.SetColor("_TintColor", Color.white);
            weaveDash.transform.GetChild(5).gameObject.SetActive(value: false);
            weaveDash.transform.GetChild(6).gameObject.SetActive(value: false);
            weaveDash.transform.GetChild(9).gameObject.GetComponent<TrailRenderer>().material = material;
            weaveDash.transform.GetChild(10).GetChild(0).gameObject.GetComponent<TrailRenderer>().material = material;
            weaveDash.transform.GetChild(10).GetChild(1).gameObject.GetComponent<TrailRenderer>().material = material;
            material = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Merc/matMercIgnition.mat").WaitForCompletion());
            material.SetColor("_TintColor", Color.red);
            weaveDash.transform.GetChild(10).GetChild(2).gameObject.GetComponent<TrailRenderer>().material = material;
            weaveDash.transform.GetChild(10).GetChild(3).gameObject.GetComponent<TrailRenderer>().material = material;

            weaveDashButchered = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/MercAssaulterEffect.prefab").WaitForCompletion().InstantiateClone("WeaveDashButchered");
            weaveDashButchered.AddComponent<NetworkIdentity>();
            material = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Merc/matMercIgnition.mat").WaitForCompletion());
            material.SetColor("_TintColor", new Color(155f / 255f, 55f / 255f, 55f / 255f));
            weaveDashButchered.transform.GetChild(5).gameObject.SetActive(value: false);
            weaveDashButchered.transform.GetChild(6).gameObject.SetActive(value: false);
            weaveDashButchered.transform.GetChild(9).gameObject.GetComponent<TrailRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpSwipe.mat").WaitForCompletion();
            weaveDashButchered.transform.GetChild(10).GetChild(0).gameObject.GetComponent<TrailRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpSwipe.mat").WaitForCompletion();
            weaveDashButchered.transform.GetChild(10).GetChild(1).gameObject.GetComponent<TrailRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpSwipe.mat").WaitForCompletion();
            weaveDashButchered.transform.GetChild(10).GetChild(2).gameObject.GetComponent<TrailRenderer>().material = material;
            weaveDashButchered.transform.GetChild(10).GetChild(3).gameObject.GetComponent<TrailRenderer>().material = material;

            expungeEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/LunarSkillReplacements/LunarDetonatorConsume.prefab").WaitForCompletion().InstantiateClone("WeaveReset");
            expungeEffect.AddComponent<NetworkIdentity>();
            material = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Merc/matMercExposedBackdrop.mat").WaitForCompletion());
            material.SetColor("_TintColor", Color.red);
            expungeEffect.transform.GetChild(0).gameObject.GetComponent<ParticleSystemRenderer>().material = material;
            expungeEffect.transform.GetChild(1).gameObject.SetActive(value: false);
            expungeEffect.transform.GetChild(2).gameObject.GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpSwipe.mat").WaitForCompletion();
            expungeEffect.transform.GetChild(3).gameObject.GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpSwipe.mat").WaitForCompletion();
            expungeEffect.transform.GetChild(3).gameObject.transform.localScale = new Vector3(.25f, .25f, .25f);
            expungeEffect.transform.GetChild(4).gameObject.SetActive(value: false);
            material = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/LunarSkillReplacements/matLunarNeedleImpactEffect.mat").WaitForCompletion());
            material.SetColor("_TintColor", Color.red);
            expungeEffect.transform.GetChild(5).gameObject.GetComponent<ParticleSystemRenderer>().material = material;
            expungeEffect.transform.GetChild(5).gameObject.transform.localScale = new Vector3(.25f, .25f, .25f);
            expungeEffect.transform.GetChild(6).gameObject.SetActive(value: false);

            reapBleedEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/LunarSkillReplacements/LunarDetonatorConsume.prefab").WaitForCompletion().InstantiateClone("WeaveReset");
            reapBleedEffect.AddComponent<NetworkIdentity>();
            material = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Merc/matMercExposedBackdrop.mat").WaitForCompletion());
            material.SetColor("_TintColor", Color.red);
            reapBleedEffect.transform.GetChild(0).gameObject.GetComponent<ParticleSystemRenderer>().material = material;
            reapBleedEffect.transform.GetChild(1).gameObject.GetComponent<ParticleSystemRenderer>().material.SetColor("_TintColor", Color.red);
            reapBleedEffect.transform.GetChild(2).gameObject.GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpSwipe.mat").WaitForCompletion();
            reapBleedEffect.transform.GetChild(3).gameObject.SetActive(value: false);
            reapBleedEffect.transform.GetChild(4).gameObject.SetActive(value: false);
            material = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/LunarSkillReplacements/matLunarNeedleImpactEffect.mat").WaitForCompletion());
            material.SetColor("_TintColor", Color.red);
            reapBleedEffect.transform.GetChild(5).gameObject.GetComponent<ParticleSystemRenderer>().material = material;
            reapBleedEffect.transform.GetChild(6).gameObject.SetActive(value: false);

            reapEndEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/LunarSkillReplacements/LunarDetonatorConsume.prefab").WaitForCompletion().InstantiateClone("WeaveResetEnd");
            reapEndEffect.AddComponent<NetworkIdentity>();
            material = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Merc/matMercExposedBackdrop.mat").WaitForCompletion());
            material.SetColor("_TintColor", Color.white);
            reapEndEffect.transform.GetChild(0).gameObject.GetComponent<ParticleSystemRenderer>().material = material;
            reapEndEffect.transform.GetChild(1).gameObject.GetComponent<ParticleSystemRenderer>().material.SetColor("_TintColor", Color.white);
            reapEndEffect.transform.GetChild(2).gameObject.GetComponent<ParticleSystemRenderer>().material.SetColor("_Color", Color.white);
            reapEndEffect.transform.GetChild(3).gameObject.SetActive(value: false);
            reapEndEffect.transform.GetChild(4).gameObject.SetActive(value: false);
            material = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/LunarSkillReplacements/matLunarNeedleImpactEffect.mat").WaitForCompletion());
            material.SetColor("_TintColor", Color.white);
            reapEndEffect.transform.GetChild(5).gameObject.GetComponent<ParticleSystemRenderer>().material = material;
            reapEndEffect.transform.GetChild(6).gameObject.SetActive(value: false);

        }

        #endregion effects

        #region projectiles
        private static void CreateProjectiles()
        {
            CreateNeedle ();
            Content.AddProjectilePrefab(needlePrefab);
            CreateEmpoweredNeedle();
            Content.AddProjectilePrefab(needleButcheredPrefab);
        }
        private static void CreateNeedle()
        {
            needlePrefab = Assets.CloneProjectilePrefab("FMJ", "Needle");

            ProjectileSimple needleSimple = needlePrefab.GetComponent<ProjectileSimple>();
            needleSimple.desiredForwardSpeed = 150f;
            needleSimple.lifetime = 5f;
            needleSimple.updateAfterFiring = true;
            
            ProjectileDamage needleDamage = needlePrefab.GetComponent<ProjectileDamage>();
            needleDamage.damageType = DamageType.Generic;
            DamageAPI.ModdedDamageTypeHolderComponent needleModdedDamage = needlePrefab.AddComponent<DamageAPI.ModdedDamageTypeHolderComponent>();
            needleModdedDamage.Add(DamageTypes.StitchDamage);
            needleModdedDamage.Add(DamageTypes.BeginHoming);

            needlePrefab.AddComponent<ProjectileTargetComponent>();
            ProjectileSteerTowardTarget needleSteer = needlePrefab.AddComponent<ProjectileSteerTowardTarget>();
            needleSteer.yAxisOnly = false;
            needleSteer.rotationSpeed = 700f;

            ProjectileOverlapAttack needleLap = needlePrefab.GetComponent<ProjectileOverlapAttack>();
            needleLap.resetInterval = 0.5f;
            needleLap.overlapProcCoefficient = SeamstressStaticValues.sewNeedleDamageCoefficient;

            ProjectileHealOwnerOnDamageInflicted needleHeal = needlePrefab.AddComponent<ProjectileHealOwnerOnDamageInflicted>();
            needleHeal.fractionOfDamage = SeamstressStaticValues.needleHealAmount;

            ProjectileController needleController = needlePrefab.GetComponent<ProjectileController>();
            needleController.procCoefficient = 1f;
            needleController.allowPrediction = false;
            needleGhost = RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/MageIceBombProjectile").GetComponent<ProjectileController>().ghostPrefab;
            Material material2 = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Mage/matMageMatrixDirectionalIce.mat").WaitForCompletion());
            material2.SetColor("_TintColor", Color.white);
            Material material = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Mage/matMageIceCore.mat").WaitForCompletion());
            material.SetColor("_TintColor", Color.white);
            needleGhost.transform.GetChild(2).localScale = new Vector3(0.1f, 0.1f, 0.705f);
            needleGhost.transform.GetChild(2).gameObject.GetComponent<MeshRenderer>().material = material;
            needleGhost.transform.GetChild(4).localScale = new Vector3(.5f, .5f, .5f);
            needleGhost.transform.GetChild(4).GetChild(0).gameObject.GetComponent<TrailRenderer>().material = material2;
            needleGhost.transform.GetChild(4).GetChild(1).gameObject.GetComponent<TrailRenderer>().material = material2;
            needleGhost.transform.GetChild(4).GetChild(2).gameObject.SetActive(value: false);
            needleGhost.transform.GetChild(4).GetChild(3).gameObject.SetActive(value: false);
            needleGhost = PrefabAPI.InstantiateClone(needleGhost, "Needle");
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
            needleButcheredPrefab = Assets.CloneProjectilePrefab("FMJ", "NeedleButchered");

            ProjectileSimple needleSimple = needleButcheredPrefab.GetComponent<ProjectileSimple>();
            needleSimple.desiredForwardSpeed = 150f;
            needleSimple.lifetime = 5f;
            needleSimple.updateAfterFiring = true;

            ProjectileDamage needleDamage = needleButcheredPrefab.GetComponent<ProjectileDamage>();
            needleDamage.damageType = DamageType.SlowOnHit;
            DamageAPI.ModdedDamageTypeHolderComponent needleModdedDamage = needleButcheredPrefab.AddComponent<DamageAPI.ModdedDamageTypeHolderComponent>();
            needleModdedDamage.Add(DamageTypes.StitchDamage);
            needleModdedDamage.Add(DamageTypes.BeginHoming);

            needleButcheredPrefab.AddComponent<ProjectileTargetComponent>();
            ProjectileSteerTowardTarget needleSteer = needleButcheredPrefab.AddComponent<ProjectileSteerTowardTarget>();
            needleSteer.yAxisOnly = false;
            needleSteer.rotationSpeed = 700f;

            ProjectileOverlapAttack needleLap = needleButcheredPrefab.GetComponent<ProjectileOverlapAttack>();
            needleLap.resetInterval = 0.5f;
            needleLap.overlapProcCoefficient = SeamstressStaticValues.sewNeedleDamageCoefficient;

            ProjectileHealOwnerOnDamageInflicted needleHeal = needleButcheredPrefab.AddComponent<ProjectileHealOwnerOnDamageInflicted>();
            needleHeal.fractionOfDamage = SeamstressStaticValues.needleHealAmount;

            ProjectileController needleController = needleButcheredPrefab.GetComponent<ProjectileController>();
            needleController.procCoefficient = 1f;
            needleController.allowPrediction = false;
            needleButcheredGhost = RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/MageIceBombProjectile").GetComponent<ProjectileController>().ghostPrefab;
            Material material = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Mage/matMageMatrixDirectionalIce.mat").WaitForCompletion());
            material.SetColor("_TintColor", Color.red);
            Material material3 = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Mage/matMageIceCore.mat").WaitForCompletion());
            material3.SetColor("_Color", Color.red);
            needleButcheredGhost.transform.GetChild(2).localScale = new Vector3(0.1f, 0.1f, 0.705f);
            needleButcheredGhost.transform.GetChild(2).gameObject.GetComponent<MeshRenderer>().material = material3;
            needleButcheredGhost.transform.GetChild(4).localScale = new Vector3(.5f, .5f, .5f);
            needleButcheredGhost.transform.GetChild(3).gameObject.SetActive(value: false);
            needleButcheredGhost.transform.GetChild(4).GetChild(3).gameObject.SetActive(value: false);
            needleButcheredGhost.transform.GetChild(4).GetChild(0).gameObject.GetComponent<TrailRenderer>().material = material;
            needleButcheredGhost.transform.GetChild(4).GetChild(1).gameObject.GetComponent<TrailRenderer>().material = material;
            needleButcheredGhost.transform.GetChild(4).GetChild(2).gameObject.SetActive(value: false);
            needleButcheredGhost.transform.GetChild(4).GetChild(3).gameObject.SetActive(value: false);
            needleButcheredGhost = PrefabAPI.InstantiateClone(needleButcheredGhost, "NeedleButchered");
            if (RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/MageIceBombProjectile") != null)
                needleController.ghostPrefab = needleButcheredGhost;
            if(!needleController.ghostPrefab.GetComponent<NetworkIdentity>()) 
                needleController.ghostPrefab.AddComponent<NetworkIdentity>();
            if(!needleController.ghostPrefab.GetComponent<ProjectileGhostController>()) 
                needleController.ghostPrefab.AddComponent<ProjectileGhostController>();
            needleController.startSound = "";
        }
        #endregion projectiles
    }
}
