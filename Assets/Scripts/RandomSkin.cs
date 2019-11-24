using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSkin : MonoBehaviour
{
    public List<Texture2D> PossibleTextures;

    // Start is called before the first frame update
    void Start()
    {
        int i = Random.Range(0, PossibleTextures.Count);
        Texture2D skin = PossibleTextures[i];

        GetComponent<SkinnedMeshRenderer>().material.mainTexture = skin;
    }
}
