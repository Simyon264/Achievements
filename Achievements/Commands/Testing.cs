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
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    internal class Testing : ICommand
    {
        public string Command { get; } = "actest";
        public string[] Aliases { get; } = new[] { "act" };
        public string Description { get; } = "Test command";
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            string id = ((CommandSender)sender).SenderId;
            Plugin.StaticInstance.network.SendData($"{{ \"id\": \"123\", \"type\": \"achievementCheck\", \"data\": {{ \"id\": 0 }} }}");
            response = "Sent.";
            return true;
        }
    }
}
