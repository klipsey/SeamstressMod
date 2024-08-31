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

        private void Awake()
        {
        }
        private void OnEnable()
        {

        }
        private void FixedUpdate()
        {
            if (ownerBody && !ownerBody.HasBuff(SeamstressBuffs.SeamstressInsatiableBuff))
            {
                Destroy(this.gameObject);
            }
        }
    }
}