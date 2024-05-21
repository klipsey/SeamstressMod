using HG;
using Newtonsoft.Json.Linq;
using R2API;
using RoR2;
using RoR2.Projectile;
using SeamstressMod.Modules;
using SeamstressMod.Seamstress.Components;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;
using static RoR2.DotController;

namespace SeamstressMod.Seamstress.Content
{
    public static class DamageTypes
    {
        public static DamageAPI.ModdedDamageType Empty;
        public static DamageAPI.ModdedDamageType CutDamage;
        public static DamageAPI.ModdedDamageType NoScissors;
        public static DamageAPI.ModdedDamageType SeamstressLifesteal;
        public static DamageAPI.ModdedDamageType PullDamage;
        internal static void Init()
        {
            Empty = DamageAPI.ReserveDamageType();
            CutDamage = DamageAPI.ReserveDamageType();
            NoScissors = DamageAPI.ReserveDamageType();
            SeamstressLifesteal = DamageAPI.ReserveDamageType();
            PullDamage = DamageAPI.ReserveDamageType();
            Hook();
        }
        private static void Hook()
        {
            GlobalEventManager.onServerDamageDealt += GlobalEventManager_onServerDamageDealt;
            On.RoR2.GlobalEventManager.OnHitEnemy += new On.RoR2.GlobalEventManager.hook_OnHitEnemy(GlobalEventManager_OnHitEnemy);
        }
        private static void GlobalEventManager_OnHitEnemy(On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, GlobalEventManager self, DamageInfo damageInfo, GameObject victim)
        {
            orig.Invoke(self, damageInfo, victim);
            CharacterBody victimBody = victim.GetComponent<CharacterBody>();
            if (damageInfo.HasModdedDamageType(PullDamage))
            {
                PullComponent victimPull = victim.AddComponent<PullComponent>();
                victimPull.attackerBody = damageInfo.attacker.GetComponent<CharacterBody>();
            }
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
            if (NetworkServer.active && attackerBody && attackerBody.baseNameToken == "KENKO_SEAMSTRESS_NAME")
            {
                SeamstressController seamstressController = attackerBody.GetComponent<SeamstressController>();
                if (damageInfo.HasModdedDamageType(DamageTypes.CutDamage))
                {
                    if (victimBody.isBoss)
                    {
                        DotController.InflictDot(victimBody.gameObject, attackerBody.gameObject, Dots.SeamstressBossDot, SeamstressStaticValues.cutDuration, damageInfo.procCoefficient);
                    }
                    else
                    {
                        DotController.InflictDot(victimBody.gameObject, attackerBody.gameObject, Dots.SeamstressDot, SeamstressStaticValues.cutDuration, damageInfo.procCoefficient);
                    }
                }
                if (damageInfo.HasModdedDamageType(NoScissors))
                {
                    Util.PlaySound("Play_bandit2_m2_alt_throw", attackerObject);
                    if (attackerBody.GetBuffCount(SeamstressBuffs.needles) < SeamstressStaticValues.maxNeedleAmount) attackerBody.AddBuff(SeamstressBuffs.needles);
                }
                if (damageInfo.HasModdedDamageType(SeamstressLifesteal))
                {
                    if(seamstressController)
                    {
                        float healthMissing = (attackerBody.healthComponent.health + attackerBody.healthComponent.shield) / (attackerBody.healthComponent.fullHealth + attackerBody.healthComponent.fullShield);
                        attackerBody.healthComponent.Heal(damageReport.damageDealt * (healthMissing * SeamstressStaticValues.passiveHealingScaling), default, true);
                    }
                }
            }
        }
    }
}
