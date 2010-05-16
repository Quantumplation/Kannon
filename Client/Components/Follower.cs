using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Kannon.EntitySystem;
using Kannon.EntitySystem.Components;

namespace Kannon.Components
{
    /// <summary>
    ///  This is a test component to show off the screen to world projection
    /// </summary>
    class Follower : Component, IGenericComponent
    {
        [ComponentCreator]
        public static Component Create(Entity ent, String name)
        {
            return new Follower(ent, name);
        }

        bool active = false;
        Property<Microsoft.Xna.Framework.Vector3> m_Position;

        public Follower(Entity ent, String name) : base(ent, name)
        {
            XNAGame.Instance.GetBroadphase<Broadphases.Input>("Input").KeyPressed += new Broadphases.KeyEventHandler(Follower_KeyPressed);
            m_Position = ent.AddProperty<Microsoft.Xna.Framework.Vector3>("Position", Microsoft.Xna.Framework.Vector3.Zero);
        }

        void Follower_KeyPressed(Broadphases.Input.KeyData key)
        {
            if (key.keyCode == Microsoft.Xna.Framework.Input.Keys.S)
                active = !active;
        }

        public override void Parse(System.Xml.XmlNode data)
        {
        }

        public void Update(float elapsedTime)
        {
            if (active)
            {
                Microsoft.Xna.Framework.Vector2 screenPos = XNAGame.Instance.GetBroadphase<Broadphases.Input>("Input").MousePosition;
                m_Position.Value += (Camera.ScreenToWorld(screenPos, m_Position.Value.Z) - m_Position.Value) / 100;
            }
        }
    }
}
