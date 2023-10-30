using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Security.Cryptography;

public class SaveManager : Singleton<SaveManager>
{
    [HideInInspector] public LocalGameData localGameData;

    public override void Awake()
    {
        base.Awake();
        Load();
    }
    public void Save()
    {
        string jsonString = DataToJson(localGameData);
        SaveFile(jsonString);
    }

    public LocalGameData Load()
    {
        //������ �����ϴ������� üũ.
        if (!File.Exists(GetPath()))
        {
            localGameData = new LocalGameData();
            localGameData.Init();
            Debug.Log("���̺� ������ �������� ����.");
            return localGameData;
        }

        string encryptData = LoadFile(GetPath());

        Debug.Log(encryptData);

        localGameData = JsonToData(encryptData);
        return localGameData;
    }


    static string DataToJson(LocalGameData sd)
    {
        string jsonData = JsonUtility.ToJson(sd);
        return jsonData;
    }

    static LocalGameData JsonToData(string jsonData)
    {
        LocalGameData sd = JsonUtility.FromJson<LocalGameData>(jsonData);
        return sd;
    }

    static void SaveFile(string jsonData)
    {
        using (FileStream fs = new FileStream(GetPath(), FileMode.Create, FileAccess.Write))
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(jsonData);

            //bytes�� ���빰�� 0 ~ max ���̱��� fs�� ����
            fs.Write(bytes, 0, bytes.Length);
            fs.Close();
        }
    }

    static string LoadFile(string path)
    {
        using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
        {
            byte[] bytes = new byte[(int)fs.Length];

            fs.Read(bytes, 0, (int)fs.Length);
            string jsonString = System.Text.Encoding.UTF8.GetString(bytes);
            fs.Close();
            return jsonString;
        }
    }

    static string GetPath()
    {
        string filePath = string.Empty;
        string gameDataFileName = "save.txt";

        filePath = Application.persistentDataPath + "/" + gameDataFileName;

        return Path.Combine(filePath);
    }

}
public class LocalGameData
{
    //��������
    public int nStageLevel;
    public List<int> lisStageStar = new List<int>();

    //������ ����
    public int nLollipop;
    public int nAll;
    public int nSwitch;
    public int nColorBomb;

    //����
    public float fSounds;
    public float fMusic;

    public int nSoundsMute;
    public int nMusicMute;

    public void Init()
    {
        nStageLevel = 1;

        nLollipop = 10;
        nAll = 10;
        nSwitch = 10;
        nColorBomb = 10;

        fSounds = 100f;
        fMusic = 100f;

        nSoundsMute = 0;
        nMusicMute = 0;
    }

}