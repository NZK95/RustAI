[![Downloads](https://img.shields.io/github/downloads/NZK95/RustAI/total.svg)](https://github.com/NZK95/RustAI/releases)

<p align="center">
  <img src="https://github.com/NZK95/RustAI/blob/master/docs/examples/start.png?raw=true" height="300">
</p>

## Features
- Advanced server and player analytics.
- Player tracking with allerts.
- Smart notifications.
- Server connection management with autoconnect too.
- Rust launch management.
  
## Known Issues
> The information displayed is sourced from the Battlemetrics API and may not always be completely accurate or up-to-date.  If some parameters cannot be retrieved, they will be automatically replaced with default values.

- The bot may occasionally freeze due to API issues. Please wait and it should resume automatically.
- The  `/disconnect` and  `/autoconnect` command may not work.
  
## Requirements
- Last version of **RustAI** from [`releases`](https://github.com/NZK95/RustAI/releases) <br>
- Configured ```config.json```
- Windows 10 or older
- Telegram

## Config
1. Get and paste into ```config.json``` a bot token with ```@BotFather``` bot in Telegram. <br>
2. Get and paste into ```config.json``` your BattleMetrics ID. <br>
3. Calculate and paste into ```config.json``` , the time in seconds to launch **Rust**. <br>

| Variable                 | Type   | Default Value | Description                                                                                                                    |
|--------------------------|--------|---------------|--------------------------------------------------------------------------------------------------------------------------------|
| `CurrentVersion`             | String  | N/A             | **Do not modify.** Rappresents the latest version of program. The value is set by default.           |
| `QueueLimit`             | Int32  | 100             | Used for `/connect`. Represents the number of queued players. Connection starts when the queue reaches this number.           |
| `ConnectTimerMinutes`    | Double | 10           | Used for `/connect`. Represents the delay in minutes. Connection starts when the timer expires.                               |
| `TelegramChatID`         | Int64  | 0             | **Do not modify.** This value is automatically set on the first launch of the program.                                         |
| `RustLaunchDelaySeconds` | Int32  | 60            | **Ensure the value is correct**. Delay in seconds before launching Rust after connection is established.                                                         |
| `FavoriteServers` | List  | null            | The array of favorite servers, where `Name` is server identifier and `Id` his BattleMetrics ID.                                                    |
| `FavoritePlayers` | List  | null            | The array of favorite players, where `Name` is player identifier and `Id` his BattleMetrics ID.                                                    |
| `TrackedPlayers` | List  | null            | The array of tracked players, where `Name` is player identifier, `Id` his BattleMetrics ID, and `CurrentServer` the last played server.                                             |
| `GetPlayerNamesHistory`       | Boolean | `true`  | Send the `.txt` file with player's history of names during the `/players` command.                                                              |
| `GetPlayerServersHistory`     | Boolean | `true`  | Send the `.txt` file with player's history of servers  during the `/players` command.                                            |
| `GetServerDescription`        | Boolean | `true`  | Display server description during the `/servers` command. Сan be annoying if the description is too long.                                                     |
| `GetServerPlayers`            | Boolean | `true`  |  Display the list of players currently on the server.                                       |
| `SendScreenshotWhenJoined`    | Boolean | `false` | Automatically send a screenshot when successfully joining a server.                                  |
> The program will not launch if the config is not set correctly.

## Usage
See <a href="https://github.com/NZK95/RustAI/blob/master/docs/examples/players.png">examples of usage</a>

| Command                  | Description                                                                                                                      |
|--------------------------|----------------------------------------------------------------------------------------------------------------------------------|
| `/start`, `/menu`        | Open the main menu with all commands.                                                                                            |
| `/players`, `/servers`   | Get player/server info by Battlemetrics ID. May include `.txt` exports (configurable in `config.json`).                         |
| `/launch`                | Launch Rust.                                                                                                                     |
| `/quit`                  | Close Rust.                                                                                                                      |
| `/disconnect`            | Disconnect from server. Requires active connection. Auto-focuses Rust window.                                                   |
| `/connect`               | Connect to selected server. Auto-launches Rust if needed, waits `RustLaunchDelaySeconds`, then connects. Auto-focuses window.   |
| `/autoconnect`           | Auto-connect when server comes online. Useful for offline servers.                                                              |
| `/status`                | Send connection status screenshot. Auto-focuses Rust. Ignored if Rust not running.                                              |
| `/list`                  | List tracked players (format: `Name - Current Server`).                                                                         |
| `/add`                   | Add player to tracking by Battlemetrics ID (max 20 players).                                                                    |
| `/remove`                | Remove player from tracking by Battlemetrics ID.                                                                                |
| `/clear`                 | Clear all tracked players.                                                                                                      |

## Troubleshooting
If you encounter errors or bugs, please report them via the [issue tracker](https://github.com/NZK95/RustAI/issues).<br>

## Resources
[BattleMetrics API](https://www.battlemetrics.com/developers/documentation) <br>
[Telegram Bot API](https://core.telegram.org/bots/api) <br>

## License
This project is licensed under the MIT License - see the [LICENSE](LICENSE.txt) file for details.
