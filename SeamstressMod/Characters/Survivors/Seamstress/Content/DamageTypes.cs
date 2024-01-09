using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace SeamstressMod.Survivors.Seamstress
{
    public class DamageTypes
    {
        public static DamageAPI.ModdedDamageType CutDamage;
        //public static DamageAPI.ModdedDamageType HealDamage;
        //public static DamageAPI.ModdedDamageType HealDamageEmpowered;
        public static DamageAPI.ModdedDamageType ResetWeave;
        public static DamageAPI.ModdedDamageType Empty;
        internal static void Init()
        {
            Empty = DamageAPI.ReserveDamageType();
            CutDamage = DamageAPI.ReserveDamageType();
            //HealDamage = DamageAPI.ReserveDamageType();
            //HealDamageEmpowered = DamageAPI.ReserveDamageType();
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
            if (!damageInfo.attacker)
            {
                return;
            }
            HealthComponent victim = damageReport.victim;
            CharacterBody attacker = damageReport.attackerBody;
            if (NetworkServer.active)
            {
                if (damageInfo.HasModdedDamageType(CutDamage))
                {
                    if(damageReport.victimIsBoss)
                    {
                        DamageInfo cut = new DamageInfo
                        {
                            damage = victim.health * 0.025f,
                            damageColorIndex = DamageColorIndex.SuperBleed,
                            damageType = DamageType.Generic,
                            attacker = damageInfo.attacker,
                            crit = false,
                            force = Vector3.zero,
                            inflictor = null,
                            position = damageInfo.position,
                            procCoefficient = 0f
                        };
                        victim.TakeDamage(cut);
                        float lifeSteal = cut.damage * 0.60f;
                        attacker.healthComponent.Heal(lifeSteal, default(ProcChainMask));
                    }
                    else
                    {
                        DamageInfo cut = new DamageInfo
                        {
                            damage = victim.health * 0.05f,
                            damageColorIndex = DamageColorIndex.SuperBleed,
                            damageType = DamageType.Generic,
                            attacker = damageInfo.attacker,
                            crit = false,
                            force = Vector3.zero,
                            inflictor = null,
                            position = damageInfo.position,
                            procCoefficient = 0f
                        };
                        victim.TakeDamage(cut);
                        float lifeSteal = cut.damage * 0.60f;
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
            if (NetworkServer.active)
            {
                if (damageInfo.HasModdedDamageType(ResetWeave))
                {
                    attacker.skillLocator.secondary.Reset();
                }
            }
        }
    }
}
