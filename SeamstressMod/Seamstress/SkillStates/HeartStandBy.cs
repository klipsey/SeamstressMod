using System.Linq;
using EntityStates;
using RoR2.Projectile;
using RoR2.Orbs;
using UnityEngine;
using R2API;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using RoR2;
using System.Collections.Generic;
using SeamstressMod.Seamstress.Components;
using SeamstressMod.Seamstress.Content;
namespace SeamstressMod.Seamstress.SkillStates
{
    public class HeartStandBy : BaseState
    {
        public GameObject chain = SeamstressAssets.chainToHeart;

        public static AnimationCurve yankSuitabilityCurve;

        private CharacterBody ownerBody;

        private SeamstressController seamstressController;

        private GameObject owner;

        private float snapBackDelay;

        private bool hasFired;

        private bool splat;

        private static float bleedInterval = 0.2f;

        private float bleedTimer;

        public override void OnEnter()
        {
            base.OnEnter();
            splat = false;
            ProjectileController component = GetComponent<ProjectileController>();
            if (component)
            {
                owner = component.owner;
            }
            PlayAnimation("Base", "SpawnToIdle");
            Util.PlaySound("Play_treeBot_R_yank", owner);
            seamstressController = owner.GetComponent<SeamstressController>();
            ownerBody = owner.GetComponent<CharacterBody>();
            chain.GetComponent<DestroyOnCondition>().enabled = true;
            chain.GetComponent<DestroyOnCondition>().ownerBody = ownerBody;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (ownerBody && ownerBody.HasBuff(SeamstressBuffs.SeamstressInsatiableBuff))
            {
                HandleBleed();
                if (!hasFired)
                {
                    hasFired = true;
                    ChainUpdate(SeamstressConfig.insatiableDuration.Value);
                }
            }
            else if (ownerBody && !ownerBody.HasBuff(SeamstressBuffs.SeamstressInsatiableBuff) && hasFired)
            {
                if (!splat)
                {
                    splat = true;
                    snapBackDelay = (ownerBody.corePosition - transform.position).magnitude / 10f;
                    snapBackDelay = Mathf.Clamp(snapBackDelay, 0.2f, 1f);
                    chain.GetComponent<DestroyOnCondition>().enabled = false;
                    ChainUpdate(snapBackDelay);
                }

                snapBackDelay -= Time.fixedDeltaTime;

                if (snapBackDelay <= 0.2f)
                {
                    outer.SetNextStateToMain();
                }
            }
        }
        private void HandleBleed()
        {
            bleedTimer += Time.fixedDeltaTime;

            if (bleedTimer >= bleedInterval)
            {
                bleedTimer = 0f;

                if (NetworkServer.active)
                {
                    DamageInfo damageInfo = new DamageInfo
                    {
                        damage = (ownerBody.healthComponent.fullCombinedHealth - (ownerBody.healthComponent.fullCombinedHealth - (ownerBody.healthComponent.health + ownerBody.healthComponent.shield))) * 0.02f,
                        damageType = DamageType.NonLethal | DamageType.BypassArmor | DamageType.BypassBlock | DamageType.DoT,
                        dotIndex = DotController.DotIndex.Bleed,
                        position = ownerBody.corePosition,
                        attacker = base.gameObject,
                        procCoefficient = 0f,
                        crit = false,
                        damageColorIndex = DamageColorIndex.Bleed,
                    };

                    ownerBody.healthComponent.TakeDamage(damageInfo);
                }
            }
        }
        private void ChainUpdate(float num)
        {
            Vector3 position = transform.position;
            EffectData effectData = new EffectData
            {
                scale = 1f,
                origin = position,
                genericFloat = num,
            };
            effectData.SetHurtBoxReference(owner);

            EffectManager.SpawnEffect(chain, effectData, transmit: true);
        }

        public override void OnExit()
        {
            if (ownerBody != null)
            {
                ownerBody = null;
            }

            EntityState.Destroy(this.gameObject);

            base.OnExit();
        }
    }
}