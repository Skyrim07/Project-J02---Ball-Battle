using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SKCell;
public class PenaltyManager : MonoSingleton<PenaltyManager>
{
    public bool isGameActive = false;
    public float currentTime;
    private float maxTime = 59;

    int width = 12, height = 19;
    public Grid grid;
    public GameObject obstaclePrefab, ball;

    private Vector3Int destination = new Vector3Int(4, 0, 16);
    private int[,] mazeValue;

    private Vector2Int[] dirs = new Vector2Int[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right};
    private void Start()
    {
        currentTime = 0;
        isGameActive = false;
        mazeValue = new int[width, height];
        UIManager.instance.SetBeginMatchMenu(true);
    }
    private void Update()
    {
        if (isGameActive)
        {
            currentTime+=Time.deltaTime;
            PenaltyReference.instance.timeText.text = $"00 : {((int)(maxTime - currentTime)).ToString("d2")}";
            if (currentTime >= maxTime)
            {
                OnTimeUp();
            }
        }
    }
    public void StartPenaltyGame()
    {
        GenerateMaze(5, 1);
        mazeValue[4, height - 1] = 1;
        mazeValue[5, height - 1] = 1;
        mazeValue[6, height - 1] = 1;
        mazeValue[7, height - 1] = 1;
        StartCoroutine( DrawMaze());
        isGameActive = true;
    }

    public void OnTimeUp()
    {
        isGameActive = false;
        PenaltyReference.instance.go_TitleText.text = "Failed!";
        PenaltyReference.instance.go_ResultText.text = "You failed the penalty game! Better luck next time!";
        UIManager.instance.SetGameOverMenu(true);
    }
    public void OnGoal()
    {
        isGameActive=false;
        PenaltyReference.instance.go_TitleText.text = "Success!";
        PenaltyReference.instance.go_ResultText.text = "You competed the penalty game! Congratulations!";
        UIManager.instance.SetGameOverMenu(true);
    }

    public void GenerateMaze(int x, int z)
    {
        if (Neighbors(x, z) >= 2)
            return;
        mazeValue[x, z] = 1;
        ShuffleList<Vector2Int>(dirs);
        GenerateMaze(x + dirs[0].x, z + dirs[0].y);
        GenerateMaze(x + dirs[1].x, z + dirs[1].y);
        GenerateMaze(x + dirs[2].x, z + dirs[2].y);
        GenerateMaze(x + dirs[3].x, z + dirs[3].y);
    }

    IEnumerator DrawMaze()
    {
        bool hasBall = false;
        for (int i = 1; i < width-1; i++)
        {
            for (int j =1; j < height-1; j++)
            {
                if (mazeValue[i, j] == 1)
                {
                    if (!hasBall)
                    {
                        float rand = Random.value;
                        if (rand < 0.4f)
                        {
                            Vector3 ballPos = grid.CellToWorld(new Vector3Int(i - 1, j - 1, 0)) + new Vector3(grid.cellSize.x / 2f, 0, grid.cellSize.z / 2f);
                            ball.transform.position = ballPos;
                            hasBall = true;
                        }
                    }
                    continue;
                }
                Vector3 pos = grid.CellToWorld(new Vector3Int(i-1, j-1, 0))+new Vector3(grid.cellSize.x/2f, 0, grid.cellSize.z/2f);
                Instantiate(obstaclePrefab, pos, Quaternion.identity);
                yield return new WaitForSeconds(0.01f);
            }
        }
    }

    public int Neighbors(int x, int z)
    {
        int count = 0;
        if (x<=0 || x >=width-1|| z <= 0 || z >=height-1)
            return 5;
        if(mazeValue[x-1, z]==1)
            count++;
        if (mazeValue[x + 1, z] == 1)
            count++;
        if (mazeValue[x , z+1] == 1)
            count++;
        if (mazeValue[x, z-1] == 1)
            count++;
        return count;
    }

    public void ShuffleList<T>(T[] list)
    {
        int n = list.Length;
        while (n > 1)
        {
            n--;
            int k =Random.Range(0, n + 1);
            T v = list[k];
            list[k] = list[n];
            list[n] = v;
        }
    }

    public void LoadStartScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Start");
    }
}
