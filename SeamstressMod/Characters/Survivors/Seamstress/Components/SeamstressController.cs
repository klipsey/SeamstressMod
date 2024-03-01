using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;
using RoR2.EntityLogic;
using UnityEngine.Networking;

namespace SeamstressMod.Survivors.Seamstress
{
    public class SeamstressController : NetworkBehaviour
    {
        private CharacterBody characterBody;

        private CharacterMotor characterMotor;

        private HealthComponent healthComponent;

        private SkillLocator skillLocator;

        private GameObject endReap = SeamstressAssets.reapEndEffect;

        private float fiendGauge = 0f;

        private float drainAmount;

        public float hopoopFeatherTimer;

        public float blinkCd;

        public bool blinkReady;

        private bool hasPlayed = false;

        private bool butchered = false;

        private bool hasFired = false;

        //private float leapLength = 0f;

        //public float lockOutLength = 0f;

        private float bd = 0f;

        public bool fuckYou = false;

        public bool drainGauge;
        public void Awake()
        {
            characterBody = base.GetComponent<CharacterBody>();
            characterMotor = base.GetComponent<CharacterMotor>();
            healthComponent = base.GetComponent<HealthComponent>();
            skillLocator = base.GetComponent<SkillLocator>();
        }
        public void FixedUpdate()
        {
            hopoopFeatherTimer -= Time.fixedDeltaTime;
            blinkCd -= Time.fixedDeltaTime;
            if (bd > 0f)
            {
                bd -= Time.fixedDeltaTime;
            }
            if (drainGauge && fiendGauge > 0)
            {
                fiendGauge -= drainAmount;
            }
            else if (fiendGauge < 0)
            {
                drainGauge = false;
                fiendGauge = 0f;
            }
            Log.Debug("Fiend gauge " + fiendGauge);
            CalculateBonusDamage();
            ButcheredSound();
            IsButchered();      
        }
        public void RefreshBlink()
        {
            this.blinkReady = true;
        }
        public void FiendGaugeCalc(float healDamage)
        {
            if((fiendGauge + healDamage) < (healthComponent.fullHealth * SeamstressStaticValues.maxFiendGaugeCoefficient))
            {
                fiendGauge += healDamage;
            }
            else
            {
                fiendGauge = healthComponent.fullHealth * SeamstressStaticValues.maxFiendGaugeCoefficient;
            }
        }
        public float FiendGaugeAmount()
        {
            return fiendGauge;
        }
        public float FiendGaugeAmountPercent()
        {
            return (fiendGauge / (healthComponent.fullHealth * SeamstressStaticValues.maxFiendGaugeCoefficient));
        }
        private void IsButchered()
        {
            if (fuckYou && !butchered)
            {
                drainGauge = false;
                bd = SeamstressStaticValues.butcheredDuration;
                butchered = true;
                hasFired = false;
                Transform modelTransform = characterBody.modelLocator.modelTransform;
                if (modelTransform)
                {
                    #region overlay
                    TemporaryOverlay temporaryOverlay = modelTransform.gameObject.AddComponent<TemporaryOverlay>();
                    temporaryOverlay.duration = 1f;
                    temporaryOverlay.destroyComponentOnEnd = true;
                    temporaryOverlay.originalMaterial = Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/matOnFire.mat").WaitForCompletion();
                    temporaryOverlay.inspectorCharacterModel = modelTransform.GetComponent<CharacterModel>();
                    temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                    temporaryOverlay.animateShaderAlpha = true;
                    #endregion
                }
            }
            else if (!fuckYou && butchered)
            {
                butchered = false;
                drainGauge = true;
                drainAmount = healthComponent.fullHealth / 125;
                Transform modelTransform = characterBody.modelLocator.modelTransform;
                if (modelTransform)
                {
                    TemporaryOverlay temporaryOverlay = modelTransform.gameObject.AddComponent<TemporaryOverlay>();
                temporaryOverlay.duration = 1f;
                temporaryOverlay.destroyComponentOnEnd = true;
                temporaryOverlay.originalMaterial = SeamstressAssets.destealthMaterial;
                temporaryOverlay.inspectorCharacterModel = modelTransform.GetComponent<CharacterModel>();
                temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                temporaryOverlay.animateShaderAlpha = true;
                }
                UnityEngine.Object.Instantiate<GameObject>(endReap, characterBody.modelLocator.transform);
                Util.PlaySound("Play_voidman_transform_return", characterBody.gameObject);
                //fire expunge at end of butchered
            }
        }
        //butchered end sound
        private void ButcheredSound()
        {
            if (fuckYou)
            {
                if (bd < 2f && !hasPlayed)
                {
                    Util.PlaySound("Play_nullifier_impact", characterBody.gameObject);
                    hasPlayed = true;
                }
            }
            hasPlayed = false;
        }
        //passive damage
        private void CalculateBonusDamage()
        {
            float healthMissing = (healthComponent.fullHealth + healthComponent.fullShield) - (healthComponent.health + healthComponent.shield);
            float fakeHealthMissing = (healthComponent.fullHealth) * 0.5f;
            if(fuckYou && skillLocator.utility.skillNameToken == SeamstressSurvivor.SEAMSTRESS_PREFIX + "UTILITY_PARRY_NAME") characterBody.baseDamage = 8f + (fakeHealthMissing * SeamstressStaticValues.passiveScaling) + (healthMissing * SeamstressStaticValues.passiveScaling);
            else characterBody.baseDamage = 8f + (healthMissing * SeamstressStaticValues.passiveScaling);
        }
    }
}