using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public enum TipoDePorta
{
    Entrada = 0,
    Saida
}
public class PortaUI : MonoBehaviour
{
    JanelaUI janelaUI;
    public  TipoDePorta tipoDePorta;
    public  TipoDeLigacao tipoDeLigacao;
    public  int offset;

    //public Propriedade valor;
    public List<LinkUI> listaDeLinks = new List<LinkUI>();
  

    public void SetPortaUI(int _offset, TipoDePorta _tipoDePorta, TipoDeLigacao _tipoDeLigacao)
    {
        tipoDeLigacao = _tipoDeLigacao;
        tipoDePorta = _tipoDePorta;
        offset = _offset;
        if (tipoDePorta == TipoDePorta.Entrada)
            name = "Entrada" + offset.ToString()+ transform.parent.name;
        else
            name = "Saida" + offset.ToString()+ transform.parent.name;
    }

    void Start()
    {
        janelaUI = GameObject.Find("Canvas/Panel").GetComponent<JanelaUI>();
    }
    void Update()
    {

        if (listaDeLinks.Count > 0)
            GetComponent<Toggle>().isOn = true;
    
          
    }
    public void LigarConexao()
    {
        if (listaDeLinks.Count > 0)
            GetComponent<Toggle>().isOn = true;
        else
            GetComponent<Toggle>().isOn = false;
       
        
        
        //se veio aqui o toggle foi mudado
        if(janelaUI.modoMouse==ModoMouse.Idle)
        {
            if (janelaUI.PortaSelecionadaAtualmente == null)
            {
                if(tipoDePorta==TipoDePorta.Entrada && listaDeLinks.Count!=0)
                {
                   
                    listaDeLinks[0].saida.ComecarConeaxao();
                    janelaUI.LinkSelecionadoAtualmente = listaDeLinks[0];
                    janelaUI.RemoverLink();
                    GetComponent<Toggle>().isOn = false;
                    return;
                }
                GetComponent<Toggle>().isOn = true;
                ComecarConeaxao();
                return;
            }
        }
        if (janelaUI.modoMouse==ModoMouse.Connecting)
        {
            if (janelaUI.PortaSelecionadaAtualmente == null)
            {
                Debug.Log("esta no modo connecting com nodoatualmente null -> errado");
                return;
            }
            TerminarConexao(); 
        }
        
    }

    void ComecarConeaxao()
    {
       // Debug.Log("começando conexao");
        transform.parent.GetComponent<NodoUI>().Selecionar();
        Selecionar();
        janelaUI.modoMouse = ModoMouse.Connecting;
    }
    void TerminarConexao()
    {      
        if (janelaUI.nodoSelecionadoAtualmente == this.transform.parent.gameObject)
        {
            if (janelaUI.PortaSelecionadaAtualmente == this)
                return;
            DesSelecionar();
            janelaUI.modoMouse = ModoMouse.Idle;
            Debug.Log("mesmo nodo");
            return;
        }     
        if (janelaUI.PortaSelecionadaAtualmente.tipoDePorta == this.tipoDePorta)
        {
            DesSelecionar();
            //Debug.Log("mesmo tipo de porta");
            return;
        }
        janelaUI.AdicionarLink(janelaUI.PortaSelecionadaAtualmente, this);
       
       
        janelaUI.PortaSelecionadaAtualmente = null;       
        janelaUI.modoMouse = ModoMouse.Idle;
     //   Debug.Log("criando link...");
    }
    public void Selecionar()
    {
        
        janelaUI.PortaSelecionadaAtualmente = this;
       
    }
   
    public void DesSelecionar()
    {
        GetComponent<Toggle>().isOn = false;
      
    }

    public void RemoverTodosLinks()
    {
        Debug.Log("remover todos os links="+listaDeLinks.Count);
        for (int i = 0; i < listaDeLinks.Count;i++ )
        {
            int id = listaDeLinks[i].id;
            Debug.Log("apagando esse link=" + id);
          
            janelaUI.RemoverLink(id);

        }
    }


    /*
    public void Adicionarlink(Link link)
    { 
        listaDeLinks.Add(link); 
    }
    public Link ObterLink(PortaUI OutraPorta)
    {
        foreach (var link in listaDeLinks)
        {
            if (link.entrada == OutraPorta || link.saida == OutraPorta)
                return link;
        }
        return null;

    }
    public void RemoverLink(int _id) 
    { 
        listaDeLinks.Remove(listaDeLinks.Find(x => x.id == _id));
    
    }

    public void RemoverTodosLinks()
    {
        foreach (var link in listaDeLinks)
        {
            link.OutraPorta(tipoDePorta).listaDeLinks.Remove(link);
            listaDeLinks.Remove(link);
        }
    }

    */
}

