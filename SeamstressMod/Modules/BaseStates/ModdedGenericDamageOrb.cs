using R2API;
using RoR2.Orbs;
using RoR2;
using UnityEngine;

namespace SeamstressMod.Modules.BaseStates
{
    public class ModdedGenericDamageOrb : GenericDamageOrb
    {
        public DamageAPI.ModdedDamageType moddedDamageType;

        public override void OnArrival()
        {
            if ((bool)target)
            {
                HealthComponent healthComponent = target.healthComponent;
                if ((bool)healthComponent)
                {
                    DamageInfo damageInfo = new DamageInfo();
                    damageInfo.damage = damageValue;
                    damageInfo.attacker = attacker;
                    damageInfo.inflictor = null;
                    damageInfo.force = Vector3.zero;
                    damageInfo.crit = isCrit;
                    damageInfo.procChainMask = procChainMask;
                    damageInfo.procCoefficient = procCoefficient;
                    damageInfo.position = target.transform.position;
                    damageInfo.damageColorIndex = damageColorIndex;
                    damageInfo.damageType = damageType;
                    damageInfo.AddModdedDamageType(moddedDamageType);
                    healthComponent.TakeDamage(damageInfo);
                    GlobalEventManager.instance.OnHitEnemy(damageInfo, healthComponent.gameObject);
                    GlobalEventManager.instance.OnHitAll(damageInfo, healthComponent.gameObject);
                }
            }
        }
    }
}
