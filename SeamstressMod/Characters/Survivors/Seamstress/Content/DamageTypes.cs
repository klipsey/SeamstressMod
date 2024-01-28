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
        public static DamageAPI.ModdedDamageType WeaveLifeSteal;
        public static DamageAPI.ModdedDamageType BeginHoming;

        internal static void Init()
        {
            Empty = DamageAPI.ReserveDamageType();
            CutDamage = DamageAPI.ReserveDamageType();
            StitchDamage = DamageAPI.ReserveDamageType();
            AddNeedlesDamage = DamageAPI.ReserveDamageType();
            WeaveLifeSteal = DamageAPI.ReserveDamageType();
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
            if (!damageReport.attacker || !damageReport.victimBody)
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
                if(damageInfo.HasModdedDamageType(BeginHoming))
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
                        needleSimple.lifetime = 1.25f;
                        needleSimple.updateAfterFiring = true;
                    }
                }
                if(damageInfo.HasModdedDamageType(StitchDamage))
                {
                    attacker.skillLocator.DeductCooldownFromAllSkillsServer(SeamstressStaticValues.stitchCooldownReduction * damageInfo.procCoefficient);
                    if (!damageInfo.HasModdedDamageType(CutDamage))
                    {
                        victimBody.AddBuff(SeamstressBuffs.stitchSetup);
                    }

                }

                if (damageInfo.HasModdedDamageType(CutDamage))
                {
                    #region cut initial damage
                    DamageInfo cutsume = new DamageInfo
                    {
                        damage = 0f,
                        damageColorIndex = DamageColorIndex.DeathMark,
                        damageType = DamageType.Generic,
                        attacker = damageInfo.attacker,
                        crit = false,
                        force = Vector3.zero,
                        inflictor = damageInfo.inflictor,
                        position = damageInfo.position,
                        procCoefficient = 0f
                    };
                    #endregion
                    //check for overlapping damagetypes
                    if (damageInfo.HasModdedDamageType(StitchDamage))
                    {
                        victimBody.AddBuff(SeamstressBuffs.stitchSetup);
                    }
                    //begin damage process
                    if (victimBody.HasBuff(SeamstressBuffs.stitchSetup))
                    {
                        Util.PlaySound("Play_imp_overlord_teleport_end", victimBody.gameObject);
                        for (int i = victimBody.GetBuffCount(SeamstressBuffs.stitchSetup); i > 0; i--)
                        {
                            if (damageReport.victimIsBoss)
                            {
                                cutsume.damage = (attacker.damage* SeamstressStaticValues.cutBaseDamage * damageInfo.procCoefficient);
                                DotController.InflictDot(victimBody.gameObject, attackerObject, Dots.SeamstressBossDot, SeamstressStaticValues.cutDuration, damageInfo.procCoefficient);
                            }
                            else
                            {
                                cutsume.damage = (attacker.damage * SeamstressStaticValues.cutBaseDamage * damageInfo.procCoefficient);
                                DotController.InflictDot(victimBody.gameObject, attackerObject, Dots.SeamstressDot, SeamstressStaticValues.cutDuration, damageInfo.procCoefficient);
                            }
                            victim.TakeDamage(cutsume);
                            attacker.skillLocator.DeductCooldownFromAllSkillsServer(SeamstressStaticValues.stitchCooldownReduction * damageInfo.procCoefficient);
                            victimBody.RemoveBuff(SeamstressBuffs.stitchSetup);
                        }
                        //useless
                        #region dotController
                        /*
                        DotController hi = DotController.FindDotController(victimBody.gameObject);
                        List<PendingDamage> list = CollectionPool<PendingDamage, List<PendingDamage>>.RentCollection();
                        if (!NetworkServer.active)
                        {
                            Debug.LogWarning("[Server] function 'System.Void RoR2.DotController::RemoveAllDots(UnityEngine.GameObject)' called on client");
                        }
                        else if (hi.HasDotActive(Dots.SeamstressDot) || hi.HasDotActive(Dots.SeamstressBossDot))
                        {
                            for (int num = hi.dotStackList.Count - 1; num >= 0; num--)
                            {
                                DotStack dotStack = hi.dotStackList[num];
                                if (dotStack.dotIndex == Dots.SeamstressDot || dotStack.dotIndex == Dots.SeamstressBossDot)
                                {
                                    dotStack.timer = 0f;
                                    AddPendingDamageEntry(list, dotStack.attackerObject, dotStack.damage, dotStack.damageType);
                                    if (dotStack.timer <= 0f)
                                    {
                                        hi.RemoveDotStackAtServer(num);
                                    }
                                }
                            }
                        }
                        */
                        #endregion
                    }

                    if (attacker.GetBuffCount(SeamstressBuffs.needles) < SeamstressStaticValues.maxNeedleAmount + attacker.skillLocator.special.maxStock - 1)
                    {
                        attacker.AddBuff(SeamstressBuffs.needles);
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
                        ProjectileManager.instance.FireProjectile(projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), attacker.gameObject, attacker.damage * SeamstressStaticValues.sewNeedleDamageCoefficient, 0f, attacker.RollCrit(), DamageColorIndex.Default, null, -1f);
                    }
                }
                #region NeedleDamage incase of emergency
                if (damageInfo.HasModdedDamageType(AddNeedlesDamage))
                {
                    if (attacker.GetBuffCount(SeamstressBuffs.needles) < SeamstressStaticValues.maxNeedleAmount + attacker.skillLocator.special.maxStock - 1)
                    {
                        attacker.AddBuff(SeamstressBuffs.needles);
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
                        ProjectileManager.instance.FireProjectile(projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), attacker.gameObject, attacker.damage * SeamstressStaticValues.sewNeedleDamageCoefficient, 0f, attacker.RollCrit(), DamageColorIndex.Default, null, -1f);
                    }
                }
                #endregion
                if (damageInfo.HasModdedDamageType(WeaveLifeSteal))
                {
                    attacker.healthComponent.Heal(damageReport.damageDealt * SeamstressStaticValues.weaveLifeSteal, default(ProcChainMask), true);
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
            CharacterBody attacker = damageReport.attacker.GetComponent<CharacterBody>();
            Transform attackerTransform = attacker.modelLocator.transform;
            GameObject attackerObject = attacker.gameObject;
            if (NetworkServer.active)
            {
                if (victimBody.HasBuff(SeamstressBuffs.stitched))
                {
                    if (attacker.GetBuffCount(SeamstressBuffs.needles) < SeamstressStaticValues.maxNeedleAmount + attacker.skillLocator.special.maxStock - 1)
                    {
                        attacker.AddBuff(SeamstressBuffs.needles);
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
                        ProjectileManager.instance.FireProjectile(projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), attacker.gameObject, attacker.damage * SeamstressStaticValues.sewNeedleDamageCoefficient, 0f, attacker.RollCrit(), DamageColorIndex.Default, null, -1f);
                    }
                }
                if (victimBody.HasBuff(SeamstressBuffs.stitchSetup))
                {
                    if(!damageInfo.HasModdedDamageType(CutDamage))
                    {
                        for (int i = victimBody.GetBuffCount(SeamstressBuffs.stitchSetup); i > 0; i--)
                        {
                            attacker.skillLocator.DeductCooldownFromAllSkillsServer(SeamstressStaticValues.stitchCooldownReduction * damageInfo.procCoefficient);
                        }
                    }
                }
            }
        }
    }
}
