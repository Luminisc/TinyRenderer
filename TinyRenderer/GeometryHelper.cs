using static System.Math;

namespace TinyRenderer
{
    internal static class GeometryHelper
    {
        public static Vec3f Barycentric(Span<Vec2i> pts, Vec2i P)
        {
            Vec3f u = new Vec3f(pts[2].X - pts[0].X, pts[1].X - pts[0].X, pts[0].X - P.X) ^ new Vec3f(pts[2].Y - pts[0].Y, pts[1].Y - pts[0].Y, pts[0].Y - P.Y);
            if (Abs(u.Z) < 1)
                return new Vec3f(-1, 1, 1);
            return new Vec3f(1f - (u.X + u.Y) / u.Z, u.Y / u.Z, u.X / u.Z);
        }
    }
}
