import { Canvas } from '@react-three/fiber';
import { Billboard, Text } from '@react-three/drei';
import { altAzToCartesian } from '../utils/coordinates';
import CameraRig from '../three/CameraRig';
import StarField from '../three/StarField';
import DynamicObjects from "../three/DynamicObjects.tsx";


const SKY_RADIUS = 1000;

const CARDINAL_POINTS = [
  { label: 'N', azimuth: 0 },
  { label: 'E', azimuth: 90 },
  { label: 'S', azimuth: 180 },
  { label: 'W', azimuth: 270 },
];

// Inverted sphere so the deep-blue sky wraps the whole world (cheap: one draw call, no shader).
const SkyBackground = () => (
    <mesh scale={[-1, -1, -1]}>
      <sphereGeometry args={[SKY_RADIUS, 32, 32]} />
      <meshBasicMaterial color="#060b14" />
    </mesh>
);

// Massive flat circle instead of a cylinder, so there is no rim/band exposing the sky sphere behind it.
const Ground = () => (
    <mesh rotation={[-Math.PI / 2, 0, 0]} position={[0, -0.01, 0]}>
      <circleGeometry args={[2000, 32]} />
      <meshBasicMaterial color="#020402" />
    </mesh>
);

const CardinalPoints = () => (
    <>
      {CARDINAL_POINTS.map(({ label, azimuth }) => {
        const { x, z } = altAzToCartesian(0, azimuth, 90);
        return (
            <Billboard key={label} position={[x, 1, z]}>
              <Text fontSize={3} color="#ff3333" anchorX="center" anchorY="middle" fillOpacity={0.8}>
                {label}
              </Text>
            </Billboard>
        );
      })}
    </>
);

const SkyCanvas = () => {
    return (
        <div className="absolute inset-0 w-full h-full">
            <Canvas camera={{ position: [0, 0, 0], fov: 75, near: 0.01, far: 5000 }}>
                <CameraRig />
                <ambientLight intensity={0.3} />

                <SkyBackground />
                <StarField />
                <DynamicObjects /> {/* Додано рендер вирахуваних об'єктів */}
                <Ground />
                <CardinalPoints />
            </Canvas>
        </div>
    );
};

export default SkyCanvas;
