import { describe, expect, it } from 'vitest';
import { Horizon, Observer } from 'astronomy-engine';
import { Vector3 } from 'three';
import { altAzToCartesian } from './coordinates';
import {
  buildEquatorialToHorizontalMatrix,
  equatorialToHorizontalSphericalTrig,
  equatorialToUnitVector,
  localSiderealTimeDeg,
} from './equatorial';

const DEFAULT_LOCATION = { latitude: 49.83, longitude: 24.02 };

describe('equatorialToHorizontalSphericalTrig — hand-derived algebraic identities', () => {
  it('places an object at the zenith when RA = LST and Dec = latitude', () => {
    const { altDeg } = equatorialToHorizontalSphericalTrig(
      { raHours: 10, decDeg: 45 },
      45,
      150, // 10h * 15 = 150deg, so RA == LST
    );
    expect(altDeg).toBeCloseTo(90, 6);
  });

  it('places the North Celestial Pole at altitude = latitude, azimuth = North (0/360)', () => {
    const { altDeg, azDeg } = equatorialToHorizontalSphericalTrig(
      { raHours: 5, decDeg: 90 },
      49.83,
      123.4,
    );
    expect(altDeg).toBeCloseTo(49.83, 6);
    expect(azDeg === 0 || Math.abs(azDeg - 360) < 1e-6).toBe(true);
  });

  it('places an equatorial object at meridian transit at altitude = 90 - latitude, azimuth = South (180)', () => {
    const { altDeg, azDeg } = equatorialToHorizontalSphericalTrig(
      { raHours: 8, decDeg: 0 },
      45,
      120, // RA == LST -> hour angle 0 -> transit
    );
    expect(altDeg).toBeCloseTo(45, 6);
    expect(azDeg).toBeCloseTo(180, 6);
  });

  it('places an equatorial object rising/setting at altitude = 0, azimuth = East/West (90/270)', () => {
    const rising = equatorialToHorizontalSphericalTrig({ raHours: 8, decDeg: 0 }, 45, 120 - 90);
    const setting = equatorialToHorizontalSphericalTrig({ raHours: 8, decDeg: 0 }, 45, 120 + 90);
    expect(rising.altDeg).toBeCloseTo(0, 6);
    expect(rising.azDeg).toBeCloseTo(90, 6);
    expect(setting.altDeg).toBeCloseTo(0, 6);
    expect(setting.azDeg).toBeCloseTo(270, 6);
  });
});

describe('Polaris sanity check (CLAUDE.md-mandated)', () => {
  const polaris = { raHours: 2.5303, decDeg: 89.264 };

  it('keeps Polaris altitude close to the observer latitude and nearly time-invariant over several hours', () => {
    const baseDate = new Date('2024-06-21T00:00:00Z');
    const altitudes: number[] = [];
    const azimuths: number[] = [];

    for (let hourOffset = 0; hourOffset <= 6; hourOffset += 1) {
      const date = new Date(baseDate.getTime() + hourOffset * 60 * 60 * 1000);
      const lstDeg = localSiderealTimeDeg(date, DEFAULT_LOCATION.longitude);
      const { altDeg, azDeg } = equatorialToHorizontalSphericalTrig(polaris, DEFAULT_LOCATION.latitude, lstDeg);
      altitudes.push(altDeg);
      azimuths.push(azDeg);
    }

    for (const altDeg of altitudes) {
      expect(Math.abs(altDeg - DEFAULT_LOCATION.latitude)).toBeLessThan(1.0);
    }
    expect(Math.max(...altitudes) - Math.min(...altitudes)).toBeLessThan(1.0);
    for (const azDeg of azimuths) {
      expect(azDeg < 5 || azDeg > 355).toBe(true);
    }
  });
});

describe('cross-check against astronomy-engine Horizon()', () => {
  const date = new Date('2024-06-21T21:00:00Z');
  const observer = new Observer(DEFAULT_LOCATION.latitude, DEFAULT_LOCATION.longitude, 0);
  const lstDeg = localSiderealTimeDeg(date, DEFAULT_LOCATION.longitude);

  const stars = [
    { name: 'Sirius', raHours: 6.7525, decDeg: -16.7161 },
    { name: 'Betelgeuse', raHours: 5.9195, decDeg: 7.4071 },
  ];

  it.each(stars)('matches astronomy-engine Horizon() for $name within a tight tolerance', ({ raHours, decDeg }) => {
    const expected = Horizon(date, observer, raHours, decDeg, undefined);
    const actual = equatorialToHorizontalSphericalTrig({ raHours, decDeg }, DEFAULT_LOCATION.latitude, lstDeg);

    // Both paths use unprecessed J2000 RA/Dec and the same astronomy-engine-derived
    // LST, so they should agree far tighter than the "skips precession/nutation"
    // caveat would otherwise require — a looser tolerance is used defensively.
    expect(Math.abs(actual.altDeg - expected.altitude)).toBeLessThan(0.05);
    const azDiff = Math.min(Math.abs(actual.azDeg - expected.azimuth), 360 - Math.abs(actual.azDeg - expected.azimuth));
    expect(azDiff).toBeLessThan(0.05);
  });
});

describe('rotation matrix vs spherical-trig oracle (regression test for the render pipeline)', () => {
  const latitude = 49.83;
  const lstDeg = 123.5;

  const fixtures: Array<{ raHours: number; decDeg: number }> = [
    { raHours: 0, decDeg: 0 },
    { raHours: 6.7525, decDeg: -16.7161 },
    { raHours: 5.9195, decDeg: 7.4071 },
    { raHours: 2.5303, decDeg: 89.264 },
    { raHours: 18, decDeg: -30 },
    { raHours: 12, decDeg: 60 },
    { raHours: 23.5, decDeg: -75 },
  ];

  it.each(fixtures)('agrees with altAzToCartesian for RA=$raHours Dec=$decDeg', ({ raHours, decDeg }) => {
    const { altDeg, azDeg } = equatorialToHorizontalSphericalTrig({ raHours, decDeg }, latitude, lstDeg);
    const expected = altAzToCartesian(altDeg, azDeg, 1);

    const unitVector = equatorialToUnitVector({ raHours, decDeg });
    const matrix = buildEquatorialToHorizontalMatrix(latitude, lstDeg);
    const actual = new Vector3(...unitVector).applyMatrix4(matrix);

    expect(actual.x).toBeCloseTo(expected.x, 4);
    expect(actual.y).toBeCloseTo(expected.y, 4);
    expect(actual.z).toBeCloseTo(expected.z, 4);
  });
});
