using Microsoft.Xna.Framework;

namespace Kannon.Components
{
    public class Camera : Component, ITransformer
    {
        Matrix m_ScaleMatrix;
        Matrix m_TranslationMatrix;
        Matrix m_RotationMatrix;

        Property<Vector2> m_ScreenDimensions;
        Property<Vector2> m_Position;
        Property<float> m_Rotation;
        Property<float> m_Zoom;

        [ComponentCreator]
        public static Component Create(Entity ent, string name)
        {
            return new Camera(ent, name) as Component;
        }

        public Camera(Entity ent, string name) : base(ent, name)
        {
            m_ScreenDimensions = GlobalProperties.Instance.AddProperty<Vector2>("ScreenDimensions", Vector2.Zero);
            m_Position = Entity.AddProperty<Vector2>("Position", Vector2.Zero);
            m_Rotation = Entity.AddProperty<float>("Rotation", 0.0f);
            m_Zoom = Entity.AddProperty<float>("Zoom", 1.0f);

            m_Position.ValueChanged += new ValueChanged<Vector2>(PositionChanged);
            m_Rotation.ValueChanged += new ValueChanged<float>(RotationChanged);
            m_Zoom.ValueChanged += new ValueChanged<float>(ZoomChanged);

            PositionChanged(Vector2.Zero, m_Position.Value);
            RotationChanged(0, m_Rotation.Value);
            ZoomChanged(0, m_Zoom.Value);
            
            Entity.AddEvent("SetActive", (o) => SetActiveCamera());
        }


        void SetActiveCamera()
        {
            Broadphases.Graphics gr = XNAGame.Instance.GetBroadphase<Broadphases.Graphics>("Graphics") as Broadphases.Graphics;
            gr.SetTransformer(this, 1, 100);
            
        }

        void PositionChanged(Vector2 oldValue, Vector2 newValue)
        {
            m_TranslationMatrix = Matrix.CreateTranslation(-newValue.X, -newValue.Y, 0);
        }

        void RotationChanged(float oldValue, float newValue)
        {
            m_RotationMatrix = Matrix.CreateRotationZ(newValue);
        }

        void ZoomChanged(float oldValue, float newValue)
        {
            m_ScaleMatrix = Matrix.CreateScale(newValue, newValue, 1);
        }


        public override void Parse(System.Xml.XmlNode data)
        {
            if (data.Attributes["active"] != null)
                if (bool.Parse(data.Attributes["active"].Value) == true)
                    SetActiveCamera();
        }

        public Matrix GetTransformation(int Layer)
        {
            Matrix screen = Matrix.CreateTranslation((m_ScreenDimensions.Value.X / 2), (m_ScreenDimensions.Value.Y / 2), 0);
            Matrix tempScale = Matrix.CreateScale(1 / (float)Layer, 1 / (float)Layer, 1);
            return m_TranslationMatrix * m_RotationMatrix * m_ScaleMatrix * tempScale * screen;
        }

        public static Vector2 ScreenToWorld(Vector2 screenPos, int Layer)
        {
            return Vector2.Transform(screenPos, ScreenToWorldMatrix(Layer));
        }

        public static Matrix ScreenToWorldMatrix(int Layer)
        {
            Matrix trans = Matrix.Invert(XNAGame.Instance.GetBroadphase<Broadphases.Graphics>("Graphics").GetTransformer(Layer).GetTransformation(Layer));
            return trans;
        }

        public static Vector2 WorldToScreen(Vector2 worldPos, int Layer)
        {
            return Vector2.Transform(worldPos, WorldToScreenMatrix(Layer));
        }

        public static Matrix WorldToScreenMatrix(int Layer)
        {
            Matrix trans = XNAGame.Instance.GetBroadphase<Broadphases.Graphics>("Graphics").GetTransformer(Layer).GetTransformation(Layer);
            return trans;
        }
    }
}