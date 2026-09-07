# Ezy CG Migrator

> [!WARNING]
> **Experimental software — use at your own risk.** It is provided as-is, without
> warranties. The author and contributors are not responsible for lost or damaged
> settings, files, plug-ins, licenses, work time, or other losses. Keep an independent
> copy of important data and test restores before relying on this tool.

**If a restore or removal went wrong, stop the affected application and follow the
[Recovery and rollback guide](RECOVERY.md) before changing any more files.**

Portable Windows utility for CG artists and studios. It saves application settings
to an ordinary folder, moves them between PCs or software versions, updates existing
saved copies, and can roll back a restore.

[Download the latest portable release](https://github.com/Ezypoly/EzyCGMigrator/releases/latest)

## What it moves

Preferences, workspaces, hotkeys, presets, brushes, scripts, plug-ins, extensions,
materials, libraries, and other known user data. The scanner only shows locations
that exist on the current PC.

Supported applications (44):

- **Adobe and texturing:** Photoshop, Illustrator, After Effects, Media Encoder,
  Camera Raw, Lightroom Classic, Substance 3D Painter, Designer, Modeler and Sampler,
  plus 3DCoat.
- **3D, CAD, rendering and game tools:** Blender, Maya, 3ds Max, Cinema 4D, Houdini,
  ZBrush, Plasticity, Rhino, Grasshopper, SketchUp, Nuke, Mari, Modo, Marmoset Toolbag,
  Marvelous Designer, CLO, KeyShot, Unreal Engine, Unity and Godot.
- **2D, painting and design:** Affinity Photo, Designer and Publisher, Krita, GIMP,
  Inkscape, CorelDRAW, Corel Painter, Clip Studio Paint, paint.net, Aseprite, PureRef
  and Capture One.

See [SUPPORTED_APPS.md](SUPPORTED_APPS.md) for exact locations and scope notes.

## Quick start

1. Close the graphics applications you are migrating.
2. In **Save**, scan, review the rows, select what you need, and create a saved copy.
3. Move that folder to the other PC if required.
4. In **Open / restore**, open the folder containing `manifest.json`.
5. Review the proposed target paths, use **Preview**, then restore.
6. Use **Rollback** if you need to undo that restore. See the
   [recovery guide](RECOVERY.md) for interrupted restores, skipped files, manual
   recovery, and recovery after **Remove selected**.

**Update saved copy** refreshes selected sets and adds newly discovered ones. Unchanged
sets are skipped using SHA-256 comparison. **Remove selected** creates a recovery copy
and always asks for confirmation before deleting settings from the PC.

## Moving to a newer software version

Yes. Ezy CG Migrator proposes the newest detected target for the same application and
settings category; every target path remains editable. This is file and registry
migration, not format conversion. A newer application may reject old binary preferences
or native plug-ins, so launch the new version once, migrate selectively, and keep the
rollback until the setup is verified.

## Selection and experimental discovery

- Cache-containing and oversized folders are visible but never auto-selected.
- **Settings > Show unclassified profile data** adds experimental items found beside
  known settings inside an application's profile. Cache, log, temporary, crash,
  licensing, credential, session, QuickSave and autosave data are excluded.
- Unclassified rows are never selected automatically — not even by
  **Select / clear all**. Inspect and enable them manually.
- Ctrl/Shift selects multiple rows; clicking one selected checkbox or pressing Space
  applies the same checkbox state to the whole selection.

## Safety

- Saved copies are normal portable folders with a manifest and SHA-256 checksums.
- Restore merges files; it does not delete unrelated target files.
- Existing files and registry values are copied to a rollback package before overwrite.
- Running supported applications block restore and removal.
- Licensing data and machine-specific secrets are intentionally outside the migration scope.

## Updates and build

The portable app updates from this repository's latest GitHub Release. Downloads are
limited to `EzyCGMigrator-win-x64.zip` and verified with the GitHub-provided SHA-256
digest.

Build locally with:

    dotnet build -c Release
    dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
