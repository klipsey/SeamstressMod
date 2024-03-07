using System.Linq;
using UnityEngine;
using RoR2;
using UnityEngine.Networking;

namespace SeamstressMod.Survivors.Seamstress

{
    [RequireComponent(typeof(TeamComponent))]
    [RequireComponent(typeof(CharacterBody))]
    [RequireComponent(typeof(InputBankTest))]
    public class Tracker : MonoBehaviour
    {
        public float maxTrackingDistance = 40f;

        public float maxTrackingAngle = 10f;

        public float trackerUpdateFrequency = 10f;

        private HurtBox trackingTarget;

        private Rigidbody rigidbody;

        private CharacterBody characterBody;

        private TeamComponent teamComponent;

        private InputBankTest inputBank;

        private float trackerUpdateStopwatch;

        private Indicator indicator;

        private Indicator indicator2;

        private bool onCooldown;

        private readonly BullseyeSearch search = new BullseyeSearch();

        private void Awake()
        {
            indicator = new Indicator(base.gameObject, SeamstressAssets.trackingTelekinesis);
            indicator2 = new Indicator(base.gameObject, SeamstressAssets.notTrackingTelekinesis);
        }

        private void Start()
        {
            characterBody = GetComponent<CharacterBody>();
            inputBank = GetComponent<InputBankTest>();
            teamComponent = GetComponent<TeamComponent>();
        }

        public HurtBox GetTrackingTarget()
        {
            if (trackingTarget != null)
            {
                if(!trackingTarget.healthComponent.body.HasBuff(SeamstressBuffs.manipulatedCd))
                {
                    return trackingTarget;
                }
                else
                {
                    return null;
                }
            }
            else return trackingTarget;
        }

        public Rigidbody GetRigidbody() 
        {
            if (rigidbody != null)
            {
                if (rigidbody.gameObject.transform.name == "ScissorR(Clone)" || rigidbody.gameObject.transform.name == "ScissorL(Clone)") return rigidbody;
                else return null;
            }
            else return rigidbody;
        }
        private void OnEnable()
        {
            indicator.active = true;
            indicator2.active = true;
        }

        private void OnDisable()
        {
            indicator.active = false;
            indicator2.active = false;
        }

        private void FixedUpdate()
        {
            if (characterBody.skillLocator.secondary.skillNameToken != SeamstressSurvivor.SEAMSTRESS_PREFIX + "SECONDARY_PLANMAN_NAME")
            {
                this.enabled = false;
            }
            trackerUpdateStopwatch += Time.fixedDeltaTime;
            if (trackerUpdateStopwatch >= 1f / trackerUpdateFrequency)
            {
                trackerUpdateStopwatch -= 1f / trackerUpdateFrequency;
                _ = trackingTarget;
                Ray aimRay = new Ray(inputBank.aimOrigin, inputBank.aimDirection);
                SearchForTarget(aimRay);
                SearchForScissors(aimRay);
                if (NetworkServer.active && trackingTarget != null)
                {
                    if(rigidbody != null)
                    {
                        if (rigidbody.gameObject.transform.name == "ScissorR(Clone)" || rigidbody.gameObject.transform.name == "ScissorL(Clone)") onCooldown = false;
                        else onCooldown = trackingTarget.healthComponent.body.HasBuff(SeamstressBuffs.manipulatedCd);
                    }
                    else onCooldown = trackingTarget.healthComponent.body.HasBuff(SeamstressBuffs.manipulatedCd);
                }
                if (rigidbody != null)
                {
                    if (rigidbody.gameObject.transform.name == "ScissorR(Clone)" || rigidbody.gameObject.transform.name == "ScissorL(Clone)")
                    {
                        indicator.targetTransform = (rigidbody ? rigidbody.transform : null);
                        onCooldown = false;
                    }
                    else
                    {
                        if (onCooldown)
                        {
                            indicator2.targetTransform = (trackingTarget ? trackingTarget.transform : null);
                            indicator.targetTransform = null;
                        }
                        else
                        {
                            indicator2.targetTransform = null;
                            indicator.targetTransform = (trackingTarget ? trackingTarget.transform : null);
                        }
                    }
                }
                else
                {
                    if (onCooldown)
                    {
                        indicator2.targetTransform = (trackingTarget ? trackingTarget.transform : null);
                        indicator.targetTransform = null;
                    }
                    else
                    {
                        indicator2.targetTransform = null;
                        indicator.targetTransform = (trackingTarget ? trackingTarget.transform : null);
                    }
                }
            }
        }

        private void SearchForTarget(Ray aimRay)
        {
            search.teamMaskFilter = TeamMask.GetUnprotectedTeams(teamComponent.teamIndex);
            search.filterByLoS = true;
            search.searchOrigin = aimRay.origin;
            search.searchDirection = aimRay.direction;
            search.sortMode = BullseyeSearch.SortMode.Distance;
            search.maxDistanceFilter = maxTrackingDistance;
            search.maxAngleFilter = maxTrackingAngle;
            search.RefreshCandidates();
            search.FilterOutGameObject(base.gameObject);
            trackingTarget = search.GetResults().FirstOrDefault();
        }

        private void SearchForScissors(Ray aimRay)
        {
            if (Physics.Raycast(aimRay, out RaycastHit hit, maxTrackingDistance) && hit.rigidbody)
            {
                rigidbody = hit.rigidbody;
            }
            else
            {
                rigidbody = null;
            }
        }
    }
}

