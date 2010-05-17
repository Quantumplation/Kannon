using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Kannon.EntitySystem;
using Kannon.EntitySystem.Components;

namespace Kannon.Components
{
    /// <summary>
    /// Temporary solution for controlling things with the keyboard.
    /// </summary>
    class KeyboardController : Component, IGenericComponent
    {
        [ComponentCreator]
        public static Component Create(Entity ent, String name)
        {
            return new KeyboardController(ent, name);
        }

        Broadphases.Input m_InputObj;

        Property<Vector3> m_Position;

        public Boolean IsActive
        {
            get;
            set;
        }

        public float Speed
        {
            get;
            set;
        }

        public KeyboardController(Entity ent, String name) : base(ent, name)
        {
            m_InputObj = XNAGame.Instance.GetBroadphase<Broadphases.Input>("Input");
            m_Position = Entity.AddProperty<Vector3>("Position", Vector3.Zero);

            Entity.AddEvent("SetActive", (o) => this.IsActive = true);
            Entity.AddEvent("SetInactive", (o) => this.IsActive = false);

            IsActive = true;
            Speed = 0.5f;
        }

        public override void Parse(System.Xml.XmlNode data)
        {
        }

        public void Update(float elapsedTime)
        {
            if( !IsActive )
                return;
            if (m_InputObj.IsDown(Keys.Up))
                m_Position.Value -= Vector3.UnitY * Speed * elapsedTime;
            if (m_InputObj.IsDown(Keys.Down))
                m_Position.Value += Vector3.UnitY * Speed * elapsedTime;
            if (m_InputObj.IsDown(Keys.Left))
                m_Position.Value -= Vector3.UnitX * Speed * elapsedTime;
            if (m_InputObj.IsDown(Keys.Right))
                m_Position.Value += Vector3.UnitX * Speed * elapsedTime;
            if (m_InputObj.IsDown(Keys.PageUp) && Entity.HasProperty<float>("Zoom"))
            {
                Property<float> prop = Entity.GetProperty<float>("Zoom");
                prop.Value = Math.Max(prop.Value - (0.1f * Speed * elapsedTime), GlobalProperties.Instance.GetProperty<float>("MaxZoom").Value);
            }
            if (m_InputObj.IsDown(Keys.PageDown) && Entity.HasProperty<float>("Zoom"))
            {
                Property<float> prop = Entity.GetProperty<float>("Zoom");
                prop.Value = Math.Min(prop.Value + (0.1f * Speed * elapsedTime), GlobalProperties.Instance.GetProperty<float>("MinZoom").Value);
            }
            if (m_InputObj.IsDown(Keys.R))
                m_Position.Value = new Vector3(-500, -500, m_Position.Value.Z);
                
        }
    }

    /// <summary>
    /// Long term solution for controlling the camera (or objects, if you want)
    /// with the mouse.  Includes "smart zoom", and edge scrolling.
    /// </summary>
    public class StandardCameraController : Component, IGenericComponent
    {
        [ComponentCreator]
        public static Component Create(Entity ent, String name)
        {
            return new StandardCameraController(ent, name);
        }

        Broadphases.Input m_InputObj;

        Property<Vector3> m_Position;
        Property<float> m_Zoom;

        Vector2 m_ScreenDimensions;

        float minZoom;
        float maxZoom;

        float lastWheelPosition;
        float currentWheelPosition;

        int zoomDestinationIndex;
        float timerCounter;
        float startZoom;
        bool zooming = false;

        const int zoomPoints = 17;
        const float zoomDuration = 450f;
        const float edgeScrollThreshold = 100f;

        float[] zoomLevels;

        public Boolean IsActive
        {
            get;
            set;
        }

        public float KeyboardSpeed
        {
            get;
            set;
        }
        public float EdgeScrollSpeed
        {
            get;
            set;
        }
        public float ZoomSpeed
        {
            get;
            set;
        }

        public StandardCameraController(Entity ent, String name)
            : base(ent, name)
        {
            m_InputObj = XNAGame.Instance.GetBroadphase<Broadphases.Input>("Input");

            minZoom = GlobalProperties.Instance.GetProperty<float[]>("ZoomLevels").Value[0];
            int zoomcount = GlobalProperties.Instance.GetProperty<float[]>("ZoomLevels").Value.GetLength(0);
            maxZoom = GlobalProperties.Instance.GetProperty<float[]>("ZoomLevels").Value[zoomcount - 1];

            zoomLevels = GlobalProperties.Instance.GetProperty<float[]>("ZoomLevels").Value;

            m_Position = Entity.AddProperty<Vector3>("Position", Vector3.Zero);
            m_Zoom = Entity.AddProperty<float>("Zoom", (maxZoom + minZoom) / 2);

            float minDist = float.MaxValue;
            int minDistIndex = 0;
            int index = 0;
            foreach (float z in zoomLevels)
            {
                float dist = Math.Abs(m_Zoom.Value - z);
                if (dist < minDist)
                {
                    minDist = dist;
                    minDistIndex = index;
                }
                index++;
            }
            zoomDestinationIndex = minDistIndex;

            m_ScreenDimensions = GlobalProperties.Instance.GetProperty<Vector2>("ScreenDimensions").Value;

            Entity.AddEvent("SetActive", (o) => this.IsActive = true);
            Entity.AddEvent("SetInactive", (o) => this.IsActive = false);

            IsActive = true;
        }

