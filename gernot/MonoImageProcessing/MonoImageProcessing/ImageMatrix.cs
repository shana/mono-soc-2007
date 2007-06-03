//
// Mono.ImageProcessing.ImageMatrix.cs:
// Represents an image as matrix.
// 
// Author:
//   Gernot Margreitner (gmargreitner@gmail.com)
//
// Copyright (C) Gernot Margreitner, 2007
// 

using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Mono.ImageProcessing
{
        /// <summary>
        /// Represents an image as matrix.
        /// </summary>
        public class ImageMatrix : Matrix
        {
                /// <summary>
                /// Initializes a new instance of the <see cref="ImageMatrix"/>
                /// class from the given existing Bitmap.
                /// </summary>
                /// <param name="bmp"></param>
                public ImageMatrix (Bitmap bmp) : this (bmp.Height, bmp.Width)
                {
                        Bmp2GrayscaleMatrix(bmp);
                }

                /// <summary>
                /// Initializes a new instance of the <see cref="ImageMatrix"/>
                /// class with the given dimensions.
                /// </summary>
                /// <param name="height"> 
                /// Height of the <see cref="ImageMatrix"/>
                /// </param>
                /// <param name="width">
                /// Width of the <see cref="ImageMatrix"/>
                /// </param>
                public ImageMatrix (int height, int width) 
                        : base (height, width, MatrixType.Byte)
                {
                }

                /// <summary>
                /// Initializes the values of the created 
                /// <see cref="ImageMatrix"/> based on the pixel values of the
                /// given Bitmap.
                /// </summary>
                /// <param name="bmp">
                /// The source Bitmap to init the <see cref="ImageMatrix"/>.
                /// </param>
                private void Bmp2GrayscaleMatrix(Bitmap bmp)
                {
                        /*Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);

                        BitmapData src_data = bmp.LockBits(rect, ImageLockMode.ReadOnly, bmp.PixelFormat);
                        IntPtr src_ptr = src_data.Scan0;

                        int rgb_bytes = (bmp.Width * bmp.Height * 3) + bmp.Height;

                        byte[] rgb_values = new byte[rgb_bytes];

                        System.Runtime.InteropServices.Marshal.Copy(src_ptr, rgb_values, 0, rgb_bytes);

                        int rgb_index = 0;

                        for (int i = 0; i < bmp.Height; i++)
                        {
                                for (int j = 0; j < bmp.Width; j++, rgb_index += 3)
                                {
                                        matrix[j, i] = rgb_values[rgb_index];
                                }

                                rgb_index++;
                        }

                        bmp.UnlockBits(src_data);*/
                        System.Drawing.Color col;
                        for (int r = 0; r < this.num_rows; r++)
                        for (int c = 0; c < this.num_cols; c++)
                        {
                                col = bmp.GetPixel (c, r);
                                matrix [c, r] = col.R;
                        }
                }


                /// <summary>
                /// Thresholds the given <see cref="ImageMatrix"/> with the 
                /// given value.
                /// </summary>
                /// <param name="matrix">
                /// The <see cref="ImageMatrix"/> to use for thresholding.
                /// </param>
                /// <param name="thres"> The threshold value.</param>
                /// <returns>
                /// A 'logical' <see cref="ImageMatrix"/> which consists of 1s
                /// and 0s.
                /// </returns>
                public static ImageMatrix operator < (ImageMatrix matrix, 
                                                      byte thres)
                {
                        ImageMatrix dest = new ImageMatrix (matrix.Height, 
                                matrix.Width);

                        for (int r = 0; r < matrix.num_rows; r++)
                        for (int c = 0; c < matrix.num_cols; c++)
                        {
                                dest.matrix [c, r] = 
                                        (matrix[c, r] < thres) ? 1 : 0;
                        }

                        return dest;
                }


                /// <summary>
                /// Thresholds the given <see cref="ImageMatrix"/> with the 
                /// given value.
                /// </summary>
                /// <param name="matrix">
                /// The <see cref="ImageMatrix"/> to use for thresholding.
                /// </param>
                /// <param name="thres"> The threshold value.</param>
                /// <returns>
                /// A 'logical' <see cref="ImageMatrix"/> which consists of 1s
                /// and 0s.
                /// </returns>
                public static ImageMatrix operator > (ImageMatrix matrix, 
                                                      byte thres)
                {
                        ImageMatrix dest = new ImageMatrix (matrix.Height, 
                                matrix.Width);

                        for (int r = 0; r < matrix.num_rows; r++)
                        for (int c = 0; c < matrix.num_cols; c++)
                        {
                                dest.matrix [c, r] = 
                                        (matrix[c, r] > thres) ? 1 : 0;
                        }

                        return dest;
                }

                /// <summary>
                /// Thresholds the given <see cref="ImageMatrix"/> with the 
                /// given value.
                /// </summary>
                /// <param name="matrix">
                /// The <see cref="ImageMatrix"/> to use for thresholding.
                /// </param>
                /// <param name="thres"> The threshold value.</param>
                /// <returns>
                /// A 'logical' <see cref="ImageMatrix"/> which consists of 1s
                /// and 0s.
                /// </returns>
                public static ImageMatrix operator <= (ImageMatrix matrix,
                                                       byte thres)
                {
                        return (matrix < ++thres);
                }

                /// <summary>
                /// Thresholds the given <see cref="ImageMatrix"/> with the 
                /// given value.
                /// </summary>
                /// <param name="matrix">
                /// The <see cref="ImageMatrix"/> to use for thresholding.
                /// </param>
                /// <param name="thres"> The threshold value.</param>
                /// <returns>
                /// A 'logical' <see cref="ImageMatrix"/> which consists of 1s
                /// and 0s.
                /// </returns>
                public static ImageMatrix operator >= (ImageMatrix matrix,
                                                       byte thres)
                {
                        return (matrix > --thres);
                }

                /// <summary>
                /// Inverts the <see cref="ImageMatrix"/>.
                /// </summary>
                /// <param name="matrix">
                /// The <see cref="ImageMatrix"/> to invert.
                /// </param>
                /// <returns>The inverted <see cref="ImageMatrix"/>.</returns>
                public static ImageMatrix operator ! (ImageMatrix matrix)
                {
                        ImageMatrix inv = new ImageMatrix(matrix.num_rows,
                                matrix.num_cols);

                        for (int r = 0; r < matrix.num_rows; r++)
                        for (int c = 0; c < matrix.num_cols; c++)
                        {
                                inv[c, r] = (int)System.Math.Abs(255 - matrix[c, r]);
                        }

                        return inv;
                }
        }
}
