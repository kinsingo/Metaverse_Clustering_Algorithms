using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.SubModules;

public class Kmeans : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        const float boundary = 2.0f;
        List<GameObject> datalist = Get_datalist(numOfData: 500, boundary);
        List<GameObject> meanlist = Get_meanlist(K: 3, boundary);
        KMeansManager kMeansManager = new KMeansManager(datalist, meanlist);
        StartCoroutine(kMeansManager.Cluster());
    }

    List<GameObject> Get_meanlist(int K, float boundary)
    {
        List<GameObject> meanlist = new List<GameObject>();
        for (int i = 0; i < K; i++)
        {
            GameObject Cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            InitialziePosition(Cube, boundary);
            Cube.transform.localScale = new Vector3(x: 0.3f, y: 0.3f, z: 0.3f);
            meanlist.Add(Cube);
        }
        return meanlist;
    }

    List<GameObject> Get_datalist(int numOfData, float boundary)
    {
        List<GameObject> datalist  = new List<GameObject>();
        for (int i = 0; i < numOfData; i++)
        {
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            InitialziePosition(sphere, boundary);
            sphere.transform.localScale = new Vector3(x: 0.1f, y: 0.1f, z: 0.1f);
            datalist.Add(sphere);
        }
        return datalist;
    }

    void InitialziePosition(GameObject primitive, float boundary) 
    {
        float x = Random.Range(-boundary, boundary);
        float y = Random.Range(-boundary, boundary);
        float z = Random.Range(-boundary, boundary);
        primitive.transform.position = this.gameObject.transform.position + new Vector3(x, y, z);
    }

    // Update is called once per frame
    void Update()
    {
        //this.gameObject.transform.Rotate(new Vector3(0, 30, 0) * Time.deltaTime);
    }
}
