using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace Kannon.Components
{
    class Cursor : Component
    {
        Property<Vector2> m_Position;

        [ComponentCreator]
        public static Component Create(Entity ent, String name)
        {
            return new Cursor(ent, name);
        }

        public Cursor(Entity ent, String name) : base(ent, name)
        {
            m_Position = Entity.AddProperty<Vector2>("Position", Vector2.Zero);
            Broadphases.Input inp = XNAGame.Instance.GetBroadphase<Broadphases.Input>("Input");
            inp.MouseMoved += new Broadphases.MouseEventHandler(inp_MouseMoved);
        }

        void inp_MouseMoved(Broadphases.Input.MouseData data)
        {
            m_Position.Value = new Vector2(data.mouseX, data.mouseY);
        }

        public override void Parse(System.Xml.XmlNode data)
        {
        }
    }
}
