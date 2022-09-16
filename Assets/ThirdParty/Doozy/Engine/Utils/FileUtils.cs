// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable IdentifierTypo

namespace Doozy.Engine.Utils
{
    public class FileUtils
    {
        public const bool IGNORE_META = true;
        public const string UNITY_METAFILE_EXTENSION = ".meta";
        public const string DOTSTART_HIDDEN_FILE_HEADSTRING = ".";
        public const char UNITY_FOLDER_SEPARATOR = '/'; // Mac/Windows/Linux can use '/' in Unity.

        public static void RemakeDirectory(string localFolderPath)
        {
            if (Directory.Exists(localFolderPath)) DeleteDirectory(localFolderPath, true);
            Directory.CreateDirectory(localFolderPath);
        }

        public static void CopyFile(string sourceFilePath, string targetFilePath)
        {
            string parentDirectoryPath = Path.GetDirectoryName(targetFilePath);
            // ReSharper disable once AssignNullToNotNullAttribute
            Directory.CreateDirectory(parentDirectoryPath);
            File.Copy(sourceFilePath, targetFilePath, true);
        }

        public static void CopyTemplateFile(string sourceFilePath, string targetFilePath, string srcName, string dstName)
        {
            string parentDirectoryPath = Path.GetDirectoryName(targetFilePath);
            // ReSharper disable once AssignNullToNotNullAttribute
            Directory.CreateDirectory(parentDirectoryPath);

            StreamReader r = File.OpenText(sourceFilePath);

            string contents = r.ReadToEnd();
            contents = contents.Replace(srcName, dstName);

            File.WriteAllText(targetFilePath, contents);
        }

        public static void DeleteFileThenDeleteFolderIfEmpty(string localTargetFilePath)
        {
            File.Delete(localTargetFilePath);
            File.Delete(localTargetFilePath + UNITY_METAFILE_EXTENSION);
            string directoryPath = Directory.GetParent(localTargetFilePath).FullName;
            IEnumerable<string> restFiles = GetFilePathsInFolder(directoryPath);
            if (restFiles.Any()) return;
            DeleteDirectory(directoryPath, true);
            File.Delete(directoryPath + UNITY_METAFILE_EXTENSION);
        }

        /// <summary>
        ///     Get all files under given path, including files in child folders
        /// </summary>
        public static List<string> GetAllFilePathsInFolder(string localFolderPath, bool includeHidden = false, bool includeMeta = !IGNORE_META)
        {
            var filePaths = new List<string>();

            if (string.IsNullOrEmpty(localFolderPath)) return filePaths;
            if (!Directory.Exists(localFolderPath)) return filePaths;

            GetFilePathsRecursively(localFolderPath, filePaths, includeHidden, includeMeta);

            return filePaths;
        }

        /// <summary>
        ///     Get files under given path
        /// </summary>
        public static IEnumerable<string> GetFilePathsInFolder(string folderPath, bool includeHidden = false, bool includeMeta = !IGNORE_META)
        {
            IEnumerable<string> filePaths = Directory.GetFiles(folderPath).Select(p => p);

            if (!includeHidden) filePaths = filePaths.Where(path => !Path.GetFileName(path).StartsWith(DOTSTART_HIDDEN_FILE_HEADSTRING));
            if (!includeMeta) filePaths = filePaths.Where(path => !IsMetaFile(path));

            // Directory.GetFiles() returns platform dependent delimiter, so make sure replace with "/"
            if (Path.DirectorySeparatorChar != UNITY_FOLDER_SEPARATOR) filePaths = filePaths.Select(filePath => filePath.Replace(Path.DirectorySeparatorChar.ToString(), UNITY_FOLDER_SEPARATOR.ToString()));

            return filePaths.ToList();
        }


        private static void GetFilePathsRecursively(string localFolderPath, List<string> filePaths, bool includeHidden = false, bool includeMeta = !IGNORE_META)
        {
            string[] folders = Directory.GetDirectories(localFolderPath);

            foreach (string folder in folders) GetFilePathsRecursively(folder, filePaths, includeHidden, includeMeta);

            IEnumerable<string> files = GetFilePathsInFolder(localFolderPath, includeHidden, includeMeta);
            filePaths.AddRange(files);
        }

