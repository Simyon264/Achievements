using System;
using Exiled.API.Features;
using Exiled.API.Enums;
using System.Collections.Generic;
using MEC;
using SuperSimpleTcp;
using System.Linq;
using PlayerEvents = Exiled.Events.Handlers.Player;

namespace Achievements
{
    public class Counter
    {
        public int bulletsShot = 0;
        public int doorsOpened = 0;
        public int doorsClosed = 0;
        public int damage = 0;
        public int HidDamage = 0;
        public int kills = 0;
        public bool isPinkCandy = false;
    }

    public class TriedMyBest
    {
        public int kills = 0;
        public LeadingTeam leadingTeamNeeded = LeadingTeam.Draw;
    }

    public class Plugin : Plugin<Config>
    {
        public override string Name { get; } = "Achievements";
        public override string Prefix { get; } = "achievements";
        public override string Author { get; } = "Simyon";
        public override Version Version { get; } = new Version(1, 0, 0);
        public override PluginPriority Priority { get; } = PluginPriority.Lowest;

        private static readonly Plugin InstanceValue = new Plugin();
        private Plugin()
        {

        }

        public static Plugin StaticInstance => InstanceValue;

        public Network network = new Network()
        {
            
        };
        
 

        public IEnumerator<float> connection()
        {
            while (true)
            {   
                yield return Timing.WaitForSeconds(Config.ReconnectionInterval);
                network.Connect();
            }
        }

        public IEnumerator<float> massSend()
        {
            while (true)
            {
                yield return Timing.WaitForSeconds(Config.MassSendTimes);
                Dictionary<Player, Counter> copy = new Dictionary<Player, Counter>(massCounts);
                foreach(KeyValuePair<Player, Counter> current in copy)
                {
                    if (current.Value.HidDamage > 0)
                    {
                        network.SendData($"{{ \"id\": \"{current.Key.Sender.SenderId}\", \"type\": \"updateStat\", \"data\": {{ \"name\": \"HidDamage\", \"value\": \"++{current.Value.HidDamage}\" }} }}");
                        yield return Timing.WaitForSeconds(0.05f);
                    }

                    if (current.Value.bulletsShot > 0)
                    {
                        network.SendData($"{{ \"id\": \"{current.Key.Sender.SenderId}\", \"type\": \"updateStat\", \"data\": {{ \"name\": \"bulletsShot\", \"value\": \"++{current.Value.bulletsShot}\" }} }}");
                        yield return Timing.WaitForSeconds(0.05f);
                    }

                    if (current.Value.doorsOpened > 0)
                    {
                        network.SendData($"{{ \"id\": \"{current.Key.Sender.SenderId}\", \"type\": \"updateStat\", \"data\": {{ \"name\": \"doorsOpened\", \"value\": \"++{current.Value.doorsOpened}\" }} }}");
                        yield return Timing.WaitForSeconds(0.05f);

                    }

                    if (current.Value.doorsClosed > 0)
                    {
                        network.SendData($"{{ \"id\": \"{current.Key.Sender.SenderId}\", \"type\": \"updateStat\", \"data\": {{ \"name\": \"doorsClosed\", \"value\": \"++{current.Value.doorsClosed}\" }} }}");
                        yield return Timing.WaitForSeconds(0.05f);

                    }
                    if (current.Value.damage > 0)
                    {
                        network.SendData($"{{ \"id\": \"{current.Key.Sender.SenderId}\", \"type\": \"updateStat\", \"data\": {{ \"name\": \"damage\", \"value\": \"++{current.Value.damage}\" }} }}");
                        yield return Timing.WaitForSeconds(0.05f);

                    }
                    if (current.Value.kills > 0)
                    {
                        if (current.Value.isPinkCandy)
                        {
                            network.SendData($"{{ \"id\": \"{current.Key.Sender.SenderId}\", \"type\": \"updateStat\", \"data\": {{ \"name\": \"pinkCandyKills\", \"value\": \"++{current.Value.kills}\" }} }}");
                            if (current.Value.kills >= 3)
                            {
                                network.SendData($"{{ \"id\": \"{current.Key.Sender.SenderId}\", \"type\": \"giveAchievement\", \"data\": {{ \"achievementId\": 8}} }}");
                            }
                        } else
                        {
                            network.SendData($"{{ \"id\": \"{current.Key.Sender.SenderId}\", \"type\": \"updateStat\", \"data\": {{ \"name\": \"kills\", \"value\": \"++{current.Value.kills}\" }} }}");
                        }
                        yield return Timing.WaitForSeconds(0.05f);
                    }
                }
                copy.Clear();
                massCounts.Clear();
            }
        }

