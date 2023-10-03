using System.Runtime.InteropServices;
using static System.MathF;

namespace TinyRenderer
{
    // Can't use fixed size buffers with generics, so only way to have Vec2F and Vec2I is make them explicitly defined
    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct Vec2f
    {
        [FieldOffset(0)] public float X;
        [FieldOffset(sizeof(float))] public float Y;

        [FieldOffset(0)] public float U;
        [FieldOffset(sizeof(float))] public float V;

        [FieldOffset(0)] public fixed float Raw[2];

        public Vec2f() { X = 0; Y = 0; }
        public Vec2f(float _u, float _v) { U = _u; V = _v; }

        public static Vec2f operator +(in Vec2f v1, in Vec2f v2) { return new Vec2f(v1.U + v2.U, v1.V + v2.V); }
        public static Vec2f operator -(in Vec2f v1, in Vec2f v2) { return new Vec2f(v1.U - v2.U, v1.V - v2.V); }
        public static Vec2f operator *(in Vec2f v1, float f) { return new Vec2f(v1.U * f, v1.V * f); }
        public override string ToString() => $"({X}, {Y})";
    }

    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct Vec2i
    {
        [FieldOffset(0)] public int X;
        [FieldOffset(sizeof(int))] public int Y;

        [FieldOffset(0)] public int U;
        [FieldOffset(sizeof(int))] public int V;

        [FieldOffset(0)] public fixed int Raw[2];

        public Vec2i() { X = 0; Y = 0; }
        public Vec2i(int _u, int _v) { U = _u; V = _v; }

        public static Vec2i operator +(in Vec2i v1, in Vec2i v2) { return new Vec2i(v1.U + v2.U, v1.V + v2.V); }
        public static Vec2i operator -(in Vec2i v1, in Vec2i v2) { return new Vec2i(v1.U - v2.U, v1.V - v2.V); }
        public static Vec2i operator *(in Vec2i v1, float f) { return new Vec2i((int)(v1.U * f), (int)(v1.V * f)); }
        public override string ToString() => $"({X}, {Y})";
    }

    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct Vec3f
    {
        [FieldOffset(0)] public float X;
        [FieldOffset(sizeof(float))] public float Y;
        [FieldOffset(sizeof(float) * 2)] public float Z;

        [FieldOffset(0)] public float Ivert;
        [FieldOffset(sizeof(float))] public float Iuv;
        [FieldOffset(sizeof(float) * 2)] public float Inorm;

        [FieldOffset(0)] public fixed float Raw[3];

        public Vec3f() { X = 0; Y = 0; Z = 0; }
        public Vec3f(float x, float y, float z) { X = x; Y = y; Z = z; }

        public static Vec3f operator ^(in Vec3f v1, in Vec3f v2) { return new Vec3f(v1.Y * v2.Z - v1.Z * v2.Y, v1.Z * v2.X - v1.X * v2.Z, v1.X * v2.Y - v1.Y * v2.X); }
        public static Vec3f operator +(in Vec3f v1, in Vec3f v2) { return new Vec3f(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z); }
        public static Vec3f operator -(in Vec3f v1, in Vec3f v2) { return new Vec3f(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z); }
        public static Vec3f operator *(in Vec3f v1, float f) { return new Vec3f(v1.X * f, v1.Y * f, v1.Z * f); }
        public static float operator *(in Vec3f v1, in Vec3f v2) { return v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z; }
        public float Norm() => Sqrt(X * X + Y * Y + Z * Z);
        public Vec3f Normalize(float l = 1)
        {
            var val = l / Norm();
            X *= val;
            Y *= val;
            Z *= val;
            return this;
        }
        public override string ToString() => $"({X}, {Y}, {Z})";
    }
}
