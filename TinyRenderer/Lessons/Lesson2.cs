namespace TinyRenderer.Lessons
{
    internal class Lesson2
    {
        public static void Run(TGAImage image)
        {
            var t0 = new Vec2i[3] { new(10, 70), new(50, 160), new(70, 80) };
            var t1 = new Vec2i[3] { new(180, 50), new(150, 1), new(70, 180) };
            var t2 = new Vec2i[3] { new(180, 150), new(120, 160), new(130, 180) };

            //Draw.Triangle(t0[0], t0[1], t0[2], image, TGAColor.Red);
            //Draw.Triangle(t1[0], t1[1], t1[2], image, TGAColor.White);
            //Draw.Triangle(t2[0], t2[1], t2[2], image, TGAColor.Green);

            Draw.Triangle(t0.AsSpan(), image, TGAColor.Red);
            Draw.Triangle(t1.AsSpan(), image, TGAColor.White);
            Draw.Triangle(t2.AsSpan(), image, TGAColor.Green);
        }

        public static unsafe void RunWithModel(TGAImage image, string filepath)
        {
            var rnd = new Random();
            var model = new Model(filepath);
            Span<Vec2i> screen_coords = stackalloc Vec2i[3];
            for (var i = 0; i < model.nfaces(); i++)
            {
                var face = model.face(i);
                for (var j = 0; j < 3; j++)
                {
                    Vec3f world_coords = model.vert(face[j]);
                    screen_coords[j] = new Vec2i((world_coords.X + 1f) * image.Width / 2f, (world_coords.Y + 1f) * image.Height / 2f);                    
                }
                var color = (byte)(rnd.Next() % 255);
                Draw.Triangle(screen_coords, image, new TGAColor(color, color, color, 255));
            }
        }

        public static unsafe void RunWithLighting(TGAImage image, string filepath)
        {
            var light_dir = new Vec3f(0, 0, -1);
            var rnd = new Random();
            var model = new Model(filepath);
            Span<Vec2i> screen_coords = stackalloc Vec2i[3];
            Span<Vec3f> wc = stackalloc Vec3f[3]; //world_coords
            for (var i = 0; i < model.nfaces(); i++)
            {
                var face = model.face(i);
                for (var j = 0; j < 3; j++)
                {
                    Vec3f v = model.vert(face[j]);
                    screen_coords[j] = new Vec2i((v.X + 1f) * image.Width / 2f, (v.Y + 1f) * image.Height / 2f);
                    wc[j] = v;
                }
                Vec3f n = (wc[2] - wc[0]) ^ (wc[1] - wc[0]);
                n.Normalize();
                var intensity = n * light_dir;
                if (intensity > 0)
                {
                    intensity *= 255;
                    Draw.Triangle(screen_coords, image, new TGAColor((byte)intensity, (byte)intensity, (byte)intensity, 255));
                }
            }
        }
    }
}
