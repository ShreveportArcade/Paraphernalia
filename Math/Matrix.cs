/*
	Matrix class in C#
	Written by Ivan Kuckir (ivan.kuckir@gmail.com, http://blog.ivank.net)
	Faculty of Mathematics and Physics
	Charles University in Prague
	(C) 2010
	- updated on 14.6.2012 - parsing improved. Thanks to Andy!
	- updated on 3.10.2012 - there was a terrible bug in LU, SoLE and Inversion. Thanks to Danilo Neves Cruz for reporting that!
	- updated on 3.18.2014 - adds generalized function on elements, flattened float[,] into float[] to make serializable (Nolan Baker)
	- updated on 5.30.2014 - namespaced and absorbed into Paraphernalia, version control can write these messages now (Nolan Baker)

	This code is distributed under MIT licence.
	
		Permission is hereby granted, free of charge, to any person
		obtaining a copy of this software and associated documentation
		files (the "Software"), to deal in the Software without
		restriction, including without limitation the rights to use,
		copy, modify, merge, publish, distribute, sublicense, and/or sell
		copies of the Software, and to permit persons to whom the
		Software is furnished to do so, subject to the following
		conditions:

		The above copyright notice and this permission notice shall be
		included in all copies or substantial portions of the Software.

		THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
		EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
		OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
		NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
		HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
		WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
		FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
		OTHER DEALINGS IN THE SOFTWARE.
*/

using UnityEngine;
using System.Text.RegularExpressions;

namespace Paraphernalia.Math {
[System.Serializable]
public class Matrix {
	public int rows;
	public int cols;

	public float[] mat;

	public Matrix L;
	public Matrix U;
	private int[] pi;
	private float detOfP = 1;

	// Matrix Class constructor
	public Matrix (int iRows, int iCols) {
		rows = iRows;
		cols = iCols;
		mat = new float[rows * cols];
	}

	public bool IsSquare() {
		return (rows == cols);
	}

	// Access this matrix as a 2D array
	public float this[int iRow, int iCol] {
		get { return mat[iRow * cols + iCol]; }
		set { mat[iRow * cols + iCol] = value; }
	}

	public Matrix GetCol(int k) {
		Matrix m = new Matrix(rows, 1);
		for (int i = 0; i < rows; i++) m[i, 0] = this[i, k];
		return m;
	}

	public void SetCol (Matrix v, int k) {
		for (int i = 0; i < rows; i++) this[i, k] = v[i, 0];
	}

	// Function for LU decomposition
	public void MakeLU() {
		if (!IsSquare()) throw new MException("The matrix is not square!");
		L = IdentityMatrix(rows, cols);
		U = Duplicate();

		pi = new int[rows];
		for (int i = 0; i < rows; i++) pi[i] = i;

		float p = 0;
		float pom2;
		int k0 = 0;
		int pom1 = 0;

		for (int k = 0; k < cols - 1; k++) {
			// find the row with the biggest pivot
			p = 0;
			for (int i = k; i < rows; i++) {
				if (Mathf.Abs(U[i, k]) > p) {
					p = Mathf.Abs(U[i, k]);
					k0 = i;
				}
			}

			// zeros in the column
			if (p == 0) {
				throw new MException("The matrix is singular!");
			}

			pom1 = pi[k]; pi[k] = pi[k0]; pi[k0] = pom1;    // switch two rows in permutation matrix

			for (int i = 0; i < k; i++) {
				pom2 = L[k, i]; L[k, i] = L[k0, i]; L[k0, i] = pom2;
			}

			if (k != k0) detOfP *= -1;

			// Switch rows in U
			for (int i = 0; i < cols; i++) {
				pom2 = U[k, i]; U[k, i] = U[k0, i]; U[k0, i] = pom2;
			}

			for (int i = k + 1; i < rows; i++) {
				L[i, k] = U[i, k] / U[k, k];
				for (int j = k; j < cols; j++)
					U[i, j] = U[i, j] - L[i, k] * U[k, j];
			}
		}
	}

	// Function solves Ax = v in confirmity with solution vector "v"
	public Matrix SolveWith (Matrix v) {
		if (rows != cols) throw new MException("The matrix is not square!");
		if (rows != v.rows) throw new MException("Wrong number of results in solution vector!");
		if (L == null) MakeLU();

		Matrix b = new Matrix(rows, 1);
		for (int i = 0; i < rows; i++) {
			b[i, 0] = v[pi[i], 0];   // switch two items in "v" due to permutation matrix
		}

		Matrix z = SubsForth(L, b);
		Matrix x = SubsBack(U, z);

		return x;
	}

