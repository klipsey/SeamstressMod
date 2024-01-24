using R2API;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;

namespace SeamstressMod.Survivors.Seamstress
{
    public static class DamageTypes
    {
        public static DamageAPI.ModdedDamageType Empty;
        public static DamageAPI.ModdedDamageType CutDamage;
        public static DamageAPI.ModdedDamageType Stitched;
        public static DamageAPI.ModdedDamageType AddNeedlesKill;
        public static DamageAPI.ModdedDamageType AddNeedlesDamage;
        public static DamageAPI.ModdedDamageType WeaveLifeSteal;


        internal static void Init()
        {
            Empty = DamageAPI.ReserveDamageType();
            CutDamage = DamageAPI.ReserveDamageType();
            Stitched = DamageAPI.ReserveDamageType();
            AddNeedlesKill = DamageAPI.ReserveDamageType();
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
                if(damageInfo.HasModdedDamageType(Stitched))
                {
                    if (attacker.skillLocator.secondary.stock != attacker.skillLocator.secondary.maxStock && attacker.skillLocator.secondary.rechargeStock != 0)
                    {
                        attacker.skillLocator.secondary.rechargeStopwatch += SeamstressStaticValues.cutCooldownReduction;
                    }
                    if (attacker.skillLocator.FindSkill("Utility").stock != attacker.skillLocator.FindSkill("Utility").maxStock && attacker.skillLocator.FindSkill("Utility").rechargeStock != 0)
                    {
                        attacker.skillLocator.FindSkill("Utility").rechargeStopwatch += SeamstressStaticValues.cutCooldownReduction;
                    }
                    if (attacker.skillLocator.special.stock != attacker.skillLocator.special.maxStock && attacker.skillLocator.special.rechargeStock != 0)
                    {
                        attacker.skillLocator.special.rechargeStopwatch += SeamstressStaticValues.cutCooldownReduction;
                    }

                    if(damageReport.victimIsBoss)
                    {
                        DotController.InflictDot(victimBody.gameObject, attackerObject, Dots.SeamstressDotWeak, 2f, 1f);
                    }
                    else
                    {
                        DotController.InflictDot(victimBody.gameObject, attackerObject, Dots.SeamstressDot, 2f, 1f);
                    }
                }

                if (damageInfo.HasModdedDamageType(CutDamage))
                {
                    if (attacker.skillLocator.secondary.stock != attacker.skillLocator.secondary.maxStock && attacker.skillLocator.secondary.rechargeStock != 0)
                    {
                        attacker.skillLocator.secondary.rechargeStopwatch += SeamstressStaticValues.cutCooldownReduction;
                    }
                    if (attacker.skillLocator.FindSkill("Utility").stock != attacker.skillLocator.FindSkill("Utility").maxStock && attacker.skillLocator.FindSkill("Utility").rechargeStock != 0)
                    {
                        attacker.skillLocator.FindSkill("Utility").rechargeStopwatch += SeamstressStaticValues.cutCooldownReduction;
                    }
                    if (attacker.skillLocator.special.stock != attacker.skillLocator.special.maxStock && attacker.skillLocator.special.rechargeStock != 0)
                    {
                        attacker.skillLocator.special.rechargeStopwatch += SeamstressStaticValues.cutCooldownReduction;
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

                    if (damageReport.victimIsBoss)
                    {
                        DamageInfo cut = new DamageInfo
                        {
                            damage = victim.health * (SeamstressStaticValues.cutBossDamageCoefficient * damageInfo.procCoefficient),
                            damageColorIndex = DamageColorIndex.DeathMark,
                            damageType = DamageType.Generic,
                            attacker = damageInfo.attacker,
                            crit = false,
                            force = Vector3.zero,
                            inflictor = damageInfo.inflictor,
                            position = damageInfo.position,
                            procCoefficient = 0f
                        };
                        victim.TakeDamage(cut);
                    }
                    else
                    {
                        DamageInfo cut = new DamageInfo
                        {
                            damage = victim.health * (SeamstressStaticValues.cutDamageCoefficient * damageInfo.procCoefficient),
                            damageColorIndex = DamageColorIndex.DeathMark,
                            damageType = DamageType.Generic,
                            attacker = damageInfo.attacker,
                            crit = false,
                            force = Vector3.zero,
                            inflictor = damageInfo.inflictor,
                            position = damageInfo.position,
                            procCoefficient = 0f
                        };
                        victim.TakeDamage(cut);
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
            CharacterBody attacker = damageReport.attacker.GetComponent<CharacterBody>();
            Transform transform = attacker.modelLocator.transform;
            GameObject attackerObject = attacker.gameObject;
            if (NetworkServer.active)
            {
                if (damageInfo.HasModdedDamageType(AddNeedlesKill))
                {
                    if (attacker.GetBuffCount(SeamstressBuffs.needles) < SeamstressStaticValues.maxNeedleAmount + attacker.skillLocator.special.maxStock - 1)
                    {
                        attacker.AddBuff(SeamstressBuffs.needles);
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
