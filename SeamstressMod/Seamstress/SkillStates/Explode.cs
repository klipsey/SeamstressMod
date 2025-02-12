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
        public BlastAttack BlastAttack;
        private bool _hasDelayed;
        public override void OnEnter()
        {
            base.OnEnter();

            BlastAttack = new BlastAttack();
            BlastAttack.position = this.transform.position;
            BlastAttack.baseDamage = SeamstressConfig.explodeDamageCoefficient.Value * damageStat;
            BlastAttack.baseForce = 800f;
            BlastAttack.bonusForce = Vector3.zero;
            BlastAttack.radius = 25f;
            BlastAttack.attacker = this.gameObject;
            BlastAttack.inflictor = this.gameObject;
            BlastAttack.teamIndex = this.teamComponent.teamIndex;
            BlastAttack.crit = RollCrit();
            BlastAttack.procChainMask = default;
            BlastAttack.procCoefficient = 1f;
            BlastAttack.falloffModel = BlastAttack.FalloffModel.Linear;
            BlastAttack.damageColorIndex = DamageColorIndex.Default;
            BlastAttack.damageType = DamageType.Stun1s | DamageType.AOE;
            BlastAttack.AddModdedDamageType(DamageTypes.SeamstressLifesteal);

            skillLocator.utility.UnsetSkillOverride(gameObject, SeamstressSurvivor.explodeSkillDef, GenericSkill.SkillOverridePriority.Contextual);

            PlayCrossfade("FullBody, Override", "RipHeart", "Dash.playbackRate", (BaseDuration / attackSpeedStat) * 1.8f, (BaseDuration / attackSpeedStat) * 0.05f);
            Util.PlayAttackSpeedSound("Play_imp_overlord_attack2_tell", gameObject, attackSpeedStat);

            if (NetworkServer.active)
            {
                this.characterBody.AddBuff(RoR2Content.Buffs.Slow50);
                this.characterBody.AddTimedBuff(RoR2Content.Buffs.SmallArmorBoost, BaseDuration / attackSpeedStat);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.isAuthority)
            {
                if (base.fixedAge > (0.5f / attackSpeedStat) && !_hasDelayed)
                {
                    Util.PlaySound("Play_imp_overlord_teleport_end", this.gameObject);
                    BlastAttack.Fire();
                    EffectManager.SpawnEffect(SeamstressAssets.genericImpactExplosionEffect, new EffectData
                    {
                        origin = characterBody.corePosition,
                        rotation = Quaternion.identity,
                        color = SeamstressAssets.coolRed,
                    }, true);
                    EffectManager.SpawnEffect(SeamstressAssets.slamEffect, new EffectData
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

            if (NetworkServer.active && this.characterBody.HasBuff(RoR2Content.Buffs.Slow50) && _hasDelayed)
            {
                this.characterBody.RemoveBuff(RoR2Content.Buffs.Slow50);
            }
        }

        public override void OnExit()
        {
            if (NetworkServer.active)
            {
                this.characterBody.RemoveOldestTimedBuff(SeamstressBuffs.SeamstressInsatiableBuff);
                if (this.characterBody.HasBuff(RoR2Content.Buffs.Slow50)) this.characterBody.RemoveBuff(RoR2Content.Buffs.Slow50);
            }

            base.OnExit();
        }
    }
}