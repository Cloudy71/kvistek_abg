using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class AssetLoader {
    private static Dictionary<string, Object> objects = new Dictionary<string, Object>();

    public static Texture GetTexture(string name) {
        Object ret = null;

        if (objects.TryGetValue(name, out ret)) return (Texture) ret;

        ret = (Texture) Resources.Load(name);
        objects.Add(name, ret);

        return (Texture) ret;
    }

    public static Object GetObject(string name) {
        Object ret = null;

        if (objects.TryGetValue(name, out ret)) return ret;

        ret = Resources.Load(name);
        objects.Add(name, ret);

        return ret;
    }

    public static Texture2D GetColor(int r, int g, int b, int a = 255) {
        return GetColor(new Color(r / 255f, 1f / 255f, b / 255f, a / 255f));
    }

    public static Texture2D GetColor(Color color) {
        Object ret = null;
        String name = "_c_" + color.r + "_" + color.g + "_" + color.b + "_" + color.a;

        if (objects.TryGetValue(name, out ret)) return (Texture2D) ret;

        Texture2D tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, color);
        tex.Apply();
        objects.Add(name, tex);

        return tex;
    }
}