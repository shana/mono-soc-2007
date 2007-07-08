//
// Mono.ImageProcessing.Morphology.Erosion.cs:
// Mathematical morphological dilation.
// 
// Author:
//   Gernot Margreitner (gmargreitner@gmail.com)
//
// Copyright (C) Gernot Margreitner, 2007
// 

namespace Mono.ImageProcessing.Morphology
{
        /// <summary>
        /// Mathematical morphology <see cref="Erosion"/> operator.
        /// </summary>
        /// <remarks>
        /// Erosion is one of the two basic operators in the area of 
        /// mathematical morphology. The basic effect of the operator on a 
        /// binary image is to gradually reduce the boundaries of regions of 
        /// foreground pixels (typically logical '1'). Thus areas of foreground
        /// pixels shrink in size and holes within those regions become larger.
        /// </remarks>
        /// <example>
        /// StructuringElement se = StructuringElement.CreateSquare(3);
        /// Erosion er = new Erosion(se);
        /// Bitmap bmp = Bitmap.FromFile("johndoe.jpg") as Bitmap;
        /// Matrix img_matrix = new Matrix(bmp);
        /// // create binary image by thresholding
        /// Matrix thres_matrix = img_matrix &lt; 100;
        /// Matrix dilation_result = er.Execute(thres_matrix);
        /// </example>
        public class Erosion : IOperation {
                /// <summary>
                /// Determines the precise effect of the erosion.
                /// </summary>
                private StructuringElement se;

                /// <summary>
                /// Initializes a new instance of the Erosion class with the
                /// given <see cref="StructuringElement"/>.
                /// </summary>
                /// <param name="se">
                /// The <see cref="StructuringElement"/> to use for erosion.
                /// </param>
                public Erosion (StructuringElement se)
                {
                        this.se = se;
                }
                /// <summary>
                /// Performs the <see cref="Dilation"/> operator on the given
                /// <see cref="Matrix"/>.
                /// </summary>
                /// <param name="src">
                /// The <see cref="Matrix"/> which should be used by the
                /// operator.
                /// </param>
                /// <returns> The eroded <see cref="Matrix"/>. </returns>
                public Matrix Execute (Matrix src)
                {
                        return (src.isLogical () ? BinaryErosion (src) :
                                GrayscaleErosion(src));
                }

                /// <summary>
                /// Binary erosion.
                /// </summary>
                /// <param name="src">
                /// The <see cref="Matrix"/> which should be used by the
                /// operator.
                /// </param>
                /// <returns>The eroded binary <see cref="Matrix"/>.</returns>
                private Matrix BinaryErosion (Matrix src)
                {
                        int width = src.Width;
                        int height = src.Height;
                        int seWidth = se.Neighborhood.Width;
                        int seHeight = se.Neighborhood.Height;

                        int seXBegin = this.se.Neighborhood.Width / 2;
                        //int seXEnd = src.Width - seXBegin;
                        int seYBegin = this.se.Neighborhood.Height / 2;
                        //int seYEnd = src.Height - seYBegin;

                        Matrix dest = new Matrix (height, width);
                        dest.setLogical ();
                        Matrix sem = this.se.Neighborhood;

                        // for each row in the source image
                        for (int r = 0; r < height; r++) {
                                //for each col in the source image
                                for (int c = 0; c < width; c++) {
                                        //bool match = true;
                                        byte nhd_min = 1;

                                        int y = r - seYBegin;

                                        // for each row in the SE
                                        for (int i = 0; i < seHeight; i++, y++) {
                                                // skip if SE row is outside
                                                if (y < 0 || y >= height)
                                                        continue;

                                                if (nhd_min == 0)
                                                        break;

                                                int x = c - seXBegin;

                                                // for each col in the SE
                                                for (int j = 0; j < seWidth; j++, x++) {
                                                        // skip if SE col is
                                                        // outside
                                                        if (x < 0 || x >= width)
                                                                continue;

                                                        // skip if SE element
                                                        // is 'off'
                                                        if (sem [j, i] == 0)
                                                                continue;

                                                        if (src [x, y] == 0)
                                                        {
                                                                nhd_min = 0;
                                                                break;
                                                        }
                                                }
                                        }

                                        dest [c, r] = nhd_min;
                                }

                        }

                        return dest;
                }

                /// <summary>
                /// Grayscale erosion.
                /// </summary>
                /// <param name="src">
                /// The <see cref="Matrix"/> which should be used by the
                /// operator.
                /// </param>
                /// <returns>The eroded grayscale <see cref="Matrix"/>.</returns>
                private Matrix GrayscaleErosion (Matrix src)
                {
                        int width = src.Width;
                        int height = src.Height;
                        int seWidth = se.Neighborhood.Width;
                        int seHeight = se.Neighborhood.Height;

                        int seXBegin = this.se.Neighborhood.Width / 2;
                        int seYBegin = this.se.Neighborhood.Height / 2;

                        Matrix dest = new Matrix (height, width);
                        Matrix sem = this.se.Neighborhood;

                        // for each row in the source image
                        for (int r = 0; r < height; r++)
                        {
                                //for each col in the source image
                                for (int c = 0; c < width; c++) {
                                        byte nhd_min = 255;

                                        int y = r - seYBegin;

                                        // for each row in the SE
                                        for (int i = 0; i < seHeight; i++, y++) {
                                                // skip if SE row is outside
                                                if (y < 0 || y >= height)
                                                        continue;

                                                int x = c - seXBegin;

                                                // for each col in the SE
                                                for (int j = 0; j < seWidth; j++, x++) {
                                                        // skip if SE col is
                                                        // outside
                                                        if (x < 0 || x >= width)
                                                                continue;

                                                        // skip if SE element
                                                        // is 'off'
                                                        if (sem [j, i] == 0)
                                                                continue;

                                                        nhd_min = (src [x, y] < nhd_min) ?
                                                                        src [x, y] : nhd_min;
                                                }
                                        }

                                        dest [c, r] = nhd_min;
                                }

                        }

                        return dest;
                }
        }
}
