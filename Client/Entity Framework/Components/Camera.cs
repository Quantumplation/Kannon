using Microsoft.Xna.Framework;

namespace Kannon.Components
{
    public class Camera : Component, ITransformer
    {
        Matrix m_Scale;
        Matrix m_Translation;

        Property<Vector2> m_Position;
        Property<Vector2> m_ScreenDimensions;
        Property<float> m_Zoom;

        [ComponentCreator]
        public static Component Create(Entity ent, string name)
        {
            return new Camera(ent, name) as Component;
        }

        public Camera(Entity ent, string name) : base(ent, name)
        {
            m_Position = Entity.AddProperty<Vector2>("Position", Vector2.Zero);
            m_ScreenDimensions = GlobalProperties.Instance.AddProperty<Vector2>("ScreenDimensions", Vector2.Zero);
            m_Zoom = Entity.AddProperty<float>("Zoom", 1.1f);

            m_Position.ValueChanged += new ValueChanged<Vector2>(PositionChanged);
            m_Zoom.ValueChanged += new ValueChanged<float>(ZoomChanged);

            ZoomChanged(0, m_Zoom.Value);
            PositionChanged(Vector2.Zero, m_Position.Value);
            
            Entity.AddEvent("SetActiveCamera", (o) => SetActiveCamera());
        }

        void SetActiveCamera()
        {
            Broadphases.Graphics gr = XNAGame.Instance.GetBroadphase<Broadphases.Graphics>("Graphics") as Broadphases.Graphics;
            gr.SetTransformer(this, 1, 100);
            
        }

        void ZoomChanged(float oldValue, float newValue)
        {
            m_Scale = Matrix.CreateScale(newValue, newValue, 1);
        }

        void PositionChanged(Vector2 oldValue, Vector2 newValue)
        {
            m_Translation = Matrix.CreateTranslation(-newValue.X, -newValue.Y, 0);
        }

        public override void Parse(System.Xml.XmlNode data)
        {
            if (data.Attributes["active"] != null)
                if (bool.Parse(data.Attributes["active"].Value) == true)
                    SetActiveCamera();
        }

        public Matrix GetTransformation(int Layer)
        {
            Matrix tempTranslate = m_Translation;
            tempTranslate.Translation /= (float)Layer;
            Matrix screen = Matrix.CreateTranslation((m_ScreenDimensions.Value.X / 2), (m_ScreenDimensions.Value.Y / 2), 0);
            return tempTranslate * m_Scale * screen;
        }
    }
}