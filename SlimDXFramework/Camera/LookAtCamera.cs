using SlimDX;

namespace SlimDX_Framework.Camera
{
    public class LookAtCamera : CameraBase
    {
        private float _alpha;
        private float _beta;

        public float Radius { get; private set; }

        public LookAtCamera() : base()
        {
            _alpha = _beta = 0.5f;
            Radius = 10.0f;
            Target = new Vector3();

        }

        public new void Reset()
        {
            _alpha = _beta = 0.5f;
            Radius = 10.0f;
            Target = new Vector3();
        }

        public Vector3 Target { get; set; }

        public override void LookAt(Vector3 pos, Vector3 target, Vector3 up)
        {
            Target = target;
            Position = pos;
            Position = pos;
            Look = Vector3.Normalize(target - pos);
            Right = Vector3.Normalize(Vector3.Cross(up, Look));
            Up = Vector3.Cross(Look, Right);
            Radius = (target - pos).Length();
        }

        public override void Strafe(float d)
        {
            var dt = Vector3.Normalize(new Vector3(Right.X, 0, Right.Z)) * d;
            Target += dt;
        }

        public override void Walk(float d)
        {
            Target += Vector3.Normalize(new Vector3(Look.X, 0, Look.Z)) * d;
        }

        public void Fly(float d)
        {
            Target += Vector3.Normalize(new Vector3(0, Up.Y, 0)) * d;
        }

        public override void Pitch(float angle)
        {
            _beta += angle;
            _beta = MathF.Clamp(_beta, -(MathF.PI / 2.0f - 0.01f), MathF.PI / 2.0f - 0.01f);
        }

        public override void Yaw(float angle)
        {
            _alpha = (_alpha + angle) % (MathF.PI * 2.0f);
        }
        public override void Zoom(float dr)
        {
            Radius += dr;
            Radius = MathF.Clamp(Radius, 0.001f, 500.0f);
        }

        public override void UpdateViewMatrix()
        {
            var sideRadius = Radius * MathF.Cos(_beta);
            var height = Radius * MathF.Sin(_beta);

            Position = new Vector3(
                Target.X + sideRadius * MathF.Cos(_alpha),
                Target.Y + height,
                Target.Z + sideRadius * MathF.Sin(_alpha)
            );
            if (Height != null && Position.Y <= Height(Position.X, Position.Z) + 2.0f)
            {
                Position = new Vector3(Position.X, Height(Position.X, Position.Z) + 2.0f, Position.Z);
            }

            View = Matrix.LookAtRH(Position, Target, Vector3.UnitY);

            Right = new Vector3(View.M11, View.M21, View.M31);
            Right.Normalize();

            Look = new Vector3(View.M13, View.M23, View.M33);
            Look.Normalize();
            _frustum = Frustum.FromViewProj(ViewProj);
        }
    }
}