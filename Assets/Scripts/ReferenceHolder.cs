using System;
using System.Collections.Generic;
using UnityEngine;

public class ReferenceHolder : MonoBehaviour
{
    private static ReferenceHolder _instance;
    public static ReferenceHolder Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<ReferenceHolder>();
            return _instance;
        }
    }

    private static Dictionary<Type, object> _objectDictionary = new();
    private static Dictionary<Type, List<Action<object>>> _actionListDictionary = new();

    public static bool TryRegister<T>(T obj) where T : class
    {
        if (_objectDictionary.ContainsKey(typeof(T)))
        {
            Debug.LogError($"({typeof(T)}) is already exists");
            return false;
        }

        _objectDictionary.Add(typeof(T), obj);

        if (_actionListDictionary == null)
            return true;

        if (!_actionListDictionary.ContainsKey(typeof(T)))
            return true;

        foreach (var action in _actionListDictionary[typeof(T)])
            action?.Invoke(obj);

        _actionListDictionary.Remove(typeof(T));

        return true;
    }

    public static void Request<T>(Action<T> action)
    {
        if (_objectDictionary == null)
            _objectDictionary = new Dictionary<Type, object>();

        if (_objectDictionary.ContainsKey(typeof(T)))
        {
            action?.Invoke((T)_objectDictionary[typeof(T)]);
            return;
        }

        if (_actionListDictionary == null)
            _actionListDictionary = new Dictionary<Type, List<Action<object>>>();

        if (!_actionListDictionary.ContainsKey(typeof(T)))
            _actionListDictionary.Add(typeof(T), new List<Action<object>>());

        _actionListDictionary[typeof(T)].Add((o) => action?.Invoke((T)o));
    }

}