using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Kannon.Components
{
    class KeyboardController : Component, IGenericComponent
    {
        [ComponentCreator]
        public static Component Create(Entity ent, String name)
        {
            return new KeyboardController(ent, name);
        }

        Broadphases.Input m_InputObj;

        Property<Vector2> m_Position;

        public Boolean IsActive
        {
            get;
            set;
        }

        public float Speed
        {
            get;
            set;
        }

        public KeyboardController(Entity ent, String name) : base(ent, name)
        {
            m_InputObj = XNAGame.Instance.GetBroadphase<Broadphases.Input>("Input");
            m_Position = Entity.AddProperty<Vector2>("Position", Vector2.Zero);

            Entity.AddEvent("SetActive", (o) => this.IsActive = true);
            Entity.AddEvent("SetInactive", (o) => this.IsActive = false);

            IsActive = true;
            Speed = 0.5f;
        }

        public override void Parse(System.Xml.XmlNode data)
        {
        }

        public void Update(float elapsedTime)
        {
            if( !IsActive )
                return;
            if (m_InputObj.IsDown(Keys.Up))
                m_Position.Value -= Vector2.UnitY * Speed * elapsedTime;
            if (m_InputObj.IsDown(Keys.Down))
                m_Position.Value += Vector2.UnitY * Speed * elapsedTime;
            if (m_InputObj.IsDown(Keys.Left))
                m_Position.Value -= Vector2.UnitX * Speed * elapsedTime;
            if (m_InputObj.IsDown(Keys.Right))
                m_Position.Value += Vector2.UnitX * Speed * elapsedTime;
        }
    }
}
