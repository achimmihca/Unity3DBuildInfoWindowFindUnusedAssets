using UnityEngine;
using UnityEditor;
using System;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class BuildLogParser {

    public List<AssetData> includedAssets { get; private set; }
    public List<string> includedDependencies { get; private set; }

    public BuildLogParser() {
        includedAssets = new List<AssetData>();
        includedDependencies = new List<string>();
    }

    public void Update() {
        includedAssets.Clear();
        includedDependencies.Clear();

        string LocalAppData = string.Empty;
        string UnityEditorLogfile = string.Empty;

        if (Application.platform == RuntimePlatform.WindowsEditor) {
            LocalAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            UnityEditorLogfile = LocalAppData + "\\Unity\\Editor\\Editor.log";
        } else if (Application.platform == RuntimePlatform.OSXEditor) {
            LocalAppData = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            UnityEditorLogfile = LocalAppData + "/Library/Logs/Unity/Editor.log";
        }

        try {
            // Have to use FileStream to get around sharing violations!
            FileStream FS = new FileStream(UnityEditorLogfile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            StreamReader SR = new StreamReader(FS);

            string line;
            string path;
            string size;
            string byteSize;
            string perCentSize;
            int perCentIndex;
            AssetData assetData;
            // TODO: Skip file to last build log
            while (!SR.EndOfStream && !(line = SR.ReadLine()).Contains("Mono dependencies included in the build")) ;
            while (!SR.EndOfStream && (line = SR.ReadLine()) != "") {
                includedDependencies.Add(line);
            }
            while (!SR.EndOfStream && !(line = SR.ReadLine()).Contains("Used Assets")) ;
            while (!SR.EndOfStream && (line = SR.ReadLine()) != "") {
                perCentIndex = line.IndexOf("%");
                path = line.Substring(perCentIndex+2).Trim();

                size = line.Substring(0, perCentIndex+1).Trim();
                byteSize = Regex.Match(size, "\\d+\\.\\d+\\s\\w+").Value;
                perCentSize = Regex.Match(size, "\\d+.\\d+%").Value;

                assetData = new AssetData(path, byteSize, perCentSize);
                includedAssets.Add(assetData);
            }
        } catch (Exception E) {
            Debug.LogError("Error: " + E);
        }
    }
}