        /// <summary>
        ///     Create combination of path. Delimiter is always '/'.
        /// </summary>
        public static string PathCombine(params string[] paths)
        {
            if (paths.Length < 2) throw new ArgumentException("Argument must contain at least 2 strings to combine.");
            string combinedPath = _PathCombine(paths[0], paths[1]);
            var restPaths = new string[paths.Length - 2];
            Array.Copy(paths, 2, restPaths, 0, restPaths.Length);
            foreach (string path in restPaths) combinedPath = _PathCombine(combinedPath, path);
            return combinedPath;
        }

        private static string _PathCombine(string head, string tail)
        {
            if (!head.EndsWith(UNITY_FOLDER_SEPARATOR.ToString())) head = head + UNITY_FOLDER_SEPARATOR;
            if (string.IsNullOrEmpty(tail)) return head;
            if (tail.StartsWith(UNITY_FOLDER_SEPARATOR.ToString())) tail = tail.Substring(1);
            return Path.Combine(head, tail);
        }

        public static string GetPathWithProjectPath(string pathUnderProjectFolder)
        {
            string assetPath = Application.dataPath;
            string projectPath = Directory.GetParent(assetPath).ToString();
            return PathCombine(projectPath, pathUnderProjectFolder);
        }

        public static string GetPathWithAssetsPath(string pathUnderAssetsFolder)
        {
            string assetPath = Application.dataPath;
            return PathCombine(assetPath, pathUnderAssetsFolder);
        }

        public static string ProjectPathWithSlash()
        {
            string assetPath = Application.dataPath;
            return Directory.GetParent(assetPath).ToString() + UNITY_FOLDER_SEPARATOR;
        }

        public static bool IsMetaFile(string filePath) { return filePath.EndsWith(UNITY_METAFILE_EXTENSION); }

        public static bool ContainsHiddenFiles(string filePath)
        {
            string[] pathComponents = filePath.Split(UNITY_FOLDER_SEPARATOR);
            return pathComponents.Any(path => path.StartsWith(DOTSTART_HIDDEN_FILE_HEADSTRING));
        }

        public static void DeleteDirectory(string dirPath, bool isRecursive, bool forceDelete = true)
        {
            if (forceDelete) RemoveFileAttributes(dirPath, isRecursive);
            Directory.Delete(dirPath, isRecursive);
        }

        public static void RemoveFileAttributes(string dirPath, bool isRecursive)
        {
            foreach (string file in Directory.GetFiles(dirPath)) File.SetAttributes(file, FileAttributes.Normal);
            if (!isRecursive) return;
            foreach (string dir in Directory.GetDirectories(dirPath))
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                RemoveFileAttributes(dir, isRecursive);
        }

        /// <summary> Searches for the directoryName in all the project's directories and returns the absolute path of the first one it encounters. </summary>
        /// <param name="directoryName"> Directory name to search for </param>
        /// <param name="debug"> If anything goes wrong, should debug messages be shown? </param>
        public static string GetAbsoluteDirectoryPath(string directoryName, bool debug = false)
        {
            string[] directoryPath = Directory.GetDirectories(Application.dataPath, directoryName, SearchOption.AllDirectories);
            if (directoryPath.Length == 0)
            {
                if (debug) Debug.LogError("You searched for the [" + directoryName + "] folder, but no folder with that name exists in the current project.");
                return "ERROR";
            }

            if (directoryPath.Length <= 1) return directoryPath[0];
            if (debug)
                Debug.LogWarning("You searched for the [" + directoryName + "] folder. There are " + directoryPath.Length +
                                 " folders with that name. Returned the folder location for the first one, but it might not be the one you're looking for. Give the folder you are looking for an unique name to avoid any issues.");
            return directoryPath[0];
        }

        /// <summary> Searches for the directoryName in all the project's directories and returns the relative path of the first one it encounters </summary>
        public static string GetRelativeDirectoryPath(string directoryName)
        {
            string directoryPath = GetAbsoluteDirectoryPath(directoryName);
            directoryPath = directoryPath.Replace(Application.dataPath, "Assets");
            return directoryPath;
        }
    }
}