using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesManager
{
    private static Dictionary<string, Sprite> nameSpriteDict = new Dictionary<string, Sprite>();
    /// <summary>
    /// 获取图集
    /// </summary>
    /// <param name="icon">图片名称</param>
    /// <returns></returns>
    public static Sprite GetSprite(string icon)
    {
        if (nameSpriteDict.ContainsKey(icon))
        {
            return nameSpriteDict[icon];
        }

        Sprite[] sprites = Resources.LoadAll<Sprite>("headIcon");
        string[] arr = icon.Split('_');
        int index = int.Parse(arr[1]);
        nameSpriteDict.Add(icon, sprites[index]);
        return sprites[index];
    }

    /// <summary>
    /// 加载牌的图
    /// </summary>
    /// <param name="cardName"></param>
    /// <returns></returns>
    public static Sprite LoadCardSprite(string cardName)
    {
        if (nameSpriteDict.ContainsKey(cardName))
        {
            return nameSpriteDict[cardName];
        }

        Sprite sp = Resources.Load<Sprite>("poker/" + cardName);
        nameSpriteDict.Add(cardName,sp);
        return sp;
    }
}
