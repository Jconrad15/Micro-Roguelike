using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path_TileGraph
{
    // This class constructs a simple path-finding compatible graph
    // of the Location. Each tile is a node. Each walkable neighbour
    // from a tile is linked via an edge connection.

    public Dictionary<Tile, Path_Node<Tile>> NodesDict { get; protected set; }

    public Path_TileGraph()
    {
        // Get map data from LocationData
        Tile[] tiles = LocationData.Instance.MapData;
        int width = LocationData.Instance.Width;
        int height = LocationData.Instance.Height;

        // Loop through all tiles of the location
        // For each tile, create a node

        // Instantiate dictionary map
        NodesDict = new Dictionary<Tile, Path_Node<Tile>>();

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Tile t = tiles[LocationData.Instance.GetIndexFromCoord(x,z)];

                Path_Node<Tile> n = new Path_Node<Tile>
                {
                    data = t
                };

                // Add to dictionary
                NodesDict.Add(t, n);
            }
        }
        //Debug.Log("Path_TileGraph: Created " + NodesDict.Count + " nodes.");

        // Now loop through all nodes
        // Create edges
        int edgeCount = 0;

        foreach (Tile t in NodesDict.Keys)
        {
            Path_Node<Tile> n = NodesDict[t];

            List<Path_Edge<Tile>> edges = new List<Path_Edge<Tile>>();

            // Get a list of neighbours for the tile
            Tile[] neighbours = t.neighbors; // Note, some of the array spots could be null.

            // If neighbour is walkable, create an edge to the relevant node.
            for (int i = 0; i < neighbours.Length; i++)
            {
                if (neighbours[i] != null && neighbours[i].isWalkable)
                {
                    // This neighbour exists and is walkable, so create an edge.

                    Path_Edge<Tile> e = new Path_Edge<Tile>();
                    e.cost = 1; // neighbours[i].MovementCost;
                    e.node = NodesDict[neighbours[i]];

                    // Add the edge to the temporary (and growable) list
                    edges.Add(e);
                    edgeCount++;
                }

                n.edges = edges.ToArray();
            }

        }
        //Debug.Log("Path_TileGraph: Created " + edgeCount + " edges");
    }
}
