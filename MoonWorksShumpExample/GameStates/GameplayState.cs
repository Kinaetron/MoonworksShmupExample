using MoonTools.ECS;
using MoonWorks;
using MoonWorks.Graphics;
using MoonWorks.Storage;
using MoonWorksShumpExample.Components;
using MoonWorksShumpExample.Graphics;
using System.Numerics;

namespace MoonWorksShumpExample.GameStates
{
    public class GameplayState : IGameState
    {
        private readonly World _world;
        private readonly SpriteRenderer _spriteRenderer;

        public GameplayState(
            GraphicsDevice graphicsDevice, 
            TitleStorage titleStorage,
            Window window)
        {
            _world = new World();
            var spriteBatch = new SpriteBatch(
                Dimensions.GameWidth,
                Dimensions.GameHeight,
                graphicsDevice,
                titleStorage,
                window);

            _spriteRenderer = 
                new SpriteRenderer(_world, spriteBatch);
        }

        public void Start()
        {
            var player = _world.CreateEntity();
            _world.Set(player, new TextureId(_spriteRenderer.CreateTexture("Content/Sprites/Triangle.png")));
            _world.Set(player, new Position(new Vector2(120, 120)));
            _world.Set(player, new Rotation(0));
            _world.Set(player, new Size(new Vector2(13, 15)));
            _world.Set(player, Color.White);
        }

        public void Update(TimeSpan delta)
        {
        }

        public void Draw(double alpha)
        {
            _spriteRenderer.Render(alpha);
        }

        public void End()
        {
        }
    }
}
