using System;
using System.Collections.Generic;

namespace LambAdmin
{
    public partial class LAdmin
    {
        public static partial class ConfigValues
        {
            public static bool SmokePatch
            {
                get
                {
                    return bool.Parse(Sett_GetString("settings_smokepatch"));
                }
            }
        }
        public void NORMAL_OnServerStart()
        {
            NORMAL_InitCommands();
        }
        public void NORMAL_InitCommands()
        {
            CommandList.Add(new Command("ft", 1, Command.Behaviour.Normal,
               (sender, arguments, optarg) =>
               {
                   CMD_applyfilmtweakPromod(sender, arguments[0]);
                   WriteChatToPlayer(sender, Command.GetString("ft", "message").Format(new Dictionary<string, string>()
                   {
                        {"<ft>", arguments[0] },
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
            CommandList.Add(new Command("fx", 1, Command.Behaviour.Normal,
                (sender, arguments, optarg) =>
                {
                    bool state = UTILS_ParseBool(arguments[0]);
                    if (state)
                    {
                        sender.SetClientDvar("clientSideEffects", "0");
                        WriteChatToPlayer(sender, Command.GetString("fxr", "message_on"));
                    }
                    else
                    {
                        sender.SetClientDvar("clientSideEffects", "1");
                        WriteChatToPlayer(sender, Command.GetString("fxr", "message_off"));
                    }
                }));
        }
    }
}
