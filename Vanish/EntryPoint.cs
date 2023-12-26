using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using MEC;
using Mirror;
using PlayerRoles;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;

namespace Vanish;

public class EntryPoint
{
    public const string Version = "1.0.0.1";

    public static readonly HashSet<ReferenceHub> VanishedPlayers = [];
    
    [PluginAPI.Core.Attributes.PluginConfig] public static PluginConfig Config;
    
    private readonly Harmony _harmony = new Harmony("com.jesusqc.vanish");
    
    [PluginEntryPoint("Vanish", Version, "Makes mods being able to vanish", "Jesus-QC")]
    private void Init()
    {
        if (!Config.IsEnabled)
            return;
        
        Log.Raw($"<color=blue>Loading Vanish {Version} by Jesus-QC</color>");
        EventManager.RegisterEvents(this);
        
        _harmony.PatchAll();
    }
	
    [PluginEvent(ServerEventType.PlayerJoined)]
    private void OnPlayerJoined(PlayerJoinedEvent ev)
    {
        // Sync all vanished players
        if (!ev.Player.IsGlobalModerator)
        {
            foreach (ReferenceHub hub in VanishedPlayers)
            {
                ev.Player.Connection.Send(new ObjectDestroyMessage
                {
                    netId = hub.netId
                });
            }
        }
        
        if (!Config.VanishedPlayers.Contains(ev.Player.UserId))
            return;

        Timing.CallDelayed(0.1f, () =>
        {
            Vanish(ev.Player.ReferenceHub);
        });
    }

    [PluginEvent(ServerEventType.PlayerLeft)]
    private void OnPlayerLeft(PlayerLeftEvent ev)
    {
        VanishedPlayers.Remove(ev.Player.ReferenceHub);
    }
    
    [PluginEvent(ServerEventType.RoundRestart)]
    private void OnRoundRestart(RoundRestartEvent ev)
    {
        // Clear vanished players collection
        VanishedPlayers.Clear();
    }
    
    public static bool IsVanished(ReferenceHub hub) => VanishedPlayers.Contains(hub);

    public static void Vanish(ReferenceHub player)
    {
        VanishedPlayers.Add(player);
        
        player.roleManager.ServerSetRole(RoleTypeId.Overwatch, RoleChangeReason.RemoteAdmin);
        
        foreach (ReferenceHub hub in ReferenceHub.AllHubs.Where(hub => hub != player && hub != ReferenceHub.HostHub && hub.authManager.RemoteAdminGlobalAccess))
        {
            hub.connectionToClient.Send(new ObjectDestroyMessage
            {
                netId = player.netId
            });
        }

        Log.Info(player.nicknameSync.DisplayName + " is now vanished.");
    }
}