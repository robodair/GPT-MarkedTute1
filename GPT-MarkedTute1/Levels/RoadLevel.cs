using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;

using RC_Framework;

namespace GPT_MarkedTute1
{
    public class RoadLevel : RC_GameStateParent
    {
        // DEBUG
        bool drawInfo = false;

        // SPRITES
        Sprite3 playerCar;
        SpriteList oncomingCars;
        SpriteList withCars;
        SpriteList explosions;
        SpriteList roadside;

        // Lane Changing Cars
        Sprite3 withLaneChangingCar;
        Sprite3 oncomingLaneChangingCar;

        // Powerups
        SpriteList powerups;
        const int withPowerupY = 172;
        const int oncomingPowerupY = 310;

        // Car Textures
        Texture2D[] carTextures = new Texture2D[4];
        HealthBar playerHealthBar;

        // Powerup Textures
        Texture2D healthTexture;
        Texture2D moneyTexture;

        // Explosion Texture
        Texture2D texExplosion;

        // Sign Texture
        Texture2D texSign;

        // Snake Texture
        Texture2D texSnake;

        // BACKGROUND
        private ScrollBackGround roadBackground;

        // ENVIRONMENT
        String distString = "DISTANCE: {0:F2} km";
        String moneyString = "MONEY: ${0:F2}";
        double distance;
        double money;
        String message;

        // Health metrics
        const int maxPlayerHealth = 1000;
        const int wallCollideDamagePerUpdate = 10;
        const int oncomingCollideDamagePerUpdate = 100;
        const int withCollideDamagePerUpdate = 80;


        const int carYSpeed = 4;
        const int carXSpeed = 3;

        Rectangle carArea;
        Rectangle topShoulder;
        Rectangle bottomShoulder;
        Rectangle disallowedZone;
        int rearEdge = 10;

        bool freeze; // global toggle whether or not the game is frozen
        bool willPause; // level is paused

        Random rand;

        int[] withLaneY = { 140, 205 };
        int[] oncomingLaneY = { 273, 336 };

        int[] roadsideLines = { 60, 415 };

        int oncomingSpeed = -6;
        int withSpeed = -2;

        int backgroundScrollSpeed = -5;

        float withCarSpawnInterval = 2500;
        float oncomingCarSpawnInterval = 800;
        float withCarSpawnTimer;
        float oncomingCarSpawnTimer;

        float signSpawnInterval = 3000;
        float signSpawnTimer;

        float withPowerupSpawnInterval = 1250 * 9;
        float withPowerupSpawnTimer;
        float oncomingPowerupSpawnInterval = 400 * 5;
        float oncomingPowerupSpawnTimer;

        LimitSound crashSound;
        LimitSound repairSound;
        LimitSound moneySound;

        public RoadLevel() { }

        public override void EnterLevel(int fromLevelNum)
        {
            freeze = false;
            willPause = false;
            base.EnterLevel(fromLevelNum);
        }

        public override void ExitLevel()
        {
            freeze = true;
            base.ExitLevel();
        }

        public override void InitializeLevel(GraphicsDevice g, SpriteBatch s, ContentManager c,
                                             RC_GameStateManager lm)
        {
            base.InitializeLevel(g, s, c, lm);

            // ENVIRONMENT
            int shoulderOffset = 95;
            topShoulder = new Rectangle(0, 0, graphicsManager.PreferredBackBufferWidth, shoulderOffset - 3);
            bottomShoulder = new Rectangle(0, graphicsManager.PreferredBackBufferHeight - shoulderOffset,
                                           graphicsManager.PreferredBackBufferWidth, shoulderOffset);
            disallowedZone = new Rectangle(400, 0, 400, graphicsManager.PreferredBackBufferHeight);
            carArea = new Rectangle(0, 0, graphicsManager.PreferredBackBufferWidth + 200,
                                    graphicsManager.PreferredBackBufferHeight);
        }

