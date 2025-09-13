using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pj
{
    internal class InvisibleEnemyEntity : IEntity
    {
        private readonly Game1 _game;
        public IShapeF Bounds { get; }
        private bool isVisible = false;

        private AnimatedSprite _enemySprite;
        string animation;

        public InvisibleEnemyEntity(Game1 game, RectangleF rectangleF, AnimatedSprite enemySprite)
        {
            _game = game;
            Bounds = rectangleF;

            animation = "enermynoo";
            _enemySprite = enemySprite;
            _enemySprite.Play(animation);
        }

        public virtual void Update(GameTime gameTime)
        {
            _enemySprite.Play(animation);
            _enemySprite.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.J) && (Keyboard.GetState().IsKeyDown(Keys.K)) && (Keyboard.GetState().IsKeyDown(Keys.L)))
            {
                spriteBatch.DrawRectangle((RectangleF)Bounds, Color.Red, 3f);
            }
            if (isVisible)
            {
                spriteBatch.Draw(_enemySprite.TextureRegion.Texture,((RectangleF)Bounds).ToRectangle(),_enemySprite.TextureRegion.Bounds,Color.White);
            }
        }

        public void OnCollision(CollisionEventArgs collisionInfo)
        {
            if(collisionInfo.Other is PlayerEntity)
            {
                isVisible = true;
                ((PlayerEntity)collisionInfo.Other).Die();
            }
        }
    }
}
