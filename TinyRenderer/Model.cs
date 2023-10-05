namespace TinyRenderer
{
    public class Model
    {
        private List<Vec3f> verts_;
        private List<int[]> faces_;

        public unsafe Model(string filename)
        {
            verts_ = new List<Vec3f>();
            faces_ = new List<int[]>();
            var lines = File.ReadAllLines(filename);
            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                if (line.StartsWith("v "))
                {
                    int index = 0;
                    Vec3f v = new();
                    foreach (var number in line.Split(' ').Skip(1).Take(3))
                    {
                        v.Raw[index++] = float.Parse(number.Replace('.', ','), System.Globalization.NumberStyles.Any);
                    }
                    verts_.Add(v);
                }
                else if (line.StartsWith("f "))
                {
                    var f = new int[3];
                    int index = 0;
                    foreach (var number in line.Split(' ').Skip(1).Take(3).SelectMany(x=>x.Split('/').Take(1)))
                    {
                        f[index++] = int.Parse(number)-1;
                    }
                    faces_.Add(f);
                }
            }
            Console.WriteLine($"[Model] Loaded v# {verts_.Count}, f# {faces_.Count}");
        }

        public int nverts()
        {
            return verts_.Count;
        }

        public int nfaces()
        {
            return faces_.Count;
        }

        public Vec3f vert(int i)
        {
            return verts_[i];
        }

        public int[] face(int idx)
        {
            return faces_[idx];
        }
    }
}
