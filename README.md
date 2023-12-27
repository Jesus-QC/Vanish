# Vanish
Introduces a vanish command into SCP: Secret Laboratory to fight around cheaters.

It fakes that the player has been disconnected by sending to non whitelisted players a destroy message.

## Configuration Options

| Option                 | Type     | Description                                             | Default Value                                            |
|------------------------|----------|---------------------------------------------------------|----------------------------------------------------------|
| IsEnabled              | Boolean  | Wheter or not the plugin is enabled                     | `true`                                                   |
| VanishedPlayer         | String[] | The list of players that will be automatically vanished when they join the server | `- someone@northwood` |
| IsOverwatchWhitelisted           | Boolean  | If true, players with overwatch permissions will be able to see other vanished players | `true` |
| IsEnabledOnlyForGlobalModerators | Boolean  | If true, only global moderators will be able to use the vanish command | `false` |
| IsEnabledOnlyForOverwatch        | Boolean  | If true, only players with overwatch permissions will be able use the vanish command | `false` |


## How to use

Run the `vanish` command in the Remote Admin panel, you will be automatically set as overwatch. Once you finish you can use the `vanish` command again.

Anyone with access to RA can use the command with the default settings.
