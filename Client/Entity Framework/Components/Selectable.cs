using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace Kannon.Components
{
    class Selectable : Component
    {
        Property<bool> m_Selected;
        Property<Vector2> m_Position;
        Property<Vector2> m_Bounds;
        Property<float> m_Scale;
        Property<Int32> m_Layer;

        Rectangle transformedBoundary;
        Vector2 scaledSize;

        [ComponentCreator]
        public static Component Create(Entity ent, String name)
        {
            return new Selectable(ent, name);
        }

        public Selectable(Entity ent, String name) : base(ent, name)
        {
            m_Selected = Entity.AddProperty<bool>("Selected", false);
            m_Position = Entity.AddProperty<Vector2>("Position", Vector2.Zero);
            m_Bounds = Entity.AddProperty<Vector2>("Bounds", Vector2.Zero);
            m_Scale = Entity.AddProperty<float>("Scale", 0.0f);
            m_Layer = Entity.AddProperty<Int32>("Layer", 1);

            m_Position.ValueChanged += new ValueChanged<Vector2>(PositionChanged);
            m_Bounds.ValueChanged += new ValueChanged<Vector2>(BoundsChanged);
            m_Scale.ValueChanged += new ValueChanged<float>(ScaleChanged);

            Entity.AddEvent("Select", (o) => ChangeSelect(true));
            Entity.AddEvent("Deselect", (o) => ChangeSelect(false));
        }

        void PositionChanged(Vector2 oldValue, Vector2 newValue)
        {
            transformedBoundary.X = (int)(newValue.X - scaledSize.X/2);
            transformedBoundary.Y = (int)(newValue.Y - scaledSize.Y/2);
        }
        
        void BoundsChanged(Vector2 oldValue, Vector2 newValue)
        {
            scaledSize = newValue * m_Scale.Value;
            transformedBoundary.Width = (int)scaledSize.X;
            transformedBoundary.Height = (int)scaledSize.Y;
            PositionChanged(Vector2.Zero, m_Position.Value);
        }

        void ScaleChanged(float oldValue, float newValue)
        {
            scaledSize = m_Bounds.Value * newValue;
            transformedBoundary.Width = (int)scaledSize.X;
            transformedBoundary.Height = (int)scaledSize.Y;
            PositionChanged(Vector2.Zero, m_Position.Value);
        }

        public void ChangeSelect(bool selected)
        {
            m_Selected.Value = selected;
        }

        public bool Intersects(Rectangle rect)
        {
            Vector2 tl = Vector2.Transform(Vector2.Transform(new Vector2(rect.X, rect.Y), Camera.WorldToScreenMatrix(1)), Camera.ScreenToWorldMatrix(m_Layer.Value));
            Vector2 bl = Vector2.Transform(Vector2.Transform(new Vector2(rect.X + rect.Width, rect.Y + rect.Height), Camera.WorldToScreenMatrix(1)), Camera.ScreenToWorldMatrix(m_Layer.Value));
            Rectangle newRect = new Rectangle((int)Math.Min(tl.X, bl.X), (int)Math.Min(tl.Y, bl.Y), (int)Math.Abs(bl.X - tl.X), (int)Math.Abs(bl.Y - tl.Y));

            return transformedBoundary.Intersects(newRect);
        }

        public override void Parse(System.Xml.XmlNode data)
        {
        }
    }
}
