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
	
    [PluginEvent(ServerEventType.PlayerJoined)]
    private void OnPlayerJoined(PlayerJoinedEvent ev)
    {
        // We sync all vanished players
        if (!ev.Player.IsGlobalModerator)
        {
            foreach (ReferenceHub hub in VanishHandler.VanishedPlayers)
            {
                ev.Player.Connection.Send(new ObjectDestroyMessage
                {
                    netId = hub.netId
                });
            }
        }               
        else 
        {
            ev.Player.ReceiveHint($"\n\n\n\nThis server is running the Vanish plugin.\nVersion:{Version}", 10f);
        }
        
        // We vanish the player if they are in the config or if they have disconnected while being vanished
        if (!VanishHandler.IsVanishSaved(ev.Player.UserId))
            return;

        Timing.CallDelayed(0.1f, () =>
        {
            ev.Player.ReferenceHub.Vanish();
        });
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
