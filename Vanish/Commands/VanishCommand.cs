using System;
using CommandSystem;
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
        
        if (EntryPoint.Config.IsEnabledOnlyForGlobalModerators && !player.ReferenceHub.authManager.RemoteAdminGlobalAccess)
        {
            response = "You must be a global moderator to use this command";
            return false;
        }
        
        if (EntryPoint.Config.IsEnabledOnlyForOverwatch && !player.CheckPermission(PlayerPermissions.Overwatch))
        {
            response = "You must have overwatch permissions to use this command";
            return false;
        }
        
        if (VanishHandler.VanishedPlayers.Contains(player.ReferenceHub))
        {
            player.ReferenceHub.UnVanish();
            response = "You are visible now";
            return true;
        }
        
        player.ReferenceHub.Vanish();
        response = "You are now vanished";
        return true;
    }

    public string Command { get; } = "vanish";
    public string[] Aliases { get; } = Array.Empty<string>();
    public string Description { get; } = "Vanish from other players";
}