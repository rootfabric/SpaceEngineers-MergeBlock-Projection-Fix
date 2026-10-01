# Changelog

## R4 / 4.0.0 — 2026-10-01

- Confirmed working on Space Engineers Dedicated Server 1.210.014.
- Fixed compatibility guard to accept the real 1.210.014 runtime method shape with one direct `UpdateBlockNeighbours()` call.
- Gates Merge Block neighbour refresh on actual `IsWorking` state transitions.
- Preserves normal unrelated grid neighbour updates.
- Safe-disable behavior retained for unknown runtime layouts.

## R3 / 3.0.0

- Removed dependency on reflective `SlimBlock` lookup.
- Not effective on tested 1.210.014 because the compatibility guard expected exactly two direct neighbour-refresh calls.

## R2 / 2.0.0

- First transition-gated neighbour refresh implementation.
- Safe-disabled on tested 1.210.014 because `SlimBlock` was not exposed in the expected runtime reflection shape.

## R1 / 1.0.0

- Initial diagnostic patch around Merge Block structural connection checks.
