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
        SpriteBatch m_SpriteBatch;
        Texture2D m_Texture;

        public StaticRenderable(Entity ent, String name) : base(ent, name)
        {
            m_Filename = Entity.AddProperty<String>("Graphics.Filename", "ERROR");
        }

        public void Load(Microsoft.Xna.Framework.Content.ContentManager cm, SpriteBatch sb)
        {
            m_SpriteBatch = sb;
            m_Texture = cm.Load<Texture2D>(m_Filename.Value);
        }

        public void Render()
        {
            m_SpriteBatch.Draw(m_Texture,new Vector2(100,100), Color.White);
        }
    }
}
