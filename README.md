<p align="center"> <img alt="Goob Station 14" width="880" height="300" src="https://github.com/Goob-Station/Goob-Station/blob/master/Resources/Textures/Logo/logo.png" /></p>

This is a fork from the primary repo for Space Station 14 called Goob Station. To prevent people forking RobustToolbox, a "content" pack is loaded by the client and server. This content pack contains everything needed to play the game on one specific server this is the content pack for Goob Station.

If you want to host or create content for SS14, go to the [Space Station 14 repository](https://github.com/space-wizards/space-station-14) as it contains both RobustToolbox and the content pack for development of new content packs and is the base for your fork.

## Links

[Goob Station Discord Server](https://discord.gg/goobstation) | [Goob Station Development Discord Server](https://discord.gg/zXk2cyhzPN) | [Goob Station Forum](https://forums.goobstation.com/) | [Goob Station Website](https://goobstation.com)

## Documentation/Wiki

The Goob Station [docs site](https://docs.goobstation.com/) has documentation on GS14's content, engine, game design, and more. It also has lots of resources for new contributors to the project.

## Contributing

We are happy to accept contributions from anybody. Get in [Development Discord Server](https://discord.gg/zXk2cyhzPN) if you want to help. Feel free to check the [list of issues](https://github.com/Goob-Station/Goob-Station/issues) that need to be done and anybody can pick them up. Don't be afraid to ask for help either!
While following the [Space Station 14 contribution guidelines](https://docs.spacestation14.com/en/general-development/codebase-info/pull-request-guidelines.html) is not mandatory for Goob Station, we recommend reviewing them for best practices.

We are not currently accepting translations of the game on our main repository. If you would like to translate the game into another language consider creating a fork or contributing to a fork.

## Building

1. Clone this repo.
2. Run `RUN_THIS.py` to init submodules and download the engine.
3. <b style="color:crimson;">Different from other SS14 projects! </b>Compile the solution with `dotnet run --project Goobstation.Bootstrap`

[More detailed instructions on building the project.](https://docs.goobstation.com/en/general-development/setup.html)
