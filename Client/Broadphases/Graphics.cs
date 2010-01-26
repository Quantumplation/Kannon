using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Kannon.Broadphases
{
    /// <summary>
    /// Provides a render heartbeat to IRenderable components.
    /// </summary>
    public class Graphics :IBroadphase
    {
        private ContentManager m_ContentManager;
        private SpriteBatch m_SpriteBatch;

        public Graphics(ContentManager cm, GraphicsDevice gd)
        {
            m_Components = new List<Kannon.Components.IRenderableComponent>();

            ComponentFactory.RegisterCreatedCallback<Components.IRenderableComponent>(this.RegisterComponent);
            m_ContentManager = cm;
            m_SpriteBatch = new SpriteBatch(gd);

            XNAGame.Instance.LoadEvent += Load;
            XNAGame.Instance.RenderEvent += Do;
        }

        public void RegisterComponent(Component c)
        {
            if (Loaded)
                (c as Components.IRenderableComponent).Load(m_ContentManager, m_SpriteBatch);
            m_Components.Add(c as Components.IRenderableComponent);
        }

        public void RemoveComponent(Component c)
        {
            m_Components.Remove(c as Components.IRenderableComponent);
        }

        private bool Loaded = false;
        public void Load()
        {
            Loaded = true;
            foreach (Components.IRenderableComponent c in m_Components)
            {
                c.Load(m_ContentManager, m_SpriteBatch);
            }
        }

        private float internalTimer;
        public void Do(float elapsedTime)
        {
            internalTimer += elapsedTime;
            if (elapsedTime > ExecutionFrequency)
            {
                if (elapsedTime > (ExecutionFrequency * 2))
                    ExecutingTooSlowly = true;
                else
                    ExecutingTooSlowly = false;
                internalTimer = 0;
                Render();
            }
        }

        protected void Render()
        {
            m_SpriteBatch.Begin();
            foreach (Components.IRenderableComponent renderable in m_Components)
                renderable.Render();
            m_SpriteBatch.End();
        }

        List<Components.IRenderableComponent> m_Components;

        public float ExecutionFrequency
        {
            get;
            set;
        }

        public bool ExecutingTooSlowly
        {
            get;
            protected set;
        }
    }
}
