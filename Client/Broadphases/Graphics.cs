using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kannon.Broadphases
{
    public class Graphics :IBroadphase
    {
        public Graphics()
        {
            ComponentFactory.RegisterCreatedCallback<Components.IRenderableComponent>(this.RegisterComponent);
        }

        public void RegisterComponent(Component c)
        {
            m_Components.Add(c as Components.IRenderableComponent);
        }

        public void RemoveComponent(Component c)
        {
            m_Components.Remove(c as Components.IRenderableComponent);
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
            foreach (Components.IRenderableComponent renderable in m_Components)
                renderable.Render();
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
