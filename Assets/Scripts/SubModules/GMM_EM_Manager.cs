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
    internal class GMM_EM_Manager
    {
        KMeansManager kMeansManager;
        int K;//K = 3; 
        int N;
        
        Vector3[] data;//[N x D] => [dataLength x 3] (3차원 Data임)
        Vector3[] means;//[K x D] => [3 x 3] (K=3, 3차원 Data임)
        Vector3[] variances;//[K x D] => [3 x 3] (K=3, 3차원 Data임)

        float[][] responsibilities;//[N x K] => [dataLength x 3] (K=3, 3차원 Data임)
        float[] clustering;//[K]
        float[] Nk;//[K] => [3] 

        const int maxIteration = 5;

        public GMM_EM_Manager(KMeansManager kMeansManager)
        {
            this.kMeansManager = kMeansManager;
            this.K = kMeansManager.meanlist.Count;
            this.N = kMeansManager.datalist.Count;
        }

        void Initialize()
        {
            //X (data) [N x D]
            data = new Vector3[N];
            for (int n = 0; n < N; n++)
                data[n] = kMeansManager.datalist[n].transform.position;

            //u (means) [K x D]
            means = new Vector3[K];
            for (int k = 0; k < K; k++)
                means[k] = kMeansManager.meanlist[k].transform.position;

            //v (variances) [K x D]
            variances = new Vector3[K];
            for (int k = 0; k < K; k++)
                variances[k] = new Vector3(0.01f, 0.01f, 0.01f);

            //responsibilities [N x K]
            responsibilities = new float[N][];
            for (int n = 0; n < N; ++n)
                responsibilities[n] = new float[K];

            clustering = new float[] { 1.0f / K, 1.0f / K, 1.0f / K }; //(k=0,k=1,k=2's values sum to 1)
            Nk = new float[K]; 
        }

        public IEnumerator Cluster()
        {
            yield return kMeansManager.Cluster();
            Initialize();

            int iteration = 0;
            while (iteration < maxIteration)
            {
                //E Step
                Update_Responsibilities(ref responsibilities, data, means, variances, clustering);
                Set_Clustering_Color(responsibilities);

                //M Step
                Update_Nk(ref Nk, responsibilities);
                Update_Means(ref means, responsibilities, data, Nk);
                Update_Variances(ref variances, means, responsibilities, data, Nk);
                Update_Clustering(ref clustering, Nk);
                Update_mean_Position(k: 0, Color.red);
                Update_mean_Position(k: 1, Color.green);
                Update_mean_Position(k: 2, Color.blue);

                iteration++;
                yield return new WaitForSeconds(1);
            }
        }

        void Update_mean_Position(int k, Color color)
        {
            GameObject cube = kMeansManager.meanlist[k];
            cube.transform.position = means[k];
            Renderer renderer = cube.GetComponent<Renderer>();
            renderer.material.color = color;
        }

        void Set_Clustering_Color(float[][] responsibilities)
        {
            //float[][] responsibilities;//[N x K] => [dataLength x 3] (K=3, 3차원 Data임)
            for (int n = 0; n < N; n++)
            {
                GameObject sphere = kMeansManager.datalist[n];
                Renderer renderer = sphere.GetComponent<Renderer>();
                renderer.material.color = new Color(1.0f * responsibilities[n][0], 1.0f * responsibilities[n][1], 1.0f * responsibilities[n][2]);
            }
        }

        //void UpdateMembershipWts(double[][] w,double[][] x, double[][] u, double[][] V, double[] a)
        void Update_Responsibilities(ref float[][] responsibilities, Vector3[] data, Vector3[] means, Vector3[] variances, float[] clustering)
        {
            for (int n = 0; n < N; ++n)
            {
                float rowSum = 0.0f;
                for (int k = 0; k < K; ++k)
                {
                    float pdf = NaiveProb(data[n], means[k], variances[k]);
                    responsibilities[n][k] = clustering[k] * pdf;
                    rowSum += responsibilities[n][k];
                }
                for (int k = 0; k < K; ++k)
                {
                    responsibilities[n][k] /= rowSum;
                }
            }
        }

        void Update_Nk(ref float[] Nk, float[][] responsibilities)
        {
            for (int k = 0; k < K; ++k)
            {
                float sum = 0.0f;
                for (int n = 0; n < N; ++n)
                    sum += responsibilities[n][k];
                Nk[k] = sum;
            }
        }

        void Update_Clustering(ref float[] clustering, float[] Nk)
        {
            for (int k = 0; k < K; ++k)
                clustering[k] = Nk[k] / N;
        }
        void Update_Means(ref Vector3[] means, float[][] responsibilities, Vector3[] data, float[] Nk)
        {
            Vector3[] result = new Vector3[K];
            for (int k = 0; k < K; ++k)
            {
                result[k] = new Vector3(0.0f, 0.0f, 0.0f);
                for (int n = 0; n < N; ++n)
                {
                    float Rnk = responsibilities[n][k];
                    result[k].x += Rnk * data[n].x;
                    result[k].y += Rnk * data[n].y;
                    result[k].z += Rnk * data[n].z;
                }
                    
                result[k].x /= Nk[k];
                result[k].y /= Nk[k];
                result[k].z /= Nk[k];
            }

            for (int k = 0; k < K; ++k)
            {
                means[k] = result[k];
            }
        }

        void Update_Variances(ref Vector3[] variances, Vector3[] means, float[][] responsibilities, Vector3[] data, float[] Nk)
        {
            Vector3[] result;//[K x D] => [3 x 3] (K=3, 3차원 Data임)
            result = new Vector3[K];
            for (int k = 0; k < K; k++)
                result[k] = new Vector3(0,0,0);

            for (int k = 0; k < K; ++k)
            {
                for (int n = 0; n < N; ++n)
                {
                    float Rnk = responsibilities[n][k];
                    result[k].x += Rnk * (data[n].x - means[k].x) * (data[n].x - means[k].x);
                    result[k].y += Rnk * (data[n].y - means[k].y) * (data[n].y - means[k].y);
                    result[k].z += Rnk * (data[n].z - means[k].z) * (data[n].z - means[k].z);
                }
                result[k].x /= Nk[k];
                result[k].y /= Nk[k];
                result[k].z /= Nk[k];
            }
            for (int k = 0; k < K; ++k)
                variances[k] = result[k];
        }

        float NaiveProb(Vector3 dataPoint, Vector3 mean, Vector3 variance)
        {
            // Poor man's multivariate Gaussian PDF
            double sum = 0.0;
            sum += ProbDenFunc(dataPoint.x, mean.x, variance.x);
            sum += ProbDenFunc(dataPoint.y, mean.y, variance.y);
            sum += ProbDenFunc(dataPoint.z, mean.z, variance.z);
            return Convert.ToSingle(sum / 3);
        }
        
       double ProbDenFunc(float data, float mean, float variance)
       {
            // Univariate Gaussian
            float eps = 0.000001f;
           if (-eps < variance && variance < eps)
               variance = eps;
       
           if (variance == 0.0) throw new Exception("0 in ProbDenFun");
           double left = 1.0 / Math.Sqrt(2.0 * Math.PI * variance);
           double right = Math.Exp(-1 * ((data - mean) * (data - mean)) / (2 * variance));
           return left * right;
       }



    }
}
