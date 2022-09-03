using System;
using System.Collections.Generic;
using System.Linq;
using SCPRebalance;
using Exiled.API.Features;
using Exiled.Events.EventArgs;
using InventorySystem.Items.Usables.Scp330;
using MEC;
using Exiled.API.Features.Roles;
using Exiled.API.Enums;

namespace Achievements
{
    public class Eventhandler
    {
        public static void OnVerified(VerifiedEventArgs ev)
        {
            if (ev.Player.DoNotTrack)
            {
                Plugin.StaticInstance.network.SendData($"{{ \"id\": \"{ev.Player.Sender.SenderId}\", \"type\": \"resetStats\", \"data\": {{ }} }}");
                ev.Player.SendConsoleMessage("Deine Stats wurden zurückgesetz durch DNT.", "red");
            } else
            {
                Plugin.StaticInstance.network.SendData($"{{ \"id\": \"{ev.Player.Sender.SenderId}\", \"type\": \"updateStat\", \"data\": {{ \"name\": \"id\", \"value\": \"{ev.Player.Sender.SenderId}\" }} }}");
            }
        }

        public static void Disconnect(LeftEventArgs ev)
        {
            if (ev.Player.DoNotTrack)
            {
                Plugin.StaticInstance.network.SendData($"{{ \"id\": \"{ev.Player.Sender.SenderId}\", \"type\": \"updateStat\", \"data\": {{ \"name\": \"id\", \"value\": \"{ev.Player.Sender.SenderId}\" }} }}");
            }
        }
        public static void EndingRound(EndingRoundEventArgs ev)
        {
            if (ev.IsRoundEnded)
            {
                Plugin.StaticInstance.network.SendData($"{{ \"id\": \"0\", \"type\": \"win\", \"data\": {{ \"winner\": \"{ev.LeadingTeam}\" }} }}");
                bool awardedScp055 = false;
                foreach (Player player in Player.List)
                {
                    if (Plugin.StaticInstance.triedMyBest.ContainsKey(player))
                    {
                        if (Plugin.StaticInstance.triedMyBest[player].kills >= 5)
                        {
                            if (ev.LeadingTeam != Plugin.StaticInstance.triedMyBest[player].leadingTeamNeeded)
                            {
                                Plugin.StaticInstance.network.SendData($"{{ \"id\": \"{player.Sender.SenderId}\", \"type\": \"giveAchievement\", \"data\": {{ \"achievementId\": 17}} }}");
                            }
                        }
                    }
                    if (player.IsScp)
                    {
                        if (player.Health <= 30)
                        {
                            Plugin.StaticInstance.network.SendData($"{{ \"id\": \"{player.Sender.SenderId}\", \"type\": \"giveAchievement\", \"data\": {{ \"achievementId\": 5}} }}");
                        }
                    }
                    if (awardedScp055 == false)
                    {
                        if (Plugin.StaticInstance.IsInChance(1))
                        {
                            Plugin.StaticInstance.network.SendData($"{{ \"id\": \"{player.Sender.SenderId}\", \"type\": \"giveAchievement\", \"data\": {{ \"achievementId\": 26}} }}");
                            awardedScp055 = true;
                        }
                    }
                }
            }
        }

