using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Kannon.Broadphases
{
    /// <summary>
    /// Represents a single sweep of rendering with a specific transform.
    /// Concretely, it's a single SpriteBatch begin/end pair.
    /// </summary>
    public class RenderPass
    {
        List<Components.IRenderable> m_Renderables;
        ITransformer m_Transform;

        public String PassID
        {
            get;
            set;
        }

        public RenderPass()
        {
            m_Renderables = new List<Components.IRenderable>();
            m_Transform = IDTransformer.Identity;
        }

        public void RegisterComponent(Components.IRenderable r)
        {
            m_Renderables.Add(r);
        }

        public void RemoveComponent(Components.IRenderable r)
        {
            if (m_Renderables.Contains(r))
                m_Renderables.Remove(r);
        }

        public void SetTransformer(ITransformer trans)
        {
            m_Transform = trans;
        }

        public ITransformer GetTransformer()
        {
            return m_Transform;
        }

        public void Render(SpriteBatch sb)
        {
            sb.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None, m_Transform.GetTransformation());
            sb.GraphicsDevice.RenderState.DepthBufferEnable = true;
            foreach (Components.IRenderable rend in m_Renderables)
            {
                rend.Render(sb);
            }
            sb.End();
        }
    }
}
