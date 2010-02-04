using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Kannon.Components
{
    class SelectionBox : Component, IRenderableComponent, IContentComponent
    {
        Texture2D m_Image;
        Vector2 m_StartPosition;
        Vector2 m_CurrentPosition;

        Property<Vector2> m_Position;

        Broadphases.Input m_InputObj;

        public Rectangle Box
        {
            get
            {
                Vector2 transCurrent = Vector2.Transform(new Vector2(m_CurrentPosition.X, m_CurrentPosition.Y), Camera.ScreenToWorldMatrix(Layer));
                return new Rectangle((int)m_StartPosition.X, (int)m_StartPosition.Y, (int)(transCurrent.X - m_StartPosition.X), (int)(transCurrent.Y - m_StartPosition.Y));
            }
            set{}
        }

        [ComponentCreator]
        public static Component Create(Entity ent, String name)
        {
            return new SelectionBox(ent, name);
        }

        public SelectionBox(Entity ent, String name)
            : base(ent, name)
        {
            m_Position = Entity.AddProperty<Vector2>("Position", Vector2.Zero);
            m_InputObj = XNAGame.Instance.GetBroadphase<Broadphases.Input>("Input");

            m_InputObj.ButtonClicked += new Broadphases.MouseEventHandler(ButtonClicked);
            m_InputObj.ButtonHeld += new Broadphases.MouseEventHandler(ButtonHeld);
            m_InputObj.ButtonReleased += new Broadphases.MouseEventHandler(ButtonReleased);
        }

        void ButtonClicked(Broadphases.Input.MouseData data)
        {
            if (data.mouseButtons[0])
            {
                Vector2 pos = Camera.ScreenToWorld(new Vector2(data.mouseX, data.mouseY), 1);
                m_StartPosition = pos;
                m_CurrentPosition = pos;
            }
        }

        void ButtonHeld(Broadphases.Input.MouseData data)
        {
            if( data.mouseButtons[0] )
            {
                Vector2 pos = new Vector2(data.mouseX, data.mouseY);
                m_CurrentPosition = pos;
                Selecting = true;
            }
        }

        void ButtonReleased(Broadphases.Input.MouseData data)
        {
            m_StartPosition = Vector2.Zero;
            m_CurrentPosition = Vector2.Zero;
            Selecting = false;
        }

        public override void Parse(System.Xml.XmlNode data)
        {
        }

        public int Layer
        {
            get
            {
                return 1;
            }
            set { }
        }

        public bool Selecting
        {
            get;
            set;
        }

        public void Render(Microsoft.Xna.Framework.Graphics.SpriteBatch sb, int Layer)
        {
            if (Selecting)
            {
                Rectangle box = Box;
                Vector2 tl = Vector2.Transform(Vector2.Transform(new Vector2(box.X, box.Y), Camera.WorldToScreenMatrix(1)), Camera.ScreenToWorldMatrix(Layer));
                Vector2 bl = new Vector2(box.X+box.Width, box.Y+box.Height);
                Rectangle newRect = new Rectangle((int)Math.Min(tl.X, bl.X), (int)Math.Min(tl.Y, bl.Y), (int)Math.Abs(bl.X - tl.X), (int)Math.Abs(bl.Y - tl.Y));

                sb.Draw(m_Image, newRect, new Color(Color.DarkGreen, 150));
            }
        }

        public void Load(Microsoft.Xna.Framework.Content.ContentManager cm)
        {
            m_Image = cm.Load<Texture2D>("Selection");
        }
    }
}
