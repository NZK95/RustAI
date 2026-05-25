<div align="center">

# RustAI

*A Telegram bot for realtime Rust automating and game management.*

![GitHub Last Commit](https://img.shields.io/github/last-commit/NZK95/RustAI?style=flat-square)
[![Downloads](https://img.shields.io/github/downloads/NZK95/RustAI/total?style=flat-square&color=brightgreen)](https://github.com/NZK95/RustAI/releases)
![GitHub Stars](https://img.shields.io/github/stars/NZK95/RustAI?style=flat-square)
![GitHub Issues](https://img.shields.io/github/issues/NZK95/RustAI?style=flat-square)
![GitHub License](https://img.shields.io/github/license/NZK95/RustAI?style=flat-square)
![Platform](https://img.shields.io/badge/platform-Windows-blue?style=flat-square&logo=windows)

<img src="https://github.com/NZK95/RustAI/blob/master/docs/examples/start.png?raw=true" width="400"/>

</div>

<br>

## Table of Contents
- [Features](#features)
- [Known Issues](#known-issues)
- [Requirements](#requirements)
- [Config](#config)
- [Usage](#usage)
- [Resources](#resources)
- [Troubleshooting](#troubleshooting)
- [License](#license)

## Features
- Advanced server and player analytics.
- Player tracking with alerts.
- Smart notifications.
- Server connection management with autoconnect.
- Rust launch management.

## Known Issues

> [!NOTE]
> Information is sourced from the Battlemetrics API and may not always be accurate. Missing parameters are replaced with default values.

> [!CAUTION]
> - The bot may occasionally freeze due to API issues — wait and it should resume automatically.
> - `/disconnect` and `/autoconnect` commands may not work.

## Requirements
- Windows x64
- Administrator privileges (for proper functioning of windows switching)
- Configured `config.json`
- Telegram bot token
- Last version of **RustAI** from [`releases`](https://github.com/NZK95/RustAI/releases)

## Config

1. Get a bot token from `@BotFather` in Telegram and paste it into `config.json`.
2. Get your BattleMetrics ID and paste it into `config.json`.
3. Calculate the time in seconds to launch **Rust** and paste it into `config.json`.

> [!WARNING]
> The program will not launch if the config is not set correctly.

| Variable | Type | Default | Description |
|---|---|---|---|
| `CurrentVersion` | String | N/A | **Do not modify.** Represents the latest version. Set automatically. |
| `QueueLimit` | Int32 | 100 | Used for `/connect`. Connection starts when queue reaches this number. |
| `ConnectTimerMinutes` | Double | 10 | Used for `/connect`. Connection starts when timer expires. |
| `TelegramChatID` | Int64 | 0 | **Do not modify.** Set automatically on first launch. |
| `RustLaunchDelaySeconds` | Int32 | 60 | **Ensure this is correct.** Delay before launching Rust after connection. |
| `FavoriteServers` | List | null | Array of favorite servers. `Name` is identifier, `Id` is BattleMetrics ID. |
| `FavoritePlayers` | List | null | Array of favorite players. `Name` is identifier, `Id` is BattleMetrics ID. |
| `TrackedPlayers` | List | null | Array of tracked players with `Name`, `Id`, and `CurrentServer`. |
| `GetPlayerNamesHistory` | Boolean | `true` | Send `.txt` with player name history on `/players`. |
| `GetPlayerServersHistory` | Boolean | `true` | Send `.txt` with player server history on `/players`. |
| `GetServerDescription` | Boolean | `true` | Show server description on `/servers`. |
| `GetServerPlayers` | Boolean | `true` | Show current players on the server. |
| `SendScreenshotWhenJoined` | Boolean | `false` | Auto-send screenshot when joining a server. |

## Usage

| Command | Description |
|---|---|
| `/start`, `/menu` | Open the main menu. |
| `/players`, `/servers` | Get player/server info by Battlemetrics ID. |
| `/launch` | Launch Rust. |
| `/quit` | Close Rust. |
| `/disconnect` | Disconnect from server. Auto-focuses Rust window. |
| `/connect` | Connect to selected server. Auto-launches Rust if needed. |
| `/autoconnect` | Auto-connect when server comes online. |
| `/status` | Send connection status screenshot. |
| `/list` | List tracked players (`Name - Current Server`). |
| `/add` | Add player to tracking by Battlemetrics ID (max 20). |
| `/remove` | Remove player from tracking by Battlemetrics ID. |
| `/clear` | Clear all tracked players. |

> [!NOTE]
> See [examples](https://github.com/NZK95/RustAI/blob/master/docs/examples/players.png).

## Resources
- [BattleMetrics API](https://www.battlemetrics.com/developers/documentation)
- [Telegram Bot API](https://core.telegram.org/bots/api)

## Troubleshooting
If you encounter errors or bugs, please report them via the [issue tracker](https://github.com/NZK95/RustAI/issues).

## License
This project is licensed under the MIT License — see the [LICENSE](LICENSE.txt) file for details.
