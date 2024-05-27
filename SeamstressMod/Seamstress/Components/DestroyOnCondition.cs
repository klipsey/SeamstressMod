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
        public SeamstressController seamstressController;

        private void Awake()
        {
        }
        private void OnEnable()
        {

        }
        private void FixedUpdate()
        {
            if (!seamstressController.inInsatiableSkill)
            {
                Destroy(this.gameObject);
            }
        }
    }
}