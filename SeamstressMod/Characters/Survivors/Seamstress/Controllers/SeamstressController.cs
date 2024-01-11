using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;
using UnityEngine;
using SeamstressMod.Modules;

namespace SeamstressMod.Survivors.Seamstress
{
    public class SeamstressController : MonoBehaviour
    {
        public CharacterBody characterBody;
        private bool hasEffectiveAuthority => characterBody.hasEffectiveAuthority;

        public HealthComponent healthComponent;

        public SkillLocator skillLocator;

        public bool butchered;

        public void Start()
        {
            healthComponent = GetComponent<HealthComponent>();
            characterBody = GetComponent<CharacterBody>();
            skillLocator = GetComponent<SkillLocator>();
        }
        public void FixedUpdate()
        {
            IsButchered();
            CalculateBonusDamage();
        }
        public void IsButchered()
        {
            if (characterBody.HasBuff(SeamstressBuffs.bloodBath))
            {
                butchered = true;
            }
            else
            {
                butchered = false;
                TemporaryOverlay component = GetComponent<TemporaryOverlay>();
                if ((bool)component)
                {
                    DestroyImmediate(component);
                }
            }
        }
        public void CalculateBonusDamage()
        {
            float healthMissing = healthComponent.fullHealth - healthComponent.health;
            characterBody.baseDamage = 10f + (healthMissing * 0.1f);
        }
    }
}
