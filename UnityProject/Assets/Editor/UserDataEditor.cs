using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Reflection;

[CustomEditor(typeof(UserData))]
public class UserDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        UserData.SetCurrentLevel(EditorGUILayout.IntField("CurrentLevel", UserData.CurrentLevel));
        UserData.SetLevelUnlocked(EditorGUILayout.IntField("LevelUnlocked", UserData.LevelUnlocked));
        UserData.SetBulletSpeedLevel(EditorGUILayout.IntField("BulletSpeedLevel", UserData.BulletSpeedLevel));
        UserData.SetFireRate(EditorGUILayout.IntField("FireRateLevel", UserData.FireRateLevel));
        UserData.SetMultishotLevel(EditorGUILayout.IntField("MultishotLevel", UserData.MultishotLevel));
        UserData.SetSideLaserLevel(EditorGUILayout.IntField("SideLaserLevel", UserData.SideLaserLevel));
        UserData.SetPoints(EditorGUILayout.IntField("Points", UserData.Points));
        UserData.SetLevelPoints(EditorGUILayout.IntField("LevelPoints", UserData.LevelPoints));
        UserData.SetRealMoneyPoints(EditorGUILayout.IntField("RealMoneyPoints", UserData.RealMoneyPoints));
        UserData.SetRealMoneyPurchases(EditorGUILayout.IntField("RealMoneyPurchases", UserData.RealMoneyPurchases));

        EditorGUILayout.FloatField("TutorialMovementControl", UserData.TutorialMovementControl);
        EditorGUILayout.FloatField("TutorialOverworld", UserData.TutorialOverworld);
        EditorGUILayout.FloatField("TutorialSideLaser", UserData.TutorialSideLaser);



        if (GUILayout.Button("Reset"))
        {
            UserData.Clear();
        }


        Repaint();
    }
}