using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//if the player is in the triangle, the enemy follows him
//Try it!! 

public class EnemyController : MonoBehaviour
{
    GameObject player;//Player
    Animator anim;//Enemy animator
    CharacterController controller;//Enemy controller

    List<GameObject> Enemy = new List<GameObject>(); //lista di nemici

    [SerializeField] float speedWalk = 2;//velocità enemy

    //gravity
    private Vector3 velocity; // vettore movimento lungo Y
    float gravity = 9.81f;

    Vector3 moveDirection = Vector3.zero; // vettore movimento nel piano XZ


    //SENSORE
    [SerializeField] float distance = 10f;
    [SerializeField] float angle = 30f;
    [SerializeField] float height = 1f;
    [SerializeField] Color meshColor = Color.green;
    [SerializeField] float rayEnemyDetection = 1;

    [SerializeField] GameObject s_copy;//sfera da aggiungere al gameobject, serve per essere spawnata

    //il vertice 1 del triangolo è nell'origine del gameobject
    GameObject s2; //sfera vertice 2
    GameObject s3; //sfera vertice 3

    Mesh mesh;//mesh triangolo

    Vector3 trianglePosition;

    void Start()
    {
        player = GameObject.FindWithTag("Player"); //individuo l'oggetto con il tag Player
        anim = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();

        //creo due sfere dal codice, le copio da una sfera che aggiungo al gameobject
        //le sfere le creo nell'origine del gameobject, poi le riposiziono nell'Update
        s2 = Instantiate(s_copy, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
        s3 = Instantiate(s_copy, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);

        trianglePosition = new Vector3(transform.position.x, height, transform.position.z);
    }

    
    void Update()
    {
        DisegnoTriangolo();

        Movement();
    }


    /// <summary>
    /// Ottengo la lista dei nemici, escluso il "questo nemico" 
    /// </summary>
    void GetEnemy()
    {
        Enemy.Clear();

        foreach (GameObject en in GameObject.FindGameObjectsWithTag("Enemy"))//ottengo la lista dei nemici
        {
            if (en.transform.position != transform.position)
            {
                Enemy.Add(en);
            }
        }
    }


    void Movement()
    {
        GetEnemy();

        anim.SetInteger("attack", 0);

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position); // distanza tra enemy e Player

        EnemyGravity();

        if (IsInsight() | distanceToPlayer <= 1)//se il player è nel triangolo mi giro verso il player
        {
            Rotate();

            for (int i = 0; i < Enemy.Count; i++)
            {
                float distanceToEnemy = Vector3.Distance(transform.position, Enemy[i].transform.position); // distanza tra QuestoNemico e Gli altri nemici

                FollowPlayer(distanceToPlayer, distanceToEnemy);
            }

            if (Enemy.Count == 0)
            {
                FollowPlayer(distanceToPlayer, 2);
            }

        }
        else //idle
        {
            Idle();
        }
    }

    void DestroyEnemy()
    {
        Destroy(gameObject);
    }


    void Idle()
    {
        anim.SetFloat("enemySpeed", 0, 0.1f, Time.deltaTime);
    }

    void Walk()
    {
        anim.SetFloat("enemySpeed", 1f, 0.1f, Time.deltaTime);
    }

    void Rotate()
    {
        int damping = 30;
        var lookPos = player.transform.position - transform.position;
        lookPos.y = 0;
        var rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * damping);
    }

    void RandomAttack()
    {
        //enemy attack
    }

    void EnemyGravity()
    {
        velocity.y -= gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime); //gravità
    }





    void FollowPlayer(float distanceToPlayer, float distanceToEnemy)
    {
         if (distanceToPlayer >= 1) // cammino verso il player
         {
             //float step = speedWalk * Time.deltaTime;
             //transform.position = Vector3.MoveTowards(transform.position, player.transform.position, step);

              controller.Move(moveDirection * Time.deltaTime); //movimento

              Walk();
          }
          else if (distanceToPlayer < 1 && distanceToEnemy >= 1) // attacco
          {
              RandomAttack();
          }
    }



    //SENSORE

    /// <summary>
    /// Metodo che controlla se il Player è all'interno del triangolo mesh
    /// </summary>
    /// <returns></returns>
    bool IsInsight()
    {
        float x = player.transform.position.x;
        float z = player.transform.position.z;

        Vector3 A = trianglePosition;
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
    /// Disegno il triangolo entro cui fare il controllo della presenza del player
    /// </summary>
    void DisegnoTriangolo()
    {
        //direzione di movimento
        moveDirection = new Vector3(0, 0, 1);
        moveDirection *= speedWalk;
        moveDirection = transform.TransformDirection(moveDirection);
        //aggiorno la posizione delle sfere ai vertici della mesh
        s2.transform.position = trianglePosition + Quaternion.Euler(0, -angle, 0) * moveDirection * distance / 2;
        s3.transform.position = trianglePosition + Quaternion.Euler(0, angle, 0) * moveDirection * distance / 2;
        //disegno le linee di contorno
        Debug.DrawLine(trianglePosition, s2.transform.position, Color.red);
        Debug.DrawLine(trianglePosition, s3.transform.position, Color.red);
        Debug.DrawLine(s2.transform.position, s3.transform.position, Color.red);
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

            trianglePosition = new Vector3(transform.position.x, height, transform.position.z);

            Gizmos.DrawMesh(mesh, trianglePosition, transform.rotation);

            Gizmos.DrawWireSphere(trianglePosition, rayEnemyDetection);
        }
    }


}




