using System.Linq;
using RoR2;
using RoR2.Navigation;
using UnityEngine;
using SeamstressMod.Modules.BaseStates;
using SeamstressMod.Survivors.Seamstress;
using R2API;

namespace SeamstressMod.SkillStates
{
    public class BlinkSeamstress : BaseSeamstressSkillState
    {
        private Transform modelTransform;

        public static bool disappearWhileBlinking = true;

        private Vector3 blinkDestination;

        private Vector3 blinkStart;

        public static GameObject blinkPrefab = SeamstressAssets.blinkPrefab;

        public static GameObject blinkDestinationPrefab = SeamstressAssets.blinkDestinationPrefab;

        public static float duration = 0.3f;

        public static float exitDuration = 0.15f;

        public static float destinationAlertDuration = 0.15f;

        public static float blinkDistance = 25f;

        public string beginSoundString = "Play_imp_overlord_teleport_start";

        public string endSoundString = "Play_imp_overlord_teleport_end";

        //test sphere collider changing with scale
        public static float blastAttackRadius = 12.5f;

        public static float empoweredBlastAttackRadius = 25f;

        public static float blastAttackDamageCoefficient = SeamstressStaticValues.blinkDamageCoefficient;

        public static float blastAttackForce = 50f;

        public static float blastAttackProcCoefficient = 1f;

        private Animator animator;

        private CharacterModel characterModel;

        private HurtBoxGroup hurtboxGroup;

        private ChildLocator childLocator;

        private GameObject blinkDestinationInstance;

        private bool isExiting = false;

        private bool hasBlinked = false;

        public override void OnEnter()
        {
            base.OnEnter();
            blinkDestination = Vector3.zero;
            blinkStart = Vector3.zero;
            Util.PlaySound(beginSoundString, base.gameObject);
            this.modelTransform = GetModelTransform();
            if ((bool)this.modelTransform)
            {
                this.animator = this.modelTransform.GetComponent<Animator>();
                this.characterModel = this.modelTransform.GetComponent<CharacterModel>();
                this.hurtboxGroup = this.modelTransform.GetComponent<HurtBoxGroup>();
                this.childLocator = this.modelTransform.GetComponent<ChildLocator>();
            }
            if (disappearWhileBlinking)
            {
                if ((bool)this.characterModel)
                {
                    this.characterModel.invisibilityCount++;
                }
                if ((bool)this.hurtboxGroup)
                {
                    HurtBoxGroup hurtBoxGroup = hurtboxGroup;
                    int hurtBoxesDeactivatorCounter = hurtBoxGroup.hurtBoxesDeactivatorCounter + 1;
                    hurtBoxGroup.hurtBoxesDeactivatorCounter = hurtBoxesDeactivatorCounter;
                }
                /*
                if ((bool)this.childLocator)
                {
                    this.childLocator.FindChild("DustCenter").gameObject.SetActive(value: false);
                }
                */
            }
            CalculateBlinkDestination();
            CreateBlinkEffect(Util.GetCorePosition(base.gameObject));
        }

