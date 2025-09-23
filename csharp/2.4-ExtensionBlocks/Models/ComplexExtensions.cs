namespace ExtensionBlocks.Models;

/// <summary>
/// Extension methods for Complex numbers demonstrating C# 14 extension blocks.
/// Shows how extension blocks organize mathematical operations for better code organization.
/// 
/// Uses the actual C# 14 extension syntax that compiles and runs in .NET 10 RC 1.
/// Extension properties (like Magnitude and Phase) are accessed as true properties:
/// complex.Magnitude (not complex.Magnitude()).
/// </summary>
public static class ComplexExtensions
{
    extension (Complex complex)
    {
        /// <summary>
        /// Calculates the magnitude (absolute value) of the complex number.
        /// Extension property for complex magnitude calculation.
        /// </summary>
        public double Magnitude => Math.Sqrt(complex.Real * complex.Real + complex.Imaginary * complex.Imaginary);

        /// <summary>
        /// Calculates the phase (argument) of the complex number.
        /// Extension property for complex number phase calculation.
        /// </summary>
        public double Phase => Math.Atan2(complex.Imaginary, complex.Real);

        /// <summary>
        /// Returns the complex conjugate of the number.
        /// Extension method demonstrating complex number operations.
        /// </summary>
        public Complex Conjugate() => new(complex.Real, -complex.Imaginary);

        /// <summary>
        /// Converts the complex number to polar form.
        /// Extension method returning tuple for polar representation.
        /// </summary>
        public (double Magnitude, double Phase) ToPolar() => (complex.Magnitude, complex.Phase);

        /// <summary>
        /// Raises the complex number to a power.
        /// Extension method for complex exponentiation using De Moivre's theorem.
        /// </summary>
        public Complex Power(double exponent)
        {
            var (magnitude, phase) = complex.ToPolar();
            var newMagnitude = Math.Pow(magnitude, exponent);
            var newPhase = phase * exponent;
            return FromPolar(newMagnitude, newPhase);
        }

        /// <summary>
        /// Calculates the square root of the complex number.
        /// Extension method for complex square root calculation.
        /// </summary>
        public Complex SquareRoot()
        {
            var magnitude = complex.Magnitude;
            var phase = complex.Phase;
            var sqrtMagnitude = Math.Sqrt(magnitude);
            return FromPolar(sqrtMagnitude, phase / 2);
        }
    }

    extension (Complex) // extension receiver type only
    {
        /// <summary>
        /// Extension operator for complex number addition.
        /// Demonstrates basic arithmetic operators in extension blocks.
        /// </summary>
        public static Complex operator +(Complex left, Complex right)
            => new(left.Real + right.Real, left.Imaginary + right.Imaginary);

        /// <summary>
        /// Extension operator for complex number subtraction.
        /// Shows how multiple operators can be defined in the same extension block.
        /// </summary>
        public static Complex operator -(Complex left, Complex right)
            => new(left.Real - right.Real, left.Imaginary - right.Imaginary);

        /// <summary>
        /// Extension operator for complex number multiplication.
        /// Demonstrates more complex mathematical operations in extension operators.
        /// Formula: (a + bi)(c + di) = (ac - bd) + (ad + bc)i
        /// </summary>
        public static Complex operator *(Complex left, Complex right)
            => new(
                left.Real * right.Real - left.Imaginary * right.Imaginary,
                left.Real * right.Imaginary + left.Imaginary * right.Real
            );

        /// <summary>
        /// Extension operator for complex number division.
        /// Shows advanced mathematical operations with proper error handling.
        /// Formula: (a + bi)/(c + di) = [(ac + bd) + (bc - ad)i] / (c² + d²)
        /// </summary>
        public static Complex operator /(Complex left, Complex right)
        {
            var denominator = right.Real * right.Real + right.Imaginary * right.Imaginary;

            if (Math.Abs(denominator) < double.Epsilon)
                throw new DivideByZeroException("Cannot divide by zero complex number");

            return new(
                (left.Real * right.Real + left.Imaginary * right.Imaginary) / denominator,
                (left.Imaginary * right.Real - left.Real * right.Imaginary) / denominator
            );
        }

        /// <summary>
        /// Extension operator for scalar multiplication.
        /// Demonstrates operators with different parameter types.
        /// </summary>
        public static Complex operator *(Complex complex, double scalar)
            => new(complex.Real * scalar, complex.Imaginary * scalar);

        /// <summary>
        /// Extension operator for scalar multiplication (commutative).
        /// Shows how to make operations commutative with multiple operator overloads.
        /// </summary>
        public static Complex operator *(double scalar, Complex complex)
            => complex * scalar;

        /// <summary>
        /// Extension operator for unary negation.
        /// Demonstrates unary operators in extension blocks.
        /// </summary>
        public static Complex operator -(Complex complex)
            => new(-complex.Real, -complex.Imaginary);
    }

    /// <summary>
    /// Creates a complex number from polar coordinates.
    /// Static method demonstrating polar to rectangular conversion.
    /// </summary>
    public static Complex FromPolar(double magnitude, double phase)
        => new(magnitude * Math.Cos(phase), magnitude * Math.Sin(phase));
}