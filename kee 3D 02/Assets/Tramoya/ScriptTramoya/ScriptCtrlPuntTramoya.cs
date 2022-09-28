using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// //////////////////////////////////////////////////////////////////////////////////////
/// ///////////  Script de control del puntero que actua sobre los elementos de la tramoya
// Mueve el cursor para que el usuario pueda seleccionar botones y elementos en la tramoya, desde
// los muros de trabajo a la tramoya y vicebersa
// Autor : 	Miguel Angel Fernandez Graciani
// Fecha :	2020-05-28
// Observaciones :
// 		- Debemos tener al menos dos punteros. Uno para la mano derecha y otro para la izquierda
//				Se manejaran bien con dos ratones o bien con la pantlla tactil u otros elementos de control que pueda manejar el usuario (PENDIENTE MAFG 2020-05-28)
// 		- Cada puntero son en realidad un gupo de punteros, a saber:
//			- Puntero de Usuario : Es el que va pegado al usuario y por tanto se usa para manejar los elementos del muro de trabajo
//			- Puntero de Muro de Usuario :  Va con el puntero de usuario (es hijo suyo), pero en Z esta algo detras, para colocarse sobre el muro de usuario
//					Cuando el usuario se coloca en el muro de trabajo este puntero coloca sobre el muro de usuario. Se usa para manejar los elementos del muro de Usuario
//			- Puntero de Tramoya :  Va con el puntero de usuario (es hijo suyo), pero en Z esta algo detras, para colocarse sobre la tramoya
//					Cuando el usuario se coloca en el muro de trabajo este puntero coloca sobre la tramoya. Se usa para manejar los elementos de la tramoya
// 		- Este script maneja el comportamiento de puntero de la tramoya

public class ScriptCtrlPuntTramoya : MonoBehaviour
{

    public GameObject ctrlInterfaz;
    public GameObject Usuario;
    public GameObject PunteroUsuario;

    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        // Asignamos objetos
        ctrlInterfaz = GameObject.FindWithTag("ctrlInterfaz");
        Usuario = GameObject.FindWithTag("Usuario");
        PunteroUsuario = GameObject.FindWithTag("PunteroUsuario");

