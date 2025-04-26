using MoonTools.ECS;
using MoonWorks.Graphics;
using MoonWorksShumpExample.Components;
using System.Numerics;

namespace MoonWorksShumpExample.Graphics;

public class SpriteRenderer : Renderer
{
    private int textureId = 0;
    private readonly SpriteBatch _spriteBatch;
    private Dictionary<int, Texture> _textureDictionary = [];

    private readonly MoonTools.ECS.Filter _spriteFilter;

    public SpriteRenderer(World world, SpriteBatch spriteBatch) :
        base(world)
    {
        _spriteBatch = spriteBatch;

        _spriteFilter = FilterBuilder
            .Include<TextureId>()
            .Include<Position>()
            .Include<Rotation>()
            .Include<Size>()
            .Include<Color>()
            .Build();
    }

    public int CreateTexture(string filePath)
    {
        var texture = _spriteBatch.CreateTexture(filePath);
        _textureDictionary.Add(textureId, texture);
        return textureId++;
    }

    public void Render(double alpha)
    {
        _spriteBatch.Begin(Color.Black, Matrix4x4.Identity);
        foreach (var entity in _spriteFilter.Entities)
        {
            var textureId = Get<TextureId>(entity).Value;
            var position = Get<Position>(entity).Value;
            var rotation = Get<Rotation>(entity).Value;
            var size = Get<Size>(entity).Value;
            var color = Get<Color>(entity);

            _spriteBatch.Draw(
                _textureDictionary[textureId],
                position,
                rotation,
                size,
                color);
        }
        _spriteBatch.End();
    }
}
