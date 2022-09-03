using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Exiled;
using Exiled.API.Features;
using MEC;
using SuperSimpleTcp;
using Newtonsoft.Json;
using GameStore_Exiled.Database;

namespace Achievements
{
    public class Network
    {
        internal bool EventsRegistered = false;
        public SimpleTcpClient client;
        public bool waitingOnReturn = false;
        public List<string> events = new List<string>();
        public CoroutineHandle coroutine;
        public IEnumerator<float> sendLoop()
        {
            while (true)
            {
                if (client.IsConnected)
                {
                    if (!waitingOnReturn)
                    {
                        try
                        {
                            if (events.Count != 0)
                            {
                                client.Send(events[0]);
                                waitingOnReturn = true;
                                events.RemoveAt(0);
                            }
                        }
                        catch (Exception e)
                        {
                            Log.Info($"[NET] [ERROR] Can't send data. {e.Message}");
                        }

                    }
                }
                yield return Timing.WaitForSeconds(0.05f);
            }
        }

        public void Connect()
        {
            if (client.IsConnected) return;
            try
            {
                if (!EventsRegistered)
                {
                    client.Events.Connected += Connected;
                    client.Events.Disconnected += Disconnected;
                    client.Events.DataReceived += DataReceived;
                    coroutine = Timing.RunCoroutine(sendLoop());
                    EventsRegistered = true;
                }

                client.Connect();
            }
            catch (Exception e)
            {
                Log.Warn($"[NET] [ERROR] Can't connect to server. {e.Message}");
            }

        }

        static void Connected(object sender, ConnectionEventArgs e)
        {
            Log.Info($"[NET] [INFO] Connected with {e.IpPort}");
        }

        static void Disconnected(object sender, ConnectionEventArgs e)
        {
            Log.Info($"[NET] [INFO] Disconnected from {e.IpPort}");
        }

        public void DataReceived(object sender, DataReceivedEventArgs e)
        {
            try
            {
                string data = Encoding.UTF8.GetString(e.Data);
                Log.Debug($"[NET] [DATA] [{e.IpPort}] {data}", Plugin.StaticInstance.Config.Debug);
                HandleData(data);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                Log.Info($"Error recieving data. {ex.Message}");
            }
        }

        public void HandleData(string input)
        {
            waitingOnReturn = false;
            dynamic data = JsonConvert.DeserializeObject<dynamic>(input);
            string id = data.id;
            if (id == null)
            {
                Log.Warn($"[NET] [WARN] Recieved data with no ID. DATA: {input}");
                return;
            }
            Player player = Player.Get(id);
            switch (data.type.ToString())
            {
                case "error":
                    switch (data.error.ToString())
                    {
                        case "codeNotNew":
                            Plugin.StaticInstance.network.SendData($"{{ \"id\": \"{id}\", \"type\": \"link\", \"data\": {{ \"code\": \"{Plugin.StaticInstance.RandomString(6)}\" }} }}");
                            break;
                        default:
                            Log.Warn($"[NET] [WARN] Error {data.error} for {id} was not handled.");
                            break;
                    }
                    break;
                case "linkCheckResponse":
                    if (data.isFound.ToString().ToLower() == "true")
                    {
                        player.SendConsoleMessage("Du hast bereits ein Link gestartet.", "red");
                    } else
                    {
                        Plugin.StaticInstance.network.SendData($"{{ \"id\": \"{id}\", \"type\": \"link\", \"data\": {{ \"code\": \"{Plugin.StaticInstance.RandomString(6)}\" }} }}");
                    }
                    break;
                case "linkSuccess":
                    player.SendConsoleMessage($"Link gestartet. Dein Code lautet: {data.code}. Dieser Code läuft in {data.expiresIn} Sekunden ab. Um zu linken, trete den Discord bei {Plugin.StaticInstance.Config.DiscordLink}", "green");
                    break;
                case "achievement":
                    if (data.achievementId.ToString() != "-1")
                    {
                        int acId = int.Parse(data.achievementId.ToString());
                        bool hasReward = Plugin.StaticInstance.Config.AchievementRewards.TryGetValue(acId, out int reward);
                        if (hasReward)
                        {
                            Database.AddBalance(player, reward);
                        }
                        player.Broadcast(5, $"<b><color=#59ff00>Errungenschaft freigeschaltet:\n</color></b> {data.name}", Broadcast.BroadcastFlags.Normal, true);
                        foreach (Player p in Player.List)
                        {
                            if (p.Sender.SenderId != player.Sender.SenderId)
                            {
                                p.Broadcast(5, $"<b><color=#59ff00>{player.Nickname} hat eine Errungenschaft freigeschaltet:\n</color></b> {data.name}", Broadcast.BroadcastFlags.Normal, true);
                            }
                        }
                    }
                    break;
                case "giveAchievementResponse":
                    if (data.wasGiven.ToString().ToLower() == "true")
                    {
                        Plugin.StaticInstance.network.SendData($"{{ \"id\": \"{id}\", \"type\": \"achievementCheck\", \"data\": {{ \"id\": \"{data.acId.ToString()}\" }} }}");
                    }
                    break;
                case "statUpdate":
                    //player.SendConsoleMessage($"Deine Stats wurden aktualisiert", "green");
                    break;
                case "silentAdd":
                    if (int.TryParse(data.amount.ToString(), out int amount))
                    {
                        Database.AddBalanceById(id, amount);
                    }
                    break;
                case "getBal":
                    Plugin.StaticInstance.network.SendData($"{{ \"id\": \"{id}\", \"type\": \"sendAll\", \"data\": {{ \"data\": {{ \"bal\": \"{Database.GetBalanceById(id)}\"}} }} }}");
                    break;
                default:
                    Log.Warn($"[NET] [WARN] Type {data.type} for {id} was not handled.");
                    break;
            }
        }

        public void Shutdown()
        {
            client.Events.Connected -= Connected;
            client.Events.Disconnected -= Disconnected;
            client.Events.DataReceived -= DataReceived;
            Timing.KillCoroutines(coroutine);

            EventsRegistered = false;

            client.Disconnect();
        }

        public bool SendData(string data)
        {
            try
            {
                events.Add(data);
                return true;
            }
            catch (Exception e)
            {
                Log.Info($"[NET] [ERROR] Can't add data. {e.Message}");
                return false;
            }
        }
    }
}
