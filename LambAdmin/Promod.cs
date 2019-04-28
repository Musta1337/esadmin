using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using InfinityScript;

namespace LambAdmin
{
    public partial class LAdmin
    {
        public static partial class ConfigValues
        {
            public static bool PROMOD
            {
                get
                {
                    return bool.Parse(Sett_GetString("settings_promod"));
                }
            }
            public static class PROMOD_SETTINGS
            {
                public static bool ColoredScoreBoard
                {
                    get
                    {
                        return bool.Parse(PROMOD_Sett_GetString("settings_promod_coloredScoreboard"));
                    }
                }
                public static bool NormalKillcam
                {
                    get
                    {
                        return bool.Parse(PROMOD_Sett_GetString("settings_promod_normalkillcam"));
                    }
                }
                public static bool NoSentry
                {
                    get
                    {
                        return bool.Parse(PROMOD_Sett_GetString("settings_promod_NoSentry"));
                    }
                }
                public static bool GernadeThrow
                {
                    get
                    {
                        return bool.Parse(PROMOD_Sett_GetString("settings_promod_NONadeThrowingback"));
                    }
                }
            }

        }
        public void PROMOD_OnServerStart()
        {
            PROMOD_Breath();
            PROMOD_BaseSettings();
            PROMOD_InitCommands();
            try
            {
                NoSoundADS();
            }
            catch (Exception)
            {
            }
            PlayerConnected += PROMOD_OnPlayerConnect;
            PlayerConnecting += PROMOD_OnPlayerConnecting;
            PlayerActuallySpawned += PROMOD_OnPlayerSpawn;
            OnPlayerDamageEvent += PROMOD_OnPlayerDamage;

        }
        public void PROMOD_BaseSettings()
        {
            string mode = Call<string>("getdvar", "g_gametype");
            if (ConfigValues.PROMOD_SETTINGS.GernadeThrow && !mode.Contains("infect"))
            {
                Call("setdvar", "player_throwBackInnerRadius", "0");
                Call("setdvar", "player_throwBackOuterRadius", "0");
            }
            else
            {
                Call("setdvar", "player_throwBackInnerRadius", "90");
                Call("setdvar", "player_throwBackOuterRadius", "90");
            }
            if (mode.Contains("sd"))
            {
                ExecuteCommand("set g_hardcore 1");
                WriteLog.Info("Search and Destroy and Promod configuration detected, Enabling Hardcore.");
            }
            //scr_game_allowkillcam
            ExecuteCommand("set scr_showperksonspawn 0");
            if (ConfigValues.PROMOD_SETTINGS.NormalKillcam)
            {
                ExecuteCommand("set scr_game_allowkillcam 1");
            }
            else
            {
                ExecuteCommand("set scr_game_allowkillcam 0");
            }
            ExecuteCommand("set ui_hud_showdeathicons 0");
            ExecuteCommand("set scr_game_matchstarttime 5");
            ExecuteCommand("set scr_game_playerwaittime 3");
            ExecuteCommand("set scr_sd_planttime 5");
            ExecuteCommand("set scr_sd_defusetime 7.5");
            Call("setdvar", "glass_DamageToDestroy", "50");
            if (mode != "gun" && mode != "oic" && mode != "dm" && mode != "war" && mode != "infect")
            {
                ExecuteCommand("set scr_" + mode + "_score_kill 5");
                ExecuteCommand("set scr_" + mode + "_score_headshot 5");
                ExecuteCommand("set scr_" + mode + "_score_assist 2");
                ExecuteCommand("set scr_" + mode + "_score_plant 10");
                ExecuteCommand("set scr_" + mode + "_score_defuse 10");
                ExecuteCommand("set scr_" + mode + "_score_teamkill 5");
                ExecuteCommand("set scr_" + mode + "_score_capture 5");
                ExecuteCommand("set scr_" + mode + "_score_defend 5");
                ExecuteCommand("set scr_" + mode + "_score_defend_assist 2");
                ExecuteCommand("set scr_" + mode + "_score_assault 5");
                ExecuteCommand("set scr_" + mode + "_score_assault_assist 2");
            }
            if (ConfigValues.PROMOD_SETTINGS.NoSentry && !mode.Contains("infect"))
            {
                Call("setdvar", "player_MGUseRadius", "0");
            }
            else
            {
                Call("setdvar", "player_MGUseRadius", "1");
            }
        }
        public void PROMOD_OnPlayerConnect(Entity player)
        {
            player.OnNotify("weapon_change", delegate (Entity Player, Parameter weap)
            {
                if (weap.ToString() == "briefcase_bomb_mp")
                {
                    Parameter[] parameters = new Parameter[2]
                    {
                        Player.Origin,
                        "mp_bomb_plant"
                    };
                    Call("PlaySoundAtPos", parameters);
                }
                else if (weap.ToString() == "briefcase_bomb_defuse_mp")
                {
                    Parameter[] parameters2 = new Parameter[2]
                    {
                        Player.Origin,
                        "mp_bomb_defuse"
                    };
                    Call("PlaySoundAtPos", parameters2);
                }
            });
        }
        public void PROMOD_OnPlayerConnecting(Entity player)
        {
            player.SetClientDvar("useRelativeTeamColors", "1");
            player.SetClientDvar("cg_crosshairEnemyColor", "0");
            player.SetClientDvar("cg_drawcrosshairnames", "0");
            player.SetClientDvar("cg_brass", "0");
            player.SetClientDvar("r_distortion", "0");
            player.SetClientDvar("r_dlightlimit", "0");
            player.SetClientDvar("r_normalMap", "1");
            player.SetClientDvar("r_fog", "0");
            player.SetClientDvar("r_fastskin", "0");
            player.SetClientDvar("r_drawdecals", "1");
            player.SetClientDvar("clientsideeffects", "0");
            player.SetClientDvar("fx_draw", "1");
            player.SetClientDvar("cl_maxpackets", "100");
            player.SetClientDvar("snaps", "30");
            player.SetClientDvar("ragdoll_enable", "0");
            player.SetClientDvar("waypointIconHeight", "13");
            player.SetClientDvar("waypointIconWidth", "13");
            player.SetClientDvar("g_teamname_allies", "^1Team A");
            player.SetClientDvar("g_teamname_axis", "^2Team B");
            player.SetClientDvar("bg_weaponBobMax", "0");
            player.SetClientDvar("bg_viewBobMax", "0");
            player.SetClientDvar("bg_viewBobAmplitudeStandingAds", "0 0");
            player.SetClientDvar("bg_viewBobAmplitudeSprinting", "0 0");
            player.SetClientDvar("bg_viewBobAmplitudeDucked", "0 0");
            player.SetClientDvar("bg_viewBobAmplitudeDuckedAds", "0 0");
            player.SetClientDvar("bg_viewBobAmplitudeProne", "0 0");
            player.SetClientDvar("bg_viewKickRandom", "0.2");
            player.SetClientDvar("bg_viewKickMin", "1");
            player.SetClientDvar("bg_viewKickScale", "0.15");
            player.SetClientDvar("bg_viewKickMax", "75");
            if (ConfigValues.PROMOD_SETTINGS.ColoredScoreBoard)
            {
                player.SetClientDvar("g_ScoresColor_Axis", "0.180392 0.545098 0.341176 1");
                player.SetClientDvar("g_ScoresColor_Allies", "0.12 0.56 1 1");
                player.SetClientDvar("g_ScoresColor_Spectator", "0.423 1 0 0.49");
                player.SetClientDvar("g_ScoresColor_Free", "1 0.070 0 1");
            }
        }

