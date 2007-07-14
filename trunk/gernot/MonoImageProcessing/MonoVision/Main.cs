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
                        
                        //this.DemoBottomHat();
                        //this.DemoClosing();
                        this.DemoDilation();
		}

                private static void DemoBottomHat()
                {
                        StructuringElement se = StructuringElement.CreateSquare(3);
                        BottomHat bothat = new BottomHat(se);

                        using (StreamReader sr = new StreamReader("img.txt"))
                        {
                                String line;

                                while ((line = sr.ReadLine()) != null)
                                {
                                        string source = string.Format("grayscale_img/{0}.jpg", line);
                                        Bitmap bmp = Bitmap.FromFile(source) as Bitmap;

                                        Matrix img_matrix = Matrix.FromBitmap(bmp);
                                        //Matrix thres_matrix = img_matrix < 100;
                                        Matrix result = bothat.Execute(img_matrix);

                                        string dest = string.Format("bottomhat/{0}.txt", line);
                                        using (StreamWriter sw = new StreamWriter(dest, false))
                                        {
                                                sw.Write(result.ToString());
                                        }
                                        result.ToBitmap().Save(string.Format("bottomhat/{0}.png", line));
                                }
                        }

                }

                private static void DemoClosing()
                {
                        StructuringElement se = StructuringElement.CreateLine(3);
                        Closing closing = new Closing(se);

                        using (StreamReader sr = new StreamReader("img.txt"))
                        {
                                String line;

                                while ((line = sr.ReadLine()) != null)
                                {
                                        string source = string.Format("grayscale_img/{0}.jpg", line);
                                        Bitmap bmp = Bitmap.FromFile(source) as Bitmap;

                                        Matrix img_matrix = Matrix.FromBitmap(bmp);
                                        //Matrix thres_matrix = img_matrix < 100;
                                        Matrix result = closing.Execute(img_matrix);

                                        string dest = string.Format("closing/{0}.txt", line);
                                        using (StreamWriter sw = new StreamWriter(dest, false))
                                        {
                                                sw.Write(result.ToString());
                                        }
                                        result.ToBitmap().Save(string.Format("closing/{0}.png", line));
                                }
                        }

                }

                private static void DemoDilation()
                {
                        StructuringElement se = StructuringElement.CreateSquare(3);
                        Dilation dil = new Dilation(se);

                        using (StreamReader sr = new StreamReader("img.txt"))
                        {
                                String line;

                                while ((line = sr.ReadLine()) != null)
                                {
                                        string source = string.Format("grayscale_img/{0}.jpg", line);
                                        Bitmap bmp = Bitmap.FromFile(source) as Bitmap;

                                        Matrix img_matrix = Matrix.FromBitmap(bmp);
                                        Matrix thres_matrix = img_matrix < 100;
                                        Matrix result = dil.Execute(thres_matrix);

                                        string dest = string.Format("dilation/{0}.txt", line);
                                        using (StreamWriter sw = new StreamWriter(dest, false))
                                        {
                                                sw.Write(result.ToString());
                                        }
                                        result.ToBitmap().Save(string.Format("dilation/{0}.png", line));
                                }
                        }

                }
	}
}