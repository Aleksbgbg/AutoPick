# AutoPick
AutoPick is a program which accepts, calls your lane, and auto-picks your champion during blind pick champion select in League of Legends.

Although primarily intended for blind pick, runes and draft pick may soon be implemented.

Currently only works on 1280x720 clients and the UI is not very advanced - improvements are being worked on.

If the program misbehaves, it can be disabled by pressing `Ctrl+Shift+Alt+F2` and re-enabled by pressing `Ctrl+Shift+Alt+F3`.

To get a release version you can run, right click `publish.ps1` and press `Run with PowerShell`, or execute `powershell ./publish.ps1` in the current directory. The script will produce a runnable executable `AutoPick.exe`.

## Demo
![AutoPick demo](demo.gif)

## Resolution Support
Common client resolutions and their support:
- `1024x576` - Fully supported
- `1280x720` - Fully supported (default)
- `1600x900` - Fully supported
- `1920x1080` - Fully supported
- `2560x1440` - Fully supported

DPI scaling is supported, however any physical resolution bigger than 2560x1440 probably won't work, so file an issue to ask and I can support it.

Scaled resolutions where the client has a physical resolution not listed in the common resolutions are likely to work but not guaranteed. If it doesn't work for you, file an issue and try a different client resolution for the time being.

All physical resolutions above 1024x576 are enabled. If you are using a resolution below 1024x576 please tell me how you managed to achieve that :).

A resolution is fully supported when all tests pass after the following changes are made:
1. The following basic screens are added to `AutoPick.Tests/TestImages/{resolution}/BasicScreens`:
   - `[1]Home.png`
   - `[2]Lobby.png`
   - `[3]Queue.png`
   - `[4]Accept.png`
   - `[4]AcceptHover.png`
   - `[5]Accepted.png`
   - `[5]Decline.png`
   - `[6]ChampSelectTransition.png`
   - `[6]ChampSelectTransitionBlank.png`
   - `[7]Connecting.png`
   - `[8]Pick.png`
   - `[9]Selected.png`
   - `[9]SelectedHover.png`
   - `[10]Locked.png`
   - `[11]InGame.png`
  
  Note that `[7]ConnectingEarly.png` and `[10]LockedHover.png`, present in the default resolution, are _not_ required.

2. 2 varying frames containing the champ select blue text, and 1 blank transition frame are added to `AutoPick.Tests/TestImages/{resolution}/ChampSelectTransitionFrames`.

3. 5 connecting transition frames (distributed well between starting and finishing the transition) are added to `AutoPick.Tests/TestImages/{resolution}/ConnectingTransitionFrames`.

4. An identical copy of the screens in #1 is added to `MockApp/Screens/{resolution}`, named `1.png` - `15.png`. Screens `[7]ConnectingEarly.png` and `[10]LockedHover.png` are not used.

5. A parameterised test case is added to `AutoPick.Tests/EndToEnd/EndToEndTest.cs` with the resolution parameters.

Check tests for the base resolution `1280x720` to verify what these images look like.
