﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SeamstressMod.Modules
{
    internal static class SoundBanks
    {
        private static bool initialized = false;
        public static string SoundBankDirectory
        {
            get
            {
                return Path.Combine(Path.Combine(Path.GetDirectoryName(SeamstressPlugin.instance.Info.Location)), "SoundBanks");
            }
        }

        public static void Init()
        {
            if (initialized) return;
            initialized = true;
            AKRESULT akResult = AkSoundEngine.AddBasePath(SoundBankDirectory);

            AkSoundEngine.LoadBank("seam_bank.bnk", out _);
        }
    }
}