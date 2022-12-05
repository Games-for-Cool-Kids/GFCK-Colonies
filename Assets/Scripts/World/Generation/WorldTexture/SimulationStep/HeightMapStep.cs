using UnityEngine;

// Using DiamondSquare algorithm
[CreateAssetMenu(menuName = "GameWorld/SimulationStep/HeightMap")]
public class HeightMapStep : ScriptableObject
{
	public float grain = 20;
	public float midPoint = 0.5f;
	public bool invertHeight = false;

	public void ApplyTo(WorldVariable worldVariable)
    {
		GenerateHeightMap(worldVariable, worldVariable.size, grain);
	}

	float OffsetValue(float current, float total, float grain)
	{
		float max = current / total * grain;
		return Random.Range(-.5f, .5f) * max;
	}

	public void GenerateHeightMap(WorldVariable worldVariable, int length, float grain)
	{
		float c1, c2, c3, c4;

		c1 = Mathf.Lerp(Random.value, midPoint, .5f);
		c2 = Mathf.Lerp(Random.value, midPoint, .5f);
		c3 = Mathf.Lerp(Random.value, midPoint, .5f);
		c4 = Mathf.Lerp(Random.value, midPoint, .5f);

		DivideGrid(worldVariable, 0, 0, length, length, c1, c2, c3, c4, grain, length + length);
	}

	void DivideGrid(WorldVariable worldVariable, float x, float y, float width, float height, float c1, float c2, float c3, float c4, float _grain, float total)
	{
		float _width = width * .5f;
		float _height = height * .5f;

		if (_width < 1.0f || _height < 1.0f)
		{
			float c = (c1 + c2 + c3 + c4) * .25f;
			if (invertHeight)
				c = 1 - c;
			worldVariable.blockGrid[(int)x, (int)y].height = c;
		}
		else
		{
			float m = (c1 + c2 + c3 + c4) * .25f + OffsetValue(_width + _height, total, _grain);
			float e1 = (c1 + c2) * .5f;
			float e2 = (c2 + c3) * .5f;
			float e3 = (c3 + c4) * .5f;
			float e4 = (c4 + c1) * .5f;

			m = Mathf.Clamp(m, 0.0f, 1.0f);

			DivideGrid(worldVariable, x, y, _width, _height, c1, e1, m, e4, _grain, total);
			DivideGrid(worldVariable, x + _width, y, _width, _height, e1, c2, e2, m, _grain, total);
			DivideGrid(worldVariable, x + _width, y + _height, _width, _height, m, e2, c3, e3, _grain, total);
			DivideGrid(worldVariable, x, y + _height, _width, _height, e4, m, e3, c4, _grain, total);
		}
	}
}
