//
// Mono.ImageProcessing.Morphology.BottomHat.cs:
// Morphological bottom hat filtering.
// 
// Author:
//   Gernot Margreitner (gmargreitner@gmail.com)
//
// Copyright (C) Gernot Margreitner, 2007
// 

namespace Mono.ImageProcessing.Morphology
{
        /// <summary>
        /// Subtracts the original image from a morphologically closed version
        /// of the image. Can be used to find intensity troughs in an image.
        /// </summary>
        public class BottomHat : IOperation {
                /// <summary>
                /// Determines the precise effect of the filtering.
                /// </summary>
                private StructuringElement se;

                /// <summary>
                /// Initializes a new instance of the Closing class with the
                /// given <see cref="StructuringElement"/>.
                /// </summary>
                /// <param name="se">
                /// The <see cref="StructuringElement"/> to use for closing.
                /// </param>
                public BottomHat (StructuringElement se)
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
                        Closing closing = new Closing (this.se);

                        Matrix closed = closing.Execute (src);
                        return (src.isLogical() ? 
                                (closed & (!src)) : (closed - src));
                }
        }
}
