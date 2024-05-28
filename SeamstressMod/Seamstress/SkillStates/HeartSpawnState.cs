using System.Linq;
using RoR2.CharacterAI;
using RoR2.Navigation;
using UnityEngine;
using R2API;
using UnityEngine.Networking;
using EntityStates;
using UnityEngine.Networking.Match;
using RoR2;
using SeamstressMod.Seamstress.Content;
using RoR2.Projectile;

namespace SeamstressMod.Seamstress.SkillStates
{
    public class HeartSpawnState : BaseState
    {
        public static float duration = 0.01f;

        public static string enterSoundString = "Play_bleedOnCritAndExplode_impact";

        public override void OnEnter()
        {
            base.OnEnter();
            if (NetworkServer.active) DotController.InflictDot(base.gameObject.GetComponent<ProjectileController>().owner, base.gameObject, Dots.SeamstressSelfBleed, SeamstressStaticValues.insatiableDuration, 1, 1u);
            Util.PlaySound(enterSoundString, gameObject);
            FindModelChild("ChargeUpFX").gameObject.SetActive(value: true);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (fixedAge >= duration)
            {
                outer.SetNextState(new HeartStandBy());
            }
        }
    }
}