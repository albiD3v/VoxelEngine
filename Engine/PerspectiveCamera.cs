using System.Numerics;

namespace Engine
{
    public class PerspectiveCamera : ICamera
    {
        private Vector3 m_position;

        private float m_yaw;
        private float m_pitch;

        private float m_fieldOfView;
        private float m_aspectRatio;

        //frustum shit
        private float m_nearPlaneDistance;
        private float m_farPlaneDistance;

        //cached values
        private Matrix4x4 m_viewMatrix;
        private Matrix4x4 m_projectionMatrix;
        private Matrix4x4 m_ProjectionViewMatrix;

        public Vector3 Up { get; private set; }
        public Vector3 Position
        {
            get => m_position;
            set
            {
                m_position = value;
                ComputeViewMatrix();
                ComputeViewProjectionMatrix();
            }
        }
        public Vector3 Direction
        {
            get
            {
                Vector3 direction = Vector3.Zero;
                direction.X = MathF.Cos(MathHelper.DegreesToRadians(m_yaw)) * MathF.Cos(MathHelper.DegreesToRadians(m_pitch));
                direction.Y = MathF.Sin(MathHelper.DegreesToRadians(m_pitch));
                direction.Z = MathF.Sin(MathHelper.DegreesToRadians(m_yaw)) * MathF.Cos(MathHelper.DegreesToRadians(m_pitch));

                return Vector3.Normalize(direction);
            }
        }

        public float Yaw
        {
            get => m_yaw;
            set
            {
                m_yaw = value;
                ComputeViewMatrix();
                ComputeViewProjectionMatrix();
            }
        }
        public float Pitch
        {
            get => m_pitch;
            set
            {
                m_pitch = value;
                m_pitch = Math.Clamp(m_pitch, -89.0f, 89.0f);
                ComputeViewMatrix();
                ComputeViewProjectionMatrix();
            }
        }

        public float FieldOfView
        {
            get => m_fieldOfView;
            set
            {
                m_fieldOfView = value;
                m_fieldOfView = Math.Clamp(m_fieldOfView, 30.0f, 120.0f);
                ComputeProjectionMatrix();
                ComputeViewProjectionMatrix();
            }
        }
        public float AspectRatio
        {
            get => m_aspectRatio;
            set
            {
                m_aspectRatio = value;
                ComputeProjectionMatrix();
                ComputeViewProjectionMatrix();
            }
        }

        public float NearPlaneDistance
        {
            get => m_nearPlaneDistance;
            set
            {
                m_nearPlaneDistance = value;
                ComputeProjectionMatrix();
                ComputeViewProjectionMatrix();
            }
        }
        public float FarPlaneDistance
        {
            get => m_farPlaneDistance;
            set
            {
                m_farPlaneDistance = value;
                ComputeProjectionMatrix();
                ComputeViewProjectionMatrix();
            }
        }

        public Matrix4x4 ViewMatrix => m_viewMatrix;
        public Matrix4x4 ProjectionMatrix => m_projectionMatrix;

        public Matrix4x4 ProjectionViewMatrix => m_ProjectionViewMatrix;

        public PerspectiveCamera(Vector3 position, float aspectRatio)
        {
            Position = position;
            Up = Vector3.UnitY;

            m_yaw = 45.0f;
            m_pitch = 0.0f;

            m_aspectRatio = aspectRatio;
            m_fieldOfView = 45.0f;

            m_nearPlaneDistance = 0.1f;
            m_farPlaneDistance = 1000.0f;

            ComputeViewMatrix();
            ComputeProjectionMatrix();
            ComputeViewProjectionMatrix();
        }

        public PerspectiveCamera(float aspectRatio) : this(Vector3.Zero, aspectRatio) { }
        public PerspectiveCamera(int width, int height) : this(Vector3.Zero, (float)width / height) { }

        private void ComputeViewMatrix()
        {
            m_viewMatrix = Matrix4x4.CreateLookAt(m_position, m_position + Direction, Up);
            ComputeViewProjectionMatrix();
        }

        private void ComputeProjectionMatrix()
        {
            m_projectionMatrix = Matrix4x4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(m_fieldOfView), m_aspectRatio, m_nearPlaneDistance, m_farPlaneDistance);
            ComputeViewProjectionMatrix();
        }

        private void ComputeViewProjectionMatrix()
        {
            m_ProjectionViewMatrix = m_viewMatrix * m_projectionMatrix;
        }
    }
}
