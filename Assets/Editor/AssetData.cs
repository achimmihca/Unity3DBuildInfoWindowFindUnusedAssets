using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class AssetData {
	public string path { get; private set; }
	public string name { get; private set; }
	public string byteSize { get; private set; }
	public string perCentSize { get; private set; }

	public AssetData(string path) {
		this.path = path;
		// Set name to last segment of path
		var i = path.LastIndexOf("/");
		if(i < 0) {
			name = path;
		} else {
			name = path.Substring(i);
		}
	}

	public AssetData(string path, string byteSize, string perCentSize) : this(path) {
		this.byteSize = byteSize;
		this.perCentSize = perCentSize;
	}

	private UnityEngine.Object _obj;
	public UnityEngine.Object obj {
		get {
			if(_obj == null) {
				_obj = AssetDatabase.LoadAssetAtPath(path, typeof(UnityEngine.Object));
			}
			return _obj;
		}
	}
}