        // Lo ponemos por defecto en estado activo
        this.GetComponent<ClassPuntero>().estadoPuntero = ClassPuntero.estPuntero_desactivado;  // Inicialmente el puntero de la tramoya esta desactivaso, se activara cuando se active la tramoya

    }

    // Update is called once per frame
    void Update()
    {
        // Ajustamos la posicion de este puntero de tramoya
        // EL PUNTERO DE MURO DE TRAMOYA ES HIJO DEL PUNTERO DE USUARIO, POR LO QUE SUS CORRDENADAS SON COORDENADAS LOCALES A ESTE
        // OJOO. esta en otro plano que el puntero de usuario, por lo que al moverse con su padre (puntero de usuario), debido a la 
        // prespectiva de la camara, se desplaza separado de este (el puntero de usuario) y queremos verlos como uno solo para poder
        // realizar su funcion de puntero de forma adecuada.
        // EL plano de usuario esta mas lejos de la camara, y para que este en el mismo campo de vision que el muro de trabajo, este es mas pequeño
        // y el puntero de tramoya si se desplaza lo mismo  que el de usuario (en x e y), visualmente se desplaza mucho mas.
        // para que los dos punteros se vean como uno solo realizamos el siguiente ajuste :
        // VER GRAFICO "calculo posicion puntero muro usr.png" donde se realiza el calclo pertinente

        Vector3 posicion = new Vector3();

        float PU_x = PunteroUsuario.transform.localPosition.x;
        float PU_y = PunteroUsuario.transform.localPosition.y;
        float DC = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().distancia_z_CamaraUsuario;
        float DPT = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().distancia_z_PuntTramoya;

        // realizamos el calculo. Lo dividimos por la escala del puntero de usuario, (que es su padre), para ajustar la escala del calculo
        float posLocal_x = (-1) * (PU_x - (((DC - DPT) * PU_x) / DC)) * (1 / ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escala_x_PunteroUsuario);
        float posLocal_y = (-1) * (PU_y - (((DC - DPT) * PU_y) / DC)) * (1 / ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escala_y_PunteroUsuario);

        //        Debug.Log("Desde ScriptCtrlPuntMuroUsuario Calculando posision puntero muro usuario con  - PU_x = " + PU_x +
        //            " - PU_y = " + PU_y  +
        //            " - DC = " + DC +
        //            " - DPMU = " + DPMU +
        //            " - posLocal_x = " + posLocal_x +
        //            " - posLocal_y = " + posLocal_y );

        posicion.x = posLocal_x;
        posicion.y = posLocal_y;
        posicion.z = DPT;

        // Incluimos los ajustes en la posicion
        //        transform.localPosition = posicion;
        transform.localPosition = posicion;


    }

    /// /////////////////////////////////////////////
    /// /////////////////////////////////////////////
    /// Vamos con los trigers

    // Cuando el Game object Usuario LLEGA a un muro de trabajo, este pasa a estar ACTIVO
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Telon"))
        {
            if (DatosGlobal.niveDebug > 1000)
            {
                Debug.Log("Dede ScriptCtrlPuntTramoya => OnTriggerEnter. He llegado en el cuadro = " + ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().numDeFrame +
                " - Colision con el oggeto other de name = " + other.name);
            }

            // miramos si el usuario esta arrastrando algun elemento de la interfaz
            if (ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().objetoEnArrastre != null)
            {
                GameObject objetoEnArrastre = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().objetoEnArrastre;
                // Cuando un elemento pasa a la tramoya, o bien es el mismo elemento el que pasa del origen a la tramoya y viceversa, o bien
                // se genera un evi "EviRefElemen" de referencia a dicho elemento de interfaz
                // esto tiene que ser asi porque quizas podamos llevarnos a la mochila, ramas, muros u otras cosas que no tienen una base de evi.
                //
                // Segun lo que arrastremos a la tramoya, actuaremos en consecuencia. A la tramoya se puede arrartrar :
                //      1.) Si arrastramos un evi de rama => generamos un EviRefElemen que hace referencia a la rama asociada al evi de rama
                //              - generamos un evi de referencia a elemento Llamamos al generador .generaEviRefElemen(objetoEnArrastre, other.gameObject);
                //              - Cuando se descarga en un muro actvo (¿Se copia o se ingerta la rama en este mudo???? PENDIENTE 2021-06-16)
                //              - Cuando se descarga en un buscador (¿Se hace una busqueda de ramas similares???? PENDIENTE 2021-06-16)
                //                ctrlInterfaz.GetComponent<ScriptLibGestorEvis>().generaEviRefElemen(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().objetoEnArrastre, other.gameObject);
                //      2.) Si arrastramos un evi buscador => el evi pasa del muro activo a la tramoya
                //              - y de la tramoya al muro activo
                //      3.) Si arrastramos un evi Fractal de referencia, instancia o sin techo u otros => el evi pasa del muro activo a la tramoya 
                //              - y de la tramoya al muro activo
                //              - Desaparece del lugar de origen (si se quiere dejar alli hay que hacerle una copia y mover la copia

                // Comenzamos por lo mas especifico y luego lo general lo metemos todo en la bolsa del else final
                //      Vamos con 1.) Si arrastramos un evi de rama => generamos un EviRefElemen que hace referencia a la rama asociada al evi de rama
                if (objetoEnArrastre.gameObject.GetComponent<ScriptDatosElemenItf>().subTipoElementIntf == ScriptDatosElemenItf.subTipoElemItf_evi_rama)
                {
                    ctrlInterfaz.GetComponent<ScriptLibGestorEvis>().generaEviRefElemen(objetoEnArrastre, other.gameObject);
                }
                //      Vamos con 2.) Si arrastramos un evi buscador
                else if (objetoEnArrastre.gameObject.GetComponent<ScriptDatosElemenItf>().subTipoElementIntf == ScriptDatosElemenItf.subTipoElemItf_evi_camino_00)
                {
                    // PENDIENTE MAFG 2021-06-16, por ahora no tenemos evis de camino
                }
                // Vamos con 3.) Si arrastramos un evi Fractal de referencia, instancia o sin techo u otros
                else
                {
                    // Desahijamos el elemento del telon (en la gerarquia que no es unity)
                    GameObject objPadre = objetoEnArrastre.gameObject.transform.parent.gameObject;
                    objetoEnArrastre.gameObject.GetComponent<ScriptDatosElemenItf>().gestionaArbolGenealogico(objPadre, "desahija");

                    // - Hacemos al evi que arrastramos hijo del elemento de la tramoya que corresponda
                    objetoEnArrastre.gameObject.transform.SetParent(other.gameObject.transform);

                    // ahijamos el elemento al telon (en la gerarquia que no es unity)
                    objetoEnArrastre.gameObject.GetComponent<ScriptDatosElemenItf>().gestionaArbolGenealogico(other.gameObject, "ahija");

                    // Ajustamos la escala 
                    objetoEnArrastre.gameObject.transform.localScale = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().estaEscalaBaseDeEviEnTramoya;

                    // Lo colocamos en su posicion en la tramoya
                    Vector3 posicionObjetoEnArrastre = objetoEnArrastre.gameObject.GetComponent<ScriptCtrlBaseDeEvi>().calculaPosicionEvi();
                    objetoEnArrastre.gameObject.transform.localPosition = posicionObjetoEnArrastre;
                }
            }  // Fin de - if (ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().objetoEnArrastre != null)
        }  //  FIn de - if (other.gameObject.CompareTag("Telon"))
    } // Fin de - void OnTriggerEnter(Collider other) 

    // Cuando el Game object Usuario SALE de la tramoya
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Telon"))
        {
            if (DatosGlobal.niveDebug > 1000)
            { Debug.Log("Dede ScriptCtrlPuntTramoya => OnTriggerExit. He salido en el cuadro = " + ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().numDeFrame); }
            // miramos si el usuario esta arrastrando algun elemento de la interfaz
            // Si en el telon correspondiente estamos arrastrando algun tipo de evi, y salimos de la tramolla durante el arratre, debemos sacar el evi del telon
            // y ponerlo en el muro activo que hay debajo del telon (en el muro activo)
            if (ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().objetoEnArrastre != null)
            {
                GameObject objetoEnArrastre = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().objetoEnArrastre;

                // Desahijamos el elemento del telon (en la gerarquia que no es unity)
                objetoEnArrastre.gameObject.GetComponent<ScriptDatosElemenItf>().gestionaArbolGenealogico(other.gameObject, "desahija");

                // - Hacemos al evi que arrastramos hijo del muro activo
                objetoEnArrastre.gameObject.transform.SetParent(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().muro_Activo.transform);

                // ahijamos el elemento del telon (en la gerarquia que no es unity)
                objetoEnArrastre.gameObject.GetComponent<ScriptDatosElemenItf>().gestionaArbolGenealogico(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().muro_Activo.transform.gameObject, "ahija");

                // Ajustamos la escala 
                objetoEnArrastre.gameObject.transform.localScale = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().estaEscalaBaseDeEvi_01;


                // Ajustamoes el modo del elemento que arrastramos al modo del nuevo padre que lo alberga (el muro activo)
                // Ya se coloca como hijo de su padre en 
                ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().objetoEnArrastre.gameObject.transform.GetComponent<ScriptDatosElemenItf>().modo = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().muro_Activo.transform.GetComponent<ScriptDatosElemenItf>().modo;

                // Colocamos el evi que hemos ahijado en el muro activo, en la posicion del puntero de usuario
                Vector3 posicionDePunteroUsuario = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().PunteroUsuario.transform.localPosition;
                float pos_x = (posicionDePunteroUsuario.x / ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escala_x_MuroTrabajo); // es - porque el boton de desplazar esta abajo a la derecha
                float pos_y = (posicionDePunteroUsuario.y / ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escala_y_MuroTrabajo);
                float pos_z = 0f;
//                pos_x = pos_x - ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().posicionAnchoBaseBotones * ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().factorEscalaBaseDeEvi; // es - porque el boton de desplazar esta abajo a la derecha
//                pos_y = pos_y + ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().posicionAltoBaseBotones * ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().factorEscalaBaseDeEvi;

                //  Por fin, definimos la posicion del evi base
                Vector3 nuevaPosicionDeBaseDeEvi = new Vector3(pos_x, pos_y, pos_z);
                ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().objetoEnArrastre.gameObject.transform.localPosition = nuevaPosicionDeBaseDeEvi;


            }

        }
    } // Fin de - void OnTriggerEnter(Collider other) 

}
