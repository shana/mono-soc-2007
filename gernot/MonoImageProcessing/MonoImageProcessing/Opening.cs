//
// Mono.ImageProcessing.Morphology.Opening.cs:
// Morphologically open image.
// 
// Author:
//   Gernot Margreitner (gmargreitner@gmail.com)
//
// Copyright (C) Gernot Margreitner, 2007
// 

namespace Mono.ImageProcessing.Morphology
{
        /// <summary>
        /// Mathematical morphology <see cref="Opening"/> operator.
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <example>
        /// StructuringElement se = StructuringElement.CreateSquare(3);
        /// Opening opening_op = new Opening(se);
        /// Bitmap bmp = Bitmap.FromFile("johndoe.jpg") as Bitmap;
        /// Matrix img_matrix = new Matrix(bmp);
        /// // create binary image by thresholding
        /// Matrix thres_matrix = img_matrix &lt; 100;
        /// Matrix result = opening_op.Execute(thres_matrix);
        /// </example>
        public class Opening : IOperation {
                /// <summary>
                /// Determines the precise effect of the opening.
                /// </summary>
                private StructuringElement se;

                /// <summary>
                /// Initializes a new instance of the Opening class with the
                /// given <see cref="StructuringElement"/>.
                /// </summary>
                /// <param name="se">
                /// The <see cref="StructuringElement"/> to use for opening.
                /// </param>
                public Opening (StructuringElement se)
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
                /// <returns> The opened <see cref="Matrix"/>. </returns>
                public Matrix Execute (Matrix src)
                {
                        Erosion erosion = new Erosion (this.se);
                        Dilation dilation = new Dilation (this.se);

                        return (dilation.Execute (erosion.Execute (src)));
                }
        }
}
