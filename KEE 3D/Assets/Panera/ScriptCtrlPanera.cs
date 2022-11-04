using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptCtrlPanera : MonoBehaviour
{
    public GameObject ctrlInterfaz;
    public List<GameObject> List_BreadcrumbsTrails;

    // Start is called before the first frame update
    void Start()
    {
        ctrlInterfaz = GameObject.FindWithTag("ctrlInterfaz");
        
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
        // Si las migas de pan NO contienen el nuevo elemento, se annade
        if (!this.List_BreadcrumbsTrails.Contains(evi))
        {
            // Ahijo la miga de pan a la panera
            evi.transform.SetParent(this.transform);

            this.List_BreadcrumbsTrails.Add(evi);
        }
        else // en caso contrario elimino todos los elementos detras del nuevo evi
        {
            int posicion_evi = this.List_BreadcrumbsTrails.IndexOf(evi);
            this.List_BreadcrumbsTrails.RemoveRange(posicion_evi + 1, this.List_BreadcrumbsTrails.Count - (posicion_evi + 1));
        }

        this.redimensionar(evi);

    } // Fin de - public void annadir_Evi_BreadcrumbsTrail(GameObject evi)

    // Autor Nicolas Merino Ramirez
    // Fecha 19/10/2022
    // Descripcion
    //      Redimensiona los EVIs en la panera para que tengan un tamanno adecuado y los muestra
    //      con un formato adecuado
    void redimensionar(GameObject evi)
    {
        // pongo a los elementos de la panera, la escala de los botones de herramientas,...
        evi.transform.localScale = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escala_BtnMenu_N2_1_Herramientas_MigaPan;

        // pongo el evi en la panera, pero en el eje x lo muevo ".12f" para crear una separacion
        evi.transform.localPosition = new Vector3
        (
            this.transform.localPosition.x
            // Sumo el espacio que ocupa un Evi * el Numero de evis -1 y dejo un espacio entre medias
            + (this.List_BreadcrumbsTrails.Count - 1) * evi.transform.localScale.x + .12f,  
            this.transform.localPosition.y * 1/2 + evi.transform.localScale.y * 1/2,    // Centrar en el eje y
            this.transform.localPosition.z - 1f
        );
    }
}
