using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using RC_Framework;

/*
 * GAME ART
 * road.png - adapted from Alucard - http://opengameart.org/content/2d-top-down-highway-background
 * Car sprites - sujit1717 - http://opengameart.org/content/free-top-down-car-sprites-by-unlucky-studio
 * Comic Explosion - Pompei2 - http://opengameart.org/content/comic-explosion-kaboom
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
        SpriteList oncomingCars;
        SpriteList withCars;
        SpriteList explosions;
        SpriteList signs;

        // Lane Changing Cars
        Sprite3 withLaneChangingCar;
        Sprite3 oncomingLaneChangingCar;

        // Car Textures
        Texture2D[] carTextures = new Texture2D[3];
        HealthBar playerHealthBar;

        // Explosion Texture
        Texture2D texExplosion;

        // Sign Texture
        Texture2D texSign;

        // BACKGROUND
        private ScrollBackGround roadBackground;

        // CONTROLS
        KeyboardState keyboardState;

        // ENVIRONMENT
        const int maxPlayerHealth = 1000;
        const int wallCollideDamagePerUpdate = 10;
        const int oncomingCollideDamagePerUpdate = 1000;//= 50;
        const int withCollideDamagePerUpdate = 500;//30;


        const int carYSpeed = 3;
        const int carXSpeed = 2;

        Rectangle carArea;
        Rectangle topShoulder;
        Rectangle bottomShoulder;
        Rectangle disallowedZone;
        int rearEdge = 10;

        bool freeze; // global toggle whether or not the game is frozen

        Random rand;

        int[] withLaneY = { 140, 205 };
        int[] oncomingLaneY = { 273, 336 };

        int[] signLines = { 60, 415 };

        int oncomingSpeed = -6;
        int withSpeed = -2;

        int backgroundScrollSpeed = -5;

        float withCarSpawnInterval = 2500;
        float oncomingCarSpawnInterval = 800;
        float withCarSpawnTimer;
        float oncomingCarSpawnTimer;

        float signSpawnInterval = 3000;
        float signSpawnTimer;

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
            freeze = false;

            rand = new Random();

            // Oncoming Sprite List
            oncomingCars = new SpriteList(20);
            oncomingLaneChangingCar = null;
            // With Sprite List
            withCars = new SpriteList(20);
            withLaneChangingCar = null;
            // Explosions Sprite List
            explosions = new SpriteList(20);
            // Signs Sprite List
            signs = new SpriteList(8);

            // TIMERS
            withCarSpawnTimer = withCarSpawnInterval;
            oncomingCarSpawnTimer = oncomingCarSpawnInterval;
            signSpawnTimer = signSpawnInterval;

            // ENVIRONMENT
            int shoulderOffset = 95;
            topShoulder = new Rectangle(0, 0, graphics.PreferredBackBufferWidth, shoulderOffset - 3);
            bottomShoulder = new Rectangle(0, graphics.PreferredBackBufferHeight - shoulderOffset, graphics.PreferredBackBufferWidth, shoulderOffset);
            disallowedZone = new Rectangle(400, 0, 400, graphics.PreferredBackBufferHeight);
            carArea = new Rectangle(0, 0, graphics.PreferredBackBufferWidth + 200, graphics.PreferredBackBufferHeight);

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

            // BACKGROUND
            Texture2D background = Content.Load<Texture2D>("road.png");
            roadBackground = new ScrollBackGround(background,
                new Rectangle(0, 0, background.Width, background.Height),
                new Rectangle(0, 25, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight - 50),
                backgroundScrollSpeed,
                2
                );

            // PLAYER CAR
            Texture2D texCar = Content.Load<Texture2D>("car.png");
            playerCar = new Sprite3(true, texCar, 200, 200);
            playerCar.setHSoffset(new Vector2(128, 128));
            playerCar.setBB(25, 85, 210, 85);
            playerCar.setWidthHeight(120, 120);
            playerCar.hitPoints = maxPlayerHealth;
            playerCar.maxHitPoints = maxPlayerHealth;

            // HealthBar
            playerHealthBar = new HealthBar(Color.Green, Color.Black, Color.Red, 5, true);
            playerHealthBar.parent = playerCar;
            playerHealthBar.offset = new Vector2(0, -12);

            // OTHER CARS
            carTextures[0] = Content.Load<Texture2D>("minivan.png");
            carTextures[1] = Content.Load<Texture2D>("ute.png");
            carTextures[2] = Content.Load<Texture2D>("taxi.png");

            // EXPLOSION
            texExplosion = Content.Load<Texture2D>("explosion.png");

            // SIGNS
            texSign = Content.Load<Texture2D>("sign.png");

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

            if (keyboardState.IsKeyDown(Keys.S) && prevKeyboardState.IsKeyUp(Keys.S))
            {
                createNewCar(false);
                createSign();
            }

            // RESTART
            if (keyboardState.IsKeyDown(Keys.R) && prevKeyboardState.IsKeyUp(Keys.R))
            {
                Initialize();
            }

            if (freeze)
            {
                return;
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
                Rectangle collisionRect = Rectangle.Intersect(playerCar.getBoundingBoxAA(), topShoulder);
                playerCar.moveByDeltaY(collisionRect.Height);
                playerCar.hitPoints -= wallCollideDamagePerUpdate;
                if (playerCar.hitPoints <= 0)
                {
                    createExplosion(new Point(collisionRect.Right, collisionRect.Center.Y));
                }
            }
            else if (playerCar.getBoundingBoxAA().Intersects(bottomShoulder))
            {
                Rectangle collisionRect = Rectangle.Intersect(playerCar.getBoundingBoxAA(), bottomShoulder);
                playerCar.moveByDeltaY(-collisionRect.Height);
                playerCar.hitPoints -= wallCollideDamagePerUpdate;
                if (playerCar.hitPoints <= 0)
                {
                    createExplosion(collisionRect.Center);
                }
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

            // Move the other cars
            oncomingCars.moveDeltaXY();
            oncomingCars.removeIfOutside(carArea);
            withCars.moveDeltaXY();
            withCars.removeIfOutside(carArea);

            changeCarLane(ref oncomingLaneChangingCar, oncomingLaneY, ref oncomingCars);
            changeCarLane(ref withLaneChangingCar, withLaneY, ref withCars);

            // Detect Collisions with cars
            int oncomingCollision = oncomingCars.collisionAA(playerCar);
            int withCollision = withCars.collisionAA(playerCar);

            if (oncomingCollision != -1)
            {
                if (Util.collisionRect4Rect4Points(oncomingCars[oncomingCollision].bbTemp, playerCar.bbTemp))
                {
                    Rectangle collisionRect = oncomingCars[oncomingCollision].collisionRect(playerCar);
                    playerCar.hitPoints -= oncomingCollideDamagePerUpdate;
                    if (playerCar.hitPoints <= 0)
                    {
                        createExplosion(collisionRect.Center);
                    }
                }
            }

            if (withCollision != -1)
            {
                if (Util.collisionRect4Rect4Points(withCars[withCollision].bbTemp, playerCar.bbTemp))
                {
                    Rectangle collisionRect = withCars[withCollision].collisionRect(playerCar);
                    playerCar.hitPoints -= withCollideDamagePerUpdate;
                    if (playerCar.hitPoints <= 0)
                    {
                        createExplosion(collisionRect.Center);
                    }
                }
            }

            // Freeze Game if Player HP get to 0
            if (playerCar.hitPoints <= 0)
            {
                playerCar.hitPoints = 0;
                freeze = true;
            }
            else
            {
                playerCar.hitPoints++;
                if (playerCar.hitPoints > maxPlayerHealth)
                {
                    playerCar.hitPoints = maxPlayerHealth;
                }
            }

            // Update scrolling background
            roadBackground.Update(gameTime);

            // Move signs
            signs.Update(gameTime);
            signs.moveDeltaXY();
            signs.removeIfOutside(carArea);

            // Randomly create new cars and signs
            withCarSpawnTimer -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (withCarSpawnTimer < 0)
            {
                createNewCar(false);
                withCarSpawnTimer = withCarSpawnInterval;
            }

            oncomingCarSpawnTimer -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (oncomingCarSpawnTimer < 0)
            {
                createNewCar(true);
                oncomingCarSpawnTimer = oncomingCarSpawnInterval;
            }

            signSpawnTimer -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (signSpawnTimer < 0)
            {
                createSign();
                signSpawnTimer = signSpawnInterval;
            }

            base.Update(gameTime);
        }

        private void changeCarLane(ref Sprite3 car, int[] lanes, ref SpriteList cars)
        {
            const int UPPER_LANE = 0;
            const int LOWER_LANE = 1;

            if (car == null && cars.count() > 0)
            {
                // Select a random car to do lane changing with
                int i_car = rand.Next(0, cars.count());
                if (i_car != -1) // Make sure the car exists
                {
                    car = cars[i_car];
                    int carX = (int) car.getPosX();
                    Console.WriteLine("Testing" + carX + ">" + 400);
                    Console.WriteLine(carX > 400);

                    if (carX > 400)
                    // Only lane change this car if it's in the 'disallowed zone' so that it doesn't run into the player
                    {
                        Console.WriteLine("IF");
                        Console.WriteLine("Car at" + car.getPos());
                        int angleDirection = car.getDeltaSpeed().X == oncomingSpeed ? 1 : -1; // Invert angles for the with cars

                        if (car.getPosY() > lanes[UPPER_LANE] + 10) // if car is in the UPPER_LANE
                        {
                            // turn to LOWER_LANE
                            car.setDisplayAngleOffsetDegrees(10 * angleDirection);
                            car.setDeltaSpeed(
                                car.getDeltaSpeed() + new Vector2(0, -1)
                                );
                        }
                        else // car is in LOWER_LANE
                        {
                            // Turn to UPPER_LANE
                            car.setDisplayAngleOffsetDegrees(-10 * angleDirection);
                            car.setDeltaSpeed(
                                car.getDeltaSpeed() + new Vector2(0, 1)
                                );
                        }
                    }
                }

            }
            else if (car != null)
            {
                if (car.getPosX() < 0)
                {
                    car = null;
                }
                // If the car has reached either lane, force it's position to be correct
                else if (car.getDeltaSpeed().Y > 0) // if we're moving down
                {
                    if (car.getPosY() >= lanes[LOWER_LANE]) // if we've reached our target
                    {
                        // stop the rotation and up movement
                        car.setPosY(lanes[LOWER_LANE]);
                        car.setDeltaSpeed(
                            new Vector2(car.getDeltaSpeed().X, 0)
                            );
                        car.setDisplayAngleOffsetDegrees(0);
                        car = null;
                    }
                }
                else // else we're moving up
                {
                    if (car.getPosY() <= lanes[UPPER_LANE]) // if we've reached our target
                    {
                        // stop the rotation and down movement
                        car.setPosY(lanes[UPPER_LANE]);
                        car.setDeltaSpeed(
                            new Vector2(car.getDeltaSpeed().X, 0)
                            );
                        car.setDisplayAngleOffsetDegrees(0);
                        car = null;
                    }
                }
            }
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

            // Draw Cars
            oncomingCars.Draw(spriteBatch);
            withCars.Draw(spriteBatch);

            // Draw Player Car
            playerCar.Draw(spriteBatch);
            playerHealthBar.Draw(spriteBatch);

            // Draw Signs
            signs.Draw(spriteBatch);

            // Draw Explosions
            explosions.Draw(spriteBatch);

            if (drawInfo)
            {
                // CAR
                playerCar.drawInfo(spriteBatch, Color.AliceBlue, Color.Green);
                LineBatch.drawRect4(spriteBatch, playerCar.bbTemp, Color.Blue);

                // ENV
                LineBatch.drawLineRectangle(spriteBatch, topShoulder, Color.BlueViolet);
                LineBatch.drawLineRectangle(spriteBatch, bottomShoulder, Color.BlueViolet);
                LineBatch.drawLineRectangle(spriteBatch, disallowedZone, Color.Fuchsia);
                LineBatch.drawLineRectangle(spriteBatch, carArea, Color.DarkSalmon);

                foreach (int y in oncomingLaneY)
                {
                    LineBatch.drawLine(spriteBatch, 0f, (float)y, (float)graphics.PreferredBackBufferWidth, (float)y, Color.Aqua);
                }
                foreach (int y in withLaneY)
                {
                    LineBatch.drawLine(spriteBatch, 0f, (float)y, (float)graphics.PreferredBackBufferWidth, (float)y, Color.Aqua);
                }
                foreach (int y in signLines)
                {
                    LineBatch.drawLine(spriteBatch, 0f, (float)y, (float)graphics.PreferredBackBufferWidth, (float)y, Color.MediumAquamarine);
                }

                oncomingCars.drawInfo(spriteBatch, Color.Turquoise, Color.Goldenrod);
                withCars.drawInfo(spriteBatch, Color.Turquoise, Color.Goldenrod);
                oncomingCars.drawRect4(spriteBatch, Color.HotPink);
                withCars.drawRect4(spriteBatch, Color.HotPink);

                explosions.drawInfo(spriteBatch, Color.MidnightBlue, Color.Black);
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }

        void createNewCar(bool oncoming)
        {
            int i_tex = rand.Next(0, carTextures.Length);
            int i_lane = rand.Next(0, 2);
            int i_oncoming;
             
            if (oncoming)
            {
                i_oncoming = 1;
            }
            else
            {
                i_oncoming = 0;
            }

            Sprite3 s;

            if (i_oncoming == 1)
            {
                s = new Sprite3(true, carTextures[i_tex], graphics.PreferredBackBufferWidth + 200, oncomingLaneY[i_lane]);
                s.setDisplayAngleDegrees(180);
                s.setDeltaSpeed(new Vector2(oncomingSpeed, 0));
                oncomingCars.addSpriteReuse(s);
            }
            else
            {
                s = new Sprite3(true, carTextures[i_tex], graphics.PreferredBackBufferWidth + 200, withLaneY[i_lane]);
                s.setDeltaSpeed(new Vector2(withSpeed, 0));
                withCars.addSpriteReuse(s);
            }

            s.setHSoffset(new Vector2(carTextures[i_tex].Width / 2, carTextures[i_tex].Height / 2));
            s.setWidthHeight(120, 120);

            // Per texture customisations
            switch (i_tex)
            {
                case 0:
                    // minivan
                    s.setBB(35, 80, 195, 80);
                    break;
                case 1:
                    // ute
                    s.setBB(19, 75, 204, 85);
                    break;
                case 2:
                    // taxi
                    s.setBB(23, 77, 219, 90);
                    break;
                default:
                    break;
            }

        }

        void createExplosion(Point point)
        {
            float scale = 0.5f;
            Sprite3 s = new Sprite3(true, texExplosion, point.X, point.Y);
            s.setWidthHeight(256*scale, 256*scale);
            s.setHSoffset(new Vector2(128, 128));
            explosions.addSpriteReuse(s);
        }

        void createSign()
        {
            int y = signLines[rand.Next(0, 2)];
            float scale = 1f;
            Sprite3 sign = new Sprite3(true, texSign, graphics.PreferredBackBufferWidth + 200, y);
            sign.setHSoffset(new Vector2(texSign.Width, texSign.Height));
            sign.setWidthHeight(58 * scale, 55 * scale);
            sign.setDeltaSpeed(new Vector2(backgroundScrollSpeed+1.5f, 0));
            signs.addSpriteReuse(sign);
        }
    }
}
