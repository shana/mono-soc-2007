//
// Mono.ImageProcessing.Morphology.BinaryHitMiss.cs:
// Binary hit-miss operation.
// 
// Author:
//   Gernot Margreitner (gmargreitner@gmail.com)
//
// Copyright (C) Gernot Margreitner, 2007
// 

namespace Mono.ImageProcessing.Morphology
{
        /// <summary>
        /// Logical AND of an image, eroded with one 
        /// <see cref="StructuringElement"/> and the complement of the image,
        /// eroded with a second <see cref="StructuringElement"/>.
        /// </summary>
        public class BinaryHitMiss : IOperation {
                /// <summary>
                /// Determines the precise effect of the binary hit-miss 
                /// operation.
                /// </summary>
                private StructuringElement se1;

                /// <summary>
                /// Determines the precise effect of the binary hit-miss 
                /// operation.
                /// </summary>
                private StructuringElement se2;


                /// <summary>
                /// Initializes a new instance of the BinaryHitMiss class with 
                /// the given <see cref="StructuringElement"/> objects.
                /// </summary>
                /// <param name="se1">
                /// The <see cref="StructuringElement"/> to use for binary 
                /// hit-miss.
                /// </param>
                /// <param name="se2">
                /// The <see cref="StructuringElement"/> to use for binary 
                /// hit-miss.
                /// </param>
                public BinaryHitMiss (StructuringElement se1, 
                        StructuringElement se2)
                {
                        this.se1 = se1;
                        this.se2 = se2;
                }

                /// <summary>
                /// Performs the <see cref="BinaryHitMiss"/> operator on the given
                /// <see cref="Matrix"/>.
                /// </summary>
                /// <param name="src">
                /// The <see cref="Matrix"/> which should be used by the
                /// operator.
                /// </param>
                /// <returns> The resulting <see cref="Matrix"/>. </returns>
                public Matrix Execute (Matrix src)
                {
                        Erosion erosion1 = new Erosion (this.se1);
                        Erosion erosion2 = new Erosion (this.se2);
                        Matrix bw1 = erosion1.Execute (src);
                        Matrix bw2 = erosion2.Execute (!src);

                        Matrix result = bw1 & bw2;

                        return result;
                }

        }
}
