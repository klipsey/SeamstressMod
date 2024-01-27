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

        internal static void Init()
        {
            Empty = DamageAPI.ReserveDamageType();
            CutDamage = DamageAPI.ReserveDamageType();
            StitchDamage = DamageAPI.ReserveDamageType();
            AddNeedlesDamage = DamageAPI.ReserveDamageType();
            WeaveLifeSteal = DamageAPI.ReserveDamageType();

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
            if (!damageReport.attacker)
            {
                return;
            }
            HealthComponent victim = damageReport.victim;
            CharacterBody victimBody = damageReport.victimBody;
            CharacterBody attacker = damageReport.attackerBody;
            GameObject attackerObject = damageReport.attacker.gameObject;
            if (NetworkServer.active)
            {
                if(damageInfo.HasModdedDamageType(StitchDamage))
                {
                    if(damageInfo.HasModdedDamageType(CutDamage))
                    {
                        return;
                    }
                    else
                    {
                        DamageInfo stitch = new DamageInfo
                        {
                            damage = attacker.damage * (SeamstressStaticValues.stitchDamageCoefficient * damageInfo.procCoefficient),
                            damageColorIndex = DamageColorIndex.DeathMark,
                            damageType = DamageType.Generic,
                            attacker = damageInfo.attacker,
                            crit = false,
                            force = Vector3.zero,
                            inflictor = null,
                            position = damageInfo.position,
                            procCoefficient = 0f
                        };
                        victim.TakeDamage(stitch);
                        if (damageReport.victimIsBoss)
                        {
                            DotController.InflictDot(victimBody.gameObject, attackerObject, Dots.SeamstressDotWeak, SeamstressStaticValues.stitchDuration, 1f);
                        }
                        else
                        {
                            DotController.InflictDot(victimBody.gameObject, attackerObject, Dots.SeamstressDot, SeamstressStaticValues.stitchDuration, 1f);
                        }
                    }

                }

                if (damageInfo.HasModdedDamageType(CutDamage))
                {
                    int placeboBuff;
                    if (damageInfo.HasModdedDamageType(StitchDamage))
                    {
                        placeboBuff = 1;
                        victimBody.AddBuff(SeamstressBuffs.stitched);
                    }
                    else
                    {
                        placeboBuff = 0;
                    }
                    DamageInfo cutsume = new DamageInfo
                    {
                        damage = attacker.damage * (SeamstressStaticValues.cutsumeMissingHpDamage * damageInfo.procCoefficient),
                        damageColorIndex = DamageColorIndex.DeathMark,
                        damageType = DamageType.Generic,
                        attacker = damageInfo.attacker,
                        crit = false,
                        force = Vector3.zero,
                        inflictor = damageInfo.inflictor,
                        position = damageInfo.position,
                        procCoefficient = 0f
                    };

                    if (victimBody.HasBuff(SeamstressBuffs.stitched))
                    {
                        for (int i = 0; i < victimBody.GetBuffCount(SeamstressBuffs.stitched) - placeboBuff; i++)
                        {
                            victim.TakeDamage(cutsume);
                            attacker.skillLocator.DeductCooldownFromAllSkillsServer(SeamstressStaticValues.cutCooldownReduction * damageInfo.procCoefficient);
                        }
                        if(placeboBuff == 1)
                        {
                            victimBody.RemoveBuff(SeamstressBuffs.stitched);
                        }
                        DotController hi = DotController.FindDotController(victimBody.gameObject);
                        List<PendingDamage> list = CollectionPool<PendingDamage, List<PendingDamage>>.RentCollection();
                        if (!NetworkServer.active)
                        {
                            Debug.LogWarning("[Server] function 'System.Void RoR2.DotController::RemoveAllDots(UnityEngine.GameObject)' called on client");
                        }
                        else if (hi.HasDotActive(Dots.SeamstressDot) || hi.HasDotActive(Dots.SeamstressDotWeak))
                        {
                            for (int num = hi.dotStackList.Count - 1; num >= 0; num--)
                            {
                                DotStack dotStack = hi.dotStackList[num];
                                if (dotStack.dotIndex == Dots.SeamstressDot || dotStack.dotIndex == Dots.SeamstressDotWeak)
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
                        ProjectileManager.instance.FireProjectile(projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), attacker.gameObject, attacker.damage * SeamstressStaticValues.sewNeedleDamageCoefficient, 600f, attacker.RollCrit(), DamageColorIndex.Default, null, -1f);
                    }
                }
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
                        ProjectileManager.instance.FireProjectile(projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), attacker.gameObject, attacker.damage * SeamstressStaticValues.sewNeedleDamageCoefficient, 600f, attacker.RollCrit(), DamageColorIndex.Default, null, -1f);
                    }
                }
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
                    for (int i = 0; i < victimBody.GetBuffCount(SeamstressBuffs.stitched); i++)
                    {
                        attacker.skillLocator.DeductCooldownFromAllSkillsServer(SeamstressStaticValues.cutCooldownReduction * damageInfo.procCoefficient);
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
                        ProjectileManager.instance.FireProjectile(projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), attacker.gameObject, attacker.damage * SeamstressStaticValues.sewNeedleDamageCoefficient, 600f, attacker.RollCrit(), DamageColorIndex.Default, null, -1f);
                    }

                }
            }
        }
    }
}
