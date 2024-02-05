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
        public static DamageAPI.ModdedDamageType flurryLifeSteal;
        public static DamageAPI.ModdedDamageType BeginHoming;

        internal static void Init()
        {
            Empty = DamageAPI.ReserveDamageType();
            CutDamage = DamageAPI.ReserveDamageType();
            StitchDamage = DamageAPI.ReserveDamageType();
            AddNeedlesDamage = DamageAPI.ReserveDamageType();
            flurryLifeSteal = DamageAPI.ReserveDamageType();
            BeginHoming = DamageAPI.ReserveDamageType();
            Hook();
        }
        private static void Hook()
        {
            //GlobalEventManager.onCharacterDeathGlobal += GlobalEventManager_onCharacterDeathGlobal;
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
            CharacterBody attacker = damageReport.attackerBody;
            GameObject attackerObject = damageReport.attacker.gameObject;
            if (NetworkServer.active)
            {
                DamageInfo cutsume = new DamageInfo
                {
                    damage = attacker.damage * SeamstressStaticValues.stitchBaseDamage * damageInfo.procCoefficient,
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

                if (victimBody.HasBuff(SeamstressBuffs.stitchSetup))
                {
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
                    if (attacker.skillLocator.special.stock < attacker.skillLocator.special.maxStock)
                    {
                        attacker.skillLocator.special.AddOneStock();
                        Util.PlaySound("Play_bandit2_m2_alt_throw", attackerObject);
                    }
                    else
                    {
                        GameObject projectilePrefab;
                        Ray aimRay;
                        aimRay = new Ray(attacker.inputBank.aimOrigin, attacker.inputBank.aimDirection);
                        if (attacker.HasBuff(SeamstressBuffs.butchered))
                        {
                            projectilePrefab = SeamstressAssets.needleButcheredPrefab;
                        }
                        else projectilePrefab = SeamstressAssets.needlePrefab;
                        ProjectileManager.instance.FireProjectile(projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), attacker.gameObject, attacker.damage * SeamstressStaticValues.needleDamageCoefficient, 0f, attacker.RollCrit(), DamageColorIndex.Default, null, -1f);
                    }
                }
                if (damageInfo.HasModdedDamageType(StitchDamage))
                {
                    victimBody.AddBuff(SeamstressBuffs.stitchSetup);
                }
                if (damageInfo.HasModdedDamageType(CutDamage))
                {
                    attacker.skillLocator.secondary.rechargeStopwatch += SeamstressStaticValues.stitchCooldownReduction;
                    Util.PlaySound("Play_imp_overlord_teleport_end", victimBody.gameObject);
                    if (damageReport.victimIsBoss)
                    {
                        DotController.InflictDot(victimBody.gameObject, attackerObject, Dots.SeamstressBossDot, SeamstressStaticValues.cutDuration, damageInfo.procCoefficient);
                    }
                    else
                    {
                        DotController.InflictDot(victimBody.gameObject, attackerObject, Dots.SeamstressDot, SeamstressStaticValues.cutDuration, damageInfo.procCoefficient);
                    }
                }
                if (damageInfo.HasModdedDamageType(flurryLifeSteal))
                {
                    attacker.healthComponent.Heal(damageReport.damageDealt * SeamstressStaticValues.flurryLifeSteal, default(ProcChainMask), true);
                }
            }
        }
        /*
        private static void GlobalEventManager_onCharacterDeathGlobal(DamageReport damageReport)
        {
            if (!damageReport.attackerBody || !damageReport.victimBody)
            {
                return;
            }
            DamageInfo damageInfo = damageReport.damageInfo;
            CharacterBody victimBody = damageReport.victimBody;
            CharacterBody attacker = damageReport.attacker.GetComponent<CharacterBody>();
            Transform attackerTransform = attacker.modelLocator.transform;
            GameObject attackerObject = attacker.gameObject;
            if (NetworkServer.active)
            {
            }
        }
        */
    }
}
