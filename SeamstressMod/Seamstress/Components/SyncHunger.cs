using R2API.Networking.Interfaces;
using RoR2;
using UnityEngine.Networking;
using UnityEngine;

namespace SeamstressMod.Seamstress.Components
{
    internal class SyncHunger : INetMessage
    {
        private NetworkInstanceId netId;
        private ulong gauge;

        public SyncHunger()
        {
        }

        public SyncHunger(NetworkInstanceId netId, ulong gauge)
        {
            this.netId = netId;
            this.gauge = gauge;
        }

        public void Deserialize(NetworkReader reader)
        {
            this.netId = reader.ReadNetworkId();
            this.gauge = reader.ReadUInt64();
        }

        public void OnReceived()
        {
            GameObject bodyObject = Util.FindNetworkObject(this.netId);
            if (!bodyObject)
            {
                return;
            }

            SeamstressMod.Seamstress.Components.SeamstressController seamCon = bodyObject.GetComponent<SeamstressMod.Seamstress.Components.SeamstressController>();
            if (seamCon)
            {
                seamCon.fiendMeter = this.gauge * 0.01f;
            }
        }

        public void Serialize(NetworkWriter writer)
        {
            writer.Write(this.netId);
            writer.Write(this.gauge);
        }
    }
}
