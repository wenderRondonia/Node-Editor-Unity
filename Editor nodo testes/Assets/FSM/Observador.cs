using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Observador {



    public  interface IObservador<T>
    {
      
         void SerNotificado(T t);
       
    }

    public  interface IObservavel<T>
    {

         void RegistrarObservador(IObservador<T> observador);
         void DesRegistrarObservador(IObservador<T> observador);
         void NotificarObservadores();
 
    }
}
