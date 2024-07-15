using BepInEx;
using R2API.Utils;
using RoR2;
using System.Collections.Generic;
using System.Security;
using System.Security.Permissions;
using R2API.Networking;
using SeamstressMod.Modules;
using SeamstressMod.Seamstress.Content;
using ShaderSwapper;

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

//rename this namespace
namespace SeamstressMod
{
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    [BepInPlugin(MODUID, MODNAME, MODVERSION)]
    [BepInDependency(NetworkingAPI.PluginGUID)]
    [BepInDependency("com.weliveinasociety.CustomEmotesAPI", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.DestroyedClone.AncientScepter", BepInDependency.DependencyFlags.SoftDependency)]
    public class SeamstressPlugin : BaseUnityPlugin
    {
        // if you do not change this, you are giving permission to deprecate the mod-
        //  please change the names to your own stuff, thanks
        //   this shouldn't even have to be said
        public const string MODUID = "com.kenko.Seamstress";
        public const string MODNAME = "Seamstress";
        public const string MODVERSION = "1.0.0";

        // a prefix for name tokens to prevent conflicts- please capitalize all name tokens for convention
        public const string DEVELOPER_PREFIX = "KENKO";

        public static SeamstressPlugin instance;

        public static bool emotesInstalled => BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.weliveinasociety.CustomEmotesAPI");
        public static bool scepterInstalled => BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.DestroyedClone.AncientScepter");
        void Awake()
        {
            instance = this;

            //easy to use logger
            Log.Init(Logger);

            // used when you want to properly set up language folders
            Modules.Language.Init();

            //SeamstressAssets.Init(Assets.LoadAssetBundle("seamstressassets"));
            //StartCoroutine(SeamstressAssets.mainAssetBundle.UpgradeStubbedShadersAsync());
            // character initialization
            new Seamstress.SeamstressSurvivor().Initialize();

            // make a content pack and add it. this has to be last
            new ContentPacks().Initialize();

            //On.RoR2.Networking.NetworkManagerSystemSteam.OnClientConnect += (s, u, t) => { };
        }

        private void Start()
        {
            SoundBanks.Init();
        }
    }
}
