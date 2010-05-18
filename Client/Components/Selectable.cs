using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Kannon.EntitySystem;
using Kannon.EntitySystem.Components;

namespace Kannon.Components
{
    /// <summary>
    /// Component that, when attached, allows the entity to be selected.
    /// Note: This is very much a work in progress.  Once new camera system
    /// is implemented, huge chunks of this will need to be rewritten.
    /// </summary>
    class Selectable : Component
    {
        Property<bool> m_Selected;
        Property<Vector3> m_Position;
        Property<Vector2> m_Origin;
        Property<Vector2> m_Bounds;
        Property<float> m_Scale;

        // Rectangle, in world coordinates, defining the object
        Rectangle transformedBoundary;
        Vector2 scaledSize;

        String m_Pass = "Unsorted";

        [ComponentCreator]
        public static Component Create(Entity ent, String name)
        {
            return new Selectable(ent, name);
        }

        public Selectable(Entity ent, String name) : base(ent, name)
        {
            m_Selected = Entity.AddProperty<bool>("Selected", false);
            m_Position = Entity.AddProperty<Vector3>("Position", Vector3.Zero);
            m_Origin = Entity.AddProperty<Vector2>("Origin", Vector2.Zero);
            m_Bounds = Entity.AddProperty<Vector2>("Bounds", Vector2.Zero);
            m_Scale = Entity.AddProperty<float>("Scale", 0.0f);
            
            m_Position.ValueChanged += new ValueChanged<Vector3>(PositionChanged);
            m_Bounds.ValueChanged += new ValueChanged<Vector2>(BoundsChanged);
            m_Scale.ValueChanged += new ValueChanged<float>(ScaleChanged);

            Entity.AddEvent("Select", (o) => ChangeSelect(true));
            Entity.AddEvent("Deselect", (o) => ChangeSelect(false));


        }
        
        // Transformed boundary:
        // Position should line up with origin.
        // Width and height are scaled by the Scale Value
        // transformedBoundary.X = m_Position.Value.X - (m_Origin.Value.X * m_Scale.Value);
        // transformedBoundary.Y = m_Position.Value.Y - (m_Origin.Value.Y * m_Scale.Value);
        // transformedBoundary.Width = m_Bounds.Value.X * m_Scale.Value;
        // transformedBoundary.Height = m_Bounds.Value.Y * m_Scale.Value;

        void PositionChanged(Vector3 oldValue, Vector3 newValue)
        {
            transformedBoundary.X = (int)(newValue.X - (m_Origin.Value.X * m_Scale.Value));
            transformedBoundary.Y = (int)(newValue.Y - (m_Origin.Value.Y * m_Scale.Value));
            transformedBoundary.Width = (int)(newValue.X * m_Scale.Value);
            transformedBoundary.Height = (int)(newValue.Y * m_Scale.Value);
        }
        
        void BoundsChanged(Vector2 oldValue, Vector2 newValue)
        {
            transformedBoundary.X = (int)(m_Position.Value.X - (m_Origin.Value.X * m_Scale.Value));
            transformedBoundary.Y = (int)(m_Position.Value.Y - (m_Origin.Value.Y * m_Scale.Value));
            transformedBoundary.Width = (int)(newValue.X * m_Scale.Value);
            transformedBoundary.Height = (int)(newValue.Y * m_Scale.Value);
        }

        void ScaleChanged(float oldValue, float newValue)
        {
            transformedBoundary.X = (int)(m_Position.Value.X - (m_Origin.Value.X * newValue));
            transformedBoundary.Y = (int)(m_Position.Value.Y - (m_Origin.Value.Y * newValue));
            transformedBoundary.Width = (int)(m_Bounds.Value.X * newValue);
            transformedBoundary.Height = (int)(m_Bounds.Value.Y * newValue);
        }

        public void ChangeSelect(bool selected)
        {
            m_Selected.Value = selected;
        }

        public bool Intersects(Rectangle rect)
        {
            Rectangle fixedR = rect.FixCorners();
            // Transform the given rect into world coordinates on this guys layer
            Vector3 tl = Camera.ScreenToWorld(new Vector2(fixedR.X, fixedR.Y), m_Position.Value.Z, m_Pass);
            Vector3 br = Camera.ScreenToWorld(new Vector2(fixedR.X + fixedR.Width, fixedR.Y + fixedR.Height), m_Position.Value.Z, m_Pass);
            Rectangle newRect = new Rectangle((int)tl.X, (int)tl.Y, (int)(br.X - tl.X), (int)(br.Y - tl.Y));
            bool inter = newRect.Intersects(transformedBoundary);
            if (inter)
                return true;
            return false;
        }

        public override void Parse(System.Xml.XmlNode data)
        {
            if (data.Attributes["pass"] != null)
            {
                m_Pass = data.Attributes["pass"].Value;
            }
        }
    }
}
