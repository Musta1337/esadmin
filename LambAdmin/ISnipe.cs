using System;
using System.Collections.Generic;
using InfinityScript;

namespace LambAdmin
{
    public partial class LAdmin
    {
        public static partial class ConfigValues
        {
            public static bool ISNIPE_MODE
            {
                get
                {
                    return bool.Parse(Sett_GetString("settings_isnipe"));
                }
            }

            public static class ISNIPE_SETTINGS
            {
                public static bool ANTIHARDSCOPE
                {
                    get
                    {
                        return bool.Parse(ISNIPE_Sett_GetString("settings_isnipe_antihardscope"));
                    }
                }
                public static bool ANTIKNIFE
                {
                    get
                    {
                        return bool.Parse(ISNIPE_Sett_GetString("settings_isnipe_antiknife"));
                    }
                }
                public static bool ANTIPLANT
                {
                    get
                    {
                        return bool.Parse(ISNIPE_Sett_GetString("settings_isnipe_antiplant"));
                    }
                }
                public static bool REPLACESECONDARY
                {
                    get
                    {
                        return bool.Parse(ISNIPE_Sett_GetString("settings_isnipe_replacesecondary"));
                    }
                }
                public static bool ANTIFALLDAMAGE
                {
                    get
                    {
                        return bool.Parse(ISNIPE_Sett_GetString("settings_isnipe_antifalldamage"));
                    }
                }
                public static bool ANTIWEAPONHACK
                {
                    get
                    {
                        return bool.Parse(ISNIPE_Sett_GetString("settings_isnipe_antiweaponhack"));
                    }
                }
                public static List<string> AllowedWeapons = new List<string>()
                {
                    "l96a1",
                    "msr",
                    "moab",
                    "briefcase_bomb_mp",
                    "none",
                    "throwingknife",
                    "destructible_car",
                    "barrel_mp",
                    "destructible_toy",
                    "knife",
                    "trophy",
                };
                public static List<string> Hardscopeweapon = new List<string>()
                {
                    "l96a1",
                    "msr",
                    "barrett",
                    "rsass",
                    "dragunov",
                    "as50",
                };
            }
        }

        public void SNIPE_OnServerStart()
        {
            WriteLog.Info("Loading iSnipe Configuration.");
            SNIPE_InitCommands();
            SetupKnife();
            PlayerActuallySpawned += SNIPE_OnPlayerSpawn;
            OnPlayerDamageEvent += SNIPE_PeriodicChecks;
            PlayerConnected += SNIPE_OnPlayerConnect;

            if (ConfigValues.ISNIPE_SETTINGS.ANTIKNIFE)
            {
                DisableKnife();
                WriteLog.Info("Knife has been disabled by default.");
            }

            AfterDelay(5000, () =>
            {
                if (Call<string>("getdvar", "g_gametype") == "infect")
                    EnableKnife();
            });

            WriteLog.Info("iSnipe Configuration loaded.");
        }

        public void SNIPE_OnPlayerSpawn(Entity player)
        {
            CMD_HideBombIconAndSmoke(player);
            CMD_GiveMaxAmmo(player);
            if (ConfigValues.ISNIPE_SETTINGS.REPLACESECONDARY)
                if (player.HasWeapon("stinger_mp"))
                {
                    player.TakeWeapon("stinger_mp");
                    player.GiveWeapon("iw5_44magnum_mp");
                    player.Call("SetWeaponAmmoStock", "iw5_44magnum_mp", "0");
                    player.Call("SetWeaponAmmoClip", "iw5_44magnum_mp", "0");

                    OnInterval(1000, () =>
                    {
                        player.Call("SetWeaponAmmoStock", "iw5_44magnum_mp", "0");
                        player.Call("SetWeaponAmmoClip", "iw5_44magnum_mp", "0");
                        return true;
                    });
                }

            /*
            player.OnInterval(1000, (e2) =>
            {
                if (!e2.IsAlive)
                    return false;
                if (e2.CurrentWeapon.Equals("briefcase_bomb_mp"))
                {
                    e2.TakeWeapon("briefcase_bomb_mp");
                    e2.IPrintLnBold(Lang_GetString("Message_PlantingNotAllowed"));
                    return true;
                }
                return true;
            });
            */
            if (ConfigValues.ISNIPE_SETTINGS.ANTIPLANT)
                player.OnNotify("weapon_change", delegate (Entity Player, Parameter weap)
                {
                    if (weap.ToString() == "briefcase_bomb_mp")
                    {
                        Player.TakeWeapon("briefcase_bomb_mp");
                        Player.IPrintLnBold(Lang_GetString("Message_PlantingNotAllowed"));
                    }
                });
            string weapon = player.CurrentWeapon;
            if (ConfigValues.ISNIPE_SETTINGS.ANTIHARDSCOPE && SNIPE_IsHardscopeWeapon(weapon))
            {
                player.SetField("adscycles", 0);
                player.SetField("letmehardscope", 0);
                player.OnInterval(100, ent =>
                {
                    if (!ent.IsAlive)
                        return false;
                    if (ent.GetField<int>("letmehardscope") == 1)
                        return true;
                    if (Call<string>("getdvar", "g_gametype") == "infect" && ent.GetTeam() != "allies")
                        return true;
                    float ads = ent.Call<float>("playerads");
                    int adscycles = player.GetField<int>("adscycles");
                    if (ads == 1f)
                        adscycles++;
                    else
                        adscycles = 0;

                    if (adscycles > 5)
                    {
                        ent.Call("allowads", false);
                        ent.IPrintLnBold(Lang_GetString("Message_HardscopingNotAllowed"));
                    }

                    if (ent.Call<int>("adsbuttonpressed") == 0 && ads == 0)
                    {
                        ent.Call("allowads", true);
                    }

                    player.SetField("adscycles", adscycles);
                    return true;
                });
            }
        }

