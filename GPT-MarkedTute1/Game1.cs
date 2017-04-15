using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using RC_Framework;

/*
 * GAME ART
 * road - adapted from Alucard - http://opengameart.org/content/2d-top-down-highway-background
 * Car sprites - sujit1717 - http://opengameart.org/content/free-top-down-car-sprites-by-unlucky-studio
 * Comic Explosion - Pompei2 - http://opengameart.org/content/comic-explosion-kaboom
 * Crash sound effect - squareal - https://freesound.org/people/squareal/sounds/237375/
 */

namespace GPT_MarkedTute1
{
	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public class Game1 : Game
	{
		int levelCounter = 0;
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;

		RC_GameStateManager levelManager;

		KeyboardState keyboardState;
		KeyboardState prevKeyboardState;

		public Game1()
		{
			graphics = new GraphicsDeviceManager(this);
			RC_GameStateParent.graphicsManager = graphics;
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
			spriteBatch = new SpriteBatch(GraphicsDevice);

			levelManager = new RC_GameStateManager();

			levelManager.AddLevel(levelCounter, new StartSplashScreen());
			levelCounter++;

			levelManager.AddLevel(levelCounter, new PauseOverlay());
			levelCounter++;

			levelManager.AddLevel(levelCounter, new RoadLevel());

			for (int i = 0; i <= levelCounter; i++)
			{
				levelManager.getLevel(i).InitializeLevel(GraphicsDevice, spriteBatch, Content, levelManager);
			}

			levelManager.setLevel(2);
			levelManager.pushLevel(0);

			UtilTexSI.initTextures(GraphicsDevice);

			base.Initialize();
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			RC_GameStateParent.font = Content.Load<SpriteFont>("fonts/SpriteFont");

			for (int i = 0; i <= levelCounter; i++)
			{
				levelManager.getLevel(i).LoadContent();
			}

			base.LoadContent();
		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// game-specific content.
		/// </summary>
		protected override void UnloadContent()
		{
			// TODO: Unload any non ContentManager content here
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			prevKeyboardState = keyboardState;
			keyboardState = Keyboard.GetState();
			if (keyboardState.IsKeyDown(Keys.Escape))
				Exit();

			levelManager.getCurrentLevel().Update(gameTime);

			base.Update(gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			spriteBatch.Begin();
			levelManager.getCurrentLevel().Draw(gameTime);
			spriteBatch.End();

			base.Draw(gameTime);
		}
	}
}
