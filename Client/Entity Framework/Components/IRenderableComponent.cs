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
        void Load(ContentManager cm, SpriteBatch sb);
        void Render();
    }
}
