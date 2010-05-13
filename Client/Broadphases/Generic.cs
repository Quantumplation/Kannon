using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Kannon.EntitySystem.Components;
using Kannon.EntitySystem;

namespace Kannon.Broadphases
{
    /// <summary>
    /// Provides an Update(gameTime) heartbeat to components that implement IGeneric.
    /// </summary>
    public class Generic : IBroadphase
    {
        List<IGenericComponent> m_Components;

        public Generic()
        {
            ComponentFactory.RegisterCreatedCallback<IGenericComponent>(this.RegisterComponent);
            XNAGame.Instance.UpdateEvent += Do;
            m_Components = new List<IGenericComponent>();
        }

        public void RegisterComponent(Component c)
        {
            m_Components.Add(c as IGenericComponent);
            c.Entity.AddEvent(c.Name + ".Remove", new EntityEvent(this.RemoveComponent));
        }

        public void RemoveComponent(Object c)
        {
            RemoveComponent(c as Component);
        }

        public void RemoveComponent(Component c)
        {
            m_Components.Remove(c as IGenericComponent);
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
            foreach (IGenericComponent generic in m_Components)
                generic.Update(elapsedTime);
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
