using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Kannon.Components
{
    public interface IRenderableComponent
    {
        Int32 Layer
        {
            get;
            set;
        }
        void Render(SpriteBatch sb, int Layer);
    }

    public interface IContentComponent
    {
        void Load(ContentManager cm);
    }
}