        public override void LoadContent()
        {

            // Required for drawing sprite info
            LineBatch.init(graphicsDevice);

            // BACKGROUND
            Texture2D background = Content.Load<Texture2D>("images/road");
            roadBackground = new ScrollBackGround(
                            background,
                            new Rectangle(0, 0, background.Width, background.Height),
                            new Rectangle(0, 25, graphicsManager.PreferredBackBufferWidth,
                                          graphicsManager.PreferredBackBufferHeight - 50),
                            backgroundScrollSpeed,
                            2
                            );

            // PLAYER CAR
            Texture2D texCar = Content.Load<Texture2D>("images/car");
            playerCar = new Sprite3(true, texCar, 200, 200);
            playerCar.setHSoffset(new Vector2(128, 128));
            playerCar.setBB(25, 85, 210, 85);
            playerCar.setWidthHeight(120, 120);

            // HealthBar
            playerHealthBar = new HealthBar(Color.Green, Color.Black, Color.Red, 5, true);
            playerHealthBar.parent = playerCar;
            playerHealthBar.offset = new Vector2(0, -12);

            // Powerups
            healthTexture = Content.Load<Texture2D>("images/repairup");
            moneyTexture = Content.Load<Texture2D>("images/moneyup");

            // OTHER CARS
            carTextures[0] = Content.Load<Texture2D>("images/minivan");
            carTextures[1] = Content.Load<Texture2D>("images/ute");
            carTextures[2] = Content.Load<Texture2D>("images/taxi");
            carTextures[3] = Content.Load<Texture2D>("images/police");

            // EXPLOSION
            texExplosion = Content.Load<Texture2D>("images/explosion");

            // SIGNS
            texSign = Content.Load<Texture2D>("images/sign");

            // SNAKE
            texSnake = Content.Load<Texture2D>("images/snake");

            // Sound
            crashSound = new LimitSound(Content.Load<SoundEffect>("sfx/carcrash_16"), 1);
            repairSound = new LimitSound(Content.Load<SoundEffect>("sfx/socket_wrench_16"), 1);
            moneySound = new LimitSound(Content.Load<SoundEffect>("sfx/cha_ching"), 1);

            Reset();

            base.LoadContent();
        }

        public void Reset()
        {
            freeze = false;
            willPause = false;

            rand = new Random();

            // Oncoming Sprite List
            oncomingCars = new SpriteList(20);
            oncomingLaneChangingCar = null;
            // With Sprite List
            withCars = new SpriteList(20);
            withLaneChangingCar = null;
            // Powerups Sprite List
            powerups = new SpriteList(20);
            // Explosions Sprite List
            explosions = new SpriteList(20);
            // Signs Sprite List
            roadside = new SpriteList(8);

            // TIMERS
            withCarSpawnTimer = withCarSpawnInterval;
            oncomingCarSpawnTimer = oncomingCarSpawnInterval;
            signSpawnTimer = signSpawnInterval;
            withPowerupSpawnTimer = withPowerupSpawnInterval;
            oncomingPowerupSpawnTimer = oncomingPowerupSpawnInterval;

            // Player
            playerCar.hitPoints = maxPlayerHealth;
            playerCar.maxHitPoints = maxPlayerHealth;
            playerCar.setPos(200, 200);

            // Scoring
            distance = 0;
            money = 0;
            message = String.Format(distString, distance);

            // Stop Sound
            crashSound.StopAll();
            repairSound.StopAll();
            moneySound.StopAll();
        }

