using System.Collections.Generic;

namespace Vanish;

public class PluginConfig
{
    public bool IsEnabled { get; set; } = true;

    public HashSet<string> VanishedPlayers { get; set; } = ["someone@northwood"];
}