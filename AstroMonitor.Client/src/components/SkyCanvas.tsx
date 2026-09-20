import { Canvas } from '@react-three/fiber';
import { Billboard, OrbitControls, Stars, Text } from '@react-three/drei';
import { useAstroStore } from '../store/useAstroStore';

const RADIUS = 100;
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
        const azRad = azimuth * (Math.PI / 180);
        const x = 90 * Math.sin(azRad);
        const z = -90 * Math.cos(azRad);
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

const CelestialObject = ({ obj, onClick }: { obj: any, onClick: () => void }) => {
  const altRad = obj.altitude * (Math.PI / 180);
  const azRad = obj.azimuth * (Math.PI / 180);

  const x = RADIUS * Math.cos(altRad) * Math.sin(azRad);
  const y = RADIUS * Math.sin(altRad);
  const z = -RADIUS * Math.cos(altRad) * Math.cos(azRad);

  const isConstellation = obj.category === 'Constellation';

  if (isConstellation) {
    return (
        <Billboard position={[x, y, z]}>
          <Text fontSize={1.5} color="white" anchorX="center" anchorY="middle" fillOpacity={0.4}>
            {obj.name}
          </Text>
        </Billboard>
    );
  }

  const isSun = obj.magnitude < -20;
  const isMoon = !isSun && obj.magnitude > -20 && obj.magnitude < -10;
  const isPlanet = !isSun && !isMoon && obj.category === 'Planet';

  let size: number;
  let color: string;
  let segments: number;

  if (isSun) {
    size = 2;
    color = '#ffea88';
    segments = 32;
  } else if (isMoon) {
    size = 1.2;
    color = '#cccccc';
    segments = 32;
  } else if (isPlanet) {
    size = 0.3;
    color = '#ffffff';
    segments = 16;
  } else {
    // Stars: exponential falloff so faint stars stay tiny and bright stars pop.
    size = Math.max(0.03, 0.15 * Math.pow(0.8, obj.magnitude));
    color = '#ffffff';
    segments = 6;
  }

  // Only major bodies get a text label — rendering Billboard/Text per star would tank FPS.
  const showLabel = isSun || isMoon || isPlanet;

  return (
      <>
        <mesh position={[x, y, z]} onClick={(e) => { e.stopPropagation(); onClick(); }}>
          <sphereGeometry args={[size, segments, segments]} />
          <meshBasicMaterial color={color} />
        </mesh>
        {showLabel && (
            <Billboard position={[x, y + size + 0.6, z]}>
              <Text fontSize={0.8} color="white" anchorX="center" anchorY="middle" fillOpacity={0.6}>
                {obj.name}
              </Text>
            </Billboard>
        )}
      </>
  );
};

const SkyCanvas = () => {
  const skyObjects = useAstroStore((state) => state.skyObjects);
  const selectObject = useAstroStore((state) => state.selectObject);

  return (
      <div className="absolute inset-0 w-full h-full">
        <Canvas camera={{ position: [0, 0, 0], fov: 75, near: 0.1, far: 5000 }}>
          <OrbitControls
              target={[0, 0, -0.01]}
              enableZoom={true}
              enablePan={false}
              minPolarAngle={0}
              maxPolarAngle={Math.PI / 2}
              makeDefault
          />

          <ambientLight intensity={0.3} />

          <SkyBackground />
          <Stars radius={SKY_RADIUS - 20} depth={50} count={3000} factor={4} saturation={0} fade speed={0.5} />
          <Ground />
          <CardinalPoints />

          {skyObjects?.map((obj) => (
              <CelestialObject key={obj.id} obj={obj} onClick={() => selectObject(obj)} />
          ))}
        </Canvas>
      </div>
  );
};

export default SkyCanvas;
