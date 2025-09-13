using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
    internal class EnemyEntity2 : IEntity
    {
        //enemy
        float _distance = 200;
        bool _moveleft = true;

        private readonly Game1 _game;
        public IShapeF Bounds { get; }
        private Vector2 _startPos;

        float _angle = 0f;
        float _speed = 700f;

        private AnimatedSprite _enemySprite;
        string animation;

        public EnemyEntity2(Game1 game, Vector2 position, AnimatedSprite enemySprite)
        {
            _game = game;
            _startPos = position;
            Bounds = new CircleF(position, 30);

            animation = "mouse";
            enemySprite.Play(animation);
            _enemySprite = enemySprite;

        }

        public virtual void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            //Move Left
            if (_moveleft)
            {
                if (Bounds.Position.X < _startPos.X + _distance)
                {
                    Bounds.Position = new Point2(Bounds.Position.X + _speed * deltaTime, Bounds.Position.Y);
                    _enemySprite.Effect = SpriteEffects.FlipHorizontally;
                }
                else
                {
                    _moveleft = false;
                }
            }
            else
            {
                if (Bounds.Position.X > _startPos.X - _distance)
                {
                    Bounds.Position = new Point2(Bounds.Position.X - _speed * deltaTime, Bounds.Position.Y);
                    _enemySprite.Effect = SpriteEffects.None;
                }
                else
                {
                    _moveleft = true;
                }
            }
            // Update sprite
            _enemySprite.Play(animation);
            _enemySprite.Update(deltaTime);
        }
        public virtual void Draw(SpriteBatch spriteBatch)
        {
          //  spriteBatch.DrawCircle((CircleF)Bounds, 8, Color.Red, 3f); // Debug circle
            spriteBatch.Draw(_enemySprite, Bounds.Position);
        }
        public void OnCollision(CollisionEventArgs collisionInfo)
        {
            if (collisionInfo.Other is PlayerEntity)
            {
                ((PlayerEntity)collisionInfo.Other).Die();
            }
        }
    }
}