        private void CalculateBlinkDestination()
        {
            RaycastHit hitInfo;
            Vector3 position = Vector3.zero;
            if (base.isAuthority)
            {
                position = ((!this.inputBank.GetAimRaycast(blinkDistance, out hitInfo)) ? this.inputBank.GetAimRay().GetPoint(blinkDistance) : hitInfo.point);
            }
            blinkDestination = position;
            blinkDestination += base.transform.position - base.characterBody.footPosition;
            blinkStart = base.transform.position;
            base.characterMotor.velocity = Vector3.zero;
            base.characterMotor.rootMotion += position;
        }
        private void CreateBlinkEffect(Vector3 origin)
        {
            if ((bool)blinkPrefab)
            {
                EffectData effectData = new EffectData();
                effectData.rotation = Util.QuaternionSafeLookRotation(blinkDestination - blinkStart);
                effectData.origin = origin;
                EffectManager.SpawnEffect(blinkPrefab, effectData, transmit: false);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if ((bool)base.characterMotor)
            {
                base.characterMotor.velocity = Vector3.zero;
            }
            if (base.fixedAge >= duration - destinationAlertDuration && !hasBlinked)
            {
                hasBlinked = true;
                if ((bool)blinkDestinationPrefab)
                {
                    blinkDestinationInstance = Object.Instantiate(blinkDestinationPrefab, blinkDestination, Quaternion.identity);
                    blinkDestinationInstance.GetComponent<ScaleParticleSystemDuration>().newDuration = destinationAlertDuration;
                }
            }
            if (base.fixedAge >= duration)
            {
                ExitCleanup();
            }
            if (base.fixedAge >= duration + exitDuration && base.isAuthority)
            {
                outer.SetNextStateToMain();
            }
        }

        private void ExitCleanup()
        {
            if (isExiting)
            {
                return;
            }
            isExiting = true;
            Util.PlaySound(endSoundString, base.gameObject);
            CreateBlinkEffect(Util.GetCorePosition(base.gameObject));
            modelTransform = GetModelTransform();
            if (blastAttackDamageCoefficient > 0f)
            {
                BlastAttack blastAttack = new BlastAttack();
                blastAttack.attacker = base.gameObject;
                blastAttack.inflictor = base.gameObject;
                blastAttack.teamIndex = TeamComponent.GetObjectTeam(base.gameObject);
                blastAttack.baseDamage = damageStat * blastAttackDamageCoefficient;
                blastAttack.baseForce = blastAttackForce;
                blastAttack.position = blinkDestination;
                if (empowered)
                {
                    blastAttack.radius = empoweredBlastAttackRadius;
                    blastAttack.damageType = DamageType.Stun1s;
                    blastAttack.AddModdedDamageType(DamageTypes.Weaken);
                }
                else
                {
                    blastAttack.radius = blastAttackRadius;
                    blastAttack.RemoveModdedDamageType(DamageTypes.Weaken);
                }
                blastAttack.falloffModel = BlastAttack.FalloffModel.Linear;
                blastAttack.attackerFiltering = AttackerFiltering.NeverHitSelf;
                blastAttack.AddModdedDamageType(DamageTypes.CutDamage);
                blastAttack.Fire();
            }
            if (disappearWhileBlinking)
            {
                if ((bool)this.modelTransform && (bool)SeamstressAssets.destealthMaterial)
                {
                    TemporaryOverlay temporaryOverlay = animator.gameObject.AddComponent<TemporaryOverlay>();
                    temporaryOverlay.duration = 1f;
                    temporaryOverlay.destroyComponentOnEnd = true;
                    temporaryOverlay.originalMaterial = SeamstressAssets.destealthMaterial;
                    temporaryOverlay.inspectorCharacterModel = animator.gameObject.GetComponent<CharacterModel>();
                    temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                    temporaryOverlay.animateShaderAlpha = true;
                }
                if ((bool)this.characterModel)
                {
                    this.characterModel.invisibilityCount--;
                }
                if ((bool)this.hurtboxGroup)
                {
                    HurtBoxGroup hurtBoxGroup = hurtboxGroup;
                    int hurtBoxesDeactivatorCounter = hurtBoxGroup.hurtBoxesDeactivatorCounter - 1;
                    hurtBoxGroup.hurtBoxesDeactivatorCounter = hurtBoxesDeactivatorCounter;
                }
                /*
                if ((bool)this.childLocator)
                {
                    childLocator.FindChild("DustCenter").gameObject.SetActive(value: true);
                }
                */
                //PlayAnimation("Gesture, Additive", "BlinkEnd", "BlinkEnd.playbackRate", exitDuration);
            }
            if ((bool)blinkDestinationInstance)
            {
                UnityEngine.Object.Destroy(blinkDestinationInstance);
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            ExitCleanup();
        }
    }
}