	// Function returns the inverted matrix
	public Matrix Invert() {
		if (L == null) MakeLU();

		Matrix inv = new Matrix(rows, cols);

		for (int i = 0; i < rows; i++) {
			Matrix Ei = Matrix.ZeroMatrix(rows, 1);
			Ei[i, 0] = 1;
			Matrix col = SolveWith(Ei);
			inv.SetCol(col, i);
		}
		return inv;
	}


	// Function for determinant
	public float Det() {
		if (L == null) MakeLU();
		float det = detOfP;
		for (int i = 0; i < rows; i++) {
			det *= U[i, i];
		}
		return det;
	}

   // Function returns permutation matrix "P" due to permutation vector "pi"
	public Matrix GetP() {
		if (L == null) MakeLU();

		Matrix matrix = ZeroMatrix(rows, cols);
		for (int i = 0; i < rows; i++) matrix[pi[i], i] = 1;
		return matrix;
	}

	// Function returns the copy of this matrix
	public Matrix Duplicate() {
		Matrix matrix = new Matrix(rows, cols);
		for (int i = 0; i < rows; i++)
			for (int j = 0; j < cols; j++)
				matrix[i, j] = this[i, j];
		return matrix;
	}

	// Function solves Ax = b for A as a lower triangular matrix
	public static Matrix SubsForth (Matrix A, Matrix b) {
		if (A.L == null) A.MakeLU();
		int n = A.rows;
		Matrix x = new Matrix(n, 1);

		for (int i = 0; i < n; i++) {
			x[i, 0] = b[i, 0];
			for (int j = 0; j < i; j++) x[i, 0] -= A[i, j] * x[j, 0];
			x[i, 0] = x[i, 0] / A[i, i];
		}
		return x;
	}

	// Function solves Ax = b for A as an upper triangular matrix
	public static Matrix SubsBack (Matrix A, Matrix b) {
		if (A.L == null) A.MakeLU();
		int n = A.rows;
		Matrix x = new Matrix(n, 1);

		for (int i = n - 1; i > -1; i--) {
			x[i, 0] = b[i, 0];
			for (int j = n - 1; j > i; j--) x[i, 0] -= A[i, j] * x[j, 0];
			x[i, 0] = x[i, 0] / A[i, i];
		}
		return x;
	}

	// Function generates the zero matrix
	public static Matrix ZeroMatrix(int iRows, int iCols) {
		Matrix matrix = new Matrix(iRows, iCols);
		for (int i = 0; i < iRows; i++)
			for (int j = 0; j < iCols; j++)
				matrix[i, j] = 0;
		return matrix;
	}

	// Function generates the identity matrix
	public static Matrix IdentityMatrix(int iRows, int iCols) {
		Matrix matrix = ZeroMatrix(iRows, iCols);
		for (int i = 0; i < Mathf.Min(iRows, iCols); i++)
			matrix[i, i] = 1;
		return matrix;
	}

	// Function generates the random matrix
	public static Matrix RandomMatrix(int iRows, int iCols, float dispersion) {
		Matrix matrix = new Matrix(iRows, iCols);
		for (int i = 0; i < iRows; i++) {
			for (int j = 0; j < iCols; j++) {
				matrix[i, j] = (2f * Random.value - 1f) * dispersion;
			}
		}
		return matrix;
	}

	// Function parses the matrix from string
	public static Matrix Parse (string ps) {
		string s = NormalizeMatrixString(ps);
		string[] rows = Regex.Split(s, "\r\n");
		string[] nums = rows[0].Split(' ');
		Matrix matrix = new Matrix(rows.Length, nums.Length);
		try {
			for (int i = 0; i < rows.Length; i++) {
				nums = rows[i].Split(' ');
				for (int j = 0; j < nums.Length; j++) matrix[i, j] = float.Parse(nums[j]);
			}
		}
		catch { 
			throw new MException("Wrong input format! "); 
		}
		return matrix;
	}

