# Planet Crafter Mods
- [Discord Presence](#discord-presence)
- [Terraformation Details Overlay](#terraformation-details-overlay)

If you don't know how to install mods for Planet Crafter, follow the instructions [here](https://planet-crafter.fandom.com/wiki/Modding). You can find the dll for the mods on the [releases](https://github.com/Ludeo/PlanetCrafterDiscordPresence/releases) page.

## Discord Presence
This is a Discord Presence for Planet Crafter. It will show whether you are in the Main Menu, the Option Menu, the World Selection Menu or inside of a World as well as how long you are in the specific Menu.
Additionally if you are inside of a world, it will show the world name, the terraformation stage and all the terraformation levels (Biomass, Oxygen, Heat, Pressure). The Presence gets updated every 15 seconds except when you switch the Menu, then it will instantly update the Presence.

### Preview
![Main Menu](https://i.imgur.com/FISzATX.png) ![Option Menu](https://i.imgur.com/0y9X57H.png) ![World Selection Menu](https://i.imgur.com/JAiAjO9.png)

![World1](https://i.imgur.com/iM6DSoh.png) ![World2](https://i.imgur.com/D33gxpk.png) ![World3](https://i.imgur.com/gmcLUyV.png)

### Dependencies
All the required dependencies are inside of the zip file but I am still gonna list them here:

[discord_game_sdk](https://discord.com/developers/docs/game-sdk/sdk-starter-guide)

[DiscordGameSDKWrapper](https://www.nuget.org/packages/DiscordGameSDKWrapper/)

### Known Issues
- When you start the game it might take a few seconds or menu switches till the presence shows up
- When you close the game the presence sometimes doesn't close

### Other
This is my first time creating a mod for a game and also using BepInEx so it won't be perfect, sorry :D Help is appreciated though ^^

## Terraformation Details Overlay
Adds an overlay to the top right of the screen below the Terraformation Index with a textbox consisting of the current Oxygen, Heat, Pressure and Biomass Values.

### Preview
![Preview](https://i.imgur.com/7uIj9M6.png)

### Config
You can change the following properties in the config file:
- Font size
- Distance from top
- Distance from right
- Width of textbox
- Height of textbox
