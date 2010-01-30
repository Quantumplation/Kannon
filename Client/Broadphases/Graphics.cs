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
        private SpriteBatch m_SpriteBatch;
        SortedDictionary<Int32, List<Components.IRenderableComponent>> m_Components;
        Dictionary<Int32, ITransformer> m_Transformers;

        public Graphics(GraphicsDevice gd)
        {
            m_Components = new SortedDictionary<int, List<Components.IRenderableComponent>>();
            m_Transformers = new Dictionary<int, ITransformer>();

            ComponentFactory.RegisterCreatedCallback<Components.IRenderableComponent>(this.RegisterComponent);
            m_SpriteBatch = new SpriteBatch(gd);

            XNAGame.Instance.RenderEvent += Do;
        }

        public void RegisterComponent(Component c)
        {
            Components.IRenderableComponent r = c as Components.IRenderableComponent;
            if (!m_Components.ContainsKey(r.Layer))
            {
                m_Components.Add(r.Layer, new List<Components.IRenderableComponent>());
                m_Transformers.Add(r.Layer, IDTransformer.Identity);
            }
            m_Components[r.Layer].Add(r);
        }

        public void RemoveComponent(Component c)
        {
            m_Components.Remove((c as Components.IRenderableComponent).Layer);
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
            foreach (Int32 layer in m_Components.Keys)
            {
                m_SpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred, SaveStateMode.None, m_Transformers[layer].GetTransformation(layer)); ;

                foreach (Components.IRenderableComponent renderable in m_Components[layer])
                    renderable.Render(m_SpriteBatch);

                m_SpriteBatch.End();
            }
        }


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