	public static Matrix ApplyFunc (Matrix matrix, System.Func<float, float> func) {
		Matrix newMatrix = new Matrix(matrix.rows, matrix.cols);
		for (int i = 0; i < matrix.rows; i++) {
			for (int j = 0; j < matrix.cols; j++){
				newMatrix[i,j] = func(matrix[i,j]);
			}
		}
		return newMatrix;
	}

	public static Matrix ApplyFunc (Matrix A, Matrix B, System.Func<float, float, float> func) {
		if (A.rows != B.rows || A.cols != B.cols) throw new MException("Dimensions of A and B don't match");

		Matrix newMatrix = new Matrix(A.rows, A.cols);
		for (int i = 0; i < A.rows; i++) {
			for (int j = 0; j < A.cols; j++){
				newMatrix[i,j] = func(A[i,j], B[i,j]);
			}
		}
		return newMatrix;
	}


	public static Matrix ApplyFunc (Matrix A, Matrix B, Matrix C, System.Func<float, float, float, float> func) {
		if (A.rows != B.rows || A.cols != B.cols || A.rows != C.rows || A.cols != C.cols) {
			throw new MException("Dimensions of don't match");
		}

		Matrix newMatrix = new Matrix(A.rows, A.cols);
		for (int i = 0; i < A.rows; i++) {
			for (int j = 0; j < A.cols; j++){
				newMatrix[i,j] = func(A[i,j], B[i,j], C[i,j]);
			}
		}
		return newMatrix;
	}

	public static Matrix ApplyFunc (Matrix[] matrices, System.Func<float[], float> func) {
		int rows = matrices[0].rows;
		int cols = matrices[0].cols;
		int mats = matrices.Length;
		for (int i = 1; i < mats; i++) {
			if (rows != matrices[i].rows || cols != matrices[i].cols) {
				throw new MException("Wrong dimension of matrix!");
			}
		}

		float[] vals = new float[mats];
		Matrix newMatrix = new Matrix(rows, cols);
		for (int i = 0; i < rows; i++) {
			for (int j = 0; j < cols; j++){
				for (int k = 0; k < mats; k++) {
					vals[k] = matrices[k][i,j];
				}
				newMatrix[i,j] = func(vals);
			}
		}
		return newMatrix;
	}

	// Function returns matrix as a string
	public override string ToString() {
		string s = "";
		for (int i = 0; i < rows; i++) {
			for (int j = 0; j < cols; j++) {
				s += string.Format("{0,5:0.00}", this[i, j]) + " ";
			}
			s += "\r\n";
		}
		return s;
	}

	// Matrix transpose, for any rectangular matrix
	public static Matrix Transpose (Matrix m) {
		Matrix t = new Matrix(m.cols, m.rows);
		for (int i = 0; i < m.rows; i++) {
			for (int j = 0; j < m.cols; j++) {
				t[j, i] = m[i, j];
			}
		}
		return t;
	}

	// Power matrix to exponent
	public static Matrix Power (Matrix m, int pow) {
		if (pow == 0) return IdentityMatrix(m.rows, m.cols);
		if (pow == 1) return m.Duplicate();
		if (pow == -1) return m.Invert();

		Matrix x;
		if (pow < 0) { 
			x = m.Invert(); pow *= -1; 
		}
		else {
			x = m.Duplicate();
		}

		Matrix ret = IdentityMatrix(m.rows, m.cols);
		while (pow != 0) {
			if ((pow & 1) == 1) ret *= x;
			x *= x;
			pow >>= 1;
		}
		return ret;
	}

	private static void SafeAplusBintoC (Matrix A, int xa, int ya, Matrix B, int xb, int yb, Matrix C, int size) {
		// rows
		for (int i = 0; i < size; i++) {    
			// cols
			for (int j = 0; j < size; j++) {
				C[i, j] = 0;
				if (xa + j < A.cols && ya + i < A.rows) C[i, j] += A[ya + i, xa + j];
				if (xb + j < B.cols && yb + i < B.rows) C[i, j] += B[yb + i, xb + j];
			}
		}
	}

	private static void SafeAminusBintoC (Matrix A, int xa, int ya, Matrix B, int xb, int yb, Matrix C, int size) {
		// rows
		for (int i = 0; i < size; i++) {         
			// cols
			for (int j = 0; j < size; j++) {   
				C[i, j] = 0;
				if (xa + j < A.cols && ya + i < A.rows) C[i, j] += A[ya + i, xa + j];
				if (xb + j < B.cols && yb + i < B.rows) C[i, j] -= B[yb + i, xb + j];
			}
		}
	}

