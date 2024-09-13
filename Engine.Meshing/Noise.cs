using System.Numerics;

namespace Engine.Meshing
{
    public class Noise
    {
        public static float Octaves(float x, float y, int octaves, float persistance = 1.0f, float amplitude = 1.0f, int gridSize = 32)
        {
            x += 0.1f;
            y += 0.1f;

            float total = 0.0f;
            float max_value = 0.0f;
            float frequency = 1.0f;
            for (int i = 0; i < octaves; i++)
            {
                total += Perlin(x * frequency / gridSize, y * frequency / gridSize) * amplitude;
                max_value += amplitude;

                amplitude *= persistance;
                frequency *= 2;
            }

            // Dividing by the max amplitude sum brings it into [-1, 1] range
            float value = total / max_value;
            float val = Math.Clamp(total, -1.0f, 1.0f);
            return val;
        }

        public static float Perlin(float x, float y)
        {
            //determine grid cell corner coordinates
            int x0 = (int)x;
            int y0 = (int)y;
            int x1 = x0 + 1;
            int y1 = y0 + 1;

            //compute iterpolation weights
            float sx = x - (float)x0;
            float sy = y - (float)y0;

            //compute and interpolate top two corner
            float n0 = DotGridGradient(x0, y0, x, y);
            float n1 = DotGridGradient(x1, y0, x, y);
            float ix0 = Interpolate(n0, n1, sx);

            //compute and interpolate bottom two corner
            float n2 = DotGridGradient(x0, y1, x, y);
            float n3 = DotGridGradient(x1, y1, x, y);
            float ix1 = Interpolate(n2, n3, sx);

            //interpolate between the two previously interpolated values, now in y
            float value = Interpolate(ix0, ix1, sy);
            return Math.Clamp(value, -1.0f, 1.0f);
        }

        //computes the dot product of the distance and gradient vectors
        private static float DotGridGradient(int ix, int iy, float x, float y)
        {
            //get gradient from integer coordinates
            Vector2 gradient = RandomGradient(ix, iy);

            //compute the distance vector
            float dx = x - (float)ix;
            float dy = y - (float)iy;

            //compute dot product
            return(dx * gradient.X + dy * gradient.Y);
        }

        private static float Interpolate(float a0, float a1, float w)
        {
            //quadratic interpolation
            return (a1- a0) * (3.0f - w * 2.0f) * w * w + a0;
        }

        //TO DO:: seed based 
        private static Vector2 RandomGradient(int ix, int iy)
        {
            int w = 8 * sizeof(uint);
            int s = w / 2;

            uint a = (uint)ix;
            uint b = (uint)iy;

            a *= 3284157443;

            b ^= a << s | a >> w - s;
            b *= 1911520717;

            a ^= b << s | b >> w - s;
            a *= 2048419325;
            float random = a * (3.14159265f / ~(~0u >> 1));

            return new Vector2(MathF.Sin(random), MathF.Cos(random));
        }
    }

}
