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
    internal class KMeansData
    {
        int K; //=3 (3개의 색 R/G/B로 Clustering Data 표기 예정)
        bool isContinue; //int[] clustering 이 더이상 변화가 없으면(최적에 수렴하면) IsContinue = false 하여 k-means를 종료함
        Vector3[] data;//[N x D] => [dataLength x 3] (3차원 Data임)
        int[] clustering;//[N] => [dataLength]
        Vector3[] means;//[K x D] => [3 x 3] (K=3, 3차원 Data임)

        public bool IsContinue => isContinue;
        public int[] Clustering => clustering;
        public Vector3[] Means => means;

        public KMeansData(List<GameObject> datalist, int K)
        {
            this.K = K;
            this.isContinue = true;

            data = new Vector3[datalist.Count];
            for (int i = 0; i < datalist.Count; i++)
                data[i] = datalist[i].transform.position;
            
            var random = new System.Random(Seed: 0);
            clustering = new int[datalist.Count]; 
            for (int i = 0; i < clustering.Length; ++i)
                clustering[i] = random.Next(0, K);
            
            means = new Vector3[K];   
        }

        public void Update_mean()
        {
            int[] clusterItemCount = new int[K];
            for (int k = 0; k < K; k++)
                means[k] = new Vector3();

            for (int i = 0; i < data.Length; i++)
            {
                int k = clustering[i]; //0,1 or 2
                means[k] += data[i];
                clusterItemCount[k]++; // Increment the count of the cluster that row i is assigned to
            }

            for (int k = 0; k < means.Length; k++)
            {
                int itemCount = clusterItemCount[k];
                means[k] /= itemCount > 0 ? itemCount : 1;
            }
        }

        public void Update_clustering(Vector3[] means)
        {
            isContinue = false;

            for (int i = 0; i < data.Length; i++)
            {
                double minDistance = double.MaxValue;
                int minDistance_k = -1;

                for (int k = 0; k < K; k++)
                {
                    double distance = Get_Distance(data[i], means[k]);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        minDistance_k = k;
                    }
                }

                // Re-arrange the clustering for datapoint if needed
                if (clustering[i] != minDistance_k)
                {
                    isContinue = true;
                    clustering[i] = minDistance_k;
                }
            }
        }

        public double Get_TotalDistance(int[] clustering,Vector3[] means)
        {
            double totalDistance = 0;
            for (int i = 0; i < data.Length; i++)
            {
                int k = clustering[i]; // What cluster is data i assigned to
                var distance = Get_Distance(data[i], means[k]);
                totalDistance += distance;
            }
            return totalDistance;
        }

        public List<int>[] Get_clusterIndexes()
        {
            List<int>[] clusterIndexes = new List<int>[K];
            for (int k = 0; k < K; k++)
                clusterIndexes[k] = new List<int>();
            
            for (int index = 0; index < data.Length; index++)
            {
                int k = clustering[index];
                clusterIndexes[k].Add(index);
            }
            return clusterIndexes;
        }

        double Get_Distance(Vector3 dataPoint, Vector3 mean) => Math.Sqrt(Math.Pow(dataPoint.x - mean.x, 2) + Math.Pow(dataPoint.y - mean.y, 2) + Math.Pow(dataPoint.z - mean.z, 2));
    }
}
