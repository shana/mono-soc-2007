//
// Mono.ImageProcessing.Matrix.cs: 
// Basic mathematical matrix.
//
// Author:
//   Gernot Margreitner (gmargreitner@gmail.com)
//
// Copyright (C) Gernot Margreitner, 2007
// 

using System;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace Mono.ImageProcessing
{
        public class Matrix
        {
                private enum MatrixType
                {
                        Logical,
                        Grayscale
                }

                protected int num_rows;
                protected int num_cols;
                public byte [,] matrix;
                private MatrixType type;
                private static byte max_val = 1; //byte.MaxValue;
                private static byte min_val = byte.MinValue;

                /// <summary>
                /// Gets the number of rows in the matrix.
                /// </summary>
                public int Height
                {
                        get { return this.num_rows; }
                }

                /// <summary>
                /// Gets the number of columns in the matrix.
                /// </summary>
                public int Width
                {
                        get { return this.num_cols; }
                }

                /// <summary>
                /// Indicates whether all matrix elements are nonzero.
                /// </summary>
                public bool All
                {
                        get 
                        { 
                                for (int r = 0; r < this.num_rows; r++)
                                for (int c = 0; c < this.num_cols; c++)
                                {
                                        if (this.matrix [c, r] == min_val)
                                                return false;
                                }

                                return true;
                        }
                }

                /// <summary>
                /// Indicates whether all matrix elements are nonzero.
                /// </summary>
                public bool Any
                {
                        get 
                        {
                                for (int r = 0; r < this.num_rows; r++)
                                for (int c = 0; c < this.num_cols; c++)
                                {
                                        if (this.matrix[c, r] > min_val)
                                                return true;
                                }

                                return false;
                        }
                }

                /// <summary>
                /// Initializes a new instance of the <see cref="Matrix"/>
                /// class with the given dimensions.
                /// </summary>
                /// <param name="num_rows">Number of matrix rows.</param>
                /// <param name="num_cols">Number of matrix cols.</param>
                public Matrix (int num_rows, int num_cols)
                {
                        if (num_rows < 1 || num_cols < 1)
                                throw new ArgumentException();

                        this.num_rows = num_rows;
                        this.num_cols = num_cols;

                        this.matrix = new byte [num_cols, num_rows];

                        this.type = MatrixType.Grayscale;
                }

                public byte this [int x, int y]
                {
                        get { return this.matrix [x, y]; }

                        set
                        {
                                this.matrix [x, y] = value;
                        }
                }

                public override string ToString ()
                {
                        StringBuilder matrix_builder = new StringBuilder();
                        for (int row_idx = 0; row_idx < this.num_rows; row_idx++)
                        {
                                for (int col_idx = 0; col_idx < this.num_cols; col_idx++)
                                {
                                        matrix_builder.AppendFormat("{0},", matrix[col_idx, row_idx]);
                                }
                                matrix_builder.Remove(matrix_builder.Length - 1, 1);
                                matrix_builder.Append("\n");
                        }

                        return matrix_builder.ToString();
                }

                public Bitmap ToBitmap()
                {
                        return (type == MatrixType.Logical) ? ToBitmapBinary() : ToBitmapGrayscale();
                }

                private Bitmap ToBitmapBinary()
                {
                        Bitmap bmp = new Bitmap(this.num_cols, this.num_rows);
                        byte r, g, b;

                        for (int row_idx = 0; row_idx < this.num_rows; row_idx++)
                        for (int col_idx = 0; col_idx < this.num_cols; col_idx++)
                        {
                                r = g = b = (this.matrix[col_idx, row_idx] > 0) ? byte.MaxValue : byte.MinValue;
                                bmp.SetPixel(col_idx, row_idx, Color.FromArgb(r, g, b));
                        }

                        return bmp;
                }

                private Bitmap ToBitmapGrayscale()
                {
                        Bitmap bmp = new Bitmap(this.num_cols, this.num_rows);
                        byte r, g, b;

                        for (int row_idx = 0; row_idx < this.num_rows; row_idx++)
                        for (int col_idx = 0; col_idx < this.num_cols; col_idx++)
                        {
                                r = g = b = this.matrix[col_idx, row_idx];
                                bmp.SetPixel(col_idx, row_idx, Color.FromArgb(r, g, b));
                        }

                        return bmp;
                }


                public override bool Equals(object obj)
                {
                        bool isMatrix = obj is Matrix;

                        if (!isMatrix)
                                return false;

                        Matrix m2 = obj as Matrix;

                        if (this.num_rows != m2.num_rows || this.num_cols != m2.num_cols)
                                return false;

                        for (int r = 0; r < this.num_rows; r++)
                        for (int c = 0; c < this.num_cols; c++)
                        {
                                if (this.matrix [c, r] != m2.matrix [c, r])
                                        return false;
                        }

                        return true;
                }

                public override int GetHashCode()
                {
                        return base.GetHashCode();
                }

                public static Matrix FromBitmap(Bitmap bmp)
                {
                        // TODO: check if grayscale

                        Matrix m = new Matrix(bmp.Height, bmp.Width);

                        /*Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);

                        BitmapData src_data = bmp.LockBits(rect, ImageLockMode.ReadOnly, bmp.PixelFormat);
                        IntPtr src_ptr = src_data.Scan0;

                        int rgb_bytes = (bmp.Width * bmp.Height * 3) + bmp.Height;

                        byte[] rgb_values = new byte[rgb_bytes];

                        System.Runtime.InteropServices.Marshal.Copy(src_ptr, rgb_values, 0, rgb_bytes);

                        int rgb_index = 0;

                        for (int r = 0; r < bmp.Height; r++)
                        {
                                for (int c = 0; c < bmp.Width; c++, rgb_index += 3)
                                {
                                        m.matrix[c, r] = rgb_values[rgb_index];
                                }

                                rgb_index++;
                        }

                        bmp.UnlockBits(src_data);
                        return m;*/

                        for (int r = 0; r < bmp.Height; r++)
                        {
                                for (int c = 0; c < bmp.Width; c++)
                                {
                                		System.Drawing.Color col = bmp.GetPixel(c,r);                                		
                                        m.matrix [c, r] = col.R;
                                }
                        }
                        
                        return m;
                }

                /// <summary>
                /// Creates an n-by-n identity matrix.
                /// </summary>
                /// <param name="n">The number of columns and rows in the
                /// matrix.</param>
                /// <returns>The n-by-n identity matrix.</returns>
                public static Matrix Identity (int n)
                {
                        if (n < 2)
                                throw new ArgumentException();

                        Matrix m = new Matrix(n, n);

                        while (--n >= 0)
                        {
                                m.matrix[n, n] = max_val;
                        }

                        return m;
                }

                /// <summary>
                /// Creates a matrix of all ones.
                /// </summary>
                /// <param name="num_rows">Number of matrix rows.</param>
                /// <param name="num_cols">Number of matrix columns.</param>
                /// <returns>An m-by-n matrix of ones</returns>
                public static Matrix Ones (int num_rows, int num_cols)
                {
                        if (num_rows < 1 || num_cols < 1)
                                throw new ArgumentException();

                        Matrix m = new Matrix(num_rows, num_cols);

                        for (int r = 0; r < m.num_rows; r++)
                        for (int c = 0; c < m.num_cols; c++)
                        {
                                m.matrix[c, r] = max_val;
                        }

                        return m;
                }

                /// <summary>
                /// Copies both the data for this <see cref="Matrix"/>.
                /// </summary>
                /// <returns>A copy of the <see cref="Matrix"/></returns>
                public Matrix Copy()
                {
                        Matrix m = new Matrix(this.num_rows, this.num_cols);

                        for (int r = 0; r < m.num_rows; r++)
                        for (int c = 0; c < m.num_cols; c++)
                        {
                                m.matrix[c, r] = this.matrix[c, r];
                        }

                        return m;
                }


                public static Matrix operator != (Matrix m, int val)
                {
                        Matrix m2 = new Matrix(m.num_rows, m.num_cols);

                        for (int r = 0; r < m.num_rows; r++)
                        for (int c = 0; c < m.num_cols; c++)
                        {
                                m2.matrix[c, r] = (m.matrix[c, r] != val) ? 
                                                        max_val : min_val;
                        }

                        m2.type = MatrixType.Logical;
                        return m2;
                }

                public static Matrix operator == (Matrix m, int val)
                {
                        Matrix m2 = new Matrix (m.num_rows, m.num_cols);

                        for (int r = 0; r < m.num_rows; r++)
                        for (int c = 0; c < m.num_cols; c++)
                        {
                                m2.matrix[c, r] = (m.matrix[c, r] == val) ? 
                                                        max_val : min_val;
                        }

                        m2.type = MatrixType.Logical;
                        return m2;
                }

                public static Matrix operator != (Matrix m1, Matrix m2)
                {
                        if (m1.num_rows != m2.num_rows || m1.num_cols != m2.num_cols)
                                throw new ArgumentException();

                        Matrix m3 = new Matrix (m1.num_rows, m1.num_cols);

                        for (int r = 0; r < m1.num_rows; r++)
                        for (int c = 0; c < m1.num_cols; c++)
                        {
                                m3.matrix [c, r] = (m1.matrix [c, r] != m2.matrix [c, r]) ?
                                                        max_val : min_val;
                        }

                        m3.type = MatrixType.Logical;
                        return m3;
                }

                public static Matrix operator == (Matrix m1, Matrix m2)
                {
                        if (m1.num_rows != m2.num_rows || m1.num_cols != m2.num_cols)
                                throw new ArgumentException();

                        Matrix m3 = new Matrix (m1.num_rows, m1.num_cols);

                        for (int r = 0; r < m1.num_rows; r++)
                        for (int c = 0; c < m1.num_cols; c++)
                        {
                                m3.matrix [c, r] = (m1.matrix [c, r] == m2.matrix [c, r]) ?
                                                        max_val : min_val;
                        }

                        m3.type = MatrixType.Logical;
                        return m3;
                }

                public static Matrix operator + (Matrix m1, Matrix m2)
                {
                        if (m1.num_rows != m2.num_rows || m1.num_cols != m2.num_cols)
                                throw new ArgumentException();

                        Matrix m3 = new Matrix(m1.num_rows, m2.num_cols);

                        int val = 0;

                        for (int r = 0; r < m1.num_rows; r++)
                        for (int c = 0; c < m1.num_cols; c++)
                        {
                                val = m1.matrix [c, r] + m2.matrix [c, r];
                                m3.matrix[c, r] = (max_val < val) ? 
                                                        max_val : (byte)val;
                                        
                        }

                        return m3;
                }

                public static Matrix operator - (Matrix m1, Matrix m2)
                {
                        if (m1.num_rows != m2.num_rows || m1.num_cols != m2.num_cols)
                                throw new ArgumentException();

                        Matrix m3 = new Matrix(m1.num_rows, m1.num_cols);

                        int val = 0;

                        for (int r = 0; r < m1.num_rows; r++)
                        for (int c = 0; c < m1.num_cols; c++)
                        {
                                val = m1.matrix[c, r] - m2.matrix[c, r];
                                m3.matrix[c, r] = (min_val > val) ?
                                                        min_val : (byte)val;
                        }

                        return m3;
                }

                public static Matrix operator + (Matrix m1, int value)
                {
                        Matrix m3 = new Matrix(m1.num_rows, m1.num_cols);

                        int sum = 0;

                        for (int r = 0; r < m1.num_rows; r++)
                        for (int c = 0; c < m1.num_cols; c++)
                        {
                                sum = m1.matrix[c, r] + value;
                                m3.matrix[c, r] = (max_val < sum) ?
                                                        max_val : (byte)sum;

                        }

                        return m3;
                }

                public static Matrix operator - (Matrix m1, int value)
                {
                        Matrix m3 = new Matrix(m1.num_rows, m1.num_cols);

                        int sum = 0;

                        for (int r = 0; r < m1.num_rows; r++)
                        for (int c = 0; c < m1.num_cols; c++)
                        {
                                sum = m1.matrix[c, r] - value;
                                m3.matrix[c, r] = (min_val > sum) ?
                                                        min_val : (byte)sum;
                        }

                        return m3;
                }

                public static Matrix operator < (Matrix m, int val)
                {
                        Matrix m2 = new Matrix (m.num_rows, m.num_cols);

                        for (int r = 0; r < m.num_rows; r++)
                        for (int c = 0; c < m.num_cols; c++)
                        {
                                m2.matrix [c, r] = (m.matrix [c, r] < val) ? 
                                                        max_val : min_val;
                        }

                        m2.type = MatrixType.Logical;
                        return m2;
                }

                public static Matrix operator > (Matrix m, int val)
                {
                        Matrix m2 = new Matrix(m.num_rows, m.num_cols);

                        for (int r = 0; r < m.num_rows; r++)
                        for (int c = 0; c < m.num_cols; c++)
                        {
                                m2.matrix [c, r] = (m.matrix [c, r] > val) ?
                                                        max_val : min_val;
                        }

                        m2.type = MatrixType.Logical;
                        return m2;
                }


                public static Matrix operator <= (Matrix m, int val)
                {
                        return (m < ++val);
                }

                public static Matrix operator >= (Matrix m, int val)
                {
                        return (m > --val);
                }


                public static Matrix operator ! (Matrix m)
                {
                        Matrix inv = new Matrix(m.num_rows, m.num_cols);
                        byte max = (m.type == MatrixType.Logical) ? (byte)1 : byte.MaxValue;

                        for (int r = 0; r < m.num_rows; r++)
                        for (int c = 0; c < m.num_cols; c++)
                        {
                                inv.matrix[c, r] = (byte)(max - m.matrix[c, r]);
                        }

                        return inv;
                }

                public static Matrix operator & (Matrix m1, Matrix m2)
                {
                        if (m1.num_rows != m2.num_rows || m1.num_cols != m2.num_cols)
                                throw new ArgumentException();


                        Matrix AND = new Matrix(m1.num_rows, m1.num_cols);

                        for (int r = 0; r < m1.num_rows; r++)
                        for (int c = 0; c < m1.num_cols; c++)
                        {
                                AND.matrix[c, r] = ((m1.matrix[c, r] > 0) &&
                                                        (m2.matrix[c, r] > 0)) ?
                                                        max_val : min_val;
                        }

                        AND.type = MatrixType.Logical;
                        return AND;
                }

                public bool isLogical()
                {
                        return (this.type == MatrixType.Logical);
                }

                public void setLogical()
                {
                        this.type = MatrixType.Logical;
                }

                

        }
}
