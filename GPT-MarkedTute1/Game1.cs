using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RC_Framework;

/*
 * GAME ART
 * road.png - adapted from Alucard - http://opengameart.org/content/2d-top-down-highway-background
 * Car sprites - sujit1717 - http://opengameart.org/content/free-top-down-car-sprites-by-unlucky-studio
 */

namespace GPT_MarkedTute1
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // DEBUG
        bool drawInfo = false;

        // SPRITES
        Sprite3 playerCar;

        // BACKGROUND
        private ScrollBackGround roadBackground;

        // CONTROLS
        KeyboardState keyboardState;

        // ENVIRONMENT
        const int carYSpeed = 3;
        const int carXSpeed = 2;

        Rectangle topShoulder;
        Rectangle bottomShoulder;
        Rectangle disallowedZone;
        int rearEdge = 10;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
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

            // Required for drawing sprite info
            LineBatch.init(GraphicsDevice);

            // ENVIRONMENT
            int shoulderOffset = 95;
            topShoulder = new Rectangle(0, 0, graphics.PreferredBackBufferWidth, shoulderOffset-3);
            bottomShoulder = new Rectangle(0, graphics.PreferredBackBufferHeight - shoulderOffset, graphics.PreferredBackBufferWidth, shoulderOffset);
            disallowedZone = new Rectangle(400, 0, 400, graphics.PreferredBackBufferHeight);

            // BACKGROUND
            Texture2D background = Content.Load<Texture2D>("road.png");
            roadBackground = new ScrollBackGround(background,
                new Rectangle(0, 0, background.Width, background.Height),
                new Rectangle(0, 25, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight - 50),
                -5, // TODO MAKE A CONSTANT?
                2
                );

            // PLAYER CAR
            Texture2D texViper = Content.Load<Texture2D>("car.png");
            playerCar = new Sprite3(true, texViper, 200, 200);
            playerCar.setHSoffset(new Vector2(128, 128));
            playerCar.setBB(25, 85, 210, 85);
            playerCar.setWidthHeight(120, 120);
            playerCar.setMoveSpeed(10);
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
            KeyboardState prevKeyboardState = keyboardState;
            keyboardState = Keyboard.GetState();
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.Escape))
                Exit();

            // DEBUG
            if (keyboardState.IsKeyDown(Keys.B) && prevKeyboardState.IsKeyUp(Keys.B))
            {
                drawInfo = !drawInfo;
            }

            // CAR MOVEMENT

            // Make the movement
            playerCar.setDisplayAngleDegrees(0);
            if (keyboardState.IsKeyDown(Keys.Up))
            {
                // Car Up
                playerCar.setDisplayAngleDegrees(-10);
                playerCar.moveByDeltaY(-carYSpeed);
            }
            else if (keyboardState.IsKeyDown(Keys.Down))
            {
                // Car Down
                playerCar.setDisplayAngleDegrees(10);
                playerCar.setMoveAngleDegrees(90);
                playerCar.moveByDeltaY(carYSpeed);
            }

            if (keyboardState.IsKeyDown(Keys.Left))
            {
                // Car Left
                playerCar.moveByDeltaX(-carXSpeed);
            }
            else if (keyboardState.IsKeyDown(Keys.Right))
            {
                // Car Right
                playerCar.moveByDeltaX(carXSpeed);
            }
            else
            {
                playerCar.moveByDeltaX(-carXSpeed/3f); // Fall back if not being moved forward explicitly
            }

            // Detect collisions with the sides
            if (playerCar.getBoundingBoxAA().Intersects(topShoulder))
            {
                playerCar.moveByDeltaY(Rectangle.Intersect(playerCar.getBoundingBoxAA(), topShoulder).Height);
                // TODO: Take down car hitpoints
            }
            else if (playerCar.getBoundingBoxAA().Intersects(bottomShoulder))
            {
                playerCar.moveByDeltaY(-Rectangle.Intersect(playerCar.getBoundingBoxAA(), bottomShoulder).Height);
                // TODO: Take down car hitpoints
            }

            // Detect being outside the allowed area and move back into the allowed area
            if (playerCar.getBoundingBoxAA().X < rearEdge)
            {
                playerCar.moveByDeltaX(rearEdge - playerCar.getBoundingBoxAA().X);
            }
            else if (playerCar.getBoundingBoxAA().Intersects(disallowedZone))
            {
                playerCar.moveByDeltaX(-Rectangle.Intersect(playerCar.getBoundingBoxAA(), disallowedZone).Width);
            }

            // Update scrolling background
            roadBackground.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Wheat);

            spriteBatch.Begin();

            // Draw the scrolling background
            roadBackground.Draw(spriteBatch);

            // Draw Player Car
            playerCar.Draw(spriteBatch);

            if (drawInfo)
            {
                // CAR
                playerCar.drawInfo(spriteBatch, Color.AliceBlue, Color.Green);
                LineBatch.drawRect4(spriteBatch, playerCar.bbTemp, Color.Blue);

                // ENV
                LineBatch.drawLineRectangle(spriteBatch, topShoulder, Color.BlueViolet);
                LineBatch.drawLineRectangle(spriteBatch, bottomShoulder, Color.BlueViolet);
                LineBatch.drawLineRectangle(spriteBatch, disallowedZone, Color.Fuchsia);
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
