using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Json ��ȯ ���� Ŭ����
/// </summary>
public class JsonHelper
{
    /// <summary>
    /// Json ���ڿ��� ������ �迭 ���·� ��ȯ
    /// </summary>
    /// <typeparam name="T">Ŭ���� Ÿ��</typeparam>
    /// <param name="json">Json ���ڿ�</param>
    /// <returns>��ȯ�� �迭 ������Ʈ</returns>
    public static T[] FromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.Items;
    }

    /// <summary>
    /// �迭 ������Ʈ �����͸� Json ���ڿ��� ��ȯ
    /// </summary>
    /// <typeparam name="T">Ŭ���� Ÿ��</typeparam>
    /// <param name="array">�迭 ������Ʈ</param>
    /// <returns>Json ���ڿ�</returns>
    public static string ToJson<T>(T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper);
    }

    /// <summary>
    /// �迭 ������Ʈ �����͸� ���� ���� ������ Json ���ڿ��� ��ȯ
    /// </summary>
    /// <typeparam name="T">Ŭ���� Ÿ��</typeparam>
    /// <param name="array">�迭 ������Ʈ</param>
    /// <param name="prettyPrint">���� ���� ���·� ��ȯ���� ����</param>
    /// <returns>Json ���ڿ�</returns>
    public static string ToJson<T>(T[] array, bool prettyPrint)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }

    /// <summary>
    /// Json�� �迭�� �迭 ������Ʈ�� ��ȯ���ִ� Ŭ����
    /// </summary>
    /// <typeparam name="T">Ŭ���� Ÿ��</typeparam>
    [Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }
}
