#region Using Statements
using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

#endregion

namespace EmeraldsAndElements.Shared
{
	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public class GameMain : Game
	{
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;

		#region Properties

		#endregion

		#region Fields
		SpriteFont spritefont;
		#endregion

		#region Events/Delegates

		#endregion


		public GameMain()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			graphics.PreferredBackBufferHeight = 1000;
			graphics.PreferredBackBufferWidth = 1000;
			graphics.IsFullScreen = false; // set true to default later
			Content.RootDirectory = "Content";
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
		{
			// TODO: Add your initialization logic here
			base.Initialize();
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch(GraphicsDevice);

			//  Load our spritefont file
			spritefont = Content.Load<SpriteFont>("Font/Font");

		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			// For Mobile devices, this logic will close the Game when the Back button is pressed
			// Exit() is obsolete on iOS
#if !__IOS__ && !__TVOS__
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
					Keyboard.GetState().IsKeyDown(Keys.Escape))
			{
				Exit();
			}
#endif
			// TODO: Add your update logic here			
			base.Update(gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.Black);

			//TODO: Add your drawing code here

			base.Draw(gameTime);
		}
	}
}
