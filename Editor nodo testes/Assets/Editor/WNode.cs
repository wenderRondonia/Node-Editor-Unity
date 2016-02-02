using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
public class WNode  {

    
   
   
    public int Id = 0; 
    public string _nome = "WNode";
    public string Nome
    {
        get
        {
            return _nome;
        }
        set
        {
            _nome = value;
        }
    }
    [SerializeField]
    protected List<Porta> _listaDePortas = new List<Porta>();
    [SerializeField]
    public List<Propriedade> listaDePropriedades = new List<Propriedade>();   
    public List<Porta> listaDePortas { get { return _listaDePortas; } }
    [SerializeField]
    protected Rect _posicaoDaCaixaX = new Rect(10, -25, 20, 20);
    [SerializeField]
    protected Rect _posicaoDoMenu = new Rect(40, -25, 20   , 20);
    
    //Construtor
    public WNode(string nome, int ID)
    {

        Nome = nome;
        Id = ID;
        _nome += ID.ToString();
        Rect = new Rect(_posicao.x, _posicao.y, _tamanho.x, _tamanho.y);

        AdicionarPorta(TipoDePorta.ENTRADA, "porta");
        AdicionarPorta(TipoDePorta.SAIDA, "porta");

        AdicionarPorta(TipoDePorta.ENTRADA, "porta");
        AdicionarPorta(TipoDePorta.SAIDA, "porta");

        AdicionarPropriedade("estado", "1");
        AdicionarPropriedade("codigo", "1");
        AdicionarPropriedade("opcao um", "1");
        AdicionarPropriedade("opcao dois", "1");
    }
    public WNode(string nome, int ID,float x, float y, float tamanhox,float tamanhoy)
    {

        Nome = nome;
        Id = ID;
        _nome += ID.ToString();
        Rect = new Rect(x, y, tamanhox, tamanhoy);
        _posicao.x = x;
        _posicao.y = y;
        _tamanho.x = tamanhox;
        _tamanho.y = tamanhoy;
        AdicionarPorta(TipoDePorta.ENTRADA,  "porta");
        AdicionarPorta(TipoDePorta.SAIDA,  "porta");

        AdicionarPorta(TipoDePorta.ENTRADA,  "porta");
        AdicionarPorta(TipoDePorta.SAIDA,"porta");

        AdicionarPropriedade("estado", "1");
        AdicionarPropriedade("codigo", "1");
        AdicionarPropriedade("opcao um", "1");
        AdicionarPropriedade("opcao dois", "1");

    }
    bool ExisteEssaCor(Color cor)
    {
        for (int i = 0; i < listaDePortas.Count; i++)
        {
            if (listaDePortas[i].cor == cor)
            {
                return true;
            }
           
        }
        return false;
    }
    public  void AdicionarPortas()
    {
        //TODO: usar um método melhor pra tirar cores novas
        Color novaCor=Color.green;
        if(ExisteEssaCor(novaCor))
            novaCor=Color.blue;
             if(ExisteEssaCor(novaCor))
                 novaCor=Color.red;
                 if(ExisteEssaCor(novaCor))
                     novaCor=Color.yellow;
                     if (ExisteEssaCor(novaCor))
                         novaCor = Color.black;
                         if (ExisteEssaCor(novaCor))
                             novaCor = Color.magenta;
                                 if (ExisteEssaCor(novaCor))
                                     Debug.LogError("erro! adicionar mais cores a este script");
        AdicionarPorta(TipoDePorta.ENTRADA,"porta");
        AdicionarPorta(TipoDePorta.SAIDA, "porta");
    }
     void AdicionarPorta(TipoDePorta tipo, string nome="Porta")
    {
        Porta porta = new Porta((int)listaDePortas.Count / 2);
        porta.Nome = nome;     
        porta.tipoDaPorta = tipo;
        porta.nodoDono=this;
        _listaDePortas.Add(porta);

    }
     
    public void AdicionarPorta(Porta porta)
     {
         _listaDePortas.Add(porta);

     }
    public void RemoverUltimoParDePortas()
     {
         if (listaDePortas.Count > 2)
         {
             DestruirUltimaPorta();
             DestruirUltimaPorta();
         }
         else
         {
             Debug.Log("~deixei pra não retirar todas as portas, se for mudar, tem que fazer mais verificações no codigo sobre null");
         }
     }

