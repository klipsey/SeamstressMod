using RoR2;
using SeamstressMod.Survivors.Seamstress;
using SeamstressMod.SkillStates.BaseStates;
using UnityEngine;
using UnityEngine.Networking;
using EntityStates;

namespace SeamstressMod.SkillStates
{
    public class Reap : BaseSeamstressSkillState
    {
        public GameObject reapPrefab;

        public static float baseDuration = 1f;

        public static float duration;

        public static float firePercentTime = 0f;

        private float fireTime;

        private bool hasFired;

        public static float healthCostFraction = 0.5f;
        public override void OnEnter()
        {
            base.OnEnter();
            rechargeStocks();//quality of life
            reapPrefab = SeamstressAssets.reapBleedEffect;
            duration = baseDuration / attackSpeedStat;
            fireTime = firePercentTime * duration;
            Util.PlaySound("Play_item_proc_novaonheal_impact", gameObject);
            PlayAnimation("Gesture, Override", "ThrowBomb", "ThrowBomb.playbackRate", duration);
            CharacterModel component = (GetModelTransform()).GetComponent<CharacterModel>();
            TemporaryOverlay temporaryOverlay = base.gameObject.AddComponent<TemporaryOverlay>();
            temporaryOverlay.originalMaterial = LegacyResourcesAPI.Load<Material>("Materials/matFullCrit");
            temporaryOverlay.duration = SeamstressStaticValues.butcheredDuration;
            temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
            temporaryOverlay.AddToCharacerModel(component);
            Util.PlaySound("Play_bandit2_m2_alt_throw", base.characterBody.gameObject);
            UnityEngine.Object.Instantiate<GameObject>(reapPrefab, base.characterBody.modelLocator.transform);
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (fixedAge >= fireTime)
            {
                Fire();
            }
            if (fixedAge >= duration && isAuthority)
            {
                outer.SetNextStateToMain();
                return;
            }
        }

        public void rechargeStocks()
        {
            if (characterBody.skillLocator.secondary.stock < characterBody.skillLocator.secondary.maxStock)
            {
                base.characterBody.skillLocator.secondary.AddOneStock();
            }
            if (characterBody.skillLocator.special.stock < characterBody.skillLocator.special.maxStock)
            {
                base.characterBody.skillLocator.special.AddOneStock();
            }
        }
        private void Fire()
        {
            if(!hasFired)
            {
                hasFired = true;
                if (NetworkServer.active && (bool)healthComponent && healthCostFraction >= Mathf.Epsilon)
                {
                    float currentBarrier = healthComponent.barrier;
                    DamageInfo damageInfo = new DamageInfo();
                    damageInfo.damage = ((healthComponent.health + healthComponent.shield) * healthCostFraction) + healthComponent.barrier;
                    damageInfo.position = characterBody.corePosition;
                    damageInfo.force = Vector3.zero;
                    damageInfo.damageColorIndex = DamageColorIndex.Default;
                    damageInfo.crit = false;
                    damageInfo.attacker = null;
                    damageInfo.inflictor = null;
                    damageInfo.damageType = DamageType.NonLethal | DamageType.BypassArmor | DamageType.BypassBlock;
                    damageInfo.procCoefficient = 0f;
                    healthComponent.TakeDamage(damageInfo);
                    healthComponent.AddBarrier(currentBarrier);
                    if(characterBody.HasBuff(SeamstressBuffs.butchered))
                    {
                        characterBody.RemoveBuff(SeamstressBuffs.butchered);
                    }
                    characterBody.AddTimedBuff(SeamstressBuffs.butchered, SeamstressStaticValues.butcheredDuration, 1);
                    characterBody.AddTimedBuff(RoR2Content.Buffs.HiddenInvincibility, 0.25f);
                    if (base.characterBody.GetBuffCount(SeamstressBuffs.needles) < SeamstressStaticValues.maxNeedleAmount)
                    {
                        base.characterBody.AddBuff(SeamstressBuffs.needles);
                    }
                }
            }
        }
        public override void OnExit()
        {
            base.OnExit();
        }
    }
}
