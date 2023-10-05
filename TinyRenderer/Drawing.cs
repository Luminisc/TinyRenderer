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
    }
}
