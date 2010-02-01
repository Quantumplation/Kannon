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
        class ReverseCompararer : IComparer<Int32>
        {
            public int Compare(int x, int y)
            {
                return y.CompareTo(x);
            }
        }

        private SpriteBatch m_SpriteBatch;
        SortedDictionary<Int32, List<Components.IRenderableComponent>> m_Components;
        ITransformer[] m_Transformers;

        public Graphics(GraphicsDevice gd)
        {
            m_Components = new SortedDictionary<int, List<Components.IRenderableComponent>>(new ReverseCompararer());
            m_Transformers = new ITransformer[100];

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
            }
            m_Components[r.Layer].Add(r);
        }

        public void RemoveComponent(Component c)
        {
            m_Components.Remove((c as Components.IRenderableComponent).Layer);
        }

        public void SetTransformer(ITransformer trans, Int32 layerStart, Int32? layerEnd = null )
        {
            if (layerEnd > 100 || layerStart < 0)
                throw new Exception("Layer must be between 0 and 100");
            if (layerEnd == null)
                layerEnd = layerStart;
            for (int x = layerStart; x < layerEnd; x++)
            {
                m_Transformers[x] = trans;
            }
        }

        public void ChangeLayer(Components.IRenderableComponent comp, Int32 oldLayer, Int32 newLayer)
        {
            if (m_Components.ContainsKey(oldLayer))
                if (m_Components[oldLayer].Contains(comp))
                {
                    m_Components[oldLayer].Remove(comp);
                    if (!m_Components.ContainsKey(newLayer))
                    {
                        m_Components.Add(newLayer, new List<Components.IRenderableComponent>());
                    }
                    m_Components[newLayer].Add(comp);
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
            foreach (var x in m_Components)
            {
                Int32 layer = x.Key;
                if (m_Components[layer].Count > 0)
                {
                    m_SpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred, SaveStateMode.None, (m_Transformers[layer] ?? IDTransformer.Identity).GetTransformation(layer)); ;

                    foreach (Components.IRenderableComponent renderable in x.Value)
                        renderable.Render(m_SpriteBatch);

                    m_SpriteBatch.End();
                }
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
