using System.Linq;
using UnityEngine;
using RoR2;

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

        private readonly BullseyeSearch search = new BullseyeSearch();

        private void Awake()
        {
            indicator = new Indicator(base.gameObject, LegacyResourcesAPI.Load<GameObject>("Prefabs/HuntressTrackingIndicator"));
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
                if (rigidbody.gameObject.transform.root.name == "ScissorR(Clone)" || rigidbody.gameObject.transform.root.name == "ScissorL(Clone)") return rigidbody;
                else return null;
            }
            else return rigidbody;
        }
        private void OnEnable()
        {
            indicator.active = true;
        }

        private void OnDisable()
        {
            indicator.active = false;
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
                if(rigidbody != null)
                {
                    if (rigidbody.gameObject.transform.root.name == "ScissorR(Clone)" || rigidbody.gameObject.transform.root.name == "ScissorL(Clone)")
                    {
                        indicator.targetTransform = (trackingTarget ? rigidbody.transform : null);
                    }
                    else
                    {
                        indicator.targetTransform = (trackingTarget ? trackingTarget.transform : null);
                    }
                }
                else
                {
                    indicator.targetTransform = (trackingTarget ? trackingTarget.transform : null);
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

