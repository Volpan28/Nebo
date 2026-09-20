import { useEffect, useRef } from 'react';
import { useFrame } from '@react-three/fiber';
import * as THREE from 'three';
import { Billboard, Text, Line } from '@react-three/drei';
import { useAstroStore } from '../store/useAstroStore';
import { useSimClockStore } from '../store/useSimClockStore';
import { useCameraStore } from '../store/useCameraStore';
import { equatorialToUnitVector, buildEquatorialToHorizontalMatrix, localSiderealTimeDeg } from '../utils/equatorial';

const TargetReticle = ({ radius, scale }: { radius: number; scale: number }) => {
    const s = radius + (3 * Math.max(0.15, scale));
    const l = 5 * Math.max(0.15, scale);
    const reticleColor = "#00ffcc";

    const pointsTopLeft = [[-s, s - l, 0], [-s, s, 0], [-s + l, s, 0]] as [number, number, number][];
    const pointsTopRight = [[s - l, s, 0], [s, s, 0], [s, s - l, 0]] as [number, number, number][];
    const pointsBottomLeft = [[-s, -s + l, 0], [-s, -s, 0], [-s + l, -s, 0]] as [number, number, number][];
    const pointsBottomRight = [[s - l, -s, 0], [s, -s, 0], [s, -s + l, 0]] as [number, number, number][];

    return (
        <group>
            <Line points={pointsTopLeft} color={reticleColor} lineWidth={2} />
            <Line points={pointsTopRight} color={reticleColor} lineWidth={2} />
            <Line points={pointsBottomLeft} color={reticleColor} lineWidth={2} />
            <Line points={pointsBottomRight} color={reticleColor} lineWidth={2} />
        </group>
    );
};

export default function DynamicObjects() {
    const skyObjects = useAstroStore((state) => state.skyObjects);
    const fetchSkyMap = useAstroStore((state) => state.fetchSkyMap);
    const setTime = useAstroStore((state) => state.setTime);
    const selectedObject = useAstroStore((state) => state.selectedObject);
    const selectObject = useAstroStore((state) => state.selectObject);

    const simulatedTime = useSimClockStore((state) => state.simulatedTime);
    const fovDeg = useCameraStore((state) => state.fovDeg);
    const setAzAlt = useCameraStore((state) => state.setAzAlt);
    const groupRef = useRef<THREE.Group>(null);

    useEffect(() => {
        setTime(simulatedTime);
        const timeoutId = setTimeout(() => {
            fetchSkyMap();
        }, 300);
        return () => clearTimeout(timeoutId);
    }, [simulatedTime, setTime, fetchSkyMap]);

    useEffect(() => {
        if (groupRef.current) {
            groupRef.current.matrixAutoUpdate = false;
        }
    }, []);

    useFrame(() => {
        const { location } = useAstroStore.getState();
        const lstDeg = localSiderealTimeDeg(simulatedTime, location.longitude);
        const matrix = buildEquatorialToHorizontalMatrix(location.latitude, lstDeg);

        if (groupRef.current) {
            groupRef.current.matrix.copy(matrix);
            groupRef.current.matrixWorldNeedsUpdate = true;
        }
    });

    return (
        <group ref={groupRef}>
            {skyObjects.map((obj) => {
                const isOurMoon = obj.id.toLowerCase() === 'moon';
                const isPlanet = obj.category === 'Planet';
                const isSatellite = obj.category === 'Moon' && !isOurMoon;
                const isStar = obj.category.includes('Star');

                if (isSatellite && fovDeg > 10) {
                    return null;
                }

                const isSelected = selectedObject?.id === obj.id;
                const [x, y, z] = equatorialToUnitVector({
                    raHours: obj.rightAscension,
                    decDeg: obj.declination
                });

                const scale = fovDeg / 75;

                let baseRadius = 1.0;
                let color = "#ffffff";
                let minScale = 0.05;

                if (isOurMoon) {
                    baseRadius = 5.0;
                    color = "#e2e8f0";
                    minScale = 0.15; 
                } else if (isPlanet) {
                    baseRadius = 4.0;
                    color = "#fbbf24";
                    minScale = 0.15; 
                } else if (isSatellite) {
                    baseRadius = 1.5;
                    color = "#94a3b8";
                    minScale = 0.15; 
                } else if (isStar) {
                    baseRadius = 0.8;
                    color = "#ffffff";
                    minScale = 0.02; 
                }

                const currentRadius = baseRadius * Math.max(minScale, scale);
                const fontSize = 8 * Math.max(0.15, scale);
                const showText = isSelected || !isSatellite;

                return (
                    <Billboard key={obj.id} position={[x * 900, y * 900, z * 900]}>
                        <group
                            onClick={(e) => {
                                e.stopPropagation();
                                selectObject(obj);
                                setAzAlt(obj.azimuth, obj.altitude);
                            }}
                        >
                            <mesh>
                                <circleGeometry args={[currentRadius, 16]} />
                                <meshBasicMaterial color={color} />
                            </mesh>

                            {isSelected && <TargetReticle radius={currentRadius} scale={scale} />}

                            {showText && (
                                <Text
                                    position={[0, currentRadius + (2 * scale), 0]}
                                    anchorY="bottom"
                                    fontSize={fontSize}
                                    color={isSelected ? "#00ffcc" : "white"}
                                >
                                    {obj.name}
                                </Text>
                            )}
                        </group>
                    </Billboard>
                );
            })}
        </group>
    );
}