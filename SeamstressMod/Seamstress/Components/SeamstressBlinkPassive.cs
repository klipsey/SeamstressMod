using RoR2;
using RoR2.Skills;
using UnityEngine;

namespace SeamstressMod.Seamstress.Components
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
                if (blinkPassive && passiveSkillSlot)
                {
                    return passiveSkillSlot.skillDef == blinkPassive;
                }

                return false;
            }
        }
    }
}