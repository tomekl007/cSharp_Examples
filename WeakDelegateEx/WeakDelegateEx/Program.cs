using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace WeakDelegateEx
{
    class Program
    {
       

        static void Main(string[] args)
        {
           
        }
     }
    
    
    public class WeakDelegate<TDelegate> where TDelegate : class
    {  
        class MethodTarget  {   
            public readonly WeakReference Reference;   
            public readonly MethodInfo Method; 
            
            public MethodTarget (Delegate d)    {    
                Reference = new WeakReference (d.Target);    
                Method = d.Method;
                 }  
        }
        List<MethodTarget> _targets = new List<MethodTarget>();
        
        public WeakDelegate()  {    
            if (!typeof (TDelegate).IsSubclassOf (typeof (Delegate)))     
                throw new InvalidOperationException("TDelegate must be a delegate type");  
        } 
        public void Combine (TDelegate target)  {  
            if (target == null) return;  
            foreach (Delegate d in (target as Delegate).GetInvocationList())    
                _targets.Add (new MethodTarget (d));  
        }
        
        public void Remove (TDelegate target)  {  
            if (target == null) return;    
            foreach (Delegate d in (target as Delegate).GetInvocationList())   
            {      MethodTarget mt = _targets.Find (w =>
                                    d.Target.Equals (w.Reference.Target) &&      
                                    d.Method.MethodHandle.Equals (w.Method.MethodHandle));   
                if (mt != null) _targets.Remove (mt);    
            }  
        }
        
        public TDelegate Target  {   
            get    
            {      
                var deadRefs = new List<MethodTarget>();   
                Delegate combinedTarget = null;      
                foreach (MethodTarget mt in _targets.ToArray()) {   
                    WeakReference target = mt.Reference;       
                    if (target != null && target.IsAlive){      
                        var newDelegate = Delegate.CreateDelegate (          
                            typeof (TDelegate), mt.Reference.Target, mt.Method);      
                        combinedTarget = Delegate.Combine (combinedTarget, newDelegate);     
                    }        else          
                        deadRefs.Add (mt);      
                }
                foreach (MethodTarget mt in deadRefs)   // Remove dead references      
                    _targets.Remove (mt);                 // from _targets
                return combinedTarget as TDelegate;
            }
            set { _targets.Clear(); Combine(value); }
        }
    }

    
    public class Foo { 
        WeakDelegate<EventHandler> _click = new WeakDelegate<EventHandler>(); 
        public event EventHandler Click { 
            add { _click.Combine(value); } 
            remove { _click.Remove(value); }
        } 
        protected virtual void OnClick(EventArgs e) {
            EventHandler target = _click.Target;
            if (target != null) 
                target(this, e); } 
    }
}
