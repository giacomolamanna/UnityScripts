using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensorEnemy : MonoBehaviour
{

    [SerializeField] float distance = 10f;
    [SerializeField] float angle = 30f;
    [SerializeField] Color meshColor = Color.red;

    [SerializeField] GameObject s_copy;//sfera da aggiungere al gameobject, serve per essere spawnata

    //il vertice 1 del triangolo è nell'origine del gameobject
    GameObject s2; //sfera vertice 2
    GameObject s3; //sfera vertice 3

    Vector3 moveDirection = Vector3.zero; // vettore movimento nel piano XZ

    Mesh mesh;//mesh triangolo

    GameObject player;//player

    float speed = 2; // velocità di movimento


    void Start()
    {
        player = GameObject.FindWithTag("Player"); //individuo l'oggetto con il tag Player

        //creo due sfere dal codice, le copio da una sfera che aggiungo al gameobject
        //le sfere le creo nell'origine del gameobject, poi le riposiziono nell'Update
        s2 = Instantiate(s_copy, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
        s3 = Instantiate(s_copy, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
    }

    
    void Update()
    {
        DisegnoTriangolo();

        //se il player è dentro il triangolo lo seguo
        if (IsInsight())
        {
            Rotate();

            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, step);
        }
    }


    /// <summary>
    /// Disegno il triangolo entro cui fare il controllo della presenza del player
    /// </summary>
    void DisegnoTriangolo()
    {
        //direzione di movimento
        moveDirection = new Vector3(0, 0, 1);
        moveDirection *= speed;
        moveDirection = transform.TransformDirection(moveDirection);
        //aggiorno la posizione delle sfere ai vertici della mesh
        s2.transform.position = transform.position + Quaternion.Euler(0, -angle, 0) * moveDirection * distance / 2;
        s3.transform.position = transform.position + Quaternion.Euler(0, angle, 0) * moveDirection * distance / 2;
        //disegno le linee di contorno
        Debug.DrawLine(transform.position, s2.transform.position, Color.red);
        Debug.DrawLine(transform.position, s3.transform.position, Color.red);
        Debug.DrawLine(s2.transform.position, s3.transform.position, Color.red);
    }


    /// <summary>
    /// Ruoto il gameobject nella direzione del Player
    /// </summary>
    void Rotate()
    {
        int damping = 30;
        var lookPos = player.transform.position - transform.position;
        lookPos.y = 0;
        var rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * damping);
    }


    /// <summary>
    /// Metodo che controlla se il Player è all'interno del triangolo mesh
    /// </summary>
    /// <returns></returns>
    bool IsInsight()
    {
        float x = player.transform.position.x;
        float z = player.transform.position.z;

        Vector3 A = transform.position;
        Vector3 B = s2.transform.position;
        Vector3 C = s3.transform.position;

        float d1 = (x - A.x) * (C.z - A.z) - (z - A.z) * (C.x - A.x);
        float d2 = (A.x - x) * (B.z - A.z) - (A.z - z) * (B.x - A.x);
        float d = (B.x - A.x) * (C.z - A.z) - (B.z - A.z) * (C.x - A.x);

        float a = d1 / d;
        float b = d2 / d;

        if (a > 0 && b > 0 && a + b < 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    /// <summary>
    /// Disegno la mesh di un triangolo
    /// (La mesh non ha una funzione specifica, mi fa solo visualizzare l'area di ricerca del player)
    /// </summary>
    /// <returns></returns>
    Mesh CreateWedegeMesh()
    {
        Mesh mesh = new Mesh();

        int numTriangles = 1;
        int numVertices = numTriangles * 3;

        Vector3[] vertices = new Vector3[numVertices];
        int[] triangles = new int[numVertices];


        Vector3 bottomCenter = Vector3.zero;
        Vector3 bottomLeft = Quaternion.Euler(0, -angle, 0) * Vector3.forward * distance;
        Vector3 bottomRight = Quaternion.Euler(0, angle, 0) * Vector3.forward * distance;

        int vert = 0;

        vertices[vert++] = bottomCenter;
        vertices[vert++] = bottomLeft;
        vertices[vert++] = bottomRight;

        for (int i = 0; i < numVertices; i++)
        {
            triangles[i] = i;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;
    }


    private void OnValidate()
    {
        mesh = CreateWedegeMesh();
    }

    private void OnDrawGizmos()
    {
        if (mesh)
        {
            Gizmos.color = meshColor;
            Gizmos.DrawMesh(mesh, transform.position, transform.rotation);
        }
    }



}











