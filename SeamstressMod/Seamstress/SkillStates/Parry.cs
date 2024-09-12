using EntityStates;
using R2API;
using RoR2;
using SeamstressMod.Modules.BaseStates;
using SeamstressMod.Seamstress.Components;
using SeamstressMod.Seamstress.Content;
using UnityEngine;
using UnityEngine.Networking;

namespace SeamstressMod.Seamstress.SkillStates
{
    public class Parry : BaseSeamstressSkillState
    {
        public static NetworkSoundEventDef parrySoundDef = SeamstressAssets.parrySuccessSoundEvent;

        public static string enterSoundString = "Play_bandit2_m2_impact";

        public static float duration = SeamstressStaticValues.parryWindow;

        public static float attackDelay = SeamstressStaticValues.parryWindow;

        public static float invulnDuration = SeamstressStaticValues.parryWindow * 1.25f;

        public override void OnEnter()
        {
            RefreshState();
            base.OnEnter();
            Util.PlaySound(enterSoundString, gameObject);
            if (NetworkServer.active)
            {
                CleanBuffsServer();
                characterBody.AddBuff(SeamstressBuffs.ParryStart);
            }
            PlayCrossfade("FullBody, Override", "Parry", "Slash.playbackRate", duration * 1.5f, duration * 0.05f);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            bool success = characterBody.HasBuff(SeamstressBuffs.ParrySuccess);
            if (base.isAuthority && base.fixedAge >= duration && success)
            {
                DoAttackServer();
                outer.SetNextState(new ParryDash());
            }
            else if (base.isAuthority && base.fixedAge >= duration && !success)
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
                if (characterBody.HasBuff(SeamstressBuffs.ParryStart))
                {
                    characterBody.RemoveBuff(SeamstressBuffs.ParryStart);
                }
                if (characterBody.HasBuff(SeamstressBuffs.ParrySuccess))
                {
                    characterBody.RemoveBuff(SeamstressBuffs.ParrySuccess);
                }
            }
        }

        private void DoAttackServer()
        {
            if (!NetworkServer.active)
            {
                return;
            }
            characterBody.AddTimedBuff(SeamstressBuffs.SeamstressInsatiableBuff, SeamstressStaticValues.insatiableDuration / 1.5f, 1);
            CleanBuffsServer();
            if (parrySoundDef)
            {
                EffectManager.SimpleSoundEffect(parrySoundDef.index, characterBody.corePosition, transmit: true);
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Death;
        }
    }

}
