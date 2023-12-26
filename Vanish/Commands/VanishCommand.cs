using System;
using System.Diagnostics.CodeAnalysis;
using CommandSystem;
using Mirror;
using PlayerRoles;
using PluginAPI.Core;
using RemoteAdmin;

namespace Vanish.Commands;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class VanishCommand : ICommand
{
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (sender is not PlayerCommandSender player)
        {
            response = "You must be a player to use this command";
            return false;
        }

        if (EntryPoint.VanishedPlayers.Remove(player.ReferenceHub))
        {
            foreach (ReferenceHub hub in ReferenceHub.AllHubs)
            {
                if (hub == player.ReferenceHub)
                    continue;
                
                NetworkServer.SendSpawnMessage(player.ReferenceHub.networkIdentity, hub.connectionToClient);
            }
            
            Log.Info(player.Nickname + " is now visible.");
            response = "You are visible now";
            return true;
        }
        
        EntryPoint.Vanish(player.ReferenceHub);
        response = "You are now vanished";
        return true;
    }

    public string Command { get; } = "vanish";
    public string[] Aliases { get; } = Array.Empty<string>();
    public string Description { get; } = "Vanish from other players";
}