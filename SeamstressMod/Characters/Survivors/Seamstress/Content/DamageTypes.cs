using HG;
using Newtonsoft.Json.Linq;
using R2API;
using RoR2;
using RoR2.Projectile;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;
using static RoR2.DotController;

namespace SeamstressMod.Survivors.Seamstress
{
    public static class DamageTypes
    {
        public static DamageAPI.ModdedDamageType Empty;
        public static DamageAPI.ModdedDamageType CutDamage;
        public static DamageAPI.ModdedDamageType StitchDamage;
        public static DamageAPI.ModdedDamageType AddNeedlesDamage;
        public static DamageAPI.ModdedDamageType FlurryLifeSteal;
        public static DamageAPI.ModdedDamageType PlanarLifeSteal;
        public static DamageAPI.ModdedDamageType BeginHoming;

        internal static void Init()
        {
            Empty = DamageAPI.ReserveDamageType();
            CutDamage = DamageAPI.ReserveDamageType();
            StitchDamage = DamageAPI.ReserveDamageType();
            AddNeedlesDamage = DamageAPI.ReserveDamageType();
            FlurryLifeSteal = DamageAPI.ReserveDamageType();
            PlanarLifeSteal = DamageAPI.ReserveDamageType();
            BeginHoming = DamageAPI.ReserveDamageType();
            Hook();
        }
        private static void Hook()
        {
            GlobalEventManager.onCharacterDeathGlobal += GlobalEventManager_onCharacterDeathGlobal;
            GlobalEventManager.onServerDamageDealt += GlobalEventManager_onServerDamageDealt;
        }
        private static void GlobalEventManager_onServerDamageDealt(DamageReport damageReport)
        {
            DamageInfo damageInfo = damageReport.damageInfo;
            if (!damageReport.attackerBody || !damageReport.victimBody)
            {
                return;
            }
            HealthComponent victim = damageReport.victim;
            GameObject inflictorObject = damageInfo.inflictor;
            CharacterBody victimBody = damageReport.victimBody;
            CharacterBody attackerBody = damageReport.attackerBody;
            GameObject attackerObject = damageReport.attacker.gameObject;
            if (NetworkServer.active)
            {
                DamageInfo cutsume = new DamageInfo
                {
                    damage = attackerBody.damage * SeamstressStaticValues.stitchBaseDamage * damageInfo.procCoefficient,
                    damageColorIndex = DamageColorIndex.DeathMark,
                    damageType = DamageType.Generic,
                    attacker = damageInfo.attacker,
                    crit = false,
                    force = Vector3.zero,
                    inflictor = damageInfo.inflictor,
                    position = damageInfo.position,
                    procCoefficient = 0f
                };

                if (damageInfo.HasModdedDamageType(BeginHoming))
                {
                    ProjectileDirectionalTargetFinder s = inflictorObject.GetComponent<ProjectileDirectionalTargetFinder>();
                    if(!inflictorObject.TryGetComponent<ProjectileDirectionalTargetFinder>(out s))
                    {
                        ProjectileDirectionalTargetFinder needleFinder = inflictorObject.AddComponent<ProjectileDirectionalTargetFinder>();
                        needleFinder.lookRange = 35f;
                        needleFinder.lookCone = 180f;
                        needleFinder.targetSearchInterval = 0.2f;
                        needleFinder.onlySearchIfNoTarget = false;
                        needleFinder.allowTargetLoss = true;
                        needleFinder.testLoS = true;
                        needleFinder.ignoreAir = false;
                        needleFinder.flierAltitudeTolerance = Mathf.Infinity;

                        ProjectileSimple needleSimple = inflictorObject.GetComponent<ProjectileSimple>();
                        needleSimple.desiredForwardSpeed = 150f;
                        needleSimple.lifetime = 1f;
                        needleSimple.updateAfterFiring = true;
                    }
                }

                if (victimBody.HasBuff(SeamstressBuffs.stitchSetup) && attackerBody.baseNameToken == "KENKO_SEAMSTRESS_NAME")
                {
                    Util.PlaySound("Play_imp_overlord_teleport_end", victimBody.gameObject);
                    victimBody.RemoveBuff(SeamstressBuffs.stitchSetup);
                    if (damageReport.victimIsBoss)
                    {
                        victim.TakeDamage(cutsume);
                        DotController.InflictDot(victimBody.gameObject, attackerObject, Dots.SeamstressBossDot, SeamstressStaticValues.cutDuration, damageInfo.procCoefficient);
                    }
                    else
                    {
                        victim.TakeDamage(cutsume);
                        DotController.InflictDot(victimBody.gameObject, attackerObject, Dots.SeamstressDot, SeamstressStaticValues.cutDuration, damageInfo.procCoefficient);
                    }
                    if (attackerBody.skillLocator.secondary.stock < attackerBody.skillLocator.secondary.maxStock)
                    {
                        attackerBody.skillLocator.secondary.AddOneStock();
                        Util.PlaySound("Play_bandit2_m2_alt_throw", attackerObject);
                    }
                    else
                    {
                        GameObject projectilePrefab;
                        Ray aimRay;
                        aimRay = new Ray(attackerBody.inputBank.aimOrigin, attackerBody.inputBank.aimDirection);
                        if (attackerBody.HasBuff(SeamstressBuffs.butchered))
                        {
                            projectilePrefab = SeamstressAssets.needleButcheredPrefab;
                        }
                        else projectilePrefab = SeamstressAssets.needlePrefab;
                        ProjectileManager.instance.FireProjectile(projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), attackerBody.gameObject, attackerBody.damage * SeamstressStaticValues.needleDamageCoefficient, 0f, attackerBody.RollCrit(), DamageColorIndex.Default, null, -1f);
                    }
                }
                if (damageInfo.HasModdedDamageType(StitchDamage))
                {
                    victimBody.AddBuff(SeamstressBuffs.stitchSetup);
                }
                if (damageInfo.HasModdedDamageType(CutDamage))
                {
                    if (damageReport.victimIsBoss)
                    {
                        DotController.InflictDot(victimBody.gameObject, attackerObject, Dots.SeamstressBossDot, SeamstressStaticValues.cutDuration, damageInfo.procCoefficient);
                    }
                    else
                    {
                        DotController.InflictDot(victimBody.gameObject, attackerObject, Dots.SeamstressDot, SeamstressStaticValues.cutDuration, damageInfo.procCoefficient);
                    }
                }
                if (damageInfo.HasModdedDamageType(FlurryLifeSteal))
                {
                    attackerBody.healthComponent.Heal(damageReport.damageDealt * SeamstressStaticValues.flurryLifeSteal, default(ProcChainMask), true);
                }
                if (damageInfo.HasModdedDamageType(PlanarLifeSteal))
                {
                    attackerBody.healthComponent.Heal(damageReport.damageDealt * SeamstressStaticValues.needleHealAmount, default(ProcChainMask), true);
                }
            }
        }
        private static void GlobalEventManager_onCharacterDeathGlobal(DamageReport damageReport)
        {
            if (!damageReport.attackerBody || !damageReport.victimBody)
            {
                return;
            }
            DamageInfo damageInfo = damageReport.damageInfo;
            CharacterBody victimBody = damageReport.victimBody;
            CharacterBody attackerBody = damageReport.attacker.GetComponent<CharacterBody>();
            Transform attackerTransform = attackerBody.modelLocator.transform;
            GameObject attackerObject = attackerBody.gameObject;
            if (NetworkServer.active)
            {
                if(attackerBody.baseNameToken == "KENKO_SEAMSTRESS_NAME")
                {
                    if (attackerBody.skillLocator.secondary.stock < attackerBody.skillLocator.secondary.maxStock)
                    {
                        attackerBody.skillLocator.secondary.AddOneStock();
                        Util.PlaySound("Play_bandit2_m2_alt_throw", attackerObject);
                    }
                    else
                    {
                        GameObject projectilePrefab;
                        Ray aimRay;
                        aimRay = new Ray(attackerBody.inputBank.aimOrigin, attackerBody.inputBank.aimDirection);
                        if (attackerBody.HasBuff(SeamstressBuffs.butchered))
                        {
                            projectilePrefab = SeamstressAssets.needleButcheredPrefab;
                        }
                        else projectilePrefab = SeamstressAssets.needlePrefab;
                        ProjectileManager.instance.FireProjectile(projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), attackerBody.gameObject, attackerBody.damage * SeamstressStaticValues.needleDamageCoefficient, 0f, attackerBody.RollCrit(), DamageColorIndex.Default, null, -1f);
                    }
                }
            }
        }
    }
}
