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
        public static GameObject swordSwingEffect;
        public static GameObject swordHitImpactEffect;

        public static GameObject bombExplosionEffect;

        // networked hit sounds
        internal static NetworkSoundEventDef scissorsHitSoundEvent;

        internal static NetworkSoundEventDef sewHitSoundEvent;
        //projectiles
        public static GameObject bombProjectilePrefab;

        internal static GameObject deathOrb;

        private static AssetBundle _assetBundle;
        //crosshairs
        public static GameObject crosshairOverridePrefab;

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
            CreateBombExplosionEffect();
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

            sewSwingEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Croco/CrocoComboFinisherSlash.prefab").WaitForCompletion().InstantiateClone("SewSwing");
            sewSwingEffect.AddComponent<NetworkIdentity>();
            sewSwingEffect.transform.GetChild(0).gameObject.SetActive(value: false);
            sewSwingEffect.transform.GetChild(1).gameObject.GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpSwipe.mat").WaitForCompletion();//Assets.LoadEffect("HenrySwordSwingEffect", true);

            deathOrb = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/Effects/OrbEffects/InfusionOrbEffect"), "WeaveOrbEffect", true);
            if (!deathOrb.GetComponent<NetworkIdentity>())
            {
                deathOrb.AddComponent<NetworkIdentity>();
            }
            deathOrb.transform.Find("VFX").Find("Core").GetComponent<ParticleSystemRenderer>()
                .material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matBloodHumanLarge.mat").WaitForCompletion();
            deathOrb.transform.Find("VFX").localScale = Vector3.one * 0.5f;
            deathOrb.transform.Find("VFX").Find("Core").localScale = Vector3.one * 4.5f;
            deathOrb.transform.Find("VFX").Find("PulseGlow").GetComponent<ParticleSystemRenderer>()
                .material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matOmniRing2Generic.mat").WaitForCompletion();
            Modules.Content.CreateAndAddEffectDef(deathOrb);
        }

        private static void CreateBombExplosionEffect()
        {
            bombExplosionEffect = _assetBundle.LoadEffect("BombExplosionEffect", "SeamstressBombExplosion");

            if (!bombExplosionEffect)
                return;

            ShakeEmitter shakeEmitter = bombExplosionEffect.AddComponent<ShakeEmitter>();
            shakeEmitter.amplitudeTimeDecay = true;
            shakeEmitter.duration = 0.5f;
            shakeEmitter.radius = 200f;
            shakeEmitter.scaleShakeRadiusWithLocalScale = false;

            shakeEmitter.wave = new Wave
            {
                amplitude = 1f,
                frequency = 40f,
                cycleOffset = 0f
            };

        }
        #endregion effects

        #region projectiles
        private static void CreateProjectiles()
        {

            CreateBombProjectile();
            Content.AddProjectilePrefab(bombProjectilePrefab);
        }

        private static void CreateBombProjectile()
        {
            //highly recommend setting up projectiles in editor, but this is a quick and dirty way to prototype if you want //todo guide
            bombProjectilePrefab = Assets.CloneProjectilePrefab("CommandoGrenadeProjectile", "SeamstressBombProjectile");

            //remove their ProjectileImpactExplosion component and start from default values
            UnityEngine.Object.DestroyImmediate(bombProjectilePrefab.GetComponent<ProjectileImpactExplosion>());
            ProjectileImpactExplosion bombImpactExplosion = bombProjectilePrefab.AddComponent<ProjectileImpactExplosion>();
            
            bombImpactExplosion.blastRadius = 16f;
            bombImpactExplosion.blastDamageCoefficient = 1f;
            bombImpactExplosion.falloffModel = BlastAttack.FalloffModel.None;
            bombImpactExplosion.destroyOnEnemy = true;
            bombImpactExplosion.lifetime = 12f;
            bombImpactExplosion.impactEffect = bombExplosionEffect;
            bombImpactExplosion.lifetimeExpiredSound = Content.CreateAndAddNetworkSoundEventDef("SeamstressBombExplosion");
            bombImpactExplosion.timerAfterImpact = true;
            bombImpactExplosion.lifetimeAfterImpact = 0.1f;

            ProjectileController bombController = bombProjectilePrefab.GetComponent<ProjectileController>();

            if (_assetBundle.LoadAsset<GameObject>("SeamstressBombGhost") != null)
                bombController.ghostPrefab = _assetBundle.CreateProjectileGhostPrefab("SeamstressBombGhost");
            
            bombController.startSound = "";
        }
        #endregion projectiles
    }
}
