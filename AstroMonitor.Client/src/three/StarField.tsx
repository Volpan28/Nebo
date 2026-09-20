import { useEffect, useMemo, useRef } from 'react';
import { useFrame, useThree } from '@react-three/fiber';
import * as THREE from 'three';
import { useStarCatalogStore } from '../store/useStarCatalogStore';
import { useAstroStore } from '../store/useAstroStore';
import { useSimClockStore } from '../store/useSimClockStore';
import { useCameraStore } from '../store/useCameraStore';
import { buildEquatorialToHorizontalMatrix, localSiderealTimeDeg } from '../utils/equatorial';
import { bvToRgb } from '../utils/starColor';

const BASE_FOV_DEG = 75;
const MIN_POINT_PX = 1;
const MAX_POINT_PX = 8;

const vertexShader = `
  attribute float pointSize;
  attribute vec3 starColor;
  uniform float fovScale;
  uniform float pixelRatio;
  varying vec3 vColor;
  varying float vHeight;
  void main() {
    vColor = starColor;
    vec4 worldPosition = modelMatrix * vec4(position, 1.0);
    vHeight = worldPosition.y;
    gl_Position = projectionMatrix * modelViewMatrix * vec4(position, 1.0);
    gl_PointSize = pointSize * fovScale * pixelRatio;
  }
`;

const fragmentShader = `
  varying vec3 vColor;
  varying float vHeight;
  void main() {
    if (vHeight < 0.0) discard;
    vec2 centered = gl_PointCoord - vec2(0.5);
    float d = length(centered) * 2.0;
    float alpha = exp(-4.0 * d * d);
    if (alpha < 0.02) discard;
    gl_FragColor = vec4(vColor, alpha);
  }
`;

/**
 * Renders the whole star catalog as a single Points draw call. Each star's position
 * is its FIXED equatorial-frame unit vector (set once, never touched again); the
 * parent group's rotation matrix (equatorial -> horizontal, recomputed every frame
 * from local sidereal time + observer latitude) does the actual sky rotation via
 * three.js's standard modelMatrix multiply — no per-star CPU work per frame.
 */
export default function StarField() {
  const stars = useStarCatalogStore((state) => state.stars);
  const { gl } = useThree();
  const groupRef = useRef<THREE.Group>(null);
  const materialRef = useRef<THREE.ShaderMaterial>(null);

  const geometry = useMemo(() => {
    const geo = new THREE.BufferGeometry();
    const count = stars.length;
    const positions = new Float32Array(count * 3);
    const colors = new Float32Array(count * 3);
    const sizes = new Float32Array(count);

    stars.forEach((star, i) => {
      positions[i * 3] = star.eqVector[0];
      positions[i * 3 + 1] = star.eqVector[1];
      positions[i * 3 + 2] = star.eqVector[2];

      const [r, g, b] = bvToRgb(star.colorIndex);
      colors[i * 3] = r;
      colors[i * 3 + 1] = g;
      colors[i * 3 + 2] = b;

      sizes[i] = Math.max(MIN_POINT_PX, Math.min(MAX_POINT_PX, 6.5 - star.magnitude));
    });

    geo.setAttribute('position', new THREE.BufferAttribute(positions, 3));
    geo.setAttribute('starColor', new THREE.BufferAttribute(colors, 3));
    geo.setAttribute('pointSize', new THREE.BufferAttribute(sizes, 1));
    return geo;
  }, [stars]);

  useEffect(() => {
    if (groupRef.current) {
      groupRef.current.matrixAutoUpdate = false;
    }
  }, []);

  useFrame(() => {
    const { location } = useAstroStore.getState();
    const { simulatedTime } = useSimClockStore.getState();
    const { fovDeg } = useCameraStore.getState();

    const lstDeg = localSiderealTimeDeg(simulatedTime, location.longitude);
    const matrix = buildEquatorialToHorizontalMatrix(location.latitude, lstDeg);

    if (groupRef.current) {
      groupRef.current.matrix.copy(matrix);
      groupRef.current.matrixWorldNeedsUpdate = true;
    }

    if (materialRef.current) {
      const fovScale =
        Math.tan((BASE_FOV_DEG * Math.PI) / 360) / Math.tan((fovDeg * Math.PI) / 360);
      materialRef.current.uniforms.fovScale.value = fovScale;
      materialRef.current.uniforms.pixelRatio.value = gl.getPixelRatio();
    }
  });

  return (
    <group ref={groupRef}>
      <points geometry={geometry} frustumCulled={false}>
        <shaderMaterial
          ref={materialRef}
          vertexShader={vertexShader}
          fragmentShader={fragmentShader}
          uniforms={{ fovScale: { value: 1 }, pixelRatio: { value: 1 } }}
          transparent
          depthWrite={false}
          depthTest={false}
          blending={THREE.AdditiveBlending}
        />
      </points>
    </group>
  );
}
