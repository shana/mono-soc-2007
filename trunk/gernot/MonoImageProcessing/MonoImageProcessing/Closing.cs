//
// Mono.ImageProcessing.Morphology.Closing.cs:
// Morphologically close image.
// 
// Author:
//   Gernot Margreitner (gmargreitner@gmail.com)
//
// Copyright (C) Gernot Margreitner, 2007
// 

namespace Mono.ImageProcessing.Morphology
{
        /// <summary>
        /// Mathematical morphology <see cref="Closing"/> operator.
        /// </summary>
        /// <remarks>
        /// Closing is one of the two basic operators in the area of 
        /// mathematical morphology. The basic effect of the operator on a 
        /// binary image is to gradually reduce the boundaries of regions of 
        /// foreground pixels (typically logical '1'). Thus areas of foreground
        /// pixels shrink in size and holes within those regions become larger.
        /// </remarks>
        /// <example>
        /// StructuringElement se = StructuringElement.CreateSquare(3);
        /// Closing close_op = new Closing(se);
        /// Bitmap bmp = Bitmap.FromFile("johndoe.jpg") as Bitmap;
        /// Matrix img_matrix = new Matrix(bmp);
        /// // create binary image by thresholding
        /// Matrix thres_matrix = img_matrix &lt; 100;
        /// Matrix result = close_op.Execute(thres_matrix);
        /// </example>
        public class Closing : IOperation {
                /// <summary>
                /// Determines the precise effect of the closing.
                /// </summary>
                private StructuringElement se;

                /// <summary>
                /// Initializes a new instance of the Closing class with the
                /// given <see cref="StructuringElement"/>.
                /// </summary>
                /// <param name="se">
                /// The <see cref="StructuringElement"/> to use for closing.
                /// </param>
                public Closing (StructuringElement se)
                {
                        this.se = se;
                }

                /// <summary>
                /// Performs the <see cref="Closing"/> operator on the given
                /// <see cref="Matrix"/>.
                /// </summary>
                /// <param name="src">
                /// The <see cref="Matrix"/> which should be used by the
                /// operator.
                /// </param>
                /// <returns> The closed <see cref="Matrix"/>. </returns>
                public Matrix Execute (Matrix src)
                {
                        Dilation dilation = new Dilation (this.se);
                        Erosion erosion = new Erosion (this.se);

                        return (erosion.Execute (dilation.Execute (src)));
                }
        }
}
