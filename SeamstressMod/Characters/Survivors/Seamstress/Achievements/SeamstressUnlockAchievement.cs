using R2API;
using RoR2;
using RoR2.Achievements;
using System;
using System.Collections.Generic;
using System.Text;

namespace SeamstressMod.Survivors.Seamstress.Achievements
{
    [RegisterAchievement(identifier, unlockableIdentifier, null, null)]
    public class SeamstressUnlockAchievement : BaseAchievement
    {
        public const string identifier = SeamstressSurvivor.SEAMSTRESS_PREFIX + "UNLOCK_ACHIEVEMENT";
        public const string unlockableIdentifier = SeamstressSurvivor.SEAMSTRESS_PREFIX + "UNLOCK_ACHIEVEMENT";
        public CharacterBody body;

        private void CharacterBody_OnDeathStart(On.RoR2.CharacterBody.orig_OnDeathStart orig, CharacterBody self)
        {
            if (self)
            {
                if (self.bodyIndex == BodyCatalog.FindBodyIndex("BrotherBody") &&
                    self.HasBuff(RoR2Content.Buffs.Bleeding))
                {
                    base.Grant();
                }
            }
            orig(self);
        }

        public override void OnInstall()
        {
            base.OnInstall();
            On.RoR2.CharacterBody.OnDeathStart += CharacterBody_OnDeathStart;
            On.RoR2.CharacterBody.Start += CharacterBody_Start;
        }

        private void CharacterBody_Start(On.RoR2.CharacterBody.orig_Start orig, CharacterBody self)
        {
            orig(self);
            if (self)
            {
                if (self.isPlayerControlled)
                {
                    if (!body)
                    {
                        body = self;
                    }
                }
            }
        }

        public override void OnUninstall()
        {
            base.OnUninstall();
            On.RoR2.CharacterBody.OnDeathStart -= CharacterBody_OnDeathStart;
            On.RoR2.CharacterBody.Start -= CharacterBody_Start;
        }
    }
}
