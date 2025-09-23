namespace ExtensionBlocks.Models;

/// <summary>
/// Extension methods for Vector3D demonstrating mathematical operations.
/// In C# 14, these would be organized in extension blocks for better code organization.
/// 
/// ACTUAL C# 14 SYNTAX (when available):
/// These static methods would remain as static utility methods since they operate
/// on two Vector3D instances rather than extending a single instance.
/// 
/// Current implementation uses traditional static methods:
/// </summary>
public static class Vector3DMathOperators
{
    extension(Vector3D) // extension receiver type only
    {
        /// <summary>
        /// Extension operator for vector addition.
        /// Basic vector arithmetic demonstrating component-wise operations.
        /// </summary>
        public static Vector3D operator +(Vector3D left, Vector3D right)
            => new(left.X + right.X, left.Y + right.Y, left.Z + right.Z);

        /// <summary>
        /// Extension operator for vector subtraction.
        /// Shows component-wise subtraction in extension operators.
        /// </summary>
        public static Vector3D operator -(Vector3D left, Vector3D right)
            => new(left.X - right.X, left.Y - right.Y, left.Z - right.Z);

        /// <summary>
        /// Extension operator for scalar multiplication.
        /// Demonstrates scaling vectors with scalar values.
        /// </summary>
        public static Vector3D operator *(Vector3D vector, double scalar)
            => new(vector.X * scalar, vector.Y * scalar, vector.Z * scalar);

        /// <summary>
        /// Extension operator for scalar multiplication (commutative).
        /// Shows commutative scalar multiplication.
        /// </summary>
        public static Vector3D operator *(double scalar, Vector3D vector)
            => vector * scalar;

        /// <summary>
        /// Extension operator for dot product.
        /// Demonstrates custom mathematical operations for vectors.
        /// </summary>
        public static double operator *(Vector3D left, Vector3D right)
            => left.X * right.X + left.Y * right.Y + left.Z * right.Z;

        /// <summary>
        /// Extension operator for cross product.
        /// Shows complex vector operations in extension methods.
        /// </summary>
        public static Vector3D operator ^(Vector3D left, Vector3D right)
            => new(
                left.Y * right.Z - left.Z * right.Y,
                left.Z * right.X - left.X * right.Z,
                left.X * right.Y - left.Y * right.X
            );

        /// <summary>
        /// Extension operator for scalar division.
        /// Demonstrates vector scaling with division and error handling.
        /// </summary>
        public static Vector3D operator /(Vector3D vector, double scalar)
        {
            if (Math.Abs(scalar) < double.Epsilon)
                throw new DivideByZeroException("Cannot divide vector by zero");

            return new(vector.X / scalar, vector.Y / scalar, vector.Z / scalar);
        }

        /// <summary>
        /// Extension operator for unary negation.
        /// Shows unary operators for vector direction reversal.
        /// </summary>
        public static Vector3D operator -(Vector3D vector)
            => new(-vector.X, -vector.Y, -vector.Z);

    }
}
