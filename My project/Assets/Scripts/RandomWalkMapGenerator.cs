using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomWalkMapGenerator : MonoBehaviour
{
    

    [SerializeField] protected Vector2Int startPosition =  Vector2Int.zero;

    [SerializeField] private int iterations = 10;
    [SerializeField] public int walkLength = 10;
    [SerializeField] public bool startRandomlyEachIteration = true;

    [SerializeField]
    private TilemapVisualiser tilemapVisualiser;

    public void RunProceduralGeneration() {
        HashSet<Vector2Int> floorPositions = RunRandomWalk();

        floorPositions = RemoveThinProtrusions(floorPositions, 4);
        floorPositions = SmoothPositions(floorPositions, 4);
        floorPositions = RemoveTinyBits(floorPositions);

        tilemapVisualiser.Clear();
        tilemapVisualiser.PaintOceanTiles(floorPositions);
    }

    protected HashSet<Vector2Int> RunRandomWalk() {
        var currentPosition = startPosition;
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
        for (int i = 0; i < iterations; i++) {
            var path = ProceduralGenerationScript.SimpleRandomWalk(currentPosition, walkLength);
            floorPositions.UnionWith(path);
            if (startRandomlyEachIteration) {
                currentPosition = floorPositions.ElementAt(Random.Range(0, floorPositions.Count));
            }
        }
        return floorPositions;
    }

    // new code

    private HashSet<Vector2Int> SmoothPositions(HashSet<Vector2Int> positions, int passes) {
        HashSet<Vector2Int> result = new HashSet<Vector2Int>(positions);

        for (int i = 0; i < passes; i++) {
            HashSet<Vector2Int> newResult = new HashSet<Vector2Int>(result);

            foreach (var position in GetBoundsPositions(result)) {
                int neighbours = CountNeighbours(result, position);

                if (neighbours >= 5)
                    newResult.Add(position);
                else if (neighbours <= 2)
                    newResult.Remove(position);
            }

            result = newResult;
        }

        return result;
    }

    private HashSet<Vector2Int> RemoveTinyBits(HashSet<Vector2Int> positions) {
        HashSet<Vector2Int> result = new HashSet<Vector2Int>(positions);

        foreach (var position in positions) {
            int neighbours = CountNeighbours(positions, position);

            if (neighbours <= 2)
                result.Remove(position);
        }

        return result;
    }

    private int CountNeighbours(HashSet<Vector2Int> positions, Vector2Int position) {
        int count = 0;

        for (int x = -1; x <= 1; x++) {
            for (int y = -1; y <= 1; y++) {
                if (x == 0 && y == 0)
                    continue;

                Vector2Int neighbour = position + new Vector2Int(x, y);

                if (positions.Contains(neighbour))
                    count++;
            }
        }

        return count;
    }

    private IEnumerable<Vector2Int> GetBoundsPositions(HashSet<Vector2Int> positions) {
        int minX = positions.Min(pos => pos.x) - 1;
        int maxX = positions.Max(pos => pos.x) + 1;
        int minY = positions.Min(pos => pos.y) - 1;
        int maxY = positions.Max(pos => pos.y) + 1;

        for (int x = minX; x <= maxX; x++) {
            for (int y = minY; y <= maxY; y++) {
                yield return new Vector2Int(x, y);
            }
        }
    }

    private HashSet<Vector2Int> RemoveThinProtrusions(HashSet<Vector2Int> positions, int passes) {
        HashSet<Vector2Int> result = new HashSet<Vector2Int>(positions);

        for (int i = 0; i < passes; i++) {
            HashSet<Vector2Int> newResult = new HashSet<Vector2Int>(result);

            foreach (var position in result) {
                int cardinalNeighbours = CountCardinalNeighbours(result, position);

                if (cardinalNeighbours <= 2) {
                    newResult.Remove(position);
                }
            }

            result = newResult;
        }

        return result;
    }

    private int CountCardinalNeighbours(HashSet<Vector2Int> positions, Vector2Int position) {
        int count = 0;

        foreach (var direction in Direction2D.cardinalDirectionsList) {
            if (positions.Contains(position + direction)) {
                count++;
            }
        }

        return count;
    }
}
