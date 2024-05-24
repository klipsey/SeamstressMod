using UnityEngine;
using UnityEngine.UI;
using RoR2;
using RoR2.UI;
using TMPro;
using UnityEngine.Networking;

namespace SeamstressMod.Seamstress.Components
{
    public class DestroyOnCondition : MonoBehaviour
    {
        public SeamstressController seamCom;

        private void Awake()
        {
        }
        private void OnEnable()
        {

        }
        private void FixedUpdate()
        {
            if (!seamCom.inInsatiableSkill)
            {
                Destroy(this.gameObject);
            }
        }
    }
}


