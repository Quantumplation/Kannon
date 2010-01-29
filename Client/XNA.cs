using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace Kannon
{
    public class XNAGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        public GraphicsDeviceManager Graphics
        {
            get { return graphics; }
            set { graphics = value; }
        }

        SpriteBatch spriteBatch;
        public SpriteBatch SpriteBatch
        {
            get { return spriteBatch; }
            set { spriteBatch = value; }
        }

        /// <summary>
        /// Broadphases for the Kannon Game.
        /// </summary>
        Dictionary<String, IBroadphase> m_Broadphases;
        public Dictionary<String, IBroadphase> Broadphases
        {
            get
            {
                return m_Broadphases;
            }
        }

        /// <summary>
        /// Returns the Instance of this Singleton.
        /// </summary>
        /// <seealso cref="Singleton<T>"/>
        private static XNAGame instance;
        public static XNAGame Instance
        {
            get
            {
                // ?? is shorthand for (a == null ? a : b)
                // Therefore, this returns either instance, or, if instance is null, returns new T();
                return instance ?? (instance = new XNAGame());
            }
        }

        public XNAGame()
        {
            if (instance != null)
                return;
            instance = this;
            if (instance == null)
                return;

            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Invoked once all broadphases have been created.
        /// </summary>
        public event Action InitializeEvent;

        /// <summary>
        /// Invoked when content should be loaded.
        /// </summary>
        public event Action LoadEvent;

        /// <summary>
        /// Invoked when things should be updated.
        /// </summary>
        public event Action<float> UpdateEvent;

        /// <summary>
        /// Invoked when the game should render.
        /// </summary>
        public event Action<float> RenderEvent;

        /// <summary>
        /// Invoked when content should be loaded.
        /// </summary>
        public event Action UnloadEvent;

        /// <summary>
        /// Invoked when the game is exiting.
        /// </summary>
        public event Action DestroyEvent;

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            ComponentFactory.RegisterComponentType(typeof(Kannon.Components.StaticRenderable));

            m_Broadphases = new Dictionary<string, IBroadphase>();
            m_Broadphases.Add("Generic", new Broadphases.Generic());
            m_Broadphases.Add("Graphics", new Broadphases.Graphics(this.Content, this.GraphicsDevice));

            /*Entity ent = new Entity();
            ent.AddProperty<float>("Scale", .5f);
            ent.AddProperty<float>("Rotation", (float)Math.PI / 6);
            ent.AddProperty<Vector2>("Origin", new Vector2(16, 16));
            ent.AddProperty<Vector2>("Position", new Vector2(200, 200));
            ent.AddComponent("StaticRenderable");
            */
            System.Collections.Generic.List<Entity> set = EntityFactory.ProduceSet("TestSetA");

            if( InitializeEvent != null )
                InitializeEvent();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            if( LoadEvent != null )
                LoadEvent();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            if (UnloadEvent != null)
                UnloadEvent();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if( UpdateEvent != null )
                UpdateEvent((float)gameTime.ElapsedGameTime.TotalMilliseconds);
            base.Update(gameTime);
        }


        /// <summary>
        /// Called when the game is on the verge of exiting.
        /// </summary>
        protected override void OnExiting(object sender, EventArgs args)
        {
            if( DestroyEvent != null )
                DestroyEvent();
            base.OnExiting(sender, args);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            if (RenderEvent != null)
                RenderEvent((float)gameTime.ElapsedGameTime.TotalMilliseconds);
            base.Draw(gameTime);
        }
    }
}
