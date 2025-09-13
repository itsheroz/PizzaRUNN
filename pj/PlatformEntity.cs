using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;

namespace pj
{
    internal class PlatformEntity : IEntity
    {
        private readonly Game1 _game;
        public IShapeF Bounds { get; }
        public PlatformEntity(Game1 game, RectangleF rectangleF)
        {
            _game = game;
            Bounds = rectangleF;
        }
        public virtual void Update(GameTime gameTime)
        {
        }
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.J) && (Keyboard.GetState().IsKeyDown(Keys.K)) && (Keyboard.GetState().IsKeyDown(Keys.L)))
            {

                spriteBatch.DrawRectangle((RectangleF)Bounds, Color.Red, 3f);
            }
        }
        public void OnCollision(CollisionEventArgs collisionInfo)
        {
        }
    }
}