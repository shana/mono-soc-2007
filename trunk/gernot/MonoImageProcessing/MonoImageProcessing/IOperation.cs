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
        /// operations applied to instances of type <see cref="Matrix"/>.
        /// </summary>
        public interface IOperation
        {
                /// <summary>
                /// Apply the implemented operation to an Matrix.
                /// </summary>
                /// <remarks>
                /// This method keeps the source <see cref="Matrix"/> unchanged. 
                /// The result of the operation is returned as new 
                /// <see cref="Matrix"/>.
                /// </remarks>
                /// <param name="src">Matrix to apply the operation to.</param>
                /// <returns>The resulting Matrix after the operation has been
                /// applied to <paramref name="src"/>.
                /// </returns>
                /// <seealso cref="Matrix"/>
                Matrix Execute(Matrix src);
        }
}
