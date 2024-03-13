using System.Linq;
using RoR2.CharacterAI;
using RoR2.Navigation;
using UnityEngine;
using SeamstressMod.Modules.BaseStates;
using SeamstressMod.Survivors.Seamstress;
using R2API;
using UnityEngine.Networking;
using EntityStates;
using UnityEngine.Networking.Match;
using RoR2;
namespace SeamstressMod.SkillStates
{
    public class HeartSpawnState : BaseState
    {
        public static float duration = 0.01f;

        public static string enterSoundString = "Play_bleedOnCritAndExplode_impact";

        public override void OnEnter()
        {
            base.OnEnter();
            Util.PlaySound(enterSoundString, base.gameObject);
            FindModelChild("ChargeUpFX").gameObject.SetActive(value: true);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.fixedAge >= duration)
            {
                outer.SetNextState(new HeartStandBy());
            }
        }
    }
}


