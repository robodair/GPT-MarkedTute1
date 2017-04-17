using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RC_Framework;
namespace GPT_MarkedTute1
{
	public class PauseOverlay : RC_GameStateParent
	{
		Rectangle pauseBackground;
		Vector2 pauseTextPos;
        Vector2 resumeTextPos;

		public PauseOverlay()
		{
		}

		public override void Update(GameTime gameTime)
		{
			getKeyboardAndMouse();
			if (keyState.IsKeyDown(Keys.P) && prevKeyState.IsKeyUp(Keys.P))
			{
				gameStateManager.popLevel();
			}
			base.Update(gameTime);
		}

		public override void InitializeLevel(GraphicsDevice g, SpriteBatch s, Microsoft.Xna.Framework.Content.ContentManager c, RC_GameStateManager lm)
		{
			base.InitializeLevel(g, s, c, lm);

			pauseBackground = new Rectangle(graphicsManager.PreferredBackBufferWidth / 2 - 100,
										   graphicsManager.PreferredBackBufferHeight / 2 - 100,
											200, 100);
			pauseTextPos = new Vector2(graphicsManager.PreferredBackBufferWidth / 2 - 35,
									   graphicsManager.PreferredBackBufferHeight / 2 - 60);
            resumeTextPos = pauseTextPos;
            resumeTextPos.Y += 30;
		}

		public override void Draw(GameTime gameTime)
		{
			spriteBatch.Draw(UtilTexSI.texWhite, pauseBackground, Color.DarkSlateBlue);
            spriteBatch.DrawString(font, "Paused", pauseTextPos, Color.White);
            spriteBatch.DrawString(font, "(P) To Resume", resumeTextPos, Color.White, 0, Vector2.Zero, 0.5f, SpriteEffects.None, 0);
		}
	}
}
