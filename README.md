# TidalLib
Unofficial C# API for TIDAL music streaming service. I overhauled [Yaronzz](https://github.com/yaronzz) implementation, but for the moment, I only left the features I require, meaning you can only perform searches.

## Installation
You can find the .nupkg file in [bin/publish/](https://github.com/Kaufi-Jonas/TidalLib/tree/master/TidalLib/TidalLib/bin/publish). In order to use it in your project, you have to add a local folder to NuGet. Have a look [here](https://stackoverflow.com/a/10240180) to learn how to do this.

## Implementation Notes
The necessary key for authentication is read from the TIDAL desktop client's log file in AppData.
