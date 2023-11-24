using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Flags]
public enum WallState
{
    LEFT = 1, 
    RIGHT = 2, 
    UP = 4, 
    DOWN = 8,

    VISITED = 128,
}

public struct Position
{
    public int x, y;
}

public struct Neighbor
{
    public Position position;
    public WallState sharedWall;
}

public static class MazeGenerator
{
    public static WallState[,] Generate(int width, int height)
    {
        WallState[,] maze = new WallState[width, height];
        WallState initial = WallState.RIGHT | WallState.LEFT | WallState.UP | WallState.DOWN;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                maze[i, j] = initial;
            }
        }

        return RecursiveBackTracker(maze, width, height);
    }

    private static List<Neighbor> GetUnvisitedNeighbors(Position position, WallState[,] maze, int width, int height)
    {
        var list = new List<Neighbor>();
        
        if(position.x > 0) //left
        {
            if (!maze[position.x - 1, position.y].HasFlag(WallState.VISITED))
            {
                list.Add(new Neighbor
                {
                    position = new Position
                    {
                        x = position.x - 1,
                        y = position.y
                    },
                    sharedWall = WallState.LEFT
                });
            }
        }
        if (position.y > 0) //down
        {
            if (!maze[position.x, position.y - 1].HasFlag(WallState.VISITED))
            {
                list.Add(new Neighbor
                {
                    position = new Position
                    {
                        x = position.x,
                        y = position.y - 1
                    },
                    sharedWall = WallState.DOWN
                });
            }
        }
        if (position.y < height - 1) //up
        {
            if (!maze[position.x, position.y + 1].HasFlag(WallState.VISITED))
            {
                list.Add(new Neighbor
                {
                    position = new Position
                    {
                        x = position.x,
                        y = position.y + 1
                    },
                    sharedWall = WallState.UP
                });
            }
        }
        if (position.x < width - 1) //right
        {
            if (!maze[position.x + 1, position.y].HasFlag(WallState.VISITED))
            {
                list.Add(new Neighbor
                {
                    position = new Position
                    {
                        x = position.x + 1,
                        y = position.y
                    },
                    sharedWall = WallState.RIGHT
                });
            }
        }

        return list;
    }

    private static WallState[,] RecursiveBackTracker(WallState[,] maze, int width, int height)
    {
        var rng = new System.Random();
        var positionStack = new Stack<Position>();
        var position = new Position { x = rng.Next(0, width), y= rng.Next(0, height) };

        maze[position.x, position.y] |= WallState.VISITED;
        positionStack.Push(position);

        while(positionStack.Count > 0)
        {
            var current = positionStack.Pop();
            var neighbors = GetUnvisitedNeighbors(current, maze, width, height);

            if (neighbors.Count > 0)
            {
                positionStack.Push(current);

                var randomIndex = rng.Next(0, neighbors.Count);
                var randomNeighbor = neighbors[randomIndex];

                var nPosition = randomNeighbor.position;
                maze[current.x, current.y] &= ~randomNeighbor.sharedWall;
                maze[nPosition.x, nPosition.y] &= ~GetOppositeWall(randomNeighbor.sharedWall);

                maze[nPosition.x, nPosition.y] |= WallState.VISITED;

                positionStack.Push(nPosition);
            }
        }

        return maze;
    }

    public static WallState GetOppositeWall(WallState wall)
    {
        switch (wall)
        {
            case WallState.RIGHT: return WallState.LEFT;
            case WallState.LEFT: return WallState.RIGHT;
            case WallState.UP: return WallState.DOWN;
            case WallState.DOWN: return WallState.UP;
            default: return WallState.LEFT;
        }
    }
}

