# Space Engineers Merge Block Projection Fix

[![Build](https://github.com/rootfabric/SpaceEngineers-MergeBlock-Projection-Fix/actions/workflows/build.yml/badge.svg)](https://github.com/rootfabric/SpaceEngineers-MergeBlock-Projection-Fix/actions/workflows/build.yml)
[![Release](https://img.shields.io/github/v/release/rootfabric/SpaceEngineers-MergeBlock-Projection-Fix)](https://github.com/rootfabric/SpaceEngineers-MergeBlock-Projection-Fix/releases/latest)

**Download:** [MergeBlockProjectionFix-R4.zip](https://github.com/rootfabric/SpaceEngineers-MergeBlock-Projection-Fix/releases/download/v4.0.0/MergeBlockProjectionFix-R4.zip)

Server-side compatibility plugin for **Space Engineers Dedicated Server** that fixes projected **Merge Blocks detaching while being welded in Survival**.

- **No Torch required**
- **Server-side only** — clients do not install anything
- Confirmed working on **Space Engineers Dedicated Server 1.210.014**
- Built for the vanilla `VRage.Plugins.IPlugin` loader
- Uses Harmony only for a narrow runtime patch

## The bug

After the Prosperity / 1.210 changes, an unfinished Merge Block built from a projector can briefly appear attached, then become a separate grid and float/fall away before welding completes.

This breaks common survival printer designs such as:

- automated missiles and torpedoes
- drones
- detachable printed ships
- printer systems that start a projected grid from a Merge Block

Keen Software House reproduced the regression and marked it as **Reported**:

- Keen support report: https://support.keenswh.com/spaceengineers/pc/topic/55863-merge-blocks-from-projector-broken-with-prosperity-update

Related Keen changes:

- Hotfix 1.210.013: https://www.spaceengineersgame.com/space-engineers-hotfix-1-210-013/
- Hotfix 1.210.014: https://www.spaceengineersgame.com/space-engineers-hotfix-1-210-014/

Hotfix 1.210.013 fixed a related case where Merge Blocks detached when welded from projection even when both were already On and functional. The remaining Survival regression affects the unfinished projected Merge Block state.

## What this plugin changes

The regression path involves `MyShipMergeBlock.UpdateOnceBeforeFrame()` refreshing structural neighbours via `UpdateBlockNeighbours()` even when the Merge Block's working state did not actually change.

For an unfinished projected Merge Block, that neighbour refresh can expose the merge face to a connection rule that rejects it while the block is non-working. Grid disconnect detection can then split the unfinished block into a separate grid.

This plugin applies a narrow runtime gate:

1. Enter `MyShipMergeBlock.UpdateOnceBeforeFrame()`.
2. Record `IsWorking`.
3. Intercept the direct `UpdateBlockNeighbours()` call(s) made from that update.
4. If `IsWorking` did **not** change, suppress that neighbour refresh.
5. If `IsWorking` **did** change, allow the normal vanilla refresh.
6. All unrelated `UpdateBlockNeighbours()` calls remain untouched.

This preserves normal Merge Block state transitions while preventing the unfinished projected Merge Block from being disconnected during welding.

## Version

Current release: **R4 / 4.0.0**

R4 accepts the actual runtime shape observed on vanilla Dedicated Server `1.210.014`, where `MyShipMergeBlock.UpdateOnceBeforeFrame()` contains **one direct** `UpdateBlockNeighbours()` call. The plugin also tolerates two calls for compatibility with related builds.

## Installation

Download the latest archive from **GitHub Releases** and keep these files together:

```text
MergeBlockProjectionFix.dll
0Harmony.dll
```

Add `MergeBlockProjectionFix.dll` to the vanilla Dedicated Server plugin list, or launch with the standard plugin argument used by your server setup.

Example:

```bat
SpaceEngineersDedicated.exe -plugin "C:\SEPlugins\MergeBlockProjectionFix\MergeBlockProjectionFix.dll"
```

Clients do **not** need the plugin.

The release ZIP is built and published by GitHub Actions from the repository source.

## Verify that the patch is active

On server startup, check `SpaceEngineersDedicated_*.log` or `MergeBlockProjectionFix.log`.

Expected lines on 1.210.014:

```text
[MergeBlockProjectionFix R4] Initializing R4 runtime-signature merge-update neighbour gate.
[MergeBlockProjectionFix R4] Compatibility guard: UpdateOnceBeforeFrame contains 1 direct UpdateBlockNeighbours call(s).
[MergeBlockProjectionFix R4] PATCH ACTIVE R4: 1 direct UpdateBlockNeighbours call(s) gated by MyShipMergeBlock IsWorking transition.
```

During a projected Merge Block weld, a protected call should produce a message similar to:

```text
SUPPRESS neighbour refresh: Merge IsWorking stayed False inside UpdateOnceBeforeFrame
```

If the runtime game code no longer matches the known shape, the plugin **safe-disables** instead of blindly patching an unknown version.

## Emergency disable

Set before server startup:

```bat
set SE_MERGE_PROJECTION_FIX_DISABLE=1
```

The plugin will load but will not patch the game.

## Building

Requirements:

- .NET Framework 4.8 targeting support
- .NET SDK capable of building `net48`

Build:

```powershell
dotnet build MergeBlockProjectionFix.csproj -c Release
```

The project uses a tiny compile-time `VRage.Plugins.IPlugin` reference surface in `ReferenceVRage/`. That reference project is **not shipped** with the plugin. At runtime, Space Engineers supplies its own `VRage.dll`.

`Lib.Harmony` is restored from NuGet.

## Scope and safety

This is intentionally a small compatibility patch, not a replacement Merge Block implementation.

- no polling loop
- no grid movement
- no ownership changes
- no projector changes
- no welding-speed changes
- no client mod
- normal topology updates outside `MyShipMergeBlock.UpdateOnceBeforeFrame()` are allowed unchanged
- failures fall back to vanilla behavior where possible

## Compatibility

Tested and confirmed working on:

- Space Engineers Dedicated Server `1.210.014`
- Survival
- vanilla Dedicated Server plugin loading
- projected Merge Block printer workflow

Future Space Engineers updates may change the internal method layout. Check the log for `PATCH ACTIVE` after every game update.

## Disclaimer

This is an unofficial community compatibility fix. It is not affiliated with or endorsed by Keen Software House.

Space Engineers and related names are trademarks of their respective owners.

## License

MIT License. See [LICENSE](LICENSE).
