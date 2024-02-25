/*
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using R2API;
using SeamstressMod.Survivors.Seamstress;
using SeamstressMod.Modules.BaseStates;
using UnityEngine.Networking;

namespace SeamstressMod.SkillStates
{
    public class ReapRecast : BaseSeamstressSkillState
    {
        public static GameObject _expungeEffect = SeamstressAssets.expungeEffect;
        public static GameObject _expungeEffect2 = SeamstressAssets.expungeSlashEffect;
        public static GameObject _expungeEffect3 = SeamstressAssets.expungeSlashEffect2;
        public static GameObject _expungeEffect4 = SeamstressAssets.expungeSlashEffect3;

        public static NetworkSoundEventDef expungeSoundDef = SeamstressAssets.parrySuccessSoundEvent;

        public static float duration = 0.5f;

        private bool hasFiredServer;
        public override void OnEnter()
        {
            RefreshState();
            base.OnEnter();
            if (base.isAuthority) 
            {
                Util.CleanseBody(this.characterBody, true, false, false, true, true, true);
            }
            this.skillLocator.special = skillLocator.FindSkill("Special");
            if (!isGrounded)
            {
                SmallHop(characterMotor, 5f);
            }
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (NetworkServer.active && !hasFiredServer && base.fixedAge >= duration)
            {
                FireAttack();
            }
            if (base.isAuthority && base.fixedAge >= duration)
            {
                outer.SetNextStateToMain();
            }
        }
        private void FireAttack()
        {
            hasFiredServer = true;
            if (expungeSoundDef)
            {
                EffectManager.SimpleSoundEffect(expungeSoundDef.index, base.characterBody.corePosition, transmit: true);
            }
            PlayCrossfade("Gesture, Override", "ThrowBomb", "ThrowBomb.playbackRate", duration, 0.1f * duration);
            UnityEngine.Object.Instantiate<GameObject>(_expungeEffect, transform);
            UnityEngine.Object.Instantiate<GameObject>(_expungeEffect2, transform);
            UnityEngine.Object.Instantiate<GameObject>(_expungeEffect3, transform);
            UnityEngine.Object.Instantiate<GameObject>(_expungeEffect4, transform);
            BlastAttack blastAttack = new BlastAttack();
            blastAttack.attacker = base.gameObject;
            blastAttack.inflictor = base.gameObject;
            blastAttack.teamIndex = TeamComponent.GetObjectTeam(base.gameObject);
            blastAttack.baseDamage = seamCon.GetButcheredConversion();
            blastAttack.baseForce = 200f;
            blastAttack.position = base.characterBody.corePosition;
            blastAttack.radius = base.characterBody.radius + 12f;
            blastAttack.falloffModel = BlastAttack.FalloffModel.None;
            blastAttack.damageType = DamageType.Stun1s;
            blastAttack.AddModdedDamageType(DamageTypes.NoSword);
            blastAttack.AddModdedDamageType(DamageTypes.CutDamage);
            blastAttack.attackerFiltering = AttackerFiltering.NeverHitSelf;
            blastAttack.Fire();
        }
        public override void OnExit()
        {
            if (NetworkServer.active)
            {
                if (!hasFiredServer)
                {
                    FireAttack();
                }
            }
            base.OnExit();
        }
    }
}
*/