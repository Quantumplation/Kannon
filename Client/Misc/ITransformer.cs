using System;
using Microsoft.Xna.Framework;

namespace Kannon
{
    public interface ITransformer
    {
        Matrix GetTransformation(Int32 Layer);
    }

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

        public Matrix GetTransformation(Int32 Layer)
        {
            return Matrix.Identity;
        }
    }
}