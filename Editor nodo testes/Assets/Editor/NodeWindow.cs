using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.Xml;

using System;
public class NodeWindow : EditorWindow {

    public  int IdXml = 0; //auxiliar para salvar arvores diferentes
    List<WNode> listaDeWNodes = new List<WNode>();
    List<WLink> listaDeLinks = new List<WLink>();
    //sobre mecanica se seleção:
    MouseMode modoAtualDoMouse = MouseMode.IDLE;
    WNode nodoSelecionadoAtualmente = null;
    Porta _portaSelecionadaAtualmente = null;

    [MenuItem("Window/WNodeWindow")]
    public static void MostraJanela()
    {
        NodeWindow editor=EditorWindow.GetWindow<NodeWindow>();
        editor.Inicializa();
    }
    public void Inicializa()
    {
        //criar os nodes
        CriarNode();
        
    }
    List<int> IDs=new List<int>();
    void LibertaID(int id)
    {
        for(int i=0;i< IDs.Count;i++)
        {
            if (IDs[i] == id)
            {
                IDs.RemoveAt(i);
                return;
            }
        }
    }
    int livreID()
    {
        int id=menorID(0);
       
        while(IDs.Contains(id)==true)
        {
            id++;
            if (id > 999)
            {
                Debug.LogError("erro no algoritimo : id indefinido");
                break;
            }
        }
        IDs.Add(id);
       // Debug.Log("criado id="+id);
        return id;
    }
    int menorID(int aux) //retorna o menor id depois de aux
    {
        int menor = 999;
        for (int i = 0; i < IDs.Count;i++ )
        {
            if(IDs[i]<menor && IDs[i]>aux)
            {
                menor = IDs[i];
            }
        }
        if (IDs.Count < 1)
            return 1;
        return menor;
    }

    void CriarNode()
    {
        WNode novoNode = new WNode("Node", livreID());
        
       
        if (listaDeWNodes.Count > 0)
        {
            novoNode._posicao.x = Event.current.mousePosition.x;
            novoNode._posicao.y = Event.current.mousePosition.y;
            novoNode._tamanho.x = listaDeWNodes[0]._tamanho.x;
            novoNode._tamanho.y = listaDeWNodes[0]._tamanho.y;
        }
        listaDeWNodes.Add(novoNode);
    }
    void CriarNode(float x, float y)
    {
        WNode novoNode = new WNode("Node", livreID());


        if (listaDeWNodes.Count > 0)
        {
            novoNode._posicao.x = x;
            novoNode._posicao.y = y;
            novoNode._tamanho.x = listaDeWNodes[0]._tamanho.x;
            novoNode._tamanho.y = listaDeWNodes[0]._tamanho.y;
        }
        listaDeWNodes.Add(novoNode);
    }

    WNode CriarNode(float x, float y, float largura,float altura,ref List<Propriedade> lista)
    {

        WNode novoNode = new WNode("nodo", livreID(),x,y,largura,altura);
        novoNode.listaDePropriedades = lista;

        listaDeWNodes.Add(novoNode);
        return novoNode;
    }