        private void PROMOD_OnPlayerSpawn(Entity player)
        {
            player.Call("clearperks");
            if (player.Call<int>("hasperk", "specialty_reducedsway") == 0)
            {
                player.SetPerk("specialty_bulletaccuracy", true, false);
                player.SetPerk("specialty_reducedsway", true, false);
                player.SetPerk("specialty_coldblooded", true, false);
            }
            player.Call("GiveMaxAmmo", player.CurrentWeapon);
            if (player.HasWeapon("flash_grenade_mp"))
            {
                player.GiveWeapon("flash_grenade_mp");
                player.Call("SetWeaponAmmoClip", "flash_grenade_mp", 1);
            }
        }
        public void PROMOD_OnPlayerDamage(Entity player, Entity inflictor, Entity attacker, int damage, int dFlags, string mod, string weapon, Vector3 point, Vector3 dir, string hitLoc)
        {
            if (weapon == "frag_grenade_mp" && damage != 15)
            {
                damage = ((damage <= 10) ? ((int)((double)(damage * 12) * 1.15)) : ((int)((double)damage * 1.15)));
            }
            else
            {
                if (weapon == "flash_grenade_mp" && damage < 10)
                {
                    damage = 0;
                }
                if (weapon == "destructible_car" && damage <= 10)
                {
                    damage *= 12;
                }
                damage = (int)((double)damage * 1.15);
            }
        }
        #region Commands
        public void PROMOD_InitCommands()
        {
            // FT // FILMTWEAK
            CommandList.Add(new Command("ft", 1, Command.Behaviour.Normal,
                (sender, arguments, optarg) =>
                {
                    CMD_applyfilmtweakPromod(sender, arguments[0]);
                    WriteChatToPlayer(sender, Command.GetString("ft", "message").Format(new Dictionary<string, string>()
                    {
                        {"<ft>", arguments[0] },
                    }));
                }));
            CommandList.Add(new Command("snaps", 1, Command.Behaviour.Normal,
                (sender, arguments, optarg) =>
                {
                    CMD_applysnaps(sender, arguments[0]);
                    WriteChatToPlayer(sender, Command.GetString("snaps", "message").Format(new Dictionary<string, string>()
                    {
                        {"<snaps>", arguments[0] },
                    }));
                }));
            CommandList.Add(new Command("fov", 1, Command.Behaviour.Normal,
                (sender, arguments, optarg) =>
                {
                    CMD_applyfieldofview(sender, arguments[0]);
                    WriteChatToPlayer(sender, Command.GetString("fov", "message").Format(new Dictionary<string, string>()
                    {
                        {"<fov>", arguments[0] },
                    }));
                }));

            CommandList.Add(new Command("r_distortion", 1, Command.Behaviour.Normal,
                (sender, arguments, optarg) =>
                {
                    bool enabled = UTILS_ParseBool(arguments[0]);
                    CMD_r_distortion(sender, enabled);
                    if (enabled)
                        WriteChatToPlayer(sender, Command.GetString("r_distortion", "message_on"));
                    else
                        WriteChatToPlayer(sender, Command.GetString("r_distortion", "message_off"));
                }));
            CommandList.Add(new Command("r_fog", 1, Command.Behaviour.Normal,
                (sender, arguments, optarg) =>
                {
                    bool enabled = UTILS_ParseBool(arguments[0]);
                    CMD_r_fog(sender, enabled);
                    if (enabled)
                        WriteChatToPlayer(sender, Command.GetString("r_fog", "message_on"));
                    else
                        WriteChatToPlayer(sender, Command.GetString("r_fog", "message_off"));
                }));
            CommandList.Add(new Command("r_detail", 1, Command.Behaviour.Normal,
                (sender, arguments, optarg) =>
                {
                    bool enabled = UTILS_ParseBool(arguments[0]);
                    CMD_r_detail(sender, enabled);
                    if (enabled)
                        WriteChatToPlayer(sender, Command.GetString("r_detail", "message_on"));
                    else
                        WriteChatToPlayer(sender, Command.GetString("r_detail", "message_off"));
                }));
            CommandList.Add(new Command("r_dlightlimit", 1, Command.Behaviour.Normal,
                (sender, arguments, optarg) =>
                {
                    bool enabled = UTILS_ParseBool(arguments[0]);
                    CMD_r_dlightlimit(sender, enabled);
                    if (enabled)
                        WriteChatToPlayer(sender, Command.GetString("r_dlightlimit", "message_on"));
                    else
                        WriteChatToPlayer(sender, Command.GetString("r_dlightlimit", "message_off"));
                }));
        }
        public void CMD_r_distortion(Entity sender, bool state)
        {
            if (state)
                sender.SetClientDvar("r_distortion", "1");
            else
                sender.SetClientDvar("r_distortion", "0");
        }
        public void CMD_r_dlightlimit(Entity sender, bool state)
        {
            if (state)
                sender.SetClientDvar("r_dlightlimit", "1");
            else
                sender.SetClientDvar("r_dlightlimit", "0");
        }
        public void CMD_r_fog(Entity sender, bool state)
        {
            if (state)
                sender.SetClientDvar("r_fog", "1");
            else
                sender.SetClientDvar("r_fog", "0");
        }
        public void CMD_r_detail(Entity sender, bool state)
        {
            if (state)
                sender.SetClientDvar("r_detail", "1");
            else
                sender.SetClientDvar("r_detail", "0");
        }

