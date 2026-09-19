export interface Vector3Tuple {
  x: number;
  y: number;
  z: number;
}

/**
 * Converts horizontal (Alt/Az) coordinates to Three.js Cartesian coordinates.
 * Formula is fixed by the API contract in CLAUDE.md — do not modify.
 */
export function altAzToCartesian(
  altitude: number,
  azimuth: number,
  radius = 100,
): Vector3Tuple {
  const altRad = altitude * (Math.PI / 180);
  const azRad = azimuth * (Math.PI / 180);

  const x = radius * Math.cos(altRad) * Math.sin(azRad);
  const y = radius * Math.sin(altRad);
  const z = -radius * Math.cos(altRad) * Math.cos(azRad);

  return { x, y, z };
}
