using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Json 변환 헬퍼 클래스
/// </summary>
public class JsonHelper
{
    /// <summary>
    /// Json 문자열을 가지고 배열 형태로 변환
    /// </summary>
    /// <typeparam name="T">클래스 타입</typeparam>
    /// <param name="json">Json 문자열</param>
    /// <returns>변환된 배열 오브젝트</returns>
    public static T[] FromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.Items;
    }

    /// <summary>
    /// 배열 오브젝트 데이터를 Json 문자열로 변환
    /// </summary>
    /// <typeparam name="T">클래스 타입</typeparam>
    /// <param name="array">배열 오브젝트</param>
    /// <returns>Json 문자열</returns>
    public static string ToJson<T>(T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper);
    }

    /// <summary>
    /// 배열 오브젝트 데이터를 보기 좋은 형태의 Json 문자열로 변환
    /// </summary>
    /// <typeparam name="T">클래스 타입</typeparam>
    /// <param name="array">배열 오브젝트</param>
    /// <param name="prettyPrint">보기 좋은 형태로 변환할지 여부</param>
    /// <returns>Json 문자열</returns>
    public static string ToJson<T>(T[] array, bool prettyPrint)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }

    /// <summary>
    /// Json의 배열을 배열 오브젝트로 변환해주는 클래스
    /// </summary>
    /// <typeparam name="T">클래스 타입</typeparam>
    [Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }
}
