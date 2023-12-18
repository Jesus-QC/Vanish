using System.Collections.Generic;
using HarmonyLib;
using Mirror;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;

namespace Vanish;

public class EntryPoint
{
    public const string Version = "1.0.0.0";

    public static readonly HashSet<ReferenceHub> VanishedPlayers = [];
    
    [PluginAPI.Core.Attributes.PluginConfig] public static PluginConfig Config;
    
    private Harmony _harmony = new Harmony("com.jesusqc.vanish");
    
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
        foreach (ReferenceHub hub in VanishedPlayers)
        {
            ev.Player.Connection.Send(new ObjectDestroyMessage
            {
                netId = hub.netId
            });
        }
    }
    
    [PluginEvent(ServerEventType.RoundRestart)]
    private void OnRoundRestart(RoundRestartEvent ev)
    {
        VanishedPlayers.Clear();
    }
}