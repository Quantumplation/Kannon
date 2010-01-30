using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Input;

namespace Kannon.Broadphases
{
    public delegate void KeyEventHandler(Input.KeyData key);
    public delegate void MouseEventHandler(Input.MouseData data);

    /// <summary>
    /// The static Input class provides a common, unified
    /// interface for polling for input updates.  It also
    /// provides an interface for registering callbacks,
    /// which are triggered when the state matches.  A
    /// system may choose to poll for IO state when they
    /// require the data, or register a callback to be
    /// notified without having to query for it.
    /// </summary>
    public class Input : IBroadphase
    {
        #region Structures

        public struct KeyData
        {
            public KeyData(float length, Keys key)
            {
                downFor = length;
                keyCode = key;
            }

            public float downFor;
            public Keys keyCode;
        }

        public struct MouseData
        {
            public MouseData(bool ignore = true)
            {
                mouseButtons = new bool[3];
                downFor = new float[3];
                wheelPosition = 0;
                mouseX = 0;
                mouseY = 0;
            }

            public float mouseX, mouseY;
            public bool[] mouseButtons;
            public float[] downFor;
            public float wheelPosition;
        }

        #endregion

        #region Events

        public event KeyEventHandler KeyPressed;
        public event KeyEventHandler KeyHeld;
        public event KeyEventHandler KeyReleased;

        public event MouseEventHandler ButtonClicked;
        public event MouseEventHandler ButtonHeld;
        public event MouseEventHandler ButtonReleased;
        public event MouseEventHandler ScrollWheelMoved;
        public event MouseEventHandler MouseMoved;

        #endregion

        #region Private Variables

        private Dictionary<Keys, KeyData> m_Keys;
        private MouseData m_Mouse;

        #endregion

        #region Functions

        /// <summary>
        /// Public Constructor
        /// </summary>
        public Input()
        {
            m_Keys = new Dictionary<Keys, KeyData>();
            m_Mouse = new MouseData();

            XNAGame.Instance.UpdateEvent += Do;
        }

        public Boolean IsDown(Keys key)
        {
            if (m_Keys.ContainsKey(key))
                return m_Keys[key].downFor > 0;
            return false;
        }

        /// <summary>
        /// Causes the input to tick, and updates state information.
        /// </summary>
        /// <param name="elapsedTime"></param>
        public void Do(float elapsedTime)
        {
            if (elapsedTime > ExecutionFrequency)
            {
                ExecutingTooSlowly = elapsedTime > 2 * ExecutionFrequency;

                DoKeyboard(elapsedTime);
                //DoMouse(elapsedTime);

            }
            else
                return;
        }

        private void DoKeyboard(float elapsedTime)
        {
            KeyboardState keyState = Keyboard.GetState();
            // Cull the list
            {
                List<Keys> keysToRemove = new List<Keys>();

                foreach (Keys k in m_Keys.Keys)
                {
                    if (!keyState.IsKeyDown(k))
                        keysToRemove.Add(k);
                }

                foreach (Keys k in keysToRemove)
                {
                    m_Keys.Remove(k);
                    if (KeyReleased != null)
                        KeyReleased(m_Keys[k]);
                }
            }
            // Update with keyboard
            {
                foreach (Keys k in keyState.GetPressedKeys())
                {
                    if (m_Keys.ContainsKey(k))
                    {
                        KeyData id = m_Keys[k];
                        id.downFor += (float)elapsedTime;
                        if (id.downFor > HeldThreshold && KeyHeld != null)
                            KeyHeld(id);
                        m_Keys[k] = id;
                    }
                    else
                    {
                        m_Keys.Add(k, new KeyData(0.0f, k));
                        if( KeyPressed != null )
                            KeyPressed(m_Keys[k]);
                    }
                }
            }
        }
        private void DoMouse(float elapsedTime)
        {
            MouseState mouseState = Mouse.GetState();
            // Position
            if (m_Mouse.mouseX != mouseState.X || m_Mouse.mouseY != mouseState.Y)
            {
                m_Mouse.mouseX = mouseState.X;
                m_Mouse.mouseY = mouseState.Y;
                if( MouseMoved != null )
                    MouseMoved(m_Mouse);
            }

            bool triggerPressed = false, triggerHeld = false, triggerReleased = false;

            // Left
            if (!m_Mouse.mouseButtons[0] && mouseState.LeftButton == ButtonState.Pressed)
            {
                triggerPressed = true;
            }
            m_Mouse.mouseButtons[0] = mouseState.LeftButton == ButtonState.Pressed;
            if (!m_Mouse.mouseButtons[0])
            {
                m_Mouse.downFor[0] = 0;
                triggerReleased = true;
            }
            else
            {
                m_Mouse.downFor[0] += (float)elapsedTime;
                if (m_Mouse.downFor[0] > HeldThreshold)
                    triggerHeld = true;
            }

            // Middle
            if (!m_Mouse.mouseButtons[1] && mouseState.MiddleButton == ButtonState.Pressed)
            {
                triggerPressed = true;
            }
            m_Mouse.mouseButtons[1] = mouseState.MiddleButton == ButtonState.Pressed;
            if (!m_Mouse.mouseButtons[0])
            {
                m_Mouse.downFor[1] = 0;
                triggerReleased = true;
            }
            else
            {
                m_Mouse.downFor[1] += (float)elapsedTime;
                if (m_Mouse.downFor[1] > HeldThreshold)
                    triggerHeld = true;
            }

            // Right
            if (!m_Mouse.mouseButtons[2] && mouseState.RightButton == ButtonState.Pressed)
            {
                triggerPressed = true;
            }
            m_Mouse.mouseButtons[2] = mouseState.RightButton == ButtonState.Pressed;
            if (!m_Mouse.mouseButtons[2])
            {
                m_Mouse.downFor[2] = 0;
                triggerReleased = true;
            }
            else
            {
                m_Mouse.downFor[2] += (float)elapsedTime;
                if (m_Mouse.downFor[2] > HeldThreshold)
                    triggerHeld = true;
            }

            if (triggerPressed)
                ButtonClicked(m_Mouse);
            if (triggerHeld)
                ButtonHeld(m_Mouse);
            if (triggerReleased)
                ButtonReleased(m_Mouse);

            // Wheel
            if (m_Mouse.wheelPosition != mouseState.ScrollWheelValue)
            {
                ScrollWheelMoved(m_Mouse);
                m_Mouse.wheelPosition = mouseState.ScrollWheelValue;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Represents the minimum amount of time something must be "pressed" before it's considered
        /// "held"
        /// </summary>
        public float HeldThreshold
        {
            get;
            set;
        }

        /// <summary>
        /// Represents how often the input should be updated.
        /// </summary>
        public float ExecutionFrequency
        {
            get;
            set;
        }

        /// <summary>
        /// Represents whether it is getting too little execution time (less than half as often as it's execution
        /// frequency)
        /// </summary>
        public bool ExecutingTooSlowly
        {
            get;
            protected set;
        }

        #endregion

        public void RegisterComponent(Component c)
        {
            throw new NotImplementedException();
        }

        public void RemoveComponent(Component c)
        {
            throw new NotImplementedException();
        }
    }
}