        public override void Update(GameTime gameTime)
        {
            getKeyboardAndMouse();

            crashSound.Update(gameTime);

            if (willPause) // if the will pause flag is true show pause screen
            {
                gameStateManager.pushLevel(1);
            }

            if (keyState.IsKeyDown(Keys.P) && prevKeyState.IsKeyUp(Keys.P))
            {
                if (playerCar.hitPoints > 0) // may only pause when HP > 0
                {
                    willPause = true; // set the will pause flag so we pause on the next update call
                    freeze = true;
                }
            }

            // DEBUG
            if (keyState.IsKeyDown(Keys.B) && prevKeyState.IsKeyUp(Keys.B))
            {
                drawInfo = !drawInfo;
            }

            // RESTART
            if (keyState.IsKeyDown(Keys.R) && prevKeyState.IsKeyUp(Keys.R))
            {
                this.InitializeLevel(graphicsDevice, spriteBatch, Content, gameStateManager);
                this.Reset();
            }

            if (freeze)
            {
                return;
            }

            // ===== PLAYER MOVEMENT =====

            // Make the movement
            playerCar.setDisplayAngleDegrees(0);
            if (keyState.IsKeyDown(Keys.Up))
            {
                // Car Up
                playerCar.setDisplayAngleDegrees(-10);
                playerCar.moveByDeltaY(-carYSpeed);
            }
            else if (keyState.IsKeyDown(Keys.Down))
            {
                // Car Down
                playerCar.setDisplayAngleDegrees(10);
                playerCar.moveByDeltaY(carYSpeed);
            }

            if (keyState.IsKeyDown(Keys.Left))
            {
                // Car Left
                playerCar.moveByDeltaX(-carXSpeed);
            }
            else if (keyState.IsKeyDown(Keys.Right))
            {
                // Car Right
                playerCar.moveByDeltaX(carXSpeed);
            }
            else
            {
                playerCar.moveByDeltaX(-carXSpeed / 3f); // Fall back if not being moved forward explicitly
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

            // keep track of distance travelled
            distance += 0.0005;
            message = String.Format(distString, distance);

            // ===== OTHER CARS =====
            oncomingCars.moveDeltaXY();
            oncomingCars.removeIfOutside(carArea);
            oncomingCars.animationTick();
            withCars.moveDeltaXY();
            withCars.removeIfOutside(carArea);
            withCars.animationTick();

            // ===== POWERUPS =====
            powerups.moveDeltaXY();
            powerups.removeIfOutside(carArea);


            changeCarLane(ref oncomingLaneChangingCar, oncomingLaneY, ref oncomingCars);

            changeCarLane(ref withLaneChangingCar, withLaneY, ref withCars);

            // ===== COLLISIONS =====
            int oncomingCollision = oncomingCars.collisionAA(playerCar);
            int withCollision = withCars.collisionAA(playerCar);
            int powerupCollision = powerups.collisionAA(playerCar);

            if (oncomingCollision != -1)
            {
                if (Util.collisionRect4Rect4Points(oncomingCars[oncomingCollision].bbTemp, playerCar.bbTemp))
                {
                    // TODO check object name it might be a powerup!
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
                    // TODO check object name it might be a powerup!
                    Rectangle collisionRect = withCars[withCollision].collisionRect(playerCar);
                    playerCar.hitPoints -= withCollideDamagePerUpdate;
                    if (playerCar.hitPoints <= 0)
                    {

                        createExplosion(collisionRect.Center);
                    }
                }
            }

            if (powerupCollision != -1 && powerups[powerupCollision].collisionOfRectWithCircle(playerCar)) // Check for a circle collision as well!
            {
                if (powerups[powerupCollision].getName() == "health")
                {
                    repairSound.playSoundIfOk();
                    playerCar.hitPoints += 200;
                }
                else if (powerups[powerupCollision].getName() == "money")
                {
                    moneySound.playSoundIfOk();
                    money += 20;
                }
                powerups[powerupCollision].setActive(false);
            }

            // ==== GAME OVER CONDITION ====
            if (playerCar.hitPoints <= 0)
            {
                playerCar.hitPoints = 0;
                freeze = true;
            }
            else
            {
                //playerCar.hitPoints++; // NO more health regen, we have powerups for that
                if (playerCar.hitPoints > maxPlayerHealth)
                {
                    playerCar.hitPoints = maxPlayerHealth;
                }
            }

            // ==== BG ====
            roadBackground.Update(gameTime);

            // ==== SIGNS MOVEMENT ====
            roadside.Update(gameTime);
            roadside.moveDeltaXY();
            roadside.removeIfOutside(carArea);
            roadside.animationTick();

            // ==== SPAWN SPRITES ====
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

                createRoadsideObject();
                signSpawnTimer = signSpawnInterval;
            }

            // Spawn powerups
            withPowerupSpawnTimer -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (withPowerupSpawnTimer < 0)
            {
                if (Math.Abs(withCarSpawnTimer - withCarSpawnInterval) > 2)
                {
                    createNewPowerup(false);
                }
                withPowerupSpawnTimer = withPowerupSpawnInterval;
            }

            oncomingPowerupSpawnTimer -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (oncomingPowerupSpawnTimer < 0)
            {
                if (Math.Abs(oncomingCarSpawnTimer - oncomingCarSpawnInterval) > 2)
                {
                    createNewPowerup(true);
                }
                oncomingPowerupSpawnTimer = oncomingPowerupSpawnInterval;
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
                    int carX = (int)car.getPosX();

                    if (carX > 400)
                    // Only lane change this car if it's in the 'disallowed zone' so that it doesn't run into the player
                    {
                        int angleDirection = Math.Abs(car.getDeltaSpeed().X - oncomingSpeed) < 1 ? 1 : -1; // Invert 
                                                                                                           //angles for the with cars

                        if (car.getPosY() > lanes[UPPER_LANE] + 10) // if car is in the UPPER_LANE
                        {
                            // turn to LOWER_LANE
                            car.setDisplayAngleDegrees(10 * angleDirection);
                            car.setDeltaSpeed(
                                car.getDeltaSpeed() + new Vector2(0, -1)
                                );
                        }
                        else // car is in LOWER_LANE
                        {
                            // Turn to UPPER_LANE
                            car.setDisplayAngleDegrees(-10 * angleDirection);
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
                        car.setDisplayAngleDegrees(0);
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
                        car.setDisplayAngleDegrees(0);
                        car = null;
                    }
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            graphicsDevice.Clear(Color.Wheat);

            // Draw the scrolling background
            roadBackground.Draw(spriteBatch);

            // Draw Cars
            oncomingCars.Draw(spriteBatch);
            withCars.Draw(spriteBatch);

            // Draw Player Car
            playerCar.Draw(spriteBatch);
            playerHealthBar.Draw(spriteBatch);

            // Draw Signs
            roadside.Draw(spriteBatch);

            // Draw Powerups
            powerups.Draw(spriteBatch);

            // Draw Explosions
            explosions.Draw(spriteBatch);


            // UI Text
            spriteBatch.DrawString(font, message, new Vector2(3, 3), Color.Chocolate);
            spriteBatch.DrawString(font, String.Format(moneyString, money), new Vector2(500, 3), Color.Chocolate);
            spriteBatch.DrawString(font, "2017 - A. Robertson - (B) Sprite Info - (P) Pause - (Arrows) Move - (R) Restart",
                                   new Vector2(3, graphicsManager.PreferredBackBufferHeight - 23), Color.DarkGoldenrod);

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
                    LineBatch.drawLine(spriteBatch, 0f, (float)y, (float)graphicsManager.PreferredBackBufferWidth,
                                       (float)y, Color.Aqua);
                }
                foreach (int y in withLaneY)
                {
                    LineBatch.drawLine(spriteBatch, 0f, (float)y, (float)graphicsManager.PreferredBackBufferWidth,
                                       (float)y, Color.Aqua);
                }
                foreach (int y in roadsideLines)
                {
                    LineBatch.drawLine(spriteBatch, 0f, (float)y, (float)graphicsManager.PreferredBackBufferWidth,
                                       (float)y, Color.MediumAquamarine);
                }

                oncomingCars.drawInfo(spriteBatch, Color.Turquoise, Color.Goldenrod);
                withCars.drawInfo(spriteBatch, Color.Turquoise, Color.Goldenrod);
                oncomingCars.drawRect4(spriteBatch, Color.HotPink);
                withCars.drawRect4(spriteBatch, Color.HotPink);

                explosions.drawInfo(spriteBatch, Color.MidnightBlue, Color.Black);

                powerups.drawInfo(spriteBatch, Color.Turquoise, Color.Goldenrod);
                powerups.drawBoundingSphere(spriteBatch, Color.MediumSpringGreen);
            }
        }

