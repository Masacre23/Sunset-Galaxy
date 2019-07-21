using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrismData : MonoBehaviour {
    public enum PrismPosition { UP, DOWN, AROUND };
    public int height;
    public GameObject prismUp;
    public GameObject prismDown;
    public GameObject[] prismsAround;


    public PrismData(int height) {
        this.height = height;
    }
}
