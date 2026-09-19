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