	private static void SafeACopytoC (Matrix A, int xa, int ya, Matrix C, int size) {
		// rows
		for (int i = 0; i < size; i++) {  
			// cols      
			for (int j = 0; j < size; j++) {
				C[i, j] = 0;
				if (xa + j < A.cols && ya + i < A.rows) C[i, j] += A[ya + i, xa + j];
			}
		}
	}

	private static void AplusBintoC (Matrix A, int xa, int ya, Matrix B, int xb, int yb, Matrix C, int size) {
		for (int i = 0; i < size; i++)          // rows
			for (int j = 0; j < size; j++) C[i, j] = A[ya + i, xa + j] + B[yb + i, xb + j];
	}

	private static void AminusBintoC (Matrix A, int xa, int ya, Matrix B, int xb, int yb, Matrix C, int size) {
		for (int i = 0; i < size; i++)          // rows
			for (int j = 0; j < size; j++) C[i, j] = A[ya + i, xa + j] - B[yb + i, xb + j];
	}

	private static void ACopytoC (Matrix A, int xa, int ya, Matrix C, int size) {
		for (int i = 0; i < size; i++)          // rows
			for (int j = 0; j < size; j++) C[i, j] = A[ya + i, xa + j];
	}

	// Smart matrix multiplication
	private static Matrix StrassenMultiply (Matrix A, Matrix B) {
		if (A.cols != B.rows) throw new MException("Wrong dimension of matrix!");

		Matrix R;

		int msize = Mathf.Max(Mathf.Max(A.rows, A.cols), Mathf.Max(B.rows, B.cols));

		if (msize < 32) {
			R = ZeroMatrix(A.rows, B.cols);
			for (int i = 0; i < R.rows; i++)
				for (int j = 0; j < R.cols; j++)
					for (int k = 0; k < A.cols; k++)
						R[i, j] += A[i, k] * B[k, j];
			return R;
		}

		int size = 1; int n = 0;
		while (msize > size) { size *= 2; n++; };
		int h = size / 2;


		Matrix[,] mField = new Matrix[n, 9];

		/*
		 *  8x8, 8x8, 8x8, ...
		 *  4x4, 4x4, 4x4, ...
		 *  2x2, 2x2, 2x2, ...
		 *  . . .
		 */

		int z;
		for (int i = 0; i < n - 4; i++)          // rows
		{
			z = (int)Mathf.Pow(2, n - i - 1);
			for (int j = 0; j < 9; j++) mField[i, j] = new Matrix(z, z);
		}

		SafeAplusBintoC(A, 0, 0, A, h, h, mField[0, 0], h);
		SafeAplusBintoC(B, 0, 0, B, h, h, mField[0, 1], h);
		StrassenMultiplyRun(mField[0, 0], mField[0, 1], mField[0, 1 + 1], 1, mField); // (A11 + A22) * (B11 + B22);

		SafeAplusBintoC(A, 0, h, A, h, h, mField[0, 0], h);
		SafeACopytoC(B, 0, 0, mField[0, 1], h);
		StrassenMultiplyRun(mField[0, 0], mField[0, 1], mField[0, 1 + 2], 1, mField); // (A21 + A22) * B11;

		SafeACopytoC(A, 0, 0, mField[0, 0], h);
		SafeAminusBintoC(B, h, 0, B, h, h, mField[0, 1], h);
		StrassenMultiplyRun(mField[0, 0], mField[0, 1], mField[0, 1 + 3], 1, mField); //A11 * (B12 - B22);

		SafeACopytoC(A, h, h, mField[0, 0], h);
		SafeAminusBintoC(B, 0, h, B, 0, 0, mField[0, 1], h);
		StrassenMultiplyRun(mField[0, 0], mField[0, 1], mField[0, 1 + 4], 1, mField); //A22 * (B21 - B11);

		SafeAplusBintoC(A, 0, 0, A, h, 0, mField[0, 0], h);
		SafeACopytoC(B, h, h, mField[0, 1], h);
		StrassenMultiplyRun(mField[0, 0], mField[0, 1], mField[0, 1 + 5], 1, mField); //(A11 + A12) * B22;

		SafeAminusBintoC(A, 0, h, A, 0, 0, mField[0, 0], h);
		SafeAplusBintoC(B, 0, 0, B, h, 0, mField[0, 1], h);
		StrassenMultiplyRun(mField[0, 0], mField[0, 1], mField[0, 1 + 6], 1, mField); //(A21 - A11) * (B11 + B12);

		SafeAminusBintoC(A, h, 0, A, h, h, mField[0, 0], h);
		SafeAplusBintoC(B, 0, h, B, h, h, mField[0, 1], h);
		StrassenMultiplyRun(mField[0, 0], mField[0, 1], mField[0, 1 + 7], 1, mField); // (A12 - A22) * (B21 + B22);

		R = new Matrix(A.rows, B.cols);                  // result

		/// C11
		for (int i = 0; i < Mathf.Min(h, R.rows); i++)          // rows
			for (int j = 0; j < Mathf.Min(h, R.cols); j++)     // cols
				R[i, j] = mField[0, 1 + 1][i, j] + mField[0, 1 + 4][i, j] - mField[0, 1 + 5][i, j] + mField[0, 1 + 7][i, j];

		/// C12
		for (int i = 0; i < Mathf.Min(h, R.rows); i++)          // rows
			for (int j = h; j < Mathf.Min(2 * h, R.cols); j++)     // cols
				R[i, j] = mField[0, 1 + 3][i, j - h] + mField[0, 1 + 5][i, j - h];

		/// C21
		for (int i = h; i < Mathf.Min(2 * h, R.rows); i++)          // rows
			for (int j = 0; j < Mathf.Min(h, R.cols); j++)     // cols
				R[i, j] = mField[0, 1 + 2][i - h, j] + mField[0, 1 + 4][i - h, j];

		/// C22
		for (int i = h; i < Mathf.Min(2 * h, R.rows); i++)          // rows
			for (int j = h; j < Mathf.Min(2 * h, R.cols); j++)     // cols
				R[i, j] = mField[0, 1 + 1][i - h, j - h] - mField[0, 1 + 2][i - h, j - h] + mField[0, 1 + 3][i - h, j - h] + mField[0, 1 + 6][i - h, j - h];

		return R;
	}

