using static System.Math;

namespace TinyRenderer
{
    public static class Draw
    {
        public static void Line(int x0, int y0, int x1, int y1, TGAImage image, TGAColor color)
        {
            var steep = false;
            if (Abs(x0 - x1) < Abs(y0 - y1))
            {
                CPP.Swap(ref x0, ref y0);
                CPP.Swap(ref x1, ref y1);
                steep = true;
            }
            if (x0 > x1)
            {
                CPP.Swap(ref x0, ref x1);
                CPP.Swap(ref y0, ref y1);
            }
            int dx = x1 - x0;
            int dy = y1 - y0;
            int derror2 = Abs(dy) * 2;
            int error2 = 0;
            int y = y0;
            for (int x = x0; x <= x1; x++)
            {
                if (steep)
                {
                    image.set(y, x, color);
                }
                else
                {
                    image.set(x, y, color);
                }
                error2 += derror2;
                if (error2 > dx)
                {
                    y += (y1 > y0 ? 1 : -1);
                    error2 -= dx * 2;
                }
            }
        }

        public static void Line(Vec2i v0, Vec2i v1, TGAImage image, TGAColor color)
            => Line(v0.X, v0.Y, v1.X, v1.Y, image, color);

        public static void TriangleWire(Vec2i t0, Vec2i t1, Vec2i t2, TGAImage image, TGAColor color)
        {
            Line(t0, t1, image, color);
            Line(t1, t2, image, color);
            Line(t2, t0, image, color);
        }

        public static void Triangle(Vec2i t0, Vec2i t1, Vec2i t2, TGAImage image, TGAColor color)
        {
            if (t0.Y > t1.Y) CPP.Swap(ref t0, ref t1);
            if (t1.Y > t2.Y) CPP.Swap(ref t1, ref t2);
            if (t0.Y > t1.Y) CPP.Swap(ref t0, ref t1);

            Line(t0, t1, image, TGAColor.Green);
            Line(t1, t2, image, TGAColor.Blue);
            Line(t2, t0, image, TGAColor.Red);
        }

        public static void Triangle(Span<Vec2i> points, TGAImage image, TGAColor color)
        {
            if (points[0].Y > points[1].Y) CPP.Swap(ref points[0], ref points[1]);
            if (points[1].Y > points[2].Y) CPP.Swap(ref points[1], ref points[2]);
            if (points[0].Y > points[1].Y) CPP.Swap(ref points[0], ref points[1]);

            var bboxmin = new Vec2i(image.Width - 1, image.Height - 1);
            var bboxmax = new Vec2i(0, 0);
            var clamp = new Vec2i(image.Width - 1, image.Height - 1);
            for (int i = 0; i < 3; i++)
            {
                bboxmin.X = Max(0, Min(bboxmin.X, points[i].X));
                bboxmin.Y = Max(0, Min(bboxmin.Y, points[i].Y));

                bboxmax.X = Min(clamp.X, Max(bboxmax.X, points[i].X));
                bboxmax.Y = Min(clamp.Y, Max(bboxmax.Y, points[i].Y));
            }

            Vec2i P = new();
            for (P.X = bboxmin.X; P.X < bboxmax.X; P.X++)
            {
                for (P.Y = bboxmin.Y; P.Y < bboxmax.Y; P.Y++)
                {
                    var bc_screen = GeometryHelper.Barycentric(points, P);
                    if (bc_screen.X < 0 || bc_screen.Y < 0 || bc_screen.Z < 0) continue;
                    image.set(P.X, P.Y, color);
                }
            }
        }
    }
}
