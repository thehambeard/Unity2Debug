
#  Unity2Debug

> [!IMPORTANT]
> **The version of unity for your game is required ONLY if you plan on using the Create Debug Option. Unity is not required for Decompilation Only.**

> [!NOTE]
> Due to this tool using symlink functionality it must be run as admin.

This will quickly decompile Unity games with ILSpy and setup an optional development copy of the game in a separate folder.

Features:
* Game profiles: Each profile will contain persistent settings allowing to quickly run the tool again if a game releases an update.  Profiles for Owlcat games have been included.
* Batch decompilation: A custom assembly list will be decompiled. Useful for games with multiple assemblies (e.g. Rogue Trader).
* Automatic setup of development environment for debugging.
  * Copies of the dev binaries from the appropriate unity directory. Unity version of the game will attempt to auto-detect. This does mean the unity version of the game you wish to debug will be required to be installed.
  * Sets up the boot.config.  Any custom options in the boot.config will not be lost the tool ensures it only sets or adds the required options.
  * steam_appid.txt will be generated. This prevents SteamWorks from interfering with the debugger. The steam_appid can be auto-detected.
  * A new copy of the game will be created as to not interfere with retail game. Allows for being able to play and develop separate from each other and have different mods installed for each version.
  * Adds ini for each binary to disable optimizations from the JIT. Not sure if this actually does anything but from what I have read it should. Jury is still out.
  * Symlinking of directories and files. These increases speed of copying and decreases size of development version. Most of the files and assets not needed in development can be symlinked with custom filters.

Future Features:
* Automatic generation of debug symbols (portable pdb's).

## Documentation

### Decompilation Page

The initial page you will be presented on launch is the decompilation options. These are an explanation of each option as you go down the page.

1. **Profile**
 * Editable Combo Box: Lists each profile that has been setup. Selecting a profile from the drop down will select the saved settings for the game in question. Typing when selected allows for a new profile to be created once the Add button is clicked
 * Add Button: Adds a new profile with whatever has been entered to the combo box as long as it is a different name than any other profile.
    * Remove Button: Removes the currently selected profile.
3. **Source Output Code**
 * Text Box: Path to where you would like the code to be outputted. It is recommended that this directory is empty to avoid overwriting anything and being able to later tell is something was removed from an assembly if the game is updated.
   * Browse Button: Will open a folder dialog to select output directory. Any button labeled Browse will no longer be explained.
4. **Files to Decompile**
 * List Box: A list of all the assemblies to decompile.
   * Add Button: Opens file dialog to select assemblies. Multi file selection is enabled.
   * Remove Button: Removes the currently selected assembly.
5. **Create Debug Copy / Only Decompile**
 * Create Debug Copy: When Next Button is clicked the debug options will be shown. This is intended if you wish to decompile and setup the development environment.
   * Only Decompile: When Next Button is clicked the tool will go directly to decompilation process.
6. **Errors**
 * List any errors or warnings here.
7. **Next Button**
 * Clicking next will go to the next page and save your current settings. The button will be disabled if there are any errors with your settings. Warnings are ignored.

### Debug Page

Settings for creating of the development environment are set up here.

1. **Retail Game Exe**
 * Path to the retail game exe file.  This is usually named after the game for an abbreviation located in the base directory of the install. (e.g. C:\Program Files (x86)\Steam\steam apps\common\Pathfinder Second Adventure\Wrath.exe)
2. **Steam Appd Id**
 * App I'd for the game. Ensure SteamWorks does not interfere with debugging. Apparently GOG games that are also released on Steam may have SteamWorks so it is recommended to include it.
   * Find Button. Will attempt to locate the App I'd from your steamapps manifests. Untested on those who have 100's or 1000s of games so it may be slow in those cases. If any issues please report. Does not work on GOG and you will have to go to the steam page of the game to find the App I'd in the URL.
3. **Debug Output Path**
 * Path to where you would like development copy to be located. Should probably be put in the same directory of your other Steam/GOG games. (e.g. C:\Program Files (x86)\Steam\steam apps\common\Pathfinder Second Adventure Debug)
4. **Unity Path**
 * Path to the Unity Editor folder that contains all the version of Unity that have been installed.  Default path is C:\Program Files\Unity\Hub\Editor.
5. **Unity Version**
 * Version of Unity the game uses. These must match or game probably will not even launch. The version will try to be automatically detected from the game exe.
6. **Use Symlinks**
 * Checked if symlinks are to be used, unchecked if they are not.
7. **Symlinks**
 * List Box: The list of the current symlink filters that are to be used.
   * Add Button: Opens the symlink dialog
 * Remove Button: Removes the currently selected symlink filter in the list box.
8. **Navigation Buttons**
 * Back Button will return to Decompile Page.
   * Next Button will move on to processing.
  
### Symlinks Dialog

They filter for symlinks are setup here. Filters are windows standard wildcards. (e.g. *.dll shows all dll files. level\* shows all files that start with level). Directories take precedence, and all files and sub directories will be symlinked. No need to symlink files in a symlink directory. The format is {path-to-filter-target}\{filter}. Do not symlink the managed folder where the assemblies that are to be decompiled.

1. **Files/Directories**
 * Switch between showing only affected files or only affected directories.
2. **List of Symlinks/List of Affected**
 * Left/Right List Boxes.
     * Left box is the list of the filters to add.
     * Right box is a list of currently affected items. Right clicking anything in this box will copy it to the text box below. If nothing is in the is box then the filter is invalid and will not be allowed to be added.
3. **Filter Text Box/Buttons**
 * Text Box: Filter to be added. Any change here will be reflected in the affected list box. Again, if there is nothing listed in the affected box then it cannot be added.  In Files mode a filter with '\' at the end will denote a directory (e.g. Bundles\ will symlink that directory, not need to symlink anything else in that directory.  In Directory mode you only need to type Bundles for the same effect).
   * Add Button: Adds to left list box, directories will be denoted with a '\' at the end.
   * Remove Button: Removes currently selected filter from the left box.
   * Ok Button: Returns to the Debug Settings Page and adds the symlinks. Closing the dialog via 'X' will discard the changes.

### Processing Page

Logs what is going on during the copying and decompilation. During decompilation each file will show progress via the progress bar at the bottom of the page. For large assemblies it is normal for the bar to "get stuck" at the end for a bit.  This is normal behavior as files are being written to disk.  Processing should be rather quick, Pathfinder: Wrath of the Righteous takes less than 1 minute to complete all processing.
