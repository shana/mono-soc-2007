//
// Mono.ImageProcessing.Morphology.Dilation.cs:
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
        /// Mathematical morphology <see cref="Dilation"/> operator.
        /// </summary>
        /// <remarks>
        /// Dilation is one of the two basic operators in the area of 
        /// mathematical morphology. The basic effect of the operator on a 
        /// binary image is to gradually enlarge the boundaries of regions of 
        /// foreground pixels (typically logical '1'). Thus areas of foreground
        /// pixels grow in size and holes within those regions become smaller.
        /// </remarks>
        /// <example>
        /// StructuringElement se = StructuringElement.CreateSquare(3);
        /// Dilation dil = new Dilation(se);
        /// Bitmap bmp = Bitmap.FromFile("johndoe.jpg") as Bitmap;
        /// ImageMatrix img_matrix = new ImageMatrix(bmp);
        /// // create binary image by thresholding
        /// ImageMatrix thres_matrix = img_matrix &lt; 100;
        /// ImageMatrix dilation_result = dil.Execute(thres_matrix);
        /// </example>
        public class Dilation : IOperation {

                /// <summary>
                /// Determines the precise effect of the dilation.
                /// </summary>
                private StructuringElement se;

                /// <summary>
                /// Initializes a new instance of the Dilation class with the
                /// given <see cref="StructuringElement"/>.
                /// </summary>
                /// <param name="se">
                /// The <see cref="StructuringElement"/> to use for dilation.
                /// </param>
                public Dilation (StructuringElement se)
                {
                        this.se = se;
                }


                /// <summary>
                /// Performs the <see cref="Dilation"/> operator on the given
                /// <see cref="ImageMatrix"/>.
                /// </summary>
                /// <param name="src">
                /// The <see cref="ImageMatrix"/> which should be used by the
                /// operator.
                /// </param>
                /// <returns> The dilated <see cref="ImageMatrix"/>. </returns>
                public ImageMatrix Execute (ImageMatrix src)
                {
                        int width = src.Width;
                        int height = src.Height;
                        int seWidth = se.Neighborhood.Width;
                        int seHeight = se.Neighborhood.Height;

                        int seXBegin = this.se.Neighborhood.Width / 2;
                        int seXEnd = src.Width - seXBegin;
                        int seYBegin = this.se.Neighborhood.Height / 2;
                        int seYEnd = src.Height - seYBegin;

                        ImageMatrix dest = new ImageMatrix (height, width);
                        Matrix sem = this.se.Neighborhood;

                        ((Matrix)dest).BeginLoadData();

                        // for each row in the source image
                        for (int r = 0; r < height; r++) {
                                //for each col in the source image
                                for (int c = 0; c < width; c++) {
                                        if (src [c, r] == 0)
                                                continue;

                                        int y = r - seYBegin;

                                        // for each row in the SE
                                        for (int i = 0; i < seHeight; i++, y++) {
                                                // skip if SE row is outside
                                                if (y < 0 || y >= height )
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

                                                        dest [x, y] = 1;
                                                }
                                        }
                                }

                        }
                        ((Matrix)dest).EndLoadData();
                        return dest;
                }

        }
}
