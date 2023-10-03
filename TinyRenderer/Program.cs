using TinyRenderer;
using static System.Math;

var white = new TGAColor(255, 255, 255, 255);
var red = new TGAColor(255, 0, 0, 255);

var image = new TGAImage(100, 100, TGAImage.Format.RGB);

// lesson0();
lesson1();
image.flip_vertically(); // i want to have the origin at the left bottom corner of the image
image.write_tga_file("../../../../output.tga");


void lesson0()
{
    image.set(52, 41, red);
}

void lesson1()
{
    line(13, 20, 80, 40, image, white);
    line(20, 13, 40, 80, image, red);
    line(80, 40, 13, 20, image, red);

    void line(int x0, int y0, int x1, int y1, TGAImage image, TGAColor color)
    {
        var steep = false;
        if (Abs(x0-x1) < Abs(y0-y1))
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