    public Porta PortaEntradaLivre()
    {
        for (int i = 0; i < listaDePortas.Count; i++)
        {
            if (listaDePortas[i].Link == null && listaDePortas[i].tipoDaPorta == TipoDePorta.ENTRADA)
            {
                return listaDePortas[i];
            }
        }
        return null;
    }
    public Porta PortaSaidaLivre()
    {
        for (int i = 0; i < listaDePortas.Count; i++)
        {
            if (listaDePortas[i].Link == null && listaDePortas[i].tipoDaPorta == TipoDePorta.SAIDA)
            {
                return listaDePortas[i];
            }
        }
        return null;
    }
    public int NumeroDePortasENTRADA()
    {
        int numPortas=0;
        foreach(var porta in listaDePortas)
        {
            if (porta.tipoDaPorta == TipoDePorta.ENTRADA)
                numPortas++;
        }
        return numPortas;
    }
    public int NumeroDePortasSAIDA()
    {
        int numPortas = 0;
        foreach (var porta in listaDePortas)
        {
            if (porta.tipoDaPorta == TipoDePorta.SAIDA)
                numPortas++;
        }
        return numPortas;
    }
     void DestruirUltimaPorta()
    {
        
        _listaDePortas.RemoveAt(_listaDePortas.Count-1);
       
    }
    // parte geométrica da GUI
    
    public Rect Rect=new Rect();
   
    public Vector2 _posicao = new Vector2(200, 200);

  
    public Vector2 _tamanho = new Vector2(100, 100);
   
    
    //parte das propriedades do WNode 
    /*
    public void AdicionarPropriedade( string nome, int valor  )
    {
        listaDePropriedades.Add( new Propriedade(nome, valor));
    }
    */
    public void AdicionarPropriedade(string nome, string valor)
    {
        listaDePropriedades.Add(new Propriedade(nome, 0,valor,0));
    }
    public void RemoverPropriedade(string nome="")
    {
        if(nome =="")
        {
            //remove a ultima
            listaDePropriedades.RemoveAt(listaDePropriedades.Count-1);

        }
        else
        {
            int indice = PesquisarPropriedade(nome);
            if (indice == -1)
                Debug.Log("erro ao remover propriedade, nome não encontrado");
            else
                listaDePropriedades.RemoveAt(indice);
        }
    }
    int  PesquisarPropriedade(string nome)
    {
        for(int i=0; i< listaDePropriedades.Count;i++)
        {

            if(listaDePropriedades[i].Nome.Equals(nome)==true)
            {
                return i;
            }

        }
        return -1;
    }
    /*
    public void AdicionarPropriedade(string nome, float valor)
    {
        listaDePropriedades.Add(new Propriedade(nome,0,"" ,valor));
    }
    */
    public void RemoverTodasAsPortas()
    {
        
        listaDePortas.Clear();
    }

    #region NODE_EDITOR_FUCTIONS
#if UNITY_EDITOR
    public void Desenhar()
    {
        Rect.x=_posicao.x;
        Rect.y=_posicao.y;
        Rect.width = _tamanho.x;
        Rect.height = _tamanho.y;
        Rect = GUI.Window(Id, Rect, WindowCallback, "_nome " + Id);
     
          
        //atualiza umas variaveis auxiliares
        _posicao.x = Rect.x;
        _posicao.y = Rect.y;
        _tamanho.x = Rect.width;
        _tamanho.y = Rect.height;
         GUI.Box(_posicaoDaCaixaX, Resources.Load<Texture2D>("botaoFechar"));
         GUI.Box(_posicaoDoMenu, Resources.Load<Texture2D>("menu"));
         for (int i = 0; i < listaDePortas.Count; i++)
         {
             listaDePortas[i].DesenharPorta();
         }
    }

