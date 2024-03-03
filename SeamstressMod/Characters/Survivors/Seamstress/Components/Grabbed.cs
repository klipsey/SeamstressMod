using System.Linq;
using UnityEngine;
using RoR2;
using UnityEngine.Networking;
using SeamstressMod.Modules;

namespace SeamstressMod.Survivors.Seamstress

{
    public class Grabbed : MonoBehaviour
    {
        CharacterBody characterBody;
        private void Awake()
        {
            this.characterBody = GetComponent<CharacterBody>();
            if(NetworkServer.active)
            {
                this.characterBody.AddBuff(SeamstressBuffs.manipulated);
            }
        }

        private void Start()
        {
        }

        private void FixedUpdate()
        {
        }

        private void OnDestroy()
        {
            if (NetworkServer.active && this.characterBody) this.characterBody.RemoveBuff(SeamstressBuffs.manipulated);
        }
    }

}

