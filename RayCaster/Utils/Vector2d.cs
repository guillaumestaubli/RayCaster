using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayCaster.Utils
{
    /// <summary>
    /// Vector2d is an utility class for manipulating 2 dimensional vectors with double components
    /// </summary>
    public struct Vector2d : IEquatable<Vector2d>
    { 
        /// <summary>
        /// X (horizontal) component of the vector
        /// </summary>
        public double X;
 
        /// <summary>
        /// Y (vertical) component of the vector
        /// </summary>
        public double Y;

        /// <summary>
        /// Construct the vector from its coordinates
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        public Vector2d(double x, double y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Operator - overload ; returns the opposite of a vector
        /// </summary>
        /// <param name="v">Vector to negate</param>
        /// <returns>-v</returns>
        public static Vector2d operator -(Vector2d v)
        {
            return new Vector2d(0 - v.X, 0 - v.Y);
        }

        /// <summary>
        /// Operator - overload ; subtracts two vectors
        /// </summary>
        /// <param name="v1">First vector</param>
        /// <param name="v2">Second vector</param>
        /// <returns>v1 - v2</returns>
        public static Vector2d operator -(Vector2d v1, Vector2d v2)
        {
            return new Vector2d(v1.X - v2.X, v1.Y - v2.Y);
        }

        /// <summary>
        /// Operator + overload ; add two vectors
        /// </summary>
        /// <param name="v1">First vector</param>
        /// <param name="v2">Second vector</param>
        /// <returns>v1 + v2</returns>
        public static Vector2d operator +(Vector2d v1, Vector2d v2)
        {
            return new Vector2d(v1.X + v2.X, v1.Y + v2.Y);
        }

        /// <summary>
        /// Operator * overload ; multiply a vector by a scalar value
        /// </summary>
        /// <param name="v">Vector</param>
        /// <param name="x">Scalar value</param>
        /// <returns>v * x</returns>
        public static Vector2d operator *(Vector2d v, double x)
        {
            return new Vector2d(v.X * x, v.Y * x);
        }

        /// <summary>
        /// Operator * overload ; multiply a scalar value by a vector
        /// </summary>
        /// <param name="x">Scalar value</param>
        /// <param name="v">Vector</param>
        /// <returns>x * v</returns>
        public static Vector2d operator *(double x, Vector2d v)
        {
            return new Vector2d(v.X * x, v.Y * x);
        }

        /// <summary>
        /// Operator / overload ; divide a vector by a scalar value
        /// </summary>
        /// <param name="v">Vector</param>
        /// <param name="x">Scalar value</param>
        /// <returns>v / x</returns>
        public static Vector2d operator /(Vector2d v, double x)
        {
            return new Vector2d(v.X / x, v.Y / x);
        }

        /// <summary>
        /// Operator == overload ; check vector equality
        /// </summary>
        /// <param name="v1">First vector</param>
        /// <param name="v2">Second vector</param>
        /// <returns></returns>
        public static bool operator ==(Vector2d v1, Vector2d v2)
        {
            return v1.Equals(v2);
        }

        /// <summary>
        /// Operator != overload ; check vector inequality
        /// </summary>
        /// <param name="v1">First vector</param>
        /// <param name="v2">Second vector</param>
        /// <returns>v1 != v2</returns>
        public static bool operator !=(Vector2d v1, Vector2d v2)
        {
            return !v1.Equals(v2);
        }

        /// <summary>
        /// Provide a string describing the object
        /// </summary>
        /// <returns>String description of the object</returns>
        public override string ToString()
        {
            return $"[Vector2d] X({X}) Y({Y})";
        }

        /// <summary>
        /// Compare vector and object and checks if they are equal
        /// </summary>
        /// <param name="obj">Object to check</param>
        /// <returns>Object and vector are equal</returns>
        public override bool Equals(object obj)
        {
            if (obj is Vector2d)
            {
                return Equals((Vector2d)obj);
            }

            return false;
        }

        /// <summary>
        /// Compare two vectors and checks if they are equal
        /// </summary>
        /// <param name="other">Vector to check</param>
        /// <returns>Vectors are equal</returns>
        public bool Equals(Vector2d other)
        {
            if (X == other.X)
            {
                return Y == other.Y;
            }

            return false;
        }

        /// <summary>
        /// Provide a integer describing the object
        /// </summary>
        /// <returns>Integer description of the object</returns>
        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }

        /// <summary>
        /// Explicit casting to another vector type
        /// </summary>
        /// <param name="v">Vector being casted</param>
        public static explicit operator Vector2i(Vector2d v)
        {
            return new Vector2i((int)v.X, (int)v.Y);
        }

        /// <summary>
        /// Explicit casting to another vector type
        /// </summary>
        /// <param name="v">Vector being casted</param>
        public static explicit operator Vector2u(Vector2d v)
        {
            return new Vector2u((uint)v.X, (uint)v.Y);
        }

        /// <summary>
        /// Explicit casting to another vector type
        /// </summary>
        /// <param name="v">Vector being casted</param>
        public static explicit operator Vector2f(Vector2d v)
        {
            return new Vector2f((float)v.X, (float)v.Y);
        }
    }
}
