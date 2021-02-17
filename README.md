# Unity3DBuildInfoWindowFindUnusedAssets
C# scripts for Unity game engine to show used and unused assets of a build.

Reads the Unity build log to find the assets that are really needed and thus also the assets that are unused.

The used and unused assets are displayed in a new window.

The shown assets in this window can be filtered simply by selecting a folder in the project view.

Tested with Unity 5.5.0f3 and Unity 2020.2.1f1 

See also the [forum entry](https://answers.unity.com/questions/57909/find-unused-assets-in-project.html?childToView=1815394#comment-1815394) where this solution has emerged.

<img src="https://answers.unity.com/storage/attachments/108425-screenshot.jpg" alt="build-info-window-screenshot"></src>

## How to use

- Put the cs-files in an [Editor](https://docs.unity3d.com/Manual/SpecialFolders.html) special Unity folder.
- After recompile, a new menu item should show up under `Window > Build Info`. Click this menu item.
- Build your app.
- Click "Update Build Info" button in the window. The used and unused assets should now be listed.

