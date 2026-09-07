# Recovery and rollback guide

Use this guide when a restore produced the wrong result, targeted the wrong software
version, was interrupted, or when settings removed with **Remove selected** must be
recovered.

## First: stop and preserve the recovery data

1. Close the affected graphics application. Do not reopen it repeatedly: many programs
   rewrite their settings while closing.
2. Do not delete or modify the original saved copy.
3. Do not delete the matching folder under:

       DocumentsEzyCGMigrator Rollbacks

4. If the data is important, copy both folders to another drive before proceeding.

Each restore creates a timestamped rollback folder before it changes the destination.
The completion message shows the exact path. A restore does not modify its source saved
copy, so the original package remains available for another attempt.

## Normal recovery through the Rollback tab

1. Make sure every affected graphics application is closed.
2. Start Ezy CG Migrator and open **Rollback**.
3. Choose **Refresh** if the expected entry is not visible.
4. Select the entry whose timestamp matches the problematic restore. Prefer an entry
   marked **ready**. An interrupted restore may also be listed and can undo the changes
   recorded before the interruption.
5. Choose **Revert selected restore**.
6. Read the confirmation carefully, then choose **Yes**.
7. Check the result counts before launching the graphics application:

   - **Restored files** were replaced with their pre-restore copies.
   - **Removed files created by restore** did not exist before and were removed.
   - **Restored registry sets** were returned to their previous state.
   - **Skipped files changed since restore** were left untouched to protect newer work.

The rollback affects only entries recorded for that restore. It does not delete unrelated
files. Avoid applying an entry marked **already reverted** again unless you have inspected
the folder and deliberately want to retry it.

> [!IMPORTANT]
> File rollback is checksum-aware, but registry rollback replaces the complete recorded
> target key with its previous snapshot, or removes that key if it did not exist before.
> Always close the application first.

## If files were skipped

A skipped file no longer matches the file written by the original restore. This usually
means the application or the user changed it afterward. The automatic rollback preserves
that newer file.

1. Keep the current file and make a separate copy of it.
2. Select the rollback entry and choose **Open folder**.
3. Use `rollback-manifest.json` to locate the matching backup file.
4. Compare the current and saved copies before replacing anything manually.

Do not force-copy the entire rollback folder over an application profile: the rollback
folder contains manifests and entry IDs, not a directly reusable profile tree.

## Manual file recovery

Use manual recovery only if Ezy CG Migrator cannot run, automatic rollback reports an
error, or the entry is marked **legacy / manual only**.

First duplicate both the rollback folder and the current destination. Then open
`rollback-manifest.json` in a text editor. Each item under `Entries` provides:

- `TargetPath`: the destination changed by Restore;
- `Kind`: `Directory`, `File`, or `Registry`;
- `BackupDirectory`: the subfolder inside the rollback package;
- `Files`: the individual affected files;
- `RelativePath`: the file path relative to a directory target;
- `ExistedBefore`: whether that file existed before Restore;
- `AppliedSha256`: the checksum of the file written by Restore.

For a `Directory` entry where `ExistedBefore` is `true`, copy:

    <rollback folder><BackupDirectory><RelativePath>

back to:

    <TargetPath><RelativePath>

For a `File` entry, restore the backed-up file to the exact `TargetPath` shown in the
manifest. Preserve the current destination first.

Where `ExistedBefore` is `false`, Restore created the destination file. Delete it manually
only if you are certain it is still the restored file. You can compare its checksum in
PowerShell:

    Get-FileHash -Algorithm SHA256 -LiteralPath "C:exactpath	oile"

The result must equal `AppliedSha256`. If it differs, keep the file and inspect it.

Do not try to import `registry-before.json` with Registry Editor: it is an internal JSON
snapshot, not a `.reg` file. Preserve the rollback folder and use the application's
automatic rollback for registry entries. Manual registry editing should be a last resort
and requires a separate Registry Editor export first.

Legacy folders without `rollback-manifest.json` have no reliable automatic mapping.
Use **Open folder**, compare their contents and dates with the application profile, and
restore individual files only after copying the current versions elsewhere.

## Recovering settings removed from the PC

**Remove selected** creates a normal saved copy before deletion under:

    DocumentsEzyCGMigrator Removed Settings<timestamp>

To recover it:

1. Open **Open / restore**.
2. Select the timestamped folder containing `manifest.json`.
3. Verify every proposed target path. Edit a target if the wrong software version was
   selected.
4. Choose **Preview** and inspect the file count, existing files, and warnings.
5. Select only the required rows and choose **Restore**.

This restore creates its own rollback package, so it can also be reverted.

## Common failure cases

### Wrong version or wrong target path

Rollback the mistaken restore first. Launch the intended software version once so it
creates a clean profile, close it, then restore again with the correct editable target
paths.

### Restore was interrupted

Open **Rollback** and look for the newest timestamp. The manifest is updated before each
file is copied, so the recorded portion can be reverted even when the completion time is
missing. Review the result for skipped files.

### The graphics application no longer starts

Keep the rollback and current profile untouched. Try automatic rollback first. If that is
not possible, copy the current profile, rename the broken profile folder, and let the
application create a clean one. Then restore only known-good categories or individual
files. Never rename or remove a profile while its application is running.

### Rollback itself reports an error

Stop. Do not run Restore again over the same destination. Copy the rollback folder and
the current target, use **Open folder**, and inspect `rollback-manifest.json`. Missing
payload files or an invalid manifest require manual recovery from the preserved copies.
