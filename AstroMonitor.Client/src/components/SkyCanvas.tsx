import { OrbitControls, Sphere, Stars } from '@react-three/drei';
import { Canvas, type ThreeEvent } from '@react-three/fiber';
import { useMemo } from 'react';
import { useAstroStore } from '../store/useAstroStore';
import type { SkyMapItemDto } from '../types/skyMap';
import { altAzToCartesian } from '../utils/coordinates';

const SKY_RADIUS = 100;

function clamp(value: number, min: number, max: number): number {
  return Math.min(max, Math.max(min, value));
}

function getObjectRadius(category: string, magnitude: number): number {
  const isPlanetOrMoon = category === 'Planet' || category === 'Moon';
  if (isPlanetOrMoon) {
    return clamp(3 - magnitude * 0.6, 0.8, 4);
  }
  // Stars and Deep Sky Objects render as small points of light.
  return clamp(1.1 - magnitude * 0.18, 0.08, 1.2);
}

function getObjectColor(category: string): string {
  switch (category) {
    case 'Planet':
      return '#e8b978';
    case 'Moon':
      return '#d8d8d8';
    case 'Deep Sky Object':
      return '#a78bfa';
    default:
      return '#ffffff';
  }
}

interface SkyObjectMeshProps {
  object: SkyMapItemDto;
  isSelected: boolean;
  onSelect: (object: SkyMapItemDto) => void;
}

function SkyObjectMesh({ object, isSelected, onSelect }: SkyObjectMeshProps) {
  const position = useMemo(
    () => altAzToCartesian(object.altitude, object.azimuth, SKY_RADIUS),
    [object.altitude, object.azimuth],
  );
  const radius = getObjectRadius(object.category, object.magnitude);
  const color = isSelected ? '#38bdf8' : getObjectColor(object.category);

  const handleClick = (event: ThreeEvent<MouseEvent>) => {
    event.stopPropagation();
    onSelect(object);
  };

  return (
    <Sphere
      args={[radius, 16, 16]}
      position={[position.x, position.y, position.z]}
      onClick={handleClick}
      onPointerOver={(event) => {
        event.stopPropagation();
        document.body.style.cursor = 'pointer';
      }}
      onPointerOut={() => {
        document.body.style.cursor = 'default';
      }}
    >
      <meshBasicMaterial color={color} />
    </Sphere>
  );
}

function GroundPlane() {
  return (
    <mesh rotation={[-Math.PI / 2, 0, 0]} position={[0, -0.05, 0]}>
      <planeGeometry args={[500, 500]} />
      <meshBasicMaterial color="#05050a" />
    </mesh>
  );
}

export default function SkyCanvas() {
  const skyObjects = useAstroStore((state) => state.skyObjects);
  const selectedObject = useAstroStore((state) => state.selectedObject);
  const selectObject = useAstroStore((state) => state.selectObject);

  return (
    <div className="absolute inset-0 h-full w-full -z-10 bg-black">
      <Canvas camera={{ position: [0, 0, 0.1], fov: 75, near: 0.1, far: 1000 }}>
        <color attach="background" args={['#000008']} />
        <ambientLight intensity={0.5} />
        <directionalLight position={[0, 10, 0]} intensity={1} />
        <Stars radius={SKY_RADIUS} depth={50} count={5000} factor={4} saturation={0} fade />
        <GroundPlane />
        {skyObjects.map((object) => (
          <SkyObjectMesh
            key={object.id}
            object={object}
            isSelected={selectedObject?.id === object.id}
            onSelect={selectObject}
          />
        ))}
        <OrbitControls
          target={[0, 0, 0]}
          maxPolarAngle={Math.PI / 2 - 0.05}
          enableZoom={false}
          enablePan={false}
        />
      </Canvas>
    </div>
  );
}
