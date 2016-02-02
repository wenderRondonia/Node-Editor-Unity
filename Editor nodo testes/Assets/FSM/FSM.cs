using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

 namespace MaquinaDeEstados
{
       
        public class Propriedade<T> : Observador.IObservavel<T>
        {

            private string nome;
            private string descricao;
            private T valor;
            public string Nome
            {
                get { return nome; }

            }
            public string Descricao
            {
                get
                {
                    return descricao;
                }
            }
            public T Valor
            {
                get { return valor; }
                set 
                { 
                    valor = value;
                    NotificarObservadores();
             }
            }
            public Propriedade(string nomee, T t, string descricaoo = "")
            {
                valor = t;
                nome = nomee;
                descricao = descricaoo;
            }


            List<Observador.IObservador<T>> listaDeObservadores = new List<Observador.IObservador<T>>();
            public void RegistrarObservador(Observador.IObservador<T> observador)
            {
                listaDeObservadores.Add(observador);
            }

            public void DesRegistrarObservador(Observador.IObservador<T> observador)
            {
                listaDeObservadores.Remove(observador);
            }

            public void NotificarObservadores()
            {
                foreach (var observador in listaDeObservadores)
                {
                    observador.SerNotificado(valor);
                }

            }
        }
    
        public  class Sistema
        {
            public Estado estadoAtual = null;
            List<Estado> listaDeEstado = new List<Estado>();
            List<Transicao> listaDeTransicao = new List<Transicao>();
            List<Propriedade<float>> listaDePropriedade = new List<Propriedade<float>>();
            
            public Sistema()
            {
               
            }
            public void ImportarXML()
            {

            }
            public void ExportarXML()
            {

            }

            public void AdicionarPropriedade(Propriedade<float> prop)
            {
                listaDePropriedade.Add(prop);
            }
            public void RemoverPropriedade(string prop)
            {
                listaDePropriedade.Remove(ProcurarPropriedade(prop));
            }
            public Propriedade<float> ProcurarPropriedade(string prop)
            {
               return listaDePropriedade.Find(x => x.Nome == prop);
            }

            public string EstadoAtual()
            {
                if (estadoAtual != null)
                    return estadoAtual.nome;
                else
                    return "estado null!";
            }
            public void AdicionarEstado(Estado estado)
            {
                listaDeEstado.Add(estado);
            }
            public void RemoverEstado(string nome)
            {
                listaDeEstado.Remove( ProcurarEstado(nome));
            }
            public Estado ProcurarEstado(string nome)
            {
                return listaDeEstado.Find(x => x.nome == nome);
            }
            public void SetarEstadoInicial(string nome =null)
            {
                if (nome == null)
                    estadoAtual = listaDeEstado[0];
                else
                    estadoAtual = ProcurarEstado(nome);
            }


            public void AdicionarTransicao(Transicao transicao)
            {
                listaDeTransicao.Add(transicao);
            }
            public Transicao ProcurarTransicao(string nome)
            {
                return listaDeTransicao.Find(x => x.nome ==nome);
            }
            public void RemoverTransicao(string trans)
            {
                listaDeTransicao.Remove(ProcurarTransicao(trans));
            }
            public void TrocaDeEstado(Estado novoEstado)
            {
              //  Debug.Log("estado trocado de " + estadoAtual.nome +" para " + novoEstado.nome);
                estadoAtual = novoEstado;
            }
            public void TrocaDeEstado(string novoEstado)
            {
                estadoAtual = ProcurarEstado(novoEstado);
            }

            public void Atualizar()
            {
                estadoAtual.Atualizar();
                
            }
          
         }
        public class Estado
        {
            Sistema sistema;
            List<Transicao> listaDeTransicao;
            public string nome;
            bool iniciou = false;
            //public void AoAtualizar(){} em vez de atualizar a todo frame , muda os valores através ObserverPattern de eventos
            public Estado(string _nome, Sistema _sistema)
            {
                sistema = _sistema;
                listaDeTransicao = new List<Transicao>();
                nome = _nome;
            }
            public void AdicionarTransicao(Transicao transicao)
            {
                listaDeTransicao.Add(transicao);
            }
            public void RemoverTransicao(string nome)
            {
                listaDeTransicao.Remove(listaDeTransicao.Find( x => x.nome==nome ));
            }
            
            public void Atualizar()
            {
                if (iniciou == false)
                {
                    iniciou = true;
                    AoEntrar();
                    return;
                }

                AoAtualizar();
                

            }
            public virtual void AoEntrar()
            {

            }
            public virtual void AoSair()
            {

            }
            public virtual void AoAtualizar()
            {
            
            }
        
        }
        public class Condicao<T> : Observador.IObservador<T>
        {
          //  Propriedade<T> propriedadeObservada;
            T ultimoValor;
            public string nome;
            Func<T, bool> condicao;
            Transicao transicao;
            public Condicao(string _nome, Func<T, bool> cond, Transicao _transicao)
            {
                if (cond == null || _nome==null || String.IsNullOrEmpty(_nome) )
                {
                    Debug.LogError("valor  null");
                }
              //  propriedadeObservada = prop;
                nome = _nome;
                transicao = _transicao;
                condicao = cond;
                //TODO: tratar exceções!
       
                   
            }
    
            public bool ExecutarCondicao()
            {
                return condicao.Invoke(ultimoValor);
            }
            public void SerNotificado(T t)
            {
                ultimoValor = t;
                if(transicao.sistema.estadoAtual == transicao.estadoOrigem)  
                if( transicao.VerificarTodasAsCondicoes()==true)
                {
                    transicao.TransicaoConfirmada();
                }
                
            }
        }
        public class Transicao 
        {
            //TODO: Transicao pode herdar do IObserver e receber eventos do ISubject (propriedade herda de Ipropriedade e de ISubject)
            public string nome;
            public Estado estadoOrigem;
            public Estado estadoDestino;

            public Sistema sistema;
            List<Condicao<float>> ListaDeCondicoes = new List<Condicao<float>>();

            public Transicao(string _nome, Estado _estadoOrigem, Estado _estadoDestino,Sistema _sistema)
            {
                nome = _nome;
                sistema = _sistema;
                //TODO: lançar exceção
                if (_estadoOrigem == null || _estadoDestino == null)
                    return;
                estadoOrigem = _estadoOrigem;
                estadoDestino = _estadoDestino;

                estadoOrigem.AdicionarTransicao(this);
               
            }
   
            public bool VerificarTodasAsCondicoes()
            {
                foreach (var condicao in ListaDeCondicoes)
                    if (condicao.ExecutarCondicao() == false)
                        return false;
                return true;
            }
            public void TransicaoConfirmada()
            {
                sistema.ProcurarEstado(sistema.EstadoAtual()).AoSair();
                sistema.TrocaDeEstado(estadoDestino);
            }
            public void AdicionarCondicao(Condicao<float> condicao)
            {
             
                //verificar se as condicoes são válidos
                ListaDeCondicoes.Add(condicao);
            }
            public Condicao<float> ProcurarCondicao(string nome)
            {
                return ListaDeCondicoes.Find(x => x.nome ==nome);
            }
            public void RemoverCondicao(string nome)
            {
                ListaDeCondicoes.Remove(ProcurarCondicao(nome));
            }

        }

}



