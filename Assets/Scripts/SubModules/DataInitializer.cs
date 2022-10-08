using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.SubModules
{
    internal class DataInitializer
    {
        GameObject center;
        const float boundary = 2.0f;
        System.Random random;
        public DataInitializer(GameObject center)
        {
            this.center = center;
            random = new System.Random(Seed: 0);
        }


        public List<GameObject> Get_meanlist(int K)
        {
            List<GameObject> meanlist = new List<GameObject>();
            for (int k = 0; k < K; k++)
            {
                GameObject Cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                InitialziePositionForMean(Cube);
                Cube.transform.localScale = new Vector3(x: 0.3f, y: 0.3f, z: 0.3f);
                meanlist.Add(Cube);
            }
            return meanlist;
        }

        public List<GameObject> Get_datalist(int numOfData)
        {
            List<GameObject> datalist = new List<GameObject>();
            for (int n = 0; n < numOfData; n++)
            {
                GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                InitialziePositionForData(sphere,n);
                sphere.transform.localScale = new Vector3(x: 0.1f, y: 0.1f, z: 0.1f);
                datalist.Add(sphere);
            }
            return datalist;
        }

        void InitialziePositionForMean(GameObject primitive)
        {
            float x = Random.Range(-boundary, boundary);
            float y = Random.Range(-boundary, boundary);
            float z = Random.Range(-boundary, boundary);
            primitive.transform.position = this.center.transform.position + new Vector3(x, y, z);
        }

        void InitialziePositionForData(GameObject primitive,int n)
        {
            float x, y, z;
            int indicator = n % 3;
            if (indicator == 0)
            {
                x = Random.Range(boundary / 3.0f, boundary);
                y = Random.Range(boundary / 3.0f, boundary);
                z = Random.Range(boundary / 3.0f, boundary);
            }
               
            else if (indicator == 1)
            {
                x = Random.Range(-boundary, -boundary / 3.0f);
                y = Random.Range(-boundary, -boundary / 3.0f);
                z = Random.Range(-boundary, -boundary / 3.0f);
            }
            else //k==2
            {
                x = Random.Range(-boundary / 3.0f, boundary / 3.0f);
                y = Random.Range(-boundary / 3.0f, boundary / 3.0f);
                z = Random.Range(-boundary / 3.0f, boundary / 3.0f);
            }

            primitive.transform.position = this.center.transform.position + new Vector3(x, y, z);
        }

    }
}