        void createNewPowerup(bool oncoming)
        {
            Texture2D tex;
            String name;
            switch (rand.Next(0, 2))
            { // 0 is health 1 is money
                case 0: // Health powerup
                    tex = healthTexture;
                    name = "health";
                    break;
                default: // Money powerup
                    tex = moneyTexture;
                    name = "money";
                    break;
            }
            int yPos = withPowerupY;
            int speed = withSpeed;
            if (oncoming)
            {
                yPos = oncomingPowerupY;
                speed = oncomingSpeed;
            }
            Sprite3 s = new Sprite3(true, tex, name, graphicsManager.PreferredBackBufferWidth + 200, yPos);
            s.setHSoffset(new Vector2(tex.Width / 2, tex.Height / 2));
            s.setWidthHeight(50, 50);
            s.setDeltaSpeed(new Vector2(speed, 0));
            s.boundingSphereRadius = 25;
            powerups.addSpriteReuse(s);
        }

        void createNewCar(bool oncoming)
        {
            const int policeTextureID = 3;
            int i_tex = rand.Next(0, carTextures.Length);
            int i_lane = rand.Next(0, 2);

            Sprite3 s;

            if (oncoming)
            {
                s = new Sprite3(true, carTextures[i_tex], graphicsManager.PreferredBackBufferWidth + 200,
                                oncomingLaneY[i_lane]);
                s.setDisplayAngleOffsetDegrees(180);
                s.setDeltaSpeed(new Vector2(oncomingSpeed, 0));
                oncomingCars.addSpriteReuse(s);
            }
            else
            {
                s = new Sprite3(true, carTextures[i_tex], graphicsManager.PreferredBackBufferWidth + 200,
                                withLaneY[i_lane]);
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
                case policeTextureID:
                    // Spec the police car
                    s.setBB(77, 23, 90, 219);
                    s.setXframes(4);
                    s.setYframes(1);
                    if (oncoming)
                    {
                        s.setDisplayAngleOffsetDegrees(270);
                    }
                    else
                    {
                        s.setDisplayAngleOffsetDegrees(90);
                    }
                    s.setHSoffset(new Vector2(256 / 2, 256 / 2));
                    if (rand.Next(0, 4) <= 1) // Only some police cars will have lights flashing
                    {
                        Vector2[] sequence;
                        switch (rand.Next(0, 2)) // Different flash sequences the cars can have
                        {
                            case 0:
                                sequence = new Vector2[] {
                                    new Vector2(0, 0),
                                    new Vector2(1, 0),
                                    new Vector2(3, 0),
                                    new Vector2(2, 0)
                                };
                                break;
                            default:
                                sequence = new Vector2[] {
                                    new Vector2(0, 0),
                                    new Vector2(3, 0),
                                    new Vector2(1, 0),
                                    new Vector2(2, 0)
                                };
                                break;
                        }
                        s.setAnimationSequence(
                            sequence,
                            1,
                            rand.Next(2, 4), // potentially add a bit of a break in there with the "yellow" flash
                            10);
                        s.animationStart();
                    }
                    break;
                default:
                    break;
            }
        }

