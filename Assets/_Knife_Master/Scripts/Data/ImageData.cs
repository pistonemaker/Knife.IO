using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Image Data", menuName = "Data / Image Data")]
public class ImageData : ScriptableObject
{
    public List<ImageColorRank> playerColorRanks;
    public List<ImageFace> faces;
    public List<ImageKnife> knifes;

    public ImageFace curFace;
    public Sprite curKnifeSprite;
    public Sprite curColor;
    public Sprite curRank;
}

[Serializable]
public class ImageColorRank
{
    public Sprite color;
    public Sprite rank;
}

[Serializable]
public class ImageFace
{
    public bool isUnlock;
    public bool hasClickedAfterUnlock;
    public Sprite sprite1;
    public Sprite sprite2;
    public UnlockCondition unlockCondition;
}

[Serializable]
public class ImageKnife
{
    public bool isUnlock;
    public bool hasClickedAfterUnlock;
    public List<Sprite> sprites;
    public UnlockCondition unlockCondition;
}

[Serializable]
public class UnlockCondition
{
    public UnlockType unlockType;
    public int amountToUnlock;
}