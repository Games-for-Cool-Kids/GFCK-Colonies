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

	private void Start()
	{
		CreateBase();
	}

	void CreateBase()
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

				if (Random.Range(0, 100) < 20)
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
		for (int x = 0; x < maxX; x++)
		{
			for (int y = 0; y < maxY; y++)
			{
				WorldTextureNode node = grid[x, y];

				bool isAlive = IsAlive(node);
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

	public bool IsAlive(WorldTextureNode node)
	{
		bool result = false;
		int neighborCount = 0;

		for (int x = -1; x <= 1; x++)
		{
			for (int y = -1; y <= 1; y++)
			{
				int _x = x + node.x;
				int _y = y + node.y;
				if (_x == node.x && _y == node.y)
					continue;

				WorldTextureNode neighbor = GetNode(_x, _y);
				if (neighbor != null)
				{
					if (neighbor.isGround)
					{
						neighborCount++;
					}
				}
			}
		}

		if (node.isGround)
		{
			if (neighborCount < 2)
			{
				result = false;
			}

			if (neighborCount >= 2)
			{
				result = true;
			}

			if (neighborCount > 3)
			{
				result = false;
			}
		}
		else
		{
			if (neighborCount == 3)
			{
				result = true;
			}
		}

		return result;
	}

	WorldTextureNode GetNode(int x, int y)
	{
		if (x < 0 || y < 0 
		 || x >= maxX || y >= maxY)
			return null;

		return grid[x, y];
	}
}
