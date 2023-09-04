using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CZToolKit;
using Random = UnityEngine.Random;

public class TTTT : MonoBehaviour
{
    public float size;
    public float gridMinSize;
    public int count;
    public GameObject go;
    private SpaceOctree tree;

    private Dictionary<GameObject, Vector3> positions = new Dictionary<GameObject, Vector3>();

    // Start is called before the first frame update
    IEnumerator Start()
    {
        tree = new SpaceOctree(size);
        tree.gridMinSize = gridMinSize;
        if (go.activeSelf)
        {
            positions[go] = go.transform.position;
            tree.Add(go.GetComponent<Collider>());
        }
        else
        {
            List<GameObject> gos = new List<GameObject>();
            for (int i = 0; i < count; i++)
            {
                var g = Instantiate(go);
                g.SetActive(true);
                g.transform.position = new Vector3(Random.Range(-(size / 2 - 1), size / 2 - 1), Random.Range(-(size / 2 - 1), size / 2 - 1), Random.Range(-(size / 2 - 1), size / 2 - 1));
                positions[g] = g.transform.position;
                gos.Add(g);
            }

            yield return null;
            foreach (var g in gos)
            {
                tree.Add(g.GetComponent<Collider>());
            }
        }
    }

    [Sirenix.OdinInspector.Button]
    public void A()
    {
        Log.Debug(Snowflake.GenerateID());
    }
    
    private void Update()
    {
        foreach (var pair in positions)
        {
            if (pair.Key.transform.position != pair.Value)
            {
                tree.Remove(pair.Key.GetComponent<Collider>());
                tree.Add(pair.Key.GetComponent<Collider>());
            }
        }
    }

    private void OnDrawGizmos()
    {
        var oldColor = Gizmos.color;
        Gizmos.color = Color.green;
        if (tree != null)
            DrawNode(tree.root);
        Gizmos.color = oldColor;
    }

    private void DrawNode(OctreeNode node)
    {
        if (node == null)
        {
            return;
        }

        var data = node.userData as SpaceOctreeNodeData;
        Gizmos.DrawWireCube(data.bounds.center, data.bounds.size);

        for (int i = 0; i < node.children.Length; i++)
        {
            DrawNode(node.children[i]);
        }
    }
}

public class SpaceOctreeNodeData
{
    public Bounds bounds;
    public Bounds[] childBounds = new Bounds[8];
    public HashSet<Collider> colliders = new HashSet<Collider>();

    public SpaceOctreeNodeData(Bounds bounds)
    {
        this.bounds = bounds;
        var childSize = bounds.size / 2;
        var quarter = bounds.size.x / 4;
        childBounds[(int)OctreeNodeType.TopLeftFront] = new Bounds(bounds.center + new Vector3(-quarter, quarter, quarter), childSize);
        childBounds[(int)OctreeNodeType.TopRightFront] = new Bounds(bounds.center + new Vector3(quarter, quarter, quarter), childSize);
        childBounds[(int)OctreeNodeType.BottomLeftFront] = new Bounds(bounds.center + new Vector3(-quarter, -quarter, quarter), childSize);
        childBounds[(int)OctreeNodeType.BottomRightFront] = new Bounds(bounds.center + new Vector3(quarter, -quarter, quarter), childSize);
        childBounds[(int)OctreeNodeType.TopLeftBack] = new Bounds(bounds.center + new Vector3(-quarter, quarter, -quarter), childSize);
        childBounds[(int)OctreeNodeType.TopRightBack] = new Bounds(bounds.center + new Vector3(quarter, quarter, -quarter), childSize);
        childBounds[(int)OctreeNodeType.BottomLeftBack] = new Bounds(bounds.center + new Vector3(-quarter, -quarter, -quarter), childSize);
        childBounds[(int)OctreeNodeType.BottomRightBack] = new Bounds(bounds.center + new Vector3(quarter, -quarter, -quarter), childSize);
    }
}

public class SpaceOctree
{
    public OctreeNode root;
    public float gridMinSize;

    public SpaceOctree(float size)
    {
        root = new OctreeNode();
        var bounds = new Bounds(Vector3.zero, Vector3.one * size);
        var rootSpaceOctreeNodeData = new SpaceOctreeNodeData(bounds);
        root.userData = rootSpaceOctreeNodeData;
    }

    public void Add(Collider collider)
    {
        if (collider == null)
            return;

        // var data = root.userData as SpaceOctreeNodeData;
        // while (!data.bounds.Intersects(collider.bounds))
        // {
        //     // 应该扩容了
        //     // 根据要添加的go的位置，计算出新的bounds，并把旧的根节点作为新的子节点
        // }

        DivideAndAdd(root, collider);
    }

    private Dictionary<Collider, OctreeNode> nodeMap = new Dictionary<Collider, OctreeNode>();

    private void DivideAndAdd(OctreeNode node, Collider collider)
    {
        var nodeData = node.userData as SpaceOctreeNodeData;
        var intersectsMap = 0;
        var index = 0;
        if (nodeData.bounds.size.x > gridMinSize)
        {
            for (int i = 0; i < 8; i++)
            {
                var childBounds = nodeData.childBounds[i];
                if (!childBounds.Intersects(collider.bounds))
                    continue;

                index = i;
                intersectsMap |= 1 << i;
            }
        }

        if (1 << index == intersectsMap)
        {
            var child = node.children[index];
            if (child == null)
            {
                child = node.GetOrCreateChild(index);
                child.userData = new SpaceOctreeNodeData(nodeData.childBounds[index]);
            }

            DivideAndAdd(child, collider);
        }
        else
        {
            nodeMap[collider] = node;
            nodeData.colliders.Add(collider);
        }
    }

    public void Remove(Collider collider)
    {
        if (!nodeMap.TryGetValue(collider, out var node))
            return;

        nodeMap.Remove(collider);
        var nodeData = node.userData as SpaceOctreeNodeData;
        nodeData.colliders.Remove(collider);
        while (node != root)
        {
            if (nodeData.colliders.Count == 0)
            {
                var temp = node;
                node = node.parent;
                node.RemoveChild(temp);
            }
        }
    }
}