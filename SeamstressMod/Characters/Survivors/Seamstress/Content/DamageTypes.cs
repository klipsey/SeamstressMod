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
        public static DamageAPI.ModdedDamageType PlanarLifeSteal;
        public static DamageAPI.ModdedDamageType BeginHoming;
        public static DamageAPI.ModdedDamageType DotFlag;

        internal static void Init()
        {
            Empty = DamageAPI.ReserveDamageType();
            CutDamage = DamageAPI.ReserveDamageType();
            StitchDamage = DamageAPI.ReserveDamageType();
            AddNeedlesDamage = DamageAPI.ReserveDamageType();
            PlanarLifeSteal = DamageAPI.ReserveDamageType();
            BeginHoming = DamageAPI.ReserveDamageType();
            DotFlag = DamageAPI.ReserveDamageType();
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
                if (damageInfo.HasModdedDamageType(BeginHoming))
                {
                    ProjectileDirectionalTargetFinder s = inflictorObject.GetComponent<ProjectileDirectionalTargetFinder>();
                    if(!inflictorObject.TryGetComponent<ProjectileDirectionalTargetFinder>(out s))
                    {
                        ProjectileDirectionalTargetFinder needleFinder = inflictorObject.AddComponent<ProjectileDirectionalTargetFinder>();
                        needleFinder.lookRange = 35f;
                        needleFinder.lookCone = 180f;
                        needleFinder.targetSearchInterval = 0.2f;
                        needleFinder.onlySearchIfNoTarget = false;
                        needleFinder.allowTargetLoss = true;
                        needleFinder.testLoS = true;
                        needleFinder.ignoreAir = false;
                        needleFinder.flierAltitudeTolerance = Mathf.Infinity;

                        ProjectileSimple needleSimple = inflictorObject.GetComponent<ProjectileSimple>();
                        needleSimple.desiredForwardSpeed = 150f;
                        needleSimple.lifetime = 1f;
                        needleSimple.updateAfterFiring = true;
                    }
                }
                if (damageInfo.HasModdedDamageType(PlanarLifeSteal))
                {
                    attackerBody.healthComponent.Heal(damageReport.damageDealt * SeamstressStaticValues.planarLifeSteal, default(ProcChainMask), true);
                }
            }
        }
    }
}
