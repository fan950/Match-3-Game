using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;

public class AddMatchTools : EditorWindow
{
    private enum eGoalType
    {
        Tile,
        Element
    }
    private string sLevel = "0";
    private string sWidth = "0";
    private string sHeight = "0";

    private int nLevel = 0;
    private int nWidth = 0;
    private int nHeight = 0;

    private string sScore1 = "0";
    private string sScore2 = "0";
    private string sScore3 = "0";

    private string sGoal = "0";
    private string sMove = "0";

    private eElementType elementType;
    private eTileType tileType;
    private eGoalType goalType;

    private Dictionary<int, eTileType> dicTileType = new Dictionary<int, eTileType>();
    private Dictionary<int, eElementType> dicElementType = new Dictionary<int, eElementType>();
    private Dictionary<int, Goal> dicGoal = new Dictionary<int, Goal>();

    private Vector2 scrollPos = Vector2.zero;
    [MenuItem("Tools/AddMatchTools")]
    public static void ShowWindow()
    {
        AddMatchTools build = (AddMatchTools)GetWindow(typeof(AddMatchTools));
        build.Show();
        build.titleContent.text = "AddMatchTools";
        build.minSize = new Vector2(700f, 700f);
    }

    void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        GUILayout.Space(10);

        if (GUILayout.Button("Open", GUILayout.Width(100), GUILayout.Height(50)))
        {
            string path = EditorUtility.OpenFilePanel("Open level", Application.dataPath + "/Editor/Resources/Level", "json");
            if (!string.IsNullOrEmpty(path))
            {
                Level level = LoadJsonFile(path);
                sLevel = level.nLevel.ToString();
                sWidth = level.nWidth.ToString();
                sHeight = level.nHeight.ToString();

                sScore1 = level.nScore1.ToString();
                sScore2 = level.nScore2.ToString();
                sScore3 = level.nScore3.ToString();

                dicTileType.Clear();
                for (int i = 0; i < level.lisTileType.Count; ++i)
                {
                    dicTileType.Add(i, level.lisTileType[i]);
                }
                dicElementType.Clear();
                for (int i = 0; i < level.lisElementType.Count; ++i)
                {
                    dicElementType.Add(i, level.lisElementType[i]);
                }
                dicGoal.Clear();
                for (int i = 0; i < level.lisGoal.Count; ++i)
                {
                    dicGoal.Add(i, level.lisGoal[i]);
                }
                sGoal = dicGoal.Count.ToString();
                sMove = level.nMove.ToString();
            }
        }
        GUILayout.Space(10);
        if (GUILayout.Button("Save", GUILayout.Width(100), GUILayout.Height(50)))
        {
            string path = Application.dataPath + "/Resources/Level/";
            Level level = new Level();
            level.nLevel = nLevel;

            level.nWidth = nWidth;
            level.nHeight = nHeight;

            level.nScore1 = int.Parse(sScore1);
            level.nScore2 = int.Parse(sScore2);
            level.nScore3 = int.Parse(sScore3);

            level.nMove = int.Parse(sMove);

            level.SetTileType(dicTileType, dicElementType);
            level.SetGoal(dicGoal);

            SaveJsonFile(path, level);
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(10);
        GUILayout.Label("Level", EditorStyles.boldLabel);
        sLevel = EditorGUILayout.TextField("Level", sLevel, GUILayout.Width(300));
        GUILayout.Space(30);

        GUILayout.BeginHorizontal();

        GUILayout.BeginVertical();
        GUILayout.Label("Goal", EditorStyles.boldLabel);
        sGoal = EditorGUILayout.TextField("Goal Count", sGoal, GUILayout.Width(300));
        GUILayout.Space(10);
        if (sGoal != "")
        {
            if (dicGoal.Count != int.Parse(sGoal))
                dicGoal.Clear();
            goalType = (eGoalType)EditorGUILayout.EnumPopup(goalType, GUILayout.Width(200));

            for (int i = 0; i < int.Parse(sGoal); ++i)
            {
                if (!dicGoal.ContainsKey(i))
                {
                    Goal goal = new Goal();
                    dicGoal.Add(i, goal);
                }
                if (goalType == eGoalType.Tile)
                {
                    dicGoal[i].tileType = (eTileType)EditorGUILayout.EnumPopup(dicGoal[i].tileType, GUILayout.Width(200));
                    dicGoal[i].isTile = true;

                    if (dicGoal[i].tileType != eTileType.None && dicGoal[i].tileType != eTileType.Chocolate)
                    {
                        string _sCount = EditorGUILayout.TextField("Count", dicGoal[i].nCount.ToString(), GUILayout.Width(300));
                        dicGoal[i].nCount = int.Parse(_sCount);
                    }
                }
                else
                {
                    dicGoal[i].elementType = (eElementType)EditorGUILayout.EnumPopup(dicGoal[i].elementType, GUILayout.Width(200));
                    dicGoal[i].isTile = false;

                    if (dicGoal[i].elementType != eElementType.None)
                    {
                        string _sCount = EditorGUILayout.TextField("Count", dicGoal[i].nCount.ToString(), GUILayout.Width(300));
                        dicGoal[i].nCount = int.Parse(_sCount);
                    }
                }
            }
        }
        GUILayout.EndVertical();

        GUILayout.BeginVertical();
        GUILayout.Label("Score", EditorStyles.boldLabel);
        sScore1 = EditorGUILayout.TextField("Score1", sScore1, GUILayout.Width(300));
        sScore2 = EditorGUILayout.TextField("Score2", sScore2, GUILayout.Width(300));
        sScore3 = EditorGUILayout.TextField("Score3", sScore3, GUILayout.Width(300));

        GUILayout.EndVertical();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical();
        GUILayout.Space(30);

        GUILayout.Label("Match", EditorStyles.boldLabel);
        sWidth = EditorGUILayout.TextField("Width", sWidth, GUILayout.Width(300));
        sHeight = EditorGUILayout.TextField("Height", sHeight, GUILayout.Width(300));

        if (sWidth == "" || sHeight == "")
            return;

        if (sLevel != "")
            nLevel = int.Parse(sLevel);

        nWidth = int.Parse(sWidth);
        nHeight = int.Parse(sHeight);
        GUILayout.EndVertical();
        GUILayout.BeginVertical();
        GUILayout.Space(20);
        GUILayout.Label("Move", EditorStyles.boldLabel);
        sMove = EditorGUILayout.TextField("Move", sMove, GUILayout.Width(300));
        GUILayout.EndVertical();

        GUILayout.EndHorizontal();

        GUILayout.Space(10);
        GUILayout.Label("ElementType", EditorStyles.boldLabel);
        elementType = (eElementType)EditorGUILayout.EnumPopup(elementType, GUILayout.Width(200));
        GUILayout.Space(10);
        GUILayout.Label("TileType", EditorStyles.boldLabel);
        tileType = (eTileType)EditorGUILayout.EnumPopup(tileType, GUILayout.Width(200));
        GUILayout.Space(20);

        for (int i = 0; i < nHeight; ++i)
        {
            GUILayout.BeginHorizontal();
            for (int j = 0; j < nWidth; ++j)
            {
                int _nIndex = (i * nWidth) + j;
                if (!dicTileType.ContainsKey(_nIndex))
                    dicTileType.Add(_nIndex, eTileType.None);

                if (!dicElementType.ContainsKey(_nIndex))
                    dicElementType.Add(_nIndex, eElementType.None);

                string texturePath = string.Empty;
                if (dicElementType[_nIndex] == eElementType.None)
                    texturePath = "Tile/" + dicTileType[_nIndex].ToString();
                else
                    texturePath = "Tile/" + dicTileType[_nIndex].ToString() + "_" + dicElementType[_nIndex].ToString();

                Texture texture = Resources.Load(texturePath) as Texture;
                if (GUILayout.Button(texture, GUILayout.Width(50), GUILayout.Height(50)))
                {
                    dicTileType[_nIndex] = tileType;
                    dicElementType[_nIndex] = elementType;
                }
            }
            GUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();
        GUILayout.Space(30);
    }

    public static void SaveJsonFile(string sPath, Level level)
    {
        if (!Directory.Exists(sPath))
        {
            Directory.CreateDirectory(sPath);
        }

        string saveJson = JsonUtility.ToJson(level);

        string saveFilePath = sPath + level.nLevel + ".json";
        File.WriteAllText(saveFilePath, saveJson);
    }

    public static Level LoadJsonFile(string sPath)
    {
        if (!File.Exists(sPath))
        {
            Debug.LogError("No such saveFile exists");
            return null;
        }

        string saveFile = File.ReadAllText(sPath);
        Level saveData = JsonUtility.FromJson<Level>(saveFile);
        return saveData;
    }
}