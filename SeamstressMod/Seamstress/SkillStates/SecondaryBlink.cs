using System.Linq;
using RoR2;
using RoR2.Navigation;
using UnityEngine;
using SeamstressMod.Modules.BaseStates;
using R2API;
using UnityEngine.Networking;
using EntityStates;
using static R2API.DamageAPI;
using SeamstressMod.Seamstress.Content;

namespace SeamstressMod.Seamstress.SkillStates
{
    public class SecondaryBlink : BaseSeamstressSkillState
    {
        public static bool disappearWhileBlinking = true;

        public CameraTargetParams.AimRequest aimRequest;

        private Vector3 blinkDestination = Vector3.zero;

        private Vector3 blinkStart = Vector3.zero;

        public float baseDuration;

        public float exitDuration;

        public float destinationAlertDuration;

        public float stockMultiplier;

        public static float blinkDistance = 30f;

        public static string beginSoundString = "Play_imp_overlord_teleport_start";

        public static string endSoundString = "Play_imp_overlord_teleport_end";

        //test sphere collider changing with scale
        public float blastAttackRadius = SeamstressAssets.blinkDestinationPrefab.transform.GetChild(1).gameObject.GetComponent<SphereCollider>().radius - 10;

        public static float blastAttackDamageCoefficient = SeamstressStaticValues.sewAltDamageCoefficient;

        public static float blastAttackForce = 50f;

        public static float blastAttackProcCoefficient = 1f;

        private Animator animator;

        private CharacterModel characterModel;

        private HurtBoxGroup hurtboxGroup;

        private Transform modelTransform;

        private GameObject blinkDestinationInstance;

        private bool isExiting = false;

        private bool hasBlinked = false;

        private float doThings;

        public override void OnEnter()
        {
            base.OnEnter();
            RefreshState();
            stockMultiplier = needleCount;
            if (NetworkServer.active)
            {
                for (int i = needleCount; i > 0; i--)
                {
                    Util.PlaySound("Play_bandit2_m2_alt_throw", gameObject);
                    characterBody.RemoveBuff(SeamstressBuffs.needles);
                }
            }
            if (stockMultiplier > 6)
            {
                baseDuration = 0.5f;
                doThings = 1f;
            }
            else
            {
                doThings = 0.5f * (stockMultiplier / 3f);
                baseDuration = stockMultiplier / 12f;
            }
            exitDuration = baseDuration;
            destinationAlertDuration = baseDuration;
            if (doThings < 0.1f)
            {
                doThings = 0.1f;
            }
            blastAttackRadius += doThings * 20;

            if (cameraTargetParams)
            {
                aimRequest = cameraTargetParams.RequestAimType(CameraTargetParams.AimType.Aura);
            }
            Util.PlaySound(beginSoundString, gameObject);
            modelTransform = GetModelTransform();
            if (modelTransform)
            {
                animator = modelTransform.GetComponent<Animator>();
                characterModel = modelTransform.GetComponent<CharacterModel>();
                hurtboxGroup = modelTransform.GetComponent<HurtBoxGroup>();
            }
            if (disappearWhileBlinking)
            {
                if (characterModel)
                {
                    characterModel.invisibilityCount++;
                }
                if (hurtboxGroup)
                {
                    HurtBoxGroup hurtBoxGroup = hurtboxGroup;
                    int hurtBoxesDeactivatorCounter = hurtBoxGroup.hurtBoxesDeactivatorCounter + 1;
                    hurtBoxGroup.hurtBoxesDeactivatorCounter = hurtBoxesDeactivatorCounter;
                }
                /*
                if (this.childLocator)
                {
                    this.childLocator.FindChild("DustCenter").gameObject.SetActive(false);
                }
                */
            }
            CalculateBlinkDestination();
            CreateBlinkEffect(Util.GetCorePosition(gameObject));
            PlayAnimation("FullBody, Override", "Roll", "Roll.playbackRate", (baseDuration + exitDuration) / attackSpeedStat);
        }

        private void CalculateBlinkDestination()
        {
            if (isAuthority)
            {
                RaycastHit hitInfo;
                Vector3 position = !inputBank.GetAimRaycast(blinkDistance, out hitInfo) ? Vector3.MoveTowards(inputBank.GetAimRay().GetPoint(blinkDistance), transform.position, 5f) : Vector3.MoveTowards(hitInfo.point, transform.position, 5f);
                position.y += 2.5f;
                blinkDestination = position;
                Vector3 vector = new Vector3(0f, 0.1f, 0f);
                blinkDestination -= characterBody.footPosition - transform.position + vector;
                blinkStart = transform.position;
                characterDirection.forward = position;
            }
        }
        private void CreateBlinkEffect(Vector3 origin)
        {
            if (SeamstressAssets.blinkPrefab)
            {
                EffectData effectData = new EffectData();
                effectData.rotation = Util.QuaternionSafeLookRotation(blinkDestination - blinkStart);
                effectData.origin = origin;
                effectData.scale = doThings;
                EffectManager.SpawnEffect(SeamstressAssets.blinkPrefab, effectData, transmit: false);
            }
        }

