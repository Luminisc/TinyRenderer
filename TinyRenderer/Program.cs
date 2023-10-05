using System.Diagnostics;
using TinyRenderer;
using TinyRenderer.Lessons;

var white = new TGAColor(255, 255, 255, 255);
var red = new TGAColor(255, 0, 0, 255);
var modelPath = "../../../../african_head.obj";

var width = 1000;
var height = 1000;
var image = new TGAImage(width, height, TGAImage.Format.RGB);

var sw = Stopwatch.StartNew();
// Lesson0.Run(image);
// Lesson1.Run(image, modelPath);
// Lesson2.Run(image);
// Lesson2.RunWithModel(image, modelPath);
Lesson2.RunWithLighting(image, modelPath);

image.flip_vertically(); // i want to have the origin at the left bottom corner of the image
Console.WriteLine($"Draw {sw.ElapsedMilliseconds} ms");

sw.Restart();
image.write_tga_file("../../../../output.tga");
Console.WriteLine($"Stored {sw.ElapsedMilliseconds} ms");

