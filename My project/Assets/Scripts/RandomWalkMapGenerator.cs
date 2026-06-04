using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomWalkMapGenerator : MonoBehaviour
{
    

    [SerializeField] protected Vector2Int startPosition =  Vector2Int.zero;

    [SerializeField] private int iterations = 2500;
    [SerializeField] public int walkLength = 120;
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

        List<PointOfInterest> pointsOfInterest = GeneratePointsOfInterest(floorPositions);

        foreach (var poi in pointsOfInterest) {
            Debug.Log("POI: " + poi.type + " at " + poi.center);
            GeneratePointOfInterest(poi, floorPositions);
        }
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

    // POI generation code
    [SerializeField] private int islandCount = 10;
    [SerializeField] private int islandWalkLength = 30;
    [SerializeField] private int islandIterations = 8;
    [SerializeField] private int minPOISeparation = 15;
    [SerializeField] private int islandEdgePadding = 15;
    [SerializeField] private int spawnSafeRadius = 12;

    public enum POIType {
        Island, //25
        Rock, //45
        VillageIsland, //10
        Shipwreck, //3
        Crate, //7
        Shoal, //10

    }
    public struct PointOfInterest {
        public Vector2Int center;
        public POIType type;

        public PointOfInterest(Vector2Int center, POIType type) {
            this.center = center;
            this.type = type;
        }
    }

    private List<PointOfInterest> GeneratePointsOfInterest(HashSet<Vector2Int> oceanPositions) {
        List<Vector2Int> centers = PickIslandCenters(
            oceanPositions,
            islandCount,
            minPOISeparation,
            islandEdgePadding
        );

        List<PointOfInterest> points = new List<PointOfInterest>();

        foreach (var center in centers) {
            POIType type = GetRandomPOIType();
            points.Add(new PointOfInterest(center, type));
        }

        return points;
    }

    private POIType GetRandomPOIType() {
        int random = Random.Range(0, 99);
        if (random >= 0 && random < 25) {
            return POIType.Island;
        } else if (random >= 25 && random < 70) {
            return POIType.Rock;
        } else if (random >= 70 && random < 80) {
            return POIType.VillageIsland;
        } else if (random >= 80 && random < 83) {
            return POIType.Shipwreck;
        } else if (random >= 83 && random < 90) {
            return POIType.Crate;
        } else /*if (random >= 90 && random < 100)*/ {
            return POIType.Shoal;
        }
    }

    private List<Vector2Int> PickIslandCenters(
    HashSet<Vector2Int> oceanPositions,
    int islandCount,
    int minDistanceBetweenIslands,
    int edgePadding
) {
        List<Vector2Int> centers = new List<Vector2Int>();
        List<Vector2Int> possiblePositions = oceanPositions.ToList();

        int attempts = 0;
        int maxAttempts = islandCount * 100;

        while (centers.Count < islandCount && attempts < maxAttempts) {
            attempts++;

            Vector2Int candidate = possiblePositions[Random.Range(0, possiblePositions.Count)];

            if (Vector2Int.Distance(candidate, startPosition) < spawnSafeRadius)
                continue;
            if (!IsFarFromEdge(oceanPositions, candidate, edgePadding))
                continue;

            bool tooClose = false;

            foreach (var center in centers) {
                if (Vector2Int.Distance(candidate, center) < minDistanceBetweenIslands) {
                    tooClose = true;
                    break;
                }
            }

            if (!tooClose) {
                centers.Add(candidate);
            }
        }

        return centers;
    }
    private bool IsFarFromEdge(HashSet<Vector2Int> positions, Vector2Int center, int padding) {
        for (int x = -padding; x <= padding; x++) {
            for (int y = -padding; y <= padding; y++) {
                Vector2Int checkPos = center + new Vector2Int(x, y);

                if (!positions.Contains(checkPos)) {
                    return false;
                }
            }
        }

        return true;
    }

    // POI Generation Code
    private void GeneratePointOfInterest(PointOfInterest poi, HashSet<Vector2Int> oceanPositions) {
        if (poi.type == POIType.Island) {
            GenerateIsland(poi.center, oceanPositions);
        } else if (poi.type == POIType.Rock) {
            GenerateRock(poi.center, oceanPositions);
        } else {
            Debug.Log("No generation code yet for: " + poi.type);
        }
    }

    private void GenerateRock(Vector2Int center, HashSet<Vector2Int> oceanPositions) {
        if (oceanPositions.Contains(center)) {
            tilemapVisualiser.PaintRockTile(center);
        }
    }
    private HashSet<Vector2Int> GenerateIsland(Vector2Int center, HashSet<Vector2Int> oceanPositions) {
        HashSet<Vector2Int> islandPositions = new HashSet<Vector2Int>();

        Vector2Int currentPosition = center;

        for (int i = 0; i < islandIterations; i++) {
            var path = ProceduralGenerationScript.SimpleRandomWalk(currentPosition, islandWalkLength);

            foreach (var pos in path) {
                if (oceanPositions.Contains(pos)) {
                    islandPositions.Add(pos);
                }
            }

            currentPosition = path.ElementAt(Random.Range(0, path.Count));
        }

        islandPositions = FillIslandHoles(islandPositions);
        islandPositions = FillIslandHoles(islandPositions);
        islandPositions = FillIslandHoles(islandPositions);
        islandPositions = RemoveTinyBits(islandPositions);
        islandPositions = RemoveTinyBits(islandPositions);

        tilemapVisualiser.PaintLandTiles(islandPositions);

        return islandPositions;
    }
    private HashSet<Vector2Int> FillIslandHoles(HashSet<Vector2Int> islandPositions) {
        HashSet<Vector2Int> result = new HashSet<Vector2Int>(islandPositions);

        foreach (var position in GetBoundsPositions(islandPositions)) {
            if (islandPositions.Contains(position))
                continue;

            int cardinalNeighbours = CountCardinalNeighbours(islandPositions, position);
            int allNeighbours = CountNeighbours(islandPositions, position);

            if (cardinalNeighbours >= 2 && allNeighbours >= 5) {
                result.Add(position);
            }
        }

        return result;
    }


    // New Ocean Generation Code
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
