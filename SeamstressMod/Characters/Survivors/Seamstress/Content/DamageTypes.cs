using HG;
using Newtonsoft.Json.Linq;
using R2API;
using RoR2;
using RoR2.Projectile;
using System.Collections.Generic;
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
                        DotController.InflictDot(victimBody.gameObject, attackerObject, Dots.SeamstressDotWeak, 4f, 1f);
                    }
                    else
                    {
                        DotController.InflictDot(victimBody.gameObject, attackerObject, Dots.SeamstressDot, 4f, 1f);
                    }
                }

                if (damageInfo.HasModdedDamageType(CutDamage))
                {
                    if(victimBody.HasBuff(SeamstressBuffs.stitched))
                    {
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
                        for (int i = 0; i < victimBody.GetBuffCount(SeamstressBuffs.stitched); i++)
                        {
                            victim.TakeDamage(cutsume);
                            if (attacker.skillLocator.secondary.stock != attacker.skillLocator.secondary.maxStock && attacker.skillLocator.secondary.rechargeStock != 0)
                            {
                                attacker.skillLocator.secondary.rechargeStopwatch += SeamstressStaticValues.cutCooldownReduction * damageInfo.procCoefficient;
                            }
                            if (attacker.skillLocator.FindSkill("Utility").stock != attacker.skillLocator.FindSkill("Utility").maxStock && attacker.skillLocator.FindSkill("Utility").rechargeStock != 0)
                            {
                                attacker.skillLocator.FindSkill("Utility").rechargeStopwatch += SeamstressStaticValues.cutCooldownReduction * damageInfo.procCoefficient;
                            }
                            if (attacker.skillLocator.special.stock != attacker.skillLocator.special.maxStock && attacker.skillLocator.special.rechargeStock != 0)
                            {
                                attacker.skillLocator.special.rechargeStopwatch += SeamstressStaticValues.cutCooldownReduction * damageInfo.procCoefficient;
                            }
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
            if (!damageReport.attackerBody)
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
                        if (attacker.skillLocator.secondary.stock != attacker.skillLocator.secondary.maxStock && attacker.skillLocator.secondary.rechargeStock != 0)
                        {
                            attacker.skillLocator.secondary.rechargeStopwatch += SeamstressStaticValues.cutCooldownReduction * damageInfo.procCoefficient;
                        }
                        if (attacker.skillLocator.FindSkill("Utility").stock != attacker.skillLocator.FindSkill("Utility").maxStock && attacker.skillLocator.FindSkill("Utility").rechargeStock != 0)
                        {
                            attacker.skillLocator.FindSkill("Utility").rechargeStopwatch += SeamstressStaticValues.cutCooldownReduction * damageInfo.procCoefficient;
                        }
                        if (attacker.skillLocator.special.stock != attacker.skillLocator.special.maxStock && attacker.skillLocator.special.rechargeStock != 0)
                        {
                            attacker.skillLocator.special.rechargeStopwatch += SeamstressStaticValues.cutCooldownReduction * damageInfo.procCoefficient;
                        }
                    }
                }
            }
        }
    }
}
