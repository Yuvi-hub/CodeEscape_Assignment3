
using System.Collections.Generic;
using UnityEngine;

public class CodeEscapeGame : MonoBehaviour
{
    private enum Phase { Menu, Playing, Won, Lost, Finished }

    private Phase phase = Phase.Menu;

    private readonly string[] maps =
    {
        "#########\n#P..C..E#\n#.#.###.#\n#.......#\n#########",
        "###########\n#P..#...E.#\n#.#.#.##..#\n#.#...C...#\n#...#.....#\n###########",
        "#############\n#P..#......E#\n#.#.#.####..#\n#.#...C..#..#\n#...##.#....#\n#..H...#C...#\n#############",
        "###############\n#P..#.......E.#\n#.#.#.#####.#.#\n#.#...C...#...#\n#...##.#H.#.#.#\n#C.....#...#C.#\n#...H......#..#\n###############"
    };

    private readonly string[] names =
    {
        "Tutorial: First Command",
        "Easy: Branching Paths",
        "Medium: Debug the Maze",
        "Hard: Final Compilation"
    };

    private readonly string[] lessons =
    {
        "Collect the token, then reach the exit. Use WASD, arrows or touch.",
        "Navigate the maze and collect the code token.",
        "Collect both tokens. Avoid the red bug and protect your lives.",
        "Collect all three tokens and avoid the moving bugs."
    };

    private readonly List<GameObject> spawned = new List<GameObject>();
    private readonly List<Vector2Int> hazards = new List<Vector2Int>();
    private readonly List<Vector2Int> hazardStarts = new List<Vector2Int>();
    private readonly List<GameObject> hazardObjects = new List<GameObject>();
    private readonly List<int> hazardDirections = new List<int>();

    private char[,] board;
    private int width;
    private int height;
    private int level;
    private int collected;
    private int required;
    private int steps;
    private int lives;
    private int bestUnlocked;

    private Vector2Int player;
    private Vector2Int spawnPoint;
    private Vector2Int exit;

    private GameObject playerObject;
    private float moveTimer;
    private float hazardTimer;

    private GUIStyle heading;
    private GUIStyle textStyle;
    private GUIStyle buttonStyle;
    private Texture2D white;

    private readonly Color background = new Color(.07f, .10f, .18f);
    private readonly Color wallColor = new Color(.18f, .28f, .45f);
    private readonly Color floorColor = new Color(.12f, .17f, .28f);

    private void Awake()
    {
        Application.targetFrameRate = 60;

        bestUnlocked = Mathf.Clamp(
            PlayerPrefs.GetInt("CodeEscapeUnlocked", 0), 0, 3
        );

        white = new Texture2D(1, 1);
        white.SetPixel(0, 0, Color.white);
        white.Apply();

        SetupCamera();
    }

    private void SetupCamera()
    {
        Camera cam = Camera.main;

        if (cam == null)
        {
            GameObject cameraObject = new GameObject("Main Camera");
            cam = cameraObject.AddComponent<Camera>();
            cameraObject.tag = "MainCamera";
        }

        cam.orthographic = true;
        cam.orthographicSize = 10;
        cam.transform.position = new Vector3(7, -4, -20);
        cam.transform.rotation = Quaternion.identity;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = background;
    }

    private void InitializeGUI()
    {
        if (heading != null) return;

        heading = new GUIStyle(GUI.skin.label);
        heading.fontSize = 26;
        heading.fontStyle = FontStyle.Bold;
        heading.alignment = TextAnchor.MiddleCenter;
        heading.wordWrap = true;
        heading.normal.textColor = Color.white;

        textStyle = new GUIStyle(GUI.skin.label);
        textStyle.fontSize = 17;
        textStyle.alignment = TextAnchor.MiddleCenter;
        textStyle.wordWrap = true;
        textStyle.normal.textColor = Color.white;

        buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.fontSize = 18;
        buttonStyle.fontStyle = FontStyle.Bold;
        buttonStyle.alignment = TextAnchor.MiddleCenter;
    }