        public static void Died(DyingEventArgs ev)
        {
            Plugin.StaticInstance.network.SendData($"{{ \"id\": \"{ev.Target.Sender.SenderId}\", \"type\": \"updateStat\", \"data\": {{ \"name\": \"deaths\", \"value\": \"++1\" }} }}");
            if (ev.Killer != null)
            {
                if (Plugin.StaticInstance.howDidWeGetHereCounter.ContainsKey(ev.Target))
                {
                    Plugin.StaticInstance.howDidWeGetHereCounter.Remove(ev.Target);
                }
                if (Plugin.StaticInstance.howDidWeGetHereLastTeam.ContainsKey(ev.Target))
                {
                    Plugin.StaticInstance.howDidWeGetHereLastTeam.Remove(ev.Target);
                }
                if (Plugin.StaticInstance.revivedRecently.Contains(ev.Target))
                {
                    if (ev.Target != ev.Killer)
                    {
                        Plugin.StaticInstance.network.SendData($"{{ \"id\": \"{ev.Target.Sender.SenderId}\", \"type\": \"giveAchievement\", \"data\": {{ \"achievementId\": 23}} }}");
                    }
                }
                if (Plugin.StaticInstance.loveYouPapa.ContainsKey(ev.Target))
                {
                    Plugin.StaticInstance.loveYouPapa.Remove(ev.Target);
                }
                if (Plugin.StaticInstance.triedMyBest.ContainsKey(ev.Killer))
                {
                    Plugin.StaticInstance.triedMyBest[ev.Killer].kills++;
                }
                Plugin.StaticInstance.CheckMassCounts(ev.Killer);
                Plugin.StaticInstance.CheckMassCounts(ev.Target);
                if (ev.Killer.RawUserId == "76561198325732380")
                {
                    Plugin. StaticInstance.network.SendData($"{{ \"id\": \"{ev.Target.Sender.SenderId}\", \"type\": \"updateStat\", \"data\": {{ \"name\": \"gotKilledByOptix\", \"value\": \"++1\" }} }}");
                }
                if (ev.Target.RawUserId == "76561198325732380")
                {
                    Plugin.StaticInstance.network.SendData($"{{ \"id\": \"{ev.Killer.Sender.SenderId}\", \"type\": \"updateStat\", \"data\": {{ \"name\": \"optixKilled\", \"value\": \"++1\" }} }}");
                    if (!Plugin.StaticInstance.optixKills.ContainsKey(ev.Killer))
                    {
                        Plugin.StaticInstance.optixKills.Add(ev.Killer, 1);
                    } else
                    {
                        Plugin.StaticInstance.optixKills[ev.Killer]++;
                    }
                    if (Plugin.StaticInstance.optixKills[ev.Killer] >= 3)
                    {
                        Plugin.StaticInstance.network.SendData($"{{ \"id\": \"{ev.Killer.Sender.SenderId}\", \"type\": \"giveAchievement\", \"data\": {{ \"achievementId\": 19}} }}");
                    }
                }
                if (ev.Target.IsScp)
                {
                    if (ev.Target.Role.Type != RoleType.Scp0492)
                    {
                        if (!Plugin.StaticInstance.scpKillCount.ContainsKey(ev.Killer))
                        {
                            Plugin.StaticInstance.scpKillCount.Add(ev.Killer, 1);
                        } else
                        {
                            Plugin.StaticInstance.scpKillCount[ev.Killer]++;
                        }
                        if (Plugin.StaticInstance.scpKillCount[ev.Killer] >= 3)
                        {
                            Plugin.StaticInstance.network.SendData($"{{ \"id\": \"{ev.Killer.Sender.SenderId}\", \"type\": \"giveAchievement\", \"data\": {{ \"achievementId\": 13}} }}");
                        }
                    }
                    Plugin.StaticInstance.network.SendData($"{{ \"id\": \"{ev.Killer.Sender.SenderId}\", \"type\": \"updateStat\", \"data\": {{ \"name\": \"scpsKilled\", \"value\": \"++1\" }} }}");
                }
                Plugin.StaticInstance.massCounts[ev.Killer].kills += 1;

                if (ev.Killer.Role.Type == RoleType.Scp173)
                {
                    if (SCPRebalance.Plugin.StaticInstance.EventActive)
                    {
                        if (!Plugin.StaticInstance.scp173BlackoutKills.ContainsKey(ev.Killer))
                        {
                            Plugin.StaticInstance.scp173BlackoutKills.Add(ev.Killer, 1);
                            Timing.CallDelayed(30f, () =>
                            {
                                Plugin.StaticInstance.scp173BlackoutKills.Remove(ev.Killer);
                            });
                        }
                        else
                        {
                            Plugin.StaticInstance.scp173BlackoutKills[ev.Killer]++;
                        }
                        if (Plugin.StaticInstance.scp173BlackoutKills[ev.Killer] >= 3)
                        {
                            Plugin.StaticInstance.network.SendData($"{{ \"id\": \"{ev.Killer.Sender.SenderId}\", \"type\": \"giveAchievement\", \"data\": {{ \"achievementId\": 2}} }}");
                        }
                    }
                }
                if (ev.Killer.Role.Team == Team.SCP)
                {
                    if (!Plugin.StaticInstance.pentaKillCounter.ContainsKey(ev.Killer))
                    {
                        Plugin.StaticInstance.pentaKillCounter.Add(ev.Killer, 1);
                        Timing.CallDelayed(25f, () =>
                        {
                            Plugin.StaticInstance.pentaKillCounter.Remove(ev.Killer);
                        });
                    } else
                    {
                        Plugin.StaticInstance.pentaKillCounter[ev.Killer]++;
                    }
                    if (Plugin.StaticInstance.pentaKillCounter[ev.Killer] >= 5)
                    {
                        Plugin.StaticInstance.network.SendData($"{{ \"id\": \"{ev.Killer.Sender.SenderId}\", \"type\": \"giveAchievement\", \"data\": {{ \"achievementId\": 3}} }}");
                    }
                }
                if (ev.Killer.Role.Type == RoleType.ClassD && ev.Target.Role.Type == RoleType.Scientist)
                {
                    if (!Plugin.StaticInstance.paybackCounter.ContainsKey(ev.Killer))
                    {
                        Plugin.StaticInstance.paybackCounter.Add(ev.Killer, 1);
                    } else
                    {
                        Plugin.StaticInstance.paybackCounter[ev.Killer]++;
                    }
                    if (Plugin.StaticInstance.paybackCounter[ev.Killer] >= 3)
                    {
                        Plugin.StaticInstance.network.SendData($"{{ \"id\": \"{ev.Killer.Sender.SenderId}\", \"type\": \"giveAchievement\", \"data\": {{ \"achievementId\": 7}} }}");
                    }
                }
                if (ev.Killer.Role.Team == Team.CHI && ev.Target.Role.Team == Team.MTF)
                {
                    if (!Plugin.StaticInstance.chaosKills.ContainsKey(ev.Killer))
                    {
                        Plugin.StaticInstance.chaosKills.Add(ev.Killer, 1);
                    } else
                    {
                        Plugin.StaticInstance.chaosKills[ev.Killer]++;
                    }
                    if (Plugin.StaticInstance.chaosKills[ev.Killer] >= 10)
                    {
                        Plugin.StaticInstance.network.SendData($"{{ \"id\": \"{ev.Killer.Sender.SenderId}\", \"type\": \"giveAchievement\", \"data\": {{ \"achievementId\": 18}} }}");
                    }
                }
                if (ev.Target.Role.Team == Team.CHI)
                {
                    if (Plugin.StaticInstance.survivedSpawnwavesChaos.ContainsKey(ev.Target))
                    {
                        Plugin.StaticInstance.survivedSpawnwavesChaos.Remove(ev.Target);
                    }
                }
                if (Plugin.StaticInstance.proGamerEsacpes.ContainsKey(ev.Target))
                {
                    Plugin.StaticInstance.proGamerEsacpes.Remove(ev.Target);
                }
                if (Plugin.StaticInstance.colasDrunk.ContainsKey(ev.Target))
                {
                    Plugin.StaticInstance.colasDrunk.Remove(ev.Target);
                }
                if (ev.Killer != ev.Target)
                {
                    if (ev.Target.RemoteAdminAccess)
                    {
                        Plugin.StaticInstance.network.SendData($"{{ \"id\": \"{ev.Killer.Sender.SenderId}\", \"type\": \"giveAchievement\", \"data\": {{ \"achievementId\": 27}} }}");
                    }
                }
            }
        }

