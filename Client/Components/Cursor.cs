﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Kannon.EntitySystem;
using Kannon.EntitySystem.Components;

namespace Kannon.Components
{
    /// <summary>
    /// Component which sets the position of the entity to the position of the mouse.
    /// Software mouse allows complex cursor behaviors, such as rotating when clicked, 
    /// animating based on context, etc, but might run slower than a Hardware mouse.
    /// </summary>
    class Cursor : Component
    {
        Property<Vector3> m_Position;

        bool transform = false;

        [ComponentCreator]
        public static Component Create(Entity ent, String name)
        {
            return new Cursor(ent, name);
        }

        public Cursor(Entity ent, String name) : base(ent, name)
        {
            m_Position = Entity.AddProperty<Vector3>("Position", Vector3.Zero);
            Broadphases.Input inp = XNAGame.Instance.GetBroadphase<Broadphases.Input>("Input");
            inp.MouseMoved += new Broadphases.MouseEventHandler(inp_MouseMoved);
        }

        void inp_MouseMoved(Broadphases.Input.MouseData data)
        {
            if( !transform )
                m_Position.Value = new Vector3(data.mouseX, data.mouseY, 0.0f);
            if (transform)
                m_Position.Value = Camera.ScreenToWorld(new Vector2(data.mouseX, data.mouseY) , 0.0f);
        }

        public override void Parse(System.Xml.XmlNode data)
        {
            if (data.Attributes["transform"] != null)
                transform = bool.Parse(data.Attributes["transform"].Value);
        }
    }
}
