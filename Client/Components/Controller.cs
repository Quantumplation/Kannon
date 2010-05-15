using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Kannon.EntitySystem;
using Kannon.EntitySystem.Components;
using System.Xml;
using VBTablet;
using System.Diagnostics;

namespace Kannon.Components
{
    /// <summary>
    /// Temporary solution for controlling things with the keyboard.
    /// </summary>
    //class KeyboardController : Component, IGenericComponent
    //{
    //    [ComponentCreator]
    //    public static Component Create(Entity ent, String name)
    //    {
    //        return new KeyboardController(ent, name);
    //    }

    //    Broadphases.Input m_InputObj;

    //    Property<Vector3> m_Position;

    //    public Boolean IsActive
    //    {
    //        get;
    //        set;
    //    }

    //    public float Speed
    //    {
    //        get;
    //        set;
    //    }

    //    public KeyboardController(Entity ent, String name) : base(ent, name)
    //    {
    //        m_InputObj = XNAGame.Instance.GetBroadphase<Broadphases.Input>("Input");
    //        m_Position = Entity.AddProperty<Vector3>("Position", Vector3.Zero);

    //        Entity.AddEvent("SetActive", (o) => this.IsActive = true);
    //        Entity.AddEvent("SetInactive", (o) => this.IsActive = false);

    //        IsActive = true;
    //        Speed = 0.5f;
    //    }

    //    public override void Parse(System.Xml.XmlNode data)
    //    {
    //    }

    //    public void Update(float elapsedTime)
    //    {
    //        if( !IsActive )
    //            return;
    //        if (m_InputObj.IsDown(Keys.Up))
    //            m_Position.Value -= Vector3.UnitY * Speed * elapsedTime;
    //        if (m_InputObj.IsDown(Keys.Down))
    //            m_Position.Value += Vector3.UnitY * Speed * elapsedTime;
    //        if (m_InputObj.IsDown(Keys.Left))
    //            m_Position.Value -= Vector3.UnitX * Speed * elapsedTime;
    //        if (m_InputObj.IsDown(Keys.Right))
    //            m_Position.Value += Vector3.UnitX * Speed * elapsedTime;
    //        if (m_InputObj.IsDown(Keys.PageUp) && Entity.HasProperty<float>("Zoom"))
    //        {
    //            if (Entity.GetProperty<float>("Zoom").Value - .01f * Speed * elapsedTime <= GlobalProperties.Instance.GetProperty<float>("MaxZoom").Value)
    //                Entity.GetProperty<float>("Zoom").Value = GlobalProperties.Instance.GetProperty<float>("MaxZoom").Value;
    //            else
    //                Entity.GetProperty<float>("Zoom").Value -= .01f * Speed * elapsedTime;
    //        }
    //        if (m_InputObj.IsDown(Keys.PageDown) && Entity.HasProperty<float>("Zoom"))
    //        {
    //            if (Entity.GetProperty<float>("Zoom").Value + .01f * Speed * elapsedTime >= GlobalProperties.Instance.GetProperty<float>("MinZoom").Value)
    //                Entity.GetProperty<float>("Zoom").Value = GlobalProperties.Instance.GetProperty<float>("MinZoom").Value;
    //            else
    //                Entity.GetProperty<float>("Zoom").Value += .01f * Speed * elapsedTime;
    //        }
    //        if (m_InputObj.IsDown(Keys.R))
    //            m_Position.Value = new Vector3(-500, -500, m_Position.Value.Z);
                
    //    }
    //}

    /// <summary>
    /// Long term solution for controlling the camera (or objects, if you want)
    /// with the mouse.  Includes "smart zoom", and edge scrolling.
    /// </summary>
    //public class StandardCameraController : Component, IGenericComponent
    //{
    //    [ComponentCreator]
    //    public static Component Create(Entity ent, String name)
    //    {
    //        return new StandardCameraController(ent, name);
    //    }

    //    Broadphases.Input m_InputObj;

    //    Property<Vector3> m_Position;
    //    Property<float> m_Zoom;

    //    Vector2 m_ScreenDimensions;

    //    float minZoom;
    //    float maxZoom;

    //    float lastWheelPosition;
    //    float currentWheelPosition;

