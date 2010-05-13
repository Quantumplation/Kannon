using Microsoft.Xna.Framework;
using System;

namespace Kannon.Components
{
    /// <summary>
    /// Component that, when attached, keeps track of a view and projection matrix.
    /// </summary>
    public class Camera : Component, ITransformer
    {
        /// <summary>
        /// View and Projection matrices.
        /// </summary>
        Matrix m_View;
        Matrix m_Proj;

        /// <summary>
        /// Properties from the entity or elsewhere.
        /// </summary>
        // The global property for width and height of the screen.
        Property<Vector2> m_ScreenDimensions;
        // The position of the entity the camera is attached to.
        Property<Vector3> m_Position;
        // The rotation of the entity the camera is attached to.
        Property<float> m_Rotation;
        // The zoom value specified by the entity (Obsolete?)
        Property<float> m_Zoom;
        
        //The pass this camera is located on.
        string m_Pass;

        [ComponentCreator]
        public static Component Create(Entity ent, string name)
        {
            return new Camera(ent, name) as Component;
        }

        public Camera(Entity ent, string name) : base(ent, name)
        {
            // Retrieve the Screen dimensions from global.
            m_ScreenDimensions = GlobalProperties.Instance.AddProperty<Vector2>("ScreenDimensions", Vector2.Zero);
            // Retrieve position, rotation, and zoom.
            m_Rotation = Entity.AddProperty<float>("Rotation", 0.0f);
            m_Zoom = Entity.AddProperty<float>("Zoom", 10);
            m_Position = Entity.AddProperty<Vector3>("Position", Vector3.UnitZ * m_Zoom.Value);

            // For now, our transform does NOTHING.
            m_View = Matrix.Identity;
            m_Proj = Matrix.Identity;

            m_Position.ValueChanged += new ValueChanged<Vector3>(PositionChanged);
            m_Zoom.ValueChanged += new ValueChanged<float>(ZoomChanged);
            m_Rotation.ValueChanged += new ValueChanged<float>(RotationChanged);

            RecalculateView(m_Position.Value);

            m_Proj = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(40.0f), 1, 1.0f, 1000.0f) * Matrix.CreateTranslation(new Vector3(m_ScreenDimensions.Value.X/2, m_ScreenDimensions.Value.Y/2, 0.0f));

            // Whenever the entities "SetActive" event is triggered, make this camera active.
            Entity.AddEvent("SetActive", (o) => SetActiveCamera());
        }

        /// <summary>
        /// Takes the Pass variable (should be parsed from the XML if things are going well), and sets this
        /// camera as the transformer for that pass.  Any objects on that pass will then be transformed by this
        /// camera.
        /// </summary>
        void SetActiveCamera()
        {
            if (m_Pass != null)
            {
                Broadphases.Graphics gr = XNAGame.Instance.GetBroadphase<Broadphases.Graphics>("Graphics") as Broadphases.Graphics;
                gr.SetTransformer(this, m_Pass);
            }
            else
            {/*Somethings wrong with this camera, show a warning*/}
        }

        void PositionChanged(Vector3 oldValue, Vector3 newValue)
        {
            RecalculateView(newValue);
            // Update the view/projection matrix for the new position values.
            // Note: Initial implementation, just recalculate it.
            // Possible Optimization: transform the matrix based on the difference between old and new.
        }

        void RotationChanged(float oldValue, float newValue)
        {
            RecalculateView(m_Position.Value);
            // Update the view/projection matrix for the new rotation value.
            // Note: Initial implementation, just recalculate it.
            // Possible Optimization: transform the matrix based on the difference between old and new.
        }

        void ZoomChanged(float oldValue, float newValue)
        {
            // TODO: Refactor min/max zoom to here.
            m_Position.Value = new Vector3(m_Position.Value.XY(), newValue);
            // Update the view/projection matrix for the new zoom.
            // Note: Initial implementation, just recalculate it.
            // Possible Optimization: transform the matrix based on the difference between old and new.
        }

        void RecalculateView(Vector3 Position)
        {
            m_View = Matrix.CreateLookAt(new Vector3(Position.X, Position.Y, Position.Z), new Vector3(Position.X, Position.Y, Position.Z - 1), Vector3.UnitY);
        }

        /// <summary>
        /// Pull various configuration options out of the XML
        /// </summary>
        /// <param name="data"></param>
        public override void Parse(System.Xml.XmlNode data)
        {
            // Pull out the Pass variable, which determines what objects get influenced by this camera.
            if (data.Attributes["pass"] != null)
                m_Pass = data.Attributes["pass"].Value;
            // If we're set as active from the beginning, make it so.
            if (data.Attributes["active"] != null)
                if (bool.Parse(data.Attributes["active"].Value) == true)
                    SetActiveCamera();
        }

        /// <summary>
        /// Return the transformation to apply when rendering the layer this camera is on.
        /// </summary>
        /// <returns></returns>
        public Matrix GetTransformation()
        {
            return m_View * m_Proj;
        }

        /// <summary>
        /// Transform a screen position to a world position at a given depth, using the pass defined by PassID
        /// </summary>
        /// <param name="screenPos"></param>
        /// <param name="depth"></param>
        /// <param name="PassID"></param>
        /// <returns></returns>
        public static Vector3 ScreenToWorld(Vector2 screenPos, float depth, String PassID = "Unsorted")
        {
            return new Vector3(screenPos, depth);
        }

        /// <summary>
        /// Retrieve the matrix which transforms a point Vector3( ScreenPos, depth ); into the world using the
        /// pass defined by PassID.
        /// </summary>
        /// <param name="PassID"></param>
        /// <returns></returns>
        public static Matrix ScreenToWorldMatrix(String PassID = "Unsorted")
        {
            return Matrix.Identity;
        }

        /// <summary>
        ///  Transform a world position to a screen position using the pass defined by PassID
        /// </summary>
        /// <param name="worldPos"></param>
        /// <returns></returns>
        public static Vector2 WorldToScreen(Vector3 worldPos, string PassID = "Unsorted")
        {
            return worldPos.XY();
        }

        /// <summary>
        /// Get the matrix which transforms a given world position into screen coordinates through PassID;
        /// </summary>
        /// <returns></returns>
        public static Matrix WorldToScreenMatrix(string PassID = "Unsorted")
        {
            return Matrix.Identity;
        }
    }
}