        private static Random random = new Random();
        
        public Dictionary<Player, Counter> massCounts = new Dictionary<Player, Counter>();
        public Dictionary<Player, int> infectCounts = new Dictionary<Player, int>();
        public Dictionary<Player, int> scp173BlackoutKills = new Dictionary<Player, int>();
        public Dictionary<Player, int> pentaKillCounter = new Dictionary<Player, int>();
        public Dictionary<Player, int> proGamerEsacpes = new Dictionary<Player, int>();
        public Dictionary<Player, int> tantrumCounter = new Dictionary<Player, int>();
        public Dictionary<Player, int> paybackCounter = new Dictionary<Player, int>();
        public Dictionary<Player, int> scpKillCount = new Dictionary<Player, int>();
        public Dictionary<Player, int> survivedSpawnwaves = new Dictionary<Player, int>();
        public Dictionary<Player, int> survivedSpawnwavesChaos = new Dictionary<Player, int>();
        public Dictionary<Player, int> colasDrunk = new Dictionary<Player, int>();
        public Dictionary<Player, TriedMyBest> triedMyBest = new Dictionary<Player, TriedMyBest>();
        public Dictionary<Player, int> chaosKills = new Dictionary<Player, int>();
        public Dictionary<Player, int> optixKills = new Dictionary<Player, int>();
        public Dictionary<Player, int> loveYouPapa = new Dictionary<Player, int>();
        public List<Player> revivedRecently = new List<Player>();
        public Dictionary<Player, int> junkieCount = new Dictionary<Player, int>();
        public Dictionary<Player, int> howDidWeGetHereCounter = new Dictionary<Player, int>();
        public Dictionary<Player, Team> howDidWeGetHereLastTeam = new Dictionary<Player, Team>();
        public string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789@_#-abcdefghijklmnopqrstuvwxyz";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public bool IsInChance(int chance)
        {
            if (random == null)
                random = new Random();
            if (random.Next(100) < chance)
            {
                return true;
            }
            return false;
        }

        public void CheckMassCounts(Player player)
        {
            if (!massCounts.ContainsKey(player))
            {
                massCounts.Add(player, new Counter());
            }
        }
        
        public List<CoroutineHandle> coroutineHandles = new List<CoroutineHandle>();

        public override void OnEnabled()
        {
            massCounts.Clear();
            infectCounts.Clear();
            scp173BlackoutKills.Clear();
            pentaKillCounter.Clear();
            proGamerEsacpes.Clear();
            tantrumCounter.Clear();
            paybackCounter.Clear();
            scpKillCount.Clear();
            survivedSpawnwaves.Clear();
            survivedSpawnwavesChaos.Clear();
            colasDrunk.Clear();
            triedMyBest.Clear();
            chaosKills.Clear();
            optixKills.Clear();
            loveYouPapa.Clear();
            junkieCount.Clear();
            howDidWeGetHereCounter.Clear();
            howDidWeGetHereLastTeam.Clear();

            network.client = new SimpleTcpClient($"{Config.Ip}:{Config.Port}")
            {
                Keepalive = new SimpleTcpKeepaliveSettings()
                {
                    EnableTcpKeepAlives = true,
                    TcpKeepAliveRetryCount = 5,
                    TcpKeepAliveInterval = 2,
                    TcpKeepAliveTime = 2
                }
            };

            coroutineHandles.Add(Timing.RunCoroutine(massSend()));
            coroutineHandles.Add(Timing.RunCoroutine(connection()));
            RegisterEvents();

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            UnregisterEvents();

            foreach (CoroutineHandle item in coroutineHandles)
            {
                Timing.KillCoroutines(item);
            }

            base.OnDisabled();
        }

