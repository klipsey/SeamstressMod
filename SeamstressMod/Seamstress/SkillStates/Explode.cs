using RoR2;
using RoR2.Projectile;
using UnityEngine;
using R2API;
using SeamstressMod.Modules.BaseStates;
using SeamstressMod.Seamstress.Components;
using System.ComponentModel;
using SeamstressMod.Seamstress.Content;
using UnityEngine.Networking;


namespace SeamstressMod.Seamstress.SkillStates
{
    public class Explode : BaseSeamstressSkillState
    {
        public float BaseDuration = 0.8f;
        public BlastAttack blastAttack;
        private bool _hasDelayed;
        private GameObject explosionEffect = SeamstressAssets.genericImpactExplosionEffect;
        private GameObject slamEffect = SeamstressAssets.slamEffect;
        public override void OnEnter()
        {
            base.OnEnter();

            explosionEffect = seamstressController.blue ? SeamstressAssets.genericImpactExplosionEffect2 : SeamstressAssets.genericImpactExplosionEffect;
            slamEffect = seamstressController.blue ? SeamstressAssets.slamEffect2 : SeamstressAssets.slamEffect;
            blastAttack = new BlastAttack();
            blastAttack.position = this.transform.position;
            blastAttack.baseDamage = SeamstressConfig.explodeDamageCoefficient.Value * damageStat;
            blastAttack.baseForce = 800f;
            blastAttack.bonusForce = Vector3.zero;
            blastAttack.radius = 25f;
            blastAttack.attacker = this.gameObject;
            blastAttack.inflictor = this.gameObject;
            blastAttack.teamIndex = this.teamComponent.teamIndex;
            blastAttack.crit = RollCrit();
            blastAttack.procChainMask = default;
            blastAttack.procCoefficient = 1f;
            blastAttack.falloffModel = BlastAttack.FalloffModel.Linear;
            blastAttack.damageColorIndex = DamageColorIndex.Default;
            blastAttack.damageType = DamageType.Stun1s | DamageType.AOE;
            blastAttack.damageType.AddModdedDamageType(DamageTypes.SeamstressLifesteal);

            skillLocator.utility.UnsetSkillOverride(gameObject, SeamstressSurvivor.explodeSkillDef, GenericSkill.SkillOverridePriority.Contextual);

            PlayCrossfade("FullBody, Override", "RipHeart", "Dash.playbackRate", (BaseDuration / attackSpeedStat) * 1.8f, (BaseDuration / attackSpeedStat) * 0.05f);
            Util.PlayAttackSpeedSound("Play_imp_overlord_attack2_tell", gameObject, attackSpeedStat);

            if(NetworkServer.active)
            {
                this.characterBody.AddBuff(RoR2Content.Buffs.Slow50);
                this.characterBody.AddTimedBuff(RoR2Content.Buffs.SmallArmorBoost, BaseDuration / attackSpeedStat);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if(base.isAuthority)
            {
                if (base.fixedAge > (0.5f / attackSpeedStat) && !_hasDelayed)
                {
                    Util.PlaySound("Play_imp_overlord_teleport_end", this.gameObject);
                    blastAttack.Fire();
                    EffectManager.SpawnEffect(explosionEffect, new EffectData
                    {
                        origin = characterBody.corePosition,
                        rotation = Quaternion.identity,
                    }, true);
                    EffectManager.SpawnEffect(slamEffect, new EffectData
                    {
                        origin = characterBody.corePosition,
                        rotation = Quaternion.identity,
                    }, true);

                    _hasDelayed = true;
                }

                if (base.fixedAge >= BaseDuration / attackSpeedStat && _hasDelayed)
                {
                    outer.SetNextStateToMain();
                }
            }

            if(NetworkServer.active && this.characterBody.HasBuff(RoR2Content.Buffs.Slow50) && _hasDelayed)
            {
                this.characterBody.RemoveBuff(RoR2Content.Buffs.Slow50);
            }
        }

        public override void OnExit()
        {
            if (NetworkServer.active)
            {
                this.characterBody.RemoveOldestTimedBuff(SeamstressBuffs.SeamstressInsatiableBuff);
                if(this.characterBody.HasBuff(RoR2Content.Buffs.Slow50)) this.characterBody.RemoveBuff(RoR2Content.Buffs.Slow50);
            }

            base.OnExit();
        }
    }
}
