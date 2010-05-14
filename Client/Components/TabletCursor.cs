using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kannon.EntitySystem;
using Microsoft.Xna.Framework;
using Kannon.EntitySystem.Components;
using Kannon.EntitySystem;
using Kannon;
using VBTablet;
using System.Xml;

namespace Kannon.Components
{
    public class TabletCursor : Component
    {
        #region fields
        Property<Vector3> m_Position;

        Tablet tablet;
        #endregion

        #region factory/constructor
        [ComponentCreator]
        public static Component Create(Entity ent, String name)
        {
            return new TabletCursor(ent, name);
        }

        public TabletCursor(Entity ent, String name)
            : base(ent, name)
        {
            m_Position = Entity.AddProperty<Vector3>("Position", Vector3.Zero);

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
        #endregion

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
            float xVal = tablet.TwipsToFloatRangeX(X) * XNAGame.Instance.GraphicsDevice.Viewport.Width;
            float yVal = (1 - tablet.TwipsToFloatRangeY(Y)) * XNAGame.Instance.GraphicsDevice.Viewport.Height;

            m_Position.Value = new Vector3(xVal, yVal, 0);
        }

        public override void Parse(XmlNode data)
        {
        }
    }
}
