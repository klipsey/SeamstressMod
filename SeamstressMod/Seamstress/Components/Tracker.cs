using System.Linq;
using UnityEngine;
using RoR2;
using UnityEngine.Networking;
using SeamstressMod.Seamstress;
using SeamstressMod.Seamstress.Content;

namespace SeamstressMod.Seamstress.Components

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
            indicator = new Indicator(gameObject, SeamstressAssets.trackingTelekinesis);
            indicator2 = new Indicator(gameObject, SeamstressAssets.notTrackingTelekinesis);
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
                if (!trackingTarget.healthComponent.body.HasBuff(SeamstressBuffs.manipulatedCd))
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
                enabled = false;
            }
            trackerUpdateStopwatch += Time.fixedDeltaTime;
            if (trackerUpdateStopwatch >= 1f / trackerUpdateFrequency)
            {
                trackerUpdateStopwatch -= 1f / trackerUpdateFrequency;
                _ = trackingTarget;
                Ray aimRay = new Ray(inputBank.aimOrigin, inputBank.aimDirection);
                SearchForTarget(aimRay);
                if (NetworkServer.active)
                {
                    if (trackingTarget != null) onCooldown = trackingTarget.healthComponent.body.HasBuff(SeamstressBuffs.manipulatedCd);
                }
                if (onCooldown)
                {
                    indicator2.targetTransform = trackingTarget ? trackingTarget.transform : null;
                    indicator.targetTransform = null;
                }
                else
                {
                    indicator.targetTransform = trackingTarget ? trackingTarget.transform : null;
                    indicator2.targetTransform = null;
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
            search.FilterOutGameObject(gameObject);
            trackingTarget = search.GetResults().FirstOrDefault();
        }
    }
}

