import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class SharedService {
  readonly APIUrl="http://localhost:5000/api";

  constructor(private http:HttpClient) { }

  getCaixaStatus():Observable<any[]>{
    return this.http.get<any>(this.APIUrl+'/caixa/getstatus');
  }
  getCaixaSignalStatus():Observable<any[]>{
    return this.http.get<any>(this.APIUrl+'/sgca/getsignalstatus');
  }
  getCaixaAtivo():Observable<any[]>{
    return this.http.get<any>(this.APIUrl+'/caixa/getcaixaativo');
  }
  
  getStatus():Observable<any[]>{
    return this.http.get<any>(this.APIUrl+'/getstatus');
  }

  addCaixa(val:any){
    return this.http.post(this.APIUrl+'/Caixa',val); 
  }
  
  updateCaixa(val:any){
    return this.http.put(this.APIUrl+'/Caixa',val); 
  }
  
  deleteCaixa(val:any){
    return this.http.delete(this.APIUrl+'/Caixa'+val); 
  }

  saqueCaixa(val:any){
    return this.http.put(this.APIUrl+'/caixa/saque',val); 
  }
}
