using System;
using System.Drawing;
using System.IO;
using Mono.ImageProcessing;
using Mono.ImageProcessing.Morphology;

namespace MonoVision
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			Console.WriteLine("Mono.ImageProcessing");
			Console.WriteLine("Please set desired method call...");
			//TestDilation();
			OutputGrayscaleImages();
		}
		
		public static void OutputGrayscaleImages()
		{
			// Create an instance of StreamReader to read from a file.
            // The using statement also closes the StreamReader.
            using (StreamReader sr = new StreamReader("/media/GERNOTHD2/projects/mono/img.txt"))
            {
                String line;
                // Read and display lines from the file until the end of 
                // the file is reached.
                line = "100075";
                //while ((line = sr.ReadLine()) != null)
                {
                    string source = string.Format(@"/media/GERNOTHD2/projects/mono/bsds300/gray/{0}.jpg", line);
                    Bitmap bmp1 = Bitmap.FromFile(source) as Bitmap;

                    Matrix img_matrix = Matrix.FromBitmap (bmp1); //new Matrix(bmp1);

                    string dest = string.Format("monogray/{0}.txt", line);
                    using (StreamWriter sw = new StreamWriter(dest, false))
                    {
                        sw.Write(img_matrix.ToString());
                    }
                }
            }
			
		}
		
		public static void TestImageMatrix()
		{
			// Create an instance of StreamReader to read from a file.
            // The using statement also closes the StreamReader.
            using (StreamReader sr = new StreamReader("/media/GERNOTHD2/projects/mono/img.txt"))
            {
                String line;
                // Read and display lines from the file until the end of 
                // the file is reached.
                while ((line = sr.ReadLine()) != null)
                {
                    string source = string.Format(@"/media/GERNOTHD2/projects/mono/bsds300/gray/{0}.jpg", line);
                    Bitmap bmp1 = Bitmap.FromFile(source) as Bitmap;

                    Matrix img_matrix = Matrix.FromBitmap (bmp1); //new Matrix(bmp1);

                    string dest = string.Format("monogray/{0}.txt", line);
                    using (StreamWriter sw = new StreamWriter(dest, false))
                    {
                        sw.Write(img_matrix.ToString());
                    }
                }
            }
		}
		
		public static void ThresholdImageMatrix()
		{
			// Create an instance of StreamReader to read from a file.
            // The using statement also closes the StreamReader.
            using (StreamReader sr = new StreamReader("/media/GERNOTHD2/projects/mono/img.txt"))
            {
                String line;
                
                // Read and display lines from the file until the end of 
                // the file is reached.
                while ((line = sr.ReadLine()) != null)
                {
                    Console.WriteLine(line);
                
                    string source = string.Format(@"/media/GERNOTHD2/projects/mono/bsds300/gray/{0}.jpg", line);
                    Bitmap bmp1 = Bitmap.FromFile(source) as Bitmap;

                    Matrix img_matrix = Matrix.FromBitmap (bmp1);
                    Matrix threshold_matrix = (img_matrix < 100);
                    string dest = string.Format("monothres/{0}.txt", line);
                    using (StreamWriter sw = new StreamWriter(dest, false))
                    {
                        sw.Write(threshold_matrix.ToString());
                    }
                }
            }
		}
		
		public static void TestDilation()
		{
              StructuringElement se = StructuringElement.CreateSquare(3);
              Dilation dil = new Dilation(se);
              
			// Create an instance of StreamReader to read from a file.
            // The using statement also closes the StreamReader.
            using (StreamReader sr = new StreamReader("/media/GERNOTHD2/projects/mono/img.txt"))
            {
                String line;
                
                // Read and display lines from the file until the end of 
                // the file is reached.
                while ((line = sr.ReadLine()) != null)
                {
                    Console.WriteLine(line);
                
                    string source = string.Format(@"/media/GERNOTHD2/projects/mono/bsds300/gray/{0}.jpg", line);
                    Bitmap bmp1 = Bitmap.FromFile(source) as Bitmap;

                    Matrix img_matrix = Matrix.FromBitmap(bmp1);
                    Matrix thres_matrix = (img_matrix < 100);
                    Matrix dil_result = dil.Execute(thres_matrix);
                    
                    string dest = string.Format("monodilation/{0}.txt", line);
                    using (StreamWriter sw = new StreamWriter(dest, false))
                    {
                        sw.Write(dil_result.ToString());
                    }
                }
            }
		}		
	}
}