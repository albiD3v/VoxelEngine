using System.Numerics;

namespace Engine
{
    public interface ICamera
    {
        //apsect ration
        //fov

        Vector3 Position { get; set; }

        public Vector3 Direction
        {
            get
            {
                Vector3 direction = Vector3.Zero;
                direction.X = MathF.Cos(MathHelper.DegreesToRadians(this.Yaw)) * MathF.Cos(MathHelper.DegreesToRadians(this.Pitch));
                direction.Y = MathF.Sin(MathHelper.DegreesToRadians(this.Pitch));
                direction.Z = MathF.Sin(MathHelper.DegreesToRadians(this.Yaw)) * MathF.Cos(MathHelper.DegreesToRadians(this.Pitch));

                return Vector3.Normalize(direction);
            }
        }

        public Vector3 Up => Vector3.UnitY;

        float Yaw { get; set; }
        float Pitch { get; set; }

        Matrix4x4 ViewMatrix { get; }
        Matrix4x4 ProjectionMatrix { get; }
        Matrix4x4 ProjectionViewMatrix { get; }
    }
}