        public static void UsingItem(UsingItemEventArgs ev)
        {
            if (ev.Item.Type == ItemType.Painkillers)
            {
                if (Plugin.StaticInstance.junkieCount.ContainsKey(ev.Player))
                {
                    Plugin.StaticInstance.junkieCount[ev.Player]++;
                    if (Plugin.StaticInstance.junkieCount[ev.Player] >= 10)
                    {
                        Plugin.StaticInstance.network.SendData($"{{ \"id\": \"{ev.Player.Sender.SenderId}\", \"type\": \"giveAchievement\", \"data\": {{ \"achievementId\": 24}} }}");
                    }
                }
                else
                {
                    Plugin.StaticInstance.junkieCount.Add(ev.Player, 1);
                }
            }
        }

        public static void UsedItem(UsedItemEventArgs ev)
        {
            Plugin.StaticInstance.network.SendData($"{{ \"id\": \"{ev.Player.Sender.SenderId}\", \"type\": \"updateStat\", \"data\": {{ \"name\": \"itemsUsed\", \"value\": \"++1\" }} }}");
            if (ev.Item.IsScp)
            {
                Plugin.StaticInstance.network.SendData($"{{ \"id\": \"{ev.Player.Sender.SenderId}\", \"type\": \"updateStat\", \"data\": {{ \"name\": \"scpItemsUsed\", \"value\": \"++1\" }} }}");
            }
            if (ev.Item.Type == ItemType.SCP207)
            {
                if (!Plugin.StaticInstance.colasDrunk.ContainsKey(ev.Player))
                {
                    Plugin.StaticInstance.colasDrunk.Add(ev.Player, 1);
                } else
                {
                    Plugin.StaticInstance.colasDrunk[ev.Player]++;
                }
                if (Plugin.StaticInstance.colasDrunk[ev.Player] >= 4)
                {
                    Plugin.StaticInstance.network.SendData($"{{ \"id\": \"{ev.Player.Sender.SenderId}\", \"type\": \"giveAchievement\", \"data\": {{ \"achievementId\": 16}} }}");
                }
                Plugin.StaticInstance.network.SendData($"{{ \"id\": \"{ev.Player.Sender.SenderId}\", \"type\": \"updateStat\", \"data\": {{ \"name\": \"cokesDrunk\", \"value\": \"++1\" }} }}");
            }
        }

