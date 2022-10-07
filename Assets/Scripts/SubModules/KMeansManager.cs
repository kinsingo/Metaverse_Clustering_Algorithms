using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Threading;
using System.Collections;

namespace Assets.Scripts.SubModules
{
    internal class KMeansManager
    {
        List<GameObject> datalist;
        List<GameObject> meanlist;
        const int maxIteration = 1000;
        KMeansData kMeansData;

        public KMeansManager(List<GameObject> datalist, List<GameObject> meanlist)
        {
            this.datalist = datalist;
            this.meanlist = meanlist;
            kMeansData = new KMeansData(datalist, K:meanlist.Count);
        }

        public IEnumerator Cluster()
        {
            int iteration = 0;
            while (kMeansData.IsContinue && iteration < maxIteration)
            {
                kMeansData.Update_mean();
                Update_mean_Position(k: 0, Color.red);
                Update_mean_Position(k: 1, Color.green);
                Update_mean_Position(k: 2, Color.blue);

                kMeansData.Update_clustering(kMeansData.Means);
                List<int>[] clusterIndexes = kMeansData.Get_clusterIndexes();
                Set_Clustering_Color(k: 0, clusterIndexes, Color.red);
                Set_Clustering_Color(k: 1, clusterIndexes, Color.green);
                Set_Clustering_Color(k: 2, clusterIndexes, Color.blue);

                double totalDistance =  kMeansData.Get_TotalDistance(kMeansData.Clustering, kMeansData.Means);
                Debug.Log($"iteration : {iteration}, totalDistance:{totalDistance}");

                iteration++;
                yield return new WaitForSeconds(1);
            }
        }


        void Update_mean_Position(int k, Color color)
        {
            GameObject cube = meanlist[k];
            cube.transform.position = kMeansData.Means[k];
            Renderer renderer = cube.GetComponent<Renderer>();
            renderer.material.color = color;
        }

        void Set_Clustering_Color(int k, List<int>[] clusterIndexes, Color color)
        {
            foreach (int index in clusterIndexes[k])
            {
                GameObject sphere = datalist[index];
                Renderer renderer = sphere.GetComponent<Renderer>();
                renderer.material.color = color;
            }
        }
        
    }
}
