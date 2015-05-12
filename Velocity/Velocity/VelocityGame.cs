using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using Velocity.Objects;
using Velocity.Levels;

namespace Velocity
{
    public class VelocityGame : Microsoft.Xna.Framework.Game
    {
        public GraphicsDeviceManager graphics;
        public SpriteBatch spriteBatch;

		private int startLevelNum = 2;

		public int gameWidth, gameHeight;

		public Level level;

        public VelocityGame()
        {
            graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";

			gameWidth = 800; gameHeight = 480;

			//Base Size
			graphics.PreferredBackBufferWidth = gameWidth;
			graphics.PreferredBackBufferHeight = gameHeight;

			//My Size
			//graphics.PreferredBackBufferWidth = 800;
			//graphics.PreferredBackBufferHeight = 480;
			
			graphics.ApplyChanges();
		}

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
			level = NewLevel(startLevelNum);

            base.Initialize();
        }

		/// <summary>
		/// Used to select and initialize the new level. Defaults to 1.
		/// </summary>
		/// <param name="num">The number of the new level</param>
		/// <returns>Returns the new level</returns>
		protected Level NewLevel(int num)
		{
			Level l;
			int maxLevelNum = 2;

			if (num == 1)
				l = new Level1();
			else if (num == 2)
				l = new Level2();

			else if (num > maxLevelNum)
				l = new Level1();			//First Level
			else if (num <= 0)
				l = new Level2();			//Last Level
			else
				l = null;					//Error, check maxLevelNum

			l.Initialize(this, gameWidth, gameHeight);
			return l;
		}

		/// <summary>
		/// Switches the level, quitting the current one and starting a new one.
		/// </summary>
		/// <param name="num">The number of the new level</param>
		public void SwitchLevel(int num)
		{
			//Quit current level

			level = NewLevel(num);
		}

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
			level.Update(this);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
			level.Draw(this);

            base.Draw(gameTime);
        }
    }
}