    private void StartLevel(int index)
    {
        if (index < 0 || index >= maps.Length) return;

        foreach (GameObject item in spawned)
        {
            if (item != null) Destroy(item);
        }

        spawned.Clear();
        hazards.Clear();
        hazardStarts.Clear();
        hazardObjects.Clear();
        hazardDirections.Clear();

        level = index;
        collected = 0;
        required = 0;
        steps = 0;
        lives = 3;

        string[] rows = maps[level].Split('\n');

        height = rows.Length;
        width = rows[0].Length;
        board = new char[width, height];

        for (int y = 0; y < height; y++)
        {
            if (rows[y].Length != width)
            {
                Debug.LogError("Invalid map width on level " + level);
                phase = Phase.Menu;
                return;
            }

            for (int x = 0; x < width; x++)
            {
                char c = rows[y][x];
                Vector2Int cell = new Vector2Int(x, y);

                board[x, y] = c;

                if (c == 'P')
                {
                    player = cell;
                    spawnPoint = cell;
                    board[x, y] = '.';
                }
                else if (c == 'E')
                {
                    exit = cell;
                }
                else if (c == 'C')
                {
                    required++;
                }
                else if (c == 'H')
                {
                    hazards.Add(cell);
                    hazardStarts.Add(cell);
                    hazardDirections.Add(1);
                    board[x, y] = '.';
                }
            }
        }

        playerObject = Spawn(
            "Player", player, new Color(.20f, .84f, .97f)
        );

        Spawn("Exit", exit, new Color(.25f, .85f, .48f));

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (board[x, y] == 'C')
                {
                    Spawn(
                        "Token",
                        new Vector2Int(x, y),
                        new Color(1f, .77f, .22f)
                    );
                }
            }
        }

        for (int i = 0; i < hazards.Count; i++)
        {
            GameObject obj = Spawn(
                "Hazard",
                hazards[i],
                new Color(.95f, .29f, .38f)
            );

            hazardObjects.Add(obj);
        }

        moveTimer = 0;
        hazardTimer = 0;
        phase = Phase.Playing;
    }

    private GameObject Spawn(string kind, Vector2Int cell, Color tint)
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/" + kind);

        GameObject obj = prefab != null
            ? Instantiate(prefab)
            : GameObject.CreatePrimitive(PrimitiveType.Cube);

        obj.name = kind + "_" + cell.x + "_" + cell.y;
        obj.transform.position = CellPosition(cell);
        obj.transform.localScale = Vector3.one * .7f;

        Renderer renderer = obj.GetComponent<Renderer>();

        if (renderer != null)
        {
            renderer.material.color = tint;
        }

        spawned.Add(obj);
        return obj;
    }

    private Vector3 CellPosition(Vector2Int cell)
    {
        return new Vector3(cell.x, -cell.y, 0);
    }

    private void Update()
    {
        if (phase != Phase.Playing) return;

        moveTimer -= Time.deltaTime;
        hazardTimer += Time.deltaTime;

        if (moveTimer <= 0)
        {
            Vector2Int dir = Vector2Int.zero;

            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
                dir = Vector2Int.left;
            else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
                dir = Vector2Int.right;
            else if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
                dir = Vector2Int.down;
            else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
                dir = Vector2Int.up;

            if (dir != Vector2Int.zero)
            {
                Move(dir);
                moveTimer = .16f;
            }
        }

        if (level == 3 && hazardTimer >= .75f)
        {
            hazardTimer = 0;
            MoveHazards();
        }
    }

    private void MoveHazards()
    {
        for (int i = 0; i < hazards.Count; i++)
        {
            Vector2Int direction = new Vector2Int(hazardDirections[i], 0);
            Vector2Int next = hazards[i] + direction;

            if (!HazardCanMove(next, i))
            {
                hazardDirections[i] *= -1;
                direction = new Vector2Int(hazardDirections[i], 0);
                next = hazards[i] + direction;
            }

            if (HazardCanMove(next, i))
            {
                hazards[i] = next;
            }

            if (hazardObjects[i] != null)
            {
                hazardObjects[i].transform.position = CellPosition(hazards[i]);
            }
        }

        CheckHazard();
    }

    private bool HazardCanMove(Vector2Int cell, int current)
    {
        if (!Walkable(cell)) return false;
        if (cell == exit) return false;
        if (board[cell.x, cell.y] == 'C') return false;

        for (int i = 0; i < hazards.Count; i++)
        {
            if (i != current && hazards[i] == cell) return false;
        }

        return true;
    }

    private bool Walkable(Vector2Int cell)
    {
        return cell.x >= 0 &&
               cell.y >= 0 &&
               cell.x < width &&
               cell.y < height &&
               board[cell.x, cell.y] != '#';
    }

    private void Move(Vector2Int dir)
    {
        if (phase != Phase.Playing) return;

        Vector2Int next = player + dir;

        if (!Walkable(next)) return;

        player = next;
        steps++;

        if (playerObject != null)
        {
            playerObject.transform.position = CellPosition(player);
        }

        if (board[player.x, player.y] == 'C')
        {
            board[player.x, player.y] = '.';
            collected++;

            foreach (GameObject item in spawned)
            {
                if (item != null &&
                    item.name == "Token_" + player.x + "_" + player.y)
                {
                    Destroy(item);
                    break;
                }
            }
        }

        CheckHazard();

        if (phase != Phase.Playing) return;

        if (player == exit && collected == required)
        {
            CompleteLevel();
        }
    }

    private void CompleteLevel()
    {
        phase = level == 3 ? Phase.Finished : Phase.Won;

        bestUnlocked = Mathf.Max(
            bestUnlocked,
            Mathf.Min(3, level + 1)
        );

        PlayerPrefs.SetInt("CodeEscapeUnlocked", bestUnlocked);
        PlayerPrefs.SetInt("CodeEscapeLastSteps", steps);
        PlayerPrefs.Save();
    }

    private void CheckHazard()
    {
        foreach (Vector2Int hazard in hazards)
        {
            if (hazard != player) continue;

            lives--;

            if (lives <= 0)
            {
                phase = Phase.Lost;
            }
            else
            {
                player = spawnPoint;
            }

            if (playerObject != null)
            {
                playerObject.transform.position = CellPosition(player);
            }

            break;
        }
    }

    private void Fill(Rect rect, Color color)
    {
        Color previous = GUI.color;
        GUI.color = color;
        GUI.DrawTexture(rect, white);
        GUI.color = previous;
    }

    private bool Action(Rect rect, string caption)
    {
        return GUI.Button(rect, caption, buttonStyle);
    }

    private void OnGUI()
    {
        InitializeGUI();

        float scale = Mathf.Min(
            Screen.width / 900f,
            Screen.height / 720f
        );

        scale = Mathf.Max(scale, .01f);

        Matrix4x4 oldMatrix = GUI.matrix;
        GUI.matrix = Matrix4x4.Scale(new Vector3(scale, scale, 1));

        float w = Screen.width / scale;
        float h = Screen.height / scale;

        Fill(new Rect(0, 0, w, h), background);

        if (phase == Phase.Menu)
        {
            GUI.Label(new Rect(0, 30, w, 60), "CODE ESCAPE", heading);

            GUI.Label(
                new Rect(35, 100, w - 70, 55),
                "Find code tokens, avoid bugs, unlock the exit.",
                textStyle
            );

            for (int i = 0; i < 4; i++)
            {
                GUI.enabled = i <= bestUnlocked;

                string caption = (i + 1) + ". " + names[i];

                if (i > bestUnlocked)
                    caption += " [LOCKED]";

                if (Action(
                    new Rect(w / 2 - 210, 185 + i * 82, 420, 62),
                    caption
                ))
                {
                    StartLevel(i);
                }
            }

            GUI.enabled = true;

            GUI.Label(
                new Rect(0, h - 60, w, 35),
                "Arrow keys / WASD / touch controls",
                textStyle
            );
        }
        else if (phase == Phase.Playing)
        {
            GUI.Label(new Rect(0, 8, w, 40), names[level], heading);

            GUI.Label(
                new Rect(0, 50, w, 35),
                "Tokens: " + collected + "/" + required +
                "     Lives: " + lives +
                "     Steps: " + steps,
                textStyle
            );

            GUI.Label(
                new Rect(25, 87, w - 50, 50),
                lessons[level],
                textStyle
            );

            float tile = Mathf.Min(
                (w - 50) / width,
                (h - 330) / height
            );

            float left = (w - width * tile) / 2;
            float top = 150;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Rect cell = new Rect(
                        left + x * tile + 1,
                        top + y * tile + 1,
                        tile - 2,
                        tile - 2
                    );

                    Fill(
                        cell,
                        board[x, y] == '#' ? wallColor : floorColor
                    );

                    if (board[x, y] == 'C')
                    {
                        Fill(
                            new Rect(
                                cell.x + tile * .28f,
                                cell.y + tile * .28f,
                                tile * .42f,
                                tile * .42f
                            ),
                            new Color(1f, .78f, .2f)
                        );
                    }

                    if (exit.x == x && exit.y == y)
                    {
                        Fill(
                            new Rect(
                                cell.x + tile * .16f,
                                cell.y + tile * .16f,
                                tile * .64f,
                                tile * .64f
                            ),
                            collected == required
                                ? Color.green
                                : new Color(.2f, .5f, .35f)
                        );
                    }
                }
            }

            foreach (Vector2Int hazard in hazards)
            {
                Fill(
                    new Rect(
                        left + hazard.x * tile + tile * .2f,
                        top + hazard.y * tile + tile * .2f,
                        tile * .6f,
                        tile * .6f
                    ),
                    Color.red
                );
            }

            Fill(
                new Rect(
                    left + player.x * tile + tile * .15f,
                    top + player.y * tile + tile * .15f,
                    tile * .7f,
                    tile * .7f
                ),
                Color.cyan
            );

            float by = h - 174;

            if (Action(new Rect(w / 2 - 45, by, 90, 48), "UP"))
                Move(Vector2Int.down);

            if (Action(new Rect(w / 2 - 145, by + 54, 90, 48), "LEFT"))
                Move(Vector2Int.left);

            if (Action(new Rect(w / 2 - 45, by + 54, 90, 48), "DOWN"))
                Move(Vector2Int.up);

            if (Action(new Rect(w / 2 + 55, by + 54, 90, 48), "RIGHT"))
                Move(Vector2Int.right);

            if (Action(
                new Rect(w - 145, h - 60, 130, 45),
                "RESTART"
            ))
            {
                StartLevel(level);
            }

            if (Action(
                new Rect(15, h - 60, 115, 45),
                "MENU"
            ))
            {
                phase = Phase.Menu;
            }
        }
        else
        {
            string title = phase == Phase.Lost
                ? "GAME OVER"
                : phase == Phase.Finished
                    ? "YOU ESCAPED!"
                    : "LEVEL COMPLETE!";

            GUI.Label(
                new Rect(0, h / 2 - 145, w, 70),
                title,
                heading
            );

            GUI.Label(
                new Rect(30, h / 2 - 65, w - 60, 45),
                "Level " + (level + 1) + " | Steps: " + steps,
                textStyle
            );

            if (phase == Phase.Won &&
                Action(
                    new Rect(w / 2 - 150, h / 2 + 10, 300, 65),
                    "NEXT LEVEL"
                ))
            {
                StartLevel(level + 1);
            }

            if (phase == Phase.Lost &&
                Action(
                    new Rect(w / 2 - 150, h / 2 + 10, 300, 65),
                    "TRY AGAIN"
                ))
            {
                StartLevel(level);
            }

            if (Action(
                new Rect(w / 2 - 150, h / 2 + 95, 300, 60),
                "LEVEL SELECT"
            ))
            {
                phase = Phase.Menu;
            }
        }

        GUI.matrix = oldMatrix;
    }

    private void OnDestroy()
    {
        if (white != null)
        {
            Destroy(white);
        }
    }
}
