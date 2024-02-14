

// Parry, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// Parry.ParryStrike
using EntityStates;
using R2API;
using RoR2;
using SeamstressMod.Survivors.Seamstress;
using UnityEngine;
using UnityEngine.Networking;

namespace SeamstressMod.SkillStates
{
    public class Parry : BaseState
    {
        public static GameObject _expungeEffect = SeamstressAssets.expungeEffect;
        public static GameObject _expungeEffect2 = SeamstressAssets.expungeSlashEffect;
        public static GameObject _expungeEffect3 = SeamstressAssets.expungeSlashEffect2;
        public static GameObject _expungeEffect4 = SeamstressAssets.expungeSlashEffect3;

        public static string enterSoundString = "";

        public static NetworkSoundEventDef parrySoundDef = SeamstressAssets.scissorsHitSoundEvent;

        public static float duration = SeamstressStaticValues.parryDuration;

        public static float attackDelay = 0.3f;

        public static float invulnDuration = SeamstressStaticValues.parryDuration;

        public static float blastAttackDamageCoefficient = SeamstressStaticValues.parryDamage;

        private bool hasFiredServer;

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
            if (NetworkServer.active && !hasFiredServer && base.fixedAge >= attackDelay)
            {
                DoAttackServer();
            }
            if (base.isAuthority && base.fixedAge >= duration)
            {
                outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {
            if (NetworkServer.active)
            {
                if (!hasFiredServer)
                {
                    DoAttackServer();
                }
                CleanBuffsServer();
            }
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
            bool num = base.characterBody.HasBuff(SeamstressBuffs.parrySuccess);
            if (!NetworkServer.active || !num)
            {
                return;
            }
            characterBody.AddTimedBuff(SeamstressBuffs.butchered, SeamstressStaticValues.butcheredDuration, 1);
            PlayCrossfade("Gesture, Override", "Slash", "Slash.playbackRate", duration, 0.05f);
            hasFiredServer = true;
            if (num)
            {
                if (parrySoundDef)
                {
                    EffectManager.SimpleSoundEffect(parrySoundDef.index, base.characterBody.corePosition, transmit: true);
                }
            }
            UnityEngine.Object.Instantiate<GameObject>(_expungeEffect, transform);
            UnityEngine.Object.Instantiate<GameObject>(_expungeEffect2, transform);
            UnityEngine.Object.Instantiate<GameObject>(_expungeEffect3, transform);
            UnityEngine.Object.Instantiate<GameObject>(_expungeEffect4, transform);
            BlastAttack blastAttack = new BlastAttack();
            blastAttack.attacker = base.gameObject;
            blastAttack.inflictor = base.gameObject;
            blastAttack.teamIndex = TeamComponent.GetObjectTeam(base.gameObject);
            blastAttack.baseDamage = damageStat * blastAttackDamageCoefficient;
            blastAttack.baseForce = 200f;
            blastAttack.position = base.characterBody.corePosition;
            blastAttack.radius = base.characterBody.radius + 12f;
            blastAttack.falloffModel = BlastAttack.FalloffModel.None;
            blastAttack.damageType = DamageType.Stun1s;
            blastAttack.AddModdedDamageType(DamageTypes.StitchDamage);
            blastAttack.attackerFiltering = AttackerFiltering.NeverHitSelf;
            blastAttack.Fire();
            CleanBuffsServer();
        }
    }

}
