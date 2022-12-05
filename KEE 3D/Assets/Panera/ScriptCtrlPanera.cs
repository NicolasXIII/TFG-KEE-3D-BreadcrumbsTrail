using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;  // Para usar filter, map y reduce


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

    /*
    // Autor Nicolas Merino Ramirez
    // Fecha 23/05/2022
    // Descripcion
    //      Annadir elementos al vector de migas de pan, y en caso de que el elemento ya este
    //      dentro del vector, se eliminan todos los que estan detras del elemento introducido
    public void annadir_Evi_A_Migas(GameObject evi)
    {
        Debug.Log("Entrando en panera _ annadir");
        
        int id_New_Evi = esta_el_elemento_referenciado_en_las_migas2(evi);
        // Si las migas de pan NO contienen el nuevo elemento, se annade
        if (id_New_Evi == -1)
        {
            evi.transform.SetParent(this.transform);    // Ahijo la miga de pan a la panera
            this.List_BreadcrumbsTrails.Add(evi);       // Annado la miga a la lista
            this.redimensionar_Evi(evi);
        }
        else // en caso contrario elimino todos los elementos detras del nuevo evi
        {
            Debug.Log(
                "ID de Evi que hay que borrar " + id_New_Evi + "\n" +
                "El evi es " + buscar_Evi_En_Panera_By_ID(id_New_Evi) + "\n" +
                "El evi esta en la posicion " + List_BreadcrumbsTrails.IndexOf(buscar_Evi_En_Panera_By_ID(id_New_Evi))
            );

            // Buscar elemento de id = "id_New_Evi" y borrar a partir de ahi
            int posicion_evi = this.List_BreadcrumbsTrails.IndexOf( buscar_Evi_En_Panera_By_ID(id_New_Evi) );
            GameObject elem_A_Eliminar = List_BreadcrumbsTrails[posicion_evi];
            
            Debug.Log(
                "posicion evi a borrar" + posicion_evi + "\n"
                + " el elemento a borrar es " + elem_A_Eliminar
            );

            // Borrar a partir del elemento que entra
            for (int i = posicion_evi; i < this.List_BreadcrumbsTrails.Count; i++)
            {
                // Lo pongo en un elem auxiliar puesto que lo tengo que eliminar de la lista
                GameObject eliminar = this.List_BreadcrumbsTrails[i];
                this.List_BreadcrumbsTrails.RemoveAt(i);
                eliminar.GetComponent<ScriptDatosElemenItf>().eliminaElemIntf();
            }
        }
    }*/

    // Autor Nicolas Merino Ramirez
    // Fecha        2022/05/23
    // Modificacion 2022/11/30
    // Descripcion
    //      Annadir elementos al vector de migas de pan, y en caso de que el elemento ya este
    //      dentro del vector, se eliminan todos los que estan detras del elemento introducido
    public void annadir_Evi_A_Migas(GameObject evi)
    {
        Debug.Log("Entrando en panera _ annadir");

        // Si el elemento referenciado esta en las migas, se eliminan los que van detras
        bool repetido = esta_el_elemento_referenciado_en_las_migas(evi);
        
        // Si las migas de pan NO contienen el nuevo elemento, se annade
        if (repetido == false)
        {
            /* 
            // Con este if, pretendo distinguir si entra un evi o un muro
            // es el presunto evi que entra de tipo "evi"?
            if ("evi" == evi.transform.Find("ContenedorDeEvi_01").transform.Find("EviRefElemen(Clone)").gameObject.GetComponent<SctCtrlEviRefElemen>().tipoElementIntf_elemenRef)
            {
                // El evi de referencia de la panera, le pongo en el script "SctCtrlEviRefElemen" valor al atributo "migaPan_MuroDestino",
                // dicho valor sera rellenado por el muro donde se abre el concepto original, el cual viene en el atributo "EviBase"
                //
                // ....migaPan_MuroDestino = ....EviBase
                evi.transform.Find("ContenedorDeEvi_01").transform.Find("EviRefElemen(Clone)").gameObject.GetComponent<SctCtrlEviRefElemen>().migaPan_MuroDestino 
                    =
                evi.transform.Find("ContenedorDeEvi_01").transform.Find("EviRefElemen(Clone)").gameObject.GetComponent<SctCtrlEviRefElemen>().EviBase;
            }*/

            evi.transform.SetParent(this.transform);    // Ahijo la miga de pan a la panera
            this.List_BreadcrumbsTrails.Add(evi);       // Annado la miga a la lista
            this.redimensionar_Evi(evi);
        }
    }

    // Autor Nicolas Merino Ramirez
    // Fecha 2022/10/19
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
            .2f,    // El evi ocupa el 20% de la panera en x
            1f,    // El evi ocupa el 100% de la panera en y
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
    // Fecha        23/05/2022
    // Modificacion 30/11/2022
    // Descripcion
    //      Metodo que me dice si la version original de una referencia se encuentra ya en la panera o no
    // Devolucion
    //      false     - No esta en la lista de migas de pan
    //      true      - Esta    en la lista de migas de pan y elimino hasta el
    private bool esta_el_elemento_referenciado_en_las_migas(GameObject evi)
    {
        int ref_Original = evi.transform.Find("ContenedorDeEvi_01").transform.Find("EviRefElemen(Clone)").gameObject.GetComponent<SctCtrlEviRefElemen>().idElementIntf_elemenRef;

        Debug.Log("metodo:  ->  esta_el_elemento_referenciado_en_las_migas\n" + "Nº Evi referencia: " + evi.GetComponent<ScriptDatosElemenItf>().idElementIntf + "\n" + "Nº Evi original de la ref: " + ref_Original);

        //GameObject anterior = null;

        foreach (GameObject elemento in List_BreadcrumbsTrails)
        {
            Debug.Log("metodo:  ->esta_el_elemento_referenciado_en_las_migas\n- foreach - elemento anterior " + anterior?.name);

            // Si el original' de la referencia esta en las migas de pan, devuelvo el true
            //
            // Explicacion del if
            //      - Saco el ID de elemento de interfaz al que hace referencia el evi que entra
            //      - Saco el ID de elemento de interfaz al que hace referencia cada uno de los elementos de la panera
            //  Si son iguales -> elemento repe -> borrar el resto
            if (ref_Original ==
                elemento.transform.Find("ContenedorDeEvi_01").transform.Find("EviRefElemen(Clone)").gameObject.GetComponent<SctCtrlEviRefElemen>().idElementIntf_elemenRef
            )
            {
                Debug.Log("metodo:  ->  esta_el_elemento_referenciado_en_las_migas\n" + "2 evis iguales en las migas (muros)");
                Debug.Log("metodo:  ->  esta_el_elemento_referenciado_en_las_migas\n" + "Id a buscar en " + elemento.GetComponent<ScriptDatosElemenItf>().idElementIntf + " en la panera");

                eliminar_Migas( 1 + 
                    List_BreadcrumbsTrails.IndexOf(
                        buscar_Evi_En_Panera_By_ID
                        (
                            elemento.GetComponent<ScriptDatosElemenItf>().idElementIntf
                        )
                    )
                );
                return true;
            }
            // el id de la referencia orinal del evi == al id del padre del evi orinal de la miga de pan
            //      para comprobar si cuando vamos hacia atras, el evi y el muro
            //
            // Explicacion del if
            //      - Saco el ID de elemento de interfaz al que hace referencia el evi que entra
            //      - Saco el ID de elemento de interfaz del padre del elemento que estoy comprobando
            //  Si son iguales, el muro que viene, es el padre del elemento que esta en la miga de pan
            else if (ref_Original ==
                elemento.transform.Find("ContenedorDeEvi_01").transform.Find("EviRefElemen(Clone)").gameObject.GetComponent<SctCtrlEviRefElemen>().EviBase.GetComponent<ScriptDatosElemenItf>().listaDePadres[0].GetComponent<ScriptDatosElemenItf>().idElementIntf)
            {
                Debug.Log("metodo:  ->  esta_el_elemento_referenciado_en_las_migas\n" + "Hijo de muro");
                Debug.Log("metodo:  ->  esta_el_elemento_referenciado_en_las_migas\n" + "Entra el muro " + evi.transform.Find("ContenedorDeEvi_01").transform.Find("EviRefElemen(Clone)").gameObject.GetComponent<SctCtrlEviRefElemen>().idElementIntf_elemenRef + " a las migas, del cual cuelga el concepto " + elemento.transform.Find("ContenedorDeEvi_01").transform.Find("EviRefElemen(Clone)").gameObject.GetComponent<SctCtrlEviRefElemen>().idElementIntf_elemenRef + " que esta en las migas de pan");

                eliminar_Migas( 0 +
                    List_BreadcrumbsTrails.IndexOf(
                        buscar_Evi_En_Panera_By_ID
                        (
                            elemento.GetComponent<ScriptDatosElemenItf>().idElementIntf
                        )
                    )
                );
                return true;
            }
            // Explicacion del if
            //      - Saco el ID de elemento de interfaz al que hace referencia el evi que entra
            //      - Saco el id del muro donde se abre el concepto,... que aparece en la miga de pan
            // Si son iguales, el muro que viene es hijo de un elemento de la panera, con lo que abria que borrar hasta dicho elemento (sin incluirlo)
            else if (ref_Original ==
                elemento.transform.Find("ContenedorDeEvi_01").transform.Find("EviRefElemen(Clone)").gameObject.GetComponent<SctCtrlEviRefElemen>().migaPan_MuroDestino.GetComponent<ScriptDatosElemenItf>().idElementIntf
                )
            {
                // 1+, porque -> el muro que viene es hijo de un elemento de la panera, con lo que abria que borrar hasta dicho elemento (sin incluirlo)
                eliminar_Migas(1 +
                    List_BreadcrumbsTrails.IndexOf(
                        buscar_Evi_En_Panera_By_ID
                        (
                            elemento.GetComponent<ScriptDatosElemenItf>().idElementIntf
                        )
                    )
                );

                return true;
            }
            // Caso de que sea un evi de "Instancia"
            //
            // Explicacion
            //    - Si el anterior no es null, es decir, no es el primer evi
            //    - Compruebo que el subtipo del evi anterior no sea de tipo Instancia (evi_baseInstFractal)
            // En caso de que sea de tipo instancia, borro tanto el muro donde se expande, como la referencia
            // a la instancia dentro de las migas de pan
            /*else if (anterior != null &&
                "evi_baseInstFractal" ==
                anterior.transform.Find("ContenedorDeEvi_01").transform.Find("EviRefElemen(Clone)").gameObject.GetComponent<SctCtrlEviRefElemen>().subTipoElementIntf_elemenRef)
            {
                Debug.Log("metodo:  ->  esta_el_elemento_referenciado_en_las_migas\n" + "Muro "+ elemento.name + " generado por la instancia " + anterior.name);

                // Borro la posicion del elemento (muro) + la posicion de la instancia que genera dicho muro
                eliminar_Migas(1 +
                    List_BreadcrumbsTrails.IndexOf(
                        buscar_Evi_En_Panera_By_ID
                        (
                            elemento.GetComponent<ScriptDatosElemenItf>().idElementIntf
                        )
                    )
                );
            }

            // Pongo el elemento actual como "anterior", para poder acceder a el en la siguiente iteracion
            anterior = elemento;*/
        }
        return false;
    }




    // Autor Nicolas Merino Ramirez
    // Fecha        26/11/2022
    // Modificacion 30/11/2022
    // Descripcion
    //      Metodo que elimina migas de pan hasta la posicion pasada,
    //      dicha posicion incluida
    private void eliminar_Migas(int posicion_evi)
    {
        /*// Borrar a partir del elemento que entra
        for (int i = posicion_evi; i < this.List_BreadcrumbsTrails.Count; i++)
        {
            // Lo pongo en un elem auxiliar puesto que lo tengo que eliminar de la lista
            GameObject eliminar = this.List_BreadcrumbsTrails[i];
            this.List_BreadcrumbsTrails.RemoveAt(i);
            eliminar.GetComponent<ScriptDatosElemenItf>().eliminaElemIntf();
        }*/

        for(int i = this.List_BreadcrumbsTrails.Count -1; i >= posicion_evi; i--)
        {
            // Lo pongo en un elem auxiliar puesto que lo tengo que eliminar de la lista
            GameObject eliminar = this.List_BreadcrumbsTrails[i];
            this.List_BreadcrumbsTrails.RemoveAt(i);                            // Borro de la lista
            eliminar.GetComponent<ScriptDatosElemenItf>().eliminaElemIntf();    // Borro elemento de interfaz
        }
    }

    // Autor Nicolas Merino Ramirez
    // Fecha 14/11/2022
    // Descripcion
    //      Metodo que me devuelve el Evi buscado en la panera, por medio de su id unico,
    //      id almacenado en el script "ScriptDatosElemenItf"
    private GameObject buscar_Evi_En_Panera_By_ID(int id)
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

    // Autor Nicolas Merino Ramirez
    // Fecha 14/11/2022
    // Descripcion
    //      Metodo que 
    public void click(GameObject evi)
    {
        // Para ir a una posicion en concreto dentro de las migas debo:
        //  - Eliminar todas las posiciones por la cola hasta llegar a la que me interesa
        //  Apunte:
        //      El "+1" es porque el metodo eliminar_Migas(), elimina hasta la posicion pasada, dicha posicion incluida
        eliminar_Migas(1 +
            List_BreadcrumbsTrails.IndexOf(
                buscar_Evi_En_Panera_By_ID
                (
                    evi.GetComponent<ScriptDatosElemenItf>().idElementIntf
                )
            )
        );


        // Como me preocupo de si el elemento que entra en las migas ya esta en ellas,
        // no tengo porque borrar los elementos de despues (?)
        evi.GetComponent<SctExpandirEvi>().botonExpandeEvi();
    }
}
