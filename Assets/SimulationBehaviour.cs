using System.Collections;
using System.Collections.Generic;
using KDS.Neat;
using KDS.Neat.Solver;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class SimulationBehaviour : MonoBehaviour
{
    public Vector2 Direction;
    public Vector2 Position = Vector2.zero;
    public float Speed = 10;
    public float PlayerSpeed = 5;
    public bool Done = false;
    public float Fitness = 0;
    public GameObject PlayerPrefab;
    public float PlayerSize = 4;
    public GameObject Player;
    public Genome Genome;
    public INeatConfiguration Configuration;
    private GenomeSolver solver = new GenomeSolver();

    public void Start()
    {
        Direction = new Vector2(Random.Range(0.01f, 1), Random.Range(0.01f, 1));
        Direction = Direction.normalized;
        if (Direction.x == 0 || Direction.y == 0)
        {
            Direction = Vector2.one.normalized;
        }
        Player = GameObject.Instantiate(PlayerPrefab);
        var color = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
        Player.GetComponent<MeshRenderer>().material.color = color;
        this.GetComponent<MeshRenderer>().material.color = color;
        Player.transform.position = new Vector3(-10, 0, 0);
        Player.transform.localScale = new Vector3(1, PlayerSize, 1);
    }

    // Update is called once per frame
    public void Update()
    {
        float playerSizeHalf = PlayerSize / 2;
        for (int i = 0; i < 4; i++)
        {
            if (!Done)
            {
                Fitness += Time.deltaTime * 100;
                Position += Direction * Speed * Time.deltaTime;

                if (Position.x < -9)
                {
                    if (Position.y < Player.transform.position.y + playerSizeHalf &&
                        Position.y > Player.transform.position.y - playerSizeHalf)
                    {
                        Direction = new Vector2(Random.Range(0.01f, 1), Direction.y).normalized;
                    }
                }

                if (Position.x > 10)
                {
                    Direction = new Vector2(-Random.Range(0.01f, 1), Direction.y).normalized;
                }

                if (Position.y > 10)
                {
                    Direction = new Vector2(Direction.x, -Random.Range(0.01f, 1)).normalized;
                }

                if (Position.y < -10)
                {
                    Direction = new Vector2(Direction.x, Random.Range(0.01f, 1)).normalized;
                }

                if (Position.x < -10)
                {
                    Done = true;
                }

                this.transform.position = new Vector3(Position.x, Position.y, 0);

                // In Nodes
                //
                // Ball x
                // Ball y
                // Ball Direction x
                // Ball Direction y
                // My yMin
                // My yMax
                //
                // Out Nodes
                //     Go Up
                //     Go Down

                var outputs = solver.TraverseSolver(Genome, 100, new Dictionary<int, float>()
                {
                    {1, this.transform.position.x},
                    {2, this.transform.position.y},
                    {3, this.Direction.x},
                    {4, this.Direction.y},
                    {5, this.Player.transform.position.y - playerSizeHalf},
                    {6, this.Player.transform.position.y + playerSizeHalf}
                });

                if (outputs[0].Value >= 1.0f)
                {
                    float y = this.Player.transform.position.y;
                    y -= PlayerSpeed * Time.deltaTime;

                    if (y < -10 + playerSizeHalf)
                    {
                        y = -10 + playerSizeHalf;
                    }

                    this.Player.transform.position = new Vector3(this.Player.transform.position.x, y,
                        this.Player.transform.position.z);
                }

                if (outputs[1].Value >= 1.0f)
                {
                    float y = this.Player.transform.position.y;
                    y += PlayerSpeed * Time.deltaTime;

                    if (y > 10 - playerSizeHalf)
                    {
                        y = 10 - playerSizeHalf;
                    }

                    this.Player.transform.position = new Vector3(this.Player.transform.position.x, y,
                        this.Player.transform.position.z);
                }
            }
        }
    }
}
