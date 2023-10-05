using System.Diagnostics;
using TinyRenderer;
using TinyRenderer.Lessons;
using static System.Math;

var white = new TGAColor(255, 255, 255, 255);
var red = new TGAColor(255, 0, 0, 255);

var width = 500;
var height = 500;
var image = new TGAImage(width, height, TGAImage.Format.RGB);

var sw = Stopwatch.StartNew();
// Lesson0.Run(image);
Lesson1.Run("../../../../african_head.obj", image);

image.flip_vertically(); // i want to have the origin at the left bottom corner of the image
Console.WriteLine($"Draw {sw.ElapsedMilliseconds} ms");

sw.Restart();
image.write_tga_file("../../../../output.tga");
Console.WriteLine($"Stored {sw.ElapsedMilliseconds} ms");

