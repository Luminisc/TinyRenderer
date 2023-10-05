using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyRenderer.Lessons
{
    /// <summary>Draw into TGA</summary>
    internal class Lesson0
    {
        public static void Run(TGAImage image)
        {
            var red = new TGAColor(255, 0, 0, 255);
            image.set(52, 41, red);
        }
    }
}
