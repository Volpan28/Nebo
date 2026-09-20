import { Canvas } from '@react-three/fiber';
import { Billboard, OrbitControls, Stars, Text } from '@react-three/drei';
import { useAstroStore } from '../store/useAstroStore';

const RADIUS = 100;

const CelestialObject = ({ obj, onClick }: { obj: any, onClick: () => void }) => {
  const altRad = obj.altitude * (Math.PI / 180);
  const azRad = obj.azimuth * (Math.PI / 180);

  const x = RADIUS * Math.cos(altRad) * Math.sin(azRad);
  const y = RADIUS * Math.sin(altRad);
  const z = -RADIUS * Math.cos(altRad) * Math.cos(azRad);

  const isPlanetOrMoon = obj.category === 'Planet' || obj.category === 'Moon';
  const isConstellation = obj.category === 'Constellation';

  let size = 0.1;
  if (obj.magnitude < -20) size = 4; // Sun
  else if (obj.magnitude < -10) size = 1.5; // Moon
  else if (obj.category === 'Planet') size = 0.5; // Planets
  else size = Math.max(0.05, 0.3 - obj.magnitude * 0.05); // Stars

  let color = "#ffffff";
  if (obj.magnitude < -20) color = "#ffea88"; // Sun
  else if (obj.magnitude < -10) color = "#cccccc"; // Moon

  if (isConstellation) {
    return (
        <Billboard position={[x, y, z]}>
          <Text fontSize={2} color="rgba(255,255,255,0.4)" anchorX="center" anchorY="middle">
            {obj.name}
          </Text>
        </Billboard>
    );
  }

  return (
      <mesh position={[x, y, z]} onClick={(e) => { e.stopPropagation(); onClick(); }}>
        <sphereGeometry args={[size, 16, 16]} />
        <meshBasicMaterial color={color} />
      </mesh>
  );
};

const SkyCanvas = () => {
  const skyObjects = useAstroStore((state) => state.skyObjects);
  const selectObject = useAstroStore((state) => state.selectObject);

  console.log("Об'єктів у небі:", skyObjects?.length, skyObjects);

  return (
      <div className="absolute inset-0 w-full h-full">

        <Canvas camera={{ position: [0, 0.1, 0.1], fov: 75 }}>
          <OrbitControls target={[0, 0, 0]} enableZoom={false} enablePan={false} maxPolarAngle={Math.PI / 2} />

          <ambientLight intensity={0.5} />

          <Stars radius={100} depth={50} count={3000} factor={4} saturation={0} fade speed={1} />

          <mesh rotation={[-Math.PI / 2, 0, 0]} position={[0, 0, 0]}>
            <planeGeometry args={[500, 500]} />
            <meshBasicMaterial color="#0a120a" />
          </mesh>

          {skyObjects?.map((obj) => (
              <CelestialObject key={obj.id} obj={obj} onClick={() => selectObject(obj)} />
          ))}
        </Canvas>
      </div>
  );
};

export default SkyCanvas;
