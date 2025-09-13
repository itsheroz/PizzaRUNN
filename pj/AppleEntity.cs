using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Sprites;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using System;

namespace pj
{
    internal class AppleEntity : IEntity
    {
        private readonly Game1 _game;
        public IShapeF Bounds { get; }
        private bool isExist;

        private AnimatedSprite _appleSprite;
        string animation;
        public AppleEntity(Game1 game, CircleF circleF, AnimatedSprite appleSprite)
        {
            _game = game;
            Bounds = circleF;
            isExist = true;

            animation = "bell";
            appleSprite.Play(animation);
            _appleSprite = appleSprite;
        }
        public virtual void Update(GameTime gameTime)
        {
            _appleSprite.Play(animation);
            _appleSprite.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
        }
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (isExist)
            {
                //spriteBatch.DrawCircle((CircleF)Bounds,8,Color.Red,3f);
                spriteBatch.Draw(_appleSprite, Bounds.Position);
            }
        }
        public void OnCollision(CollisionEventArgs collisioninfo)
        {
            if (isExist)
            {
                if (collisioninfo.Other.ToString().Contains("PlayerEntity"))
                {
                    isExist = false;

                }
            }
        }
        public bool IsExist()
        {
            return isExist;
        }

    }
}