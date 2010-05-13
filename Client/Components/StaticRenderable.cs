using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Kannon.EntitySystem;
using Kannon.EntitySystem.Components;

namespace Kannon.Components
{
    /// <summary>
    /// Add a convenience method to produce a Vector2 from a Vector3.
    /// </summary>
    public static class VectorExtensions
    {
        public static Vector2 XY(this Vector3 vec)
        {
            return new Vector2(vec.X, vec.Y);
        }

        public static Vector2 YX(this Vector3 vec)
        {
            return new Vector2(vec.Y, vec.X);
        }

        public static Vector2 XZ(this Vector3 vec)
        {
            return new Vector2(vec.X, vec.Z);
        }

        public static Vector2 ZX(this Vector3 vec)
        {
            return new Vector2(vec.Z, vec.X);
        }

        public static Vector2 YZ(this Vector3 vec)
        {
            return new Vector2(vec.Y, vec.Z);
        }

        public static Vector2 ZY(this Vector3 vec)
        {
            return new Vector2(vec.Z, vec.Y);
        }
    }


    /// <summary>
    /// Makes an entity a static (not animated) renderable component.  Needs work =P
    /// </summary>
    class StaticRenderable : Component, IRenderable, IContent
    {
        [ComponentCreator]
        public static Component Create(Entity ent, String name)
        {
            return new StaticRenderable(ent, name);
        }

        Property<Vector3> m_Position;
        Property<Vector2> m_Origin;
        Property<Vector2> m_Bounds;
        Property<float> m_Rotation;
        Property<float> m_Scale;
        String m_Filename;
        Texture2D m_Texture;
        String m_Pass;

        public StaticRenderable(Entity ent, String name) : base(ent, name)
        {
            m_Position = Entity.AddProperty<Vector3>("Position", Vector3.Zero);
            m_Origin = Entity.AddProperty<Vector2>("Origin", Vector2.Zero);
            m_Bounds = Entity.AddProperty<Vector2>("Bounds", Vector2.Zero);
            m_Rotation = Entity.AddProperty<float>("Rotation", 0.0f);
            m_Scale = Entity.AddProperty<float>("Scale", 1.0f);
        }

        public void Load(Microsoft.Xna.Framework.Content.ContentManager cm)
        {
            m_Texture = cm.Load<Texture2D>(m_Filename);
            if (m_Origin.Value == Vector2.Zero)
                m_Origin.Value = new Vector2(m_Texture.Width / 2, m_Texture.Height / 2);
            if (m_Bounds.Value == Vector2.Zero)
                m_Bounds.Value = new Vector2(m_Texture.Width, m_Texture.Height);
        }

        public void Render(SpriteBatch sb, String passID = "")
        {
            Color col = Color.White;
            if (Entity.GetProperty<bool>("Selected") != null && Entity.GetProperty<bool>("Selected").Value)
                col = Color.Black;
            // Render the texture, at the XY position, the whole texture, with the entities rotation, offset by the origin,
            // with a set scale, No special effects, and a depth.  Note: Depth is negated to establish this convention:
            // 0 is the default depth.  Anything negative is coming closer to the camera, anything positive is going deeper
            // into the screen.
            sb.Draw(m_Texture, m_Position.Value.XY(), null, col, m_Rotation.Value, m_Origin.Value, m_Scale.Value, SpriteEffects.None, m_Position.Value.Z);
        }

        public override void Parse(System.Xml.XmlNode data)
        {
            if (data.Attributes["pass"] != null)
            {
                m_Pass = data.Attributes["pass"].Value;
                XNAGame.Instance.GetBroadphase<Broadphases.Graphics>("Graphics").AddComponentToPass(this, m_Pass);
            }
            if (data.Attributes["file"] != null)
                m_Filename = data.Attributes["file"].Value;
            else
                m_Filename = "ERROR_SOLID";
        }
    }
}
