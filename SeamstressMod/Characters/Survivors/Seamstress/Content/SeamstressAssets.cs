using RoR2;
using UnityEngine;
using SeamstressMod.Modules;
using System;
using RoR2.Projectile;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using R2API;

namespace SeamstressMod.Survivors.Seamstress
{
    public static class SeamstressAssets
    {
        //effects
        internal static GameObject scissorsSwingEffect;
        internal static GameObject sewSwingEffect;
        internal static GameObject scissorsComboSwingEffect;
        internal static GameObject scissorsHitImpactEffect;
        // particle effects

        // networked hit sounds
        internal static NetworkSoundEventDef scissorsHitSoundEvent;

        internal static NetworkSoundEventDef sewHitSoundEvent;

        private static AssetBundle _assetBundle;
        //crosshairs
        public static GameObject crosshairOverridePrefab;

        internal static GameObject needlePrefab;

        internal static GameObject needlePrefabEmpowered;
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
            crosshairOverridePrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Loader/LoaderCrosshair.prefab").WaitForCompletion().InstantiateClone("SeamstressCrosshair");
            crosshairOverridePrefab.AddComponent<NetworkIdentity>();

            scissorsSwingEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/MercSwordFinisherSlash.prefab").WaitForCompletion().InstantiateClone("ScissorSwing");
            scissorsSwingEffect.AddComponent<NetworkIdentity>();
            scissorsSwingEffect.transform.GetChild(0).gameObject.SetActive(value: false);
            scissorsSwingEffect.transform.GetChild(1).gameObject.GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpSwipe.mat").WaitForCompletion();//Assets.LoadEffect("HenrySwordSwingEffect", true);

            scissorsComboSwingEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/MercSwordSlashWhirlwind.prefab").WaitForCompletion().InstantiateClone("ScissorSwing");
            scissorsComboSwingEffect.AddComponent<NetworkIdentity>();
            scissorsComboSwingEffect.transform.GetChild(0).gameObject.GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpSwipe.mat").WaitForCompletion();
            ParticleSystem.MainModule main = scissorsComboSwingEffect.transform.GetChild(0).gameObject.GetComponent<ParticleSystem>().main;
            main.startLifetimeMultiplier = 0.6f;
            scissorsComboSwingEffect.transform.GetChild(0).localScale = Vector3.one * 2f;
            UnityEngine.Object.Destroy(scissorsComboSwingEffect.GetComponent<EffectComponent>());

            scissorsHitImpactEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/OmniImpactVFXSlashMerc.prefab").WaitForCompletion().InstantiateClone("ScissorImpact", false);
            scissorsHitImpactEffect.AddComponent<NetworkIdentity>();
            scissorsHitImpactEffect.GetComponent<OmniEffect>().enabled = false;
            Material material = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Merc/matOmniHitspark3Merc.mat").WaitForCompletion());
            material.SetColor("_TintColor", Color.red);
            scissorsHitImpactEffect.transform.GetChild(1).gameObject.GetComponent<ParticleSystemRenderer>().material = material;
            scissorsHitImpactEffect.transform.GetChild(2).localScale = Vector3.one * 1.5f;
            scissorsHitImpactEffect.transform.GetChild(2).gameObject.GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/VoidSurvivor/matVoidSurvivorBlasterFireCorrupted.mat").WaitForCompletion();
            Material material2 = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Merc/matOmniRadialSlash1Merc.mat").WaitForCompletion());
            material2.SetColor("_TintColor", Color.red);
            scissorsHitImpactEffect.transform.GetChild(5).gameObject.GetComponent<ParticleSystemRenderer>().material = material2;
            scissorsHitImpactEffect.transform.GetChild(4).localScale = Vector3.one * 3f;
            scissorsHitImpactEffect.transform.GetChild(4).gameObject.GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpDust.mat").WaitForCompletion();
            scissorsHitImpactEffect.transform.GetChild(6).GetChild(0).gameObject.GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/Common/Void/matOmniHitspark1Void.mat").WaitForCompletion();
            scissorsHitImpactEffect.transform.GetChild(6).gameObject.GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/Common/Void/matOmniHitspark2Void.mat").WaitForCompletion();
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

            sewSwingEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Croco/CrocoSlash.prefab").WaitForCompletion().InstantiateClone("SewSwing");
            sewSwingEffect.AddComponent<NetworkIdentity>();
            sewSwingEffect.transform.GetChild(0).gameObject.SetActive(value: false);
            sewSwingEffect.transform.GetChild(1).gameObject.GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpSwipe.mat").WaitForCompletion();//Assets.LoadEffect("HenrySwordSwingEffect", true);
        }

