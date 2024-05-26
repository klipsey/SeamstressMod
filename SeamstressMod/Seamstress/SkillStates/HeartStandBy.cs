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

        private TeamIndex teamIndex = TeamIndex.None;

        private float snapBackDelay;

        private bool hasFired;

        private bool splat;

        private bool ownerIsEmpowered;
        public override void OnEnter()
        {
            base.OnEnter();
            splat = false;
            ProjectileController component = GetComponent<ProjectileController>();
            if ((bool)component)
            {
                owner = component.owner;
                teamIndex = component.teamFilter.teamIndex;
            }
            PlayAnimation("Base", "SpawnToIdle");
            Util.PlaySound("Play_treeBot_R_yank", owner);
            seamstressController = owner.GetComponent<SeamstressController>();
            ownerBody = owner.GetComponent<CharacterBody>();
            if (seamstressController.blue) this.chain = SeamstressAssets.chainToHeart2;
            //i need to delete this but i have no clue if its keeping everything together or not
            chain.GetComponent<DestroyOnCondition>().enabled = true;
            chain.GetComponent<DestroyOnCondition>().seamstressController = seamstressController;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            ownerIsEmpowered = ownerBody.HasBuff(SeamstressBuffs.instatiable);
            if (ownerIsEmpowered && !hasFired)
            {
                ChainUpdate(SeamstressStaticValues.insatiableDuration);
                hasFired = true;
            }
            if (!ownerIsEmpowered && base.fixedAge > 1f)
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
                    EntityState.Destroy(this.gameObject);
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
            EffectManager.SpawnEffect(chain, effectData, transmit: false);
        }

        public override void OnExit()
        {
            if (ownerBody != null)
            {
                ownerBody = null;
            }
            base.OnExit();
        }
    }
}


