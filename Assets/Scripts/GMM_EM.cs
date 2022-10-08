using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.SubModules;
using Assets.Scripts;


public class GMM_EM : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        DataInitializer dataInitializer = new DataInitializer(center: this.gameObject);
        List<GameObject> datalist = dataInitializer.Get_datalist(numOfData: 500);
        List<GameObject> meanlist = dataInitializer.Get_meanlist(K: 3);

        KMeansManager kMeansManager = new KMeansManager(datalist, meanlist);
        GMM_EM_Manager GMM_EM_manager = new GMM_EM_Manager(kMeansManager);
        StartCoroutine(GMM_EM_manager.Cluster());
    }

    IEnumerator Initalize_Means_By_Kmeans(KMeansManager kMeansManager)
    {
        for (int i = 0; i < 5; i++)
            kMeansManager.Cluster_Once();
        yield return new WaitForSeconds(1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
