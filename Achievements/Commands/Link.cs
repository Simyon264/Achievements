using CommandSystem;
using Exiled.API.Features;
using Exiled.CustomItems.API.Features;
using Exiled.Permissions.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Achievements.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    internal class Link : ICommand
    {
        public string Command { get; } = "link";
        public string[] Aliases { get; } = new string[] { };
        public string Description { get; } = "Starte den Link Process.";
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            string id = ((CommandSender)sender).SenderId;
            Plugin.StaticInstance.network.SendData($"{{ \"id\": \"{id}\", \"type\": \"linkCheck\", \"data\": {{}} }}");
            response = "";
            return true;
        }
    }
}
