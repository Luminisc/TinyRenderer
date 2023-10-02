using TinyRenderer;

var white = new TGAColor(255, 255, 255, 255);
var red = new TGAColor(255, 0, 0, 255);

var image = new TGAImage(100, 100, TGAImage.Format.RGB);

image.set(52, 41, red);
image.flip_vertically(); // i want to have the origin at the left bottom corner of the image
image.write_tga_file("output.tga");