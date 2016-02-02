using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
    using UnityEditor;
#endif
using System.Collections.Generic;
using System.Collections;
using System;
using System.Xml;


/*
public class Janela
{
   // ObjectPooling PiscinaDeObjetosManager;
    Dictionary<int,Nodo> DicionarioDeNodos= new Dictionary<int,Nodo>();
    Dictionary<int, LinkUI> DicionarioDeLinks= new Dictionary<int,LinkUI>();
    Dictionary<int, Nodo> DicionarioDeTemplatesDeNodos= new Dictionary<int,Nodo>();

   

    public Janela() { }

    public void AdicionarNodo() 
    {
        Nodo novonodo = new Nodo(CriarIdOriginalNodo());
        DicionarioDeNodos.Add(novonodo.id, novonodo);
    }
    int CriarIdOriginalNodo()
    {
        for (int i = 0; i < 100; i++)
        {
            if (DicionarioDeNodos.ContainsKey(i) == false)
                return i;
        }
        return -1;
    }
    
    public bool RemoverNodo(int id)
    {
        foreach (var idlink in DicionarioDeNodos[id].TodosIDsDosLinks())
            DicionarioDeLinks.Remove(idlink);

        //DicionarioDeNodos[id].RemoverTodasAsPortas();
        return DicionarioDeNodos.Remove(id);
    }
    
    public void RemoverTodosNodos()
    {
        foreach (var id in DicionarioDeNodos.Keys)
            RemoverNodo(id);

    }
    public void CriarLink(PortaUI porta1, PortaUI porta2)
    { 
        LinkUI linkNovo = new LinkUI(porta1,porta2, CriarIdOriginalLink());
        if (linkNovo.tipoLink == TipoDeLigacao.Errada)
            return;
        DicionarioDeLinks.Add(linkNovo.id, linkNovo);
    }
    int CriarIdOriginalLink() 
    { 
        for (int i = 0; i < 100; i++) 
        { 
            if (DicionarioDeLinks.ContainsKey(i) == false) 
                return i; 
        }
        return -1;
    }
    /*
    public bool RemoverLink(PortaUI porta1, PortaUI porta2 = null) 
    {
        if(porta2==null)
        {
            foreach (var linka in porta1.listaDeLinks)
                DicionarioDeLinks.Remove(linka.id);
            porta1.RemoverTodosLinks();
            return true;
        }
        Link link = porta1.ObterLink(porta2);
        porta1.RemoverLink(link.id);
        porta2.RemoverLink(link.id);
        return DicionarioDeLinks.Remove(link.id);   
    }
    
    public void AdicionarTemplateNodo() { }
    public void Exportar(){}//em xml
    public void Importar(){} // em xml
}
*/
//sempre atribuido ao um Panel



public enum ModoMouse
{
    Idle = 0,
    Resizing,
    MovingNodo,
    Connecting
}
public class JanelaUI : MonoBehaviour
{
    public GameObject CameraGameObjct;
    public GameObject PrefabCanvas;
    RectTransform canvas;
    //public  GameObject PrefabBotaoImportar;
   // public  GameObject PrefabBotaoExportar;
    public GameObject PrefabBotaoLimpar;
    public GameObject PrefabPorta;
    public GameObject PrefabNodo;
    public GameObject PrefabLink;
    public GameObject PrefabNodoMenu;
    public GameObject PrefabJanelaMenu;
    public GameObject PrefabLinkMenu;
    public GameObject Propriedades;
    public GameObject PropriedadeString;
    public GameObject PropriedadeBool;

    //Dictionary<int, GameObject> DicionarioDeNodos = new Dictionary<int, GameObject>();
   // Dictionary<int, GameObject> DicionarioDeLinks = new Dictionary<int, GameObject>();

