using UnityEngine;
using UnityEngine.UI;
using RoR2;
using RoR2.UI;
using TMPro;
using UnityEngine.Networking;
namespace SeamstressMod.Survivors.Seamstress
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
            if (!seamCon.inButchered)
            {
                Object.Destroy(base.gameObject);
            }
        }
    }
}


