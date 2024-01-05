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
        public static DamageAPI.ModdedDamageType HealDamage;
        public static DamageAPI.ModdedDamageType HealDamageEmpowered;
        public static DamageAPI.ModdedDamageType ResetWeave;
        public static DamageAPI.ModdedDamageType Empty;
        internal static void Init()
        {
            Empty = DamageAPI.ReserveDamageType();
            CutDamage = DamageAPI.ReserveDamageType();
            HealDamage = DamageAPI.ReserveDamageType();
            HealDamageEmpowered = DamageAPI.ReserveDamageType();
            ResetWeave = DamageAPI.ReserveDamageType();
            Hook();
        }
        private static void Hook()
        {
            On.RoR2.SetStateOnHurt.OnTakeDamageServer += new On.RoR2.SetStateOnHurt.hook_OnTakeDamageServer(SetStateOnHurt_OnTakeDamageServer);
        }
        private static void SetStateOnHurt_OnTakeDamageServer(On.RoR2.SetStateOnHurt.orig_OnTakeDamageServer orig, SetStateOnHurt self, DamageReport damageReport)
        {
            orig.Invoke(self, damageReport);
            DamageInfo damageInfo = damageReport.damageInfo;
            if(damageInfo.attacker != null) 
            {
                HealthComponent victim = damageReport.victim;
                CharacterBody attacker = damageInfo.attacker.GetComponent<CharacterBody>();
                if (damageInfo.HasModdedDamageType(CutDamage))
                {
                    DamageInfo cut = new DamageInfo
                    {
                        damage = victim.health * 0.1f,
                        damageColorIndex = DamageColorIndex.Nearby,
                        damageType = DamageType.Generic,
                        attacker = damageInfo.attacker,
                        crit = damageInfo.crit,
                        force = Vector3.zero,
                        inflictor = null,
                        position = damageInfo.position,
                        procCoefficient = 1f
                    };
                    victim.TakeDamage(cut);
                }
                if (damageInfo.HasModdedDamageType(HealDamage))
                {
                    float lifeSteal = damageReport.damageDealt * 0.15f;
                    attacker.healthComponent.Heal(lifeSteal, default(ProcChainMask));
                }
                if (damageInfo.HasModdedDamageType(HealDamageEmpowered))
                {
                    float lifeSteal = damageReport.damageDealt * 0.3f;
                    attacker.healthComponent.Heal(lifeSteal, default(ProcChainMask));
                }
                if(damageInfo.HasModdedDamageType(ResetWeave))
                {
                    attacker.skillLocator.secondary.Reset();
                }
            }
        }
    }
}