	// function for square matrix 2^N x 2^N
	// A * B into C, level of recursion, matrix field
	private static void StrassenMultiplyRun (Matrix A, Matrix B, Matrix C, int l, Matrix[,] f) {
		int size = A.rows;
		int h = size / 2;

		if (size < 32) {
			for (int i = 0; i < C.rows; i++)
				for (int j = 0; j < C.cols; j++)
				{
					C[i, j] = 0;
					for (int k = 0; k < A.cols; k++) C[i, j] += A[i, k] * B[k, j];
				}
			return;
		}

		AplusBintoC(A, 0, 0, A, h, h, f[l, 0], h);
		AplusBintoC(B, 0, 0, B, h, h, f[l, 1], h);
		StrassenMultiplyRun(f[l, 0], f[l, 1], f[l, 1 + 1], l + 1, f); // (A11 + A22) * (B11 + B22);

		AplusBintoC(A, 0, h, A, h, h, f[l, 0], h);
		ACopytoC(B, 0, 0, f[l, 1], h);
		StrassenMultiplyRun(f[l, 0], f[l, 1], f[l, 1 + 2], l + 1, f); // (A21 + A22) * B11;

		ACopytoC(A, 0, 0, f[l, 0], h);
		AminusBintoC(B, h, 0, B, h, h, f[l, 1], h);
		StrassenMultiplyRun(f[l, 0], f[l, 1], f[l, 1 + 3], l + 1, f); //A11 * (B12 - B22);

		ACopytoC(A, h, h, f[l, 0], h);
		AminusBintoC(B, 0, h, B, 0, 0, f[l, 1], h);
		StrassenMultiplyRun(f[l, 0], f[l, 1], f[l, 1 + 4], l + 1, f); //A22 * (B21 - B11);

		AplusBintoC(A, 0, 0, A, h, 0, f[l, 0], h);
		ACopytoC(B, h, h, f[l, 1], h);
		StrassenMultiplyRun(f[l, 0], f[l, 1], f[l, 1 + 5], l + 1, f); //(A11 + A12) * B22;

		AminusBintoC(A, 0, h, A, 0, 0, f[l, 0], h);
		AplusBintoC(B, 0, 0, B, h, 0, f[l, 1], h);
		StrassenMultiplyRun(f[l, 0], f[l, 1], f[l, 1 + 6], l + 1, f); //(A21 - A11) * (B11 + B12);

		AminusBintoC(A, h, 0, A, h, h, f[l, 0], h);
		AplusBintoC(B, 0, h, B, h, h, f[l, 1], h);
		StrassenMultiplyRun(f[l, 0], f[l, 1], f[l, 1 + 7], l + 1, f); // (A12 - A22) * (B21 + B22);

		/// C11
		for (int i = 0; i < h; i++)          // rows
			for (int j = 0; j < h; j++)     // cols
				C[i, j] = f[l, 1 + 1][i, j] + f[l, 1 + 4][i, j] - f[l, 1 + 5][i, j] + f[l, 1 + 7][i, j];

		/// C12
		for (int i = 0; i < h; i++)          // rows
			for (int j = h; j < size; j++)     // cols
				C[i, j] = f[l, 1 + 3][i, j - h] + f[l, 1 + 5][i, j - h];

		/// C21
		for (int i = h; i < size; i++)          // rows
			for (int j = 0; j < h; j++)     // cols
				C[i, j] = f[l, 1 + 2][i - h, j] + f[l, 1 + 4][i - h, j];

		/// C22
		for (int i = h; i < size; i++)          // rows
			for (int j = h; j < size; j++)     // cols
				C[i, j] = f[l, 1 + 1][i - h, j - h] - f[l, 1 + 2][i - h, j - h] + f[l, 1 + 3][i - h, j - h] + f[l, 1 + 6][i - h, j - h];
	}

