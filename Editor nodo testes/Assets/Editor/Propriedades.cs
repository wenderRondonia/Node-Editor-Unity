using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Propriedade
{
    private string _nome;

    public string Nome
    {
        get
        {
            if (string.IsNullOrEmpty(_nome))
            {
                Debug.Log("propriedade [Nome] na classe [Propriedade] é null ou vazia");
            }
            return _nome;
        }
        set { _nome = value; }
    }

    public string texto;
    public int numero=0;
    public float flutuante=0;
    //por ai vai

    public Propriedade(string nome, int valor = 0, string _texto = "", float _flutuante = 0)
    {
        _nome = nome;
        texto = _texto;
        numero = valor;
        flutuante = _flutuante;
    }
}