    //    int zoomDestinationIndex;
    //    float timerCounter;
    //    float startZoom;
    //    bool zooming = false;

    //    const int zoomPoints = 17;
    //    const float zoomDuration = 450f;
    //    const float edgeScrollThreshold = 100f;

    //    float[] zoomLevels = { 1f, 1.5f, 2f, 3f, 4f, 6f, 8f, 12f, 16f, 24f, 32f, 48f, 64f, 96f, 128f, 192f, 256f };

    //    public Boolean IsActive
    //    {
    //        get;
    //        set;
    //    }

    //    public float KeyboardSpeed
    //    {
    //        get;
    //        set;
    //    }
    //    public float EdgeScrollSpeed
    //    {
    //        get;
    //        set;
    //    }
    //    public float ZoomSpeed
    //    {
    //        get;
    //        set;
    //    }

    //    public StandardCameraController(Entity ent, String name)
    //        : base(ent, name)
    //    {
    //        m_InputObj = XNAGame.Instance.GetBroadphase<Broadphases.Input>("Input");

    //        minZoom = GlobalProperties.Instance.GetProperty<float>("MinZoom").Value;
    //        maxZoom = GlobalProperties.Instance.GetProperty<float>("MaxZoom").Value;

    //        m_Position = Entity.AddProperty<Vector3>("Position", Vector3.Zero);
    //        m_Zoom = Entity.AddProperty<float>("Zoom", (maxZoom + minZoom) / 2);

    //        m_ScreenDimensions = GlobalProperties.Instance.GetProperty<Vector2>("ScreenDimensions").Value;

    //        Entity.AddEvent("SetActive", (o) => this.IsActive = true);
    //        Entity.AddEvent("SetInactive", (o) => this.IsActive = false);

    //        IsActive = true;
    //    }

    //    public override void Parse(System.Xml.XmlNode data)
    //    {
    //        if (data.Attributes["EdgeScroll"] != null)
    //            EdgeScrollSpeed = float.Parse(data.Attributes["EdgeScroll"].Value);
    //        else
    //            EdgeScrollSpeed = 1.0f;

    //        if (data.Attributes["Keyboard"] != null)
    //            KeyboardSpeed = float.Parse(data.Attributes["Keyboard"].Value);
    //        else
    //            KeyboardSpeed = 1.0f;

    //        if (data.Attributes["Zoom"] != null)
    //            ZoomSpeed = float.Parse(data.Attributes["Zoom"].Value);
    //        else
    //            ZoomSpeed = 1.0f;
    //    }

    //    public void Update(float elapsedTime)
    //    {
    //        if( !IsActive )
    //            return;

    //        #region Keyboard

    //        if (m_InputObj.IsDown(Keys.Up))
    //            m_Position.Value -= Vector3.UnitY * KeyboardSpeed * m_Zoom.Value * elapsedTime;
    //        if (m_InputObj.IsDown(Keys.Down))
    //            m_Position.Value += Vector3.UnitY * KeyboardSpeed * m_Zoom.Value * elapsedTime;
    //        if (m_InputObj.IsDown(Keys.Left))
    //            m_Position.Value -= Vector3.UnitX * KeyboardSpeed * m_Zoom.Value * elapsedTime;
    //        if (m_InputObj.IsDown(Keys.Right))
    //            m_Position.Value += Vector3.UnitX * KeyboardSpeed * m_Zoom.Value * elapsedTime;
    //        if (m_InputObj.IsDown(Keys.PageUp))
    //        {
    //            if (m_Zoom.Value - .01f * KeyboardSpeed * elapsedTime <= maxZoom)
    //                m_Zoom.Value = GlobalProperties.Instance.GetProperty<float>("MaxZoom").Value;
    //            else
    //                m_Zoom.Value -= .01f * KeyboardSpeed * elapsedTime;
    //        }
    //        if (m_InputObj.IsDown(Keys.PageDown))
    //        {
    //            if (m_Zoom.Value + .01f * KeyboardSpeed * elapsedTime >= minZoom)
    //                m_Zoom.Value = GlobalProperties.Instance.GetProperty<float>("MinZoom").Value;
    //            else
    //                m_Zoom.Value += .01f * KeyboardSpeed * elapsedTime;
    //        }

