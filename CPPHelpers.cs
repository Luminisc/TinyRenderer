namespace TinyRenderer
{
    public static class CPP
    {
        public static void Memset(byte[] data, byte value, int size)
        {
            System.Runtime.CompilerServices.Unsafe.InitBlock(ref data[0], value, (uint)size);
        }

        public static void Memcpy(byte[] destination, byte[] source, int size)
        {
            Array.Copy(source, destination, size);
        }

        public static void Memcpy(byte[] destination, int doffset, byte[] source, int soffset, int size)
        {
            Array.Copy(source, soffset, destination, doffset, size);
        }
    }
}
