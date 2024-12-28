using System.Numerics;

namespace Engine.Meshing
{
    public class Chunk
    {
        private readonly int m_Size;
        private readonly ushort[] m_Voxels;
        private readonly Vector3 m_WorldPos;

        public int Size => m_Size;

        public Vector3 WorldPos => m_WorldPos;

        public Chunk(Vector3 worldPos, int size = 32)
        {
            m_Size = size;
            m_Voxels = new ushort[size * size * size];
            m_WorldPos = worldPos * m_Size;
        }

        public void GenerateChunkPerlin(int octaves, float persistance, float amplitude)
        {
            OpenSimplexNoise noise = new OpenSimplexNoise(3L);

            for (int x = 0; x < m_Size; x++)
            {
                for(int z = 0; z < m_Size; z++)
                {
                    double maxHeight = noise.Octaves((x + m_WorldPos.X) / m_Size, (z + m_WorldPos.Z) / m_Size, octaves, amplitude, persistance, 43.0d, 1.0d);
                    for (int y = 0; y + m_WorldPos.Y < maxHeight; y++)
                    {
                        int index = x | (y << 5) | (z << 10);
                        m_Voxels[index] = 1;
                    }
                }
            }
        }

        public IMesh GenerateMesh()
        {
            List<int> data = new List<int>();
            List<uint> indices = new List<uint>();

            uint currentVertex = 0;
            for (int i = 0; i < m_Voxels.Length; i++)
            {
                Voxel? v = GetVoxel(i);
                if(v == null || !v.IsSolid) { continue; }

                int x = i & (m_Size - 1);
                int y = (i >> 5) & (m_Size - 1);
                int z = (i >> 10) & (m_Size - 1);

                Voxel? vNorth = GetVoxel(x, y, z - 1);
                Voxel? vSouth = GetVoxel(x, y, z + 1);
                Voxel? vWest  = GetVoxel(x - 1, y, z);
                Voxel? vEast  = GetVoxel(x + 1, y, z);
                Voxel? vDown  = GetVoxel(x, y - 1, z);
                Voxel? vUp    = GetVoxel(x, y + 1, z);

                int opaqueBitMask = 0b000000;
                opaqueBitMask |= vNorth != null && vNorth.IsSolid ? 0b100000 : 0b000000;
                opaqueBitMask |= vSouth != null && vSouth.IsSolid ? 0b010000 : 0b000000;
                opaqueBitMask |= vWest  != null && vWest.IsSolid ? 0b001000 : 0b000000;
                opaqueBitMask |= vEast  != null && vEast.IsSolid ? 0b000100 : 0b000000;
                opaqueBitMask |= vDown  != null && vDown.IsSolid ? 0b000010 : 0b000000;
                opaqueBitMask |= vUp    != null && vUp.IsSolid ? 0b000001 : 0b000000;

                if(opaqueBitMask == 0b111111)
                {
                    continue;
                }

                //z- north ccw
                if ((opaqueBitMask & 0b100000) == 0)
                {
                    data.Add((x + 1) | ((y + 0) << 6) | ((z + 0) << 12) | (0 << 18));
                    data.Add((x + 0) | ((y + 0) << 6) | ((z + 0) << 12) | (0 << 18));
                    data.Add((x + 1) | ((y + 1) << 6) | ((z + 0) << 12) | (0 << 18));
                    data.Add((x + 0) | ((y + 1) << 6) | ((z + 0) << 12) | (0 << 18));

                    indices.Add(currentVertex + 0);
                    indices.Add(currentVertex + 3);
                    indices.Add(currentVertex + 1);
                    indices.Add(currentVertex + 0);
                    indices.Add(currentVertex + 2);
                    indices.Add(currentVertex + 3);

                    currentVertex += 4;
                }

                //z+ south cw
                if ((opaqueBitMask & 0b010000) == 0)
                {
                    data.Add((x + 0) | ((y + 0) << 6) | ((z + 1) << 12) | (1 << 18));
                    data.Add((x + 1) | ((y + 0) << 6) | ((z + 1) << 12) | (1 << 18));
                    data.Add((x + 0) | ((y + 1) << 6) | ((z + 1) << 12) | (1 << 18));
                    data.Add((x + 1) | ((y + 1) << 6) | ((z + 1) << 12) | (1 << 18));

                    indices.Add(currentVertex + 0);
                    indices.Add(currentVertex + 3);
                    indices.Add(currentVertex + 1);
                    indices.Add(currentVertex + 0);
                    indices.Add(currentVertex + 2);
                    indices.Add(currentVertex + 3);
                    
                    currentVertex += 4;
                }

                //x- west cw
                if ((opaqueBitMask & 0b001000) == 0)
                {
                    data.Add((x + 0) | ((y + 0) << 6) | ((z + 0) << 12) | (2 << 18));
                    data.Add((x + 0) | ((y + 0) << 6) | ((z + 1) << 12) | (2 << 18));
                    data.Add((x + 0) | ((y + 1) << 6) | ((z + 0) << 12) | (2 << 18));
                    data.Add((x + 0) | ((y + 1) << 6) | ((z + 1) << 12) | (2 << 18));

                    indices.Add(currentVertex + 0);
                    indices.Add(currentVertex + 3);
                    indices.Add(currentVertex + 1);
                    indices.Add(currentVertex + 0);
                    indices.Add(currentVertex + 2);
                    indices.Add(currentVertex + 3);

                    currentVertex += 4;
                }

                //x+ east ccw
                if ((opaqueBitMask & 0b000100) == 0)
                {
                    data.Add((x + 1) | ((y + 0) << 6) | ((z + 1) << 12) | (3 << 18));
                    data.Add((x + 1) | ((y + 0) << 6) | ((z + 0) << 12) | (3 << 18));
                    data.Add((x + 1) | ((y + 1) << 6) | ((z + 1) << 12) | (3 << 18));
                    data.Add((x + 1) | ((y + 1) << 6) | ((z + 0) << 12) | (3 << 18));

                    indices.Add(currentVertex + 0);
                    indices.Add(currentVertex + 3);
                    indices.Add(currentVertex + 1);
                    indices.Add(currentVertex + 0);
                    indices.Add(currentVertex + 2);
                    indices.Add(currentVertex + 3);

                    currentVertex += 4;
                }

                //y- down cw
                if ((opaqueBitMask & 0b000010) == 0)
                {
                    data.Add((x + 0) | ((y + 0) << 6) | ((z + 0) << 12) | (4 << 18));
                    data.Add((x + 1) | ((y + 0) << 6) | ((z + 0) << 12) | (4 << 18));
                    data.Add((x + 0) | ((y + 0) << 6) | ((z + 1) << 12) | (4 << 18));
                    data.Add((x + 1) | ((y + 0) << 6) | ((z + 1) << 12) | (4 << 18));

                    indices.Add(currentVertex + 0);
                    indices.Add(currentVertex + 3);
                    indices.Add(currentVertex + 1);
                    indices.Add(currentVertex + 0);
                    indices.Add(currentVertex + 2);
                    indices.Add(currentVertex + 3);

                    currentVertex += 4;
                }

                //y+ up ccw
                if ((opaqueBitMask & 0b000001) == 0)
                {
                    data.Add((x + 1) | ((y + 1) << 6) | ((z + 0) << 12) | (5 << 18));
                    data.Add((x + 0) | ((y + 1) << 6) | ((z + 0) << 12) | (5 << 18));
                    data.Add((x + 1) | ((y + 1) << 6) | ((z + 1) << 12) | (5 << 18));
                    data.Add((x + 0) | ((y + 1) << 6) | ((z + 1) << 12) | (5 << 18));

                    indices.Add(currentVertex + 0);
                    indices.Add(currentVertex + 3);
                    indices.Add(currentVertex + 1);
                    indices.Add(currentVertex + 0);
                    indices.Add(currentVertex + 2);
                    indices.Add(currentVertex + 3);

                    currentVertex += 4;
                }
            }
            return new Mesh(data.ToArray(), indices.ToArray());
        }

        public Voxel? GetVoxel(int x, int y, int z)
        {
            if (x < 0 || x > m_Size - 1 || y < 0 || y > m_Size - 1 || z < 0 || z > m_Size - 1)
            {
                return null;
            }

            int index = x | (y << 5) | (z << 10);
            return new Voxel(x, y, z, m_Voxels[index]);
        }

        public Voxel? GetVoxel(int index)
        {
            if (index >= m_Voxels.Length)
            {
                return null;
            }

            int x = index & (m_Size - 1);
            int y = (index >> 5) & (m_Size - 1);
            int z = (index >> 10) & (m_Size - 1);

            return new Voxel(x, y, z, m_Voxels[index]);
        }
    }
}