    NodoUI[] ArrayDeNodos;
    LinkUI[] ArrayDeLinks;
    public GameObject LinkConnecting;
    BezierManager BezierLinkConnecting;
    public NodoUI nodoSelecionadoAtualmente;
    public PortaUI PortaSelecionadaAtualmente;
    public LinkUI LinkSelecionadoAtualmente;
    public ModoMouse modoMouse=ModoMouse.Idle;
    void Start()
    {
        ArrayDeLinks = new LinkUI[200];
        ArrayDeNodos = new NodoUI[200];
        CameraGameObjct = GameObject.Find("Main Camera");
        PrefabCanvas = GameObject.Find("Canvas");
         canvas = PrefabCanvas.GetComponent<RectTransform>();
        PrefabNodo = GameObject.Find("Canvas/Nodo");
        PrefabNodo.SetActive(false);
        PrefabPorta = GameObject.Find("Canvas/Porta");
        PrefabPorta.SetActive(false);
        PrefabNodoMenu = GameObject.Find("Canvas2/MenuNodo");
        PrefabNodoMenu.SetActive(false);
        PrefabJanelaMenu = GameObject.Find("Canvas2/MenuJanela");
        PrefabJanelaMenu.SetActive(false);
        PrefabLinkMenu = GameObject.Find("Canvas2/MenuLink");
        PrefabLinkMenu.SetActive(false);
        Propriedades = GameObject.Find("Canvas2/Propriedades");
        Propriedades.SetActive(false);
        PropriedadeString = Propriedades.transform.FindChild("ScrollRect/ListaDePropriedades/PropriedadeString").gameObject;
        PropriedadeString.SetActive(false);
        PropriedadeBool = Propriedades.transform.FindChild("ScrollRect/ListaDePropriedades/PropriedadeBool").gameObject;
        PropriedadeBool.SetActive(false);
         
        PrefabLink = GameObject.Find("Canvas/Link");
        LinkConnecting = GameObject.Instantiate( PrefabLink);
        LinkConnecting.GetComponent<LinkUI>().enabled = false;
        LinkConnecting.SetActive(false);
        BezierLinkConnecting = LinkConnecting.GetComponent<BezierManager>();
        memoria = modoMouse;
    }


    ModoMouse memoria;
    Vector2 oldMouePosition = Vector2.zero;
    public float xMovement;
    public  float yMovement;


    void Update()
    {
        if (PortaSelecionadaAtualmente != null && modoMouse!=ModoMouse.Connecting)
            Debug.Log("Está errado?"+PortaSelecionadaAtualmente.name);

       
        MovendoNodo();
        InputAtalhos();
        ArrasteCamera();
        Connecting();
        
    }
    void InputAtalhos()
    {
        if (Input.GetKey("left shift") )
        {
            if (Input.GetKeyDown("c"))
                MoverNodoParaOCentro();
            if (Input.GetKeyDown("d"))
                DuplicarNodo();
            if (Input.GetKeyDown("p"))
                nodoSelecionadoAtualmente.AdicionarPorta();  
            if (Input.GetKeyDown("x"))
                RemoverNodo();              
            if (Input.GetKeyDown("f"))
                FocusOnNodoSelecionado();
            if (Input.GetKeyDown("w"))
                AdicionarNodo();
            if (Input.GetKeyDown("r"))
                OrganizarNodosEmArvore();
            if (Input.GetKeyDown("g"))    
                if (modoMouse == ModoMouse.Idle)            
                    modoMouse = ModoMouse.MovingNodo;
                else
                    if (modoMouse == ModoMouse.MovingNodo)
                        modoMouse = ModoMouse.Idle;
                   
           
        }
        
    }
    void ArrasteCamera()
    {
        Vector2 currentMousePosition = Input.mousePosition;

        xMovement = oldMouePosition.x - currentMousePosition.x;
        yMovement = oldMouePosition.y - currentMousePosition.y;
        oldMouePosition = currentMousePosition;

        if (Input.GetMouseButton(2))
        {
            float taxa = Mathf.Abs(CameraGameObjct.transform.position.z / 500);
            //Debug.Log("taxa="+taxa);
            CameraGameObjct.transform.position = CameraGameObjct.transform.position + new Vector3(xMovement, yMovement) * taxa;
        }
    }

    void MovendoNodo()
    {
       
        if(modoMouse==ModoMouse.MovingNodo)
        {
            if(nodoSelecionadoAtualmente==null)
            {
                modoMouse = ModoMouse.Idle;
                return;
            }
            nodoSelecionadoAtualmente.transform.position = nodoSelecionadoAtualmente.transform.position - new Vector3(xMovement,yMovement)*2;
            nodoSelecionadoAtualmente.AtualizarLinks();
        }
    }
    bool MenuLink()
    {     //tenho que adicionar um collider no link e edita-lo proceduralmente no Render() do bezier script
            int link = RaycastLink();
            
        Debug.Log(link);
            if (link >= 0)
            {
                Debug.Log("menu link");
                PrefabLinkMenu.SetActive(true);
                return true;
            }
            return false;
    }
    
