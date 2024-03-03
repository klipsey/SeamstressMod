using EntityStates;
using R2API;
using RoR2;
using SeamstressMod.Modules.BaseStates;
using SeamstressMod.Survivors.Seamstress;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements.UIR;


namespace SeamstressMod.SkillStates
{
    public class Telekinesis : BaseSeamstressSkillState
    {
        private float _maxGrabDistance = 40f;

        private float _minGrabDistance = 1f;

        private Transform _barrelPoint;

        private Rigidbody _grabbedObject;

        private float _pickDistance;

        private Vector3 _pickOffset;

        private Vector3 _pickTargetPosition;

        private Vector3 _pickForce;

        private HurtBox victim;

        private Tracker tracker;

        private bool modelLocatorStartedDisabled;

        private Transform modelTransform;

        private Quaternion originalRotation;

        private bool bodyHadGravity = true;

        private bool bodyWasKinematic = true;

        private CollisionDetectionMode collisionDetectionMode;

        private CharacterGravityParameters gravParams;

        public override void OnEnter()
        {
            base.OnEnter();
            tracker = GetComponent<Tracker>();
            if (tracker)
            {
                if (base.isAuthority) victim = tracker.GetTrackingTarget();
                victim.healthComponent.body.gameObject.AddComponent<Grabbed>();
                _grabbedObject = victim.healthComponent.GetComponent<Rigidbody>();
                if(_grabbedObject)
                {
                    bodyHadGravity = _grabbedObject.useGravity;
                    bodyWasKinematic = _grabbedObject.isKinematic;
                    collisionDetectionMode = _grabbedObject.collisionDetectionMode;
                }
                CharacterMotor component = victim.healthComponent.GetComponent<CharacterMotor>();
                if (component)
                {
                    bodyHadGravity = component.gravityParameters.CheckShouldUseGravity();
                    gravParams = component.gravityParameters;
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

        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (inputBank.skill2.down)
            {
                Grab();
            }

            _pickDistance = Mathf.Clamp(_pickDistance + Input.mouseScrollDelta.y * 10f, _minGrabDistance, _maxGrabDistance);

            StartAimMode();
            if (_grabbedObject != null)
            {
                var ray = base.inputBank.GetAimRay();
                _pickTargetPosition = ray.origin + ray.direction * _pickDistance;// + _pickOffset;
                var forceDir = _pickTargetPosition - GetTargetRootPosition();
                _pickForce = forceDir / Time.fixedDeltaTime * 0.3f / Mathf.Clamp(_grabbedObject.mass, 60f, 120f);
                CharacterMotor component = victim.healthComponent.body.gameObject.GetComponent<CharacterMotor>();
                Rigidbody component2 = victim.healthComponent.body.gameObject.GetComponent<Rigidbody>();
                if(component.isGrounded) component.Motor.ForceUnground();
                if (component && component.velocity.magnitude < 200)
                {
                    component.velocity += _pickForce;
                }
                else if (component2 && component2.velocity.magnitude < 200)
                {
                    component2.velocity += _pickForce;
                }
                else if(component)
                {
                    component.velocity = Vector3.ClampMagnitude(component.velocity, 200);
                }
                else if(component2)
                {
                    component2.velocity = Vector3.ClampMagnitude(component2.velocity, 200);
                }
                if (!inputBank.skill2.down || !victim.healthComponent.alive)
                {
                    if (_grabbedObject)
                    {
                        if (component)
                        {
                            component.velocity += _pickForce;
                        }
                        if (component2)
                        {
                            component2.velocity += _pickForce;
                        }
                        Release();
                    }
                    else if (base.isAuthority)
                    {
                        outer.SetNextStateToMain();
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
            var ray = base.inputBank.GetAimRay();
            if(Physics.Raycast(ray, out RaycastHit hit, _maxGrabDistance) && hit.rigidbody == victim.healthComponent.GetComponent<Rigidbody>())
            {
                //_pickOffset = victim.healthComponent.GetComponent<Rigidbody>().worldCenterOfMass - hit.point;
                _pickDistance = hit.distance;
                _grabbedObject.isKinematic = false;
                _grabbedObject.useGravity = false;
                _grabbedObject.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            }
            CharacterMotor component = victim.healthComponent.body.gameObject.GetComponent<CharacterMotor>();
            if (component)
            {
                component.disableAirControlUntilCollision = true;
                component.gravityParameters = new CharacterGravityParameters
                {
                    environmentalAntiGravityGranterCount = 0,
                    antiGravityNeutralizerCount = 0,
                    channeledAntiGravityGranterCount = 0
                };
            }
        }

        private void Release()
        {
            CharacterMotor component = victim.healthComponent.body.gameObject.GetComponent<CharacterMotor>();
            if (component)
            {
                component.useGravity = bodyHadGravity;
                component.gravityParameters = gravParams;
            }
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
            _grabbedObject.collisionDetectionMode = collisionDetectionMode;
            _grabbedObject.useGravity = bodyHadGravity;
            _grabbedObject.isKinematic = bodyWasKinematic;
            GameObject.Destroy(victim.healthComponent.body.gameObject.GetComponent<Grabbed>());
            _grabbedObject = null;
            if (base.isAuthority) outer.SetNextStateToMain();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}