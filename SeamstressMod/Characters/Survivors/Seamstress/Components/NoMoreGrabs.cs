using System.Linq;
using UnityEngine;
using RoR2;
using UnityEngine.Networking;
using SeamstressMod.Modules;

namespace SeamstressMod.Survivors.Seamstress

{
    public class NoMoreGrabs : MonoBehaviour
    {
        private float stopwatch;
        private void Awake()
        {
        }

        private void Start()
        {
        }

        private void FixedUpdate()
        {
            stopwatch += Time.deltaTime;
            if (stopwatch > 6f) Object.Destroy(this);
        }

        private void OnDestroy()
        {
        }
    }

}

