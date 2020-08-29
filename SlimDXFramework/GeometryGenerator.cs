﻿using System.Collections.Generic;
using System.Runtime.InteropServices;
using SlimDX;

namespace SlimDX_Framework
{
    public class GeometryGenerator
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct Vertex
        {
            public Vector3 Position { get; set; }
            public Vector3 Normal { get; set; }
            public Vector3 TangentU { get; set; }
            public Vector2 TexC { get; set; }
            public Vertex(Vector3 pos, Vector3 norm, Vector3 tan, Vector2 uv) : this()
            {
                Position = pos;
                Normal = norm;
                TangentU = tan;
                TexC = uv;
            }

            public Vertex(float px, float py, float pz, float nx, float ny, float nz, float tx, float ty, float tz, float u, float v) :
                this(new Vector3(px, py, pz), new Vector3(nx, ny, nz), new Vector3(tx, ty, tz), new Vector2(u, v))
            {
            }
        }
        public class MeshData
        {
            public List<Vertex> Vertices = new List<Vertex>();
            public List<int> Indices = new List<int>();
        }

        public static MeshData CreateBox(float width, float height, float depth)
        {
            var ret = new MeshData();

            var w2 = 0.5f * width;
            var h2 = 0.5f * height;
            var d2 = 0.5f * depth;
            // front
            ret.Vertices.Add(new Vertex(-w2, -h2, -d2, 0, 0, -1, 1, 0, 0, 0, 1));
            ret.Vertices.Add(new Vertex(-w2, +h2, -d2, 0, 0, -1, 1, 0, 0, 0, 0));
            ret.Vertices.Add(new Vertex(+w2, +h2, -d2, 0, 0, -1, 1, 0, 0, 1, 0));
            ret.Vertices.Add(new Vertex(+w2, -h2, -d2, 0, 0, -1, 1, 0, 0, 1, 1));
            // back
            ret.Vertices.Add(new Vertex(-w2, -h2, +d2, 0, 0, 1, -1, 0, 0, 1, 1));
            ret.Vertices.Add(new Vertex(+w2, -h2, +d2, 0, 0, 1, -1, 0, 0, 0, 1));
            ret.Vertices.Add(new Vertex(+w2, +h2, +d2, 0, 0, 1, -1, 0, 0, 0, 0));
            ret.Vertices.Add(new Vertex(-w2, +h2, +d2, 0, 0, 1, -1, 0, 0, 1, 0));
            // top
            ret.Vertices.Add(new Vertex(-w2, +h2, -d2, 0, 1, 0, 1, 0, 0, 0, 1));
            ret.Vertices.Add(new Vertex(-w2, +h2, +d2, 0, 1, 0, 1, 0, 0, 0, 0));
            ret.Vertices.Add(new Vertex(+w2, +h2, +d2, 0, 1, 0, 1, 0, 0, 1, 0));
            ret.Vertices.Add(new Vertex(+w2, +h2, -d2, 0, 1, 0, 1, 0, 0, 1, 1));
            // bottom
            ret.Vertices.Add(new Vertex(-w2, -h2, -d2, 0, -1, 0, -1, 0, 0, 1, 1));
            ret.Vertices.Add(new Vertex(+w2, -h2, -d2, 0, -1, 0, -1, 0, 0, 0, 1));
            ret.Vertices.Add(new Vertex(+w2, -h2, +d2, 0, -1, 0, -1, 0, 0, 0, 0));
            ret.Vertices.Add(new Vertex(-w2, -h2, +d2, 0, -1, 0, -1, 0, 0, 1, 0));
            // left
            ret.Vertices.Add(new Vertex(-w2, -h2, +d2, -1, 0, 0, 0, 0, -1, 0, 1));
            ret.Vertices.Add(new Vertex(-w2, +h2, +d2, -1, 0, 0, 0, 0, -1, 0, 0));
            ret.Vertices.Add(new Vertex(-w2, +h2, -d2, -1, 0, 0, 0, 0, -1, 1, 0));
            ret.Vertices.Add(new Vertex(-w2, -h2, -d2, -1, 0, 0, 0, 0, -1, 1, 1));
            // right
            ret.Vertices.Add(new Vertex(+w2, -h2, -d2, 1, 0, 0, 0, 0, 1, 0, 1));
            ret.Vertices.Add(new Vertex(+w2, +h2, -d2, 1, 0, 0, 0, 0, 1, 0, 0));
            ret.Vertices.Add(new Vertex(+w2, +h2, +d2, 1, 0, 0, 0, 0, 1, 1, 0));
            ret.Vertices.Add(new Vertex(+w2, -h2, +d2, 1, 0, 0, 0, 0, 1, 1, 1));

            ret.Indices.AddRange(new[]{
                0,1,2,0,2,3,
                4,5,6,4,6,7,
                8,9,10,8,10,11,
                12,13,14,12,14,15,
                16,17,18,16,18,19,
                20,21,22,20,22,23
            });

            return ret;
        }