        void createExplosion(Point point)
        {
            crashSound.playSoundIfOk();
            float scale = 0.5f;
            Sprite3 s = new Sprite3(true, texExplosion, point.X, point.Y);
            s.setWidthHeight(256 * scale, 256 * scale);
            s.setHSoffset(new Vector2(128, 128));
            explosions.addSpriteReuse(s);
        }

        void createRoadsideObject()
        {
            int y = roadsideLines[rand.Next(0, 2)];

            Sprite3 roadsideObject;

            switch (rand.Next(0, 5))
            {
                case 0: // Snake
                    roadsideObject = new Sprite3(true, texSnake, graphicsManager.PreferredBackBufferWidth + 200, y - 20);
                    roadsideObject.setXframes(10);
                    roadsideObject.setYframes(1);
                    Vector2[] sequence = new Vector2[10];
                    for (int i = 0; i < 10; i++)
                    {
                        sequence[i] = new Vector2(i, 0);
                    }
                    roadsideObject.setAnimationSequence(sequence, 1, 9, 10);
                    roadsideObject.animationStart();
                    roadsideObject.setWidthHeight(35, 35);
                    break;
                default:
                    float scale = 1f;
                    roadsideObject = new Sprite3(true, texSign, graphicsManager.PreferredBackBufferWidth + 200, y);
                    roadsideObject.setHSoffset(new Vector2(texSign.Width, texSign.Height));
                    roadsideObject.setWidthHeight(58 * scale, 55 * scale);
                    break;
            }

            roadsideObject.setDeltaSpeed(new Vector2(backgroundScrollSpeed + 1.5f, 0));
            roadside.addSpriteReuse(roadsideObject);
        }
    }
}
