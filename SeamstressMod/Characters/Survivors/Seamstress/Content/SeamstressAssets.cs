using RoR2;
using UnityEngine;
using SeamstressMod.Modules;
using System;
using RoR2.Projectile;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using R2API;
using System.Runtime.CompilerServices;
using UnityEngine.UIElements;

namespace SeamstressMod.Survivors.Seamstress
{
    public static class SeamstressAssets
    {
        //effects
        internal static GameObject scissorsSwingEffect;
        internal static GameObject scissorsButcheredSwingEffect;
        internal static GameObject sewEffect;
        internal static GameObject scissorsComboSwingEffect;
        internal static GameObject scissorsButcheredComboSwingEffect;
        internal static GameObject scissorsHitImpactEffect;
        internal static GameObject scissorsButcheredHitImpactEffect;
        internal static GameObject needleGhost;
        // particle effects

        // networked hit sounds
        internal static NetworkSoundEventDef scissorsHitSoundEvent;

        internal static NetworkSoundEventDef sewHitSoundEvent;

        private static AssetBundle _assetBundle;
        //crosshairs
        public static GameObject crosshairOverridePrefab;

        internal static GameObject needlePrefab;
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

            scissorsButcheredSwingEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/MercSwordFinisherSlash.prefab").WaitForCompletion().InstantiateClone("ScissorSwing");
            scissorsButcheredSwingEffect.AddComponent<NetworkIdentity>();
            scissorsButcheredSwingEffect.transform.GetChild(0).gameObject.SetActive(value: false);
            scissorsButcheredSwingEffect.transform.GetChild(1).gameObject.GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpSwipe.mat").WaitForCompletion();//Assets.LoadEffect("HenrySwordSwingEffect", true);

            scissorsSwingEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/MercSwordFinisherSlash.prefab").WaitForCompletion().InstantiateClone("ScissorSwing");
            scissorsSwingEffect.AddComponent<NetworkIdentity>();
            scissorsSwingEffect.transform.GetChild(0).gameObject.SetActive(value: false);
            //final hit
            scissorsButcheredComboSwingEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/MercSwordFinisherSlash.prefab").WaitForCompletion().InstantiateClone("ScissorSwing");
            scissorsButcheredComboSwingEffect.AddComponent<NetworkIdentity>();
            scissorsButcheredComboSwingEffect.transform.GetChild(0).gameObject.SetActive(value: false);
            scissorsButcheredComboSwingEffect.transform.GetChild(1).gameObject.GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpSwipe.mat").WaitForCompletion();
            ParticleSystem.MainModule main = scissorsButcheredComboSwingEffect.transform.GetChild(1).gameObject.GetComponent<ParticleSystem>().main;
            main.startLifetimeMultiplier = 0.1f;
            scissorsButcheredComboSwingEffect.transform.GetChild(1).localScale = Vector3.one * 0.75f;
            UnityEngine.Object.Destroy(scissorsButcheredComboSwingEffect.GetComponent<EffectComponent>());
            //final hit
            scissorsComboSwingEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/MercSwordFinisherSlash.prefab").WaitForCompletion().InstantiateClone("ScissorSwing");
            scissorsComboSwingEffect.AddComponent<NetworkIdentity>();
            scissorsComboSwingEffect.transform.GetChild(0).gameObject.SetActive(value: false);
            ParticleSystem.MainModule main2 = scissorsComboSwingEffect.transform.GetChild(1).gameObject.GetComponent<ParticleSystem>().main;
            main2.startLifetimeMultiplier = 0.1f;
            scissorsComboSwingEffect.transform.GetChild(1).localScale = Vector3.one * 0.75f;
            UnityEngine.Object.Destroy(scissorsComboSwingEffect.GetComponent<EffectComponent>());

            scissorsHitImpactEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/OmniImpactVFXSlashMerc.prefab").WaitForCompletion().InstantiateClone("ScissorImpact", false);
            scissorsHitImpactEffect.AddComponent<NetworkIdentity>();
            scissorsHitImpactEffect.GetComponent<OmniEffect>().enabled = false;
            Material material = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Merc/matOmniHitspark3Merc.mat").WaitForCompletion());
            material.SetColor("_TintColor", Color.white);
            scissorsHitImpactEffect.transform.GetChild(1).gameObject.GetComponent<ParticleSystemRenderer>().material = material;
            scissorsHitImpactEffect.transform.GetChild(2).localScale = Vector3.one * 1.5f;
            scissorsHitImpactEffect.transform.GetChild(2).gameObject.GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/VoidSurvivor/matVoidSurvivorBlasterFireCorrupted.mat").WaitForCompletion();
            Material material2 = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Merc/matOmniRadialSlash1Merc.mat").WaitForCompletion());
            material2.SetColor("_TintColor", Color.white);
            scissorsHitImpactEffect.transform.GetChild(5).gameObject.GetComponent<ParticleSystemRenderer>().material = material2;
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
            material2 = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Merc/matOmniRadialSlash1Merc.mat").WaitForCompletion());
            material2.SetColor("_TintColor", Color.red);
            scissorsButcheredHitImpactEffect.transform.GetChild(5).gameObject.GetComponent<ParticleSystemRenderer>().material = material2;
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

            sewEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ImpBoss/ImpBossBlink.prefab").WaitForCompletion().InstantiateClone("SewSplosion");
            sewEffect.AddComponent<NetworkIdentity>();
            sewEffect.transform.GetChild(0).gameObject.SetActive(value: false);
        }

        #endregion effects

        #region projectiles
        private static void CreateProjectiles()
        {
            CreateNeedle ();
            Content.AddProjectilePrefab(needlePrefab);
        }
        private static void CreateNeedle()
        {
            needlePrefab = Assets.CloneProjectilePrefab("FMJ", "Needle");

            ProjectileSimple needleSimple = needlePrefab.GetComponent<ProjectileSimple>();
            needleSimple.desiredForwardSpeed = 100f;
            needleSimple.lifetime = 3f;
            needleSimple.updateAfterFiring = true;
            
            ProjectileDamage needleDamage = needlePrefab.GetComponent<ProjectileDamage>();
            needleDamage.damageType = DamageType.Generic;
            DamageAPI.ModdedDamageTypeHolderComponent needleModdedDamage = needlePrefab.AddComponent<DamageAPI.ModdedDamageTypeHolderComponent>();
            needleModdedDamage.Add(DamageTypes.CutDamageNeedle);

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
            needleHeal.fractionOfDamage = 0f;

            ProjectileController needleController = needlePrefab.GetComponent<ProjectileController>();
            needleController.procCoefficient = 0.5f;
            needleController.allowPrediction = false;
            needleGhost = RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/MageIceBombProjectile").GetComponent<ProjectileController>().ghostPrefab;
            needleGhost = PrefabAPI.InstantiateClone(needleGhost, "Needle");
            if (_assetBundle.LoadAsset<GameObject>("HenryBombGhost") != null)
                needleController.ghostPrefab = needleGhost;
            needleController.startSound = "";
        }
        #endregion projectiles
    }
}
