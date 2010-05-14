using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Kannon.Components;
using Microsoft.Xna.Framework;
using Kannon.EntitySystem;

namespace Kannon.Broadphases
{
    /// <summary>
    /// Selection broadphase.
    /// Very much a work in progress, will need heavy work when camera gets rewritten.
    /// </summary>
    class Selection : IBroadphase
    {
        SortedList<Int32, List<Selectable>> m_Components;
        List<Selectable> m_Selected;

        SelectionBox m_SelectionBox;

        Input m_InputObj;

        public Selection()
        {
            m_Components = new SortedList<Int32, List<Selectable>>();
            m_Selected = new List<Selectable>();

            m_InputObj = XNAGame.Instance.GetBroadphase<Input>("Input");
            m_InputObj.ButtonClicked += new MouseEventHandler(ButtonClicked);

            ComponentFactory.RegisterCreatedCallback<Selectable>(RegisterComponent);
            ComponentFactory.RegisterCreatedCallback<SelectionBox>(RegisterComponent);

            XNAGame.Instance.UpdateEvent += Do;
        }

        void ButtonClicked(Input.MouseData data)
        {
            if (data.mouseButtons[0])
            {
                foreach (Selectable sel in m_Selected)
                {
                    sel.Entity.InvokeEvent("Deselect", null);
                }
                m_Selected.Clear();

                Rectangle r = new Rectangle((int)data.mouseX, (int)data.mouseY, 1, 1);
                foreach (Int32 key in m_Components.Keys)
                {
                    foreach (Selectable sel in m_Components[key])
                    {
                        if (sel.Intersects(r))
                        {
                            if (!m_Selected.Contains(sel))
                            {
                                sel.Entity.InvokeEvent("Select", null);
                                m_Selected.Add(sel);
                                return;
                            }
                        }
                    }
                }
            }
        }

        public void RegisterComponent(Component c)
        {
            if (c is Selectable)
            {
                int key = c.Entity.AddProperty<Int32>("Layer", 1).Value;
                if (!m_Components.ContainsKey(key))
                    m_Components.Add(key, new List<Selectable>());
                m_Components[key].Add(c as Selectable);
            }
            else if (c is SelectionBox)
                m_SelectionBox = c as SelectionBox;
        }

        public void RemoveComponent(Component c)
        {
            if (c.Entity.HasProperty<Int32>("Layer"))
            {
                int key = c.Entity.AddProperty<Int32>("Layer", 1).Value;
                if (m_Components.ContainsKey(key))
                    if (m_Components[key].Contains(c as Selectable))
                    {
                        m_Components[key].Remove(c as Selectable);
                        if (m_Components[key].Count <= 0)
                            m_Components.Remove(key);
                    }
            }
            if (c == m_SelectionBox)
                m_SelectionBox = null;
       
        }

        public void Do(float elapsedTime)
        {
            /*if (m_SelectionBox != null && m_SelectionBox.Selecting)
            {
                foreach (Int32 key in m_Components.Keys)
                {
                    foreach (Selectable sel in m_Components[key])
                    {
                        if (sel.Intersects(m_SelectionBox.Box))
                        {
                            if (!m_Selected.Contains(sel))
                            {
                                sel.Entity.InvokeEvent("Select", null);
                                m_Selected.Add(sel);
                            }
                        }
                        else
                        {
                            if (m_Selected.Contains(sel))
                            {
                                sel.Entity.InvokeEvent("Deselect", null);
                                m_Selected.Remove(sel);
                            }
                        }
                    }
                }
            }*/
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
