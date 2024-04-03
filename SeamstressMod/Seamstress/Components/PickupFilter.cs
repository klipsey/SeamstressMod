using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;
using RoR2.EntityLogic;
using RoR2.Projectile;
using UnityEngine.Events;
using UnityEngine.Networking;
using SeamstressMod.Seamstress.Content;

namespace SeamstressMod.Seamstress.Components
{
    public class PickupFilter : MonoBehaviour
    {
        public TeamFilter myTeamFilter;

        public static GameObject boomEffect = SeamstressAssets.blinkPrefab;

        public UnityEvent triggerEvents;

        private bool hasFired;

        private float pickupTimer = 0.75f;

        public void FixedUpdate()
        {
            pickupTimer -= Time.fixedDeltaTime;
        }
        public void OnTriggerEnter(Collider collider)
        {
            if (!collider)
            {
                return;
            }
            HurtBox component = collider.GetComponent<HurtBox>();
            if (!component)
            {
                return;
            }
            HealthComponent healthComponent = component.healthComponent;
            if (healthComponent)
            {
                TeamComponent component2 = healthComponent.GetComponent<TeamComponent>();
                if ((!component2 || component2.teamIndex == myTeamFilter.teamIndex) && !hasFired && pickupTimer < 0f && healthComponent.body.baseNameToken == "KENKO_SEAMSTRESS_NAME")
                {
                    string hi = gameObject.transform.root.name;
                    if (hi == "ScissorR(Clone)")
                    {
                        healthComponent.body.GetComponent<ScissorController>().isRight = true;
                    }
                    else if (hi == "ScissorL(Clone)")
                    {
                        healthComponent.body.GetComponent<ScissorController>().isRight = false;
                    }
                    EffectManager.SpawnEffect(boomEffect, new EffectData
                    {
                        origin = Util.GetCorePosition(gameObject),
                        rotation = Quaternion.identity,
                        scale = 1.5f,
                    }, false);
                    triggerEvents?.Invoke();
                    Util.PlaySound("Play_bandit2_m2_alt_throw", gameObject);
                    if (healthComponent.body.GetBuffCount(SeamstressBuffs.needles) < SeamstressStaticValues.maxNeedleAmount) healthComponent.body.AddBuff(SeamstressBuffs.needles);
                    healthComponent.body.GetComponent<ScissorController>().RpcAddSpecialStock();
                    hasFired = true;
                }
            }
        }
    }
}
