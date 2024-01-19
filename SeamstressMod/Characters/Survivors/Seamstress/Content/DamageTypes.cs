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
        public static DamageAPI.ModdedDamageType CutDamageNeedle;
        public static DamageAPI.ModdedDamageType AddNeedlesKill;
        public static DamageAPI.ModdedDamageType AddNeedlesDamage;
        public static DamageAPI.ModdedDamageType ResetWeave;
        public static DamageAPI.ModdedDamageType ResetWeakWeave;
        public static DamageAPI.ModdedDamageType Empty;
        internal static void Init()
        {
            Empty = DamageAPI.ReserveDamageType();
            CutDamage = DamageAPI.ReserveDamageType();
            CutDamageNeedle = DamageAPI.ReserveDamageType();
            AddNeedlesKill = DamageAPI.ReserveDamageType();
            AddNeedlesDamage = DamageAPI.ReserveDamageType();
            ResetWeave = DamageAPI.ReserveDamageType();
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
            if (!damageInfo.attacker && !damageInfo.inflictor)
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
                if (damageInfo.HasModdedDamageType(CutDamage) || damageInfo.HasModdedDamageType(CutDamageNeedle))
                {
                    if(damageReport.victimIsBoss)
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
                        float lifeSteal = cut.damage * SeamstressStaticValues.cutHealCoefficient;
                        if (lifeSteal > attacker.maxHealth * SeamstressStaticValues.maxNeedleHeal)
                        {
                            lifeSteal = attacker.maxHealth * SeamstressStaticValues.maxNeedleHeal;
                        }
                        attacker.healthComponent.Heal(lifeSteal, default(ProcChainMask));
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
                        float lifeSteal = cut.damage * SeamstressStaticValues.cutHealCoefficient;
                        if (lifeSteal > attacker.maxHealth * SeamstressStaticValues.maxNeedleHeal)
                        {
                            lifeSteal = attacker.maxHealth * SeamstressStaticValues.maxNeedleHeal;
                        }
                        attacker.healthComponent.Heal(lifeSteal, default(ProcChainMask));
                    }
                }
            }
        }
        private static void GlobalEventManager_onCharacterDeathGlobal(DamageReport damageReport)
        {
            DamageInfo damageInfo = damageReport.damageInfo;
            if (!damageInfo.attacker)
            {
                return;
            }
            CharacterBody attacker = damageReport.attacker.GetComponent<CharacterBody>();
            Transform transform = attacker.modelLocator.transform;
            GameObject attackerObject = attacker.gameObject;
            if (NetworkServer.active)
            {
                if (damageInfo.HasModdedDamageType(ResetWeave))
                {
                    attacker.skillLocator.secondary.Reset();
                    if ((bool)transform && (bool)SeamstressAssets.weaveDashOnKill)
                    {
                        Util.PlaySound("Play_imp_overlord_teleport_end", attackerObject);
                        UnityEngine.Object.Instantiate<GameObject>(SeamstressAssets.weaveDashOnKill, transform);
                    }
                }
                if (damageInfo.HasModdedDamageType(ResetWeakWeave))
                {
                    if(attacker.skillLocator.secondary.stock < attacker.skillLocator.secondary.maxStock)
                    {
                        attacker.skillLocator.secondary.AddOneStock();
                    }
                    if ((bool)transform && (bool)SeamstressAssets.weaveDashOnKill)
                    {
                        Util.PlaySound("Play_UI_cooldownRefresh", attackerObject);
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
