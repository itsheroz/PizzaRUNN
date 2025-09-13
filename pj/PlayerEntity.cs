using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.Sprites;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using MonoGame.Extended.ViewportAdapters;
using MonoGame.Extended.TextureAtlases;

namespace pj
{
    internal class PlayerEntity : IEntity
    {

        //dalay=============
        bool isdying = false;
        float deathTimer = 0f;
        const float Death_ani_Time = 1.5f;
        float deathJumpVelocity = -300f;
        bool hasPlayedDeathSound = false;
        SoundEffect deathsound;
        //delay=============

        private bool isExist;
        private readonly Game1 _game;
        public int Velocity = 7;
        Vector2 move;

        private SoundEffect jumpsound;

        public IShapeF Bounds { get; }

        bool isJumping;
        bool isGrounded;
        public int jumpSpeed;
        float force;

        //keyboard ------------------------------------------------
        private KeyboardState _currentKey;
        private KeyboardState _oldKey;

        private AnimatedSprite _playerSprite;
        string animation;
        public PlayerEntity(Game1 game, IShapeF circleF, AnimatedSprite playerSprite)
        {
            _game = game;
            Bounds = circleF;
            jumpsound = game.Content.Load<SoundEffect>("jump");
            deathsound = game.Content.Load<SoundEffect>("death");

            jumpSpeed = -6;
            force = 20;
            isGrounded = false;
            _playerSprite = playerSprite;

            animation = "p_idle";
            playerSprite.Play(animation);
            _playerSprite = playerSprite;
        }
        public virtual void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _currentKey = Keyboard.GetState();
            //dead=======================
            if (isdying)
            {
                deathTimer += deltaTime;
                Bounds.Position = new Vector2(Bounds.Position.X, Bounds.Position.Y+ deathJumpVelocity * deltaTime);
                deathJumpVelocity += 800f * deltaTime;
                animation = "p_dead";
                _playerSprite.Play(animation);
                _playerSprite.Update(gameTime);

                if(deathTimer > Death_ani_Time)
                {
                    _game.GameOver();
                }
                return;
            }
            //cheat-----------------------------------------------------------
            if(_currentKey.IsKeyDown(Keys.P) && _oldKey.IsKeyUp(Keys.P))
            {
                Vector2 pizzariaPos = _game.GetPizzariaPosition();
                Bounds.Position = new Vector2(pizzariaPos.X-1400, pizzariaPos.Y);
                //float cameraDelta = pizzariaPos.X - Bounds.Position.X;

                Bounds.Position = new Vector2(pizzariaPos.X-1400, pizzariaPos.Y);

                _game.SetCemaraPosition(pizzariaPos.X - 1400);
                //_game.UpdateCamera(new Vector2(cameraDelta, 0));
            }

            if (isGrounded) { animation = "p_idle"; }

            if (_currentKey.IsKeyDown(Keys.D) && Bounds.Position.X < _game.GetMapWidth() -
            ((RectangleF)Bounds).Width)
            {
                move = new Vector2(Velocity, 0) * gameTime.GetElapsedSeconds() * 50;
                if (Bounds.Position.X - _game.GetCameraPosX() >= 200 && _game.GetCameraPosX() < _game.GetMapWidth() - (_game.GetMapWidth() / 14))
                {
                    _game.UpdateCamera(move);
                }
                Bounds.Position += move;
                if (isGrounded)
                {
                    animation = "p_run";
                    _playerSprite.Effect = SpriteEffects.None;
                }
            }
            else if (_currentKey.IsKeyDown(Keys.A) && Bounds.Position.X > 0)
            {
                move = new Vector2(-Velocity, 0) * gameTime.GetElapsedSeconds() * 50;
                if (Bounds.Position.X - _game.GetCameraPosX() <= 300 && _game.GetCameraPosX() > 0)
                {
                    _game.UpdateCamera(move);
                }
                Bounds.Position += move;
                if (isGrounded)
                {
                    animation = "p_run";
                    _playerSprite.Effect = SpriteEffects.FlipHorizontally;
                }
            }

            if (isJumping && force < 0)
            {
                isJumping = false;
            }
            if (_currentKey.IsKeyDown(Keys.W) && _oldKey.IsKeyUp(Keys.W) && isGrounded)
            {
                jumpsound.Play(0.5f, 0.0f, 0.0f);
                isJumping = true;
                isGrounded = false;
            }
            if (isJumping)
            {
                animation = "p_jump";
                jumpSpeed = -16;
                force -= gameTime.GetElapsedSeconds() * 32;
            }
            else
            {
                jumpSpeed = 18;
                if (!isGrounded)
                {
                    animation = "p_idle";
                    
                }
            }
            Bounds.Position += new Vector2(0, jumpSpeed) * gameTime.GetElapsedSeconds() * 50;
            _playerSprite.Play(animation);
            _playerSprite.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            _oldKey = _currentKey;
        }

        public void Die()
        {
            if (!isdying)
            {
                isdying = true;
                deathTimer = 0f;
                deathJumpVelocity = -300f;
                hasPlayedDeathSound = false;
            }

            if (!hasPlayedDeathSound)
            {
                _game.StopBGM();
                deathsound.Play(0.5f,0.0f,0.0f);
                hasPlayedDeathSound = true;
            }
            animation = "p_dead";
            _playerSprite.Play(animation);
        }
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (isdying)
            {
                // Draw with red tint when dying
                spriteBatch.Draw(_playerSprite.TextureRegion.Texture, ((RectangleF)Bounds).Center,_playerSprite.TextureRegion.Bounds,
                    Color.Red,0f,new Vector2(_playerSprite.TextureRegion.Width/2, _playerSprite.TextureRegion.Height /2),1f,_playerSprite.Effect,0);
            }
            else
            {
                // Normal white color when alive
                spriteBatch.Draw(_playerSprite, ((RectangleF)Bounds).Center);
            }
        }
        public void OnCollision(CollisionEventArgs collisionInfo)
        {
            //dying-------
            if(isdying) return;
            if(collisionInfo.Other is EnemyEntity || collisionInfo.Other is EnemyEntity2 || collisionInfo.Other is CloudInvisibleEntity)
            {
                Die();
                return;
            }

            //dying--------

            if (collisionInfo.Other.ToString().Contains("PizzariaPoint"))
            {
                _game.LevelComplete();
                return;
            }

            if (collisionInfo.Other.ToString().Contains("PlatformEntity"))
            {
                if (!isJumping)
                {
                    if (((RectangleF)Bounds).Top < ((RectangleF)collisionInfo.Other.Bounds).Top &&
                    ((RectangleF)Bounds).Bottom < ((RectangleF)collisionInfo.Other.Bounds).Bottom)
                    {
                        isGrounded = true;
                        force = 11;
                    }
                    Bounds.Position -= collisionInfo.PenetrationVector;
                }
            }
            if (isJumping)
            {


                if (collisionInfo.Other.ToString().Contains("PlatformEntity"))
                {
                    force = -11;
                }
                Bounds.Position -= collisionInfo.PenetrationVector;
            }
        }

    }
}