    //        #endregion

    //        #region Mouse

    //        lastWheelPosition = currentWheelPosition;
    //        currentWheelPosition = m_InputObj.WheelPosition;

    //        int steps = -(int)(currentWheelPosition - lastWheelPosition) / 120;
    //        if (steps != 0)
    //        {
    //            zoomDestinationIndex = Math.Max(0, Math.Min(zoomPoints - 1, zoomDestinationIndex + steps));
    //            if (!zooming)
    //            {
    //                zooming = true;
    //                startZoom = m_Zoom.Value;
    //            }
    //        }

    //        if (zooming)
    //        {
    //            timerCounter += elapsedTime;
    //            if (timerCounter > zoomDuration)
    //            {
    //                m_Zoom.Value = zoomLevels[zoomDestinationIndex];
    //                timerCounter = 0;
    //                zooming = false;
    //            }
    //            else
    //            {
    //                m_Zoom.Value = interpolate(timerCounter, startZoom, zoomLevels[zoomDestinationIndex], zoomDuration);
    //            }
    //        }

    //        if (m_InputObj.MousePosition.X < edgeScrollThreshold)
    //            m_Position.Value -= Vector3.UnitX * EdgeScrollSpeed * m_Zoom.Value * (elapsedTime / 1000);
    //        if (m_InputObj.MousePosition.Y < edgeScrollThreshold)
    //            m_Position.Value -= Vector3.UnitY * EdgeScrollSpeed * m_Zoom.Value * (elapsedTime / 1000);
    //        if (m_InputObj.MousePosition.X > m_ScreenDimensions.X - edgeScrollThreshold)
    //            m_Position.Value += Vector3.UnitX * EdgeScrollSpeed * m_Zoom.Value * (elapsedTime / 1000);
    //        if (m_InputObj.MousePosition.Y > m_ScreenDimensions.Y - edgeScrollThreshold)
    //            m_Position.Value += Vector3.UnitY * EdgeScrollSpeed * m_Zoom.Value * (elapsedTime / 1000);
    //        #endregion

    //        /*
    //         * float interpolate(float time, float start, float end, float duration) 
    //         * {
    //         *      float normalized_time = time / duration;
    //         *      float time_squared = normalized_time * normalized_time;
    //         *      float time_cubed = time_squared * normalized_time;
    //         *      return start + end*(-3.8*time_cubed*time_squared + 9.5*time_squared*time_squared + -9.6*time_cubed + 4.9*time_squared);
    //         * }
    //         */
    //    }


    //    float interpolate(float time, float start, float end, float duration)
    //    {
    //        float normalized_time = time / duration;
    //        float time_squared = normalized_time * normalized_time;
    //        float time_cubed = time_squared * normalized_time;
    //        return start + (end-start) * (-3.8f * time_cubed * time_squared + 9.5f * time_squared * time_squared + -9.6f * time_cubed + 4.9f * time_squared);
    //    }    
    //}

    /// <summary>
    /// Moves the attached entity around based on user input, either mouse or graphics tablet
    /// </summary>
    public class ScrollController : Component, IGenericComponent
    {
        private IGenericComponent behaviour;

        public bool IsActive
        {
            get;
            set;
        }

        [ComponentCreator]
        public static Component Create(Entity ent, String name)
        {
            return new ScrollController(ent, name);
        }

        public ScrollController(Entity ent, String name)
            : base(ent, name)
        {
            IsActive = true;
            Entity.AddEvent("SetActive", (o) => this.IsActive = true);
            Entity.AddEvent("SetInactive", (o) => this.IsActive = false);
        }

        public override void Parse(XmlNode data)
        {
            string s = "";
            if (data.Attributes != null)
                s = data.Attributes["Device"].InnerText;

            behaviour = CreateBehaviour(s);
        }

