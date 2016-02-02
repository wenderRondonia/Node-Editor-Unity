using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class NodoUI : MonoBehaviour
{
    public string nome="";
    public int id;
    //edição in-game
    RectTransform canvas;
    RectTransform nodoUIRectTransform;
    RectTransform Panel;
    JanelaUI janelaUI;
    public List<PortaUI> listaDePortas= new List<PortaUI>();
    public List<PropriedadeUIString> listaDePropriedades = new List<PropriedadeUIString>();
    bool selecionado = false;
    int offset = 0;
  
   
    void Start()
    {
        canvas = GameObject.Find("Canvas").GetComponent<RectTransform>();
        nodoUIRectTransform = GetComponent<RectTransform>();
        Panel = GameObject.Find("Canvas/Panel").GetComponent<RectTransform>();
        janelaUI=Panel.GetComponent<JanelaUI>();
        
      //  transform.parent = canvas.transform;
        // Vector3 vetor= new Vector3(canvas.GetComponent<RectTransform>().rect.width, canvas.GetComponent<RectTransform>().rect.height,0);
        // transform.position += canvas.GetComponent<RectTransform>().position +vetor;
      
        //Vector3 vetor = new Vector3(canvas.rect.width/2, canvas.rect.height/2, 0);
        //transform.position = vetor;
        //AdicionarPorta();
    }
  
    void Update()
    {
       

    }
    
    /*
    public void DestruirNodo()
    {
        //janelaUI.PrefabNodoMenu.SetActive(false);
        janelaUI.PrefabJanelaMenu.SetActive(false);
        janelaUI.RemoverNodo(id);
      
    }
    */
    public void SetarNome()
    {
       nome= transform.FindChild("InputFieldNome").GetComponent<InputField>().text;
        Debug.Log("nome setado para " + nome);
    }

    public void IniciarNome()
    {
        transform.FindChild("InputFieldNome").GetComponent<InputField>().text = nome;
    }
    public void AdicionarPorta()
    {
        offset++;
        if (janelaUI == null)
        {
            
            janelaUI = GameObject.Find("Canvas/Panel").GetComponent<JanelaUI>();
        }
        if (nodoUIRectTransform == null)
            nodoUIRectTransform = GetComponent<RectTransform>();
        PortaUI portaDeEntrada;
        PortaUI portaSaida;
        RectTransform RectPortaEntrada;
        RectTransform RectPortaSaida;
      
        portaDeEntrada = GameObject.Instantiate(janelaUI.PrefabPorta).GetComponent<PortaUI>();
        portaDeEntrada.gameObject.SetActive(true);
        listaDePortas.Add(portaDeEntrada);

        portaSaida = GameObject.Instantiate(janelaUI.PrefabPorta).GetComponent<PortaUI>();
        portaSaida.gameObject.SetActive(true);
        listaDePortas.Add(portaSaida);


        //seta os parents
        RectPortaEntrada = portaDeEntrada.GetComponent<RectTransform>();

        
        RectPortaEntrada.transform.parent = nodoUIRectTransform.transform;

        RectPortaSaida = portaSaida.GetComponent<RectTransform>();
        RectPortaSaida.transform.parent = nodoUIRectTransform.transform;

        portaSaida.SetPortaUI(offset,TipoDePorta.Saida,TipoDeLigacao.Green);
        portaDeEntrada.SetPortaUI(offset, TipoDePorta.Entrada, TipoDeLigacao.Green);

        RectPortaEntrada.anchorMax = new Vector2(0,1f);
        RectPortaEntrada.anchorMin = new Vector2(0, 1f);
        RectPortaSaida.anchorMax = new Vector2(1, 1f);
        RectPortaSaida.anchorMin = new Vector2(1, 1f);
       // Debug.Log(RectPortaEntrada.anchorMax + "," + RectPortaEntrada.anchorMin + "," + RectPortaSaida.anchorMax + "," + RectPortaSaida.anchorMin);
        RectPortaEntrada.position = new Vector3(-nodoUIRectTransform.rect.width / 2 + nodoUIRectTransform.position.x,
            +nodoUIRectTransform.rect.height - (offset + 1) * (RectPortaEntrada.rect.height + 8) + nodoUIRectTransform.position.y, 0);

        RectPortaSaida.position = new Vector3(nodoUIRectTransform.rect.width / 2 + nodoUIRectTransform.position.x,
        nodoUIRectTransform.rect.height - (offset + 1) * (RectPortaEntrada.rect.height + 8) + nodoUIRectTransform.position.y, 0);

       // Debug.Log("portas adicionadas=" + listaDePortas.Count);
    }
    public PortaUI PortaSaida()
    {
        if (listaDePortas.Count == 0)
            Debug.LogError("errrooo");
        return listaDePortas.Find(x => x.tipoDePorta == TipoDePorta.Saida );
    }
    public PortaUI PortaSaidaLivre()
    {
        if (listaDePortas.Count == 0)
            Debug.LogError("errrooo");
        return listaDePortas.Find(x => x.tipoDePorta == TipoDePorta.Saida && x.listaDeLinks.Count==0);
    }
    public PortaUI PortaEntrada()
    {
        if (listaDePortas.Count == 0)
            Debug.LogError("errrooo");
        
        return listaDePortas.Find(x => x.tipoDePorta == TipoDePorta.Entrada);
    }
    public void RemoverTodasAsPortas()
    {
     //   Debug.Log("Remover todas as portas=" + listaDePortas.Count);
        for(int i=0;i<listaDePortas.Count;i++)
        {
           
            listaDePortas[i].RemoverTodosLinks();
            GameObject.Destroy(listaDePortas[i].gameObject);
            
            Debug.Log(listaDePortas.Count + "-> removendo porta " + i);
        }
        offset = 0;
        listaDePortas.Clear();
    }
   
    public PropriedadeUIString AdicionarPropriedade() 
    {
        if (janelaUI == null)
        {
            //Debug.Log("ops");
            janelaUI = GameObject.Find("Canvas/Panel").GetComponent<JanelaUI>();
        }
       // Transform props = janelaUI.Propriedades.GetComponent<PropriedadesUI>().ListaDePropriedades.transform;
       // GameObject filho= props.GetChild(props.childCount-1).ga;
        PropriedadeUIString nova = GameObject.Instantiate(janelaUI.PropriedadeString).GetComponent<PropriedadeUIString>();
        nova.paiNodo = this;
        listaDePropriedades.Add(nova);
        nova.gameObject.SetActive(false);
        return nova;
    }
    public void RemoverPropriedade( PropriedadeUIString prop) 
    {
        listaDePropriedades.Remove(prop);
        GameObject.Destroy(prop.gameObject);
    }

    public void MenuPropriedades()
    {
        janelaUI.Propriedades.GetComponent<PropriedadesUI>().nodoAtual = this;

        janelaUI.Propriedades.SetActive(true);
       
    }
    private Vector2 oldMouePosition = Vector2.zero;
    public void ExpandirNodoDiagonal() 
    {
    
        Vector2 currentMousePosition = Input.mousePosition;
 
        float xMovement =oldMouePosition.x - currentMousePosition.x;
        float yMovement = oldMouePosition.y - currentMousePosition.y;
        oldMouePosition = currentMousePosition;

        if (Mathf.Abs(xMovement) <= 40.0f && Mathf.Abs(yMovement) <= 40.0f)
        {
            nodoUIRectTransform.sizeDelta += new Vector2(-xMovement, yMovement); 
        
        }
        Debug.Log("ExpandirNodoDiagonal(){}");
        AtualizarLinks();
    }
 
   
    public void ArrastarNodo() 
    {  
        //Debug.Log("ArrastandoNodo(){}");
        if (Input.GetMouseButton(0) )
        {
            janelaUI.PrefabJanelaMenu.SetActive(false);
            janelaUI.PrefabNodoMenu.SetActive(false);
          //  Vector3 mouse = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
          //  Vector3 canvasV = new Vector3(canvas.rect.width / 2, canvas.rect.height / 2, 0);
          //  Vector3 nodoV = new Vector3(-nodoUIRectTransform.rect.width *1.2f, -nodoUIRectTransform.rect.height *1.7f, 0);
            transform.position = transform.position - new Vector3(janelaUI.xMovement, janelaUI.yMovement)*1.5f;
            AtualizarLinks();
        }
        
    }
    public void AtualizarLinks()
    {
        foreach (var porta in listaDePortas)
            foreach (var link in porta.listaDeLinks)
                link.RedesenharLink();
    }
    /*
    public void Zoom() 
    {
        Debug.Log("Zoom");
        nodoUIRectTransform.sizeDelta += Vector2.one*5*(Input.mouseScrollDelta.y);
        AtualizarLinks();
    }
    */
    public void Selecionar()
    {
        if (selecionado == true)
            return;
        if (janelaUI == null)
            janelaUI= GameObject.Find("Canvas/Panel").GetComponent<JanelaUI>();
        janelaUI.PrefabNodoMenu.SetActive(false);
        janelaUI.PrefabJanelaMenu.SetActive(false);
            //Debug.Log("selecionar(){}");
            selecionado = true;
            GetComponent<Image>().color = Color.cyan;
            transform.SetAsLastSibling();
            if (janelaUI.nodoSelecionadoAtualmente != null && janelaUI.nodoSelecionadoAtualmente.name!=gameObject.name )
                janelaUI.nodoSelecionadoAtualmente.DesSelecionar();
            janelaUI.nodoSelecionadoAtualmente = this;
    }
    public void DesSelecionar()
    {
       
          //  Debug.Log("desselecionar(){}");
            selecionado = false;
            GetComponent<Image>().color = Color.white;
        
    }
    public void MenuNodo()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Selecionar();
          //  Debug.Log("menu nodo");
            janelaUI.PrefabNodoMenu.SetActive(true);
            janelaUI.PrefabJanelaMenu.SetActive(false);
            /*
            Vector3 offset = new Vector3(nodoUIRectTransform.rect.width,nodoUIRectTransform.rect.height);
            janelaUI.PrefabNodoMenu.transform.position = transform.position + offset/4; //+ new Vector3( janelaUI.xMovement,janelaUI.yMovement );
            */
            Vector3 mouse = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
            Vector3 canvasV = new Vector3(canvas.rect.width / 2, canvas.rect.height / 2, 0);
            Vector3 nodoV = new Vector3(-janelaUI.PrefabNodoMenu.GetComponent<RectTransform>().rect.width , -janelaUI.PrefabNodoMenu.GetComponent<RectTransform>().rect.height , 0);
            janelaUI.PrefabNodoMenu.GetComponent<RectTransform>().anchoredPosition3D = mouse - canvasV; //- nodoV;
        }
       
    }

}
/*
public class Nodo
{

    //  bool invisivel;
    //   bool EstaSelecionado;
    public readonly int id;
    List<Propriedade> listaDePropriedades = new List<Propriedade>();
    List<PortaUI> listaDePortas = new List<PortaUI>();
    NodoUI nodoUI;
    public Nodo(int _id)
    {
        nodoUI = new NodoUI(_id);
        id = _id;
   
    }
  
    public void AdicionarPropriedade(Propriedade propriedade)
    { listaDePropriedades.Add(propriedade); }
    public bool RemoverPropriedade(string _nome)
    { return listaDePropriedades.Remove(listaDePropriedades.Find(x => x.nome == _nome)); }
    public void AdicionarPorta()
    {
       
        nodoUI.AdicionarPorta();
    }
    /*
    public bool RemoverPorta(int _offset, TipoDePorta tipo)
    {
        PortaUI porta = listaDePortas.Find(x => x.offset == _offset && x.tipoDePorta == tipo);
        porta.RemoverTodosLinks();
        return listaDePortas.Remove(porta);
    }
    */
    /*
    public void RemoverTodasAsPortas()
    {
        foreach (var porta in listaDePortas)
            porta.RemoverTodosLinks();

        listaDePortas.Clear();
    }
    
    
    public List<int> TodosIDsDosLinks()
    {
        List<int> lista = new List<int>();
        foreach (var porta in listaDePortas)
            foreach (var link in porta.listaDeLinks)
                lista.Add(link.id);

        return lista;
    }

    public void DesconectarComUmaPorta(PortaUI portaSaida) { }


}
*/