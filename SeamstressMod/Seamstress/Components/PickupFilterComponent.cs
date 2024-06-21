using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;
using RoR2.EntityLogic;
using RoR2.Projectile;
using UnityEngine.Events;
using UnityEngine.Networking;
using SeamstressMod.Seamstress.Content;
using System;

namespace SeamstressMod.Seamstress.Components
{
    public class PickupFilterComponent : NetworkBehaviour
    {
        public TeamFilter myTeamFilter;

        public static GameObject pickupEffect = SeamstressAssets.blinkEffect;

        public UnityEvent triggerEvents;

        private bool hasFired;

        private bool hasActivated = false;

        public float pickupTimer;

        public void FixedUpdate()
        {
            pickupTimer -= Time.fixedDeltaTime;
            if (pickupTimer <= 0 && !hasActivated)
            {
                this.transform.root.Find("SeamstressTeamIndicator(Clone)").gameObject.SetActive(true);
                hasActivated = true;
            }
        }

        public void OnTriggerStay(Collider collider)
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
                if ((!component2 || component2.teamIndex == myTeamFilter.teamIndex) && !hasFired && pickupTimer <= 0f && healthComponent.body.baseNameToken == "KENKO_SEAMSTRESS_NAME")
                {
                    string scissorName = gameObject.transform.root.name;
                    if (scissorName == "ScissorR(Clone)")
                    {
                        healthComponent.body.GetComponent<ScissorController>().isRight = true;
                        if(healthComponent.body.TryGetComponent<SeamstressController>(out var seamstressController))
                        {
                            seamstressController.PlayScissorRSwing();
                        }
                    }
                    else if (scissorName == "ScissorL(Clone)")
                    {
                        healthComponent.body.GetComponent<ScissorController>().isRight = false;
                        if (healthComponent.body.TryGetComponent<SeamstressController>(out var seamstressController))
                        {
                            seamstressController.PlayScissorLSwing();
                        }
                    }
                    triggerEvents?.Invoke();
                    Util.PlaySound("sfx_seamstress_swing_scissor", gameObject);
                    if (healthComponent.body.GetBuffCount(SeamstressBuffs.needles) < SeamstressStaticValues.maxNeedleAmount) healthComponent.body.AddBuff(SeamstressBuffs.needles);
                    healthComponent.body.GetComponent<ScissorController>().RpcAddSpecialStock();
                    hasFired = true;
                }
            }
        }
    }
}
