using System.Numerics;

namespace Engine.Meshing
{
    
    public class Chunk
    {
        private readonly int m_Size;
        private readonly bool[] voxels;
        private readonly Vector3 m_WorldPos;

        public int Size => m_Size;

        //chunk pos * chunk size;
        public Vector3 WorldPos => m_WorldPos;

        public Chunk(Vector3 worldPos, int size = 32)
        {
            m_Size = size;
            voxels = new bool[size * size * size];
            m_WorldPos = worldPos * m_Size;
        }

        public void GenerateChunkPerlin(int octaves, float persistance, float amplitude)
        {
            for(int x = 0; x < m_Size; x++)
            {
                for(int z = 0; z < m_Size; z++)
                {
                    double noise = Noise.Octaves(x + WorldPos.X, z + WorldPos.Z, octaves, persistance, amplitude);
                    double idk = m_Size * noise;
                    double height = (m_Size / 2) + idk;
                    for(int y = 0; y < m_Size; y++)
                    {
                        int index = x | (y << 5) | (z << 10);
                        voxels[index] = y < height;
                    }
                }
            }
        }

        public IMesh GenerateMesh()
        {
            List<int> data = new List<int>();
            List<uint> indices = new List<uint>();

            uint currentVertex = 0;
            for (int i = 0; i < voxels.Length; i++)
            {
                if (!voxels[i]) { continue; }

                int x = i & (m_Size - 1);
                int y = (i >> 5) & (m_Size - 1);
                int z = (i >> 10) & (m_Size - 1);

                //z- north ccw
                if (!GetVoxel(x, y, z - 1))
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
                if (!GetVoxel(x, y, z + 1))
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
                if (!GetVoxel(x - 1, y, z))
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
                if (!GetVoxel(x + 1, y, z))
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
                if (!GetVoxel(x, y - 1, z))
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
                if (!GetVoxel(x, y + 1, z))
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

        private bool GetVoxel(int x, int y, int z)
        {
            if (x < 0 || x > m_Size - 1 || y < 0 || y > m_Size - 1 || z < 0 || z > m_Size - 1)
            {
                return false;
            }

            int index = x | (y << 5) | (z << 10);
            return voxels[index];
        }
    }
}