        public static void PutInPocket(EnteringPocketDimensionEventArgs ev)
        {
            if (ev.Player.CurrentRoom.Type == Exiled.API.Enums.RoomType.Hcz106)
            {
                Plugin.StaticInstance.network.SendData($"{{ \"id\": \"{ev.Scp106.Sender.SenderId}\", \"type\": \"giveAchievement\", \"data\": {{ \"achievementId\": 22}} }}");
            }
        }

        public static void ThrowingItem(ThrowingItemEventArgs ev)
        {
            if (ev.Item.Type == ItemType.GrenadeFlash || ev.Item.Type == ItemType.GrenadeHE)
            {
                Plugin.StaticInstance.network.SendData($"{{ \"id\": \"{ev.Player.Sender.SenderId}\", \"type\": \"updateStat\", \"data\": {{ \"name\": \"grenadesThrown\", \"value\": \"++1\" }} }}");
            }
        }

        public static void EatenScp330(EatenScp330EventArgs ev)
        {
            Plugin.StaticInstance.CheckMassCounts(ev.Player);
            if (ev.Candy.Kind == CandyKindID.Pink)
            {
                Plugin.StaticInstance.massCounts[ev.Player].isPinkCandy = true;
            }
        }

        public static void BulletsShot(ShootingEventArgs ev)
        {
            if (ev.IsAllowed)
            {
                Plugin.StaticInstance.CheckMassCounts(ev.Shooter);
                Plugin.StaticInstance.massCounts[ev.Shooter].bulletsShot++;
            }
        }

