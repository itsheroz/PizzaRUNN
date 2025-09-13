using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pj
{
    internal class PizzariaEntity : IEntity
    {
        private readonly Game1 _game;
        public IShapeF Bounds { get; }

        public PizzariaEntity(Game1 game, RectangleF rectangleF)
        {
            _game = game;
            Bounds = rectangleF;
        }
        public virtual void Update(GameTime gameTime)
        {

        }
        public virtual void Draw(SpriteBatch spriteBatch)
        {
          //  spriteBatch.DrawRectangle((RectangleF)Bounds, Color.Yellow, 3);
        }

        public void OnCollision(CollisionEventArgs collisionInfo)
        {
            if (collisionInfo.Other.ToString().Contains("PlayerEntity"))
            {
                _game.LevelComplete();
            }
        }
    }
}