        public void SNIPE_PeriodicChecks(Entity player, Entity inflictor, Entity attacker, int damage, int dFlags, string mod, string weapon, Vector3 point, Vector3 dir, string hitLoc)
        {
            if (ConfigValues.ISNIPE_SETTINGS.ANTIFALLDAMAGE && mod == "MOD_FALLING")
            {
                player.Health += damage;
                return;
            }
            if (!attacker.IsPlayer)
                return;
            if (weapon == "iw5_usp45_mp_tactical" && Call<string>("getdvar", "g_gametype") == "infect" && attacker.GetTeam() != "allies")
                return;
            if (ConfigValues.ISNIPE_SETTINGS.ANTIWEAPONHACK && !SNIPE_IsWeaponAllowed(weapon) && !CMDS_IsRekt(attacker))
            {
                try
                {
                    WriteLog.Info("----STARTREPORT----");
                    WriteLog.Info("Bad weapon detected: " + weapon + " at player " + attacker.Name);
                    HaxLog.WriteInfo("----STARTREPORT----");
                    HaxLog.WriteInfo("BAD WEAPON: " + weapon);
                    HaxLog.WriteInfo("Player Info:");
                    HaxLog.WriteInfo(attacker.Name);
                    HaxLog.WriteInfo(attacker.GUID.ToString());
                    HaxLog.WriteInfo(attacker.IP.ToString());
                    HaxLog.WriteInfo(attacker.GetEntityNumber().ToString());
                }
                finally
                {
                    WriteLog.Info("----ENDREPORT----");
                    HaxLog.WriteInfo("----ENDREPORT----");
                    player.Health += damage;
                    CMDS_Rek(attacker);
                    WriteChatToAll(Command.GetString("rek", "message").Format(new Dictionary<string, string>()
                    {
                        {"<target>", attacker.Name},
                        {"<targetf>", attacker.GetFormattedName(database)},
                        {"<issuer>", ConfigValues.ChatPrefix},
                        {"<issuerf>", ConfigValues.ChatPrefix},
                    }));
                }
            }
        }

        public bool SNIPE_IsWeaponAllowed(string weapon)
        {
            foreach (string weap in ConfigValues.ISNIPE_SETTINGS.AllowedWeapons)
            {
                if (weapon.Contains(weap))
                    return true;
            }
            return false;
        }
        public bool SNIPE_IsHardscopeWeapon(string weapon)
        {
            foreach (string weap in ConfigValues.ISNIPE_SETTINGS.Hardscopeweapon)
            {
                if (weapon.Contains(weap))
                    return true;
            }
            return false;
        }

        public void SNIPE_OnPlayerConnect(Entity player)
        {
            player.OnNotify("giveLoadout", (ent) =>
            {
                CMD_GiveMaxAmmo(ent);
            });
            if (Call<string>("getdvar", "g_gametype") == "infect")
                player.OnNotify("giveLoadout", (ent) =>
                {
                    ent.TakeAllWeapons();
                    switch (ent.GetTeam())
                    {
                        case "allies":
                            ent.GiveWeapon("iw5_l96a1_mp_l96a1scope_xmags");
                            ent.SwitchToWeaponImmediate("iw5_l96a1_mp_l96a1scope_xmags");
                            break;
                        default:
                            try
                            {
                                ent.GiveWeapon("iw5_usp45_mp_tactical");
                                ent.Call("clearperks");
                                ent.AfterDelay(100, (e) =>
                                {
                                    e.SwitchToWeaponImmediate("iw5_usp45_mp_tactical");
                                });
                                ent.Call("setweaponammoclip", "iw5_usp45_mp_tactical", 0);
                                ent.Call("setweaponammostock", "iw5_usp45_mp_tactical", 0);
                                ent.Call("SetOffhandSecondaryClass", "specialty_tacticalinsertion");
                                ent.GiveWeapon("specialty_tacticalinsertion");
                            }
                            catch (Exception)
                            {

                            }
                            break;
                    }
                });
        }

        #region COMMANDS

