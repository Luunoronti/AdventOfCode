# A3A (Amarthdae's 3D AoC Visualizer) - Design and Implementation Plan

## Goals
- Cross-platform C# app with explicit Vulkan (Linux), Metal (macOS), D3D12 (Windows).
- 3D visualization for AoC problems with a modern 2D UI layer.
- Separate client libraries so debug-stopping a client does not freeze the visualizer.
- Time-travel: keep history of visual states and allow rewind/step.

## Non-Goals (for now)
- Networked multi-user sessions.
- Web or mobile targets.
- Full DCC asset pipeline.

## High-Level Architecture
```
Client App (AoC solution)
  -> Client Lib (C#)
     -> Local IPC (named pipes / Unix domain sockets)
        -> A3A Visualizer
           - Runtime Core
           - Scene & Timeline
           - Renderer (Vulkan/Metal/D3D12)
           - UI Layer
```

## Tech Stack Candidates (Decision Required)
### Renderer Backend (must support Vulkan/Metal/D3D12)
Option A: bgfx (via C# bindings)
- Pros: battle-tested, backend-agnostic, D3D12/Vulkan/Metal support.
- Cons: lower-level, custom C# interop, limited modern RT features.

Option B: wgpu-native (via C# bindings)
- Pros: modern API, D3D12/Vulkan/Metal support, good safety model.
- Cons: binding maturity in .NET varies, RT features limited.

Option C: Custom per-backend (Vulkan + Metal + D3D12 wrappers)
- Pros: max control, best RT access.
- Cons: large effort, high maintenance.

### 2D UI Layer
Option A: Dear ImGui (ImGui.NET) for tools + custom 2D HUD
- Pros: fast dev, great tooling.
- Cons: not "modern UX" by default.

Option B: SkiaSharp or NanoVG for custom 2D UI
- Pros: flexible visuals.
- Cons: more work to build widgets.

Option C: Avalonia (separate UI window)
- Pros: modern UX, strong widgets.
- Cons: integration/interop with 3D view can be tricky.

## Core Subsystems
1) **Runtime Core**
   - App lifecycle, windowing, input, settings.
   - Frame scheduler + fixed time step.

2) **Renderer**
   - Backend abstraction (Vulkan/Metal/D3D12).
   - Render graph (deferred/forward+ hybrid).
   - Post-processing chain.

3) **Scene System**
   - Entities, transforms, components (Mesh, Light, Camera, Text).
   - Instancing support (GPU instance buffers).
   - Materials and shaders.

4) **Timeline / History**
   - Stores a stream of state deltas.
   - Supports rewind/step/playhead.
   - Snapshot + delta compression for memory control.

5) **Client Protocol + IPC**
   - Versioned message schema.
   - Local transport (named pipes on Windows, Unix domain sockets on Linux/macOS).
   - Stateless reconnection and resync.

6) **UI**
   - 2D overlay for controls and debug.
   - 3D UI text elements inside the scene.
   - Camera mode toggles (UE5/Unity style).

## Rendering Feature Requirements (Initial Scope)
- Mesh rendering + animation.
- Instancing for large counts.
- Lighting: ambient, directional, sky, point, cone (spot), area.
- Shadows: soft shadows with ray tracing (hardware if available, compute fallback).
- Ambient occlusion: RT or SSAO fallback.
- Global illumination: choose between RT/Lumen-like or screen-space.
- Reflections: screen-space reflections (SSR).
- Anti-aliasing: FXAA, MSAA, TAA, extensible.
- HDR: output up to 1000 nits.
- Post-processing: color correction, tone mapping, bokeh DoF.

## Data Model and Flow
### Client -> Visualizer Message Types (Draft)
- `Hello` (protocol version, capabilities).
- `FrameBegin`, `FrameEnd` (explicit frame boundary).
- `UpsertEntity` (id, transform, mesh/material refs).
- `RemoveEntity`.
- `UpsertMesh`, `UpsertMaterial`, `UpsertTexture`.
- `UpsertLight`.
- `SetCamera`, `SetEnvironment`.
- `DrawText3D`, `DrawText2D`.
- `SetUserOverlay` (custom info panels).

### Runtime Flow
1) Client emits messages for each simulation step.
2) Visualizer ingests and builds a scene state.
3) State is stored to timeline as delta or snapshot.
4) Renderer presents current timeline state.
5) UI controls playhead/time travel and settings.

## Timeline / History Strategy
- Keep a rolling buffer of snapshots (every N frames).
- Store deltas between snapshots (entity creates, updates, deletes).
- Allow jump-to-frame by loading nearest snapshot + replay deltas.
- Provide limits: max RAM usage or max history duration.

## Settings and Controls
- Backend selection (auto, Vulkan, Metal, D3D12).
- VSync on/off.
- FPS limit (power saving).
- Camera style: UE5 or Unity.
- Render quality presets (AA, shadows, GI).

## Open Decisions
- Renderer backend selection (bgfx vs wgpu vs custom).
- UI approach (ImGui + custom vs Avalonia vs Skia).
- Global illumination path (screen-space vs RT-based).

## Implementation Steps (Ordered)
1) **Repo + Solution Layout**
   - Create `A3A.Visualizer` (app), `A3A.Client` (library), `A3A.Protocol` (shared).
   - Add build scripts and CI skeleton.

2) **Windowing + Minimal Renderer**
   - Pick backend candidate and create a window + swapchain.
   - Clear screen, present frame, handle resize.

3) **Input + Camera**
   - Implement free-fly camera + UE5/Unity modes.
   - Basic input mappings and settings file.

4) **Scene Core**
   - Entities + transforms.
   - Mesh rendering (single mesh).
   - Basic material/shader pipeline.

5) **Client Protocol + IPC**
   - Define message schema and versioning.
   - Implement named pipe + Unix domain socket transport.
   - Sample client that drives a cube.

6) **Instancing + Large Data**
   - Instance buffer system.
   - Basic culling (frustum).

7) **Lighting + Shadows**
   - Add light types.
   - Shadow mapping + RT shadows (if backend supports).

8) **Post Processing Stack**
   - HDR pipeline + tone mapping.
   - FXAA/MSAA/TAA baseline.
   - DoF (bokeh).

9) **AO, Reflections, GI**
   - SSAO fallback.
   - SSR.
   - Decide GI path and integrate.

10) **Timeline / History**
   - Snapshot + delta storage.
   - UI controls for rewind/step.

11) **UI / UX Pass**
   - Modern overlay panels.
   - Scene list, perf stats, playback controls.

12) **Packaging + Docs**
   - Cross-platform build output.
   - Client usage docs and examples.

## Deliverables
- Design doc (this file).
- Initial implementation skeleton after design iteration.
- Example client demos (2D grid, maze, graph).
