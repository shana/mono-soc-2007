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
using System.Collections.Generic;
using System.Text;

namespace Mono.ImageProcessing
{
        public class Matrix
        {
                public enum MatrixType
                {
                        Logical,
                        Byte
                }

                protected int num_rows;
                protected int num_cols;
                protected int [,] matrix;
                protected MatrixType type;
                private int max_val;
                private int min_val;

                private bool load_data = false;
                private bool all = false;
                private bool any = false;

                public int Height
                {
                        get { return this.num_rows; }
                }

                public int Width
                {
                        get { return this.num_cols; }
                }

                public MatrixType Type
                {
                        get { return this.type; }

                        set
                        {
                                this.type = value;
                                this.setMinMax();
                        }
                }

                public bool All
                {
                        get { return all; }
                }

                public bool Any
                {
                        get { return any; }
                }

                public Matrix(int num_rows, int num_cols, MatrixType type)
                {
                        this.num_rows = num_rows;
                        this.num_cols = num_cols;

                        matrix = new int [num_cols, num_rows];

                        this.type = type;
                        this.setMinMax ();
                }

                private void setMinMax ()
                {
                        switch (this.type)
                        {
                                case MatrixType.Logical:
                                        this.max_val = 1;
                                        this.min_val = 0;
                                        break;
                                case MatrixType.Byte:
                                        this.max_val = byte.MaxValue;
                                        this.min_val = byte.MinValue;
                                        break;
                                default:
                                        this.max_val = 0;
                                        this.min_val = 0;
                                        break;
                        }
                }

                public int this [int x, int y]
                {
                        get { return matrix [x, y]; }

                        set
                        {
                                matrix [x, y] = value;

                                if (!this.load_data)
                                        this.UpdateAllAndAnyProperties ();
                        }
                }

                public void BeginLoadData ()
                {
                        this.load_data = true;
                }

                public void EndLoadData ()
                {
                        this.load_data = false;
                        this.UpdateAllAndAnyProperties ();
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

                public override bool Equals(object obj)
                {
                        return base.Equals(obj);
                }

                public override int GetHashCode()
                {
                        return base.GetHashCode();
                }



                protected void UpdateAllAndAnyProperties()
                {
                        this.any = false;
                        this.all = true;

                        for (int row_idx = 0; row_idx < this.num_rows; row_idx++)
                                for (int col_idx = 0; col_idx < this.num_cols; col_idx++)
                                {
                                        int current_val = this[col_idx, row_idx];

                                        if (current_val == this.min_val)
                                                this.all = false;

                                        if (current_val == this.max_val)
                                                this.any = true;

                                        if (!this.all && this.any)
                                                return;
                                }
                }


                public static Matrix CreateIdentity(int size)
                {
                        Matrix matrix = new Matrix(size, size, MatrixType.Logical);

                        while (--size >= 0)
                        {
                                matrix[size, size] = matrix.max_val;
                        }

                        return matrix;
                }

                public static Matrix Ones(int num_rows, int num_cols)
                {
                        Matrix matrix = new Matrix(num_rows, num_cols, MatrixType.Logical);

                        for (int row_idx = 0; row_idx < matrix.num_rows; row_idx++)
                                for (int col_idx = 0; col_idx < matrix.num_cols; col_idx++)
                                {
                                        matrix[col_idx, row_idx] = matrix.max_val;
                                }

                        return matrix;
                }
                
                public static bool operator !=(Matrix m, int watermark)
                {
                        for (int row_idx = 0; row_idx < m.num_rows; row_idx++)
                                for (int col_idx = 0; col_idx < m.num_cols; col_idx++)
                                {
                                        m[col_idx, row_idx] = (m[col_idx, row_idx] != watermark) ? m.max_val : m.min_val;
                                }

                        m.type = MatrixType.Logical;

                        return true;
                }

                public static bool operator ==(Matrix m, int watermark)
                {
                        for (int row_idx = 0; row_idx < m.num_rows; row_idx++)
                                for (int col_idx = 0; col_idx < m.num_cols; col_idx++)
                                {
                                        m[col_idx, row_idx] = (m[col_idx, row_idx] != watermark) ? m.max_val : m.min_val;
                                }

                        m.type = MatrixType.Logical;
                        return true;
                }
        }
}