        public static void EscapingPocket(EscapingPocketDimensionEventArgs ev)
        {
            if (!Plugin.StaticInstance.proGamerEsacpes.ContainsKey(ev.Player))
            {
                Plugin.StaticInstance.proGamerEsacpes.Add(ev.Player, 1);
            } else
            {
                Plugin.StaticInstance.proGamerEsacpes[ev.Player]++;
            }
            if (Plugin.StaticInstance.proGamerEsacpes[ev.Player] >= 3)
            {
                Plugin.StaticInstance.network.SendData($"{{ \"id\": \"{ev.Player.Sender.SenderId}\", \"type\": \"giveAchievement\", \"data\": {{ \"achievementId\": 4}} }}");
            }
        }

        public static void Revive(FinishingRecallEventArgs ev)
        {
            Plugin.StaticInstance.revivedRecently.Add(ev.Scp049);
            Timing.CallDelayed(10f, () =>
            {
                Plugin.StaticInstance.revivedRecently.Remove(ev.Scp049);
            });
        }

        public static void Spawned(RespawningTeamEventArgs ev)
        {
            List<Player> zombieList = Player.List.Where(x => x.Role.Type == RoleType.Scp0492).ToList();
            foreach (var item in zombieList)
            {
                if (Plugin.StaticInstance.loveYouPapa.ContainsKey(item))
                {
                    Plugin.StaticInstance.loveYouPapa[item]++;
                    if (Plugin.StaticInstance.loveYouPapa[item] >= 3)
                    {
                        Plugin.StaticInstance.network.SendData($"{{ \"id\": \"{item.Sender.SenderId}\", \"type\": \"giveAchievement\", \"data\": {{ \"achievementId\": 21}} }}");
                    }
                }
            }
            List<Player> guardList = Player.List.Where(x => x.Role.Type == RoleType.FacilityGuard).ToList();
            foreach (var item in guardList)
            {
                if (!Plugin.StaticInstance.survivedSpawnwaves.ContainsKey(item))
                {
                    Plugin.StaticInstance.survivedSpawnwaves.Add(item, 1);
                } else
                {
                    Plugin.StaticInstance.survivedSpawnwaves[item]++;
                }
                if (Plugin.StaticInstance.survivedSpawnwaves[item] >= 3)
                { 
                    Plugin.StaticInstance.network.SendData($"{{ \"id\": \"{item.Sender.SenderId}\", \"type\": \"giveAchievement\", \"data\": {{ \"achievementId\": 14}} }}");
                }
            }
            if (ev.NextKnownTeam == Respawning.SpawnableTeamType.NineTailedFox)
            {
                List<Player> chaos = Player.List.Where(x => x.Role.Team == Team.CHI).ToList();
                foreach (var item in chaos)
                {
                    if (!Plugin.StaticInstance.survivedSpawnwavesChaos.ContainsKey(item))
                    {
                        Plugin.StaticInstance.survivedSpawnwavesChaos.Add(item, 1);
                    }
                    else
                    {
                        Plugin.StaticInstance.survivedSpawnwavesChaos[item]++;
                    }
                    if (Plugin.StaticInstance.survivedSpawnwavesChaos[item] >= 2)
                    {
                        Plugin.StaticInstance.network.SendData($"{{ \"id\": \"{item.Sender.SenderId}\", \"type\": \"giveAchievement\", \"data\": {{ \"achievementId\": 15}} }}");
                    }
                }
            }
        }

