using UnityEngine;
using System.Collections;


public class LinkUI : MonoBehaviour
{

    public  int id;
    public  TipoDeLigacao tipoLink;
    public  PortaUI saida;
    public  PortaUI entrada;
    BezierManager scriptBezier;
    // Event output;
    // 	->http://www.codeproject.com/Articles/29922/Weak-Events-in-C
    //EventHandler input;
    public void SetLinkUI(PortaUI porta1, PortaUI porta2, int _id)
    {
        
        if (porta1.tipoDeLigacao == porta2.tipoDeLigacao && porta1.tipoDePorta != porta2.tipoDePorta)
        {
            tipoLink = porta1.tipoDeLigacao;
            id = _id;
            name = "Link"+id.ToString();
            if (porta1.tipoDePorta == TipoDePorta.Entrada)
            {
                saida = porta2;
                entrada = porta1;
            }
            else
            {
                saida = porta1;
                entrada = porta2;
            }
        }
        else
        {
            Debug.LogError("erro ao criar porta");
            tipoLink = TipoDeLigacao.Errada;
        }

        scriptBezier= GetComponent<BezierManager>();
    
       
      
        scriptBezier.lineRenderer.SetColors(Color.green,Color.green);
        scriptBezier.Render(saida.transform.position, entrada.transform.position);
    }

    public void RedesenharLink()
    {
        scriptBezier.Render(saida.transform.position, entrada.transform.position);
    }
    public PortaUI OutraPorta(TipoDePorta tipo)
    {
        if (saida.tipoDePorta != tipo)
            return saida;
        else
            return entrada;
    }
    public PortaUI EssaPorta(TipoDePorta tipo)
    {
        if (saida.tipoDePorta == tipo)
            return saida;
        else
            return entrada;
    }
}
public enum TipoDeLigacao
{
    Errada = 0,
    White,
    Green,
    Red

}