        private IGenericComponent CreateBehaviour(string s)
        {
            if (s == "")
                s = "multitouch"; //assume the least likely device, and fall back until we find a device

            switch (s.ToLowerInvariant())
            {
                case "mouse":
                    throw new NotImplementedException("Mouse");
                    break;
                case "keyboard":
                    throw new NotImplementedException("Keyboard");
                    break;
                case "tablet":
                    try { return new TabletBehaviour(Entity.AddProperty<Vector3>("Position", Vector3.Zero)); }
                    catch (Exception e) { return CreateBehaviour("mouse"); }
                case "multitouch":
                    //todo: implement a multi touch scroll controller
                    try { throw new NotImplementedException(); }
                    catch (Exception e) { return CreateBehaviour("tablet"); }
                default:
                    throw new InvalidOperationException("No device specified or detected");
            }
        }

        public void Update(float elapsedTime)
        {
            if (!IsActive)
                return;

            behaviour.Update(elapsedTime);
        }

        private class TabletBehaviour : IGenericComponent
        {
            Tablet tablet;

            private Stopwatch timer = new Stopwatch();

            private int queueLength = -1;
            private Queue<TabletDataPacket> positions = new Queue<TabletDataPacket>();

            private Property<Vector3> position;

            public TabletBehaviour(Property<Vector3> position)
            {
                this.position = position;

                bool isDigitizing = true;
                string selectedContext = "FirstContext";

                tablet = new Tablet();
                tablet.AddContext("FirstContext", ref isDigitizing);
                tablet.SelectContext(ref selectedContext);
                tablet.hWnd = XNAGame.Instance.Window.Handle;
                tablet.Connected = true;
                tablet.Context.Enabled = true;

                tablet.PacketArrival += PacketArrivalEventHandler;
            }

            #region packet event handler
            private void PacketArrivalEventHandler
            (
                ref IntPtr ContextHandle,
                ref int Cursor,
                ref int X,
                ref int Y,
                ref int Z,
                ref int Buttons,
                ref int NormalPressure,
                ref int TangentPressure,
                ref int Azimuth,
                ref int Altitude,
                ref int Twist,
                ref int Pitch,
                ref int Roll,
                ref int Yaw,
                ref int PacketSerial,
                ref int PacketTime
            )
            {
                if (positions.Count == 0)
                    timer.Start();

                positions.Enqueue(new TabletDataPacket()
                {
                    Cursor = Cursor,
                    X = X,
                    Y = Y,
                    Z = Z,
                    Buttons = Buttons,
                    NormalPressure = NormalPressure,
                    TangentPressure = TangentPressure,
                    Azimuth = Azimuth,
                    Altitude = Altitude,
                    Twist = Twist,
                    Pitch = Pitch,
                    Roll = Roll,
                    Yaw = Yaw,
                    PacketSerial = PacketSerial,
                    PacketTime = PacketTime,
                });

                if (timer.IsRunning)
                {
                    //Time for 100ms to see how many packet that is
                    if (timer.ElapsedMilliseconds >= 100)
                    {
                        queueLength = positions.Count;
                        timer.Stop();
                        Trace.TraceInformation("Determined tablet queue length @ " + queueLength);
                    }
                }
                else
                {
                    //keep queue 100 ms long
                    while (positions.Count > queueLength)
                        positions.Dequeue();
                }
            }
            #endregion

            public void Update(float elapsedTime)
            {
                if (positions.Count > 1)
                {
                    TabletDataPacket p1 = positions.Skip(positions.Count - 2).First();
                    TabletDataPacket p2 = positions.Skip(positions.Count - 1).First();

                    float scale = 10;
                    Vector2 delta = new Vector2(tablet.TwipsToFloatRangeX(p2.X - p1.X) * XNAGame.Instance.GraphicsDevice.Viewport.Width, tablet.TwipsToFloatRangeY(p2.Y - p1.Y) * XNAGame.Instance.GraphicsDevice.Viewport.Height);

                    position.Value += new Vector3(delta * new Vector2(-scale, scale), 0);
                }
            }

            private struct TabletDataPacket
            {
                public int Cursor;
                public int X;
                public int Y;
                public int Z;
                public int Buttons;
                public int NormalPressure;
                public int TangentPressure;
                public int Azimuth;
                public int Altitude;
                public int Twist;
                public int Pitch;
                public int Roll;
                public int Yaw;
                public int PacketSerial;
                public int PacketTime;
            }
        }
    }
}
