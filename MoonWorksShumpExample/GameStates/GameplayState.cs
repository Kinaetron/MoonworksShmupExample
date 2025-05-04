using MoonTools.ECS;
using MoonWorks.Graphics;
using MoonWorksShumpExample.Components;
using MoonWorksShumpExample.Graphics;
using MoonWorksShumpExample.Systems;
using MoonWorksShumpExample.Utility;
using System.Numerics;
using Random = System.Random;

namespace MoonWorksShumpExample.GameStates
{
    public class GameplayState : IGameState
    {
        private readonly Input _input;
        private readonly World _world;
        private readonly Motion _motion;
        private readonly Timing _timing;
        private readonly SpriteRenderer _spriteRenderer;
        private readonly PlayerController _playerController;
        private readonly BulletController _bulletController;
        private readonly Collision _collision;
        private readonly Destory _destory;
        private readonly EnemyController _enemyController;
        private readonly Destroy _destory;
        private readonly Health _health;

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

            _bulletController = new BulletController(
             _world,
             new TextureId(_spriteRenderer.CreateTexture("Content/Sprites/Bullet.png")));

            _input = new Input(_world, game.Inputs);
            _playerController = new PlayerController(_world, _bulletController);
            _enemyController = new EnemyController(_world);
            _motion = new Motion(_world);
            _timing = new Timing(_world);
            _collision = new Collision(_world);
            _destory = new Destroy(_world);
            _health = new Health(_world);
        }

        public void Start()
        {
            var player = _world.CreateEntity();
            _world.Set(player, new Player());
            _world.Set(player, new TextureId(_spriteRenderer.CreateTexture("Content/Sprites/Triangle.png")));
            _world.Set(player, new Position(new Vector2(150, 200)));
            _world.Set(player, new Rotation(0));
            _world.Set(player, new Size(new Vector2(13, 15)));
            _world.Set(player, Color.White);
            _world.Set(player, new Velocity(Vector2.Zero));
            _world.Set(player, new Accerlation(2.0f * Time.TargetFrameRate));
            _world.Set(player, new MaxSpeed(2.0f * Time.TargetFrameRate));
            _world.Set(player, new Rectangle(0, 0, 8, 8));
            _world.Set(player, new Solid());
            _world.Set(player, new SweepCollision());

            var enemy = _world.CreateEntity();
            _world.Set(enemy, new Enemy());
            _world.Set(enemy, new TextureId(_spriteRenderer.CreateTexture("Content/Sprites/Square.png")));
            _world.Set(enemy, new Position(new Vector2(160, 120)));
            _world.Set(enemy, new Rotation(0));
            _world.Set(enemy, new Size(new Vector2(16, 16)));
            _world.Set(enemy, new Rectangle(0, 0, 16, 16));
            _world.Set(enemy, new Solid());
            _world.Set(enemy, Color.White);
            _world.Set(enemy, new CanTakeDamage(5));

            SpawnXEnemies(3);

            var topBorder = _world.CreateEntity();
            _world.Set(topBorder, new Position(Vector2.Zero));
            _world.Set(topBorder, new Rectangle(0, 0, Dimensions.GameWidth, 10));
            _world.Set(topBorder, new Solid());

            var leftBorder = _world.CreateEntity();
            _world.Set(leftBorder, new Position(new Vector2(-10, 0)));
            _world.Set(leftBorder, new Rectangle(0, 0, 15, Dimensions.GameHeight));
            _world.Set(leftBorder, new Solid());

            var rightBorder = _world.CreateEntity();
            _world.Set(rightBorder, new Position(new Vector2(Dimensions.GameWidth-10, 0)));
            _world.Set(rightBorder, new Rectangle(0, 0, 10, Dimensions.GameHeight));
            _world.Set(rightBorder, new Solid());

            var bottomBorder = _world.CreateEntity();
            _world.Set(bottomBorder, new Position(new Vector2(0, Dimensions.GameHeight-10)));
            _world.Set(bottomBorder, new Rectangle(0, 0, Dimensions.GameWidth, 10));
            _world.Set(bottomBorder, new Solid());
        }

        public void Update(TimeSpan delta)
        {
            _timing.Update(delta);
            _input.Update(delta);
            _playerController.Update(delta);
            _bulletController.Update(delta);
            _enemyController.Update(delta);
            _motion.Update(delta);
            _collision.Update(delta);
            _health.Update(delta);
            _destory.Update(delta);

            _world.FinishUpdate();
        }

        public void Draw(double alpha)
        {
            _spriteRenderer.Render(alpha);
        }

        public void End()
        {
        }

        private void SpawnXEnemies(int enemiesToSpawn)
        {
            var rndNum = new Random();
            while (enemiesToSpawn > 0)
            {
                var enemy = _world.CreateEntity();
                _world.Set(enemy, new Enemy());
                _world.Set(enemy, new TextureId(_spriteRenderer.CreateTexture("Content/Sprites/Square.png")));
                _world.Set(enemy, new Position(new Vector2(rndNum.Next(10, Dimensions.GameHeight-10), rndNum.Next(10, Dimensions.GameWidth-10))));
                _world.Set(enemy, new Rotation(0));
                _world.Set(enemy, new Size(new Vector2(13, 15)));
                _world.Set(enemy, Color.White);
                _world.Set(enemy, new Velocity(Vector2.Zero));
                _world.Set(enemy, new Accerlation(2.0f * Time.TargetFrameRate));
                _world.Set(enemy, new MaxSpeed(2.0f * Time.TargetFrameRate));
                _world.Set(enemy, new Rectangle(0, 0, 8, 8));
                _world.Set(enemy, new Solid());
                
                enemiesToSpawn--;
            }
        }
    }
}
