using System;
using Microsoft.Xna.Framework;

namespace Kannon
{
    /// <summary>
    /// Represents something which provides a matrix for transforming other things.
    /// </summary>
    public interface ITransformer
    {
        Matrix GetTransformation();
    }

    /// <summary>
    /// Represents an Identity Matrix Transformation in ITransformer format.
    /// Perhaps, instead, provide an extension method for ITransformer? or something.
    /// </summary>
    public class IDTransformer : ITransformer
    {
        static IDTransformer()
        {
            Identity = new IDTransformer();
        }

        public static IDTransformer Identity
        {
            get;
            protected set;
        }

        public Matrix GetTransformation()
        {
            return Matrix.Identity;
        }
    }
}