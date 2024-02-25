using RoR2;
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
                if ((!component2 || component2.teamIndex == myTeamFilter.teamIndex) && !hasFired)
                {
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
