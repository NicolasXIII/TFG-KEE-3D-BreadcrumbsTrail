using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


/* ************************************************************************************************** ScriptGestionaEscena
*****************************************************************************************************
                        Script de Gestion de la interfaz 
*****************************************************************************************************
*****************************************************************************************************
Gestiona la interfaz, las escenas, las ramas, los muros , sus evis hijosa y sus ramas hijas
  La informacion del estado de la escena esta en el fichero DAUS de usuario, que es un KDL del concepto de DAUS
Autor : 	Miguel Angel Fernandez Graciani
Fecha :	2020-03-30
Modificaciones :
      - 2020-07-20, incluyo los accesos reales a los DAUS remotos y comienzo a programar la gestion del DAUS y la interfaz
Observaciones :
 		- El fichero DAUS es un KDL instancia de DAUS que contiene la configuracion de la interfaz, SU estructura basicamente es:
               INTERFAZ DE USUARIO
                  - Escena(s)
                  - Raiz del arbol (rama principal)
                      - * Muro(s)
                          - EVI(s)
                              - Rama(s)
                                  - Muro(s) Recursivamente a *
                                      Las ramas tienen muros, en los que hay evis de los que pueden salir ramas con otros muros que tiene otros evis y asi sucesivamente
                  - Historial (ultimas localizaciones del usuario
                  - Estado de usuario 
                      - muro de usuario

                      - Opciones (sonido, idiomas, etc...)
                      - Mochila
                      - Agentes

      - Todos los conceptos del DAUS (escenas, ramas, muros y evis) tienen su Key y su Host.
              - El Key lo va proporcionando el gestor de interfaz, con la intencion de que no se repita ninguno en toda la interfaz
              - EL host de todos los conceptos de la interfaz es el mismo. Lo llamamos
                          host = "raizDelConcepto"
                  Esto es asi porque son entidades que tienen sentido dentro del concepto total (interfaz de usuario en este caso), pero 
                  NO tienen sentido furera de estos (si llegaran a tenerlo algun host los apadrinara)

	DATOS GENERALES :
        - De todas las entiodades
              - Key (el KEY es unico en la interfaz y lo define este script para que no este duplicado)
              - Host = "raizDelConcepto"  // Para todos salvo el concepto General que instancia el DAUS ( que tendra de host el que lo hospede y evidentemente
                                          todos los conceptos que se referencian o nstancian
             Todas las entidades instancian a escena, rama, muro o evi
        - Escena :
              - Nombre de escena
              - Descripcion
              - Ordinal
              - Fecha de creacion
              - Fecha de ultima modificacion
        - Rama :
              - Rama madre
              - EVI Padre
              - Punto de origen
              - Direccion global
              - Distancia entre muros
        - Muro
            - Rama en la que esta
            - Posicion global
            - Factor de escala
        - Evi
            - Muro en el que esta
            - Posicion global
            - Factor de escala
            - Expandifo SI/NO
            - Minimizado SI/NO


*****************************************************************************************************
	METODOS GENERALES
        (ver el documento "D:\Datos\Ideando\Desarrollo\KLW\KEE3D\Documentos de especificacion/Especificacion funcional.docx". 
        Mas concretamente el apartado de o	Botones de control de interfaz (generales a toda la interfaz))

Metodos Generados



Metodos a generar
    




 * *****************************************************************************************************
*****************************************************************************************************
*****************************************************************************************************
***************************************************************************************************** */

public class ScriptGestionaDAUS : MonoBehaviour {

	public GameObject ctrlInterfaz;
	public GameObject Usuario;

    // Para pruebas cargamos el DAUS de Paulino
    public string rutaACargar = "http://crab.uclm.es/klw/pruebasPau/uploads/gen_miDAUS_limpio.xml";
    //       ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().DausInterfaz

    // Inicializamos la escena
    void Start () {
			// Asignamos objetos
			ctrlInterfaz = GameObject.FindWithTag("ctrlInterfaz");
        //			Usuario = GameObject.FindWithTag("Usuario");

        rutaACargar = DatosGlobal.Dks_madre_host;  // El host del DKS es la localizacion en si Del DKS evidentemente

        // Del DKS debe venir 

        // ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().host_efimero_de_interfaz =  El identificador del KEE como generador de 
        //                                                                              conceptos (para que los conceptos generados puedan tener un host)

        // PENDIENTE MAFG 2022-03-20 OJO, por ahora y hasta que programemos el acceso mediante el usuario al DKS, ponemos estos datos a pelo para que funcionen 
        // los chequeos al enviar ficheros y otros.
        // Ojo esto, mientras no lo programemos como dios manda, debe coincidir con :
        //      $_SESSION["clave_usuario"] = "valor_key_usr_provisional";
        //      $_SESSION["host_usuario"] = "valor_host_usr_provisional";
        // en el fichero "identificadoresDeConceptos.php" del DKS
        ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().usrKey = "valor_key_usr_provisional";
        ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().usrHost = "valor_host_usr_provisional";

        // ////////////////////////////////////////
        // ////////////////////////////////////////
        // ////////////////////////////////////////
        // Cargar el Daus del usuario

        // ////////////////////////////////////////
        // ////////////////////////////////////////
        // ////////////////////////////////////////
        // Cargar el Daus del usuario

        // Cargamos los datos del EVI, llamando a su DKS correspondiente
        // Arrancamos una corrutina para que traiga el KDL de forma asincrona (sin detener la ejecucion)
        //       StartCoroutine("cargaDausInterfaz");
        // /////////////////////////////////////////
        // Configurar la interfaz de usuario segun el daus

        // PENDIENTES 
        //      - poner el valor de "ultimoIdDeSolicitud" en "ScriptGestorSolicitudes" para continuar con el numero de solicitud adecuado


    } // Fin de - void Start () {

    // Update is called once per frame
    void Update () {
		
	}  // Fin de - void Update () {

    IEnumerator cargaDausInterfaz()
    {

        UnityWebRequest www = UnityWebRequest.Get(rutaACargar);
        yield return www.Send();

        if (www.isNetworkError)
        {
            if (DatosGlobal.niveDebug > 90)
            { Debug.Log(www.error); }
        }
        else
        {
            // Show results as text
            if (DatosGlobal.niveDebug > 90)
            { Debug.Log(www.downloadHandler.text); }
        }

        if (DatosGlobal.niveDebug > 1000)
        { Debug.Log("Aqui fin de la carga del daus de escena"); }

    } // FIn de - IEnumerator cumplimentaDatosEviTipo00()




}
