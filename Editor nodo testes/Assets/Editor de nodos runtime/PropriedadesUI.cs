using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;
using System.Collections;
using System;
using System.Xml;

public class PropriedadesUI : MonoBehaviour {

   public GameObject ListaDePropriedades;
    RectTransform rectTransform;
   // PropriedadeUIBool propBool;
   // PropriedadeUIString propString;
    public NodoUI nodoAtual;
    Text textScript;

    JanelaUI janelaUI;
    void Start()
    {
        janelaUI = GameObject.Find("Canvas/Panel").GetComponent<JanelaUI>();
        ListaDePropriedades = transform.FindChild("ScrollRect/ListaDePropriedades").gameObject ;
        textScript = transform.FindChild("NodoNome").GetComponent<Text>();
        rectTransform = ListaDePropriedades.GetComponent<RectTransform>();
       // propBool = ListaDePropriedades.transform.FindChild("PropriedadeBool").GetComponent<PropriedadeUIBool>();
     //   propString = ListaDePropriedades.transform.FindChild("PropriedadeString").GetComponent<PropriedadeUIString>();
        

    }
 
    public void LerPropriedadesDoNodo()
    {
        if(nodoAtual==null)
        {
            Debug.LogError("erro!");
        }
        if (ListaDePropriedades == null)
        {
            ListaDePropriedades = transform.FindChild("ScrollRect/ListaDePropriedades").gameObject;
            //Debug.Log("ops");
        }
        if (rectTransform == null)
        {
            rectTransform = ListaDePropriedades.GetComponent<RectTransform>();
        }
        
        for (int i = 0; i < nodoAtual.listaDePropriedades.Count; i++)
        {
            RectTransform rectTransformProp=nodoAtual.listaDePropriedades[i].GetComponent<RectTransform>();
            AjustarPropsUI(rectTransformProp,i);
    
        }
    }
    void AjustarPropsUI(RectTransform rectTransformProp, int i)
    {
        rectTransformProp.gameObject.SetActive(true);
        rectTransformProp.transform.parent = ListaDePropriedades.transform;
        //redimensionar
        rectTransformProp.SetSizeWithCurrentAnchors( RectTransform.Axis.Horizontal,rectTransform.rect.width*(90f/100f) );
        //posicionar
        float altura = rectTransformProp.rect.height;
        float offset = rectTransform.rect.height / 2 - altura / 2;
        rectTransformProp.localPosition = new Vector2(0, offset - altura * i);
        //rectTransformProp.anchoredPosition = new Vector2(0, offset - altura * i);
        UtilidadesUI.AjustarAnchors(rectTransformProp.gameObject);
       
    }
    public void Scroll()
    {
        ScrollRect scriptScrollRect=transform.FindChild("ScrollRect").GetComponent<ScrollRect>();
       // Debug.Log(scriptScrollRect.normalizedPosition);
        if (scriptScrollRect.normalizedPosition.y > 1 && Input.mouseScrollDelta.y > 0)
            return;
        if (scriptScrollRect.normalizedPosition.y <0 && Input.mouseScrollDelta.y < 0)
            return;
        scriptScrollRect.normalizedPosition += Input.mouseScrollDelta / 15;
    }
   
    public void AdicionarPropriedade()
    {
        //fazer mais tipos aqui
        nodoAtual.AdicionarPropriedade();
        LerPropriedadesDoNodo();
    }

    public void RemoverPropriedades()
    {
         for (int i = 0; i < nodoAtual.listaDePropriedades.Count; i++)
         {
            nodoAtual.listaDePropriedades[i].gameObject.SetActive(false);

         }
    }
    private Vector2 oldMouePosition = Vector2.zero;
    public void ExpandirNodoDiagonal()
    {

        Vector2 currentMousePosition = Input.mousePosition;

        float xMovement = oldMouePosition.x - currentMousePosition.x;
        float yMovement = oldMouePosition.y - currentMousePosition.y;
        oldMouePosition = currentMousePosition;

        if (Mathf.Abs(xMovement) <= 40.0f && Mathf.Abs(yMovement) <= 40.0f)
        {
            GetComponent<RectTransform>().sizeDelta += new Vector2(-xMovement, yMovement);
            
        }
       
    }

    public void Arrastar()
    {
        //Debug.Log("ArrastandoNodo(){}");
        if (Input.GetMouseButton(0))
        {
          
            //  Vector3 mouse = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
            //  Vector3 canvasV = new Vector3(canvas.rect.width / 2, canvas.rect.height / 2, 0);
            //  Vector3 nodoV = new Vector3(-nodoUIRectTransform.rect.width *1.2f, -nodoUIRectTransform.rect.height *1.7f, 0);
            transform.position = transform.position - new Vector3(janelaUI.xMovement, janelaUI.yMovement) * 1.5f;
            
        }

    }
    void OnEnable()
    {
        if (nodoAtual==null)
            return;
        janelaUI = GameObject.Find("Canvas/Panel").GetComponent<JanelaUI>();
        janelaUI.PrefabNodoMenu.SetActive(false);
        LerPropriedadesDoNodo();
        textScript = transform.FindChild("NodoNome").GetComponent<Text>();
        textScript.text = nodoAtual.name;
    }

    void OnDisable() 
     {
         if (nodoAtual == null)
             return;
         RemoverPropriedades();
     }


}


public class UtilidadesUI : MonoBehaviour
{
    public static void AjustarAnchors(GameObject o)
    {

        if (o != null && o.GetComponent<RectTransform>() != null)
        {
            var r = o.GetComponent<RectTransform>();
            var p = o.transform.parent.GetComponent<RectTransform>();

            var offsetMin = r.offsetMin;
            var offsetMax = r.offsetMax;
            var _anchorMin = r.anchorMin;
            var _anchorMax = r.anchorMax;

            var parent_width = p.rect.width;
            var parent_height = p.rect.height;

            var anchorMin = new Vector2(_anchorMin.x + (offsetMin.x / parent_width),
                                        _anchorMin.y + (offsetMin.y / parent_height));
            var anchorMax = new Vector2(_anchorMax.x + (offsetMax.x / parent_width),
                                        _anchorMax.y + (offsetMax.y / parent_height));

            r.anchorMin = anchorMin;
            r.anchorMax = anchorMax;

            r.offsetMin = new Vector2(0, 0);
           r.offsetMax = new Vector2(1, 1);
           // r.offsetMax = new Vector2(0, 0);
            r.pivot = new Vector2(0.5f, 0.5f);

        }
    }
}