        public static void OnDamage(HurtingEventArgs ev)
        {
            if (ev.Attacker != null)
            {
                Plugin.StaticInstance.CheckMassCounts(ev.Attacker);
                if (ev.Attacker.CurrentItem != null && ev.Attacker.CurrentItem.Type == ItemType.MicroHID)
                {
                    Plugin.StaticInstance.massCounts[ev.Attacker].HidDamage += (int)Math.Round(ev.Amount);
                }
                else
                {
                    Plugin.StaticInstance.massCounts[ev.Attacker].damage += (int)Math.Round(ev.Amount);
                }
                if (ev.Attacker.Role.Type == RoleType.Scp0492)
                {
                    if (!Plugin.StaticInstance.infectCounts.ContainsKey(ev.Attacker))
                    {
                        Plugin.StaticInstance.infectCounts.Add(ev.Attacker, 1);
                    } else
                    {
                        Plugin.StaticInstance.infectCounts[ev.Attacker]++;
                    }
                    if (Plugin.StaticInstance.infectCounts[ev.Attacker] >= 10)
                    {
                        Plugin.StaticInstance.network.SendData($"{{ \"id\": \"{ev.Attacker.Sender.SenderId}\", \"type\": \"giveAchievement\", \"data\": {{ \"achievementId\": 1}} }}");
                    }
                }
            }
        }

        public static void RoleChange(ChangingRoleEventArgs ev)
        {
            Team lastTeam = Team.TUT;
            if (Plugin.StaticInstance.howDidWeGetHereLastTeam.ContainsKey(ev.Player))
            {
                lastTeam = Plugin.StaticInstance.howDidWeGetHereLastTeam[ev.Player];
            }
            
            switch (ev.NewRole)
            {
                case RoleType.NtfSpecialist:
                case RoleType.NtfSergeant:
                case RoleType.NtfCaptain:
                case RoleType.NtfPrivate:
                    Plugin.StaticInstance.network.SendData($"{{ \"id\": \"{ev.Player.Sender.SenderId}\", \"type\": \"updateStat\", \"data\": {{ \"name\": \"NtfSpawns\", \"value\": \"++1\" }} }}");
                    if (Plugin.StaticInstance.howDidWeGetHereLastTeam.ContainsKey(ev.Player))
                    {
                        Plugin.StaticInstance.howDidWeGetHereLastTeam[ev.Player] = Team.MTF;
                    }
                    else
                    {
                        Plugin.StaticInstance.howDidWeGetHereLastTeam.Add(ev.Player, Team.MTF);
                    }
                    break;
                case RoleType.ChaosConscript:
                case RoleType.ChaosRifleman:
                case RoleType.ChaosRepressor:
                case RoleType.ChaosMarauder:
                    Plugin.StaticInstance.network.SendData($"{{ \"id\": \"{ev.Player.Sender.SenderId}\", \"type\": \"updateStat\", \"data\": {{ \"name\": \"chaosSpawns\", \"value\": \"++1\" }} }}");
                    if (Plugin.StaticInstance.howDidWeGetHereLastTeam.ContainsKey(ev.Player))
                    {
                        Plugin.StaticInstance.howDidWeGetHereLastTeam[ev.Player] = Team.CHI;
                    }
                    else
                    {
                        Plugin.StaticInstance.howDidWeGetHereLastTeam.Add(ev.Player, Team.CHI);
                    }
                    break;
                case RoleType.Scp0492:
                    Plugin.StaticInstance.loveYouPapa.Add(ev.Player, 0);
                    int zombieCount = 0;
                    List<Player> scp049List = new List<Player>();
                    foreach (Player player in Player.List)
                    {
                        if (player.Role.Type == RoleType.Scp0492)
                        {
                            zombieCount++;
                        }
                        if (player.Role.Type == RoleType.Scp049)
                        {
                            scp049List.Add(player);
                        }
                    }
                    if (zombieCount >= 10)
                    {
                        Plugin.StaticInstance.network.SendData($"{{ \"id\": \"{scp049List.First().Sender.SenderId}\", \"type\": \"giveAchievement\", \"data\": {{ \"achievementId\": 0}} }}");
                    }
                    break;
                case RoleType.ClassD:
                    if (!Plugin.StaticInstance.triedMyBest.ContainsKey(ev.Player)) {
                        Plugin.StaticInstance.triedMyBest.Add(ev.Player, new TriedMyBest()
                        {
                            kills = 0,
                            leadingTeamNeeded = Exiled.API.Enums.LeadingTeam.ChaosInsurgency
                        });
                    }
                    break;
                case RoleType.FacilityGuard:
                case RoleType.Scientist:
                    if (!Plugin.StaticInstance.triedMyBest.ContainsKey(ev.Player))
                    {
                        Plugin.StaticInstance.triedMyBest.Add(ev.Player, new TriedMyBest()
                        {
                            kills = 0,
                            leadingTeamNeeded = Exiled.API.Enums.LeadingTeam.FacilityForces
                        });
                    }
                    break;
                case RoleType.Scp079:
                    Timing.RunCoroutine(Scp079Level(ev.Player));
                    break;
            }
            if (lastTeam != Team.TUT)
            {
                if (lastTeam != Plugin.StaticInstance.howDidWeGetHereLastTeam[ev.Player])
                {
                    if (Plugin.StaticInstance.howDidWeGetHereCounter.ContainsKey(ev.Player))
                    {
                        Plugin.StaticInstance.howDidWeGetHereCounter[ev.Player]++;
                    }
                    else
                    {
                        Plugin.StaticInstance.howDidWeGetHereCounter.Add(ev.Player, 1);
                    }
                    if (Plugin.StaticInstance.howDidWeGetHereCounter[ev.Player] >= 3)
                    {
                        Plugin.StaticInstance.network.SendData($"{{ \"id\": \"{ev.Player.Sender.SenderId}\", \"type\": \"giveAchievement\", \"data\": {{ \"achievementId\": 25}} }}");
                    }
                }
            }
        }