    public void MenuPropriedades()
    {
        nodoSelecionadoAtualmente.MenuPropriedades();
    }
    int RaycastLink()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * 10000, Color.yellow,100);
      
        if (Physics.Raycast (ray,out hit,1000))
        {
            var link = hit.transform.GetComponent<LinkUI>();
            if(link!=null)
            {
                LinkSelecionadoAtualmente = link;
                return link.id; 
            }
            // Do something with the object that was hit by the raycast.
        }
        LinkSelecionadoAtualmente = null;
        return -1;
    }
    void Connecting()
    {
        if (memoria != modoMouse)
        {
            if (modoMouse == ModoMouse.Connecting)
            {
                LinkConnecting.SetActive(true);
                BezierLinkConnecting.lineRenderer.SetColors(Color.green, Color.green);
            }
            if (modoMouse == ModoMouse.Idle)
            {
                LinkConnecting.SetActive(false);
            }
            memoria = modoMouse;
        }
        if (modoMouse == ModoMouse.Connecting)
        {
            var v3 = Input.mousePosition;
            v3.z = -Camera.main.transform.position.z;
            v3 = Camera.main.ScreenToWorldPoint(v3);

            BezierLinkConnecting.Render(PortaSelecionadaAtualmente.transform.position, v3);
        }
    }

    public void AdicionarNodo2()
    {
        NodoUI novo = GameObject.Instantiate(PrefabNodo).GetComponent<NodoUI>();
        novo.gameObject.SetActive(true);
        novo.enabled = true;
        novo.transform.SetParent(this.transform, true);

      //  Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
       // Debug.Log(ray.origin);
        int idnovo = CriarIdOriginalNodo();
        novo.name = "Nodo" + idnovo.ToString();
        novo.GetComponent<NodoUI>().id = idnovo;
        ArrayDeNodos[idnovo] = novo;
        novo.Selecionar();
        MoverNodoParaOCentro();
    //    DicionarioDeNodos.Add(idnovo, novo);
      //  novo.GetComponent<NodoUI>().AdicionarPorta();
    }
    public NodoUI AdicionarNodo()
    {
        NodoUI novo = GameObject.Instantiate(PrefabNodo).GetComponent<NodoUI>();
        novo.gameObject.SetActive(true);
        novo.enabled = true;
        novo.transform.SetParent(this.transform,true);

       // Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //Debug.Log(ray.origin);
        int idnovo=CriarIdOriginalNodo();
        novo.name = "Nodo"+idnovo.ToString();
        novo.id = idnovo;
        ArrayDeNodos[idnovo] = novo;

        novo.Selecionar();
        MoverNodoParaOCentro();
       // DicionarioDeNodos.Add(idnovo, novo);
    ///  novo.GetComponent<NodoUI>().AdicionarPorta();
        return novo;
    }
    public NodoUI DuplicarNodo()
    {
        if (nodoSelecionadoAtualmente == null)
            return null;
       NodoUI novo= AdicionarNodo();
       for (int i = 0; i < nodoSelecionadoAtualmente.listaDePortas.Count / 2; i++)
       {
           novo.AdicionarPorta();
       }
       foreach (var prop in nodoSelecionadoAtualmente.listaDePropriedades)
       {
          var novaProp= novo.AdicionarPropriedade();
          novaProp.nome = prop.nome;
          novaProp.Valor = prop.Valor;
          novaProp.SetarNome();
          novaProp.SetarValor();
       }
        return novo;
    }
    public void RemoverNodo()
    {
        if(nodoSelecionadoAtualmente==null)
        {
            Debug.Log("nada a apagar");
            return;
        }
        int id = nodoSelecionadoAtualmente.id;
      //  Debug.Log("id a ser removido=" + id);
      //  Debug.Log(DicionarioDeNodos.Count);
     //   DicionarioDeNodos[id].GetComponent<NodoUI>().RemoverTodasAsPortas();
        ArrayDeNodos[id].RemoverTodasAsPortas();
       // GameObject.Destroy(DicionarioDeNodos[id]);
        GameObject.Destroy(ArrayDeNodos[id].gameObject);
        Debug.Log("nodo destruido");
       // DicionarioDeNodos.Remove(id);
        ArrayDeNodos[id] = null;
        nodoSelecionadoAtualmente = null;
    }
    public void RemoverTodosNodos() 
    {
     //   foreach (var id in new List<int>(DicionarioDeNodos.Keys))
        for (int id = 0; id < ArrayDeNodos.Length;id++ )
        {
            if (ArrayDeNodos[id] != null)
            {
                //   nodoSelecionadoAtualmente = DicionarioDeNodos[id];
                nodoSelecionadoAtualmente = ArrayDeNodos[id];
                RemoverNodo();
            }
        }
    }
    public void AdicionarLink(PortaUI porta1, PortaUI porta2)
    {
        if (JaExisteEsseLink(porta1, porta2))
        {          
           // Debug.Log("link ja existe1!!!");
            return;
        }
        LinkUI novo = GameObject.Instantiate(PrefabLink).GetComponent<LinkUI>();
        int idnovo = CriarIdOriginalLink();
        novo.name = idnovo.ToString();
        novo.SetLinkUI(porta1, porta2, idnovo);
        novo.transform.SetParent(this.transform, true);

        porta1.listaDeLinks.Add(novo);
        porta2.listaDeLinks.Add(novo);
      //  Debug.Log("adicionando link em:" + porta1.name +" e em:" + porta2.name);
        //Debug.Log( porta1.listaDeLinks.Count + porta2.listaDeLinks.Count);
       // DicionarioDeLinks.Add(idnovo, novo);
        ArrayDeLinks[idnovo] = novo;
    }
    public bool JaExisteEsseLink(PortaUI porta1, PortaUI porta2)
    {
        if (porta1.tipoDePorta == TipoDePorta.Entrada)
            return porta1.listaDeLinks.Exists(x => x.saida == porta2);
        else
            return porta1.listaDeLinks.Exists(x => x.entrada == porta2);
            
    }
    public void RemoverLink()
    {
        //melhorar esse script
        int id = LinkSelecionadoAtualmente.id;
        LinkSelecionadoAtualmente.entrada.listaDeLinks.Remove(LinkSelecionadoAtualmente.entrada.listaDeLinks.Find(x => x.id == id));
        LinkSelecionadoAtualmente.saida.listaDeLinks.Remove(LinkSelecionadoAtualmente.saida.listaDeLinks.Find(x => x.id == id));

        GameObject.Destroy(ArrayDeLinks[id].gameObject);
        ArrayDeLinks[id] = null;


       
        LinkSelecionadoAtualmente = null;
        
    }
    public void RemoverLink(int id)
    {
        //melhorar esse script
        LinkSelecionadoAtualmente = ArrayDeLinks[id];
        PortaUI portaEntrada = LinkSelecionadoAtualmente.entrada;
        PortaUI portaSaida = LinkSelecionadoAtualmente.saida;

        portaEntrada.listaDeLinks.Remove(portaEntrada.listaDeLinks.Find(x => x.id == id));
        portaSaida.listaDeLinks.Remove(portaSaida.listaDeLinks.Find(x => x.id == id));
    
        Debug.Log("removendo link:" + ArrayDeLinks[id].gameObject.name);
        GameObject.Destroy(ArrayDeLinks[id].gameObject);
        ArrayDeLinks[id]= null;
        
        Debug.Log("link removido:" + id);
        LinkSelecionadoAtualmente = null;
     //   portaEntrada.GetComponent<Toggle>().isOn = false;
      //  portaSaida.GetComponent<Toggle>().isOn = false;

    }
    public void AdicionarPortasNoNodoSelecionado()
    {
        nodoSelecionadoAtualmente.GetComponent<NodoUI>().AdicionarPorta();
    }
    public void RemoverPortasNoNodoSelecionado()
    {
        nodoSelecionadoAtualmente.GetComponent<NodoUI>().RemoverTodasAsPortas();
    }
    int CriarIdOriginalNodo()
    {
        for (int i = 0; i < 100; i++)
        {
            //if (DicionarioDeNodos.ContainsKey(i) == false)
            if(ArrayDeNodos[i]==null)
                return i;
        }
        return -1;
    }
    int CriarIdOriginalLink()
    {
        for (int i = 0; i < 100; i++)
        {
            if (ArrayDeLinks[i] == null)
                return i;
        }
        return -1;
    }
    
    public void ClickOnJanela() 
    {      
        if(modoMouse==ModoMouse.Connecting)
        {
            if (Input.GetMouseButton(1))
            {
               if (PortaSelecionadaAtualmente == null)
                    Debug.Log("estava no modo connecting com porta !! null");
               modoMouse = ModoMouse.Idle;               
               CancelarPortaSelecionada();
               if (PortaSelecionadaAtualmente != null)
                    Debug.Log("!! era pra dar null null");
            }
            return;
        }
        if(Input.GetMouseButton(0))
        {
            modoMouse = ModoMouse.Idle;
            PrefabNodoMenu.SetActive(false);
            PrefabJanelaMenu.SetActive(false);
            PrefabLinkMenu.SetActive(false);
            Propriedades.SetActive(false);
            CancelarNodoSelecionado();
        }

        if (Input.GetMouseButton(1))
        {
            /*
            if (MenuLink()==true)
            {
                Debug.Log("não liga menu janela pq menu link já tá ligado");
                return;
            }
            */
            PrefabNodoMenu.SetActive(false);
            PrefabJanelaMenu.SetActive(true);
            PrefabLinkMenu.SetActive(false);
            Vector3 mouse = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
            Vector3 canvasV = new Vector3(canvas.rect.width / 2, canvas.rect.height / 2, 0);
            Vector3 nodoV = new Vector3(-PrefabJanelaMenu.GetComponent<RectTransform>().rect.width * 1.2f, -PrefabJanelaMenu.GetComponent<RectTransform>().rect.height * 1.7f, 0);
            PrefabJanelaMenu.GetComponent<RectTransform>().anchoredPosition3D = mouse - canvasV; //- nodoV;
            CancelarNodoSelecionado();
           
        }
        
    }
    
    public void CancelarNodoSelecionado()
    {
        if (nodoSelecionadoAtualmente != null)
        {
            nodoSelecionadoAtualmente.GetComponent<NodoUI>().DesSelecionar();
            nodoSelecionadoAtualmente = null;
        }
        if(PortaSelecionadaAtualmente!=null)
        {
            PortaSelecionadaAtualmente.GetComponent<PortaUI>().DesSelecionar();
        }
    }
    public void CancelarPortaSelecionada()
    {
        if (PortaSelecionadaAtualmente != null)
        {
            PortaSelecionadaAtualmente.GetComponent<PortaUI>().DesSelecionar();
            PortaSelecionadaAtualmente = null;
        }
        else
            Debug.Log("já estava nulll");
      
        if (PortaSelecionadaAtualmente != null)
            Debug.Log("ridiculo!!!");
    }
    public void Zoom()
    {
        CameraGameObjct.transform.position = CameraGameObjct.transform.position + Vector3.forward*70*(Input.mouseScrollDelta.y);

        /*
        Debug.Log("Zoom janela");
        if(nodoSelecionadoAtualmente !=null)
        {
            nodoSelecionadoAtualmente.GetComponent<NodoUI>().AtualizarLinks();
            nodoSelecionadoAtualmente.GetComponent<RectTransform>().sizeDelta += Vector2.one * 3 * (Input.mouseScrollDelta.y);
            return;
        }
        GetComponent<RectTransform>().sizeDelta += Vector2.one * 6 * (Input.mouseScrollDelta.y);
        for(int i=0; i<  transform.childCount;i++)
        {
            if (transform.GetChild(i).tag == "Nodo")
            {
                transform.GetChild(i).GetComponent<NodoUI>().AtualizarLinks();
                transform.GetChild(i).GetComponent<RectTransform>().sizeDelta += Vector2.one * 3 * (Input.mouseScrollDelta.y);
            }
        }
         */
    }
    void FocusOnNodoSelecionado()
    {
       if(nodoSelecionadoAtualmente==null)
       {
           //nodoSelecionadoAtualmente = DicionarioDeNodos[IdNodoMaisAEsquerda()];
           nodoSelecionadoAtualmente = ArrayDeNodos[IdNodoMaisAEsquerda()];
           if (nodoSelecionadoAtualmente == null)
               return;
       }
       CameraGameObjct.transform.position = new Vector3(nodoSelecionadoAtualmente.transform.position.x, nodoSelecionadoAtualmente.transform.position.y, CameraGameObjct.transform.position.z);
    }
    public void MoverNodoParaOCentro()
    {
        if (nodoSelecionadoAtualmente != null)
            nodoSelecionadoAtualmente.transform.position = new Vector3(CameraGameObjct.transform.position.x, CameraGameObjct.transform.position.y);
        else
            Debug.Log("centralizar nodo: nodo null");

    }
    int IdNodoMaisAEsquerda()
    {
        //retorna o id do nodo mais a esquerda
       // float maisEsquerda = DicionarioDeNodos[0].transform.position.x;
        float maisEsquerda = ArrayDeNodos[0].transform.position.x;
   //     int id = DicionarioDeNodos[0].GetComponent<NodoUI>().id;
        int id = ArrayDeNodos[0].GetComponent<NodoUI>().id;
     //   foreach (var nodo in DicionarioDeNodos)
        foreach(var nodo in ArrayDeNodos)
        {
        //    if (nodo.Value.transform.position.x < maisEsquerda)
            if(nodo!=null)
            if (nodo.transform.position.x < maisEsquerda)
            {
                id = nodo.GetComponent<NodoUI>().id;
                maisEsquerda = nodo.transform.position.x;
            }
        }       
        return id;
    }
    
    public void OrganizarNodosEmArvore()
    {
        RectTransform primeiroNodo = ArrayDeNodos[IdNodoMaisAEsquerda()].GetComponent<RectTransform>();

        TraverseNodeGeral(primeiroNodo, null,0);
        foreach(var nodo in ArrayDeNodos)
        {
            if(nodo!=null)
            nodo.GetComponent<NodoUI>().AtualizarLinks();
        }
    }
    void TraverseNodeGeral(RectTransform nodo, RectTransform pai, int j)
    {
        ProcessarNodo(nodo, pai,j);
        foreach (var porta in nodo.GetComponent<NodoUI>().listaDePortas)
        {
            if (porta.tipoDePorta == TipoDePorta.Saida)

                for (int i = 0; i < porta.listaDeLinks.Count; i++ )
                {
                    //Debug.Log(link.entrada.transform.parent.name);                   
                    TraverseNodeGeral(porta.listaDeLinks[i].entrada.transform.parent.GetComponent<RectTransform>(), nodo,i);
                }
        }
    }
    void ProcessarNodo(RectTransform nodo, RectTransform pai, int i)
    {
        if (pai == null)
            return;

        float x = pai.rect.width + nodo.rect.width;
        float y = i * (nodo.rect.height+50);

        nodo.transform.position = pai.transform.position + new Vector3(x, y, 0);
        // Debug.Log("processando nodo=" + nodo.name + "X="+x+"Y="+y+" com pai=" + pai.name);
    }   
  
    public void ExportarXML()
    {
       // if (DicionarioDeNodos.Count < 1)
        {
            Debug.Log("sem nós pra exportar!");
            
        }
        XmlDocument xml = new XmlDocument();
        XmlElement rootElement = xml.CreateElement("Raiz");
        xml.AppendChild(rootElement);


        //Define a Raiz
        NodoUI nodoRaiz = ArrayDeNodos[IdNodoMaisAEsquerda()].GetComponent<NodoUI>();

        //lê o resto dos nodos
        TraverseNodeExportar( nodoRaiz,  rootElement, xml);
        string path = "";
#if UNITY_EDITOR
        // /*
        path = EditorUtility.SaveFilePanel(
                    "Salve uma Warvore XML :)",
                    Application.dataPath + "/Editor de nodos runtime",
                    "Warvore" ,
                    "xml");
        if (path == "" || path == null)
        {
            Debug.LogError("erro no pathFiile");
            return;
        }    
#endif
        //*/
#if     UNITY_STANDALONE
     //   path = GameObject.Find("Canvas2/Exportar/Text").GetComponent<Text>().text;
#endif
        xml.Save(path);
    }
    void TraverseNodeExportar( NodoUI nodo,  XmlElement nodoXMLPai,  XmlDocument xml)
    {
        XmlElement novoNodoXML = xml.CreateElement("Nodo");
        novoNodoXML.SetAttribute("Nome", nodo.nome);

        //serializar novas propriedades?
        XmlElement propriedades = xml.CreateElement("Propriedades");
        for (int i = 0; i < nodo.listaDePropriedades.Count; i++)
        {
            XmlElement propriedade = xml.CreateElement("propriedade");
            propriedade.SetAttribute("Nome", nodo.listaDePropriedades[i].nome);
            propriedade.SetAttribute( "Valor",nodo.listaDePropriedades[i].Valor);        
            propriedades.AppendChild(propriedade);
        }

        novoNodoXML.AppendChild(propriedades);
        nodoXMLPai.AppendChild(novoNodoXML);
             
        foreach(var porta in nodo.listaDePortas)
        {
            if (porta.tipoDePorta == TipoDePorta.Saida)
            {
                XmlElement NovaportaXml = null ;
                if(porta.listaDeLinks.Count>0)
                {
                     NovaportaXml = xml.CreateElement("PortaSaida");
                    novoNodoXML.AppendChild(NovaportaXml);
                }             
                foreach (var link in porta.listaDeLinks)
                {
                    //Debug.Log(link.entrada.transform.parent.name);                   
                    TraverseNodeExportar(link.entrada.transform.parent.GetComponent<NodoUI>(), NovaportaXml, xml);
                }
            }
        }
    }
    public void ImportarXML()
    {
        // /*
        string path = "";
        XmlDocument XmlDoc = new XmlDocument();
#if UNITY_EDITOR
        
         path = EditorUtility.OpenFilePanel(
                    "Abra uma Warvore XML :)", Application.dataPath +
                    "/Editor de nodos runtime",
                    "xml");
#endif
#if     UNITY_STANDALONE
        
       //  path = GameObject.Find("Canvas2/Importar/Text").GetComponent<Text>().text;
#endif
        XmlDoc.Load(path);
        var rootElement = XmlDoc.DocumentElement;
        //considerando que a arvore começa com um filho       
            
        //variaveis auxiliares para a barra de progresso

        foreach (XmlNode nodo in rootElement.ChildNodes)
            TraverseNodeImportar(null, nodo);

        
    }

    void TraverseNodeImportar( NodoUI nodo, XmlNode XmlNode)
    {
      //essa função tem um erro, por algum motivo um nodo adiciona dois links ao importar arvores maiores que 4
        //tentando criar uma estrutura pra arvore :S
        NodoUI novoNodo = AdicionarNodo();
        novoNodo.AdicionarPorta();
        if (nodo != null)
        {
           
            AdicionarLink(nodo.PortaSaida(), novoNodo.PortaEntrada());
        }
        else
            Debug.Log("nodo null");
        novoNodo.name = "Nodo" + novoNodo.id.ToString();
        novoNodo.nome = XmlNode.Attributes["Nome"].Value;
     //   Debug.Log("adicionando nodo" + novoNodo.nome);
        novoNodo.IniciarNome();

        for (int i = 0; i < XmlNode.ChildNodes.Count; i++)
        {
            XmlNode ChildXml=XmlNode.ChildNodes[i];

            if (ChildXml.Name=="Propriedades")
            {
                foreach( XmlNode prop in ChildXml.ChildNodes)
                {
                    var propUI = novoNodo.AdicionarPropriedade();
                    propUI.gameObject.SetActive(true);
                    propUI.nome = prop.Attributes["Nome"].Value;
                    propUI.Valor = prop.Attributes["Valor"].Value;
                    propUI.SetarNome();
                    propUI.SetarValor();
                    propUI.gameObject.SetActive(false);
                   // Debug.Log("adicionando " + propUI.nome);
                }
            }
            if (ChildXml.Name == "PortaSaida")
            {
               // Debug.Log("chegou no nodo de saida");
                if(ChildXml.ChildNodes.Count>0)
                {
                  //  Debug.Log("adicionando porta numero de filhos:" + ChildXml.ChildNodes.Count);
                   // novoNodo.AdicionarPorta();
                }
                foreach (XmlNode xmlNodeChild in ChildXml.ChildNodes)
                {              
 
                    TraverseNodeImportar(novoNodo, xmlNodeChild);
                }
            }
      
        }

    }
    
}




