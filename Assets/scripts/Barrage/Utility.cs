﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility : MonoBehaviour {

    /// <summary>
    /// 不是每次都创建一个新的map，用于减少gc
    /// </summary>
    private static readonly Dictionary<object, Vector2> _randomIntervalMap = new Dictionary<object, Vector2>();

    /// <summary>
    /// 根据权重配置随机出一种结果。
    /// </summary>
    /// <param name="weightInfo"></param>
    /// <returns></returns>
    public static object RandomObjectByWeight(Dictionary<object, float> weightInfo)
    {
        object randomResult = null;
        //count the total weights.
        float weightSum = 0f;
        foreach (var item in weightInfo)
        {
            weightSum += item.Value;
        }
        //Debug.Log( "weightSum: " + weightSum );

        //value -> Vector2(min,max)
        _randomIntervalMap.Clear();
        //calculate the interval of each object.
        float currentWeight = 0f;
        foreach (var item in weightInfo)
        {
            float min = currentWeight;
            currentWeight += item.Value;
            float max = currentWeight;
            Vector2 interval = new Vector2(min, max);
            _randomIntervalMap.Add(item.Key, interval);
        }

        //random a value.
        float randomValue = UnityEngine.Random.Range(0f, weightSum);
        //Debug.Log( "randomValue: " + randomValue );
        int currentSearchCount = 0;
        foreach (var item in _randomIntervalMap)
        {
            currentSearchCount++;
            if (currentSearchCount == _randomIntervalMap.Count)
            {
                //the last interval is [closed,closed]
                if (item.Value.x <= randomValue && randomValue <= item.Value.y)
                {
                    return item.Key;
                }
            }
            else
            {
                //interval is [closed, opened)
                if (item.Value.x <= randomValue && randomValue < item.Value.y)
                {
                    randomResult = item.Key;
                }
            }
        }
        return randomResult;
    }


    /// <summary>
    /// 删除所有子节点
    /// </summary>
    /// <param name="parent"></param>
    public static void DestroyAllChildren(GameObject parent)
    {
        Transform parentTrans = parent.GetComponent<Transform>();
        for (int i = parentTrans.childCount - 1; i >= 0; i--)
        {
            GameObject child = parentTrans.GetChild(i).gameObject;
            GameObject.Destroy(child);
        }
    }
}

