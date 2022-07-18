using UnityEngine;

public class WorldTextureGenerator : MonoBehaviour
{
	public SpriteRenderer spriteRenderer;
	public Sprite worldBaseSprite;
	Sprite worldSprite;

	public Color ground;
	public Color grass;
	public Color water;
	public Color sand;

	public WorldTextureNode[,] grid;
	int maxX;
	int maxY;

	public int initialWaterPercentage = 55;

	public SimulationStep currentStep;
	public SimulationStep beachStep;

	private void Start()
	{
		CreateBase();
	}

	public void CreateBase()
	{
		worldSprite = Instantiate(worldBaseSprite);
		maxX = worldSprite.texture.width;
		maxY = worldSprite.texture.height;

		grid = new WorldTextureNode[maxX, maxY];

		for (int x = 0; x < maxX; x++)
		{
			for (int y = 0; y < maxY; y++)
			{
				WorldTextureNode node = new();
				node.x = x;
				node.y = y;
				grid[x, y] = node;

				if (Random.Range(0, 100) > initialWaterPercentage)
				{
					node.type = WorldTextureNode.Type.GRASS;
				}
				else
				{
					node.type = WorldTextureNode.Type.WATER;
				}
			}
		}

		DrawWorldSprite();

		spriteRenderer.sprite = worldSprite;
	}

	public void Step()
	{
		for (int x = 0; x < maxX; x++)
		{
			for (int y = 0; y < maxY; y++)
			{
				WorldTextureNode node = grid[x, y];
				node.type = currentStep.GetNodeState(node, grid, maxX, maxY);
			}
		}

        ApplyBeachStep();

        DrawWorldSprite();
	}

	public void ApplySteps(int amount)
	{
		for (int i = 0; i < amount; i++)
		{
			Step();
		}
	}

	private void ApplyBeachStep()
    {
		for (int x = 0; x < maxX; x++)
		{
			for (int y = 0; y < maxY; y++)
			{
				WorldTextureNode node = grid[x, y];
				node.type = beachStep.GetNodeState(node, grid, maxX, maxY);
			}
		}
	}

	private void DrawWorldSprite()
	{
		for (int x = 0; x < maxX; x++)
		{
			for (int y = 0; y < maxY; y++)
			{
				WorldTextureNode node = grid[x, y];

				switch(node.type)
                {
					case WorldTextureNode.Type.GROUND:
						worldSprite.texture.SetPixel(x, y, ground);
						break;
					case WorldTextureNode.Type.GRASS:
						worldSprite.texture.SetPixel(x, y, grass);
						break;
					case WorldTextureNode.Type.WATER:
						worldSprite.texture.SetPixel(x, y, water);
						break;
					case WorldTextureNode.Type.BEACH:
						worldSprite.texture.SetPixel(x, y, sand);
						break;
				}
			}
		}

		worldSprite.texture.Apply();
	}
}