        private void SetPosition(Vector3 newPosition)
        {
            if (characterMotor)
            {
                characterMotor.Motor.SetPositionAndRotation(newPosition, Quaternion.identity);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (characterMotor)
            {
                characterMotor.velocity = Vector3.zero;
            }
            if (!hasBlinked)
            {
                SetPosition(Vector3.Lerp(blinkStart, blinkDestination, fixedAge / baseDuration));
            }
            if (fixedAge >= (baseDuration - destinationAlertDuration) / attackSpeedStat && !hasBlinked)
            {
                hasBlinked = true;
                if (SeamstressAssets.blinkDestinationPrefab)
                {
                    blinkDestinationInstance = Object.Instantiate(SeamstressAssets.blinkDestinationPrefab, blinkDestination, Quaternion.identity);
                    blinkDestinationInstance.GetComponent<ScaleParticleSystemDuration>().newDuration = destinationAlertDuration;
                }
                SetPosition(blinkDestination);
            }
            if (fixedAge >= baseDuration / attackSpeedStat)
            {
                ExitCleanup();
            }
            if (fixedAge >= (baseDuration + exitDuration) / attackSpeedStat && isAuthority)
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
            Util.PlaySound(endSoundString, gameObject);
            CreateBlinkEffect(Util.GetCorePosition(gameObject));
            modelTransform = GetModelTransform();
            if (blastAttackDamageCoefficient > 0f && isAuthority)
            {
                BlastAttack blastAttack = new BlastAttack();
                blastAttack.attacker = gameObject;
                blastAttack.inflictor = gameObject;
                blastAttack.teamIndex = TeamComponent.GetObjectTeam(gameObject);
                blastAttack.baseDamage = damageStat * blastAttackDamageCoefficient * stockMultiplier;
                blastAttack.baseForce = blastAttackForce;
                blastAttack.position = blinkDestination;
                blastAttack.procCoefficient = blastAttackProcCoefficient;
                blastAttack.radius = blastAttackRadius;
                blastAttack.damageType = DamageType.Stun1s;
                if (butchered)
                {
                    blastAttack.AddModdedDamageType(DamageTypes.InsatiableLifeSteal);
                    blastAttack.AddModdedDamageType(DamageTypes.CutDamage);
                }
                else
                {
                    blastAttack.RemoveModdedDamageType(DamageTypes.InsatiableLifeSteal);
                    blastAttack.RemoveModdedDamageType(DamageTypes.CutDamage);
                }
                blastAttack.falloffModel = BlastAttack.FalloffModel.Linear;
                blastAttack.attackerFiltering = AttackerFiltering.NeverHitSelf;
                blastAttack.Fire();
            }
            if (disappearWhileBlinking)
            {
                if (modelTransform && SeamstressAssets.destealthMaterial)
                {
                    TemporaryOverlay temporaryOverlay = animator.gameObject.AddComponent<TemporaryOverlay>();
                    temporaryOverlay.duration = 1f;
                    temporaryOverlay.destroyComponentOnEnd = true;
                    temporaryOverlay.originalMaterial = SeamstressAssets.destealthMaterial;
                    temporaryOverlay.inspectorCharacterModel = animator.gameObject.GetComponent<CharacterModel>();
                    temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                    temporaryOverlay.animateShaderAlpha = true;
                }
                if (characterModel)
                {
                    characterModel.invisibilityCount--;
                }
                if (hurtboxGroup)
                {
                    HurtBoxGroup hurtBoxGroup = hurtboxGroup;
                    int hurtBoxesDeactivatorCounter = hurtBoxGroup.hurtBoxesDeactivatorCounter - 1;
                    hurtBoxGroup.hurtBoxesDeactivatorCounter = hurtBoxesDeactivatorCounter;
                }
                /*
                if (this.childLocator)
                {
                    childLocator.FindChild("DustCenter").gameObject.SetActive(
                false);
                }
                */
                //PlayAnimation("Gesture, Additive", "BlinkEnd", "BlinkEnd.playbackRate", exitDuration);
            }
            if (blinkDestinationInstance)
            {
                Object.Destroy(blinkDestinationInstance);
            }
        }

        public override void OnExit()
        {
            if (cameraTargetParams)
            {
                aimRequest.Dispose();
            }
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