	// Stupid matrix multiplication
	public static Matrix StupidMultiply (Matrix m1, Matrix m2) {
		if (m1.cols != m2.rows) throw new MException("Wrong dimensions of matrix!");

		Matrix result = ZeroMatrix(m1.rows, m2.cols);
		for (int i = 0; i < result.rows; i++)
			for (int j = 0; j < result.cols; j++)
				for (int k = 0; k < m1.cols; k++)
					result[i, j] += m1[i, k] * m2[k, j];
		return result;
	}

	// Multiplication by constant n
	private static Matrix Multiply (float n, Matrix m) {
		Matrix r = new Matrix(m.rows, m.cols);
		for (int i = 0; i < m.rows; i++)
			for (int j = 0; j < m.cols; j++)
				r[i, j] = m[i, j] * n;
		return r;
	}

	// Sčítání matic
	private static Matrix Add (Matrix m1, Matrix m2) {
		if (m1.rows != m2.rows || m1.cols != m2.cols) throw new MException("Matrices must have the same dimensions!");
		Matrix r = new Matrix(m1.rows, m1.cols);
		for (int i = 0; i < r.rows; i++)
			for (int j = 0; j < r.cols; j++)
				r[i, j] = m1[i, j] + m2[i, j];
		return r;
	}

	// From Andy - thank you! :)
	public static string NormalizeMatrixString (string matStr) {
		// Remove any multiple spaces
		while (matStr.IndexOf("  ") != -1)
			matStr = matStr.Replace("  ", " ");

		// Remove any spaces before or after newlines
		matStr = matStr.Replace(" \r\n", "\r\n");
		matStr = matStr.Replace("\r\n ", "\r\n");

		// If the data ends in a newline, remove the trailing newline.
		// Make it easier by first replacing \r\n’s with |’s then
		// restore the |’s with \r\n’s
		matStr = matStr.Replace("\r\n", "|");
		while (matStr.LastIndexOf("|") == (matStr.Length - 1))
			matStr = matStr.Substring(0, matStr.Length - 1);

		matStr = matStr.Replace("|", "\r\n");
		return matStr;
	}

	public static Matrix operator - (Matrix m) { 
		return Matrix.Multiply(-1, m); 
	}

	public static Matrix operator + (Matrix m1, Matrix m2) { 
		return Matrix.Add(m1, m2); 
	}

	public static Matrix operator - (Matrix m1, Matrix m2) { 
		return Matrix.Add(m1, -m2); 
	}

	public static Matrix operator * (Matrix m1, Matrix m2) { 
		return Matrix.StrassenMultiply(m1, m2); 
	}

	public static Matrix operator * (float n, Matrix m) { 
		return Matrix.Multiply(n, m); 
	}
}

//  The class for exceptions
public class MException : System.Exception {
	public MException (string Message) : base(Message) { }
}
}
