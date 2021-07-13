# AutoPick
AutoPick is a program which accepts, calls your lane, and auto-picks your champion during blind pick champion select in League of Legends.

Although primarily intended for blind pick, runes and draft pick may soon be implemented.

Currently only works on 1280x720 clients and the UI is not very advanced - improvements are being worked on.

If the program misbehaves, it can be disabled by pressing `Ctrl+Shift+Alt+F2` and re-enabled by pressing `Ctrl+Shift+Alt+F3`.

To get a release version you can run, right click `publish.ps1` and press `Run with PowerShell`, or execute `powershell ./publish.ps1` in the current directory. The script will produce a runnable executable `AutoPick.exe`.

## Resolution Support
Common resolutions and their support:
- `1024x576` - Unsupported
- `1280x720` - Fully supported (default)
- `1600x900` - Unsupported
- `1920x1080` - Fully supported
- `2560x1440` - Unsupported

DPI scaling is currently **not supported** - running the client on monitors using scaling will cause the program to not be able to detect anything or take any actions.

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

## Demo
![AutoPick demo](demo.gif)
