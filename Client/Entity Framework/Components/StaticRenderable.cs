using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Kannon.Components
{
    class StaticRenderable : Component, IRenderableComponent, IContentComponent
    {
        [ComponentCreator]
        public static Component Create(Entity ent, String name)
        {
            return new StaticRenderable(ent, name);
        }

        Property<Vector2> m_Position;
        Property<Vector2> m_Origin;
        Property<float> m_Rotation;
        Property<float> m_Scale;
        String m_Filename;
        Texture2D m_Texture;

        public bool Otherd = false;

        public StaticRenderable(Entity ent, String name) : base(ent, name)
        {
            m_Position = Entity.AddProperty<Vector2>("Position", Vector2.Zero);
            m_Origin = Entity.AddProperty<Vector2>("Origin", Vector2.Zero);
            m_Rotation = Entity.AddProperty<float>("Rotation", 0.0f);
            m_Scale = Entity.AddProperty<float>("Scale", 1.0f);
        }

        public void Load(Microsoft.Xna.Framework.Content.ContentManager cm)
        {
            m_Texture = cm.Load<Texture2D>(m_Filename);
            if (m_Origin.Value == Vector2.Zero)
                m_Origin.Value = new Vector2(m_Texture.Width / 2, m_Texture.Height / 2);
        }

        public void Render(SpriteBatch sb)
        {
            sb.Draw(m_Texture, m_Position.Value, null, Color.White, m_Rotation.Value, m_Origin.Value, m_Scale.Value, SpriteEffects.None, 0.0f);
        }

        public override void Parse(System.Xml.XmlNode data)
        {
            if (data.Attributes["layer"] != null)
            {
                int newLayer = Int32.Parse(data.Attributes["layer"].Value);
                XNAGame.Instance.GetBroadphase<Broadphases.Graphics>("Graphics").ChangeLayer(this, Layer, newLayer);
                Layer = newLayer;
            }
            if (data.Attributes["file"] != null)
                m_Filename = data.Attributes["file"].Value;
            else
                m_Filename = "ERROR";
        }

        public Int32 Layer
        {
            get;
            set;
        }
    }
}
