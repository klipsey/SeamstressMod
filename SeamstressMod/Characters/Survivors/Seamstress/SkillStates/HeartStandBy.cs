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
using SeamstressMod.Survivors.Seamstress;
namespace SeamstressMod.SkillStates
{
    public class HeartStandBy : BaseState
    {
        public static GameObject chain = SeamstressAssets.chainToHeart;
        
        public static AnimationCurve yankSuitabilityCurve;

        private CharacterBody ownerBody;

        private SeamstressController seamCon;

        private GameObject owner;

        private TeamIndex teamIndex = TeamIndex.None;

        private float snapBackDelay = 1f;

        private bool hasFired;

        private bool splat;
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
            seamCon = owner.GetComponent<SeamstressController>();
            chain.GetComponent<DestroyOnCondition>().enabled = true;
            chain.GetComponent<DestroyOnCondition>().seamCon = this.seamCon;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (NetworkServer.active)
            {
                if (seamCon.inButchered && !hasFired)
                {
                    ChainUpdate(SeamstressStaticValues.butcheredDuration);
                    hasFired = true;
                }
                if (!seamCon.inButchered && base.fixedAge > 1f)
                {
                    if (!splat)
                    {
                        splat = true;
                        chain.GetComponent<DestroyOnCondition>().enabled = false;
                        ChainUpdate(1f);
                    }
                    snapBackDelay -= Time.fixedDeltaTime;
                    if(snapBackDelay <= 0f) EntityState.Destroy(base.gameObject);
                }
            }
        }

        private void ChainUpdate(float num)
        {
            Vector3 position = base.transform.position;
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
            base.OnExit();
        }
    }
}


