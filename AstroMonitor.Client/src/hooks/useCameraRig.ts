import { useEffect, useRef } from 'react';
import { useFrame, useThree } from '@react-three/fiber';
import type { PerspectiveCamera } from 'three';
import { useCameraStore } from '../store/useCameraStore';
import { altAzToCartesian } from '../utils/coordinates';

const DRAG_SENSITIVITY = 0.15; // degrees per pixel, at the default fov
const BASE_FOV = 75;
const INERTIA_DECAY = 0.9;
const ZOOM_SENSITIVITY = 0.0015;

/**
 * Rotate-only, first-person planetarium camera rig. Replaces drei's OrbitControls,
 * which is a translate/dolly-around-target model that doesn't fit "camera fixed at
 * the observer, only rotates + changes FOV, never translates" (CLAUDE.md).
 *
 * Must be called from a component rendered inside <Canvas>.
 */
export function useCameraRig() {
  const { camera, gl } = useThree();
  const velocityRef = useRef({ az: 0, alt: 0 });
  const isDraggingRef = useRef(false);
  const lastPointerRef = useRef({ x: 0, y: 0 });

  useEffect(() => {
    const element = gl.domElement;

    const handlePointerDown = (event: PointerEvent) => {
      isDraggingRef.current = true;
      velocityRef.current = { az: 0, alt: 0 };
      lastPointerRef.current = { x: event.clientX, y: event.clientY };
      element.setPointerCapture(event.pointerId);
    };

    const handlePointerMove = (event: PointerEvent) => {
      if (!isDraggingRef.current) return;
      const dx = event.clientX - lastPointerRef.current.x;
      const dy = event.clientY - lastPointerRef.current.y;
      lastPointerRef.current = { x: event.clientX, y: event.clientY };

      const { fovDeg, azimuthDeg, altitudeDeg, setAzAlt } = useCameraStore.getState();
      const scale = (fovDeg / BASE_FOV) * DRAG_SENSITIVITY;
      const deltaAz = -dx * scale;
      const deltaAlt = dy * scale;

      velocityRef.current = { az: deltaAz, alt: deltaAlt };
      setAzAlt(azimuthDeg + deltaAz, altitudeDeg + deltaAlt);
    };

    const handlePointerUp = (event: PointerEvent) => {
      isDraggingRef.current = false;
      element.releasePointerCapture(event.pointerId);
    };

    const handleWheel = (event: WheelEvent) => {
      event.preventDefault();
      const factor = Math.exp(event.deltaY * ZOOM_SENSITIVITY);
      useCameraStore.getState().zoomBy(factor);
    };

    element.addEventListener('pointerdown', handlePointerDown);
    element.addEventListener('pointermove', handlePointerMove);
    element.addEventListener('pointerup', handlePointerUp);
    element.addEventListener('pointercancel', handlePointerUp);
    element.addEventListener('wheel', handleWheel, { passive: false });

    return () => {
      element.removeEventListener('pointerdown', handlePointerDown);
      element.removeEventListener('pointermove', handlePointerMove);
      element.removeEventListener('pointerup', handlePointerUp);
      element.removeEventListener('pointercancel', handlePointerUp);
      element.removeEventListener('wheel', handleWheel);
    };
  }, [gl]);

  useFrame(() => {
    if (!isDraggingRef.current) {
      const { az, alt } = velocityRef.current;
      if (Math.abs(az) > 0.001 || Math.abs(alt) > 0.001) {
        const { azimuthDeg, altitudeDeg, setAzAlt } = useCameraStore.getState();
        setAzAlt(azimuthDeg + az, altitudeDeg + alt);
        velocityRef.current = { az: az * INERTIA_DECAY, alt: alt * INERTIA_DECAY };
      }
    }

    const { azimuthDeg, altitudeDeg, fovDeg } = useCameraStore.getState();

    // Reuse the exact same alt/az -> XYZ convention used to place every sky object,
    // so "look at this direction" and "object is at this direction" are always consistent.
    const forward = altAzToCartesian(altitudeDeg, azimuthDeg, 1);
    camera.up.set(0, 1, 0);
    camera.lookAt(forward.x, forward.y, forward.z);

    const perspectiveCamera = camera as PerspectiveCamera;
    if (perspectiveCamera.fov !== fovDeg) {
      perspectiveCamera.fov = fovDeg;
      perspectiveCamera.updateProjectionMatrix();
    }
  });
}
