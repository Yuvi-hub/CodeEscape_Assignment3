# Part B — Implementation and submission instructions

## Unity setup
1. In Unity Hub, choose **Add project from disk** and select the `CodeEscape_Assignment3` folder. Use a compatible Unity 2022.3 LTS editor, or allow Unity to upgrade it.
2. Open `Assets/Scenes/Main.unity`, then press Play. This is a code-driven, single-scene game; its controller object is already wired to the script.
3. If Unity warns about legacy input, go to Project Settings > Player > Active Input Handling and choose **Both** (or Input Manager Old), then restart the editor.
4. The game uses `OnGUI` for HUD and touch buttons; it does not need an imported font or sprites.

## Required programming concepts
- **Spawning / prefabs:** `Spawn` loads a prefab from `Resources/Prefabs/{Player,Token,Exit,Hazard}` if supplied and instantiates it; otherwise it instantiates a Unity cube. The token, exit, player and hazard types are generated from the level map. To satisfy a strict requirement for actual prefabs, create four cube prefabs with those names in `Assets/Resources/Prefabs/` and verify they spawn. The package includes prefab-ready code but no authored `.prefab` assets.
- **Collision:** tile-based overlap detects tokens, exits and hazards. Walls are checked with `Walkable`; physics colliders are not needed for this discrete grid game.
- **Level management:** the game changes states (menu, playing, won, lost, finished) without scene reloads; unlocked level index and last completed step count are saved with PlayerPrefs.
- **Mobile controls:** four large on-screen direction buttons support touch input. Keyboard movement also works.

## Build and publish (must be completed in Unity)
1. Add `Assets/Scenes/Main.unity` to Build Settings (Scenes in Build).
2. Install **WebGL Build Support** for the editor in Unity Hub. Switch platform to WebGL, build into a new `Builds/WebGL` folder. Test the exported `index.html` via a local web server, then zip the *contents* of the WebGL output folder for itch.io.
3. On itch.io create/edit the game, set **HTML** playable-in-browser, upload the WebGL zip, select **This file will be played in the browser**, and save/publish. Test the published URL on desktop and on a phone browser.
4. Install **Android Build Support** (SDK/NDK/OpenJDK) in Unity Hub. Switch platform to Android, configure package identifier, orientation and minimum API as required by your device, then build an APK. Install it on a physical Android phone and record a brief demonstration.
5. Submit the itch.io URL, repository URL, Android demonstration video, Part A design file, and Unity project as required by the course portal. Confirm the exact submission fields there.

## Test checklist
- [ ] Unity imports without compiler errors
- [ ] All four level maps render correctly
- [ ] Tutorial: token must be collected before exit works
- [ ] Walls block movement
- [ ] Medium hazard costs a life and three hits end the run
- [ ] Hard hazards move and can damage player
- [ ] Completing a level unlocks the next after app restart
- [ ] Touch buttons work on a phone
- [ ] WebGL build loads on itch.io
- [ ] Android APK installs and launches on a phone

**Not yet performed:** Unity Editor execution, actual prefabs creation, WebGL export, itch.io upload, Android build, device testing, video recording. These require a Unity installation and access to the publishing/device environment.