    public virtual bool MouseOver(Vector2 mousePos)
    {

        bool handled = false;
        for (int i = 0; i < listaDePortas.Count; i++)
        {
            if (listaDePortas[i].MouseOver(mousePos))
            {
                handled = true;
                break;
            }
        }

        return handled;
    }
    public virtual bool MouseDown(Vector2 mousePos, int button)
    {
        bool handled = false;
        for (int i = 0; i < listaDePortas.Count; i++)
        {
            if (listaDePortas[i].MouseDown(mousePos, button))
            {
                handled = true;
                break;
            }

         
        }

        return handled;
    }
    public virtual bool MouseDown(Vector2 mousePos)
    {

        if (mousePos.x > _posicao.x
            && mousePos.x < _posicao.x + _tamanho.x
            && mousePos.y > _posicao.y
            && mousePos.y < _posicao.y + _tamanho.y)
            {

               
                return true;
            }
        

        return false;
    }
    public virtual bool MouseDownNodeCima(Vector2 mousePos)
    {

        if (mousePos.x > _posicao.x
            && mousePos.x < _posicao.x + _tamanho.x
            && mousePos.y > _posicao.y
            && mousePos.y < _posicao.y + 20)
        {

            Debug.Log("mouse down no node!");
            return true;
        }


        return false;
    }
    public virtual Porta MouseDownPorta(Vector2 mousePos,int button)
    {

        for (int i = 0; i < listaDePortas.Count; i++)
        {
            if (listaDePortas[i].MouseDown(mousePos, button))
            {

                return listaDePortas[i];

            }
        }

        return null;
    }
    public virtual bool MouseOnX(Vector2 mousePos, int button)
    {
        bool handled = false;
        if (
            mousePos.x > _posicaoDaCaixaX.x 
            && mousePos.x < _posicaoDaCaixaX.x + _posicaoDaCaixaX.width
            && mousePos.y > _posicaoDaCaixaX.y
            && mousePos.y < _posicaoDaCaixaX.y + _posicaoDaCaixaX.height
            )
        {
            if (button == 0)
            {
                handled = true;
               // Debug.Log("clicou botão x!");
            }
        }

        return handled;

    }
    public virtual bool MouseOnMenu(Vector2 mousePos, int button)
    {
        bool handled = false;
        if (
            mousePos.x > _posicaoDoMenu.x
            && mousePos.x < _posicaoDoMenu.x + _posicaoDoMenu.width
            && mousePos.y > _posicaoDoMenu.y
            && mousePos.y < _posicaoDoMenu.y + _posicaoDoMenu.height
            )
        {
            if (button == 0)
            {
                handled = true; 
            }
        }

        return handled;

    }
    public virtual bool MouseUpNode(Vector2 mousePos)
    {
        if (mousePos.x > _posicao.x
          && mousePos.x < _posicao.x + _tamanho.x
          && mousePos.y > _posicao.y
          && mousePos.y < _posicao.y + _tamanho.y)
        {
            
            return true;
        }
        return false;

    }
    public virtual bool MouseUp(Vector2 mousePos)
    {

        bool handled = false;
        for (int i = 0; i < listaDePortas.Count; i++)
        {
            if (listaDePortas[i].MouseUp(mousePos)==true)
            {
               // Debug.Log("mouse up!");
                handled = true;
                break;
            }
        }

        return handled;
    }
    public virtual Porta MouseUpPorta(Vector2 mousePos)
    {
    
        for (int i = 0; i < listaDePortas.Count; i++)
        {
            if (listaDePortas[i].MouseUp(mousePos))
            {
          
                return listaDePortas[i];
                
            }
        }

        return null;
    }
    public virtual bool MouseDrag(Vector2 mousePos)
    {

        bool handled = false;
        for (int i = 0; i < listaDePortas.Count; i++)
        {
            if (listaDePortas[i].MouseDrag(mousePos))
            {
                handled = true;
                break;
            }
        }
        return handled;
    }



    public virtual void WindowCallback(int id)
     {
         
        //função chamada no NodeWindow.cs ao desenhar node na tela
        //desenha os auxiliares do node       
        GUI.DragWindow(new Rect(0,0,_tamanho.x,20 ));
        //opção do menu
        _posicaoDoMenu = new Rect(_posicao.x + 40, _posicao.y - _posicaoDoMenu.height, _posicaoDoMenu.width, _posicaoDoMenu.height);
        //opção pra apagar o node
        _posicaoDaCaixaX = new Rect(_posicao.x + 2, _posicao.y - _posicaoDaCaixaX.height, _posicaoDaCaixaX.width, _posicaoDaCaixaX.height);

     
        //mostra a lista de propriedades:
        for (int i = 0; i < listaDePropriedades.Count; i++)
        {
 
           listaDePropriedades[i].Nome = GUILayout.TextField(listaDePropriedades[i].Nome);

           listaDePropriedades[i].texto = EditorGUILayout.TextField(listaDePropriedades[i].texto, GUILayout.ExpandHeight(true));
            //tira foco dos TextField, tava meio irritante xD
           //Debug.Log(listaDePropriedades[i].Nome + listaDePropriedades[i].texto);
        }
     }
    
#endif
    #endregion

   
}


