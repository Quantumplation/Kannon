using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace Kannon.Broadphases
{
    /// <summary>
    /// Provides an Update(gameTime) heartbeat to components that implement IGeneric.
    /// </summary>
    public class Generic : IBroadphase
    {
        public Generic()
        {
            ComponentFactory.RegisterCreatedCallback<Components.IGenericComponent>(this.RegisterComponent);
        }

        public void RegisterComponent(Component c)
        {
            m_Components.Add(c as Components.IGenericComponent);
            c.Entity.AddEvent(c.Name + ".Remove", new EntityEvent(this.RemoveComponent));
        }

        public void RemoveComponent(Object c)
        {
            RemoveComponent(c as Component);
        }

        public void RemoveComponent(Component c)
        {
            m_Components.Remove(c as Components.IGenericComponent);
        }

        public void Initialize()
        {
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
                Update(elapsedTime);
            }
        }

        protected void Update(float elapsedTime)
        {
            foreach (Components.IGenericComponent generic in m_Components)
                generic.Update(elapsedTime);
        }

        List<Components.IGenericComponent> m_Components;

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
