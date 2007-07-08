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
                /// Private default constructor.
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
                        se.nhood = nhood;

                        /*int height = nhood.Height;
                        int width = nhood.Width;

                        if (neighborhood.All && height > 1 && width > 1)
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

                /// <summary>
                /// Creates a morphological structuring element of type 
                /// 'diamond'. 
                /// </summary>
                /// <example>
                /// StructuringElement.CreateDiamond(4);
                /// Neighborhood = 
                ///         |-- 4 --|
                /// 0,0,0,0,1,0,0,0,0  -
                /// 0,0,0,1,1,1,0,0,0  |
                /// 0,0,1,1,1,1,1,0,0  4
                /// 0,1,1,1,1,1,1,1,0  |
                /// 1,1,1,1,1,1,1,1,1  -
                /// 0,1,1,1,1,1,1,1,0
                /// 0,0,1,1,1,1,1,0,0
                /// 0,0,0,1,1,1,0,0,0
                /// 0,0,0,0,1,0,0,0,0
                /// </example>
                /// <param name="radius">
                /// Specifies the distance from the origin to the border.
                /// </param>
                /// <returns>
                /// The created <see cref="StructuringElement"/>.
                /// </returns>
                public static StructuringElement CreateDiamond (byte radius)
                {
                        int length = 2 * radius + 1;
                        Matrix rr = new Matrix (length, length);
                        Matrix cc = new Matrix (length, length);

                        byte idx_off = 0;
                        
                        //rr.BeginLoadData();
                        //cc.BeginLoadData();

                        for (int r = 0; r < length; r++)
                        {
                                idx_off = radius;

                                while (idx_off > 0) 
                                {
                                        rr[radius - idx_off, r] = idx_off;
                                        rr[radius + idx_off, r] = idx_off;

                                        cc[r, radius - idx_off] = idx_off;
                                        cc[r, radius + idx_off] = idx_off;

                                        idx_off--;
                                } 
                        }

                        //rr.EndLoadData();
                        //cc.EndLoadData();

                        Matrix diamond = rr + cc;
                        diamond = (diamond <= radius);

                        Console.WriteLine(rr.ToString());
                        Console.WriteLine(cc.ToString());
                        Console.WriteLine(diamond.ToString());

                        StructuringElement se = new StructuringElement();
                        se.nhood = diamond;

                        return se;
                }

                /// <summary>
                /// Creates a morphological structuring element of type 
                /// 'octagon'.
                /// </summary>
                /// <example>
                /// StructuringElement.CreateOctagon(3);
                /// Neighborhood = 
                ///       |- 3 -|
                /// 0,0,1,1,1,0,0  -
                /// 0,1,1,1,1,1,0  3
                /// 1,1,1,1,1,1,1  |
                /// 1,1,1,1,1,1,1  -
                /// 1,1,1,1,1,1,1
                /// 0,1,1,1,1,1,0
                /// 0,0,1,1,1,0,0
                /// </example>
                /// <param name="radius">
                /// Specifies the distance from the origin to the border.
                /// </param>
                /// <returns>
                /// The created <see cref="StructuringElement"/>.
                /// </returns>
                public static StructuringElement CreateOctagon (byte radius)
                {
                        int length = 2 * radius + 1;
                        Matrix rr = new Matrix (length, length);
                        Matrix cc = new Matrix (length, length);

                        byte idx_off = 0;
                        
                        //rr.BeginLoadData();
                        //cc.BeginLoadData();

                        for (int r = 0; r < length; r++)
                        {
                                idx_off = radius;

                                while (idx_off > 0) 
                                {
                                        rr[radius - idx_off, r] = idx_off;
                                        rr[radius + idx_off, r] = idx_off;

                                        cc[r, radius - idx_off] = idx_off;
                                        cc[r, radius + idx_off] = idx_off;

                                        idx_off--;
                                } 
                        }

                        //rr.EndLoadData();
                        //cc.EndLoadData();

                        byte k = (byte)(radius / 3);

                        Matrix oct = rr + cc;
                        oct = (oct <= (radius + k));

                        Console.WriteLine(rr.ToString());
                        Console.WriteLine(cc.ToString());
                        Console.WriteLine(oct.ToString());

                        StructuringElement se = new StructuringElement();
                        se.nhood = oct;

                        return se;
                }

                /// <summary>
                /// Creates a morphological structuring element of type 'pair'
                /// with 2 elements which have a logical '1' value. One of 
                /// these 2 elements is the origin in the center of the SE,
                /// the other one is positioned with <paramref name="xOff"/>
                /// and <paramref name="yOff"/> relatively to the origin.
                /// </summary>
                /// <example>
                /// StructuringElement.CreatePair(-2, -3);
                /// Neighborhood = 
                /// 1,0,0,0,0
                /// 0,0,0,0,0
                /// 0,0,0,0,0
                /// 0,0,1,0,0
                /// 0,0,0,0,0
                /// 0,0,0,0,0
                /// 0,0,0,0,0
                /// </example>
                /// <param name="xOff">
                /// The column offset between the origin and the second element
                /// </param>
                /// <param name="yOff">
                /// The row offset between the origin and the second element.
                /// </param>
                /// <returns></returns>
                public static StructuringElement CreatePair (short xOff,
                                                             short yOff)
                {
                        int xAbsOff = System.Math.Abs (xOff);
                        int yAbsOff = System.Math.Abs (yOff);

                        int width = xAbsOff * 2 + 1;
                        int height = yAbsOff * 2 + 1;

                        Matrix pair = new Matrix (height, width);

                        //pair.BeginLoadData ();
                        pair [xAbsOff, yAbsOff] = 1; //origin
                        pair [xAbsOff + xOff, yAbsOff + yOff] = 1;
                        //pair.EndLoadData ();

                        Console.WriteLine (pair.ToString ());

                        StructuringElement se = new StructuringElement ();
                        se.nhood = pair;
                        return se;
                }

                /*public static StructuringElement CreateLine (ushort length,
                                                             byte angle)
                {
                        float theta = (float)((angle * System.Math.PI) / 180.0);
                        double sin_theta = System.Math.Sin(theta);
                        double cos_theta = System.Math.Cos(theta);
                        int x2 = (int)System.Math.Round((length / 2) * cos_theta);
                        int y2 = -(int)System.Math.Round((length / 2) * sin_theta);

                        int x1 = -x2;
                        int y1 = -y2;

                        int dx = System.Math.Abs(x2 - x1);
                        int dy = System.Math.Abs(y2 - y1); 

                        StructuringElement se = new StructuringElement();
                        //se.nhood = pair;
                        return se;
                }*/

                /// <summary>
                /// Creates a morphological structuring element of type 
                /// 'periodic line' with 2 * <paramref name="el"/> + 1 elements
                /// which have a logical '1' value.
                /// </summary>
                /// <example>
                /// StructuringElement.CreatePeriodicLine(2, 2, 1);
                /// Neighborhood = 
                /// 1,0,0,0,0,0,0,0,0
                /// 0,0,1,0,0,0,0,0,0
                /// 0,0,0,0,1,0,0,0,0
                /// 0,0,0,0,0,0,1,0,0
                /// 0,0,0,0,0,0,0,0,1
                /// </example>
                /// <param name="el">
                /// The SE will have 2 * el + 1 elements which have a logical 
                /// '1' value.
                /// </param>
                /// <param name="xOff">
                /// The column offset between two '1' elements.
                /// </param>
                /// <param name="yOff">
                /// The row offset between two '1' elements.
                /// </param>
                /// <returns>
                /// The created <see cref="StructuringElement"/>.
                /// </returns>
                public static StructuringElement CreatePeriodicLine(ushort el,
                                                                    short xOff,
                                                                    short yOff)
                {
                        int length = 2 * el + 1;
                        int colMax = 0;
                        int rowMax = 0;
                        Matrix idx = new Matrix(length, 2);

                        for (int i = 0; i < length; i++)
                        {
                                byte c = (byte)((i - el) * xOff);
                                byte r = (byte)((i - el) * yOff);
                                idx[0, i] = c;
                                idx[1, i] = r;

                                colMax = (c > colMax) ? c : colMax;
                                rowMax = (r > rowMax) ? r : rowMax;
                        }

                        int cols = 2 * colMax + 1;
                        int rows = 2 * rowMax + 1;

                        Matrix nhood = new Matrix(rows, cols);

                        for (int r = 0; r < length; r++)
                        {
                                int x = idx[0, r] + colMax;
                                int y = idx[1, r] + rowMax;
                                nhood[x, y] = 1;
                        }

                        Console.WriteLine(nhood.ToString());
                        
                        StructuringElement se = new StructuringElement();
                        se.nhood = nhood;
                        return se;
                }

                /// <summary>
                /// Creates a morphological structuring element of type 
                /// 'line'.
                /// </summary>
                /// <param name="size">Denotes the length of the line.</param>
                /// <returns>
                /// The created <see cref="StructuringElement"/>.
                /// </returns>
                public static StructuringElement CreateLine(ushort length)
                {
                        Matrix square = Matrix.Ones(1, length);
                        return CreateArbitrary(square);
                }
        }
}
