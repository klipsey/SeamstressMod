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
        public SeamstressController seamCon;

        private void Awake()
        {
        }
        private void OnEnable()
        {

        }
        private void FixedUpdate()
        {
            if (!seamCon.inInsatiable)
            {
                Destroy(gameObject);
            }
        }
    }
}


