using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptCtrlPanera : MonoBehaviour
{
    public GameObject ctrlInterfaz;
    public GameObject muruUsuario;
    public List<GameObject> List_BreadcrumbsTrails;

    // Start is called before the first frame update
    void Start()
    {
        ctrlInterfaz = GameObject.FindWithTag("ctrlInterfaz");
        muruUsuario = GameObject.FindWithTag("MuroUsuario");

        //this.transform.localPosition = new Vector3(-22f, -20f, -15f);
        //this.transform.localScale = new Vector3(32, 3.5f, 0.100000001f);
        //this.transform.gameObject.SetActive(false);

        // Actualizo la panera de "ScriptDatosInterfaz" con los datos que he puesto a la panera actualmente
        //ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().panera = this.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Autor Nicolas Merino Ramirez
    // Fecha 19/10/2022
    // Descripcion
    //      Activa y desactiva la panera, al darle al boton de las migas de pan
    public void activar_o_desactivar_Panera(bool on_Off)
    {
        this.transform.gameObject.SetActive(on_Off);
    }

 // Nicolas 2022-10-27 ojo borrado

    // Autor Nicolas Merino Ramirez
    // Fecha 23/05/2022
    // Descripcion
    //      Annadir elementos al vector de migas de pan, y en caso de que el elemento ya este
    //      dentro del vector, se eliminan todos los que estan detras del elemento introducido
    public void annadir_Evi_A_Migas(GameObject evi)
    {
        Debug.Log("Entrando en panera _ annadir");
        
        int id_New_Evi = esta_el_elemento_referenciado_en_las_migas(evi);
        // Si las migas de pan NO contienen el nuevo elemento, se annade
        if (id_New_Evi == -1)
        {
            evi.transform.SetParent(this.transform);    // Ahijo la miga de pan a la panera
            this.List_BreadcrumbsTrails.Add(evi);       // Annado la miga a la lista
            this.redimensionar_Evi(evi);
        }
        else // en caso contrario elimino todos los elementos detras del nuevo evi
        {
            // Buscar elemento de id = "id_New_Evi" y borrar a partir de ahi
            int posicion_evi = this.List_BreadcrumbsTrails.IndexOf( bucar_Evi_En_Panera_By_ID(id_New_Evi) );
            this.List_BreadcrumbsTrails.RemoveRange(posicion_evi + 1, this.List_BreadcrumbsTrails.Count - (posicion_evi + 1));
        }
    }

    // Autor Nicolas Merino Ramirez
    // Fecha 19/10/2022
    // Descripcion
    //      Redimensiona los EVIs en la panera para que tengan un tamanno adecuado y los muestra
    //      con un formato adecuado
    private void redimensionar_Evi(GameObject evi)
    {
        // pongo a los elementos en el espacio reservado a la panera
        
        // Pongo la rotacion a 0, porque daba problema a la hora de mostar
        evi.transform.localRotation = new Quaternion(0f,0f,0f,0f);

        // /////////////////////////////////////////////////////
        // ESCALA       del     EVI
        // /////////////////////////////////////////////////////

        evi.transform.localScale = new Vector3
        (   ///*
            .1f,    // El evi ocupa el 10% de la panera en x
            .5f,    // El evi ocupa el 50% de la panera en y
            1f
         /*
         (muruUsuario.transform.localScale.x * .3f) // 30% de la pantalla
         / (evi.),    
         .5f,    // El evi ocupa el 50% de la panera en y
         1f
         */
         );

        // /////////////////////////////////////////////////////
        // POSICION     del     EVI
        // /////////////////////////////////////////////////////

        // pongo el evi en la panera, pero en el eje x lo muevo ".12f" para crear una separacion
        evi.transform.localPosition = new Vector3
        (
            - .5f       // Lo pone a la izquierda de la panera
            + evi.transform.localScale.x * 1f/2f    // Evita que la mitad del primer Evi quede fuera
            // Sumo el espacio que ocupa un Evi * el Numero de evis -1
            + (this.List_BreadcrumbsTrails.Count - 1f) * evi.transform.localScale.x             // Pone los Evis consecutivos
            + (this.List_BreadcrumbsTrails.Count - 1f) * evi.transform.localScale.x * 1f/2f     // Añade espacio entre evis
            ,
            0f, // Centrar en el eje y
            1f  // Para que no se distorsionen los botones
        );

    }

    // Autor Nicolas Merino Ramirez
    // Fecha 19/10/2022
    // Descripcion
    //      Redimensiona los EVIs en la panera para que tengan un tamanno adecuado y los muestra
    //      con un formato adecuado
    private void redimensionar_Panera()
    {

        if (List_BreadcrumbsTrails.Count == 1)
        {
            this.transform.localScale = List_BreadcrumbsTrails[0].transform.localScale;
        } 
        else if (List_BreadcrumbsTrails.Count > 1
              && List_BreadcrumbsTrails.Count > 6
              )
        {
            this.transform.localScale = List_BreadcrumbsTrails[0].transform.localScale * List_BreadcrumbsTrails.Count;
        }
    }

    // Autor Nicolas Merino Ramirez
    // Fecha 11/11/2022
    // Descripcion
    //      Metodo que me dice si la version original de una referencia se encuentra ya en la panera o no
    // Devolucion
    //      -1   - No esta en la lista de migas de pan
    //      Int  - Esta    en la lista de migas de pan y devuelvo, el id de la ANTIGUA REF
    private int esta_el_elemento_referenciado_en_las_migas(GameObject evi)
    {
        int ref_Original = evi.transform.Find("ContenedorDeEvi_01").transform.Find("EviRefElemen(Clone)").gameObject.GetComponent<SctCtrlEviRefElemen>().idElementIntf_elemenRef;

        Debug.Log(
            "Entrando:  ->  esta_el_elemento_referenciado_en_las_migas\n" +
            "Nº Evi referencia:         " + evi.GetComponent<ScriptDatosElemenItf>().idElementIntf + "\n" +
            "Nº Evi original de la ref: " + ref_Original
        );

        foreach (GameObject elemento in List_BreadcrumbsTrails)
        {
            // Si el original' de la referencia esta en las migas de pan, devuelvo true y salgo del bucle
            if (ref_Original == 
                elemento.transform.Find("ContenedorDeEvi_01").transform.Find("EviRefElemen(Clone)").gameObject.GetComponent<SctCtrlEviRefElemen>().idElementIntf_elemenRef
            ){
                // Devuelvo el ID del elemento del EVI en la panera (para buscarlo en las migas)
                Debug.Log("Id a buscar en " + elemento.GetComponent<ScriptDatosElemenItf>().idElementIntf + " en la panera");
                return elemento.GetComponent<ScriptDatosElemenItf>().idElementIntf;    // Encontrado
            }
        }
        return -1;        // No Encontrado
    }

    // Autor Nicolas Merino Ramirez
    // Fecha 14/11/2022
    // Descripcion
    //      Metodo que me devuelve el Evi buscado en la panera, por medio de su id unico,
    //      id almacenado en el script "ScriptDatosElemenItf"
    private GameObject bucar_Evi_En_Panera_By_ID(int id)
    {
        foreach (GameObject elemento in List_BreadcrumbsTrails)
        {
            if (id == elemento.GetComponent<ScriptDatosElemenItf>().idElementIntf)
            {
                // Devuelvo el elemento de id "x" en la panera
                Debug.Log("Buscar elem " + id + "\n" + "elemento encontrado: " + elemento);
                return elemento;    // Encontrado
            }
        }
        return null;    //Nunca deberia llegar aqui
    }
}
