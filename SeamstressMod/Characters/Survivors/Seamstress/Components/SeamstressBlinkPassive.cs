using RoR2;
using RoR2.Skills;
using UnityEngine;

namespace SeamstressMod.Survivors.Seamstress
{
    public class SeamstressPassive : MonoBehaviour
    {
        public SkillDef blinkPassive;
        public SkillDef impGauge;

        public GenericSkill passiveSkillSlot;
        public GenericSkill impGaugeSkillSlot;

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