        private static IEnumerator<float> Scp079Level(Player player)
        {
            int time = 0;
            bool isScp079 = true;
            while (isScp079)
            {
                if (player.Role.Is(out Scp079Role scp079))
                {
                    yield return Timing.WaitForSeconds(1f);
                    time++;
                    if (scp079.Level == 4)
                    {
                        if (time < 600)
                        {
                            Plugin.StaticInstance.network.SendData($"{{ \"id\": \"{player.Sender.SenderId}\", \"type\": \"giveAchievement\", \"data\": {{ \"achievementId\": 20}} }}");
                        }
                        isScp079 = false;
                    }
                } else
                {
                    isScp079 = false;
                }
            }
        }
        
        public static void Tantrum(PlacingTantrumEventArgs ev)
        {
            Plugin.StaticInstance.network.SendData($"{{ \"id\": \"{ev.Player.Sender.SenderId}\", \"type\": \"updateStat\", \"data\": {{ \"name\": \"poopedAs173\", \"value\": \"++1\" }} }}");
            if (!Plugin.StaticInstance.tantrumCounter.ContainsKey(ev.Player))
            {
                Plugin.StaticInstance.tantrumCounter.Add(ev.Player, 1);
            } else
            {
                Plugin.StaticInstance.tantrumCounter[ev.Player]++;
            }
            if (Plugin.StaticInstance.tantrumCounter[ev.Player] >= 10)
            {
                Plugin.StaticInstance.network.SendData($"{{ \"id\": \"{ev.Player.Sender.SenderId}\", \"type\": \"giveAchievement\", \"data\": {{ \"achievementId\": 6}} }}");
            }
        }

        public static void DoorInteracting(InteractingDoorEventArgs ev)
        {
            Plugin.StaticInstance.CheckMassCounts(ev.Player);
            if (ev.Door.IsOpen)
            {
                Plugin.StaticInstance.massCounts[ev.Player].doorsClosed += 1;
            } else
            {
                Plugin.StaticInstance.massCounts[ev.Player].doorsOpened += 1;
            }
        }

