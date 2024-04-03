using R2API.Networking.Interfaces;
using RoR2;
using UnityEngine.Networking;
using UnityEngine;

namespace SeamstressMod.Seamstress.Components
{
    internal class SyncBlink : INetMessage
    {
        private NetworkInstanceId netId;
        private bool blinkReady;
        private ulong blinkCD;

        public SyncBlink()
        {
        }

        public SyncBlink(NetworkInstanceId netId, bool blinkReady, ulong blinkCD)
        {
            this.netId = netId;
            this.blinkReady = blinkReady;
            this.blinkCD = blinkCD;
        }

        public void Deserialize(NetworkReader reader)
        {
            this.netId = reader.ReadNetworkId();
            this.blinkReady = reader.ReadBoolean();
        }

        public void OnReceived()
        {
            GameObject bodyObject = Util.FindNetworkObject(this.netId);
            if (!bodyObject)
            {
                Log.Message("No Body Object");
                return;
            }

            SeamstressController seamCon = bodyObject.GetComponent<SeamstressController>();
            if (seamCon)
            {
                seamCon.blinkReady = this.blinkReady;
                seamCon.blinkCd = this.blinkCD;
            }
        }

        public void Serialize(NetworkWriter writer)
        {
            writer.Write(this.netId);
            writer.Write(this.blinkReady);
            writer.Write(this.blinkCD);
        }
    }
}
