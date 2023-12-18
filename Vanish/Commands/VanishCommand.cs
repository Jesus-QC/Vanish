using System;
using System.Diagnostics.CodeAnalysis;
using CommandSystem;
using Mirror;
using PlayerRoles;
using RemoteAdmin;

namespace Vanish.Commands;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class VanishCommand : ICommand
{
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, [UnscopedRef] out string response)
    {
        if (sender is not PlayerCommandSender player)
        {
            response = "You must be a player to use this command";
            return false;
        }

        if (!EntryPoint.VanishedPlayers.Add(player.ReferenceHub))
        {
            EntryPoint.VanishedPlayers.Remove(player.ReferenceHub);
            
            foreach (ReferenceHub hub in ReferenceHub.AllHubs)
            {
                if (hub == player.ReferenceHub)
                    continue;
                
                NetworkServer.SendSpawnMessage(player.ReferenceHub.networkIdentity, hub.connectionToClient);
            }
            
            response = "You are visible now";
            return true;
        }
        
        player.ReferenceHub.roleManager.ServerSetRole(RoleTypeId.Overwatch, RoleChangeReason.RemoteAdmin);
        
        foreach (ReferenceHub hub in ReferenceHub.AllHubs)
        {
            if (hub == player.ReferenceHub || hub == ReferenceHub.HostHub)
                continue;
            
            hub.connectionToClient.Send(new ObjectDestroyMessage
            {
                netId = player.ReferenceHub.netId
            });
        }

        response = "You are now vanished";
        return true;
    }

    public string Command { get; } = "vanish";
    public string[] Aliases { get; } = Array.Empty<string>();
    public string Description { get; } = "Vanish from other players";
}