        public static void Sacrificed(EnteringFemurBreakerEventArgs ev)
        {
            Plugin.StaticInstance.network.SendData($"{{ \"id\": \"{ev.Player.Sender.SenderId}\", \"type\": \"updateStat\", \"data\": {{ \"name\": \"sacrificed106\", \"value\": \"++1\" }} }}");
        }

        public static void ActivatingNuke(ActivatingWarheadPanelEventArgs ev)
        {
            Plugin.StaticInstance.network.SendData($"{{ \"id\": \"{ev.Player.Sender.SenderId}\", \"type\": \"updateStat\", \"data\": {{ \"name\": \"nukesActivated\", \"value\": \"++1\" }} }}");
        }

        public static void Activating914(ActivatingEventArgs ev)
        {
            Plugin.StaticInstance.network.SendData($"{{ \"id\": \"{ev.Player.Sender.SenderId}\", \"type\": \"updateStat\", \"data\": {{ \"name\": \"upgraded914\", \"value\": \"++1\" }} }}");
        }

        public static void Escaping(EscapingEventArgs ev)
        {
            if (ev.Player.Role.Type == RoleType.ClassD && Respawn.NextKnownTeam == Respawning.SpawnableTeamType.NineTailedFox && !ev.Player.IsCuffed)
            {
                Plugin.StaticInstance.network.SendData($"{{ \"id\": \"{ev.Player.Sender.SenderId}\", \"type\": \"giveAchievement\", \"data\": {{ \"achievementId\": 12}} }}");
            }
            if (Player.List.Where(x => x.Role.Team == ev.Player.Role.Team).ToList().Count == 1 && !ev.Player.IsCuffed)
            {
                Plugin.StaticInstance.network.SendData($"{{ \"id\": \"{ev.Player.Sender.SenderId}\", \"type\": \"giveAchievement\", \"data\": {{ \"achievementId\": 9}} }}");
            }
        }

        public static void PickUp(PickingUpItemEventArgs ev)
        {
            bool hasMicroHID = false;
            bool has3x = false;
            bool hasScp268 = false;
            bool hasScp244 = false;
            bool hasScp018 = false;
            bool has05 = false;

            switch (ev.Pickup.Type)
            {
                case ItemType.MicroHID:
                    hasMicroHID = true;
                    break;
                case ItemType.ParticleDisruptor:
                    has3x = true;
                    break;
                case ItemType.SCP268:
                    hasScp268 = true;
                    break;
                case ItemType.SCP244a:
                case ItemType.SCP244b:
                    hasScp244 = true;
                    break;
                case ItemType.SCP018:
                    hasScp018 = true;
                    break;
                case ItemType.KeycardO5:
                    has05 = true;
                    break;
            }

            foreach (var item in ev.Player.Inventory.UserInventory.Items)
            {
                if (item.Value.ItemTypeId == ItemType.MicroHID)
                {
                    hasMicroHID = true;
                }
                if (item.Value.ItemTypeId == ItemType.ParticleDisruptor)
                {
                    has3x = true;
                }
                if (item.Value.ItemTypeId == ItemType.SCP268)
                {
                    hasScp268 = true;
                }
                if (item.Value.ItemTypeId == ItemType.SCP244a || item.Value.ItemTypeId == ItemType.SCP244b)
                {
                    hasScp244 = true;
                }
                if (item.Value.ItemTypeId == ItemType.SCP018)
                {
                    hasScp018 = true;
                }
                if (item.Value.ItemTypeId == ItemType.KeycardO5)
                {
                    has05 = true;
                }
            }
            if (hasMicroHID && has3x && hasScp018 && hasScp244 && hasScp268 && has05)
            {
                Plugin.StaticInstance.network.SendData($"{{ \"id\": \"{ev.Player.Sender.SenderId}\", \"type\": \"giveAchievement\", \"data\": {{ \"achievementId\": 10}} }}");
            }
        }
    }
}
