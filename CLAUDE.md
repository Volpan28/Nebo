\# AstroMonitor Frontend Architecture \& Rules



\## Tech Stack

\- Framework: React 18 + TypeScript + Vite

\- Styling: Tailwind CSS (for UI overlays)

\- 3D Engine: @react-three/fiber, @react-three/drei, three

\- State Management: Zustand (Crucial for 3D performance, keep UI state out of React Context to avoid Canvas re-renders)

\- Date manipulation: date-fns



\## Project Goal

Build a web-based planetarium (similar to Stellarium) showing a 3D sky vault from a specific location on Earth. 



\## API Contract

The backend provides a single endpoint for the sky map:

`GET /api/skymap?latitude={lat}\&longitude={lon}\&observationDate={isoString}`



Response is an array of `SkyMapItemDto`:

```typescript

interface SkyMapItemDto {

&#x20; id: string;

&#x20; name: string;

&#x20; category: 'Planet' | 'Moon' | 'Deep Sky Object' | 'Star' | string; // Stars can be 'Star\_Ori', etc.

&#x20; altitude: number; // Degrees (0 to 90)

&#x20; azimuth: number; // Degrees (0 to 360)

&#x20; magnitude: number; // For sizing/brightness

&#x20; textureUrl: string;

}



\## 3D Coordinate Math (Strict Rule)

Backend returns Altitude and Azimuth in degrees. Do NOT invent your own formulas. Use this precise conversion to Three.js Cartesian coordinates (where Y is UP):

```typescript

const radius = 100;

const altRad = altitude \* (Math.PI / 180);

const azRad = azimuth \* (Math.PI / 180);



// In Three.js: Y is up. Z is towards the viewer.

// Azimuth 0 (North) should be -Z. Azimuth 90 (East) should be +X.

const x = radius \* Math.cos(altRad) \* Math.sin(azRad);

const y = radius \* Math.sin(altRad);

const z = -radius \* Math.cos(altRad) \* Math.cos(azRad);

```



\## UI Rules

\- The 3D Canvas must take up 100% of the screen (100vw, 100vh).

\- UI elements (Time controls, Info panels) must be absolutely positioned div overlays on top of the Canvas.

\- Use dark mode styles with semi-transparent black/blur backgrounds for panels (bg-black/70 backdrop-blur-md).


## Core concept (most important)
This is NOT a solar-system model with real distances. It is a view from INSIDE the celestial sphere:
- Camera sits at the origin (the observer). It only rotates (azimuth/altitude, altitude clamped to ±90°) and changes FOV (0.5°–120°). It never translates.
- Every object (stars, Sun, Moon, planets, deep-sky objects) is placed on one unit sphere by DIRECTION only. No real distances, no real sizes. Disable depth test; order by render order/priority.
- Pipeline: catalog RA/Dec (J2000) → unit vector → ONE rotation matrix built from local sidereal time + observer latitude → horizontal frame (az/alt) → screen. Pass this matrix as a uniform (vertex shader) so ~10k+ stars are not recomputed on the CPU per frame. Sun/Moon/planets are recomputed about once per second with an astronomy library (e.g. astronomy-engine, or what the project already uses).
- Projection: prefer stereographic (as in Stellarium). If that is too costly in the current stack, use perspective with FOV capped at ~100°.
- Zoom = FOV change around the view center. Drag = rotate with light inertia. Wheel/pinch = smooth exponential zoom. Touch supported.

## Visual rules (minimal, calm, readable)
- Background: near-black with a very subtle darker-to-lighter gradient toward the horizon. Sky brightness follows Sun altitude (day / civil / nautical / astronomical twilight / night); stars fade out in daylight.
- Horizon: dark ground with slight transparency and soft edge; N/E/S/W labels on the horizon (N slightly emphasized). Objects below the horizon are hidden while ground is on.
- Stars: single draw call, point sprites with soft Gaussian glow, additive blending. Size and brightness come from a smooth, monotonic function of magnitude (clamp roughly 1–8 px). Color from B–V index, desaturated (warm orange → white → pale blue). Default catalog: mag ≤ 6.5 (HYG/Hipparcos); load fainter stars only when zoomed in, if data allows. Limiting magnitude grows as FOV shrinks.
- Sun, Moon, planets: fixed minimum pixel size (Moon ≥ ~14 px) that grows when zoomed in; Moon shows correct phase and terminator orientation; planets are colored dots slightly larger than equally bright stars.
- Constellation lines: 1 px, ~25% opacity, toggleable; names toggleable. Az/alt and equatorial grids: off by default.
- Labels (canvas or HTML overlay): 12 px, subtle text shadow, no boxes. Always show Sun, Moon, planets. Show only the ~25 brightest named stars at wide FOV, more as FOV shrinks. Priority-based greedy collision avoidance, 150 ms fade in/out. Labels must never overlap.
- Selection: click selects nearest object within ~12 px, shown by a thin ring marker.

## UI rules (Stellarium-like, but even quieter)
- Default screen = full-screen sky + tiny top-left text: location, lat/lon, local date-time, sky state ("Night", "Nautical twilight"...).
- Bottom toolbar of icon-only buttons with tooltips, auto-hides after ~3 s of mouse inactivity: constellation lines, constellation names, az grid, equatorial grid, ground/atmosphere, labels, night (red) mode, search, location, "Visible now".
- Time control (bottom center): ◀◀ ◀ ▶ ▶▶ | Now, with a speed indicator (×1, ×10, ×60, ×3600...). Click the time to open a date-time picker. Space = pause. Simulated clock is separate from real time.
- Info panel (right, small, Esc closes): name, type, magnitude, constellation, az/alt, RA/Dec, rise/transit/set in local time, and "Visible now: yes/no + reason" (below horizon / daylight / too faint).
- "Visible now" panel: list of objects visible at the simulated time and observer location, grouped (Sun/Moon, Planets, Brightest stars, Deep-sky), sorted by brightness, showing alt/az. Click = smooth slew to the object and select it. Refresh at ≤1 Hz and when time/location changes.
- Visibility rule (keep constants configurable in one place): altitude > 0° AND magnitude ≤ limiting magnitude by Sun altitude: day (>-0.83°) → -4, civil twilight (to -6°) → 1.5, nautical (to -12°) → 3.5, astronomical (to -18°) → 5.0, night → 6.0.
- Style: panels rgba(10,14,24,.6) with backdrop blur, 1 px subtle border, 8 px radius, system font 12–13 px, ONE soft blue accent, no heavy shadows or gradients. Night mode tints the whole UI and sky red.
- Keyboard: arrows pan, +/- zoom, Space pause, / or Ctrl+F search, Esc close panels.

## Phases (stop after each one and wait for my review)
1. Core sky: inside-sphere camera, correct coordinate pipeline, stars, horizon + cardinals, simulated clock, observer location. Add unit tests for the coordinate conversion and sanity checks (e.g. Polaris altitude ≈ latitude and barely moves; a known star/planet az/alt at a known time and place matches astronomy-engine).
2. Sun, Moon, planets, labels with collision avoidance, constellation lines.
3. UI: toolbar, time controls, info panel, Visible-now list, search.
4. Polish: twilight/sky gradient, red mode, touch, performance (60 fps target, no per-frame allocations, cap devicePixelRatio at 2).


