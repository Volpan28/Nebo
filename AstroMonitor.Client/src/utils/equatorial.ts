import { Matrix4 } from 'three';
import { SiderealTime } from 'astronomy-engine';

export interface EquatorialCoord {
  raHours: number;
  decDeg: number;
}

export interface HorizontalCoord {
  altDeg: number;
  azDeg: number;
}

const DEG2RAD = Math.PI / 180;
const RAD2DEG = 180 / Math.PI;

/**
 * Fixed unit vector for a star in the equatorial (RA/Dec) frame. Computed once per
 * star at catalog-load time — never recomputed per frame.
 */
export function equatorialToUnitVector({ raHours, decDeg }: EquatorialCoord): [number, number, number] {
  const alpha = raHours * 15 * DEG2RAD;
  const delta = decDeg * DEG2RAD;
  return [
    Math.cos(delta) * Math.cos(alpha),
    Math.cos(delta) * Math.sin(alpha),
    Math.sin(delta),
  ];
}

/** Local sidereal time in degrees, via astronomy-engine's GAST + observer longitude. */
export function localSiderealTimeDeg(date: Date, longitudeDeg: number): number {
  const gastHours = SiderealTime(date);
  const lstHours = (((gastHours + longitudeDeg / 15) % 24) + 24) % 24;
  return lstHours * 15;
}

/**
 * Single rotation matrix mapping a fixed equatorial-frame unit vector (see
 * equatorialToUnitVector) directly into the three.js horizontal-frame convention
 * mandated by CLAUDE.md (Y up, azimuth 0 = North = -Z, azimuth 90 = East = +X).
 *
 * Derived by composing: equatorial -> hour-angle frame (rotate by LST around the
 * polar axis) -> horizontal frame (rotate by colatitude around the East axis).
 * Applying this to a whole group's transform (rather than per-star) is what lets
 * ~10k+ stars be rendered without any per-frame CPU work per star — three.js
 * already multiplies every vertex by the group's modelMatrix for free.
 *
 * Verified against equatorialToHorizontalSphericalTrig for known cases (zenith,
 * NCP/Polaris, meridian transit, rise/set) — see equatorial.test.ts.
 */
export function buildEquatorialToHorizontalMatrix(latitudeDeg: number, lstDeg: number): Matrix4 {
  const phi = latitudeDeg * DEG2RAD;
  const theta = lstDeg * DEG2RAD;
  const sinPhi = Math.sin(phi);
  const cosPhi = Math.cos(phi);
  const sinTheta = Math.sin(theta);
  const cosTheta = Math.cos(theta);

  const matrix = new Matrix4();
  // Matrix4.set() takes arguments in row-major order.
  matrix.set(
    -sinTheta, cosTheta, 0, 0,
    cosPhi * cosTheta, cosPhi * sinTheta, sinPhi, 0,
    sinPhi * cosTheta, sinPhi * sinTheta, -cosPhi, 0,
    0, 0, 0, 1,
  );
  return matrix;
}

/**
 * Traditional spherical-trig RA/Dec -> Alt/Az formula. Kept as an independent
 * implementation from buildEquatorialToHorizontalMatrix so a bug in the matrix
 * path is caught by comparison rather than by comparing a formula to itself.
 * HA = LST - RA; sin(alt) = sin(dec)sin(lat) + cos(dec)cos(lat)cos(HA).
 */
export function equatorialToHorizontalSphericalTrig(
  { raHours, decDeg }: EquatorialCoord,
  latitudeDeg: number,
  lstDeg: number,
): HorizontalCoord {
  const alpha = raHours * 15 * DEG2RAD;
  const delta = decDeg * DEG2RAD;
  const phi = latitudeDeg * DEG2RAD;
  const theta = lstDeg * DEG2RAD;
  const hourAngle = theta - alpha;

  const sinAlt = Math.sin(phi) * Math.sin(delta) + Math.cos(phi) * Math.cos(delta) * Math.cos(hourAngle);
  const altDeg = Math.asin(sinAlt) * RAD2DEG;

  const azRad = Math.atan2(
    -Math.cos(delta) * Math.sin(hourAngle),
    Math.sin(delta) * Math.cos(phi) - Math.cos(delta) * Math.sin(phi) * Math.cos(hourAngle),
  );
  const azDeg = ((azRad * RAD2DEG) + 360) % 360;

  return { altDeg, azDeg };
}
