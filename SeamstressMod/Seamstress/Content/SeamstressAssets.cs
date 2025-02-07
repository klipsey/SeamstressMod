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
using UnityEngine.UIElements;
using System.IO;
using System.Reflection;

namespace SeamstressMod.Seamstress.Content
{
    public static class SeamstressAssets
    {
        //AssetBundle
        internal static AssetBundle mainAssetBundle;

        //Shader
        internal static Shader hotpoo = Resources.Load<Shader>("Shaders/Deferred/HGStandard");
        
        //Effects

        internal static GameObject spawnPrefab;
        internal static GameObject spawnPrefabBlue;

        internal static GameObject parrySlashEffect;
        internal static GameObject parrySlashEffectBlue;

        internal static GameObject wideSlashEffect;
        internal static GameObject wideSlashEffectBlue;

        internal static GameObject uppercutEffect;
        internal static GameObject uppercutEffectBlue;

        internal static GameObject clipSlashEffect;
        internal static GameObject clipSlashEffectBlue;

        internal static GameObject pickupScissorEffectDefault;
        internal static GameObject pickupScissorEffectBlue;

        internal static GameObject scissorsSlashEffect;
        internal static GameObject scissorsSlashEffectBlue;

        internal static GameObject scissorsSlashComboEffect;
        internal static GameObject scissorsSlashComboEffectBlue;

        internal static GameObject clawSlashEffect;
        internal static GameObject clawSlashEffectBlue;

        internal static GameObject clawSlashComboEffect;
        internal static GameObject clawSlashComboEffectBlue;

        internal static GameObject blinkEffectDefault;
        internal static GameObject blinkEffectBlue;

        internal static GameObject impDashEffect;
        internal static GameObject impDashEffectBlue;

        internal static GameObject smallBlinkEffect;
        internal static GameObject smallBlinkEffectBlue;

        internal static GameObject insatiableEndEffect;
        internal static GameObject insatiableEndEffectBlue;

        internal static GameObject scissorsHitImpactEffect;
        internal static GameObject scissorsHitImpactEffectBlue;

        internal static GameObject slamEffect;
        internal static GameObject slamEffectBlue;

        internal static GameObject impactExplosionEffectDefault;
        internal static GameObject impactExplosionEffectBlue;

        internal static GameObject bloodSplatterEffect;
        internal static GameObject bloodSplatterEffectBlue;

        internal static GameObject sewnCdEffect;
        internal static GameObject sewnEffect;
        internal static GameObject sewnEffectBlue;

        internal static GameObject scissorTrailEffectDefault;
        internal static GameObject trailEffectHandsDefault;
        internal static GameObject scissorTrailEffectBlue;
        internal static GameObject trailEffectHandsBlue;

        internal static GameObject bloodExplosionEffect;
        internal static GameObject bloodExplosionEffectBlue;
        internal static GameObject bloodSpurtEffect;
        internal static GameObject bloodSpurtEffectBlue;

        internal static GameObject flashRed;
        internal static GameObject flashBlue;

        internal static GameObject longLifeTrails;
        internal static GameObject longLifeTrailsBlue;

        internal static GameObject spikeDash;
        internal static GameObject spikeDashBlue;

        //Misc Prefabs
        internal static TeamAreaIndicator seamstressTeamAreaIndicator;
        internal static TeamAreaIndicator seamstressTeamAreaIndicatorBlue;

        internal static GameObject telekinesisTracker;
        internal static GameObject telekinesisCdTracker;

        internal static GameObject chainToHeart;
        internal static GameObject heartPrefab;

        internal static GameObject chainToHeartBlue;
        internal static GameObject heartPrefabBlue;
        //Overlay Effects
        internal static GameObject bleedEffect;

        //Materials
        internal static Material destealthMaterial;
        internal static Material destealthMaterialBlue;
        internal static Material insatiableOverlayMat;
        internal static Material insatiableOverlayMatBlue;
        internal static Material parryMat;
        internal static Material commandoMat;
        internal static Material mercMat;

        //Networked Hit Sounds
        internal static NetworkSoundEventDef scissorsHitSoundEvent;
        internal static NetworkSoundEventDef parrySuccessSoundEvent;

        //Projectiles
        internal static GameObject needlePrefab;
        internal static GameObject needlePrefabBlue;
        internal static GameObject needleGhostDefault;

        internal static GameObject needleGhostBlue;

        internal static GameObject scissorRPrefab;
        internal static GameObject scissorRGhostDefault;
        internal static GameObject scissorLPrefab;
        internal static GameObject scissorLGhostDefault;

        internal static GameObject scissorRGhostSword;
        internal static GameObject scissorLGhostSword;

        internal static GameObject scissorRGhostSwordAlt;
        internal static GameObject scissorLGhostSwordAlt;

        internal static GameObject scissorRGhostRaven;
        internal static GameObject scissorLGhostRaven;
        //Colors
        internal static Color coolRed = new Color(84f / 255f, 0f / 255f, 11f / 255f);
        internal static Color theRed = new Color(155f / 255f, 55f / 255f, 55f / 255f);
        public static void Init(AssetBundle assetBundle)
        {
            mainAssetBundle = assetBundle;

            CreateMaterials();

            CreateEffects();

            CreateProjectiles();

            CreateHeart();

            CreateHeartBlue();

            CreateSounds();
        }

        private static void CreateMaterials()
        {
            insatiableOverlayMat = Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/DLC1/VoidSurvivor/matVoidSurvivorCorruptOverlay.mat").WaitForCompletion());

            insatiableOverlayMatBlue = Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/DLC1/VoidSurvivor/matVoidSurvivorCorruptOverlay.mat").WaitForCompletion());
            insatiableOverlayMatBlue.SetTexture("_RemapTex", Addressables.LoadAssetAsync<Texture>("RoR2/Base/Common/ColorRamps/texRampBombOrb.png").WaitForCompletion());

            destealthMaterial = Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpBossDissolve.mat").WaitForCompletion();

            destealthMaterialBlue = Addressables.LoadAssetAsync<Material>("RoR2/Base/Merc/matMercEvisTarget.mat").WaitForCompletion();

            parryMat = Addressables.LoadAssetAsync<Material>("RoR2/Base/CritOnUse/matFullCrit.mat").WaitForCompletion();

            mercMat = Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Merc/matMercSwipe1.mat").WaitForCompletion());
        }