        private void RegisterEvents()
        {
            PlayerEvents.Verified += Eventhandler.OnVerified;
            PlayerEvents.Dying += Eventhandler.Died;
            PlayerEvents.UsedItem += Eventhandler.UsedItem;
            PlayerEvents.Shooting += Eventhandler.BulletsShot;
            PlayerEvents.ChangingRole += Eventhandler.RoleChange;
            Exiled.Events.Handlers.Scp173.PlacingTantrum += Eventhandler.Tantrum;
            PlayerEvents.EnteringFemurBreaker += Eventhandler.Sacrificed;
            PlayerEvents.ActivatingWarheadPanel += Eventhandler.ActivatingNuke;
            PlayerEvents.InteractingDoor += Eventhandler.DoorInteracting;
            PlayerEvents.Hurting += Eventhandler.OnDamage;
            Exiled.Events.Handlers.Scp330.EatenScp330 += Eventhandler.EatenScp330;
            PlayerEvents.ThrowingItem += Eventhandler.ThrowingItem;
            Exiled.Events.Handlers.Scp914.Activating += Eventhandler.Activating914;
            Exiled.Events.Handlers.Server.EndingRound += Eventhandler.EndingRound;
            PlayerEvents.Escaping += Eventhandler.Escaping;
            PlayerEvents.PickingUpItem += Eventhandler.PickUp;
            Exiled.Events.Handlers.Server.RespawningTeam += Eventhandler.Spawned;
            PlayerEvents.EscapingPocketDimension += Eventhandler.EscapingPocket;
            PlayerEvents.Left += Eventhandler.Disconnect;
            Exiled.Events.Handlers.Scp049.FinishingRecall += Eventhandler.Revive;
            PlayerEvents.EnteringPocketDimension += Eventhandler.PutInPocket;
            PlayerEvents.UsingItem += Eventhandler.UsingItem;
        }

        private void UnregisterEvents()
        {
            PlayerEvents.Verified -= Eventhandler.OnVerified;
            PlayerEvents.Dying -= Eventhandler.Died;
            PlayerEvents.UsedItem -= Eventhandler.UsedItem;
            PlayerEvents.Shooting -= Eventhandler.BulletsShot;
            PlayerEvents.ChangingRole -= Eventhandler.RoleChange;
            Exiled.Events.Handlers.Scp173.PlacingTantrum -= Eventhandler.Tantrum;
            PlayerEvents.EnteringFemurBreaker -= Eventhandler.Sacrificed;
            PlayerEvents.ActivatingWarheadPanel -= Eventhandler.ActivatingNuke;
            PlayerEvents.InteractingDoor -= Eventhandler.DoorInteracting;
            PlayerEvents.Hurting -= Eventhandler.OnDamage;
            Exiled.Events.Handlers.Scp330.EatenScp330 -= Eventhandler.EatenScp330;
            PlayerEvents.ThrowingItem -= Eventhandler.ThrowingItem;
            Exiled.Events.Handlers.Scp914.Activating -= Eventhandler.Activating914;
            Exiled.Events.Handlers.Server.EndingRound -= Eventhandler.EndingRound;
            PlayerEvents.Escaping -= Eventhandler.Escaping;
            PlayerEvents.PickingUpItem -= Eventhandler.PickUp;
            Exiled.Events.Handlers.Server.RespawningTeam -= Eventhandler.Spawned;
            PlayerEvents.EscapingPocketDimension -= Eventhandler.EscapingPocket;
            PlayerEvents.Left -= Eventhandler.Disconnect;
            Exiled.Events.Handlers.Scp049.FinishingRecall -= Eventhandler.Revive;
            PlayerEvents.EnteringPocketDimension -= Eventhandler.PutInPocket;
            PlayerEvents.UsingItem -= Eventhandler.UsingItem;
        }
    }
}
