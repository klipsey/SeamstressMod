using EntityStates;
using EntityStates.SurvivorPod;
using KinematicCharacterController;
using RoR2;
using SeamstressMod.Survivors.Seamstress;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;

namespace SeamstressMod.SkillStates
{
    public class Telekinesis : BaseSkillState
    {
        private float _maxGrabDistance = 40f;

        private float _minGrabDistance = 1f;

        private Transform _barrelPoint;

        private Rigidbody _grabbedObject;

        private Rigidbody _previousObject;

        private float _pickDistance;

        private Vector3 _pickOffset;

        private Vector3 _pickTargetPosition;

        private Vector3 _pickForce;

        private HurtBox victim;

        private Tracker tracker;

        private bool modelLocatorStartedDisabled;

        private Transform modelTransform;

        private Quaternion originalRotation;

        public override void OnEnter()
        {
            base.OnEnter();
            tracker = GetComponent<Tracker>();
            if (tracker)
            {
                if (base.isAuthority) victim = tracker.GetTrackingTarget();
                _grabbedObject = victim.healthComponent.GetComponent<Rigidbody>();
                _previousObject = _grabbedObject;
                victim.healthComponent.gameObject.layer = LayerIndex.fakeActor.intVal;
                _grabbedObject.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
                CharacterMotor component = victim.healthComponent.body.gameObject.GetComponent<CharacterMotor>();
                if (component)
                {
                    component.Motor.ForceUnground();
                    SmallHop(component, 2f);
                    component.disableAirControlUntilCollision = true;
                    component.velocity = Vector3.zero;
                    component.rootMotion = Vector3.zero;
                }
                CharacterDirection component2 = victim.healthComponent.body.gameObject.GetComponent<CharacterDirection>();
                if (component2)
                {
                    component2.enabled = false;
                }
                ModelLocator modelLocator = victim.healthComponent.body.gameObject.GetComponent<ModelLocator>();
                if (modelLocator)
                {
                    if (!modelLocator.enabled)
                    {
                        modelLocatorStartedDisabled = true;
                    }
                    if (modelLocator.modelTransform)
                    {
                        modelTransform = modelLocator.modelTransform;
                        originalRotation = modelTransform.rotation;
                    }
                }
                _grabbedObject.useGravity = false;
                _grabbedObject.freezeRotation = true;
                _grabbedObject.isKinematic = false;
            }

            if (!_barrelPoint)
            {
                _barrelPoint = transform;
            }
        }

        public override void OnExit()
        {
            skillLocator.secondary.DeductStock(1);
            base.OnExit();
        }

        public override void Update()
        {
            base.Update();
            if (inputBank.skill2.down)
            {
                Grab();
            }
            if (!inputBank.skill2.down || !victim.healthComponent.alive)
            {
                if (_grabbedObject)
                {
                    Release();
                }
                else if (base.isAuthority)
                {
                    outer.SetNextStateToMain();
                }
            }

            _pickDistance = Mathf.Clamp(_pickDistance + Input.mouseScrollDelta.y * 10f, _minGrabDistance, _maxGrabDistance);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            StartAimMode();
            if (_grabbedObject != null)
            {
                var ray = base.inputBank.GetAimRay();
                _pickTargetPosition = ray.origin + ray.direction * _pickDistance;// + _pickOffset;
                var forceDir = _pickTargetPosition - GetTargetRootPosition();
                _pickForce = forceDir / Time.fixedDeltaTime * 0.3f / Mathf.Clamp(_grabbedObject.mass, 60f, 120f);
                if (Util.HasEffectiveAuthority(victim.healthComponent.body.gameObject))
                {
                    CharacterMotor component = victim.healthComponent.body.gameObject.GetComponent<CharacterMotor>();
                    if (component)
                    {
                        component.rootMotion += _pickForce;
                        return;
                    }
                    Rigidbody component2 = victim.healthComponent.body.gameObject.GetComponent<Rigidbody>();
                    if ((bool)component2)
                    {
                        component2.velocity += _pickForce;
                    }
                }
            }
        }
        private Vector3 GetTargetRootPosition()
        {
            if (victim)
            {
                Vector3 result = victim.gameObject.transform.position;
                if ((bool)victim.healthComponent)
                {
                    result = victim.healthComponent.body.corePosition;
                }
                return result;
            }
            return base.transform.position;
        }
        private void Grab()
        {
            //_pickOffset = victim.healthComponent.GetComponent<Rigidbody>().worldCenterOfMass - victim.transform.position;
            _pickDistance = (victim.transform.position - _barrelPoint.transform.position).magnitude;
        }

        private void Release()
        {
            _grabbedObject.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            CharacterDirection component2 = victim.healthComponent.body.gameObject.GetComponent<CharacterDirection>();
            if (component2)
            {
                component2.enabled = true;
            }
            ModelLocator modelLocator = victim.healthComponent.body.gameObject.GetComponent<ModelLocator>();
            if (modelLocator && !modelLocatorStartedDisabled)
            {
                modelLocator.enabled = true;
            }
            if (this.modelTransform)
            {
                this.modelTransform.rotation = this.originalRotation;
            }
            _grabbedObject.useGravity = _previousObject.useGravity;
            _grabbedObject.freezeRotation = _previousObject.freezeRotation;
            _grabbedObject.isKinematic = _previousObject.isKinematic;
            _grabbedObject.gameObject.layer = _previousObject.gameObject.layer;
            _grabbedObject = null;
            _previousObject = null;
            if (base.isAuthority) outer.SetNextStateToMain();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}