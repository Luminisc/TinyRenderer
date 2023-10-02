using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TinyRenderer
{
    public enum DataTypeCode : byte
    {
        NoImage = 0,
        UncompressedColorMapped = 1,
        UncompressedRgb = 2,
        UncompressedBnW = 3,
        RunlengthColorMapped = 9,
        RunlengthRgb = 10,
        CompressedBnW = 11,
        //CompressedColorMapped_Huffman_Delta_Runlength = 32,
        //CompressedColorMapped_Huffman_Delta_Runlength_FourPassQuadTree = 33,
    }

    public struct TGAHeader
    {
        public byte idlength;
        public byte colormaptype;
        public byte datatypecode;
        public short colormaporigin;
        public short colormaplength;
        public byte colormapdepth;
        public short x_origin;
        public short y_origin;
        public short width;
        public short height;
        public byte bitsperpixel;
        public byte imagedescriptor;

        // it is easier and more secure to read byte by byte from file, rather than c-like reading array to struct
        public static bool ReadHeader(BinaryReader reader, out TGAHeader header)
        {
            header = new TGAHeader();

            header.idlength = reader.ReadByte();
            header.colormaptype = reader.ReadByte();
            header.datatypecode = reader.ReadByte();
            header.colormaporigin = reader.ReadInt16();
            header.colormaplength = reader.ReadInt16();
            header.colormapdepth = reader.ReadByte();
            header.x_origin = reader.ReadInt16();
            header.y_origin = reader.ReadInt16();
            header.width = reader.ReadInt16();
            header.height = reader.ReadInt16();
            header.bitsperpixel = reader.ReadByte();
            header.imagedescriptor = reader.ReadByte();

            return true;
        }

        public bool WriteHeader(BinaryWriter writer)
        {
            writer.Write(idlength);
            writer.Write(colormaptype);
            writer.Write(datatypecode);
            writer.Write(colormaporigin);
            writer.Write(colormaplength);
            writer.Write(colormapdepth);
            writer.Write(x_origin);
            writer.Write(y_origin);
            writer.Write(width);
            writer.Write(height);
            writer.Write(bitsperpixel);
            writer.Write(imagedescriptor);

            return true;
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct TGAColor
    {
        [FieldOffset(0)]
        public fixed byte raw[4];

        [FieldOffset(0)]
        public int val;

        [FieldOffset(0)]
        public byte b;
        [FieldOffset(1)]
        public byte g;
        [FieldOffset(2)]
        public byte r;
        [FieldOffset(3)]
        public byte a;

        [FieldOffset(4)]
        public int bytespp;

        public TGAColor() { val = 0; bytespp = 1; }

        public TGAColor(byte R, byte G, byte B, byte A) { b = B; g = G; r = R; a = A; bytespp = 4; }

        public TGAColor(int v, int bpp) { val = v; bytespp = bpp; }

        public TGAColor(in TGAColor c) { val = c.val; bytespp = c.bytespp; }

        public TGAColor(Span<byte> p, int bpp)
        {
            for (int i = 0; i < bpp; i++)
            {
                raw[i] = p[i];
            }
        }

        // TODO: (something similar to) assign operator override
    }

    public class TGAImage
    {
        protected byte[]? data = null;
        int width = 0;
        int height = 0;
        int bytespp = 0;

        public enum Format : int
        {
            GRAYSCALE = 1, RGB = 3, RGBA = 4
        }

        public TGAImage() { }

        public TGAImage(int w, int h, int bpp)
        {
            width = w;
            height = h;
            bytespp = bpp;
            var nbytes = width * height * bytespp;
            data = new byte[nbytes];
            CPP.Memset(data, 0, nbytes);
        }

        public TGAImage(TGAImage img)
        {
            width = img.width;
            height = img.height;
            bytespp = img.bytespp;
            var nbytes = width * height * bytespp;
            data = new byte[nbytes];
            if (img.data != null)
                CPP.Memcpy(data, img.data, nbytes);
            else
                CPP.Memset(data, 0, nbytes);
        }

        public TGAImage(int w, int h, Format format) : this(w, h, (int)format) { }

        // TODO: (something similar to) assign operator override

        public bool read_tga_file(string filename)
        {
            data = null;
            try
            {
                using var sr = new StreamReader(filename);
                using var br = new BinaryReader(sr.BaseStream);

                if (!TGAHeader.ReadHeader(br, out var header))
                    return false;

                width = header.width;
                height = header.height;
                bytespp = header.bitsperpixel >> 3;
                if (width <= 0 || height <= 0 || (bytespp != (int)Format.GRAYSCALE && bytespp != (int)Format.RGB && bytespp != (int)Format.RGBA))
                {
                    Console.Error.WriteLine("bad bpp (or width/height) value");
                    return false;
                }

                var nbytes = bytespp * width * height;
                data = new byte[nbytes];

                if (header.datatypecode == (byte)DataTypeCode.UncompressedRgb || header.datatypecode == (byte)DataTypeCode.UncompressedBnW)
                {
                    if (nbytes != br.Read(data, 0, nbytes))
                    {
                        Console.Error.WriteLine("an error occured while reading the data");
                        return false;
                    }
                }
                else if (header.datatypecode == (byte)DataTypeCode.RunlengthRgb || header.datatypecode == (byte)DataTypeCode.CompressedBnW)
                {
                    if (!load_rle_data(br))
                    {
                        Console.Error.WriteLine("an error occured while reading the data");
                        return false;
                    }
                }
                else
                {
                    Console.Error.WriteLine("unknown file format");
                    return false;
                }

                if ((header.imagedescriptor & 0x20) == 0)
                {
                    flip_vertically();
                }
                if ((header.imagedescriptor & 0x10) != 0)
                {
                    flip_horizontally();
                }

                Console.WriteLine($"{width}x{height}/{bytespp * 8}");
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Error occured while loading image:");
                Console.Error.WriteLine($"{e.Message}");
                Console.Error.WriteLine($"{e.StackTrace}");
                return false;
            }
            return true;
        }

        public unsafe bool load_rle_data(BinaryReader reader)
        {
            ulong pixelcount = (ulong)width * (ulong)height;
            ulong currentpixel = 0;
            ulong currentbyte = 0;

            TGAColor colorbuffer = new TGAColor();
            do
            {
                byte chunkheader = 0;
                chunkheader = reader.ReadByte();

                if (chunkheader < 128)
                {
                    chunkheader++;
                    for (int i = 0; i < chunkheader; i++)
                    {
                        for (var j = 0; j < bytespp; j++)
                        {
                            var b = reader.ReadByte();
                            colorbuffer.raw[j] = b;
                        }

                        for (int t = 0; t < bytespp; t++)
                            data![currentbyte++] = colorbuffer.raw[t];
                        currentpixel++;
                        if (currentpixel > pixelcount)
                        {
                            Console.Error.WriteLine("Too many pixels read");
                            return false;
                        }
                    }
                }
                else
                {
                    chunkheader -= 127;
                    for (var j = 0; j < bytespp; j++)
                    {
                        var b = reader.ReadByte();
                        colorbuffer.raw[j] = b;
                    }

                    for (int i = 0; i < chunkheader; i++)
                    {
                        for (int t = 0; t < bytespp; t++)
                            data[currentbyte++] = colorbuffer.raw[t];
                        currentpixel++;
                        if (currentpixel > pixelcount)
                        {
                            Console.Error.WriteLine("Too many pixels read");
                            return false;
                        }
                    }
                }

            } while (currentpixel < pixelcount);
            return true;
        }

        public bool write_tga_file(string filename, bool rle = false)
        {
            var developer_area_ref = new byte[] { 0, 0, 0, 0 };
            var extension_area_ref = new byte[] { 0, 0, 0, 0 };
            var footer = new char[] { 'T', 'R', 'U', 'E', 'V', 'I', 'S', 'I', 'O', 'N', '-', 'X', 'F', 'I', 'L', 'E', '.', '\0' };
            try
            {
                using var sw = new StreamWriter(filename, false, Encoding.ASCII);
                using var bw = new BinaryWriter(sw.BaseStream);

                TGAHeader header = new TGAHeader();
                header.bitsperpixel = (byte)(bytespp << 3);
                header.width = (short)width;
                header.height = (short)height;
                header.datatypecode = (bytespp == (int)Format.GRAYSCALE
                    ? (rle ? (byte)11 : (byte)3)
                    : (rle ? (byte)10 : (byte)2));
                header.imagedescriptor = 0x20; // top-left origin

                header.WriteHeader(bw);
                if (!rle)
                {
                    bw.Write(data);
                }
                else
                {
                    unload_rle_data(bw);
                }

                bw.Write(developer_area_ref);
                bw.Write(extension_area_ref);
                bw.Write(footer);
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }

        public bool unload_rle_data(BinaryWriter writer)
        {
            byte max_chunk_length = 128;
            ulong npixels = (ulong)width * (ulong)height;
            ulong curpix = 0;
            while (curpix < npixels)
            {
                ulong chunkstart = curpix * (ulong)bytespp;
                ulong curbyte = curpix * (ulong)bytespp;
                byte run_length = 1;
                bool raw = true;
                while (curpix + run_length < npixels && run_length < max_chunk_length)
                {
                    bool succ_eq = true;
                    for (ulong t = 0; succ_eq && t < (ulong)bytespp; t++)
                    {
                        succ_eq = (data[curbyte + t] == data[curbyte + t + (ulong)bytespp]);
                    }
                    curbyte += (ulong)bytespp;
                    if (1 == run_length)
                    {
                        raw = !succ_eq;
                    }
                    if (raw && succ_eq)
                    {
                        run_length--;
                        break;
                    }
                    if (!raw && !succ_eq)
                    {
                        break;
                    }
                    run_length++;
                }
                curpix += run_length;
                writer.Write((byte)(raw ? run_length - 1 : run_length + 127));
                writer.Write(data, (int)chunkstart, (raw ? run_length * bytespp : bytespp));
            }

            return true;
        }

        public TGAColor get(int x, int y)
        {
            if (data == null || x < 0 || y < 0 || x >= width || y >= height)
            {
                return new TGAColor();
            }
            return new TGAColor(data.AsSpan().Slice((x + y * width) * bytespp, bytespp), bytespp);
        }

        public bool set(int x, int y, TGAColor c)
        {
            if (data == null || x < 0 || y < 0 || x >= width || y >= height)
            {
                return false;
            }
            var value = c.val;
            var offset = 0;
            for (var i = 0; i < bytespp; i++)
            {
                data[(x + y * width) * bytespp + i] = (byte)(value >> offset*8);
                offset++;
            }
            return true;
        }

        public int Bytespp => bytespp;
        public int Width => width;
        public int Height => height;



        private bool flip_horizontally()
        {
            if (data == null) return false;
            int half = width >> 1;
            for (int i = 0; i < half; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    TGAColor c1 = get(i, j);
                    TGAColor c2 = get(width - 1 - i, j);
                    set(i, j, c2);
                    set(width - 1 - i, j, c1);
                }
            }
            return true;
        }

        public bool flip_vertically()
        {
            if (data == null) return false;
            var bytes_per_line = width * bytespp;
            var line = new byte[bytes_per_line];
            var half = height >> 1;
            for (int j = 0; j < half; j++)
            {
                var l1 = j * bytes_per_line;
                var l2 = (height - 1 - j) * bytes_per_line;
                Array.Copy(data, l1, line, 0, bytes_per_line);
                Array.Copy(data, l2, data, l1, bytes_per_line);
                Array.Copy(line, 0, data, l2, bytes_per_line);
            }
            return true;
        }

        public byte[] buffer() => data;

        public void clear() => CPP.Memset(data, 0, data.Length);

        public bool Scale(int w, int h)
        {
            if (w <= 0 || h <= 0 || data == null) return false;
            var tdata = new byte[w * h * bytespp];
            int nscanline = 0;
            int oscanline = 0;
            int erry = 0;
            int nlinebytes = w * bytespp;
            int olinebytes = width * bytespp;
            for (int j = 0; j < height; j++)
            {
                int errx = width - w;
                int nx = -bytespp;
                int ox = -bytespp;
                for (int i = 0; i < width; i++)
                {
                    ox += bytespp;
                    errx += w;
                    while (errx >= (int)width)
                    {
                        errx -= width;
                        nx += bytespp;
                        CPP.Memcpy(tdata, nscanline + nx, data, oscanline + ox, bytespp);
                    }
                }
                erry += h;
                oscanline += olinebytes;
                while (erry >= (int)height)
                {
                    if (erry >= (int)height << 1) // it means we jump over a scanline
                        CPP.Memcpy(tdata, nscanline + nlinebytes, tdata, nscanline, nlinebytes);
                    erry -= height;
                    nscanline += nlinebytes;
                }
            }
            data = tdata;
            width = w;
            height = h;
            return true;
        }
    }
}