        public override void Parse(System.Xml.XmlNode data)
        {
            if (data.Attributes["EdgeScroll"] != null)
                EdgeScrollSpeed = float.Parse(data.Attributes["EdgeScroll"].Value);
            else
                EdgeScrollSpeed = 1.0f;

            if (data.Attributes["Keyboard"] != null)
                KeyboardSpeed = float.Parse(data.Attributes["Keyboard"].Value);
            else
                KeyboardSpeed = 1.0f;

            if (data.Attributes["Zoom"] != null)
                ZoomSpeed = float.Parse(data.Attributes["Zoom"].Value);
            else
                ZoomSpeed = 1.0f;
        }

        public void Update(float elapsedTime)
        {
            if( !IsActive )
                return;

            #region Keyboard

            if (m_InputObj.IsDown(Keys.Up))
                m_Position.Value -= Vector3.UnitY * KeyboardSpeed * m_Zoom.Value * elapsedTime;
            if (m_InputObj.IsDown(Keys.Down))
                m_Position.Value += Vector3.UnitY * KeyboardSpeed * m_Zoom.Value * elapsedTime;
            if (m_InputObj.IsDown(Keys.Left))
                m_Position.Value -= Vector3.UnitX * KeyboardSpeed * m_Zoom.Value * elapsedTime;
            if (m_InputObj.IsDown(Keys.Right))
                m_Position.Value += Vector3.UnitX * KeyboardSpeed * m_Zoom.Value * elapsedTime;
            if (m_InputObj.IsDown(Keys.PageUp))
            {
                if (m_Zoom.Value - .01f * KeyboardSpeed * elapsedTime <= maxZoom)
                    m_Zoom.Value = GlobalProperties.Instance.GetProperty<float>("MaxZoom").Value;
                else
                    m_Zoom.Value -= .01f * KeyboardSpeed * elapsedTime;
            }
            if (m_InputObj.IsDown(Keys.PageDown))
            {
                if (m_Zoom.Value + .01f * KeyboardSpeed * elapsedTime >= minZoom)
                    m_Zoom.Value = GlobalProperties.Instance.GetProperty<float>("MinZoom").Value;
                else
                    m_Zoom.Value += .01f * KeyboardSpeed * elapsedTime;
            }

            #endregion

            #region Mouse

            lastWheelPosition = currentWheelPosition;
            currentWheelPosition = m_InputObj.WheelPosition;

            int steps = -(int)(currentWheelPosition - lastWheelPosition) / 120;
            if (steps != 0)
            {
                zoomDestinationIndex = Math.Max(0, Math.Min(zoomPoints - 1, zoomDestinationIndex + steps));
                if (!zooming)
                {
                    zooming = true;
                    startZoom = m_Zoom.Value;
                }
            }

            // Begin zoom, capture the anchor point.
            Vector3 anchor_before = Camera.ScreenToWorld(m_InputObj.MousePosition, 0);

            
            if (zooming)
            {
                timerCounter += elapsedTime;
                if (timerCounter > zoomDuration)
                {
                    m_Zoom.Value = zoomLevels[zoomDestinationIndex];
                    timerCounter = 0;
                    zooming = false;
                }
                else
                {
                    m_Zoom.Value = interpolate(timerCounter, startZoom, zoomLevels[zoomDestinationIndex], zoomDuration);
                }
            }

            // Finished zooming this frame, capture the NEW anchor point.
            Vector3 anchor_after = Camera.ScreenToWorld(m_InputObj.MousePosition, 0);

            // Now, get the vector FROM new TO old, and adjust the camera position by that vector.
            Vector3 camera_delta = anchor_before - anchor_after;
            m_Position.Value += camera_delta;
            
//                m_Position.Value = (frame_endZoom / frame_startZoom) * (m_Position.Value - Camera.ScreenToWorld(m_InputObj.MousePosition, 0.0f));
            /*
            if (m_InputObj.MousePosition.X < edgeScrollThreshold)
                m_Position.Value -= Vector3.UnitX * EdgeScrollSpeed * m_Zoom.Value * (elapsedTime / 1000);
            if (m_InputObj.MousePosition.Y < edgeScrollThreshold)
                m_Position.Value -= Vector3.UnitY * EdgeScrollSpeed * m_Zoom.Value * (elapsedTime / 1000);
            if (m_InputObj.MousePosition.X > m_ScreenDimensions.X - edgeScrollThreshold)
                m_Position.Value += Vector3.UnitX * EdgeScrollSpeed * m_Zoom.Value * (elapsedTime / 1000);
            if (m_InputObj.MousePosition.Y > m_ScreenDimensions.Y - edgeScrollThreshold)
                m_Position.Value += Vector3.UnitY * EdgeScrollSpeed * m_Zoom.Value * (elapsedTime / 1000);
            */
            #endregion

            /*
             * float interpolate(float time, float start, float end, float duration) 
             * {
             *      float normalized_time = time / duration;
             *      float time_squared = normalized_time * normalized_time;
             *      float time_cubed = time_squared * normalized_time;
             *      return start + end*(-3.8*time_cubed*time_squared + 9.5*time_squared*time_squared + -9.6*time_cubed + 4.9*time_squared);
             * }
             */
            
        }


        float interpolate(float time, float start, float end, float duration)
        {
            
            //float normalized_time = time / duration;
            //float time_squared = normalized_time * normalized_time;
            //float time_cubed = time_squared * normalized_time;
            //return start + (end-start) * (-3.8f * time_cubed * time_squared + 9.5f * time_squared * time_squared + -9.6f * time_cubed + 4.9f * time_squared);
             
            return MathHelper.SmoothStep(start, end, time / duration);
        }    
    }
}
