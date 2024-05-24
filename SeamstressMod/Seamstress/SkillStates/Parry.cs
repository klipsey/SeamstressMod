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

        private GameObject blinkPrefab;

        public static string enterSoundString = "Play_bandit2_m2_impact";

        public static float duration = SeamstressStaticValues.parryWindow;

        public static float attackDelay = SeamstressStaticValues.parryWindow;

        public static float invulnDuration = SeamstressStaticValues.parryWindow * 1.25f;

        public override void OnEnter()
        {
            base.OnEnter();
            Util.PlaySound(enterSoundString, gameObject);
            if (NetworkServer.active)
            {
                CleanBuffsServer();
                characterBody.AddBuff(SeamstressBuffs.parryStart);
            }
            PlayAnimation("FullBody, Override", "Parry", "Slash.playbackRate", duration * 1.5f);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            bool success = characterBody.HasBuff(SeamstressBuffs.parrySuccess);
            if (base.isAuthority && base.fixedAge >= duration && success)
            {
                blinkPrefab = SeamstressAssets.blinkEffect;
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
                if (characterBody.HasBuff(SeamstressBuffs.parryStart))
                {
                    characterBody.RemoveBuff(SeamstressBuffs.parryStart);
                }
                if (characterBody.HasBuff(SeamstressBuffs.parrySuccess))
                {
                    characterBody.RemoveBuff(SeamstressBuffs.parrySuccess);
                }
            }
        }

        private void DoAttackServer()
        {
            if (!NetworkServer.active)
            {
                return;
            }
            characterBody.AddTimedBuff(SeamstressBuffs.instatiable, SeamstressStaticValues.insatiableDuration / 1.5f, 1);
            CleanBuffsServer();
            if (parrySoundDef)
            {
                EffectManager.SimpleSoundEffect(parrySoundDef.index, characterBody.corePosition, transmit: false);
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Death;
        }
    }

}
