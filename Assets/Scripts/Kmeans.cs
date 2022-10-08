using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.SubModules;
using Assets.Scripts;

public class Kmeans : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        DataInitializer dataInitializer = new DataInitializer(center: this.gameObject);
        List<GameObject> datalist = dataInitializer.Get_datalist(numOfData: 500);
        List<GameObject> meanlist = dataInitializer.Get_meanlist(K: 3);
        KMeansManager kMeansManager = new KMeansManager(datalist, meanlist);
        StartCoroutine(kMeansManager.Cluster());
    }
    
    // Update is called once per frame
    void Update()
    {
        //this.gameObject.transform.Rotate(new Vector3(0, 30, 0) * Time.deltaTime);
    }
}
