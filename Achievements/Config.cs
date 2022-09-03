using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Interfaces;
using Exiled.API.Features;
using Exiled.Events.EventArgs;
using Exiled.API.Enums;

namespace Achievements
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public string Ip { get; set; } = "127.0.0.1";
        public ushort Port { get; set; } = 6969;
        public float ReconnectionInterval { get; set; } = 2;
        public string DiscordLink { get; set; } = "discord.gg/RxzaN3jGeb";
        public float MassSendTimes { get; set; } = 5f;

        public bool Debug { get; set; } = false;
        public Dictionary<int, int> AchievementRewards { get; set; } = new Dictionary<int, int>()
        {
            { 0, 2000 },
            { 1, 10000 },
            { 2, 5000 },
            { 3, 5000 },
            { 4, 1000 },
            { 5, 5000},
            { 6, 1000},
            { 7, 3000},
            { 8, 2000},
            { 9, 4000},
            { 10, 10000},
            { 11, 50},
            { 12, 2000},
            { 13, 5000},
            { 14, 6000},
            { 15, 6000},
            { 16, 1000},
            { 17, 3000},
            { 18, 15000},
            { 19, 100}
        };
    }
}
