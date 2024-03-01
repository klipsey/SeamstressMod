using HG;
using Newtonsoft.Json.Linq;
using R2API;
using RoR2;
using RoR2.Projectile;
using SeamstressMod.Modules;
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
        public static DamageAPI.ModdedDamageType NoSword;
        public static DamageAPI.ModdedDamageType AddNeedlesDamage;
        public static DamageAPI.ModdedDamageType ButcheredLifeSteal;
        public static DamageAPI.ModdedDamageType ClipLifeSteal;
        internal static void Init()
        {
            Empty = DamageAPI.ReserveDamageType();
            CutDamage = DamageAPI.ReserveDamageType();
            NoSword = DamageAPI.ReserveDamageType();
            AddNeedlesDamage = DamageAPI.ReserveDamageType();
            ButcheredLifeSteal = DamageAPI.ReserveDamageType();
            ClipLifeSteal = DamageAPI.ReserveDamageType();
            Hook();
        }
        private static void Hook()
        {
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
            CharacterBody attackerBody = damageReport.attackerBody;
            GameObject attackerObject = damageReport.attacker.gameObject;
            if (NetworkServer.active)
            {
                if (damageInfo.HasModdedDamageType(NoSword))
                {
                    attackerBody.GetComponent<NeedleController>().RpcAddNeedle();
                }
                if(damageInfo.HasModdedDamageType(AddNeedlesDamage))
                {
                    attackerBody.GetComponent<NeedleController>().RpcAddNeedle();
                }
                if (damageInfo.HasModdedDamageType(ButcheredLifeSteal))
                {
                    attackerBody.healthComponent.Heal(damageReport.damageDealt * SeamstressStaticValues.butcheredLifeSteal, default(ProcChainMask), true);
                }
                if (damageInfo.HasModdedDamageType(ClipLifeSteal))
                {
                    attackerBody.healthComponent.Heal(damageReport.damageDealt * SeamstressStaticValues.butcheredLifeSteal, default(ProcChainMask), true);
                }
            }
        }
    }
}
