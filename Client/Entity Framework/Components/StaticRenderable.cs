using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Kannon.Components
{
    class StaticRenderable : Component, IRenderableComponent
    {
        [ComponentCreator]
        public static Component Create(Entity ent, String name)
        {
            return new StaticRenderable(ent, name);
        }

        Property<String> m_Filename;
        Property<Vector2> m_Position;
        Property<Vector2> m_Origin;
        Property<float> m_Rotation;
        Property<float> m_Scale;
        SpriteBatch m_SpriteBatch;
        Texture2D m_Texture;

        public bool Otherd = false;

        public StaticRenderable(Entity ent, String name) : base(ent, name)
        {
            m_Filename = Entity.AddProperty<String>("Graphics.Filename", "ERROR");
            m_Position = Entity.AddProperty<Vector2>("Position", Vector2.Zero);
            m_Origin = Entity.AddProperty<Vector2>("Origin", Vector2.Zero);
            m_Rotation = Entity.AddProperty<float>("Rotation", 0.0f);
            m_Scale = Entity.AddProperty<float>("Scale", 1.0f);
        }

        public void Load(Microsoft.Xna.Framework.Content.ContentManager cm, SpriteBatch sb)
        {
            m_SpriteBatch = sb;
            m_Texture = cm.Load<Texture2D>(m_Filename.Value);
        }

        public void Render()
        {
            m_SpriteBatch.Draw(m_Texture, m_Position.Value, null, Color.White, m_Rotation.Value, m_Origin.Value, m_Scale.Value, SpriteEffects.None, 0.0f);
        }

        public override void Parse(System.Xml.XmlNode data)
        {
            if (data.HasChildNodes)
                if (data.FirstChild.Name == "Other")
                    Otherd = true;
        }
    }
}
