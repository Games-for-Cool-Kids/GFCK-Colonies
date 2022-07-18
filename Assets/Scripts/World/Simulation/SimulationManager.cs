using UnityEngine;

public class SimulationManager : MonoBehaviour
{
	public SpriteRenderer spriteRenderer;
	public Sprite worldSprite;

	private void Start()
	{
		CreateBase();
	}

	void CreateBase()
	{
		Sprite newSprite = Instantiate(worldSprite);

		for (int x = 0; x < newSprite.texture.width; x++)
		{
			for (int y = 0; y < newSprite.texture.height; y++)
			{
				if (Random.Range(0, 100) < 50)
				{
					newSprite.texture.SetPixel(x, y, Color.red);
				}
			}
		}

		newSprite.texture.Apply();

		spriteRenderer.sprite = newSprite;
	}
}