    void OnGUI()
    {
        if(nodoSelecionadoAtualmente!=null)
        //Debug.Log("nodo="+ nodoSelecionadoAtualmente.Id);
        Debug.Log("Modo="+modoAtualDoMouse);

        DesenhaLinks();
        BeginWindows();
            DesenhaNodes();          
        EndWindows();   
        DesenhaCurvaDoMouse();

        // processa input..
        AlterarTamanhoNode();
        Arraste();
        Zoom();
        FecharNode();
        MenuNode();
        ComecaUmaConexao();
        TerminaUmaConexao();
        MenuPrincipal();
      
    }   
    void DesenhaLinks()
    {

        for (int i = 0; i < listaDeLinks.Count; i++)
        {
            DesenharCurva(listaDeLinks[i].portas[0], listaDeLinks[i].portas[1]);
        }
    }
    void DesenhaNodes()
    {
        //desenha os nós e o que tem dentro dele
        for (int i = 0; i < listaDeWNodes.Count; i++)
        {
            //muda o tamanho    revisar isso: bug: muda duas janelas ao msm tempo
            //       HorizResizer(ref listaDeWNodes[i].Rect);
            //   HorizResizer(ref listaDeWNodes[i].Rect,false);              
            listaDeWNodes[i].Desenhar();

            //aproveita o loop para atualizar a variavel nodoSelecionadoAtualmente
            SelecionarNode(i);

           // AlterarTamanhoNode(ref listaDeWNodes[i]._tamanho,ref listaDeWNodes[i]._posicao);
        }
    }
    void  SelecionarNode(int i)
    {
        if( listaDeWNodes[i].MouseDown(Event.current.mousePosition)==true)
        {
            //aqui seria pro mouse perder o ultimo foco
           
            nodoSelecionadoAtualmente = listaDeWNodes[i];
           
        }

    }
    void DesenhaCurvaDoMouse()
    {
        //desenha uma curvaa no mouse se estiver selecionando
        if (modoAtualDoMouse == MouseMode.CONNECTING && _portaSelecionadaAtualmente != null)
        {
            Vector3 inicio = new Vector3(_portaSelecionadaAtualmente.Posicao.x + 10, _portaSelecionadaAtualmente.Posicao.y + 10, 0);
            Vector3 fim = new Vector3(Event.current.mousePosition.x, Event.current.mousePosition.y, 0);

            DesenharCurva(inicio, fim, _portaSelecionadaAtualmente.cor, _portaSelecionadaAtualmente.tipoDaPorta);
            Repaint();
        }
        else
        {
          //  Debug.Log("nenhuma porta selecionado");
        }
    }
    void MenuPrincipal()
    {
        EditorGUILayout.HelpBox("Crie nós e ligações para poder exportar em xml. \n Comandos: zoom com mouse,\n arraste com mouse,\n ctrl+w novo nó, \n botão fechar \n menu na parte de cima do nó para poder editá-lo. \n s=escala", MessageType.Info);
        //menu básico      
        if (Event.current.Equals(Event.KeyboardEvent("^w")))
        {
            CriarNode();
            Repaint();
        }
        if(GUI.Button(new Rect(Screen.width - 130,10 , 120, 50), "Criar Node/ctrl+w"))
        {
            CriarNode(Screen.width/2,Screen.height/2);
            Repaint();
        }
        if (GUI.Button(new Rect(Screen.width - 130, 80, 100, 50), "exportar XML"))
        {
            ExportarXML();
        }
        if (GUI.Button(new Rect(Screen.width - 130, 160, 100, 50), "importar XML"))
        {
            if (EditorUtility.DisplayDialog("Isso vai apagar os nós existentes",
            "Tem certeza disso cowboy?", "sim", "não"))
            {
                DeletarTodosNodes();
                ImportarXML();
            }
        }
        if (GUI.Button(new Rect(Screen.width - 130, 220, 100, 50), "Deletar todos nodos"))
        {
            if (EditorUtility.DisplayDialog("Isso vai apagar os nós existentes",
            "Tem certeza disso cowboy?", "sim", "não"))
            {
                DeletarTodosNodes();
               
            }
        }
      
       
    }
    void Arraste()
    {
        if(modoAtualDoMouse==MouseMode.IDLE && (Event.current.type==EventType.mouseDown ||Event.current.type==EventType.mouseDrag))
        {
            if(Event.current.button==2)
            {
                
                if (Event.current.delta.magnitude < 150)
                {
                    for (int i = 0; i < listaDeWNodes.Count; i++)
                    {
                    //Debug.Log("bolinha do mouse");
              
                   
                       // Debug.Log(Event.current.delta);
                        listaDeWNodes[i]._posicao.x += Event.current.delta.x/2;
                        listaDeWNodes[i]._posicao.y += Event.current.delta.y / 2;
                        Repaint();
                    }
                }
            }

        }
    }
    
