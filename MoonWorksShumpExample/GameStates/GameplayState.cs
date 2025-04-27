using MoonTools.ECS;
using MoonWorks.Graphics;
using MoonWorksShumpExample.Components;
using MoonWorksShumpExample.Graphics;
using MoonWorksShumpExample.Systems;
using System.Numerics;

namespace MoonWorksShumpExample.GameStates
{
    public class GameplayState : IGameState
    {
        private readonly Input _input;
        private readonly World _world;
        private readonly SpriteRenderer _spriteRenderer;

        public GameplayState(ShumpExample game)
        {
            _world = new World();
            var spriteBatch = new SpriteBatch(
                Dimensions.GameWidth,
                Dimensions.GameHeight,
                game.GraphicsDevice,
                game.RootTitleStorage,
                game.MainWindow);

            _spriteRenderer = 
                new SpriteRenderer(_world, spriteBatch);

            _input = new Input(_world, game.Inputs);
        }

        public void Start()
        {
            var player = _world.CreateEntity();
            _world.Set(player, new Player());
            _world.Set(player, new TextureId(_spriteRenderer.CreateTexture("Content/Sprites/Triangle.png")));
            _world.Set(player, new Position(new Vector2(160, 120)));
            _world.Set(player, new Rotation(0));
            _world.Set(player, new Size(new Vector2(13, 15)));
            _world.Set(player, Color.White);
        }

        public void Update(TimeSpan delta)
        {
            _input.Update(delta);
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
