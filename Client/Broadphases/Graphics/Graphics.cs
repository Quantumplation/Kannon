using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Kannon.Broadphases
{
    /// <summary>
    /// Provides a render heartbeat to IRenderable components.
    /// </summary>
    public class Graphics :IBroadphase
    {
        SpriteBatch m_SpriteBatch;
        GraphicsDevice m_GraphicsDevice;
        Dictionary<String, RenderPass> m_RenderPasses;

        public Graphics(GraphicsDevice gd)
        {
            ComponentFactory.RegisterCreatedCallback<Components.IRenderable>(this.RegisterComponent);
            m_SpriteBatch = new SpriteBatch(gd);
            m_GraphicsDevice = gd;
            RasterizerState rs = new RasterizerState();
            rs.CullMode = CullMode.None;
            m_GraphicsDevice.RasterizerState = rs;

            m_RenderPasses = new Dictionary<String,RenderPass>();
            m_RenderPasses.Add("Unsorted", new RenderPass());

            XNAGame.Instance.RenderEvent += Do;
        }

        /// <summary>
        /// Register a renderable component.  For now, put it in the "Unsorted" layer.
        /// </summary>
        /// <param name="c"></param>
        public void RegisterComponent(Component c)
        {
            m_RenderPasses["Unsorted"].RegisterComponent(c as Components.IRenderable);
        }

        /// <summary>
        /// Add a component to a pass.  This takes it out of the unsorted layer, and puts it in whatever you specify.
        /// </summary>
        /// <param name="r"></param>
        /// <param name="PassIDs"></param>
        public void AddComponentToPass(Components.IRenderable r, params String[] PassIDs)
        {
            m_RenderPasses["Unsorted"].RemoveComponent(r);

            foreach(String passID in PassIDs)
            {
                if( !m_RenderPasses.ContainsKey(passID) )
                {
                    // Err: Pass doesn't exist, creating default pass.
                    m_RenderPasses.Add(passID, new RenderPass());
                }
                m_RenderPasses[passID].RegisterComponent(r);
            }
        }

        /// <summary>
        /// Removes a component from all passes.
        /// </summary>
        /// <param name="c"></param>
        public void RemoveComponent(Component c)
        {
            foreach (RenderPass rp in m_RenderPasses.Values)
            {
                rp.RemoveComponent(c as Components.IRenderable);
            }
        }

        /// <summary>
        /// Removes a component from the specified pass.
        /// </summary>
        /// <param name="r"></param>
        /// <param name="PassIDs"></param>
        public void RemoveComponentFromPass(Components.IRenderable r, params String[] PassIDs)
        {
            foreach (String passID in PassIDs)
            {
                if (m_RenderPasses.ContainsKey(passID))
                    m_RenderPasses[passID].RemoveComponent(r);
            }
        }

        /// <summary>
        /// Set the transformer trans for the specified pass ID's.
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="PassIDs"></param>
        public void SetTransformer(ITransformer trans, params String[] PassIDs)
        {
            foreach (String passID in PassIDs)
            {
                if (m_RenderPasses.ContainsKey(passID))
                    m_RenderPasses[passID].SetTransformer(trans);
            }
        }

        /// <summary>
        /// Retrieve the transformer for the specified pass.
        /// </summary>
        /// <param name="PassID"></param>
        /// <returns></returns>
        public ITransformer GetTransformer(String PassID = "Unsorted")
        {
            return m_RenderPasses[PassID].GetTransformer();
        }

        /// <summary>
        /// Makes sure we aren't rendering too fast.
        /// </summary>
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
            foreach (RenderPass r in m_RenderPasses.Values)
                r.Render(m_SpriteBatch);
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
