using System.Collections;
using System.Collections.Generic;
using System.Linq;
using KDS.Neat;
using UnityEngine;

public class GenomeRenderer : MonoBehaviour
{
    public Genome Genome = null;

    public GameObject NodePrefab;

    public int YStart = -20;
    public int XStart = 20;
    public int YSpace = -60;

    public GameObject LinePrefab;

    private Dictionary<int, int> GetLayerInformation(List<NodeGene> genes, List<ConnectionGene> connections)
    {
        Dictionary<int, int> layerInfo = new Dictionary<int, int>();

        List<int> idsProcessed = new List<int>();
        Queue<NodeGene> nodesToProcess = new Queue<NodeGene>(genes.Where(x => x.Type == NodeGeneType.Input));
        int maxLayer = 2;
        foreach (var gene in genes)
        {
            layerInfo.Add(gene.Id, gene.Type == NodeGeneType.Input ? 1 : 2);
        }

        while (nodesToProcess.Count > 0)
        {
            var node = nodesToProcess.Dequeue();
            idsProcessed.Add(node.Id);
            int childLayer = layerInfo[node.Id] + 1;
            foreach (var con in connections.Where(x => x.InNode == node.Id))
            {
                if (!idsProcessed.Contains(con.OutNode))
                {
                    nodesToProcess.Enqueue(genes.First(x => x.Id == con.OutNode));
                }

                if (layerInfo.ContainsKey(con.OutNode))
                {
                    if (childLayer > layerInfo[con.OutNode])
                    {
                        layerInfo[con.OutNode] = childLayer;
                    }
                }
                else
                {
                    layerInfo.Add(con.OutNode, childLayer);
                }

                if (childLayer > maxLayer)
                {
                    maxLayer = childLayer;
                }
            }
        }

        foreach (var gene in genes.Where(x => x.Type == NodeGeneType.Output))
        {
            layerInfo[gene.Id] = maxLayer;
        }

        return layerInfo;
    }

    public void FixedUpdate()
    {
        if (Genome != null)
        {
            var genome = Genome;
            Genome = null;
            foreach (var comp in this.gameObject.GetComponentsInChildren<NodeDisplay>())
            {
                GameObject.Destroy(comp.gameObject);
            }
            foreach (var comp in this.gameObject.GetComponentsInChildren<LineUiRender>())
            {
                GameObject.Destroy(comp.gameObject);
            }

            var nodes = new List<NodeGene>(genome.Nodes.Values);
            var connections = new List<ConnectionGene>(genome.GetConnections().Values);

            var layerInfo = GetLayerInformation(nodes, connections);
            int maxLayer = layerInfo.Values.Max();
            int xSpace = 600 / maxLayer;

            Dictionary<int, Vector2> positions = new Dictionary<int, Vector2>();

            Dictionary<int, int> yPositions = new Dictionary<int, int>();
            foreach (var node in nodes)
            {
                int layer = layerInfo[node.Id];
                var nodeGo = GameObject.Instantiate(NodePrefab);
                nodeGo.transform.SetParent(this.gameObject.transform, false);
                if (!yPositions.ContainsKey(layer))
                {
                    yPositions.Add(layer, YStart);
                }

                int y = yPositions[layer];
                int x = XStart + ((layer - 1) * xSpace);
                positions.Add(node.Id, new Vector2(x, y));
                yPositions[layer] += YSpace;
                ((RectTransform)nodeGo.transform).anchoredPosition = new Vector2(x, y);
                nodeGo.GetComponent<NodeDisplay>().Id = node.Id;
            }

            foreach (var con in connections)
            {
                var lineGo = GameObject.Instantiate(LinePrefab);
                lineGo.transform.SetParent(this.gameObject.transform, false);
                var renderer = lineGo.GetComponent<LineUiRender>();
                renderer.PointA = positions[con.InNode];
                renderer.PointB = positions[con.OutNode];
                renderer.Color = con.Expressed ? Color.green : Color.red;
            }
        }
    }
}
