﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class CubeBehavior : MonoBehaviour
{


    int i = 0;
    int j = 1;
    int z = 0;
    int counter = 0;
    public bool flag = true;
    bool changeInDirection = false;
    int n = 0;

    Vector3 source;
    Vector3 destination = new Vector3(0, 0, 0);
    Vector3 offset;
    Vector3 actualCameraPosition;
    //public Vector3 currentpos;
    public Vector3[] points = new Vector3[100];

    //public Text coordinates;
    //public Text sourceText;
    //public Text destinationText;
    public Text debugger;
    //public Text serverReturns;
    //public Text updateText;

    public Camera mainCam;
    GameObject cube;



    // Use this for initialization
    void Start()
    {
        //mainCam = Camera.main;
        // Camera.main.transform.position = new Vector3(0, 0, 0);
        //points[6] = winlab3;
        debugger.text = "Calling Coroutine";

        StartCoroutine(GetRequest1());
    }

    IEnumerator GetRequest1()
    {

        while (flag)
        {
            WWW webpage = new WWW("http://10.50.0.73:8080/?sending=" + flag.ToString());
            yield return webpage;
            var readJSON = webpage.text;
            var JSONObject = JsonUtility.FromJson<InitJSON>(readJSON);//kp waas here
            if (JSONObject.id == 3000)
            {
                offset.x = 0 + JSONObject.xpos;
                offset.z = 0 - JSONObject.zpos;
                offset.y = 0;
                //serverReturns.text = "x= "+JSONObject.xpos.ToString() + " z= " + JSONObject.zpos.ToString();

                flag = false;
                StartCoroutine(CoroLoop());

            }
            else if (JSONObject.id == 3001)
            {
                offset.x = -16.3f + JSONObject.xpos;
                offset.z = -0.6f - JSONObject.zpos;
                offset.y = 0;
                //serverReturns.text = "x= "+JSONObject.xpos.ToString() + " z= " + JSONObject.zpos.ToString();

                flag = false;
                StartCoroutine(CoroLoop());

            }
            else if (JSONObject.id == 3002)
            {
                offset.x = 13.3f + JSONObject.xpos;
                offset.z = -1 - JSONObject.zpos;
                offset.y = 0;
                //serverReturns.text = "x= "+JSONObject.xpos.ToString() + " z= " + JSONObject.zpos.ToString();

                flag = false;
                StartCoroutine(CoroLoop());

            }
            else
            {
                debugger.text = "Look at your nearest chilitag";
            }
        }


    }
    IEnumerator CoroLoop()
    {

        debugger.text = "Chilitag identified";

        WWW webpage = new WWW("http://10.50.0.73:8080/?sending=" + flag.ToString() + "&x1=" + offset.x.ToString() + "&z1=" + offset.z.ToString());
        yield return webpage;

        var readJSON = webpage.text;
        debugger.text = readJSON;
        var JSONObject = JsonUtility.FromJson<JSONScripting>(readJSON); //kp waas here
        debugger.text = JSONObject.x[0].ToString() + " " + JSONObject.x[1].ToString();
        for (counter = 0; counter < JSONObject.x.Length; counter++)
        {
            points[counter] = new Vector3(JSONObject.x[counter], 0, JSONObject.z[counter]);
        }
        n = points.Length - 1;
        debugger.text = "Location set to Ivan's Office";
        StartCoroutine(UpdateLoop());
    }
    IEnumerator UpdateLoop()
    {
        while (true)
        {

            actualCameraPosition.x = Mathf.Round((Camera.main.transform.position.x + offset.x) * 10f) / 10f;
            actualCameraPosition.y = (int)Camera.main.transform.position.y + (int)offset.y;
            actualCameraPosition.z = Mathf.Round((Camera.main.transform.position.z + offset.z) * 10f) / 10f;
            //updateText.text = Camera.main.transform.position.ToString();
            //coordinates.text = "( " + actualCameraPosition.x.ToString() + " , " + actualCameraPosition.y.ToString() + " , " + actualCameraPosition.z.ToString() + " )";
            source = points[i];
            destination = points[j];

            //sourceText.text = "Source :" + source.ToString();
            //destinationText.text = "Destination: " + destination.ToString();
            if (i == 0 && j == 1)
            {
                DrawCubes(source - offset, destination - offset);
            }
            else
            {
                float xpos = points[j - 1].x - points[i - 1].x;
                float zpos = points[j - 1].z - points[i - 1].z;
                if (Vector3.Distance(actualCameraPosition, points[j - 1]) > Vector3.Distance(points[i - 1], points[j - 1]) && changeInDirection == true)
                {
                    if (xpos == 0 && Mathf.Abs(actualCameraPosition.x - points[i - 1].x) > 2.3)
                    {

                        StartCoroutine(NewGetRequest());
                        i = 0;
                        j = 1;
                        //updateText.text = "New X Get Request Done" + i.ToString() + j.ToString();
                        GameObject.FindGameObjectWithTag("cubes").SetActive(false);
                        continue;
                    }

                    else if (zpos == 0 && Mathf.Abs(actualCameraPosition.z - points[i - 1].z) > 2.3)
                    {

                        StartCoroutine(NewGetRequest());
                        i = 0;
                        j = 1;
                        GameObject.FindGameObjectWithTag("cubes").SetActive(false);
                        //updateText.text = "New Z Get Request Done" + i.ToString() + j.ToString();
                        continue;
                    }
                }
                Vector3 distance = actualCameraPosition - source;
                if (Mathf.Abs(distance.x) < 0.3 && Mathf.Abs(distance.z) < 0.3)            //if (actualCameraPosition == source)
                {
                    //i++;
                    //j++;
                    for (int i = 0; i < z; i++)
                    {
                        //Destroy(GameObject.FindGameObjectWithTag("cubes"));
                        GameObject.FindGameObjectWithTag("cubes").SetActive(false);
                    }
                    changeInDirection = true;
                    DrawCubes(source - offset, destination - offset);
                }

            }

            if (source == points[n])
            {
                debugger.text = "Reached Destination";
                GameObject.FindGameObjectWithTag("cubes").SetActive(false);
                yield return 0;

            }
            yield return new WaitForSeconds(0.01f);
        }
    }
    // Update is called once per frame
    /*void Update()
    {
        updateText.text = "Reached update! Kill me pls!";
        actualCameraPosition.x = (int)mainCam.transform.position.x + offset.x;
        actualCameraPosition.y = (int)mainCam.transform.position.y + offset.y;
        actualCameraPosition.z = (int)mainCam.transform.position.z + offset.z;
        coordinates.text = "( " +actualCameraPosition.x.ToString() + " , " + actualCameraPosition.y.ToString() + " , " + actualCameraPosition.z.ToString() + " )";
        source = points[i];
        destination = points[j];
        sourceText.text = "Source :" + source.ToString();
        destinationText.text = "Destination: " + destination.ToString();
        if (i == 0 && j == 1)
        {
            DrawCubes(source, destination);
        }
        else
        {
            if (actualCameraPosition == source)
            {
                //i++;
                //j++;
                for (int i = 0; i < z; i++)
                {
                    //Destroy(GameObject.FindGameObjectWithTag("cubes"));
                    GameObject.FindGameObjectWithTag("cubes").SetActive(false);
                }
                DrawCubes(source, destination);
            }
        }
        if (source == points[5])
        {
            return;
        }
    }*/
    IEnumerator NewGetRequest()
    {
        WWW webpage = new WWW("http://10.50.0.73:8080/?sending=" + flag.ToString() + "&x1=" + actualCameraPosition.x.ToString() + "&z1=" + actualCameraPosition.z.ToString());
        yield return webpage;
        Array.Clear(points, 0, points.Length);
        var readJSON = webpage.text;
        debugger.text = readJSON;
        var JSONObject = JsonUtility.FromJson<JSONScripting>(readJSON); //kp waas here
        debugger.text = JSONObject.x[0].ToString() + " " + JSONObject.x[1].ToString();
        for (counter = 0; counter < JSONObject.x.Length; counter++)
        {
            points[counter] = new Vector3(JSONObject.x[counter], 0, JSONObject.z[counter]);
        }
        //serverReturns.text = points[0].ToString() + " , " + points[1].ToString()+ " , " + points[2].ToString();
        n = points.Length - 1;
        debugger.text = "Direction Changed! Destination: Ivan's Office";
        changeInDirection = false;
        //StartCoroutine(UpdateLoop());
    }
    void DrawCubes(Vector3 source, Vector3 destination)
    {

        float posx = destination.x - source.x;
        float posz = destination.z - source.z;
        z = 0;
        cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);
        cube.transform.position = new Vector3(source.x, -1, source.z);
        cube.GetComponent<MeshRenderer>().material.color = Color.red;
        cube.tag = "cubes";
        z++;
        cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);
        cube.transform.position = new Vector3(destination.x, -1, destination.z);
        cube.GetComponent<MeshRenderer>().material.color = Color.red;
        cube.tag = "cubes";
        z++;
        if (posx == 0)
        {
            for (float i = source.z + 1.0f; i < destination.z; i++)
            {
                //debugger.text = i.ToString();
                cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);
                cube.transform.position = new Vector3(source.x, -1, i);
                cube.GetComponent<MeshRenderer>().material.color = Color.red;
                cube.tag = "cubes";
                z++;
            }
        }
        if (posz == 0)
        {
            for (float i = source.x + 1.0f; i < destination.x; i++)
            {
                //debugger.text = i.ToString();
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);
                cube.transform.position = new Vector3(i, -1, source.z);
                cube.GetComponent<MeshRenderer>().material.color = Color.red;
                cube.tag = "cubes";
                z++;
            }
            /*if (Vector3.Distance(actualCameraPosition, points[j - 1]) > Vector3.Distance(points[i - 1], points[j - 1]) && Mathf.Abs(actualCameraPosition.z - points[i - 1].z) > 1 && changeInDirection == true)
            {
                StartCoroutine(NewGetRequest());
            }*/
        }
        i++;
        j++;
        /*for(int i = 0; i < end.x; i+=x)
        {
            GameObject capsule = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            capsule.transform.position = new Vector3(0, 0, i);
            capsule.transform.eulerAngles = new Vector3(90, 0, 0);
            capsule.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            rend.material.color = Color.white;
        }*/

    }
}
