using Microsoft.Z3;

namespace AdventOfCode2023
{
    partial class Day24
    {


        private static long UseZ3(List<SingleHail> hail)
        {
            // too tired for now. will use Z3. but this is not what I wanted to do
            // cloned the code, just to see if it works.
            // this is not my code, my solution bellow actually worked in the end
            var ctx = new Context();
            var solver = ctx.MkSolver();

            // Coordinates of the stone
            var x = ctx.MkIntConst("x");
            var y = ctx.MkIntConst("y");
            var z = ctx.MkIntConst("z");

            // Velocity of the stone
            var vx = ctx.MkIntConst("vx");
            var vy = ctx.MkIntConst("vy");
            var vz = ctx.MkIntConst("vz");

            for (var i = 0; i < 3; i++)
            {
                var t = ctx.MkIntConst($"t{i}"); // time for the stone to reach the hail
                var h = hail[i];

                var px = ctx.MkInt(Convert.ToInt64(h.Position.x));
                var py = ctx.MkInt(Convert.ToInt64(h.Position.y));
                var pz = ctx.MkInt(Convert.ToInt64(h.Position.z));

                var pvx = ctx.MkInt(Convert.ToInt64(h.Velocity.x));
                var pvy = ctx.MkInt(Convert.ToInt64(h.Velocity.y));
                var pvz = ctx.MkInt(Convert.ToInt64(h.Velocity.z));

                var xLeft = ctx.MkAdd(x, ctx.MkMul(t, vx)); // x + t * vx
                var yLeft = ctx.MkAdd(y, ctx.MkMul(t, vy)); // y + t * vy
                var zLeft = ctx.MkAdd(z, ctx.MkMul(t, vz)); // z + t * vz

                var xRight = ctx.MkAdd(px, ctx.MkMul(t, pvx)); // px + t * pvx
                var yRight = ctx.MkAdd(py, ctx.MkMul(t, pvy)); // py + t * pvy
                var zRight = ctx.MkAdd(pz, ctx.MkMul(t, pvz)); // pz + t * pvz

                solver.Add(t >= 0); // time should always be positive - we don't want solutions for negative time
                solver.Add(ctx.MkEq(xLeft, xRight)); // x + t * vx = px + t * pvx
                solver.Add(ctx.MkEq(yLeft, yRight)); // y + t * vy = py + t * pvy
                solver.Add(ctx.MkEq(zLeft, zRight)); // z + t * vz = pz + t * pvz
            }

            solver.Check();
            var model = solver.Model;

            var rx = Convert.ToInt64(model.Eval(x).ToString());
            var ry = Convert.ToInt64(model.Eval(y).ToString());
            var rz = Convert.ToInt64(model.Eval(z).ToString());

            return rx + ry + rz;
        }

        //[RemoveSpacesFromInput]
        //[RemoveNewLinesFromInput]
        // change to string or string[] to get other types of input
        public static long Part2(string[] input)
        {
            return 0;

            var hail = ReadHail(input);


            // I'll try that as well, to see if it works
            // but I may look for another solution. 
            // This one is based on the observation that
            // it is enough to solve for first 3 segments. 
            // This observation was not done by me, but it may prove useful.

            // unfortunately, I have to use 6x6 matrix, and vector6.
            // will have to write my own.
            // I'm too sick and tired today for this... :(
            // maybe I'll just copy some matrix code from SO


            // everyone seem to use Z3 library.
            // lets see how it does
            //return UseZ3(hail);

            // use matrix linear solve equation solution
            // https://www.mathsisfun.com/algebra/systems-linear-equations-matrices.html

            // we can use it find a vector this is perpendicular to 
            // all hail pos x vel vectors.
            // this vector will be our answer
            // because of above observation that 3 vectors are enough to solve the problem,
            // we can use 6x6 matrix of cross products with vector 6 of crossed cross products, to solve for our final vector
            // if we needed more, we would just increase the size of our matrix and vectors, and
            // create cross products accordingly, but at some point, this will become problematic
            // to both write and compute (big matrix inverse is quite slow if not using optimization and SIMD)
            // but it's more of writing problem, I'd need some way to write 

            var h1 = hail[0];
            var h2 = hail[1];
            var h3 = hail[2];

            // make our names smaller and more manageable
            var h1p = h1.Position;
            var h1v = h1.Velocity;
            var h2p = h2.Position;
            var h2v = h2.Velocity;
            var h3p = h3.Position;
            var h3v = h3.Velocity;

            // construct a matrix that will hold cross products for our
            // vectors that we now have
            // this is THE hard part, how not to mess up the order :)
            // and we need to put h1 × h2 and h3 × h1
            // but computationally speaking, this is super-simple,
            // we just put cross products of what we now have (pos/vels)
            // into the matrix
            // Look here https://en.wikipedia.org/wiki/Cross_product for matrix notation of cross product

            Matrix mat = new(6, 6);

            // h1 vel, h2 vel
            mat[0, 0] = 0;
            mat[0, 1] = -h1v.z + h2v.z;
            mat[0, 2] = h1v.y - h2v.y;

            mat[1, 0] = h1v.z - h2v.z;
            mat[1, 1] = 0;
            mat[1, 2] = -h1v.x + h2v.x;

            mat[2, 0] = -h1v.y + h2v.y;
            mat[2, 1] = h1v.x - h2v.x;
            mat[2, 2] = 0;

            // h1 vel, h3 vel
            mat[3, 0] = 0;
            mat[3, 1] = -h1v.z + h3v.z;
            mat[3, 2] = h1v.y - h3v.y;

            mat[4, 0] = h1v.z - h3v.z;
            mat[4, 1] = 0;
            mat[4, 2] = -h1v.x + h3v.x;

            mat[5, 0] = -h1v.y + h3v.y;
            mat[5, 1] = h1v.x - h3v.x;
            mat[5, 2] = 0;

            // h2 pos, h1 pos
            mat[0, 3] = 0;
            mat[0, 4] = -h2p.z + h1p.z;
            mat[0, 5] = h2p.y - h1p.y;

            mat[1, 3] = h2p.z - h1p.z;
            mat[1, 4] = 0;
            mat[1, 5] = -h2p.x + h1p.x;

            mat[2, 3] = -h2p.y + h1p.y;
            mat[2, 4] = h2p.x - h1p.x;
            mat[2, 5] = 0;

            // h3 pos, h1 pos
            mat[3, 3] = 0;
            mat[3, 4] = -h3p.z + h1p.z;
            mat[3, 5] = h3p.y - h1p.y;

            mat[4, 3] = h3p.z - h1p.z;
            mat[4, 4] = 0;
            mat[4, 5] = -h3p.x + h1p.x;

            mat[5, 3] = -h3p.y + h1p.y;
            mat[5, 4] = h3p.x - h1p.x;
            mat[5, 5] = 0;


            // this is simple
            // first, create vectors perpendicular to our hails
            var h1pvc = Vector3.Cross(h1.Position, h1.Velocity);
            var h2pvc = Vector3.Cross(h2.Position, h2.Velocity);
            var h3pvc = Vector3.Cross(h3.Position, h3.Velocity);

            // we need perpendicular vector to our positions and velocities.
            // note that we get two vectors, Per(h2) - Per(h1) and Per(h3) - Per(h1)

            // this will give us two vectors that are both perpendicular
            // to h1 and h2
            var v1 = h2pvc - h1pvc;
            // and to h1 and h3
            var v2 = h3pvc - h1pvc;

            // we look for these vectors
            var solutionVector = new double[6] { v1.x, v1.y, v1.z, v2.x, v2.y, v2.z, };

            // solve the equation (as explained above)
            var matInv = mat.Inverse();
            var mul = matInv * solutionVector;

            // and that's it, this is our answer
            var result2 = (long)(mul[0] + mul[1] + mul[2]);

            //Debug.Assert(result2 == 880547248556435);

            return result2;
        }

    }



