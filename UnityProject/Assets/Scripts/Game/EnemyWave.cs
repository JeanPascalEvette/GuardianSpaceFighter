using System;
using System.Linq;

[Serializable]
public class EnemyWave
{
	private string[] mEnemies;
	private int mRows;
    
	public int NumberOfRows { get { return mRows; } }

	public EnemyWave(string[] enemies )
	{
        int u;
		mRows = enemies.Length;
        mEnemies = new string[mRows];
        for (int i = 0; i < mRows; i++)
        {
            if (!int.TryParse(enemies[i], out u))
                continue;
            mEnemies[i] = enemies[i];
        }
    }
    
    public string GetRow(int row)
    {
        return mEnemies[row];
    }
    
}
