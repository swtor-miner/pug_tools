﻿using System.Drawing;
using SlimDX;

namespace SlimDX_Framework.Camera
{
    public delegate float HeightFunc(float x, float y);
    public class FpsCamera : CameraBase
    {

        public override void LookAt(Vector3 pos, Vector3 target, Vector3 up)
        {
            Position = pos;
            Look = Vector3.Normalize(target - pos);
            Right = Vector3.Normalize(Vector3.Cross(up, Look));
            Up = Vector3.Cross(Look, Right);
        }

        public override void Strafe(float d)
        {
            Position += Right * d;
        }

        public override void Walk(float d)
        {
            Position += Look * d;
        }

        public override void Pitch(float angle)
        {
            var r = Matrix.RotationAxis(Right, angle);
            Up = Vector3.TransformNormal(Up, r);
            Look = Vector3.TransformNormal(Look, r);
        }

        public override void Yaw(float angle)
        {
            var r = Matrix.RotationY(angle);
            Right = Vector3.TransformNormal(Right, r);
            Up = Vector3.TransformNormal(Up, r);
            Look = Vector3.TransformNormal(Look, r);
        }
        public override void Zoom(float dr)
        {
            var newFov = MathF.Clamp(FovY + dr, 0.1f, MathF.PI / 2);
            SetLens(newFov, Aspect, NearZ, FarZ);
        }

        public override void UpdateViewMatrix()
        {
            var r = Right;
            _ = Up;
            var l = Look;
            var p = Position;

            l = Vector3.Normalize(l);
            Vector3 u = Vector3.Normalize(Vector3.Cross(l, r));

            r = Vector3.Cross(u, l);

            var x = -Vector3.Dot(p, r);
            var y = -Vector3.Dot(p, u);
            var z = -Vector3.Dot(p, l);

            Right = r;
            Up = u;
            Look = l;

            var v = new Matrix();
            v[0, 0] = Right.X;
            v[1, 0] = Right.Y;
            v[2, 0] = Right.Z;
            v[3, 0] = x;

            v[0, 1] = Up.X;
            v[1, 1] = Up.Y;
            v[2, 1] = Up.Z;
            v[3, 1] = y;

            v[0, 2] = Look.X;
            v[1, 2] = Look.Y;
            v[2, 2] = Look.Z;
            v[3, 2] = z;

            v[0, 3] = v[1, 3] = v[2, 3] = 0;
            v[3, 3] = 1;

            View = v;

            _frustum = Frustum.FromViewProj(ViewProj);
        }
    }
}