        public static MeshData CreateSphere(float radius, int sliceCount, int stackCount)
        {
            var ret = new MeshData();
            ret.Vertices.Add(new Vertex(0, radius, 0, 0, 1, 0, 1, 0, 0, 0, 0));
            var phiStep = MathF.PI / stackCount;
            var thetaStep = 2.0f * MathF.PI / sliceCount;

            for (int i = 1; i <= stackCount - 1; i++)
            {
                var phi = i * phiStep;
                for (int j = 0; j <= sliceCount; j++)
                {
                    var theta = j * thetaStep;
                    var p = new Vector3(
                        radius * MathF.Sin(phi) * MathF.Cos(theta),
                        radius * MathF.Cos(phi),
                        radius * MathF.Sin(phi) * MathF.Sin(theta)
                        );

                    var t = new Vector3(-radius * MathF.Sin(phi) * MathF.Sin(theta), 0, radius * MathF.Sin(phi) * MathF.Cos(theta));
                    t.Normalize();
                    var n = Vector3.Normalize(p);

                    var uv = new Vector2(theta / (MathF.PI * 2), phi / MathF.PI);
                    ret.Vertices.Add(new Vertex(p, n, t, uv));
                }
            }
            ret.Vertices.Add(new Vertex(0, -radius, 0, 0, -1, 0, 1, 0, 0, 0, 1));


            for (int i = 1; i <= sliceCount; i++)
            {
                ret.Indices.Add(0);
                ret.Indices.Add(i + 1);
                ret.Indices.Add(i);
            }
            var baseIndex = 1;
            var ringVertexCount = sliceCount + 1;
            for (int i = 0; i < stackCount - 2; i++)
            {
                for (int j = 0; j < sliceCount; j++)
                {
                    ret.Indices.Add(baseIndex + i * ringVertexCount + j);
                    ret.Indices.Add(baseIndex + i * ringVertexCount + j + 1);
                    ret.Indices.Add(baseIndex + (i + 1) * ringVertexCount + j);

                    ret.Indices.Add(baseIndex + (i + 1) * ringVertexCount + j);
                    ret.Indices.Add(baseIndex + i * ringVertexCount + j + 1);
                    ret.Indices.Add(baseIndex + (i + 1) * ringVertexCount + j + 1);
                }
            }
            var southPoleIndex = ret.Vertices.Count - 1;
            baseIndex = southPoleIndex - ringVertexCount;
            for (int i = 0; i < sliceCount; i++)
            {
                ret.Indices.Add(southPoleIndex);
                ret.Indices.Add(baseIndex + i);
                ret.Indices.Add(baseIndex + i + 1);
            }
            return ret;
        }
        public static MeshData CreateCylinder(float bottomRadius, float topRadius, float height, int sliceCount, int stackCount)
        {
            var ret = new MeshData();

            var stackHeight = height / stackCount;
            var radiusStep = (topRadius - bottomRadius) / stackCount;
            var ringCount = stackCount + 1;

            for (int i = 0; i < ringCount; i++)
            {
                var y = -0.5f * height + i * stackHeight;
                var r = bottomRadius + i * radiusStep;
                var dTheta = 2.0f * MathF.PI / sliceCount;
                for (int j = 0; j <= sliceCount; j++)
                {

                    var c = MathF.Cos(j * dTheta);
                    var s = MathF.Sin(j * dTheta);

                    var v = new Vector3(r * c, y, r * s);
                    var uv = new Vector2((float)j / sliceCount, 1.0f - (float)i / stackCount);
                    var t = new Vector3(-s, 0.0f, c);

                    var dr = bottomRadius - topRadius;
                    var bitangent = new Vector3(dr * c, -height, dr * s);

                    var n = Vector3.Normalize(Vector3.Cross(t, bitangent));

                    ret.Vertices.Add(new Vertex(v, n, t, uv));

                }
            }
            var ringVertexCount = sliceCount + 1;
            for (int i = 0; i < stackCount; i++)
            {
                for (int j = 0; j < sliceCount; j++)
                {
                    ret.Indices.Add(i * ringVertexCount + j);
                    ret.Indices.Add((i + 1) * ringVertexCount + j);
                    ret.Indices.Add((i + 1) * ringVertexCount + j + 1);

                    ret.Indices.Add(i * ringVertexCount + j);
                    ret.Indices.Add((i + 1) * ringVertexCount + j + 1);
                    ret.Indices.Add(i * ringVertexCount + j + 1);
                }
            }
            BuildCylinderTopCap(topRadius, height, sliceCount, ref ret);
            BuildCylinderBottomCap(bottomRadius, height, sliceCount, ref ret);
            return ret;
        }