        public void CMD_applyfieldofview(Entity sender, string fov)
        {
            switch (fov)
            {
                case "60":
                    sender.SetClientDvar("cg_fovscale", "0.92");
                    return;
                case "65":
                    sender.SetClientDvar("cg_fovscale", "1");
                    return;
                case "70":
                    sender.SetClientDvar("cg_fovscale", "1.08");
                    return;
                case "75":
                    sender.SetClientDvar("cg_fovscale", "1.15");
                    return;
                case "80":
                    sender.SetClientDvar("cg_fovscale", "1.23");
                    return;
                case "85":
                    sender.SetClientDvar("cg_fovscale", "1.30");
                    return;
                case "90":
                    sender.SetClientDvar("cg_fovscale", "1.38");
                    return;
                case "95":
                    sender.SetClientDvar("cg_fovscale", "1.46");
                    return;
                case "100":
                    sender.SetClientDvar("cg_fovscale", "1.53");
                    return;
            }
        }
        public void CMD_applysnaps(Entity sender, string snaps)
        {
            switch (snaps)
            {
                case "30":
                    sender.SetClientDvar("snaps", "30");
                    return;
            }
        }

        public void CMD_applyfilmtweakPromod(Entity sender, string ft)
        {
            switch (ft)
            {
                case "0":
                    sender.SetClientDvar("r_filmusetweaks", "0");
                    sender.SetClientDvar("r_filmtweakenable", "0");
                    sender.SetClientDvar("r_colorMap", "1");
                    sender.SetClientDvar("r_specularMap", "1");
                    sender.SetClientDvar("r_normalMap", "1");
                    return;
                case "1":
                    sender.SetClientDvar("r_filmtweakdarktint", "0.65 0.7 0.8");
                    sender.SetClientDvar("r_filmtweakcontrast", "1.3");
                    sender.SetClientDvar("r_filmtweakbrightness", "0.15");
                    sender.SetClientDvar("r_filmtweakdesaturation", "0");
                    sender.SetClientDvar("r_filmusetweaks", "1");
                    sender.SetClientDvar("r_filmtweaklighttint", "1.8 1.8 1.8");
                    sender.SetClientDvar("r_filmtweakenable", "1");
                    return;
                case "2":
                    sender.SetClientDvar("r_filmtweakdarktint", "1.15 1.1 1.3");
                    sender.SetClientDvar("r_filmtweakcontrast", "1.6");
                    sender.SetClientDvar("r_filmtweakbrightness", "0.2");
                    sender.SetClientDvar("r_filmtweakdesaturation", "0");
                    sender.SetClientDvar("r_filmusetweaks", "1");
                    sender.SetClientDvar("r_filmtweaklighttint", "1.35 1.3 1.25");
                    sender.SetClientDvar("r_filmtweakenable", "1");
                    return;
                case "3":
                    sender.SetClientDvar("r_filmtweakdarktint", "0.8 0.8 1.1");
                    sender.SetClientDvar("r_filmtweakcontrast", "2.25");
                    sender.SetClientDvar("r_filmtweakbrightness", "0.48");
                    sender.SetClientDvar("r_filmtweakdesaturation", "0");
                    sender.SetClientDvar("r_filmusetweaks", "1");
                    sender.SetClientDvar("r_filmtweaklighttint", "1 1 1.4");
                    sender.SetClientDvar("r_filmtweakenable", "1");
                    return;
                case "4":
                    sender.SetClientDvar("r_filmtweakdarktint", "1.8 1.8 2");
                    sender.SetClientDvar("r_filmtweakcontrast", "1.25");
                    sender.SetClientDvar("r_filmtweakbrightness", "0.02");
                    sender.SetClientDvar("r_filmtweakdesaturation", "0");
                    sender.SetClientDvar("r_filmusetweaks", "1");
                    sender.SetClientDvar("r_filmtweaklighttint", "0.8 0.8 1");
                    sender.SetClientDvar("r_filmtweakenable", "1");
                    return;
                case "5":
                    sender.SetClientDvar("r_filmtweakdarktint", "1 1 0.8");
                    sender.SetClientDvar("r_filmtweakcontrast", "1.1");
                    sender.SetClientDvar("r_filmtweakbrightness", "0.05");
                    sender.SetClientDvar("r_filmtweakdesaturation", "2");
                    sender.SetClientDvar("r_filmusetweaks", "1");
                    sender.SetClientDvar("r_filmtweaklighttint", "1.5 1.5 1.5");
                    sender.SetClientDvar("r_filmtweakenable", "1");
                    return;
                case "6":
                    sender.SetClientDvar("r_filmtweakdarktint", "1.1 1.1 1.3");
                    sender.SetClientDvar("r_filmtweakcontrast", "1.5");
                    sender.SetClientDvar("r_filmtweakbrightness", "0.255");
                    sender.SetClientDvar("r_filmtweakdesaturation", "0");
                    sender.SetClientDvar("r_filmusetweaks", "1");
                    sender.SetClientDvar("r_filmtweaklighttint", "1.3 1.3 1.3");
                    sender.SetClientDvar("r_filmtweakenable", "1");
                    return;
                case "7":
                    sender.SetClientDvar("r_filmtweakdarktint", "1.7 1.7 2");
                    sender.SetClientDvar("r_filmtweakcontrast", "1");
                    sender.SetClientDvar("r_filmtweakbrightness", "0.125");
                    sender.SetClientDvar("r_filmtweakdesaturation", "0");
                    sender.SetClientDvar("r_filmusetweaks", "1");
                    sender.SetClientDvar("r_filmtweaklighttint", "1.6 1.6 1.8");
                    sender.SetClientDvar("r_filmtweakenable", "1");
                    return;
                case "default":
                    sender.SetClientDvar("r_filmtweakdarktint", "0.7 0.85 1");
                    sender.SetClientDvar("r_filmtweakcontrast", "1.4");
                    sender.SetClientDvar("r_filmtweakdesaturation", "0.2");
                    sender.SetClientDvar("r_filmusetweaks", "0");
                    sender.SetClientDvar("r_filmtweaklighttint", "1.1 1.05 0.85");
                    sender.SetClientDvar("cg_fov", "66");
                    sender.SetClientDvar("cg_scoreboardpingtext", "1");
                    sender.SetClientDvar("waypointIconHeight", "13");
                    sender.SetClientDvar("waypointIconWidth", "13");
                    sender.SetClientDvar("cl_maxpackets", "100");
                    sender.SetClientDvar("r_fog", "0");
                    sender.SetClientDvar("fx_drawclouds", "0");
                    sender.SetClientDvar("r_distortion", "0");
                    sender.SetClientDvar("r_dlightlimit", "0");
                    sender.SetClientDvar("cg_brass", "0");
                    sender.SetClientDvar("snaps", "30");
                    sender.SetClientDvar("com_maxfps", "100");
                    sender.SetClientDvar("clientsideeffects", "0");
                    sender.SetClientDvar("r_filmTweakBrightness", "0.2");
                    return;

            }
        }
        #endregion
        #region MemoryEdit

