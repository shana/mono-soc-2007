//
// Mono.ImageProcessing.GrayscalePixel.cs: 
// 2D pixel coordinates and corresponding grayscale intensity value.
//
// Author:
//   Gernot Margreitner (gmargreitner@gmail.com)
//
// Copyright (C) Gernot Margreitner, 2007
// 

namespace Mono.ImageProcessing.Morphology
{
        /// <summary>
        /// Pixel describes a 2D pixel's coordinates and its intensity value.
        /// </summary>
        public class GrayscalePixel
        {
                /// <summary>
                /// The x coordinate value.
                /// </summary>
                private int x;

                /// <summary>
                /// The y coordinate value.
                /// </summary>
                private int y;

                /// <summary>
                /// The pixel's grayscale intensity value.
                /// </summary>
                private byte val;

                /// <summary>
                /// Gets the x coordinate of the pixel.
                /// </summary>
                public int X {
                        get { return x; }
                        //set { x = value; }
                }

                /// <summary>
                /// Gets the y coordinate of the pixel.
                /// </summary>
                public int Y {
                        get { return y; }
                        //set { y = value; }
                }

                /// <summary>
                /// Gets the grayscale intensitiy value of the pixel.
                /// </summary>
                public byte Value {
                        get { return val; }
                        //set { val = value; }
                }

                /// <summary>
                /// Initializes a new instance of the 
                /// <see cref="GrayscalePixel"/> class.
                /// </summary>
                /// <param name="x">The x coordinate value.</param>
                /// <param name="y">The y coordinate value.</param>
                public GrayscalePixel (int x, int y)
                        : this (x, y, byte.MinValue)
                {
                }

                /// <summary>
                /// Initializes a new instance of the
                /// <see cref="GrayscalePixel"/> class.
                /// </summary>
                /// <param name="x">The x coordinate value.</param>
                /// <param name="y">The y coordinate value.</param>
                /// <param name="val">The grayscale intensity value.</param>
                public GrayscalePixel (int x, int y, byte val)
                {
                        this.x = x;
                        this.y = y;
                        this.val = val;
                }
        }
}
