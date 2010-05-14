using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Kannon.EntitySystem;
using Kannon.EntitySystem.Components;

namespace Kannon.Components
{
    /// <summary>
    /// Temporary solution for controlling things with the keyboard.
    /// </summary>
    class KeyboardController : Component, IGenericComponent
    {
        [ComponentCreator]
        public static Component Create(Entity ent, String name)
        {
            return new KeyboardController(ent, name);
        }

        Broadphases.Input m_InputObj;

        Property<Vector3> m_Position;

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
            m_Position = Entity.AddProperty<Vector3>("Position", Vector3.Zero);

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
                m_Position.Value -= Vector3.UnitY * Speed * elapsedTime;
            if (m_InputObj.IsDown(Keys.Down))
                m_Position.Value += Vector3.UnitY * Speed * elapsedTime;
            if (m_InputObj.IsDown(Keys.Left))
                m_Position.Value -= Vector3.UnitX * Speed * elapsedTime;
            if (m_InputObj.IsDown(Keys.Right))
                m_Position.Value += Vector3.UnitX * Speed * elapsedTime;
            if (m_InputObj.IsDown(Keys.PageUp) && Entity.HasProperty<float>("Zoom"))
            {
                if (Entity.GetProperty<float>("Zoom").Value - .01f * Speed * elapsedTime <= GlobalProperties.Instance.GetProperty<float>("MaxZoom").Value)
                    Entity.GetProperty<float>("Zoom").Value = GlobalProperties.Instance.GetProperty<float>("MaxZoom").Value;
                else
                    Entity.GetProperty<float>("Zoom").Value -= .01f * Speed * elapsedTime;
            }
            if (m_InputObj.IsDown(Keys.PageDown) && Entity.HasProperty<float>("Zoom"))
            {
                if (Entity.GetProperty<float>("Zoom").Value + .01f * Speed * elapsedTime >= GlobalProperties.Instance.GetProperty<float>("MinZoom").Value)
                    Entity.GetProperty<float>("Zoom").Value = GlobalProperties.Instance.GetProperty<float>("MinZoom").Value;
                else
                    Entity.GetProperty<float>("Zoom").Value += .01f * Speed * elapsedTime;
            }
            if (m_InputObj.IsDown(Keys.R))
                m_Position.Value = new Vector3(-500, -500, m_Position.Value.Z);
                
        }
    }
}
