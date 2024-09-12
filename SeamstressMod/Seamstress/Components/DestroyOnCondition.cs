using UnityEngine;
using UnityEngine.UI;
using RoR2;
using RoR2.UI;
using TMPro;
using UnityEngine.Networking;
using SeamstressMod.Seamstress.Content;

namespace SeamstressMod.Seamstress.Components
{
    public class DestroyOnCondition : MonoBehaviour
    {
        public CharacterBody ownerBody;

        private bool hasHadBuff;

        private void Awake()
        {
        }
        private void OnEnable()
        {

        }
        private void FixedUpdate()
        {
            if(ownerBody.HasBuff(SeamstressBuffs.SeamstressInsatiableBuff))
            {
                hasHadBuff = true;
            }
            else if (!ownerBody.HasBuff(SeamstressBuffs.SeamstressInsatiableBuff) && hasHadBuff)
            {
                Destroy(this.gameObject);
            }
        }
    }
}