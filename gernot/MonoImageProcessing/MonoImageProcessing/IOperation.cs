//
// Mono.ImageProcessing.IOperation.cs: 
//
// Author:
//   Gernot Margreitner (gmargreitner@gmail.com)
//
// Copyright (C) Gernot Margreitner, 2007
// 

namespace Mono.ImageProcessing
{
    /// <summary>
    /// Defines the set of methods, which should be implemented by all
    /// operations applied to instances of type <see cref="ImageMatrix"/>.
    /// </summary>
    public interface IOperation
    {
        /// <summary>
        /// Apply the implemented operation to an ImageMatrix.
        /// </summary>
        /// <remarks>
        /// This method keeps the source <see cref="ImageMatrix"/> unchanged. 
        /// The result of the operation is returned as new 
        /// <see cref="ImageMatrix"/>.
        /// </remarks>
        /// <param name="src">ImageMatrix to apply the operation to.</param>
        /// <returns>The resulting ImageMatrix after the operation has been
        /// applied to <paramref name="src"/>.
        /// </returns>
        /// <seealso cref="ImageMatrix"/>
        ImageMatrix Execute (ImageMatrix src);
    }
}
