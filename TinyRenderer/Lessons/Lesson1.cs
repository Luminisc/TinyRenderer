namespace TinyRenderer.Lessons
{
    /// <summary>Draw lines and draw wireframe of model</summary>
    internal class Lesson1
    {
        public static void Run(string filepath, TGAImage image)
        {
            var white = new TGAColor(255, 255, 255, 255);
            var model = new Model("../../../../african_head.obj");

            for (var i = 0; i < model.nfaces(); i++)
            {
                var face = model.face(i);
                for (var j = 0; j < 3; j++)
                {
                    var v0 = model.vert(face[j]);
                    var v1 = model.vert(face[(j + 1) % 3]);
                    int x0 = (int)((v0.X + 1f) * image.Width / 2f);
                    int y0 = (int)((v0.Y + 1f) * image.Height / 2f);
                    int x1 = (int)((v1.X + 1f) * image.Width / 2f);
                    int y1 = (int)((v1.Y + 1f) * image.Height / 2f);
                    Draw.Line(x0, y0, x1, y1, image, white);
                }
            }
        }
    }
}
