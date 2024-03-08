using RoR2;
using RoR2.Skills;
using UnityEngine;

namespace SeamstressMod.Survivors.Seamstress
{
    public class SeamstressBlinkPassive : MonoBehaviour
    {
        public SkillDef blinkPassive;
        public GenericSkill passiveSkillSlot;

        public bool isBlink
        {
            get
            {
                if (this.blinkPassive && this.passiveSkillSlot)
                {
                    return this.passiveSkillSlot.skillDef == this.blinkPassive;
                }

                return false;
            }
        }
    }
}