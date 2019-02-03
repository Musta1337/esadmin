using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfinityScript;
using System.Net;


namespace LambAdmin
{
    public partial class LAdmin
    {
        System.Globalization.CultureInfo Culture = System.Globalization.CultureInfo.InvariantCulture;

        SLOG MainLog = new SLOG("main");
        SLOG PlayersLog = new SLOG("players");
        SLOG CommandsLog = new SLOG("commands");
        SLOG HaxLog = new SLOG("haxor");

        HudElem ESAdminMessage;
        HudElem OnlineAdmins;

        public static partial class ConfigValues
        {
            public static string Version = "v1.0.0.4 Final Build";
            public static string ConfigPath = @"scripts\ESAdmin\";
            public static string PromodPath = @"scripts\ESAdmin\Promod\";
            public static string iSnipePath = @"scripts\ESAdmin\iSnipe\";
            public static string ChatPrefix
            {
                get
                {
                    return Lang_GetString("ChatPrefix");
                }
            }
            public static string ChatPrefixPM
            {
                get
                {
                    return Lang_GetString("ChatPrefixPM");
                }
            }
            public static string ObjectiveText
            {
                get
                {
                    return Lang_GetString("ObjectiveText_Menu");
                }
            }
            public static string G_motd
            {
                get
                {
                    return Lang_GetString("WelcomeText");
                }
            }
            public static string ChatPrefixSPY
            {
                get
                {
                    return Lang_GetString("ChatPrefixSPY");
                }
            }
            public static string ChatPrefixAdminMSG
            {
                get
                {
                    return Lang_GetString("ChatPrefixAdminMSG");
                }
            }
            public static string settings_teamnames_allies
            {
                get
                {
                    return ISNIPE_Sett_GetString("settings_teamnames_allies");
                }
            }
            public static string settings_teamnames_axis
            {
                get
                {
                    return ISNIPE_Sett_GetString("settings_teamnames_axis");
                }
            }
            public static string settings_teamicons_allies
            {
                get
                {
                    return ISNIPE_Sett_GetString("settings_teamicons_allies");
                }
            }
            public static string settings_teamicons_axis
            {
                get
                {
                    return ISNIPE_Sett_GetString("settings_teamicons_axis");
                }
            }
            public static bool DEBUG = false;
            public static List<Data.GameMap> AvailableMaps = Data.StandardMapNames;
            public static class DEBUGOPT
            {
                public static bool PERMSFORALL = false;
            }
        }

        public static class Data
        {
            public static int HWIDOffset = 0x4A30335;
            public static int HWIDDataSize = 0x78688;

            public static int XNADDROffset = 0x049EBD00;
            public static int XNADDRDataSize = 0x78688;

            public static int ClantagOffset = 0x01AC5564;
            public static int ClantagPlayerDataSize = 0x38A4;

            public static List<char> HexChars = new List<char>()
            {
                'a', 'b', 'c', 'd', 'e', 'f', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
            };

            public static Dictionary<string, string> Colors = new Dictionary<string, string>()
            {
                {"^1", "red"},
                {"^2", "green"},
                {"^3", "yellow"},
                {"^4", "blue"},
                {"^5", "lightblue"},
                {"^6", "purple"},
                {"^7", "white"},
                {"^8", "defmapcolor"},
                {"^9", "grey"},
                {"^0", "black"},
                {"^;", "yaleblue"},
                {"^:", "orange"}
            };
            public static List<GameMap> StandardMapNames = new List<GameMap>()
            {
                new GameMap("Dome", "mp_dome"),
                new GameMap("Mission", "mp_bravo"),
                new GameMap("Lockdown", "mp_alpha"),
                new GameMap("Bootleg", "mp_bootleg"),
                new GameMap("Hardhat", "mp_hardhat"),
                new GameMap("Bakaara", "mp_mogadishu"),
                new GameMap("Arkaden", "mp_plaza2"),
                new GameMap("Carbon", "mp_carbon"),
                new GameMap("Fallen", "mp_lambeth"),
                new GameMap("Outpost", "mp_radar"),
                new GameMap("Downturn", "mp_exchange"),
                new GameMap("Interchange", "mp_interchange"),
                new GameMap("Resistance", "mp_paris"),
                new GameMap("Seatown", "mp_seatown"),
                new GameMap("Village", "mp_village"),
                new GameMap("Underground", "mp_underground"),
            };

            public static List<GameMap> DLCMapNames = new List<GameMap>()
            {
                new GameMap("Piazza", "mp_italy"),
                new GameMap("Liberation", "mp_park"),
                new GameMap("Blackbox", "mp_morningwood"),
                new GameMap("Overwatch", "mp_overwatch"),
                new GameMap("Foundation", "mp_cement"),
                new GameMap("Sanctuary", "mp_meteora"),
                new GameMap("Oasis", "mp_qadeem"),
                new GameMap("Boardwalk", "mp_boardwalk"),
                new GameMap("Parish", "mp_nola"),
                new GameMap("Off Shore", "mp_roughneck"),
                new GameMap("Decomission", "mp_shipbreaker"),
                new GameMap("Gulch", "mp_moab"),
                new GameMap("Terminal", "mp_terminal_cls"),
                new GameMap("Aground", "mp_aground_ss"),
                new GameMap("Vortex", "mp_six_ss"),
                new GameMap("Lookout", "mp_restrepo_ss"),
                new GameMap("Getaway", "mp_hillside_ss"),
                new GameMap("Erosion", "mp_courtyard_ss"),
                new GameMap("U-Turn", "mp_burn_ss"),
            };
            public static List<GameMap> AllMapNames = StandardMapNames.Concat(DLCMapNames).ToList();
            public static List<string> TeamNames = new List<string>()
            {
                "axis", "allies", "spectator"
            };

