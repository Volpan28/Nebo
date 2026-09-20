import { useEffect, useRef } from 'react';
import { useFrame } from '@react-three/fiber';
import * as THREE from 'three';
import { Billboard, Text, Line } from '@react-three/drei';
import { useAstroStore } from '../store/useAstroStore';
import { useSimClockStore } from '../store/useSimClockStore';
import { useCameraStore } from '../store/useCameraStore';
import { equatorialToUnitVector, buildEquatorialToHorizontalMatrix, localSiderealTimeDeg } from '../utils/equatorial';

const TargetReticle = () => {
    const s = 12;
    const l = 6;

    const pointsTopLeft = [[-s, s - l, 0], [-s, s, 0], [-s + l, s, 0]] as [number, number, number][];
    const pointsTopRight = [[s - l, s, 0], [s, s, 0], [s, s - l, 0]] as [number, number, number][];
    const pointsBottomLeft = [[-s, -s + l, 0], [-s, -s, 0], [-s + l, -s, 0]] as [number, number, number][];
    const pointsBottomRight = [[s - l, -s, 0], [s, -s, 0], [s, -s + l, 0]] as [number, number, number][];

    return (
        <group>
            <Line points={pointsTopLeft} color="#00ffcc" lineWidth={2} />
            <Line points={pointsTopRight} color="#00ffcc" lineWidth={2} />
            <Line points={pointsBottomLeft} color="#00ffcc" lineWidth={2} />
            <Line points={pointsBottomRight} color="#00ffcc" lineWidth={2} />
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
    }, [] );

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

                if (obj.category === 'Moon' && !isOurMoon && fovDeg > 15) {
                    return null;
                }

                const isSelected = selectedObject?.id === obj.id;
                const [x, y, z] = equatorialToUnitVector({
                    raHours: obj.rightAscension,
                    decDeg: obj.declination
                });

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
                                <circleGeometry args={[isOurMoon ? 8 : 5, 16]} />
                                <meshBasicMaterial color={isOurMoon ? "#ffffff" : "#00ffcc"} />
                            </mesh>

                            {isSelected && <TargetReticle />}

                            <Text
                                position={[0, isOurMoon ? 14 : 12, 0]}
                                fontSize={isOurMoon ? 10 : 8}
                                color={isSelected ? "#00ffcc" : "white"}
                            >
                                {obj.name}
                            </Text>
                        </group>
                    </Billboard>
                );
            })}
        </group>
    );
}