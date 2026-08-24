# Graphics Settings Migrator

> [!WARNING]
> **Experimental software — use entirely at your own risk.** This project is
> provided as-is, without warranties or guarantees of compatibility, completeness,
> data integrity, or fitness for a particular purpose. The author and contributors
> accept no responsibility or liability for lost or damaged settings, files,
> plug-ins, licenses, work time, or any other direct or indirect loss. Keep an
> independent copy of important data and test the workflow before relying on it.

Graphics Settings Migrator is a portable Windows utility for artists, technical
artists, freelancers, and studios that need to copy application settings between
software versions or prepare another workstation with a familiar production setup.

Typical uses include:

- moving an artist from an old PC to a new workstation;
- preparing multiple studio computers from an approved settings package;
- carrying hotkeys, workspaces, presets, brushes, scripts, plug-ins, and preferences
  to a newer application version where the target format remains compatible;
- keeping updateable, checksum-verified copies of important application setup;
- removing obsolete settings with a recovery copy and reverting previous restores.

The program discovers known settings locations, lets the user choose individual
sets, stores them in an ordinary portable folder, and restores them to automatically
detected or manually edited destinations. It does not upload settings to a service.

[Download the latest portable release](https://github.com/Ezypoly/GraphicsSettingsMigrator/releases/latest)

## Before using it

- Close supported graphics applications before Save, Update, Restore, or Remove.
- Keep the original application profiles and a separate copy of valuable custom content.
- Test a saved copy and restore on a disposable profile or non-production workstation first.
- Review the selected rows, target paths, size, file count, and Preview before confirming.
- Treat native plug-ins, binary preference files, licensing data, and machine-specific
  settings as version- and workstation-dependent unless the vendor documents otherwise.

## Typical workflow

1. Open **Save**, scan the workstation, and select the settings sets you need.
2. Choose a **Save library folder** and create a timestamped portable saved copy.
3. Move that saved-copy folder to another PC, or keep it for a later software upgrade.
4. Open **Open / restore**, select the folder containing `manifest.json`, and review
   the automatically proposed destination for every row.
5. Use **Preview**, adjust editable target paths if necessary, and restore only the
   compatible sets you intend to migrate.
6. If required, use **Rollback** to revert files and registry values changed by a restore.

## Moving to a newer application version

Yes, cross-version migration is supported. For each saved set, Restore looks for
targets belonging to the same application, settings category, and storage type,
then normally proposes the newest detected version. The target path is always
visible and editable before Restore.

This is file and registry migration, not vendor-specific format conversion. A newer
application may reject, ignore, rewrite, or misinterpret data produced by an older
version. Presets, scripts, brushes, hotkeys, and text-based configuration are generally
safer than opaque binary preferences or compiled native plug-ins. Launch the new
application once so it creates its profile, migrate selectively, use **Preview**, and
keep the automatic rollback copy until the new setup has been verified.

## Supported applications (44)

The scanner only shows settings that exist on the current PC. Restore can map a
backup to another installed version or to an editable custom target path.

### Adobe and texturing

- Adobe Photoshop
- Adobe Illustrator
- Adobe After Effects
- Adobe Media Encoder
- Adobe Camera Raw
- Adobe Lightroom Classic
- Adobe Substance 3D Painter
- Adobe Substance 3D Designer
- Adobe Substance 3D Modeler
- Adobe Substance 3D Sampler
- 3DCoat

### 3D, CAD, rendering, and game tools

- Blender
- Autodesk Maya
- Autodesk 3ds Max
- Cinema 4D
- Houdini
- Maxon ZBrush
- Plasticity
- Rhino
- Grasshopper
- SketchUp
- Nuke
- Mari
- Modo
- Marmoset Toolbag
- Marvelous Designer
- CLO
- KeyShot
- Unreal Engine
- Unity
- Godot

### 2D, painting, and design

- Affinity Photo 2
- Affinity Designer 2
- Affinity Publisher 2
- Krita
- GIMP
- Inkscape
- CorelDRAW
- Corel Painter
- Clip Studio Paint
- paint.net
- Aseprite
- PureRef
- Capture One

See [SUPPORTED_APPS.md](SUPPORTED_APPS.md) for scope notes and application-specific details.

## Safety model

- Backup packages are ordinary portable folders with manifest.json and payload.
- Every payload file has a SHA-256 checksum.
- Restore merges files and never deletes extra target files.
- Existing destination files are copied to
  Documents\GraphicsSettingsMigrator Rollbacks before overwrite.
- Running graphics applications block restore.
- Paths use portable profile tokens, so a package can be moved to another Windows user or PC.
- Custom scripts, plug-ins, extensions, presets, brushes, materials, packages,
  and libraries stored outside the usual preferences folder are included where
  their locations are known.

## Plug-in discovery

In addition to plug-ins already contained in normal application profiles, the
scanner checks install-level, shared, and environment-configured locations for
ZBrush, Autodesk/Maya/3ds Max, Cinema 4D, Houdini, Nuke/OpenFX, Blender, Mari,
paint.net, Capture One, and Unreal Engine. Adobe installation plug-ins and
CEP/UXP extensions remain supported. Native binaries are shown separately
because they may require administrator rights and may not work across versions.

Project plug-ins belonging to Unity, Godot, or Unreal projects remain inside
those projects and are not discovered by a global disk search.

## Updating an existing saved copy

Open **Update saved copy**, choose a folder containing `manifest.json`, and the
app will show the same detailed settings table used by the Save and Open / restore
workflows. It combines the package contents with every settings set currently
found on the PC. Existing rows can be refreshed, while rows marked **New — add**
can be appended to an older package (including newly discovered plug-in folders).
Select the required sets and choose **Update selected**.

- Every selected set is compared by SHA-256, not only by file date or size.
- If all files are identical, that set is skipped and the package is not rewritten.
- Newly discovered selected sets are appended with their own payload and checksums.
- Unchecked sets and contents not found on the current PC remain untouched.
- Files removed from a selected source are removed from that set in the package.
- The original package is replaced only after a complete staging copy and new
  manifest are ready; a failed update moves the original folder back.
- Cache and oversized folders remain manual selections, just like on the Save tab.
- Ctrl/Shift multi-selection and Space or group checkbox toggling work on this tab too.

The **Save library folder** is a parent folder that can contain many timestamped
saved copies. A **Saved copy folder** is one specific child folder containing
`manifest.json`; choose that folder when updating or restoring.

## Selection and cache handling

- Use Ctrl or Shift to highlight multiple rows, then click any highlighted
  checkbox or press Space to toggle all of their checkboxes.
- **Auto-select folders up to** is saved between runs and defaults to 500 MB.
  Larger folders remain visible but unchecked; set it to `0` for no size limit.
  **Select / clear all** also respects this limit. The control lives on the
  **Settings** tab and applies to Save, Open / restore, and Update saved copy rows.
- **Select / clear all** selects normal settings but deliberately skips every
  cache-containing set. Cache rows can only be enabled manually.
- Choosing **Open...** on the Open / restore or Update saved copy tab loads the
  selected folder immediately. **Open** remains available for a pasted or manually
  edited path.
- Save destination, last opened restore folder, last opened update folder, overwrite
  choice, size limit, and row selections are stored under `%LOCALAPPDATA%` and
  survive portable application updates.
- **Appearance** switches instantly between **Classic dark** (the original dark
  interface) and **Console**; both keep the native Windows frame dark. The choice
  is available on **Settings** and is remembered between launches and updates.
- Every settings table allows column widths to be resized by dragging a header
  divider. Drag a column header to change its display order; Path and Notes use
  normal resizable widths instead of locked fill sizing.

## Removing old settings

Highlight one or more rows on the Save tab and choose **Remove selected...**.
Removal is blocked while a supported graphics application is running and always
requires an explicit Yes/No confirmation (No is the default). Before anything
is removed, the selected data is backed up to
`Documents\GraphicsSettingsMigrator Removed Settings`. Only files whose SHA-256
still matches that recovery backup are deleted. Excluded projects, scenes, and
caches remain untouched unless their own row is explicitly highlighted.

## Updates

Version 1.3.0 and later can update themselves from the repository's latest
GitHub Release. Use **Check for updates** on the **Settings** tab. The updater:

- downloads only `GraphicsSettingsMigrator-win-x64.zip` from this repository;
- verifies the exact asset size and GitHub-provided SHA-256 digest;
- closes the running application before replacing files;
- copies release files into the current portable folder without deleting other files;
- restarts the updated application and removes its temporary download.

Updating an installation in a protected folder may show a Windows UAC prompt.

## Rollback

Every restore creates a rollback manifest. The Rollback tab can restore overwritten files and registry settings and remove files created by that restore. Files changed again after the restore are skipped to protect newer work. Rollbacks created by older versions remain available for manual recovery.

## Build

Run:

    dotnet build -c Release
    dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true

For cross-version migration, review the editable target path before restoring.
Application presets are generally safer to move between versions than complete binary preference files.
