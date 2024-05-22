using UnityEngine;
using RoR2;

namespace SeamstressMod.Seamstress.Components
{
    public class SeamstressCSS : MonoBehaviour
    {
        private bool hasPlayed = false;
        private bool hasPlayed2 = false;
        private float timer = 0f;
        private void Awake()
        {
        }
        private void FixedUpdate()
        {
            timer += Time.fixedDeltaTime;
            if (!hasPlayed && timer >= 0.1f)
            {
                hasPlayed = true;
                Util.PlaySound("sfx_seamstress_snap", this.gameObject);
            }
            if(!hasPlayed2 && timer >= 0.6f)
            {
                hasPlayed2 = true;
                Util.PlaySound("sfx_seamstress_cloth", this.gameObject);
            }
        }
    }
}