            public class GameMap
            {
                public string DisplayName, ConsoleName;
                public List<string> Aliases = new List<string>();

                public GameMap(string DisplayName, string ConsoleName, string[] Aliases = null)
                {
                    if (Aliases != null)
                        this.Aliases = this.Aliases.Concat(Aliases).ToList();
                    this.DisplayName = DisplayName;
                    this.ConsoleName = ConsoleName;
                    this.Aliases.Add(ConsoleName);
                    this.Aliases.Add(DisplayName);
                }
            }
        }

        public static class WriteLog
        {
            public static void Info(string message)
            {
                Log.Write(LogLevel.Info, message);
            }

            public static void Error(string message)
            {
                Log.Write(LogLevel.Error, message);
            }

            public static void Warning(string message)
            {
                Log.Write(LogLevel.Warning, message);
            }

            public static void Debug(string message)
            {
                if (ConfigValues.DEBUG)
                    Log.Write(LogLevel.Debug, message);
            }
        }

        public static class Mem
        {
            public static unsafe string ReadString(int address, int maxlen = 0)
            {
                string ret = "";
                maxlen = (maxlen == 0) ? int.MaxValue : maxlen;
                for (; address < address + maxlen && *(byte*)address != 0; address++)
                {
                    ret += Encoding.ASCII.GetString(new byte[] { *(byte*)address });
                }
                return ret;
            }

            public static unsafe void WriteString(int address, string str)
            {
                byte[] strarr = Encoding.ASCII.GetBytes(str);
                foreach (byte ch in strarr)
                {
                    *(byte*)address = ch;
                    address++;
                }
                *(byte*)address = 0;
            }
        }

        public class SLOG
        {
            string path = ConfigValues.ConfigPath + @"Logs\";
            string filepath;
            bool notify;

            public SLOG(string filename, bool NotifyIfFileExists = false)
            {
                if (!System.IO.Directory.Exists(ConfigValues.ConfigPath + @"Logs"))
                    System.IO.Directory.CreateDirectory(ConfigValues.ConfigPath + @"Logs");
                path += filename;
                notify = NotifyIfFileExists;
            }

            private void CheckFile()
            {
                filepath = path + " " + DateTime.Now.ToString("yyyy MM dd") + ".log";
                if (!System.IO.File.Exists(filepath))
                    System.IO.File.WriteAllLines(filepath, new string[]
                    {
                        "---- LOG FILE CREATED ----",
                    });
                if (notify)
                    System.IO.File.AppendAllLines(filepath, new string[]
                    {
                        "---- INSTANCE CREATED ----",
                    });
            }

            public void WriteInfo(string message)
            {
                WriteMsg("INFO", message);
            }

            public void WriteError(string message)
            {
                WriteMsg("ERROR", message);
            }

            public void WriteWarning(string message)
            {
                WriteMsg("WARNING", message);
            }
            public void WriteMsg(string prefix, string message)
            {
                CheckFile();
                using (System.IO.StreamWriter file = System.IO.File.AppendText(filepath))
                {
                    file.WriteLine(DateTime.Now.TimeOfDay.ToString() + " [" + prefix + "] " + message);
                }
            }
        }

        public class Announcer
        {
            List<string> message_list;
            public int message_interval;
            string name;

            public Announcer(string announcername, List<string> messages, int interval = 40000)
            {
                message_interval = interval;
                message_list = messages;
                name = announcername;
            }

            public string SpitMessage()
            {
                int currentmsg = GetStep();
                string messagetobespit = message_list[currentmsg];
                if (++currentmsg >= message_list.Count)
                    currentmsg = 0;
                SetStep(currentmsg);
                return messagetobespit;
            }

