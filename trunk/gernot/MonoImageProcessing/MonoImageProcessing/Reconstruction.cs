//
// Mono.ImageProcessing.Morphology.Reconstruction.cs:
// Morphological grayscale reconstruction.
// 
// Author:
//   Gernot Margreitner (gmargreitner@gmail.com)
//
// Copyright (C) Gernot Margreitner, 2007
// 

using System.Collections.Generic;

namespace Mono.ImageProcessing.Morphology
{
        /// <summary>
        /// Morphological reconstruction can be thought of conceptually as 
        /// repeated dilations of the marker image until the contour of the 
        /// marker image fits under the mask image. 
        /// In this way the peaks in the marker image spread out or dilate.
        /// </summary>
        /// <remarks>
        /// In contrast to other morpholobical operations processing is based
        /// on two images, referred as marker and mask.
        /// <para>This implementation is based on the work of
        /// Vincent, L., "Morphological Grayscale Reconstruction in Image 
        /// Analysis: Applications and Efficient Algorithms," 
        /// IEEE Transactions on Image Processing, Vol. 2, No. 2, April, 1993,
        /// pp. 176-201.
        /// [http://www.vincent-net.com/luc/papers/92cvpr_recons.pdf]
        /// </para>
        /// </remarks>
        /// <example>
        /// Bitmap bmp = Bitmap.FromFile("johndoe.jpg") as Bitmap;
        /// Matrix mask = Matrix.FromBitmap(bmp)
        /// Matrix marker = mask - 30;
        /// Matrix recon = reconstruct.Execute(marker, mask);
        /// </example>
        public class Reconstruction {
                /// <summary>
                /// Default constructor.
                /// </summary>
                public Reconstruction ()
                {
                }

                /// <summary>
                /// Performs morphological reconstruction of the image
                /// <paramref name="marker"/> under the image 
                /// <paramref name="mask"/>. <paramref name="marker"/> and
                /// <paramref name="mask"/> can be both grayscale or binary
                /// images with the same size. The element value of the 
                /// <paramref name="marker"/> image must be less than or equal
                /// to the corresponding elements of <paramref name="mask"/>.
                /// </summary>
                /// <param name="marker">The marker image.</param>
                /// <param name="mask">The mask image.</param>
                /// <returns>The morphologically reconstructed image.</returns>
                public Matrix Execute (Matrix marker, Matrix mask)
                {
                        int width = marker.Width;
                        int height = marker.Height;

                        Queue<GrayscalePixel> queue = new Queue<GrayscalePixel> ();

                        Matrix res = marker.Copy ();

                        // scan in raster order
                        // check only 'x' neighborhood pixels of p:
                        // x x x
                        // x p o
                        // o o o
                        for (int r = 0; r < height; r++) {
                                for (int c = 0; c < width; c++) {
                                        byte max = 0;
                                        int y = r - 1;
                                        
                                        //8 connectivity
                                        int q_y = r - 1;
                                        int q_x = -1;
                                        if (q_y > 0)
                                        {
                                                max = (res [c, q_y] > max) ? res [c, q_y] : max;

                                                q_x = c + 1;
                                                if (q_x < width)
                                                        max = (res [q_x, q_y] > max) ? res [q_x, q_y] : max;

                                                q_x = c - 1;
                                                if (q_x > 0)
                                                        max = (res [q_x, q_y] > max) ? res [q_x, q_y] : max;
                                        }

                                        q_x = c - 1;
                                        if (q_x > 0)
                                                max = (res [q_x, r] > max) ? res [q_x, r] : max;

                                        max = (res [c, r] > max) ? res [c, r] : max;

                                        res [c, r] = (max > mask [c, r]) ? mask [c, r] : max;
                                }
                        }

                        // scan in anti-raster order
                        // check only 'x' neighborhood pixels of p:
                        // o o o
                        // o p x
                        // x x x
                        for (int r = height - 1; r >= 0; r--) {
                                for (int c = 0; c < width; c++) {
                                        byte max = 0;
                                        int y = r + 1;

                                        //8 connectivity
                                        int q_y = r + 1;
                                        int q_x = -1;
                                        if (q_y < height)
                                        {
                                                max = (res [c, q_y] > max) ? res [c, q_y] : max;

                                                q_x = c + 1;
                                                if (q_x < width)
                                                        max = (res [q_x, q_y] > max) ? res [q_x, q_y] : max;

                                                q_x = c - 1;
                                                if (q_x > 0)
                                                        max = (res [q_x, q_y] > max) ? res [q_x, q_y] : max;
                                        }

                                        q_x = c + 1;
                                        if (q_x < width)
                                                max = (res [q_x, r] > max) ? res [q_x, r] : max;

                                        max = (res [c, r] > max) ? res [c, r] : max;

                                        res [c, r] = (max > mask [c, r]) ? mask [c, r] : max;

                                        q_y = r + 1;
                                        q_x = -1;
                                        if (q_y < height)
                                        {
                                                if ((res [c, q_y] < res [c,r]) && res [c, q_y] < mask [c,q_y])
                                                {
                                                        queue.Enqueue (new GrayscalePixel (c, r));
                                                        continue;
                                                }

                                                q_x = c + 1;
                                                if (q_x < width)
                                                {
                                                        if ((res [q_x, q_y] < res [c, r]) && res [q_x, q_y] < mask [q_x, q_y])
                                                        {
                                                                queue.Enqueue (new GrayscalePixel (c, r));
                                                                continue;
                                                        }
                                                }

                                                q_x = c - 1;
                                                if (q_x > 0)
                                                {
                                                        if ((res [q_x, q_y] < res [c, r]) && res [q_x, q_y] < mask [q_x, q_y])
                                                        {
                                                                queue.Enqueue (new GrayscalePixel (c, r));
                                                                continue;
                                                        }
                                                }
                                        }

                                        q_x = c + 1;
                                        if (q_x < width)
                                        {
                                                if ((res [q_x, r] < res [c, r]) && res [q_x, r] < mask [q_x, r])
                                                {
                                                        queue.Enqueue (new GrayscalePixel (c, r));
                                                        continue;
                                                }
                                        }
                                }
                        }

                        while (queue.Count > 0)
                        {
                                GrayscalePixel p = queue.Dequeue ();
                                int p_x = p.X;
                                int p_y = p.Y;
                                int q_x = 0;
                                int q_y = 0;

                                for (int i = 0; i < 3; i++) {
                                        for (int j = 0; j < 3; j++) {
                                                q_y = p_y - 1;

                                                if (q_y > 0)
                                                {
                                                        q_x = p_x - 1;
                                                        if (q_x > 0)
                                                        {
                                                                if ((res [q_x, q_y] < res [p_x, p_y]) &&
                                                                        (mask [q_x, q_y] != res [q_x, q_y]))
                                                                {
                                                                        res[q_x, q_y] = (mask[q_x, q_y] > res[p_x, p_y]) ? marker[p_x, p_y] : mask[q_x, q_y];
                                                                        queue.Enqueue (new GrayscalePixel (q_x, q_y));
                                                                }
                                                        }
                                                }

                                        }
                                }
                        }

                        return (res);
                }
        }
}