    void AlterarTamanhoNode()
    {
        
        if (Event.current.Equals(Event.KeyboardEvent("s")) && modoAtualDoMouse == MouseMode.IDLE)
        {            
            if(nodoSelecionadoAtualmente!=null)
            {       
               modoAtualDoMouse=MouseMode.RESIZING;
            }
            else
            {
                Debug.Log("selecione um nó!");
            }
        }
        if(modoAtualDoMouse==MouseMode.RESIZING)
        {
            EditorGUIUtility.AddCursorRect(new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 10, 10), MouseCursor.ResizeUpLeft);
            GUI.Box(new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 10, 10),"");
            Repaint();
            if ( Event.current.type==EventType.mouseDrag)
            {
                if(Event.current.button==0)
                {                
                    if (Event.current.delta.magnitude < 300)
                    {
                        nodoSelecionadoAtualmente._tamanho.x += Event.current.delta.x ;
                        nodoSelecionadoAtualmente._tamanho.y += Event.current.delta.y ;
                        Repaint();
                    }
                } 
            }
            if (Event.current.type==EventType.scrollWheel)
            {
                if (Event.current.delta.magnitude < 100)
                {
                    nodoSelecionadoAtualmente._tamanho.x -= Event.current.delta.y*2 ;
                    nodoSelecionadoAtualmente._tamanho.y -= Event.current.delta.y*2 ;
                    Repaint();
                }
            }
            //comfirma transformação
            if (Event.current.type == EventType.mouseUp)
            {             
                    modoAtualDoMouse = MouseMode.IDLE;

            }
        }
     }

    //TODO: melhorar essa função zoom
    void Zoom()
    {       
        if(modoAtualDoMouse==MouseMode.IDLE && Event.current.type==EventType.scrollWheel)
        {            
           // Debug.Log(Event.current.delta);
            for(int i=0; i< listaDeWNodes.Count;i++)
            {
                   //diminuindo
                if (Event.current.delta.y> 0 )
                {
                    if(listaDeWNodes[i]._tamanho.x>30 || listaDeWNodes[i]._tamanho.y>30)
                    {     
                       listaDeWNodes[i]._posicao.x -= Event.current.delta.y * 2 ;                                  
                       listaDeWNodes[i]._posicao.y -= Event.current.delta.y*2 ;

                        listaDeWNodes[i]._tamanho.x -= Event.current.delta.y * 2;
                        listaDeWNodes[i]._tamanho.y -= Event.current.delta.y * 2;
                        Repaint();
                    }            
                }  
                else
                //aumentando
                if (Event.current.delta.y < 0)
                {
                    if (listaDeWNodes[i]._tamanho.x < 300 || listaDeWNodes[i]._tamanho.y > 300)
                    {
                        listaDeWNodes[i]._posicao.x -= Event.current.delta.y*2;
                        listaDeWNodes[i]._posicao.y -= Event.current.delta.y*2;
                        listaDeWNodes[i]._tamanho.x -= Event.current.delta.y * 2;
                        listaDeWNodes[i]._tamanho.y -= Event.current.delta.y * 2;
                        Repaint();
                    }
                }
                   
              
              }
        }

    }
    void  FecharNode()
  {
      if (Event.current.type == EventType.mouseDown && modoAtualDoMouse == MouseMode.IDLE)
      {
         
              for (int i = 0; i < listaDeWNodes.Count; i++)
              {
                  if (listaDeWNodes[i].MouseOnX(Event.current.mousePosition, Event.current.button))
                  {
                      DeleteNode(i);
                  }
              }        
      }
  }
    void MenuNode()
    {
        if (Event.current.type == EventType.mouseDown )
        {
            if (Event.current.button == 0)
            {               
                for (int i = 0; i < listaDeWNodes.Count; i++)
                {
                    if (listaDeWNodes[i].MouseOnMenu(Event.current.mousePosition, Event.current.button))
                    {                     
                            Debug.Log("entra no menu");
                            CriarNodeMenu(i);
                                                            
                    }
                }
            }
        }
       
    }
    void CriarNodeMenu(int nodo)
    {       
       GenericMenu menu = new GenericMenu();
       int indice= listaDeWNodes[nodo].listaDePortas.Count-1;
       int link1 = JaExisteEsseLink(listaDeWNodes[nodo].listaDePortas[indice]);
       int link2 = JaExisteEsseLink(listaDeWNodes[nodo].listaDePortas[indice - 1]);
       menu.AddItem(new GUIContent("AdicionarPropriedade string"), false, () => { listaDeWNodes[nodo].AdicionarPropriedade("nomestring","string aqui"); });
       menu.AddItem(new GUIContent("Adicionar Par de Portas"), false, () => { listaDeWNodes[nodo].AdicionarPortas(); });
       menu.AddItem(new GUIContent("Remover Par de Portas"), false, () => { listaDeWNodes[nodo].RemoverUltimoParDePortas(); RemoverLink(link1); RemoverLink(link2); });
       menu.AddItem(new GUIContent("Remover Propriedde"), false, () => { listaDeWNodes[nodo].RemoverPropriedade(); });
       menu.ShowAsContext();
    }
    void RemoverLink(int indice)
    {
        if (indice > -1 && indice < listaDeLinks.Count)
        {
            listaDeLinks.RemoveAt(indice);
        }

    }
    
    /*void ExportarXML()
    {
        if (listaDeWNodes.Count < 1)
            return;
      
        XmlDocument xml = new XmlDocument();
        XmlElement rootElement = xml.CreateElement("Raiz");
        xml.AppendChild(rootElement);

        
        // cria lista de nodos
        XmlElement listaDeNodos = xml.CreateElement("listaDeNodos");
        rootElement.AppendChild(listaDeNodos);
       //pra cada nodo
        for(int i=0; i< listaDeWNodes.Count;i++)
        {
           //cria o nó de xml
            XmlElement nodo = xml.CreateElement(listaDeWNodes[i].Nome);
            nodo.SetAttribute("ID", listaDeWNodes[i].Id.ToString());
            nodo.SetAttribute("NumeroDePortas", listaDeWNodes[i].listaDePortas.Count.ToString());
            //adiciona as coordenadas
            nodo.SetAttribute("x",listaDeWNodes[i]._posicao.x.ToString());
            nodo.SetAttribute("y", listaDeWNodes[i]._posicao.y.ToString());
            nodo.SetAttribute("largura", listaDeWNodes[i]._tamanho.x.ToString());
            nodo.SetAttribute("posicao", listaDeWNodes[i]._tamanho.y.ToString());
            //pra cada propriedade adiciona um atributo ao nodo
            for (int j = 0; j < listaDeWNodes[i].listaDePropriedades.Count;j++ )
            {               
                nodo.SetAttribute(listaDeWNodes[i].listaDePropriedades[j].Nome, listaDeWNodes[i].listaDePropriedades[j].texto);
            }
            listaDeNodos.AppendChild(nodo);
        }
        //agora a lista de links
        XmlElement listaDeConexoes = xml.CreateElement("listaDeConexões");
        rootElement.AppendChild(listaDeConexoes);
        //pra cada link
        for (int i = 0; i < listaDeLinks.Count; i++)
        {        
            XmlElement link = xml.CreateElement(listaDeLinks[i].Name);
            //pra cada porta
            for (int j = 0; j < listaDeLinks[i].portas.Length; j++)
            {
                XmlElement porta = xml.CreateElement(listaDeLinks[i].portas[j].Nome);
                //offset ou cor ou tipo da ligação
                porta.SetAttribute("Offset", listaDeLinks[i].portas[j].yOffset.ToString());
                //id do dono
                porta.SetAttribute("IDNodoDono", listaDeLinks[i].portas[j].nodoDono.Id.ToString());
                //entrada ou saida// esquerda ou direita
                if (listaDeLinks[i].portas[j].tipoDaPorta == TipoDePorta.ENTRADA)
                {
                    porta.SetAttribute("tipoDaPorta", "1");
                }
                else
                {
                    porta.SetAttribute("tipoDaPorta", "2");
                }
                link.AppendChild(porta);
            }
            listaDeConexoes.AppendChild(link);
        }
        var path = EditorUtility.SaveFilePanel(
                    "Salve uma Warvore XML :)",
                    Application.dataPath+"/Resources/XML/",
                    "Warvore"+IdXml.ToString(),
                    "xml");
        if(path=="" || path==null)
        {
            return;
        }
        xml.Save(path);

    }
    */

    void ExportarXML()
    {
        if (listaDeWNodes.Count < 1)
        {
            Debug.Log("sem nós pra exportar!");
            return;
        }
        XmlDocument xml = new XmlDocument();
        XmlElement rootElement = xml.CreateElement("Raiz");
        xml.AppendChild(rootElement);

        //Define a Raiz
        WNode nodoRaiz = ProcurarNodoPeloID( IdNodoMaisAEsquerda() );
       
        //lê o resto dos nodos
        TraverseNode(ref nodoRaiz,ref rootElement,ref xml);

       
        var path = EditorUtility.SaveFilePanel(
                    "Salve uma Warvore XML :)",
                    Application.dataPath + "/Resources/XML/",
                    "Warvore" + IdXml.ToString(),
                    "xml");
        if (path == "" || path == null)
        {
            Debug.LogError("erro no pathFiile");
            return;
        }
        xml.Save(path);
    }

    int IdNodoMaisAEsquerda()
    {
        //retorna o id do nodo mais a esquerda
        float maisEsquerda = listaDeWNodes[0]._posicao.x;
        int id=listaDeWNodes[0].Id;
        foreach(var nodo in listaDeWNodes)
        {
            if(nodo._posicao.x< maisEsquerda)
            {
                maisEsquerda = nodo._posicao.x;
                id=nodo.Id;
            }
        }
        return id;
    }
    //usado na função exportarxml2
    void TraverseNode(ref WNode nodo, ref XmlElement nodoXML,ref  XmlDocument xml)
    {
        //processa nodo
        XmlElement novoNodoXML = xml.CreateElement(nodo.Nome);
        //pra cada propriedade adiciona um atributo ao nodo
    //    novoNodoXML.SetAttribute("porta",nodo);
        Debug.Log(" numero de propriedades desse nó =" + nodo.listaDePropriedades.Count);
        for (int j = 0; j < nodo.listaDePropriedades.Count; j++)
        {
          
            novoNodoXML.SetAttribute( nodo.listaDePropriedades[j].Nome , nodo.listaDePropriedades[j].texto);
        }
        nodoXML.AppendChild(novoNodoXML);
       // Debug.Log("nodo processado!");
        //pra cada porta...
        for(int i=0; i<nodo.listaDePortas.Count ;i++)
        {
          if(nodo.listaDePortas[i].tipoDaPorta==TipoDePorta.SAIDA)
              //pega o nodo pai no final desse link
              //e propaga
              if(nodo.listaDePortas[i].Link!=null)
             TraverseNode(ref nodo.listaDePortas[i].Link.portaEntrada().nodoDono, ref  novoNodoXML, ref  xml);
     
        }
       
    }

    void ImportarXML()
    {
        XmlDocument XmlDoc = new XmlDocument();
        var path = EditorUtility.OpenFilePanel(
                    "Abra uma Warvore XML :)", Application.dataPath +
                    "/Resources/XML/",
                    "xml");
        XmlDoc.Load(path);
        var rootElement = XmlDoc.DocumentElement;
        //considerando que a arvore começa com um filho
        List<Propriedade> listaP = new List<Propriedade>();
        //lista de propriedade do node
        for (int i = 0; i < rootElement.FirstChild.Attributes.Count; i++)
        {
            listaP.Add(new Propriedade(rootElement.FirstChild.Attributes[i].Name, 0, rootElement.FirstChild.Attributes[i].Value));
        }
        WNode novoNodo=CriarNode(0,Screen.height/2, 100, 100, ref listaP);
        //variaveis auxiliares para a barra de progresso
        int k = 0;
        float progresso = 0;
        float começo = (float)EditorApplication.timeSinceStartup;
        //variaveis auxiliares para indentação
        int numFilhos = rootElement.FirstChild.ChildNodes.Count;
        int indentacao = -numFilhos / 2;

        foreach (XmlNode nodo in rootElement.FirstChild.ChildNodes)
        {
            TraverseNode2(ref novoNodo, nodo, indentacao);
            indentacao += numFilhos/2;
            EditorUtility.DisplayProgressBar(
                 "Importando  nós",
                 "carregando  " + progresso.ToString("N") + "segs",
                  (float)k / (float)rootElement.FirstChild.ChildNodes.Count);

            progresso = (float)EditorApplication.timeSinceStartup - começo;
            k++;
        }
        EditorUtility.ClearProgressBar();
    }
    //usado na função importarxml2
    void TraverseNode2(ref WNode nodo, XmlNode XmlNode, int indentacao)
    {

        //processa nodo
        List<Propriedade> listaP = new List<Propriedade>();
        //lista de propriedade do node
        for (int i = 0; i < XmlNode.Attributes.Count; i++)
        {
            listaP.Add(new Propriedade(XmlNode.Attributes[i].Name, 0, XmlNode.Attributes[i].Value));
        }
        //tentando criar uma estrutura pra arvore :S
        WNode novoNodo = CriarNode(nodo._posicao.x + nodo._tamanho.x + 100, nodo._posicao.y + nodo._tamanho.y/2 + indentacao * (nodo._tamanho.y+20), 100, 100, ref listaP);
        Conectar(nodo, novoNodo);
        int numFilhos=XmlNode.ChildNodes.Count;
        indentacao ++;
        foreach (XmlNode nodoxml in XmlNode.ChildNodes)
        {         
            TraverseNode2(ref novoNodo, nodoxml,indentacao);
            indentacao += numFilhos/2;
        }


      
    }

    /*void ImportarXML()
    {
       
        XmlDocument XmlDoc = new XmlDocument();
        var path = EditorUtility.OpenFilePanel(
                    "Abra uma Warvore XML :)", Application.dataPath+
                    "/Resources/XML/",
                    "xml");
         
        XmlDoc.Load(path);
        var rootElement = XmlDoc.DocumentElement;
       
        var listaDeNodos = rootElement.FirstChild;
        var listaDeConexoes = rootElement.LastChild;
        //carrega os dados
        int k = 0;
        float progresso = 0;
        float começo = (float)EditorApplication.timeSinceStartup;
        foreach (XmlNode nodo in listaDeNodos)
        {
            EditorUtility.DisplayProgressBar(
                    "Importando  nós",
                    "carregando ids e propriedade "+progresso.ToString("N")+"segs",
                     (float)k/(float)listaDeNodos.ChildNodes.Count);
            progresso = (float)EditorApplication.timeSinceStartup - começo;
            k++;
            int id = Convert.ToInt16(nodo.Attributes[0].Value);
            List<Propriedade> listaP = new List<Propriedade>();

            int numeroDePortas=Convert.ToInt16(nodo.Attributes[1].Value);
            //parte geométrica
            float x=0;
                if(Single.TryParse(nodo.Attributes[2].Value,out x)==false)
                    Debug.LogError("erro ao converter string to float");
            float y=10;
                if(Single.TryParse(nodo.Attributes[3].Value,out y)==false)
                        Debug.LogError("erro ao converter string to float");
            float largura=100;
                if(Single.TryParse(nodo.Attributes[4].Value,out largura)==false)
                    Debug.LogError("erro ao converter string to float");
            float altura=100;     
                if(Single.TryParse(nodo.Attributes[5].Value,out altura)==false)
                    Debug.LogError("erro ao converter string to float");

            //lista de propriedade do node
            for (int i=6; i< nodo.Attributes.Count;i++)
            {
                listaP.Add(new Propriedade( nodo.Attributes[i].Name, 0, nodo.Attributes[i].Value));

            }
            
          //  CriarNode(nodo.Name,id,x,y,largura,altura,ref listaP);
           
        }


        EditorUtility.ClearProgressBar();
         progresso = 0;
         começo = (float)EditorApplication.timeSinceStartup;
        k = 0;
        foreach (XmlNode conexao in listaDeConexoes)
        {
            EditorUtility.DisplayProgressBar(
                     "Importando  nós",
                     "carregando conexões " + progresso.ToString("N") + "segs",
                      (float)k / (float)listaDeConexoes.ChildNodes.Count);
            progresso = (float)EditorApplication.timeSinceStartup - começo;
            k++;
            //primeira porta
            int offset = Convert.ToInt32(conexao.FirstChild.Attributes["Offset"].Value);
            int id = Convert.ToInt32(conexao.FirstChild.Attributes["IDNodoDono"].Value);
            int tipoDaPorta = Convert.ToInt32(conexao.FirstChild.Attributes["tipoDaPorta"].Value);
            TipoDePorta tipo = TipoDePorta.INDEFINIDO;
            if (tipoDaPorta == 1)
                tipo = TipoDePorta.ENTRADA;
            else
                tipo = TipoDePorta.SAIDA;
            if (tipo == TipoDePorta.INDEFINIDO)
                Debug.LogError("erro na leitura do tipo de porta");

            WNode nodo = ProcurarNodoPeloID(id);
            if (nodo == null)
            {
                Debug.LogError("nodo não encontrado id="+id);
                return;
            }
            Porta porta1 = new Porta("porta", offset, tipo, nodo);
            
            nodo.AdicionarPorta(porta1);
            //segunda porta
             offset = Convert.ToInt32(conexao.LastChild.Attributes["Offset"].Value);
             id = Convert.ToInt32(conexao.LastChild.Attributes["IDNodoDono"].Value);
             tipoDaPorta = Convert.ToInt32(conexao.LastChild.Attributes["tipoDaPorta"].Value);
             tipo = TipoDePorta.INDEFINIDO;
            if (tipoDaPorta == 1)
                tipo = TipoDePorta.ENTRADA;
            else
                tipo = TipoDePorta.SAIDA;
            if (tipo == TipoDePorta.INDEFINIDO)
                Debug.LogError("erro na leitura do tipo de porta");
            nodo = ProcurarNodoPeloID(id);
            Porta porta2 = new Porta("porta", offset, tipo, nodo);
            nodo.AdicionarPorta(porta2);
            WLink novolink = new WLink(porta1,porta2);
            listaDeLinks.Add(novolink);
        }
        EditorUtility.ClearProgressBar();
        
    }
     */   
     WNode ProcurarNodoPeloID(int id)
    {
        
        for (int i = 0; i < listaDeWNodes.Count; i++)
        {
            if(listaDeWNodes[i].Id==id)
            {
                return  listaDeWNodes[i];
            }

        }
        return null;
    }
    void ComecaUmaConexao()
    {
        if (Event.current.type == EventType.mouseDown && modoAtualDoMouse == MouseMode.IDLE )
        {
            if(Event.current.button==0)
            {
                for(int i=0; i< listaDeWNodes.Count;i++)
                {
                    Porta porta=listaDeWNodes[i].MouseDownPorta(Event.current.mousePosition, Event.current.button);

                    if (porta!=null)
                    {
                        //verifica ja existe uma conexão
                        if (JaExisteEsseLink(porta) == -1 || porta.tipoDaPorta == TipoDePorta.SAIDA)
                        {
                            _portaSelecionadaAtualmente = porta;
                            modoAtualDoMouse = MouseMode.CONNECTING;
                        }
                        else //essa parte desconecta um nó já com link
                        {                           
                                int link = JaExisteEsseLink(porta);
                                if (listaDeLinks[link].portas[0].nodoDono.Id == porta.nodoDono.Id)
                                {
                                    _portaSelecionadaAtualmente = listaDeLinks[link].portas[1];
                                    Debug.Log("o que?");
                                    modoAtualDoMouse = MouseMode.CONNECTING;

                                    listaDeLinks.RemoveAt(link);
                                }
                                else
                                {
                                    
                                    _portaSelecionadaAtualmente = listaDeLinks[link].portas[0];
                                    //revisar essa parte do codigo obs: fazer pra que cada porta tenha um link somente!

                                    _portaSelecionadaAtualmente.Link = null;
                                    listaDeLinks[link].portas[1].Link = null;
                                    modoAtualDoMouse = MouseMode.CONNECTING;
                                    listaDeLinks.RemoveAt(link);
                                }
                            

                        }
                    }
                }
             }         
        }
             
    }
    void TerminaUmaConexao()
    {
        if (Event.current.type == EventType.MouseUp && modoAtualDoMouse == MouseMode.CONNECTING )
        {
            if (Event.current.button == 0)
            {
                for (int i = 0; i < listaDeWNodes.Count; i++)
                {
                    if (listaDeWNodes[i].MouseUp(Event.current.mousePosition) == true)
                    {
                        //cria conexão
                        Porta porta = listaDeWNodes[i].MouseUpPorta(Event.current.mousePosition);
                        //se alguma porta for null
                        if (porta == null || _portaSelecionadaAtualmente == null)
                        {
                            modoAtualDoMouse = MouseMode.IDLE;
                            Debug.Log("porta nula");
                        }
                        else
                        if (JaExisteEsseLink(porta,_portaSelecionadaAtualmente) == -1 && porta.cor==_portaSelecionadaAtualmente.cor ) //se forem de cores diferentes
                        {
                            //se as portas forem diferente
                            if (_portaSelecionadaAtualmente.nodoDono.Id != porta.nodoDono.Id)
                            {
                                if (_portaSelecionadaAtualmente.tipoDaPorta == porta.tipoDaPorta)
                                {
                                    modoAtualDoMouse = MouseMode.IDLE;
                                    Debug.Log("ops portas iguais");
                                }
                                else
                                {
                                    if (porta.Posicao.x < _portaSelecionadaAtualmente.Posicao.x || ( porta.Posicao.x > _portaSelecionadaAtualmente.Posicao.x && porta.tipoDaPorta == TipoDePorta.SAIDA) )
                                    {
                                        modoAtualDoMouse = MouseMode.IDLE;
                                        Debug.Log("conectar para trás: considerei esse um comportamento incoreto, tirar esse if se tiver esse objetivo");

                                    }
                                    else
                                    {
                                        // Debug.Log("criando conexão");
                                        Conectar(_portaSelecionadaAtualmente, porta);                        
                                        modoAtualDoMouse = MouseMode.IDLE;
                                    }
                                }
                            }
                            else
                            {
                                _portaSelecionadaAtualmente = null;
                                modoAtualDoMouse = MouseMode.IDLE;
                                Debug.Log("mesmo dono!");
                            }
                        }
                        else
                        {
                            modoAtualDoMouse = MouseMode.IDLE;
                           _portaSelecionadaAtualmente = null;
                            Debug.Log("ja existe esse link!");
                        }
                    }
                }
            }
            if (Event.current.button == 1)
            {
                _portaSelecionadaAtualmente = null;
                modoAtualDoMouse = MouseMode.IDLE;
            }

        }
        if(Event.current.type == EventType.mouseDown && modoAtualDoMouse == MouseMode.CONNECTING)
        {

            if (Event.current.button == 1)
            {
                modoAtualDoMouse = MouseMode.IDLE;
            }
        }
     }
    public void Conectar(WNode nodo1, WNode nodo2)
    {
        Porta portalivre1 = nodo1.PortaSaidaLivre();
        Porta portalivre2 = nodo2.PortaEntradaLivre();
        if(portalivre1==null)
        {
          nodo1.AdicionarPortas();
          portalivre1 = nodo1.PortaSaidaLivre();
        }
        if (portalivre2 == null)
        {
            nodo2.AdicionarPortas();
            portalivre2 = nodo2.PortaSaidaLivre();
        }
        Conectar(portalivre1, portalivre2);
    }
   
    public void Conectar(Porta porta1, Porta porta2)
    {
        // Debug.Log("criando conexão");
        WLink NovoLink = new WLink(porta1, porta2);
        porta1.Link = NovoLink;
        porta2.Link = NovoLink;
        listaDeLinks.Add(NovoLink);
    }
    public void DesenharCurva(Vector3 PosicaoInicial, Vector3 PosicaoFinal, Color cor, TipoDePorta tipo)
    {
        PosicaoInicial.z = 0;
        PosicaoFinal.z = 0;
        Vector3 TangenteInicial;
        Vector3 TangenteFinal;
        if (PosicaoInicial.x < PosicaoFinal.x)
        {
             TangenteInicial = PosicaoInicial + Vector3.right * 50;
             TangenteFinal = PosicaoFinal + Vector3.left * 50;
        }
        else
        {

             TangenteInicial = PosicaoInicial + Vector3.left * 50;
             TangenteFinal = PosicaoFinal + Vector3.right * 50;
        }

        Color corDeSombra = new Color(0, 0, 0, 0.06f);
        for (int i = 0; i < 3; i++) //desenha as sombras
            Handles.DrawBezier(PosicaoInicial, PosicaoFinal, TangenteInicial, TangenteFinal, corDeSombra, null, (i + 1) * 5);
        Handles.DrawBezier(PosicaoInicial, PosicaoFinal, TangenteInicial, TangenteFinal, cor, null, 5);
    }
    public void DesenharCurva(Porta portaInicial,Porta portaFinal)
    {

        Vector3 inicio = new Vector3(portaInicial.Posicao.x + 10, portaInicial.Posicao.y + 10, 0);
        Vector3 fim = new Vector3(portaFinal.Posicao.x + 10, portaFinal.Posicao.y + 10, 0);
        

        Vector3 TangenteInicial;
        Vector3 TangenteFinal;
        //essa parte tenta acertar as direções da curva
        if (portaInicial.Posicao.x < portaFinal.Posicao.x)
        {
            if (portaInicial.tipoDaPorta == TipoDePorta.ENTRADA)
            {
                //posicao inicial entrada mais a esquerda
                TangenteInicial = inicio + Vector3.left * 50;
                TangenteFinal = fim + Vector3.right * 50;
            }
            else
            {
                //posicao inicial saida mais a esquerda
                TangenteInicial = inicio + Vector3.right * 50;
                TangenteFinal = fim + Vector3.left * 50;
            }
        }
        else
        {
            if (portaInicial.tipoDaPorta == TipoDePorta.SAIDA)
            {
                //posicao inicial saida mais a direita
                TangenteInicial = inicio + Vector3.right * 50;
                TangenteFinal = fim + Vector3.left * 50;
            }
            else
            {
                //posicao inicial entrada mais a direita
                TangenteInicial = inicio + Vector3.left * 50;
                TangenteFinal = fim + Vector3.right * 50;
            }
        }

        Color corDeSombra = new Color(0, 0, 0, 0.06f);
        for (int i = 0; i < 3; i++) //desenha as sombras
            Handles.DrawBezier(inicio, fim, TangenteInicial, TangenteFinal, corDeSombra, null, (i + 1) * 5);
        Handles.DrawBezier(inicio, fim, TangenteInicial, TangenteFinal, portaInicial.cor, null, 5);
       
    }
 
    public void DeleteNode(int i)
    {
        //TODO: talvez um pool object pra ecnommizar memoria?
        if(_portaSelecionadaAtualmente!=null)
            if (_portaSelecionadaAtualmente.nodoDono.Id == listaDeWNodes[i].Id)
            {
                _portaSelecionadaAtualmente = null;
            }

        int link = 0;
        while( (link = JaExisteEsseLink(listaDeWNodes[i])) !=-1)
        {
            
            listaDeLinks.RemoveAt(link);
        }
        //remove os links das portas conectadas a essa porta
        foreach(var porta in listaDeWNodes[i].listaDePortas)
        {
               if(porta.tipoDaPorta==TipoDePorta.ENTRADA)
               {
                   if(porta.Link!=null)
                   porta.Link.portaSaida().Link = null;
               }       
               else
               {
                   if (porta.Link != null)
                   porta.Link.portaEntrada().Link = null;
               }
        }

        //limpa portas
        listaDeWNodes[i].RemoverTodasAsPortas();
        //apaga o node
        LibertaID(listaDeWNodes[i].Id);
        listaDeWNodes.RemoveAt(i);
        
    }
    void DeletarTodosNodes()
    {
        for(int i=listaDeWNodes.Count-1;i>=0;i--)
        {
            DeleteNode(i);
        }

    }
    public int JaExisteEsseLink(WNode nodo )
    {
        //acha o link que tem esse nodo e retorna o indice do link
        for (int l = 0; l < listaDeLinks.Count; l++)
        {
            if (listaDeLinks[l].portas[0].nodoDono.Id == nodo.Id)
            {
                return l;
            }
            if (listaDeLinks[l].portas[1].nodoDono.Id == nodo.Id)
            {
                
                return l;
            }
        }
        return -1;
    }
    public int JaExisteEsseLink(Porta porta1, Porta porta2)
    {

        //acha o link que tem essas portas juntas 
        for (int l = 0; l < listaDeLinks.Count; l++)
        {
            
            if (listaDeLinks[l].portas[0] == porta1 && listaDeLinks[l].portas[1]== porta2)
            {

                return l;
            }
            if (listaDeLinks[l].portas[1] == porta1&& listaDeLinks[l].portas[0]== porta2)
            {
                return l;
            }
        }
        return -1;
    }
    public int JaExisteEsseLink(Porta porta)
    {

        //acha o link que tem essa porta 
        for (int l = 0; l < listaDeLinks.Count; l++)
        {

            if (listaDeLinks[l].portas[0] == porta)
            {

                return l;
            }
            if (listaDeLinks[l].portas[1] == porta)
            {
                return l;
            }
        }
        return -1;
    }

   
}
