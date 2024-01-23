using R2API;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;

namespace SeamstressMod.Survivors.Seamstress
{
    public class DamageTypes
    {
        public static DamageAPI.ModdedDamageType CutDamage;
        public static DamageAPI.ModdedDamageType AddNeedlesKill;
        public static DamageAPI.ModdedDamageType AddNeedlesDamage;
        public static DamageAPI.ModdedDamageType ResetWeakWeave;
        public static DamageAPI.ModdedDamageType Empty;
        internal static void Init()
        {
            Empty = DamageAPI.ReserveDamageType();
            CutDamage = DamageAPI.ReserveDamageType();
            AddNeedlesKill = DamageAPI.ReserveDamageType();
            AddNeedlesDamage = DamageAPI.ReserveDamageType();
            ResetWeakWeave = DamageAPI.ReserveDamageType();
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
            CharacterBody attacker = damageReport.attackerBody;
            GameObject attackerObject = damageReport.attacker.gameObject;
            if (NetworkServer.active)
            {
                if(damageInfo.HasModdedDamageType(AddNeedlesDamage))
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
                    if (damageReport.victimIsBoss)
                    {
                        DamageInfo cut = new DamageInfo
                        {
                            damage = victim.health * (SeamstressStaticValues.cutBossDamageCoefficient * damageInfo.procCoefficient),
                            damageColorIndex = DamageColorIndex.WeakPoint,
                            damageType = DamageType.Generic,
                            attacker = damageInfo.attacker,
                            crit = false,
                            force = Vector3.zero,
                            inflictor = damageInfo.inflictor,
                            position = damageInfo.position,
                            procCoefficient = 0f
                        };
                        victim.TakeDamage(cut);
                        /*
                        float lifeSteal = cut.damage * SeamstressStaticValues.cutHealCoefficient;
                        if (lifeSteal > attacker.maxHealth * SeamstressStaticValues.maxCutHeal)
                        {
                            lifeSteal = attacker.maxHealth * SeamstressStaticValues.maxCutHeal;
                        }
                        attacker.healthComponent.Heal(lifeSteal, default(ProcChainMask));
                        */
                    }
                    else
                    {
                        DamageInfo cut = new DamageInfo
                        {
                            damage = victim.health * (SeamstressStaticValues.cutDamageCoefficient * damageInfo.procCoefficient),
                            damageColorIndex = DamageColorIndex.WeakPoint,
                            damageType = DamageType.Generic,
                            attacker = damageInfo.attacker,
                            crit = false,
                            force = Vector3.zero,
                            inflictor = damageInfo.inflictor,
                            position = damageInfo.position,
                            procCoefficient = 0f
                        };
                        victim.TakeDamage(cut);
                        /*
                        float lifeSteal = cut.damage * SeamstressStaticValues.cutHealCoefficient;
                        if (lifeSteal > attacker.maxHealth * SeamstressStaticValues.maxCutHeal)
                        {
                            lifeSteal = attacker.maxHealth * SeamstressStaticValues.maxCutHeal;
                        }
                        attacker.healthComponent.Heal(lifeSteal, default(ProcChainMask));
                        */
                    }
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
                /*
                if (damageInfo.HasModdedDamageType(ResetWeave))
                {2
                    attacker.skillLocator.secondary.Reset();
                    if ((bool)transform && (bool)SeamstressAssets.weaveDashOnKill)
                    {
                        Util.PlaySound("Play_imp_overlord_teleport_end", attackerObject);
                        UnityEngine.Object.Instantiate<GameObject>(SeamstressAssets.weaveDashOnKill, transform);
                    }
                }
                */
                if (damageInfo.HasModdedDamageType(ResetWeakWeave))
                {
                    attacker.skillLocator.secondary.rechargeStopwatch += attacker.skillLocator.secondary.cooldownRemaining * SeamstressStaticValues.killRefund;
                    if ((bool)transform && (bool)SeamstressAssets.expungeEffect)
                    {
                        Util.PlaySound("Play_merc_shift_end", attackerObject);
                    }
                }
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
