/**
 * Maps a B-V color index to a desaturated star color (warm orange -> white -> pale blue),
 * computed once per star at catalog-load time.
 */
export function bvToRgb(colorIndex: number | null | undefined): [number, number, number] {
  const bv = Math.max(-0.4, Math.min(2.0, colorIndex ?? 0.65));

  // Piecewise-linear approximation of blackbody-ish star color, desaturated toward white
  // so faint dots don't read as garish on a dark background.
  let r: number;
  let g: number;
  let b: number;

  if (bv < 0) {
    const t = (bv + 0.4) / 0.4;
    r = 0.65 + 0.2 * t;
    g = 0.75 + 0.2 * t;
    b = 1.0;
  } else if (bv < 0.4) {
    const t = bv / 0.4;
    r = 0.85 + 0.15 * t;
    g = 0.95 + 0.05 * t;
    b = 1.0 - 0.15 * t;
  } else {
    const t = Math.min(1, (bv - 0.4) / 1.6);
    r = 1.0;
    g = 1.0 - 0.25 * t;
    b = 0.85 - 0.55 * t;
  }

  // Desaturate toward white so colors stay subtle at small point sizes.
  const desaturation = 0.35;
  r = r + (1 - r) * desaturation;
  g = g + (1 - g) * desaturation;
  b = b + (1 - b) * desaturation;

  return [r, g, b];
}
