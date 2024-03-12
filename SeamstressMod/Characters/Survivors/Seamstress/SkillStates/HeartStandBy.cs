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
        public static AnimationCurve yankSuitabilityCurve;

        public static string exitSoundString = "Play_item_proc_bounceChain";

        public static GameObject enterEffectPrefab;

        public static GameObject exitEffectPrefab = SeamstressAssets.clawsEffect;

        private CharacterBody ownerBody;

        private SeamstressController seamCon;

        private GameObject owner;

        private TeamIndex teamIndex = TeamIndex.None;

        private bool isOwnerButchered;

        private float snapBackDelay = 1f;

        private float delay = 1f;

        public override void OnEnter()
        {
            base.OnEnter();
            ProjectileController component = GetComponent<ProjectileController>();
            if ((bool)component)
            {
                owner = component.owner;
                teamIndex = component.teamFilter.teamIndex;
            }
            PlayAnimation("Base", "SpawnToIdle");
            if ((bool)enterEffectPrefab)
            {
                EffectManager.SimpleEffect(enterEffectPrefab, base.transform.position, base.transform.rotation, transmit: false);
            }
            seamCon = owner.GetComponent<SeamstressController>();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (NetworkServer.active)
            {
                delay -= Time.fixedDeltaTime;
                if (seamCon.inButchered && delay <= 0f)
                {
                    delay += 0.5f;
                    ChainUpdate();
                }
                else if (!seamCon.inButchered)
                {
                    snapBackDelay -= Time.fixedDeltaTime;
                    if(snapBackDelay <= 0f) EntityState.Destroy(base.gameObject);
                }
            }
        }

        private void ChainUpdate()
        {
            Vector3 position = base.transform.position;
            HurtBox hurtBox = owner.GetComponent<HurtBox>();
            if(ownerBody == null || ownerBody.baseNameToken != "KENKO_SEAMSTRESS_NAME")
            {
                ownerBody = owner.GetComponent<CharacterBody>();
                HurtBox hurtBoxReference = hurtBox;
                HurtBoxGroup hurtBoxGroup = hurtBox.hurtBoxGroup;
                for (int j = 0; (float)j < Mathf.Min(4f, ownerBody.radius * 2f); j++)
                {
                    EffectData effectData = new EffectData
                    {
                        scale = 1f,
                        origin = position,
                        genericBool = seamCon.inButchered,
                    };
                    effectData.SetHurtBoxReference(hurtBoxReference);
                    EffectManager.SpawnEffect(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/OrbEffects/EntangleOrbEffect"), effectData, transmit: true);
                    hurtBoxReference = hurtBoxGroup.hurtBoxes[Random.Range(0, hurtBoxGroup.hurtBoxes.Length)];
                }
            }
        }

        public override void OnExit()
        {
            if (ownerBody != null)
            {
                ownerBody = null;
            }
            Util.PlaySound(exitSoundString, base.gameObject);
            if ((bool)exitEffectPrefab)
            {
                Vector3 effectPos = this.transform.localPosition;
                RaycastHit raycastHit;
                if (Physics.Raycast(effectPos, Vector3.one, out raycastHit, 10f, LayerIndex.world.mask))
                {
                    effectPos = raycastHit.point;
                }
                EffectManager.SpawnEffect(exitEffectPrefab, new EffectData
                {
                    origin = effectPos,
                    rotation = Quaternion.identity,
                    color = SeamstressAssets.coolRed,
                }, true);
            }
            base.OnExit();
        }
    }
}