        private static void BuildCylinderTopCap(float topRadius, float height, int sliceCount, ref MeshData ret)
        {
            var baseIndex = ret.Vertices.Count;

            var y = 0.5f * height;
            var dTheta = 2.0f * MathF.PI / sliceCount;

            for (int i = 0; i <= sliceCount; i++)
            {
                var x = topRadius * MathF.Cos(i * dTheta);
                var z = topRadius * MathF.Sin(i * dTheta);

                var u = x / height + 0.5f;
                var v = z / height + 0.5f;
                ret.Vertices.Add(new Vertex(x, y, z, 0, 1, 0, 1, 0, 0, u, v));
            }
            ret.Vertices.Add(new Vertex(0, y, 0, 0, 1, 0, 1, 0, 0, 0.5f, 0.5f));
            var centerIndex = ret.Vertices.Count - 1;
            for (int i = 0; i < sliceCount; i++)
            {
                ret.Indices.Add(centerIndex);
                ret.Indices.Add(baseIndex + i + 1);
                ret.Indices.Add(baseIndex + i);
            }
        }

        private static void BuildCylinderBottomCap(float bottomRadius, float height, int sliceCount, ref MeshData ret)
        {
            var baseIndex = ret.Vertices.Count;

            var y = -0.5f * height;
            var dTheta = 2.0f * MathF.PI / sliceCount;

            for (int i = 0; i <= sliceCount; i++)
            {
                var x = bottomRadius * MathF.Cos(i * dTheta);
                var z = bottomRadius * MathF.Sin(i * dTheta);

                var u = x / height + 0.5f;
                var v = z / height + 0.5f;
                ret.Vertices.Add(new Vertex(x, y, z, 0, -1, 0, 1, 0, 0, u, v));
            }
            ret.Vertices.Add(new Vertex(0, y, 0, 0, -1, 0, 1, 0, 0, 0.5f, 0.5f));
            var centerIndex = ret.Vertices.Count - 1;
            for (int i = 0; i < sliceCount; i++)
            {
                ret.Indices.Add(centerIndex);
                ret.Indices.Add(baseIndex + i);
                ret.Indices.Add(baseIndex + i + 1);
            }
        }

        public static MeshData CreateGrid(float width, float depth, int m, int n)
        {
            var ret = new MeshData();

            var halfWidth = width * 0.5f;
            var halfDepth = depth * 0.5f;

            var dx = width / (n - 1);
            var dz = depth / (m - 1);

            var du = 1.0f / (n - 1);
            var dv = 1.0f / (m - 1);

            for (var i = 0; i < m; i++)
            {
                var z = halfDepth - i * dz;
                for (var j = 0; j < n; j++)
                {
                    var x = -halfWidth + j * dx;
                    ret.Vertices.Add(new Vertex(new Vector3(x, 0, z), new Vector3(0, 1, 0), new Vector3(1, 0, 0), new Vector2(j * du, i * dv)));
                }
            }
            for (var i = 0; i < m - 1; i++)
            {
                for (var j = 0; j < n - 1; j++)
                {
                    ret.Indices.Add(i * n + j);
                    ret.Indices.Add(i * n + j + 1);
                    ret.Indices.Add((i + 1) * n + j);

                    ret.Indices.Add((i + 1) * n + j);
                    ret.Indices.Add(i * n + j + 1);
                    ret.Indices.Add((i + 1) * n + j + 1);
                }
            }
            return ret;
        }

        public static MeshData CreateFullScreenQuad()
        {
            var ret = new MeshData();

            ret.Vertices.Add(new Vertex(-1, -1, 0, 0, 0, -1, 1, 0, 0, 0, 1));
            ret.Vertices.Add(new Vertex(-1, 1, 0, 0, 0, -1, 1, 0, 0, 0, 0));
            ret.Vertices.Add(new Vertex(1, 1, 0, 0, 0, -1, 1, 0, 0, 1, 0));
            ret.Vertices.Add(new Vertex(1, -1, 0, 0, 0, -1, 1, 0, 0, 1, 1));

            ret.Indices.AddRange(new[] { 0, 1, 2, 0, 2, 3 });


            return ret;
        }
    }
}
