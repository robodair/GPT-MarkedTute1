using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RC_Framework;

namespace GPT_MarkedTute1
{
	public class StartSplashScreen : RC_GameStateParent
	{
		int counter;
		Vector2 titlePos;
		float titleRot = 0.3f;
		float titleScale = 2;
		Vector2 subtitlePos;


		public StartSplashScreen()
		{
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
			counter++;
			if (counter == 240)
			{
				gameStateManager.popLevel();
			}
		}

		public override void InitializeLevel(Microsoft.Xna.Framework.Graphics.GraphicsDevice g, Microsoft.Xna.Framework.Graphics.SpriteBatch s, Microsoft.Xna.Framework.Content.ContentManager c, RC_GameStateManager lm)
		{
			base.InitializeLevel(g, s, c, lm);
			titlePos = new Vector2(graphicsManager.PreferredBackBufferWidth / 3 * 0.9f,
								   graphicsManager.PreferredBackBufferHeight / 4 * 1.2f);

			subtitlePos = new Vector2(graphicsManager.PreferredBackBufferWidth / 3,
									  graphicsManager.PreferredBackBufferHeight / 3 * 1.5f);


		}

		public override void Draw(GameTime gameTime)
		{
			graphicsDevice.Clear(Color.Black);

			spriteBatch.DrawString(font, "Dangerous Driving", titlePos, Color.Chocolate, 0.3f, Vector2.Zero,
								   titleScale, SpriteEffects.None, titleRot);
			spriteBatch.DrawString(font, "Early Access", subtitlePos, Color.Maroon);

		}

		public override void LoadContent()
		{
			base.LoadContent();
		}
	}
}
