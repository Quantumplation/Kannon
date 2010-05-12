using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Kannon.Components
{
    /// <summary>
    /// When attached to an entity, uses the entities position
    /// while the mouse button is held to create a selection box.
    /// Note: This is very much a work in progress.  Once new camera system
    /// is implemented, huge chunks of this will need to be rewritten.
    /// </summary>
    class SelectionBox : Component, IRenderable, IContent
    {
        Texture2D m_Image;
        Vector3 m_StartPosition;
        Vector3 m_CurrentPosition;

        Property<Vector3> m_Position;

        Broadphases.Input m_InputObj;

        private Rectangle m_Box;
        public Rectangle Box
        {
            get { return m_Box; }
            set { m_Box = value; }
        }

        [ComponentCreator]
        public static Component Create(Entity ent, String name)
        {
            return new SelectionBox(ent, name);
        }

        public SelectionBox(Entity ent, String name)
            : base(ent, name)
        {
            m_Position = Entity.AddProperty<Vector3>("Position", Vector3.Zero);
            m_InputObj = XNAGame.Instance.GetBroadphase<Broadphases.Input>("Input");

            m_InputObj.ButtonClicked += new Broadphases.MouseEventHandler(ButtonClicked);
            m_InputObj.ButtonHeld += new Broadphases.MouseEventHandler(ButtonHeld);
            m_InputObj.ButtonReleased += new Broadphases.MouseEventHandler(ButtonReleased);
        }

        void ButtonClicked(Broadphases.Input.MouseData data)
        {
            if (data.mouseButtons[0])
            {
                m_Box = new Rectangle((int)data.mouseX, (int)data.mouseY, 0, 0);
                m_CurrentPosition = new Vector3(data.mouseX, data.mouseY, 0.0f);
            }
        }

        void ButtonHeld(Broadphases.Input.MouseData data)
        {
            if( data.mouseButtons[0] )
            {
                Vector3 pos = new Vector3(data.mouseX, data.mouseY, 0.0f);
                m_CurrentPosition = pos;
                m_Box.Width = (int)pos.X - m_Box.X;
                m_Box.Height = (int)pos.Y - m_Box.Y;
                Selecting = true;
            }
        }

        void ButtonReleased(Broadphases.Input.MouseData data)
        {
            Box = Rectangle.Empty;
            Selecting = false;
        }

        public override void Parse(System.Xml.XmlNode data)
        {
            XNAGame.Instance.GetBroadphase<Broadphases.Graphics>("Graphics").AddComponentToPass(this, "UI");
        }

        public bool Selecting
        {
            get;
            set;
        }

        public void Render(Microsoft.Xna.Framework.Graphics.SpriteBatch sb, String passID = "")
        {
            if (Selecting)
            {
                sb.Draw(m_Image, Box, new Color(new Vector4(Color.DarkGreen.ToVector3(), 150)));
            }
        }

        public void Load(Microsoft.Xna.Framework.Content.ContentManager cm)
        {
            m_Image = cm.Load<Texture2D>("Selection");
        }
    }
}
