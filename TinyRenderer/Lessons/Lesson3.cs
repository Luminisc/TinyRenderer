namespace TinyRenderer.Lessons
{
    internal class Lesson3
    {
        public static void Run(TGAImage image, string filepath)
        {
            var zbuffer = new float[image.Width * image.Height];
            var light_dir = new Vec3f(0, 0, -1);
            var rnd = new Random();
            var model = new Model(filepath);
            Span<Vec3f> screen_coords = stackalloc Vec3f[3];
            Span<Vec3f> world_coords = stackalloc Vec3f[3]; //world_coords
            for (var i = 0; i < model.nfaces(); i++)
            {
                var face = model.face(i);
                for (var j = 0; j < 3; j++)
                {
                    Vec3f v = model.vert(face[j]);
                    screen_coords[j] = new Vec3f((v.X + 1f) * image.Width / 2f, (v.Y + 1f) * image.Height / 2f, v.Z);
                    world_coords[j] = v;
                }
                Vec3f n = (world_coords[2] - world_coords[0]) ^ (world_coords[1] - world_coords[0]);
                n.Normalize();
                var intensity = n * light_dir;
                if (intensity > 0)
                {
                    intensity *= 255;
                    Draw.Triangle(world_coords, screen_coords, image, new TGAColor((byte)intensity, (byte)intensity, (byte)intensity, 255), zbuffer);
                }
            }
        }
    }
}
