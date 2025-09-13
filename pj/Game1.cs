using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;

using System.Collections.Generic;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;

using MonoGame.Extended.Sprites;
using MonoGame.Extended.Serialization;
using MonoGame.Extended.Content;

using MonoGame.Extended.ViewportAdapters;
using System.Security.Principal;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;

namespace pj
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        //sound
        private Song BGM;
        private SoundEffect clickbutt;
        private SoundEffect jumpsound;
        private SoundEffect deadthsound;

        TiledMapObjectLayer _appleObj;
        TiledMapObjectLayer _platformTiledObj;
        TiledMapObjectLayer pizzariapoint;
        TiledMapObjectLayer block_visioin1;
        TiledMapObjectLayer block_visioin2;
        TiledMapObjectLayer mons;
        TiledMapObjectLayer enemyLayer, enemyLayer2;
        TiledMapObjectLayer cloudInvisibleLayer;
        TiledMapObjectLayer invisibleEnemyLayer;
        TiledMapObjectLayer invisibleEnemyLayer2;

        bool IsDead;
        int deadcount = 1;
        SpriteFont deadcountfont;
        SpriteFont Text;
        SpriteFont Text2;

        private readonly List<IEntity> _entities = new List<IEntity>();
        public readonly CollisionComponent _collisionComponent;

        const int MapWidth = 12256;
        const int MapHeight = 600;

        public static OrthographicCamera _camera;
        public static Vector2 _cameraPosition;
        public static Vector2 _bgPosition;

        //menu
        private Texture2D menumain;
        private Texture2D _gameoverTex;

        //buttons
        private Texture2D _startbuttTex;
        private Texture2D _exitbuttTex;
        private Texture2D _restartbuttTex;
        private Texture2D _menubuttTex;
        private Texture2D _victoryScreenTex;

        private Rectangle _startButton;
        private Rectangle _exitButton;
        private Rectangle _restartButton;
        private Rectangle _menuButton;

        //pizzariapoint
        private Vector2 pizzariaPosition;

        TiledMap _tiledMap;
        TiledMapRenderer _tiledMapRenderer;

        KeyboardState ks;
        public Game1()
        {
            _collisionComponent = new CollisionComponent(new RectangleF(0, 0, MapWidth, MapHeight));
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }
        GameState currentState;
        public enum GameState
        {
            MainMenu,
            Gameplay,
            GameOver,
            LevelComplete
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = 1050; // Set the width of window
            _graphics.PreferredBackBufferHeight = 600; // Set the height of window
            _graphics.ApplyChanges();
            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            var viewportadapter = new BoxingViewportAdapter(Window, GraphicsDevice, 800, 576);

            _camera = new OrthographicCamera(viewportadapter);
            _cameraPosition = Vector2.Zero;

            _bgPosition = new Vector2(400, 320);
            base.Initialize();
        }

        protected override void LoadContent()
        {

            //sound
            BGM = Content.Load<Song>("BGM");
            clickbutt = Content.Load<SoundEffect>("pressbutton");
            jumpsound = Content.Load<SoundEffect>("jump");
            deadthsound = Content.Load<SoundEffect>("death");

            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = 0.5f;
            MediaPlayer.Play(BGM);

            //map
            _tiledMap = Content.Load<TiledMap>("LostInsauce");
            _tiledMapRenderer = new TiledMapRenderer(GraphicsDevice, _tiledMap);

            //fontd
            deadcountfont = Content.Load<SpriteFont>("Resources/deadcountfont");
            Text = Content.Load<SpriteFont>("Resources/Ingamefont");
            Text2 = Content.Load<SpriteFont>("Resources/Ingamefont");

            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _victoryScreenTex = Content.Load<Texture2D>("victorynakub");
            _gameoverTex = Content.Load<Texture2D>("DIE_screen");
            menumain = Content.Load<Texture2D>("menu");
            _startbuttTex = Content.Load<Texture2D>("StartButton");
            _exitbuttTex = Content.Load<Texture2D>("exit");
            _restartbuttTex = Content.Load<Texture2D>("restart");
            _menubuttTex = Content.Load<Texture2D>("menubutt");


            currentState = GameState.MainMenu;
            
            //buttons position
            _startButton = new Rectangle(450, 300, _startbuttTex.Width, _startbuttTex.Height);
            _exitButton = new Rectangle(450, 400, _exitbuttTex.Width, _exitbuttTex.Height);
            _restartButton = new Rectangle(450, 400, _restartbuttTex.Width, _restartbuttTex.Height);
            _menuButton = new Rectangle(450, 500, _menubuttTex.Width, _menubuttTex.Height);

            //Setup apples
            SpriteSheet enemyinvisiblesheet = Content.Load<SpriteSheet>("Resources/enemynoo.sf", new JsonContentLoader());
            SpriteSheet AppleSheet = Content.Load<SpriteSheet>("Resources/bell_ani.sf", new
            JsonContentLoader());
            //set enemy
            SpriteSheet enemySheet = Content.Load<SpriteSheet>("Resources/enemy_2.sf", new JsonContentLoader());
            SpriteSheet cloudSheet = Content.Load<SpriteSheet>("Resources/cloud_sheet.sf", new JsonContentLoader());
            //set player
            SpriteSheet playerSheet = Content.Load<SpriteSheet>("Resources/Character_Ani.sf",
new JsonContentLoader());

            foreach (TiledMapObjectLayer layer in _tiledMap.ObjectLayers)
            {
                if (layer.Name == "block_visioin1")
                {
                    block_visioin1 = layer;
                }
                if (layer.Name == "block_visioin2")
                {
                    block_visioin2 = layer;
                }
                if (layer.Name == "finishpizzaria")
                {
                    pizzariapoint = layer;
                }
                if (layer.Name == "enemyLayer")
                {
                    enemyLayer = layer;
                }
                if (layer.Name == "enemyLayer2")
                {
                    enemyLayer2 = layer;
                }
                if (layer.Name == "mons")
                {
                    mons = layer;
                }
                if (layer.Name == "cloud_invisible")
                {
                    cloudInvisibleLayer = layer;
                }
                if(layer.Name == "invisible_enemy")
                {
                    invisibleEnemyLayer = layer;
                }
                if(layer.Name == "pineapple_invisible")
                {
                    invisibleEnemyLayer2 = layer;
                }
            }
            //pizzaria cheat--------------------------------------------------------------
            foreach(TiledMapObject obj in pizzariapoint.Objects)
            {
                Point2 position = new Point2(obj.Position.X, obj.Position.Y);
                pizzariaPosition = new Vector2 (position.X, position.Y);
                _entities.Add(new PizzariaEntity(this, new RectangleF(position, obj.Size)));
            }
            //enemy============================================================================
            foreach (TiledMapObject obj in invisibleEnemyLayer2.Objects)
            {
                Point2 position = new Point2(obj.Position.X, obj.Position.Y);
                _entities.Add(new InvisibleEnemyEntity2(this, new RectangleF(position, obj.Size), new AnimatedSprite(enemySheet)));
            }
            foreach (TiledMapObject obj in invisibleEnemyLayer.Objects)
            {
                Point2 position = new Point2(obj.Position.X,obj.Position.Y);
                _entities.Add(new InvisibleEnemyEntity(this, new RectangleF(position, obj.Size), new AnimatedSprite(enemyinvisiblesheet)));
            }
            foreach(TiledMapObject obj in enemyLayer.Objects)
            {
                Point2 position = new Point2(obj.Position.X + obj.Size.Width /2, obj.Position.Y + obj.Size.Height /2);
                float radius = obj.Size.Width / 2;
                _entities.Add(new EnemyEntity(this, new Vector2(position.X, position.Y), new AnimatedSprite(enemySheet)));
            }
            foreach (TiledMapObject obj in enemyLayer2.Objects)
            {
                Point2 position = new Point2(obj.Position.X + obj.Size.Width / 2, obj.Position.Y + obj.Size.Height / 2);
                float radius = obj.Size.Width / 2;
                _entities.Add(new EnemyEntity2(this, new Vector2(position.X, position.Y), new AnimatedSprite(enemySheet)));
            }
            foreach(TiledMapObject obj in cloudInvisibleLayer.Objects)
            {
                Point2 position = new Point2(obj.Position.X, obj.Position.Y);
                _entities.Add(new CloudInvisibleEntity(this, new RectangleF(position, obj.Size), new AnimatedSprite(cloudSheet)));
            }
            //==================================================================================
            foreach (TiledMapObject obj in pizzariapoint.Objects)
            {
                Point2 position = new Point2(obj.Position.X, obj.Position.Y);
                _entities.Add(new PizzariaEntity(this, new RectangleF(position, obj.Size)));
            }
            foreach (TiledMapObject obj in block_visioin1.Objects)
            {
                Point2 position = new Point2(obj.Position.X, obj.Position.Y);
                _entities.Add(new PlatformEntity(this, new RectangleF(position, obj.Size)));
            }
            foreach (TiledMapObject obj in block_visioin2.Objects)
            {
                Point2 position = new Point2(obj.Position.X, obj.Position.Y);
                _entities.Add(new PlatformEntity(this, new RectangleF(position, obj.Size)));
            }
            //Setup apples
            foreach (TiledMapObject obj in mons.Objects)
            {
                Point2 position = new Point2(obj.Position.X + obj.Size.Width / 2, obj.Position.Y + obj.Size.Height / 2);
                float radius = obj.Size.Width / 2;
                _entities.Add(new AppleEntity(this, new CircleF(position, obj.Size.Width / 2), new AnimatedSprite(AppleSheet)));
            }
            _entities.Add(new PlayerEntity(this, new RectangleF(new Point2(64, 470), new Size2(64, 64)), new AnimatedSprite(playerSheet)));

            foreach (IEntity entity in _entities)
            {
                _collisionComponent.Insert(entity);
            }


            // TODO: use this.Content to load your game content here
        }

        //cheat--------------------------------------------------------------------
        public Vector2 GetPizzariaPosition()
        {
            return pizzariaPosition;
        }
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            _tiledMapRenderer.Update(gameTime);
            // TODO: Add your update logic here

            foreach (IEntity entity in _entities)
            {
                if (entity is AppleEntity)
                {
                    if (!((AppleEntity)entity).IsExist())
                    {
                        _collisionComponent.Remove(entity);
                    }
                }
                entity.Update(gameTime);
            }

            //scene
            switch (currentState)
            {
                case GameState.MainMenu:
                    UpdateMainmenu();
                    break;

                case GameState.Gameplay:
                    UpdateGameplay();
                    break;

                case GameState.GameOver:
                    UpdateGameOver();
                    break;
                case GameState.LevelComplete:
                    UpdateVictoryScreen();
                    break;
            }

            _collisionComponent.Update(gameTime);
            _camera.LookAt(_bgPosition + _cameraPosition);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            var transformMatrix = _camera.GetViewMatrix();
            _tiledMapRenderer.Draw(transformMatrix);
            // TODO: Add your drawing code here

            switch (currentState)
            {
                case GameState.MainMenu:
                    _spriteBatch.Begin();
                    Drawmenu();
                    _spriteBatch.End();
                    break;

                case GameState.Gameplay:
                    _spriteBatch.Begin(transformMatrix: transformMatrix);
                    DrawGameplay();
                    foreach (IEntity entity in _entities)
                    {
                        entity.Draw(_spriteBatch);
                    }
                    _spriteBatch.End();
                    break;

                case GameState.GameOver:
                    _spriteBatch.Begin();
                    DrawGameOver();
                    _spriteBatch.End();
                    break;
                case GameState.LevelComplete:
                    _spriteBatch.Begin();
                    DrawVictoryScreen();
                    _spriteBatch.End();
                    break;
            }

        }
        public int GetMapWidth()
        {
            return MapWidth;
        }
        public void UpdateCamera(Vector2 move)
        {
            _cameraPosition.X += move.X;
        }
        public float GetCameraPosX()
        {
            return _cameraPosition.X;
        }

        //Menu
        private void Drawmenu()
        {
            _spriteBatch.Draw(menumain, Vector2.Zero, Color.White);
            _spriteBatch.Draw(_startbuttTex, _startButton, Color.White);
            _spriteBatch.Draw(_exitbuttTex, _exitButton, Color.White);
        }
        private void DrawGameplay()
        {
            _spriteBatch.DrawString(Text, "Glai  laew  try  to  susu  do", new Vector2(9940, 175), Color.Black);
            _spriteBatch.DrawString(Text2, "Press  R  to  Emote", new Vector2(11000, 230), Color.Black);
        }

        private void ResetGame()
        {
            // Reset camera position
            _cameraPosition = Vector2.Zero;
            _bgPosition = new Vector2(400, 320);

            // Clear existing entities
            foreach (IEntity entity in _entities)
            {
                _collisionComponent.Remove(entity);  // Remove each entity from collision component
            }
            _entities.Clear();

            // Reload entities
            // SpriteSheet enemyInvisible = Content.Load<SpriteSheet>("Resources/Enermy2.sf", new JsonContentLoader());
            SpriteSheet enemyinvisiblesheet = Content.Load<SpriteSheet>("Resources/enemynoo.sf", new JsonContentLoader());
            SpriteSheet AppleSheet = Content.Load<SpriteSheet>("Resources/bell_ani.sf", new JsonContentLoader());
            SpriteSheet playerSheet = Content.Load<SpriteSheet>("Resources/Character_Ani.sf", new JsonContentLoader());
            SpriteSheet enemySheet = Content.Load<SpriteSheet>("Resources/enemy_2.sf", new JsonContentLoader());
            SpriteSheet cloudSheet = Content.Load<SpriteSheet>("Resources/cloud_sheet.sf", new JsonContentLoader());

            //enemy============================================================================
            foreach (TiledMapObject obj in invisibleEnemyLayer2.Objects)
            {
                Point2 position = new Point2(obj.Position.X, obj.Position.Y);
                _entities.Add(new InvisibleEnemyEntity2(this, new RectangleF(position, obj.Size), new AnimatedSprite(enemySheet)));
            }

            foreach (TiledMapObject obj in invisibleEnemyLayer.Objects)
            {
                Point2 position = new Point2(obj.Position.X, obj.Position.Y);
                _entities.Add(new InvisibleEnemyEntity(this, new RectangleF(position, obj.Size), new AnimatedSprite(enemyinvisiblesheet)));
            }
            foreach (TiledMapObject obj in enemyLayer.Objects)
            {
                Point2 position = new Point2(obj.Position.X + obj.Size.Width /2, obj.Position.Y+ obj.Size.Height/2);
                float radius = obj.Size.Width / 2;
                _entities.Add(new EnemyEntity(this, new Vector2(position.X, position.Y),new AnimatedSprite(enemySheet)));
            }
            foreach (TiledMapObject obj in enemyLayer2.Objects)
            {
                Point2 position = new Point2(obj.Position.X + obj.Size.Width / 2, obj.Position.Y + obj.Size.Height / 2);
                float radius = obj.Size.Width / 2;
                _entities.Add(new EnemyEntity2(this, new Vector2(position.X, position.Y), new AnimatedSprite(enemySheet)));
            }
            foreach (TiledMapObject obj in cloudInvisibleLayer.Objects)
            {
                Point2 position = new Point2(obj.Position.X, obj.Position.Y);
                _entities.Add(new CloudInvisibleEntity(this, new RectangleF(position, obj.Size), new AnimatedSprite(cloudSheet)));
            }
            //enemy=============================================================================
            //reload pizzariapoint ---------------------------------------------------------------------
            foreach (TiledMapObject obj in pizzariapoint.Objects)
            {
                Point2 position = new Point2(obj.Position.X, obj.Position.Y);
                pizzariaPosition = new Vector2(position.X, position.Y);
                _entities.Add(new PizzariaEntity(this, new RectangleF(position, obj.Size)));
            }
            // Reload platforms
            foreach (TiledMapObject obj in block_visioin1.Objects)
            {
                Point2 position = new Point2(obj.Position.X, obj.Position.Y);
                _entities.Add(new PlatformEntity(this, new RectangleF(position, obj.Size)));
            }
            foreach (TiledMapObject obj in block_visioin2.Objects)
            {
                Point2 position = new Point2(obj.Position.X, obj.Position.Y);
                _entities.Add(new PlatformEntity(this, new RectangleF(position, obj.Size)));
            }

            // Reload collectibles
            foreach (TiledMapObject obj in mons.Objects)
            {
                Point2 position = new Point2(obj.Position.X + obj.Size.Width / 2,
                obj.Position.Y + obj.Size.Height / 2);
                _entities.Add(new AppleEntity(this, new CircleF(position, obj.Size.Width / 2),
                    new AnimatedSprite(AppleSheet)));
            }

            // Reload player
            _entities.Add(new PlayerEntity(this, new RectangleF(new Point2(64, 470), new Size2(64, 64)),
                new AnimatedSprite(playerSheet)));

            // Insert all new entities into collision component
            foreach (IEntity entity in _entities)
            {
                _collisionComponent.Insert(entity);
            }
            PlayBGM();
        }
        //camera cheat--------------------------------------------------------------------------
        public void SetCemaraPosition(float X)
        {
            _cameraPosition.X = X;
        }
        public void LevelComplete()
        {

            currentState = GameState.LevelComplete;
        }
        public void GameOver()
        {
            currentState = GameState.GameOver;
        }
        private void DrawVictoryScreen()
        {
            _spriteBatch.Draw(_victoryScreenTex, Vector2.Zero, Color.White);
            _spriteBatch.Draw(_menubuttTex, _menuButton, Color.White);
            _spriteBatch.DrawString(deadcountfont, "Wasted Pizza: " + deadcount, new Vector2(400, 400), Color.Black);
        }
        private void DrawGameOver()
        {
            _spriteBatch.Draw(_gameoverTex, Vector2.Zero, Color.White);
            _spriteBatch.Draw(_restartbuttTex, _restartButton, Color.White);
            _spriteBatch.Draw(_menubuttTex, _menuButton, Color.White);
            _spriteBatch.DrawString(deadcountfont, "Wasted Pizza = " + deadcount, new Vector2(400, 200), Color.Black);
        }
        private void UpdateVictoryScreen()
        {
            var mouseState = Mouse.GetState();
            var MousePosition = new Point(mouseState.X, mouseState.Y);
            var keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.M))
            {
                clickbutt.Play(0.5f, 0.0f, 0.0f);
                currentState = GameState.MainMenu;
                ResetGame();
            }

            if (_menuButton.Contains(MousePosition) && mouseState.LeftButton == ButtonState.Pressed)
            {
                clickbutt.Play(0.5f, 0.0f, 0.0f);
                currentState = GameState.MainMenu;
                ResetGame();
            }
        }
        //update
        private void UpdateMainmenu()
        {
            var mouseState = Mouse.GetState();
            var MousePosition = new Point(mouseState.X, mouseState.Y);
            var keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Space))
            {
                clickbutt.Play(0.5f, 0.0f, 0.0f);
                currentState = GameState.Gameplay;
            }
            if (keyboardState.IsKeyDown(Keys.Escape))
            {
                clickbutt.Play(0.5f, 0.0f, 0.0f);
                Exit();
            }

            if (_startButton.Contains(MousePosition) && mouseState.LeftButton == ButtonState.Pressed)
            {
                clickbutt.Play(0.5f, 0.0f, 0.0f);
                Exit();
            }
            if (_exitButton.Contains(MousePosition) && mouseState.LeftButton == ButtonState.Pressed)
            {
                clickbutt.Play(0.5f, 0.0f, 0.0f);
                currentState = GameState.Gameplay;
            }
            PauseBGM();
            ResumeeBGM();
        }

        private void UpdateGameplay()
        {
            var keyboardState = Keyboard.GetState();


            if (keyboardState.IsKeyDown(Keys.R))
            {
                ResetGame();
            }
            if (keyboardState.IsKeyDown(Keys.M))
            {
                currentState = GameState.MainMenu;
                ResetGame();
            }
            PlayerEntity player = null;
            foreach (IEntity entity in _entities)
            {
                if (entity is PlayerEntity)
                {
                    player = (PlayerEntity)entity;
                    break;
                }
            }

            if (player != null)
            {
                if (player.Bounds.Position.Y > MapHeight)
                {
                    player.Die();
                    return;
                }
                /*
                                var mouseState = Mouse.GetState();
                                var MousePosition = new Point(mouseState.X, mouseState.Y);
                                if (_restartButton.Contains(MousePosition) && mouseState.LeftButton == ButtonState.Pressed)
                                {
                                    currentState = GameState.Gameplay;
                                    return;
                                }
                                if (_menuButton.Contains(MousePosition) && mouseState.LeftButton == ButtonState.Pressed)
                                {
                                    currentState = GameState.MainMenu;
                                    return;
                                }
                            }
                            */
            }
            PauseBGM();
            ResumeeBGM();
        }

        private void UpdateGameOver()
        {
            var mouseState = Mouse.GetState();
            var MousePosition = new Point(mouseState.X, mouseState.Y);
            var keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.R))
            {
                clickbutt.Play(0.5f, 0.0f, 0.0f);
                currentState = GameState.Gameplay;
                ResetGame();
                deadcount++;
            }
            if (keyboardState.IsKeyDown(Keys.M))
            {
                clickbutt.Play(0.5f, 0.0f, 0.0f);
                currentState = GameState.MainMenu;
                ResetGame();
                deadcount++;
            }
            if (_restartButton.Contains(MousePosition) && mouseState.LeftButton == ButtonState.Pressed)
            {
                clickbutt.Play(0.5f, 0.0f, 0.0f);
                ResetGame();  // Reset the game state
                currentState = GameState.Gameplay;
                deadcount++;
                return;
            }
            if (_menuButton.Contains(MousePosition) && mouseState.LeftButton == ButtonState.Pressed)
            {
                clickbutt.Play(0.5f, 0.0f, 0.0f);
                ResetGame();  // Reset the game state
                currentState = GameState.MainMenu;
                deadcount++;
                return;
            }
            StopBGM();
        }

        private void PauseBGM()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.D1))
            {

                MediaPlayer.Pause();
            }
        }
        private void ResumeeBGM()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.D2))
            {

                MediaPlayer.Resume();
            }
        }
        private void PlayBGM()
        {
            MediaPlayer.Play(BGM);
        }

        public void StopBGM()
        {
            MediaPlayer.Pause();
        }
        public void DeathSound()
        {
            deadthsound.Play(0.5f, 0.0f, 0.0f);
        }
    }
}