using HarmonyLib;
using MEC;
using Mirror;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;

namespace Vanish;

public class EntryPoint
{
    public const string Version = "1.0.0.7";
    
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

    [PluginEvent(ServerEventType.PlayerSpawn)]
    private void OnPlayerSpawn(PlayerSpawnEvent ev)
    {
        // We vanish the player if they are in the config or if they have disconnected while being vanished
        if (!VanishHandler.IsVanishSaved(ev.Player.UserId))
            return;

        // We wait one frame
        Timing.CallDelayed(0.1f, () =>
        {
            ev.Player.ReferenceHub.Vanish();
        });
    }
    
    [PluginEvent(ServerEventType.PlayerJoined)]
    private void OnPlayerJoined(PlayerJoinedEvent ev)
    {
        // If the player is global moderator, we send them a hint telling them about the plugin
        if (ev.Player.IsGlobalModerator)
        {
            ev.Player.ReceiveHint($"\n\n\n\nThis server is running the Vanish plugin.\nVersion:{Version}", 10f);
            return;
        }          
        
        // We sync all vanished players to the new player
        foreach (ReferenceHub hub in VanishHandler.VanishedPlayers)
        {
            ev.Player.Connection.Send(new ObjectDestroyMessage
            {
                netId = hub.netId
            });
        }
    }

    [PluginEvent(ServerEventType.PlayerLeft)]
    private void OnPlayerLeft(PlayerLeftEvent ev)
    {
        VanishHandler.VanishedPlayers.Remove(ev.Player.ReferenceHub);
    }

    [PluginEvent(ServerEventType.PlayerChangeRole)]
    private void OnPlayerChangingRole(PlayerChangeRoleEvent ev)
    {
        // If the player is vanished, we unvanish them
        if (!VanishHandler.VanishedPlayers.Contains(ev.Player.ReferenceHub))
            return;
        
        ev.Player.ReferenceHub.UnVanish();
    }
    
    [PluginEvent(ServerEventType.RoundRestart)]
    private void OnRoundRestart(RoundRestartEvent ev)
    {
        // Clear vanished players collection
        VanishHandler.VanishedPlayers.Clear();
    }
}
