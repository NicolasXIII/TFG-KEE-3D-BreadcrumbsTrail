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
        
       // this.transform.localPosition = new Vector3(-22f, -20f, -15f);
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
        // Si las migas de pan NO contienen el nuevo elemento, se annade
        if (!this.List_BreadcrumbsTrails.Contains(evi))
        {
            this.List_BreadcrumbsTrails.Add(evi);
        }
        else // en caso contrario elimino todos los elementos detras del nuevo evi
        {
            int posicion_evi = this.List_BreadcrumbsTrails.IndexOf(evi);
            this.List_BreadcrumbsTrails.RemoveRange(posicion_evi + 1, this.List_BreadcrumbsTrails.Count - (posicion_evi + 1));
        }

        // Ahijo la miga de pan a la panera
        evi.transform.SetParent(this.transform);

        this.redimensionar(evi);

    } // Fin de - public void annadir_Evi_BreadcrumbsTrail(GameObject evi)

    // Autor Nicolas Merino Ramirez
    // Fecha 19/10/2022
    // Descripcion
    //      Redimensiona los EVIs en la panera para que tengan un tamanno adecuado
    void redimensionar(GameObject evi)
    {
        Debug.Log("Escala del Evi " + evi.transform.localScale);
        Debug.Log("Escala de los botones " + ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escala_BtnMenu_N2_1_Herramientas_MigaPan);
        // pongo a los elementos de la panera, la escala de los botones de herramientas,...
        evi.transform.localScale = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escala_BtnMenu_N2_1_Herramientas_MigaPan;
        Debug.Log("Escala del Evi despues de la transformacion" + evi.transform.localScale);

        // pongo el evi en la panera, pero en el eje x lo muevo ".12f" para crear una separacion
        evi.transform.localPosition = new Vector3
        (
            this.List_BreadcrumbsTrails.Count + .12f,
            this.transform.position.y,
            this.transform.position.z
        );
    }
}
