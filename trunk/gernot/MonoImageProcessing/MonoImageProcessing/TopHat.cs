//
// Mono.ImageProcessing.Morphology.TopHat.cs:
// Morphological top hat filtering.
// 
// Author:
//   Gernot Margreitner (gmargreitner@gmail.com)
//
// Copyright (C) Gernot Margreitner, 2007
// 

namespace Mono.ImageProcessing.Morphology
{
        /// <summary>
        /// Subtracts a morphologically opened image from the original image.
        /// Can be used to enhance contrast in an image.
        /// </summary>
        public class TopHat : IOperation {
                /// <summary>           
                /// Determines the precise effect of the filtering.
                /// </summary>
                private StructuringElement se;

                /// <summary>
                /// Initializes a new instance of the TopHat class with the
                /// given <see cref="StructuringElement"/>.
                /// </summary>
                /// <param name="se">
                /// The <see cref="StructuringElement"/> to use for top hat 
                /// filtering.
                /// </param>
                public TopHat (StructuringElement se)
                {
                        this.se = se;
                }

                /// <summary>
                /// Performs the <see cref="TopHat"/> operator on the given
                /// <see cref="Matrix"/>.
                /// </summary>
                /// <remarks>
                /// Performs morphological top-hat filtering on a grayscale or 
                /// binary input image using the applied structuring element.
                /// </remarks>
                /// <param name="src">
                /// The <see cref="Matrix"/> which should be used by the
                /// operator.
                /// </param>
                /// <returns> The filtered <see cref="Matrix"/>. </returns>
                public Matrix Execute (Matrix src)
                {
                        Opening opening = new Opening (this.se);

                        Matrix opened = opening.Execute (src);

                        return ((src.isLogical()) ? 
                                (src & (!opened)) : (src - opened));
                }
        }
}