        public void PROMOD_Breath()
        {
            byte[] array3 = new byte[2]
            {
            144,
            144
            };
            //byte? nullable = null;
            byte?[] array4 = new byte?[20];
            int int_;
            try
            {
                array4[0] = 131;
                array4[1] = null;
                array4[2] = null;
                array4[3] = null;
                array4[4] = null;
                array4[5] = null;
                array4[6] = null;
                array4[7] = 125;
                array4[8] = 10;
                array4[9] = 199;
                array4[10] = null;
                array4[11] = null;
                array4[12] = null;
                array4[13] = null;
                array4[14] = null;
                array4[15] = 0;
                array4[16] = 0;
                array4[17] = 0;
                array4[18] = 0;
                array4[19] = 133;
                byte?[] nullableArray = array4;
                int value = smethod_62(4194304, 4718592, 1, nullableArray) + 7;
                WriteProcessMemory((IntPtr)(-1), (IntPtr)value, array3, (IntPtr)array3.Length, out int_);
            }
            catch (Exception)
            {
            }
            try
            {
                byte[] array5 = new byte[3]
                {
                144,
                144,
                144
                };
                smethod_62(4194304, 4718592, 1, new byte?[13]
                {
                247,
                null,
                null,
                null,
                null,
                null,
                0,
                128,
                0,
                0,
                116,
                17,
                139
                });
                array4 = new byte?[13]
                {
                116,
                7,
                43,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null
                };
                array4[3] = null;
                array4[4] = null;
                array4[5] = null;
                array4[6] = 137;
                array4[7] = null;
                array4[8] = null;
                array4[9] = 131;
                array4[10] = null;
                array4[11] = null;
                array4[12] = 0;
                byte?[] nullableArray2 = array4;
                int value2 = smethod_62(4194304, 4718592, 1, nullableArray2) + 6;
                WriteProcessMemory((IntPtr)(-1), (IntPtr)value2, array5, (IntPtr)array5.Length, out int_);
            }
            catch (Exception)
            {
            }
            try
            {
                byte[] array6 = new byte[2]
                {
                144,
                144
                };
                byte[] array7 = new byte[3]
                {
                144,
                144,
                144
                };
                int value3 = FindMem(new byte?[15]
                {
                116,
                29,
                247,
                null,
                null,
                null,
                null,
                null,
                0,
                0,
                0,
                4,
                116,
                17,
                139
                }, 1, 4194304, 4718592) + 12;
                WriteProcessMemory((IntPtr)(-1), (IntPtr)value3, array6, (IntPtr)array6.Length, out int int_2);
                int value4 = FindMem(new byte?[15]
                {
                116,
                53,
                247,
                null,
                null,
                null,
                null,
                null,
                0,
                0,
                0,
                4,
                116,
                41,
                139
                }, 1, 4194304, 4718592) + 12;
                WriteProcessMemory((IntPtr)(-1), (IntPtr)value4, array6, (IntPtr)array6.Length, out int_2);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }


        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool WriteProcessMemory(IntPtr intptr_0, IntPtr intptr_1, byte[] byte_0, IntPtr intptr_2, out int int_0);

        [DllImport("kernel32.dll")]
        internal static extern bool VirtualProtect(IntPtr intptr_0, IntPtr intptr_1, uint uint_0, out uint uint_1);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr VirtualAlloc(IntPtr lpAddress, UIntPtr dwSize, uint flAllocationType, uint flProtect);

        private unsafe void NoSoundADS()
        {
            int num = FindMem(new byte?[14]
            {
            139,
            null,
            139,
            null,
            null,
            null,
            0,
            0,
            131,
            null,
            22,
            117,
            7,
            184
            }, 1, 4194304, 4718592);
            byte[] array = new byte[22]
            {
            80,
            139,
            64,
            12,
            169,
            60,
            0,
            0,
            0,
            88,
            116,
            3,
            49,
            192,
            195,
            139,
            128,
            100,
            1,
            0,
            0,
            233
            };
            IntPtr value = VirtualAlloc(IntPtr.Zero, (UIntPtr)(ulong)(array.Length + 5), 4096u, 64u);
            VirtualProtect((IntPtr)(num + 2), (IntPtr)5, 64u, out uint uint_);
            for (int i = 0; i < array.Length; i++)
            {
                *(sbyte*)((int)value + i) = (sbyte)array[i];
            }
            int num2 = num + 8 - ((int)value + array.Length + 4);
            *(int*)(void*)(IntPtr)((int)value + array.Length) = num2;
            *(sbyte*)(void*)(IntPtr)(num + 2) = -23;
            int num3 = (int)value - (num + 7);
            *(int*)(void*)(IntPtr)(num + 3) = num3;
            *(sbyte*)(void*)(IntPtr)(num + 7) = -112;
            VirtualProtect((IntPtr)(num + 2), (IntPtr)5, uint_, out uint_);
        }

        private unsafe int FindMem(byte?[] search, int num = 1, int start = 16777216, int end = 63963136)
        {
            try
            {
                int num2 = 0;
                for (int i = start; i < end; i++)
                {
                    int num3 = i;
                    bool flag = false;
                    for (int j = 0; j < search.Length; j++)
                    {
                        if (search[j].HasValue)
                        {
                            int num4 = *(byte*)num3;
                            byte? nullable = search[j];
                            if (num4 != nullable.GetValueOrDefault() || !nullable.HasValue || 1 == 0)
                            {
                                break;
                            }
                        }
                        if (j == search.Length - 1)
                        {
                            if (num == 1)
                            {
                                flag = true;
                            }
                            else
                            {
                                num2++;
                                if (num2 == num)
                                {
                                    flag = true;
                                }
                            }
                        }
                        else
                        {
                            num3++;
                        }
                    }
                    if (flag)
                    {
                        return i;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
            return 0;
        }
        private unsafe static int smethod_62(int p1, int p2, int p3, byte?[] nullableArray1)
        {
            try
            {
                int num = 0;
                for (int i = p1; i < p2; i++)
                {
                    byte* ptr = (byte*)i;
                    bool flag = false;
                    for (int j = 0; j < nullableArray1.Length; j++)
                    {
                        if (nullableArray1[j].HasValue)
                        {
                            int num2 = *ptr;
                            byte? nullable = nullableArray1[j];
                            if (num2 != nullable.GetValueOrDefault() || ((!nullable.HasValue) ? true : false))
                            {
                                break;
                            }
                        }
                        if (j != nullableArray1.Length - 1)
                        {
                            ptr++;
                        }
                        else if (p3 != 1)
                        {
                            num++;
                            if (num == p3)
                            {
                                flag = true;
                            }
                        }
                        else
                        {
                            flag = true;
                        }
                    }
                    if (flag)
                    {
                        return i;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
            return 0;
        }

        #endregion
    }
}
