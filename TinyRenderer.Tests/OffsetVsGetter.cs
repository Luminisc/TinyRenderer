using System.Diagnostics;
using System.Runtime.InteropServices;
using Xunit.Abstractions;

namespace TinyRenderer.Tests
{
    public class OffsetVsGetter
    {
        private readonly ITestOutputHelper output;

        public OffsetVsGetter(ITestOutputHelper output)
        {
            this.output = output;
        }

        /*
         Simple performance test to determine if getters/setters could be used instead of fieldoffsetted U & V.
         In debug - no, on average:
            Getter/Setter   ~5,300,000 ticks
            Offsetted       ~2,000,000 ticks
            Array           ~2,000,000 ticks
         But in release, it is ~1,900,000 ticks for all three approaches
         */

        [Fact(Skip = "For performance testing only")]
        public void OffsetIsFasterThanGetter()
        {
            var numOfOperations = 100000000;
            var numOfIterations = 10;

            Vec2f v = new()
            {
                X = 1.0f,
                Y = 2.0f
            };

            output.WriteLine($"Offsetted values:");
            for (int iteration = 0; iteration < numOfIterations; iteration++)
            {
                var timestamp = Stopwatch.GetTimestamp();
                var result = 0.0f;
                for (int i = 0; i < numOfOperations; i++)
                {
                    //result += v.U;
                    //result += v.V;
                    v.U += 1.0f;
                    v.V += 2.0f;
                }
                var endstamp = Stopwatch.GetTimestamp();
                output.WriteLine($"[{iteration}] finished in {endstamp - timestamp} ticks");
                v.X += 1.0f;
                v.Y += 2.0f;
            }

            v = new()
            {
                X = 1.0f,
                Y = 2.0f
            };

            output.WriteLine($"Getters:");
            for (int iteration = 0; iteration < numOfIterations; iteration++)
            {
                var timestamp = Stopwatch.GetTimestamp();
                var result = 0.0f;
                for (int i = 0; i < numOfOperations; i++)
                {
                    //result += v.Ugetter;
                    //result += v.Vgetter;
                    v.Ugetter += 1.0f;
                    v.Vgetter += 2.0f;
                }
                var endstamp = Stopwatch.GetTimestamp();
                output.WriteLine($"[{iteration}] finished in {endstamp - timestamp} ticks");
                v.X += 1.0f;
                v.Y += 2.0f;
            }

            v = new()
            {
                X = 1.0f,
                Y = 2.0f
            };

            output.WriteLine($"Array:");
            for (int iteration = 0; iteration < numOfIterations; iteration++)
            {
                var timestamp = Stopwatch.GetTimestamp();
                var result = 0.0f;
                for (int i = 0; i < numOfOperations; i++)
                {
                    unsafe
                    {
                        //result += v.Raw[0];
                        //result += v.Raw[1];
                        v.Raw[0] += 1.0f;
                        v.Raw[1] += 2.0f;
                    }
                }
                var endstamp = Stopwatch.GetTimestamp();
                output.WriteLine($"[{iteration}] finished in {endstamp - timestamp} ticks");
                v.X += 1.0f;
                v.Y += 2.0f;
            }
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct Vec2f
    {
        [FieldOffset(0)]
        public float X;

        [FieldOffset(sizeof(float))]
        public float Y;

        [FieldOffset(0)]
        public float U;

        [FieldOffset(sizeof(float))]
        public float V;

        [FieldOffset(0)]
        public fixed float Raw[2];

        public float Ugetter { get => X; set => X = value; }

        public float Vgetter { get => Y; set => Y = value; }
    }
}