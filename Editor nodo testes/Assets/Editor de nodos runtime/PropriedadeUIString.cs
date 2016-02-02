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
public class PropriedadeUI : MonoBehaviour {

   
    
	void Start () 
    {
	
	}
	
	
	void Update () 
    {
	
	}

    public void MudarNome()
    {
        nome=transform.FindChild("Nome/Text").GetComponent<Text>().text;
    }
    public virtual void MudarValor()
    {

    }



}

*/

public class PropriedadeUIString  : MonoBehaviour
{
    public string nome="";
    public string Valor="";
    public NodoUI paiNodo;
    public void SetarNome()
    {
         transform.FindChild("Nome").GetComponent<InputField>().text =nome;
         //Debug.Log("  setando nome " + transform.FindChild("Nome/Text").GetComponent<Text>().text);
    }
    public void SetarValor()
    {
        transform.FindChild("Valor").GetComponent<InputField>().text = Valor;
       // Debug.Log("setando vslor:" + Valor);
    }

    public void MudarNome()
    {
        nome = transform.FindChild("Nome/Text").GetComponent<Text>().text;
    }
 
    public  void MudarValor()
    {
        Valor=transform.FindChild("Valor/Text").GetComponent<Text>().text;
    }

    public void RemoverEssaPropriedade()
    {
        paiNodo.RemoverPropriedade(this);
        AcharPropriedades().LerPropriedadesDoNodo();
        Debug.Log("removendo");
    }
    PropriedadesUI AcharPropriedades()
    {
               
        return GameObject.Find("Canvas2/Propriedades") .GetComponent<PropriedadesUI>();
    }


}

public class PropriedadeUIBool : MonoBehaviour
{
    public string nome="";
    public bool Valor = false;
    public void MudarNome()
    {
        nome = transform.FindChild("Nome/Text").GetComponent<Text>().text;
    }
    public  void MudarValor()
    {
        Valor = transform.FindChild("Valor").GetComponent<Toggle>().isOn;
    }
}