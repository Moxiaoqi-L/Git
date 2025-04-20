// 简单 ObjectPool 实现示例（可使用 Unity 的内置池或第三方库）
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{
    private Queue<GameObject> pool = new Queue<GameObject>();
    private GameObject prefab;
    private Transform parent;

    public ObjectPool(GameObject prefab, int initialSize, Transform parent)
    {
        this.prefab = prefab;
        this.parent = parent;
        for (int i = 0; i < initialSize; i++) AddToPool();
    }

    private void AddToPool()
    {
        GameObject obj = Object.Instantiate(prefab, parent);
        obj.SetActive(false);
        pool.Enqueue(obj);
    }

    public GameObject Get()
    {
        if (pool.Count == 0) AddToPool();
        GameObject obj = pool.Dequeue();
        obj.SetActive(true);
        return obj;
    }

    public void ReturnAll()
    {
        // 先将所有子对象纳入队列（处理未返回的对象）
        foreach (Transform child in parent)
        {
            GameObject obj = child.gameObject;
            if (!pool.Contains(obj))
            {
                obj.SetActive(false);
                pool.Enqueue(obj); // 加入队列以便后续复用
            }
        }
        // 隐藏队列中的所有对象
        foreach (GameObject obj in pool)
        {
            obj.SetActive(false);
        }
    }
}