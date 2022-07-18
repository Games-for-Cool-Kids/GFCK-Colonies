using UnityEngine;

public class WorldTextureGenerator : MonoBehaviour
{
	public SpriteRenderer spriteRenderer;
	public Sprite worldBaseSprite;
	Sprite worldSprite;

	public Color ground;
	public Color water;

	public WorldTextureNode[,] grid;
	int maxX;
	int maxY;

	public int initialWaterPercentage = 40;

	public SimulationStep currentStep;

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
					node.isGround = true;
					worldSprite.texture.SetPixel(x, y, ground);
				}
				else
				{
					node.isGround = false;
					worldSprite.texture.SetPixel(x, y, water);
				}
			}
		}

		worldSprite.texture.Apply();

		spriteRenderer.sprite = worldSprite;
	}

	public void Step()
	{
		var cloneGrid = new WorldTextureNode[maxX, maxY];
		System.Array.Copy(grid, cloneGrid, grid.Length);

		for (int x = 0; x < maxX; x++)
		{
			for (int y = 0; y < maxY; y++)
			{
				WorldTextureNode node = grid[x, y];

				bool isAlive = currentStep.isAlive(node, cloneGrid, maxX, maxY);
				if (isAlive)
				{
					node.isGround = true;
					worldSprite.texture.SetPixel(x, y, ground);
				}
				else
				{
					node.isGround = false;
					worldSprite.texture.SetPixel(x, y, water);
				}
			}
		}

		worldSprite.texture.Apply();
	}

	public void ApplySteps(int amount)
	{
		for (int i = 0; i < amount; i++)
		{
			Step();
		}
	}
}
