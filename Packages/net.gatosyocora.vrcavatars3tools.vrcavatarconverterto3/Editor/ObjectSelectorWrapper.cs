using UnityEditor;
using System.Reflection;
using System;

// https://qiita.com/Alt_Shift_N/items/3831f7ea17e56d5d0a72
namespace Gatosyocora.VRCAvatars3Tools
{
    public static class ObjectSelectorWrapper
    {
        private static System.Type T;
        private static bool oldState = false;
        static ObjectSelectorWrapper()
        {
            T = System.Type.GetType("UnityEditor.ObjectSelector,UnityEditor");
        }

        public static void SetFilterString(string filter)
        {
            if (filter == null) filter = String.Empty;
            FieldInfo field = T.GetField("m_SearchFilter", BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            field.SetValue(Get(), filter);

        }

        private static EditorWindow Get()
        {
            PropertyInfo P = T.GetProperty("get", BindingFlags.Public | BindingFlags.Static);
            return P.GetValue(null, null) as EditorWindow;
        }
        public static void ShowSelector(System.Type aRequiredType)
        {
            MethodInfo ShowMethod = T.GetMethod("Show", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            ShowMethod.Invoke(Get(), new object[] { null, aRequiredType, null, true });
        }
        public static T GetSelectedObject<T>() where T : UnityEngine.Object
        {

            MethodInfo GetCurrentObjectMethod = ObjectSelectorWrapper.T.GetMethod("GetCurrentObject", BindingFlags.Static | BindingFlags.Public);
            return GetCurrentObjectMethod.Invoke(null, null) as T;
        }
        public static bool isVisible
        {
            get
            {
                PropertyInfo P = T.GetProperty("isVisible", BindingFlags.Public | BindingFlags.Static);
                return (bool)P.GetValue(null, null);
            }
        }
        public static bool HasJustBeenClosed()
        {
            bool visible = isVisible;
            if (visible != oldState && visible == false)
            {
                oldState = false;
                return true;
            }
            oldState = visible;
            return false;
        }
    }
}