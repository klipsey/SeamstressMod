﻿using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;
using RoR2.EntityLogic;
using RoR2.Projectile;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace SeamstressMod.Survivors.Seamstress
{
    public class PickupFilter : MonoBehaviour
    {
        public TeamFilter myTeamFilter;

        public UnityEvent triggerEvents;

        private bool hasFired;

        private float pickupTimer = 2f;

        public void FixedUpdate()
        {
            pickupTimer -= Time.fixedDeltaTime;
        }
        public void OnTriggerEnter(Collider collider)
        {
            if (!NetworkServer.active || !collider)
            {
                return;
            }
            HurtBox component = collider.GetComponent<HurtBox>();
            if (!component)
            {
                return;
            }
            HealthComponent healthComponent = component.healthComponent;
            if ((bool)healthComponent)
            {
                TeamComponent component2 = healthComponent.GetComponent<TeamComponent>();
                if ((!component2 || component2.teamIndex == myTeamFilter.teamIndex) && !hasFired && pickupTimer < 0f && healthComponent.body.baseNameToken == "KENKO_SEAMSTRESS_NAME")
                {
                    string hi = base.gameObject.transform.GetParent().GetParent().name;
                    if (hi == "ScissorR(Clone)" || hi == "ScissorRButchered(Clone)")
                    {
                        healthComponent.body.GetComponent<ScissorController>().isRight = true;
                    }
                    else if (hi == "ScissorL(Clone)" || hi == "ScissorLButchered(Clone)")
                    {
                        healthComponent.body.GetComponent<ScissorController>().isRight = false;
                    }

                    triggerEvents?.Invoke();
                    if (healthComponent.body.GetBuffCount(SeamstressBuffs.needles) < 5) healthComponent.body.AddBuff(SeamstressBuffs.needles);
                    healthComponent.body.GetComponent<ScissorController>().RpcAddSpecialStock();
                    hasFired = true;
                }
                //else damage?
                //the enemy dying destroys the needles
            }
        }
    }
}