        public void SNIPE_InitCommands()
        {
            // GA
            CommandList.Add(new Command("ga", 0, Command.Behaviour.Normal,
                (sender, arguments, optarg) =>
                {
                    if (Call<string>("getdvar", "g_gametype") == "infect" && sender.GetTeam() == "axis")
                    {
                        WriteChatToPlayer(sender, "I like the way you're thinking, but nope.");
                        return;
                    }
                    CMD_GiveMaxAmmo(sender);
                    WriteChatToPlayer(sender, Command.GetString("ga", "message"));
                }));

            // FT // FILMTWEAK
            CommandList.Add(new Command("ft", 1, Command.Behaviour.Normal,
                (sender, arguments, optarg) =>
                {
                    CMD_applyfilmtweak(sender, arguments[0]);
                    WriteChatToPlayer(sender, Command.GetString("ft", "message").Format(new Dictionary<string, string>()
                    {
                        {"<ft>", arguments[0] },
                    }));
                }));
            // HIDEBOMBICON

            // KNIFE
            CommandList.Add(new Command("knife", 1, Command.Behaviour.Normal,
                (sender, arguments, optarg) =>
                {
                    bool enabled = UTILS_ParseBool(arguments[0]);
                    CMD_knife(enabled);
                    if (enabled)
                        WriteChatToAll(Command.GetString("knife", "message_on"));
                    else
                        WriteChatToAll(Command.GetString("knife", "message_off"));
                }));

            // LETMEHARDSCOPE
            CommandList.Add(new Command("letmehardscope", 1, Command.Behaviour.Normal,
                (sender, arguments, optarg) =>
                {
                    bool state = UTILS_ParseBool(arguments[0]);
                    if (state)
                    {
                        sender.SetField("letmehardscope", 1);
                        WriteChatToPlayer(sender, Command.GetString("letmehardscope", "message_on"));
                    }
                    else
                    {
                        sender.SetField("letmehardscope", 0);
                        WriteChatToPlayer(sender, Command.GetString("letmehardscope", "message_off"));
                    }
                }));
            CommandList.Add(new Command("fx", 1, Command.Behaviour.Normal,
                (sender, arguments, optarg) =>
                {
                    bool state = UTILS_ParseBool(arguments[0]);
                    if (state)
                    {
                        sender.SetClientDvar("fx_draw", "0");
                        sender.SetClientDvar("clientSideEffects", "0");
                        WriteChatToPlayer(sender, Command.GetString("fx", "message_on"));
                    }
                    else
                    {
                        sender.SetClientDvar("fx_draw", "1");
                        sender.SetClientDvar("clientSideEffects", "1");
                        WriteChatToPlayer(sender, Command.GetString("fx", "message_off"));
                    }
                }));
        }

        #region CMDS

        public void CMD_GiveMaxAmmo(Entity player)
        {
            player.Call("giveMaxAmmo", player.CurrentWeapon);
        }

        public void CMD_applyfilmtweak(Entity sender, string ft)
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
                    sender.SetClientDvar("r_filmtweakcontrast", "1.3");
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
                    sender.SetClientDvar("r_filmtweakdarktint", "1 1 2");
                    sender.SetClientDvar("r_filmtweakcontrast", "1.5");
                    sender.SetClientDvar("r_filmtweakbrightness", "0.07");
                    sender.SetClientDvar("r_filmtweakdesaturation", "0");
                    sender.SetClientDvar("r_filmusetweaks", "1");
                    sender.SetClientDvar("r_filmtweaklighttint", "1 1.2 1");
                    sender.SetClientDvar("r_filmtweakenable", "1");
                    return;
                case "6":
                    sender.SetClientDvar("r_filmtweakdarktint", "1.5 1.5 2");
                    sender.SetClientDvar("r_filmtweakcontrast", "1");
                    sender.SetClientDvar("r_filmtweakbrightness", "0.0.4");
                    sender.SetClientDvar("r_filmtweakdesaturation", "0");
                    sender.SetClientDvar("r_filmusetweaks", "1");
                    sender.SetClientDvar("r_filmtweaklighttint", "1.5 1.5 1");
                    sender.SetClientDvar("r_filmtweakenable", "1");
                    return;
                case "7":
                    sender.SetClientDvar("r_specularMap", "2");
                    sender.SetClientDvar("r_normalMap", "0");
                    return;
                case "8":
                    sender.SetClientDvar("cg_drawFPS", "1");
                    sender.SetClientDvar("cg_fovScale", "1.5");
                    return;
                case "9":
                    sender.SetClientDvar("r_debugShader", "1");
                    return;
                case "10":
                    sender.SetClientDvar("r_colorMap", "3");
                    return;
                case "11":
                    sender.SetClientDvar("com_maxfps", "0");
                    sender.SetClientDvar("con_maxfps", "0");
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

        public void CMD_knife(bool state)
        {
            if (state)
                EnableKnife();
            else
                DisableKnife();
        }

        #endregion

        #endregion
    }
}
