using UnityEngine;
using System.Collections;
using System;
using MaquinaDeEstados;


public class script : MonoBehaviour {

    Sistema sistema = new Sistema();
	// Use this for initialization


	void Start () {
        Propriedade<float> propriedade1 = new Propriedade<float>("propriedade1", 9);
        sistema.AdicionarPropriedade(propriedade1);

        Estado estado1 = new Estado("estado1",sistema);
        sistema.AdicionarEstado(estado1);
        Estado estado2 = new Estado("estado2", sistema);
        sistema.AdicionarEstado(estado2);

        Transicao transicao = new Transicao("valor maior que dez",estado1,estado2,sistema);
        sistema.AdicionarTransicao(transicao);
        Func<float, bool> comparacao = x => {  return x > 10; };
        Condicao<float> condicao = new Condicao<float>("maior", comparacao, transicao);
        transicao.AdicionarCondicao(condicao);
        
        Transicao transicao2 = new Transicao("valor menor que dez", estado2, estado1, sistema);
        sistema.AdicionarTransicao(transicao2);
        Func<float, bool> comparacao2 = x => {  return x < 10; };
        Condicao<float> condicao2 = new Condicao<float>("menor", comparacao2, transicao2);
            
        transicao2.AdicionarCondicao(condicao2);


        propriedade1.RegistrarObservador(condicao);
        propriedade1.RegistrarObservador(condicao2);
        sistema.SetarEstadoInicial(estado1.nome);
	}
	
	// Update is called once per frame
	void Update () {

        Debug.Log(sistema.EstadoAtual());
     
        sistema.Atualizar();


        if (Input.GetKeyDown("w"))
            sistema.ProcurarPropriedade("propriedade1").Valor++;
        if (Input.GetKeyDown("s"))
            sistema.ProcurarPropriedade("propriedade1").Valor--;
	}
}
