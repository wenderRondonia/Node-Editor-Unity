using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum TipoDePorta
{ 
    INDEFINIDO,
    ENTRADA,
    SAIDA

}

public class Porta
{
    public Texture2D textura;
    //public GUISkin guiskin;
    public Color cor;
    public Porta(int offset)
    {
        yOffset = offset;
        cor = Color.white;
        switch(offset)
        {
            case 0: cor = Color.green; break;
            case 1: cor = Color.blue; break;
            case 2: cor = Color.red; break;
            case 3: cor = Color.yellow; break;
            case 4: cor = Color.black; break;
            case 5: cor = Color.cyan; break;
            default:
               
                Debug.LogError("erro cor da porta não suportada");
                break;
        }
       
        //guiskin = Resources.Load<GUISkin>("GUISkinNode");
     //   textura = Resources.Load<Texture2D>("CirculoBranco");
        textura = (Texture2D)Object.Instantiate(Resources.Load<Texture2D>("CirculoBranco"));
        Color[] colors = textura.GetPixels();
        for (int i = 0; i < colors.Length;i++ )
        { 
            if(colors[i].a!=0)
                colors[i] = cor;
         }
        textura.SetPixels(colors);
        textura.Apply();    

    }
    public Porta(string nome,int offset, TipoDePorta tipo, WNode nodoPai)
    {

        Nome = nome;
        yOffset = offset;
        tipoDaPorta = tipo;
        nodoDono = nodoPai;
        cor = Color.white;
        switch(offset)
        {
            case 0: cor = Color.green; break;
            case 1: cor = Color.blue; break;
            case 2: cor = Color.red; break;
            case 3: cor = Color.yellow; break;
            case 4: cor = Color.black; break;
            case 5: cor = Color.cyan; break;
            default:
                
                Debug.LogError("erro cor da porta não suportada");
                break;
        }
        //guiskin = Resources.Load<GUISkin>("GUISkinNode");
        //   textura = Resources.Load<Texture2D>("CirculoBranco");
        textura = (Texture2D)Object.Instantiate(Resources.Load<Texture2D>("CirculoBranco"));
        Color[] colors = textura.GetPixels();
        for (int i = 0; i < colors.Length; i++)
        {
            if (colors[i].a != 0)
                colors[i] = cor;
        }
        textura.SetPixels(colors);
        textura.Apply();

    }
    public string Nome = "Porta";
    public TipoDePorta tipoDaPorta=TipoDePorta.ENTRADA;
    public WNode nodoDono = null;
    public Rect Posicao = new Rect(0, 0, 0, 0);
    [SerializeField]
    protected Vector2 _tamanho = new Vector2(20, 20);
    public int yOffset;
    public WLink Link = null;

  
    #region NODE_EDITOR_FUCTIONS
#if UNITY_EDITOR
    public virtual void DesenharPorta()
    {
        if (nodoDono == null)
            return;
        Rect rect = nodoDono.Rect;
        
        if(tipoDaPorta==TipoDePorta.ENTRADA)
        {

            Posicao = new Rect(rect.x - 20, rect.y + 2 + yOffset * (_tamanho.y+10), _tamanho.x, _tamanho.y);
            GUI.Box(Posicao, textura);//, guiskin.box);
            return;
        }
        if (tipoDaPorta == TipoDePorta.SAIDA)
        {
            Posicao = new Rect(rect.x + rect.width - 2, rect.y + 2 + yOffset * (_tamanho.y+10), _tamanho.x, _tamanho.y);
            GUI.Box(Posicao, textura);//,guiskin.box);
            return;
        }
        Debug.Log("erro");

    }

    public void MudarCor(Color novacor)
    {
        
        textura = (Texture2D)Object.Instantiate(Resources.Load<Texture2D>("CirculoBranco"));
        Color[] colors = textura.GetPixels();
        for (int i = 0; i < colors.Length; i++)
        {
            if (colors[i].a !=0)
                colors[i] = novacor;
        }
        textura.SetPixels(colors);
       // textura.Apply();

    }
    public virtual bool MouseOver(Vector2 mousePos)
        {
            if (mousePos.x > Posicao.x
            && mousePos.x < Posicao.x + _tamanho.x
            && mousePos.y > Posicao.y - _tamanho.y
            && mousePos.y < Posicao.y + _tamanho.y)
            {
               

                return true;
            }
            return false;
        }
    public virtual bool MouseDown(Vector2 mousePos, int button)
    {


        if (   mousePos.x > Posicao.x 
            && mousePos.x < Posicao.x  + _tamanho.x 
            && mousePos.y > Posicao.y - _tamanho.y 
            && mousePos.y < Posicao.y + _tamanho.y)
        {
            
               
               
            
            return true;
        }

        return false;
    }
    public virtual bool MouseUp(Vector2 mousePos)
    {

        if (mousePos.x > Posicao.x
                    && mousePos.x < Posicao.x + _tamanho.x
                    && mousePos.y > Posicao.y - _tamanho.y
                    && mousePos.y < Posicao.y + _tamanho.y)
        {

            
            return true;
        }
        return false;
    }
    public virtual bool MouseDrag(Vector2 mousePos)
    {
        if (mousePos.x > Posicao.x
            && mousePos.x < Posicao.x + _tamanho.x
            && mousePos.y > Posicao.y - _tamanho.y
            && mousePos.y < Posicao.y + _tamanho.y)
        {


            return true;
        }
        return false;
    }

#endif
    #endregion
}

public class WLink  {

    public string Name = "Link";
    [SerializeField]
    public Porta portaEntrada()
    {
        if (portas[0].tipoDaPorta == TipoDePorta.ENTRADA)
            return portas[0];
        else
            return portas[1];
    }
    public Porta portaSaida()
    {
        if (portas[0].tipoDaPorta == TipoDePorta.SAIDA)
            return portas[0];
        else
            return portas[1];
    }
    public Porta[] portas = new Porta[2];
    public WLink(Porta _porta1,Porta _porta2)
    {
        portas[0] = _porta1;
        portas[1]= _porta2;
    }

   
}
