using BepInEx;
using SeamstressMod.Survivors.Seamstress;
using R2API.Utils;
using RoR2;
using System.Collections.Generic;
using System.Security;
using System.Security.Permissions;

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

//rename this namespace
namespace SeamstressMod
{
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    [BepInPlugin(MODUID, MODNAME, MODVERSION)]

    public class SeamstressPlugin : BaseUnityPlugin
    {
        // if you do not change this, you are giving permission to deprecate the mod-
        //  please change the names to your own stuff, thanks
        //   this shouldn't even have to be said
        public const string MODUID = "com.kenko.SeamstressMod";
        public const string MODNAME = "SeamstressMod";
        public const string MODVERSION = "1.0.0";

        // a prefix for name tokens to prevent conflicts- please capitalize all name tokens for convention
        public const string DEVELOPER_PREFIX = "KENKO";

        public static SeamstressPlugin instance;

        void Awake()
        {
            instance = this;

            Log.Init(Logger);
            Modules.Language.Init();
            DamageTypes.Init();
            // collect item display prefabs for use in our display rules
            Modules.ItemDisplays.PopulateDisplays();

            // character initialization. this should be after itemdisplays
            new SeamstressSurvivor().Initialize();

            // now make a content pack and add it this has to be last
            new Modules.ContentPacks().Initialize();


        }
    }
}
