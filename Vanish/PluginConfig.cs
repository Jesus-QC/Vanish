using System.Collections.Generic;
using System.ComponentModel;

namespace Vanish;

public class PluginConfig
{
    public bool IsEnabled { get; set; } = true;

    [Description("The list of players that will be automatically vanished when they join the server")]
    public List<string> VanishedPlayers { get; set; } = ["someone@northwood"];
    
    [Description("If true, players with overwatch permissions will be able to see other vanished players")]
    public bool IsOverwatchWhitelisted { get; set; } = true;
    
    [Description("If true, only global moderators will be able to use the vanish command")]
    public bool IsEnabledOnlyForGlobalModerators { get; set; } = false;
    
    [Description("If true, only players with overwatch permissions will be able use the vanish command")]
    public bool IsEnabledOnlyForOverwatch { get; set; } = false;
}