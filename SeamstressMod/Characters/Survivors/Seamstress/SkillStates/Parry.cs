using EntityStates;
using R2API;
using RoR2;
using SeamstressMod.Modules.BaseStates;
using SeamstressMod.Survivors.Seamstress;
using UnityEngine;
using UnityEngine.Networking;

namespace SeamstressMod.SkillStates
{
    public class Parry : BaseSeamstressSkillState
    {
        public static NetworkSoundEventDef parrySoundDef = SeamstressAssets.parrySuccessSoundEvent;

        private GameObject blinkPrefab; 

        public static string enterSoundString = "Play_bandit2_m2_impact";

        public static float duration = SeamstressStaticValues.parryWindow;

        public static float attackDelay = SeamstressStaticValues.parryWindow;

        public static float invulnDuration = SeamstressStaticValues.parryWindow * 1.25f;

        public override void OnEnter()
        {
            base.OnEnter();
            Util.PlaySound(enterSoundString, base.gameObject);
            if (NetworkServer.active)
            {
                CleanBuffsServer();
                base.characterBody.AddBuff(SeamstressBuffs.parryStart);
            }
            PlayAnimation("FullBody, Override", "Roll", "Roll.playbackRate", duration);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            bool num = base.characterBody.HasBuff(SeamstressBuffs.parrySuccess);
            if (base.isAuthority && base.fixedAge >= duration && num)
            {
                blinkPrefab = SeamstressAssets.blinkPrefab;
                DoAttackServer();
                outer.SetNextState(new ParryDash());
            }
            else if(base.isAuthority && base.fixedAge >= duration && !num)
            {
                skillLocator.utility.rechargeStopwatch += 0.5f * skillLocator.utility.cooldownRemaining;
                outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {
            CleanBuffsServer();
            base.OnExit();
        }

        private void CleanBuffsServer()
        {
            if (NetworkServer.active)
            {
                if (base.characterBody.HasBuff(SeamstressBuffs.parryStart))
                {
                    base.characterBody.RemoveBuff(SeamstressBuffs.parryStart);
                }
                if (base.characterBody.HasBuff(SeamstressBuffs.parrySuccess))
                {
                    base.characterBody.RemoveBuff(SeamstressBuffs.parrySuccess);
                }
            }
        }

        private void DoAttackServer()
        {
            if (!NetworkServer.active)
            {
                return;
            }
            characterBody.AddTimedBuff(SeamstressBuffs.butchered, SeamstressStaticValues.butcheredDuration, 1);
            SeamstressController s = characterBody.GetComponent<SeamstressController>();
            s.inButchered = false;
            CleanBuffsServer();
            if (parrySoundDef)
            {
                EffectManager.SimpleSoundEffect(parrySoundDef.index, base.characterBody.corePosition, transmit: true);
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Death;
        }
    }

}
