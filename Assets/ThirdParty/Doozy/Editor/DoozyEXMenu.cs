namespace DoozyEX
{
    using UnityEngine;
    using UnityEditor;
    using Doozy.Engine.UI.Settings;
    using System.Text;
    using System.IO;
    using Doozy.Engine.Soundy;

    public static class DoozyEXMenu
    {
        private const string TEMPLATE_CLASS_NAME = "DoozyNamesDB";

        private const string NAMEDB_FILE_PATH = "ThirdParty/Doozy/{0}.cs";

        private const string TEMPLATE_CLASS = @"
//自动生成，勿修改
public static class {0}
{{
{1}
}}";

        private const string TEMPLATE_FIELD_ITEM = @"   public const string {0} = ""{1}"";";

        /// <summary>
        /// 通过doozy databases 生成强类型name字段
        /// </summary>
        [MenuItem("Tools/Doozy/EX/GenerateUINameFile")]
        public static void GenerateUINameFile()
        {
            var contentSB = new StringBuilder();
 
            var fieldSB = new StringBuilder();

            //views
            foreach (var viewNameList in UIViewSettings.Database.Categories)
            {
                AddFieldStr(fieldSB, "VIEW_CATEGORY_" + viewNameList.CategoryName, viewNameList.CategoryName);
                
                foreach (var viewName in viewNameList.Names)
                {
                    AddFieldStr(fieldSB, $"VIEW_{viewNameList.CategoryName.ToUpper()}_" + viewName, viewName);
                }
            }

            //btns
            foreach (var btnNameList in UIButtonSettings.Database.Categories)
            {
                fieldSB.Append("\n");
                AddFieldStr(fieldSB, "BTN_CATEGORY_" + btnNameList.CategoryName, btnNameList.CategoryName);

                foreach (var btnName in btnNameList.Names)
                {
                    AddFieldStr(fieldSB, $"BTN_{btnNameList.CategoryName.ToUpper()}_" + btnName, btnName);
                }
            }
        
            //popups
            fieldSB.Append("\n");

            foreach (var popupName in UIPopupSettings.Database.PopupNames)
            {
                AddFieldStr(fieldSB, "POPUP_NAME_" + popupName, popupName);
            }

            //sounds
            fieldSB.Append("\n");
 
            foreach (var soundList in SoundySettings.Database.SoundDatabases)
            {
                AddFieldStr(fieldSB, "SOUND_CATEGORY_" + soundList.DatabaseName, soundList.DatabaseName);

                foreach (var sound in soundList.Database)
                {
                    AddFieldStr(fieldSB, $"SOUND_{sound.DatabaseName.ToUpper()}_" + sound.SoundName, sound.SoundName);
                    
                    Debug.Log($"---------soundDatabaseName:{sound.DatabaseName}  soundName:{sound.SoundName}"); 
                }
            }
            
            // Debug.LogFormat("public static class {0}\n\n{1}\n|", "a", "bbccdd");

            contentSB.AppendFormat(TEMPLATE_CLASS, TEMPLATE_CLASS_NAME, fieldSB.ToString());

            Debug.Log("content string:\n" + contentSB.ToString());

            var filePath = Path.Combine(Application.dataPath,string.Format(NAMEDB_FILE_PATH, TEMPLATE_CLASS_NAME));
 
            var dirName = Path.GetDirectoryName(filePath);

            if(!Directory.Exists(dirName))
                Directory.CreateDirectory(dirName);

            Debug.Log($"filePath:{filePath}");

            if (File.Exists(filePath))
            {
                File.Delete(filePath);

                Debug.Log("Delete exists file.");
            }

            StreamWriter fw = null;

            using (fw = File.CreateText(filePath))
            {
                fw.Write(contentSB.ToString());

                fw.Close();
            }

            AssetDatabase.Refresh();
        }

        private static void GetFilePath([System.Runtime.CompilerServices.CallerFilePath] string filePath = "")
        {
            Debug.Log(filePath);
        }

        private static void AddFieldStr(StringBuilder fieldSB, string fieldName, string fieldVal)
        {
            //有可能有空格，待处理 todo
            if (fieldName.IndexOf(' ') > -1)
            {
                Debug.LogWarning($"有空格，跳过：{fieldVal}");
                return;
            }

            fieldSB.AppendFormat(TEMPLATE_FIELD_ITEM, fieldName.ToUpper(), fieldVal);
            fieldSB.Append("\n");
        }
    }
}