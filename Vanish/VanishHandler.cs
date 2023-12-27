using System.Collections.Generic;
using Mirror;
using PlayerRoles;
using PluginAPI.Core;

namespace Vanish;

public static class VanishHandler
{
    public static readonly HashSet<ReferenceHub> VanishedPlayers = [];
    
    public static bool IsVanished(ReferenceHub hub) => VanishedPlayers.Contains(hub);

    // Players whitelisted will be able to see vanished players. Contains global moderators by default.
    public static bool IsWhitelisted(ReferenceHub hub)
    {
        // Has overwatch permissions and overwatch is whitelisted
        if (EntryPoint.Config.IsOverwatchWhitelisted && hub.queryProcessor._sender.CheckPermission(PlayerPermissions.Overwatch))
            return true;
        
        // Is global moderator
        return hub.authManager.RemoteAdminGlobalAccess;
    }
    
    public static void Vanish(this ReferenceHub player)
    {
        VanishedPlayers.Add(player);
        
        // We set the player role to overwatch so it prevents crashes and bugs
        player.roleManager.ServerSetRole(RoleTypeId.Overwatch, RoleChangeReason.RemoteAdmin);

        foreach (ReferenceHub hub in ReferenceHub.AllHubs)
        {
            // Do not destroy the own player, will cause a crash on its end!
            if (hub == player) 
                continue;
            
            // Do not send it to the host!
            if (hub == ReferenceHub.HostHub) 
                continue;
            
            // Do not send it to global moderators and other whitelisted players
            if (IsWhitelisted(hub))
                continue;
            
            hub.connectionToClient.Send(new ObjectDestroyMessage { netId = player.netId });
        }

        Log.Info(player.nicknameSync.DisplayName + " is now vanished.");
    }

    public static void UnVanish(this ReferenceHub player)
    {
        VanishedPlayers.Remove(player);
        
        foreach (ReferenceHub hub in ReferenceHub.AllHubs)
        {
            if (hub == player)
                continue;
                
            NetworkServer.SendSpawnMessage(player.networkIdentity, hub.connectionToClient);
        }

        Log.Info(player.nicknameSync.DisplayName + " is now visible.");
    }
}