            public int GetStep()
            {
                if (System.IO.File.Exists(ConfigValues.ConfigPath + @"Utils\internal\announcers\" + name + ".txt"))
                    return int.Parse(System.IO.File.ReadAllText(ConfigValues.ConfigPath + @"Utils\internal\announcers\" + name + ".txt"));
                return 0;
            }

            public void SetStep(int step)
            {
                System.IO.File.WriteAllLines(ConfigValues.ConfigPath + @"Utils\internal\announcers\" + name + ".txt", new string[] { step.ToString() });
            }
        }

        public class HWID
        {
            public string Value
            {
                get; private set;
            }

            public HWID(Entity player)
            {
                if (player == null || !player.IsPlayer)
                {
                    Value = null;
                    return;
                }
                int address = Data.HWIDDataSize * player.GetEntityNumber() + Data.HWIDOffset;
                string formattedhwid = "";
                unsafe
                {
                    for (int i = 0; i < 12; i++)
                    {
                        if (i % 4 == 0 && i != 0)
                            formattedhwid += "-";
                        formattedhwid += (*(byte*)(address + i)).ToString("x2");
                    }
                }
                Value = formattedhwid;
            }

            private HWID(string value)
            {
                Value = value;
            }

            public bool IsBadHWID()
            {
                return string.IsNullOrWhiteSpace(Value) || Value == "00000000-00000000-00000000";
            }

            public override string ToString()
            {
                return Value;
            }

            public static bool TryParse(string str, out HWID parsedhwid)
            {
                str = str.ToLowerInvariant();
                if (str.Length != 26)
                {
                    parsedhwid = new HWID((string)null);
                    return false;
                }
                for (int i = 0; i < 26; i++)
                {
                    if (i == 8 || i == 17)
                    {
                        if (str[i] != '-')
                        {
                            parsedhwid = new HWID((string)null);
                            return false;
                        }
                        continue;
                    }
                    if (!str[i].IsHex())
                    {
                        parsedhwid = new HWID((string)null);
                        return false;
                    }
                }
                parsedhwid = new HWID(str);
                return true;
            }
        }

        public class XNADDR
        {
            public string Value
            {
                get; private set;
            }

            public XNADDR(Entity player)
            {
                if (player == null || !player.IsPlayer)
                {
                    Value = null;
                    return;
                }
                string connectionstring = Mem.ReadString(Data.XNADDRDataSize * player.GetEntityNumber() + Data.XNADDROffset, Data.XNADDRDataSize);
                string[] parts = connectionstring.Split('\\');
                for (int i = 1; i < parts.Length; i++)
                {
                    if (parts[i - 1] == "xnaddr")
                    {
                        Value = parts[i].Substring(0, 12);
                        return;
                    }
                }
                Value = null;
            }

            public override string ToString()
            {
                return Value;
            }

            public static bool TryParse(string str, out XNADDR parsedxnaddr)
            {
                str = str.ToLowerInvariant();
                if (str.Length != 12)
                {
                    parsedxnaddr = null;
                    return false;
                }
                for (int i = 0; i < 12; i++)
                {
                    if (!str[i].IsHex())
                    {
                        parsedxnaddr = null;
                        return false;
                    }
                }
                parsedxnaddr = null;
                return true;
            }
        }

        public class PlayerInfo
        {
            internal string player_ip = null;
            internal long? player_guid = null;
            internal HWID player_hwid = null;

            public PlayerInfo(Entity player)
            {
                player_ip = player.IP.Address.ToString();
                player_guid = player.GUID;
                player_hwid = player.GetHWID();
            }

            private PlayerInfo()
            {

            }

            public bool MatchesAND(PlayerInfo B)
            {
                if (B.isNull() || isNull())
                    return false;
                return
                    (B.player_ip == null || player_ip == B.player_ip) &&
                    (B.player_guid == null || player_guid.Value == B.player_guid.Value) &&
                    (B.player_hwid == null || player_hwid.Value == B.player_hwid.Value);
            }

            public bool MatchesOR(PlayerInfo B)
            {
                if (B.isNull() || isNull())
                    return false;
                if ((player_ip != null && B.player_ip != null) && player_ip == B.player_ip)
                    return true;
                if ((player_guid != null && B.player_guid != null) && player_guid.Value == B.player_guid.Value)
                    return true;
                if ((player_hwid != null && B.player_hwid != null) && player_hwid.Value == B.player_hwid.Value)
                    return true;
                return false;
            }

            public void addIdentifier(string identifier)
            {
                long result;
                if (long.TryParse(identifier, out result))
                {
                    player_guid = result;
                    return;
                }
                IPAddress address;
                if (IPAddress.TryParse(identifier, out address))
                {
                    player_ip = address.ToString();
                    return;
                }
                HWID possibleHWID;
                if (HWID.TryParse(identifier, out possibleHWID))
                {
                    player_hwid = possibleHWID;
                    return;
                }
            }

            public static PlayerInfo Parse(string str)
            {
                PlayerInfo pi = new PlayerInfo();
                string[] parts = str.Split(',');
                foreach (string part in parts)
                    pi.addIdentifier(part);
                return pi;
            }

            public string getIdentifiers()
            {
                List<string> identifiers = new List<string>();
                if (player_hwid != null)
                    identifiers.Add(player_hwid.ToString());
                return string.Join("", identifiers);
            }

            public bool isNull()
            {
                return player_ip == null && !player_guid.HasValue && player_hwid == null;
            }

            public override string ToString()
            {
                return getIdentifiers();
            }

            public string GetGUIDString()
            {
                if (player_guid.HasValue)
                    return player_guid.Value.ToString();
                return null;
            }

            public string GetIPString()
            {
                return player_ip;
            }

            //CHANGE
            public string GetHWIDString()
            {
                return player_hwid != null ? player_hwid.Value : null;
            }

            //CHANGE
            public static PlayerInfo CommonIdentifiers(PlayerInfo A, PlayerInfo B)
            {
                PlayerInfo commoninfo = new PlayerInfo();
                if (B.isNull() || A.isNull())
                    return null;
                if (!string.IsNullOrWhiteSpace(A.GetIPString()))
                {
                    if (!string.IsNullOrWhiteSpace(B.GetIPString()) && A.GetIPString() == B.GetIPString())
                        commoninfo.player_ip = A.player_ip;
                }
                if (!string.IsNullOrWhiteSpace(A.GetGUIDString()))
                {
                    if (!string.IsNullOrWhiteSpace(B.GetGUIDString()) && A.GetGUIDString() == B.GetGUIDString())
                        commoninfo.player_guid = A.player_guid;
                }
                if (!string.IsNullOrWhiteSpace(A.GetHWIDString()))
                {
                    if (!string.IsNullOrWhiteSpace(B.GetHWIDString()) && A.GetHWIDString() == B.GetHWIDString())
                        commoninfo.player_hwid = A.player_hwid;
                }
                return commoninfo;
            }
        }

        public void WriteChatToAll(string message)
        {
            Utilities.RawSayAll(ConfigValues.ChatPrefix + " " + message);
        }

        public void WriteChatToPlayer(Entity player, string message)
        {
            Utilities.RawSayTo(player, ConfigValues.ChatPrefixPM + " " + message);
        }

        public void WriteChatToAllMultiline(IEnumerable<string> messages, int delay = 500)
        {
            IEnumerator<string> enrt = messages.GetEnumerator();

            OnInterval(delay, () =>
            {
                if (enrt.MoveNext())
                {
                    WriteChatToAll(enrt.Current);
                    return true;
                }
                else
                {
                    enrt.Dispose();
                    return false;
                }
            });
        }

        public void WriteChatToPlayerMultiline(Entity player, IEnumerable<string> messages, int delay = 500)
        {
            IEnumerator<string> enrt = messages.GetEnumerator();

            OnInterval(delay, () =>
            {
                if (enrt.MoveNext())
                {
                    WriteChatToPlayer(player, enrt.Current);
                    return true;
                }
                else
                {
                    enrt.Dispose();
                    return false;
                }
            });
        }

        public void WriteChatToPlayerCondensed(Entity player, IEnumerable<string> messages, int delay = 1000, int condenselevel = 40, string separator = ", ")
        {
            WriteChatToPlayerMultiline(player, messages.Condense(condenselevel, separator), delay);
        }

        public void WriteChatSpyToPlayer(Entity player, string message)
        {
            Utilities.RawSayTo(player, ConfigValues.ChatPrefixSPY + " " + message);
        }

        public void WriteChatAdmToPlayer(Entity player, string message)
        {
            Utilities.RawSayTo(player, ConfigValues.ChatPrefixAdminMSG + message);
        }

        public void ChangeMap(string devmapname)
        {
            ExecuteCommand("map " + devmapname);
        }

        public List<Entity> FindPlayers(string identifier)
        {
            if (identifier.StartsWith("#"))
            {
                try
                {
                    int number = int.Parse(identifier.Substring(1));
                    Entity ent = Entity.GetEntity(number);
                    if (number >= 0 && number < 18)
                    {
                        foreach (Entity player in Players)
                        {
                            if (player.GetEntityNumber() == number)
                                return new List<Entity>() { ent };
                        }
                    }
                    return new List<Entity>();
                }
                catch (Exception)
                {
                }
            }
            identifier = identifier.ToLowerInvariant();
            return (from player in Players
                    where player.Name.ToLowerInvariant().Contains(identifier)
                    select player).ToList();
        }

        public Entity FindSinglePlayer(string identifier)
        {
            List<Entity> players = FindPlayers(identifier);
            if (players.Count != 1)
                return null;
            return players[0];
        }

        public List<Data.GameMap> FindMaps(string identifier)
        {
            identifier = identifier.ToLowerInvariant();
            List<Data.GameMap> foundmaps = new List<Data.GameMap>();
            foreach (var map in ConfigValues.AvailableMaps)
            {
                bool found = false;
                foreach (var name in map.Aliases)
                {
                    if (name.ToLowerInvariant().Contains(identifier))
                    {
                        found = true;
                        break;
                    }
                }

                if (found)
                    foundmaps.Add(map);
            }
            return foundmaps;
        }

        public Data.GameMap FindSingleMap(string identifier)
        {
            List<Data.GameMap> maps = FindMaps(identifier);
            if (maps.Count != 1)
                return null;
            return maps[0];
        }

        public static bool ParseCommand(string CommandToBeParsed, int ArgumentAmount, out string[] arguments, out string optionalarguments)
        {
            CommandToBeParsed = CommandToBeParsed.TrimEnd(' ');
            List<string> list = new List<string>();
            if (CommandToBeParsed.IndexOf(' ') == -1)
            {
                arguments = new string[0];
                optionalarguments = null;
                if (ArgumentAmount == 0)
                    return true;
                else
                    return false;
            }
            CommandToBeParsed = CommandToBeParsed.Substring(CommandToBeParsed.IndexOf(' ') + 1);
            while (list.Count < ArgumentAmount)
            {
                int length = CommandToBeParsed.IndexOf(' ');
                if (length == -1)
                {
                    list.Add(CommandToBeParsed);
                    CommandToBeParsed = null;
                }
                else
                {
                    if (CommandToBeParsed == null)
                    {
                        arguments = new string[0];
                        optionalarguments = null;
                        return false;
                    }
                    list.Add(CommandToBeParsed.Substring(0, length));
                    CommandToBeParsed = CommandToBeParsed.Substring(CommandToBeParsed.IndexOf(' ') + 1);
                }
            }
            arguments = list.ToArray();
            optionalarguments = CommandToBeParsed;
            return true;
        }

        public IEnumerable<Entity> GetEntities()
        {
            for (int i = 0; i < 2048; i++)
                yield return Entity.GetEntity(i);
        }

        public void UTILS_OnPlayerConnect(Entity player)
        {
            //check if bad name
            foreach (string identifier in System.IO.File.ReadAllLines(ConfigValues.ConfigPath + @"Utils\badnames.txt"))
                if (player.Name == identifier)
                {
                    CMD_tmpban(player, "^1Piss off, hacker.");
                    WriteChatToAll(Command.GetString("tmpban", "message").Format(new Dictionary<string, string>()
                    {
                        {"<target>", "^:" + player.Name },
                        {"<targetf>", "^:" + player.GetFormattedName(database) },
                        {"<issuer>", ConfigValues.ChatPrefix },
                        {"<issuerf>", ConfigValues.ChatPrefix },
                        {"<reason>", "Piss off, hacker." },
                    }));
                    return;
                }

            //check if bad clantag
            foreach (string identifier in System.IO.File.ReadAllLines(ConfigValues.ConfigPath + @"Utils\badclantags.txt"))
                if (player.GetClantag() == identifier)
                {
                    CMD_tmpban(player, "^1Piss off, hacker.");
                    WriteChatToAll(Command.GetString("tmpban", "message").Format(new Dictionary<string, string>()
                    {
                        {"<target>", "^:" + player.Name },
                        {"<targetf>", "^:" + player.GetFormattedName(database) },
                        {"<issuer>", ConfigValues.ChatPrefix },
                        {"<issuerf>", ConfigValues.ChatPrefix },
                        {"<reason>", "Piss off, hacker." },
                    }));
                    return;
                }

            //check if bad xnaddr
            if (player.GetXNADDR() == null || string.IsNullOrEmpty(player.GetXNADDR().ToString()))
            {
                ExecuteCommand("dropclient " + player.EntRef + " \"Cool story bro.\"");
                return;
            }

            //check if EO Unbanner
            string rawplayerhwid = player.GetHWIDRaw();
            if (System.Text.RegularExpressions.Regex.Matches(rawplayerhwid, "adde").Count >= 3)
            {
                WriteChatToAll(Command.GetString("ban", "message").Format(new Dictionary<string, string>()
                    {
                        {"<target>", "^:" + player.Name },
                        {"<targetf>", "^:" + player.GetFormattedName(database) },
                        {"<issuer>", ConfigValues.ChatPrefix },
                        {"<issuerf>", ConfigValues.ChatPrefix },
                        {"<reason>", "Piss off, hacker." },
                    }));
                ExecuteCommand("dropclient " + player.EntRef + " \"^3BAI SCRUB :D\"");
                return;
            }
            player.SetClientDvar("cg_objectiveText", (string)ConfigValues.ObjectiveText);
            //log player name
            if (System.IO.File.Exists(string.Format(ConfigValues.ConfigPath + @"Utils\playerlogs\{0}.txt", player.HWID)))
            {
                List<string> lines = System.IO.File.ReadAllLines(string.Format(ConfigValues.ConfigPath + @"Utils\playerlogs\{0}.txt", player.HWID)).ToList();
                if (!lines.Contains(player.Name))
                    lines.Add(player.Name);
                System.IO.File.WriteAllLines(string.Format(ConfigValues.ConfigPath + @"Utils\playerlogs\{0}.txt", player.HWID), lines);
            }
            else
                System.IO.File.WriteAllLines(string.Format(ConfigValues.ConfigPath + @"Utils\playerlogs\{0}.txt", player.HWID), new string[] { player.Name });

            //set team names and other dvar
            UTILS_SetClientDvars(player);
            //ClientDvar, Default!

            //check for names ingame
            if (player.Name.Length < 3)
            {
                CMD_kick(player, "Name must be at least 3 characters long.");
                return;
            }
            string invariantname = player.Name.ToLowerInvariant();
            foreach (Entity scrub in Players)
            {
                string invariantscrub = scrub.Name.ToLowerInvariant();
                if (player.EntRef != scrub.EntRef && (invariantscrub.Contains(invariantname) || invariantname.Contains(invariantscrub)))
                {
                    CMD_kick(player, "Your name is containing another user's/contained by another user");
                    return;
                }
            }
        }
        public void UTILS_SetClientConnectDvar(Entity player)
        {
            player.SetClientDvar("motd", (string)ConfigValues.G_motd);
            player.SetClientDvar("sv_network_fps", "200");
            player.SetClientDvar("sys_lockThreads", "all");
            player.SetClientDvar("com_maxFrameTime", "1000");
            player.SetClientDvar("g_motd", (string)ConfigValues.G_motd);
            player.SetClientDvar("rate", "25000");
            if (bool.Parse(Sett_GetString("settings_enable_autofpsunlock")))
                {
                player.SetClientDvar("com_maxfps", "0");
                player.SetClientDvar("con_maxfps", "0");
            };
            if (ConfigValues.SmokePatch)
            {
                player.SetClientDvar("fx_draw", "1");
                player.SetClientDvar("r_specular", "0");
            }
            player.SpawnedPlayer += () =>
            {
                UTILS_SpawnDvar(player);
            };
        }
        public void UTILS_SpawnDvar(Entity player)
        {
            player.SetClientDvar("cg_objectiveText", (string)ConfigValues.ObjectiveText);
        }

        private void hud_counter(Entity player)
        {
            HudElem fontString1 = HudElem.CreateFontString(player, "hudbig", 0.6f);
            fontString1.SetPoint("DOWNRIGHT", "DOWNRIGHT", -19, 60);
            fontString1.SetText("^2Allies^0:");
            fontString1.HideWhenInMenu = true;
            HudElem fontString2 = HudElem.CreateFontString(player, "hudbig", 0.6f);
            fontString2.SetPoint("DOWNRIGHT", "DOWNRIGHT", -19, 80);
            fontString2.SetText("^1Enemies^0:");
            fontString2.HideWhenInMenu = true;
            HudElem hudElem2 = HudElem.CreateFontString(player, "hudbig", 0.6f);
            hudElem2.SetPoint("DOWNRIGHT", "DOWNRIGHT", -8, 60);
            hudElem2.HideWhenInMenu = true;
            HudElem hudElem3 = HudElem.CreateFontString(player, "hudbig", 0.6f);
            hudElem3.SetPoint("DOWNRIGHT", "DOWNRIGHT", -8, 80);
            hudElem3.HideWhenInMenu = true;
            this.OnInterval(50, (Func<bool>)(() =>
            {
                string str1 = (string)player.GetField<string>("sessionteam");
                string str2 = ((int)this.Call<int>("getteamplayersalive", new Parameter[1]
                    {
                        "axis"
                    })).ToString();
                string str3 = ((int)this.Call<int>("getteamplayersalive", new Parameter[1]
                    {
                        "allies"
                    })).ToString();
                hudElem2.SetText(str1.Equals("allies") ? str3 : str2);
                hudElem3.SetText(str1.Equals("allies") ? str2 : str3);
                return true;
            }));
            HudElem fontString3 = HudElem.CreateFontString(player, "hudbig", 0.45f);
            fontString3.SetPoint("BOTTOM", "BOTTOM", 0, -5);
            fontString3.SetText("^0| ^;Name^0 | ^:" + player.Name + " ^0| ^;ID^0 | ^:" + player.EntRef + " ^0|");
            fontString3.HideWhenInMenu = true;
        }

        public void UTILS_OnServerStart()
        {
            PlayerConnected += UTILS_OnPlayerConnect;
            PlayerConnecting += UTILS_OnPlayerConnecting;
            OnPlayerKilledEvent += UTILS_BetterBalance;
            //ServerDvars
            UTILS_SetServerDvars();

            if (!System.IO.Directory.Exists(ConfigValues.ConfigPath + @"Utils"))
                System.IO.Directory.CreateDirectory(ConfigValues.ConfigPath + @"Utils");

            if (!System.IO.File.Exists(ConfigValues.ConfigPath + @"Utils\badnames.txt"))
                System.IO.File.WriteAllLines(ConfigValues.ConfigPath + @"Utils\badnames.txt", new string[]
                    {
                        "thisguyhax.",
                        "MW2Player",
                    });

            if (!System.IO.File.Exists(ConfigValues.ConfigPath + @"Utils\badclantags.txt"))
                System.IO.File.WriteAllLines(ConfigValues.ConfigPath + @"Utils\badclantags.txt", new string[]
                    {
                        "hkClan",
                    });
            if (!System.IO.File.Exists(ConfigValues.ConfigPath + @"Utils\announcer.txt"))
                System.IO.File.WriteAllLines(ConfigValues.ConfigPath + @"Utils\announcer.txt", new string[]
                    {
                        "^1Welcome to ^:ES ^3Promod srver^0.",
                    });

            if (System.IO.File.Exists(ConfigValues.ConfigPath + @"Utils\announcer.txt"))
            {
                Announcer announcer = new Announcer("default", System.IO.File.ReadAllLines(ConfigValues.ConfigPath + @"Utils\announcer.txt").ToList());
                OnInterval(announcer.message_interval, () =>
                {
                    WriteChatToAll(announcer.SpitMessage());
                    return true;
                });
            }

            if (!System.IO.Directory.Exists(ConfigValues.ConfigPath + @"Utils\playerlogs"))
                System.IO.Directory.CreateDirectory(ConfigValues.ConfigPath + @"Utils\playerlogs");

            if (!System.IO.Directory.Exists(ConfigValues.ConfigPath + @"Utils\internal\announcers"))
                System.IO.Directory.CreateDirectory(ConfigValues.ConfigPath + @"Utils\internal\announcers");

            // TEAM NAMES
            foreach (Entity player in Players)
            {
                UTILS_SetClientDvars(player);
            }

            // ESADMIN HUDELEM
            if (bool.Parse(Sett_GetString("settings_showversion")))
            {
                ESAdminMessage = HudElem.CreateServerFontString("hudsmall", 0.5f);
                ESAdminMessage.SetPoint("bottom", "bottom", 0, 0);
                ESAdminMessage.SetText("ES Admin " + ConfigValues.Version);
                ESAdminMessage.Color = new Vector3(1f, 0f, 0f);
                ESAdminMessage.Foreground = true;
                ESAdminMessage.HideWhenInMenu = true;
            }

            // ADMINS HUDELEM
            if (bool.Parse(Sett_GetString("settings_adminshudelem")))
            {
                OnlineAdmins = HudElem.CreateServerFontString("hudsmall", 0.5f);
                OnlineAdmins.SetPoint("top", "top", 0, 5);
                OnlineAdmins.Foreground = true;
                OnlineAdmins.Archived = false;
                OnlineAdmins.HideWhenInMenu = true;
                OnInterval(5000, () =>
                {
                    OnlineAdmins.SetText("^1Online Admins:\n" + string.Join("\n", database.GetAdminsString(Players).Condense(100, "^7, ")));
                    return true;
                });
            }

            // UNFREEZE PLAYERS ON GAME END
            if (bool.Parse(Sett_GetString("settings_unfreezeongameend")))
                OnGameEnded += UTILS_OnGameEnded;

            // BETTER BALANCE
            Call("setdvarifuninitialized", "betterbalance", bool.Parse(Sett_GetString("settings_betterbalance_enable")) ? "1" : "0");

            // AUTOFPSUNLOCK

            //DLCMAPS
            if (bool.Parse(Sett_GetString("settings_enable_dlcmaps")))
                ConfigValues.AvailableMaps = Data.AllMapNames;

        }
        //Server Dvars to make performance better.
        public void UTILS_SetServerDvars()
        {
            Function.Call("setdvar", "sv_network_fps", "200");
            Function.Call("setdvar", "sv_fps", "120");
            Function.Call("setdvar", "sv_hugeSnapshotSize", "10000");
            Function.Call("setdvar", "sv_hugeSnapshotDelay", "100");
            Function.Call("setdvar", "sv_pingDegradation", "0");
            Function.Call("setdvar", "sv_pingDegradationLimit", "9999");
            Function.Call("setdvar", "sv_acceptableRateThrottle", "9999");
            Function.Call("setdvar", "sv_newRateThrottling", "2");
            Function.Call("setdvar", "sv_minPingClamp", "50");
            Function.Call("setdvar", "sv_cumulThinkTime", "1000");
            Function.Call("setdvar", "sys_lockThreads", "all");
            WriteLog.Info("Server Dvar loaded.");
        }

        public void UTILS_BetterBalance(Entity player, Entity inflictor, Entity attacker, int damage, string mod, string weapon, Vector3 dir, string hitLoc)
        {
            if (Call<string>("getdvar", "betterbalance") == "0" || Call<string>("getdvar", "g_gametype") == "infect")
                return;
            int axis = 0, allies = 0;
            UTILS_GetTeamPlayers(out axis, out allies);
            switch (player.GetTeam())
            {
                case "axis":
                    if (axis - allies > 1)
                    {
                        player.SetField("sessionteam", "allies");
                        player.Notify("menuresponse", "team_marinesopfor", "allies");
                        WriteChatToAll(Sett_GetString("settings_betterbalance_message").Format(new Dictionary<string, string>()
                        {
                            {"<player>", player.Name},
                            {"<playerf>", player.GetFormattedName(database)},
                        }));
                    }
                    return;
                case "allies":
                    if (allies - axis > 1)
                    {
                        player.SetField("sessionteam", "axis");
                        player.Notify("menuresponse", "team_marinesopfor", "axis");
                        WriteChatToAll(Sett_GetString("settings_betterbalance_message").Format(new Dictionary<string, string>()
                        {
                            {"<player>", player.Name},
                            {"<playerf>", player.GetFormattedName(database)},
                        }));
                    }
                    return;
            }
        }

        public void UTILS_OnGameEnded()
        {
            AfterDelay(1100, () =>
            {
                foreach (Entity player in Players)
                    if (!CMDS_IsRekt(player))
                        player.Call("freezecontrols", false);
            });
        }

        public void UTILS_OnPlayerConnecting(Entity player)
        {
            if (player.GetClantag().Contains(Encoding.ASCII.GetString(new byte[] { 0x5E, 0x02 })))
                ExecuteCommand("dropclient " + player.GetEntityNumber() + " \"Get out.\"");
            UTILS_SetClientConnectDvar(player);
        }

        public bool UTILS_ParseBool(string message)
        {
            message = message.ToLowerInvariant().Trim();
            if (message == "y" || message == "ye" || message == "yes" || message == "on" || message == "true" || message == "1")
                return true;
            return false;
        }

        public bool UTILS_ParseTimeSpan(string message, out TimeSpan timespan)
        {
            timespan = TimeSpan.Zero;
            try
            {
                string[] parts = message.Split(',');
                foreach (string part in parts)
                {
                    int time = int.Parse(part.Substring(0, part.Length - 1));
                    if (time == 0)
                        return false;
                    if (part.EndsWith("h"))
                        timespan += new TimeSpan(time, 0, 0);
                    if (part.EndsWith("m"))
                        timespan += new TimeSpan(0, time, 0);
                    if (part.EndsWith("s"))
                        timespan += new TimeSpan(0, 0, time);
                    return false;
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public string UTILS_GetDvar(string dvar)
        {
            return Call<string>("getdvar", dvar);
        }

        public void UTILS_SetDvar(string dvar, string value)
        {
            Call<string>("setdvar", dvar, value);
        }

        public void ExecuteCommand(string command)
        {
            Utilities.ExecuteCommand(command);
        }

        public void UTILS_SetClientDvars(Entity player)
        {
            if (ConfigValues.ISNIPE_MODE)
            {
                player.SetClientDvar("g_TeamName_Allies", ConfigValues.settings_teamnames_allies);
                player.SetClientDvar("g_TeamName_Axis", ConfigValues.settings_teamnames_axis);
                player.SetClientDvar("g_TeamIcon_Allies", ConfigValues.settings_teamicons_allies);
                player.SetClientDvar("g_TeamIcon_Axis", ConfigValues.settings_teamicons_axis);
            }
        }

        public void UTILS_GetTeamPlayers(out int axis, out int allies)
        {
            axis = 0;
            allies = 0;
            foreach (Entity player in Players)
            {
                string team = player.GetTeam();
                switch (team)
                {
                    case "axis":
                        axis++;
                        break;
                    case "allies":
                        allies++;
                        break;
                }
            }
        }
    }

    public static partial class Extensions
    {
        public static string RemoveColors(this string message)
        {
            foreach (string color in LAdmin.Data.Colors.Keys)
                message = message.Replace(color, "");
            return message;
        }

        public static void LogTo(this string message, params LAdmin.SLOG[] logs)
        {
            foreach (LAdmin.SLOG log in logs)
            {
                log.WriteInfo(message);
            }
        }

        public static void IPrintLnBold(this Entity player, string message)
        {
            player.Call("iprintlnbold", message);
        }

        public static void IPrintLn(this Entity player, string message)
        {
            player.Call("iprintln", message);
        }

        public static int GetEntityNumber(this Entity player)
        {
            return player.Call<int>("getentitynumber");
        }

        public static void Suicide(this Entity player)
        {
            player.Call("suicide");
        }

        public static string GetTeam(this Entity player)
        {
            return player.GetField<string>("sessionteam");
        }

        public static bool IsSpectating(this Entity player)
        {
            return player.GetTeam() == "spectator";
        }

        public static string Format(this string str, Dictionary<string, string> format)
        {
            foreach (KeyValuePair<string, string> pair in format)
                str = str.Replace(pair.Key, pair.Value);
            return str;
        }

        public static TValue GetValue<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key)
        {
            TValue value;
            if (dict.TryGetValue(key, out value))
                return value;
            throw new ArgumentOutOfRangeException();
        }

        public static IEnumerable<string> Condense(this IEnumerable<string> erbl, int condenselevel = 40, string separator = ", ")
        {
            using (IEnumerator<string> enrt = erbl.GetEnumerator())
            {

                if (!enrt.MoveNext())
                    return erbl;

                List<string> lines = new List<string>();

                string line = enrt.Current;
                while (enrt.MoveNext())
                {
                    if ((line + separator + enrt.Current).RemoveColors().Length > condenselevel)
                    {
                        lines.Add(line);
                        line = enrt.Current;
                        continue;
                    }
                    line += separator + enrt.Current;
                }
                lines.Add(line);
                return lines.ToArray();
            }
        }

        public static LAdmin.HWID GetHWID(this Entity player)
        {
            return new LAdmin.HWID(player);
        }

        public static string GetHWIDRaw(this Entity player)
        {
            int address = LAdmin.Data.HWIDDataSize * player.GetEntityNumber() + LAdmin.Data.HWIDOffset;
            string formattedhwid = "";
            unsafe
            {
                for (int i = 0; i < 12; i++)
                {
                    formattedhwid += (*(byte*)(address + i)).ToString("x2");
                }
            }
            return formattedhwid;
        }

        public static string GetClantag(this Entity player)
        {
            if (player == null || !player.IsPlayer)
                return null;
            int address = LAdmin.Data.ClantagPlayerDataSize * player.GetEntityNumber() + LAdmin.Data.ClantagOffset;
            return LAdmin.Mem.ReadString(address, 8);
        }

        public static void SetClantag(this Entity player, string clantag)
        {
            if (player == null || !player.IsPlayer || clantag.Length > 7)
                return;
            int address = LAdmin.Data.ClantagPlayerDataSize * player.GetEntityNumber() + LAdmin.Data.ClantagOffset;
            unsafe
            {
                for (int i = 0; i < clantag.Length; i++)
                {
                    *(byte*)(address + i) = (byte)clantag[i];
                }
                *(byte*)(address + clantag.Length) = 0;
            }
        }

        public static bool IsHex(this char ch)
        {
            return LAdmin.Data.HexChars.Contains(ch);
        }

        public static LAdmin.XNADDR GetXNADDR(this Entity player)
        {
            return new LAdmin.XNADDR(player);
        }
    }
}
