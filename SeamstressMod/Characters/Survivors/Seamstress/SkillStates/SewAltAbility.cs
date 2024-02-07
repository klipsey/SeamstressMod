using System.Linq;
using RoR2;
using RoR2.Navigation;
using UnityEngine;
using SeamstressMod.Modules.BaseStates;
using SeamstressMod.Survivors.Seamstress;
using R2API;
using UnityEngine.Networking;
using EntityStates;

namespace SeamstressMod.SkillStates
{
    public class SewAltAbility : BaseSeamstressSkillState
    {
        public static bool disappearWhileBlinking = true;

        public CameraTargetParams.AimRequest aimRequest;

        private Vector3 blinkDestination = Vector3.zero;

        private Vector3 blinkStart = Vector3.zero;

        public float baseDuration;

        public float exitDuration;

        public float destinationAlertDuration;

        public static float blinkDistance = 30f;

        public static string beginSoundString = "Play_imp_overlord_teleport_start";

        public static string endSoundString = "Play_imp_overlord_teleport_end";

        //test sphere collider changing with scale
        public float blastAttackRadius = SeamstressAssets.blinkDestinationPrefab.transform.GetChild(1).gameObject.GetComponent<SphereCollider>().radius;

        public static float blastAttackDamageCoefficient = SeamstressStaticValues.sewAltDamageCoefficient;

        public static float blastAttackForce = 50f;

        public static float blastAttackProcCoefficient = 1f;

        private Animator animator;

        private CharacterModel characterModel;

        private HurtBoxGroup hurtboxGroup;

        private ChildLocator childLocator;

        private Transform modelTransform;

        private GameObject blinkDestinationInstance;

        private bool isExiting = false;

        private bool hasBlinked = false;

        private float doThings;

        public override void OnEnter()
        {
            if(base.skillLocator.special.stock > 10)
            {
                this.baseDuration = 1f;
            }
            else
            {
                this.baseDuration = base.skillLocator.special.stock / 10;
            }
            this.exitDuration = (this.baseDuration / 2) / this.attackSpeedStat;
            this.destinationAlertDuration = (baseDuration / 2) / this.attackSpeedStat;
            this.doThings = 0.5f * (base.skillLocator.special.stock / base.skillLocator.special.maxStock);
            if(this.doThings < 0.1f)
            {
                this.doThings = 0.1f;
            }
            this.blastAttackRadius *= this.doThings;
            base.OnEnter();
            RefreshState();
            if ((bool)base.cameraTargetParams)
            {
                aimRequest = base.cameraTargetParams.RequestAimType(CameraTargetParams.AimType.Aura);
            }
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
            PlayAnimation("FullBody, Override", "Roll", "Roll.playbackRate", baseDuration / this.attackSpeedStat + exitDuration);
        }

        private void CalculateBlinkDestination()
        {
            if (base.isAuthority)
            {
                RaycastHit hitInfo;
                Vector3 position = (!base.inputBank.GetAimRaycast(blinkDistance, out hitInfo) ? Vector3.MoveTowards(base.inputBank.GetAimRay().GetPoint(blinkDistance), base.transform.position, 5f) : Vector3.MoveTowards(hitInfo.point, base.transform.position, 5f));
                position.y += 2.5f;
                blinkDestination = position;
                Vector3 vector = new Vector3(0f, 0.1f, 0f);
                blinkDestination -= (base.characterBody.footPosition - base.transform.position + vector);
                blinkStart = base.transform.position;
                base.characterDirection.forward = position;
            }
        }
        private void CreateBlinkEffect(Vector3 origin)
        {
            if ((bool)SeamstressAssets.blinkPrefab)
            {
                EffectData effectData = new EffectData();
                effectData.rotation = Util.QuaternionSafeLookRotation(blinkDestination - blinkStart);
                effectData.origin = origin;
                effectData.scale = doThings;
                EffectManager.SpawnEffect(SeamstressAssets.blinkPrefab, effectData, transmit: true);
            }
        }

        private void SetPosition(Vector3 newPosition)
        {
            if ((bool)base.characterMotor)
            {
                base.characterMotor.Motor.SetPositionAndRotation(newPosition, Quaternion.identity);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if ((bool)base.characterMotor)
            {
                base.characterMotor.velocity = Vector3.zero;
            }
            if (!hasBlinked)
            {
                SetPosition(Vector3.Lerp(blinkStart, blinkDestination, base.fixedAge / baseDuration));
            }
            if (base.fixedAge >= baseDuration / this.attackSpeedStat - destinationAlertDuration && !hasBlinked)
            {
                hasBlinked = true;
                if ((bool)SeamstressAssets.blinkDestinationPrefab)
                {
                    blinkDestinationInstance = Object.Instantiate(SeamstressAssets.blinkDestinationPrefab, blinkDestination, Quaternion.identity);
                    blinkDestinationInstance.GetComponent<ScaleParticleSystemDuration>().newDuration = destinationAlertDuration;
                }
                SetPosition(blinkDestination);
            }
            if (base.fixedAge >= baseDuration / this.attackSpeedStat)
            {
                ExitCleanup();
            }
            if (base.fixedAge >= baseDuration / this.attackSpeedStat + exitDuration && base.isAuthority)
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
            Util.PlaySound(endSoundString, base.gameObject);
            CreateBlinkEffect(Util.GetCorePosition(base.gameObject));
            modelTransform = GetModelTransform();
            if (blastAttackDamageCoefficient > 0f && base.isAuthority)
            {
                BlastAttack blastAttack = new BlastAttack();
                blastAttack.attacker = base.gameObject;
                blastAttack.inflictor = base.gameObject;
                blastAttack.teamIndex = TeamComponent.GetObjectTeam(base.gameObject);
                blastAttack.baseDamage = damageStat * blastAttackDamageCoefficient * skillLocator.special.stock;
                blastAttack.baseForce = blastAttackForce;
                blastAttack.position = blinkDestination;
                blastAttack.procCoefficient = blastAttackProcCoefficient;
                blastAttack.radius = blastAttackRadius;
                blastAttack.damageType = DamageType.Stun1s;
                blastAttack.AddModdedDamageType(DamageTypes.StitchDamage);
                if(empowered)
                {
                    blastAttack.AddModdedDamageType(DamageTypes.CutDamage);
                }
                else
                {
                    blastAttack.RemoveModdedDamageType(DamageTypes.CutDamage);
                }
                blastAttack.falloffModel = BlastAttack.FalloffModel.Linear;
                blastAttack.attackerFiltering = AttackerFiltering.NeverHitSelf;
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
                    characterModel.invisibilityCount--;
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
            aimRequest?.Dispose();
            base.OnExit();
            ExitCleanup();
        }
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.Write(blinkDestination);
            writer.Write(blinkStart);
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            blinkDestination = reader.ReadVector3();
            blinkStart = reader.ReadVector3();
        }
    }
}