        #region effects
        private static void CreateEffects()
        {
            spawnPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ImpBoss/ImpBossDeathEffect.prefab").WaitForCompletion().InstantiateClone("SpawnEffect");
            spawnPrefab.AddComponent<NetworkIdentity>();
            spawnPrefab.GetComponent<EffectComponent>().applyScale = true;
            spawnPrefab.transform.Find("DashRings").localScale *= 0.75f;
            Modules.Content.CreateAndAddEffectDef(spawnPrefab);

            spawnPrefabBlue = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ImpBoss/ImpBossDeathEffect.prefab").WaitForCompletion().InstantiateClone("SpawnEffect2");
            spawnPrefabBlue.AddComponent<NetworkIdentity>();
            spawnPrefabBlue.GetComponent<EffectComponent>().applyScale = true;
            spawnPrefabBlue.transform.Find("DashRings").localScale *= 0.75f;
            spawnPrefabBlue.transform.Find("NoiseTrails").gameObject.GetComponent<ParticleSystemRenderer>().material.SetTexture("_RemapTex", Addressables.LoadAssetAsync<Texture>("RoR2/Base/Common/ColorRamps/texRampLightning.png").WaitForCompletion());
            spawnPrefabBlue.transform.Find("Dash").gameObject.GetComponent<ParticleSystemRenderer>().material.SetTexture("_RemapTex", Addressables.LoadAssetAsync<Texture>("RoR2/Base/Common/ColorRamps/texRampLightning.png").WaitForCompletion());
            spawnPrefabBlue.transform.Find("DashRings").gameObject.GetComponent<ParticleSystemRenderer>().material = mercMat;
            spawnPrefabBlue.transform.Find("Ring").gameObject.GetComponent<ParticleSystemRenderer>().material.SetTexture("_RemapTex", Addressables.LoadAssetAsync<Texture>("RoR2/Base/Common/ColorRamps/texRampLightning.png").WaitForCompletion());
            spawnPrefabBlue.transform.Find("Point Light").gameObject.GetComponent<Light>().color = Color.cyan;

            GameObject.Destroy(spawnPrefabBlue.transform.Find("Flash, Red").gameObject);
            GameObject.Destroy(spawnPrefabBlue.transform.Find("PP").gameObject);
            Modules.Content.CreateAndAddEffectDef(spawnPrefabBlue);

            bloodExplosionEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ImpBoss/ImpBossBlink.prefab").WaitForCompletion().InstantiateClone("SeamstressBloodExplosion", false);

            Material bloodMat = Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matBloodHumanLarge.mat").WaitForCompletion());
            Material bloodMat2 = Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/moon2/matBloodSiphon.mat").WaitForCompletion());

            bloodExplosionEffect.transform.Find("Particles/LongLifeNoiseTrails").GetComponent<ParticleSystemRenderer>().material = bloodMat;
            bloodExplosionEffect.transform.Find("Particles/LongLifeNoiseTrails, Bright").GetComponent<ParticleSystemRenderer>().material = bloodMat;
            bloodExplosionEffect.transform.Find("Particles/Dash").GetComponent<ParticleSystemRenderer>().material = bloodMat;
            bloodExplosionEffect.transform.Find("Particles/Dash, Bright").GetComponent<ParticleSystemRenderer>().material = bloodMat;
            bloodExplosionEffect.transform.Find("Particles/DashRings").GetComponent<ParticleSystemRenderer>().material = bloodMat2;
            bloodExplosionEffect.GetComponentInChildren<Light>().gameObject.SetActive(false);

            bloodExplosionEffect.GetComponentInChildren<PostProcessVolume>().sharedProfile = Addressables.LoadAssetAsync<PostProcessProfile>("RoR2/Base/title/ppLocalGold.asset").WaitForCompletion();

            Modules.Content.CreateAndAddEffectDef(bloodExplosionEffect);

            bloodSpurtEffect = mainAssetBundle.LoadAsset<GameObject>("BloodSpurtEffect");

            bloodSpurtEffect.transform.Find("Blood").GetComponent<ParticleSystemRenderer>().material = bloodMat2;
            bloodSpurtEffect.transform.Find("Trails").GetComponent<ParticleSystemRenderer>().trailMaterial = bloodMat2;

            bloodExplosionEffectBlue = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ImpBoss/ImpBossBlink.prefab").WaitForCompletion().InstantiateClone("SeamstressBloodExplosion2", false);

            Material bloodMat3 = Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matBloodHumanLarge.mat").WaitForCompletion());
            Material bloodMat4 = Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/moon2/matBloodSiphon.mat").WaitForCompletion());

            bloodExplosionEffectBlue.transform.Find("Particles/LongLifeNoiseTrails").GetComponent<ParticleSystemRenderer>().material = bloodMat3;
            bloodExplosionEffectBlue.transform.Find("Particles/LongLifeNoiseTrails").GetComponent<ParticleSystemRenderer>().material.SetTexture("_RemapTex", Addressables.LoadAssetAsync<Texture>("RoR2/Base/Common/ColorRamps/texRampHuntressSoft.png").WaitForCompletion());
            bloodExplosionEffectBlue.transform.Find("Particles/LongLifeNoiseTrails, Bright").GetComponent<ParticleSystemRenderer>().material = bloodMat3;
            bloodExplosionEffectBlue.transform.Find("Particles/LongLifeNoiseTrails, Bright").GetComponent<ParticleSystemRenderer>().material.SetTexture("_RemapTex", Addressables.LoadAssetAsync<Texture>("RoR2/Base/Common/ColorRamps/texRampHuntressSoft.png").WaitForCompletion());

            bloodExplosionEffectBlue.transform.Find("Particles/Dash").GetComponent<ParticleSystemRenderer>().material = bloodMat3;
            bloodExplosionEffectBlue.transform.Find("Particles/Dash").GetComponent<ParticleSystemRenderer>().material.SetTexture("_RemapTex", Addressables.LoadAssetAsync<Texture>("RoR2/Base/Common/ColorRamps/texRampHuntressSoft.png").WaitForCompletion());
            bloodExplosionEffectBlue.transform.Find("Particles/Dash, Bright").GetComponent<ParticleSystemRenderer>().material = bloodMat3;
            bloodExplosionEffectBlue.transform.Find("Particles/Dash, Bright").GetComponent<ParticleSystemRenderer>().material.SetTexture("_RemapTex", Addressables.LoadAssetAsync<Texture>("RoR2/Base/Common/ColorRamps/texRampHuntressSoft.png").WaitForCompletion());
            bloodExplosionEffectBlue.transform.Find("Particles/DashRings").GetComponent<ParticleSystemRenderer>().material = bloodMat4;
            bloodExplosionEffectBlue.transform.Find("Particles/DashRings").GetComponent<ParticleSystemRenderer>().material.SetColor("_TintColor", Color.cyan);
            bloodExplosionEffectBlue.GetComponentInChildren<Light>().gameObject.SetActive(false);

            bloodExplosionEffectBlue.GetComponentInChildren<PostProcessVolume>().sharedProfile = Addressables.LoadAssetAsync<PostProcessProfile>("RoR2/Base/title/ppLocalGold.asset").WaitForCompletion();

            Modules.Content.CreateAndAddEffectDef(bloodExplosionEffectBlue);

            bloodSpurtEffectBlue = mainAssetBundle.LoadAsset<GameObject>("BloodSpurtEffect");

            bloodSpurtEffectBlue.transform.Find("Blood").GetComponent<ParticleSystemRenderer>().material = bloodMat4;
            bloodSpurtEffectBlue.transform.Find("Blood").GetComponent<ParticleSystemRenderer>().material.SetColor("_TintColor", Color.cyan);
            bloodSpurtEffectBlue.transform.Find("Trails").GetComponent<ParticleSystemRenderer>().trailMaterial = bloodMat4;
            bloodSpurtEffectBlue.transform.Find("Trails").GetComponent<ParticleSystemRenderer>().trailMaterial.SetColor("_TintColor", Color.cyan);

            bleedEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/BleedEffect.prefab").WaitForCompletion().InstantiateClone("StitchEffect");
            bleedEffect.AddComponent<NetworkIdentity>();
            bleedEffect.transform.GetChild(1).gameObject.GetComponent<ParticleSystemRenderer>().material.SetColor("_EmissionColor", coolRed);

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

            sewnEffectBlue = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Nullifier/NullifyStack3Effect.prefab").WaitForCompletion().InstantiateClone("SewnYes2");
            sewnEffectBlue.AddComponent<NetworkIdentity>();
            sewnEffectBlue.transform.GetChild(0).GetChild(0).gameObject.GetComponent<MeshRenderer>().materials[0].SetColor("_TintColor", Color.cyan);
            sewnEffectBlue.transform.GetChild(0).GetChild(1).gameObject.GetComponent<MeshRenderer>().materials[0].SetColor("_TintColor", Color.cyan);
            sewnEffectBlue.transform.GetChild(0).GetChild(2).gameObject.GetComponent<MeshRenderer>().materials[0].SetColor("_TintColor", Color.cyan);

            parrySlashEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidJailer/VoidJailerCaptureTracer.prefab").WaitForCompletion().InstantiateClone("ParrySlashEffect");
            parrySlashEffect.AddComponent<NetworkIdentity>();
            parrySlashEffect.transform.gameObject.GetComponent<EffectComponent>().soundName = "Play_huntress_R_snipe_shoot";
            parrySlashEffect.transform.GetChild(2).GetChild(0).gameObject.GetComponent<ParticleSystemRenderer>().material.SetColor("_TintColor", coolRed);
            parrySlashEffect.transform.GetChild(3).gameObject.GetComponent<LineRenderer>().material.SetColor("_TintColor", coolRed);
            Modules.Content.CreateAndAddEffectDef(parrySlashEffect);

            parrySlashEffectBlue = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidJailer/VoidJailerCaptureTracer.prefab").WaitForCompletion().InstantiateClone("ParrySlashEffect2");
            parrySlashEffectBlue.AddComponent<NetworkIdentity>();
            parrySlashEffectBlue.transform.gameObject.GetComponent<EffectComponent>().soundName = "Play_huntress_R_snipe_shoot";
            parrySlashEffectBlue.transform.GetChild(2).GetChild(0).gameObject.GetComponent<ParticleSystemRenderer>().material.SetColor("_TintColor", Color.cyan);
            parrySlashEffectBlue.transform.GetChild(3).gameObject.GetComponent<LineRenderer>().material.SetColor("_TintColor", Color.cyan);
            Modules.Content.CreateAndAddEffectDef(parrySlashEffectBlue);

            telekinesisTracker = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Huntress/HuntressTrackingIndicator.prefab").WaitForCompletion().InstantiateClone("SeamstressTracker", false);
            Material component = Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/UI/matUIOverbrighten2x.mat").WaitForCompletion());
            Object.DestroyImmediate(telekinesisTracker.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>());
            SpriteRenderer balls = telekinesisTracker.transform.GetChild(0).gameObject.AddComponent<SpriteRenderer>();
            balls.material = component;
            balls.sprite = mainAssetBundle.LoadAsset<Sprite>("Grab");
            telekinesisTracker.transform.GetChild(1).gameObject.SetActive(false);
            Sprite sprite = Addressables.LoadAssetAsync<Sprite>("RoR2/Base/UI/texCrosshair2.png").WaitForCompletion();
            Material component2 = telekinesisTracker.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().material;
            Object.DestroyImmediate(telekinesisTracker.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>());
            SpriteRenderer balls2 = telekinesisTracker.transform.GetChild(2).gameObject.AddComponent<SpriteRenderer>();
            balls2.material = component2;
            balls2.sprite = sprite;
            balls2.color = coolRed;

            telekinesisCdTracker = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Huntress/HuntressTrackingIndicator.prefab").WaitForCompletion().InstantiateClone("NotSeamstressTracker", false);
            component = Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/UI/matUIOverbrighten2x.mat").WaitForCompletion());
            Object.DestroyImmediate(telekinesisCdTracker.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>());
            balls = telekinesisCdTracker.transform.GetChild(0).gameObject.AddComponent<SpriteRenderer>();
            balls.material = component;
            balls.sprite = mainAssetBundle.LoadAsset<Sprite>("NoGrab");
            telekinesisCdTracker.transform.GetChild(1).gameObject.SetActive(false);
            sprite = Addressables.LoadAssetAsync<Sprite>("RoR2/Base/UI/texCrosshair2.png").WaitForCompletion();
            component2 = telekinesisCdTracker.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().material;
            Object.DestroyImmediate(telekinesisCdTracker.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>());
            balls2 = telekinesisCdTracker.transform.GetChild(2).gameObject.AddComponent<SpriteRenderer>();
            balls2.material = component2;
            balls2.sprite = sprite;
            balls2.color = coolRed;

            blinkEffectDefault = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ImpBoss/ImpBossBlink.prefab").WaitForCompletion().InstantiateClone("BlinkStart");
            blinkEffectDefault.AddComponent<NetworkIdentity>();
            blinkEffectDefault.GetComponent<EffectComponent>().applyScale = true;
            blinkEffectDefault.transform.GetChild(0).localScale = Vector3.one * 0.5f;
            blinkEffectDefault.transform.GetChild(1).localScale = Vector3.one * 0.5f;
            GameObject.DestroyImmediate(blinkEffectDefault.transform.Find("Particles").Find("Point light").gameObject);
            GameObject.DestroyImmediate(blinkEffectDefault.transform.Find("Particles").Find("Flash, Red").gameObject);
            GameObject.DestroyImmediate(blinkEffectDefault.transform.Find("Particles").Find("Flash, White").gameObject);
            GameObject.DestroyImmediate(blinkEffectDefault.transform.Find("Particles").Find("Distortion").gameObject);
            GameObject.DestroyImmediate(blinkEffectDefault.transform.Find("Particles").Find("Dash").gameObject);
            GameObject.DestroyImmediate(blinkEffectDefault.transform.Find("Particles").Find("Dash, Bright").gameObject);
            GameObject.DestroyImmediate(blinkEffectDefault.transform.Find("Particles").Find("LongLifeNoiseTrails").gameObject);
            GameObject.DestroyImmediate(blinkEffectDefault.transform.Find("Particles").Find("LongLifeNoiseTrails, Bright").gameObject);
            GameObject.DestroyImmediate(blinkEffectDefault.transform.Find("PP").gameObject);

            Modules.Content.CreateAndAddEffectDef(blinkEffectDefault);

            blinkEffectBlue = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ImpBoss/ImpBossBlink.prefab").WaitForCompletion().InstantiateClone("BlinkStart2");
            blinkEffectBlue.AddComponent<NetworkIdentity>();
            blinkEffectBlue.GetComponent<EffectComponent>().applyScale = true;
            blinkEffectBlue.transform.GetChild(0).localScale = Vector3.one * 0.5f;
            blinkEffectBlue.transform.GetChild(1).localScale = Vector3.one * 0.5f;
            blinkEffectBlue.transform.Find("Particles").Find("DashRings").gameObject.GetComponent<ParticleSystemRenderer>().material = mercMat;
            blinkEffectBlue.transform.Find("Particles").Find("Sphere").gameObject.GetComponent<ParticleSystemRenderer>().material.SetTexture("_RemapTex", Addressables.LoadAssetAsync<Texture>("RoR2/Base/Common/ColorRamps/texRampLunarElectric.png").WaitForCompletion());
            GameObject.DestroyImmediate(blinkEffectBlue.transform.Find("Particles").Find("Point light").gameObject);
            GameObject.DestroyImmediate(blinkEffectBlue.transform.Find("Particles").Find("Flash, Red").gameObject);
            GameObject.DestroyImmediate(blinkEffectBlue.transform.Find("Particles").Find("Flash, White").gameObject);
            GameObject.DestroyImmediate(blinkEffectBlue.transform.Find("Particles").Find("Distortion").gameObject);
            GameObject.DestroyImmediate(blinkEffectBlue.transform.Find("Particles").Find("Dash").gameObject);
            GameObject.DestroyImmediate(blinkEffectBlue.transform.Find("Particles").Find("Dash, Bright").gameObject);
            GameObject.DestroyImmediate(blinkEffectBlue.transform.Find("Particles").Find("LongLifeNoiseTrails").gameObject);
            GameObject.DestroyImmediate(blinkEffectBlue.transform.Find("Particles").Find("LongLifeNoiseTrails, Bright").gameObject);
            GameObject.DestroyImmediate(blinkEffectBlue.transform.Find("PP").gameObject);

            Modules.Content.CreateAndAddEffectDef(blinkEffectBlue);

            smallBlinkEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Imp/ImpBlinkEffect.prefab").WaitForCompletion().InstantiateClone("BlinkSmall");
            smallBlinkEffect.AddComponent<NetworkIdentity>();
            Modules.Content.CreateAndAddEffectDef(smallBlinkEffect);

            smallBlinkEffectBlue = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Imp/ImpBlinkEffect.prefab").WaitForCompletion().InstantiateClone("BlinkSmall2");
            smallBlinkEffectBlue.AddComponent<NetworkIdentity>();
            smallBlinkEffectBlue.transform.Find("Particles").Find("NoiseTrails").gameObject.GetComponent<ParticleSystemRenderer>().material = Object.Instantiate( Addressables.LoadAssetAsync<Material>("RoR2/Base/LunarGolem/matLunarGolemBlastDustLG.mat").WaitForCompletion());
            smallBlinkEffectBlue.transform.Find("Particles").Find("Flash, White").gameObject.GetComponent<ParticleSystemRenderer>().material = Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/bazaar/matSeerPortalEffectEdge.mat").WaitForCompletion());
            smallBlinkEffectBlue.transform.Find("Particles").Find("Point light").gameObject.GetComponent<Light>().color = Color.cyan;
            smallBlinkEffectBlue.transform.Find("Particles").Find("Dash").gameObject.GetComponent<ParticleSystemRenderer>().material = mercMat;
            Modules.Content.CreateAndAddEffectDef(smallBlinkEffectBlue);

            clawSlashEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Croco/CrocoSlash.prefab").WaitForCompletion().InstantiateClone("SeamstressSlash");
            clawSlashEffect.AddComponent<NetworkIdentity>();
            clawSlashEffect.transform.GetChild(0).localScale = new Vector3(1f, 1f, 1f);
            clawSlashEffect.GetComponent<ScaleParticleSystemDuration>().initialDuration = 0.5f;
            clawSlashEffect.transform.Find("SwingTrail").gameObject.GetComponent<ParticleSystemRenderer>().material = Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Croco/matCrocoSlash.mat").WaitForCompletion());
            clawSlashEffect.transform.Find("SwingTrail").gameObject.GetComponent<ParticleSystemRenderer>().material.SetTexture("_RemapTex", mainAssetBundle.LoadAsset<Texture>("texRampSeamstress"));

            clawSlashEffectBlue = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Croco/CrocoSlash.prefab").WaitForCompletion().InstantiateClone("SeamstressSlash2");
            clawSlashEffectBlue.AddComponent<NetworkIdentity>();
            clawSlashEffectBlue.transform.GetChild(0).localScale = new Vector3(1f, 1f, 1f);
            clawSlashEffectBlue.GetComponent<ScaleParticleSystemDuration>().initialDuration = 0.5f;
            clawSlashEffectBlue.transform.Find("SwingTrail").gameObject.GetComponent<ParticleSystemRenderer>().material = Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Croco/matCrocoSlash.mat").WaitForCompletion());
            clawSlashEffectBlue.transform.Find("SwingTrail").gameObject.GetComponent<ParticleSystemRenderer>().material.SetTexture("_RemapTex", Addressables.LoadAssetAsync<Texture>("RoR2/Base/Common/ColorRamps/texRampHuntressSoft2.png").WaitForCompletion());

            clawSlashComboEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Croco/CrocoComboFinisherSlash.prefab").WaitForCompletion().InstantiateClone("SeamstressComboSlash");
            clawSlashComboEffect.AddComponent<NetworkIdentity>();
            clawSlashComboEffect.transform.GetChild(0).localScale = new Vector3(1.25f, 1.25f, 1.25f);
            clawSlashComboEffect.GetComponent<ScaleParticleSystemDuration>().initialDuration = 0.5f;
            clawSlashComboEffect.transform.GetChild(0).GetComponent<ParticleSystemRenderer>().material = Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Croco/matCrocoSlash.mat").WaitForCompletion());
            clawSlashComboEffect.transform.Find("SwingTrail").gameObject.GetComponent<ParticleSystemRenderer>().material.SetTexture("_RemapTex", mainAssetBundle.LoadAsset<Texture>("texRampSeamstress"));
            clawSlashComboEffect.transform.GetChild(1).gameObject.SetActive(false);

            clawSlashComboEffectBlue = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Croco/CrocoComboFinisherSlash.prefab").WaitForCompletion().InstantiateClone("SeamstressComboSlash2");
            clawSlashComboEffectBlue.AddComponent<NetworkIdentity>();
            clawSlashComboEffectBlue.transform.GetChild(0).localScale = new Vector3(1.25f, 1.25f, 1.25f);
            clawSlashComboEffectBlue.GetComponent<ScaleParticleSystemDuration>().initialDuration = 0.5f;
            clawSlashComboEffectBlue.transform.GetChild(0).GetComponent<ParticleSystemRenderer>().material = Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Croco/matCrocoSlash.mat").WaitForCompletion());
            clawSlashComboEffectBlue.transform.Find("SwingTrail").gameObject.GetComponent<ParticleSystemRenderer>().material.SetTexture("_RemapTex", Addressables.LoadAssetAsync<Texture>("RoR2/Base/Common/ColorRamps/texRampHuntressSoft2.png").WaitForCompletion());
            clawSlashComboEffectBlue.transform.GetChild(1).gameObject.SetActive(false);

            scissorsSlashEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/MercSwordFinisherSlash.prefab").WaitForCompletion().InstantiateClone("ScissorSwing");
            scissorsSlashEffect.AddComponent<NetworkIdentity>();
            scissorsSlashEffect.transform.GetChild(0).gameObject.SetActive(false);
            scissorsSlashEffect.transform.GetChild(1).gameObject.GetComponent<ParticleSystemRenderer>().material = Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpSwipe.mat").WaitForCompletion());//Assets.LoadEffect("HenrySwordSwingEffect", true);
            var fard = scissorsSlashEffect.transform.GetChild(0).gameObject.GetComponent<ParticleSystem>().main;
            fard.startLifetimeMultiplier = 2f;

            scissorsSlashEffectBlue = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/MercSwordFinisherSlash.prefab").WaitForCompletion().InstantiateClone("ScissorSwingBlue");
            scissorsSlashEffectBlue.AddComponent<NetworkIdentity>();
            scissorsSlashEffectBlue.transform.GetChild(0).gameObject.SetActive(false);
            fard = scissorsSlashEffectBlue.transform.GetChild(0).gameObject.GetComponent<ParticleSystem>().main;
            fard.startLifetimeMultiplier = 2f;

            scissorsSlashComboEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/MercSwordFinisherSlash.prefab").WaitForCompletion().InstantiateClone("ScissorSwing3");
            scissorsSlashComboEffect.AddComponent<NetworkIdentity>();
            scissorsSlashComboEffect.transform.GetChild(0).gameObject.SetActive(false);
            scissorsSlashComboEffect.transform.GetChild(1).gameObject.GetComponent<ParticleSystemRenderer>().material = Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpSwipe.mat").WaitForCompletion());
            scissorsSlashComboEffect.transform.GetChild(1).localScale = new Vector3(1f, 1.5f, 1.5f);

            scissorsSlashComboEffectBlue = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/MercSwordFinisherSlash.prefab").WaitForCompletion().InstantiateClone("ScissorSwing3Blue");
            scissorsSlashComboEffectBlue.AddComponent<NetworkIdentity>();
            scissorsSlashComboEffectBlue.transform.GetChild(0).gameObject.SetActive(false);
            scissorsSlashComboEffectBlue.transform.GetChild(1).localScale = new Vector3(1f, 1.5f, 1.5f);

            clipSlashEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/MercSwordFinisherSlash.prefab").WaitForCompletion().InstantiateClone("ClipSwing");
            clipSlashEffect.AddComponent<NetworkIdentity>();
            clipSlashEffect.transform.GetChild(1).gameObject.GetComponent<ParticleSystemRenderer>().material = Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpSwipe.mat").WaitForCompletion());
            clipSlashEffect.transform.GetChild(1).localScale = new Vector3(0.5f, 0.75f, 0.5f);
            fard = clipSlashEffect.transform.GetChild(0).gameObject.GetComponent<ParticleSystem>().main;
            fard.startLifetimeMultiplier = 0.6f;

            clipSlashEffectBlue = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/MercSwordFinisherSlash.prefab").WaitForCompletion().InstantiateClone("ClipSwing2");
            clipSlashEffectBlue.AddComponent<NetworkIdentity>();
            clipSlashEffectBlue.transform.GetChild(1).localScale = new Vector3(0.5f, 0.75f, 0.5f);
            fard = clipSlashEffectBlue.transform.GetChild(0).gameObject.GetComponent<ParticleSystem>().main;
            fard.startLifetimeMultiplier = 0.6f;

            pickupScissorEffectDefault = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/MercSwordSlashWhirlwind.prefab").WaitForCompletion().InstantiateClone("ScissorPickupSwing");
            pickupScissorEffectDefault.AddComponent<NetworkIdentity>();
            pickupScissorEffectDefault.transform.GetChild(0).localScale *= 1.5f;
            pickupScissorEffectDefault.transform.GetChild(0).rotation = Quaternion.AngleAxis(90f,Vector3.left);
            pickupScissorEffectDefault.transform.GetChild(1).rotation = Quaternion.AngleAxis(90f, Vector3.left);
            pickupScissorEffectDefault.transform.GetChild(0).gameObject.GetComponent<ParticleSystemRenderer>().material = Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpSwipe.mat").WaitForCompletion());
            fard = pickupScissorEffectDefault.transform.GetChild(0).gameObject.GetComponent<ParticleSystem>().main;
            fard.startLifetimeMultiplier = 2f;
            Modules.Content.CreateAndAddEffectDef(pickupScissorEffectDefault);

            pickupScissorEffectBlue = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/MercSwordSlashWhirlwind.prefab").WaitForCompletion().InstantiateClone("ScissorPickupSwing2");
            pickupScissorEffectBlue.AddComponent<NetworkIdentity>();
            pickupScissorEffectBlue.transform.GetChild(0).localScale *= 1.5f;
            pickupScissorEffectBlue.transform.GetChild(0).rotation = Quaternion.AngleAxis(90f, Vector3.left);
            pickupScissorEffectBlue.transform.GetChild(1).rotation = Quaternion.AngleAxis(90f, Vector3.left);
            fard = pickupScissorEffectBlue.transform.GetChild(0).gameObject.GetComponent<ParticleSystem>().main;
            fard.startLifetimeMultiplier = 2f;
            Modules.Content.CreateAndAddEffectDef(pickupScissorEffectBlue);

            wideSlashEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/MercSwordSlashWhirlwind.prefab").WaitForCompletion().InstantiateClone("SeamstressWideSwordSwing");
            wideSlashEffect.AddComponent<NetworkIdentity>();
            wideSlashEffect.transform.GetChild(0).localScale *= 1.5f;
            wideSlashEffect.transform.GetChild(0).gameObject.GetComponent<ParticleSystemRenderer>().material = Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpSwipe.mat").WaitForCompletion());
            var sex = wideSlashEffect.transform.GetChild(0).gameObject.GetComponent<ParticleSystem>().main;
            sex.startLifetimeMultiplier = 0.6f;
            sex.startRotation3D = false;
            Modules.Content.CreateAndAddEffectDef(wideSlashEffect);

            wideSlashEffectBlue = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/MercSwordSlashWhirlwind.prefab").WaitForCompletion().InstantiateClone("SeamstressWideSwordSwing");
            wideSlashEffectBlue.AddComponent<NetworkIdentity>();
            wideSlashEffectBlue.transform.GetChild(0).localScale *= 1.5f;
            sex = wideSlashEffectBlue.transform.GetChild(0).gameObject.GetComponent<ParticleSystem>().main;
            sex.startLifetimeMultiplier = 0.6f;
            sex.startRotation3D = false;

            Modules.Content.CreateAndAddEffectDef(wideSlashEffectBlue);

            uppercutEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/MercSwordSlashWhirlwind.prefab").WaitForCompletion().InstantiateClone("SeamstressUppercut");
            uppercutEffect.AddComponent<NetworkIdentity>();
            uppercutEffect.transform.GetChild(0).gameObject.GetComponent<ParticleSystemRenderer>().material = Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpSwipe.mat").WaitForCompletion());
            sex = uppercutEffect.transform.GetChild(0).gameObject.GetComponent<ParticleSystem>().main;
            sex.startLifetimeMultiplier = 0.6f;
            sex.startRotation3D = false;
            Modules.Content.CreateAndAddEffectDef(uppercutEffect);

            uppercutEffectBlue = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/MercSwordSlashWhirlwind.prefab").WaitForCompletion().InstantiateClone("SeamstressUppercut2");
            uppercutEffectBlue.AddComponent<NetworkIdentity>();
            sex = uppercutEffectBlue.transform.GetChild(0).gameObject.GetComponent<ParticleSystem>().main;
            sex.startLifetimeMultiplier = 0.6f;
            sex.startRotation3D = false;
            Modules.Content.CreateAndAddEffectDef(uppercutEffectBlue);


            scissorsHitImpactEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/OmniImpactVFXSlashMerc.prefab").WaitForCompletion().InstantiateClone("ScissorImpact", false);
            scissorsHitImpactEffect.AddComponent<NetworkIdentity>();
            scissorsHitImpactEffect.GetComponent<OmniEffect>().enabled = false;
            Material material = Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Merc/matOmniHitspark3Merc.mat").WaitForCompletion());
            material.SetColor("_TintColor", Color.red);
            scissorsHitImpactEffect.transform.GetChild(1).gameObject.GetComponent<ParticleSystemRenderer>().material = material;
            scissorsHitImpactEffect.transform.GetChild(2).localScale = Vector3.one * 1.5f;
            scissorsHitImpactEffect.transform.GetChild(2).gameObject.GetComponent<ParticleSystemRenderer>().material = Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/DLC1/VoidSurvivor/matVoidSurvivorBlasterFireCorrupted.mat").WaitForCompletion());
            material = Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpSlashImpact.mat").WaitForCompletion());
            scissorsHitImpactEffect.transform.GetChild(5).gameObject.GetComponent<ParticleSystemRenderer>().material = material;
            scissorsHitImpactEffect.transform.GetChild(4).localScale = Vector3.one * 3f;
            scissorsHitImpactEffect.transform.GetChild(4).gameObject.GetComponent<ParticleSystemRenderer>().material = Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpDust.mat").WaitForCompletion());
            scissorsHitImpactEffect.transform.GetChild(6).GetChild(0).gameObject.GetComponent<ParticleSystemRenderer>().material = Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/DLC1/Common/Void/matOmniHitspark1Void.mat").WaitForCompletion());
            scissorsHitImpactEffect.transform.GetChild(6).gameObject.GetComponent<ParticleSystemRenderer>().material = Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/DLC1/Common/Void/matOmniHitspark2Void.mat").WaitForCompletion());
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

            scissorsHitImpactEffectBlue = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/OmniImpactVFXSlashMerc.prefab").WaitForCompletion().InstantiateClone("ScissorImpact2", false);
            scissorsHitImpactEffectBlue.AddComponent<NetworkIdentity>();
            scissorsHitImpactEffectBlue.GetComponent<OmniEffect>().enabled = false;
            scissorsHitImpactEffectBlue.transform.GetChild(1).gameObject.GetComponent<ParticleSystemRenderer>().material = Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Merc/matOmniHitspark3Merc.mat").WaitForCompletion());
            scissorsHitImpactEffectBlue.transform.GetChild(2).localScale = Vector3.one * 1.5f;
            scissorsHitImpactEffectBlue.transform.GetChild(4).localScale = Vector3.one * 3f;
            scissorsHitImpactEffectBlue.transform.GetChild(1).localScale = Vector3.one * 1.5f;
            scissorsHitImpactEffectBlue.transform.GetChild(1).gameObject.SetActive(true);
            scissorsHitImpactEffectBlue.transform.GetChild(2).gameObject.SetActive(true);
            scissorsHitImpactEffectBlue.transform.GetChild(3).gameObject.SetActive(true);
            scissorsHitImpactEffectBlue.transform.GetChild(4).gameObject.SetActive(true);
            scissorsHitImpactEffectBlue.transform.GetChild(5).gameObject.SetActive(true);
            scissorsHitImpactEffectBlue.transform.GetChild(6).gameObject.SetActive(true);
            scissorsHitImpactEffectBlue.transform.GetChild(6).GetChild(0).gameObject.SetActive(true);
            scissorsHitImpactEffectBlue.transform.GetChild(6).transform.localScale = new Vector3(1f, 1f, 3f);
            scissorsHitImpactEffectBlue.transform.localScale = Vector3.one * 1.5f;
            Modules.Content.CreateAndAddEffectDef(scissorsHitImpactEffectBlue);

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

            impDashEffectBlue = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Huntress/HuntressBlinkEffect.prefab").WaitForCompletion().InstantiateClone("ImpDash2");
            impDashEffectBlue.AddComponent<NetworkIdentity>();
            impDashEffectBlue.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
            impDashEffectBlue.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
            impDashEffectBlue.transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
            impDashEffectBlue.transform.GetChild(0).GetChild(3).gameObject.SetActive(false);
            SeamstressMod.Modules.Content.CreateAndAddEffectDef(impDashEffectBlue);

            insatiableEndEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/LunarSkillReplacements/LunarDetonatorConsume.prefab").WaitForCompletion().InstantiateClone("InsatiableEndEffect");
            insatiableEndEffect.AddComponent<NetworkIdentity>();
            var fart = insatiableEndEffect.transform.GetChild(0).gameObject.GetComponent<ParticleSystem>().main;
            fart.startColor = Color.black;
            fart = insatiableEndEffect.transform.GetChild(1).gameObject.GetComponent<ParticleSystem>().main;
            fart.startColor = Color.red;
            insatiableEndEffect.transform.GetChild(2).gameObject.GetComponent<ParticleSystemRenderer>().material.SetColor("_TintColor", Color.red);
            insatiableEndEffect.transform.GetChild(3).gameObject.SetActive(false);
            insatiableEndEffect.transform.GetChild(4).gameObject.GetComponent<ParticleSystemRenderer>().material.SetColor("_TintColor", Color.red);
            material = Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/LunarSkillReplacements/matLunarNeedleImpactEffect.mat").WaitForCompletion());
            material.SetColor("_TintColor", Color.red);
            insatiableEndEffect.transform.GetChild(5).gameObject.GetComponent<ParticleSystemRenderer>().material = material;
            insatiableEndEffect.transform.GetChild(6).gameObject.SetActive(false);
            SeamstressMod.Modules.Content.CreateAndAddEffectDef(insatiableEndEffect);

            insatiableEndEffectBlue = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/LunarSkillReplacements/LunarDetonatorConsume.prefab").WaitForCompletion().InstantiateClone("InsatiableEndEffect2");
            insatiableEndEffectBlue.AddComponent<NetworkIdentity>();
            fart = insatiableEndEffectBlue.transform.GetChild(0).gameObject.GetComponent<ParticleSystem>().main;
            fart.startColor = Color.black;
            fart = insatiableEndEffectBlue.transform.GetChild(1).gameObject.GetComponent<ParticleSystem>().main;
            fart.startColor = Color.cyan;
            insatiableEndEffectBlue.transform.GetChild(2).gameObject.GetComponent<ParticleSystemRenderer>().material.SetColor("_TintColor", Color.cyan);
            insatiableEndEffectBlue.transform.GetChild(3).gameObject.SetActive(false);
            insatiableEndEffectBlue.transform.GetChild(4).gameObject.GetComponent<ParticleSystemRenderer>().material.SetColor("_TintColor", Color.cyan);
            material = Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/LunarSkillReplacements/matLunarNeedleImpactEffect.mat").WaitForCompletion());
            material.SetColor("_TintColor", Color.cyan);
            insatiableEndEffectBlue.transform.GetChild(5).gameObject.GetComponent<ParticleSystemRenderer>().material = material;
            insatiableEndEffectBlue.transform.GetChild(6).gameObject.SetActive(false);
            SeamstressMod.Modules.Content.CreateAndAddEffectDef(insatiableEndEffectBlue);

            impactExplosionEffectDefault = CreateImpactExplosionEffect("SeamstressScissorImpact", Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matBloodGeneric.mat").WaitForCompletion(), 
                Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpDecal.mat").WaitForCompletion(), false, 2);
            Material blueMat = Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpDecal.mat").WaitForCompletion());
            blueMat.SetTexture("_RemapTex", Addressables.LoadAssetAsync<Texture>("RoR2/Base/Common/ColorRamps/texRampLightning.png").WaitForCompletion());
            impactExplosionEffectBlue = CreateImpactExplosionEffect("SeamstressScissorImpact2", Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matBloodGeneric.mat").WaitForCompletion(), 
                blueMat, true, 2);

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
            bloodSplatterEffect.transform.Find("Decal").GetComponent<Decal>().Material = Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpDecal.mat").WaitForCompletion());
            bloodSplatterEffect.transform.Find("Decal").GetComponent<AnimateShaderAlpha>().timeMax = 10f;
            bloodSplatterEffect.transform.GetChild(12).gameObject.SetActive(false);
            bloodSplatterEffect.transform.GetChild(13).gameObject.SetActive(false);
            bloodSplatterEffect.transform.GetChild(14).gameObject.SetActive(false);
            bloodSplatterEffect.transform.GetChild(15).gameObject.SetActive(false);
            bloodSplatterEffect.transform.localScale = Vector3.one;
            SeamstressMod.Modules.Content.CreateAndAddEffectDef(bloodSplatterEffect);

            bloodSplatterEffectBlue = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Brother/BrotherSlamImpact.prefab").WaitForCompletion().InstantiateClone("Splat", true);
            bloodSplatterEffectBlue.AddComponent<NetworkIdentity>();
            bloodSplatterEffectBlue.transform.GetChild(0).gameObject.SetActive(false);
            bloodSplatterEffectBlue.transform.GetChild(1).gameObject.SetActive(false);
            bloodSplatterEffectBlue.transform.GetChild(2).gameObject.SetActive(false);
            bloodSplatterEffectBlue.transform.GetChild(3).gameObject.SetActive(false);
            bloodSplatterEffectBlue.transform.GetChild(4).gameObject.SetActive(false);
            bloodSplatterEffectBlue.transform.GetChild(5).gameObject.SetActive(false);
            bloodSplatterEffectBlue.transform.GetChild(6).gameObject.SetActive(false);
            bloodSplatterEffectBlue.transform.GetChild(7).gameObject.SetActive(false);
            bloodSplatterEffectBlue.transform.GetChild(8).gameObject.SetActive(false);
            bloodSplatterEffectBlue.transform.GetChild(9).gameObject.SetActive(false);
            bloodSplatterEffectBlue.transform.GetChild(10).gameObject.SetActive(false);
            bloodSplatterEffectBlue.transform.Find("Decal").GetComponent<Decal>().Material = Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/CrippleWard/matLunarWardDecal.mat").WaitForCompletion());
            bloodSplatterEffectBlue.transform.Find("Decal").GetComponent<AnimateShaderAlpha>().timeMax = 10f;
            bloodSplatterEffectBlue.transform.GetChild(12).gameObject.SetActive(false);
            bloodSplatterEffectBlue.transform.GetChild(13).gameObject.SetActive(false);
            bloodSplatterEffectBlue.transform.GetChild(14).gameObject.SetActive(false);
            bloodSplatterEffectBlue.transform.GetChild(15).gameObject.SetActive(false);
            bloodSplatterEffectBlue.transform.localScale = Vector3.one;
            SeamstressMod.Modules.Content.CreateAndAddEffectDef(bloodSplatterEffectBlue);

            slamEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ImpBoss/ImpBossGroundSlam.prefab").WaitForCompletion().InstantiateClone("SeamstressSlamEffect");
            slamEffect.AddComponent<NetworkIdentity>();
            SeamstressMod.Modules.Content.CreateAndAddEffectDef(slamEffect);
            slamEffectBlue = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ImpBoss/ImpBossGroundSlam.prefab").WaitForCompletion().InstantiateClone("SeamstressSlamEffect2");
            slamEffectBlue.AddComponent<NetworkIdentity>();
            slamEffectBlue.transform.Find("Particles").Find("ClawMesh").gameObject.GetComponent<ParticleSystemRenderer>().material.SetTexture("_RemapTex", Addressables.LoadAssetAsync<Texture>("RoR2/Base/Common/ColorRamps/texRampLightning.png").WaitForCompletion());
            var main = slamEffectBlue.transform.Find("Particles").Find("Dust, Directional").gameObject.GetComponent<ParticleSystem>().main;
            main.startColor = Color.cyan;
            main = slamEffectBlue.transform.Find("Particles").Find("Dust, Billboard").gameObject.GetComponent<ParticleSystem>().main;
            main.startColor = Color.cyan;
            slamEffectBlue.transform.Find("Particles").Find("Dash").gameObject.GetComponent<ParticleSystemRenderer>().material = mercMat;
            slamEffectBlue.transform.Find("Particles").Find("DashRings").gameObject.GetComponent<ParticleSystemRenderer>().material = mercMat;
            slamEffectBlue.transform.Find("Particles").Find("Sphere").gameObject.GetComponent<ParticleSystemRenderer>().material.SetTexture("_RemapTex", Addressables.LoadAssetAsync<Texture>("RoR2/Base/Common/ColorRamps/texRampLunarElectric.png").WaitForCompletion());
            GameObject.DestroyImmediate(slamEffectBlue.transform.Find("PP").gameObject);
            GameObject.DestroyImmediate(slamEffectBlue.transform.Find("Particles").Find("Dash, Bright").gameObject);
            GameObject.DestroyImmediate(slamEffectBlue.transform.Find("Particles").Find("Point light").gameObject);
            GameObject.DestroyImmediate(slamEffectBlue.transform.Find("Particles").Find("Flash, White").gameObject);
            GameObject.DestroyImmediate(slamEffectBlue.transform.Find("Particles").Find("Flash, Red").gameObject);
            SeamstressMod.Modules.Content.CreateAndAddEffectDef(slamEffectBlue);

            GameObject impThing = LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/ImpVoidspikeProjectile");

            TeamAreaIndicator teamArea = PrefabAPI.InstantiateClone(impThing.transform.Find("ImpactEffect/TeamAreaIndicator, FullSphere").gameObject, "SeamstressTeamIndicator", false).GetComponent<TeamAreaIndicator>();
            
            teamArea.teamMaterialPairs[1].sharedMaterial = new Material(teamArea.teamMaterialPairs[1].sharedMaterial);
            teamArea.teamMaterialPairs[1].sharedMaterial.SetColor("_TintColor", Color.red);

            seamstressTeamAreaIndicator = teamArea;

            GameObject impThing2 = LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/ImpVoidspikeProjectile");

            TeamAreaIndicator teamArea2 = PrefabAPI.InstantiateClone(impThing2.transform.Find("ImpactEffect/TeamAreaIndicator, FullSphere").gameObject, "SeamstressTeamIndicator", false).GetComponent<TeamAreaIndicator>();

            teamArea2.teamMaterialPairs[1].sharedMaterial = new Material(teamArea2.teamMaterialPairs[1].sharedMaterial);
            teamArea2.teamMaterialPairs[1].sharedMaterial.SetColor("_TintColor", Color.cyan);
            seamstressTeamAreaIndicatorBlue = teamArea2;

            //Add this to her hands during insatiable?
            GameObject obj = new GameObject();
            scissorTrailEffectDefault = obj.InstantiateClone("ScissorTrail", false);
            TrailRenderer trail = scissorTrailEffectDefault.AddComponent<TrailRenderer>();
            trail.startWidth = 1f;
            trail.endWidth = 0f;
            trail.time = 0.5f;
            trail.emitting = true;
            trail.numCornerVertices = 0;
            trail.numCapVertices = 0;
            trail.material = Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matSmokeTrail.mat").WaitForCompletion());
            trail.material.SetTexture("_RemapTex", mainAssetBundle.LoadAsset<Texture>("texRampSeamstressTrail"));
            trail.alignment = LineAlignment.TransformZ;

            GameObject obj2 = new GameObject();
            trailEffectHandsDefault = obj2.InstantiateClone("SeamstressTrail", false);
            TrailRenderer trail2 = trailEffectHandsDefault.AddComponent<TrailRenderer>();
            trail2.startWidth = 0.3f;
            trail2.endWidth = 0f;
            trail2.time = 0.5f;
            trail2.emitting = true;
            trail2.numCornerVertices = 0;
            trail2.numCapVertices = 0;
            trail2.material = Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matSmokeTrail.mat").WaitForCompletion());
            trail2.material.SetTexture("_RemapTex", mainAssetBundle.LoadAsset<Texture>("texRampSeamstressTrail"));

            GameObject obj3 = new GameObject();
            scissorTrailEffectBlue = obj3.InstantiateClone("ScissorTrail2", false);
            TrailRenderer trail3 = scissorTrailEffectBlue.AddComponent<TrailRenderer>();
            trail3.startWidth = 1f;
            trail3.endWidth = 0f;
            trail3.time = 0.5f;
            trail3.emitting = true;
            trail3.numCornerVertices = 0;
            trail3.numCapVertices = 0;
            trail3.material = Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matSmokeTrail.mat").WaitForCompletion());
            trail3.material.SetTexture("_RemapTex", Addressables.LoadAssetAsync<Texture>("RoR2/Base/Common/ColorRamps/texRampLightning.png").WaitForCompletion());
            trail3.alignment = LineAlignment.TransformZ;

            GameObject obj4 = new GameObject();
            trailEffectHandsBlue = obj4.InstantiateClone("SeamstressTrail2", false);
            TrailRenderer trail4 = trailEffectHandsBlue.AddComponent<TrailRenderer>();
            trail4.startWidth = 0.3f;
            trail4.endWidth = 0f;
            trail4.time = 0.5f;
            trail4.emitting = true;
            trail4.numCornerVertices = 0;
            trail4.numCapVertices = 0;
            trail4.material = Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matSmokeTrail.mat").WaitForCompletion());
            trail4.material.SetTexture("_RemapTex", Addressables.LoadAssetAsync<Texture>("RoR2/Base/Common/ColorRamps/texRampLightning.png").WaitForCompletion());

            GameObject impSpike = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ImpBoss/ImpVoidspikeProjectile.prefab").WaitForCompletion();

            flashRed = impSpike.transform.Find("ImpactEffect").Find("Flash, Red").gameObject;

            flashBlue = impSpike.transform.Find("ImpactEffect").Find("Flash, Red").gameObject.InstantiateClone("Flash, Blue", false);
            var mainb = flashBlue.gameObject.GetComponent<ParticleSystem>().main;
            mainb.startColor = Color.cyan;

            longLifeTrails = impSpike.transform.Find("ImpactEffect").Find("LongLifeNoiseTrails").gameObject.InstantiateClone("LongLifeNoiseTrails", false);
            
            longLifeTrailsBlue = impSpike.transform.Find("ImpactEffect").Find("LongLifeNoiseTrails").gameObject.InstantiateClone("LongLifeNoiseTrailsBlue", false);
            longLifeTrailsBlue.GetComponent<ParticleSystemRenderer>().material.SetTexture("_RemapTex", Addressables.LoadAssetAsync<Texture>("RoR2/Base/Common/ColorRamps/texRampLightning.png").WaitForCompletion());

            spikeDash = impSpike.transform.Find("ImpactEffect").Find("Dash").gameObject.InstantiateClone("Dash", false);

            spikeDashBlue = impSpike.transform.Find("ImpactEffect").Find("Dash").gameObject.InstantiateClone("DashBlue", false);
            spikeDashBlue.GetComponent<ParticleSystemRenderer>().material = mercMat;
        }

        #endregion

        #region projectiles
        private static void CreateNeedleGhosts()
        {
            needleGhostDefault = LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/MageIceBombProjectile").GetComponent<ProjectileController>()
               .ghostPrefab.InstantiateClone("NeedleGhost", false);
            needleGhostDefault.transform.GetChild(0).gameObject.SetActive(false);
            needleGhostDefault.transform.GetChild(1).gameObject.SetActive(false);
            needleGhostDefault.transform.GetChild(2).localScale = new Vector3(0.2f, 0.2f, 1.66f);
            needleGhostDefault.transform.GetChild(2).gameObject.GetComponent<MeshFilter>().mesh = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ImpBoss/ImpVoidspikeProjectileGhost.prefab").WaitForCompletion().transform.GetChild(0).GetComponent<MeshFilter>().mesh;
            needleGhostDefault.transform.GetChild(2).gameObject.GetComponent<MeshRenderer>().material = Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpClaw.mat").WaitForCompletion());
            needleGhostDefault.transform.GetChild(3).gameObject.SetActive(false);
            needleGhostDefault.transform.GetChild(4).localScale = new Vector3(.2f, .2f, .2f);
            needleGhostDefault.transform.GetChild(4).GetChild(0).gameObject.GetComponent<TrailRenderer>().material = Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpPortalEffectEdge.mat").WaitForCompletion());
            needleGhostDefault.transform.GetChild(4).GetChild(1).gameObject.GetComponent<TrailRenderer>().material = Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpPortalEffectEdge.mat").WaitForCompletion());
            needleGhostDefault.transform.GetChild(4).GetChild(3).gameObject.SetActive(false);

            needleGhostBlue = LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/MageIceBombProjectile").GetComponent<ProjectileController>()
                .ghostPrefab.InstantiateClone("NeedleGhostBlue", false);
            needleGhostBlue.transform.GetChild(0).gameObject.SetActive(false);
            needleGhostBlue.transform.GetChild(1).gameObject.SetActive(false);
            needleGhostBlue.transform.GetChild(2).localScale = new Vector3(0.2f, 0.2f, 1.66f);
            needleGhostBlue.transform.GetChild(3).gameObject.SetActive(false);
            needleGhostBlue.transform.GetChild(4).localScale = new Vector3(.2f, .2f, .2f);
            needleGhostBlue.transform.GetChild(4).GetChild(3).gameObject.SetActive(false);
        }

        private static void CreateScissorGhosts()
        {
            scissorRGhostDefault = mainAssetBundle.CreateProjectileGhostPrefab("ScissorRightGhost");

            if (!scissorRGhostDefault.GetComponent<NetworkIdentity>()) scissorRGhostDefault.AddComponent<NetworkIdentity>();
            if (!scissorRGhostDefault.GetComponent<VFXAttributes>()) scissorRGhostDefault.AddComponent<VFXAttributes>();
            scissorRGhostDefault.GetComponent<VFXAttributes>().vfxPriority = VFXAttributes.VFXPriority.Always;
            scissorRGhostDefault.GetComponent<VFXAttributes>().vfxIntensity = VFXAttributes.VFXIntensity.Low;
            scissorRGhostDefault.GetComponent<VFXAttributes>().DoNotPool = true;

            scissorLGhostDefault = mainAssetBundle.CreateProjectileGhostPrefab("ScissorLeftGhost");

            if (!scissorLGhostDefault.GetComponent<NetworkIdentity>()) scissorLGhostDefault.AddComponent<NetworkIdentity>();
            if (!scissorLGhostDefault.GetComponent<VFXAttributes>()) scissorLGhostDefault.AddComponent<VFXAttributes>();
            scissorLGhostDefault.GetComponent<VFXAttributes>().vfxPriority = VFXAttributes.VFXPriority.Always;
            scissorLGhostDefault.GetComponent<VFXAttributes>().vfxIntensity = VFXAttributes.VFXIntensity.Low;
            scissorLGhostDefault.GetComponent<VFXAttributes>().DoNotPool = true;

            #region Mastery Skin Ghost
            scissorRGhostSword = scissorRGhostDefault.InstantiateClone("ScissorRightGhostSword");

            scissorRGhostSword.GetComponent<MeshFilter>().mesh = mainAssetBundle.LoadAsset<Mesh>("meshPrincessSwordR");
            scissorRGhostSword.GetComponent<MeshRenderer>().material = mainAssetBundle.LoadAsset<Material>("matPrincessSword");

            scissorLGhostSword = scissorRGhostDefault.InstantiateClone("ScissorLeftGhostSword");

            scissorLGhostSword.GetComponent<MeshFilter>().mesh = mainAssetBundle.LoadAsset<Mesh>("meshPrincessSwordL");
            scissorLGhostSword.GetComponent<MeshRenderer>().material = mainAssetBundle.LoadAsset<Material>("matPrincessSword");
            

            scissorRGhostSwordAlt = scissorRGhostDefault.InstantiateClone("ScissorRightGhostSwordAlt");

            scissorRGhostSwordAlt.GetComponent<MeshFilter>().mesh = mainAssetBundle.LoadAsset<Mesh>("meshPrincessSwordR");
            scissorRGhostSwordAlt.GetComponent<MeshRenderer>().material = mainAssetBundle.LoadAsset<Material>("matPrincessSwordAlt");

            scissorLGhostSwordAlt = scissorRGhostDefault.InstantiateClone("ScissorLeftGhostSwordAlt");

            scissorLGhostSwordAlt.GetComponent<MeshFilter>().mesh = mainAssetBundle.LoadAsset<Mesh>("meshPrincessSwordL");
            scissorLGhostSwordAlt.GetComponent<MeshRenderer>().material = mainAssetBundle.LoadAsset<Material>("matPrincessSwordAlt");

            #endregion

            #region Raven
            scissorRGhostRaven = scissorRGhostDefault.InstantiateClone("ScissorRightGhostRaven");

            scissorRGhostRaven.GetComponent<MeshFilter>().mesh = mainAssetBundle.LoadAsset<Mesh>("meshShadowClawsR");
            scissorRGhostRaven.GetComponent<MeshRenderer>().material = mainAssetBundle.LoadAsset<Material>("matRavenShadowClaws");

            scissorLGhostRaven = scissorRGhostDefault.InstantiateClone("ScissorLeftGhostRaven");

            scissorLGhostRaven.GetComponent<MeshFilter>().mesh = mainAssetBundle.LoadAsset<Mesh>("meshShadowClawsL");
            scissorLGhostRaven.GetComponent<MeshRenderer>().material = mainAssetBundle.LoadAsset<Material>("matRavenShadowClaws");
            #endregion
        }
        private static void CreateProjectiles()
        {
            CreateNeedleGhosts();
            CreateScissorGhosts();

            CreateNeedle(needleGhostDefault);

            scissorRPrefab = CreateScissor("ScissorRightGhost", "ScissorR", scissorRGhostDefault);

            scissorLPrefab = CreateScissor("ScissorLeftGhost", "ScissorL", scissorLGhostDefault);
        }
        private static GameObject CreateNeedle(GameObject needleGhost)
        {
            needlePrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/FMJRamping.prefab").WaitForCompletion().InstantiateClone("SeamstressNeedle");

            ProjectileSimple needleSimple = needlePrefab.GetComponent<ProjectileSimple>();
            needleSimple.desiredForwardSpeed = 125f;
            needleSimple.lifetime = 3f;
            needleSimple.updateAfterFiring = true;

            ProjectileDamage needleDamage = needlePrefab.GetComponent<ProjectileDamage>();
            needleDamage.damageType = DamageType.Generic;
            needleDamage.damageType.AddModdedDamageType(DamageTypes.SeamstressLifesteal);

            needlePrefab.AddComponent<ProjectileTargetComponent>();

            ProjectileSteerTowardTarget needleSteer = needlePrefab.AddComponent<ProjectileSteerTowardTarget>();
            needleSteer.yAxisOnly = false;
            needleSteer.rotationSpeed = 700f;

            ProjectileOverlapAttack needleLap = needlePrefab.GetComponent<ProjectileOverlapAttack>();
            needleLap.impactEffect = scissorsHitImpactEffect;
            needleLap.resetInterval = 0.5f;
            needleLap.overlapProcCoefficient = SeamstressConfig.needleProcCoefficient.Value;

            ProjectileDirectionalTargetFinder needleFinder = needlePrefab.AddComponent<ProjectileDirectionalTargetFinder>();
            needleFinder.lookRange = 35f;
            needleFinder.lookCone = 110f;
            needleFinder.targetSearchInterval = 0.2f;
            needleFinder.onlySearchIfNoTarget = false;
            needleFinder.allowTargetLoss = true;
            needleFinder.testLoS = true;
            needleFinder.ignoreAir = false;
            needleFinder.flierAltitudeTolerance = Mathf.Infinity;

            ProjectileController needleProjectileController = needlePrefab.GetComponent<ProjectileController>();
            needleProjectileController.procCoefficient = 1f;
            needleProjectileController.ghostPrefab = needleGhost;

            Object.Destroy(needleGhost.GetComponent<EffectComponent>());

            if (needleGhost)
                needleProjectileController.ghostPrefab = needleGhost;
            if (!needleProjectileController.ghostPrefab.GetComponent<NetworkIdentity>())
                needleProjectileController.ghostPrefab.AddComponent<NetworkIdentity>();
            if (!needleProjectileController.ghostPrefab.GetComponent<ProjectileGhostController>())
                needleProjectileController.ghostPrefab.AddComponent<ProjectileGhostController>();
            needleProjectileController.startSound = "";

            needleProjectileController.ghostPrefab.GetComponent<VFXAttributes>().DoNotPool = true;

            Modules.Content.AddProjectilePrefab(needlePrefab);

            return needlePrefab;
        }

        private static GameObject CreateScissor(string modelName, string name, GameObject ghostPrefab)
        {
            GameObject scissorPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ImpBoss/ImpVoidspikeProjectile.prefab").WaitForCompletion().InstantiateClone(name, true);
            TeamAreaIndicator teamIndicator = seamstressTeamAreaIndicator;

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

            TeamAreaIndicator seamArea = Object.Instantiate(teamIndicator, scissorPrefab.transform);
            seamArea.gameObject.transform.localScale = Vector3.one * 6f;
            seamArea.teamFilter = scissorPrefab.GetComponent<TeamFilter>();
            seamArea.gameObject.SetActive(false);

            Object.Instantiate(scissorTrailEffectDefault, scissorPrefab.transform);

            ProjectileImpactExplosion impactAlly = scissorPrefab.GetComponent<ProjectileImpactExplosion>();
            impactAlly.blastDamageCoefficient = SeamstressConfig.scissorPickupDamageCoefficient.Value;
            impactAlly.blastProcCoefficient = 0.7f;
            impactAlly.destroyOnEnemy = false;
            impactAlly.blastAttackerFiltering = AttackerFiltering.NeverHitSelf;
            impactAlly.lifetime = 14f;
            impactAlly.lifetimeAfterImpact = 14f;
            impactAlly.impactEffect = pickupScissorEffectDefault;
            impactAlly.blastRadius *= 2f;

            ProjectileDamage scissorDamage = scissorPrefab.GetComponent<ProjectileDamage>();
            scissorDamage.damageType = DamageType.Stun1s | DamageType.AOE;
            scissorDamage.damageType.damageSource = DamageSource.Special;
            scissorDamage.damageType.AddModdedDamageType(DamageTypes.SeamstressLifesteal);

            ProjectileSimple simple = scissorPrefab.GetComponent<ProjectileSimple>();
            simple.desiredForwardSpeed = 120f;
            simple.updateAfterFiring = true;

            //changes team filter to only team
            PickupFilterComponent scissorPickup = scissorPrefab.transform.GetChild(0).GetChild(5).gameObject.AddComponent<PickupFilterComponent>();
            scissorPickup.myTeamFilter = scissorPrefab.GetComponent<TeamFilter>();
            scissorPickup.triggerEvents = scissorPrefab.transform.GetChild(0).GetChild(5).gameObject.GetComponent<MineProximityDetonator>().triggerEvents;
            Object.Destroy(scissorPrefab.transform.GetChild(0).GetChild(5).gameObject.GetComponent<MineProximityDetonator>());

            ScissorImpact impact = scissorPrefab.AddComponent<ScissorImpact>();
            impact.stickSoundString = "sfx_seamstress_scissor_land";
            impact.stickParticleSystem = scissorPrefab.GetComponent<ProjectileStickOnImpact>().stickParticleSystem;
            impact.ignoreCharacters = false;
            impact.ignoreWorld = false;
            impact.stickEvent = scissorPrefab.GetComponent<ProjectileStickOnImpact>().stickEvent;
            impact.alignNormals = true;
            impact.impactEffect = blinkEffectDefault;
            impact.explosionEffect = impactExplosionEffectDefault;
            Object.Destroy(scissorPrefab.GetComponent<ProjectileStickOnImpact>());

            scissorPrefab.transform.GetChild(0).GetChild(5).gameObject.GetComponent<SphereCollider>().radius = 6f;

            ProjectileSteerTowardTarget scissorSteer = scissorPrefab.AddComponent<ProjectileSteerTowardTarget>();
            scissorSteer.yAxisOnly = false;
            scissorSteer.rotationSpeed = 700f;
            scissorSteer.enabled = true;

            ProjectileDirectionalTargetFinder scissorTracking = scissorPrefab.AddComponent<ProjectileDirectionalTargetFinder>();
            scissorTracking.lookRange = 0f;
            scissorTracking.lookCone = 0f;
            scissorTracking.targetSearchInterval = 0.2f;
            scissorTracking.onlySearchIfNoTarget = true;
            scissorTracking.allowTargetLoss = false;
            scissorTracking.testLoS = true;
            scissorTracking.ignoreAir = false;
            scissorTracking.flierAltitudeTolerance = Mathf.Infinity;
            scissorTracking.enabled = true;

            float die = modelName == "ScissorRightGhost" ? 3.1f : -3.1f;
            GameObject scissorModelTransform = new GameObject();
            scissorModelTransform.name = modelName + "Transform";
            scissorModelTransform.transform.localScale = Vector3.one * 2f;
            scissorModelTransform.transform.localPosition = new Vector3(0, die, -1);
            scissorModelTransform.transform.rotation = Quaternion.AngleAxis(270f, Vector3.forward);
            scissorModelTransform.transform.SetParent(scissorPrefab.transform, false);

            ProjectileController scissorProjectileController = scissorPrefab.GetComponent<ProjectileController>();
            scissorProjectileController.procCoefficient = 1f;
            scissorProjectileController.ghostTransformAnchor = scissorPrefab.transform.Find(modelName + "Transform");
            scissorProjectileController.ghostPrefab = ghostPrefab;

            Modules.Content.AddProjectilePrefab(scissorPrefab);

            return scissorPrefab;
        }
        #endregion

        #region sounds
        private static void CreateSounds()
        {
            LoadSoundbank();

            scissorsHitSoundEvent = SeamstressMod.Modules.Content.CreateAndAddNetworkSoundEventDef("Play_merc_sword_impact");

            parrySuccessSoundEvent = SeamstressMod.Modules.Content.CreateAndAddNetworkSoundEventDef("Play_voidman_m2_explode");
        }

        internal static void LoadSoundbank()
        {
            using (Stream manifestResourceStream2 = Assembly.GetExecutingAssembly().GetManifestResourceStream("SeamstressMod.seam_bank.bnk"))
            {
                byte[] array = new byte[manifestResourceStream2.Length];
                manifestResourceStream2.Read(array, 0, array.Length);
                SoundAPI.SoundBanks.Add(array);
            }
        }
        #endregion

        #region helpers
        private static GameObject CreateImpactExplosionEffect(string effectName, Material bloodMat, Material colorMat, bool blue, float scale = 1f)
        {
            GameObject newEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Brother/BrotherSlamImpact.prefab").WaitForCompletion().InstantiateClone(effectName, true);

            newEffect.transform.Find("Spikes, Small").gameObject.SetActive(false);

            newEffect.transform.Find("PP").gameObject.SetActive(false);
            newEffect.transform.Find("Point light").gameObject.SetActive(false);
            newEffect.transform.Find("Flash Lines").GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matOpaqueDustLargeDirectional.mat").WaitForCompletion();
            if(blue) newEffect.transform.Find("Flash Lines").GetComponent<ParticleSystemRenderer>().material.SetColor("_TintColor", Color.cyan);

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

            newEffect.transform.Find("Decal").GetComponent<Decal>().Material = colorMat;
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
            if(blue)
            {
                newEffect.transform.Find("Fire").gameObject.SetActive(false);
                newEffect.transform.Find("Flash Lines, Fire").gameObject.SetActive(false);
                newEffect.transform.GetChild(6).gameObject.SetActive(false);
                newEffect.transform.GetChild(3).gameObject.SetActive(false);
            }
            PSCFED.effectComponent = newEffect.GetComponent<EffectComponent>();

            SeamstressMod.Modules.Content.CreateAndAddEffectDef(newEffect);

            return newEffect;
        }
        public static Material CreateMaterial(string materialName, float emission, Color emissionColor, float normalStrength)
        {
            if (!commandoMat) commandoMat = Resources.Load<GameObject>("Prefabs/CharacterBodies/CommandoBody").GetComponentInChildren<CharacterModel>().baseRendererInfos[0].defaultMaterial;

            Material mat = UnityEngine.Object.Instantiate<Material>(commandoMat);
            Material tempMat = SeamstressAssets.mainAssetBundle.LoadAsset<Material>(materialName);

            if (!tempMat) return commandoMat;

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
            return SeamstressAssets.CreateMaterial(materialName, 0f);
        }

        public static Material CreateMaterial(string materialName, float emission)
        {
            return SeamstressAssets.CreateMaterial(materialName, emission, Color.black);
        }

        public static Material CreateMaterial(string materialName, float emission, Color emissionColor)
        {
            return SeamstressAssets.CreateMaterial(materialName, emission, emissionColor, 0f);
        }
        #endregion

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
            heartMdl.transform.GetChild(2).GetChild(0).gameObject.GetComponent<LightIntensityCurve>().timeMax = SeamstressConfig.insatiableDuration.Value;
            heartMdl.transform.GetChild(2).GetChild(1).gameObject.GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Junk/Common/VFX/matBloodParticle.mat").WaitForCompletion();
            var fard = heartMdl.transform.GetChild(2).GetChild(1).gameObject.GetComponent<ParticleSystem>().main;
            fard.duration = SeamstressConfig.insatiableDuration.Value;
            heartMdl.transform.GetChild(2).GetChild(2).gameObject.SetActive(false);
            heartMdl.transform.GetChild(2).GetChild(3).gameObject.SetActive(false);
            fard = heartMdl.transform.GetChild(2).GetChild(4).gameObject.GetComponent<ParticleSystem>().main;
            fard.startColor = theRed;
            fard.duration = SeamstressConfig.insatiableDuration.Value;
            fard = heartMdl.transform.GetChild(2).GetChild(5).gameObject.GetComponent<ParticleSystem>().main;
            fard.startColor = coolRed;
            fard.duration = SeamstressConfig.insatiableDuration.Value;
            heartMdl.transform.GetChild(2).GetChild(6).gameObject.GetComponent<ParticleSystemRenderer>().material.SetColor("_TintColor", Color.red);
            fard = heartMdl.transform.GetChild(2).GetChild(6).gameObject.GetComponent<ParticleSystem>().main;
            fard.duration = SeamstressConfig.insatiableDuration.Value;
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
            Modules.Content.CreateAndAddEffectDef(chainToHeart);
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
            Prefabs.AddMainEntityStateMachine(heartPrefab, "Body", typeof(SkillStates.HeartStandBy), typeof(SkillStates.HeartSpawnState));
        }
        private static void CreateHeartBlue()
        {
            GameObject heartMdl = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/EliteEarth/AffixEarthHealerBody.prefab").WaitForCompletion().transform.GetChild(0).GetChild(0).gameObject.InstantiateClone("HeartMdl", false);
            heartMdl.transform.localScale /= 3f;
            Material eatMyButt = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/EliteEarth/AffixEarthCore.mat").WaitForCompletion();
            Material[] explodeAndDie = new Material[1];
            explodeAndDie[0] = eatMyButt;
            heartMdl.transform.GetChild(1).gameObject.GetComponent<SkinnedMeshRenderer>().materials = explodeAndDie;
            heartMdl.transform.GetChild(1).gameObject.GetComponent<SkinnedMeshRenderer>().materials[0].SetColor("_Color", Color.cyan);
            heartMdl.transform.GetChild(1).gameObject.GetComponent<SkinnedMeshRenderer>().materials[0].SetColor("_EmColor", Color.cyan);
            Object.Destroy(heartMdl.GetComponent<CharacterModel>());
            Object.Destroy(heartMdl.GetComponent<HurtBoxGroup>());
            Object.DestroyImmediate(heartMdl.transform.GetChild(2).gameObject);
            heartMdl.transform.GetChild(2).localScale = new Vector3(1.5f, 1.5f, 1.5f);
            heartMdl.transform.GetChild(2).GetChild(0).gameObject.GetComponent<Light>().color = Color.cyan;
            heartMdl.transform.GetChild(2).GetChild(0).gameObject.GetComponent<Light>().range = 2f;
            heartMdl.transform.GetChild(2).GetChild(0).gameObject.GetComponent<LightIntensityCurve>().timeMax = SeamstressConfig.insatiableDuration.Value;
            heartMdl.transform.GetChild(2).GetChild(1).gameObject.GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Junk/Common/VFX/matBloodParticle.mat").WaitForCompletion();
            heartMdl.transform.GetChild(2).GetChild(1).gameObject.GetComponent<ParticleSystemRenderer>().material.SetColor("_TintColor", Color.cyan);
            var fard = heartMdl.transform.GetChild(2).GetChild(1).gameObject.GetComponent<ParticleSystem>().main;
            fard.duration = SeamstressConfig.insatiableDuration.Value;
            heartMdl.transform.GetChild(2).GetChild(2).gameObject.SetActive(false);
            heartMdl.transform.GetChild(2).GetChild(3).gameObject.SetActive(false);
            fard = heartMdl.transform.GetChild(2).GetChild(4).gameObject.GetComponent<ParticleSystem>().main;
            fard.startColor = Color.cyan;
            fard.duration = SeamstressConfig.insatiableDuration.Value;
            fard = heartMdl.transform.GetChild(2).GetChild(5).gameObject.GetComponent<ParticleSystem>().main;
            fard.startColor = Color.cyan;
            fard.duration = SeamstressConfig.insatiableDuration.Value;
            heartMdl.transform.GetChild(2).GetChild(6).gameObject.GetComponent<ParticleSystemRenderer>().material.SetColor("_TintColor", Color.cyan);
            fard = heartMdl.transform.GetChild(2).GetChild(6).gameObject.GetComponent<ParticleSystem>().main;
            fard.duration = SeamstressConfig.insatiableDuration.Value;
            Material chains = Addressables.LoadAssetAsync<Material>("RoR2/Base/bazaar/matSeerPortalEffectEdge.mat").WaitForCompletion();
            chains.SetColor("_EmColor", Color.cyan);
            chains.SetColor("_TintColor", Color.cyan);
            Material[] ballsackTickler = new Material[1];
            ballsackTickler[0] = chains;
            chainToHeartBlue = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Treebot/EntangleOrbEffect.prefab").WaitForCompletion().InstantiateClone("HeartChainsBlue");
            chainToHeartBlue.AddComponent<NetworkIdentity>();
            chainToHeartBlue.AddComponent<DestroyOnCondition>();
            chainToHeartBlue.transform.GetChild(0).GetComponent<LineRenderer>().materials = ballsackTickler;
            chainToHeartBlue.transform.GetChild(0).GetComponent<LineRenderer>().startColor = Color.red;
            chainToHeartBlue.transform.GetChild(0).GetComponent<LineRenderer>().startColor = coolRed;
            chainToHeartBlue.transform.GetChild(0).GetComponent<LineRenderer>().shadowBias = 0.5f;
            chainToHeartBlue.transform.localScale *= 0.5f;
            chainToHeartBlue.transform.GetChild(0).GetChild(0).gameObject.GetComponent<ParticleSystemRenderer>().mesh = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ElementalRings/PickupFireRing.prefab").WaitForCompletion().transform.GetChild(0).gameObject.GetComponent<MeshFilter>().mesh;
            chainToHeartBlue.transform.GetChild(0).GetChild(0).gameObject.GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/bazaar/matSeerPortalEffectEdge.mat").WaitForCompletion();
            chainToHeartBlue.gameObject.GetComponent<AkEvent>().enabled = false;
            chainToHeartBlue.gameObject.GetComponent<AkGameObj>().enabled = false;
            Modules.Content.CreateAndAddEffectDef(chainToHeartBlue);
            heartPrefabBlue = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Treebot/TreebotFlower2.prefab").WaitForCompletion().InstantiateClone("HeartPrefabBlue");
            heartPrefabBlue.transform.localRotation = Quaternion.identity;
            CleanChildren(heartPrefabBlue.transform.GetChild(0));
            heartPrefabBlue.transform.GetChild(0).localRotation = new Quaternion(6.643471e-24f, 4.689353e-39f, 2.914701e-43f, Quaternion.identity.w);
            heartPrefabBlue.transform.localPosition = Vector3.zero;
            heartMdl.transform.SetParent(heartPrefabBlue.transform.GetChild(0));
            heartPrefabBlue.gameObject.GetComponent<ModelLocator>().modelTransform = heartPrefabBlue.transform.GetChild(0).GetChild(0);
            heartPrefabBlue.gameObject.GetComponent<ProjectileDamage>().enabled = false;
            EntityStateMachine[] machines = heartPrefabBlue.GetComponents<EntityStateMachine>();

            for (int i = machines.Length - 1; i >= 0; i--)
            {
                Object.DestroyImmediate(machines[i]);
            }
            Prefabs.AddMainEntityStateMachine(heartPrefabBlue, "Main", typeof(SkillStates.HeartStandBy), typeof(SkillStates.HeartSpawnState));
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
    }
}