        #endregion effects

        #region projectiles
        private static void CreateProjectiles()
        {
            CreateNeedle ();
            Content.AddProjectilePrefab(needlePrefab);
            CreateNeedleEmpowered();
            Content.AddProjectilePrefab(needlePrefabEmpowered);
        }
        private static void CreateNeedle()
        {
            needlePrefab = Assets.CloneProjectilePrefab("FMJ", "Needle");

            ProjectileSimple needleSimple = needlePrefab.GetComponent<ProjectileSimple>();
            needleSimple.desiredForwardSpeed = 100f;
            needleSimple.lifetime = 5f;
            needleSimple.updateAfterFiring = true;

            ProjectileDamage needleDamage = needlePrefab.GetComponent<ProjectileDamage>();
            needleDamage.damageType = DamageType.Generic;

            needlePrefab.AddComponent<ProjectileTargetComponent>();
            ProjectileSteerTowardTarget needleSteer = needlePrefab.AddComponent<ProjectileSteerTowardTarget>();
            needleSteer.yAxisOnly = false;
            needleSteer.rotationSpeed = 600f;

            ProjectileDirectionalTargetFinder needleFinder = needlePrefab.AddComponent<ProjectileDirectionalTargetFinder>();
            needleFinder.lookRange = 50f;   //25f
            needleFinder.lookCone = 120f;    //20f
            needleFinder.targetSearchInterval = 0.2f;
            needleFinder.onlySearchIfNoTarget = false;
            needleFinder.allowTargetLoss = true;
            needleFinder.testLoS = true;
            needleFinder.ignoreAir = false;
            needleFinder.flierAltitudeTolerance = Mathf.Infinity;

            ProjectileHealOwnerOnDamageInflicted needleHeal = needlePrefab.AddComponent<ProjectileHealOwnerOnDamageInflicted>();
            needleHeal.fractionOfDamage = 0.1f;

            ProjectileController needleController = needlePrefab.GetComponent<ProjectileController>();
            needleController.allowPrediction = false;

            if (_assetBundle.LoadAsset<GameObject>("MageIceBombProjectile") != null) 
                needleController.ghostPrefab = _assetBundle.CreateProjectileGhostPrefab("MageIceBombProjectile");
            needleController.startSound = "";
        }
        private static void CreateNeedleEmpowered()
        {
            needlePrefabEmpowered = Assets.CloneProjectilePrefab("FMJ", "Needle2");

            ProjectileSimple needleSimple = needlePrefabEmpowered.GetComponent<ProjectileSimple>();
            needleSimple.desiredForwardSpeed = 100f;
            needleSimple.lifetime = 5f;
            needleSimple.updateAfterFiring = true;

            ProjectileDamage needleDamage = needlePrefabEmpowered.GetComponent<ProjectileDamage>();
            needleDamage.damageType = DamageType.BleedOnHit;

            needlePrefabEmpowered.AddComponent<ProjectileTargetComponent>();
            ProjectileSteerTowardTarget needleSteer = needlePrefabEmpowered.AddComponent<ProjectileSteerTowardTarget>();
            needleSteer.yAxisOnly = false;
            needleSteer.rotationSpeed = 600f;

            ProjectileDirectionalTargetFinder needleFinder = needlePrefabEmpowered.AddComponent<ProjectileDirectionalTargetFinder>();
            needleFinder.lookRange = 50f;   //25f
            needleFinder.lookCone = 120f;    //20f
            needleFinder.targetSearchInterval = 0.1f;
            needleFinder.onlySearchIfNoTarget = false;
            needleFinder.allowTargetLoss = true;
            needleFinder.testLoS = true;
            needleFinder.ignoreAir = false;
            needleFinder.flierAltitudeTolerance = Mathf.Infinity;

            ProjectileHealOwnerOnDamageInflicted needleHeal = needlePrefabEmpowered.AddComponent<ProjectileHealOwnerOnDamageInflicted>();
            needleHeal.fractionOfDamage = 0.2f;

            ProjectileController needleController = needlePrefabEmpowered.GetComponent<ProjectileController>();
            needleController.allowPrediction = false;

            if (_assetBundle.LoadAsset<GameObject>("MageIceBombProjectile") != null) needleController.ghostPrefab = _assetBundle.CreateProjectileGhostPrefab("MageIceBombProjectile");
            needleController.startSound = "";
        }
        #endregion projectiles
    }
}