    // This class is generated by Bing Chat.
    // sorry, I am too lazy today.
    // Code for matrix and matrix multiplication, inverse and such is always the same.
    // But i think it's still better than using Z3.. I think..
    class Matrix
    {
        // A matrix is represented as a 2D array of doubles
        private double[,] data;
        private int rows;
        private int cols;

        // Constructor that takes the number of rows and columns
        public Matrix(int rows, int cols)
        {
            this.rows = rows;
            this.cols = cols;
            this.data = new double[rows, cols];
        }

        // Get and set the element at the given row and column
        public double this[int row, int col]
        {
            get { return data[row, col]; }
            set { data[row, col] = value; }
        }

        // Get the number of rows
        public int Rows
        {
            get { return rows; }
        }

        // Get the number of columns
        public int Cols
        {
            get { return cols; }
        }

        // Compute the inverse of the matrix using Gaussian elimination
        // Adapted from [1](https://stackoverflow.com/questions/46836908/how-to-invert-double-in-c-sharp)
        public Matrix Inverse()
        {
            // Check if the matrix is square
            if (rows != cols)
            {
                throw new Exception("Matrix must be square to have an inverse");
            }

            // Create an augmented matrix that consists of the original matrix and the identity matrix
            Matrix aug = new Matrix(rows, cols * 2);
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    aug[i, j] = data[i, j]; // Copy the original matrix
                    aug[i, j + cols] = (i == j) ? 1 : 0; // Append the identity matrix
                }
            }

            // Perform row operations to reduce the matrix to row echelon form
            for (int i = 0; i < rows; i++)
            {
                // Find the pivot row with the largest value in the current column
                int pivot = i;
                for (int j = i + 1; j < rows; j++)
                {
                    if (Math.Abs(aug[j, i]) > Math.Abs(aug[pivot, i]))
                    {
                        pivot = j;
                    }
                }

                // Swap the pivot row with the current row
                if (pivot != i)
                {
                    for (int j = i; j < cols * 2; j++)
                    {
                        double temp = aug[i, j];
                        aug[i, j] = aug[pivot, j];
                        aug[pivot, j] = temp;
                    }
                }

                // Check if the matrix is singular (non-invertible)
                if (aug[i, i] == 0)
                {
                    return aug;
                    //throw new Exception("Matrix is singular and cannot be inverted");
                }

                // Divide the current row by the pivot element
                double div = aug[i, i];
                for (int j = i; j < cols * 2; j++)
                {
                    aug[i, j] /= div;
                }

                // Subtract multiples of the current row from the other rows
                for (int j = 0; j < rows; j++)
                {
                    if (j != i)
                    {
                        double sub = aug[j, i];
                        for (int k = i; k < cols * 2; k++)
                        {
                            aug[j, k] -= sub * aug[i, k];
                        }
                    }
                }
            }

            // Extract the right half of the augmented matrix as the inverse matrix
            Matrix inv = new Matrix(rows, cols);
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    inv[i, j] = aug[i, j + cols];
                }
            }
            return inv;
        }

        // Define the multiplication by vector operator for the matrix class
        public static double[] operator *(Matrix m, double[] v)
        {
            // Create a new vector with the same number of rows as the matrix
            var prod = new double[m.Rows];

            // Loop through the rows of the matrix and the elements of the vector and multiply them
            for (int i = 0; i < m.Rows; i++)
            {
                double temp = 0;
                for (int j = 0; j < m.Cols; j++)
                {
                    temp += m[i, j] * v[j];
                }


                prod[i] = temp;
            }

            // Return the product vector
            return prod;
        }
    }

}