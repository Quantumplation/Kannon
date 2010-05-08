using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Kannon.Components
{
    /// <summary>
    /// Something which can be rendered.
    /// </summary>
    public interface IRenderable
    {
        /// <summary>
        /// Render this, using the spritebatch sb, on the pass PassID
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="PassID">
        /// Defaults to "Unsorted". This allows for custom behavior depending on 
        /// which pass is being rendered, if an object is on multiple passes.</param>
        void Render(SpriteBatch sb, string PassID = "Unsorted");
    }

    /// <summary>
    /// Something which has content which needs to be loaded sometime early on.
    /// </summary>
    public interface IContent
    {
        // Load the content, using Content Manager.
        void Load(ContentManager cm);
    }
}
