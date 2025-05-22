using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

public class SaveManagement : MonoBehaviourSingleton<SaveManagement>
{
    public static XmlSerializer _serializer;
    public static string FilePath { get; private set; }

    private void Awake()
    {
        FilePath = Application.persistentDataPath;
        Debug.Log(FilePath);
    }

    public T CastObject<T>(object input)
    {
        return (T)input;
    }

    public T ConvertObject<T>(object input)
    {
        return (T)Convert.ChangeType(input, typeof(T));
    }

    SaveManagement()
    {
        _serializer = new XmlSerializer(typeof(SaveObject));
    }


    public bool Read(out SaveObject state, string filePath)
    {
        if (File.Exists(filePath))
        {
#if UNITY_EDITOR
            //Debug.Log("Save file READ at " + filePath);
#endif

            var sr = File.ReadAllText(filePath);
            state = _serializer.Deserialize(new StringReader(sr)) as SaveObject;

            return true;

        }
        state = null;
        return false;
    }

    public static void Write(SaveObject state, string filePath)
    {
        string directory = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        StringWriter sw = new StringWriter();
        _serializer.Serialize(sw, state);
        string xml = sw.ToString();
        //Debug.Log(xml);

        var sr = File.CreateText(filePath);
        sr.WriteLine(xml);
        sr.Close();
#if UNITY_EDITOR
        //Debug.Log("Save file written at " + filePath);
#endif
    }



}
