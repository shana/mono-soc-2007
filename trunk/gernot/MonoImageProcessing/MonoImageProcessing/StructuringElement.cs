//
// Mono.ImageProcessing.Morphology.StructuringElement.cs:
// Determines the precise details of the effect of a morphological operator on
// the image.
//
// Author:
//   Gernot Margreitner (gmargreitner@gmail.com)
//
// Copyright (C) Gernot Margreitner, 2007
// 

using System;

namespace Mono.ImageProcessing.Morphology
{
        /// <summary>
        /// Mathematical morphology structuring element.
        /// </summary>
        /// <remarks>
        /// The <see cref="StructuringElement"/> consists of a pattern 
        /// specified as the coordinates of a number of discrete points 
        /// relative to some origin. 
        /// Normally cartesian coordinates are used and so a convenient way of 
        /// representing the element is as a small image on a rectangular grid.
        /// </para>
        /// </remarks>
        public class StructuringElement {

                private Matrix nhood;

                public Matrix Neighborhood {
                        get { return nhood; }
                }


                /// <summary>
                /// 
                /// </summary>
                private StructuringElement ()
                {
                }

                /// <summary>
                /// Reflect structuring element.
                /// </summary>
                /// <remarks>
                /// Reflects a <see cref="StructuringElement"/> through its 
                /// center. The effect is the same as if you rotated the 
                /// structuring element's domain 180 degrees around its center
                /// </remarks>
                /// <returns></returns>
                public void Reflect ()
                {
                        throw new NotImplementedException();
                }

                /// <summary>
                /// Creates a morphological structuring element of type 
                /// 'arbitrary'.
                /// </summary>
                /// <remarks>
                /// <paramref name="nhood"/> is a matrix containing 1s and 0s.
                /// The locations of 1s define the neighborhood for the 
                /// morphological operator. The origin of 
                /// <paramref name="nhood"/> is its center element.
                /// </remarks>
                /// <param name="nhood"></param>
                /// <returns>
                /// The created <see cref="StructuringElement"/>.
                /// </returns>
                public static StructuringElement CreateArbitrary (Matrix nhood)
                {
                        StructuringElement se = new StructuringElement();
                        bool tmp = (nhood != 0);
                        se.nhood = nhood;

                        int height = nhood.Height;
                        int width = nhood.Width;

                        /*if (neighborhood.All && height > 1 && width > 1)
                        {
                                // The structuring element has an all-ones neighborhood.
                                // Decide if the SE should be decomposed.

                                int advantage = (height * width) / (height + width);

                                // Heuristic: if the theoretical computation advantage is
                                // minimum a factor of 2, then we assume that the overhead cost
                                // is worth to execute dilation or erosion twice.
                                if (advantage >= 2) {
                                        Matrix sed1 = Matrix.Ones(height, 1);
                                        Matrix sed2 = Matrix.Ones(1, width);

                                        // TODO: add SEDs to queue
                                }
                        }*/

                        return se;
                }

                /// <summary>
                /// Creates a morphological structuring element of type 
                /// 'square'.
                /// </summary>
                /// <param name="size">Denotes the width of the square.</param>
                /// <returns>
                /// The created <see cref="StructuringElement"/>.
                /// </returns>
                public static StructuringElement CreateSquare (ushort size)
                {
                        Matrix square = Matrix.Ones(size, size);
                        return CreateArbitrary(square);
                }


                /// <summary>
                /// Creates a morphological structuring element of type 
                /// 'rectangle'.
                /// </summary>
                /// <param name="width">Width of the rectangle</param>
                /// <param name="height">Height of the rectangle</param>
                /// <returns>
                /// The created <see cref="StructuringElement"/>.
                /// </returns>
                public static StructuringElement CreateRectangle (ushort width,
                                                                 ushort height)
                {
                        Matrix rect = Matrix.Ones(height, width);
                        return CreateArbitrary(rect);
                }
        }
}
